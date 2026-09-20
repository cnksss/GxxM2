using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmMagicCD.pas（583 行）的纯逻辑与窗体测试。
/// 重点：
///   * DoOpen 的三重过滤（`MagicID > 0`、Descr 排除「英雄/静之/怒之」且 SameText 大小写不敏感、MagicID 去重）；
///   * CD 回填来自 `g_MagicCDList.Find(MagicID)`，未命中为 0；
///   * `btnSearchClick` 子串匹配**区分大小写**，`btnSearchNextClick` **不回绕**；
///   * `RebuildMagicCdList` 的 Add 返回 nil（重复）时跳过 Interval 赋值；
///   * INI 6 键与 g_MagicCDList 落盘。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormMagicCDTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormMagicCDTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "p2rg_form_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        FormGlobals.ResetForTest();
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    private string IniPath => Path.Combine(_dir, "Config.ini");
    private string CdListFile => Path.Combine(_dir, "MagicCD.ini");
    private string IniText => File.Exists(IniPath) ? File.ReadAllText(IniPath, System.Text.Encoding.GetEncoding(936)) : "";

    /// <summary>测试用 IMagicDbReader：按给定行集合顺序迭代。</summary>
    private class FakeReader : IMagicDbReader
    {
        private readonly List<(int MagId, string Descr, string MagName)> _rows;
        private int _index;

        public FakeReader(params (int MagId, string Descr, string MagName)[] rows) => _rows = new List<(int, string, string)>(rows);

        public int RecordCount => _rows.Count;
        public void First() => _index = 0;
        public bool Next() => ++_index < _rows.Count;
        public int GetMagId() => _rows[_index].MagId;
        public string GetDescr() => _rows[_index].Descr;
        public string GetMagName() => _rows[_index].MagName;
    }

    // ---------------- IsDescrAllowed（原 :367-369）----------------

    [Theory]
    [InlineData("英雄")]
    [InlineData("静之")]
    [InlineData("怒之")]
    public void IsDescrAllowed_三个排除词被拒(string descr)
        => Assert.False(MagicCDLogic.IsDescrAllowed(descr));

    [Fact]
    public void IsDescrAllowed_大小写不敏感对中文无影响但对英文有()
    {
        // SameText = Trim + OrdinalIgnoreCase；这三个词是中文，但前后空格会被忽略
        Assert.False(MagicCDLogic.IsDescrAllowed("  英雄  "));
        Assert.False(MagicCDLogic.IsDescrAllowed("静之 "));
    }

    [Theory]
    [InlineData("火球术")]
    [InlineData("")]
    [InlineData("英雄2")]
    [InlineData("大英雄")]
    public void IsDescrAllowed_其它值通过(string descr)
        => Assert.True(MagicCDLogic.IsDescrAllowed(descr));

    // ---------------- ReadMagicRows（原 :337-398）----------------

    [Fact]
    public void ReadMagicRows_MagicID为0或负被跳过()
    {
        var reader = new FakeReader((0, "", "零"), (-1, "", "负"), (5, "", "正"));
        var rows = MagicCDLogic.ReadMagicRows(reader, new TMagicIntervalList());

        Assert.Single(rows);
        Assert.Equal(5, rows[0].MagicID);
        Assert.Equal("正", rows[0].MagicName);
    }

    [Fact]
    public void ReadMagicRows_排除三个Descr()
    {
        var reader = new FakeReader(
            (1, "英雄", "A"), (2, "静之", "B"), (3, "怒之", "C"), (4, "普通", "D"));
        var rows = MagicCDLogic.ReadMagicRows(reader, new TMagicIntervalList());

        Assert.Single(rows);
        Assert.Equal(4, rows[0].MagicID);
    }

    [Fact]
    public void ReadMagicRows_重复MagicID只保留首次()
    {
        var reader = new FakeReader((1, "", "first"), (1, "", "second"), (2, "", "other"));
        var rows = MagicCDLogic.ReadMagicRows(reader, new TMagicIntervalList());

        Assert.Equal(2, rows.Count);
        Assert.Equal("first", rows[0].MagicName);   // 首次出现的名字
        Assert.Equal(2, rows[1].MagicID);
    }

    [Fact]
    public void ReadMagicRows_CDTime回填自g_MagicCDList_未命中为0()
    {
        var list = new TMagicIntervalList();
        list.Add(7).Interval = 1500;
        list.Add(9).Interval = 250;

        var reader = new FakeReader((7, "", "has"), (8, "", "none"), (9, "", "has2"));
        var rows = MagicCDLogic.ReadMagicRows(reader, list);

        Assert.Equal(3, rows.Count);
        Assert.Equal(1500u, rows[0].CDTime);      // 原 :381
        Assert.Equal(0u, rows[1].CDTime);         // 原 :383
        Assert.Equal(250u, rows[2].CDTime);
    }

    [Fact]
    public void ReadMagicRows_空数据库返回空()
        => Assert.Empty(MagicCDLogic.ReadMagicRows(new FakeReader(), new TMagicIntervalList()));

    [Fact]
    public void ReadMagicRows_Descr字段缺失时异常向上抛_原文无tryexcept()
    {
        // 接缝层面的等价：读取器抛异常 → ReadMagicRows 不吞
        var ex = Record.Exception(() => MagicCDLogic.ReadMagicRows(new ThrowingReader(), new TMagicIntervalList()));
        Assert.IsType<InvalidOperationException>(ex);
    }

    private class ThrowingReader : IMagicDbReader
    {
        public int RecordCount => 1;
        public void First() { }
        public bool Next() => false;
        public int GetMagId() => 1;
        public string GetDescr() => throw new InvalidOperationException("字段不存在");
        public string GetMagName() => "";
    }

    // ---------------- GetCellText / CanEditColumn / 隔行底色（原 :314-417）----------------

    [Fact]
    public void GetCellText_三列规则()
    {
        var row = new MagicCDRow { MagicID = 42, MagicName = "火球", CDTime = 1200 };
        Assert.Equal("42", MagicCDLogic.GetCellText(row, 0));       // 原 :322
        Assert.Equal("火球", MagicCDLogic.GetCellText(row, 1));      // 原 :323
        Assert.Equal("1200", MagicCDLogic.GetCellText(row, 2));      // 原 :324
        Assert.Equal("", MagicCDLogic.GetCellText(row, 3));          // 无 default 赋值
    }

    [Fact]
    public void CanEditColumn_仅第2列可编辑()
    {
        Assert.False(MagicCDLogic.CanEditColumn(0));
        Assert.False(MagicCDLogic.CanEditColumn(1));
        Assert.True(MagicCDLogic.CanEditColumn(2));                 // 原 :422
        Assert.False(MagicCDLogic.CanEditColumn(3));
    }

    [Fact]
    public void ShouldEraseAltRow_奇数行使用F9F9F9()
    {
        Assert.False(MagicCDLogic.ShouldEraseAltRow(0));            // 原 :412 `mod 2 <> 0`
        Assert.True(MagicCDLogic.ShouldEraseAltRow(1));
        Assert.False(MagicCDLogic.ShouldEraseAltRow(2));
        Assert.True(MagicCDLogic.ShouldEraseAltRow(3));
        Assert.Equal(System.Drawing.Color.FromArgb(0xF9, 0xF9, 0xF9), MagicCDLogic.AltRowColor);   // 原 :415 $00F9F9F9
    }

    // ---------------- FindFirst（原 :506-535）----------------

    [Fact]
    public void FindFirst_子串匹配且区分大小写()
    {
        var rows = new List<MagicCDRow>
        {
            new MagicCDRow { MagicName = "FireBall" },
            new MagicCDRow { MagicName = "IceBolt" }
        };

        Assert.Equal(0, MagicCDLogic.FindFirst(rows, "Fire"));
        Assert.Equal(0, MagicCDLogic.FindFirst(rows, "Ball"));
        Assert.Equal(1, MagicCDLogic.FindFirst(rows, "Ice"));
        Assert.Equal(-1, MagicCDLogic.FindFirst(rows, "fire"));      // ★ 区分大小写，与 ProcessBlacklist 不同
        Assert.Equal(-1, MagicCDLogic.FindFirst(rows, "zzz"));
    }

    [Fact]
    public void FindFirst_搜索文本先Trim()
    {
        var rows = new List<MagicCDRow> { new MagicCDRow { MagicName = "火球术" } };
        Assert.Equal(0, MagicCDLogic.FindFirst(rows, "  火球  "));
    }

    [Fact]
    public void FindFirst_空搜索文本返回负1_由调用方弹窗()
    {
        var rows = new List<MagicCDRow> { new MagicCDRow { MagicName = "x" } };
        Assert.Equal(-1, MagicCDLogic.FindFirst(rows, ""));
        Assert.Equal(-1, MagicCDLogic.FindFirst(rows, "   "));
    }

    [Fact]
    public void FindFirst_返回首个命中()
    {
        var rows = new List<MagicCDRow>
        {
            new MagicCDRow { MagicName = "ab" },
            new MagicCDRow { MagicName = "abc" }
        };
        Assert.Equal(0, MagicCDLogic.FindFirst(rows, "ab"));
    }

    // ---------------- FindNext（原 :537-571，不回绕）----------------

    [Fact]
    public void FindNext_从当前项下一个开始()
    {
        var rows = new List<MagicCDRow>
        {
            new MagicCDRow { MagicName = "hitA" },
            new MagicCDRow { MagicName = "miss" },
            new MagicCDRow { MagicName = "hitB" }
        };

        // 焦点在 0 → 从 1 开始 → 命中 2
        Assert.Equal(2, MagicCDLogic.FindNext(rows, 0, "hit"));
        // 焦点在 2（末项）→ 从 3 开始 → **不回绕** → -1
        Assert.Equal(-1, MagicCDLogic.FindNext(rows, 2, "hit"));
        // 无焦点 → 从 0 开始 → 0
        Assert.Equal(0, MagicCDLogic.FindNext(rows, -1, "hit"));
    }

    [Fact]
    public void FindNext_空搜索文本返回负1()
    {
        var rows = new List<MagicCDRow> { new MagicCDRow { MagicName = "x" } };
        Assert.Equal(-1, MagicCDLogic.FindNext(rows, -1, ""));
    }

    [Fact]
    public void FindNext_与FindFirst不同_不回绕_差异断言()
    {
        var rows = new List<MagicCDRow> { new MagicCDRow { MagicName = "hit" } };
        // FindFirst 恒从 0 开始 → 命中
        Assert.Equal(0, MagicCDLogic.FindFirst(rows, "hit"));
        // FindNext 从 0+1 开始 → 越过末尾 → -1（uFrmProcessBlacklist 的 NextSearchStart 会回绕，此处不会）
        Assert.Equal(-1, MagicCDLogic.FindNext(rows, 0, "hit"));
    }

    // ---------------- ApplyGlobalsFromUi / WriteIni（原 :463-480）----------------

    [Fact]
    public void ApplyGlobalsFromUi_写六个全局量()
    {
        MagicCDLogic.ApplyGlobalsFromUi(1, "MSG", 0x11, 0x22, 33, 44);

        Assert.Equal(1, FormGlobals.g_btMagicCDMsgType);
        Assert.Equal("MSG", FormGlobals.g_sMagicCDMsgText);
        Assert.Equal(0x11, FormGlobals.g_btMagicCDFColor);
        Assert.Equal(0x22, FormGlobals.g_btMagicCDBColor);
        Assert.Equal(33, FormGlobals.g_nMagicCDShowX);
        Assert.Equal(44, FormGlobals.g_nMagicCDShowY);
    }

    [Fact]
    public void ApplyGlobalsFromUi_null文本变空串()
    {
        MagicCDLogic.ApplyGlobalsFromUi(0, null, 0, 0, 0, 0);
        Assert.Equal("", FormGlobals.g_sMagicCDMsgText);
    }

    [Fact]
    public void WriteIni_MagicCD节六键顺序与键名逐字节()
    {
        FormGlobals.g_btMagicCDMsgType = 2;
        FormGlobals.g_sMagicCDMsgText = "T";
        FormGlobals.g_btMagicCDFColor = 255;
        FormGlobals.g_btMagicCDBColor = 56;
        FormGlobals.g_nMagicCDShowX = 30;
        FormGlobals.g_nMagicCDShowY = 40;

        MagicCDLogic.WriteIni(IniPath);

        string expected =
            "[MagicCD]\r\n" +
            "MsgType=2\r\n" +
            "MsgText=T\r\n" +
            "FColor=255\r\n" +
            "BColor=56\r\n" +
            "ShowX=30\r\n" +
            "ShowY=40\r\n" +
            "\r\n";
        Assert.Equal(expected, IniText);
    }

    // ---------------- RebuildMagicCdList（原 :483-501）----------------

    [Fact]
    public void RebuildMagicCdList_清空后逐行加入并写Interval()
    {
        var list = new TMagicIntervalList();
        list.Add(99).Interval = 999;                       // 旧数据应被清掉

        var rows = new[]
        {
            new MagicCDRow { MagicID = 1, CDTime = 100 },
            new MagicCDRow { MagicID = 2, CDTime = 200 }
        };
        var skipped = MagicCDLogic.RebuildMagicCdList(rows, list);

        Assert.Empty(skipped);
        Assert.Equal(2, list.Count);
        Assert.Equal(100u, list.Find(1).Interval);
        Assert.Equal(200u, list.Find(2).Interval);
        Assert.Null(list.Find(99));                        // 已被 Clear
    }

    [Fact]
    public void RebuildMagicCdList_重复MagicID被跳过并记入skipped()
    {
        var list = new TMagicIntervalList();
        var rows = new[]
        {
            new MagicCDRow { MagicID = 5, CDTime = 10 },
            new MagicCDRow { MagicID = 5, CDTime = 20 }    // 重复
        };
        var skipped = MagicCDLogic.RebuildMagicCdList(rows, list);

        Assert.Single(skipped);
        Assert.Equal(5, skipped[0]);
        Assert.Equal(1, list.Count);
        Assert.Equal(10u, list.Find(5).Interval);          // 首次的 Interval 保留
    }

    [Fact]
    public void RebuildMagicCdList_空行集合清空列表()
    {
        var list = new TMagicIntervalList();
        list.Add(1).Interval = 1;
        MagicCDLogic.RebuildMagicCdList(Array.Empty<MagicCDRow>(), list);
        Assert.Equal(0, list.Count);
    }

    // ---------------- ButtonOK（原 :456-504）----------------

    [Fact]
    public void ButtonOK_写全局写INI并落盘CD列表()
    {
        FormGlobals.g_MagicCDList.Add(1).Interval = 5;
        var rows = new[] { new MagicCDRow { MagicID = 11, CDTime = 1234 } };

        MagicCDLogic.ButtonOK(rows, IniPath, CdListFile, 3, "冷却中", 10, 20, 30, 40);

        Assert.Equal(3, FormGlobals.g_btMagicCDMsgType);
        Assert.Equal("冷却中", FormGlobals.g_sMagicCDMsgText);
        Assert.Equal(10, FormGlobals.g_btMagicCDFColor);
        Assert.Equal(20, FormGlobals.g_btMagicCDBColor);
        Assert.Equal(30, FormGlobals.g_nMagicCDShowX);
        Assert.Equal(40, FormGlobals.g_nMagicCDShowY);

        Assert.Contains("MsgType=3\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("ShowY=40\r\n", IniText, StringComparison.Ordinal);

        Assert.True(File.Exists(CdListFile));
        string cdText = File.ReadAllText(CdListFile, System.Text.Encoding.GetEncoding(936));
        Assert.Contains("[Interval]\r\n", cdText, StringComparison.Ordinal);
        Assert.Contains("11=1234\r\n", cdText, StringComparison.Ordinal);
    }

    // ---------------- ComputeDefaultMagicDbPath（原 :278-279）----------------

    [Fact]
    public void ComputeDefaultMagicDbPath_exe目录的上一级加Mud2路径()
    {
        string path = MagicCDLogic.ComputeDefaultMagicDbPath(@"C:\Game\RunGate\RunGate.exe");
        Assert.Equal(System.IO.Path.Combine(@"C:\Game", "Mud2", "DB", "Magic.DB"), path);
    }

    [Fact]
    public void ComputeDefaultMagicDbPath_空输入不抛异常()
    {
        var ex = Record.Exception(() => MagicCDLogic.ComputeDefaultMagicDbPath(null));
        Assert.Null(ex);
    }

    // ---------------- 窗体（DFM 对齐 + 端到端）----------------

    [Fact]
    public void 窗体_DFM属性与列宽对齐()
    {
        using var f = new FrmMagicCD();

        Assert.Equal("技能CD设置", f.Text);                       // DFM: Caption
        Assert.Equal(369, f.ClientSize.Width);                    // DFM: ClientWidth=369
        Assert.Equal(429, f.ClientSize.Height);                   // DFM: ClientHeight=429

        Assert.NotNull(f.lbl4);
        Assert.NotNull(f.vstMagicCD);
        Assert.NotNull(f.btnOK);
        Assert.NotNull(f.GroupBox1);
        Assert.NotNull(f.lbl1);
        Assert.NotNull(f.lbl2);
        Assert.NotNull(f.edtMagicCDMsgText);
        Assert.NotNull(f.cbbMagicCDMsgType);
        Assert.NotNull(f.seMagicCDFColor);
        Assert.NotNull(f.seMagicCDBColor);
        Assert.NotNull(f.seMagicCDShowX);
        Assert.NotNull(f.seMagicCDShowY);
        Assert.NotNull(f.edtMagicName);
        Assert.NotNull(f.btnSearch);
        Assert.NotNull(f.btnSearchNext);
        Assert.NotNull(f.dlgOpen1);

        Assert.Equal("技能：", f.lbl4.Text);
        Assert.Equal("内容：", f.lbl1.Text);
        Assert.Equal("位置：", f.lbl2.Text);
        Assert.Equal("技能冷确时间未到提示", f.GroupBox1.Text);
        Assert.Equal("确定", f.btnOK.Text);
        Assert.Equal("搜索", f.btnSearch.Text);
        Assert.Equal("搜索下一个", f.btnSearchNext.Text);

        // DFM: Columns=[技能ID(60), 技能名称(140), 冷确时间 [毫秒](149)]
        Assert.Equal(3, f.vstMagicCD.Columns.Count);
        Assert.Equal("技能ID", f.vstMagicCD.Columns[0].Text);
        Assert.Equal(60, f.vstMagicCD.Columns[0].Width);
        Assert.Equal("技能名称", f.vstMagicCD.Columns[1].Text);
        Assert.Equal(140, f.vstMagicCD.Columns[1].Width);
        Assert.Equal("冷确时间 [毫秒]", f.vstMagicCD.Columns[2].Text);
        Assert.Equal(149, f.vstMagicCD.Columns[2].Width);
    }

    [Fact]
    public void 窗体_DoOpenWithReader回填提示参数与行()
    {
        FormGlobals.g_btMagicCDMsgType = 2;
        FormGlobals.g_sMagicCDMsgText = "M";
        FormGlobals.g_btMagicCDFColor = 11;
        FormGlobals.g_btMagicCDBColor = 22;
        FormGlobals.g_nMagicCDShowX = 1;
        FormGlobals.g_nMagicCDShowY = 2;
        FormGlobals.g_MagicCDList.Add(3).Interval = 777;

        using var f = new FrmMagicCD();
        f.FormCreate(f, EventArgs.Empty);
        f.DoOpenWithReader(new FakeReader((3, "普通", "火球"), (4, "英雄", "排除")));

        Assert.Equal(2, f.cbbMagicCDMsgType.SelectedIndex);
        Assert.Equal("M", f.edtMagicCDMsgText.Text);
        Assert.Equal(11, f.seMagicCDFColor.ColorIndex);
        Assert.Equal(22, f.seMagicCDBColor.ColorIndex);
        Assert.Equal(1, f.seMagicCDShowX.Value);
        Assert.Equal(2, f.seMagicCDShowY.Value);

        Assert.Single(f.Rows);
        Assert.Equal(3, f.Rows[0].MagicID);
        Assert.Equal("火球", f.Rows[0].MagicName);
        Assert.Equal(777u, f.Rows[0].CDTime);
        Assert.Single(f.vstMagicCD.Items);
        Assert.Equal("3", f.vstMagicCD.Items[0].Text);
        Assert.Equal("火球", f.vstMagicCD.Items[0].SubItems[1].Text);
        Assert.Equal("777", f.vstMagicCD.Items[0].SubItems[2].Text);
    }

    [Fact]
    public void 窗体_DoOpen文件版未接线时抛NotSupported()
    {
        using var f = new FrmMagicCD();
        var ex = Record.Exception(() => f.DoOpen("Magic.DB"));
        Assert.IsType<NotSupportedException>(ex);
    }

    [Fact]
    public void 窗体_btnOK_Click写INI与CD列表()
    {
        FormGlobals.g_sIniFileName = IniPath;
        FormGlobals.g_MagicCDListFileName = CdListFile;

        using var f = new FrmMagicCD();
        f.FormCreate(f, EventArgs.Empty);
        f.cbbMagicCDMsgType.SelectedIndex = 1;      // 下拉项在构造时已填充（FrmMagicCD.MagicCDMsgTypeItems）
        f.edtMagicCDMsgText.Text = "冷却提示";
        f.seMagicCDFColor.ColorIndex = 7;
        f.seMagicCDBColor.ColorIndex = 8;
        f.seMagicCDShowX.Value = 5;
        f.seMagicCDShowY.Value = 6;
        f.SetRows(new[]
        {
            new MagicCDRow { MagicID = 1, MagicName = "A", CDTime = 100 },
            new MagicCDRow { MagicID = 2, MagicName = "B", CDTime = 200 }
        });

        f.btnOK_Click(f, EventArgs.Empty);

        Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);
        Assert.Equal(1, FormGlobals.g_btMagicCDMsgType);
        Assert.Equal("冷却提示", FormGlobals.g_sMagicCDMsgText);
        Assert.Equal(7, FormGlobals.g_btMagicCDFColor);
        Assert.Equal(8, FormGlobals.g_btMagicCDBColor);
        Assert.Equal(2, FormGlobals.g_MagicCDList.Count);
        Assert.Equal(100u, FormGlobals.g_MagicCDList.Find(1).Interval);
        Assert.Equal(200u, FormGlobals.g_MagicCDList.Find(2).Interval);
        Assert.Contains("MsgText=冷却提示\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void 窗体_btnSearch_Click空文本弹窗()
    {
        var shown = new List<string>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };

            using var f = new FrmMagicCD();
            f.FormCreate(f, EventArgs.Empty);
            f.edtMagicName.Text = "   ";
            f.btnSearch_Click(f, EventArgs.Empty);

            Assert.Single(shown);
            Assert.Equal("搜索内容不能为空", shown[0]);       // 原 :515
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_btnSearch_Click与btnSearchNext_Click定位行为()
    {
        using var f = new FrmMagicCD();
        f.FormCreate(f, EventArgs.Empty);
        f.SetRows(new[]
        {
            new MagicCDRow { MagicID = 1, MagicName = "hitA" },
            new MagicCDRow { MagicID = 2, MagicName = "miss" },
            new MagicCDRow { MagicID = 3, MagicName = "hitB" }
        });
        f.edtMagicName.Text = "hit";

        f.btnSearch_Click(f, EventArgs.Empty);
        Assert.Equal(0, f.FocusedRow);

        // 搜索下一个：从 0 的下一个开始 → 命中 2（原 :557-570；**不回绕**）
        f.btnSearchNext_Click(f, EventArgs.Empty);
        Assert.Equal(2, f.FocusedRow);

        // 再点一次：从 2 的下一个开始 → 越过末尾 → 焦点不变
        f.btnSearchNext_Click(f, EventArgs.Empty);
        Assert.Equal(2, f.FocusedRow);
    }

    [Fact]
    public void 窗体_edtMagicNameKeyDown回车且有文本触发搜索()
    {
        var original = MessageBoxSeam.Show;
        try
        {
            int popups = 0;
            MessageBoxSeam.Show = (t, c, b, i) => { popups++; return System.Windows.Forms.DialogResult.OK; };

            using var f = new FrmMagicCD();
            f.FormCreate(f, EventArgs.Empty);
            f.SetRows(new[] { new MagicCDRow { MagicID = 1, MagicName = "x" } });

            // 空文本：不触发搜索（因此不弹"搜索内容不能为空"）
            f.edtMagicName.Text = "   ";
            f.edtMagicName_KeyDown(f, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Return));
            Assert.Equal(0, popups);

            // 非回车：不触发
            f.edtMagicName.Text = "x";
            f.edtMagicName_KeyDown(f, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.A));
            Assert.Equal(0, popups);

            // 回车 + 非空：触发搜索（命中，无弹窗）
            f.edtMagicName_KeyDown(f, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Return));
            Assert.Equal(0, popups);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_vstMagicCDKeyDown回车时开始编辑()
    {
        using var f = new FrmMagicCD();
        f.FormCreate(f, EventArgs.Empty);
        f.SetRows(new[] { new MagicCDRow { MagicID = 1, MagicName = "x" } });
        f.SetFocusedRow(0);

        var ex = Record.Exception(() => f.vstMagicCD_KeyDown(f,
            new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Return)));
        Assert.Null(ex);   // 原 :452 EditNode(FocusedNode, 2)
    }

    [Fact]
    public void 窗体_SetRows_ReadRows往返()
    {
        using var f = new FrmMagicCD();
        f.FormCreate(f, EventArgs.Empty);
        f.SetRows(new[] { new MagicCDRow { MagicID = 7, MagicName = "N", CDTime = 88 } });

        var rows = f.ReadRows();
        Assert.Single(rows);
        Assert.Equal(7, rows[0].MagicID);
        Assert.Equal("N", rows[0].MagicName);
        Assert.Equal(88u, rows[0].CDTime);
    }

    [Fact]
    public void 窗体_ReadRows把用户编辑后的文本读回()
    {
        using var f = new FrmMagicCD();
        f.FormCreate(f, EventArgs.Empty);
        f.SetRows(new[] { new MagicCDRow { MagicID = 7, MagicName = "N", CDTime = 88 } });

        f.vstMagicCD.Items[0].SubItems[2].Text = "999";       // 模拟单元格编辑
        var rows = f.ReadRows();

        Assert.Equal(999u, rows[0].CDTime);
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>基础接缝（DelphiDate / DelphiRandom / DelphiStrUtils / TStringsHelper / ListViewSink / SpinControls）的单元测试。</summary>
public class SeamTests : TempDirTest
{
    // ---------------- DelphiDate ----------------

    [Fact]
    public void EncodeDate_Delphi零点为1899年12月30日()
    {
        Assert.Equal(0.0, DelphiDate.EncodeDate(1899, 12, 30));
        Assert.Equal(1.0, DelphiDate.EncodeDate(1899, 12, 31));
        Assert.Equal(2.0, DelphiDate.EncodeDate(1900, 1, 1));
    }

    [Fact]
    public void EncodeDate_非法日期抛EConvertError()
    {
        Assert.Throws<EConvertError>(() => DelphiDate.EncodeDate(0, 1, 1));
        Assert.Throws<EConvertError>(() => DelphiDate.EncodeDate(10000, 1, 1));
        Assert.Throws<EConvertError>(() => DelphiDate.EncodeDate(2025, 0, 1));
        Assert.Throws<EConvertError>(() => DelphiDate.EncodeDate(2025, 13, 1));
        Assert.Throws<EConvertError>(() => DelphiDate.EncodeDate(2025, 2, 30));
        Assert.Throws<EConvertError>(() => DelphiDate.EncodeDate(2025, 1, 0));
    }

    [Fact]
    public void DecodeDate_往返一致()
    {
        double d = DelphiDate.EncodeDate(2025, 3, 15);
        DelphiDate.DecodeDate(d, out ushort y, out ushort m, out ushort day);
        Assert.Equal((ushort)2025, y);
        Assert.Equal((ushort)3, m);
        Assert.Equal((ushort)15, day);
    }

    [Fact]
    public void DecodeTime_取小数部分()
    {
        double t = 0.5;                                   // 12:00:00.000
        DelphiDate.DecodeTime(t, out ushort h, out ushort mi, out ushort s, out ushort ms);
        Assert.Equal((ushort)12, h);
        Assert.Equal((ushort)0, mi);
        Assert.Equal((ushort)0, s);
        Assert.Equal((ushort)0, ms);

        DelphiDate.DecodeTime(0.25 + 0.0000115740740740741, out h, out mi, out s, out ms);
        Assert.Equal((ushort)6, h);
        Assert.Equal((ushort)0, mi);
        Assert.Equal((ushort)1, s);
    }

    [Fact]
    public void FromDateTime与ToDateTime互逆()
    {
        var dt = new DateTime(2025, 3, 15, 6, 30, 0);
        double tdt = DelphiDate.FromDateTime(dt);
        // double 天数换算存在亚毫秒误差，按 1ms 容差比较
        Assert.True(Math.Abs((DelphiDate.ToDateTime(tdt) - dt).TotalMilliseconds) < 1.0);
    }

    [Fact]
    public void Date返回整数天()
    {
        Assert.Equal(Math.Floor(DelphiDate.Now()), DelphiDate.Date());
    }

    // ---------------- DelphiRandom ----------------

    [Fact]
    public void Random_范围0时返回0_Delphi语义()
    {
        int calls = 0;
        Func<int, int> saved = DelphiRandom.Next;
        try
        {
            DelphiRandom.Next = _ => { calls++; return 5; };
            Assert.Equal(0, DelphiRandom.Random(0));
            Assert.Equal(0, calls);            // Range=0 不会调用底层取数
            Assert.Equal(5, DelphiRandom.Random(7));
            Assert.Equal(1, calls);
        }
        finally
        {
            DelphiRandom.Next = saved;
        }
    }

    // ---------------- DelphiStrUtils ----------------

    [Fact]
    public void SameText_忽略大小写()
    {
        Assert.True(DelphiStrUtils.SameText("AbC", "aBc"));
        Assert.False(DelphiStrUtils.SameText("abc", "abd"));
        Assert.True(DelphiStrUtils.SameText("", ""));
        Assert.True(DelphiStrUtils.SameText(null, ""));
    }

    [Fact]
    public void CompareText_返回符号()
    {
        Assert.Equal(0, DelphiStrUtils.CompareText("a", "A"));
        Assert.Equal(-1, DelphiStrUtils.CompareText("a", "b"));
        Assert.Equal(1, DelphiStrUtils.CompareText("b", "a"));
    }

    // ---------------- TStringsHelper ----------------

    [Fact]
    public void ValueFromIndex_取等号之后_无等号返回空串()
    {
        Assert.Equal("v", TStringsHelper.ValueFromIndex("k=v"));
        Assert.Equal("", TStringsHelper.ValueFromIndex("kv"));
        Assert.Equal("a=b", TStringsHelper.ValueFromIndex("k=a=b"));
    }

    // ---------------- TShortString15 / TBool ----------------

    [Fact]
    public void Truncate_短串原样返回()
    {
        Assert.Equal("abc", TShortString15.Truncate("abc", 15));
        Assert.Equal("", TShortString15.Truncate(null, 15));
        Assert.Equal("", TShortString15.Truncate("", 15));
    }

    [Fact]
    public void TBool_按字节与整数判定()
    {
        Assert.Equal((byte)1, TBool.ToByte(true));
        Assert.Equal((byte)0, TBool.ToByte(false));
        Assert.True(TBool.ToBool((byte)2));
        Assert.False(TBool.ToBool((byte)0));
        Assert.True(TBool.ToBool(-1));
    }

    // ---------------- TIniFile 其余 API ----------------

    [Fact]
    public void ReadSections_按文件顺序返回节名()
    {
        var ini = new TIniFile(Path2("sections.ini"));
        ini.WriteString("B", "k", "1");
        ini.WriteString("A", "k", "1");

        var sl = new TStringList();
        ini.ReadSections(sl);

        Assert.Equal(2, sl.Count);
        Assert.Equal("B", sl[0]);
        Assert.Equal("A", sl[1]);
        Assert.True(ini.SectionExists("A"));
        Assert.False(ini.SectionExists("C"));
        Assert.True(ini.ValueExists("A", "k"));
        Assert.False(ini.ValueExists("A", "x"));
    }

    [Fact]
    public void UpdateFile_可显式重写且内容不变()
    {
        var ini = new TIniFile(Path2("u.ini"));
        ini.WriteString("S", "k", "v");
        string a = System.IO.File.ReadAllText(ini.FileName);
        ini.UpdateFile();
        Assert.Equal(a, System.IO.File.ReadAllText(ini.FileName));
    }

    // ---------------- ListViewSink ----------------

    [Fact]
    public void ListViewSink_写行与挂载Tag_SubItems偏移一()
    {
        using var lv = new ListView();
        var sink = new ListViewSink(lv);
        var tag = new object();

        sink.AddRow("cap", tag, "s1", "s2");

        Assert.Equal(1, sink.Count);
        Assert.Equal("cap", lv.Items[0].Text);
        Assert.Equal("s1", lv.Items[0].SubItems[1].Text);   // SubItems[0] 即 Caption
        Assert.Equal("s2", lv.Items[0].SubItems[2].Text);
        Assert.Same(tag, sink.GetTag(0));

        sink.Clear();
        Assert.Equal(0, sink.Count);
    }

    [Fact]
    public void MemoryListViewSink_与ListViewSink语义一致()
    {
        var sink = new MemoryListViewSink();
        var tag = new object();
        sink.AddRow("cap", tag, "s1");

        Assert.Equal(1, sink.Count);
        Assert.Equal("cap", sink.Rows[0].Caption);
        Assert.Equal(new[] { "s1" }, sink.Rows[0].SubItems);
        Assert.Same(tag, sink.GetTag(0));
    }

    // ---------------- TSpinEdit ----------------

    [Fact]
    public void TSpinEdit_编程赋值不裁剪_上下按钮按DFM范围裁剪()
    {
        using var spin = new TSpinEdit();
        spin.SetDfmRange(0, 0);                 // AddrEdit.dfm: ERowCount MaxValue=0 MinValue=0
        spin.Value = 8;
        Assert.Equal(8, spin.Value);            // Delphi TSpinEdit 编程赋值不受 Max/Min 限制

        spin.SetDfmRange(1, 8);
        spin.Value = 100;
        Assert.Equal(100, spin.Value);

        spin.UpButton();
        Assert.Equal(100, spin.Value);          // 已到 DFM 上限 → 不再增加
        spin.DownButton();
        Assert.Equal(99, spin.Value);
    }

    [Fact]
    public void TSpinEdit_Value为int而非decimal()
    {
        using var spin = new TSpinEdit();
        spin.Value = 5;
        int v = spin.Value;                    // 编译期即证明 Value 是 int（decimal 无法隐式转 int）
        Assert.Equal(5, v);
        Assert.Equal(-3, new TSpinEdit { Value = -3 }.Value);
    }

    // ---------------- FdbExploreSeam 默认值 ----------------

    [Fact]
    public void Timer1Timer_接缝默认值不误删_全部阈值与开关为0()
    {
        TestReset.All();
        Assert.Equal((byte)0, FdbExploreSeam.boAutoClearDB);
        Assert.Equal(0, FdbExploreSeam.nLevel1);
        Assert.Equal(0, FdbExploreSeam.nLevel2);
        Assert.Equal(0, FdbExploreSeam.nLevel3);
        Assert.Null(FdbExploreSeam.g_HumDataDB);
    }
}

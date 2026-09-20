using System;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmReadFileIP.pas（153 行）的纯逻辑与窗体测试。
/// 重点：12 个字符串全局量全部 Trim（唯独 bool 不 Trim）、13 条 INI 写入的键序与键名、
///       4 个 TSpinEditEx 的 DFM Min/Max/Value 组合（含 Value=60 超出 MinValue/MaxValue 的情形）。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormReadFileIPTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormReadFileIPTests()
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
    private string IniText => File.Exists(IniPath) ? File.ReadAllText(IniPath, System.Text.Encoding.GetEncoding(936)) : "";

    // ---------------- Open（原 :74-98）----------------

    [Fact]
    public void Open_把十三个全局量映射到界面值()
    {
        FormGlobals.g_sFYReadDenyIPFile = "a.txt";
        FormGlobals.g_dwFYReadDenyIPTime = 111;
        FormGlobals.g_sFYDownDenyIPUrl = "http://1";
        FormGlobals.g_dwFYDownDenyIPTime = 112;
        FormGlobals.g_sFYReadPassIPFile = "b.txt";
        FormGlobals.g_dwFYReadPassIPTime = 113;
        FormGlobals.g_sFYDownPassIPUrl = "http://2";
        FormGlobals.g_dwFYDownPassIPTime = 114;
        FormGlobals.g_sFYReadDenyMACFile = "c.txt";
        FormGlobals.g_dwFYReadDenyMACTime = 115;
        FormGlobals.g_sFYDownDenyMACUrl = "http://3";
        FormGlobals.g_dwFYDownDenyMACTime = 116;
        FormGlobals.g_OnlyWhiteListLink = true;

        var v = ReadFileIPLogic.Open();

        Assert.Equal("a.txt", v.FYReadDenyIPFile);
        Assert.Equal(111u, v.FYReadDenyIPTime);
        Assert.Equal("http://1", v.FYDownDenyIPUrl);
        Assert.Equal(112u, v.FYDownDenyIPTime);
        Assert.Equal("b.txt", v.FYReadPassIPFile);
        Assert.Equal(113u, v.FYReadPassIPTime);
        Assert.Equal("http://2", v.FYDownPassIPUrl);
        Assert.Equal(114u, v.FYDownPassIPTime);
        Assert.Equal("c.txt", v.FYReadDenyMACFile);
        Assert.Equal(115u, v.FYReadDenyMACTime);
        Assert.Equal("http://3", v.FYDownDenyMACUrl);
        Assert.Equal(116u, v.FYDownDenyMACTime);
        Assert.True(v.OnlyWhiteListLink);
    }

    [Fact]
    public void Open_不做Trim_原样带出()
    {
        FormGlobals.g_sFYReadDenyIPFile = "  x  ";
        Assert.Equal("  x  ", ReadFileIPLogic.Open().FYReadDenyIPFile);
    }

    // ---------------- ApplyToGlobals（原 :104-122）----------------

    [Fact]
    public void ApplyToGlobals_八个字符串全部Trim()
    {
        ReadFileIPLogic.ApplyToGlobals(new ReadFileIPValues
        {
            FYReadDenyIPFile = "  a  ",
            FYDownDenyIPUrl = "  b  ",
            FYReadPassIPFile = "  c  ",
            FYDownPassIPUrl = "  d  ",
            FYReadDenyMACFile = "  e  ",
            FYDownDenyMACUrl = "  f  "
        });

        Assert.Equal("a", FormGlobals.g_sFYReadDenyIPFile);
        Assert.Equal("b", FormGlobals.g_sFYDownDenyIPUrl);
        Assert.Equal("c", FormGlobals.g_sFYReadPassIPFile);
        Assert.Equal("d", FormGlobals.g_sFYDownPassIPUrl);
        Assert.Equal("e", FormGlobals.g_sFYReadDenyMACFile);
        Assert.Equal("f", FormGlobals.g_sFYDownDenyMACUrl);
    }

    [Fact]
    public void ApplyToGlobals_只Trim首尾_中间空格保留()
    {
        ReadFileIPLogic.ApplyToGlobals(new ReadFileIPValues { FYReadDenyIPFile = "  a b  " });
        Assert.Equal("a b", FormGlobals.g_sFYReadDenyIPFile);
    }

    [Fact]
    public void ApplyToGlobals_时间与开关按原值写入()
    {
        ReadFileIPLogic.ApplyToGlobals(new ReadFileIPValues
        {
            FYReadDenyIPTime = 60,
            FYDownDenyIPTime = 2,
            FYReadPassIPTime = 600000000,
            FYDownPassIPTime = 120,
            FYReadDenyMACTime = 7,
            FYDownDenyMACTime = 8,
            OnlyWhiteListLink = true
        });

        Assert.Equal(60u, FormGlobals.g_dwFYReadDenyIPTime);
        Assert.Equal(2u, FormGlobals.g_dwFYDownDenyIPTime);
        Assert.Equal(600000000u, FormGlobals.g_dwFYReadPassIPTime);
        Assert.Equal(120u, FormGlobals.g_dwFYDownPassIPTime);
        Assert.Equal(7u, FormGlobals.g_dwFYReadDenyMACTime);
        Assert.Equal(8u, FormGlobals.g_dwFYDownDenyMACTime);
        Assert.True(FormGlobals.g_OnlyWhiteListLink);
    }

    // ---------------- WriteIni（原 :124-146）----------------

    [Fact]
    public void WriteIni_十三键顺序与键名逐字节()
    {
        ReadFileIPLogic.ApplyToGlobals(new ReadFileIPValues
        {
            FYReadDenyIPFile = "a", FYReadDenyIPTime = 1,
            FYDownDenyIPUrl = "b", FYDownDenyIPTime = 2,
            FYReadPassIPFile = "c", FYReadPassIPTime = 3,
            FYDownPassIPUrl = "d", FYDownPassIPTime = 4,
            FYReadDenyMACFile = "e", FYReadDenyMACTime = 5,
            FYDownDenyMACUrl = "f", FYDownDenyMACTime = 6,
            OnlyWhiteListLink = true
        });
        ReadFileIPLogic.WriteIni(IniPath, new ReadFileIPValues());

        string expected =
            "[GameGate]\r\n" +
            "FYReadDenyIPFile=a\r\n" +
            "FYReadDenyIPTime=1\r\n" +
            "FYDownDenyIPUrl=b\r\n" +
            "FYDownDenyIPTime=2\r\n" +
            "FYReadPassIPFile=c\r\n" +
            "FYReadPassIPTime=3\r\n" +
            "FYDownPassIPUrl=d\r\n" +
            "FYDownPassIPTime=4\r\n" +
            "FYReadDenyMACFile=e\r\n" +
            "FYReadDenyMACTime=5\r\n" +
            "FYDownDenyMACUrl=f\r\n" +
            "FYDownDenyMACTime=6\r\n" +
            "OnlyWhiteListLink=1\r\n" +
            "\r\n";                       // TIniFileEx 每节后补一行空行
        Assert.Equal(expected, IniText);
    }

    [Fact]
    public void WriteIni_OnlyWhiteListLink为false写0()
    {
        ReadFileIPLogic.ApplyToGlobals(new ReadFileIPValues { OnlyWhiteListLink = false });
        ReadFileIPLogic.WriteIni(IniPath, new ReadFileIPValues());
        Assert.Contains("OnlyWhiteListLink=0\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void Save_合并调用_先写全局再落盘()
    {
        ReadFileIPLogic.Save(new ReadFileIPValues
        {
            FYReadDenyIPFile = "  z  ",
            FYReadDenyIPTime = 99,
            OnlyWhiteListLink = true
        }, IniPath);

        Assert.Equal("z", FormGlobals.g_sFYReadDenyIPFile);        // 已 Trim
        Assert.Equal(99u, FormGlobals.g_dwFYReadDenyIPTime);
        Assert.Contains("FYReadDenyIPFile=z\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("FYReadDenyIPTime=99\r\n", IniText, StringComparison.Ordinal);
    }

    // ---------------- 窗体（DFM 对齐）----------------

    [Fact]
    public void 窗体_DFM属性与控件名对齐()
    {
        using var f = new FrmReadFileIP();

        Assert.Equal("防御设置", f.Text);                          // DFM: Caption
        Assert.Equal(554, f.ClientSize.Width);                     // DFM: ClientWidth=554
        Assert.Equal(284, f.ClientSize.Height);                    // DFM: ClientHeight=284

        // 三个分组
        Assert.Equal("过滤列表", f.grp1.Text);                     // DFM: grp1 Caption
        Assert.Equal("绿色通道", f.GroupBox1.Text);                // DFM: GroupBox1 Caption
        Assert.Equal("机器码过滤列表 （引擎配置）", f.GroupBox2.Text);   // DFM: GroupBox2 Caption

        // 18 个控件名一个不少
        Assert.NotNull(f.lbl1); Assert.NotNull(f.lbl2); Assert.NotNull(f.Label1);
        Assert.NotNull(f.lbl3); Assert.NotNull(f.Label8); Assert.NotNull(f.Label9);
        Assert.NotNull(f.edtFYReadDenyIPFile); Assert.NotNull(f.edtFYDownDenyIPUrl);
        Assert.NotNull(f.seFYReadDenyIPTime); Assert.NotNull(f.seFYDownDenyIPTime);
        Assert.NotNull(f.Label2); Assert.NotNull(f.Label3); Assert.NotNull(f.Label4);
        Assert.NotNull(f.Label10); Assert.NotNull(f.Label11); Assert.NotNull(f.Label12);
        Assert.NotNull(f.edtFYReadPassIPFile); Assert.NotNull(f.edtFYDownPassIPUrl);
        Assert.NotNull(f.seFYReadPassIPTime); Assert.NotNull(f.seFYDownPassIPTime);
        Assert.NotNull(f.Label5); Assert.NotNull(f.Label6); Assert.NotNull(f.Label7);
        Assert.NotNull(f.Label13); Assert.NotNull(f.Label14); Assert.NotNull(f.Label15);
        Assert.NotNull(f.edtFYReadDenyMACFile); Assert.NotNull(f.edtFYDownDenyMACUrl);
        Assert.NotNull(f.seFYReadDenyMACTime); Assert.NotNull(f.seFYDownDenyMACTime);
        Assert.NotNull(f.btnOK); Assert.NotNull(f.chkOnlyWhiteListLink);

        Assert.Equal("只允许绿色通道（白名单）连接", f.chkOnlyWhiteListLink.Text);
        Assert.Equal("确定(&O)", f.btnOK.Text);
        Assert.Equal(8, f.chkOnlyWhiteListLink.Left);              // DFM: Left=8
        Assert.Equal(258, f.chkOnlyWhiteListLink.Top);             // DFM: Top=258
        Assert.Equal(201, f.chkOnlyWhiteListLink.Width);           // DFM: Width=201
    }

    [Fact]
    public void 窗体_四个SpinEditEx的DFM范围与Value()
    {
        using var f = new FrmReadFileIP();

        // DFM: seFYReadDenyIPTime Left=442 Top=16 Width=72 Increment=10 MaxValue=600000000 MinValue=60 Value=60
        Assert.Equal(442, f.seFYReadDenyIPTime.Left);
        Assert.Equal(16, f.seFYReadDenyIPTime.Top);
        Assert.Equal(72, f.seFYReadDenyIPTime.Width);
        Assert.Equal(10, f.seFYReadDenyIPTime.Increment);
        Assert.Equal(60, f.seFYReadDenyIPTime.DfmMinValue);
        Assert.Equal(600000000, f.seFYReadDenyIPTime.DfmMaxValue);
        Assert.Equal(60, f.seFYReadDenyIPTime.Value);

        // DFM: seFYDownDenyIPTime Left=442 Top=43 Width=72 MaxValue=120 MinValue=2 Value=60
        // ★ Value=60 落在 [2,120] 内；但 MinValue=2 MaxValue=120 与 "分钟" 语义并存 → 记录原文如此
        Assert.Equal(442, f.seFYDownDenyIPTime.Left);
        Assert.Equal(2, f.seFYDownDenyIPTime.DfmMinValue);
        Assert.Equal(120, f.seFYDownDenyIPTime.DfmMaxValue);
        Assert.Equal(60, f.seFYDownDenyIPTime.Value);

        // DFM: seFYReadPassIPTime Left=441（注意与 Deny 的 442 差 1）
        Assert.Equal(441, f.seFYReadPassIPTime.Left);
        // DFM: seFYDownPassIPTime Left=442 Top=43
        Assert.Equal(442, f.seFYDownPassIPTime.Left);

        // DFM: seFYReadDenyMACTime Left=442 Top=17；seFYDownDenyMACTime Left=442 Top=43
        Assert.Equal(17, f.seFYReadDenyMACTime.Top);
        Assert.Equal(43, f.seFYDownDenyMACTime.Top);
    }

    [Fact]
    public void 窗体_Open把全局量灌进控件()
    {
        FormGlobals.g_sFYReadDenyIPFile = "local.txt";
        FormGlobals.g_dwFYReadDenyIPTime = 77;
        FormGlobals.g_OnlyWhiteListLink = true;

        using var f = new FrmReadFileIP();
        f.Open();

        Assert.Equal("local.txt", f.edtFYReadDenyIPFile.Text);
        Assert.Equal(77, f.seFYReadDenyIPTime.Value);
        Assert.True(f.chkOnlyWhiteListLink.Checked);
    }

    [Fact]
    public void 窗体_btnOK_Click读取控件并落盘()
    {
        FormGlobals.g_sIniFileName = IniPath;

        using var f = new FrmReadFileIP();
        f.edtFYReadDenyIPFile.Text = "  my.txt  ";
        f.seFYReadDenyIPTime.Value = 123;
        f.edtFYDownDenyIPUrl.Text = "http://d";
        f.seFYDownDenyIPTime.Value = 12;
        f.edtFYReadPassIPFile.Text = "p.txt";
        f.seFYReadPassIPTime.Value = 456;
        f.edtFYDownPassIPUrl.Text = "http://p";
        f.seFYDownPassIPTime.Value = 13;
        f.edtFYReadDenyMACFile.Text = "m.txt";
        f.seFYReadDenyMACTime.Value = 789;
        f.edtFYDownDenyMACUrl.Text = "http://m";
        f.seFYDownDenyMACTime.Value = 14;
        f.chkOnlyWhiteListLink.Checked = true;

        f.btnOK_Click(f, EventArgs.Empty);

        Assert.Equal("my.txt", FormGlobals.g_sFYReadDenyIPFile);       // Trim 过
        Assert.Equal(123u, FormGlobals.g_dwFYReadDenyIPTime);
        Assert.True(FormGlobals.g_OnlyWhiteListLink);
        Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);   // 原 :147

        Assert.Contains("FYReadDenyIPFile=my.txt\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("FYReadDenyIPTime=123\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("FYDownDenyMACTime=14\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("OnlyWhiteListLink=1\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void 窗体_SpinEditEx编程赋值不裁剪_超DFM上限也接受()
    {
        using var f = new FrmReadFileIP();
        f.seFYDownDenyIPTime.Value = 9999;                        // 远超 MaxValue=120
        Assert.Equal(9999, f.seFYDownDenyIPTime.Value);           // 原文/既有实现语义：不裁剪
    }
}

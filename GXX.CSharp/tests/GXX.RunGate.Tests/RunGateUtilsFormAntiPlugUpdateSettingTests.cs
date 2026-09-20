using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrm* 窗体族测试共用的 xUnit 集合：`DisableParallelization = true`。
/// 原因：这些窗体把状态放在 `FormGlobals` 的**静态全局量**里（对应 Delphi 的单元级全局变量），
/// 并会替换 `MessageBoxSeam.Show` / `InputQueryWithValue` 等静态接缝。
/// 若并行执行会互相污染（实测：ProcessBlacklist 与 MessageFilter 的全局列表互相清空）。
/// 用一个共享集合把它们两两串行，**不改 csproj、不加 AssemblyInfo**。
/// </summary>
[CollectionDefinition("RunGateFormLane", DisableParallelization = true)]
public class RunGateFormLaneCollection : ICollectionFixture<RunGateFormLaneFixture>
{
}

/// <summary>
/// 集合夹具：**关键安全措施** —— 关闭真实模态对话框。
/// 无头（headless）测试宿主里 `MessageBox.Show` / `Form.ShowDialog` 会启动消息循环并挂住 testhost，
/// 本车道曾因此出现 `dotnet test` 10 分钟超时。
/// 夹具在整个集合运行期间把 `MessageBoxSeam.UiEnabled` 置 false 并把 `Show` 换成非阻塞桩；
/// 需要断言弹窗的用例会在自己内部保存/替换/还原 `Show`，还原后仍是本桩。
/// </summary>
public sealed class RunGateFormLaneFixture : IDisposable
{
    private readonly Func<string, string, System.Windows.Forms.MessageBoxButtons,
                             System.Windows.Forms.MessageBoxIcon, System.Windows.Forms.DialogResult> _originalShow;

    public RunGateFormLaneFixture()
    {
        _originalShow = MessageBoxSeam.Show;
        MessageBoxSeam.UiEnabled = false;                                             // 禁止任何 ShowDialog
        MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.OK;    // 非阻塞桩
    }

    public void Dispose()
    {
        MessageBoxSeam.Show = _originalShow;
        MessageBoxSeam.UiEnabled = true;
    }
}

/// <summary>
/// uFrmAntiPlugUpdateSetting.pas（73 行）的纯逻辑与窗体测试。
/// 覆盖：AntiPlugUpdateSettingLogic.Open / Save、INI 键序与键名（含 `AntiPlugUpdateConfigUrl5` 的尾随 5）、
///       FrmAntiPlugUpdateSetting 的 DFM 控件对齐与 OnCreate/OnClick 行为。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormAntiPlugUpdateSettingTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormAntiPlugUpdateSettingTests()
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

    // ---------------- AntiPlugUpdateSettingLogic.Open（原 :45-50）----------------

    [Fact]
    public void Open_把三个全局量映射到界面值_不做Trim也不做范围裁剪()
    {
        FormGlobals.g_boAntiPlugAutoUpdateCheck = true;
        FormGlobals.g_wAntiPlugUpdateCheckInterval = 42;
        FormGlobals.g_sAntiPlugUpdateConfigUrl = "http://a/b.ini";

        var v = AntiPlugUpdateSettingLogic.Open();

        Assert.True(v.AntiPlugAutoUpdateCheck);
        Assert.Equal(42, v.AntiPlugUpdateCheckInterval);
        Assert.Equal("http://a/b.ini", v.AntiPlugUpdateConfigUrl);
    }

    [Fact]
    public void Open_间隔越界也原样返回_编程赋值不裁剪()
    {
        // 原 DFM 的 SpinEditEx 是 MaxValue=120 MinValue=1，但 FormCreate 直接赋全局值。
        // 与 GXX.DBServer 的 TSpinEdit「编程赋值不裁剪」语义一致：0 与 999 都原样显示。
        FormGlobals.g_wAntiPlugUpdateCheckInterval = 0;
        Assert.Equal(0, AntiPlugUpdateSettingLogic.Open().AntiPlugUpdateCheckInterval);

        FormGlobals.g_wAntiPlugUpdateCheckInterval = 999;
        Assert.Equal(999, AntiPlugUpdateSettingLogic.Open().AntiPlugUpdateCheckInterval);
    }

    [Fact]
    public void Open_地址不做Trim_前后空格保留()
    {
        FormGlobals.g_sAntiPlugUpdateConfigUrl = "  http://x  ";
        Assert.Equal("  http://x  ", AntiPlugUpdateSettingLogic.Open().AntiPlugUpdateConfigUrl);
    }

    // ---------------- AntiPlugUpdateSettingLogic.Save（原 :52-70）----------------

    [Fact]
    public void Save_写回三个全局量()
    {
        AntiPlugUpdateSettingLogic.Save(new AntiPlugUpdateSettingValues
        {
            AntiPlugAutoUpdateCheck = true,
            AntiPlugUpdateCheckInterval = 7,
            AntiPlugUpdateConfigUrl = "u"
        }, IniPath);

        Assert.True(FormGlobals.g_boAntiPlugAutoUpdateCheck);
        Assert.Equal(7, FormGlobals.g_wAntiPlugUpdateCheckInterval);
        Assert.Equal("u", FormGlobals.g_sAntiPlugUpdateConfigUrl);
    }

    [Fact]
    public void Save_INI三键顺序与大小写逐字节_含尾随数字5()
    {
        AntiPlugUpdateSettingLogic.Save(new AntiPlugUpdateSettingValues
        {
            AntiPlugAutoUpdateCheck = true,
            AntiPlugUpdateCheckInterval = 5,
            AntiPlugUpdateConfigUrl = "abc"
        }, IniPath);

        // 原 :62-64 的键序 / 键名逐字：
        //   [GameGate]
        //   AntiPlugAutoUpdateCheck=1
        //   AntiPlugUpdateCheckInterval=5
        //   AntiPlugUpdateConfigUrl5=abc        ← 键名末尾的 '5' 是原文原样
        // 末尾多一个空行来自 TIniFileEx.GetStrings（每个节后补一行 ""，IniFilesEx.cs:493），
        // 与 Delphi TMemIniFile.UpdateFile 的输出一致。
        Assert.Equal("[GameGate]\r\nAntiPlugAutoUpdateCheck=1\r\nAntiPlugUpdateCheckInterval=5\r\nAntiPlugUpdateConfigUrl5=abc\r\n\r\n",
                     IniText);
    }

    [Fact]
    public void Save_false写成0_true写成1()
    {
        AntiPlugUpdateSettingLogic.Save(new AntiPlugUpdateSettingValues
        {
            AntiPlugAutoUpdateCheck = false,
            AntiPlugUpdateCheckInterval = 1,
            AntiPlugUpdateConfigUrl = ""
        }, IniPath);

        Assert.Contains("AntiPlugAutoUpdateCheck=0\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("AntiPlugUpdateConfigUrl5=\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void Save_键名差异断言_不是无5的AntiPlugUpdateConfigUrl()
    {
        AntiPlugUpdateSettingLogic.Save(new AntiPlugUpdateSettingValues { AntiPlugUpdateConfigUrl = "x" }, IniPath);

        Assert.Contains("AntiPlugUpdateConfigUrl5=x", IniText, StringComparison.Ordinal);
        // "看起来一样实则不同"：无 5 版本**不应**出现
        Assert.DoesNotContain("\r\nAntiPlugUpdateConfigUrl=x", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void Save_空路径不抛异常_也不落盘()
    {
        // TIniFileEx("") 的 UpdateFile 在 FileName == "" 时直接返回。
        var ex = Record.Exception(() => AntiPlugUpdateSettingLogic.Save(
            new AntiPlugUpdateSettingValues { AntiPlugUpdateConfigUrl = "x" }, ""));
        Assert.Null(ex);
    }

    // ---------------- FrmAntiPlugUpdateSetting 窗体（DFM 对齐）----------------

    [Fact]
    public void 窗体_DFM关键属性与控件名对齐()
    {
        using var f = new FrmAntiPlugUpdateSetting();

        Assert.Equal("网关插件自动更新设置", f.Text);                       // DFM: Caption
        Assert.Equal(432, f.ClientSize.Width);                              // DFM: ClientWidth=432
        Assert.Equal(145, f.ClientSize.Height);                             // DFM: ClientHeight=145
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedDialog, f.FormBorderStyle);   // bsDialog

        // 控件名保留原名（对照 .dfm）
        Assert.NotNull(f.grpMain);
        Assert.NotNull(f.lbl1);
        Assert.NotNull(f.lbl2);
        Assert.NotNull(f.Label1);
        Assert.NotNull(f.chkAntiPlugAutoUpdateCheck);
        Assert.NotNull(f.edtAntiPlugUpdateConfigUrl);
        Assert.NotNull(f.seAntiPlugUpdateCheckInterval);
        Assert.NotNull(f.btnOK);

        Assert.Equal("自动更新设置", f.grpMain.Text);
        Assert.Equal("自动检测插件更新", f.chkAntiPlugAutoUpdateCheck.Text);
        Assert.Equal("确定", f.btnOK.Text);
        Assert.Equal(1, f.seAntiPlugUpdateCheckInterval.Value);             // DFM: Value=1
        Assert.Equal(125, f.edtAntiPlugUpdateConfigUrl.Left);               // DFM: Left=125
        Assert.Equal(70, f.edtAntiPlugUpdateConfigUrl.Top);                 // DFM: Top=70
        Assert.Equal(285, f.edtAntiPlugUpdateConfigUrl.Width);              // DFM: Width=285
    }

    [Fact]
    public void 窗体_FormCreate把全局量灌进控件()
    {
        FormGlobals.g_boAntiPlugAutoUpdateCheck = true;
        FormGlobals.g_wAntiPlugUpdateCheckInterval = 33;
        FormGlobals.g_sAntiPlugUpdateConfigUrl = "http://cfg";

        using var f = new FrmAntiPlugUpdateSetting();
        f.FormCreate(f, EventArgs.Empty);

        Assert.True(f.chkAntiPlugAutoUpdateCheck.Checked);
        Assert.Equal(33, f.seAntiPlugUpdateCheckInterval.Value);
        Assert.Equal("http://cfg", f.edtAntiPlugUpdateConfigUrl.Text);
    }

    [Fact]
    public void 窗体_btnOK_Click读控件写全局并置DialogResult()
    {
        using var f = new FrmAntiPlugUpdateSetting();
        f.chkAntiPlugAutoUpdateCheck.Checked = false;
        f.seAntiPlugUpdateCheckInterval.Value = 88;      // 超出 DFM 的 1..120 —— 编程赋值不裁剪
        f.edtAntiPlugUpdateConfigUrl.Text = "http://z";

        FormGlobals.g_sIniFileName = IniPath;
        f.btnOK_Click(f, EventArgs.Empty);

        Assert.False(FormGlobals.g_boAntiPlugAutoUpdateCheck);
        Assert.Equal(88, FormGlobals.g_wAntiPlugUpdateCheckInterval);
        Assert.Equal("http://z", FormGlobals.g_sAntiPlugUpdateConfigUrl);
        Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);   // 原 :69
        Assert.Contains("AntiPlugUpdateCheckInterval=88", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void 窗体_编程赋值不裁剪_DFM的MinMax只影响上下按钮()
    {
        using var f = new FrmAntiPlugUpdateSetting();
        f.seAntiPlugUpdateCheckInterval.Value = 5000;               // 远超 DFM 的 MaxValue=120
        Assert.Equal(5000, f.seAntiPlugUpdateCheckInterval.Value);  // 编程赋值不裁剪（原文语义）
        Assert.Equal(1, f.seAntiPlugUpdateCheckInterval.DfmMinValue);
        Assert.Equal(120, f.seAntiPlugUpdateCheckInterval.DfmMaxValue);

        // UpButton：Value(5000) >= DfmMaxValue(120) → 直接返回，不加
        f.seAntiPlugUpdateCheckInterval.Value = 5000;
        f.seAntiPlugUpdateCheckInterval.UpButton();
        Assert.Equal(5000, f.seAntiPlugUpdateCheckInterval.Value);

        // DownButton：Value(5000) > DfmMinValue(1) → 正常减 1（"超上限后仍可下减"是原文/既有实现语义）
        f.seAntiPlugUpdateCheckInterval.DownButton();
        Assert.Equal(4999, f.seAntiPlugUpdateCheckInterval.Value);

        // 回到区间内：UpButton 正常加 1
        f.seAntiPlugUpdateCheckInterval.Value = 100;
        f.seAntiPlugUpdateCheckInterval.UpButton();
        Assert.Equal(101, f.seAntiPlugUpdateCheckInterval.Value);

        // 到 DfmMaxValue 后不再加
        f.seAntiPlugUpdateCheckInterval.Value = 120;
        f.seAntiPlugUpdateCheckInterval.UpButton();
        Assert.Equal(120, f.seAntiPlugUpdateCheckInterval.Value);

        // 到 DfmMinValue 后不再减
        f.seAntiPlugUpdateCheckInterval.Value = 1;
        f.seAntiPlugUpdateCheckInterval.DownButton();
        Assert.Equal(1, f.seAntiPlugUpdateCheckInterval.Value);
    }
}

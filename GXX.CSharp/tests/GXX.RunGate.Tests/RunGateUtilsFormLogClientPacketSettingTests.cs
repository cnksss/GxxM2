using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmLogClientPacketSetting.pas（146 行）的纯逻辑与窗体测试。
/// 重点：
///   * 掩码由 **7 位** 组成（chkLogOther **不参与**，尽管 DFM 里它 Checked=True 且 Enabled=False）；
///   * CPT_* 位值来自 GateShare.pas:1338-1344（1/2/4/8/16/32/64）；
///   * 添加人物名的两条校验（空 / 重复）与 `FormCreate`/`lstLogUserClick` 的 btnDelUser 使能规则。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormLogClientPacketSettingTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormLogClientPacketSettingTests()
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
    private string UserFile => Path.Combine(_dir, "LogUser.txt");
    private string IniText => File.Exists(IniPath) ? File.ReadAllText(IniPath, System.Text.Encoding.GetEncoding(936)) : "";

    // ---------------- CPT_* 常量（GateShare.pas:1338-1344）----------------

    [Fact]
    public void CPT位值与原文逐条一致()
    {
        Assert.Equal(1, RunGateConst.CptMove);
        Assert.Equal(2, RunGateConst.CptHit);
        Assert.Equal(4, RunGateConst.CptSpell);
        Assert.Equal(8, RunGateConst.CptQuery);
        Assert.Equal(16, RunGateConst.CptTeam);
        Assert.Equal(32, RunGateConst.CptGuild);
        Assert.Equal(64, RunGateConst.CptShop);
    }

    // ---------------- BuildPacketTypeMask（原 :63-72）----------------

    [Fact]
    public void BuildPacketTypeMask_全不勾为0()
    {
        Assert.Equal(0, LogClientPacketSettingLogic.BuildPacketTypeMask(new LogClientPacketSettingValues()));
    }

    [Fact]
    public void BuildPacketTypeMask_七项全勾为127()
    {
        var v = new LogClientPacketSettingValues
        {
            LogMove = true, LogHit = true, LogSpell = true, LogQuery = true,
            LogTeam = true, LogGuild = true, LogShop = true
        };
        Assert.Equal(127, LogClientPacketSettingLogic.BuildPacketTypeMask(v));   // 1+2+4+8+16+32+64
    }

    [Fact]
    public void BuildPacketTypeMask_逐位单独验证()
    {
        Assert.Equal(1, LogClientPacketSettingLogic.BuildPacketTypeMask(new LogClientPacketSettingValues { LogMove = true }));
        Assert.Equal(2, LogClientPacketSettingLogic.BuildPacketTypeMask(new LogClientPacketSettingValues { LogHit = true }));
        Assert.Equal(4, LogClientPacketSettingLogic.BuildPacketTypeMask(new LogClientPacketSettingValues { LogSpell = true }));
        Assert.Equal(8, LogClientPacketSettingLogic.BuildPacketTypeMask(new LogClientPacketSettingValues { LogQuery = true }));
        Assert.Equal(16, LogClientPacketSettingLogic.BuildPacketTypeMask(new LogClientPacketSettingValues { LogTeam = true }));
        Assert.Equal(32, LogClientPacketSettingLogic.BuildPacketTypeMask(new LogClientPacketSettingValues { LogGuild = true }));
        Assert.Equal(64, LogClientPacketSettingLogic.BuildPacketTypeMask(new LogClientPacketSettingValues { LogShop = true }));
    }

    [Fact]
    public void BuildPacketTypeMask_LogOther不参与掩码_原文缺陷断言()
    {
        // ★ chkLogOther 在 DFM 里 Checked=True Enabled=False，但 btnOKClick **从不读它**。
        var v = new LogClientPacketSettingValues { LogOther = true };
        Assert.Equal(0, LogClientPacketSettingLogic.BuildPacketTypeMask(v));

        // 与 LogOther=false 完全一致（差异断言）
        var v2 = new LogClientPacketSettingValues { LogOther = false };
        Assert.Equal(LogClientPacketSettingLogic.BuildPacketTypeMask(v2),
                     LogClientPacketSettingLogic.BuildPacketTypeMask(v));
    }

    // ---------------- Open（原 :126-137）----------------

    [Fact]
    public void Open_按位拆解掩码()
    {
        FormGlobals.g_nLogClientPacketType = 1 | 4 | 32;      // Move + Spell + Guild
        var v = LogClientPacketSettingLogic.Open();

        Assert.True(v.LogMove);
        Assert.False(v.LogHit);
        Assert.True(v.LogSpell);
        Assert.False(v.LogQuery);
        Assert.False(v.LogTeam);
        Assert.True(v.LogGuild);
        Assert.False(v.LogShop);
        Assert.True(v.LogOther);                              // DFM Checked=True
    }

    [Fact]
    public void Open_掩码为0时全不勾()
    {
        FormGlobals.g_nLogClientPacketType = 0;
        var v = LogClientPacketSettingLogic.Open();
        Assert.False(v.LogMove);
        Assert.False(v.LogHit);
        Assert.False(v.LogSpell);
        Assert.False(v.LogQuery);
        Assert.False(v.LogTeam);
        Assert.False(v.LogGuild);
        Assert.False(v.LogShop);
    }

    [Fact]
    public void Open_读入开关与人物列表()
    {
        FormGlobals.g_boLogClientPacket = true;
        FormGlobals.g_LogClientPacketUser.Text = "u1\r\nu2";
        var v = LogClientPacketSettingLogic.Open();

        Assert.True(v.LogClientPacket);
        Assert.Equal(new[] { "u1", "u2" }, v.LogUsers);
    }

    [Fact]
    public void Open_掩码含未知高位时不影响已定义位()
    {
        FormGlobals.g_nLogClientPacketType = 0xFF;            // 全 1
        var v = LogClientPacketSettingLogic.Open();
        Assert.True(v.LogMove && v.LogHit && v.LogSpell && v.LogQuery && v.LogTeam && v.LogGuild && v.LogShop);
    }

    // ---------------- ButtonOK（原 :58-92）----------------

    [Fact]
    public void ButtonOK_写回掩码开关与INI两键()
    {
        var v = new LogClientPacketSettingValues
        {
            LogMove = true,
            LogShop = true,
            LogClientPacket = true,
            LogUsers = new List<string> { "a", "b" }
        };
        LogClientPacketSettingLogic.ButtonOK(v, IniPath, UserFile);

        Assert.Equal(1 | 64, FormGlobals.g_nLogClientPacketType);
        Assert.True(FormGlobals.g_boLogClientPacket);
        Assert.Equal(2, FormGlobals.g_LogClientPacketUser.Count);

        string expected =
            "[GameGate]\r\n" +
            "LogClientPacket=1\r\n" +
            "LogClientPacketType=65\r\n" +
            "\r\n";
        Assert.Equal(expected, IniText);
        Assert.True(File.Exists(UserFile));
    }

    [Fact]
    public void ButtonOK_人物列表按CRLF落盘()
    {
        LogClientPacketSettingLogic.ButtonOK(new LogClientPacketSettingValues
        {
            LogUsers = new List<string> { "u1", "u2", "u3" }
        }, IniPath, UserFile);

        string text = File.ReadAllText(UserFile, System.Text.Encoding.GetEncoding(936));
        Assert.Equal("u1\r\nu2\r\nu3\r\n", text);
    }

    [Fact]
    public void ButtonOK_空文件名不抛异常()
    {
        var ex = Record.Exception(() => LogClientPacketSettingLogic.ButtonOK(
            new LogClientPacketSettingValues { LogUsers = new List<string> { "x" } }, "", ""));
        Assert.Null(ex);
    }

    // ---------------- ValidateAddUser（原 :94-114）----------------

    [Fact]
    public void ValidateAddUser_空名称被拒绝()
    {
        bool ok = LogClientPacketSettingLogic.ValidateAddUser("", new List<string>(), out string err);
        Assert.False(ok);
        Assert.Equal("人物名称不能为空", err);                  // 原 :101
    }

    [Fact]
    public void ValidateAddUser_全空白被拒绝_Trim后判空()
    {
        bool ok = LogClientPacketSettingLogic.ValidateAddUser("   ", new List<string>(), out string err);
        Assert.False(ok);
        Assert.Equal("人物名称不能为空", err);
    }

    [Fact]
    public void ValidateAddUser_重复名称被拒绝()
    {
        bool ok = LogClientPacketSettingLogic.ValidateAddUser("abc", new List<string> { "abc" }, out string err);
        Assert.False(ok);
        Assert.Equal("人物名称已经在列表中存在", err);           // 原 :108
    }

    [Fact]
    public void ValidateAddUser_前后空格Trim后与既有项判重()
    {
        bool ok = LogClientPacketSettingLogic.ValidateAddUser("  abc  ", new List<string> { "abc" }, out string err);
        Assert.False(ok);
        Assert.Equal("人物名称已经在列表中存在", err);
    }

    [Fact]
    public void ValidateAddUser_判重区分大小写()
    {
        // `TStrings.IndexOf` 默认大小写敏感 → "ABC" 与 "abc" 不算重复
        bool ok = LogClientPacketSettingLogic.ValidateAddUser("ABC", new List<string> { "abc" }, out string err);
        Assert.True(ok);
        Assert.Equal("", err);
    }

    [Fact]
    public void ValidateAddUser_合法名称通过()
    {
        bool ok = LogClientPacketSettingLogic.ValidateAddUser("新人物", new List<string> { "other" }, out string err);
        Assert.True(ok);
        Assert.Equal("", err);
    }

    // ---------------- 窗体（DFM 对齐）----------------

    [Fact]
    public void 窗体_DFM属性与控件名对齐()
    {
        using var f = new FrmLogClientPacketSetting();

        Assert.Equal("封包记录设置", f.Text);                       // DFM: Caption
        Assert.Equal(339, f.ClientSize.Width);                      // DFM: ClientWidth=339
        Assert.Equal(238, f.ClientSize.Height);                     // DFM: ClientHeight=238

        Assert.NotNull(f.grpPacketType);
        Assert.NotNull(f.grpLogUser);
        Assert.NotNull(f.chkLogMove);
        Assert.NotNull(f.chkLogHit);
        Assert.NotNull(f.chkLogSpell);
        Assert.NotNull(f.chkLogQuery);
        Assert.NotNull(f.chkLogTeam);
        Assert.NotNull(f.chkLogGuild);
        Assert.NotNull(f.chkLogShop);
        Assert.NotNull(f.chkLogOther);
        Assert.NotNull(f.lbl1);
        Assert.NotNull(f.lstLogUser);
        Assert.NotNull(f.edtUserName);
        Assert.NotNull(f.btnAddUser);
        Assert.NotNull(f.btnDelUser);
        Assert.NotNull(f.btnOK);
        Assert.NotNull(f.btnCancel);
        Assert.NotNull(f.chkLogClientPacket);

        Assert.Equal("封包类型", f.grpPacketType.Text);
        Assert.Equal("记录指定人物（为空表示记录所有）", f.grpLogUser.Text);
        Assert.Equal("移动相关", f.chkLogMove.Text);
        Assert.Equal("攻击相关", f.chkLogHit.Text);
        Assert.Equal("魔法相关", f.chkLogSpell.Text);
        Assert.Equal("查询相关", f.chkLogQuery.Text);
        Assert.Equal("组队相关", f.chkLogTeam.Text);
        Assert.Equal("行会相关", f.chkLogGuild.Text);
        Assert.Equal("商铺摆摊", f.chkLogShop.Text);
        Assert.Equal("其他操作", f.chkLogOther.Text);
        Assert.Equal("人物", f.lbl1.Text);
        Assert.Equal("添加", f.btnAddUser.Text);
        Assert.Equal("删除", f.btnDelUser.Text);
        Assert.Equal("确定", f.btnOK.Text);
        Assert.Equal("取消", f.btnCancel.Text);
        Assert.Equal("开启封包记录", f.chkLogClientPacket.Text);
        Assert.Equal(System.Windows.Forms.DialogResult.Cancel, f.btnCancel.DialogResult);   // ModalResult=2

        // DFM: chkLogOther Checked=True Enabled=False（照抄）
        Assert.True(f.chkLogOther.Checked);
        Assert.False(f.chkLogOther.Enabled);
    }

    [Fact]
    public void 窗体_FormCreate按掩码勾选七项()
    {
        FormGlobals.g_nLogClientPacketType = 2 | 8 | 16;   // Hit + Query + Team
        FormGlobals.g_boLogClientPacket = true;
        FormGlobals.g_LogClientPacketUser.Text = "u1";

        using var f = new FrmLogClientPacketSetting();
        f.FormCreate(f, EventArgs.Empty);

        Assert.False(f.chkLogMove.Checked);
        Assert.True(f.chkLogHit.Checked);
        Assert.False(f.chkLogSpell.Checked);
        Assert.True(f.chkLogQuery.Checked);
        Assert.True(f.chkLogTeam.Checked);
        Assert.False(f.chkLogGuild.Checked);
        Assert.False(f.chkLogShop.Checked);
        Assert.True(f.chkLogClientPacket.Checked);
        Assert.Equal(1, f.lstLogUser.Items.Count);
        Assert.Equal("u1", f.lstLogUser.Items[0]);
        Assert.False(f.btnDelUser.Enabled);      // 无选中（原 :137）
    }

    [Fact]
    public void 窗体_btnOK_Click写掩码与INI()
    {
        FormGlobals.g_sIniFileName = IniPath;
        FormGlobals.g_sLogClientPakcetUserFile = UserFile;

        using var f = new FrmLogClientPacketSetting();
        f.chkLogMove.Checked = true;
        f.chkLogSpell.Checked = true;
        f.chkLogClientPacket.Checked = true;
        f.lstLogUser.Items.Add("p1");

        f.btnOK_Click(f, EventArgs.Empty);

        Assert.Equal(1 | 4, FormGlobals.g_nLogClientPacketType);
        Assert.True(FormGlobals.g_boLogClientPacket);
        Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);      // 原 :91
        Assert.Contains("LogClientPacketType=5\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("LogClientPacket=1\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void 窗体_btnAddUser_Click校验两条分支()
    {
        var shown = new List<string>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };

            using var f = new FrmLogClientPacketSetting();

            // 空名
            f.edtUserName.Text = "  ";
            f.btnAddUser_Click(f, EventArgs.Empty);
            Assert.Single(shown);
            Assert.Equal("人物名称不能为空", shown[0]);
            Assert.Equal(0, f.lstLogUser.Items.Count);

            // 重复名
            shown.Clear();
            f.lstLogUser.Items.Add("dup");
            f.edtUserName.Text = "dup";
            f.btnAddUser_Click(f, EventArgs.Empty);
            Assert.Single(shown);
            Assert.Equal("人物名称已经在列表中存在", shown[0]);
            Assert.Equal(1, f.lstLogUser.Items.Count);

            // 合法（Trim 后加入）
            shown.Clear();
            f.edtUserName.Text = "  ok  ";
            f.btnAddUser_Click(f, EventArgs.Empty);
            Assert.Empty(shown);
            Assert.Equal(2, f.lstLogUser.Items.Count);
            Assert.Equal("ok", f.lstLogUser.Items[1]);      // 原 :113 Add(UserName)（已 Trim）
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_btnDelUser_Click仅在选中时删除()
    {
        using var f = new FrmLogClientPacketSetting();
        f.lstLogUser.Items.AddRange(new object[] { "a", "b" });

        // 未选中 → 不删
        f.lstLogUser.SelectedIndex = -1;
        f.btnDelUser_Click(f, EventArgs.Empty);
        Assert.Equal(2, f.lstLogUser.Items.Count);

        // 选中 → 删
        f.lstLogUser.SelectedIndex = 0;
        f.btnDelUser_Click(f, EventArgs.Empty);
        Assert.Equal(1, f.lstLogUser.Items.Count);
        Assert.Equal("b", f.lstLogUser.Items[0]);
    }

    [Fact]
    public void 窗体_lstLogUserClick按选中切换btnDelUser使能()
    {
        using var f = new FrmLogClientPacketSetting();
        f.btnDelUser.Enabled = true;
        f.lstLogUser_Click(f, EventArgs.Empty);          // 空列表
        Assert.False(f.btnDelUser.Enabled);              // 原 :142

        f.lstLogUser.Items.Add("a");
        f.lstLogUser.SelectedIndex = 0;
        f.lstLogUser_Click(f, EventArgs.Empty);
        Assert.True(f.btnDelUser.Enabled);
    }
}

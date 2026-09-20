using System;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmItemEatCD.pas（265 行）的纯逻辑与窗体测试。
/// 重点：
///   * 84 个编辑框的 6 slot × 7 字段映射顺序（NormalHP/NormalMP/NormalHPMP/SpecialHP/SpecialMP/SpecialHPMP/Other）；
///   * INI 键名 = `IntToStr(I+1) + 后缀` → '1NormalHP'..'3Other'（**键前缀从 1 开始、无分隔符**）；
///   * 两个节 [HumanItemEatCD] / [HeroItemEatCD]；
///   * DFM 的 84 个 TSpinEditEx 全为 MaxValue=0 MinValue=0 → 编程赋值不裁剪。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormItemEatCDTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormItemEatCDTests()
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

    // ---------------- 键名规则（原 :185-207）----------------

    [Fact]
    public void FieldSuffixes_七个后缀顺序与原文一致()
    {
        Assert.Equal(new[] { "NormalHP", "NormalMP", "NormalHPMP", "SpecialHP", "SpecialMP", "SpecialHPMP", "Other" },
                     ItemEatCDLogic.FieldSuffixes);
    }

    [Fact]
    public void KeyPrefix_从1开始()
    {
        Assert.Equal("1", ItemEatCDLogic.KeyPrefix(0));      // 原 :187 IntToStr(I + 1)
        Assert.Equal("2", ItemEatCDLogic.KeyPrefix(1));
        Assert.Equal("3", ItemEatCDLogic.KeyPrefix(2));
    }

    [Fact]
    public void KeyName_无分隔符拼接()
    {
        Assert.Equal("1NormalHP", ItemEatCDLogic.KeyName(0, "NormalHP"));
        Assert.Equal("3Other", ItemEatCDLogic.KeyName(2, "Other"));
        // 差异断言：不是 '1_NormalHP'、也不是 '0NormalHP'
        Assert.NotEqual("1_NormalHP", ItemEatCDLogic.KeyName(0, "NormalHP"));
        Assert.NotEqual("0NormalHP", ItemEatCDLogic.KeyName(0, "NormalHP"));
    }

    [Fact]
    public void 节名逐字照抄()
    {
        Assert.Equal("HumanItemEatCD", ItemEatCDLogic.HumanSection);
        Assert.Equal("HeroItemEatCD", ItemEatCDLogic.HeroSection);
    }

    // ---------------- GetSlotValues / SetSlotValues ----------------

    [Fact]
    public void GetSetSlotValues_七字段往返()
    {
        var t = new TItemCDTime();
        ItemEatCDLogic.SetSlotValues(t, new[] { 1, 2, 3, 4, 5, 6, 7 });

        Assert.Equal(1, t.NormalHP);
        Assert.Equal(2, t.NormalMP);
        Assert.Equal(3, t.NormalHPMP);
        Assert.Equal(4, t.SpecialHP);
        Assert.Equal(5, t.SpecialMP);
        Assert.Equal(6, t.SpecialHPMP);
        Assert.Equal(7, t.Other);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7 }, ItemEatCDLogic.GetSlotValues(t));
    }

    [Fact]
    public void SetSlotValues_元素不足时抛异常()
    {
        var t = new TItemCDTime();
        Assert.Throws<ArgumentException>(() => ItemEatCDLogic.SetSlotValues(t, new[] { 1, 2, 3 }));
        Assert.Throws<ArgumentException>(() => ItemEatCDLogic.SetSlotValues(t, null));
    }

    // ---------------- WriteIni（原 :184-208）----------------

    [Fact]
    public void WriteIni_两节各三组各七键_键序逐字节()
    {
        var cfg = new TEatItemCDConfig();
        ItemEatCDLogic.SetSlotValues(cfg.Hum[0], new[] { 11, 12, 13, 14, 15, 16, 17 });
        ItemEatCDLogic.SetSlotValues(cfg.Hum[1], new[] { 21, 22, 23, 24, 25, 26, 27 });
        ItemEatCDLogic.SetSlotValues(cfg.Hum[2], new[] { 31, 32, 33, 34, 35, 36, 37 });
        ItemEatCDLogic.SetSlotValues(cfg.Hero[0], new[] { 41, 42, 43, 44, 45, 46, 47 });
        ItemEatCDLogic.SetSlotValues(cfg.Hero[1], new[] { 51, 52, 53, 54, 55, 56, 57 });
        ItemEatCDLogic.SetSlotValues(cfg.Hero[2], new[] { 61, 62, 63, 64, 65, 66, 67 });

        ItemEatCDLogic.WriteIni(IniPath, cfg);

        string expected =
            "[HumanItemEatCD]\r\n" +
            "1NormalHP=11\r\n1NormalMP=12\r\n1NormalHPMP=13\r\n1SpecialHP=14\r\n1SpecialMP=15\r\n1SpecialHPMP=16\r\n1Other=17\r\n" +
            "2NormalHP=21\r\n2NormalMP=22\r\n2NormalHPMP=23\r\n2SpecialHP=24\r\n2SpecialMP=25\r\n2SpecialHPMP=26\r\n2Other=27\r\n" +
            "3NormalHP=31\r\n3NormalMP=32\r\n3NormalHPMP=33\r\n3SpecialHP=34\r\n3SpecialMP=35\r\n3SpecialHPMP=36\r\n3Other=37\r\n" +
            "\r\n" +
            "[HeroItemEatCD]\r\n" +
            "1NormalHP=41\r\n1NormalMP=42\r\n1NormalHPMP=43\r\n1SpecialHP=44\r\n1SpecialMP=45\r\n1SpecialHPMP=46\r\n1Other=47\r\n" +
            "2NormalHP=51\r\n2NormalMP=52\r\n2NormalHPMP=53\r\n2SpecialHP=54\r\n2SpecialMP=55\r\n2SpecialHPMP=56\r\n2Other=57\r\n" +
            "3NormalHP=61\r\n3NormalMP=62\r\n3NormalHPMP=63\r\n3SpecialHP=64\r\n3SpecialMP=65\r\n3SpecialHPMP=66\r\n3Other=67\r\n" +
            "\r\n";
        Assert.Equal(expected, IniText);
    }

    [Fact]
    public void WriteIni_负值原样写出()
    {
        var cfg = new TEatItemCDConfig();
        cfg.Hum[0].NormalHP = -5;                      // 原文 IntToStr 无符号性约束
        ItemEatCDLogic.WriteIni(IniPath, cfg);
        Assert.Contains("1NormalHP=-5\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void WriteIni_空文件名不抛异常()
    {
        var ex = Record.Exception(() => ItemEatCDLogic.WriteIni("", new TEatItemCDConfig()));
        Assert.Null(ex);
    }

    // ---------------- ButtonOK（原 :130-211）----------------

    [Fact]
    public void ButtonOK_六个slot按序写回()
    {
        var cfg = new TEatItemCDConfig();
        var hum = new[]
        {
            new[] { 1, 1, 1, 1, 1, 1, 1 },
            new[] { 2, 2, 2, 2, 2, 2, 2 },
            new[] { 3, 3, 3, 3, 3, 3, 3 }
        };
        var hero = new[]
        {
            new[] { 4, 4, 4, 4, 4, 4, 4 },
            new[] { 5, 5, 5, 5, 5, 5, 5 },
            new[] { 6, 6, 6, 6, 6, 6, 6 }
        };
        ItemEatCDLogic.ButtonOK(hum, hero, cfg, IniPath);

        Assert.Equal(1, cfg.Hum[0].NormalHP);
        Assert.Equal(2, cfg.Hum[1].NormalMP);
        Assert.Equal(3, cfg.Hum[2].Other);
        Assert.Equal(4, cfg.Hero[0].NormalHP);
        Assert.Equal(5, cfg.Hero[1].SpecialHPMP);
        Assert.Equal(6, cfg.Hero[2].Other);

        Assert.Contains("1NormalHP=1\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("2NormalMP=2\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("3Other=3\r\n", IniText, StringComparison.Ordinal);
        // Hero slot 0 的值是 4；键前缀仍是 1..3（下标 + 1），与 slot 值无关
        Assert.Contains("[HeroItemEatCD]\r\n1NormalHP=4\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("2SpecialHPMP=5\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("3Other=6\r\n", IniText, StringComparison.Ordinal);
        Assert.DoesNotContain("7Other", IniText);                 // 不存在 7 前缀（只有 1..3）
    }

    // ---------------- 窗体（DFM 对齐 + 端到端）----------------

    [Fact]
    public void 窗体_DFM属性与六个分组框对齐()
    {
        using var f = new FrmItemEatCD();

        Assert.Equal("吃药CD设置", f.Text);                       // DFM: Caption
        Assert.Equal(530, f.ClientSize.Width);                    // DFM: ClientWidth=530
        Assert.Equal(466, f.ClientSize.Height);                   // DFM: ClientHeight=466

        Assert.NotNull(f.grpHum);
        Assert.NotNull(f.grpHero);
        Assert.NotNull(f.grp2);
        Assert.NotNull(f.GroupBox1);
        Assert.NotNull(f.GroupBox2);
        Assert.NotNull(f.GroupBox4);
        Assert.NotNull(f.GroupBox5);
        Assert.NotNull(f.GroupBox6);
        Assert.NotNull(f.btnOK);

        Assert.Equal("人物吃药CD设置 [毫秒]", f.grpHum.Text);
        Assert.Equal("英雄吃药CD设置 [毫秒]", f.grpHero.Text);
        Assert.Equal("战士", f.grp2.Text);                        // DFM: grp2 Caption='战士'
        Assert.Equal("法师", f.GroupBox1.Text);                   // DFM: GroupBox1 Caption='法师'
        Assert.Equal("道士", f.GroupBox2.Text);                   // DFM: GroupBox2 Caption='道士'
        Assert.Equal("战士", f.GroupBox4.Text);
        Assert.Equal("法师", f.GroupBox5.Text);
        Assert.Equal("道士", f.GroupBox6.Text);
        Assert.Equal("确定", f.btnOK.Text);

        // 几何：grpHum Left=7 Top=8 W=515 H=206；grpHero Left=7 Top=220 W=515 H=206
        Assert.Equal(7, f.grpHum.Left);
        Assert.Equal(8, f.grpHum.Top);
        Assert.Equal(515, f.grpHum.Width);
        Assert.Equal(206, f.grpHum.Height);
        Assert.Equal(220, f.grpHero.Top);
        // btnOK Left=447 Top=432 W=75 H=25
        Assert.Equal(447, f.btnOK.Left);
        Assert.Equal(432, f.btnOK.Top);
    }

    [Fact]
    public void 窗体_84个编辑框与42个标签全部非空()
    {
        using var f = new FrmItemEatCD();

        // 42 个标签（名称严格按 .dfm）
        foreach (var l in new[] { f.lbl1, f.Label1, f.Label2, f.Label30, f.Label31, f.Label32, f.Label4,
                                  f.Label5, f.Label6, f.Label7, f.Label10, f.Label11, f.Label12, f.Label9,
                                  f.Label13, f.Label14, f.Label33, f.Label36, f.Label37, f.Label38, f.Label35,
                                  f.Label3, f.Label8, f.Label15, f.Label17, f.Label18, f.Label19, f.Label16,
                                  f.Label20, f.Label21, f.Label22, f.Label24, f.Label25, f.Label26, f.Label23,
                                  f.Label27, f.Label28, f.Label29, f.Label39, f.Label40, f.Label41, f.Label34 })
            Assert.NotNull(l);

        // 84 个 TSpinEditEx = 2 组（Hum/Hero）× 3 职业 × 7 字段
        var all = new System.Collections.Generic.List<TSpinEditEx>
        {
            // Hum 战士 / 法师 / 道士
            f.seHumNormalHP0, f.seHumNormalMP0, f.seHumNormalHPMP0, f.seHumSpecialHP0, f.seHumSpecialMP0, f.seHumSpecialHPMP0, f.seHumOther0,
            f.seHumNormalHP1, f.seHumNormalMP1, f.seHumNormalHPMP1, f.seHumSpecialHP1, f.seHumSpecialMP1, f.seHumSpecialHPMP1, f.seHumOther1,
            f.seHumNormalHP2, f.seHumNormalMP2, f.seHumNormalHPMP2, f.seHumSpecialHP2, f.seHumSpecialMP2, f.seHumSpecialHPMP2, f.seHumOther2,
            // Hero 战士 / 法师 / 道士
            f.seHeroNormalHP0, f.seHeroNormalMP0, f.seHeroNormalHPMP0, f.seHeroSpecialHP0, f.seHeroSpecialMP0, f.seHeroSpecialHPMP0, f.seHeroOther0,
            f.seHeroNormalHP1, f.seHeroNormalMP1, f.seHeroNormalHPMP1, f.seHeroSpecialHP1, f.seHeroSpecialMP1, f.seHeroSpecialHPMP1, f.seHeroOther1,
            f.seHeroNormalHP2, f.seHeroNormalMP2, f.seHeroNormalHPMP2, f.seHeroSpecialHP2, f.seHeroSpecialMP2, f.seHeroSpecialHPMP2, f.seHeroOther2
        };
        foreach (var s in all) Assert.NotNull(s);
        Assert.Equal(2 * 3 * 7, all.Count);       // 42 个字段（每组的 7 个字段名互不相同）

        // DFM: 每个 TSpinEditEx 都是 MaxValue=0 MinValue=0
        Assert.Equal(0, f.seHumNormalHP0.DfmMinValue);
        Assert.Equal(0, f.seHumNormalHP0.DfmMaxValue);
        Assert.Equal(0, f.seHeroOther2.DfmMinValue);
        Assert.Equal(0, f.seHeroOther2.DfmMaxValue);
    }

    [Fact]
    public void 窗体_FormCreate把全局配置灌进84个控件()
    {
        var cfg = FormGlobals.g_EatItemCDConfig;
        ItemEatCDLogic.SetSlotValues(cfg.Hum[0], new[] { 101, 102, 103, 104, 105, 106, 107 });
        ItemEatCDLogic.SetSlotValues(cfg.Hum[1], new[] { 111, 112, 113, 114, 115, 116, 117 });
        ItemEatCDLogic.SetSlotValues(cfg.Hum[2], new[] { 121, 122, 123, 124, 125, 126, 127 });
        ItemEatCDLogic.SetSlotValues(cfg.Hero[0], new[] { 201, 202, 203, 204, 205, 206, 207 });
        ItemEatCDLogic.SetSlotValues(cfg.Hero[1], new[] { 211, 212, 213, 214, 215, 216, 217 });
        ItemEatCDLogic.SetSlotValues(cfg.Hero[2], new[] { 221, 222, 223, 224, 225, 226, 227 });

        using var f = new FrmItemEatCD();
        f.FormCreate(f, EventArgs.Empty);

        Assert.Equal(101, f.seHumNormalHP0.Value);
        Assert.Equal(107, f.seHumOther0.Value);
        Assert.Equal(117, f.seHumOther1.Value);
        Assert.Equal(127, f.seHumOther2.Value);
        Assert.Equal(201, f.seHeroNormalHP0.Value);
        Assert.Equal(217, f.seHeroOther1.Value);
        Assert.Equal(227, f.seHeroOther2.Value);
    }

    [Fact]
    public void 窗体_btnOK_Click把84个控件写回配置与INI()
    {
        FormGlobals.g_sIniFileName = IniPath;

        using var f = new FrmItemEatCD();
        f.seHumNormalHP0.Value = 11;
        f.seHumNormalMP0.Value = 12;
        f.seHumNormalHPMP0.Value = 13;
        f.seHumSpecialHP0.Value = 14;
        f.seHumSpecialMP0.Value = 15;
        f.seHumSpecialHPMP0.Value = 16;
        f.seHumOther0.Value = 17;
        f.seHeroOther2.Value = 67;

        f.btnOK_Click(f, EventArgs.Empty);

        var cfg = FormGlobals.g_EatItemCDConfig;
        Assert.Equal(11, cfg.Hum[0].NormalHP);
        Assert.Equal(12, cfg.Hum[0].NormalMP);
        Assert.Equal(13, cfg.Hum[0].NormalHPMP);
        Assert.Equal(14, cfg.Hum[0].SpecialHP);
        Assert.Equal(15, cfg.Hum[0].SpecialMP);
        Assert.Equal(16, cfg.Hum[0].SpecialHPMP);
        Assert.Equal(17, cfg.Hum[0].Other);
        Assert.Equal(67, cfg.Hero[2].Other);

        Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);    // 原 :210
        Assert.Contains("[HumanItemEatCD]\r\n1NormalHP=11\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("3Other=67\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void 窗体_ReadControlValues顺序与FieldSuffixes一致()
    {
        using var f = new FrmItemEatCD();
        f.seHumNormalHP0.Value = 1;
        f.seHumNormalMP0.Value = 2;
        f.seHumNormalHPMP0.Value = 3;
        f.seHumSpecialHP0.Value = 4;
        f.seHumSpecialMP0.Value = 5;
        f.seHumSpecialHPMP0.Value = 6;
        f.seHumOther0.Value = 7;

        var values = f.ReadControlValues();
        Assert.Equal(6, values.Length);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7 }, values[0]);
    }

    [Fact]
    public void 窗体_编程赋值远超DFM的0到0范围也不裁剪()
    {
        using var f = new FrmItemEatCD();
        f.seHumNormalHP0.Value = 999999;
        Assert.Equal(999999, f.seHumNormalHP0.Value);     // TSpinEditEx 语义：编程赋值不裁剪
    }
}

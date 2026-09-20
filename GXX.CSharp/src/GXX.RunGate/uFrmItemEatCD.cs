using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmItemEatCD.pas 1:1 转换（Source\RunGate\uFrmItemEatCD.pas，265 行 / LF 264）。
// 布局真源：Source\RunGate\uFrmItemEatCD.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：人物 / 英雄 两套 × 战士 / 法师 / 道士 三职业 × 7 类药物 = 84 个"吃药 CD（毫秒）"编辑框，
// 确定时写回 `g_EatItemCDConfig` 并落盘 Config.ini 的两个节：
//   [HumanItemEatCD] / [HeroItemEatCD]，每节 3 组（键前缀 '1'/'2'/'3'）× 7 键。
//
// ★ 原文要点（照抄 + 差异断言）：
//   D1. `btnOKClick`（原 :136-182）**逐个人工赋值** 84 次（不是循环）——
//       顺序是 Hum[0..2] 再 Hero[0..2]，每项 7 个字段固定顺序：
//       NormalHP, NormalMP, NormalHPMP, SpecialHP, SpecialMP, SpecialHPMP, Other。
//   D2. 写 INI 的键名是 `IntToStr(I + 1) + 'NormalHP'` 等 → '1NormalHP'..'3Other'
//       （**键前缀从 1 开始**，且**没有分隔符** —— 不是 '1_NormalHP'）。
//   D3. `FormCreate`（原 :213-262）同样是 84 次人工赋值（控件 ← 全局），顺序与 D1 对应。
//   D4. DFM 里 84 个 TSpinEditEx 全是 `MaxValue=0 MinValue=0 Value=0`：
//       与 GXX.DBServer 的 TSpinEdit「编程赋值不裁剪」语义一致 ——
//       即使配置里是 12345，也会原样显示（不会被裁到 0）。差异断言见测试。
//   D5. `.dfm` 的 GroupBox 标题是**职业名**：grp2/GroupBox4='战士'，GroupBox1/GroupBox5='法师'，
//       GroupBox2/GroupBox6='道士'；两个外层 GroupBox Caption 分别为
//       '人物吃药CD设置 [毫秒]' / '英雄吃药CD设置 [毫秒]'。
//   D6. 7 类药物的**控件声明顺序**与**视觉行顺序不同**：视觉上（Top）依次是
//       NormalHP(12) / NormalMP(36) / NormalHPMP(60) / SpecialHP(84) / SpecialMP(108) /
//       SpecialHPMP(132) / Other(156)。本移植按视觉顺序创建，标签名严格按 .dfm。
//   D7. 本窗体**没有**任何校验分支，也**没有**弹窗（无 MessageBox/InputQuery），
//       所有输入都直通 INI —— 故本文件不涉及 MessageBoxSeam 的注入（保留说明）。
// =====================================================================================

/// <summary>uFrmItemEatCD.pas 的 84 个编辑框取值（与 `TEatItemCDConfig` 同构）。</summary>
public static class ItemEatCDLogic
{
    /// <summary>原 :185-207 的节名（**逐字照抄**）。</summary>
    public const string HumanSection = "HumanItemEatCD";
    public const string HeroSection = "HeroItemEatCD";

    /// <summary>原 :188-194 / :200-206 的 7 个键后缀（顺序即原文顺序）。</summary>
    public static readonly string[] FieldSuffixes =
    {
        "NormalHP", "NormalMP", "NormalHPMP", "SpecialHP", "SpecialMP", "SpecialHPMP", "Other"
    };

    /// <summary>原 :187 / :199 `StrIndex := IntToStr(I + 1)` —— 键前缀从 '1' 开始。</summary>
    public static string KeyPrefix(int index) => DelphiRTL.IntToStr(index + 1);

    /// <summary>完整键名（如 '1NormalHP'、'3Other'）—— **无分隔符**，见 D2。</summary>
    public static string KeyName(int index, string suffix) => KeyPrefix(index) + suffix;

    /// <summary>取某个 slot 的 7 个值（顺序与 <see cref="FieldSuffixes"/> 一致）。</summary>
    public static int[] GetSlotValues(TItemCDTime t)
    {
        return new[] { t.NormalHP, t.NormalMP, t.NormalHPMP, t.SpecialHP, t.SpecialMP, t.SpecialHPMP, t.Other };
    }

    /// <summary>写回某个 slot 的 7 个值。</summary>
    public static void SetSlotValues(TItemCDTime t, int[] values)
    {
        if (values == null || values.Length < 7) throw new ArgumentException("values 需要 7 项", nameof(values));
        t.NormalHP = values[0];       // 原 :136-142 / :160-166
        t.NormalMP = values[1];
        t.NormalHPMP = values[2];
        t.SpecialHP = values[3];
        t.SpecialMP = values[4];
        t.SpecialHPMP = values[5];
        t.Other = values[6];
    }

    /// <summary>原 :184-208 `btnOKClick` 的 INI 落盘（两节 × 3 组 × 7 键，键序逐字节照抄）。</summary>
    public static void WriteIni(string iniFileName, TEatItemCDConfig config)
    {
        var ini = new TIniFileEx(iniFileName);                                        // 原 :184
        try
        {
            for (int i = 0; i < TEatItemCDConfig.SlotCount; i++)                      // 原 :185-195
            {
                string strIndex = KeyPrefix(i);                                       // 原 :187
                var t = config.Hum[i];
                ini.WriteInteger(HumanSection, strIndex + "NormalHP", t.NormalHP);     // 原 :188
                ini.WriteInteger(HumanSection, strIndex + "NormalMP", t.NormalMP);     // 原 :189
                ini.WriteInteger(HumanSection, strIndex + "NormalHPMP", t.NormalHPMP); // 原 :190
                ini.WriteInteger(HumanSection, strIndex + "SpecialHP", t.SpecialHP);   // 原 :191
                ini.WriteInteger(HumanSection, strIndex + "SpecialMP", t.SpecialMP);   // 原 :192
                ini.WriteInteger(HumanSection, strIndex + "SpecialHPMP", t.SpecialHPMP); // 原 :193
                ini.WriteInteger(HumanSection, strIndex + "Other", t.Other);           // 原 :194
            }

            for (int i = 0; i < TEatItemCDConfig.SlotCount; i++)                      // 原 :197-207
            {
                string strIndex = KeyPrefix(i);                                       // 原 :199
                var t = config.Hero[i];
                ini.WriteInteger(HeroSection, strIndex + "NormalHP", t.NormalHP);      // 原 :200
                ini.WriteInteger(HeroSection, strIndex + "NormalMP", t.NormalMP);      // 原 :201
                ini.WriteInteger(HeroSection, strIndex + "NormalHPMP", t.NormalHPMP);  // 原 :202
                ini.WriteInteger(HeroSection, strIndex + "SpecialHP", t.SpecialHP);    // 原 :203
                ini.WriteInteger(HeroSection, strIndex + "SpecialMP", t.SpecialMP);    // 原 :204
                ini.WriteInteger(HeroSection, strIndex + "SpecialHPMP", t.SpecialHPMP); // 原 :205
                ini.WriteInteger(HeroSection, strIndex + "Other", t.Other);            // 原 :206
            }
        }
        finally
        {
            ini.Dispose();                                                            // 原 :208 IniFile.Free
        }
    }

    /// <summary>原 :130-211 `btnOKClick` 的完整流程（控件取值由调用方提供）。</summary>
    public static void ButtonOK(int[][] humSlots, int[][] heroSlots, TEatItemCDConfig config, string iniFileName)
    {
        for (int i = 0; i < TEatItemCDConfig.SlotCount; i++)      // 原 :136-158
            SetSlotValues(config.Hum[i], humSlots[i]);
        for (int i = 0; i < TEatItemCDConfig.SlotCount; i++)      // 原 :160-182
            SetSlotValues(config.Hero[i], heroSlots[i]);
        WriteIni(iniFileName, config);                           // 原 :184-208
    }
}

/// <summary>原 :118-128 `function ShowFrmItemEatCD: Boolean;`。</summary>
public static class ItemEatCDUnit
{
    public static bool ShowFrmItemEatCD()
    {
        using var form = new FrmItemEatCD();
        return form.ShowDialog() == DialogResult.OK;      // 原 :124
    }
}

/// <summary>原 uFrmItemEatCD.pas:10-110 `TFrmItemEatCD`（DFM: uFrmItemEatCD.dfm）。</summary>
public class FrmItemEatCD : Form
{
    // DFM: FrmItemEatCD Left=495 Top=338 BorderStyle=bsDialog Caption='吃药CD设置'
    //      ClientHeight=466 ClientWidth=530 Font.Charset=GB2312_CHARSET Font.Height=-12 Font.Name='宋体'
    //      Position=poMainFormCenter OnCreate=FormCreate PixelsPerInch=96
    public GroupBox grpHum;    // DFM: grpHum Left=7 Top=8 Width=515 Height=206 Caption='人物吃药CD设置 [毫秒]' TabOrder=0
    public GroupBox grpHero;   // DFM: grpHero Left=7 Top=220 Width=515 Height=206 Caption='英雄吃药CD设置 [毫秒]' TabOrder=1
    public GroupBox grp2;      // DFM: grp2 (在 grpHum 内) Left=8 Top=15 Width=162 Height=184 Caption='战士' TabOrder=0
    public GroupBox GroupBox1; // DFM: GroupBox1 (在 grpHum 内) Left=176 Top=15 Width=162 Height=184 Caption='法师' TabOrder=1
    public GroupBox GroupBox2; // DFM: GroupBox2 (在 grpHum 内) Left=344 Top=15 Width=162 Height=184 Caption='道士' TabOrder=2
    public GroupBox GroupBox4; // DFM: GroupBox4 (在 grpHero 内) Left=8 Top=15 Width=162 Height=184 Caption='战士' TabOrder=0
    public GroupBox GroupBox5; // DFM: GroupBox5 (在 grpHero 内) Left=176 Top=15 Width=162 Height=184 Caption='法师' TabOrder=1
    public GroupBox GroupBox6; // DFM: GroupBox6 (在 grpHero 内) Left=344 Top=15 Width=162 Height=184 Caption='道士' TabOrder=2
    public Button btnOK;       // DFM: btnOK Left=447 Top=432 Width=75 Height=25 Caption='确定' TabOrder=2 OnClick=btnOKClick

    // 42 个标签（名称严格按 .dfm）
    public Label lbl1, Label1, Label2, Label30, Label31, Label32, Label4;
    public Label Label5, Label6, Label7, Label10, Label11, Label12, Label9;
    public Label Label13, Label14, Label33, Label36, Label37, Label38, Label35;
    public Label Label3, Label8, Label15, Label17, Label18, Label19, Label16;
    public Label Label20, Label21, Label22, Label24, Label25, Label26, Label23;
    public Label Label27, Label28, Label29, Label39, Label40, Label41, Label34;

    // 84 个 TSpinEditEx（名称严格按 .dfm）
    public TSpinEditEx seHumNormalHP0, seHumNormalMP0, seHumNormalHPMP0, seHumSpecialHP0, seHumSpecialMP0, seHumSpecialHPMP0, seHumOther0;
    public TSpinEditEx seHumNormalHP1, seHumNormalMP1, seHumNormalHPMP1, seHumSpecialHP1, seHumSpecialMP1, seHumSpecialHPMP1, seHumOther1;
    public TSpinEditEx seHumNormalHP2, seHumNormalMP2, seHumNormalHPMP2, seHumSpecialHP2, seHumSpecialMP2, seHumSpecialHPMP2, seHumOther2;
    public TSpinEditEx seHeroNormalHP0, seHeroNormalMP0, seHeroNormalHPMP0, seHeroSpecialHP0, seHeroSpecialMP0, seHeroSpecialHPMP0, seHeroOther0;
    public TSpinEditEx seHeroNormalHP1, seHeroNormalMP1, seHeroNormalHPMP1, seHeroSpecialHP1, seHeroSpecialMP1, seHeroSpecialHPMP1, seHeroOther1;
    public TSpinEditEx seHeroNormalHP2, seHeroNormalMP2, seHeroNormalHPMP2, seHeroSpecialHP2, seHeroSpecialMP2, seHeroSpecialHPMP2, seHeroOther2;

    public FrmItemEatCD()
    {
        // DFM: FrmItemEatCD Caption='吃药CD设置' BorderStyle=bsDialog Position=poMainFormCenter
        Text = "吃药CD设置";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        Location = new Point(495, 338);
        ClientSize = new Size(530, 466);
        Font = new Font("宋体", 9F);                      // Font.Height=-12 Font.Name='宋体'
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: grpHum Left=7 Top=8 Width=515 Height=206 Caption='人物吃药CD设置 [毫秒]' TabOrder=0
        grpHum = new GroupBox { Left = 7, Top = 8, Width = 515, Height = 206,
                                Text = "人物吃药CD设置 [毫秒]", TabIndex = 0 };
        // DFM: grpHero Left=7 Top=220 Width=515 Height=206 Caption='英雄吃药CD设置 [毫秒]' TabOrder=1
        grpHero = new GroupBox { Left = 7, Top = 220, Width = 515, Height = 206,
                                 Text = "英雄吃药CD设置 [毫秒]", TabIndex = 1 };
        // DFM: btnOK Left=447 Top=432 Width=75 Height=25 Caption='确定' TabOrder=2 OnClick=btnOKClick
        btnOK = new Button { Left = 447, Top = 432, Width = 75, Height = 25, Text = "确定", TabIndex = 2 };

        // ==== 84 个编辑框 + 42 个标签（按 .dfm 逐条创建）====
        // ---- Hum ----
        grp2 = new GroupBox { Left = 8, Top = 15, Width = 162, Height = 184, Text = "战士" };
        // DFM: lbl1 Left=8 Top=17 Width=72 Height=12 Caption='普通HP药物：'
        lbl1 = new Label { Left = 8, Top = 17, Width = 72, Height = 12, Text = "普通HP药物：" };
        // DFM: Label1 Left=8 Top=41 Width=72 Height=12 Caption='普通MP药物：'
        Label1 = new Label { Left = 8, Top = 41, Width = 72, Height = 12, Text = "普通MP药物：" };
        // DFM: Label2 Left=8 Top=65 Width=72 Height=12 Caption='普通HPMP药物：'
        Label2 = new Label { Left = 8, Top = 65, Width = 72, Height = 12, Text = "普通HPMP药物：" };
        // DFM: Label30 Left=8 Top=89 Width=72 Height=12 Caption='特殊HP药物：'
        Label30 = new Label { Left = 8, Top = 89, Width = 72, Height = 12, Text = "特殊HP药物：" };
        // DFM: Label31 Left=8 Top=112 Width=72 Height=12 Caption='特殊MP药物：'
        Label31 = new Label { Left = 8, Top = 112, Width = 72, Height = 12, Text = "特殊MP药物：" };
        // DFM: Label32 Left=8 Top=136 Width=72 Height=12 Caption='特殊HPMP药物：'
        Label32 = new Label { Left = 8, Top = 136, Width = 72, Height = 12, Text = "特殊HPMP药物：" };
        // DFM: Label4 Left=8 Top=160 Width=72 Height=12 Caption='其它类药物：'
        Label4 = new Label { Left = 8, Top = 160, Width = 72, Height = 12, Text = "其它类药物：" };
        // DFM: seHumNormalHP0 Left=79 Top=12 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumNormalHP0 = new TSpinEditEx { Left = 79, Top = 12, Width = 76, Height = 21 };
        seHumNormalHP0.SetDfmRange(0, 0);
        // DFM: seHumNormalMP0 Left=79 Top=36 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumNormalMP0 = new TSpinEditEx { Left = 79, Top = 36, Width = 76, Height = 21 };
        seHumNormalMP0.SetDfmRange(0, 0);
        // DFM: seHumNormalHPMP0 Left=79 Top=60 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumNormalHPMP0 = new TSpinEditEx { Left = 79, Top = 60, Width = 76, Height = 21 };
        seHumNormalHPMP0.SetDfmRange(0, 0);
        // DFM: seHumSpecialHP0 Left=79 Top=84 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumSpecialHP0 = new TSpinEditEx { Left = 79, Top = 84, Width = 76, Height = 21 };
        seHumSpecialHP0.SetDfmRange(0, 0);
        // DFM: seHumSpecialMP0 Left=79 Top=108 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumSpecialMP0 = new TSpinEditEx { Left = 79, Top = 108, Width = 76, Height = 21 };
        seHumSpecialMP0.SetDfmRange(0, 0);
        // DFM: seHumSpecialHPMP0 Left=79 Top=132 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumSpecialHPMP0 = new TSpinEditEx { Left = 79, Top = 132, Width = 76, Height = 21 };
        seHumSpecialHPMP0.SetDfmRange(0, 0);
        // DFM: seHumOther0 Left=79 Top=156 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumOther0 = new TSpinEditEx { Left = 79, Top = 156, Width = 76, Height = 21 };
        seHumOther0.SetDfmRange(0, 0);
        grp2.Controls.AddRange(new Control[] { lbl1, Label1, Label2, Label30, Label31, Label32, Label4,
            seHumNormalHP0, seHumNormalMP0, seHumNormalHPMP0, seHumSpecialHP0, seHumSpecialMP0, seHumSpecialHPMP0, seHumOther0 });
        GroupBox1 = new GroupBox { Left = 176, Top = 15, Width = 162, Height = 184, Text = "法师" };
        // DFM: Label5 Left=8 Top=17 Width=72 Height=12 Caption='普通HP药物：'
        Label5 = new Label { Left = 8, Top = 17, Width = 72, Height = 12, Text = "普通HP药物：" };
        // DFM: Label6 Left=8 Top=41 Width=72 Height=12 Caption='普通MP药物：'
        Label6 = new Label { Left = 8, Top = 41, Width = 72, Height = 12, Text = "普通MP药物：" };
        // DFM: Label7 Left=8 Top=65 Width=72 Height=12 Caption='普通HPMP药物：'
        Label7 = new Label { Left = 8, Top = 65, Width = 72, Height = 12, Text = "普通HPMP药物：" };
        // DFM: Label10 Left=8 Top=89 Width=72 Height=12 Caption='特殊HP药物：'
        Label10 = new Label { Left = 8, Top = 89, Width = 72, Height = 12, Text = "特殊HP药物：" };
        // DFM: Label11 Left=8 Top=112 Width=72 Height=12 Caption='特殊MP药物：'
        Label11 = new Label { Left = 8, Top = 112, Width = 72, Height = 12, Text = "特殊MP药物：" };
        // DFM: Label12 Left=8 Top=136 Width=72 Height=12 Caption='特殊HPMP药物：'
        Label12 = new Label { Left = 8, Top = 136, Width = 72, Height = 12, Text = "特殊HPMP药物：" };
        // DFM: Label9 Left=8 Top=160 Width=72 Height=12 Caption='其它类药物：'
        Label9 = new Label { Left = 8, Top = 160, Width = 72, Height = 12, Text = "其它类药物：" };
        // DFM: seHumNormalHP1 Left=79 Top=12 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumNormalHP1 = new TSpinEditEx { Left = 79, Top = 12, Width = 76, Height = 21 };
        seHumNormalHP1.SetDfmRange(0, 0);
        // DFM: seHumNormalMP1 Left=79 Top=36 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumNormalMP1 = new TSpinEditEx { Left = 79, Top = 36, Width = 76, Height = 21 };
        seHumNormalMP1.SetDfmRange(0, 0);
        // DFM: seHumNormalHPMP1 Left=79 Top=60 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumNormalHPMP1 = new TSpinEditEx { Left = 79, Top = 60, Width = 76, Height = 21 };
        seHumNormalHPMP1.SetDfmRange(0, 0);
        // DFM: seHumSpecialHP1 Left=79 Top=84 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumSpecialHP1 = new TSpinEditEx { Left = 79, Top = 84, Width = 76, Height = 21 };
        seHumSpecialHP1.SetDfmRange(0, 0);
        // DFM: seHumSpecialMP1 Left=79 Top=108 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumSpecialMP1 = new TSpinEditEx { Left = 79, Top = 108, Width = 76, Height = 21 };
        seHumSpecialMP1.SetDfmRange(0, 0);
        // DFM: seHumSpecialHPMP1 Left=79 Top=132 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumSpecialHPMP1 = new TSpinEditEx { Left = 79, Top = 132, Width = 76, Height = 21 };
        seHumSpecialHPMP1.SetDfmRange(0, 0);
        // DFM: seHumOther1 Left=79 Top=156 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumOther1 = new TSpinEditEx { Left = 79, Top = 156, Width = 76, Height = 21 };
        seHumOther1.SetDfmRange(0, 0);
        GroupBox1.Controls.AddRange(new Control[] { Label5, Label6, Label7, Label10, Label11, Label12, Label9,
            seHumNormalHP1, seHumNormalMP1, seHumNormalHPMP1, seHumSpecialHP1, seHumSpecialMP1, seHumSpecialHPMP1, seHumOther1 });
        GroupBox2 = new GroupBox { Left = 344, Top = 15, Width = 162, Height = 184, Text = "道士" };
        // DFM: Label13 Left=8 Top=17 Width=72 Height=12 Caption='普通HP药物：'
        Label13 = new Label { Left = 8, Top = 17, Width = 72, Height = 12, Text = "普通HP药物：" };
        // DFM: Label14 Left=8 Top=41 Width=72 Height=12 Caption='普通MP药物：'
        Label14 = new Label { Left = 8, Top = 41, Width = 72, Height = 12, Text = "普通MP药物：" };
        // DFM: Label33 Left=8 Top=65 Width=72 Height=12 Caption='普通HPMP药物：'
        Label33 = new Label { Left = 8, Top = 65, Width = 72, Height = 12, Text = "普通HPMP药物：" };
        // DFM: Label36 Left=8 Top=89 Width=72 Height=12 Caption='特殊HP药物：'
        Label36 = new Label { Left = 8, Top = 89, Width = 72, Height = 12, Text = "特殊HP药物：" };
        // DFM: Label37 Left=8 Top=112 Width=72 Height=12 Caption='特殊MP药物：'
        Label37 = new Label { Left = 8, Top = 112, Width = 72, Height = 12, Text = "特殊MP药物：" };
        // DFM: Label38 Left=8 Top=136 Width=72 Height=12 Caption='特殊HPMP药物：'
        Label38 = new Label { Left = 8, Top = 136, Width = 72, Height = 12, Text = "特殊HPMP药物：" };
        // DFM: Label35 Left=8 Top=160 Width=72 Height=12 Caption='其它类药物：'
        Label35 = new Label { Left = 8, Top = 160, Width = 72, Height = 12, Text = "其它类药物：" };
        // DFM: seHumNormalHP2 Left=79 Top=12 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumNormalHP2 = new TSpinEditEx { Left = 79, Top = 12, Width = 76, Height = 21 };
        seHumNormalHP2.SetDfmRange(0, 0);
        // DFM: seHumNormalMP2 Left=79 Top=36 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumNormalMP2 = new TSpinEditEx { Left = 79, Top = 36, Width = 76, Height = 21 };
        seHumNormalMP2.SetDfmRange(0, 0);
        // DFM: seHumNormalHPMP2 Left=79 Top=60 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumNormalHPMP2 = new TSpinEditEx { Left = 79, Top = 60, Width = 76, Height = 21 };
        seHumNormalHPMP2.SetDfmRange(0, 0);
        // DFM: seHumSpecialHP2 Left=79 Top=84 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumSpecialHP2 = new TSpinEditEx { Left = 79, Top = 84, Width = 76, Height = 21 };
        seHumSpecialHP2.SetDfmRange(0, 0);
        // DFM: seHumSpecialMP2 Left=79 Top=108 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumSpecialMP2 = new TSpinEditEx { Left = 79, Top = 108, Width = 76, Height = 21 };
        seHumSpecialMP2.SetDfmRange(0, 0);
        // DFM: seHumSpecialHPMP2 Left=79 Top=132 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumSpecialHPMP2 = new TSpinEditEx { Left = 79, Top = 132, Width = 76, Height = 21 };
        seHumSpecialHPMP2.SetDfmRange(0, 0);
        // DFM: seHumOther2 Left=79 Top=156 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHumOther2 = new TSpinEditEx { Left = 79, Top = 156, Width = 76, Height = 21 };
        seHumOther2.SetDfmRange(0, 0);
        GroupBox2.Controls.AddRange(new Control[] { Label13, Label14, Label33, Label36, Label37, Label38, Label35,
            seHumNormalHP2, seHumNormalMP2, seHumNormalHPMP2, seHumSpecialHP2, seHumSpecialMP2, seHumSpecialHPMP2, seHumOther2 });
        // ---- Hero ----
        GroupBox4 = new GroupBox { Left = 8, Top = 15, Width = 162, Height = 184, Text = "战士" };
        // DFM: Label3 Left=8 Top=17 Width=72 Height=12 Caption='普通HP药物：'
        Label3 = new Label { Left = 8, Top = 17, Width = 72, Height = 12, Text = "普通HP药物：" };
        // DFM: Label8 Left=8 Top=41 Width=72 Height=12 Caption='普通MP药物：'
        Label8 = new Label { Left = 8, Top = 41, Width = 72, Height = 12, Text = "普通MP药物：" };
        // DFM: Label15 Left=8 Top=65 Width=72 Height=12 Caption='普通HPMP药物：'
        Label15 = new Label { Left = 8, Top = 65, Width = 72, Height = 12, Text = "普通HPMP药物：" };
        // DFM: Label17 Left=8 Top=89 Width=72 Height=12 Caption='特殊HP药物：'
        Label17 = new Label { Left = 8, Top = 89, Width = 72, Height = 12, Text = "特殊HP药物：" };
        // DFM: Label18 Left=8 Top=112 Width=72 Height=12 Caption='特殊MP药物：'
        Label18 = new Label { Left = 8, Top = 112, Width = 72, Height = 12, Text = "特殊MP药物：" };
        // DFM: Label19 Left=8 Top=136 Width=72 Height=12 Caption='特殊HPMP药物：'
        Label19 = new Label { Left = 8, Top = 136, Width = 72, Height = 12, Text = "特殊HPMP药物：" };
        // DFM: Label16 Left=8 Top=160 Width=72 Height=12 Caption='其它类药物：'
        Label16 = new Label { Left = 8, Top = 160, Width = 72, Height = 12, Text = "其它类药物：" };
        // DFM: seHeroNormalHP0 Left=79 Top=12 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroNormalHP0 = new TSpinEditEx { Left = 79, Top = 12, Width = 76, Height = 21 };
        seHeroNormalHP0.SetDfmRange(0, 0);
        // DFM: seHeroNormalMP0 Left=79 Top=36 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroNormalMP0 = new TSpinEditEx { Left = 79, Top = 36, Width = 76, Height = 21 };
        seHeroNormalMP0.SetDfmRange(0, 0);
        // DFM: seHeroNormalHPMP0 Left=79 Top=60 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroNormalHPMP0 = new TSpinEditEx { Left = 79, Top = 60, Width = 76, Height = 21 };
        seHeroNormalHPMP0.SetDfmRange(0, 0);
        // DFM: seHeroSpecialHP0 Left=79 Top=84 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroSpecialHP0 = new TSpinEditEx { Left = 79, Top = 84, Width = 76, Height = 21 };
        seHeroSpecialHP0.SetDfmRange(0, 0);
        // DFM: seHeroSpecialMP0 Left=79 Top=108 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroSpecialMP0 = new TSpinEditEx { Left = 79, Top = 108, Width = 76, Height = 21 };
        seHeroSpecialMP0.SetDfmRange(0, 0);
        // DFM: seHeroSpecialHPMP0 Left=79 Top=132 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroSpecialHPMP0 = new TSpinEditEx { Left = 79, Top = 132, Width = 76, Height = 21 };
        seHeroSpecialHPMP0.SetDfmRange(0, 0);
        // DFM: seHeroOther0 Left=79 Top=156 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroOther0 = new TSpinEditEx { Left = 79, Top = 156, Width = 76, Height = 21 };
        seHeroOther0.SetDfmRange(0, 0);
        GroupBox4.Controls.AddRange(new Control[] { Label3, Label8, Label15, Label17, Label18, Label19, Label16,
            seHeroNormalHP0, seHeroNormalMP0, seHeroNormalHPMP0, seHeroSpecialHP0, seHeroSpecialMP0, seHeroSpecialHPMP0, seHeroOther0 });
        GroupBox5 = new GroupBox { Left = 176, Top = 15, Width = 162, Height = 184, Text = "法师" };
        // DFM: Label20 Left=8 Top=17 Width=72 Height=12 Caption='普通HP药物：'
        Label20 = new Label { Left = 8, Top = 17, Width = 72, Height = 12, Text = "普通HP药物：" };
        // DFM: Label21 Left=8 Top=41 Width=72 Height=12 Caption='普通MP药物：'
        Label21 = new Label { Left = 8, Top = 41, Width = 72, Height = 12, Text = "普通MP药物：" };
        // DFM: Label22 Left=8 Top=65 Width=72 Height=12 Caption='普通HPMP药物：'
        Label22 = new Label { Left = 8, Top = 65, Width = 72, Height = 12, Text = "普通HPMP药物：" };
        // DFM: Label24 Left=8 Top=89 Width=72 Height=12 Caption='特殊HP药物：'
        Label24 = new Label { Left = 8, Top = 89, Width = 72, Height = 12, Text = "特殊HP药物：" };
        // DFM: Label25 Left=8 Top=112 Width=72 Height=12 Caption='特殊MP药物：'
        Label25 = new Label { Left = 8, Top = 112, Width = 72, Height = 12, Text = "特殊MP药物：" };
        // DFM: Label26 Left=8 Top=136 Width=72 Height=12 Caption='特殊HPMP药物：'
        Label26 = new Label { Left = 8, Top = 136, Width = 72, Height = 12, Text = "特殊HPMP药物：" };
        // DFM: Label23 Left=8 Top=160 Width=72 Height=12 Caption='其它类药物：'
        Label23 = new Label { Left = 8, Top = 160, Width = 72, Height = 12, Text = "其它类药物：" };
        // DFM: seHeroNormalHP1 Left=79 Top=12 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroNormalHP1 = new TSpinEditEx { Left = 79, Top = 12, Width = 76, Height = 21 };
        seHeroNormalHP1.SetDfmRange(0, 0);
        // DFM: seHeroNormalMP1 Left=79 Top=36 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroNormalMP1 = new TSpinEditEx { Left = 79, Top = 36, Width = 76, Height = 21 };
        seHeroNormalMP1.SetDfmRange(0, 0);
        // DFM: seHeroNormalHPMP1 Left=79 Top=60 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroNormalHPMP1 = new TSpinEditEx { Left = 79, Top = 60, Width = 76, Height = 21 };
        seHeroNormalHPMP1.SetDfmRange(0, 0);
        // DFM: seHeroSpecialHP1 Left=79 Top=84 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroSpecialHP1 = new TSpinEditEx { Left = 79, Top = 84, Width = 76, Height = 21 };
        seHeroSpecialHP1.SetDfmRange(0, 0);
        // DFM: seHeroSpecialMP1 Left=79 Top=108 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroSpecialMP1 = new TSpinEditEx { Left = 79, Top = 108, Width = 76, Height = 21 };
        seHeroSpecialMP1.SetDfmRange(0, 0);
        // DFM: seHeroSpecialHPMP1 Left=79 Top=132 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroSpecialHPMP1 = new TSpinEditEx { Left = 79, Top = 132, Width = 76, Height = 21 };
        seHeroSpecialHPMP1.SetDfmRange(0, 0);
        // DFM: seHeroOther1 Left=79 Top=156 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroOther1 = new TSpinEditEx { Left = 79, Top = 156, Width = 76, Height = 21 };
        seHeroOther1.SetDfmRange(0, 0);
        GroupBox5.Controls.AddRange(new Control[] { Label20, Label21, Label22, Label24, Label25, Label26, Label23,
            seHeroNormalHP1, seHeroNormalMP1, seHeroNormalHPMP1, seHeroSpecialHP1, seHeroSpecialMP1, seHeroSpecialHPMP1, seHeroOther1 });
        GroupBox6 = new GroupBox { Left = 344, Top = 15, Width = 162, Height = 184, Text = "道士" };
        // DFM: Label27 Left=8 Top=17 Width=72 Height=12 Caption='普通HP药物：'
        Label27 = new Label { Left = 8, Top = 17, Width = 72, Height = 12, Text = "普通HP药物：" };
        // DFM: Label28 Left=8 Top=41 Width=72 Height=12 Caption='普通MP药物：'
        Label28 = new Label { Left = 8, Top = 41, Width = 72, Height = 12, Text = "普通MP药物：" };
        // DFM: Label29 Left=8 Top=65 Width=72 Height=12 Caption='普通HPMP药物：'
        Label29 = new Label { Left = 8, Top = 65, Width = 72, Height = 12, Text = "普通HPMP药物：" };
        // DFM: Label39 Left=8 Top=89 Width=72 Height=12 Caption='特殊HP药物：'
        Label39 = new Label { Left = 8, Top = 89, Width = 72, Height = 12, Text = "特殊HP药物：" };
        // DFM: Label40 Left=8 Top=112 Width=72 Height=12 Caption='特殊MP药物：'
        Label40 = new Label { Left = 8, Top = 112, Width = 72, Height = 12, Text = "特殊MP药物：" };
        // DFM: Label41 Left=8 Top=136 Width=72 Height=12 Caption='特殊HPMP药物：'
        Label41 = new Label { Left = 8, Top = 136, Width = 72, Height = 12, Text = "特殊HPMP药物：" };
        // DFM: Label34 Left=8 Top=160 Width=72 Height=12 Caption='其它类药物：'
        Label34 = new Label { Left = 8, Top = 160, Width = 72, Height = 12, Text = "其它类药物：" };
        // DFM: seHeroNormalHP2 Left=79 Top=12 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroNormalHP2 = new TSpinEditEx { Left = 79, Top = 12, Width = 76, Height = 21 };
        seHeroNormalHP2.SetDfmRange(0, 0);
        // DFM: seHeroNormalMP2 Left=79 Top=36 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroNormalMP2 = new TSpinEditEx { Left = 79, Top = 36, Width = 76, Height = 21 };
        seHeroNormalMP2.SetDfmRange(0, 0);
        // DFM: seHeroNormalHPMP2 Left=79 Top=60 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroNormalHPMP2 = new TSpinEditEx { Left = 79, Top = 60, Width = 76, Height = 21 };
        seHeroNormalHPMP2.SetDfmRange(0, 0);
        // DFM: seHeroSpecialHP2 Left=79 Top=84 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroSpecialHP2 = new TSpinEditEx { Left = 79, Top = 84, Width = 76, Height = 21 };
        seHeroSpecialHP2.SetDfmRange(0, 0);
        // DFM: seHeroSpecialMP2 Left=79 Top=108 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroSpecialMP2 = new TSpinEditEx { Left = 79, Top = 108, Width = 76, Height = 21 };
        seHeroSpecialMP2.SetDfmRange(0, 0);
        // DFM: seHeroSpecialHPMP2 Left=79 Top=132 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroSpecialHPMP2 = new TSpinEditEx { Left = 79, Top = 132, Width = 76, Height = 21 };
        seHeroSpecialHPMP2.SetDfmRange(0, 0);
        // DFM: seHeroOther2 Left=79 Top=156 Width=76 Height=21 MaxValue=0 MinValue=0 Value=0
        seHeroOther2 = new TSpinEditEx { Left = 79, Top = 156, Width = 76, Height = 21 };
        seHeroOther2.SetDfmRange(0, 0);
        GroupBox6.Controls.AddRange(new Control[] { Label27, Label28, Label29, Label39, Label40, Label41, Label34,
            seHeroNormalHP2, seHeroNormalMP2, seHeroNormalHPMP2, seHeroSpecialHP2, seHeroSpecialMP2, seHeroSpecialHPMP2, seHeroOther2 });

        grpHum.Controls.Add(grp2);
        grpHum.Controls.Add(GroupBox1);
        grpHum.Controls.Add(GroupBox2);
        grpHero.Controls.Add(GroupBox4);
        grpHero.Controls.Add(GroupBox5);
        grpHero.Controls.Add(GroupBox6);
        Controls.Add(grpHum);
        Controls.Add(grpHero);
        Controls.Add(btnOK);

        // DFM: OnCreate = FormCreate；btnOK OnClick = btnOKClick
        Load += (s, e) => FormCreate(s, e);
        btnOK.Click += (s, e) => btnOK_Click(s, e);
    }

    /// <summary>原 uFrmItemEatCD.pas:213-262 `TFrmItemEatCD.FormCreate`（84 次人工赋值，全局 → 控件）。</summary>
    public void FormCreate(object sender, EventArgs e)
    {
        var cfg = FormGlobals.g_EatItemCDConfig;

        // 原 :215-221 Hum[0]
        seHumNormalHP0.Value = cfg.Hum[0].NormalHP;
        seHumNormalMP0.Value = cfg.Hum[0].NormalMP;
        seHumNormalHPMP0.Value = cfg.Hum[0].NormalHPMP;
        seHumSpecialHP0.Value = cfg.Hum[0].SpecialHP;
        seHumSpecialMP0.Value = cfg.Hum[0].SpecialMP;
        seHumSpecialHPMP0.Value = cfg.Hum[0].SpecialHPMP;
        seHumOther0.Value = cfg.Hum[0].Other;

        // 原 :223-229 Hum[1]
        seHumNormalHP1.Value = cfg.Hum[1].NormalHP;
        seHumNormalMP1.Value = cfg.Hum[1].NormalMP;
        seHumNormalHPMP1.Value = cfg.Hum[1].NormalHPMP;
        seHumSpecialHP1.Value = cfg.Hum[1].SpecialHP;
        seHumSpecialMP1.Value = cfg.Hum[1].SpecialMP;
        seHumSpecialHPMP1.Value = cfg.Hum[1].SpecialHPMP;
        seHumOther1.Value = cfg.Hum[1].Other;

        // 原 :231-237 Hum[2]
        seHumNormalHP2.Value = cfg.Hum[2].NormalHP;
        seHumNormalMP2.Value = cfg.Hum[2].NormalMP;
        seHumNormalHPMP2.Value = cfg.Hum[2].NormalHPMP;
        seHumSpecialHP2.Value = cfg.Hum[2].SpecialHP;
        seHumSpecialMP2.Value = cfg.Hum[2].SpecialMP;
        seHumSpecialHPMP2.Value = cfg.Hum[2].SpecialHPMP;
        seHumOther2.Value = cfg.Hum[2].Other;

        // 原 :239-245 Hero[0]
        seHeroNormalHP0.Value = cfg.Hero[0].NormalHP;
        seHeroNormalMP0.Value = cfg.Hero[0].NormalMP;
        seHeroNormalHPMP0.Value = cfg.Hero[0].NormalHPMP;
        seHeroSpecialHP0.Value = cfg.Hero[0].SpecialHP;
        seHeroSpecialMP0.Value = cfg.Hero[0].SpecialMP;
        seHeroSpecialHPMP0.Value = cfg.Hero[0].SpecialHPMP;
        seHeroOther0.Value = cfg.Hero[0].Other;

        // 原 :247-253 Hero[1]
        seHeroNormalHP1.Value = cfg.Hero[1].NormalHP;
        seHeroNormalMP1.Value = cfg.Hero[1].NormalMP;
        seHeroNormalHPMP1.Value = cfg.Hero[1].NormalHPMP;
        seHeroSpecialHP1.Value = cfg.Hero[1].SpecialHP;
        seHeroSpecialMP1.Value = cfg.Hero[1].SpecialMP;
        seHeroSpecialHPMP1.Value = cfg.Hero[1].SpecialHPMP;
        seHeroOther1.Value = cfg.Hero[1].Other;

        // 原 :255-261 Hero[2]
        seHeroNormalHP2.Value = cfg.Hero[2].NormalHP;
        seHeroNormalMP2.Value = cfg.Hero[2].NormalMP;
        seHeroNormalHPMP2.Value = cfg.Hero[2].NormalHPMP;
        seHeroSpecialHP2.Value = cfg.Hero[2].SpecialHP;
        seHeroSpecialMP2.Value = cfg.Hero[2].SpecialMP;
        seHeroSpecialHPMP2.Value = cfg.Hero[2].SpecialHPMP;
        seHeroOther2.Value = cfg.Hero[2].Other;
    }

    /// <summary>测试辅助：读出 6 个 slot × 7 个字段的当前控件值（顺序 = FieldSuffixes）。</summary>
    public int[][] ReadControlValues()
    {
        return new[]
        {
            new[] { seHumNormalHP0.Value, seHumNormalMP0.Value, seHumNormalHPMP0.Value, seHumSpecialHP0.Value, seHumSpecialMP0.Value, seHumSpecialHPMP0.Value, seHumOther0.Value },
            new[] { seHumNormalHP1.Value, seHumNormalMP1.Value, seHumNormalHPMP1.Value, seHumSpecialHP1.Value, seHumSpecialMP1.Value, seHumSpecialHPMP1.Value, seHumOther1.Value },
            new[] { seHumNormalHP2.Value, seHumNormalMP2.Value, seHumNormalHPMP2.Value, seHumSpecialHP2.Value, seHumSpecialMP2.Value, seHumSpecialHPMP2.Value, seHumOther2.Value },
            new[] { seHeroNormalHP0.Value, seHeroNormalMP0.Value, seHeroNormalHPMP0.Value, seHeroSpecialHP0.Value, seHeroSpecialMP0.Value, seHeroSpecialHPMP0.Value, seHeroOther0.Value },
            new[] { seHeroNormalHP1.Value, seHeroNormalMP1.Value, seHeroNormalHPMP1.Value, seHeroSpecialHP1.Value, seHeroSpecialMP1.Value, seHeroSpecialHPMP1.Value, seHeroOther1.Value },
            new[] { seHeroNormalHP2.Value, seHeroNormalMP2.Value, seHeroNormalHPMP2.Value, seHeroSpecialHP2.Value, seHeroSpecialMP2.Value, seHeroSpecialHPMP2.Value, seHeroOther2.Value }
        };
    }

    /// <summary>原 uFrmItemEatCD.pas:130-211 `TFrmItemEatCD.btnOKClick`（84 次人工赋值，控件 → 全局 + INI）。</summary>
    public void btnOK_Click(object sender, EventArgs e)
    {
        var cfg = FormGlobals.g_EatItemCDConfig;

        // 原 :136-142 Hum[0]
        cfg.Hum[0].NormalHP = seHumNormalHP0.Value;
        cfg.Hum[0].NormalMP = seHumNormalMP0.Value;
        cfg.Hum[0].NormalHPMP = seHumNormalHPMP0.Value;
        cfg.Hum[0].SpecialHP = seHumSpecialHP0.Value;
        cfg.Hum[0].SpecialMP = seHumSpecialMP0.Value;
        cfg.Hum[0].SpecialHPMP = seHumSpecialHPMP0.Value;
        cfg.Hum[0].Other = seHumOther0.Value;

        // 原 :144-150 Hum[1]
        cfg.Hum[1].NormalHP = seHumNormalHP1.Value;
        cfg.Hum[1].NormalMP = seHumNormalMP1.Value;
        cfg.Hum[1].NormalHPMP = seHumNormalHPMP1.Value;
        cfg.Hum[1].SpecialHP = seHumSpecialHP1.Value;
        cfg.Hum[1].SpecialMP = seHumSpecialMP1.Value;
        cfg.Hum[1].SpecialHPMP = seHumSpecialHPMP1.Value;
        cfg.Hum[1].Other = seHumOther1.Value;

        // 原 :152-158 Hum[2]
        cfg.Hum[2].NormalHP = seHumNormalHP2.Value;
        cfg.Hum[2].NormalMP = seHumNormalMP2.Value;
        cfg.Hum[2].NormalHPMP = seHumNormalHPMP2.Value;
        cfg.Hum[2].SpecialHP = seHumSpecialHP2.Value;
        cfg.Hum[2].SpecialMP = seHumSpecialMP2.Value;
        cfg.Hum[2].SpecialHPMP = seHumSpecialHPMP2.Value;
        cfg.Hum[2].Other = seHumOther2.Value;

        // 原 :160-166 Hero[0]
        cfg.Hero[0].NormalHP = seHeroNormalHP0.Value;
        cfg.Hero[0].NormalMP = seHeroNormalMP0.Value;
        cfg.Hero[0].NormalHPMP = seHeroNormalHPMP0.Value;
        cfg.Hero[0].SpecialHP = seHeroSpecialHP0.Value;
        cfg.Hero[0].SpecialMP = seHeroSpecialMP0.Value;
        cfg.Hero[0].SpecialHPMP = seHeroSpecialHPMP0.Value;
        cfg.Hero[0].Other = seHeroOther0.Value;

        // 原 :168-174 Hero[1]
        cfg.Hero[1].NormalHP = seHeroNormalHP1.Value;
        cfg.Hero[1].NormalMP = seHeroNormalMP1.Value;
        cfg.Hero[1].NormalHPMP = seHeroNormalHPMP1.Value;
        cfg.Hero[1].SpecialHP = seHeroSpecialHP1.Value;
        cfg.Hero[1].SpecialMP = seHeroSpecialMP1.Value;
        cfg.Hero[1].SpecialHPMP = seHeroSpecialHPMP1.Value;
        cfg.Hero[1].Other = seHeroOther1.Value;

        // 原 :176-182 Hero[2]
        cfg.Hero[2].NormalHP = seHeroNormalHP2.Value;
        cfg.Hero[2].NormalMP = seHeroNormalMP2.Value;
        cfg.Hero[2].NormalHPMP = seHeroNormalHPMP2.Value;
        cfg.Hero[2].SpecialHP = seHeroSpecialHP2.Value;
        cfg.Hero[2].SpecialMP = seHeroSpecialMP2.Value;
        cfg.Hero[2].SpecialHPMP = seHeroSpecialHPMP2.Value;
        cfg.Hero[2].Other = seHeroOther2.Value;

        ItemEatCDLogic.WriteIni(FormGlobals.g_sIniFileName, cfg);   // 原 :184-208
        DialogResult = DialogResult.OK;                             // 原 :210 ModalResult := mrOK
    }
}

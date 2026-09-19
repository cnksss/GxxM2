using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GXX.Core.Util;

namespace GXX.DBServer;

// Setting.pas (1-138) → Setting.cs
// 基本设置窗体（DBShare 的 g_bo*/g_n*/过滤串列表）。INI 键序与原文 Writing 顺序逐字一致（含大小写）。

/// <summary>Setting.pas 窗体控件值 ⇄ 全局量 ⇄ INI 的中间载体。</summary>
public class SettingValues
{
    public bool CanCreateHuman;
    public bool CanDeleteHuman;
    public bool CanGetBackDeleteHuman;
    public int CanDeleteHumanLowLevel;
    public bool ForbidNumberName;
    public bool ForbidLetterName;
    public bool DenyChrName;
    public bool Ranking;
    public int CreateChrNameCount;
    public bool UseActiveRunGage;
    public bool ShowBlockIPLog;
    public string FilterNewHumanNameText = "";
    public string FilterRankingNameText = "";
}

/// <summary>Setting.pas 的非 UI 逻辑（可单测）。</summary>
public static class SettingLogic
{
    /// <summary>Setting.pas:62-83 `TFrmSetting.Open`：全局量 → 控件值。</summary>
    public static SettingValues Open()
    {
        var v = new SettingValues();
        v.DenyChrName = TBool.ToBool(DBShareSeam.g_boDenyChrName);
        v.CanCreateHuman = TBool.ToBool(DBShareSeam.g_boCanCreateHuman);
        v.CanDeleteHuman = TBool.ToBool(DBShareSeam.g_boCanDeleteHuman);
        v.CanGetBackDeleteHuman = TBool.ToBool(DBShareSeam.g_boCanGetBackDeleteHuman);
        v.ForbidNumberName = TBool.ToBool(DBShareSeam.g_boForbidNumberName);
        v.ForbidLetterName = TBool.ToBool(DBShareSeam.g_boForbidLetterName);

        v.Ranking = TBool.ToBool(DBShareSeam.g_boCanRanking);

        // MemoFilterNewHumanName.Clear; ...Lines.AddStrings(g_FilterNewHumanNameTextList);
        v.FilterNewHumanNameText = JoinLines(DBShareSeam.g_FilterNewHumanNameTextList);
        v.FilterRankingNameText = JoinLines(DBShareSeam.g_FilterRankingNameTextList);

        v.CanDeleteHumanLowLevel = DBShareSeam.g_nCanDeleteHumanLowLevel;
        v.CreateChrNameCount = DBShareSeam.g_nCreateChrNameCount;

        v.UseActiveRunGage = TBool.ToBool(DBShareSeam.g_boUseActiveRunGage);
        v.ShowBlockIPLog = TBool.ToBool(DBShareSeam.g_boShowBlockIPLog);
        return v;
    }

    /// <summary>Delphi `TMemo.Lines.AddStrings(list)` → Text 的等价文本（CRLF 连接）。</summary>
    public static string JoinLines(TStringList list)
    {
        var parts = new List<string>();
        for (int i = 0; i < list.Count; i++) parts.Add(list[i]);
        return string.Join("\r\n", parts);
    }

    /// <summary>
    /// Setting.pas:85-136 `ButtonOKClick`：
    ///   控件 → 全局量（顺序原文如此）→ TIniFile 写 11 个键 → 两个过滤串文件（异常吞掉）→ ModalResult := mrOK。
    /// </summary>
    public static void ButtonOK(SettingValues v)
    {
        DBShareSeam.g_boCanCreateHuman = TBool.ToByte(v.CanCreateHuman);
        DBShareSeam.g_boCanDeleteHuman = TBool.ToByte(v.CanDeleteHuman);
        DBShareSeam.g_nCanDeleteHumanLowLevel = v.CanDeleteHumanLowLevel;
        DBShareSeam.g_boCanGetBackDeleteHuman = TBool.ToByte(v.CanGetBackDeleteHuman);
        DBShareSeam.g_boDenyChrName = TBool.ToByte(v.DenyChrName);
        DBShareSeam.g_boForbidNumberName = TBool.ToByte(v.ForbidNumberName);
        DBShareSeam.g_boForbidLetterName = TBool.ToByte(v.ForbidLetterName);
        DBShareSeam.g_nCreateChrNameCount = v.CreateChrNameCount;
        TStringsHelper.SetText(DBShareSeam.g_FilterNewHumanNameTextList, v.FilterNewHumanNameText);

        DBShareSeam.g_boUseActiveRunGage = TBool.ToByte(v.UseActiveRunGage);
        DBShareSeam.g_boCanRanking = TBool.ToByte(v.Ranking);
        TStringsHelper.SetText(DBShareSeam.g_FilterRankingNameTextList, v.FilterRankingNameText);

        DBShareSeam.g_boShowBlockIPLog = TBool.ToByte(v.ShowBlockIPLog);

        // Setting.pas:105-122：Conf := TIniFile.Create(g_sConfFileName); if Conf <> nil then ...（TIniFile.Create 从不返回 nil，原文为冗余判断）
        var Conf = new TIniFile(DBShareSeam.g_sConfFileName);
        if (Conf != null)
        {
            Conf.WriteBool("Setup", "CanCreateHuman", DBShareSeam.g_boCanCreateHuman != 0);                                  //允许建立新人物
            Conf.WriteBool("Setup", "CanDeleteHuman", DBShareSeam.g_boCanDeleteHuman != 0);                                  //允许删除人物
            Conf.WriteBool("Setup", "CanGetBackDeleteHuman", DBShareSeam.g_boCanGetBackDeleteHuman != 0);                    //允许找回删除的人物
            Conf.WriteInteger("Setup", "CanDeleteHumanLowLevel", DBShareSeam.g_nCanDeleteHumanLowLevel);                     //以上级别不允许被删除
            Conf.WriteBool("Setup", "ForbidNumberName", DBShareSeam.g_boForbidNumberName != 0);                              //禁止建立包含数字的人物名
            Conf.WriteBool("Setup", "ForbidLetterName", DBShareSeam.g_boForbidLetterName != 0);                              //禁止建立全英文人物名

            Conf.WriteBool("Setup", "DenyChrName", DBShareSeam.g_boDenyChrName != 0);
            Conf.WriteBool("Setup", "CanRanking", DBShareSeam.g_boCanRanking != 0);
            Conf.WriteInteger("Setup", "CreateChrNameCount", DBShareSeam.g_nCreateChrNameCount);
            Conf.WriteBool("Setup", "UseActiveRunGage", DBShareSeam.g_boUseActiveRunGage != 0);
            Conf.WriteBool("Setup", "ShowBlockIPLog", DBShareSeam.g_boShowBlockIPLog != 0);

            Conf.Dispose();
        }

        // Setting.pas:124-133：两个 SaveToFile 各自 try/except 空处理（静默吞异常）
        try
        {
            DBShareSeam.g_FilterNewHumanNameTextList.SaveToFile(DBShareSeam.g_sFilePath + "FilterNewHumanNameString.txt");
        }
        catch
        {
        }
        try
        {
            DBShareSeam.g_FilterRankingNameTextList.SaveToFile(DBShareSeam.g_sFilePath + "FilterRankingNameString.txt");
        }
        catch
        {
        }

        // ModalResult := mrOK;  （窗体侧处理）
    }
}

/// <summary>Setting.pas:41 `function ShowFrmSetting: Boolean;`。</summary>
public static class SettingUnit
{
    /// <summary>Setting.pas:49-60 `ShowFrmSetting`：Create → Open → ShowModal = mrOK。</summary>
    public static bool ShowFrmSetting()
    {
        var FrmSetting = new FrmSetting();
        try
        {
            FrmSetting.Open();
            return FrmSetting.ShowDialog() == DialogResult.OK;
        }
        finally
        {
            FrmSetting.Dispose();
        }
    }
}

/// <summary>Setting.pas:10-39 `TFrmSetting`（DFM: Setting.dfm）。</summary>
public class FrmSetting : Form
{
    // DFM: FrmSetting Left=451 Top=182 BorderStyle=bsDialog Caption='基本设置' ClientHeight=490 ClientWidth=737
    //      Font.Charset=GB2312_CHARSET Position=poMainFormCenter PixelsPerInch=96
    public Button ButtonOK;
    public TabControl PageControl1;
    public TabPage TabSheet1;
    public GroupBox GroupBox1;
    public Label Label1;
    public CheckBox CheckBoxDenyChrName;
    public CheckBox CheckBoxCanDeleteHuman;
    public TSpinEdit EditCreateChrNameCount;
    public CheckBox CheckBoxCanCreateHuman;
    public CheckBox CheckBoxCanGetBackDeleteHuman;
    public TSpinEdit EditCanDeleteHumanLowLevel;
    public Label Label2;
    public CheckBox CheckBoxForbidNumberName;
    public CheckBox CheckBoxForbidLetterName;
    public Label Label3;
    public GroupBox GroupBox2;
    public Label Label4;
    public CheckBox CheckBoxRanking;
    public TextBox MemoFilterNewHumanName;     // TMemo(ScrollBars=ssBoth)
    public TextBox MemoFilterRankingName;      // TMemo(ScrollBars=ssBoth)
    public CheckBox chkUseActiveRunGage;
    public Button Button1;
    public CheckBox chkShowBlockIPLog;

    public FrmSetting()
    {
        // DFM: FrmSetting Left=451 Top=182 BorderStyle=bsDialog Caption='基本设置' ClientHeight=490 ClientWidth=737
        Text = "基本设置";
        StartPosition = FormStartPosition.CenterParent;    // Position=poMainFormCenter
        Location = new System.Drawing.Point(451, 182);
        ClientSize = new System.Drawing.Size(737, 490);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: ButtonOK Left=558 Top=456 Width=75 Height=25 Caption='确定(&O)' TabOrder=0 OnClick=ButtonOKClick
        ButtonOK = new Button { Left = 558, Top = 456, Width = 75, Height = 25, Text = "确定(&O)", TabIndex = 0 };
        // DFM: Button1 Left=654 Top=456 Width=75 Height=25 Caption='取消(&C)' ModalResult=2 TabOrder=2
        //      原文如此：OnClick = ButtonOKClick（取消按钮也走保存逻辑）
        Button1 = new Button { Left = 654, Top = 456, Width = 75, Height = 25, Text = "取消(&C)", TabIndex = 2, DialogResult = DialogResult.Cancel };

        // DFM: PageControl1 Left=8 Top=8 Width=721 Height=441 ActivePage=TabSheet1 TabOrder=1
        PageControl1 = new TabControl { Left = 8, Top = 8, Width = 721, Height = 441, TabIndex = 1 };
        // DFM: TabSheet1 Caption='基本设置'
        TabSheet1 = new TabPage { Text = "基本设置" };

        // DFM: GroupBox1 Left=8 Top=5 Width=345 Height=404 Caption='基本设置' TabOrder=0
        GroupBox1 = new GroupBox { Left = 8, Top = 5, Width = 345, Height = 404, Text = "基本设置", TabIndex = 0 };
        // DFM: Label1 Left=12 Top=136 Width=84 Height=12 Caption='允许创建角色数'
        Label1 = new Label { Left = 12, Top = 136, Width = 84, Height = 12, Text = "允许创建角色数" };
        // DFM: Label2 Left=192 Top=38 Width=120 Height=12 Caption='以上级别不允许被删除'
        Label2 = new Label { Left = 192, Top = 38, Width = 120, Height = 12, Text = "以上级别不允许被删除" };
        // DFM: Label3 Left=12 Top=160 Width=240 Height=12 Caption='禁止建立包含以下字符的人物名称(一行一个)'
        Label3 = new Label { Left = 12, Top = 160, Width = 240, Height = 12, Text = "禁止建立包含以下字符的人物名称(一行一个)" };
        // DFM: CheckBoxDenyChrName Left=12 Top=76 Width=145 Height=17 Caption='允许特殊字符创建人物' TabOrder=0
        CheckBoxDenyChrName = new CheckBox { Left = 12, Top = 76, Width = 145, Height = 17, Text = "允许特殊字符创建人物", TabIndex = 0 };
        // DFM: CheckBoxCanDeleteHuman Left=12 Top=36 Width=92 Height=17 Caption='允许删除人物' TabOrder=1
        CheckBoxCanDeleteHuman = new CheckBox { Left = 12, Top = 36, Width = 92, Height = 17, Text = "允许删除人物", TabIndex = 1 };
        // DFM: EditCreateChrNameCount Left=104 Top=134 Width=57 Height=21 MaxValue=20 MinValue=1 TabOrder=2 Value=2
        EditCreateChrNameCount = new TSpinEdit { Left = 104, Top = 134, Width = 57, Height = 21, TabIndex = 2 };
        EditCreateChrNameCount.SetDfmRange(1, 20);
        EditCreateChrNameCount.Value = 2;
        // DFM: CheckBoxCanCreateHuman Left=12 Top=16 Width=108 Height=17 Caption='允许建立新人物' TabOrder=3
        CheckBoxCanCreateHuman = new CheckBox { Left = 12, Top = 16, Width = 108, Height = 17, Text = "允许建立新人物", TabIndex = 3 };
        // DFM: CheckBoxCanGetBackDeleteHuman Left=12 Top=56 Width=116 Height=17 Caption='允许找回删除人物' TabOrder=4
        CheckBoxCanGetBackDeleteHuman = new CheckBox { Left = 12, Top = 56, Width = 116, Height = 17, Text = "允许找回删除人物", TabIndex = 4 };
        // DFM: EditCanDeleteHumanLowLevel Left=136 Top=32 Width=49 Height=21 MaxValue=0 MinValue=0 TabOrder=5 Value=0
        EditCanDeleteHumanLowLevel = new TSpinEdit { Left = 136, Top = 32, Width = 49, Height = 21, TabIndex = 5 };
        EditCanDeleteHumanLowLevel.SetDfmRange(0, 0);
        // DFM: CheckBoxForbidNumberName Left=12 Top=96 Width=173 Height=17 Caption='禁止建立包含数字的人物名' TabOrder=6
        CheckBoxForbidNumberName = new CheckBox { Left = 12, Top = 96, Width = 173, Height = 17, Text = "禁止建立包含数字的人物名", TabIndex = 6 };
        // DFM: CheckBoxForbidLetterName Left=12 Top=116 Width=141 Height=17 Caption='禁止建立全英文人物名' TabOrder=7
        CheckBoxForbidLetterName = new CheckBox { Left = 12, Top = 116, Width = 141, Height = 17, Text = "禁止建立全英文人物名", TabIndex = 7 };
        // DFM: MemoFilterNewHumanName Left=16 Top=176 Width=233 Height=218 ScrollBars=ssBoth TabOrder=8
        MemoFilterNewHumanName = new TextBox { Left = 16, Top = 176, Width = 233, Height = 218, Multiline = true, ScrollBars = ScrollBars.Both, TabIndex = 8 };

        // DFM: GroupBox2 Left=360 Top=5 Width=345 Height=403 Caption='其他设置' TabOrder=1
        GroupBox2 = new GroupBox { Left = 360, Top = 5, Width = 345, Height = 403, Text = "其他设置", TabIndex = 1 };
        // DFM: Label4 Left=12 Top=157 Width=276 Height=12 Caption='禁止包含以下字符的人物名称进入排行榜(一行一个)'
        Label4 = new Label { Left = 12, Top = 157, Width = 276, Height = 12, Text = "禁止包含以下字符的人物名称进入排行榜(一行一个)" };
        // DFM: CheckBoxRanking Left=12 Top=136 Width=81 Height=17 Caption='开启排行榜' TabOrder=0
        CheckBoxRanking = new CheckBox { Left = 12, Top = 136, Width = 81, Height = 17, Text = "开启排行榜", TabIndex = 0 };
        // DFM: MemoFilterRankingName Left=8 Top=176 Width=233 Height=218 ScrollBars=ssBoth TabOrder=1
        MemoFilterRankingName = new TextBox { Left = 8, Top = 176, Width = 233, Height = 218, Multiline = true, ScrollBars = ScrollBars.Both, TabIndex = 1 };
        // DFM: chkUseActiveRunGage Left=8 Top=16 Width=209 Height=17 Caption='只分配可连接的游戏网关给客户端' TabOrder=2
        chkUseActiveRunGage = new CheckBox { Left = 8, Top = 16, Width = 209, Height = 17, Text = "只分配可连接的游戏网关给客户端", TabIndex = 2 };
        // DFM: chkShowBlockIPLog Left=8 Top=34 Width=169 Height=17 Caption='显示非法请求内部端口日志' TabOrder=3
        chkShowBlockIPLog = new CheckBox { Left = 8, Top = 34, Width = 169, Height = 17, Text = "显示非法请求内部端口日志", TabIndex = 3 };

        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(Label3);
        GroupBox1.Controls.Add(CheckBoxDenyChrName);
        GroupBox1.Controls.Add(CheckBoxCanDeleteHuman);
        GroupBox1.Controls.Add(EditCreateChrNameCount);
        GroupBox1.Controls.Add(CheckBoxCanCreateHuman);
        GroupBox1.Controls.Add(CheckBoxCanGetBackDeleteHuman);
        GroupBox1.Controls.Add(EditCanDeleteHumanLowLevel);
        GroupBox1.Controls.Add(CheckBoxForbidNumberName);
        GroupBox1.Controls.Add(CheckBoxForbidLetterName);
        GroupBox1.Controls.Add(MemoFilterNewHumanName);

        GroupBox2.Controls.Add(Label4);
        GroupBox2.Controls.Add(CheckBoxRanking);
        GroupBox2.Controls.Add(MemoFilterRankingName);
        GroupBox2.Controls.Add(chkUseActiveRunGage);
        GroupBox2.Controls.Add(chkShowBlockIPLog);

        TabSheet1.Controls.Add(GroupBox1);
        TabSheet1.Controls.Add(GroupBox2);
        PageControl1.TabPages.Add(TabSheet1);

        Controls.Add(ButtonOK);
        Controls.Add(PageControl1);
        Controls.Add(Button1);

        ButtonOK.Click += (s, e) => ButtonOKClick(s, e);
        Button1.Click += (s, e) => ButtonOKClick(s, e);   // 原文如此：取消按钮的 OnClick 也是 ButtonOKClick
    }

    /// <summary>Setting.pas:62-83 `TFrmSetting.Open`：全局量 → 控件。</summary>
    public void Open()
    {
        SettingValues v = SettingLogic.Open();
        CheckBoxDenyChrName.Checked = v.DenyChrName;
        CheckBoxCanCreateHuman.Checked = v.CanCreateHuman;
        CheckBoxCanDeleteHuman.Checked = v.CanDeleteHuman;
        CheckBoxCanGetBackDeleteHuman.Checked = v.CanGetBackDeleteHuman;
        CheckBoxForbidNumberName.Checked = v.ForbidNumberName;
        CheckBoxForbidLetterName.Checked = v.ForbidLetterName;

        CheckBoxRanking.Checked = v.Ranking;

        MemoFilterNewHumanName.Clear();
        MemoFilterRankingName.Clear();
        MemoFilterNewHumanName.Text = v.FilterNewHumanNameText;
        MemoFilterRankingName.Text = v.FilterRankingNameText;

        EditCanDeleteHumanLowLevel.Value = v.CanDeleteHumanLowLevel;
        EditCreateChrNameCount.Value = v.CreateChrNameCount;

        chkUseActiveRunGage.Checked = v.UseActiveRunGage;
        chkShowBlockIPLog.Checked = v.ShowBlockIPLog;
    }

    /// <summary>Setting.pas:85-136 `ButtonOKClick`：控件 → 全局/INI/文件 → ModalResult := mrOK。</summary>
    public void ButtonOKClick(object Sender, EventArgs e)
    {
        var v = new SettingValues
        {
            CanCreateHuman = CheckBoxCanCreateHuman.Checked,
            CanDeleteHuman = CheckBoxCanDeleteHuman.Checked,
            CanDeleteHumanLowLevel = EditCanDeleteHumanLowLevel.Value,
            CanGetBackDeleteHuman = CheckBoxCanGetBackDeleteHuman.Checked,
            DenyChrName = CheckBoxDenyChrName.Checked,
            ForbidNumberName = CheckBoxForbidNumberName.Checked,
            ForbidLetterName = CheckBoxForbidLetterName.Checked,
            CreateChrNameCount = EditCreateChrNameCount.Value,
            FilterNewHumanNameText = MemoFilterNewHumanName.Text,

            UseActiveRunGage = chkUseActiveRunGage.Checked,
            Ranking = CheckBoxRanking.Checked,
            FilterRankingNameText = MemoFilterRankingName.Text,

            ShowBlockIPLog = chkShowBlockIPLog.Checked
        };
        SettingLogic.ButtonOK(v);
        DialogResult = DialogResult.OK;   // ModalResult := mrOK
    }
}

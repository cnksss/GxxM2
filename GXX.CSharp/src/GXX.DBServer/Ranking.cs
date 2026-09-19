using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GXX.DBServer;

// Ranking.pas (1-332) → Ranking.cs
// 排行榜管理窗体。排行数据本体（判定键 = 等级/出师徒弟数，并列 = 按 DB 返回序）由角色 DB 侧的
// DoGetRankData（SqliteRoleDB.pas:3028 / 5397，SQL 侧 ORDER BY 等级 DESC + 过滤器跳过）产出，
// 本单元只负责触发刷新（RankingEngine.RefRanking）与九张榜的显示、以及 9 个 INI 键的保存。

/// <summary>Ranking.pas:46-54 的九个 ListView（单测注入内存实现）。</summary>
public class RankingDisplay
{
    public IListViewSink ListViewHum;
    public IListViewSink ListViewWarrior;
    public IListViewSink ListViewWizzard;
    public IListViewSink ListViewMonk;
    public IListViewSink ListViewHero;
    public IListViewSink ListViewHeroWarrior;
    public IListViewSink ListViewHeroWizzard;
    public IListViewSink ListViewHeroMonk;
    public IListViewSink ListViewMaster;
}

/// <summary>Ranking.pas 的非 UI 逻辑（可单测）。</summary>
public static class RankingLogic
{
    /// <summary>Delphi `Application.ProcessMessages` 接缝（每 100 行让出一次消息循环）。</summary>
    public static Action ProcessMessages = () => Application.DoEvents();

    /// <summary>
    /// Ranking.pas:107-221 `TFrmRankingDlg.RefRanking`：
    ///   ① 若 g_boRefRanking 或 m_boRefRanking 为真则直接 Exit（防重入）；
    ///   ② 清空九张榜；
    ///   ③ 置 m_boRefRanking := True，逐条按 `I mod 100 = 0` 让出消息循环并追加行；
    ///   ④ finally 复位 m_boRefRanking。
    /// 原文如此：rank 列表本体顺序即显示顺序（RankIndex 不参与显示）。
    /// </summary>
    public static void RefRanking(RankingDisplay d, ref byte m_boRefRanking)
    {
        if (DBShareSeam.g_boRefRanking != 0 || m_boRefRanking != 0) return;

        d.ListViewHum.Clear();
        d.ListViewWarrior.Clear();
        d.ListViewWizzard.Clear();
        d.ListViewMonk.Clear();
        d.ListViewHero.Clear();
        d.ListViewHeroWarrior.Clear();
        d.ListViewHeroWizzard.Clear();
        d.ListViewHeroMonk.Clear();
        d.ListViewMaster.Clear();

        m_boRefRanking = 1;
        try
        {
            TRoleRankList list;
            TRoleRankData RankData;

            list = DBShareSeam.g_HumanRankList;
            for (int I = 0; I <= list.Count - 1; I++)
            {
                if (I % 100 == 0) ProcessMessages();
                RankData = list.Items(I);
                d.ListViewHum.AddRow(I.ToString(), null, RankData.HumanName, RankData.Level.ToString());
            }

            list = DBShareSeam.g_WarriorRankList;
            for (int I = 0; I <= list.Count - 1; I++)
            {
                if (I % 100 == 0) ProcessMessages();
                RankData = list.Items(I);
                d.ListViewWarrior.AddRow(I.ToString(), null, RankData.HumanName, RankData.Level.ToString());
            }

            list = DBShareSeam.g_WizardRankList;
            for (int I = 0; I <= list.Count - 1; I++)
            {
                if (I % 100 == 0) ProcessMessages();
                RankData = list.Items(I);
                d.ListViewWizzard.AddRow(I.ToString(), null, RankData.HumanName, RankData.Level.ToString());
            }

            list = DBShareSeam.g_TaoistRankList;
            for (int I = 0; I <= list.Count - 1; I++)
            {
                if (I % 100 == 0) ProcessMessages();
                RankData = list.Items(I);
                d.ListViewMonk.AddRow(I.ToString(), null, RankData.HumanName, RankData.Level.ToString());
            }

            list = DBShareSeam.g_HeroRankList;
            for (int I = 0; I <= list.Count - 1; I++)
            {
                if (I % 100 == 0) ProcessMessages();
                RankData = list.Items(I);
                d.ListViewHero.AddRow(I.ToString(), null, RankData.HeroName, RankData.HumanName, RankData.Level.ToString());
            }

            list = DBShareSeam.g_HeroWarriorRankList;
            for (int I = 0; I <= list.Count - 1; I++)
            {
                if (I % 100 == 0) ProcessMessages();
                RankData = list.Items(I);
                d.ListViewHeroWarrior.AddRow(I.ToString(), null, RankData.HeroName, RankData.HumanName, RankData.Level.ToString());
            }

            list = DBShareSeam.g_HeroWizardRankList;
            for (int I = 0; I <= list.Count - 1; I++)
            {
                if (I % 100 == 0) ProcessMessages();
                RankData = list.Items(I);
                d.ListViewHeroWizzard.AddRow(I.ToString(), null, RankData.HeroName, RankData.HumanName, RankData.Level.ToString());
            }

            list = DBShareSeam.g_HeroTaoistRankList;
            for (int I = 0; I <= list.Count - 1; I++)
            {
                if (I % 100 == 0) ProcessMessages();
                RankData = list.Items(I);
                d.ListViewHeroMonk.AddRow(I.ToString(), null, RankData.HeroName, RankData.HumanName, RankData.Level.ToString());
            }

            list = DBShareSeam.g_MasterRankList;
            for (int I = 0; I <= list.Count - 1; I++)
            {
                if (I % 100 == 0) ProcessMessages();
                RankData = list.Items(I);
                d.ListViewMaster.AddRow(I.ToString(), null, RankData.HumanName, RankData.MasterCount.ToString());
            }
        }
        finally
        {
            m_boRefRanking = 0;
        }
    }

    /// <summary>
    /// Ranking.pas:234-255 `ButtonSaveClick`：TIniFile 写 9 个键（键序原文如此），
    /// 并注意原文 `Conf.Free` 在 `if Conf &lt;&gt; nil` **之外**（TIniFile.Create 从不返回 nil，故冗余）。
    /// </summary>
    public static void ButtonSave()
    {
        var Conf = new TIniFile(DBShareSeam.g_sConfFileName);
        if (Conf != null)
        {
            Conf.WriteBool("Setup", "AutoRefRanking", DBShareSeam.g_boAutoRefRanking != 0);
            Conf.WriteInteger("Setup", "RankingCount", DBShareSeam.g_nRankingCount);
            Conf.WriteInteger("Setup", "RankingMinLevel", DBShareSeam.g_nRankingMinLevel);
            Conf.WriteInteger("Setup", "RankingMaxLevel", DBShareSeam.g_nRankingMaxLevel);
            Conf.WriteInteger("Setup", "RefRankingHour1", DBShareSeam.g_nRefRankingHour1);
            Conf.WriteInteger("Setup", "RefRankingHour2", DBShareSeam.g_nRefRankingHour2);

            Conf.WriteInteger("Setup", "RefRankingMinute1", DBShareSeam.g_nRefRankingMinute1);
            Conf.WriteInteger("Setup", "RefRankingMinute2", DBShareSeam.g_nRefRankingMinute2);

            Conf.WriteInteger("Setup", "AutoRefRankingType", DBShareSeam.g_nAutoRefRankingType);
        }
        Conf.Dispose();
    }

    /// <summary>Ranking.pas:299-306 `RadioButton1Click`：Checked → 0，否则 → 1（原文如此，忽略 Sender）。</summary>
    public static void RadioButton1Click(bool radioButton1Checked)
    {
        if (radioButton1Checked)
            DBShareSeam.g_nAutoRefRankingType = 0;
        else
            DBShareSeam.g_nAutoRefRankingType = 1;
    }
}

/// <summary>Ranking.pas:81 `var FrmRankingDlg: TFrmRankingDlg;`。</summary>
public static class RankingGlobal
{
    public static FrmRankingDlg FrmRankingDlg;
}

/// <summary>
/// Ranking.pas:11-78 `TFrmRankingDlg`（DFM: Ranking.dfm）。
/// 九个 ListView 的 DFM 属性（脚本 dfm_compact 抽取，均为 Align=alClient / GridLines=True / ReadOnly=True /
/// RowSelect=True / ViewStyle=vsReport）：
///   ListViewHum(417x291)      列: 序号60 / 名称100 / 等级60
///   ListViewWarrior(417x291)  列: 序号60 / 名称100 / 等级60
///   ListViewWizzard(417x291)  列: 序号60 / 名称100 / 等级60
///   ListViewMonk(417x291)     列: 序号60 / 名称100 / 等级60
///   ListViewHero(417x291)     列: 序号60 / 英雄名称100 / 角色名称100 / 等级60
///   ListViewHeroWarrior(417x291) 列: 序号60 / 英雄名称100 / 角色名称100 / 等级60
///   ListViewHeroWizzard(417x291) 列: 序号60 / 英雄名称100 / 角色名称100 / 等级60
///   ListViewHeroMonk(417x291)    列: 序号60 / 英雄名称100 / 角色名称100 / 等级60
///   ListViewMaster(425x318)   列: 序号60 / 名称100 / 出师徒弟数100
/// </summary>
public class FrmRankingDlg : Form
{
    public GroupBox GroupBox1;
    public CheckBox CheckBoxAutoRefRanking;
    public Label Label1;
    public Label Label2;
    public TSpinEdit EditMinLevel;
    public TSpinEdit EditMaxLevel;
    public Label Label3;
    public Label Label4;
    public RadioButton RadioButton1;
    public RadioButton RadioButton2;
    public TSpinEdit EditTime;
    public TSpinEdit EditHour;
    public Label Label5;
    public Label Label6;
    public TSpinEdit EditMinute1;
    public TSpinEdit EditMinute2;
    public Label Label7;
    public Label Label8;
    public Button ButtonSave;
    public Button ButtonRefRanking;
    public TabControl PageControl1;
    public TabPage TabSheet1;
    public TabPage TabSheet2;
    public TabPage TabSheet3;
    public TabControl PageControl2;
    public TabPage TabSheet4;
    public TabPage TabSheet5;
    public TabPage TabSheet6;
    public TabPage TabSheet10;
    public TabControl PageControl3;
    public TabPage TabSheet7;
    public TabPage TabSheet8;
    public TabPage TabSheet9;
    public TabPage TabSheet11;
    public ListView ListViewHum;
    public ListView ListViewWarrior;
    public ListView ListViewWizzard;
    public ListView ListViewMonk;
    public ListView ListViewHero;
    public ListView ListViewHeroWarrior;
    public ListView ListViewHeroWizzard;
    public ListView ListViewHeroMonk;
    public ListView ListViewMaster;
    public System.Windows.Forms.Timer Timer;
    public Label lbl1;
    public TSpinEdit seRankingCount;

    /// <summary>Ranking.pas:73 `m_boRefRanking: Boolean`。</summary>
    private byte m_boRefRanking;

    /// <summary>Ranking.pas:46-54 的 ListView 集合（显示逻辑的唯一入口）。</summary>
    public readonly RankingDisplay Display = new RankingDisplay();

    public FrmRankingDlg()
    {
        // DFM: FrmRankingDlg Left=563 Top=265 BorderStyle=bsDialog Caption='排行榜管理'
        //      ClientHeight=465 ClientWidth=450 OnCloseQuery=FormCloseQuery OnCreate=FormCreate
        Text = "排行榜管理";
        StartPosition = FormStartPosition.Manual;
        Location = new System.Drawing.Point(563, 265);
        ClientSize = new System.Drawing.Size(450, 465);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: GroupBox1 Left=8 Top=8 Width=433 Height=97 Caption='排行榜设置' TabOrder=0
        GroupBox1 = new GroupBox { Left = 8, Top = 8, Width = 433, Height = 97, Text = "排行榜设置", TabIndex = 0 };

        // DFM: Label1 Left=8 Top=44 Width=54 Height=12 Caption='起始等级:'
        Label1 = new Label { Left = 8, Top = 44, Width = 54, Height = 12, Text = "起始等级:" };
        // DFM: Label2 Left=8 Top=68 Width=54 Height=12 Caption='最高等级:'
        Label2 = new Label { Left = 8, Top = 68, Width = 54, Height = 12, Text = "最高等级:" };
        // DFM: Label3 Left=128 Top=44 Width=12 Height=12 Caption='级'
        Label3 = new Label { Left = 128, Top = 44, Width = 12, Height = 12, Text = "级" };
        // DFM: Label4 Left=128 Top=68 Width=12 Height=12 Caption='级'
        Label4 = new Label { Left = 128, Top = 68, Width = 12, Height = 12, Text = "级" };
        // DFM: Label5 Left=256 Top=42 Width=12 Height=12 Caption='点'
        Label5 = new Label { Left = 256, Top = 42, Width = 12, Height = 12, Text = "点" };
        // DFM: Label6 Left=256 Top=66 Width=24 Height=12 Caption='小时'
        Label6 = new Label { Left = 256, Top = 66, Width = 24, Height = 12, Text = "小时" };
        // DFM: Label7 Left=344 Top=42 Width=12 Height=12 Caption='分'
        Label7 = new Label { Left = 344, Top = 42, Width = 12, Height = 12, Text = "分" };
        // DFM: Label8 Left=344 Top=66 Width=12 Height=12 Caption='分'
        Label8 = new Label { Left = 344, Top = 66, Width = 12, Height = 12, Text = "分" };
        // DFM: lbl1 Left=216 Top=18 Width=72 Height=12 Caption='排行榜数量：'
        lbl1 = new Label { Left = 216, Top = 18, Width = 72, Height = 12, Text = "排行榜数量：" };

        // DFM: CheckBoxAutoRefRanking Left=8 Top=16 Width=105 Height=17 Caption='自动刷新排行榜' TabOrder=0 OnClick=CheckBoxAutoRefRankingClick
        CheckBoxAutoRefRanking = new CheckBox { Left = 8, Top = 16, Width = 105, Height = 17, Text = "自动刷新排行榜", TabIndex = 0 };
        // DFM: EditMinLevel Left=64 Top=40 Width=57 Height=21 MaxValue=65535 MinValue=0 TabOrder=1 Value=0 OnChange=EditMinLevelChange
        EditMinLevel = new TSpinEdit { Left = 64, Top = 40, Width = 57, Height = 21, TabIndex = 1 };
        EditMinLevel.SetDfmRange(0, 65535);
        // DFM: EditMaxLevel Left=64 Top=64 Width=57 Height=21 MaxValue=65535 MinValue=0 TabOrder=2 Value=0 OnChange=EditMaxLevelChange
        EditMaxLevel = new TSpinEdit { Left = 64, Top = 64, Width = 57, Height = 21, TabIndex = 2 };
        EditMaxLevel.SetDfmRange(0, 65535);
        // DFM: RadioButton1 Left=144 Top=42 Width=49 Height=17 Caption='每天' TabOrder=3 OnClick=RadioButton1Click
        RadioButton1 = new RadioButton { Left = 144, Top = 42, Width = 49, Height = 17, Text = "每天", TabIndex = 3 };
        // DFM: RadioButton2 Left=144 Top=66 Width=49 Height=17 Caption='每隔' Checked=True TabOrder=4 OnClick=RadioButton1Click
        RadioButton2 = new RadioButton { Left = 144, Top = 66, Width = 49, Height = 17, Text = "每隔", TabIndex = 4, Checked = true };
        // DFM: EditTime Left=200 Top=40 Width=49 Height=21 MaxValue=65535 MinValue=0 TabOrder=5 Value=0 OnChange=EditTimeChange
        EditTime = new TSpinEdit { Left = 200, Top = 40, Width = 49, Height = 21, TabIndex = 5 };
        EditTime.SetDfmRange(0, 65535);
        // DFM: EditHour Left=200 Top=64 Width=49 Height=21 MaxValue=65535 MinValue=0 TabOrder=6 Value=0 OnChange=EditHourChange
        EditHour = new TSpinEdit { Left = 200, Top = 64, Width = 49, Height = 21, TabIndex = 6 };
        EditHour.SetDfmRange(0, 65535);
        // DFM: EditMinute1 Left=288 Top=40 Width=49 Height=21 MaxValue=65535 MinValue=0 TabOrder=7 Value=0 OnChange=EditMinute1Change
        EditMinute1 = new TSpinEdit { Left = 288, Top = 40, Width = 49, Height = 21, TabIndex = 7 };
        EditMinute1.SetDfmRange(0, 65535);
        // DFM: EditMinute2 Left=288 Top=64 Width=49 Height=21 MaxValue=65535 MinValue=0 TabOrder=8 Value=0 OnChange=EditMinute2Change
        EditMinute2 = new TSpinEdit { Left = 288, Top = 64, Width = 49, Height = 21, TabIndex = 8 };
        EditMinute2.SetDfmRange(0, 65535);
        // DFM: ButtonSave Left=366 Top=38 Width=59 Height=20 Caption='保存(&S)' TabOrder=9 OnClick=ButtonSaveClick
        ButtonSave = new Button { Left = 366, Top = 38, Width = 59, Height = 20, Text = "保存(&S)", TabIndex = 9 };
        // DFM: ButtonRefRanking Left=366 Top=64 Width=59 Height=20 Caption='刷新(&R)' TabOrder=10 OnClick=ButtonRefRankingClick
        ButtonRefRanking = new Button { Left = 366, Top = 64, Width = 59, Height = 20, Text = "刷新(&R)", TabIndex = 10 };
        // DFM: seRankingCount Left=288 Top=14 Width=49 Height=21 MaxValue=200 MinValue=1 TabOrder=11 Value=1 OnChange=seRankingCountChange
        seRankingCount = new TSpinEdit { Left = 288, Top = 14, Width = 49, Height = 21, TabIndex = 11 };
        seRankingCount.SetDfmRange(1, 200);

        // DFM: PageControl1 Left=8 Top=112 Width=433 Height=345 ActivePage=TabSheet1 TabOrder=1
        PageControl1 = new TabControl { Left = 8, Top = 112, Width = 433, Height = 345, TabIndex = 1 };
        TabSheet1 = new TabPage { Text = "个人榜" };      // DFM: TabSheet1 Caption='个人榜'
        TabSheet2 = new TabPage { Text = "英雄榜" };      // DFM: TabSheet2 Caption='英雄榜' ImageIndex=1
        TabSheet3 = new TabPage { Text = "名师榜" };      // DFM: TabSheet3 Caption='名师榜' ImageIndex=2

        // DFM: PageControl2 Left=0 Top=0 Width=425 Height=318 ActivePage=TabSheet4 Align=alClient TabOrder=0
        PageControl2 = new TabControl { Left = 0, Top = 0, Width = 425, Height = 318, TabIndex = 0, Dock = DockStyle.Fill };
        TabSheet4 = new TabPage { Text = "群英榜" };      // DFM: TabSheet4 Caption='群英榜'
        TabSheet5 = new TabPage { Text = "战神榜" };      // DFM: TabSheet5 Caption='战神榜' ImageIndex=1
        TabSheet6 = new TabPage { Text = "法圣榜" };      // DFM: TabSheet6 Caption='法圣榜' ImageIndex=2
        TabSheet10 = new TabPage { Text = "道尊榜" };     // DFM: TabSheet10 Caption='道尊榜' ImageIndex=3

        // DFM: PageControl3 Left=0 Top=0 Width=425 Height=318 ActivePage=TabSheet11 Align=alClient TabOrder=0
        PageControl3 = new TabControl { Left = 0, Top = 0, Width = 425, Height = 318, TabIndex = 0, Dock = DockStyle.Fill };
        TabSheet7 = new TabPage { Text = "群英榜" };      // DFM: TabSheet7 Caption='群英榜'
        TabSheet8 = new TabPage { Text = "战神榜" };      // DFM: TabSheet8 Caption='战神榜' ImageIndex=1
        TabSheet9 = new TabPage { Text = "法圣榜" };      // DFM: TabSheet9 Caption='法圣榜' ImageIndex=2
        TabSheet11 = new TabPage { Text = "道尊榜" };     // DFM: TabSheet11 Caption='道尊榜' ImageIndex=3

        ListViewHum = MakeListView("序号", 60, "名称", 100, "等级", 60);
        ListViewWarrior = MakeListView("序号", 60, "名称", 100, "等级", 60);
        ListViewWizzard = MakeListView("序号", 60, "名称", 100, "等级", 60);
        ListViewMonk = MakeListView("序号", 60, "名称", 100, "等级", 60);
        ListViewHero = MakeListView("序号", 60, "英雄名称", 100, "角色名称", 100, "等级", 60);
        ListViewHeroWarrior = MakeListView("序号", 60, "英雄名称", 100, "角色名称", 100, "等级", 60);
        ListViewHeroWizzard = MakeListView("序号", 60, "英雄名称", 100, "角色名称", 100, "等级", 60);
        ListViewHeroMonk = MakeListView("序号", 60, "英雄名称", 100, "角色名称", 100, "等级", 60);
        ListViewMaster = MakeListView("序号", 60, "名称", 100, "出师徒弟数", 100);

        TabSheet4.Controls.Add(ListViewHum);
        TabSheet5.Controls.Add(ListViewWarrior);
        TabSheet6.Controls.Add(ListViewWizzard);
        TabSheet10.Controls.Add(ListViewMonk);
        TabSheet7.Controls.Add(ListViewHero);
        TabSheet8.Controls.Add(ListViewHeroWarrior);
        TabSheet9.Controls.Add(ListViewHeroWizzard);
        TabSheet11.Controls.Add(ListViewHeroMonk);
        TabSheet3.Controls.Add(ListViewMaster);

        PageControl2.TabPages.Add(TabSheet4);
        PageControl2.TabPages.Add(TabSheet5);
        PageControl2.TabPages.Add(TabSheet6);
        PageControl2.TabPages.Add(TabSheet10);
        TabSheet1.Controls.Add(PageControl2);

        PageControl3.TabPages.Add(TabSheet7);
        PageControl3.TabPages.Add(TabSheet8);
        PageControl3.TabPages.Add(TabSheet9);
        PageControl3.TabPages.Add(TabSheet11);
        TabSheet2.Controls.Add(PageControl3);

        PageControl1.TabPages.Add(TabSheet1);
        PageControl1.TabPages.Add(TabSheet2);
        PageControl1.TabPages.Add(TabSheet3);

        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(Label3);
        GroupBox1.Controls.Add(Label4);
        GroupBox1.Controls.Add(Label5);
        GroupBox1.Controls.Add(Label6);
        GroupBox1.Controls.Add(Label7);
        GroupBox1.Controls.Add(Label8);
        GroupBox1.Controls.Add(lbl1);
        GroupBox1.Controls.Add(CheckBoxAutoRefRanking);
        GroupBox1.Controls.Add(EditMinLevel);
        GroupBox1.Controls.Add(EditMaxLevel);
        GroupBox1.Controls.Add(RadioButton1);
        GroupBox1.Controls.Add(RadioButton2);
        GroupBox1.Controls.Add(EditTime);
        GroupBox1.Controls.Add(EditHour);
        GroupBox1.Controls.Add(EditMinute1);
        GroupBox1.Controls.Add(EditMinute2);
        GroupBox1.Controls.Add(ButtonSave);
        GroupBox1.Controls.Add(ButtonRefRanking);
        GroupBox1.Controls.Add(seRankingCount);

        // DFM: Timer Enabled=False OnTimer=TimerTimer
        Timer = new System.Windows.Forms.Timer { Enabled = false };

        Controls.Add(GroupBox1);
        Controls.Add(PageControl1);

        Load += (s, e) => FormCreate(s, e);
        FormClosing += (s, e) =>
        {
            bool CanClose = true;
            FormCloseQuery(s, ref CanClose);
            if (!CanClose) e.Cancel = true;
        };
        Timer.Tick += (s, e) => TimerTimer(s, e);
        ButtonSave.Click += (s, e) => ButtonSaveClick(s, e);
        ButtonRefRanking.Click += (s, e) => ButtonRefRankingClick(s, e);
        CheckBoxAutoRefRanking.Click += (s, e) => CheckBoxAutoRefRankingClick(s, e);
        EditMinLevel.ValueChanged += (s, e) => EditMinLevelChange(s, e);
        EditMaxLevel.ValueChanged += (s, e) => EditMaxLevelChange(s, e);
        EditTime.ValueChanged += (s, e) => EditTimeChange(s, e);
        EditHour.ValueChanged += (s, e) => EditHourChange(s, e);
        EditMinute1.ValueChanged += (s, e) => EditMinute1Change(s, e);
        EditMinute2.ValueChanged += (s, e) => EditMinute2Change(s, e);
        RadioButton1.Click += (s, e) => RadioButton1Click(s, e);
        RadioButton2.Click += (s, e) => RadioButton1Click(s, e);
        seRankingCount.ValueChanged += (s, e) => seRankingCountChange(s, e);

        Display.ListViewHum = new ListViewSink(ListViewHum);
        Display.ListViewWarrior = new ListViewSink(ListViewWarrior);
        Display.ListViewWizzard = new ListViewSink(ListViewWizzard);
        Display.ListViewMonk = new ListViewSink(ListViewMonk);
        Display.ListViewHero = new ListViewSink(ListViewHero);
        Display.ListViewHeroWarrior = new ListViewSink(ListViewHeroWarrior);
        Display.ListViewHeroWizzard = new ListViewSink(ListViewHeroWizzard);
        Display.ListViewHeroMonk = new ListViewSink(ListViewHeroMonk);
        Display.ListViewMaster = new ListViewSink(ListViewMaster);
    }

    private static ListView MakeListView(params object[] headerPairs)
    {
        var lv = new ListView
        {
            Left = 0,
            Top = 0,
            Width = 417,
            Height = 291,
            Dock = DockStyle.Fill,      // DFM: Align=alClient
            GridLines = true,           // DFM: GridLines=True
            FullRowSelect = true,       // DFM: RowSelect=True
            View = View.Details,        // DFM: ViewStyle=vsReport
            MultiSelect = false
        };
        for (int i = 0; i + 1 < headerPairs.Length; i += 2)
            lv.Columns.Add((string)headerPairs[i], (int)headerPairs[i + 1]);
        return lv;
    }

    /// <summary>Ranking.pas:87-105 `Open`：全局量 → 控件；ButtonSave.Enabled := False；Timer.Enabled := True；ShowModal。</summary>
    public void Open()
    {
        m_boRefRanking = 0;
        CheckBoxAutoRefRanking.Checked = DBShareSeam.g_boAutoRefRanking != 0;
        seRankingCount.Value = DBShareSeam.g_nRankingCount;
        EditMinLevel.Value = DBShareSeam.g_nRankingMinLevel;
        EditMaxLevel.Value = DBShareSeam.g_nRankingMaxLevel;
        EditTime.Value = DBShareSeam.g_nRefRankingHour1;
        EditHour.Value = DBShareSeam.g_nRefRankingHour2;
        EditMinute1.Value = DBShareSeam.g_nRefRankingMinute1;
        EditMinute2.Value = DBShareSeam.g_nRefRankingMinute2;
        if (DBShareSeam.g_nAutoRefRankingType == 0) RadioButton1.Checked = true;
        if (DBShareSeam.g_nAutoRefRankingType == 1) RadioButton2.Checked = true;
        //RadioButton2.Checked:= Boolean(g_nAutoRefRankingType);
        ButtonSave.Enabled = false;
        Timer.Enabled = true;

        ShowDialog();   // Self.ShowModal
    }

    /// <summary>Ranking.pas:107-221 `RefRanking`（转发到 RankingLogic.RefRanking）。</summary>
    public void RefRanking()
    {
        RankingLogic.RefRanking(Display, ref m_boRefRanking);
    }

    /// <summary>Ranking.pas:223-232 `ButtonRefRankingClick`。</summary>
    public void ButtonRefRankingClick(object Sender, EventArgs e)
    {
        if (DBShareSeam.g_boRefRanking != 0) return;

        ButtonRefRanking.Enabled = false;
        DBShareSeam.g_dwAutoRefRankingTick = DelphiTick.GetTickCount();
        DBShareSeam.RankingEngine_RefRanking();
        RefRanking();
        ButtonRefRanking.Enabled = true;
    }

    /// <summary>Ranking.pas:234-255 `ButtonSaveClick`。</summary>
    public void ButtonSaveClick(object Sender, EventArgs e)
    {
        RankingLogic.ButtonSave();
        ButtonSave.Enabled = false;
    }

    /// <summary>Ranking.pas:257-261 `CheckBoxAutoRefRankingClick`。</summary>
    public void CheckBoxAutoRefRankingClick(object Sender, EventArgs e)
    {
        DBShareSeam.g_boAutoRefRanking = TBool.ToByte(CheckBoxAutoRefRanking.Checked);
        ButtonSave.Enabled = true;
    }

    /// <summary>Ranking.pas:263-267 `EditMinLevelChange`。</summary>
    public void EditMinLevelChange(object Sender, EventArgs e)
    {
        DBShareSeam.g_nRankingMinLevel = EditMinLevel.Value;
        ButtonSave.Enabled = true;
    }

    /// <summary>Ranking.pas:269-273 `EditMaxLevelChange`。</summary>
    public void EditMaxLevelChange(object Sender, EventArgs e)
    {
        DBShareSeam.g_nRankingMaxLevel = EditMaxLevel.Value;
        ButtonSave.Enabled = true;
    }

    /// <summary>Ranking.pas:275-279 `EditTimeChange`。</summary>
    public void EditTimeChange(object Sender, EventArgs e)
    {
        DBShareSeam.g_nRefRankingHour1 = EditTime.Value;
        ButtonSave.Enabled = true;
    }

    /// <summary>Ranking.pas:281-285 `EditHourChange`。</summary>
    public void EditHourChange(object Sender, EventArgs e)
    {
        DBShareSeam.g_nRefRankingHour2 = EditHour.Value;
        ButtonSave.Enabled = true;
    }

    /// <summary>Ranking.pas:287-291 `EditMinute1Change`。</summary>
    public void EditMinute1Change(object Sender, EventArgs e)
    {
        DBShareSeam.g_nRefRankingMinute1 = EditMinute1.Value;
        ButtonSave.Enabled = true;
    }

    /// <summary>Ranking.pas:293-297 `EditMinute2Change`。</summary>
    public void EditMinute2Change(object Sender, EventArgs e)
    {
        DBShareSeam.g_nRefRankingMinute2 = EditMinute2.Value;
        ButtonSave.Enabled = true;
    }

    /// <summary>Ranking.pas:299-306 `RadioButton1Click`（RadioButton1/2 共用同一 OnClick）。</summary>
    public void RadioButton1Click(object Sender, EventArgs e)
    {
        RankingLogic.RadioButton1Click(RadioButton1.Checked);
        ButtonSave.Enabled = true;
    }

    /// <summary>Ranking.pas:308-312 `FormCreate`：两个 PageControl 都定位到第 0 页。</summary>
    public void FormCreate(object Sender, EventArgs e)
    {
        PageControl1.SelectedIndex = 0;   // ActivePageIndex := 0
        PageControl2.SelectedIndex = 0;
    }

    /// <summary>Ranking.pas:314-318 `TimerTimer`：Timer.Enabled := False; RefRanking。</summary>
    public void TimerTimer(object Sender, EventArgs e)
    {
        Timer.Enabled = false;
        RefRanking();
    }

    /// <summary>Ranking.pas:320-324 `FormCloseQuery`：刷新中禁止关闭。</summary>
    public void FormCloseQuery(object Sender, ref bool CanClose)
    {
        if (m_boRefRanking != 0) CanClose = false;
    }

    /// <summary>Ranking.pas:326-330 `seRankingCountChange`。</summary>
    public void seRankingCountChange(object Sender, EventArgs e)
    {
        DBShareSeam.g_nRankingCount = seRankingCount.Value;
        ButtonSave.Enabled = true;
    }
}

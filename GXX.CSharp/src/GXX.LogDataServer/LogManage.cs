using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.LogDataServer;

namespace GXX.LogDataServer;

/// <summary>
/// LogManage.pas TFrmLogManage 1:1（日志查询窗体）。
/// 源码：LogManage.pas:206-1039；DFM：LogManage.dfm（Caption='日志查询', ClientWidth=1330, ClientHeight=849）。
/// TVirtualStringTree → TVirtualStringTreeModel 接缝；TSearchManager/TSearchTask → LogDataShare 接缝。
/// </summary>
public sealed class TFrmLogManage : System.Windows.Forms.Form
{
    // ---- LogManage.pas:101-148 常量（1:1）----
    public const int LOG_ActionNone = 00;

    public const int LOG_ItemRefining = 01;
    public const int LOG_ItemMake = 02;
    public const int LOG_ItemNpcGive = 03;
    public const int LOG_ItemNpcRecycle = 04;
    public const int LOG_ItemPickup = 05;
    public const int LOG_ItemDrop = 06;
    public const int LOG_ButchItem = 07;
    public const int LOG_ItemFall = 08;
    public const int LOG_ItemDisappear = 09;
    public const int LOG_ItemSell = 10;
    public const int LOG_ItemBuy = 11;
    public const int LOG_ItemInlaid = 12;
    public const int LOG_ItemDisassemble = 13;
    public const int LOG_ItemUpgrade = 14;
    public const int LOG_ItemTrading = 15;
    public const int LOG_ItemChallenge = 16;
    public const int LOG_ItemPutInto = 17;
    public const int LOG_ItemTakeBack = 18;
    public const int LOG_ItemMove = 19;
    public const int LOG_ItemSplit = 20;
    public const int LOG_ItemOverlap = 21;
    public const int LOG_ItemSellPrice = 22;
    public const int LOG_ItemBuyPrice = 23;
    public const int LOG_ItemUpdate = 24;
    public const int LOG_ItemTakeOn = 25;
    public const int LOG_ItemTakeOff = 26;
    public const int LOG_ItemNpcMonDrop = 27;

    public const int LOG_GoldChange = 50;
    public const int LOG_GameGoldChange = 51;
    public const int LOG_GamePointChange = 52;
    public const int LOG_GameDiamondChange = 53;
    public const int LOG_GameGirdChange = 54;
    public const int LOG_CreditPointChange = 55;
    public const int LOG_GamegLoryChange = 56;
    public const int LOG_LevelChange = 57;
    public const int LOG_AbilPointChange = 58;

    public const int LOG_CastleSaveMoney = 60;
    public const int LOG_CastleGetMoney = 61;

    public const int LOG_PlayerDie = 70;
    public const int LOG_PlayerLogon = 71;
    public const int LOG_PlayerLogOff = 72;
    public const int LOG_PlayerTrading = 73;

    /// <summary>LogManage.pas TActionCategory。</summary>
    public enum TActionCategory
    {
        acAll,
        acParent,
        acSpecify,
    }

    /// <summary>LogManage.pas TNodeData（Action 为 Byte，Text 为 string）。</summary>
    public sealed class TLogTypeNodeData
    {
        public TActionCategory Category;
        public byte Action;
        public string Text = "";
    }

    /// <summary>LogManage.pas:150 ActionNames: array[0..42-1] of TLogAction（42 项，顺序 1:1）。</summary>
    public static readonly (int Action, string Text)[] ActionNames =
    {
        // 物品相关
        (LOG_ItemRefining,       "炼制物品"),
        (LOG_ItemMake,           "制造物品"),
        (LOG_ItemNpcGive,        "NPC给予"),
        (LOG_ItemNpcRecycle,     "NPC回收"),
        (LOG_ItemPickup,         "捡取物品"),
        (LOG_ItemDrop,           "丢弃物品"),
        (LOG_ButchItem,          "挖到物品"),
        (LOG_ItemFall,           "掉落物品"),
        (LOG_ItemDisappear,      "物品消失"),
        (LOG_ItemSell,           "卖出物品"),
        (LOG_ItemBuy,            "买入物品"),
        (LOG_ItemSellPrice,      "卖出费用"),
        (LOG_ItemBuyPrice,       "买入费用"),
        (LOG_ItemInlaid,         "镶嵌物品"),
        (LOG_ItemDisassemble,    "拆开镶嵌"),
        (LOG_ItemUpgrade,        "物品升级"),
        (LOG_ItemTrading,        "交易物品"),
        (LOG_ItemChallenge,      "挑战物品"),
        (LOG_ItemPutInto,        "放入物品"),
        (LOG_ItemTakeBack,       "取回物品"),
        (LOG_ItemMove,           "移动物品"),
        (LOG_ItemSplit,          "拆分物品"),
        (LOG_ItemOverlap,        "叠加物品"),
        (LOG_ItemUpdate,         "物品更新"),
        (LOG_ItemTakeOn,         "穿戴装备"),
        (LOG_ItemTakeOff,        "脱下装备"),
        (LOG_ItemNpcMonDrop,     "命令爆出"),

        // 普通数据
        (LOG_CastleSaveMoney,    "城堡存钱"),
        (LOG_CastleGetMoney,     "城堡取钱"),
        (LOG_GoldChange,         "金币改变"),
        (LOG_GameGoldChange,     "元宝改变"),
        (LOG_GamePointChange,    "游戏点改变"),
        (LOG_GameDiamondChange,  "金刚石改变"),
        (LOG_GameGirdChange,     "灵符改变"),
        (LOG_CreditPointChange,  "声望改变"),
        (LOG_GamegLoryChange,    "荣誉值改变"),
        (LOG_LevelChange,        "等级改变"),
        (LOG_AbilPointChange,    "属性点改变"),

        // 其他动作
        (LOG_PlayerDie,          "人物死亡"),
        (LOG_PlayerLogon,        "人物上线"),
        (LOG_PlayerLogOff,       "人物下线"),
        (LOG_PlayerTrading,      "角色交易"),
    };

    /// <summary>LogManage.pas:215 GetActString（★ 取 LoWord→LoByte 后按 Action 匹配，找不到返回 '无法分析'）。</summary>
    public static string GetActString(uint nAct)
    {
        // 日志类型暂时只用了 Word
        int W1 = (int)DelphiRTL.LoWord((int)nAct);
        byte B1 = DelphiRTL.LoByte(W1);
        //B2 := HiByte(W1);

        string Result = "无法分析";
        if (nAct >= 0)
        {
            for (int I = 0; I <= ActionNames.Length - 1; I++)
            {
                if (ActionNames[I].Action == B1)
                {
                    Result = ActionNames[I].Text;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// LogManage.pas:240 LastDirectoryName。
    /// ★ Delphi 中 Directory[Length(Directory)] 对空串取的是 S[0]（长度字节 #0）→ 返回 ''；此处 1:1 复刻。
    /// </summary>
    public static string LastDirectoryName(string Directory)
    {
        string Result = "";
        char last = Directory.Length == 0 ? '\0' : Directory[Directory.Length - 1];
        if (last == '\\')
            Directory = DelphiRTL.Copy(Directory, 1, Directory.Length - 1);
        for (int I = Directory.Length; I >= 1; I--)
        {
            if (Directory[I - 1] == '\\')
            {
                Result = DelphiRTL.Copy(Directory, I + 1, Directory.Length - I + 1);
                break;
            }
        }
        return Result;
    }

    // ---- 控件（DFM 1:1）----
    public System.Windows.Forms.Panel Panel = null!;
    public System.Windows.Forms.Label Label1 = null!;   // '开始日期:'
    public System.Windows.Forms.Label Label2 = null!;   // '结束日期:'
    public System.Windows.Forms.DateTimePicker DateTimeEditBegin = null!;
    public System.Windows.Forms.DateTimePicker DateTimeEditEnd = null!;
    public System.Windows.Forms.Button btnStart = null!;   // '开始查询' / '停止查询'
    public System.Windows.Forms.ContextMenuStrip PopupMenu = null!;
    public System.Windows.Forms.ToolStripMenuItem pmiCopy = null!;         // '复制'
    public System.Windows.Forms.ToolStripMenuItem pmiCopyLine = null!;     // '复制选中行'
    public System.Windows.Forms.ToolStripMenuItem N1 = null!;              // '-'
    public System.Windows.Forms.ToolStripMenuItem pmiExportLine = null!;   // '导出选中行'
    public System.Windows.Forms.ToolStripMenuItem pmiExportAll = null!;    // '导出所有'
    public System.Windows.Forms.StatusStrip StatusBar = null!;
    public System.Windows.Forms.Timer Timer = null!;
    public System.Windows.Forms.CheckBox chkObjName = null!;   // '角色名称'
    public System.Windows.Forms.TextBox edtObjName = null!;
    public System.Windows.Forms.CheckBox chkItemName = null!;  // '物品名'
    public System.Windows.Forms.TextBox edtItemName = null!;
    public System.Windows.Forms.CheckBox chkItemID = null!;    // '物品ID'
    public System.Windows.Forms.TextBox edtItemID = null!;
    public System.Windows.Forms.CheckBox chkActObjName = null!;  // '目标对象'
    public System.Windows.Forms.TextBox edtActObjName = null!;
    public System.Windows.Forms.CheckBox chkObjType = null!;   // '角色类型'
    public System.Windows.Forms.ComboBox cbbActionType = null!;
    public System.Windows.Forms.SaveFileDialog dlgSave = null!;
    public System.Windows.Forms.Panel pnlClient = null!;
    public System.Windows.Forms.Splitter splLeft = null!;
    public TVirtualStringTreeModel vstLog = null!;
    public TVirtualStringTreeModel vstLogType = null!;

    /// <summary>DFM: StatusBar 4 个 Panel（宽 100/100/400/50）。</summary>
    private System.Windows.Forms.ToolStripStatusLabel[] _statusPanels = null!;

    /// <summary>DFM: vstLog 14 列标题/宽度。</summary>
    public static readonly string[] LogColumnTitles =
        { "序号", "动作", "地图", "坐标X", "坐标Y", "角色名称", "角色类型", "物品名称", "物品ID", "目标对象", "新数据", "参考数据", "描述", "时间" };
    public static readonly int[] LogColumnWidths = { 0, 90, 60, 46, 46, 90, 65, 90, 80, 120, 80, 69, 168, 130 };

    /// <summary>测试注入：非 null 时替代 dlgSave.Execute；返回 null 表示取消。</summary>
    public static Func<string?>? SaveFileProvider;

    /// <summary>测试注入：非 null 时替代剪贴板写入。</summary>
    public static Action<string>? ClipboardHandler;

    private TThreadList FLogDataList = null!;
    private int FTaskCount;

    public const int SC_MINIMIZE = 0xF020;

    /// <summary>Win32 WM_SYSCOMMAND 参数（对应 TWMSYSCommand.CmdType）。</summary>
    public struct TWMSYSCommand
    {
        public uint CmdType;
    }

    public TFrmLogManage()
    {
        InitializeComponent();
        FormCreate(this);
    }

    /// <summary>StatusBar.Panels[i]（Delphi 0-based）。</summary>
    public System.Windows.Forms.ToolStripStatusLabel StatusPanel(int index) => _statusPanels[index];

    private void InitializeComponent()
    {
        // DFM: Caption = '日志查询' / ClientWidth = 1330  ClientHeight = 849 / Position = poMainFormCenter
        Text = "日志查询";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        ClientSize = new System.Drawing.Size(1330, 849);

        // DFM: Panel TPanel(0,0,1330,36) Align=alTop BevelOuter=bvNone
        Panel = new System.Windows.Forms.Panel { Left = 0, Top = 0, Width = 1330, Height = 36, TabIndex = 0 };
        Label1 = new System.Windows.Forms.Label { Text = "开始日期:", Left = 8, Top = 12, Width = 54, Height = 12, AutoSize = false };
        Label2 = new System.Windows.Forms.Label { Text = "结束日期:", Left = 167, Top = 12, Width = 54, Height = 12, AutoSize = false };
        // DFM: DateTimeEditBegin TRzDateTimeEdit(61,8,92,20) EditType=etDate TabOrder=0
        DateTimeEditBegin = new System.Windows.Forms.DateTimePicker { Left = 61, Top = 8, Width = 92, Height = 20, Format = System.Windows.Forms.DateTimePickerFormat.Short, TabIndex = 0 };
        DateTimeEditBegin.ValueChanged += (s, e) => DateTimeEditBeginDateTimeChange(s, DateTimeEditBegin.Value);
        // DFM: DateTimeEditEnd TRzDateTimeEdit(220,8,92,20) EditType=etDate TabOrder=1
        DateTimeEditEnd = new System.Windows.Forms.DateTimePicker { Left = 220, Top = 8, Width = 92, Height = 20, Format = System.Windows.Forms.DateTimePickerFormat.Short, TabIndex = 1 };
        DateTimeEditEnd.ValueChanged += (s, e) => DateTimeEditEndDateTimeChange(s, DateTimeEditEnd.Value);
        // DFM: btnStart TButton(1224,5,75,25) Caption='开始查询' TabOrder=12
        btnStart = new System.Windows.Forms.Button { Text = "开始查询", Left = 1224, Top = 5, Width = 75, Height = 25, TabIndex = 12 };
        btnStart.Click += (s, e) => btnStartClick(s);

        // DFM 顶部查询条件：chkObjName(332,9) edtObjName(400,8,99,20) chkObjType(524,9) cbbActionType(594,8,75,20)
        //                       chkActObjName(692,9) edtActObjName(760,8,99,20) chkItemName(880,9)
        //                       edtItemName(938,8,99,20) chkItemID(1058,9) edtItemID(1116,8,99,20)
        chkObjName = Chk("角色名称", 332, 9, 70, 2);
        edtObjName = Ed(400, 8, 99, 3);
        chkObjType = Chk("角色类型", 524, 9, 70, 4);
        cbbActionType = new System.Windows.Forms.ComboBox
        {
            Left = 594,
            Top = 8,
            Width = 75,
            Height = 20,
            DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
            TabIndex = 5,
        };
        chkActObjName = Chk("目标对象", 692, 9, 70, 6);
        edtActObjName = Ed(760, 8, 99, 7);
        chkItemName = Chk("物品名", 880, 9, 59, 8);
        edtItemName = Ed(938, 8, 99, 9);
        chkItemID = Chk("物品ID", 1058, 9, 59, 10);
        edtItemID = Ed(1116, 8, 99, 11);

        Panel.Controls.Add(Label1);
        Panel.Controls.Add(Label2);
        Panel.Controls.Add(DateTimeEditBegin);
        Panel.Controls.Add(DateTimeEditEnd);
        Panel.Controls.Add(btnStart);
        Panel.Controls.Add(chkObjName);
        Panel.Controls.Add(edtObjName);
        Panel.Controls.Add(chkObjType);
        Panel.Controls.Add(cbbActionType);
        Panel.Controls.Add(chkActObjName);
        Panel.Controls.Add(edtActObjName);
        Panel.Controls.Add(chkItemName);
        Panel.Controls.Add(edtItemName);
        Panel.Controls.Add(chkItemID);
        Panel.Controls.Add(edtItemID);

        // DFM: StatusBar TStatusBar(0,830,1330,19) Panels 宽 100/100/400/50
        StatusBar = new System.Windows.Forms.StatusStrip { Left = 0, Top = 830, Width = 1330, Height = 19, TabIndex = 1 };
        _statusPanels = new System.Windows.Forms.ToolStripStatusLabel[4];
        int[] widths = { 100, 100, 400, 50 };
        for (int i = 0; i < 4; i++)
        {
            _statusPanels[i] = new System.Windows.Forms.ToolStripStatusLabel { Text = "", Width = widths[i], AutoSize = false };
            StatusBar.Items.Add(_statusPanels[i]);
        }

        // DFM: pnlClient TPanel(0,36,1330,794) Align=alClient；splLeft(137,1,4,792)；vstLog 14 列；vstLogType(1,1,136,792) alLeft
        pnlClient = new System.Windows.Forms.Panel { Left = 0, Top = 36, Width = 1330, Height = 794, TabIndex = 2 };
        splLeft = new System.Windows.Forms.Splitter { Left = 137, Top = 1, Width = 4, Height = 792, TabIndex = 0 };
        vstLog = new TVirtualStringTreeModel();
        vstLogType = new TVirtualStringTreeModel();
        vstLog.TextHandler = (node, column) =>
        {
            string cellText = "";
            vstLogGetText(vstLog, node, column, ref cellText);
            return cellText;
        };
        vstLogType.TextHandler = (node, column) =>
        {
            string cellText = "";
            vstLogTypeGetText(vstLogType, node, column, ref cellText);
            return cellText;
        };
        vstLog.Header.SortTreeHandler = (column, direction, recursive) => SortLogTree(column, direction);

        // DFM: PopupMenu 项（复制 / 复制选中行 / - / 导出选中行 / 导出所有）
        pmiCopy = new System.Windows.Forms.ToolStripMenuItem("复制");
        pmiCopy.Click += (s, e) => pmiCopyClick(s);
        pmiCopyLine = new System.Windows.Forms.ToolStripMenuItem("复制选中行");
        pmiCopyLine.Click += (s, e) => pmiCopyLineClick(s);
        N1 = new System.Windows.Forms.ToolStripMenuItem("-");
        pmiExportLine = new System.Windows.Forms.ToolStripMenuItem("导出选中行");
        pmiExportLine.Click += (s, e) => pmiExportLineClick(s);
        pmiExportAll = new System.Windows.Forms.ToolStripMenuItem("导出所有");
        pmiExportAll.Click += (s, e) => pmiExportAllClick(s);
        PopupMenu = new System.Windows.Forms.ContextMenuStrip();
        PopupMenu.Items.Add(pmiCopy);
        PopupMenu.Items.Add(pmiCopyLine);
        PopupMenu.Items.Add(N1);
        PopupMenu.Items.Add(pmiExportLine);
        PopupMenu.Items.Add(pmiExportAll);

        // DFM: Timer TTimer Enabled=False Interval=100
        Timer = new System.Windows.Forms.Timer { Interval = 100, Enabled = false };
        Timer.Tick += (s, e) => TimerTimer(s);

        // DFM: dlgSave TSaveDialog Filter='文本文件(*.txt)|*.txt' Title='导出日志数据'
        dlgSave = new System.Windows.Forms.SaveFileDialog { Filter = "文本文件(*.txt)|*.txt", Title = "导出日志数据" };

        Controls.Add(Panel);
        Controls.Add(StatusBar);
        Controls.Add(pnlClient);
        Controls.Add(splLeft);
        // DFM: Timer 是非可视组件（TTimer），不加入 Controls
    }

    private static System.Windows.Forms.CheckBox Chk(string caption, int left, int top, int width, int tab)
        => new() { Text = caption, Left = left, Top = top, Width = width, Height = 17, TabIndex = tab, AutoSize = false };

    private static System.Windows.Forms.TextBox Ed(int left, int top, int width, int tab)
        => new() { Left = left, Top = top, Width = width, Height = 20, TabIndex = tab };

    // ================= Delphi 1:1 =================

    /// <summary>LogManage.pas:255 DateTimeEditBeginDateTimeChange。</summary>
    public void DateTimeEditBeginDateTimeChange(object? Sender, DateTime DateTime)
    {
        if (DateTime > DateTimeEditEnd.Value.Date)
            DateTimeEditEnd.Value = DateTime;
    }

    /// <summary>LogManage.pas:262 DateTimeEditEndDateTimeChange（★ 原文改的是 DateTimeEditEnd.Date 而非 Begin）。</summary>
    public void DateTimeEditEndDateTimeChange(object? Sender, DateTime DateTime)
    {
        if (DateTime < DateTimeEditBegin.Value.Date)
            DateTimeEditEnd.Value = DateTimeEditBegin.Value.Date;
    }

    /// <summary>LogManage.pas:269 DoSearchFile（*.nlf 且文件名前 4 字符大小写不敏感等于 'log-'）。</summary>
    public void DoSearchFile(string Path, TStringList FileList)
    {
        Path = IncludeTrailingBackslash(Path);

        string[] entries;
        try
        {
            entries = Directory.GetFileSystemEntries(Path, "*.nlf");
        }
        catch (DirectoryNotFoundException)
        {
            return;   // 原文 FindFirst 返回非 0（未找到/目录不存在）→ 直接 FindClose
        }
        catch (IOException)
        {
            return;
        }

        // 原文 repeat ... continue ... until FindNext(Info) <> 0 —— continue 同样跳到条件求值
        foreach (string entry in entries)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(entry);
                if (((int)attr & 0x10) == 0x10) continue;      // Info.Attr and faDirectory = faDirectory
            }
            catch (IOException)
            {
                continue;
            }
            string name = System.IO.Path.GetFileName(entry);
            if (!SameText(DelphiRTL.Copy(name, 1, 4), "log-")) continue;

            string FileName = Path + name;

            FileList.Add(FileName);
        }
    }

    private static string IncludeTrailingBackslash(string path)
        => path.EndsWith("\\", StringComparison.Ordinal) || path.EndsWith("/", StringComparison.Ordinal) ? path : path + "\\";

    private static bool SameText(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>LogManage.pas:292 btnStartClick。</summary>
    public void btnStartClick(object? Sender)
    {
        if (Equals(btnStart.Tag, 1))
        {
            TSearchManagerHost.g_SearchManager.CancelAndClearAllTask();
            btnStart.Tag = 0;
            btnStart.Text = "开始查询";
            return;
        }

        string sObjName = DelphiRTL.Trim(edtObjName.Text);
        string sActObjName = DelphiRTL.Trim(edtActObjName.Text);
        string sItemName = DelphiRTL.Trim(edtItemName.Text);
        string sItemID = DelphiRTL.Trim(edtItemID.Text);

        if (chkObjName.Checked && sObjName.Length == 0)
        {
            LogDataForms.MessageBox("请输入查询的人物名称 ！！！", "提示信息", LogDataForms.MB_ICONQUESTION);
            edtObjName.Focus();
            return;
        }

        if (chkActObjName.Checked && sActObjName.Length == 0)
        {
            LogDataForms.MessageBox("请输入查询的交易对象 ！！！", "提示信息", LogDataForms.MB_ICONQUESTION);
            edtActObjName.Focus();
            return;
        }

        if (chkItemName.Checked && sItemName.Length == 0)
        {
            LogDataForms.MessageBox("请输入查询的物品名称 ！！！", "提示信息", LogDataForms.MB_ICONQUESTION);
            edtItemName.Focus();
            return;
        }

        if (chkItemID.Checked && sItemID.Length == 0)
        {
            LogDataForms.MessageBox("请输入查询的物品ID ！！！", "提示信息", LogDataForms.MB_ICONQUESTION);
            edtItemID.Focus();
            return;
        }

        int nItemID = DelphiRTL.StrToIntDef(sItemID, 0);
        if (chkItemID.Checked && nItemID <= 0)
        {
            LogDataForms.MessageBox("物品ID必须是正整数 ！！！", "提示信息", LogDataForms.MB_ICONQUESTION);
            edtItemID.Focus();
            return;
        }

        int SearchWhere = 0;
        if (chkObjName.Checked) SearchWhere = SearchWhere + 1;
        if (chkObjType.Checked) SearchWhere = SearchWhere + 2;
        if (chkActObjName.Checked) SearchWhere = SearchWhere + 4;
        if (chkItemName.Checked) SearchWhere = SearchWhere + 8;
        if (chkItemID.Checked) SearchWhere = SearchWhere + 16;

        btnStart.Tag = 1;
        btnStart.Text = "停止查询";

        StatusPanel(3).Text = "";

        vstLog.Clear();

        ClearLogDataList();

        TSearchManagerHost.g_SearchManager.SearchWhere = SearchWhere;
        TSearchManagerHost.g_SearchManager.SearchObjName = sObjName;
        TSearchManagerHost.g_SearchManager.SearchActObjName = sActObjName;
        TSearchManagerHost.g_SearchManager.SearchActObjType = cbbActionType.SelectedIndex;
        TSearchManagerHost.g_SearchManager.SearchItemName = sItemName;
        TSearchManagerHost.g_SearchManager.SearchItemID = nItemID;
        TSearchManagerHost.g_SearchManager.SearchDataList = FLogDataList;

        TVirtualNode? Node = vstLogType.GetFirst();
        if (Node != null && Node.CheckState == TCheckState.csCheckedNormal)
        {
            for (int I = 0; I <= TSearchManagerHost.g_SearchManager.SearchActions.Length - 1; I++)
            {
                TSearchManagerHost.g_SearchManager.SearchActions[I] = true;
            }
        }
        else
        {
            for (int I = 0; I <= TSearchManagerHost.g_SearchManager.SearchActions.Length - 1; I++)
            {
                TSearchManagerHost.g_SearchManager.SearchActions[I] = false;
            }

            Node = vstLogType.GetFirst();
            while (Node != null)
            {
                if (Node.CheckState == TCheckState.csCheckedNormal)
                {
                    TLogTypeNodeData? NodeData = Node.Data as TLogTypeNodeData;
                    if (NodeData != null && NodeData.Category == TActionCategory.acSpecify)
                    {
                        TSearchManagerHost.g_SearchManager.SearchActions[NodeData.Action] = true;
                    }
                }

                Node = vstLogType.GetNext(Node);
            }
        }

        TStringList FileList = new();
        int nDay = HUtil32.GetDayCount(DateTimeEditEnd.Value.Date, DateTimeEditBegin.Value.Date);
        for (int I = 0; I <= nDay; I++)
        {
            DateTime SearchDay = DateTimeEditBegin.Value.Date.AddDays(I);
            int Year = SearchDay.Year, Month = SearchDay.Month, Day = SearchDay.Day;
            string sLogDir = IncludeTrailingBackslash(LogDataShare.sBaseDir) + DelphiRTL.IntToStr(Year) + "-"
                           + LogDataShare.IntToString(Month) + "-" + LogDataShare.IntToString(Day);
            if (Directory.Exists(sLogDir))
            {
                DoSearchFile(sLogDir, FileList);
            }
        }

        FTaskCount = FileList.Count;

        if (FTaskCount == 0)
        {
            StatusPanel(3).Text = "";
            StatusPanel(2).Text = "查询已完成";
            btnStart.Tag = 0;
            btnStart.Text = "开始查询";
            return;
        }

        for (int I = 0; I <= FileList.Count - 1; I++)
        {
            TSearchTask Task = new();
            Task.TaskID = I;
            Task.ShowPanel = StatusPanel(2);
            Task.FileName = FileList[I];

            TSearchManagerHost.g_SearchManager.AddTask(Task);
        }
    }

    /// <summary>LogManage.pas:458 CheckListBoxClickCheck（★ 原文方法体整体注释为空）。</summary>
    public void CheckListBoxClickCheck(object? Sender)
    {
        // (* ... *) 原文如此（LogManage.pas:462-468）
    }

    /// <summary>LogManage.pas:471 FormCreate。</summary>
    public void FormCreate(object? Sender)
    {
        FLogDataList = new TThreadList();

        DateTimeEditBegin.Value = DateTime.Today;
        DateTimeEditEnd.Value = DateTime.Today;

        cbbActionType.Items.Clear();
        for (TLogActorType ActorType = TLogActorType.latNone; ActorType <= TLogActorType.latMonster; ActorType++)
        {
            cbbActionType.Items.Add(LogActorTypeNames.Get(ActorType));
        }
        cbbActionType.SelectedIndex = 0;

        vstLogType.Clear();
        vstLogType.NodeDataSize = 1;   // 原文 SizeOf(TNodeData)

        TVirtualNode ParentNode = vstLogType.AddChild(null);
        ParentNode.CheckType = TCheckType.ctTriStateCheckBox;
        ParentNode.CheckState = TCheckState.csCheckedNormal;
        TLogTypeNodeData NodeData = (TLogTypeNodeData)EnsureNodeData(ParentNode);
        NodeData.Category = TActionCategory.acAll;
        NodeData.Text = "查询所有";

        ParentNode = vstLogType.AddChild(null);
        ParentNode.CheckType = TCheckType.ctTriStateCheckBox;
        ParentNode.CheckState = TCheckState.csCheckedNormal;
        NodeData = (TLogTypeNodeData)EnsureNodeData(ParentNode);
        NodeData.Category = TActionCategory.acParent;
        NodeData.Text = "物品相关";
        for (int I = 0; I <= 25; I++)
        {
            TVirtualNode Node = vstLogType.AddChild(ParentNode);
            Node.CheckType = TCheckType.ctTriStateCheckBox;
            Node.CheckState = TCheckState.csCheckedNormal;

            NodeData = (TLogTypeNodeData)EnsureNodeData(Node);
            NodeData.Category = TActionCategory.acSpecify;
            NodeData.Action = (byte)ActionNames[I].Action;
            NodeData.Text = ActionNames[I].Text;
        }

        ParentNode = vstLogType.AddChild(null);
        ParentNode.CheckType = TCheckType.ctTriStateCheckBox;
        ParentNode.CheckState = TCheckState.csCheckedNormal;
        NodeData = (TLogTypeNodeData)EnsureNodeData(ParentNode);
        NodeData.Category = TActionCategory.acParent;
        NodeData.Text = "普通数据";
        for (int I = 26; I <= 37; I++)
        {
            TVirtualNode Node = vstLogType.AddChild(ParentNode);
            Node.CheckType = TCheckType.ctTriStateCheckBox;
            Node.CheckState = TCheckState.csCheckedNormal;

            NodeData = (TLogTypeNodeData)EnsureNodeData(Node);
            NodeData.Category = TActionCategory.acSpecify;
            NodeData.Action = (byte)ActionNames[I].Action;
            NodeData.Text = ActionNames[I].Text;
        }

        ParentNode = vstLogType.AddChild(null);
        ParentNode.CheckType = TCheckType.ctTriStateCheckBox;
        ParentNode.CheckState = TCheckState.csCheckedNormal;
        NodeData = (TLogTypeNodeData)EnsureNodeData(ParentNode);
        NodeData.Category = TActionCategory.acParent;
        NodeData.Text = "其他动作";
        for (int I = 38; I <= 41; I++)
        {
            TVirtualNode Node = vstLogType.AddChild(ParentNode);
            Node.CheckType = TCheckType.ctTriStateCheckBox;
            Node.CheckState = TCheckState.csCheckedNormal;

            NodeData = (TLogTypeNodeData)EnsureNodeData(Node);
            NodeData.Category = TActionCategory.acSpecify;
            NodeData.Action = (byte)ActionNames[I].Action;
            NodeData.Text = ActionNames[I].Text;
        }

        vstLogType.FullExpand(null);

        Timer.Enabled = true;

        TSearchManagerHost.g_SearchManager.OnTaskComplete = OnTaskComplete;
    }

    private static object EnsureNodeData(TVirtualNode node) => node.Data ??= new TLogTypeNodeData();

    /// <summary>LogManage.pas:563 FormDestroy。</summary>
    public void FormDestroy(object? Sender)
    {
        ClearLogDataList();
        FLogDataList = null!;   // 原文 FLogDataList.Free
    }

    /// <summary>LogManage.pas:569 SetClipboardText（WideString → CF_UNICODETEXT）。</summary>
    public static void SetClipboardText(string Text)
    {
        if (ClipboardHandler != null)
        {
            ClipboardHandler(Text);
            return;
        }
        try
        {
            System.Windows.Forms.Clipboard.SetText(Text);
        }
        catch
        {
            // 剪贴板被占用等 → 原文在 except 中 GlobalFree 后 re-raise；此处保持静默以免拖垮查询窗体
        }
    }

    /// <summary>LogManage.pas:590 pmiCopyClick。</summary>
    public void pmiCopyClick(object? Sender)
    {
        string sText = vstLog.GetText(vstLog.FocusedNode, vstLog.FocusedColumn);
        SetClipboardText(sText);
    }

    /// <summary>LogManage.pas:598 pmiCopyLineClick（Tab 分隔 + sLineBreak，末尾 Copy(S,1,Len-2)）。</summary>
    public void pmiCopyLineClick(object? Sender)
    {
        string S = "";
        if (vstLog.SelectedCount > 0)
        {
            TVirtualNode? Node = vstLog.GetFirstSelected();
            while (Node != null)
            {
                TLogData? LogData = Node.Data as TLogData;
                if (LogData != null)
                {
                    S = S + DelphiRTL.IntToStr(LogData.nIndx) + "\t" +
                        GetActString(LogData.nAct) + "\t" +
                        LogData.sMapName + "\t" +
                        DelphiRTL.IntToStr(LogData.nX) + "\t" +
                        DelphiRTL.IntToStr(LogData.nY) + "\t" +
                        LogData.sObjectName + "\t" +
                        LogActorTypeNames.Get(LogData.ObjectType) + "\t" +
                        LogData.sItemName + "\t" +
                        DelphiRTL.IntToStr(LogData.nItemIndex) + "\t" +
                        LogData.sActObjectName + "\t" +
                        DelphiRTL.IntToStr(LogData.nData1) + "\t" +
                        DelphiRTL.IntToStr(LogData.nData2) + "\t" +
                        LogData.LogDesc + "\t" +
                        LogData.Date.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                }

                Node = vstLog.GetNextSelected(Node);
            }

            if (S.Length > 0)
            {
                S = DelphiRTL.Copy(S, 1, S.Length - 2);
                SetClipboardText(S);
            }
        }
    }

    /// <summary>LogManage.pas:643 pmiExportLineClick。</summary>
    public void pmiExportLineClick(object? Sender)
    {
        if (vstLog.SelectedCount > 0)
        {
            TStringList SL = new();
            TVirtualNode? Node = vstLog.GetFirstSelected();
            while (Node != null)
            {
                TLogData? LogData = Node.Data as TLogData;
                if (LogData != null)
                {
                    SL.Add(FormatLogLine(LogData));
                }

                Node = vstLog.GetNextSelected(Node);
            }

            if (SL.Count > 0)
            {
                string? fileName = AskSaveFileName();
                if (fileName != null)
                {
                    SL.SaveToFile(ChangeFileExt(fileName, ".txt"));
                }
            }
        }
    }

    /// <summary>LogManage.pas:698 pmiExportAllClick。</summary>
    public void pmiExportAllClick(object? Sender)
    {
        if (vstLog.RootNodeCount > 0)
        {
            TStringList SL = new();
            TVirtualNode? Node = vstLog.GetFirst();
            while (Node != null)
            {
                TLogData? LogData = Node.Data as TLogData;
                if (LogData != null)
                {
                    SL.Add(FormatLogLine(LogData));
                }

                Node = vstLog.GetNext(Node);
            }

            if (SL.Count > 0)
            {
                string? fileName = AskSaveFileName();
                if (fileName != null)
                {
                    SL.SaveToFile(ChangeFileExt(fileName, ".txt"));
                }
            }
        }
    }

    private string? AskSaveFileName()
    {
        if (SaveFileProvider != null) return SaveFileProvider();
        return dlgSave.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dlgSave.FileName : null;
    }

    private static string ChangeFileExt(string fileName, string ext)
        => System.IO.Path.ChangeExtension(fileName, ext);

    private static string FormatLogLine(TLogData LogData)
        => DelphiRTL.IntToStr(LogData.nIndx) + "\t" +
           GetActString(LogData.nAct) + "\t" +
           LogData.sMapName + "\t" +
           DelphiRTL.IntToStr(LogData.nX) + "\t" +
           DelphiRTL.IntToStr(LogData.nY) + "\t" +
           LogData.sObjectName + "\t" +
           LogActorTypeNames.Get(LogData.ObjectType) + "\t" +
           LogData.sItemName + "\t" +
           DelphiRTL.IntToStr(LogData.nItemIndex) + "\t" +
           LogData.sActObjectName + "\t" +
           DelphiRTL.IntToStr(LogData.nData1) + "\t" +
           DelphiRTL.IntToStr(LogData.nData2) + "\t" +
           LogData.LogDesc + "\t" +
           LogData.Date.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>LogManage.pas:753 TimerTimer。</summary>
    public void TimerTimer(object? Sender)
    {
        Timer.Enabled = false;
    }

    /// <summary>LogManage.pas:759 ClearLogDataList。</summary>
    public void ClearLogDataList()
    {
        List<object> List = FLogDataList.LockList();
        try
        {
            // 原文 Dispose(pTLogData(...)) —— 托管侧由 GC 回收，仅清空列表
            List.Clear();
        }
        finally
        {
            FLogDataList.UnlockList();
        }
    }

    /// <summary>LogManage.pas:776 OnTaskComplete。</summary>
    public void OnTaskComplete(TPoolTask Task)
    {
        FTaskCount--;
        if (FTaskCount == 0)
        {
            List<object> List = FLogDataList.LockList();
            try
            {
                if (List.Count > 1)
                {
                    QuickSortLogData(List, 0, List.Count - 1);
                }

                vstLog.BeginUpdate();
                try
                {
                    for (int I = 0; I <= List.Count - 1; I++)
                    {
                        TLogData LogData = (TLogData)List[I];
                        LogData.nIndx = I;

                        vstLog.AddChild(null).Data = LogData;
                    }
                }
                finally
                {
                    vstLog.EndUpdate();
                }
            }
            finally
            {
                FLogDataList.UnlockList();
            }

            StatusPanel(3).Text = "";
            StatusPanel(2).Text = "查询已完成";
            btnStart.Tag = 0;
            btnStart.Text = "开始查询";
        }
    }

    /// <summary>LogManage.pas:815 QuickSortLogData（按 nIndx 升序，原文 Hoare 快排 1:1）。</summary>
    public void QuickSortLogData(List<object> List, int L, int R)
    {
        int SCompare(object Item1, object Item2) => ((TLogData)Item1).nIndx - ((TLogData)Item2).nIndx;

        int I, J;
        object P, T;
        do
        {
            I = L;
            J = R;
            P = List[(L + R) >> 1];
            do
            {
                while (SCompare(List[I], P) < 0)
                    I++;
                while (SCompare(List[J], P) > 0)
                    J--;
                if (I <= J)
                {
                    T = List[I];
                    List[I] = List[J];
                    List[J] = T;
                    I++;
                    J--;
                }
            } while (I <= J);
            if (L < J)
                QuickSortLogData(List, L, J);
            L = I;
        } while (I < R);
    }

    /// <summary>LogManage.pas:850 vstLogGetText（14 列）。</summary>
    public void vstLogGetText(TVirtualStringTreeModel Sender, TVirtualNode? Node, int Column, ref string CellText)
    {
        TLogData? LogData = Node?.Data as TLogData;
        if (LogData != null)
        {
            switch (Column)
            {
                case 0: CellText = DelphiRTL.IntToStr(LogData.nIndx); break;
                case 1: CellText = GetActString(LogData.nAct); break;
                case 2: CellText = LogData.sMapName; break;
                case 3: CellText = DelphiRTL.IntToStr(LogData.nX); break;
                case 4: CellText = DelphiRTL.IntToStr(LogData.nY); break;
                case 5: CellText = LogData.sObjectName; break;
                case 6: CellText = LogActorTypeNames.Get(LogData.ObjectType); break;
                case 7: CellText = LogData.sItemName; break;
                case 8: CellText = DelphiRTL.IntToStr(LogData.nItemIndex); break;
                case 9: CellText = LogData.sActObjectName; break;
                case 10: CellText = DelphiRTL.IntToStr(LogData.nData1); break;
                case 11: CellText = DelphiRTL.IntToStr(LogData.nData2); break;
                case 12: CellText = LogData.LogDesc; break;
                case 13: CellText = LogData.Date.ToString("yyyy-MM-dd HH:mm:ss"); break;
            }
        }
    }

    /// <summary>LogManage.pas:880 vstLogDrawText（焦点单元格黄字加粗）。</summary>
    public void vstLogDrawText(TVirtualStringTreeModel Sender, System.Drawing.Color[] fontColor, bool[] bold,
        TVirtualNode? Node, int Column, ref bool DefaultDraw)
    {
        if (Sender.FocusedNode == Node && Node != null && Node.Selected && Sender.FocusedColumn == Column)
        {
            fontColor[0] = System.Drawing.Color.Yellow;
            bold[0] = true;
        }
    }

    /// <summary>LogManage.pas:891 vstLogKeyAction（Ctrl+C 复制焦点单元格）。</summary>
    public void vstLogKeyAction(TVirtualStringTreeModel Sender, ushort CharCode, TShiftState Shift, ref bool DoDefault)
    {
        if (Shift == TShiftState.ssCtrl && CharCode == 'C')
        {
            if (Sender.FocusedNode != null && Sender.FocusedColumn >= 0)
            {
                string S = Sender.GetText(Sender.FocusedNode, Sender.FocusedColumn);
                SetClipboardText(S);
            }
        }
    }

    /// <summary>LogManage.pas:906 PopupMenuPopup。</summary>
    public void PopupMenuPopup(object? Sender)
    {
        pmiCopy.Enabled = vstLog.RootNodeCount > 0 && vstLog.FocusedNode != null && vstLog.FocusedColumn >= 0;
        pmiCopyLine.Enabled = vstLog.RootNodeCount > 0 && vstLog.SelectedCount > 0;
        if (vstLog.FocusedNode != null && vstLog.FocusedColumn >= 0)
        {
            string S = vstLog.GetText(vstLog.FocusedNode, vstLog.FocusedColumn);
            pmiCopy.Text = "复制 \"" + S + "\"";
        }
    }

    /// <summary>LogManage.pas:919 vstLogTypeGetText。</summary>
    public void vstLogTypeGetText(TVirtualStringTreeModel Sender, TVirtualNode? Node, int Column, ref string CellText)
    {
        TLogTypeNodeData? NodeData = Node?.Data as TLogTypeNodeData;
        if (NodeData == null) return;
        CellText = NodeData.Text;
    }

    /// <summary>LogManage.pas:929 vstLogTypeCollapsing（Allowed := False，永不允许折叠）。</summary>
    public void vstLogTypeCollapsing(TVirtualStringTreeModel Sender, TVirtualNode Node, ref bool Allowed)
    {
        Allowed = false;
    }

    /// <summary>LogManage.pas:935 vstLogTypeDrawText（acAll 蓝字加粗 / acParent 加粗）。</summary>
    public void vstLogTypeDrawText(TVirtualStringTreeModel Sender, TVirtualNode? Node,
        System.Drawing.Color[] fontColor, bool[] bold)
    {
        TLogTypeNodeData? NodeData = Node?.Data as TLogTypeNodeData;
        if (NodeData == null) return;
        switch (NodeData.Category)
        {
            case TActionCategory.acAll:
                bold[0] = true;
                fontColor[0] = System.Drawing.Color.Blue;
                break;
            case TActionCategory.acParent:
                bold[0] = true;
                break;
        }
    }

    /// <summary>LogManage.pas:955 vstLogBeforeItemErase（奇数行 $00FFFBF7）。</summary>
    public void vstLogBeforeItemErase(TVirtualStringTreeModel Sender, TVirtualNode Node,
        ref int ItemColor, ref TItemEraseAction EraseAction)
    {
        if (Node.Index % 2 != 0)
        {
            ItemColor = 0x00FFFBF7;
            EraseAction = TItemEraseAction.eaColor;
        }
    }

    /// <summary>LogManage.pas:966 vstLogHeaderClick（列 0 → NoColumn 升序；同列切换升降）。</summary>
    public void vstLogHeaderClick(TVTHeader Sender, TVTHeaderHitInfo HitInfo)
    {
        if (HitInfo.Button == TMouseButton.mbLeft)
        {
            if (HitInfo.Column == 0)
            {
                Sender.SortColumn = TVTHeader.NoColumn;
                Sender.SortTree(0, TSortDirection.sdAscending, false);
            }
            else
            {
                if (Sender.SortColumn != HitInfo.Column)
                {
                    Sender.SortColumn = HitInfo.Column;
                    Sender.SortDirection = TSortDirection.sdAscending;
                }
                else
                {
                    if (Sender.SortDirection == TSortDirection.sdAscending)
                        Sender.SortDirection = TSortDirection.sdDescending;
                    else
                        Sender.SortDirection = TSortDirection.sdAscending;
                }

                Sender.SortTree(Sender.SortColumn, Sender.SortDirection, false);
            }
        }
    }

    /// <summary>LogManage.pas:999 vstLogCompareNodes（14 列比较，1:1）。</summary>
    public void vstLogCompareNodes(TVirtualStringTreeModel Sender, TVirtualNode? Node1, TVirtualNode? Node2,
        int Column, ref int Result)
    {
        TLogData Data1 = (TLogData)Node1!.Data!;
        TLogData Data2 = (TLogData)Node2!.Data!;

        switch (Column)
        {
            case 0: Result = Data1.nIndx - Data2.nIndx; break;
            case 1: Result = (int)Data1.nAct - (int)Data2.nAct; break;
            case 2: Result = string.Compare(Data1.sMapName, Data2.sMapName, StringComparison.OrdinalIgnoreCase); break;
            case 3: Result = Data1.nX - Data2.nX; break;
            case 4: Result = Data1.nY - Data2.nY; break;
            case 5: Result = string.Compare(Data1.sObjectName, Data2.sObjectName, StringComparison.OrdinalIgnoreCase); break;
            case 6: Result = (int)Data1.ObjectType - (int)Data2.ObjectType; break;
            case 7: Result = string.Compare(Data1.sItemName, Data2.sItemName, StringComparison.OrdinalIgnoreCase); break;
            case 8: Result = Data1.nItemIndex - Data2.nItemIndex; break;
            case 9: Result = string.Compare(Data1.sActObjectName, Data2.sActObjectName, StringComparison.OrdinalIgnoreCase); break;
            case 10: Result = Data1.nData1 - Data2.nData1; break;
            case 11: Result = Data1.nData2 - Data2.nData2; break;
            case 12: Result = string.Compare(Data1.LogDesc, Data2.LogDesc, StringComparison.OrdinalIgnoreCase); break;
            case 13: Result = (int)Math.Round((Data1.Date - Data2.Date).TotalDays); break;
        }
    }

    /// <summary>SortTree 接缝实现（对应 Treeview.SortTree → 逐对调用 vstLogCompareNodes）。</summary>
    private void SortLogTree(int column, TSortDirection direction)
    {
        vstLog.SortRoots((a, b) =>
        {
            int r = 0;
            vstLogCompareNodes(vstLog, a, b, column, ref r);
            return r;
        }, direction);
    }

    /// <summary>LogManage.pas:1026 WMSYSCommand（SC_MINIMIZE → Visible := False，且不调用 inherited）。</summary>
    public void WMSYSCommand(ref TWMSYSCommand Msg)
    {
        if (Msg.CmdType == SC_MINIMIZE)
        {
            Visible = false;
            //inherited;    // 原文如此（LogManage.pas:1031 注释）
        }
        else
        {
            // inherited：由 WndProc 落到 base.WndProc 完成
        }
    }

    protected override void WndProc(ref System.Windows.Forms.Message m)
    {
        if (m.Msg == 0x0112 /* WM_SYSCOMMAND */)
        {
            var msg = new TWMSYSCommand { CmdType = unchecked((uint)m.WParam.ToInt64()) };
            WMSYSCommand(ref msg);
            if (msg.CmdType == SC_MINIMIZE) return;   // 原文 minimize 分支不调用 inherited
        }
        base.WndProc(ref m);
    }
}

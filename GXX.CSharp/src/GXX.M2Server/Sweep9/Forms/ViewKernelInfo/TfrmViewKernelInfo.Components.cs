// ============================================================================
// ViewKernelInfo.dfm（文本，500 行）控件树 1:1 实例化（64 个控件 + 窗体自身）
// ============================================================================

namespace GXX.M2Server.Sweep9.Forms;

public sealed partial class TfrmViewKernelInfo
{
    /// <summary>DFM 两张表的 `Options` 集合（3 行，逐字来自 DFM）。</summary>
    private static readonly string[] DfmGridOptions =
    {
        "goFixedVertLine", "goFixedHorzLine", "goVertLine", "goHorzLine", "goRangeSelect", "goRowSelect",
    };

    /// <summary>DFM 控件树 1:1 实例化（属性逐条取自 ViewKernelInfo.dfm）。</summary>
    private void InitializeComponents()
    {
        // ---- DFM :1-17 窗体自身 ----
        Name = "frmViewKernelInfo";
        Text = "内核数据查看";                                            // Caption
        Left = 950;
        Top = 517;
        ClientSize = new System.Drawing.Size(474, 226);                   // ClientWidth/ClientHeight
        // BorderIcons = [biSystemMenu, biMinimize] ⇒ 无最大化、有最小化
        MaximizeBox = false;
        MinimizeBox = true;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle; // BorderStyle = bsSingle
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; // Position = poMainFormCenter
        Font = new System.Drawing.Font("宋体", 9F);                        // Font.Height = -12 / Name = 宋体
        // DFM :16 OnCreate = FormCreate ⇒ WinForms 等价 Load（绑定数对账：DFM 2 ↔ 托管 2）
        Load += (_, _) => FormCreate(this);

        // ---- DFM :18-24 PageControl1（ActivePage = TabSheet1 ⇒ SelectedIndex = 0） ----
        PageControl1 = new System.Windows.Forms.TabControl
        {
            Name = "PageControl1",
            Left = 8, Top = 8, Width = 457, Height = 209,
            TabIndex = 0,
        };
        TabSheet1 = MakePage("TabSheet1", "游戏数据");
        TabSheet2 = MakePage("TabSheet2", "彩票数据", 1);
        TabSheet3 = MakePage("TabSheet3", "全局变量", 2);
        TabSheet5 = MakePage("TabSheet5", "工作线程", 4);
        TabSheet6 = MakePage("TabSheet6", "内存池", 5);
        PageControl1.TabPages.AddRange(new[] { TabSheet1, TabSheet2, TabSheet3, TabSheet5, TabSheet6 });
        PageControl1.SelectedIndex = 0;
        Controls.Add(PageControl1);

        // =================================================================
        // TabSheet1 '游戏数据'（DFM :25-133）
        // =================================================================
        GroupBox1 = MakeGroupBox("GroupBox1", "游戏数据库", 8, 4, 153, 121, 0);
        Label1 = MakeLabel("Label1", "读取请求次数:", 8, 20, 78);
        Label2 = MakeLabel("Label2", "读取失败次数:", 8, 44, 78);
        Label3 = MakeLabel("Label3", "保存请求次数:", 8, 68, 78);
        Label4 = MakeLabel("Label4", "请求标识数字:", 8, 92, 78);
        EditLoadHumanDBCount = MakeReadOnlyEdit("EditLoadHumanDBCount", 88, 16, 57, 0);
        EditLoadHumanDBErrorCoun = MakeReadOnlyEdit("EditLoadHumanDBErrorCoun", 88, 40, 57, 1);
        EditSaveHumanDBCount = MakeReadOnlyEdit("EditSaveHumanDBCount", 88, 64, 57, 2);
        EditHumanDBQueryID = MakeReadOnlyEdit("EditHumanDBQueryID", 88, 88, 57, 3);
        GroupBox1.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Label1, Label2, Label3, Label4,
            EditLoadHumanDBCount, EditLoadHumanDBErrorCoun, EditSaveHumanDBCount, EditHumanDBQueryID,
        });
        TabSheet1.Controls.Add(GroupBox1);

        GroupBox4 = MakeGroupBox("GroupBox4", "物品系列号", 168, 4, 177, 69, 1);
        Label7 = MakeLabel("Label7", "怪物掉落物品:", 8, 20, 78);
        Label8 = MakeLabel("Label8", "命令制造物品:", 8, 44, 78);
        EditItemNumber = MakeReadOnlyEdit("EditItemNumber", 88, 16, 73, 0);
        EditItemNumberEx = MakeReadOnlyEdit("EditItemNumberEx", 88, 40, 73, 1);
        GroupBox4.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Label7, Label8, EditItemNumber, EditItemNumberEx,
        });
        TabSheet1.Controls.Add(GroupBox4);

        // =================================================================
        // TabSheet2 '彩票数据'（DFM :134-273）
        // =================================================================
        GroupBox2 = MakeGroupBox("GroupBox2", "中奖数量", 8, 4, 153, 77, 0);
        Label5 = MakeLabel("Label5", "中奖总数:", 8, 20, 54);
        Label6 = MakeLabel("Label6", "未中奖数:", 8, 44, 54);
        EditWinLotteryCount = MakeReadOnlyEdit("EditWinLotteryCount", 88, 16, 57, 0);
        EditNoWinLotteryCount = MakeReadOnlyEdit("EditNoWinLotteryCount", 88, 40, 57, 1);
        GroupBox2.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Label5, Label6, EditWinLotteryCount, EditNoWinLotteryCount,
        });
        TabSheet2.Controls.Add(GroupBox2);

        GroupBox3 = MakeGroupBox("GroupBox3", "中奖比例", 168, 4, 129, 165, 1);
        Label9 = MakeLabel("Label9", "一等奖:", 8, 20, 42);
        Label10 = MakeLabel("Label10", "二等奖:", 8, 44, 42);
        Label11 = MakeLabel("Label11", "三等奖:", 8, 68, 42);
        Label12 = MakeLabel("Label12", "四等奖:", 8, 92, 42);
        Label13 = MakeLabel("Label13", "五等奖:", 8, 116, 42);
        Label14 = MakeLabel("Label14", "六等奖:", 8, 140, 42);
        EditWinLotteryLevel1 = MakeReadOnlyEdit("EditWinLotteryLevel1", 56, 16, 57, 0);
        EditWinLotteryLevel2 = MakeReadOnlyEdit("EditWinLotteryLevel2", 56, 40, 57, 1);
        EditWinLotteryLevel3 = MakeReadOnlyEdit("EditWinLotteryLevel3", 56, 64, 57, 2);
        EditWinLotteryLevel4 = MakeReadOnlyEdit("EditWinLotteryLevel4", 56, 88, 57, 3);
        EditWinLotteryLevel5 = MakeReadOnlyEdit("EditWinLotteryLevel5", 56, 112, 57, 4);
        EditWinLotteryLevel6 = MakeReadOnlyEdit("EditWinLotteryLevel6", 56, 136, 57, 5);
        GroupBox3.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Label9, Label10, Label11, Label12, Label13, Label14,
            EditWinLotteryLevel1, EditWinLotteryLevel2, EditWinLotteryLevel3,
            EditWinLotteryLevel4, EditWinLotteryLevel5, EditWinLotteryLevel6,
        });
        TabSheet2.Controls.Add(GroupBox3);

        // =================================================================
        // TabSheet3 '全局变量'（DFM :274-435）
        // =================================================================
        GroupBox5 = MakeGroupBox("GroupBox5", "全局变量状态", 8, 4, 425, 165, 0);
        Label15 = MakeLabel("Label15", "变量一:", 8, 20, 42);
        Label16 = MakeLabel("Label16", "变量二:", 8, 44, 42);
        Label17 = MakeLabel("Label17", "变量三:", 8, 68, 42);
        Label18 = MakeLabel("Label18", "变量四:", 8, 92, 42);
        Label19 = MakeLabel("Label19", "变量五:", 8, 116, 42);
        Label20 = MakeLabel("Label20", "变量六:", 8, 140, 42);
        Label21 = MakeLabel("Label21", "变量十:", 120, 92, 42);
        Label22 = MakeLabel("Label22", "变量七:", 120, 20, 42);
        Label23 = MakeLabel("Label23", "变量八:", 120, 44, 42);
        Label24 = MakeLabel("Label24", "变量九:", 120, 68, 42);
        EditGlobalVal1 = MakeReadOnlyEdit("EditGlobalVal1", 56, 16, 57, 0);
        EditGlobalVal2 = MakeReadOnlyEdit("EditGlobalVal2", 56, 40, 57, 1);
        EditGlobalVal3 = MakeReadOnlyEdit("EditGlobalVal3", 56, 64, 57, 2);
        EditGlobalVal4 = MakeReadOnlyEdit("EditGlobalVal4", 56, 88, 57, 3);
        EditGlobalVal5 = MakeReadOnlyEdit("EditGlobalVal5", 56, 112, 57, 4);
        EditGlobalVal6 = MakeReadOnlyEdit("EditGlobalVal6", 56, 136, 57, 5);
        EditGlobalVal7 = MakeReadOnlyEdit("EditGlobalVal7", 168, 16, 57, 6);
        EditGlobalVal8 = MakeReadOnlyEdit("EditGlobalVal8", 168, 40, 57, 7);
        EditGlobalVal9 = MakeReadOnlyEdit("EditGlobalVal9", 168, 64, 57, 8);
        EditGlobalVal10 = MakeReadOnlyEdit("EditGlobalVal10", 168, 88, 57, 9);
        GroupBox5.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            Label15, Label16, Label17, Label18, Label19, Label20,
            Label21, Label22, Label23, Label24,
            EditGlobalVal1, EditGlobalVal2, EditGlobalVal3, EditGlobalVal4, EditGlobalVal5,
            EditGlobalVal6, EditGlobalVal7, EditGlobalVal8, EditGlobalVal9, EditGlobalVal10,
        });
        TabSheet3.Controls.Add(GroupBox5);

        // =================================================================
        // TabSheet5 '工作线程'（DFM :436-465）
        // =================================================================
        GroupBox7 = MakeGroupBox("GroupBox7", "线程状态", 8, 4, 425, 165, 0);
        GridThread = MakeGrid("GridThread", colCount: 6, defaultRowHeight: 18, tabOrder: 0,
            colWidths: new[] { 30, 80, 51, 90, 64, 64 });
        GroupBox7.Controls.Add(GridThread);
        TabSheet5.Controls.Add(GroupBox7);

        // =================================================================
        // TabSheet6 '内存池'（DFM :466-492）
        // =================================================================
        GroupBox8 = MakeGroupBox("GroupBox8", "内存池", 8, 4, 425, 165, 0);
        GridMemory = MakeGrid("GridMemory", colCount: 3, defaultRowHeight: 18, tabOrder: 0,
            colWidths: new[] { 126, 98, 86 });
        GroupBox8.Controls.Add(GridMemory);
        TabSheet6.Controls.Add(GroupBox8);

        // =================================================================
        // DFM :494-499 Timer（TTimer 组件，不在 Controls 树里）
        // =================================================================
        Timer = new System.Windows.Forms.Timer
        {
            Interval = 1000,          // ⚠ DFM 未写 Interval ⇒ Delphi 默认 1000 ms（见测试）
            Enabled = false,          // DFM Enabled = False
        };
        Timer.Tick += (_, _) => TimerTimer(Timer);
    }

    // ------------------------------------------------------------------
    // 构造辅助
    // ------------------------------------------------------------------

    private static System.Windows.Forms.TabPage MakePage(string name, string caption, int imageIndex = -1)
    {
        var page = new System.Windows.Forms.TabPage { Name = name, Text = caption, UseVisualStyleBackColor = false };
        if (imageIndex >= 0) page.ImageIndex = imageIndex;
        return page;
    }

    private static System.Windows.Forms.GroupBox MakeGroupBox(string name, string caption,
        int left, int top, int width, int height, int tabOrder)
        => new()
        {
            Name = name,
            Text = caption,
            Left = left, Top = top, Width = width, Height = height,
            TabIndex = tabOrder,
        };

    private static System.Windows.Forms.Label MakeLabel(string name, string caption, int left, int top, int width)
        => new()
        {
            Name = name,
            Text = caption,
            Left = left, Top = top, Width = width, Height = 12,
            AutoSize = false,
        };

    private static System.Windows.Forms.TextBox MakeReadOnlyEdit(string name, int left, int top, int width, int tabOrder)
        => new()
        {
            Name = name,
            Left = left, Top = top, Width = width, Height = 20,
            AutoSize = false,                    // 否则 WinForms 会按字体行高（21）覆盖 DFM 的 Height = 20
            ReadOnly = true,                     // DFM 24 个 TEdit 全部 ReadOnly = True
            TabIndex = tabOrder,
        };

    private static Sweep9FormsStringGrid MakeGrid(string name, int colCount, int defaultRowHeight,
        int tabOrder, int[] colWidths)
    {
        var grid = new Sweep9FormsStringGrid
        {
            Name = name,
            Left = 8, Top = 16, Width = 409, Height = 137,
            TabIndex = tabOrder,
            DfmColCount = colCount,
            DfmRowCount = 5,                     // DFM 未写 RowCount ⇒ Delphi 默认 5
            DfmFixedCols = 0,                    // DFM FixedCols = 0
            DfmFixedRows = 1,                    // DFM 未写 ⇒ 默认 1
            DfmDefaultRowHeight = defaultRowHeight,
            DfmDefaultColWidth = 64,
        };
        grid.DfmOptions.AddRange(DfmGridOptions);
        grid.DfmColWidths.AddRange(colWidths);
        grid.ApplyDfmCounts();
        return grid;
    }
}

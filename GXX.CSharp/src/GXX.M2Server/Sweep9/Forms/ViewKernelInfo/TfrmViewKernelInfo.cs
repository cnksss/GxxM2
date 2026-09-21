// ============================================================================
// 源单元：Source/M2Engine/Forms/ViewKernelInfo.pas（198 行，GBK）
// 同源 DFM：Source/M2Engine/Forms/ViewKernelInfo.dfm（文本，500 行）
//   DFM 控件 **64** 个（含 1 个 `TTimer` 组件）；事件绑定 **2** 个
//   （根节点 `OnCreate = FormCreate` + `Timer.OnTimer = TimerTimer`）。
//
// 方法清单（.pas 行号）：
//   FormCreate  :94-134       Open :136-141       TimerTimer :143-195
//   单元级全局 frmViewKernelInfo :85
//
// ★★ 原文缺陷（最高价值，逐字保留 + 逐字段差异断言锁定；偏离 D-P9-01）：
//   `:99 Config := @g_Config;` 之后，
//   `:116 ThreadInfo := @Config.UserEngineThread;` —— `UserEngineThread` 是**指针字段**，
//   `@` 取的是**该指针字段的地址**（作者显然想写 `Config.UserEngineThread`，
//   但那也没 `New` ⇒ 仍是 nil 解引用；两种写法都是 bug，而这一种**破坏 g_Config 本身**）。
//   随后 `:117-121` 的 5 句 `ThreadInfo.xxx := 0` 按 `TThreadInfo` 的字段偏移
//   往那块内存写 4 字节 0 ⇒ **`FormCreate` 跑一次就把 `g_Config` 打坏**：
//     UserEngineThread / IDSocketThread / DBSOcketThread → nil
//     nUserSellOffCount / nUserSellOffTax / nSkill69CD / nSkill69AddTime /
//     nSkill69AddRange → 0；boSkill69SameLevel → False
//   三处（:116 / :122 / :128）**逐块**展开，托管侧用 `TConfigThreadRegion`
//   的 dword 段内存视图精确复刻（见 Sweep9FormsKernelSeams.cs 的排版表）。
//   ⇒ 打开"内核数据查看"窗口即破坏引擎配置，是本单元最重要的原文缺陷。
//
// 其它原文要点：
//   · `:100-110` / `:138/:140` `Timer.Enabled := True/False` 包住 `ShowModal`（:139）。
//   · `:185` `Format('%d/%d/%d', [RunTick, MinRunTick, MaxRunTick])`。
//   · `:186-189` 每次用**本次**读数更新 `MaxThreadCPUUsage`（峰值），再显示 `当前/峰值`。
//   · `:181` 第 0 列写的是 `IntToStr(I)`（**序号从 0 起**），行号才是 `I + 1`。
//   · `:192-194` 只把 GridMemory 的第 1 列第 2/3/4 行清空（不动第 0 列也不动第 1 行）。
// ============================================================================

using GXX.Core.Rtl;

namespace GXX.M2Server.Sweep9.Forms;

/// <summary>
/// 原文 `ViewKernelInfo.pas:10-82 TfrmViewKernelInfo = class(TForm)` 1:1。
/// </summary>
public sealed partial class TfrmViewKernelInfo : System.Windows.Forms.Form
{
    // ==================================================================
    // DFM 控件（64 个；含 `Timer` 组件）
    // ==================================================================

    /// <summary>DFM `Timer: TTimer`（`Enabled = False`，`OnTimer = TimerTimer`，Left=288 Top=192）。</summary>
    public System.Windows.Forms.Timer Timer = null!;

    /// <summary>DFM `PageControl1: TPageControl`（8,8 457×209，`ActivePage = TabSheet1`）。</summary>
    public System.Windows.Forms.TabControl PageControl1 = null!;

    /// <summary>DFM `TabSheet1`（`Caption = '游戏数据'`）。</summary>
    public System.Windows.Forms.TabPage TabSheet1 = null!;
    /// <summary>DFM `TabSheet2`（`Caption = '彩票数据'`，`ImageIndex = 1`）。</summary>
    public System.Windows.Forms.TabPage TabSheet2 = null!;
    /// <summary>DFM `TabSheet3`（`Caption = '全局变量'`，`ImageIndex = 2`）。</summary>
    public System.Windows.Forms.TabPage TabSheet3 = null!;
    /// <summary>DFM `TabSheet5`（`Caption = '工作线程'`，`ImageIndex = 4`）。</summary>
    public System.Windows.Forms.TabPage TabSheet5 = null!;
    /// <summary>DFM `TabSheet6`（`Caption = '内存池'`，`ImageIndex = 5`）。</summary>
    public System.Windows.Forms.TabPage TabSheet6 = null!;

    /// <summary>DFM `GroupBox1`（`Caption = '游戏数据库'`）。</summary>
    public System.Windows.Forms.GroupBox GroupBox1 = null!;
    /// <summary>DFM `GroupBox2`（`Caption = '中奖数量'`）。</summary>
    public System.Windows.Forms.GroupBox GroupBox2 = null!;
    /// <summary>DFM `GroupBox3`（`Caption = '中奖比例'`）。</summary>
    public System.Windows.Forms.GroupBox GroupBox3 = null!;
    /// <summary>DFM `GroupBox4`（`Caption = '物品系列号'`）。</summary>
    public System.Windows.Forms.GroupBox GroupBox4 = null!;
    /// <summary>DFM `GroupBox5`（`Caption = '全局变量状态'`）。</summary>
    public System.Windows.Forms.GroupBox GroupBox5 = null!;
    /// <summary>DFM `GroupBox7`（`Caption = '线程状态'`）。</summary>
    public System.Windows.Forms.GroupBox GroupBox7 = null!;
    /// <summary>DFM `GroupBox8`（`Caption = '内存池'`）。</summary>
    public System.Windows.Forms.GroupBox GroupBox8 = null!;

    // ---- Label1..Label24（DFM 声明顺序即 .pas:14-68 顺序） ----
    /// <summary>DFM `Label1`（'读取请求次数:'）。</summary>
    public System.Windows.Forms.Label Label1 = null!;
    /// <summary>DFM `Label2`（'读取失败次数:'）。</summary>
    public System.Windows.Forms.Label Label2 = null!;
    /// <summary>DFM `Label3`（'保存请求次数:'）。</summary>
    public System.Windows.Forms.Label Label3 = null!;
    /// <summary>DFM `Label4`（'请求标识数字:'）。</summary>
    public System.Windows.Forms.Label Label4 = null!;
    /// <summary>DFM `Label5`（'中奖总数:'）。</summary>
    public System.Windows.Forms.Label Label5 = null!;
    /// <summary>DFM `Label6`（'未中奖数:'）。</summary>
    public System.Windows.Forms.Label Label6 = null!;
    /// <summary>DFM `Label7`（'怪物掉落物品:'）。</summary>
    public System.Windows.Forms.Label Label7 = null!;
    /// <summary>DFM `Label8`（'命令制造物品:'）。</summary>
    public System.Windows.Forms.Label Label8 = null!;
    /// <summary>DFM `Label9`（'一等奖:'）。</summary>
    public System.Windows.Forms.Label Label9 = null!;
    /// <summary>DFM `Label10`（'二等奖:'）。</summary>
    public System.Windows.Forms.Label Label10 = null!;
    /// <summary>DFM `Label11`（'三等奖:'）。</summary>
    public System.Windows.Forms.Label Label11 = null!;
    /// <summary>DFM `Label12`（'四等奖:'）。</summary>
    public System.Windows.Forms.Label Label12 = null!;
    /// <summary>DFM `Label13`（'五等奖:'）。</summary>
    public System.Windows.Forms.Label Label13 = null!;
    /// <summary>DFM `Label14`（'六等奖:'）。</summary>
    public System.Windows.Forms.Label Label14 = null!;
    /// <summary>DFM `Label15`（'变量一:'）。</summary>
    public System.Windows.Forms.Label Label15 = null!;
    /// <summary>DFM `Label16`（'变量二:'）。</summary>
    public System.Windows.Forms.Label Label16 = null!;
    /// <summary>DFM `Label17`（'变量三:'）。</summary>
    public System.Windows.Forms.Label Label17 = null!;
    /// <summary>DFM `Label18`（'变量四:'）。</summary>
    public System.Windows.Forms.Label Label18 = null!;
    /// <summary>DFM `Label19`（'变量五:'）。</summary>
    public System.Windows.Forms.Label Label19 = null!;
    /// <summary>DFM `Label20`（'变量六:'）。</summary>
    public System.Windows.Forms.Label Label20 = null!;
    /// <summary>DFM `Label21`（'变量十:'，120,92）。</summary>
    public System.Windows.Forms.Label Label21 = null!;
    /// <summary>DFM `Label22`（'变量七:'，120,20）。</summary>
    public System.Windows.Forms.Label Label22 = null!;
    /// <summary>DFM `Label23`（'变量八:'，120,44）。</summary>
    public System.Windows.Forms.Label Label23 = null!;
    /// <summary>DFM `Label24`（'变量九:'，120,68）。</summary>
    public System.Windows.Forms.Label Label24 = null!;

    // ---- 24 个 TEdit（全部 ReadOnly = True） ----
    /// <summary>DFM `EditLoadHumanDBCount: TEdit`（`ReadOnly = True`）。</summary>
    public System.Windows.Forms.TextBox EditLoadHumanDBCount = null!;
    /// <summary>DFM `EditLoadHumanDBErrorCoun: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditLoadHumanDBErrorCoun = null!;
    /// <summary>DFM `EditSaveHumanDBCount: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditSaveHumanDBCount = null!;
    /// <summary>DFM `EditHumanDBQueryID: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditHumanDBQueryID = null!;
    /// <summary>DFM `EditItemNumber: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditItemNumber = null!;
    /// <summary>DFM `EditItemNumberEx: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditItemNumberEx = null!;
    /// <summary>DFM `EditWinLotteryCount: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditWinLotteryCount = null!;
    /// <summary>DFM `EditNoWinLotteryCount: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditNoWinLotteryCount = null!;
    /// <summary>DFM `EditWinLotteryLevel1: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditWinLotteryLevel1 = null!;
    /// <summary>DFM `EditWinLotteryLevel2: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditWinLotteryLevel2 = null!;
    /// <summary>DFM `EditWinLotteryLevel3: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditWinLotteryLevel3 = null!;
    /// <summary>DFM `EditWinLotteryLevel4: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditWinLotteryLevel4 = null!;
    /// <summary>DFM `EditWinLotteryLevel5: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditWinLotteryLevel5 = null!;
    /// <summary>DFM `EditWinLotteryLevel6: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditWinLotteryLevel6 = null!;
    /// <summary>DFM `EditGlobalVal1: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal1 = null!;
    /// <summary>DFM `EditGlobalVal2: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal2 = null!;
    /// <summary>DFM `EditGlobalVal3: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal3 = null!;
    /// <summary>DFM `EditGlobalVal4: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal4 = null!;
    /// <summary>DFM `EditGlobalVal5: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal5 = null!;
    /// <summary>DFM `EditGlobalVal6: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal6 = null!;
    /// <summary>DFM `EditGlobalVal7: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal7 = null!;
    /// <summary>DFM `EditGlobalVal8: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal8 = null!;
    /// <summary>DFM `EditGlobalVal9: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal9 = null!;
    /// <summary>DFM `EditGlobalVal10: TEdit`。</summary>
    public System.Windows.Forms.TextBox EditGlobalVal10 = null!;

    /// <summary>DFM `GridThread: TStringGrid`（ColCount=6，DefaultRowHeight=18，FixedCols=0）。</summary>
    public Sweep9FormsStringGrid GridThread = null!;
    /// <summary>DFM `GridMemory: TStringGrid`（ColCount=3，DefaultRowHeight=18，FixedCols=0）。</summary>
    public Sweep9FormsStringGrid GridMemory = null!;

    // ==================================================================
    // 接缝
    // ==================================================================

    /// <summary>
    /// 接缝：`g_Config`（`M2Share.pas`）的本窗体读取面。
    /// **未接线即抛**（§25.2）：本窗体显示的 0 恰好是合法读数，静默填 0 会变成"一整屏假数据"。
    /// </summary>
    public Sweep9FormsKernelConfig? KernelConfig;

    /// <summary>
    /// 接缝：`g_M2RunThreadMgr`（`M2Threads.pas:58`，`Count` + `Items[I]`）。
    /// **未接线即抛**（§25.2）。返回 `null` 视为空表（原文 `Count = 0`）。
    /// </summary>
    public Func<IReadOnlyList<Sweep9FormsRunThreadInfo>?>? RunThreadMgrHandler;

    /// <summary>`g_Config` 读取面；未接线即抛。</summary>
    private Sweep9FormsKernelConfig Cfg => KernelConfig ?? throw Sweep9FormsKit.NotWired(
        nameof(KernelConfig), "M2Share.pas g_Config（nLoadDBCount/nWinLottery*/GlobalVal[0..999] 等）");

    // ==================================================================
    // 构造
    // ==================================================================

    /// <summary>构造 + 装载 DFM 控件树（`OnCreate = FormCreate` 由 `Load` 触发）。</summary>
    public TfrmViewKernelInfo()
    {
        InitializeComponents();
    }

    // ==================================================================
    // :94-134 FormCreate
    // ==================================================================

    /// <summary>
    /// 原文 `:94-134 procedure TfrmViewKernelInfo.FormCreate(Sender: TObject)`。
    /// <para>★★ 本方法**包含原文最严重的缺陷**：`:116-133` 把 `g_Config` 的三个线程指针
    /// 及其后 5 个配置字段写坏（逐字段后果见测试 `FormCreate_ClobbersConfigRegion_*`）。</para>
    /// </summary>
    public void FormCreate(object? Sender = null)
    {
        // 原文 var Config: pTConfig; ThreadInfo: pTThreadInfo;
        // 原文 :99 Config := @g_Config;
        Sweep9FormsKernelConfig Config = Cfg;

        // 原文 :100-105 GridThread 表头（6 列）
        GridThread.Cells[0, 0] = "序号";
        GridThread.Cells[1, 0] = "描述";
        GridThread.Cells[2, 0] = "句柄";
        GridThread.Cells[3, 0] = "线程ID";
        GridThread.Cells[4, 0] = "运行时间";
        GridThread.Cells[5, 0] = "CPU占用";

        // 原文 :107-109 GridMemory 表头（3 列）
        GridMemory.Cells[0, 0] = "名称";
        GridMemory.Cells[1, 0] = "数量";
        GridMemory.Cells[2, 0] = "大小";

        // 原文 :111-114 GridMemory 第 0 列第 1..4 行
        GridMemory.Cells[0, 1] = "UserItem";
        GridMemory.Cells[0, 2] = "MapItem";
        GridMemory.Cells[0, 3] = "BaseObject";
        GridMemory.Cells[0, 4] = "MapEvent";

        // 原文 :116-121 ThreadInfo := @Config.UserEngineThread; + 5 句清零
        //   ★ 基址是**指针字段本身**的地址 ⇒ 破坏 g_Config（D-P9-01）。
        Config.ThreadRegion.WriteThreadInfoFieldsAt(0);      // @Config.UserEngineThread

        // 原文 :122-127 ThreadInfo := @Config.IDSocketThread; + 同一个 5 句块
        Config.ThreadRegion.WriteThreadInfoFieldsAt(1);      // @Config.IDSocketThread

        // 原文 :128-133 ThreadInfo := @Config.DBSOcketThread; + 同一个 5 句块
        Config.ThreadRegion.WriteThreadInfoFieldsAt(2);      // @Config.DBSOcketThread
    }

    // ==================================================================
    // :136-141 Open
    // ==================================================================

    /// <summary>
    /// 原文 `:136-141 procedure TfrmViewKernelInfo.Open;`。
    /// <para>`Timer.Enabled := True; ShowModal; Timer.Enabled := False;`</para>
    /// </summary>
    public void Open()
    {
        Timer.Enabled = true;                                     // 原文 :138
        Sweep9FormsMessageBoxSeam.ShowModal(this);                // 原文 :139（返回值被丢弃）
        Timer.Enabled = false;                                    // 原文 :140
    }

    // ==================================================================
    // :143-195 TimerTimer
    // ==================================================================

    /// <summary>
    /// 原文 `:143-195 procedure TfrmViewKernelInfo.TimerTimer(Sender: TObject)`。
    /// <para>原文 `Sender` 未被使用（保留形参以对齐签名）。</para>
    /// </summary>
    public void TimerTimer(object? Sender = null)
    {
        // 原文 var I: Integer; Thread: TM2RunThread; ThreadCPUUsage: Integer;
        int I;
        Sweep9FormsRunThreadInfo Thread;
        int ThreadCPUUsage;

        Sweep9FormsKernelConfig Config = Cfg;                      // 原文 g_Config（每 tick 直读全局）

        // ---- 原文 :149-152 数据库计数 ----
        EditLoadHumanDBCount.Text = DelphiRTL.IntToStr(Config.nLoadDBCount);
        EditLoadHumanDBErrorCoun.Text = DelphiRTL.IntToStr(Config.nLoadDBErrorCount);
        EditSaveHumanDBCount.Text = DelphiRTL.IntToStr(Config.nSaveDBCount);
        EditHumanDBQueryID.Text = DelphiRTL.IntToStr(Config.nDBQueryID);

        // ---- 原文 :154-155 物品系列号 ----
        EditItemNumber.Text = DelphiRTL.IntToStr(Config.nItemNumber);
        EditItemNumberEx.Text = DelphiRTL.IntToStr(Config.nItemNumberEx);

        // ---- 原文 :157-164 彩票 ----
        EditWinLotteryCount.Text = DelphiRTL.IntToStr(Config.nWinLotteryCount);
        EditNoWinLotteryCount.Text = DelphiRTL.IntToStr(Config.nNoWinLotteryCount);
        EditWinLotteryLevel1.Text = DelphiRTL.IntToStr(Config.nWinLotteryLevel1);
        EditWinLotteryLevel2.Text = DelphiRTL.IntToStr(Config.nWinLotteryLevel2);
        EditWinLotteryLevel3.Text = DelphiRTL.IntToStr(Config.nWinLotteryLevel3);
        EditWinLotteryLevel4.Text = DelphiRTL.IntToStr(Config.nWinLotteryLevel4);
        EditWinLotteryLevel5.Text = DelphiRTL.IntToStr(Config.nWinLotteryLevel5);
        EditWinLotteryLevel6.Text = DelphiRTL.IntToStr(Config.nWinLotteryLevel6);

        // ---- 原文 :166-175 全局变量 1..10（GlobalVal[0..9]） ----
        EditGlobalVal1.Text = DelphiRTL.IntToStr(Config.GlobalVal[0]);
        EditGlobalVal2.Text = DelphiRTL.IntToStr(Config.GlobalVal[1]);
        EditGlobalVal3.Text = DelphiRTL.IntToStr(Config.GlobalVal[2]);
        EditGlobalVal4.Text = DelphiRTL.IntToStr(Config.GlobalVal[3]);
        EditGlobalVal5.Text = DelphiRTL.IntToStr(Config.GlobalVal[4]);
        EditGlobalVal6.Text = DelphiRTL.IntToStr(Config.GlobalVal[5]);
        EditGlobalVal7.Text = DelphiRTL.IntToStr(Config.GlobalVal[6]);
        EditGlobalVal8.Text = DelphiRTL.IntToStr(Config.GlobalVal[7]);
        EditGlobalVal9.Text = DelphiRTL.IntToStr(Config.GlobalVal[8]);
        EditGlobalVal10.Text = DelphiRTL.IntToStr(Config.GlobalVal[9]);

        // ---- 原文 :177-190 工作线程表 ----
        // 接缝：g_M2RunThreadMgr 未接线即抛（§25.2；返回 null 视作空表）
        IReadOnlyList<Sweep9FormsRunThreadInfo>? mgr =
            RunThreadMgrHandler != null
                ? RunThreadMgrHandler()
                : throw Sweep9FormsKit.NotWired(nameof(RunThreadMgrHandler), "M2Threads.pas:58 g_M2RunThreadMgr");
        int count = mgr?.Count ?? 0;

        // 原文 :177 GridThread.RowCount := g_M2RunThreadMgr.Count + 1;
        GridThread.RowCount = count + 1;

        // 原文 :178 for I := 0 to g_M2RunThreadMgr.Count - 1 do
        for (I = 0; I < count; I++)
        {
            Thread = mgr![I];
            // 原文 :181 IntToStr(I) —— 序号从 0 起（行号才是 I + 1）
            GridThread.Cells[0, I + 1] = DelphiRTL.IntToStr(I);
            GridThread.Cells[1, I + 1] = Thread.ThreadDesc;
            GridThread.Cells[2, I + 1] = DelphiRTL.IntToStr(Thread.Handle);
            GridThread.Cells[3, I + 1] = DelphiRTL.IntToStr(Thread.ThreadID);
            // 原文 :185 Format('%d/%d/%d', [Thread.RunTick, Thread.MinRunTick, Thread.MaxRunTick])
            GridThread.Cells[4, I + 1] = DelphiRTL.Format("%d/%d/%d",
                Thread.RunTick, Thread.MinRunTick, Thread.MaxRunTick);
            // 原文 :186-188 峰值更新（先读、再比、再写回）
            ThreadCPUUsage = Thread.ThreadCPUUsage();
            if (ThreadCPUUsage > Thread.MaxThreadCPUUsage)
                Thread.MaxThreadCPUUsage = ThreadCPUUsage;
            // 原文 :189 Format('%d/%d', [ThreadCPUUsage, Thread.MaxThreadCPUUsage])
            GridThread.Cells[5, I + 1] = DelphiRTL.Format("%d/%d",
                ThreadCPUUsage, Thread.MaxThreadCPUUsage);
        }

        // 原文 :192-194 只清 GridMemory 第 1 列的第 2/3/4 行
        GridMemory.Cells[1, 2] = "";
        GridMemory.Cells[1, 3] = "";
        GridMemory.Cells[1, 4] = "";
    }
}

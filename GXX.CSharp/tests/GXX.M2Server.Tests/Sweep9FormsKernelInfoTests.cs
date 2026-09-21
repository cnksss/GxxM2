// ============================================================================
// ViewKernelInfo.pas（198 行）1:1 测试
//   DFM 对账（§37.3 计数取证）：控件 **63** + 组件 `Timer` **1** = DFM 64 个 object；
//   事件绑定 **2**（`Form.OnCreate = FormCreate` ↔ `Load`；`Timer.OnTimer = TimerTimer` ↔ `Tick`）
//   ★★ 重点：`FormCreate` 的 `@Config.XxxThread` 缺陷（9 个 `g_Config` 字段被写坏）
// ============================================================================

using GXX.M2Server.Sweep9.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("Sweep9FormsSerial")]
public sealed class Sweep9FormsKernelInfoTests : IDisposable
{
    private readonly Sweep9FormsKernelConfig Cfg = new();
    private readonly List<Sweep9FormsRunThreadInfo> Threads = new();
    private TfrmViewKernelInfo Form = null!;

    public Sweep9FormsKernelInfoTests()
    {
        Sweep9FormsMessageBoxSeam.Reset();
        Sweep9FormsMessageBoxSeam.UiEnabled = false;               // ★ 无头
        Sweep9FormsKernelInfoGlobals.Reset();
        Form = new TfrmViewKernelInfo();
        Form.KernelConfig = Cfg;
        Form.RunThreadMgrHandler = () => Threads;
    }

    public void Dispose()
    {
        Form.Dispose();
        Sweep9FormsKernelInfoGlobals.Reset();
        Sweep9FormsMessageBoxSeam.Reset();
    }

    private Sweep9FormsRunThreadInfo AddThread(string desc, uint handle = 0, uint tid = 0,
        uint runTick = 0, uint minTick = 0, uint maxTick = 0, params int[] cpuReadings)
    {
        int i = 0;
        var t = new Sweep9FormsRunThreadInfo
        {
            ThreadDesc = desc,
            Handle = handle,
            ThreadID = tid,
            RunTick = runTick,
            MinRunTick = minTick,
            MaxRunTick = maxTick,
            ThreadCPUUsageFn = () => cpuReadings.Length == 0 ? 0 : cpuReadings[Math.Min(i++, cpuReadings.Length - 1)],
        };
        Threads.Add(t);
        return t;
    }

    // ------------------------------------------------------------------
    // DFM 对账（计数取证）
    // ------------------------------------------------------------------

    [Fact]
    public void DfmReconcile_ControlCount_Is63PlusTimerComponent()
    {
        // ViewKernelInfo.dfm 的 object 节点 = 64（含 `object Timer: TTimer`）。
        // `TTimer` 是**组件**（不进 Controls 树）⇒ 控件 63 + 组件 1 = 64。
        Assert.Equal(63, Sweep9FormsReconcile.CountControlsExcludingForm(Form));
        Assert.Equal(64, Sweep9FormsReconcile.CountControlsExcludingForm(Form) + 1);
        Assert.NotNull(Form.Timer);
    }

    [Fact]
    public void DfmReconcile_ControlNameSet_MatchesDfmObjectNames()
    {
        // DFM 里 64 个 object 名（手工从 ViewKernelInfo.dfm 抄录）
        var expected = new[]
        {
            "Timer", "PageControl1",
            "TabSheet1", "GroupBox1", "Label1", "Label2", "Label3", "Label4",
            "EditLoadHumanDBCount", "EditLoadHumanDBErrorCoun", "EditSaveHumanDBCount", "EditHumanDBQueryID",
            "TabSheet2", "GroupBox2", "Label5", "Label6", "EditWinLotteryCount", "EditNoWinLotteryCount",
            "GroupBox3", "Label9", "Label10", "Label11", "Label12",
            "EditWinLotteryLevel1", "EditWinLotteryLevel2", "EditWinLotteryLevel3", "EditWinLotteryLevel4",
            "Label13", "EditWinLotteryLevel5", "Label14", "EditWinLotteryLevel6",
            "GroupBox4", "Label7", "Label8", "EditItemNumber", "EditItemNumberEx",
            "TabSheet3", "GroupBox5", "Label15", "Label16", "Label17", "Label18", "Label19", "Label20",
            "EditGlobalVal1", "EditGlobalVal2", "EditGlobalVal3", "EditGlobalVal4", "EditGlobalVal5",
            "EditGlobalVal6", "EditGlobalVal7", "Label21", "Label22", "EditGlobalVal8", "EditGlobalVal9",
            "Label23", "EditGlobalVal10", "Label24",
            "TabSheet5", "GroupBox7", "GridThread",
            "TabSheet6", "GroupBox8", "GridMemory",
        };
        Assert.Equal(64, expected.Length);

        var got = Sweep9FormsReconcile.EnumerateControls(Form).Skip(1)
            .Select(c => c.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();
        var want = expected.Where(n => n != "Timer").OrderBy(n => n, StringComparer.Ordinal).ToArray();
        Assert.Equal(want, got);
    }

    [Fact]
    public void DfmReconcile_EventBindingCount_Is2()
    {
        // DFM 绑定实测：`OnCreate = FormCreate`（根节点）+ `Timer.OnTimer = TimerTimer`
        int total = Sweep9FormsReconcile.CountEventBindings(Form)
                  + Sweep9FormsReconcile.CountEventBindingsOn(Form.Timer);
        Assert.Equal(2, total);
        Assert.True(Sweep9FormsReconcile.IsBound(Form, "Load"));
        Assert.True(Sweep9FormsReconcile.IsBound(Form.Timer, "Tick"));
    }

    [Fact]
    public void DfmReconcile_NoOtherControlHasAnyBoundEvent()
    {
        // 否定性断言用计数取证：63 个控件里除 Timer 外**没有任何**事件绑定。
        int bound = 0;
        foreach (var c in Sweep9FormsReconcile.DfmControls(Form))
            bound += Sweep9FormsReconcile.CountEventBindingsOn(c);
        Assert.Equal(0, bound);
    }

    [Fact]
    public void DfmReconcile_FormProperties_MatchDfm()
    {
        Assert.Equal("frmViewKernelInfo", Form.Name);
        Assert.Equal("内核数据查看", Form.Text);
        Assert.Equal(950, Form.Left);
        Assert.Equal(517, Form.Top);
        Assert.Equal(474, Form.ClientSize.Width);
        Assert.Equal(226, Form.ClientSize.Height);
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedSingle, Form.FormBorderStyle);   // bsSingle
        Assert.Equal(System.Windows.Forms.FormStartPosition.CenterParent, Form.StartPosition);   // poMainFormCenter
        Assert.False(Form.MaximizeBox);
        Assert.True(Form.MinimizeBox);
    }

    [Fact]
    public void DfmReconcile_TimerProperties_MatchDfm()
    {
        Assert.False(Form.Timer.Enabled);          // DFM Enabled = False
        // DFM 未写 Interval ⇒ Delphi TTimer 默认 1000（WinForms 默认只有 100，故必须显式设）
        Assert.Equal(1000, Form.Timer.Interval);
    }

    [Fact]
    public void DfmReconcile_TabPagesAndGroupBoxCaptions()
    {
        Assert.Equal("游戏数据", Form.TabSheet1.Text);
        Assert.Equal("彩票数据", Form.TabSheet2.Text);
        Assert.Equal(1, Form.TabSheet2.ImageIndex);
        Assert.Equal("全局变量", Form.TabSheet3.Text);
        Assert.Equal(2, Form.TabSheet3.ImageIndex);
        Assert.Equal("工作线程", Form.TabSheet5.Text);
        Assert.Equal(4, Form.TabSheet5.ImageIndex);
        Assert.Equal("内存池", Form.TabSheet6.Text);
        Assert.Equal(5, Form.TabSheet6.ImageIndex);
        // DFM ActivePage = TabSheet1 ⇒ 索引 0
        Assert.Equal(0, Form.PageControl1.SelectedIndex);

        Assert.Equal("游戏数据库", Form.GroupBox1.Text);
        Assert.Equal("中奖数量", Form.GroupBox2.Text);
        Assert.Equal("中奖比例", Form.GroupBox3.Text);
        Assert.Equal("物品系列号", Form.GroupBox4.Text);
        Assert.Equal("全局变量状态", Form.GroupBox5.Text);
        Assert.Equal("线程状态", Form.GroupBox7.Text);
        Assert.Equal("内存池", Form.GroupBox8.Text);
    }

    [Fact]
    public void DfmReconcile_All24LabelCaptions()
    {
        var pairs = new (System.Windows.Forms.Label L, string T)[]
        {
            (Form.Label1, "读取请求次数:"), (Form.Label2, "读取失败次数:"),
            (Form.Label3, "保存请求次数:"), (Form.Label4, "请求标识数字:"),
            (Form.Label5, "中奖总数:"), (Form.Label6, "未中奖数:"),
            (Form.Label7, "怪物掉落物品:"), (Form.Label8, "命令制造物品:"),
            (Form.Label9, "一等奖:"), (Form.Label10, "二等奖:"), (Form.Label11, "三等奖:"),
            (Form.Label12, "四等奖:"), (Form.Label13, "五等奖:"), (Form.Label14, "六等奖:"),
            (Form.Label15, "变量一:"), (Form.Label16, "变量二:"), (Form.Label17, "变量三:"),
            (Form.Label18, "变量四:"), (Form.Label19, "变量五:"), (Form.Label20, "变量六:"),
            (Form.Label21, "变量十:"), (Form.Label22, "变量七:"), (Form.Label23, "变量八:"),
            (Form.Label24, "变量九:"),
        };
        Assert.Equal(24, pairs.Length);
        foreach (var (l, t) in pairs)
            Assert.Equal(t, l.Text);
        // 变量十/七/八/九 的坐标顺序在 DFM 里是 "十, 七, 八, 九"（Label21..24 声明顺序），逐字保留
        Assert.Equal(new[] { (120, 92), (120, 20), (120, 44), (120, 68) },
            new[] { (Form.Label21.Left, Form.Label21.Top), (Form.Label22.Left, Form.Label22.Top),
                    (Form.Label23.Left, Form.Label23.Top), (Form.Label24.Left, Form.Label24.Top) });
    }

    [Fact]
    public void DfmReconcile_All24EditsAreReadOnly()
    {
        var edits = new[]
        {
            Form.EditLoadHumanDBCount, Form.EditLoadHumanDBErrorCoun, Form.EditSaveHumanDBCount,
            Form.EditHumanDBQueryID, Form.EditItemNumber, Form.EditItemNumberEx,
            Form.EditWinLotteryCount, Form.EditNoWinLotteryCount,
            Form.EditWinLotteryLevel1, Form.EditWinLotteryLevel2, Form.EditWinLotteryLevel3,
            Form.EditWinLotteryLevel4, Form.EditWinLotteryLevel5, Form.EditWinLotteryLevel6,
            Form.EditGlobalVal1, Form.EditGlobalVal2, Form.EditGlobalVal3, Form.EditGlobalVal4,
            Form.EditGlobalVal5, Form.EditGlobalVal6, Form.EditGlobalVal7, Form.EditGlobalVal8,
            Form.EditGlobalVal9, Form.EditGlobalVal10,
        };
        Assert.Equal(24, edits.Length);
        foreach (var e in edits)
        {
            Assert.True(e.ReadOnly);            // DFM 24 个 TEdit 全部 ReadOnly = True
            Assert.Equal(20, e.Height);         // DFM Height = 20（AutoSize = false 才保得住）
        }
        // 宽度按 DFM 分三档：57（4+2+6+10 个 57 宽）/ 73（物品系列号那两个）
        Assert.Equal(57, Form.EditLoadHumanDBCount.Width);
        Assert.Equal(57, Form.EditWinLotteryLevel6.Width);
        Assert.Equal(57, Form.EditGlobalVal10.Width);
        Assert.Equal(73, Form.EditItemNumber.Width);        // DFM :117 Width = 73
        Assert.Equal(73, Form.EditItemNumberEx.Width);      // DFM :125 Width = 73
    }

    [Fact]
    public void DfmReconcile_GridProperties_MatchDfm()
    {
        Assert.Equal(6, Form.GridThread.DfmColCount);
        Assert.Equal(new[] { 30, 80, 51, 90, 64, 64 }, Form.GridThread.DfmColWidths);
        Assert.Equal(18, Form.GridThread.DfmDefaultRowHeight);
        Assert.Equal(0, Form.GridThread.DfmFixedCols);
        Assert.Equal(5, Form.GridThread.DfmRowCount);        // DFM 未写 ⇒ 默认 5

        Assert.Equal(3, Form.GridMemory.DfmColCount);
        Assert.Equal(new[] { 126, 98, 86 }, Form.GridMemory.DfmColWidths);
        Assert.Equal(18, Form.GridMemory.DfmDefaultRowHeight);

        Assert.Equal(new[]
        {
            "goFixedVertLine", "goFixedHorzLine", "goVertLine", "goHorzLine", "goRangeSelect", "goRowSelect",
        }, Form.GridThread.DfmOptions);
        Assert.Equal(Form.GridThread.DfmOptions, Form.GridMemory.DfmOptions);
    }

    // ------------------------------------------------------------------
    // ★★ FormCreate 的 g_Config 破坏（D-P9-01）
    // ------------------------------------------------------------------

    [Fact]
    public void FormCreate_ClobbersConfigRegion_AllNineFields()
    {
        // 先把该段内存全部填成"可辨识哨兵"（dword i = 100 + i）
        Cfg.ThreadRegion.FillSentinels();
        Assert.Equal(100, Cfg.ThreadRegion.UserEngineThread);
        Assert.True(Cfg.ThreadRegion.boSkill69SameLevel);      // dword8 = 108 → 低字节非 0
        Assert.Equal(109, Cfg.ThreadRegion.nSkill70CD);

        Form.FormCreate();

        // ★ 被写坏的 9 个字段（`:116/122/128` 三块 4 字节重叠写）
        Assert.Equal(0, Cfg.ThreadRegion.UserEngineThread);     // 原文 := nil
        Assert.Equal(0, Cfg.ThreadRegion.IDSocketThread);       // 原文 := nil
        Assert.Equal(0, Cfg.ThreadRegion.DBSOcketThread);       // 原文 := nil
        Assert.Equal(0, Cfg.ThreadRegion.nUserSellOffCount);
        Assert.Equal(0, Cfg.ThreadRegion.nUserSellOffTax);
        Assert.Equal(0, Cfg.ThreadRegion.nSkill69CD);
        Assert.Equal(0, Cfg.ThreadRegion.nSkill69AddTime);
        Assert.Equal(0, Cfg.ThreadRegion.nSkill69AddRange);
        Assert.False(Cfg.ThreadRegion.boSkill69SameLevel);

        // 只有 dword9（nSkill70CD）落在覆盖范围之外 ⇒ 保持哨兵
        Assert.Equal(109, Cfg.ThreadRegion.nSkill70CD);
    }

    [Fact]
    public void FormCreate_FirstBlockAlone_ClobbersOnlyFiveFields()
    {
        // 机制取证：单独跑第一块（只 `@Config.UserEngineThread`）时，
        // 恰好命中 dword {0,2,4,5,6} —— 与 TThreadInfo 的字段偏移一致。
        Cfg.ThreadRegion.FillSentinels();
        Cfg.ThreadRegion.WriteThreadInfoFieldsAt(0);

        Assert.Equal(0, Cfg.ThreadRegion.UserEngineThread);     // +0 dwRunTick
        Assert.Equal(101, Cfg.ThreadRegion.IDSocketThread);     // +1 未写
        Assert.Equal(0, Cfg.ThreadRegion.DBSOcketThread);       // +2 nRunFlag
        Assert.Equal(103, Cfg.ThreadRegion.nUserSellOffCount);  // +3 未写
        Assert.Equal(0, Cfg.ThreadRegion.nUserSellOffTax);      // +4 nRunTime
        Assert.Equal(0, Cfg.ThreadRegion.nSkill69CD);           // +5 nMaxRunTime
        Assert.Equal(0, Cfg.ThreadRegion.nSkill69AddTime);      // +6 hThreadHandle
        Assert.Equal(107, Cfg.ThreadRegion.nSkill69AddRange);   // +7 未写
        Assert.True(Cfg.ThreadRegion.boSkill69SameLevel);       // +8 未写（boActived/boTerminaled 是 1 字节字段）
        Assert.Equal(109, Cfg.ThreadRegion.nSkill70CD);         // +9 未写
    }

    [Fact]
    public void ThreadRegion_ModelsDwordLayoutOfTConfig()
    {
        // 段内存视图的排版与 M2Share.pas:1808-1817 的字段序一致
        Assert.Equal(10, TConfigThreadRegion.DwordCount);
        var r = new TConfigThreadRegion();
        r.UserEngineThread = 1; r.IDSocketThread = 2; r.DBSOcketThread = 3;
        r.nUserSellOffCount = 4; r.nUserSellOffTax = 5; r.nSkill69CD = 6;
        r.nSkill69AddTime = 7; r.nSkill69AddRange = 8; r.nSkill70CD = 10;
        Assert.Equal(1, r.GetDword(0));
        Assert.Equal(2, r.GetDword(1));
        Assert.Equal(3, r.GetDword(2));
        Assert.Equal(4, r.GetDword(3));
        Assert.Equal(5, r.GetDword(4));
        Assert.Equal(6, r.GetDword(5));
        Assert.Equal(7, r.GetDword(6));
        Assert.Equal(8, r.GetDword(7));
        Assert.Equal(10, r.GetDword(9));
        // boSkill69SameLevel 落在 dword8 的低字节
        r.boSkill69SameLevel = true;
        Assert.Equal(1, r.GetDword(8) & 0xFF);
        r.SetDword(8, unchecked((int)0xFFFFFF00));
        Assert.False(r.boSkill69SameLevel);
    }

    [Fact]
    public void FormCreate_WritesGridHeaders_Only()
    {
        Form.FormCreate();

        Assert.Equal("序号", Form.GridThread.Cells[0, 0]);
        Assert.Equal("描述", Form.GridThread.Cells[1, 0]);
        Assert.Equal("句柄", Form.GridThread.Cells[2, 0]);
        Assert.Equal("线程ID", Form.GridThread.Cells[3, 0]);
        Assert.Equal("运行时间", Form.GridThread.Cells[4, 0]);
        Assert.Equal("CPU占用", Form.GridThread.Cells[5, 0]);

        Assert.Equal("名称", Form.GridMemory.Cells[0, 0]);
        Assert.Equal("数量", Form.GridMemory.Cells[1, 0]);
        Assert.Equal("大小", Form.GridMemory.Cells[2, 0]);
        Assert.Equal("UserItem", Form.GridMemory.Cells[0, 1]);
        Assert.Equal("MapItem", Form.GridMemory.Cells[0, 2]);
        Assert.Equal("BaseObject", Form.GridMemory.Cells[0, 3]);
        Assert.Equal("MapEvent", Form.GridMemory.Cells[0, 4]);
        // 第 1 列第 1..4 行 FormCreate 里**不写**（只 TimerTimer 清）
        Assert.Equal("", Form.GridMemory.Cells[1, 1]);
        Assert.Equal("", Form.GridMemory.Cells[1, 2]);
    }

    // ------------------------------------------------------------------
    // TimerTimer（:143-195）
    // ------------------------------------------------------------------

    [Fact]
    public void TimerTimer_BackfillsAllIntegerFields()
    {
        Cfg.nLoadDBCount = 11;
        Cfg.nLoadDBErrorCount = 22;
        Cfg.nSaveDBCount = 33;
        Cfg.nDBQueryID = 44;
        Cfg.nItemNumber = 55;
        Cfg.nItemNumberEx = 66;
        Cfg.nWinLotteryCount = 77;
        Cfg.nNoWinLotteryCount = 88;
        Cfg.nWinLotteryLevel1 = 1;
        Cfg.nWinLotteryLevel2 = 2;
        Cfg.nWinLotteryLevel3 = 3;
        Cfg.nWinLotteryLevel4 = 4;
        Cfg.nWinLotteryLevel5 = 5;
        Cfg.nWinLotteryLevel6 = 6;
        for (int i = 0; i < 10; i++) Cfg.GlobalVal[i] = 1000 + i;

        Form.TimerTimer();

        Assert.Equal("11", Form.EditLoadHumanDBCount.Text);
        Assert.Equal("22", Form.EditLoadHumanDBErrorCoun.Text);
        Assert.Equal("33", Form.EditSaveHumanDBCount.Text);
        Assert.Equal("44", Form.EditHumanDBQueryID.Text);
        Assert.Equal("55", Form.EditItemNumber.Text);
        Assert.Equal("66", Form.EditItemNumberEx.Text);
        Assert.Equal("77", Form.EditWinLotteryCount.Text);
        Assert.Equal("88", Form.EditNoWinLotteryCount.Text);
        Assert.Equal("1", Form.EditWinLotteryLevel1.Text);
        Assert.Equal("6", Form.EditWinLotteryLevel6.Text);
        Assert.Equal("1000", Form.EditGlobalVal1.Text);
        Assert.Equal("1009", Form.EditGlobalVal10.Text);
    }

    [Fact]
    public void TimerTimer_GlobalValIndexes_AreZeroBased()
    {
        // GlobalVal 是 array[0..999] ⇒ EditGlobalVal1 读 [0]、EditGlobalVal10 读 [9]
        Cfg.GlobalVal[0] = 7;
        Cfg.GlobalVal[1] = 8;
        Cfg.GlobalVal[8] = 15;
        Cfg.GlobalVal[9] = 16;
        Form.TimerTimer();
        Assert.Equal("7", Form.EditGlobalVal1.Text);
        Assert.Equal("8", Form.EditGlobalVal2.Text);
        Assert.Equal("15", Form.EditGlobalVal9.Text);
        Assert.Equal("16", Form.EditGlobalVal10.Text);
    }

    [Fact]
    public void TimerTimer_PopulatesThreadGrid_WithZeroBasedIndex()
    {
        AddThread("人物处理", handle: 1234, tid: 5678, runTick: 10, minTick: 5, maxTick: 20, 7);
        AddThread("怪物处理", handle: 1, tid: 2, runTick: 3, minTick: 1, maxTick: 4, 9);

        Form.TimerTimer();

        Assert.Equal(3, Form.GridThread.RowCount);        // Count + 1
        Assert.Equal("0", Form.GridThread.Cells[0, 1]);    // IntToStr(I) —— 从 0 起
        Assert.Equal("人物处理", Form.GridThread.Cells[1, 1]);
        Assert.Equal("1234", Form.GridThread.Cells[2, 1]);
        Assert.Equal("5678", Form.GridThread.Cells[3, 1]);
        Assert.Equal("10/5/20", Form.GridThread.Cells[4, 1]);
        Assert.Equal("7/7", Form.GridThread.Cells[5, 1]);
        Assert.Equal("1", Form.GridThread.Cells[0, 2]);
        Assert.Equal("怪物处理", Form.GridThread.Cells[1, 2]);
        Assert.Equal("3/1/4", Form.GridThread.Cells[4, 2]);
        Assert.Equal("9/9", Form.GridThread.Cells[5, 2]);
    }

    [Fact]
    public void TimerTimer_UpdatesPeakCpuUsage_AndKeepsItAcrossTicks()
    {
        var t = AddThread("T", cpuReadings: new[] { 5, 9, 3 });
        Form.TimerTimer();
        Assert.Equal(5, t.MaxThreadCPUUsage);
        Assert.Equal("5/5", Form.GridThread.Cells[5, 1]);

        Form.TimerTimer();
        Assert.Equal(9, t.MaxThreadCPUUsage);          // 峰值抬升
        Assert.Equal("9/9", Form.GridThread.Cells[5, 1]);

        Form.TimerTimer();
        Assert.Equal(9, t.MaxThreadCPUUsage);          // 读数降到 3，但峰值保留
        Assert.Equal("3/9", Form.GridThread.Cells[5, 1]);
    }

    [Fact]
    public void TimerTimer_EmptyThreadList_LeavesOnlyHeaderRow()
    {
        // 原文 :177 GridThread.RowCount := 0 + 1 = 1 ⇒ 只剩表头行，循环体不执行
        Form.TimerTimer();
        Assert.Equal(1, Form.GridThread.RowCount);
        Assert.Equal("", Form.GridThread.Cells[0, 0]);   // 表头由 FormCreate 写，TimerTimer 不重写
    }

    [Fact]
    public void TimerTimer_ClearsOnlyGridMemoryColumn1Rows2To4()
    {
        // 预置：第 0 列不动、第 1 列第 1 行不动
        Form.GridMemory.Cells[0, 2] = "UserItem";
        Form.GridMemory.Cells[1, 2] = "旧值2";
        Form.GridMemory.Cells[1, 3] = "旧值3";
        Form.GridMemory.Cells[1, 4] = "旧值4";
        Form.GridMemory.Cells[1, 1] = "保留1";
        Form.GridMemory.Cells[2, 2] = "保留SIZE";

        Form.TimerTimer();

        Assert.Equal("", Form.GridMemory.Cells[1, 2]);
        Assert.Equal("", Form.GridMemory.Cells[1, 3]);
        Assert.Equal("", Form.GridMemory.Cells[1, 4]);
        Assert.Equal("保留1", Form.GridMemory.Cells[1, 1]);     // :192-194 不含第 1 行
        Assert.Equal("UserItem", Form.GridMemory.Cells[0, 2]);  // 第 0 列不动
        Assert.Equal("保留SIZE", Form.GridMemory.Cells[2, 2]);  // 第 2 列不动
    }

    [Fact]
    public void TimerTimer_UnwiredRunThreadMgr_Throws()
    {
        Form.RunThreadMgrHandler = null;
        var ex = Assert.Throws<InvalidOperationException>(() => Form.TimerTimer());
        Assert.Contains("M2Threads.pas", ex.Message);
    }

    [Fact]
    public void TimerTimer_UnwiredConfig_Throws()
    {
        Form.KernelConfig = null;
        var ex = Assert.Throws<InvalidOperationException>(() => Form.TimerTimer());
        Assert.Contains("g_Config", ex.Message);
    }

    [Fact]
    public void FormCreate_UnwiredConfig_Throws()
    {
        Form.KernelConfig = null;
        var ex = Assert.Throws<InvalidOperationException>(() => Form.FormCreate());
        Assert.Contains("g_Config", ex.Message);
    }

    [Fact]
    public void RunThreadInfo_UnwiredCpuUsage_Throws()
    {
        var t = new Sweep9FormsRunThreadInfo { ThreadDesc = "T" };
        var ex = Assert.Throws<InvalidOperationException>(() => t.ThreadCPUUsage());
        Assert.Contains("M2Threads.pas", ex.Message);
    }

    // ------------------------------------------------------------------
    // Open（:136-141）
    // ------------------------------------------------------------------

    [Fact]
    public void Open_TogglesTimerAroundShowModal()
    {
        Assert.False(Form.Timer.Enabled);

        Form.Open();

        // ShowModal 返回后 Timer 被关掉 ⇒ 观察到的终态是 false
        Assert.False(Form.Timer.Enabled);
        Assert.Equal(1, Sweep9FormsMessageBoxSeam.ShowModalCount);
    }

    [Fact]
    public void Open_EnablesTimerBeforeShowModal_ProbeViaHandler()
    {
        bool enabledInsideModal = false;
        Sweep9FormsMessageBoxSeam.ShowModalHandler = () =>
        {
            enabledInsideModal = Form.Timer.Enabled;      // :138 已置 True，:140 才会置 False
            return 2;
        };

        Form.Open();

        Assert.True(enabledInsideModal);
        Assert.False(Form.Timer.Enabled);
        Assert.Equal(2, Sweep9FormsMessageBoxSeam.LastModalResult);
    }

    [Fact]
    public void GlobalFormVariable_IsNullUntilWired()
    {
        Assert.Null(Sweep9FormsKernelInfoGlobals.frmViewKernelInfo);
        var f = new TfrmViewKernelInfo();
        Sweep9FormsKernelInfoGlobals.frmViewKernelInfo = f;
        Assert.Same(f, Sweep9FormsKernelInfoGlobals.frmViewKernelInfo);
        f.Dispose();
    }
}

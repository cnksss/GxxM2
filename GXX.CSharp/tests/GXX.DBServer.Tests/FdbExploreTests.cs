using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>FDBexpl.pas:1-369（人物数据管理）逻辑测试：过期人物清理、物品清理与日志、各按钮分支。</summary>
public class FdbExploreTests : TempDirTest
{
    private const string LogName = "ClearItemLog.txt";

    private static double D(int y, int m, int d) => DelphiDate.EncodeDate((ushort)y, (ushort)m, (ushort)d);

    private static string LogPath() => FdbExploreSeam.ClearItemLogFile;

    // ---------------- GetDateTime ----------------

    [Fact]
    public void GetDateTime_零阈值返回今天()
    {
        Assert.Equal(D(2025, 3, 15), FdbExploreLogic.GetDateTime(0, 0, 2025, 3, 15));
    }

    [Fact]
    public void GetDateTime_退月不跨年与跨年()
    {
        Assert.Equal(D(2025, 2, 15), FdbExploreLogic.GetDateTime(1, 0, 2025, 3, 15));
        Assert.Equal(D(2024, 12, 5), FdbExploreLogic.GetDateTime(1, 0, 2025, 1, 5));
        Assert.Equal(D(2024, 11, 5), FdbExploreLogic.GetDateTime(2, 0, 2025, 1, 5));
        Assert.Equal(D(2023, 12, 5), FdbExploreLogic.GetDateTime(13, 0, 2025, 1, 5));
    }

    [Fact]
    public void GetDateTime_退天不跨月与跨月()
    {
        Assert.Equal(D(2025, 3, 8), FdbExploreLogic.GetDateTime(0, 7, 2025, 3, 15));
        Assert.Equal(D(2025, 2, 28), FdbExploreLogic.GetDateTime(0, 15, 2025, 3, 15));   // 日归 1 时直接跳到 28 号
        Assert.Equal(D(2025, 2, 28), FdbExploreLogic.GetDateTime(0, 16, 2025, 3, 16));
    }

    [Fact]
    public void GetDateTime_退天跨年()
    {
        // 2025-01-02 退 5 天：1→28 时跳回 2024-12，再退到 25
        Assert.Equal(D(2024, 12, 25), FdbExploreLogic.GetDateTime(0, 5, 2025, 1, 2));
    }

    [Fact]
    public void GetDateTime_使用Now接缝()
    {
        FdbExploreSeam.Now = () => D(2025, 3, 15);
        Assert.Equal(D(2025, 3, 8), FdbExploreLogic.GetDateTime(0, 7));
    }

    // ---------------- ClearHumanItem ----------------

    private static THumDataInfo InfoWithItems()
    {
        var info = TestReset.MakeInfo("chr1", D(2025, 1, 1), 1);
        info.Data.HumItems[2] = TestReset.Item(5, 77);
        info.Data.HumItems[3] = TestReset.Item(6, 78);
        info.HumAddItems[0] = TestReset.Item(9, 77);
        info.Data.BagItems[1] = TestReset.Item(7, 77);
        info.Data.StorageItems[0] = TestReset.Item(8, 77);
        return info;
    }

    [Fact]
    public void ClearHumanItem_四类槽位按序清理并把wIndex清0()
    {
        FdbExploreSeam.InClearMakeIndexList = m => m == 77;
        var info = InfoWithItems();

        bool changed = FdbExploreLogic.ClearHumanItem(info, out int cleared);

        Assert.True(changed);
        Assert.Equal(4, cleared);
        Assert.Equal((ushort)0, info.Data.HumItems[2].wIndex);
        Assert.Equal((ushort)0, info.HumAddItems[0].wIndex);
        Assert.Equal((ushort)0, info.Data.BagItems[1].wIndex);
        Assert.Equal((ushort)0, info.Data.StorageItems[0].wIndex);
        Assert.Equal((ushort)6, info.Data.HumItems[3].wIndex);      // 未命中的保持
        Assert.Equal(4, FdbExploreSeam.g_nClearItemIndexCount);
    }

    [Fact]
    public void ClearHumanItem_wIndex小于等于0的槽位跳过()
    {
        FdbExploreSeam.InClearMakeIndexList = _ => true;
        var info = TestReset.MakeInfo("chr1", 0, 1);
        info.Data.HumItems[0] = TestReset.Item(0, 77);              // wIndex = 0 → 跳过
        info.Data.BagItems[0] = TestReset.Item(3, 77);

        Assert.True(FdbExploreLogic.ClearHumanItem(info, out int cleared));
        Assert.Equal(1, cleared);
        Assert.Equal(77, info.Data.HumItems[0].MakeIndex);          // 未被清（未被处理）
    }

    [Fact]
    public void ClearHumanItem_无命中返回false且不创建日志()
    {
        FdbExploreSeam.InClearMakeIndexList = _ => false;
        var info = InfoWithItems();

        Assert.False(FdbExploreLogic.ClearHumanItem(info, out int cleared));
        Assert.Equal(0, cleared);
        Assert.Equal(0, FdbExploreSeam.g_nClearItemIndexCount);
        Assert.False(File.Exists(LogPath()));
    }

    [Fact]
    public void ClearHumanItem_日志内容为名称_Tab_wIndex_Tab_MakeIndex_并按插入序倒排()
    {
        FdbExploreSeam.InClearMakeIndexList = m => m == 77;
        var info = InfoWithItems();

        FdbExploreLogic.ClearHumanItem(info, out _);

        // 处理顺序 HumItems → HumAddItems → BagItems → StorageItems，每条 Insert(0) → 最终倒序
        string expected =
            "chr1\t8\t77\r\n" +
            "chr1\t7\t77\r\n" +
            "chr1\t9\t77\r\n" +
            "chr1\t5\t77\r\n";
        Assert.Equal(expected, File.ReadAllText(LogPath()));
    }

    [Fact]
    public void ClearHumanItem_已有日志文件时新条目插在最前_保留旧内容()
    {
        FdbExploreSeam.InClearMakeIndexList = m => m == 77;
        File.WriteAllText(LogPath(), "old-line\r\n\r\n");
        var info = TestHelperSingle();

        FdbExploreLogic.ClearHumanItem(info, out _);

        string text = File.ReadAllText(LogPath());
        Assert.StartsWith("chr1\t5\t77\r\n", text);
        Assert.Contains("old-line\r\n", text);
    }

    private static THumDataInfo TestHelperSingle()
    {
        var info = TestReset.MakeInfo("chr1", 0, 1);
        info.Data.HumItems[0] = TestReset.Item(5, 77);
        return info;
    }

    [Fact]
    public void ClearHumanItem_累加计数跨多次调用()
    {
        FdbExploreSeam.InClearMakeIndexList = m => m == 77;
        FdbExploreLogic.ClearHumanItem(TestHelperSingle(), out _);
        FdbExploreLogic.ClearHumanItem(TestHelperSingle(), out _);
        Assert.Equal(2, FdbExploreSeam.g_nClearItemIndexCount);
    }

    // ---------------- Timer1Timer ----------------

    [Fact]
    public void Timer1Timer_未开启自动清理直接返回且不打开数据库()
    {
        var db = new MemoryHumDataDB();
        FdbExploreSeam.boAutoClearDB = 0;

        string name = FdbExploreLogic.Timer1Timer(db, true, true, true);

        Assert.Equal("", name);
        Assert.Equal(0, db.OpenCount);
    }

    [Fact]
    public void Timer1Timer_过期人物被删除并回调DelHum()
    {
        var db = new MemoryHumDataDB();
        db.Records[0] = TestReset.MakeInfo("old", D(2025, 1, 1), 1);
        FdbExploreSeam.boAutoClearDB = 1;
        FdbExploreSeam.Now = () => D(2025, 3, 15);
        FdbExploreSeam.nDay1 = 7;
        FdbExploreSeam.nLevel1 = 5;
        var deletedNames = new List<string>();
        FdbExploreSeam.FrmDBSrv_DelHum = n => deletedNames.Add(n);

        string name = FdbExploreLogic.Timer1Timer(db, true, false, false);

        Assert.Equal("old", name);
        Assert.Equal(new List<string> { "old" }, deletedNames);
        Assert.Equal(new List<int> { 0 }, db.Deleted);
        Assert.Equal(1, FdbExploreSeam.g_nClearCount);
        Assert.Equal(1, FdbExploreSeam.g_nClearIndex);
        Assert.Equal(1, FdbExploreSeam.g_nClearRecordCount);
        Assert.Equal(1, db.OpenCount);
        Assert.Equal(1, db.CloseCount);
    }

    [Fact]
    public void Timer1Timer_等级超过阈值不删除_转而清理物品并Update()
    {
        var db = new MemoryHumDataDB();
        var info = TestReset.MakeInfo("recent", D(2025, 3, 14), 99);        // 未过期
        info.Data.BagItems[0] = TestReset.Item(1, 77);
        db.Records[0] = info;
        FdbExploreSeam.boAutoClearDB = 1;
        FdbExploreSeam.Now = () => D(2025, 3, 15);
        FdbExploreSeam.nDay1 = 7;
        FdbExploreSeam.nLevel1 = 5;
        FdbExploreSeam.InClearMakeIndexList = m => m == 77;

        string name = FdbExploreLogic.Timer1Timer(db, true, false, false);

        Assert.Equal("", name);
        Assert.Empty(db.Deleted);
        Assert.Equal(new List<int> { 0 }, db.Updated);
        Assert.Equal(0, FdbExploreSeam.g_nClearCount);
        Assert.Equal(1, FdbExploreSeam.g_nClearIndex);
    }

    [Fact]
    public void Timer1Timer_未勾选清理项时阈值为0_等级为0的人物仍会被删_原文如此()
    {
        var db = new MemoryHumDataDB();
        db.Records[0] = TestReset.MakeInfo("x", D(2025, 3, 1), 0);      // 等级 0
        FdbExploreSeam.boAutoClearDB = 1;
        FdbExploreSeam.Now = () => D(2025, 3, 15);

        string name = FdbExploreLogic.Timer1Timer(db, false, false, false);

        // 未勾选时 w32/wDayCount1/wLevel1 全为 0 → dt20 = 今天、wLevel1 = 0
        // → (创建日期 < 今天) and (等级 <= 0) 成立 → 被删
        Assert.Equal("x", name);
        Assert.Equal(new List<int> { 0 }, db.Deleted);
    }

    [Fact]
    public void Timer1Timer_未勾选清理项时等级大于0的人物不会被删()
    {
        var db = new MemoryHumDataDB();
        db.Records[0] = TestReset.MakeInfo("x", D(2025, 3, 1), 1);
        FdbExploreSeam.boAutoClearDB = 1;
        FdbExploreSeam.Now = () => D(2025, 3, 15);

        string name = FdbExploreLogic.Timer1Timer(db, false, false, false);

        Assert.Equal("", name);
        Assert.Empty(db.Deleted);
    }

    [Fact]
    public void Timer1Timer_索引走到底后回绕为0()
    {
        var db = new MemoryHumDataDB();
        FdbExploreSeam.boAutoClearDB = 1;
        FdbExploreSeam.g_nClearIndex = 5;
        FdbExploreSeam.Now = () => D(2025, 3, 15);

        FdbExploreLogic.Timer1Timer(db, true, false, false);

        Assert.Equal(0, FdbExploreSeam.g_nClearIndex);
        Assert.Equal(0, FdbExploreSeam.g_nClearRecordCount);
    }

    [Fact]
    public void Timer1Timer_Open失败也会Close()
    {
        var db = new MemoryHumDataDB { OpenResult = false };
        FdbExploreSeam.boAutoClearDB = 1;
        FdbExploreSeam.Now = () => D(2025, 3, 15);

        FdbExploreLogic.Timer1Timer(db, true, false, false);

        Assert.Equal(1, db.OpenCount);
        Assert.Equal(1, db.CloseCount);
        Assert.Equal(0, FdbExploreSeam.g_nClearRecordCount);
    }

    [Fact]
    public void Timer1Timer_Get返回负值时索引不前进()
    {
        var db = new MemoryHumDataDB { CountOverride = 1 };      // Count=1 但没有记录 → Get 返回 -1
        FdbExploreSeam.boAutoClearDB = 1;
        FdbExploreSeam.Now = () => D(2025, 3, 15);

        FdbExploreLogic.Timer1Timer(db, true, false, false);

        Assert.Equal(0, FdbExploreSeam.g_nClearIndex);
        Assert.Equal(1, FdbExploreSeam.g_nClearRecordCount);
    }

    [Fact]
    public void Timer1Timer_三个阈值是或关系_任一命中即删除()
    {
        var db = new MemoryHumDataDB();
        db.Records[0] = TestReset.MakeInfo("x", D(2024, 1, 1), 14);
        FdbExploreSeam.boAutoClearDB = 1;
        FdbExploreSeam.Now = () => D(2025, 3, 15);
        FdbExploreSeam.nMonth3 = 4;                              // 4 个月前 = 2024-11-15
        FdbExploreSeam.nLevel3 = 14;

        string name = FdbExploreLogic.Timer1Timer(db, false, false, true);

        Assert.Equal("x", name);
        Assert.Equal(new List<int> { 0 }, db.Deleted);
    }

    // ---------------- 各按钮 ----------------

    [Fact]
    public void BtnAutoCleanClick_翻转开关并切换标题()
    {
        Assert.Equal("自动清理", FdbExploreLogic.BtnAutoCleanClick());
        Assert.Equal((byte)1, FdbExploreSeam.boAutoClearDB);
        Assert.Equal("已停止清理", FdbExploreLogic.BtnAutoCleanClick());
        Assert.Equal((byte)0, FdbExploreSeam.boAutoClearDB);
    }

    [Fact]
    public void EdFindKeyPress_仅回车生效_并写出记录号()
    {
        var db = new MemoryHumDataDB();
        db.FoundByName[3] = "aaa";
        db.FoundByName[7] = "aab";
        var list1 = new GXX.Core.Util.TStringList();
        var list2 = new List<int>();

        FdbExploreLogic.EdFindKeyPress(db, 'x', "aa", list1, list2);
        Assert.Equal(0, list1.Count);
        Assert.Equal(0, db.OpenCount);

        FdbExploreLogic.EdFindKeyPress(db, '\r', " aa ", list1, list2);
        Assert.Equal(2, list1.Count);
        Assert.Equal(new List<int> { 3, 7 }, list2);
        Assert.Equal(1, db.OpenCount);
        Assert.Equal(1, db.CloseCount);
    }

    [Fact]
    public void EdFindKeyPress_回车但名称为空时不动()
    {
        var db = new MemoryHumDataDB();
        var list1 = new GXX.Core.Util.TStringList();
        list1.Add("keep");
        var list2 = new List<int> { 1 };

        FdbExploreLogic.EdFindKeyPress(db, '\r', "   ", list1, list2);

        Assert.Equal(1, list1.Count);
        Assert.Equal(new List<int> { 1 }, list2);
        Assert.Equal(0, db.OpenCount);
    }

    [Fact]
    public void BtnDelClick_无选中时不弹确认也不删除()
    {
        using var ui = new UiRecorder();
        var db = new MemoryHumDataDB();

        FdbExploreLogic.BtnDelClick(db, -1, 5);

        Assert.Empty(ui.MessageDlgs);
        Assert.Empty(db.Deleted);
    }

    [Fact]
    public void BtnDelClick_确认后打开数据库删除并关闭()
    {
        using var ui = new UiRecorder();
        ui.MessageDlgResult = TModalResult.mrYes;
        var db = new MemoryHumDataDB();

        FdbExploreLogic.BtnDelClick(db, 0, 5);

        Assert.Equal("是否确认删除人物数据 5 ？", ui.MessageDlgs[0].Msg);
        Assert.Equal(TMsgDlgType.mtConfirmation, ui.MessageDlgs[0].Type);
        Assert.Equal(new List<int> { 5 }, db.Deleted);
        Assert.Equal(1, db.OpenCount);
        Assert.Equal(1, db.CloseCount);
    }

    [Fact]
    public void BtnDelClick_取消则不删除()
    {
        using var ui = new UiRecorder();
        ui.MessageDlgResult = TModalResult.mrNo;
        var db = new MemoryHumDataDB();

        FdbExploreLogic.BtnDelClick(db, 0, 5);

        Assert.Empty(db.Deleted);
        Assert.Equal(0, db.OpenCount);
    }

    [Fact]
    public void BtnRebuildClick_确认后重建并提示完成()
    {
        using var ui = new UiRecorder();
        ui.MessageDlgResult = TModalResult.mrYes;
        var db = new MemoryHumDataDB();

        FdbExploreLogic.BtnRebuildClick(db);

        Assert.Equal(1, db.RebuildCount);
        Assert.Equal(2, ui.MessageDlgs.Count);
        Assert.Equal("数据库重建完成！！！", ui.MessageDlgs[1].Msg);
        Assert.Equal(TMsgDlgType.mtInformation, ui.MessageDlgs[1].Type);
    }

    [Fact]
    public void BtnRebuildClick_取消则不重建()
    {
        using var ui = new UiRecorder();
        ui.MessageDlgResult = TModalResult.mrNo;
        var db = new MemoryHumDataDB();

        FdbExploreLogic.BtnRebuildClick(db);

        Assert.Equal(0, db.RebuildCount);
        Assert.Single(ui.MessageDlgs);
    }

    [Fact]
    public void BtnAddClick_回填新人物名称()
    {
        FdbExploreSeam.FrmNewChr_sub_49BD60 = cb => cb("NewGuy");
        Assert.Equal("NewGuy", FdbExploreLogic.BtnAddClick());
    }

    [Fact]
    public void BtnCopyRcdClick_未确认时不复制不提示()
    {
        using var ui = new UiRecorder();
        bool ok = FdbExploreLogic.BtnCopyRcdClick(out _, out _, out _);
        Assert.False(ok);
        Assert.Empty(ui.ShowMessages);
    }

    [Fact]
    public void BtnCopyRcdClick_复制成功时提示()
    {
        using var ui = new UiRecorder();
        FdbExploreSeam.FrmCopyRcd_sub_49C09C = () => true;
        FdbExploreSeam.FrmCopyRcd_s2F0 = "src";
        FdbExploreSeam.FrmCopyRcd_s2F4 = "dest";
        FdbExploreSeam.FrmCopyRcd_s2F8 = "user";
        string gotSrc = null, gotDest = null, gotUser = null;
        FdbExploreSeam.FrmDBSrv_CopyHumData = (a, b, c) => { gotSrc = a; gotDest = b; gotUser = c; return true; };

        bool ok = FdbExploreLogic.BtnCopyRcdClick(out string src, out string dest, out string user);

        Assert.True(ok);
        Assert.Equal("src", src);
        Assert.Equal("dest", dest);
        Assert.Equal("user", user);
        Assert.Equal("src", gotSrc);
        Assert.Equal("dest", gotDest);
        Assert.Equal("user", gotUser);
        Assert.Equal("src -> dest 复制成功！！！", ui.ShowMessages[0]);
    }

    [Fact]
    public void BtnCopyRcdClick_复制失败时不提示()
    {
        using var ui = new UiRecorder();
        FdbExploreSeam.FrmCopyRcd_sub_49C09C = () => true;
        FdbExploreSeam.FrmDBSrv_CopyHumData = (_, _, _) => false;

        FdbExploreLogic.BtnCopyRcdClick(out _, out _, out _);

        Assert.Empty(ui.ShowMessages);
    }

    [Fact]
    public void BtnCopyNewClick_需先建人物成功再复制才提示()
    {
        using var ui = new UiRecorder();
        FdbExploreSeam.FrmCopyRcd_sub_49C09C = () => true;
        FdbExploreSeam.FrmCopyRcd_s2F0 = "src";
        FdbExploreSeam.FrmCopyRcd_s2F4 = "dest";
        FdbExploreSeam.FrmCopyRcd_s2F8 = "user";

        FdbExploreSeam.FrmUserSoc_NewChrData = (_, _, _, _, _) => false;   // 建人物失败
        FdbExploreSeam.FrmDBSrv_CopyHumData = (_, _, _) => true;
        FdbExploreLogic.BtnCopyNewClick(out _, out _, out _);
        Assert.Empty(ui.ShowMessages);

        FdbExploreSeam.FrmUserSoc_NewChrData = (n, _, _, _, hero) => n == "dest" && !hero;
        FdbExploreLogic.BtnCopyNewClick(out _, out _, out _);
        Assert.Equal("src -> dest 复制成功！！！", ui.ShowMessages[0]);
    }

    [Fact]
    public void BtnCopyNewClick_未确认时直接返回()
    {
        using var ui = new UiRecorder();
        FdbExploreSeam.FrmCopyRcd_sub_49C09C = () => false;
        Assert.False(FdbExploreLogic.BtnCopyNewClick(out _, out _, out _));
        Assert.Empty(ui.ShowMessages);
    }

    // ---------------- 窗体 ----------------

    [Fact]
    public void 窗体FormCreate_定时器间隔与清理计数器初始化()
    {
        FdbExploreSeam.dwInterval = 2500;
        using var form = new FrmFDBExplore();

        form.FormCreate(null, EventArgs.Empty);

        Assert.Equal(2500, form.Timer1.Interval);
        Assert.True(form.Timer1.Enabled);
        Assert.NotNull(form.SList_320);
        Assert.Equal(0, FdbExploreSeam.g_nClearIndex);
        Assert.Equal(0, FdbExploreSeam.g_nClearCount);
        Assert.Equal(0, FdbExploreSeam.g_nClearItemIndexCount);
    }

    [Fact]
    public void 窗体FormDestroy_释放SList_320()
    {
        using var form = new FrmFDBExplore();
        form.FormCreate(null, EventArgs.Empty);
        form.FormDestroy(null, EventArgs.Empty);
        Assert.NotNull(form.SList_320);
    }

    [Fact]
    public void 窗体BtnBlankCountClick_清空两个列表()
    {
        using var form = new FrmFDBExplore();
        form.ListBox1.Items.Add("a");
        form.ListBox2.Items.Add("1");

        form.BtnBlankCountClick(null, EventArgs.Empty);

        Assert.Equal(0, form.ListBox1.Items.Count);
        Assert.Equal(0, form.ListBox2.Items.Count);
    }

    [Fact]
    public void 窗体BtnAutoCleanClick_标题随开关切换()
    {
        using var form = new FrmFDBExplore();
        form.BtnAutoCleanClick(null, EventArgs.Empty);
        Assert.Equal("自动清理", form.BtnAutoClean.Text);
        form.BtnAutoCleanClick(null, EventArgs.Empty);
        Assert.Equal("已停止清理", form.BtnAutoClean.Text);
    }

    [Fact]
    public void 窗体DFM_控件几何与文案对齐()
    {
        using var form = new FrmFDBExplore();
        Assert.Equal("人物数据管理", form.Text);
        Assert.Equal(new System.Drawing.Size(577, 238), form.ClientSize);
        Assert.Equal("搜索人物数据:", form.Label1.Text);
        Assert.Equal(new System.Drawing.Point(16, 8), form.ListBox1.Location);
        Assert.Equal(new System.Drawing.Point(112, 8), form.ListBox2.Location);
        Assert.Equal(new System.Drawing.Point(16, 168), form.EdFind.Location);
        Assert.Equal("新建人物数据", form.BtnAdd.Text);
        Assert.Equal("删除人物数据", form.BtnDel.Text);
        Assert.Equal("重建数据库", form.BtnRebuild.Text);
        Assert.Equal("清空搜索结果(&C)", form.BtnBlankCount.Text);
        Assert.Equal("自动清理数据", form.GroupBox1.Text);
        Assert.Equal("自动清理", form.BtnAutoClean.Text);
        Assert.Equal("1级以下人物(1星期)", form.CkLv1.Text);
        Assert.Equal("7级以下人物(1个月)", form.CkLv7.Text);
        Assert.Equal("14级以下人物 (4个月)", form.CkLv14.Text);
        Assert.True(form.CkLv1.Checked);
        Assert.True(form.CkLv7.Checked);
        Assert.True(form.CkLv14.Checked);
        Assert.Equal("复制人物数据", form.BtnCopyRcd.Text);
        Assert.Equal("复制到新人物", form.BtnCopyNew.Text);
        Assert.Equal(3000, form.Timer1.Interval);          // DFM: Timer1.Interval=3000
        Assert.False(form.Timer1.Enabled);                 // DFM: Enabled=False
    }

    [Fact]
    public void 窗体Timer1Timer_转发到逻辑并写回名称()
    {
        var db = new MemoryHumDataDB();
        db.Records[0] = TestReset.MakeInfo("old", D(2025, 1, 1), 1);
        FdbExploreSeam.g_HumDataDB = db;
        FdbExploreSeam.boAutoClearDB = 1;
        FdbExploreSeam.Now = () => D(2025, 3, 15);
        FdbExploreSeam.nDay1 = 7;
        FdbExploreSeam.nLevel1 = 5;
        var deleted = new List<string>();
        FdbExploreSeam.FrmDBSrv_DelHum = n => deleted.Add(n);

        using var form = new FrmFDBExplore();
        form.CkLv1.Checked = true;
        form.CkLv7.Checked = false;
        form.CkLv14.Checked = false;
        form.Timer1Timer(null, EventArgs.Empty);

        Assert.Equal(new List<string> { "old" }, deleted);
    }
}

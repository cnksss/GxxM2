using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J3：OnlineMsg.pas + dlgSearchText/dlgReplaceText/dlgConfirmReplace 族 1:1 转换测试。
/// OnlineMsgForm 测试体整体在单 STA 线程执行（窗体构造/操作/断言/销毁同线程，规避跨线程句柄边缘行为）。
/// </summary>
public sealed class FormOnlineMsgTests : IDisposable
{
    private readonly string _dir;
    private readonly TUserEngine _engine = new();

    public FormOnlineMsgTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j3b_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetGeneralDefaults();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        OnlineMsgControl.ResetDefaults();
    }

    public void Dispose()
    {
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private string MsgListPath => Path.Combine(_dir, "MsgList.txt");

    /// <summary>Delphi StrListFile := '.\MsgList.txt'；测试注入工作目录，整测试体单 STA 线程。</summary>
    private void Sta(Action<TUserEngine> body) => StaRunner.New(() => body(_engine));

    [StaFact]
    public void FormCreate_CreatesMsgListTxtWhenMissing()
        => Sta(engine =>
        {
            using var form = new OnlineMsgForm(engine, _dir);
            Assert.True(File.Exists(MsgListPath));
            Assert.Equal(0, form.GridRowCount);
        });

    [StaFact]
    public void FormCreate_LoadsExistingMsgListTxt()
    {
        File.WriteAllLines(MsgListPath, new[] { "全服双倍经验", "全服掉率翻倍" }, System.Text.Encoding.GetEncoding(936));
        Sta(engine =>
        {
            using var form = new OnlineMsgForm(engine, _dir);
            Assert.Equal(2, form.GridRowCount);
            Assert.Equal("全服双倍经验", form.Grid[0, 0].Value);
        });
    }

    [StaFact]
    public void ButtonAdd_AppendsAndSaves()
        => Sta(engine =>
        {
            using var form = new OnlineMsgForm(engine, _dir);
            form.ComboBoxMsg.Text = "攻城战即将开始";
            form.ButtonAddClick(form);
            Assert.Equal(1, form.GridRowCount);
            Assert.False(form.ButtonAdd.Enabled);
            Assert.Contains("攻城战即将开始", File.ReadAllLines(MsgListPath, System.Text.Encoding.GetEncoding(936)));
        });

    [StaFact]
    public void ButtonDelete_RemovesSelectedRow()
    {
        File.WriteAllLines(MsgListPath, new[] { "甲", "乙" }, System.Text.Encoding.GetEncoding(936));
        Sta(engine =>
        {
            using var form = new OnlineMsgForm(engine, _dir);
            form.ButtonDeleteClick(form); // 无选择时 Delphi StringGrid.Row=0 等效
            Assert.Equal(1, form.GridRowCount);
            Assert.Equal("乙", form.Grid[0, 0].Value);
        });
    }

    [StaFact]
    public void btnSend_SavesFlagsIniAndBroadcasts()
        => Sta(engine =>
        {
            var p1 = engine.AddPlayer("玩家一");
            var p2 = engine.AddPlayer("玩家二");
            p2.m_boDeath = true; // 死亡玩家不接收

            using var form = new OnlineMsgForm(engine, _dir);
            form.chkDisableTrading.Checked = true;
            form.chkDisableShop.Checked = true;
            form.ComboBoxMsg.Text = "全服公告测试";
            form.btnSendClick(form);

            // boSave + 活动标志 1:1
            Assert.True(OnlineMsgControl.g_OnlineMsgControl.boSaveDisableTrading);
            Assert.True(OnlineMsgControl.g_OnlineMsgControl.boSaveDisableShop);
            Assert.True(OnlineMsgControl.g_OnlineMsgControl.boDisableTrading);

            var ini = new TFastIniFile(Path.Combine(_dir, "!Setup.txt"));
            Assert.True(ini.ReadBool("OnlineMsgControl", "DisableTrading", false));
            Assert.True(ini.ReadBool("OnlineMsgControl", "DisableShop", false));
            Assert.False(ini.ReadBool("OnlineMsgControl", "DisableRepair", true));

            // 广播：存活玩家收到原始消息（前缀由客户端/记录层附加），死亡玩家不收
            Assert.Single(p1.SysMsgs);
            Assert.Equal("全服公告测试", p1.SysMsgs[0]);
            Assert.Empty(p2.SysMsgs);
            Assert.Contains("〖系统〗全服公告测试", form.MemoMsg.Text);
        });

    [StaFact]
    public void btnSend_EmptyMsg_NoBroadcast()
        => Sta(engine =>
        {
            var p1 = engine.AddPlayer("玩家一");
            using var form = new OnlineMsgForm(engine, _dir);
            form.ComboBoxMsg.Text = "   ";
            form.btnSendClick(form);
            Assert.Empty(p1.SysMsgs);
        });

    [StaFact]
    public void btnSend_AutoRun_EnablesTimerWithInterval()
        => Sta(engine =>
        {
            var p1 = engine.AddPlayer("玩家一");
            using var form = new OnlineMsgForm(engine, _dir);
            form.chkAutoRun.Checked = true;
            form.seAutRunInterval.Value = 3;
            form.ComboBoxMsg.Text = "循环消息";
            form.btnSendClick(form);
            Assert.True(form.tmrRun.Enabled);
            Assert.Equal(3000, form.tmrRun.Interval);

            form.tmrRunTimer(form); // 手动触发一轮
            Assert.Equal(2, p1.SysMsgs.Count);
        });

    [StaFact]
    public void ShowForm_FinallyResetsActiveFlags()
    {
        OnlineMsgControl.g_OnlineMsgControl.boDisableTrading = true;
        Sta(engine =>
        {
            using var form = new OnlineMsgForm(engine, _dir);
            form.Open(showModal: false);
        });
        // ShowFrmOnlineMsg finally 段 1:1：活动标志复位
        Assert.False(OnlineMsgControl.g_OnlineMsgControl.boDisableTrading);
    }

    [StaFact]
    public void MemoChange_ClearsWhenOver80Lines()
        => Sta(engine =>
        {
            using var form = new OnlineMsgForm(engine, _dir);
            for (int i = 0; i < 85; i++) form.MemoMsg.AppendText("行" + i + "\r\n");
            // Delphi OnChange 语义：每次追加都触发，>80 行时清空后继续追加
            form.MemoMsgChange(form);
            Assert.Equal("行80\r\n行81\r\n行82\r\n行83\r\n行84\r\n", form.MemoMsg.Text);
        });
}

/// <summary>SynEdit 搜索/替换对话框族（dlgSearchText/dlgReplaceText/dlgConfirmReplace）。</summary>
public sealed class SearchReplaceDialogTests
{
    [StaFact]
    public void SearchDialog_PropertiesRoundTrip()
    {
        StaRunner.New(() =>
        {
            using var dlg = new TextSearchDialog();
            dlg.SearchText = "火球术";
            Assert.Equal("火球术", dlg.SearchText);
            dlg.SearchBackwards = true;
            Assert.True(dlg.SearchBackwards);
            dlg.SearchCaseSensitive = true;
            Assert.True(dlg.SearchCaseSensitive);
            dlg.SearchFromCursor = true;
            Assert.True(dlg.SearchFromCursor);
            dlg.SearchInSelectionOnly = true;
            Assert.True(dlg.SearchInSelectionOnly);
            dlg.SearchWholeWords = true;
            Assert.True(dlg.SearchWholeWords);
            dlg.SearchRegularExpression = true;
            Assert.True(dlg.SearchRegularExpression);
        });
    }

    [StaFact]
    public void SearchDialog_HistoryCap10AndJoin()
    {
        StaRunner.New(() =>
        {
            using var dlg = new TextSearchDialog();
            var items = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l" };
            dlg.SearchTextHistory = string.Join("\r\n", items);
            string hist = dlg.SearchTextHistory;
            Assert.Equal(10, hist.Split("\r\n").Length);
            Assert.Equal("a\r\nb\r\nc\r\nd\r\ne\r\nf\r\ng\r\nh\r\ni\r\nj", hist);
        });
    }

    [StaFact]
    public void SearchDialog_CloseQueryMovesTextToTop()
    {
        StaRunner.New(() =>
        {
            using var dlg = new TextSearchDialog();
            dlg.SearchTextHistory = "a\r\nb\r\nc";
            dlg.SearchText = "b";
            dlg.ModalResult = System.Windows.Forms.DialogResult.OK;
            dlg.FormCloseQuery(out bool canClose);
            Assert.True(canClose);
            Assert.Equal(3, dlg.cbSearchText.Items.Count);
            Assert.Equal("b", dlg.cbSearchText.Items[0]); // 已存在项移到顶部
            Assert.Equal("b", dlg.cbSearchText.Text);
        });
    }

    [StaFact]
    public void SearchDialog_CloseQueryNewTextInsertedTop()
    {
        StaRunner.New(() =>
        {
            using var dlg = new TextSearchDialog();
            dlg.SearchTextHistory = "a\r\nb";
            dlg.SearchText = "新词";
            dlg.ModalResult = System.Windows.Forms.DialogResult.OK;
            dlg.FormCloseQuery(out _);
            Assert.Equal(3, dlg.cbSearchText.Items.Count);
            Assert.Equal("新词", dlg.cbSearchText.Items[0]);
        });
    }

    [StaFact]
    public void ReplaceDialog_PropertiesAndHistory()
    {
        StaRunner.New(() =>
        {
            using var dlg = new TextReplaceDialog();
            dlg.ReplaceText = "新名字";
            Assert.Equal("新名字", dlg.ReplaceText);
            dlg.ReplaceTextHistory = "x\r\ny\r\nz";
            Assert.Equal("x\r\ny\r\nz", dlg.ReplaceTextHistory);
        });
    }

    [StaFact]
    public void ReplaceDialog_CloseQueryMovesReplaceToTop()
    {
        StaRunner.New(() =>
        {
            using var dlg = new TextReplaceDialog();
            dlg.ReplaceTextHistory = "x\r\ny";
            dlg.ReplaceText = "y";
            dlg.ModalResult = System.Windows.Forms.DialogResult.OK;
            dlg.FormCloseQuery(out _);
            Assert.Equal("y", dlg.cbReplaceText.Items[0]);
        });
    }

    [StaFact]
    public void ConfirmReplace_PrepareShowFormatCaption()
    {
        StaRunner.New(() =>
        {
            using var dlg = new ConfirmReplaceDialog();
            dlg.PrepareShow(new System.Drawing.Rectangle(10, 10, 300, 200), 50, 20, 60, "木剑");
            Assert.Equal("是否要对 \"木剑\" 进行替换?", dlg.lblConfirmation.Text);
        });
    }
}

/// <summary>TUserEngine 测试辅助（Delphi m_PlayObjectList 注入等效）。</summary>
internal static class OnlineMsgTestExt
{
    public static TPlayObject AddPlayer(this TUserEngine engine, string name)
    {
        var p = new TPlayObject { m_sCharName = name };
        engine.PlayObjects.Add(p);
        return p;
    }
}

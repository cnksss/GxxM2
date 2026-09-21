// ============================================================================
// 车道 p10-db-login-forms —— GHeroDBConfig.pas（565 行）1:1 移植的验收测试
//
// 覆盖：
//   · 每个 public 成员（Open / CheckHeroDB / 6 个事件处理例程 / SelectDirectory / SelectDirCB）
//   · 每个原文缺陷（逐条差异断言锁定）
//   · 边界（缺 DB 文件、空 ListBox、TabVisible 组合、空 EditHeroDB/EditHeroDBPath、尾部反斜杠）
//   · DFM 对账（object 计数 / 事件绑定计数，全部**计数取证**）
//
// 禁止：真实网络、真实数据路径、阻塞式对话框。全部经 GameCenterTestBase 的临时目录 +
//       注入接缝（HeroDBFactory / FolderPickerProvider / GameCenterDialogs / ShowModalHandler）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.GameCenter.Forms;
using Xunit;
using static GXX.GameCenter.GShareGlobals;

namespace GXX.GameCenter.Forms.Tests;

/// <summary>GHeroDBConfig 窗体行为测试（触碰 GShare 全局 ⇒ 禁止并行）。</summary>
[Collection("GameCenterSequential")]
public sealed class P10HeroDbConfigTests : GXX.GameCenter.Tests.GameCenterTestBase
{
    private FakeHeroDB _db = null!;

    /// <summary>
    /// MemoLog1 的"最长快照"。原因：<c>ButtonCreateStdItemsFieldClick</c> 末尾的
    /// <c>CheckHeroDB</c>（原文 :489）开头会 <c>MemoLog1.Lines.Clear</c>，
    /// 于是方法返回后 Log1 已空 —— 要断言逐字日志就必须在过程中抓快照。
    /// </summary>
    private List<string> _memo1Snapshot = new();

    /// <summary>MemoLog2 的最长快照（同理：Monster 按钮末尾的 CheckHeroDB 会清空它）。</summary>
    private List<string> _memo2Snapshot = new();

    /// <summary>MemoLog3 的最长快照（同理：Magic 按钮末尾的 CheckHeroDB 会清空它）。</summary>
    private List<string> _memo3Snapshot = new();

    private void CaptureMemo1(System.Windows.Forms.TextBox memo) => _memo1Snapshot = Longer(memo, _memo1Snapshot);

    private void CaptureMemo2(System.Windows.Forms.TextBox memo) => _memo2Snapshot = Longer(memo, _memo2Snapshot);

    private void CaptureMemo3(System.Windows.Forms.TextBox memo) => _memo3Snapshot = Longer(memo, _memo3Snapshot);

    private static List<string> Longer(System.Windows.Forms.TextBox memo, List<string> current)
    {
        if (memo.TextLength == 0) return current;
        var lines = memo.Lines.ToList();
        while (lines.Count > 0 && lines[lines.Count - 1].Length == 0) lines.RemoveAt(lines.Count - 1);
        if (lines.Count < current.Count) return current;
        // 文本被 MaxLength 截断时，行数可能不变而末尾行被截短 —— 取"更长的那个"。
        string a = string.Join("\n", lines);
        string b = string.Join("\n", current);
        return a.Length >= b.Length ? lines : current;
    }

    public P10HeroDbConfigTests()
    {
        _db = new FakeHeroDB();
        HeroDBFactory.InstanceFactory = () => _db;
    }

    // ==================================================================
    // 内部辅助
    // ==================================================================

    /// <summary>建"三个 DB 文件齐全"的目录，返回目录绝对路径。</summary>
    private static string MakeDbDir(FakeHeroDB fake, string baseDir, string tableAlias = "StdItems")
    {
        string dir = Path.Combine(baseDir, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, tableAlias + ".DB"), "");
        File.WriteAllText(Path.Combine(dir, "Monster.DB"), "");
        File.WriteAllText(Path.Combine(dir, "Magic.DB"), "");
        return dir;
    }

    private static List<string> Items(System.Windows.Forms.ListBox box)
        => box.Items.Cast<object>().Select(o => Convert.ToString(o) ?? "").ToList();

    // ==================================================================
    // Open（:378-383）
    // ==================================================================

    [Fact]
    public void Open_LoadsGlobalsIntoEditsPathAndCallsShowModalSeam()
    {
        FormSta.Run(() =>
        {
            g_sHeroDBName = "MyHeroDB";
            g_sGameDirectory = Dir + Path.DirectorySeparatorChar;

            bool shown = false;
            TFrmHeroDB.ShowModalHandler = _ => { shown = true; return true; };
            try
            {
                using var f = new TFrmHeroDB();
                f.Open();
                Assert.Equal("MyHeroDB", f.EditHeroDB.Text);
                // 原文 :381：g_sGameDirectory + 'Mud2\DB'（依赖全局自带尾分隔符，此处不 Trim/不补）
                Assert.Equal(g_sGameDirectory + "Mud2\\DB", f.EditHeroDBPath.Text);
                Assert.True(shown);
            }
            finally { TFrmHeroDB.ShowModalHandler = null; }
        });
    }

    [Fact]
    public void Open_ShowModalSeamDefault_DoesNotBlockAndReturnsFalse()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            Assert.False(f.ShowModalEquivalent());   // 默认无接缝 ⇒ 不弹窗、返回 false
        });
    }

    // ==================================================================
    // CheckHeroDB（:165-376）
    // ==================================================================

    [Fact]
    public void CheckHeroDB_CompleteDatabase_ReturnsFalseAndAllTabsHidden()
    {
        _db = FieldSets.CompleteFake();
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();   // 原文窗体是**显示中**的；CheckHeroDB:338 的 Control.Visible 语义只有在显示时才成立
            Assert.False(f.CheckHeroDB());            // false = 无缺失
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet1));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet2));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet3));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet4));
            Assert.Empty(f.MemoLogLines);
            Assert.Empty(Items(f.ListBoxStdItems));
            Assert.Empty(Items(f.ListBoxMonster));
            Assert.Empty(Items(f.ListBoxMagic));
            Assert.Empty(f.MemoLog1Lines);
            Assert.Empty(f.MemoLog2Lines);
            Assert.Empty(f.MemoLog3Lines);
            Assert.Equal(1, _db.DisposeCount);        // HeroDB.Free（:375）
        });
    }

    [Fact]
    public void CheckHeroDB_MissingAlias_OnlyTabSheet1AndSingleMessage()
    {
        _db.AliasExists = false;                      // HeroDBExist = False（:192）
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            Assert.True(f.CheckHeroDB());
            Assert.Equal(new[] { g_sHeroDBName + "配置错误！" }, f.MemoLogLines);
            Assert.True(TFrmHeroDB.GetTabVisible(f.TabSheet1));
            // :198 的 `if not TabSheet1.TabVisible` 守卫 ⇒ 表检查整段跳过
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet2));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet3));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet4));
            Assert.Empty(Items(f.ListBoxStdItems));
        });
    }

    [Theory]
    [InlineData("StdItems", "配置错误,没有发现“StdItems.DB”！")]
    [InlineData("Monster", "配置错误,没有发现“Monster.DB”！")]
    [InlineData("Magic", "配置错误,没有发现“Magic.DB”！")]
    public void CheckHeroDB_MissingTable_LocksMessageAndTabSheet1(string table, string expected)
    {
        _db = FieldSets.CompleteFake();
        _db.Tables.Remove(table);                     // TableExist = False（:199/:204/:209）
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            Assert.True(f.CheckHeroDB());
            Assert.Equal(new[] { g_sHeroDBName + expected }, f.MemoLogLines);
            Assert.True(TFrmHeroDB.GetTabVisible(f.TabSheet1));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet2));
        });
    }

    [Fact]
    public void CheckHeroDB_MissingStdItemsFields_ListIsCompleteAndOrdered()
    {
        _db = FieldSets.CompleteFake();
        // 顺序 = 原文检查顺序：14 具名 → 24 个 Element → InsuranceCurrency/InsuranceGold
        var removed = new[] { "Color", "OverLap", "HP", "Expand5", "Element1", "Element24", "InsuranceCurrency" };
        foreach (string r in removed) _db.RemoveField("StdItems", r);
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            Assert.True(f.CheckHeroDB());
            Assert.Equal(removed, Items(f.ListBoxStdItems));
            Assert.True(TFrmHeroDB.GetTabVisible(f.TabSheet2));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet1));
            Assert.Empty(f.MemoLogLines);
            // :311 的 `if not TabSheet2.TabVisible` 守卫 ⇒ Monster/Magic 整段跳过
            Assert.Empty(Items(f.ListBoxMonster));
            Assert.Empty(Items(f.ListBoxMagic));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet3));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet4));
        });
    }

    [Fact]
    public void CheckHeroDB_StdItemsCheckCoversExactly38Fields()
    {
        _db = FieldSets.CompleteFake();
        // 全部 38 个 StdItems 字段都拿掉 ⇒ ListBox 必须逐字列出 38 项（上界 1..24 / 1..5 计数证据）
        foreach (string field in FieldSets.StdItemsCheckOrder()) _db.RemoveField("StdItems", field);
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            Assert.True(f.CheckHeroDB());
            var items = Items(f.ListBoxStdItems);
            Assert.Equal(38, items.Count);
            Assert.Equal(FieldSets.StdItemsCheckOrder(), items);
            // 原文 :290 是 `for I := 1 to 24`：Element24 存在、Element25 不参与检查
            Assert.Contains("Element1", items);
            Assert.Contains("Element24", items);
            Assert.DoesNotContain("Element25", items);
        });
    }

    [Fact]
    public void CheckHeroDB_MissingMonsterFields_LocksFourFieldMessages()
    {
        _db = FieldSets.CompleteFake();
        foreach (string m in FieldSets.Monster) _db.RemoveField("Monster", m);
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            Assert.True(f.CheckHeroDB());
            Assert.Equal(FieldSets.Monster, Items(f.ListBoxMonster));
            Assert.True(TFrmHeroDB.GetTabVisible(f.TabSheet3));
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet4));
        });
    }

    [Fact]
    public void CheckHeroDB_MagicLoopGeneratesNeedL1To15AndTrain1To15()
    {
        _db = FieldSets.CompleteFake();
        foreach (string m in FieldSets.Magic()) _db.RemoveField("Magic", m);
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            Assert.True(f.CheckHeroDB());
            var items = Items(f.ListBoxMagic);
            Assert.Equal(33, items.Count);                       // 15*2 + 3
            Assert.Equal("NeedL1", items[0]);
            Assert.Equal("L1Train", items[1]);
            Assert.Equal("NeedL15", items[28]);
            Assert.Equal("L15Train", items[29]);
            Assert.Equal(new[] { "MaxTrainLv", "CanUpgrade", "MaxUpgradeLv" }, items.Skip(30));
            // :340 是 `for I := 0 to 14` + I+1 ⇒ 不存在 NeedL0 / NeedL16
            Assert.DoesNotContain("NeedL0", items);
            Assert.DoesNotContain("NeedL16", items);
            Assert.DoesNotContain("L0Train", items);
            Assert.DoesNotContain("L16Train", items);
            Assert.True(TFrmHeroDB.GetTabVisible(f.TabSheet4));
        });
    }

    // ==================================================================
    // 原文缺陷 1：:338 的 TabSheet3.Visible（Control.Visible）守卫
    // ==================================================================

    [Fact]
    public void CheckHeroDB_TabSheet3VisibleGuardIsDead_MagicChecksRunAnyway()
    {
        // 场景：StdItems 齐（TabSheet2 不点亮）⇒ 进入 :311 的 else 分支；
        // 同时 Monster 也缺字段 ⇒ :335 已经把 TabSheet3.TabVisible 置 True。
        // 原文 :338 查的却是 Control.Visible（TabSheet3 不是被选中页 ⇒ 恒 False）
        // ⇒ :340 的 Magic 字段检查**照样执行**。这是原文缺陷，逐字保留。
        //
        // 实测结论（D-P10-20）：WinForms 的 TabPage.Visible 在"页签被程序化摘除/追加"之后
        // 并不可靠（会与被选中页脱钩），因此本用例**以可见后果**取证：
        // Magic 检查确实跑了（ListBoxMagic 非空），而不是拿一个不可靠属性当真值。
        _db = FieldSets.CompleteFake();
        foreach (string m in FieldSets.Monster) _db.RemoveField("Monster", m);
        _db.RemoveField("Magic", "NeedL1");
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            Assert.True(f.CheckHeroDB());

            Assert.True(TFrmHeroDB.GetTabVisible(f.TabSheet3));     // TabVisible 已是 True
            Assert.False(f.TabSheet3.Visible);                      // 但 Control.Visible 仍是 False（ActivePage 恒为 TabSheet2）
            Assert.Equal(FieldSets.Monster, Items(f.ListBoxMonster));
            // 关键：Monster 刚被判定缺字段，Magic 检查仍被执行（原文 :338 的 TabSheet3.Visible 拦不住）
            Assert.Equal(new[] { "NeedL1" }, Items(f.ListBoxMagic));
            Assert.True(TFrmHeroDB.GetTabVisible(f.TabSheet4));
        });
    }

    [Fact]
    public void TabPage_VisibleOnlyForSelectedPage()
    {
        // 前提固化（WinForms 实测）：**未经**程序化摘除/追加时，TabPage.Visible 只对当前选中页为 True。
        // 原文缺陷 1（:338 用 Control.Visible 而不是 TabVisible）之所以是死条件，根因就在这里。
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            System.Windows.Forms.Application.DoEvents();
            Assert.Same(f.TabSheet2, f.PageControl.SelectedTab);   // DFM: ActivePage = TabSheet2
            Assert.True(f.TabSheet2.Visible);
            Assert.False(f.TabSheet1.Visible);
            Assert.False(f.TabSheet3.Visible);
            Assert.False(f.TabSheet4.Visible);
        });
    }

    [Fact]
    public void TabPage_VisibleBecomesUnreliableAfterProgrammaticTabRemoval()
    {
        // 前提固化 2（偏差 D-P10-19/D-P10-20 的取证）：TabSheet3.TabVisible := False 会把它
        // 从 TabControl.Controls 摘除，之后再 TabVisible := True 追加回去时，WinForms 会把
        // TabPage.Visible 与"当前选中页"脱钩（Delphi 的 TabVisible/Visible 是互不干扰的两个属性）。
        // 结论：托管侧**不得**把 TabPage.Visible 当作 ActivePage 的可靠判据 —— 原文缺陷 1 的
        // 语义改由"Magic 检查是否执行"这条可见后果锁定（见上一个用例）。
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            f.SetTabVisible(f.TabSheet3, false);
            Assert.False(f.TabSheet3.Visible);
            f.SetTabVisible(f.TabSheet3, true);
            Assert.True(TFrmHeroDB.GetTabVisible(f.TabSheet3));     // TabVisible 恢复
            Assert.False(ReferenceEquals(f.PageControl.SelectedTab, f.TabSheet3));  // 但 ActivePage 没变
            Assert.True(f.TabSheet3.Visible);                       // ← Visible 已不可靠（不是被选中页却为 True）
        });
    }

    [Fact]
    public void TabPage_VisibleSetterDoesNotHideTab()
    {
        // 前提 2：WinForms 里设置 TabPage.Visible **不会**把页签从页签行摘除，
        // 因此 TabVisible 只能用"摘除/追加 TabControl.Controls"来等价实现（偏差 D-P10-19）。
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            int tabsBefore = f.PageControl.TabCount;
            f.TabSheet1.Visible = false;                       // 直接写 Control.Visible：
            Assert.Equal(tabsBefore, f.PageControl.TabCount);  // 页签数量不变
            Assert.Contains(f.TabSheet1, f.PageControl.Controls.Cast<System.Windows.Forms.Control>());

            f.SetTabVisible(f.TabSheet1, false);               // 垫片：
            Assert.Equal(tabsBefore - 1, f.PageControl.TabCount);   // 页签确实消失
            Assert.DoesNotContain(f.TabSheet1, f.PageControl.Controls.Cast<System.Windows.Forms.Control>());
            Assert.False(TFrmHeroDB.GetTabVisible(f.TabSheet1));
        });
    }

    // ==================================================================
    // EditHeroDBPathButtonClick（:112-130）
    // ==================================================================

    [Fact]
    public void EditHeroDBPathButtonClick_PickerCancelled_DoesNothing()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.EditHeroDBPath.Text = "原值";
            FolderPicker.Cancels();
            try
            {
                f.EditHeroDBPathButtonClick(f);
                Assert.Equal("原值", f.EditHeroDBPath.Text);   // 未选中 ⇒ 不写回
                Assert.Empty(f.MemoLogLines);
            }
            finally { FolderPicker.Restore(); }
        });
    }

    [Fact]
    public void EditHeroDBPathButtonClick_StripsSingleTrailingBackslash_AndLogsThreeMisses()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            string missing = Path.Combine(Dir, "no_such_dir");
            FolderPicker.Returns(missing + "\\");              // 原文 :117-118 只剥**一个**尾部反斜杠
            try
            {
                f.EditHeroDBPathButtonClick(f);
                Assert.Equal(missing, f.EditHeroDBPath.Text);
                Assert.Equal(
                    new[]
                    {
                        "当前目录中没有发现“StdItems.DB”",
                        "当前目录中没有发现“Monster.DB”",
                        "当前目录中没有发现“Magic.DB”",
                    },
                    f.MemoLogLines);
                Assert.Equal(System.Drawing.Color.Red, f.MemoLog.ForeColor);   // :120 MemoLog.Font.Color := clRed
            }
            finally { FolderPicker.Restore(); }
        });
    }

    [Fact]
    public void EditHeroDBPathButtonClick_OnlyMissingOneFile_LogsOnlyThatOne()
    {
        string dir = MakeDbDir(_db, Dir);
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            FolderPicker.Returns(dir);
            try
            {
                f.EditHeroDBPathButtonClick(f);
                Assert.Equal(dir, f.EditHeroDBPath.Text);
                Assert.Empty(f.MemoLogLines);                   // 三个都在 ⇒ 无告警
            }
            finally { FolderPicker.Restore(); }
        });
    }

    [Fact]
    public void EditHeroDBPathButtonClick_EmptyStringFromPicker_IsWrittenVerbatim()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            FolderPicker.Returns("");                            // 选中了"空路径"（异常但合法输入）
            try
            {
                f.EditHeroDBPathButtonClick(f);
                Assert.Equal("", f.EditHeroDBPath.Text);
                // sFilePath = '' ⇒ FileExists('\StdItems.DB') 等一律 False ⇒ 三条告警
                Assert.Equal(3, f.MemoLogLines.Count);
            }
            finally { FolderPicker.Restore(); }
        });
    }

    // ==================================================================
    // ButtonSaveHeroDBConfigClick（:132-163）
    // ==================================================================

    [Fact]
    public void ButtonSaveHeroDBConfigClick_MissingFiles_ExitsBeforeCreatingHeroDB()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.EditHeroDB.Text = "  NewHero  ";                   // :139 Trim
            f.EditHeroDBPath.Text = Path.Combine(Dir, "no_such_dir");
            f.ButtonSaveHeroDBConfigClick(f);

            // :139 在文件检查**之前**就已经改了全局量
            Assert.Equal("NewHero", g_sHeroDBName);
            Assert.Equal(3, f.MemoLogLines.Count);
            Assert.Empty(_db.SavedConfigFiles);                  // :150 Exit ⇒ 根本没建 THeroDB
            Assert.Equal(0, _db.DisposeCount);
            Assert.False(g_boHeroDBOK);                          // g_boHeroDBOK 保持初始 False（ResetForTests）
        });
    }

    [Fact]
    public void ButtonSaveHeroDBConfigClick_Success_WritesAliasAndIniAndSetsOk()
    {
        string dir = MakeDbDir(_db, Dir);
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.EditHeroDB.Text = " MyHeroDB ";
            f.EditHeroDBPath.Text = dir + "\\";                  // :141-142 剥尾反斜杠
            f.ButtonSaveHeroDBConfigClick(f);

            Assert.Equal("MyHeroDB", g_sHeroDBName);
            Assert.Single(_db.SavedConfigFiles);
            Assert.Equal(("MyHeroDB", dir), _db.SavedConfigFiles[0]);      // :153 不带尾反斜杠
            // _db 被用了两次（:152 SaveHeroDBConfigFile + :158 CheckHeroDB）⇒ Dispose 计数 2（原文各 Free 一次）
            Assert.Equal(2, _db.DisposeCount);
            Assert.Equal("MyHeroDB", g_IniConf!.ReadString("GameConf", "HeroDBName", ""));   // :156

            Assert.True(g_boHeroDBOK);                                     // :158 not CheckHeroDB
            Assert.Equal("数据库更新成功！！！", f.LastMessageBoxText);       // :160
            Assert.Equal("提示信息", f.LastMessageBoxCaption);
            Assert.Equal(0x00 + 0x30, f.LastMessageBoxFlags);              // MB_OK + MB_ICONWARNING
            Assert.True(f.CloseCalled);                                    // :161 Close
        });
    }

    [Fact]
    public void ButtonSaveHeroDBConfigClick_WhenFieldsStillMissing_NoMessageBoxNoClose()
    {
        string dir = MakeDbDir(_db, Dir);
        _db.RemoveField("StdItems", "Color");
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.EditHeroDB.Text = "HeroDB";
            f.EditHeroDBPath.Text = dir;
            f.ButtonSaveHeroDBConfigClick(f);

            Assert.False(g_boHeroDBOK);                    // :158 → CheckHeroDB 为 True ⇒ OK=False
            Assert.Null(f.LastMessageBoxText);
            Assert.False(f.CloseCalled);
        });
    }

    [Fact]
    public void ButtonSaveHeroDBConfigClick_UnwiredHeroDBFactory_ThrowsNotWired()
    {
        string dir = MakeDbDir(_db, Dir);
        HeroDBFactory.ResetForTests();                     // 台账 §25.2：默认必须显式抛"未接线"
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.EditHeroDB.Text = "HeroDB";
            f.EditHeroDBPath.Text = dir;
            var ex = Assert.Throws<NotWiredException>(() => f.ButtonSaveHeroDBConfigClick(f));
            Assert.Contains("未接线", ex.Message);
            Assert.Contains("GHeroDB.pas 未移植", ex.Message);
        });
    }

    // ==================================================================
    // ButtonCreateStdItemsFieldClick（:391-494）
    // ==================================================================

    [Fact]
    public void ButtonCreateStdItemsFieldClick_UnwiredFactory_Throws()
    {
        HeroDBFactory.ResetForTests();
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            Assert.Throws<NotWiredException>(() => f.ButtonCreateStdItemsFieldClick(f));
        });
    }

    [Fact]
    public void ButtonCreateStdItemsFieldClick_AllFail_Logs38FailuresAndReenablesButton()
    {
        _db.CreateFieldResult = false;                     // 全部走 else 分支
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            f.MemoLog1.TextChanged += (s, e) => CaptureMemo1(f.MemoLog1);
            f.ButtonCreateStdItemsFieldClick(f);

            // 原文 :394 先 MemoLog1.Clear；但 :489 的 CheckHeroDB 开头会**再次** Clear 所有 Memo，
            // 所以方法返回后 Log1 已被清空 —— 这里用"调用过程中的最长快照"断言。
            var lines = _memo1Snapshot;
            // 计数取证：14 具名（含 InsuranceCurrency/InsuranceGold）+ 24 个 'ElementI' = **38 行**
            //（CreateField 共 40 次：:400-:472 的 14 次 + :464/:469 的 2 次 + :476 循环 24 次）
            // 行序 = 原文检查顺序：14 具名 → 24 元素（原文 :474 的循环在 :464/:469 之后）。
            Assert.Equal(38, lines.Count);
            Assert.Equal("Color字段创建失败", lines[0]);
            Assert.Equal("OverLap字段创建失败", lines[1]);
            Assert.Equal("Element字段创建失败", lines[6]);
            Assert.Equal("Expand5字段创建失败", lines[11]);
            Assert.Equal("InsuranceCurrency字段创建失败", lines[12]);
            Assert.Equal("InsuranceGold字段创建失败", lines[13]);
            Assert.Equal("Element1字段创建失败", lines[14]);
            Assert.Equal("Element24字段创建失败", lines[37]);

            // CreateField 的（值/长度）规则逐字比对（共 38 次调用：:400-:472 的 14 次 + :474 循环 24 次）
            var created = _db.CreatedFields;
            Assert.Equal(38, created.Count);
            // 逐字快照 "字段:默认值:长度"（避免元组里 object 装箱的 int/byte 比较歧义）
            Assert.Equal(
                new[]
                {
                    "Color:255:2", "OverLap:0:2", "HP:0:4", "MP:0:4", "Light:0:4", "Horse:0:4",
                    "Element:0:2", "Expand1:0:4", "Expand2:0:4", "Expand3:0:4", "Expand4:0:4",
                    "Expand5:0:4", "InsuranceCurrency:0:4", "InsuranceGold:0:4",
                    "Element1:0:2", "Element2:0:2", "Element3:0:2", "Element4:0:2", "Element5:0:2",
                    "Element6:0:2", "Element7:0:2", "Element8:0:2", "Element9:0:2", "Element10:0:2",
                    "Element11:0:2", "Element12:0:2", "Element13:0:2", "Element14:0:2", "Element15:0:2",
                    "Element16:0:2", "Element17:0:2", "Element18:0:2", "Element19:0:2", "Element20:0:2",
                    "Element21:0:2", "Element22:0:2", "Element23:0:2", "Element24:0:2",
                },
                created.Select(t => t.Field + ":" + t.Default + ":" + t.Len));
            Assert.Equal(FieldSets.StdItemsCreateOrder(), created.Select(t => t.Field).ToList());

            Assert.True(f.ButtonCreateStdItemsField.Enabled);               // :488 重新启用
            Assert.False(g_boHeroDBOK);
        });
    }

    [Fact]
    public void ButtonCreateStdItemsFieldClick_Success_SetsOkShowsBoxAndCloses()
    {
        _db = FieldSets.CompleteFake();
        _db.CreateFieldResult = true;
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            f.MemoLog1.TextChanged += (s, e) => CaptureMemo1(f.MemoLog1);
            f.ButtonCreateStdItemsFieldClick(f);

            Assert.Equal(38, _memo1Snapshot.Count);
            Assert.All(_memo1Snapshot, l => Assert.EndsWith("字段创建成功", l));
            Assert.Equal(38, _db.CreatedFields.Count);          // 建字段 38 次，日志 38 行
            Assert.Equal(FieldSets.StdItemsCreateOrder(), _db.CreatedFields.Select(t => t.Field).ToList());
            Assert.Equal(
                new[]
                {
                    "Color", "OverLap", "HP", "MP", "Light", "Horse", "Element",
                    "Expand1", "Expand2", "Expand3", "Expand4", "Expand5",
                    "InsuranceCurrency", "InsuranceGold",
                },
                _memo1Snapshot.Take(14).Select(l => l.Replace("字段创建成功", "")));
            Assert.Equal("Element1字段创建成功", _memo1Snapshot[14]);
            Assert.Equal("Element24字段创建成功", _memo1Snapshot[37]);
            Assert.True(g_boHeroDBOK);
            Assert.Equal("数据库更新成功！！！", f.LastMessageBoxText);
            Assert.Equal(0x00 + 0x30, f.LastMessageBoxFlags);
            Assert.True(f.CloseCalled);
        });
    }

    private static (string, string, object, object) ToTuple((string Table, string Field, object Default, object Len) t)
        => (t.Table, t.Field, t.Default, t.Len);

    // ==================================================================
    // ButtonMagicFieldClick（:496-532）+ 取值规则（:509-516）
    // ==================================================================

    [Theory]
    [InlineData("CanUpgrade", 0)]
    [InlineData("canupgrade", 0)]         // SameText：大小写不敏感
    [InlineData("MaxUpgradeLv", 0)]
    [InlineData("MAXUPGRADELV", 0)]
    [InlineData("MaxTrainLv", 3)]         // 首字符 'M'
    [InlineData("NeedL1", 20)]            // 首字符 'N'
    [InlineData("L1Train", 200)]          // 其余
    [InlineData("m", 200)]                // 'm' ≠ 'M'：原文首字符比较**区分大小写**
    [InlineData("n", 200)]
    public void MagicFieldValue_RulesAreLocked(string field, int expected)
    {
        Assert.Equal(expected, TFrmHeroDB.MagicFieldValue(field));
    }

    [Fact]
    public void MagicFieldValue_EmptyFieldName_ThrowsLikeDelphiEStringIndex()
    {
        // 原文缺陷 3：:511 `sFieldName[1]` 是 Delphi 1 基索引，空串访问即 EStringIndex（无守卫）。
        Assert.Throws<IndexOutOfRangeException>(() => TFrmHeroDB.MagicFieldValue(""));
    }

    [Fact]
    public void ButtonMagicFieldClick_ValueRulesAndLogLines()
    {
        _db.CreateFieldResult = true;
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            f.MemoLog3.TextChanged += (s, e) => CaptureMemo3(f.MemoLog3);
            f.ListBoxMagic.Items.AddRange(new object[] { "MaxTrainLv", "CanUpgrade", "NeedL3", "L3Train" });
            f.ButtonMagicFieldClick(f);

            // 同上：:526 的 CheckHeroDB 会清空 MemoLog3 ⇒ 用过程中的最长快照断言。
            Assert.Equal(
                new[]
                {
                    "MaxTrainLv字段创建成功",
                    "CanUpgrade字段创建成功",
                    "NeedL3字段创建成功",
                    "L3Train字段创建成功",
                },
                _memo3Snapshot);

            Assert.Equal(new (string, string, object, object)[]
            {
                ("Magic", "MaxTrainLv", 3, (byte)4),
                ("Magic", "CanUpgrade", 0, (byte)4),
                ("Magic", "NeedL3", 20, (byte)4),
                ("Magic", "L3Train", 200, (byte)4),
            }, _db.CreatedFields.Select(ToTuple).ToArray());
        });
    }

    [Fact]
    public void ButtonMagicFieldClick_EmptyListBox_StillRunsCheckAndReenablesButton()
    {
        _db = FieldSets.CompleteFake();
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            f.MemoLog3.TextChanged += (s, e) => CaptureMemo3(f.MemoLog3);
            Assert.Empty(f.ListBoxMagic.Items);                 // 空 ListBox ⇒ 循环体 0 次
            f.ButtonMagicFieldClick(f);

            Assert.Empty(_memo3Snapshot);                       // 全程没写过一行
            Assert.Empty(_db.CreatedFields);
            Assert.True(f.ButtonMagicField.Enabled);
            Assert.True(g_boHeroDBOK);                          // 后面照样 CheckHeroDB + 弹窗 + Close
            Assert.Equal("数据库更新成功！！！", f.LastMessageBoxText);
            Assert.True(f.CloseCalled);
        });
    }

    [Fact]
    public void ButtonMagicFieldClick_EmptyItemString_ThrowsLikeDelphiEStringIndex()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.ListBoxMagic.Items.Add("");                       // 原文 :511 会在这里炸 EStringIndex
            Assert.Throws<IndexOutOfRangeException>(() => f.ButtonMagicFieldClick(f));
        });
    }

    // ==================================================================
    // ButtonMonsterFieldClick（:534-563）
    // ==================================================================

    [Fact]
    public void ButtonMonsterFieldClick_ZeroValueLen4AndLogLines()
    {
        _db.CreateFieldResult = true;
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            f.MemoLog2.TextChanged += (s, e) => CaptureMemo2(f.MemoLog2);
            f.ListBoxMonster.Items.AddRange(new object[] { "AttackState", "ExploreItem" });
            f.ButtonMonsterFieldClick(f);

            Assert.Equal(new[] { "AttackState字段创建成功", "ExploreItem字段创建成功" }, _memo2Snapshot);
            Assert.Equal(new (string, string, object, object)[]
            {
                ("Monster", "AttackState", 0, (byte)4),       // :548 nValue := 0（循环体内）＋ Len=4
                ("Monster", "ExploreItem", 0, (byte)4),
            }, _db.CreatedFields.Select(ToTuple).ToArray());
            Assert.True(f.ButtonMonsterField.Enabled);
        });
    }

    [Fact]
    public void ButtonMonsterFieldClick_Failure_PrefixesFieldName()
    {
        _db.CreateFieldResult = false;
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            f.MemoLog2.TextChanged += (s, e) => CaptureMemo2(f.MemoLog2);
            f.ListBoxMonster.Items.Add("AttackSource");
            f.ButtonMonsterFieldClick(f);
            Assert.Equal(new[] { "AttackSource字段创建失败" }, _memo2Snapshot);
        });
    }

    // ==================================================================
    // ButtonCloseClick（:385-389）
    // ==================================================================

    [Fact]
    public void ButtonCloseClick_SetsBoHeroDBOKFromNotCheckHeroDB()
    {
        _db = FieldSets.CompleteFake();                      // CheckHeroDB = False ⇒ OK = True
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            f.ButtonCloseClick(f);
            Assert.True(g_boHeroDBOK);
            Assert.True(f.CloseCalled);
        });
    }

    [Fact]
    public void ButtonCloseClick_MissingFields_SetsBoHeroDBOKFalse()
    {
        _db = FieldSets.CompleteFake();
        _db.RemoveField("StdItems", "Color");
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.Show();
            f.ButtonCloseClick(f);
            Assert.False(g_boHeroDBOK);
            Assert.True(f.CloseCalled);
        });
    }

    // ==================================================================
    // SelectDirectory（:59-110）/ SelectDirCB（:52-57）
    // ==================================================================

    [Fact]
    public void SelectDirCB_AlwaysReturnsZeroAndHandlesInitializedOnly()
    {
        // 原文 :54-56：只有 BFFM_INITIALIZED 且 lpData <> 0 才发消息；:56 Result := 0 无条件执行。
        Assert.Equal(0, TFrmHeroDB.SelectDirCB(IntPtr.Zero, TFrmHeroDB.BFFM_INITIALIZED, IntPtr.Zero, (IntPtr)1234));
        Assert.Equal(0, TFrmHeroDB.SelectDirCB((IntPtr)1, TFrmHeroDB.BFFM_INITIALIZED, IntPtr.Zero, IntPtr.Zero));
        Assert.Equal(0, TFrmHeroDB.SelectDirCB((IntPtr)1, 0u, IntPtr.Zero, (IntPtr)1234));
    }

    [Fact]
    public void SelectDirCB_ConstantsMatchShlObj()
    {
        Assert.Equal(1, TFrmHeroDB.BFFM_INITIALIZED);
        Assert.Equal(1029, TFrmHeroDB.BFFM_SETSELECTION);
        Assert.Equal(0x0001, TFrmHeroDB.BIF_RETURNONLYFSDIRS);
        Assert.Equal(0x0050, TFrmHeroDB.BIF_USENEWUI);
        Assert.Equal(0, TFrmHeroDB.S_OK);
    }

    [Fact]
    public void SelectDirectory_NonExistentInput_ClearsDirectoryBeforePicking()
    {
        string missing = Path.Combine(Dir, "definitely_missing");
        FolderPicker.Capture(out var initial, result: null);
        try
        {
            string Directory = missing;
            Assert.False(TFrmHeroDB.SelectDirectory("请选择数据库目录", "", ref Directory, IntPtr.Zero));
            Assert.Equal("", Directory);                     // :71-72 目录不存在 ⇒ var 参数清空
            Assert.Equal("", initial());                     // 且预选目录也随之变空
        }
        finally { FolderPicker.Restore(); }
    }

    [Fact]
    public void SelectDirectory_ExistingInput_KeepsDirectoryAsPreselection()
    {
        FolderPicker.Capture(out var initial, result: null);
        try
        {
            string Directory = Dir;                          // Dir 由 GameCenterTestBase 真实建好
            Assert.False(TFrmHeroDB.SelectDirectory("请选择数据库目录", "", ref Directory, IntPtr.Zero));
            Assert.Equal(Dir, Directory);                    // 未取消前原值不动（取消才返回 False，但不清空）
            Assert.Equal(Dir, initial());
        }
        finally { FolderPicker.Restore(); }
    }

    [Fact]
    public void SelectDirectory_Chosen_CopiesVerbatimWithTrailingBackslash()
    {
        // 原文 :104 `Directory := Buffer;` —— **不**在 SelectDirectory 内剥尾部反斜杠
        // （剥除发生在调用方 :117-118 / :141-142）。
        FolderPicker.Returns(@"D:\MirServer\Mud2\DB\");
        try
        {
            string Directory = "";
            Assert.True(TFrmHeroDB.SelectDirectory("请选择数据库目录", "", ref Directory, IntPtr.Zero));
            Assert.Equal(@"D:\MirServer\Mud2\DB\", Directory);
        }
        finally { FolderPicker.Restore(); }
    }

    [Fact]
    public void SelectDirectory_RootArgumentIsIgnoredInManagedPort()
    {
        FolderPicker.Returns(@"D:\X");
        try
        {
            string d1 = "";
            string d2 = "";
            // 原文 :78-82 用 Root 解析 pidlRoot；托管侧无对应物（偏差 D-P10-18）⇒ 不影响结果
            Assert.True(TFrmHeroDB.SelectDirectory("C", @"D:\MirServer", ref d1, IntPtr.Zero));
            Assert.True(TFrmHeroDB.SelectDirectory("C", "", ref d2, IntPtr.Zero));
            Assert.Equal(d1, d2);
        }
        finally { FolderPicker.Restore(); }
    }

    // ==================================================================
    // 接缝默认值（台账 §25.2：不得静默返回中性值）
    // ==================================================================

    [Fact]
    public void Seams_DefaultToUnwiredOrNull_NotNeutralValues()
    {
        HeroDBFactory.ResetForTests();
        TFrmHeroDB.ResetForTests();
        Assert.Null(HeroDBFactory.InstanceFactory);
        Assert.Null(TFrmHeroDB.FolderPickerProvider);
        Assert.Null(TFrmHeroDB.ShowModalHandler);
        var ex = Assert.Throws<NotWiredException>(() => HeroDBFactory.Create());
        Assert.Contains("未接线：GHeroDB.pas 未移植", ex.Message);
    }

    // ==================================================================
    // 边界：空 EditHeroDB / 尾反斜杠 / SetItemIndex 垫片
    // ==================================================================

    [Fact]
    public void ButtonSaveHeroDBConfigClick_EmptyEditHeroDB_WritesEmptyAlias()
    {
        string dir = MakeDbDir(_db, Dir);
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.EditHeroDB.Text = "   ";                       // Trim 后为空串
            f.EditHeroDBPath.Text = dir;
            f.ButtonSaveHeroDBConfigClick(f);

            Assert.Equal("", g_sHeroDBName);                 // 原文不校验空别名（原文如此）
            Assert.Equal(("", dir), _db.SavedConfigFiles[0]);
            Assert.Equal("", g_IniConf!.ReadString("GameConf", "HeroDBName", "<缺>"));
        });
    }

    [Fact]
    public void ButtonSaveHeroDBConfigClick_EmptyEditHeroDBPath_LogsThreeMissesAndExits()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.EditHeroDB.Text = "HeroDB";
            f.EditHeroDBPath.Text = "   ";                   // Trim 后为空
            f.ButtonSaveHeroDBConfigClick(f);

            Assert.Equal(3, f.MemoLogLines.Count);
            Assert.Empty(_db.SavedConfigFiles);
        });
    }

    [Fact]
    public void SetItemIndex_OutOfRangeYieldsMinusOneLikeDelphi()
    {
        // Delphi TListBox.ItemIndex 越界赋值为 -1；WinForms SelectedIndex 会抛。
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            f.ListBoxStdItems.Items.Add("A");
            TFrmHeroDB.SetItemIndex(f.ListBoxStdItems, 0);
            Assert.Equal(0, f.ListBoxStdItems.SelectedIndex);
            TFrmHeroDB.SetItemIndex(f.ListBoxStdItems, 5);
            Assert.Equal(-1, f.ListBoxStdItems.SelectedIndex);
            TFrmHeroDB.SetItemIndex(f.ListBoxStdItems, -3);
            Assert.Equal(-1, f.ListBoxStdItems.SelectedIndex);
        });
    }

    // ==================================================================
    // DFM 对账（计数取证；台账 §37.3 / §41.3）
    // ==================================================================

    [Fact]
    public void DfmReconcile_25Objects_FormPlus24Children()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            Assert.Equal(25, P10FormReconcile.CountDfmObjects(f));   // DFM: 25 个 object 节点
            Assert.Equal(24, P10FormReconcile.CountChildrenOf(f));   // 1 个窗体根 + 24 个子控件
        });
    }

    [Fact]
    public void DfmReconcile_ControlNamesMatchDfmOrder()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            var expected = new[]
            {
                "PageControl",
                "TabSheet1", "Label1", "Label2", "EditHeroDB", "EditHeroDBPath",
                "ButtonSaveHeroDBConfig", "MemoLog",
                "TabSheet2", "GroupBox1", "ListBoxStdItems", "ButtonCreateStdItemsField", "MemoLog1",
                "TabSheet3", "GroupBox2", "ListBoxMonster", "MemoLog2", "ButtonMonsterField",
                "TabSheet4", "GroupBox3", "ListBoxMagic", "MemoLog3", "ButtonMagicField",
                "ButtonClose",
            };
            var actual = P10FormReconcile.EnumerateDfmObjects(f)
                .Skip(1)                                  // 跳过窗体自身
                .Select(o => LeafName(o.Name))
                .ToArray();
            Assert.Equal(24, expected.Length);
            Assert.Equal(expected, actual);
        });
    }

    [Fact]
    public void DfmReconcile_RzButtonEditInnerButtonIsUnnamed_NotCountedAsObject()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            // Raize 的内嵌按钮在 DFM 里没有 object 节点 ⇒ 托管等价物刻意不留名字
            Assert.Equal("", f.EditHeroDBPathButton.Name);
            Assert.DoesNotContain(P10FormReconcile.DfmControls(f), c => ReferenceEquals(c, f.EditHeroDBPathButton));
            Assert.Contains(P10FormReconcile.AllControls(f), c => ReferenceEquals(c, f.EditHeroDBPathButton));
            // 但事件必须真的挂上（否证"没接线"）
            Assert.True(P10FormReconcile.IsBound(f.EditHeroDBPathButton, "Click"));
        });
    }

    [Fact]
    public void DfmReconcile_SixOnClickBindings_PlusLoad()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();

            // DFM 声明的 6 个 On* 绑定，逐一在托管侧存在
            Assert.True(P10FormReconcile.IsBound(f.EditHeroDBPathButton, "Click"));   // OnButtonClick（Raize 内嵌按钮）
            Assert.True(P10FormReconcile.IsBound(f.ButtonSaveHeroDBConfig, "Click"));
            Assert.True(P10FormReconcile.IsBound(f.ButtonCreateStdItemsField, "Click"));
            Assert.True(P10FormReconcile.IsBound(f.ButtonMonsterField, "Click"));
            Assert.True(P10FormReconcile.IsBound(f.ButtonMagicField, "Click"));
            Assert.True(P10FormReconcile.IsBound(f.ButtonClose, "Click"));

            // 有名字控件上的绑定数 = 5：Raize 内嵌按钮在 DFM 里没有 object 节点（无名），
            // 不计入 DFM 控件树，故单独计数。
            int onControls = P10FormReconcile.DfmControls(f)
                .Sum(c => P10FormReconcile.CountEventBindingsOn(c));
            Assert.Equal(5, onControls);
            Assert.Equal(1, P10FormReconcile.CountEventBindingsOn(f.EditHeroDBPathButton));
            Assert.Equal(1, P10FormReconcile.CountEventBindingsOn(f));
            Assert.Equal(6, P10FormReconcile.CountEventBindings(f));   // 5 + 窗体自身 1（Closed）
            Assert.True(P10FormReconcile.IsBound(f, "Closed"));
        });
    }

    [Fact]
    public void DfmReconcile_FormRootHasNoOnCreateOrOnDestroy()
    {
        // 负数断言必须计数取证：DFM 窗体根没有 OnCreate/OnDestroy，托管侧也就不该绑 Load/FormClosed。
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            Assert.False(P10FormReconcile.IsBound(f, "Load"));
            Assert.False(P10FormReconcile.IsBound(f, "FormClosed"));
            Assert.False(P10FormReconcile.IsBound(f, "FormClosing"));
            Assert.False(P10FormReconcile.IsBound(f, "Shown"));
            Assert.Equal(0, P10FormReconcile.CountEventBindingsOn(f.PageControl));
            Assert.Equal(0, P10FormReconcile.CountEventBindingsOn(f.EditHeroDB));
            Assert.Equal(0, P10FormReconcile.CountEventBindingsOn(f.MemoLog));
        });
    }

    [Fact]
    public void DfmReconcile_TextsAndBorderStyleMatchDfm()
    {
        FormSta.Run(() =>
        {
            using var f = new TFrmHeroDB();
            Assert.Equal("HeroDB自动配置", f.Text);                                  // Caption
            Assert.Equal(new System.Drawing.Size(511, 330), f.ClientSize);          // ClientWidth/Height
            Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedDialog, f.FormBorderStyle);  // bsDialog
            Assert.False(f.MaximizeBox);
            Assert.False(f.MinimizeBox);
            Assert.Equal("HeroDB", f.TabSheet1.Text);
            Assert.Equal("StdItems.DB", f.TabSheet2.Text);
            Assert.Equal("Monster.DB", f.TabSheet3.Text);
            Assert.Equal("Magic.DB", f.TabSheet4.Text);
            Assert.Equal("数据库别名:", f.Label1.Text);
            Assert.Equal("数据库路径:", f.Label2.Text);
            Assert.Equal("HeroDB", f.EditHeroDB.Text);
            Assert.Equal("自动配置HeroDB", f.ButtonSaveHeroDBConfig.Text);
            Assert.Equal("取消", f.ButtonClose.Text);
            Assert.Equal("StdItems.DB中缺少以下字段", f.GroupBox1.Text);
            Assert.Equal("Monster.DB中缺少以下字段", f.GroupBox2.Text);
            Assert.Equal("Magic.DB中缺少以下字段", f.GroupBox3.Text);
            Assert.True(f.MemoLog.ReadOnly);
            Assert.True(f.MemoLog1.ReadOnly);
            Assert.True(f.MemoLog2.ReadOnly);
            Assert.True(f.MemoLog3.ReadOnly);
            Assert.Same(f.TabSheet2, f.PageControl.SelectedTab);                    // ActivePage=TabSheet2
        });
    }

    private static string LeafName(string name)
    {
        int i = name.LastIndexOf('.');
        return i >= 0 ? name.Substring(i + 1) : name;
    }
}

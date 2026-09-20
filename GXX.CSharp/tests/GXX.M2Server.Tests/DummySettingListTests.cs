using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms.DummySetting;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 两个列表页（`ts4` 禁止移动地图 :913-995 / `ts5` 不主动攻击怪物 :997-1078）
/// + `lstDummyListClick`（:327-343）+ `btn1Click`（:552-614）1:1 覆盖。
/// <para>
/// 原文缺陷差异断言：
///   · :924 / :1008 外层 `if <List>.Items.Count &gt;= 0` **恒真**；
///   · :251 / :960 / :1044 等 `AddAll` 的循环上界用的是**源**列表 Count；
///   · :946 / :1031 用 `Items.Delete(ItemIndex)` 删**当前行**（不是全部选中）。
/// </para>
/// </summary>
[Collection("DummySettingSerial")]
public sealed class DummySettingListTests : DummySettingTestBase
{
    public DummySettingListTests()
    {
        OpenWith(maps: new[] { "0", "3", "D401" }, monsters: new[] { "鸡", "鹿", "僵尸" });
    }

    // ---------------- lstDummyListClick（:327-343） ----------------

    [Fact]
    public void DummyListClick_ValidIndex_EnablesButtonsAndCopiesName()
    {
        Form.Ct.lstDummyList.Items.Clear();
        Form.Ct.lstDummyList.Items.Add("甲");
        Form.Ct.lstDummyList.Items.Add("乙");
        Form.Ct.lstDummyList.SelectedIndex = 1;

        Form.lstDummyListClick(Form.Ct.lstDummyList);

        Assert.True(Form.Ct.btnDummyLogon.Enabled);
        Assert.True(Form.Ct.btnDummyDel.Enabled);
        Assert.Equal("乙", Form.Ct.edtDummyName.Text);
    }

    [Fact]
    public void DummyListClick_NoSelection_DisablesButtons()
    {
        Form.Ct.lstDummyList.Items.Clear();
        Form.Ct.lstDummyList.Items.Add("甲");
        Form.Ct.lstDummyList.SelectedIndex = -1;
        Form.Ct.btnDummyLogon.Enabled = true;
        Form.Ct.btnDummyDel.Enabled = true;

        Form.lstDummyListClick(Form.Ct.lstDummyList);

        Assert.False(Form.Ct.btnDummyLogon.Enabled);
        Assert.False(Form.Ct.btnDummyDel.Enabled);
    }

    [Fact]
    public void DummyListClick_EmptyList_DisablesButtonsAndLeavesNameText()
    {
        Form.Ct.lstDummyList.Items.Clear();
        Form.Ct.edtDummyName.Text = "KEEP";
        Form.Ct.btnDummyLogon.Enabled = true;

        Form.lstDummyListClick(Form.Ct.lstDummyList);

        Assert.False(Form.Ct.btnDummyLogon.Enabled);
        Assert.Equal("KEEP", Form.Ct.edtDummyName.Text);
    }

    // ---------------- btnDummyAddClick（:376-393） ----------------

    [Fact]
    public void DummyAdd_TrimsAndAddsToGlobalAndListBox()
    {
        Form.Ct.edtDummyName.Text = "   甲   ";
        Form.btnDummyAddClick(Form.Ct.btnDummyAdd);

        Assert.Equal(new[] { "甲" }, DummySettingState.Snapshot(DummySettingState.g_DummyNameList));
        Assert.Equal("甲", Form.Ct.lstDummyList.Items[0]);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void DummyAdd_EmptyAfterTrim_NoOp()
    {
        Form.Ct.edtDummyName.Text = "   ";
        Form.btnDummyAddClick(Form.Ct.btnDummyAdd);

        Assert.Empty(DummySettingState.Snapshot(DummySettingState.g_DummyNameList));
        Assert.Empty(Form.Ct.lstDummyList.Items);
    }

    [Fact]
    public void DummyAdd_Duplicate_NoOp()
    {
        Form.Ct.edtDummyName.Text = "甲";
        Form.btnDummyAddClick(Form.Ct.btnDummyAdd);
        Form.btnDummyAddClick(Form.Ct.btnDummyAdd);

        Assert.Single(DummySettingState.Snapshot(DummySettingState.g_DummyNameList));
        Assert.Single(Form.Ct.lstDummyList.Items);
    }

    [Fact]
    public void DummyAdd_DuplicateDifferentCase_NoOp_CompareTextSemantics()
    {
        // `GetDummyNameList`（M2Share.pas:16649）用 `CompareText` ⇒ **大小写无关**
        Form.Ct.edtDummyName.Text = "DummyA";
        Form.btnDummyAddClick(Form.Ct.btnDummyAdd);
        Form.Ct.edtDummyName.Text = "dummya";
        Form.btnDummyAddClick(Form.Ct.btnDummyAdd);

        Assert.Single(DummySettingState.Snapshot(DummySettingState.g_DummyNameList));
    }

    [Fact]
    public void DummyAdd_DuplicateExactCase_NoOp()
    {
        DummySettingState.g_DummyNameList.Add("甲");
        Form.Ct.edtDummyName.Text = "甲";
        Form.btnDummyAddClick(Form.Ct.btnDummyAdd);
        Assert.Single(DummySettingState.Snapshot(DummySettingState.g_DummyNameList));
    }

    [Fact]
    public void DummyAdd_RebuildsWholeListBox_FromGlobal()
    {
        DummySettingState.g_DummyNameList.Add("旧1");
        Form.Ct.lstDummyList.Items.Add("应该被清掉");
        Form.Ct.edtDummyName.Text = "新1";

        Form.btnDummyAddClick(Form.Ct.btnDummyAdd);

        Assert.Equal(new[] { "旧1", "新1" }, new[]
        {
            Form.Ct.lstDummyList.Items[0]!.ToString(),
            Form.Ct.lstDummyList.Items[1]!.ToString(),
        });
    }

    // ---------------- btnDummyDelClick（:395-406） ----------------

    [Fact]
    public void DummyDel_DeletesAllSelectedAndSyncsGlobal()
    {
        Form.Ct.lstDummyList.Items.Clear();
        Form.Ct.lstDummyList.Items.AddRange(new object[] { "甲", "乙", "丙" });
        DummySettingState.g_DummyNameList.Clear();
        DummySettingState.g_DummyNameList.Add("甲");
        DummySettingState.g_DummyNameList.Add("乙");
        DummySettingState.g_DummyNameList.Add("丙");

        SelectDummyList(0, 2);
        Form.btnDummyDelClick(Form.Ct.btnDummyDel);

        Assert.Equal(new[] { "乙" }, DummySettingState.Snapshot(DummySettingState.g_DummyNameList));
        Assert.Single(Form.Ct.lstDummyList.Items);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void DummyDel_NoSelection_StillSyncsGlobalFromListBox()
    {
        // 原文 :397 `DeleteSelected` 无选中 = no-op；但 :400-401 **仍会**用控件列表覆写全局。
        Form.Ct.lstDummyList.Items.Clear();
        Form.Ct.lstDummyList.Items.AddRange(new object[] { "甲", "乙" });
        DummySettingState.g_DummyNameList.Clear();
        DummySettingState.g_DummyNameList.Add("别的");

        Form.btnDummyDelClick(Form.Ct.btnDummyDel);

        Assert.Equal(new[] { "甲", "乙" }, DummySettingState.Snapshot(DummySettingState.g_DummyNameList));
    }

    [Fact]
    public void DummyDel_EmptyListBox_ClearsGlobal()
    {
        DummySettingState.g_DummyNameList.Add("甲");
        Form.Ct.lstDummyList.Items.Clear();

        Form.btnDummyDelClick(Form.Ct.btnDummyDel);

        Assert.Empty(DummySettingState.Snapshot(DummySettingState.g_DummyNameList));
    }

    // ---------------- lstDisableMoveMapClick（:913-917） ----------------

    [Fact]
    public void DisableMoveMapClick_Selected_EnablesDeleteButton()
    {
        Form.Ct.lstDisableMoveMap.Items.Add("0");
        Form.Ct.lstDisableMoveMap.SelectedIndex = 0;
        Form.lstDisableMoveMapClick(Form.Ct.lstDisableMoveMap);
        Assert.True(Form.Ct.btnDisableMoveMapDelete.Enabled);
    }

    [Fact]
    public void DisableMoveMapClick_NoSelection_LeavesButtonUnchanged()
    {
        // 原文 :915-916 **没有** else 分支 ⇒ 不选中时按钮保持原状
        Form.Ct.btnDisableMoveMapDelete.Enabled = true;
        Form.Ct.lstDisableMoveMap.SelectedIndex = -1;
        Form.lstDisableMoveMapClick(Form.Ct.lstDisableMoveMap);
        Assert.True(Form.Ct.btnDisableMoveMapDelete.Enabled);
    }

    [Fact]
    public void DisableMoveMapClick_EmptyList_NoThrow()
    {
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.SelectedIndex = -1;
        Assert.Null(Record.Exception(() => { Form.lstDisableMoveMapClick(Form.Ct.lstDisableMoveMap); }));
    }

    // ---------------- btnDisableMoveMapAddClick（:919-940） ----------------

    [Fact]
    public void DisableMoveMapAdd_AddsSelectedMapNames()
    {
        SelectMapList(0, 2);
        Form.btnDisableMoveMapAddClick(Form.Ct.btnDisableMoveMapAdd);

        Assert.Equal(new[] { "0", "D401" }, new[]
        {
            Form.Ct.lstDisableMoveMap.Items[0]!.ToString(),
            Form.Ct.lstDisableMoveMap.Items[1]!.ToString(),
        });
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void DisableMoveMapAdd_Deduplicates_WithinTargetList()
    {
        Form.Ct.lstDisableMoveMap.Items.Add("3");
        SelectMapList(0, 1, 2);
        Form.btnDisableMoveMapAddClick(Form.Ct.btnDisableMoveMapAdd);

        // "3" 已在目标里 ⇒ 只新增 "0" 与 "D401"
        Assert.Equal(3, Form.Ct.lstDisableMoveMap.Items.Count);
        int countOf3 = 0;
        foreach (var o in Form.Ct.lstDisableMoveMap.Items)
            if (o!.ToString() == "3") countOf3++;
        Assert.Equal(1, countOf3);
    }

    [Fact]
    public void DisableMoveMapAdd_NoSelection_EntersTrueBranchButAddsNothing_AndMarksDirty()
    {
        // ★ 差异断言：原文 :924 `if lstMapList.Items.Count >= 0` **恒真** ⇒
        // 即使一个都没选，:937 `FIsDummyDisableMoveMapChanged := True` 与
        // :938 `ModValue()` **仍然执行**。
        SelectMapList();
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.uModValue();

        Form.btnDisableMoveMapAddClick(Form.Ct.btnDisableMoveMapAdd);

        Assert.Empty(Form.Ct.lstDisableMoveMap.Items);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);      // :938 ModValue 确实跑了
    }

    [Fact]
    public void DisableMoveMapAdd_EmptySourceList_StillMarksDirty()
    {
        // ★ 同上：Count == 0 时 `0 >= 0` 仍为 True
        OpenWith(maps: Array.Empty<string>());
        Form.uModValue();

        Form.btnDisableMoveMapAddClick(Form.Ct.btnDisableMoveMapAdd);

        Assert.Empty(Form.Ct.lstDisableMoveMap.Items);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void AddSelectedMaps_PureLogic_MatchesHandler()
    {
        SelectMapList(1);
        bool entered = TFrmDummySetting.AddSelectedMaps(Form.Ct);
        Assert.True(entered);
        Assert.Equal(new[] { "3" }, new[] { Form.Ct.lstDisableMoveMap.Items[0]!.ToString() });
    }

    [Fact]
    public void AddSelectedMaps_PureLogic_OutOfRangeSelectionIndex_NullSafe()
    {
        // 越界选中索引 ⇒ Delphi `Selected[i]` 返回 False（不抛）；WinForms `GetSelected` 越界抛
        // ⇒ 本断言锁定"源列表为空时纯函数不抛"。
        OpenWith(maps: Array.Empty<string>());
        Assert.Null(Record.Exception(() => { TFrmDummySetting.AddSelectedMaps(Form.Ct); }));
    }

    // ---------------- btnDisableMoveMapDeleteClick（:942-953） ----------------

    [Fact]
    public void DisableMoveMapDelete_DeletesCurrentRowOnly()
    {
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.AddRange(new object[] { "A", "B", "C" });
        Form.Ct.lstDisableMoveMap.SelectedIndex = 1;

        Form.btnDisableMoveMapDeleteClick(Form.Ct.btnDisableMoveMapDelete);

        Assert.Equal(2, Form.Ct.lstDisableMoveMap.Items.Count);
        Assert.Equal("A", Form.Ct.lstDisableMoveMap.Items[0]!.ToString());
        Assert.Equal("C", Form.Ct.lstDisableMoveMap.Items[1]!.ToString());
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void DisableMoveMapDelete_NoSelection_DisablesButton()
    {
        Form.Ct.lstDisableMoveMap.Items.Add("A");
        Form.Ct.lstDisableMoveMap.SelectedIndex = -1;
        Form.Ct.btnDisableMoveMapDelete.Enabled = true;

        Form.btnDisableMoveMapDeleteClick(Form.Ct.btnDisableMoveMapDelete);

        Assert.False(Form.Ct.btnDisableMoveMapDelete.Enabled);   // :951-952
        Assert.Single(Form.Ct.lstDisableMoveMap.Items);          // 未删除
    }

    [Fact]
    public void DisableMoveMapDelete_LastRow_LeavesNoSelectionAndDisablesButton()
    {
        Form.Ct.lstDisableMoveMap.Items.Add("A");
        Form.Ct.lstDisableMoveMap.SelectedIndex = 0;

        Form.btnDisableMoveMapDeleteClick(Form.Ct.btnDisableMoveMapDelete);

        Assert.Empty(Form.Ct.lstDisableMoveMap.Items);
        Assert.False(Form.Ct.btnDisableMoveMapDelete.Enabled);
    }

    // ---------------- btnDisableMoveMapAddAllClick（:955-966） ----------------

    [Fact]
    public void DisableMoveMapAddAll_CopiesWholeSourceList()
    {
        Form.Ct.lstDisableMoveMap.Items.Add("旧");
        Form.btnDisableMoveMapAddAllClick(Form.Ct.btnDisableMoveMapAddAll);

        Assert.Equal(3, Form.Ct.lstDisableMoveMap.Items.Count);
        Assert.Equal("0", Form.Ct.lstDisableMoveMap.Items[0]!.ToString());
        Assert.Equal("D401", Form.Ct.lstDisableMoveMap.Items[2]!.ToString());
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void DisableMoveMapAddAll_EmptySource_ClearsTarget()
    {
        OpenWith(maps: Array.Empty<string>());
        Form.Ct.lstDisableMoveMap.Items.Add("旧");
        Form.btnDisableMoveMapAddAllClick(Form.Ct.btnDisableMoveMapAddAll);
        Assert.Empty(Form.Ct.lstDisableMoveMap.Items);
    }

    [Fact]
    public void DisableMoveMapAddAll_LoopBoundIsSourceCount_NotTarget()
    {
        // ★ 差异断言：:960 上界取 `lstMapList.Items.Count`（源），:959 先 Clear 目标
        // ⇒ 若误取目标 Count，Clear 后循环体不会执行、结果为空。
        OpenWith(maps: new[] { "M1", "M2" });
        Form.btnDisableMoveMapAddAllClick(Form.Ct.btnDisableMoveMapAddAll);
        Assert.Equal(2, Form.Ct.lstDisableMoveMap.Items.Count);
    }

    // ---------------- btnDisableMoveMapDeleteAllClick（:968-974） ----------------

    [Fact]
    public void DisableMoveMapDeleteAll_ClearsAndDisablesButton()
    {
        Form.Ct.lstDisableMoveMap.Items.AddRange(new object[] { "A", "B" });
        Form.Ct.btnDisableMoveMapDelete.Enabled = true;

        Form.btnDisableMoveMapDeleteAllClick(Form.Ct.btnDisableMoveMapDeleteAll);

        Assert.Empty(Form.Ct.lstDisableMoveMap.Items);
        Assert.False(Form.Ct.btnDisableMoveMapDelete.Enabled);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void DisableMoveMapDeleteAll_AlreadyEmpty_StillMarksDirty()
    {
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.uModValue();
        Form.btnDisableMoveMapDeleteAllClick(Form.Ct.btnDisableMoveMapDeleteAll);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void DisableMoveMapDeleteAll_ThenSave_WritesEmptyGlobal()
    {
        OpenWith(maps: new[] { "0", "3", "D401" });
        DummySettingState.g_DummyDisableMoveMapList.Add("OLD");
        Form.btnDisableMoveMapDeleteAllClick(Form.Ct.btnDisableMoveMapDeleteAll);

        Form.Ct.edtDummyHomeMap.Text = "3";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);

        Assert.Empty(DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList));
    }

    // ---------------- btnDisableMoveMapSaveClick（:976-995） ----------------

    [Fact]
    public void DisableMoveMapSave_WritesGlobalAndClearsDirtyFlag()
    {
        bool seamHit = false;
        DummySettingState.SaveListToFile = (_, _) => seamHit = true;
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.Add("X1");

        Form.btnDisableMoveMapSaveClick(Form.Ct.btnDisableMoveMapSave);

        Assert.True(seamHit);
        Assert.Equal(new[] { "X1" }, DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList));
    }

    [Fact]
    public void DisableMoveMapSave_ClearsDirtyFlag_SoLaterButtonSaveSkipsBlock()
    {
        // ★ 差异断言：`btnDisableMoveMapSaveClick` **清脏**（:993 False），
        // 而 `btnDisableMoveMapAddClick` / `DeleteAll` **置脏**。
        DummySettingState.g_DummyDisableMoveMapList.Add("OLD");
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.Add("NEW");
        Form.btnDisableMoveMapSaveClick(Form.Ct.btnDisableMoveMapSave);

        // 清脏后再改控件，保存按钮不应把控件内容写回全局
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.Add("CHANGED");
        Form.Ct.edtDummyHomeMap.Text = "3";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);

        Assert.Equal(new[] { "NEW" }, DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList));
    }

    [Fact]
    public void DisableMoveMapSave_EmptyList_ClearsGlobal()
    {
        DummySettingState.g_DummyDisableMoveMapList.Add("OLD");
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.btnDisableMoveMapSaveClick(Form.Ct.btnDisableMoveMapSave);
        Assert.Empty(DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList));
    }

    [Fact]
    public void DisableMoveMapSave_AppliesSortedTrue()
    {
        Form.Ct.lstDisableMoveMap.Items.Clear();
        Form.Ct.lstDisableMoveMap.Items.Add("zzz");
        Form.Ct.lstDisableMoveMap.Items.Add("aaa");
        Form.btnDisableMoveMapSaveClick(Form.Ct.btnDisableMoveMapSave);
        Assert.Equal(new[] { "aaa", "zzz" },
            DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList));
    }

    // ---------------- 不主动攻击怪物页（:997-1078） ----------------

    [Fact]
    public void NoAttackMonListClick_Selected_EnablesDeleteButton()
    {
        Form.Ct.lstNoAttackMonList.Items.Add("鸡");
        Form.Ct.lstNoAttackMonList.SelectedIndex = 0;
        Form.lstNoAttackMonListClick(Form.Ct.lstNoAttackMonList);
        Assert.True(Form.Ct.btnNoAttackMonDel.Enabled);
    }

    [Fact]
    public void NoAttackMonListClick_NoSelection_LeavesButtonUnchanged()
    {
        Form.Ct.btnNoAttackMonDel.Enabled = true;
        Form.Ct.lstNoAttackMonList.SelectedIndex = -1;
        Form.lstNoAttackMonListClick(Form.Ct.lstNoAttackMonList);
        Assert.True(Form.Ct.btnNoAttackMonDel.Enabled);
    }

    [Fact]
    public void NoAttackMonListClick_EmptyList_NoThrow()
    {
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.Ct.lstNoAttackMonList.SelectedIndex = -1;
        Assert.Null(Record.Exception(() => { Form.lstNoAttackMonListClick(Form.Ct.lstNoAttackMonList); }));
    }

    [Fact]
    public void NoAttackMonAdd_AddsSelectedMonsterNames()
    {
        SelectMonList(0, 2);
        Form.btnNoAttackMonAddClick(Form.Ct.btnNoAttackMonAdd);

        Assert.Equal(new[] { "鸡", "僵尸" }, new[]
        {
            Form.Ct.lstNoAttackMonList.Items[0]!.ToString(),
            Form.Ct.lstNoAttackMonList.Items[1]!.ToString(),
        });
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void NoAttackMonAdd_Deduplicates()
    {
        Form.Ct.lstNoAttackMonList.Items.Add("鸡");
        SelectMonList(0, 1, 2);
        Form.btnNoAttackMonAddClick(Form.Ct.btnNoAttackMonAdd);
        Assert.Equal(3, Form.Ct.lstNoAttackMonList.Items.Count);
    }

    [Fact]
    public void NoAttackMonAdd_NoSelection_StillMarksDirty_OriginalAlwaysTrueIf()
    {
        // ★ 差异断言：:1008 `if lstMonList.Items.Count >= 0` 恒真
        SelectMonList();
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.uModValue();
        Form.btnNoAttackMonAddClick(Form.Ct.btnNoAttackMonAdd);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void NoAttackMonAdd_EmptySource_StillMarksDirty()
    {
        OpenWith(maps: new[] { "0" }, monsters: Array.Empty<string>());
        Form.uModValue();
        Form.btnNoAttackMonAddClick(Form.Ct.btnNoAttackMonAdd);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void AddSelectedMons_PureLogic_MatchesHandler()
    {
        SelectMonList(1);
        bool entered = TFrmDummySetting.AddSelectedMons(Form.Ct);
        Assert.True(entered);
        Assert.Equal("鹿", Form.Ct.lstNoAttackMonList.Items[0]!.ToString());
    }

    [Fact]
    public void AddSelectedMons_PureLogic_EmptySource_NoThrow()
    {
        OpenWith(monsters: Array.Empty<string>());
        Assert.Null(Record.Exception(() => { TFrmDummySetting.AddSelectedMons(Form.Ct); }));
    }

    [Fact]
    public void NoAttackMonDel_DeletesCurrentRowOnly()
    {
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.Ct.lstNoAttackMonList.Items.AddRange(new object[] { "A", "B", "C" });
        Form.Ct.lstNoAttackMonList.SelectedIndex = 0;

        Form.btnNoAttackMonDelClick(Form.Ct.btnNoAttackMonDel);

        Assert.Equal(new[] { "B", "C" }, new[]
        {
            Form.Ct.lstNoAttackMonList.Items[0]!.ToString(),
            Form.Ct.lstNoAttackMonList.Items[1]!.ToString(),
        });
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void NoAttackMonDel_NoSelection_DisablesButton()
    {
        Form.Ct.lstNoAttackMonList.Items.Add("A");
        Form.Ct.btnNoAttackMonDel.Enabled = true;
        Form.Ct.lstNoAttackMonList.SelectedIndex = -1;

        Form.btnNoAttackMonDelClick(Form.Ct.btnNoAttackMonDel);

        Assert.False(Form.Ct.btnNoAttackMonDel.Enabled);
        Assert.Single(Form.Ct.lstNoAttackMonList.Items);
    }

    [Fact]
    public void NoAttackMonDel_LastRow_DisablesButton()
    {
        Form.Ct.lstNoAttackMonList.Items.Add("A");
        Form.Ct.lstNoAttackMonList.SelectedIndex = 0;

        Form.btnNoAttackMonDelClick(Form.Ct.btnNoAttackMonDel);

        Assert.Empty(Form.Ct.lstNoAttackMonList.Items);
        Assert.False(Form.Ct.btnNoAttackMonDel.Enabled);
    }

    [Fact]
    public void NoAttackMonAddAll_CopiesWholeMonsterList()
    {
        Form.Ct.lstNoAttackMonList.Items.Add("旧");
        Form.btnNoAttackMonAddAllClick(Form.Ct.btnNoAttackMonAddAll);
        Assert.Equal(3, Form.Ct.lstNoAttackMonList.Items.Count);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void NoAttackMonAddAll_EmptySource_ClearsTarget()
    {
        OpenWith(monsters: Array.Empty<string>());
        Form.Ct.lstNoAttackMonList.Items.Add("旧");
        Form.btnNoAttackMonAddAllClick(Form.Ct.btnNoAttackMonAddAll);
        Assert.Empty(Form.Ct.lstNoAttackMonList.Items);
    }

    [Fact]
    public void NoAttackMonAddAll_LoopBoundIsMonListCount()
    {
        OpenWith(monsters: new[] { "M1", "M2" });
        Form.btnNoAttackMonAddAllClick(Form.Ct.btnNoAttackMonAddAll);
        Assert.Equal(2, Form.Ct.lstNoAttackMonList.Items.Count);
    }

    [Fact]
    public void NoAttackMonDelAll_ClearsAndDisablesButton()
    {
        Form.Ct.lstNoAttackMonList.Items.AddRange(new object[] { "A", "B" });
        Form.Ct.btnNoAttackMonDel.Enabled = true;

        Form.btnNoAttackMonDelAllClick(Form.Ct.btnNoAttackMonDelAll);

        Assert.Empty(Form.Ct.lstNoAttackMonList.Items);
        Assert.False(Form.Ct.btnNoAttackMonDel.Enabled);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void NoAttackMonDelAll_AlreadyEmpty_StillMarksDirty()
    {
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.uModValue();
        Form.btnNoAttackMonDelAllClick(Form.Ct.btnNoAttackMonDelAll);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void NoAttackMonSave_WritesGlobalAndClearsDirtyFlag()
    {
        DummySettingState.SaveListToFile = (_, _) => { };
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.Ct.lstNoAttackMonList.Items.Add("M1");

        Form.btnNoAttackMonSaveClick(Form.Ct.btnNoAttackMonSave);

        Assert.Equal(new[] { "M1" }, DummySettingState.Snapshot(DummySettingState.g_DummyNoActiveAttackMonList));
    }

    [Fact]
    public void NoAttackMonSave_ClearsDirtyFlag_SoLaterButtonSaveSkipsBlock()
    {
        DummySettingState.g_DummyNoActiveAttackMonList.Add("OLD");
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.Ct.lstNoAttackMonList.Items.Add("NEW");
        Form.btnNoAttackMonSaveClick(Form.Ct.btnNoAttackMonSave);

        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.Ct.lstNoAttackMonList.Items.Add("CHANGED");
        Form.Ct.edtDummyHomeMap.Text = "3";
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);

        Assert.Equal(new[] { "NEW" }, DummySettingState.Snapshot(DummySettingState.g_DummyNoActiveAttackMonList));
    }

    [Fact]
    public void NoAttackMonSave_EmptyList_ClearsGlobal()
    {
        DummySettingState.g_DummyNoActiveAttackMonList.Add("OLD");
        Form.Ct.lstNoAttackMonList.Items.Clear();
        Form.btnNoAttackMonSaveClick(Form.Ct.btnNoAttackMonSave);
        Assert.Empty(DummySettingState.Snapshot(DummySettingState.g_DummyNoActiveAttackMonList));
    }

    // ---------------- btn1Click（:552-614，「默认」按钮） ----------------

    [Fact]
    public void Btn1_ResetsSixteenHpMpFieldsToDefaults()
    {
        M2Config.nDummyHPTime_Warrior = 1;
        M2Config.nDummyHPBase_Warrior = 2;
        M2Config.nDummyMPTime_Warrior = 3;
        M2Config.nDummyMPBase_Warrior = 4;
        M2Config.nDummyHPTime_DF = 5;
        M2Config.nDummyHPBase_DF = 6;
        M2Config.nDummyMPTime_DF = 7;
        M2Config.nDummyMPBase_DF = 8;
        M2Config.nDummyHeroHPTime_Warrior = 9;
        M2Config.nDummyHeroHPBase_Warrior = 10;
        M2Config.nDummyHeroMPTime_Warrior = 11;
        M2Config.nDummyHeroMPBase_Warrior = 12;
        M2Config.nDummyHeroHPTime_DF = 13;
        M2Config.nDummyHeroHPBase_DF = 14;
        M2Config.nDummyHeroMPTime_DF = 15;
        M2Config.nDummyHeroMPBase_DF = 16;

        Form.btn1Click(Form.Ct.btn1);

        Assert.Equal(350, M2Config.nDummyHPTime_Warrior);
        Assert.Equal(350, M2Config.nDummyHPTime_DF);
        Assert.Equal(350, M2Config.nDummyHeroHPTime_Warrior);
        Assert.Equal(350, M2Config.nDummyHeroHPTime_DF);
        Assert.Equal(800, M2Config.nDummyMPTime_Warrior);
        Assert.Equal(800, M2Config.nDummyMPTime_DF);
        Assert.Equal(800, M2Config.nDummyHeroMPTime_Warrior);
        Assert.Equal(800, M2Config.nDummyHeroMPTime_DF);
        Assert.Equal(75, M2Config.nDummyHPBase_Warrior);
        Assert.Equal(75, M2Config.nDummyHPBase_DF);
        Assert.Equal(75, M2Config.nDummyHeroHPBase_Warrior);
        Assert.Equal(75, M2Config.nDummyHeroHPBase_DF);
        Assert.Equal(18, M2Config.nDummyMPBase_Warrior);
        Assert.Equal(18, M2Config.nDummyMPBase_DF);
        Assert.Equal(18, M2Config.nDummyHeroMPBase_Warrior);
        Assert.Equal(18, M2Config.nDummyHeroMPBase_DF);
    }

    [Fact]
    public void Btn1_BackfillsSixteenSpins()
    {
        Form.btn1Click(Form.Ct.btn1);

        Assert.Equal(350, (int)Form.Ct.seDummyHPTime_Warrior.Value);
        Assert.Equal(350, (int)Form.Ct.seDummyHPTime_DF.Value);
        Assert.Equal(350, (int)Form.Ct.seDummyHeroHPTime_Warrior.Value);
        Assert.Equal(350, (int)Form.Ct.seDummyHeroHPTime_DF.Value);
        Assert.Equal(800, (int)Form.Ct.seDummyMPTime_Warrior.Value);
        Assert.Equal(800, (int)Form.Ct.seDummyMPTime_DF.Value);
        Assert.Equal(800, (int)Form.Ct.seDummyHeroMPTime_Warrior.Value);
        Assert.Equal(800, (int)Form.Ct.seDummyHeroMPTime_DF.Value);
        Assert.Equal(75, (int)Form.Ct.seDummyHPBase_Warrior.Value);
        Assert.Equal(75, (int)Form.Ct.seDummyHPBase_DF.Value);
        Assert.Equal(75, (int)Form.Ct.seDummyHeroHPBase_Warrior.Value);
        Assert.Equal(75, (int)Form.Ct.seDummyHeroHPBase_DF.Value);
        Assert.Equal(18, (int)Form.Ct.seDummyMPBase_Warrior.Value);
        Assert.Equal(18, (int)Form.Ct.seDummyMPBase_DF.Value);
        Assert.Equal(18, (int)Form.Ct.seDummyHeroMPBase_Warrior.Value);
        Assert.Equal(18, (int)Form.Ct.seDummyHeroMPBase_DF.Value);
    }

    [Fact]
    public void Btn1_DoesNotTouchAddHpMpPercentOrAutoFlags_OriginalScope()
    {
        // ★ 差异断言：`btn1Click` 只动 HP/MP 的 Time/Base（原文 :554-612）
        M2Config.nDummyAddHPPercent = 11;
        M2Config.nDummyAddMPPercent = 22;
        M2Config.boDummyAutoAddHP = false;
        M2Config.boDummyAutoAddMP = false;

        Form.btn1Click(Form.Ct.btn1);

        Assert.Equal(11, M2Config.nDummyAddHPPercent);
        Assert.Equal(22, M2Config.nDummyAddMPPercent);
        Assert.False(M2Config.boDummyAutoAddHP);
        Assert.False(M2Config.boDummyAutoAddMP);
    }

    [Fact]
    public void Btn1_DoesNotTouchHomeMapOrRunFlags()
    {
        M2Config.sDummyHomeMap = "D401";
        M2Config.nDummyHomeX = 5;
        M2Config.boDiableDummyRun = true;
        M2Config.dwDummyWarrorWalkTime = 123;

        Form.btn1Click(Form.Ct.btn1);

        Assert.Equal("D401", M2Config.sDummyHomeMap);
        Assert.Equal(5, M2Config.nDummyHomeX);
        Assert.True(M2Config.boDiableDummyRun);
        Assert.Equal(123, M2Config.dwDummyWarrorWalkTime);
    }

    [Fact]
    public void Btn1_MarksDirty_ModValue()
    {
        Form.uModValue();
        Form.btn1Click(Form.Ct.btn1);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }
}

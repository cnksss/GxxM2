using System.Linq;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Forms.GamePets;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 宠物列表页操作 1:1 覆盖：
///   :392-457 lstGamePetsClick（回填/清空两分支）
///   :534-612 btnAddPetClick（空名/重名/成功三分支）
///   :614-648 btnDelPetClick（未选/未命中/命中三分支）
///   :459-519 btnEditPetClick（未选/回写两分支）
///   :650-654 btnSavePetClick
///   :656-662 lstMonsterListDblClick
///   :664-669 ModValue
///   :671-698 lstMonsterListKeyDown（Ctrl+F）
/// </summary>
[Collection("GamePetsSerial")]
public sealed class GamePetsListOpsTests : GamePetsTestBase
{
    // ---------- lstGamePetsClick (:392-457) ----------

    [Fact]
    public void LstGamePetsClick_WithItems_BackfillsEveryFieldFromSelectedConfig()
    {
        var cfg = Pet("狼");
        OpenWith(cfg);
        // ShowFile1=3 / ShowFile2=9 需要下拉有足够项，否则 Delphi/WinForms 都会落到 -1
        // （见 LstGamePetsClick_ShowFileOutOfRange_ClampsToMinusOne 的专门断言）。
        for (int i = 0; i < 10; i++)
        {
            Form.cbbPetShowFile1.Items.Add("f" + i);
            Form.cbbPetShowFile2.Items.Add("g" + i);
        }
        Form.lstGamePets.SelectedIndex = 0;

        Form.RaiseGamePetsClick();

        Assert.Same(cfg, Form.SelGamePetConfig);
        Assert.True(Form.btnEditPet.Enabled);
        Assert.True(Form.btnDelPet.Enabled);

        Assert.Equal("狼", Form.edtPetName.Text);
        Assert.Equal(1, (int)Form.sePetCaptureRate.Value);
        Assert.True(Form.chkLevelDifference.Checked);
        Assert.Equal(2, (int)Form.seLevelDifference.Value);
        Assert.Equal(50, (int)Form.seHPScale.Value);
        Assert.Equal(3, Form.cbbPetShowFile1.SelectedIndex);
        Assert.Equal(4, (int)Form.sePetShowStart1.Value);
        Assert.Equal(5, (int)Form.sePetShowCount1.Value);
        Assert.Equal(6, (int)Form.sePetShowTime1.Value);
        Assert.Equal(7, (int)Form.sePetShowOffsetX1.Value);
        Assert.Equal(8, (int)Form.sePetShowOffsetY1.Value);
        Assert.Equal(9, Form.cbbPetShowFile2.SelectedIndex);
        Assert.Equal(10, (int)Form.sePetShowStart2.Value);
        Assert.Equal(11, (int)Form.sePetShowCount2.Value);
        Assert.Equal(12, (int)Form.sePetShowTime2.Value);
        Assert.Equal(13, (int)Form.sePetShowOffsetX2.Value);
        Assert.Equal(14, (int)Form.sePetShowOffsetY2.Value);
        Assert.Equal(15, (int)Form.sePetAddHP.Value);
        Assert.Equal(1, Form.cbbPetAddHPType.SelectedIndex);       // IsAddHPRate=true → 1
        Assert.Equal(16, (int)Form.sePetAddDC1.Value);
        Assert.Equal(1, Form.cbbPetAddDCType1.SelectedIndex);
        Assert.Equal(17, (int)Form.sePetAddDC2.Value);
        Assert.Equal(1, Form.cbbPetAddDCType2.SelectedIndex);
        Assert.Equal(18, (int)Form.sePetAddMC1.Value);
        Assert.Equal(1, Form.cbbPetAddMCType1.SelectedIndex);
        Assert.Equal(19, (int)Form.sePetAddMC2.Value);
        Assert.Equal(1, Form.cbbPetAddMCType2.SelectedIndex);
        Assert.Equal(20, (int)Form.sePetAddSC1.Value);
        Assert.Equal(1, Form.cbbPetAddSCType1.SelectedIndex);
        Assert.Equal(21, (int)Form.sePetAddSC2.Value);
        Assert.Equal(1, Form.cbbPetAddSCType2.SelectedIndex);
        Assert.Equal(22, (int)Form.sePetAddAC1.Value);
        Assert.Equal(1, Form.cbbPetAddACType1.SelectedIndex);
        Assert.Equal(23, (int)Form.sePetAddAC2.Value);
        Assert.Equal(1, Form.cbbPetAddACType2.SelectedIndex);
        Assert.Equal(24, (int)Form.sePetAddMAC1.Value);
        Assert.Equal(1, Form.cbbPetAddMACType1.SelectedIndex);
        Assert.Equal(25, (int)Form.sePetAddMAC2.Value);
        Assert.Equal(1, Form.cbbPetAddMACType2.SelectedIndex);
    }

    [Fact]
    public void LstGamePetsClick_ShowFileOutOfRange_ClampsToMinusOne_NoThrow()
    {
        // ★ 差异断言（真实移植陷阱）：原文 `cbbPetShowFile1.ItemIndex := ShowFile1`，
        //   Delphi 对越界值**静默置 -1**；WinForms 的 `SelectedIndex` 越界**抛异常**。
        //   ShowFile* 来自 INI，越界是常态（素材表未装载时下拉只有"根据Appr计算"一项）。
        //   本实现走 SetItemIndex 垫片复刻 Delphi 语义：越界 → -1。
        var cfg = Pet("越界");
        cfg.ShowFile1 = 3;      // 下拉只有 1 项（根据Appr计算）
        cfg.ShowFile2 = 99;
        OpenWith(cfg);
        Form.lstGamePets.SelectedIndex = 0;

        Form.RaiseGamePetsClick();      // 不抛异常

        Assert.Equal(-1, Form.cbbPetShowFile1.SelectedIndex);
        Assert.Equal(-1, Form.cbbPetShowFile2.SelectedIndex);
        Assert.Same(cfg, Form.SelGamePetConfig);
    }

    [Fact]
    public void LstGamePetsClick_ShowFileNegative_AlsoClampsToMinusOne()
    {
        var cfg = Pet("负值");
        cfg.ShowFile1 = -5;
        OpenWith(cfg);
        Form.lstGamePets.SelectedIndex = 0;

        Form.RaiseGamePetsClick();

        Assert.Equal(-1, Form.cbbPetShowFile1.SelectedIndex);
    }

    [Fact]
    public void LstGamePetsClick_WithItems_FalseRateFlagsMapToComboIndexZero()
    {
        // 差异断言：`cbb*.ItemIndex := Integer(Boolean)` —— false → 0（不是 -1、不是保持不变）
        var cfg = Pet("鹿");
        cfg.IsAddHPRate = false;
        OpenWith(cfg);
        Form.lstGamePets.SelectedIndex = 0;

        Form.RaiseGamePetsClick();

        Assert.Equal(0, Form.cbbPetAddHPType.SelectedIndex);
    }

    [Fact]
    public void LstGamePetsClick_EmptyList_DisablesButtonsAndClearsSelection()
    {
        OpenWith();
        Form.SelGamePetConfig = Pet("残留");   // 预算一个残留值，验证被清空

        Form.RaiseGamePetsClick();

        Assert.False(Form.btnEditPet.Enabled);
        Assert.False(Form.btnDelPet.Enabled);
        Assert.Null(Form.SelGamePetConfig);
    }

    [Fact]
    public void LstGamePetsClick_SecondItem_SelectsThatExactConfigObject()
    {
        var a = Pet("A");
        var b = Pet("B");
        OpenWith(a, b);
        Form.lstGamePets.SelectedIndex = 1;

        Form.RaiseGamePetsClick();

        Assert.Same(b, Form.SelGamePetConfig);
        Assert.Equal("B", Form.edtPetName.Text);
    }

    // ---------- btnAddPetClick (:534-612) ----------

    [Fact]
    public void AddPet_EmptyName_ShowsErrorFocusesEditAndAddsNothing()
    {
        OpenWith();
        Form.edtPetName.Text = "   ";      // Trim 后为空

        Form.RaiseAddPet();

        Assert.Single(Messages);
        Assert.Equal("请输入怪物名称！", Messages[0].Text);
        Assert.Equal("错误信息", Messages[0].Caption);
        Assert.Equal(M2Forms.MB_OK | M2Forms.MB_ICONERROR, Messages[0].Flags);
        Assert.Equal(new[] { "edtPetName" }, FocusProbes);
        Assert.Empty(GamePetsState.g_GamePetConfigList);
    }

    [Fact]
    public void AddPet_ZeroLengthName_AlsoRejected()
    {
        OpenWith();
        Form.edtPetName.Text = "";

        Form.RaiseAddPet();

        Assert.Single(Messages);
        Assert.Empty(GamePetsState.g_GamePetConfigList);
    }

    [Fact]
    public void AddPet_DuplicateName_RejectedWithTrimmedNameInMessage()
    {
        // ★ 去重判定用 Trim 后的 S（:539/:547），提示文本也拼 S（:549）
        OpenWith(Pet("僵尸"));
        Form.edtPetName.Text = "  僵尸  ";   // 首尾空格 → Trim 后与已有项重名

        Form.RaiseAddPet();

        Assert.Single(Messages);
        Assert.Equal("怪物 僵尸 已经存在！", Messages[0].Text);
        Assert.Equal(new[] { "edtPetName" }, FocusProbes);
        Assert.Single(GamePetsState.g_GamePetConfigList);
    }

    [Fact]
    public void AddPet_DuplicateIsCaseSensitiveIndexOf_UnlikeGetGamePetConfig()
    {
        // 差异断言：原文此处用 TListBox.Items.IndexOf(S)（**区分大小写**），
        // 而 M2Share.GetGamePetConfig 用 SameText（大小写无关）—— 两者语义不同，不可"统一"。
        OpenWith(Pet("Zombie"));
        Form.edtPetName.Text = "zombie";

        Form.RaiseAddPet();

        Assert.Empty(Messages);                                     // 未判重 → 允许新增
        Assert.Equal(2, GamePetsState.g_GamePetConfigList.Count);

        // 对照：GetGamePetConfig 大小写无关
        Assert.NotNull(GamePetsState.GetGamePetConfig("zombie"));
        Assert.NotNull(GamePetsState.GetGamePetConfig("ZOMBIE"));
        Assert.Null(GamePetsState.GetGamePetConfig("nope"));
    }

    [Fact]
    public void AddPet_UniqueName_StoresUntrimmedTextButListsAndSelectsIt()
    {
        // ★ 差异断言（原文 :557）：判重用 Trim 后的 S，但**存的是 edtPetName.Text 未 Trim**。
        //   而 RefreshGamePetConfigList 列表显示的是 config.Name（未 Trim）。
        OpenWith();
        Form.edtPetName.Text = "  骷髅  ";
        Form.sePetCaptureRate.Value = 33;
        Form.cbbPetShowFile1.Items.Add("x");
        Form.cbbPetShowFile1.SelectedIndex = 0;

        Form.RaiseAddPet();

        Assert.Empty(Messages);
        var cfg = Assert.Single(GamePetsState.g_GamePetConfigList);
        Assert.Equal("  骷髅  ", cfg.Name);                         // 未 Trim
        Assert.Equal(33, cfg.CaptureRate);
        Assert.Equal(0, cfg.ShowFile1);
        Assert.Equal(0, Form.lstGamePets.SelectedIndex);
        Assert.Same(cfg, Form.SelGamePetConfig);
        Assert.True(Form.btnSavePet.Enabled);
        Assert.Equal("  骷髅  ", (string)Form.lstGamePets.Items[0]);
    }

    [Fact]
    public void AddPet_Multiple_SelectsLastAndAppendsInOrder()
    {
        OpenWith();
        Form.edtPetName.Text = "一号";
        Form.RaiseAddPet();
        Form.edtPetName.Text = "二号";
        Form.RaiseAddPet();

        Assert.Equal(2, GamePetsState.g_GamePetConfigList.Count);
        Assert.Equal(new[] { "一号", "二号" }, Form.lstGamePets.Items.Cast<object>().Select(o => (string)o));
        Assert.Equal(1, Form.lstGamePets.SelectedIndex);
        Assert.Same(GamePetsState.g_GamePetConfigList[1], Form.SelGamePetConfig);
    }

    [Fact]
    public void AddPet_ComboUnselected_ItemIndexIsMinusOne_StoredVerbatim()
    {
        // 边界：原文 `ShowFile1 := cbbPetShowFile1.ItemIndex`，未选中时为 -1，**照存 -1**。
        OpenWith();
        Form.edtPetName.Text = "无选中";
        Form.cbbPetShowFile1.SelectedIndex = -1;
        Form.cbbPetAddHPType.SelectedIndex = -1;

        Form.RaiseAddPet();

        var cfg = Assert.Single(GamePetsState.g_GamePetConfigList);
        Assert.Equal(-1, cfg.ShowFile1);
        Assert.False(cfg.IsAddHPRate);        // -1 == 1 → false
    }

    // ---------- btnDelPetClick (:614-648) ----------

    [Fact]
    public void DelPet_NoSelection_ShowsItemWordingErrorButDoesFocusList()
    {
        // ★ 原文缺陷（:621）：删除宠物分支的提示文本误写为"物品"。
        OpenWith(Pet("A"));
        Form.SelGamePetConfig = null;

        Form.RaiseDelPet();

        Assert.Single(Messages);
        Assert.Equal("请选择一个需要修改的物品！", Messages[0].Text);   // 原文如此
        Assert.Equal("错误信息", Messages[0].Caption);
        Assert.Equal(new[] { "lstGamePets" }, FocusProbes);
        Assert.Single(GamePetsState.g_GamePetConfigList);               // 未删除
    }

    [Fact]
    public void DelPet_SelectedInList_RemovesItClearsSelectionAndRefreshes()
    {
        var a = Pet("A");
        var b = Pet("B");
        OpenWith(a, b);
        Form.lstGamePets.SelectedIndex = 0;
        Form.RaiseGamePetsClick();
        Assert.Same(a, Form.SelGamePetConfig);

        Form.RaiseDelPet();

        Assert.Empty(Messages);
        Assert.Single(GamePetsState.g_GamePetConfigList);
        Assert.Same(b, GamePetsState.g_GamePetConfigList[0]);
        Assert.Null(Form.SelGamePetConfig);
        Assert.Equal(new[] { "B" }, Form.lstGamePets.Items.Cast<object>().Select(o => (string)o));
    }

    [Fact]
    public void DelPet_SelectionNotInGlobalList_ShowsDeleteFailed()
    {
        // :629 指针相等判定 —— 选中对象不在 g_GamePetConfigList 里 → IsOK 保持 False
        OpenWith(Pet("A"));
        Form.SelGamePetConfig = Pet("幽灵");    // 不在列表里

        Form.RaiseDelPet();

        Assert.Single(Messages);
        Assert.Equal("删除失败！", Messages[0].Text);
        Assert.Single(GamePetsState.g_GamePetConfigList);   // 原列表未动
        Assert.NotNull(Form.SelGamePetConfig);              // 未清空
    }

    [Fact]
    public void DelPet_IdentityComparisonIsByReference_NotByName()
    {
        // 差异断言：:629 是**指针相等**（ReferenceEquals），不是 Name 相等。
        // 造一个同名但不同实例的对象 → 必须走"删除失败"分支。
        var inList = Pet("同名");
        OpenWith(inList);
        Form.SelGamePetConfig = Pet("同名");    // 同名不同实例

        Form.RaiseDelPet();

        Assert.Single(Messages);
        Assert.Equal("删除失败！", Messages[0].Text);
        Assert.Same(inList, GamePetsState.g_GamePetConfigList[0]);
    }

    [Fact]
    public void DelPet_EmptyListWithNullSelection_TakesErrorBranchNotCrash()
    {
        OpenWith();
        Form.SelGamePetConfig = null;

        Form.RaiseDelPet();

        Assert.Single(Messages);
        Assert.Equal("请选择一个需要修改的物品！", Messages[0].Text);
    }

    [Fact]
    public void DelPet_DeletesOnlyFirstMatchThenBreaks()
    {
        // :635 Break —— 即使同一实例被塞进列表两次，也只删第一个
        var shared = Pet("共享");
        GamePetsState.ClearGamePetsConfig();
        GamePetsState.g_GamePetConfigList.Add(shared);
        GamePetsState.g_GamePetConfigList.Add(shared);
        GamePetsState.g_GamePetConfigList.Add(Pet("别的"));
        OpenWith(GamePetsState.g_GamePetConfigList.ToArray());
        Form.SelGamePetConfig = shared;

        Form.RaiseDelPet();

        Assert.Empty(Messages);
        Assert.Equal(2, GamePetsState.g_GamePetConfigList.Count);   // 3 → 2（只删一个）
    }

    // ---------- btnEditPetClick (:459-519) ----------

    [Fact]
    public void EditPet_NoSelection_ShowsMonsterWordingErrorAndExits()
    {
        // ★ 与 btnDelPetClick 的差异断言：此处提示是"怪物"（:463），删除处是"物品"（:621）。
        OpenWith(Pet("A"));
        Form.SelGamePetConfig = null;

        Form.RaiseEditPet();

        Assert.Single(Messages);
        Assert.Equal("请选择一个需要修改的怪物！", Messages[0].Text);
        Assert.Equal("错误信息", Messages[0].Caption);
        Assert.Equal(new[] { "lstGamePets" }, FocusProbes);
    }

    [Fact]
    public void EditPet_Selected_WritesBackAllControlsIntoConfigAndMarksSave()
    {
        var cfg = Pet("原值");
        OpenWith(cfg);
        Form.lstGamePets.SelectedIndex = 0;
        Form.RaiseGamePetsClick();

        Form.edtPetName.Text = "新名";
        Form.sePetCaptureRate.Value = 77;
        Form.chkLevelDifference.Checked = false;
        Form.seLevelDifference.Value = 8;
        Form.seHPScale.Value = 99;
        Form.cbbPetShowFile1.Items.Add("f1");
        Form.cbbPetShowFile1.SelectedIndex = 0;
        Form.sePetShowStart1.Value = 101;
        Form.sePetShowCount1.Value = 102;
        Form.sePetShowTime1.Value = 103;
        Form.sePetShowOffsetX1.Value = 104;
        Form.sePetShowOffsetY1.Value = 105;
        Form.cbbPetAddHPType.SelectedIndex = 0;    // false
        Form.sePetAddHP.Value = 106;

        Form.RaiseEditPet();

        Assert.Empty(Messages);
        Assert.Equal("新名", cfg.Name);
        Assert.Equal(77, cfg.CaptureRate);
        Assert.False(cfg.EnabledLevelDifference);
        Assert.Equal(8, cfg.LevelDifference);
        Assert.Equal(99, cfg.HPScale);
        Assert.Equal(0, cfg.ShowFile1);
        Assert.Equal(101, cfg.ShowStart1);
        Assert.Equal(102, cfg.ShowCount1);
        Assert.Equal(103, cfg.ShowTime1);
        Assert.Equal(104, cfg.ShowOffsetX1);
        Assert.Equal(105, cfg.ShowOffsetY1);
        Assert.Equal(106, cfg.AddHP);
        Assert.False(cfg.IsAddHPRate);
        Assert.True(Form.btnSavePet.Enabled);
        Assert.Equal(new[] { "新名" }, Form.lstGamePets.Items.Cast<object>().Select(o => (string)o));
    }

    [Fact]
    public void EditPet_RateFlagOnlyTrueWhenComboIndexIsExactlyOne()
    {
        // 差异断言：`= 1` 判定 —— 索引 0 与**任何非 1 值**（含 -1）都为 false。
        var cfg = Pet("X");
        OpenWith(cfg);
        Form.lstGamePets.SelectedIndex = 0;
        Form.RaiseGamePetsClick();

        Form.cbbPetAddDCType1.SelectedIndex = 0;
        Form.cbbPetAddDCType2.SelectedIndex = -1;
        Form.cbbPetAddMCType1.SelectedIndex = 1;

        Form.RaiseEditPet();

        Assert.False(cfg.IsAddDC1Rate);
        Assert.False(cfg.IsAddDC2Rate);
        Assert.True(cfg.IsAddMC1Rate);
    }

    [Fact]
    public void EditPet_KeepsSelectionIdentityAfterRefresh()
    {
        // RefreshGamePetConfigList 会重建列表项 → 选中项对象必须仍是同一实例
        var cfg = Pet("保持");
        OpenWith(cfg);
        Form.lstGamePets.SelectedIndex = 0;
        Form.RaiseGamePetsClick();

        Form.RaiseEditPet();

        Assert.Same(cfg, Form.SelGamePetConfig);
        Assert.Single(GamePetsState.g_GamePetConfigList);
    }

    // ---------- btnSavePetClick (:650-654) ----------

    [Fact]
    public void SavePet_WritesIniFileAndDisablesButton()
    {
        var cfg = Pet("存盘");
        OpenWith(cfg);
        Form.btnSavePet.Enabled = true;

        Form.RaiseSavePet();

        string file = Path.Combine(Dir, "GamePetConfigs.txt");
        Assert.True(File.Exists(file));
        Assert.False(Form.btnSavePet.Enabled);
    }

    [Fact]
    public void SavePet_EmptyList_WritesCountZero()
    {
        OpenWith();
        Form.RaiseSavePet();

        string file = Path.Combine(Dir, "GamePetConfigs.txt");
        Assert.True(File.Exists(file));
        Assert.Contains("count=0", File.ReadAllText(file));
    }

    // ---------- lstMonsterListDblClick (:656-662) ----------

    [Fact]
    public void MonsterListDblClick_CopiesSelectedNameIntoEdit()
    {
        OpenWith();
        Form.lstMonsterList.Items.Add("稻草人");
        Form.lstMonsterList.Items.Add("钉耙猫");
        Form.lstMonsterList.SelectedIndex = 1;

        Form.RaiseMonsterListDblClick();

        Assert.Equal("钉耙猫", Form.edtPetName.Text);
    }

    [Fact]
    public void MonsterListDblClick_NoSelection_DoesNotTouchEdit()
    {
        OpenWith();
        Form.lstMonsterList.Items.Add("稻草人");
        Form.lstMonsterList.SelectedIndex = -1;
        Form.edtPetName.Text = "保留";

        Form.RaiseMonsterListDblClick();

        Assert.Equal("保留", Form.edtPetName.Text);
    }

    [Fact]
    public void MonsterListDblClick_EmptyList_NoThrow()
    {
        OpenWith();
        Form.lstMonsterList.SelectedIndex = -1;
        Form.RaiseMonsterListDblClick();
        Assert.Equal("", Form.edtPetName.Text);
    }

    // ---------- ModValue (:664-669) ----------

    [Fact]
    public void ModValue_SetsBothSaveButtonsEnabled()
    {
        OpenWith();
        Form.btnSavePet.Enabled = false;
        Form.btnSavePetParams.Enabled = false;

        // 通过任一处理器触发 ModValue（原文 :705 等）
        Form.RaiseParamHandler(nameof(Form.chkOpenGamePet), true);

        Assert.True(Form.btnSavePet.Enabled);
        Assert.True(Form.btnSavePetParams.Enabled);
    }

    [Fact]
    public void ModValue_NotCalledWhenBoOpenedFalse()
    {
        // 未打开窗体时处理器早退 → ModValue 不执行 → 按钮保持禁用
        var fresh = StaRunner.New(() => new GamePetsForm());
        Wire(fresh);
        fresh.btnSavePet.Enabled = false;
        fresh.btnSavePetParams.Enabled = false;

        fresh.RaiseParamHandler(nameof(fresh.chkOpenGamePet), true);

        Assert.False(fresh.btnSavePet.Enabled);
        Assert.False(fresh.btnSavePetParams.Enabled);
        Assert.False(M2Config.boOpenGamePet);       // 未写配置

        StaRunner.New(() => fresh.Dispose());
    }

    // ---------- lstMonsterListKeyDown (:671-698) ----------

    [Fact]
    public void MonsterListKeyDown_CtrlF_WithHit_SelectsMatchingRow()
    {
        OpenWith();
        Form.lstMonsterList.Items.Clear();
        Form.lstMonsterList.Items.Add("鸡");
        Form.lstMonsterList.Items.Add("鹿");
        Form.lstMonsterList.Items.Add("羊");
        NextInputQuery = (true, "羊");

        Form.RaiseMonsterListKeyDown(70, true);   // Word('F') = 70, ssCtrl in Shift

        Assert.Equal(2, Form.lstMonsterList.SelectedIndex);
    }

    [Fact]
    public void MonsterListKeyDown_CtrlF_Cancel_DoesNotChangeSelection()
    {
        OpenWith();
        Form.lstMonsterList.Items.Clear();
        Form.lstMonsterList.Items.Add("鸡");
        Form.lstMonsterList.Items.Add("鹿");
        Form.lstMonsterList.SelectedIndex = 0;
        NextInputQuery = (false, "鹿");           // 用户取消

        Form.RaiseMonsterListKeyDown(70, true);

        Assert.Equal(0, Form.lstMonsterList.SelectedIndex);   // :684 Exit，未查找
    }

    [Fact]
    public void MonsterListKeyDown_CtrlF_EmptyInput_DoesNotChangeSelection()
    {
        OpenWith();
        Form.lstMonsterList.Items.Clear();
        Form.lstMonsterList.Items.Add("鸡");
        Form.lstMonsterList.SelectedIndex = -1;
        NextInputQuery = (true, "");              // :685 sMonName = '' → Exit

        Form.RaiseMonsterListKeyDown(70, true);

        Assert.Equal(-1, Form.lstMonsterList.SelectedIndex);
    }

    [Fact]
    public void MonsterListKeyDown_CtrlF_NoHit_LeavesSelectionUnchanged()
    {
        OpenWith();
        Form.lstMonsterList.Items.Clear();
        Form.lstMonsterList.Items.Add("鸡");
        Form.lstMonsterList.SelectedIndex = 0;
        NextInputQuery = (true, "不存在的怪");

        Form.RaiseMonsterListKeyDown(70, true);

        Assert.Equal(0, Form.lstMonsterList.SelectedIndex);
    }

    [Fact]
    public void MonsterListKeyDown_CtrlF_MatchesExactStringCaseSensitively()
    {
        // 差异断言：`Items.Strings[I] = sMonName` 是**精确区分大小写**比较（非 SameText）。
        OpenWith();
        Form.lstMonsterList.Items.Clear();
        Form.lstMonsterList.Items.Add("Zombie");
        NextInputQuery = (true, "zombie");

        Form.RaiseMonsterListKeyDown(70, true);

        Assert.Equal(-1, Form.lstMonsterList.SelectedIndex);   // 未命中
    }

    [Fact]
    public void MonsterListKeyDown_FWithoutCtrl_DoesNothing()
    {
        OpenWith();
        Form.lstMonsterList.Items.Clear();
        Form.lstMonsterList.Items.Add("鸡");
        NextInputQuery = (true, "鸡");

        Form.RaiseMonsterListKeyDown(70, false);   // 无 Ctrl

        Assert.Equal(-1, Form.lstMonsterList.SelectedIndex);
    }

    [Fact]
    public void MonsterListKeyDown_OtherKeys_AreIgnored()
    {
        // 原文 `case Key of Word('F'): ...` 只有 F 一个分支，其余键全部无动作。
        OpenWith();
        Form.lstMonsterList.Items.Clear();
        Form.lstMonsterList.Items.Add("鸡");
        NextInputQuery = (true, "鸡");

        Form.RaiseMonsterListKeyDown(65, true);    // 'A'
        Form.RaiseMonsterListKeyDown(70 - 1, true); // 'E'
        Form.RaiseMonsterListKeyDown(0, true);

        Assert.Equal(-1, Form.lstMonsterList.SelectedIndex);
    }

    [Fact]
    public void MonsterListKeyDown_CtrlF_WithEmptyList_NoThrow()
    {
        OpenWith();
        Form.lstMonsterList.Items.Clear();
        NextInputQuery = (true, "任意");

        Form.RaiseMonsterListKeyDown(70, true);

        Assert.Equal(-1, Form.lstMonsterList.SelectedIndex);
    }
}

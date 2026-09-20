// ============================================================================
// uFrmCustomMagic.pas 窗体主体测试
//   SetControlEnabled :1605-1623 ／ SetConfigChanged :1863-1875 ／ DoOpen :1638-1666
//   FormCreate :1668-1861 ／ NodeClick :1910-2376 ／ cbbClientLevelChange :2378-2509
//   特殊处理器 :2519-4799 ／ btnSave :3601-3627 ／ btnMakeConfigData :3628-3665
//   RebuildCustomMagicListText :3532-3600 ／ ShowCustomMagic :1625-1636
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Forms.CustomMagic;
using TMagicAttackDecAttributesType = GXX.M2Server.Engine.TMagicAttackDecAttributesType;
using TMagicProtectAddAttributesType = GXX.M2Server.Engine.TMagicProtectAddAttributesType;
using TItemElementsType = GXX.M2Server.Engine.TItemElementsType;
using TBreakDefenseType = GXX.M2Server.Engine.TBreakDefenseType;
using TCheckVarType = GXX.M2Server.Engine.TCheckVarType;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("CustomMagicFormLane")]
public sealed class CustomMagicFormCoreTests : CustomMagicTestBase
{
    // ---- SetControlEnabled（原文 :1605-1623）----

    [Fact]
    public void SetControlEnabled_RecursesIntoSheetPanelGroupBox()
    {
        var root = new TPanelSeam();
        var sheet = new TTabSheetSeam();
        var panel = new TPanelSeam();
        var group = new TGroupBoxSeam();
        var edit = new TEditSeam();
        var nested = new TEditSeam();
        var groupInner = new TGroupBoxSeam();
        var deep = new TButtonSeam();

        groupInner.Controls.Add(deep);
        group.Controls.Add(nested);
        group.Controls.Add(groupInner);
        panel.Controls.Add(edit);
        sheet.Controls.Add(group);
        root.Controls.Add(sheet);
        root.Controls.Add(panel);

        TFrmCustomMagic.SetControlEnabled(root, false);

        Assert.False(nested.Enabled);
        Assert.False(edit.Enabled);
        Assert.False(deep.Enabled);
        // 容器自身不被置 Enabled（原文只对非容器置 Enabled）
        Assert.True(sheet.Enabled);
        Assert.True(panel.Enabled);
        Assert.True(group.Enabled);
        Assert.True(groupInner.Enabled);

        TFrmCustomMagic.SetControlEnabled(root, true);
        Assert.True(nested.Enabled);
        Assert.True(edit.Enabled);
        Assert.True(deep.Enabled);
    }

    [Fact]
    public void SetControlEnabled_EmptyContainer_NoThrow()
    {
        TFrmCustomMagic.SetControlEnabled(new TPanelSeam(), false);
        TFrmCustomMagic.SetControlEnabled(new TPanelSeam(), true);
    }

    // ---- SetConfigChanged（原文 :1863-1875）----

    [Fact]
    public void SetConfigChanged_NullConfig_NoOp()
    {
        using var form = new TFrmCustomMagic();
        form.FIsConfigChanged = false;
        form.SetConfigChanged();
        Assert.False(form.FIsConfigChanged);
        Assert.False(form.FCurrentCustomConfig != null);
    }

    [Fact]
    public void SetConfigChanged_WritesChangedAndEnablesSave()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        Assert.False(form.btnSave.Enabled);          // DFM: Enabled = False
        form.SetConfigChanged();
        Assert.True(cfg.IsChanged);
        Assert.True(form.FIsConfigChanged);
        Assert.True(form.btnSave.Enabled);
    }

    [Fact]
    public void SetConfigChanged_False_ClearsChangedFlag()
    {
        var cfg = MakeConfig();
        cfg.SetChanged(true);
        using var form = MakeFormWith(cfg);

        form.SetConfigChanged(false);
        Assert.False(cfg.IsChanged);
        // 原文无条件把 FIsConfigChanged 置 True、btnSave 打开
        Assert.True(form.FIsConfigChanged);
        Assert.True(form.btnSave.Enabled);
    }

    [Fact]
    public void SetConfigChanged_InvalidatesFocusedNode()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var node = form.MainTreeHost!.AddChild(null);
        form.MainTreeHost.FocusedNode = node;

        form.SetConfigChanged();
        Assert.Contains(node, form.MainTreeHost.InvalidatedNodes);
    }

    [Fact]
    public void SetConfigChanged_NoFocusedNode_NoInvalidate()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        form.MainTreeHost!.FocusedNode = null;
        form.SetConfigChanged();
        Assert.Empty(form.MainTreeHost.InvalidatedNodes);
    }

    // ---- ShowCustomMagic（原文 :1625-1636）----

    [Fact]
    public void ShowCustomMagic_ModalOk_ReturnsTrue()
    {
        CustomMagicMessageBoxSeam.ShowModalHandler = () => System.Windows.Forms.DialogResult.OK;
        Assert.True(TFrmCustomMagic.ShowCustomMagic());
    }

    [Fact]
    public void ShowCustomMagic_ModalCancel_ReturnsFalse()
    {
        CustomMagicMessageBoxSeam.ShowModalHandler = () => System.Windows.Forms.DialogResult.Cancel;
        Assert.False(TFrmCustomMagic.ShowCustomMagic());
    }

    [Fact]
    public void ShowCustomMagic_Headless_ReturnsFalse_NoHang()
    {
        CustomMagicMessageBoxSeam.UiEnabled = false;
        CustomMagicMessageBoxSeam.ShowModalHandler = null;
        Assert.False(TFrmCustomMagic.ShowCustomMagic());
    }

    [Fact]
    public void ShowCustomMagic_RunsDoOpen()
    {
        CustomMagicFormGlobals.boSendCustomMagicConfig = true;
        CustomMagicFormGlobals.m_CustomMagicList.Add(MakeConfig());
        CustomMagicMessageBoxSeam.ShowModalHandler = () => System.Windows.Forms.DialogResult.OK;

        Assert.True(TFrmCustomMagic.ShowCustomMagic());
        // DoOpen 执行过（临时窗体已释放，只能通过全局接缝观察）
        Assert.Empty(CustomMagicFormGlobals.ListLockCalls);   // g_MultiThreadRun = false → 不加锁
    }

    // ---- DoOpen（原文 :1638-1666）----

    [Fact]
    public void DoOpen_EmptyList_NoNodes()
    {
        using var form = new TFrmCustomMagic();
        form.DoOpen();
        Assert.Empty(form.MainTreeHost!.Nodes);
    }

    [Fact]
    public void DoOpen_AddsOneNodePerConfig_AndBindsNodeData()
    {
        var a = MakeConfig("技能A", 1);
        var b = MakeConfig("技能B", 2, warr: true);
        CustomMagicFormGlobals.m_CustomMagicList.Add(a);
        CustomMagicFormGlobals.m_CustomMagicList.Add(b);

        using var form = new TFrmCustomMagic();
        form.DoOpen();

        Assert.Equal(2, form.MainTreeHost!.Nodes.Count);
        Assert.Same(a, ((TMagicConfigNodeData)form.MainTreeHost.Nodes[0].Data!).Config);
        Assert.Same(b, ((TMagicConfigNodeData)form.MainTreeHost.Nodes[1].Data!).Config);
    }

    [Fact]
    public void DoOpen_ReadsSendCustomMagicConfigFromGlobals()
    {
        CustomMagicFormGlobals.boSendCustomMagicConfig = true;
        using var form = new TFrmCustomMagic();
        form.DoOpen();
        Assert.True(form.chkSendCustomMagicConfig.Checked);

        CustomMagicFormGlobals.boSendCustomMagicConfig = false;
        form.DoOpen();
        Assert.False(form.chkSendCustomMagicConfig.Checked);
    }

    [Fact]
    public void DoOpen_MultiThread_LocksAndUnlocks()
    {
        CustomMagicFormGlobals.g_MultiThreadRun = true;
        CustomMagicFormGlobals.m_CustomMagicList.Add(MakeConfig());

        using var form = new TFrmCustomMagic();
        form.DoOpen();

        Assert.Equal(new[] { ("LockR", 3), ("UnLockR", 0) }, CustomMagicFormGlobals.ListLockCalls);
    }

    // ---- FormCreate（原文 :1668-1861）----

    [Fact]
    public void FormCreate_FillsComboItemCounts()
    {
        using var form = new TFrmCustomMagic();
        form.FormCreate();

        Assert.Equal(GXX.M2Server.Engine.CustomMagicUtils.MagicPlusLevelNames.Length, form.cbbClientLevel.Items.Count);
        Assert.Equal(GXX.M2Server.Engine.CustomMagicUtils.MagicActionTypeNames.Length, form.cbbClientActionType.Items.Count);
        Assert.Equal(GXX.M2Server.Engine.CustomMagicUtils.MagicSwitchModeNames.Length, form.cbbMagicSwitchMode.Items.Count);
        Assert.Equal(GXX.M2Server.Engine.CustomMagicUtils.MagicWarrNGOptionNames.Length, form.cbbMagicWarrNGOption.Items.Count);
        Assert.Equal(GXX.M2Server.Engine.CustomMagicUtils.CustomMagicLevelNames.Length, form.cbbAttackPowerLevel.Items.Count);
        Assert.Equal(GXX.M2Server.Engine.CustomMagicUtils.CheckVarTypeNames.Length, form.cbbCheckVarType.Items.Count);
        Assert.Equal(2, form.cbbClientFlyDrawMode.Items.Count);
        Assert.Equal(2, form.cbbClientFlyDirCount.Items.Count);
        Assert.Equal(2, form.cbbClientSelfDrawOrder.Items.Count);
        Assert.Equal(3, form.cbbClientSelfDirCalcType.Items.Count);
        Assert.Equal(2, form.cbbOperateMode.Items.Count);
        Assert.Equal(6, form.cbbAttackTarget.Items.Count);
        Assert.Equal(4, form.cbbAttackPowerCalc.Items.Count);
        Assert.Equal(5, form.cbbNeedItem.Items.Count);
        Assert.Equal(new[] { "无", "红毒", "绿毒", "符", "自定义物品" },
            form.cbbNeedItem.Items.ToArray());
    }

    [Fact]
    public void FormCreate_FileCombos_StartWithNone_ThenEffectList()
    {
        CustomMagicFormGlobals.EffectImageList = () => new List<string> { "f1", "f2" };
        using var form = new TFrmCustomMagic();
        form.FormCreate();

        foreach (var combo in new[]
                 {
                     form.cbbClientIconFile, form.cbbClientFlyFile, form.cbbClientFlyEffFile,
                     form.cbbClientSelfFile, form.cbbClientSelfKeepFile, form.cbbClientFastMoveFile,
                     form.cbbClientPreTargetFile, form.cbbClientTargetFile,
                     form.cbbTargetStatus1_File, form.cbbTargetStatus2_File,
                 })
        {
            Assert.Equal(new[] { "无", "f1", "f2" }, combo.Items.ToArray());
        }
    }

    [Fact]
    public void FormCreate_EmptyEffectList_OnlyNone()
    {
        CustomMagicFormGlobals.EffectImageList = () => new List<string>();
        using var form = new TFrmCustomMagic();
        form.FormCreate();
        Assert.Single(form.cbbClientIconFile.Items);
        Assert.Equal("无", form.cbbClientIconFile.Items[0]);
    }

    [Fact]
    public void FormCreate_DisablesMainPageAndClearsState()
    {
        using var form = new TFrmCustomMagic();
        form.FCurrentCustomConfig = MakeConfig();
        form.FCurrentClientConfig = form.FCurrentCustomConfig.ClientConfigs[0];
        form.FCurrentServerConfig = form.FCurrentCustomConfig.ServerConfig;

        form.FormCreate();

        Assert.Null(form.FCurrentCustomConfig);
        Assert.Null(form.FCurrentClientConfig);
        Assert.Null(form.FCurrentServerConfig);
        Assert.Equal(0, form.pgcMain.ActivePageIndex);
        Assert.Equal(0, form.pgcClient.ActivePageIndex);
        // 原文此处调用 SetControlEnabled(pgcMain, False)；托管侧未复刻 DFM 的父子层级，
        // 递归语义由 SetControlEnabled_RecursesIntoSheetPanelGroupBox 单独覆盖。
        Assert.False(form.btnSave.Enabled);   // DFM: btnSave.Enabled = False
    }

    [Fact]
    public void FormCreate_MovesCheckVarLabelsToNeedItemRows()
    {
        using var form = new TFrmCustomMagic();
        form.lblNeedItem.Top = 10;
        form.cbbNeedItem.Top = 12;
        form.lblNeedItemCount.Top = 20;
        form.seNeedItemCount.Top = 22;
        form.lblNeedItemCustomItemName.Top = 30;
        form.edtNeedItemCustomItemName.Top = 32;

        form.FormCreate();

        Assert.Equal(10, form.lblCheckVarName.Top);
        Assert.Equal(12, form.edtCheckVarName.Top);
        Assert.Equal(20, form.lblCheckVarType.Top);
        Assert.Equal(22, form.cbbCheckVarType.Top);
        Assert.Equal(30, form.lblCheckVarValue.Top);
        Assert.Equal(32, form.seCheckVarValue.Top);
        Assert.Equal(30, form.lblCheckVarAdd.Top);       // 原文 lblCheckVarAdd.Top := lblCheckVarValue.Top
        Assert.Equal(32, form.seCheckVarAdd.Top);
    }

    // ---- cbbClientLevelChange（原文 :2378-2509）----

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(int.MinValue)]
    public void CbbClientLevelChange_OutOfRangeIndex_OnlyClearsClientConfig(int index)
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg, 2);
        form.cbbClientLevel.ItemIndex = index;

        form.CbbClientLevelChange();

        Assert.Null(form.FCurrentClientConfig);
        Assert.False(form.FIsConfigChanged);
    }

    [Fact]
    public void CbbClientLevelChange_NullConfig_ReturnsImmediately()
    {
        using var form = new TFrmCustomMagic();
        form.FCurrentClientConfig = null;
        form.cbbClientLevel.ItemIndex = 0;
        form.CbbClientLevelChange();
        Assert.Null(form.FCurrentClientConfig);
    }

    [Fact]
    public void CbbClientLevelChange_LoadsSelectedLevelIntoTControls()
    {
        var cfg = MakeConfig();
        // TMagicClientConfig 是 packed struct（值类型）：必须**就地**通过 holder 字段赋值，
        // 取局部副本（var c = ...）修改不会写回（跨语言易错点，已在报告登记）。
        cfg.ClientConfigs[3].Value.Icon_File = 5;
        cfg.ClientConfigs[3].Value.Icon_Index = 6;
        cfg.ClientConfigs[3].Value.Fly_File = 7;
        cfg.ClientConfigs[3].Value.Fly_StartIndex = 8;
        cfg.ClientConfigs[3].Value.Fly_CalcDir = 1;
        cfg.ClientConfigs[3].Value.Sounds[(int)TMagicSoundType.cmstManWarr].Value = "s1";
        cfg.ClientConfigs[3].Value.Sounds[(int)TMagicSoundType.custMagicFail].Value = "s6";
        cfg.ClientConfigs[3].Value.SelfKeep_KeepTime = 11;
        cfg.ClientConfigs[3].Value.SelfKeep_KeepTime2 = 12;
        cfg.ClientConfigs[3].Value.SelfKeep_File = -1;

        using var form = MakeFormWith(cfg, 0);
        form.cbbClientLevel.ItemIndex = 3;
        form.CbbClientLevelChange();

        Assert.Same(cfg.ClientConfigs[3], form.FCurrentClientConfig);
        Assert.Equal(6, form.cbbClientIconFile.ItemIndex);      // File + 1
        Assert.Equal(6, form.seClientIconIndex.Value);
        Assert.Equal(8, form.cbbClientFlyFile.ItemIndex);
        Assert.Equal(8, form.seClientFlyStartIndex.Value);
        Assert.True(form.chkClientFlyCalcDir.Checked);
        Assert.Equal("s1", form.edtSound1.Text);
        Assert.Equal("s6", form.edtSound6.Text);
        Assert.Equal(11, form.seClientSelfKeepTime.Value);
        Assert.Equal(12, form.seClientSelfKeepTime2.Value);
        Assert.Equal(0, form.cbbClientSelfKeepFile.ItemIndex);  // -1 + 1
    }

    /// <summary>原文 :2502-2506：先 SetConfigChanged(OldChanged) 再把 FIsConfigChanged 还原。</summary>
    [Fact]
    public void CbbClientLevelChange_RestoresConfigChangedFlag()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg, 0);
        form.cbbClientLevel.ItemIndex = 1;
        form.FIsConfigChanged = false;

        form.CbbClientLevelChange();

        Assert.False(form.FIsConfigChanged);
        Assert.False(form.btnSave.Enabled);
        Assert.False(cfg.IsChanged);          // OldChanged = False → SetChanged(False)
    }

    [Fact]
    public void CbbClientLevelChange_KeepsChangedFlagWhenConfigWasChanged()
    {
        var cfg = MakeConfig();
        cfg.SetChanged(true);
        using var form = MakeFormWith(cfg, 0);
        form.cbbClientLevel.ItemIndex = 1;
        form.FIsConfigChanged = true;

        form.CbbClientLevelChange();

        Assert.True(form.FIsConfigChanged);
        Assert.True(cfg.IsChanged);
    }

    // ---- 特殊处理器差异断言 ----

    /// <summary>
    /// 原文缺陷#3（:3278-3286）：守卫用 <c>FCurrentServerConfig</c>，写回却走
    /// <c>FCurrentCustomConfig.ServerConfig</c>。照抄原文。
    /// </summary>
    [Fact]
    public void SeAttackLineWidthChange_GuardsOnServerButWritesThroughCustom()
    {
        var cfgA = MakeConfig("A", 1);
        var cfgB = MakeConfig("B", 2);
        using var form = new TFrmCustomMagic
        {
            FCurrentCustomConfig = cfgB,          // 写回目标
            FCurrentServerConfig = cfgA.ServerConfig,   // 守卫对象（不同对象！）
        };
        form.seAttackLineWidth.Value = 42;

        form.SeAttackLineWidthChange();

        Assert.Equal(42, cfgB.ServerConfig.AttackLineWidth);
        Assert.Equal(0, cfgA.ServerConfig.AttackLineWidth);
    }

    [Fact]
    public void SeAttackLineWidthChange_NullServerGuard_Skips()
    {
        var cfg = MakeConfig();
        using var form = new TFrmCustomMagic { FCurrentCustomConfig = cfg, FCurrentServerConfig = null };
        form.seAttackLineWidth.Value = 42;
        form.SeAttackLineWidthChange();
        Assert.Equal(0, cfg.ServerConfig.AttackLineWidth);
    }

    /// <summary>原文 :3296-3303：<c>cbbAttackPowerLevelChange</c> **没有** SetConfigChanged()。</summary>
    [Fact]
    public void CbbAttackPowerLevelChange_DoesNotMarkChanged()
    {
        var cfg = MakeConfig();
        cfg.ServerConfig.AttackPowerRates[2] = 777;
        using var form = MakeFormWith(cfg);
        form.cbbAttackPowerLevel.ItemIndex = 2;

        form.CbbAttackPowerLevelChange();

        Assert.Equal(777, form.seAttackPowerRate.Value);
        Assert.False(form.FIsConfigChanged);
        Assert.False(cfg.IsChanged);
    }

    /// <summary>原文 :3205-3215 <c>chkFailNoShowEffClick</c> 整段被 {} 注释 → 死代码。</summary>
    [Fact]
    public void ChkFailNoShowEffClick_IsDeadCode()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        form.chkFailNoShowEff.Checked = true;

        form.ChkFailNoShowEffClick();

        Assert.False(form.FIsConfigChanged);
        Assert.False(cfg.IsChanged);
    }

    [Fact]
    public void CbbOperateModeChange_SwitchesVisibilityAndPage()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.cbbOperateMode.ItemIndex = (int)TCustomOperateMode.momAttack;
        form.CbbOperateModeChange();
        Assert.True(form.lblAttackDelay.Visible);
        Assert.True(form.seAttackDelayTime.Visible);
        Assert.True(form.lblAttackDelayTime.Visible);
        Assert.False(form.lblProtectTargetRangeTitle.Visible);
        Assert.Same(form.tsMagicAttack, form.pgcMagicType.ActivePage);

        form.cbbOperateMode.ItemIndex = (int)TCustomOperateMode.momProtect;
        form.CbbOperateModeChange();
        Assert.False(form.lblAttackDelay.Visible);
        Assert.True(form.lblProtectTargetRangeTitle.Visible);
        Assert.True(form.seProtectTargetRange.Visible);
        Assert.Same(form.tsMagicProtected, form.pgcMagicType.ActivePage);
    }

    [Fact]
    public void CbbAttackTargetChange_LineTargetsShowExtraControls()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.cbbAttackTarget.ItemIndex = (int)TCustomAttackTarget.matSingle;
        form.CbbAttackTargetChange();
        Assert.False(form.lblLineAttackAddPower.Visible);
        Assert.False(form.seAttackLineWidth.Visible);

        form.cbbAttackTarget.ItemIndex = (int)TCustomAttackTarget.matLine;
        form.CbbAttackTargetChange();
        Assert.True(form.lblLineAttackAddPower.Visible);
        Assert.True(form.seAttackLineWidth.Visible);        // 原文手写常量 2
        Assert.True(form.lblAttackWidth.Visible);
        Assert.True(form.lblH_AttackWidth.Visible);

        form.cbbAttackTarget.ItemIndex = (int)TCustomAttackTarget.matDir8;
        form.CbbAttackTargetChange();
        Assert.True(form.lblLineAttackAddPower.Visible);
        Assert.False(form.seAttackLineWidth.Visible);       // 只有 matLine(=2) 显示宽度

        form.cbbAttackTarget.ItemIndex = (int)TCustomAttackTarget.matDir16;
        form.CbbAttackTargetChange();
        Assert.True(form.lblLineAttackAddPower.Visible);

        form.cbbAttackTarget.ItemIndex = (int)TCustomAttackTarget.matSwordWide;
        form.CbbAttackTargetChange();
        Assert.False(form.lblLineAttackAddPower.Visible);
    }

    [Fact]
    public void CbbClientActionTypeChange_TogglesCustomActionControls()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.cbbClientActionType.ItemIndex = (int)TMagicActionType.matSpell;
        form.CbbClientActionTypeChange();
        Assert.False(form.seClientActionStartIndex.Enabled);
        Assert.False(form.seClientActionPlayCount.Enabled);
        Assert.False(form.seClientActionEmptyCount.Enabled);
        Assert.False(form.chkClientActionContinue.Enabled);

        form.cbbClientActionType.ItemIndex = (int)TMagicActionType.matCustom;
        form.CbbClientActionTypeChange();
        Assert.True(form.seClientActionStartIndex.Enabled);
        Assert.True(form.seClientActionPlayCount.Enabled);
        Assert.True(form.seClientActionEmptyCount.Enabled);
        Assert.True(form.chkClientActionContinue.Enabled);
    }

    [Fact]
    public void CbbMagicSwitchModeChange_TogglesSwitchControls()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.cbbMagicSwitchMode.ItemIndex = (int)TMagicSwitchMode.msmNone;
        form.CbbMagicSwitchModeChange();
        Assert.False(form.chkSwitchModeNoClose.Enabled);
        Assert.False(form.chkMagicAutoOpen.Enabled);

        form.cbbMagicSwitchMode.ItemIndex = (int)TMagicSwitchMode.msmSwitch;
        form.CbbMagicSwitchModeChange();
        Assert.True(form.chkSwitchModeNoClose.Enabled);
        Assert.True(form.chkMagicAutoOpen.Enabled);
    }

    // ---- Tag 驱动数组族（原文 :3376-3459 / :3672-3700 / :3917-3934 / :4724-4799）----

    [Fact]
    public void AdditionalHandlers_TagInsideRange_WritesSlot()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        var chk = new TCheckBoxSeam { Checked = true };
        form.ChkAdditional0Click(chk, 3);
        Assert.True(cfg.ServerConfig.Additionals[3].Checked);

        var spin = new TSpinEditExSeam { Value = 111 };
        form.SeAdditionalRate0Change(spin, 3);
        Assert.Equal(111, cfg.ServerConfig.Additionals[3].Rate);

        spin.Value = 112;
        form.SeAdditionalRate0_2Change(spin, 3);
        Assert.Equal(112, cfg.ServerConfig.Additionals[3].Rate2);

        spin.Value = 113;
        form.SeAdditionalTime0Change(spin, 3);
        Assert.Equal(113, cfg.ServerConfig.Additionals[3].Time);

        spin.Value = 114;
        form.SeAdditionalTime0_2Change(spin, 3);
        Assert.Equal(114, cfg.ServerConfig.Additionals[3].Time2);

        var cbb = new TComboBoxSeam { ItemIndex = 2 };
        form.CbbAdditionalTime0_1Change(cbb, 3);
        Assert.Equal(2, cfg.ServerConfig.Additionals[3].TimeUnit);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(int.MaxValue)]
    public void AdditionalHandlers_TagOutOfRange_SkipsWriteButKeepsSetChanged(int tag)
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.ChkAdditional0Click(new TCheckBoxSeam { Checked = true }, tag);
        form.SeAdditionalRate0Change(new TSpinEditExSeam { Value = 9 }, tag);
        form.CbbAdditionalTime0_1Change(new TComboBoxSeam { ItemIndex = 1 }, tag);

        foreach (var slot in cfg.ServerConfig.Additionals)
        {
            Assert.False(slot.Checked);
            Assert.Equal(0, slot.Rate);
            Assert.Equal(0, slot.TimeUnit);
        }
        Assert.True(form.FIsConfigChanged);      // 原文 SetConfigChanged 在 if 之外
    }

    [Fact]
    public void AdditionalHandlers_WrongSenderType_Skips()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.ChkAdditional0Click(new TEditSeam(), 0);
        form.SeAdditionalRate0Change(new TCheckBoxSeam(), 0);
        form.CbbAdditionalTime0_1Change(new TSpinEditExSeam(), 0);

        Assert.False(form.FIsConfigChanged);
    }

    [Fact]
    public void CallMonsterHandlers_TagInsideRange()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.EdtCallMonster1Change(new TEditSeam { Text = "稻草人" }, 0);
        form.EdtCallMonster1Change(new TEditSeam { Text = "钉耙猫" }, 1);
        Assert.Equal("稻草人", cfg.ServerConfig.GetCallMonster(0));
        Assert.Equal("钉耙猫", cfg.ServerConfig.GetCallMonster(1));

        form.SeCallMonsterNum1Change(new TSpinEditExSeam { Value = 5 }, 1);
        Assert.Equal(5, cfg.ServerConfig.CallMonsterNums[1]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void CallMonsterHandlers_TagOutOfRange_Skips(int tag)
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.EdtCallMonster1Change(new TEditSeam { Text = "x" }, tag);
        form.SeCallMonsterNum1Change(new TSpinEditExSeam { Value = 5 }, tag);

        Assert.Equal("", cfg.ServerConfig.GetCallMonster(0));
        Assert.Equal("", cfg.ServerConfig.GetCallMonster(1));
        Assert.Equal(0, cfg.ServerConfig.CallMonsterNums[0]);
        Assert.Equal(0, cfg.ServerConfig.CallMonsterNums[1]);
        Assert.True(form.FIsConfigChanged);
    }

    [Fact]
    public void EdtSound1Change_NullClientConfig_Returns()
    {
        var cfg = MakeConfig();
        using var form = new TFrmCustomMagic { FCurrentCustomConfig = cfg, FCurrentClientConfig = null };
        form.EdtSound1Change(new TEditSeam { Text = "s" }, 0);
        Assert.Equal("", cfg.ClientConfigs[0].Value.Sounds[0].Value);
        Assert.False(form.FIsConfigChanged);
    }

    [Fact]
    public void EdtSound1Change_WritesTaggedSlot()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg, 1);
        form.EdtSound1Change(new TEditSeam { Text = "snd" }, (int)TMagicSoundType.custMagicFail);
        Assert.Equal("snd", cfg.ClientConfigs[1].Value.Sounds[(int)TMagicSoundType.custMagicFail].Value);
        Assert.True(form.FIsConfigChanged);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void EdtSound1Change_TagOutOfRange_Skips(int tag)
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        form.EdtSound1Change(new TEditSeam { Text = "snd" }, tag);
        Assert.False(form.FIsConfigChanged);
    }

    [Fact]
    public void MagicACHandlers_TagInsideRange()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        int tag = (int)TBreakDefenseType.bdtHeroMagDefense;

        form.ChkMagicACHumClick(new TCheckBoxSeam { Checked = true }, tag);
        form.SeMagicACHumRateChange(new TSpinEditSeam { Value = 1 }, tag);
        form.SeMagicACHumRateAddChange(new TSpinEditSeam { Value = 2 }, tag);
        form.SeMagicACHumValueChange(new TSpinEditSeam { Value = 3 }, tag);
        form.SeMagicACHumValueAddChange(new TSpinEditSeam { Value = 4 }, tag);

        var d = cfg.ServerConfig.AttackBreakDefense[tag];
        Assert.True(d.IsChecked);
        Assert.Equal(1, d.Rate);
        Assert.Equal(2, d.RateAdd);
        Assert.Equal(3, d.Value);
        Assert.Equal(4, d.ValueAdd);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void MagicACHandlers_TagOutOfRange_Skips(int tag)
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.ChkMagicACHumClick(new TCheckBoxSeam { Checked = true }, tag);
        form.SeMagicACHumRateChange(new TSpinEditSeam { Value = 1 }, tag);

        foreach (var d in cfg.ServerConfig.AttackBreakDefense)
        {
            Assert.False(d.IsChecked);
            Assert.Equal(0, d.Rate);
        }
        Assert.True(form.FIsConfigChanged);
    }

    // ---- chkCheckVarValueClick（原文 :4183-4208）----

    [Fact]
    public void ChkCheckVarValueClick_VisibilityMatrix()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        form.chkCheckVarValue.Checked = false;
        form.ChkCheckVarValueClick();
        Assert.False(cfg.ServerConfig.IsCheckVarValue);
        Assert.True(form.lblNeedItem.Visible);
        Assert.True(form.cbbNeedItem.Visible);
        Assert.True(form.chkNeedItemUseBagItem.Visible);
        Assert.False(form.lblCheckVarName.Visible);
        Assert.False(form.seCheckVarAdd.Visible);

        form.chkCheckVarValue.Checked = true;
        form.ChkCheckVarValueClick();
        Assert.True(cfg.ServerConfig.IsCheckVarValue);
        Assert.False(form.lblNeedItem.Visible);
        Assert.False(form.edtNeedItemCustomItemName.Visible);
        Assert.True(form.lblCheckVarName.Visible);
        Assert.True(form.edtCheckVarName.Visible);
        Assert.True(form.cbbCheckVarType.Visible);
        Assert.True(form.seCheckVarValue.Visible);
        Assert.True(form.seCheckVarAdd.Visible);
    }

    // ---- chkSendCustomMagicConfigClick / btnCopyConfig（原文 :3666-3671 / :4046-4056）----

    [Fact]
    public void ChkSendCustomMagicConfigClick_WritesGlobalAndIni()
    {
        using var form = new TFrmCustomMagic();
        form.chkSendCustomMagicConfig.Checked = true;

        form.ChkSendCustomMagicConfigClick();

        Assert.True(CustomMagicFormGlobals.boSendCustomMagicConfig);
        Assert.Equal(("Setup", "SendCustomMagicConfig", true), CustomMagicFormGlobals.ConfigBools[0]);
    }

    [Fact]
    public void BtnCopyConfigClick_CopiesSelectedLevelToDest()
    {
        var cfg = MakeConfig();
        cfg.ClientConfigs[1].Value.Icon_Index = 9;
        using var form = MakeFormWith(cfg, 1);
        form.cbbClientLevel.ItemIndex = 1;

        TFrmCustomMagic.ShowCustomMagicCopySettingHandler = (src, dest) =>
        {
            Assert.Equal(TMagicPlusLevel.mpl1_3, src);
            dest.Value = TMagicPlusLevel.mpl7_9;
            return true;
        };

        try
        {
            form.BtnCopyConfigClick();
            Assert.Equal(9, cfg.ClientConfigs[3].Value.Icon_Index);
            Assert.True(form.FIsConfigChanged);
        }
        finally
        {
            TFrmCustomMagic.ShowCustomMagicCopySettingHandler = (_, _) => false;
        }
    }

    [Fact]
    public void BtnCopyConfigClick_DialogCancelled_NoCopy()
    {
        var cfg = MakeConfig();
        cfg.ClientConfigs[1].Value.Icon_Index = 9;
        using var form = MakeFormWith(cfg, 1);
        form.cbbClientLevel.ItemIndex = 1;
        form.BtnCopyConfigClick();
        Assert.Equal(0, cfg.ClientConfigs[3].Value.Icon_Index);
        Assert.False(form.FIsConfigChanged);
    }

    [Fact]
    public void BtnCopyConfigClick_NullConfig_NoDialog()
    {
        bool called = false;
        TFrmCustomMagic.ShowCustomMagicCopySettingHandler = (_, _) => { called = true; return true; };
        try
        {
            using var form = new TFrmCustomMagic { FCurrentCustomConfig = null };
            form.BtnCopyConfigClick();
            Assert.False(called);
        }
        finally
        {
            TFrmCustomMagic.ShowCustomMagicCopySettingHandler = (_, _) => false;
        }
    }

    // ---- btnSaveClick（原文 :3601-3627）----

    [Fact]
    public void BtnSaveClick_SavesOnlyChangedConfigs()
    {
        var changed = MakeConfig("变更", 1);
        changed.SetChanged(true);
        var clean = MakeConfig("未变", 2);
        CustomMagicFormGlobals.m_CustomMagicList.Add(changed);
        CustomMagicFormGlobals.m_CustomMagicList.Add(clean);

        var saved = new List<TCustomMagicConfig>();
        CustomMagicConfigDefaults.SaveToIniFile = saved.Add;
        try
        {
            using var form = new TFrmCustomMagic();
            form.DoOpen();
            form.FIsConfigChanged = true;
            form.btnSave.Enabled = true;

            form.BtnSaveClick();

            Assert.Single(saved);
            Assert.Same(changed, saved[0]);
            Assert.Equal(1, form.TreeHosts["vstAttackDecAttr"].EndEditNodeCalls);
            Assert.Equal(1, form.MainTreeHost!.InvalidateCalls);
            Assert.Equal(1, CustomMagicFormGlobals.ResetMagicCDListCalls);
            Assert.Equal(1, CustomMagicFormGlobals.SendServerConfigCalls);
            Assert.False(form.FIsConfigChanged);
            Assert.False(form.btnSave.Enabled);
        }
        finally
        {
            CustomMagicConfigDefaults.SaveToIniFile = null;
        }
    }

    [Fact]
    public void BtnSaveClick_EmptyList_StillRebuildsAndSends()
    {
        using var form = new TFrmCustomMagic();
        form.BtnSaveClick();

        Assert.Equal(0, CustomMagicFormGlobals.g_CustomMagicListTextLen);
        Assert.Equal(1, CustomMagicFormGlobals.ResetMagicCDListCalls);
        Assert.Equal(1, CustomMagicFormGlobals.SendServerConfigCalls);
        Assert.False(form.FIsConfigChanged);
    }

    // ---- RebuildCustomMagicListText（原文 :3532-3600）----

    [Fact]
    public void RebuildCustomMagicListText_EmptyList_ZeroLength()
    {
        using var form = new TFrmCustomMagic();
        form.RebuildCustomMagicListText();

        Assert.Equal(0, CustomMagicFormGlobals.g_CustomMagicListTextLen);
        Assert.NotNull(CustomMagicFormGlobals.g_CustomMagicListText);
        Assert.Empty(CustomMagicFormGlobals.g_CustomMagicListText!);
        Assert.Equal(0u, CustomMagicFormGlobals.g_CustomMagicListTextCRC);
    }

    [Fact]
    public void RebuildCustomMagicListText_PayloadLengthIsCountTimesRecordSize()
    {
        CustomMagicFormGlobals.m_CustomMagicList.Add(MakeConfig("A", 1));
        CustomMagicFormGlobals.m_CustomMagicList.Add(MakeConfig("B", 2, warr: true));

        byte[]? seen = null;
        int seenLen = -1;
        CustomMagicFormGlobals.ZLibCompressBuffer = (buf, len) =>
        {
            seen = buf;
            seenLen = len;
            return new byte[] { 0xAA, 0xBB, 0xCC };
        };

        using var form = new TFrmCustomMagic();
        form.RebuildCustomMagicListText();

        Assert.Equal(2 * TFrmCustomMagic.SizeOfClientConfig(), seenLen);
        Assert.Equal(seenLen, CustomMagicFormGlobals.g_CustomMagicListTextLen);
        Assert.Equal(new byte[] { 0xAA, 0xBB, 0xCC }, CustomMagicFormGlobals.g_CustomMagicListText);
        Assert.NotNull(seen);
        // 原文多分配 1 字节（GetMem(InBuf, InBytes + 1)）
        Assert.Equal(seenLen + 1, seen!.Length);
    }

    [Fact]
    public void RebuildCustomMagicListText_WritesMagicIdAndWarrRange()
    {
        var cfg = MakeConfig("战士技", 5001, warr: true);
        cfg.ServerConfig.AttackNearRange = 7;
        cfg.ServerConfig.IsAttackUseNG = true;
        cfg.ServerConfig.NoChangeDir = true;
        CustomMagicFormGlobals.m_CustomMagicList.Add(cfg);

        byte[]? payload = null;
        CustomMagicFormGlobals.ZLibCompressBuffer = (buf, len) => { payload = buf; return new byte[] { 1 }; };

        using var form = new TFrmCustomMagic();
        form.RebuildCustomMagicListText();

        Assert.NotNull(payload);
        int size = TFrmCustomMagic.SizeOfClientConfig();
        var cc = System.Runtime.InteropServices.MemoryMarshal.Read<TClientCustomMagicConfig>(payload!);
        Assert.Equal(5001, cc.wMagicId);
        Assert.Equal(1, cc.boIsMagicWarr);
        Assert.Equal(7, cc.btNearAttackRange);
        Assert.Equal(1, cc.IsAttackUseNG);
        Assert.Equal(1, cc.NoChangeDir);
        Assert.Equal(size, System.Runtime.InteropServices.Marshal.SizeOf<TClientCustomMagicConfig>());
    }

    [Fact]
    public void RebuildCustomMagicListText_NonWarrClampsNearAttackRangeToOne()
    {
        var cfg = MakeConfig("法师技", 5002, warr: false);
        cfg.ServerConfig.AttackNearRange = 9;
        CustomMagicFormGlobals.m_CustomMagicList.Add(cfg);

        byte[]? payload = null;
        CustomMagicFormGlobals.ZLibCompressBuffer = (buf, len) => { payload = buf; return new byte[] { 1 }; };

        using var form = new TFrmCustomMagic();
        form.RebuildCustomMagicListText();

        var cc = System.Runtime.InteropServices.MemoryMarshal.Read<TClientCustomMagicConfig>(payload!);
        Assert.Equal(0, cc.boIsMagicWarr);
        Assert.Equal(1, cc.btNearAttackRange);
    }

    /// <summary>原文 :286-299：IsCheckVarValue 时把需求物品四项清零。</summary>
    [Fact]
    public void RebuildCustomMagicListText_CheckVarValueClearsNeedItem()
    {
        var cfg = MakeConfig("A", 1);
        cfg.ServerConfig.IsCheckVarValue = false;
        cfg.ServerConfig.NeedItem = TMagicNeedItem.meiFu;
        cfg.ServerConfig.NeedItemCount = 3;
        cfg.ServerConfig.NeedItemCustomItemName = "自定义";
        cfg.ServerConfig.NeedItemUseBagItem = true;
        CustomMagicFormGlobals.m_CustomMagicList.Add(cfg);

        byte[]? payload = null;
        CustomMagicFormGlobals.ZLibCompressBuffer = (buf, len) => { payload = buf; return new byte[] { 1 }; };
        using var form = new TFrmCustomMagic();
        form.RebuildCustomMagicListText();
        var cc = System.Runtime.InteropServices.MemoryMarshal.Read<TClientCustomMagicConfig>(payload!);
        Assert.Equal(TMagicNeedItem.meiFu, cc.NeedItem);
        Assert.Equal(3, cc.NeedItemCount);
        Assert.Equal("自定义", cc.NeedItemCustomItemNameStr);
        Assert.Equal(1, cc.NeedItemUseBagItem);

        cfg.ServerConfig.IsCheckVarValue = true;
        form.RebuildCustomMagicListText();
        cc = System.Runtime.InteropServices.MemoryMarshal.Read<TClientCustomMagicConfig>(payload!);
        Assert.Equal(TMagicNeedItem.meiNone, cc.NeedItem);
        Assert.Equal(0, cc.NeedItemCount);
        Assert.Equal("", cc.NeedItemCustomItemNameStr);
        Assert.Equal(0, cc.NeedItemUseBagItem);
    }

    [Fact]
    public void RebuildCustomMagicListText_CompressesWithConfiguredSeam()
    {
        CustomMagicFormGlobals.m_CustomMagicList.Add(MakeConfig());
        // 还原默认（EDcode.zLibCompressBuffer）后走真实压缩
        CustomMagicFormGlobals.ZLibCompressBuffer = GXX.Core.Protocol.EDcode.zLibCompressBuffer;

        using var form = new TFrmCustomMagic();
        form.RebuildCustomMagicListText();

        Assert.NotNull(CustomMagicFormGlobals.g_CustomMagicListText);
        Assert.Equal(TFrmCustomMagic.SizeOfClientConfig(), CustomMagicFormGlobals.g_CustomMagicListTextLen);
        Assert.NotEqual(0u, CustomMagicFormGlobals.g_CustomMagicListTextCRC);
    }

    // ---- btnMakeConfigDataClick（原文 :3628-3665）----

    [Fact]
    public void BtnMakeConfigDataClick_DialogCancelled_ExitsAfterSetCurrentDirectory()
    {
        CustomMagicFormGlobals.ApplicationExeName = @"C:\srv\M2Server.exe";
        using var form = new TFrmCustomMagic();
        form.dlgSaveMagics.ExecuteHandler = () => false;
        form.dlgSaveMagics.FileName = "should-not-be-used.dat";

        form.BtnMakeConfigDataClick();

        Assert.Equal(1, form.dlgSaveMagics.ExecuteCalls);
        Assert.Equal(new[] { @"C:\srv" }, CustomMagicFormGlobals.SetCurrentDirectoryCalls);
        Assert.Empty(CustomMagicMessageBoxSeam.ShownMessages);
        Assert.Empty(CustomMagicFormGlobals.ConfigStrings);
    }

    [Fact]
    public void BtnMakeConfigDataClick_PreFillsFileNameFromGlobal()
    {
        CustomMagicFormGlobals.sCustomMagicClientConfigFileName = @"D:\old\pre.dat";
        CustomMagicFormGlobals.ApplicationExeName = @"C:\srv\M2Server.exe";
        using var form = new TFrmCustomMagic();
        form.dlgSaveMagics.ExecuteHandler = () => false;

        form.BtnMakeConfigDataClick();

        Assert.Equal(@"D:\old\pre.dat", form.dlgSaveMagics.FileName);
    }

    [Fact]
    public void BtnMakeConfigDataClick_SavesWithDatExtension_AndWritesIni()
    {
        CustomMagicFormGlobals.ApplicationExeName = @"C:\srv\M2Server.exe";
        CustomMagicFormGlobals.m_CustomMagicList.Add(MakeConfig("A", 1));
        var savedTo = "";
        CustomMagicFormGlobals.SaveCustomMagicClientConfigs = (list, file) =>
        {
            Assert.Single(list);
            savedTo = file;
        };

        using var form = new TFrmCustomMagic();
        form.dlgSaveMagics.ExecuteHandler = () => true;
        form.dlgSaveMagics.FileName = @"C:\out\magics.txt";

        form.BtnMakeConfigDataClick();

        Assert.Equal(@"C:\out\magics.dat", savedTo);
        Assert.Equal(@"C:\out\magics.dat", CustomMagicFormGlobals.sCustomMagicClientConfigFileName);
        Assert.Equal(("Setup", "CustomMagicClientConfigFileName", @"C:\out\magics.dat"),
            CustomMagicFormGlobals.ConfigStrings[0]);
        Assert.Equal(new[] { "已经生成自定义技能登录器配置文件" }, CustomMagicMessageBoxSeam.ShownMessages);
        // 原文只有一次 SetCurrentDirectory（成功分支 :3635 一处；失败分支 :3633 与 :3635 各一处）
        Assert.Single(CustomMagicFormGlobals.SetCurrentDirectoryCalls);
    }

    [Fact]
    public void BtnMakeConfigDataClick_MultiThread_LocksWith5()
    {
        CustomMagicFormGlobals.g_MultiThreadRun = true;
        CustomMagicFormGlobals.ApplicationExeName = @"C:\srv\M2Server.exe";
        using var form = new TFrmCustomMagic();
        form.dlgSaveMagics.ExecuteHandler = () => true;
        form.dlgSaveMagics.FileName = "m";

        form.BtnMakeConfigDataClick();

        Assert.Equal(new[] { ("LockR", 5), ("UnLockR", 0) }, CustomMagicFormGlobals.ListLockCalls);
    }

    /// <summary>接缝：uCustomMagicUtils.pas SaveCustomMagicClientConfigs 未移植时的兜底只序列化。</summary>
    [Fact]
    public void BtnMakeConfigDataClick_FallbackSerializerProducesBytes()
    {
        TFrmCustomMagic.LastSavedConfigBytes = null;
        CustomMagicFormGlobals.ApplicationExeName = @"C:\srv\M2Server.exe";
        CustomMagicFormGlobals.m_CustomMagicList.Add(MakeConfig("A", 1));

        using var form = new TFrmCustomMagic();
        form.dlgSaveMagics.ExecuteHandler = () => true;
        form.dlgSaveMagics.FileName = "m";

        form.BtnMakeConfigDataClick();

        Assert.NotNull(TFrmCustomMagic.LastSavedConfigBytes);
        Assert.Equal(TFrmCustomMagic.SizeOfClientConfig(), TFrmCustomMagic.LastSavedConfigBytes!.Length);
    }

    // ---- vstCustomMagic 三事件 / NodeClick（原文 :1877-2376）----

    [Fact]
    public void VstCustomMagicGetNodeDataSize_IsFour()
    {
        using var form = new TFrmCustomMagic();
        form.VstCustomMagicGetNodeDataSize(out int size);
        Assert.Equal(4, size);
    }

    [Fact]
    public void VstCustomMagicGetText_AndDrawText()
    {
        var cfg = MakeConfig("技能X", 1);
        using var form = new TFrmCustomMagic();
        var node = form.MainTreeHost!.AddChild(null);
        ((TMagicConfigNodeData)node.Data!).Config = cfg;

        Assert.Equal("技能X", form.VstCustomMagicGetText(form.MainTreeHost, node));
        Assert.Equal("", form.VstCustomMagicGetText(form.MainTreeHost, null));

        int fontColor = 0x00FF00;
        form.MainTreeHost.FontColor = fontColor;
        Assert.Equal(fontColor, form.VstCustomMagicDrawText(form.MainTreeHost, node, false));

        cfg.SetChanged(true);
        Assert.Equal(CustomMagicColors.clRed, form.VstCustomMagicDrawText(form.MainTreeHost, node, false));

        cfg.SetChanged(false);
        form.MainTreeHost.SetSelected(node, true);
        form.MainTreeHost.Focused = true;
        Assert.Equal(CustomMagicColors.clHighlightText, form.VstCustomMagicDrawText(form.MainTreeHost, node, false));
    }

    [Fact]
    public void VstCustomMagicNodeClick_NoFocusedNode_ClearsState()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        form.MainTreeHost!.FocusedNode = null;

        form.VstCustomMagicNodeClick(form.MainTreeHost, new THitInfo());

        Assert.Null(form.FCurrentCustomConfig);
        Assert.Null(form.FCurrentClientConfig);
        Assert.Null(form.FCurrentServerConfig);
    }

    [Fact]
    public void VstCustomMagicNodeClick_LoadsWholeConfig()
    {
        var cfg = MakeConfig("技能A", 10, warr: false);
        cfg.ClientBaseConfig.MagicLock = 1;
        cfg.ClientBaseConfig.MagicLockSelf = 1;
        cfg.ClientBaseConfig.NotRaiseHand = 1;
        cfg.ClientBaseConfig.MagicActionType = TMagicActionType.matCustom;
        cfg.ServerConfig.OperateMode = TCustomOperateMode.momAttack;
        cfg.ServerConfig.AttackTarget = TCustomAttackTarget.matLine;
        cfg.ServerConfig.IsAttackUseNG = true;
        cfg.ServerConfig.FailMsg = "失败文案";
        cfg.ServerConfig.NeedItem = TMagicNeedItem.meiGreenPoison;
        cfg.ServerConfig.IsCheckVarValue = false;
        cfg.ServerConfig.UseInterval = 1234;
        cfg.ServerConfig.Additionals[0].Rate = 9;
        cfg.ServerConfig.AttackSubAttrib[(int)TMagicAttackDecAttributesType.daMAC].IsChecked = true;
        cfg.ServerConfig.ProtectAddAttrib[(int)TMagicProtectAddAttributesType.aaHide].IsChecked = true;
        cfg.ServerConfig.AttackBreakDefense[(int)TBreakDefenseType.bdtMonMagDefense].Value = 66;
        cfg.ServerConfig.ProtectAddHpSlowCount = 4;
        cfg.ClientConfigs[0].Value.Icon_Index = 77;

        CustomMagicFormGlobals.m_CustomMagicList.Add(cfg);
        using var form = new TFrmCustomMagic();
        form.DoOpen();
        var node = form.MainTreeHost!.Nodes[0];
        form.MainTreeHost.FocusedNode = node;

        form.VstCustomMagicNodeClick(form.MainTreeHost, new THitInfo { HitNode = node, HitColumn = 0 });

        Assert.Same(cfg, form.FCurrentCustomConfig);
        Assert.Same(cfg.ClientConfigs[0], form.FCurrentClientConfig);   // cbbClientLevelChange 已接管
        Assert.Same(cfg.ServerConfig, form.FCurrentServerConfig);

        Assert.True(form.chkClientLock.Checked);
        Assert.True(form.chkClientLockSelf.Checked);
        Assert.True(form.chkClientNotRaiseHand.Checked);
        Assert.Equal((int)TMagicActionType.matCustom, form.cbbClientActionType.ItemIndex);
        Assert.True(form.seClientActionStartIndex.Enabled);
        Assert.Equal((int)TCustomOperateMode.momAttack, form.cbbOperateMode.ItemIndex);
        Assert.Equal((int)TCustomAttackTarget.matLine, form.cbbAttackTarget.ItemIndex);
        Assert.True(form.chkAttackUseNG.Checked);
        Assert.Equal("失败文案", form.edtFailMsg.Text);
        Assert.Equal((int)TMagicNeedItem.meiGreenPoison, form.cbbNeedItem.ItemIndex);
        Assert.Equal(1234, form.seUseInterval.Value);
        Assert.Equal(9, form.seAdditionalRate0.Value);
        Assert.Equal(77, form.seClientIconIndex.Value);
        Assert.Equal(4, form.seProtectAddHPSlowCount.Value);
        Assert.Equal(66, form.seDefenceMonValue.Value);
        Assert.False(form.chkDefenceMon.Checked);   // 只设了 Value，未设 IsChecked

        // 三棵树各按枚举项数建了节点
        Assert.Equal(9, form.TreeHosts["vstAttackDecAttr"].Nodes.Count);
        Assert.Equal(25, form.TreeHosts["vstDecElement"].Nodes.Count);
        Assert.Equal(16, form.TreeHosts["vstProtectedAddAttr"].Nodes.Count);
        Assert.Equal(25, form.TreeHosts["vstAddElement"].Nodes.Count);

        // 勾选状态来自配置
        Assert.Equal(TCheckState.csCheckedNormal, form.TreeHosts["vstAttackDecAttr"].Nodes[1].CheckState);
        Assert.Equal(TCheckState.csUnCheckedNormal, form.TreeHosts["vstAttackDecAttr"].Nodes[0].CheckState);
        Assert.Equal(TCheckState.csCheckedNormal, form.TreeHosts["vstProtectedAddAttr"].Nodes[15].CheckState);

        // 节点数据指向配置内的**同一对象**（原文 := @... 的指针等价）
        var decNode = form.TreeHosts["vstAttackDecAttr"].Nodes[1];
        var decData = (TAttackDecAttribData)form.TreeHosts["vstAttackDecAttr"].GetNodeData(decNode)!;
        Assert.Same(cfg.ServerConfig.AttackSubAttrib[1], decData.Data);
        Assert.Equal(TMagicAttackDecAttributesType.daMAC, decData.AttribType);
    }

    [Fact]
    public void VstCustomMagicNodeClick_WarrMagic_ForcesNearAttackAndDisablesFly()
    {
        var cfg = MakeConfig("战士技", 1, warr: true);
        cfg.ServerConfig.OperateMode = TCustomOperateMode.momProtect;
        cfg.ClientBaseConfig.MagicSwitchMode = TMagicSwitchMode.msmSwitch;
        cfg.ServerConfig.EnableHitPoint = true;

        CustomMagicFormGlobals.m_CustomMagicList.Add(cfg);
        using var form = new TFrmCustomMagic();
        form.DoOpen();
        form.MainTreeHost!.FocusedNode = form.MainTreeHost.Nodes[0];

        form.VstCustomMagicNodeClick(form.MainTreeHost, new THitInfo());

        Assert.Equal("战士技能", form.txtMagicWarr.Caption);
        Assert.Equal(CustomMagicColors.clRed, form.txtMagicWarr.FontColor);
        Assert.False(form.grpFly.Enabled);
        Assert.False(form.grpFlyEff.Enabled);
        Assert.False(form.cbbOperateMode.Enabled);
        Assert.True(form.cbbMagicSwitchMode.Enabled);
        Assert.True(form.cbbMagicWarrNGOption.Enabled);
        Assert.False(form.chkClientTargetMultiPlay.Enabled);
        Assert.False(form.chkEnableAntiMagic.Enabled);
        Assert.False(form.chkClientNotRaiseHand.Enabled);
        Assert.True(form.chkEnableHitPoint.Enabled);
        Assert.True(form.chkEnableHitPoint.Checked);
        // 原文 :1964-1970：非攻击模式时强制改回 momAttack 并再触发一次 OnChange
        Assert.Equal(TCustomOperateMode.momAttack, cfg.ServerConfig.OperateMode);
        Assert.Equal(TCustomAttackMode.mamNear, cfg.ServerConfig.AttackMode);
    }

    [Fact]
    public void VstCustomMagicNodeClick_NonWarrMagic_ResetsSwitchMode()
    {
        var cfg = MakeConfig("法师技", 1, warr: false);
        cfg.ServerConfig.EnableAntiMagic = true;
        cfg.ClientBaseConfig.MagicSwitchMode = TMagicSwitchMode.msmSwitch;
        cfg.ClientBaseConfig.NotRaiseHand = 1;

        CustomMagicFormGlobals.m_CustomMagicList.Add(cfg);
        using var form = new TFrmCustomMagic();
        form.DoOpen();
        form.MainTreeHost!.FocusedNode = form.MainTreeHost.Nodes[0];

        form.VstCustomMagicNodeClick(form.MainTreeHost, new THitInfo());

        Assert.Equal("非战士技能", form.txtMagicWarr.Caption);
        Assert.Equal(CustomMagicColors.clBlue, form.txtMagicWarr.FontColor);
        Assert.True(form.grpFly.Enabled);
        Assert.True(form.cbbOperateMode.Enabled);
        Assert.False(form.cbbMagicSwitchMode.Enabled);
        Assert.Equal(TMagicSwitchMode.msmNone, cfg.ClientBaseConfig.MagicSwitchMode);
        Assert.Equal((int)TCustomAttackMode.mamFar, (int)cfg.ServerConfig.AttackMode);
        Assert.True(form.chkEnableAntiMagic.Enabled);
        Assert.True(form.chkEnableAntiMagic.Checked);
        Assert.False(form.chkEnableHitPoint.Enabled);
        Assert.False(form.chkEnableHitPoint.Checked);
        Assert.True(form.chkClientNotRaiseHand.Enabled);
        Assert.True(form.chkClientNotRaiseHand.Checked);
    }

    [Fact]
    public void VstCustomMagicNodeClick_NodeDataNull_ExitsEarly()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var node = form.MainTreeHost!.AddChild(null);
        node.Data = null;
        form.MainTreeHost.FocusedNode = node;

        form.VstCustomMagicNodeClick(form.MainTreeHost, new THitInfo());

        Assert.Null(form.FCurrentCustomConfig);
    }

    // ---- 树事件处理器（原文 :4290-4723）----

    [Fact]
    public void VstAttackDecAttrChecked_WritesIsCheckedFromCheckState()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var host = form.TreeHosts["vstAttackDecAttr"];
        var node = host.AddChild(null);
        node.Data = new TAttackDecAttribData
        {
            AttribType = TMagicAttackDecAttributesType.daDC,
            Data = cfg.ServerConfig.AttackSubAttrib[(int)TMagicAttackDecAttributesType.daDC],
        };

        host.SetCheckState(node, TCheckState.csCheckedNormal);
        form.VstAttackDecAttrChecked(host, node);
        Assert.True(cfg.ServerConfig.AttackSubAttrib[2].IsChecked);
        Assert.True(form.FIsConfigChanged);

        host.SetCheckState(node, TCheckState.csUnCheckedNormal);
        form.VstAttackDecAttrChecked(host, node);
        Assert.False(cfg.ServerConfig.AttackSubAttrib[2].IsChecked);
    }

    [Fact]
    public void VstAttackDecAttrChecked_NullNodeData_Skips()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var host = form.TreeHosts["vstAttackDecAttr"];
        var node = host.AddChild(null);
        node.Data = null;

        form.VstAttackDecAttrChecked(host, node);
        Assert.False(form.FIsConfigChanged);
    }

    [Fact]
    public void VstAttackDecAttrNodeClick_TogglesHintOnColumn12()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var host = form.TreeHosts["vstAttackDecAttr"];
        var node = host.AddChild(null);
        node.Data = new TAttackDecAttribData
        {
            AttribType = TMagicAttackDecAttributesType.daAC,
            Data = cfg.ServerConfig.AttackSubAttrib[0],
        };
        CustomMagicPostMessageSeam.Clear();

        form.VstAttackDecAttrNodeClick(host, new THitInfo { HitNode = node, HitColumn = 12 });
        Assert.True(cfg.ServerConfig.AttackSubAttrib[0].ShowHint);
        Assert.Contains(node, host.InvalidatedNodes);
        Assert.Empty(CustomMagicPostMessageSeam.Posted);

        form.VstAttackDecAttrNodeClick(host, new THitInfo { HitNode = node, HitColumn = 12 });
        Assert.False(cfg.ServerConfig.AttackSubAttrib[0].ShowHint);
    }

    [Fact]
    public void VstAttackDecAttrNodeClick_OtherColumn_PostsStartEditing()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var host = form.TreeHosts["vstAttackDecAttr"];
        var node = host.AddChild(null);
        node.Index = 5;
        node.Data = new TAttackDecAttribData { Data = cfg.ServerConfig.AttackSubAttrib[0] };
        CustomMagicPostMessageSeam.Clear();

        form.VstAttackDecAttrNodeClick(host, new THitInfo { HitNode = node, HitColumn = 3 });

        var posted = Assert.Single(CustomMagicPostMessageSeam.Posted);
        Assert.Equal(CustomMagicWm.WM_STARTEDITING_DEC_ATTRIB, posted.Msg);
        Assert.Equal(5, posted.WParam);
        Assert.Equal(3, posted.LParam);
    }

    [Fact]
    public void VstAttackDecAttrNodeClick_NullHitNode_Exits()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        CustomMagicPostMessageSeam.Clear();
        form.VstAttackDecAttrNodeClick(form.TreeHosts["vstAttackDecAttr"], new THitInfo { HitNode = null, HitColumn = 12 });
        Assert.Empty(CustomMagicPostMessageSeam.Posted);
        Assert.False(form.FIsConfigChanged);
    }

    [Fact]
    public void DecElementNodeClick_SelectsMessageBySender()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var decHost = form.TreeHosts["vstDecElement"];
        var incHost = form.TreeHosts["vstAddElement"];

        var decNode = decHost.AddChild(null);
        decNode.Index = 1;
        decNode.Data = new TMagicElementData { Data = cfg.ServerConfig.AttackSubElements[0] };
        var incNode = incHost.AddChild(null);
        incNode.Index = 2;
        incNode.Data = new TMagicElementData { Data = cfg.ServerConfig.ProtectAddElements[0] };

        CustomMagicPostMessageSeam.Clear();
        form.VstDecElementNodeClick(decHost, new THitInfo { HitNode = decNode, HitColumn = 1 });
        form.VstDecElementNodeClick(incHost, new THitInfo { HitNode = incNode, HitColumn = 1 });

        Assert.Equal(2, CustomMagicPostMessageSeam.Posted.Count);
        Assert.Equal(CustomMagicWm.WM_STARTEDITING_DEC_ELEMENT, CustomMagicPostMessageSeam.Posted[0].Msg);
        Assert.Equal(1, CustomMagicPostMessageSeam.Posted[0].WParam);
        Assert.Equal(CustomMagicWm.WM_STARTEDITING_INC_ELEMENT, CustomMagicPostMessageSeam.Posted[1].Msg);
        Assert.Equal(2, CustomMagicPostMessageSeam.Posted[1].WParam);
    }

    [Fact]
    public void DecElementNodeClick_TogglesHintOnColumn9()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var host = form.TreeHosts["vstDecElement"];
        var node = host.AddChild(null);
        node.Data = new TMagicElementData { Data = cfg.ServerConfig.AttackSubElements[0] };

        form.VstDecElementNodeClick(host, new THitInfo { HitNode = node, HitColumn = 9 });
        Assert.True(cfg.ServerConfig.AttackSubElements[0].ShowHint);
        Assert.True(form.FIsConfigChanged);
    }

    [Fact]
    public void VstProtectedAddAttrChecked_AndNodeClick()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var host = form.TreeHosts["vstProtectedAddAttr"];
        var node = host.AddChild(null);
        node.Index = 3;
        node.Data = new TProtectAddAttribData
        {
            AttribType = TMagicProtectAddAttributesType.aaHP,
            Data = cfg.ServerConfig.ProtectAddAttrib[(int)TMagicProtectAddAttributesType.aaHP],
        };

        host.SetCheckState(node, TCheckState.csCheckedNormal);
        form.VstProtectedAddAttrChecked(host, node);
        Assert.True(cfg.ServerConfig.ProtectAddAttrib[(int)TMagicProtectAddAttributesType.aaHP].IsChecked);

        CustomMagicPostMessageSeam.Clear();
        form.VstProtectedAddAttrNodeClick(host, new THitInfo { HitNode = node, HitColumn = 2 });
        Assert.Equal(CustomMagicWm.WM_STARTEDITING_INC_ATTRIB, Assert.Single(CustomMagicPostMessageSeam.Posted).Msg);
    }

    [Fact]
    public void AfterCellPaint_DrawsOnlyOnHintColumn()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var host = form.TreeHosts["vstAttackDecAttr"];
        var node = host.AddChild(null);
        node.Data = new TAttackDecAttribData
        {
            Data = cfg.ServerConfig.AttackSubAttrib[0],
        };
        cfg.ServerConfig.AttackSubAttrib[0].ShowHint = true;

        var rect = new TRectSeam { Left = 100, Top = 50, Right = 120, Bottom = 70 };
        form.VstAttackDecAttrAfterCellPaint(host, node, 11, rect);
        Assert.Empty(form.ilCheck.DrawCalls);

        form.VstAttackDecAttrAfterCellPaint(host, node, 12, rect);
        var call = Assert.Single(form.ilCheck.DrawCalls);
        Assert.Equal(1, call.Index);
        Assert.Equal(100 + (120 - 100 - 13) / 2, call.X);
        Assert.Equal(50 + (70 - 50 - 13) / 2, call.Y);
        Assert.True(form.BrushStyleClearMirror);
    }

    [Fact]
    public void GetTextHandlers_DelegateToTreeLogic()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        cfg.ServerConfig.AttackSubAttrib[0].Rate = 5;

        var decHost = form.TreeHosts["vstAttackDecAttr"];
        var decNode = decHost.AddChild(null);
        decNode.Data = new TAttackDecAttribData
        {
            AttribType = TMagicAttackDecAttributesType.daAC,
            Data = cfg.ServerConfig.AttackSubAttrib[0],
        };
        Assert.Equal("5", form.VstAttackDecAttrGetText(decHost, decNode, 1));

        var protHost = form.TreeHosts["vstProtectedAddAttr"];
        var protNode = protHost.AddChild(null);
        protNode.Data = new TProtectAddAttribData
        {
            AttribType = TMagicProtectAddAttributesType.aaHP,
            Data = cfg.ServerConfig.ProtectAddAttrib[(int)TMagicProtectAddAttributesType.aaHP],
        };
        Assert.Equal("-", form.VstProtectedAddAttrGetText(protHost, protNode, 9));

        var elemHost = form.TreeHosts["vstDecElement"];
        var elemNode = elemHost.AddChild(null);
        elemNode.Data = new TMagicElementData
        {
            ElementType = TItemElementsType.ietDamageAdd,
            Data = cfg.ServerConfig.AttackSubElements[(int)TItemElementsType.ietDamageAdd],
        };
        Assert.Equal(
            GXX.M2Server.Engine.CustomMagicUtils.ItemElementsTypeNames[(int)TItemElementsType.ietDamageAdd],
            form.VstDecElementGetText(elemHost, elemNode, 0));
    }

    [Fact]
    public void EditingHandlers_MatchPureLogic()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        var host = form.TreeHosts["vstAttackDecAttr"];
        var node = host.AddChild(null);
        node.Data = new TAttackDecAttribData
        {
            AttribType = TMagicAttackDecAttributesType.daHitPoint,
            Data = cfg.ServerConfig.AttackSubAttrib[0],
        };

        form.VstAttackDecAttrEditing(host, node, 0, out bool a0);
        Assert.False(a0);
        form.VstAttackDecAttrEditing(host, node, 3, out bool a3);
        Assert.False(a3);                                   // AttribType >= daHitPoint
        form.VstAttackDecAttrEditing(host, node, 6, out bool a6);
        Assert.True(a6);
        form.VstAttackDecAttrEditing(host, null, 1, out bool aNull);
        Assert.False(aNull);

        form.VstProtectedAddAttrEditing(form.TreeHosts["vstProtectedAddAttr"], null, 1, out bool pNull);
        Assert.False(pNull);
        form.VstDecElementEditing(null, 1, out bool eNull);
        Assert.False(eNull);
        form.VstDecElementEditing(node, 9, out bool e9);
        Assert.False(e9);
        form.VstDecElementEditing(node, 1, out bool e1);
        Assert.True(e1);
    }

    [Fact]
    public void CreateEditorHandlers_ReturnExpectedLinkTypes()
    {
        using var form = new TFrmCustomMagic();

        form.VstAttackDecAttrCreateEditor(out var a);
        Assert.IsType<TDecAttribPropertyEditLink>(a);
        form.VstProtectedAddAttrCreateEditor(out var b);
        Assert.IsType<TDecAttribPropertyEditLink>(b);
        form.VstDecElementCreateEditor(out var c);
        Assert.IsType<TElementPropertyEditLink>(c);
    }

    [Fact]
    public void WmStartEditing_DispatchToCorrectTree()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);

        var decHost = form.TreeHosts["vstAttackDecAttr"];
        var decNode = decHost.AddChild(null);
        decNode.Data = new TAttackDecAttribData { Data = cfg.ServerConfig.AttackSubAttrib[0] };
        form.WMStartEditingDecAttrib(0, 7);
        Assert.Equal((decNode, 7), Assert.Single(decHost.EditNodeCalls));

        var elemHost = form.TreeHosts["vstDecElement"];
        var elemNode = elemHost.AddChild(null);
        elemNode.Data = new TMagicElementData { Data = cfg.ServerConfig.AttackSubElements[0] };
        form.WMStartEditingDecElement(0, 3);
        Assert.Equal((elemNode, 3), Assert.Single(elemHost.EditNodeCalls));

        var incElemHost = form.TreeHosts["vstAddElement"];
        var incElemNode = incElemHost.AddChild(null);
        incElemNode.Data = new TMagicElementData { Data = cfg.ServerConfig.ProtectAddElements[0] };
        form.WMStartEditingIncElement(0, 2);
        Assert.Equal((incElemNode, 2), Assert.Single(incElemHost.EditNodeCalls));

        var protHost = form.TreeHosts["vstProtectedAddAttr"];
        var protNode = protHost.AddChild(null);
        protNode.Data = new TProtectAddAttribData { Data = cfg.ServerConfig.ProtectAddAttrib[0] };
        form.WMStartEditingIncAttrib(0, 5);
        Assert.Equal((protNode, 5), Assert.Single(protHost.EditNodeCalls));

        // 越界下标 → EditNode(null, ...)（原文的 WPARAM 是节点地址，托管侧退化为序号）
        form.WMStartEditingDecAttrib(99, 1);
        Assert.Equal(((TVirtualNodeSeam?)null, 1), decHost.EditNodeCalls[^1]);
    }

    // ---- 空体处理器（原文 :3082-3090 / :3145-3153）----

    [Fact]
    public void EmptyPlayTimeHandlers_DoNothing()
    {
        var cfg = MakeConfig();
        using var form = MakeFormWith(cfg);
        form.SeTargetStatus1_PlayTimeChange();
        form.SeTargetStatus2_PlayTimeChange();
        Assert.False(form.FIsConfigChanged);
    }

    // ---- 消息号常量（原文 :9-13）----

    [Fact]
    public void WmConstants_AreOneHundredAboveWmUser()
    {
        Assert.Equal(0x0400, CustomMagicWm.WM_USER);
        Assert.Equal(0x0400 + 300, CustomMagicWm.WM_STARTEDITING_DEC_ATTRIB);
        Assert.Equal(0x0400 + 303, CustomMagicWm.WM_STARTEDITING_INC_ELEMENT);
    }

    // ---- 短字符串截断（原文 string[40]/string[20]/string[GN]）----

    [Fact]
    public void ServerConfig_ShortStringFieldsTruncate()
    {
        var cfg = MakeConfig();
        cfg.ServerConfig.FailMsg = new string('x', 50);
        Assert.Equal(40, cfg.ServerConfig.FailMsg.Length);
        cfg.ServerConfig.CheckVarName = new string('y', 25);
        Assert.Equal(20, cfg.ServerConfig.CheckVarName.Length);
        cfg.ServerConfig.NeedItemCustomItemName = new string('z', 100);
        Assert.Equal(Grobal2Const.ITEM_NAME_LEN, cfg.ServerConfig.NeedItemCustomItemName.Length);
        cfg.ServerConfig.SetCallMonster(0, new string('w', 100));
        Assert.Equal(Grobal2Const.ITEM_NAME_LEN, cfg.ServerConfig.GetCallMonster(0).Length);
        cfg.ServerConfig.SetCallMonster(1, null!);
        Assert.Equal("", cfg.ServerConfig.GetCallMonster(1));

        Assert.Equal("abc", CustomMagicShortStr.Trunc("abc", 40));
        Assert.Equal("", CustomMagicShortStr.Trunc(null, 5));
    }

    [Fact]
    public void Config_ModelBasics()
    {
        var cfg = new TCustomMagicConfig("名称", 1234, true);
        Assert.Equal("名称", cfg.MagicName);
        Assert.Equal(1234, cfg.MagicID);
        Assert.True(cfg.IsMagicWarr);
        Assert.False(cfg.IsChanged);

        cfg.SetChanged();
        Assert.True(cfg.IsChanged);
        cfg.SetChanged(false);
        Assert.False(cfg.IsChanged);

        cfg.IsMagicWarr = false;
        Assert.False(cfg.IsMagicWarr);

        foreach (var holder in cfg.ClientConfigs)
            Assert.NotNull(holder);
        Assert.Equal(new[] { 0, 0, 0, 0 }, System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(cfg.ClientConfigs, h => (int)h.Value.Icon_Index)));
        Assert.Equal(11, cfg.ServerConfig.Additionals.Length);
        Assert.Equal(9, cfg.ServerConfig.AttackSubAttrib.Length);
        Assert.Equal(25, cfg.ServerConfig.AttackSubElements.Length);
        Assert.Equal(6, cfg.ServerConfig.AttackBreakDefense.Length);
        Assert.Equal(16, cfg.ServerConfig.ProtectAddAttrib.Length);
        Assert.Equal(25, cfg.ServerConfig.ProtectAddElements.Length);
        Assert.Equal(11, cfg.ServerConfig.AttackPowerRates.Length);
    }

    [Fact]
    public void SimulatedNativeCallbacks_WriteThroughToSameObjects()
    {
        // 模拟"树节点持有的 Data 指针 == 配置内对象"的共享语义
        var cfg = MakeConfig();
        var data = cfg.ServerConfig.AttackSubAttrib[(int)TMagicAttackDecAttributesType.daSC];
        var link = new TAttackDecAttribData { AttribType = TMagicAttackDecAttributesType.daSC, Data = data };

        Assert.True(DecAttribPropertyEditLinkLogic.ApplyEditorResult(1, link, new TVtEditorResult { Value = 55 }));
        Assert.Equal(55, cfg.ServerConfig.AttackSubAttrib[(int)TMagicAttackDecAttributesType.daSC].Rate);
    }
}

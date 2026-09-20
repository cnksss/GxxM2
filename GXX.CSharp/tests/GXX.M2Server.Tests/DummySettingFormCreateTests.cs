using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms.DummySetting;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// `FormCreate`（uFrmDummySetting.pas:229-325）1:1 覆盖。
/// 与 `FormCreate` **直接相关**的原文缺陷：
///   · :320-321 `lstDisableMoveMap.Items.Assign(...)` 在 Delphi 里**不可编译**
///     （`TListBox.Items` 无 `Assign`）⇒ 托管按语义等价实现；差异断言见
///     `FormCreate_ListAssign_UsesClearThenAppend`。
///   · :307-318 `{$IF MULTI_THREAD = 1}` 包裹：`g_MultiThreadRun = false` 时不锁。
/// </summary>
[Collection("DummySettingSerial")]
public sealed class DummySettingFormCreateTests : DummySettingTestBase
{
    [Fact]
    public void FormCreate_PopulatesDummyListFromGlobal()
    {
        DummySettingState.g_DummyNameList.Add("假人甲");
        DummySettingState.g_DummyNameList.Add("假人乙");

        OpenWith();

        Assert.Equal(2, Form.Ct.lstDummyList.Items.Count);
        Assert.Equal("假人甲", Form.Ct.lstDummyList.Items[0]);
        Assert.Equal("假人乙", Form.Ct.lstDummyList.Items[1]);
    }

    [Fact]
    public void FormCreate_EmptyGlobal_LeavesEmptyList()
    {
        OpenWith();
        Assert.Empty(Form.Ct.lstDummyList.Items);
    }

    [Fact]
    public void FormCreate_Repeated_ClearsInsteadOfAppending()
    {
        DummySettingState.g_DummyNameList.Add("A");
        OpenWith();
        DoFormCreate();
        DoFormCreate();
        Assert.Single(Form.Ct.lstDummyList.Items);
    }

    [Fact]
    public void FormCreate_BackfillsHomeMapFromConfig()
    {
        M2Config.sDummyHomeMap = "0";
        M2Config.nDummyHomeX = 123;
        M2Config.nDummyHomeY = 456;
        M2Config.nDummyLogonTime = 7;
        M2Config.boDummyLogonRand = true;

        OpenWith();

        Assert.Equal("0", Form.Ct.edtDummyHomeMap.Text);
        Assert.Equal(123, (int)Form.Ct.seDummyHomeX.Value);
        Assert.Equal(456, (int)Form.Ct.seDummyHomeY.Value);
        Assert.Equal(7, (int)Form.Ct.seDummyLogonTime.Value);
        Assert.True(Form.Ct.chkDummyLogonRand.Checked);
    }

    [Fact]
    public void FormCreate_HomeMap_TextIsNotTrimmed_OriginalBehaviour()
    {
        // 原文 :238 `edtDummyHomeMap.Text := g_Config.sDummyHomeMap` —— **不 Trim**
        // （对比 :441 ButtonDummySaveClick 里对 Text 做 Trim 后才写回 g_Config）。
        M2Config.sDummyHomeMap = "  3  ";
        OpenWith();
        Assert.Equal("  3  ", Form.Ct.edtDummyHomeMap.Text);
    }

    [Fact]
    public void FormCreate_AutoRepairAndRecallHero_Backfilled()
    {
        M2Config.boDummyAutoRepairItem = false;
        M2Config.boDummyAutoRecallHero = false;
        OpenWith();
        Assert.False(Form.Ct.CheckBoxDummyAutoRepairItem.Checked);
        Assert.False(Form.Ct.CheckBoxDummyAutoRecallHero.Checked);
    }

    [Fact]
    public void FormCreate_AutoAddHpMp_AndPercents_Backfilled()
    {
        M2Config.boDummyAutoAddHP = false;
        M2Config.nDummyAddHPPercent = 33;
        M2Config.boDummyAutoAddMP = false;
        M2Config.nDummyAddMPPercent = 44;
        OpenWith();

        Assert.False(Form.Ct.chkDummyAutoAddHP.Checked);
        Assert.Equal(33, (int)Form.Ct.seDummyAddHPPercent.Value);
        Assert.False(Form.Ct.chkDummyAutoAddMP.Checked);
        Assert.Equal(44, (int)Form.Ct.seDummyAddMPPercent.Value);
    }

    [Fact]
    public void FormCreate_AllSixteenHpMpSpins_BackfilledExactly()
    {
        M2Config.nDummyHPTime_Warrior = 301;
        M2Config.nDummyHPBase_Warrior = 302;
        M2Config.nDummyMPTime_Warrior = 303;
        M2Config.nDummyMPBase_Warrior = 304;
        M2Config.nDummyHPTime_DF = 305;
        M2Config.nDummyHPBase_DF = 306;
        M2Config.nDummyMPTime_DF = 307;
        M2Config.nDummyMPBase_DF = 308;
        M2Config.nDummyHeroHPTime_Warrior = 309;
        M2Config.nDummyHeroHPBase_Warrior = 310;
        M2Config.nDummyHeroMPTime_Warrior = 311;
        M2Config.nDummyHeroMPBase_Warrior = 312;
        M2Config.nDummyHeroHPTime_DF = 313;
        M2Config.nDummyHeroHPBase_DF = 314;
        M2Config.nDummyHeroMPTime_DF = 315;
        M2Config.nDummyHeroMPBase_DF = 316;

        OpenWith();

        Assert.Equal(301, (int)Form.Ct.seDummyHPTime_Warrior.Value);
        Assert.Equal(302, (int)Form.Ct.seDummyHPBase_Warrior.Value);
        Assert.Equal(303, (int)Form.Ct.seDummyMPTime_Warrior.Value);
        Assert.Equal(304, (int)Form.Ct.seDummyMPBase_Warrior.Value);
        Assert.Equal(305, (int)Form.Ct.seDummyHPTime_DF.Value);
        Assert.Equal(306, (int)Form.Ct.seDummyHPBase_DF.Value);
        Assert.Equal(307, (int)Form.Ct.seDummyMPTime_DF.Value);
        Assert.Equal(308, (int)Form.Ct.seDummyMPBase_DF.Value);
        Assert.Equal(309, (int)Form.Ct.seDummyHeroHPTime_Warrior.Value);
        Assert.Equal(310, (int)Form.Ct.seDummyHeroHPBase_Warrior.Value);
        Assert.Equal(311, (int)Form.Ct.seDummyHeroMPTime_Warrior.Value);
        Assert.Equal(312, (int)Form.Ct.seDummyHeroMPBase_Warrior.Value);
        Assert.Equal(313, (int)Form.Ct.seDummyHeroHPTime_DF.Value);
        Assert.Equal(314, (int)Form.Ct.seDummyHeroHPBase_DF.Value);
        Assert.Equal(315, (int)Form.Ct.seDummyHeroMPTime_DF.Value);
        Assert.Equal(316, (int)Form.Ct.seDummyHeroMPBase_DF.Value);
    }

    [Fact]
    public void FormCreate_AllSixAttackWalkTimes_BackfilledExactly()
    {
        M2Config.dwDummyWarrorAttackTime = 1100;
        M2Config.dwDummyWizardAttackTime = 1200;
        M2Config.dwDummyTaoistAttackTime = 1300;
        M2Config.dwDummyWarrorWalkTime = 400;
        M2Config.dwDummyWizardWalkTime = 500;
        M2Config.dwDummyTaoistWalkTime = 600;

        OpenWith();

        Assert.Equal(1100, (int)Form.Ct.EditDummyWarrorAttackTime.Value);
        Assert.Equal(1200, (int)Form.Ct.EditDummyWizardAttackTime.Value);
        Assert.Equal(1300, (int)Form.Ct.EditDummyTaoistAttackTime.Value);
        Assert.Equal(400, (int)Form.Ct.EditDummyWarrorWalkTime.Value);
        Assert.Equal(500, (int)Form.Ct.EditDummyWizardWalkTime.Value);
        Assert.Equal(600, (int)Form.Ct.EditDummyTaoistWalkTime.Value);
    }

    [Fact]
    public void FormCreate_AttackTimeBelowDfmMin_IsClampedTo10()
    {
        // DFM :245 等 `MinValue = 10`（Delphi TSpinEditEx 静默钳制）
        M2Config.dwDummyWarrorAttackTime = 1;
        OpenWith();
        Assert.Equal(10, (int)Form.Ct.EditDummyWarrorAttackTime.Value);
    }

    [Fact]
    public void FormCreate_AttackTimeAboveDfmMax_IsClampedTo10000()
    {
        M2Config.dwDummyWizardAttackTime = 999999;
        OpenWith();
        Assert.Equal(10000, (int)Form.Ct.EditDummyWizardAttackTime.Value);
    }

    [Fact]
    public void FormCreate_HomeXBelowDfmMin_IsClampedTo1()
    {
        M2Config.nDummyHomeX = -50;
        OpenWith();
        Assert.Equal(1, (int)Form.Ct.seDummyHomeX.Value);
    }

    [Fact]
    public void FormCreate_AddHpPercentAboveDfmMax_IsClampedTo100()
    {
        M2Config.nDummyAddHPPercent = 1000;
        OpenWith();
        Assert.Equal(100, (int)Form.Ct.seDummyAddHPPercent.Value);
    }

    [Fact]
    public void FormCreate_HpTimeSpin_AcceptsValueRoundTrip_DfmMinMaxZero()
    {
        // DFM :442 `MinValue = 0 MaxValue = 0` ⇒ 原文 TSpinEditEx **不钳制**。
        // 托管侧用 `ClampSpinValue(v, 0, 0)`（min=0/hi=int.MaxValue）。
        // ⚠ 本用例**只断言 0 附近的往返**：负数会被 min=0 钳掉（见
        // `ClampSpinValue_ZeroMinWithMaxZero_ClampsLowerBound` 登记的 D-p8-03 偏离）。
        M2Config.nDummyHPTime_Warrior = 12345;
        OpenWith();
        Assert.Equal(12345, (int)Form.Ct.seDummyHPTime_Warrior.Value);

        M2Config.nDummyHPTime_DF = 0;
        DoFormCreate();
        Assert.Equal(0, (int)Form.Ct.seDummyHPTime_DF.Value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FormCreate_DisableDummyRunCheckBox_IsInvertedConfig(bool boDiableDummyRun)
    {
        // 原文 :285 `chkDisDummyRun.Checked := not g_Config.boDiableDummyRun`
        M2Config.boDiableDummyRun = boDiableDummyRun;
        OpenWith();
        Assert.Equal(!boDiableDummyRun, Form.Ct.chkDisDummyRun.Checked);
    }

    [Fact]
    public void FormCreate_RunFlags_AndWarHreoRunEnabledFollowsWarDisHumRun()
    {
        // ★ 关键：必须在 `OpenWith()` **之前**把全部标志设好。
        // 因为 `FormCreate` 在 :285 会写 `chkDisDummyRun.Checked := not boDiableDummyRun`，
        // 该赋值**连带触发** `chkDisDummyRunClick`（:616-663）：若变为**勾选**态则进入
        // :621 `boChecked = True` 分支 ⇒ :660 把 `boDiableDummyRun` 覆写为 True、
        // 且 :623-645 **清空并禁用** 8 个从属勾选（但**不写回** `boDummyRunHum` 等字段）。
        // 置 `boDiableDummyRun = true` ⇒ 该框保持**未勾选** ⇒ 不触发清空分支。
        M2Config.boDiableDummyRun = true;
        M2Config.boDummyRunHum = true;
        M2Config.boDummyRunMon = true;
        M2Config.boDummyRunNpc = true;
        M2Config.boDummyRunGuard = true;
        M2Config.boDummySafeAreaLimited = true;
        M2Config.boDummySafeAreaDisNpcRun = true;
        M2Config.boSafeAreaDisShopStallDummyRun = true;
        M2Config.boSafeAreaDisOffLineDummyRun = true;
        M2Config.boDummyWarDisHumRun = true;
        M2Config.boDummyWarHreoRun = true;

        OpenWith();

        Assert.True(Form.Ct.chkDummyRunHum.Checked);
        Assert.True(Form.Ct.chkDummyRunMon.Checked);
        Assert.True(Form.Ct.chkDummyRunNpc.Checked);
        Assert.True(Form.Ct.chkDummyRunGuard.Checked);
        Assert.True(Form.Ct.chkDummySafeArea.Checked);
        Assert.True(Form.Ct.chkDummyWarDisHumRun.Checked);
        Assert.True(Form.Ct.chkDummyWarHreoRun.Checked);
        Assert.True(Form.Ct.chkDummyWarHreoRun.Enabled);          // :293 Enabled := boDummyWarDisHumRun
        Assert.True(Form.Ct.chkDummySafeAreaDisNpcRun.Checked);
        Assert.True(Form.Ct.chkSafeAreaDisShopStallDummyRun.Checked);
        Assert.True(Form.Ct.chkSafeAreaDisOffLineDummyRun.Checked);
        Assert.False(Form.Ct.chkDisDummyRun.Checked);             // 与 boDiableDummyRun = True 互为取反
    }

    [Fact]
    public void FormCreate_DisableDummyRunCheckboxChecked_CascadesAndClobbersSlaveFlags_OriginalBehaviour()
    {
        // ★★ 原文缺陷差异断言：`boDiableDummyRun = False`（默认值！）时
        // :285 把 `chkDisDummyRun.Checked` 置 **True** ⇒ 连带 `chkDisDummyRunClick`
        // ⇒ `boChecked := not True = False` ⇒ :660 **把 `g_Config.boDiableDummyRun` 写回 True**
        // ⇒ **窗体创建后该配置项恒为 True**，后台配置里设的 False 被静默覆盖。
        // 同时 :649-657 走 else 分支，8 个从属只被置 `Enabled := True`（不清勾选）。
        M2Config.boDiableDummyRun = false;
        M2Config.boDummyRunHum = true;

        OpenWith();

        Assert.True(Form.Ct.chkDisDummyRun.Checked);   // :285 取反
        Assert.True(M2Config.boDiableDummyRun);        // ★ 被连带处理器覆写回 True
        Assert.True(Form.Ct.chkDummyRunHum.Checked);   // else 分支不清勾选
        Assert.True(Form.Ct.chkDummyRunHum.Enabled);
    }

    [Fact]
    public void FormCreate_WarDisHumRunFalse_LeavesWarHreoRunDisabled_ButKeepsItsCheckState()
    {
        // 差异断言：原文 :292 先无条件 `chkDummyWarHreoRun.Checked := boDummyWarHreoRun`，
        // :293 再 `Enabled := boDummyWarDisHumRun` ⇒ **勾选态可能为 True 而 Enabled 为 False**。
        M2Config.boDummyWarDisHumRun = false;
        M2Config.boDummyWarHreoRun = true;

        OpenWith();

        Assert.True(Form.Ct.chkDummyWarHreoRun.Checked);
        Assert.False(Form.Ct.chkDummyWarHreoRun.Enabled);
    }

    [Fact]
    public void Diagnostic_DisabledCheckBoxBlocksCheckedAssign()
    {
        var box = Form.Ct.chkDummyWarHreoRun;
        box.Enabled = false;
        box.Checked = true;
        bool blockedByDisabled = !box.Checked;
        box.Enabled = true;
        box.Checked = true;
        Assert.True(false, $"blockedByDisabled={blockedByDisabled} afterEnable={box.Checked}");
    }

    [Fact]
    public void Diagnostic_CheckedChangedFires()
    {
        int fired = 0;
        Form.Ct.chkDummyWarHreoRun.CheckedChanged += (_, _) => fired++;
        Form.Ct.chkDummyWarHreoRun.Checked = true;
        Assert.True(fired > 0, $"fired={fired}");
    }

    [Fact]
    public void Diagnostic_ChkDisDummyRunCascadeFires()
    {
        // 直接调处理器（不依赖事件）—— 验证处理器本体是否正确
        Form.Ct.chkDisDummyRun.Checked = false;
        Form.chkDisDummyRunClick(Form.Ct.chkDisDummyRun);
        Assert.True(false,
            $"cfgDisable={M2Config.boDiableDummyRun} humChecked={Form.Ct.chkDummyRunHum.Checked} humEnabled={Form.Ct.chkDummyRunHum.Enabled}");
    }

    [Fact]
    public void Diagnostic_FormCreateCheckboxCascade()
    {
        int fired = 0;
        Form.Ct.chkDummyRunHum.CheckedChanged += (_, _) => fired++;
        Form.Ct.chkDummyRunHum.Checked = true;
        int direct = fired;
        Form.uModValue();
        DoFormCreate();
        Assert.True(false, $"directFired={direct} humChecked={Form.Ct.chkDummyRunHum.Checked} humEnabled={Form.Ct.chkDummyRunHum.Enabled}");
    }

    [Fact]
    public void Diagnostic_RunFlagsState()
    {
        M2Config.boDiableDummyRun = true;
        M2Config.boDummyWarDisHumRun = true;
        M2Config.boDummyWarHreoRun = true;
        OpenWith();
        Assert.True(false,
            $"hreoChecked={Form.Ct.chkDummyWarHreoRun.Checked} hreoEnabled={Form.Ct.chkDummyWarHreoRun.Enabled} "
            + $"cfgWarDis={M2Config.boDummyWarDisHumRun} cfgHreo={M2Config.boDummyWarHreoRun}");
    }

    [Fact]
    public void FormCreate_SetsActivePageToZero_AndDisablesLogonButton()
    {
        OpenWith();
        Assert.Equal(0, Form.Ct.pgcMain.SelectedIndex);          // :298 ActivePageIndex := 0
        Assert.False(Form.Ct.btnDummyLogon.Enabled);             // :299
    }

    [Fact]
    public void FormCreate_FillsMapListFromMapManager()
    {
        OpenWith(maps: new[] { "0", "3", "D401" });
        Assert.Equal(3, Form.Ct.lstMapList.Items.Count);
        Assert.Equal("0", Form.Ct.lstMapList.Items[0]);
        Assert.Equal("3", Form.Ct.lstMapList.Items[1]);
        Assert.Equal("D401", Form.Ct.lstMapList.Items[2]);
    }

    [Fact]
    public void FormCreate_NullMapTable_LeavesMapListEmpty()
    {
        Form.MapListHandler = () => null;
        DoFormCreate();
        Assert.Empty(Form.Ct.lstMapList.Items);
    }

    [Fact]
    public void FormCreate_FillsMonsterListFromMonsterTable_NamesOnly()
    {
        OpenWith(monsters: new[] { "鸡", "鹿", "僵尸" });
        Assert.Equal(3, Form.Ct.lstMonList.Items.Count);
        Assert.Equal("鸡", Form.Ct.lstMonList.Items[0]);
        Assert.Equal("僵尸", Form.Ct.lstMonList.Items[2]);
    }

    [Fact]
    public void FormCreate_MultiThreadGate_InvokesLockAndUnlockInOrder()
    {
        var order = new List<string>();
        Form.g_MultiThreadRun = true;
        Form.MonsterListLockR = n => order.Add("lock:" + n);
        Form.MonsterListUnLockR = () => order.Add("unlock");
        Form.MapListHandler = () => null;
        Form.MonsterListHandler = () => null;

        DoFormCreate();

        Assert.Equal(new[] { "lock:6", "unlock" }, order);        // :308 LockR(6) / :317
    }

    [Fact]
    public void FormCreate_MultiThreadOff_NoLockCalls()
    {
        var order = new List<string>();
        Form.g_MultiThreadRun = false;
        Form.MonsterListLockR = n => order.Add("lock:" + n);
        Form.MonsterListUnLockR = () => order.Add("unlock");
        Form.MapListHandler = () => null;
        Form.MonsterListHandler = () => null;

        DoFormCreate();

        Assert.Empty(order);
    }

    [Fact]
    public void FormCreate_UnlockStillRunsWhenMonsterEnumerationThrows()
    {
        // 原文 :309-318 `try/finally`：异常也 UnLock
        bool unlocked = false;
        Form.g_MultiThreadRun = true;
        Form.MonsterListLockR = _ => { };
        Form.MonsterListUnLockR = () => unlocked = true;
        Form.MapListHandler = () => null;
        Form.MonsterListHandler = () => throw new InvalidOperationException("boom");

        Assert.Throws<InvalidOperationException>(() => DoFormCreate());
        Assert.True(unlocked);
    }

    [Fact]
    public void FormCreate_ListAssign_UsesClearThenAppend_AndPreservesOrder()
    {
        DummySettingState.g_DummyDisableMoveMapList.Add("Z9");
        DummySettingState.g_DummyDisableMoveMapList.Add("A1");
        DummySettingState.g_DummyNoActiveAttackMonList.Add("怪乙");
        DummySettingState.g_DummyNoActiveAttackMonList.Add("怪甲");

        OpenWith();

        Assert.Equal(new[] { "Z9", "A1" }, new[]
        {
            Form.Ct.lstDisableMoveMap.Items[0]!.ToString(),
            Form.Ct.lstDisableMoveMap.Items[1]!.ToString(),
        });
        Assert.Equal(new[] { "怪乙", "怪甲" }, new[]
        {
            Form.Ct.lstNoAttackMonList.Items[0]!.ToString(),
            Form.Ct.lstNoAttackMonList.Items[1]!.ToString(),
        });
    }

    [Fact]
    public void FormCreate_ListAssign_Repeated_DoesNotDuplicate()
    {
        DummySettingState.g_DummyDisableMoveMapList.Add("Z9");
        OpenWith();
        DoFormCreate();
        DoFormCreate();
        Assert.Single(Form.Ct.lstDisableMoveMap.Items);
    }

    [Fact]
    public void FormCreate_ResetsBothChangedFlags()
    {
        // 原文 :323-324 `FIsDummyDisableMoveMapChanged := False; FIsDummyNoActiveAttackMonChanged := False;`
        // 置脏：btnDisableMoveMapDeleteAllClick（:972）与 btnNoAttackMonDelAllClick（:1057）
        Form.btnDisableMoveMapDeleteAllClick(Form.Ct.btnDisableMoveMapDeleteAll);
        Form.btnNoAttackMonDelAllClick(Form.Ct.btnNoAttackMonDelAll);
        Form.Ct.lstDisableMoveMap.Items.Add("Z9");
        Form.Ct.lstNoAttackMonList.Items.Add("怪甲");
        DoFormCreate();

        // 复位后保存**不应**把控件列表覆写进全局
        DummySettingState.g_DummyDisableMoveMapList.Clear();
        DummySettingState.g_DummyNoActiveAttackMonList.Clear();
        Form.Ct.edtDummyHomeMap.Text = "3";
        MapTable = new List<TEnvirnoment> { Env("3") };
        Form.FindMapHandler = n => MapTable.Find(e => e.sMapName == n);
        ClearCaptures();
        Form.ButtonDummySaveClick(Form.Ct.ButtonDummySave);

        Assert.Empty(DummySettingState.Snapshot(DummySettingState.g_DummyDisableMoveMapList));
        Assert.Empty(DummySettingState.Snapshot(DummySettingState.g_DummyNoActiveAttackMonList));
    }

    [Fact]
    public void FormCreate_SaveButtonWasEnabled_BecomesDisabledWhenAllWritesAreSameValue()
    {
        // 原文 :239-283 的 Spin 回填会**连带触发 OnChange ⇒ ModValue()**（置脏）。
        // 但只有当值确实**变化**时才触发；本用例先把控件值预置为与配置**不同**，
        // 使 FormCreate 的赋值必然触发一次 ValueChanged。
        OpenWith();
        Form.uModValue();
        Assert.False(Form.Ct.ButtonDummySave.Enabled);

        // ⚠ 顺序敏感：先让控件变成 1（会连带把 g_Config 写回 1），**再**把 g_Config 设为 777。
        Form.Ct.seDummyHomeX.Value = 1;
        M2Config.nDummyHomeX = 777;      // FormCreate 会读到 777 ⇒ 控件 1→777 触发 ValueChanged
        Form.FormCreate();

        Assert.Equal(777, (int)Form.Ct.seDummyHomeX.Value);
        Assert.True(Form.Ct.ButtonDummySave.Enabled);   // 连带 OnChange 已把按钮置脏
    }

    [Fact]
    public void FormCreate_SameValues_NoValueChanged_ButtonStaysClean()
    {
        // 对照：若控件值已等于配置值，`Value := 同值` **不**触发 ValueChanged ⇒ 不置脏。
        OpenWith();
        Form.uModValue();
        Form.FormCreate();
        Assert.False(Form.Ct.ButtonDummySave.Enabled);
    }
}

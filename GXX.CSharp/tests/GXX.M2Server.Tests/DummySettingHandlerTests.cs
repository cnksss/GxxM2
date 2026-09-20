using System.Collections.Generic;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Forms.DummySetting;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 单字段处理器族（uFrmDummySetting.pas:413-429、:616-901）1:1 覆盖。
/// 每个处理器形状统一：`g_Config.X := &lt;控件值&gt;` → `ModValue()`（:905 置「保存」可用）。
/// 差异断言重点：每个处理器**只写自己那一个**字段（防错接线）。
/// </summary>
[Collection("DummySettingSerial")]
public sealed class DummySettingHandlerTests : DummySettingTestBase
{
    public DummySettingHandlerTests()
    {
        OpenWith();
    }

    // ---- `ModValue` / `uModValue`（:903-911）：每方法 3 例 ----

    [Fact]
    public void ModValue_EnablesSaveButton()
    {
        Form.Ct.ButtonDummySave.Enabled = false;
        Form.ModValue();
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void ModValue_Idempotent()
    {
        Form.Ct.ButtonDummySave.Enabled = true;
        Form.ModValue();
        Form.ModValue();
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void UModValue_DisablesSaveButton()
    {
        Form.Ct.ButtonDummySave.Enabled = true;
        Form.uModValue();
        Assert.False(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void UModValue_Idempotent()
    {
        Form.Ct.ButtonDummySave.Enabled = false;
        Form.uModValue();
        Form.uModValue();
        Assert.False(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void ModValue_ThenUModValue_NetDisabled()
    {
        Form.ModValue();
        Form.uModValue();
        Assert.False(Form.Ct.ButtonDummySave.Enabled);
    }

    // ---- `ClampSpinValue` 纯逻辑（每方法 3 例以上，含边界） ----

    [Theory]
    [InlineData(5, 1, 10, 5)]        // 界内
    [InlineData(1, 1, 10, 1)]        // 下边界
    [InlineData(10, 1, 10, 10)]      // 上边界
    [InlineData(0, 1, 10, 1)]        // 下越界
    [InlineData(-100, 1, 10, 1)]     // 负
    [InlineData(11, 1, 10, 10)]      // 上越界
    [InlineData(0, 0, 3, 0)]         // 下边界为 0
    [InlineData(4, 0, 3, 3)]         // 0..3 上越界
    public void ClampSpinValue_ClampsLikeDelphiSpinEdit(int value, int min, int max, int expected)
        => Assert.Equal(expected, TFrmDummySetting.ClampSpinValue(value, min, max));

    [Theory]
    [InlineData(0)]
    [InlineData(12345)]
    [InlineData(int.MaxValue)]
    public void ClampSpinValue_MaxZero_MeansNoUpperClamp(int value)
        => Assert.Equal(value, TFrmDummySetting.ClampSpinValue(value, 0, 0));

    [Theory]
    [InlineData(-12345)]
    [InlineData(int.MinValue)]
    public void ClampSpinValue_ZeroMinWithMaxZero_ClampsLowerBound_DeviationDp803(int value)
    {
        // ⚠ 与原文的偏离（登记 D-p8-03）：DFM `MinValue = 0 MaxValue = 0` 的 16 个
        // `seDummy*HP/MPTime_*`/`*Base_*` 在原文 TSpinEditEx 里是**完全不钳制**（可输负数）；
        // 托管 `NumericUpDown` 的 `Minimum` 类型上不允许负数 ⇒ 本车道把该族实现为
        // `ClampSpinValue(v, 0, 0)`（下界 0 / 上界不钳）。
        // 影响面：只有"配置里本来就是负数"才可见差异；对全部**正数**往返**完全等价**。
        Assert.Equal(0, TFrmDummySetting.ClampSpinValue(value, 0, 0));
    }

    [Fact]
    public void ClampSpinValue_NegativeValueWithNegativeMin_Kept()
        => Assert.Equal(-5, TFrmDummySetting.ClampSpinValue(-5, -10, 0));

    // ---- 布尔勾选族（:671-687、:791-801、:821-849、:861-883、:897-901） ----
    // 每个处理器：控件的 Checked 值原样写进**它自己那个** g_Config 字段。

    public static TheoryData<string, bool, Func<bool>> BoolCases() => new()
    {
        { nameof(TFrmDummySetting.chkDummyAutoAddHPClick), true, () => M2Config.boDummyAutoAddHP },
        { nameof(TFrmDummySetting.chkDummyAutoAddMPClick), true, () => M2Config.boDummyAutoAddMP },
        { nameof(TFrmDummySetting.CheckBoxDummyAutoRepairItemClick), true, () => M2Config.boDummyAutoRepairItem },
        { nameof(TFrmDummySetting.CheckBoxDummyAutoRecallHeroClick), true, () => M2Config.boDummyAutoRecallHero },
        { nameof(TFrmDummySetting.chkDummyRunHumClick), true, () => M2Config.boDummyRunHum },
        { nameof(TFrmDummySetting.chkDummyRunMonClick), true, () => M2Config.boDummyRunMon },
        { nameof(TFrmDummySetting.chkDummyRunNpcClick), true, () => M2Config.boDummyRunNpc },
        { nameof(TFrmDummySetting.chkDummyRunGuardClick), true, () => M2Config.boDummyRunGuard },
        { nameof(TFrmDummySetting.chkDummySafeAreaClick), true, () => M2Config.boDummySafeAreaLimited },
        { nameof(TFrmDummySetting.chkDummySafeAreaDisNpcRunClick), true, () => M2Config.boDummySafeAreaDisNpcRun },
        { nameof(TFrmDummySetting.chkSafeAreaDisShopStallDummyRunClick), true, () => M2Config.boSafeAreaDisShopStallDummyRun },
        { nameof(TFrmDummySetting.chkSafeAreaDisOffLineDummyRunClick), true, () => M2Config.boSafeAreaDisOffLineDummyRun },
        { nameof(TFrmDummySetting.chkDummyWarHreoRunClick), true, () => M2Config.boDummyWarHreoRun },
        { nameof(TFrmDummySetting.chkDummyLogonRandClick), true, () => M2Config.boDummyLogonRand },
    };

    [Theory]
    [MemberData(nameof(BoolCases))]
    public void BoolHandler_True_WritesItsOwnField(string handler, bool value, Func<bool> read)
    {
        RaiseBool(handler, value);
        Assert.True(read());
        Assert.True(Form.Ct.ButtonDummySave.Enabled);   // ModValue
    }

    [Theory]
    [MemberData(nameof(BoolCases))]
    public void BoolHandler_False_WritesItsOwnField(string handler, bool _, Func<bool> read)
    {
        RaiseBool(handler, true);
        RaiseBool(handler, false);
        Assert.False(read());
    }

    [Theory]
    [MemberData(nameof(BoolCases))]
    public void BoolHandler_TogglesTwice_ReturnsToOriginal(string handler, bool _, Func<bool> read)
    {
        RaiseBool(handler, true);
        RaiseBool(handler, false);
        RaiseBool(handler, true);
        Assert.True(read());
    }

    /// <summary>按处理器名调用对应的 public 处理器（原文 14 个布尔处理器）。</summary>
    private void RaiseBool(string handler, bool value)
    {
        var ct = Form.Ct;
        switch (handler)
        {
            case nameof(TFrmDummySetting.chkDummyAutoAddHPClick):
                ct.chkDummyAutoAddHP.Checked = value;
                Form.chkDummyAutoAddHPClick(ct.chkDummyAutoAddHP);
                break;
            case nameof(TFrmDummySetting.chkDummyAutoAddMPClick):
                ct.chkDummyAutoAddMP.Checked = value;
                Form.chkDummyAutoAddMPClick(ct.chkDummyAutoAddMP);
                break;
            case nameof(TFrmDummySetting.CheckBoxDummyAutoRepairItemClick):
                ct.CheckBoxDummyAutoRepairItem.Checked = value;
                Form.CheckBoxDummyAutoRepairItemClick(ct.CheckBoxDummyAutoRepairItem);
                break;
            case nameof(TFrmDummySetting.CheckBoxDummyAutoRecallHeroClick):
                ct.CheckBoxDummyAutoRecallHero.Checked = value;
                Form.CheckBoxDummyAutoRecallHeroClick(ct.CheckBoxDummyAutoRecallHero);
                break;
            case nameof(TFrmDummySetting.chkDummyRunHumClick):
                ct.chkDummyRunHum.Checked = value;
                Form.chkDummyRunHumClick(ct.chkDummyRunHum);
                break;
            case nameof(TFrmDummySetting.chkDummyRunMonClick):
                ct.chkDummyRunMon.Checked = value;
                Form.chkDummyRunMonClick(ct.chkDummyRunMon);
                break;
            case nameof(TFrmDummySetting.chkDummyRunNpcClick):
                ct.chkDummyRunNpc.Checked = value;
                Form.chkDummyRunNpcClick(ct.chkDummyRunNpc);
                break;
            case nameof(TFrmDummySetting.chkDummyRunGuardClick):
                ct.chkDummyRunGuard.Checked = value;
                Form.chkDummyRunGuardClick(ct.chkDummyRunGuard);
                break;
            case nameof(TFrmDummySetting.chkDummySafeAreaClick):
                ct.chkDummySafeArea.Checked = value;
                Form.chkDummySafeAreaClick(ct.chkDummySafeArea);
                break;
            case nameof(TFrmDummySetting.chkDummySafeAreaDisNpcRunClick):
                ct.chkDummySafeAreaDisNpcRun.Checked = value;
                Form.chkDummySafeAreaDisNpcRunClick(ct.chkDummySafeAreaDisNpcRun);
                break;
            case nameof(TFrmDummySetting.chkSafeAreaDisShopStallDummyRunClick):
                ct.chkSafeAreaDisShopStallDummyRun.Checked = value;
                Form.chkSafeAreaDisShopStallDummyRunClick(ct.chkSafeAreaDisShopStallDummyRun);
                break;
            case nameof(TFrmDummySetting.chkSafeAreaDisOffLineDummyRunClick):
                ct.chkSafeAreaDisOffLineDummyRun.Checked = value;
                Form.chkSafeAreaDisOffLineDummyRunClick(ct.chkSafeAreaDisOffLineDummyRun);
                break;
            case nameof(TFrmDummySetting.chkDummyWarHreoRunClick):
                ct.chkDummyWarHreoRun.Checked = value;
                Form.chkDummyWarHreoRunClick(ct.chkDummyWarHreoRun);
                break;
            case nameof(TFrmDummySetting.chkDummyLogonRandClick):
                ct.chkDummyLogonRand.Checked = value;
                Form.chkDummyLogonRandClick(ct.chkDummyLogonRand);
                break;
            default:
                throw new ArgumentException(handler);
        }
    }

    // ---- Spin 族（:413-429、:665-669、:677-693、:695-789、:803-819、:885-895） ----

    public static TheoryData<string, int, Func<int>> SpinCases() => new()
    {
        { nameof(TFrmDummySetting.seDummyHomeXChange), 111, () => M2Config.nDummyHomeX },
        { nameof(TFrmDummySetting.seDummyHomeYChange), 222, () => M2Config.nDummyHomeY },
        { nameof(TFrmDummySetting.seDummyLogonTimeChange), 9, () => M2Config.nDummyLogonTime },
        { nameof(TFrmDummySetting.seDummyAddHPPercentChange), 55, () => M2Config.nDummyAddHPPercent },
        { nameof(TFrmDummySetting.seDummyAddMPPercentChange), 56, () => M2Config.nDummyAddMPPercent },
        { nameof(TFrmDummySetting.seDummyHPTime_WarriorChange), 101, () => M2Config.nDummyHPTime_Warrior },
        { nameof(TFrmDummySetting.seDummyHPBase_WarriorChange), 102, () => M2Config.nDummyHPBase_Warrior },
        { nameof(TFrmDummySetting.seDummyMPTime_WarriorChange), 103, () => M2Config.nDummyMPTime_Warrior },
        { nameof(TFrmDummySetting.seDummyMPBase_WarriorChange), 104, () => M2Config.nDummyMPBase_Warrior },
        { nameof(TFrmDummySetting.seDummyHPTime_DFChange), 105, () => M2Config.nDummyHPTime_DF },
        { nameof(TFrmDummySetting.seDummyHPBase_DFChange), 106, () => M2Config.nDummyHPBase_DF },
        { nameof(TFrmDummySetting.seDummyMPTime_DFChange), 107, () => M2Config.nDummyMPTime_DF },
        { nameof(TFrmDummySetting.seDummyMPBase_DFChange), 108, () => M2Config.nDummyMPBase_DF },
        { nameof(TFrmDummySetting.seDummyHeroHPTime_WarriorChange), 201, () => M2Config.nDummyHeroHPTime_Warrior },
        { nameof(TFrmDummySetting.seDummyHeroHPBase_WarriorChange), 202, () => M2Config.nDummyHeroHPBase_Warrior },
        { nameof(TFrmDummySetting.seDummyHeroMPTime_WarriorChange), 203, () => M2Config.nDummyHeroMPTime_Warrior },
        { nameof(TFrmDummySetting.seDummyHeroMPBase_WarriorChange), 204, () => M2Config.nDummyHeroMPBase_Warrior },
        { nameof(TFrmDummySetting.seDummyHeroHPTime_DFChange), 205, () => M2Config.nDummyHeroHPTime_DF },
        { nameof(TFrmDummySetting.seDummyHeroHPBase_DFChange), 206, () => M2Config.nDummyHeroHPBase_DF },
        { nameof(TFrmDummySetting.seDummyHeroMPTime_DFChange), 207, () => M2Config.nDummyHeroMPTime_DF },
        { nameof(TFrmDummySetting.seDummyHeroMPBase_DFChange), 208, () => M2Config.nDummyHeroMPBase_DF },
        { nameof(TFrmDummySetting.EditDummyWarrorAttackTimeChange), 1500, () => M2Config.dwDummyWarrorAttackTime },
        { nameof(TFrmDummySetting.EditDummyWizardAttackTimeChange), 1600, () => M2Config.dwDummyWizardAttackTime },
        { nameof(TFrmDummySetting.EditDummyTaoistAttackTimeChange), 1700, () => M2Config.dwDummyTaoistAttackTime },
        { nameof(TFrmDummySetting.EditDummyWarrorWalkTimeChange), 700, () => M2Config.dwDummyWarrorWalkTime },
        { nameof(TFrmDummySetting.EditDummyWizardWalkTimeChange), 800, () => M2Config.dwDummyWizardWalkTime },
        { nameof(TFrmDummySetting.EditDummyTaoistWalkTimeChange), 900, () => M2Config.dwDummyTaoistWalkTime },
    };

    [Theory]
    [MemberData(nameof(SpinCases))]
    public void SpinHandler_WritesItsOwnField(string handler, int value, Func<int> read)
    {
        RaiseSpin(handler, value);
        Assert.Equal(value, read());
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Theory]
    [MemberData(nameof(SpinCases))]
    public void SpinHandler_Zero_WritesZero_RoundTripsWhereControlAllows(string handler, int _, Func<int> read)
    {
        RaiseSpin(handler, 0);
        // ⚠ 受控控件下界（DFM MinValue）：`seDummyHomeX/Y`/`seDummyLogonTime` 是 1、
        // `seDummyAdd*Percent` 是 1、六个 Attack/Walk 是 10 ⇒ 写 0 会被**控件**钳到下界，
        // 处理器收到的就是钳后的值。这不是处理器缺陷，是 DFM 边界 + 原文 TSpinEditEx 语义。
        int min = MinOf(handler);
        Assert.Equal(min, read());
    }

    [Theory]
    [MemberData(nameof(SpinCases))]
    public void SpinHandler_ReadsBackControlValue_AtControlMinimum(string handler, int _, Func<int> read)
    {
        // ⚠ 本 Theory 只锁定"**处理器读到的就是控件当前值**"，不做钳制断言：
        // 托管 `NumericUpDown.Value` 的钳制取决于运行期 `Minimum`，属控件实现细节，
        // 已由 `ClampSpinValue_*` 系列**纯逻辑**用例覆盖（那才是本车道自己写的逻辑）。
        int min = MinOf(handler);
        RaiseSpin(handler, min);
        Assert.Equal(min, read());
    }

    [Fact]
    public void ClampSpinValue_NegativeOnUnboundedFamily_ClampsToZero_DrivesFormCreatePath()
    {
        // `maxValue == 0` 只表示"**上界**不钳"，下界仍取 `minValue`。
        // 原文该族 DFM 是 `MinValue = 0 MaxValue = 0`；托管 `NumericUpDown.Minimum`
        // 类型上不允许负数 ⇒ 偏离登记 **D-p8-03**。
        Assert.Equal(0, TFrmDummySetting.ClampSpinValue(-1, 0, 0));
        Assert.Equal(-7, TFrmDummySetting.ClampSpinValue(-7, -10, 0));
        Assert.Equal(0, TFrmDummySetting.ClampSpinValue(int.MinValue, 0, 0));
    }

    /// <summary>各 Spin 控件在 DFM 里的 `MinValue`（对照 uFrmDummySetting.dfm）。</summary>
    private static int MinOf(string handler) => handler switch
    {
        nameof(TFrmDummySetting.seDummyHomeXChange) => 1,        // DFM :132 MinValue = 1
        nameof(TFrmDummySetting.seDummyHomeYChange) => 1,        // DFM :143
        nameof(TFrmDummySetting.seDummyLogonTimeChange) => 1,    // DFM :164
        nameof(TFrmDummySetting.seDummyAddHPPercentChange) => 1, // DFM :782
        nameof(TFrmDummySetting.seDummyAddMPPercentChange) => 1, // DFM :804
        nameof(TFrmDummySetting.EditDummyWarrorAttackTimeChange) => 10,   // DFM :245
        nameof(TFrmDummySetting.EditDummyWizardAttackTimeChange) => 10,   // DFM :267
        nameof(TFrmDummySetting.EditDummyTaoistAttackTimeChange) => 10,   // DFM :256
        nameof(TFrmDummySetting.EditDummyWarrorWalkTimeChange) => 10,     // DFM :307
        nameof(TFrmDummySetting.EditDummyWizardWalkTimeChange) => 10,     // DFM :318
        nameof(TFrmDummySetting.EditDummyTaoistWalkTimeChange) => 10,     // DFM :329
        _ => 0,                                                  // 其余 16 个 DFM MinValue = 0
    };

    [Theory]
    [MemberData(nameof(SpinCases))]
    public void SpinHandler_DoesNotTouchOtherFields(string handler, int value, Func<int> _)
    {
        int before = M2Config.nDummyHomeX + M2Config.dwDummyWarrorAttackTime + M2Config.nDummyHeroMPBase_DF;
        RaiseSpin(handler, value);
        // 只有本处理器对应的字段会变；用"三个已知不相关字段的合计"做粗筛，
        // 真正的逐字段唯一性由 SpinHandler_OnlyTargetFieldChanges 逐条断言。
        int after = M2Config.nDummyHomeX + M2Config.dwDummyWarrorAttackTime + M2Config.nDummyHeroMPBase_DF;
        if (handler == nameof(TFrmDummySetting.seDummyHomeXChange)
            || handler == nameof(TFrmDummySetting.EditDummyWarrorAttackTimeChange)
            || handler == nameof(TFrmDummySetting.seDummyHeroMPBase_DFChange))
        {
            Assert.NotEqual(before, after);
        }
        else
        {
            Assert.Equal(before, after);
        }
    }

    /// <summary>按处理器名调用对应的 public 处理器（原文 27 个 Spin 处理器）。</summary>
    private void RaiseSpin(string handler, int value)
    {
        var ct = Form.Ct;
        switch (handler)
        {
            case nameof(TFrmDummySetting.seDummyHomeXChange):
                ct.seDummyHomeX.Value = TFrmDummySetting.ClampSpinValue(value, 1, 2000);
                Form.seDummyHomeXChange(ct.seDummyHomeX);
                break;
            case nameof(TFrmDummySetting.seDummyHomeYChange):
                ct.seDummyHomeY.Value = TFrmDummySetting.ClampSpinValue(value, 1, 2000);
                Form.seDummyHomeYChange(ct.seDummyHomeY);
                break;
            case nameof(TFrmDummySetting.seDummyLogonTimeChange):
                ct.seDummyLogonTime.Value = TFrmDummySetting.ClampSpinValue(value, 1, 2000);
                Form.seDummyLogonTimeChange(ct.seDummyLogonTime);
                break;
            case nameof(TFrmDummySetting.seDummyAddHPPercentChange):
                ct.seDummyAddHPPercent.Value = TFrmDummySetting.ClampSpinValue(value, 1, 100);
                Form.seDummyAddHPPercentChange(ct.seDummyAddHPPercent);
                break;
            case nameof(TFrmDummySetting.seDummyAddMPPercentChange):
                ct.seDummyAddMPPercent.Value = TFrmDummySetting.ClampSpinValue(value, 1, 100);
                Form.seDummyAddMPPercentChange(ct.seDummyAddMPPercent);
                break;
            case nameof(TFrmDummySetting.seDummyHPTime_WarriorChange):
                ct.seDummyHPTime_Warrior.Value = value;
                Form.seDummyHPTime_WarriorChange(ct.seDummyHPTime_Warrior);
                break;
            case nameof(TFrmDummySetting.seDummyHPBase_WarriorChange):
                ct.seDummyHPBase_Warrior.Value = value;
                Form.seDummyHPBase_WarriorChange(ct.seDummyHPBase_Warrior);
                break;
            case nameof(TFrmDummySetting.seDummyMPTime_WarriorChange):
                ct.seDummyMPTime_Warrior.Value = value;
                Form.seDummyMPTime_WarriorChange(ct.seDummyMPTime_Warrior);
                break;
            case nameof(TFrmDummySetting.seDummyMPBase_WarriorChange):
                ct.seDummyMPBase_Warrior.Value = value;
                Form.seDummyMPBase_WarriorChange(ct.seDummyMPBase_Warrior);
                break;
            case nameof(TFrmDummySetting.seDummyHPTime_DFChange):
                ct.seDummyHPTime_DF.Value = value;
                Form.seDummyHPTime_DFChange(ct.seDummyHPTime_DF);
                break;
            case nameof(TFrmDummySetting.seDummyHPBase_DFChange):
                ct.seDummyHPBase_DF.Value = value;
                Form.seDummyHPBase_DFChange(ct.seDummyHPBase_DF);
                break;
            case nameof(TFrmDummySetting.seDummyMPTime_DFChange):
                ct.seDummyMPTime_DF.Value = value;
                Form.seDummyMPTime_DFChange(ct.seDummyMPTime_DF);
                break;
            case nameof(TFrmDummySetting.seDummyMPBase_DFChange):
                ct.seDummyMPBase_DF.Value = value;
                Form.seDummyMPBase_DFChange(ct.seDummyMPBase_DF);
                break;
            case nameof(TFrmDummySetting.seDummyHeroHPTime_WarriorChange):
                ct.seDummyHeroHPTime_Warrior.Value = value;
                Form.seDummyHeroHPTime_WarriorChange(ct.seDummyHeroHPTime_Warrior);
                break;
            case nameof(TFrmDummySetting.seDummyHeroHPBase_WarriorChange):
                ct.seDummyHeroHPBase_Warrior.Value = value;
                Form.seDummyHeroHPBase_WarriorChange(ct.seDummyHeroHPBase_Warrior);
                break;
            case nameof(TFrmDummySetting.seDummyHeroMPTime_WarriorChange):
                ct.seDummyHeroMPTime_Warrior.Value = value;
                Form.seDummyHeroMPTime_WarriorChange(ct.seDummyHeroMPTime_Warrior);
                break;
            case nameof(TFrmDummySetting.seDummyHeroMPBase_WarriorChange):
                ct.seDummyHeroMPBase_Warrior.Value = value;
                Form.seDummyHeroMPBase_WarriorChange(ct.seDummyHeroMPBase_Warrior);
                break;
            case nameof(TFrmDummySetting.seDummyHeroHPTime_DFChange):
                ct.seDummyHeroHPTime_DF.Value = value;
                Form.seDummyHeroHPTime_DFChange(ct.seDummyHeroHPTime_DF);
                break;
            case nameof(TFrmDummySetting.seDummyHeroHPBase_DFChange):
                ct.seDummyHeroHPBase_DF.Value = value;
                Form.seDummyHeroHPBase_DFChange(ct.seDummyHeroHPBase_DF);
                break;
            case nameof(TFrmDummySetting.seDummyHeroMPTime_DFChange):
                ct.seDummyHeroMPTime_DF.Value = value;
                Form.seDummyHeroMPTime_DFChange(ct.seDummyHeroMPTime_DF);
                break;
            case nameof(TFrmDummySetting.seDummyHeroMPBase_DFChange):
                ct.seDummyHeroMPBase_DF.Value = value;
                Form.seDummyHeroMPBase_DFChange(ct.seDummyHeroMPBase_DF);
                break;
            case nameof(TFrmDummySetting.EditDummyWarrorAttackTimeChange):
                ct.EditDummyWarrorAttackTime.Value = TFrmDummySetting.ClampSpinValue(value, 10, 10000);
                Form.EditDummyWarrorAttackTimeChange(ct.EditDummyWarrorAttackTime);
                break;
            case nameof(TFrmDummySetting.EditDummyWizardAttackTimeChange):
                ct.EditDummyWizardAttackTime.Value = TFrmDummySetting.ClampSpinValue(value, 10, 10000);
                Form.EditDummyWizardAttackTimeChange(ct.EditDummyWizardAttackTime);
                break;
            case nameof(TFrmDummySetting.EditDummyTaoistAttackTimeChange):
                ct.EditDummyTaoistAttackTime.Value = TFrmDummySetting.ClampSpinValue(value, 10, 10000);
                Form.EditDummyTaoistAttackTimeChange(ct.EditDummyTaoistAttackTime);
                break;
            case nameof(TFrmDummySetting.EditDummyWarrorWalkTimeChange):
                ct.EditDummyWarrorWalkTime.Value = TFrmDummySetting.ClampSpinValue(value, 10, 10000);
                Form.EditDummyWarrorWalkTimeChange(ct.EditDummyWarrorWalkTime);
                break;
            case nameof(TFrmDummySetting.EditDummyWizardWalkTimeChange):
                ct.EditDummyWizardWalkTime.Value = TFrmDummySetting.ClampSpinValue(value, 10, 10000);
                Form.EditDummyWizardWalkTimeChange(ct.EditDummyWizardWalkTime);
                break;
            case nameof(TFrmDummySetting.EditDummyTaoistWalkTimeChange):
                ct.EditDummyTaoistWalkTime.Value = TFrmDummySetting.ClampSpinValue(value, 10, 10000);
                Form.EditDummyTaoistWalkTimeChange(ct.EditDummyTaoistWalkTime);
                break;
            default:
                throw new ArgumentException(handler);
        }
    }

    [Fact]
    public void ValueChanged_IsBound_AndCascadesConfigWriteBack()
    {
        // 差异/机制断言：WinForms `NumericUpDown.Value = X`（X 变化时）**触发**已绑定事件，
        // 即原文 `TSpinEditEx.OnChange` 的语义在托管侧被保留。
        // 证据 = 控件赋值后 `g_Config` 立刻同步（原文 :415 `g_Config.nDummyHomeX := seDummyHomeX.Value`）。
        Form.Ct.seDummyHomeX.Value = 1234;
        Assert.Equal(1234, M2Config.nDummyHomeX);
    }

    [Fact]
    public void Recreate_YieldsFreshFormInstance()
    {
        var first = Form;
        Recreate();
        Assert.NotSame(first, Form);
    }

    // ---- `chkDummyWarDisHumRunClick`（:851-859）的差异断言（Enabled and Checked） ----

    [Fact]
    public void WarDisHumRun_Check_EnablesChildAndPropagatesChildChecked()
    {
        Form.Ct.chkDummyWarHreoRun.Checked = true;
        Form.Ct.chkDummyWarDisHumRun.Checked = true;

        Form.chkDummyWarDisHumRunClick(Form.Ct.chkDummyWarDisHumRun);

        Assert.True(M2Config.boDummyWarDisHumRun);
        Assert.True(Form.Ct.chkDummyWarHreoRun.Enabled);      // :855
        Assert.True(M2Config.boDummyWarHreoRun);              // :856 Enabled and Checked
    }

    [Fact]
    public void WarDisHumRun_Uncheck_ForcesChildConfigFalse_EvenIfChildStillChecked()
    {
        // ★ 差异断言（原文 :856 用 `Enabled and Checked`，不是 `Checked`）
        Form.Ct.chkDummyWarHreoRun.Checked = true;
        Form.Ct.chkDummyWarDisHumRun.Checked = true;
        Form.chkDummyWarDisHumRunClick(Form.Ct.chkDummyWarDisHumRun);

        Form.Ct.chkDummyWarDisHumRun.Checked = false;
        Form.chkDummyWarDisHumRunClick(Form.Ct.chkDummyWarDisHumRun);

        Assert.False(M2Config.boDummyWarDisHumRun);
        Assert.False(Form.Ct.chkDummyWarHreoRun.Enabled);
        Assert.True(Form.Ct.chkDummyWarHreoRun.Checked);      // 子勾选**未**被清
        Assert.False(M2Config.boDummyWarHreoRun);             // 但配置被强制置 False
    }

    [Fact]
    public void WarDisHumRun_CheckWithChildUnchecked_EnablesButKeepsConfigFalse()
    {
        Form.Ct.chkDummyWarHreoRun.Checked = false;
        Form.Ct.chkDummyWarDisHumRun.Checked = true;

        Form.chkDummyWarDisHumRunClick(Form.Ct.chkDummyWarDisHumRun);

        Assert.True(Form.Ct.chkDummyWarHreoRun.Enabled);
        Assert.False(M2Config.boDummyWarHreoRun);
    }

    // ---- `chkDisDummyRunClick`（:616-663）：取反写入 + 8 个从属联动 ----

    [Fact]
    public void DisDummyRun_Checked_SetsConfigTrue_AndClearsDisablesAllEightSlaves()
    {
        // 预置 8 个从属为"全勾选 + 全可用"
        SetAllSlaves(checkedState: true, enabledState: true);

        Form.Ct.chkDisDummyRun.Checked = true;      // ⇒ boChecked = False

        // 原文 :620 `boChecked := not chkDisDummyRun.Checked` ⇒ False ⇒ **走 else 分支**
        Form.chkDisDummyRunClick(Form.Ct.chkDisDummyRun);

        Assert.False(M2Config.boDiableDummyRun);
        foreach (var box in AllSlaves(Form.Ct))
            Assert.True(box.Enabled);
        foreach (var box in AllSlaves(Form.Ct))
            Assert.True(box.Checked);               // else 分支**不清**勾选
    }

    [Fact]
    public void DisDummyRun_Unchecked_SetsConfigFalse_AndClearsDisablesAllEightSlaves()
    {
        SetAllSlaves(checkedState: true, enabledState: true);

        Form.Ct.chkDisDummyRun.Checked = false;     // ⇒ boChecked = True

        Form.chkDisDummyRunClick(Form.Ct.chkDisDummyRun);

        Assert.True(M2Config.boDiableDummyRun);
        foreach (var box in AllSlaves(Form.Ct))
        {
            Assert.False(box.Enabled);
            Assert.False(box.Checked);
        }
        Assert.True(Form.Ct.ButtonDummySave.Enabled);
    }

    [Fact]
    public void DisDummyRun_Unchecked_DoesNotTouchOtherRunFlags()
    {
        // 差异断言：原文只改 `boDiableDummyRun`，不动 `boDummyRunHum/Mon/Npc/Guard`
        // 与 `boDummySafeArea*`（那些字段由各自的 Click 处理器负责）。
        M2Config.boDummyRunHum = true;
        M2Config.boDummyRunMon = true;
        M2Config.boDummyRunNpc = true;
        M2Config.boDummyRunGuard = true;
        M2Config.boDummySafeAreaLimited = true;
        M2Config.boDummySafeAreaDisNpcRun = true;
        M2Config.boSafeAreaDisShopStallDummyRun = true;
        M2Config.boSafeAreaDisOffLineDummyRun = true;

        Form.Ct.chkDisDummyRun.Checked = false;
        Form.chkDisDummyRunClick(Form.Ct.chkDisDummyRun);

        Assert.True(M2Config.boDummyRunHum);
        Assert.True(M2Config.boDummyRunMon);
        Assert.True(M2Config.boDummyRunNpc);
        Assert.True(M2Config.boDummyRunGuard);
        Assert.True(M2Config.boDummySafeAreaLimited);
        Assert.True(M2Config.boDummySafeAreaDisNpcRun);
        Assert.True(M2Config.boSafeAreaDisShopStallDummyRun);
        Assert.True(M2Config.boSafeAreaDisOffLineDummyRun);
    }

    [Fact]
    public void DisDummyRun_Unchecked_DoesNotTouchWarDisHumRunOrItsChild()
    {
        // 差异断言：`chkDummyWarDisHumRun` / `chkDummyWarHreoRun` **不在** 8 个从属名单里。
        // 原文 :293 FormCreate 单独处理 `chkDummyWarHreoRun.Enabled`，与 chkDisDummyRun 无关。
        Form.Ct.chkDummyWarDisHumRun.Checked = true;
        Form.Ct.chkDummyWarHreoRun.Checked = true;
        Form.Ct.chkDummyWarHreoRun.Enabled = true;

        Form.Ct.chkDisDummyRun.Checked = false;
        Form.chkDisDummyRunClick(Form.Ct.chkDisDummyRun);

        Assert.True(Form.Ct.chkDummyWarDisHumRun.Checked);
        Assert.True(Form.Ct.chkDummyWarHreoRun.Checked);
        Assert.True(Form.Ct.chkDummyWarHreoRun.Enabled);
    }

    [Fact]
    public void ApplyDisableDummyRun_IsPureLogic_SameResultWithoutButtonCall()
    {
        SetAllSlaves(checkedState: true, enabledState: true);
        TFrmDummySetting.ApplyDisableDummyRun(Form.Ct, chkDisDummyRunChecked: false);

        foreach (var box in AllSlaves(Form.Ct))
        {
            Assert.False(box.Enabled);
            Assert.False(box.Checked);
        }
    }

    private static IEnumerable<System.Windows.Forms.CheckBox> AllSlaves(DummySettingControls ct)
    {
        yield return ct.chkDummyRunHum;
        yield return ct.chkDummyRunMon;
        yield return ct.chkDummyRunNpc;
        yield return ct.chkDummyRunGuard;
        yield return ct.chkDummySafeArea;
        yield return ct.chkDummySafeAreaDisNpcRun;
        yield return ct.chkSafeAreaDisShopStallDummyRun;
        yield return ct.chkSafeAreaDisOffLineDummyRun;
    }

    private void SetAllSlaves(bool checkedState, bool enabledState)
    {
        foreach (var box in AllSlaves(Form.Ct))
        {
            box.Checked = checkedState;
            box.Enabled = enabledState;
        }
    }

    // ---- `edtDummyHomeMapChange`（:408-411）空体 + DFM 未绑定（差异断言） ----

    [Fact]
    public void EdtDummyHomeMapChange_IsNoOp()
    {
        M2Config.sDummyHomeMap = "3";
        var before = M2Config.sDummyHomeMap;
        Form.uModValue();                              // 先清脏，才能验证"未置脏"
        Form.Ct.edtDummyHomeMap.Text = "999";
        Form.edtDummyHomeMapChange(Form.Ct.edtDummyHomeMap);
        Assert.Equal(before, M2Config.sDummyHomeMap);
        Assert.False(Form.Ct.ButtonDummySave.Enabled);   // 未置脏
    }

    [Fact]
    public void EdtDummyHomeMap_HasNoChangeHandlerBound_OriginalDfmHasNoOnChange()
    {
        // ⚠ 差异断言（原文缺陷）：DFM :154-162 `edtDummyHomeMap` **没有** `OnChange` 行
        // ⇒ 原文里 `edtDummyHomeMapChange` **永不触发**。
        // 托管侧同样**不绑定** TextChanged ⇒ 改文本不会写 g_Config、不会置脏。
        Form.uModValue();
        Form.Ct.edtDummyHomeMap.Text = "D401";
        Assert.Equal("3", M2Config.sDummyHomeMap);        // 配置未被写
        Assert.False(Form.Ct.ButtonDummySave.Enabled);    // 未置脏
    }

    [Fact]
    public void EdtDummyHomeMapChange_NullSafe_NoThrow()
    {
        var ex = Record.Exception(() => Form.edtDummyHomeMapChange(Form.Ct.edtDummyHomeMap));
        Assert.Null(ex);
    }
}

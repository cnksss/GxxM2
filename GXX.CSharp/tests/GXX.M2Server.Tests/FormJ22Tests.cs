using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J22：GameConfig.pas 巨片拆分第一片（GameSpeed 游戏速度页）1:1 转换测试。</summary>
public sealed class GameConfigGameSpeedTests : IDisposable
{
    private readonly string _dir;
    private readonly GameConfigForm _form;

    public GameConfigGameSpeedTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j22_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetGameSpeedDefaults();
        GameConfigState.SendServerConfigCalls = 0;
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() => new GameConfigForm());
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [Fact]
    public void Open_RefGameSpeedConf_FillsControls()
    {
        StaRunner.New(() =>
        {
            M2Config.boSpeedControl = true;
            M2Config.dwHitIntervalTime = 800;
            M2Config.dwMagicHitIntervalTime = 500;
            M2Config.nMaxHitMsgCount = 2;
            M2Config.nOverSpeedKickCount = 5;
            M2Config.boKickOverSpeed = true;
            M2Config.dwDropOverSpeed = 200;
            M2Config.btSpeedControlMode = 1;
            M2Config.boDisableStruck = true;
            M2Config.dwStruckTime = 123;
            M2Config.boSpellSendUpdateMsg = true;
            M2Config.boCheckActionCount = true;
            M2Config.nCanHitCount = 7;
            M2Config.nCheckMoveCount = 9;
            M2Config.boSendUpdateMsg = true;
            M2Config.nMaxHitDeliveryTime = 123;
            M2Config.boHorseRun3Grid = true;

            _form.Open(showModal: false);

            Assert.True(_form.chkSpeedControl.Checked);
            Assert.Equal(800, (int)_form.EditHitIntervalTime.Value);
            Assert.Equal(500, (int)_form.EditMagicHitIntervalTime.Value);
            Assert.Equal(2, (int)_form.EditMaxHitMsgCount.Value);
            Assert.Equal(5, (int)_form.EditOverSpeedKickCount.Value);
            Assert.True(_form.EditOverSpeedKickCount.Enabled); // boKickOverSpeed && boSpeedControl
            Assert.Equal(200, (int)_form.EditDropOverSpeed.Value);
            Assert.False(_form.RadioButtonDelyMode.Checked);
            Assert.True(_form.RadioButtonFilterMode.Checked);
            Assert.True(_form.CheckBoxDisableStruck.Checked);
            Assert.False(_form.EditStruckTime.Enabled); // 弯腰时间随 DisableStruck 禁用
            Assert.Equal(123, (int)_form.EditStruckTime.Value);
            Assert.True(_form.CheckBoxSpellSendUpdateMsg.Checked);
            Assert.True(_form.CheckBoxCheckActionCount.Checked);
            Assert.Equal(7, (int)_form.EditCanHitCount.Value);
            Assert.Equal(9, (int)_form.EditCheckMoveCount.Value);
            Assert.True(_form.CheckBoxSendUpdateMsg.Checked);
            Assert.Equal(123, (int)_form.EditMaxHitDeliveryTime.Value);
            Assert.True(_form.chkHorseRun3Grid.Checked);
            Assert.False(_form.IsModValued);
            Assert.False(_form.ButtonGameSpeedSave.Enabled);
        });
    }

    [Fact]
    public void ChangeHandlers_GatedByBoOpened()
    {
        StaRunner.New(() =>
        {
            _form.EditHitIntervalTime.Value = 900; // 未 Open：boOpened 门控拦截
            Assert.Equal(700, (int)M2Config.dwHitIntervalTime);
            Assert.False(_form.IsModValued);

            _form.Open(showModal: false);
            _form.EditHitIntervalTime.Value = 800;
            Assert.Equal(800, (int)M2Config.dwHitIntervalTime);
            Assert.True(_form.IsModValued);
            Assert.True(_form.ButtonGameSpeedSave.Enabled);
        });
    }

    [Fact]
    public void RadioButtons_SetSpeedControlMode()
    {
        StaRunner.New(() =>
        {
            _form.RadioButtonFilterMode.Checked = true;
            _form.RadioButtonFilterModeClick(_form); // 未 Open 不写入
            Assert.Equal(0, (int)M2Config.btSpeedControlMode);

            _form.Open(showModal: false);
            _form.RadioButtonFilterMode.Checked = true;
            _form.RadioButtonFilterModeClick(_form);
            Assert.Equal(1, (int)M2Config.btSpeedControlMode);
            _form.RadioButtonDelyMode.Checked = true;
            _form.RadioButtonDelyModeClick(_form);
            Assert.Equal(0, (int)M2Config.btSpeedControlMode);
        });
    }

    [Fact]
    public void KickOverSpeed_AndStruck_EnableLogic()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxboKickOverSpeed.Checked = false;
            _form.CheckBoxboKickOverSpeedClick(_form);
            Assert.False(_form.EditOverSpeedKickCount.Enabled);
            Assert.False(M2Config.boKickOverSpeed);

            _form.CheckBoxDisableStruck.Checked = true;
            _form.CheckBoxDisableStruckClick(_form);
            Assert.False(_form.EditStruckTime.Enabled);
            Assert.True(M2Config.boDisableStruck);

            _form.CheckBoxDisableSelfStruck.Checked = false; // Delphi Click 自动翻转等效
            _form.CheckBoxDisableSelfStruckClick(_form);
            Assert.False(M2Config.boDisableSelfStruck);

            _form.chkMagicshieldStruck.Checked = true;
            _form.chkMagicshieldStruckClick(_form);
            Assert.True(M2Config.boMagicshieldStruck);
        });
    }

    [Fact]
    public void ButtonGameSpeedDefault_IDNO_KeepsValues()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditHitIntervalTime.Value = 901;
            M2Forms.NextAnswer = M2Forms.IDNO; // 独立注入：拒绝恢复默认
            _form.ButtonGameSpeedDefaultClick(_form);
            Assert.Equal(901, (int)M2Config.dwHitIntervalTime);
            Assert.True(_form.IsModValued); // 提前 Exit：不触发 ModValue，之前的状态保留
        });
    }

    [Fact]
    public void ButtonGameSpeedDefault_IDYES_RestoresDefaults()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.dwHitIntervalTime = 901;
            M2Config.nMaxHitMsgCount = 2;
            M2Config.boDisableSelfStruck = false;
            M2Config.btSpeedControlMode = 1;
            M2Forms.NextAnswer = M2Forms.IDYES;
            _form.ButtonGameSpeedDefaultClick(_form);
            Assert.Equal(700, (int)M2Config.dwHitIntervalTime);
            Assert.Equal(1, M2Config.nMaxHitMsgCount);
            Assert.True(M2Config.boDisableSelfStruck);
            Assert.Equal(0, (int)M2Config.btSpeedControlMode);
            Assert.Equal(700, (int)_form.EditHitIntervalTime.Value); // RefGameSpeedConf 回显
            Assert.True(_form.IsModValued);
            Assert.True(_form.ButtonGameSpeedSave.Enabled);
        });
    }

    [Fact]
    public void ButtonGameSpeedSave_WritesIni_AndSendsServerConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditHitIntervalTime.Value = 800; // 触发 boSendServerConfig
            _form.ButtonGameSpeedSaveClick(_form);
            Assert.Equal(800, M2ShareState.ConfigIni.ReadInteger("Setup", "HitIntervalTime", -1));
            Assert.Equal(1, M2ShareState.ConfigIni.ReadInteger("Setup", "MaxSitDonwMsgCount", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "SpeedControl", false));
            Assert.Equal(0, M2ShareState.ConfigIni.ReadInteger("Setup", "SpeedControlMode", -1));
            Assert.Equal(1, GameConfigState.SendServerConfigCalls);
            Assert.False(_form.ButtonGameSpeedSave.Enabled); // uModValue

            _form.ButtonGameSpeedSaveClick(_form); // 未再修改：不再下发
            Assert.Equal(1, GameConfigState.SendServerConfigCalls);
        });
    }

    [Fact]
    public void ButtonCheckActionDefault_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.dwHitCountIntervalTime = 999;
            M2Forms.NextAnswer = M2Forms.IDYES;
            _form.ButtonCheckActionDefaultClick(_form);
            Assert.Equal(800, (int)M2Config.dwHitCountIntervalTime);
            Assert.Equal(1500, (int)M2Config.dwMagicHitCountIntervalTime);
            Assert.Equal(800, (int)M2Config.dwMoveCountIntervalTime);
            Assert.Equal(3, (int)M2Config.nCanHitCount);
            Assert.Equal(3, (int)M2Config.nCanMagicHitCount);
            Assert.Equal(3, (int)M2Config.nCanMoveCount);
            Assert.Equal(4, (int)M2Config.nCheckHitCount);
            Assert.Equal(3, (int)M2Config.nCheckMagicHitCount);
            Assert.Equal(4, (int)M2Config.nCheckMoveCount);
            Assert.True(M2Config.boCheckActionCount);

            _form.ButtonCheckActionSaveClick(_form);
            Assert.Equal(1, M2ShareState.ConfigIni.ReadInteger("Setup", "CheckActionCount", 0));
            Assert.Equal(800, M2ShareState.ConfigIni.ReadInteger("Setup", "HitCountIntervalTime", -1));
            Assert.Equal(4, M2ShareState.ConfigIni.ReadInteger("Setup", "CheckMoveCount", -1));
            Assert.False(_form.ButtonCheckActionSave.Enabled);
        });
    }

    [Fact]
    public void GameConfigControlChanging_ConfirmDiscard()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            Assert.True(_form.GameConfigControlChanging()); // 未修改：直接允许
            _form.EditRunIntervalTime.Value = 650;
            Assert.True(_form.IsModValued);
            M2Forms.NextAnswer = M2Forms.IDYES;
            Assert.True(_form.GameConfigControlChanging()); // 确认放弃 → uModValue
            Assert.False(_form.IsModValued);
            _form.EditRunIntervalTime.Value = 660;
            M2Forms.NextAnswer = M2Forms.IDNO;
            Assert.False(_form.GameConfigControlChanging()); // 拒绝 → 阻止切页
            Assert.True(_form.IsModValued);
        });
    }
}

/// <summary>批次J22：FunctionConfig.pas 巨片拆分第一片（密码保护页 + 常规页 Hunger/名称颜色/刻名前缀）。</summary>
public sealed class FunctionConfigLockGeneralTests : IDisposable
{
    private readonly string _dir;
    private readonly FunctionConfigForm _form;

    public FunctionConfigLockGeneralTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j22f_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetFunctionDefaults();
        GameConfigState.SendServerConfigCalls = 0;
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() =>
        {
            var f = new FunctionConfigForm();
            f.GetNameInFilterListHandler = _ => false; // 默认无非法字符
            return f;
        });
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [Fact]
    public void DoOpen_FillsLockAndGeneralControls()
    {
        StaRunner.New(() =>
        {
            M2Config.boPasswordLockSystem = true;
            M2Config.boLockGetBackItemAction = true;
            M2Config.boLockHumanLogin = true;
            M2Config.boLockWalkAction = true;
            M2Config.nPasswordErrorCountLock = 5;
            M2Config.boHungerSystem = true;
            M2Config.btPKFlagNameColor = 0x11;

            _form.Open(showModal: false);

            Assert.True(_form.CheckBoxEnablePasswordLock.Checked);
            Assert.True(_form.CheckBoxLockGetBackItem.Checked);
            Assert.True(_form.CheckBoxLockLogin.Checked);
            Assert.True(_form.CheckBoxLockWalk.Checked);
            Assert.True(_form.CheckBoxLockWalk.Enabled); // Login 级联使能
            Assert.Equal(5, (int)_form.EditErrorPasswordCount.Value);
            Assert.True(_form.CheckBoxHungerSystem.Checked);
            Assert.True(_form.CheckBoxHungerDecHP.Enabled); // Hunger 级联使能
            Assert.Equal(0x11, (int)_form.EditPKFlagNameColor.Value);
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void EnablePasswordLock_CascadeAndConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxEnablePasswordLock.Checked = true;
            _form.CheckBoxEnablePasswordLockClick(_form);
            Assert.True(M2Config.boPasswordLockSystem);
            Assert.True(_form.CheckBoxLockGetBackItem.Enabled);
            Assert.True(_form.CheckBoxLockLogin.Enabled);

            _form.CheckBoxEnablePasswordLock.Checked = false;
            _form.CheckBoxEnablePasswordLockClick(_form); // 级联清空并禁用
            Assert.False(M2Config.boPasswordLockSystem);
            Assert.False(_form.CheckBoxLockGetBackItem.Checked);
            Assert.False(_form.CheckBoxLockGetBackItem.Enabled);
            Assert.False(_form.CheckBoxLockLogin.Checked);
            Assert.False(_form.CheckBoxLockLogin.Enabled);
        });
    }

    [Fact]
    public void LockLogin_Cascade()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxLockLogin.Checked = true;
            _form.CheckBoxLockLoginClick(_form);
            Assert.True(M2Config.boLockHumanLogin);
            Assert.True(_form.CheckBoxLockRun.Enabled);
            Assert.True(_form.chkLockStall.Enabled);

            _form.CheckBoxLockLogin.Checked = false;
            _form.CheckBoxLockLoginClick(_form);
            Assert.False(M2Config.boLockHumanLogin);
            Assert.False(_form.CheckBoxLockRun.Checked);
            Assert.False(_form.CheckBoxLockRun.Enabled);
            Assert.False(_form.chkLockStall.Enabled);
        });
    }

    [Fact]
    public void LockActionHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxLockDealItem.Checked = true;
            _form.CheckBoxLockDealItemClick(_form);
            _form.CheckBoxLockDropItem.Checked = true;
            _form.CheckBoxLockDropItemClick(_form);
            _form.CheckBoxLockUseItem.Checked = true;
            _form.CheckBoxLockUseItemClick(_form);
            _form.CheckBoxLockRun.Checked = true;
            _form.CheckBoxLockRunClick(_form);
            _form.CheckBoxLockHit.Checked = true;
            _form.CheckBoxLockHitClick(_form);
            _form.CheckBoxLockSpell.Checked = true;
            _form.CheckBoxLockSpellClick(_form);
            _form.CheckBoxLockSendMsg.Checked = true;
            _form.CheckBoxLockSendMsgClick(_form);
            _form.CheckBoxLockInObMode.Checked = true;
            _form.CheckBoxLockInObModeClick(_form);
            _form.chkLockChallenge.Checked = true;
            _form.chkLockChallengeClick(_form);
            _form.chkLockSummonHero.Checked = true;
            _form.chkLockSummonHeroClick(_form);
            _form.chkLockShop.Checked = true;
            _form.chkLockShopClick(_form);
            _form.chkLockStall.Checked = true;
            _form.chkLockStallClick(_form);

            Assert.True(M2Config.boLockDealAction);
            Assert.True(M2Config.boLockDropAction);
            Assert.True(M2Config.boLockUserItemAction);
            Assert.True(M2Config.boLockRunAction);
            Assert.True(M2Config.boLockHitAction);
            Assert.True(M2Config.boLockSpellAction);
            Assert.True(M2Config.boLockSendMsgAction);
            Assert.True(M2Config.boLockInObModeAction);
            Assert.True(M2Config.boLockChallenge);
            Assert.True(M2Config.boLockSummonHero);
            Assert.True(M2Config.boLockShop);
            Assert.True(M2Config.boLockStall);
        });
    }

    [Fact]
    public void ButtonPasswordLockSave_WritesIni_NoSendWithoutFlag()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxLockDealItem.Checked = true;
            _form.CheckBoxLockDealItemClick(_form);
            _form.ButtonPasswordLockSaveClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "PasswordLockDealAction", false));
            Assert.Equal(3, M2ShareState.ConfigIni.ReadInteger("Setup", "PasswordErrorCountLock", -1));
            Assert.False(_form.ButtonPasswordLockSave.Enabled); // uModValue
            Assert.Equal(0, GameConfigState.SendServerConfigCalls); // boSendServerConfig 未置位则不下发
        });
    }

    [Fact]
    public void ButtonGeneralSave_ItemNameFilter_Rejects()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxHungerSystem.Checked = true; // 先改动触发 ModValue（保存钮可用）
            _form.CheckBoxHungerSystemClick(_form);
            _form.CheckBoxItemName.Checked = true;
            _form.EditItemName.Text = "非法*前缀";
            _form.GetNameInFilterListHandler = _ => true; // 含非法字符
            _form.ButtonGeneralSaveClick(_form);
            Assert.Equal("装备刻名自定义前缀包含非法字符", M2Forms.LastMessage);
            Assert.True(_form.ButtonGeneralSave.Enabled); // Exit：未落盘也未 uModValue
            Assert.Equal(-1, M2ShareState.ConfigIni.ReadInteger("Setup", "HungerSystem", -1)); // 未写盘
        });
    }

    [Fact]
    public void ButtonGeneralSave_WritesIni_AndAlwaysSends()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.boHungerSystem = true;
            M2Config.btPKFlagNameColor = 0x22;
            _form.ButtonGeneralSaveClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "HungerSystem", false));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "HungerDecHP", false) == M2Config.boHungerDecHP);
            Assert.Equal(0x22, M2ShareState.ConfigIni.ReadInteger("Setup", "PKFlagNameColor", -1));
            Assert.Equal(10, M2ShareState.ConfigIni.ReadInteger("Setup", "HPRockRate", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "DropOverLapItem", false));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "OpenMapEvent", true));
            Assert.Equal(1, GameConfigState.SendServerConfigCalls); // Delphi 尾部无条件 SendServerConfig
            Assert.False(_form.ButtonGeneralSave.Enabled);
        });
    }

    [Fact]
    public void CheckBoxHungerSystem_Cascade()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxHungerSystem.Checked = true;
            _form.CheckBoxHungerSystemClick(_form);
            Assert.True(M2Config.boHungerSystem);
            Assert.True(_form.CheckBoxHungerDecHP.Enabled);
            _form.CheckBoxHungerDecHP.Checked = true;
            _form.CheckBoxHungerDecHPClick(_form);
            Assert.True(M2Config.boHungerDecHP);

            _form.CheckBoxHungerSystem.Checked = false;
            _form.CheckBoxHungerSystemClick(_form);
            Assert.False(_form.CheckBoxHungerDecHP.Checked);
            Assert.False(_form.CheckBoxHungerDecHP.Enabled);
            Assert.False(_form.CheckBoxHungerDecPower.Enabled);
        });
    }

    [Fact]
    public void FunctionConfigControlChanging_ConfirmDiscard()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            Assert.True(_form.FunctionConfigControlChanging());
            _form.CheckBoxLockWalk.Checked = true;
            _form.CheckBoxLockWalkClick(_form);
            Assert.True(_form.IsModValued);
            M2Forms.NextAnswer = M2Forms.IDYES;
            Assert.True(_form.FunctionConfigControlChanging());
            Assert.False(_form.IsModValued);
            _form.CheckBoxLockHit.Checked = true;
            _form.CheckBoxLockHitClick(_form);
            M2Forms.NextAnswer = M2Forms.IDNO;
            Assert.False(_form.FunctionConfigControlChanging());
            Assert.True(_form.IsModValued);
        });
    }
}

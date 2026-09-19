using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J26：FunctionConfig.pas 巨片第三片（Skill 技能页头部 + UpgradeWeapon 升级武器页）1:1 测试。</summary>
public sealed class FunctionConfigSkillUpgradeTests : IDisposable
{
    private readonly string _dir;
    private readonly FunctionConfigForm _form;

    public FunctionConfigSkillUpgradeTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j26_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetFunctionDefaults();
        M2Config.ResetFunctionSkillDefaults();
        M2Config.ResetMagicExDefaults();
        GameConfigState.SendServerConfigCalls = 0;
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() =>
        {
            var f = new FunctionConfigForm();
            f.GetMonRaceHandler = _ => 5; // 默认全部怪物名有效
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
    public void Open_RefMagicSkill_And_UpgradeWeapon()
    {
        StaRunner.New(() =>
        {
            M2Config.nSwordLongPowerRate = 120;
            M2Config.boLimitSwordLong = true;
            M2Config.nFireBoomRage = 3;
            M2Config.nSnowWindRange = 2;
            M2Config.nElecBlizzardRange = 4;
            M2Config.nMagicAttackRage = 9;
            M2Config.nAmyOunsulPoint = 15;
            M2Config.nMagTurnUndeadLevel = 60;
            M2Config.nMagTammingLevel = 40;
            M2Config.nUpgradeWeaponDCRate = 110;
            M2Config.nUpgradeWeaponMaxPoint = 25;
            M2Config.dwUPgradeWeaponGetBackTime = 7200000;
            M2Config.boWeaponUpgradeFailNotDelete = true;
            M2Config.BoneFammArray[0] = (19, "变异骷髅", 19, 1);
            M2Config.sBoneFamm = "变异骷髅";

            _form.Open(showModal: false);

            Assert.Equal(120, (int)_form.EditSwordLongPowerRate.Value);
            Assert.True(_form.CheckBoxLimitSwordLong.Checked);
            Assert.Equal(3, (int)_form.seFireBoomRage.Value);
            Assert.Equal(2, (int)_form.seSnowWindRange.Value);
            Assert.Equal(4, (int)_form.seElecBlizzardRange.Value);
            Assert.Equal(9, (int)_form.EditMagicAttackRage.Value);
            Assert.Equal(15, (int)_form.EditAmyOunsulPoint.Value);
            Assert.Equal(60, (int)_form.seMagTurnUndeadLevel.Value);
            Assert.Equal(40, (int)_form.EditMagTammingLevel.Value);
            Assert.Equal("变异骷髅", _form.EditBoneFammName.Text);
            Assert.Equal("19", _form.GridBoneFamm.Rows[0].Cells[0].Value?.ToString());
            // 升级武器页
            Assert.Equal(110, (int)_form.ScrollBarUpgradeWeaponDCRate.Value);
            Assert.Equal("110", _form.EditUpgradeWeaponDCRate.Text);
            Assert.Equal(25, (int)_form.EditUpgradeWeaponMaxPoint.Value);
            Assert.Equal(7200, (int)_form.EditUPgradeWeaponGetBackTime.Value); // div 1000
            Assert.True(_form.CheckBoxWeaponUpgradeFailNotDelete.Checked);
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void SkillHandlers_WriteConfig_GatedByBoOpened()
    {
        StaRunner.New(() =>
        {
            _form.EditMagicAttackRage.Value = 16; // 未 Open：不写入（保持 MagicConst 默认 12）
            Assert.Equal(12, (int)M2Config.nMagicAttackRage);

            _form.Open(showModal: false);
            _form.EditMagicAttackRage.Value = 12;
            _form.EditBoneFammCount.Value = 2;
            _form.EditBoneFammCountChange(_form);
            _form.EditDogzCount.Value = 3;
            _form.EditDogzCountChange(_form);
            _form.CheckBoxLimitSwordLong.Checked = true;
            _form.CheckBoxLimitSwordLongClick(_form);
            _form.EditSwordLongPowerRate.Value = 150;
            _form.EditSwordLongPowerRateChange(_form);
            _form.EditBoneFammName.Text = "大骷髅";
            _form.EditBoneFammNameChange(_form);
            _form.EditDogzName.Text = "大神兽";
            _form.EditDogzNameChange(_form);
            _form.seFireBoomRage.Value = 4;
            _form.seFireBoomRageChange(_form);
            _form.seSnowWindRange.Value = 5;
            _form.seSnowWindRangeChange(_form);
            _form.seElecBlizzardRange.Value = 6;
            _form.seElecBlizzardRangeChange(_form);
            _form.seMagTurnUndeadLevel.Value = 80;
            _form.seMagTurnUndeadLevelChange(_form);
            _form.EditAmyOunsulPoint.Value = 30;
            _form.EdiAmyOunsulPointChange(_form);
            _form.CheckBoxFireCrossInSafeZone.Checked = true;
            _form.CheckBoxFireCrossInSafeZoneClick(_form);
            _form.chkSkill41MbAttackPlayObject.Checked = true;
            _form.chkSkill41MbAttackPlayObjectClick(_form);

            Assert.Equal(12, (int)M2Config.nMagicAttackRage);
            Assert.Equal(2, (int)M2Config.nBoneFammCount);
            Assert.Equal(3, (int)M2Config.nDogzCount);
            Assert.True(M2Config.boLimitSwordLong);
            Assert.Equal(150, (int)M2Config.nSwordLongPowerRate);
            Assert.Equal("大骷髅", M2Config.sBoneFamm);
            Assert.Equal("大神兽", M2Config.sDogz);
            Assert.Equal(4, (int)M2Config.nFireBoomRage);
            Assert.Equal(5, (int)M2Config.nSnowWindRange);
            Assert.Equal(6, (int)M2Config.nElecBlizzardRange);
            Assert.Equal(80, (int)M2Config.nMagTurnUndeadLevel);
            Assert.Equal(30, (int)M2Config.nAmyOunsulPoint);
            Assert.True(M2Config.boDisableInSafeZoneFireCross);
            Assert.True(M2Config.boSkill41MbAttackPlayObject);
            Assert.True(_form.IsModValued);
        });
    }

    [Fact]
    public void GridBoneFamm_EditWritesArray()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.GridBoneFamm.Rows[0].Cells[0].Value = "22";
            _form.GridBoneFamm.Rows[0].Cells[1].Value = "变异骷髅";
            _form.GridBoneFamm.Rows[0].Cells[2].Value = "2";
            _form.GridBoneFamm.Rows[0].Cells[3].Value = "25";
            _form.GridBoneFammSetEditText(_form, 0);
            Assert.Equal(22, (int)M2Config.BoneFammArray[0].nHumLevel);
            Assert.Equal("变异骷髅", M2Config.BoneFammArray[0].sMonName);
            Assert.Equal(2, (int)M2Config.BoneFammArray[0].nCount);
            Assert.Equal(25, (int)M2Config.BoneFammArray[0].nLevel);
            Assert.True(_form.IsModValued);
        });
    }

    [Fact]
    public void ButtonSkillSave_MonNameValidation_ThenWrite()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditBoneFammCount.Value = 2;
            _form.EditBoneFammCountChange(_form);
            _form.GetMonRaceHandler = name => name == "白骨精" ? 80 : 0; // 骷髅有效，其余无效
            _form.ButtonSkillSaveClick(_form);
            Assert.StartsWith("怪物名称设置错误！", M2Forms.LastMessage);
            Assert.True(_form.ButtonSkillSave.Enabled); // Exit 未落盘

            _form.GetMonRaceHandler = _ => 80; // 全部有效
            _form.ButtonSkillSaveClick(_form);
            Assert.Equal(2, M2ShareState.ConfigIni.ReadInteger("Setup", "BoneFammCount", -1));
            Assert.Equal("白骨精", M2ShareState.ConfigIni.ReadString("Names", "BoneFamm", ""));
            Assert.Equal("神兽", M2ShareState.ConfigIni.ReadString("Names", "Dogz", ""));
            Assert.Equal("圣兽", M2ShareState.ConfigIni.ReadString("Names", "BigDogz", ""));
            Assert.Equal(100, M2ShareState.ConfigIni.ReadInteger("Setup", "FireBoomRagePowerRate", -1));
            Assert.Equal(1, M2ShareState.ConfigIni.ReadInteger("Setup", "SnowwindWaitTime", -1));
            Assert.False(_form.ButtonSkillSave.Enabled);
        });
    }

    [Fact]
    public void ButtonSkillSave_ArrayRows_WriteSetupAndNames()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.BoneFammArray[0] = (19, "变异骷髅", 19, 1);
            M2Config.DogzArray[0] = (25, "神兽", 30, 2);
            _form.ButtonSkillSaveClick(_form);
            Assert.Equal(19, M2ShareState.ConfigIni.ReadInteger("Setup", "BoneFammHumLevel0", -1));
            Assert.Equal("变异骷髅", M2ShareState.ConfigIni.ReadString("Names", "BoneFamm0", ""));
            Assert.Equal(1, M2ShareState.ConfigIni.ReadInteger("Setup", "BoneFammCount0", -1));
            Assert.Equal(19, M2ShareState.ConfigIni.ReadInteger("Setup", "BoneFammLevel0", -1));
            Assert.Equal(25, M2ShareState.ConfigIni.ReadInteger("Setup", "DogzHumLevel0", -1));
            Assert.Equal(30, M2ShareState.ConfigIni.ReadInteger("Setup", "DogzLevel0", -1));
            Assert.Equal(0, M2ShareState.ConfigIni.ReadInteger("Setup", "DogzHumLevel1", -1));
        });
    }

    [Fact]
    public void UpgradeWeaponScrolls_WriteConfig_AndEditMirror()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.ScrollBarUpgradeWeaponDCTwoPointRate.Value = 44;
            _form.ScrollBarUpgradeWeaponDCTwoPointRateChange(_form);
            Assert.Equal(44, (int)M2Config.nUpgradeWeaponDCTwoPointRate);
            Assert.Equal("44", _form.EditUpgradeWeaponDCTwoPointRate.Text);
            _form.ScrollBarUpgradeWeaponMCThreePointRate.Value = 250;
            _form.ScrollBarUpgradeWeaponMCThreePointRateChange(_form);
            Assert.Equal(250, (int)M2Config.nUpgradeWeaponMCThreePointRate);
            _form.ScrollBarUpgradeWeaponSCRate.Value = 90;
            _form.ScrollBarUpgradeWeaponSCRateChange(_form);
            Assert.Equal(90, (int)M2Config.nUpgradeWeaponSCRate);
            Assert.True(_form.ButtonUpgradeWeaponSave.Enabled);
        });
    }

    [Fact]
    public void ButtonUpgradeWeaponSave_Writes15Keys()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.nUpgradeWeaponMaxPoint = 33;
            M2Config.nUpgradeWeaponPrice = 20000;
            M2Config.boWeaponUpgradeFailNotDelete = true;
            _form.ButtonUpgradeWeaponSaveClick(_form);
            Assert.Equal(33, M2ShareState.ConfigIni.ReadInteger("Setup", "UpgradeWeaponMaxPoint", -1));
            Assert.Equal(20000, M2ShareState.ConfigIni.ReadInteger("Setup", "UpgradeWeaponPrice", -1));
            Assert.Equal(8, M2ShareState.ConfigIni.ReadInteger("Setup", "ClearExpireUpgradeWeaponDays", -1));
            Assert.Equal(3600000, M2ShareState.ConfigIni.ReadInteger("Setup", "UPgradeWeaponGetBackTime", -1));
            Assert.Equal(100, M2ShareState.ConfigIni.ReadInteger("Setup", "UpgradeWeaponDCRate", -1));
            Assert.Equal(200, M2ShareState.ConfigIni.ReadInteger("Setup", "UpgradeWeaponMCThreePointRate", -1));
            Assert.Equal(100, M2ShareState.ConfigIni.ReadInteger("Setup", "UpgradeWeaponSCRate", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "WeaponUpgradeFailNotDelete", false));
            Assert.False(_form.ButtonUpgradeWeaponSave.Enabled);
        });
    }

    [Fact]
    public void ButtonUpgradeWeaponDefaulf_IDNO_Then_IDYES()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.nUpgradeWeaponMaxPoint = 99;
            M2Config.nUpgradeWeaponDCRate = 300;
            M2Forms.NextAnswer = M2Forms.IDNO;
            _form.ButtonUpgradeWeaponDefaulfClick(_form);
            Assert.Equal(99, (int)M2Config.nUpgradeWeaponMaxPoint);

            M2Forms.NextAnswer = M2Forms.IDYES;
            _form.ButtonUpgradeWeaponDefaulfClick(_form);
            Assert.Equal(20, (int)M2Config.nUpgradeWeaponMaxPoint);
            Assert.Equal(10000, (int)M2Config.nUpgradeWeaponPrice);
            Assert.Equal(8, (int)M2Config.nClearExpireUpgradeWeaponDays);
            Assert.Equal(3600000, (int)M2Config.dwUPgradeWeaponGetBackTime);
            Assert.Equal(100, (int)M2Config.nUpgradeWeaponDCRate);
            Assert.Equal(30, (int)M2Config.nUpgradeWeaponDCTwoPointRate);
            Assert.Equal(200, (int)M2Config.nUpgradeWeaponDCThreePointRate);
            Assert.Equal(100, (int)M2Config.nUpgradeWeaponSCRate);
            Assert.False(M2Config.boWeaponUpgradeFailNotDelete);
            Assert.Equal(100, (int)_form.ScrollBarUpgradeWeaponDCRate.Value); // RefUpgradeWeapon 回显
            Assert.True(_form.IsModValued);
        });
    }
}

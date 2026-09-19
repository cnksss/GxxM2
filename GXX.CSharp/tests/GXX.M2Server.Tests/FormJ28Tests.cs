using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J28：FunctionConfig.pas 巨片第五片（ReNewLevel 转生 + MonUpgrade 宝宝升级页）1:1 测试。</summary>
public sealed class FunctionConfigReNewMonUpgradeTests : IDisposable
{
    private readonly string _dir;
    private readonly FunctionConfigForm _form;

    public FunctionConfigReNewMonUpgradeTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j28_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetFunctionDefaults();
        M2Config.ResetFunctionSkillDefaults();
        M2Config.ResetFunctionMineDefaults();
        M2Config.ResetFunctionReNewMonDefaults();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() =>
        {
            var f = new FunctionConfigForm();
            f.GetMonRaceHandler = _ => 5;
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
    public void Open_RefReNewLevel_And_MonUpgrade()
    {
        StaRunner.New(() =>
        {
            M2Config.ReNewNameColor[0] = 1;
            M2Config.dwReNewNameColorTime = 4000;
            M2Config.boReNewChangeColor = false;
            M2Config.boReNewLevelClearExp = true;
            M2Config.SlaveColor[3] = 0x11;
            M2Config.MonUpLvNeedKillCount[4] = 250;
            M2Config.nMonUpLvNeedKillBase = 150;
            M2Config.nMonUpLvRate = 20;
            M2Config.boMasterDieMutiny = true;
            M2Config.nMasterDieMutinyRate = 7;
            M2Config.dwBBMonAutoChangeColorTime = 5000;

            _form.Open(showModal: false);

            Assert.Equal(1, (int)_form.EditReNewNameColorCells[0].Value);
            Assert.Equal(4, (int)_form.EditReNewNameColorTime.Value); // div 1000
            Assert.False(_form.CheckBoxReNewChangeColor.Checked);
            Assert.True(_form.CheckBoxReNewLevelClearExp.Checked);
            Assert.Equal(0x11, (int)_form.EditMonUpgradeColorCells[3].Value);
            Assert.Equal(250, (int)_form.EditMonUpgradeKillCountCells[4].Value);
            Assert.Equal(150, (int)_form.EditMonUpLvNeedKillBase.Value);
            Assert.Equal(20, (int)_form.EditMonUpLvRate.Value);
            Assert.True(_form.CheckBoxMasterDieMutiny.Checked);
            Assert.True(_form.EditMasterDieMutinyRate.Enabled); // Mutiny 级联
            Assert.Equal(7, (int)_form.EditMasterDieMutinyRate.Value);
            Assert.Equal(5, (int)_form.EditBBMonAutoChangeColorTime.Value); // div 1000
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void ReNewHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditReNewNameColorCells[0].Value = 200;
            _form.EditReNewNameColor1Change(_form);
            _form.EditReNewNameColorCells[9].Value = 99;
            _form.EditReNewNameColor10Change(_form);
            _form.EditReNewNameColorTime.Value = 6;
            _form.EditReNewNameColorTimeChange(_form);
            _form.CheckBoxReNewChangeColor.Checked = true;
            _form.CheckBoxReNewChangeColorClick(_form);
            Assert.Equal(200, (int)M2Config.ReNewNameColor[0]);
            Assert.Equal(99, (int)M2Config.ReNewNameColor[9]);
            Assert.Equal(6000, (int)M2Config.dwReNewNameColorTime);
            Assert.True(M2Config.boReNewChangeColor);
        });
    }

    [Fact]
    public void ButtonReNewLevelSave_WritesArrayAndKeys()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.ReNewNameColor[3] = 0x9A;
            M2Config.dwReNewNameColorTime = 5000;
            _form.ButtonReNewLevelSaveClick(_form);
            Assert.Equal(0xFF, M2ShareState.ConfigIni.ReadInteger("Setup", "ReNewNameColor0", -1));
            Assert.Equal(0x9A, M2ShareState.ConfigIni.ReadInteger("Setup", "ReNewNameColor3", -1));
            Assert.Equal(5000, M2ShareState.ConfigIni.ReadInteger("Setup", "ReNewNameColorTime", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "ReNewChangeColor", false));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "ReNewLevelClearExp", false));
            Assert.False(_form.ButtonReNewLevelSave.Enabled);
        });
    }

    [Fact]
    public void MonUpgradeHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditMonUpgradeColorCells[0].Value = 3;
            _form.EditMonUpgradeColor0Change(_form);
            _form.EditMonUpgradeKillCountCells[0].Value = 77;
            _form.EditMonUpgradeKillCount1Change(_form);
            _form.EditMonUpLvNeedKillBase.Value = 300;
            _form.EditMonUpLvNeedKillBaseChange(_form);
            Assert.Equal(3, (int)M2Config.SlaveColor[0]);
            Assert.Equal(77, (int)M2Config.MonUpLvNeedKillCount[0]);
            Assert.Equal(300, (int)M2Config.nMonUpLvNeedKillBase);

            _form.CheckBoxMasterDieMutiny.Checked = false;
            _form.CheckBoxMasterDieMutinyClick(_form);
            Assert.False(M2Config.boMasterDieMutiny);
            Assert.False(_form.EditMasterDieMutinyRate.Enabled);
        });
    }

    [Fact]
    public void ButtonMonUpgradeSave_WritesKeys_WithDupKeyQuirk()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.nMasterDieMutinyRate = 8;
            M2Config.nMasterDieMutinyPower = 15;
            M2Config.nMasterDieMutinySpeed = 6;
            _form.ButtonMonUpgradeSaveClick(_form);
            Assert.Equal(100, M2ShareState.ConfigIni.ReadInteger("Setup", "MonUpLvNeedKillBase", -1));
            Assert.Equal(16, M2ShareState.ConfigIni.ReadInteger("Setup", "MonUpLvRate", -1));
            Assert.Equal(0, M2ShareState.ConfigIni.ReadInteger("Setup", "MonUpLvNeedKillCount0", -1));
            Assert.Equal(1200, M2ShareState.ConfigIni.ReadInteger("Setup", "MonUpLvNeedKillCount7", -1));
            Assert.Equal(255, M2ShareState.ConfigIni.ReadInteger("Setup", "SlaveColor0", -1));
            Assert.Equal(249, M2ShareState.ConfigIni.ReadInteger("Setup", "SlaveColor9", -1));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "MasterDieMutiny", true));
            // Delphi 原文重复写 MasterDieMutinyPower 键名但第二次取 Speed 值
            Assert.Equal(6, M2ShareState.ConfigIni.ReadInteger("Setup", "MasterDieMutinyPower", -1));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "SlaveNotAttackHuman", true));
            Assert.Equal(60, M2ShareState.ConfigIni.ReadInteger("Setup", "Slave9HP", -1));
            Assert.False(_form.ButtonMonUpgradeSave.Enabled);
        });
    }
}

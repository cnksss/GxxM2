using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J29：FunctionConfig.pas 巨片第六片（SpiritMutiny 叛变/MonSayMsg 怪物发言/WeaponMakeLuck 武器炼 luck 页）1:1 测试。</summary>
public sealed class FunctionConfigMutinyMsgLuckTests : IDisposable
{
    private readonly string _dir;
    private readonly FunctionConfigForm _form;

    public FunctionConfigMutinyMsgLuckTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j29_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetFunctionDefaults();
        M2Config.ResetFunctionSkillDefaults();
        M2Config.ResetFunctionMineDefaults();
        M2Config.ResetFunctionReNewMonDefaults();
        M2Config.ResetFunctionMutinyMsgDefaults();
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
    public void Open_RefMutinyMsgLuck()
    {
        StaRunner.New(() =>
        {
            M2Config.boSpiritMutiny = true;
            M2Config.dwSpiritMutinyTime = 60 * 60 * 1000;
            M2Config.nSpiritPowerRate = 4;
            M2Config.boMonSayMsg = true;
            M2Config.nWeaponMakeUnLuckRate = 25;

            _form.Open(showModal: false);

            Assert.True(_form.CheckBoxSpiritMutiny.Checked);
            Assert.True(_form.EditSpiritMutinyTime.Enabled); // Mutiny 级联
            Assert.Equal(60, (int)_form.EditSpiritMutinyTime.Value); // div (60*1000)
            Assert.Equal(4, (int)_form.EditSpiritPowerRate.Value);
            Assert.True(_form.CheckBoxMonSayMsg.Checked);
            Assert.Equal(25, (int)_form.ScrollBarWeaponMakeUnLuckRate.Value);
            Assert.Equal("25", _form.EditWeaponMakeUnLuckRate.Text);
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void SpiritMutinyHandlers_WriteConfig_Cascade()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxSpiritMutiny.Checked = false;
            _form.CheckBoxSpiritMutinyClick(_form);
            Assert.False(M2Config.boSpiritMutiny);
            Assert.False(_form.EditSpiritMutinyTime.Enabled);
            Assert.False(_form.EditSpiritPowerRate.Enabled); // 注：取消时时间与威力均禁用

            _form.CheckBoxSpiritMutiny.Checked = true;
            _form.CheckBoxSpiritMutinyClick(_form);
            Assert.True(_form.EditSpiritMutinyTime.Enabled);

            _form.EditSpiritMutinyTime.Value = 45;
            _form.EditSpiritMutinyTimeChange(_form);
            Assert.Equal(2700000, (int)M2Config.dwSpiritMutinyTime); // ×60×1000
            _form.EditSpiritPowerRate.Value = 6;
            _form.EditSpiritPowerRateChange(_form);
            Assert.Equal(6, (int)M2Config.nSpiritPowerRate);

            _form.ButtonSpiritMutinySaveClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "SpiritMutiny", false));
            Assert.Equal(2700000, M2ShareState.ConfigIni.ReadInteger("Setup", "SpiritMutinyTime", -1));
            Assert.Equal(6, M2ShareState.ConfigIni.ReadInteger("Setup", "SpiritPowerRate", -1));
            Assert.False(_form.ButtonSpiritMutinySave.Enabled);
        });
    }

    [Fact]
    public void MonSayMsgHandler_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxMonSayMsg.Checked = true;
            _form.CheckBoxMonSayMsgClick(_form);
            Assert.True(M2Config.boMonSayMsg);

            _form.ButtonMonSayMsgSaveClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "MonSayMsg", false));
            Assert.False(_form.ButtonMonSayMsgSave.Enabled);
        });
    }

    [Fact]
    public void WeaponMakeLuckScrolls_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.ScrollBarWeaponMakeLuckPoint1.Value = 2;
            _form.ScrollBarWeaponMakeLuckPoint1Change(_form);
            Assert.Equal(2, (int)M2Config.nWeaponMakeLuckPoint1);
            _form.ScrollBarWeaponMakeLuckPoint3Rate.Value = 55;
            _form.ScrollBarWeaponMakeLuckPoint3RateChange(_form);
            Assert.Equal(55, (int)M2Config.nWeaponMakeLuckPoint3Rate);

            _form.ButtonWeaponMakeLuckSaveClick(_form);
            Assert.Equal(20, M2ShareState.ConfigIni.ReadInteger("Setup", "WeaponMakeUnLuckRate", -1));
            Assert.Equal(2, M2ShareState.ConfigIni.ReadInteger("Setup", "WeaponMakeLuckPoint1", -1));
            Assert.Equal(3, M2ShareState.ConfigIni.ReadInteger("Setup", "WeaponMakeLuckPoint2", -1));
            Assert.Equal(7, M2ShareState.ConfigIni.ReadInteger("Setup", "WeaponMakeLuckPoint3", -1));
            Assert.Equal(6, M2ShareState.ConfigIni.ReadInteger("Setup", "WeaponMakeLuckPoint2Rate", -1));
            Assert.Equal(55, M2ShareState.ConfigIni.ReadInteger("Setup", "WeaponMakeLuckPoint3Rate", -1));
            Assert.False(_form.ButtonWeaponMakeLuckSave.Enabled);
        });
    }

    [Fact]
    public void ButtonWeaponMakeLuckDefaulf_IDYES_Restores()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.nWeaponMakeUnLuckRate = 99;
            M2Forms.NextAnswer = M2Forms.IDNO;
            _form.ButtonWeaponMakeLuckDefaulfClick(_form);
            Assert.Equal(99, (int)M2Config.nWeaponMakeUnLuckRate);

            M2Forms.NextAnswer = M2Forms.IDYES;
            _form.ButtonWeaponMakeLuckDefaulfClick(_form);
            Assert.Equal(20, (int)M2Config.nWeaponMakeUnLuckRate);
            Assert.Equal(1, (int)M2Config.nWeaponMakeLuckPoint1);
            Assert.Equal(3, (int)M2Config.nWeaponMakeLuckPoint2);
            Assert.Equal(7, (int)M2Config.nWeaponMakeLuckPoint3);
            Assert.Equal(6, (int)M2Config.nWeaponMakeLuckPoint2Rate);
            Assert.Equal(40, (int)M2Config.nWeaponMakeLuckPoint3Rate);
            Assert.Equal("20", _form.EditWeaponMakeUnLuckRate.Text); // 回显
        });
    }
}

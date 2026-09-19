using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J31：FunctionConfig.pas 巨片收尾（HeroOption/Other/BonusAbilof/Other3 页）1:1 测试。</summary>
public sealed class FunctionConfigFinalPagesTests : IDisposable
{
    private readonly string _dir;
    private readonly FunctionConfigForm _form;

    public FunctionConfigFinalPagesTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j31_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetFunctionDefaults();
        M2Config.ResetFunctionSkillDefaults();
        M2Config.ResetFunctionMineDefaults();
        M2Config.ResetFunctionReNewMonDefaults();
        M2Config.ResetFunctionMutinyMsgDefaults();
        M2Config.ResetFunctionShopDefaults();
        M2Config.ResetFunctionFinalDefaults();
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
    public void Open_RefFinalPages()
    {
        StaRunner.New(() =>
        {
            M2Config.boHeroGetAllExp = true;
            M2Config.nHeroKillMonExpRate = 60;
            M2Config.nLimitExpLevel = 500;
            M2Config.boAllowCopySelf = true;
            M2Config.nRecallHeroTime = 90;
            M2Config.boDeleteItemDuraZero = true;
            M2Config.sMysteriousManName = "神秘老人";
            M2Config.boOpenSelfShop = true;
            M2Config.dwRevivalTime = 90000;
            M2Config.nStarBaseNum = 12;

            _form.Open(showModal: false);

            Assert.True(_form.CheckBoxHeroGetAllExp.Checked);
            Assert.Equal(60, (int)_form.EditHeroKillMonExpRate.Value);
            Assert.True(_form.CheckBoxAllowCopySelf.Checked);
            Assert.Equal(500, (int)_form.EditLimitExpLevel.Value);
            Assert.Equal(90, (int)_form.EditRecallHeroTime.Value);
            Assert.True(_form.CheckBoxDeleteItemDuraZero.Checked);
            Assert.Equal("神秘老人", _form.EditMysteriousManName.Text);
            Assert.True(_form.CheckBoxOpenSelfShop.Checked);
            Assert.Equal(90, (int)_form.EditRevivalTime.Value); // div 1000
            Assert.Equal(12, (int)_form.EditStarBaseNum.Value);
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void HeroOptionHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxHeroGetAllExp.Checked = false;
            _form.CheckBoxHeroGetAllExpClick(_form);
            _form.EditHeroKillMonExpRate.Value = 70;
            _form.EditHeroKillMonExpRateChange(_form);
            _form.CheckBoxAllowCopySelf.Checked = true;
            _form.CheckBoxAllowCopySelfClick(_form);
            _form.EditRecallHeroTime.Value = 120;
            _form.EditRecallHeroTimeChange(_form);
            _form.EditHeroWarrorAttackTime.Value = 800;
            _form.EditHeroWarrorAttackTimeChange(_form);
            Assert.False(M2Config.boHeroGetAllExp);
            Assert.Equal(70, (int)M2Config.nHeroKillMonExpRate);
            Assert.True(M2Config.boAllowCopySelf);
            Assert.Equal(120, (int)M2Config.nRecallHeroTime);
            Assert.Equal(800, (int)M2Config.dwHeroWarrorAttackTime);
        });
    }

    [Fact]
    public void ButtonHeroOptionSave_ZeroValidation_ThenWrite()
    {
        StaRunner.New(() =>
        {
            Array.Copy(M2Config.OldHeroNeedExps, M2Config.dwHeroNeedExps, 1001); // Open 前预置有效表
            _form.Open(showModal: false);
            _form.CheckBoxHeroGetAllExp.Checked = true; // 先改动触发 ModValue（保存钮可用）
            _form.CheckBoxHeroGetAllExpClick(_form);
            _form.GridHeroExp.Rows[9].Cells[1].Value = "0"; // 等级 10 非法
            _form.ButtonHeroOptionSaveClick(_form);
            Assert.Equal("等级 10 升级经验设置错误！！！", M2Forms.LastMessage);
            Assert.True(_form.ButtonHeroOptionSave.Enabled);

            _form.GridHeroExp.Rows[9].Cells[1].Value = "12000";
            M2Config.boAllowCopySelf = true;
            _form.ButtonHeroOptionSaveClick(_form);
            Assert.Equal(12000, M2ShareState.ExpConfigIni.ReadInteger("HeroExp", "Level10", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "HeroGetAllExp", false) == M2Config.boHeroGetAllExp);
            // Delphi 原文：HeroUseBagItem/MonUseBagItem 两键均写 boAllowCopySelf
            Assert.Equal(M2Config.boAllowCopySelf ? 1 : 0, M2ShareState.ConfigIni.ReadInteger("Setup", "HeroUseBagItem", -1) is int v && (v == 1 || v == 0) ? v : -1);
            // 原文瑕疵：英雄攻击间隔写入人物键名 WarrorAttackTime
            Assert.Equal(700, M2ShareState.ConfigIni.ReadInteger("Setup", "WarrorAttackTime", -1));
            Assert.Equal(600, M2ShareState.ConfigIni.ReadInteger("Setup", "WizardWalkTime", -1));
            Assert.Equal(147, M2ShareState.ConfigIni.ReadInteger("Setup", "HeroNameColor", -1));
            Assert.False(_form.ButtonHeroOptionSave.Enabled);
        });
    }

    [Fact]
    public void OtherHandlers_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxDeleteItemDuraZero.Checked = true;
            _form.CheckBoxDeleteItemDuraZeroClick(_form);
            _form.EditMysteriousManName.Text = "神秘老人";
            _form.EditMysteriousManNameChange(_form);
            _form.EditMaxLuckMaxPower.Value = 12;
            _form.EditMaxLuckMaxPowerChange(_form);
            Assert.True(M2Config.boDeleteItemDuraZero);
            Assert.Equal("神秘老人", M2Config.sMysteriousManName);
            Assert.Equal(12, (int)M2Config.nMaxLuckMaxPower);

            _form.ButtonOtherClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "DeleteItemDuraZero", false));
            Assert.Equal("神秘老人", M2ShareState.ConfigIni.ReadString("Setup", "MysteriousManName", ""));
            Assert.Equal(12, M2ShareState.ConfigIni.ReadInteger("Setup", "MaxLuckMaxPower", -1));
            Assert.Equal(3, M2ShareState.ConfigIni.ReadInteger("Setup", "QueryBagItemsTime", -1));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "GroupUseOldMode", true));
            Assert.False(_form.ButtonOther.Enabled);
        });
    }

    [Fact]
    public void BonusAbilofHandlers_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditBonusAbilofWarrDC.Value = 8;
            _form.EditBonusAbilofWarrDCChange(_form);
            Assert.Equal(8, M2Config.BonusAbilofWarr.DC);
            _form.EditBonusAbilofTaosSpeed.Value = 4;
            _form.EditBonusAbilofTaosSpeedChange(_form);
            Assert.Equal(4, M2Config.BonusAbilofTaos.Speed);

            _form.ButtonBonusAbilofSaveClick(_form);
            Assert.Equal(8, M2ShareState.ConfigIni.ReadInteger("Setup", "BonusAbilofWarrDC", -1));
            Assert.Equal(4, M2ShareState.ConfigIni.ReadInteger("Setup", "BonusAbilofTaosSpeed", -1));
            Assert.Equal(0, M2ShareState.ConfigIni.ReadInteger("Setup", "BonusAbilofWizardHP", -1));
            Assert.False(_form.ButtonBonusAbilofSave.Enabled);
        });
    }

    [Fact]
    public void Other3Handlers_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxRevivalTouch.Checked = true;
            _form.CheckBoxRevivalTouchClick(_form);
            _form.EditRevivalTime.Value = 120;
            _form.EditRevivalTimeChange(_form);
            _form.EditStarBaseNum.Value = 15;
            _form.EditStarBaseNumChange(_form);
            Assert.True(M2Config.boRevivalTouch);
            Assert.Equal(120000, (int)M2Config.dwRevivalTime);
            Assert.Equal(15, (int)M2Config.nStarBaseNum);

            _form.ButtonOther3Click(_form);
            Assert.Equal(120000, M2ShareState.ConfigIni.ReadInteger("Setup", "RevivalTime", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "RevivalTouch", false));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "SaveRevivalTime", false));
            Assert.Equal("首饰盒", M2ShareState.ConfigIni.ReadString("Setup", "JewelryBoxHint", ""));
            Assert.Equal(15, M2ShareState.ConfigIni.ReadInteger("Setup", "StarBaseNum", -1));
            Assert.Equal(30, M2ShareState.ConfigIni.ReadInteger("Setup", "StarLineMaxCount", -1));
            Assert.False(_form.ButtonOther3.Enabled);
        });
    }
}

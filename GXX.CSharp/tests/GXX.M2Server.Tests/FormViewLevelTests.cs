using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J5正片：ViewLevel.pas TfrmViewLevel 1:1 转换测试（等级属性查看器）。</summary>
public sealed class FormViewLevelTests : IDisposable
{
    private readonly string _dir;

    public FormViewLevelTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j5_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2ShareAbilConfig.ResetDefaults();
        M2Config.ResetServerValueDefaults();
    }

    public void Dispose()
    {
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [StaFact]
    public void Open_WarriorLevel1_ShowsSystemDefaults()
    {
        StaRunner.New(() =>
        {
            using var form = new ViewLevelForm();
            form.Open(showModal: false);
            // 战士 1 级系统公式（btMaxLevel=0 / 默认参数）
            Assert.Equal("0/19", form.GridHumanInfo[1, 6].Value);   // HP 初始 0（Delphi 1:1，仅上限 clamp）   // HP/MaxHP
            Assert.Equal("0/15", form.GridHumanInfo[1, 7].Value);   // MP/MaxMP
            Assert.Equal("0/1", form.GridHumanInfo[1, 3].Value);     // DC1/DC2
            Assert.Equal("0/50", form.GridHumanInfo[1, 8].Value);      // Weight/MaxWeight
            Assert.True(form.rbDef.Checked);
        });
    }

    [StaFact]
    public void JobAndLevelChange_Taos7()
    {
        StaRunner.New(() =>
        {
            using var form = new ViewLevelForm();
            form.Open(showModal: false);
            form.cbbDefJob.SelectedIndex = 2; // 道士 → cbbDefJobChange
            form.seDefLevel.Value = 7;        // → seDefLevelChange → RefView

            Assert.Equal("0/40", form.GridHumanInfo[1, 6].Value);   // 道士 7 级 MaxHP = 40
            Assert.Equal("0/26", form.GridHumanInfo[1, 7].Value);   // MaxMP = 26
            Assert.Equal("0/1", form.GridHumanInfo[1, 3].Value);     // DC: n=1
            Assert.Equal("0/2", form.GridHumanInfo[1, 2].Value);       // MAC1/MAC2 = 0/2
            Assert.Equal(2, form.PlayObject.m_btJob);
            Assert.Equal(2, form.HeroObject.m_btJob); // 职业同步到英雄
        });
    }

    [StaFact]
    public void UserTypeToggle_ShowsHeroObject()
    {
        StaRunner.New(() =>
        {
            using var form = new ViewLevelForm();
            form.Open(showModal: false);
            form.HeroObject.m_btJob = 1; // 法师
            form.HeroObject.m_wAbil.Level = 5;
            form.cbbDefUserType.SelectedIndex = 1; // 英雄 → RefView
            form.RecalcHuman();
            form.RefView();
            // 法师 5 级 MaxHP = 25（ReCalcHuman 先重算回战士公式？不——Hero job=1 保持）
            Assert.Equal("0/25", form.GridHumanInfo[1, 6].Value);
        });
    }

    [StaFact]
    public void DefLevelClamp_ToMinimum1()
    {
        StaRunner.New(() =>
        {
            using var form = new ViewLevelForm();
            form.Open(showModal: false);
            form.seDefLevel.Value = 0;
            form.seDefLevelChange(form);
            Assert.Equal(1u, form.PlayObject.m_wAbil.Level);
        });
    }

    [StaFact]
    public void DefLevelsList_ItemIndexMapsToLevel()
    {
        StaRunner.New(() =>
        {
            using var form = new ViewLevelForm();
            form.Open(showModal: false);
            form.lstDefLevelsClick(9); // ItemIndex 9 → 等级 10
            Assert.Equal(10u, form.PlayObject.m_wAbil.Level);
        });
    }

    [StaFact]
    public void SaveClick_WritesBaseAbilIniFormat()
    {
        M2ShareAbilConfig.g_BaseAbilConfig.UseDefault = false;
        M2ShareAbilConfig.g_BaseAbilConfig.HumAbil[0].Base[0] = new TBaseAbilInfo { DC1 = 1, DC2 = 3, MaxHP = 19 };
        M2ShareAbilConfig.g_BaseAbilConfig.HumAbil[0].Add = new TBaseAbilInfo { DC2 = 1, MaxHP = 20 };

        StaRunner.New(() =>
        {
            using var form = new ViewLevelForm();
            form.Open(showModal: false);
            Environment.CurrentDirectory = _dir;
            form.btnSaveClick(form);

            string content = File.ReadAllText(Path.Combine(_dir, "BaseAbil.txt"), System.Text.Encoding.GetEncoding(936));
            Assert.Contains("[Setup]", content);
            Assert.Contains("UseDefault=0", content);
            Assert.Contains("[Hum_0]", content);
            Assert.Contains("DC1_1=1", content);
            Assert.Contains("MaxHP_1=19", content);
            Assert.Contains("MaxHandWeight_1000=", content);
            Assert.Contains("AddDC2=1", content);
            Assert.Contains("AddMaxHP=20", content);
            Assert.False(form.btnSave.Enabled);
        });
    }

    [StaFact]
    public void RadioButtons_ToggleUseDefault()
    {
        StaRunner.New(() =>
        {
            using var form = new ViewLevelForm();
            form.Open(showModal: false);
            form.rbCustomClick(form);
            Assert.False(M2ShareAbilConfig.g_BaseAbilConfig.UseDefault);
            Assert.True(form.btnSave.Enabled);
            form.rbDefClick(form);
            Assert.True(M2ShareAbilConfig.g_BaseAbilConfig.UseDefault);
        });
    }
}

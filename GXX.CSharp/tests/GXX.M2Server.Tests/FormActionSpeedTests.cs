using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J3：ActionSpeedConfig.pas + GroupItemSkillPowerConfig.pas 1:1 转换测试。
/// </summary>
public sealed class FormActionSpeedTests : IDisposable
{
    private readonly string _dir;
    private readonly ActionSpeedConfigForm _form;

    public FormActionSpeedTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j3a_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetGeneralDefaults();
        M2Config.ResetSpeedDefaults();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() => new ActionSpeedConfigForm());
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private string IniPath => Path.Combine(_dir, "!Setup.txt");

    [StaFact]
    public void Open_LoadsConfigValues()
    {
        M2Config.dwActionIntervalTime = 333;
        M2Config.dwRunMagicIntervalTime = 999;
        M2Config.boControlWalkHit = false;
        _form.Open(showModal: false);
        Assert.Equal(333, (int)_form.EditActionIntervalTime.Value);
        Assert.Equal(999, (int)_form.EditRunMagicIntervalTime.Value);
        Assert.False(_form.CheckBoxControlWalkHit.Checked);
        Assert.False(_form.IsModValued);
        Assert.False(_form.ButtonSave.Enabled);
    }

    [StaFact]
    public void EditChanges_WriteConfigWhenOpened()
    {
        _form.Open(showModal: false);
        _form.EditActionIntervalTime.Value = 555;
        _form.EditActionIntervalTimeChange(_form);
        _form.EditRunLongHitIntervalTime.Value = 444;
        _form.EditRunLongHitIntervalTimeChange(_form);
        _form.EditRunHitIntervalTime.Value = 333;
        _form.EditRunHitIntervalTimeChange(_form);
        _form.EditWalkHitIntervalTime.Value = 222;
        _form.EditWalkHitIntervalTimeChange(_form);
        _form.EditRunMagicIntervalTime.Value = 111;
        _form.EditRunMagicIntervalTimeChange(_form);

        Assert.Equal(555, (int)M2Config.dwActionIntervalTime);
        Assert.Equal(444, (int)M2Config.dwRunLongHitIntervalTime);
        Assert.Equal(333, (int)M2Config.dwRunHitIntervalTime);
        Assert.Equal(222, (int)M2Config.dwWalkHitIntervalTime);
        Assert.Equal(111, (int)M2Config.dwRunMagicIntervalTime);
        Assert.True(_form.ButtonSave.Enabled);
    }

    [StaFact]
    public void Save_WritesIniSetupKeys()
    {
        M2Config.dwActionIntervalTime = 400;
        M2Config.dwRunLongHitIntervalTime = 800;
        M2Config.dwRunHitIntervalTime = 810;
        M2Config.dwWalkHitIntervalTime = 820;
        M2Config.dwRunMagicIntervalTime = 900;
        M2Config.boControlRunMagic = false;
        _form.Open(showModal: false);
        _form.ButtonSaveClick(_form);

        var ini = new TFastIniFile(IniPath);
        Assert.Equal(400, ini.ReadInteger("Setup", "ActionIntervalTime", 0));
        Assert.Equal(800, ini.ReadInteger("Setup", "RunLongHitIntervalTime", 0));
        Assert.Equal(810, ini.ReadInteger("Setup", "RunHitIntervalTime", 0));
        Assert.Equal(820, ini.ReadInteger("Setup", "WalkHitIntervalTime", 0));
        Assert.Equal(900, ini.ReadInteger("Setup", "RunMagicIntervalTime", 0));
        Assert.False(ini.ReadBool("Setup", "ControlRunMagic", true));
        Assert.True(ini.ReadBool("Setup", "ControlActionInterval", false));
        Assert.False(_form.ButtonSave.Enabled); // uModValue
    }

    [StaFact]
    public void Default_ConfirmYes_ResetsDelphiDefaults()
    {
        _form.Open(showModal: false);
        M2Config.dwActionIntervalTime = 100;
        M2Config.boControlActionInterval = false;
        M2Forms.NextAnswer = M2Forms.IDYES;
        _form.ButtonDefaultClick(_form);

        Assert.Equal(400, (int)M2Config.dwActionIntervalTime);
        Assert.Equal(800, (int)M2Config.dwRunLongHitIntervalTime);
        Assert.Equal(800, (int)M2Config.dwRunHitIntervalTime);
        Assert.Equal(800, (int)M2Config.dwWalkHitIntervalTime);
        Assert.Equal(900, (int)M2Config.dwRunMagicIntervalTime);
        Assert.True(M2Config.boControlActionInterval);
        Assert.True(M2Config.boControlRunLongHit);
        Assert.True(M2Config.boControlRunHit);
        Assert.True(M2Config.boControlWalkHit);
        Assert.True(M2Config.boControlRunMagic);
        Assert.Equal(400, (int)_form.EditActionIntervalTime.Value); // RefSpeedConfig 回填
        Assert.True(_form.ButtonSave.Enabled); // ModValue 已标记
    }

    [StaFact]
    public void Default_ConfirmNo_NoChange()
    {
        _form.Open(showModal: false);
        M2Config.dwActionIntervalTime = 123;
        M2Forms.NextAnswer = M2Forms.IDNO;
        _form.ButtonDefaultClick(_form);
        Assert.Equal(123, (int)M2Config.dwActionIntervalTime);
    }

    [StaFact]
    public void ControlActionInterval_CascadesOtherCheckboxes()
    {
        _form.Open(showModal: false);
        _form.CheckBoxControlActionInterval.Checked = true;
        _form.CheckBoxControlRunLongHit.Checked = true;
        _form.CheckBoxControlRunHit.Checked = true;
        _form.CheckBoxControlWalkHit.Checked = true;
        _form.CheckBoxControlRunMagic.Checked = true;
        _form.CheckBoxControlActionIntervalClick(_form);
        // 全开 → 各子项使能
        Assert.True(_form.CheckBoxControlRunLongHit.Enabled);
        Assert.True(_form.EditActionIntervalTime.Enabled);

        _form.CheckBoxControlActionInterval.Checked = false;
        _form.CheckBoxControlActionIntervalClick(_form);
        Assert.False(_form.CheckBoxControlRunLongHit.Enabled);
        Assert.False(_form.CheckBoxControlRunHit.Enabled);
        Assert.False(_form.CheckBoxControlWalkHit.Enabled);
        Assert.False(_form.CheckBoxControlRunMagic.Enabled);
        Assert.False(M2Config.boControlActionInterval);
        Assert.True(_form.ButtonSave.Enabled);
    }

    [StaFact]
    public void SubCheckbox_EnabledGate_KeepsConfigValueWhenParentDisabled()
    {
        _form.Open(showModal: false);
        // 主开关关闭 → 子项禁用；此时点击子项 boStatus=false（Checked AND Enabled 门控）
        _form.CheckBoxControlActionInterval.Checked = false;
        _form.CheckBoxControlActionIntervalClick(_form);
        M2Config.boControlRunLongHit = true; // 预置值
        _form.CheckBoxControlRunLongHit.Checked = false;
        _form.CheckBoxControlRunLongHitClick(_form);
        // boStatus = Checked(false) and Enabled(false) = false → g_Config 写 false
        Assert.False(M2Config.boControlRunLongHit);
        Assert.False(_form.EditRunLongHitIntervalTime.Enabled);
    }

    [StaFact]
    public void Incremeng_TogglesSpinIncrement()
    {
        _form.CheckBoxIncremeng.Checked = true;
        _form.CheckBoxIncremengClick(_form);
        Assert.Equal(1, (int)_form.EditActionIntervalTime.Increment);
        Assert.Equal(1, (int)_form.EditRunHitIntervalTime.Increment);
        _form.CheckBoxIncremeng.Checked = false;
        _form.CheckBoxIncremengClick(_form);
        Assert.Equal(10, (int)_form.EditActionIntervalTime.Increment);
        Assert.Equal(10, (int)_form.EditRunHitIntervalTime.Increment);
    }

    [StaFact]
    public void ButtonClose_Unmodified_ClosesDirectly()
    {
        _form.Open(showModal: false);
        M2Forms.LastMessage = null;
        _form.ButtonCloseClick(_form);
        Assert.Null(M2Forms.LastMessage); // 未修改不弹确认
    }

    [StaFact]
    public void ButtonClose_Modified_ConfirmYes_AllowsClose()
    {
        _form.Open(showModal: false);
        _form.EditActionIntervalTime.Value = 777;
        _form.EditActionIntervalTimeChange(_form);
        Assert.True(_form.IsModValued);
        M2Forms.NextAnswer = M2Forms.IDYES;
        _form.ButtonCloseClick(_form);
        Assert.Equal("设置已被修改是否不保存设置退出？", M2Forms.LastMessage);
    }
}

/// <summary>GroupItemSkillPowerConfig.pas（套装技能威力百分比设置）。</summary>
public sealed class GroupItemSkillPowerTests : IDisposable
{
    private readonly TUserEngine _engine = new();

    public GroupItemSkillPowerTests()
    {
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        ViewList2State.SelGroupItem = new TGroupItemModel { FLD_INDEX = 7 };
        Array.Clear(ViewList2State.SelAttackSkillPercent);
        Array.Clear(ViewList2State.SelDefenseSkillPercent);
    }

    public void Dispose()
    {
        M2Forms.MessageBoxHandler = null;
        ViewList2State.SelGroupItem = null;
    }

    private static void ClearMagics(TUserEngine engine) => engine.MagicDefs.Clear();

    [StaFact]
    public void FormCreate_CaptionHeadersAnd115Rows()
    {
        var form = StaRunner.New(() => new GroupItemSkillPowerForm(_engine));
        try
        {
            Assert.Equal("套装编号7的技能威力百分比设置", form.Text);
            Assert.Equal("技能名称", form.Grid[0, 0].Value);
            Assert.Equal("增加技能伤害百分比", form.Grid[1, 0].Value);
            Assert.Equal("增加技能防御百分比", form.Grid[2, 0].Value);
            Assert.Equal(115, form.Grid.RowCount);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void MagicNamesFilled_HumPriority_ContinuousFallback()
    {
        ClearMagics(_engine);
        _engine.AddMagicDef(new TMagicDef { wMagicId = 1, sMagicName = "火球术" }, TMagicAttr.mtHum);
        _engine.AddMagicDef(new TMagicDef { wMagicId = 60, sMagicName = "连击技" }, TMagicAttr.mtContinuous);

        var form = StaRunner.New(() => new GroupItemSkillPowerForm(_engine));
        try
        {
            Assert.Equal("火球术", form.Grid[0, 1].Value);      // 行号 = 技能 ID
            Assert.Equal("连击技", form.Grid[0, 60].Value);     // mtHum 无 → mtContinuous 兜底
            Assert.Equal("", form.Grid[0, 3].Value);            // 无技能空串
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void InvalidMagicIdsGetSuffix()
    {
        ClearMagics(_engine);
        _engine.AddMagicDef(new TMagicDef { wMagicId = 2, sMagicName = "治愈术" }, TMagicAttr.mtHum);    // 在无效集合
        _engine.AddMagicDef(new TMagicDef { wMagicId = 5, sMagicName = "攻杀剑术" }, TMagicAttr.mtHum);  // 不在
        var form = StaRunner.New(() => new GroupItemSkillPowerForm(_engine));
        try
        {
            Assert.Equal("治愈术[无效]", form.Grid[0, 2].Value);
            Assert.Equal("攻杀剑术", form.Grid[0, 5].Value);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void ButtonClick_WritesBackPercentArrays()
    {
        ClearMagics(_engine);
        var form = StaRunner.New(() => new GroupItemSkillPowerForm(_engine));
        try
        {
            form.Grid[1, 1].Value = "30";   // 技能1 攻%
            form.Grid[2, 1].Value = "20";   // 技能1 防%
            form.Grid[1, 114].Value = "99"; // 技能114 攻%
            form.Button1Click(form);
            Assert.Equal(30, ViewList2State.SelAttackSkillPercent[1]);
            Assert.Equal(20, ViewList2State.SelDefenseSkillPercent[1]);
            Assert.Equal(99, ViewList2State.SelAttackSkillPercent[114]);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }

    [StaFact]
    public void ButtonClick_NoSelGroupItem_SkipsCaptionButWrites()
    {
        ClearMagics(_engine);
        ViewList2State.SelGroupItem = null;
        var form = StaRunner.New(() => new GroupItemSkillPowerForm(_engine));
        try
        {
            form.Grid[1, 2].Value = "15";
            form.Button1Click(form);
            Assert.Equal(15, ViewList2State.SelAttackSkillPercent[2]);
        }
        finally { StaRunner.New(() => form.Dispose()); }
    }
}

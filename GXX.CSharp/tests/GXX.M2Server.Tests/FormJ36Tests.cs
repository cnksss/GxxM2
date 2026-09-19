using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J36：MonsterConfig.pas 巨片第一片（Open 骨架 + 常规信息组 + 怪物时序组）1:1 测试。</summary>
public sealed class MonsterConfigSlice1Tests : IDisposable
{
    private readonly string _dir;
    private readonly MonsterConfigForm _form;

    public MonsterConfigSlice1Tests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j36_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetMonsterConfigSliceDefaults();
        GameConfigState.SendServerConfigCalls = 0;
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() =>
        {
            var f = new MonsterConfigForm();
            f.MagicListHandler = () => new List<string> { "火球术", "治愈术" };
            f.MonsterListHandler = () => new List<string> { "鸡", "鹿", "稻草人" };
            f.CustomMonsterListHandler = () => new List<string> { "自定义怪1" };
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
    public void Open_BuildsLists_AndLoadsControls()
    {
        StaRunner.New(() =>
        {
            M2Config.dwMonsterWarrorAttackTime = 1300;
            M2Config.nMonsterNeedMagicItem = 2;
            M2Config.boDamageLimitation = true;
            M2Config.dwMonButchDelayClearTime = 70;
            M2Config.btMonsterShowLevel = 2;
            M2Config.sMonsterShowFormat = "%s(Lv.%d)";

            _form.Open(showModal: false);

            Assert.Equal(2, _form.ListBoxMagicList.Items.Count);
            Assert.Equal("火球术", _form.ListBoxMagicList.Items[0]);
            Assert.Equal(3, _form.ListBoxMonsterList.Items.Count); // 仅 RC_PLAYMOSTER 人形怪
            Assert.Equal("自定义怪1", _form.ListBoxCustomMonster.Items[0]);
            Assert.Equal(1300, (int)_form.EditMonsterWarrorAttackTime.Value);
            Assert.Equal(2, _form.RadioGroupMonsterNeedMagicItem.SelectedIndex);
            Assert.True(_form.CheckBoxDamageLimitation.Checked);
            Assert.Equal(70, (int)_form.seMonButchDelayClearTime.Value);
            Assert.Equal(2, _form.cbbMonsterShowLevel.SelectedIndex);
            Assert.Equal("%s(Lv.%d)", _form.edtMonsterShowFormat.Text);
            Assert.True(_form.edtMonsterShowFormat.Enabled); // ShowLevel > 0
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void GeneralInfoHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.seMonButchDelayClearTime.Value = 80;
            _form.seMonButchDelayClearTimeChange(_form); // 未 Open：不写入
            Assert.Equal(60, (int)M2Config.dwMonButchDelayClearTime);

            _form.Open(showModal: false);
            _form.seMonButchDelayClearTime.Value = 80;
            _form.seMonButchDelayClearTimeChange(_form);
            _form.chkNoHumanClearMon.Checked = true;
            _form.chkNoHumanClearMonClick(_form);
            _form.seNoHumanClearMonTime.Value = 90;
            _form.seNoHumanClearMonTimeChange(_form);
            _form.cbbMonsterShowLevel.SelectedIndex = 1;
            _form.cbbMonsterShowLevelChange(_form);
            _form.edtMagStruckMonLevel.Value = 60;
            _form.edtMagStruckMonLevelChange(_form);
            _form.seScatterItemRange.Value = 4;
            _form.seScatterItemRangeChange(_form);
            _form.CheckBoxDropGoldToPlayBag.Checked = false;
            _form.CheckBoxDropGoldToPlayBagClick(_form);

            Assert.Equal(80, (int)M2Config.dwMonButchDelayClearTime);
            Assert.True(M2Config.boNoHumanClearMon);
            Assert.Equal(90, (int)M2Config.dwNoHumanClearMonTime);
            Assert.Equal(1, M2Config.btMonsterShowLevel);
            Assert.Equal(60, (int)M2Config.nMagStruckMonLevel);
            Assert.Equal(4, (int)M2Config.nScatterItemRange);
            Assert.False(M2Config.boDropGoldToPlayBag);
            Assert.True(_form.ButtonGeneralSave.Enabled);
        });
    }

    [Fact]
    public void ButtonGeneralSave_Writes16Keys_AndUnconditionalSend()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.boEnabledMaxMapItemCount = true;
            M2Config.nMaxMapItemCount = 8;
            _form.ButtonGeneralSaveClick(_form);
            Assert.Equal(60, M2ShareState.ConfigIni.ReadInteger("Setup", "MonButchDelayClearTime", -1));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "NoHumanClearMon", true));
            Assert.Equal(60, M2ShareState.ConfigIni.ReadInteger("Setup", "NoHumanClearMonTime", -1));
            Assert.Equal(0, M2ShareState.ConfigIni.ReadInteger("Setup", "MonsterShowLevel", -1));
            Assert.Equal("%s\\[Lv:%d]", M2ShareState.ConfigIni.ReadString("Setup", "MonsterShowFormat", ""));
            Assert.Equal(50, M2ShareState.ConfigIni.ReadInteger("Setup", "MagStruckMonLevel", -1));
            Assert.Equal(10, M2ShareState.ConfigIni.ReadInteger("Setup", "ElfWarriorMonsterDownDelay", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "EnabledMaxMapItemCount", false));
            Assert.Equal(8, M2ShareState.ConfigIni.ReadInteger("Setup", "MaxMapItemCount", -1));
            Assert.Equal(2000, M2ShareState.ConfigIni.ReadInteger("Setup", "MonOneDropGoldCount", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "DropGoldToPlayBag", false));
            Assert.Equal(1, GameConfigState.SendServerConfigCalls); // 无条件下发
            Assert.False(_form.ButtonGeneralSave.Enabled);
        });
    }

    [Fact]
    public void MonsterTimeHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditMonsterWarrorAttackTime.Value = 1400;
            _form.EditMonsterWarrorAttackTimeChange(_form);
            _form.EditMonsterWizardWalkTime.Value = 600;
            _form.EditMonsterWizardWalkTimeChange(_form);
            _form.RadioGroupMonsterNeedMagicItem.SelectedIndex = 3;
            _form.RadioGroupMonsterNeedMagicItemClick(_form);
            _form.chkSendCustomMonsterConfig.Checked = false;
            _form.chkSendCustomMonsterConfigClick(_form);
            _form.CheckBoxDamageLimitation.Checked = true;
            _form.CheckBoxDamageLimitationClick(_form);
            Assert.Equal(1400, (int)M2Config.dwMonsterWarrorAttackTime);
            Assert.Equal(600, (int)M2Config.dwMonsterWizardWalkTime);
            Assert.Equal(3, M2Config.nMonsterNeedMagicItem);
            Assert.False(M2Config.boSendCustomMonsterConfig);
            Assert.True(M2Config.boDamageLimitation);
        });
    }

    [Fact]
    public void ButtonMonsterConfigSave_Writes8Keys()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.dwMonsterWarrorAttackTime = 1500;
            M2Config.nMonsterNeedMagicItem = 4;
            M2Config.boDamageLimitation = true;
            _form.ButtonMonsterConfigSaveClick(_form);
            Assert.Equal(1500, M2ShareState.ConfigIni.ReadInteger("Setup", "MonsterWarrorAttackTime", -1));
            Assert.Equal(1200, M2ShareState.ConfigIni.ReadInteger("Setup", "MonsterWizardAttackTime", -1));
            Assert.Equal(500, M2ShareState.ConfigIni.ReadInteger("Setup", "MonsterTaoistWalkTime", -1));
            Assert.Equal(4, M2ShareState.ConfigIni.ReadInteger("Setup", "MonsterNeedMagicItem", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "DamageLimitation", false));
            Assert.False(_form.ButtonMonsterConfigSave.Enabled);
        });
    }
}

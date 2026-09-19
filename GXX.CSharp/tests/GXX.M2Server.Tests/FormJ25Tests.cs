using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J25：GameConfig.pas 巨片第四片（MsgColor/HumanDie/CharStatus/DieDrop 页）1:1 转换测试。</summary>
public sealed class GameConfigDieColorStatusTests : IDisposable
{
    private readonly string _dir;
    private readonly GameConfigForm _form;

    public GameConfigDieColorStatusTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j25_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetGameSpeedDefaults();
        M2Config.ResetGameOptionDefaults();
        M2Config.ResetGameMsgTimeDefaults();
        M2Config.ResetGameDieDefaults();
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
    public void Open_LoadsColorDieStatusControls()
    {
        StaRunner.New(() =>
        {
            M2Config.btHearMsgFColor = 10;
            M2Config.btHearMsgBColor = 20;
            M2Config.btNPCLabelNormalColor = 251;
            M2Config.boNPCLabelFontStroke = true;
            M2Config.nDieDropUseItemRate = 33;
            M2Config.nDropJewelryBoxItemRate = 44;
            M2Config.boDieScatterBag = true;
            M2Config.boParalyCanRun = true;
            M2Config.AttatckModes[7] = true;

            _form.Open(showModal: false);

            Assert.Equal(10, (int)_form.EditHearMsgFColor.Value);
            Assert.Equal(20, (int)_form.EdittHearMsgBColor.Value);
            Assert.Equal(251, (int)_form.seNPCLabelNormalColor.Value);
            Assert.True(_form.chkNPCLabelFontStroke.Checked);
            Assert.Equal(33, (int)_form.ScrollBarDieDropUseItemRate.Value);
            Assert.Equal("33", _form.EditDieDropUseItemRate.Text);
            Assert.Equal(44, (int)_form.scrlbrJewelryBoxItem.Value);
            Assert.Equal("44", _form.edtJewelryBoxItem.Text);
            Assert.True(_form.CheckBoxDieScatterBag.Checked);
            Assert.True(_form.CheckBoxParalyCanRun.Checked);
            Assert.False(_form.CheckBoxParalyCanWalk.Checked);
            Assert.True(_form.CheckGroupAttatckModeItems[7].Checked);
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void MsgColorHandlers_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditHearMsgFColor.Value = 100;
            _form.EditHearMsgFColorChange(_form);
            Assert.Equal(100, (int)M2Config.btHearMsgFColor);
            _form.CheckBoxShutRedMsgShowGMNameClick(_form); // 保持保存钮可用
            _form.seNPCLabelNormalColor.Value = 252;
            _form.seNPCLabelNormalColorChange(_form);

            _form.ButtonMsgColorSaveClick(_form);
            Assert.Equal(100, M2ShareState.ConfigIni.ReadInteger("Setup", "HearMsgFColor", -1));
            Assert.Equal(255, M2ShareState.ConfigIni.ReadInteger("Setup", "HearMsgBColor", -1));
            Assert.Equal(252, M2ShareState.ConfigIni.ReadInteger("Setup", "NPCLabelNormalColor", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "NPCLabelFontStroke", false));
            Assert.Equal(56, M2ShareState.ConfigIni.ReadInteger("Setup", "RedMsgBColor", -1));
            Assert.False(_form.ButtonMsgColorSave.Enabled);
        });
    }

    [Fact]
    public void ButtonMsgColorSave_UnconditionalSend_WithNationQuirk()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditHearMsgFColorChange(_form); // ModValue（无需下发标志）
            _form.ButtonMsgColorSaveClick(_form);
            Assert.Equal(1, GameConfigState.SendServerConfigCalls); // 尾部无条件 SendServerConfig
            // Delphi 原文 NationMsgBColor 键先于 NationMsgFColor 键写入：两键独立存在即验证顺序无碍
            Assert.Equal(255, M2ShareState.ConfigIni.ReadInteger("Setup", "NationMsgBColor", -1));
            Assert.Equal(219, M2ShareState.ConfigIni.ReadInteger("Setup", "NationMsgFColor", -1));
        });
    }

    [Fact]
    public void HumanDieHandlers_WriteConfig_AndSave_WithDoubleKeyQuirk()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.ScrollBarDieScatterBagRate.Value = 7;
            _form.ScrollBarDieScatterBagRateChange(_form);
            Assert.Equal(7, (int)M2Config.nDieScatterBagRate);
            Assert.Equal("7", _form.EditDieScatterBagRate.Text);
            _form.CheckBoxDieDropGold.Checked = true;
            _form.CheckBoxDieDropGoldClick(_form);
            Assert.True(M2Config.boDieDropGold);

            M2Config.nDieScatterBagRate = 9; // 复合键验证：保存两次写同一键均取当前值
            _form.ButtonHumanDieSaveClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "DieScatterBag", false));
            Assert.Equal(9, M2ShareState.ConfigIni.ReadInteger("Setup", "DieScatterBagRate", -1));
            Assert.Equal(30, M2ShareState.ConfigIni.ReadInteger("Setup", "DieDropUseItemRate", -1));
            Assert.Equal(30, M2ShareState.ConfigIni.ReadInteger("Setup", "DropJewelryBoxItemRate", -1));
            Assert.Equal(15, M2ShareState.ConfigIni.ReadInteger("Setup", "DropUseItemsMaxCount", -1));
            Assert.Equal(1, M2ShareState.ConfigIni.ReadInteger("Setup", "ScatterBagItemsMinLevel", -1));
            Assert.False(_form.ButtonHumanDieSave.Enabled);
        });
    }

    [Fact]
    public void DieDropUseItemRates_ScrollAndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            Assert.Equal(30, (int)_form.DieDropUseItemRateScrolls[5].Value); // Open 预置
            _form.DieDropUseItemRateScrolls[5].Value = 77;
            _form.DieDropUseItemRateScrollChanged(5);
            Assert.Equal(77, (int)M2Config.DieDropUseItemRates[5]);
            Assert.Equal("77", _form.edtDieDropUseItemRateCells[5].Text);

            _form.CheckBoxDropUseItem.Checked = true;
            _form.CheckBoxDropUseItemClick(_form);
            Assert.True(M2Config.boDropUseItem);

            _form.ButtonDieDropUseItemSaveClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "DropUseItem", false));
            Assert.Equal(20, M2ShareState.ConfigIni.ReadInteger("Setup", "DieRedDropUseItemOneRate", -1));
            Assert.Equal(30, M2ShareState.ConfigIni.ReadInteger("Setup", "DieDropUseItemRates0", -1));
            Assert.Equal(77, M2ShareState.ConfigIni.ReadInteger("Setup", "DieDropUseItemRates5", -1));
            Assert.Equal(30, M2ShareState.ConfigIni.ReadInteger("Setup", "DieDropUseItemRates29", -1));
            Assert.False(_form.ButtonDieDropUseItemSave.Enabled);
        });
    }

    [Fact]
    public void ParalyHandlers_WriteConfig_AndSend()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxParalyCanWalk.Checked = true;
            _form.CheckBoxParalyCanWalkClick(_form);
            Assert.True(M2Config.boParalyCanWalk);
            Assert.Equal(1, GameConfigState.SendServerConfigCalls); // 每个弯腰开关固定下发
            _form.CheckBoxParalyCanWalkClick(_form);
            Assert.Equal(2, GameConfigState.SendServerConfigCalls);

            _form.ButtonCharStatusSaveClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "ParalyCanWalk", false));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "ParalyCanRun", true));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "AttatckModes0", false));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "AttatckModes7", true));
            Assert.False(_form.ButtonCharStatusSave.Enabled);
        });
    }

    [Fact]
    public void CheckGroupAttatckMode_GuardLastRemaining()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            // 关闭 1..7，仅剩 0
            for (int i = 1; i <= 7; i++)
                _form.CheckGroupAttatckModeChange(i, false);
            Assert.False(M2Config.AttatckModes[7]);

            M2Forms.NextAnswer = M2Forms.IDOK;
            _form.CheckGroupAttatckModeChange(0, false); // 最后一种被关：弹窗并回勾
            Assert.Equal("最少要选择一种攻击模式", M2Forms.LastMessage);
            Assert.True(M2Config.AttatckModes[0]);
            Assert.True(_form.CheckGroupAttatckModeItems[0].Checked);

            _form.CheckGroupAttatckModeChange(1, true); // 正常开启
            Assert.True(M2Config.AttatckModes[1]);
        });
    }

    [Fact]
    public void JewelryAndGodBless_Checkboxes()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.chkKillByMonstDropJewelryBoxItem.Checked = true;
            _form.chkKillByMonstDropJewelryBoxItemClick(_form);
            Assert.True(M2Config.boKillByMonstDropJewelryBoxItem);
            _form.chkKillByHumanDropGodBlessItem.Checked = true;
            _form.chkKillByHumanDropGodBlessItemClick(_form);
            Assert.True(M2Config.boKillByHumanDropGodBlessItem);
            _form.scrlbrGodBlessItem.Value = 88;
            _form.scrlbrGodBlessItemChange(_form);
            Assert.Equal(88, (int)M2Config.nDropGodBlessItemRate);
            Assert.Equal("88", _form.edtGodBlessItem.Text);
        });
    }
}

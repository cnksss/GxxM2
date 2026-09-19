using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J34：ItemSet.pas 巨片第一片（主保存组：凹槽/体验率/传送/异常状态）1:1 测试。</summary>
public sealed class ItemSetSlice1Tests : IDisposable
{
    private readonly string _dir;
    private readonly ItemSetForm _form;

    public ItemSetSlice1Tests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j34_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetItemSetSliceDefaults();
        GameConfigState.SendServerConfigCalls = 0;
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        _form = StaRunner.New(() => new ItemSetForm());
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
    public void Open_LoadsSliceControls()
    {
        StaRunner.New(() =>
        {
            M2Config.boOpenItemFlute = false;
            M2Config.nItemFluteStoneCount = 5;
            M2Config.nItemExpRate = 20000;
            M2Config.nAttackPosionRate = 8;
            M2Config.dwUserMoveTime = 20;
            M2Config.dwFrozenTime = 9;

            _form.Open(showModal: false);

            Assert.False(_form.chkOpenItemFlute.Checked);
            Assert.Equal(5, (int)_form.seItemFluteStoneCount.Value);
            Assert.Equal(20000, (int)_form.EditItemExpRate.Value);
            Assert.Equal(10000, (int)_form.EditItemPowerRate.Value);
            Assert.Equal(8, (int)_form.EditAttackPosionRate.Value);
            Assert.Equal(20, (int)_form.EditUserMoveTime.Value);
            Assert.Equal(9, (int)_form.EditFrozenTime.Value);
            Assert.False(_form.IsModValued);
            Assert.False(_form.ButtonItemSetSave.Enabled);
        });
    }

    [Fact]
    public void FluteHandlers_GatedAndSendFlag()
    {
        StaRunner.New(() =>
        {
            _form.chkDisableRightClickFluteStone.Checked = true;
            _form.chkDisableRightClickFluteStoneClick(_form); // 未 Open：不写入
            Assert.False(M2Config.boDisableRightClickFluteStone);

            _form.Open(showModal: false);
            _form.chkOpenItemFlute.Checked = false;
            _form.chkOpenItemFluteClick(_form);
            Assert.False(M2Config.boOpenItemFlute); // 写入成功（下发标志的有无由保存测试验证）

            _form.chkDisableRightClickFluteStone.Checked = true;
            _form.chkDisableRightClickFluteStoneClick(_form);
            Assert.True(M2Config.boDisableRightClickFluteStone);
            Assert.True(_form.ButtonItemSetSave.Enabled);

            _form.seItemFluteStoneCount.Value = 4;
            _form.seItemFluteStoneCountChange(_form);
            Assert.Equal(4, (int)M2Config.nItemFluteStoneCount);
        });
    }

    [Fact]
    public void ExpPosionUserMoveHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditItemExpRate.Value = 30000;
            _form.EditItemExpRateChange(_form);
            Assert.Equal(30000, (int)M2Config.nItemExpRate);
            _form.EditItemPowerRate.Value = 20000;
            _form.EditItemPowerRateChange(_form);
            Assert.Equal(20000, (int)M2Config.nItemPowerRate);
            _form.EditAttackPosionRate.Value = 9;
            _form.EditAttackPosionRateChange(_form);
            Assert.Equal(9, (int)M2Config.nAttackPosionRate);
            _form.EditUserMoveTime.Value = 30;
            _form.EditUserMoveTimeChange(_form);
            Assert.Equal(30, (int)M2Config.dwUserMoveTime);
            _form.CheckBoxUserMoveCanDupObj.Checked = true;
            _form.CheckBoxUserMoveCanDupObjClick(_form);
            Assert.True(M2Config.boUserMoveCanDupObj);
        });
    }

    [Fact]
    public void StatusHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditMDParalysisRate.Value = 7;
            _form.EditMDParalysisRateChange(_form);
            Assert.Equal(7, (int)M2Config.dwMDParalysisRate);
            _form.EditFrozenRate.Value = 8;
            _form.EditFrozenRateChange(_form);
            Assert.Equal(8, (int)M2Config.dwFrozenRate);
            _form.CheckBoxFrozenUseMagicStruck.Checked = true;
            _form.CheckBoxFrozenUseMagicStruckClick(_form);
            Assert.True(M2Config.boFrozenUseMagicStruck);
            _form.EditCobwebWindingRate.Value = 11;
            _form.EditCobwebWindingRateChange(_form);
            Assert.Equal(11, (int)M2Config.dwCobwebWindingRate);
        });
    }

    [Fact]
    public void ButtonItemSetSave_WritesKeys_AndGatedSend()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.chkDisableRightClickFluteStone.Checked = true;
            _form.chkDisableRightClickFluteStoneClick(_form); // 置下发标志
            _form.ButtonItemSetSaveClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "OpenItemFlute", false));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "DisableRightClickFluteStone", false));
            Assert.Equal(3, M2ShareState.ConfigIni.ReadInteger("Setup", "ItemFluteStoneCount", -1));
            Assert.Equal(10000, M2ShareState.ConfigIni.ReadInteger("Setup", "ItemPowerRate", -1));
            Assert.Equal(10000, M2ShareState.ConfigIni.ReadInteger("Setup", "ItemExpRate", -1));
            Assert.Equal(180, M2ShareState.ConfigIni.ReadInteger("Setup", "GuildRecallTime", -1));
            // 原文瑕疵：GroupRecallTime 键写 nAttackPosionRate 值
            Assert.Equal(5, M2ShareState.ConfigIni.ReadInteger("Setup", "GroupRecallTime", -1));
            Assert.Equal(5, M2ShareState.ConfigIni.ReadInteger("Setup", "AttackPosionRate", -1));
            Assert.Equal(6, M2ShareState.ConfigIni.ReadInteger("Setup", "AttackPosionTime", -1));
            Assert.Equal(5, M2ShareState.ConfigIni.ReadInteger("Setup", "MDParalysisRate", -1));
            Assert.Equal(6, M2ShareState.ConfigIni.ReadInteger("Setup", "FrozenTime", -1));
            Assert.Equal(1, GameConfigState.SendServerConfigCalls); // 下发后 boSendServerConfig 复位
            _form.ButtonItemSetSaveClick(_form);
            Assert.Equal(1, GameConfigState.SendServerConfigCalls); // 标志已清，不再下发
            Assert.False(_form.ButtonItemSetSave.Enabled);
        });
    }
}

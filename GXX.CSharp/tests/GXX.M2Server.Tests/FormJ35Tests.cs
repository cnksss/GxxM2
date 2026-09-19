using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J35：ItemSet.pas 巨片第二片（AddValue 极品属性全键/UnKnow 未鉴定/NewAbil 新属性标量）1:1 测试。</summary>
public sealed class ItemSetSlice2Tests : IDisposable
{
    private readonly string _dir;
    private readonly ItemSetForm _form;

    public ItemSetSlice2Tests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j35_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetItemSetSliceDefaults();
        M2Config.ResetItemSetSlice2Defaults();
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
    public void Open_LoadsAddValueAndUnknowControls()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);

            Assert.Equal(206, _form.AddItemValueSpins.Count);
            Assert.Equal(30, _form.UnknowSpins.Count);
            Assert.Equal(12, (int)_form.AddItemValueSpins["WeaponDCAddValueMaxLimit"].Value);
            Assert.Equal(0, (int)_form.AddItemValueSpins["ShieldMACAddRate"].Value);
            Assert.Equal(20, (int)_form.UnknowSpins["UnknowRingACAddRate"].Value);
            Assert.Equal(4, (int)_form.UnknowSpins["UnknowRingMACAddValueMaxLimit"].Value);
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void AddValueSpinWritesDictionary()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.AddItemValueSpins["WeaponDCAddValueMaxLimit"].Value = 20;
            _form.AddItemValueSpins["NeckLace19SCAddRate"].Value = 33;
            Assert.Equal(20, M2Config.ItemSetAddValues["WeaponDCAddValueMaxLimit"]);
            Assert.Equal(33, M2Config.ItemSetAddValues["NeckLace19SCAddRate"]);
            Assert.True(_form.ButtonAddValueSave.Enabled);
        });
    }

    [Fact]
    public void ButtonAddValueSave_WritesAllKeysInOrder()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.ItemSetAddValues["MonRandomAddValue"] = 15;
            M2Config.ItemSetAddValues["WeaponDCAddValueMaxLimit"] = 20;
            M2Config.ItemSetAddValues["ShieldMACAddRate"] = 5;
            _form.ButtonAddValueSaveClick(_form);
            Assert.Equal(15, M2ShareState.ConfigIni.ReadInteger("Setup", "MonRandomAddValue", -1));
            Assert.Equal(20, M2ShareState.ConfigIni.ReadInteger("Setup", "WeaponDCAddValueMaxLimit", -1));
            Assert.Equal(15, M2ShareState.ConfigIni.ReadInteger("Setup", "WeaponDCAddValueRate", -1));
            Assert.Equal(5, M2ShareState.ConfigIni.ReadInteger("Setup", "ShieldMACAddRate", -1));
            Assert.Equal(0, M2ShareState.ConfigIni.ReadInteger("Setup", "ShieldMACAddValueMaxLimit", -1));
            Assert.False(_form.ButtonAddValueSave.Enabled);
        });
    }

    [Fact]
    public void UnknowSpinWritesDictionary_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.UnknowSpins["UnknowRingDCAddValueMaxLimit"].Value = 9;
            _form.UnknowSpinChanged("UnknowRingDCAddValueMaxLimit");
            Assert.Equal(9, (int)M2Config.ItemSetUnknowValues["UnknowRingDCAddValueMaxLimit"]);

            _form.ButtonUnKnowItemSaveClick(_form);
            Assert.Equal(20, M2ShareState.ConfigIni.ReadInteger("Setup", "UnknowRingACAddRate", -1));
            Assert.Equal(9, M2ShareState.ConfigIni.ReadInteger("Setup", "UnknowRingDCAddValueMaxLimit", -1));
            Assert.Equal(30, M2ShareState.ConfigIni.ReadInteger("Setup", "UnknowNecklaceDCAddRate", -1));
            Assert.Equal(4, M2ShareState.ConfigIni.ReadInteger("Setup", "UnknowHelMetMACAddValueMaxLimit", -1));
            Assert.False(_form.ButtonUnKnowItemSave.Enabled);
        });
    }

    [Fact]
    public void NewAbilHandlers_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            Assert.False(_form.chkItemNewAbilAllowUse.Checked); // 默认 False

            _form.chkItemNewAbilAllowUse.Checked = false;
            _form.chkItemNewAbilAllowUseClick(_form);
            Assert.False(M2Config.boItemNewAbilAllowUse);

            M2Config.nItemNewAbilMonRandomAddValue = 25;
            M2Config.dwCritAttackHurtRate = 15000;
            _form.ButtonNewAbilSaveClick(_form);
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "ItemNewAbilAllowUse", true));
            Assert.Equal(25, M2ShareState.ConfigIni.ReadInteger("Setup", "ItemNewAbilMonRandomAddValue", -1));
            Assert.Equal(15000, M2ShareState.ConfigIni.ReadInteger("Setup", "CritAttackHurtRate", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "CloseDefenseUseScale", true) == M2Config.boCloseDefenseUseScale);
            Assert.False(_form.ButtonNewAbilSave.Enabled);
        });
    }
}

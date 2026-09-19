using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J30：FunctionConfig.pas 巨片第七片（OffLine 离线 + MyShop 个人商铺页）1:1 测试。</summary>
public sealed class FunctionConfigOffLineMyShopTests : IDisposable
{
    private readonly string _dir;
    private readonly FunctionConfigForm _form;

    public FunctionConfigOffLineMyShopTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j30_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetFunctionDefaults();
        M2Config.ResetFunctionSkillDefaults();
        M2Config.ResetFunctionMineDefaults();
        M2Config.ResetFunctionReNewMonDefaults();
        M2Config.ResetFunctionMutinyMsgDefaults();
        M2Config.ResetFunctionShopDefaults();
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
    public void Open_RefOffLine_And_MyShop()
    {
        StaRunner.New(() =>
        {
            M2Config.boOffLineLoginSafeArea = true;
            M2Config.sSetOffLineLoginMapName = "5";
            M2Config.nMaxMyShopSellingItemCount = 10;
            M2Config.boOpenSelfShop = false;

            _form.Open(showModal: false);

            Assert.True(_form.CheckBoxOffLineLoginSafeArea.Checked);
            Assert.Equal(1, (int)_form.EditOffLineLoginMapName.Value);
            Assert.Equal("5", _form.EditSetOffLineLoginMapName.Text);
            Assert.Equal(10, (int)_form.EditMaxMyShopSellingItemCount.Value);
            Assert.False(_form.CheckBoxOpenSelfShop.Checked);
            Assert.True(_form.CheckBoxMyShopGold.Checked); // 默认 True
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void OffLineHandlers_WriteConfig_Gated()
    {
        StaRunner.New(() =>
        {
            _form.CheckBoxMonNoAttackOffLinePlayer.Checked = true;
            _form.CheckBoxMonNoAttackOffLinePlayerClick(_form); // 未 Open：不写入
            Assert.False(M2Config.boMonNoAttackOffLinePlayer);

            _form.Open(showModal: false);
            _form.CheckBoxOffLineLoginSafeArea.Checked = true;
            _form.CheckBoxOffLineLoginSafeAreaClick(_form);
            _form.EditSetOffLineLoginMapName.Text = "8";
            _form.EditSetOffLineLoginMapNameChange(_form);
            _form.CheckBoxMonNoAttackOffLinePlayer.Checked = true;
            _form.CheckBoxMonNoAttackOffLinePlayerClick(_form);
            Assert.True(M2Config.boOffLineLoginSafeArea);
            Assert.Equal("8", M2Config.sSetOffLineLoginMapName);
            Assert.True(M2Config.boMonNoAttackOffLinePlayer);
            Assert.True(_form.ButtonOffLineSave.Enabled);

            _form.ButtonOffLineSaveClick(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "OffLineLoginSafeArea", false));
            Assert.Equal(1, M2ShareState.ConfigIni.ReadInteger("Setup", "OffLineLoginMapName", -1));
            Assert.Equal("8", M2ShareState.ConfigIni.ReadString("Setup", "SetOffLineLoginMapName", ""));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "MonNoAttackOffLinePlayer", false));
            Assert.False(_form.ButtonOffLineSave.Enabled);
        });
    }

    [Fact]
    public void MyShopHandlers_WriteConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditMaxMyShopSellingItemCount.Value = 12;
            _form.EditMaxMyShopSellingItemCountChange(_form);
            _form.CheckBoxUseHeroM2Shop.Checked = false;
            _form.CheckBoxUseHeroM2ShopClick(_form);
            _form.CheckBoxProhibitModifyPrices.Checked = true;
            _form.CheckBoxProhibitModifyPricesClick(_form);
            _form.EditInfinityStorageCount.Value = 80;
            _form.EditInfinityStorageCountChange(_form);
            _form.CheckBoxMyShopGameDiamond.Checked = false;
            _form.CheckBoxMyShopGameDiamondClick(_form);
            _form.CheckBoxMapShop.Checked = true;
            _form.CheckBoxMapShopClick(_form);
            _form.EditSellOffGoldTaxRate.Value = 15;
            _form.EditSellOffGoldTaxRateChange(_form);

            Assert.Equal(12, (int)M2Config.nMaxMyShopSellingItemCount);
            Assert.False(M2Config.boUseHeroM2Shop);
            Assert.True(M2Config.boProhibitModifyPrices);
            Assert.Equal(80, (int)M2Config.nInfinityStorageCount);
            Assert.False(M2Config.boMyShopGameDiamond);
            Assert.True(M2Config.boMapShop);
            Assert.Equal(15, (int)M2Config.dwSellOffGoldTaxRate);
        });
    }

    [Fact]
    public void ButtonMyShopSave_WritesKeys()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.boOpenSelfShop = false;
            M2Config.boSafeZoneShop = false;
            M2Config.dwSellOffGamePointTaxRate = 20;
            _form.ButtonMyShopSaveClick(_form);
            Assert.Equal(7, M2ShareState.ConfigIni.ReadInteger("Setup", "MaxMyShopSellingItemCount", -1));
            Assert.Equal(7, M2ShareState.ConfigIni.ReadInteger("Setup", "MaxMyShopStorageItemCount", -1));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "OfflineCloseMyShop", true));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "ProhibitModifyPrices", true));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "UseHeroM2Shop", false));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "InfinityStorage", true));
            Assert.Equal(50, M2ShareState.ConfigIni.ReadInteger("Setup", "InfinityStorageCount", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "MyShopGold", false));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "MyShopGamePoint", false));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "OpenSelfShop", true));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "SafeZoneShop", true));
            Assert.Equal(20, M2ShareState.ConfigIni.ReadInteger("Setup", "SellOffGamePointTaxRate", -1));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "ShopHeadPic", true));
            Assert.False(_form.ButtonMyShopSave.Enabled);
        });
    }
}

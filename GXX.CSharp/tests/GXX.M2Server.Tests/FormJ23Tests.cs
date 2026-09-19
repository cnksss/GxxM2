using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J23：GameConfig.pas 巨片第二片（城堡/Option 跑动交易安全区/PK/测试服页）1:1 转换测试。</summary>
public sealed class GameConfigOptionPageTests : IDisposable
{
    private readonly string _dir;
    private readonly GameConfigForm _form;

    public GameConfigOptionPageTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j23_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetGameSpeedDefaults();
        M2Config.ResetGameOptionDefaults();
        GameConfigState.SendServerConfigCalls = 0;
        GameConfigState.SendMapCanRunCalls = 0;
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
    public void Open_LoadsCastleOptionPkTestServerControls()
    {
        StaRunner.New(() =>
        {
            M2Config.nRepairDoorPrice = 3000000;
            M2Config.sCASTLENAME = "沙城";
            M2Config.sCastleHomeMap = "4";
            M2Config.boDiableHumanRun = false;
            M2Config.boRUNHUMAN = true;
            M2Config.boWarDisHumRun = true;
            M2Config.boWarHreoRun = true;
            M2Config.dwTryDealTime = 5000;
            M2Config.nSafeZoneSize = 12;
            M2Config.boHintSafeZone = true;
            M2Config.sRedHomeMap = "5";
            M2Config.nRedHomeX = 100;
            M2Config.sHomeMap = "2";
            M2Config.dwDecPkPointTime = 90000;
            M2Config.nDecPkPointCount = 3;
            M2Config.boKillHumanWinLevel = true;
            M2Config.nKillHumanWinLevel = 7;
            M2Config.boTestServer = true;
            M2Config.nTestLevel = 30;
            M2Config.nHumanMaxGold = 200000000;

            _form.Open(showModal: false);

            // 城堡页
            Assert.Equal(3000000, (int)_form.EditRepairDoorPrice.Value);
            Assert.Equal("沙城", _form.EditCastleName.Text);
            Assert.Equal("4", _form.EditCastleHomeMap.Text);
            // Option 跑动组（chkDisHumRun.Checked = not boDiableHumanRun → true；子项使能）
            Assert.True(_form.chkDisHumRun.Checked);
            Assert.True(_form.chkRunHum.Checked);
            Assert.True(_form.chkRunHum.Enabled);
            Assert.True(_form.chkWarHreoRun.Enabled); // WarDisHumRun 级联
            // 交易/安全区
            Assert.Equal(5, (int)_form.seTryDealTime.Value); // 5000 div 1000
            Assert.Equal(12, (int)_form.EditSafeZoneSize.Value);
            Assert.True(_form.seHintSafeZoneY.Enabled); // HintSafeZone 级联
            // 红名/回城
            Assert.Equal("5", _form.EditRedHomeMap.Text);
            Assert.Equal(100, (int)_form.EditRedHomeX.Value);
            Assert.Equal("2", _form.EditHomeMap.Text);
            // PK（时间 div 1000）
            Assert.Equal(90, (int)_form.EditDecPkPointTime.Value);
            Assert.Equal(3, (int)_form.EditDecPkPointCount.Value);
            Assert.True(_form.CheckBoxKillHumanWinLevel.Checked);
            Assert.Equal(7, (int)_form.EditKillHumanWinLevel.Value);
            Assert.True(_form.EditKillHumanWinLevel.Enabled); // WinLevel 级联
            // 测试服
            Assert.True(_form.CheckBoxTestServer.Checked);
            Assert.True(_form.seTestLevel.Enabled);
            Assert.Equal(30, (int)_form.seTestLevel.Value);
            Assert.Equal(200000000, (int)_form.seHumanMaxGold.Value);
            Assert.False(_form.IsModValued);
        });
    }

    [Fact]
    public void CastleHandlers_WriteConfig_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditRepairDoorPrice.Value = 4000000;
            _form.EditCastleHomeMap.Text = "9";
            _form.EditCastleHomeMapChange(_form);
            _form.EditCastleName.Text = "新城";
            _form.EditCastleNameChange(_form);
            _form.CheckBoxGetAllNpcTax.Checked = true;
            _form.CheckBoxGetAllNpcTaxClick(_form);
            Assert.Equal(4000000, (int)M2Config.nRepairDoorPrice);
            Assert.Equal("9", M2Config.sCastleHomeMap);
            Assert.Equal("新城", M2Config.sCASTLENAME);
            Assert.True(M2Config.boGetAllNpcTax);
            Assert.True(_form.ButtonCastleSave.Enabled);

            _form.ButtonCastleSaveClick(_form);
            Assert.Equal(4000000, M2ShareState.ConfigIni.ReadInteger("Setup", "RepairDoor", -1));
            Assert.Equal("新城", M2ShareState.ConfigIni.ReadString("Setup", "CastleName", ""));
            Assert.Equal("9", M2ShareState.ConfigIni.ReadString("Setup", "CastleHomeMap", ""));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "CastleGetAllNpcTax", false));
            Assert.False(_form.ButtonCastleSave.Enabled);
        });
    }

    [Fact]
    public void DisHumRun_Cascade()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.chkDisHumRun.Checked = true; // Checked=true → boChecked=false → 子项使能
            _form.chkDisHumRunClick(_form);
            Assert.False(M2Config.boDiableHumanRun);
            Assert.True(_form.chkRunMon.Enabled);
            Assert.True(_form.chkSafeArea.Enabled);

            _form.chkDisHumRun.Checked = false; // boChecked=true → 子项清空并禁用
            _form.chkDisHumRunClick(_form);
            Assert.True(M2Config.boDiableHumanRun);
            Assert.False(_form.chkRunHum.Checked);
            Assert.False(_form.chkRunHum.Enabled);
            Assert.False(_form.chkWarDisHumRun.Enabled);
            Assert.False(_form.chkSafeArea.Enabled);
        });
    }

    [Fact]
    public void WarDisHumRun_EnablesWarHreoRun_AndWrites()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.chkWarDisHumRun.Checked = true;
            _form.chkWarDisHumRunClick(_form);
            Assert.True(M2Config.boWarDisHumRun);
            Assert.True(_form.chkWarHreoRun.Enabled);
            _form.chkWarHreoRun.Checked = true;
            _form.chkWarHreoRunClick(_form);
            Assert.True(M2Config.boWarHreoRun);

            _form.chkWarDisHumRun.Checked = false;
            _form.chkWarDisHumRunClick(_form); // boWarHreoRun = Enabled(false) and Checked → false
            Assert.False(M2Config.boWarHreoRun);
        });
    }

    [Fact]
    public void DealSafeZoneHandlers_WriteConfig_TimesScaled()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.seTryDealTime.Value = 6;
            _form.seTryDealTimeChange(_form);
            Assert.Equal(6000, (int)M2Config.dwTryDealTime);
            _form.seDealOKTime.Value = 2;
            _form.seDealOKTimeChange(_form);
            Assert.Equal(2000, (int)M2Config.dwDealOKTime);
            _form.seCanDropGold.Value = 2000;
            _form.seCanDropGoldChange(_form);
            Assert.Equal(2000, (int)M2Config.nCanDropGold);
            _form.chkControlDropItem.Checked = true;
            _form.chkControlDropItemClick(_form);
            Assert.True(M2Config.boControlDropItem);
            _form.EditSafeZoneSize.Value = 15;
            _form.EditSafeZoneSizeChange(_form);
            Assert.Equal(15, (int)M2Config.nSafeZoneSize);
            _form.chkHintSafeZone.Checked = false;
            _form.chkHintSafeZoneClick(_form);
            Assert.False(M2Config.boHintSafeZone);
            Assert.False(_form.seHintSafeZoneY.Enabled);
        });
    }

    [Fact]
    public void ButtonOptionSave3_WritesRunDealChallengeIni()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            M2Config.boSafeAreaDisNpcRun = true;
            M2Config.btChallengeGoldIndex = 2;
            _form.ButtonOptionSave3Click(_form);
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "DiableHumanRun", false));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "RunHuman", true));
            Assert.Equal(3000, M2ShareState.ConfigIni.ReadInteger("Setup", "TryDealTime", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "SafeAreaDisNpcRun", false));
            Assert.Equal(60000, M2ShareState.ConfigIni.ReadInteger("Setup", "ChallengeTime", -1));
            Assert.Equal(2, M2ShareState.ConfigIni.ReadInteger("Setup", "ChallengeGoldIndex", -1));
            Assert.Equal(1, GameConfigState.SendMapCanRunCalls); // UserEngine.SendMapCanRun 接缝
            Assert.False(_form.ButtonOptionSave3.Enabled);
        });
    }

    [Fact]
    public void ButtonOptionSave_EmptyMapValidation_ThenWrite()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.EditSafeZoneSize.Value = 12; // 先改动触发 ModValue（保存钮可用）
            _form.EditSafeZoneSizeChange(_form);
            _form.EditRedHomeMap.Text = "";
            M2Forms.NextAnswer = M2Forms.IDOK;
            _form.ButtonOptionSaveClick(_form);
            Assert.Equal("红名村地图设置错误！", M2Forms.LastMessage);
            Assert.True(_form.ButtonOptionSave.Enabled); // Exit：未落盘

            _form.EditRedHomeMap.Text = "5";
            _form.EditRedDieHomeMap.Text = "5";
            _form.EditHomeMap.Text = "2";
            _form.EditSafeZoneSize.Value = 20;
            _form.chkNationGroupCheck.Checked = true;
            _form.chkNationGroupCheckClick(_form);
            _form.ButtonOptionSaveClick(_form);
            Assert.Equal(20, M2ShareState.ConfigIni.ReadInteger("Setup", "SafeZoneSize", -1));
            Assert.Equal("5", M2ShareState.ConfigIni.ReadString("Setup", "RedHomeMap", ""));
            Assert.Equal("5", M2ShareState.ConfigIni.ReadString("Setup", "RedDieHomeMap", ""));
            Assert.Equal("2", M2ShareState.ConfigIni.ReadString("Setup", "HomeMap", ""));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "NationGroupCheck", false));
            Assert.False(_form.ButtonOptionSave.Enabled);
        });
    }

    [Fact]
    public void PkCascades_AndSave()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxKillHumanWinLevel.Checked = false;
            _form.CheckBoxKillHumanWinLevelClick(_form);
            Assert.False(M2Config.boKillHumanWinLevel);
            Assert.False(_form.CheckBoxKilledLostLevel.Enabled);
            Assert.False(_form.CheckBoxKilledLostLevel.Checked);
            Assert.False(_form.EditKillHumanWinLevel.Enabled);

            _form.CheckBoxKillHumanWinExp.Checked = true;
            _form.CheckBoxKillHumanWinExpClick(_form); // WinExp 开 → EditHumanLevelDiffer 使能
            Assert.True(M2Config.boKillHumanWinExp);
            Assert.True(_form.EditHumanLevelDiffer.Enabled);

            _form.CheckBoxPKLevelProtect.Checked = true;
            _form.CheckBoxPKLevelProtectClick(_form);
            Assert.True(M2Config.boPKLevelProtect);
            Assert.True(_form.EditPKProtectLevel.Enabled);

            _form.EditDecPkPointTime.Value = 100;
            _form.EditDecPkPointTimeChange(_form);
            Assert.Equal(100000, (int)M2Config.dwDecPkPointTime);

            _form.ButtonOptionSave2Click(_form);
            Assert.Equal(100000, M2ShareState.ConfigIni.ReadInteger("Setup", "DecPkPointTime", -1));
            Assert.Equal(60000, M2ShareState.ConfigIni.ReadInteger("Setup", "PKFlagTime", -1));
            Assert.Equal(100, M2ShareState.ConfigIni.ReadInteger("Setup", "KillHumanAddPKPoint", -1));
            Assert.False(M2ShareState.ConfigIni.ReadBool("Setup", "KillHumanWinLevel", true));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "KillHumanWinExp", false));
            Assert.Equal(100000, M2ShareState.ConfigIni.ReadInteger("Setup", "KillHumanWinExpPoint", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "PKProtect", false));
            Assert.False(_form.ButtonOptionSave2.Enabled);
        });
    }

    [Fact]
    public void TestServer_Cascade_AndSave0()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.CheckBoxTestServer.Checked = false;
            _form.CheckBoxTestServerClick(_form);
            Assert.False(M2Config.boTestServer);
            Assert.False(_form.seTestLevel.Enabled);
            Assert.False(_form.seTestGold.Enabled);
            Assert.False(_form.seTestUserLimit.Enabled);

            M2Config.boTestServer = true;
            M2Config.boNonPKServer = true;
            M2Config.nTestLevel = 30;
            M2Config.nStartPermission = 2;
            M2Config.nHumanMaxGold = 200000000;
            _form.seStartPermission.Value = 2;
            _form.seStartPermissionChange(_form);
            _form.chkOffLineShop.Checked = true;
            _form.chkOffLineShopClick(_form);
            Assert.True(M2Config.boOffLineShop);

            _form.ButtonOptionSave0Click(_form);
            Assert.Equal("-1", M2ShareState.ConfigIni.ReadString("Server", "TestServer", "")); // BoolToStr '-1' 形态
            Assert.Equal("0", M2ShareState.ConfigIni.ReadString("Server", "ServiceMode", ""));
            Assert.Equal("-1", M2ShareState.ConfigIni.ReadString("Server", "NonPKServer", ""));
            Assert.Equal(30, M2ShareState.ConfigIni.ReadInteger("Server", "TestLevel", -1));
            Assert.Equal(2, M2ShareState.ConfigIni.ReadInteger("Setup", "StartPermission", -1));
            Assert.Equal(200000000, M2ShareState.ConfigIni.ReadInteger("Setup", "HumanMaxGold", -1));
            Assert.True(M2ShareState.ConfigIni.ReadBool("Setup", "OffLineShop", false));
            Assert.Equal(0, GameConfigState.SendServerConfigCalls); // boSendServerConfig 未置位
            Assert.False(_form.ButtonOptionSave0.Enabled);
        });
    }

    [Fact]
    public void SpeedControl_Click_SetsSendFlag()
    {
        StaRunner.New(() =>
        {
            _form.Open(showModal: false);
            _form.chkSpeedControl.Checked = false;
            _form.chkSpeedControlClick(_form);
            Assert.False(M2Config.boSpeedControl);
            _form.ButtonGameSpeedSaveClick(_form); // boSendServerConfig 已由 chkSpeedControlClick 置位
            Assert.Equal(1, GameConfigState.SendServerConfigCalls);
        });
    }
}

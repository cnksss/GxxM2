using System.Text;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J69：ConfigClient.pas 尾片（2500-4266：恢复节奏族/小地图族/偏移族/四 Save/恢复默认/排列按钮）。</summary>
public sealed class FormJ69Tests : IDisposable
{
    private readonly string _tempDir;
    private readonly ConfigClientForm _form;
    private int _sendServerConfigCalls;

    public FormJ69Tests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "j69_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        M2ShareState.ResetForTests(_tempDir);
        M2Config.boUseFindPath = true;
        M2Config.boUseOldSerialWindows = false;
        M2Config.boHealthNumberText = false;
        M2Config.boHideIconWithHideTitle = false;
        M2Config.boShowExSkillIcon = false;
        M2Config.btMinMapColorSelf = 255;
        M2Config.nPerHealth = 10;
        M2Config.nPerSpell = 10;
        M2Config.nIncHealthSpellTime = 700;
        M2Config.nHealthFillTime = 450;
        M2Config.nHealthFillTime_Human_Warrior = 350;
        M2Config.nHealthBaseNum_Human_Warrior = 75;
        M2Config.nSpellFillTime_Human_Warrior = 800;
        M2Config.nSpellBaseNum_Human_Warrior = 18;
        M2Config.dwUseItemIntervalTime = 500;
        M2Config.dwUseAttackItemIntervalTime = 500;
        M2Config.dwUseOrdinaryTime_Human_Warrior = 300;
        M2Config.dwUseSpecialTime_Human_Warrior = 1000;
        M2Config.dwHitFrameTime = 700;
        M2Config.nIncHealingLimite = 300;
        M2Config.nPerHealing = 5;
        M2Config.nPerHealingTime = 700;
        M2Config.nBigPerHealing = 5;
        M2Config.nBigPerHealingTime = 700;
        M2Config.nBetterItemX = 550;
        M2Config.nBetterItemY = 350;
        M2Config.nSmallInfoX = 10;
        M2Config.nSmallInfoY = 280;
        M2Config.nJoyStickX = 90;
        M2Config.nJoyStickY = 100;
        M2Config.nJoyStickMaxX = 320;
        M2Config.nJoyStickMaxY = 280;
        M2Config.nSkillCtrX = 35;
        M2Config.nSkillCtrY = 60;
        M2Config.btMapScale = 13;
        M2Config.btGuiScale = 11;
        M2Config.btMultiViewRange = 5;
        M2Config.nTitleFileIndex = -1;
        M2Config.boBagRightkey = true;
        M2Config.dwMinMapFlagFlash = 300;
        M2Config.nHealthNumberOffsetX = 0;
        M2Config.nNewLeftGroupInfoOffsetY = 0;
        M2Config.nHumHPBarOffsetX = 0;
        M2Config.nMonNameOffsetY = 0;
        M2Config.sShowHintFontName = "宋体";
        M2Config.NewAbilShowStateDlg.AsSpan().Fill(false);
        M2Config.boShowItemFromFields.AsSpan().Fill(true);

        _form = StaRunner.New(() =>
        {
            var f = new ConfigClientForm
            {
                SendServerConfigHandler = () => _sendServerConfigCalls++,
            };
            f.FormCreate();
            return f;
        });
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    private string IniText()
        => File.ReadAllText(Path.Combine(_tempDir, "!Setup.txt"), Encoding.GetEncoding("GBK"));

    [Fact]
    public void Restore_RhythmWriteBacks_WithAndWithoutFlag()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            _form.sePerHealth.Value = 22;
            _form.SePerHealthChange();
            Assert.Equal(22, M2Config.nPerHealth);
            Assert.False(_form.boSendServerConfigPublic); // 无标志

            _form.seUseOrdinaryTime_Human_Warrior.Value = 400;
            _form.SeUseOrdinaryTime_Human_WarriorChange();
            Assert.Equal(400u, M2Config.dwUseOrdinaryTime_Human_Warrior);
            Assert.True(_form.boSendServerConfigPublic);  // Use* 八项带标志

            // 小地图色带标志
            _form.seMinMapColorSelf.Value = 111;
            _form.SeMinMapColorSelfChange();
            Assert.Equal(111, M2Config.btMinMapColorSelf);
        });
    }

    [Fact]
    public void DrugAndRestoreSave_WritesIni_WithFlagGate()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            _form.ChkUseFindPathClick(); // 置 boSendServerConfig
            M2Config.nHealthFillTime_Human_Warrior = 360;
            _form.BtnSaveDrugAndRestoreClick();

            var ini = IniText();
            Assert.Contains("PerHealth=10", ini);
            Assert.Contains("HealthFillTime_Human_Warrior=360", ini);
            Assert.Contains("UseSpecialTime_Human_Warrior=1000", ini);
            Assert.Contains("BigPerHealingTime=700", ini);
            Assert.Equal(1, _sendServerConfigCalls); // 标志门：一次下发

            _form.BtnSaveDrugAndRestoreClick();
            Assert.Equal(1, _sendServerConfigCalls); // 标志已清 → 不再下发
            Assert.False(_form.btnSaveDrugAndRestore.Enabled);
        });
    }

    [Fact]
    public void Option2Save_WritesMinMapAndOffsets()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            M2Config.boShowExSkillIcon = true;
            M2Config.btMinMapColorSelf = 200;
            _form.BtnSaveOption2Click();

            var ini = IniText();
            Assert.Contains("UseFindPath=1", ini);
            Assert.Contains("MinMapColorSelf=200", ini);
            Assert.Contains("ShowExSkillIcon=1", ini);
            Assert.Contains("MultiViewRange=5", ini);
            Assert.Contains("BetterItemX=550", ini);
            Assert.Contains("HealthNumberText=0", ini);
            Assert.False(_form.btnSaveOption2.Enabled);
        });
    }

    [Fact]
    public void ArrButtonSave_WritesSectionAndDisables()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            var crcBefore = M2Config.g_ArrButtonConfigCRC;
            M2Config.g_ArrButtonConfig[2].HorzAligment = 2;
            M2Config.g_ArrButtonConfig[2].NextOffsetY = 9;
            _form.btnArrBtnSetting.Enabled = true;
            _form.BtnArrBtnSettingClick();

            var ini = IniText();
            Assert.Contains("HorzAlign2=2", ini);
            Assert.Contains("NextOffsetY2=9", ini);
            Assert.NotEqual(crcBefore, M2Config.g_ArrButtonConfigCRC);
            Assert.False(_form.btnArrBtnSetting.Enabled);
        });
    }

    [Fact]
    public void RestoreDefault_ResetsControlsAndModifies()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            _form.uModValue();
            _form.seUseOrdinaryTime_Human_Warrior.Value = 999;
            _form.sePerHealth.Value = 99;
            _form.BtnRestoreDefaultClick();
            Assert.Equal(300, _form.seUseOrdinaryTime_Human_Warrior.Value);
            Assert.Equal(10, _form.sePerHealth.Value);
            Assert.Equal(450, _form.seHealthFillTime.Value);
            Assert.Equal(5, _form.sePerHealing.Value);
            Assert.True(_form.ButtonPrguseSave.Enabled); // ModValue
        });
    }

    [Fact]
    public void HealthNumberTextClick_EnablesLinked()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            _form.chkHealthNumberText.Checked = true;
            _form.ChkHealthNumberTextClick();
            Assert.True(M2Config.boHealthNumberText);
            Assert.True(_form.chkBlastHitShowHealthNum.Enabled);
            Assert.False(_form.chkHPStoneHideHealthNum.Enabled);
            Assert.False(_form.chkMPStoneHideHealthNum.Enabled);
            Assert.True(_form.boSendServerConfigPublic);
        });
    }

    [Fact]
    public void UseOldSerialWindows_LinksStateGroup()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            _form.CheckBoxUseOldSerialWindows.Checked = true;
            _form.CheckBoxUseOldSerialWindowsClick();
            Assert.True(M2Config.boUseOldSerialWindows);
            Assert.True(_form.rgStateWindows[1].Enabled);

            _form.rgStateWindows[1].Checked = true;
            _form.RgStateWindowsClick();
            Assert.Equal(1, M2Config.boStateWindowsType);
        });
    }

    [Fact]
    public void TagIndexWriteBacks()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            _form.chkItemFromField[3].Checked = false;
            _form.ChkItemFromFieldClick(3);
            Assert.False(M2Config.boShowItemFromFields[3]);

            _form.CheckGroupNewAbil.SetItemChecked(5, true);
            _form.CheckGroupNewAbilChange(5);
            Assert.True(M2Config.NewAbilShowStateDlg[5]);

            _form.cbbBagRightkey.SelectedIndex = 0;
            _form.CbbBagRightkeyChange();
            Assert.False(M2Config.boBagRightkey);

            _form.cbbTitleFileIndex.Items.Add("T1");
            _form.cbbTitleFileIndex.SelectedIndex = 0;
            _form.CbbTitleFileIndexChange();
            Assert.Equal(0, M2Config.nTitleFileIndex);

            _form.edtShowHintFontName.Text = "黑体";
            _form.EdtShowHintFontNameChange();
            Assert.Equal("黑体", M2Config.sShowHintFontName);
            Assert.True(_form.boSendServerConfigPublic);
        });
    }

    [Fact]
    public void WriteBacks_GatedByOpened()
    {
        StaRunner.New(() =>
        {
            _form.sePerHealth.Value = 66;
            _form.SePerHealthChange();
            Assert.Equal(10, M2Config.nPerHealth); // 未 Open 不写回
            Assert.False(_form.ButtonPrguseSave.Enabled);
        });
    }
}

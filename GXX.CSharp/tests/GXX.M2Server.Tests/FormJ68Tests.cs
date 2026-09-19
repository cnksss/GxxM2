using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J68：ConfigClient.pas 第二片（七个 Save 处理器 + SpecialCmd 增删改 + 逐项写回）。</summary>
public sealed class FormJ68Tests : IDisposable
{
    private readonly string _tempDir;
    private readonly ConfigClientForm _form;
    private int _sendServerConfigCalls;
    private int _sendSpecialCmdCalls;
    private bool _clientItemListSaved;
    private List<string>? _eatItemWriteBack;

    public FormJ68Tests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "j68_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        M2ShareState.ResetForTests(_tempDir);
        M2ConfigClientConfReset();

        _form = StaRunner.New(() =>
        {
            var f = new ConfigClientForm
            {
                SendServerConfigHandler = () => _sendServerConfigCalls++,
                SendSpecialCmdListHandler = () => _sendSpecialCmdCalls++,
                SaveClientItemListHandler = () => _clientItemListSaved = true,
                ClientEatItemNameListWriteBack = list => _eatItemWriteBack = list,
                SaveSpecialCmdListHandler = _ => _specialCmdSaved = true,
            };
            f.FormCreate();
            return f;
        });
    }

    private bool _specialCmdSaved;

    private static void M2ConfigClientConfReset()
    {
        M2Config.btConfigDlgType = 0;
        M2Config.nMoveSpeed = 0;
        M2Config.boNotCanUseClientConfig = false;
        M2Config.boShowHintWindowFrame = true;
        M2Config.sShowHintFontName = "宋体";
        M2Config.boViewFog = false;
        M2Config.btAddItemMsgFColor = 250;
        M2Config.boAddItemMsgXRightToLeft = false;
        M2Config.NewAbilShowStateDlg.AsSpan().Fill(false);
        M2Config.DBotFuncs.AsSpan().Fill(true);
        M2Config.boBagRightkey = true;
        M2Config.btMinMapType = 0;
        M2Config.boUseOldSerialWindows = false;
        M2Config.nTitleFileIndex = -1;
        for (int i = 0; i < 90; i++)
            M2Config.ClientConfigs[i] = i <= 68;
        M2Config.ClientConfigTabSheetVisibles.AsSpan().Fill(true);
        M2Config.boShowItemFromFields.AsSpan().Fill(true);
        M2Config.BrightConfig[0] = 3;
        M2Config.BrightConfig[1] = 3;
        M2Config.BrightConfig[2] = 3;
        M2Config.BrightConfig[3] = 3;
        M2Config.BrightConfig[4] = 0;
        M2Config.BrightConfig[11] = 2;
        M2Config.BrightConfig[12] = 3;
        M2Config.BrightConfig[13] = 3;
        M2Config.BrightConfig[14] = 3;
        M2Config.BrightConfig[15] = 0;
        M2Config.BrightConfig[23] = 2;
        for (int i = 5; i <= 10; i++) M2Config.BrightConfig[i] = 1;
        for (int i = 16; i <= 22; i++) M2Config.BrightConfig[i] = 1;
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    private string IniText()
        => File.ReadAllText(Path.Combine(_tempDir, "!Setup.txt"), System.Text.Encoding.GetEncoding("GBK"));

    [Fact]
    public void GameAuxiliarySave_WritesIniAndNotifies()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            M2Config.btConfigDlgType = 1;
            M2Config.nMoveSpeed = 5;
            M2Config.ClientConfigs[0] = false;
            M2Config.ClientConfigTabSheetVisibles[3] = false;
            _form.ButtonGameAuxiliarySaveClick();

            var ini = IniText();
            Assert.Contains("PlugIn=1", ini);
            Assert.Contains("MoveSpeed=5", ini);
            Assert.Contains("ClientConfig0=0", ini);
            Assert.Contains("ClientConfigTabSheetVisible3=0", ini);
            Assert.Contains("HitFrameTime=700", ini);
            Assert.Contains("GreenHintNewStyle=0", ini);
            Assert.Equal(1, _sendServerConfigCalls);
            Assert.False(_form.ButtonGameAuxiliarySave.Enabled); // uModValue
        });
    }

    [Fact]
    public void HintWindowsSave_WritesFontNameFromEdit()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            _form.edtShowHintFontName.Text = "黑体";
            M2Config.boShowHintWindowFrame = true;
            _form.ButtonClientHintWindowsSaveClick();

            Assert.Equal("黑体", M2Config.sShowHintFontName); // 保存先回写字段
            var ini = IniText();
            Assert.Contains("ShowHintFontName=黑体", ini);
            Assert.Contains("ShowHintWindowFrame=1", ini);
            Assert.Contains("HintWindowBorderWidthLeft=8", ini);
            Assert.Contains("SuspensionShowItem=1", ini);
            Assert.Contains("ShowItemFromFields0=1", ini);
            Assert.Equal(1, _sendServerConfigCalls);
        });
    }

    [Fact]
    public void WeatherSave_WritesBrightConfig()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            M2Config.boViewFog = true;
            M2Config.BrightConfig[4] = 2;
            _form.ButtonWeatherSaveClick();

            var ini = IniText();
            Assert.Contains("ViewFog=1", ini);
            Assert.Contains("BrightConfig4=2", ini);
            Assert.Contains("BrightConfig23=2", ini);
            Assert.Equal(1, _sendServerConfigCalls);
            Assert.False(_form.ButtonWeatherSave.Enabled);
        });
    }

    [Fact]
    public void GameAuxiliarySave2_WritesColorsDirectionsAndTitle()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            M2Config.btAddItemMsgFColor = 9;
            M2Config.boAddItemMsgXRightToLeft = true;
            M2Config.NewAbilShowStateDlg[2] = true;
            M2Config.nHeroUpLevelMsgY = 77;
            _form.ButtonGameAuxiliarySave2Click();

            var ini = IniText();
            Assert.Contains("MonStruckShowNumber=1", ini);
            Assert.Contains("NewAbilShowStateDlg2=1", ini);
            Assert.Contains("AddItemMsgFColor=9", ini);
            Assert.Contains("AddItemMsgXRightToLeft=1", ini);
            Assert.Contains("HeroUpLevelMsgY=77", ini);
            Assert.Contains("TitleFileIndex=-1", ini);
            Assert.Equal(1, _sendServerConfigCalls);
            Assert.False(_form.btnSaveOption2.Enabled);
        });
    }

    [Fact]
    public void ItemName_AddGate_DuplicateGate()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            string? lastMsg = null;
            M2Forms.MessageBoxHandler = (text, _, _) => { lastMsg = text; return 1; };

            _form.ButtonClientItemNameAddClick();
            Assert.Equal("请输入药品名称！", lastMsg);

            _form.EditClientItemName.Text = "  药A  ";
            _form.ButtonClientItemNameAddClick();
            Assert.Equal("药A", _form.ListBoxClientItemName.Items[0].ToString()); // Trim 入表
            Assert.True(_form.ButtonClientItemNameSave.Enabled);

            lastMsg = null;
            _form.EditClientItemName.Text = "药a"; // CompareText 大小写不敏感（中文无大小写，仍命中同名）
            _form.ButtonClientItemNameAddClick();
            Assert.Equal("此药品已经在列表中了！", lastMsg);
        });
    }

    [Fact]
    public void ItemName_UpDownDelSave_TruncateAtNine()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            _form.ListBoxClientItemName.Items.Add("甲");
            _form.ListBoxClientItemName.Items.Add("乙");

            _form.ListBoxClientItemName.SelectedIndex = 1;
            _form.ListBoxClientItemNameClick();
            Assert.True(_form.ButtonClientItemNameeUP.Enabled);
            Assert.True(_form.ButtonClientItemNameDel.Enabled);
            Assert.Equal("乙", _form.EditClientItemName.Text);

            _form.ButtonClientItemNameeUPClick();
            Assert.Equal("乙", _form.ListBoxClientItemName.Items[0].ToString());
            Assert.True(_form.ButtonClientItemNameSave.Enabled);

            _form.ButtonClientItemNameDelClick();
            Assert.Single(_form.ListBoxClientItemName.Items);
            Assert.False(_form.ButtonClientItemNameDel.Enabled);

            // Save：回写全局 + >9 截断
            _form.ClientEatItemNameList.Clear();
            for (int i = 0; i < 12; i++)
                _form.ListBoxClientItemName.Items.Add($"药{i}");
            _form.ListBoxClientItemName.SelectedIndex = -1;
            _form.ButtonClientItemNameSaveClick();
            Assert.Equal(9, _form.ClientEatItemNameList.Count); // g_ClientEatItemNameList >9 截断
            Assert.Equal(9, _eatItemWriteBack!.Count);
            Assert.True(_clientItemListSaved);
            Assert.Equal(1, _sendServerConfigCalls);
        });
    }

    [Fact]
    public void SpecialCmd_AddDupChgDelSave_FullFlow()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            string? lastMsg = null;
            M2Forms.MessageBoxHandler = (text, _, _) => { lastMsg = text; return 1; };

            // 空显示名
            _form.ButtonSpecialCmdAddClick();
            Assert.Equal("请输入显示名称！", lastMsg);

            // 正常添加
            _form.EditSpecialCmdCaption.Text = "领取";
            _form.EditSpecialCmd.Text = "@LINGQU";
            _form.ButtonSpecialCmdAddClick();
            Assert.Single(_form.g_SpecialCmdList);
            Assert.True(_form.ButtonSpecialCmdSave.Enabled);

            // 命令重复
            lastMsg = null;
            _form.EditSpecialCmdCaption.Text = "其它";
            _form.EditSpecialCmd.Text = "@lingqu"; // CompareText
            _form.ButtonSpecialCmdAddClick();
            Assert.Equal("该命令已经存在！", lastMsg);

            // 点击回填
            _form.ListViewSpecialCmd.Items[0].Selected = true;
            _form.ListViewSpecialCmdClick();
            Assert.Equal("领取", _form.EditSpecialCmdCaption.Text);
            Assert.True(_form.ButtonSpecialCmdDel.Enabled);
            Assert.True(_form.ButtonSpecialCmdChg.Enabled);

            // 修改显示名（命令不变 → 无重名检查）
            _form.EditSpecialCmdCaption.Text = "领取奖励";
            _form.ButtonSpecialCmdChgClick();
            Assert.Equal("领取奖励", _form.g_SpecialCmdList[0].sCaption);
            Assert.False(_form.ButtonSpecialCmdChg.Enabled); // Chg 后复位
            Assert.True(_form.ButtonSpecialCmdSave.Enabled);

            // 删除
            _form.ListViewSpecialCmd.Items[0].Selected = true;
            _form.ListViewSpecialCmdClick();
            _form.ButtonSpecialCmdDelClick();
            Assert.Empty(_form.g_SpecialCmdList);
            Assert.False(_form.ButtonSpecialCmdDel.Enabled);
            Assert.True(_form.ButtonSpecialCmdSave.Enabled);

            // 保存
            _form.ButtonSpecialCmdSaveClick();
            Assert.False(_form.ButtonSpecialCmdSave.Enabled);
            Assert.True(_specialCmdSaved);
            Assert.Equal(1, _sendSpecialCmdCalls);
        });
    }

    [Fact]
    public void Bright_ListAndRadioGroupLinkage()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            _form.ListBoxBright.SelectedIndex = 0;
            _form.ListBoxBrightClick();
            Assert.True(_form.RadioGroupBright[3].Checked); // BrightConfig[0]=3 → 黑夜

            _form.RadioGroupBright[2].Checked = true;
            _form.RadioGroupBrightClick();
            Assert.Equal(2, M2Config.BrightConfig[0]); // 值变化 → 写回
            Assert.True(_form.ButtonWeatherSave.Enabled);

            _form.ButtonWeatherSaveClick(); // 保存复位
            _form.RadioGroupBright[2].Checked = true;
            _form.RadioGroupBrightClick();  // 同值 → 不触发 ModValue
            Assert.False(_form.ButtonWeatherSave.Enabled);
        });
    }

    [Fact]
    public void PlugInType_SwitchRefillsTabSheet()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            Assert.Equal(10, _form.RzCheckGroupClientTabSheet.Items.Count); // type=0

            _form.RadioButtonPlugIn2.Checked = true;
            _form.RadioButtonPlugIn2Click();
            Assert.Equal(1, M2Config.btConfigDlgType);
            Assert.Equal(12, _form.RzCheckGroupClientTabSheet.Items.Count); // type=1 十二项
            Assert.Equal("NPC", _form.RzCheckGroupClientTabSheet.Items[5].ToString());
            Assert.True(_form.ButtonPrguseSave.Enabled); // ModValue

            _form.RadioButtonPlugIn1.Checked = true;
            _form.RadioButtonPlugIn1Click();
            Assert.Equal(0, M2Config.btConfigDlgType);
            Assert.Equal(10, _form.RzCheckGroupClientTabSheet.Items.Count);
        });
    }

    [Fact]
    public void CheckBoxWriteBacks_GatedByOpened()
    {
        StaRunner.New(() =>
        {
            // 未 Open：boOpened=false → 勾选不写回、不启用保存
            _form.CheckBoxRankButton.Checked = false;
            _form.CheckBoxRankButtonClick();
            Assert.True(M2Config.boRankButton);
            Assert.False(_form.ButtonPrguseSave.Enabled);

            _form.Open();
            _form.CheckBoxRankButton.Checked = false;
            _form.CheckBoxRankButtonClick();
            Assert.False(M2Config.boRankButton);
            Assert.True(_form.ButtonPrguseSave.Enabled);

            // DBotFunc Tag 写回
            _form.CheckBoxDBotFunc[2].Checked = false;
            _form.CheckBoxDBotFuncClick(2);
            Assert.False(M2Config.DBotFuncs[2]);

            // HomePage Trim 写回
            _form.EditHomePage.Text = "  http://x.com  ";
            _form.EditHomePageChange();
            Assert.Equal("http://x.com", M2Config.sHomePage);

            // 方向复选写回
            _form.chkAddItemMsgXRightToLeft.Checked = true;
            _form.ChkAddItemMsgXRightToLeftClick();
            Assert.True(M2Config.boAddItemMsgXRightToLeft);

            // 页签隐藏写回
            _form.chkHideTabSheet7.Checked = true;
            _form.ChkHideTabSheet7Click();
            Assert.True(M2Config.boHideTabSheet7);
        });
    }

    [Fact]
    public void SpinnerTrackBar_Sync()
    {
        StaRunner.New(() =>
        {
            _form.RzSpinnerMoveSpeed.Value = 5;
            _form.RzSpinnerMoveSpeedChange();
            Assert.Equal(5, _form.TrackBarMoveSpeed.Value);
            _form.RzSpinnerAttackSpeed.Value = -3;
            _form.RzSpinnerAttackSpeedChange();
            Assert.Equal(-3, _form.TrackBarAttackSpeed.Value);
        });
    }
}

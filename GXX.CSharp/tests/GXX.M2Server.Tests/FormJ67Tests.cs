using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J67：ConfigClient.pas TFrmConfigClient 第一片（RefClientConf 全量装载 + PrguseSave + SpecialCmd）。</summary>
public sealed class FormJ67Tests : IDisposable
{
    private readonly ConfigClientForm _form;

    public FormJ67Tests()
    {
        M2ShareState.ResetForTests(Path.GetTempPath());
        M2ConfigClientConfReset();

        _form = StaRunner.New(() =>
        {
            var f = new ConfigClientForm
            {
                EffectImageListHandler = () => new List<string> { "Title1.wzl", "Title2.wzl" },
                ClientEatItemNameListHandler = () => new List<string> { "金创药", "魔法药" },
                SendServerConfigHandler = () => _sendServerConfigCalls++,
            };
            f.FormCreate();
            return f;
        });
    }

    private int _sendServerConfigCalls;

    private static void M2ConfigClientConfReset()
    {
        // 恢复 typed constant 默认（防跨测试静态污染）
        M2Config.boNotCanUseClientConfig = false;
        M2Config.nPerHealingTime = 700;
        M2Config.nBigPerHealingTime = 700;
        M2Config.boBagRightkey = true;
        M2Config.boHealthNumberText = false;
        M2Config.boUseOldSerialWindows = false;
        M2Config.btSuspensionShowItem = 1;
        M2Config.btBagFastItemCompareMode = 0;
        M2Config.boStateWindowsType = 1;
        M2Config.nTitleFileIndex = -1;
        M2Config.sHomePage = "http://www.gxxm2.com";
        M2Config.btMinMapType = 0;
        M2Config.boShowItemFromFields.AsSpan().Fill(true);
        M2Config.DBotFuncs.AsSpan().Fill(true);
        // ClientConfigs typed constant：[0..68]=true、[69..89]=false
        for (int i = 0; i < 90; i++)
            M2Config.ClientConfigs[i] = i <= 68;
        M2Config.NewAbilShowStateDlg.AsSpan().Fill(false);
        M2Config.HintWindowBorderWidth = (8, 8, 8, 8);
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
        M2Config.btShowHintNameFontBold = 0;
        M2Config.btShowHintNameFontStroke = 0;
        M2Config.btShowHintOtherFontBold = 0;
        M2Config.btShowHintOtherFontStroke = 0;
        for (int i = 0; i < 7; i++)
            M2Config.g_ArrButtonConfig[i] = new ArrButtonGroupConfig();
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2ShareState.ResetForTests(null);
    }

    [Fact]
    public void GetBrightString_Table()
    {
        Assert.Equal("日出", ConfigClientForm.GetBrightString(0));
        Assert.Equal("白天", ConfigClientForm.GetBrightString(1));
        Assert.Equal("傍晚", ConfigClientForm.GetBrightString(2));
        Assert.Equal("黑夜", ConfigClientForm.GetBrightString(3));
    }

    [Fact]
    public void FormCreate_FillsTitleFileIndexAndHint()
    {
        StaRunner.New(() =>
        {
            Assert.Equal(2, _form.cbbTitleFileIndex.Items.Count); // g_EffectImageList 名表
            Assert.Equal("游戏中人物二次使用物品间隔时间，此参数默认为 500毫秒。", _form.seUseItemIntervalTime.Tag);
        });
    }

    [Fact]
    public void Open_LoadsDefaultsAndDisablesSaves()
    {
        StaRunner.New(() =>
        {
            _form.Open();
            Assert.Equal(0, _form.ClientPageControl.SelectedIndex);
            Assert.False(_form.ButtonPrguseSave.Enabled);
            Assert.False(_form.ButtonGameAuxiliarySave2.Enabled);
            Assert.True(_form.RadioButtonPlugIn1.Checked);   // btConfigDlgType=0
            Assert.False(_form.RadioButtonPlugIn2.Checked);
            Assert.True(_form.CheckBoxStartGameAuxiliary.Checked);
            Assert.Equal(700, _form.seHitFrameTime.Value);   // dwHitFrameTime
            Assert.Equal(500, _form.seUseItemIntervalTime.Value);
        });
    }

    [Fact]
    public void RefClientConf_DefaultMappings()
    {
        StaRunner.New(() =>
        {
            _form.RefClientConf();

            // 速度三轴：TrackBar 与 Spinner 同步（默认 0）
            Assert.Equal(0, _form.TrackBarMoveSpeed.Value);
            Assert.Equal(0m, _form.RzSpinnerAttackSpeed.Value);
            Assert.Equal("0", _form.TrackBarSpellSpeed.Tag); // Delphi Hint → Tag

            // 内挂选项组：69 勾选 + 21 未勾
            int checkedCount = 0;
            for (int i = 0; i < 90; i++)
            {
                if (_form.RzCheckGroupClientConfig.GetItemChecked(i)) checkedCount++;
            }
            Assert.Equal(69, checkedCount);
            Assert.True(_form.RzCheckGroupClientConfig.GetItemChecked(0));
            Assert.False(_form.RzCheckGroupClientConfig.GetItemChecked(89));
            // btConfigDlgType=0 → 10 项（基本/物品/…/帮助），全部勾选
            Assert.Equal(10, _form.RzCheckGroupClientTabSheet.Items.Count);
            Assert.True(_form.RzCheckGroupClientTabSheet.GetItemChecked(9));
            Assert.Equal("物品", _form.RzCheckGroupClientTabSheet.Items[1].ToString());

            // 悬浮样式 / 背包对比 / 装备栏类型
            Assert.True(_form.RadioGroupShowItemStyle[1].Checked);
            Assert.True(_form.RadioGroupBagFastItemCompare[0].Checked);
            Assert.False(_form.rgStateWindows[1].Enabled); // boUseOldSerialWindows=false

            // 悬浮窗边框 8
            Assert.Equal(8, _form.seHintWindowBorderWidthLeft.Value);

            // 物品来源七字段全勾
            Assert.All(_form.chkItemFromField, c => Assert.True(c.Checked));

            // 内挂功能六项全勾
            Assert.All(_form.CheckBoxDBotFunc, c => Assert.True(c.Checked));

            // 亮度 24 行格式化（0→黑夜、4→日出）
            Assert.Equal(24, _form.ListBoxBright.Items.Count);
            Assert.Equal("0点  黑夜", _form.ListBoxBright.Items[0].ToString());
            Assert.Equal("4点  日出", _form.ListBoxBright.Items[4].ToString());

            // 治愈下限 400（默认 700 不变）
            Assert.Equal(700, _form.sePerHealingTime.Value);

            // 排列按钮七组全缺省（左/上/0）
            Assert.All(_form.cbbArrBtnHorzAlign, c => Assert.Equal(0, c.SelectedIndex));
            Assert.All(_form.seArrBtnOffsetX, c => Assert.Equal(0m, c.Value));
            Assert.False(_form.btnArrBtnSetting.Enabled);

            // 提示坐标（本体/英雄）
            Assert.Equal(30, _form.seAddItemMsgX.Value);
            Assert.Equal(9, _form.seHeroGetExpMsgX.Value);
            Assert.Equal(380, _form.seHeroGetExpMsgY.Value);
        });
    }

    [Fact]
    public void RefClientConf_HealingTimeFloor400()
    {
        StaRunner.New(() =>
        {
            M2Config.nPerHealingTime = 300;
            M2Config.nBigPerHealingTime = 100;
            _form.RefClientConf();
            Assert.Equal(400, M2Config.nPerHealingTime);      // <400 归 400
            Assert.Equal(400, M2Config.nBigPerHealingTime);
            Assert.Equal(400, _form.sePerHealingTime.Value);
        });
    }

    [Fact]
    public void ModValue_TogglesAllSaveButtons()
    {
        StaRunner.New(() =>
        {
            _form.ModValue();
            Assert.True(_form.ButtonPrguseSave.Enabled);
            Assert.True(_form.btnArrBtnSetting.Enabled == false); // btnArrBtnSetting 不受 ModValue 控制
            _form.uModValue();
            Assert.False(_form.ButtonPrguseSave.Enabled);
        });
    }

    [Fact]
    public void ClientPageControlChanging_ConfirmFlow()
    {
        StaRunner.New(() =>
        {
            Assert.True(_form.ClientPageControlChanging()); // 未修改直接放行

            _form.ModValue();
            _form.PageChangeConfirmHandler = () => false;
            Assert.False(_form.ClientPageControlChanging()); // 拒绝 → 阻止切页

            _form.PageChangeConfirmHandler = () => true;
            Assert.True(_form.ClientPageControlChanging());  // 确认 → 放行并复位
            Assert.False(_form.IsModValued && _form.ButtonPrguseSave.Enabled);
        });
    }

    [Fact]
    public void RefSpecialCmd_FillsListAndButtonStates()
    {
        StaRunner.New(() =>
        {
            _form.g_SpecialCmdList.Add(new ConfigClientForm.TClientCmd { sCaption = "领取奖励", sCmd = "@LINGQU" });
            _form.RefSpecialCmd();
            Assert.Single(_form.ListViewSpecialCmd.Items);
            Assert.Equal("领取奖励", _form.ListViewSpecialCmd.Items[0].Text);
            Assert.Equal("@LINGQU", _form.ListViewSpecialCmd.Items[0].SubItems[1].Text);
            Assert.True(_form.ButtonSpecialCmdAdd.Enabled);
            Assert.False(_form.ButtonSpecialCmdDel.Enabled);
            Assert.False(_form.ButtonSpecialCmdChg.Enabled);
        });
    }

    [Fact]
    public void PrguseSave_WritesIniAndSendsServerConfig()
    {
        StaRunner.New(() =>
        {
            M2Config.boShowBagArrange = true;
            M2Config.boViewFog = true;
            M2Config.boGemUpgrade = true;
            M2Config.boNPCGuiCanMove = true;
            _form.ModValue();
            _form.ButtonPrguseSaveClick();

            var iniPath = Path.Combine(Path.GetTempPath(), "!Setup.txt");
            Assert.True(File.Exists(iniPath));
            var content = File.ReadAllText(iniPath);
            Assert.Contains("StartGameAuxiliary=1", content);
            Assert.Contains("ActionLogButton=1", content);
            Assert.Contains("ShowDeputyHeroButton=1", content);
            Assert.Contains("HomePage=http://www.gxxm2.com", content);
            Assert.Contains("DBotFuncs0=1", content);
            Assert.Contains("DBotFuncs5=1", content);
            Assert.Contains("ViewFog=1", content);
            Assert.Contains("GemUpgrade=1", content);
            Assert.Contains("NPCGuiCanMove=1", content);

            Assert.Equal(1, _sendServerConfigCalls);   // UserEngine.SendServerConfig
            Assert.False(_form.ButtonPrguseSave.Enabled); // uModValue
        });
    }

    [Fact]
    public void M2Config_TypedConstantDefaults()
    {
        M2ConfigClientConfReset();
        // typed constant 锁定（M2Share.pas g_Config 常量块）
        Assert.True(M2Config.ClientConfigs[0]);
        Assert.True(M2Config.ClientConfigs[69] == false);
        Assert.False(M2Config.ClientConfigs[89]);
        Assert.Equal(3, M2Config.BrightConfig[0]);
        Assert.Equal(2, M2Config.BrightConfig[11]);
        Assert.Equal(0, M2Config.BrightConfig[15]);
        Assert.Equal(2, M2Config.BrightConfig[23]);
        Assert.Equal((byte)8, M2Config.HintWindowBorderWidth.Left);
        Assert.Equal((byte)8, M2Config.HintWindowBorderWidth.Bottom);
        Assert.Equal(-1, M2Config.nTitleFileIndex);
        Assert.Equal(1, M2Config.boStateWindowsType);
        Assert.Equal(255, M2Config.btMinMapColorSelf);
        Assert.Equal(222, M2Config.btMinMapColorNPC);
        Assert.Equal((byte)13, M2Config.btMapScale);
        Assert.Equal((byte)11, M2Config.btGuiScale);
        Assert.Equal(700u, M2Config.dwHitFrameTime);
        Assert.Equal(500u, M2Config.dwUseItemIntervalTime);
        Assert.Equal(1000u, M2Config.dwUseSpecialTime_Human_Warrior);
        Assert.Equal("宋体", M2Config.sShowHintFontName);
        Assert.Equal((byte)10, M2Config.btShowHintNameFontSize);
        Assert.Equal((byte)9, M2Config.btShowHintOtherFontSize);
        Assert.Equal(550, M2Config.nBetterItemX);
        Assert.Equal(90, M2Config.nJoyStickX);
        Assert.Equal(320, M2Config.nJoyStickMaxX);
        Assert.Equal(300, M2Config.nIncHealingLimite);
    }
}

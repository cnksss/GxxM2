using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ConfigClient.pas TFrmConfigClient 第一片（批次J67）：FormCreate/Open/ModValue/uModValue/
/// ClientPageControlChanging（切页确认接缝）/RefClientConf 全量装载（g_Config → 控件 1:1，
/// ConfigClient.pas 945-1395）/RefSpecialCmd（g_SpecialCmdList → ListView）/GetBrightString/
/// ButtonPrguseSaveClick（Setup ini 写入 + SendServerConfig 接缝 + uModValue，1427-1464）。
/// 其余 Save 按钮（GameAuxiliary/ClientHintWindows/ItemName/Weather/GameAuxiliary2/DrugAndRestore/Option2）
/// 随后续片接入。
/// </summary>
public sealed class ConfigClientForm : System.Windows.Forms.Form
{
    private bool boOpened;
    private bool boModValued;
    private bool boSendServerConfig;

    /// <summary>boModValued 观测位（测试）。</summary>
    public bool IsModValued => boModValued;

    /// <summary>boSendServerConfig 观测位（测试）。</summary>
    public bool boSendServerConfigPublic => boSendServerConfig;

    // ---- 控件（RefClientConf/PrguseSave 引用全集，WinForms 镜像）----
    public System.Windows.Forms.TabControl ClientPageControl = null!;
    public System.Windows.Forms.RadioButton RadioButtonPlugIn1 = null!, RadioButtonPlugIn2 = null!;
    public System.Windows.Forms.CheckBox CheckBoxStartGameAuxiliary = null!, CheckBoxCanOpenGameConfigDlg = null!,
        CheckBoxNotCanUseClientConfig = null!, chkGreenHintNewStyle = null!;
    public System.Windows.Forms.NumericUpDown seMoveFrameTime = null!, seHitFrameTime = null!, seMagicHitFrameTime = null!,
        sePluginPickupTime = null!, sePluginMinEatItemTime = null!, seIncSpeedDecInterval = null!,
        seIncMoveSpeedDecInterval = null!, seIncSpellSpeedDecInterval = null!;
    public System.Windows.Forms.CheckBox CheckBoxFriendButton = null!, CheckBoxControlHelpButton = null!,
        CheckBoxRankButton = null!, CheckBoxWhisperButton = null!, CheckBoxActionLogButton = null!,
        CheckBoxMissionButton = null!, CheckBoxWebButton = null!, CheckBoxOpenShopButton = null!,
        CheckBoxUserShopButton = null!, CheckBoxShowMerchantDlgHelp = null!, CheckBoxChallengeButton = null!;
    public System.Windows.Forms.CheckBox chkShowGlory = null!, chkShowHorseButton = null!, chkShowDeputyHeroButton = null!,
        chkShowBagArrange = null!, chkKeyTabGetActor = null!, chkShowMagicShieldHP = null!;
    public System.Windows.Forms.NumericUpDown seHumHPBarOffsetX = null!, seHumHPBarOffsetY = null!,
        seNpcHPBarOffsetX = null!, seNpcHPBarOffsetY = null!, seMonHPBarOffsetX = null!, seMonHPBarOffsetY = null!,
        seHumNameOffsetX = null!, seHumNameOffsetY = null!, seNpcNameOffsetX = null!, seNpcNameOffsetY = null!,
        seMonNameOffsetX = null!, seMonNameOffsetY = null!;
    public System.Windows.Forms.CheckBox chkHealthNumberText = null!, chkBlastHitShowHealthNum = null!,
        chkPoisoningHideHealthNum = null!, chkHPStoneHideHealthNum = null!, chkMPStoneHideHealthNum = null!,
        chkMagicSetDir = null!;
    public System.Windows.Forms.NumericUpDown seHealthNumberOffsetX = null!, seHealthNumberOffsetY = null!,
        seHealthNumberMoveSpeed = null!, seNewLeftGroupInfoOffsetX = null!, seNewLeftGroupInfoOffsetY = null!;
    public System.Windows.Forms.NumericUpDown seHintWindowBGColor = null!, seHintWindowBGAlpha = null!,
        seHintWindowBorderWidthLeft = null!, seHintWindowBorderWidthTop = null!, seHintWindowBorderWidthRight = null!,
        seHintWindowBorderWidthBottom = null!;
    public System.Windows.Forms.CheckBox chkShowHintWindowFrame = null!, chkShowHintLines = null!;
    public System.Windows.Forms.TextBox edtShowHintFontName = null!;
    public System.Windows.Forms.NumericUpDown seShowHintNameFontSize = null!, seShowHintOtherFontSize = null!;
    public System.Windows.Forms.ComboBox cbbShowHintNameFontBold = null!, cbbShowHintNameFontStroke = null!,
        cbbShowHintOtherFontBold = null!, cbbShowHintOtherFontStroke = null!;
    public System.Windows.Forms.TrackBar TrackBarMoveSpeed = null!, TrackBarAttackSpeed = null!, TrackBarSpellSpeed = null!;
    public System.Windows.Forms.NumericUpDown RzSpinnerMoveSpeed = null!, RzSpinnerAttackSpeed = null!, RzSpinnerSpellSpeed = null!;
    public System.Windows.Forms.CheckedListBox RzCheckGroupClientConfig = null!, RzCheckGroupClientTabSheet = null!;
    public System.Windows.Forms.RadioButton[] RadioGroupShowItemStyle = new System.Windows.Forms.RadioButton[3];
    public System.Windows.Forms.RadioButton[] RadioGroupBagFastItemCompare = new System.Windows.Forms.RadioButton[4];
    public System.Windows.Forms.ListBox ListBoxClientItemName = null!;
    public System.Windows.Forms.Button ButtonClientItemNameDel = null!;
    public System.Windows.Forms.CheckBox CheckBoxOpenHeroButton = null!, CheckBoxEscCloseNPC = null!,
        chkHintWithMouse = null!, chkNpcDlgHintWithMouse = null!, chkHelmetShowInBox = null!;
    public System.Windows.Forms.CheckBox chkShowItemForm = null!, chkShowItemSellPrice = null!, chkShowInsuranceInfo = null!;
    public System.Windows.Forms.CheckBox chkShowExSkillIcon = null!, chkHideIconWithHideTitle = null!;
    public System.Windows.Forms.NumericUpDown seShowItemFormColor = null!, seShowItemSellPriceColor = null!, seShowInsuranceInfoColor = null!;
    public System.Windows.Forms.CheckBox[] chkItemFromField = new System.Windows.Forms.CheckBox[7];
    public System.Windows.Forms.CheckBox chkShowNormalFashion = null!, chkShowFashionHideShield = null!,
        chkShowFashionHideHats = null!, chkFashionJewelryOpen = null!;
    public System.Windows.Forms.TextBox EditHomePage = null!;
    public System.Windows.Forms.CheckBox[] CheckBoxDBotFunc = new System.Windows.Forms.CheckBox[6];
    public System.Windows.Forms.CheckBox CheckBoxViewFog = null!;
    public System.Windows.Forms.ListBox ListBoxBright = null!;
    public System.Windows.Forms.Button ButtonSpecialCmdAdd = null!, ButtonSpecialCmdDel = null!,
        ButtonSpecialCmdChg = null!, ButtonSpecialCmdSave = null!;
    public System.Windows.Forms.ListView ListViewSpecialCmd = null!;
    public System.Windows.Forms.CheckBox CheckBoxMonStruckShowNumber = null!, CheckBoxHumStruckShowNumber = null!,
        CheckBoxCloseBookProtect = null!, CheckBoxCloseLogoutProtect = null!;
    public System.Windows.Forms.ComboBox cbbBagRightkey = null!;
    public System.Windows.Forms.CheckBox CheckBoxGetExpMsgAddChatBoardMsg = null!;
    public System.Windows.Forms.CheckBox chkAddItemMsgXRightToLeft = null!, chkAddItemMsgYBottomToTop = null!,
        chkGetExpMsgXRightToLeft = null!, chkGetExpMsgYBottomToTop = null!, chkUpLevelMsgXRightToLeft = null!,
        chkUpLevelMsgYBottomToTop = null!, chkHeroAddItemMsgXRightToLeft = null!, chkHeroAddItemMsgYBottomToTop = null!,
        chkHeroGetExpMsgXRightToLeft = null!, chkHeroGetExpMsgYBottomToTop = null!, chkHeroUpLevelMsgXRightToLeft = null!,
        chkHeroUpLevelMsgYBottomToTop = null!, chkHideTabSheet2 = null!, chkHeroHideTabSheet2 = null!,
        chkHideTabSheet5 = null!, chkHeroHideTabSheet5 = null!, chkHideTabSheet7 = null!;
    public System.Windows.Forms.CheckedListBox CheckGroupNewAbil = null!;
    public System.Windows.Forms.NumericUpDown seAddItemMsgFColor = null!, seAddItemMsgBColor = null!,
        seGetExpMsgFColor = null!, seGetExpMsgBColor = null!, seUpLevelMsgFColor = null!, seUpLevelMsgBColor = null!,
        seAddItemMsgX = null!, seAddItemMsgY = null!, seGetExpMsgX = null!, seGetExpMsgY = null!,
        seUpLevelMsgX = null!, seUpLevelMsgY = null!;
    public System.Windows.Forms.NumericUpDown seHeroAddItemMsgFColor = null!, seHeroAddItemMsgBColor = null!,
        seHeroGetExpMsgFColor = null!, seHeroGetExpMsgBColor = null!, seHeroUpLevelMsgFColor = null!,
        seHeroUpLevelMsgBColor = null!, seHeroAddItemMsgX = null!, seHeroAddItemMsgY = null!,
        seHeroGetExpMsgX = null!, seHeroGetExpMsgY = null!, seHeroUpLevelMsgX = null!, seHeroUpLevelMsgY = null!;
    public System.Windows.Forms.CheckBox chkShowBagGameGoldSeparator = null!, chkShowBagGameInfo = null!,
        chkShowHeroShortKey = null!;
    public System.Windows.Forms.NumericUpDown seShowHeroShortKeyX = null!, seShowHeroShortKeyY = null!;
    public System.Windows.Forms.CheckBox chkUseFindPath = null!, CheckBoxUseOldSerialWindows = null!;
    public System.Windows.Forms.RadioButton[] rgStateWindows = new System.Windows.Forms.RadioButton[2];
    public System.Windows.Forms.CheckBox chkMoveItemShowID = null!;
    public System.Windows.Forms.ComboBox cbbTitleFileIndex = null!;
    public System.Windows.Forms.NumericUpDown sePerHealth = null!, sePerSpell = null!, seIncHealthSpell = null!,
        seHealthFillTime = null!, seHealthFillTime_Human_Warrior = null!, seHealthFillTime_Human_TaoistAndWizard = null!,
        seHealthFillTime_Hero_Warrior = null!, seHealthFillTime_Hero_TaoistAndWizard = null!, seSpellFillTime = null!,
        seSpellFillTime_Human_Warrior = null!, seSpellFillTime_Human_TaoistAndWizard = null!,
        seSpellFillTime_Hero_Warrior = null!, seSpellFillTime_Hero_TaoistAndWizard = null!, seHealthBaseNum = null!,
        seSpellBaseNum = null!, seHealthBaseNum_Human_Warrior = null!, seHealthBaseNum_Human_TaoistAndWizard = null!,
        seHealthBaseNum_Hero_Warrior = null!, seHealthBaseNum_Hero_TaoistAndWizard = null!, seSpellBaseNum_Human_Warrior = null!,
        seSpellBaseNum_Human_TaoistAndWizard = null!, seSpellBaseNum_Hero_Warrior = null!, seSpellBaseNum_Hero_TaoistAndWizard = null!,
        seUseItemIntervalTime = null!, seUseAttackItemIntervalTime = null!, seUseOrdinaryTime_Human_Warrior = null!,
        seUseSpecialTime_Human_Warrior = null!, seUseOrdinaryTime_Human_TaoistAndWizard = null!,
        seUseSpecialTime_Human_TaoistAndWizard = null!, seUseOrdinaryTime_Hero_Warrior = null!,
        seUseSpecialTime_Hero_Warrior = null!, seUseOrdinaryTime_Hero_TaoistAndWizard = null!,
        seUseSpecialTime_Hero_TaoistAndWizard = null!;
    public System.Windows.Forms.CheckBox chkMinMapCloseRadar = null!, chkMinMapUseFindPath = null!, chkLoginShowMinMap = null!;
    public System.Windows.Forms.ComboBox cbbMinMapType = null!;
    public System.Windows.Forms.NumericUpDown seMinMapFlagFlash = null!, seMinMapColorSelf = null!, seMinMapColorOther = null!,
        seMinMapColorNPC = null!, seMinMapColorGuard = null!, seMinMapColorMonster = null!, seMinMapColorHero = null!,
        seMinMapColorBoss = null!;
    public System.Windows.Forms.CheckBox chkHideItemNameNum = null!, chkGemUpgrade = null!, chkShowGuildName = null!,
        chkShopGuiCanMove = null!, chkNPCGuiCanMove = null!;
    public System.Windows.Forms.NumericUpDown seIncHealingLimite = null!, sePerHealing = null!, sePerHealingTime = null!,
        seBigPerHealing = null!, seBigPerHealingTime = null!, seThrowAwayItemColor = null!;
    public System.Windows.Forms.ComboBox[] cbbArrBtnHorzAlign = new System.Windows.Forms.ComboBox[7];
    public System.Windows.Forms.ComboBox[] cbbArrBtnVertAlign = new System.Windows.Forms.ComboBox[7];
    public System.Windows.Forms.NumericUpDown[] seArrBtnOffsetX = new System.Windows.Forms.NumericUpDown[7];
    public System.Windows.Forms.NumericUpDown[] seArrBtnOffsetY = new System.Windows.Forms.NumericUpDown[7];
    public System.Windows.Forms.NumericUpDown[] seNextArrBtnOffsetX = new System.Windows.Forms.NumericUpDown[7];
    public System.Windows.Forms.NumericUpDown[] seNextArrBtnOffsetY = new System.Windows.Forms.NumericUpDown[7];
    public System.Windows.Forms.Button btnArrBtnSetting = null!;
    public System.Windows.Forms.NumericUpDown seBetterItemX = null!, seBetterItemY = null!, seSmallInfoX = null!,
        seSmallInfoY = null!, seJoyStickX = null!, seJoyStickY = null!, seJoyStickMaxX = null!, seJoyStickMaxY = null!,
        seSkillCtrX = null!, seSkillCtrY = null!, seMapScale = null!, seGuiScale = null!, seMultiViewRange = null!;
    public System.Windows.Forms.CheckBox chkShowMulitDlg = null!, chkShowBetterItem = null!;
    public System.Windows.Forms.Button ButtonPrguseSave = null!, ButtonGameAuxiliarySave = null!,
        ButtonClientHintWindowsSave = null!, ButtonClientItemNameSave = null!, ButtonWeatherSave = null!,
        ButtonGameAuxiliarySave2 = null!, btnSaveDrugAndRestore = null!, btnSaveOption2 = null!;

    // ---- 接缝 ----
    /// <summary>g_EffectImageList（称号素材名表）。</summary>
    public Func<List<string>>? EffectImageListHandler;
    /// <summary>g_ClientEatItemNameList（客户端物品名表；全局随后续批次接入）。</summary>
    public Func<List<string>>? ClientEatItemNameListHandler;
    /// <summary>UserEngine.SendServerConfig。</summary>
    public Action? SendServerConfigHandler;
    /// <summary>ClientPageControlChanging 的确认框（Application.MessageBox）。</summary>
    public Func<bool>? PageChangeConfirmHandler;
    /// <summary>g_SpecialCmdList（TList of TClientCmd）。</summary>
    public readonly List<TClientCmd> g_SpecialCmdList = new();

    // ---- 批次J68 第二片新增控件 ----
    public System.Windows.Forms.TextBox EditClientItemName = null!;
    public System.Windows.Forms.Button ButtonClientItemNameeUP = null!, ButtonClientItemNameDown = null!;
    public System.Windows.Forms.TextBox EditSpecialCmdCaption = null!, EditSpecialCmd = null!;
    public System.Windows.Forms.RadioButton[] RadioGroupBright = new System.Windows.Forms.RadioButton[4];

    // ---- 批次J68 第二片新增接缝 ----
    /// <summary>g_ClientEatItemNameList（Delphi 单元全局；窗体内镜像实例）。</summary>
    public readonly List<string> ClientEatItemNameList = new();
    /// <summary>全局表写回接缝（g_ClientEatItemNameList 真实全局接入时替换）。</summary>
    public Action<List<string>>? ClientEatItemNameListWriteBack;
    /// <summary>SaveClientItemList（ClientEatItemNameList.txt 落盘）。</summary>
    public Action? SaveClientItemListHandler;
    /// <summary>SaveSpecialCmdList（SpecialCmdList.txt 落盘 + 打包 CRC 接缝）。</summary>
    public Action<List<TClientCmd>>? SaveSpecialCmdListHandler;
    /// <summary>UserEngine.SendSpecialCmdList。</summary>
    public Action? SendSpecialCmdListHandler;

    public sealed class TClientCmd
    {
        public string sCmd = "";
        public string sCaption = "";
    }

    public ConfigClientForm()
    {
        // 反射兜底：单控件字段全部实例化（数组字段已在声明处定长，由下方显式循环装填）
        foreach (var field in typeof(ConfigClientForm).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            if (field.GetValue(this) != null || field.FieldType.IsArray)
                continue;
            if (typeof(System.Windows.Forms.Control).IsAssignableFrom(field.FieldType))
            {
                var ctrl = (System.Windows.Forms.Control?)Activator.CreateInstance(field.FieldType);
                if (ctrl is System.Windows.Forms.NumericUpDown num)
                {
                    num.Minimum = -1000000000;
                    num.Maximum = 1000000000;
                }
                field.SetValue(this, ctrl);
            }
        }

        ClientPageControl = new System.Windows.Forms.TabControl();
        Controls.Add(ClientPageControl);

        var spin = () => new System.Windows.Forms.NumericUpDown { Minimum = -1000000000, Maximum = 1000000000 };
        seMoveFrameTime = spin(); seHitFrameTime = spin(); seMagicHitFrameTime = spin();
        sePluginPickupTime = spin(); sePluginMinEatItemTime = spin();
        seIncSpeedDecInterval = spin(); seIncMoveSpeedDecInterval = spin(); seIncSpellSpeedDecInterval = spin();
        seHumHPBarOffsetX = spin(); seHumHPBarOffsetY = spin();
        seNpcHPBarOffsetX = spin(); seNpcHPBarOffsetY = spin();
        seMonHPBarOffsetX = spin(); seMonHPBarOffsetY = spin();
        seHumNameOffsetX = spin(); seHumNameOffsetY = spin();
        seNpcNameOffsetX = spin(); seNpcNameOffsetY = spin();
        seMonNameOffsetX = spin(); seMonNameOffsetY = spin();
        seHealthNumberOffsetX = spin(); seHealthNumberOffsetY = spin(); seHealthNumberMoveSpeed = spin();
        seNewLeftGroupInfoOffsetX = spin(); seNewLeftGroupInfoOffsetY = spin();
        seHintWindowBGColor = spin(); seHintWindowBGAlpha = spin();
        seHintWindowBorderWidthLeft = spin(); seHintWindowBorderWidthTop = spin();
        seHintWindowBorderWidthRight = spin(); seHintWindowBorderWidthBottom = spin();
        seShowHintNameFontSize = spin(); seShowHintOtherFontSize = spin();
        for (int i = 0; i < 3; i++) RadioGroupShowItemStyle[i] = new System.Windows.Forms.RadioButton();
        for (int i = 0; i < 4; i++) RadioGroupBagFastItemCompare[i] = new System.Windows.Forms.RadioButton();
        for (int i = 0; i < 2; i++) rgStateWindows[i] = new System.Windows.Forms.RadioButton();
        for (int i = 0; i < 7; i++) chkItemFromField[i] = new System.Windows.Forms.CheckBox();
        for (int i = 0; i < 6; i++) CheckBoxDBotFunc[i] = new System.Windows.Forms.CheckBox();
        for (int i = 0; i < 4; i++) RadioGroupBright[i] = new System.Windows.Forms.RadioButton();
        EditClientItemName = new();
        ButtonClientItemNameeUP = new() { Enabled = false };
        ButtonClientItemNameDown = new() { Enabled = false };
        EditSpecialCmdCaption = new();
        EditSpecialCmd = new();
        chkShowGlory = new(); chkShowHorseButton = new(); chkShowDeputyHeroButton = new();
        chkShowBagArrange = new(); chkKeyTabGetActor = new(); chkShowMagicShieldHP = new();
        chkHealthNumberText = new(); chkBlastHitShowHealthNum = new();
        chkPoisoningHideHealthNum = new(); chkHPStoneHideHealthNum = new(); chkMPStoneHideHealthNum = new();
        chkMagicSetDir = new();
        chkShowHintWindowFrame = new(); chkShowHintLines = new();
        edtShowHintFontName = new();
        cbbShowHintNameFontBold = new(); cbbShowHintNameFontStroke = new();
        cbbShowHintOtherFontBold = new(); cbbShowHintOtherFontStroke = new();
        foreach (var c in new[] { cbbShowHintNameFontBold, cbbShowHintNameFontStroke, cbbShowHintOtherFontBold, cbbShowHintOtherFontStroke })
            c.Items.AddRange(new object[] { "正常", "粗体" });
        cbbShowHintNameFontStroke.Items.AddRange(new object[] { "无", "描边" });
        cbbShowHintOtherFontStroke.Items.AddRange(new object[] { "无", "描边" });
        TrackBarMoveSpeed = new() { Minimum = -10, Maximum = 10 };
        TrackBarAttackSpeed = new() { Minimum = -10, Maximum = 10 };
        TrackBarSpellSpeed = new() { Minimum = -10, Maximum = 10 };
        RzSpinnerMoveSpeed = spin(); RzSpinnerAttackSpeed = spin(); RzSpinnerSpellSpeed = spin();
        RzCheckGroupClientConfig = new();
        for (int i = 0; i < 90; i++) RzCheckGroupClientConfig.Items.Add($"配置{i}");
        // RzCheckGroupClientTabSheet 条目由 RefClientPlugTableVisible 按 btConfigDlgType 装填
        ListBoxClientItemName = new();
        ButtonClientItemNameDel = new() { Enabled = false };
        seShowItemFormColor = spin(); seShowItemSellPriceColor = spin(); seShowInsuranceInfoColor = spin();
        EditHomePage = new();
        ListBoxBright = new();
        ListViewSpecialCmd = new() { View = System.Windows.Forms.View.Details };
        ListViewSpecialCmd.Columns.Add("标题");
        ListViewSpecialCmd.Columns.Add("命令");
        ButtonSpecialCmdAdd = new(); ButtonSpecialCmdDel = new() { Enabled = false };
        ButtonSpecialCmdChg = new() { Enabled = false }; ButtonSpecialCmdSave = new() { Enabled = false };
        cbbBagRightkey = new();
        cbbBagRightkey.Items.AddRange(new object[] { "默认", "右键" });
        CheckGroupNewAbil = new();
        for (int i = 0; i < 24; i++) CheckGroupNewAbil.Items.Add($"新属性{i}");
        cbbTitleFileIndex = new();
        cbbMinMapType = new();
        cbbMinMapType.Items.AddRange(new object[] { "新式", "老式" });
        sePerHealth = spin(); sePerSpell = spin(); seIncHealthSpell = spin();
        seHealthFillTime = spin(); seHealthFillTime_Human_Warrior = spin(); seHealthFillTime_Human_TaoistAndWizard = spin();
        seHealthFillTime_Hero_Warrior = spin(); seHealthFillTime_Hero_TaoistAndWizard = spin();
        seSpellFillTime = spin(); seSpellFillTime_Human_Warrior = spin(); seSpellFillTime_Human_TaoistAndWizard = spin();
        seSpellFillTime_Hero_Warrior = spin(); seSpellFillTime_Hero_TaoistAndWizard = spin();
        seHealthBaseNum = spin(); seSpellBaseNum = spin();
        seHealthBaseNum_Human_Warrior = spin(); seHealthBaseNum_Human_TaoistAndWizard = spin();
        seHealthBaseNum_Hero_Warrior = spin(); seHealthBaseNum_Hero_TaoistAndWizard = spin();
        seSpellBaseNum_Human_Warrior = spin(); seSpellBaseNum_Human_TaoistAndWizard = spin();
        seSpellBaseNum_Hero_Warrior = spin(); seSpellBaseNum_Hero_TaoistAndWizard = spin();
        seUseItemIntervalTime = spin(); seUseAttackItemIntervalTime = spin();
        seUseOrdinaryTime_Human_Warrior = spin(); seUseSpecialTime_Human_Warrior = spin();
        seUseOrdinaryTime_Human_TaoistAndWizard = spin(); seUseSpecialTime_Human_TaoistAndWizard = spin();
        seUseOrdinaryTime_Hero_Warrior = spin(); seUseSpecialTime_Hero_Warrior = spin();
        seUseOrdinaryTime_Hero_TaoistAndWizard = spin(); seUseSpecialTime_Hero_TaoistAndWizard = spin();
        seMinMapFlagFlash = spin(); seMinMapColorSelf = spin(); seMinMapColorOther = spin(); seMinMapColorNPC = spin();
        seMinMapColorGuard = spin(); seMinMapColorMonster = spin(); seMinMapColorHero = spin(); seMinMapColorBoss = spin();
        seIncHealingLimite = spin(); sePerHealing = spin(); sePerHealingTime = spin();
        seBigPerHealing = spin(); seBigPerHealingTime = spin(); seThrowAwayItemColor = spin();
        for (int i = 0; i < 7; i++)
        {
            cbbArrBtnHorzAlign[i] = new();
            cbbArrBtnHorzAlign[i].Items.AddRange(new object[] { "左", "中", "右" });
            cbbArrBtnVertAlign[i] = new();
            cbbArrBtnVertAlign[i].Items.AddRange(new object[] { "上", "中", "下" });
            seArrBtnOffsetX[i] = spin(); seArrBtnOffsetY[i] = spin();
            seNextArrBtnOffsetX[i] = spin(); seNextArrBtnOffsetY[i] = spin();
        }
        btnArrBtnSetting = new() { Enabled = false };
        seBetterItemX = spin(); seBetterItemY = spin(); seSmallInfoX = spin(); seSmallInfoY = spin();
        seJoyStickX = spin(); seJoyStickY = spin(); seJoyStickMaxX = spin(); seJoyStickMaxY = spin();
        seSkillCtrX = spin(); seSkillCtrY = spin(); seMapScale = spin(); seGuiScale = spin(); seMultiViewRange = spin();

        ButtonPrguseSave = new() { Enabled = false };
        ButtonPrguseSave.Click += (_, _) => ButtonPrguseSaveClick();
        ButtonGameAuxiliarySave = new() { Enabled = false };
        ButtonClientHintWindowsSave = new() { Enabled = false };
        ButtonClientItemNameSave = new() { Enabled = false };
        ButtonWeatherSave = new() { Enabled = false };
        ButtonGameAuxiliarySave2 = new() { Enabled = false };
        btnSaveDrugAndRestore = new() { Enabled = false };
        btnSaveOption2 = new() { Enabled = false };
    }

    /// <summary>GetBrightString（ConfigClient.pas 871）：0 日出/1 白天/2 傍晚/3 黑夜。</summary>
    public static string GetBrightString(int nBright) => nBright switch
    {
        0 => "日出",
        1 => "白天",
        2 => "傍晚",
        3 => "黑夜",
        _ => "",
    };

    public void FormCreate()
    {
        cbbTitleFileIndex.Items.Clear();
        var names = EffectImageListHandler?.Invoke() ?? new List<string>();
        foreach (var name in names)
            cbbTitleFileIndex.Items.Add(name);
        boSendServerConfig = false;
        seUseItemIntervalTime.Tag = "游戏中人物二次使用物品间隔时间，此参数默认为 500毫秒。";
    }

    public void Open()
    {
        boOpened = false;
        uModValue();
        RefClientConf();
        boOpened = true;
        ClientPageControl.SelectedIndex = 0;
    }

    public void ModValue()
    {
        boModValued = true;
        ButtonPrguseSave.Enabled = true;
        ButtonGameAuxiliarySave.Enabled = true;
        ButtonClientHintWindowsSave.Enabled = true;
        ButtonClientItemNameSave.Enabled = true;
        ButtonWeatherSave.Enabled = true;
        ButtonGameAuxiliarySave2.Enabled = true;
        btnSaveDrugAndRestore.Enabled = true;
        btnSaveOption2.Enabled = true;
    }

    public void uModValue()
    {
        boModValued = false;
        ButtonPrguseSave.Enabled = false;
        ButtonGameAuxiliarySave.Enabled = false;
        ButtonClientHintWindowsSave.Enabled = false;
        ButtonClientItemNameSave.Enabled = false;
        ButtonWeatherSave.Enabled = false;
        ButtonGameAuxiliarySave2.Enabled = false;
        btnSaveDrugAndRestore.Enabled = false;
        btnSaveOption2.Enabled = false;
    }

    /// <summary>ClientPageControlChanging（1416）：修改中切页 → 确认放行（uModValue）或阻止。</summary>
    public bool ClientPageControlChanging()
    {
        if (!boModValued)
            return true;
        if (PageChangeConfirmHandler != null && PageChangeConfirmHandler())
        {
            uModValue();
            return true;
        }
        return false;
    }

    /// <summary>RefClientPlugTableVisible（937-962）：按 btConfigDlgType 装填内挂页签组条目。</summary>
    public void RefClientPlugTableVisible()
    {
        RzCheckGroupClientTabSheet.Items.Clear();
        if (M2Config.btConfigDlgType == 0)
        {
            foreach (var name in new[] { "基本", "物品", "保护", "药品", "技能", "按键", "战斗", "挂机", "便签", "帮助" })
                RzCheckGroupClientTabSheet.Items.Add(name);
        }
        else
        {
            foreach (var name in new[] { "基本", "技能", "保护", "战斗", "物品", "NPC", "挂机", "按键", "便签", "帮助", "英雄", "发言" })
                RzCheckGroupClientTabSheet.Items.Add(name);
        }
    }

    /// <summary>RefSpecialCmd（1395-1414）。</summary>
    public void RefSpecialCmd()
    {
        ListViewSpecialCmd.Items.Clear();
        ButtonSpecialCmdAdd.Enabled = true;
        ButtonSpecialCmdDel.Enabled = false;
        ButtonSpecialCmdChg.Enabled = false;
        foreach (var clientCmd in g_SpecialCmdList)
        {
            var item = ListViewSpecialCmd.Items.Add(clientCmd.sCaption);
            item.Tag = clientCmd;
            item.SubItems.Add(clientCmd.sCmd);
        }
    }

    private static int I(bool b) => b ? 1 : 0;

    /// <summary>RefClientConf（945-1394）1:1：g_Config → 控件全量装载。</summary>
    public void RefClientConf()
    {
        RadioButtonPlugIn1.Checked = M2Config.btConfigDlgType == 0;
        RadioButtonPlugIn2.Checked = M2Config.btConfigDlgType == 1;
        RefClientPlugTableVisible();

        CheckBoxStartGameAuxiliary.Checked = M2Config.boStartGameAuxiliary;
        CheckBoxCanOpenGameConfigDlg.Checked = M2Config.boCanOpenGameConfigDlg;
        CheckBoxNotCanUseClientConfig.Checked = M2Config.boNotCanUseClientConfig;
        chkGreenHintNewStyle.Checked = M2Config.boGreenHintNewStyle;

        seMoveFrameTime.Value = M2Config.dwMoveFrameTime;
        seHitFrameTime.Value = M2Config.dwHitFrameTime;
        seMagicHitFrameTime.Value = M2Config.dwMagicHitFrameTime;
        sePluginPickupTime.Value = M2Config.dwPluginPickupTime;
        sePluginMinEatItemTime.Value = M2Config.dwPluginMinEatItemTime;
        seIncSpeedDecInterval.Value = M2Config.dwIncSpeedDecInterval;
        seIncMoveSpeedDecInterval.Value = M2Config.dwIncMoveSpeedDecInterval;
        seIncSpellSpeedDecInterval.Value = M2Config.dwIncSpellSpeedDecInterval;

        CheckBoxFriendButton.Checked = M2Config.boFriendButton;
        CheckBoxControlHelpButton.Checked = M2Config.boControlHelpButton;
        CheckBoxRankButton.Checked = M2Config.boRankButton;
        CheckBoxWhisperButton.Checked = M2Config.boWhisperButton;
        CheckBoxActionLogButton.Checked = M2Config.boActionLogButton;
        CheckBoxMissionButton.Checked = M2Config.boMissionButton;
        CheckBoxWebButton.Checked = M2Config.boWebButton;
        CheckBoxOpenShopButton.Checked = M2Config.boOpenShopButton;
        CheckBoxUserShopButton.Checked = M2Config.boUserShopButton;
        CheckBoxShowMerchantDlgHelp.Checked = M2Config.boShowMerchantDlgHelp;
        CheckBoxChallengeButton.Checked = M2Config.boChallengeButton;
        chkShowGlory.Checked = M2Config.boShowGlory;
        chkShowHorseButton.Checked = M2Config.boShowHorseButton;
        chkShowDeputyHeroButton.Checked = M2Config.boShowDeputyHeroButton;
        chkShowBagArrange.Checked = M2Config.boShowBagArrange;
        chkKeyTabGetActor.Checked = M2Config.boKeyTabGetActor;
        chkShowMagicShieldHP.Checked = M2Config.boShowMagicShieldHP;

        seHumHPBarOffsetX.Value = M2Config.nHumHPBarOffsetX;
        seHumHPBarOffsetY.Value = M2Config.nHumHPBarOffsetY;
        seNpcHPBarOffsetX.Value = M2Config.nNpcHPBarOffsetX;
        seNpcHPBarOffsetY.Value = M2Config.nNpcHPBarOffsetY;
        seMonHPBarOffsetX.Value = M2Config.nMonHPBarOffsetX;
        seMonHPBarOffsetY.Value = M2Config.nMonHPBarOffsetY;
        seHumNameOffsetX.Value = M2Config.nHumNameOffsetX;
        seHumNameOffsetY.Value = M2Config.nHumNameOffsetY;
        seNpcNameOffsetX.Value = M2Config.nNpcNameOffsetX;
        seNpcNameOffsetY.Value = M2Config.nNpcNameOffsetY;
        seMonNameOffsetX.Value = M2Config.nMonNameOffsetX;
        seMonNameOffsetY.Value = M2Config.nMonNameOffsetY;

        chkHealthNumberText.Checked = M2Config.boHealthNumberText;
        chkBlastHitShowHealthNum.Checked = M2Config.boBlastHitShowHealthNum;
        seHealthNumberOffsetX.Value = M2Config.nHealthNumberOffsetX;
        seHealthNumberOffsetY.Value = M2Config.nHealthNumberOffsetY;
        seHealthNumberMoveSpeed.Value = M2Config.nHealthNumberMoveSpeed;
        seNewLeftGroupInfoOffsetX.Value = M2Config.nNewLeftGroupInfoOffsetX;
        seNewLeftGroupInfoOffsetY.Value = M2Config.nNewLeftGroupInfoOffsetY;
        chkPoisoningHideHealthNum.Checked = M2Config.boPoisoningHideHealthNum;
        chkHPStoneHideHealthNum.Checked = M2Config.boHPStoneHideHealthNum;
        chkMPStoneHideHealthNum.Checked = M2Config.boMPStoneHideHealthNum;
        chkMagicSetDir.Checked = M2Config.boMagicSetDir;

        chkBlastHitShowHealthNum.Enabled = chkHealthNumberText.Checked;
        chkHPStoneHideHealthNum.Enabled = !chkHealthNumberText.Checked;
        chkMPStoneHideHealthNum.Enabled = !chkHealthNumberText.Checked;

        seHintWindowBGColor.Value = M2Config.btHintWindowbackgroundColor;
        seHintWindowBGAlpha.Value = M2Config.btHintWindowbackgroundAlpha;
        seHintWindowBorderWidthLeft.Value = M2Config.HintWindowBorderWidth.Left;
        seHintWindowBorderWidthTop.Value = M2Config.HintWindowBorderWidth.Top;
        seHintWindowBorderWidthRight.Value = M2Config.HintWindowBorderWidth.Right;
        seHintWindowBorderWidthBottom.Value = M2Config.HintWindowBorderWidth.Bottom;

        chkShowHintWindowFrame.Checked = M2Config.boShowHintWindowFrame;
        chkShowHintLines.Checked = M2Config.boShowHintLines;
        edtShowHintFontName.Text = M2Config.sShowHintFontName;
        seShowHintNameFontSize.Value = M2Config.btShowHintNameFontSize;
        if (M2Config.btShowHintNameFontBold < cbbShowHintNameFontBold.Items.Count)
            cbbShowHintNameFontBold.SelectedIndex = M2Config.btShowHintNameFontBold;
        if (M2Config.btShowHintNameFontStroke < cbbShowHintNameFontStroke.Items.Count)
            cbbShowHintNameFontStroke.SelectedIndex = M2Config.btShowHintNameFontStroke;
        seShowHintOtherFontSize.Value = M2Config.btShowHintOtherFontSize;
        if (M2Config.btShowHintOtherFontBold < cbbShowHintOtherFontBold.Items.Count)
            cbbShowHintOtherFontBold.SelectedIndex = M2Config.btShowHintOtherFontBold;
        if (M2Config.btShowHintOtherFontStroke < cbbShowHintOtherFontStroke.Items.Count)
            cbbShowHintOtherFontStroke.SelectedIndex = M2Config.btShowHintOtherFontStroke;

        TrackBarMoveSpeed.Value = Math.Clamp(M2Config.nMoveSpeed, TrackBarMoveSpeed.Minimum, TrackBarMoveSpeed.Maximum);
        TrackBarAttackSpeed.Value = Math.Clamp(M2Config.nAttackSpeed, TrackBarAttackSpeed.Minimum, TrackBarAttackSpeed.Maximum);
        TrackBarSpellSpeed.Value = Math.Clamp(M2Config.nSpellSpeed, TrackBarSpellSpeed.Minimum, TrackBarSpellSpeed.Maximum);
        RzSpinnerMoveSpeed.Value = M2Config.nMoveSpeed;
        RzSpinnerAttackSpeed.Value = M2Config.nAttackSpeed;
        RzSpinnerSpellSpeed.Value = M2Config.nSpellSpeed;
        TrackBarMoveSpeed.Tag = TrackBarMoveSpeed.Value.ToString();   // Delphi Hint → Tag 承载（headless 语义位）
        TrackBarAttackSpeed.Tag = TrackBarAttackSpeed.Value.ToString();
        TrackBarSpellSpeed.Tag = TrackBarSpellSpeed.Value.ToString();

        for (int i = 0; i < RzCheckGroupClientConfig.Items.Count; i++)
            RzCheckGroupClientConfig.SetItemChecked(i, M2Config.ClientConfigs[i]);
        for (int i = 0; i < RzCheckGroupClientTabSheet.Items.Count; i++)
            RzCheckGroupClientTabSheet.SetItemChecked(i, M2Config.ClientConfigTabSheetVisibles[i]);

        SetRadio(RadioGroupShowItemStyle, M2Config.btSuspensionShowItem);
        SetRadio(RadioGroupBagFastItemCompare, M2Config.btBagFastItemCompareMode);

        ListBoxClientItemName.Items.Clear();
        foreach (var name in ClientEatItemNameListHandler?.Invoke() ?? new List<string>())
            ListBoxClientItemName.Items.Add(name);
        ButtonClientItemNameDel.Enabled = false;

        CheckBoxOpenHeroButton.Checked = M2Config.boOpenHeroButton;
        CheckBoxEscCloseNPC.Checked = M2Config.boEscCloseNPC;
        chkHintWithMouse.Checked = M2Config.boHintWithMouse;
        chkNpcDlgHintWithMouse.Checked = M2Config.boNpcDlgHintWithMouse;
        chkHelmetShowInBox.Checked = M2Config.boHelmetShowInBox;

        chkShowItemForm.Checked = M2Config.boShowItemForm;
        chkShowItemSellPrice.Checked = M2Config.boShowItemSellPrice;
        chkShowInsuranceInfo.Checked = M2Config.boShowInsuranceInfo;
        seShowItemFormColor.Value = M2Config.btShowItemFormColor;
        seShowInsuranceInfoColor.Value = M2Config.btShowInsuranceInfoColor;
        seShowItemSellPriceColor.Value = M2Config.btShowItemSellPriceColor;
        for (int i = 0; i < 7; i++)
            chkItemFromField[i].Checked = M2Config.boShowItemFromFields[i];

        chkShowNormalFashion.Checked = M2Config.boShowNormalFashion;
        chkShowFashionHideShield.Checked = M2Config.boShowFashionHideShield;
        chkShowFashionHideHats.Checked = M2Config.boShowFashionHideHats;
        chkFashionJewelryOpen.Checked = M2Config.boFashionJewelryOpen;

        EditHomePage.Text = M2Config.sHomePage;

        for (int i = 0; i < 6; i++)
            CheckBoxDBotFunc[i].Checked = M2Config.DBotFuncs[i];
        CheckBoxViewFog.Checked = M2Config.boViewFog;

        ListBoxBright.Items.Clear();
        for (int i = 0; i < M2Config.BrightConfig.Length; i++)
            ListBoxBright.Items.Add($"{i}点  {GetBrightString(M2Config.BrightConfig[i])}");

        ButtonSpecialCmdSave.Enabled = false;
        RefSpecialCmd();

        CheckBoxMonStruckShowNumber.Checked = M2Config.boMonStruckShowNumber;
        CheckBoxHumStruckShowNumber.Checked = M2Config.boHumStruckShowNumber;
        CheckBoxCloseBookProtect.Checked = M2Config.boCloseBookProtect;
        CheckBoxCloseLogoutProtect.Checked = M2Config.boCloseLogoutProtect;

        cbbBagRightkey.SelectedIndex = M2Config.boBagRightkey ? 1 : 0;

        CheckBoxGetExpMsgAddChatBoardMsg.Checked = M2Config.boGetExpMsgAddChatBoardMsg;

        chkAddItemMsgXRightToLeft.Checked = M2Config.boAddItemMsgXRightToLeft;
        chkAddItemMsgYBottomToTop.Checked = M2Config.boAddItemMsgYBottomToTop;
        chkGetExpMsgXRightToLeft.Checked = M2Config.boGetExpMsgXRightToLeft;
        chkGetExpMsgYBottomToTop.Checked = M2Config.boGetExpMsgYBottomToTop;
        chkUpLevelMsgXRightToLeft.Checked = M2Config.boUpLevelMsgXRightToLeft;
        chkUpLevelMsgYBottomToTop.Checked = M2Config.boUpLevelMsgYBottomToTop;
        chkHeroAddItemMsgXRightToLeft.Checked = M2Config.boHeroAddItemMsgXRightToLeft;
        chkHeroAddItemMsgYBottomToTop.Checked = M2Config.boHeroAddItemMsgYBottomToTop;
        chkHeroGetExpMsgXRightToLeft.Checked = M2Config.boHeroGetExpMsgXRightToLeft;
        chkHeroGetExpMsgYBottomToTop.Checked = M2Config.boHeroGetExpMsgYBottomToTop;
        chkHeroUpLevelMsgXRightToLeft.Checked = M2Config.boHeroUpLevelMsgXRightToLeft;
        chkHeroUpLevelMsgYBottomToTop.Checked = M2Config.boHeroUpLevelMsgYBottomToTop;

        chkHideTabSheet2.Checked = M2Config.boHideTabSheet2;
        chkHeroHideTabSheet2.Checked = M2Config.boHeroHideTabSheet2;
        chkHideTabSheet5.Checked = M2Config.boHideTabSheet5;
        chkHeroHideTabSheet5.Checked = M2Config.boHeroHideTabSheet5;
        chkHideTabSheet7.Checked = M2Config.boHideTabSheet7;

        for (int i = 0; i < CheckGroupNewAbil.Items.Count; i++)
            CheckGroupNewAbil.SetItemChecked(i, M2Config.NewAbilShowStateDlg[i]);

        seAddItemMsgFColor.Value = M2Config.btAddItemMsgFColor;
        seAddItemMsgBColor.Value = M2Config.btAddItemMsgBColor;
        seGetExpMsgFColor.Value = M2Config.btGetExpMsgFColor;
        seGetExpMsgBColor.Value = M2Config.btGetExpMsgBColor;
        seUpLevelMsgFColor.Value = M2Config.btUpLevelMsgFColor;
        seUpLevelMsgBColor.Value = M2Config.btUpLevelMsgBColor;
        seAddItemMsgX.Value = M2Config.nAddItemMsgX;
        seAddItemMsgY.Value = M2Config.nAddItemMsgY;
        seGetExpMsgX.Value = M2Config.nGetExpMsgX;
        seGetExpMsgY.Value = M2Config.nGetExpMsgY;
        seUpLevelMsgX.Value = M2Config.nUpLevelMsgX;
        seUpLevelMsgY.Value = M2Config.nUpLevelMsgY;
        seHeroAddItemMsgFColor.Value = M2Config.btHeroAddItemMsgFColor;
        seHeroAddItemMsgBColor.Value = M2Config.btHeroAddItemMsgBColor;
        seHeroGetExpMsgFColor.Value = M2Config.btHeroGetExpMsgFColor;
        seHeroGetExpMsgBColor.Value = M2Config.btHeroGetExpMsgBColor;
        seHeroUpLevelMsgFColor.Value = M2Config.btHeroUpLevelMsgFColor;
        seHeroUpLevelMsgBColor.Value = M2Config.btHeroUpLevelMsgBColor;
        seHeroAddItemMsgX.Value = M2Config.nHeroAddItemMsgX;
        seHeroAddItemMsgY.Value = M2Config.nHeroAddItemMsgY;
        seHeroGetExpMsgX.Value = M2Config.nHeroGetExpMsgX;
        seHeroGetExpMsgY.Value = M2Config.nHeroGetExpMsgY;
        seHeroUpLevelMsgX.Value = M2Config.nHeroUpLevelMsgX;
        seHeroUpLevelMsgY.Value = M2Config.nHeroUpLevelMsgY;

        chkShowBagGameGoldSeparator.Checked = M2Config.boShowBagGameGoldSeparator;
        chkShowBagGameInfo.Checked = M2Config.boShowBagGameInfo;
        chkShowHeroShortKey.Checked = M2Config.boShowHeroShortKey;
        seShowHeroShortKeyX.Value = M2Config.nShowHeroShortKeyX;
        seShowHeroShortKeyY.Value = M2Config.nShowHeroShortKeyY;

        chkUseFindPath.Checked = M2Config.boUseFindPath;
        CheckBoxUseOldSerialWindows.Checked = M2Config.boUseOldSerialWindows;
        rgStateWindows[1].Enabled = CheckBoxUseOldSerialWindows.Checked;
        chkMoveItemShowID.Checked = M2Config.boMoveItemShowID;

        SetRadio(rgStateWindows, M2Config.boStateWindowsType);
        cbbTitleFileIndex.SelectedIndex = Math.Clamp(M2Config.nTitleFileIndex, -1, cbbTitleFileIndex.Items.Count - 1);

        sePerHealth.Value = M2Config.nPerHealth;
        sePerSpell.Value = M2Config.nPerSpell;
        seIncHealthSpell.Value = M2Config.nIncHealthSpellTime;
        seHealthFillTime.Value = M2Config.nHealthFillTime;
        seHealthFillTime_Human_Warrior.Value = M2Config.nHealthFillTime_Human_Warrior;
        seHealthFillTime_Human_TaoistAndWizard.Value = M2Config.nHealthFillTime_Human_TaoistAndWizard;
        seHealthFillTime_Hero_Warrior.Value = M2Config.nHealthFillTime_Hero_Warrior;
        seHealthFillTime_Hero_TaoistAndWizard.Value = M2Config.nHealthFillTime_Hero_TaoistAndWizard;
        seSpellFillTime.Value = M2Config.nSpellFillTime;
        seSpellFillTime_Human_Warrior.Value = M2Config.nSpellFillTime_Human_Warrior;
        seSpellFillTime_Human_TaoistAndWizard.Value = M2Config.nSpellFillTime_Human_TaoistAndWizard;
        seSpellFillTime_Hero_Warrior.Value = M2Config.nSpellFillTime_Hero_Warrior;
        seSpellFillTime_Hero_TaoistAndWizard.Value = M2Config.nSpellFillTime_Hero_TaoistAndWizard;
        seHealthBaseNum.Value = M2Config.nHealthBaseNum;
        seSpellBaseNum.Value = M2Config.nSpellBaseNum;
        seHealthBaseNum_Human_Warrior.Value = M2Config.nHealthBaseNum_Human_Warrior;
        seHealthBaseNum_Human_TaoistAndWizard.Value = M2Config.nHealthBaseNum_Human_TaoistAndWizard;
        seHealthBaseNum_Hero_Warrior.Value = M2Config.nHealthBaseNum_Hero_Warrior;
        seHealthBaseNum_Hero_TaoistAndWizard.Value = M2Config.nHealthBaseNum_Hero_TaoistAndWizard;
        seSpellBaseNum_Human_Warrior.Value = M2Config.nSpellBaseNum_Human_Warrior;
        seSpellBaseNum_Human_TaoistAndWizard.Value = M2Config.nSpellBaseNum_Human_TaoistAndWizard;
        seSpellBaseNum_Hero_Warrior.Value = M2Config.nSpellBaseNum_Hero_Warrior;
        seSpellBaseNum_Hero_TaoistAndWizard.Value = M2Config.nSpellBaseNum_Hero_TaoistAndWizard;
        seUseItemIntervalTime.Value = M2Config.dwUseItemIntervalTime;
        seUseAttackItemIntervalTime.Value = M2Config.dwUseAttackItemIntervalTime;
        seUseOrdinaryTime_Human_Warrior.Value = M2Config.dwUseOrdinaryTime_Human_Warrior;
        seUseSpecialTime_Human_Warrior.Value = M2Config.dwUseSpecialTime_Human_Warrior;
        seUseOrdinaryTime_Human_TaoistAndWizard.Value = M2Config.dwUseOrdinaryTime_Human_TaoistAndWizard;
        seUseSpecialTime_Human_TaoistAndWizard.Value = M2Config.dwUseSpecialTime_Human_TaoistAndWizard;
        seUseOrdinaryTime_Hero_Warrior.Value = M2Config.dwUseOrdinaryTime_Hero_Warrior;
        seUseSpecialTime_Hero_Warrior.Value = M2Config.dwUseSpecialTime_Hero_Warrior;
        seUseOrdinaryTime_Hero_TaoistAndWizard.Value = M2Config.dwUseOrdinaryTime_Hero_TaoistAndWizard;
        seUseSpecialTime_Hero_TaoistAndWizard.Value = M2Config.dwUseSpecialTime_Hero_TaoistAndWizard;

        chkMinMapCloseRadar.Checked = M2Config.boMinMapCloseRadar;
        cbbMinMapType.SelectedIndex = Math.Clamp(M2Config.btMinMapType, 0, cbbMinMapType.Items.Count - 1);
        chkMinMapUseFindPath.Checked = M2Config.boMinMapUseFindPath;
        chkLoginShowMinMap.Checked = M2Config.boLoginShowMinMap;
        seMinMapFlagFlash.Value = M2Config.dwMinMapFlagFlash;
        seMinMapColorSelf.Value = M2Config.btMinMapColorSelf;
        seMinMapColorOther.Value = M2Config.btMinMapColorOther;
        seMinMapColorNPC.Value = M2Config.btMinMapColorNPC;
        seMinMapColorGuard.Value = M2Config.btMinMapColorGuard;
        seMinMapColorMonster.Value = M2Config.btMinMapColorMonster;
        seMinMapColorHero.Value = M2Config.btMinMapColorHero;
        seMinMapColorBoss.Value = M2Config.btMinMapColorBoss;

        chkHideItemNameNum.Checked = M2Config.boHideItemNameNum;
        chkGemUpgrade.Checked = M2Config.boGemUpgrade;
        chkShowGuildName.Checked = M2Config.boShowGuildName;
        chkShopGuiCanMove.Checked = M2Config.boShopGuiCanMove;
        chkNPCGuiCanMove.Checked = M2Config.boNPCGuiCanMove;

        seIncHealingLimite.Value = M2Config.nIncHealingLimite;
        if (M2Config.nPerHealingTime < 400)
            M2Config.nPerHealingTime = 400;
        if (M2Config.nBigPerHealingTime < 400)
            M2Config.nBigPerHealingTime = 400;
        sePerHealing.Value = M2Config.nPerHealing;
        sePerHealingTime.Value = M2Config.nPerHealingTime;
        seBigPerHealing.Value = M2Config.nBigPerHealing;
        seBigPerHealingTime.Value = M2Config.nBigPerHealingTime;

        seThrowAwayItemColor.Value = M2Config.btThrowAwayItemColor;

        for (int i = 0; i < 7; i++)
        {
            cbbArrBtnHorzAlign[i].SelectedIndex = Math.Clamp(M2Config.g_ArrButtonConfig[i].HorzAligment, 0, cbbArrBtnHorzAlign[i].Items.Count - 1);
            seArrBtnOffsetX[i].Value = M2Config.g_ArrButtonConfig[i].OffsetX;
            cbbArrBtnVertAlign[i].SelectedIndex = Math.Clamp(M2Config.g_ArrButtonConfig[i].VertAligment, 0, cbbArrBtnVertAlign[i].Items.Count - 1);
            seArrBtnOffsetY[i].Value = M2Config.g_ArrButtonConfig[i].OffsetY;
            seNextArrBtnOffsetX[i].Value = M2Config.g_ArrButtonConfig[i].NextOffsetX;
            seNextArrBtnOffsetY[i].Value = M2Config.g_ArrButtonConfig[i].NextOffsetY;
        }
        btnArrBtnSetting.Enabled = false;

        seBetterItemX.Value = M2Config.nBetterItemX;
        seBetterItemY.Value = M2Config.nBetterItemY;
        seSmallInfoX.Value = M2Config.nSmallInfoX;
        seSmallInfoY.Value = M2Config.nSmallInfoY;
        seJoyStickX.Value = M2Config.nJoyStickX;
        seJoyStickY.Value = M2Config.nJoyStickY;
        seJoyStickMaxX.Value = M2Config.nJoyStickMaxX;
        seJoyStickMaxY.Value = M2Config.nJoyStickMaxY;
        seSkillCtrX.Value = M2Config.nSkillCtrX;
        seSkillCtrY.Value = M2Config.nSkillCtrY;
        seMapScale.Value = M2Config.btMapScale;
        seGuiScale.Value = M2Config.btGuiScale;

        chkShowMulitDlg.Checked = M2Config.boShowMulitDlg;
        chkShowBetterItem.Checked = M2Config.boShowBetterItem;
        seMultiViewRange.Value = M2Config.btMultiViewRange;
    }

    private static void SetRadio(System.Windows.Forms.RadioButton[] group, int index)
    {
        for (int i = 0; i < group.Length; i++)
        {
            if (group[i] != null)
                group[i].Checked = i == index;
        }
    }

    /// <summary>ButtonPrguseSaveClick（1427-1464）1:1：Setup ini 写入 + SendServerConfig + uModValue。</summary>
    public void ButtonPrguseSaveClick()
    {
        var config = M2ShareState.ConfigIni;
        config.WriteBool("Setup", "StartGameAuxiliary", M2Config.boStartGameAuxiliary);
        config.WriteBool("Setup", "ActionLogButton", M2Config.boActionLogButton);
        config.WriteBool("Setup", "MissionButton", M2Config.boMissionButton);
        config.WriteBool("Setup", "FriendButton", M2Config.boFriendButton);
        config.WriteBool("Setup", "ControlHelpButton", M2Config.boControlHelpButton);
        config.WriteBool("Setup", "RankButton", M2Config.boRankButton);
        config.WriteBool("Setup", "WhisperButton", M2Config.boWhisperButton);
        config.WriteBool("Setup", "OpenShopButton", M2Config.boOpenShopButton);
        config.WriteBool("Setup", "UserShopButton", M2Config.boUserShopButton);
        config.WriteBool("Setup", "WebButton", M2Config.boWebButton);
        config.WriteBool("Setup", "OpenHeroButton", M2Config.boOpenHeroButton);
        config.WriteBool("Setup", "ShowMerchantDlgHelp", M2Config.boShowMerchantDlgHelp);
        config.WriteBool("Setup", "ChallengeButton", M2Config.boChallengeButton);
        config.WriteBool("Setup", "ShowGlory", M2Config.boShowGlory);
        config.WriteBool("Setup", "ShowHorseButton", M2Config.boShowHorseButton);
        config.WriteBool("Setup", "ShowDeputyHeroButton", M2Config.boShowDeputyHeroButton);
        config.WriteBool("Setup", "ShowBagArrange", M2Config.boShowBagArrange);
        config.WriteString("Setup", "HomePage", M2Config.sHomePage);
        for (int i = 0; i < M2Config.DBotFuncs.Length; i++)
            config.WriteBool("Setup", "DBotFuncs" + i, M2Config.DBotFuncs[i]);
        config.WriteBool("Setup", "ViewFog", M2Config.boViewFog);
        config.WriteBool("Setup", "GemUpgrade", M2Config.boGemUpgrade);
        config.WriteBool("Setup", "ShowGuildName", M2Config.boShowGuildName);
        config.WriteBool("Setup", "ShopGuiCanMove", M2Config.boShopGuiCanMove);
        config.WriteBool("Setup", "NPCGuiCanMove", M2Config.boNPCGuiCanMove);

        SendServerConfigHandler?.Invoke();
        config.UpdateFile();
        uModValue();
    }

    // ==================== 批次J68 第二片（ConfigClient.pas 1466-2500） ====================

    /// <summary>逐项写回公共形（if not boOpened then Exit; 赋值; ModValue）。</summary>
    private void WriteBack(Action assign)
    {
        if (!boOpened)
            return;
        assign();
        ModValue();
    }

    /// <summary>ButtonGameAuxiliarySaveClick（1466-1502）：内挂/速度/帧间隔 + ClientConfigs/TabSheet 两组循环写入。</summary>
    public void ButtonGameAuxiliarySaveClick()
    {
        var config = M2ShareState.ConfigIni;
        config.WriteInteger("Setup", "PlugIn", M2Config.btConfigDlgType);
        config.WriteBool("Setup", "StartGameAuxiliary", M2Config.boStartGameAuxiliary);
        config.WriteBool("Setup", "CanOpenGameConfigDlg", M2Config.boCanOpenGameConfigDlg);
        config.WriteBool("Setup", "NotCanUseClientConfig", M2Config.boNotCanUseClientConfig);
        config.WriteBool("Setup", "GreenHintNewStyle", M2Config.boGreenHintNewStyle);

        config.WriteInteger("Setup", "MoveSpeed", M2Config.nMoveSpeed);
        config.WriteInteger("Setup", "AttackSpeed", M2Config.nAttackSpeed);
        config.WriteInteger("Setup", "SpellSpeed", M2Config.nSpellSpeed);
        config.WriteInteger("Setup", "PluginPickupTime", (int)M2Config.dwPluginPickupTime);
        config.WriteInteger("Setup", "PluginMinEatItemTime", (int)M2Config.dwPluginMinEatItemTime);
        config.WriteInteger("Setup", "IncSpeedDecInterval", (int)M2Config.dwIncSpeedDecInterval);
        config.WriteInteger("Setup", "IncMoveSpeedDecInterval", (int)M2Config.dwIncMoveSpeedDecInterval);
        config.WriteInteger("Setup", "IncSpellSpeedDecInterval", (int)M2Config.dwIncSpellSpeedDecInterval);

        for (int i = 0; i < M2Config.ClientConfigs.Length; i++)
            config.WriteBool("Setup", "ClientConfig" + i, M2Config.ClientConfigs[i]);
        for (int i = 0; i < M2Config.ClientConfigTabSheetVisibles.Length; i++)
            config.WriteBool("Setup", "ClientConfigTabSheetVisible" + i, M2Config.ClientConfigTabSheetVisibles[i]);

        config.WriteInteger("Setup", "MoveFrameTime", (int)M2Config.dwMoveFrameTime);
        config.WriteInteger("Setup", "HitFrameTime", (int)M2Config.dwHitFrameTime);
        config.WriteInteger("Setup", "MagicHitFrameTime", (int)M2Config.dwMagicHitFrameTime);

        SendServerConfigHandler?.Invoke();
        config.UpdateFile();
        uModValue();
    }

    /// <summary>ButtonClientHintWindowsSaveClick（1504-1570）：先回写 sShowHintFontName，随后约 40 项 ini。</summary>
    public void ButtonClientHintWindowsSaveClick()
    {
        M2Config.sShowHintFontName = edtShowHintFontName.Text;
        var config = M2ShareState.ConfigIni;
        config.WriteBool("Setup", "ShowHintWindowFrame", M2Config.boShowHintWindowFrame);
        config.WriteBool("Setup", "ShowHintLines", M2Config.boShowHintLines);
        config.WriteInteger("Setup", "HintWindowbackgroundColor", M2Config.btHintWindowbackgroundColor);
        config.WriteInteger("Setup", "HintWindowbackgroundAlpha", M2Config.btHintWindowbackgroundAlpha);
        config.WriteInteger("Setup", "HintWindowBorderWidthLeft", M2Config.HintWindowBorderWidth.Left);
        config.WriteInteger("Setup", "HintWindowBorderWidthTop", M2Config.HintWindowBorderWidth.Top);
        config.WriteInteger("Setup", "HintWindowBorderWidthRight", M2Config.HintWindowBorderWidth.Right);
        config.WriteInteger("Setup", "HintWindowBorderWidthBottom", M2Config.HintWindowBorderWidth.Bottom);
        config.WriteInteger("Setup", "SuspensionShowItem", M2Config.btSuspensionShowItem);
        config.WriteInteger("Setup", "BagFastItemCompareMode", M2Config.btBagFastItemCompareMode);
        config.WriteBool("Setup", "EscCloseNPC", M2Config.boEscCloseNPC);
        config.WriteBool("Setup", "HintWithMouse", M2Config.boHintWithMouse);
        config.WriteBool("Setup", "NpcDlgHintWithMouse", M2Config.boNpcDlgHintWithMouse);
        config.WriteBool("Setup", "HelmetShowInBox", M2Config.boHelmetShowInBox);
        config.WriteBool("Setup", "ShowNormalFashion", M2Config.boShowNormalFashion);
        config.WriteBool("Setup", "ShowFashionHideShield", M2Config.boShowFashionHideShield);
        config.WriteBool("Setup", "ShowFashionHideHats", M2Config.boShowFashionHideHats);
        config.WriteBool("Setup", "FashionJewelryOpen", M2Config.boFashionJewelryOpen);
        config.WriteBool("Setup", "UseOldSerialWindows", M2Config.boUseOldSerialWindows);
        config.WriteInteger("Setup", "boStateWindowsType", M2Config.boStateWindowsType);
        config.WriteBool("Setup", "MoveItemShowID", M2Config.boMoveItemShowID);
        config.WriteBool("Setup", "HideTabSheet2", M2Config.boHideTabSheet2);
        config.WriteBool("Setup", "HeroHideTabSheet2", M2Config.boHeroHideTabSheet2);
        config.WriteBool("Setup", "HideTabSheet5", M2Config.boHideTabSheet5);
        config.WriteBool("Setup", "HeroHideTabSheet5", M2Config.boHeroHideTabSheet5);
        config.WriteBool("Setup", "HideTabSheet7", M2Config.boHideTabSheet7);
        config.WriteInteger("Setup", "ThrowAwayItemColor", M2Config.btThrowAwayItemColor);
        config.WriteString("Setup", "ShowHintFontName", M2Config.sShowHintFontName);
        config.WriteInteger("Setup", "ShowHintNameFontSize", M2Config.btShowHintNameFontSize);
        config.WriteInteger("Setup", "ShowHintNameFontBold", M2Config.btShowHintNameFontBold);
        config.WriteInteger("Setup", "ShowHintNameFontStroke", M2Config.btShowHintNameFontStroke);
        config.WriteInteger("Setup", "ShowHintOtherFontSize", M2Config.btShowHintOtherFontSize);
        config.WriteInteger("Setup", "ShowHintOtherFontBold", M2Config.btShowHintOtherFontBold);
        config.WriteInteger("Setup", "ShowHintOtherFontStroke", M2Config.btShowHintOtherFontStroke);
        config.WriteBool("Setup", "ShowItemForm", M2Config.boShowItemForm);
        config.WriteBool("Setup", "ShowItemSellPrice", M2Config.boShowItemSellPrice);
        config.WriteBool("Setup", "ShowInsuranceInfo", M2Config.boShowInsuranceInfo);
        config.WriteInteger("Setup", "ShowItemFormColor", M2Config.btShowItemFormColor);
        config.WriteInteger("Setup", "ShowInsuranceInfoColor", M2Config.btShowInsuranceInfoColor);
        config.WriteInteger("Setup", "ShowItemSellPriceColor", M2Config.btShowItemSellPriceColor);
        for (int i = 0; i < M2Config.boShowItemFromFields.Length; i++)
            config.WriteBool("Setup", "ShowItemFromFields" + i, M2Config.boShowItemFromFields[i]);

        SendServerConfigHandler?.Invoke();
        config.UpdateFile();
        uModValue();
    }

    /// <summary>ButtonWeatherSaveClick（2039-2050）：ViewFog + BrightConfig×24。</summary>
    public void ButtonWeatherSaveClick()
    {
        var config = M2ShareState.ConfigIni;
        config.WriteBool("Setup", "ViewFog", M2Config.boViewFog);
        for (int i = 0; i < M2Config.BrightConfig.Length; i++)
            config.WriteInteger("Setup", "BrightConfig" + i, M2Config.BrightConfig[i]);
        SendServerConfigHandler?.Invoke();
        config.UpdateFile();
        uModValue();
    }

    /// <summary>ButtonGameAuxiliarySave2Click（2132-2204）：受击显数/保护/提示颜色坐标方向/背包/称号 60 余项。</summary>
    public void ButtonGameAuxiliarySave2Click()
    {
        var config = M2ShareState.ConfigIni;
        config.WriteBool("Setup", "MonStruckShowNumber", M2Config.boMonStruckShowNumber);
        config.WriteBool("Setup", "HumStruckShowNumber", M2Config.boHumStruckShowNumber);
        config.WriteBool("Setup", "CloseBookProtect", M2Config.boCloseBookProtect);
        config.WriteBool("Setup", "CloseLogoutProtect", M2Config.boCloseLogoutProtect);
        config.WriteBool("Setup", "BagRightkey", M2Config.boBagRightkey);
        config.WriteBool("Setup", "GetExpMsgAddChatBoardMsg", M2Config.boGetExpMsgAddChatBoardMsg);
        for (int i = 0; i < M2Config.NewAbilShowStateDlg.Length; i++)
            config.WriteBool("Setup", "NewAbilShowStateDlg" + i, M2Config.NewAbilShowStateDlg[i]);

        config.WriteInteger("Setup", "AddItemMsgFColor", M2Config.btAddItemMsgFColor);
        config.WriteInteger("Setup", "AddItemMsgBColor", M2Config.btAddItemMsgBColor);
        config.WriteInteger("Setup", "GetExpMsgFColor", M2Config.btGetExpMsgFColor);
        config.WriteInteger("Setup", "GetExpMsgBColor", M2Config.btGetExpMsgBColor);
        config.WriteInteger("Setup", "UpLevelMsgFColor", M2Config.btUpLevelMsgFColor);
        config.WriteInteger("Setup", "UpLevelMsgBColor", M2Config.btUpLevelMsgBColor);
        config.WriteInteger("Setup", "AddItemMsgX", M2Config.nAddItemMsgX);
        config.WriteInteger("Setup", "AddItemMsgY", M2Config.nAddItemMsgY);
        config.WriteInteger("Setup", "GetExpMsgX", M2Config.nGetExpMsgX);
        config.WriteInteger("Setup", "GetExpMsgY", M2Config.nGetExpMsgY);
        config.WriteInteger("Setup", "UpLevelMsgX", M2Config.nUpLevelMsgX);
        config.WriteInteger("Setup", "UpLevelMsgY", M2Config.nUpLevelMsgY);
        config.WriteInteger("Setup", "HeroAddItemMsgFColor", M2Config.btHeroAddItemMsgFColor);
        config.WriteInteger("Setup", "HeroAddItemMsgBColor", M2Config.btHeroAddItemMsgBColor);
        config.WriteInteger("Setup", "HeroGetExpMsgFColor", M2Config.btHeroGetExpMsgFColor);
        config.WriteInteger("Setup", "HeroGetExpMsgBColor", M2Config.btHeroGetExpMsgBColor);
        config.WriteInteger("Setup", "HeroUpLevelMsgFColor", M2Config.btHeroUpLevelMsgFColor);
        config.WriteInteger("Setup", "HeroUpLevelMsgBColor", M2Config.btHeroUpLevelMsgBColor);
        config.WriteInteger("Setup", "HeroAddItemMsgX", M2Config.nHeroAddItemMsgX);
        config.WriteInteger("Setup", "HeroAddItemMsgY", M2Config.nHeroAddItemMsgY);
        config.WriteInteger("Setup", "HeroGetExpMsgX", M2Config.nHeroGetExpMsgX);
        config.WriteInteger("Setup", "HeroGetExpMsgY", M2Config.nHeroGetExpMsgY);
        config.WriteInteger("Setup", "HeroUpLevelMsgX", M2Config.nHeroUpLevelMsgX);
        config.WriteInteger("Setup", "HeroUpLevelMsgY", M2Config.nHeroUpLevelMsgY);

        config.WriteBool("Setup", "AddItemMsgXRightToLeft", M2Config.boAddItemMsgXRightToLeft);
        config.WriteBool("Setup", "AddItemMsgYBottomToTop", M2Config.boAddItemMsgYBottomToTop);
        config.WriteBool("Setup", "GetExpMsgXRightToLeft", M2Config.boGetExpMsgXRightToLeft);
        config.WriteBool("Setup", "GetExpMsgYBottomToTop", M2Config.boGetExpMsgYBottomToTop);
        config.WriteBool("Setup", "UpLevelMsgXRightToLeft", M2Config.boUpLevelMsgXRightToLeft);
        config.WriteBool("Setup", "UpLevelMsgYBottomToTop", M2Config.boUpLevelMsgYBottomToTop);
        config.WriteBool("Setup", "HeroAddItemMsgXRightToLeft", M2Config.boHeroAddItemMsgXRightToLeft);
        config.WriteBool("Setup", "HeroAddItemMsgYBottomToTop", M2Config.boHeroAddItemMsgYBottomToTop);
        config.WriteBool("Setup", "HeroGetExpMsgXRightToLeft", M2Config.boHeroGetExpMsgXRightToLeft);
        config.WriteBool("Setup", "HeroGetExpMsgYBottomToTop", M2Config.boHeroGetExpMsgYBottomToTop);
        config.WriteBool("Setup", "HeroUpLevelMsgXRightToLeft", M2Config.boHeroUpLevelMsgXRightToLeft);
        config.WriteBool("Setup", "HeroUpLevelMsgYBottomToTop", M2Config.boHeroUpLevelMsgYBottomToTop);

        config.WriteBool("Setup", "ShowBagGameGoldSeparator", M2Config.boShowBagGameGoldSeparator);
        config.WriteBool("Setup", "ShowBagGameInfo", M2Config.boShowBagGameInfo);
        config.WriteBool("Setup", "ShowHeroShortKey", M2Config.boShowHeroShortKey);
        config.WriteInteger("Setup", "ShowHeroShortKeyX", M2Config.nShowHeroShortKeyX);
        config.WriteInteger("Setup", "ShowHeroShortKeyY", M2Config.nShowHeroShortKeyY);
        config.WriteInteger("Setup", "TitleFileIndex", M2Config.nTitleFileIndex);

        SendServerConfigHandler?.Invoke();
        config.UpdateFile();
        uModValue();
    }

    // ---- 吃物品名单（ListBoxClientItemName 族，1733-1834）----

    private int ListBoxItemNameIndex => ListBoxClientItemName.SelectedIndex;

    private void ItemNameButtonStates()
    {
        ButtonClientItemNameDown.Enabled = ListBoxItemNameIndex >= 0 && ListBoxItemNameIndex < ListBoxClientItemName.Items.Count - 1;
        ButtonClientItemNameeUP.Enabled = ListBoxItemNameIndex > 0;
    }

    /// <summary>ButtonClientItemNameeUPClick（1733）：选中上移一位 + 按钮态 + Save 启用。</summary>
    public void ButtonClientItemNameeUPClick()
    {
        int itemIndex = ListBoxItemNameIndex;
        if (itemIndex > 0)
        {
            string sItemName = ListBoxClientItemName.Items[itemIndex]!.ToString()!;
            ListBoxClientItemName.Items.RemoveAt(itemIndex);
            ListBoxClientItemName.Items.Insert(itemIndex - 1, sItemName);
            ListBoxClientItemName.SelectedIndex = itemIndex - 1;
            ItemNameButtonStates();
            ButtonClientItemNameSave.Enabled = true;
        }
    }

    /// <summary>ButtonClientItemNameDownClick（1752）。</summary>
    public void ButtonClientItemNameDownClick()
    {
        int itemIndex = ListBoxItemNameIndex;
        if (itemIndex >= 0 && itemIndex < ListBoxClientItemName.Items.Count - 1)
        {
            string sItemName = ListBoxClientItemName.Items[itemIndex]!.ToString()!;
            ListBoxClientItemName.Items.RemoveAt(itemIndex);
            ListBoxClientItemName.Items.Insert(itemIndex + 1, sItemName);
            ListBoxClientItemName.SelectedIndex = itemIndex + 1;
            ItemNameButtonStates();
            ButtonClientItemNameSave.Enabled = true;
        }
    }

    /// <summary>ButtonClientItemNameAddClick（1771）：空名/CompareText 重名两门。</summary>
    public void ButtonClientItemNameAddClick()
    {
        string sItemName = EditClientItemName.Text.Trim();
        if (sItemName.Length == 0)
        {
            M2Forms.ErrorBox("请输入药品名称！");
            return;
        }
        foreach (var existing in ListBoxClientItemName.Items)
        {
            if (string.Compare(existing?.ToString(), sItemName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                M2Forms.ErrorBox("此药品已经在列表中了！");
                return;
            }
        }
        ListBoxClientItemName.Items.Add(sItemName);
        ButtonClientItemNameSave.Enabled = true;
    }

    /// <summary>ListBoxClientItemNameClick（1801）：选中回填编辑框与按钮态。</summary>
    public void ListBoxClientItemNameClick()
    {
        if (ListBoxItemNameIndex >= 0)
        {
            ItemNameButtonStates();
            EditClientItemName.Text = ListBoxClientItemName.Items[ListBoxItemNameIndex]?.ToString() ?? "";
            ButtonClientItemNameDel.Enabled = true;
        }
        else
        {
            ButtonClientItemNameDel.Enabled = false;
        }
    }

    /// <summary>ButtonClientItemNameSaveClick（1815）：回写全局表（>9 截断）→ SaveClientItemList 接缝 → SendServerConfig。</summary>
    public void ButtonClientItemNameSaveClick()
    {
        ClientEatItemNameList.Clear();
        foreach (var item in ListBoxClientItemName.Items)
            ClientEatItemNameList.Add(item?.ToString() ?? "");
        while (ClientEatItemNameList.Count > 9)
            ClientEatItemNameList.RemoveAt(ClientEatItemNameList.Count - 1); // SaveClientItemList：>9 截断（M2Share 13378）
        ClientEatItemNameListWriteBack?.Invoke(ClientEatItemNameList);
        SaveClientItemListHandler?.Invoke();
        SendServerConfigHandler?.Invoke();
    }

    /// <summary>ButtonClientItemNameDelClick（1823）。</summary>
    public void ButtonClientItemNameDelClick()
    {
        if (ListBoxItemNameIndex >= 0)
        {
            ListBoxClientItemName.Items.RemoveAt(ListBoxItemNameIndex);
            ButtonClientItemNameDel.Enabled = false;
            ButtonClientItemNameSave.Enabled = true;
            ItemNameButtonStates();
        }
    }

    // ---- SpecialCmd（1899-2030）----

    private ConfigClientForm.TClientCmd? GetSelectedSpecialCmd()
    {
        System.Windows.Forms.ListViewItem? item = null;
        if (ListViewSpecialCmd.SelectedItems.Count > 0)
            item = ListViewSpecialCmd.SelectedItems[0];
        else
        {
            foreach (System.Windows.Forms.ListViewItem it in ListViewSpecialCmd.Items)
            {
                if (it.Selected)
                {
                    item = it;
                    break;
                }
            }
        }
        return item?.Tag as ConfigClientForm.TClientCmd;
    }

    /// <summary>ListViewSpecialCmdClick（1899）：选中回填两个编辑框并启用改/删。</summary>
    public void ListViewSpecialCmdClick()
    {
        ButtonSpecialCmdDel.Enabled = false;
        ButtonSpecialCmdChg.Enabled = false;
        var clientCmd = GetSelectedSpecialCmd();
        if (clientCmd == null)
            return;
        EditSpecialCmdCaption.Text = clientCmd.sCaption;
        EditSpecialCmd.Text = clientCmd.sCmd;
        ButtonSpecialCmdDel.Enabled = true;
        ButtonSpecialCmdChg.Enabled = true;
    }

    /// <summary>ButtonSpecialCmdDelClick（1918）：按引用删除 + Save 启用 + 刷新（改/删复位）。</summary>
    public void ButtonSpecialCmdDelClick()
    {
        var clientCmd = GetSelectedSpecialCmd();
        if (clientCmd == null)
            return;
        g_SpecialCmdList.Remove(clientCmd);
        ButtonSpecialCmdDel.Enabled = false;
        ButtonSpecialCmdSave.Enabled = true;
        RefSpecialCmd();
    }

    /// <summary>ButtonSpecialCmdChgClick（1945）：空名门 + 命令变更重名门 → 写回 + 刷新 + Save 启用 + Chg 复位。</summary>
    public void ButtonSpecialCmdChgClick()
    {
        var clientCmd = GetSelectedSpecialCmd();
        if (clientCmd == null)
            return;
        string sCaption = EditSpecialCmdCaption.Text;
        string sCmd = EditSpecialCmd.Text;
        if (sCaption.Length == 0)
        {
            M2Forms.ErrorBox("请输入显示名称！");
            return;
        }
        if (sCmd.Length != 0 && sCmd != clientCmd.sCmd)
        {
            foreach (var existing in g_SpecialCmdList)
            {
                if (string.Compare(existing.sCmd, sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    M2Forms.ErrorBox("该命令已经存在！");
                    return;
                }
            }
        }
        clientCmd.sCaption = sCaption;
        clientCmd.sCmd = sCmd;
        RefSpecialCmd();
        ButtonSpecialCmdSave.Enabled = true;
        ButtonSpecialCmdChg.Enabled = false;
    }

    /// <summary>ButtonSpecialCmdAddClick（1988）：空名门 + 非空命令重名门 → 追加 + Save 启用 + 刷新。</summary>
    public void ButtonSpecialCmdAddClick()
    {
        string sCaption = EditSpecialCmdCaption.Text;
        string sCmd = EditSpecialCmd.Text;
        if (sCaption.Length == 0)
        {
            M2Forms.ErrorBox("请输入显示名称！");
            return;
        }
        if (sCmd.Length != 0)
        {
            foreach (var existing in g_SpecialCmdList)
            {
                if (string.Compare(existing.sCmd, sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    M2Forms.ErrorBox("该命令已经存在！");
                    return;
                }
            }
        }
        g_SpecialCmdList.Add(new ConfigClientForm.TClientCmd { sCaption = sCaption, sCmd = sCmd });
        ButtonSpecialCmdSave.Enabled = true;
        RefSpecialCmd();
    }

    /// <summary>ButtonSpecialCmdSaveClick（2024）：复位 Save → SaveSpecialCmdList 接缝 → SendSpecialCmdList 接缝。</summary>
    public void ButtonSpecialCmdSaveClick()
    {
        ButtonSpecialCmdSave.Enabled = false;
        SaveSpecialCmdListHandler?.Invoke(g_SpecialCmdList);
        SendSpecialCmdListHandler?.Invoke();
    }

    // ---- 亮度/天气（2052-2120）----

    /// <summary>ListBoxBrightClick（2052）：选中段 → RadioGroupBright 选中亮度。</summary>
    public void ListBoxBrightClick()
    {
        int nItemIndex = ListBoxBright.SelectedIndex;
        if (nItemIndex >= 0 && nItemIndex < M2Config.BrightConfig.Length)
            SetRadio(RadioGroupBright, M2Config.BrightConfig[nItemIndex]);
    }

    /// <summary>RadioGroupBrightClick（2071）：值变化才写回 BrightConfig[选中段] + ModValue。</summary>
    public void RadioGroupBrightClick()
    {
        if (!boOpened)
            return;
        int nItemIndex = ListBoxBright.SelectedIndex;
        if (nItemIndex >= 0 && nItemIndex < M2Config.BrightConfig.Length)
        {
            int idx = RadioGroupBrightIndex();
            if (M2Config.BrightConfig[nItemIndex] != idx)
            {
                M2Config.BrightConfig[nItemIndex] = idx;
                ModValue();
            }
        }
    }

    private int RadioGroupBrightIndex()
    {
        for (int i = 0; i < RadioGroupBright.Length; i++)
        {
            if (RadioGroupBright[i].Checked)
                return i;
        }
        return -1;
    }

    /// <summary>RadioGroupBagFastItemCompareClick（2063）。</summary>
    public void RadioGroupBagFastItemCompareClick()
        => WriteBack(() => M2Config.btBagFastItemCompareMode = (byte)RadioGroupBagFastItemCompareIndex());

    private int RadioGroupBagFastItemCompareIndex()
    {
        for (int i = 0; i < RadioGroupBagFastItemCompare.Length; i++)
        {
            if (RadioGroupBagFastItemCompare[i].Checked)
                return i;
        }
        return -1;
    }

    /// <summary>RadioButtonPlugIn1/2Click（2088-2122）：切类型 → 重填页签组 → 重挂可见 → ModValue。</summary>
    public void RadioButtonPlugIn1Click()
        => RadioButtonPlugInClick(0, RadioButtonPlugIn1);

    public void RadioButtonPlugIn2Click()
        => RadioButtonPlugInClick(1, RadioButtonPlugIn2);

    private void RadioButtonPlugInClick(int dlgType, System.Windows.Forms.RadioButton radio)
    {
        if (!boOpened)
            return;
        if (radio.Checked)
        {
            M2Config.btConfigDlgType = (byte)dlgType;
            RefClientPlugTableVisible();
            for (int i = 0; i < RzCheckGroupClientTabSheet.Items.Count; i++)
                RzCheckGroupClientTabSheet.SetItemChecked(i, M2Config.ClientConfigTabSheetVisibles[i]);
        }
        ModValue();
    }

    /// <summary>RzSpinner→TrackBar 联动（1852-1861，仅同步不 ModValue）。</summary>
    public void RzSpinnerMoveSpeedChange() => TrackBarMoveSpeed.Value = Math.Clamp((int)RzSpinnerMoveSpeed.Value, TrackBarMoveSpeed.Minimum, TrackBarMoveSpeed.Maximum);

    public void RzSpinnerAttackSpeedChange() => TrackBarAttackSpeed.Value = Math.Clamp((int)RzSpinnerAttackSpeed.Value, TrackBarAttackSpeed.Minimum, TrackBarAttackSpeed.Maximum);

    public void RzSpinnerSpellSpeedChange() => TrackBarSpellSpeed.Value = Math.Clamp((int)RzSpinnerSpellSpeed.Value, TrackBarSpellSpeed.Minimum, TrackBarSpellSpeed.Maximum);

    // ---- 逐项 CheckBox/坐标/颜色 写回（同型 if not boOpened Exit; 赋值; ModValue）----

    public void CheckBoxControlHelpButtonClick() => WriteBack(() => M2Config.boControlHelpButton = CheckBoxControlHelpButton.Checked);
    public void CheckBoxRankButtonClick() => WriteBack(() => M2Config.boRankButton = CheckBoxRankButton.Checked);
    public void CheckBoxWhisperButtonClick() => WriteBack(() => M2Config.boWhisperButton = CheckBoxWhisperButton.Checked);
    public void CheckBoxActionLogButtonClick() => WriteBack(() => M2Config.boActionLogButton = CheckBoxActionLogButton.Checked);
    public void CheckBoxMissionButtonClick() => WriteBack(() => M2Config.boMissionButton = CheckBoxMissionButton.Checked);
    public void CheckBoxWebButtonClick() => WriteBack(() => M2Config.boWebButton = CheckBoxWebButton.Checked);
    public void CheckBoxOpenShopButtonClick() => WriteBack(() => M2Config.boOpenShopButton = CheckBoxOpenShopButton.Checked);
    public void CheckBoxUserShopButtonClick() => WriteBack(() => M2Config.boUserShopButton = CheckBoxUserShopButton.Checked);
    public void CheckBoxOpenHeroButtonClick() => WriteBack(() => M2Config.boOpenHeroButton = CheckBoxOpenHeroButton.Checked);
    public void CheckBoxShowMerchantDlgHelpClick() => WriteBack(() => M2Config.boShowMerchantDlgHelp = CheckBoxShowMerchantDlgHelp.Checked);
    public void CheckBoxChallengeButtonClick() => WriteBack(() => M2Config.boChallengeButton = CheckBoxChallengeButton.Checked);
    public void CheckBoxCanOpenGameConfigDlgClick() => WriteBack(() => M2Config.boCanOpenGameConfigDlg = CheckBoxCanOpenGameConfigDlg.Checked);
    public void CheckBoxNotCanUseClientConfigClick() => WriteBack(() => M2Config.boNotCanUseClientConfig = CheckBoxNotCanUseClientConfig.Checked);
    public void CheckBoxViewFogClick() => WriteBack(() => M2Config.boViewFog = CheckBoxViewFog.Checked);
    public void CheckBoxMonStruckShowNumberClick() => WriteBack(() => M2Config.boMonStruckShowNumber = CheckBoxMonStruckShowNumber.Checked);
    public void CheckBoxHumStruckShowNumberClick() => WriteBack(() => M2Config.boHumStruckShowNumber = CheckBoxHumStruckShowNumber.Checked);
    public void CheckBoxCloseBookProtectClick() => WriteBack(() => M2Config.boCloseBookProtect = CheckBoxCloseBookProtect.Checked);
    public void CheckBoxCloseLogoutProtectClick() => WriteBack(() => M2Config.boCloseLogoutProtect = CheckBoxCloseLogoutProtect.Checked);
    public void CheckBoxGetExpMsgAddChatBoardMsgClick() => WriteBack(() => M2Config.boGetExpMsgAddChatBoardMsg = CheckBoxGetExpMsgAddChatBoardMsg.Checked);

    /// <summary>CheckBoxDBotFunc1Click（1891）：Tag 索引写回（共用一条处理器）。</summary>
    public void CheckBoxDBotFuncClick(int tag) => WriteBack(() => M2Config.DBotFuncs[tag] = CheckBoxDBotFunc[tag].Checked);

    /// <summary>EditHomePageChange（1836）：Trim 写回。</summary>
    public void EditHomePageChange() => WriteBack(() => M2Config.sHomePage = EditHomePage.Text.Trim());

    /// <summary>颜色/坐标 se*Change 族（2238-2467，同型 24 处）。</summary>
    public void SeAddItemMsgFColorChange() => WriteBack(() => M2Config.btAddItemMsgFColor = (byte)seAddItemMsgFColor.Value);
    public void SeAddItemMsgBColorChange() => WriteBack(() => M2Config.btAddItemMsgBColor = (byte)seAddItemMsgBColor.Value);
    public void SeGetExpMsgFColorChange() => WriteBack(() => M2Config.btGetExpMsgFColor = (byte)seGetExpMsgFColor.Value);
    public void SeGetExpMsgBColorChange() => WriteBack(() => M2Config.btGetExpMsgBColor = (byte)seGetExpMsgBColor.Value);
    public void SeUpLevelMsgFColorChange() => WriteBack(() => M2Config.btUpLevelMsgFColor = (byte)seUpLevelMsgFColor.Value);
    public void SeUpLevelMsgBColorChange() => WriteBack(() => M2Config.btUpLevelMsgBColor = (byte)seUpLevelMsgBColor.Value);
    public void SeAddItemMsgXChange() => WriteBack(() => M2Config.nAddItemMsgX = (int)seAddItemMsgX.Value);
    public void SeAddItemMsgYChange() => WriteBack(() => M2Config.nAddItemMsgY = (int)seAddItemMsgY.Value);
    public void SeGetExpMsgXChange() => WriteBack(() => M2Config.nGetExpMsgX = (int)seGetExpMsgX.Value);
    public void SeGetExpMsgYChange() => WriteBack(() => M2Config.nGetExpMsgY = (int)seGetExpMsgY.Value);
    public void SeUpLevelMsgXChange() => WriteBack(() => M2Config.nUpLevelMsgX = (int)seUpLevelMsgX.Value);
    public void SeUpLevelMsgYChange() => WriteBack(() => M2Config.nUpLevelMsgY = (int)seUpLevelMsgY.Value);

    /// <summary>方向 12 复选（2475-2499 + HeroAdd/Get/Up 族，同型）。</summary>
    public void ChkAddItemMsgXRightToLeftClick() => WriteBack(() => M2Config.boAddItemMsgXRightToLeft = chkAddItemMsgXRightToLeft.Checked);
    public void ChkAddItemMsgYBottomToTopClick() => WriteBack(() => M2Config.boAddItemMsgYBottomToTop = chkAddItemMsgYBottomToTop.Checked);
    public void ChkGetExpMsgXRightToLeftClick() => WriteBack(() => M2Config.boGetExpMsgXRightToLeft = chkGetExpMsgXRightToLeft.Checked);
    public void ChkGetExpMsgYBottomToTopClick() => WriteBack(() => M2Config.boGetExpMsgYBottomToTop = chkGetExpMsgYBottomToTop.Checked);
    public void ChkUpLevelMsgXRightToLeftClick() => WriteBack(() => M2Config.boUpLevelMsgXRightToLeft = chkUpLevelMsgXRightToLeft.Checked);
    public void ChkUpLevelMsgYBottomToTopClick() => WriteBack(() => M2Config.boUpLevelMsgYBottomToTop = chkUpLevelMsgYBottomToTop.Checked);
    public void ChkHeroAddItemMsgXRightToLeftClick() => WriteBack(() => M2Config.boHeroAddItemMsgXRightToLeft = chkHeroAddItemMsgXRightToLeft.Checked);
    public void ChkHeroAddItemMsgYBottomToTopClick() => WriteBack(() => M2Config.boHeroAddItemMsgYBottomToTop = chkHeroAddItemMsgYBottomToTop.Checked);
    public void ChkHeroGetExpMsgXRightToLeftClick() => WriteBack(() => M2Config.boHeroGetExpMsgXRightToLeft = chkHeroGetExpMsgXRightToLeft.Checked);
    public void ChkHeroGetExpMsgYBottomToTopClick() => WriteBack(() => M2Config.boHeroGetExpMsgYBottomToTop = chkHeroGetExpMsgYBottomToTop.Checked);
    public void ChkHeroUpLevelMsgXRightToLeftClick() => WriteBack(() => M2Config.boHeroUpLevelMsgXRightToLeft = chkHeroUpLevelMsgXRightToLeft.Checked);
    public void ChkHeroUpLevelMsgYBottomToTopClick() => WriteBack(() => M2Config.boHeroUpLevelMsgYBottomToTop = chkHeroUpLevelMsgYBottomToTop.Checked);

    /// <summary>页签隐藏五项（2483-2489 + Hero 族）。</summary>
    public void ChkHideTabSheet2Click() => WriteBack(() => M2Config.boHideTabSheet2 = chkHideTabSheet2.Checked);
    public void ChkHeroHideTabSheet2Click() => WriteBack(() => M2Config.boHeroHideTabSheet2 = chkHeroHideTabSheet2.Checked);
    public void ChkHideTabSheet5Click() => WriteBack(() => M2Config.boHideTabSheet5 = chkHideTabSheet5.Checked);
    public void ChkHeroHideTabSheet5Click() => WriteBack(() => M2Config.boHeroHideTabSheet5 = chkHeroHideTabSheet5.Checked);
    public void ChkHideTabSheet7Click() => WriteBack(() => M2Config.boHideTabSheet7 = chkHideTabSheet7.Checked);

    // ==================== 批次J69 尾片（ConfigClient.pas 2500-4266） ====================

    /// <summary>CheckGroupNewAbilChange（2507）：按索引写新属性显示组。</summary>
    public void CheckGroupNewAbilChange(int index)
        => WriteBack(() => M2Config.NewAbilShowStateDlg[index] = CheckGroupNewAbil.GetItemChecked(index));

    /// <summary>chkUseFindPathClick（2612）：置 boSendServerConfig 标志（Option2 保存时才下发）。</summary>
    public void ChkUseFindPathClick()
        => WriteBack(() => { M2Config.boUseFindPath = chkUseFindPath.Checked; boSendServerConfig = true; });

    /// <summary>CheckBoxUseOldSerialWindowsClick（2621）：写回 + 联动装备栏类型组启用。</summary>
    public void CheckBoxUseOldSerialWindowsClick()
        => WriteBack(() =>
        {
            M2Config.boUseOldSerialWindows = CheckBoxUseOldSerialWindows.Checked;
            rgStateWindows[1].Enabled = CheckBoxUseOldSerialWindows.Checked;
        });

    /// <summary>rgStateWindowsClick（2630）。</summary>
    public void RgStateWindowsClick()
    {
        if (!boOpened)
            return;
        for (int i = 0; i < rgStateWindows.Length; i++)
        {
            if (rgStateWindows[i].Checked)
            {
                M2Config.boStateWindowsType = (byte)i;
                break;
            }
        }
        ModValue();
    }

    /// <summary>chkHealthNumberTextClick（3583）：写回 + boSendServerConfig + 血数字三联动启用。</summary>
    public void ChkHealthNumberTextClick()
        => WriteBack(() =>
        {
            M2Config.boHealthNumberText = chkHealthNumberText.Checked;
            boSendServerConfig = true;
            chkBlastHitShowHealthNum.Enabled = chkHealthNumberText.Checked;
            chkHPStoneHideHealthNum.Enabled = !chkHealthNumberText.Checked;
            chkMPStoneHideHealthNum.Enabled = !chkHealthNumberText.Checked;
        });

    /// <summary>chkItemFromField0Click（4098，Sender.Tag 共用一条）：boShowItemFromFields[Tag] 写回。</summary>
    public void ChkItemFromFieldClick(int tag)
        => WriteBack(() => M2Config.boShowItemFromFields[tag] = chkItemFromField[tag].Checked);

    /// <summary>btnSaveDrugAndRestoreClick（2678-2736）：恢复节奏 45 项 ini（不发客户端，回血服务端运算；boSendServerConfig 门下发）。</summary>
    public void BtnSaveDrugAndRestoreClick()
    {
        var config = M2ShareState.ConfigIni;
        config.WriteInteger("Setup", "PerSpell", M2Config.nPerSpell);
        config.WriteInteger("Setup", "PerHealth", M2Config.nPerHealth);
        config.WriteInteger("Setup", "IncHealthSpellTime", M2Config.nIncHealthSpellTime);
        config.WriteInteger("Setup", "HealthFillTime", M2Config.nHealthFillTime);
        config.WriteInteger("Setup", "HealthFillTime_Human_Warrior", M2Config.nHealthFillTime_Human_Warrior);
        config.WriteInteger("Setup", "HealthFillTime_Human_TaoistAndWizard", M2Config.nHealthFillTime_Human_TaoistAndWizard);
        config.WriteInteger("Setup", "HealthFillTime_Hero_Warrior", M2Config.nHealthFillTime_Hero_Warrior);
        config.WriteInteger("Setup", "HealthFillTime_Hero_TaoistAndWizard", M2Config.nHealthFillTime_Hero_TaoistAndWizard);
        config.WriteInteger("Setup", "SpellFillTime", M2Config.nSpellFillTime);
        config.WriteInteger("Setup", "SpellFillTime_Human_Warrior", M2Config.nSpellFillTime_Human_Warrior);
        config.WriteInteger("Setup", "SpellFillTime_Human_TaoistAndWizard", M2Config.nSpellFillTime_Human_TaoistAndWizard);
        config.WriteInteger("Setup", "SpellFillTime_Hero_Warrior", M2Config.nSpellFillTime_Hero_Warrior);
        config.WriteInteger("Setup", "SpellFillTime_Hero_TaoistAndWizard", M2Config.nSpellFillTime_Hero_TaoistAndWizard);
        config.WriteInteger("Setup", "HealthBaseNum", M2Config.nHealthBaseNum);
        config.WriteInteger("Setup", "SpellBaseNum", M2Config.nSpellBaseNum);
        config.WriteInteger("Setup", "HealthBaseNum_Human_Warrior", M2Config.nHealthBaseNum_Human_Warrior);
        config.WriteInteger("Setup", "HealthBaseNum_Human_TaoistAndWizard", M2Config.nHealthBaseNum_Human_TaoistAndWizard);
        config.WriteInteger("Setup", "HealthBaseNum_Hero_Warrior", M2Config.nHealthBaseNum_Hero_Warrior);
        config.WriteInteger("Setup", "HealthBaseNum_Hero_TaoistAndWizard", M2Config.nHealthBaseNum_Hero_TaoistAndWizard);
        config.WriteInteger("Setup", "SpellBaseNum_Human_Warrior", M2Config.nSpellBaseNum_Human_Warrior);
        config.WriteInteger("Setup", "SpellBaseNum_Human_TaoistAndWizard", M2Config.nSpellBaseNum_Human_TaoistAndWizard);
        config.WriteInteger("Setup", "SpellBaseNum_Hero_Warrior", M2Config.nSpellBaseNum_Hero_Warrior);
        config.WriteInteger("Setup", "SpellBaseNum_Hero_TaoistAndWizard", M2Config.nSpellBaseNum_Hero_TaoistAndWizard);
        config.WriteInteger("Setup", "UseItemIntervalTime", (int)M2Config.dwUseItemIntervalTime);
        config.WriteInteger("Setup", "UseAttackItemIntervalTime", (int)M2Config.dwUseAttackItemIntervalTime);
        config.WriteInteger("Setup", "UseOrdinaryTime_Human_Warrior", (int)M2Config.dwUseOrdinaryTime_Human_Warrior);
        config.WriteInteger("Setup", "UseSpecialTime_Human_Warrior", (int)M2Config.dwUseSpecialTime_Human_Warrior);
        config.WriteInteger("Setup", "UseOrdinaryTime_Human_TaoistAndWizard", (int)M2Config.dwUseOrdinaryTime_Human_TaoistAndWizard);
        config.WriteInteger("Setup", "UseSpecialTime_Human_TaoistAndWizard", (int)M2Config.dwUseSpecialTime_Human_TaoistAndWizard);
        config.WriteInteger("Setup", "UseOrdinaryTime_Hero_Warrior", (int)M2Config.dwUseOrdinaryTime_Hero_Warrior);
        config.WriteInteger("Setup", "UseSpecialTime_Hero_Warrior", (int)M2Config.dwUseSpecialTime_Hero_Warrior);
        config.WriteInteger("Setup", "UseOrdinaryTime_Hero_TaoistAndWizard", (int)M2Config.dwUseOrdinaryTime_Hero_TaoistAndWizard);
        config.WriteInteger("Setup", "UseSpecialTime_Hero_TaoistAndWizard", (int)M2Config.dwUseSpecialTime_Hero_TaoistAndWizard);
        config.WriteInteger("Setup", "IncHealingLimite", M2Config.nIncHealingLimite);
        config.WriteInteger("Setup", "PerHealing", M2Config.nPerHealing);
        config.WriteInteger("Setup", "PerHealingTime", M2Config.nPerHealingTime);
        config.WriteInteger("Setup", "BigPerHealing", M2Config.nBigPerHealing);
        config.WriteInteger("Setup", "BigPerHealingTime", M2Config.nBigPerHealingTime);

        if (boSendServerConfig)
        {
            SendServerConfigHandler?.Invoke();
            boSendServerConfig = false;
        }
        config.UpdateFile();
        uModValue();
    }

    /// <summary>btnSaveOption2Click（3181-3276）：小地图/血条/名字/坐标缩放 60 余项 ini（boSendServerConfig 门下发）。</summary>
    public void BtnSaveOption2Click()
    {
        var config = M2ShareState.ConfigIni;
        config.WriteBool("Setup", "UseFindPath", M2Config.boUseFindPath);
        config.WriteBool("Setup", "MinMapCloseRadar", M2Config.boMinMapCloseRadar);
        config.WriteInteger("Setup", "MinMapUseOld", M2Config.btMinMapType);
        config.WriteBool("Setup", "MinMapUseFindPath", M2Config.boMinMapUseFindPath);
        config.WriteBool("Setup", "LoginShowMinMap", M2Config.boLoginShowMinMap);
        config.WriteInteger("Setup", "MinMapFlagFlash", (int)M2Config.dwMinMapFlagFlash);
        config.WriteInteger("Setup", "MinMapColorSelf", M2Config.btMinMapColorSelf);
        config.WriteInteger("Setup", "MinMapColorOther", M2Config.btMinMapColorOther);
        config.WriteInteger("Setup", "MinMapColorNPC", M2Config.btMinMapColorNPC);
        config.WriteInteger("Setup", "MinMapColorGuard", M2Config.btMinMapColorGuard);
        config.WriteInteger("Setup", "MinMapColorMonster", M2Config.btMinMapColorMonster);
        config.WriteInteger("Setup", "MinMapColorHero", M2Config.btMinMapColorHero);
        config.WriteInteger("Setup", "MinMapColorBoss", M2Config.btMinMapColorBoss);
        config.WriteBool("Setup", "HideItemNameNum", M2Config.boHideItemNameNum);
        config.WriteBool("Setup", "KeyTabGetActor", M2Config.boKeyTabGetActor);
        config.WriteBool("Setup", "ShowMagicShieldHP", M2Config.boShowMagicShieldHP);
        config.WriteInteger("Setup", "HumHPBarOffsetX", M2Config.nHumHPBarOffsetX);
        config.WriteInteger("Setup", "HumHPBarOffsetY", M2Config.nHumHPBarOffsetY);
        config.WriteInteger("Setup", "NpcHPBarOffsetX", M2Config.nNpcHPBarOffsetX);
        config.WriteInteger("Setup", "NpcHPBarOffsetY", M2Config.nNpcHPBarOffsetY);
        config.WriteInteger("Setup", "MonHPBarOffsetX", M2Config.nMonHPBarOffsetX);
        config.WriteInteger("Setup", "MonHPBarOffsetY", M2Config.nMonHPBarOffsetY);
        config.WriteInteger("Setup", "HumNameOffsetX", M2Config.nHumNameOffsetX);
        config.WriteInteger("Setup", "HumNameOffsetY", M2Config.nHumNameOffsetY);
        config.WriteInteger("Setup", "NpcNameOffsetX", M2Config.nNpcNameOffsetX);
        config.WriteInteger("Setup", "NpcNameOffsetY", M2Config.nNpcNameOffsetY);
        config.WriteInteger("Setup", "MonNameOffsetX", M2Config.nMonNameOffsetX);
        config.WriteInteger("Setup", "MonNameOffsetY", M2Config.nMonNameOffsetY);
        config.WriteBool("Setup", "HealthNumberText", M2Config.boHealthNumberText);
        config.WriteBool("Setup", "BlastHitShowHealthNum", M2Config.boBlastHitShowHealthNum);
        config.WriteBool("Setup", "PoisoningHideHealthNum", M2Config.boPoisoningHideHealthNum);
        config.WriteBool("Setup", "HPStoneHideHealthNum", M2Config.boHPStoneHideHealthNum);
        config.WriteBool("Setup", "MPStoneHideHealthNum", M2Config.boMPStoneHideHealthNum);
        config.WriteInteger("Setup", "HealthNumberOffsetX", M2Config.nHealthNumberOffsetX);
        config.WriteInteger("Setup", "HealthNumberOffsetY", M2Config.nHealthNumberOffsetY);
        config.WriteInteger("Setup", "HealthNumberMoveSpeed", M2Config.nHealthNumberMoveSpeed);
        config.WriteInteger("Setup", "NewLeftGroupInfoOffsetX", M2Config.nNewLeftGroupInfoOffsetX);
        config.WriteInteger("Setup", "NewLeftGroupInfoOffsetY", M2Config.nNewLeftGroupInfoOffsetY);
        config.WriteBool("Setup", "MagicSetDir", M2Config.boMagicSetDir);
        config.WriteInteger("Setup", "BetterItemX", M2Config.nBetterItemX);
        config.WriteInteger("Setup", "BetterItemY", M2Config.nBetterItemY);
        config.WriteInteger("Setup", "SmallInfoX", M2Config.nSmallInfoX);
        config.WriteInteger("Setup", "SmallInfoY", M2Config.nSmallInfoY);
        config.WriteInteger("Setup", "JoyStickX", M2Config.nJoyStickX);
        config.WriteInteger("Setup", "JoyStickY", M2Config.nJoyStickY);
        config.WriteInteger("Setup", "JoyStickMaxX", M2Config.nJoyStickMaxX);
        config.WriteInteger("Setup", "JoyStickMaxY", M2Config.nJoyStickMaxY);
        config.WriteInteger("Setup", "SkillCtrX", M2Config.nSkillCtrX);
        config.WriteInteger("Setup", "SkillCtrY", M2Config.nSkillCtrY);
        config.WriteBool("Setup", "ShowExSkillIcon", M2Config.boShowExSkillIcon);
        config.WriteInteger("Setup", "MapScale", M2Config.btMapScale);
        config.WriteInteger("Setup", "GuiScale", M2Config.btGuiScale);
        config.WriteInteger("Setup", "MultiViewRange", M2Config.btMultiViewRange);
        config.WriteBool("Setup", "ShowMulitDlg", M2Config.boShowMulitDlg);
        config.WriteBool("Setup", "ShowBetterItem", M2Config.boShowBetterItem);

        if (boSendServerConfig)
        {
            SendServerConfigHandler?.Invoke();
            boSendServerConfig = false;
        }
        config.UpdateFile();
        uModValue();
    }

    /// <summary>btnArrBtnSettingClick（4030-4052）：ArrButton 节七组写入 + CRC 标记 + 按钮禁用。</summary>
    public void BtnArrBtnSettingClick()
    {
        var config = M2ShareState.ConfigIni;
        for (int i = 0; i < M2Config.g_ArrButtonConfig.Length; i++)
        {
            var g = M2Config.g_ArrButtonConfig[i];
            string idx = i.ToString();
            config.WriteInteger("ArrButton", "HorzAlign" + idx, g.HorzAligment);
            config.WriteInteger("ArrButton", "OffsetX" + idx, g.OffsetX);
            config.WriteInteger("ArrButton", "VertAlign" + idx, g.VertAligment);
            config.WriteInteger("ArrButton", "OffsetY" + idx, g.OffsetY);
            config.WriteInteger("ArrButton", "NextOffsetX" + idx, g.NextOffsetX);
            config.WriteInteger("ArrButton", "NextOffsetY" + idx, g.NextOffsetY);
        }
        M2Config.g_ArrButtonConfigCRC++;
        config.UpdateFile();
        btnArrBtnSetting.Enabled = false;
    }

    /// <summary>btnRestoreDefaultClick（3006-3068）：恢复节奏 25 控件到出厂值（仅控件，不动 g_Config）。</summary>
    public void BtnRestoreDefaultClick()
    {
        seUseOrdinaryTime_Human_Warrior.Value = 300;
        seUseSpecialTime_Human_Warrior.Value = 1000;
        seUseOrdinaryTime_Human_TaoistAndWizard.Value = 300;
        seUseSpecialTime_Human_TaoistAndWizard.Value = 1000;
        seUseOrdinaryTime_Hero_Warrior.Value = 300;
        seUseSpecialTime_Hero_Warrior.Value = 1000;
        seUseOrdinaryTime_Hero_TaoistAndWizard.Value = 300;
        seUseSpecialTime_Hero_TaoistAndWizard.Value = 1000;
        sePerHealth.Value = 10;
        sePerSpell.Value = 10;
        seIncHealthSpell.Value = 700;
        seUseItemIntervalTime.Value = 500;
        seUseAttackItemIntervalTime.Value = 500;
        seHealthFillTime_Human_Warrior.Value = 350;
        seHealthBaseNum_Human_Warrior.Value = 75;
        seSpellFillTime_Human_Warrior.Value = 800;
        seSpellBaseNum_Human_Warrior.Value = 18;
        seHealthFillTime_Human_TaoistAndWizard.Value = 350;
        seHealthBaseNum_Human_TaoistAndWizard.Value = 75;
        seSpellFillTime_Human_TaoistAndWizard.Value = 800;
        seSpellBaseNum_Human_TaoistAndWizard.Value = 18;
        seHealthFillTime_Hero_Warrior.Value = 350;
        seHealthBaseNum_Hero_Warrior.Value = 75;
        seSpellFillTime_Hero_Warrior.Value = 800;
        seSpellBaseNum_Hero_Warrior.Value = 18;
        seHealthFillTime_Hero_TaoistAndWizard.Value = 350;
        seHealthBaseNum_Hero_TaoistAndWizard.Value = 75;
        seSpellFillTime_Hero_TaoistAndWizard.Value = 800;
        seSpellBaseNum_Hero_TaoistAndWizard.Value = 18;
        seHealthFillTime.Value = 450;
        seHealthBaseNum.Value = 75;
        seSpellFillTime.Value = 800;
        seSpellBaseNum.Value = 18;
        sePerHealingTime.Value = 700;
        sePerHealing.Value = 5;
        seBigPerHealingTime.Value = 700;
        seBigPerHealing.Value = 5;
        ModValue();
    }

    // ---- 恢复节奏族（2654-3004；UseOrdinary/Special 八项带 boSendServerConfig）----

    public void SePerHealthChange() => WriteBack(() => M2Config.nPerHealth = (int)sePerHealth.Value);
    public void SePerSpellChange() => WriteBack(() => M2Config.nPerSpell = (int)sePerSpell.Value);
    public void SeIncHealthSpellChange() => WriteBack(() => M2Config.nIncHealthSpellTime = (int)seIncHealthSpell.Value);
    public void SeHealthFillTimeChange() => WriteBack(() => M2Config.nHealthFillTime = (int)seHealthFillTime.Value);
    public void SeHealthBaseNumChange() => WriteBack(() => M2Config.nHealthBaseNum = (int)seHealthBaseNum.Value);
    public void SeSpellFillTimeChange() => WriteBack(() => M2Config.nSpellFillTime = (int)seSpellFillTime.Value);
    public void SeSpellBaseNumChange() => WriteBack(() => M2Config.nSpellBaseNum = (int)seSpellBaseNum.Value);
    public void SeHealthFillTime_Human_WarriorChange() => WriteBack(() => M2Config.nHealthFillTime_Human_Warrior = (int)seHealthFillTime_Human_Warrior.Value);
    public void SeHealthBaseNum_Human_WarriorChange() => WriteBack(() => M2Config.nHealthBaseNum_Human_Warrior = (int)seHealthBaseNum_Human_Warrior.Value);
    public void SeSpellFillTime_Human_WarriorChange() => WriteBack(() => M2Config.nSpellFillTime_Human_Warrior = (int)seSpellFillTime_Human_Warrior.Value);
    public void SeSpellBaseNum_Human_WarriorChange() => WriteBack(() => M2Config.nSpellBaseNum_Human_Warrior = (int)seSpellBaseNum_Human_Warrior.Value);
    public void SeHealthFillTime_Human_TaoistAndWizardChange() => WriteBack(() => M2Config.nHealthFillTime_Human_TaoistAndWizard = (int)seHealthFillTime_Human_TaoistAndWizard.Value);
    public void SeHealthBaseNum_Human_TaoistAndWizardChange() => WriteBack(() => M2Config.nHealthBaseNum_Human_TaoistAndWizard = (int)seHealthBaseNum_Human_TaoistAndWizard.Value);
    public void SeSpellFillTime_Human_TaoistAndWizardChange() => WriteBack(() => M2Config.nSpellFillTime_Human_TaoistAndWizard = (int)seSpellFillTime_Human_TaoistAndWizard.Value);
    public void SeSpellBaseNum_Human_TaoistAndWizardChange() => WriteBack(() => M2Config.nSpellBaseNum_Human_TaoistAndWizard = (int)seSpellBaseNum_Human_TaoistAndWizard.Value);
    public void SeHealthFillTime_Hero_WarriorChange() => WriteBack(() => M2Config.nHealthFillTime_Hero_Warrior = (int)seHealthFillTime_Hero_Warrior.Value);
    public void SeHealthBaseNum_Hero_WarriorChange() => WriteBack(() => M2Config.nHealthBaseNum_Hero_Warrior = (int)seHealthBaseNum_Hero_Warrior.Value);
    public void SeSpellFillTime_Hero_WarriorChange() => WriteBack(() => M2Config.nSpellFillTime_Hero_Warrior = (int)seSpellFillTime_Hero_Warrior.Value);
    public void SeSpellBaseNum_Hero_WarriorChange() => WriteBack(() => M2Config.nSpellBaseNum_Hero_Warrior = (int)seSpellBaseNum_Hero_Warrior.Value);
    public void SeHealthFillTime_Hero_TaoistAndWizardChange() => WriteBack(() => M2Config.nHealthFillTime_Hero_TaoistAndWizard = (int)seHealthFillTime_Hero_TaoistAndWizard.Value);
    public void SeHealthBaseNum_Hero_TaoistAndWizardChange() => WriteBack(() => M2Config.nHealthBaseNum_Hero_TaoistAndWizard = (int)seHealthBaseNum_Hero_TaoistAndWizard.Value);
    public void SeSpellFillTime_Hero_TaoistAndWizardChange() => WriteBack(() => M2Config.nSpellFillTime_Hero_TaoistAndWizard = (int)seSpellFillTime_Hero_TaoistAndWizard.Value);
    public void SeSpellBaseNum_Hero_TaoistAndWizardChange() => WriteBack(() => M2Config.nSpellBaseNum_Hero_TaoistAndWizard = (int)seSpellBaseNum_Hero_TaoistAndWizard.Value);
    public void SeUseItemIntervalTimeChange() => WriteBack(() => M2Config.dwUseItemIntervalTime = (uint)seUseItemIntervalTime.Value);
    public void SeUseAttackItemIntervalTimeChange() => WriteBack(() => M2Config.dwUseAttackItemIntervalTime = (uint)seUseAttackItemIntervalTime.Value);
    public void SeUseOrdinaryTime_Human_WarriorChange() => WriteBack(() => { M2Config.dwUseOrdinaryTime_Human_Warrior = (uint)seUseOrdinaryTime_Human_Warrior.Value; boSendServerConfig = true; });
    public void SeUseSpecialTime_Human_WarriorChange() => WriteBack(() => { M2Config.dwUseSpecialTime_Human_Warrior = (uint)seUseSpecialTime_Human_Warrior.Value; boSendServerConfig = true; });
    public void SeUseOrdinaryTime_Human_TaoistAndWizardChange() => WriteBack(() => { M2Config.dwUseOrdinaryTime_Human_TaoistAndWizard = (uint)seUseOrdinaryTime_Human_TaoistAndWizard.Value; boSendServerConfig = true; });
    public void SeUseSpecialTime_Human_TaoistAndWizardChange() => WriteBack(() => { M2Config.dwUseSpecialTime_Human_TaoistAndWizard = (uint)seUseSpecialTime_Human_TaoistAndWizard.Value; boSendServerConfig = true; });
    public void SeUseOrdinaryTime_Hero_WarriorChange() => WriteBack(() => { M2Config.dwUseOrdinaryTime_Hero_Warrior = (uint)seUseOrdinaryTime_Hero_Warrior.Value; boSendServerConfig = true; });
    public void SeUseSpecialTime_Hero_WarriorChange() => WriteBack(() => { M2Config.dwUseSpecialTime_Hero_Warrior = (uint)seUseSpecialTime_Hero_Warrior.Value; boSendServerConfig = true; });
    public void SeUseOrdinaryTime_Hero_TaoistAndWizardChange() => WriteBack(() => { M2Config.dwUseOrdinaryTime_Hero_TaoistAndWizard = (uint)seUseOrdinaryTime_Hero_TaoistAndWizard.Value; boSendServerConfig = true; });
    public void SeUseSpecialTime_Hero_TaoistAndWizardChange() => WriteBack(() => { M2Config.dwUseSpecialTime_Hero_TaoistAndWizard = (uint)seUseSpecialTime_Hero_TaoistAndWizard.Value; boSendServerConfig = true; });

    // ---- 帧间隔/插件族（3278+ / 3417 / 3575 / 3653-3746）----

    public void SeHitFrameTimeChange() => WriteBack(() => M2Config.dwHitFrameTime = (uint)seHitFrameTime.Value);
    public void SeMagicHitFrameTimeChange() => WriteBack(() => M2Config.dwMagicHitFrameTime = (uint)seMagicHitFrameTime.Value);
    public void SeMoveFrameTimeChange() => WriteBack(() => M2Config.dwMoveFrameTime = (uint)seMoveFrameTime.Value);
    public void SePluginPickupTimeChange() => WriteBack(() => M2Config.dwPluginPickupTime = (uint)sePluginPickupTime.Value);
    public void SePluginMinEatItemTimeChange() => WriteBack(() => M2Config.dwPluginMinEatItemTime = (uint)sePluginMinEatItemTime.Value);
    public void SeIncSpeedDecIntervalChange() => WriteBack(() => { M2Config.dwIncSpeedDecInterval = (uint)seIncSpeedDecInterval.Value; boSendServerConfig = true; });
    public void SeIncMoveSpeedDecIntervalChange() => WriteBack(() => { M2Config.dwIncMoveSpeedDecInterval = (uint)seIncMoveSpeedDecInterval.Value; boSendServerConfig = true; });
    public void SeIncSpellSpeedDecIntervalChange() => WriteBack(() => { M2Config.dwIncSpellSpeedDecInterval = (uint)seIncSpellSpeedDecInterval.Value; boSendServerConfig = true; });

    // ---- 小地图族（3078-3169、3967、4054-4070；带 boSendServerConfig）----

    public void ChkMinMapCloseRadarClick() => WriteBack(() => { M2Config.boMinMapCloseRadar = chkMinMapCloseRadar.Checked; boSendServerConfig = true; });
    public void ChkMinMapUseFindPathClick() => WriteBack(() => { M2Config.boMinMapUseFindPath = chkMinMapUseFindPath.Checked; boSendServerConfig = true; });
    public void ChkLoginShowMinMapClick() => WriteBack(() => { M2Config.boLoginShowMinMap = chkLoginShowMinMap.Checked; boSendServerConfig = true; });
    public void SeMinMapFlagFlashChange() => WriteBack(() => { M2Config.dwMinMapFlagFlash = (uint)seMinMapFlagFlash.Value; boSendServerConfig = true; });
    public void SeMinMapColorSelfChange() => WriteBack(() => { M2Config.btMinMapColorSelf = (byte)seMinMapColorSelf.Value; boSendServerConfig = true; });
    public void SeMinMapColorOtherChange() => WriteBack(() => { M2Config.btMinMapColorOther = (byte)seMinMapColorOther.Value; boSendServerConfig = true; });
    public void SeMinMapColorNPCChange() => WriteBack(() => { M2Config.btMinMapColorNPC = (byte)seMinMapColorNPC.Value; boSendServerConfig = true; });
    public void SeMinMapColorMonsterChange() => WriteBack(() => { M2Config.btMinMapColorMonster = (byte)seMinMapColorMonster.Value; boSendServerConfig = true; });
    public void SeMinMapColorGuardChange() => WriteBack(() => { M2Config.btMinMapColorGuard = (byte)seMinMapColorGuard.Value; boSendServerConfig = true; });
    public void SeMinMapColorHeroChange() => WriteBack(() => { M2Config.btMinMapColorHero = (byte)seMinMapColorHero.Value; boSendServerConfig = true; });
    public void SeMinMapColorBossChange() => WriteBack(() => { M2Config.btMinMapColorBoss = (byte)seMinMapColorBoss.Value; boSendServerConfig = true; });
    public void CbbMinMapTypeChange() => WriteBack(() => { M2Config.btMinMapType = (byte)Math.Max(0, cbbMinMapType.SelectedIndex); boSendServerConfig = true; });

    // ---- 血条/名字偏移族（4090-4216、4072-4089；带 boSendServerConfig）----

    public void SeHumHPBarOffsetXChange() => WriteBack(() => { M2Config.nHumHPBarOffsetX = (int)seHumHPBarOffsetX.Value; boSendServerConfig = true; });
    public void SeHumHPBarOffsetYChange() => WriteBack(() => { M2Config.nHumHPBarOffsetY = (int)seHumHPBarOffsetY.Value; boSendServerConfig = true; });
    public void SeNpcHPBarOffsetXChange() => WriteBack(() => { M2Config.nNpcHPBarOffsetX = (int)seNpcHPBarOffsetX.Value; boSendServerConfig = true; });
    public void SeNpcHPBarOffsetYChange() => WriteBack(() => { M2Config.nNpcHPBarOffsetY = (int)seNpcHPBarOffsetY.Value; boSendServerConfig = true; });
    public void SeMonHPBarOffsetXChange() => WriteBack(() => { M2Config.nMonHPBarOffsetX = (int)seMonHPBarOffsetX.Value; boSendServerConfig = true; });
    public void SeMonHPBarOffsetYChange() => WriteBack(() => { M2Config.nMonHPBarOffsetY = (int)seMonHPBarOffsetY.Value; boSendServerConfig = true; });
    public void SeHumNameOffsetXChange() => WriteBack(() => { M2Config.nHumNameOffsetX = (int)seHumNameOffsetX.Value; boSendServerConfig = true; });
    public void SeHumNameOffsetYChange() => WriteBack(() => { M2Config.nHumNameOffsetY = (int)seHumNameOffsetY.Value; boSendServerConfig = true; });
    public void SeNpcNameOffsetXChange() => WriteBack(() => { M2Config.nNpcNameOffsetX = (int)seNpcNameOffsetX.Value; boSendServerConfig = true; });
    public void SeNpcNameOffsetYChange() => WriteBack(() => { M2Config.nNpcNameOffsetY = (int)seNpcNameOffsetY.Value; boSendServerConfig = true; });
    public void SeMonNameOffsetXChange() => WriteBack(() => { M2Config.nMonNameOffsetX = (int)seMonNameOffsetX.Value; boSendServerConfig = true; });
    public void SeMonNameOffsetYChange() => WriteBack(() => { M2Config.nMonNameOffsetY = (int)seMonNameOffsetY.Value; boSendServerConfig = true; });
    public void SeHealthNumberOffsetXChange() => WriteBack(() => { M2Config.nHealthNumberOffsetX = (int)seHealthNumberOffsetX.Value; boSendServerConfig = true; });
    public void SeHealthNumberOffsetYChange() => WriteBack(() => { M2Config.nHealthNumberOffsetY = (int)seHealthNumberOffsetY.Value; boSendServerConfig = true; });
    public void SeHealthNumberMoveSpeedChange() => WriteBack(() => { M2Config.nHealthNumberMoveSpeed = (int)seHealthNumberMoveSpeed.Value; boSendServerConfig = true; });
    public void SeNewLeftGroupInfoOffsetXChange() => WriteBack(() => { M2Config.nNewLeftGroupInfoOffsetX = (int)seNewLeftGroupInfoOffsetX.Value; boSendServerConfig = true; });
    public void SeNewLeftGroupInfoOffsetYChange() => WriteBack(() => { M2Config.nNewLeftGroupInfoOffsetY = (int)seNewLeftGroupInfoOffsetY.Value; boSendServerConfig = true; });

    // ---- 其余 chk* 直写（chk 族，多数不带标志）----

    public void ChkShowHintLinesClick() => WriteBack(() => M2Config.boShowHintLines = chkShowHintLines.Checked);
    public void ChkShowHintWindowFrameClick() => WriteBack(() => M2Config.boShowHintWindowFrame = chkShowHintWindowFrame.Checked);
    public void ChkGemUpgradeClick() => WriteBack(() => M2Config.boGemUpgrade = chkGemUpgrade.Checked);
    public void ChkShowGuildNameClick() => WriteBack(() => M2Config.boShowGuildName = chkShowGuildName.Checked);
    public void ChkShopGuiCanMoveClick() => WriteBack(() => M2Config.boShopGuiCanMove = chkShopGuiCanMove.Checked);
    public void ChkNPCGuiCanMoveClick() => WriteBack(() => M2Config.boNPCGuiCanMove = chkNPCGuiCanMove.Checked);
    public void ChkShowGloryClick() => WriteBack(() => M2Config.boShowGlory = chkShowGlory.Checked);
    public void ChkShowHorseButtonClick() => WriteBack(() => M2Config.boShowHorseButton = chkShowHorseButton.Checked);
    public void ChkShowDeputyHeroButtonClick() => WriteBack(() => M2Config.boShowDeputyHeroButton = chkShowDeputyHeroButton.Checked);
    public void ChkShowExSkillIconClick() => WriteBack(() => { M2Config.boShowExSkillIcon = chkShowExSkillIcon.Checked; boSendServerConfig = true; });
    public void ChkShowBagArrangeClick() => WriteBack(() => M2Config.boShowBagArrange = chkShowBagArrange.Checked);
    public void ChkKeyTabGetActorClick() => WriteBack(() => { M2Config.boKeyTabGetActor = chkKeyTabGetActor.Checked; boSendServerConfig = true; });
    public void ChkShowMagicShieldHPClick() => WriteBack(() => { M2Config.boShowMagicShieldHP = chkShowMagicShieldHP.Checked; boSendServerConfig = true; });
    public void ChkShowMulitDlgClick() => WriteBack(() => { M2Config.boShowMulitDlg = chkShowMulitDlg.Checked; boSendServerConfig = true; });
    public void ChkShowBetterItemClick() => WriteBack(() => M2Config.boShowBetterItem = chkShowBetterItem.Checked);
    public void ChkShowBagGameGoldSeparatorClick() => WriteBack(() => M2Config.boShowBagGameGoldSeparator = chkShowBagGameGoldSeparator.Checked);
    public void ChkShowBagGameInfoClick() => WriteBack(() => M2Config.boShowBagGameInfo = chkShowBagGameInfo.Checked);
    public void ChkShowHeroShortKeyClick() => WriteBack(() => M2Config.boShowHeroShortKey = chkShowHeroShortKey.Checked);
    public void ChkShowNormalFashionClick() => WriteBack(() => M2Config.boShowNormalFashion = chkShowNormalFashion.Checked);
    public void ChkFashionJewelryOpenClick() => WriteBack(() => M2Config.boFashionJewelryOpen = chkFashionJewelryOpen.Checked);
    public void ChkShowItemFormClick() => WriteBack(() => M2Config.boShowItemForm = chkShowItemForm.Checked);
    public void ChkShowItemSellPriceClick() => WriteBack(() => M2Config.boShowItemSellPrice = chkShowItemSellPrice.Checked);
    public void ChkShowFashionHideShieldClick() => WriteBack(() => M2Config.boShowFashionHideShield = chkShowFashionHideShield.Checked);
    public void ChkShowFashionHideHatsClick() => WriteBack(() => M2Config.boShowFashionHideHats = chkShowFashionHideHats.Checked);
    public void ChkShowInsuranceInfoClick() => WriteBack(() => M2Config.boShowInsuranceInfo = chkShowInsuranceInfo.Checked);
    public void ChkGreenHintNewStyleClick() => WriteBack(() => M2Config.boGreenHintNewStyle = chkGreenHintNewStyle.Checked);
    public void ChkMagicSetDirClick() => WriteBack(() => { M2Config.boMagicSetDir = chkMagicSetDir.Checked; boSendServerConfig = true; });
    public void ChkBlastHitShowHealthNumClick() => WriteBack(() => { M2Config.boBlastHitShowHealthNum = chkBlastHitShowHealthNum.Checked; boSendServerConfig = true; });
    public void ChkPoisoningHideHealthNumClick() => WriteBack(() => M2Config.boPoisoningHideHealthNum = chkPoisoningHideHealthNum.Checked);
    public void ChkHPStoneHideHealthNumClick() => WriteBack(() => M2Config.boHPStoneHideHealthNum = chkHPStoneHideHealthNum.Checked);
    public void ChkMPStoneHideHealthNumClick() => WriteBack(() => M2Config.boMPStoneHideHealthNum = chkMPStoneHideHealthNum.Checked);
    public void ChkHideIconWithHideTitleClick() => WriteBack(() => M2Config.boHideIconWithHideTitle = chkHideIconWithHideTitle.Checked);
    public void ChkHideItemNameNumClick() => WriteBack(() => { M2Config.boHideItemNameNum = chkHideItemNameNum.Checked; boSendServerConfig = true; });
    public void ChkMoveItemShowIDClick() => WriteBack(() => M2Config.boMoveItemShowID = chkMoveItemShowID.Checked);
    public void ChkNpcDlgHintWithMouseClick() => WriteBack(() => M2Config.boNpcDlgHintWithMouse = chkNpcDlgHintWithMouse.Checked);
    public void ChkHelmetShowInBoxClick() => WriteBack(() => M2Config.boHelmetShowInBox = chkHelmetShowInBox.Checked);
    public void ChkHintWithMouseClick() => WriteBack(() => M2Config.boHintWithMouse = chkHintWithMouse.Checked);
    public void CheckBoxEscCloseNPCClick() => WriteBack(() => M2Config.boEscCloseNPC = CheckBoxEscCloseNPC.Checked);

    // ---- 其余 se*/cbb* 直写 ----

    public void SeIncHealingLimiteChange() => WriteBack(() => M2Config.nIncHealingLimite = (int)seIncHealingLimite.Value);
    public void SePerHealingTimeChange() => WriteBack(() => M2Config.nPerHealingTime = (int)sePerHealingTime.Value);
    public void SePerHealingChange() => WriteBack(() => M2Config.nPerHealing = (int)sePerHealing.Value);
    public void SeBigPerHealingTimeChange() => WriteBack(() => M2Config.nBigPerHealingTime = (int)seBigPerHealingTime.Value);
    public void SeBigPerHealingChange() => WriteBack(() => M2Config.nBigPerHealing = (int)seBigPerHealing.Value);
    public void SeBetterItemXChange() => WriteBack(() => { M2Config.nBetterItemX = (int)seBetterItemX.Value; boSendServerConfig = true; });
    public void SeBetterItemYChange() => WriteBack(() => { M2Config.nBetterItemY = (int)seBetterItemY.Value; boSendServerConfig = true; });
    public void SeSmallInfoXChange() => WriteBack(() => { M2Config.nSmallInfoX = (int)seSmallInfoX.Value; boSendServerConfig = true; });
    public void SeSmallInfoYChange() => WriteBack(() => { M2Config.nSmallInfoY = (int)seSmallInfoY.Value; boSendServerConfig = true; });
    public void SeJoyStickXChange() => WriteBack(() => { M2Config.nJoyStickX = (int)seJoyStickX.Value; boSendServerConfig = true; });
    public void SeJoyStickYChange() => WriteBack(() => { M2Config.nJoyStickY = (int)seJoyStickY.Value; boSendServerConfig = true; });
    public void SeJoyStickMaxXChange() => WriteBack(() => { M2Config.nJoyStickMaxX = (int)seJoyStickMaxX.Value; boSendServerConfig = true; });
    public void SeJoyStickMaxYChange() => WriteBack(() => { M2Config.nJoyStickMaxY = (int)seJoyStickMaxY.Value; boSendServerConfig = true; });
    public void SeSkillCtrXChange() => WriteBack(() => { M2Config.nSkillCtrX = (int)seSkillCtrX.Value; boSendServerConfig = true; });
    public void SeSkillCtrYChange() => WriteBack(() => { M2Config.nSkillCtrY = (int)seSkillCtrY.Value; boSendServerConfig = true; });
    public void SeMapScaleChange() => WriteBack(() => { M2Config.btMapScale = (byte)seMapScale.Value; boSendServerConfig = true; });
    public void SeGuiScaleChange() => WriteBack(() => { M2Config.btGuiScale = (byte)seGuiScale.Value; boSendServerConfig = true; });
    public void SeMultiViewRangeChange() => WriteBack(() => { M2Config.btMultiViewRange = (byte)seMultiViewRange.Value; boSendServerConfig = true; });
    public void SeThrowAwayItemColorChange() => WriteBack(() => M2Config.btThrowAwayItemColor = (byte)seThrowAwayItemColor.Value);
    public void SeShowHintNameFontSizeChange() => WriteBack(() => M2Config.btShowHintNameFontSize = (byte)seShowHintNameFontSize.Value);
    public void SeShowHintOtherFontSizeChange() => WriteBack(() => M2Config.btShowHintOtherFontSize = (byte)seShowHintOtherFontSize.Value);
    public void CbbShowHintNameFontBoldChange() => WriteBack(() => M2Config.btShowHintNameFontBold = (byte)Math.Max(0, cbbShowHintNameFontBold.SelectedIndex));
    public void CbbShowHintNameFontStrokeChange() => WriteBack(() => M2Config.btShowHintNameFontStroke = (byte)Math.Max(0, cbbShowHintNameFontStroke.SelectedIndex));
    public void CbbShowHintOtherFontBoldChange() => WriteBack(() => M2Config.btShowHintOtherFontBold = (byte)Math.Max(0, cbbShowHintOtherFontBold.SelectedIndex));
    public void CbbShowHintOtherFontStrokeChange() => WriteBack(() => M2Config.btShowHintOtherFontStroke = (byte)Math.Max(0, cbbShowHintOtherFontStroke.SelectedIndex));
    public void EdtShowHintFontNameChange() => WriteBack(() => { M2Config.sShowHintFontName = edtShowHintFontName.Text; boSendServerConfig = true; });
    public void SeShowItemFormColorChange() => WriteBack(() => M2Config.btShowItemFormColor = (byte)seShowItemFormColor.Value);
    public void SeShowInsuranceInfoColorChange() => WriteBack(() => M2Config.btShowInsuranceInfoColor = (byte)seShowInsuranceInfoColor.Value);
    public void SeShowItemSellPriceColorChange() => WriteBack(() => M2Config.btShowItemSellPriceColor = (byte)seShowItemSellPriceColor.Value);
    public void SeShowHeroShortKeyXChange() => WriteBack(() => M2Config.nShowHeroShortKeyX = (int)seShowHeroShortKeyX.Value);
    public void SeShowHeroShortKeyYChange() => WriteBack(() => M2Config.nShowHeroShortKeyY = (int)seShowHeroShortKeyY.Value);
    public void SeHintWindowBorderWidthLeftChange() => WriteBack(() => M2Config.HintWindowBorderWidth.Left = (byte)seHintWindowBorderWidthLeft.Value);
    public void SeHintWindowBorderWidthTopChange() => WriteBack(() => M2Config.HintWindowBorderWidth.Top = (byte)seHintWindowBorderWidthTop.Value);
    public void SeHintWindowBorderWidthRightChange() => WriteBack(() => M2Config.HintWindowBorderWidth.Right = (byte)seHintWindowBorderWidthRight.Value);
    public void SeHintWindowBorderWidthBottomChange() => WriteBack(() => M2Config.HintWindowBorderWidth.Bottom = (byte)seHintWindowBorderWidthBottom.Value);
    public void CbbTitleFileIndexChange() => WriteBack(() => M2Config.nTitleFileIndex = cbbTitleFileIndex.SelectedIndex);
    public void CbbBagRightkeyChange() => WriteBack(() => M2Config.boBagRightkey = cbbBagRightkey.SelectedIndex == 1);
}

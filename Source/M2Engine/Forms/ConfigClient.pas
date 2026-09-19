unit ConfigClient;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls, Spin, ComCtrls, ExtCtrls, RzRadGrp,
  RzSpnEdt, ColorIndexEdit, SpinEditEx, CheckUnit, RzPanel;

type
  TFrmConfigClient = class(TForm)
    ClientPageControl: TPageControl;
    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    TabSheet3: TTabSheet;
    GroupBox75: TGroupBox;
    CheckBoxControlHelpButton: TCheckBox;
    CheckBoxRankButton: TCheckBox;
    CheckBoxWhisperButton: TCheckBox;
    CheckBoxActionLogButton: TCheckBox;
    CheckBoxMissionButton: TCheckBox;
    CheckBoxWebButton: TCheckBox;
    CheckBoxOpenShopButton: TCheckBox;
    CheckBoxUserShopButton: TCheckBox;
    ButtonPrguseSave: TButton;
    ButtonClientHintWindowsSave: TButton;
    CheckBoxFriendButton: TCheckBox;
    TabSheet4: TTabSheet;
    ListBoxClientItemName: TListBox;
    ButtonClientItemNameeUP: TButton;
    ButtonClientItemNameDown: TButton;
    Label27: TLabel;
    EditClientItemName: TEdit;
    ButtonClientItemNameAdd: TButton;
    ButtonClientItemNameDel: TButton;
    ButtonClientItemNameSave: TButton;
    Label4: TLabel;
    Label5: TLabel;
    EditHomePage: TEdit;
    CheckBoxOpenHeroButton: TCheckBox;
    CheckBoxShowMerchantDlgHelp: TCheckBox;
    GroupBox3: TGroupBox;
    ListViewSpecialCmd: TListView;
    EditSpecialCmdCaption: TEdit;
    EditSpecialCmd: TEdit;
    Label6: TLabel;
    Label7: TLabel;
    ButtonSpecialCmdAdd: TButton;
    ButtonSpecialCmdDel: TButton;
    CheckBoxDBotFunc1: TCheckBox;
    CheckBoxDBotFunc2: TCheckBox;
    CheckBoxDBotFunc3: TCheckBox;
    CheckBoxDBotFunc4: TCheckBox;
    CheckBoxDBotFunc6: TCheckBox;
    CheckBoxDBotFunc5: TCheckBox;
    ButtonSpecialCmdChg: TButton;
    ButtonSpecialCmdSave: TButton;
    TabSheet5: TTabSheet;
    GroupBox4: TGroupBox;
    CheckBoxViewFog: TCheckBox;
    ButtonWeatherSave: TButton;
    GroupBox5: TGroupBox;
    ListBoxBright: TListBox;
    RadioGroupBright: TRadioGroup;
    CheckBoxChallengeButton: TCheckBox;
    TabSheet6: TTabSheet;
    GroupBox6: TGroupBox;
    CheckBoxMonStruckShowNumber: TCheckBox;
    CheckBoxHumStruckShowNumber: TCheckBox;
    CheckBoxCloseBookProtect: TCheckBox;
    CheckBoxCloseLogoutProtect: TCheckBox;
    ButtonGameAuxiliarySave2: TButton;
    GroupBox7: TGroupBox;
    GroupBox55: TGroupBox;
    Label108: TLabel;
    Label109: TLabel;
    seAddItemMsgFColor: TColorIndexEdit;
    seAddItemMsgBColor: TColorIndexEdit;
    GroupBox8: TGroupBox;
    Label8: TLabel;
    Label9: TLabel;
    seGetExpMsgFColor: TColorIndexEdit;
    seGetExpMsgBColor: TColorIndexEdit;
    GroupBox9: TGroupBox;
    Label12: TLabel;
    Label13: TLabel;
    seUpLevelMsgFColor: TColorIndexEdit;
    seUpLevelMsgBColor: TColorIndexEdit;
    Label29: TLabel;
    Label30: TLabel;
    Label31: TLabel;
    seAddItemMsgX: TSpinEditEx;
    Label32: TLabel;
    seAddItemMsgY: TSpinEditEx;
    Label33: TLabel;
    seGetExpMsgX: TSpinEditEx;
    Label34: TLabel;
    seGetExpMsgY: TSpinEditEx;
    Label35: TLabel;
    seUpLevelMsgX: TSpinEditEx;
    Label36: TLabel;
    seUpLevelMsgY: TSpinEditEx;
    GroupBox10: TGroupBox;
    Label16: TLabel;
    Label17: TLabel;
    Label20: TLabel;
    Label21: TLabel;
    seHeroAddItemMsgFColor: TColorIndexEdit;
    seHeroAddItemMsgBColor: TColorIndexEdit;
    seHeroAddItemMsgX: TSpinEditEx;
    seHeroAddItemMsgY: TSpinEditEx;
    GroupBox11: TGroupBox;
    Label22: TLabel;
    Label23: TLabel;
    Label26: TLabel;
    Label28: TLabel;
    seHeroGetExpMsgFColor: TColorIndexEdit;
    seHeroGetExpMsgBColor: TColorIndexEdit;
    seHeroGetExpMsgX: TSpinEditEx;
    seHeroGetExpMsgY: TSpinEditEx;
    GroupBox12: TGroupBox;
    Label37: TLabel;
    Label38: TLabel;
    Label41: TLabel;
    Label42: TLabel;
    seHeroUpLevelMsgFColor: TColorIndexEdit;
    seHeroUpLevelMsgBColor: TColorIndexEdit;
    seHeroUpLevelMsgX: TSpinEditEx;
    seHeroUpLevelMsgY: TSpinEditEx;
    CheckBoxGetExpMsgAddChatBoardMsg: TCheckBox;
    CheckGroupNewAbil: TRzCheckGroup;
    chkAddItemMsgXRightToLeft: TCheckBox;
    chkGetExpMsgXRightToLeft: TCheckBox;
    chkUpLevelMsgXRightToLeft: TCheckBox;
    chkHeroAddItemMsgXRightToLeft: TCheckBox;
    chkHeroGetExpMsgXRightToLeft: TCheckBox;
    chkHeroUpLevelMsgXRightToLeft: TCheckBox;
    GroupBox14: TGroupBox;
    tsDrugAndRestore: TTabSheet;
    GroupBox81: TGroupBox;
    Label168: TLabel;
    sePerHealth: TSpinEditEx;
    GroupBox89: TGroupBox;
    Label211: TLabel;
    seHealthFillTime: TSpinEditEx;
    btnSaveDrugAndRestore: TButton;
    Label212: TLabel;
    seSpellFillTime: TSpinEditEx;
    GroupBox16: TGroupBox;
    Label15: TLabel;
    seHealthFillTime_Human_Warrior: TSpinEditEx;
    Label18: TLabel;
    seSpellFillTime_Human_Warrior: TSpinEditEx;
    Label19: TLabel;
    seHealthFillTime_Human_TaoistAndWizard: TSpinEditEx;
    Bevel1: TBevel;
    GroupBox17: TGroupBox;
    Label25: TLabel;
    Label39: TLabel;
    Label40: TLabel;
    Label43: TLabel;
    Bevel2: TBevel;
    seHealthFillTime_Hero_Warrior: TSpinEditEx;
    seSpellFillTime_Hero_Warrior: TSpinEditEx;
    seHealthFillTime_Hero_TaoistAndWizard: TSpinEditEx;
    seSpellFillTime_Hero_TaoistAndWizard: TSpinEditEx;
    Label44: TLabel;
    seHealthBaseNum_Human_TaoistAndWizard: TSpinEditEx;
    Label46: TLabel;
    Label47: TLabel;
    Bevel4: TBevel;
    seHealthBaseNum_Hero_TaoistAndWizard: TSpinEditEx;
    seSpellBaseNum_Hero_TaoistAndWizard: TSpinEditEx;
    Label48: TLabel;
    Label49: TLabel;
    Bevel5: TBevel;
    seHealthBaseNum: TSpinEditEx;
    seSpellBaseNum: TSpinEditEx;
    Label50: TLabel;
    seHealthBaseNum_Human_Warrior: TSpinEditEx;
    Label51: TLabel;
    seSpellBaseNum_Human_Warrior: TSpinEditEx;
    Bevel6: TBevel;
    Label24: TLabel;
    seSpellFillTime_Human_TaoistAndWizard: TSpinEditEx;
    Label45: TLabel;
    seSpellBaseNum_Human_TaoistAndWizard: TSpinEditEx;
    Bevel3: TBevel;
    Label52: TLabel;
    seHealthBaseNum_Hero_Warrior: TSpinEditEx;
    Bevel7: TBevel;
    Label53: TLabel;
    seSpellBaseNum_Hero_Warrior: TSpinEditEx;
    btnRestoreDefault: TButton;
    Label169: TLabel;
    sePerSpell: TSpinEditEx;
    GroupBox18: TGroupBox;
    Label54: TLabel;
    Label55: TLabel;
    Label58: TLabel;
    Label59: TLabel;
    Bevel9: TBevel;
    seUseOrdinaryTime_Human_Warrior: TSpinEditEx;
    seUseOrdinaryTime_Human_TaoistAndWizard: TSpinEditEx;
    seUseSpecialTime_Human_Warrior: TSpinEditEx;
    seUseSpecialTime_Human_TaoistAndWizard: TSpinEditEx;
    Label207: TLabel;
    GroupBox19: TGroupBox;
    Label56: TLabel;
    Label57: TLabel;
    Label60: TLabel;
    Label61: TLabel;
    Bevel8: TBevel;
    seUseOrdinaryTime_Hero_Warrior: TSpinEditEx;
    seUseOrdinaryTime_Hero_TaoistAndWizard: TSpinEditEx;
    seUseSpecialTime_Hero_Warrior: TSpinEditEx;
    seUseSpecialTime_Hero_TaoistAndWizard: TSpinEditEx;
    Label62: TLabel;
    seIncHealthSpell: TSpinEditEx;
    tsOption2: TTabSheet;
    GroupBox20: TGroupBox;
    chkMinMapCloseRadar: TCheckBox;
    GroupBox21: TGroupBox;
    Label65: TLabel;
    seMinMapColorSelf: TColorIndexEdit;
    Label68: TLabel;
    seMinMapColorOther: TColorIndexEdit;
    Label63: TLabel;
    seMinMapFlagFlash: TSpinEditEx;
    Label66: TLabel;
    seMinMapColorNPC: TColorIndexEdit;
    Label72: TLabel;
    seMinMapColorGuard: TColorIndexEdit;
    Label74: TLabel;
    seMinMapColorMonster: TColorIndexEdit;
    Label76: TLabel;
    seMinMapColorHero: TColorIndexEdit;
    btnSaveOption2: TButton;
    chkUseFindPath: TCheckBox;
    chkGemUpgrade: TCheckBox;
    chkShowGuildName: TCheckBox;
    chkShopGuiCanMove: TCheckBox;
    chkNPCGuiCanMove: TCheckBox;
    GroupBox180: TGroupBox;
    Label569: TLabel;
    sePerHealingTime: TSpinEditEx;
    Label64: TLabel;
    sePerHealing: TSpinEditEx;
    Label67: TLabel;
    seBigPerHealingTime: TSpinEditEx;
    Label69: TLabel;
    seBigPerHealing: TSpinEditEx;
    lbl1: TLabel;
    seMinMapColorBoss: TColorIndexEdit;
    pgc1: TPageControl;
    ts1: TTabSheet;
    RzCheckGroupClientConfig: TRzCheckGroup;
    RzCheckGroupClientTabSheet: TRzCheckGroup;
    GroupBox2: TGroupBox;
    CheckBoxStartGameAuxiliary: TCheckBox;
    CheckBoxCanOpenGameConfigDlg: TCheckBox;
    CheckBoxNotCanUseClientConfig: TCheckBox;
    RadioButtonPlugIn1: TRadioButton;
    RadioButtonPlugIn2: TRadioButton;
    ts2: TTabSheet;
    GroupBox1: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    lbl9: TLabel;
    lbl8: TLabel;
    TrackBarMoveSpeed: TTrackBar;
    RzSpinnerMoveSpeed: TRzSpinner;
    TrackBarAttackSpeed: TTrackBar;
    RzSpinnerAttackSpeed: TRzSpinner;
    TrackBarSpellSpeed: TTrackBar;
    RzSpinnerSpellSpeed: TRzSpinner;
    seHitFrameTime: TSpinEditEx;
    seMagicHitFrameTime: TSpinEditEx;
    ButtonGameAuxiliarySave: TButton;
    chkShowGlory: TCheckBox;
    chkShowHorseButton: TCheckBox;
    chkHideItemNameNum: TCheckBox;
    chkShowDeputyHeroButton: TCheckBox;
    chkKeyTabGetActor: TCheckBox;
    grp2: TGroupBox;
    lbl2: TLabel;
    cbbTitleFileIndex: TComboBox;
    GroupBox15: TGroupBox;
    Label75: TLabel;
    Label77: TLabel;
    chkShowHeroShortKey: TCheckBox;
    seShowHeroShortKeyX: TSpinEditEx;
    seShowHeroShortKeyY: TSpinEditEx;
    grp4: TGroupBox;
    Label78: TLabel;
    Label79: TLabel;
    sePluginMinEatItemTime: TSpinEditEx;
    GroupBox22: TGroupBox;
    Label100: TLabel;
    lbl17: TLabel;
    seIncSpeedDecInterval: TSpinEditEx;
    chkGreenHintNewStyle: TCheckBox;
    chkMagicSetDir: TCheckBox;
    chkHideIconWithHideTitle: TCheckBox;
    grp6: TGroupBox;
    chkHealthNumberText: TCheckBox;
    chkBlastHitShowHealthNum: TCheckBox;
    chkPoisoningHideHealthNum: TCheckBox;
    chkHPStoneHideHealthNum: TCheckBox;
    chkMPStoneHideHealthNum: TCheckBox;
    chkShowBagArrange: TCheckBox;
    Label110: TLabel;
    Label111: TLabel;
    seIncMoveSpeedDecInterval: TSpinEditEx;
    Label112: TLabel;
    seIncSpellSpeedDecInterval: TSpinEditEx;
    Label113: TLabel;
    lbl4: TLabel;
    sePluginPickupTime: TSpinEditEx;
    Label71: TLabel;
    Label114: TLabel;
    seMoveFrameTime: TSpinEditEx;
    chkAddItemMsgYBottomToTop: TCheckBox;
    chkGetExpMsgYBottomToTop: TCheckBox;
    chkUpLevelMsgYBottomToTop: TCheckBox;
    chkHeroAddItemMsgYBottomToTop: TCheckBox;
    chkHeroGetExpMsgYBottomToTop: TCheckBox;
    chkHeroUpLevelMsgYBottomToTop: TCheckBox;
    lbl5: TLabel;
    chkShowBagGameGoldSeparator: TCheckBox;
    lbl7: TLabel;
    lbl10: TLabel;
    seHealthNumberOffsetX: TSpinEdit;
    Label116: TLabel;
    seHealthNumberOffsetY: TSpinEdit;
    Label117: TLabel;
    seHealthNumberMoveSpeed: TSpinEditEx;
    lbl11: TLabel;
    GroupBox25: TGroupBox;
    Label14: TLabel;
    Label101: TLabel;
    seUseItemIntervalTime: TSpinEditEx;
    seUseAttackItemIntervalTime: TSpinEditEx;
    chkShowBagGameInfo: TCheckBox;
    lbl3: TLabel;
    cbbMinMapType: TComboBox;
    ts3: TTabSheet;
    grpArrButton1: TGroupBox;
    lblArrBtnHorzAlign1: TLabel;
    cbbArrBtnHorzAlign1: TComboBox;
    lblArrBtnOffsetX1: TLabel;
    seArrBtnOffsetX1: TSpinEditEx;
    lblArrBtnVertAlign1: TLabel;
    cbbArrBtnVertAlign1: TComboBox;
    lblArrBtnOffsetY1: TLabel;
    seArrBtnOffsetY1: TSpinEditEx;
    lbl1ArrBtnStart1: TLabel;
    lbl1NextArrBtnOffset1: TLabel;
    lblNextArrBtnOffsetX1: TLabel;
    seNextArrBtnOffsetX1: TSpinEditEx;
    lblNextArrBtnOffsetY1: TLabel;
    seNextArrBtnOffsetY1: TSpinEditEx;
    grpArrButton2: TGroupBox;
    lblArrBtnHorzAlign2: TLabel;
    lblArrBtnOffsetX2: TLabel;
    lblArrBtnVertAlign2: TLabel;
    lblArrBtnOffsetY2: TLabel;
    lbl1ArrBtnStart2: TLabel;
    lbl1NextArrBtnOffset2: TLabel;
    lblNextArrBtnOffsetX2: TLabel;
    lblNextArrBtnOffsetY2: TLabel;
    cbbArrBtnHorzAlign2: TComboBox;
    seArrBtnOffsetX2: TSpinEditEx;
    cbbArrBtnVertAlign2: TComboBox;
    seArrBtnOffsetY2: TSpinEditEx;
    seNextArrBtnOffsetX2: TSpinEditEx;
    seNextArrBtnOffsetY2: TSpinEditEx;
    grpArrButton3: TGroupBox;
    lblArrBtnHorzAlign3: TLabel;
    lblArrBtnOffsetX3: TLabel;
    lblArrBtnVertAlign3: TLabel;
    lblArrBtnOffsetY3: TLabel;
    lbl1ArrBtnStart3: TLabel;
    lbl1NextArrBtnOffset3: TLabel;
    lblNextArrBtnOffsetX3: TLabel;
    lblNextArrBtnOffsetY3: TLabel;
    cbbArrBtnHorzAlign3: TComboBox;
    seArrBtnOffsetX3: TSpinEditEx;
    cbbArrBtnVertAlign3: TComboBox;
    seArrBtnOffsetY3: TSpinEditEx;
    seNextArrBtnOffsetX3: TSpinEditEx;
    seNextArrBtnOffsetY3: TSpinEditEx;
    grpArrButton4: TGroupBox;
    lblArrBtnHorzAlign4: TLabel;
    lblArrBtnOffsetX4: TLabel;
    lblArrBtnVertAlign4: TLabel;
    lblArrBtnOffsetY4: TLabel;
    lbl1ArrBtnStart4: TLabel;
    lbl1NextArrBtnOffset4: TLabel;
    lblNextArrBtnOffsetX4: TLabel;
    lblNextArrBtnOffsetY4: TLabel;
    cbbArrBtnHorzAlign4: TComboBox;
    seArrBtnOffsetX4: TSpinEditEx;
    cbbArrBtnVertAlign4: TComboBox;
    seArrBtnOffsetY4: TSpinEditEx;
    seNextArrBtnOffsetX4: TSpinEditEx;
    seNextArrBtnOffsetY4: TSpinEditEx;
    grpArrButton5: TGroupBox;
    lblArrBtnHorzAlign5: TLabel;
    lblArrBtnOffsetX5: TLabel;
    lblArrBtnVertAlign5: TLabel;
    lblArrBtnOffsetY5: TLabel;
    lbl1ArrBtnStart5: TLabel;
    lbl1NextArrBtnOffset5: TLabel;
    lblNextArrBtnOffsetX5: TLabel;
    lblNextArrBtnOffsetY5: TLabel;
    cbbArrBtnHorzAlign5: TComboBox;
    seArrBtnOffsetX5: TSpinEditEx;
    cbbArrBtnVertAlign5: TComboBox;
    seArrBtnOffsetY5: TSpinEditEx;
    seNextArrBtnOffsetX5: TSpinEditEx;
    seNextArrBtnOffsetY5: TSpinEditEx;
    grpArrButton6: TGroupBox;
    lblArrBtnHorzAlign6: TLabel;
    lblArrBtnOffsetX6: TLabel;
    lblArrBtnVertAlign6: TLabel;
    lblArrBtnOffsetY6: TLabel;
    lbl1ArrBtnStart6: TLabel;
    lbl1NextArrBtnOffset6: TLabel;
    lblNextArrBtnOffsetX6: TLabel;
    lblNextArrBtnOffsetY6: TLabel;
    cbbArrBtnHorzAlign6: TComboBox;
    seArrBtnOffsetX6: TSpinEditEx;
    cbbArrBtnVertAlign6: TComboBox;
    seArrBtnOffsetY6: TSpinEditEx;
    seNextArrBtnOffsetX6: TSpinEditEx;
    seNextArrBtnOffsetY6: TSpinEditEx;
    grpArrButton7: TGroupBox;
    lblArrBtnHorzAlign7: TLabel;
    lblArrBtnOffsetX7: TLabel;
    lblArrBtnVertAlign7: TLabel;
    lblArrBtnOffsetY7: TLabel;
    lbl1ArrBtnStart7: TLabel;
    lbl1NextArrBtnOffset7: TLabel;
    lblNextArrBtnOffsetX7: TLabel;
    lblNextArrBtnOffsetY7: TLabel;
    cbbArrBtnHorzAlign7: TComboBox;
    seArrBtnOffsetX7: TSpinEditEx;
    cbbArrBtnVertAlign7: TComboBox;
    seArrBtnOffsetY7: TSpinEditEx;
    seNextArrBtnOffsetX7: TSpinEditEx;
    seNextArrBtnOffsetY7: TSpinEditEx;
    btnArrBtnSetting: TButton;
    lbl12: TLabel;
    chkMinMapUseFindPath: TCheckBox;
    chkLoginShowMinMap: TCheckBox;
    grp5: TGroupBox;
    Label84: TLabel;
    seNewLeftGroupInfoOffsetX: TSpinEditEx;
    Label85: TLabel;
    seNewLeftGroupInfoOffsetY: TSpinEditEx;
    lbl13: TLabel;
    cbbBagRightkey: TComboBox;
    Label70: TLabel;
    RadioGroupShowItemStyle: TRadioGroup;
    rgStateWindows: TRadioGroup;
    GroupBox13: TGroupBox;
    chkHideTabSheet2: TCheckBox;
    chkHideTabSheet5: TCheckBox;
    chkHideTabSheet7: TCheckBox;
    chkHeroHideTabSheet5: TCheckBox;
    chkHeroHideTabSheet2: TCheckBox;
    CheckBoxUseOldSerialWindows: TCheckBox;
    CheckBoxEscCloseNPC: TCheckBox;
    chkNpcDlgHintWithMouse: TCheckBox;
    seThrowAwayItemColor: TColorIndexEdit;
    grp1: TGroupBox;
    chkShowHintWindowFrame: TCheckBox;
    chkShowHintLines: TCheckBox;
    chkShowItemForm: TCheckBox;
    chkShowInsuranceInfo: TCheckBox;
    chkShowItemSellPrice: TCheckBox;
    chkHintWithMouse: TCheckBox;
    seShowItemFormColor: TColorIndexEdit;
    seShowInsuranceInfoColor: TColorIndexEdit;
    seShowItemSellPriceColor: TColorIndexEdit;
    grp3: TGroupBox;
    chkShowNormalFashion: TCheckBox;
    chkFashionJewelryOpen: TCheckBox;
    chkShowFashionHideShield: TCheckBox;
    chkShowFashionHideHats: TCheckBox;
    GroupBox23: TGroupBox;
    Label86: TLabel;
    Label87: TLabel;
    seHintWindowBGColor: TColorIndexEdit;
    seHintWindowBGAlpha: TSpinEditEx;
    GroupBox24: TGroupBox;
    lbl6: TLabel;
    Label88: TLabel;
    Label73: TLabel;
    seShowHintNameFontSize: TSpinEditEx;
    edtShowHintFontName: TEdit;
    seShowHintOtherFontSize: TSpinEditEx;
    cbbShowHintNameFontBold: TComboBox;
    cbbShowHintNameFontStroke: TComboBox;
    cbbShowHintOtherFontBold: TComboBox;
    cbbShowHintOtherFontStroke: TComboBox;
    chkMoveItemShowID: TCheckBox;
    GroupBox26: TGroupBox;
    chkItemFromField0: TCheckBox;
    chkItemFromField3: TCheckBox;
    chkItemFromField6: TCheckBox;
    chkItemFromField1: TCheckBox;
    chkItemFromField2: TCheckBox;
    chkItemFromField4: TCheckBox;
    chkItemFromField5: TCheckBox;
    GroupBox27: TGroupBox;
    chkShowMagicShieldHP: TCheckBox;
    Label89: TLabel;
    Label90: TLabel;
    seHumHPBarOffsetX: TSpinEditEx;
    seHumHPBarOffsetY: TSpinEditEx;
    lbl14: TLabel;
    bvl1: TBevel;
    Label91: TLabel;
    seNpcHPBarOffsetX: TSpinEditEx;
    Label92: TLabel;
    seNpcHPBarOffsetY: TSpinEditEx;
    Label93: TLabel;
    seMonHPBarOffsetX: TSpinEditEx;
    Label94: TLabel;
    seMonHPBarOffsetY: TSpinEditEx;
    GroupBox28: TGroupBox;
    Label95: TLabel;
    Label96: TLabel;
    Label98: TLabel;
    Label99: TLabel;
    Label102: TLabel;
    Label103: TLabel;
    seHumNameOffsetX: TSpinEditEx;
    seHumNameOffsetY: TSpinEditEx;
    seNpcNameOffsetX: TSpinEditEx;
    seNpcNameOffsetY: TSpinEditEx;
    seMonNameOffsetX: TSpinEditEx;
    seMonNameOffsetY: TSpinEditEx;
    Label97: TLabel;
    seIncHealingLimite: TSpinEditEx;
    GroupBox29: TGroupBox;
    Label104: TLabel;
    seHintWindowBorderWidthLeft: TSpinEditEx;
    Label105: TLabel;
    seHintWindowBorderWidthTop: TSpinEditEx;
    Label106: TLabel;
    seHintWindowBorderWidthRight: TSpinEditEx;
    Label107: TLabel;
    seHintWindowBorderWidthBottom: TSpinEditEx;
    chkHelmetShowInBox: TCheckBox;
    lbl15: TLabel;
    lbl16: TLabel;
    Label10: TLabel;
    RadioGroupBagFastItemCompare: TRadioGroup;
    GroupBox30: TGroupBox;
    Label11: TLabel;
    Label80: TLabel;
    Label81: TLabel;
    Label82: TLabel;
    Label83: TLabel;
    Label115: TLabel;
    Label118: TLabel;
    Label119: TLabel;
    Label120: TLabel;
    Label121: TLabel;
    Label122: TLabel;
    Label123: TLabel;
    seBetterItemX: TSpinEdit;
    seBetterItemY: TSpinEdit;
    seSmallInfoX: TSpinEdit;
    seSmallInfoY: TSpinEdit;
    seJoyStickX: TSpinEdit;
    seJoyStickY: TSpinEdit;
    seJoyStickMaxX: TSpinEdit;
    seJoyStickMaxY: TSpinEdit;
    seSkillCtrX: TSpinEdit;
    seSkillCtrY: TSpinEdit;
    seMapScale: TSpinEdit;
    seGuiScale: TSpinEdit;
    chkShowExSkillIcon: TCheckBox;
    Label124: TLabel;
    Label125: TLabel;
    Label126: TLabel;
    Label127: TLabel;
    Label128: TLabel;
    Label129: TLabel;
    chkShowMulitDlg: TCheckBox;
    seMultiViewRange: TSpinEdit;
    Label130: TLabel;
    chkShowBetterItem: TCheckBox;
    procedure ClientPageControlChanging(Sender: TObject; var AllowChange: Boolean);
    procedure ButtonPrguseSaveClick(Sender: TObject);
    procedure ButtonGameAuxiliarySaveClick(Sender: TObject);
    procedure CheckBoxControlHelpButtonClick(Sender: TObject);
    procedure CheckBoxRankButtonClick(Sender: TObject);
    procedure CheckBoxWhisperButtonClick(Sender: TObject);
    procedure CheckBoxActionLogButtonClick(Sender: TObject);
    procedure CheckBoxMissionButtonClick(Sender: TObject);
    procedure CheckBoxWebButtonClick(Sender: TObject);
    procedure CheckBoxOpenShopButtonClick(Sender: TObject);
    procedure CheckBoxUserShopButtonClick(Sender: TObject);
    procedure CheckBoxStartGameAuxiliaryClick(Sender: TObject);
    procedure seHintWindowBGColorChange(Sender: TObject);
    procedure seHintWindowBGAlphaChange(Sender: TObject);
    procedure chkShowHintWindowFrameClick(Sender: TObject);
    procedure ButtonClientHintWindowsSaveClick(Sender: TObject);
    procedure RzCheckGroupClientConfigChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
    procedure TrackBarMoveSpeedChange(Sender: TObject);
    procedure TrackBarAttackSpeedChange(Sender: TObject);
    procedure TrackBarSpellSpeedChange(Sender: TObject);
    procedure RzCheckGroupClientTabSheetChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
    procedure RadioGroupShowItemStyleClick(Sender: TObject);
    procedure CheckBoxFriendButtonClick(Sender: TObject);
    procedure ButtonClientItemNameeUPClick(Sender: TObject);
    procedure ButtonClientItemNameDownClick(Sender: TObject);
    procedure ButtonClientItemNameAddClick(Sender: TObject);
    procedure ListBoxClientItemNameClick(Sender: TObject);
    procedure ButtonClientItemNameSaveClick(Sender: TObject);
    procedure ButtonClientItemNameDelClick(Sender: TObject);
    procedure EditHomePageChange(Sender: TObject);
    procedure CheckBoxOpenHeroButtonClick(Sender: TObject);
    procedure RzSpinnerMoveSpeedChange(Sender: TObject);
    procedure RzSpinnerAttackSpeedChange(Sender: TObject);
    procedure RzSpinnerSpellSpeedChange(Sender: TObject);
    procedure CheckBoxCanOpenGameConfigDlgClick(Sender: TObject);
    procedure CheckBoxNotCanUseClientConfigClick(Sender: TObject);
    procedure CheckBoxShowMerchantDlgHelpClick(Sender: TObject);
    procedure CheckBoxDBotFunc1Click(Sender: TObject);
    procedure ListViewSpecialCmdClick(Sender: TObject);
    procedure ButtonSpecialCmdDelClick(Sender: TObject);
    procedure ButtonSpecialCmdChgClick(Sender: TObject);
    procedure ButtonSpecialCmdAddClick(Sender: TObject);
    procedure ButtonSpecialCmdSaveClick(Sender: TObject);
    procedure CheckBoxViewFogClick(Sender: TObject);
    procedure ButtonWeatherSaveClick(Sender: TObject);
    procedure ListBoxBrightClick(Sender: TObject);
    procedure RadioGroupBrightClick(Sender: TObject);
    procedure RadioButtonPlugIn1Click(Sender: TObject);
    procedure RadioButtonPlugIn2Click(Sender: TObject);
    procedure CheckBoxChallengeButtonClick(Sender: TObject);
    procedure ButtonGameAuxiliarySave2Click(Sender: TObject);
    procedure CheckBoxMonStruckShowNumberClick(Sender: TObject);
    procedure CheckBoxHumStruckShowNumberClick(Sender: TObject);
    procedure CheckBoxCloseBookProtectClick(Sender: TObject);
    procedure CheckBoxCloseLogoutProtectClick(Sender: TObject);
    procedure seAddItemMsgFColorChange(Sender: TObject);
    procedure seAddItemMsgBColorChange(Sender: TObject);
    procedure seGetExpMsgFColorChange(Sender: TObject);
    procedure seGetExpMsgBColorChange(Sender: TObject);
    procedure seUpLevelMsgFColorChange(Sender: TObject);
    procedure seUpLevelMsgBColorChange(Sender: TObject);
    procedure seHeroAddItemMsgFColorChange(Sender: TObject);
    procedure seHeroAddItemMsgBColorChange(Sender: TObject);
    procedure seHeroGetExpMsgFColorChange(Sender: TObject);
    procedure seHeroGetExpMsgBColorChange(Sender: TObject);
    procedure seHeroUpLevelMsgFColorChange(Sender: TObject);
    procedure seHeroUpLevelMsgBColorChange(Sender: TObject);
    procedure seAddItemMsgXChange(Sender: TObject);
    procedure seAddItemMsgYChange(Sender: TObject);
    procedure seHeroAddItemMsgXChange(Sender: TObject);
    procedure seHeroAddItemMsgYChange(Sender: TObject);
    procedure seGetExpMsgXChange(Sender: TObject);
    procedure seGetExpMsgYChange(Sender: TObject);
    procedure seHeroGetExpMsgXChange(Sender: TObject);
    procedure seHeroGetExpMsgYChange(Sender: TObject);
    procedure seUpLevelMsgXChange(Sender: TObject);
    procedure seUpLevelMsgYChange(Sender: TObject);
    procedure seHeroUpLevelMsgXChange(Sender: TObject);
    procedure seHeroUpLevelMsgYChange(Sender: TObject);
    procedure CheckBoxGetExpMsgAddChatBoardMsgClick(Sender: TObject);
    procedure chkHideTabSheet2Click(Sender: TObject);
    procedure chkHideTabSheet5Click(Sender: TObject);
    procedure chkHideTabSheet7Click(Sender: TObject);
    procedure CheckGroupNewAbilChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
    procedure chkAddItemMsgXRightToLeftClick(Sender: TObject);
    procedure chkGetExpMsgXRightToLeftClick(Sender: TObject);
    procedure chkUpLevelMsgXRightToLeftClick(Sender: TObject);
    procedure chkHeroAddItemMsgXRightToLeftClick(Sender: TObject);
    procedure chkHeroGetExpMsgXRightToLeftClick(Sender: TObject);
    procedure chkHeroUpLevelMsgXRightToLeftClick(Sender: TObject);
    procedure chkUseFindPathClick(Sender: TObject);
    procedure CheckBoxUseOldSerialWindowsClick(Sender: TObject);
    procedure rgStateWindowsClick(Sender: TObject);
    procedure CheckBoxEscCloseNPCClick(Sender: TObject);
    procedure chkHintWithMouseClick(Sender: TObject);
    procedure sePerHealthChange(Sender: TObject);
    procedure sePerSpellChange(Sender: TObject);
    procedure seIncHealthSpellChange(Sender: TObject);
    procedure seHealthFillTimeChange(Sender: TObject);
    procedure seSpellFillTimeChange(Sender: TObject);
    procedure btnSaveDrugAndRestoreClick(Sender: TObject);
    procedure seUseItemIntervalTimeChange(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure seHealthFillTime_Human_WarriorChange(Sender: TObject);
    procedure seSpellFillTime_Human_WarriorChange(Sender: TObject);
    procedure seHealthFillTime_Human_TaoistAndWizardChange(Sender: TObject);
    procedure seSpellFillTime_Human_TaoistAndWizardChange(Sender: TObject);
    procedure seHealthFillTime_Hero_WarriorChange(Sender: TObject);
    procedure seSpellFillTime_Hero_WarriorChange(Sender: TObject);
    procedure seHealthFillTime_Hero_TaoistAndWizardChange(Sender: TObject);
    procedure seSpellFillTime_Hero_TaoistAndWizardChange(Sender: TObject);
    procedure seHealthBaseNum_Human_WarriorChange(Sender: TObject);
    procedure seSpellBaseNum_Human_WarriorChange(Sender: TObject);
    procedure seHealthBaseNum_Human_TaoistAndWizardChange(Sender: TObject);
    procedure seSpellBaseNum_Human_TaoistAndWizardChange(Sender: TObject);
    procedure seHealthBaseNum_Hero_WarriorChange(Sender: TObject);
    procedure seSpellBaseNum_Hero_WarriorChange(Sender: TObject);
    procedure seHealthBaseNum_Hero_TaoistAndWizardChange(Sender: TObject);
    procedure seSpellBaseNum_Hero_TaoistAndWizardChange(Sender: TObject);
    procedure seHealthBaseNumChange(Sender: TObject);
    procedure seSpellBaseNumChange(Sender: TObject);
    procedure seUseOrdinaryTime_Human_WarriorChange(Sender: TObject);
    procedure seUseSpecialTime_Human_WarriorChange(Sender: TObject);
    procedure seUseOrdinaryTime_Human_TaoistAndWizardChange(Sender: TObject);
    procedure seUseSpecialTime_Human_TaoistAndWizardChange(Sender: TObject);
    procedure seUseOrdinaryTime_Hero_WarriorChange(Sender: TObject);
    procedure seUseSpecialTime_Hero_WarriorChange(Sender: TObject);
    procedure seUseOrdinaryTime_Hero_TaoistAndWizardChange(Sender: TObject);
    procedure seUseSpecialTime_Hero_TaoistAndWizardChange(Sender: TObject);
    procedure btnRestoreDefaultClick(Sender: TObject);
    procedure chkShowHintLinesClick(Sender: TObject);
    procedure seMinMapColorSelfChange(Sender: TObject);
    procedure seMinMapColorOtherChange(Sender: TObject);
    procedure seMinMapColorNPCChange(Sender: TObject);
    procedure seMinMapColorMonsterChange(Sender: TObject);
    procedure seMinMapColorGuardChange(Sender: TObject);
    procedure seMinMapColorHeroChange(Sender: TObject);
    procedure chkMinMapCloseRadarClick(Sender: TObject);
    procedure seMinMapFlagFlashChange(Sender: TObject);
    procedure btnSaveOption2Click(Sender: TObject);
    procedure seHitFrameTimeChange(Sender: TObject);
    procedure seMagicHitFrameTimeChange(Sender: TObject);
    procedure chkGemUpgradeClick(Sender: TObject);
    procedure chkShowGuildNameClick(Sender: TObject);
    procedure chkShopGuiCanMoveClick(Sender: TObject);
    procedure chkNPCGuiCanMoveClick(Sender: TObject);
    procedure sePerHealingTimeChange(Sender: TObject);
    procedure sePerHealingChange(Sender: TObject);
    procedure seBigPerHealingTimeChange(Sender: TObject);
    procedure seBigPerHealingChange(Sender: TObject);
    procedure chkNpcDlgHintWithMouseClick(Sender: TObject);
    procedure seThrowAwayItemColorChange(Sender: TObject);
    procedure seMinMapColorBossChange(Sender: TObject);
    procedure chkShowGloryClick(Sender: TObject);
    procedure chkShowHorseButtonClick(Sender: TObject);
    procedure sePluginPickupTimeChange(Sender: TObject);
    procedure seShowHintNameFontSizeChange(Sender: TObject);
    procedure chkHideItemNameNumClick(Sender: TObject);
    procedure chkShowDeputyHeroButtonClick(Sender: TObject);
    procedure chkKeyTabGetActorClick(Sender: TObject);
    procedure cbbTitleFileIndexChange(Sender: TObject);
    procedure chkShowMagicShieldHPClick(Sender: TObject);
    procedure chkShowNormalFashionClick(Sender: TObject);
    procedure chkShowHeroShortKeyClick(Sender: TObject);
    procedure seShowHeroShortKeyXChange(Sender: TObject);
    procedure seShowHeroShortKeyYChange(Sender: TObject);
    procedure chkFashionJewelryOpenClick(Sender: TObject);
    procedure sePluginMinEatItemTimeChange(Sender: TObject);
    procedure chkHealthNumberTextClick(Sender: TObject);
    procedure chkHeroHideTabSheet5Click(Sender: TObject);
    procedure chkHeroHideTabSheet2Click(Sender: TObject);
    procedure chkShowItemFormClick(Sender: TObject);
    procedure chkShowItemSellPriceClick(Sender: TObject);
    procedure chkShowFashionHideShieldClick(Sender: TObject);
    procedure chkShowInsuranceInfoClick(Sender: TObject);
    procedure chkShowFashionHideHatsClick(Sender: TObject);
    procedure seIncSpeedDecIntervalChange(Sender: TObject);
    procedure chkGreenHintNewStyleClick(Sender: TObject);
    procedure seUseAttackItemIntervalTimeChange(Sender: TObject);
    procedure chkMagicSetDirClick(Sender: TObject);
    procedure chkBlastHitShowHealthNumClick(Sender: TObject);
    procedure chkPoisoningHideHealthNumClick(Sender: TObject);
    procedure chkHideIconWithHideTitleClick(Sender: TObject);
    procedure chkHPStoneHideHealthNumClick(Sender: TObject);
    procedure chkMPStoneHideHealthNumClick(Sender: TObject);
    procedure chkShowBagArrangeClick(Sender: TObject);
    procedure seIncMoveSpeedDecIntervalChange(Sender: TObject);
    procedure seIncSpellSpeedDecIntervalChange(Sender: TObject);
    procedure seMoveFrameTimeChange(Sender: TObject);
    procedure chkAddItemMsgYBottomToTopClick(Sender: TObject);
    procedure chkGetExpMsgYBottomToTopClick(Sender: TObject);
    procedure chkUpLevelMsgYBottomToTopClick(Sender: TObject);
    procedure chkHeroAddItemMsgYBottomToTopClick(Sender: TObject);
    procedure chkHeroGetExpMsgYBottomToTopClick(Sender: TObject);
    procedure chkHeroUpLevelMsgYBottomToTopClick(Sender: TObject);
    procedure chkShowBagGameGoldSeparatorClick(Sender: TObject);
    procedure edtShowHintFontNameChange(Sender: TObject);
    procedure seShowHintOtherFontSizeChange(Sender: TObject);
    procedure cbbShowHintNameFontBoldChange(Sender: TObject);
    procedure cbbShowHintNameFontStrokeChange(Sender: TObject);
    procedure cbbShowHintOtherFontBoldChange(Sender: TObject);
    procedure cbbShowHintOtherFontStrokeChange(Sender: TObject);
    procedure seShowItemFormColorChange(Sender: TObject);
    procedure seShowInsuranceInfoColorChange(Sender: TObject);
    procedure seShowItemSellPriceColorChange(Sender: TObject);
    procedure seHealthNumberOffsetXChange(Sender: TObject);
    procedure seHealthNumberOffsetYChange(Sender: TObject);
    procedure seHealthNumberMoveSpeedChange(Sender: TObject);
    procedure chkShowBagGameInfoClick(Sender: TObject);
    procedure cbbMinMapTypeChange(Sender: TObject);
    procedure seNextArrBtnOffsetX1Change(Sender: TObject);
    procedure seArrBtnOffsetX1Change(Sender: TObject);
    procedure cbbArrBtnVertAlign1Change(Sender: TObject);
    procedure seArrBtnOffsetY1Change(Sender: TObject);
    procedure seNextArrBtnOffsetY1Change(Sender: TObject);
    procedure cbbArrBtnHorzAlign1Change(Sender: TObject);
    procedure btnArrBtnSettingClick(Sender: TObject);
    procedure chkMinMapUseFindPathClick(Sender: TObject);
    procedure chkLoginShowMinMapClick(Sender: TObject);
    procedure seNewLeftGroupInfoOffsetXChange(Sender: TObject);
    procedure seNewLeftGroupInfoOffsetYChange(Sender: TObject);
    procedure cbbBagRightkeyChange(Sender: TObject);
    procedure chkMoveItemShowIDClick(Sender: TObject);
    procedure chkItemFromField0Click(Sender: TObject);
    procedure seHumHPBarOffsetXChange(Sender: TObject);
    procedure seHumHPBarOffsetYChange(Sender: TObject);
    procedure seNpcHPBarOffsetXChange(Sender: TObject);
    procedure seNpcHPBarOffsetYChange(Sender: TObject);
    procedure seMonHPBarOffsetXChange(Sender: TObject);
    procedure seMonHPBarOffsetYChange(Sender: TObject);
    procedure seHumNameOffsetXChange(Sender: TObject);
    procedure seHumNameOffsetYChange(Sender: TObject);
    procedure seNpcNameOffsetXChange(Sender: TObject);
    procedure seNpcNameOffsetYChange(Sender: TObject);
    procedure seMonNameOffsetXChange(Sender: TObject);
    procedure seMonNameOffsetYChange(Sender: TObject);
    procedure seIncHealingLimiteChange(Sender: TObject);
    procedure seHintWindowBorderWidthLeftChange(Sender: TObject);
    procedure seHintWindowBorderWidthTopChange(Sender: TObject);
    procedure seHintWindowBorderWidthRightChange(Sender: TObject);
    procedure seHintWindowBorderWidthBottomChange(Sender: TObject);
    procedure chkHelmetShowInBoxClick(Sender: TObject);
    procedure RadioGroupBagFastItemCompareClick(Sender: TObject);
    procedure seBetterItemXChange(Sender: TObject);
    procedure seBetterItemYChange(Sender: TObject);
    procedure seSmallInfoXChange(Sender: TObject);
    procedure seSmallInfoYChange(Sender: TObject);
    procedure seJoyStickXChange(Sender: TObject);
    procedure seJoyStickYChange(Sender: TObject);
    procedure seJoyStickMaxXChange(Sender: TObject);
    procedure seJoyStickMaxYChange(Sender: TObject);
    procedure seSkillCtrXChange(Sender: TObject);
    procedure seSkillCtrYChange(Sender: TObject);
    procedure seMapScaleChange(Sender: TObject);
    procedure seGuiScaleChange(Sender: TObject);
    procedure chkShowExSkillIconClick(Sender: TObject);
    procedure chkShowMulitDlgClick(Sender: TObject);
    procedure seMultiViewRangeChange(Sender: TObject);
    procedure chkShowBetterItemClick(Sender: TObject);
  private
    boOpened: Boolean;
    boModValued: Boolean;
    boSendServerConfig: Boolean;
    procedure ModValue();
    procedure uModValue();
    procedure RefClientConf();
    procedure RefSpecialCmd();
    procedure RefClientPlugTableVisible;
    { Private declarations }
  public
    procedure Open();
    { Public declarations }
  end;

var
  FrmConfigClient: TFrmConfigClient;

implementation

uses
  M2Share, Grobal2;
{$R *.dfm}

function GetBrightString(nBright: Integer): string;
begin
  case nBright of
    0:
      Result := '日出';
    1:
      Result := '白天';
    2:
      Result := '傍晚';
    3:
      Result := '黑夜';
  end;
end;

procedure TFrmConfigClient.FormCreate(Sender: TObject);
var
  I: Integer;
begin
  cbbTitleFileIndex.Clear;
  for I := 0 to g_EffectImageList.Count - 1 do
  begin
    cbbTitleFileIndex.Items.Add(g_EffectImageList.Strings[I]);
  end;

  boSendServerConfig := False;
  seUseItemIntervalTime.Hint := '游戏中人物二次使用物品间隔时间，此参数默认为 500毫秒。';
end;

procedure TFrmConfigClient.Open();
begin
  boOpened := False;
  uModValue();
  RefClientConf();
  boOpened := True;
  ClientPageControl.ActivePageIndex := 0;
  ShowModal;
end;

procedure TFrmConfigClient.ModValue();
begin
  boModValued := True;
  ButtonPrguseSave.Enabled := True;
  ButtonGameAuxiliarySave.Enabled := True;
  ButtonClientHintWindowsSave.Enabled := True;
  ButtonClientItemNameSave.Enabled := True;
  ButtonWeatherSave.Enabled := True;
  ButtonGameAuxiliarySave2.Enabled := True;
  btnSaveDrugAndRestore.Enabled := True;
  btnSaveOption2.Enabled := True;
end;

procedure TFrmConfigClient.uModValue();
begin
  boModValued := False;
  ButtonPrguseSave.Enabled := False;
  ButtonGameAuxiliarySave.Enabled := False;
  ButtonClientHintWindowsSave.Enabled := False;
  ButtonClientItemNameSave.Enabled := False;
  ButtonWeatherSave.Enabled := False;
  ButtonGameAuxiliarySave2.Enabled := False;
  btnSaveDrugAndRestore.Enabled := False;
  btnSaveOption2.Enabled := False;
end;

procedure TFrmConfigClient.RefClientConf();
var
  I: Integer;
begin
  RadioButtonPlugIn1.Checked := g_Config.btConfigDlgType = 0;
  RadioButtonPlugIn2.Checked := g_Config.btConfigDlgType = 1;
  RzCheckGroupClientTabSheet.Items.Clear;
  RefClientPlugTableVisible;

  CheckBoxStartGameAuxiliary.Checked := g_Config.boStartGameAuxiliary;
  CheckBoxCanOpenGameConfigDlg.Checked := g_Config.boCanOpenGameConfigDlg;
  CheckBoxNotCanUseClientConfig.Checked := g_Config.boNotCanUseClientConfig;
  chkGreenHintNewStyle.Checked := g_Config.boGreenHintNewStyle;

  seMoveFrameTime.Value := g_Config.dwMoveFrameTime;

  // 两次普通攻击之间的间隔时间 piaoyun 2013-07-20
  seHitFrameTime.Value := g_Config.dwHitFrameTime;
  // 两次魔法攻击之间的间隔时间 piaoyun 2013-07-20
  seMagicHitFrameTime.Value := g_Config.dwMagicHitFrameTime;
  sePluginPickupTime.Value := g_Config.dwPluginPickupTime;
  sePluginMinEatItemTime.Value := g_Config.dwPluginMinEatItemTime;
  seIncSpeedDecInterval.Value := g_Config.dwIncSpeedDecInterval;
  seIncMoveSpeedDecInterval.Value := g_Config.dwIncMoveSpeedDecInterval;
  seIncSpellSpeedDecInterval.Value := g_Config.dwIncSpellSpeedDecInterval;

  CheckBoxFriendButton.Checked := g_Config.boFriendButton;
  CheckBoxControlHelpButton.Checked := g_Config.boControlHelpButton;
  CheckBoxRankButton.Checked := g_Config.boRankButton;
  CheckBoxWhisperButton.Checked := g_Config.boWhisperButton;
  CheckBoxActionLogButton.Checked := g_Config.boActionLogButton;
  CheckBoxMissionButton.Checked := g_Config.boMissionButton;
  CheckBoxWebButton.Checked := g_Config.boWebButton;
  CheckBoxOpenShopButton.Checked := g_Config.boOpenShopButton;
  CheckBoxUserShopButton.Checked := g_Config.boUserShopButton;
  CheckBoxShowMerchantDlgHelp.Checked := g_Config.boShowMerchantDlgHelp;
  CheckBoxChallengeButton.Checked := g_Config.boChallengeButton;
  chkShowGlory.Checked := g_Config.boShowGlory;
  chkShowHorseButton.Checked := g_Config.boShowHorseButton;
  chkShowDeputyHeroButton.Checked := g_Config.boShowDeputyHeroButton;
  chkShowBagArrange.Checked := g_Config.boShowBagArrange;
  chkKeyTabGetActor.Checked := g_Config.boKeyTabGetActor;

  chkShowMagicShieldHP.Checked := g_Config.boShowMagicShieldHP;

  seHumHPBarOffsetX.Value := g_Config.nHumHPBarOffsetX;
  seHumHPBarOffsetY.Value := g_Config.nHumHPBarOffsetY;

  seNpcHPBarOffsetX.Value := g_Config.nNpcHPBarOffsetX;
  seNpcHPBarOffsetY.Value := g_Config.nNpcHPBarOffsetY;

  seMonHPBarOffsetX.Value := g_Config.nMonHPBarOffsetX;
  seMonHPBarOffsetY.Value := g_Config.nMonHPBarOffsetY;

  seHumNameOffsetX.Value := g_Config.nHumNameOffsetX;
  seHumNameOffsetY.Value := g_Config.nHumNameOffsetY;

  seNpcNameOffsetX.Value := g_Config.nNpcNameOffsetX;
  seNpcNameOffsetY.Value := g_Config.nNpcNameOffsetY;

  seMonNameOffsetX.Value := g_Config.nMonNameOffsetX;
  seMonNameOffsetY.Value := g_Config.nMonNameOffsetY;

  chkHealthNumberText.Checked := g_Config.boHealthNumberText;
  chkBlastHitShowHealthNum.Checked := g_Config.boBlastHitShowHealthNum;

  seHealthNumberOffsetX.Value := g_Config.nHealthNumberOffsetX;
  seHealthNumberOffsetY.Value := g_Config.nHealthNumberOffsetY;
  seHealthNumberMoveSpeed.Value := g_Config.nHealthNumberMoveSpeed;

  seNewLeftGroupInfoOffsetX.Value := g_Config.nNewLeftGroupInfoOffsetX;
  seNewLeftGroupInfoOffsetY.Value := g_Config.nNewLeftGroupInfoOffsetY;

  chkPoisoningHideHealthNum.Checked := g_Config.boPoisoningHideHealthNum;
  chkHPStoneHideHealthNum.Checked := g_Config.boHPStoneHideHealthNum;
  chkMPStoneHideHealthNum.Checked := g_Config.boMPStoneHideHealthNum;
  chkMagicSetDir.Checked := g_Config.boMagicSetDir;

  chkBlastHitShowHealthNum.Enabled := chkHealthNumberText.Checked;
  chkHPStoneHideHealthNum.Enabled := not chkHealthNumberText.Checked;
  chkMPStoneHideHealthNum.Enabled := not chkHealthNumberText.Checked;

  seHintWindowBGColor.Value := g_Config.btHintWindowbackgroundColor;
  seHintWindowBGAlpha.Value := g_Config.btHintWindowbackgroundAlpha;

  seHintWindowBorderWidthLeft.Value := g_Config.HintWindowBorderWidth.Left;
  seHintWindowBorderWidthTop.Value := g_Config.HintWindowBorderWidth.Top;
  seHintWindowBorderWidthRight.Value := g_Config.HintWindowBorderWidth.Right;
  seHintWindowBorderWidthBottom.Value := g_Config.HintWindowBorderWidth.Bottom;

  chkShowHintWindowFrame.Checked := g_Config.boShowHintWindowFrame;
  chkShowHintLines.Checked := g_Config.boShowHintLines;

  edtShowHintFontName.Text := g_Config.sShowHintFontName;

  seShowHintNameFontSize.Value := g_Config.btShowHintNameFontSize;

  if (g_Config.btShowHintNameFontBold < cbbShowHintNameFontBold.Items.Count) then
  begin
    cbbShowHintNameFontBold.ItemIndex := g_Config.btShowHintNameFontBold;
  end;

  if (g_Config.btShowHintNameFontStroke < cbbShowHintNameFontStroke.Items.Count) then
  begin
    cbbShowHintNameFontStroke.ItemIndex := g_Config.btShowHintNameFontStroke;
  end;

  seShowHintOtherFontSize.Value := g_Config.btShowHintOtherFontSize;
  if (g_Config.btShowHintOtherFontBold < cbbShowHintOtherFontBold.Items.Count) then
  begin
    cbbShowHintOtherFontBold.ItemIndex := g_Config.btShowHintOtherFontBold;
  end;

  if (g_Config.btShowHintOtherFontStroke < cbbShowHintOtherFontStroke.Items.Count) then
  begin
    cbbShowHintOtherFontStroke.ItemIndex := g_Config.btShowHintOtherFontStroke;
  end;

  TrackBarMoveSpeed.Position := g_Config.nMoveSpeed;
  TrackBarAttackSpeed.Position := g_Config.nAttackSpeed;
  TrackBarSpellSpeed.Position := g_Config.nSpellSpeed;

  RzSpinnerMoveSpeed.Value := g_Config.nMoveSpeed;
  RzSpinnerAttackSpeed.Value := g_Config.nAttackSpeed;
  RzSpinnerSpellSpeed.Value := g_Config.nSpellSpeed;

  TrackBarMoveSpeed.Hint := IntToStr(TrackBarMoveSpeed.Position);
  TrackBarAttackSpeed.Hint := IntToStr(TrackBarAttackSpeed.Position);
  TrackBarSpellSpeed.Hint := IntToStr(TrackBarSpellSpeed.Position);
  for I := 0 to RzCheckGroupClientConfig.Items.Count - 1 do
    RzCheckGroupClientConfig.ItemChecked[I] := g_Config.ClientConfigs[I];

  for I := 0 to RzCheckGroupClientTabSheet.Items.Count - 1 do
    RzCheckGroupClientTabSheet.ItemChecked[I] := g_Config.ClientConfigTabSheetVisibles[I];

  RadioGroupShowItemStyle.ItemIndex := g_Config.btSuspensionShowItem;

  RadioGroupBagFastItemCompare.ItemIndex := g_Config.btBagFastItemCompareMode;

  ListBoxClientItemName.Clear;
  ListBoxClientItemName.Items.AddStrings(g_ClientEatItemNameList);
  ButtonClientItemNameDel.Enabled := False;

  CheckBoxOpenHeroButton.Checked := g_Config.boOpenHeroButton;
  CheckBoxEscCloseNPC.Checked := g_Config.boEscCloseNPC;
  chkHintWithMouse.Checked := g_Config.boHintWithMouse;
  chkNpcDlgHintWithMouse.Checked := g_Config.boNpcDlgHintWithMouse;
  chkHelmetShowInBox.Checked := g_Config.boHelmetShowInBox;

  chkShowItemForm.Checked := g_Config.boShowItemForm;
  chkShowItemSellPrice.Checked := g_Config.boShowItemSellPrice;
  chkShowInsuranceInfo.Checked := g_Config.boShowInsuranceInfo;

  seShowItemFormColor.Value := g_Config.btShowItemFormColor;
  seShowInsuranceInfoColor.Value := g_Config.btShowInsuranceInfoColor;
  seShowItemSellPriceColor.Value := g_Config.btShowItemSellPriceColor;

  chkItemFromField0.Checked := g_Config.boShowItemFromFields[0];
  chkItemFromField1.Checked := g_Config.boShowItemFromFields[1];
  chkItemFromField2.Checked := g_Config.boShowItemFromFields[2];
  chkItemFromField3.Checked := g_Config.boShowItemFromFields[3];
  chkItemFromField4.Checked := g_Config.boShowItemFromFields[4];
  chkItemFromField5.Checked := g_Config.boShowItemFromFields[5];
  chkItemFromField6.Checked := g_Config.boShowItemFromFields[6];

  chkShowNormalFashion.Checked := g_Config.boShowNormalFashion;
  chkShowFashionHideShield.Checked := g_Config.boShowFashionHideShield;
  chkShowFashionHideHats.Checked := g_Config.boShowFashionHideHats;
  chkFashionJewelryOpen.Checked := g_Config.boFashionJewelryOpen;

  EditHomePage.Text := g_Config.sHomePage;

  CheckBoxDBotFunc1.Checked := g_Config.DBotFuncs[0];
  CheckBoxDBotFunc2.Checked := g_Config.DBotFuncs[1];
  CheckBoxDBotFunc3.Checked := g_Config.DBotFuncs[2];
  CheckBoxDBotFunc4.Checked := g_Config.DBotFuncs[3];
  CheckBoxDBotFunc5.Checked := g_Config.DBotFuncs[4];
  CheckBoxDBotFunc6.Checked := g_Config.DBotFuncs[5];

  CheckBoxViewFog.Checked := g_Config.boViewFog;

  ListBoxBright.Clear;
  for I := 0 to Length(g_Config.BrightConfig) - 1 do
  begin
    ListBoxBright.Items.AddObject(Format('%d点  %s', [I, GetBrightString(g_Config.BrightConfig[I])]), TObject(g_Config.BrightConfig
      [I]));
  end;

  ButtonSpecialCmdSave.Enabled := FALSE;
  RefSpecialCmd();

  CheckBoxMonStruckShowNumber.Checked := g_Config.boMonStruckShowNumber;
  CheckBoxHumStruckShowNumber.Checked := g_Config.boHumStruckShowNumber;
  CheckBoxCloseBookProtect.Checked := g_Config.boCloseBookProtect;
  CheckBoxCloseLogoutProtect.Checked := g_Config.boCloseLogoutProtect;

  if g_Config.boBagRightkey then
    cbbBagRightkey.ItemIndex := 1
  else
    cbbBagRightkey.ItemIndex := 0;

  CheckBoxGetExpMsgAddChatBoardMsg.checked := g_Config.boGetExpMsgAddChatBoardMsg;

  chkAddItemMsgXRightToLeft.checked := g_Config.boAddItemMsgXRightToLeft;
  chkAddItemMsgYBottomToTop.checked := g_Config.boAddItemMsgYBottomToTop;

  chkGetExpMsgXRightToLeft.checked := g_Config.boGetExpMsgXRightToLeft;
  chkGetExpMsgYBottomToTop.checked := g_Config.boGetExpMsgYBottomToTop;

  chkUpLevelMsgXRightToLeft.checked := g_Config.boUpLevelMsgXRightToLeft;
  chkUpLevelMsgYBottomToTop.checked := g_Config.boUpLevelMsgYBottomToTop;

  chkHeroAddItemMsgXRightToLeft.checked := g_Config.boHeroAddItemMsgXRightToLeft;
  chkHeroAddItemMsgYBottomToTop.checked := g_Config.boHeroAddItemMsgYBottomToTop;

  chkHeroGetExpMsgXRightToLeft.checked := g_Config.boHeroGetExpMsgXRightToLeft;
  chkHeroGetExpMsgYBottomToTop.checked := g_Config.boHeroGetExpMsgYBottomToTop;

  chkHeroUpLevelMsgXRightToLeft.checked := g_Config.boHeroUpLevelMsgXRightToLeft;
  chkHeroUpLevelMsgYBottomToTop.checked := g_Config.boHeroUpLevelMsgYBottomToTop;

  chkHideTabSheet2.checked := g_Config.boHideTabSheet2;
  chkHeroHideTabSheet2.checked := g_Config.boHeroHideTabSheet2;
  chkHideTabSheet5.checked := g_Config.boHideTabSheet5;
  chkHeroHideTabSheet5.Checked := g_Config.boHeroHideTabSheet5;
  chkHideTabSheet7.checked := g_Config.boHideTabSheet7;

  for I := 0 to CheckGroupNewAbil.Items.Count - 1 do
  begin
    CheckGroupNewAbil.ItemChecked[I] := g_Config.NewAbilShowStateDlg[I];
  end;

  seAddItemMsgFColor.Value := g_Config.btAddItemMsgFColor;
  seAddItemMsgBColor.Value := g_Config.btAddItemMsgBColor;
  seGetExpMsgFColor.Value := g_Config.btGetExpMsgFColor;
  seGetExpMsgBColor.Value := g_Config.btGetExpMsgBColor;
  seUpLevelMsgFColor.Value := g_Config.btUpLevelMsgFColor;
  seUpLevelMsgBColor.Value := g_Config.btUpLevelMsgBColor;

  seAddItemMsgX.Value := g_Config.nAddItemMsgX;
  seAddItemMsgY.Value := g_Config.nAddItemMsgY;
  seGetExpMsgX.Value := g_Config.nGetExpMsgX;
  seGetExpMsgY.Value := g_Config.nGetExpMsgY;
  seUpLevelMsgX.Value := g_Config.nUpLevelMsgX;
  seUpLevelMsgY.Value := g_Config.nUpLevelMsgY;

  seHeroAddItemMsgFColor.Value := g_Config.btHeroAddItemMsgFColor;
  seHeroAddItemMsgBColor.Value := g_Config.btHeroAddItemMsgBColor;
  seHeroGetExpMsgFColor.Value := g_Config.btHeroGetExpMsgFColor;
  seHeroGetExpMsgBColor.Value := g_Config.btHeroGetExpMsgBColor;
  seHeroUpLevelMsgFColor.Value := g_Config.btHeroUpLevelMsgFColor;
  seHeroUpLevelMsgBColor.Value := g_Config.btHeroUpLevelMsgBColor;

  seHeroAddItemMsgX.Value := g_Config.nHeroAddItemMsgX;
  seHeroAddItemMsgY.Value := g_Config.nHeroAddItemMsgY;
  seHeroGetExpMsgX.Value := g_Config.nHeroGetExpMsgX;
  seHeroGetExpMsgY.Value := g_Config.nHeroGetExpMsgY;
  seHeroUpLevelMsgX.Value := g_Config.nHeroUpLevelMsgX;
  seHeroUpLevelMsgY.Value := g_Config.nHeroUpLevelMsgY;

  chkShowBagGameGoldSeparator.Checked := g_Config.boShowBagGameGoldSeparator;
  chkShowBagGameInfo.Checked := g_Config.boShowBagGameInfo;

  chkShowHeroShortKey.Checked := g_Config.boShowHeroShortKey;
  seShowHeroShortKeyX.Value := g_Config.nShowHeroShortKeyX;
  seShowHeroShortKeyY.Value := g_Config.nShowHeroShortKeyY;

  chkUseFindPath.Checked := g_Config.boUseFindPath;
  CheckBoxUseOldSerialWindows.Checked := g_Config.boUseOldSerialWindows;
  rgStateWindows.Enabled := CheckBoxUseOldSerialWindows.Checked;

  chkMoveItemShowID.Checked := g_Config.boMoveItemShowID;

  rgStateWindows.ItemIndex := g_Config.boStateWindowsType;

  cbbTitleFileIndex.ItemIndex := g_Config.nTitleFileIndex;
  //chkHideIconWithHideTitle.Checked := g_Config.boHideIconWithHideTitle;

  sePerHealth.Value := g_Config.nPerHealth;
  sePerSpell.Value := g_Config.nPerSpell;
  seIncHealthSpell.Value := g_Config.nIncHealthSpellTime;

  seHealthFillTime.Value := g_Config.nHealthFillTime;
  seHealthFillTime_Human_Warrior.Value := g_Config.nHealthFillTime_Human_Warrior;
  seHealthFillTime_Human_TaoistAndWizard.Value := g_Config.nHealthFillTime_Human_TaoistAndWizard;
  seHealthFillTime_Hero_Warrior.Value := g_Config.nHealthFillTime_Hero_Warrior;
  seHealthFillTime_Hero_TaoistAndWizard.Value := g_Config.nHealthFillTime_Hero_TaoistAndWizard;

  seSpellFillTime.Value := g_Config.nSpellFillTime;
  seSpellFillTime_Human_Warrior.Value := g_Config.nSpellFillTime_Human_Warrior;
  seSpellFillTime_Human_TaoistAndWizard.Value := g_Config.nSpellFillTime_Human_TaoistAndWizard;
  seSpellFillTime_Hero_Warrior.Value := g_Config.nSpellFillTime_Hero_Warrior;
  seSpellFillTime_Hero_TaoistAndWizard.Value := g_Config.nSpellFillTime_Hero_TaoistAndWizard;

  seHealthBaseNum.Value := g_Config.nHealthBaseNum;
  seSpellBaseNum.Value := g_Config.nSpellBaseNum;

  seHealthBaseNum_Human_Warrior.Value := g_Config.nHealthBaseNum_Human_Warrior;
  seHealthBaseNum_Human_TaoistAndWizard.Value := g_Config.nHealthBaseNum_Human_TaoistAndWizard;
  seHealthBaseNum_Hero_Warrior.Value := g_Config.nHealthBaseNum_Hero_Warrior;
  seHealthBaseNum_Hero_TaoistAndWizard.Value := g_Config.nHealthBaseNum_Hero_TaoistAndWizard;

  seSpellBaseNum_Human_Warrior.Value := g_Config.nSpellBaseNum_Human_Warrior;
  seSpellBaseNum_Human_TaoistAndWizard.Value := g_Config.nSpellBaseNum_Human_TaoistAndWizard;
  seSpellBaseNum_Hero_Warrior.Value := g_Config.nSpellBaseNum_Hero_Warrior;
  seSpellBaseNum_Hero_TaoistAndWizard.Value := g_Config.nSpellBaseNum_Hero_TaoistAndWizard;

  seUseItemIntervalTime.Value := g_Config.dwUseItemIntervalTime;
  seUseAttackItemIntervalTime.Value := g_Config.dwUseAttackItemIntervalTime;

  seUseOrdinaryTime_Human_Warrior.Value := g_Config.dwUseOrdinaryTime_Human_Warrior;
    // 人物战士吃普通药间隔 - 2013-07-16  (+ chongchong)
  seUseSpecialTime_Human_Warrior.Value := g_Config.dwUseSpecialTime_Human_Warrior;
    // 人物战士吃特殊药间隔 - 2013-07-16  (+ chongchong)
  seUseOrdinaryTime_Human_TaoistAndWizard.Value := g_Config.dwUseOrdinaryTime_Human_TaoistAndWizard;
    // 人物道法吃普通药间隔 - 2013-07-16  (+ chongchong)
  seUseSpecialTime_Human_TaoistAndWizard.Value := g_Config.dwUseSpecialTime_Human_TaoistAndWizard;
    // 人物道法吃特殊药间隔 - 2013-07-16  (+ chongchong)

  seUseOrdinaryTime_Hero_Warrior.Value := g_Config.dwUseOrdinaryTime_Hero_Warrior;
    // 英雄战士吃普通药间隔 - 2013-07-16  (+ chongchong)
  seUseSpecialTime_Hero_Warrior.Value := g_Config.dwUseSpecialTime_Hero_Warrior;
    // 英雄战士吃特殊药间隔 - 2013-07-16  (+ chongchong)
  seUseOrdinaryTime_Hero_TaoistAndWizard.Value := g_Config.dwUseOrdinaryTime_Hero_TaoistAndWizard;
    // 英雄道法吃普通药间隔 - 2013-07-16  (+ chongchong)
  seUseSpecialTime_Hero_TaoistAndWizard.Value := g_Config.dwUseSpecialTime_Hero_TaoistAndWizard;
    // 英雄道法吃特殊药间隔 - 2013-07-16  (+ chongchong)
  { 小地图相关控制 chongchong 2013-07-19 }
  chkMinMapCloseRadar.Checked := g_Config.boMinMapCloseRadar;
    // 关闭雷达显示 chongchong 2013-07-19
  cbbMinMapType.ItemIndex := g_Config.btMinMapType;
    // 使用老式雷达  chongchong 2013-07-19
  chkMinMapUseFindPath.Checked := g_Config.boMinMapUseFindPath;
  chkLoginShowMinMap.Checked := g_Config.boLoginShowMinMap;
  seMinMapFlagFlash.Value := g_Config.dwMinMapFlagFlash;
    // 小地图中玩家自身闪烁频率  chongchong 2013-07-19

  seMinMapColorSelf.Value := g_Config.btMinMapColorSelf;
    // 玩家自身颜色  chongchong 2013-07-19
  seMinMapColorOther.Value := g_Config.btMinMapColorOther;
    // 其他玩家自身颜色  chongchong 2013-07-19
  seMinMapColorNPC.Value := g_Config.btMinMapColorNPC;
    // NPC颜色  chongchong 2013-07-19
  seMinMapColorGuard.Value := g_Config.btMinMapColorGuard;
    // 守卫颜色  chongchong 2013-07-19
  seMinMapColorMonster.Value := g_Config.btMinMapColorMonster;
    // 怪物颜色  chongchong 2013-07-19
  seMinMapColorHero.Value := g_Config.btMinMapColorHero;
    // 英雄颜色  chongchong 2013-07-19
  seMinMapColorBoss.Value := g_Config.btMinMapColorBoss;
    // Boss颜色  chongchong 2013-08-03

  chkHideItemNameNum.Checked := g_Config.boHideItemNameNum;
    // 隐藏物品名后面的数字 chongchong 2013-11-23

  chkGemUpgrade.Checked := g_Config.boGemUpgrade;
    // 开启宝石升级 chongchong 2013-07-20
  chkShowGuildName.Checked := g_Config.boShowGuildName;
    // 名字显示行会信息 chongchong 2013-07-20
  // 商铺界面能否移动 piaoyun 2013-07-23
  chkShopGuiCanMove.Checked := g_Config.boShopGuiCanMove;
  // NPC界面能否移动 piaoyun 2013-07-23
  chkNPCGuiCanMove.Checked := g_config.boNPCGuiCanMove;

  seIncHealingLimite.Value := g_Config.nIncHealingLimite;
  if g_Config.nPerHealingTime < 400 then
    g_Config.nPerHealingTime := 400;
  if g_Config.nBigPerHealingTime < 400 then
    g_Config.nBigPerHealingTime := 400;
  sePerHealing.Value := g_Config.nPerHealing;
    // 治愈术恢复基数  chongchong 2013-07-25
  sePerHealingTime.Value := g_Config.nPerHealingTime;
    // 治愈术恢复速度  chongchong 2013-07-25
  seBigPerHealing.Value := g_Config.nBigPerHealing;
    // 群体治愈术恢复基数 chongchong 2013-07-25
  seBigPerHealingTime.Value := g_Config.nBigPerHealingTime;
    // 群体治愈术恢复速度 chongchong 2013-07-25

  seThrowAwayItemColor.Value := g_Config.btThrowAwayItemColor;
    // 物品丢到地上显示颜色 chongchong 2013-07-28

  cbbArrBtnHorzAlign1.ItemIndex := Integer(g_ArrButtonConfig[0].HorzAligment);
  seArrBtnOffsetX1.Value := g_ArrButtonConfig[0].OffsetX;
  cbbArrBtnVertAlign1.ItemIndex := Integer(g_ArrButtonConfig[0].VertAligment);
  seArrBtnOffsetY1.Value := g_ArrButtonConfig[0].OffsetY;
  seNextArrBtnOffsetX1.Value := g_ArrButtonConfig[0].NextOffsetX;
  seNextArrBtnOffsetY1.Value := g_ArrButtonConfig[0].NextOffsetY;

  cbbArrBtnHorzAlign2.ItemIndex := Integer(g_ArrButtonConfig[1].HorzAligment);
  seArrBtnOffsetX2.Value := g_ArrButtonConfig[1].OffsetX;
  cbbArrBtnVertAlign2.ItemIndex := Integer(g_ArrButtonConfig[1].VertAligment);
  seArrBtnOffsetY2.Value := g_ArrButtonConfig[1].OffsetY;
  seNextArrBtnOffsetX2.Value := g_ArrButtonConfig[1].NextOffsetX;
  seNextArrBtnOffsetY2.Value := g_ArrButtonConfig[1].NextOffsetY;

  cbbArrBtnHorzAlign3.ItemIndex := Integer(g_ArrButtonConfig[2].HorzAligment);
  seArrBtnOffsetX3.Value := g_ArrButtonConfig[2].OffsetX;
  cbbArrBtnVertAlign3.ItemIndex := Integer(g_ArrButtonConfig[2].VertAligment);
  seArrBtnOffsetY3.Value := g_ArrButtonConfig[2].OffsetY;
  seNextArrBtnOffsetX3.Value := g_ArrButtonConfig[2].NextOffsetX;
  seNextArrBtnOffsetY3.Value := g_ArrButtonConfig[2].NextOffsetY;

  cbbArrBtnHorzAlign4.ItemIndex := Integer(g_ArrButtonConfig[3].HorzAligment);
  seArrBtnOffsetX4.Value := g_ArrButtonConfig[3].OffsetX;
  cbbArrBtnVertAlign4.ItemIndex := Integer(g_ArrButtonConfig[3].VertAligment);
  seArrBtnOffsetY4.Value := g_ArrButtonConfig[3].OffsetY;
  seNextArrBtnOffsetX4.Value := g_ArrButtonConfig[3].NextOffsetX;
  seNextArrBtnOffsetY4.Value := g_ArrButtonConfig[3].NextOffsetY;

  cbbArrBtnHorzAlign5.ItemIndex := Integer(g_ArrButtonConfig[4].HorzAligment);
  seArrBtnOffsetX5.Value := g_ArrButtonConfig[4].OffsetX;
  cbbArrBtnVertAlign5.ItemIndex := Integer(g_ArrButtonConfig[4].VertAligment);
  seArrBtnOffsetY5.Value := g_ArrButtonConfig[4].OffsetY;
  seNextArrBtnOffsetX5.Value := g_ArrButtonConfig[4].NextOffsetX;
  seNextArrBtnOffsetY5.Value := g_ArrButtonConfig[4].NextOffsetY;

  cbbArrBtnHorzAlign6.ItemIndex := Integer(g_ArrButtonConfig[5].HorzAligment);
  seArrBtnOffsetX6.Value := g_ArrButtonConfig[5].OffsetX;
  cbbArrBtnVertAlign6.ItemIndex := Integer(g_ArrButtonConfig[5].VertAligment);
  seArrBtnOffsetY6.Value := g_ArrButtonConfig[5].OffsetY;
  seNextArrBtnOffsetX6.Value := g_ArrButtonConfig[5].NextOffsetX;
  seNextArrBtnOffsetY6.Value := g_ArrButtonConfig[5].NextOffsetY;

  cbbArrBtnHorzAlign7.ItemIndex := Integer(g_ArrButtonConfig[6].HorzAligment);
  seArrBtnOffsetX7.Value := g_ArrButtonConfig[6].OffsetX;
  cbbArrBtnVertAlign7.ItemIndex := Integer(g_ArrButtonConfig[6].VertAligment);
  seArrBtnOffsetY7.Value := g_ArrButtonConfig[6].OffsetY;
  seNextArrBtnOffsetX7.Value := g_ArrButtonConfig[6].NextOffsetX;
  seNextArrBtnOffsetY7.Value := g_ArrButtonConfig[6].NextOffsetY;

  btnArrBtnSetting.Enabled := False;

  seBetterItemX.Value := g_Config.nBetterItemX;
  seBetterItemY.Value := g_Config.nBetterItemY;
  seSmallInfoX.Value := g_Config.nSmallInfoX;
  seSmallInfoY.Value := g_Config.nSmallInfoY;
  seJoyStickX.Value := g_Config.nJoyStickX;
  seJoyStickY.Value := g_Config.nJoyStickY;
  seJoyStickMaxX.Value := g_Config.nJoyStickMaxX;
  seJoyStickMaxY.Value := g_Config.nJoyStickMaxY;
  seSkillCtrX.Value := g_Config.nSkillCtrX;
  seSkillCtrY.Value := g_Config.nSkillCtrY;
  seMapScale.Value := g_Config.btMapScale;
  seGuiScale.Value := g_Config.btGuiScale;

  chkShowMulitDlg.Checked := g_Config.boShowMulitDlg;
  chkShowBetterItem.Checked := g_Config.boShowBetterItem;
  seMultiViewRange.Value := g_Config.btMultiViewRange;
end;

procedure TFrmConfigClient.RefSpecialCmd();
var
  I: Integer;
  ClientCmd: pTClientCmd;
  ListItem: TListItem;
begin
  ListViewSpecialCmd.Clear;
  ButtonSpecialCmdAdd.Enabled := TRUE;
  ButtonSpecialCmdDel.Enabled := FALSE;
  ButtonSpecialCmdChg.Enabled := FALSE;
  for I := 0 to g_SpecialCmdList.Count - 1 do
  begin
    ClientCmd := g_SpecialCmdList.Items[I];

    ListItem := ListViewSpecialCmd.Items.Add;
    ListItem.Caption := ClientCmd.sCaption;
    ListItem.Data := ClientCmd;
    ListItem.SubItems.Add(ClientCmd.sCmd);
  end;
end;

procedure TFrmConfigClient.ClientPageControlChanging(Sender: TObject; var AllowChange: Boolean);
begin
  if boModValued then
  begin
    if Application.MessageBox('参数设置已经被修改，是否确认不保存修改的设置？', '确认信息', MB_YESNO + MB_ICONQUESTION) = IDYES then
      uModValue
    else
      AllowChange := False;
  end;
end;

procedure TFrmConfigClient.ButtonPrguseSaveClick(Sender: TObject);
var
  I: Integer;
begin
  Config.WriteBool('Setup', 'StartGameAuxiliary', g_Config.boStartGameAuxiliary);
  Config.WriteBool('Setup', 'ActionLogButton', g_Config.boActionLogButton);
  Config.WriteBool('Setup', 'MissionButton', g_Config.boMissionButton);
  Config.WriteBool('Setup', 'FriendButton', g_Config.boFriendButton);
  Config.WriteBool('Setup', 'ControlHelpButton', g_Config.boControlHelpButton);
  Config.WriteBool('Setup', 'RankButton', g_Config.boRankButton);
  Config.WriteBool('Setup', 'WhisperButton', g_Config.boWhisperButton);
  Config.WriteBool('Setup', 'OpenShopButton', g_Config.boOpenShopButton);
  Config.WriteBool('Setup', 'UserShopButton', g_Config.boUserShopButton);
  Config.WriteBool('Setup', 'WebButton', g_Config.boWebButton);
  Config.WriteBool('Setup', 'OpenHeroButton', g_Config.boOpenHeroButton);
  Config.WriteBool('Setup', 'ShowMerchantDlgHelp', g_Config.boShowMerchantDlgHelp);
  Config.WriteBool('Setup', 'ChallengeButton', g_Config.boChallengeButton);
  Config.WriteBool('Setup', 'ShowGlory', g_Config.boShowGlory);
  Config.WriteBool('Setup', 'ShowHorseButton', g_Config.boShowHorseButton);
  Config.WriteBool('Setup', 'ShowDeputyHeroButton', g_Config.boShowDeputyHeroButton);
  Config.WriteBool('Setup', 'ShowBagArrange', g_Config.boShowBagArrange);

  Config.WriteString('Setup', 'HomePage', g_Config.sHomePage);

  for I := 0 to Length(g_Config.DBotFuncs) - 1 do
  begin
    Config.WriteBool('Setup', 'DBotFuncs' + IntToStr(I), g_Config.DBotFuncs[I]);
  end;
  Config.WriteBool('Setup', 'ViewFog', g_Config.boViewFog);

  Config.WriteBool('Setup', 'GemUpgrade', g_Config.boGemUpgrade);
  Config.WriteBool('Setup', 'ShowGuildName', g_Config.boShowGuildName);
  Config.WriteBool('Setup', 'ShopGuiCanMove', g_Config.boShopGuiCanMove);
  Config.WriteBool('Setup', 'NPCGuiCanMove', g_Config.boNPCGuiCanMove);

  UserEngine.SendServerConfig();
  uModValue();
end;

procedure TFrmConfigClient.ButtonGameAuxiliarySaveClick(Sender: TObject);
var
  I: Integer;
begin
  Config.WriteInteger('Setup', 'PlugIn', g_Config.btConfigDlgType);
  Config.WriteBool('Setup', 'StartGameAuxiliary', g_Config.boStartGameAuxiliary);
  Config.WriteBool('Setup', 'CanOpenGameConfigDlg', g_Config.boCanOpenGameConfigDlg);
  Config.WriteBool('Setup', 'NotCanUseClientConfig', g_Config.boNotCanUseClientConfig);
  Config.WriteBool('Setup', 'GreenHintNewStyle', g_Config.boGreenHintNewStyle);

  Config.WriteInteger('Setup', 'MoveSpeed', g_Config.nMoveSpeed);
  Config.WriteInteger('Setup', 'AttackSpeed', g_Config.nAttackSpeed);
  Config.WriteInteger('Setup', 'SpellSpeed', g_Config.nSpellSpeed);
  Config.WriteInteger('Setup', 'PluginPickupTime', g_Config.dwPluginPickupTime);
  Config.WriteInteger('Setup', 'PluginMinEatItemTime', g_Config.dwPluginMinEatItemTime);
  Config.WriteInteger('Setup', 'IncSpeedDecInterval', g_Config.dwIncSpeedDecInterval);
  Config.WriteInteger('Setup', 'IncMoveSpeedDecInterval', g_Config.dwIncMoveSpeedDecInterval);
  Config.WriteInteger('Setup', 'IncSpellSpeedDecInterval', g_Config.dwIncSpellSpeedDecInterval);

  for I := 0 to Length(g_Config.ClientConfigs) - 1 do
  begin
    Config.WriteBool('Setup', 'ClientConfig' + IntToStr(I), g_Config.ClientConfigs[I]);
  end;
  for I := 0 to Length(g_Config.ClientConfigTabSheetVisibles) - 1 do
  begin
    Config.WriteBool('Setup', 'ClientConfigTabSheetVisible' + IntToStr(I), g_Config.ClientConfigTabSheetVisibles[I]);
  end;

  Config.WriteInteger('Setup', 'MoveFrameTime', g_Config.dwMoveFrameTime);
  // 两次普通攻击之间的间隔时间 piaoyun 2013-07-20
  Config.WriteInteger('Setup', 'HitFrameTime', g_Config.dwHitFrameTime);
  // 两次魔法攻击之间的间隔时间 piaoyun 2013-07-20
  Config.WriteInteger('Setup', 'MagicHitFrameTime', g_Config.dwMagicHitFrameTime);

  UserEngine.SendServerConfig();
  uModValue();
end;

procedure TFrmConfigClient.ButtonClientHintWindowsSaveClick(Sender: TObject);
var
  I: Integer;
begin
  g_Config.sShowHintFontName := edtShowHintFontName.Text;

  Config.WriteBool('Setup', 'ShowHintWindowFrame', g_Config.boShowHintWindowFrame);
  // 装备栏提示信息分界线 piaoyun 2013-07-17
  Config.WriteBool('Setup', 'ShowHintLines', g_Config.boShowHintLines);

  Config.WriteInteger('Setup', 'HintWindowbackgroundColor', g_Config.btHintWindowbackgroundColor);
  Config.WriteInteger('Setup', 'HintWindowbackgroundAlpha', g_Config.btHintWindowbackgroundAlpha);

  Config.WriteInteger('Setup', 'HintWindowBorderWidthLeft', g_Config.HintWindowBorderWidth.Left);
  Config.WriteInteger('Setup', 'HintWindowBorderWidthTop', g_Config.HintWindowBorderWidth.Top);
  Config.WriteInteger('Setup', 'HintWindowBorderWidthRight', g_Config.HintWindowBorderWidth.Right);
  Config.WriteInteger('Setup', 'HintWindowBorderWidthBottom', g_Config.HintWindowBorderWidth.Bottom);

  Config.WriteInteger('Setup', 'SuspensionShowItem', g_Config.btSuspensionShowItem);
  Config.WriteInteger('Setup', 'BagFastItemCompareMode', g_Config.btBagFastItemCompareMode);

  Config.WriteBool('Setup', 'EscCloseNPC', g_Config.boEscCloseNPC);
  Config.WriteBool('Setup', 'HintWithMouse', g_Config.boHintWithMouse);
  Config.WriteBool('Setup', 'NpcDlgHintWithMouse', g_Config.boNpcDlgHintWithMouse);
  Config.WriteBool('Setup', 'HelmetShowInBox', g_Config.boHelmetShowInBox);

  Config.WriteBool('Setup', 'ShowNormalFashion', g_Config.boShowNormalFashion);
  Config.WriteBool('Setup', 'ShowFashionHideShield', g_Config.boShowFashionHideShield);
  Config.WriteBool('Setup', 'ShowFashionHideHats', g_Config.boShowFashionHideHats);
  Config.WriteBool('Setup', 'FashionJewelryOpen', g_Config.boFashionJewelryOpen);

  Config.WriteBool('Setup', 'UseOldSerialWindows', g_Config.boUseOldSerialWindows);
  Config.WriteInteger('Setup', 'boStateWindowsType', g_Config.boStateWindowsType);
  Config.WriteBool('Setup', 'MoveItemShowID', g_Config.boMoveItemShowID);
  Config.WriteBool('Setup', 'HideTabSheet2', g_Config.boHideTabSheet2);
  Config.WriteBool('Setup', 'HeroHideTabSheet2', g_Config.boHeroHideTabSheet2);
  Config.WriteBool('Setup', 'HideTabSheet5', g_Config.boHideTabSheet5);
  Config.WriteBool('Setup', 'HeroHideTabSheet5', g_Config.boHeroHideTabSheet5);
  Config.WriteBool('Setup', 'HideTabSheet7', g_Config.boHideTabSheet7);
  Config.WriteInteger('Setup', 'ThrowAwayItemColor', g_Config.btThrowAwayItemColor);

  Config.WriteString('Setup', 'ShowHintFontName', g_Config.sShowHintFontName);

  Config.WriteInteger('Setup', 'ShowHintNameFontSize', g_Config.btShowHintNameFontSize);
  Config.WriteInteger('Setup', 'ShowHintNameFontBold', g_Config.btShowHintNameFontBold);
  Config.WriteInteger('Setup', 'ShowHintNameFontStroke', g_Config.btShowHintNameFontStroke);

  Config.WriteInteger('Setup', 'ShowHintOtherFontSize', g_Config.btShowHintOtherFontSize);
  Config.WriteInteger('Setup', 'ShowHintOtherFontBold', g_Config.btShowHintOtherFontBold);
  Config.WriteInteger('Setup', 'ShowHintOtherFontStroke', g_Config.btShowHintOtherFontStroke);

  Config.WriteBool('Setup', 'ShowItemForm', g_Config.boShowItemForm);
  Config.WriteBool('Setup', 'ShowItemSellPrice', g_Config.boShowItemSellPrice);
  Config.WriteBool('Setup', 'ShowInsuranceInfo', g_Config.boShowInsuranceInfo);

  Config.WriteInteger('Setup', 'ShowItemFormColor', g_Config.btShowItemFormColor);
  Config.WriteInteger('Setup', 'ShowInsuranceInfoColor', g_Config.btShowInsuranceInfoColor);
  Config.WriteInteger('Setup', 'ShowItemSellPriceColor', g_Config.btShowItemSellPriceColor);

  for I := Low(g_Config.boShowItemFromFields) to High(g_Config.boShowItemFromFields) do
  begin
    Config.WriteBool('Setup', 'ShowItemFromFields' + IntToStr(I), g_Config.boShowItemFromFields[I]);
  end;

  UserEngine.SendServerConfig();
  uModValue();
end;

procedure TFrmConfigClient.CheckBoxControlHelpButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boControlHelpButton := CheckBoxControlHelpButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxRankButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRankButton := CheckBoxRankButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxWhisperButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boWhisperButton := CheckBoxWhisperButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxActionLogButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boActionLogButton := CheckBoxActionLogButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxMissionButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMissionButton := CheckBoxMissionButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxWebButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boWebButton := CheckBoxWebButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxOpenShopButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOpenShopButton := CheckBoxOpenShopButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxUserShopButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boUserShopButton := CheckBoxUserShopButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxStartGameAuxiliaryClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boStartGameAuxiliary := CheckBoxStartGameAuxiliary.Checked;
  ModValue();
end;

procedure TFrmConfigClient.seHintWindowBGColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seHintWindowBGColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHintWindowbackgroundColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seHintWindowBGAlphaChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btHintWindowbackgroundAlpha := seHintWindowBGAlpha.Value;
  ModValue();
end;

procedure TFrmConfigClient.chkShowHintWindowFrameClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowHintWindowFrame := chkShowHintWindowFrame.Checked;
  ModValue();
end;

procedure TFrmConfigClient.RzCheckGroupClientConfigChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
begin
  if not boOpened then
    Exit;
  g_Config.ClientConfigs[Index] := NewState = cbChecked;
  ModValue();
end;

procedure TFrmConfigClient.TrackBarMoveSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMoveSpeed := TrackBarMoveSpeed.Position;
  RzSpinnerMoveSpeed.Value := TrackBarMoveSpeed.Position;
  TrackBarMoveSpeed.Hint := IntToStr(TrackBarMoveSpeed.Position);
  ModValue();
end;

procedure TFrmConfigClient.TrackBarAttackSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAttackSpeed := TrackBarAttackSpeed.Position;
  RzSpinnerAttackSpeed.Value := TrackBarAttackSpeed.Position;
  TrackBarAttackSpeed.Hint := IntToStr(TrackBarAttackSpeed.Position);
  ModValue();
end;

procedure TFrmConfigClient.TrackBarSpellSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSpellSpeed := TrackBarSpellSpeed.Position;
  RzSpinnerSpellSpeed.Value := TrackBarSpellSpeed.Position;
  TrackBarSpellSpeed.Hint := IntToStr(TrackBarSpellSpeed.Position);
  ModValue();
end;

procedure TFrmConfigClient.RzCheckGroupClientTabSheetChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
begin
  if not boOpened then
    Exit;
  g_Config.ClientConfigTabSheetVisibles[Index] := NewState = cbChecked;
  ModValue();
end;

procedure TFrmConfigClient.RadioGroupShowItemStyleClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btSuspensionShowItem := RadioGroupShowItemStyle.ItemIndex;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxFriendButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boFriendButton := CheckBoxFriendButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.ButtonClientItemNameeUPClick(Sender: TObject);
var
  sItemName: string;
  ItemIndex: Integer;
begin
  ItemIndex := ListBoxClientItemName.ItemIndex;
  if ItemIndex > 0 then
  begin
    sItemName := ListBoxClientItemName.Items[ItemIndex];
    ListBoxClientItemName.DeleteSelected;
    ListBoxClientItemName.Items.Insert(ItemIndex - 1, sItemName);
    ListBoxClientItemName.ItemIndex := ItemIndex - 1;
    ButtonClientItemNameDown.Enabled := (ListBoxClientItemName.ItemIndex >= 0) and (ListBoxClientItemName.ItemIndex <
      ListBoxClientItemName.Count - 1);
    ButtonClientItemNameeUP.Enabled := (ListBoxClientItemName.ItemIndex > 0);
    ButtonClientItemNameSave.Enabled := True;
  end;
end;

procedure TFrmConfigClient.ButtonClientItemNameDownClick(Sender: TObject);
var
  sItemName: string;
  ItemIndex: Integer;
begin
  ItemIndex := ListBoxClientItemName.ItemIndex;
  if (ItemIndex >= 0) and (ItemIndex < ListBoxClientItemName.Count - 1) then
  begin
    sItemName := ListBoxClientItemName.Items[ItemIndex];
    ListBoxClientItemName.DeleteSelected;
    ListBoxClientItemName.Items.Insert(ItemIndex + 1, sItemName);
    ListBoxClientItemName.ItemIndex := ItemIndex + 1;
    ButtonClientItemNameDown.Enabled := (ListBoxClientItemName.ItemIndex >= 0) and (ListBoxClientItemName.ItemIndex <
      ListBoxClientItemName.Count - 1);
    ButtonClientItemNameeUP.Enabled := (ListBoxClientItemName.ItemIndex > 0);
    ButtonClientItemNameSave.Enabled := True;
  end;
end;

procedure TFrmConfigClient.ButtonClientItemNameAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  sItemName := Trim(EditClientItemName.Text);
  if sItemName = '' then
  begin
    Application.MessageBox('请输入药品名称！', '错误信息', MB_OK + MB_ICONERROR);
    EditClientItemName.SetFocus;
    Exit;
  end;
  for I := 0 to ListBoxClientItemName.Items.Count - 1 do
  begin
    if CompareText(ListBoxClientItemName.Items.Strings[I], sItemName) = 0 then
    begin
      Application.MessageBox('此药品已经在列表中了！', '错误信息', MB_OK + MB_ICONERROR);
      EditClientItemName.SetFocus;
      Exit;
    end;
  end;
  {if GetWilName(sWilName) then begin
    Application.MessageBox('此WIL文件名称已经在列表中了！', '错误信息', MB_OK + MB_ICONERROR);
    EditWilName.SetFocus;
    Exit;
  end;}
  ListBoxClientItemName.Items.Add(sItemName);
  ButtonClientItemNameSave.Enabled := True;
end;

procedure TFrmConfigClient.ListBoxClientItemNameClick(Sender: TObject);
begin
  if ListBoxClientItemName.ItemIndex >= 0 then
  begin
    ButtonClientItemNameDown.Enabled := (ListBoxClientItemName.ItemIndex >= 0) and (ListBoxClientItemName.ItemIndex <
      ListBoxClientItemName.Count - 1);
    ButtonClientItemNameeUP.Enabled := (ListBoxClientItemName.ItemIndex > 0);
    EditClientItemName.Text := ListBoxClientItemName.Items[ListBoxClientItemName.ItemIndex];
    ButtonClientItemNameDel.Enabled := True;
  end
  else
    ButtonClientItemNameDel.Enabled := False;
end;

procedure TFrmConfigClient.ButtonClientItemNameSaveClick(Sender: TObject);
begin
  g_ClientEatItemNameList.Clear;
  g_ClientEatItemNameList.AddStrings(ListBoxClientItemName.Items);
  SaveClientItemList;
  UserEngine.SendServerConfig();
end;

procedure TFrmConfigClient.ButtonClientItemNameDelClick(Sender: TObject);
begin
  if ListBoxClientItemName.ItemIndex >= 0 then
  begin
    ListBoxClientItemName.DeleteSelected;
    ButtonClientItemNameDel.Enabled := False;
    ButtonClientItemNameSave.Enabled := True;
    ButtonClientItemNameDown.Enabled := (ListBoxClientItemName.ItemIndex >= 0) and (ListBoxClientItemName.ItemIndex <
      ListBoxClientItemName.Count - 1);
    ButtonClientItemNameeUP.Enabled := (ListBoxClientItemName.ItemIndex > 0);
  end;
end;

procedure TFrmConfigClient.EditHomePageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sHomePage := Trim(EditHomePage.Text);
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxOpenHeroButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOpenHeroButton := CheckBoxOpenHeroButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.RzSpinnerMoveSpeedChange(Sender: TObject);
begin
  TrackBarMoveSpeed.Position := RzSpinnerMoveSpeed.Value;
end;

procedure TFrmConfigClient.RzSpinnerAttackSpeedChange(Sender: TObject);
begin
  TrackBarAttackSpeed.Position := RzSpinnerAttackSpeed.Value;
end;

procedure TFrmConfigClient.RzSpinnerSpellSpeedChange(Sender: TObject);
begin
  TrackBarSpellSpeed.Position := RzSpinnerSpellSpeed.Value;
end;

procedure TFrmConfigClient.CheckBoxCanOpenGameConfigDlgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCanOpenGameConfigDlg := CheckBoxCanOpenGameConfigDlg.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxNotCanUseClientConfigClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNotCanUseClientConfig := CheckBoxNotCanUseClientConfig.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxShowMerchantDlgHelpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowMerchantDlgHelp := CheckBoxShowMerchantDlgHelp.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxDBotFunc1Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.DBotFuncs[TCheckBox(Sender).Tag] := TCheckBox(Sender).Checked;
  ModValue();
end;

procedure TFrmConfigClient.ListViewSpecialCmdClick(Sender: TObject);
var
  ClientCmd: pTClientCmd;
  ListItem: TListItem;
begin
  ButtonSpecialCmdDel.Enabled := False;
  ButtonSpecialCmdChg.Enabled := False;
  ListItem := ListViewSpecialCmd.Selected;
  if ListItem = nil then
    Exit;
  ClientCmd := ListItem.Data;
  if ClientCmd = nil then
    Exit;
  EditSpecialCmdCaption.Text := ClientCmd.sCaption;
  EditSpecialCmd.Text := ClientCmd.sCmd;
  ButtonSpecialCmdDel.Enabled := TRUE;
  ButtonSpecialCmdChg.Enabled := TRUE;
end;

procedure TFrmConfigClient.ButtonSpecialCmdDelClick(Sender: TObject);
var
  I: Integer;
  ClientCmd: pTClientCmd;
  ListItem: TListItem;
begin
  ListItem := ListViewSpecialCmd.Selected;
  if ListItem = nil then
    Exit;
  ClientCmd := ListItem.Data;
  if ClientCmd = nil then
    Exit;

  for I := 0 to g_SpecialCmdList.Count - 1 do
  begin
    if ClientCmd = g_SpecialCmdList.Items[I] then
    begin
      g_SpecialCmdList.Delete(I);
      Dispose(ClientCmd);
      Break;
    end;
  end;
  ButtonSpecialCmdDel.Enabled := FALSE;
  ButtonSpecialCmdSave.Enabled := True;
  RefSpecialCmd;
end;

procedure TFrmConfigClient.ButtonSpecialCmdChgClick(Sender: TObject);
var
  I: Integer;
  sCaption, sCmd: string;
  ClientCmd: pTClientCmd;
  ListItem: TListItem;
begin
  ListItem := ListViewSpecialCmd.Selected;
  if ListItem = nil then
    Exit;
  ClientCmd := ListItem.Data;
  if ClientCmd = nil then
    Exit;

  sCaption := EditSpecialCmdCaption.Text;
  sCmd := EditSpecialCmd.Text;
  if sCaption = '' then
  begin
    Application.MessageBox('请输入显示名称！', '错误信息', MB_OK + MB_ICONERROR);
    EditSpecialCmdCaption.SetFocus;
    Exit;
  end;

  if (sCmd <> '') and (sCmd <> ClientCmd.sCmd) then
  begin
    for I := 0 to g_SpecialCmdList.Count - 1 do
    begin
      if CompareText(pTClientCmd(g_SpecialCmdList.Items[I]).sCmd, sCmd) = 0 then
      begin
        Application.MessageBox('该命令已经存在！', '错误信息', MB_OK + MB_ICONERROR);
        EditSpecialCmd.SetFocus;
        Exit;
      end;
    end;
  end;

  ClientCmd.sCaption := sCaption;
  ClientCmd.sCmd := sCmd;
  RefSpecialCmd;
  ButtonSpecialCmdSave.Enabled := True;
  ButtonSpecialCmdChg.Enabled := FALSE;
end;

procedure TFrmConfigClient.ButtonSpecialCmdAddClick(Sender: TObject);
var
  I: Integer;
  ClientCmd: pTClientCmd;
  sCaption, sCmd: string;
begin
  sCaption := EditSpecialCmdCaption.Text;
  sCmd := EditSpecialCmd.Text;
  if sCaption = '' then
  begin
    Application.MessageBox('请输入显示名称！', '错误信息', MB_OK + MB_ICONERROR);
    EditSpecialCmdCaption.SetFocus;
    Exit;
  end;
  if sCmd <> '' then
  begin
    for I := 0 to g_SpecialCmdList.Count - 1 do
    begin
      ClientCmd := g_SpecialCmdList.Items[I];
      if CompareText(ClientCmd.sCmd, sCmd) = 0 then
      begin
        Application.MessageBox('该命令已经存在！', '错误信息', MB_OK + MB_ICONERROR);
        EditSpecialCmd.SetFocus;
        Exit;
      end;
    end;
  end;

  New(ClientCmd);
  ClientCmd.sCaption := sCaption;
  ClientCmd.sCmd := sCmd;
  g_SpecialCmdList.Add(ClientCmd);
  ButtonSpecialCmdSave.Enabled := True;
  RefSpecialCmd;
end;

procedure TFrmConfigClient.ButtonSpecialCmdSaveClick(Sender: TObject);
begin
  ButtonSpecialCmdSave.Enabled := FALSE;
  SaveSpecialCmdList();
  UserEngine.SendSpecialCmdList();
end;

procedure TFrmConfigClient.CheckBoxViewFogClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boViewFog := CheckBoxViewFog.Checked;
  ModValue();
end;

procedure TFrmConfigClient.ButtonWeatherSaveClick(Sender: TObject);
var
  I: Integer;
begin
  Config.WriteBool('Setup', 'ViewFog', g_Config.boViewFog);
  for I := 0 to Length(g_Config.BrightConfig) - 1 do
  begin
    Config.WriteInteger('Setup', 'BrightConfig' + IntToStr(I), g_Config.BrightConfig[I]);
  end;
  UserEngine.SendServerConfig();
  uModValue();
end;

procedure TFrmConfigClient.ListBoxBrightClick(Sender: TObject);
var
  nItemIndex: Integer;
begin
  nItemIndex := ListBoxBright.ItemIndex;
  if (nItemIndex >= 0) and (nItemIndex < Length(g_Config.BrightConfig)) then
  begin
    RadioGroupBright.ItemIndex := g_Config.BrightConfig[nItemIndex];
  end;
end;

procedure TFrmConfigClient.RadioGroupBagFastItemCompareClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btBagFastItemCompareMode := RadioGroupBagFastItemCompare.ItemIndex;
  ModValue();
end;

procedure TFrmConfigClient.RadioGroupBrightClick(Sender: TObject);
var
  nItemIndex: Integer;
begin
  if not boOpened then
    Exit;
  nItemIndex := ListBoxBright.ItemIndex;
  if (nItemIndex >= 0) and (nItemIndex < Length(g_Config.BrightConfig)) then
  begin
    if g_Config.BrightConfig[nItemIndex] <> RadioGroupBright.ItemIndex then
    begin
      g_Config.BrightConfig[nItemIndex] := RadioGroupBright.ItemIndex;
      ModValue();
    end;
  end;
end;

procedure TFrmConfigClient.RadioButtonPlugIn1Click(Sender: TObject);
var
  I: Integer;
begin
  if not boOpened then
    Exit;
  if RadioButtonPlugIn1.Checked then
  begin
    g_Config.btConfigDlgType := 0;
    RefClientPlugTableVisible;
    for I := 0 to RzCheckGroupClientTabSheet.Items.Count - 1 do
    begin
      RzCheckGroupClientTabSheet.ItemChecked[I] := g_Config.ClientConfigTabSheetVisibles[I];
    end;
  end;
  ModValue();
end;

procedure TFrmConfigClient.RadioButtonPlugIn2Click(Sender: TObject);
var
  I: Integer;
begin
  if not boOpened then
    Exit;
  if RadioButtonPlugIn2.Checked then
  begin
    g_Config.btConfigDlgType := 1;
    RefClientPlugTableVisible;
    for I := 0 to RzCheckGroupClientTabSheet.Items.Count - 1 do
    begin
      RzCheckGroupClientTabSheet.ItemChecked[I] := g_Config.ClientConfigTabSheetVisibles[I];
    end;
  end;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxChallengeButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boChallengeButton := CheckBoxChallengeButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.ButtonGameAuxiliarySave2Click(Sender: TObject);
var
  I: Integer;
begin
  Config.WriteBool('Setup', 'MonStruckShowNumber', g_Config.boMonStruckShowNumber);
  Config.WriteBool('Setup', 'HumStruckShowNumber', g_Config.boHumStruckShowNumber);
  Config.WriteBool('Setup', 'CloseBookProtect', g_Config.boCloseBookProtect);
  Config.WriteBool('Setup', 'CloseLogoutProtect', g_Config.boCloseLogoutProtect);
  Config.WriteBool('Setup', 'BagRightkey', g_Config.boBagRightkey);
  Config.WriteBool('Setup', 'GetExpMsgAddChatBoardMsg', g_Config.boGetExpMsgAddChatBoardMsg);

  for I := 0 to Length(g_Config.NewAbilShowStateDlg) - 1 do
  begin
    Config.WriteBool('Setup', 'NewAbilShowStateDlg' + IntToStr(I), g_Config.NewAbilShowStateDlg[I]);
  end;

  Config.WriteInteger('Setup', 'AddItemMsgFColor', g_Config.btAddItemMsgFColor);
  Config.WriteInteger('Setup', 'AddItemMsgBColor', g_Config.btAddItemMsgBColor);
  Config.WriteInteger('Setup', 'GetExpMsgFColor', g_Config.btGetExpMsgFColor);
  Config.WriteInteger('Setup', 'GetExpMsgBColor', g_Config.btGetExpMsgBColor);
  Config.WriteInteger('Setup', 'UpLevelMsgFColor', g_Config.btUpLevelMsgFColor);
  Config.WriteInteger('Setup', 'UpLevelMsgBColor', g_Config.btUpLevelMsgBColor);
  Config.WriteInteger('Setup', 'AddItemMsgX', g_Config.nAddItemMsgX);
  Config.WriteInteger('Setup', 'AddItemMsgY', g_Config.nAddItemMsgY);
  Config.WriteInteger('Setup', 'GetExpMsgX', g_Config.nGetExpMsgX);
  Config.WriteInteger('Setup', 'GetExpMsgY', g_Config.nGetExpMsgY);
  Config.WriteInteger('Setup', 'UpLevelMsgX', g_Config.nUpLevelMsgX);
  Config.WriteInteger('Setup', 'UpLevelMsgY', g_Config.nUpLevelMsgY);
  Config.WriteInteger('Setup', 'HeroAddItemMsgFColor', g_Config.btHeroAddItemMsgFColor);
  Config.WriteInteger('Setup', 'HeroAddItemMsgBColor', g_Config.btHeroAddItemMsgBColor);
  Config.WriteInteger('Setup', 'HeroGetExpMsgFColor', g_Config.btHeroGetExpMsgFColor);
  Config.WriteInteger('Setup', 'HeroGetExpMsgBColor', g_Config.btHeroGetExpMsgBColor);
  Config.WriteInteger('Setup', 'HeroUpLevelMsgFColor', g_Config.btHeroUpLevelMsgFColor);
  Config.WriteInteger('Setup', 'HeroUpLevelMsgBColor', g_Config.btHeroUpLevelMsgBColor);
  Config.WriteInteger('Setup', 'HeroAddItemMsgX', g_Config.nHeroAddItemMsgX);
  Config.WriteInteger('Setup', 'HeroAddItemMsgY', g_Config.nHeroAddItemMsgY);
  Config.WriteInteger('Setup', 'HeroGetExpMsgX', g_Config.nHeroGetExpMsgX);
  Config.WriteInteger('Setup', 'HeroGetExpMsgY', g_Config.nHeroGetExpMsgY);
  Config.WriteInteger('Setup', 'HeroUpLevelMsgX', g_Config.nHeroUpLevelMsgX);
  Config.WriteInteger('Setup', 'HeroUpLevelMsgY', g_Config.nHeroUpLevelMsgY);

  Config.WriteBool('Setup', 'AddItemMsgXRightToLeft', g_Config.boAddItemMsgXRightToLeft);
  Config.WriteBool('Setup', 'AddItemMsgYBottomToTop', g_Config.boAddItemMsgYBottomToTop);

  Config.WriteBool('Setup', 'GetExpMsgXRightToLeft', g_Config.boGetExpMsgXRightToLeft);
  Config.WriteBool('Setup', 'GetExpMsgYBottomToTop', g_Config.boGetExpMsgYBottomToTop);

  Config.WriteBool('Setup', 'UpLevelMsgXRightToLeft', g_Config.boUpLevelMsgXRightToLeft);
  Config.WriteBool('Setup', 'UpLevelMsgYBottomToTop', g_Config.boUpLevelMsgYBottomToTop);

  Config.WriteBool('Setup', 'HeroAddItemMsgXRightToLeft', g_Config.boHeroAddItemMsgXRightToLeft);
  Config.WriteBool('Setup', 'HeroAddItemMsgYBottomToTop', g_Config.boHeroAddItemMsgYBottomToTop);

  Config.WriteBool('Setup', 'HeroGetExpMsgXRightToLeft', g_Config.boHeroGetExpMsgXRightToLeft);
  Config.WriteBool('Setup', 'HeroGetExpMsgYBottomToTop', g_Config.boHeroGetExpMsgYBottomToTop);

  Config.WriteBool('Setup', 'HeroUpLevelMsgXRightToLeft', g_Config.boHeroUpLevelMsgXRightToLeft);
  Config.WriteBool('Setup', 'HeroUpLevelMsgYBottomToTop', g_Config.boHeroUpLevelMsgYBottomToTop);

  Config.WriteBool('Setup', 'ShowBagGameGoldSeparator', g_Config.boShowBagGameGoldSeparator);

  Config.WriteBool('Setup', 'ShowBagGameInfo', g_Config.boShowBagGameInfo);

  Config.WriteBool('Setup', 'ShowHeroShortKey', g_Config.boShowHeroShortKey);
  Config.WriteInteger('Setup', 'ShowHeroShortKeyX', g_Config.nShowHeroShortKeyX);
  Config.WriteInteger('Setup', 'ShowHeroShortKeyY', g_Config.nShowHeroShortKeyY);

  Config.WriteInteger('Setup', 'TitleFileIndex', g_Config.nTitleFileIndex);
  //Config.WriteBool('Setup', 'HideIconWithHideTitle', g_Config.boHideIconWithHideTitle);

  UserEngine.SendServerConfig();
  uModValue();
end;

procedure TFrmConfigClient.CheckBoxMonStruckShowNumberClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMonStruckShowNumber := CheckBoxMonStruckShowNumber.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxHumStruckShowNumberClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHumStruckShowNumber := CheckBoxHumStruckShowNumber.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxCloseBookProtectClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCloseBookProtect := CheckBoxCloseBookProtect.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxCloseLogoutProtectClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCloseLogoutProtect := CheckBoxCloseLogoutProtect.Checked;
  ModValue();
end;

procedure TFrmConfigClient.seAddItemMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seAddItemMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btAddItemMsgFColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seAddItemMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seAddItemMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btAddItemMsgBColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seGetExpMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seGetExpMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGetExpMsgFColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seGetExpMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seGetExpMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGetExpMsgBColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seUpLevelMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seUpLevelMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btUpLevelMsgFColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seUpLevelMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seUpLevelMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btUpLevelMsgBColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seHeroAddItemMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seHeroAddItemMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHeroAddItemMsgFColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seHeroAddItemMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seHeroAddItemMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHeroAddItemMsgBColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seHeroGetExpMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seHeroGetExpMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHeroGetExpMsgFColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seHeroGetExpMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seHeroGetExpMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHeroGetExpMsgBColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seHeroUpLevelMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seHeroUpLevelMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHeroUpLevelMsgFColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seHeroUpLevelMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seHeroUpLevelMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHeroUpLevelMsgBColor := btColor;
  ModValue();
end;

procedure TFrmConfigClient.seAddItemMsgXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAddItemMsgX := seAddItemMsgX.Value;
  ModValue();
end;

procedure TFrmConfigClient.seAddItemMsgYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAddItemMsgY := seAddItemMsgY.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHeroAddItemMsgXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroAddItemMsgX := seHeroAddItemMsgX.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHeroAddItemMsgYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroAddItemMsgY := seHeroAddItemMsgY.Value;
  ModValue();
end;

procedure TFrmConfigClient.seGetExpMsgXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGetExpMsgX := seGetExpMsgX.Value;
  ModValue();
end;

procedure TFrmConfigClient.seGetExpMsgYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGetExpMsgY := seGetExpMsgY.Value;
  ModValue();
end;

procedure TFrmConfigClient.seGuiScaleChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btGuiScale := seGuiScale.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seHeroGetExpMsgXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroGetExpMsgX := seHeroGetExpMsgX.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHeroGetExpMsgYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroGetExpMsgY := seHeroGetExpMsgY.Value;
  ModValue();
end;

procedure TFrmConfigClient.seUpLevelMsgXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUpLevelMsgX := seUpLevelMsgX.Value;
  ModValue();
end;

procedure TFrmConfigClient.seUpLevelMsgYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUpLevelMsgY := seUpLevelMsgY.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHeroUpLevelMsgXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroUpLevelMsgX := seHeroUpLevelMsgX.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHeroUpLevelMsgYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroUpLevelMsgY := seHeroUpLevelMsgY.Value;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxGetExpMsgAddChatBoardMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGetExpMsgAddChatBoardMsg := CheckBoxGetExpMsgAddChatBoardMsg.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHideTabSheet2Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHideTabSheet2 := chkHideTabSheet2.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHideTabSheet5Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHideTabSheet5 := chkHideTabSheet5.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHideTabSheet7Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHideTabSheet7 := chkHideTabSheet7.Checked;
  ModValue();
end;

procedure TFrmConfigClient.CheckGroupNewAbilChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
begin
  if not boOpened then
    Exit;
  g_Config.NewAbilShowStateDlg[Index] := CheckGroupNewAbil.ItemChecked[Index];

  ModValue();
end;

procedure TFrmConfigClient.chkAddItemMsgXRightToLeftClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boAddItemMsgXRightToLeft := chkAddItemMsgXRightToLeft.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkAddItemMsgYBottomToTopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boAddItemMsgYBottomToTop := chkAddItemMsgYBottomToTop.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkGetExpMsgXRightToLeftClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGetExpMsgXRightToLeft := chkGetExpMsgXRightToLeft.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkGetExpMsgYBottomToTopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGetExpMsgYBottomToTop := chkGetExpMsgYBottomToTop.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkUpLevelMsgXRightToLeftClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boUpLevelMsgXRightToLeft := chkUpLevelMsgXRightToLeft.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkUpLevelMsgYBottomToTopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boUpLevelMsgYBottomToTop := chkUpLevelMsgYBottomToTop.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHeroAddItemMsgXRightToLeftClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroAddItemMsgXRightToLeft := chkHeroAddItemMsgXRightToLeft.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHeroAddItemMsgYBottomToTopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroAddItemMsgYBottomToTop := chkHeroAddItemMsgYBottomToTop.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHeroGetExpMsgXRightToLeftClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroGetExpMsgXRightToLeft := chkHeroGetExpMsgXRightToLeft.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHeroGetExpMsgYBottomToTopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroGetExpMsgYBottomToTop := chkHeroGetExpMsgYBottomToTop.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHeroUpLevelMsgXRightToLeftClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroUpLevelMsgXRightToLeft := chkHeroUpLevelMsgXRightToLeft.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHeroUpLevelMsgYBottomToTopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroUpLevelMsgYBottomToTop := chkHeroUpLevelMsgYBottomToTop.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkUseFindPathClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boUseFindPath := chkUseFindPath.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxUseOldSerialWindowsClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boUseOldSerialWindows := CheckBoxUseOldSerialWindows.Checked;
  rgStateWindows.Enabled := CheckBoxUseOldSerialWindows.Checked;
  ModValue();
end;

procedure TFrmConfigClient.rgStateWindowsClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boStateWindowsType := rgStateWindows.ItemIndex;
  ModValue();
end;

procedure TFrmConfigClient.CheckBoxEscCloseNPCClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEscCloseNPC := CheckBoxEscCloseNPC.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHintWithMouseClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHintWithMouse := chkHintWithMouse.Checked;
  ModValue();
end;

procedure TFrmConfigClient.sePerHealthChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPerHealth := sePerHealth.Value;
  ModValue();
end;

procedure TFrmConfigClient.sePerSpellChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPerSpell := sePerSpell.Value;
  ModValue();
end;

procedure TFrmConfigClient.seIncHealthSpellChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nIncHealthSpellTime := seIncHealthSpell.Value;
  ModValue();
end;

procedure TFrmConfigClient.btnSaveDrugAndRestoreClick(Sender: TObject);
begin
  // 这几个参数不用发到客户端，回血由服务器端运算得出

  Config.WriteInteger('Setup', 'PerSpell', g_Config.nPerSpell);
  Config.WriteInteger('Setup', 'PerHealth', g_Config.nPerHealth);
  Config.WriteInteger('Setup', 'IncHealthSpellTime', g_Config.nIncHealthSpellTime);

  Config.WriteInteger('Setup', 'HealthFillTime', g_Config.nHealthFillTime);
  Config.WriteInteger('Setup', 'HealthFillTime_Human_Warrior', g_Config.nHealthFillTime_Human_Warrior);
  Config.WriteInteger('Setup', 'HealthFillTime_Human_TaoistAndWizard', g_Config.nHealthFillTime_Human_TaoistAndWizard);
  Config.WriteInteger('Setup', 'HealthFillTime_Hero_Warrior', g_Config.nHealthFillTime_Hero_Warrior);
  Config.WriteInteger('Setup', 'HealthFillTime_Hero_TaoistAndWizard', g_Config.nHealthFillTime_Hero_TaoistAndWizard);

  Config.WriteInteger('Setup', 'SpellFillTime', g_Config.nSpellFillTime);
  Config.WriteInteger('Setup', 'SpellFillTime_Human_Warrior', g_Config.nSpellFillTime_Human_Warrior);
  Config.WriteInteger('Setup', 'SpellFillTime_Human_TaoistAndWizard', g_Config.nSpellFillTime_Human_TaoistAndWizard);
  Config.WriteInteger('Setup', 'SpellFillTime_Hero_Warrior', g_Config.nSpellFillTime_Hero_Warrior);
  Config.WriteInteger('Setup', 'SpellFillTime_Hero_TaoistAndWizard', g_Config.nSpellFillTime_Hero_TaoistAndWizard);

  Config.WriteInteger('Setup', 'HealthBaseNum', g_Config.nHealthBaseNum);
  Config.WriteInteger('Setup', 'SpellBaseNum', g_Config.nSpellBaseNum);

  Config.WriteInteger('Setup', 'HealthBaseNum_Human_Warrior', g_Config.nHealthBaseNum_Human_Warrior);
  Config.WriteInteger('Setup', 'HealthBaseNum_Human_TaoistAndWizard', g_Config.nHealthBaseNum_Human_TaoistAndWizard);
  Config.WriteInteger('Setup', 'HealthBaseNum_Hero_Warrior', g_Config.nHealthBaseNum_Hero_Warrior);
  Config.WriteInteger('Setup', 'HealthBaseNum_Hero_TaoistAndWizard', g_Config.nHealthBaseNum_Hero_TaoistAndWizard);

  Config.WriteInteger('Setup', 'SpellBaseNum_Human_Warrior', g_Config.nSpellBaseNum_Human_Warrior);
  Config.WriteInteger('Setup', 'SpellBaseNum_Human_TaoistAndWizard', g_Config.nSpellBaseNum_Human_TaoistAndWizard);
  Config.WriteInteger('Setup', 'SpellBaseNum_Hero_Warrior', g_Config.nSpellBaseNum_Hero_Warrior);
  Config.WriteInteger('Setup', 'SpellBaseNum_Hero_TaoistAndWizard', g_Config.nSpellBaseNum_Hero_TaoistAndWizard);

  Config.WriteInteger('Setup', 'UseItemIntervalTime', g_Config.dwUseItemIntervalTime);
  Config.WriteInteger('Setup', 'UseAttackItemIntervalTime', g_Config.dwUseAttackItemIntervalTime);

  Config.WriteInteger('Setup', 'UseOrdinaryTime_Human_Warrior', g_Config.dwUseOrdinaryTime_Human_Warrior);
  Config.WriteInteger('Setup', 'UseSpecialTime_Human_Warrior', g_Config.dwUseSpecialTime_Human_Warrior);
  Config.WriteInteger('Setup', 'UseOrdinaryTime_Human_TaoistAndWizard', g_Config.dwUseOrdinaryTime_Human_TaoistAndWizard);
  Config.WriteInteger('Setup', 'UseSpecialTime_Human_TaoistAndWizard', g_Config.dwUseSpecialTime_Human_TaoistAndWizard);

  Config.WriteInteger('Setup', 'UseOrdinaryTime_Hero_Warrior', g_Config.dwUseOrdinaryTime_Hero_Warrior);
  Config.WriteInteger('Setup', 'UseSpecialTime_Hero_Warrior', g_Config.dwUseSpecialTime_Hero_Warrior);
  Config.WriteInteger('Setup', 'UseOrdinaryTime_Hero_TaoistAndWizard', g_Config.dwUseOrdinaryTime_Hero_TaoistAndWizard);
  Config.WriteInteger('Setup', 'UseSpecialTime_Hero_TaoistAndWizard', g_Config.dwUseSpecialTime_Hero_TaoistAndWizard);

  Config.WriteInteger('Setup', 'IncHealingLimite', g_Config.nIncHealingLimite);
  Config.WriteInteger('Setup', 'PerHealing', g_Config.nPerHealing);
  Config.WriteInteger('Setup', 'PerHealingTime', g_Config.nPerHealingTime);
  Config.WriteInteger('Setup', 'BigPerHealing', g_Config.nBigPerHealing);
  Config.WriteInteger('Setup', 'BigPerHealingTime', g_Config.nBigPerHealingTime);

  if boSendServerConfig then
  begin
    UserEngine.SendServerConfig();
    boSendServerConfig := False;
  end;
  uModValue();
end;

procedure TFrmConfigClient.seHealthFillTimeChange(Sender: TObject);
begin
  // 怪物回血速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthFillTime := seHealthFillTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHealthBaseNumChange(Sender: TObject);
begin
  // 怪物回血基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthBaseNum := seHealthBaseNum.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellFillTimeChange(Sender: TObject);
begin
  // 怪物回蓝速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellFillTime := seSpellFillTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellBaseNumChange(Sender: TObject);
begin
  // 怪物回蓝速基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellBaseNum := seSpellBaseNum.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHealthFillTime_Human_WarriorChange(Sender: TObject);
begin
  // 人物－战士职业回血速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthFillTime_Human_Warrior := seHealthFillTime_Human_Warrior.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHealthBaseNum_Human_WarriorChange(Sender: TObject);
begin
  // 人物－战士职业回血基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthBaseNum_Human_Warrior := seHealthBaseNum_Human_Warrior.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellFillTime_Human_WarriorChange(Sender: TObject);
begin
  // 人物－战士职业回蓝速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellFillTime_Human_Warrior := seSpellFillTime_Human_Warrior.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellBaseNum_Human_WarriorChange(Sender: TObject);
begin
  // 人物－战士职业回蓝基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellBaseNum_Human_Warrior := seSpellBaseNum_Human_Warrior.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHealthFillTime_Human_TaoistAndWizardChange(Sender: TObject);
begin
  // 人物－道法职业回血速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthFillTime_Human_TaoistAndWizard := seHealthFillTime_Human_TaoistAndWizard.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHealthBaseNum_Human_TaoistAndWizardChange(Sender: TObject);
begin
  // 人物－道法职业回血基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthBaseNum_Human_TaoistAndWizard := seHealthBaseNum_Human_TaoistAndWizard.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellFillTime_Human_TaoistAndWizardChange(Sender: TObject);
begin
  // 人物－道法职业回蓝速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellFillTime_Human_TaoistAndWizard := seSpellFillTime_Human_TaoistAndWizard.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellBaseNum_Human_TaoistAndWizardChange(Sender: TObject);
begin
  // 人物－道法职业回蓝基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellBaseNum_Human_TaoistAndWizard := seSpellBaseNum_Human_TaoistAndWizard.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHealthFillTime_Hero_WarriorChange(Sender: TObject);
begin
  // 英雄－战士职业回血速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthFillTime_Hero_Warrior := seHealthFillTime_Hero_Warrior.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHealthBaseNum_Hero_WarriorChange(Sender: TObject);
begin
  // 英雄－战士职业回血基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthBaseNum_Hero_Warrior := seHealthBaseNum_Hero_Warrior.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellFillTime_Hero_WarriorChange(Sender: TObject);
begin
  // 英雄－战士职业回蓝速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellFillTime_Hero_Warrior := seSpellFillTime_Hero_Warrior.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellBaseNum_Hero_WarriorChange(Sender: TObject);
begin
  // 英雄－战士职业回蓝基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellBaseNum_Hero_Warrior := seSpellBaseNum_Hero_Warrior.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHealthFillTime_Hero_TaoistAndWizardChange(Sender: TObject);
begin
  // 英雄－道法职业回血速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthFillTime_Hero_TaoistAndWizard := seHealthFillTime_Hero_TaoistAndWizard.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHealthBaseNum_Hero_TaoistAndWizardChange(Sender: TObject);
begin
  // 英雄－道法职业回血基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nHealthBaseNum_Hero_TaoistAndWizard := seHealthBaseNum_Hero_TaoistAndWizard.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellFillTime_Hero_TaoistAndWizardChange(Sender: TObject);
begin
  // 英雄－道法职业回蓝速度 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellFillTime_Hero_TaoistAndWizard := seSpellFillTime_Hero_TaoistAndWizard.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSpellBaseNum_Hero_TaoistAndWizardChange(Sender: TObject);
begin
  // 英雄－道法职业回蓝基数 - 2013-07-16
  if not boOpened then
    Exit;
  g_Config.nSpellBaseNum_Hero_TaoistAndWizard := seSpellBaseNum_Hero_TaoistAndWizard.Value;
  ModValue();
end;

procedure TFrmConfigClient.seUseItemIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwUseItemIntervalTime := seUseItemIntervalTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.seUseOrdinaryTime_Human_WarriorChange(Sender: TObject);
begin
  // 人物战士吃特殊药间隔 - 2013-07-16  (+ chongchong)
  if not boOpened then
    Exit;
  g_Config.dwUseOrdinaryTime_Human_Warrior := seUseOrdinaryTime_Human_Warrior.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seUseSpecialTime_Human_WarriorChange(Sender: TObject);
begin
  // 人物战士吃特殊药间隔 - 2013-07-16  (+ chongchong)
  if not boOpened then
    Exit;
  g_Config.dwUseSpecialTime_Human_Warrior := seUseSpecialTime_Human_Warrior.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seUseOrdinaryTime_Human_TaoistAndWizardChange(Sender: TObject);
begin
  // 人物道法吃普通药间隔 - 2013-07-16  (+ chongchong)
  if not boOpened then
    Exit;
  g_Config.dwUseOrdinaryTime_Human_TaoistAndWizard := seUseOrdinaryTime_Human_TaoistAndWizard.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seUseSpecialTime_Human_TaoistAndWizardChange(Sender: TObject);
begin
  // 人物道法吃特殊药间隔 - 2013-07-16  (+ chongchong)
  if not boOpened then
    Exit;
  g_Config.dwUseSpecialTime_Human_TaoistAndWizard := seUseSpecialTime_Human_TaoistAndWizard.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seUseOrdinaryTime_Hero_WarriorChange(Sender: TObject);
begin
  // 英雄战士吃普通药间隔 - 2013-07-16  (+ chongchong)
  if not boOpened then
    Exit;
  g_Config.dwUseOrdinaryTime_Hero_Warrior := seUseOrdinaryTime_Hero_Warrior.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seUseSpecialTime_Hero_WarriorChange(Sender: TObject);
begin
  // 英雄道法吃普通药间隔 - 2013-07-16  (+ chongchong)
  if not boOpened then
    Exit;
  g_Config.dwUseSpecialTime_Hero_Warrior := seUseSpecialTime_Hero_Warrior.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seUseOrdinaryTime_Hero_TaoistAndWizardChange(Sender: TObject);
begin
  // 英雄战士吃特殊药间隔 - 2013-07-16  (+ chongchong)
  if not boOpened then
    Exit;
  g_Config.dwUseOrdinaryTime_Hero_TaoistAndWizard := seUseOrdinaryTime_Hero_TaoistAndWizard.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seUseSpecialTime_Hero_TaoistAndWizardChange(Sender: TObject);
begin
  // 英雄道法吃特殊药间隔 - 2013-07-16  (+ chongchong)
  if not boOpened then
    Exit;
  g_Config.dwUseSpecialTime_Hero_TaoistAndWizard := seUseSpecialTime_Hero_TaoistAndWizard.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.btnRestoreDefaultClick(Sender: TObject);
var
  dwUseSpecialTime: LongWord;                                                                       // 特殊药品间隔
  dwUseOrdinaryTime: LongWord;                                                                      // 普通药品间隔
  nHealthFillTime: Integer;                                                                         // 恢复速度
  nSpellFillTime: Integer;                                                                          // 魔法速度
  nHPBaseNum: Integer;                                                                              // 体力恢复基数
  nMPBaseNum: Integer;                                                                              // 魔法恢复基数
begin
  nHealthFillTime := 350;
  dwUseSpecialTime := 1000;
  dwUseOrdinaryTime := 300;
  nSpellFillTime := 800;

  nHPBaseNum := 75;
  nMPBaseNum := 18;

  seUseOrdinaryTime_Human_Warrior.Value := dwUseOrdinaryTime;
  seUseSpecialTime_Human_Warrior.Value := dwUseSpecialTime;
  seUseOrdinaryTime_Human_TaoistAndWizard.Value := dwUseOrdinaryTime;
  seUseSpecialTime_Human_TaoistAndWizard.Value := dwUseSpecialTime;

  seUseOrdinaryTime_Hero_Warrior.Value := dwUseOrdinaryTime;
  seUseSpecialTime_Hero_Warrior.Value := dwUseSpecialTime;
  seUseOrdinaryTime_Hero_TaoistAndWizard.Value := dwUseOrdinaryTime;
  seUseSpecialTime_Hero_TaoistAndWizard.Value := dwUseSpecialTime;

  sePerHealth.Value := 10;
  sePerSpell.Value := 10;
  seIncHealthSpell.Value := 700;
  seUseItemIntervalTime.Value := 500;
  seUseAttackItemIntervalTime.Value := 500;

  seHealthFillTime_Human_Warrior.Value := nHealthFillTime;
  seHealthBaseNum_Human_Warrior.Value := nHPBaseNum;
  seSpellFillTime_Human_Warrior.Value := nSpellFillTime;
  seSpellBaseNum_Human_Warrior.Value := nMPBaseNum;
  seHealthFillTime_Human_TaoistAndWizard.Value := nHealthFillTime;
  seHealthBaseNum_Human_TaoistAndWizard.Value := nHPBaseNum;
  seSpellFillTime_Human_TaoistAndWizard.Value := nSpellFillTime;
  seSpellBaseNum_Human_TaoistAndWizard.Value := nMPBaseNum;

  seHealthFillTime_Hero_Warrior.Value := nHealthFillTime;
  seHealthBaseNum_Hero_Warrior.Value := nHPBaseNum;
  seSpellFillTime_Hero_Warrior.Value := nSpellFillTime;
  seSpellBaseNum_Hero_Warrior.Value := nMPBaseNum;
  seHealthFillTime_Hero_TaoistAndWizard.Value := nHealthFillTime;
  seHealthBaseNum_Hero_TaoistAndWizard.Value := nHPBaseNum;
  seSpellFillTime_Hero_TaoistAndWizard.Value := nSpellFillTime;
  seSpellBaseNum_Hero_TaoistAndWizard.Value := nMPBaseNum;

  seHealthFillTime.Value := 450;
  seHealthBaseNum.Value := nHPBaseNum;
  seSpellFillTime.Value := 800;
  seSpellBaseNum.Value := nMPBaseNum;

  sePerHealingTime.Value := 700;
  sePerHealing.Value := 5;
  seBigPerHealingTime.Value := 700;
  seBigPerHealing.Value := 5;

  ModValue();
end;

procedure TFrmConfigClient.chkShowHintLinesClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowHintLines := chkShowHintLines.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkMinMapCloseRadarClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMinMapCloseRadar := chkMinMapCloseRadar.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMinMapFlagFlashChange(Sender: TObject);
begin
  // 玩家自身地雷闪烁
  if not boOpened then
    Exit;
  g_Config.dwMinMapFlagFlash := seMinMapFlagFlash.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMinMapColorSelfChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seMinMapColorSelf.Value;
  if not boOpened then
    Exit;
  g_Config.btMinMapColorSelf := btColor;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMinMapColorOtherChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seMinMapColorOther.Value;
  if not boOpened then
    Exit;
  g_Config.btMinMapColorOther := btColor;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMinMapColorNPCChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seMinMapColorNPC.Value;
  if not boOpened then
    Exit;
  g_Config.btMinMapColorNPC := btColor;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMinMapColorMonsterChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seMinMapColorMonster.Value;
  if not boOpened then
    Exit;
  g_Config.btMinMapColorMonster := btColor;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMinMapColorGuardChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seMinMapColorGuard.Value;
  if not boOpened then
    Exit;
  g_Config.btMinMapColorGuard := btColor;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMinMapColorHeroChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seMinMapColorHero.Value;
  if not boOpened then
    Exit;
  g_Config.btMinMapColorHero := btColor;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMinMapColorBossChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seMinMapColorBoss.Value;
  if not boOpened then
    Exit;
  g_Config.btMinMapColorBoss := btColor;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.btnSaveOption2Click(Sender: TObject);
begin
{
    : Boolean;                    // 关闭雷达显示 chongchong 2013-07-19
    : Boolean;                        // 使用老式雷达  chongchong 2013-07-19
    : LongWord;                    // 小地图中玩家自身闪烁频率  chongchong 2013-07-19

    : Byte;                        // 玩家自身颜色  chongchong 2013-07-19
    : Byte;                       // 其他玩家自身颜色  chongchong 2013-07-19
    : Byte;                         // NPC颜色  chongchong 2013-07-19
    : Byte;                        // BOSS颜色  chongchong 2013-07-19
    : Byte;                       // 守卫颜色  chongchong 2013-07-19
    : Byte;                     // 怪物颜色  chongchong 2013-07-19
    : Byte;                        // 英雄颜色  chongchong 2013-07-19
    : Byte;                        // 宝宝颜色  chongchong 2013-07-19
}
  Config.WriteBool('Setup', 'UseFindPath', g_Config.boUseFindPath);

  Config.WriteBool('Setup', 'MinMapCloseRadar', g_Config.boMinMapCloseRadar);
  Config.WriteInteger('Setup', 'MinMapUseOld', g_Config.btMinMapType);
  Config.WriteBool('Setup', 'MinMapUseFindPath', g_Config.boMinMapUseFindPath);
  Config.WriteBool('Setup', 'LoginShowMinMap', g_Config.boLoginShowMinMap);
  Config.WriteInteger('Setup', 'MinMapFlagFlash', g_Config.dwMinMapFlagFlash);

  Config.WriteInteger('Setup', 'MinMapColorSelf', g_Config.btMinMapColorSelf);
  Config.WriteInteger('Setup', 'MinMapColorOther', g_Config.btMinMapColorOther);
  Config.WriteInteger('Setup', 'MinMapColorNPC', g_Config.btMinMapColorNPC);
  Config.WriteInteger('Setup', 'MinMapColorGuard', g_Config.btMinMapColorGuard);

  Config.WriteInteger('Setup', 'MinMapColorMonster', g_Config.btMinMapColorMonster);
  Config.WriteInteger('Setup', 'MinMapColorHero', g_Config.btMinMapColorHero);
  Config.WriteInteger('Setup', 'MinMapColorBoss', g_Config.btMinMapColorBoss);

  Config.WriteBool('Setup', 'HideItemNameNum', g_Config.boHideItemNameNum);

  Config.WriteBool('Setup', 'KeyTabGetActor', g_Config.boKeyTabGetActor);

  Config.WriteBool('Setup', 'ShowMagicShieldHP', g_Config.boShowMagicShieldHP);

  Config.WriteInteger('Setup', 'HumHPBarOffsetX', g_Config.nHumHPBarOffsetX);
  Config.WriteInteger('Setup', 'HumHPBarOffsetY', g_Config.nHumHPBarOffsetY);

  Config.WriteInteger('Setup', 'NpcHPBarOffsetX', g_Config.nNpcHPBarOffsetX);
  Config.WriteInteger('Setup', 'NpcHPBarOffsetY', g_Config.nNpcHPBarOffsetY);

  Config.WriteInteger('Setup', 'MonHPBarOffsetX', g_Config.nMonHPBarOffsetX);
  Config.WriteInteger('Setup', 'MonHPBarOffsetY', g_Config.nMonHPBarOffsetY);

  Config.WriteInteger('Setup', 'HumNameOffsetX', g_Config.nHumNameOffsetX);
  Config.WriteInteger('Setup', 'HumNameOffsetY', g_Config.nHumNameOffsetY);

  Config.WriteInteger('Setup', 'NpcNameOffsetX', g_Config.nNpcNameOffsetX);
  Config.WriteInteger('Setup', 'NpcNameOffsetY', g_Config.nNpcNameOffsetY);

  Config.WriteInteger('Setup', 'MonNameOffsetX', g_Config.nMonNameOffsetX);
  Config.WriteInteger('Setup', 'MonNameOffsetY', g_Config.nMonNameOffsetY);

  Config.WriteBool('Setup', 'HealthNumberText', g_Config.boHealthNumberText);
  Config.WriteBool('Setup', 'BlastHitShowHealthNum', g_Config.boBlastHitShowHealthNum);
  Config.WriteBool('Setup', 'PoisoningHideHealthNum', g_Config.boPoisoningHideHealthNum);
  Config.WriteBool('Setup', 'HPStoneHideHealthNum', g_Config.boHPStoneHideHealthNum);
  Config.WriteBool('Setup', 'MPStoneHideHealthNum', g_Config.boMPStoneHideHealthNum);

  Config.WriteInteger('Setup', 'HealthNumberOffsetX', g_Config.nHealthNumberOffsetX);
  Config.WriteInteger('Setup', 'HealthNumberOffsetY', g_Config.nHealthNumberOffsetY);
  Config.WriteInteger('Setup', 'HealthNumberMoveSpeed', g_Config.nHealthNumberMoveSpeed);

  Config.WriteInteger('Setup', 'NewLeftGroupInfoOffsetX', g_Config.nNewLeftGroupInfoOffsetX);
  Config.WriteInteger('Setup', 'NewLeftGroupInfoOffsetY', g_Config.nNewLeftGroupInfoOffsetY);

  Config.WriteBool('Setup', 'MagicSetDir', g_Config.boMagicSetDir);

  Config.WriteInteger('Setup', 'BetterItemX', g_Config.nBetterItemX);
  Config.WriteInteger('Setup', 'BetterItemY', g_Config.nBetterItemY);
  Config.WriteInteger('Setup', 'SmallInfoX', g_Config.nSmallInfoX);
  Config.WriteInteger('Setup', 'SmallInfoY', g_Config.nSmallInfoY);
  Config.WriteInteger('Setup', 'JoyStickX', g_Config.nJoyStickX);
  Config.WriteInteger('Setup', 'JoyStickY', g_Config.nJoyStickY);
  Config.WriteInteger('Setup', 'JoyStickMaxX', g_Config.nJoyStickMaxX);
  Config.WriteInteger('Setup', 'JoyStickMaxY', g_Config.nJoyStickMaxY);
  Config.WriteInteger('Setup', 'SkillCtrX', g_Config.nSkillCtrX);
  Config.WriteInteger('Setup', 'SkillCtrY', g_Config.nSkillCtrY);
  Config.WriteBool('Setup', 'ShowExSkillIcon', g_Config.boShowExSkillIcon);
  Config.WriteInteger('Setup', 'MapScale', g_Config.btMapScale);
  Config.WriteInteger('Setup', 'GuiScale', g_Config.btGuiScale);
  Config.WriteInteger('Setup', 'MultiViewRange', g_Config.btMultiViewRange);
  Config.WriteBool('Setup', 'ShowMulitDlg', g_Config.boShowMulitDlg);
  Config.WriteBool('Setup', 'ShowBetterItem', g_Config.boShowBetterItem);

  if boSendServerConfig then
  begin
    UserEngine.SendServerConfig();
    boSendServerConfig := False;
  end;
  uModValue();
end;

procedure TFrmConfigClient.seHitFrameTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHitFrameTime := seHitFrameTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.seMagicHitFrameTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMagicHitFrameTime := seMagicHitFrameTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.seMapScaleChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMapScale := seMapScale.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkGemUpgradeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGemUpgrade := chkGemUpgrade.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowGuildNameClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowGuildName := chkShowGuildName.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShopGuiCanMoveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShopGuiCanMove := chkShopGuiCanMove.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkNPCGuiCanMoveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNPCGuiCanMove := chkNPCGuiCanMove.Checked;
  ModValue();
end;

procedure TFrmConfigClient.sePerHealingTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPerHealingTime := sePerHealingTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.sePerHealingChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPerHealing := sePerHealing.Value;
  ModValue();
end;

procedure TFrmConfigClient.seBigPerHealingTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBigPerHealingTime := seBigPerHealingTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.seBetterItemXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBetterItemX := seBetterItemX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seBetterItemYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBetterItemY := seBetterItemY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seBigPerHealingChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBigPerHealing := seBigPerHealing.Value;
  ModValue();
end;

procedure TFrmConfigClient.chkNpcDlgHintWithMouseClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNpcDlgHintWithMouse := chkNpcDlgHintWithMouse.Checked;
  ModValue();
end;

procedure TFrmConfigClient.seThrowAwayItemColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btThrowAwayItemColor := seThrowAwayItemColor.Value;
  ModValue();
end;

procedure TFrmConfigClient.chkShowGloryClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowGlory := chkShowGlory.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowHorseButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowHorseButton := chkShowHorseButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.sePluginPickupTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwPluginPickupTime := sePluginPickupTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.seShowHintNameFontSizeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowHintNameFontSize := seShowHintNameFontSize.Value;
  ModValue();
end;

procedure TFrmConfigClient.chkHideItemNameNumClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHideItemNameNum := chkHideItemNameNum.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkShowDeputyHeroButtonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowDeputyHeroButton := chkShowDeputyHeroButton.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowExSkillIconClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowExSkillIcon := chkShowExSkillIcon.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkShowBagArrangeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowBagArrange := chkShowBagArrange.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkKeyTabGetActorClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKeyTabGetActor := chkKeyTabGetActor.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.cbbTitleFileIndexChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nTitleFileIndex := cbbTitleFileIndex.ItemIndex;
  ModValue();
end;

procedure TFrmConfigClient.chkShowMagicShieldHPClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowMagicShieldHP := chkShowMagicShieldHP.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkShowMulitDlgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowMulitDlg := chkShowMulitDlg.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.RefClientPlugTableVisible;
begin
  RzCheckGroupClientTabSheet.Items.Clear;
  if g_Config.btConfigDlgType = 0 then
  begin
    RzCheckGroupClientTabSheet.Items.Add('基本');
    RzCheckGroupClientTabSheet.Items.Add('物品');
    RzCheckGroupClientTabSheet.Items.Add('保护');
    RzCheckGroupClientTabSheet.Items.Add('药品');
    RzCheckGroupClientTabSheet.Items.Add('技能');
    RzCheckGroupClientTabSheet.Items.Add('按键');
    RzCheckGroupClientTabSheet.Items.Add('战斗');
    RzCheckGroupClientTabSheet.Items.Add('挂机');
    RzCheckGroupClientTabSheet.Items.Add('便签');
    RzCheckGroupClientTabSheet.Items.Add('帮助');
  end
  else
  begin
    RzCheckGroupClientTabSheet.Items.Add('基本');
    RzCheckGroupClientTabSheet.Items.Add('技能');
    RzCheckGroupClientTabSheet.Items.Add('保护');
    RzCheckGroupClientTabSheet.Items.Add('战斗');
    RzCheckGroupClientTabSheet.Items.Add('物品');
    RzCheckGroupClientTabSheet.Items.Add('NPC');
    RzCheckGroupClientTabSheet.Items.Add('挂机');
    RzCheckGroupClientTabSheet.Items.Add('按键');
    RzCheckGroupClientTabSheet.Items.Add('便签');
    RzCheckGroupClientTabSheet.Items.Add('帮助');
    RzCheckGroupClientTabSheet.Items.Add('英雄');
    RzCheckGroupClientTabSheet.Items.Add('发言');
  end;
end;

procedure TFrmConfigClient.chkShowNormalFashionClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowNormalFashion := chkShowNormalFashion.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowHeroShortKeyClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowHeroShortKey := chkShowHeroShortKey.Checked;
  ModValue();
end;

procedure TFrmConfigClient.seShowHeroShortKeyXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShowHeroShortKeyX := seShowHeroShortKeyX.Value;
  ModValue();
end;

procedure TFrmConfigClient.seShowHeroShortKeyYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShowHeroShortKeyY := seShowHeroShortKeyY.Value;
  ModValue();
end;

procedure TFrmConfigClient.chkFashionJewelryOpenClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boFashionJewelryOpen := chkFashionJewelryOpen.Checked;
  ModValue();
end;

procedure TFrmConfigClient.sePluginMinEatItemTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwPluginMinEatItemTime := sePluginMinEatItemTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.chkHealthNumberTextClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHealthNumberText := chkHealthNumberText.Checked;
  boSendServerConfig := True;

  chkBlastHitShowHealthNum.Enabled := chkHealthNumberText.Checked;
  chkHPStoneHideHealthNum.Enabled := not chkHealthNumberText.Checked;
  chkMPStoneHideHealthNum.Enabled := not chkHealthNumberText.Checked;

  ModValue();
end;

procedure TFrmConfigClient.chkHeroHideTabSheet5Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroHideTabSheet5 := chkHeroHideTabSheet5.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHeroHideTabSheet2Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroHideTabSheet2 := chkHeroHideTabSheet2.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowItemFormClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowItemForm := chkShowItemForm.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowItemSellPriceClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowItemSellPrice := chkShowItemSellPrice.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowFashionHideShieldClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowFashionHideShield := chkShowFashionHideShield.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowFashionHideHatsClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowFashionHideHats := chkShowFashionHideHats.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowInsuranceInfoClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowInsuranceInfo := chkShowInsuranceInfo.Checked;
  ModValue();
end;

procedure TFrmConfigClient.seIncSpeedDecIntervalChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwIncSpeedDecInterval := seIncSpeedDecInterval.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkGreenHintNewStyleClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGreenHintNewStyle := chkGreenHintNewStyle.Checked;
  ModValue();
end;

procedure TFrmConfigClient.seUseAttackItemIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwUseAttackItemIntervalTime := seUseAttackItemIntervalTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.chkMagicSetDirClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMagicSetDir := chkMagicSetDir.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkBlastHitShowHealthNumClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBlastHitShowHealthNum := chkBlastHitShowHealthNum.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkPoisoningHideHealthNumClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPoisoningHideHealthNum := chkPoisoningHideHealthNum.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkHideIconWithHideTitleClick(Sender: TObject);
begin
  {
  if not boOpened then Exit;
  g_Config.boHideIconWithHideTitle := chkHideIconWithHideTitle.Checked;
  ModValue();
  }
end;

procedure TFrmConfigClient.chkHPStoneHideHealthNumClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHPStoneHideHealthNum := chkHPStoneHideHealthNum.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkMPStoneHideHealthNumClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMPStoneHideHealthNum := chkMPStoneHideHealthNum.Checked;
  ModValue();
end;

procedure TFrmConfigClient.seIncMoveSpeedDecIntervalChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwIncMoveSpeedDecInterval := seIncMoveSpeedDecInterval.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seIncSpellSpeedDecIntervalChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwIncSpellSpeedDecInterval := seIncSpellSpeedDecInterval.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seJoyStickMaxXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nJoyStickMaxX := seJoyStickMaxX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seJoyStickMaxYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nJoyStickMaxY := seJoyStickMaxY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seJoyStickXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nJoyStickX := seJoyStickX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seJoyStickYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nJoyStickY := seJoyStickY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMoveFrameTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMoveFrameTime := seMoveFrameTime.Value;
  ModValue();
end;

procedure TFrmConfigClient.seMultiViewRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMultiViewRange := seMultiViewRange.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkShowBagGameGoldSeparatorClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowBagGameGoldSeparator := chkShowBagGameGoldSeparator.Checked;
  ModValue();
end;

procedure TFrmConfigClient.edtShowHintFontNameChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seShowHintOtherFontSizeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowHintOtherFontSize := seShowHintOtherFontSize.Value;
  ModValue();
end;

procedure TFrmConfigClient.cbbShowHintNameFontBoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowHintNameFontBold := cbbShowHintNameFontBold.ItemIndex;
  ModValue();
end;

procedure TFrmConfigClient.cbbShowHintNameFontStrokeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowHintNameFontStroke := cbbShowHintNameFontStroke.ItemIndex;
  ModValue();
end;

procedure TFrmConfigClient.cbbShowHintOtherFontBoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowHintOtherFontBold := cbbShowHintOtherFontBold.ItemIndex;
  ModValue();
end;

procedure TFrmConfigClient.cbbShowHintOtherFontStrokeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowHintOtherFontStroke := cbbShowHintOtherFontStroke.ItemIndex;
  ModValue();
end;

procedure TFrmConfigClient.seShowItemFormColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowItemFormColor := seShowItemFormColor.Value;
  ModValue();
end;

procedure TFrmConfigClient.seShowInsuranceInfoColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowInsuranceInfoColor := seShowInsuranceInfoColor.Value;
  ModValue();
end;

procedure TFrmConfigClient.seShowItemSellPriceColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowItemSellPriceColor := seShowItemSellPriceColor.Value;
  ModValue();
end;

procedure TFrmConfigClient.seSkillCtrXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkillCtrX := seSkillCtrX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seSkillCtrYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkillCtrY := seSkillCtrY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seSmallInfoXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSmallInfoX := seSmallInfoX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seSmallInfoYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSmallInfoY := seSmallInfoY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seHealthNumberOffsetXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHealthNumberOffsetX := seHealthNumberOffsetX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seHealthNumberOffsetYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHealthNumberOffsetY := seHealthNumberOffsetY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seHealthNumberMoveSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHealthNumberMoveSpeed := seHealthNumberMoveSpeed.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkShowBagGameInfoClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowBagGameInfo := chkShowBagGameInfo.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkShowBetterItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowBetterItem := chkShowBetterItem.Checked;
  ModValue();
end;

procedure TFrmConfigClient.cbbBagRightkeyChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBagRightkey := cbbBagRightkey.ItemIndex = 1;
  ModValue();
end;

procedure TFrmConfigClient.cbbMinMapTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMinMapType := cbbMinMapType.ItemIndex;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seNextArrBtnOffsetX1Change(Sender: TObject);
var
  Ctrl: TSpinEditEx;
begin
  Ctrl := Sender as TSpinEditEx;
  g_ArrButtonConfig[Ctrl.Tag].NextOffsetX := Ctrl.Value;
  btnArrBtnSetting.Enabled := True;
end;

procedure TFrmConfigClient.seArrBtnOffsetX1Change(Sender: TObject);
var
  Ctrl: TSpinEditEx;
begin
  Ctrl := Sender as TSpinEditEx;
  g_ArrButtonConfig[Ctrl.Tag].OffsetX := Ctrl.Value;
  btnArrBtnSetting.Enabled := True;
end;

procedure TFrmConfigClient.cbbArrBtnVertAlign1Change(Sender: TObject);
var
  Ctrl: TComboBox;
begin
  Ctrl := Sender as TComboBox;
  g_ArrButtonConfig[Ctrl.Tag].VertAligment := TVerticalAlignment(Ctrl.ItemIndex);
  btnArrBtnSetting.Enabled := True;
end;

procedure TFrmConfigClient.seArrBtnOffsetY1Change(Sender: TObject);
var
  Ctrl: TSpinEditEx;
begin
  Ctrl := Sender as TSpinEditEx;
  g_ArrButtonConfig[Ctrl.Tag].OffsetY := Ctrl.Value;
  btnArrBtnSetting.Enabled := True;
end;

procedure TFrmConfigClient.seNextArrBtnOffsetY1Change(Sender: TObject);
var
  Ctrl: TSpinEditEx;
begin
  Ctrl := Sender as TSpinEditEx;
  g_ArrButtonConfig[Ctrl.Tag].NextOffsetY := Ctrl.Value;
  btnArrBtnSetting.Enabled := True;
end;

procedure TFrmConfigClient.cbbArrBtnHorzAlign1Change(Sender: TObject);
var
  Ctrl: TComboBox;
begin
  Ctrl := Sender as TComboBox;
  g_ArrButtonConfig[Ctrl.Tag].HorzAligment := TAlignment(Ctrl.ItemIndex);
  btnArrBtnSetting.Enabled := True;
end;

procedure TFrmConfigClient.btnArrBtnSettingClick(Sender: TObject);
var
  I: Integer;
  StrIndex: string;
  GroupConfig: PArrButtonGroupConfig;
begin
  for I := Low(g_ArrButtonConfig) to High(g_ArrButtonConfig) do
  begin
    GroupConfig := @g_ArrButtonConfig[I];
    StrIndex := IntToStr(I);

    Config.WriteInteger('ArrButton', 'HorzAlign' + StrIndex, Integer(GroupConfig.HorzAligment));
    Config.WriteInteger('ArrButton', 'OffsetX' + StrIndex, GroupConfig.OffsetX);
    Config.WriteInteger('ArrButton', 'VertAlign' + StrIndex, Integer(GroupConfig.VertAligment));
    Config.WriteInteger('ArrButton', 'OffsetY' + StrIndex, GroupConfig.OffsetY);
    Config.WriteInteger('ArrButton', 'NextOffsetX' + StrIndex, GroupConfig.NextOffsetX);
    Config.WriteInteger('ArrButton', 'NextOffsetY' + StrIndex, GroupConfig.NextOffsetY);
  end;

  g_ArrButtonConfigCRC := BufferCrc(@g_ArrButtonConfig, SizeOf(g_ArrButtonConfig));

  btnArrBtnSetting.Enabled := False;
end;

procedure TFrmConfigClient.chkMinMapUseFindPathClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMinMapUseFindPath := chkMinMapUseFindPath.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkLoginShowMinMapClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLoginShowMinMap := chkLoginShowMinMap.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seNewLeftGroupInfoOffsetXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNewLeftGroupInfoOffsetX := seNewLeftGroupInfoOffsetX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seNewLeftGroupInfoOffsetYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNewLeftGroupInfoOffsetY := seNewLeftGroupInfoOffsetY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.chkMoveItemShowIDClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMoveItemShowID := chkMoveItemShowID.Checked;
  ModValue();
end;

procedure TFrmConfigClient.chkItemFromField0Click(Sender: TObject);
var
  chk: TCheckBox;
begin
  chk := Sender as TCheckBox;
  if not boOpened then
    Exit;
  g_Config.boShowItemFromFields[chk.Tag] := chk.Checked;
  ModValue();
end;

procedure TFrmConfigClient.seHumHPBarOffsetXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumHPBarOffsetX := seHumHPBarOffsetX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seHumHPBarOffsetYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumHPBarOffsetY := seHumHPBarOffsetY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seNpcHPBarOffsetXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNpcHPBarOffsetX := seNpcHPBarOffsetX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seNpcHPBarOffsetYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNpcHPBarOffsetY := seNpcHPBarOffsetY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMonHPBarOffsetXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonHPBarOffsetX := seMonHPBarOffsetX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMonHPBarOffsetYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonHPBarOffsetY := seMonHPBarOffsetY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seHumNameOffsetXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumNameOffsetX := seHumNameOffsetX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seHumNameOffsetYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumNameOffsetY := seHumNameOffsetY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seNpcNameOffsetXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNpcNameOffsetX := seNpcNameOffsetX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seNpcNameOffsetYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNpcNameOffsetY := seNpcNameOffsetY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMonNameOffsetXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonNameOffsetX := seMonNameOffsetX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seMonNameOffsetYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonNameOffsetY := seMonNameOffsetY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TFrmConfigClient.seIncHealingLimiteChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nIncHealingLimite := seIncHealingLimite.Value;
  ModValue()
end;

procedure TFrmConfigClient.seHintWindowBorderWidthLeftChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.HintWindowBorderWidth.Left := seHintWindowBorderWidthLeft.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHintWindowBorderWidthTopChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.HintWindowBorderWidth.Top := seHintWindowBorderWidthTop.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHintWindowBorderWidthRightChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.HintWindowBorderWidth.Right := seHintWindowBorderWidthRight.Value;
  ModValue();
end;

procedure TFrmConfigClient.seHintWindowBorderWidthBottomChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.HintWindowBorderWidth.Bottom := seHintWindowBorderWidthBottom.Value;
  ModValue();
end;

procedure TFrmConfigClient.chkHelmetShowInBoxClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHelmetShowInBox := chkHelmetShowInBox.Checked;
  ModValue();
end;

end.


unit GameConfig;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ComCtrls, StdCtrls, Spin, Grids, ExtCtrls,
  RzRadGrp, ColorIndexEdit, SpinEditEx, Math, RzPanel;

type
  TLevelExpScheme = (s_OldLevelExp, s_StdLevelExp, s_2Mult, s_5Mult, s_8Mult, s_10Mult, s_20Mult, s_30Mult, s_40Mult, s_50Mult,
    s_60Mult, s_70Mult, s_80Mult, s_90Mult, s_100Mult, s_150Mult, s_200Mult, s_250Mult, s_300Mult);

  TfrmGameConfig = class(TForm)
    GameConfigControl: TPageControl;
    GameSpeedSheet: TTabSheet;
    Label14: TLabel;
    ExpSheet: TTabSheet;
    GeneralSheet: TTabSheet;
    GroupBoxInfo: TGroupBox;
    Label16: TLabel;
    EditSoftVersionDate: TEdit;
    GroupBox5: TGroupBox;
    Label17: TLabel;
    EditConsoleShowUserCountTime: TSpinEditEx;
    GroupBox6: TGroupBox;
    Label18: TLabel;
    EditShowLineNoticeTime: TSpinEditEx;
    ComboBoxLineNoticeColor: TComboBox;
    Label19: TLabel;
    ButtonGeneralSave: TButton;
    Label21: TLabel;
    EditLineNoticePreFix: TEdit;
    GroupBox8: TGroupBox;
    Label23: TLabel;
    EditKillMonExpMultiple: TSpinEditEx;
    CheckBoxHighLevelKillMonFixExp: TCheckBox;
    ButtonExpSave: TButton;
    CastleSheet: TTabSheet;
    GroupBox9: TGroupBox;
    Label24: TLabel;
    EditRepairDoorPrice: TSpinEditEx;
    Label25: TLabel;
    EditRepairWallPrice: TSpinEditEx;
    Label26: TLabel;
    EditHireArcherPrice: TSpinEditEx;
    Label27: TLabel;
    EditHireGuardPrice: TSpinEditEx;
    GroupBox10: TGroupBox;
    Label31: TLabel;
    Label32: TLabel;
    EditCastleGoldMax: TSpinEditEx;
    EditCastleOneDayGold: TSpinEditEx;
    GroupBox11: TGroupBox;
    Label28: TLabel;
    Label29: TLabel;
    EditCastleHomeX: TSpinEditEx;
    Label30: TLabel;
    EditCastleHomeY: TSpinEditEx;
    EditCastleHomeMap: TEdit;
    GroupBox12: TGroupBox;
    Label34: TLabel;
    Label35: TLabel;
    EditWarRangeX: TSpinEditEx;
    EditWarRangeY: TSpinEditEx;
    ButtonCastleSave: TButton;
    GroupBox13: TGroupBox;
    Label36: TLabel;
    EditTaxRate: TSpinEditEx;
    CheckBoxGetAllNpcTax: TCheckBox;
    GroupBox14: TGroupBox;
    Label33: TLabel;
    EditCastleName: TEdit;
    GroupBoxLevelExp: TGroupBox;
    ComboBoxLevelExp: TComboBox;
    GridLevelExp: TStringGrid;
    Label37: TLabel;
    TabSheet1: TTabSheet;
    ButtonOptionSave: TButton;
    GroupBox16: TGroupBox;
    EditSafeZoneSize: TSpinEditEx;
    Label39: TLabel;
    GroupBox18: TGroupBox;
    Label40: TLabel;
    EditStartPointSize: TSpinEditEx;
    GroupBox20: TGroupBox;
    EditRedHomeX: TSpinEditEx;
    Label42: TLabel;
    Label43: TLabel;
    EditRedHomeY: TSpinEditEx;
    Label44: TLabel;
    EditRedHomeMap: TEdit;
    GroupBox21: TGroupBox;
    Label45: TLabel;
    Label46: TLabel;
    Label47: TLabel;
    EditRedDieHomeX: TSpinEditEx;
    EditRedDieHomeY: TSpinEditEx;
    EditRedDieHomeMap: TEdit;
    TabSheet2: TTabSheet;
    TabSheet3: TTabSheet;
    GroupBox17: TGroupBox;
    chkDisHumRun: TCheckBox;
    chkRunHum: TCheckBox;
    chkRunMon: TCheckBox;
    chkWarDisHumRun: TCheckBox;
    chkRunNpc: TCheckBox;
    ButtonOptionSave3: TButton;
    GroupBox22: TGroupBox;
    Label48: TLabel;
    Label49: TLabel;
    Label50: TLabel;
    EditHomeX: TSpinEditEx;
    EditHomeY: TSpinEditEx;
    EditHomeMap: TEdit;
    ButtonOptionSave2: TButton;
    GroupBox23: TGroupBox;
    Label51: TLabel;
    Label52: TLabel;
    EditDecPkPointTime: TSpinEditEx;
    EditDecPkPointCount: TSpinEditEx;
    Label53: TLabel;
    GroupBox24: TGroupBox;
    Label54: TLabel;
    EditPKFlagTime: TSpinEditEx;
    GroupBox25: TGroupBox;
    Label55: TLabel;
    seHumanAddPKPoint: TSpinEditEx;
    TabSheet4: TTabSheet;
    GroupBox28: TGroupBox;
    CheckBoxTestServer: TCheckBox;
    CheckBoxServiceMode: TCheckBox;
    CheckBoxVentureMode: TCheckBox;
    CheckBoxNonPKMode: TCheckBox;
    GroupBox29: TGroupBox;
    ButtonOptionSave0: TButton;
    seTestLevel: TSpinEditEx;
    Label61: TLabel;
    seTestGold: TSpinEditEx;
    Label62: TLabel;
    seTestUserLimit: TSpinEditEx;
    Label63: TLabel;
    GroupBox31: TGroupBox;
    Label64: TLabel;
    seUserFull: TSpinEditEx;
    GroupBox32: TGroupBox;
    CheckBoxKillHumanWinLevel: TCheckBox;
    CheckBoxKilledLostLevel: TCheckBox;
    CheckBoxKilledLostExp: TCheckBox;
    CheckBoxKillHumanWinExp: TCheckBox;
    Label58: TLabel;
    EditKillHumanWinLevel: TSpinEditEx;
    Label65: TLabel;
    EditKilledLostLevel: TSpinEditEx;
    Label66: TLabel;
    EditKillHumanWinExp: TSpinEditEx;
    EditKillHumanLostExp: TSpinEditEx;
    Label56: TLabel;
    Label67: TLabel;
    EditHumanLevelDiffer: TSpinEditEx;
    GroupBox33: TGroupBox;
    Label68: TLabel;
    seHumanMaxGold: TSpinEditEx;
    seHumanTryModeMaxGold: TSpinEditEx;
    Label69: TLabel;
    GroupBox34: TGroupBox;
    Label70: TLabel;
    seTryModeLevel: TSpinEditEx;
    CheckBoxTryModeUseStorage: TCheckBox;
    GroupBox35: TGroupBox;
    CheckBoxShowMakeItemMsg: TCheckBox;
    CbViewHack: TCheckBox;
    CkViewAdmfail: TCheckBox;
    TabSheet5: TTabSheet;
    GroupBox36: TGroupBox;
    Label71: TLabel;
    EditSayMsgMaxLen: TSpinEditEx;
    Label72: TLabel;
    EditSayRedMsgMaxLen: TSpinEditEx;
    GroupBox37: TGroupBox;
    Label73: TLabel;
    EditCanShoutMsgLevel: TSpinEditEx;
    GroupBox38: TGroupBox;
    Label75: TLabel;
    CheckBoxShutRedMsgShowGMName: TCheckBox;
    EditGMRedMsgCmd: TEdit;
    ButtonMsgSave: TButton;
    TabSheet6: TTabSheet;
    GroupBox39: TGroupBox;
    Label74: TLabel;
    EditStartCastleWarDays: TSpinEditEx;
    GroupBox40: TGroupBox;
    Label76: TLabel;
    EditStartCastlewarTime: TSpinEditEx;
    Label77: TLabel;
    Label78: TLabel;
    GroupBox41: TGroupBox;
    Label79: TLabel;
    EditShowCastleWarEndMsgTime: TSpinEditEx;
    Label80: TLabel;
    GroupBox42: TGroupBox;
    Label81: TLabel;
    Label82: TLabel;
    EditCastleWarTime: TSpinEditEx;
    GroupBox43: TGroupBox;
    Label83: TLabel;
    Label84: TLabel;
    EditGetCastleTime: TSpinEditEx;
    GroupBox44: TGroupBox;
    Label85: TLabel;
    Label86: TLabel;
    seSaveHumanRcdTime: TSpinEditEx;
    GroupBox46: TGroupBox;
    Label89: TLabel;
    seMakeMonGhostTime: TSpinEditEx;
    Label91: TLabel;
    seClearDropOnFloorItemTime: TSpinEditEx;
    GroupBox47: TGroupBox;
    Label93: TLabel;
    Label94: TLabel;
    seFloorItemCanPickUpTime: TSpinEditEx;
    ButtonTimeSave: TButton;
    TabSheet7: TTabSheet;
    GroupBox48: TGroupBox;
    Label95: TLabel;
    EditBuildGuildPrice: TSpinEditEx;
    GroupBox49: TGroupBox;
    Label96: TLabel;
    EditGuildWarPrice: TSpinEditEx;
    GroupBox50: TGroupBox;
    Label97: TLabel;
    EditMakeDurgPrice: TSpinEditEx;
    ButtonPriceSave: TButton;
    GroupBox51: TGroupBox;
    Label98: TLabel;
    EditSendOnlineCountRate: TSpinEditEx;
    Label99: TLabel;
    EditSendOnlineTime: TSpinEditEx;
    CheckBoxSendOnlineCount: TCheckBox;
    Label100: TLabel;
    GroupBox52: TGroupBox;
    Label101: TLabel;
    Label102: TLabel;
    Label103: TLabel;
    EditMonsterPowerRate: TSpinEditEx;
    EditEditItemsPowerRate: TSpinEditEx;
    EditItemsACPowerRate: TSpinEditEx;
    GroupBox53: TGroupBox;
    Label20: TLabel;
    Label104: TLabel;
    seTryDealTime: TSpinEditEx;
    seDealOKTime: TSpinEditEx;
    Label105: TLabel;
    Label106: TLabel;
    GroupBox54: TGroupBox;
    Label107: TLabel;
    EditCastleMemberPriceRate: TSpinEditEx;
    TabSheet8: TTabSheet;
    ButtonMsgColorSave: TButton;
    GroupBox55: TGroupBox;
    Label108: TLabel;
    Label109: TLabel;
    EditHearMsgFColor: TColorIndexEdit;
    EdittHearMsgBColor: TColorIndexEdit;
    GroupBox56: TGroupBox;
    Label110: TLabel;
    Label111: TLabel;
    EditWhisperMsgFColor: TColorIndexEdit;
    EditWhisperMsgBColor: TColorIndexEdit;
    GroupBox57: TGroupBox;
    Label112: TLabel;
    Label113: TLabel;
    EditGMWhisperMsgFColor: TColorIndexEdit;
    EditGMWhisperMsgBColor: TColorIndexEdit;
    GroupBox58: TGroupBox;
    Label116: TLabel;
    Label117: TLabel;
    EditRedMsgFColor: TColorIndexEdit;
    EditRedMsgBColor: TColorIndexEdit;
    GroupBox59: TGroupBox;
    Label120: TLabel;
    Label121: TLabel;
    EditGreenMsgFColor: TColorIndexEdit;
    EditGreenMsgBColor: TColorIndexEdit;
    GroupBox60: TGroupBox;
    Label124: TLabel;
    Label125: TLabel;
    EditBlueMsgFColor: TColorIndexEdit;
    EditBlueMsgBColor: TColorIndexEdit;
    GroupBox61: TGroupBox;
    Label128: TLabel;
    Label129: TLabel;
    EditCryMsgFColor: TColorIndexEdit;
    EditCryMsgBColor: TColorIndexEdit;
    GroupBox62: TGroupBox;
    Label132: TLabel;
    Label133: TLabel;
    EditGuildMsgFColor: TColorIndexEdit;
    EditGuildMsgBColor: TColorIndexEdit;
    GroupBox63: TGroupBox;
    Label136: TLabel;
    Label137: TLabel;
    EditGroupMsgFColor: TColorIndexEdit;
    EditGroupMsgBColor: TColorIndexEdit;
    CheckBoxPKLevelProtect: TCheckBox;
    Label114: TLabel;
    EditPKProtectLevel: TSpinEditEx;
    Label115: TLabel;
    EditRedPKProtectLevel: TSpinEditEx;
    GroupBox19: TGroupBox;
    Label41: TLabel;
    seGroupMembersMax: TSpinEditEx;
    chkCanNotGetBackDeal: TCheckBox;
    chkDisableDeal: TCheckBox;
    GroupBox64: TGroupBox;
    Label118: TLabel;
    seCanDropPrice: TSpinEditEx;
    chkControlDropItem: TCheckBox;
    Label119: TLabel;
    seCanDropGold: TSpinEditEx;
    chkIsSafeDisableDrop: TCheckBox;
    GroupBox65: TGroupBox;
    Label122: TLabel;
    Label123: TLabel;
    EditCustMsgFColor: TColorIndexEdit;
    EditCustMsgBColor: TColorIndexEdit;
    GroupBox66: TGroupBox;
    Label126: TLabel;
    EditSuperRepairPriceRate: TSpinEditEx;
    Label127: TLabel;
    EditRepairItemDecDura: TSpinEditEx;
    TabSheet9: TTabSheet;
    chkGMRunAll: TCheckBox;
    GroupBox68: TGroupBox;
    Label135: TLabel;
    Label138: TLabel;
    EditSayMsgTime: TSpinEditEx;
    EditSayMsgCount: TSpinEditEx;
    Label139: TLabel;
    EditDisableSayMsgTime: TSpinEditEx;
    Label140: TLabel;
    Label141: TLabel;
    GroupBox70: TGroupBox;
    Label143: TLabel;
    Label144: TLabel;
    EditGuildWarTime: TSpinEditEx;
    GroupBox71: TGroupBox;
    CheckBoxShowPreFixMsg: TCheckBox;
    CheckBoxShowExceptionMsg: TCheckBox;
    TabSheet10: TTabSheet;
    ButtonCharStatusSave: TButton;
    GroupBoxParaly: TGroupBox;
    CheckBoxParalyCanRun: TCheckBox;
    CheckBoxParalyCanWalk: TCheckBox;
    CheckBoxParalyCanHit: TCheckBox;
    CheckBoxParalyCanSpell: TCheckBox;
    GroupBox73: TGroupBox;
    CheckBoxCanOldClientLogon: TCheckBox;
    chkRunGuard: TCheckBox;
    chkSafeArea: TCheckBox;
    GroupBox74: TGroupBox;
    CheckBoxFixExp: TCheckBox;
    Label15: TLabel;
    Label145: TLabel;
    SpinEditBaseExp: TSpinEditEx;
    SpinEditAddExp: TSpinEditEx;
    CheckBoxHighLevelGroupFixExp: TCheckBox;
    RadioGroupMaxLevel: TRadioGroup;
    GroupBox75: TGroupBox;
    Label147: TLabel;
    Label148: TLabel;
    EditUserSayMsgFColor: TColorIndexEdit;
    EditUserSayMsgBColor: TColorIndexEdit;
    GroupBox76: TGroupBox;
    Label151: TLabel;
    Label152: TLabel;
    EditTopUserSayMsgFColor: TColorIndexEdit;
    EditTopUserSayMsgBColor: TColorIndexEdit;
    GroupBox77: TGroupBox;
    Label149: TLabel;
    Label150: TLabel;
    EditHighLevel: TSpinEditEx;
    EditHighLevelGetExp: TSpinEditEx;
    CheckBoxLimitChangeExp: TCheckBox;
    PageControlGameSpeed: TPageControl;
    TabSheet11: TTabSheet;
    TabSheet12: TTabSheet;
    GroupBox1: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Label5: TLabel;
    Label6: TLabel;
    EditHitIntervalTime: TSpinEditEx;
    EditMagicHitIntervalTime: TSpinEditEx;
    EditRunIntervalTime: TSpinEditEx;
    EditWalkIntervalTime: TSpinEditEx;
    EditTurnIntervalTime: TSpinEditEx;
    EditDigUpIntervalTime: TSpinEditEx;
    GroupBox2: TGroupBox;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    Label10: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    EditMaxHitMsgCount: TSpinEditEx;
    EditMaxSpellMsgCount: TSpinEditEx;
    EditMaxRunMsgCount: TSpinEditEx;
    EditMaxWalkMsgCount: TSpinEditEx;
    EditMaxTurnMsgCount: TSpinEditEx;
    EditMaxDigUpMsgCount: TSpinEditEx;
    GroupBox15: TGroupBox;
    Label38: TLabel;
    Label142: TLabel;
    CheckBoxboKickOverSpeed: TCheckBox;
    EditDropOverSpeed: TSpinEditEx;
    CheckBoxSpellSendUpdateMsg: TCheckBox;
    CheckBoxActionSendActionMsg: TCheckBox;
    GroupBox7: TGroupBox;
    Label22: TLabel;
    EditStruckTime: TSpinEditEx;
    CheckBoxDisableStruck: TCheckBox;
    CheckBoxDisableSelfStruck: TCheckBox;
    ButtonGameSpeedDefault: TButton;
    ButtonGameSpeedSave: TButton;
    ButtonActionSpeedConfig: TButton;
    GroupBox4: TGroupBox;
    RadioButtonDelyMode: TRadioButton;
    RadioButtonFilterMode: TRadioButton;
    GroupBox78: TGroupBox;
    Label153: TLabel;
    Label154: TLabel;
    Label155: TLabel;
    Label157: TLabel;
    CheckBoxCheckActionCount: TCheckBox;
    EditHitCountIntervalTime: TSpinEditEx;
    EditCanHitCount: TSpinEditEx;
    EditMagicHitCountIntervalTime: TSpinEditEx;
    EditCanMagicHitCount: TSpinEditEx;
    EditMoveCountIntervalTime: TSpinEditEx;
    EditCanMoveCount: TSpinEditEx;
    ButtonCheckActionSave: TButton;
    ButtonCheckActionDefault: TButton;
    Label159: TLabel;
    EditCheckHitCount: TSpinEditEx;
    Label160: TLabel;
    Label156: TLabel;
    EditCheckMagicHitCount: TSpinEditEx;
    Label158: TLabel;
    Label161: TLabel;
    EditCheckMoveCount: TSpinEditEx;
    Label162: TLabel;
    GroupBox79: TGroupBox;
    Label163: TLabel;
    Label164: TLabel;
    Label165: TLabel;
    Label166: TLabel;
    seTryChallengeTime: TSpinEditEx;
    seChallengeOKTime: TSpinEditEx;
    chkCanNotGetBackChallenge: TCheckBox;
    chkDisableChallenge: TCheckBox;
    GroupBox80: TGroupBox;
    CheckBoxShowWhisperLevelMsg: TCheckBox;
    EditShowWhisperLevelMsg: TEdit;
    Label167: TLabel;
    PageControl1: TPageControl;
    TabSheet13: TTabSheet;
    TabSheet14: TTabSheet;
    GroupBox67: TGroupBox;
    CheckBoxKillByMonstDropUseItem: TCheckBox;
    CheckBoxKillByHumanDropUseItem: TCheckBox;
    CheckBoxDieScatterBag: TCheckBox;
    CheckBoxDieDropGold: TCheckBox;
    CheckBoxDieRedScatterBagAll: TCheckBox;
    GroupBox69: TGroupBox;
    Label130: TLabel;
    Label131: TLabel;
    Label134: TLabel;
    ScrollBarDieDropUseItemRate: TScrollBar;
    EditDieDropUseItemRate: TEdit;
    ScrollBarDieRedDropUseItemRate: TScrollBar;
    EditDieRedDropUseItemRate: TEdit;
    ScrollBarDieScatterBagRate: TScrollBar;
    EditDieScatterBagRate: TEdit;
    ButtonHumanDieSave: TButton;
    GroupBoxDieDropUseItemRate: TGroupBox;
    CheckBoxDropUseItem: TCheckBox;
    Label170: TLabel;
    ScrollBarDieDropUseItemRate0: TScrollBar;
    edtDieDropUseItemRate0: TEdit;
    Label171: TLabel;
    ScrollBarDieDropUseItemRate1: TScrollBar;
    edtDieDropUseItemRate1: TEdit;
    Label172: TLabel;
    ScrollBarDieDropUseItemRate2: TScrollBar;
    edtDieDropUseItemRate2: TEdit;
    Label173: TLabel;
    ScrollBarDieDropUseItemRate3: TScrollBar;
    edtDieDropUseItemRate3: TEdit;
    Label174: TLabel;
    ScrollBarDieDropUseItemRate4: TScrollBar;
    edtDieDropUseItemRate4: TEdit;
    Label175: TLabel;
    ScrollBarDieDropUseItemRate5: TScrollBar;
    edtDieDropUseItemRate5: TEdit;
    Label176: TLabel;
    ScrollBarDieDropUseItemRate6: TScrollBar;
    edtDieDropUseItemRate6: TEdit;
    Label177: TLabel;
    ScrollBarDieDropUseItemRate13: TScrollBar;
    edtDieDropUseItemRate13: TEdit;
    Label178: TLabel;
    ScrollBarDieDropUseItemRate12: TScrollBar;
    edtDieDropUseItemRate12: TEdit;
    edtDieDropUseItemRate11: TEdit;
    edtDieDropUseItemRate10: TEdit;
    edtDieDropUseItemRate9: TEdit;
    edtDieDropUseItemRate8: TEdit;
    edtDieDropUseItemRate7: TEdit;
    ScrollBarDieDropUseItemRate7: TScrollBar;
    ScrollBarDieDropUseItemRate8: TScrollBar;
    ScrollBarDieDropUseItemRate9: TScrollBar;
    ScrollBarDieDropUseItemRate10: TScrollBar;
    ScrollBarDieDropUseItemRate11: TScrollBar;
    Label179: TLabel;
    Label180: TLabel;
    Label181: TLabel;
    Label182: TLabel;
    Label183: TLabel;
    Label184: TLabel;
    Label185: TLabel;
    EditDieRedDropUseItemOneRate: TSpinEditEx;
    ButtonDieDropUseItemSave: TButton;
    GroupBox83: TGroupBox;
    CheckBoxSendUpdateMsg: TCheckBox;
    Label186: TLabel;
    EditMaxHitDeliveryTime: TSpinEditEx;
    Label187: TLabel;
    Label188: TLabel;
    EditMaxMagicHitDeliveryTime: TSpinEditEx;
    Label189: TLabel;
    Label190: TLabel;
    EditMaxRunDeliveryTime: TSpinEditEx;
    Label191: TLabel;
    Label192: TLabel;
    EditMaxWalkDeliveryTime: TSpinEditEx;
    Label193: TLabel;
    Label194: TLabel;
    EditMaxTurnDeliveryTime: TSpinEditEx;
    Label195: TLabel;
    Label196: TLabel;
    EditMaxDigUpDeliveryTime: TSpinEditEx;
    Label197: TLabel;
    GroupBox84: TGroupBox;
    Label198: TLabel;
    Label199: TLabel;
    EditDropItemFColor: TColorIndexEdit;
    EditDropItemBColor: TColorIndexEdit;
    GroupBox72: TGroupBox;
    Label200: TLabel;
    EditUserItemSayMsgTime: TSpinEditEx;
    Label201: TLabel;
    CheckGroupAttatckMode: TRzCheckGroup;
    GroupBox85: TGroupBox;
    Label202: TLabel;
    Label203: TLabel;
    EditNationMsgFColor: TColorIndexEdit;
    EditNationMsgBColor: TColorIndexEdit;
    EditScatterBagItemsMinLevel: TSpinEditEx;
    Label204: TLabel;
    Label205: TLabel;
    EditDropUseItemsMaxCount: TSpinEditEx;
    Label206: TLabel;
    GroupBox87: TGroupBox;
    GridLevelExpRate: TStringGrid;
    GroupBox88: TGroupBox;
    EditMaxUpLevelCount: TSpinEditEx;
    Label209: TLabel;
    Label210: TLabel;
    grp1: TGroupBox;
    lbl1: TLabel;
    lbl2: TLabel;
    seNPCLabelMouseMoveColor: TColorIndexEdit;
    seNPCLabelMouseDownColor: TColorIndexEdit;
    grp2: TGroupBox;
    lbl3: TLabel;
    seGuildMemberMaxLimit: TSpinEditEx;
    lbl4: TLabel;
    seMakeGhostTime: TSpinEditEx;
    Label87: TLabel;
    Label88: TLabel;
    seHumanFreeDelayTime: TSpinEditEx;
    lbl6: TLabel;
    seGetDBSockMsgTime: TSpinEditEx;
    lbl7: TLabel;
    chkOffLineShop: TCheckBox;
    chkOffLineHero: TCheckBox;
    chkOffLineSlave: TCheckBox;
    chkWarHreoRun: TCheckBox;
    chkSafeAreaDisNpcRun: TCheckBox;
    chkSafeAreaDisShopStallHumRun: TCheckBox;
    chkSafeAreaDisOffLineHumRun: TCheckBox;
    chkWarDisTeleport: TCheckBox;
    lbl8: TLabel;
    seChallengeTime: TSpinEditEx;
    lbl9: TLabel;
    rgChallengeGold: TRadioGroup;
    grp3: TGroupBox;
    Label57: TLabel;
    Label59: TLabel;
    Label146: TLabel;
    Label168: TLabel;
    Label169: TLabel;
    Label207: TLabel;
    seDearRecallTime: TSpinEditEx;
    seMasterRecallTime: TSpinEditEx;
    seGroupRecallTime: TSpinEditEx;
    chkRecordPrivateMsg: TCheckBox;
    GroupBox26: TGroupBox;
    chkShareExpGroupSameScreen: TCheckBox;
    chkShareExpGroupSameMap: TCheckBox;
    chkShareExpHeroSameMap: TCheckBox;
    GroupBox27: TGroupBox;
    Label211: TLabel;
    seKillHumanWeaponUnlockRate: TSpinEditEx;
    Label212: TLabel;
    seGuildNameLen: TSpinEditEx;
    Label213: TLabel;
    seGuildRankNameLen: TSpinEditEx;
    grp4: TGroupBox;
    chkHintSafeZone: TCheckBox;
    Label215: TLabel;
    seHintSafeZoneY: TSpinEditEx;
    seHintSafeZoneFColor: TColorIndexEdit;
    seHintSafeZoneBColor: TColorIndexEdit;
    seHintSafeZoneFSize: TSpinEditEx;
    Label216: TLabel;
    Label217: TLabel;
    Label218: TLabel;
    grp5: TGroupBox;
    chkHorseRun3Grid: TCheckBox;
    GroupBox45: TGroupBox;
    Label214: TLabel;
    Label219: TLabel;
    seHorseTakeTime: TSpinEditEx;
    lbl10: TLabel;
    lbl11: TLabel;
    lbl12: TLabel;
    lbl13: TLabel;
    ScrollBarDieDropUseItemRate15: TScrollBar;
    edtDieDropUseItemRate15: TEdit;
    ScrollBarDieDropUseItemRate16: TScrollBar;
    edtDieDropUseItemRate16: TEdit;
    ScrollBarDieDropUseItemRate18: TScrollBar;
    edtDieDropUseItemRate18: TEdit;
    ScrollBarDieDropUseItemRate14: TScrollBar;
    edtDieDropUseItemRate14: TEdit;
    lbl14: TLabel;
    ScrollBarDieDropUseItemRate17: TScrollBar;
    edtDieDropUseItemRate17: TEdit;
    GroupBox81: TGroupBox;
    Label220: TLabel;
    seNpcButtonClickTime: TSpinEditEx;
    Label222: TLabel;
    seMakeDummyGhostTime: TSpinEditEx;
    lbl5: TLabel;
    seDummyAddPKPoint: TSpinEditEx;
    chkSpeedControl: TCheckBox;
    EditOverSpeedKickCount: TSpinEditEx;
    Label90: TLabel;
    seNpcActorClickTime: TSpinEditEx;
    lbl15: TLabel;
    Label92: TLabel;
    Label221: TLabel;
    seTakeOnHorseUseTime: TSpinEdit;
    chkReadyOnHorseDisableAction: TCheckBox;
    chkKillByHumanDropJewelryBoxItem: TCheckBox;
    chkKillByMonstDropJewelryBoxItem: TCheckBox;
    chkKillByHumanDropGodBlessItem: TCheckBox;
    chkKillByMonstDropGodBlessItem: TCheckBox;
    Label223: TLabel;
    scrlbrJewelryBoxItem: TScrollBar;
    edtJewelryBoxItem: TEdit;
    Label224: TLabel;
    scrlbrGodBlessItem: TScrollBar;
    edtGodBlessItem: TEdit;
    rgMaxAC: TRadioGroup;
    chkMagicshieldStruck: TCheckBox;
    chkRecordPublicMsg: TCheckBox;
    chkRecordGuildMsg: TCheckBox;
    chkRecordCryCryMsg: TCheckBox;
    chkRecordGroupMsg: TCheckBox;
    chkHeroKillHumanNotWeaponUnlock: TCheckBox;
    GroupBox82: TGroupBox;
    chkNationGroupCheck: TCheckBox;
    chkNationGuildCheck: TCheckBox;
    Label225: TLabel;
    seNationSayLevel: TSpinEditEx;
    chkRecordNationMsg: TCheckBox;
    grp6: TGroupBox;
    lbl16: TLabel;
    seMaxInputStringLen: TSpinEditEx;
    GroupBox3: TGroupBox;
    Label13: TLabel;
    seHumChgMapOrLoginProtectTime: TSpinEditEx;
    lbl17: TLabel;
    grp7: TGroupBox;
    chkSellItemToNpcShopNoCalcAddProperty: TCheckBox;
    chkShowNewValueFromBuyNpcItem: TCheckBox;
    seKillHeroAddPKPoint: TSpinEditEx;
    Label226: TLabel;
    Label227: TLabel;
    seNPCLabelNormalColor: TColorIndexEdit;
    chkNPCLabelFontStroke: TCheckBox;
    grp8: TGroupBox;
    Label228: TLabel;
    sePlayerVarJClearTime: TSpinEditEx;
    lbl18: TLabel;
    rgMaxHitPoint: TRadioGroup;
    Label60: TLabel;
    seStartPermission: TSpinEditEx;
    GroupBox30: TGroupBox;
    Label229: TLabel;
    Label230: TLabel;
    seSendWhisperMsgFColor: TColorIndexEdit;
    seSendWhisperMsgBColor: TColorIndexEdit;
    GroupBox86: TGroupBox;
    Label231: TLabel;
    Label232: TLabel;
    seRefreshGameGoldFColor: TColorIndexEdit;
    seRefreshGameGoldBColor: TColorIndexEdit;
    GroupBox89: TGroupBox;
    Label233: TLabel;
    Label234: TLabel;
    seShowWhisperFColor: TColorIndexEdit;
    seShowWhisperBColor: TColorIndexEdit;
    GroupBox90: TGroupBox;
    Label235: TLabel;
    Label236: TLabel;
    seCloseWhisperFColor: TColorIndexEdit;
    seCloseWhisperBColor: TColorIndexEdit;
    chkPermissionChangeLog: TCheckBox;
    chkOldClient: TCheckBox;
    procedure EditHitIntervalTimeChange(Sender: TObject);
    procedure EditMagicHitIntervalTimeChange(Sender: TObject);
    procedure EditRunIntervalTimeChange(Sender: TObject);
    procedure EditWalkIntervalTimeChange(Sender: TObject);
    procedure EditTurnIntervalTimeChange(Sender: TObject);
    procedure EditMaxHitMsgCountChange(Sender: TObject);
    procedure EditMaxSpellMsgCountChange(Sender: TObject);
    procedure EditMaxRunMsgCountChange(Sender: TObject);
    procedure EditMaxWalkMsgCountChange(Sender: TObject);
    procedure EditMaxTurnMsgCountChange(Sender: TObject);
    procedure EditMaxDigUpMsgCountChange(Sender: TObject);
    procedure ButtonGameSpeedSaveClick(Sender: TObject);
    procedure GameConfigControlChanging(Sender: TObject; var AllowChange: Boolean);
    procedure FormCreate(Sender: TObject);
    procedure EditConsoleShowUserCountTimeChange(Sender: TObject);
    procedure EditShowLineNoticeTimeChange(Sender: TObject);
    procedure ComboBoxLineNoticeColorChange(Sender: TObject);
    procedure EditSoftVersionDateChange(Sender: TObject);
    procedure ButtonGeneralSaveClick(Sender: TObject);
    procedure EditLineNoticePreFixChange(Sender: TObject);
    procedure CheckBoxDisableStruckClick(Sender: TObject);
    procedure EditStruckTimeChange(Sender: TObject);
    procedure EditKillMonExpMultipleChange(Sender: TObject);
    procedure CheckBoxHighLevelKillMonFixExpClick(Sender: TObject);
    procedure ButtonExpSaveClick(Sender: TObject);
    procedure EditRepairDoorPriceChange(Sender: TObject);
    procedure EditRepairWallPriceChange(Sender: TObject);
    procedure EditHireArcherPriceChange(Sender: TObject);
    procedure EditHireGuardPriceChange(Sender: TObject);
    procedure EditCastleGoldMaxChange(Sender: TObject);
    procedure EditCastleOneDayGoldChange(Sender: TObject);
    procedure EditCastleHomeMapChange(Sender: TObject);
    procedure EditCastleHomeXChange(Sender: TObject);
    procedure EditCastleHomeYChange(Sender: TObject);
    procedure EditCastleNameChange(Sender: TObject);
    procedure EditWarRangeXChange(Sender: TObject);
    procedure EditWarRangeYChange(Sender: TObject);
    procedure CheckBoxGetAllNpcTaxClick(Sender: TObject);
    procedure EditTaxRateChange(Sender: TObject);
    procedure ButtonCastleSaveClick(Sender: TObject);
    procedure GridLevelExpSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
    procedure ComboBoxLevelExpClick(Sender: TObject);
    procedure EditOverSpeedKickCountChange(Sender: TObject);
    procedure CheckBoxboKickOverSpeedClick(Sender: TObject);
    procedure chkDisHumRunClick(Sender: TObject);
    procedure ButtonOptionSaveClick(Sender: TObject);
    procedure chkRunHumClick(Sender: TObject);
    procedure chkRunMonClick(Sender: TObject);
    procedure chkWarDisHumRunClick(Sender: TObject);
    procedure chkRunNpcClick(Sender: TObject);
    procedure EditSafeZoneSizeChange(Sender: TObject);
    procedure EditStartPointSizeChange(Sender: TObject);
    procedure seGroupMembersMaxChange(Sender: TObject);
    procedure EditRedHomeXChange(Sender: TObject);
    procedure EditRedHomeYChange(Sender: TObject);
    procedure EditRedHomeMapChange(Sender: TObject);
    procedure EditRedDieHomeMapChange(Sender: TObject);
    procedure EditRedDieHomeXChange(Sender: TObject);
    procedure EditRedDieHomeYChange(Sender: TObject);
    procedure ButtonOptionSave3Click(Sender: TObject);
    procedure EditHomeMapChange(Sender: TObject);
    procedure EditHomeXChange(Sender: TObject);
    procedure EditHomeYChange(Sender: TObject);
    procedure EditDecPkPointTimeChange(Sender: TObject);
    procedure EditDecPkPointCountChange(Sender: TObject);
    procedure EditPKFlagTimeChange(Sender: TObject);
    procedure seHumanAddPKPointChange(Sender: TObject);
    procedure ButtonOptionSave2Click(Sender: TObject);
    procedure CheckBoxTestServerClick(Sender: TObject);
    procedure CheckBoxServiceModeClick(Sender: TObject);
    procedure CheckBoxVentureModeClick(Sender: TObject);
    procedure CheckBoxNonPKModeClick(Sender: TObject);
    procedure seTestLevelChange(Sender: TObject);
    procedure seTestGoldChange(Sender: TObject);
    procedure seTestUserLimitChange(Sender: TObject);
    procedure seStartPermissionChange(Sender: TObject);
    procedure seUserFullChange(Sender: TObject);
    procedure ButtonOptionSave0Click(Sender: TObject);
    procedure CheckBoxKillHumanWinLevelClick(Sender: TObject);
    procedure CheckBoxKilledLostLevelClick(Sender: TObject);
    procedure CheckBoxKillHumanWinExpClick(Sender: TObject);
    procedure CheckBoxKilledLostExpClick(Sender: TObject);
    procedure EditKillHumanWinLevelChange(Sender: TObject);
    procedure EditKilledLostLevelChange(Sender: TObject);
    procedure EditKillHumanWinExpChange(Sender: TObject);
    procedure EditKillHumanLostExpChange(Sender: TObject);
    procedure EditHumanLevelDifferChange(Sender: TObject);
    procedure seHumanMaxGoldChange(Sender: TObject);
    procedure seHumanTryModeMaxGoldChange(Sender: TObject);
    procedure seTryModeLevelChange(Sender: TObject);
    procedure CheckBoxTryModeUseStorageClick(Sender: TObject);
    procedure CheckBoxShowMakeItemMsgClick(Sender: TObject);
    procedure CbViewHackClick(Sender: TObject);
    procedure CkViewAdmfailClick(Sender: TObject);
    procedure EditSayMsgMaxLenChange(Sender: TObject);
    procedure EditSayRedMsgMaxLenChange(Sender: TObject);
    procedure EditCanShoutMsgLevelChange(Sender: TObject);
    procedure CheckBoxShutRedMsgShowGMNameClick(Sender: TObject);
    procedure EditGMRedMsgCmdChange(Sender: TObject);
    procedure ButtonMsgSaveClick(Sender: TObject);
    procedure EditStartCastleWarDaysChange(Sender: TObject);
    procedure EditStartCastlewarTimeChange(Sender: TObject);
    procedure EditShowCastleWarEndMsgTimeChange(Sender: TObject);
    procedure EditCastleWarTimeChange(Sender: TObject);
    procedure EditGetCastleTimeChange(Sender: TObject);
    procedure seMakeMonGhostTimeChange(Sender: TObject);
    procedure seClearDropOnFloorItemTimeChange(Sender: TObject);
    procedure seSaveHumanRcdTimeChange(Sender: TObject);
    procedure seHumanFreeDelayTimeChange(Sender: TObject);
    procedure seFloorItemCanPickUpTimeChange(Sender: TObject);
    procedure ButtonTimeSaveClick(Sender: TObject);
    procedure EditBuildGuildPriceChange(Sender: TObject);
    procedure EditGuildWarPriceChange(Sender: TObject);
    procedure EditMakeDurgPriceChange(Sender: TObject);
    procedure ButtonPriceSaveClick(Sender: TObject);
    procedure CheckBoxSendOnlineCountClick(Sender: TObject);
    procedure EditSendOnlineCountRateChange(Sender: TObject);
    procedure EditSendOnlineTimeChange(Sender: TObject);
    procedure EditMonsterPowerRateChange(Sender: TObject);
    procedure EditEditItemsPowerRateChange(Sender: TObject);
    procedure EditItemsACPowerRateChange(Sender: TObject);
    procedure seTryDealTimeChange(Sender: TObject);
    procedure seDealOKTimeChange(Sender: TObject);
    procedure EditCastleMemberPriceRateChange(Sender: TObject);
    procedure EditHearMsgFColorChange(Sender: TObject);
    procedure EdittHearMsgBColorChange(Sender: TObject);
    procedure EditWhisperMsgFColorChange(Sender: TObject);
    procedure EditWhisperMsgBColorChange(Sender: TObject);
    procedure EditGMWhisperMsgFColorChange(Sender: TObject);
    procedure EditGMWhisperMsgBColorChange(Sender: TObject);
    procedure EditRedMsgFColorChange(Sender: TObject);
    procedure EditRedMsgBColorChange(Sender: TObject);
    procedure EditGreenMsgFColorChange(Sender: TObject);
    procedure EditGreenMsgBColorChange(Sender: TObject);
    procedure EditBlueMsgFColorChange(Sender: TObject);
    procedure EditBlueMsgBColorChange(Sender: TObject);
    procedure EditCryMsgFColorChange(Sender: TObject);
    procedure EditCryMsgBColorChange(Sender: TObject);
    procedure EditGuildMsgFColorChange(Sender: TObject);
    procedure EditGuildMsgBColorChange(Sender: TObject);
    procedure EditGroupMsgFColorChange(Sender: TObject);
    procedure EditGroupMsgBColorChange(Sender: TObject);
    procedure ButtonMsgColorSaveClick(Sender: TObject);
    procedure CheckBoxPKLevelProtectClick(Sender: TObject);
    procedure EditPKProtectLevelChange(Sender: TObject);
    procedure EditRedPKProtectLevelChange(Sender: TObject);
    procedure CheckBoxDisableSelfStruckClick(Sender: TObject);
    procedure chkCanNotGetBackDealClick(Sender: TObject);
    procedure chkDisableDealClick(Sender: TObject);
    procedure chkControlDropItemClick(Sender: TObject);
    procedure seCanDropPriceChange(Sender: TObject);
    procedure seCanDropGoldChange(Sender: TObject);
    procedure chkIsSafeDisableDropClick(Sender: TObject);
    procedure EditCustMsgFColorChange(Sender: TObject);
    procedure EditCustMsgBColorChange(Sender: TObject);
    procedure EditSuperRepairPriceRateChange(Sender: TObject);
    procedure EditRepairItemDecDuraChange(Sender: TObject);
    procedure ButtonHumanDieSaveClick(Sender: TObject);
    procedure ScrollBarDieDropUseItemRateChange(Sender: TObject);
    procedure ScrollBarDieRedDropUseItemRateChange(Sender: TObject);
    procedure ScrollBarDieScatterBagRateChange(Sender: TObject);
    procedure CheckBoxKillByMonstDropUseItemClick(Sender: TObject);
    procedure CheckBoxKillByHumanDropUseItemClick(Sender: TObject);
    procedure CheckBoxDieScatterBagClick(Sender: TObject);
    procedure CheckBoxDieDropGoldClick(Sender: TObject);
    procedure CheckBoxDieRedScatterBagAllClick(Sender: TObject);
    procedure chkGMRunAllClick(Sender: TObject);
    procedure EditSayMsgTimeChange(Sender: TObject);
    procedure EditSayMsgCountChange(Sender: TObject);
    procedure EditDisableSayMsgTimeChange(Sender: TObject);
    procedure EditDropOverSpeedChange(Sender: TObject);
    procedure EditGuildWarTimeChange(Sender: TObject);
    procedure CheckBoxShowPreFixMsgClick(Sender: TObject);
    procedure CheckBoxShowExceptionMsgClick(Sender: TObject);
    procedure CheckBoxParalyCanRunClick(Sender: TObject);
    procedure CheckBoxParalyCanWalkClick(Sender: TObject);
    procedure CheckBoxParalyCanHitClick(Sender: TObject);
    procedure CheckBoxParalyCanSpellClick(Sender: TObject);
    procedure ButtonCharStatusSaveClick(Sender: TObject);
    procedure ButtonGameSpeedDefaultClick(Sender: TObject);
    procedure CheckBoxCanOldClientLogonClick(Sender: TObject);
    procedure CheckBoxSpellSendUpdateMsgClick(Sender: TObject);
    procedure CheckBoxActionSendActionMsgClick(Sender: TObject);
    procedure RadioButtonDelyModeClick(Sender: TObject);
    procedure RadioButtonFilterModeClick(Sender: TObject);
    procedure seTestLevelKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure ButtonActionSpeedConfigClick(Sender: TObject);
    procedure chkRunGuardClick(Sender: TObject);
    procedure chkSafeAreaClick(Sender: TObject);
    procedure CheckBoxFixExpClick(Sender: TObject);
    procedure SpinEditBaseExpChange(Sender: TObject);
    procedure SpinEditAddExpChange(Sender: TObject);
    procedure CheckBoxHighLevelGroupFixExpClick(Sender: TObject);
    procedure RadioGroupMaxLevelClick(Sender: TObject);
    procedure EditUserSayMsgFColorChange(Sender: TObject);
    procedure EditUserSayMsgBColorChange(Sender: TObject);
    procedure EditTopUserSayMsgFColorChange(Sender: TObject);
    procedure EditTopUserSayMsgBColorChange(Sender: TObject);
    procedure EditHighLevelChange(Sender: TObject);
    procedure EditHighLevelGetExpChange(Sender: TObject);
    procedure CheckBoxLimitChangeExpClick(Sender: TObject);
    procedure CheckBoxCheckActionCountClick(Sender: TObject);
    procedure EditHitCountIntervalTimeChange(Sender: TObject);
    procedure EditMagicHitCountIntervalTimeChange(Sender: TObject);
    procedure EditMoveCountIntervalTimeChange(Sender: TObject);
    procedure EditCanHitCountChange(Sender: TObject);
    procedure EditCanMagicHitCountChange(Sender: TObject);
    procedure EditCanMoveCountChange(Sender: TObject);
    procedure ButtonCheckActionDefaultClick(Sender: TObject);
    procedure ButtonCheckActionSaveClick(Sender: TObject);
    procedure EditCheckHitCountChange(Sender: TObject);
    procedure EditCheckMagicHitCountChange(Sender: TObject);
    procedure EditCheckMoveCountChange(Sender: TObject);
    procedure seTryChallengeTimeChange(Sender: TObject);
    procedure seChallengeOKTimeChange(Sender: TObject);
    procedure chkCanNotGetBackChallengeClick(Sender: TObject);
    procedure chkDisableChallengeClick(Sender: TObject);
    procedure CheckBoxShowWhisperLevelMsgClick(Sender: TObject);
    procedure EditShowWhisperLevelMsgChange(Sender: TObject);
    procedure ScrollBarDieDropUseItemRate0Change(Sender: TObject);
    procedure ButtonDieDropUseItemSaveClick(Sender: TObject);
    procedure CheckBoxDropUseItemClick(Sender: TObject);
    procedure EditDieRedDropUseItemOneRateChange(Sender: TObject);
    procedure CheckBoxSendUpdateMsgClick(Sender: TObject);
    procedure EditMaxHitDeliveryTimeChange(Sender: TObject);
    procedure EditMaxMagicHitDeliveryTimeChange(Sender: TObject);
    procedure EditMaxRunDeliveryTimeChange(Sender: TObject);
    procedure EditMaxWalkDeliveryTimeChange(Sender: TObject);
    procedure EditMaxTurnDeliveryTimeChange(Sender: TObject);
    procedure EditMaxDigUpDeliveryTimeChange(Sender: TObject);
    procedure EditDigUpIntervalTimeChange(Sender: TObject);
    procedure EditDropItemFColorChange(Sender: TObject);
    procedure EditDropItemBColorChange(Sender: TObject);
    procedure EditUserItemSayMsgTimeChange(Sender: TObject);
    procedure EditNationMsgFColorChange(Sender: TObject);
    procedure EditNationMsgBColorChange(Sender: TObject);
    procedure CheckGroupAttatckModeChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
    procedure EditScatterBagItemsMinLevelChange(Sender: TObject);
    procedure EditDropUseItemsMaxCountChange(Sender: TObject);
    procedure EditMaxUpLevelCountChange(Sender: TObject);
    procedure chkOldClientClick(Sender: TObject);
    procedure seNPCLabelMouseMoveColorChange(Sender: TObject);
    procedure seNPCLabelMouseDownColorChange(Sender: TObject);
    procedure seGuildMemberMaxLimitChange(Sender: TObject);
    procedure seMakeGhostTimeChange(Sender: TObject);
    procedure seGetDBSockMsgTimeChange(Sender: TObject);
    procedure chkOffLineShopClick(Sender: TObject);
    procedure chkOffLineHeroClick(Sender: TObject);
    procedure chkOffLineSlaveClick(Sender: TObject);
    procedure chkWarHreoRunClick(Sender: TObject);
    procedure chkSafeAreaDisNpcRunClick(Sender: TObject);
    procedure chkSafeAreaDisShopStallHumRunClick(Sender: TObject);
    procedure chkSafeAreaDisOffLineHumRunClick(Sender: TObject);
    procedure chkWarDisTeleportClick(Sender: TObject);
    procedure seChallengeTimeChange(Sender: TObject);
    procedure rgChallengeGoldClick(Sender: TObject);
    procedure seDearRecallTimeChange(Sender: TObject);
    procedure seMasterRecallTimeChange(Sender: TObject);
    procedure seGroupRecallTimeChange(Sender: TObject);
    procedure chkRecordPrivateMsgClick(Sender: TObject);
    procedure chkShareExpGroupSameScreenClick(Sender: TObject);
    procedure chkShareExpGroupSameMapClick(Sender: TObject);
    procedure chkShareExpHeroSameMapClick(Sender: TObject);
    procedure seKillHumanWeaponUnlockRateChange(Sender: TObject);
    procedure seGuildRankNameLenChange(Sender: TObject);
    procedure seGuildNameLenChange(Sender: TObject);
    procedure chkHintSafeZoneClick(Sender: TObject);
    procedure seHintSafeZoneYChange(Sender: TObject);
    procedure seHintSafeZoneFColorChange(Sender: TObject);
    procedure seHintSafeZoneBColorChange(Sender: TObject);
    procedure seHintSafeZoneFSizeChange(Sender: TObject);
    procedure chkHorseRun3GridClick(Sender: TObject);
    procedure seHorseTakeTimeChange(Sender: TObject);
    procedure seNpcButtonClickTimeChange(Sender: TObject);
    procedure seMakeDummyGhostTimeChange(Sender: TObject);
    procedure seDummyAddPKPointChange(Sender: TObject);
    procedure chkSpeedControlClick(Sender: TObject);
    procedure seNpcActorClickTimeChange(Sender: TObject);
    procedure seTakeOnHorseUseTimeChange(Sender: TObject);
    procedure chkReadyOnHorseDisableActionClick(Sender: TObject);
    procedure chkKillByMonstDropJewelryBoxItemClick(Sender: TObject);
    procedure chkKillByHumanDropJewelryBoxItemClick(Sender: TObject);
    procedure chkKillByMonstDropGodBlessItemClick(Sender: TObject);
    procedure chkKillByHumanDropGodBlessItemClick(Sender: TObject);
    procedure scrlbrJewelryBoxItemChange(Sender: TObject);
    procedure scrlbrGodBlessItemChange(Sender: TObject);
    procedure rgMaxACClick(Sender: TObject);
    procedure chkMagicshieldStruckClick(Sender: TObject);
    procedure chkRecordPublicMsgClick(Sender: TObject);
    procedure chkRecordGuildMsgClick(Sender: TObject);
    procedure chkRecordCryCryMsgClick(Sender: TObject);
    procedure chkRecordGroupMsgClick(Sender: TObject);
    procedure chkHeroKillHumanNotWeaponUnlockClick(Sender: TObject);
    procedure chkNationGroupCheckClick(Sender: TObject);
    procedure chkNationGuildCheckClick(Sender: TObject);
    procedure seNationSayLevelChange(Sender: TObject);
    procedure chkRecordNationMsgClick(Sender: TObject);
    procedure seMaxInputStringLenChange(Sender: TObject);
    procedure seHumChgMapOrLoginProtectTimeChange(Sender: TObject);
    procedure chkSellItemToNpcShopNoCalcAddPropertyClick(Sender: TObject);
    procedure chkShowNewValueFromBuyNpcItemClick(Sender: TObject);
    procedure seKillHeroAddPKPointChange(Sender: TObject);
    procedure seNPCLabelNormalColorChange(Sender: TObject);
    procedure chkNPCLabelFontStrokeClick(Sender: TObject);
    procedure sePlayerVarJClearTimeChange(Sender: TObject);
    procedure rgMaxHitPointClick(Sender: TObject);
    procedure seSendWhisperMsgFColorChange(Sender: TObject);
    procedure seSendWhisperMsgBColorChange(Sender: TObject);
    procedure seRefreshGameGoldFColorChange(Sender: TObject);
    procedure seRefreshGameGoldBColorChange(Sender: TObject);
    procedure seShowWhisperFColorChange(Sender: TObject);
    procedure seShowWhisperBColorChange(Sender: TObject);
    procedure seCloseWhisperFColorChange(Sender: TObject);
    procedure seCloseWhisperBColorChange(Sender: TObject);
    procedure chkPermissionChangeLogClick(Sender: TObject);
  private
    boOpened: Boolean;
    boModValued: Boolean;
    boSendServerConfig: Boolean;
    procedure ModValue();
    procedure uModValue();
    procedure RefGameSpeedConf();
    procedure RefCharStatusConf();
    procedure RefGameVarConf();

    procedure RefGlobalSpeedCtrl;
    { Private declarations }
  public
    procedure Open();
    { Public declarations }
  end;

var
  frmGameConfig: TfrmGameConfig;

implementation

uses
  M2Share, HUtil32, ActionSpeedConfig;

{$R *.dfm}
{ TfrmGameConfig }

procedure TfrmGameConfig.FormCreate(Sender: TObject);
var
  I: Integer;
begin
  ComboBoxLineNoticeColor.Items.Add('红色');
  ComboBoxLineNoticeColor.Items.Add('绿色');
  ComboBoxLineNoticeColor.Items.Add('蓝色');
  GridLevelExp.ColWidths[0] := 30;
  GridLevelExp.ColWidths[1] := 100;
  GridLevelExp.Cells[0, 0] := '等级';
  GridLevelExp.Cells[1, 0] := '经验值';

  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    GridLevelExp.Cells[0, I] := IntToStr(I);
  end;

  GridLevelExpRate.Cells[0, 0] := '等级';
  GridLevelExpRate.Cells[1, 0] := '百分比';

  for I := 1 to GridLevelExpRate.RowCount - 1 do
  begin
    GridLevelExpRate.Cells[0, I] := '0';
    GridLevelExpRate.Cells[1, I] := '0';
  end;

  ComboBoxLevelExp.AddItem('原始经验值', TObject(s_OldLevelExp));
  ComboBoxLevelExp.AddItem('标准经验值', TObject(s_StdLevelExp));
  ComboBoxLevelExp.AddItem('当前1/2倍经验', TObject(s_2Mult));
  ComboBoxLevelExp.AddItem('当前1/5倍经验', TObject(s_5Mult));
  ComboBoxLevelExp.AddItem('当前1/8倍经验', TObject(s_8Mult));
  ComboBoxLevelExp.AddItem('当前1/10倍经验', TObject(s_10Mult));
  ComboBoxLevelExp.AddItem('当前1/20倍经验', TObject(s_20Mult));
  ComboBoxLevelExp.AddItem('当前1/30倍经验', TObject(s_30Mult));
  ComboBoxLevelExp.AddItem('当前1/40倍经验', TObject(s_40Mult));
  ComboBoxLevelExp.AddItem('当前1/50倍经验', TObject(s_50Mult));
  ComboBoxLevelExp.AddItem('当前1/60倍经验', TObject(s_60Mult));
  ComboBoxLevelExp.AddItem('当前1/70倍经验', TObject(s_70Mult));
  ComboBoxLevelExp.AddItem('当前1/80倍经验', TObject(s_80Mult));
  ComboBoxLevelExp.AddItem('当前1/90倍经验', TObject(s_90Mult));
  ComboBoxLevelExp.AddItem('当前1/100倍经验', TObject(s_100Mult));
  ComboBoxLevelExp.AddItem('当前1/150倍经验', TObject(s_150Mult));
  ComboBoxLevelExp.AddItem('当前1/200倍经验', TObject(s_200Mult));
  ComboBoxLevelExp.AddItem('当前1/250倍经验', TObject(s_250Mult));
  ComboBoxLevelExp.AddItem('当前1/300倍经验', TObject(s_300Mult));

  EditSoftVersionDate.Hint :=
    '客户端版本日期设置，此参数默认为 20020522，此版本日期必须与客户端相匹配，否则在进入游戏时将提示版本不正确';
  EditConsoleShowUserCountTime.Hint := '程序控制台上显示当前在线人数间隔时间，此参数默认为 10分钟。';
  EditShowLineNoticeTime.Hint := '游戏中显示公告信息的间隔时间，此参数默认为 300秒。';
  ComboBoxLineNoticeColor.Hint := '游戏中显示公告信息的文字颜色，此参数默认为 蓝色。';
  EditLineNoticePreFix.Hint := '游戏中显示公告信息的文字行前缀文字。';
  EditHitIntervalTime.Hint := '游戏中人物二次攻击间隔时间，此参数默认为 700毫秒。如果出现卡顿，需要调整此参数';
  EditMagicHitIntervalTime.Hint := '游戏中人物二次魔法攻击间隔时间，此参数默认为 450毫秒。如果出现卡顿，需要调整此参数';
  EditRunIntervalTime.Hint := '游戏中人物二次跑动间隔时间，此参数默认为 400毫秒。如果出现卡顿，需要调整此参数';
  EditWalkIntervalTime.Hint := '游戏中人物二次走动间隔时间，此参数默认为 400毫秒。如果出现卡顿，需要调整此参数';
  EditTurnIntervalTime.Hint := '游戏中人物二次变方向间隔时间，此参数默认为 100毫秒。如果出现卡顿，需要调整此参数';
  EditDigUpIntervalTime.Hint := '游戏中人物二次挖肉间隔时间，此参数默认为 100毫秒。如果出现卡顿，需要调整此参数';
  // EditUseItemIntervalTime.Hint := '游戏中人物二次使用物品间隔时间，此参数默认为 500毫秒。';

  EditStruckTime.Hint := '人物被攻击后弯腰停留时间控制，此参数默认为 100毫秒。';
  CheckBoxDisableStruck.Hint := '人物在被攻击后是否显示弯腰动作。';

  GridLevelExp.Hint := '修改的经验在点击保存按钮后生效。';
  ComboBoxLevelExp.Hint := '选择的经验计划，立即生效。';
  EditKillMonExpMultiple.Hint := '人物杀怪物所得经验值倍，此参数默认为 1，此经验值以怪物数据库里的经验值为基准。';
  CheckBoxHighLevelKillMonFixExp.Hint := '高等级人物杀怪经验是否保持不变，此参数默认为关闭(不打钩)。';
  EditRepairDoorPrice.Hint := '维修城门所需费用，此参数默认为 2000000金币。';
  EditRepairWallPrice.Hint := '维修城墙所需费用，此参数默认为 500000金币。';
  EditHireArcherPrice.Hint := '雇用弓箭手所需费用，此参数默认为 300000金币。';
  EditHireGuardPrice.Hint := '维修守卫所需费用，此参数默认为 300000金币。';
  EditCastleGoldMax.Hint := '城堡内最高可存金币数量，此参数默认为 10000000金币。';
  EditCastleOneDayGold.Hint := '城堡一天内最高收入上限，此参数默认为 2000000金币。';
  EditCastleHomeMap.Hint := '行会回城点默认所在地图号，此参数默认地图号为 3，以城堡配置文件中的参数为准';
  EditCastleHomeX.Hint := '行会回城点默认所在地图座标X，此参数默认座标为 644，以城堡配置文件中的参数为准';
  EditCastleHomeY.Hint := '行会回城点默认所在地图座标Y，此参数默认座标为 290，以城堡配置文件中的参数为准';
  EditCastleName.Hint := '城堡默认的名称，以城堡配置文件中的参数为准。';
  EditWarRangeX.Hint := '攻城区域默认座标X范围大小，此参数默认为 100，以城堡配置文件中的参数为准';
  EditWarRangeY.Hint := '攻城区域默认座标Y范围大小，此参数默认为 100，以城堡配置文件中的参数为准';
  CheckBoxGetAllNpcTax.Hint := '是否收取所有交易NPC的交易税，此参数默认为关闭(不打钩)。';
  EditTaxRate.Hint := '交易税率，此参为默认为 5，也就是 0.05%。';
end;

procedure TfrmGameConfig.GameConfigControlChanging(Sender: TObject; var AllowChange: Boolean);
begin
  if boModValued then
  begin
    if Application.MessageBox('参数设置已经被修改，是否确认不保存修改的设置？', '确认信息', MB_YESNO + MB_ICONQUESTION) = IDYES then
    begin
      uModValue
    end
    else
      AllowChange := False;
  end;
end;

procedure TfrmGameConfig.ModValue;
begin
  boModValued := True;
  ButtonGameSpeedSave.Enabled := True;
  ButtonGeneralSave.Enabled := True;
  ButtonExpSave.Enabled := True;
  ButtonCastleSave.Enabled := True;
  ButtonOptionSave0.Enabled := True;
  ButtonOptionSave.Enabled := True;
  ButtonOptionSave2.Enabled := True;
  ButtonOptionSave3.Enabled := True;
  ButtonTimeSave.Enabled := True;
  ButtonPriceSave.Enabled := True;
  ButtonMsgSave.Enabled := True;
  ButtonMsgColorSave.Enabled := True;
  ButtonHumanDieSave.Enabled := True;
  ButtonCharStatusSave.Enabled := True;
  ButtonCheckActionSave.Enabled := True;
  ButtonDieDropUseItemSave.Enabled := True;
end;

procedure TfrmGameConfig.uModValue;
begin
  boModValued := False;
  boSendServerConfig := False;
  ButtonGameSpeedSave.Enabled := False;
  ButtonGeneralSave.Enabled := False;
  ButtonExpSave.Enabled := False;
  ButtonCastleSave.Enabled := False;
  ButtonOptionSave0.Enabled := False;
  ButtonOptionSave.Enabled := False;
  ButtonOptionSave2.Enabled := False;
  ButtonOptionSave3.Enabled := False;
  ButtonTimeSave.Enabled := False;
  ButtonPriceSave.Enabled := False;
  ButtonMsgSave.Enabled := False;
  ButtonMsgColorSave.Enabled := False;
  ButtonHumanDieSave.Enabled := False;
  ButtonCharStatusSave.Enabled := False;
  ButtonCheckActionSave.Enabled := False;
  ButtonDieDropUseItemSave.Enabled := False;
end;

procedure TfrmGameConfig.Open;
var
  I: Integer;
begin
  boOpened := False;
  uModValue();
  RefGameSpeedConf();

  EditKillMonExpMultiple.Value := g_Config.dwKillMonExpMultiple;
  CheckBoxHighLevelKillMonFixExp.Checked := g_Config.boHighLevelKillMonFixExp;
  CheckBoxHighLevelGroupFixExp.Checked := g_Config.boHighLevelGroupFixExp;
  EditMaxUpLevelCount.Value := g_Config.nMaxUpLevelCount;

  EditRepairDoorPrice.Value := g_Config.nRepairDoorPrice;
  EditRepairWallPrice.Value := g_Config.nRepairWallPrice;
  EditHireArcherPrice.Value := g_Config.nHireArcherPrice;
  EditHireGuardPrice.Value := g_Config.nHireGuardPrice;

  EditCastleGoldMax.Value := g_Config.nCastleGoldMax;
  EditCastleOneDayGold.Value := g_Config.nCastleOneDayGold;
  EditCastleHomeMap.Text := g_Config.sCastleHomeMap;
  EditCastleHomeX.Value := g_Config.nCastleHomeX;
  EditCastleHomeY.Value := g_Config.nCastleHomeY;
  EditCastleName.Text := g_Config.sCASTLENAME;
  EditWarRangeX.Value := g_Config.nCastleWarRangeX;
  EditWarRangeY.Value := g_Config.nCastleWarRangeY;
  CheckBoxGetAllNpcTax.Checked := g_Config.boGetAllNpcTax;
  EditTaxRate.Value := g_Config.nCastleTaxRate;

  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    GridLevelExp.Cells[1, I] := IntToStr(g_Config.dwNeedExps[I]);
  end;
  GroupBoxLevelExp.Caption := Format('升级经验(最高有效等级%d)', [MAXUPLEVEL]);

  for I := 1 to GridLevelExpRate.RowCount - 1 do
  begin
    GridLevelExpRate.Cells[0, I] := IntToStr(I);
    GridLevelExpRate.Cells[1, I] := IntToStr(g_Config.LevelExpRates[I]);
  end;

  chkDisHumRun.Checked := not g_Config.boDiableHumanRun;
  chkRunHum.Checked := g_Config.boRUNHUMAN;
  chkRunMon.Checked := g_Config.boRUNMON;
  chkRunNpc.Checked := g_Config.boRunNpc;
  chkRunGuard.Checked := g_Config.boRunGuard;
  chkWarDisHumRun.Checked := g_Config.boWarDisHumRun;
  chkWarHreoRun.Checked := g_Config.boWarHreoRun;
  chkWarHreoRun.Enabled := g_Config.boWarDisHumRun;
  chkGMRunAll.Checked := g_Config.boGMRunAll;
  chkSafeArea.Checked := g_Config.boSafeAreaLimited;
  chkDisHumRunClick(chkDisHumRun);
  chkSafeAreaDisNpcRun.Checked := g_Config.boSafeAreaDisNpcRun;
  chkSafeAreaDisShopStallHumRun.Checked := g_Config.boSafeAreaDisShopStallHumRun;
  chkSafeAreaDisOffLineHumRun.Checked := g_Config.boSafeAreaDisOffLineHumRun;
  chkWarDisTeleport.Checked := g_Config.boWarDisTeleport;

  EditSafeZoneSize.Value := g_Config.nSafeZoneSize;

  chkHintSafeZone.Checked := g_Config.boHintSafeZone;
  // seHintSafeZoneX.Value := g_Config.dwHintSafeZoneX;
  seHintSafeZoneY.Value := g_Config.dwHintSafeZoneY;
  seHintSafeZoneFColor.Value := g_Config.btHintSafeZoneFColor;
  seHintSafeZoneBColor.Value := g_Config.btHintSafeZoneBColor;
  seHintSafeZoneFSize.Value := g_Config.btHintSafeZoneFSize;

  // seHintSafeZoneX.Enabled := g_Config.boHintSafeZone;
  seHintSafeZoneY.Enabled := g_Config.boHintSafeZone;
  seHintSafeZoneFColor.Enabled := g_Config.boHintSafeZone;
  seHintSafeZoneBColor.Enabled := g_Config.boHintSafeZone;
  seHintSafeZoneFSize.Enabled := g_Config.boHintSafeZone;

  EditStartPointSize.Value := g_Config.nStartPointSize;
  seGroupMembersMax.Value := g_Config.nGroupMembersMax;

  EditRedHomeMap.Text := g_Config.sRedHomeMap;
  EditRedHomeX.Value := g_Config.nRedHomeX;
  EditRedHomeY.Value := g_Config.nRedHomeY;

  EditRedDieHomeMap.Text := g_Config.sRedDieHomeMap;
  EditRedDieHomeX.Value := g_Config.nRedDieHomeX;
  EditRedDieHomeY.Value := g_Config.nRedDieHomeY;

  EditHomeMap.Text := g_Config.sHomeMap;
  EditHomeX.Value := g_Config.nHomeX;
  EditHomeY.Value := g_Config.nHomeY;

  EditDecPkPointTime.Value := g_Config.dwDecPkPointTime div 1000;
  EditDecPkPointCount.Value := g_Config.nDecPkPointCount;
  EditPKFlagTime.Value := g_Config.dwPKFlagTime div 1000;
  seHumanAddPKPoint.Value := g_Config.nKillHumanAddPKPoint;
  seKillHumanWeaponUnlockRate.Value := g_Config.dwKillHumanWeaponUnlockRate;
  chkHeroKillHumanNotWeaponUnlock.Checked := g_Config.boHeroKillHumanNotWeaponUnlock;

  seDummyAddPKPoint.Value := g_Config.nDummyAddPKPoint;
  seKillHeroAddPKPoint.Value := g_Config.dwKillHeroAddPKPoint;

  CheckBoxTestServer.Checked := g_Config.boTestServer;
  CheckBoxServiceMode.Checked := g_Config.boServiceMode;
  CheckBoxVentureMode.Checked := g_Config.boVentureServer;
  CheckBoxNonPKMode.Checked := g_Config.boNonPKServer;

  // 离线挂机禁止项 控件值绑定--- piaoyun 2013-07-17
  chkOffLineShop.Checked := g_Config.boOffLineShop;
  chkOffLineHero.Checked := g_Config.boOffLineHero;
  chkOffLineSlave.Checked := g_Config.boOffLineSlave;

  seStartPermission.Value := g_Config.nStartPermission;
  seTestLevel.Value := g_Config.nTestLevel;
  seTestGold.Value := g_Config.nTestGold;
  seTestUserLimit.Value := g_Config.nTestUserLimit;
  seUserFull.Value := g_Config.nUserFull;
  // 行会人数限制 piaoyun 2013-07-17
  seGuildMemberMaxLimit.Value := g_Config.nGuildMemberMaxLimit;
  seGuildNameLen.Value := g_Config.nGuildNameLen;
  seGuildRankNameLen.Value := g_Config.nGuildRankNameLen;

  seHumChgMapOrLoginProtectTime.Value := g_Config.dwHumChgMapOrLoginProtectTime;

  chkSellItemToNpcShopNoCalcAddProperty.Checked := g_Config.boSellItemToNpcShopNoCalcAddProperty;
  chkShowNewValueFromBuyNpcItem.Checked := g_Config.boShowNewValueFromBuyNpcItem;

  CheckBoxTestServerClick(CheckBoxTestServer);

  CheckBoxKillHumanWinLevel.Checked := g_Config.boKillHumanWinLevel;
  CheckBoxKilledLostLevel.Checked := g_Config.boKilledLostLevel;
  CheckBoxKillHumanWinExp.Checked := g_Config.boKillHumanWinExp;
  CheckBoxKilledLostExp.Checked := g_Config.boKilledLostExp;
  EditKillHumanWinLevel.Value := g_Config.nKillHumanWinLevel;
  EditKilledLostLevel.Value := g_Config.nKilledLostLevel;
  EditKillHumanWinExp.Value := g_Config.nKillHumanWinExp;
  EditKillHumanLostExp.Value := g_Config.nKillHumanLostExp;
  EditHumanLevelDiffer.Value := g_Config.nHumanLevelDiffer;

  CheckBoxKillHumanWinLevelClick(CheckBoxKillHumanWinLevel);
  CheckBoxKilledLostLevelClick(CheckBoxKilledLostLevel);
  CheckBoxKillHumanWinExpClick(CheckBoxKillHumanWinExp);
  CheckBoxKilledLostExpClick(CheckBoxKilledLostExp);

  seHumanMaxGold.Value := g_Config.nHumanMaxGold;
  seHumanTryModeMaxGold.Value := g_Config.nHumanTryModeMaxGold;
  seTryModeLevel.Value := g_Config.nTryModeLevel;
  CheckBoxTryModeUseStorage.Checked := g_Config.boTryModeUseStorage;

  EditSayMsgMaxLen.Value := g_Config.nSayMsgMaxLen;
  EditSayRedMsgMaxLen.Value := g_Config.nSayRedMsgMaxLen;
  EditCanShoutMsgLevel.Value := g_Config.nCanShoutMsgLevel;
  CheckBoxShutRedMsgShowGMName.Checked := g_Config.boShutRedMsgShowGMName;
  CheckBoxShowPreFixMsg.Checked := g_Config.boShowPreFixMsg;
  EditGMRedMsgCmd.Text := g_GMRedMsgCmd;

  CheckBoxShowWhisperLevelMsg.Checked := g_Config.boShowWhisperLevelMsg;
  EditShowWhisperLevelMsg.Text := g_sShowWhisperLevelMsg;

  EditStartCastleWarDays.Value := g_Config.nStartCastleWarDays;
  EditStartCastlewarTime.Value := g_Config.nStartCastlewarTime;
  EditShowCastleWarEndMsgTime.Value := g_Config.dwShowCastleWarEndMsgTime div (60 * 1000);
  EditCastleWarTime.Value := g_Config.dwCastleWarTime div (60 * 1000);
  EditGetCastleTime.Value := g_Config.dwGetCastleTime div (60 * 1000);
  EditGuildWarTime.Value := g_Config.dwGuildWarTime div (60 * 1000);

  // 人物、人形怪、可挖尸体清理时间 piaoyun 2013-07-17
  seMakeGhostTime.Value := g_Config.dwMakeGhostTime div 1000;
  // 怪物清理时间 piaoyun 2013-07-17
  seMakeMonGhostTime.Value := g_Config.dwMakeMonGhostTime div 1000;

  // 假人尸体清理时间 chongchong 2013-11-12
  seMakeDummyGhostTime.Value := g_Config.dwMakeDummyGhostTime div 1000;

  // 物品清理时间 piaoyun 2013-07-17
  seClearDropOnFloorItemTime.Value := g_Config.dwClearDropOnFloorItemTime div 1000;

  // 上下马时间间隔 chongchong 2013-10-19
  seHorseTakeTime.Value := g_Config.dwHorseTakeTime;

  seTakeOnHorseUseTime.Value := g_Config.dwTakeOnHorseUseTime;

  chkReadyOnHorseDisableAction.Checked := g_Config.boReadyOnHorseDisableAction;

  seNpcButtonClickTime.Value := g_Config.dwNpcButtonClickTime;
  seNpcActorClickTime.Value := g_Config.dwNpcActorClickTime;

  sePlayerVarJClearTime.Value := g_Config.btPlayerVarJClearTime;

  seSaveHumanRcdTime.Value := g_Config.dwSaveHumanRcdTime div (60 * 1000);
  seHumanFreeDelayTime.Value := g_Config.dwHumanFreeDelayTime div (60 * 1000);
  // 数据读取保存超时间隔 -- piaoyun 2013-07-17
  seGetDBSockMsgTime.Value := g_Config.dwGetDBSockMsgTime div 1000;
  seFloorItemCanPickUpTime.Value := g_Config.dwFloorItemCanPickUpTime div 1000;

  EditBuildGuildPrice.Value := g_Config.nBuildGuildPrice;
  EditGuildWarPrice.Value := g_Config.nGuildWarPrice;
  EditMakeDurgPrice.Value := g_Config.nMakeDurgPrice;

  seTryDealTime.Value := g_Config.dwTryDealTime div 1000;
  seDealOKTime.Value := g_Config.dwDealOKTime div 1000;

  seTryChallengeTime.Value := g_Config.dwTryChallengeTime div 1000;
  seChallengeOKTime.Value := g_Config.dwChallengeOKTime div 1000;
  // 挑战时间 piaoyun 2013-07-22
  seChallengeTime.Value := g_Config.dwChallengeTime div 1000 div 60;
  // 挑战附加币 piaoyun 2013-07-22
  rgChallengeGold.ItemIndex := g_Config.btChallengeGoldIndex;

  chkCanNotGetBackChallenge.Checked := g_Config.boCanNotGetBackChallenge;
  chkDisableChallenge.Checked := g_Config.boDisableChallenge;

  EditCastleMemberPriceRate.Value := g_Config.nCastleMemberPriceRate;

  EditHearMsgFColor.Value := g_Config.btHearMsgFColor;
  EdittHearMsgBColor.Value := g_Config.btHearMsgBColor;
  EditWhisperMsgFColor.Value := g_Config.btWhisperMsgFColor;
  EditWhisperMsgBColor.Value := g_Config.btWhisperMsgBColor;
  EditGMWhisperMsgFColor.Value := g_Config.btGMWhisperMsgFColor;
  EditGMWhisperMsgBColor.Value := g_Config.btGMWhisperMsgBColor;

  seSendWhisperMsgFColor.Value := g_Config.btSendWhisperMsgFColor;
  seSendWhisperMsgBColor.Value := g_Config.btSendWhisperMsgBColor;

  seRefreshGameGoldFColor.Value := g_Config.btRefreshGameGoldFColor;
  seRefreshGameGoldBColor.Value := g_Config.btRefreshGameGoldBColor;

  seShowWhisperFColor.Value := g_Config.btShowWhisperFColor;
  seShowWhisperBColor.Value := g_Config.btShowWhisperBColor;

  seCloseWhisperFColor.Value := g_Config.btCloseWhisperFColor;
  seCloseWhisperBColor.Value := g_Config.btCloseWhisperBColor;

  EditRedMsgFColor.Value := g_Config.btRedMsgFColor;
  EditRedMsgBColor.Value := g_Config.btRedMsgBColor;
  EditGreenMsgFColor.Value := g_Config.btGreenMsgFColor;
  EditGreenMsgBColor.Value := g_Config.btGreenMsgBColor;
  EditBlueMsgFColor.Value := g_Config.btBlueMsgFColor;
  EditBlueMsgBColor.Value := g_Config.btBlueMsgBColor;
  EditCryMsgFColor.Value := g_Config.btCryMsgFColor;
  EditCryMsgBColor.Value := g_Config.btCryMsgBColor;
  EditGuildMsgFColor.Value := g_Config.btGuildMsgFColor;
  EditGuildMsgBColor.Value := g_Config.btGuildMsgBColor;
  EditGroupMsgFColor.Value := g_Config.btGroupMsgFColor;
  EditGroupMsgBColor.Value := g_Config.btGroupMsgBColor;
  EditCustMsgFColor.Value := g_Config.btCustMsgFColor;
  EditCustMsgBColor.Value := g_Config.btCustMsgBColor;

  EditUserSayMsgFColor.Value := g_Config.btUserSayMsgFColor;
  EditUserSayMsgBColor.Value := g_Config.btUserSayMsgBColor;

  EditTopUserSayMsgFColor.Value := g_Config.btTopUserSayMsgFColor;
  EditTopUserSayMsgBColor.Value := g_Config.btTopUserSayMsgBColor;

  EditDropItemFColor.Value := g_Config.btDropItemFColor;
  EditDropItemBColor.Value := g_Config.btDropItemBColor;

  seNPCLabelNormalColor.Value := g_Config.btNPCLabelNormalColor;
  chkNPCLabelFontStroke.Checked := g_Config.boNPCLabelFontStroke;
  seNPCLabelMouseMoveColor.Value := g_Config.btNPCLabelMouseMoveColor;
  seNPCLabelMouseDownColor.Value := g_Config.btNPCLabelMouseDownColor;

  EditNationMsgFColor.Value := g_Config.btNationMsgFColor;
  EditNationMsgBColor.Value := g_Config.btNationMsgBColor;

  CheckBoxPKLevelProtect.Checked := g_Config.boPKLevelProtect;
  EditPKProtectLevel.Value := g_Config.nPKProtectLevel;
  EditRedPKProtectLevel.Value := g_Config.nRedPKProtectLevel;
  CheckBoxPKLevelProtectClick(CheckBoxPKLevelProtect);

  chkCanNotGetBackDeal.Checked := g_Config.boCanNotGetBackDeal;
  chkDisableDeal.Checked := g_Config.boDisableDeal;
  chkControlDropItem.Checked := g_Config.boControlDropItem;
  chkIsSafeDisableDrop.Checked := g_Config.boInSafeDisableDrop;
  seCanDropPrice.Value := g_Config.nCanDropPrice;
  seCanDropGold.Value := g_Config.nCanDropGold;
  EditSuperRepairPriceRate.Value := g_Config.nSuperRepairPriceRate;
  EditRepairItemDecDura.Value := g_Config.nRepairItemDecDura;

  CheckBoxKillByMonstDropUseItem.Checked := g_Config.boKillByMonstDropUseItem;
  CheckBoxKillByHumanDropUseItem.Checked := g_Config.boKillByHumanDropUseItem;

  chkKillByMonstDropJewelryBoxItem.Checked := g_Config.boKillByMonstDropJewelryBoxItem;
  chkKillByHumanDropJewelryBoxItem.Checked := g_Config.boKillByHumanDropJewelryBoxItem;

  chkKillByMonstDropGodBlessItem.Checked := g_Config.boKillByMonstDropGodBlessItem;
  chkKillByHumanDropGodBlessItem.Checked := g_Config.boKillByHumanDropGodBlessItem;

  CheckBoxDieScatterBag.Checked := g_Config.boDieScatterBag;
  CheckBoxDieDropGold.Checked := g_Config.boDieDropGold;
  CheckBoxDieRedScatterBagAll.Checked := g_Config.boDieRedScatterBagAll;

  EditScatterBagItemsMinLevel.Value := g_Config.nScatterBagItemsMinLevel;
  EditDropUseItemsMaxCount.Value := g_Config.nDropUseItemsMaxCount;

  ScrollBarDieDropUseItemRate.Min := 1;
  ScrollBarDieDropUseItemRate.Max := 200;
  ScrollBarDieDropUseItemRate.Position := g_Config.nDieDropUseItemRate;
  ScrollBarDieRedDropUseItemRate.Min := 1;
  ScrollBarDieRedDropUseItemRate.Max := 200;
  ScrollBarDieRedDropUseItemRate.Position := g_Config.nDieRedDropUseItemRate;
  ScrollBarDieScatterBagRate.Min := 1;
  ScrollBarDieScatterBagRate.Max := 200;
  ScrollBarDieScatterBagRate.Position := g_Config.nDieScatterBagRate;

  scrlbrJewelryBoxItem.Min := 1;
  scrlbrJewelryBoxItem.Max := 200;
  scrlbrJewelryBoxItem.Position := g_Config.nDropJewelryBoxItemRate;

  scrlbrGodBlessItem.Min := 1;
  scrlbrGodBlessItem.Max := 200;
  scrlbrGodBlessItem.Position := g_Config.nDropGodBlessItemRate;

  EditSayMsgTime.Value := g_Config.dwSayMsgTime div 1000;
  EditSayMsgCount.Value := g_Config.nSayMsgCount;
  EditDisableSayMsgTime.Value := g_Config.dwDisableSayMsgTime div 1000;
  EditUserItemSayMsgTime.Value := g_Config.nUserItemSayMsgTime;

  seMaxInputStringLen.Value := g_Config.nMaxInputStringLen;

  CheckBoxFixExp.Checked := g_Config.boUseFixExp;
  SpinEditBaseExp.Value := g_Config.nBaseExp;
  SpinEditAddExp.Value := g_Config.nAddExp;
  SpinEditBaseExp.Enabled := not CheckBoxFixExp.Checked;
  SpinEditAddExp.Enabled := not CheckBoxFixExp.Checked;

  EditHighLevel.Value := g_Config.nHighLevel;
  EditHighLevelGetExp.Value := g_Config.nHighLevelGetExp;
  CheckBoxLimitChangeExp.Checked := g_Config.boLimitChangeExp;

  RadioGroupMaxLevel.ItemIndex := g_Config.btMaxLevel;
  rgMaxAC.ItemIndex := g_Config.btMaxAC;
  rgMaxHitPoint.ItemIndex := g_Config.btMaxHitPoint;

  CheckBoxDropUseItem.Checked := g_Config.boDropUseItem;
  EditDieRedDropUseItemOneRate.Value := g_Config.nDieRedDropUseItemOneRate;

  for I := 0 to GroupBoxDieDropUseItemRate.ControlCount - 1 do
  begin
    if GroupBoxDieDropUseItemRate.Controls[I] is TScrollBar then
    begin
      TScrollBar(GroupBoxDieDropUseItemRate.Controls[I]).Min := 1;
      TScrollBar(GroupBoxDieDropUseItemRate.Controls[I]).Max := 1000;
      TScrollBar(GroupBoxDieDropUseItemRate.Controls[I]).Position := g_Config.DieDropUseItemRates[TScrollBar(GroupBoxDieDropUseItemRate.Controls
        [I]).Tag];
    end;
  end;

  seDearRecallTime.Value := g_Config.dwDearRecallTime;
  seMasterRecallTime.Value := g_Config.dwMasterRecallTime;
  seGroupRecallTime.Value := g_Config.dwGroupRecallTime;

  GroupBoxParaly.Enabled := g_Config.boStartGameAuxiliary and g_Config.ClientConfigs[38];

  chkShareExpGroupSameScreen.Checked := g_Config.boShareExpGroupSameScreen;
  chkShareExpGroupSameMap.Checked := g_Config.boShareExpGroupSameMap;
  chkShareExpHeroSameMap.Checked := g_Config.boShareExpHeroSameMap;
  if not chkShareExpGroupSameScreen.Checked then
  begin
    chkShareExpGroupSameMap.Checked := False;
    chkShareExpGroupSameMap.Enabled := False;
    g_Config.boShareExpGroupSameMap := False;
  end
  else
  begin
    chkShareExpGroupSameMap.Enabled := True;
  end;

  chkHorseRun3Grid.Checked := g_Config.boHorseRun3Grid;

  chkNationGroupCheck.Checked := g_Config.boNationGroupCheck;
  chkNationGuildCheck.Checked := g_Config.boNationGuildCheck;
  seNationSayLevel.Value := g_Config.nNationSayLevel;

  RefGameVarConf();
  RefCharStatusConf();

  boOpened := True;
  GameConfigControl.ActivePageIndex := 0;
  PageControlGameSpeed.ActivePageIndex := 0;
  ShowModal;
end;

procedure TfrmGameConfig.RefGameSpeedConf;
begin
  chkSpeedControl.Checked := g_Config.boSpeedControl;

  RefGlobalSpeedCtrl;

  EditHitIntervalTime.Value := g_Config.dwHitIntervalTime;
  EditMagicHitIntervalTime.Value := g_Config.dwMagicHitIntervalTime;
  EditRunIntervalTime.Value := g_Config.dwRunIntervalTime;
  EditWalkIntervalTime.Value := g_Config.dwWalkIntervalTime;
  EditTurnIntervalTime.Value := g_Config.dwTurnIntervalTime;
  EditDigUpIntervalTime.Value := g_Config.dwDigUpIntervalTime;
  // EditUseItemIntervalTime.Value := g_Config.dwUseItemIntervalTime;

  EditMaxHitMsgCount.Value := g_Config.nMaxHitMsgCount;
  EditMaxSpellMsgCount.Value := g_Config.nMaxSpellMsgCount;
  EditMaxRunMsgCount.Value := g_Config.nMaxRunMsgCount;
  EditMaxWalkMsgCount.Value := g_Config.nMaxWalkMsgCount;
  EditMaxTurnMsgCount.Value := g_Config.nMaxTurnMsgCount;
  EditMaxDigUpMsgCount.Value := g_Config.nMaxDigUpMsgCount;
  CheckBoxboKickOverSpeed.Checked := g_Config.boKickOverSpeed;
  EditOverSpeedKickCount.Value := g_Config.nOverSpeedKickCount;
  EditDropOverSpeed.Value := g_Config.dwDropOverSpeed;
  CheckBoxboKickOverSpeedClick(CheckBoxboKickOverSpeed);

  CheckBoxSpellSendUpdateMsg.Checked := g_Config.boSpellSendUpdateMsg;
  CheckBoxActionSendActionMsg.Checked := g_Config.boActionSendActionMsg;

  if g_Config.btSpeedControlMode = 0 then
  begin
    RadioButtonDelyMode.Checked := True;
    RadioButtonFilterMode.Checked := False;
  end
  else
  begin
    RadioButtonDelyMode.Checked := False;
    RadioButtonFilterMode.Checked := True;
  end;

  CheckBoxDisableStruck.Checked := g_Config.boDisableStruck;
  CheckBoxDisableSelfStruck.Checked := g_Config.boDisableSelfStruck;
  chkMagicshieldStruck.Checked := g_Config.boMagicshieldStruck;

  EditStruckTime.Value := g_Config.dwStruckTime;

  CheckBoxCheckActionCount.Checked := g_Config.boCheckActionCount;
  EditHitCountIntervalTime.Value := g_Config.dwHitCountIntervalTime;
  EditMagicHitCountIntervalTime.Value := g_Config.dwMagicHitCountIntervalTime;
  EditMoveCountIntervalTime.Value := g_Config.dwMoveCountIntervalTime;
  EditCanHitCount.Value := g_Config.nCanHitCount;
  EditCanMagicHitCount.Value := g_Config.nCanMagicHitCount;
  EditCanMoveCount.Value := g_Config.nCanMoveCount;

  EditCheckHitCount.Value := g_Config.nCheckHitCount;
  EditCheckMagicHitCount.Value := g_Config.nCheckMagicHitCount;
  EditCheckMoveCount.Value := g_Config.nCheckMoveCount;

  CheckBoxSendUpdateMsg.Checked := g_Config.boSendUpdateMsg;

  EditMaxHitDeliveryTime.Value := g_Config.nMaxHitDeliveryTime;
  EditMaxMagicHitDeliveryTime.Value := g_Config.nMaxMagicHitDeliveryTime;
  EditMaxRunDeliveryTime.Value := g_Config.nMaxRunDeliveryTime;
  EditMaxWalkDeliveryTime.Value := g_Config.nMaxWalkDeliveryTime;
  EditMaxTurnDeliveryTime.Value := g_Config.nMaxTurnDeliveryTime;
  EditMaxDigUpDeliveryTime.Value := g_Config.nMaxDigUpDeliveryTime;

  chkHorseRun3Grid.Checked := g_Config.boHorseRun3Grid;
end;

procedure TfrmGameConfig.ButtonGameSpeedDefaultClick(Sender: TObject);
begin
  if Application.MessageBox('是否确认恢复默认设置？', '确认信息', MB_YESNO + MB_ICONQUESTION) <> IDYES then
  begin
    Exit;
  end;
  g_Config.dwHitIntervalTime := 700;
  g_Config.dwMagicHitIntervalTime := 450;
  g_Config.dwRunIntervalTime := 400;
  g_Config.dwWalkIntervalTime := 400;
  g_Config.dwTurnIntervalTime := 100;
  g_Config.dwDigUpIntervalTime := 100;
  // g_Config.dwUseItemIntervalTime := 500;
  g_Config.nMaxHitMsgCount := 1;
  g_Config.nMaxSpellMsgCount := 1;
  g_Config.nMaxRunMsgCount := 1;
  g_Config.nMaxWalkMsgCount := 1;
  g_Config.nMaxTurnMsgCount := 1;
  g_Config.nMaxDigUpMsgCount := 1;
  g_Config.nOverSpeedKickCount := 2;
  g_Config.dwDropOverSpeed := 200;
  g_Config.boKickOverSpeed := True;
  g_Config.boDisableStruck := False;
  g_Config.boDisableSelfStruck := True;
  g_Config.boMagicshieldStruck := False;
  g_Config.dwStruckTime := 300;
  g_Config.boSpellSendUpdateMsg := True;
  g_Config.boActionSendActionMsg := True;
  g_Config.btSpeedControlMode := 0;

  g_Config.nMaxHitDeliveryTime := 100;
  g_Config.nMaxMagicHitDeliveryTime := 100;
  g_Config.nMaxRunDeliveryTime := 100;
  g_Config.nMaxWalkDeliveryTime := 100;
  g_Config.nMaxTurnDeliveryTime := 100;
  g_Config.nMaxDigUpDeliveryTime := 100;

  g_Config.boSendUpdateMsg := False;
  g_Config.boHorseRun3Grid := False;

  RefGameSpeedConf();
  ModValue();
end;

procedure TfrmGameConfig.ButtonGameSpeedSaveClick(Sender: TObject);
begin
  Config.WriteBool('Setup', 'SpeedControl', g_Config.boSpeedControl);

  Config.WriteInteger('Setup', 'HitIntervalTime', g_Config.dwHitIntervalTime);
  Config.WriteInteger('Setup', 'MagicHitIntervalTime', g_Config.dwMagicHitIntervalTime);
  Config.WriteInteger('Setup', 'RunIntervalTime', g_Config.dwRunIntervalTime);
  Config.WriteInteger('Setup', 'WalkIntervalTime', g_Config.dwWalkIntervalTime);
  Config.WriteInteger('Setup', 'TurnIntervalTime', g_Config.dwTurnIntervalTime);
  Config.WriteInteger('Setup', 'DigUpIntervalTime', g_Config.dwDigUpIntervalTime);

  Config.WriteInteger('Setup', 'MaxHitMsgCount', g_Config.nMaxHitMsgCount);
  Config.WriteInteger('Setup', 'MaxSpellMsgCount', g_Config.nMaxSpellMsgCount);
  Config.WriteInteger('Setup', 'MaxRunMsgCount', g_Config.nMaxRunMsgCount);
  Config.WriteInteger('Setup', 'MaxWalkMsgCount', g_Config.nMaxWalkMsgCount);
  Config.WriteInteger('Setup', 'MaxTurnMsgCount', g_Config.nMaxTurnMsgCount);
  Config.WriteInteger('Setup', 'MaxSitDonwMsgCount', g_Config.nMaxSitDonwMsgCount);
  Config.WriteInteger('Setup', 'MaxDigUpMsgCount', g_Config.nMaxDigUpMsgCount);
  Config.WriteInteger('Setup', 'OverSpeedKickCount', g_Config.nOverSpeedKickCount);
  Config.WriteBool('Setup', 'KickOverSpeed', g_Config.boKickOverSpeed);
  Config.WriteBool('Setup', 'SpellSendUpdateMsg', g_Config.boSpellSendUpdateMsg);
  Config.WriteBool('Setup', 'ActionSendActionMsg', g_Config.boActionSendActionMsg);
  Config.WriteInteger('Setup', 'DropOverSpeed', g_Config.dwDropOverSpeed);
  Config.WriteBool('Setup', 'DisableStruck', g_Config.boDisableStruck);
  Config.WriteBool('Setup', 'DisableSelfStruck', g_Config.boDisableSelfStruck);
  Config.WriteBool('Setup', 'MagicshieldStruck', g_Config.boMagicshieldStruck);
  Config.WriteInteger('Setup', 'StruckTime', g_Config.dwStruckTime);
  Config.WriteInteger('Setup', 'SpeedControlMode', g_Config.btSpeedControlMode);

  Config.WriteBool('Setup', 'SendUpdateMsg', g_Config.boSendUpdateMsg);
  Config.WriteInteger('Setup', 'MaxHitDeliveryTime', g_Config.nMaxHitDeliveryTime);
  Config.WriteInteger('Setup', 'MaxMagicHitDeliveryTime', g_Config.nMaxMagicHitDeliveryTime);
  Config.WriteInteger('Setup', 'MaxRunDeliveryTime', g_Config.nMaxRunDeliveryTime);
  Config.WriteInteger('Setup', 'MaxWalkDeliveryTime', g_Config.nMaxWalkDeliveryTime);
  Config.WriteInteger('Setup', 'MaxWalkDeliveryTime', g_Config.nMaxWalkDeliveryTime);
  Config.WriteInteger('Setup', 'MaxTurnDeliveryTime', g_Config.nMaxTurnDeliveryTime);
  Config.WriteInteger('Setup', 'MaxDigUpDeliveryTime', g_Config.nMaxDigUpDeliveryTime);
  Config.WriteBool('Setup', 'HorseRun3Grid', g_Config.boHorseRun3Grid);

  if boSendServerConfig then
    UserEngine.SendServerConfig();

  uModValue();
end;

procedure TfrmGameConfig.EditHitIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHitIntervalTime := EditHitIntervalTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.EditMagicHitIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMagicHitIntervalTime := EditMagicHitIntervalTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.EditRunIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwRunIntervalTime := EditRunIntervalTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.EditWalkIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwWalkIntervalTime := EditWalkIntervalTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.EditTurnIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwTurnIntervalTime := EditTurnIntervalTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxHitMsgCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxHitMsgCount := EditMaxHitMsgCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxSpellMsgCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxSpellMsgCount := EditMaxSpellMsgCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxRunMsgCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxRunMsgCount := EditMaxRunMsgCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxWalkMsgCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxWalkMsgCount := EditMaxWalkMsgCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxTurnMsgCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxTurnMsgCount := EditMaxTurnMsgCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxDigUpMsgCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxDigUpMsgCount := EditMaxDigUpMsgCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditOverSpeedKickCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nOverSpeedKickCount := EditOverSpeedKickCount.Value;
  ModValue();

end;

procedure TfrmGameConfig.EditDropOverSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwDropOverSpeed := EditDropOverSpeed.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxSpellSendUpdateMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSpellSendUpdateMsg := CheckBoxSpellSendUpdateMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxActionSendActionMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boActionSendActionMsg := CheckBoxActionSendActionMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxboKickOverSpeedClick(Sender: TObject);
begin
  EditOverSpeedKickCount.Enabled := CheckBoxboKickOverSpeed.Checked and g_Config.boSpeedControl;
  if not boOpened then
    Exit;
  g_Config.boKickOverSpeed := CheckBoxboKickOverSpeed.Checked;
  ModValue();
end;

procedure TfrmGameConfig.RadioButtonDelyModeClick(Sender: TObject);
var
  boFalg: Boolean;
begin
  if not boOpened then
    Exit;
  boFalg := RadioButtonDelyMode.Checked;
  if boFalg then
  begin
    g_Config.btSpeedControlMode := 0;
  end
  else
  begin
    g_Config.btSpeedControlMode := 1;
  end;
  ModValue();
end;

procedure TfrmGameConfig.RadioButtonFilterModeClick(Sender: TObject);
var
  boFalg: Boolean;
begin
  if not boOpened then
    Exit;
  boFalg := RadioButtonFilterMode.Checked;
  if boFalg then
  begin
    g_Config.btSpeedControlMode := 1;
  end
  else
  begin
    g_Config.btSpeedControlMode := 0;
  end;
  ModValue();
end;

procedure TfrmGameConfig.EditConsoleShowUserCountTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwConsoleShowUserCountTime := EditConsoleShowUserCountTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.EditShowLineNoticeTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwShowLineNoticeTime := EditShowLineNoticeTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.ComboBoxLineNoticeColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nLineNoticeColor := ComboBoxLineNoticeColor.ItemIndex;
  ModValue();
end;

procedure TfrmGameConfig.EditSoftVersionDateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmGameConfig.EditLineNoticePreFixChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sLineNoticePreFix := Trim(EditLineNoticePreFix.Text);
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxShowMakeItemMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowMakeItemMsg := CheckBoxShowMakeItemMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CbViewHackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boViewHackMessage := CbViewHack.Checked;

  ModValue();
end;

procedure TfrmGameConfig.CkViewAdmfailClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boViewAdmissionFailure := CkViewAdmfail.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxShowExceptionMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowExceptionMsg := CheckBoxShowExceptionMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxCanOldClientLogonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCanOldClientLogon := CheckBoxCanOldClientLogon.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxSendOnlineCountClick(Sender: TObject);
var
  boStatus: Boolean;
begin
  boStatus := CheckBoxSendOnlineCount.Checked;
  EditSendOnlineCountRate.Enabled := boStatus;
  EditSendOnlineTime.Enabled := boStatus;
  if not boOpened then
    Exit;
  g_Config.boSendOnlineCount := boStatus;
  ModValue();
end;

procedure TfrmGameConfig.EditSendOnlineCountRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSendOnlineCountRate := EditSendOnlineCountRate.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditSendOnlineTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSendOnlineTime := EditSendOnlineTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.EditMonsterPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonsterPowerRate := EditMonsterPowerRate.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditEditItemsPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemsPowerRate := EditEditItemsPowerRate.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditItemsACPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemsACPowerRate := EditItemsACPowerRate.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxDisableStruckClick(Sender: TObject);
begin
  EditStruckTime.Enabled := not CheckBoxDisableStruck.Checked;
  if not boOpened then
    Exit;
  g_Config.boDisableStruck := CheckBoxDisableStruck.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxDisableSelfStruckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableSelfStruck := CheckBoxDisableSelfStruck.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.EditStruckTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwStruckTime := EditStruckTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.RefGameVarConf;
begin
  EditSoftVersionDate.Text := IntToStr(g_Config.nSoftVersionDate);
  EditConsoleShowUserCountTime.Value := g_Config.dwConsoleShowUserCountTime div 1000;
  EditShowLineNoticeTime.Value := g_Config.dwShowLineNoticeTime div 1000;
  ComboBoxLineNoticeColor.ItemIndex := _MAX(0, _MIN(2, g_Config.nLineNoticeColor));
  EditLineNoticePreFix.Text := g_Config.sLineNoticePreFix;

  CheckBoxShowMakeItemMsg.Checked := g_Config.boShowMakeItemMsg;
  CbViewHack.Checked := g_Config.boViewHackMessage;
  CkViewAdmfail.Checked := g_Config.boViewAdmissionFailure;
  CheckBoxShowExceptionMsg.Checked := g_Config.boShowExceptionMsg;

  CheckBoxSendOnlineCount.Checked := g_Config.boSendOnlineCount;
  EditSendOnlineCountRate.Value := g_Config.nSendOnlineCountRate;
  EditSendOnlineTime.Value := g_Config.dwSendOnlineTime div 1000;
  CheckBoxSendOnlineCountClick(CheckBoxSendOnlineCount);

  EditMonsterPowerRate.Value := g_Config.nMonsterPowerRate;
  EditEditItemsPowerRate.Value := g_Config.nItemsPowerRate;
  EditItemsACPowerRate.Value := g_Config.nItemsACPowerRate;
  CheckBoxCanOldClientLogon.Checked := g_Config.boCanOldClientLogon;
  chkOldClient.Checked := g_Config.boIsOldClient;

  chkRecordPublicMsg.Checked := g_Config.boRecordPublicMsg;
  chkRecordPrivateMsg.Checked := g_Config.boRecordPrivateMsg;
  chkRecordGuildMsg.Checked := g_Config.boRecordGuildMsg;
  chkRecordCryCryMsg.Checked := g_Config.boRecordCryCryMsg;
  chkRecordGroupMsg.Checked := g_Config.boRecordGroupMsg;
  chkRecordNationMsg.Checked := g_Config.boRecordNationMsg;
  chkPermissionChangeLog.Checked := g_Config.boPermissionChangeLog;
end;

procedure TfrmGameConfig.ButtonGeneralSaveClick(Sender: TObject);
var
  SoftVersionDate: Integer;
begin
  SoftVersionDate := StrToIntDef(Trim(EditSoftVersionDate.Text), -1);
  if (SoftVersionDate < 0) then
  begin
    Application.MessageBox('客户端版号设置错误！', '错误信息', MB_OK + MB_ICONERROR);
    EditSoftVersionDate.SetFocus;
    Exit;
  end;
  g_Config.nSoftVersionDate := SoftVersionDate;

  Config.WriteInteger('Setup', 'SoftVersionDate', g_Config.nSoftVersionDate);
  Config.WriteInteger('Setup', 'ConsoleShowUserCountTime', g_Config.dwConsoleShowUserCountTime);
  Config.WriteInteger('Setup', 'ShowLineNoticeTime', g_Config.dwShowLineNoticeTime);
  Config.WriteInteger('Setup', 'LineNoticeColor', g_Config.nLineNoticeColor);
  StringConf.WriteString('String', 'LineNoticePreFix', g_Config.sLineNoticePreFix);
  Config.WriteBool('Setup', 'ShowMakeItemMsg', g_Config.boShowMakeItemMsg);
  Config.WriteString('Server', 'ViewHackMessage', BoolToStr(g_Config.boViewHackMessage));
  Config.WriteString('Server', 'ViewAdmissionFailure', BoolToStr(g_Config.boViewAdmissionFailure));
  Config.WriteBool('Setup', 'ShowExceptionMsg', g_Config.boShowExceptionMsg);

  Config.WriteBool('Setup', 'SendOnlineCount', g_Config.boSendOnlineCount);
  Config.WriteInteger('Setup', 'SendOnlineCountRate', g_Config.nSendOnlineCountRate);
  Config.WriteInteger('Setup', 'SendOnlineTime', g_Config.dwSendOnlineTime);

  Config.WriteInteger('Setup', 'MonsterPowerRate', g_Config.nMonsterPowerRate);
  Config.WriteInteger('Setup', 'ItemsPowerRate', g_Config.nItemsPowerRate);
  Config.WriteInteger('Setup', 'ItemsACPowerRate', g_Config.nItemsACPowerRate);
  Config.WriteBool('Setup', 'CanOldClientLogon', g_Config.boCanOldClientLogon);
  Config.WriteBool('Setup', 'IsOldClient', g_Config.boIsOldClient);

  Config.WriteBool('Setup', 'RecordPublicMsg', g_Config.boRecordPublicMsg);
  Config.WriteBool('Setup', 'RecordPrivateMsg', g_Config.boRecordPrivateMsg);
  Config.WriteBool('Setup', 'RecordGuildMsg', g_Config.boRecordGuildMsg);
  Config.WriteBool('Setup', 'RecordCryCryMsg', g_Config.boRecordCryCryMsg);
  Config.WriteBool('Setup', 'RecordGroupMsg', g_Config.boRecordGroupMsg);
  Config.WriteBool('Setup', 'RecordNationMsg', g_Config.boRecordNationMsg);
  Config.WriteBool('Setup', 'PermissionChangeLog', g_Config.boPermissionChangeLog);

  uModValue();
end;

procedure TfrmGameConfig.EditKillMonExpMultipleChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwKillMonExpMultiple := EditKillMonExpMultiple.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxHighLevelKillMonFixExpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHighLevelKillMonFixExp := CheckBoxHighLevelKillMonFixExp.Checked;
  ModValue();
end;

procedure TfrmGameConfig.GridLevelExpSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmGameConfig.ComboBoxLevelExpClick(Sender: TObject);
const
  HIGH_VALUE = 4200000000;
var
  I: Integer;
  LevelExpScheme: TLevelExpScheme;
  dwOneLevelExp: LongWord;
  dwExp: LongWord;
  Int64Value: Int64;
  IsHighLongWord: Boolean;
begin
  if not boOpened then
    Exit;
  if Application.MessageBox('升级经验计划设置的经验将立即生效，是否确认使用此经验计划？', '确认信息', MB_YESNO + MB_ICONQUESTION) = IDNO then
  begin
    Exit;
  end;
  // ComboBoxLevelExp.AddItem('原始经验值', TObject(s_OldLevelExp));
  // ComboBoxLevelExp.AddItem('标准经验值', TObject(s_StdLevelExp));
  LevelExpScheme := TLevelExpScheme(ComboBoxLevelExp.Items.Objects[ComboBoxLevelExp.ItemIndex]);
  case LevelExpScheme of
    s_OldLevelExp:
      g_Config.dwNeedExps := g_dwOldNeedExps;
    s_StdLevelExp:
      begin
        IsHighLongWord := False;
        g_Config.dwNeedExps := g_dwOldNeedExps;
        dwOneLevelExp := 4000000000 div (High(g_Config.dwNeedExps) { div 2 } );
        for I := 1 to MAXCHANGELEVEL do
        begin
          if (26 + I) > MAXCHANGELEVEL then
            Break;

          if IsHighLongWord then
            dwExp := HIGH_VALUE
          else
          begin
            Int64Value := Int64(dwOneLevelExp) * Int64(I);
            if Int64Value >= HIGH_VALUE then
            begin
              IsHighLongWord := True;
              Int64Value := HIGH_VALUE;
            end;
            dwExp := Int64Value;
          end;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[26 + I] := dwExp;
        end;
      end;
    s_2Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 2;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_5Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 5;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_8Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 8;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_10Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 10;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_20Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 20;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_30Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 30;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_40Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 40;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_50Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 50;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_60Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 60;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_70Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 70;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_80Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 80;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_90Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 90;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_100Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 100;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_150Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 150;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_200Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 200;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_250Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 250;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
    s_300Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwNeedExps[I] div 300;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwNeedExps[I] := dwExp;
        end;
      end;
  end;
  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    GridLevelExp.Cells[1, I] := IntToStr(g_Config.dwNeedExps[I]);
  end;
  ModValue();
end;

procedure TfrmGameConfig.ButtonExpSaveClick(Sender: TObject);
var
  I: Integer;
  dwExp: LongWord;
  NeedExps: TLevelNeedExp;
begin
  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    dwExp := LongWord(StrToInt64Def(GridLevelExp.Cells[1, I], 0));
    if (dwExp = 0) then
    begin
      Application.MessageBox(PChar('等级 ' + IntToStr(I) + ' 升级经验设置错误！'), '错误信息', MB_OK + MB_ICONERROR);
      GridLevelExp.Row := I;
      GridLevelExp.SetFocus;
      Exit;
    end;
    NeedExps[I] := dwExp;
  end;
  g_Config.dwNeedExps := NeedExps;

  for I := 1 to GridLevelExpRate.RowCount - 1 do
  begin
    g_Config.LevelExpRates[I] := StrToIntDef(GridLevelExpRate.Cells[1, I], 0);
  end;

  ExpConfig.WriteInteger('Exp', 'KillMonExpMultiple', g_Config.dwKillMonExpMultiple);
  ExpConfig.WriteBool('Exp', 'HighLevelKillMonFixExp', g_Config.boHighLevelKillMonFixExp);
  for I := Low(g_Config.dwNeedExps) to High(g_Config.dwNeedExps) do
  begin
    ExpConfig.WriteString('Exp', 'Level' + IntToStr(I), IntToStr(g_Config.dwNeedExps[I]));
  end;

  for I := Low(g_Config.LevelExpRates) to High(g_Config.LevelExpRates) do
  begin
    ExpConfig.WriteInteger('Exp', 'LevelExpRate' + IntToStr(I), g_Config.LevelExpRates[I]);
  end;

  ExpConfig.WriteBool('Exp', 'UseFixExp', g_Config.boUseFixExp);
  ExpConfig.WriteInteger('Exp', 'BaseExp', g_Config.nBaseExp);
  ExpConfig.WriteInteger('Exp', 'AddExp', g_Config.nAddExp);
  ExpConfig.WriteInteger('Exp', 'HighLevel', g_Config.nHighLevel);
  ExpConfig.WriteInteger('Exp', 'HighLevelGetExp', g_Config.nHighLevelGetExp);
  ExpConfig.WriteBool('Exp', 'LimitChangeExp', g_Config.boLimitChangeExp);
  ExpConfig.WriteInteger('Exp', 'MaxUpLevelCount', g_Config.nMaxUpLevelCount);

  ExpConfig.WriteBool('Exp', 'HighLevelGroupFixExp', g_Config.boHighLevelGroupFixExp);

  ExpConfig.WriteBool('Setup', 'ShareExpGroupSameScreen', g_Config.boShareExpGroupSameScreen);
  ExpConfig.WriteBool('Setup', 'ShareExpGroupSameMap', g_Config.boShareExpGroupSameMap);
  ExpConfig.WriteBool('Setup', 'ShareExpHeroSameMap', g_Config.boShareExpHeroSameMap);

  uModValue();
end;

procedure TfrmGameConfig.EditRepairDoorPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRepairDoorPrice := EditRepairDoorPrice.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditRepairWallPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRepairWallPrice := EditRepairWallPrice.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditHireArcherPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHireArcherPrice := EditHireArcherPrice.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditHireGuardPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHireGuardPrice := EditHireGuardPrice.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCastleGoldMaxChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCastleGoldMax := EditCastleGoldMax.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCastleOneDayGoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCastleOneDayGold := EditCastleOneDayGold.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCastleHomeMapChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sCastleHomeMap := Trim(EditCastleHomeMap.Text);
  ModValue();
end;

procedure TfrmGameConfig.EditCastleHomeXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCastleHomeX := EditCastleHomeX.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCastleHomeYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCastleHomeY := EditCastleHomeY.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCastleNameChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sCASTLENAME := Trim(EditCastleName.Text);
  ModValue();
end;

procedure TfrmGameConfig.EditWarRangeXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCastleWarRangeX := EditWarRangeX.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditWarRangeYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCastleWarRangeY := EditWarRangeY.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxGetAllNpcTaxClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGetAllNpcTax := CheckBoxGetAllNpcTax.Checked;
  ModValue();
end;

procedure TfrmGameConfig.EditTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCastleTaxRate := EditTaxRate.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCastleMemberPriceRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCastleMemberPriceRate := EditCastleMemberPriceRate.Value;
  ModValue();
end;

procedure TfrmGameConfig.ButtonCastleSaveClick(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'RepairDoor', g_Config.nRepairDoorPrice);
  Config.WriteInteger('Setup', 'RepairWall', g_Config.nRepairWallPrice);
  Config.WriteInteger('Setup', 'HireArcher', g_Config.nHireArcherPrice);
  Config.WriteInteger('Setup', 'HireGuard', g_Config.nHireGuardPrice);
  Config.WriteInteger('Setup', 'CastleGoldMax', g_Config.nCastleGoldMax);
  Config.WriteInteger('Setup', 'CastleOneDayGold', g_Config.nCastleOneDayGold);
  Config.WriteString('Setup', 'CastleName', g_Config.sCASTLENAME);
  Config.WriteString('Setup', 'CastleHomeMap', g_Config.sCastleHomeMap);
  Config.WriteInteger('Setup', 'CastleHomeX', g_Config.nCastleHomeX);
  Config.WriteInteger('Setup', 'CastleHomeY', g_Config.nCastleHomeY);
  Config.WriteInteger('Setup', 'CastleWarRangeX', g_Config.nCastleWarRangeX);
  Config.WriteInteger('Setup', 'CastleWarRangeY', g_Config.nCastleWarRangeY);
  Config.WriteInteger('Setup', 'CastleTaxRate', g_Config.nCastleTaxRate);
  Config.WriteBool('Setup', 'CastleGetAllNpcTax', g_Config.boGetAllNpcTax);
  Config.WriteInteger('Setup', 'CastleMemberPriceRate', g_Config.nCastleMemberPriceRate);

  uModValue();
end;

procedure TfrmGameConfig.chkDisHumRunClick(Sender: TObject);
var
  boChecked: Boolean;
begin
  boChecked := not chkDisHumRun.Checked;
  if boChecked then
  begin
    chkRunHum.Checked := False;
    chkRunHum.Enabled := False;
    chkRunMon.Checked := False;
    chkRunMon.Enabled := False;
    chkWarDisHumRun.Checked := False;
    chkWarDisHumRun.Enabled := False;
    chkRunNpc.Checked := False;
    chkRunGuard.Checked := False;
    chkRunNpc.Enabled := False;
    chkRunGuard.Enabled := False;
    chkGMRunAll.Checked := False;
    chkGMRunAll.Enabled := False;
    chkSafeArea.Checked := False;
    chkSafeArea.Enabled := False;
    chkSafeAreaDisNpcRun.Checked := False;
    chkSafeAreaDisShopStallHumRun.Enabled := False;
    chkSafeAreaDisOffLineHumRun.Enabled := False;
    chkSafeAreaDisNpcRun.Enabled := False;
    chkSafeAreaDisOffLineHumRun.Checked := False;
    chkSafeAreaDisNpcRun.Checked := False;
    chkSafeAreaDisShopStallHumRun.Checked := False;
    chkWarDisTeleport.Enabled := False;
    chkWarDisTeleport.Checked := False;

    { chkSafeAreaDisNpcRun.Enabled := CheckBoxRunNpc.Checked;
      chkSafeAreaDisShopStallHumRun.Checked := False;
      chkSafeAreaDisShopStallHumRun.Enabled := CheckBoxRunHum.Checked;
      chkSafeAreaDisOffLineHumRun.Checked := False;
      chkSafeAreaDisOffLineHumRun.Enabled := CheckBoxRunHum.Checked; }
  end
  else
  begin
    chkRunHum.Enabled := True;
    chkRunMon.Enabled := True;
    chkWarDisHumRun.Enabled := True;
    chkRunNpc.Enabled := True;
    chkRunGuard.Enabled := True;
    chkGMRunAll.Enabled := True;
    chkSafeArea.Enabled := True;
    chkSafeAreaDisShopStallHumRun.Enabled := True;
    chkSafeAreaDisOffLineHumRun.Enabled := True;
    chkSafeAreaDisNpcRun.Enabled := True;
    chkWarDisTeleport.Enabled := True;

    { chkSafeAreaDisNpcRun.Enabled := CheckBoxRunNpc.Checked;
      chkSafeAreaDisShopStallHumRun.Enabled := CheckBoxRunHum.Checked;
      chkSafeAreaDisOffLineHumRun.Enabled := CheckBoxRunHum.Checked; }
  end;

  if not boOpened then
    Exit;
  g_Config.boDiableHumanRun := boChecked;

  ModValue();
end;

procedure TfrmGameConfig.chkRunHumClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRUNHUMAN := chkRunHum.Checked;
  // chkSafeAreaDisShopStallHumRun.Enabled := CheckBoxRunHum.Checked;
  // chkSafeAreaDisOffLineHumRun.Enabled := CheckBoxRunHum.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkRunMonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRUNMON := chkRunMon.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkRunNpcClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRunNpc := chkRunNpc.Checked;
  // chkSafeAreaDisNpcRun.Enabled := CheckBoxRunNpc.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkRunGuardClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRunGuard := chkRunGuard.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkWarDisHumRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boWarDisHumRun := chkWarDisHumRun.Checked;

  chkWarHreoRun.Enabled := chkWarDisHumRun.Checked;
  g_Config.boWarHreoRun := chkWarHreoRun.Enabled and chkWarHreoRun.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkGMRunAllClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGMRunAll := chkGMRunAll.Checked;
  ModValue();
end;

procedure TfrmGameConfig.seTryDealTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwTryDealTime := seTryDealTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.seDealOKTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwDealOKTime := seDealOKTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.chkCanNotGetBackDealClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCanNotGetBackDeal := chkCanNotGetBackDeal.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkDisableDealClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableDeal := chkDisableDeal.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkControlDropItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boControlDropItem := chkControlDropItem.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkIsSafeDisableDropClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boInSafeDisableDrop := chkIsSafeDisableDrop.Checked;
  ModValue();
end;

procedure TfrmGameConfig.seCanDropPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCanDropPrice := seCanDropPrice.Value;
  ModValue();
end;

procedure TfrmGameConfig.seCanDropGoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCanDropGold := seCanDropGold.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditSafeZoneSizeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSafeZoneSize := EditSafeZoneSize.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkHintSafeZoneClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHintSafeZone := chkHintSafeZone.Checked;
  // seHintSafeZoneX.Enabled := g_Config.boHintSafeZone;
  seHintSafeZoneY.Enabled := g_Config.boHintSafeZone;
  seHintSafeZoneFColor.Enabled := g_Config.boHintSafeZone;
  seHintSafeZoneBColor.Enabled := g_Config.boHintSafeZone;
  seHintSafeZoneFSize.Enabled := g_Config.boHintSafeZone;
  ModValue();
end;

procedure TfrmGameConfig.EditStartPointSizeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStartPointSize := EditStartPointSize.Value;
  ModValue();
end;

procedure TfrmGameConfig.seGroupMembersMaxChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGroupMembersMax := seGroupMembersMax.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditRedHomeXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRedHomeX := EditRedHomeX.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditRedHomeYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRedHomeY := EditRedHomeY.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditRedHomeMapChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmGameConfig.EditRedDieHomeMapChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmGameConfig.EditRedDieHomeXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRedDieHomeX := EditRedDieHomeX.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditHomeMapChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmGameConfig.EditHomeXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHomeX := EditHomeX.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditHomeYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHomeY := EditHomeY.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditRedDieHomeYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRedDieHomeY := EditRedDieHomeY.Value;
  ModValue();
end;

procedure TfrmGameConfig.ButtonOptionSaveClick(Sender: TObject);
begin
  if EditRedHomeMap.Text = '' then
  begin
    Application.MessageBox('红名村地图设置错误！', '错误信息', MB_OK + MB_ICONERROR);
    EditRedHomeMap.SetFocus;
    Exit;
  end;
  g_Config.sRedHomeMap := Trim(EditRedHomeMap.Text);

  if EditRedDieHomeMap.Text = '' then
  begin
    Application.MessageBox('红名村地图设置错误！', '错误信息', MB_OK + MB_ICONERROR);
    EditRedDieHomeMap.SetFocus;
    Exit;
  end;
  g_Config.sRedDieHomeMap := Trim(EditRedDieHomeMap.Text);

  if EditHomeMap.Text = '' then
  begin
    Application.MessageBox('应急回城地图设置错误！', '错误信息', MB_OK + MB_ICONERROR);
    EditHomeMap.SetFocus;
    Exit;
  end;
  g_Config.sHomeMap := Trim(EditHomeMap.Text);

  Config.WriteInteger('Setup', 'SafeZoneSize', g_Config.nSafeZoneSize);
  Config.WriteBool('Setup', 'HintSafeZone', g_Config.boHintSafeZone);
  // Config.WriteInteger('Setup', 'HintSafeZoneX', g_Config.dwHintSafeZoneX);
  Config.WriteInteger('Setup', 'HintSafeZoneY', g_Config.dwHintSafeZoneY);
  Config.WriteInteger('Setup', 'HintSafeZoneFColor', g_Config.btHintSafeZoneFColor);
  Config.WriteInteger('Setup', 'HintSafeZoneBColor', g_Config.btHintSafeZoneBColor);
  Config.WriteInteger('Setup', 'HintSafeZoneFSize', g_Config.btHintSafeZoneFSize);

  Config.WriteInteger('Setup', 'StartPointSize', g_Config.nStartPointSize);

  Config.WriteString('Setup', 'RedHomeMap', g_Config.sRedHomeMap);
  Config.WriteInteger('Setup', 'RedHomeX', g_Config.nRedHomeX);
  Config.WriteInteger('Setup', 'RedHomeY', g_Config.nRedHomeY);

  Config.WriteString('Setup', 'RedDieHomeMap', g_Config.sRedDieHomeMap);
  Config.WriteInteger('Setup', 'RedDieHomeX', g_Config.nRedDieHomeX);
  Config.WriteInteger('Setup', 'RedDieHomeY', g_Config.nRedDieHomeY);

  Config.WriteString('Setup', 'HomeMap', g_Config.sHomeMap);
  Config.WriteInteger('Setup', 'HomeX', g_Config.nHomeX);
  Config.WriteInteger('Setup', 'HomeY', g_Config.nHomeY);

  Config.WriteBool('Setup', 'NationGroupCheck', g_Config.boNationGroupCheck);
  Config.WriteBool('Setup', 'NationGuildCheck', g_Config.boNationGuildCheck);
  Config.WriteInteger('Setup', 'NationSayLevel', g_Config.nNationSayLevel);

  uModValue();
end;

procedure TfrmGameConfig.ButtonOptionSave3Click(Sender: TObject);
begin
  Config.WriteBool('Setup', 'DiableHumanRun', g_Config.boDiableHumanRun);
  Config.WriteBool('Setup', 'RunHuman', g_Config.boRUNHUMAN);
  Config.WriteBool('Setup', 'RunMon', g_Config.boRUNMON);
  Config.WriteBool('Setup', 'RunNpc', g_Config.boRunNpc);
  Config.WriteBool('Setup', 'RunGuard', g_Config.boRunGuard);
  Config.WriteBool('Setup', 'WarDisableHumanRun', g_Config.boWarDisHumRun);
  Config.WriteBool('Setup', 'GMRunAll', g_Config.boGMRunAll);
  Config.WriteBool('Setup', 'SafeAreaLimitedRun', g_Config.boSafeAreaLimited);

  Config.WriteInteger('Setup', 'TryDealTime', g_Config.dwTryDealTime);
  Config.WriteInteger('Setup', 'DealOKTime', g_Config.dwDealOKTime);
  Config.WriteBool('Setup', 'CanNotGetBackDeal', g_Config.boCanNotGetBackDeal);
  Config.WriteBool('Setup', 'DisableDeal', g_Config.boDisableDeal);
  Config.WriteBool('Setup', 'ControlDropItem', g_Config.boControlDropItem);
  Config.WriteBool('Setup', 'InSafeDisableDrop', g_Config.boInSafeDisableDrop);
  Config.WriteInteger('Setup', 'CanDropGold', g_Config.nCanDropGold);
  Config.WriteInteger('Setup', 'CanDropPrice', g_Config.nCanDropPrice);

  Config.WriteInteger('Setup', 'DecLightItemDrugTime', g_Config.dwDecLightItemDrugTime);

  Config.WriteInteger('Setup', 'TryChallengeTime', g_Config.dwTryChallengeTime);
  Config.WriteInteger('Setup', 'ChallengeOKTime', g_Config.dwChallengeOKTime);
  Config.WriteBool('Setup', 'CanNotGetBackChallenge', g_Config.boCanNotGetBackChallenge);
  Config.WriteBool('Setup', 'DisableChallenge', g_Config.boDisableChallenge);
  Config.WriteBool('Setup', 'SafeAreaDisNpcRun', g_Config.boSafeAreaDisNpcRun);

  // 安全区禁止穿摆摊人物
  Config.WriteBool('Setup', 'SafeAreaDisShopStallHumRun', g_Config.boSafeAreaDisShopStallHumRun);
  // 安全区禁止穿离线人物
  Config.WriteBool('Setup', 'SafeAreaDisOffLineHumRun', g_Config.boSafeAreaDisOffLineHumRun);
  // 攻城区域禁止传送戒指
  Config.WriteBool('Setup', 'WarDisTeleport', g_Config.boWarDisTeleport);
  // 攻城区域禁止穿英雄
  Config.WriteBool('Setup', 'WarHreoRun', g_Config.boWarHreoRun);
  // 挑战时间
  Config.WriteInteger('Setup', 'ChallengeTime', g_Config.dwChallengeTime);
  // 挑战附加币
  Config.WriteInteger('Setup', 'ChallengeGoldIndex', g_Config.btChallengeGoldIndex);

  UserEngine.SendMapCanRun();
  uModValue();
end;

procedure TfrmGameConfig.EditDecPkPointTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwDecPkPointTime := EditDecPkPointTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.EditDecPkPointCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDecPkPointCount := EditDecPkPointCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditPKFlagTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwPKFlagTime := EditPKFlagTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.seHumanAddPKPointChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nKillHumanAddPKPoint := seHumanAddPKPoint.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxKillHumanWinLevelClick(Sender: TObject);
var
  boStatus: Boolean;
begin
  boStatus := CheckBoxKillHumanWinLevel.Checked;
  CheckBoxKilledLostLevel.Enabled := boStatus;
  EditKillHumanWinLevel.Enabled := boStatus;
  EditKilledLostLevel.Enabled := boStatus;
  if not boStatus then
  begin
    CheckBoxKilledLostLevel.Checked := False;
    if not CheckBoxKillHumanWinExp.Checked then
      EditHumanLevelDiffer.Enabled := False;
  end
  else
  begin
    EditHumanLevelDiffer.Enabled := True;
  end;
  if not boOpened then
    Exit;
  g_Config.boKillHumanWinLevel := boStatus;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxKilledLostLevelClick(Sender: TObject);
var
  boStatus: Boolean;
begin
  boStatus := CheckBoxKilledLostLevel.Checked;
  EditKilledLostLevel.Enabled := boStatus;
  if not boOpened then
    Exit;
  g_Config.boKilledLostLevel := boStatus;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxKillHumanWinExpClick(Sender: TObject);
var
  boStatus: Boolean;
begin
  boStatus := CheckBoxKillHumanWinExp.Checked;
  CheckBoxKilledLostExp.Enabled := boStatus;
  EditKillHumanWinExp.Enabled := boStatus;
  EditKillHumanLostExp.Enabled := boStatus;
  if not boStatus then
  begin
    CheckBoxKilledLostExp.Checked := False;
    if not CheckBoxKillHumanWinLevel.Checked then
      EditHumanLevelDiffer.Enabled := False;
  end
  else
  begin
    EditHumanLevelDiffer.Enabled := True;
  end;
  if not boOpened then
    Exit;
  g_Config.boKillHumanWinExp := boStatus;
  ModValue();

end;

procedure TfrmGameConfig.CheckBoxKilledLostExpClick(Sender: TObject);
var
  boStatus: Boolean;
begin
  boStatus := CheckBoxKilledLostExp.Checked;
  EditKillHumanLostExp.Enabled := boStatus;
  if not boOpened then
    Exit;
  g_Config.boKilledLostExp := boStatus;
  ModValue();

end;

procedure TfrmGameConfig.EditKillHumanWinLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nKillHumanWinLevel := EditKillHumanWinLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditKilledLostLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nKilledLostLevel := EditKilledLostLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditKillHumanWinExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nKillHumanWinExp := EditKillHumanWinExp.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditKillHumanLostExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nKillHumanLostExp := EditKillHumanLostExp.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditHumanLevelDifferChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumanLevelDiffer := EditHumanLevelDiffer.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxPKLevelProtectClick(Sender: TObject);
var
  boStatus: Boolean;
begin
  boStatus := CheckBoxPKLevelProtect.Checked;
  EditPKProtectLevel.Enabled := boStatus;
  if not boOpened then
    Exit;
  g_Config.boPKLevelProtect := boStatus;
  ModValue();
end;

procedure TfrmGameConfig.EditPKProtectLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPKProtectLevel := EditPKProtectLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditRedPKProtectLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRedPKProtectLevel := EditRedPKProtectLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.ButtonOptionSave2Click(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'DecPkPointTime', g_Config.dwDecPkPointTime);
  Config.WriteInteger('Setup', 'DecPkPointCount', g_Config.nDecPkPointCount);
  Config.WriteInteger('Setup', 'PKFlagTime', g_Config.dwPKFlagTime);
  Config.WriteInteger('Setup', 'KillHumanAddPKPoint', g_Config.nKillHumanAddPKPoint);
  Config.WriteInteger('Setup', 'KillHumanDecLuckPoint', g_Config.nKillHumanDecLuckPoint);

  Config.WriteBool('Setup', 'KillHumanWinLevel', g_Config.boKillHumanWinLevel);
  Config.WriteBool('Setup', 'KilledLostLevel', g_Config.boKilledLostLevel);
  Config.WriteInteger('Setup', 'KillHumanWinLevelPoint', g_Config.nKillHumanWinLevel);
  Config.WriteInteger('Setup', 'KilledLostLevelPoint', g_Config.nKilledLostLevel);
  Config.WriteBool('Setup', 'KillHumanWinExp', g_Config.boKillHumanWinExp);
  Config.WriteBool('Setup', 'KilledLostExp', g_Config.boKilledLostExp);
  Config.WriteInteger('Setup', 'KillHumanWinExpPoint', g_Config.nKillHumanWinExp);
  Config.WriteInteger('Setup', 'KillHumanLostExpPoint', g_Config.nKillHumanLostExp);
  Config.WriteInteger('Setup', 'HumanLevelDiffer', g_Config.nHumanLevelDiffer);

  Config.WriteInteger('Setup', 'DummyAddPKPoint', g_Config.nDummyAddPKPoint);

  // 杀英雄增加PK值 (点数) chongchong 2013-08-24
  Config.WriteInteger('Setup', 'KillHeroAddPKPoint', g_Config.dwKillHeroAddPKPoint);

  Config.WriteBool('Setup', 'PKProtect', g_Config.boPKLevelProtect);
  Config.WriteInteger('Setup', 'PKProtectLevel', g_Config.nPKProtectLevel);
  Config.WriteInteger('Setup', 'RedPKProtectLevel', g_Config.nRedPKProtectLevel);

  Config.WriteInteger('Setup', 'KillHumanWeaponUnlockRate', g_Config.dwKillHumanWeaponUnlockRate);
  Config.WriteBool('Setup', 'HeroKillHumanNotWeaponUnlock', g_Config.boHeroKillHumanNotWeaponUnlock);

  uModValue();
end;

procedure TfrmGameConfig.CheckBoxTestServerClick(Sender: TObject);
var
  boStatue: Boolean;
begin
  boStatue := CheckBoxTestServer.Checked;
  seTestLevel.Enabled := boStatue;
  seTestGold.Enabled := boStatue;
  seTestUserLimit.Enabled := boStatue;
  if not boOpened then
    Exit;
  g_Config.boTestServer := boStatue;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxServiceModeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boServiceMode := CheckBoxServiceMode.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxVentureModeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boVentureServer := CheckBoxVentureMode.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxNonPKModeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNonPKServer := CheckBoxNonPKMode.Checked;
  ModValue();
end;

procedure TfrmGameConfig.seTestLevelKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
begin
  seTestLevel.Tag := seTestLevel.Value;
end;

procedure TfrmGameConfig.seTestLevelChange(Sender: TObject);
begin
  if seTestLevel.Text = '' then
  begin
    seTestLevel.Tag := 1;
    seTestLevel.Text := '0';
    Exit;
  end;
  if (seTestLevel.Tag = 1) and (seTestLevel.Value <> 0) then
  begin
    seTestLevel.Tag := 0;
    seTestLevel.Value := seTestLevel.Value div 10;
  end;

  if not boOpened then
    Exit;
  g_Config.nTestLevel := seTestLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.seTestGoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nTestGold := seTestGold.Value;
  ModValue();
end;

procedure TfrmGameConfig.seTestUserLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nTestUserLimit := seTestUserLimit.Value;
  ModValue();
end;

procedure TfrmGameConfig.seStartPermissionChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStartPermission := seStartPermission.Value;
  ModValue();
end;

procedure TfrmGameConfig.seUserFullChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUserFull := seUserFull.Value;
  ModValue();
end;

procedure TfrmGameConfig.seHumanMaxGoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumanMaxGold := seHumanMaxGold.Value;
  ModValue();
end;

procedure TfrmGameConfig.seHumanTryModeMaxGoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumanTryModeMaxGold := seHumanTryModeMaxGold.Value;
  ModValue();
end;

procedure TfrmGameConfig.seTryModeLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nTryModeLevel := seTryModeLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxTryModeUseStorageClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boTryModeUseStorage := CheckBoxTryModeUseStorage.Checked;
  ModValue();
end;

procedure TfrmGameConfig.ButtonOptionSave0Click(Sender: TObject);
begin
  Config.WriteString('Server', 'TestServer', BoolToStr(g_Config.boTestServer));
  Config.WriteInteger('Server', 'TestLevel', g_Config.nTestLevel);
  Config.WriteInteger('Server', 'TestGold', g_Config.nTestGold);
  Config.WriteInteger('Server', 'TestServerUserLimit', g_Config.nTestUserLimit);
  Config.WriteString('Server', 'ServiceMode', BoolToStr(g_Config.boServiceMode));
  Config.WriteString('Server', 'NonPKServer', BoolToStr(g_Config.boNonPKServer));
  Config.WriteString('Server', 'VentureServer', BoolToStr(g_Config.boVentureServer));
  Config.WriteInteger('Setup', 'StartPermission', g_Config.nStartPermission);
  Config.WriteInteger('Server', 'UserFull', g_Config.nUserFull);
  Config.WriteInteger('Setup', 'HumChgMapOrLoginProtectTime', g_Config.dwHumChgMapOrLoginProtectTime);

  Config.WriteInteger('Setup', 'HumanMaxGold', g_Config.nHumanMaxGold);
  Config.WriteInteger('Setup', 'HumanTryModeMaxGold', g_Config.nHumanTryModeMaxGold);
  Config.WriteInteger('Setup', 'TryModeLevel', g_Config.nTryModeLevel);
  Config.WriteBool('Setup', 'TryModeUseStorage', g_Config.boTryModeUseStorage);
  Config.WriteInteger('Setup', 'GroupMembersMax', g_Config.nGroupMembersMax);
  Config.WriteInteger('Setup', 'MaxLevel', g_Config.btMaxLevel);
  Config.WriteInteger('Setup', 'MaxAC', g_Config.btMaxAC);
  Config.WriteInteger('Setup', 'MaxHitPoint', g_Config.btMaxHitPoint);

  // 行会人数限制 piaoyun 2013-07-17
  Config.WriteInteger('Setup', 'GuildMemberMaxLimit', g_Config.nGuildMemberMaxLimit);
  // 离线挂机禁止项 --- piaoyun 2013-07-17
  // 商店
  Config.WriteBool('Setup', 'OffLineShop', g_Config.boOffLineShop);
  // 英雄
  Config.WriteBool('Setup', 'OffLineHero', g_Config.boOffLineHero);
  // 宝宝
  Config.WriteBool('Setup', 'OffLineSlave', g_Config.boOffLineSlave);

  Config.WriteInteger('Setup', 'GuildNameLen', g_Config.nGuildNameLen);
  Config.WriteInteger('Setup', 'GuildRankNameLen', g_Config.nGuildRankNameLen);

  if boSendServerConfig then
    UserEngine.SendServerConfig();

  uModValue();
end;

procedure TfrmGameConfig.EditSayMsgMaxLenChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSayMsgMaxLen := EditSayMsgMaxLen.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.EditSayRedMsgMaxLenChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSayRedMsgMaxLen := EditSayRedMsgMaxLen.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCanShoutMsgLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCanShoutMsgLevel := EditCanShoutMsgLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditSayMsgTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSayMsgTime := EditSayMsgTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.EditSayMsgCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSayMsgCount := EditSayMsgCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditDisableSayMsgTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwDisableSayMsgTime := EditDisableSayMsgTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxShutRedMsgShowGMNameClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShutRedMsgShowGMName := CheckBoxShutRedMsgShowGMName.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxShowPreFixMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowPreFixMsg := CheckBoxShowPreFixMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.EditGMRedMsgCmdChange(Sender: TObject);
var
  sCmd: string;
begin
  if not boOpened then
    Exit;
  sCmd := EditGMRedMsgCmd.Text;
  if sCmd <> '' then
    g_GMRedMsgCmd := sCmd[1]
  else
    g_GMRedMsgCmd := #0;
  ModValue();
end;

procedure TfrmGameConfig.ButtonMsgSaveClick(Sender: TObject);
var
  sShowWhisperLevelMsg: string;
begin
  sShowWhisperLevelMsg := EditShowWhisperLevelMsg.Text;
  if Pos('%u', sShowWhisperLevelMsg) <= 0 then
  begin
    Application.MessageBox('私聊后缀信息设置错误！', '错误信息', MB_OK + MB_ICONERROR);
    EditShowWhisperLevelMsg.SetFocus;
    Exit;
  end;
  g_sShowWhisperLevelMsg := sShowWhisperLevelMsg;

  Config.WriteInteger('Setup', 'SayMsgMaxLen', g_Config.nSayMsgMaxLen);
  Config.WriteInteger('Setup', 'SayMsgTime', g_Config.dwSayMsgTime);
  Config.WriteInteger('Setup', 'SayMsgCount', g_Config.nSayMsgCount);
  Config.WriteInteger('Setup', 'SayRedMsgMaxLen', g_Config.nSayRedMsgMaxLen);
  Config.WriteBool('Setup', 'ShutRedMsgShowGMName', g_Config.boShutRedMsgShowGMName);
  Config.WriteInteger('Setup', 'CanShoutMsgLevel', g_Config.nCanShoutMsgLevel);
  CommandConf.WriteString('Command', 'GMRedMsgCmd', g_GMRedMsgCmd);
  Config.WriteBool('Setup', 'ShowPreFixMsg', g_Config.boShowPreFixMsg);
  Config.WriteBool('Setup', 'ShowWhisperLevelMsg', g_Config.boShowWhisperLevelMsg);
  StringConf.WriteString('String', 'ShowWhisperLevelMsg', g_sShowWhisperLevelMsg);
  Config.WriteInteger('Setup', 'UserItemSayMsgTime', g_Config.nUserItemSayMsgTime);
  Config.WriteInteger('Setup', 'DisableSayMsgTime', g_Config.dwDisableSayMsgTime);
  Config.WriteInteger('Setup', 'MaxInputStringLen', g_Config.nMaxInputStringLen);

  if boSendServerConfig then
    UserEngine.SendServerConfig();

  uModValue();
end;

procedure TfrmGameConfig.EditStartCastleWarDaysChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStartCastleWarDays := EditStartCastleWarDays.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditStartCastlewarTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStartCastlewarTime := EditStartCastlewarTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditShowCastleWarEndMsgTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwShowCastleWarEndMsgTime := EditShowCastleWarEndMsgTime.Value * (60 * 1000);
  ModValue();
end;

procedure TfrmGameConfig.EditCastleWarTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwCastleWarTime := EditCastleWarTime.Value * (60 * 1000);
  ModValue();
end;

procedure TfrmGameConfig.EditGetCastleTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwGetCastleTime := EditGetCastleTime.Value * (60 * 1000);
  ModValue();
end;

procedure TfrmGameConfig.EditGuildWarTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwGuildWarTime := EditGuildWarTime.Value * (60 * 1000);
  ModValue();
end;

procedure TfrmGameConfig.seMakeMonGhostTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMakeMonGhostTime := seMakeMonGhostTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.seMakeDummyGhostTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMakeDummyGhostTime := seMakeDummyGhostTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.seClearDropOnFloorItemTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwClearDropOnFloorItemTime := seClearDropOnFloorItemTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.seSaveHumanRcdTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSaveHumanRcdTime := seSaveHumanRcdTime.Value * (60 * 1000);
  ModValue();
end;

procedure TfrmGameConfig.seHumanFreeDelayTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHumanFreeDelayTime := seHumanFreeDelayTime.Value * (60 * 1000);
  ModValue();
end;

procedure TfrmGameConfig.seFloorItemCanPickUpTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwFloorItemCanPickUpTime := seFloorItemCanPickUpTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.ButtonTimeSaveClick(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'StartCastleWarDays', g_Config.nStartCastleWarDays);
  Config.WriteInteger('Setup', 'StartCastlewarTime', g_Config.nStartCastlewarTime);
  Config.WriteInteger('Setup', 'ShowCastleWarEndMsgTime', g_Config.dwShowCastleWarEndMsgTime);
  Config.WriteInteger('Setup', 'CastleWarTime', g_Config.dwCastleWarTime);
  Config.WriteInteger('Setup', 'GetCastleTime', g_Config.dwGetCastleTime);
  Config.WriteInteger('Setup', 'GuildWarTime', g_Config.dwGuildWarTime);
  Config.WriteInteger('Setup', 'SaveHumanRcdTime', g_Config.dwSaveHumanRcdTime);
  Config.WriteInteger('Setup', 'HumanFreeDelayTime', g_Config.dwHumanFreeDelayTime);
  // 数据读取保存超时间隔 -- piaoyun 2013-07-17
  Config.WriteInteger('Setup', 'GetDBSockMsgTime', g_Config.dwGetDBSockMsgTime);

  Config.WriteInteger('Setup', 'MakeGhostTime', g_Config.dwMakeGhostTime);
  Config.WriteInteger('Setup', 'MakeMonGhostTime', g_Config.dwMakeMonGhostTime);
  Config.WriteInteger('Setup', 'MakeDummyGhostTime', g_Config.dwMakeDummyGhostTime);
  Config.WriteInteger('Setup', 'ClearDropOnFloorItemTime', g_Config.dwClearDropOnFloorItemTime);
  Config.WriteInteger('Setup', 'FloorItemCanPickUpTime', g_Config.dwFloorItemCanPickUpTime);

  Config.WriteInteger('Setup', 'DearRecallTime', g_Config.dwDearRecallTime);
  // 夫妻传送时间间隔 chongchong 2013-07-22
  Config.WriteInteger('Setup', 'DearRecallTime', g_Config.dwMasterRecallTime);
  // 师徒传送时间间隔 chongchong 2013-07-22
  Config.WriteInteger('Setup', 'GroupRecallTime', g_Config.dwGroupRecallTime);
  // 记忆传送时间间隔 chongchong 2013-07-22

  Config.WriteInteger('Setup', 'HorseTakeTime', g_Config.dwHorseTakeTime);
  // 上下马时间间隔 chongchong 2013-10-19
  Config.WriteInteger('Setup', 'TakeOnHorseUseTime', g_Config.dwTakeOnHorseUseTime);
  Config.WriteBool('Setup', 'ReadyOnHorseDisableAction', g_Config.boReadyOnHorseDisableAction);

  Config.WriteInteger('Setup', 'NpcButtonClickTime', g_Config.dwNpcButtonClickTime);
  Config.WriteInteger('Setup', 'NpcActorClickTime', g_Config.dwNpcActorClickTime);

  Config.WriteInteger('Setup', 'PlayerVarJClearTime', g_Config.btPlayerVarJClearTime);

  if boSendServerConfig then
    UserEngine.SendServerConfig();

  uModValue();
end;

procedure TfrmGameConfig.EditBuildGuildPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBuildGuildPrice := EditBuildGuildPrice.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditGuildWarPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGuildWarPrice := EditGuildWarPrice.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMakeDurgPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMakeDurgPrice := EditMakeDurgPrice.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditSuperRepairPriceRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSuperRepairPriceRate := EditSuperRepairPriceRate.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditRepairItemDecDuraChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRepairItemDecDura := EditRepairItemDecDura.Value;
  ModValue();
end;

procedure TfrmGameConfig.ButtonPriceSaveClick(Sender: TObject);
begin

  Config.WriteInteger('Setup', 'BuildGuild', g_Config.nBuildGuildPrice);
  Config.WriteInteger('Setup', 'MakeDurg', g_Config.nMakeDurgPrice);
  Config.WriteInteger('Setup', 'GuildWarFee', g_Config.nGuildWarPrice);
  Config.WriteInteger('Setup', 'SuperRepairPriceRate', g_Config.nSuperRepairPriceRate);
  Config.WriteInteger('Setup', 'RepairItemDecDura', g_Config.nRepairItemDecDura);
  Config.WriteBool('Setup', 'SellItemToNpcShopNoCalcAddProperty', g_Config.boSellItemToNpcShopNoCalcAddProperty);
  Config.WriteBool('Setup', 'ShowNewValueFromBuyNpcItem', g_Config.boShowNewValueFromBuyNpcItem);
  uModValue();
end;

procedure TfrmGameConfig.EditHearMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditHearMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHearMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EdittHearMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EdittHearMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHearMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditWhisperMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditWhisperMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btWhisperMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditWhisperMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditWhisperMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btWhisperMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditGMWhisperMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditGMWhisperMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGMWhisperMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditGMWhisperMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditGMWhisperMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGMWhisperMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditRedMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditRedMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btRedMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditRedMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditRedMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btRedMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditGreenMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditGreenMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGreenMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditGreenMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditGreenMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGreenMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditBlueMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditBlueMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btBlueMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditBlueMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditBlueMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btBlueMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditCryMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditCryMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btCryMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditCryMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditCryMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btCryMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditGuildMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditGuildMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGuildMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditGuildMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditGuildMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGuildMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditGroupMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditGroupMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGroupMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditGroupMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditGroupMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGroupMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditCustMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditCustMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btCustMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditCustMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditCustMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btCustMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.ButtonMsgColorSaveClick(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'HearMsgFColor', g_Config.btHearMsgFColor);
  Config.WriteInteger('Setup', 'HearMsgBColor', g_Config.btHearMsgBColor);
  Config.WriteInteger('Setup', 'WhisperMsgFColor', g_Config.btWhisperMsgFColor);
  Config.WriteInteger('Setup', 'WhisperMsgBColor', g_Config.btWhisperMsgBColor);
  Config.WriteInteger('Setup', 'GMWhisperMsgFColor', g_Config.btGMWhisperMsgFColor);
  Config.WriteInteger('Setup', 'GMWhisperMsgBColor', g_Config.btGMWhisperMsgBColor);
  Config.WriteInteger('Setup', 'SendWhisperMsgFColor', g_Config.btSendWhisperMsgFColor);
  Config.WriteInteger('Setup', 'SendWhisperMsgBColor', g_Config.btSendWhisperMsgBColor);
  Config.WriteInteger('Setup', 'RefreshGameGoldFColor', g_Config.btRefreshGameGoldFColor);
  Config.WriteInteger('Setup', 'RefreshGameGoldBColor', g_Config.btRefreshGameGoldBColor);

  Config.WriteInteger('Setup', 'ShowWhisperFColor', g_Config.btShowWhisperFColor);
  Config.WriteInteger('Setup', 'ShowWhisperBColor', g_Config.btShowWhisperBColor);

  Config.WriteInteger('Setup', 'CloseWhisperFColor', g_Config.btCloseWhisperFColor);
  Config.WriteInteger('Setup', 'CloseWhisperBColor', g_Config.btCloseWhisperBColor);

  Config.WriteInteger('Setup', 'CryMsgFColor', g_Config.btCryMsgFColor);
  Config.WriteInteger('Setup', 'CryMsgBColor', g_Config.btCryMsgBColor);
  Config.WriteInteger('Setup', 'GreenMsgFColor', g_Config.btGreenMsgFColor);
  Config.WriteInteger('Setup', 'GreenMsgBColor', g_Config.btGreenMsgBColor);
  Config.WriteInteger('Setup', 'BlueMsgFColor', g_Config.btBlueMsgFColor);
  Config.WriteInteger('Setup', 'BlueMsgBColor', g_Config.btBlueMsgBColor);
  Config.WriteInteger('Setup', 'RedMsgFColor', g_Config.btRedMsgFColor);
  Config.WriteInteger('Setup', 'RedMsgBColor', g_Config.btRedMsgBColor);
  Config.WriteInteger('Setup', 'GuildMsgFColor', g_Config.btGuildMsgFColor);
  Config.WriteInteger('Setup', 'GuildMsgBColor', g_Config.btGuildMsgBColor);
  Config.WriteInteger('Setup', 'GroupMsgFColor', g_Config.btGroupMsgFColor);
  Config.WriteInteger('Setup', 'GroupMsgBColor', g_Config.btGroupMsgBColor);
  Config.WriteInteger('Setup', 'CustMsgFColor', g_Config.btCustMsgFColor);
  Config.WriteInteger('Setup', 'CustMsgBColor', g_Config.btCustMsgBColor);

  Config.WriteInteger('Setup', 'NationMsgBColor', g_Config.btNationMsgBColor);
  Config.WriteInteger('Setup', 'NationMsgFColor', g_Config.btNationMsgFColor);

  Config.WriteInteger('Setup', 'UserSayMsgFColor', g_Config.btUserSayMsgFColor);
  Config.WriteInteger('Setup', 'UserSayMsgBColor', g_Config.btUserSayMsgBColor);
  Config.WriteInteger('Setup', 'TopUserSayMsgFColor', g_Config.btTopUserSayMsgFColor);
  Config.WriteInteger('Setup', 'TopUserSayMsgBColor', g_Config.btTopUserSayMsgBColor);

  Config.WriteInteger('Setup', 'DropItemFColor', g_Config.btDropItemFColor);
  Config.WriteInteger('Setup', 'DropItemBColor', g_Config.btDropItemBColor);

  Config.WriteInteger('Setup', 'NPCLabelNormalColor', g_Config.btNPCLabelNormalColor);
  Config.WriteBool('Setup', 'NPCLabelFontStroke', g_Config.boNPCLabelFontStroke);
  Config.WriteInteger('Setup', 'NPCLabelMouseMoveColor', g_Config.btNPCLabelMouseMoveColor);
  Config.WriteInteger('Setup', 'NPCLabelMouseDownColor', g_Config.btNPCLabelMouseDownColor);
  uModValue();
  UserEngine.SendServerConfig();
end;

procedure TfrmGameConfig.ButtonHumanDieSaveClick(Sender: TObject);
begin
  Config.WriteBool('Setup', 'DieScatterBag', g_Config.boDieScatterBag);
  Config.WriteInteger('Setup', 'DieScatterBagRate', g_Config.nDieScatterBagRate);
  Config.WriteBool('Setup', 'DieRedScatterBagAll', g_Config.boDieRedScatterBagAll);
  Config.WriteInteger('Setup', 'DieDropUseItemRate', g_Config.nDieDropUseItemRate);
  Config.WriteInteger('Setup', 'DieRedDropUseItemRate', g_Config.nDieRedDropUseItemRate);
  Config.WriteBool('Setup', 'DieDropGold', g_Config.boDieDropGold);
  Config.WriteBool('Setup', 'KillByHumanDropUseItem', g_Config.boKillByHumanDropUseItem);
  Config.WriteBool('Setup', 'KillByMonstDropUseItem', g_Config.boKillByMonstDropUseItem);

  Config.WriteBool('Setup', 'KillByMonstDropJewelryBoxItem', g_Config.boKillByMonstDropJewelryBoxItem);
  Config.WriteBool('Setup', 'KillByHumanDropJewelryBoxItem', g_Config.boKillByHumanDropJewelryBoxItem);

  Config.WriteBool('Setup', 'KillByMonstDropGodBlessItem', g_Config.boKillByMonstDropGodBlessItem);
  Config.WriteBool('Setup', 'KillByHumanDropGodBlessItem', g_Config.boKillByHumanDropGodBlessItem);

  Config.WriteInteger('Setup', 'DieScatterBagRate', g_Config.nDieScatterBagRate);
  Config.WriteInteger('Setup', 'DieScatterBagRate', g_Config.nDieScatterBagRate);

  Config.WriteInteger('Setup', 'DropJewelryBoxItemRate', g_Config.nDropJewelryBoxItemRate);
  Config.WriteInteger('Setup', 'DropGodBlessItemRate', g_Config.nDropGodBlessItemRate);

  Config.WriteInteger('Setup', 'DropUseItemsMaxCount', g_Config.nDropUseItemsMaxCount);

  Config.WriteInteger('Setup', 'ScatterBagItemsMinLevel', g_Config.nScatterBagItemsMinLevel);

  uModValue();
end;

procedure TfrmGameConfig.ScrollBarDieDropUseItemRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarDieDropUseItemRate.Position;
  EditDieDropUseItemRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDieDropUseItemRate := nPostion;
  ModValue();
end;

procedure TfrmGameConfig.ScrollBarDieRedDropUseItemRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarDieRedDropUseItemRate.Position;
  EditDieRedDropUseItemRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDieRedDropUseItemRate := nPostion;
  ModValue();
end;

procedure TfrmGameConfig.ScrollBarDieScatterBagRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarDieScatterBagRate.Position;
  EditDieScatterBagRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDieScatterBagRate := nPostion;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxKillByMonstDropUseItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByMonstDropUseItem := CheckBoxKillByMonstDropUseItem.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxKillByHumanDropUseItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByHumanDropUseItem := CheckBoxKillByHumanDropUseItem.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxDieScatterBagClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDieScatterBag := CheckBoxDieScatterBag.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxDieDropGoldClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDieDropGold := CheckBoxDieDropGold.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxDieRedScatterBagAllClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDieRedScatterBagAll := CheckBoxDieRedScatterBagAll.Checked;
  ModValue();
end;

procedure TfrmGameConfig.RefCharStatusConf;
var
  I: Integer;
begin
  CheckBoxParalyCanRun.Checked := g_Config.boParalyCanRun;
  CheckBoxParalyCanWalk.Checked := g_Config.boParalyCanWalk;
  CheckBoxParalyCanHit.Checked := g_Config.boParalyCanHit;
  CheckBoxParalyCanSpell.Checked := g_Config.boParalyCanSpell;

  for I := 0 to Length(g_Config.AttatckModes) - 1 do
  begin
    CheckGroupAttatckMode.ItemChecked[I] := g_Config.AttatckModes[I];
  end;
end;

procedure TfrmGameConfig.ButtonCharStatusSaveClick(Sender: TObject);
var
  I: Integer;
begin
  Config.WriteBool('Setup', 'ParalyCanRun', g_Config.boParalyCanRun);
  Config.WriteBool('Setup', 'ParalyCanWalk', g_Config.boParalyCanWalk);
  Config.WriteBool('Setup', 'ParalyCanHit', g_Config.boParalyCanHit);
  Config.WriteBool('Setup', 'ParalyCanSpell', g_Config.boParalyCanSpell);
  for I := 0 to Length(g_Config.AttatckModes) - 1 do
  begin
    Config.WriteBool('Setup', 'AttatckModes' + IntToStr(I), g_Config.AttatckModes[I]);
  end;
  uModValue();
end;

procedure TfrmGameConfig.CheckBoxParalyCanRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boParalyCanRun := CheckBoxParalyCanRun.Checked;
  ModValue();
  UserEngine.SendServerConfig();
end;

procedure TfrmGameConfig.CheckBoxParalyCanWalkClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boParalyCanWalk := CheckBoxParalyCanWalk.Checked;
  ModValue();
  UserEngine.SendServerConfig();
end;

procedure TfrmGameConfig.CheckBoxParalyCanHitClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boParalyCanHit := CheckBoxParalyCanHit.Checked;
  ModValue();
  UserEngine.SendServerConfig();
end;

procedure TfrmGameConfig.CheckBoxParalyCanSpellClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boParalyCanSpell := CheckBoxParalyCanSpell.Checked;
  ModValue();
  UserEngine.SendServerConfig();
end;

procedure TfrmGameConfig.ButtonActionSpeedConfigClick(Sender: TObject);
begin
  frmActionSpeed := TfrmActionSpeed.Create(Owner);
  frmActionSpeed.Top := Top + 20;
  frmActionSpeed.Left := Left;
  frmActionSpeed.Open;
  frmActionSpeed.Free;
end;

procedure TfrmGameConfig.chkSafeAreaClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSafeAreaLimited := chkSafeArea.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxFixExpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boUseFixExp := CheckBoxFixExp.Checked;
  SpinEditBaseExp.Enabled := not CheckBoxFixExp.Checked;
  SpinEditAddExp.Enabled := not CheckBoxFixExp.Checked;
  ModValue();
end;

procedure TfrmGameConfig.SpinEditBaseExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBaseExp := SpinEditBaseExp.Value;
  ModValue();
end;

procedure TfrmGameConfig.SpinEditAddExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAddExp := SpinEditAddExp.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxHighLevelGroupFixExpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHighLevelGroupFixExp := CheckBoxHighLevelGroupFixExp.Checked;
  ModValue();
end;

procedure TfrmGameConfig.RadioGroupMaxLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMaxLevel := RadioGroupMaxLevel.ItemIndex;
  ModValue();
end;

procedure TfrmGameConfig.rgMaxACClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMaxAC := rgMaxAC.ItemIndex;
  ModValue();
end;

procedure TfrmGameConfig.EditUserSayMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditUserSayMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btUserSayMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditUserSayMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditUserSayMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btUserSayMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditTopUserSayMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditTopUserSayMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btTopUserSayMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditTopUserSayMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditTopUserSayMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btTopUserSayMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditHighLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHighLevel := EditHighLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditHighLevelGetExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHighLevelGetExp := EditHighLevelGetExp.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxLimitChangeExpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLimitChangeExp := CheckBoxLimitChangeExp.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxCheckActionCountClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCheckActionCount := CheckBoxCheckActionCount.Checked;
  ModValue();
end;

procedure TfrmGameConfig.EditHitCountIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHitCountIntervalTime := EditHitCountIntervalTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMagicHitCountIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMagicHitCountIntervalTime := EditMagicHitCountIntervalTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMoveCountIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMoveCountIntervalTime := EditMoveCountIntervalTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCanHitCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCanHitCount := EditCanHitCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCanMagicHitCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCanMagicHitCount := EditCanMagicHitCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCanMoveCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCanMoveCount := EditCanMoveCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.ButtonCheckActionDefaultClick(Sender: TObject);
begin
  if Application.MessageBox('是否确认恢复默认设置？', '确认信息', MB_YESNO + MB_ICONQUESTION) <> IDYES then
  begin
    Exit;
  end;
  g_Config.dwHitCountIntervalTime := 800;
  g_Config.dwMagicHitCountIntervalTime := 1500;
  g_Config.dwMoveCountIntervalTime := 800;

  g_Config.nCanHitCount := 3;
  g_Config.nCanMagicHitCount := 3;
  g_Config.nCanMoveCount := 3;

  g_Config.nCheckHitCount := 4; // 攻击次数
  g_Config.nCheckMagicHitCount := 3; // 魔法次数
  g_Config.nCheckMoveCount := 4; // 移动次数

  g_Config.boCheckActionCount := True;

  RefGameSpeedConf();
  ModValue();
end;

procedure TfrmGameConfig.ButtonCheckActionSaveClick(Sender: TObject);
begin
  Config.WriteBool('Setup', 'CheckActionCount', g_Config.boCheckActionCount);
  Config.WriteInteger('Setup', 'HitCountIntervalTime', g_Config.dwHitCountIntervalTime);
  Config.WriteInteger('Setup', 'MagicHitCountIntervalTime', g_Config.dwMagicHitCountIntervalTime);
  Config.WriteInteger('Setup', 'MoveCountIntervalTime', g_Config.dwMoveCountIntervalTime);
  Config.WriteInteger('Setup', 'CanHitCount', g_Config.nCanHitCount);
  Config.WriteInteger('Setup', 'CanMagicHitCount', g_Config.nCanMagicHitCount);
  Config.WriteInteger('Setup', 'CanMoveCount', g_Config.nCanMoveCount);

  Config.WriteInteger('Setup', 'CheckHitCount', g_Config.nCheckHitCount);
  Config.WriteInteger('Setup', 'CheckMagicHitCount', g_Config.nCheckMagicHitCount);
  Config.WriteInteger('Setup', 'CheckMoveCount', g_Config.nCheckMoveCount);

  uModValue();
end;

procedure TfrmGameConfig.EditCheckHitCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCheckHitCount := EditCheckHitCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCheckMagicHitCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCheckMagicHitCount := EditCheckMagicHitCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditCheckMoveCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCheckMoveCount := EditCheckMoveCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.seTryChallengeTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwTryChallengeTime := seTryChallengeTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.seChallengeOKTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwChallengeOKTime := seChallengeOKTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.chkCanNotGetBackChallengeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCanNotGetBackChallenge := chkCanNotGetBackChallenge.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkDisableChallengeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableChallenge := chkDisableChallenge.Checked;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxShowWhisperLevelMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowWhisperLevelMsg := CheckBoxShowWhisperLevelMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.EditShowWhisperLevelMsgChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmGameConfig.ScrollBarDieDropUseItemRate0Change(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := TScrollBar(Sender).Position;
  TEdit(FindComponent('edtDieDropUseItemRate' + IntToStr(TScrollBar(Sender).Tag))).Text := IntToStr(nPostion);

  if not boOpened then
    Exit;
  g_Config.DieDropUseItemRates[TScrollBar(Sender).Tag] := nPostion;
  ModValue();
end;

procedure TfrmGameConfig.ButtonDieDropUseItemSaveClick(Sender: TObject);
var
  I: Integer;
begin
  Config.WriteBool('Setup', 'DropUseItem', g_Config.boDropUseItem);
  Config.WriteInteger('Setup', 'DieRedDropUseItemOneRate', g_Config.nDieRedDropUseItemOneRate);

  for I := Low(g_Config.DieDropUseItemRates) to High(g_Config.DieDropUseItemRates) do
  begin
    Config.WriteInteger('Setup', 'DieDropUseItemRates' + IntToStr(I), g_Config.DieDropUseItemRates[I]);
  end;

  uModValue();
end;

procedure TfrmGameConfig.CheckBoxDropUseItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDropUseItem := CheckBoxDropUseItem.Checked;
  ModValue();
end;

procedure TfrmGameConfig.EditDieRedDropUseItemOneRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDieRedDropUseItemOneRate := EditDieRedDropUseItemOneRate.Value;
  ModValue();
end;

procedure TfrmGameConfig.CheckBoxSendUpdateMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSendUpdateMsg := CheckBoxSendUpdateMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxHitDeliveryTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxHitDeliveryTime := EditMaxHitDeliveryTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxMagicHitDeliveryTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxMagicHitDeliveryTime := EditMaxMagicHitDeliveryTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxRunDeliveryTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxRunDeliveryTime := EditMaxRunDeliveryTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxWalkDeliveryTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxWalkDeliveryTime := EditMaxWalkDeliveryTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxTurnDeliveryTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxTurnDeliveryTime := EditMaxTurnDeliveryTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxDigUpDeliveryTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxDigUpDeliveryTime := EditMaxDigUpDeliveryTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditDigUpIntervalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwDigUpIntervalTime := EditDigUpIntervalTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.EditDropItemFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditDropItemFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btDropItemFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditDropItemBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditDropItemBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btDropItemBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditUserItemSayMsgTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUserItemSayMsgTime := EditUserItemSayMsgTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditNationMsgFColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditNationMsgFColor.Value;
  if not boOpened then
    Exit;
  g_Config.btNationMsgFColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.EditNationMsgBColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditNationMsgBColor.Value;
  if not boOpened then
    Exit;
  g_Config.btNationMsgBColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.CheckGroupAttatckModeChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
var
  I: Integer;
  boFindOK: Boolean;
begin
  if not boOpened then
    Exit;
  if NewState = cbUnchecked then
  begin
    g_Config.AttatckModes[Index] := False;
    boFindOK := False;
    for I := 0 to Length(g_Config.AttatckModes) - 1 do
    begin
      if g_Config.AttatckModes[I] then
      begin
        boFindOK := True;
        Break;
      end;
    end;
    if not boFindOK then
    begin
      Application.MessageBox('最少要选择一种攻击模式', '错误信息', MB_OK + MB_ICONERROR);
      CheckGroupAttatckMode.ItemChecked[Index] := True;
      g_Config.AttatckModes[Index] := True;
      Exit;
    end;
  end
  else
  begin
    g_Config.AttatckModes[Index] := True;
  end;
  ModValue();
end;

procedure TfrmGameConfig.EditScatterBagItemsMinLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nScatterBagItemsMinLevel := EditScatterBagItemsMinLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditDropUseItemsMaxCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDropUseItemsMaxCount := EditDropUseItemsMaxCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.EditMaxUpLevelCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxUpLevelCount := EditMaxUpLevelCount.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkOldClientClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boIsOldClient := chkOldClient.Checked;
  ModValue();
end;

procedure TfrmGameConfig.seNPCLabelMouseMoveColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seNPCLabelMouseMoveColor.Value;
  if not boOpened then
    Exit;
  g_Config.btNPCLabelMouseMoveColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.seNPCLabelMouseDownColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seNPCLabelMouseDownColor.Value;
  if not boOpened then
    Exit;
  g_Config.btNPCLabelMouseDownColor := btColor;
  ModValue();
end;

procedure TfrmGameConfig.seGuildMemberMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGuildMemberMaxLimit := seGuildMemberMaxLimit.Value;
  ModValue();
end;

procedure TfrmGameConfig.seMakeGhostTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMakeGhostTime := seMakeGhostTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.seGetDBSockMsgTimeChange(Sender: TObject);
begin
  // 数据读取保存超时间隔 -- piaoyun 2013-07-17
  if not boOpened then
    Exit;
  g_Config.dwGetDBSockMsgTime := seGetDBSockMsgTime.Value * 1000;
  ModValue();
end;

procedure TfrmGameConfig.chkOffLineShopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOffLineShop := chkOffLineShop.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkOffLineHeroClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOffLineHero := chkOffLineHero.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkOffLineSlaveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOffLineSlave := chkOffLineSlave.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkWarHreoRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boWarHreoRun := chkWarHreoRun.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkSafeAreaDisNpcRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSafeAreaDisNpcRun := chkSafeAreaDisNpcRun.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkSafeAreaDisShopStallHumRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSafeAreaDisShopStallHumRun := chkSafeAreaDisShopStallHumRun.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkSafeAreaDisOffLineHumRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSafeAreaDisOffLineHumRun := chkSafeAreaDisOffLineHumRun.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkWarDisTeleportClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boWarDisTeleport := chkWarDisTeleport.Checked;
  ModValue();
end;

procedure TfrmGameConfig.seChallengeTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwChallengeTime := seChallengeTime.Value * 1000 * 60;
  ModValue();
end;

procedure TfrmGameConfig.rgChallengeGoldClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btChallengeGoldIndex := Byte(rgChallengeGold.ItemIndex);
  ModValue();
end;

procedure TfrmGameConfig.seDearRecallTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwDearRecallTime := seDearRecallTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.seMasterRecallTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMasterRecallTime := seMasterRecallTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.seGroupRecallTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwGroupRecallTime := seGroupRecallTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkRecordPublicMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRecordPublicMsg := chkRecordPublicMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkRecordPrivateMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRecordPrivateMsg := chkRecordPrivateMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkRecordGuildMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRecordGuildMsg := chkRecordGuildMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkRecordCryCryMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRecordCryCryMsg := chkRecordCryCryMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkRecordGroupMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRecordGroupMsg := chkRecordGroupMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkShareExpGroupSameScreenClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  chkShareExpGroupSameMap.Enabled := chkShareExpGroupSameScreen.Checked;
  g_Config.boShareExpGroupSameScreen := chkShareExpGroupSameScreen.Checked;
  g_Config.boShareExpGroupSameMap := chkShareExpGroupSameMap.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkShareExpGroupSameMapClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShareExpGroupSameMap := chkShareExpGroupSameMap.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkShareExpHeroSameMapClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShareExpHeroSameMap := chkShareExpHeroSameMap.Checked;
  ModValue();
end;

procedure TfrmGameConfig.seKillHumanWeaponUnlockRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwKillHumanWeaponUnlockRate := seKillHumanWeaponUnlockRate.Value;
  ModValue();
end;

procedure TfrmGameConfig.seGuildNameLenChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGuildNameLen := seGuildNameLen.Value;
  ModValue();
end;

procedure TfrmGameConfig.seGuildRankNameLenChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGuildRankNameLen := seGuildRankNameLen.Value;
  ModValue();
end;

procedure TfrmGameConfig.seHintSafeZoneYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHintSafeZoneY := seHintSafeZoneY.Value;
  ModValue();
end;

procedure TfrmGameConfig.seHintSafeZoneFColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btHintSafeZoneFColor := seHintSafeZoneFColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.seHintSafeZoneBColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btHintSafeZoneBColor := seHintSafeZoneBColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.seHintSafeZoneFSizeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btHintSafeZoneFSize := seHintSafeZoneFSize.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkHorseRun3GridClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHorseRun3Grid := chkHorseRun3Grid.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.seHorseTakeTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHorseTakeTime := seHorseTakeTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.seTakeOnHorseUseTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwTakeOnHorseUseTime := seTakeOnHorseUseTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.seNpcButtonClickTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  boSendServerConfig := True;
  g_Config.dwNpcButtonClickTime := seNpcButtonClickTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.seDummyAddPKPointChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDummyAddPKPoint := seDummyAddPKPoint.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkSpeedControlClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSpeedControl := chkSpeedControl.Checked;
  boSendServerConfig := True;
  RefGlobalSpeedCtrl;
  ModValue();
end;

procedure TfrmGameConfig.RefGlobalSpeedCtrl;

  procedure SetCtrlEnable(ParentCtrl: TWinControl; IsEnabled: Boolean);
  var
    I: Integer;
    WinCtrl: TWinControl;
  begin
    for I := 0 to ParentCtrl.ControlCount - 1 do
    begin
      if ParentCtrl.Controls[I] is TWinControl then
      begin
        WinCtrl := ParentCtrl.Controls[I] as TWinControl;
        WinCtrl.Enabled := IsEnabled;
      end;
    end;
  end;

begin
  Exit;
  SetCtrlEnable(GroupBox1, g_Config.boSpeedControl);
  SetCtrlEnable(GroupBox2, g_Config.boSpeedControl);
  SetCtrlEnable(GroupBox15, g_Config.boSpeedControl);
  SetCtrlEnable(GroupBox4, g_Config.boSpeedControl);
  SetCtrlEnable(GroupBox83, g_Config.boSpeedControl);
  ButtonActionSpeedConfig.Enabled := g_Config.boSpeedControl;
end;

procedure TfrmGameConfig.seNpcActorClickTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  boSendServerConfig := True;
  g_Config.dwNpcActorClickTime := seNpcActorClickTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkReadyOnHorseDisableActionClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boReadyOnHorseDisableAction := chkReadyOnHorseDisableAction.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkKillByMonstDropJewelryBoxItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByMonstDropJewelryBoxItem := chkKillByMonstDropJewelryBoxItem.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkKillByHumanDropJewelryBoxItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByHumanDropJewelryBoxItem := chkKillByHumanDropJewelryBoxItem.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkKillByMonstDropGodBlessItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByMonstDropGodBlessItem := chkKillByMonstDropGodBlessItem.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkKillByHumanDropGodBlessItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByHumanDropGodBlessItem := chkKillByHumanDropGodBlessItem.Checked;
  ModValue();
end;

procedure TfrmGameConfig.scrlbrJewelryBoxItemChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := scrlbrJewelryBoxItem.Position;
  edtJewelryBoxItem.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDropJewelryBoxItemRate := nPostion;
  ModValue();
end;

procedure TfrmGameConfig.scrlbrGodBlessItemChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := scrlbrGodBlessItem.Position;
  edtGodBlessItem.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDropGodBlessItemRate := nPostion;
  ModValue();
end;

procedure TfrmGameConfig.chkMagicshieldStruckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMagicshieldStruck := chkMagicshieldStruck.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkHeroKillHumanNotWeaponUnlockClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroKillHumanNotWeaponUnlock := chkHeroKillHumanNotWeaponUnlock.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkNationGroupCheckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNationGroupCheck := chkNationGroupCheck.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkNationGuildCheckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNationGuildCheck := chkNationGuildCheck.Checked;
  ModValue();
end;

procedure TfrmGameConfig.seNationSayLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNationSayLevel := seNationSayLevel.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkRecordNationMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRecordNationMsg := chkRecordNationMsg.Checked;
  ModValue();
end;

procedure TfrmGameConfig.seMaxInputStringLenChange(Sender: TObject);
begin
  boSendServerConfig := True;
  if not boOpened then
    Exit;
  g_Config.nMaxInputStringLen := seMaxInputStringLen.Value;
  ModValue();
end;

procedure TfrmGameConfig.seHumChgMapOrLoginProtectTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHumChgMapOrLoginProtectTime := seHumChgMapOrLoginProtectTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkSellItemToNpcShopNoCalcAddPropertyClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSellItemToNpcShopNoCalcAddProperty := chkSellItemToNpcShopNoCalcAddProperty.Checked;
  ModValue();
end;

procedure TfrmGameConfig.chkShowNewValueFromBuyNpcItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowNewValueFromBuyNpcItem := chkShowNewValueFromBuyNpcItem.Checked;
  ModValue();
end;

procedure TfrmGameConfig.seKillHeroAddPKPointChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwKillHeroAddPKPoint := seKillHeroAddPKPoint.Value;
  ModValue();
end;

procedure TfrmGameConfig.seNPCLabelNormalColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btNPCLabelNormalColor := seNPCLabelNormalColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkNPCLabelFontStrokeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNPCLabelFontStroke := chkNPCLabelFontStroke.Checked;
  ModValue();
end;

procedure TfrmGameConfig.sePlayerVarJClearTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btPlayerVarJClearTime := sePlayerVarJClearTime.Value;
  ModValue();
end;

procedure TfrmGameConfig.rgMaxHitPointClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMaxHitPoint := rgMaxHitPoint.ItemIndex;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmGameConfig.seSendWhisperMsgFColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btSendWhisperMsgFColor := seSendWhisperMsgFColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.seSendWhisperMsgBColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btSendWhisperMsgBColor := seSendWhisperMsgBColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.seRefreshGameGoldFColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btRefreshGameGoldFColor := seRefreshGameGoldFColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.seRefreshGameGoldBColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btRefreshGameGoldBColor := seRefreshGameGoldBColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.seShowWhisperFColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowWhisperFColor := seShowWhisperFColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.seShowWhisperBColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btShowWhisperBColor := seShowWhisperBColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.seCloseWhisperFColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btCloseWhisperFColor := seCloseWhisperFColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.seCloseWhisperBColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btCloseWhisperBColor := seCloseWhisperBColor.Value;
  ModValue();
end;

procedure TfrmGameConfig.chkPermissionChangeLogClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPermissionChangeLog := chkPermissionChangeLog.Checked;
  ModValue();
end;

end.


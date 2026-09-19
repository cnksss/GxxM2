unit uFrmCustomMagic;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, VirtualTrees, StdCtrls, Spin, SpinEditEx,
  ExtCtrls, ComCtrls, Grobal2, uCustomMagicUtils, uFrmCustomMagicCopySetting, M2Threads, CheckUnit, ImgList, System.ImageList;

const
  WM_STARTEDITING_DEC_ATTRIB = WM_USER + 300;
  WM_STARTEDITING_DEC_ELEMENT = WM_USER + 301;
  WM_STARTEDITING_INC_ATTRIB = WM_USER + 302;
  WM_STARTEDITING_INC_ELEMENT = WM_USER + 303;

type
  TFrmCustomMagic = class(TForm)
    grpMonster: TGroupBox;
    vstCustomMagic: TVirtualStringTree;
    pgcMain: TPageControl;
    tsAttack: TTabSheet;
    tsServerAttack: TTabSheet;
    pnlBottom: TPanel;
    lbl13: TLabel;
    btnSave: TButton;
    chkSendCustomMagicConfig: TCheckBox;
    btnMakeConfigData: TButton;
    dlgSaveMagics: TSaveDialog;
    pgcClient: TPageControl;
    tsBase: TTabSheet;
    grpClientBaseSetting: TGroupBox;
    Label50: TLabel;
    Label51: TLabel;
    cbbClientIconFile: TComboBox;
    seClientIconIndex: TSpinEditEx;
    GroupBox2: TGroupBox;
    Label225: TLabel;
    Label226: TLabel;
    lbl3: TLabel;
    Label227: TLabel;
    Label228: TLabel;
    edtSound1: TEdit;
    edtSound2: TEdit;
    edtSound3: TEdit;
    edtSound4: TEdit;
    edtSound5: TEdit;
    tsEffect: TTabSheet;
    grpFly: TGroupBox;
    lbl5: TLabel;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Label5: TLabel;
    Label15: TLabel;
    Label16: TLabel;
    cbbClientFlyFile: TComboBox;
    cbbClientFlyDrawMode: TComboBox;
    cbbClientFlyDirCount: TComboBox;
    chkClientFlyCalcDir: TCheckBox;
    seClientFlyPlayTime: TSpinEditEx;
    seClientFlyEmptyCount: TSpinEditEx;
    seClientFlyPlayCount: TSpinEditEx;
    seClientFlyStartIndex: TSpinEditEx;
    seClientFlyLightRange: TSpinEditEx;
    grpSelf: TGroupBox;
    Label17: TLabel;
    Label18: TLabel;
    Label19: TLabel;
    Label20: TLabel;
    Label21: TLabel;
    Label22: TLabel;
    Label24: TLabel;
    Label25: TLabel;
    Label96: TLabel;
    Label99: TLabel;
    cbbClientSelfFile: TComboBox;
    cbbClientSelfDrawOrder: TComboBox;
    seClientSelfPlayTime: TSpinEditEx;
    seClientSelfPlayCount: TSpinEditEx;
    seClientSelfStartIndex: TSpinEditEx;
    cbbClientSelfDrawMode: TComboBox;
    seClientSelfEmptyCount: TSpinEditEx;
    cbbClientSelfDirCount: TComboBox;
    chkClientSelfPlayDelayAction: TCheckBox;
    seClientSelfLightRange: TSpinEditEx;
    cbbClientSelfDirCalcType: TComboBox;
    grpTarget: TGroupBox;
    Label37: TLabel;
    Label38: TLabel;
    Label39: TLabel;
    Label40: TLabel;
    Label41: TLabel;
    Label42: TLabel;
    Label43: TLabel;
    Label44: TLabel;
    Label45: TLabel;
    Label47: TLabel;
    bvl1: TBevel;
    cbbClientTargetFile: TComboBox;
    seClientTargetPlayTime: TSpinEditEx;
    seClientTargetPlayCount: TSpinEditEx;
    seClientTargetStartIndex: TSpinEditEx;
    cbbClientTargetDrawMode: TComboBox;
    chkClientTargetMultiPlay: TCheckBox;
    chkClientTargetLockDraw: TCheckBox;
    seClientTargetLightRange: TSpinEditEx;
    chkClientTargetKeepPlay: TCheckBox;
    seClientTargetKeepTime: TSpinEditEx;
    seClientTargetKeepAttackRange: TSpinEditEx;
    seClientTargetKeepAttackInterval: TSpinEditEx;
    chkClientTargetKeepMultiPlay: TCheckBox;
    grpFlyEff: TGroupBox;
    Label14: TLabel;
    Label48: TLabel;
    Label49: TLabel;
    cbbClientFlyEffFile: TComboBox;
    seClientFlyEffStartIndex: TSpinEditEx;
    cbbClientFlyEffDrawMode: TComboBox;
    GroupBox1: TGroupBox;
    Label26: TLabel;
    Label27: TLabel;
    Label28: TLabel;
    Label29: TLabel;
    Label30: TLabel;
    Label36: TLabel;
    cbbClientPreTargetFile: TComboBox;
    seClientPreTargetPlayTime: TSpinEditEx;
    seClientPreTargetPlayCount: TSpinEditEx;
    seClientPreTargetStartIndex: TSpinEditEx;
    cbbClientPreTargetDrawMode: TComboBox;
    chkClientPreTargetLockDraw: TCheckBox;
    seClientPreTargetLightRange: TSpinEditEx;
    grpFastMove: TGroupBox;
    Label218: TLabel;
    Label219: TLabel;
    Label220: TLabel;
    Label222: TLabel;
    Label223: TLabel;
    Label224: TLabel;
    Label221: TLabel;
    cbbClientFastMoveFile: TComboBox;
    seClientFastMovePlayTime: TSpinEditEx;
    seClientFastMovePlayCount: TSpinEditEx;
    seClientFastMoveStartIndex: TSpinEditEx;
    cbbClientFastMoveDrawMode: TComboBox;
    seClientFastMoveEmptyCount: TSpinEditEx;
    chkClientFastMoveCalcDir: TCheckBox;
    seFastMoveLightRange: TSpinEditEx;
    GroupBox4: TGroupBox;
    Label229: TLabel;
    Label230: TLabel;
    Label231: TLabel;
    Label233: TLabel;
    Label234: TLabel;
    cbbClientSelfKeepFile: TComboBox;
    seClientSelfKeepPlayTime: TSpinEditEx;
    seClientSelfKeepPlayCount: TSpinEditEx;
    seClientSelfKeepStartIndex: TSpinEditEx;
    cbbClientSelfKeepDrawMode: TComboBox;
    Label232: TLabel;
    seClientSelfKeepTime: TSpinEditEx;
    Label235: TLabel;
    lbl6: TLabel;
    Label236: TLabel;
    seTargetKeepLightRange: TSpinEditEx;
    Label334: TLabel;
    Label335: TLabel;
    seClientTargetKeepTime2: TSpinEditEx;
    chkClientFastMoveNoHitAction: TCheckBox;
    Label336: TLabel;
    Label337: TLabel;
    seClientSelfKeepTime2: TSpinEditEx;
    chkClientSelfPlayFailNoDraw: TCheckBox;
    Label205: TLabel;
    edtSound6: TEdit;
    chkClientFlyFireGunMode: TCheckBox;
    lbl1: TLabel;
    Label31: TLabel;
    Label209: TLabel;
    seClientPreTargetStartIndex2: TSpinEditEx;
    Label212: TLabel;
    seClientTargetStartIndex2: TSpinEditEx;
    Label213: TLabel;
    cbbClientPreTargetDrawMode2: TComboBox;
    Label237: TLabel;
    cbbClientTargetDrawMode2: TComboBox;
    Label200: TLabel;
    Label46: TLabel;
    chkSelf_SyncHumAction: TCheckBox;
    pgcMagicType: TPageControl;
    tsMagicAttack: TTabSheet;
    tsMagicProtected: TTabSheet;
    pgcAttack: TPageControl;
    tsAdditionals: TTabSheet;
    Label60: TLabel;
    Label62: TLabel;
    Label63: TLabel;
    Label64: TLabel;
    Label65: TLabel;
    Label67: TLabel;
    Label68: TLabel;
    Label69: TLabel;
    Label70: TLabel;
    Label71: TLabel;
    Label72: TLabel;
    Label73: TLabel;
    Label74: TLabel;
    Label75: TLabel;
    Label76: TLabel;
    Label77: TLabel;
    Label78: TLabel;
    Label79: TLabel;
    Label80: TLabel;
    Label81: TLabel;
    Label82: TLabel;
    Label83: TLabel;
    Label84: TLabel;
    Label54: TLabel;
    Label55: TLabel;
    Label56: TLabel;
    Label57: TLabel;
    Label59: TLabel;
    Label61: TLabel;
    Label101: TLabel;
    Label102: TLabel;
    Label103: TLabel;
    Label104: TLabel;
    Label105: TLabel;
    Label106: TLabel;
    Label107: TLabel;
    Label108: TLabel;
    Label109: TLabel;
    Label110: TLabel;
    Label111: TLabel;
    Label112: TLabel;
    Label113: TLabel;
    Label114: TLabel;
    Label115: TLabel;
    Label116: TLabel;
    Label117: TLabel;
    Label118: TLabel;
    Label119: TLabel;
    Label120: TLabel;
    Label121: TLabel;
    Label122: TLabel;
    Label123: TLabel;
    Label124: TLabel;
    Label125: TLabel;
    Label126: TLabel;
    Label127: TLabel;
    Label128: TLabel;
    Label129: TLabel;
    Label130: TLabel;
    Label131: TLabel;
    Label132: TLabel;
    Label133: TLabel;
    Label134: TLabel;
    Label135: TLabel;
    Label136: TLabel;
    Label138: TLabel;
    Label139: TLabel;
    Label141: TLabel;
    Label142: TLabel;
    Label143: TLabel;
    Label144: TLabel;
    Label152: TLabel;
    Label154: TLabel;
    Label156: TLabel;
    Label158: TLabel;
    Label162: TLabel;
    Label163: TLabel;
    Label165: TLabel;
    Label168: TLabel;
    Label172: TLabel;
    Label173: TLabel;
    chkAdditional0: TCheckBox;
    seAdditionalRate0: TSpinEditEx;
    seAdditionalTime0: TSpinEditEx;
    chkAdditional1: TCheckBox;
    seAdditionalRate1: TSpinEditEx;
    seAdditionalTime1: TSpinEditEx;
    chkAdditional2: TCheckBox;
    seAdditionalRate2: TSpinEditEx;
    seAdditionalTime2: TSpinEditEx;
    chkAdditional3: TCheckBox;
    seAdditionalRate3: TSpinEditEx;
    seAdditionalTime3: TSpinEditEx;
    chkAdditional4: TCheckBox;
    seAdditionalRate4: TSpinEditEx;
    seAdditionalTime4: TSpinEditEx;
    chkAdditional5: TCheckBox;
    seAdditionalRate5: TSpinEditEx;
    seAdditionalTime5: TSpinEditEx;
    chkAdditional6: TCheckBox;
    seAdditionalRate6: TSpinEditEx;
    seAdditionalTime6: TSpinEditEx;
    chkAdditional7: TCheckBox;
    seAdditionalRate7: TSpinEditEx;
    seAdditionalTime7: TSpinEditEx;
    chkAdditional8: TCheckBox;
    seAdditionalRate8: TSpinEditEx;
    seAdditionalTime8: TSpinEditEx;
    chkAdditional9: TCheckBox;
    seAdditionalRate9: TSpinEditEx;
    seAdditionalTime9: TSpinEditEx;
    chkseAdditionaHighLevel4: TCheckBox;
    seAdditionaHP0: TSpinEditEx;
    chkAdditional10: TCheckBox;
    seAdditionalRate10: TSpinEditEx;
    seAdditionalTime10: TSpinEditEx;
    seAdditionalRate0_2: TSpinEditEx;
    seAdditionalRate1_2: TSpinEditEx;
    seAdditionalRate2_2: TSpinEditEx;
    seAdditionalRate3_2: TSpinEditEx;
    seAdditionalRate4_2: TSpinEditEx;
    seAdditionalRate5_2: TSpinEditEx;
    seAdditionalRate6_2: TSpinEditEx;
    seAdditionalRate7_2: TSpinEditEx;
    seAdditionalRate8_2: TSpinEditEx;
    seAdditionalRate9_2: TSpinEditEx;
    seAdditionalRate10_2: TSpinEditEx;
    seAdditionalTime0_2: TSpinEditEx;
    seAdditionalTime1_2: TSpinEditEx;
    seAdditionalTime2_2: TSpinEditEx;
    seAdditionalTime3_2: TSpinEditEx;
    seAdditionalTime4_2: TSpinEditEx;
    seAdditionalTime5_2: TSpinEditEx;
    seAdditionalTime6_2: TSpinEditEx;
    seAdditionalTime7_2: TSpinEditEx;
    seAdditionalTime8_2: TSpinEditEx;
    seAdditionalTime9_2: TSpinEditEx;
    seAdditionalTime10_2: TSpinEditEx;
    cbbPushedType4: TComboBox;
    tsSubAttrib: TTabSheet;
    pnlMagicServer: TPanel;
    lbl12: TLabel;
    lblAttackDelay: TLabel;
    lblAttackDelayTime: TLabel;
    cbbOperateMode: TComboBox;
    grpInterval: TGroupBox;
    Label32: TLabel;
    Label33: TLabel;
    Label34: TLabel;
    Label35: TLabel;
    seUseInterval: TSpinEditEx;
    chkFailNoShowEff: TCheckBox;
    edtFailMsg: TEdit;
    edtCloseMsg: TEdit;
    edtSucceedMsg: TEdit;
    seAttackDelayTime: TSpinEditEx;
    chkAttackUseNG: TCheckBox;
    grpNeedItem: TGroupBox;
    lblNeedItem: TLabel;
    lblNeedItemCount: TLabel;
    lblNeedItemCustomItemName: TLabel;
    lblCheckVarName: TLabel;
    lblCheckVarType: TLabel;
    lblCheckVarValue: TLabel;
    lblCheckVarAdd: TLabel;
    cbbNeedItem: TComboBox;
    seNeedItemCount: TSpinEditEx;
    edtNeedItemCustomItemName: TEdit;
    chkNeedItemUseBagItem: TCheckBox;
    chkCheckVarValue: TCheckBox;
    edtCheckVarName: TEdit;
    cbbCheckVarType: TComboBox;
    seCheckVarValue: TSpinEditEx;
    seCheckVarAdd: TSpinEditEx;
    lbl14: TLabel;
    vstAttackDecAttr: TVirtualStringTree;
    ilCheck: TImageList;
    tsAttackDecElement: TTabSheet;
    vstDecElement: TVirtualStringTree;
    pgcProtected: TPageControl;
    tsProtectedDec: TTabSheet;
    tsProtectedAddElement: TTabSheet;
    chkProtectAddHPSlow: TCheckBox;
    vstAddElement: TVirtualStringTree;
    cbbAdditionalTime0_1: TComboBox;
    cbbAdditionalTime1_1: TComboBox;
    cbbAdditionalTime2_1: TComboBox;
    cbbAdditionalTime3_1: TComboBox;
    cbbAdditionalTime10_1: TComboBox;
    cbbAdditionalTime7_1: TComboBox;
    cbbAdditionalTime8_1: TComboBox;
    cbbAdditionalTime9_1: TComboBox;
    seProtectAddHPSlowCount: TSpinEditEx;
    lblProtectTargetRangeTitle: TLabel;
    lblProtectTargetRangeValue: TLabel;
    seProtectTargetRange: TSpinEditEx;
    vstProtectedAddAttr: TVirtualStringTree;
    tsTargetStatus: TTabSheet;
    GroupBox11: TGroupBox;
    Label177: TLabel;
    Label178: TLabel;
    Label179: TLabel;
    Label180: TLabel;
    Label181: TLabel;
    Label186: TLabel;
    cbbTargetStatus1_File: TComboBox;
    cbbTargetStatus1_DrawMode: TComboBox;
    seTargetStatus1_PlayTime: TSpinEditEx;
    seTargetStatus1_EmptyCount: TSpinEditEx;
    seTargetStatus1_PlayCount: TSpinEditEx;
    seTargetStatus1_StartIndex: TSpinEditEx;
    GroupBox13: TGroupBox;
    Label190: TLabel;
    Label191: TLabel;
    Label192: TLabel;
    Label193: TLabel;
    Label194: TLabel;
    Label196: TLabel;
    cbbTargetStatus2_File: TComboBox;
    cbbTargetStatus2_DrawMode: TComboBox;
    seTargetStatus2_PlayTime: TSpinEditEx;
    seTargetStatus2_EmptyCount: TSpinEditEx;
    seTargetStatus2_PlayCount: TSpinEditEx;
    seTargetStatus2_StartIndex: TSpinEditEx;
    Label201: TLabel;
    Label204: TLabel;
    Label238: TLabel;
    chkAttackTargetStatus: TCheckBox;
    seAttackTargetStatusTime: TSpinEditEx;
    seAttackTargetStatusTime_2: TSpinEditEx;
    cbbAttackTargetStatusTime_1: TComboBox;
    Label198: TLabel;
    Label202: TLabel;
    chkProtectTargetStatus: TCheckBox;
    seProtectTargetStatusTime: TSpinEditEx;
    seProtectTargetStatusTime_2: TSpinEditEx;
    lbl7: TLabel;
    Label58: TLabel;
    lbl10: TLabel;
    seProtectTargetStatusTimeDelay: TSpinEditEx;
    chkTargetStatus1_CalcDir: TCheckBox;
    chkTargetStatus2_CalcDir: TCheckBox;
    Label23: TLabel;
    seAttackTargetStatusDelay: TSpinEditEx;
    lbl17: TLabel;
    cbbProtectTargetStatusTime_1: TComboBox;
    Label85: TLabel;
    chkAttackNoChangeDir: TCheckBox;
    Label86: TLabel;
    seClientPreTargetEmptyCount: TSpinEditEx;
    chkClientPreTargetCalcDir: TCheckBox;
    txtMagicWarr: TStaticText;
    lbl20: TLabel;
    GroupBox3: TGroupBox;
    lbl4: TLabel;
    Label52: TLabel;
    lbl15: TLabel;
    lbl16: TLabel;
    Label11: TLabel;
    Label195: TLabel;
    lblMagicWarrNGOption: TLabel;
    lbl18: TLabel;
    cbbClientLevel: TComboBox;
    chkClientLock: TCheckBox;
    cbbClientActionType: TComboBox;
    chkClientLockSelf: TCheckBox;
    btnCopyConfig: TButton;
    seClientActionStartIndex: TSpinEditEx;
    seClientActionPlayCount: TSpinEditEx;
    seClientActionEmptyCount: TSpinEditEx;
    chkClientActionContinue: TCheckBox;
    cbbMagicSwitchMode: TComboBox;
    cbbMagicWarrNGOption: TComboBox;
    chkSwitchModeNoClose: TCheckBox;
    chkMagicAutoOpen: TCheckBox;
    chkDisableInSafeZone: TCheckBox;
    ts1: TTabSheet;
    Label776: TLabel;
    seMagicACHumValue: TSpinEditEx;
    seMagicACMonValue: TSpinEditEx;
    seMagicACHeroValue: TSpinEditEx;
    seDefenceHumValue: TSpinEditEx;
    seDefenceMonValue: TSpinEditEx;
    seDefenceHeroValue: TSpinEditEx;
    Label92: TLabel;
    Label94: TLabel;
    Label95: TLabel;
    Label97: TLabel;
    Label98: TLabel;
    Label100: TLabel;
    chkMagicACHum: TCheckBox;
    seMagicACHumRate: TSpinEditEx;
    seMagicACHumRateAdd: TSpinEditEx;
    seMagicACHumValueAdd: TSpinEditEx;
    Label93: TLabel;
    Label137: TLabel;
    Label140: TLabel;
    Label145: TLabel;
    Label146: TLabel;
    Label147: TLabel;
    Label148: TLabel;
    Label149: TLabel;
    chkMagicACMon: TCheckBox;
    seMagicACMonRate: TSpinEditEx;
    seMagicACMonRateAdd: TSpinEditEx;
    seMagicACMonValueAdd: TSpinEditEx;
    Label150: TLabel;
    Label151: TLabel;
    Label153: TLabel;
    Label155: TLabel;
    Label157: TLabel;
    Label159: TLabel;
    Label160: TLabel;
    Label161: TLabel;
    chkMagicACHero: TCheckBox;
    seMagicACHeroRate: TSpinEditEx;
    seMagicACHeroRateAdd: TSpinEditEx;
    seMagicACHeroValueAdd: TSpinEditEx;
    Label164: TLabel;
    Label166: TLabel;
    Label167: TLabel;
    Label169: TLabel;
    Label170: TLabel;
    Label171: TLabel;
    Label174: TLabel;
    Label175: TLabel;
    chkDefenceHum: TCheckBox;
    seDefenceHumRate: TSpinEditEx;
    seDefenceHumRateAdd: TSpinEditEx;
    seDefenceHumValueAdd: TSpinEditEx;
    Label176: TLabel;
    Label182: TLabel;
    Label183: TLabel;
    Label184: TLabel;
    Label185: TLabel;
    Label187: TLabel;
    Label188: TLabel;
    Label189: TLabel;
    chkDefenceMon: TCheckBox;
    seDefenceMonRate: TSpinEditEx;
    seDefenceMonRateAdd: TSpinEditEx;
    seDefenceMonValueAdd: TSpinEditEx;
    Label197: TLabel;
    Label66: TLabel;
    Label87: TLabel;
    Label88: TLabel;
    Label89: TLabel;
    Label203: TLabel;
    Label239: TLabel;
    Label240: TLabel;
    chkDefenceHero: TCheckBox;
    seDefenceHeroRate: TSpinEditEx;
    seDefenceHeroRateAdd: TSpinEditEx;
    seDefenceHeroValueAdd: TSpinEditEx;
    Label241: TLabel;
    lbl19: TLabel;
    Label90: TLabel;
    Label91: TLabel;
    chkClientNotRaiseHand: TCheckBox;
    grpOptions: TGroupBox;
    Label6: TLabel;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    Label12: TLabel;
    seAttackNearRange: TSpinEditEx;
    seAttackGroupRange: TSpinEditEx;
    cbbAttackTarget: TComboBox;
    chkEnableAntiMagic: TCheckBox;
    chkEnableHitPoint: TCheckBox;
    grpCallMob: TGroupBox;
    lbl11: TLabel;
    Label207: TLabel;
    Label214: TLabel;
    Label215: TLabel;
    Label210: TLabel;
    Label211: TLabel;
    lbl8: TLabel;
    lbl9: TLabel;
    Label199: TLabel;
    edtCallMonster1: TEdit;
    seCallMonsterNum1: TSpinEditEx;
    chkEnabledCallMonster: TCheckBox;
    seCallMonstersRate: TSpinEditEx;
    edtCallMonster2: TEdit;
    seCallMonsterNum2: TSpinEditEx;
    seCallMonstersRoyaltySec: TSpinEditEx;
    seCallMonstersLevel: TSpinEditEx;
    grpPower: TGroupBox;
    Label10: TLabel;
    Label13: TLabel;
    lbl2: TLabel;
    Label53: TLabel;
    lblLineAttackAddPower: TLabel;
    lblLineAttackAddPowerPerc: TLabel;
    Label206: TLabel;
    Label208: TLabel;
    seAttackPowerRate: TSpinEditEx;
    cbbAttackPowerLevel: TComboBox;
    cbbAttackPowerCalc: TComboBox;
    seLineAttackAddPower: TSpinEditEx;
    seAttackPowerUndeadAdd: TSpinEditEx;
    grpMove: TGroupBox;
    Label216: TLabel;
    Label217: TLabel;
    Label242: TLabel;
    seAttackTeleportRate: TSpinEditEx;
    chkAttackTeleportRunHum: TCheckBox;
    chkAttackTeleportRunMon: TCheckBox;
    chkAttackTeleportRunNpc: TCheckBox;
    chkAttackTeleportRunGuard: TCheckBox;
    chkAttackTeleportRunObstacle: TCheckBox;
    chkAttackTeleportWarDisHumRun: TCheckBox;
    chkAttackTeleportCannotRunItem: TCheckBox;
    chkNoTeleportNoAttack: TCheckBox;
    chkAttackTeleportAfterDamage: TCheckBox;
    chkAttackTeleportRush: TCheckBox;
    seAttackTeleportRushCount: TSpinEditEx;
    chkAttackTeleportAttack: TCheckBox;
    lblAttackWidth: TLabel;
    lblH_AttackWidth: TLabel;
    seAttackLineWidth: TSpinEditEx;
    procedure FormCreate(Sender: TObject);
    procedure vstCustomMagicGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
    procedure vstCustomMagicGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstCustomMagicNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure cbbClientLevelChange(Sender: TObject);
    procedure chkClientLockClick(Sender: TObject);
    procedure cbbClientActionTypeChange(Sender: TObject);
    procedure cbbClientIconFileChange(Sender: TObject);
    procedure seClientIconIndexChange(Sender: TObject);
    procedure cbbClientFlyFileChange(Sender: TObject);
    procedure seClientFlyStartIndexChange(Sender: TObject);
    procedure seClientFlyPlayCountChange(Sender: TObject);
    procedure seClientFlyEmptyCountChange(Sender: TObject);
    procedure seClientFlyPlayTimeChange(Sender: TObject);
    procedure cbbClientFlyDrawModeChange(Sender: TObject);
    procedure cbbClientFlyDirCountChange(Sender: TObject);
    procedure chkClientFlyCalcDirClick(Sender: TObject);
    procedure seClientFlyLightRangeChange(Sender: TObject);
    procedure cbbClientFlyEffFileChange(Sender: TObject);
    procedure seClientFlyEffStartIndexChange(Sender: TObject);
    procedure cbbClientFlyEffDrawModeChange(Sender: TObject);
    procedure cbbClientSelfFileChange(Sender: TObject);
    procedure seClientSelfStartIndexChange(Sender: TObject);
    procedure seClientSelfPlayCountChange(Sender: TObject);
    procedure seClientSelfEmptyCountChange(Sender: TObject);
    procedure seClientSelfPlayTimeChange(Sender: TObject);
    procedure cbbClientSelfDrawOrderChange(Sender: TObject);
    procedure cbbClientSelfDrawModeChange(Sender: TObject);
    procedure cbbClientSelfDirCountChange(Sender: TObject);
    procedure chkClientSelfPlayDelayActionClick(Sender: TObject);
    procedure seClientSelfLightRangeChange(Sender: TObject);
    procedure cbbClientTargetFileChange(Sender: TObject);
    procedure seClientTargetStartIndexChange(Sender: TObject);
    procedure seClientTargetPlayCountChange(Sender: TObject);
    procedure seClientTargetPlayTimeChange(Sender: TObject);
    procedure cbbClientTargetDrawModeChange(Sender: TObject);
    procedure chkClientTargetMultiPlayClick(Sender: TObject);
    procedure chkClientTargetLockDrawClick(Sender: TObject);
    procedure seClientTargetLightRangeChange(Sender: TObject);
    procedure chkClientTargetKeepPlayClick(Sender: TObject);
    procedure seClientTargetKeepTimeChange(Sender: TObject);
    procedure seClientTargetKeepAttackIntervalChange(Sender: TObject);
    procedure seClientTargetKeepAttackRangeChange(Sender: TObject);
    procedure chkClientTargetKeepMultiPlayClick(Sender: TObject);
    procedure cbbOperateModeChange(Sender: TObject);
    procedure seUseIntervalChange(Sender: TObject);
    procedure chkFailNoShowEffClick(Sender: TObject);
    procedure edtFailMsgChange(Sender: TObject);
    procedure edtSucceedMsgChange(Sender: TObject);
    procedure edtCloseMsgChange(Sender: TObject);
    procedure cbbAttackTargetChange(Sender: TObject);
    procedure seAttackNearRangeChange(Sender: TObject);
    procedure seAttackGroupRangeChange(Sender: TObject);
    procedure cbbAttackPowerCalcChange(Sender: TObject);
    procedure cbbAttackPowerLevelChange(Sender: TObject);
    procedure seAttackPowerRateChange(Sender: TObject);
    procedure cbbNeedItemChange(Sender: TObject);
    procedure seNeedItemCountChange(Sender: TObject);
    procedure seProtectTargetRangeChange(Sender: TObject);
    procedure chkAdditional0Click(Sender: TObject);
    procedure seAdditionalRate0_2Change(Sender: TObject);
    procedure seAdditionalTime0Change(Sender: TObject);
    procedure seAdditionalTime0_2Change(Sender: TObject);
    procedure seAdditionaHP0Change(Sender: TObject);
    procedure chkseAdditionaHighLevel4Click(Sender: TObject);
    procedure btnSaveClick(Sender: TObject);
    procedure btnMakeConfigDataClick(Sender: TObject);
    procedure seAdditionalRate0Change(Sender: TObject);
    procedure chkSendCustomMagicConfigClick(Sender: TObject);
    procedure cbbClientPreTargetFileChange(Sender: TObject);
    procedure seClientPreTargetStartIndexChange(Sender: TObject);
    procedure seClientPreTargetPlayCountChange(Sender: TObject);
    procedure seClientPreTargetPlayTimeChange(Sender: TObject);
    procedure cbbClientPreTargetDrawModeChange(Sender: TObject);
    procedure seClientPreTargetLightRangeChange(Sender: TObject);
    procedure chkClientPreTargetLockDrawClick(Sender: TObject);
    procedure cbbClientSelfDirCalcTypeChange(Sender: TObject);
    procedure edtCallMonster1Change(Sender: TObject);
    procedure seCallMonsterNum1Change(Sender: TObject);
    procedure seCallMonstersRateChange(Sender: TObject);
    procedure chkAttackTeleportAttackClick(Sender: TObject);
    procedure seAttackTeleportRateChange(Sender: TObject);
    procedure chkEnabledCallMonsterClick(Sender: TObject);
    procedure cbbClientFastMoveFileChange(Sender: TObject);
    procedure seClientFastMoveStartIndexChange(Sender: TObject);
    procedure seClientFastMovePlayCountChange(Sender: TObject);
    procedure seClientFastMoveEmptyCountChange(Sender: TObject);
    procedure seClientFastMovePlayTimeChange(Sender: TObject);
    procedure cbbClientFastMoveDrawModeChange(Sender: TObject);
    procedure chkClientFastMoveCalcDirClick(Sender: TObject);
    procedure seFastMoveLightRangeChange(Sender: TObject);
    procedure edtSound1Change(Sender: TObject);
    procedure chkClientLockSelfClick(Sender: TObject);
    procedure cbbClientSelfKeepFileChange(Sender: TObject);
    procedure seClientSelfKeepStartIndexChange(Sender: TObject);
    procedure seClientSelfKeepPlayCountChange(Sender: TObject);
    procedure seClientSelfKeepPlayTimeChange(Sender: TObject);
    procedure cbbClientSelfKeepDrawModeChange(Sender: TObject);
    procedure seClientSelfKeepTimeChange(Sender: TObject);
    procedure seCallMonstersRoyaltySecChange(Sender: TObject);
    procedure seTargetKeepLightRangeChange(Sender: TObject);
    procedure seClientTargetKeepTime2Change(Sender: TObject);
    procedure chkClientFastMoveNoHitActionClick(Sender: TObject);
    procedure edtNeedItemCustomItemNameChange(Sender: TObject);
    procedure chkNeedItemUseBagItemClick(Sender: TObject);
    procedure seClientSelfKeepTime2Change(Sender: TObject);
    procedure btnCopyConfigClick(Sender: TObject);
    procedure chkAttackTeleportRunHumClick(Sender: TObject);
    procedure chkAttackTeleportRunMonClick(Sender: TObject);
    procedure chkAttackTeleportRunNpcClick(Sender: TObject);
    procedure chkAttackTeleportRunGuardClick(Sender: TObject);
    procedure chkAttackTeleportRunObstacleClick(Sender: TObject);
    procedure chkAttackTeleportWarDisHumRunClick(Sender: TObject);
    procedure chkClientSelfPlayFailNoDrawClick(Sender: TObject);
    procedure cbbPushedType4Change(Sender: TObject);
    procedure seClientActionStartIndexChange(Sender: TObject);
    procedure seClientActionPlayCountChange(Sender: TObject);
    procedure seClientActionEmptyCountChange(Sender: TObject);
    procedure chkClientActionContinueClick(Sender: TObject);
    procedure seCallMonstersLevelChange(Sender: TObject);
    procedure chkEnableAntiMagicClick(Sender: TObject);
    procedure seLineAttackAddPowerChange(Sender: TObject);
    procedure chkProtectAddHPSlowClick(Sender: TObject);
    procedure chkClientFlyFireGunModeClick(Sender: TObject);
    procedure seAttackDelayTimeChange(Sender: TObject);
    procedure chkEnableHitPointClick(Sender: TObject);
    procedure chkCheckVarValueClick(Sender: TObject);
    procedure edtCheckVarNameChange(Sender: TObject);
    procedure cbbCheckVarTypeChange(Sender: TObject);
    procedure seCheckVarValueChange(Sender: TObject);
    procedure seCheckVarAddChange(Sender: TObject);
    procedure seAttackPowerUndeadAddChange(Sender: TObject);
    procedure cbbMagicSwitchModeChange(Sender: TObject);
    procedure chkNoTeleportNoAttackClick(Sender: TObject);
    procedure chkAttackTeleportCannotRunItemClick(Sender: TObject);
    procedure seClientPreTargetStartIndex2Change(Sender: TObject);
    procedure seClientTargetStartIndex2Change(Sender: TObject);
    procedure cbbClientPreTargetDrawMode2Change(Sender: TObject);
    procedure cbbClientTargetDrawMode2Change(Sender: TObject);
    procedure chkSelf_SyncHumActionClick(Sender: TObject);
    procedure chkAttackUseNGClick(Sender: TObject);
    procedure vstAttackDecAttrGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstAttackDecAttrAfterCellPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column:
      TColumnIndex; CellRect: TRect);
    procedure vstAttackDecAttrNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure vstAttackDecAttrChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
    procedure vstAttackDecAttrEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstAttackDecAttrCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; out EditLink:
      IVTEditLink);
    procedure vstDecElementChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
    procedure vstDecElementAfterCellPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column:
      TColumnIndex; CellRect: TRect);
    procedure vstDecElementGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType; var
      CellText: string);
    procedure vstDecElementEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstDecElementCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; out EditLink:
      IVTEditLink);
    procedure vstDecElementNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure cbbAdditionalTime0_1Change(Sender: TObject);
    procedure seProtectAddHPSlowCountChange(Sender: TObject);
    procedure vstProtectedAddAttrChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
    procedure vstProtectedAddAttrNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure vstProtectedAddAttrAfterCellPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column:
      TColumnIndex; CellRect: TRect);
    procedure vstProtectedAddAttrGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType:
      TVSTTextType; var CellText: string);
    procedure vstProtectedAddAttrEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstProtectedAddAttrCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; out EditLink:
      IVTEditLink);
    procedure cbbTargetStatus1_FileChange(Sender: TObject);
    procedure seTargetStatus1_StartIndexChange(Sender: TObject);
    procedure seTargetStatus1_PlayCountChange(Sender: TObject);
    procedure seTargetStatus1_EmptyCountChange(Sender: TObject);
    procedure seTargetStatus1_PlayTimeChange(Sender: TObject);
    procedure cbbTargetStatus1_DrawModeChange(Sender: TObject);
    procedure chkTargetStatus1_CalcDirClick(Sender: TObject);
    procedure cbbTargetStatus2_FileChange(Sender: TObject);
    procedure seTargetStatus2_StartIndexChange(Sender: TObject);
    procedure seTargetStatus2_PlayCountChange(Sender: TObject);
    procedure seTargetStatus2_EmptyCountChange(Sender: TObject);
    procedure seTargetStatus2_PlayTimeChange(Sender: TObject);
    procedure cbbTargetStatus2_DrawModeChange(Sender: TObject);
    procedure chkTargetStatus2_CalcDirClick(Sender: TObject);
    procedure chkAttackTargetStatusClick(Sender: TObject);
    procedure cbbAttackTargetStatusTime_1Change(Sender: TObject);
    procedure seAttackTargetStatusTime_2Change(Sender: TObject);
    procedure seAttackTargetStatusDelayChange(Sender: TObject);
    procedure seAttackTargetStatusTimeChange(Sender: TObject);
    procedure chkProtectTargetStatusClick(Sender: TObject);
    procedure seProtectTargetStatusTimeChange(Sender: TObject);
    procedure cbbProtectTargetStatusTime_1Change(Sender: TObject);
    procedure seProtectTargetStatusTime_2Change(Sender: TObject);
    procedure seProtectTargetStatusTimeDelayChange(Sender: TObject);
    procedure chkAttackNoChangeDirClick(Sender: TObject);
    procedure seClientPreTargetEmptyCountChange(Sender: TObject);
    procedure chkClientPreTargetCalcDirClick(Sender: TObject);
    procedure cbbMagicWarrNGOptionChange(Sender: TObject);
    procedure chkSwitchModeNoCloseClick(Sender: TObject);
    procedure chkMagicAutoOpenClick(Sender: TObject);
    procedure chkDisableInSafeZoneClick(Sender: TObject);
    procedure chkMagicACHumClick(Sender: TObject);
    procedure seMagicACHumRateChange(Sender: TObject);
    procedure seMagicACHumRateAddChange(Sender: TObject);
    procedure seMagicACHumValueChange(Sender: TObject);
    procedure seMagicACHumValueAddChange(Sender: TObject);
    procedure vstCustomMagicDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
    procedure chkAttackTeleportAfterDamageClick(Sender: TObject);
    procedure chkAttackTeleportRushClick(Sender: TObject);
    procedure seAttackTeleportRushCountChange(Sender: TObject);
    procedure chkClientNotRaiseHandClick(Sender: TObject);
    procedure seAttackLineWidthChange(Sender: TObject);
  private
    FCurrentCustomConfig: TCustomMagicConfig;
    FCurrentClientConfig: PMagicClientConfig;
    FCurrentServerConfig: PMagicServerConfig;

    FIsConfigChanged: Boolean;

    procedure WMStartEditingDecAttrib(var Message: TMessage); message WM_STARTEDITING_DEC_ATTRIB;
    procedure WMStartEditingDecElement(var Message: TMessage); message WM_STARTEDITING_DEC_ELEMENT;
    procedure WMStartEditingIncAttrib(var Message: TMessage); message WM_STARTEDITING_INC_ATTRIB;
    procedure WMStartEditingIncElement(var Message: TMessage); message WM_STARTEDITING_INC_ELEMENT;
  public
    { Public declarations }

    procedure DoOpen;
    procedure SetConfigChanged(IsChanged: Boolean = True);
  end;

function ShowCustomMagic: Boolean;

implementation

{$R *.dfm}

uses
  M2Share, UsrEngn, EDCode;

type
  PMagicConfigNodeData = ^TMagicConfigNodeData;

  TMagicConfigNodeData = record
    Config: TCustomMagicConfig;
  end;

type
  PAttackDecAttribData = ^TAttackDecAttribData;

  TAttackDecAttribData = record
    AttribType: TMagicAttackDecAttributesType;
    Data: PMagicChangeAttributesRecord;
  end;

  PProtectAddAttribData = ^TProtectAddAttribData;

  TProtectAddAttribData = record
    AttribType: TMagicProtectAddAttributesType;
    Data: PMagicChangeAttributesRecord;
  end;

  PMagicElementData = ^TMagicElementData;

  TMagicElementData = record
    ElementType: TItemElementsType;
    Data: PMagicAttackChangeElementRecord;
  end;

type
  TDecAttribPropertyEditLink = class(TInterfacedObject, IVTEditLink)
  private
    FEdit: TWinControl; // One of the property editor classes.
    FTree: TVirtualStringTree; // A back reference to the tree calling.
    FNode: PVirtualNode; // The node being edited.
    FColumn: Integer; // The column of the node being edited.
  protected
    procedure EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
  public
    destructor Destroy; override;

    function BeginEdit: Boolean; stdcall;
    function CancelEdit: Boolean; stdcall;
    function EndEdit: Boolean; stdcall;
    function GetBounds: TRect; stdcall;
    function PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean; stdcall;
    procedure ProcessMessage(var Message: TMessage); stdcall;
    procedure SetBounds(R: TRect); stdcall;
  end;

  TElementPropertyEditLink = class(TInterfacedObject, IVTEditLink)
  private
    FEdit: TWinControl; // One of the property editor classes.
    FTree: TVirtualStringTree; // A back reference to the tree calling.
    FNode: PVirtualNode; // The node being edited.
    FColumn: Integer; // The column of the node being edited.
  protected
    procedure EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
  public
    destructor Destroy; override;

    function BeginEdit: Boolean; stdcall;
    function CancelEdit: Boolean; stdcall;
    function EndEdit: Boolean; stdcall;
    function GetBounds: TRect; stdcall;
    function PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean; stdcall;
    procedure ProcessMessage(var Message: TMessage); stdcall;
    procedure SetBounds(R: TRect); stdcall;
  end;

  /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

destructor TDecAttribPropertyEditLink.Destroy;
begin
  if FEdit <> nil then
    FEdit.Free;

  inherited;
end;

// ----------------------------------------------------------------------------------------------------------------------

procedure TDecAttribPropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  CanAdvance: Boolean;
begin
  CanAdvance := True;

  case Key of
    VK_ESCAPE:
      begin
        Key := 0; // ESC will be handled in EditKeyUp()
      end;
    VK_RETURN:
      if CanAdvance then
      begin
        Key := 0;
        FTree.EndEditNode;
        Abort;
      end;
    VK_UP, VK_DOWN:
      begin
        // Consider special cases before finishing edit mode.
        CanAdvance := Shift = [];
        if FEdit is TComboBox then
          CanAdvance := CanAdvance and not TComboBox(FEdit).DroppedDown
        else if FEdit is TSpinEditEx then
          CanAdvance := True;
        if CanAdvance then
        begin
          PostMessage(FTree.Handle, WM_KEYDOWN, Key, 0);
          Key := 0;
        end;
      end;
  end;
end;

procedure TDecAttribPropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
begin
  case Key of
    VK_ESCAPE:
      begin
        FTree.CancelEditNode;
        Key := 0;
      end; // VK_ESCAPE
  end; // case
end;

// ----------------------------------------------------------------------------------------------------------------------

function TDecAttribPropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TDecAttribPropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TDecAttribPropertyEditLink.EndEdit: Boolean;
var
  DecAttribData: PAttackDecAttribData;
  TempValue: Integer;
  S: string;
  IsChanged: Boolean;
begin
  Result := True;
  IsChanged := False;

  DecAttribData := FTree.GetNodeData(FNode);

  case FColumn of
    1, 2, 3, 5, 6, 8, 9, 10:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;

        case FColumn of
          1:
            begin
              if DecAttribData.Data.Rate <> TempValue then
              begin
                DecAttribData.Data.Rate := TempValue;
                IsChanged := True;
              end;
            end;
          2:
            begin
              if DecAttribData.Data.RateAdd <> TempValue then
              begin
                DecAttribData.Data.RateAdd := TempValue;
                IsChanged := True;
              end;
            end;
          3:
            begin
              if DecAttribData.Data.LowValue <> TempValue then
              begin
                DecAttribData.Data.LowValue := TempValue;
                IsChanged := True;
              end;
            end;
          5:
            begin
              if DecAttribData.Data.LowValueAdd <> TempValue then
              begin
                DecAttribData.Data.LowValueAdd := TempValue;
                IsChanged := True;
              end;
            end;
          6:
            begin
              if DecAttribData.Data.HighValue <> TempValue then
              begin
                DecAttribData.Data.HighValue := TempValue;
                IsChanged := True;
              end;
            end;
          8:
            begin
              if DecAttribData.Data.HighValueAdd <> TempValue then
              begin
                DecAttribData.Data.HighValueAdd := TempValue;
                IsChanged := True;
              end;
            end;
          9:
            begin
              if DecAttribData.Data.Time <> TempValue then
              begin
                DecAttribData.Data.Time := TempValue;
                IsChanged := True;
              end;
            end;
          10:
            begin
              if DecAttribData.Data.TimeAdd <> TempValue then
              begin
                DecAttribData.Data.TimeAdd := TempValue;
                IsChanged := True;
              end;
            end;
        end;
      end;
    4, 7, 11:
      begin
        TempValue := (FEdit as TComboBox).ItemIndex;

        case FColumn of
          4:
            begin
              if Integer(DecAttribData.Data.LowValueIsPoint) <> TempValue then
              begin
                DecAttribData.Data.LowValueIsPoint := TempValue = 1;
                IsChanged := True;
              end;
            end;
          7:
            begin
              if Integer(DecAttribData.Data.HighValueIsPoint) <> TempValue then
              begin
                DecAttribData.Data.HighValueIsPoint := TempValue = 1;
                IsChanged := True;
              end;
            end;
          11:
            begin
              if Integer(DecAttribData.Data.TimeAddIsPoint) <> TempValue then
              begin
                DecAttribData.Data.TimeAddIsPoint := TempValue = 1;
                IsChanged := True;
              end;
            end;
        end;
      end;
    13:
      begin
        S := (FEdit as TEdit).Text;
        if not SameText(DecAttribData.Data.HintText, S) then
        begin
          DecAttribData.Data.HintText := S;
          IsChanged := True;
        end;
      end;
  end;

  if FTree.CanFocus then
    FTree.SetFocus;
  if FEdit.Visible then
    FEdit.Visible := False;

  if IsChanged then
  begin
    if (FTree.Owner is TFrmCustomMagic) then
    begin
      TFrmCustomMagic(FTree.Owner).SetConfigChanged();
    end;
  end;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TDecAttribPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TDecAttribPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  DecAttribData: PAttackDecAttribData;
  boUnit: Boolean;
begin
  Result := True;
  FTree := Tree as TVirtualStringTree;
  FNode := Node;
  FColumn := Column;

  // determine what edit type actually is needed
  if FEdit <> nil then
  begin
    FEdit.Free;
    FEdit := nil;
  end;

  DecAttribData := FTree.GetNodeData(Node);

  case FColumn of
    1, 2, 3, 5, 6, 8, 9, 10:
      begin
        FEdit := TSpinEditEx.Create(nil);

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          if FColumn in [1] then
          begin
            MinValue := 0;
            MaxValue := 100;
          end;

          case FColumn of
            1:
              Value := DecAttribData.Data.Rate;
            2:
              Value := DecAttribData.Data.RateAdd;
            3:
              Value := DecAttribData.Data.LowValue;
            5:
              Value := DecAttribData.Data.LowValueAdd;
            6:
              Value := DecAttribData.Data.HighValue;
            8:
              Value := DecAttribData.Data.HighValueAdd;
            9:
              Value := DecAttribData.Data.Time;
            10:
              Value := DecAttribData.Data.TimeAdd;
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    4, 7, 11:
      begin
        FEdit := TComboBox.Create(nil);

        with FEdit as TComboBox do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDownList;

          if FColumn in [4, 7] then
          begin
            for boUnit := Low(Boolean) to High(Boolean) do
            begin
              Items.Add(MagicAttackDecValueTypeNames[boUnit]);
            end;

            if FColumn = 4 then
              ItemIndex := Integer(DecAttribData.Data.LowValueIsPoint)
            else
              ItemIndex := Integer(DecAttribData.Data.HighValueIsPoint);
          end
          else if FColumn = 11 then
          begin
            for boUnit := Low(Boolean) to High(Boolean) do
            begin
              Items.Add(MagicAttackDecTimeTypeNames[boUnit]);
            end;

            ItemIndex := Integer(DecAttribData.Data.TimeAddIsPoint);
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    13:
      begin
        FEdit := TEdit.Create(nil);

        with FEdit as TEdit do
        begin
          Visible := False;
          Parent := Tree;
          Text := DecAttribData.Data.HintText;
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  end;
end;

// ----------------------------------------------------------------------------------------------------------------------

procedure TDecAttribPropertyEditLink.ProcessMessage(var Message: TMessage);
begin
  FEdit.WindowProc(Message);
end;

// ----------------------------------------------------------------------------------------------------------------------

procedure TDecAttribPropertyEditLink.SetBounds(R: TRect);
var
  Dummy: Integer;
begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right);
  FEdit.BoundsRect := R;
end;

/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

destructor TElementPropertyEditLink.Destroy;
begin
  if FEdit <> nil then
    FEdit.Free;

  inherited;
end;

// ----------------------------------------------------------------------------------------------------------------------

procedure TElementPropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  CanAdvance: Boolean;
begin
  CanAdvance := True;

  case Key of
    VK_ESCAPE:
      begin
        Key := 0; // ESC will be handled in EditKeyUp()
      end;
    VK_RETURN:
      if CanAdvance then
      begin
        Key := 0;
        FTree.EndEditNode;
        Abort;
      end;
    VK_UP, VK_DOWN:
      begin
        // Consider special cases before finishing edit mode.
        CanAdvance := Shift = [];
        if FEdit is TComboBox then
          CanAdvance := CanAdvance and not TComboBox(FEdit).DroppedDown
        else if FEdit is TSpinEditEx then
          CanAdvance := True;
        if CanAdvance then
        begin
          PostMessage(FTree.Handle, WM_KEYDOWN, Key, 0);
          Key := 0;
        end;
      end;
  end;
end;

procedure TElementPropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
begin
  case Key of
    VK_ESCAPE:
      begin
        FTree.CancelEditNode;
        Key := 0;
      end; // VK_ESCAPE
  end; // case
end;

// ----------------------------------------------------------------------------------------------------------------------

function TElementPropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TElementPropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TElementPropertyEditLink.EndEdit: Boolean;
var
  MagicElementData: PMagicElementData;
  TempValue: Integer;
  S: string;
  IsChanged: Boolean;
begin
  Result := True;
  IsChanged := False;

  MagicElementData := FTree.GetNodeData(FNode);

  case FColumn of
    1, 2, 3, 5, 6, 7:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;

        case FColumn of
          1:
            begin
              if MagicElementData.Data.Rate <> TempValue then
              begin
                MagicElementData.Data.Rate := TempValue;
                IsChanged := True;
              end;
            end;
          2:
            begin
              if MagicElementData.Data.RateAdd <> TempValue then
              begin
                MagicElementData.Data.RateAdd := TempValue;
                IsChanged := True;
              end;
            end;
          3:
            begin
              if MagicElementData.Data.Value <> TempValue then
              begin
                MagicElementData.Data.Value := TempValue;
                IsChanged := True;
              end;
            end;
          5:
            begin
              if MagicElementData.Data.ValueAdd <> TempValue then
              begin
                MagicElementData.Data.ValueAdd := TempValue;
                IsChanged := True;
              end;
            end;
          6:
            begin
              if MagicElementData.Data.Time <> TempValue then
              begin
                MagicElementData.Data.Time := TempValue;
                IsChanged := True;
              end;
            end;
          7:
            begin
              if MagicElementData.Data.TimeAdd <> TempValue then
              begin
                MagicElementData.Data.TimeAdd := TempValue;
                IsChanged := True;
              end;
            end;
        end;
      end;
    4, 8:
      begin
        TempValue := (FEdit as TComboBox).ItemIndex;

        case FColumn of
          4:
            begin
              if Integer(MagicElementData.Data.ValueIsPoint) <> TempValue then
              begin
                MagicElementData.Data.ValueIsPoint := TempValue = 1;
                IsChanged := True;
              end;
            end;
          8:
            begin
              if Integer(MagicElementData.Data.TimeAddIsPoint) <> TempValue then
              begin
                MagicElementData.Data.TimeAddIsPoint := TempValue = 1;
                IsChanged := True;
              end;
            end;
        end;
      end;
    10:
      begin
        S := (FEdit as TEdit).Text;
        if not SameText(MagicElementData.Data.HintText, S) then
        begin
          MagicElementData.Data.HintText := S;
          IsChanged := True;
        end;
      end;
  end;

  if FTree.CanFocus then
    FTree.SetFocus;
  if FEdit.Visible then
    FEdit.Visible := False;

  if IsChanged then
  begin
    if (FTree.Owner is TFrmCustomMagic) then
    begin
      TFrmCustomMagic(FTree.Owner).SetConfigChanged();
    end;
  end;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TElementPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TElementPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  MagicElementData: PMagicElementData;
  boUnit: Boolean;
begin
  Result := True;
  FTree := Tree as TVirtualStringTree;
  FNode := Node;
  FColumn := Column;

  // determine what edit type actually is needed
  if FEdit <> nil then
  begin
    FEdit.Free;
    FEdit := nil;
  end;

  MagicElementData := FTree.GetNodeData(Node);

  case FColumn of
    1, 2, 3, 5, 6, 7:
      begin
        FEdit := TSpinEditEx.Create(nil);

        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          if FColumn in [1] then
          begin
            MinValue := 0;
            MaxValue := 100;
          end;

          case FColumn of
            1:
              Value := MagicElementData.Data.Rate;
            2:
              Value := MagicElementData.Data.RateAdd;
            3:
              Value := MagicElementData.Data.Value;
            5:
              Value := MagicElementData.Data.ValueAdd;
            6:
              Value := MagicElementData.Data.Time;
            7:
              Value := MagicElementData.Data.TimeAdd;
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    4, 8:
      begin
        FEdit := TComboBox.Create(nil);

        with FEdit as TComboBox do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDownList;

          if FColumn = 4 then
          begin
            for boUnit := Low(Boolean) to High(Boolean) do
            begin
              Items.Add(MagicAttackDecValueTypeNames[boUnit]);
            end;
            ItemIndex := Integer(MagicElementData.Data.ValueIsPoint)
          end
          else if FColumn = 8 then
          begin
            for boUnit := Low(Boolean) to High(Boolean) do
            begin
              Items.Add(MagicAttackDecTimeTypeNames[boUnit]);
            end;
            ItemIndex := Integer(MagicElementData.Data.TimeAddIsPoint);
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    10:
      begin
        FEdit := TEdit.Create(nil);

        with FEdit as TEdit do
        begin
          Visible := False;
          Parent := Tree;
          Text := MagicElementData.Data.HintText;
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  end;
end;

// ----------------------------------------------------------------------------------------------------------------------

procedure TElementPropertyEditLink.ProcessMessage(var Message: TMessage);
begin
  FEdit.WindowProc(Message);
end;

// ----------------------------------------------------------------------------------------------------------------------

procedure TElementPropertyEditLink.SetBounds(R: TRect);
var
  Dummy: Integer;
begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right);
  FEdit.BoundsRect := R;
end;

procedure SetControlEnabled(WinControl: TWinControl; Value: Boolean);
var
  I: Integer;
  Ctrl: TControl;
  WinCtrl: TWinControl;
begin
  for I := 0 to WinControl.ControlCount - 1 do
  begin
    Ctrl := WinControl.Controls[I];
    if Ctrl is TWinControl then
    begin
      WinCtrl := Ctrl as TWinControl;
      if (Ctrl is TTabSheet) or (Ctrl is TPanel) or (Ctrl is TGroupBox) then
        SetControlEnabled(WinCtrl, Value)
      else
        WinCtrl.Enabled := Value;
    end;
  end;
end;

function ShowCustomMagic: Boolean;
var
  FrmCustomMagic: TFrmCustomMagic;
begin
  FrmCustomMagic := TFrmCustomMagic.Create(nil);
  try
    FrmCustomMagic.DoOpen;
    Result := FrmCustomMagic.ShowModal = mrOK;
  finally
    FrmCustomMagic.Free;
  end;
end;

procedure TFrmCustomMagic.DoOpen;
var
  I: Integer;
  Node: PVirtualNode;
  CustomMagicConfig: TCustomMagicConfig;
  ConfigNodeData: PMagicConfigNodeData;
begin
  chkSendCustomMagicConfig.Checked := g_Config.boSendCustomMagicConfig;

{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_CustomMagicList.LockR(3);
  try
{$IFEND}
    for I := 0 to UserEngine.m_CustomMagicList.Count - 1 do
    begin
      CustomMagicConfig := UserEngine.m_CustomMagicList.Items[I];

      Node := vstCustomMagic.AddChild(nil);
      ConfigNodeData := vstCustomMagic.GetNodeData(Node);
      ConfigNodeData.Config := CustomMagicConfig
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomMagicList.UnLockR;
  end;
{$IFEND}
end;

procedure TFrmCustomMagic.FormCreate(Sender: TObject);
var
  I: Integer;
  DrawMode: TCustomDrawMode;
  DirCount: TCustomDirCount;
  DrawOrder: TCustomDrawOrder;
  OperateMode: TCustomOperateMode;
  DirCalcType: TCustomDirCalcType;

  // AttackMode: TCustomAttackMode;
  AttackTarget: TCustomAttackTarget;
  AttackPowerCalc: TCustomAttackPowerCalc;
  MagicPlusLevel: TMagicPlusLevel;
  MagicActionType: TMagicActionType;
  NeedItem: TMagicNeedItem;
  CheckVarType: TCheckVarType;
  MagicSwitchMode: TMagicSwitchMode;
  MagicWarrNGOption: TMagicWarrNGOption;
begin
  vstAttackDecAttr.NodeDataSize := SizeOf(TAttackDecAttribData);
  vstDecElement.NodeDataSize := SizeOf(TMagicElementData);

  vstProtectedAddAttr.NodeDataSize := SizeOf(TProtectAddAttribData);
  vstAddElement.NodeDataSize := SizeOf(TMagicElementData);

  cbbClientLevel.Items.Clear;
  for MagicPlusLevel := Low(MagicPlusLevelNames) to High(MagicPlusLevelNames) do
    cbbClientLevel.Items.Add(MagicPlusLevelNames[MagicPlusLevel]);

  cbbClientActionType.Items.Clear;
  for MagicActionType := Low(TMagicActionType) to High(TMagicActionType) do
    cbbClientActionType.Items.Add(MagicActionTypeNames[MagicActionType]);

  cbbMagicSwitchMode.Items.Clear;
  for MagicSwitchMode := Low(TMagicSwitchMode) to High(TMagicSwitchMode) do
    cbbMagicSwitchMode.Items.Add(MagicSwitchModeNames[MagicSwitchMode]);

  cbbMagicWarrNGOption.Items.Clear;
  for MagicWarrNGOption := Low(TMagicWarrNGOption) to High(TMagicWarrNGOption) do
    cbbMagicWarrNGOption.Items.Add(MagicWarrNGOptionNames[MagicWarrNGOption]);

  cbbClientIconFile.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientIconFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientFlyFile.Items.Clear;
  cbbClientFlyFile.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientFlyFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientFlyDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientFlyDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientFlyDirCount.Items.Clear;
  for DirCount := Low(TCustomDirCount) to High(TCustomDirCount) do
    cbbClientFlyDirCount.Items.Add(CustomDirNames[DirCount]);

  cbbClientFlyEffFile.Items.Clear;
  cbbClientFlyEffFile.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientFlyEffFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientFlyEffDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientFlyEffDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientSelfFile.Clear;
  cbbClientSelfFile.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientSelfFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientSelfDrawOrder.Clear;
  for DrawOrder := Low(TCustomDrawOrder) to High(TCustomDrawOrder) do
    cbbClientSelfDrawOrder.Items.Add(CustomDrawOrderNames[DrawOrder]);

  cbbClientSelfDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientSelfDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientSelfDirCount.Items.Clear;
  for DirCount := Low(TCustomDirCount) to High(TCustomDirCount) do
    cbbClientSelfDirCount.Items.Add(CustomDirNames[DirCount]);

  cbbClientSelfDirCalcType.Items.Clear;
  for DirCalcType := Low(TCustomDirCalcType) to High(TCustomDirCalcType) do
    cbbClientSelfDirCalcType.Items.Add(CustomDirCalcTypeNames[DirCalcType]);

  cbbClientSelfKeepFile.Clear;
  cbbClientSelfKeepFile.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientSelfKeepFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientSelfKeepDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientSelfKeepDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientFastMoveFile.Clear;
  cbbClientFastMoveFile.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientFastMoveFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientFastMoveDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientFastMoveDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientPreTargetDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientPreTargetDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientPreTargetDrawMode2.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientPreTargetDrawMode2.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientPreTargetFile.Clear;
  cbbClientPreTargetFile.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientPreTargetFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientTargetDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientTargetDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientTargetDrawMode2.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientTargetDrawMode2.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientTargetFile.Clear;
  cbbClientTargetFile.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientTargetFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbTargetStatus1_File.Clear;
  cbbTargetStatus1_File.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbTargetStatus1_File.Items.Add(g_EffectImageList.Strings[I]);

  cbbTargetStatus2_File.Clear;
  cbbTargetStatus2_File.Items.Add('无');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbTargetStatus2_File.Items.Add(g_EffectImageList.Strings[I]);

  cbbTargetStatus1_DrawMode.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbTargetStatus1_DrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbTargetStatus2_DrawMode.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbTargetStatus2_DrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbOperateMode.Items.Clear;
  for OperateMode := Low(TCustomOperateMode) to High(TCustomOperateMode) do
    cbbOperateMode.Items.Add(CustomOperateModeNames[OperateMode]);

  cbbAttackTarget.Items.Clear;
  for AttackTarget := Low(TCustomAttackTarget) to High(TCustomAttackTarget) do
    cbbAttackTarget.Items.Add(CustomAttackTargetNames[AttackTarget]);

  cbbAttackPowerCalc.Items.Clear;
  for AttackPowerCalc := Low(TCustomAttackPowerCalc) to High(TCustomAttackPowerCalc) do
    cbbAttackPowerCalc.Items.Add(CustomAttackPowerCalcNames[AttackPowerCalc]);

  cbbAttackPowerLevel.Items.Clear;
  for I := Low(CustomMagicLevelNames) to High(CustomMagicLevelNames) do
    cbbAttackPowerLevel.Items.Add(CustomMagicLevelNames[I]);

  cbbNeedItem.Items.Clear;
  for NeedItem := Low(TMagicNeedItem) to High(TMagicNeedItem) do
    cbbNeedItem.Items.Add(MagicNeedItemNames[NeedItem]);

  cbbCheckVarType.Items.Clear;
  for CheckVarType := Low(TCheckVarType) to High(TCheckVarType) do
    cbbCheckVarType.Items.Add(CheckVarTypeNames[CheckVarType]);

  lblCheckVarName.Top := lblNeedItem.Top;
  edtCheckVarName.Top := cbbNeedItem.Top;
  lblCheckVarType.Top := lblNeedItemCount.Top;
  cbbCheckVarType.Top := seNeedItemCount.Top;
  lblCheckVarValue.Top := lblNeedItemCustomItemName.Top;
  seCheckVarValue.Top := edtNeedItemCustomItemName.Top;
  lblCheckVarAdd.Top := lblCheckVarValue.Top;
  seCheckVarAdd.Top := seCheckVarValue.Top;

  SetControlEnabled(pgcMain, False);

  FCurrentCustomConfig := nil;
  FCurrentClientConfig := nil;
  FCurrentServerConfig := nil;

  pgcMain.ActivePageIndex := 0;
  pgcClient.ActivePageIndex := 0;

  // DoOpen;
end;

procedure TFrmCustomMagic.SetConfigChanged(IsChanged: Boolean);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.SetChanged(IsChanged);
    if vstCustomMagic.FocusedNode <> nil then
      vstCustomMagic.InvalidateNode(vstCustomMagic.FocusedNode);

    FIsConfigChanged := True;
    if not btnSave.Enabled then
      btnSave.Enabled := True;
  end;
end;

procedure TFrmCustomMagic.vstCustomMagicDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column:
  TColumnIndex; const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
var
  ConfigNodeData: PMagicConfigNodeData;
begin
  ConfigNodeData := Sender.GetNodeData(Node);

  if ConfigNodeData <> nil then
  begin
    if ConfigNodeData.Config.IsChanged then
      TargetCanvas.Font.Color := clRed
    else if Sender.Selected[Node] and (Sender.Focused) then
      TargetCanvas.Font.Color := clHighlightText
    else
      TargetCanvas.Font.Color := Sender.Font.Color;
  end;
end;

procedure TFrmCustomMagic.vstCustomMagicGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
begin
  NodeDataSize := SizeOf(TCustomMagicConfig);
end;

procedure TFrmCustomMagic.vstCustomMagicGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType:
  TVSTTextType; var CellText: string);
var
  ConfigNodeData: PMagicConfigNodeData;
begin
  ConfigNodeData := Sender.GetNodeData(Node);
  if ConfigNodeData <> nil then
    CellText := ConfigNodeData.Config.MagicName;
end;

procedure TFrmCustomMagic.vstCustomMagicNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  OldChanged, OldIsConfigCanSave: Boolean;
  ConfigNodeData: PMagicConfigNodeData;
  DecAttribType: TMagicAttackDecAttributesType;
  Node: PVirtualNode;
  DecAttribData: PAttackDecAttribData;
  AddAttribType: TMagicProtectAddAttributesType;
  AddAttribData: PProtectAddAttribData;
  ElementType: TItemElementsType;
  MagicElementData: PMagicElementData;
begin
  FCurrentCustomConfig := nil;
  FCurrentClientConfig := nil;
  FCurrentServerConfig := nil;

  if vstCustomMagic.FocusedNode = nil then
    Exit;

  SetControlEnabled(pgcMain, True);

  ConfigNodeData := vstCustomMagic.GetNodeData(vstCustomMagic.FocusedNode);
  if ConfigNodeData = nil then
    Exit;

  OldIsConfigCanSave := FIsConfigChanged;

  FCurrentCustomConfig := ConfigNodeData.Config;
  FCurrentClientConfig := @FCurrentCustomConfig.ClientBaseConfig;
  FCurrentServerConfig := @FCurrentCustomConfig.ServerConfig;

  OldChanged := FCurrentCustomConfig.IsChanged;

  cbbClientLevel.ItemIndex := 0;
  cbbClientLevel.OnChange(cbbClientLevel);

  // chkClientLevelEnabled.Checked := FCurrentCustomConfig.ClientBaseConfig.MagicLevelEnabled;
  // chkClientWarr.Checked := FCurrentCustomConfig.ClientBaseConfig.MagicWarr;
  chkClientLock.Checked := FCurrentCustomConfig.ClientBaseConfig.MagicLock;
  chkClientLockSelf.Checked := FCurrentCustomConfig.ClientBaseConfig.MagicLockSelf;

  if FCurrentCustomConfig.IsMagicWarr then
  begin
    txtMagicWarr.Caption := '战士技能';
    txtMagicWarr.Font.Color := clRed;

    SetControlEnabled(grpFly, False);
    grpFly.Enabled := False;

    SetControlEnabled(grpFlyEff, False);
    grpFlyEff.Enabled := False;

    SetControlEnabled(grpMove, False);

    if FCurrentServerConfig.OperateMode <> momAttack then
    begin
      cbbOperateMode.ItemIndex := Integer(momAttack);
      FCurrentServerConfig.OperateMode := momAttack;

      cbbOperateMode.OnChange(cbbOperateMode);
    end;

    // lblMagicWarrNGOption.Visible := True;
    cbbMagicWarrNGOption.Enabled := True;
    cbbMagicWarrNGOption.ItemIndex := Integer(FCurrentCustomConfig.ClientBaseConfig.MagicWarrNGOption);

    cbbOperateMode.Enabled := False;
    cbbMagicSwitchMode.Enabled := True;
    chkSwitchModeNoClose.Enabled := FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode = msmSwitch;
    chkMagicAutoOpen.Enabled := FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode = msmSwitch;

    seClientSelfPlayTime.Enabled := True;
    chkClientTargetMultiPlay.Enabled := False;
    chkClientTargetKeepPlay.Enabled := False;
    seClientTargetKeepTime.Enabled := False;
    seClientTargetKeepAttackInterval.Enabled := False;
    seClientTargetKeepAttackRange.Enabled := False;
    chkClientTargetKeepMultiPlay.Enabled := False;
    seTargetKeepLightRange.Enabled := False;

    cbbClientActionType.ItemIndex := Integer(FCurrentCustomConfig.ClientBaseConfig.MagicActionType);
    cbbMagicSwitchMode.ItemIndex := Integer(FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode);
    chkSwitchModeNoClose.Checked := FCurrentCustomConfig.ClientBaseConfig.SwitchModeNoClose;
    chkMagicAutoOpen.Checked := FCurrentCustomConfig.ClientBaseConfig.MagicAutoOpen;

    {
      //FCurrentCustomConfig.ClientBaseConfig.MagicActionType := matHit;
      cbbClientActionType.Enabled := False;
      seClientActionStartIndex.Enabled := False;
      seClientActionPlayCount.Enabled := False;
      seClientActionEmptyCount.Enabled := False;
      chkClientActionContinue.Enabled := False;
    }

    cbbClientActionType.Enabled := True;
    seClientActionStartIndex.Enabled := FCurrentCustomConfig.ClientBaseConfig.MagicActionType = matCustom;
    seClientActionPlayCount.Enabled := seClientActionStartIndex.Enabled;
    seClientActionEmptyCount.Enabled := seClientActionStartIndex.Enabled;
    chkClientActionContinue.Enabled := seClientActionStartIndex.Enabled { and (not chkMagicSwitchMode.Enabled) };

    seClientActionStartIndex.Value := FCurrentCustomConfig.ClientBaseConfig.MagicActionStartIndex;
    seClientActionPlayCount.Value := FCurrentCustomConfig.ClientBaseConfig.MagicActionPlayCount;
    seClientActionEmptyCount.Value := FCurrentCustomConfig.ClientBaseConfig.MagicActionEmptyCount;
    chkClientActionContinue.Checked := FCurrentCustomConfig.ClientBaseConfig.MagicActionContinue;

    seClientPreTargetEmptyCount.Enabled := True;
    chkClientPreTargetCalcDir.Enabled := True;

    chkEnableAntiMagic.Enabled := False;
    chkEnableAntiMagic.Checked := False;

    chkClientNotRaiseHand.Enabled := False;
    chkClientNotRaiseHand.Checked := False;

    chkEnableHitPoint.Enabled := True;
    chkEnableHitPoint.Checked := FCurrentCustomConfig.ServerConfig.EnableHitPoint;

    FCurrentServerConfig.AttackMode := mamNear;
  end
  else
  begin
    txtMagicWarr.Caption := '非战士技能';
    txtMagicWarr.Font.Color := clBlue;

    SetControlEnabled(grpFly, True);
    grpFly.Enabled := True;

    SetControlEnabled(grpFlyEff, True);
    grpFlyEff.Enabled := True;

    SetControlEnabled(grpMove, True);

    cbbOperateMode.Enabled := True;
    cbbMagicSwitchMode.Enabled := False;
    cbbMagicSwitchMode.ItemIndex := 0;

    chkSwitchModeNoClose.Enabled := False;
    chkSwitchModeNoClose.Checked := False;
    chkMagicAutoOpen.Enabled := False;

    // lblMagicWarrNGOption.Visible := False;
    cbbMagicWarrNGOption.ItemIndex := 0;
    cbbMagicWarrNGOption.Enabled := False;

    seClientSelfPlayTime.Enabled := True;
    chkClientTargetMultiPlay.Enabled := True;
    chkClientTargetKeepPlay.Enabled := True;
    seClientTargetKeepTime.Enabled := True;
    seClientTargetKeepAttackInterval.Enabled := True;
    seClientTargetKeepAttackRange.Enabled := True;
    chkClientTargetKeepMultiPlay.Enabled := True;
    seTargetKeepLightRange.Enabled := True;

    FCurrentServerConfig.AttackMode := mamFar;
    FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode := msmNone;

    cbbClientActionType.ItemIndex := Integer(FCurrentCustomConfig.ClientBaseConfig.MagicActionType);
    cbbClientActionType.Enabled := True;

    seClientActionStartIndex.Enabled := FCurrentCustomConfig.ClientBaseConfig.MagicActionType = matCustom;
    seClientActionPlayCount.Enabled := seClientActionStartIndex.Enabled;
    seClientActionEmptyCount.Enabled := seClientActionStartIndex.Enabled;
    chkClientActionContinue.Enabled := seClientActionStartIndex.Enabled { and (not chkMagicSwitchMode.Enabled) };

    seClientActionStartIndex.Value := FCurrentCustomConfig.ClientBaseConfig.MagicActionStartIndex;
    seClientActionPlayCount.Value := FCurrentCustomConfig.ClientBaseConfig.MagicActionPlayCount;
    seClientActionEmptyCount.Value := FCurrentCustomConfig.ClientBaseConfig.MagicActionEmptyCount;
    chkClientActionContinue.Checked := FCurrentCustomConfig.ClientBaseConfig.MagicActionContinue;

    chkClientNotRaiseHand.Checked := FCurrentCustomConfig.ClientBaseConfig.NotRaiseHand;
    chkClientNotRaiseHand.Enabled := True;

    chkEnableAntiMagic.Enabled := True;
    chkEnableAntiMagic.Checked := FCurrentCustomConfig.ServerConfig.EnableAntiMagic;

    seClientPreTargetEmptyCount.Enabled := False;
    chkClientPreTargetCalcDir.Enabled := False;

    chkEnableHitPoint.Enabled := False;
    chkEnableHitPoint.Checked := False;
  end;

  // -----------------------------------server config ---------------------------
  cbbOperateMode.ItemIndex := Integer(FCurrentCustomConfig.ServerConfig.OperateMode);
  cbbOperateMode.OnChange(cbbOperateMode);

  chkAttackUseNG.Checked := FCurrentCustomConfig.ServerConfig.IsAttackUseNG;
  chkAttackNoChangeDir.Checked := FCurrentCustomConfig.ServerConfig.NoChangeDir;
  chkDisableInSafeZone.Checked := FCurrentCustomConfig.ServerConfig.DisableInSafeZone;
  seAttackDelayTime.Value := FCurrentCustomConfig.ServerConfig.AttackDelayTime;
  seUseInterval.Value := FCurrentCustomConfig.ServerConfig.UseInterval;
  // chkFailNoShowEff.Checked := FCurrentCustomConfig.ServerConfig.FailNoShowEff;
  edtFailMsg.Text := FCurrentCustomConfig.ServerConfig.FailMsg;
  edtSucceedMsg.Text := FCurrentCustomConfig.ServerConfig.SucceedMsg;
  edtCloseMsg.Text := FCurrentCustomConfig.ServerConfig.CloseMsg;

  chkCheckVarValue.Checked := FCurrentCustomConfig.ServerConfig.IsCheckVarValue;

  cbbNeedItem.ItemIndex := Integer(FCurrentCustomConfig.ServerConfig.NeedItem);
  seNeedItemCount.Value := FCurrentCustomConfig.ServerConfig.NeedItemCount;
  edtNeedItemCustomItemName.Text := FCurrentCustomConfig.ServerConfig.NeedItemCustomItemName;
  chkNeedItemUseBagItem.Checked := FCurrentCustomConfig.ServerConfig.NeedItemUseBagItem;

  edtCheckVarName.Text := FCurrentCustomConfig.ServerConfig.CheckVarName;
  cbbCheckVarType.ItemIndex := Integer(FCurrentCustomConfig.ServerConfig.CheckVarType);
  seCheckVarValue.Value := FCurrentCustomConfig.ServerConfig.CheckVarValue;
  seCheckVarAdd.Value := FCurrentCustomConfig.ServerConfig.CheckVarAdd;

  lblNeedItem.Visible := not chkCheckVarValue.Checked;
  cbbNeedItem.Visible := lblNeedItem.Visible;
  lblNeedItemCount.Visible := lblNeedItem.Visible;
  seNeedItemCount.Visible := lblNeedItem.Visible;
  lblNeedItemCustomItemName.Visible := lblNeedItem.Visible;
  edtNeedItemCustomItemName.Visible := lblNeedItem.Visible;
  chkNeedItemUseBagItem.Visible := lblNeedItem.Visible;

  lblCheckVarName.Visible := chkCheckVarValue.Checked;
  edtCheckVarName.Visible := lblCheckVarName.Visible;
  lblCheckVarType.Visible := lblCheckVarName.Visible;
  cbbCheckVarType.Visible := lblCheckVarName.Visible;
  lblCheckVarValue.Visible := lblCheckVarName.Visible;
  seCheckVarValue.Visible := lblCheckVarName.Visible;
  lblCheckVarAdd.Visible := lblCheckVarName.Visible;
  seCheckVarAdd.Visible := lblCheckVarName.Visible;

  cbbAttackTarget.ItemIndex := Integer(FCurrentCustomConfig.ServerConfig.AttackTarget);
  seAttackNearRange.Value := FCurrentCustomConfig.ServerConfig.AttackNearRange;
  seAttackGroupRange.Value := FCurrentCustomConfig.ServerConfig.AttackGroupRange;
  seAttackLineWidth.Value := FCurrentCustomConfig.ServerConfig.AttackLineWidth;
  chkEnableAntiMagic.Checked := FCurrentCustomConfig.ServerConfig.EnableAntiMagic;
  chkEnableHitPoint.Checked := FCurrentCustomConfig.ServerConfig.EnableHitPoint;

  cbbAttackPowerCalc.ItemIndex := Integer(FCurrentCustomConfig.ServerConfig.AttackPowerCalc);
  cbbAttackPowerLevel.ItemIndex := 0;
  seAttackPowerRate.Value := FCurrentCustomConfig.ServerConfig.AttackPowerRates[0];

  lblLineAttackAddPower.Visible := FCurrentServerConfig.AttackTarget in [matLine, matDir8, matDir16];
  seLineAttackAddPower.Visible := lblLineAttackAddPower.Visible;
  lblLineAttackAddPowerPerc.Visible := lblLineAttackAddPower.Visible;
  seLineAttackAddPower.Value := FCurrentServerConfig.AttackPowerLineAdd;
  seAttackPowerUndeadAdd.Value := FCurrentServerConfig.AttackPowerUndeadAdd;

  chkEnabledCallMonster.Checked := FCurrentServerConfig.EnabledCallMonster;
  seCallMonstersRate.Value := FCurrentServerConfig.CallMonstersRate;
  seCallMonstersRoyaltySec.Value := FCurrentServerConfig.CallMonstersRoyaltySec;
  seCallMonstersLevel.Value := FCurrentServerConfig.CallMonstersLevel;
  edtCallMonster1.Text := FCurrentServerConfig.CallMonsters[0];
  seCallMonsterNum1.Value := FCurrentServerConfig.CallMonsterNums[0];

  edtCallMonster2.Text := FCurrentServerConfig.CallMonsters[1];
  seCallMonsterNum2.Value := FCurrentServerConfig.CallMonsterNums[1];

  chkAttackTeleportAttack.Checked := FCurrentServerConfig.AttackTeleportAttack;
  chkNoTeleportNoAttack.Checked := FCurrentServerConfig.IsNoTeleportNoAttack;
  seAttackTeleportRate.Value := FCurrentServerConfig.AttackTeleportRate;
  chkAttackTeleportRunHum.Checked := FCurrentServerConfig.AttackTeleportRunHum;
  chkAttackTeleportRunMon.Checked := FCurrentServerConfig.AttackTeleportRunMon;
  chkAttackTeleportRunNpc.Checked := FCurrentServerConfig.AttackTeleportRunNpc;
  chkAttackTeleportRunGuard.Checked := FCurrentServerConfig.AttackTeleportRunGuard;
  chkAttackTeleportRunObstacle.Checked := FCurrentServerConfig.AttackTeleportRunObstacle;
  chkAttackTeleportWarDisHumRun.Checked := FCurrentServerConfig.AttackTeleportWarDisHumRun;
  chkAttackTeleportCannotRunItem.Checked := FCurrentServerConfig.AttackTeleportCannotRunItem;

  chkAttackTeleportRush.Checked := FCurrentServerConfig.AttackTeleportRush;
  seAttackTeleportRushCount.Value := FCurrentServerConfig.AttackTeleportRushCount;
  chkAttackTeleportAfterDamage.Checked := FCurrentServerConfig.AttackTeleportAfterDamage;

  chkProtectAddHPSlow.Checked := FCurrentCustomConfig.ServerConfig.ProtectAddHPSlow;
  seProtectAddHPSlowCount.Value := FCurrentCustomConfig.ServerConfig.ProtectAddHpSlowCount;

  chkProtectTargetStatus.Checked := FCurrentCustomConfig.ServerConfig.ProtectTargetStatus;
  seProtectTargetStatusTime.Value := FCurrentCustomConfig.ServerConfig.ProtectTargetStatusTime;
  cbbProtectTargetStatusTime_1.ItemIndex := FCurrentCustomConfig.ServerConfig.ProtectTargetStatusTimeUnit;
  seProtectTargetStatusTime_2.Value := FCurrentCustomConfig.ServerConfig.ProtectTargetStatusTime2;
  seProtectTargetStatusTimeDelay.Value := FCurrentCustomConfig.ServerConfig.ProtectTargetStatusDelay;

  seProtectTargetRange.Value := FCurrentCustomConfig.ServerConfig.ProtectTargetRange;
  // seProtectSelfRate.Value := FCurrentCustomConfig.ServerConfig.ProtectSelfRate;

  vstProtectedAddAttr.Clear;
  for AddAttribType := Low(TMagicProtectAddAttributesType) to High(TMagicProtectAddAttributesType) do
  begin
    Node := vstProtectedAddAttr.AddChild(nil);
    Node.CheckType := ctCheckBox;
    if FCurrentServerConfig.ProtectAddAttrib[AddAttribType].IsChecked then
      Node.CheckState := csCheckedNormal
    else
      Node.CheckState := csUnCheckedNormal;

    AddAttribData := vstProtectedAddAttr.GetNodeData(Node);
    AddAttribData.AttribType := AddAttribType;
    AddAttribData.Data := @FCurrentServerConfig.ProtectAddAttrib[AddAttribType];
  end;

  vstAddElement.Clear;
  for ElementType := Low(TItemElementsType) to High(TItemElementsType) do
  begin
    Node := vstAddElement.AddChild(nil);
    Node.CheckType := ctCheckBox;
    if FCurrentServerConfig.ProtectAddElements[ElementType].IsChecked then
      Node.CheckState := csCheckedNormal
    else
      Node.CheckState := csUnCheckedNormal;

    MagicElementData := vstAddElement.GetNodeData(Node);
    MagicElementData.ElementType := ElementType;
    MagicElementData.Data := @FCurrentServerConfig.ProtectAddElements[ElementType];
  end;

  // MagicProtectedAddAttributesTypeNames

  chkAdditional0.Checked := FCurrentServerConfig.Additionals[0].Checked;
  seAdditionalRate0.Value := FCurrentServerConfig.Additionals[0].Rate;
  seAdditionalRate0_2.Value := FCurrentServerConfig.Additionals[0].Rate2;
  seAdditionalTime0.Value := FCurrentServerConfig.Additionals[0].Time;
  cbbAdditionalTime0_1.ItemIndex := FCurrentServerConfig.Additionals[0].TimeUnit;
  seAdditionalTime0_2.Value := FCurrentServerConfig.Additionals[0].Time2;
  seAdditionaHP0.Value := FCurrentServerConfig.AdditionalHP0;

  chkAdditional1.Checked := FCurrentServerConfig.Additionals[1].Checked;
  seAdditionalRate1.Value := FCurrentServerConfig.Additionals[1].Rate;
  seAdditionalRate1_2.Value := FCurrentServerConfig.Additionals[1].Rate2;
  seAdditionalTime1.Value := FCurrentServerConfig.Additionals[1].Time;
  cbbAdditionalTime1_1.ItemIndex := FCurrentServerConfig.Additionals[1].TimeUnit;
  seAdditionalTime1_2.Value := FCurrentServerConfig.Additionals[1].Time2;

  chkAdditional2.Checked := FCurrentServerConfig.Additionals[2].Checked;
  seAdditionalRate2.Value := FCurrentServerConfig.Additionals[2].Rate;
  seAdditionalRate2_2.Value := FCurrentServerConfig.Additionals[2].Rate2;
  seAdditionalTime2.Value := FCurrentServerConfig.Additionals[2].Time;
  cbbAdditionalTime2_1.ItemIndex := FCurrentServerConfig.Additionals[2].TimeUnit;
  seAdditionalTime2_2.Value := FCurrentServerConfig.Additionals[2].Time2;

  chkAdditional3.Checked := FCurrentServerConfig.Additionals[3].Checked;
  seAdditionalRate3.Value := FCurrentServerConfig.Additionals[3].Rate;
  seAdditionalRate3_2.Value := FCurrentServerConfig.Additionals[3].Rate2;
  seAdditionalTime3.Value := FCurrentServerConfig.Additionals[3].Time;
  cbbAdditionalTime3_1.ItemIndex := FCurrentServerConfig.Additionals[3].TimeUnit;
  seAdditionalTime3_2.Value := FCurrentServerConfig.Additionals[3].Time2;

  chkAdditional4.Checked := FCurrentServerConfig.Additionals[4].Checked;
  seAdditionalRate4.Value := FCurrentServerConfig.Additionals[4].Rate;
  seAdditionalRate4_2.Value := FCurrentServerConfig.Additionals[4].Rate2;
  seAdditionalTime4.Value := FCurrentServerConfig.Additionals[4].Time;
  seAdditionalTime4_2.Value := FCurrentServerConfig.Additionals[4].Time2;
  chkseAdditionaHighLevel4.Checked := FCurrentServerConfig.AdditionalHighLevel4;
  cbbPushedType4.ItemIndex := FCurrentServerConfig.AdditionalPushedType4;

  chkAdditional5.Checked := FCurrentServerConfig.Additionals[5].Checked;
  seAdditionalRate5.Value := FCurrentServerConfig.Additionals[5].Rate;
  seAdditionalRate5_2.Value := FCurrentServerConfig.Additionals[5].Rate2;
  seAdditionalTime5.Value := FCurrentServerConfig.Additionals[5].Time;
  seAdditionalTime5_2.Value := FCurrentServerConfig.Additionals[5].Time2;

  chkAdditional6.Checked := FCurrentServerConfig.Additionals[6].Checked;
  seAdditionalRate6.Value := FCurrentServerConfig.Additionals[6].Rate;
  seAdditionalRate6_2.Value := FCurrentServerConfig.Additionals[6].Rate2;
  seAdditionalTime6.Value := FCurrentServerConfig.Additionals[6].Time;
  seAdditionalTime6_2.Value := FCurrentServerConfig.Additionals[6].Time2;

  chkAdditional7.Checked := FCurrentServerConfig.Additionals[7].Checked;
  seAdditionalRate7.Value := FCurrentServerConfig.Additionals[7].Rate;
  seAdditionalRate7_2.Value := FCurrentServerConfig.Additionals[7].Rate2;
  seAdditionalTime7.Value := FCurrentServerConfig.Additionals[7].Time;
  cbbAdditionalTime7_1.ItemIndex := FCurrentServerConfig.Additionals[7].TimeUnit;
  seAdditionalTime7_2.Value := FCurrentServerConfig.Additionals[7].Time2;

  chkAdditional8.Checked := FCurrentServerConfig.Additionals[8].Checked;
  seAdditionalRate8.Value := FCurrentServerConfig.Additionals[8].Rate;
  seAdditionalRate8_2.Value := FCurrentServerConfig.Additionals[8].Rate2;
  seAdditionalTime8.Value := FCurrentServerConfig.Additionals[8].Time;
  cbbAdditionalTime8_1.ItemIndex := FCurrentServerConfig.Additionals[8].TimeUnit;
  seAdditionalTime8_2.Value := FCurrentServerConfig.Additionals[8].Time2;

  chkAdditional9.Checked := FCurrentServerConfig.Additionals[9].Checked;
  seAdditionalRate9.Value := FCurrentServerConfig.Additionals[9].Rate;
  seAdditionalRate9_2.Value := FCurrentServerConfig.Additionals[9].Rate2;
  seAdditionalTime9.Value := FCurrentServerConfig.Additionals[9].Time;
  cbbAdditionalTime9_1.ItemIndex := FCurrentServerConfig.Additionals[9].TimeUnit;
  seAdditionalTime9_2.Value := FCurrentServerConfig.Additionals[9].Time2;

  chkAdditional10.Checked := FCurrentServerConfig.Additionals[10].Checked;
  seAdditionalRate10.Value := FCurrentServerConfig.Additionals[10].Rate;
  seAdditionalRate10_2.Value := FCurrentServerConfig.Additionals[10].Rate2;
  seAdditionalTime10.Value := FCurrentServerConfig.Additionals[10].Time;
  cbbAdditionalTime10_1.ItemIndex := FCurrentServerConfig.Additionals[10].TimeUnit;
  seAdditionalTime10_2.Value := FCurrentServerConfig.Additionals[10].Time2;

  chkAttackTargetStatus.Checked := FCurrentCustomConfig.ServerConfig.AttackTargetStatus;
  seAttackTargetStatusTime.Value := FCurrentCustomConfig.ServerConfig.AttackTargetStatusTime;
  cbbAttackTargetStatusTime_1.ItemIndex := FCurrentCustomConfig.ServerConfig.AttackTargetStatusTimeUnit;
  seAttackTargetStatusTime_2.Value := FCurrentCustomConfig.ServerConfig.AttackTargetStatusTime2;
  seAttackTargetStatusDelay.Value := FCurrentCustomConfig.ServerConfig.AttackTargetStatusDelay;

  vstAttackDecAttr.Clear;
  for DecAttribType := Low(TMagicAttackDecAttributesType) to High(TMagicAttackDecAttributesType) do
  begin
    Node := vstAttackDecAttr.AddChild(nil);
    Node.CheckType := ctCheckBox;
    if FCurrentServerConfig.AttackSubAttrib[DecAttribType].IsChecked then
      Node.CheckState := csCheckedNormal
    else
      Node.CheckState := csUnCheckedNormal;

    DecAttribData := vstAttackDecAttr.GetNodeData(Node);
    DecAttribData.AttribType := DecAttribType;
    DecAttribData.Data := @FCurrentServerConfig.AttackSubAttrib[DecAttribType];
  end;

  vstDecElement.Clear;
  for ElementType := Low(TItemElementsType) to High(TItemElementsType) do
  begin
    Node := vstDecElement.AddChild(nil);
    Node.CheckType := ctCheckBox;
    if FCurrentServerConfig.AttackSubElements[ElementType].IsChecked then
      Node.CheckState := csCheckedNormal
    else
      Node.CheckState := csUnCheckedNormal;

    MagicElementData := vstDecElement.GetNodeData(Node);
    MagicElementData.ElementType := ElementType;
    MagicElementData.Data := @FCurrentServerConfig.AttackSubElements[ElementType];
  end;

  chkMagicACHum.Checked := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumDefense].IsChecked;
  seMagicACHumRate.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumDefense].Rate;
  seMagicACHumRateAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumDefense].RateAdd;
  seMagicACHumValue.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumDefense].Value;
  seMagicACHumValueAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumDefense].ValueAdd;

  chkMagicACMon.Checked := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonDefense].IsChecked;
  seMagicACMonRate.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonDefense].Rate;
  seMagicACMonRateAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonDefense].RateAdd;
  seMagicACMonValue.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonDefense].Value;
  seMagicACMonValueAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonDefense].ValueAdd;

  chkMagicACHero.Checked := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroDefense].IsChecked;
  seMagicACHeroRate.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroDefense].Rate;
  seMagicACHeroRateAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroDefense].RateAdd;
  seMagicACHeroValue.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroDefense].Value;
  seMagicACHeroValueAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroDefense].ValueAdd;

  chkDefenceHum.Checked := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumMagDefense].IsChecked;
  seDefenceHumRate.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumMagDefense].Rate;
  seDefenceHumRateAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumMagDefense].RateAdd;
  seDefenceHumValue.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumMagDefense].Value;
  seDefenceHumValueAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHumMagDefense].ValueAdd;

  chkDefenceMon.Checked := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonMagDefense].IsChecked;
  seDefenceMonRate.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonMagDefense].Rate;
  seDefenceMonRateAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonMagDefense].RateAdd;
  seDefenceMonValue.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonMagDefense].Value;
  seDefenceMonValueAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtMonMagDefense].ValueAdd;

  chkDefenceHero.Checked := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroMagDefense].IsChecked;
  seDefenceHeroRate.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroMagDefense].Rate;
  seDefenceHeroRateAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroMagDefense].RateAdd;
  seDefenceHeroValue.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroMagDefense].Value;
  seDefenceHeroValueAdd.Value := FCurrentCustomConfig.ServerConfig.AttackBreakDefense[bdtHeroMagDefense].ValueAdd;

  cbbAttackTargetChange(nil); // 处理直接选中“直线攻击”，无法触发响应事件的问题 Cursor 2023-06-07 11:21:40

  SetConfigChanged(OldChanged);
  FIsConfigChanged := OldIsConfigCanSave;
  if not FIsConfigChanged then
    btnSave.Enabled := False;
end;

procedure TFrmCustomMagic.cbbClientLevelChange(Sender: TObject);
var
  Index: Integer;
  OldChanged, OldIsConfigCanSave: Boolean;
begin
  FCurrentClientConfig := nil;
  if FCurrentCustomConfig = nil then
    Exit;

  OldIsConfigCanSave := FIsConfigChanged;
  Index := cbbClientLevel.ItemIndex;
  if (Index >= Integer(Low(FCurrentCustomConfig.ClientConfigs))) and (Index <= Integer(High(FCurrentCustomConfig.ClientConfigs)))
    then
  begin
    OldChanged := FCurrentCustomConfig.IsChanged;

    FCurrentClientConfig := @FCurrentCustomConfig.ClientConfigs[TMagicPlusLevel(Index)];
    // grpClientAttackConfigs.Caption := AttackConfigNames[Index] + '的攻击效果配置';

    cbbClientIconFile.ItemIndex := FCurrentClientConfig.Icon_File + 1;
    seClientIconIndex.Value := FCurrentClientConfig.Icon_Index;

    edtSound1.Text := FCurrentClientConfig.Sounds[cmstManWarr];
    edtSound2.Text := FCurrentClientConfig.Sounds[cmstWomanWarr];
    edtSound3.Text := FCurrentClientConfig.Sounds[cmstUseMagic];
    edtSound4.Text := FCurrentClientConfig.Sounds[cmstMagicFly];
    edtSound5.Text := FCurrentClientConfig.Sounds[custMagicExplosion];
    edtSound6.Text := FCurrentClientConfig.Sounds[custMagicFail];

    cbbClientFlyFile.ItemIndex := FCurrentClientConfig.Fly_File + 1;
    seClientFlyStartIndex.Value := FCurrentClientConfig.Fly_StartIndex;
    seClientFlyPlayCount.Value := FCurrentClientConfig.Fly_PlayCount;
    seClientFlyEmptyCount.Value := FCurrentClientConfig.Fly_EmptyCount;
    seClientFlyPlayTime.Value := FCurrentClientConfig.Fly_PlayTime;
    cbbClientFlyDrawMode.ItemIndex := Integer(FCurrentClientConfig.Fly_DrawMode);
    cbbClientFlyDirCount.ItemIndex := Integer(FCurrentClientConfig.Fly_DirCount);
    chkClientFlyCalcDir.Checked := FCurrentClientConfig.Fly_CalcDir;
    chkClientFlyFireGunMode.Checked := FCurrentClientConfig.Fly_FireGunMode;
    seClientFlyLightRange.Value := FCurrentClientConfig.Fly_LightRange;

    cbbClientFlyEffFile.ItemIndex := FCurrentClientConfig.FlyEff_File + 1;
    seClientFlyEffStartIndex.Value := FCurrentClientConfig.FlyEff_StartIndex;
    cbbClientFlyEffDrawMode.ItemIndex := Integer(FCurrentClientConfig.FlyEff_DrawMode);

    cbbClientSelfFile.ItemIndex := FCurrentClientConfig.Self_File + 1;
    seClientSelfStartIndex.Value := FCurrentClientConfig.Self_StartIndex;
    chkSelf_SyncHumAction.Checked := FCurrentClientConfig.Self_SyncHumAction;
    seClientSelfPlayCount.Value := FCurrentClientConfig.Self_PlayCount;
    seClientSelfEmptyCount.Value := FCurrentClientConfig.Self_EmptyCount;
    seClientSelfPlayTime.Value := FCurrentClientConfig.Self_PlayTime;
    // cbbClientSelfPlayMode.ItemIndex := Integer(FCurrentClientConfig.Self_PlayMode);
    cbbClientSelfDrawOrder.ItemIndex := Integer(FCurrentClientConfig.Self_DrawOrder);
    cbbClientSelfDrawMode.ItemIndex := Integer(FCurrentClientConfig.Self_DrawMode);
    cbbClientSelfDirCalcType.ItemIndex := Integer(FCurrentClientConfig.Self_DirCalcType);
    cbbClientSelfDirCount.ItemIndex := Integer(FCurrentClientConfig.Self_DirCount);
    chkClientSelfPlayDelayAction.Checked := FCurrentClientConfig.Self_PlayDelayAction;
    seClientSelfLightRange.Value := FCurrentClientConfig.Self_LightRange;
    chkClientSelfPlayFailNoDraw.Checked := FCurrentClientConfig.Self_PlayFailNoDraw;

    cbbClientSelfKeepFile.ItemIndex := FCurrentClientConfig.SelfKeep_File + 1;
    seClientSelfKeepStartIndex.Value := FCurrentClientConfig.SelfKeep_StartIndex;
    seClientSelfKeepPlayCount.Value := FCurrentClientConfig.SelfKeep_PlayCount;
    seClientSelfKeepPlayTime.Value := FCurrentClientConfig.SelfKeep_PlayTime;
    cbbClientSelfKeepDrawMode.ItemIndex := Integer(FCurrentClientConfig.SelfKeep_DrawMode);
    seClientSelfKeepTime.Value := FCurrentClientConfig.SelfKeep_KeepTime;
    seClientSelfKeepTime2.Value := FCurrentClientConfig.SelfKeep_KeepTime2;

    cbbClientFastMoveFile.ItemIndex := FCurrentClientConfig.FastMove_File + 1;
    seClientFastMoveStartIndex.Value := FCurrentClientConfig.FastMove_StartIndex;
    seClientFastMovePlayCount.Value := FCurrentClientConfig.FastMove_PlayCount;
    seClientFastMoveEmptyCount.Value := FCurrentClientConfig.FastMove_EmptyCount;
    seClientFastMovePlayTime.Value := FCurrentClientConfig.FastMove_PlayTime;
    cbbClientFastMoveDrawMode.ItemIndex := Integer(FCurrentClientConfig.FastMove_DrawMode);
    chkClientFastMoveCalcDir.Checked := FCurrentClientConfig.FastMove_CalcDir;
    chkClientFastMoveNoHitAction.Checked := FCurrentClientConfig.FastMove_NoHitAction;
    seFastMoveLightRange.Value := FCurrentClientConfig.FastMove_LightRange;

    cbbClientPreTargetFile.ItemIndex := FCurrentClientConfig.PreTarget_File + 1;
    seClientPreTargetStartIndex.Value := FCurrentClientConfig.PreTarget_StartIndex;
    seClientPreTargetStartIndex2.Value := FCurrentClientConfig.PreTarget_StartIndex2;
    seClientPreTargetPlayCount.Value := FCurrentClientConfig.PreTarget_PlayCount;
    seClientPreTargetEmptyCount.Value := FCurrentClientConfig.PreTarget_EmptyCount;
    seClientPreTargetPlayTime.Value := FCurrentClientConfig.PreTarget_PlayTime;
    cbbClientPreTargetDrawMode.ItemIndex := Integer(FCurrentClientConfig.PreTarget_DrawMode);
    cbbClientPreTargetDrawMode2.ItemIndex := Integer(FCurrentClientConfig.PreTarget_DrawMode2);
    chkClientPreTargetCalcDir.Checked := FCurrentClientConfig.PreTarget_CalcDir;
    chkClientPreTargetLockDraw.Checked := FCurrentClientConfig.PreTarget_LockDraw;
    seClientPreTargetLightRange.Value := FCurrentClientConfig.PreTarget_LightRange;

    cbbClientTargetFile.ItemIndex := FCurrentClientConfig.Target_File + 1;
    seClientTargetStartIndex.Value := FCurrentClientConfig.Target_StartIndex;
    seClientTargetStartIndex2.Value := FCurrentClientConfig.Target_StartIndex2;
    seClientTargetPlayCount.Value := FCurrentClientConfig.Target_PlayCount;
    seClientTargetPlayTime.Value := FCurrentClientConfig.Target_PlayTime;
    cbbClientTargetDrawMode.ItemIndex := Integer(FCurrentClientConfig.Target_DrawMode);
    cbbClientTargetDrawMode2.ItemIndex := Integer(FCurrentClientConfig.Target_DrawMode2);
    chkClientTargetMultiPlay.Checked := FCurrentClientConfig.Target_MultiPlay;
    chkClientTargetLockDraw.Checked := FCurrentClientConfig.Target_LockDraw;
    seClientTargetLightRange.Value := FCurrentClientConfig.Target_LightRange;

    chkClientTargetKeepPlay.Checked := FCurrentClientConfig.Target_KeepPlay;
    seClientTargetKeepTime.Value := FCurrentClientConfig.Target_KeepTime;
    seClientTargetKeepTime2.Value := FCurrentClientConfig.Target_KeepTime2;
    seClientTargetKeepAttackRange.Value := FCurrentClientConfig.Target_KeepAttackRange;
    chkClientTargetKeepMultiPlay.Checked := FCurrentClientConfig.Target_KeepMultiPlay;
    seClientTargetKeepAttackInterval.Value := FCurrentClientConfig.Target_KeepAttackInterval;
    seTargetKeepLightRange.Value := FCurrentClientConfig.Target_KeepLightRange;

    cbbTargetStatus1_File.ItemIndex := FCurrentClientConfig.TargetStatus1_File + 1;
    seTargetStatus1_StartIndex.Value := FCurrentClientConfig.TargetStatus1_StartIndex;
    seTargetStatus1_PlayCount.Value := FCurrentClientConfig.TargetStatus1_PlayCount;
    seTargetStatus1_EmptyCount.Value := FCurrentClientConfig.TargetStatus1_EmptyCount;
    // seTargetStatus1_PlayTime.Value := FCurrentClientConfig.TargetStatus1_PlayTime;
    cbbTargetStatus1_DrawMode.ItemIndex := Integer(FCurrentClientConfig.TargetStatus1_DrawMode);
    chkTargetStatus1_CalcDir.Checked := FCurrentClientConfig.TargetStatus1_CalcDir;

    cbbTargetStatus2_File.ItemIndex := FCurrentClientConfig.TargetStatus2_File + 1;
    seTargetStatus2_StartIndex.Value := FCurrentClientConfig.TargetStatus2_StartIndex;
    seTargetStatus2_PlayCount.Value := FCurrentClientConfig.TargetStatus2_PlayCount;
    seTargetStatus2_EmptyCount.Value := FCurrentClientConfig.TargetStatus2_EmptyCount;
    // seTargetStatus2_PlayTime.Value := FCurrentClientConfig.TargetStatus2_PlayTime;
    cbbTargetStatus2_DrawMode.ItemIndex := Integer(FCurrentClientConfig.TargetStatus2_DrawMode);
    chkTargetStatus2_CalcDir.Checked := FCurrentClientConfig.TargetStatus2_CalcDir;

    SetConfigChanged(OldChanged);

    FIsConfigChanged := OldIsConfigCanSave;
    if not FIsConfigChanged then
      btnSave.Enabled := False;
  end;
end;

procedure TFrmCustomMagic.chkClientLockClick(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicLock := chkClientLock.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientActionTypeChange(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicActionType := TMagicActionType(cbbClientActionType.ItemIndex);

    seClientActionStartIndex.Enabled := FCurrentCustomConfig.ClientBaseConfig.MagicActionType = matCustom;
    seClientActionPlayCount.Enabled := seClientActionStartIndex.Enabled;
    seClientActionEmptyCount.Enabled := seClientActionStartIndex.Enabled;
    chkClientActionContinue.Enabled := seClientActionStartIndex.Enabled { and (not chkMagicSwitchMode.Enabled) };
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientLockSelfClick(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicLockSelf := chkClientLockSelf.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientNotRaiseHandClick(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.NotRaiseHand := chkClientNotRaiseHand.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientIconFileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Icon_File := cbbClientIconFile.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientIconIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Icon_Index := seClientIconIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientFlyFileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_File := cbbClientFlyFile.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFlyStartIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_StartIndex := seClientFlyStartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFlyPlayCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_PlayCount := seClientFlyPlayCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFlyEmptyCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_EmptyCount := seClientFlyEmptyCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFlyPlayTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_PlayTime := seClientFlyPlayTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientFlyDrawModeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_DrawMode := TCustomDrawMode(cbbClientFlyDrawMode.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientFlyDirCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_DirCount := TCustomDirCount(cbbClientFlyDirCount.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientFlyCalcDirClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_CalcDir := chkClientFlyCalcDir.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientFlyFireGunModeClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_FireGunMode := chkClientFlyFireGunMode.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFlyLightRangeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Fly_LightRange := seClientFlyLightRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientFlyEffFileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FlyEff_File := cbbClientFlyEffFile.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFlyEffStartIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FlyEff_StartIndex := seClientFlyEffStartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientFlyEffDrawModeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FlyEff_DrawMode := TCustomDrawMode(cbbClientFlyEffDrawMode.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientSelfFileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_File := cbbClientSelfFile.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfStartIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_StartIndex := seClientSelfStartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfPlayCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_PlayCount := seClientSelfPlayCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfEmptyCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_EmptyCount := seClientSelfEmptyCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfPlayTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_PlayTime := seClientSelfPlayTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientSelfDrawOrderChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_DrawOrder := TCustomDrawOrder(cbbClientSelfDrawOrder.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientSelfDrawModeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_DrawMode := TCustomDrawMode(cbbClientSelfDrawMode.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientSelfDirCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_DirCount := TCustomDirCount(cbbClientSelfDirCount.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientSelfDirCalcTypeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_DirCalcType := TCustomDirCalcType(cbbClientSelfDirCalcType.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientSelfPlayDelayActionClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_PlayDelayAction := chkClientSelfPlayDelayAction.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientSelfPlayFailNoDrawClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_PlayFailNoDraw := chkClientSelfPlayFailNoDraw.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfLightRangeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_LightRange := seClientSelfLightRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientPreTargetFileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_File := cbbClientPreTargetFile.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientPreTargetStartIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_StartIndex := seClientPreTargetStartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientPreTargetStartIndex2Change(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_StartIndex2 := seClientPreTargetStartIndex2.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientPreTargetPlayCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_PlayCount := seClientPreTargetPlayCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientPreTargetEmptyCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_EmptyCount := seClientPreTargetEmptyCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientPreTargetCalcDirClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_CalcDir := chkClientPreTargetCalcDir.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientPreTargetPlayTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_PlayTime := seClientPreTargetPlayTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientPreTargetDrawModeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_DrawMode := TCustomDrawMode(cbbClientPreTargetDrawMode.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientPreTargetDrawMode2Change(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_DrawMode2 := TCustomDrawMode(cbbClientPreTargetDrawMode2.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientPreTargetLockDrawClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_LockDraw := chkClientPreTargetLockDraw.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientPreTargetLightRangeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.PreTarget_LightRange := seClientPreTargetLightRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientTargetFileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_File := cbbClientTargetFile.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientTargetStartIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_StartIndex := seClientTargetStartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientTargetStartIndex2Change(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_StartIndex2 := seClientTargetStartIndex2.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientTargetPlayCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_PlayCount := seClientTargetPlayCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientTargetPlayTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_PlayTime := seClientTargetPlayTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientTargetDrawModeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_DrawMode := TCustomDrawMode(cbbClientTargetDrawMode.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientTargetDrawMode2Change(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_DrawMode2 := TCustomDrawMode(cbbClientTargetDrawMode2.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientTargetMultiPlayClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_MultiPlay := chkClientTargetMultiPlay.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientTargetLockDrawClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_LockDraw := chkClientTargetLockDraw.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientTargetLightRangeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_LightRange := seClientTargetLightRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientTargetKeepPlayClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_KeepPlay := chkClientTargetKeepPlay.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientTargetKeepTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_KeepTime := seClientTargetKeepTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientTargetKeepTime2Change(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_KeepTime2 := seClientTargetKeepTime2.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientTargetKeepAttackIntervalChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_KeepAttackInterval := seClientTargetKeepAttackInterval.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientTargetKeepAttackRangeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_KeepAttackRange := seClientTargetKeepAttackRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seTargetKeepLightRangeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_KeepLightRange := seTargetKeepLightRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientTargetKeepMultiPlayClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Target_KeepMultiPlay := chkClientTargetKeepMultiPlay.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbTargetStatus1_FileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus1_File := cbbTargetStatus1_File.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seTargetStatus1_StartIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus1_StartIndex := seTargetStatus1_StartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seTargetStatus1_PlayCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus1_PlayCount := seTargetStatus1_PlayCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seTargetStatus1_EmptyCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus1_EmptyCount := seTargetStatus1_EmptyCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seTargetStatus1_PlayTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    // FCurrentClientConfig.TargetStatus1_PlayTime := seTargetStatus1_PlayTime.Value;
    // SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbTargetStatus1_DrawModeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus1_DrawMode := TCustomDrawMode(cbbTargetStatus1_DrawMode.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkTargetStatus1_CalcDirClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus1_CalcDir := chkTargetStatus1_CalcDir.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbTargetStatus2_FileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus2_File := cbbTargetStatus2_File.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seTargetStatus2_StartIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus2_StartIndex := seTargetStatus2_StartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seTargetStatus2_PlayCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus2_PlayCount := seTargetStatus2_PlayCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seTargetStatus2_EmptyCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus2_EmptyCount := seTargetStatus2_EmptyCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seTargetStatus2_PlayTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    // FCurrentClientConfig.TargetStatus2_PlayTime := seTargetStatus2_PlayTime.Value;
    // SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbTargetStatus2_DrawModeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus2_DrawMode := TCustomDrawMode(cbbTargetStatus2_DrawMode.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkTargetStatus2_CalcDirClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.TargetStatus2_CalcDir := chkTargetStatus2_CalcDir.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbOperateModeChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.OperateMode := TCustomOperateMode(cbbOperateMode.ItemIndex);
    SetConfigChanged();

    lblAttackDelay.Visible := FCurrentServerConfig.OperateMode = momAttack;
    seAttackDelayTime.Visible := FCurrentServerConfig.OperateMode = momAttack;
    lblAttackDelayTime.Visible := FCurrentServerConfig.OperateMode = momAttack;

    lblProtectTargetRangeTitle.Visible := FCurrentServerConfig.OperateMode <> momAttack;
    seProtectTargetRange.Visible := FCurrentServerConfig.OperateMode <> momAttack;
    lblProtectTargetRangeValue.Visible := FCurrentServerConfig.OperateMode <> momAttack;

    if FCurrentServerConfig.OperateMode = momAttack then
    begin
      pgcMagicType.ActivePage := tsMagicAttack;
    end
    else
      pgcMagicType.ActivePage := tsMagicProtected;
  end;
end;

procedure TFrmCustomMagic.seUseIntervalChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.UseInterval := seUseInterval.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkFailNoShowEffClick(Sender: TObject);
begin
  {
    if FCurrentServerConfig <> nil then
    begin
    FCurrentServerConfig.FailNoShowEff := chkFailNoShowEff.Checked;
    SetConfigChanged();
    end;
  }
end;

procedure TFrmCustomMagic.edtFailMsgChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.FailMsg := edtFailMsg.Text;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.edtSucceedMsgChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.SucceedMsg := edtSucceedMsg.Text;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.edtCloseMsgChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.CloseMsg := edtCloseMsg.Text;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbAttackTargetChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTarget := TCustomAttackTarget(cbbAttackTarget.ItemIndex);
    SetConfigChanged();

    lblLineAttackAddPower.Visible := FCurrentServerConfig.AttackTarget in [matLine, matDir8, matDir16];
    seLineAttackAddPower.Visible := lblLineAttackAddPower.Visible;
    lblLineAttackAddPowerPerc.Visible := lblLineAttackAddPower.Visible;

    seAttackLineWidth.Visible := cbbAttackTarget.ItemIndex = 2;
    lblAttackWidth.Visible := seAttackLineWidth.Visible;
    lblH_AttackWidth.Visible := seAttackLineWidth.Visible;
  end;
end;

procedure TFrmCustomMagic.seAttackNearRangeChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackNearRange := seAttackNearRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAttackGroupRangeChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackGroupRange := seAttackGroupRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAttackLineWidthChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentCustomConfig.ServerConfig.AttackLineWidth := seAttackLineWidth.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbAttackPowerCalcChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackPowerCalc := TCustomAttackPowerCalc(cbbAttackPowerCalc.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbAttackPowerLevelChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    seAttackPowerRate.Value := FCurrentServerConfig.AttackPowerRates[cbbAttackPowerLevel.ItemIndex];
  end;
end;

procedure TFrmCustomMagic.seAttackPowerRateChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackPowerRates[cbbAttackPowerLevel.ItemIndex] := seAttackPowerRate.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seLineAttackAddPowerChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackPowerLineAdd := seLineAttackAddPower.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAttackPowerUndeadAddChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackPowerUndeadAdd := seAttackPowerUndeadAdd.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbNeedItemChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.NeedItem := TMagicNeedItem(cbbNeedItem.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seNeedItemCountChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.NeedItemCount := seNeedItemCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.edtNeedItemCustomItemNameChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.NeedItemCustomItemName := edtNeedItemCustomItemName.Text;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkNeedItemUseBagItemClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.NeedItemUseBagItem := chkNeedItemUseBagItem.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seProtectTargetRangeChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.ProtectTargetRange := seProtectTargetRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAdditional0Click(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TCheckBox) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentServerConfig.Additionals)) and (WinCtrl.Tag <= High(FCurrentServerConfig.Additionals)) then
      FCurrentServerConfig.Additionals[WinCtrl.Tag].Checked := TCheckBox(Sender).Checked;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAdditionalRate0Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TSpinEditEx) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentServerConfig.Additionals)) and (WinCtrl.Tag <= High(FCurrentServerConfig.Additionals)) then
      FCurrentServerConfig.Additionals[WinCtrl.Tag].Rate := TSpinEditEx(Sender).Value;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAdditionalRate0_2Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TSpinEditEx) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentServerConfig.Additionals)) and (WinCtrl.Tag <= High(FCurrentServerConfig.Additionals)) then
      FCurrentServerConfig.Additionals[WinCtrl.Tag].Rate2 := TSpinEditEx(Sender).Value;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAdditionalTime0Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TSpinEditEx) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentServerConfig.Additionals)) and (WinCtrl.Tag <= High(FCurrentServerConfig.Additionals)) then
      FCurrentServerConfig.Additionals[WinCtrl.Tag].Time := TSpinEditEx(Sender).Value;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbAdditionalTime0_1Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TComboBox) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentServerConfig.Additionals)) and (WinCtrl.Tag <= High(FCurrentServerConfig.Additionals)) then
      FCurrentServerConfig.Additionals[WinCtrl.Tag].TimeUnit := TComboBox(Sender).ItemIndex;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAdditionalTime0_2Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TSpinEditEx) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentServerConfig.Additionals)) and (WinCtrl.Tag <= High(FCurrentServerConfig.Additionals)) then
      FCurrentServerConfig.Additionals[WinCtrl.Tag].Time2 := TSpinEditEx(Sender).Value;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAdditionaHP0Change(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AdditionalHP0 := seAdditionaHP0.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkseAdditionaHighLevel4Click(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AdditionalHighLevel4 := chkseAdditionaHighLevel4.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbPushedType4Change(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AdditionalPushedType4 := cbbPushedType4.ItemIndex;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTargetStatusClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTargetStatus := chkAttackTargetStatus.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAttackTargetStatusTimeChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTargetStatusTime := seAttackTargetStatusTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbAttackTargetStatusTime_1Change(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTargetStatusTimeUnit := cbbAttackTargetStatusTime_1.ItemIndex;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAttackTargetStatusTime_2Change(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTargetStatusTime2 := seAttackTargetStatusTime_2.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAttackTargetStatusDelayChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTargetStatusDelay := seAttackTargetStatusDelay.Value;
    SetConfigChanged();
  end;
end;

procedure RebuildCustomMagicListText;
var
  I: Integer;
  CustomMagicConfig: TCustomMagicConfig;
  InBuf: PAnsiChar;
  InBytes: Integer;
  ClientConfig: PClientCustomMagicConfig;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_CustomMagicList.LockR(4);
  try
{$IFEND}
    InBytes := UserEngine.m_CustomMagicList.Count * SizeOf(TClientCustomMagicConfig);
    GetMem(InBuf, InBytes + 1);
    try
      ClientConfig := PClientCustomMagicConfig(InBuf);

      for I := 0 to UserEngine.m_CustomMagicList.Count - 1 do
      begin
        CustomMagicConfig := UserEngine.m_CustomMagicList.Items[I];

        ClientConfig^.wMagicID := CustomMagicConfig.MagicID;
        ClientConfig^.MagicBaseConfig := CustomMagicConfig.ClientBaseConfig;
        ClientConfig^.MagicConfigs := CustomMagicConfig.ClientConfigs;

        ClientConfig^.boIsMagicWarr := CustomMagicConfig.IsMagicWarr;

        if CustomMagicConfig.IsMagicWarr then
          ClientConfig^.btNearAttackRange := CustomMagicConfig.ServerConfig.AttackNearRange
        else
          ClientConfig^.btNearAttackRange := 1;

        ClientConfig^.IsAttackUseNG := CustomMagicConfig.ServerConfig.IsAttackUseNG;
        ClientConfig^.NoChangeDir := CustomMagicConfig.ServerConfig.NoChangeDir;
        // ClientConfig^.FailMsg := CustomMagicConfig.ServerConfig.FailMsg;

        if not CustomMagicConfig.ServerConfig.IsCheckVarValue then
        begin
          ClientConfig.NeedItem := CustomMagicConfig.ServerConfig.NeedItem;
          ClientConfig.NeedItemCount := CustomMagicConfig.ServerConfig.NeedItemCount;
          ClientConfig.NeedItemCustomItemName := CustomMagicConfig.ServerConfig.NeedItemCustomItemName;
          ClientConfig.NeedItemUseBagItem := CustomMagicConfig.ServerConfig.NeedItemUseBagItem;
        end
        else
        begin
          ClientConfig.NeedItem := meiNone;
          ClientConfig.NeedItemCount := 0;
          ClientConfig.NeedItemCustomItemName := '';
          ClientConfig.NeedItemUseBagItem := False;
        end;

        Inc(ClientConfig);
      end;

      g_CustomMagicListTextLen := InBytes;
      g_CustomMagicListText := zLibCompressBuffer(InBuf, InBytes);
      g_CustomMagicListTextCRC := BufferCrc(PAnsiChar(g_CustomMagicListText), Length(g_CustomMagicListText));
    finally
      FreeMem(InBuf, InBytes + 1);
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomMagicList.UnLockR;
  end;
{$IFEND}
end;

procedure TFrmCustomMagic.btnSaveClick(Sender: TObject);
var
  Node: PVirtualNode;
  ConfigNodeData: PMagicConfigNodeData;
begin
  vstAttackDecAttr.EndEditNode;

  Node := vstCustomMagic.GetFirst();
  while Node <> nil do
  begin
    ConfigNodeData := vstCustomMagic.GetNodeData(Node);
    if (ConfigNodeData <> nil) and ConfigNodeData.Config.IsChanged then
      ConfigNodeData.Config.SaveToIniFile;

    Node := vstCustomMagic.GetNext(Node);
  end;
  vstCustomMagic.Invalidate;

  RebuildCustomMagicListText;
  ResetMagicCDList;

  FIsConfigChanged := False;
  btnSave.Enabled := False;

  UserEngine.SendServerConfig();
end;

procedure TFrmCustomMagic.btnMakeConfigDataClick(Sender: TObject);
var
  FileName: string;
begin
  if g_Config.sCustomMagicClientConfigFileName <> '' then
    dlgSaveMagics.FileName := g_Config.sCustomMagicClientConfigFileName;

  if not dlgSaveMagics.Execute then
  begin
    // 指定保存目录会改变当前程序目录,
    SetCurrentDirectory(PChar(ExtractFileDir(Application.ExeName)));
    Exit;
  end;

  // 指定保存目录会改变当前程序目录,
  SetCurrentDirectory(PChar(ExtractFileDir(Application.ExeName)));

  FileName := dlgSaveMagics.FileName;
  FileName := ChangeFileExt(FileName, '.dat');

  g_Config.sCustomMagicClientConfigFileName := FileName;
  Config.WriteString('Setup', 'CustomMagicClientConfigFileName', g_Config.sCustomMagicClientConfigFileName);

{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_CustomMagicList.LockR(5);
  try
{$IFEND}
    SaveCustomMagicClientConfigs(UserEngine.m_CustomMagicList, FileName);
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomMagicList.UnLockR;
  end;
{$IFEND}
  Showmessage('已经生成自定义技能登录器配置文件');
end;

procedure TFrmCustomMagic.chkSendCustomMagicConfigClick(Sender: TObject);
begin
  g_Config.boSendCustomMagicConfig := chkSendCustomMagicConfig.Checked;
  Config.WriteBool('Setup', 'SendCustomMagicConfig', g_Config.boSendCustomMagicConfig);
end;

procedure TFrmCustomMagic.edtCallMonster1Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TEdit) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentServerConfig.CallMonsters)) and (WinCtrl.Tag <= High(FCurrentServerConfig.CallMonsters)) then
      FCurrentServerConfig.CallMonsters[WinCtrl.Tag] := TEdit(Sender).Text;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seCallMonsterNum1Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TSpinEditEx) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentServerConfig.CallMonsterNums)) and (WinCtrl.Tag <= High(FCurrentServerConfig.CallMonsterNums))
      then
      FCurrentServerConfig.CallMonsterNums[WinCtrl.Tag] := TSpinEditEx(Sender).Value;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seCallMonstersRateChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.CallMonstersRate := seCallMonstersRate.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seCallMonstersLevelChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.CallMonstersLevel := seCallMonstersLevel.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportAfterDamageClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportAfterDamage := chkAttackTeleportAfterDamage.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportAttackClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportAttack := chkAttackTeleportAttack.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAttackTeleportRateChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportRate := seAttackTeleportRate.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAttackTeleportRushCountChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportRushCount := seAttackTeleportRushCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportRunHumClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportRunHum := chkAttackTeleportRunHum.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportRunMonClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportRunMon := chkAttackTeleportRunMon.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportRunNpcClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportRunNpc := chkAttackTeleportRunNpc.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportRunGuardClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportRunGuard := chkAttackTeleportRunGuard.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportRunObstacleClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportRunObstacle := chkAttackTeleportRunObstacle.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportRushClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportRush := chkAttackTeleportRush.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportWarDisHumRunClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportWarDisHumRun := chkAttackTeleportWarDisHumRun.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackTeleportCannotRunItemClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackTeleportCannotRunItem := chkAttackTeleportCannotRunItem.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkEnabledCallMonsterClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.EnabledCallMonster := chkEnabledCallMonster.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientFastMoveFileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FastMove_File := cbbClientFastMoveFile.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFastMoveStartIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FastMove_StartIndex := seClientFastMoveStartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFastMovePlayCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FastMove_PlayCount := seClientFastMovePlayCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFastMoveEmptyCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FastMove_EmptyCount := seClientFastMoveEmptyCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientFastMovePlayTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FastMove_PlayTime := seClientFastMovePlayTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientFastMoveDrawModeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FastMove_DrawMode := TCustomDrawMode(cbbClientFastMoveDrawMode.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientFastMoveCalcDirClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FastMove_CalcDir := chkClientFastMoveCalcDir.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientFastMoveNoHitActionClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FastMove_NoHitAction := chkClientFastMoveNoHitAction.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seFastMoveLightRangeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.FastMove_LightRange := seFastMoveLightRange.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.edtSound1Change(Sender: TObject);
var
  Edit: TEdit;
begin
  if FCurrentClientConfig = nil then
    Exit;

  if Sender is TEdit then
  begin
    Edit := Sender as TEdit;
    if (Edit.Tag >= Integer(Low(TMagicSoundType))) and (Edit.Tag <= Integer(High(TMagicSoundType))) then
    begin
      FCurrentClientConfig.Sounds[TMagicSoundType(Edit.Tag)] := Edit.Text;
      SetConfigChanged();
    end;
  end;
end;

procedure TFrmCustomMagic.cbbClientSelfKeepFileChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.SelfKeep_File := cbbClientSelfKeepFile.ItemIndex - 1;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfKeepStartIndexChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.SelfKeep_StartIndex := seClientSelfKeepStartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfKeepPlayCountChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.SelfKeep_PlayCount := seClientSelfKeepPlayCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfKeepPlayTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.SelfKeep_PlayTime := seClientSelfKeepPlayTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbClientSelfKeepDrawModeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.SelfKeep_DrawMode := TCustomDrawMode(cbbClientSelfKeepDrawMode.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfKeepTimeChange(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.SelfKeep_KeepTime := seClientSelfKeepTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientSelfKeepTime2Change(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.SelfKeep_KeepTime2 := seClientSelfKeepTime2.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seCallMonstersRoyaltySecChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.CallMonstersRoyaltySec := seCallMonstersRoyaltySec.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbMagicSwitchModeChange(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode := TMagicSwitchMode(cbbMagicSwitchMode.ItemIndex);
    SetConfigChanged();

    chkSwitchModeNoClose.Enabled := FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode = msmSwitch;
    chkMagicAutoOpen.Enabled := FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode = msmSwitch;
  end;
end;

procedure TFrmCustomMagic.chkSwitchModeNoCloseClick(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.SwitchModeNoClose := chkSwitchModeNoClose.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkMagicAutoOpenClick(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicAutoOpen := chkMagicAutoOpen.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbMagicWarrNGOptionChange(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicWarrNGOption := TMagicWarrNGOption(cbbMagicWarrNGOption.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.btnCopyConfigClick(Sender: TObject);
var
  DestLevel: TMagicPlusLevel;
begin
  if (FCurrentCustomConfig <> nil) and ShowCustomMagicCopySetting(TMagicPlusLevel(cbbClientLevel.ItemIndex), DestLevel) then
  begin
    FCurrentCustomConfig.ClientConfigs[DestLevel] := FCurrentCustomConfig.ClientConfigs[TMagicPlusLevel(cbbClientLevel.ItemIndex)];
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientActionStartIndexChange(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicActionStartIndex := seClientActionStartIndex.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientActionPlayCountChange(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicActionPlayCount := seClientActionPlayCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seClientActionEmptyCountChange(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicActionEmptyCount := seClientActionEmptyCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkClientActionContinueClick(Sender: TObject);
begin
  if FCurrentCustomConfig <> nil then
  begin
    FCurrentCustomConfig.ClientBaseConfig.MagicActionContinue := chkClientActionContinue.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkEnableAntiMagicClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.EnableAntiMagic := chkEnableAntiMagic.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkProtectAddHPSlowClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.ProtectAddHPSlow := chkProtectAddHPSlow.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seProtectAddHPSlowCountChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.ProtectAddHpSlowCount := seProtectAddHPSlowCount.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seAttackDelayTimeChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.AttackDelayTime := seAttackDelayTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkEnableHitPointClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.EnableHitPoint := chkEnableHitPoint.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkProtectTargetStatusClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.ProtectTargetStatus := chkProtectTargetStatus.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seProtectTargetStatusTimeChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.ProtectTargetStatusTime := seProtectTargetStatusTime.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbProtectTargetStatusTime_1Change(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.ProtectTargetStatusTimeUnit := cbbProtectTargetStatusTime_1.ItemIndex;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seProtectTargetStatusTime_2Change(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.ProtectTargetStatusTime2 := seProtectTargetStatusTime_2.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seProtectTargetStatusTimeDelayChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.ProtectTargetStatusDelay := seProtectTargetStatusTimeDelay.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkCheckVarValueClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.IsCheckVarValue := chkCheckVarValue.Checked;
    SetConfigChanged();

    lblNeedItem.Visible := not chkCheckVarValue.Checked;
    cbbNeedItem.Visible := lblNeedItem.Visible;
    lblNeedItemCount.Visible := lblNeedItem.Visible;
    seNeedItemCount.Visible := lblNeedItem.Visible;
    lblNeedItemCustomItemName.Visible := lblNeedItem.Visible;
    edtNeedItemCustomItemName.Visible := lblNeedItem.Visible;
    chkNeedItemUseBagItem.Visible := lblNeedItem.Visible;

    lblCheckVarName.Visible := chkCheckVarValue.Checked;
    edtCheckVarName.Visible := lblCheckVarName.Visible;
    lblCheckVarType.Visible := lblCheckVarName.Visible;
    cbbCheckVarType.Visible := lblCheckVarName.Visible;
    lblCheckVarValue.Visible := lblCheckVarName.Visible;
    seCheckVarValue.Visible := lblCheckVarName.Visible;
    lblCheckVarAdd.Visible := lblCheckVarName.Visible;
    seCheckVarAdd.Visible := lblCheckVarName.Visible;
  end;
end;

procedure TFrmCustomMagic.edtCheckVarNameChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.CheckVarName := edtCheckVarName.Text;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.cbbCheckVarTypeChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.CheckVarType := TCheckVarType(cbbCheckVarType.ItemIndex);
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seCheckVarValueChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.CheckVarValue := seCheckVarValue.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seCheckVarAddChange(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.CheckVarAdd := seCheckVarAdd.Value;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkNoTeleportNoAttackClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.IsNoTeleportNoAttack := chkNoTeleportNoAttack.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkSelf_SyncHumActionClick(Sender: TObject);
begin
  if FCurrentClientConfig <> nil then
  begin
    FCurrentClientConfig.Self_SyncHumAction := chkSelf_SyncHumAction.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackUseNGClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.IsAttackUseNG := chkAttackUseNG.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkAttackNoChangeDirClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.NoChangeDir := chkAttackNoChangeDir.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.chkDisableInSafeZoneClick(Sender: TObject);
begin
  if FCurrentServerConfig <> nil then
  begin
    FCurrentServerConfig.DisableInSafeZone := chkDisableInSafeZone.Checked;
    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.vstAttackDecAttrChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  DecAttribData: PAttackDecAttribData;
begin
  DecAttribData := Sender.GetNodeData(Node);
  if DecAttribData <> nil then
  begin
    if Sender.CheckState[Node] = csCheckedNormal then
      DecAttribData.Data.IsChecked := True
    else
      DecAttribData.Data.IsChecked := False;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.vstAttackDecAttrNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  DecAttribData: PAttackDecAttribData;
begin
  if HitInfo.HitNode = nil then
    Exit;

  DecAttribData := Sender.GetNodeData(HitInfo.HitNode);
  if HitInfo.HitColumn = 12 then
  begin
    DecAttribData.Data.ShowHint := not DecAttribData.Data.ShowHint;
    Sender.InvalidateNode(HitInfo.HitNode);
    SetConfigChanged();
  end
  else if (HitInfo.HitColumn > 0) then
  begin
    PostMessage(Self.Handle, WM_STARTEDITING_DEC_ATTRIB, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
  end;
end;

procedure TFrmCustomMagic.vstAttackDecAttrAfterCellPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; CellRect: TRect);
var
  Pt: TPoint;
  ImageIndex: Integer;
  DecAttribData: PAttackDecAttribData;
begin
  DecAttribData := Sender.GetNodeData(Node);

  if Column = 12 then
  begin
    TargetCanvas.Font.Color := Sender.Font.Color;
    TargetCanvas.Brush.Style := bsClear;

    ImageIndex := Integer(DecAttribData.Data.ShowHint);
    Pt.X := CellRect.Left + (CellRect.Right - CellRect.Left - ilCheck.Width) div 2;
    Pt.Y := CellRect.Top + (CellRect.Bottom - CellRect.Top - ilCheck.Height) div 2;
    ilCheck.Draw(TargetCanvas, Pt.X, Pt.Y, ImageIndex);
  end;
end;

procedure TFrmCustomMagic.vstAttackDecAttrGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType:
  TVSTTextType; var CellText: string);
var
  DecAttribData: PAttackDecAttribData;
begin
  DecAttribData := Sender.GetNodeData(Node);
  case Column of
    0:
      CellText := MagicAttackDecAttributesTypeNames[DecAttribData.AttribType];
    1:
      CellText := IntToStr(DecAttribData.Data.Rate);
    2:
      CellText := IntToStr(DecAttribData.Data.RateAdd);
    3:
      begin
        if DecAttribData.AttribType >= daHitPoint then
          CellText := '-'
        else
          CellText := IntToStr(DecAttribData.Data.LowValue);
      end;
    4:
      begin
        if DecAttribData.AttribType >= daHitPoint then
          CellText := '-'
        else
          CellText := MagicAttackDecValueTypeNames[DecAttribData.Data.LowValueIsPoint];
      end;
    5:
      begin
        if DecAttribData.AttribType >= daHitPoint then
          CellText := '-'
        else
          CellText := IntToStr(DecAttribData.Data.LowValueAdd);
      end;
    6:
      CellText := IntToStr(DecAttribData.Data.HighValue);
    7:
      CellText := MagicAttackDecValueTypeNames[DecAttribData.Data.HighValueIsPoint];
    8:
      CellText := IntToStr(DecAttribData.Data.HighValueAdd);
    9:
      CellText := IntToStr(DecAttribData.Data.Time);
    10:
      CellText := IntToStr(DecAttribData.Data.TimeAdd);
    11:
      CellText := MagicAttackDecTimeTypeNames[DecAttribData.Data.TimeAddIsPoint];
    12:
      CellText := ' ';
    13:
      CellText := DecAttribData.Data.HintText;
  end;
end;

procedure TFrmCustomMagic.vstAttackDecAttrEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed:
  Boolean);
var
  DecAttribData: PAttackDecAttribData;
begin
  Allowed := (Node <> nil) and (Column > 0) and (Column <> 12);
  if Allowed then
  begin
    DecAttribData := Sender.GetNodeData(Node);
    if (Column in [3, 4, 5]) and (DecAttribData.AttribType >= daHitPoint) then
      Allowed := False;
  end;
end;

procedure TFrmCustomMagic.vstAttackDecAttrCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; out
  EditLink: IVTEditLink);
begin
  EditLink := TDecAttribPropertyEditLink.Create;
end;

procedure TFrmCustomMagic.WMStartEditingDecAttrib(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WPARAM);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstAttackDecAttr.EditNode(Node, Message.LParam);
end;

procedure TFrmCustomMagic.vstDecElementChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  MagicElementData: PMagicElementData;
begin
  MagicElementData := Sender.GetNodeData(Node);
  if MagicElementData <> nil then
  begin
    if Sender.CheckState[Node] = csCheckedNormal then
      MagicElementData.Data.IsChecked := True
    else
      MagicElementData.Data.IsChecked := False;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.vstDecElementNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  MagicElementData: PMagicElementData;
begin
  if HitInfo.HitNode = nil then
    Exit;

  MagicElementData := Sender.GetNodeData(HitInfo.HitNode);
  if HitInfo.HitColumn = 9 then
  begin
    MagicElementData.Data.ShowHint := not MagicElementData.Data.ShowHint;
    Sender.InvalidateNode(HitInfo.HitNode);
    SetConfigChanged();
  end
  else if (HitInfo.HitColumn > 0) then
  begin
    if Sender = vstDecElement then
      PostMessage(Self.Handle, WM_STARTEDITING_DEC_ELEMENT, WPARAM(HitInfo.HitNode), HitInfo.HitColumn)
    else
      PostMessage(Self.Handle, WM_STARTEDITING_INC_ELEMENT, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
  end;
end;

procedure TFrmCustomMagic.vstDecElementAfterCellPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column:
  TColumnIndex; CellRect: TRect);
var
  Pt: TPoint;
  ImageIndex: Integer;
  MagicElementData: PMagicElementData;
begin
  MagicElementData := Sender.GetNodeData(Node);

  if Column = 9 then
  begin
    TargetCanvas.Font.Color := Sender.Font.Color;
    TargetCanvas.Brush.Style := bsClear;

    ImageIndex := Integer(MagicElementData.Data.ShowHint);
    Pt.X := CellRect.Left + (CellRect.Right - CellRect.Left - ilCheck.Width) div 2;
    Pt.Y := CellRect.Top + (CellRect.Bottom - CellRect.Top - ilCheck.Height) div 2;
    ilCheck.Draw(TargetCanvas, Pt.X, Pt.Y, ImageIndex);
  end;
end;

procedure TFrmCustomMagic.vstDecElementGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType:
  TVSTTextType; var CellText: string);
var
  MagicElementData: PMagicElementData;
begin
  MagicElementData := Sender.GetNodeData(Node);
  case Column of
    0:
      CellText := ItemElementsTypeNames[MagicElementData.ElementType];
    1:
      CellText := IntToStr(MagicElementData.Data.Rate);
    2:
      CellText := IntToStr(MagicElementData.Data.RateAdd);
    3:
      CellText := IntToStr(MagicElementData.Data.Value);
    4:
      CellText := MagicAttackDecValueTypeNames[MagicElementData.Data.ValueIsPoint];
    5:
      CellText := IntToStr(MagicElementData.Data.ValueAdd);
    6:
      CellText := IntToStr(MagicElementData.Data.Time);
    7:
      CellText := IntToStr(MagicElementData.Data.TimeAdd);
    8:
      CellText := MagicAttackDecTimeTypeNames[MagicElementData.Data.TimeAddIsPoint];
    9:
      CellText := ' ';
    10:
      CellText := MagicElementData.Data.HintText;
  end;
end;

procedure TFrmCustomMagic.vstDecElementEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed:
  Boolean);
begin
  Allowed := (Node <> nil) and (Column > 0) and (Column <> 9);
end;

procedure TFrmCustomMagic.vstDecElementCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; out
  EditLink: IVTEditLink);
begin
  EditLink := TElementPropertyEditLink.Create;
end;

procedure TFrmCustomMagic.WMStartEditingDecElement(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WPARAM);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstDecElement.EditNode(Node, Message.LParam);
end;

procedure TFrmCustomMagic.WMStartEditingIncElement(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WPARAM);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstAddElement.EditNode(Node, Message.LParam);
end;

procedure TFrmCustomMagic.vstProtectedAddAttrChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  IncAttribData: PProtectAddAttribData;
begin
  IncAttribData := Sender.GetNodeData(Node);
  if IncAttribData <> nil then
  begin
    if Sender.CheckState[Node] = csCheckedNormal then
      IncAttribData.Data.IsChecked := True
    else
      IncAttribData.Data.IsChecked := False;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.vstProtectedAddAttrNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  IncAttribData: PProtectAddAttribData;
begin
  if HitInfo.HitNode = nil then
    Exit;

  IncAttribData := Sender.GetNodeData(HitInfo.HitNode);
  if HitInfo.HitColumn = 12 then
  begin
    IncAttribData.Data.ShowHint := not IncAttribData.Data.ShowHint;
    Sender.InvalidateNode(HitInfo.HitNode);
    SetConfigChanged();
  end
  else if (HitInfo.HitColumn > 0) then
  begin
    PostMessage(Self.Handle, WM_STARTEDITING_INC_ATTRIB, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
  end;
end;

procedure TFrmCustomMagic.vstProtectedAddAttrAfterCellPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; CellRect: TRect);
var
  Pt: TPoint;
  ImageIndex: Integer;
  IncAttribData: PProtectAddAttribData;
begin
  IncAttribData := Sender.GetNodeData(Node);

  if Column = 12 then
  begin
    TargetCanvas.Font.Color := Sender.Font.Color;
    TargetCanvas.Brush.Style := bsClear;

    ImageIndex := Integer(IncAttribData.Data.ShowHint);
    Pt.X := CellRect.Left + (CellRect.Right - CellRect.Left - ilCheck.Width) div 2;
    Pt.Y := CellRect.Top + (CellRect.Bottom - CellRect.Top - ilCheck.Height) div 2;
    ilCheck.Draw(TargetCanvas, Pt.X, Pt.Y, ImageIndex);
  end;
end;

procedure TFrmCustomMagic.vstProtectedAddAttrGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType:
  TVSTTextType; var CellText: string);
var
  IncAttribData: PProtectAddAttribData;
begin
  IncAttribData := Sender.GetNodeData(Node);
  case Column of
    0:
      CellText := MagicProtectAddAttributesTypeNames[IncAttribData.AttribType];
    1:
      CellText := IntToStr(IncAttribData.Data.Rate);
    2:
      CellText := IntToStr(IncAttribData.Data.RateAdd);
    3:
      begin
        if IncAttribData.AttribType >= aaHitPoint then
          CellText := '-'
        else
          CellText := IntToStr(IncAttribData.Data.LowValue);
      end;
    4:
      begin
        if IncAttribData.AttribType >= aaHitPoint then
          CellText := '-'
        else
          CellText := MagicAttackDecValueTypeNames[IncAttribData.Data.LowValueIsPoint];
      end;
    5:
      begin
        if IncAttribData.AttribType >= aaHitPoint then
          CellText := '-'
        else
          CellText := IntToStr(IncAttribData.Data.LowValueAdd);
      end;
    6:
      begin
        if IncAttribData.AttribType in [aaHide] then
          CellText := '-'
        else
          CellText := IntToStr(IncAttribData.Data.HighValue);
      end;
    7:
      begin
        if IncAttribData.AttribType in [aaHide] then
          CellText := '-'
        else
          CellText := MagicAttackDecValueTypeNames[IncAttribData.Data.HighValueIsPoint];
      end;
    8:
      begin
        if IncAttribData.AttribType in [aaHide] then
          CellText := '-'
        else
          CellText := IntToStr(IncAttribData.Data.HighValueAdd);
      end;
    9:
      begin
        if IncAttribData.AttribType in [aaHP, aaMP] then
          CellText := '-'
        else
          CellText := IntToStr(IncAttribData.Data.Time);
      end;
    10:
      begin
        if IncAttribData.AttribType in [aaHP, aaMP] then
          CellText := '-'
        else
          CellText := IntToStr(IncAttribData.Data.TimeAdd);
      end;
    11:
      begin
        if IncAttribData.AttribType in [aaHP, aaMP] then
          CellText := '-'
        else
          CellText := MagicAttackDecTimeTypeNames[IncAttribData.Data.TimeAddIsPoint];
      end;
    12:
      CellText := ' ';
    13:
      CellText := IncAttribData.Data.HintText;
  end;
end;

procedure TFrmCustomMagic.vstProtectedAddAttrEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var
  Allowed: Boolean);
var
  IncAttribData: PProtectAddAttribData;
begin
  Allowed := (Node <> nil) and (Column > 0) and (Column <> 12);
  if Allowed then
  begin
    IncAttribData := Sender.GetNodeData(Node);
    if (Column in [3, 4, 5]) and (IncAttribData.AttribType >= aaHitPoint) then
      Allowed := False
    else if (Column in [6, 7, 8]) and (IncAttribData.AttribType = aaHide) then
      Allowed := False
    else if (Column in [9, 10, 11]) and (IncAttribData.AttribType in [aaHP, aaMP]) then
      Allowed := False;
  end;
end;

procedure TFrmCustomMagic.vstProtectedAddAttrCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; out
  EditLink: IVTEditLink);
begin
  EditLink := TDecAttribPropertyEditLink.Create;
end;

procedure TFrmCustomMagic.WMStartEditingIncAttrib(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WPARAM);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstProtectedAddAttr.EditNode(Node, Message.LParam);
end;

procedure TFrmCustomMagic.chkMagicACHumClick(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TCheckBox) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Integer(Low(FCurrentServerConfig.AttackBreakDefense))) and (WinCtrl.Tag <= Integer(High(FCurrentServerConfig.AttackBreakDefense)))
      then
      FCurrentServerConfig.AttackBreakDefense[TBreakDefenseType(WinCtrl.Tag)].IsChecked := TCheckBox(Sender).Checked;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seMagicACHumRateChange(Sender: TObject);
var
  WinCtrl: TSpinEdit;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TSpinEdit) then
  begin
    WinCtrl := Sender as TSpinEdit;
    if (WinCtrl.Tag >= Integer(Low(FCurrentServerConfig.AttackBreakDefense))) and (WinCtrl.Tag <= Integer(High(FCurrentServerConfig.AttackBreakDefense)))
      then
      FCurrentServerConfig.AttackBreakDefense[TBreakDefenseType(WinCtrl.Tag)].Rate := WinCtrl.Value;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seMagicACHumRateAddChange(Sender: TObject);
var
  WinCtrl: TSpinEdit;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TSpinEdit) then
  begin
    WinCtrl := Sender as TSpinEdit;
    if (WinCtrl.Tag >= Integer(Low(FCurrentServerConfig.AttackBreakDefense))) and (WinCtrl.Tag <= Integer(High(FCurrentServerConfig.AttackBreakDefense)))
      then
      FCurrentServerConfig.AttackBreakDefense[TBreakDefenseType(WinCtrl.Tag)].RateAdd := WinCtrl.Value;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seMagicACHumValueChange(Sender: TObject);
var
  WinCtrl: TSpinEdit;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TSpinEdit) then
  begin
    WinCtrl := Sender as TSpinEdit;
    if (WinCtrl.Tag >= Integer(Low(FCurrentServerConfig.AttackBreakDefense))) and (WinCtrl.Tag <= Integer(High(FCurrentServerConfig.AttackBreakDefense)))
      then
      FCurrentServerConfig.AttackBreakDefense[TBreakDefenseType(WinCtrl.Tag)].Value := WinCtrl.Value;

    SetConfigChanged();
  end;
end;

procedure TFrmCustomMagic.seMagicACHumValueAddChange(Sender: TObject);
var
  WinCtrl: TSpinEdit;
begin
  if (FCurrentServerConfig <> nil) and (Sender is TSpinEdit) then
  begin
    WinCtrl := Sender as TSpinEdit;
    if (WinCtrl.Tag >= Integer(Low(FCurrentServerConfig.AttackBreakDefense))) and (WinCtrl.Tag <= Integer(High(FCurrentServerConfig.AttackBreakDefense)))
      then
      FCurrentServerConfig.AttackBreakDefense[TBreakDefenseType(WinCtrl.Tag)].ValueAdd := WinCtrl.Value;

    SetConfigChanged();
  end;
end;

end.


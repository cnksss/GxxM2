unit MonsterConfig;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ComCtrls, StdCtrls, Spin, IniFiles, Grobal2,
  Menus, ExtCtrls, SpinEditEx, uCustomMonsterUtils, VirtualTrees, EDCode, M2Threads, ItemDropLimit, Math, uFrmItemDropLog,
  M2Definition, CheckUnit, DateUtils, System.ImageList, Vcl.ImgList;

const
  WM_STARTEDITING_MONSTER = WM_USER + 778;
  WM_STARTEDITING_ITEMRULE = WM_USER + 780;

type
  TfrmMonsterConfig = class(TForm)
    PageControl1: TPageControl;
    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    GroupBox2: TGroupBox;
    ListBoxMonsterList: TListBox;
    GroupBoxMonsterConfig: TGroupBox;
    GroupBoxMonsterUseItem: TGroupBox;
    Label31: TLabel;
    Label32: TLabel;
    Label33: TLabel;
    Label34: TLabel;
    Label35: TLabel;
    Label36: TLabel;
    Label37: TLabel;
    Label38: TLabel;
    Label39: TLabel;
    Label40: TLabel;
    Label41: TLabel;
    Label42: TLabel;
    Label43: TLabel;
    EditDRESSNAME: TEdit;
    EditWEAPONNAME: TEdit;
    EditNECKLACENAME: TEdit;
    EditRIGHTHANDNAME: TEdit;
    EditRINGLNAME: TEdit;
    EditARMRINGRNAME: TEdit;
    EditARMRINGLNAME: TEdit;
    EditHELMETNAME: TEdit;
    EditBELTNAME: TEdit;
    EditBUJUKNAME: TEdit;
    EditRINGRNAME: TEdit;
    EditBOOTSNAME: TEdit;
    EditCHARMNAME: TEdit;
    GroupBox5: TGroupBox;
    ListBoxMonsterMagicList: TListBox;
    GroupBox6: TGroupBox;
    lstItemList: TListBox;
    GroupBox7: TGroupBox;
    ListBoxMagicList: TListBox;
    ButtonMonsterConfigSave: TButton;
    ButtonMonUseItemsSave: TButton;
    PopupMenuItem: TPopupMenu;
    MenuItem_ShowAll: TMenuItem;
    MenuItem_ShowDress: TMenuItem;
    MenuItem_ShowWEAPON: TMenuItem;
    MenuItem_ShowHELMET: TMenuItem;
    MenuItem_ShowNECKLACE: TMenuItem;
    MenuItem_ShowARMRING: TMenuItem;
    MenuItem_ShowRING: TMenuItem;
    MenuItem_ShowRIGHTHAND: TMenuItem;
    MenuItem_ShowBUJUK: TMenuItem;
    MenuItem_ShowBOOTS: TMenuItem;
    MenuItem_CHARM: TMenuItem;
    MenuItem_BELT: TMenuItem;
    MenuItem_ShowOTHERITEM: TMenuItem;
    GroupBox4: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    seEditHair: TSpinEditEx;
    MenuItem_ShowDrugIrem: TMenuItem;
    MenuItem_ShowBindIrem: TMenuItem;
    GroupBox9: TGroupBox;
    cbbJob: TComboBox;
    cbbGender: TComboBox;
    ButtonMonUseItemsChange: TButton;
    RadioGroupMonsterNeedMagicItem: TRadioGroup;
    GroupBox67: TGroupBox;
    Label158: TLabel;
    Label162: TLabel;
    Label163: TLabel;
    EditMonsterWarrorAttackTime: TSpinEditEx;
    EditMonsterTaoistAttackTime: TSpinEditEx;
    EditMonsterWizardAttackTime: TSpinEditEx;
    GroupBox66: TGroupBox;
    Label152: TLabel;
    Label154: TLabel;
    Label156: TLabel;
    EditMonsterWarrorWalkTime: TSpinEditEx;
    EditMonsterWizardWalkTime: TSpinEditEx;
    EditMonsterTaoistWalkTime: TSpinEditEx;
    chkProtectMode: TCheckBox;
    Label4: TLabel;
    EditRestrictMonsterRange: TSpinEditEx;
    CheckBoxNonUseSpellPoint: TCheckBox;
    Label61: TLabel;
    EditHATNAME: TEdit;
    Label66: TLabel;
    EditDRUMNAME: TEdit;
    Label7: TLabel;
    EditMagicLevel: TSpinEditEx;
    EditMagicNewLevel: TSpinEditEx;
    Label8: TLabel;
    GroupBox3: TGroupBox;
    Label9: TLabel;
    Label10: TLabel;
    seMonButchDelayClearTime: TSpinEditEx;
    ButtonGeneralSave: TButton;
    GroupBox1: TGroupBox;
    Label11: TLabel;
    Label12: TLabel;
    seNoHumanClearMonTime: TSpinEditEx;
    chkNoHumanClearMon: TCheckBox;
    N1: TMenuItem;
    tmrFlash: TTimer;
    chkRunWithAcctack: TCheckBox;
    GroupBox10: TGroupBox;
    lbl1: TLabel;
    edtMonsterShowFormat: TEdit;
    tsCustomMonster: TTabSheet;
    grpMonster: TGroupBox;
    pnlBottom: TPanel;
    btnSave: TButton;
    chkSendCustomMonsterConfig: TCheckBox;
    vstCustomMonster: TVirtualStringTree;
    lbl13: TLabel;
    btn1: TButton;
    dlgSaveMonsters: TSaveDialog;
    grp6: TGroupBox;
    chkDieDropUseItem: TCheckBox;
    Label5: TLabel;
    seDieDropUseItemRate: TSpinEditEx;
    grp8: TGroupBox;
    chkButchUseItem: TCheckBox;
    chkDieDropBagItem: TCheckBox;
    Label6: TLabel;
    seButchUseItemRate: TSpinEditEx;
    chkButchListItem: TCheckBox;
    lbl21: TLabel;
    cbbButchChargeMode: TComboBox;
    Label165: TLabel;
    seButchChargeCount: TSpinEditEx;
    chkButchItemTrigger: TCheckBox;
    chkOnlyButchItemDelGold: TCheckBox;
    lbl22: TLabel;
    mniN2: TMenuItem;
    Label181: TLabel;
    edtShield: TEdit;
    N2: TMenuItem;
    lbl25: TLabel;
    seRunWithAcctackRate: TSpinEditEx;
    dlgSaveNpcs: TSaveDialog;
    ilCheck: TImageList;
    grp15: TGroupBox;
    Label212: TLabel;
    Label213: TLabel;
    seElfWarriorMonsterDownDelay: TSpinEditEx;
    GroupBox188: TGroupBox;
    Label783: TLabel;
    Label784: TLabel;
    seMaxMapItemCount: TSpinEditEx;
    chkNotDropOverlapItemAll: TCheckBox;
    seMonOneDropGoldCount: TSpinEditEx;
    chkDropGoldToPlayBag: TCheckBox;
    seScatterItemRange: TSpinEditEx;
    chkEnabledMaxMapItemCount: TCheckBox;
    lbl33: TLabel;
    cbbMonsterShowLevel: TComboBox;
    tsDropItem: TTabSheet;
    lstAllItems: TListBox;
    pnlItemDropRight: TPanel;
    grp9: TGroupBox;
    vstItemRules: TVirtualStringTree;
    vstDropLimitItems: TVirtualStringTree;
    btnClearItemRule: TButton;
    Button1: TButton;
    lbl34: TLabel;
    Label23: TLabel;
    chkRecordLog: TCheckBox;
    edtItemSearch: TEdit;
    lbl35: TLabel;
    pmItemRules: TPopupMenu;
    miItemRulesLog: TMenuItem;
    GroupBox8: TGroupBox;
    lbl36: TLabel;
    lbl37: TLabel;
    lbl38: TLabel;
    pnl1: TPanel;
    pgcMain: TPageControl;
    tsAction: TTabSheet;
    pnlActionBottom: TPanel;
    grp1: TGroupBox;
    lbl3: TLabel;
    Label13: TLabel;
    cbbBatchAction: TComboBox;
    cbbBatchEffect: TComboBox;
    GroupBox11: TGroupBox;
    Label103: TLabel;
    Label104: TLabel;
    Label185: TLabel;
    cbbClientDrawMode: TComboBox;
    cbbClientDrawOrder2: TComboBox;
    cbbClientDrawMode2: TComboBox;
    grp3: TGroupBox;
    lbl9: TLabel;
    Label50: TLabel;
    Label51: TLabel;
    Label109: TLabel;
    Label110: TLabel;
    Label111: TLabel;
    Label112: TLabel;
    Label113: TLabel;
    Label114: TLabel;
    lbl10: TLabel;
    Label128: TLabel;
    Label129: TLabel;
    edtSoundNormal: TEdit;
    edtSoundAttack: TEdit;
    edtSoundDigUP: TEdit;
    edtSoundStruck: TEdit;
    edtSoundDie: TEdit;
    edtSoundAttack1: TEdit;
    edtSoundAttack3: TEdit;
    edtSoundAttack2: TEdit;
    edtSoundAttack4: TEdit;
    edtSoundAttack5: TEdit;
    edtSoundAttack6: TEdit;
    GroupBox17: TGroupBox;
    Label100: TLabel;
    Label101: TLabel;
    seClientStartIndex: TSpinEditEx;
    seClientEffectIndex: TSpinEditEx;
    btnCalcStartIndex: TButton;
    btnCalcEffectIndex: TButton;
    grp7: TGroupBox;
    lbl20: TLabel;
    lbl14: TLabel;
    Label125: TLabel;
    Label126: TLabel;
    Label127: TLabel;
    Label169: TLabel;
    Label170: TLabel;
    Label171: TLabel;
    Label182: TLabel;
    Label183: TLabel;
    Label184: TLabel;
    seHPOffsetX: TSpinEditEx;
    seHPOffsetY: TSpinEditEx;
    cbbHPFile: TComboBox;
    seHPStartIndex: TSpinEditEx;
    seHPTextOffsetX: TSpinEditEx;
    seHPTextOffsetY: TSpinEditEx;
    seHPBgOffsetX: TSpinEditEx;
    seHPBgOffsetY: TSpinEditEx;
    grp5: TGroupBox;
    chkDieNoCalcDir: TCheckBox;
    vstAction: TVirtualStringTree;
    tsAttack: TTabSheet;
    grpClientAttackConfigs: TGroupBox;
    lbl24: TLabel;
    grpFly: TGroupBox;
    lbl5: TLabel;
    Label15: TLabel;
    Label16: TLabel;
    Label17: TLabel;
    Label18: TLabel;
    Label19: TLabel;
    Label27: TLabel;
    Label173: TLabel;
    cbbClientFlyFile: TComboBox;
    cbbClientFlyDrawMode: TComboBox;
    cbbClientFlyDirCount: TComboBox;
    chkClientFlyCalcDir: TCheckBox;
    seClientFlyPlayTime: TSpinEditEx;
    seClientFlyEmptyCount: TSpinEditEx;
    seClientFlyPlayCount: TSpinEditEx;
    seClientFlyStartIndex: TSpinEditEx;
    seFlyLightRange: TSpinEditEx;
    grpSelf: TGroupBox;
    Label20: TLabel;
    Label21: TLabel;
    Label22: TLabel;
    Label26: TLabel;
    Label24: TLabel;
    Label107: TLabel;
    Label25: TLabel;
    Label167: TLabel;
    cbbClientSelfFile: TComboBox;
    cbbClientSelfDrawOrder: TComboBox;
    seClientSelfPlayTime: TSpinEditEx;
    seClientSelfPlayCount: TSpinEditEx;
    seClientSelfStartIndex: TSpinEditEx;
    cbbClientSelfDrawMode: TComboBox;
    seClientSelfEmptyCount: TSpinEditEx;
    chkClientSelfPlayDelayAction: TCheckBox;
    seSelfLightRange: TSpinEditEx;
    grpExplosion: TGroupBox;
    Label28: TLabel;
    Label29: TLabel;
    Label30: TLabel;
    Label46: TLabel;
    Label49: TLabel;
    Label172: TLabel;
    Label207: TLabel;
    Label208: TLabel;
    Label209: TLabel;
    Bevel1: TBevel;
    cbbClientExplosionFile: TComboBox;
    seClientExplosionPlayTime: TSpinEditEx;
    seClientExplosionPlayCount: TSpinEditEx;
    seClientExplosionStartIndex: TSpinEditEx;
    cbbClientExplosionDrawMode: TComboBox;
    chkClientExplosionLockDraw: TCheckBox;
    seExplosionLightRange: TSpinEditEx;
    chkClientExplosionKeepPlay: TCheckBox;
    seClientExplosionKeepTime: TSpinEditEx;
    seClientExplosionKeepAttackRange: TSpinEditEx;
    seClientExplosionKeepAttackInterval: TSpinEditEx;
    chkClientExplosionKeepMultiPlay: TCheckBox;
    grpTarget: TGroupBox;
    Label44: TLabel;
    Label45: TLabel;
    Label47: TLabel;
    Label48: TLabel;
    Label108: TLabel;
    Label168: TLabel;
    Label174: TLabel;
    Label175: TLabel;
    Label176: TLabel;
    lbl23: TLabel;
    Label177: TLabel;
    Label178: TLabel;
    bvl1: TBevel;
    Label236: TLabel;
    cbbClientTargetFile: TComboBox;
    seClientTargetPlayTime: TSpinEditEx;
    seClientTargetPlayCount: TSpinEditEx;
    seClientTargetStartIndex: TSpinEditEx;
    cbbClientTargetDrawMode: TComboBox;
    chkClientTargetMultiPlay: TCheckBox;
    chkClientTargetLockDraw: TCheckBox;
    seTargetLightRange: TSpinEditEx;
    chkTargetKeepPlay: TCheckBox;
    seTargetKeepTime: TSpinEditEx;
    seTargetKeepAttackRange: TSpinEditEx;
    seTargetKeepAttackInterval: TSpinEditEx;
    chkTargetKeepMultiPlay: TCheckBox;
    seTargetKeepLightRange: TSpinEditEx;
    grp2: TGroupBox;
    Label14: TLabel;
    Label105: TLabel;
    Label106: TLabel;
    cbbClientFlyEffFile: TComboBox;
    seClientFlyEffStartIndex: TSpinEditEx;
    cbbClientFlyEffDrawMode: TComboBox;
    GroupBox18: TGroupBox;
    lbl4: TLabel;
    cbbClientAttackConfig: TComboBox;
    tsServerAttack: TTabSheet;
    grp4: TGroupBox;
    Label55: TLabel;
    Label56: TLabel;
    lbl8: TLabel;
    Label115: TLabel;
    lblProtect: TLabel;
    lbl15: TLabel;
    Label166: TLabel;
    cbbServerAttackConfig: TComboBox;
    seViewRange: TSpinEditEx;
    cbbMonsterType: TComboBox;
    cbbMoveOption: TComboBox;
    seProtectRange: TSpinEditEx;
    seMinAttackNearRange: TSpinEditEx;
    seLightRange: TSpinEditEx;
    chkNoAttack: TCheckBox;
    grpServerAttackConfigs: TGroupBox;
    lbl12: TLabel;
    lbl2: TLabel;
    chkAttackEnabled: TCheckBox;
    cbbOperateMode: TComboBox;
    GroupBox12: TGroupBox;
    Label59: TLabel;
    Label60: TLabel;
    lbl6: TLabel;
    Label57: TLabel;
    Label64: TLabel;
    Label71: TLabel;
    seAttackRate: TSpinEditEx;
    seAttackHPPercent: TSpinEditEx;
    seAttackTargetCount: TSpinEditEx;
    grpOptions: TGroupBox;
    Label52: TLabel;
    Label53: TLabel;
    Label54: TLabel;
    Label72: TLabel;
    Label75: TLabel;
    Label62: TLabel;
    Label63: TLabel;
    Label73: TLabel;
    Label74: TLabel;
    lbl26: TLabel;
    seAttackNearRange: TSpinEditEx;
    seAttackGroupRange: TSpinEditEx;
    cbbAttackMode: TComboBox;
    cbbAttackTarget: TComboBox;
    chkAttackIgnoreDefence: TCheckBox;
    cbbAttackPowerCalc: TComboBox;
    seAttackPowerRate: TSpinEditEx;
    chkAttackSelfDie: TCheckBox;
    seAttackPowerInc: TSpinEditEx;
    grpMove: TGroupBox;
    lbl16: TLabel;
    Label65: TLabel;
    Label67: TLabel;
    Label68: TLabel;
    Label69: TLabel;
    Label70: TLabel;
    chkAttackTeleportAttack: TCheckBox;
    seAttackTeleportDistance: TSpinEditEx;
    seAttackTeleportRate: TSpinEditEx;
    seAttackTeleportTargetDistance: TSpinEditEx;
    grpCallMob: TGroupBox;
    lbl11: TLabel;
    Label97: TLabel;
    Label98: TLabel;
    Label99: TLabel;
    Label117: TLabel;
    Label118: TLabel;
    Label121: TLabel;
    Label122: TLabel;
    Label143: TLabel;
    Label144: TLabel;
    edtCallMonster1: TEdit;
    seCallMonsterNum1: TSpinEditEx;
    edtCallMonster2: TEdit;
    seCallMonsterNum2: TSpinEditEx;
    edtCallMonster3: TEdit;
    seCallMonsterNum3: TSpinEditEx;
    edtCallMonster4: TEdit;
    seCallMonsterNum4: TSpinEditEx;
    chkEnabledCallMonster: TCheckBox;
    seCallMonstersRate: TSpinEditEx;
    grpAdditionals: TGroupBox;
    Label76: TLabel;
    Label77: TLabel;
    Label78: TLabel;
    Label79: TLabel;
    Label80: TLabel;
    Label81: TLabel;
    Label82: TLabel;
    Label83: TLabel;
    Label84: TLabel;
    Label85: TLabel;
    Label86: TLabel;
    Label87: TLabel;
    Label88: TLabel;
    Label89: TLabel;
    Label90: TLabel;
    Label91: TLabel;
    Label92: TLabel;
    Label93: TLabel;
    Label94: TLabel;
    Label95: TLabel;
    Label96: TLabel;
    Label141: TLabel;
    Label142: TLabel;
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
    grpProtect: TGroupBox;
    Label130: TLabel;
    Label116: TLabel;
    Label119: TLabel;
    Label120: TLabel;
    Label123: TLabel;
    Label102: TLabel;
    Label124: TLabel;
    Label134: TLabel;
    Label135: TLabel;
    Label131: TLabel;
    Label132: TLabel;
    Label133: TLabel;
    Label136: TLabel;
    Label138: TLabel;
    Label139: TLabel;
    Label137: TLabel;
    Label58: TLabel;
    lbl7: TLabel;
    lbl17: TLabel;
    lbl18: TLabel;
    Label140: TLabel;
    Label145: TLabel;
    Label146: TLabel;
    Label147: TLabel;
    Label148: TLabel;
    Label149: TLabel;
    Label150: TLabel;
    Label151: TLabel;
    Label153: TLabel;
    Label155: TLabel;
    Label157: TLabel;
    Label159: TLabel;
    Label160: TLabel;
    Label161: TLabel;
    Label164: TLabel;
    chkProtectAddHP: TCheckBox;
    chkProtectAddDefence: TCheckBox;
    chkProtectAddMagDefence: TCheckBox;
    seProtectAddHPRate: TSpinEditEx;
    seProtectAddHPPercent: TSpinEditEx;
    seProtectAddDefenceRate: TSpinEditEx;
    seProtectAddDefenceTime: TSpinEditEx;
    seProtectAddMagDefenceRate: TSpinEditEx;
    seProtectAddMagDefenceTime: TSpinEditEx;
    seProtectTargetRange: TSpinEditEx;
    seProtectSelfRate: TSpinEditEx;
    seProtectAddDCRate: TSpinEditEx;
    seProtectAddDCPercent: TSpinEditEx;
    chkProtectAddDC: TCheckBox;
    cbb1: TComboBox;
    seProtectAddDCTime: TSpinEditEx;
    seProtectAddMCRate: TSpinEditEx;
    seProtectAddMCPercent: TSpinEditEx;
    chkProtectAddMC: TCheckBox;
    seProtectAddMCTime: TSpinEditEx;
    seProtectAddSCRate: TSpinEditEx;
    seProtectAddSCPercent: TSpinEditEx;
    chkProtectAddSC: TCheckBox;
    seProtectAddSCTime: TSpinEditEx;
    grpMoveTarget: TGroupBox;
    Label179: TLabel;
    Label180: TLabel;
    seMoveTargetRate: TSpinEditEx;
    chkMoveTarget: TCheckBox;
    chkMoveTargetHighLevel: TCheckBox;
    pnlMonDesc: TPanel;
    Label214: TLabel;
    seExplosionKeepLightRange: TSpinEditEx;
    Label215: TLabel;
    Label216: TLabel;
    Label217: TLabel;
    Label218: TLabel;
    seClientExplosionStartIndex2: TSpinEditEx;
    Label219: TLabel;
    seClientTargetStartIndex2: TSpinEditEx;
    Label220: TLabel;
    cbbClientTargetDrawMode2: TComboBox;
    Label221: TLabel;
    cbbClientExplosionDrawMode2: TComboBox;
    lbl39: TLabel;
    GroupBox22: TGroupBox;
    Label222: TLabel;
    Label223: TLabel;
    seMonStruckFrameDelayTime: TSpinEditEx;
    Label224: TLabel;
    Label225: TLabel;
    cbbClientSelfDirCount: TComboBox;
    cbbClientSelfDirCalcType: TComboBox;
    chkNearAttackTargetCenter: TCheckBox;
    chkNoAttackMode: TCheckBox;
    GroupBox23: TGroupBox;
    lbl19: TLabel;
    edtMagStruckMonLevel: TSpinEditEx;
    Label226: TLabel;
    edtMagStruckMonDecTime: TSpinEditEx;
    Label227: TLabel;
    edtMagStruckMonDecRandom: TSpinEditEx;
    lbl40: TLabel;
    lblAttackDelay: TLabel;
    lblAttackDelayTime: TLabel;
    seAttackDelayTime: TSpinEditEx;
    lbl27: TLabel;
    lbl28: TLabel;
    chkAdditional11: TCheckBox;
    seAdditionalRate11: TSpinEditEx;
    seAdditionalTime11: TSpinEditEx;
    lbl29: TLabel;
    seAdditionalImprisonRange: TSpinEditEx;
    lbl30: TLabel;
    lbl31: TLabel;
    lbl32: TLabel;
    lbl41: TLabel;
    seProtectAddDefencePercent: TSpinEditEx;
    seProtectAddMagDefencePercent: TSpinEditEx;
    grp10: TGroupBox;
    lbl42: TLabel;
    lbl43: TLabel;
    lbl44: TLabel;
    lbl45: TLabel;
    lbl46: TLabel;
    lbl47: TLabel;
    lbl48: TLabel;
    cbbClientSelfKeepFile: TComboBox;
    seClientSelfKeepPlayTime: TSpinEditEx;
    seClientSelfKeepPlayCount: TSpinEditEx;
    seClientSelfKeepStartIndex: TSpinEditEx;
    cbbClientSelfKeepDrawMode: TComboBox;
    seClientSelfKeepTime: TSpinEditEx;
    Label186: TLabel;
    seClientSelfKeepStartIndex2: TSpinEditEx;
    Label187: TLabel;
    cbbClientSelfKeepDrawMode2: TComboBox;
    Label188: TLabel;
    cbbClientSelfKeepDrawOrder: TComboBox;
    chkAttackTeleportRush: TCheckBox;
    grpSetDamage: TGroupBox;
    chkDamageLimitation: TCheckBox;
    procedure ButtonGeneralSaveClick(Sender: TObject);
    procedure seMonOneDropGoldCountChange(Sender: TObject);
    procedure chkDropGoldToPlayBagClick(Sender: TObject);
    procedure lstItemListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure ListBoxMagicListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure ListBoxMonsterListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure ListBoxMonsterBagItemListDragOver(Sender, Source: TObject; X, Y: Integer; State: TDragState; var Accept: Boolean);
    procedure ListBoxMonsterMagicListDragDrop(Sender, Source: TObject; X, Y: Integer);
    procedure GroupBoxMonsterUseItemDockOver(Sender: TObject; Source: TDragDockObject; X, Y: Integer; State: TDragState;
      var Accept: Boolean);
    procedure GroupBoxMonsterUseItemDragDrop(Sender, Source: TObject; X, Y: Integer);
    procedure MenuItem_ShowAllClick(Sender: TObject);
    procedure ListBoxMonsterListClick(Sender: TObject);
    procedure EditDRESSNAMEChange(Sender: TObject);
    procedure ButtonMonUseItemsSaveClick(Sender: TObject);
    procedure ButtonMonUseItemsChangeClick(Sender: TObject);
    procedure ListBoxMonsterMagicListDblClick(Sender: TObject);
    procedure cbbJobChange(Sender: TObject);
    procedure ListBoxMagicListDblClick(Sender: TObject);
    procedure EditMonsterWarrorAttackTimeChange(Sender: TObject);
    procedure EditMonsterWizardAttackTimeChange(Sender: TObject);
    procedure EditMonsterTaoistAttackTimeChange(Sender: TObject);
    procedure EditMonsterWarrorWalkTimeChange(Sender: TObject);
    procedure EditMonsterWizardWalkTimeChange(Sender: TObject);
    procedure EditMonsterTaoistWalkTimeChange(Sender: TObject);
    procedure RadioGroupMonsterNeedMagicItemClick(Sender: TObject);
    procedure ButtonMonsterConfigSaveClick(Sender: TObject);
    procedure ListBoxMonsterMagicListClick(Sender: TObject);
    procedure EditMagicLevelChange(Sender: TObject);
    procedure seMonButchDelayClearTimeChange(Sender: TObject);
    procedure chkNoHumanClearMonClick(Sender: TObject);
    procedure seNoHumanClearMonTimeChange(Sender: TObject);
    procedure lstItemListDblClick(Sender: TObject);
    procedure tmrFlashTimer(Sender: TObject);
    procedure edtMonsterShowFormatChange(Sender: TObject);
    procedure seScatterItemRangeChange(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure vstActionGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstActionGetHint(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
      var LineBreakStyle: TVTTooltipLineBreakStyle; var HintText: string);
    procedure vstActionCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
      out EditLink: IVTEditLink);
    procedure vstActionEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstActionDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
    procedure cbbClientAttackConfigChange(Sender: TObject);
    procedure cbbClientFlyFileChange(Sender: TObject);
    procedure seClientFlyStartIndexChange(Sender: TObject);
    procedure seClientFlyPlayCountChange(Sender: TObject);
    procedure seClientFlyEmptyCountChange(Sender: TObject);
    procedure seClientFlyPlayTimeChange(Sender: TObject);
    procedure cbbClientFlyDrawModeChange(Sender: TObject);
    procedure cbbClientFlyDirCountChange(Sender: TObject);
    procedure chkClientFlyCalcDirClick(Sender: TObject);
    procedure seClientSelfStartIndexChange(Sender: TObject);
    procedure seClientSelfPlayCountChange(Sender: TObject);
    procedure seClientSelfPlayTimeChange(Sender: TObject);
    procedure cbbClientSelfPlayModeChange(Sender: TObject);
    procedure cbbClientSelfDrawOrderChange(Sender: TObject);
    procedure cbbClientExplosionFileChange(Sender: TObject);
    procedure seClientExplosionStartIndexChange(Sender: TObject);
    procedure seClientExplosionPlayCountChange(Sender: TObject);
    procedure seClientExplosionPlayTimeChange(Sender: TObject);
    procedure cbbClientTargetFileChange(Sender: TObject);
    procedure seClientTargetStartIndexChange(Sender: TObject);
    procedure seClientTargetPlayCountChange(Sender: TObject);
    procedure seClientTargetPlayTimeChange(Sender: TObject);
    procedure cbbBatchEffectChange(Sender: TObject);
    procedure cbbServerAttackConfigChange(Sender: TObject);
    procedure seViewRangeChange(Sender: TObject);
    procedure chkAttackEnabledClick(Sender: TObject);
    procedure seAttackHPPercentChange(Sender: TObject);
    procedure seAttackRateChange(Sender: TObject);
    procedure seAttackTargetCountChange(Sender: TObject);
    procedure cbbAttackModeChange(Sender: TObject);
    procedure cbbAttackTargetChange(Sender: TObject);
    procedure cbbAttackPowerCalcChange(Sender: TObject);
    procedure seAttackPowerRateChange(Sender: TObject);
    procedure chkAttackTeleportAttackClick(Sender: TObject);
    procedure chkAttackIgnoreDefenceClick(Sender: TObject);
    procedure seAttackNearRangeChange(Sender: TObject);
    procedure seAttackGroupRangeChange(Sender: TObject);
    procedure chkAdditional0Click(Sender: TObject);
    procedure seAdditionalRate0Change(Sender: TObject);
    procedure seAdditionalTime0Change(Sender: TObject);
    procedure seAdditionaHP0Change(Sender: TObject);
    procedure chkseAdditionaHighLevel4Click(Sender: TObject);
    procedure btnSaveClick(Sender: TObject);
    procedure cbbClientDrawModeChange(Sender: TObject);
    procedure cbbClientDrawOrder2Change(Sender: TObject);
    procedure btnCalcStartIndexClick(Sender: TObject);
    procedure vstActionNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure btnCalcEffectIndexClick(Sender: TObject);
    procedure chkSendCustomMonsterConfigClick(Sender: TObject);
    procedure cbbBatchActionChange(Sender: TObject);
    procedure vstActionChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
    procedure vstActionGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
    procedure vstCustomMonsterGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
    procedure vstCustomMonsterNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure cbbClientFlyEffFileChange(Sender: TObject);
    procedure seClientFlyEffStartIndexChange(Sender: TObject);
    procedure cbbClientFlyEffDrawModeChange(Sender: TObject);
    procedure cbbClientSelfDrawModeChange(Sender: TObject);
    procedure cbbClientTargetDrawModeChange(Sender: TObject);
    procedure cbbClientExplosionDrawModeChange(Sender: TObject);
    procedure cbbMonsterTypeChange(Sender: TObject);
    procedure edtSoundNormalChange(Sender: TObject);
    procedure cbbMoveOptionChange(Sender: TObject);
    procedure seProtectRangeChange(Sender: TObject);
    procedure edtCallMonster1Change(Sender: TObject);
    procedure seCallMonsterNum1Change(Sender: TObject);
    procedure chkCopySelfClick(Sender: TObject);
    procedure seCopySelfMaxCountChange(Sender: TObject);
    procedure seCopySelfTimeChange(Sender: TObject);
    procedure chkAttackSelfDieClick(Sender: TObject);
    procedure cbbHPFileChange(Sender: TObject);
    procedure seHPOffsetXChange(Sender: TObject);
    procedure seHPOffsetYChange(Sender: TObject);
    procedure seHPStartIndexChange(Sender: TObject);
    procedure seMinAttackNearRangeChange(Sender: TObject);
    procedure cbbOperateModeChange(Sender: TObject);
    procedure seAttackTeleportTargetDistanceChange(Sender: TObject);
    procedure seAttackTeleportDistanceChange(Sender: TObject);
    procedure seAttackTeleportRateChange(Sender: TObject);
    procedure chkProtectAddHPClick(Sender: TObject);
    procedure seProtectAddHPRateChange(Sender: TObject);
    procedure seProtectAddHPPercentChange(Sender: TObject);
    procedure chkProtectAddDefenceClick(Sender: TObject);
    procedure seProtectAddDefenceRateChange(Sender: TObject);
    procedure seProtectAddDefenceTimeChange(Sender: TObject);
    procedure chkProtectAddMagDefenceClick(Sender: TObject);
    procedure seProtectAddMagDefenceRateChange(Sender: TObject);
    procedure seProtectAddMagDefenceTimeChange(Sender: TObject);
    procedure seProtectTargetRangeChange(Sender: TObject);
    procedure seProtectSelfRateChange(Sender: TObject);
    procedure chkEnabledCallMonsterClick(Sender: TObject);
    procedure chkProtectAddDCClick(Sender: TObject);
    procedure seProtectAddDCRateChange(Sender: TObject);
    procedure seProtectAddDCPercentChange(Sender: TObject);
    procedure seClientSelfEmptyCountChange(Sender: TObject);
    procedure seCallMonstersRateChange(Sender: TObject);
    procedure btn1Click(Sender: TObject);
    procedure chkDieNoCalcDirClick(Sender: TObject);
    procedure chkClientTargetMultiPlayClick(Sender: TObject);
    procedure chkClientSelfPlayDelayActionClick(Sender: TObject);
    procedure chkClientTargetLockDrawClick(Sender: TObject);
    procedure chkClientExplosionLockDrawClick(Sender: TObject);
    procedure seProtectAddDCTimeChange(Sender: TObject);
    procedure chkProtectAddMCClick(Sender: TObject);
    procedure seProtectAddMCRateChange(Sender: TObject);
    procedure seProtectAddMCPercentChange(Sender: TObject);
    procedure seProtectAddMCTimeChange(Sender: TObject);
    procedure chkProtectAddSCClick(Sender: TObject);
    procedure seProtectAddSCRateChange(Sender: TObject);
    procedure seProtectAddSCPercentChange(Sender: TObject);
    procedure seProtectAddSCTimeChange(Sender: TObject);
    procedure chkDieDropBagItemClick(Sender: TObject);
    procedure seLightRangeChange(Sender: TObject);
    procedure seSelfLightRangeChange(Sender: TObject);
    procedure seTargetLightRangeChange(Sender: TObject);
    procedure seHPTextOffsetXChange(Sender: TObject);
    procedure seHPTextOffsetYChange(Sender: TObject);
    procedure seFlyLightRangeChange(Sender: TObject);
    procedure seExplosionLightRangeChange(Sender: TObject);
    procedure chkTargetKeepPlayClick(Sender: TObject);
    procedure seTargetKeepTimeChange(Sender: TObject);
    procedure seTargetKeepAttackRangeChange(Sender: TObject);
    procedure seTargetKeepAttackIntervalChange(Sender: TObject);
    procedure chkTargetKeepMultiPlayClick(Sender: TObject);
    procedure cbbClientSelfFileChange(Sender: TObject);
    procedure chkMoveTargetClick(Sender: TObject);
    procedure seMoveTargetRateChange(Sender: TObject);
    procedure chkMoveTargetHighLevelClick(Sender: TObject);
    procedure seTargetKeepLightRangeChange(Sender: TObject);
    procedure seHPBgOffsetXChange(Sender: TObject);
    procedure seHPBgOffsetYChange(Sender: TObject);
    procedure seAttackPowerIncChange(Sender: TObject);
    procedure cbbClientDrawMode2Change(Sender: TObject);
    procedure seElfWarriorMonsterDownDelayChange(Sender: TObject);
    procedure seMaxMapItemCountChange(Sender: TObject);
    procedure chkNotDropOverlapItemAllClick(Sender: TObject);
    procedure chkEnabledMaxMapItemCountClick(Sender: TObject);
    procedure cbbMonsterShowLevelChange(Sender: TObject);
    procedure vstDropLimitItemsGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
    procedure vstItemRulesGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
    procedure vstDropLimitItemsGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstDropLimitItemsNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure lstAllItemsDblClick(Sender: TObject);
    procedure vstItemRulesGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstItemRulesCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
      out EditLink: IVTEditLink);
    procedure vstItemRulesEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstItemRulesNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure vstItemRulesKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure btnAddItemRuleClick(Sender: TObject);
    procedure btnClearItemRuleClick(Sender: TObject);
    procedure Button1Click(Sender: TObject);
    procedure lstAllItemsKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure edtItemSearchChange(Sender: TObject);
    procedure edtItemSearchKeyPress(Sender: TObject; var Key: Char);
    procedure chkRecordLogClick(Sender: TObject);
    procedure pmItemRulesPopup(Sender: TObject);
    procedure vstItemRulesDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
    procedure chkNoAttackClick(Sender: TObject);
    procedure chkClientExplosionKeepPlayClick(Sender: TObject);
    procedure seClientExplosionKeepTimeChange(Sender: TObject);
    procedure seClientExplosionKeepAttackIntervalChange(Sender: TObject);
    procedure seClientExplosionKeepAttackRangeChange(Sender: TObject);
    procedure seExplosionKeepLightRangeChange(Sender: TObject);
    procedure chkClientExplosionKeepMultiPlayClick(Sender: TObject);
    procedure seClientExplosionStartIndex2Change(Sender: TObject);
    procedure seClientTargetStartIndex2Change(Sender: TObject);
    procedure cbbClientTargetDrawMode2Change(Sender: TObject);
    procedure cbbClientExplosionDrawMode2Change(Sender: TObject);
    procedure seMonStruckFrameDelayTimeChange(Sender: TObject);
    procedure cbbClientSelfDirCountChange(Sender: TObject);
    procedure cbbClientSelfDirCalcTypeChange(Sender: TObject);
    procedure chkNearAttackTargetCenterClick(Sender: TObject);
    procedure edtMagStruckMonLevelChange(Sender: TObject);
    procedure edtMagStruckMonDecTimeChange(Sender: TObject);
    procedure edtMagStruckMonDecRandomChange(Sender: TObject);
    procedure seAttackDelayTimeChange(Sender: TObject);
    procedure seAdditionalImprisonRangeChange(Sender: TObject);
    procedure seProtectAddDefencePercentChange(Sender: TObject);
    procedure seProtectAddMagDefencePercentChange(Sender: TObject);
    procedure cbbClientSelfKeepFileChange(Sender: TObject);
    procedure seClientSelfKeepStartIndexChange(Sender: TObject);
    procedure seClientSelfKeepPlayCountChange(Sender: TObject);
    procedure seClientSelfKeepPlayTimeChange(Sender: TObject);
    procedure cbbClientSelfKeepDrawModeChange(Sender: TObject);
    procedure seClientSelfKeepTimeChange(Sender: TObject);
    procedure vstCustomMonsterGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstCustomMonsterDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; Column: TColumnIndex;
      const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
    procedure seClientSelfKeepStartIndex2Change(Sender: TObject);
    procedure cbbClientSelfKeepDrawOrderChange(Sender: TObject);
    procedure cbbClientSelfKeepDrawMode2Change(Sender: TObject);
    procedure chkAttackTeleportRushClick(Sender: TObject);
    procedure chkDamageLimitationClick(Sender: TObject);
    procedure vstCustomMonsterKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
  private
    boOpened: Boolean;
    boModValued: Boolean;
    procedure ModValue();
    procedure uModValue();
    procedure RefGeneralInfo();
    { Private declarations }
    procedure SetMonsterConfigChanged(IsChanged: Boolean = True);
    procedure LoadStdItems(Fileter: Integer = -1);
    procedure RefreshDropLimitItemButtons;
    procedure WMStartEditingMonster(var Message: TMessage); message WM_STARTEDITING_MONSTER;
    procedure WMStartEditingItemRule(var Message: TMessage); message WM_STARTEDITING_ITEMRULE;
  private
    FCurrentMonsterCustomConfig: TCustomMonsterConfig;
    FCurrentMonsterClientConfig: PClientAttackConfig;
    FCurrentMonsterServerConfig: PMonsterServerConfig;
    FCurrentLimitItem: TDropLimitItem;
    FCurrentItemRule: PDropItemRule;
    FIsMonsterChanged: Boolean;
  public
    procedure Open;
    { Public declarations }
  end;

implementation

uses
  M2Share, UsrEngn, Envir;
{$R *.dfm}

type
  PMonsterConfigNodeData = ^TMonsterConfigNodeData;

  TMonsterConfigNodeData = record
    Config: TCustomMonsterConfig;
  end;

  PMonsterNodeData = ^TMonsterNodeData;

  TMonsterNodeData = record
    Action: PMonsterClientAction;
  end;

var
  dwChangeColorTick: LongWord = 0;
  SelMonsterConfig: pTPlayMonsterConfig = nil;

var
  LeftRightIndex: Integer = 0;
  FlashEdit: TEdit = nil;

type
  TMonsterPropertyEditLink = class(TInterfacedObject, IVTEditLink)
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

type
  TItemRulePropertyEditLink = class(TInterfacedObject, IVTEditLink)
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

destructor TMonsterPropertyEditLink.Destroy;
begin
  FEdit.Free;
  inherited;
end;

procedure TMonsterPropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
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

procedure TMonsterPropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
begin
  case Key of
    VK_ESCAPE:
      begin
        FTree.CancelEditNode;
        Key := 0;
      end; // VK_ESCAPE
  end; // case
end;

function TMonsterPropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

function TMonsterPropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

function TMonsterPropertyEditLink.EndEdit: Boolean;
var
  NodeData: PMonsterNodeData;
  TempValue: Integer;
  IsChanged: Boolean;
begin
  Result := True;
  IsChanged := False;
  NodeData := FTree.GetNodeData(FNode);
  if FEdit is TComboBox then
  begin
    TempValue := (FEdit as TComboBox).ItemIndex - 1;
    case FColumn of
      1:
        if NodeData.Action.ActionFile <> TempValue then
        begin
          NodeData.Action.ActionFile := TempValue;
          IsChanged := True;
        end;
      6:
        if NodeData.Action.EffectFile <> TempValue then
        begin
          NodeData.Action.EffectFile := TempValue;
          IsChanged := True;
        end;
      8:
        if NodeData.Action.EffectFile2 <> TempValue then
        begin
          NodeData.Action.EffectFile2 := TempValue;
          IsChanged := True;
        end;
    end;
  end
  else if FEdit is TSpinEditEx then
  begin
    TempValue := (FEdit as TSpinEditEx).Value;
    case FColumn of
      2:
        if NodeData.Action.StartIndex <> TempValue then
        begin
          NodeData.Action.StartIndex := TempValue;
          IsChanged := True;
        end;
      3:
        if NodeData.Action.PlayCount <> TempValue then
        begin
          NodeData.Action.PlayCount := TempValue;
          IsChanged := True;
        end;
      4:
        if NodeData.Action.EmptyCount <> TempValue then
        begin
          NodeData.Action.EmptyCount := TempValue;
          IsChanged := True;
        end;
      5:
        if NodeData.Action.PlayTime <> TempValue then
        begin
          NodeData.Action.PlayTime := TempValue;
          IsChanged := True;
        end;
      7:
        if NodeData.Action.EffectIndex <> TempValue then
        begin
          NodeData.Action.EffectIndex := TempValue;
          IsChanged := True;
        end;
      9:
        if NodeData.Action.EffectIndex2 <> TempValue then
        begin
          NodeData.Action.EffectIndex2 := TempValue;
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
    if (FTree.Owner is TfrmMonsterConfig) then
    begin
      TfrmMonsterConfig(FTree.Owner).SetMonsterConfigChanged();
    end;
  end;
end;

function TMonsterPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

function TMonsterPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  I: Integer;
  NodeData: PMonsterNodeData;
begin
  Result := True;
  FTree := Tree as TVirtualStringTree;
  FNode := Node;
  FColumn := Column;
  // determine what edit type actually is needed
  FEdit.Free;
  FEdit := nil;
  NodeData := FTree.GetNodeData(Node);
  case FColumn of
    2, 3, 4, 5, 7, 9:
      begin
        FEdit := TSpinEditEx.Create(nil);
        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          if FColumn in [2, 7, 9] then
          begin
            MinValue := -1;
            MaxValue := High(Smallint);
          end
          else
          begin
            MinValue := 0;
            MaxValue := High(Word);
          end;
          case FColumn of
            2:
              Value := NodeData.Action.StartIndex;
            3:
              Value := NodeData.Action.PlayCount;
            4:
              Value := NodeData.Action.EmptyCount;
            5:
              Value := NodeData.Action.PlayTime;
            7:
              Value := NodeData.Action.EffectIndex;
            9:
              Value := NodeData.Action.EffectIndex2;
          end;
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    1, 6, 8:
      begin
        FEdit := TComboBox.Create(nil);
        with FEdit as TComboBox do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDownList;
          Items.Add('根据Appr计算');
          for I := 0 to g_EffectImageList.Count - 1 do
            Items.Add(g_EffectImageList.Strings[I]);
          case FColumn of
            1:
              begin
                if NodeData.Action.ActionFile < 0 then
                  ItemIndex := 0
                else
                  ItemIndex := NodeData.Action.ActionFile + 1;
              end;
            6:
              begin
                if NodeData.Action.EffectFile < 0 then
                  ItemIndex := 0
                else
                  ItemIndex := NodeData.Action.EffectFile + 1;
              end;
            8:
              begin
                if NodeData.Action.EffectFile2 < 0 then
                  ItemIndex := 0
                else
                  ItemIndex := NodeData.Action.EffectFile2 + 1;
              end;
          end;
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  else
    Result := False;
  end;
end;

procedure TMonsterPropertyEditLink.ProcessMessage(var Message: TMessage);
begin
  FEdit.WindowProc(Message);
end;

procedure TMonsterPropertyEditLink.SetBounds(R: TRect);
var
  Dummy: Integer;
begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right);
  FEdit.BoundsRect := R;
end;

destructor TItemRulePropertyEditLink.Destroy;
begin
  FEdit.Free;
  inherited;
end;

procedure TItemRulePropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
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

procedure TItemRulePropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
begin
  case Key of
    VK_ESCAPE:
      begin
        FTree.CancelEditNode;
        Key := 0;
      end; // VK_ESCAPE
  end; // case
end;

function TItemRulePropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

function TItemRulePropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

function TItemRulePropertyEditLink.EndEdit: Boolean;
var
  ItemRule: PPDropItemRule;
  TempValue: Integer;
  TempStr: string;
  IsChanged: Boolean;
  TempDate, dt: TDateTime;
begin
  IsChanged := False;
  Result := True;
  ItemRule := FTree.GetNodeData(FNode);
  if ItemRule = nil then
    Exit;
  case FColumn of
    0:
      begin
        TempStr := (FEdit as TComboBox).Text;
        IsChanged := not SameText(ItemRule^.MapName, TempStr);
        if IsChanged then
          ItemRule^.MapName := TempStr;
      end;
    1:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := ItemRule^.ClearInterval <> TempValue;
        if IsChanged then
          ItemRule^.ClearInterval := TempValue;
      end;
    2:
      begin
        TempValue := (FEdit as TComboBox).ItemIndex;
        IsChanged := TempValue <> Integer(ItemRule^.IntervalType);
        if IsChanged then
          ItemRule^.IntervalType := TIntervalType(TempValue);
      end;
    3:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := ItemRule^.DropInterval <> TempValue;
        if IsChanged then
          ItemRule^.DropInterval := TempValue;
      end;
    4:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := ItemRule^.LimitCount <> TempValue;
        if IsChanged then
          ItemRule^.LimitCount := TempValue;
        if ItemRule^.DropedCount > ItemRule^.LimitCount then
          ItemRule^.DropedCount := ItemRule^.LimitCount;
      end;
    5:
      begin
        TempValue := (FEdit as TSpinEditEx).Value;
        IsChanged := ItemRule^.DropedCount <> TempValue;
        if IsChanged then
          ItemRule^.DropedCount := TempValue;
        if ItemRule^.DropedCount > ItemRule^.LimitCount then
          ItemRule^.DropedCount := ItemRule^.LimitCount;
      end;
    8:
      begin
        if ItemRule^.IntervalType = itDay then
          TempDate := ItemRule^.LastClearDate + ItemRule^.ClearInterval
        else if ItemRule^.IntervalType = itHour then
          TempDate := IncHour(ItemRule^.LastClearDate, ItemRule^.ClearInterval)
        else
          TempDate := IncMinute(ItemRule^.LastClearDate, ItemRule^.ClearInterval);
        dt := (FEdit as TDateTimePicker).DateTime;
        IsChanged := not SameValue(TempDate, dt, 0.0000001);
        if ItemRule^.IntervalType = itDay then
          ItemRule^.LastClearDate := dt - ItemRule^.ClearInterval
        else if ItemRule^.IntervalType = itHour then
          ItemRule^.LastClearDate := IncHour(dt, -ItemRule^.ClearInterval)
        else
          ItemRule^.LastClearDate := IncMinute(dt, -ItemRule^.ClearInterval);
      end;
  end;
  if FTree.CanFocus then
    FTree.SetFocus;
  if FEdit.Visible then
    FEdit.Visible := False;
  if IsChanged then
  begin
    if (FTree.Owner is TfrmMonsterConfig) then
    begin
      TfrmMonsterConfig(FTree.Owner).FCurrentLimitItem.Save;
    end;
  end;
end;

function TItemRulePropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

function TItemRulePropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  I: Integer;
  ItemRule: PPDropItemRule;
  Envir: TEnvirnoment;
  SL: TStringList;
  MapName: string;
  IntervalType: TIntervalType;
begin
  Result := True;
  FTree := Tree as TVirtualStringTree;
  FNode := Node;
  FColumn := Column;
  // determine what edit type actually is needed
  FEdit.Free;
  FEdit := nil;
  ItemRule := FTree.GetNodeData(Node);
  case FColumn of
    0:
      begin
        FEdit := TComboBox.Create(nil);
        with FEdit as TComboBox do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDown;
          // Items.Add('*');
          SL := TStringList.Create;
          try
            for I := 0 to g_MapManager.Count - 1 do
            begin
              Envir := g_MapManager.Items[I];
              if Envir.m_boMirror then
                Continue;
              if Envir.m_boFB then
                MapName := Envir.sMainMapName
              else
                MapName := Envir.sMapName;
              if SL.IndexOf(MapName) < 0 then
              begin
                SL.Add(MapName);
              end;
            end;
            SL.Sort;
            SL.Insert(0, '*');
            Items.Text := SL.Text;
          finally
            SL.Free;
          end;
          Text := ItemRule^.MapName;
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    1, 3, 4, 5:
      begin
        FEdit := TSpinEditEx.Create(nil);
        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          if FColumn = 1 then
          begin
            MinValue := 1;
            MaxValue := 60;
            Value := ItemRule^.ClearInterval;
          end
          else if FColumn = 3 then
          begin
            MinValue := 0;
            MaxValue := 0;
            Value := ItemRule^.DropInterval;
          end
          else if FColumn = 4 then
          begin
            MinValue := 0;
            MaxValue := 0;
            Value := ItemRule^.LimitCount;
          end
          else if FColumn = 5 then
          begin
            MinValue := 0;
            MaxValue := 0;
            Value := ItemRule^.DropedCount;
          end;
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    2:
      begin
        FEdit := TComboBox.Create(nil);
        with FEdit as TComboBox do
        begin
          Visible := False;
          Parent := Tree;
          Style := csDropDownList;
          for IntervalType := Low(IntervalType) to High(IntervalType) do
          begin
            Items.Add(TIntervalTypeNames[IntervalType]);
          end;
          ItemIndex := Integer(ItemRule^.IntervalType);
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    8:
      begin
        FEdit := TDateTimePicker.Create(nil);
        with FEdit as TDateTimePicker do
        begin
          Visible := False;
          Parent := Tree;
          begin
            Format := 'HH:mm:ss';
            Kind := dtkTime;
          end;
          if ItemRule^.IntervalType = itDay then
            DateTime := ItemRule^.LastClearDate + ItemRule^.ClearInterval
          else if ItemRule^.IntervalType = itHour then
            DateTime := IncHour(ItemRule^.LastClearDate, ItemRule^.ClearInterval)
          else
            DateTime := IncMinute(ItemRule^.LastClearDate, ItemRule^.ClearInterval);
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  else
    Result := False;
  end;
end;

procedure TItemRulePropertyEditLink.ProcessMessage(var Message: TMessage);
begin
  FEdit.WindowProc(Message);
end;

procedure TItemRulePropertyEditLink.SetBounds(R: TRect);
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

function ShowLeftRighHandMessage: Integer;
var
  Form: TForm;
  RadioGroup: TRadioGroup;
begin
  Form := TForm.Create(nil);
  try
    Form.BorderStyle := bsDialog;
    Form.Position := poMainFormCenter;
    Form.ClientWidth := 192;
    Form.ClientHeight := 92;
    RadioGroup := TRadioGroup.Create(Form);
    with RadioGroup do
    begin
      Parent := Form;
      Caption := '指定左右手';
      SetBounds(11, 8, 169, 41);
      Items.Add('左手');
      Items.Add('右手');
      Columns := 2;
      ItemIndex := LeftRightIndex;
    end;

    with TButton.Create(Form) do
    begin
      Parent := Form;
      Left := 105;
      Top := 59;
      Caption := '确定(&O)';
      ModalResult := mrOk;
      Default := True;
      Cancel := True;
    end;
    Form.ShowModal;
    LeftRightIndex := RadioGroup.ItemIndex;
    Result := LeftRightIndex;
  finally
    Form.Free;
  end;
end;

{ TfrmMonsterConfig }
procedure TfrmMonsterConfig.ModValue;
begin
  boModValued := True;
  ButtonGeneralSave.Enabled := True;
  ButtonMonsterConfigSave.Enabled := True;
end;

procedure TfrmMonsterConfig.uModValue;
begin
  boModValued := False;
  ButtonGeneralSave.Enabled := False;
  ButtonMonsterConfigSave.Enabled := False;
end;

procedure TfrmMonsterConfig.Open;
var
  I: Integer;
  Magic: pTMagic;
  Monster: pTMonInfo;
  Node: PVirtualNode;
  CustomMonsterConfig: TCustomMonsterConfig;
  ConfigNodeData: PMonsterConfigNodeData;
begin
  FIsMonsterChanged := False;
  boOpened := False;
  uModValue();
  SelMonsterConfig := nil;
  LoadStdItems();

  if g_MultiThreadRun then
    UserEngine.m_MagicList.LockR(14);
  try
    for I := 0 to UserEngine.m_MagicList.Count - 1 do
    begin
      Magic := UserEngine.m_MagicList.Items[I];
      ListBoxMagicList.Items.AddObject(Magic.sMagicName, TObject(Magic));
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.m_MagicList.UnLockR;
  end;

  if g_MultiThreadRun then
    UserEngine.MonsterList.LockR(7);

  try
    for I := 0 to UserEngine.MonsterList.Count - 1 do
    begin
      Monster := UserEngine.MonsterList.Items[I];
      if Monster.btRace = RC_PLAYMOSTER then
      begin
        ListBoxMonsterList.Items.AddObject(Monster.sName, TObject(Monster));
      end;
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.MonsterList.UnLockR;
  end;

  // 自定义怪物 chongchong 2014-07-09
  if g_MultiThreadRun then
    UserEngine.m_CustomMonsterList.LockR(4);
  try
    for I := 0 to UserEngine.m_CustomMonsterList.Count - 1 do
    begin
      CustomMonsterConfig := UserEngine.m_CustomMonsterList.Items[I];
      Node := vstCustomMonster.AddChild(nil);
      ConfigNodeData := vstCustomMonster.GetNodeData(Node);
      ConfigNodeData.Config := CustomMonsterConfig;
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomMonsterList.UnLockR;
  end;

  EditMonsterWarrorAttackTime.Value := g_Config.dwMonsterWarrorAttackTime;
  EditMonsterWizardAttackTime.Value := g_Config.dwMonsterWizardAttackTime;
  EditMonsterTaoistAttackTime.Value := g_Config.dwMonsterTaoistAttackTime;
  EditMonsterWarrorWalkTime.Value := g_Config.dwMonsterWarrorWalkTime;
  EditMonsterWizardWalkTime.Value := g_Config.dwMonsterWizardWalkTime;
  EditMonsterTaoistWalkTime.Value := g_Config.dwMonsterTaoistWalkTime;
  RadioGroupMonsterNeedMagicItem.ItemIndex := g_Config.nMonsterNeedMagicItem;
  chkSendCustomMonsterConfig.Checked := g_Config.boSendCustomMonsterConfig;
  chkDamageLimitation.Checked := g_Config.boDamageLimitation;
  RefGeneralInfo();
  boOpened := True;
  PageControl1.ActivePageIndex := 0;
  pgcMain.ActivePageIndex := 0;
  if g_nKey_DropLimitExt = 1 then
  begin
    for I := 0 to g_DropLimitMgr.Count - 1 do
    begin
      vstDropLimitItems.AddChild(nil, g_DropLimitMgr.Items[I]);
    end;
  end;

  ShowModal;
end;

procedure TfrmMonsterConfig.RefGeneralInfo;
begin
  seMonButchDelayClearTime.Value := g_Config.dwMonButchDelayClearTime;
  chkNoHumanClearMon.Checked := g_Config.boNoHumanClearMon;
  seNoHumanClearMonTime.Value := g_Config.dwNoHumanClearMonTime;
  cbbMonsterShowLevel.ItemIndex := g_Config.btMonsterShowLevel;
  edtMonsterShowFormat.Text := g_Config.sMonsterShowFormat;
  edtMonsterShowFormat.Enabled := g_Config.btMonsterShowLevel > 0;
  edtMagStruckMonLevel.Value := g_Config.nMagStruckMonLevel;
  edtMagStruckMonDecTime.Value := g_Config.nMagStruckMonDecTime;
  edtMagStruckMonDecRandom.Value := g_Config.nMagStruckMonDecRandom;
  seMonStruckFrameDelayTime.Value := g_Config.btMonStruckFrameDelayTime;
  chkEnabledMaxMapItemCount.Checked := g_Config.boEnabledMaxMapItemCount;
  seMaxMapItemCount.Value := g_Config.nMaxMapItemCount;
  chkNotDropOverlapItemAll.Checked := g_Config.boNotDropOverlapItemAll;
  // 爆物品范围 piaoyun 2013-09-12
  seScatterItemRange.Value := g_Config.nScatterItemRange;
  seMonOneDropGoldCount.Value := g_Config.nMonOneDropGoldCount;
  chkDropGoldToPlayBag.Checked := g_Config.boDropGoldToPlayBag;
  seElfWarriorMonsterDownDelay.Value := g_Config.nElfWarriorMonsterDownDelay;
end;

procedure TfrmMonsterConfig.ButtonGeneralSaveClick(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'MonButchDelayClearTime', g_Config.dwMonButchDelayClearTime);
  Config.WriteBool('Setup', 'NoHumanClearMon', g_Config.boNoHumanClearMon);
  Config.WriteInteger('Setup', 'NoHumanClearMonTime', g_Config.dwNoHumanClearMonTime);
  Config.WriteInteger('Setup', 'MonsterShowLevel', g_Config.btMonsterShowLevel);
  Config.WriteString('Setup', 'MonsterShowFormat', g_Config.sMonsterShowFormat);
  Config.WriteInteger('Setup', 'MagStruckMonLevel', g_Config.nMagStruckMonLevel);
  Config.WriteInteger('Setup', 'MagStruckMonDecTime', g_Config.nMagStruckMonDecTime);
  Config.WriteInteger('Setup', 'MagStruckMonDecRandom', g_Config.nMagStruckMonDecRandom);
  Config.WriteInteger('Setup', 'ElfWarriorMonsterDownDelay', g_Config.nElfWarriorMonsterDownDelay);
  Config.WriteInteger('Setup', 'MonStruckFrameDelayTime', g_Config.btMonStruckFrameDelayTime);
  // 爆物品范围 piaoyun 2013-09-12
  Config.WriteInteger('Setup', 'ScatterItemRange', g_Config.nScatterItemRange);
  Config.WriteBool('Setup', 'EnabledMaxMapItemCount', g_Config.boEnabledMaxMapItemCount);
  Config.WriteInteger('Setup', 'MaxMapItemCount', g_Config.nMaxMapItemCount);
  Config.WriteBool('Setup', 'NotDropOverlapItemAll', g_Config.boNotDropOverlapItemAll);
  Config.WriteInteger('Setup', 'MonOneDropGoldCount', g_Config.nMonOneDropGoldCount);
  Config.WriteBool('Setup', 'DropGoldToPlayBag', g_Config.boDropGoldToPlayBag);
  UserEngine.SendServerConfig();
  uModValue();
end;

procedure TfrmMonsterConfig.lstItemListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  I: Integer;
  sItemName: string;
begin
  case Key of
    Word('F'):
      begin
        if ssCtrl in Shift then
        begin
          Key := 0;
          sItemName := '';
          if not InputQuery('物品查找', '输入物品名称:', sItemName) then
            Exit;
          if sItemName = '' then
            Exit;
          for I := 0 to TListBox(Sender).Items.Count - 1 do
          begin
            if TListBox(Sender).Items.Strings[I] = sItemName then
            begin
              TListBox(Sender).ItemIndex := I;
              Break;
            end;
          end;
        end;
      end;
  end;
end;

procedure TfrmMonsterConfig.ListBoxMagicListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  I: Integer;
  sName: string;
begin
  case Key of
    Word('F'):
      begin
        if ssCtrl in Shift then
        begin
          Key := 0;
          sName := '';
          if not InputQuery('魔法查找', '输入魔法名称:', sName) then
            Exit;
          if sName = '' then
            Exit;
          for I := 0 to TListBox(Sender).Items.Count - 1 do
          begin
            if TListBox(Sender).Items.Strings[I] = sName then
            begin
              TListBox(Sender).ItemIndex := I;
              Break;
            end;
          end;
        end;
      end;
  end;
end;

procedure TfrmMonsterConfig.ListBoxMonsterListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  I: Integer;
  sName: string;
begin
  case Key of
    Word('F'):
      begin
        if ssCtrl in Shift then
        begin
          Key := 0;
          sName := '';
          if not InputQuery('人形怪查找', '输入人形怪名称:', sName) then
            Exit;
          if sName = '' then
            Exit;
          for I := 0 to TListBox(Sender).Items.Count - 1 do
          begin
            if TListBox(Sender).Items.Strings[I] = sName then
            begin
              TListBox(Sender).ItemIndex := I;
              Break;
            end;
          end;
        end;
      end;
  end;
end;

procedure TfrmMonsterConfig.ListBoxMonsterBagItemListDragOver(Sender, Source: TObject; X, Y: Integer; State: TDragState;
  var Accept: Boolean);
begin
  Accept := True;
end;

procedure TfrmMonsterConfig.ListBoxMonsterMagicListDragDrop(Sender, Source: TObject; X, Y: Integer);
var
  I, II: Integer;
  boFind: Boolean;
begin
  if Source = ListBoxMagicList then
  begin
    if SelMonsterConfig <> nil then
    begin
      for I := 0 to ListBoxMagicList.Items.Count - 1 do
      begin
        if ListBoxMagicList.Selected[I] then
        begin
          boFind := False;
          for II := 0 to ListBoxMonsterMagicList.Count - 1 do
          begin
            if CompareText(ListBoxMagicList.Items.Strings[I], ListBoxMonsterMagicList.Items.Strings[II]) = 0 then
            begin
              ListBoxMonsterMagicList.ItemIndex := II;
              boFind := True;
              Break;
            end;
          end;
          if not boFind then
          begin
            ButtonMonUseItemsChange.Enabled := True;
            ListBoxMonsterMagicList.Items.AddObject(ListBoxMagicList.Items.Strings[I], nil);
          end;
        end;
      end;
    end;
  end;
end;

procedure TfrmMonsterConfig.GroupBoxMonsterUseItemDockOver(Sender: TObject; Source: TDragDockObject; X, Y: Integer;
  State: TDragState; var Accept: Boolean);
begin
  Accept := True;
end;

procedure TfrmMonsterConfig.GroupBoxMonsterUseItemDragDrop(Sender, Source: TObject; X, Y: Integer);
var
  Where: Integer;
  Item: pTStdItem;
  Edit: TEdit;
begin
  if Source = lstItemList then
  begin
    if SelMonsterConfig <> nil then
    begin
      Edit := TEdit(Sender);
      Item := pTStdItem(lstItemList.Items.Objects[lstItemList.ItemIndex]);
      Where := GetTakeOnPosition(Item.StdMode);
      case Where of
        U_RINGL:
          if (Edit.Tag = U_RINGL) or (Edit.Tag = U_RINGR) then
            Edit.Text := Item.Name;
        U_ARMRINGR:
          if (Edit.Tag = U_ARMRINGR) or (Edit.Tag = U_ARMRINGL) then
            Edit.Text := Item.Name;
      else
        if (Edit.Tag = Where) then
          Edit.Text := Item.Name;
      end;
    end;
  end;
end;

procedure TfrmMonsterConfig.MenuItem_ShowAllClick(Sender: TObject);
var
  I: Integer;
  MenuItem: TMenuItem;
begin
  for I := 0 to PopupMenuItem.Items.Count - 1 do
  begin
    MenuItem := PopupMenuItem.Items[I];
    if MenuItem <> Sender then
    begin
      MenuItem.Checked := False;
    end;
  end;
  MenuItem := TMenuItem(Sender);
  if not MenuItem.Checked then
  begin
    MenuItem.Checked := True;
    LoadStdItems(MenuItem.Tag);
  end;
end;

procedure TfrmMonsterConfig.ListBoxMonsterListClick(Sender: TObject);
var
  I: Integer;
  Edit: TEdit;
begin
  SelMonsterConfig := GetPlayMonsterConfig(ListBoxMonsterList.Items.Strings[ListBoxMonsterList.ItemIndex]);
  if SelMonsterConfig <> nil then
  begin
    ListBoxMonsterMagicList.Clear;
    GroupBoxMonsterConfig.Enabled := True;
    GroupBoxMonsterConfig.Caption := SelMonsterConfig.Name;
    cbbJob.ItemIndex := SelMonsterConfig.Job;
    cbbGender.ItemIndex := SelMonsterConfig.Gender;
    seEditHair.Value := SelMonsterConfig.Hair;
    chkDieDropUseItem.Checked := SelMonsterConfig.boDieDropUseItem;
    seDieDropUseItemRate.Value := SelMonsterConfig.nDieDropUseItemRate;
    chkDieDropBagItem.Checked := SelMonsterConfig.boDieDropBagItem;
    // CheckBoxButchItem.Checked := SelMonsterConfig.boButchItem;
    chkButchUseItem.Checked := SelMonsterConfig.boButchUseItem; // 挖取身上物品
    seButchUseItemRate.Value := SelMonsterConfig.nButchUseItemRate; // 挖取身上物品几率
    chkButchListItem.Checked := SelMonsterConfig.boButchListItem; // 挖取列表物品
    chkButchItemTrigger.Checked := SelMonsterConfig.boButchItemTrigger; // 挖取触发
    cbbButchChargeMode.ItemIndex := SelMonsterConfig.nButchChargeMode; // 挖取收费模式
    seButchChargeCount.Value := SelMonsterConfig.nButchChargeCount; // 挖取收费值
    chkOnlyButchItemDelGold.Checked := SelMonsterConfig.boOnlyButchItemDelGold;
    chkProtectMode.Checked := SelMonsterConfig.boProtectMode;
    EditRestrictMonsterRange.Value := SelMonsterConfig.nProtectRange;
    CheckBoxNonUseSpellPoint.Checked := SelMonsterConfig.NonUseSpellPoint;
    chkRunWithAcctack.Checked := SelMonsterConfig.boRunWithAttack;
    seRunWithAcctackRate.Value := SelMonsterConfig.nRunWithAttackRate;
    chkNoAttackMode.Checked := SelMonsterConfig.boNoAttackMode;
    cbbJobChange(Sender);
    for I := 0 to GroupBoxMonsterUseItem.ControlCount - 1 do
    begin
      if GroupBoxMonsterUseItem.Controls[I] is TEdit then
      begin
        Edit := TEdit(GroupBoxMonsterUseItem.Controls[I]);
        Edit.Text := SelMonsterConfig.UseItems[Edit.Tag];
      end;
    end;
    ListBoxMonsterMagicList.Clear;
    ListBoxMonsterMagicList.Items.AddStrings(SelMonsterConfig.Magics);
    ButtonMonUseItemsChange.Enabled := False;
    ButtonMonUseItemsSave.Enabled := False;
  end
  else
    GroupBoxMonsterConfig.Enabled := False;
end;

procedure TfrmMonsterConfig.EditDRESSNAMEChange(Sender: TObject);
begin
  if SelMonsterConfig <> nil then
    ButtonMonUseItemsChange.Enabled := True;
end;

procedure TfrmMonsterConfig.ButtonMonUseItemsSaveClick(Sender: TObject);
begin
  ButtonMonUseItemsSave.Enabled := False;
  SavePlayMonsterConfigList();
end;

procedure TfrmMonsterConfig.ButtonMonUseItemsChangeClick(Sender: TObject);
var
  I: Integer;
  Edit: TEdit;
begin
  ButtonMonUseItemsChange.Enabled := False;
  if SelMonsterConfig <> nil then
  begin
    SelMonsterConfig.Job := cbbJob.ItemIndex;
    SelMonsterConfig.Gender := cbbGender.ItemIndex;
    SelMonsterConfig.Hair := seEditHair.Value;
    SelMonsterConfig.boDieDropUseItem := chkDieDropUseItem.Checked;
    SelMonsterConfig.nDieDropUseItemRate := seDieDropUseItemRate.Value;
    SelMonsterConfig.boDieDropBagItem := chkDieDropBagItem.Checked;
    // SelMonsterConfig.boButchItem := CheckBoxButchItem.Checked;
    SelMonsterConfig.boButchUseItem := chkButchUseItem.Checked; // 挖取身上物品
    SelMonsterConfig.nButchUseItemRate := seButchUseItemRate.Value; // 挖取身上物品几率
    SelMonsterConfig.boButchListItem := chkButchListItem.Checked; // 挖取列表物品
    SelMonsterConfig.boButchItemTrigger := chkButchItemTrigger.Checked; // 挖取触发
    SelMonsterConfig.nButchChargeMode := cbbButchChargeMode.ItemIndex; // 挖取收费模式
    SelMonsterConfig.nButchChargeCount := seButchChargeCount.Value; // 挖取收费值
    SelMonsterConfig.boOnlyButchItemDelGold := chkOnlyButchItemDelGold.Checked;
    SelMonsterConfig.boProtectMode := chkProtectMode.Checked;
    SelMonsterConfig.nProtectRange := EditRestrictMonsterRange.Value;
    SelMonsterConfig.NonUseSpellPoint := CheckBoxNonUseSpellPoint.Checked;
    SelMonsterConfig.boRunWithAttack := chkRunWithAcctack.Checked;
    SelMonsterConfig.nRunWithAttackRate := seRunWithAcctackRate.Value;
    SelMonsterConfig.boNoAttackMode := chkNoAttackMode.Checked;

    for I := 0 to GroupBoxMonsterUseItem.ControlCount - 1 do
    begin
      if GroupBoxMonsterUseItem.Controls[I] is TEdit then
      begin
        Edit := TEdit(GroupBoxMonsterUseItem.Controls[I]);
        if Edit.Tag in [Low(THumanUseItems) .. High(THumanUseItems)] then
          SelMonsterConfig.UseItems[Edit.Tag] := Edit.Text;
      end;
    end;
    SelMonsterConfig.Magics.Clear;
    SelMonsterConfig.Magics.AddStrings(ListBoxMonsterMagicList.Items);
    SelMonsterConfig.IsChanged := True;
    ButtonMonUseItemsSave.Enabled := True;
  end;
end;

procedure TfrmMonsterConfig.ListBoxMonsterMagicListDblClick(Sender: TObject);
begin
  if SelMonsterConfig <> nil then
  begin
    ListBoxMonsterMagicList.DeleteSelected;
    ButtonMonUseItemsChange.Enabled := True;
  end;
end;

procedure TfrmMonsterConfig.cbbJobChange(Sender: TObject);
var
  I: Integer;
  Magic: pTMagic;
begin
  if SelMonsterConfig <> nil then
  begin
    ListBoxMagicList.Clear;
    EnterCriticalSection(ProcessHumanCriticalSection);
    try
      for I := 0 to UserEngine.m_MagicList.Count - 1 do
      begin
        Magic := UserEngine.m_MagicList.Items[I];
        if (Magic.btJob = 99) or (Magic.btJob = cbbJob.ItemIndex) then
          ListBoxMagicList.Items.Add(Magic.sMagicName);
      end;
    finally
      LeaveCriticalSection(ProcessHumanCriticalSection);
    end;
    ButtonMonUseItemsChange.Enabled := True;
  end;
end;

procedure TfrmMonsterConfig.ListBoxMagicListDblClick(Sender: TObject);
begin
  ListBoxMonsterMagicListDragDrop(Sender, ListBoxMagicList, 0, 0);
end;

procedure TfrmMonsterConfig.EditMonsterWarrorAttackTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMonsterWarrorAttackTime := EditMonsterWarrorAttackTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.EditMonsterWizardAttackTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMonsterWizardAttackTime := EditMonsterWizardAttackTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.EditMonsterTaoistAttackTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMonsterTaoistAttackTime := EditMonsterTaoistAttackTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.EditMonsterWarrorWalkTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMonsterWarrorWalkTime := EditMonsterWarrorWalkTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.EditMonsterWizardWalkTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMonsterWizardWalkTime := EditMonsterWizardWalkTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.EditMonsterTaoistWalkTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMonsterTaoistWalkTime := EditMonsterTaoistWalkTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.RadioGroupMonsterNeedMagicItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonsterNeedMagicItem := RadioGroupMonsterNeedMagicItem.ItemIndex;
  ModValue();
end;

procedure TfrmMonsterConfig.ButtonMonsterConfigSaveClick(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'MonsterWarrorAttackTime', g_Config.dwMonsterWarrorAttackTime);
  Config.WriteInteger('Setup', 'MonsterWizardAttackTime', g_Config.dwMonsterWizardAttackTime);
  Config.WriteInteger('Setup', 'MonsterTaoistAttackTime', g_Config.dwMonsterTaoistAttackTime);
  Config.WriteInteger('Setup', 'MonsterWarrorWalkTime', g_Config.dwMonsterWarrorWalkTime);
  Config.WriteInteger('Setup', 'MonsterWizardWalkTime', g_Config.dwMonsterWizardWalkTime);
  Config.WriteInteger('Setup', 'MonsterTaoistWalkTime', g_Config.dwMonsterTaoistWalkTime);
  Config.WriteInteger('Setup', 'MonsterNeedMagicItem', g_Config.nMonsterNeedMagicItem);
  Config.WriteBool('Setup', 'DamageLimitation', g_Config.boDamageLimitation);
  uModValue();
end;

procedure TfrmMonsterConfig.ListBoxMonsterMagicListClick(Sender: TObject);
var
  Level: Integer;
begin
  if ListBoxMonsterMagicList.ItemIndex >= 0 then
  begin
    Level := Integer(ListBoxMonsterMagicList.Items.Objects[ListBoxMonsterMagicList.ItemIndex]);
    EditMagicLevel.Value := LoByte(Level);
    EditMagicNewLevel.Value := HiByte(Level);
  end;
end;

procedure TfrmMonsterConfig.EditMagicLevelChange(Sender: TObject);
var
  Level: Integer;
begin
  if ListBoxMonsterMagicList.ItemIndex >= 0 then
  begin
    Level := MakeWord(EditMagicLevel.Value, EditMagicNewLevel.Value);
    ListBoxMonsterMagicList.Items.Objects[ListBoxMonsterMagicList.ItemIndex] := TObject(Level);
  end;

  if SelMonsterConfig <> nil then
    ButtonMonUseItemsChange.Enabled := True;
end;

procedure TfrmMonsterConfig.seMonButchDelayClearTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMonButchDelayClearTime := seMonButchDelayClearTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.chkNoHumanClearMonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNoHumanClearMon := chkNoHumanClearMon.Checked;
  ModValue();
end;

procedure TfrmMonsterConfig.seNoHumanClearMonTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwNoHumanClearMonTime := seNoHumanClearMonTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.LoadStdItems(Fileter: Integer = -1);
var
  I, II: Integer;
  boFind: Boolean;
  StdItem: pTStdItem;
begin
  lstItemList.Clear;
  if g_MultiThreadRun then
    UserEngine.StdItemList.LockR(13);
  try
    for I := 0 to UserEngine.StdItemList.Count - 1 do
    begin
      StdItem := UserEngine.StdItemList.Items[I];
      if Fileter < 0 then
      begin
        lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
        Continue;
      end;
      case Fileter of
        U_DRESS:
          begin
            if StdItem.StdMode in [10, 11] then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_WEAPON:
          begin
            if (StdItem.StdMode = 5) or (StdItem.StdMode = 6) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_RIGHTHAND:
          begin
            if (StdItem.StdMode = 29) or (StdItem.StdMode = 30) or (StdItem.StdMode = 28) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_NECKLACE:
          begin
            if (StdItem.StdMode = 19) or (StdItem.StdMode = 20) or (StdItem.StdMode = 21) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_HELMET:
          begin
            if StdItem.StdMode = 15 then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_ARMRINGR, U_ARMRINGL:
          begin
            if (StdItem.StdMode = 24) { or (StdItem.StdMode = 25) } or (StdItem.StdMode = 26) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_RINGL, U_RINGR:
          begin
            if (StdItem.StdMode = 22) or (StdItem.StdMode = 23) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_BUJUK:
          begin
            if (StdItem.StdMode = 25) or (StdItem.StdMode = 51) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_BELT:
          begin
            if (StdItem.StdMode = 54) or (StdItem.StdMode = 64) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_BOOTS:
          begin
            if (StdItem.StdMode = 52) or (StdItem.StdMode = 62) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_CHARM:
          begin
            if (StdItem.StdMode = 53) or (StdItem.StdMode = 63) or (StdItem.StdMode = 7) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_HAT:
          begin
            if (StdItem.StdMode = 16) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_DRUM:
          begin
            if (StdItem.StdMode = 65) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        U_SHIELD:
          begin
            if (StdItem.StdMode = 12) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        31:
          begin
            if (StdItem.StdMode = 31) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        100:
          begin
            if (StdItem.StdMode in [0 .. 3]) or (StdItem.StdMode = 25) then
              lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
      else
        begin
          boFind := False;
          for II := Low(THumanUseItems) to High(THumanUseItems) do
          begin
            if CheckUserItems(II, StdItem) then
            begin
              boFind := True;
              Break;
            end;
          end;
          if not boFind then
            lstItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
        end;
      end;
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.StdItemList.UnLockR;
  end;
end;

procedure TfrmMonsterConfig.lstItemListDblClick(Sender: TObject);
var
  Where: Integer;
  Item: pTStdItem;
  Edit: TEdit;
begin
  if SelMonsterConfig = nil then
    Exit;
  Item := pTStdItem(lstItemList.Items.Objects[lstItemList.ItemIndex]);
  Where := GetTakeOnPosition(Item.StdMode);
  case Where of
    U_DRESS:
      Edit := EditDRESSNAME; // 衣服
    U_WEAPON:
      Edit := EditWEAPONNAME; // 武器
    U_RIGHTHAND:
      Edit := EditRIGHTHANDNAME; // 照明物品
    U_NECKLACE:
      Edit := EditNECKLACENAME; // 项链
    U_HELMET:
      Edit := EditHELMETNAME; // 头盔
    U_ARMRINGL, // 左手镯
    U_ARMRINGR: // 右手镯
      begin
        if ShowLeftRighHandMessage = 0 then
          Edit := EditARMRINGLNAME
        else
          Edit := EditARMRINGRNAME;
      end;
    U_RINGL, // 左戒指
    U_RINGR: // 右戒指
      begin
        if ShowLeftRighHandMessage = 0 then
          Edit := EditRINGLNAME
        else
          Edit := EditRINGRNAME;
      end;
    U_BUJUK:
      Edit := EditBUJUKNAME; // 符
    U_BELT:
      Edit := EditBELTNAME; // 腰带
    U_BOOTS:
      Edit := EditBOOTSNAME; // 鞋
    U_CHARM:
      Edit := EditCHARMNAME; // 宝石
    U_HAT:
      Edit := EditHATNAME; // 斗笠
    U_DRUM:
      Edit := EditDRUMNAME;
    U_SHIELD:
      Edit := edtShield;
    // U_HORSE: // 马
  else
    Edit := nil;
  end;
  if Edit <> nil then
  begin
    Edit.Text := Item.Name;
    tmrFlash.Enabled := False;
    if FlashEdit <> nil then
      FlashEdit.Color := clWindow;

    FlashEdit := Edit;
    tmrFlash.Tag := 0;
    tmrFlash.Enabled := True;
  end;
end;

procedure TfrmMonsterConfig.tmrFlashTimer(Sender: TObject);
begin
  if FlashEdit = nil then
    Exit;
  if FlashEdit.Color = clWindow then
    FlashEdit.Color := clYellow
  else
    FlashEdit.Color := clWindow;

  tmrFlash.Tag := tmrFlash.Tag + 1;
  if tmrFlash.Tag >= 10 then
  begin
    tmrFlash.Tag := 0;
    FlashEdit.Color := clWindow;
    FlashEdit := nil;
    tmrFlash.Enabled := False;
  end;
end;

procedure TfrmMonsterConfig.edtMonsterShowFormatChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sMonsterShowFormat := edtMonsterShowFormat.Text;
  ModValue();
end;

procedure TfrmMonsterConfig.FormCreate(Sender: TObject);
var
  I: Integer;
  DrawMode: TCustomDrawMode;
  DirCount: TCustomDirCount;
  DirCalcType: TCustomDirCalcType;
  // PlayMode: TMonsterPlayMode;
  DrawOrder: TCustomDrawOrder;
  DrawOrder2: TMonsterDrawOrder2;
  MonsterType: TMonsterType;
  MoveOption: TMoveOption;
  OperateMode: TCustomOperateMode;
  AttackMode: TCustomAttackMode;
  AttackTarget: TCustomAttackTarget;
  AttackPowerCalc: TCustomAttackPowerCalc;
  StdItem: pTStdItem;
begin
  cbbClientAttackConfig.Items.Clear;
  for I := Low(AttackConfigNames) to High(AttackConfigNames) do
    cbbClientAttackConfig.Items.Add(AttackConfigNames[I]);

  cbbClientFlyFile.Items.Clear;
  cbbClientFlyFile.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientFlyFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientFlyDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientFlyDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientFlyDirCount.Items.Clear;
  for DirCount := Low(TCustomDirCount) to High(TCustomDirCount) do
    cbbClientFlyDirCount.Items.Add(CustomDirNames[DirCount]);

  cbbClientFlyEffFile.Items.Clear;
  cbbClientFlyEffFile.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientFlyEffFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientFlyEffDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientFlyEffDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientSelfFile.Clear;
  cbbClientSelfFile.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientSelfFile.Items.Add(g_EffectImageList.Strings[I]);

  {
    cbbClientSelfPlayMode.Clear;
    for PlayMode := Low(TMonsterPlayMode) to High(TMonsterPlayMode) do
    cbbClientSelfPlayMode.Items.Add(MonsterPlayModeNames[PlayMode]);
  }
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
  cbbClientSelfKeepFile.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientSelfKeepFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientSelfKeepDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientSelfKeepDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientSelfKeepDrawOrder.Items.Clear;
  for DrawOrder := Low(TCustomDrawOrder) to High(TCustomDrawOrder) do
    cbbClientSelfKeepDrawOrder.Items.Add(CustomDrawOrderNames[DrawOrder]);

  cbbClientSelfKeepDrawMode2.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientSelfKeepDrawMode2.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientExplosionDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientExplosionDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientExplosionDrawMode2.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientExplosionDrawMode2.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientTargetDrawMode.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientTargetDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientTargetDrawMode2.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
    cbbClientTargetDrawMode2.Items.Add(CustomDrawModeNames[DrawMode]);

  cbbClientExplosionFile.Clear;
  cbbClientExplosionFile.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientExplosionFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientTargetFile.Clear;
  cbbClientTargetFile.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbClientTargetFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbBatchAction.Clear;
  cbbBatchAction.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbBatchAction.Items.Add(g_EffectImageList.Strings[I]);

  cbbBatchEffect.Clear;
  cbbBatchEffect.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbBatchEffect.Items.Add(g_EffectImageList.Strings[I]);

  cbbClientDrawMode.Items.Clear;
  cbbClientDrawMode2.Items.Clear;
  for DrawMode := Low(TCustomDrawMode) to High(TCustomDrawMode) do
  begin
    cbbClientDrawMode.Items.Add(CustomDrawModeNames[DrawMode]);
    cbbClientDrawMode2.Items.Add(CustomDrawModeNames[DrawMode]);
  end;

  cbbClientDrawOrder2.Items.Clear;
  for DrawOrder2 := Low(TMonsterDrawOrder2) to High(TMonsterDrawOrder2) do
    cbbClientDrawOrder2.Items.Add(MonsterDrawOrder2Names[DrawOrder2]);

  cbbHPFile.Clear;
  cbbHPFile.Items.Add('根据Appr计算');
  for I := 0 to g_EffectImageList.Count - 1 do
    cbbHPFile.Items.Add(g_EffectImageList.Strings[I]);

  cbbServerAttackConfig.Items.Clear;
  for I := Low(AttackConfigNames) to High(AttackConfigNames) do
    cbbServerAttackConfig.Items.Add(AttackConfigNames[I]);

  cbbMonsterType.Items.Clear;
  for MonsterType := Low(TMonsterType) to High(TMonsterType) do
    cbbMonsterType.Items.Add(MonsterTypeNames[MonsterType]);

  cbbMoveOption.Items.Clear;
  for MoveOption := Low(TMoveOption) to High(TMoveOption) do
    cbbMoveOption.Items.Add(MoveOptionNames[MoveOption]);

  cbbOperateMode.Items.Clear;
  for OperateMode := Low(TCustomOperateMode) to High(TCustomOperateMode) do
    cbbOperateMode.Items.Add(CustomOperateModeNames[OperateMode]);

  cbbAttackMode.Items.Clear;
  for AttackMode := Low(TCustomAttackMode) to High(TCustomAttackMode) do
    cbbAttackMode.Items.Add(CustomAttackModeNames[AttackMode]);

  cbbAttackTarget.Items.Clear;
  for AttackTarget := Low(TCustomAttackTarget) to High(TCustomAttackTarget) do
    cbbAttackTarget.Items.Add(CustomAttackTargetNames[AttackTarget]);

  cbbAttackPowerCalc.Items.Clear;
  for AttackPowerCalc := Low(TCustomAttackPowerCalc) to High(TCustomAttackPowerCalc) do
    cbbAttackPowerCalc.Items.Add(CustomAttackPowerCalcNames[AttackPowerCalc]);

  SetControlEnabled(pgcMain, False);
  FCurrentMonsterCustomConfig := nil;
  FCurrentMonsterClientConfig := nil;
  FCurrentMonsterServerConfig := nil;

  if g_nKey_DropLimitExt = 1 then
  begin
    lstAllItems.Clear;
    if g_MultiThreadRun then
      UserEngine.StdItemList.LockR(112);

    try
      for I := 0 to UserEngine.StdItemList.Count - 1 do
      begin
        StdItem := UserEngine.StdItemList.Items[I];
        lstAllItems.Items.AddObject(StdItem.Name, TObject(StdItem));
      end;
    finally
      if g_MultiThreadRun then
        UserEngine.StdItemList.UnLockR;
    end;

    FCurrentLimitItem := nil;
    FCurrentItemRule := nil;
    RefreshDropLimitItemButtons;
  end
  else
    tsDropItem.TabVisible := False;
end;

procedure TfrmMonsterConfig.vstActionNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
begin
  if Assigned(HitInfo.HitNode) and (HitInfo.HitColumn >= 0) then
    PostMessage(Self.Handle, WM_STARTEDITING_MONSTER, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
end;

procedure TfrmMonsterConfig.vstActionGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  NodeData: PMonsterNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    case Column of
      0:
        CellText := MonsterClientActionNames[NodeData.Action.ActionType];
      1:
        begin
          if (NodeData.Action.ActionFile < 0) or (NodeData.Action.ActionFile >= g_EffectImageList.Count) then
            CellText := '根据Appr计算'
          else
            CellText := g_EffectImageList.Strings[NodeData.Action.ActionFile];
        end;
      2:
        CellText := IntToStr(NodeData.Action.StartIndex);
      3:
        CellText := IntToStr(NodeData.Action.PlayCount);
      4:
        CellText := IntToStr(NodeData.Action.EmptyCount);
      5:
        CellText := IntToStr(NodeData.Action.PlayTime);
      6:
        begin
          if (NodeData.Action.EffectFile < 0) or (NodeData.Action.EffectFile >= g_EffectImageList.Count) then
            CellText := '根据Appr计算'
          else
            CellText := g_EffectImageList.Strings[NodeData.Action.EffectFile];
        end;
      7:
        CellText := IntToStr(NodeData.Action.EffectIndex);
      8:
        begin
          if (NodeData.Action.EffectFile2 < 0) or (NodeData.Action.EffectFile2 >= g_EffectImageList.Count) then
            CellText := '根据Appr计算'
          else
            CellText := g_EffectImageList.Strings[NodeData.Action.EffectFile2];
        end;
      9:
        CellText := IntToStr(NodeData.Action.EffectIndex2);
    end;
  end;
end;

procedure TfrmMonsterConfig.vstActionGetHint(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  var LineBreakStyle: TVTTooltipLineBreakStyle; var HintText: string);
var
  NodeData: PMonsterNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    if Column = 7 then
      HintText := '特效开始图片为-1表示不使用特效'
    else if (Column = 0) and (NodeData.Action.ActionType = matDie) then
      HintText := '从站立到躺下的动作';
  end;
end;

procedure TfrmMonsterConfig.vstActionCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  out EditLink: IVTEditLink);
begin
  EditLink := TMonsterPropertyEditLink.Create;
end;

procedure TfrmMonsterConfig.vstActionEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  var Allowed: Boolean);
begin
  Allowed := Node <> nil;
end;

procedure TfrmMonsterConfig.WMStartEditingMonster(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WPARAM);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstAction.EditNode(Node, Message.LParam);
end;

type
  THackTree = class(TVirtualStringTree);

procedure TfrmMonsterConfig.vstActionDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
begin
  TargetCanvas.Font.Color := Sender.Font.Color;
end;

procedure TfrmMonsterConfig.cbbClientAttackConfigChange(Sender: TObject);
var
  Index: Integer;
  OldChanged, OldIsConfigCanSave: Boolean;
begin
  FCurrentMonsterClientConfig := nil;
  if FCurrentMonsterCustomConfig = nil then
    Exit;

  OldIsConfigCanSave := FIsMonsterChanged;
  Index := cbbClientAttackConfig.ItemIndex;
  if (Index >= Low(FCurrentMonsterCustomConfig.ClientAttackConfigs)) and
    (Index <= High(FCurrentMonsterCustomConfig.ClientAttackConfigs)) then
  begin
    OldChanged := FCurrentMonsterCustomConfig.IsChanged;
    FCurrentMonsterClientConfig := @FCurrentMonsterCustomConfig.ClientAttackConfigs[Index];
    grpClientAttackConfigs.Caption := AttackConfigNames[Index] + '的攻击效果配置';
    cbbClientFlyFile.ItemIndex := FCurrentMonsterClientConfig.Fly_File + 1;
    seClientFlyStartIndex.Value := FCurrentMonsterClientConfig.Fly_StartIndex;
    seClientFlyPlayCount.Value := FCurrentMonsterClientConfig.Fly_PlayCount;
    seClientFlyEmptyCount.Value := FCurrentMonsterClientConfig.Fly_EmptyCount;
    seClientFlyPlayTime.Value := FCurrentMonsterClientConfig.Fly_PlayTime;
    cbbClientFlyDrawMode.ItemIndex := Integer(FCurrentMonsterClientConfig.Fly_DrawMode);
    cbbClientFlyDirCount.ItemIndex := Integer(FCurrentMonsterClientConfig.Fly_DirCount);
    chkClientFlyCalcDir.Checked := FCurrentMonsterClientConfig.Fly_CalcDir;
    seFlyLightRange.Value := FCurrentMonsterClientConfig.Fly_LightRange;
    cbbClientFlyEffFile.ItemIndex := FCurrentMonsterClientConfig.FlyEff_File + 1;
    seClientFlyEffStartIndex.Value := FCurrentMonsterClientConfig.FlyEff_StartIndex;
    cbbClientFlyEffDrawMode.ItemIndex := Integer(FCurrentMonsterClientConfig.FlyEff_DrawMode);
    cbbClientSelfFile.ItemIndex := FCurrentMonsterClientConfig.Self_File + 1;
    seClientSelfStartIndex.Value := FCurrentMonsterClientConfig.Self_StartIndex;
    seClientSelfPlayCount.Value := FCurrentMonsterClientConfig.Self_PlayCount;
    seClientSelfEmptyCount.Value := FCurrentMonsterClientConfig.Self_EmptyCount;
    seClientSelfPlayTime.Value := FCurrentMonsterClientConfig.Self_PlayTime;
    // cbbClientSelfPlayMode.ItemIndex := Integer(FCurrentMonsterClientConfig.Self_PlayMode);
    cbbClientSelfDrawOrder.ItemIndex := Integer(FCurrentMonsterClientConfig.Self_DrawOrder);
    cbbClientSelfDrawMode.ItemIndex := Integer(FCurrentMonsterClientConfig.Self_DrawMode);
    cbbClientSelfDirCalcType.ItemIndex := Integer(FCurrentMonsterClientConfig.Self_DirCalcType);
    cbbClientSelfDirCount.ItemIndex := Integer(FCurrentMonsterClientConfig.Self_DirCount);
    chkClientSelfPlayDelayAction.Checked := FCurrentMonsterClientConfig.Self_PlayDelayAction;
    seSelfLightRange.Value := FCurrentMonsterClientConfig.Self_LightRange;
    cbbClientSelfKeepFile.ItemIndex := FCurrentMonsterClientConfig.SelfKeep_File + 1;
    seClientSelfKeepStartIndex.Value := FCurrentMonsterClientConfig.SelfKeep_StartIndex;
    seClientSelfKeepStartIndex2.Value := FCurrentMonsterClientConfig.SelfKeep_StartIndex2;
    seClientSelfKeepPlayCount.Value := FCurrentMonsterClientConfig.SelfKeep_PlayCount;
    seClientSelfKeepPlayTime.Value := FCurrentMonsterClientConfig.SelfKeep_PlayTime;
    cbbClientSelfKeepDrawOrder.ItemIndex := Integer(FCurrentMonsterClientConfig.SelfKeep_DrawOrder);
    cbbClientSelfKeepDrawMode.ItemIndex := Integer(FCurrentMonsterClientConfig.SelfKeep_DrawMode);
    cbbClientSelfKeepDrawMode2.ItemIndex := Integer(FCurrentMonsterClientConfig.SelfKeep_DrawMode2);
    seClientSelfKeepTime.Value := FCurrentMonsterClientConfig.SelfKeep_KeepTime;
    // seClientSelfKeepTime2.Value := FCurrentMonsterClientConfig.SelfKeep_KeepTime2;
    cbbClientExplosionFile.ItemIndex := FCurrentMonsterClientConfig.Explosion_File + 1;
    seClientExplosionStartIndex.Value := FCurrentMonsterClientConfig.Explosion_StartIndex;
    seClientExplosionStartIndex2.Value := FCurrentMonsterClientConfig.Explosion_StartIndex2;
    seClientExplosionPlayCount.Value := FCurrentMonsterClientConfig.Explosion_PlayCount;
    seClientExplosionPlayTime.Value := FCurrentMonsterClientConfig.Explosion_PlayTime;
    cbbClientExplosionDrawMode.ItemIndex := Integer(FCurrentMonsterClientConfig.Explosion_DrawMode);
    cbbClientExplosionDrawMode2.ItemIndex := Integer(FCurrentMonsterClientConfig.Explosion_DrawMode2);
    chkClientExplosionLockDraw.Checked := FCurrentMonsterClientConfig.Explosion_LockDraw;
    seExplosionLightRange.Value := FCurrentMonsterClientConfig.Explosion_LightRange;
    chkClientExplosionKeepPlay.Checked := FCurrentMonsterClientConfig.Explosion_KeepPlay;
    seClientExplosionKeepTime.Value := FCurrentMonsterClientConfig.Explosion_KeepTime;
    seClientExplosionKeepAttackInterval.Value := FCurrentMonsterClientConfig.Explosion_KeepAttackInterval;
    seClientExplosionKeepAttackRange.Value := FCurrentMonsterClientConfig.Explosion_KeepAttackRange;
    seExplosionKeepLightRange.Value := FCurrentMonsterClientConfig.Explosion_KeepLightRange;
    chkClientExplosionKeepMultiPlay.Checked := FCurrentMonsterClientConfig.Explosion_KeepMultiPlay;
    cbbClientTargetFile.ItemIndex := FCurrentMonsterClientConfig.Target_File + 1;
    seClientTargetStartIndex.Value := FCurrentMonsterClientConfig.Target_StartIndex;
    seClientTargetStartIndex2.Value := FCurrentMonsterClientConfig.Target_StartIndex2;
    seClientTargetPlayCount.Value := FCurrentMonsterClientConfig.Target_PlayCount;
    seClientTargetPlayTime.Value := FCurrentMonsterClientConfig.Target_PlayTime;
    cbbClientTargetDrawMode.ItemIndex := Integer(FCurrentMonsterClientConfig.Target_DrawMode);
    cbbClientTargetDrawMode2.ItemIndex := Integer(FCurrentMonsterClientConfig.Target_DrawMode2);
    chkClientTargetMultiPlay.Checked := FCurrentMonsterClientConfig.Target_MultiPlay;
    chkClientTargetLockDraw.Checked := FCurrentMonsterClientConfig.Target_LockDraw;
    seTargetLightRange.Value := FCurrentMonsterClientConfig.Target_LightRange;
    chkTargetKeepPlay.Checked := FCurrentMonsterClientConfig.Target_KeepPlay;
    seTargetKeepTime.Value := FCurrentMonsterClientConfig.Target_KeepTime;
    seTargetKeepAttackRange.Value := FCurrentMonsterClientConfig.Target_KeepAttackRange;
    chkTargetKeepMultiPlay.Checked := FCurrentMonsterClientConfig.Target_KeepMultiPlay;
    seTargetKeepAttackInterval.Value := FCurrentMonsterClientConfig.Target_KeepAttackInterval;
    seTargetKeepLightRange.Value := FCurrentMonsterClientConfig.Target_KeepLightRange;
    SetMonsterConfigChanged(OldChanged);
    FIsMonsterChanged := OldIsConfigCanSave;
    if not FIsMonsterChanged then
      btnSave.Enabled := False;
  end;
end;

procedure TfrmMonsterConfig.cbbClientFlyFileChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Fly_File := cbbClientFlyFile.ItemIndex - 1;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientFlyStartIndexChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Fly_StartIndex := seClientFlyStartIndex.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientFlyPlayCountChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Fly_PlayCount := seClientFlyPlayCount.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientFlyEmptyCountChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Fly_EmptyCount := seClientFlyEmptyCount.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientFlyPlayTimeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Fly_PlayTime := seClientFlyPlayTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientFlyDrawModeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Fly_DrawMode := TCustomDrawMode(cbbClientFlyDrawMode.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientFlyDirCountChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Fly_DirCount := TCustomDirCount(cbbClientFlyDirCount.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkClientFlyCalcDirClick(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Fly_CalcDir := chkClientFlyCalcDir.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seFlyLightRangeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Fly_LightRange := seFlyLightRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientFlyEffFileChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.FlyEff_File := cbbClientFlyEffFile.ItemIndex - 1;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientFlyEffStartIndexChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.FlyEff_StartIndex := seClientFlyEffStartIndex.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientFlyEffDrawModeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.FlyEff_DrawMode := TCustomDrawMode(cbbClientFlyEffDrawMode.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientSelfFileChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_File := cbbClientSelfFile.ItemIndex - 1;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientSelfStartIndexChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_StartIndex := seClientSelfStartIndex.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientSelfPlayCountChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_PlayCount := seClientSelfPlayCount.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientSelfEmptyCountChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_EmptyCount := seClientSelfEmptyCount.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientSelfPlayTimeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_PlayTime := seClientSelfPlayTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientSelfPlayModeChange(Sender: TObject);
begin
  {
    if FCurrentMonsterClientConfig <> nil then
    begin
    FCurrentMonsterClientConfig.Self_PlayMode := TMonsterPlayMode(cbbClientSelfPlayMode.ItemIndex);
    SetMonsterConfigChanged();
    end;
  }
end;

procedure TfrmMonsterConfig.cbbClientSelfDrawOrderChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_DrawOrder := TCustomDrawOrder(cbbClientSelfDrawOrder.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientSelfDrawModeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_DrawMode := TCustomDrawMode(cbbClientSelfDrawMode.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientSelfDirCountChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_DirCount := TCustomDirCount(cbbClientSelfDirCount.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientSelfDirCalcTypeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_DirCalcType := TCustomDirCalcType(cbbClientSelfDirCalcType.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkClientSelfPlayDelayActionClick(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_PlayDelayAction := chkClientSelfPlayDelayAction.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seSelfLightRangeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Self_LightRange := seSelfLightRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientExplosionFileChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_File := cbbClientExplosionFile.ItemIndex - 1;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientExplosionStartIndexChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_StartIndex := seClientExplosionStartIndex.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientExplosionStartIndex2Change(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_StartIndex2 := seClientExplosionStartIndex2.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientExplosionPlayCountChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_PlayCount := seClientExplosionPlayCount.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientExplosionDrawModeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_DrawMode := TCustomDrawMode(cbbClientExplosionDrawMode.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientExplosionDrawMode2Change(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_DrawMode2 := TCustomDrawMode(cbbClientExplosionDrawMode2.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientExplosionPlayTimeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_PlayTime := seClientExplosionPlayTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkClientExplosionLockDrawClick(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_LockDraw := chkClientExplosionLockDraw.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seExplosionLightRangeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_LightRange := seExplosionLightRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientTargetFileChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_File := cbbClientTargetFile.ItemIndex - 1;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientTargetStartIndexChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_StartIndex := seClientTargetStartIndex.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientTargetStartIndex2Change(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_StartIndex2 := seClientTargetStartIndex2.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientTargetPlayCountChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_PlayCount := seClientTargetPlayCount.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientTargetPlayTimeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_PlayTime := seClientTargetPlayTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientTargetDrawModeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_DrawMode := TCustomDrawMode(cbbClientTargetDrawMode.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientTargetDrawMode2Change(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_DrawMode2 := TCustomDrawMode(cbbClientTargetDrawMode2.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkClientTargetMultiPlayClick(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_MultiPlay := chkClientTargetMultiPlay.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkClientTargetLockDrawClick(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_LockDraw := chkClientTargetLockDraw.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seTargetLightRangeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_LightRange := seTargetLightRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkTargetKeepPlayClick(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_KeepPlay := chkTargetKeepPlay.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seTargetKeepTimeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_KeepTime := seTargetKeepTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seTargetKeepAttackRangeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_KeepAttackRange := seTargetKeepAttackRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seTargetKeepLightRangeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_KeepLightRange := seTargetKeepLightRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkTargetKeepMultiPlayClick(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_KeepMultiPlay := chkTargetKeepMultiPlay.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seTargetKeepAttackIntervalChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Target_KeepAttackInterval := seTargetKeepAttackInterval.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbBatchActionChange(Sender: TObject);
var
  Node: PVirtualNode;
  NodeData: PMonsterNodeData;
begin
  if vstAction.RootNodeCount = 0 then
    Exit;
  Node := vstAction.GetFirst();
  while Node <> nil do
  begin
    NodeData := vstAction.GetNodeData(Node);
    NodeData.Action.ActionFile := cbbBatchAction.ItemIndex - 1;
    Node := vstAction.GetNext(Node);
  end;
  vstAction.Invalidate;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.cbbBatchEffectChange(Sender: TObject);
var
  Node: PVirtualNode;
  NodeData: PMonsterNodeData;
begin
  if vstAction.RootNodeCount = 0 then
    Exit;
  Node := vstAction.GetFirst();
  while Node <> nil do
  begin
    NodeData := vstAction.GetNodeData(Node);
    NodeData.Action.EffectFile := cbbBatchEffect.ItemIndex - 1;
    Node := vstAction.GetNext(Node);
  end;
  vstAction.Invalidate;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.cbbServerAttackConfigChange(Sender: TObject);
var
  Index: Integer;
  OldChanged, OldIsConfigCanSave: Boolean;
begin
  FCurrentMonsterServerConfig := nil;
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  OldIsConfigCanSave := FIsMonsterChanged;
  Index := cbbServerAttackConfig.ItemIndex;
  if (Index >= Low(FCurrentMonsterCustomConfig.MonsterServerConfigs)) and
    (Index <= High(FCurrentMonsterCustomConfig.MonsterServerConfigs)) then
  begin
    OldChanged := FCurrentMonsterCustomConfig.IsChanged;
    FCurrentMonsterServerConfig := @FCurrentMonsterCustomConfig.MonsterServerConfigs[Index];
    grpServerAttackConfigs.Caption := AttackConfigNames[Index] + '的攻击效果配置';
    chkAttackEnabled.Checked := FCurrentMonsterServerConfig.AttackEnabled;
    cbbOperateMode.ItemIndex := Integer(FCurrentMonsterServerConfig.OperateMode);
    cbbOperateMode.OnChange(cbbOperateMode);
    chkAttackSelfDie.Checked := FCurrentMonsterServerConfig.AttackSelfDie;
    seAttackDelayTime.Value := FCurrentMonsterServerConfig.AttackDelayTime;
    // seAttackWaitTime.Value := FCurrentMonsterServerConfig.AttackWaitTime;
    seAttackHPPercent.Value := FCurrentMonsterServerConfig.AttackHPPercent;
    seAttackRate.Value := FCurrentMonsterServerConfig.AttackRate;
    seAttackTargetCount.Value := FCurrentMonsterServerConfig.AttackTargetCount;
    cbbAttackMode.ItemIndex := Integer(FCurrentMonsterServerConfig.AttackMode);
    cbbAttackTarget.ItemIndex := Integer(FCurrentMonsterServerConfig.AttackTarget);
    cbbAttackPowerCalc.ItemIndex := Integer(FCurrentMonsterServerConfig.AttackPowerCalc);
    seAttackPowerRate.Value := FCurrentMonsterServerConfig.AttackPowerRate;
    chkAttackTeleportAttack.Checked := FCurrentMonsterServerConfig.AttackTeleportAttack;
    chkAttackTeleportRush.Checked := FCurrentMonsterServerConfig.AttackTeleportRush;
    seAttackTeleportTargetDistance.Value := FCurrentMonsterServerConfig.AttackTeleportTargetDistance;
    seAttackTeleportDistance.Value := FCurrentMonsterServerConfig.AttackTeleportDistance;
    seAttackTeleportRate.Value := FCurrentMonsterServerConfig.AttackTeleportRate;
    chkAttackIgnoreDefence.Checked := FCurrentMonsterServerConfig.AttackIgnoreDefence;
    seAttackNearRange.Value := FCurrentMonsterServerConfig.AttackNearRange;
    seAttackGroupRange.Value := FCurrentMonsterServerConfig.AttackGroupRange;
    chkNearAttackTargetCenter.Checked := FCurrentMonsterServerConfig.NearAttackTargetCenter;
    seAttackPowerInc.Value := FCurrentMonsterServerConfig.AttackPowerInc;
    {
      cbbHumanDefenceMagic.ItemIndex := FCurrentMonsterServerConfig.HumanDefenceMagic;
      seHumanDefenceNewLevel.Value := FCurrentMonsterServerConfig.HumanDefenceNewLevel;
      seHumanDefenceTime.Value := FCurrentMonsterServerConfig.HumanDefenceTime;
      seHumanDefenceHPPercent.Value := FCurrentMonsterServerConfig.HumanDefenceHPPercent;
      cbbHumanAttackMagic.ItemIndex := FCurrentMonsterServerConfig.HumanAttackMagic;
      seHumanAttackNewLevel.Value := FCurrentMonsterServerConfig.HumanAttackNewLevel;
      seHumanAttackRate.Value := FCurrentMonsterServerConfig.HumanAttackRate;
      chkOnlyUseHumanMagic.Checked := FCurrentMonsterServerConfig.OnlyUseHumanMagic;
    }
    chkAdditional0.Checked := FCurrentMonsterServerConfig.Additionals[0].Checked;
    seAdditionalRate0.Value := FCurrentMonsterServerConfig.Additionals[0].Rate;
    seAdditionalTime0.Value := FCurrentMonsterServerConfig.Additionals[0].Time;
    seAdditionaHP0.Value := FCurrentMonsterServerConfig.AdditionalHP0;
    chkAdditional1.Checked := FCurrentMonsterServerConfig.Additionals[1].Checked;
    seAdditionalRate1.Value := FCurrentMonsterServerConfig.Additionals[1].Rate;
    seAdditionalTime1.Value := FCurrentMonsterServerConfig.Additionals[1].Time;
    chkAdditional2.Checked := FCurrentMonsterServerConfig.Additionals[2].Checked;
    seAdditionalRate2.Value := FCurrentMonsterServerConfig.Additionals[2].Rate;
    seAdditionalTime2.Value := FCurrentMonsterServerConfig.Additionals[2].Time;
    chkAdditional3.Checked := FCurrentMonsterServerConfig.Additionals[3].Checked;
    seAdditionalRate3.Value := FCurrentMonsterServerConfig.Additionals[3].Rate;
    seAdditionalTime3.Value := FCurrentMonsterServerConfig.Additionals[3].Time;
    chkAdditional4.Checked := FCurrentMonsterServerConfig.Additionals[4].Checked;
    seAdditionalRate4.Value := FCurrentMonsterServerConfig.Additionals[4].Rate;
    seAdditionalTime4.Value := FCurrentMonsterServerConfig.Additionals[4].Time;
    chkseAdditionaHighLevel4.Checked := FCurrentMonsterServerConfig.AdditionalHighLevel4;
    chkAdditional5.Checked := FCurrentMonsterServerConfig.Additionals[5].Checked;
    seAdditionalRate5.Value := FCurrentMonsterServerConfig.Additionals[5].Rate;
    seAdditionalTime5.Value := FCurrentMonsterServerConfig.Additionals[5].Time;
    chkAdditional6.Checked := FCurrentMonsterServerConfig.Additionals[6].Checked;
    seAdditionalRate6.Value := FCurrentMonsterServerConfig.Additionals[6].Rate;
    seAdditionalTime6.Value := FCurrentMonsterServerConfig.Additionals[6].Time;
    chkAdditional7.Checked := FCurrentMonsterServerConfig.Additionals[7].Checked;
    seAdditionalRate7.Value := FCurrentMonsterServerConfig.Additionals[7].Rate;
    seAdditionalTime7.Value := FCurrentMonsterServerConfig.Additionals[7].Time;
    chkAdditional8.Checked := FCurrentMonsterServerConfig.Additionals[8].Checked;
    seAdditionalRate8.Value := FCurrentMonsterServerConfig.Additionals[8].Rate;
    seAdditionalTime8.Value := FCurrentMonsterServerConfig.Additionals[8].Time;
    chkAdditional9.Checked := FCurrentMonsterServerConfig.Additionals[9].Checked;
    seAdditionalRate9.Value := FCurrentMonsterServerConfig.Additionals[9].Rate;
    seAdditionalTime9.Value := FCurrentMonsterServerConfig.Additionals[9].Time;
    chkAdditional10.Checked := FCurrentMonsterServerConfig.Additionals[10].Checked;
    seAdditionalRate10.Value := FCurrentMonsterServerConfig.Additionals[10].Rate;
    seAdditionalTime10.Value := FCurrentMonsterServerConfig.Additionals[10].Time;
    chkAdditional11.Checked := FCurrentMonsterServerConfig.Additionals[11].Checked;
    seAdditionalRate11.Value := FCurrentMonsterServerConfig.Additionals[11].Rate;
    seAdditionalTime11.Value := FCurrentMonsterServerConfig.Additionals[11].Time;
    seAdditionalImprisonRange.Value := FCurrentMonsterServerConfig.AdditionalImprisonRange;
    chkEnabledCallMonster.Checked := FCurrentMonsterServerConfig.EnabledCallMonster;
    seCallMonstersRate.Value := FCurrentMonsterServerConfig.CallMonstersRate;
    edtCallMonster1.Text := FCurrentMonsterServerConfig.CallMonsters[0];
    seCallMonsterNum1.Value := FCurrentMonsterServerConfig.CallMonsterNums[0];
    edtCallMonster2.Text := FCurrentMonsterServerConfig.CallMonsters[1];
    seCallMonsterNum2.Value := FCurrentMonsterServerConfig.CallMonsterNums[1];
    edtCallMonster3.Text := FCurrentMonsterServerConfig.CallMonsters[2];
    seCallMonsterNum3.Value := FCurrentMonsterServerConfig.CallMonsterNums[2];
    edtCallMonster4.Text := FCurrentMonsterServerConfig.CallMonsters[3];
    seCallMonsterNum4.Value := FCurrentMonsterServerConfig.CallMonsterNums[3];
    chkMoveTarget.Checked := FCurrentMonsterServerConfig.MoveTarget;
    seMoveTargetRate.Value := FCurrentMonsterServerConfig.MoveTargetRate;
    chkMoveTargetHighLevel.Checked := FCurrentMonsterServerConfig.MoveTargetHighLevel;
    // chkCopySelf.Checked := FCurrentMonsterServerConfig.CopySelf;
    // seCopySelfMaxCount.Value := FCurrentMonsterServerConfig.CopySelfMaxCount;
    // seCopySelfTime.Value := FCurrentMonsterServerConfig.CopySelfTime;
    chkProtectAddHP.Checked := FCurrentMonsterServerConfig.ProtectAddHP;
    seProtectAddHPRate.Value := FCurrentMonsterServerConfig.ProtectAddHPRate;
    seProtectAddHPPercent.Value := FCurrentMonsterServerConfig.ProtectAddHPPercent;
    chkProtectAddDefence.Checked := FCurrentMonsterServerConfig.ProtectAddDefence;
    seProtectAddDefenceRate.Value := FCurrentMonsterServerConfig.ProtectAddDefenceRate;
    seProtectAddDefencePercent.Value := FCurrentMonsterServerConfig.ProtectAddDefencePercent;
    seProtectAddDefenceTime.Value := FCurrentMonsterServerConfig.ProtectAddDefenceTime;
    chkProtectAddMagDefence.Checked := FCurrentMonsterServerConfig.ProtectAddMagDefence;
    seProtectAddMagDefenceRate.Value := FCurrentMonsterServerConfig.ProtectAddMagDefenceRate;
    seProtectAddMagDefencePercent.Value := FCurrentMonsterServerConfig.ProtectAddMagDefencePercent;
    seProtectAddMagDefenceTime.Value := FCurrentMonsterServerConfig.ProtectAddMagDefenceTime;
    chkProtectAddDC.Checked := FCurrentMonsterServerConfig.ProtectAddDC;
    seProtectAddDCRate.Value := FCurrentMonsterServerConfig.ProtectAddDCRate;
    seProtectAddDCPercent.Value := FCurrentMonsterServerConfig.ProtectAddDCPercent;
    seProtectAddDCTime.Value := FCurrentMonsterServerConfig.ProtectAddDCTime;
    chkProtectAddMC.Checked := FCurrentMonsterServerConfig.ProtectAddMC;
    seProtectAddMCRate.Value := FCurrentMonsterServerConfig.ProtectAddMCRate;
    seProtectAddMCPercent.Value := FCurrentMonsterServerConfig.ProtectAddMCPercent;
    seProtectAddMCTime.Value := FCurrentMonsterServerConfig.ProtectAddMCTime;
    chkProtectAddSC.Checked := FCurrentMonsterServerConfig.ProtectAddSC;
    seProtectAddSCRate.Value := FCurrentMonsterServerConfig.ProtectAddSCRate;
    seProtectAddSCPercent.Value := FCurrentMonsterServerConfig.ProtectAddSCPercent;
    seProtectAddSCTime.Value := FCurrentMonsterServerConfig.ProtectAddSCTime;
    seProtectTargetRange.Value := FCurrentMonsterServerConfig.ProtectTargetRange;
    seProtectSelfRate.Value := FCurrentMonsterServerConfig.ProtectSelfRate;
    SetMonsterConfigChanged(OldChanged);
    FIsMonsterChanged := OldIsConfigCanSave;

    if not FIsMonsterChanged then
      btnSave.Enabled := False;
  end;
end;

procedure TfrmMonsterConfig.seViewRangeChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ServerBaseConfig.ViewRange := seViewRange.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.cbbMonsterTypeChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ServerBaseConfig.MonsterType := TMonsterType(cbbMonsterType.ItemIndex);
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.cbbMoveOptionChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ServerBaseConfig.MoveOption := TMoveOption(cbbMoveOption.ItemIndex);
  SetMonsterConfigChanged();
  lblProtect.Visible := cbbMoveOption.ItemIndex = Integer(moProtect);
  seProtectRange.Visible := lblProtect.Visible;
end;

procedure TfrmMonsterConfig.seProtectRangeChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ServerBaseConfig.ProtectRange := seProtectRange.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.seMinAttackNearRangeChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ServerBaseConfig.MinAttackNearRange := seMinAttackNearRange.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.seLightRangeChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ServerBaseConfig.LightRange := seLightRange.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.chkNoAttackClick(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ServerBaseConfig.NoAttack := chkNoAttack.Checked;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.chkAttackEnabledClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackEnabled := chkAttackEnabled.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbOperateModeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.OperateMode := TCustomOperateMode(cbbOperateMode.ItemIndex);
    SetMonsterConfigChanged();
    seAttackTargetCount.Enabled := FCurrentMonsterServerConfig.OperateMode = momAttack;
    grpOptions.Visible := seAttackTargetCount.Enabled;
    lblAttackDelay.Visible := seAttackTargetCount.Enabled;
    seAttackDelayTime.Visible := seAttackTargetCount.Enabled;
    lblAttackDelayTime.Visible := seAttackTargetCount.Enabled;
    grpMove.Visible := grpOptions.Visible;
    grpCallMob.Visible := grpOptions.Visible;
    grpAdditionals.Visible := grpOptions.Visible;
    grpMoveTarget.Visible := grpOptions.Visible;
    grpProtect.Visible := not grpOptions.Visible;
    grpProtect.Left := grpMove.Left;
    grpProtect.Top := grpMove.Top;
  end;
end;

procedure TfrmMonsterConfig.chkAttackSelfDieClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackSelfDie := chkAttackSelfDie.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackDelayTimeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackDelayTime := seAttackDelayTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackHPPercentChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackHPPercent := seAttackHPPercent.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackRate := seAttackRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackTargetCountChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackTargetCount := seAttackTargetCount.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbAttackModeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackMode := TCustomAttackMode(cbbAttackMode.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbAttackTargetChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackTarget := TCustomAttackTarget(cbbAttackTarget.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbAttackPowerCalcChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackPowerCalc := TCustomAttackPowerCalc(cbbAttackPowerCalc.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackPowerRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackPowerRate := seAttackPowerRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkAttackTeleportAttackClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackTeleportAttack := chkAttackTeleportAttack.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkAttackTeleportRushClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackTeleportRush := chkAttackTeleportRush.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackTeleportTargetDistanceChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackTeleportTargetDistance := seAttackTeleportTargetDistance.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackTeleportDistanceChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackTeleportDistance := seAttackTeleportDistance.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackTeleportRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackTeleportRate := seAttackTeleportRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkAttackIgnoreDefenceClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackIgnoreDefence := chkAttackIgnoreDefence.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackNearRangeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackNearRange := seAttackNearRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackGroupRangeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackGroupRange := seAttackGroupRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkAdditional0Click(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentMonsterServerConfig <> nil) and (Sender is TCheckBox) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentMonsterServerConfig.Additionals)) and
      (WinCtrl.Tag <= High(FCurrentMonsterServerConfig.Additionals)) then
      FCurrentMonsterServerConfig.Additionals[WinCtrl.Tag].Checked := TCheckBox(Sender).Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAdditionalRate0Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentMonsterServerConfig <> nil) and (Sender is TSpinEditEx) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentMonsterServerConfig.Additionals)) and
      (WinCtrl.Tag <= High(FCurrentMonsterServerConfig.Additionals)) then
      FCurrentMonsterServerConfig.Additionals[WinCtrl.Tag].Rate := TSpinEditEx(Sender).Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAdditionalTime0Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentMonsterServerConfig <> nil) and (Sender is TSpinEditEx) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentMonsterServerConfig.Additionals)) and
      (WinCtrl.Tag <= High(FCurrentMonsterServerConfig.Additionals)) then
      FCurrentMonsterServerConfig.Additionals[WinCtrl.Tag].Time := TSpinEditEx(Sender).Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAdditionaHP0Change(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AdditionalHP0 := seAdditionaHP0.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAdditionalImprisonRangeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AdditionalImprisonRange := seAdditionalImprisonRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkseAdditionaHighLevel4Click(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AdditionalHighLevel4 := chkseAdditionaHighLevel4.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkEnabledCallMonsterClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.EnabledCallMonster := chkEnabledCallMonster.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seCallMonstersRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.CallMonstersRate := seCallMonstersRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.edtCallMonster1Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentMonsterServerConfig <> nil) and (Sender is TEdit) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentMonsterServerConfig.CallMonsters)) and
      (WinCtrl.Tag <= High(FCurrentMonsterServerConfig.CallMonsters)) then
      FCurrentMonsterServerConfig.CallMonsters[WinCtrl.Tag] := TEdit(Sender).Text;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seCallMonsterNum1Change(Sender: TObject);
var
  WinCtrl: TWinControl;
begin
  if (FCurrentMonsterServerConfig <> nil) and (Sender is TSpinEditEx) then
  begin
    WinCtrl := Sender as TWinControl;
    if (WinCtrl.Tag >= Low(FCurrentMonsterServerConfig.CallMonsterNums)) and
      (WinCtrl.Tag <= High(FCurrentMonsterServerConfig.CallMonsterNums)) then
      FCurrentMonsterServerConfig.CallMonsterNums[WinCtrl.Tag] := TSpinEditEx(Sender).Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkCopySelfClick(Sender: TObject);
begin
  {
    if FCurrentMonsterServerConfig <> nil then
    begin
    FCurrentMonsterServerConfig.CopySelf := chkCopySelf.Checked;
    SetMonsterConfigChanged();
    end;
  }
end;

procedure TfrmMonsterConfig.seCopySelfMaxCountChange(Sender: TObject);
begin
  {
    if FCurrentMonsterServerConfig <> nil then
    begin
    FCurrentMonsterServerConfig.CopySelfMaxCount := seCopySelfMaxCount.Value;
    SetMonsterConfigChanged();
    end;
  }
end;

procedure TfrmMonsterConfig.seCopySelfTimeChange(Sender: TObject);
begin
  {
    if FCurrentMonsterServerConfig <> nil then
    begin
    FCurrentMonsterServerConfig.CopySelfTime := seCopySelfTime.Value;
    SetMonsterConfigChanged();
    end;
  }
end;

procedure TfrmMonsterConfig.chkProtectAddHPClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddHP := chkProtectAddHP.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddHPRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddHPRate := seProtectAddHPRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddHPPercentChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddHPPercent := seProtectAddHPPercent.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkProtectAddDefenceClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddDefence := chkProtectAddDefence.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkProtectAddMagDefenceClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddMagDefence := chkProtectAddMagDefence.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddDefenceRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddDefenceRate := seProtectAddDefenceRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddDefencePercentChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddDefencePercent := seProtectAddDefencePercent.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddDefenceTimeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddDefenceTime := seProtectAddDefenceTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkProtectAddDCClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddDC := chkProtectAddDC.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddDCRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddDCRate := seProtectAddDCRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddDCPercentChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddDCPercent := seProtectAddDCPercent.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddDCTimeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddDCTime := seProtectAddDCTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkDieNoCalcDirClick(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.DieNoCalcDir := chkDieNoCalcDir.Checked;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.chkProtectAddMCClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddMC := chkProtectAddMC.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddMCRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddMCRate := seProtectAddMCRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddMCPercentChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddMCPercent := seProtectAddMCPercent.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddMCTimeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddMCTime := seProtectAddMCTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkProtectAddSCClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddSC := chkProtectAddSC.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddSCRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddSCRate := seProtectAddSCRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddSCPercentChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddSCPercent := seProtectAddSCPercent.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddSCTimeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddSCTime := seProtectAddSCTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddMagDefenceRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddMagDefenceRate := seProtectAddMagDefenceRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddMagDefencePercentChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddMagDefencePercent := seProtectAddMagDefencePercent.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectAddMagDefenceTimeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectAddMagDefenceTime := seProtectAddMagDefenceTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectTargetRangeChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectTargetRange := seProtectTargetRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seProtectSelfRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.ProtectSelfRate := seProtectSelfRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure RebuildCustomMonsterListText;
var
  I: Integer;
  CustomMonsterConfig: TCustomMonsterConfig;
  InBuf: PAnsiChar;
  InBytes: Integer;
  ClientConfig: PClientCustomMonsterConfig;
begin
  if g_MultiThreadRun then
    UserEngine.m_CustomMonsterList.LockR(5);
  try
    InBytes := UserEngine.m_CustomMonsterList.Count * SizeOf(TClientCustomMonsterConfig);
    GetMem(InBuf, InBytes);
    try
      ClientConfig := PClientCustomMonsterConfig(InBuf);
      for I := 0 to UserEngine.m_CustomMonsterList.Count - 1 do
      begin
        CustomMonsterConfig := UserEngine.m_CustomMonsterList.Items[I];
        ClientConfig^.wMonsterAppr := CustomMonsterConfig.MonsterAppr;
        ClientConfig^.BaseConfig := CustomMonsterConfig.ClientBaseConfig;
        ClientConfig^.Actions := CustomMonsterConfig.ClientActions;
        ClientConfig^.AttackConfigs := CustomMonsterConfig.ClientAttackConfigs;
        Inc(ClientConfig);
      end;
      g_CustomMonsterListTextLen := InBytes;
      g_CustomMonsterListText := zLibCompressBuffer(InBuf, InBytes);
      g_CustomMonsterListTextCRC := BufferCrc(PAnsiChar(g_CustomMonsterListText), Length(g_CustomMonsterListText));
    finally
      FreeMem(InBuf, InBytes);
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomMonsterList.UnLockR;
  end;
end;

procedure TfrmMonsterConfig.btnSaveClick(Sender: TObject);
var
  Node: PVirtualNode;
  ConfigNodeData: PMonsterConfigNodeData;
begin
  if vstAction.IsEditing then
    vstAction.EndEditNode;
  Node := vstCustomMonster.GetFirst();
  while Node <> nil do
  begin
    ConfigNodeData := vstCustomMonster.GetNodeData(Node);
    if (ConfigNodeData <> nil) then
    begin
      if ConfigNodeData.Config.IsChanged then
        ConfigNodeData.Config.SaveToIniFile;
    end;
    Node := vstCustomMonster.GetNext(Node);
  end;
  vstCustomMonster.Invalidate;
  RebuildCustomMonsterListText;
  FIsMonsterChanged := False;
  btnSave.Enabled := False;
end;

procedure TfrmMonsterConfig.cbbClientDrawModeChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.DrawMode := TCustomDrawMode(cbbClientDrawMode.ItemIndex);
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.cbbClientDrawMode2Change(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.DrawMode2 := TCustomDrawMode(cbbClientDrawMode2.ItemIndex);
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.cbbClientDrawOrder2Change(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.DrawOrder := TMonsterDrawOrder2(cbbClientDrawOrder2.ItemIndex);
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.edtSoundNormalChange(Sender: TObject);
var
  Edit: TEdit;
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  if Sender is TEdit then
  begin
    Edit := Sender as TEdit;
    if (Edit.Tag >= Integer(Low(TMonsterSoundType))) and (Edit.Tag <= Integer(High(TMonsterSoundType))) then
    begin
      FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[TMonsterSoundType(Edit.Tag)] := Edit.Text;
      SetMonsterConfigChanged();
    end;
  end;
end;

procedure TfrmMonsterConfig.seHPBgOffsetXChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.HPBgOffsetX := seHPBgOffsetX.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.seHPBgOffsetYChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.HPBgOffsetY := seHPBgOffsetY.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.seHPOffsetXChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.HPOffsetX := seHPOffsetX.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.seHPOffsetYChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.HPOffsetY := seHPOffsetY.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.cbbHPFileChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.HPFile := cbbHPFile.ItemIndex - 1;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.seHPStartIndexChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.HPStartIndex := seHPStartIndex.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.seHPTextOffsetXChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.HPTextOffsetX := seHPTextOffsetX.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.seHPTextOffsetYChange(Sender: TObject);
begin
  if FCurrentMonsterCustomConfig = nil then
    Exit;
  FCurrentMonsterCustomConfig.ClientBaseConfig.HPTextOffsetY := seHPTextOffsetY.Value;
  SetMonsterConfigChanged();
end;

procedure TfrmMonsterConfig.btnCalcStartIndexClick(Sender: TObject);
var
  NodeData: PMonsterNodeData;
  Node: PVirtualNode;
  Offset: Integer;
begin
  if (vstAction.RootNodeCount > 0) then
  begin
    if vstAction.IsEditing then
      vstAction.CancelEditNode;
    Node := vstAction.GetFirst();
    while Node <> nil do
    begin
      NodeData := vstAction.GetNodeData(Node);
      Offset := -1;
      case NodeData.Action.ActionType of
        matStand:
          Offset := 0;
        matWalk:
          Offset := 1;
        matDefAttack:
          Offset := 2;
        matStruck:
          Offset := 3;
        matDie:
          Offset := 4;
        // matStoneMode: Offset := 5;
      end;
      if Offset <> -1 then
      begin
        if seClientStartIndex.Value < 0 then
          NodeData.Action.StartIndex := -1
        else
          NodeData.Action.StartIndex := seClientStartIndex.Value + Offset * 80;
      end;
      Node := vstAction.GetNext(Node);
    end;
    vstAction.Invalidate;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.btnCalcEffectIndexClick(Sender: TObject);
var
  NodeData: PMonsterNodeData;
  Node: PVirtualNode;
  Offset: Integer;
begin
  if (vstAction.RootNodeCount > 0) then
  begin
    if vstAction.IsEditing then
      vstAction.CancelEditNode;
    Node := vstAction.GetFirst();
    while Node <> nil do
    begin
      NodeData := vstAction.GetNodeData(Node);
      Offset := -1;
      case NodeData.Action.ActionType of
        matStand:
          Offset := 0;
        matWalk:
          Offset := 1;
        matDefAttack:
          Offset := 2;
        matStruck:
          Offset := 3;
        matDie:
          Offset := 4;
        // matStoneMode: Offset := 5;
      end;
      if Offset <> -1 then
      begin
        if seClientEffectIndex.Value < 0 then
          NodeData.Action.EffectIndex := -1
        else
          NodeData.Action.EffectIndex := seClientEffectIndex.Value + Offset * 80;
      end;
      Node := vstAction.GetNext(Node);
    end;
    vstAction.Invalidate;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkSendCustomMonsterConfigClick(Sender: TObject);
begin
  g_Config.boSendCustomMonsterConfig := chkSendCustomMonsterConfig.Checked;
  Config.WriteBool('Setup', 'SendCustomMonsterConfig', g_Config.boSendCustomMonsterConfig);
end;

procedure TfrmMonsterConfig.vstActionChecked(Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  NodeData: PMonsterNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    if vstAction.CheckState[Node] = csCheckedNormal then
      NodeData.Action.CalcDir := True
    else
      NodeData.Action.CalcDir := False;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.vstActionGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
begin
  NodeDataSize := SizeOf(PMonsterClientAction);
end;

procedure TfrmMonsterConfig.vstCustomMonsterDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
var
  ConfigNodeData: PMonsterConfigNodeData;
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

procedure TfrmMonsterConfig.vstCustomMonsterGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
begin
  NodeDataSize := SizeOf(TCustomMonsterConfig);
end;

procedure TfrmMonsterConfig.vstCustomMonsterGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  ConfigNodeData: PMonsterConfigNodeData;
begin
  ConfigNodeData := Sender.GetNodeData(Node);
  if ConfigNodeData <> nil then
    CellText := ConfigNodeData.Config.MonsterName;
end;

procedure TfrmMonsterConfig.vstCustomMonsterKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  tmpNode: PVirtualNode;
  tmpVST: TVirtualStringTree;
  tmpKeyword: string;
  tmpItem: PMonsterConfigNodeData;
  tmpHitInfo: THitInfo;
begin
  case Key of
    Word('F'):
      begin
        if ssCtrl in Shift then
        begin
          Key := 0;
          tmpKeyword := '';
          tmpVST := TVirtualStringTree(Sender);
          if not InputQuery('关键字查找', '输入关键字:', tmpKeyword) or tmpKeyword.IsEmpty then
            Exit;

          tmpNode := tmpVST.GetFirstChild(nil);
          while Assigned(tmpNode) do
          begin
            tmpItem := tmpVST.GetNodeData(tmpNode);
            if (tmpItem <> nil) and SameText(tmpKeyword, tmpItem.Config.MonsterName) then
            begin
              tmpVST.Selected[tmpNode] := True;
              vstCustomMonsterNodeClick(tmpVST, tmpHitInfo);

              Break;
            end;
            tmpNode := tmpVST.GetNextSibling(tmpNode);
          end;
        end;
      end;
  end;
end;

procedure TfrmMonsterConfig.vstCustomMonsterNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  ActionType: TMonsterClientActionType;
  Node: PVirtualNode;
  NodeData: PMonsterNodeData;
  OldChanged, OldIsConfigCanSave: Boolean;
  ConfigNodeData: PMonsterConfigNodeData;
begin
  if vstAction.IsEditing then
    vstAction.EndEditNode;
  vstAction.Clear;
  FCurrentMonsterCustomConfig := nil;
  FCurrentMonsterClientConfig := nil;
  FCurrentMonsterServerConfig := nil;
  if vstCustomMonster.FocusedNode = nil then
    Exit;
  SetControlEnabled(pgcMain, True);
  ConfigNodeData := vstCustomMonster.GetNodeData(vstCustomMonster.FocusedNode);
  if ConfigNodeData = nil then
    Exit;
  OldIsConfigCanSave := FIsMonsterChanged;
  FCurrentMonsterCustomConfig := ConfigNodeData.Config;
  OldChanged := FCurrentMonsterCustomConfig.IsChanged;
  for ActionType := Low(TMonsterClientActionType) to High(TMonsterClientActionType) do
  begin
    Node := vstAction.AddChild(nil);
    NodeData := vstAction.GetNodeData(Node);
    NodeData.Action := @FCurrentMonsterCustomConfig.ClientActions[ActionType];
    Node.CheckType := ctCheckBox;
    if FCurrentMonsterCustomConfig.ClientActions[ActionType].CalcDir then
      vstAction.CheckState[Node] := csCheckedNormal
    else
      vstAction.CheckState[Node] := csUnCheckedNormal;
  end;
  cbbClientAttackConfig.ItemIndex := 0;
  cbbClientAttackConfig.OnChange(cbbClientAttackConfig);
  cbbClientDrawMode.ItemIndex := Integer(FCurrentMonsterCustomConfig.ClientBaseConfig.DrawMode);
  cbbClientDrawMode2.ItemIndex := Integer(FCurrentMonsterCustomConfig.ClientBaseConfig.DrawMode2);
  cbbClientDrawOrder2.ItemIndex := Integer(FCurrentMonsterCustomConfig.ClientBaseConfig.DrawOrder);
  chkDieNoCalcDir.Checked := FCurrentMonsterCustomConfig.ClientBaseConfig.DieNoCalcDir;
  edtSoundNormal.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstNormal];
  edtSoundDigUP.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstDigUP];
  edtSoundAttack.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstAttack];
  edtSoundStruck.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstStruck];
  edtSoundDie.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstDie];
  edtSoundAttack1.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstAttack1];
  edtSoundAttack2.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstAttack2];
  edtSoundAttack3.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstAttack3];
  edtSoundAttack4.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstAttack4];
  edtSoundAttack5.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstAttack5];
  edtSoundAttack6.Text := FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[mstAttack6];
  seHPBgOffsetX.Value := FCurrentMonsterCustomConfig.ClientBaseConfig.HPBgOffsetX;
  seHPBgOffsetY.Value := FCurrentMonsterCustomConfig.ClientBaseConfig.HPBgOffsetY;
  seHPOffsetX.Value := FCurrentMonsterCustomConfig.ClientBaseConfig.HPOffsetX;
  seHPOffsetY.Value := FCurrentMonsterCustomConfig.ClientBaseConfig.HPOffsetY;
  cbbHPFile.ItemIndex := FCurrentMonsterCustomConfig.ClientBaseConfig.HPFile + 1;
  seHPStartIndex.Value := FCurrentMonsterCustomConfig.ClientBaseConfig.HPStartIndex;
  seHPTextOffsetX.Value := FCurrentMonsterCustomConfig.ClientBaseConfig.HPTextOffsetX;
  seHPTextOffsetY.Value := FCurrentMonsterCustomConfig.ClientBaseConfig.HPTextOffsetY;
  cbbServerAttackConfig.ItemIndex := 0;
  cbbServerAttackConfig.OnChange(cbbServerAttackConfig);
  seViewRange.Value := FCurrentMonsterCustomConfig.ServerBaseConfig.ViewRange;
  cbbMonsterType.ItemIndex := Integer(FCurrentMonsterCustomConfig.ServerBaseConfig.MonsterType);
  cbbMoveOption.ItemIndex := Integer(FCurrentMonsterCustomConfig.ServerBaseConfig.MoveOption);
  seProtectRange.Value := FCurrentMonsterCustomConfig.ServerBaseConfig.ProtectRange;
  seMinAttackNearRange.Value := FCurrentMonsterCustomConfig.ServerBaseConfig.MinAttackNearRange;
  seLightRange.Value := FCurrentMonsterCustomConfig.ServerBaseConfig.LightRange;
  chkNoAttack.Checked := FCurrentMonsterCustomConfig.ServerBaseConfig.NoAttack;
  cbbMoveOption.OnChange(cbbMoveOption);
  SetMonsterConfigChanged(OldChanged);
  pnlMonDesc.Caption := '  Race = ' + IntToStr(FCurrentMonsterCustomConfig.MonsterRace) + '; ';
  case FCurrentMonsterCustomConfig.MonsterRace of
    154:
      pnlMonDesc.Caption := pnlMonDesc.Caption + '魔王岭怪物; 不攻击目标';
    155:
      pnlMonDesc.Caption := pnlMonDesc.Caption + '魔王岭、雕像类宝宝; 不受地图MISSION参数的限制，不回血，换地图后自动消失';
    156:
      pnlMonDesc.Caption := pnlMonDesc.Caption + '普通怪物; 主动攻击目标';
    157:
      pnlMonDesc.Caption := pnlMonDesc.Caption + '普通怪物; 不会主动攻击目标，如鸡羊鹿';
    159:
      pnlMonDesc.Caption := pnlMonDesc.Caption + '采集类怪物；不攻击目标';
  end;
  tsAttack.TabVisible := not(FCurrentMonsterCustomConfig.MonsterRace in [154, 159]);
  tsServerAttack.TabVisible := not(FCurrentMonsterCustomConfig.MonsterRace in [154, 159]);
  FIsMonsterChanged := OldIsConfigCanSave;
  if not FIsMonsterChanged then
    btnSave.Enabled := False;
end;

procedure TfrmMonsterConfig.SetMonsterConfigChanged(IsChanged: Boolean);
begin
  if FCurrentMonsterCustomConfig <> nil then
  begin
    FCurrentMonsterCustomConfig.SetChanged(IsChanged);
    if vstCustomMonster.FocusedNode <> nil then
      vstCustomMonster.InvalidateNode(vstCustomMonster.FocusedNode);
    FIsMonsterChanged := True;
    if not btnSave.Enabled then
      btnSave.Enabled := True;
  end;
end;

procedure TfrmMonsterConfig.btn1Click(Sender: TObject);
var
  FileName: string;
begin
  if g_Config.sCustomMonsterClientConfigFileName <> '' then
    dlgSaveMonsters.FileName := g_Config.sCustomMonsterClientConfigFileName;
  if not dlgSaveMonsters.Execute then
  begin
    // 指定保存目录会改变当前程序目录,
    SetCurrentDirectory(PChar(ExtractFileDir(Application.ExeName)));
    Exit;
  end;
  // 指定保存目录会改变当前程序目录,
  SetCurrentDirectory(PChar(ExtractFileDir(Application.ExeName)));
  FileName := dlgSaveMonsters.FileName;
  FileName := ChangeFileExt(FileName, '.dat');
  g_Config.sCustomMonsterClientConfigFileName := FileName;
  Config.WriteString('Setup', 'CustomMonsterClientConfigFileName', g_Config.sCustomMonsterClientConfigFileName);
  if g_MultiThreadRun then
    UserEngine.m_CustomMonsterList.LockR(6);
  try
    SaveCustomMonsterClientConfigs(UserEngine.m_CustomMonsterList, FileName);
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomMonsterList.UnLockR;
  end;
  Showmessage('已经生成自定义怪物登录器配置文件');
end;

procedure TfrmMonsterConfig.chkDamageLimitationClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDamageLimitation := chkDamageLimitation.Checked;
  ModValue();
end;

procedure TfrmMonsterConfig.chkDieDropBagItemClick(Sender: TObject);
begin
  if SelMonsterConfig <> nil then
    ButtonMonUseItemsChange.Enabled := True;
end;

procedure TfrmMonsterConfig.chkMoveTargetClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.MoveTarget := chkMoveTarget.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seMoveTargetRateChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.MoveTargetRate := seMoveTargetRate.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkMoveTargetHighLevelClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.MoveTargetHighLevel := chkMoveTargetHighLevel.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seAttackPowerIncChange(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.AttackPowerInc := seAttackPowerInc.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seElfWarriorMonsterDownDelayChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nElfWarriorMonsterDownDelay := seElfWarriorMonsterDownDelay.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.chkEnabledMaxMapItemCountClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnabledMaxMapItemCount := chkEnabledMaxMapItemCount.Checked;
  ModValue();
end;

procedure TfrmMonsterConfig.seMaxMapItemCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxMapItemCount := seMaxMapItemCount.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.chkNotDropOverlapItemAllClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNotDropOverlapItemAll := chkNotDropOverlapItemAll.Checked;
  ModValue();
end;

procedure TfrmMonsterConfig.seScatterItemRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nScatterItemRange := seScatterItemRange.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.seMonOneDropGoldCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonOneDropGoldCount := seMonOneDropGoldCount.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.chkDropGoldToPlayBagClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDropGoldToPlayBag := chkDropGoldToPlayBag.Checked;
  ModValue();
end;

procedure TfrmMonsterConfig.cbbMonsterShowLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMonsterShowLevel := cbbMonsterShowLevel.ItemIndex;
  edtMonsterShowFormat.Enabled := g_Config.btMonsterShowLevel > 0;
  ModValue();
end;

procedure TfrmMonsterConfig.vstDropLimitItemsGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
begin
  NodeDataSize := SizeOf(TDropLimitItem);
end;

procedure TfrmMonsterConfig.vstItemRulesGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
begin
  NodeDataSize := SizeOf(PDropItemRule);
end;

procedure TfrmMonsterConfig.vstDropLimitItemsGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  Item: PDropLimitItem;
begin
  Item := Sender.GetNodeData(Node);
  if Item <> nil then
    CellText := Item^.Name;
end;

procedure TfrmMonsterConfig.vstDropLimitItemsNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  I: Integer;
  DropLimitItem: PDropLimitItem;
  ItemRule: PDropItemRule;
begin
  if vstItemRules.IsEditing then
    vstItemRules.EndEditNode;
  vstItemRules.Clear;
  DropLimitItem := vstDropLimitItems.GetNodeData(vstDropLimitItems.FocusedNode);
  if DropLimitItem = nil then
  begin
    FCurrentItemRule := nil;
    RefreshDropLimitItemButtons;
    Exit;
  end;
  FCurrentLimitItem := DropLimitItem^;
  FCurrentItemRule := nil;
  for I := 0 to DropLimitItem^.Count - 1 do
  begin
    ItemRule := DropLimitItem^.Rules[I];
    vstItemRules.AddChild(nil, ItemRule);
  end;
  RefreshDropLimitItemButtons;
end;

procedure TfrmMonsterConfig.lstAllItemsDblClick(Sender: TObject);
var
  Index: Integer;
  ItemName: string;
  LimitItem: TDropLimitItem;
  Node: PVirtualNode;
  HitInfo: THitInfo;
  PLimitItem: PDropLimitItem;
begin
  if lstAllItems.ItemIndex < 0 then
  begin
    FCurrentLimitItem := nil;
    Exit;
  end;
  vstDropLimitItems.BeginUpdate;
  try
    Node := vstDropLimitItems.GetFirst();
    while Node <> nil do
    begin
      vstDropLimitItems.IsVisible[Node] := True;
      Node := vstDropLimitItems.GetNext(Node);
    end;
  finally
    vstDropLimitItems.EndUpdate;
  end;
  ItemName := lstAllItems.Items.Strings[lstAllItems.ItemIndex];
  if not g_DropLimitMgr.Search(ItemName, Index) then
  begin
    LimitItem := g_DropLimitMgr.AddItem(ItemName);
    Node := vstDropLimitItems.AddChild(nil, LimitItem);
    LimitItem.IsChanged := True;
    LimitItem.Save;
    vstDropLimitItems.FocusedNode := Node;
    vstDropLimitItems.Selected[Node] := True;
    if vstDropLimitItems.CanFocus then
      vstDropLimitItems.SetFocus;
    HitInfo.HitNode := Node;
    HitInfo.HitColumn := NoColumn;
    vstDropLimitItems.OnNodeClick(vstDropLimitItems, HitInfo);
  end
  else
  begin
    Node := vstDropLimitItems.GetFirst();
    while Node <> nil do
    begin
      PLimitItem := vstDropLimitItems.GetNodeData(Node);
      if SameText(PLimitItem.Name, ItemName) then
      begin
        vstDropLimitItems.FocusedNode := Node;
        vstDropLimitItems.Selected[Node] := True;
        if vstDropLimitItems.CanFocus then
          vstDropLimitItems.SetFocus;
        HitInfo.HitNode := Node;
        HitInfo.HitColumn := NoColumn;
        vstDropLimitItems.OnNodeClick(vstDropLimitItems, HitInfo);
        Exit;
      end;
      Node := vstDropLimitItems.GetNext(Node);
    end;
  end;
end;

procedure TfrmMonsterConfig.vstItemRulesGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  TextType: TVSTTextType; var CellText: string);
var
  ItemRule: PPDropItemRule;
  TempDate: TDateTime;
begin
  ItemRule := Sender.GetNodeData(Node);
  if ItemRule <> nil then
  begin
    case Column of
      0:
        CellText := ItemRule^.MapName;
      1:
        CellText := IntToStr(ItemRule^.ClearInterval);
      2:
        CellText := TIntervalTypeNames[ItemRule^.IntervalType];
      3:
        CellText := IntToStr(ItemRule^.DropInterval);
      4:
        CellText := IntToStr(ItemRule^.LimitCount);
      5:
        CellText := IntToStr(ItemRule^.DropedCount);
      6:
        begin
          if ItemRule^.LimitCount > 0 then
            CellText := IntToStr(Max(ItemRule^.LimitCount - ItemRule^.DropedCount, 0))
          else
            CellText := '-';
        end;
      7:
        begin
          if ItemRule^.ClearInterval > 0 then
          begin
            if ItemRule^.IntervalType = itDay then
              TempDate := ItemRule^.LastClearDate + ItemRule^.ClearInterval
            else if ItemRule^.IntervalType = itHour then
              TempDate := IncHour(ItemRule^.LastClearDate, ItemRule^.ClearInterval)
            else
              TempDate := IncMinute(ItemRule^.LastClearDate, ItemRule^.ClearInterval);
            CellText := FormatDateTime('yyyy/mm/dd', Trunc(TempDate))
          end
          else
            CellText := '-';
        end;
      8:
        begin
          if ItemRule^.ClearInterval > 0 then
          begin
            if ItemRule^.IntervalType = itDay then
              TempDate := ItemRule^.LastClearDate + ItemRule^.ClearInterval
            else if ItemRule^.IntervalType = itHour then
              TempDate := IncHour(ItemRule^.LastClearDate, ItemRule^.ClearInterval)
            else
              TempDate := IncMinute(ItemRule^.LastClearDate, ItemRule^.ClearInterval);
            CellText := FormatDateTime('hh:nn:ss', TempDate)
          end
          else
            CellText := '-';
        end;
      9:
        CellText := IntToStr(ItemRule^.AllDropedCount);
      10:
        begin
          CellText := '查看';
        end;
    end;
  end;
end;

procedure TfrmMonsterConfig.vstItemRulesCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  out EditLink: IVTEditLink);
begin
  EditLink := TItemRulePropertyEditLink.Create;
end;

procedure TfrmMonsterConfig.vstItemRulesEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex;
  var Allowed: Boolean);
begin
  Allowed := (Node <> nil) and ((Column <= 5) or (Column = 8));
end;

procedure TfrmMonsterConfig.vstItemRulesNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
var
  PPItemRule: PPDropItemRule;
begin
  if Assigned(HitInfo.HitNode) and (HitInfo.HitColumn >= 0) then
  begin
    PPItemRule := vstItemRules.GetNodeData(HitInfo.HitNode);
    if PPItemRule <> nil then
    begin
      if (HitInfo.HitColumn = 10) and (FCurrentLimitItem <> nil) then
      begin
        ShowFrmItemDropLog(FCurrentLimitItem.Name, PPItemRule^.MapName);
      end
      else
      begin
        FCurrentItemRule := PPItemRule^;
        RefreshDropLimitItemButtons;
        PostMessage(Self.Handle, WM_STARTEDITING_ITEMRULE, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
      end;
    end;
  end;
end;

procedure TfrmMonsterConfig.WMStartEditingItemRule(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WPARAM);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstItemRules.EditNode(Node, Message.LParam);
end;

procedure TfrmMonsterConfig.vstItemRulesKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  ItemRule: TDropItemRule;
  PItemRule: PDropItemRule;
  PPItemRule: PPDropItemRule;
  Node: PVirtualNode;
begin
  if (ssCtrl in Shift) then
  begin
    if Key = VK_INSERT then
    begin
      if FCurrentLimitItem = nil then
        Exit;
      ItemRule.MapName := '*';
      ItemRule.ClearInterval := 1;
      ItemRule.IntervalType := itDay;
      ItemRule.DropInterval := 0;
      ItemRule.LimitCount := 1;
      ItemRule.DropedCount := 0;
      ItemRule.LastClearDate := Now;
      ItemRule.AllDropedCount := 0;
      ItemRule.LastDropTime := 0;
      PItemRule := FCurrentLimitItem.Add(ItemRule);
      Node := vstItemRules.AddChild(nil, PItemRule);
      vstItemRules.Selected[Node] := True;
      vstItemRules.FocusedNode := Node;
      FCurrentLimitItem.Save;
      vstItemRules.EditNode(Node, 0);
    end
    else if Key = VK_DELETE then
    begin
      if vstItemRules.FocusedNode <> nil then
      begin
        PPItemRule := vstItemRules.GetNodeData(vstItemRules.FocusedNode);
        Node := vstItemRules.GetNext(vstItemRules.FocusedNode);
        if Node = nil then
          Node := vstItemRules.GetPrevious(vstItemRules.FocusedNode);
        if FCurrentLimitItem.Remove(PPItemRule^) then
        begin
          vstItemRules.DeleteNode(vstItemRules.FocusedNode);
          if Node <> nil then
          begin
            vstItemRules.FocusedNode := Node;
            vstItemRules.Selected[Node] := True;
            PPItemRule := vstItemRules.GetNodeData(Node);
            if PPItemRule <> nil then
            begin
              FCurrentItemRule := PPItemRule^;
            end;
            RefreshDropLimitItemButtons;
          end
          else
          begin
            FCurrentItemRule := nil;
            RefreshDropLimitItemButtons;
          end;
          FCurrentLimitItem.Save;
        end;
      end;
    end;
  end;
end;

procedure TfrmMonsterConfig.RefreshDropLimitItemButtons;
begin
  // btnAddItemRule.Enabled := FCurrentLimitItem <> nil;
  btnClearItemRule.Enabled := FCurrentLimitItem <> nil;
  chkRecordLog.Enabled := FCurrentLimitItem <> nil;
  // btnEditItemRule.Enabled := FCurrentItemRule <> nil;
  // btnDelItemRule.Enabled := FCurrentItemRule <> nil;
  if FCurrentLimitItem <> nil then
  begin
    chkRecordLog.Checked := FCurrentLimitItem.IsRecordLog
  end
  else
  begin
    chkRecordLog.Checked := False;
  end;
  {
    if FCurrentItemRule <> nil then
    begin
    cbbMaps.Text := FCurrentItemRule.MapName;
    seItemLimitDays.Value := FCurrentItemRule.ClearDays;
    seItemLimitCount.Value := FCurrentItemRule.LimitCount;
    end
    else
    begin
    cbbMaps.Text := '';
    seItemLimitDays.Value := 0;
    seItemLimitCount.Value := 0;
    end;
  }
end;

procedure TfrmMonsterConfig.btnAddItemRuleClick(Sender: TObject);
begin
  if FCurrentLimitItem = nil then
    Exit;
end;

procedure TfrmMonsterConfig.btnClearItemRuleClick(Sender: TObject);
begin
  if FCurrentLimitItem <> nil then
  begin
    vstItemRules.Clear;
    FCurrentLimitItem.Clear;
    FCurrentLimitItem.Save;
  end;
end;

procedure TfrmMonsterConfig.Button1Click(Sender: TObject);
begin
  if FCurrentLimitItem <> nil then
  begin
    vstDropLimitItems.DeleteNode(vstDropLimitItems.FocusedNode);
    g_DropLimitMgr.Remove(FCurrentLimitItem);
    vstItemRules.Clear;
    FCurrentLimitItem := nil;
    FCurrentItemRule := nil;
    RefreshDropLimitItemButtons;
  end;
end;

procedure TfrmMonsterConfig.lstAllItemsKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  I: Integer;
  sItemName: string;
begin
  case Key of
    Word('F'):
      begin
        if ssCtrl in Shift then
        begin
          Key := 0;
          sItemName := '';
          if not InputQuery('物品查找', '输入物品名称:', sItemName) then
            Exit;
          if sItemName = '' then
            Exit;
          for I := 0 to TListBox(Sender).Items.Count - 1 do
          begin
            if TListBox(Sender).Items.Strings[I] = sItemName then
            begin
              TListBox(Sender).ItemIndex := I;
              Break;
            end;
          end;
        end;
      end;
  end;
end;

procedure TfrmMonsterConfig.edtItemSearchChange(Sender: TObject);
var
  Node: PVirtualNode;
  LimitItem: PDropLimitItem;
begin
  if Length(edtItemSearch.Text) = 0 then
  begin
    vstDropLimitItems.BeginUpdate;
    try
      Node := vstDropLimitItems.GetFirst();
      while Node <> nil do
      begin
        vstDropLimitItems.IsVisible[Node] := True;
        Node := vstDropLimitItems.GetNext(Node);
      end;
    finally
      vstDropLimitItems.EndUpdate;
    end;
  end
  else
  begin
    vstDropLimitItems.BeginUpdate;
    try
      Node := vstDropLimitItems.GetFirst();
      while Node <> nil do
      begin
        LimitItem := vstDropLimitItems.GetNodeData(Node);
        if Pos(edtItemSearch.Text, LimitItem.Name) > 0 then
        begin
          vstDropLimitItems.IsVisible[Node] := True;
        end
        else
        begin
          vstDropLimitItems.IsVisible[Node] := False;
        end;
        Node := vstDropLimitItems.GetNext(Node);
      end;
    finally
      vstDropLimitItems.EndUpdate;
    end;
  end;
end;

procedure TfrmMonsterConfig.edtItemSearchKeyPress(Sender: TObject; var Key: Char);
begin
  if Key = Chr(VK_RETURN) then
  begin
    edtItemSearchChange(edtItemSearch);
    Key := #0;
  end;
end;

procedure TfrmMonsterConfig.chkRecordLogClick(Sender: TObject);
begin
  if FCurrentLimitItem <> nil then
  begin
    FCurrentLimitItem.IsRecordLog := chkRecordLog.Checked;
    FCurrentLimitItem.Save;
  end;
end;

procedure TfrmMonsterConfig.pmItemRulesPopup(Sender: TObject);
begin
  miItemRulesLog.Visible := FCurrentItemRule <> nil;
end;

procedure TfrmMonsterConfig.vstItemRulesDrawText(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode;
  Column: TColumnIndex; const Text: string; const CellRect: TRect; var DefaultDraw: Boolean);
begin
  if Column = 10 then
    TargetCanvas.Font.Color := clBlue;
end;

procedure TfrmMonsterConfig.chkClientExplosionKeepPlayClick(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_KeepPlay := chkClientExplosionKeepPlay.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientExplosionKeepTimeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_KeepTime := seClientExplosionKeepTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientExplosionKeepAttackIntervalChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_KeepAttackInterval := seClientExplosionKeepAttackInterval.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientExplosionKeepAttackRangeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_KeepAttackRange := seClientExplosionKeepAttackRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seExplosionKeepLightRangeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_KeepLightRange := seExplosionKeepLightRange.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.chkClientExplosionKeepMultiPlayClick(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.Explosion_KeepMultiPlay := chkClientExplosionKeepMultiPlay.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seMonStruckFrameDelayTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMonStruckFrameDelayTime := seMonStruckFrameDelayTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.chkNearAttackTargetCenterClick(Sender: TObject);
begin
  if FCurrentMonsterServerConfig <> nil then
  begin
    FCurrentMonsterServerConfig.NearAttackTargetCenter := chkNearAttackTargetCenter.Checked;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.edtMagStruckMonLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagStruckMonLevel := edtMagStruckMonLevel.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.edtMagStruckMonDecTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagStruckMonDecTime := edtMagStruckMonDecTime.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.edtMagStruckMonDecRandomChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagStruckMonDecRandom := edtMagStruckMonDecRandom.Value;
  ModValue();
end;

procedure TfrmMonsterConfig.cbbClientSelfKeepFileChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.SelfKeep_File := cbbClientSelfKeepFile.ItemIndex - 1;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientSelfKeepStartIndexChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.SelfKeep_StartIndex := seClientSelfKeepStartIndex.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientSelfKeepStartIndex2Change(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.SelfKeep_StartIndex2 := seClientSelfKeepStartIndex2.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientSelfKeepPlayCountChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.SelfKeep_PlayCount := seClientSelfKeepPlayCount.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientSelfKeepPlayTimeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.SelfKeep_PlayTime := seClientSelfKeepPlayTime.Value;
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientSelfKeepDrawOrderChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.SelfKeep_DrawOrder := TCustomDrawOrder(cbbClientSelfKeepDrawOrder.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientSelfKeepDrawMode2Change(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.SelfKeep_DrawMode2 := TCustomDrawMode(cbbClientSelfKeepDrawMode2.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.cbbClientSelfKeepDrawModeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.SelfKeep_DrawMode := TCustomDrawMode(cbbClientSelfKeepDrawMode.ItemIndex);
    SetMonsterConfigChanged();
  end;
end;

procedure TfrmMonsterConfig.seClientSelfKeepTimeChange(Sender: TObject);
begin
  if FCurrentMonsterClientConfig <> nil then
  begin
    FCurrentMonsterClientConfig.SelfKeep_KeepTime := seClientSelfKeepTime.Value;
    SetMonsterConfigChanged();
  end;
end;

end.

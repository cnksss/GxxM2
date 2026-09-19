unit JSYConfigDlg;

interface

uses
  Windows,
  Messages,
  SysUtils,
  StrUtils,
  Variants,
  Classes,
  Graphics,
  Controls,
  Forms,
  DxComponents,
  GameConfigDlg,
  FilterItems,
  Dialogs,
  StdCtrls,
  ComCtrls,
  ExtCtrls,
  Spin,
  HUtil32,
  Grobal2,
  HGE,
  Math,
  SDK,
  ColorIndexEdit,
  SpinEditEx,
  CommCtrl,
  RzTabs,
  GlobalString,
  Menus,
  FastIniFile,
  TypInfo,
  RzButton,
  RzRadChk,
  Mask,
  RzEdit,
  RzTrkBar,
  RzPanel,
  RzLstBox,
  RzCmboBx,
  RzListVw,
  RzLabel,
  uFrmNGItemEdit;

type
  TRunFun = function:Boolean;

  TJSYConfigDlg = class(TGameConfigObject)
  private
    FProtectEnabled:Boolean;
    FProtectEnabledTick:LongWord;
    FLoadControl:Boolean;
    FLoadConfig:Boolean;
    FHandle:THandle;
    FScreenMode:Byte;
    FClientVersion:TClientVersion;
    FWindowMode:Boolean;

    FCharName:string;
    FLoadConfigDlg:Boolean;
    FEnabled:Boolean;
    FConfigCheckeds:array[TConfigChecked] of Boolean;
    FInitializeed:Boolean;

    //FProtectList: TStringList;
    FHintItemDuraTick:LongWord;
    FClientConfig:TClientConfig;

    procedure RefConfig;
    procedure DuraWarning(); // 持久警告
    procedure AutoProtect(Sender:TObject);
    procedure AutoUseItem(Sender:TObject);
    procedure AutoEatHPItem(Sender:TObject);
    procedure AutoEatMPItem(Sender:TObject);
    //procedure AutoEatSpecialHPItem(Sender: TObject);

    procedure LoadConfigFile();
    procedure SaveConfigFile();
  public
    function GetType:TConfigDlgType; override;
    function GetConfigChecked(Index:TConfigChecked):Boolean; override;
    procedure SetConfigChecked(Index:TConfigChecked; Value:Boolean); override;
    function GetVisible:Boolean; override;
    procedure SetVisible(Value:Boolean); override;
    function GetEnabled:Boolean; override;
    procedure SetEnabled(Value:Boolean); override;
    function GetProtectEnabled:Boolean; override;
    procedure SetProtectEnabled(Value:Boolean); override;
    procedure Open; override;
    procedure Close; override;

    procedure LoadConfig(const CharName:string); override;
    procedure Initialize(Handle:THandle; ScreenMode:Byte; ClientVersion:TClientVersion; WindowMode:Boolean); override;
    procedure Finalize; override;
    procedure Logon(const ServerName:string); override;
    procedure Logout; override;
    function FormKeyDown(var Key:Word; Shift:TShiftState):Boolean; override;
    function FormKeyPress(var Key:Char):Boolean; override;
    procedure RefreshMySelfAbil; override;
    procedure RefreshMyHeroAbil; override;
    procedure RefreshMySelfMagicList; override;
    procedure RefreshMyHeroMagicList; override;
    procedure RefreshUnBindItemList; override;
    procedure Struck(Actor:TObject; HP, MaxHP:LongInt); override;
    procedure HealthChange(Actor:TObject; HP, MP, MaxHP:LongInt); override;
    procedure LoadClientConfig(ClientConfig:pTClientConfig); override;
    procedure ClearShowItem; override;
    procedure RefShowItem; override;
    procedure Run; override;
    procedure RefActorList; override;
    function CanFilterExp(Exp:LongWord):Boolean; override;
    function GetShowItem(const ItemName:string):pTShowItem; override;
    function FindShowItem(const ItemName:string):Boolean; override;
    function FindHintItem(const ItemName:string):Boolean; override;
    function FindPickItem(const ItemName:string):Boolean; override;
    procedure HintItem(const ItemName:string; X, Y:Integer); override;

    procedure AddToBossList(sName:string); override;
    procedure RemoveFromBossList(sName:string); override;
    procedure AddOrRemoveBossList(sName:string); override;
    procedure RefreshHotKeys;


    procedure RefKeyboardConfig; override;

    function AutoUseMagic:Boolean;

    constructor Create();
    destructor Destroy; override;
  end;

  TTrackBar = class(ComCtrls.TTrackBar)
  protected
    procedure CreateParams(var Params:TCreateParams); override;
  end;

  TFrmJSYDlg = class(TForm)
    pgMain:TRzPageControl;
    tsBase:TRzTabSheet;
    lblVolume:TRzLabel;
    trbVolume:TRzTrackBar;
    chkBGMusic:TRzCheckBox;
    chkRepeatBGMusic:TRzCheckBox;
    chkShowGreenHint:TRzCheckBox;
    chkAutoOrderItem:TRzCheckBox;
    chkOnlyShowCharName:TRzCheckBox;
    chkShowHPLabel:TRzCheckBox;
    chkShowNumberLable:TRzCheckBox;
    chkShowJobAndLevel:TRzCheckBox;
    chkNotNeedShift:TRzCheckBox;
    chkShowUserName:TRzCheckBox;
    chkShowMoveLable:TRzCheckBox;
    chkShowMapDesc:TRzCheckBox;
    chkShowHighlightHPLabel:TRzCheckBox;
    chkShowNpcHPLabel:TRzCheckBox;
    grpMapRadar:TRzGroupBox;
    chkShowRadarPlayer:TRzCheckBox;
    chkShowRadarNpc:TRzCheckBox;
    chkShowRadarAttackNpc:TRzCheckBox;
    chkShowRadarActor:TRzCheckBox;
    chkExpFilter:TRzCheckBox;
    edtExpFilter:TRzEdit;
    chkShowMonName:TRzCheckBox;
    chkSpeedSlow:TRzCheckBox;
    chkHideHumEffect:TRzCheckBox;
    chkHideWeaponEffect:TRzCheckBox;
    chkShowNPCName:TRzCheckBox;
    chkShowNGLabel:TRzCheckBox;
    chkShiftSwitch:TRzCheckBox;
    chkHideTitle:TRzCheckBox;
    chkDisableChartMemoSize:TRzCheckBox;
    chkContinueButchItem:TRzCheckBox;
    chkDisableDeal:TRzCheckBox;
    chkShowUpdateStatus:TRzCheckBox;
    chkSimpleShowActor:TRzCheckBox;
    chkSimpleShowHumanDress:TRzCheckBox;
    tsMagic:TRzTabSheet;
    grpJob0:TRzGroupBox;
    chkSmartLongHit:TRzCheckBox;
    chkSmartFireHit:TRzCheckBox;
    chkSmartWideHit:TRzCheckBox;
    chkSmartPosLongHit:TRzCheckBox;
    chkSmartKTZHit:TRzCheckBox;
    chkSmartSwordHit:TRzCheckBox;
    chkSmartWalkLongHit:TRzCheckBox;
    chkSmartCRSHit:TRzCheckBox;
    chkSmartTWNHit:TRzCheckBox;
    chkHumManuallyMove10Attack:TRzCheckBox;
    grpJob1:TRzGroupBox;
    chkHumAutoShield:TRzCheckBox;
    chkHumStruckShield:TRzCheckBox;
    chkHumManuallySnowWind:TRzCheckBox;
    chkHumManuallyFireBoom:TRzCheckBox;
    chkHumShootLightenLockTarget:TRzCheckBox;
    chkHumManuallyMeteorShower:TRzCheckBox;
    grpJob2:TRzGroupBox;
    CheckBoxAutoChangePoison:TRzCheckBox;
    CheckBoxAutoHideMode:TRzCheckBox;
    grpJobHero:TRzGroupBox;
    chkAssistantHeroAutoShield:TRzCheckBox;
    chkHeroAutoShield:TRzCheckBox;
    tsProtect:TRzTabSheet;
    pgProtect:TRzPageControl;
    tsProtectHuman:TRzTabSheet;
    grpHumProtected:TRzGroupBox;
    Label3:TRzLabel;
    Label4:TRzLabel;
    Label5:TRzLabel;
    Label6:TRzLabel;
    Label7:TRzLabel;
    Label8:TRzLabel;
    Label9:TRzLabel;
    Label10:TRzLabel;
    Label11:TRzLabel;
    Label12:TRzLabel;
    Label13:TRzLabel;
    Label14:TRzLabel;
    Label22:TRzLabel;
    Label23:TRzLabel;
    CheckBoxRenewSpecialIsAuto:TRzCheckBox;
    CheckBoxRenewBookIsAuto1:TRzCheckBox;
    CheckBoxRenewBookIsAuto2:TRzCheckBox;
    CheckBoxRenewMPIsAuto:TRzCheckBox;
    CheckBoxRenewHPIsAuto2:TRzCheckBox;
    CheckBoxRenewHPIsAuto1:TRzCheckBox;
    ComboBoxRenewHPIsAuto1:TRzComboBox;
    EditRenewHPPercent1:TRzEdit;
    EditRenewHPTime1:TRzEdit;
    ComboBoxRenewHPIsAuto2:TRzComboBox;
    EditRenewHPPercent2:TRzEdit;
    EditRenewHPTime2:TRzEdit;
    ComboBoxRenewMPIsAuto:TRzComboBox;
    EditRenewMPPercent:TRzEdit;
    EditRenewMPTime:TRzEdit;
    EditRenewBookTime2:TRzEdit;
    EditRenewBookPercent2:TRzEdit;
    ComboBoxRenewBookIsAuto2:TRzComboBox;
    ComboBoxRenewBookIsAuto1:TRzComboBox;
    EditRenewBookPercent1:TRzEdit;
    EditRenewBookTime1:TRzEdit;
    EditRenewSpecialTime:TRzEdit;
    EditRenewSpecialPercent:TRzEdit;
    ComboBoxRenewSpecialIsAuto:TRzComboBox;
    CheckBoxRenewLogOut:TRzCheckBox;
    EditRenewLogOutPercent:TRzEdit;
    EditRenewLogOuttime:TRzEdit;
    chkPercentProtect:TRzCheckBox;
    grpCustomItem:TRzGroupBox;
    Label28:TRzLabel;
    Label29:TRzLabel;
    lbl9:TRzLabel;
    lstUnBindItemList:TRzListBox;
    edtItemName:TRzEdit;
    btnBindItemAdd:TRzButton;
    btnBindItemChg:TRzButton;
    btnBindItemDel:TRzButton;
    btnBindItemSave:TRzButton;
    edtBindItemName:TRzEdit;
    cbbGroupUnBindItem:TRzComboBox;
    tsProtectHero:TRzTabSheet;
    lbl4:TRzLabel;
    grpHeroProtected:TRzGroupBox;
    Label16:TRzLabel;
    Label17:TRzLabel;
    Label18:TRzLabel;
    Label19:TRzLabel;
    Label20:TRzLabel;
    Label21:TRzLabel;
    Label24:TRzLabel;
    Label25:TRzLabel;
    Label26:TRzLabel;
    Label27:TRzLabel;
    CheckBoxRenewHeroSpecialIsAuto:TRzCheckBox;
    CheckBoxRenewHeroMPIsAuto:TRzCheckBox;
    CheckBoxRenewHeroHPIsAuto2:TRzCheckBox;
    CheckBoxRenewHeroHPIsAuto1:TRzCheckBox;
    ComboBoxRenewHeroHPIsAuto1:TRzComboBox;
    EditRenewHeroHPPercent1:TRzEdit;
    EditRenewHeroHPTime1:TRzEdit;
    ComboBoxRenewHeroHPIsAuto2:TRzComboBox;
    EditRenewHeroHPPercent2:TRzEdit;
    EditRenewHeroHPTime2:TRzEdit;
    ComboBoxRenewHeroMPIsAuto:TRzComboBox;
    EditRenewHeroMPPercent:TRzEdit;
    EditRenewHeroMPTime:TRzEdit;
    EditRenewHeroSpecialTime:TRzEdit;
    EditRenewHeroSpecialPercent:TRzEdit;
    ComboBoxRenewHeroSpecialIsAuto:TRzComboBox;
    CheckBoxRenewHeroLogOutIsAuto:TRzCheckBox;
    EditRenewHeroLogOutPercent:TRzEdit;
    EditRenewHeroLogOutTime:TRzEdit;
    chkHeroPercentProtect:TRzCheckBox;
    tsFight:TRzTabSheet;
    grpBoss:TRzGroupBox;
    lbl1:TRzLabel;
    lstBoss:TRzListBox;
    edtBossName:TRzEdit;
    btnBossAdd:TRzButton;
    btnBossDel:TRzButton;
    chkNearHint:TRzCheckBox;
    chkAutoLock:TRzCheckBox;
    chkColorShow:TRzCheckBox;
    cbbShowColor:TRzComboBox;
    BtnBossModify:TRzButton;
    grpPractice:TRzGroupBox;
    Label15:TRzLabel;
    CheckBoxAutoUseMagic:TRzCheckBox;
    EditAutoUseMagic:TSpinEditEx;
    ComboBoxAutoUseMagic:TRzComboBox;
    grpAutoSay:TRzGroupBox;
    lbl2:TRzLabel;
    lbl8:TRzLabel;
    chkAutoSysMsg:TRzCheckBox;
    edtSysMsg:TRzEdit;
    seAutoMsgTime:TSpinEditEx;
    grpFightOther:TRzGroupBox;
    chkMagicLock:TRzCheckBox;
    chkDisableSelfStruck:TRzCheckBox;
    chkBlacklistHit:TRzCheckBox;
    chkFriendHit:TRzCheckBox;
    chkAutoDownHorse:TRzCheckBox;
    tsItem:TRzTabSheet;
    lvFilterItem:TRzListView;
    ComboBoxItemStdMode:TRzComboBox;
    EditSearchItem:TRzEdit;
    ButtonSearchItem:TRzButton;
    edtSpecialName:TRzEdit;
    btnSpecialAdd:TRzButton;
    btnSpecialDel:TRzButton;
    grp9:TRzGroupBox;
    lbl7:TRzLabel;
    lbl6:TRzLabel;
    Label30:TRzLabel;
    chkCheckDura:TRzCheckBox;
    seCheckDuraMin:TSpinEditEx;
    edtCheckDuraItem:TRzEdit;
    seCheckDuraTime:TSpinEditEx;
    tsNpc:TRzTabSheet;
    lvActor:TRzListView;
    chkShowHum:TRzCheckBox;
    chkShowMon:TRzCheckBox;
    chkShowNpc:TRzCheckBox;
    btnRefActorList:TRzButton;
    tsGJ:TRzTabSheet;
    lbl3:TRzLabel;
    Label2:TRzLabel;
    chkNoRedPoison:TRzCheckBox;
    chkNoBluePoison:TRzCheckBox;
    chkNoDuFu:TRzCheckBox;
    chkPlayAttack:TRzCheckBox;
    cbbPlayAttackOption:TRzComboBox;
    chkNotRushMon:TRzCheckBox;
    seNotRushMonRange:TSpinEdit;
    pgcGJSetting:TRzPageControl;
    tsGJNoAttackMon:TRzTabSheet;
    Label1:TRzLabel;
    lbl5:TRzLabel;
    lstGJMon:TRzListBox;
    edtGJMon:TRzEdit;
    btnGJMonAdd:TRzButton;
    btnGJMonDel:TRzButton;
    btnGJMonEdit:TRzButton;
    tsGJMagic:TRzTabSheet;
    lvGJMagic1:TRzListView;
    tsGJGroupMagic:TRzTabSheet;
    lvGJMagic2:TRzListView;
    chkGroupAttack:TRzCheckBox;
    seGroupAttackCount:TSpinEditEx;
    chkBagFull:TRzCheckBox;
    chkLimitScreen:TRzCheckBox;
    btnGJRun:TRzButton;
    chkAutoPickup:TRzCheckBox;
    chkDFAvoid:TRzCheckBox;
    cbbNoRedPoisonOption:TRzComboBox;
    cbbNoBluePoisonOption:TRzComboBox;
    cbbNoDuFuOption:TRzComboBox;
    cbbBagFullOption:TRzComboBox;
    btnGJPoint:TRzButton;
    tsKey:TRzTabSheet;
    scbHotKey:TScrollBox;
    lblKey1:TRzLabel;
    lblKey2:TRzLabel;
    lblKey3:TRzLabel;
    lblKey4:TRzLabel;
    lblKey5:TRzLabel;
    lblKey6:TRzLabel;
    lblKey7:TRzLabel;
    lblKey8:TRzLabel;
    lblKey9:TRzLabel;
    lblKey10:TRzLabel;
    lblKey11:TRzLabel;
    lblKey12:TRzLabel;
    lblKey13:TRzLabel;
    lblKey14:TRzLabel;
    lblKey15:TRzLabel;
    hkNormal1:TRzEdit;
    hkNormal2:TRzEdit;
    hkNormal3:TRzEdit;
    hkNormal4:TRzEdit;
    hkNormal5:TRzEdit;
    hkNormal6:TRzEdit;
    hkNormal7:TRzEdit;
    hkNormal8:TRzEdit;
    pnlHotKeyTop:TRzPanel;
    chkEnabledHotKey:TRzCheckBox;
    tsHelp:TRzTabSheet;
    mmoHelp:TRzMemo;
    hkNormal9:TRzEdit;
    hkNormal10:TRzEdit;
    hkNormal11:TRzEdit;
    hkNormal12:TRzEdit;
    hkNormal13:TRzEdit;
    hkNormal14:TRzEdit;
    hkNormal15:TRzEdit;
    lbl23:TRzLabel;
    Label33:TRzLabel;
    Label34:TRzLabel;
    bvl1:TBevel;
    pnlHotKeyBottom:TRzPanel;
    hkCustom1:TRzEdit;
    hkCustom2:TRzEdit;
    hkCustom3:TRzEdit;
    hkCustom4:TRzEdit;
    hkCustom5:TRzEdit;
    hkCustom6:TRzEdit;
    hkCustom7:TRzEdit;
    hkCustom8:TRzEdit;
    hkCustom9:TRzEdit;
    hkCustom10:TRzEdit;
    hkCustom11:TRzEdit;
    hkCustom12:TRzEdit;
    hkCustom13:TRzEdit;
    hkCustom14:TRzEdit;
    hkCustom15:TRzEdit;
    tsNotes:TRzTabSheet;
    mmoNotes:TRzMemo;
    chkAutoGroupAttack:TRzCheckBox;
    chkAutoGroupNoAttackMon:TRzCheckBox;
    lblKey16:TRzLabel;
    hkNormal16:TRzEdit;
    hkCustom16:TRzEdit;
    chkHumManuallyFire:TRzCheckBox;
    chkShowHPUnit:TRzCheckBox;
    pm1:TPopupMenu;
    N1:TMenuItem;
    mniEnable:TMenuItem;
    mniDisable:TMenuItem;
    mniReverse:TMenuItem;
    N2:TMenuItem;
    N3:TMenuItem;
    N4:TMenuItem;
    N5:TMenuItem;
    N6:TMenuItem;
    N7:TMenuItem;
    N8:TMenuItem;
    N9:TMenuItem;
    N10:TMenuItem;
    N11:TMenuItem;
    N12:TMenuItem;
    N13:TMenuItem;
    N14:TMenuItem;
    N15:TMenuItem;
    N16:TMenuItem;
    N17:TMenuItem;
    shp2:TShape;
    shp1:TShape;
    shp3:TShape;
    grpItemSetting:TRzGroupBox;
    chkSpecialQuickFlashing:TRzCheckBox;
    chkItemCompare:TRzCheckBox;
    chkBagFastItemCompare:TRzCheckBox;
    chkHideItemEffect:TRzCheckBox;
    chkPickupAll:TRzCheckBox;
    seSpecialColor:TColorIndexEdit;
    lbl10:TRzLabel;
    chkHideGhost:TRzCheckBox;
    chkAutoPickupItem:TRzCheckBox;
    chkHeroShowNumberState:TRzCheckBox;
    lblItemsExport:TRzLabel;
    lblItemImport:TRzLabel;
    chkShowTargetAperture:TRzCheckBox;
    chkNoCaton:TRzCheckBox;
    chkSmart113Hit:TRzCheckBox;
    chkAutoOpenSpell:TRzCheckBox;
    chkNotParaly:TRzCheckBox;
    lblItemEdit:TRzLabel;
    chkSmartCustomHit1:TRzCheckBox;
    chkSmartCustomHit4:TRzCheckBox;
    chkSmartCustomHit2:TRzCheckBox;
    chkSmartCustomHit3:TRzCheckBox;
    chkSmartCustomHit5:TRzCheckBox;
    chkSmartCustomHit6:TRzCheckBox;
    chkSmartCustomHit7:TRzCheckBox;
    chkSmartCustomHit8:TRzCheckBox;
    chkHumManuallyCustomHit1:TRzCheckBox;
    chkHumManuallyCustomHit2:TRzCheckBox;
    chkHumManuallyCustomHit3:TRzCheckBox;
    chkHumManuallyCustomHit4:TRzCheckBox;
    chkHumManuallyCustomHit5:TRzCheckBox;
    chkShowValueItemEffect:TRzCheckBox;
    chkDuraWarning:TRzCheckBox;
    chkHideActorIcons:TRzCheckBox;
    chkAutoContinueAttack:TRzCheckBox;
    chkSceneShake:TRzCheckBox;
    chkSimpleShowHumanWeapon:TRzCheckBox;
    chkHideMonsterIcons:TRzCheckBox;
    chkAutoDetourPath:TRzCheckBox;
    chkDimFireEffect:TRzCheckBox;
    chkSimpleShowBB:TRzCheckBox;
    chkNearEffect: TRzCheckBox;
    procedure FormClose(Sender:TObject; var Action:TCloseAction);
    procedure FormCloseQuery(Sender:TObject; var CanClose:Boolean);
    procedure FormKeyDown(Sender:TObject; var Key:Word;
      Shift:TShiftState);
    procedure FormCreate(Sender:TObject);
    procedure FormKeyPress(Sender:TObject; var Key:Char);
    procedure chkBGMusicClick(Sender:TObject);
    procedure chkRepeatBGMusicClick(Sender:TObject);
    procedure chkShowGreenHintClick(Sender:TObject);
    procedure chkAutoOrderItemClick(Sender:TObject);
    procedure chkOnlyShowCharNameClick(Sender:TObject);
    procedure chkDuraWarningClick(Sender:TObject);
    procedure chkNotNeedShiftClick(Sender:TObject);
    procedure chkShowHPLabelClick(Sender:TObject);
    procedure chkShowNumberLableClick(Sender:TObject);
    procedure chkShowJobAndLevelClick(Sender:TObject);
    procedure chkShowMoveLableClick(Sender:TObject);
    procedure chkShowMapDescClick(Sender:TObject);
    procedure chkShowHighlightHPLabelClick(Sender:TObject);
    procedure chkSmartLongHitClick(Sender:TObject);
    procedure chkSmartFireHitClick(Sender:TObject);
    procedure chkSmartWideHitClick(Sender:TObject);
    procedure chkSmartPosLongHitClick(Sender:TObject);
    procedure chkSmartSwordHitClick(Sender:TObject);
    procedure chkSmartWalkLongHitClick(Sender:TObject);
    procedure chkHumAutoShieldClick(Sender:TObject);
    procedure chkHumStruckShieldClick(Sender:TObject);
    procedure chkHumManuallySnowWindClick(Sender:TObject);
    procedure chkHumManuallyFireBoomClick(Sender:TObject);
    procedure chkHumShootLightenLockTargetClick(Sender:TObject);
    procedure chkHumManuallyMeteorShowerClick(Sender:TObject);
    procedure CheckBoxAutoHideModeClick(Sender:TObject);
    procedure chkMagicLockClick(Sender:TObject);
    procedure chkDisableSelfStruckClick(Sender:TObject);
    procedure chkHideGhostClick(Sender:TObject);
    procedure CheckBoxAutoUseMagicClick(Sender:TObject);
    procedure lvFilterItemMouseDown(Sender:TObject;
      Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
    procedure btnRefActorListClick(Sender:TObject);
    procedure EditSearchItemChange(Sender:TObject);
    procedure ButtonSearchItemClick(Sender:TObject);
    procedure ComboBoxItemStdModeSelect(Sender:TObject);
    procedure chkExpFilterClick(Sender:TObject);
    procedure chkShowMonNameClick(Sender:TObject);
    procedure chkShowRadarPlayerClick(Sender:TObject);
    procedure chkShowRadarNpcClick(Sender:TObject);
    procedure chkShowRadarAttackNpcClick(Sender:TObject);
    procedure edtExpFilterChange(Sender:TObject);
    procedure CheckBoxRenewHPIsAuto1Click(Sender:TObject);
    procedure CheckBoxRenewHPIsAuto2Click(Sender:TObject);
    procedure CheckBoxRenewMPIsAutoClick(Sender:TObject);
    procedure CheckBoxRenewSpecialIsAutoClick(Sender:TObject);
    procedure CheckBoxRenewBookIsAuto1Click(Sender:TObject);
    procedure CheckBoxRenewBookIsAuto2Click(Sender:TObject);
    procedure CheckBoxRenewLogOutClick(Sender:TObject);
    procedure CheckBoxRenewHeroHPIsAuto1Click(Sender:TObject);
    procedure CheckBoxRenewHeroHPIsAuto2Click(Sender:TObject);
    procedure CheckBoxRenewHeroMPIsAutoClick(Sender:TObject);
    procedure CheckBoxRenewHeroSpecialIsAutoClick(Sender:TObject);
    procedure CheckBoxRenewHeroLogOutIsAutoClick(Sender:TObject);
    procedure ComboBoxRenewHPIsAuto1Select(Sender:TObject);
    procedure ComboBoxRenewHPIsAuto2Select(Sender:TObject);
    procedure ComboBoxRenewMPIsAutoSelect(Sender:TObject);
    procedure ComboBoxRenewSpecialIsAutoSelect(Sender:TObject);
    procedure ComboBoxRenewBookIsAuto1Select(Sender:TObject);
    procedure ComboBoxRenewBookIsAuto2Select(Sender:TObject);
    procedure ComboBoxRenewHeroHPIsAuto1Select(Sender:TObject);
    procedure ComboBoxRenewHeroHPIsAuto2Select(Sender:TObject);
    procedure ComboBoxRenewHeroMPIsAutoSelect(Sender:TObject);
    procedure ComboBoxRenewHeroSpecialIsAutoSelect(Sender:TObject);
    procedure EditRenewHPPercent1Change(Sender:TObject);
    procedure EditRenewHPPercent2Change(Sender:TObject);
    procedure EditRenewMPPercentChange(Sender:TObject);
    procedure EditRenewSpecialPercentChange(Sender:TObject);
    procedure EditRenewBookPercent1Change(Sender:TObject);
    procedure EditRenewBookPercent2Change(Sender:TObject);
    procedure EditRenewLogOutPercentChange(Sender:TObject);
    procedure EditRenewHPTime1Change(Sender:TObject);
    procedure EditRenewHPTime2Change(Sender:TObject);
    procedure EditRenewMPTimeChange(Sender:TObject);
    procedure EditRenewSpecialTimeChange(Sender:TObject);
    procedure EditRenewBookTime1Change(Sender:TObject);
    procedure EditRenewBookTime2Change(Sender:TObject);
    procedure EditRenewLogOuttimeChange(Sender:TObject);
    procedure EditRenewHeroHPPercent1Change(Sender:TObject);
    procedure EditRenewHeroHPPercent2Change(Sender:TObject);
    procedure EditRenewHeroMPPercentChange(Sender:TObject);
    procedure EditRenewHeroSpecialPercentChange(Sender:TObject);
    procedure EditRenewHeroLogOutPercentChange(Sender:TObject);
    procedure EditRenewHeroHPTime1Change(Sender:TObject);
    procedure EditRenewHeroHPTime2Change(Sender:TObject);
    procedure EditRenewHeroMPTimeChange(Sender:TObject);
    procedure EditRenewHeroSpecialTimeChange(Sender:TObject);
    procedure EditRenewHeroLogOutTimeChange(Sender:TObject);
    procedure EditAutoUseMagicChange(Sender:TObject);
    procedure chkShowNpcHPLabelClick(Sender:TObject);
    procedure chkNotParalyClick(Sender:TObject);
    procedure chkShowUserNameClick(Sender:TObject);
    procedure chkHideHumEffectClick(Sender:TObject);
    procedure chkHideWeaponEffectClick(Sender:TObject);
    procedure chkShowHumClick(Sender:TObject);
    procedure lstUnBindItemListClick(Sender:TObject);
    procedure btnBindItemAddClick(Sender:TObject);
    procedure btnBindItemDelClick(Sender:TObject);
    procedure btnBindItemChgClick(Sender:TObject);
    procedure btnBindItemSaveClick(Sender:TObject);
    procedure chkSpeedSlowClick(Sender:TObject);
    procedure CheckBoxAutoChangePoisonClick(Sender:TObject);
    procedure chkSmartKTZHitClick(Sender:TObject);
    procedure chkShowNPCNameClick(Sender:TObject);
    procedure btnSpecialAddClick(Sender:TObject);
    procedure btnSpecialDelClick(Sender:TObject);
    procedure seSpecialColorChange(Sender:TObject);
    procedure chkShowNGLabelClick(Sender:TObject);
    procedure btnBossAddClick(Sender:TObject);
    procedure btnBossDelClick(Sender:TObject);
    procedure BtnBossModifyClick(Sender:TObject);
    procedure lstBossClick(Sender:TObject);
    procedure chkNearHintClick(Sender:TObject);
    procedure chkAutoLockClick(Sender:TObject);
    procedure chkColorShowClick(Sender:TObject);
    procedure cbbShowColorChange(Sender:TObject);
    procedure chkHeroAutoShieldClick(Sender:TObject);
    procedure chkAssistantHeroAutoShieldClick(Sender:TObject);
    procedure chkAutoSysMsgClick(Sender:TObject);
    procedure seAutoMsgTimeChange(Sender:TObject);
    procedure chkBlacklistHitClick(Sender:TObject);
    procedure chkFriendHitClick(Sender:TObject);
    procedure chkSpecialQuickFlashingClick(Sender:TObject);
    procedure chkSceneShakeClick(Sender:TObject);
    procedure chkShiftSwitchClick(Sender:TObject);
    procedure chkAutoDownHorseClick(Sender:TObject);
    procedure chkHideTitleClick(Sender:TObject);
    procedure chkSmartCRSHitClick(Sender:TObject);
    procedure chkSmartTWNHitClick(Sender:TObject);
    procedure chkShowRadarActorClick(Sender:TObject);
    procedure lstGJMonClick(Sender:TObject);
    procedure btnGJMonAddClick(Sender:TObject);
    procedure btnGJMonDelClick(Sender:TObject);
    procedure btnGJMonEditClick(Sender:TObject);
    procedure chkNoRedPoisonClick(Sender:TObject);
    procedure chkNoBluePoisonClick(Sender:TObject);
    procedure chkNoDuFuClick(Sender:TObject);
    procedure chkNotRushMonClick(Sender:TObject);
    procedure chkGroupAttackClick(Sender:TObject);
    procedure cbbPlayAttackOptionChange(Sender:TObject);
    procedure seNotRushMonRangeChange(Sender:TObject);
    procedure lvGJMagic1MouseDown(Sender:TObject; Button:TMouseButton;
      Shift:TShiftState; X, Y:Integer);
    procedure chkPlayAttackClick(Sender:TObject);
    procedure chkBagFullClick(Sender:TObject);
    procedure seGroupAttackCountChange(Sender:TObject);
    procedure chkLimitScreenClick(Sender:TObject);
    procedure btnGJRunClick(Sender:TObject);
    procedure chkAutoPickupClick(Sender:TObject);
    procedure chkDFAvoidClick(Sender:TObject);
    procedure chkPercentProtectClick(Sender:TObject);
    procedure chkHeroPercentProtectClick(Sender:TObject);
    procedure chkDisableChartMemoSizeClick(Sender:TObject);
    procedure chkItemCompareClick(Sender:TObject);
    procedure trbVolumeChange(Sender:TObject);
    procedure chkContinueButchItemClick(Sender:TObject);
    procedure chkCheckDuraClick(Sender:TObject);
    procedure seCheckDuraMinChange(Sender:TObject);
    procedure edtCheckDuraItemChange(Sender:TObject);
    procedure seCheckDuraTimeChange(Sender:TObject);
    procedure btnGJPointClick(Sender:TObject);
    procedure chkDisableDealClick(Sender:TObject);
    procedure chkShowUpdateStatusClick(Sender:TObject);
    procedure chkSimpleShowActorClick(Sender:TObject);
    procedure chkHumManuallyMove10AttackClick(Sender:TObject);
    procedure chkSimpleShowHumanDressClick(Sender:TObject);
    procedure chkHideItemEffectClick(Sender:TObject);
    procedure chkEnabledHotKeyClick(Sender:TObject);
    procedure mmoNotesChange(Sender:TObject);
    procedure chkAutoGroupAttackClick(Sender:TObject);
    procedure chkAutoGroupNoAttackMonClick(Sender:TObject);
    procedure hkCustom1KeyDown(Sender:TObject; var Key:Word;
      Shift:TShiftState);
    procedure hkCustom1MouseDown(Sender:TObject; Button:TMouseButton;
      Shift:TShiftState; X, Y:Integer);
    procedure hkCustom9ContextPopup(Sender:TObject; MousePos:TPoint;
      var Handled:Boolean);
    procedure chkBagFastItemCompareClick(Sender:TObject);
    procedure chkHumManuallyFireClick(Sender:TObject);
    procedure chkShowHPUnitClick(Sender:TObject);
    procedure mniEnableClick(Sender:TObject);
    procedure mniDisableClick(Sender:TObject);
    procedure mniReverseClick(Sender:TObject);
    procedure chkPickupAllClick(Sender:TObject);
    procedure chkAutoPickupItemClick(Sender:TObject);
    procedure chkHeroShowNumberStateClick(Sender:TObject);
    procedure lblItemsExportClick(Sender:TObject);
    procedure lblItemImportClick(Sender:TObject);
    procedure lblItemsExportMouseEnter(Sender:TObject);
    procedure lblItemsExportMouseLeave(Sender:TObject);
    procedure chkShowTargetApertureClick(Sender:TObject);
    procedure chkNoCatonClick(Sender:TObject);
    procedure chkSmart113HitClick(Sender:TObject);
    procedure chkAutoOpenSpellClick(Sender:TObject);
    procedure hkCustom1KeyPress(Sender:TObject; var Key:Char);
    procedure chkSimpleShowBBClick(Sender:TObject);
    procedure lblItemEditClick(Sender:TObject);
    procedure chkSmartCustomHit1Click(Sender:TObject);
    procedure chkSmartCustomHit2Click(Sender:TObject);
    procedure chkSmartCustomHit3Click(Sender:TObject);
    procedure chkSmartCustomHit4Click(Sender:TObject);
    procedure chkSmartCustomHit5Click(Sender:TObject);
    procedure chkSmartCustomHit6Click(Sender:TObject);
    procedure chkSmartCustomHit7Click(Sender:TObject);
    procedure chkSmartCustomHit8Click(Sender:TObject);
    procedure chkHumManuallyCustomHit1Click(Sender:TObject);
    procedure chkHumManuallyCustomHit2Click(Sender:TObject);
    procedure chkHumManuallyCustomHit3Click(Sender:TObject);
    procedure chkHumManuallyCustomHit4Click(Sender:TObject);
    procedure chkHumManuallyCustomHit5Click(Sender:TObject);
    procedure chkShowValueItemEffectClick(Sender:TObject);
    procedure chkAutoContinueAttackClick(Sender:TObject);
    procedure chkHideActorIconsClick(Sender:TObject);
    procedure chkSimpleShowHumanWeaponClick(Sender:TObject);
    procedure chkAutoDetourPathClick(Sender:TObject);
    procedure chkDimFireEffectClick(Sender:TObject);
    procedure chkHideMonsterIconsClick(Sender:TObject);
    procedure chkNearEffectClick(Sender: TObject);
  private
    //MainHandle: THandle;

    procedure RefBindItemList;
    function CheckBossNameExists(Name:string; CurIndex:Integer = -1):Boolean;
    procedure SaveOrLoadBossList(IsSave:Boolean);

    function CheckGJMonNameExists(Name:string; CurIndex:Integer = -1):Boolean;
    procedure SaveOrLoadGJMonList(IsSave:Boolean);

    procedure SaveOrLoadGJMagicList1(IsSave:Boolean);
    procedure SaveOrLoadGJMagicList2(IsSave:Boolean);
    procedure RefActorList;

    procedure RefKeyBoardConfig;

    procedure LoadNotesFile;
    procedure SaveNotesFile;
  public
    procedure Open(AHandle:THandle);
    procedure RefConfig;
    procedure RefShowItem;

    procedure AddToBossList(sName:string); //HZQ 20230829
    procedure RemoveFromBossList(sName:string);
    procedure AddOrRemoveBossList(sName:string);

    function ProcessKeyDown:Boolean;

    procedure LoadUI(IniStream:TFastIniStream);
    procedure LoadSubControls(Parent:TWinControl; IniStream:TFastIniStream);
  end;

var
  FrmJSYDlg:TFrmJSYDlg = nil;

implementation

uses MShare,
  ConfigShare,
  IniFiles,
  ClMain,
  Actor,
  BassSound,
  SoundUtil,
  FState;

{$R *.dfm}
{const
  RC_PLAYOBJECT = 0;
  RC_HEROOBJECT = 1;                                        // 英雄
  RC_PLAYMOSTER = 60;                                       // 人形怪物

  RC_MOONOBJECT = 99;                                       // 月灵
  RC_GUARD = 11;                                            // 大刀守卫
  RC_PEACENPC = 15;
  RC_ANIMAL = 50;
  RC_MONSTER = 80;
  RC_NPC = 10;
  RC_ARCHERGUARD = 112;

  RCC_USERHUMAN = RC_PLAYOBJECT;
  RCC_HEROOBJECT = RC_HEROOBJECT;
  RCC_GUARD = 12;                                           // RC_GUARD;
  RCC_MERCHANT = RC_ANIMAL;}

var
  dwMsgTick_EatHumHPSpecialItem:Cardinal = 0;
  dwMsgTick_EatHumHPItem1:Cardinal = 0;
  dwMsgTick_EatHumHPItem2:Cardinal = 0;
  dwMsgTick_EatHeroHPSpecialItem:Cardinal = 0;
  dwMsgTick_EatHeroHPItem1:Cardinal = 0;
  dwMsgTick_EatHeroHPItem2:Cardinal = 0;

  dwMsgTick_EatHumMPSpecialItem:Cardinal = 0;
  dwMsgTick_EatHumMPItem:Cardinal = 0;
  dwMsgTick_EatHeroMPSpecialItem:Cardinal = 0;
  dwMsgTick_EatHeroMPItem:Cardinal = 0;

  dwMsgTick_EatHumSpecialItem:Cardinal = 0;
  dwMsgTick_EatHeroSpecialItem:Cardinal = 0;

const
  MsgTickTime:Cardinal = 8000;

type
  TConfig = record
    nFilterMinExp:Integer;
    nAutoUseMagicTime:Integer;

    boPercentProtect:Boolean; // 百分比保护

    boRenewHPIsAuto1:Boolean;
    nRenewHPPercent1:Integer;
    nRenewHPTime1:Integer;
    sRenewHPItemName1:string;

    boRenewHPIsAuto2:Boolean;
    nRenewHPPercent2:Integer;
    nRenewHPTime2:Integer;
    sRenewHPItemName2:string;

    boRenewMPIsAuto:Boolean;
    nRenewMPPercent:Integer;
    nRenewMPTime:Integer;
    sRenewMPItemName:string;

    boRenewSpecialIsAuto:Boolean;
    nRenewSpecialPercent:Integer;
    nRenewSpecialTime:Integer;
    sRenewSpecialItemName:string;

    boRenewBookIsAuto1:Boolean;
    nRenewBookPercent1:Integer;
    nRenewBookTime1:Integer;
    sRenewBookItemName1:string;

    boRenewBookIsAuto2:Boolean;
    nRenewBookPercent2:Integer;
    nRenewBookTime2:Integer;
    sRenewBookItemName2:string;

    boRenewLogOutIsAuto:Boolean;
    nRenewLogOutTime:Integer;
    nRenewLogOutPercent:Integer;

    boCheckDuraIsAuto:Boolean;
    nCheckDuraMin:Integer;
    sCheckDuraItem:string;
    nCheckDuraTime:Integer;

    // -----------------------------------------------
    boHeroPercentProtect:Boolean; // 百分比保护
    boRenewHeroHPIsAuto1:Boolean;
    nRenewHeroHPPercent1:Integer;
    nRenewHeroHPTime1:Integer;
    sRenewHeroHPItemName1:string;

    boRenewHeroHPIsAuto2:Boolean;
    nRenewHeroHPPercent2:Integer;
    nRenewHeroHPTime2:Integer;
    sRenewHeroHPItemName2:string;

    boRenewHeroMPIsAuto:Boolean;
    nRenewHeroMPPercent:Integer;
    nRenewHeroMPTime:Integer;
    sRenewHeroMPItemName:string;

    boRenewHeroSpecialIsAuto:Boolean;
    nRenewHeroSpecialPercent:Integer;
    nRenewHeroSpecialTime:Integer;
    sRenewHeroSpecialItemName:string;

    boRenewHeroLogOutIsAuto:Boolean;
    nRenewHeroLogOutTime:Integer;
    nRenewHeroLogOutPercent:Integer;

    nColorShowEff:Byte; // BOSS变色显示 piaoyun 2013-09-09
    nSpecialColor:Byte;

    nGJPlayAttackOption:Integer; // 挂机 - 受玩家攻击后的操作
    nGJNoRedPoisonOption:Integer; // 挂机 - 红药用完后动作 chongchong 2014-12-06
    nGJNoBluePoisonOption:Integer; // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
    nGJNoDuFuOption:Integer; // 挂机 - 毒符用完后动作 chongchong 2014-12-06
    nGJBagFullOption:Integer; // 挂机 - 包裹满后动作 chongchong 2014-12-06

    nGJNotRushMonRange:Integer; // 挂机 - 怪物周围几格有玩家
    nGJGroupAttackCount:Integer; // 挂机 - 怪物周有几个怪群攻
  end;
  pTConfig = ^TConfig;
var
  PlugInObject:TGameConfigObject = nil;

  g_dwRenewHPTick1:LongWord;
  g_dwRenewHPTick2:LongWord;
  g_dwRenewMPTick:LongWord;
  g_dwRenewSpecialTick:LongWord;
  g_dwRenewBookTick1:LongWord;
  g_dwRenewBookTick2:LongWord;
  g_dwRenewLogOutTick:LongWord;
  g_dwRenewCheckLogOutTick:LongWord;

  g_dwRenewHeroHPTick1:LongWord;
  g_dwRenewHeroHPTick2:LongWord;
  g_dwRenewHeroMPTick:LongWord;
  g_dwRenewHeroSpecialTick:LongWord;
  g_dwRenewHeroLogOutTick:LongWord;

  g_dwAutoUseMagicTick:LongWord;

  g_CheckDuraCheckTick:LongWord;

  g_Config:TConfig = (
    nFilterMinExp:2000;
    nAutoUseMagicTime:3;
    boPercentProtect:False;

    boRenewHPIsAuto1:False;
    nRenewHPPercent1:100;
    nRenewHPTime1:1000;
    sRenewHPItemName1: '';

    boRenewHPIsAuto2:False;
    nRenewHPPercent2:100;
    nRenewHPTime2:500;
    sRenewHPItemName2: '';

    boRenewMPIsAuto:False;
    nRenewMPPercent:100;
    nRenewMPTime:1000;
    sRenewMPItemName: '';

    boRenewSpecialIsAuto:False;
    nRenewSpecialPercent:100;
    nRenewSpecialTime:3000;
    sRenewSpecialItemName: '';

    boRenewBookIsAuto1:False;
    nRenewBookPercent1:100;
    nRenewBookTime1:1500;
    sRenewBookItemName1: '';

    boRenewBookIsAuto2:False;
    nRenewBookPercent2:100;
    nRenewBookTime2:500;
    sRenewBookItemName2: '';

    boRenewLogOutIsAuto:False;
    nRenewLogOutTime:500;
    nRenewLogOutPercent:100;

    boCheckDuraIsAuto:False;
    nCheckDuraMin:20;
    sCheckDuraItem: '修复神水';
    nCheckDuraTime:30;

    // -----------------------------------------------
    boRenewHeroHPIsAuto1:False;
    nRenewHeroHPPercent1:100;
    nRenewHeroHPTime1:1000;
    sRenewHeroHPItemName1: '';

    boRenewHeroHPIsAuto2:False;
    nRenewHeroHPPercent2:100;
    nRenewHeroHPTime2:500;
    sRenewHeroHPItemName2: '';

    boRenewHeroMPIsAuto:False;
    nRenewHeroMPPercent:100;
    nRenewHeroMPTime:1000;
    sRenewHeroMPItemName: '';

    boRenewHeroSpecialIsAuto:False;
    nRenewHeroSpecialPercent:100;
    nRenewHeroSpecialTime:3000;
    sRenewHeroSpecialItemName: '';

    boRenewHeroLogOutIsAuto:False;
    nRenewHeroLogOutTime:500;
    nRenewHeroLogOutPercent:100;

    nColorShowEff:3; // BOSS变色显示 piaoyun 2013-09-09
    nSpecialColor:249;

    nGJPlayAttackOption:0; // 挂机 - 受玩家攻击后的操作
    nGJNoRedPoisonOption:0; // 挂机 - 红药用完后动作 chongchong 2014-12-06
    nGJNoBluePoisonOption:0; // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
    nGJNoDuFuOption:0; // 挂机 - 毒符用完后动作 chongchong 2014-12-06
    nGJBagFullOption:0; // 挂机 - 包裹满后动作 chongchong 2014-12-06

    nGJNotRushMonRange:7; // 挂机 - 怪物周围几格有玩家
    nGJGroupAttackCount:3; // 挂机 - 目标周围有几个怪群攻
    );

constructor TJSYConfigDlg.Create();
begin
  PlugInObject := Self;
  FProtectEnabled := True;
  FProtectEnabledTick := MyGetTickCount;
  FCharName := '';
  FEnabled := False;
  FLoadConfigDlg := False;
  FHandle := 0;

  FScreenMode := 0;
  FClientVersion := cvSerial;
  FWindowMode := True;
  FInitializeed := False;
  FLoadControl := False;
  FLoadConfig := False;

  FillChar(FConfigCheckeds, SizeOf(FConfigCheckeds), 0);
  FillChar(FClientConfig, SizeOf(FClientConfig), 0);

  FHintItemDuraTick := MyGetTickCount;

  {
  FProtectList := TStringList.Create;
  FProtectList.Add('随机传送卷');
  FProtectList.Add('地牢逃脱卷');
  FProtectList.Add('回城卷');
  FProtectList.Add('行会回城卷');
  FProtectList.Add('盟重传送石');
  FProtectList.Add('比奇传送石');
  FProtectList.Add('随机传送石');
  FProtectList.Add('小退');
  }

  FConfigCheckeds[ckShowHPLabel] := True;
  FConfigCheckeds[ckShowUserName] := False;
  FConfigCheckeds[ckMagicLock] := True;
  FConfigCheckeds[ckAutoOrderItem] := True;
  FConfigCheckeds[ckNotNeedShift] := True;
  FConfigCheckeds[ckPickupAll] := True;
  FConfigCheckeds[ckBGMusic] := True;
  FConfigCheckeds[ckRepeatBGMusic] := True;
  FConfigCheckeds[ckShowNpcHPLabel] := True;

  FConfigCheckeds[ckShowRadarPlayer] := True;
  FConfigCheckeds[ckShowRadarActor] := True;
  FConfigCheckeds[ckShowRadarNpc] := True;
  FConfigCheckeds[ckShowRadarAttackNpc] := True;

  FConfigCheckeds[ckNotParaly] := False;

  FConfigCheckeds[ckSmartLongHit] := False; // 刀刀刺杀
  FConfigCheckeds[ckSmartPosLongHit] := False; // 隔位刺杀
  FConfigCheckeds[ckSmartWalkLongHit] := False; // 走位刺杀
  FConfigCheckeds[ckSmartWideHit] := False; // 智能半月
  FConfigCheckeds[ckSmartFireHit] := False; // 自动烈火
  FConfigCheckeds[ckSmartSwordHit] := False; // 逐日剑法
  FConfigCheckeds[ckSmartCrsHit] := False; // 抱月刀 双龙斩
  FConfigCheckeds[ckSmartTwnHit] := False; // 龙影剑法

  FConfigCheckeds[ckHumAutoShield] := False; // 自动开盾
  FConfigCheckeds[ckHumStruckShield] := False; // 被攻击开盾
  FConfigCheckeds[ckHumShootLightenLockTarget] := False; // 疾光电影锁定目标
  FConfigCheckeds[ckHumManuallyFireBoom] := False; // 手动控制爆裂火焰
  FConfigCheckeds[ckHumManuallySnowWind] := False; // 手动控制冰咆哮
  FConfigCheckeds[ckHumManuallyMeteorShower] := False; // 手动控制流星火雨
  FConfigCheckeds[ckHumManuallyMove10Attack] := False; // 手动控制十步一杀
  FConfigCheckeds[ckHumManuallyFire] := False; // 手动控制地狱火

  FConfigCheckeds[ckAutoChangePoison] := False;

  FConfigCheckeds[ckShowNpcName] := False;
  FConfigCheckeds[ckShowNpcHPLabel] := False;
  FConfigCheckeds[ckShowNGLabel] := False;

  FConfigCheckeds[ckHideGhost] := False;

  FConfigCheckeds[ckNearHint] := True;
  FConfigCheckeds[ckAutoLock] := False;
  FConfigCheckeds[ckColorShow] := False;
  FConfigCheckeds[ckSpecialQuickFlashing] := False;
  FConfigCheckeds[ckBlacklistHit] := False;
  FConfigCheckeds[ckAutoDownHorse] := False;
  FConfigCheckeds[ckFriendHit] := False;
  FConfigCheckeds[ckHideTitle] := False;
  // 屏幕震动默认关闭 piaoyun 2013-09-14
  FConfigCheckeds[ckSceneShake] := False;
  FConfigCheckeds[ckAutoPickUpItem] := True;
  FConfigCheckeds[ckNoCaton] := False;

  //HZQ 20230602 增加火墙淡化、隐藏怪物顶戴、自动绕行
  FConfigCheckeds[ckAutoDetourPath] := True;
  FConfigCheckeds[ckDimFireEffect] := False;
  FConfigCheckeds[ckHideMonsterIcons] := False;
  FConfigCheckeds[ckObjectHintEffect] := False; //HZQ 20230829

  {$IF TESTMODE = 0}
  g_Config.boCheckDuraIsAuto := g_ConfigClient.ClientConfigs_Ex[0];
  {$IFEND}
end;

destructor TJSYConfigDlg.Destroy;
begin
  if FEnabled then
    SaveConfigFile;
  //FProtectList.Free;

  if FrmJSYDlg <> nil then begin
    FreeAndNil(FrmJSYDlg);
  end;

  inherited;
end;

function TJSYConfigDlg.GetType:TConfigDlgType;
begin
  Result := ptJSY;
end;

function TJSYConfigDlg.GetConfigChecked(Index:TConfigChecked):Boolean;
begin
  Result := FConfigCheckeds[Index];
end;

procedure TJSYConfigDlg.SetConfigChecked(Index:TConfigChecked; Value:Boolean);
begin
  if FConfigCheckeds[Index] <> Value then begin
    FConfigCheckeds[Index] := Value;
    // RefConfig;  // 选择选项的时候不要全部刷新，导致界面卡得像牛 chongchong 2017-03-30
  end;
end;

procedure TJSYConfigDlg.Open;
begin

end;

procedure TJSYConfigDlg.Close;
begin
  FEnabled := False;
  FLoadConfig := False;
  if FrmJSYDlg <> nil then
    FrmJSYDlg.Close;
  g_FileItemDB.BackUp;
end;

function TJSYConfigDlg.GetVisible:Boolean;
begin
  if FrmJSYDlg <> nil then begin
    Result := FrmJSYDlg.Visible;
  end else begin
    Result := False; //HZQ 20230520
  end;
end;

procedure TJSYConfigDlg.SetVisible(Value:Boolean);
begin
  if FrmJSYDlg <> nil then
    if Value then
      FrmJSYDlg.Open(FHandle)
    else
      FrmJSYDlg.Close;
end;

function TJSYConfigDlg.GetEnabled:Boolean;
begin
  Result := FEnabled;
end;

procedure TJSYConfigDlg.SetEnabled(Value:Boolean);
begin
  FEnabled := Value;
  if not FEnabled then
    FLoadConfig := False;
end;

function TJSYConfigDlg.GetProtectEnabled:Boolean;
begin
  Result := FProtectEnabled;
end;

procedure TJSYConfigDlg.SetProtectEnabled(Value:Boolean);
begin
  FProtectEnabled := Value;
  if FProtectEnabled then
    FProtectEnabledTick := MyGetTickCount;
end;

function TJSYConfigDlg.FormKeyDown(var Key:Word; Shift:TShiftState):Boolean;
begin
  Result := True; //HZQ 20230520
end;

function TJSYConfigDlg.FormKeyPress(var Key:Char):Boolean;
begin
  Result := True; //HZQ 20230520
end;

procedure TJSYConfigDlg.RefreshMySelfAbil;
begin

end;

procedure TJSYConfigDlg.RefreshMyHeroAbil;
begin

end;

procedure TJSYConfigDlg.RefreshMySelfMagicList;
var
  I, nItemIndex:Integer;

  Magic:PTClientMagic;
  ListItem:TListItem;
begin
  if FrmJSYDlg <> nil then begin
    if (g_MySelf <> nil) then begin
      nItemIndex := FrmJSYDlg.ComboBoxAutoUseMagic.ItemIndex;
      FrmJSYDlg.ComboBoxAutoUseMagic.Items.Clear;
      for I := 0 to g_MagicList.Count - 1 do begin
        FrmJSYDlg.ComboBoxAutoUseMagic.Items.AddObject(pTClientMagic(g_MagicList.Items[I]).Def.sMagicName, TObject(g_MagicList.Items[I]));
      end;
      if (nItemIndex >= 0) and (nItemIndex < FrmJSYDlg.ComboBoxAutoUseMagic.Items.Count) then
        FrmJSYDlg.ComboBoxAutoUseMagic.ItemIndex := nItemIndex
      else
        FrmJSYDlg.ComboBoxAutoUseMagic.ItemIndex := -1;
    end;

    FrmJSYDlg.lvGJMagic1.Clear;
    for I := 0 to g_MagicList.Count - 1 do begin
      Magic := g_MagicList.Items[I];

      ListItem := FrmJSYDlg.lvGJMagic1.Items.Add;
      ListItem.Data := Pointer(Magic.Def.wMagicId);
      ListItem.Caption := Magic.Def.sMagicName;

      ListItem.SubItems.Add(GetSelectString(g_GJUseMagic1.IndexOf(Pointer(Magic.Def.wMagicId)) <> -1))
    end;

    FrmJSYDlg.lvGJMagic2.Clear;
    for I := 0 to g_MagicList.Count - 1 do begin
      Magic := g_MagicList.Items[I];

      ListItem := FrmJSYDlg.lvGJMagic2.Items.Add;
      ListItem.Data := Pointer(Magic.Def.wMagicId);
      ListItem.Caption := Magic.Def.sMagicName;

      ListItem.SubItems.Add(GetSelectString(g_GJUseMagic2.IndexOf(Pointer(Magic.Def.wMagicId)) <> -1))
    end;
  end;
end;

procedure TJSYConfigDlg.RefreshMyHeroMagicList;
begin

end;

procedure TJSYConfigDlg.ClearShowItem;
begin
  FrmJSYDlg.lvFilterItem.Clear;
end;

procedure TJSYConfigDlg.RefShowItem;
begin
  FrmJSYDlg.RefShowItem;
end;

procedure TJSYConfigDlg.RefreshUnBindItemList;
var
  I, nIndex:Integer;
  BindItem:pTCustomBindItem;
  ClientItem:pTClientItem;
begin
  if (g_UnbindItemList = nil) or (FrmJSYDlg = nil) then Exit;

  with FrmJSYDlg do begin
    ComboBoxRenewHPIsAuto1.Items.Clear;
    ComboBoxRenewHPIsAuto2.Items.Clear;

    ComboBoxRenewMPIsAuto.Items.Clear;
    ComboBoxRenewSpecialIsAuto.Items.Clear;

    ComboBoxRenewBookIsAuto1.Items.Clear;
    ComboBoxRenewBookIsAuto2.Items.Clear;

    ComboBoxRenewBookIsAuto1.Items.Text := g_NGProtectItems.Text;
    if ComboBoxRenewBookIsAuto1.Items.Count > 0 then
      ComboBoxRenewBookIsAuto1.Items.Delete(ComboBoxRenewBookIsAuto1.Items.Count - 1);

    ComboBoxRenewBookIsAuto2.Items.Text := ComboBoxRenewBookIsAuto1.Items.Text;

    ComboBoxRenewHeroHPIsAuto1.Items.Clear;
    ComboBoxRenewHeroHPIsAuto2.Items.Clear;

    ComboBoxRenewHeroMPIsAuto.Items.Clear;
    ComboBoxRenewHeroSpecialIsAuto.Items.Clear;

    for I := 0 to g_StdItemList.Count - 1 do begin
      ClientItem := g_StdItemList.Items[I];

      // 药品 chongchong 2015-02-07
      if ((ClientItem.s.StdMode = 0) and (ClientItem.s.Shape <> 2)) or (ClientItem.s.StdMode in [2, 31]) then begin
        case ClientItem.s.Horse of
          1:begin
              ComboBoxRenewHPIsAuto1.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroHPIsAuto1.Items.Add(ClientItem.s.Name);

              ComboBoxRenewHPIsAuto2.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroHPIsAuto2.Items.Add(ClientItem.s.Name);
            end;
          3:begin
              ComboBoxRenewHPIsAuto1.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHPIsAuto2.Items.Add(ClientItem.s.Name);

              ComboBoxRenewHeroHPIsAuto1.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroHPIsAuto2.Items.Add(ClientItem.s.Name);
            end;
          4:begin
              ComboBoxRenewMPIsAuto.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroMPIsAuto.Items.Add(ClientItem.s.Name);
            end;
          6:begin
              ComboBoxRenewMPIsAuto.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroMPIsAuto.Items.Add(ClientItem.s.Name);
            end;
          7:begin
              ComboBoxRenewSpecialIsAuto.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroSpecialIsAuto.Items.Add(ClientItem.s.Name);
            end;
          8:begin
              ComboBoxRenewHPIsAuto1.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroHPIsAuto1.Items.Add(ClientItem.s.Name);

              ComboBoxRenewHPIsAuto2.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroHPIsAuto2.Items.Add(ClientItem.s.Name);

              ComboBoxRenewMPIsAuto.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroMPIsAuto.Items.Add(ClientItem.s.Name);

              ComboBoxRenewSpecialIsAuto.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroSpecialIsAuto.Items.Add(ClientItem.s.Name);
            end;
          9:begin
              ComboBoxRenewHPIsAuto2.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroHPIsAuto2.Items.Add(ClientItem.s.Name);

              ComboBoxRenewSpecialIsAuto.Items.Add(ClientItem.s.Name);
              ComboBoxRenewHeroSpecialIsAuto.Items.Add(ClientItem.s.Name);
            end;
        end;
      end;
    end;

    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      BindItem := g_CustomUnbindItemList.Items[I];
      case BindItem.UnBindItemType of
        t_HP:begin
            ComboBoxRenewHPIsAuto1.Items.Add(BindItem.sItemName);
            ComboBoxRenewHPIsAuto2.Items.Add(BindItem.sItemName);
            ComboBoxRenewSpecialIsAuto.Items.Add(BindItem.sItemName);

            ComboBoxRenewHeroHPIsAuto1.Items.Add(BindItem.sItemName);
            ComboBoxRenewHeroHPIsAuto2.Items.Add(BindItem.sItemName);
            ComboBoxRenewHeroSpecialIsAuto.Items.Add(BindItem.sItemName);
          end;
        t_MP:begin
            ComboBoxRenewMPIsAuto.Items.Add(BindItem.sItemName);
            ComboBoxRenewSpecialIsAuto.Items.Add(BindItem.sItemName);
            ComboBoxRenewHeroMPIsAuto.Items.Add(BindItem.sItemName);
            ComboBoxRenewHeroSpecialIsAuto.Items.Add(BindItem.sItemName);
          end;
        t_Special:begin
            ComboBoxRenewHPIsAuto1.Items.Add(BindItem.sItemName);
            ComboBoxRenewHPIsAuto2.Items.Add(BindItem.sItemName);
            ComboBoxRenewMPIsAuto.Items.Add(BindItem.sItemName);
            ComboBoxRenewSpecialIsAuto.Items.Add(BindItem.sItemName);

            ComboBoxRenewHeroHPIsAuto1.Items.Add(BindItem.sItemName);
            ComboBoxRenewHeroHPIsAuto2.Items.Add(BindItem.sItemName);
            ComboBoxRenewHeroMPIsAuto.Items.Add(BindItem.sItemName);
            ComboBoxRenewHeroSpecialIsAuto.Items.Add(BindItem.sItemName);
          end;
        t_Book:begin
            if ComboBoxRenewBookIsAuto1.Items.IndexOf(BindItem.sItemName) < 0 then begin
              ComboBoxRenewBookIsAuto1.Items.Add(BindItem.sItemName);
              ComboBoxRenewBookIsAuto2.Items.Add(BindItem.sItemName);
            end;
          end;
      end;
    end;

    // if FLoadConfig then

    if g_Config.sRenewHPItemName1 <> '' then begin
      nIndex := ComboBoxRenewHPIsAuto1.Items.IndexOf(g_Config.sRenewHPItemName1);
      if nIndex >= 0 then begin
        ComboBoxRenewHPIsAuto1.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewHPIsAuto1.Items.Count > 0 then begin
          ComboBoxRenewHPIsAuto1.ItemIndex := 0;
          g_Config.sRenewHPItemName1 := ComboBoxRenewHPIsAuto1.Items[0];
        end
        else begin
          ComboBoxRenewHPIsAuto1.ItemIndex := -1;
          g_Config.sRenewHPItemName1 := '';
        end;
      end;
    end;

    if g_Config.sRenewHPItemName2 <> '' then begin
      nIndex := ComboBoxRenewHPIsAuto2.Items.IndexOf(g_Config.sRenewHPItemName2);
      if nIndex >= 0 then begin
        ComboBoxRenewHPIsAuto2.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewHPIsAuto2.Items.Count > 0 then begin
          ComboBoxRenewHPIsAuto2.ItemIndex := 0;
          g_Config.sRenewHPItemName2 := ComboBoxRenewHPIsAuto2.Items[0];
        end
        else begin
          ComboBoxRenewHPIsAuto2.ItemIndex := -1;
          g_Config.sRenewHPItemName2 := '';
        end;
      end;
    end;

    if g_Config.sRenewMPItemName <> '' then begin
      nIndex := ComboBoxRenewMPIsAuto.Items.IndexOf(g_Config.sRenewMPItemName);
      if nIndex >= 0 then begin
        ComboBoxRenewMPIsAuto.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewMPIsAuto.Items.Count > 0 then begin
          ComboBoxRenewMPIsAuto.ItemIndex := 0;
          g_Config.sRenewMPItemName := ComboBoxRenewMPIsAuto.Items[0];
        end
        else begin
          ComboBoxRenewMPIsAuto.ItemIndex := -1;
          g_Config.sRenewMPItemName := '';
        end;
      end;
    end;

    if g_Config.sRenewSpecialItemName <> '' then begin
      nIndex := ComboBoxRenewSpecialIsAuto.Items.IndexOf(g_Config.sRenewSpecialItemName);
      if nIndex >= 0 then begin
        ComboBoxRenewSpecialIsAuto.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewSpecialIsAuto.Items.Count > 0 then begin
          ComboBoxRenewSpecialIsAuto.ItemIndex := 0;
          g_Config.sRenewSpecialItemName := ComboBoxRenewSpecialIsAuto.Items[0];
        end
        else begin
          ComboBoxRenewSpecialIsAuto.ItemIndex := -1;
          g_Config.sRenewSpecialItemName := '';
        end;
      end;
    end;

    if g_Config.sRenewBookItemName1 <> '' then begin
      nIndex := ComboBoxRenewBookIsAuto1.Items.IndexOf(g_Config.sRenewBookItemName1);
      if nIndex >= 0 then begin
        ComboBoxRenewBookIsAuto1.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewBookIsAuto1.Items.Count > 0 then begin
          ComboBoxRenewBookIsAuto1.ItemIndex := 0;
          g_Config.sRenewBookItemName1 := ComboBoxRenewBookIsAuto1.Items[0];
        end
        else begin
          ComboBoxRenewBookIsAuto1.ItemIndex := -1;
          g_Config.sRenewBookItemName1 := '';
        end;
      end;
    end;

    if g_Config.sRenewBookItemName2 <> '' then begin
      nIndex := ComboBoxRenewBookIsAuto2.Items.IndexOf(g_Config.sRenewBookItemName2);
      if nIndex >= 0 then begin
        ComboBoxRenewBookIsAuto2.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewBookIsAuto2.Items.Count > 0 then begin
          ComboBoxRenewBookIsAuto2.ItemIndex := 0;
          g_Config.sRenewBookItemName2 := ComboBoxRenewBookIsAuto2.Items[0];
        end
        else begin
          ComboBoxRenewBookIsAuto2.ItemIndex := -1;
          g_Config.sRenewBookItemName2 := '';
        end;
      end;
    end;

    if g_Config.sRenewHeroHPItemName1 <> '' then begin
      nIndex := ComboBoxRenewHeroHPIsAuto1.Items.IndexOf(g_Config.sRenewHeroHPItemName1);
      if nIndex >= 0 then begin
        ComboBoxRenewHeroHPIsAuto1.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewHeroHPIsAuto1.Items.Count > 0 then begin
          ComboBoxRenewHeroHPIsAuto1.ItemIndex := 0;
          g_Config.sRenewHeroHPItemName1 := ComboBoxRenewHeroHPIsAuto1.Items[0];
        end
        else begin
          ComboBoxRenewHeroHPIsAuto1.ItemIndex := -1;
          g_Config.sRenewHeroHPItemName1 := '';
        end;
      end;
    end;

    if g_Config.sRenewHeroHPItemName2 <> '' then begin
      nIndex := ComboBoxRenewHeroHPIsAuto2.Items.IndexOf(g_Config.sRenewHeroHPItemName2);
      if nIndex >= 0 then begin
        ComboBoxRenewHeroHPIsAuto2.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewHeroHPIsAuto2.Items.Count > 0 then begin
          ComboBoxRenewHeroHPIsAuto2.ItemIndex := 0;
          g_Config.sRenewHeroHPItemName2 := ComboBoxRenewHeroHPIsAuto2.Items[0];
        end
        else begin
          ComboBoxRenewHeroHPIsAuto2.ItemIndex := -1;
          g_Config.sRenewHeroHPItemName2 := '';
        end;
      end;
    end;

    if g_Config.sRenewHeroMPItemName <> '' then begin
      nIndex := ComboBoxRenewHeroMPIsAuto.Items.IndexOf(g_Config.sRenewHeroMPItemName);
      if nIndex >= 0 then begin
        ComboBoxRenewHeroMPIsAuto.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewHeroMPIsAuto.Items.Count > 0 then begin
          ComboBoxRenewHeroMPIsAuto.ItemIndex := 0;
          g_Config.sRenewHeroMPItemName := ComboBoxRenewHeroMPIsAuto.Items[0];
        end
        else begin
          ComboBoxRenewHeroMPIsAuto.ItemIndex := -1;
          g_Config.sRenewHeroMPItemName := '';
        end;
      end;
    end;

    if g_Config.sRenewHeroSpecialItemName <> '' then begin
      nIndex := ComboBoxRenewHeroSpecialIsAuto.Items.IndexOf(g_Config.sRenewHeroSpecialItemName);
      if nIndex >= 0 then begin
        ComboBoxRenewHeroSpecialIsAuto.ItemIndex := nIndex;
      end
      else begin
        if ComboBoxRenewHeroSpecialIsAuto.Items.Count > 0 then begin
          ComboBoxRenewHeroSpecialIsAuto.ItemIndex := 0;
          g_Config.sRenewHeroSpecialItemName := ComboBoxRenewHeroSpecialIsAuto.Items[0];
        end
        else begin
          ComboBoxRenewHeroSpecialIsAuto.ItemIndex := -1;
          g_Config.sRenewHeroMPItemName := '';
        end;
      end;
    end;
  end;
end;

procedure TJSYConfigDlg.Logout;
begin
  FLoadConfig := False;
  FEnabled := False;
  SaveConfigFile;
end;

procedure TJSYConfigDlg.RefConfig;
begin
  if not FInitializeed then Exit;

  with FrmJSYDlg do begin

    //HZQ 20230602 增加火墙淡化、隐藏怪物顶戴、自动绕行
    chkAutoDetourPath.Checked := PlugInObject.ConfigCheckeds[ckAutoDetourPath];
    chkHideMonsterIcons.Checked := PlugInObject.ConfigCheckeds[ckHideMonsterIcons];
    chkDimFireEffect.Checked := PlugInObject.ConfigCheckeds[ckDimFireEffect];

    chkPickupAll.Checked := PlugInObject.ConfigCheckeds[ckPickupAll];
    chkAutoPickupItem.Checked := PlugInObject.ConfigCheckeds[ckAutoPickUpItem];
    chkNoCaton.Checked := PlugInObject.ConfigCheckeds[ckNoCaton];
    chkAutoOpenSpell.Checked := PlugInObject.ConfigCheckeds[ckAutoOpenSpell];
    chkNotNeedShift.Checked := PlugInObject.ConfigCheckeds[ckNotNeedShift];
    chkShiftSwitch.Checked := FConfigCheckeds[ckShiftSwitch];
    edtExpFilter.Text := IntToStr(g_Config.nFilterMinExp);
    chkNotParaly.Checked := PlugInObject.ConfigCheckeds[ckNotParaly];

    CheckBoxRenewHPIsAuto1.Checked := g_Config.boRenewHPIsAuto1;
    CheckBoxRenewHPIsAuto2.Checked := g_Config.boRenewHPIsAuto2;
    CheckBoxRenewMPIsAuto.Checked := g_Config.boRenewMPIsAuto;
    CheckBoxRenewSpecialIsAuto.Checked := g_Config.boRenewSpecialIsAuto;
    CheckBoxRenewBookIsAuto1.Checked := g_Config.boRenewBookIsAuto1;
    CheckBoxRenewBookIsAuto2.Checked := g_Config.boRenewBookIsAuto2;
    CheckBoxRenewLogOut.Checked := g_Config.boRenewLogOutIsAuto;

    chkHeroPercentProtect.Checked := g_Config.boHeroPercentProtect;
    CheckBoxRenewHeroHPIsAuto1.Checked := g_Config.boRenewHeroHPIsAuto1;
    CheckBoxRenewHeroHPIsAuto2.Checked := g_Config.boRenewHeroHPIsAuto2;
    CheckBoxRenewHeroMPIsAuto.Checked := g_Config.boRenewHeroMPIsAuto;
    CheckBoxRenewHeroSpecialIsAuto.Checked := g_Config.boRenewHeroSpecialIsAuto;

    CheckBoxRenewHeroLogOutIsAuto.Checked := g_Config.boRenewHeroLogOutIsAuto;
    chkHeroShowNumberState.Checked := FConfigCheckeds[ckHeroShowNumberState];

    EditRenewHPPercent1.Text := IntToStr(g_Config.nRenewHPPercent1);
    EditRenewHPPercent2.Text := IntToStr(g_Config.nRenewHPPercent2);
    EditRenewMPPercent.Text := IntToStr(g_Config.nRenewMPPercent);
    EditRenewSpecialPercent.Text := IntToStr(g_Config.nRenewSpecialPercent);
    EditRenewBookPercent1.Text := IntToStr(g_Config.nRenewBookPercent1);
    EditRenewBookPercent2.Text := IntToStr(g_Config.nRenewBookPercent2);
    EditRenewLogOutPercent.Text := IntToStr(g_Config.nRenewLogOutPercent);
    EditRenewHPTime1.Text := IntToStr(g_Config.nRenewHPTime1);
    EditRenewHPTime2.Text := IntToStr(g_Config.nRenewHPTime2);
    EditRenewMPTime.Text := IntToStr(g_Config.nRenewMPTime);
    EditRenewSpecialTime.Text := IntToStr(g_Config.nRenewSpecialTime);
    EditRenewBookTime1.Text := IntToStr(g_Config.nRenewBookTime1);
    EditRenewBookTime2.Text := IntToStr(g_Config.nRenewBookTime2);
    EditRenewLogOuttime.Text := IntToStr(g_Config.nRenewLogOuttime);
    EditRenewHeroHPPercent1.Text := IntToStr(g_Config.nRenewHeroHPPercent1);
    EditRenewHeroHPPercent2.Text := IntToStr(g_Config.nRenewHeroHPPercent2);
    EditRenewHeroMPPercent.Text := IntToStr(g_Config.nRenewHeroMPPercent);
    EditRenewHeroSpecialPercent.Text := IntToStr(g_Config.nRenewHeroSpecialPercent);
    EditRenewHeroLogOutPercent.Text := IntToStr(g_Config.nRenewHeroLogOutPercent);
    EditRenewHeroHPTime1.Text := IntToStr(g_Config.nRenewHeroHPTime1);
    EditRenewHeroHPTime2.Text := IntToStr(g_Config.nRenewHeroHPTime2);
    EditRenewHeroMPTime.Text := IntToStr(g_Config.nRenewHeroMPTime);
    EditRenewHeroSpecialTime.Text := IntToStr(g_Config.nRenewHeroSpecialTime);
    EditRenewHeroLogOutTime.Text := IntToStr(g_Config.nRenewHeroLogOutTime);
    seSpecialColor.Value := g_Config.nSpecialColor;
    frmMain.nSpecialColor := g_Config.nSpecialColor;

    chkCheckDura.Checked := g_config.boCheckDuraIsAuto;
    seCheckDuraMin.Value := g_Config.nCheckDuraMin;
    edtCheckDuraItem.Text := g_Config.sCheckDuraItem;
    seCheckDuraTime.Value := g_Config.nCheckDuraTime;

    cbbPlayAttackOption.ItemIndex := g_Config.nGJPlayAttackOption; // 挂机 - 受玩家攻击后的操作
    FrmMain.nGJPlayAttackOption := g_Config.nGJPlayAttackOption;

    cbbNoRedPoisonOption.ItemIndex := g_Config.nGJNoRedPoisonOption; // 挂机 - 红药用完后动作 chongchong 2014-12-06
    FrmMain.nGJNoRedPoisonOption := g_Config.nGJNoRedPoisonOption;

    cbbNoBluePoisonOption.ItemIndex := g_Config.nGJNoBluePoisonOption; // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
    FrmMain.nGJNoBluePoisonOption := g_Config.nGJNoBluePoisonOption;

    cbbNoDuFuOption.ItemIndex := g_Config.nGJNoDuFuOption; // 挂机 - 毒符用完后动作 chongchong 2014-12-06
    FrmMain.nGJNoDuFuOption := g_Config.nGJNoDuFuOption;

    cbbBagFullOption.ItemIndex := g_Config.nGJBagFullOption; // 挂机 - 包裹满后动作 chongchong 2014-12-06
    FrmMain.nGJBagFullOption := g_Config.nGJBagFullOption;

    seNotRushMonRange.Value := g_Config.nGJNotRushMonRange; // 挂机 - 怪物周围几格有玩家
    FrmMain.nGJNotRushMonRange := g_Config.nGJNotRushMonRange;

    seGroupAttackCount.Value := g_Config.nGJGroupAttackCount;
    FrmMain.nGJGroupAttackCount := g_Config.nGJGroupAttackCount;

    cbbPlayAttackOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_PlayAttack];
    cbbNoRedPoisonOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_NoRedPoison];
    cbbNoBluePoisonOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_NoBluePoison];
    cbbNoDuFuOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_NoDuFu];
    cbbBagFullOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_BagFull];

    seGroupAttackCount.Enabled := PlugInObject.ConfigCheckeds[ckGJ_AutoPickup];
    seNotRushMonRange.Enabled := PlugInObject.ConfigCheckeds[ckGJ_NotRushMon];
    seGroupAttackCount.Enabled := PlugInObject.ConfigCheckeds[ckGJ_GroupAttack];

    chkPercentProtect.Checked := g_Config.boPercentProtect;

    cbbShowColor.ItemIndex := g_Config.nColorShowEff;
    frmMain.nColorShowEff := g_Config.nColorShowEff;

    ComboBoxItemStdMode.ItemIndex := 0;
    EditSearchItem.Text := '';
    RefShowItem;

    PlugInObject.RefreshUnBindItemList;

    chkEnabledHotKey.Checked := FConfigCheckeds[ckUseKeyBoard];

    RefKeyBoardConfig;
  end;
end;

procedure TJSYConfigDlg.LoadClientConfig(ClientConfig:pTClientConfig);

procedure SetCtrlsPosition(L:TList);
  var
    I, nTop:Integer;
    Ctrl:TWinControl;
  begin
    nTop := 7;
    for I := 0 to L.Count - 1 do begin
      Ctrl := TWinControl(L.Items[I]);
      if not Ctrl.Visible then Continue;
      Ctrl.Top := nTop;
      Inc(nTop, 19);
    end;
  end;

  procedure SetCtrlsPosition2(L:TList);
  var
    I, nTop:Integer;
    Ctrl:TWinControl;
  begin
    nTop := 17;
    for I := 0 to L.Count - 1 do begin
      Ctrl := TWinControl(L.Items[I]);
      if not Ctrl.Visible then Continue;
      Ctrl.Top := nTop;
      Inc(nTop, 16);
    end;
  end;
var
  List:TList;
begin
  FClientConfig := ClientConfig^;

  if FrmJSYDlg <> nil then begin
    with FrmJSYDlg do begin
      tsBase.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[0];
      tsMagic.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[1];
      tsProtect.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[2];
      tsFight.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[3];
      tsItem.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[4];
      tsNpc.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[5];
      tsGJ.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[6];
      tsKey.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[7];
      tsNotes.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[8];
      tsHelp.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[9];

      grpJobHero.Visible := FClientConfig.ClientConfigTabSheetVisibles[10]; // g_ClientConfig.DOpenHeroButton and
      tsProtectHero.TabVisible := g_ClientConfig.DOpenHeroButton and FClientConfig.ClientConfigTabSheetVisibles[10];
      grpAutoSay.Visible := FClientConfig.ClientConfigTabSheetVisibles[11];

      chkHideItemEffect.Visible := g_ClientConfig.boHideItemEffect;

      List := TList.Create;

      if (g_ConfigClient.boCustomUI) and (g_ConfigClient.boCustomConfigDlg) then begin
        //HZQ 20230602 增加火墙淡化、隐藏怪物顶戴、自动绕行
        chkAutoDetourPath.Visible := g_ClientConfig.boAutoDetourPath;
        chkDimFireEffect.Visible := g_ClientConfig.boDimFireEffect;
        chkHideMonsterIcons.Visible := g_ClientConfig.boHideMonsterIcons;

        chkShowUserName.Visible := g_ClientConfig.boShowUserName;
        chkOnlyShowCharName.Visible := g_ClientConfig.boOnlyShowCharName;
        chkShowNumberLable.Visible := g_ClientConfig.boShowNumberLable;
        chkShowMoveLable.Visible := g_ClientConfig.boShowMoveLable;
        chkShowHPLabel.Visible := g_ClientConfig.boShowHPLabel;
        chkShowHighlightHPLabel.Visible := g_ClientConfig.boShowHighlightHPLabel;
        chkShowHPUnit.Visible := g_ClientConfig.boShowHPUnit;
        chkShowNGLabel.Visible := g_ClientConfig.boShowNGLabel;
        chkShowJobAndLevel.Visible := g_ClientConfig.boShowJobAndLevel;
        chkShowMonName.Visible := g_ClientConfig.boShowMonName;
        chkShowNPCName.Visible := g_ClientConfig.boShowNpcName;
        chkShowNpcHPLabel.Visible := g_ClientConfig.boShowNpcHPLabel;

        chkNotNeedShift.Visible := g_ClientConfig.boNotNeedShift;
        chkShiftSwitch.Visible := g_ClientConfig.boShiftSwitch;
        chkExpFilter.Visible := g_ClientConfig.boFilterExp;
        edtExpFilter.Visible := g_ClientConfig.boFilterExp;
        chkHideTitle.Visible := g_ClientConfig.boHideTitle;
        chkHideGhost.Visible := g_ClientConfig.boHideGhost;
        chkHideHumEffect.Visible := g_ClientConfig.boHideHumEffect;
        chkHideWeaponEffect.Visible := g_ClientConfig.boHideWeaponEffect;
        chkDisableChartMemoSize.Visible := g_ClientConfig.boDisableChartMemoSize;
        chkBGMusic.Visible := g_ClientConfig.boBGMusic;
        chkRepeatBGMusic.Visible := g_ClientConfig.boRepeatBGMusic;

        trbVolume.Visible := g_ClientConfig.boVolume;
        lblVolume.Visible := g_ClientConfig.boVolume;

        chkShowGreenHint.Visible := g_ClientConfig.boShowGreenHint;
        chkSpeedSlow.Visible := g_ClientConfig.boSpeedSlow;
        chkDuraWarning.Visible := g_ClientConfig.boDuraWarning;
        chkShowMapDesc.Visible := g_ClientConfig.boShowMapDesc;
        chkDisableDeal.Visible := g_ClientConfig.boDisableDeal;
        chkNotParaly.Visible := g_ClientConfig.boNotParaly;
        chkContinueButchItem.Visible := g_ClientConfig.boContinueButchItem;
        chkAutoOrderItem.Visible := g_ClientConfig.boAutoOrderItem;
        chkSceneShake.Visible := g_ClientConfig.boSceneShake;
        chkShowUpdateStatus.Visible := g_ClientConfig.boShowUpdateStatus;
        chkSimpleShowHumanDress.Visible := g_ClientConfig.boSimpleShowHumanDress;
        chkSimpleShowHumanWeapon.Visible := g_ClientConfig.boSimpleShowHumanWeapon;
        chkSimpleShowActor.Visible := g_ClientConfig.boSimpleShowActor;
        chkSimpleShowBB.Visible := g_ClientConfig.boSimpleShowBB;

        chkItemCompare.Visible := g_ClientConfig.boItemCompare;
        chkBagFastItemCompare.Visible := g_ClientConfig.boBagFastItemCompare;
        chkPickupAll.Visible := g_ClientConfig.boPickupAll;
        chkAutoPickupItem.Visible := g_ClientConfig.boAutoPickUpItem;
        chkNoCaton.Visible := g_ClientConfig.boNoCaton;

        chkSmartLongHit.Visible := g_ClientConfig.boSmartLongHit;
        chkSmartPosLongHit.Visible := g_ClientConfig.boSmartPosLongHit;
        chkSmartWalkLongHit.Visible := g_ClientConfig.boSmartWalkLongHit;
        chkSmartSwordHit.Visible := g_ClientConfig.boSmartSwordHit;

        chkSmartFireHit.Visible := g_ClientConfig.boSmartFireHit;
        chkSmartKTZHit.Visible := g_ClientConfig.boSmart66Hit;
        chkSmartCRSHit.Visible := g_ClientConfig.boSmartCrsHit;
        chkSmart113Hit.Visible := g_ClientConfig.boSmart113Hit;

        chkSmartTWNHit.Visible := g_ClientConfig.boSmartTwnHit;
        chkSmartWideHit.Visible := g_ClientConfig.boSmartWideHit;
        chkHumManuallyMove10Attack.Visible := g_ClientConfig.boHumManuallyMove10Attack;
        chkAutoOpenSpell.Visible := g_ClientConfig.boAutoOpenSpell;

        chkHumAutoShield.Visible := g_ClientConfig.boHumAutoShield;
        chkHumStruckShield.Visible := g_ClientConfig.boHumStruckShield;

        chkHumManuallyMeteorShower.Visible := g_ClientConfig.boHumManuallyMeteorShower;
        chkHumManuallySnowWind.Visible := g_ClientConfig.boHumManuallySnowWind;
        chkHumManuallyFire.Visible := g_ClientConfig.boHumManuallyFire;

        chkHumManuallyFireBoom.Visible := g_ClientConfig.boHumManuallyFireBoom;
        chkHumShootLightenLockTarget.Visible := g_ClientConfig.boHumShootLightenLockTarget;

        chkHeroAutoShield.Visible := g_ClientConfig.boHeroAutoShield;
        chkAutoGroupAttack.Visible := g_ClientConfig.boAutoGroupAttack;

        chkAssistantHeroAutoShield.Visible := g_ClientConfig.boAssistantHeroAutoShield;
        chkAutoGroupNoAttackMon.Visible := g_ClientConfig.boAutoGroupNoAttackMon;

        chkMagicLock.Enabled := g_ClientConfig.boMagicLock;
        chkDisableSelfStruck.Enabled := g_ClientConfig.boDisableSelfStruck;

        CheckBoxAutoHideMode.Visible := g_ClientConfig.boAutoHideMode;

        chkSmartCustomHit1.Visible := g_ClientConfig.boSmartCustomHit1;
        chkSmartCustomHit2.Visible := g_ClientConfig.boSmartCustomHit2;
        chkSmartCustomHit3.Visible := g_ClientConfig.boSmartCustomHit3;
        chkSmartCustomHit4.Visible := g_ClientConfig.boSmartCustomHit4;
        chkSmartCustomHit5.Visible := g_ClientConfig.boSmartCustomHit5;
        chkSmartCustomHit6.Visible := g_ClientConfig.boSmartCustomHit6;
        chkSmartCustomHit7.Visible := g_ClientConfig.boSmartCustomHit7;
        chkSmartCustomHit8.Visible := g_ClientConfig.boSmartCustomHit8;

        chkAutoContinueAttack.Visible := g_ClientConfig.boAutoContinueAttack;
        chkHideActorIcons.Visible := g_ClientConfig.boHideActorIcons;

        //HZQ 20230602 赋值交换到此结束

        if not chkHumManuallyCustomHit1.Visible then begin
          PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit1] := False;
        end;

        if not chkHumManuallyCustomHit2.Visible then begin
          PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit2] := False;
        end;

        if not chkHumManuallyCustomHit3.Visible then begin
          PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit3] := False;
        end;

        if not chkHumManuallyCustomHit4.Visible then begin
          PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit4] := False;
        end;

        if not chkHumManuallyCustomHit5.Visible then begin
          PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit5] := False;
        end;
      end else begin
        List.Clear;
        List.Add(chkShowUserName);
        List.Add(chkOnlyShowCharName);
        List.Add(chkShowNumberLable);
        List.Add(chkShowMoveLable);
        List.Add(chkShowHPLabel);
        List.Add(chkShowHighlightHPLabel);
        List.Add(chkShowHPUnit);
        List.Add(chkShowNGLabel);
        List.Add(chkShowJobAndLevel);
        List.Add(chkShowMonName);
        List.Add(chkShowNPCName);
        List.Add(chkShowNpcHPLabel);
        List.Add(chkDimFireEffect); //HZQ 20230602  火墙淡化

        chkShowUserName.Visible := FClientConfig.boShowUserName;
        chkOnlyShowCharName.Visible := FClientConfig.boOnlyShowCharName;
        chkShowNumberLable.Visible := FClientConfig.boShowNumberLable;
        chkShowMoveLable.Visible := FClientConfig.boShowMoveLable;
        chkShowHPLabel.Visible := FClientConfig.boShowHPLabel;
        chkShowHighlightHPLabel.Visible := FClientConfig.boShowHighlightHPLabel;
        chkShowHPUnit.Visible := FClientConfig.boShowHPUnit;
        chkShowNGLabel.Visible := FClientConfig.boShowNGLabel; // 显示内功黄条控件是否可用-- 同步引擎 piaoyun 2013-09-11
        chkShowJobAndLevel.Visible := FClientConfig.boShowJobAndLevel;
        chkShowMonName.Visible := FClientConfig.boShowMonName;
        chkShowNPCName.Visible := FClientConfig.boShowNpcName;
        chkShowNpcHPLabel.Visible := FClientConfig.boShowNpcHPLabel;
        chkDimFireEffect.Visible := FClientConfig.boDimFireEffect;

        SetCtrlsPosition(List);

        List.Clear;
        List.Add(chkShowGreenHint);
        List.Add(chkExpFilter);
        List.Add(edtExpFilter);
        List.Add(chkHideTitle);
        List.Add(chkHideGhost);
        List.Add(chkHideHumEffect);
        List.Add(chkHideWeaponEffect);
        List.Add(chkHideActorIcons);
        List.Add(chkHideMonsterIcons); //HZQ 20230602  隐藏怪物顶戴花翎
        List.Add(chkDisableChartMemoSize);
        List.Add(chkBGMusic);
        List.Add(chkRepeatBGMusic);
        List.Add(trbVolume);

        chkShowGreenHint.Visible := FClientConfig.boShowGreenHint;
        chkExpFilter.Visible := FClientConfig.boFilterExp;
        edtExpFilter.Visible := FClientConfig.boFilterExp;
        chkHideTitle.Visible := FClientConfig.boHideTitle;
        chkHideGhost.Visible := FClientconfig.boHideGhost;
        chkHideHumEffect.Visible := FClientConfig.boHideHumEffect;
        chkHideWeaponEffect.Visible := FClientConfig.boHideWeaponEffect;
        chkHideActorIcons.Visible := FClientConfig.boHideActorIcons;
        chkDisableChartMemoSize.Visible := FClientConfig.boDisableChartMemoSize;
        chkBGMusic.Visible := FClientConfig.boBGMusic;
        chkRepeatBGMusic.Visible := FClientConfig.boRepeatBGMusic;
        trbVolume.Visible := FClientConfig.boVolume;
        trbVolume.Position := g_SoundVolume;
        chkHideMonsterIcons.Visible := FClientConfig.boHideMonsterIcons;

        SetCtrlsPosition(List);

        edtExpFilter.Top := edtExpFilter.Top - 2; //HZQ 20230602 简单修正经验过滤对话框的位置

        lblVolume.Visible := FClientConfig.boVolume;
        if lblVolume.Visible then begin
          lblVolume.Top := trbVolume.Top + 3;
        end;

        List.Clear;
        List.Add(chkNotNeedShift);
        List.Add(chkShiftSwitch);
        List.Add(chkSpeedSlow);
        List.Add(chkShowMapDesc);
        List.Add(chkDisableDeal);
        List.Add(chkContinueButchItem);
        List.Add(chkAutoOrderItem);
        List.Add(chkShowUpdateStatus);
        List.Add(chkSimpleShowHumanDress);
        List.Add(chkSimpleShowHumanWeapon);
        List.Add(chkSimpleShowActor);
        List.Add(chkSimpleShowBB);
        List.Add(chkAutoDetourPath); //HZQ 20230602

        chkNotNeedShift.Visible := FClientConfig.boNotNeedShift;
        chkShiftSwitch.Visible := FClientConfig.boShiftSwitch;
        chkSpeedSlow.Visible := FClientConfig.boSpeedSlow;
        chkShowMapDesc.Visible := FClientConfig.boShowMapDesc;
        chkDisableDeal.Visible := FClientConfig.boDisableDeal;
        chkContinueButchItem.Visible := FClientConfig.boContinueButchItem;
        chkAutoOrderItem.Visible := FClientConfig.boAutoOrderItem;
        chkShowUpdateStatus.Visible := FClientConfig.boShowUpdateStatus;
        chkSimpleShowHumanDress.Visible := FClientConfig.boSimpleShowHumanDress;
        chkSimpleShowHumanWeapon.Visible := FClientConfig.boSimpleShowHumanWeapon;
        chkSimpleShowActor.Visible := FClientConfig.boSimpleShowActor;
        chkSimpleShowBB.Visible := FClientConfig.boSimpleShowBB;
        chkAutoDetourPath.Visible := FClientConfig.boAutoDetourPath;

        SetCtrlsPosition(List);

        chkItemCompare.Visible := FClientConfig.boItemCompare;
        chkBagFastItemCompare.Visible := FClientConfig.boBagFastItemCompare;
        chkPickupAll.Visible := FClientConfig.boPickupAll;
        chkAutoPickupItem.Visible := FClientConfig.boAutoPickUpItem;
        chkNoCaton.Visible := FClientConfig.boNoCaton;
        chkShowValueItemEffect.Visible := FClientConfig.boShowDropValueItemEff;
        chkDuraWarning.Visible := FClientConfig.boDuraWarning;

        // 职业 -- 战士 chongchong 2014-01-07
        List.Clear;
        List.Add(chkSmartLongHit);
        List.Add(chkSmartPosLongHit);
        List.Add(chkSmartWalkLongHit);
        List.Add(chkSmartSwordHit);
        List.Add(chkSmartCustomHit1);
        List.Add(chkSmartCustomHit4);
        List.Add(chkSmartCustomHit7);
        chkSmartLongHit.Visible := FClientConfig.boSmartLongHit;
        chkSmartPosLongHit.Visible := FClientConfig.boSmartPosLongHit;
        chkSmartWalkLongHit.Visible := FClientConfig.boSmartWalkLongHit;
        chkSmartSwordHit.Visible := FClientConfig.boSmartSwordHit;
        chkSmartCustomHit1.Visible := FClientConfig.boSmartCustomHit1;
        chkSmartCustomHit4.Visible := FClientConfig.boSmartCustomHit4;
        chkSmartCustomHit7.Visible := FClientConfig.boSmartCustomHit7;
        SetCtrlsPosition2(List);

        List.Clear;
        List.Add(chkSmartFireHit);
        List.Add(chkSmartKTZHit);
        List.Add(chkSmartCRSHit);
        List.Add(chkSmart113Hit);
        List.Add(chkSmartCustomHit2);
        List.Add(chkSmartCustomHit5);
        List.Add(chkSmartCustomHit8);
        chkSmartFireHit.Visible := FClientConfig.boSmartFireHit;
        chkSmartKTZHit.Visible := FClientConfig.boSmart66Hit;
        chkSmartCRSHit.Visible := FClientConfig.boSmartCrsHit;
        chkSmart113Hit.Visible := FClientConfig.boSmart113Hit;
        chkSmartCustomHit2.Visible := FClientConfig.boSmartCustomHit2;
        chkSmartCustomHit5.Visible := FClientConfig.boSmartCustomHit5;
        chkSmartCustomHit8.Visible := FClientConfig.boSmartCustomHit8;
        SetCtrlsPosition2(List);

        List.Clear;
        List.Add(chkSmartTWNHit);
        List.Add(chkSmartWideHit);
        List.Add(chkHumManuallyMove10Attack);
        List.Add(chkAutoOpenSpell);
        List.Add(chkSmartCustomHit3);
        List.Add(chkSmartCustomHit6);
        chkSmartTWNHit.Visible := FClientConfig.boSmartTwnHit;
        chkSmartWideHit.Visible := FClientConfig.boSmartWideHit;
        chkHumManuallyMove10Attack.Visible := FClientConfig.boHumManuallyMove10Attack;
        chkAutoOpenSpell.Visible := FClientConfig.boAutoOpenSpell;
        chkSmartCustomHit3.Visible := FClientConfig.boSmartCustomHit3;
        chkSmartCustomHit6.Visible := FClientConfig.boSmartCustomHit6;
        SetCtrlsPosition2(List);

        List.Clear;
        List.Add(chkHumAutoShield);
        List.Add(chkHumStruckShield);
        List.Add(chkHumManuallyFireBoom);
        chkHumAutoShield.Visible := FClientConfig.boHumAutoShield;
        chkHumStruckShield.Visible := FClientConfig.boHumStruckShield;
        chkHumManuallyFireBoom.Visible := FClientConfig.boHumManuallyFireBoom;
        SetCtrlsPosition2(List);

        List.Clear;
        List.Add(chkHumManuallyMeteorShower);
        List.Add(chkHumManuallySnowWind);
        List.Add(chkHumManuallyFire);
        chkHumManuallyMeteorShower.Visible := FClientConfig.boHumManuallyMeteorShower;
        chkHumManuallySnowWind.Visible := FClientConfig.boHumManuallySnowWind;
        chkHumManuallyFire.Visible := FClientConfig.boHumManuallyFire;
        SetCtrlsPosition2(List);

        List.Clear;
        List.Add(chkHumShootLightenLockTarget);
        chkHumShootLightenLockTarget.Visible := FClientConfig.boHumShootLightenLockTarget;
        SetCtrlsPosition2(List);

        List.Clear;
        List.Add(chkAutoGroupAttack);
        List.Add(chkAutoGroupNoAttackMon);

        chkAutoGroupAttack.Visible := FClientConfig.boAutoGroupAttack;
        chkAutoGroupNoAttackMon.Visible := FClientConfig.boAutoGroupNoAttackMon;
        SetCtrlsPosition2(List);

        List.Clear;
        List.Add(chkHeroAutoShield);
        List.Add(chkAssistantHeroAutoShield);
        chkHeroAutoShield.Visible := FClientConfig.boHeroAutoShield and g_ClientConfig.DOpenHeroButton;
        chkAssistantHeroAutoShield.Visible := FClientConfig.boAssistantHeroAutoShield and g_ClientConfig.DOpenHeroButton;
        SetCtrlsPosition2(List);
        List.Free;

        chkAutoContinueAttack.Visible := FClientConfig.boAutoContinueAttack;

        chkMagicLock.Enabled := FClientConfig.boMagicLock;
        chkDisableSelfStruck.Enabled := FClientConfig.boDisableSelfStruck;
        chkNotParaly.Enabled := FClientConfig.boNotParaly;
        chkSceneShake.Enabled := FClientConfig.boSceneShake;

        CheckBoxAutoHideMode.Enabled := FClientConfig.boAutoHideMode;
        CheckBoxRenewLogOut.Enabled := not g_ClientConfig.boCloseLogoutProtect;
        if CheckBoxRenewLogOut.Enabled then begin
          CheckBoxRenewLogOut.Checked := False;
          g_Config.boRenewLogOutIsAuto := False;
        end;

        CheckBoxRenewBookIsAuto1.Enabled := not g_ClientConfig.boCloseBookProtect;
        CheckBoxRenewBookIsAuto2.Enabled := not g_ClientConfig.boCloseBookProtect;
        if not CheckBoxRenewBookIsAuto1.Enabled then begin
          CheckBoxRenewBookIsAuto1.Checked := False;
          CheckBoxRenewBookIsAuto2.Checked := False;

          g_Config.boRenewBookIsAuto1 := False;
          g_Config.boRenewBookIsAuto1 := False;
        end;
      end;
    end;
  end;
end;

procedure TJSYConfigDlg.Initialize(Handle:THandle;
  ScreenMode:Byte; ClientVersion:TClientVersion; WindowMode:Boolean);
begin
  FHandle := Handle;

  FScreenMode := ScreenMode;
  FClientVersion := ClientVersion;
  FWindowMode := WindowMode;

  if not FLoadControl then begin
    FLoadControl := True;
    g_dwRenewCheckLogOutTick := MyGetTickCount;
    FrmJSYDlg := TFrmJSYDlg.CreateParented(FHandle);
  end;

  FInitializeed := True;
end;

procedure TJSYConfigDlg.Finalize;
begin
  SaveConfigFile;
  // FInitializeed := False;
end;

procedure TJSYConfigDlg.Logon(const ServerName:string);
var
  sDirectory, sFileName:string;
begin
  g_dwRenewCheckLogOutTick := MyGetTickCount;
  g_sPlugServerName := ServerName;

  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);
  sFileName := sDirectory + g_sPlugServerName + DecodeResStr(SBindItemFileName);
  LoadNGCustomUnbindItemList(sFileName);
  FrmJSYDlg.RefBindItemList;
end;

procedure TJSYConfigDlg.LoadConfig(const CharName:string);
begin
  FCharName := CharName;
  g_sPlugUserName := ProcessFileNameSpecialChar(FCharName);

  if FLoadControl and (not FLoadConfig) then begin
    FLoadConfig := True;
    g_FileItemDB.LoadFormFile;
    LoadConfigFile;
    RefConfig;
    FEnabled := True;
  end;
end;

procedure TJSYConfigDlg.Run;
begin
  AutoUseItem(Self);
end;

procedure TJSYConfigDlg.RefActorList;
begin
  if (FrmJSYDlg <> nil) and (FrmJSYDlg.Visible) and (FrmJSYDlg.pgMain.ActivePage = FrmJSYDlg.tsNpc) then
    FrmJSYDlg.RefActorList;
end;

procedure TJSYConfigDlg.AddToBossList(sName:string);
begin
    FrmJSYDlg.AddToBossList(sName);
end;

procedure TJSYConfigDlg.RemoveFromBossList(sName:string);
begin
    FrmJSYDlg.RemoveFromBossList(sName);
end;

procedure TJSYConfigDlg.AddOrRemoveBossList(sName:string);
begin
    FrmJSYDlg.AddOrRemoveBossList(sName);
end;

function TJSYConfigDlg.GetShowItem(const ItemName:string):pTShowItem;
begin
  Result := g_FileItemDB.Find(ItemName);
end;

function TJSYConfigDlg.FindShowItem(const ItemName:string):Boolean;
var
  ShowItem:pTShowItem;
begin
  ShowItem := g_FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boShowName
  else
    Result := False;
end;

function TJSYConfigDlg.FindHintItem(const ItemName:string):Boolean;
var
  ShowItem:pTShowItem;
begin
  ShowItem := g_FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boHintMsg
  else
    Result := False;
end;

function TJSYConfigDlg.FindPickItem(const ItemName:string):Boolean;
var
  ShowItem:pTShowItem;
begin
  ShowItem := g_FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boPickup
  else
    Result := False;
end;

procedure TJSYConfigDlg.HintItem(const ItemName:string; X, Y:Integer);
begin
  g_FileItemDB.Hint(ItemName, X, Y);
end;

procedure TJSYConfigDlg.RefKeyboardConfig;
begin
  if not FInitializeed then Exit;
  FrmJSYDlg.RefKeyBoardConfig;
end;

procedure TJSYConfigDlg.RefreshHotKeys;
var
  List:TList;
  _Label:TLabel;
  HotKey:TEdit;
  nTop, I:Integer;
begin
  if not FInitializeed then Exit;

  List := TList.Create;
  List.Add(FrmJSYDlg.lblKey1);
  List.Add(FrmJSYDlg.lblKey2);
  List.Add(FrmJSYDlg.lblKey3);
  List.Add(FrmJSYDlg.lblKey4);
  List.Add(FrmJSYDlg.lblKey5);
  List.Add(FrmJSYDlg.lblKey6);
  List.Add(FrmJSYDlg.lblKey7);
  List.Add(FrmJSYDlg.lblKey8);
  List.Add(FrmJSYDlg.lblKey9);
  List.Add(FrmJSYDlg.lblKey10);
  List.Add(FrmJSYDlg.lblKey11);
  List.Add(FrmJSYDlg.lblKey12);
  List.Add(FrmJSYDlg.lblKey13);
  List.Add(FrmJSYDlg.lblKey14);
  List.Add(FrmJSYDlg.lblKey15);
  List.Add(FrmJSYDlg.lblKey16);

  FrmJSYDlg.lblKey1.Visible := not g_ClientConfig.boUseOldSerialWindows;
  FrmJSYDlg.lblKey2.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.lblKey3.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.lblKey4.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.lblKey5.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.lblKey6.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.lblKey7.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.lblKey13.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.lblKey16.Visible := g_ClientConfig.DOpenHeroButton;

  FrmJSYDlg.scbHotKey.VertScrollBar.Position := 0;
  nTop := 10;
  for I := 0 to List.Count - 1 do begin
    _Label := TLabel(List.Items[I]);
    if not _Label.Visible then Continue;
    _Label.Top := nTop;
    Inc(nTop, 23);
  end;

  List.Clear;
  List.Add(FrmJSYDlg.hkNormal1);
  List.Add(FrmJSYDlg.hkNormal2);
  List.Add(FrmJSYDlg.hkNormal3);
  List.Add(FrmJSYDlg.hkNormal4);
  List.Add(FrmJSYDlg.hkNormal5);
  List.Add(FrmJSYDlg.hkNormal6);
  List.Add(FrmJSYDlg.hkNormal7);
  List.Add(FrmJSYDlg.hkNormal8);
  List.Add(FrmJSYDlg.hkNormal9);
  List.Add(FrmJSYDlg.hkNormal10);
  List.Add(FrmJSYDlg.hkNormal11);
  List.Add(FrmJSYDlg.hkNormal12);
  List.Add(FrmJSYDlg.hkNormal13);
  List.Add(FrmJSYDlg.hkNormal14);
  List.Add(FrmJSYDlg.hkNormal15);
  List.Add(FrmJSYDlg.hkNormal16);

  FrmJSYDlg.hkNormal1.Visible := not g_ClientConfig.boUseOldSerialWindows;
  FrmJSYDlg.hkNormal2.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkNormal3.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkNormal4.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkNormal5.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkNormal6.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkNormal7.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkNormal13.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkNormal16.Visible := g_ClientConfig.DOpenHeroButton;

  nTop := 7;
  for I := 0 to List.Count - 1 do begin
    HotKey := TEdit(List.Items[I]);
    if not HotKey.Visible then Continue;
    HotKey.Top := nTop;
    Inc(nTop, 23);
  end;

  List.Clear;
  List.Add(FrmJSYDlg.hkCustom1);
  List.Add(FrmJSYDlg.hkCustom2);
  List.Add(FrmJSYDlg.hkCustom3);
  List.Add(FrmJSYDlg.hkCustom4);
  List.Add(FrmJSYDlg.hkCustom5);
  List.Add(FrmJSYDlg.hkCustom6);
  List.Add(FrmJSYDlg.hkCustom7);
  List.Add(FrmJSYDlg.hkCustom8);
  List.Add(FrmJSYDlg.hkCustom9);
  List.Add(FrmJSYDlg.hkCustom10);
  List.Add(FrmJSYDlg.hkCustom11);
  List.Add(FrmJSYDlg.hkCustom12);
  List.Add(FrmJSYDlg.hkCustom13);
  List.Add(FrmJSYDlg.hkCustom14);
  List.Add(FrmJSYDlg.hkCustom15);
  List.Add(FrmJSYDlg.hkCustom16);

  FrmJSYDlg.hkCustom1.Visible := not g_ClientConfig.boUseOldSerialWindows;
  FrmJSYDlg.hkCustom2.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkCustom3.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkCustom4.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkCustom5.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkCustom6.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkCustom7.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkCustom13.Visible := g_ClientConfig.DOpenHeroButton;
  FrmJSYDlg.hkCustom16.Visible := g_ClientConfig.DOpenHeroButton;

  nTop := 7;
  for I := 0 to List.Count - 1 do begin
    HotKey := TEdit(List.Items[I]);
    if not HotKey.Visible then Continue;
    HotKey.Top := nTop;
    Inc(nTop, 23);
  end;

  List.Free;
end;

procedure ShortCutToHotKey(ShortCut:TShortCut; var Key:Word; var Shift:TShiftState);
begin
  Key := ShortCut and not (scShift + scCtrl + scAlt);
  Shift := [];
  if ShortCut and scShift <> 0 then Include(Shift, ssShift);
  if ShortCut and scCtrl <> 0 then Include(Shift, ssCtrl);
  if ShortCut and scAlt <> 0 then Include(Shift, ssAlt);
end;

function ShiftStateToWord(TShift:TShiftState):Word;
begin
  Result := 0;
  if ssShift in TShift then Result := MOD_SHIFT;
  if ssCtrl in TShift then Result := Result or MOD_CONTROL;
  if ssAlt in TShift then Result := Result or MOD_ALT;
end;

function HotKeyShortCut(Key:Word; Shift:TShiftState):TShortCut;
begin
  Result := LoWord(LoByte(Key));
  if Result = 0 then Exit;
  if ssShift in Shift then Inc(Result, scShift);
  if ssCtrl in Shift then Inc(Result, scCtrl);
  if ssAlt in Shift then Inc(Result, scAlt);
end;

procedure TJSYConfigDlg.Struck(Actor:TObject; HP, MaxHP:LongInt);
{var
  nDamage: Integer;
  Abil: _TAbility;
  Death: Boolean;
  AObject: TObject;   }
begin
  { if FEnabled then begin
     if (g_ClientFunction.MySelf <> nil) and (g_ClientFunction.MySelf^ <> nil) then begin
       AObject := g_ClientFunction.MySelf^;
       Death := g_ClientFunction.Actor.m_boDeath(AObject)^;
       Abil := g_ClientFunction.Actor.m_Abil(AObject)^;
       if (Actor = AObject) then begin
         nDamage := Abil.HP - HP;
         if nDamage > 0 then
           DamageHPUseItem(0, nDamage);
       end;

       if (g_ClientFunction.MyHero <> nil) and (g_ClientFunction.MyHero^ <> nil) then begin
         AObject := g_ClientFunction.MyHero^;
         Death := g_ClientFunction.Actor.m_boDeath(AObject)^;
         Abil := g_ClientFunction.Actor.m_Abil(AObject)^;
         if (Actor = AObject) then begin
           nDamage := Abil.HP - HP;
           if nDamage > 0 then
             DamageHPUseItem(1, nDamage);
         end;
       end;
     end;
   end; }
end;

procedure TJSYConfigDlg.HealthChange(Actor:TObject; HP, MP, MaxHP:LongInt);
{var
  nDamage: Integer;
  Abil: _TAbility;
  Death: Boolean;
  AObject: TObject;   }
begin
  {if FEnabled then begin
    if (g_ClientFunction.MySelf <> nil) and (g_ClientFunction.MySelf^ <> nil) then begin
      AObject := g_ClientFunction.MySelf^;
      Death := g_ClientFunction.Actor.m_boDeath(AObject)^;
      Abil := g_ClientFunction.Actor.m_Abil(AObject)^;
      if (Actor = AObject) then begin
        nDamage := Abil.HP - HP;
        if nDamage > 0 then
          DamageHPUseItem(0, nDamage);

        nDamage := Abil.MP - MP;
        if nDamage > 0 then
          DamageMPUseItem(0, nDamage);
      end;

      if (g_ClientFunction.MyHero <> nil) and (g_ClientFunction.MyHero^ <> nil) then begin
        AObject := g_ClientFunction.MyHero^;
        Death := g_ClientFunction.Actor.m_boDeath(AObject)^;
        Abil := g_ClientFunction.Actor.m_Abil(AObject)^;
        if (Actor = AObject) then begin
          nDamage := Abil.HP - HP;
          if nDamage > 0 then
            DamageHPUseItem(1, nDamage);

          nDamage := Abil.MP - MP;
          if nDamage > 0 then
            DamageMPUseItem(1, nDamage);
        end;
      end;
    end;
  end; }
end;

function FindHumUnBindItemIndex(sItemName:string):Integer;
var
  I:Integer;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
    if CompareText(g_ItemArr[I].s.Name, sItemName) = 0 then begin
      if (CompareText(g_ItemArr[I].s.Name, sItemName) = 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;

  for I := Low(g_ItemArr) to 5 do begin
    if CompareText(g_ItemArr[I].s.Name, sItemName) = 0 then begin
      if (CompareText(g_ItemArr[I].s.Name, sItemName) = 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHumBindItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  JSYUnBindItem:pTCustomBindItem;
  UnBindItem:pTUnBindItem;
begin
  {.$I AddVmpFeatureCode_M.inc}

  Result := -1;
  if HumBagNoUseItemCount >= 6 then begin
    for II := 0 to g_UnbindItemList.Count - 1 do begin
      UnBindItem := g_UnbindItemList.Items[II];
      if (CompareText(UnBindItem.sItemName, sItemName) = 0) then begin
        for I := Low(g_ItemArr) + 6 to GetMaxBagCount - 1 do begin
          // 修正吃回城卷会吃掉卷轴 + (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) chongchong 2013-12-11
          if (g_ItemArr[I].s.Name <> '') and (g_ItemArr[I].s.StdMode = 31) and (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) then begin
            if (g_ItemArr[I].s.Shape = UnBindItem.nShape) then begin
              Result := I;
              Exit;
            end;
          end;
        end;
      end;
    end;

    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      JSYUnBindItem := g_CustomUnbindItemList.Items[I];
      if (JSYUnBindItem.sBindItemName <> '') and (CompareText(JSYUnBindItem.sItemName, sItemName) = 0) then begin
        for II := Low(g_ItemArr) to GetMaxBagCount - 1 do begin
          if (CompareText(JSYUnBindItem.sBindItemName, g_ItemArr[II].s.Name) = 0) then begin
            Result := II;
            Exit;
          end;
        end;
      end;
    end;
  end;
end;

function FindHumItemIndex(sItemName:string):Integer;
begin
  Result := FindHumUnBindItemIndex(sItemName);
  if Result < 0 then
    Result := FindHumBindItemIndex(sItemName);
end;

function FindHeroUnBindItemIndex(sItemName:string):Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
    if CompareText(g_HeroItemArr[I].s.Name, sItemName) = 0 then begin
      if (CompareText(g_HeroItemArr[I].s.Name, sItemName) = 0) then begin
        Result := I;
        Exit;
      end;
    end;
  end;
end;

function FindHeroBindItemIndex(sItemName:string):Integer;
var
  I, II:Integer;
  UnBindItem:pTCustomBindItem;
begin
  Result := -1;
  if g_MyHero = nil then Exit;

  if g_MyHero.m_nBagCount - HeroBagItemCount >= 6 then begin
    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      UnBindItem := g_CustomUnbindItemList.Items[I];
      if (UnBindItem.sBindItemName <> '') and (CompareText(UnBindItem.sItemName, sItemName) = 0) then begin
        for II := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
          if (CompareText(UnBindItem.sBindItemName, g_HeroItemArr[II].s.Name) = 0) then begin
            Result := II;
            Exit;
          end;
        end;
      end;
    end;
  end;
end;

function FindHeroItemIndex(sItemName:string):Integer;
begin
  Result := FindHeroUnBindItemIndex(sItemName);
  if Result < 0 then
    Result := FindHeroBindItemIndex(sItemName);
end;

procedure TJSYConfigDlg.AutoUseItem(Sender:TObject);
begin
  if FEnabled then begin
    // 吃药问题 chongchong 2018-01-23
    //if FProtectEnabled and (MyGetTickCount - FProtectEnabledTick > 2000) then
    begin
      if Random(2) = 0 then begin
        AutoEatHPItem(Sender);
        AutoEatMPItem(Sender);
      end
      else begin
        AutoEatMPItem(Sender);
        AutoEatHPItem(Sender);
      end;
      // AutoEatSpecialHPItem(Sender);
      AutoProtect(Sender);
    end;
    AutoUseMagic;
    DuraWarning();
  end;
end;

procedure TJSYConfigDlg.AutoEatHPItem(Sender:TObject);

  function EatHumHPSpecialItem():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    nIndex := FindHumSpecialItemIndex(g_Config.sRenewSpecialItemName);
    if nIndex >= 0 then begin
      Result := True;
      frmMain.AutoEatItem(nIndex);
      g_dwRenewSpecialTick := MyGetTickCount;

      // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
      if SameText(g_Config.sRenewSpecialItemName, g_Config.sRenewHPItemName1) then
        g_dwRenewHPTick1 := MyGetTickCount;
      if SameText(g_Config.sRenewSpecialItemName, g_Config.sRenewHPItemName2) then
        g_dwRenewHPTick2 := MyGetTickCount;
    end
    else begin
      nIndex := FindHumItemIndex(g_Config.sRenewSpecialItemName);
      if nIndex >= 0 then begin
        frmMain.AutoEatItem(nIndex);
        g_dwRenewSpecialTick := MyGetTickCount;
        Result := True;

        // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
        if SameText(g_Config.sRenewSpecialItemName, g_Config.sRenewHPItemName1) then
          g_dwRenewHPTick1 := MyGetTickCount;
        if SameText(g_Config.sRenewSpecialItemName, g_Config.sRenewHPItemName2) then
          g_dwRenewHPTick2 := MyGetTickCount;
      end
      else begin
        nIndex := FindHumHPItemIndex(g_Config.sRenewSpecialItemName);
        if nIndex >= 0 then begin
          frmMain.AutoEatItem(nIndex);
          g_dwRenewSpecialTick := MyGetTickCount;
          Result := True;

          // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
          if SameText(g_Config.sRenewSpecialItemName, g_Config.sRenewHPItemName1) then begin
            g_dwRenewHPTick1 := MyGetTickCount;
          end;

          if SameText(g_Config.sRenewSpecialItemName, g_Config.sRenewHPItemName2) then begin
            g_dwRenewHPTick2 := MyGetTickCount;
          end;
        end
        else begin
          nIndex := FindHumMPItemIndex(g_Config.sRenewSpecialItemName);
          if nIndex >= 0 then begin
            frmMain.AutoEatItem(nIndex);
            g_dwRenewSpecialTick := MyGetTickCount;
            Result := True;

            // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
            if SameText(g_Config.sRenewSpecialItemName, g_Config.sRenewHPItemName1) then
              g_dwRenewHPTick1 := MyGetTickCount;
            if SameText(g_Config.sRenewSpecialItemName, g_Config.sRenewHPItemName2) then
              g_dwRenewHPTick2 := MyGetTickCount;
          end
          else if MyGetTickCount - dwMsgTick_EatHumHPSpecialItem >= MsgTickTime then begin
            dwMsgTick_EatHumHPSpecialItem := MyGetTickCount;
            DScreen.AddChatBoardString(Format(DecodeResStr(SItemUseNoMsg), [g_Config.sRenewSpecialItemName]), clWhite, clBlue);
          end;
        end;
        ;
      end;
    end;
  end;

  function EatHumHPItem1():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    // 修正及时雨不能吃手动添加的31类物品 chongchong 2015-04-19
    nIndex := FindHumHPItemIndex(g_Config.sRenewHPItemName1, True);
    if nIndex >= 0 then begin
      frmMain.AutoEatItem(nIndex);
      g_dwRenewHPTick1 := MyGetTickCount;
      Result := True;

      // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
      if SameText(g_Config.sRenewHPItemName1, g_Config.sRenewHPItemName2) then
        g_dwRenewHPTick2 := MyGetTickCount;
      if SameText(g_Config.sRenewHPItemName1, g_Config.sRenewSpecialItemName) then
        g_dwRenewSpecialTick := MyGetTickCount;
    end
    else begin
      nIndex := FindHumItemIndex(g_Config.sRenewHPItemName1);
      if nIndex >= 0 then begin
        frmMain.AutoEatItem(nIndex);
        g_dwRenewHPTick1 := MyGetTickCount;
        Result := True;

        // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
        if SameText(g_Config.sRenewHPItemName1, g_Config.sRenewHPItemName2) then
          g_dwRenewHPTick2 := MyGetTickCount;
        if SameText(g_Config.sRenewHPItemName1, g_Config.sRenewSpecialItemName) then
          g_dwRenewSpecialTick := MyGetTickCount;
      end
      else begin
        nIndex := FindHumSpecialItemIndex(g_Config.sRenewHPItemName1);
        if nIndex >= 0 then begin
          frmMain.AutoEatItem(nIndex);
          g_dwRenewHPTick1 := MyGetTickCount;
          Result := True;

          // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
          if SameText(g_Config.sRenewHPItemName1, g_Config.sRenewHPItemName2) then
            g_dwRenewHPTick2 := MyGetTickCount;
          if SameText(g_Config.sRenewHPItemName1, g_Config.sRenewSpecialItemName) then
            g_dwRenewSpecialTick := MyGetTickCount;
        end
        else if MyGetTickCount - dwMsgTick_EatHumHPItem1 >= MsgTickTime then begin
          dwMsgTick_EatHumHPItem1 := MyGetTickCount;
          DScreen.AddChatBoardString(Format(DecodeResStr(SItemUseNoMsg), [g_Config.sRenewHPItemName1]), clWhite, clBlue);
        end;
      end;
    end;
  end;

  function EatHumHPItem2():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    // 修正及时雨不能吃手动添加的31类物品 chongchong 2015-04-19
    nIndex := FindHumHPItemIndex(g_Config.sRenewHPItemName2, True);
    // DScreen.AddChatBoardString('EatHumHPItem2:'+g_Config.sRenewHPItemName1+' nIndex:'+IntToStr(nIndex), $00FFFF, $0000FF);
    if nIndex >= 0 then begin
      frmMain.AutoEatItem(nIndex);
      g_dwRenewHPTick2 := MyGetTickCount;
      Result := True;

      // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
      if SameText(g_Config.sRenewHPItemName2, g_Config.sRenewHPItemName1) then
        g_dwRenewHPTick1 := MyGetTickCount;
      if SameText(g_Config.sRenewHPItemName2, g_Config.sRenewSpecialItemName) then
        g_dwRenewSpecialTick := MyGetTickCount;
    end
    else begin
      nIndex := FindHumItemIndex(g_Config.sRenewHPItemName2);
      if nIndex >= 0 then begin
        frmMain.AutoEatItem(nIndex);
        g_dwRenewHPTick2 := MyGetTickCount;

        Result := True;

        // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
        if SameText(g_Config.sRenewHPItemName2, g_Config.sRenewHPItemName1) then
          g_dwRenewHPTick1 := MyGetTickCount;
        if SameText(g_Config.sRenewHPItemName2, g_Config.sRenewSpecialItemName) then
          g_dwRenewSpecialTick := MyGetTickCount;
      end
      else begin
        nIndex := FindHumSpecialItemIndex(g_Config.sRenewHPItemName2);
        if nIndex >= 0 then begin
          frmMain.AutoEatItem(nIndex);
          g_dwRenewHPTick2 := MyGetTickCount;
          Result := True;

          // 用这种恶心的方法来处理所谓的吃药卡药 chongchong 2018-05-07
          if SameText(g_Config.sRenewHPItemName2, g_Config.sRenewHPItemName1) then
            g_dwRenewHPTick1 := MyGetTickCount;
          if SameText(g_Config.sRenewHPItemName2, g_Config.sRenewSpecialItemName) then
            g_dwRenewSpecialTick := MyGetTickCount;
        end
        else if MyGetTickCount - dwMsgTick_EatHumHPItem2 >= MsgTickTime then begin
          dwMsgTick_EatHumHPItem2 := MyGetTickCount;

          DScreen.AddChatBoardString(Format(DecodeResStr(SItemUseNoMsg), [g_Config.sRenewHPItemName2]), clWhite, clBlue);
        end;
      end;
    end;
  end;
  // ------------------------------------------------------------------------------

  function EatHeroHPSpecialItem():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    nIndex := FindHeroSpecialItemIndex(g_Config.sRenewHeroSpecialItemName);
    if nIndex >= 0 then begin
      Result := True;
      frmMain.AutoHeroEatItem(nIndex);
      g_dwRenewHeroSpecialTick := MyGetTickCount;
    end
    else begin
      nIndex := FindHeroItemIndex(g_Config.sRenewHeroSpecialItemName);
      if nIndex >= 0 then begin

        frmMain.AutoHeroEatItem(nIndex);
        g_dwRenewHeroSpecialTick := MyGetTickCount;
        Result := True;
      end
      else begin
        nIndex := FindHeroHPItemIndex(g_Config.sRenewHeroSpecialItemName);
        if nIndex >= 0 then begin
          frmMain.AutoHeroEatItem(nIndex);
          g_dwRenewHeroSpecialTick := MyGetTickCount;
          Result := True;
        end
        else begin
          nIndex := FindHeroMPItemIndex(g_Config.sRenewHeroSpecialItemName);
          if nIndex >= 0 then begin
            frmMain.AutoHeroEatItem(nIndex);
            g_dwRenewHeroSpecialTick := MyGetTickCount;
            Result := True;
          end
          else if MyGetTickCount - dwMsgTick_EatHeroHPSpecialItem >= MsgTickTime then begin
            dwMsgTick_EatHeroHPSpecialItem := MyGetTickCount;
            DScreen.AddChatBoardString(Format(DecodeResStr(SHeroItemUseNoMsg), [g_Config.sRenewHeroSpecialItemName]), clWhite, clBlue);
          end;
        end;
      end;
    end;
  end;

  function EatHeroHPItem1():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    nIndex := FindHeroHPItemIndex(g_Config.sRenewHeroHPItemName1);
    if nIndex >= 0 then begin
      frmMain.AutoHeroEatItem(nIndex);
      g_dwRenewHeroHPTick1 := MyGetTickCount;
      Result := True;
      // DScreen.AddChatBoardString('AutoEatHPItem:'+g_Config.sRenewHPItemName1+' g_EatingItem.S.Name:'+g_EatingItem.S.Name+' nIndex:'+IntToStr(nIndex), $00FFFF, $0000FF);
    end
    else begin
      nIndex := FindHeroItemIndex(g_Config.sRenewHeroHPItemName1);
      if nIndex >= 0 then begin
        frmMain.AutoHeroEatItem(nIndex);
        g_dwRenewHeroHPTick1 := MyGetTickCount;
        Result := True;
      end
      else begin
        nIndex := FindHeroSpecialItemIndex(g_Config.sRenewHeroHPItemName1);
        if nIndex >= 0 then begin
          frmMain.AutoHeroEatItem(nIndex);
          g_dwRenewHeroHPTick1 := MyGetTickCount;
          Result := True;
        end
        else if MyGetTickCount - dwMsgTick_EatHeroHPItem1 >= MsgTickTime then begin
          dwMsgTick_EatHeroHPItem1 := MyGetTickCount;
          DScreen.AddChatBoardString(Format(DecodeResStr(SHeroItemUseNoMsg), [g_Config.sRenewHeroHPItemName1]), clWhite, clBlue);
        end;
      end;
    end;
  end;

  function EatHeroHPItem2():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    nIndex := FindHeroHPItemIndex(g_Config.sRenewHeroHPItemName2);
    // DScreen.AddChatBoardString('EatHeroHPItem2:'+g_Config.sRenewHPItemName1+' nIndex:'+IntToStr(nIndex), $00FFFF, $0000FF);
    if nIndex >= 0 then begin
      frmMain.AutoHeroEatItem(nIndex);
      g_dwRenewHPTick2 := MyGetTickCount;
      Result := True;
    end
    else begin
      nIndex := FindHeroItemIndex(g_Config.sRenewHeroHPItemName2);
      if nIndex >= 0 then begin
        frmMain.AutoHeroEatItem(nIndex);
        g_dwRenewHeroHPTick2 := MyGetTickCount;
        Result := True;
      end
      else begin
        nIndex := FindHeroSpecialItemIndex(g_Config.sRenewHeroHPItemName2);
        if nIndex >= 0 then begin
          frmMain.AutoHeroEatItem(nIndex);
          g_dwRenewHeroHPTick2 := MyGetTickCount;
          Result := True;
        end
        else if MyGetTickCount - dwMsgTick_EatHeroHPItem2 >= MsgTickTime then begin
          dwMsgTick_EatHeroHPItem2 := MyGetTickCount;
          DScreen.AddChatBoardString(Format(DecodeResStr(SHeroItemUseNoMsg), [g_Config.sRenewHeroHPItemName2]), clWhite, clBlue);
        end;
      end;
    end;
  end;
var
  I:Integer;
  ProList:TSortStringList;
  Value:LongWord;
  Int64Value:Int64;
begin
  if (g_MySelf <> nil) then begin
    if (not g_MySelf.m_boDeath) and (g_MySelf.m_Abil.HP > 0) then begin
      ProList := TSortStringList.Create;
      if g_Config.boRenewHPIsAuto1 and (g_Config.sRenewHPItemName1 <> '')
        and (MyGetTickCount - g_dwRenewHPTick1 > Cardinal(g_Config.nRenewHPTime1)) then begin
        if g_Config.boPercentProtect then
          Value := Round(g_MySelf.m_Abil.MaxHP / 100 * Min(g_Config.nRenewHPPercent1, 99))
        else
          Value := g_Config.nRenewHPPercent1;
        ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHumHPItem1)));
      end;
      if g_Config.boRenewHPIsAuto2 and (g_Config.sRenewHPItemName2 <> '')
        and (MyGetTickCount - g_dwRenewHPTick2 > Cardinal(g_Config.nRenewHPTime2)) then begin
        if g_Config.boPercentProtect then
          Value := Round(g_MySelf.m_Abil.MaxHP / 100 * Min(g_Config.nRenewHPPercent2, 99))
        else
          Value := g_Config.nRenewHPPercent2;

        ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHumHPItem2)));
      end;
      if g_Config.boRenewSpecialIsAuto and (g_Config.sRenewSpecialItemName <> '')
        and (MyGetTickCount - g_dwRenewSpecialTick > Cardinal(g_Config.nRenewSpecialTime)) then begin
        if g_Config.boPercentProtect then
          Value := Round(g_MySelf.m_Abil.MaxHP / 100 * Min(g_Config.nRenewSpecialPercent, 99))
        else
          Value := g_Config.nRenewSpecialPercent;

        ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHumHPSpecialItem)));
      end;

      ProList.NumberSort(False);

      for I := 0 to ProList.Count - 1 do begin
        Int64Value := StrToInt64Def(ProList.Strings[I], 0);
        Value := Min(High(LongWord), Int64Value);
        if g_MySelf.m_Abil.HP <= Value then begin
          if TRunFun(ProList.Objects[I]) then break;
        end;
      end;
      ProList.Free;
    end;

    if (g_MyHero <> nil) then begin
      if (not g_MyHero.m_boDeath) and (g_MyHero.m_Abil.HP > 0) then begin
        ProList := TSortStringList.Create;
        if g_Config.boRenewHeroHPIsAuto1 and (g_Config.sRenewHeroHPItemName1 <> '')
          and (MyGetTickCount - g_dwRenewHeroHPTick1 > Cardinal(g_Config.nRenewHeroHPTime1)) then begin
          if g_Config.boHeroPercentProtect then
            Value := Round(g_MyHero.m_Abil.MaxHP / 100 * Min(g_Config.nRenewHeroHPPercent1, 99))
          else
            Value := g_Config.nRenewHeroHPPercent1;

          ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHeroHPItem1)));
        end;
        if g_Config.boRenewHeroHPIsAuto2 and (g_Config.sRenewHPItemName2 <> '')
          and (MyGetTickCount - g_dwRenewHeroHPTick2 > Cardinal(g_Config.nRenewHeroHPTime2)) then begin
          if g_Config.boHeroPercentProtect then
            Value := Round(g_MyHero.m_Abil.MaxHP / 100 * Min(g_Config.nRenewHeroHPPercent2, 99))
          else
            Value := g_Config.nRenewHeroHPPercent2;

          ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHeroHPItem2)));
        end;
        if g_Config.boRenewHeroSpecialIsAuto and (g_Config.sRenewHeroSpecialItemName <> '')
          and (MyGetTickCount - g_dwRenewHeroSpecialTick > Cardinal(g_Config.nRenewHeroSpecialTime)) then begin
          if g_Config.boHeroPercentProtect then
            Value := Round(g_MyHero.m_Abil.MaxHP / 100 * Min(g_Config.nRenewHeroSpecialPercent, 99))
          else
            Value := g_Config.nRenewHeroSpecialPercent;

          ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHeroHPSpecialItem)));
        end;

        ProList.NumberSort(False);
        for I := 0 to ProList.Count - 1 do begin
          Int64Value := StrToInt64Def(ProList.Strings[I], 0);
          Value := Min(High(LongWord), Int64Value);

          if g_MyHero.m_Abil.HP <= Value then begin
            if TRunFun(ProList.Objects[I]) then break;
          end;
        end;
        ProList.Free;
      end;
    end;
  end;
end;

procedure TJSYConfigDlg.AutoEatMPItem(Sender:TObject);

  function EatHumMPSpecialItem():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    // 修正及时雨不能吃手动添加的31类物品 chongchong 2015-04-19
    nIndex := FindHumSpecialItemIndex(g_Config.sRenewSpecialItemName, True);
    if nIndex >= 0 then begin
      frmMain.AutoEatItem(nIndex);
      g_dwRenewSpecialTick := MyGetTickCount;
      Result := True;
    end
    else begin
      nIndex := FindHumItemIndex(g_Config.sRenewSpecialItemName);
      if nIndex >= 0 then begin
        frmMain.AutoEatItem(nIndex);
        g_dwRenewSpecialTick := MyGetTickCount;
        Result := True;
      end
      else begin
        nIndex := FindHumMPItemIndex(g_Config.sRenewSpecialItemName);
        if nIndex >= 0 then begin
          frmMain.AutoEatItem(nIndex);
          g_dwRenewSpecialTick := MyGetTickCount;
          Result := True;
        end
        else if MyGetTickCount - dwMsgTick_EatHumMPSpecialItem >= MsgTickTime then begin
          dwMsgTick_EatHumMPSpecialItem := MyGetTickCount;
          DScreen.AddChatBoardString(Format(DecodeResStr(SItemUseNoMsg), [g_Config.sRenewSpecialItemName]), clWhite, clBlue);
        end;
      end;
    end;
  end;

  function EatHumMPItem():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    // 修正及时雨不能吃手动添加的31类物品 chongchong 2015-04-19
    nIndex := FindHumMPItemIndex(g_Config.sRenewMPItemName, True);
    if nIndex >= 0 then begin
      g_dwRenewMPTick := MyGetTickCount;
      frmMain.AutoEatItem(nIndex);
      Result := True;
    end
    else begin
      nIndex := FindHumItemIndex(g_Config.sRenewMPItemName);
      if nIndex >= 0 then begin
        g_dwRenewMPTick := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHumSpecialItemIndex(g_Config.sRenewMPItemName);
        if nIndex >= 0 then begin
          frmMain.AutoEatItem(nIndex);
          g_dwRenewMPTick := MyGetTickCount;
          Result := True;
        end
        else if MyGetTickCount - dwMsgTick_EatHumMPItem >= MsgTickTime then begin
          dwMsgTick_EatHumMPItem := MyGetTickCount;
          DScreen.AddChatBoardString(Format(DecodeResStr(SItemUseNoMsg), [g_Config.sRenewMPItemName]), clWhite, clBlue);
        end;
      end;
    end;
  end;
  // ==============================================================================

  function EatHeroMPSpecialItem():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    nIndex := FindHeroSpecialItemIndex(g_Config.sRenewHeroSpecialItemName);
    if nIndex >= 0 then begin
      frmMain.AutoEatItem(nIndex);
      g_dwRenewHeroSpecialTick := MyGetTickCount;
      Result := True;
    end
    else begin
      nIndex := FindHeroItemIndex(g_Config.sRenewHeroSpecialItemName);
      if nIndex >= 0 then begin
        frmMain.AutoEatItem(nIndex);
        g_dwRenewHeroSpecialTick := MyGetTickCount;
        Result := True;
      end
      else begin
        nIndex := FindHeroMPItemIndex(g_Config.sRenewHeroSpecialItemName);
        if nIndex >= 0 then begin
          frmMain.AutoHeroEatItem(nIndex);
          g_dwRenewHeroSpecialTick := MyGetTickCount;
          Result := True;
        end
        else if MyGetTickCount - dwMsgTick_EatHeroMPSpecialItem >= MsgTickTime then begin
          dwMsgTick_EatHeroMPSpecialItem := MyGetTickCount;
          DScreen.AddChatBoardString(Format(DecodeResStr(SHeroItemUseNoMsg), [g_Config.sRenewHeroSpecialItemName]), clWhite, clBlue);
        end;
      end;
    end;
  end;

  function EatHeroMPItem():Boolean;
  var
    nIndex:Integer;
  begin
    Result := False;
    nIndex := FindHeroMPItemIndex(g_Config.sRenewHeroMPItemName);
    if nIndex >= 0 then begin
      g_dwRenewHeroMPTick := MyGetTickCount;
      frmMain.AutoHeroEatItem(nIndex);
      Result := True;
    end
    else begin
      nIndex := FindHeroItemIndex(g_Config.sRenewHeroMPItemName);
      if nIndex >= 0 then begin
        g_dwRenewHeroMPTick := MyGetTickCount;
        frmMain.AutoHeroEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHeroSpecialItemIndex(g_Config.sRenewHeroMPItemName);
        if nIndex >= 0 then begin
          frmMain.AutoHeroEatItem(nIndex);
          g_dwRenewHeroMPTick := MyGetTickCount;
          Result := True;
        end
        else if MyGetTickCount - dwMsgTick_EatHeroMPItem >= MsgTickTime then begin
          dwMsgTick_EatHeroMPItem := MyGetTickCount;
          DScreen.AddChatBoardString(Format(DecodeResStr(SHeroItemUseNoMsg), [g_Config.sRenewHeroMPItemName]), clWhite, clBlue);
        end;
      end;
    end;
  end;
var
  I:Integer;
  Value:LongWord;
  Int64Value:Int64;
  ProList:TSortStringList;
begin
  if (g_MySelf <> nil) then begin
    if (not g_MySelf.m_boDeath) and (g_MySelf.m_Abil.HP > 0) then begin
      ProList := TSortStringList.Create;
      if g_Config.boRenewMPIsAuto and (g_Config.sRenewMPItemName <> '')
        and (MyGetTickCount - g_dwRenewMPTick > Cardinal(g_Config.nRenewMPTime)) then begin
        if g_Config.boPercentProtect then
          Value := Round(g_MySelf.m_Abil.MaxMP / 100 * Min(g_Config.nRenewMPPercent, 99))
        else
          Value := g_Config.nRenewMPPercent;

        ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHumMPItem)));
      end;
      if g_Config.boRenewSpecialIsAuto and (g_Config.sRenewSpecialItemName <> '')
        and (MyGetTickCount - g_dwRenewSpecialTick > Cardinal(g_Config.nRenewSpecialTime)) then begin
        if g_Config.boPercentProtect then
          Value := Round(g_MySelf.m_Abil.MaxMP / 100 * Min(g_Config.nRenewSpecialPercent, 99))
        else
          Value := g_Config.nRenewSpecialPercent;

        ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHumMPSpecialItem)));
      end;

      ProList.NumberSort(False);

      for I := 0 to ProList.Count - 1 do begin
        Int64Value := StrToInt64Def(ProList.Strings[I], 0);
        Value := Min(High(LongWord), Int64Value);

        if g_MySelf.m_Abil.MP <= Value then begin
          if TRunFun(ProList.Objects[I]) then break;
        end;
      end;
      ProList.Free;
    end;
    // ------------------------------------------------------------------------------
    if (g_MyHero <> nil) then begin
      if (not g_MyHero.m_boDeath) and (g_MyHero.m_Abil.HP > 0) then begin
        ProList := TSortStringList.Create;

        if g_Config.boRenewHeroMPIsAuto and (g_Config.sRenewHeroMPItemName <> '')
          and (MyGetTickCount - g_dwRenewHeroMPTick > Cardinal(g_Config.nRenewHeroMPTime)) then begin
          if g_Config.boHeroPercentProtect then
            Value := Round(g_MyHero.m_Abil.MaxMP / 100 * Min(g_Config.nRenewHeroMPPercent, 99))
          else
            Value := g_Config.nRenewHeroMPPercent;
          ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHeroMPItem)));
        end;
        if g_Config.boRenewHeroSpecialIsAuto and (g_Config.sRenewHeroSpecialItemName <> '')
          and (MyGetTickCount - g_dwRenewHeroSpecialTick > Cardinal(g_Config.nRenewHeroSpecialTime)) then begin
          if g_Config.boHeroPercentProtect then
            Value := Round(g_MyHero.m_Abil.MaxMP / 100 * Min(g_Config.nRenewHeroSpecialPercent, 99))
          else
            Value := g_Config.nRenewHeroSpecialPercent;

          ProList.AddObject(IntToStr(Value), TObject(Pointer(@EatHeroMPSpecialItem)));
        end;

        ProList.NumberSort(False);

        for I := 0 to ProList.Count - 1 do begin
          Int64Value := StrToInt64Def(ProList.Strings[I], 0);
          Value := Min(High(LongWord), Int64Value);

          if g_MyHero.m_Abil.MP <= Value then begin
            if TRunFun(ProList.Objects[I]) then break;
          end;
        end;
        ProList.Free;
      end;
    end;
  end;
end;

{
procedure TJSYConfigDlg.AutoEatSpecialHPItem(Sender: TObject);
var
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  Death: Boolean;

  function EatHumSpecialItem(): Boolean;
  var
    Value1, Value2: Integer;
  begin
    Result := False;

    if g_Config.boPercentProtect then
    begin
      Value1 := Round(SelfAbil.MaxHP / 100 * Min(100, g_Config.nRenewSpecialPercent));
      Value2 := Round(SelfAbil.MaxMP / 100 * Min(100, g_Config.nRenewSpecialPercent));
    end
    else
    begin
      Value1 := g_Config.nRenewSpecialPercent;
      Value2 := Value1;
    end;

    if (MyGetTickCount - g_dwRenewSpecialTick > g_Config.nRenewSpecialTime) and ((SelfAbil.HP < Value1) or (SelfAbil.MP < Value2)) then
    begin
      nIndex := FindHumSpecialItemIndex(g_Config.sRenewSpecialItemName);
      if nIndex >= 0 then
      begin
        g_dwRenewSpecialTick := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end
      else
      begin
        nIndex := FindHumItemIndex(g_Config.sRenewSpecialItemName);
        if nIndex >= 0 then
        begin
          Result := True;
          g_dwRenewSpecialTick := MyGetTickCount;
          frmMain.AutoEatItem(nIndex);
        end
        else if MyGetTickCount - dwMsgTick_EatHumSpecialItem >= MsgTickTime then
        begin
          dwMsgTick_EatHumSpecialItem := MyGetTickCount;
          DScreen.AddChatBoardString('你的' + g_Config.sRenewSpecialItemName + '已使用完', clWhite, clBlue);
        end;
      end;
    end;
  end;

  function EatHeroSpecialItem(): Boolean;
  begin
    Result := False;
    if (MyGetTickCount - g_dwRenewHeroSpecialTick > g_Config.nRenewHeroSpecialTime) and ((HeroAbil.HP < g_Config.nRenewHeroSpecialPercent) or (HeroAbil.MP < g_Config.nRenewHeroSpecialPercent)) then
    begin
      nIndex := FindHeroSpecialItemIndex(g_Config.sRenewHeroSpecialItemName);
      if nIndex >= 0 then
      begin
        g_dwRenewHeroSpecialTick := MyGetTickCount;
        frmMain.AutoHeroEatItem(nIndex);
        Result := True;
      end
      else
      begin
        nIndex := FindHeroItemIndex(g_Config.sRenewHeroSpecialItemName);
        if nIndex >= 0 then
        begin
          Result := True;
          g_dwRenewHeroSpecialTick := MyGetTickCount;
          frmMain.AutoHeroEatItem(nIndex);
        end
        else if MyGetTickCount - dwMsgTick_EatHeroSpecialItem >= MsgTickTime then
        begin
          dwMsgTick_EatHeroSpecialItem := MyGetTickCount;
          DScreen.AddChatBoardString('你英雄的' + g_Config.sRenewHeroSpecialItemName + '已使用完', clWhite, clBlue);
        end;
      end;
    end;
  end;
begin
  if (g_MySelf <> nil) then
  begin
    SelfAbil := g_MySelf.m_Abil;
    Death := g_MySelf.m_boDeath;
    if g_Config.boRenewSpecialIsAuto and (g_Config.sRenewSpecialItemName <> '') and

    (not Death) and (SelfAbil.HP > 0) then EatHumSpecialItem();

    if (g_MyHero <> nil) then
    begin
      HeroAbil := g_MyHero.m_Abil;
      Death := g_MyHero.m_boDeath;
      if g_Config.boRenewHeroSpecialIsAuto and (g_Config.sRenewHeroSpecialItemName <> '') and
        (not Death) and (HeroAbil.HP > 0) and (HeroAbil.MaxHP > 0) then EatHeroSpecialItem();
    end;
  end;
end;
}

procedure TJSYConfigDlg.AutoProtect;
const
  CM_HEROLOGON = 1050; // 召唤英雄
var
  nIndex, J:Integer;
  SelfAbil:TAbility;
  HeroAbil:TAbility;
  SelfDeath:Boolean;

  Value:Integer;
begin
  if (g_MySelf <> nil) then begin
    SelfAbil := g_MySelf.m_Abil;
    SelfDeath := g_MySelf.m_boDeath;
    if (not SelfDeath) then begin

      if g_Config.boCheckDuraIsAuto and
        (g_Config.nCheckDuraMin > 0) and
        (g_Config.nCheckDuraTime > 0) and
        (Length(g_Config.sCheckDuraItem) > 0) and
        (MyGetTickCount - g_CheckDuraCheckTick > Cardinal(g_Config.nCheckDuraTime) * 1000) then begin
        g_CheckDuraCheckTick := MyGetTickCount;
        nIndex := FindHumUnBindBookItemIndex(g_Config.sCheckDuraItem);

        // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
        //Value := g_config.nCheckDuraMin * 1000;

        if nIndex >= 0 then begin
          J := Low(g_UseItems);
          while J <= High(g_UseItems) do begin
            if (Length(g_UseItems[J].S.Name) > 0) and
              (not (g_UseItems[J].s.StdMode in [7, 2, 25, 96, 97])) and
              (not ((g_UseItems[J].s.StdMode = 53) and (g_UseItems[J].s.AniCount in [1, 2, 3]))) then begin
              // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
              Value := Round(g_UseItems[J].DuraMax / 100 * g_config.nCheckDuraMin);

              if (g_UseItems[J].Dura < g_UseItems[J].DuraMax) and (g_UseItems[J].Dura < Value) then begin
                frmMain.AutoEatItem(nIndex);
                Exit;
              end;
            end;
            Inc(J);
          end;

          J := Low(g_JewelryBoxItems);
          while J <= High(g_JewelryBoxItems) do begin
            if (Length(g_JewelryBoxItems[J].S.Name) > 0) and
              (not (g_JewelryBoxItems[J].s.StdMode in [7, 2, 25, 96, 97])) and
              (not ((g_JewelryBoxItems[J].s.StdMode = 53) and (g_JewelryBoxItems[J].s.AniCount in [1, 2, 3]))) then begin
              // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
              Value := Round(g_JewelryBoxItems[J].DuraMax / 100 * g_config.nCheckDuraMin);

              if (g_JewelryBoxItems[J].Dura < g_JewelryBoxItems[J].DuraMax) and (g_JewelryBoxItems[J].Dura < Value) then begin
                frmMain.AutoEatItem(nIndex);
                Exit;
              end;
            end;
            Inc(J);
          end;

          J := Low(g_GodBlessItems);
          while J <= High(g_GodBlessItems) do begin
            if (Length(g_GodBlessItems[J].S.Name) > 0) and
              (not (g_GodBlessItems[J].s.StdMode in [7, 2, 25, 96, 97])) and
              (not ((g_GodBlessItems[J].s.StdMode = 53) and (g_GodBlessItems[J].s.AniCount in [1, 2, 3]))) then begin
              // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
              Value := Round(g_GodBlessItems[J].DuraMax / 100 * g_config.nCheckDuraMin);

              if (g_GodBlessItems[J].Dura < g_GodBlessItems[J].DuraMax) and (g_GodBlessItems[J].Dura < Value) then begin
                frmMain.AutoEatItem(nIndex);
                Exit;
              end;
            end;
            Inc(J);
          end;
        end;
      end;

      if g_Config.boPercentProtect then
        Value := Round(SelfAbil.MaxHP / 100 * Min(100, g_Config.nRenewLogOutPercent))
      else
        Value := g_Config.nRenewLogOutPercent;
      if (not g_ClientConfig.boCloseLogoutProtect) and g_Config.boRenewLogOutIsAuto
        and (MyGetTickCount - g_dwRenewCheckLogOutTick > 1000 * 60) and (SelfAbil.HP < Cardinal(Value)) then begin
        g_IsWaitLogout := True;
        g_dwRenewLogOutTick := MyGetTickCount;
        g_dwRenewCheckLogOutTick := MyGetTickCount;
        frmMain.Logout;
        Exit;
      end;

      if g_ClientConfig.boCloseBookProtect then Exit;

      if g_Config.boPercentProtect then
        Value := Round(SelfAbil.MaxHP / 100 * Min(100, g_Config.nRenewBookPercent1))
      else
        Value := g_Config.nRenewBookPercent1;
      if g_Config.boRenewBookIsAuto1 and (g_Config.sRenewBookItemName1 <> '')
        and (MyGetTickCount - g_dwRenewBookTick1 > Cardinal(g_Config.nRenewBookTime1)) and (SelfAbil.HP < Cardinal(Value)) then begin
        nIndex := FindHumBookItemIndex(g_Config.sRenewBookItemName1);
        if nIndex >= 0 then begin
          g_dwRenewBookTick1 := MyGetTickCount;
          frmMain.AutoEatItem(nIndex);
          Exit;
        end
        else begin
          nIndex := FindHumItemIndex(g_Config.sRenewBookItemName1);
          if nIndex >= 0 then begin
            g_dwRenewBookTick1 := MyGetTickCount;
            frmMain.AutoEatItem(nIndex);
            Exit;
          end;
        end;
      end;

      if g_Config.boPercentProtect then
        Value := Round(SelfAbil.MaxHP / 100 * Min(100, g_Config.nRenewBookPercent2))
      else
        Value := g_Config.nRenewBookPercent2;
      if g_Config.boRenewBookIsAuto2 and (g_Config.sRenewBookItemName2 <> '')
        and (MyGetTickCount - g_dwRenewBookTick2 > Cardinal(g_Config.nRenewBookTime2)) and (SelfAbil.HP < Cardinal(Value)) then begin
        nIndex := FindHumBookItemIndex(g_Config.sRenewBookItemName2);
        if nIndex >= 0 then begin
          g_dwRenewBookTick2 := MyGetTickCount;
          frmMain.AutoEatItem(nIndex);
          Exit;
        end
        else begin
          nIndex := FindHumItemIndex(g_Config.sRenewBookItemName2);
          if nIndex >= 0 then begin
            g_dwRenewBookTick2 := MyGetTickCount;
            frmMain.AutoEatItem(nIndex);
            Exit;
          end;
        end;
      end;
    end;

    if g_Config.boRenewHeroLogOutIsAuto and (g_MyHero <> nil) then begin
      HeroAbil := g_MyHero.m_Abil;

      if g_Config.boHeroPercentProtect then
        Value := Round(HeroAbil.MaxHP / 100 * Min(100, g_Config.nRenewHeroLogOutPercent))
      else
        Value := g_Config.nRenewHeroLogOutPercent;

      if (not g_MyHero.m_boDeath) and (HeroAbil.HP < Cardinal(Value)) then begin // 收回英雄
        if MyGetTickCount - g_dwRenewHeroLogOutTick > 2000 then begin // g_Config.nRenewHeroLogOutTime
          g_dwRenewHeroLogOutTick := MyGetTickCount;
          frmMain.SendClientMessage(CM_HEROLOGON, 0, 0, 0, 0);
          Exit;
        end;
      end;
    end;
  end;
end;

procedure TJSYConfigDlg.DuraWarning();
var
  I:Integer;
  sHint:string;
begin
  if FConfigCheckeds[ckDuraWarning] then begin
    if (g_MySelf <> nil) then begin
      if MyGetTickCount - FHintItemDuraTick > 1000 * 10 then begin
        FHintItemDuraTick := MyGetTickCount;
        for I := Low(TUseItems) to High(TUseItems) do begin
          if (g_UseItems[I].S.Name <> '') and (g_UseItems[I].S.StdMode <> 96) and (g_UseItems[I].S.StdMode <> 97) and
            (not ((g_UseItems[I].S.StdMode in [7, 53]) and (g_UseItems[I].S.Shape in [1, 2, 3]))) and // 魔血石、气血石、幻魔石
          (not ((g_UseItems[I].S.StdMode = 25) and (g_UseItems[I].S.Shape = 9))) and // 火龙之心
          (not ((g_UseItems[I].S.StdMode = 7) and (g_UseItems[I].S.Shape = 0) and (g_UseItems[I].S.AniCount > 0))) then {// 千里传音/传音筒} begin
            if g_UseItems[I].Dura <= Round(g_UseItems[I].DuraMax * 10 / 100) then begin
              sHint := Format(DecodeResStr(SItemDuraTooLow), [ProcessItemName(g_UseItems[I].S.Name)]);
              DScreen.AddChatBoardString(sHint, clyellow, clRed);
            end;
          end;
        end;
      end;
    end;
  end;
end;

function TJSYConfigDlg.AutoUseMagic:Boolean;
var
  ClientMagic:pTClientMagic;
begin
  Result := False;
  if FConfigCheckeds[ckAutoUseMagic] then begin
    if (g_MySelf <> nil) and
      (not g_MySelf.m_boDeath) and
      (not g_MySelf.m_boShopStall) then begin
      if (FrmJSYDlg.ComboBoxAutoUseMagic.ItemIndex >= 0) and (FrmJSYDlg.ComboBoxAutoUseMagic.ItemIndex < FrmJSYDlg.ComboBoxAutoUseMagic.Items.Count) then begin
        if MyGetTickCount - g_dwAutoUseMagicTick > Cardinal(g_Config.nAutoUseMagicTime) * 1000 then begin
          g_dwAutoUseMagicTick := MyGetTickCount;
          ClientMagic := pTClientMagic(FrmJSYDlg.ComboBoxAutoUseMagic.Items.Objects[FrmJSYDlg.ComboBoxAutoUseMagic.ItemIndex]);
          frmMain.AutoTakeOnItem(ClientMagic);
          Result := frmMain.UseMagic(g_nMouseX, g_nMouseY, ClientMagic);
        end;
      end;
    end;
  end;
end;

function TJSYConfigDlg.CanFilterExp(Exp:LongWord):Boolean;
begin
  Result := False;
  if FConfigCheckeds[ckFilterExp] then
    Result := Exp < Cardinal(g_Config.nFilterMinExp);
end;

procedure TJSYConfigDlg.LoadConfigFile;
var
  I:Integer;
  ini:TIniFile;
  sDirectory, sFileName:string;
  nShift:Integer;
begin
  // / \: * ?" <> |
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  if g_sPlugUserName = '' then Exit;

  sFileName := g_sSelfFilePath + Format(CONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);

  ini := TIniFile.Create(sFileName);
  if ini <> nil then begin
    // ==============================================================================================================
    g_SoundVolume := Ini.ReadInteger('Setup', 'Volume', g_SoundVolume);

    for I := 0 to Length(FConfigCheckeds) - 1 do begin
      FConfigCheckeds[TConfigChecked(I)] := ini.ReadBool('Setup', Format('Checked%d', [I]), FConfigCheckeds[TConfigChecked(I)]);
    end;

    for I := 0 to Length(g_ShortcutKeys) - 1 do begin
      g_ShortcutKeys[I].Use := ini.ReadBool('Hotkey', 'Use' + IntToStr(I), g_ShortcutKeys[I].Use);
      g_ShortcutKeys[I].Key := ini.ReadInteger('Hotkey', 'Key' + IntToStr(I), g_ShortcutKeys[I].Key);
      nShift := ini.ReadInteger('Hotkey', 'Shift' + IntToStr(I), nShift);
      Move(nShift, g_ShortcutKeys[I].Shift, SizeOf(TShiftState));
    end;

    g_Config.nFilterMinExp := ini.ReadInteger('Setup', 'FilterMinExp', g_Config.nFilterMinExp);
    g_Config.nSpecialColor := ini.ReadInteger('Setup', 'SpecialColor', g_Config.nSpecialColor);
    frmMain.nSpecialColor := g_Config.nSpecialColor;

    g_Config.nColorShowEff := ini.ReadInteger('Setup', 'ColorShowEff', g_Config.nColorShowEff);
    frmMain.nColorShowEff := g_Config.nColorShowEff;
    g_Config.boPercentProtect := ini.ReadBool('Protect', 'PercentProtect', g_Config.boPercentProtect);
    g_Config.boRenewHPIsAuto1 := ini.ReadBool('Protect', 'RenewHPIsAuto1', g_Config.boRenewHPIsAuto1);
    g_Config.boRenewHPIsAuto2 := ini.ReadBool('Protect', 'RenewHPIsAuto2', g_Config.boRenewHPIsAuto2);
    g_Config.boRenewMPIsAuto := ini.ReadBool('Protect', 'RenewMPIsAuto', g_Config.boRenewMPIsAuto);
    g_Config.boRenewSpecialIsAuto := ini.ReadBool('Protect', 'RenewSpecialIsAuto', g_Config.boRenewSpecialIsAuto);
    g_Config.boRenewBookIsAuto1 := ini.ReadBool('Protect', 'RenewBookIsAuto1', g_Config.boRenewBookIsAuto1);
    g_Config.boRenewBookIsAuto2 := ini.ReadBool('Protect', 'RenewBookIsAuto2', g_Config.boRenewBookIsAuto2);

    // 人物小退保护不保存 chongchong 2015-03-11
    //g_Config.boRenewLogOutIsAuto := ini.ReadBool('Protect', 'RenewLogOutIsAuto', g_Config.boRenewLogOutIsAuto);

    g_Config.nRenewHPPercent1 := ini.ReadInteger('Protect', 'RenewHPPercent1', g_Config.nRenewHPPercent1);
    g_Config.nRenewHPPercent2 := ini.ReadInteger('Protect', 'RenewHPPercent2', g_Config.nRenewHPPercent2);
    g_Config.nRenewMPPercent := ini.ReadInteger('Protect', 'RenewMPPercent', g_Config.nRenewMPPercent);
    g_Config.nRenewSpecialPercent := ini.ReadInteger('Protect', 'RenewSpecialPercent', g_Config.nRenewSpecialPercent);
    g_Config.nRenewBookPercent1 := ini.ReadInteger('Protect', 'RenewBookPercent1', g_Config.nRenewBookPercent1);
    g_Config.nRenewBookPercent2 := ini.ReadInteger('Protect', 'RenewBookPercent2', g_Config.nRenewBookPercent2);
    g_Config.nRenewLogOutPercent := ini.ReadInteger('Protect', 'RenewLogOutPercent', g_Config.nRenewLogOutPercent);

    g_Config.boCheckDuraIsAuto := ini.ReadBool('Protect', 'Dura1Chk', g_Config.boCheckDuraIsAuto);
    g_Config.nCheckDuraMin := Max(Ini.ReadInteger('Protect', 'RenewDuraMin', g_Config.nCheckDuraMin), 1);
    g_Config.sCheckDuraItem := Ini.ReadString('Protect', 'RenewDuraItemName', g_Config.sCheckDuraItem);
    if Length(Trim(g_Config.sCheckDuraItem)) = 0 then
      g_Config.sCheckDuraItem := '修复神水';

    g_Config.nCheckDuraTime := Max(Ini.ReadInteger('Protect', 'RenewDuraTime', g_Config.nCheckDuraTime), 2);

    g_Config.nRenewHPTime1 := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewHPTime1', g_Config.nRenewHPTime1));
    g_Config.nRenewHPTime2 := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewHPTime2', g_Config.nRenewHPTime2));
    g_Config.nRenewMPTime := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewMPTime', g_Config.nRenewMPTime));
    g_Config.nRenewSpecialTime := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewSpecialTime', g_Config.nRenewSpecialTime));
    g_Config.nRenewBookTime1 := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewBookTime1', g_Config.nRenewBookTime1));
    g_Config.nRenewBookTime2 := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewBookTime2', g_Config.nRenewBookTime2));
    g_Config.nRenewLogOutTime := ini.ReadInteger('Protect', 'RenewLogOutTime', g_Config.nRenewLogOutTime);

    g_Config.sRenewHPItemName1 := ini.ReadString('Protect', 'RenewHPItemName1', g_Config.sRenewHPItemName1);
    g_Config.sRenewHPItemName2 := ini.ReadString('Protect', 'RenewHPItemName2', g_Config.sRenewHPItemName2);
    g_Config.sRenewMPItemName := ini.ReadString('Protect', 'RenewMPItemName', g_Config.sRenewMPItemName);
    g_Config.sRenewSpecialItemName := ini.ReadString('Protect', 'RenewSpecialItemName', g_Config.sRenewSpecialItemName);
    g_Config.sRenewBookItemName1 := ini.ReadString('Protect', 'RenewBookItemName1', g_Config.sRenewBookItemName1);
    g_Config.sRenewBookItemName2 := ini.ReadString('Protect', 'RenewBookItemName2', g_Config.sRenewBookItemName2);

    g_Config.boHeroPercentProtect := ini.ReadBool('Protect', 'HeroPercentProtect', g_Config.boHeroPercentProtect);
    g_Config.boRenewHeroHPIsAuto1 := ini.ReadBool('Protect', 'RenewHeroHPIsAuto1', g_Config.boRenewHeroHPIsAuto1);
    g_Config.boRenewHeroHPIsAuto2 := ini.ReadBool('Protect', 'RenewHeroHPIsAuto2', g_Config.boRenewHeroHPIsAuto2);
    g_Config.boRenewHeroMPIsAuto := ini.ReadBool('Protect', 'RenewHeroMPIsAuto', g_Config.boRenewHeroMPIsAuto);
    g_Config.boRenewHeroSpecialIsAuto := ini.ReadBool('Protect', 'RenewHeroSpecialIsAuto', g_Config.boRenewHeroSpecialIsAuto);
    g_Config.boRenewHeroLogOutIsAuto := ini.ReadBool('Protect', 'RenewHeroLogOutIsAuto', g_Config.boRenewHeroLogOutIsAuto);

    g_Config.nRenewHeroHPPercent1 := ini.ReadInteger('Protect', 'RenewHeroHPPercent1', g_Config.nRenewHeroHPPercent1);
    g_Config.nRenewHeroHPPercent2 := ini.ReadInteger('Protect', 'RenewHeroHPPercent2', g_Config.nRenewHeroHPPercent2);
    g_Config.nRenewHeroMPPercent := ini.ReadInteger('Protect', 'RenewHeroMPPercent', g_Config.nRenewHeroMPPercent);
    g_Config.nRenewHeroSpecialPercent := ini.ReadInteger('Protect', 'RenewHeroSpecialPercent', g_Config.nRenewHeroSpecialPercent);
    g_Config.nRenewHeroLogOutPercent := ini.ReadInteger('Protect', 'RenewHeroLogOutPercent', g_Config.nRenewHeroLogOutPercent);

    g_Config.nRenewHeroHPTime1 := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewHeroHPTime1', g_Config.nRenewHeroHPTime1));
    g_Config.nRenewHeroHPTime2 := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewHeroHPTime2', g_Config.nRenewHeroHPTime2));
    g_Config.nRenewHeroMPTime := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewHeroMPTime', g_Config.nRenewHeroMPTime));
    g_Config.nRenewHeroSpecialTime := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', 'RenewHeroSpecialTime', g_Config.nRenewHeroSpecialTime));
    g_Config.nRenewHeroLogOutTime := ini.ReadInteger('Protect', 'RenewHeroLogOutTime', g_Config.nRenewHeroLogOutTime);

    g_Config.sRenewHeroHPItemName1 := ini.ReadString('Protect', 'RenewHeroHPItemName1', g_Config.sRenewHeroHPItemName1);
    g_Config.sRenewHeroHPItemName2 := ini.ReadString('Protect', 'RenewHeroHPItemName2', g_Config.sRenewHeroHPItemName2);
    g_Config.sRenewHeroMPItemName := ini.ReadString('Protect', 'RenewHeroMPItemName', g_Config.sRenewHeroMPItemName);
    g_Config.sRenewHeroSpecialItemName := ini.ReadString('Protect', 'RenewHeroSpecialItemName', g_Config.sRenewHeroSpecialItemName);

    g_Config.nGJPlayAttackOption := ini.ReadInteger('GJ', 'GJPlayAttackOption', g_Config.nGJPlayAttackOption); // 挂机 - 受玩家攻击后的操作
    g_Config.nGJNoRedPoisonOption := ini.ReadInteger('GJ', 'GJNoRedPoisonOption', g_Config.nGJNoRedPoisonOption); // 挂机 - 红药用完后动作 chongchong 2014-12-06
    g_Config.nGJNoBluePoisonOption := ini.ReadInteger('GJ', 'GJNoBluePoisonOption', g_Config.nGJNoBluePoisonOption); // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
    g_Config.nGJNoDuFuOption := ini.ReadInteger('GJ', 'GJNoDuFuOption', g_Config.nGJNoDuFuOption); // 挂机 - 毒符用完后动作 chongchong 2014-12-06
    g_Config.nGJBagFullOption := ini.ReadInteger('GJ', 'GJBagFullOption', g_Config.nGJBagFullOption); // 挂机 - 包裹满后动作 chongchong 2014-12-06

    g_Config.nGJNotRushMonRange := ini.ReadInteger('GJ', 'GJNotRushMonRange', g_Config.nGJNotRushMonRange); // 挂机 - 怪物周围几格有玩家
    g_Config.nGJGroupAttackCount := ini.ReadInteger('GJ', 'GJGroupAttackCount', g_Config.nGJGroupAttackCount); // 挂机 - 目标范围有几个怪算群攻

    FrmMain.nGJPlayAttackOption := g_Config.nGJPlayAttackOption;
    FrmMain.nGJNoRedPoisonOption := g_Config.nGJNoRedPoisonOption;
    FrmMain.nGJNoBluePoisonOption := g_Config.nGJNoBluePoisonOption;
    FrmMain.nGJNoDuFuOption := g_Config.nGJNoDuFuOption;
    FrmMain.nGJBagFullOption := g_Config.nGJBagFullOption;

    FrmMain.nGJNotRushMonRange := g_Config.nGJNotRushMonRange;
    FrmMain.nGJGroupAttackCount := g_Config.nGJGroupAttackCount;

    ini.Free;
  end;

  if FrmJSYDlg <> nil then begin
    FrmJSYDlg.SaveOrLoadBossList(False);
    FrmJSYDlg.SaveOrLoadGJMonList(False);

    FrmJSYDlg.SaveOrLoadGJMagicList1(False);
    FrmJSYDlg.SaveOrLoadGJMagicList2(False);

    FrmJSYDlg.LoadNotesFile;

    FrmJSYDlg.cbbNoRedPoisonOption.Items.Text := g_NGProtectItems.Text;
    FrmJSYDlg.cbbNoBluePoisonOption.Items.Text := g_NGProtectItems.Text;
    FrmJSYDlg.cbbNoDuFuOption.Items.Text := g_NGProtectItems.Text;
    FrmJSYDlg.cbbBagFullOption.Items.Text := g_NGProtectItems.Text;
    FrmJSYDlg.cbbPlayAttackOption.Items.Text := g_NGProtectItems.Text;
  end;
end;

procedure TJSYConfigDlg.SaveConfigFile;
var
  I, nShift:Integer;
  ini:TIniFile;
  sDirectory, sFileName:string;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;
  if g_sPlugUserName = '' then Exit;
  sFileName := g_sSelfFilePath + Format(CONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);

  ini := TIniFile.Create(sFileName);
  if ini <> nil then begin
    try
      Ini.WriteInteger('Setup', 'Volume', g_SoundVolume);

      for I := 0 to Length(FConfigCheckeds) - 1 do begin
        ini.WriteBool('Setup', Format('Checked%d', [I]), FConfigCheckeds[TConfigChecked(I)]);
      end;
      ini.WriteInteger('Setup', 'FilterMinExp', g_Config.nFilterMinExp);

      for I := 0 to Length(g_ShortcutKeys) - 1 do begin
        ini.WriteBool('Hotkey', 'Use' + IntToStr(I), g_ShortcutKeys[I].Use);
        ini.WriteInteger('Hotkey', 'Key' + IntToStr(I), g_ShortcutKeys[I].Key);
        Move(g_ShortcutKeys[I].Shift, nShift, SizeOf(TShiftState));
        ini.WriteInteger('Hotkey', 'Shift' + IntToStr(I), nShift);
      end;

      ini.WriteInteger('Setup', 'ColorShowEff', g_Config.nColorShowEff);
      ini.WriteInteger('Setup', 'SpecialColor', g_Config.nSpecialColor);

      ini.WriteBool('Protect', 'PercentProtect', g_Config.boPercentProtect);
      ini.WriteBool('Protect', 'RenewHPIsAuto1', g_Config.boRenewHPIsAuto1);
      ini.WriteBool('Protect', 'RenewHPIsAuto2', g_Config.boRenewHPIsAuto2);
      ini.WriteBool('Protect', 'RenewMPIsAuto', g_Config.boRenewMPIsAuto);
      ini.WriteBool('Protect', 'RenewSpecialIsAuto', g_Config.boRenewSpecialIsAuto);
      ini.WriteBool('Protect', 'RenewBookIsAuto1', g_Config.boRenewBookIsAuto1);
      ini.WriteBool('Protect', 'RenewBookIsAuto2', g_Config.boRenewBookIsAuto2);

      // 人物小退保护不保存 chongchong 2015-03-11
      //ini.WriteBool('Protect', 'RenewLogOutIsAuto', g_Config.boRenewLogOutIsAuto);

      ini.WriteInteger('Protect', 'RenewHPPercent1', g_Config.nRenewHPPercent1);
      ini.WriteInteger('Protect', 'RenewHPPercent2', g_Config.nRenewHPPercent2);
      ini.WriteInteger('Protect', 'RenewMPPercent', g_Config.nRenewMPPercent);
      ini.WriteInteger('Protect', 'RenewSpecialPercent', g_Config.nRenewSpecialPercent);
      ini.WriteInteger('Protect', 'RenewBookPercent1', g_Config.nRenewBookPercent1);
      ini.WriteInteger('Protect', 'RenewBookPercent2', g_Config.nRenewBookPercent2);
      ini.WriteInteger('Protect', 'RenewLogOutPercent', g_Config.nRenewLogOutPercent);

      ini.WriteBool('Protect', 'Dura1Chk', g_Config.boCheckDuraIsAuto);
      ini.WriteInteger('Protect', 'RenewDuraMin', g_Config.nCheckDuraMin);
      Ini.WriteString('Protect', 'RenewDuraItemName', g_Config.sCheckDuraItem);
      Ini.WriteInteger('Protect', 'RenewDuraTime', g_Config.nCheckDuraTime);

      ini.WriteInteger('Protect', 'RenewHPTime1', g_Config.nRenewHPTime1);
      ini.WriteInteger('Protect', 'RenewHPTime2', g_Config.nRenewHPTime2);
      ini.WriteInteger('Protect', 'RenewMPTime', g_Config.nRenewMPTime);
      ini.WriteInteger('Protect', 'RenewSpecialTime', g_Config.nRenewSpecialTime);
      ini.WriteInteger('Protect', 'RenewBookTime1', g_Config.nRenewBookTime1);
      ini.WriteInteger('Protect', 'RenewBookTime2', g_Config.nRenewBookTime2);
      ini.WriteInteger('Protect', 'RenewLogOutTime', g_Config.nRenewLogOutTime);

      ini.WriteString('Protect', 'RenewHPItemName1', g_Config.sRenewHPItemName1);
      ini.WriteString('Protect', 'RenewHPItemName2', g_Config.sRenewHPItemName2);
      ini.WriteString('Protect', 'RenewMPItemName', g_Config.sRenewMPItemName);
      ini.WriteString('Protect', 'RenewSpecialItemName', g_Config.sRenewSpecialItemName);
      ini.WriteString('Protect', 'RenewBookItemName1', g_Config.sRenewBookItemName1);
      ini.WriteString('Protect', 'RenewBookItemName2', g_Config.sRenewBookItemName2);

      ini.WriteBool('Protect', 'HeroPercentProtect', g_Config.boHeroPercentProtect);
      ini.WriteBool('Protect', 'RenewHeroHPIsAuto1', g_Config.boRenewHeroHPIsAuto1);
      ini.WriteBool('Protect', 'RenewHeroHPIsAuto2', g_Config.boRenewHeroHPIsAuto2);
      ini.WriteBool('Protect', 'RenewHeroMPIsAuto', g_Config.boRenewHeroMPIsAuto);
      ini.WriteBool('Protect', 'RenewHeroSpecialIsAuto', g_Config.boRenewHeroSpecialIsAuto);
      ini.WriteBool('Protect', 'RenewHeroLogOutIsAuto', g_Config.boRenewHeroLogOutIsAuto);

      ini.WriteInteger('Protect', 'RenewHeroHPPercent1', g_Config.nRenewHeroHPPercent1);
      ini.WriteInteger('Protect', 'RenewHeroHPPercent2', g_Config.nRenewHeroHPPercent2);
      ini.WriteInteger('Protect', 'RenewHeroMPPercent', g_Config.nRenewHeroMPPercent);
      ini.WriteInteger('Protect', 'RenewHeroSpecialPercent', g_Config.nRenewHeroSpecialPercent);
      ini.WriteInteger('Protect', 'RenewHeroLogOutPercent', g_Config.nRenewHeroLogOutPercent);

      ini.WriteInteger('Protect', 'RenewHeroHPTime1', g_Config.nRenewHeroHPTime1);
      ini.WriteInteger('Protect', 'RenewHeroHPTime2', g_Config.nRenewHeroHPTime2);
      ini.WriteInteger('Protect', 'RenewHeroMPTime', g_Config.nRenewHeroMPTime);
      ini.WriteInteger('Protect', 'RenewHeroSpecialTime', g_Config.nRenewHeroSpecialTime);
      ini.WriteInteger('Protect', 'RenewHeroLogOutTime', g_Config.nRenewHeroLogOutTime);

      ini.WriteString('Protect', 'RenewHeroHPItemName1', g_Config.sRenewHeroHPItemName1);
      ini.WriteString('Protect', 'RenewHeroHPItemName2', g_Config.sRenewHeroHPItemName2);
      ini.WriteString('Protect', 'RenewHeroMPItemName', g_Config.sRenewHeroMPItemName);
      ini.WriteString('Protect', 'RenewHeroSpecialItemName', g_Config.sRenewHeroSpecialItemName);

      ini.WriteInteger('GJ', 'GJPlayAttackOption', g_Config.nGJPlayAttackOption); // 挂机 - 受玩家攻击后的操作
      ini.WriteInteger('GJ', 'GJNoRedPoisonOption', g_Config.nGJNoRedPoisonOption); // 挂机 - 红药用完后动作 chongchong 2014-12-06
      ini.WriteInteger('GJ', 'GJNoBluePoisonOption', g_Config.nGJNoBluePoisonOption); // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
      ini.WriteInteger('GJ', 'GJNoDuFuOption', g_Config.nGJNoDuFuOption); // 挂机 - 毒符用完后动作 chongchong 2014-12-06
      ini.WriteInteger('GJ', 'GJBagFullOption', g_Config.nGJBagFullOption); // 挂机 - 包裹满后动作 chongchong 2014-12-06

      ini.WriteInteger('GJ', 'GJNotRushMonRange', g_Config.nGJNotRushMonRange); // 挂机 - 怪物周围几格有玩家
      ini.WriteInteger('GJ', 'GJGroupAttackCount', g_Config.nGJGroupAttackCount); // 挂机 - 目标范围有几个怪算群攻

    except
      on E:Exception do begin
        //DebugOutStr('[Exception] TJSYConfigDlg::SaveConfigFile');
        DebugOutStr(DecodeResStr(SJSYConfigDlgSaveErr));
        DebugOutStr(E.Message);
      end;
    end;
    ini.Free;
  end;
end;

procedure TFrmJSYDlg.RefBindItemList;
var
  I:Integer;
  BindItem:pTCustomBindItem;
begin
  lstUnBindItemList.Clear;
  for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
    BindItem := g_CustomUnbindItemList.Items[I];
    lstUnBindItemList.Items.AddObject(BindItem.sItemName, TObject(BindItem));
  end;
end;

procedure TFrmJSYDlg.RefShowItem;
var
  I:Integer;
  ShowItem:pTShowItem;
  ListItem:TListItem;
begin
  lvFilterItem.Items.Clear;
  for I := 0 to g_FileItemDB.m_ShowItemList.Count - 1 do begin
    ShowItem := pTShowItem(g_FileItemDB.m_ShowItemList.Items[I]);
    ListItem := lvFilterItem.Items.Add;
    ListItem.Data := ShowItem;
    ListItem.Caption := ShowItem.sItemName;
    ListItem.SubItems.Add(GetSelectString(ShowItem.boHintMsg));
    ListItem.SubItems.Add(GetSelectString(ShowItem.boPickup));
    ListItem.SubItems.Add(GetSelectString(ShowItem.boShowName));
    ListItem.SubItems.Add(GetSelectString(ShowItem.boShowSpecial));
    ListItem.SubItems.Add(GetSelectString(ShowItem.boAutoMove));
  end;
end;

procedure TFrmJSYDlg.RefConfig;
begin
  chkBGMusic.Checked := PlugInObject.ConfigCheckeds[ckBGMusic];
  chkRepeatBGMusic.Checked := PlugInObject.ConfigCheckeds[ckRepeatBGMusic];
  chkShowGreenHint.Checked := PlugInObject.ConfigCheckeds[ckShowGreenHint];

  chkAutoOrderItem.Checked := PlugInObject.ConfigCheckeds[ckAutoOrderItem];
  chkOnlyShowCharName.Checked := PlugInObject.ConfigCheckeds[ckOnlyShowCharName];
  chkDuraWarning.Checked := PlugInObject.ConfigCheckeds[ckDuraWarning];

  chkShowHPLabel.Checked := PlugInObject.ConfigCheckeds[ckShowHPLabel];
  chkShowNumberLable.Checked := PlugInObject.ConfigCheckeds[ckShowNumberLable];
  chkShowJobAndLevel.Checked := PlugInObject.ConfigCheckeds[ckShowJobAndLevel];
  chkShowHPUnit.Checked := PlugInObject.ConfigCheckeds[ckShowHPUnit];
  chkShowMoveLable.Checked := PlugInObject.ConfigCheckeds[ckShowMoveLable];
  chkShowMapDesc.Checked := PlugInObject.ConfigCheckeds[ckShowMapDesc];
  chkShowHighlightHPLabel.Checked := PlugInObject.ConfigCheckeds[ckShowHighlightHPLabel];

  chkShowNpcHPLabel.Checked := PlugInObject.ConfigCheckeds[ckShowNpcHPLabel];
  chkShowMonName.Checked := PlugInObject.ConfigCheckeds[ckShowMonName];
  chkShowNPCName.Checked := PlugInObject.ConfigCheckeds[ckShowNpcName]; // 显示NPC名 piaoyun 2013-07-31
  chkExpFilter.Checked := PlugInObject.ConfigCheckeds[ckFilterExp];
  chkSpeedSlow.Checked := PlugInObject.ConfigCheckeds[ckSpeedSlow];
  chkShowRadarPlayer.Checked := PlugInObject.ConfigCheckeds[ckShowRadarPlayer];
  chkShowRadarNpc.Checked := PlugInObject.ConfigCheckeds[ckShowRadarNpc];
  chkShowRadarAttackNpc.Checked := PlugInObject.ConfigCheckeds[ckShowRadarAttackNpc];
  chkShowRadarActor.Checked := PlugInObject.ConfigCheckeds[ckShowRadarActor];
  chkDisableChartMemoSize.Checked := PlugInObject.ConfigCheckeds[ckDisableChartMemoSize];
  chkShowUpdateStatus.Checked := PlugInObject.ConfigCheckeds[ckShowUpdateStatus];

  chkItemCompare.Checked := PlugInObject.ConfigCheckeds[ckItemCompare];
  chkBagFastItemCompare.Checked := PlugInObject.ConfigCheckeds[ckBagFastItemCompare];
  chkHideItemEffect.Checked := PlugInObject.ConfigCheckeds[ckHideItemEffect];
  chkPickupAll.Checked := PlugInObject.ConfigCheckeds[ckPickUpAll];
  chkAutoPickupItem.Checked := PlugInObject.ConfigCheckeds[ckAutoPickUpItem];
  chkNoCaton.Checked := PlugInObject.ConfigCheckeds[ckNoCaton];

  //chkShowNpcHPLabel.Checked := PlugInObject.ConfigCheckeds[ckShowNpcHPLabel];

  chkShowUserName.Checked := PlugInObject.ConfigCheckeds[ckShowUserName];
  chkHideHumEffect.Checked := PlugInObject.ConfigCheckeds[ckHideHumEffect];
  chkHideWeaponEffect.Checked := PlugInObject.ConfigCheckeds[ckHideWeaponEffect];
  chkContinueButchItem.Checked := PlugInObject.ConfigCheckeds[ckContinueButchItem];
  chkSimpleShowActor.Checked := PlugInObject.ConfigCheckeds[ckSimpleShowActor];
  chkDisableDeal.Checked := PlugInObject.ConfigCheckeds[ckDisableDeal];
  chkSimpleShowHumanDress.Checked := PlugInObject.ConfigCheckeds[ckSimpleShowHumanDress];
  chkSimpleShowHumanWeapon.Checked := PlugInObject.ConfigCheckeds[ckSimpleShowHumanWeapon];
  chkSimpleShowBB.Checked := PlugInObject.ConfigCheckeds[ckSimpleShowBB];

  chkSmartLongHit.Checked := PlugInObject.ConfigCheckeds[ckSmartLongHit];
  chkSmartFireHit.Checked := PlugInObject.ConfigCheckeds[ckSmartFireHit];
  chkSmartWideHit.Checked := PlugInObject.ConfigCheckeds[ckSmartWideHit];
  chkSmartPosLongHit.Checked := PlugInObject.ConfigCheckeds[ckSmartPosLongHit];
  chkSmartSwordHit.Checked := PlugInObject.ConfigCheckeds[ckSmartSwordHit];
  chkSmartKTZHit.Checked := PlugInObject.ConfigCheckeds[ckSmart66Hit];
  chkSmartCRSHit.Checked := PlugInObject.ConfigCheckeds[ckSmartCRSHit];
  chkSmart113Hit.Checked := PlugInObject.ConfigCheckeds[ckSmart113Hit];
  chkSmartTWNHit.Checked := PlugInObject.ConfigCheckeds[ckSmartTWNHit];
  chkSmartWalkLongHit.Checked := PlugInObject.ConfigCheckeds[ckSmartWalkLongHit];
  chkHumAutoShield.Checked := PlugInObject.ConfigCheckeds[ckHumAutoShield];
  chkHumStruckShield.Checked := PlugInObject.ConfigCheckeds[ckHumStruckShield];
  chkHumManuallySnowWind.Checked := PlugInObject.ConfigCheckeds[ckHumManuallySnowWind];
  chkHumManuallyMove10Attack.Checked := PlugInObject.ConfigCheckeds[ckHumManuallyMove10Attack];
  chkAutoOpenSpell.Checked := PlugInObject.ConfigCheckeds[ckAutoOpenSpell];
  chkHumManuallyFire.Checked := PlugInObject.ConfigCheckeds[ckHumManuallyFire];

  chkAutoGroupAttack.Checked := PlugInObject.ConfigCheckeds[ckAutoGroupAttack];
  chkAutoGroupNoAttackMon.Checked := PlugInObject.ConfigCheckeds[ckAutoGroupNoAttackMon];
  chkAutoContinueAttack.Checked := PlugInObject.ConfigCheckeds[ckAutoContinueAttack];

  chkHumManuallyFireBoom.Checked := PlugInObject.ConfigCheckeds[ckHumManuallyFireBoom];
  chkHumShootLightenLockTarget.Checked := PlugInObject.ConfigCheckeds[ckHumShootLightenLockTarget];
  chkHumManuallyMeteorShower.Checked := PlugInObject.ConfigCheckeds[ckHumManuallyMeteorShower];
  CheckBoxAutoChangePoison.Checked := PlugInObject.ConfigCheckeds[ckAutoChangePoison];
  CheckBoxAutoHideMode.Checked := PlugInObject.ConfigCheckeds[ckAutoHideMode];
  chkMagicLock.Checked := PlugInObject.ConfigCheckeds[ckMagicLock];
  chkDisableSelfStruck.Checked := PlugInObject.ConfigCheckeds[ckDisableSelfStruck];
  chkHideGhost.Checked := PlugInObject.ConfigCheckeds[ckHideGhost];
  CheckBoxAutoUseMagic.Checked := PlugInObject.ConfigCheckeds[ckAutoUseMagic];

  chkShowNGLabel.Checked := PlugInObject.ConfigCheckeds[ckShowNGLabel];
  chkNearHint.Checked := PlugInObject.ConfigCheckeds[ckNearHint];
  chkAutoLock.Checked := PlugInObject.ConfigCheckeds[ckAutoLock];
  chkColorShow.Checked := PlugInObject.ConfigCheckeds[ckColorShow];

  chkBlacklistHit.Checked := PlugInObject.ConfigCheckeds[ckBlacklistHit];
  chkAutoDownHorse.Checked := PlugInObject.ConfigCheckeds[ckAutoDownHorse];

  chkFriendHit.Checked := PlugInObject.ConfigCheckeds[ckFriendHit];
  chkSpecialQuickFlashing.Checked := PlugInObject.ConfigCheckeds[ckSpecialQuickFlashing];
  chkSceneShake.Checked := PlugInObject.ConfigCheckeds[ckSceneShake];

  chkHeroAutoShield.Checked := PlugInObject.ConfigCheckeds[ckHeroAutoShield];
  chkAssistantHeroAutoShield.Checked := PlugInObject.ConfigCheckeds[ckAssistantHeroAutoShield];

  chkHideTitle.Checked := PlugInObject.ConfigCheckeds[ckHideTitle];

  g_boRepeatBGSound := PlugInObject.ConfigCheckeds[ckRepeatBGMusic];

  chkNoRedPoison.Checked := PlugInObject.ConfigCheckeds[ckGJ_NoRedPoison];
  chkNoBluePoison.Checked := PlugInObject.ConfigCheckeds[ckGJ_NoBluePoison];
  chkNoDuFu.Checked := PlugInObject.ConfigCheckeds[ckGJ_NoDuFu];
  chkBagFull.Checked := PlugInObject.ConfigCheckeds[ckGJ_BagFull];
  chkLimitScreen.Checked := PlugInObject.ConfigCheckeds[ckGJ_LimitScreen];
  chkDFAvoid.Checked := PlugInObject.ConfigCheckeds[ckGJ_DFStopAvoid];

  chkPlayAttack.Checked := PlugInObject.ConfigCheckeds[ckGJ_PlayAttack];
  chkGroupAttack.Checked := PlugInObject.ConfigCheckeds[ckGJ_GroupAttack];
  chkNotRushMon.Checked := PlugInObject.ConfigCheckeds[ckGJ_NotRushMon];

  chkShowTargetAperture.Checked := PlugInObject.ConfigCheckeds[ckShowTargetAperture];

  chkSmartCustomHit1.Checked := PlugInObject.ConfigCheckeds[ckSmartCustomHit1];
  chkSmartCustomHit2.Checked := PlugInObject.ConfigCheckeds[ckSmartCustomHit2];
  chkSmartCustomHit3.Checked := PlugInObject.ConfigCheckeds[ckSmartCustomHit3];
  chkSmartCustomHit4.Checked := PlugInObject.ConfigCheckeds[ckSmartCustomHit4];
  chkSmartCustomHit5.Checked := PlugInObject.ConfigCheckeds[ckSmartCustomHit5];
  chkSmartCustomHit6.Checked := PlugInObject.ConfigCheckeds[ckSmartCustomHit6];
  chkSmartCustomHit7.Checked := PlugInObject.ConfigCheckeds[ckSmartCustomHit7];
  chkSmartCustomHit8.Checked := PlugInObject.ConfigCheckeds[ckSmartCustomHit8];

  chkHumManuallyCustomHit1.Checked := PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit1];
  chkHumManuallyCustomHit2.Checked := PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit2];
  chkHumManuallyCustomHit3.Checked := PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit3];
  chkHumManuallyCustomHit4.Checked := PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit4];
  chkHumManuallyCustomHit5.Checked := PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit5];
  chkShowValueItemEffect.Checked := PlugInObject.ConfigCheckeds[ckShowValueItemEffect];

  chkHideActorIcons.Checked := PlugInObject.ConfigCheckeds[ckHideActorIcons];

  chkHideMonsterIcons.Checked := PlugInObject.ConfigCheckeds[ckHideMonsterIcons]; //HZQ 20230601 增加3个Checkbox的选项
  chkAutoDetourPath.Checked := PlugInObject.ConfigCheckeds[ckAutoDetourPath];
  chkDimFireEffect.Checked := PlugInObject.ConfigCheckeds[ckDimFireEffect];

  chkNearEffect.Checked := PlugInObject.ConfigCheckeds[ckObjectHintEffect]; //HZQ 20230829

  SetRepeatBGSound(g_boRepeatBGSound);
end;

procedure TFrmJSYDlg.Open(AHandle:THandle);
begin
  //MainHandle := AHandle;
  RefConfig;

  Show;
end;

procedure TFrmJSYDlg.FormClose(Sender:TObject; var Action:TCloseAction);
begin
  if EditRenewHPTime1.Focused then
    EditRenewHPTime1.OnExit(EditRenewHPTime1)
  else if EditRenewHPTime2.Focused then
    EditRenewHPTime2.OnExit(EditRenewHPTime2)
  else if EditRenewMPTime.Focused then
    EditRenewMPTime.OnExit(EditRenewMPTime)
  else if EditRenewSpecialTime.Focused then
    EditRenewSpecialTime.OnExit(EditRenewSpecialTime)
  else if EditRenewBookTime1.Focused then
    EditRenewBookTime1.OnExit(EditRenewBookTime1)
  else if EditRenewBookTime2.Focused then
    EditRenewBookTime2.OnExit(EditRenewBookTime2)
  else if EditRenewLogOuttime.Focused then
    EditRenewLogOuttime.OnExit(EditRenewLogOuttime)
  else if EditRenewHeroHPTime1.Focused then
    EditRenewHeroHPTime1.OnExit(EditRenewHeroHPTime1)
  else if EditRenewHeroHPTime2.Focused then
    EditRenewHeroHPTime2.OnExit(EditRenewHeroHPTime2)
  else if EditRenewHeroMPTime.Focused then
    EditRenewHeroMPTime.OnExit(EditRenewHeroMPTime)
  else if EditRenewHeroSpecialTime.Focused then
    EditRenewHeroSpecialTime.OnExit(EditRenewHeroSpecialTime)
  else if EditRenewHeroLogOutTime.Focused then
    EditRenewHeroLogOutTime.OnExit(EditRenewHeroLogOutTime);

  Action := caHide;
end;

procedure TFrmJSYDlg.FormCloseQuery(Sender:TObject; var CanClose:Boolean);
begin
  // CanClose := GetForegroundWindow <> Handle;
  // TimerClose.Enabled := not CanClose;
end;

procedure TFrmJSYDlg.FormKeyDown(Sender:TObject; var Key:Word;
  Shift:TShiftState);
begin
  frmMain.FormKeyDown(Sender, Key, Shift);
end;

procedure TFrmJSYDlg.FormKeyPress(Sender:TObject; var Key:Char);
begin
  // frmMain.FormKeyPress(Sender, Key);
end;

procedure TFrmJSYDlg.FormCreate(Sender:TObject);
{$IF TESTMODE = 0}
var
  IniStream:TFastIniStream;
  {$IFEND}
begin
  pgMain.ActivePageIndex := 0;
  pgProtect.ActivePageIndex := 0;

  {$IF TESTMODE = 0}
  if (g_JSYUIStream <> nil) and (g_JSYUIStream.Size > 0) then begin
    IniStream := TFastIniStream.Create(g_JSYUIStream);
    try
      LoadUI(IniStream);
    finally
      IniStream.Free;
    end;
  end;
  {$IFEND}
end;

procedure TFrmJSYDlg.chkBGMusicClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckBGMusic] := chkBGMusic.Checked;

  if g_boBGSound <> PlugInObject.ConfigCheckeds[ckBGMusic] then begin
    g_boBGSound := PlugInObject.ConfigCheckeds[ckBGMusic];

    // 地图背景音乐也在这里控制 chongchong 2015-03-28
    if g_boBGSound then begin
      if FileExists(g_sMapMusic) then
        PlayMp3(g_sMapMusic, True);
    end else begin
      EnterCriticalSection(g_SoundLock);
      try
        try
          g_BassSound.StopMusic(g_sMapMusic);
        except
        end;
      finally
        LeaveCriticalSection(g_SoundLock);
      end;
    end;
  end;
end;

procedure TFrmJSYDlg.chkRepeatBGMusicClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckRepeatBGMusic] := chkRepeatBGMusic.Checked;
  g_boRepeatBGSound := PlugInObject.ConfigCheckeds[ckRepeatBGMusic];
  SetRepeatBGSound(g_boRepeatBGSound);
end;

procedure TFrmJSYDlg.chkShowGreenHintClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowGreenHint] := chkShowGreenHint.Checked;
end;

procedure TFrmJSYDlg.chkAutoOrderItemClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoOrderItem] := chkAutoOrderItem.Checked;
end;

procedure TFrmJSYDlg.chkOnlyShowCharNameClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckOnlyShowCharName] := chkOnlyShowCharName.Checked;
end;

procedure TFrmJSYDlg.chkDuraWarningClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckDuraWarning] := chkDuraWarning.Checked;
end;

procedure TFrmJSYDlg.chkNotNeedShiftClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckNotNeedShift] := chkNotNeedShift.Checked;
end;

procedure TFrmJSYDlg.chkShowHPLabelClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowHPLabel] := chkShowHPLabel.Checked;
end;

procedure TFrmJSYDlg.chkShowNumberLableClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowNumberLable] := chkShowNumberLable.Checked;
end;

procedure TFrmJSYDlg.chkShowJobAndLevelClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowJobAndLevel] := chkShowJobAndLevel.Checked;
end;

procedure TFrmJSYDlg.chkShowMoveLableClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowMoveLable] := chkShowMoveLable.Checked;
end;

procedure TFrmJSYDlg.chkShowMapDescClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowMapDesc] := chkShowMapDesc.Checked;
end;

procedure TFrmJSYDlg.chkShowHighlightHPLabelClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowHighlightHPLabel] := chkShowHighlightHPLabel.Checked;
end;

procedure TFrmJSYDlg.chkSmartLongHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartLongHit] := chkSmartLongHit.Checked;
end;

procedure TFrmJSYDlg.chkSmartFireHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartFireHit] := chkSmartFireHit.Checked;
end;

procedure TFrmJSYDlg.chkSmartWideHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartWideHit] := chkSmartWideHit.Checked;
end;

procedure TFrmJSYDlg.chkSmartPosLongHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartPosLongHit] := chkSmartPosLongHit.Checked;
end;

procedure TFrmJSYDlg.chkSmartSwordHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartSwordHit] := chkSmartSwordHit.Checked;
end;

procedure TFrmJSYDlg.chkSmartWalkLongHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartWalkLongHit] := chkSmartWalkLongHit.Checked;
end;

procedure TFrmJSYDlg.chkHumAutoShieldClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumAutoShield] := chkHumAutoShield.Checked;
end;

procedure TFrmJSYDlg.chkHumStruckShieldClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumStruckShield] := chkHumStruckShield.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallySnowWindClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallySnowWind] := chkHumManuallySnowWind.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallyFireBoomClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallyFireBoom] := chkHumManuallyFireBoom.Checked;
end;

procedure TFrmJSYDlg.chkHumShootLightenLockTargetClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumShootLightenLockTarget] := chkHumShootLightenLockTarget.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallyMeteorShowerClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallyMeteorShower] := chkHumManuallyMeteorShower.Checked;
end;

procedure TFrmJSYDlg.CheckBoxAutoHideModeClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoHideMode] := CheckBoxAutoHideMode.Checked;
end;

procedure TFrmJSYDlg.chkMagicLockClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckMagicLock] := chkMagicLock.Checked;
end;

procedure TFrmJSYDlg.chkDisableSelfStruckClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckDisableSelfStruck] := chkDisableSelfStruck.Checked;
end;

procedure TFrmJSYDlg.chkHideGhostClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHideGhost] := chkHideGhost.Checked;
end;

procedure TFrmJSYDlg.CheckBoxAutoUseMagicClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoUseMagic] := CheckBoxAutoUseMagic.Checked;
end;

procedure TFrmJSYDlg.chkExpFilterClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckFilterExp] := chkExpFilter.Checked;
end;

procedure TFrmJSYDlg.chkShowMonNameClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowMonName] := chkShowMonName.Checked;
end;

procedure TFrmJSYDlg.chkShowRadarPlayerClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowRadarPlayer] := chkShowRadarPlayer.Checked;
end;

procedure TFrmJSYDlg.chkShowRadarNpcClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowRadarNpc] := chkShowRadarNpc.Checked;
end;

procedure TFrmJSYDlg.chkShowRadarAttackNpcClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowRadarAttackNpc] := chkShowRadarAttackNpc.Checked;
end;

procedure TFrmJSYDlg.edtExpFilterChange(Sender:TObject);
begin
  g_Config.nFilterMinExp := StrToIntDef(edtExpFilter.Text, 0);
end;

procedure TFrmJSYDlg.lvFilterItemMouseDown(Sender:TObject;
  Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  ListItem:TListItem;
  ShowItem:pTShowItem;
  p:tpoint;
  iSubLeft, iSubRight, iCol:integer;
begin
  if lvFilterItem.ItemIndex >= 0 then begin
    p := lvFilterItem.Items[lvFilterItem.ItemIndex].Position;
    if lvFilterItem.Checkboxes then
      isubleft := p.X - 18
    else
      iSubLeft := p.X;
    iSubRight := lvFilterItem.Columns[0].Width + isubleft;
    iCol := 0;
    while (iCol < lvFilterItem.Columns.Count - 1) and ((x < iSubleft) or (x > iSubright)) do begin
      icol := icol + 1;
      iSubleft := iSubright;
      isubright := isubright + lvFilterItem.Columns[icol].Width;
    end;
    if (x < iSubleft) or (x > iSubright) then
      // showmessage('没找到位置，可能在滚动条上')
    else begin
      ListItem := lvFilterItem.Items.Item[lvFilterItem.itemindex];
      ShowItem := ListItem.Data;
      case iCol of
        0:begin
            edtSpecialName.Text := ShowItem.sItemName;
          end;
        1:begin
            ShowItem.boHintMsg := not ShowItem.boHintMsg;
            ListItem.SubItems.Strings[0] := GetSelectString(ShowItem.boHintMsg);
            g_FileItemDB.SaveToFile;
            g_DropItemsMgr.RefreshDrawList;
          end;
        2:begin
            ShowItem.boPickup := not ShowItem.boPickup;
            ListItem.SubItems.Strings[1] := GetSelectString(ShowItem.boPickup);

            g_IsClientPickItemsChanged := True;
            g_FileItemDB.SaveToFile;
            g_DropItemsMgr.RefreshDrawList;
          end;
        3:begin
            ShowItem.boShowName := not ShowItem.boShowName;
            ListItem.SubItems.Strings[2] := GetSelectString(ShowItem.boShowName);
            g_FileItemDB.SaveToFile;
            g_DropItemsMgr.RefreshDrawList;
          end;
        4:begin
            ShowItem.boShowSpecial := not ShowItem.boShowSpecial;
            ListItem.SubItems.Strings[3] := GetSelectString(ShowItem.boShowSpecial);

            g_IsClientPickItemsChanged := True;
            g_FileItemDB.SaveToFile;
            g_DropItemsMgr.RefreshDrawList;
          end;
        5:begin
            ShowItem.boAutoMove := not ShowItem.boAutoMove;
            ListItem.SubItems.Strings[4] := GetSelectString(ShowItem.boAutoMove);
            g_FileItemDB.SaveToFile;
            g_DropItemsMgr.RefreshDrawList;
          end;
      end;
    end;
    {if (x < iSubleft) or (x > iSubright) then
      showmessage('没找到位置，可能在滚动条上')
    else
      showmessage('你选的是第' + inttostr(listview1.itemindex + 1) + '行,第' + inttostr(iCol + 1) + '列'); }
  end;
end;

procedure TFrmJSYDlg.AddToBossList(sName:string);
begin
    if lstBoss.IndexOf(sName) < 0 then begin
        lstBoss.Add(sName);
        SaveOrLoadBossList(True);
    end;
end;

procedure TFrmJSYDlg.RemoveFromBossList(sName:string);
var
    nIndex:Integer;
begin
    nIndex := lstBoss.IndexOf(sName);
    if nIndex >= 0 then begin
        lstBoss.Items.Delete(nIndex);
        SaveOrLoadBossList(True);
    end;
end;

procedure TFrmJSYDlg.AddOrRemoveBossList(sName:string);
var
    nIndex:Integer;
begin
    nIndex := lstBoss.IndexOf(sName);
    if nIndex >= 0 then begin
        lstBoss.Items.Delete(nIndex);
    end else begin
        lstBoss.Add(sName);
    end;
    SaveOrLoadBossList(True);
end;

procedure TfrmJSYDlg.RefActorList;
var
  I:Integer;
  ListItem:TListItem;
  Actor:TActor;
  nX, nY:Integer;
begin
  if chkShowHum.Checked or chkShowMon.Checked or chkShowNpc.Checked then begin
    lvActor.Clear;
    {$IF IsMultiThreadRender = 1}
    PlayScene.m_ActorList.Lock;
    try
      {$IFEND}
      for I := 0 to PlayScene.m_ActorList.Count - 1 do begin
        if I >= PlayScene.m_ActorList.Count then break;
        Actor := TActor(PlayScene.m_ActorList.Items[I]);
        if (chkShowHum.Checked and (Actor.m_btRace in [0, 1])) or
          (chkShowMon.Checked and (Actor.m_btRace > 1) and (Actor.m_btRace <> RC_MERCHANT)) or
          (chkShowNpc.Checked and (Actor.m_btRace = RC_MERCHANT)) then begin
          ListItem := lvActor.Items.Add;
          ListItem.Caption := Actor.m_sUserName;
          nX := Actor.m_nCurrX;
          nY := Actor.m_nCurrY;
          ListItem.SubItems.Add(GetActorDir(nX, nY));
          ListItem.SubItems.Add(IntToStr(nX));
          ListItem.SubItems.Add(IntToStr(nY));
        end;
      end;
      {$IF IsMultiThreadRender = 1}
    finally
      PlayScene.m_ActorList.UnLock;
    end;
    {$IFEND}
  end;
end;

procedure TFrmJSYDlg.btnRefActorListClick(Sender:TObject);
begin
  RefActorList;
end;

procedure TFrmJSYDlg.EditSearchItemChange(Sender:TObject);
begin
  if EditSearchItem.Text = '' then begin
    ComboBoxItemStdModeSelect(Sender);
  end;
end;

procedure TFrmJSYDlg.ButtonSearchItemClick(Sender:TObject);
var
  I:Integer;
  List:TList;

  ShowItem:pTShowItem;
  ListItem:TListItem;
  sText:string;
begin
  sText := EditSearchItem.Text;
  if sText = '' then begin
    ComboBoxItemStdModeSelect(Sender);
  end
  else begin
    sText := EditSearchItem.Text;
    List := TList.Create;
    g_FileItemDB.Get(TItemType(ComboBoxItemStdMode.ItemIndex), List);
    lvFilterItem.Clear;
    for I := 0 to List.Count - 1 do begin
      ShowItem := List.Items[I];
      if AnsiContainsText(sText, ShowItem.sItemName) or AnsiContainsText(ShowItem.sItemName, sText) then begin
        ListItem := lvFilterItem.Items.Add;
        ListItem.Data := ShowItem;
        ListItem.Caption := ShowItem.sItemName;
        ListItem.SubItems.Add(GetSelectString(ShowItem.boHintMsg));
        ListItem.SubItems.Add(GetSelectString(ShowItem.boPickup));
        ListItem.SubItems.Add(GetSelectString(ShowItem.boShowName));
        ListItem.SubItems.Add(GetSelectString(ShowItem.boShowSpecial));
        ListItem.SubItems.Add(GetSelectString(ShowItem.boAutoMove));
      end;
    end;
    List.Free;
  end;
end;

procedure TFrmJSYDlg.ComboBoxItemStdModeSelect(Sender:TObject);
var
  I:Integer;
  List:TList;

  ShowItem:pTShowItem;
  ListItem:TListItem;
begin
  List := TList.Create;
  g_FileItemDB.Get(TItemType(ComboBoxItemStdMode.ItemIndex), List);
  lvFilterItem.Clear;
  for I := 0 to List.Count - 1 do begin
    ShowItem := List.Items[I];
    ListItem := lvFilterItem.Items.Add;
    ListItem.Data := ShowItem;
    ListItem.Caption := ShowItem.sItemName;
    ListItem.SubItems.Add(GetSelectString(ShowItem.boHintMsg));
    ListItem.SubItems.Add(GetSelectString(ShowItem.boPickup));
    ListItem.SubItems.Add(GetSelectString(ShowItem.boShowName));
    ListItem.SubItems.Add(GetSelectString(ShowItem.boShowSpecial));
    ListItem.SubItems.Add(GetSelectString(ShowItem.boAutoMove));
  end;
  List.Free;
end;

procedure TFrmJSYDlg.chkPercentProtectClick(Sender:TObject);
begin
  g_Config.boPercentProtect := chkPercentProtect.Checked;
  if g_Config.boPercentProtect then begin
    if StrToIntDef(EditRenewHPPercent1.Text, 0) > 99 then begin
      EditRenewHPPercent1.Text := '90';
      g_Config.nRenewHPPercent1 := StrToIntDef(EditRenewHPPercent1.Text, 0);
    end;

    if StrToIntDef(EditRenewHPPercent2.Text, 0) > 99 then begin
      EditRenewHPPercent2.Text := '50';
      g_Config.nRenewHPPercent2 := StrToIntDef(EditRenewHPPercent2.Text, 0);
    end;

    if StrToIntDef(EditRenewMPPercent.Text, 0) > 99 then begin
      EditRenewMPPercent.Text := '90';
      g_Config.nRenewMPPercent := StrToIntDef(EditRenewMPPercent.Text, 0);
    end;

    if StrToIntDef(EditRenewSpecialPercent.Text, 0) > 99 then begin
      EditRenewSpecialPercent.Text := '40';
      g_Config.nRenewSpecialPercent := StrToIntDef(EditRenewSpecialPercent.Text, 0);
    end;

    if StrToIntDef(EditRenewBookPercent1.Text, 0) > 99 then begin
      EditRenewBookPercent1.Text := '20';
      g_Config.nRenewBookPercent1 := StrToIntDef(EditRenewBookPercent1.Text, 0);
    end;

    if StrToIntDef(EditRenewBookPercent2.Text, 0) > 99 then begin
      EditRenewBookPercent2.Text := '10';
      g_Config.nRenewBookPercent2 := StrToIntDef(EditRenewBookPercent2.Text, 0);
    end;

    if StrToIntDef(EditRenewLogOutPercent.Text, 0) > 99 then begin
      EditRenewLogOutPercent.Text := '5';
      g_Config.nRenewLogOutPercent := StrToIntDef(EditRenewLogOutPercent.Text, 0);
    end;
  end;
end;

procedure TFrmJSYDlg.CheckBoxRenewHPIsAuto1Click(Sender:TObject);
begin
  g_Config.boRenewHPIsAuto1 := CheckBoxRenewHPIsAuto1.Checked;
  {
  ComboBoxRenewHPIsAuto1.Enabled := g_Config.boRenewHPIsAuto1;
  EditRenewHPPercent1.Enabled := g_Config.boRenewHPIsAuto1;
  EditRenewHPTime1.Enabled := g_Config.boRenewHPIsAuto1;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewHPIsAuto2Click(Sender:TObject);
begin
  g_Config.boRenewHPIsAuto2 := CheckBoxRenewHPIsAuto2.Checked;
  {
  ComboBoxRenewHPIsAuto2.Enabled := g_Config.boRenewHPIsAuto2;
  EditRenewHPPercent2.Enabled := g_Config.boRenewHPIsAuto2;
  EditRenewHPTime2.Enabled := g_Config.boRenewHPIsAuto2;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewMPIsAutoClick(Sender:TObject);
begin
  g_Config.boRenewMPIsAuto := CheckBoxRenewMPIsAuto.Checked;
  {
  ComboBoxRenewMPIsAuto.Enabled := g_Config.boRenewMPIsAuto;
  EditRenewMPPercent.Enabled := g_Config.boRenewMPIsAuto;
  EditRenewMPTime.Enabled := g_Config.boRenewMPIsAuto;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewSpecialIsAutoClick(Sender:TObject);
begin
  g_Config.boRenewSpecialIsAuto := CheckBoxRenewSpecialIsAuto.Checked;
  {
  ComboBoxRenewSpecialIsAuto.Enabled := g_Config.boRenewSpecialIsAuto;
  EditRenewSpecialPercent.Enabled := g_Config.boRenewSpecialIsAuto;
  EditRenewSpecialTime.Enabled := g_Config.boRenewSpecialIsAuto;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewBookIsAuto1Click(Sender:TObject);
begin
  g_Config.boRenewBookIsAuto1 := CheckBoxRenewBookIsAuto1.Checked;
  {
  ComboBoxRenewBookIsAuto1.Enabled := g_Config.boRenewBookIsAuto1;
  EditRenewBookPercent1.Enabled := g_Config.boRenewBookIsAuto1;
  EditRenewBookTime1.Enabled := g_Config.boRenewBookIsAuto1;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewBookIsAuto2Click(Sender:TObject);
begin
  g_Config.boRenewBookIsAuto2 := CheckBoxRenewBookIsAuto2.Checked;

  {
  ComboBoxRenewBookIsAuto2.Enabled := g_Config.boRenewBookIsAuto2;
  EditRenewBookPercent2.Enabled := g_Config.boRenewBookIsAuto2;
  EditRenewBookTime2.Enabled := g_Config.boRenewBookIsAuto2;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewLogOutClick(Sender:TObject);
begin
  g_Config.boRenewLogOutIsAuto := CheckBoxRenewLogOut.Checked;
end;

procedure TFrmJSYDlg.chkHeroPercentProtectClick(Sender:TObject);
begin
  g_Config.boHeroPercentProtect := chkHeroPercentProtect.Checked;
  if g_Config.boHeroPercentProtect then begin
    if StrToIntDef(EditRenewHeroHPPercent1.Text, 0) > 99 then begin
      EditRenewHeroHPPercent1.Text := '90';
      g_Config.nRenewHeroHPPercent1 := StrToIntDef(EditRenewHeroHPPercent1.Text, 0);
    end;

    if StrToIntDef(EditRenewHeroHPPercent2.Text, 0) > 99 then begin
      EditRenewHeroHPPercent2.Text := '50';
      g_Config.nRenewHeroHPPercent2 := StrToIntDef(EditRenewHeroHPPercent2.Text, 0);
    end;

    if StrToIntDef(EditRenewHeroMPPercent.Text, 0) > 99 then begin
      EditRenewHeroMPPercent.Text := '90';
      g_Config.nRenewHeroMPPercent := StrToIntDef(EditRenewHeroMPPercent.Text, 0);
    end;

    if StrToIntDef(EditRenewHeroSpecialPercent.Text, 0) > 99 then begin
      EditRenewHeroSpecialPercent.Text := '40';
      g_Config.nRenewHeroSpecialPercent := StrToIntDef(EditRenewHeroSpecialPercent.Text, 0);
    end;

    if StrToIntDef(EditRenewHeroLogOutPercent.Text, 0) > 99 then begin
      EditRenewHeroLogOutPercent.Text := '5';
      g_Config.nRenewHeroLogOutPercent := StrToIntDef(EditRenewHeroLogOutPercent.Text, 0);
    end;
  end;
end;

procedure TFrmJSYDlg.CheckBoxRenewHeroHPIsAuto1Click(Sender:TObject);
begin
  g_Config.boRenewHeroHPIsAuto1 := CheckBoxRenewHeroHPIsAuto1.Checked;

  {
  ComboBoxRenewHeroHPIsAuto1.Enabled := g_Config.boRenewHeroHPIsAuto1;
  EditRenewHeroHPPercent1.Enabled := g_Config.boRenewHeroHPIsAuto1;
  EditRenewHeroHPTime1.Enabled := g_Config.boRenewHeroHPIsAuto1;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewHeroHPIsAuto2Click(Sender:TObject);
begin
  g_Config.boRenewHeroHPIsAuto2 := CheckBoxRenewHeroHPIsAuto2.Checked;

  {
  ComboBoxRenewHeroHPIsAuto2.Enabled := g_Config.boRenewHeroHPIsAuto2;
  EditRenewHeroHPPercent2.Enabled := g_Config.boRenewHeroHPIsAuto2;
  EditRenewHeroHPTime2.Enabled := g_Config.boRenewHeroHPIsAuto2;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewHeroMPIsAutoClick(Sender:TObject);
begin
  g_Config.boRenewHeroMPIsAuto := CheckBoxRenewHeroMPIsAuto.Checked;

  {
  ComboBoxRenewHeroMPIsAuto.Enabled := g_Config.boRenewHeroMPIsAuto;
  EditRenewHeroMPPercent.Enabled := g_Config.boRenewHeroHPIsAuto2;
  EditRenewHeroMPTime.Enabled := g_Config.boRenewHeroHPIsAuto2;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewHeroSpecialIsAutoClick(Sender:TObject);
begin
  g_Config.boRenewHeroSpecialIsAuto := CheckBoxRenewHeroSpecialIsAuto.Checked;

  {
  ComboBoxRenewHeroSpecialIsAuto.Enabled := g_Config.boRenewHeroSpecialIsAuto;
  EditRenewHeroSpecialPercent.Enabled := g_Config.boRenewHeroSpecialIsAuto;
  EditRenewHeroSpecialTime.Enabled := g_Config.boRenewHeroSpecialIsAuto;
  }
end;

procedure TFrmJSYDlg.CheckBoxRenewHeroLogOutIsAutoClick(Sender:TObject);
begin
  g_Config.boRenewHeroLogOutIsAuto := CheckBoxRenewHeroLogOutIsAuto.Checked;
  EditRenewHeroLogOutPercent.Enabled := g_Config.boRenewHeroLogOutIsAuto;
  EditRenewHeroLogOutTime.Enabled := g_Config.boRenewHeroLogOutIsAuto;
end;

procedure TFrmJSYDlg.ComboBoxRenewHPIsAuto1Select(Sender:TObject);
begin
  if ComboBoxRenewHPIsAuto1.ItemIndex >= 0 then
    g_Config.sRenewHPItemName1 := ComboBoxRenewHPIsAuto1.Items.Strings[ComboBoxRenewHPIsAuto1.ItemIndex]
  else
    g_Config.sRenewHPItemName1 := '';
end;

procedure TFrmJSYDlg.ComboBoxRenewHPIsAuto2Select(Sender:TObject);
begin
  if ComboBoxRenewHPIsAuto2.ItemIndex >= 0 then
    g_Config.sRenewHPItemName2 := ComboBoxRenewHPIsAuto2.Items.Strings[ComboBoxRenewHPIsAuto2.ItemIndex]
  else
    g_Config.sRenewHPItemName2 := '';
end;

procedure TFrmJSYDlg.ComboBoxRenewMPIsAutoSelect(Sender:TObject);
begin
  if ComboBoxRenewMPIsAuto.ItemIndex >= 0 then
    g_Config.sRenewMPItemName := ComboBoxRenewMPIsAuto.Items.Strings[ComboBoxRenewMPIsAuto.ItemIndex]
  else
    g_Config.sRenewMPItemName := '';
end;

procedure TFrmJSYDlg.ComboBoxRenewSpecialIsAutoSelect(Sender:TObject);
begin
  if ComboBoxRenewSpecialIsAuto.ItemIndex >= 0 then
    g_Config.sRenewSpecialItemName := ComboBoxRenewSpecialIsAuto.Items.Strings[ComboBoxRenewSpecialIsAuto.ItemIndex]
  else
    g_Config.sRenewSpecialItemName := '';
end;

procedure TFrmJSYDlg.ComboBoxRenewBookIsAuto1Select(Sender:TObject);
begin
  if ComboBoxRenewBookIsAuto1.ItemIndex >= 0 then
    g_Config.sRenewBookItemName1 := ComboBoxRenewBookIsAuto1.Items.Strings[ComboBoxRenewBookIsAuto1.ItemIndex]
  else
    g_Config.sRenewBookItemName1 := '';
end;

procedure TFrmJSYDlg.ComboBoxRenewBookIsAuto2Select(Sender:TObject);
begin
  if ComboBoxRenewBookIsAuto2.ItemIndex >= 0 then
    g_Config.sRenewBookItemName2 := ComboBoxRenewBookIsAuto2.Items.Strings[ComboBoxRenewBookIsAuto2.ItemIndex]
  else
    g_Config.sRenewBookItemName2 := '';
end;

procedure TFrmJSYDlg.ComboBoxRenewHeroHPIsAuto1Select(Sender:TObject);
begin
  if ComboBoxRenewHeroHPIsAuto1.ItemIndex >= 0 then
    g_Config.sRenewHeroHPItemName1 := ComboBoxRenewHeroHPIsAuto1.Items.Strings[ComboBoxRenewHeroHPIsAuto1.ItemIndex]
  else
    g_Config.sRenewHeroHPItemName1 := '';
end;

procedure TFrmJSYDlg.ComboBoxRenewHeroHPIsAuto2Select(Sender:TObject);
begin
  if ComboBoxRenewHeroHPIsAuto2.ItemIndex >= 0 then
    g_Config.sRenewHeroHPItemName2 := ComboBoxRenewHeroHPIsAuto2.Items.Strings[ComboBoxRenewHeroHPIsAuto2.ItemIndex]
  else
    g_Config.sRenewHeroHPItemName2 := '';
end;

procedure TFrmJSYDlg.ComboBoxRenewHeroMPIsAutoSelect(Sender:TObject);
begin
  if ComboBoxRenewHeroMPIsAuto.ItemIndex >= 0 then
    g_Config.sRenewHeroMPItemName := ComboBoxRenewHeroMPIsAuto.Items.Strings[ComboBoxRenewHeroMPIsAuto.ItemIndex]
  else
    g_Config.sRenewHeroMPItemName := '';
end;

procedure TFrmJSYDlg.ComboBoxRenewHeroSpecialIsAutoSelect(Sender:TObject);
begin
  if ComboBoxRenewHeroSpecialIsAuto.ItemIndex >= 0 then
    g_Config.sRenewHeroSpecialItemName := ComboBoxRenewHeroSpecialIsAuto.Items.Strings[ComboBoxRenewHeroSpecialIsAuto.ItemIndex]
  else
    g_Config.sRenewHeroSpecialItemName := '';
end;

procedure TFrmJSYDlg.EditRenewHPPercent1Change(Sender:TObject);
begin
  g_Config.nRenewHPPercent1 := StrToIntDef(EditRenewHPPercent1.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewHPPercent2Change(Sender:TObject);
begin
  g_Config.nRenewHPPercent2 := StrToIntDef(EditRenewHPPercent2.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewMPPercentChange(Sender:TObject);
begin
  g_Config.nRenewMPPercent := StrToIntDef(EditRenewMPPercent.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewSpecialPercentChange(Sender:TObject);
begin
  g_Config.nRenewSpecialPercent := StrToIntDef(EditRenewSpecialPercent.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewBookPercent1Change(Sender:TObject);
begin
  g_Config.nRenewBookPercent1 := StrToIntDef(EditRenewBookPercent1.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewBookPercent2Change(Sender:TObject);
begin
  g_Config.nRenewBookPercent2 := StrToIntDef(EditRenewBookPercent2.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewLogOutPercentChange(Sender:TObject);
begin
  g_Config.nRenewLogOutPercent := StrToIntDef(EditRenewLogOutPercent.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewHPTime1Change(Sender:TObject);
begin
  g_Config.nRenewHPTime1 := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewHPTime1.Text, 0));
  EditRenewHPTime1.Text := IntToStr(g_Config.nRenewHPTime1);
end;

procedure TFrmJSYDlg.EditRenewHPTime2Change(Sender:TObject);
begin
  g_Config.nRenewHPTime2 := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewHPTime2.Text, 0));
  EditRenewHPTime2.Text := IntToStr(g_Config.nRenewHPTime2);
end;

procedure TFrmJSYDlg.EditRenewMPTimeChange(Sender:TObject);
begin
  g_Config.nRenewMPTime := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewMPTime.Text, 0));
  EditRenewMPTime.Text := IntToStr(g_Config.nRenewMPTime);
end;

procedure TFrmJSYDlg.EditRenewSpecialTimeChange(Sender:TObject);
begin
  g_Config.nRenewSpecialTime := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewSpecialTime.Text, 0));
  EditRenewSpecialTime.Text := IntToStr(g_Config.nRenewSpecialTime);
end;

procedure TFrmJSYDlg.EditRenewBookTime1Change(Sender:TObject);
begin
  g_Config.nRenewBookTime1 := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewBookTime1.Text, 0));
  EditRenewBookTime1.Text := IntToStr(g_Config.nRenewBookTime1);
end;

procedure TFrmJSYDlg.EditRenewBookTime2Change(Sender:TObject);
begin
  g_Config.nRenewBookTime2 := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewBookTime2.Text, 0));
  EditRenewBookTime2.Text := IntToStr(g_Config.nRenewBookTime2);
end;

procedure TFrmJSYDlg.EditRenewLogOuttimeChange(Sender:TObject);
begin
  g_Config.nRenewLogOuttime := StrToIntDef(EditRenewLogOuttime.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewHeroHPPercent1Change(Sender:TObject);
begin
  g_Config.nRenewHeroHPPercent1 := StrToIntDef(EditRenewHeroHPPercent1.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewHeroHPPercent2Change(Sender:TObject);
begin
  g_Config.nRenewHeroHPPercent2 := StrToIntDef(EditRenewHeroHPPercent2.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewHeroMPPercentChange(Sender:TObject);
begin
  g_Config.nRenewHeroMPPercent := StrToIntDef(EditRenewHeroMPPercent.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewHeroSpecialPercentChange(Sender:TObject);
begin
  g_Config.nRenewHeroSpecialPercent := StrToIntDef(EditRenewHeroSpecialPercent.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewHeroLogOutPercentChange(Sender:TObject);
begin
  g_Config.nRenewHeroLogOutPercent := StrToIntDef(EditRenewHeroLogOutPercent.Text, 0);
end;

procedure TFrmJSYDlg.EditRenewHeroHPTime1Change(Sender:TObject);
begin
  g_Config.nRenewHeroHPTime1 := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewHeroHPTime1.Text, 0));
  EditRenewHeroHPTime1.Text := IntToStr(g_Config.nRenewHeroHPTime1);
end;

procedure TFrmJSYDlg.EditRenewHeroHPTime2Change(Sender:TObject);
begin
  g_Config.nRenewHeroHPTime2 := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewHeroHPTime2.Text, 0));
  EditRenewHeroHPTime2.Text := IntToStr(g_Config.nRenewHeroHPTime2);
end;

procedure TFrmJSYDlg.EditRenewHeroMPTimeChange(Sender:TObject);
begin
  g_Config.nRenewHeroMPTime := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewHeroMPTime.Text, 0));
  EditRenewHeroMPTime.Text := IntToStr(g_Config.nRenewHeroMPTime);
end;

procedure TFrmJSYDlg.EditRenewHeroSpecialTimeChange(Sender:TObject);
begin
  g_Config.nRenewHeroSpecialTime := Max(g_ClientConfig.dwPluginMinEatItemTime, StrToIntDef(EditRenewHeroSpecialTime.Text, 0));
  EditRenewHeroSpecialTime.Text := IntToStr(g_Config.nRenewHeroSpecialTime);
end;

procedure TFrmJSYDlg.EditRenewHeroLogOutTimeChange(Sender:TObject);
begin
  g_Config.nRenewHeroLogOutTime := StrToIntDef(EditRenewHeroLogOutTime.Text, 0);
end;

procedure TFrmJSYDlg.EditAutoUseMagicChange(Sender:TObject);
begin
  g_Config.nAutoUseMagicTime := EditAutoUseMagic.Value;
end;

procedure TFrmJSYDlg.chkShowNpcHPLabelClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowNpcHPLabel] := chkShowNpcHPLabel.Checked;
end;

procedure TFrmJSYDlg.chkNotParalyClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckNotParaly] := chkNotParaly.Checked;
end;

procedure TFrmJSYDlg.chkShowUserNameClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowUserName] := chkShowUserName.Checked;
end;

procedure TFrmJSYDlg.chkHideHumEffectClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHideHumEffect] := chkHideHumEffect.Checked;
end;

procedure TFrmJSYDlg.chkHideWeaponEffectClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHideWeaponEffect] := chkHideWeaponEffect.Checked;
end;

procedure TFrmJSYDlg.chkShowHumClick(Sender:TObject);
begin
  btnRefActorListClick(Sender);
end;

procedure TFrmJSYDlg.lstUnBindItemListClick(Sender:TObject);
var
  BindItem:pTCustomBindItem;
begin
  if lstUnBindItemList.ItemIndex >= 0 then begin
    btnBindItemDel.Enabled := True;
    btnBindItemChg.Enabled := True;
    BindItem := pTCustomBindItem(lstUnBindItemList.Items.Objects[lstUnBindItemList.ItemIndex]);
    cbbGroupUnBindItem.ItemIndex := Integer(BindItem.UnBindItemType) - 1;
    edtItemName.Text := BindItem.sItemName;
    edtBindItemName.Text := BindItem.sBindItemName;
  end
  else begin
    btnBindItemDel.Enabled := False;
    btnBindItemChg.Enabled := False;
  end;
end;

procedure TFrmJSYDlg.btnBindItemAddClick(Sender:TObject);
var
  BindItem:pTCustomBindItem;
  sItemName:string;
  sBindItemName:string;
begin
  sItemName := edtItemName.Text;
  sBindItemName := edtBindItemName.Text;

  if (sItemName = '') then begin
    Application.MessageBox(PChar('请输入物品名称！'), '提示信息', MB_ICONQUESTION);
    edtItemName.SetFocus;
    Exit;
  end;

  New(BindItem);
  BindItem.UnBindItemType := TUnBindItemType(cbbGroupUnBindItem.ItemIndex + 1);
  BindItem.sItemName := sItemName;
  BindItem.sBindItemName := sBindItemName;
  g_CustomUnbindItemList.Add(BindItem);
  RefBindItemList;

  btnBindItemSave.Enabled := True;

  PlugInObject.RefreshUnBindItemList;
end;

procedure TFrmJSYDlg.btnBindItemDelClick(Sender:TObject);
var
  I:Integer;
  BindItem:pTCustomBindItem;
begin
  if lstUnBindItemList.ItemIndex >= 0 then begin
    btnBindItemDel.Enabled := False;
    btnBindItemChg.Enabled := False;
    BindItem := pTCustomBindItem(lstUnBindItemList.Items.Objects[lstUnBindItemList.ItemIndex]);
    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      if BindItem = g_CustomUnbindItemList.Items[I] then begin
        g_CustomUnbindItemList.Delete(I);
        Dispose(BindItem);
        break;
      end;
    end;
    RefBindItemList;
    btnBindItemSave.Enabled := True;
    PlugInObject.RefreshUnBindItemList;
  end
  else begin
    btnBindItemDel.Enabled := False;
    btnBindItemChg.Enabled := False;
  end;
end;

procedure TFrmJSYDlg.btnBindItemChgClick(Sender:TObject);
var
  BindItem:pTCustomBindItem;
  sItemName, sBindItemName:string;
begin
  btnBindItemDel.Enabled := False;
  btnBindItemChg.Enabled := False;
  sBindItemName := edtBindItemName.Text;
  sItemName := edtItemName.Text;

  if (sItemName = '') then begin
    Application.MessageBox(PChar('请输入物品名称！'), '提示信息', MB_ICONQUESTION);
    edtItemName.SetFocus;
    Exit;
  end;

  if lstUnBindItemList.ItemIndex >= 0 then begin
    BindItem := pTCustomBindItem(lstUnBindItemList.Items.Objects[lstUnBindItemList.ItemIndex]);
    BindItem.UnBindItemType := TUnBindItemType(cbbGroupUnBindItem.ItemIndex + 1);
    BindItem.sItemName := sItemName;
    BindItem.sBindItemName := sBindItemName;
    RefBindItemList;
    btnBindItemSave.Enabled := True;
    PlugInObject.RefreshUnBindItemList;
  end;
end;

procedure TFrmJSYDlg.btnBindItemSaveClick(Sender:TObject);
var
  sDirectory, sFileName:string;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);
  sFileName := sDirectory + g_sPlugServerName + DecodeResStr(SBindItemFileName);
  SaveNGCustomUnbindItemList(sFileName);
  btnBindItemSave.Enabled := False;
end;

procedure TFrmJSYDlg.chkSpeedSlowClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSpeedSlow] := chkSpeedSlow.Checked;
end;

procedure TFrmJSYDlg.CheckBoxAutoChangePoisonClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoChangePoison] := CheckBoxAutoChangePoison.Checked;
end;

procedure TFrmJSYDlg.chkSmartKTZHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmart66Hit] := chkSmartKTZHit.Checked;
end;

procedure TFrmJSYDlg.chkShowNPCNameClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowNpcName] := chkShowNPCName.Checked;
end;

procedure TFrmJSYDlg.btnSpecialAddClick(Sender:TObject);
var
  ShowItem:pTShowItem;
  sItemName:string;
  FileItem:pTShowItem;
begin
  sItemName := edtSpecialName.Text;
  if sItemName = '' then Exit;
  ShowItem := g_FileItemDB.Find(sItemName);
  if ShowItem = nil then begin
    New(ShowItem);
    ShowItem.ItemType := i_diy; //GetItemType(sItemType);
    ShowItem.sItemType := DecodeResStr(SCustomItemType); //sItemType;
    ShowItem.sItemName := sItemName;
    ShowItem.boHintMsg := False;
    ShowItem.boPickup := False;
    ShowItem.boShowName := False;
    ShowItem.boShowSpecial := False;
    ShowItem.boAutoMove := False;
    //m_ShowItemList.Add(ShowItem);
    g_FileItemDB.Add(ShowItem);
    New(FileItem);
    FileItem^ := ShowItem^;
    g_FileItemDB.m_FileItemList.Add(FileItem);
    //g_FileItemDB.SaveToFile;
    ComboBoxItemStdMode.ItemIndex := 8;
    ComboBoxItemStdModeSelect(Sender);
    edtSpecialName.Text := '';
  end;
end;

procedure TFrmJSYDlg.btnSpecialDelClick(Sender:TObject);
var
  I, Index:Integer;
  ListItem:TListItem;
  ShowItem:pTShowItem;
begin
  for I := lvFilterItem.Items.Count - 1 downto 0 do begin
    ListItem := lvFilterItem.Items.Item[I];
    if not ListItem.Selected then Continue;

    ShowItem := ListItem.Data;

    if (not ShowItem.boFromSystem) then begin
      Index := g_FileItemDB.m_ShowItemList.IndexOf(ShowItem);

      if Index >= 0 then begin
        Dispose(ShowItem);
        g_FileItemDB.m_ShowItemList.Delete(Index);

        if Index <= g_FileItemDB.m_FileItemList.Count - 1 then begin
          ShowItem := g_FileItemDB.m_FileItemList.Items[index];

          if ShowItem.boPickup or ShowItem.boShowSpecial then
            g_IsClientPickItemsChanged := True;

          Dispose(ShowItem);
          g_FileItemDB.m_FileItemList.Delete(Index);
        end;
      end;

      lvFilterItem.Items.Delete(I);
    end;
  end;

  g_FileItemDB.SaveToFile;
  g_DropItemsMgr.RefreshDrawList(True);
end;

procedure TFrmJSYDlg.seSpecialColorChange(Sender:TObject);
begin
  g_Config.nSpecialColor := seSpecialColor.Value;
  frmMain.nSpecialColor := g_Config.nSpecialColor;
end;

procedure TFrmJSYDlg.chkShowNGLabelClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowNGLabel] := chkShowNGLabel.Checked;
end;

function TFrmJSYDlg.CheckBossNameExists(Name:string; CurIndex:Integer):Boolean;
var
  I:Integer;
begin
  Result := False;
  for I := 0 to lstBoss.Items.Count - 1 do begin
    if SameText(Name, lstBoss.Items[I]) and (CurIndex <> I) then begin
      Result := True;
      Exit;
    end;
  end;
end;

procedure TFrmJSYDlg.SaveOrLoadBossList(IsSave:Boolean);
var
  sDirectory, sFileName:string;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  sFileName := g_sSelfFilePath + Format(BOSSCONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);
  if IsSave then begin
    lstBoss.Items.SaveToFile(sFileName);
    //PlugScrollBoxBoss.Position := 0;
  end
  else begin
    lstBoss.Clear;
    if FileExists(sFileName) then
      lstBoss.Items.LoadFromFile(sFileName);
    //PlugScrollBoxBoss.Position := 0;
    //PlugScrollBoxBoss.ItemIndex := -1;
    BtnBossModify.Enabled := False;
    BtnBossDel.Enabled := False;

    if lstBoss.Items.Text = '' then begin
      lstBoss.Items.Text := g_NGBossList.Text;
    end;
  end;

  g_BossList.Text := lstBoss.Items.Text;
end;

function TFrmJSYDlg.CheckGJMonNameExists(Name:string; CurIndex:Integer = -1):Boolean;
var
  I:Integer;
begin
  Result := False;
  for I := 0 to lstGJMon.Items.Count - 1 do begin
    if SameText(Name, lstGJMon.Items[I]) and (CurIndex <> I) then begin
      Result := True;
      Exit;
    end;
  end;
end;

procedure TFrmJSYDlg.SaveOrLoadGJMonList(IsSave:Boolean);
var
  sDirectory, sFileName:string;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  sFileName := g_sSelfFilePath + Format(GJMONCONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);
  if IsSave then begin
    lstGJMon.Items.SaveToFile(sFileName);
    //PlugScrollBoxMons.Position := 0;
  end
  else begin
    lstGJMon.Items.Clear;
    if FileExists(sFileName) then
      lstGJMon.Items.LoadFromFile(sFileName);
    //PlugScrollBoxMons.Position := 0;
    lstGJMon.ItemIndex := -1;
    btnGJMonEdit.Enabled := False;
    btnGJMonDel.Enabled := False;
  end;

  g_GJMonList.Text := lstGJMon.Items.Text;
end;

procedure TFrmJSYDlg.SaveOrLoadGJMagicList1(IsSave:Boolean);
var
  I, Value:Integer;
  sDirectory, sFileName:string;
  SL:TStringList;

  ListItem:TListItem;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  sFileName := g_sSelfFilePath + Format(GJNAGICCONFIGFILE1, [g_sPlugServerName, g_sPlugUserName]);
  g_GJUseMagic1.Clear;
  if IsSave then begin
    SL := TStringList.Create;
    try
      for I := 0 to lvGJMagic1.Items.Count - 1 do begin
        ListItem := lvGJMagic1.Items[I];
        if ListItem.SubItems.Count = 1 then begin
          if Length(ListItem.SubItems[0]) > 0 then begin
            g_GJUseMagic1.Add(ListItem.Data);
            SL.Add(IntToStr(Integer(ListItem.Data)));
          end;
        end;
      end;
      SL.SaveToFile(sFileName);
    finally
      SL.Free;
    end;
  end
  else begin
    lvGJMagic1.Clear;
    if not FileExists(sFileName) then Exit;

    SL := TStringList.Create;
    try
      SL.LoadFromFile(sFileName);

      for I := 0 to SL.Count - 1 do begin
        Value := StrToIntDef(SL[I], 0);
        if Value <> 0 then begin
          g_GJUseMagic1.Add(Pointer(Value));
        end;
      end;
    finally
      SL.Free;
    end;
  end;
end;

procedure TFrmJSYDlg.SaveOrLoadGJMagicList2(IsSave:Boolean);
var
  I, Value:Integer;
  sDirectory, sFileName:string;
  SL:TStringList;

  ListItem:TListItem;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  sFileName := g_sSelfFilePath + Format(GJNAGICCONFIGFILE2, [g_sPlugServerName, g_sPlugUserName]);
  g_GJUseMagic2.Clear;
  if IsSave then begin
    SL := TStringList.Create;
    try
      for I := 0 to lvGJMagic2.Items.Count - 1 do begin
        ListItem := lvGJMagic2.Items[I];
        if ListItem.SubItems.Count = 1 then begin
          if Length(ListItem.SubItems[0]) > 0 then begin
            g_GJUseMagic2.Add(ListItem.Data);
            SL.Add(IntToStr(Integer(ListItem.Data)));
          end;
        end;
      end;
      SL.SaveToFile(sFileName);
    finally
      SL.Free;
    end;
  end
  else begin
    lvGJMagic2.Clear;
    if not FileExists(sFileName) then Exit;

    SL := TStringList.Create;
    try
      SL.LoadFromFile(sFileName);

      for I := 0 to SL.Count - 1 do begin
        Value := StrToIntDef(SL[I], 0);
        if Value <> 0 then begin
          g_GJUseMagic2.Add(Pointer(Value));
        end;
      end;
    finally
      SL.Free;
    end;
  end;
end;

procedure TFrmJSYDlg.btnBossAddClick(Sender:TObject);
var
  BossName:string;
begin
  BossName := Trim(edtBossName.Text);
  if Length(BossName) = 0 then begin
    ShowMessage(DecodeResStr(SBossNameEmpty));
    Exit;
  end;
  if not CheckBossNameExists(BossName) then begin
    lstBoss.Items.Add(BossName);
    begin
      SaveOrLoadBossList(True);
      //PlugScrollBoxBoss.ItemIndex := PlugScrollBoxBoss.Lines.Count - 1;
    end;
  end
  else
    ShowMessage(DecodeResStr(SAddBossNameExists));
end;

procedure TFrmJSYDlg.btnBossDelClick(Sender:TObject);
begin
  if (lstBoss.ItemIndex >= 0) and (lstBoss.ItemIndex <= lstBoss.Items.Count - 1) then begin
    lstBoss.Items.Delete(lstBoss.ItemIndex);
    edtBossName.Text := '';
    SaveOrLoadBossList(True);
  end;
end;

procedure TFrmJSYDlg.BtnBossModifyClick(Sender:TObject);
var
  BossName:string;
begin
  BossName := Trim(edtBossName.Text);
  if Length(BossName) = 0 then begin
    ShowMessage(DecodeResStr(SBossNameEmpty));
    Exit;
  end;
  if not CheckBossNameExists(BossName, lstBoss.ItemIndex) then begin
    lstBoss.Items[lstBoss.ItemIndex] := BossName;
    SaveOrLoadBossList(True);
  end
  else
    ShowMessage(DecodeResStr(SEditBossNameExists));
end;

procedure TFrmJSYDlg.lstBossClick(Sender:TObject);
begin
  if (lstBoss.ItemIndex >= 0) and (lstBoss.ItemIndex <= lstBoss.Items.Count - 1) then begin
    edtBossName.Text := lstBoss.Items[lstBoss.ItemIndex];
    BtnBossModify.Enabled := True;
    BtnBossDel.Enabled := True;
  end
  else begin
    edtBossName.Text := '';
    BtnBossModify.Enabled := False;
    BtnBossDel.Enabled := False;
  end;
end;

procedure TFrmJSYDlg.chkNearEffectClick(Sender: TObject);
begin
   PlugInObject.ConfigCheckeds[ckObjectHintEffect] := chkNearEffect.Checked;
end;

procedure TFrmJSYDlg.chkNearHintClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckNearHint] := chkNearHint.Checked;
end;

procedure TFrmJSYDlg.chkAutoLockClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoLock] := chkAutoLock.Checked;
end;

procedure TFrmJSYDlg.chkColorShowClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckColorShow] := chkColorShow.Checked;
end;

procedure TFrmJSYDlg.cbbShowColorChange(Sender:TObject);
begin
  g_Config.nColorShowEff := cbbShowColor.ItemIndex;
  frmMain.nColorShowEff := g_Config.nColorShowEff;
end;

procedure TFrmJSYDlg.chkHeroAutoShieldClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHeroAutoShield] := chkHeroAutoShield.Checked;
end;

procedure TFrmJSYDlg.chkAssistantHeroAutoShieldClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAssistantHeroAutoShield] := chkAssistantHeroAutoShield.Checked;
end;

procedure TFrmJSYDlg.chkAutoSysMsgClick(Sender:TObject);
begin
  if Trim(edtSysMsg.Text) <> '' then begin
    g_AutoSysMsg := chkAutoSysMsg.Checked;
    g_AutoMsgTime := seAutoMsgTime.Value * 1000;
    g_AutoMsgTick := MyGetTickCount;
    g_AutoMsg := edtSysMsg.Text;
  end;
end;

procedure TFrmJSYDlg.seAutoMsgTimeChange(Sender:TObject);
begin
  g_AutoMsgTime := seAutoMsgTime.Value * 1000;
end;

procedure TFrmJSYDlg.chkBlacklistHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckBlacklistHit] := chkBlacklistHit.Checked;
end;

procedure TFrmJSYDlg.chkFriendHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckFriendHit] := chkFriendHit.Checked;
end;

procedure TFrmJSYDlg.chkSpecialQuickFlashingClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSpecialQuickFlashing] := chkSpecialQuickFlashing.Checked;
end;

procedure TFrmJSYDlg.chkSceneShakeClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSceneShake] := chkSceneShake.Checked;
end;

procedure TFrmJSYDlg.chkShiftSwitchClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShiftSwitch] := chkShiftSwitch.Checked;
end;

procedure TFrmJSYDlg.chkAutoDetourPathClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoDetourPath] := chkAutoDetourPath.Checked;
end;

procedure TFrmJSYDlg.chkAutoDownHorseClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoDownHorse] := chkAutoDownHorse.Checked;
end;

procedure TFrmJSYDlg.chkHideTitleClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHideTitle] := chkHideTitle.Checked;
end;

procedure TFrmJSYDlg.chkSmartCRSHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartCRSHit] := chkSmartCRSHit.Checked;
end;

procedure TFrmJSYDlg.chkSmartTWNHitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartTWNHit] := chkSmartTWNHit.Checked;
end;

procedure TFrmJSYDlg.chkShowRadarActorClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowRadarActor] := chkShowRadarActor.Checked;
end;

procedure TFrmJSYDlg.lstGJMonClick(Sender:TObject);
begin
  if (lstGJMon.ItemIndex >= 0) and (lstGJMon.ItemIndex <= lstGJMon.Items.Count - 1) then begin
    edtGJMon.Text := lstGJMon.Items[lstGJMon.ItemIndex];
    btnGJMonEdit.Enabled := True;
    btnGJMonDel.Enabled := True;
  end
  else begin
    edtGJMon.Text := '';
    btnGJMonEdit.Enabled := False;
    btnGJMonDel.Enabled := False;
  end;
end;

procedure TFrmJSYDlg.btnGJMonAddClick(Sender:TObject);
var
  MonName:string;
begin
  MonName := Trim(edtGJMon.Text);
  if Length(MonName) = 0 then begin
    ShowMessage(DecodeResStr(SMonNameEmpty));
    Exit;
  end;
  if not CheckGJMonNameExists(MonName) then begin
    lstGJMon.Items.Add(MonName);
    begin
      SaveOrLoadGJMonList(True);
      //PlugScrollBoxBoss.ItemIndex := PlugScrollBoxBoss.Lines.Count - 1;
    end;
  end
  else
    ShowMessage(DecodeResStr(SAddMonNameExists));
end;

procedure TFrmJSYDlg.btnGJMonDelClick(Sender:TObject);
begin
  if (lstGJMon.ItemIndex >= 0) and (lstGJMon.ItemIndex <= lstGJMon.Items.Count - 1) then begin
    lstGJMon.Items.Delete(lstGJMon.ItemIndex);
    edtGJMon.Text := '';
    SaveOrLoadGJMonList(True);
  end;
end;

procedure TFrmJSYDlg.btnGJMonEditClick(Sender:TObject);
var
  MonName:string;
begin
  MonName := Trim(edtGJMon.Text);
  if Length(MonName) = 0 then begin
    ShowMessage(DecodeResStr(SMonNameEmpty));
    Exit;
  end;
  if not CheckGJMonNameExists(MonName, lstGJMon.ItemIndex) then begin
    lstGJMon.Items[lstGJMon.ItemIndex] := MonName;
    SaveOrLoadGJMonList(True);
  end
  else
    ShowMessage(DecodeResStr(SEditMonNameExists));
end;

procedure TFrmJSYDlg.chkNoRedPoisonClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_NoRedPoison] := chkNoRedPoison.Checked;
  cbbNoRedPoisonOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_NoRedPoison];
end;

procedure TFrmJSYDlg.chkNoBluePoisonClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_NoBluePoison] := chkNoBluePoison.Checked;
  cbbNoBluePoisonOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_NoBluePoison];
end;

procedure TFrmJSYDlg.chkNoDuFuClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_NoDuFu] := chkNoDuFu.Checked;
  cbbNoDuFuOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_NoDuFu];
end;

procedure TFrmJSYDlg.chkBagFullClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_BagFull] := chkBagFull.Checked;
  cbbBagFullOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_BagFull];
end;

procedure TFrmJSYDlg.chkLimitScreenClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_LimitScreen] := chkLimitScreen.Checked;
end;

procedure TFrmJSYDlg.chkPlayAttackClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_PlayAttack] := chkPlayAttack.Checked;
  cbbPlayAttackOption.Enabled := PlugInObject.ConfigCheckeds[ckGJ_PlayAttack];
end;

procedure TFrmJSYDlg.cbbPlayAttackOptionChange(Sender:TObject);
begin
  if Sender = cbbPlayAttackOption then begin
    g_Config.nGJPlayAttackOption := cbbPlayAttackOption.ItemIndex;
    FrmMain.nGJPlayAttackOption := g_Config.nGJPlayAttackOption;
  end
  else if Sender = cbbNoRedPoisonOption then begin
    g_Config.nGJNoRedPoisonOption := cbbNoRedPoisonOption.ItemIndex;
    FrmMain.nGJNoRedPoisonOption := g_Config.nGJNoRedPoisonOption;
  end
  else if Sender = cbbNoBluePoisonOption then begin
    g_Config.nGJNoBluePoisonOption := cbbNoBluePoisonOption.ItemIndex;
    FrmMain.nGJNoBluePoisonOption := g_Config.nGJNoBluePoisonOption;
  end
  else if Sender = cbbNoDuFuOption then begin
    g_Config.nGJNoDuFuOption := cbbNoDuFuOption.ItemIndex;
    FrmMain.nGJNoDuFuOption := g_Config.nGJNoDuFuOption;
  end
  else if Sender = cbbBagFullOption then begin
    g_Config.nGJBagFullOption := cbbBagFullOption.ItemIndex;
    FrmMain.nGJBagFullOption := g_Config.nGJBagFullOption;
  end;
end;

procedure TFrmJSYDlg.chkGroupAttackClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_GroupAttack] := chkGroupAttack.Checked;
  seGroupAttackCount.Enabled := PlugInObject.ConfigCheckeds[ckGJ_GroupAttack];
end;

procedure TFrmJSYDlg.seGroupAttackCountChange(Sender:TObject);
begin
  g_Config.nGJGroupAttackCount := seGroupAttackCount.Value;
  FrmMain.nGJGroupAttackCount := g_Config.nGJGroupAttackCount;
end;

procedure TFrmJSYDlg.chkNotRushMonClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_NotRushMon] := chkNotRushMon.Checked;
  seNotRushMonRange.Enabled := PlugInObject.ConfigCheckeds[ckGJ_NotRushMon];
end;

procedure TFrmJSYDlg.seNotRushMonRangeChange(Sender:TObject);
begin
  g_Config.nGJNotRushMonRange := seNotRushMonRange.Value;
  FrmMain.nGJNotRushMonRange := g_Config.nGJNotRushMonRange;
end;

procedure TFrmJSYDlg.lvGJMagic1MouseDown(Sender:TObject;
  Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  ListItem:TListItem;
  P:TPoint;
  nSubLeft, nSubRight, nCol:integer;
  ListView:TRzListView;
begin
  ListView := Sender as TRzListView;

  if ListView.ItemIndex < 0 then Exit;

  P := ListView.Items[ListView.ItemIndex].Position;
  if ListView.Checkboxes then
    nSubLeft := P.X - 18
  else
    nSubLeft := P.X;

  nSubRight := ListView.Columns[0].Width + nSubLeft;
  nCol := 0;
  while (nCol < ListView.Columns.Count - 1) and ((x < nSubLeft) or (x > nSubRight)) do begin
    nCol := nCol + 1;
    nSubLeft := nSubRight;
    nSubRight := nSubRight + ListView.Columns[nCol].Width;
  end;

  if (X < nSubLeft) or (X > nSubRight) then begin
    // showmessage('没找到位置，可能在滚动条上')
  end
  else begin
    ListItem := ListView.Items.Item[ListView.itemindex];
    case nCol of
      1:begin
          if Length(ListItem.SubItems.Strings[0]) = 0 then
            ListItem.SubItems.Strings[0] := GetSelectString(True)
          else
            ListItem.SubItems.Strings[0] := GetSelectString(False);

          if ListView = lvGJMagic1 then
            SaveOrLoadGJMagicList1(True)
          else if ListView = lvGJMagic2 then
            SaveOrLoadGJMagicList2(True);
        end;
    end;
  end;
end;

procedure TFrmJSYDlg.btnGJRunClick(Sender:TObject);  
begin
  if not g_boGJRun then begin
    FrmMain.SendStartGJ;
  end else begin
    FrmMain.SendStopGJ;
  end;
end;

procedure TFrmJSYDlg.chkAutoPickupClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_AutoPickup] := chkAutoPickup.Checked;
end;

procedure TFrmJSYDlg.chkDFAvoidClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckGJ_DFStopAvoid] := chkDFAvoid.Checked;
end;

procedure TFrmJSYDlg.chkDimFireEffectClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckDimFireEffect] := chkDimFireEffect.Checked;
end;

procedure TFrmJSYDlg.chkDisableChartMemoSizeClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckDisableChartMemoSize] := chkDisableChartMemoSize.Checked;
end;

procedure TFrmJSYDlg.chkItemCompareClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckItemCompare] := chkItemCompare.Checked;
end;

{ TTrackBar }

procedure TTrackBar.CreateParams(var Params:TCreateParams);
begin
  inherited;
  Params.Style := Params.Style and (not TBS_ENABLESELRANGE);
end;

procedure TFrmJSYDlg.trbVolumeChange(Sender:TObject);
begin
  g_SoundVolume := trbVolume.Position;
end;

procedure TFrmJSYDlg.chkContinueButchItemClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckContinueButchItem] := chkContinueButchItem.Checked;
end;

procedure TFrmJSYDlg.chkCheckDuraClick(Sender:TObject);
begin
  g_Config.boCheckDuraIsAuto := chkCheckDura.Checked;
end;

procedure TFrmJSYDlg.seCheckDuraMinChange(Sender:TObject);
begin
  g_config.nCheckDuraMin := seCheckDuraMin.Value;
end;

procedure TFrmJSYDlg.edtCheckDuraItemChange(Sender:TObject);
begin
  g_Config.sCheckDuraItem := edtCheckDuraItem.Text;
end;

procedure TFrmJSYDlg.seCheckDuraTimeChange(Sender:TObject);
begin
  g_Config.nCheckDuraTime := seCheckDuraTime.Value;
end;

procedure TFrmJSYDlg.btnGJPointClick(Sender:TObject);
begin
  FrmMain.SendWantMiniMap(True);
end;

procedure TFrmJSYDlg.chkDisableDealClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckDisableDeal] := chkDisableDeal.Checked;
  frmMain.SendPlugInConfig(0);
end;

procedure TFrmJSYDlg.chkShowUpdateStatusClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowUpdateStatus] := chkShowUpdateStatus.Checked;
  frmMain.ShowUpdateStatusDlg(chkShowUpdateStatus.Checked);
end;

procedure TFrmJSYDlg.chkSimpleShowActorClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSimpleShowActor] := chkSimpleShowActor.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallyMove10AttackClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallyMove10Attack] := chkHumManuallyMove10Attack.Checked;
end;

procedure TFrmJSYDlg.chkSimpleShowHumanDressClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSimpleShowHumanDress] := chkSimpleShowHumanDress.Checked;
end;

procedure TFrmJSYDlg.chkHideItemEffectClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHideItemEffect] := chkHideItemEffect.Checked;
end;

procedure TFrmJSYDlg.chkHideMonsterIconsClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHideMonsterIcons] := chkHideMonsterIcons.Checked;
end;

procedure TFrmJSYDlg.hkCustom1KeyDown(Sender:TObject; var Key:Word;
  Shift:TShiftState);
var
  I:Integer;
  HotKey:TEdit;
  Magic:PTClientMagic;
  TempKey:Char;
begin
  HotKey := TEdit(Sender);
  if (HotKey.Tag >= 0) and (HotKey.Tag < Length(g_ShortcutKeys)) then begin
    for I := 0 to Length(g_ShortcutKeys) - 1 do begin
      if (g_ShortcutKeys[I].Key = Key) and (g_ShortcutKeys[I].Shift = Shift) then begin
        g_ShortcutKeys[I].Use := False;
        g_ShortcutKeys[I].Key := 0;
        g_ShortcutKeys[I].Shift := [];
      end;
    end;
    g_ShortcutKeys[HotKey.Tag].Use := True;
    g_ShortcutKeys[HotKey.Tag].Key := Key;
    g_ShortcutKeys[HotKey.Tag].Shift := Shift;

    RefKeyBoardConfig;

    // 取消掉相同的技能快捷键 2019-11-19 23:56:33
    if (Shift = []) and (Key >= Ord('A')) and (Key <= Ord('Z')) then begin
      TempKey := Chr(Key + 32);
      for I := 0 to g_MagicList.Count - 1 do begin
        Magic := PTClientMagic(g_MagicList[I]);

        if (Magic.Key = TempKey) then begin
          Magic.Key := #0;

          frmMain.SendMagicKeyChange(Magic.Def.wMagicId, #0);
          FrmDlg.DelScreenMagicButton(Magic.Def.wMagicId);
        end;
      end;
    end
    else if (Key in [VK_F1, VK_F8]) then begin
      if ssCtrl in Shift then begin
        TempKey := Char((Key - VK_F1) + byte('E'));
      end
      else begin
        TempKey := Char((Key - VK_F1) + byte('1'));
      end;

      for I := 0 to g_MagicList.Count - 1 do begin
        Magic := PTClientMagic(g_MagicList[I]);

        if (Magic.Key = TempKey) then begin
          Magic.Key := #0;

          frmMain.SendMagicKeyChange(Magic.Def.wMagicId, #0);
          FrmDlg.DelScreenMagicButton(Magic.Def.wMagicId);
        end;
      end;
    end;
    // 取消掉相同的技能快捷键 2019-11-19 23:56:33

    Key := 0;
  end;
end;

procedure TFrmJSYDlg.hkCustom1MouseDown(Sender:TObject;
  Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  HotKey:TEdit;
begin
  if Button = mbRight then begin
    HotKey := TEdit(Sender);
    if (HotKey.Tag >= 0) and (HotKey.Tag < Length(g_ShortcutKeys)) then begin
      g_ShortcutKeys[HotKey.Tag].Use := False;
      g_ShortcutKeys[HotKey.Tag].Key := 0;
      g_ShortcutKeys[HotKey.Tag].Shift := [];
      RefKeyBoardConfig;
    end;
  end;
end;

procedure TFrmJSYDlg.RefKeyBoardConfig;
begin
  hkCustom1.Text := GetKeyDownStr(g_ShortcutKeys[0].Key, g_ShortcutKeys[0].Shift, True);
  hkCustom2.Text := GetKeyDownStr(g_ShortcutKeys[1].Key, g_ShortcutKeys[1].Shift, True);
  hkCustom3.Text := GetKeyDownStr(g_ShortcutKeys[2].Key, g_ShortcutKeys[2].Shift, True);
  hkCustom4.Text := GetKeyDownStr(g_ShortcutKeys[3].Key, g_ShortcutKeys[3].Shift, True);
  hkCustom5.Text := GetKeyDownStr(g_ShortcutKeys[4].Key, g_ShortcutKeys[4].Shift, True);
  hkCustom6.Text := GetKeyDownStr(g_ShortcutKeys[5].Key, g_ShortcutKeys[5].Shift, True);
  hkCustom7.Text := GetKeyDownStr(g_ShortcutKeys[6].Key, g_ShortcutKeys[6].Shift, True);
  hkCustom8.Text := GetKeyDownStr(g_ShortcutKeys[7].Key, g_ShortcutKeys[7].Shift, True);
  hkCustom9.Text := GetKeyDownStr(g_ShortcutKeys[8].Key, g_ShortcutKeys[8].Shift, True);
  hkCustom10.Text := GetKeyDownStr(g_ShortcutKeys[9].Key, g_ShortcutKeys[9].Shift, True);
  hkCustom11.Text := GetKeyDownStr(g_ShortcutKeys[10].Key, g_ShortcutKeys[10].Shift, True);
  hkCustom12.Text := GetKeyDownStr(g_ShortcutKeys[11].Key, g_ShortcutKeys[11].Shift, True);
  hkCustom13.Text := GetKeyDownStr(g_ShortcutKeys[12].Key, g_ShortcutKeys[12].Shift, True);
  hkCustom14.Text := GetKeyDownStr(g_ShortcutKeys[13].Key, g_ShortcutKeys[13].Shift, True);
  hkCustom15.Text := GetKeyDownStr(g_ShortcutKeys[14].Key, g_ShortcutKeys[14].Shift, True);
  hkCustom16.Text := GetKeyDownStr(g_ShortcutKeys[15].Key, g_ShortcutKeys[15].Shift, True);
end;

procedure TFrmJSYDlg.chkEnabledHotKeyClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckUseKeyBoard] := chkEnabledHotKey.Checked;
end;

procedure TFrmJSYDlg.LoadNotesFile;
var
  sDirectory, sFileName:string;
begin
  // / \: * ?" <> |
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  if g_sPlugUserName = '' then Exit;

  sFileName := g_sSelfFilePath + Format(NOTESFILE, [g_sPlugServerName, g_sPlugUserName]);
  if FileExists(sFileName) then begin
    mmoNotes.OnChange := nil;
    mmoNotes.Lines.LoadFromFile(sFileName);
    mmoNotes.OnChange := mmoNotesChange;
  end;
end;

procedure TFrmJSYDlg.SaveNotesFile;
var
  sDirectory, sFileName:string;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;
  if g_sPlugUserName = '' then Exit;
  sFileName := g_sSelfFilePath + Format(NOTESFILE, [g_sPlugServerName, g_sPlugUserName]);
  mmoNotes.Lines.SaveToFile(sFileName);
end;

procedure TFrmJSYDlg.mmoNotesChange(Sender:TObject);
begin
  SaveNotesFile;
end;

procedure TFrmJSYDlg.chkAutoGroupAttackClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoGroupAttack] := chkAutoGroupAttack.Checked;
end;

procedure TFrmJSYDlg.chkAutoGroupNoAttackMonClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoGroupNoAttackMon] := chkAutoGroupNoAttackMon.Checked;
end;

function TFrmJSYDlg.ProcessKeyDown:Boolean;
begin
  Result := Visible and (
    hkCustom1.Focused or
    hkCustom2.Focused or
    hkCustom3.Focused or
    hkCustom4.Focused or
    hkCustom5.Focused or
    hkCustom6.Focused or
    hkCustom7.Focused or
    hkCustom8.Focused or
    hkCustom9.Focused or
    hkCustom10.Focused or
    hkCustom11.Focused or
    hkCustom12.Focused or
    hkCustom13.Focused or
    hkCustom14.Focused or
    hkCustom15.Focused or
    hkCustom16.Focused or
    edtExpFilter.Focused or
    edtItemName.Focused or
    edtBindItemName.Focused or
    edtBossName.Focused or
    edtSysMsg.Focused or
    edtSpecialName.Focused or
    EditSearchItem.Focused or
    edtCheckDuraItem.Focused or
    edtGJMon.Focused or
    mmoNotes.Focused
    );
end;

procedure TFrmJSYDlg.hkCustom9ContextPopup(Sender:TObject;
  MousePos:TPoint; var Handled:Boolean);
begin
  Handled := True;
end;

procedure TFrmJSYDlg.chkBagFastItemCompareClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckBagFastItemCompare] := chkBagFastItemCompare.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallyFireClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallyFire] := chkHumManuallyFire.Checked;
end;

procedure TFrmJSYDlg.chkShowHPUnitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowHPUnit] := chkShowHPUnit.Checked;
end;

procedure TFrmJSYDlg.mniEnableClick(Sender:TObject);
var
  I:Integer;
  ListItem:TListItem;
  ShowItem:pTShowItem;
  MenuItem:TMenuItem;
begin
  MenuItem := Sender as TMenuItem;
  if MenuItem.Parent = nil then Exit;

  for I := 0 to lvFilterItem.Items.Count - 1 do begin
    ListItem := lvFilterItem.Items.Item[I];
    if not ListItem.Selected then Continue;

    ShowItem := ListItem.Data;

    case MenuItem.Parent.Tag of
      0:begin
          ShowItem.boHintMsg := True;
          ListItem.SubItems.Strings[0] := GetSelectString(ShowItem.boHintMsg);
        end;
      1:begin
          ShowItem.boPickup := True;
          ListItem.SubItems.Strings[1] := GetSelectString(ShowItem.boPickup);

          g_IsClientPickItemsChanged := True;
        end;
      2:begin
          ShowItem.boShowName := True;
          ListItem.SubItems.Strings[2] := GetSelectString(ShowItem.boShowName);
        end;
      3:begin
          ShowItem.boShowSpecial := True;
          ListItem.SubItems.Strings[3] := GetSelectString(ShowItem.boShowSpecial);

          g_IsClientPickItemsChanged := True;
        end;
      4:begin
          ShowItem.boAutoMove := True;
          ListItem.SubItems.Strings[4] := GetSelectString(ShowItem.boAutoMove);
        end;
    end;
  end;

  g_FileItemDB.SaveToFile;
  g_DropItemsMgr.RefreshDrawList;
end;

procedure TFrmJSYDlg.mniDisableClick(Sender:TObject);
var
  I:Integer;
  ListItem:TListItem;
  ShowItem:pTShowItem;
  MenuItem:TMenuItem;
begin
  MenuItem := Sender as TMenuItem;
  if MenuItem.Parent = nil then Exit;

  for I := 0 to lvFilterItem.Items.Count - 1 do begin
    ListItem := lvFilterItem.Items.Item[I];
    if not ListItem.Selected then Continue;

    ShowItem := ListItem.Data;

    case MenuItem.Parent.Tag of
      0:begin
          ShowItem.boHintMsg := False;
          ListItem.SubItems.Strings[0] := GetSelectString(ShowItem.boHintMsg);
        end;
      1:begin
          ShowItem.boPickup := False;
          ListItem.SubItems.Strings[1] := GetSelectString(ShowItem.boPickup);

          g_IsClientPickItemsChanged := True;
        end;
      2:begin
          ShowItem.boShowName := False;
          ListItem.SubItems.Strings[2] := GetSelectString(ShowItem.boShowName);
        end;
      3:begin
          ShowItem.boShowSpecial := False;
          ListItem.SubItems.Strings[3] := GetSelectString(ShowItem.boShowSpecial);

          g_IsClientPickItemsChanged := True;
        end;
      4:begin
          ShowItem.boAutoMove := False;
          ListItem.SubItems.Strings[4] := GetSelectString(ShowItem.boAutoMove);
        end;
    end;
  end;

  g_FileItemDB.SaveToFile;
  g_DropItemsMgr.RefreshDrawList;
end;

procedure TFrmJSYDlg.mniReverseClick(Sender:TObject);
var
  I:Integer;
  ListItem:TListItem;
  ShowItem:pTShowItem;
  MenuItem:TMenuItem;
begin
  MenuItem := Sender as TMenuItem;
  if MenuItem.Parent = nil then Exit;

  for I := 0 to lvFilterItem.Items.Count - 1 do begin
    ListItem := lvFilterItem.Items.Item[I];
    if not ListItem.Selected then Continue;

    ShowItem := ListItem.Data;

    case MenuItem.Parent.Tag of
      0:begin
          ShowItem.boHintMsg := not ShowItem.boHintMsg;
          ListItem.SubItems.Strings[0] := GetSelectString(ShowItem.boHintMsg);
        end;
      1:begin
          ShowItem.boPickup := not ShowItem.boPickup;
          ListItem.SubItems.Strings[1] := GetSelectString(ShowItem.boPickup);

          g_IsClientPickItemsChanged := True;
        end;
      2:begin
          ShowItem.boShowName := not ShowItem.boShowName;
          ListItem.SubItems.Strings[2] := GetSelectString(ShowItem.boShowName);
        end;
      3:begin
          ShowItem.boShowSpecial := not ShowItem.boShowSpecial;
          ListItem.SubItems.Strings[3] := GetSelectString(ShowItem.boShowSpecial);

          g_IsClientPickItemsChanged := True;
        end;
      4:begin
          ShowItem.boAutoMove := not ShowItem.boAutoMove;
          ListItem.SubItems.Strings[4] := GetSelectString(ShowItem.boAutoMove);
        end;
    end;
  end;

  g_FileItemDB.SaveToFile;
  g_DropItemsMgr.RefreshDrawList;
end;

procedure TFrmJSYDlg.chkPickupAllClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckPickupAll] := chkPickupAll.Checked;
end;

procedure ReadFont(IniStream:TFastIniStream; Section:string; Font:TFont);
var
  FStyle:Integer;
begin
  Font.Name := IniStream.ReadString(Section, 'F.Name', Font.Name);
  Font.Size := IniStream.ReadInteger(Section, 'F.Size', Font.Size);
  Font.Color := IniStream.ReadInteger(Section, 'F.Color', Font.Color);

  Font.Style := [];
  FStyle := IniStream.ReadInteger(Section, 'F.Style', 0);
  if FStyle and 1 <> 0 then
    Font.Style := Font.Style + [fsBold];

  if FStyle and 2 <> 0 then
    Font.Style := Font.Style + [fsItalic];

  if FStyle and 4 <> 0 then
    Font.Style := Font.Style + [fsUnderline];

  if FStyle and 8 <> 0 then
    Font.Style := Font.Style + [fsStrikeOut];
end;

procedure TFrmJSYDlg.LoadUI(IniStream:TFastIniStream);
begin
  ClientWidth := IniStream.ReadInteger('FrmJSYDlg', 'CW', ClientWidth);
  ClientHeight := IniStream.ReadInteger('FrmJSYDlg', 'CH', ClientHeight);
  Caption := IniStream.ReadString('FrmJSYDlg', 'C', Caption);

  ReadFont(IniStream, 'FrmJSYDlg', Font);

  LoadSubControls(Self, IniStream);
end;

procedure TFrmJSYDlg.LoadSubControls(Parent:TWinControl;
  IniStream:TFastIniStream);
var
  I:Integer;
  Ctrl:TControl;
  SectionName:string;

  PI:PPropInfo;
  Value:Variant;
  Obj:TObject;
  Font:TFont;
begin
  if Parent.ControlCount > 0 then begin
    if (Parent is TColorIndexEdit) or (Parent is TSpinEditEx) or (Parent is TSpinEdit) then begin
      Exit;
    end;

    for I := 0 to Parent.ControlCount - 1 do begin
      Ctrl := Parent.Controls[I];

      SectionName := Ctrl.Name;
      Ctrl.Left := IniStream.ReadInteger(SectionName, 'L', Ctrl.Left);
      Ctrl.Top := IniStream.ReadInteger(SectionName, 'T', Ctrl.Top);
      Ctrl.Width := IniStream.ReadInteger(SectionName, 'W', Ctrl.Width);
      Ctrl.Height := IniStream.ReadInteger(SectionName, 'H', Ctrl.Height);

      PI := GetPropInfo(Ctrl, 'Visible', tkAny);
      if PI <> nil then begin
        Ctrl.Visible := IniStream.ReadBoolean(SectionName, 'V', Ctrl.Visible);
      end;

      PI := GetPropInfo(Ctrl, 'Caption', tkAny);
      if PI <> nil then begin
        Value := GetPropValue(Ctrl, 'Caption');
        if Value <> null then begin
          SetPropValue(Ctrl, 'Caption', IniStream.ReadString(SectionName, 'C', Value));
        end;
      end;

      PI := GetPropInfo(Ctrl, 'Hint', tkAny);
      if PI <> nil then begin
        Value := GetPropValue(Ctrl, 'Hint');
        if Value <> null then begin
          SetPropValue(Ctrl, 'Hint', IniStream.ReadString(SectionName, 'Hint', Value));
        end;
      end;

      PI := GetPropInfo(Ctrl, 'Font', tkAny);
      if PI <> nil then begin
        Obj := GetObjectProp(Ctrl, 'Font');
        if Obj is TFont then begin
          Font := TFont(Obj);

          ReadFont(IniStream, SectionName, Font);
        end;
      end;

      if Ctrl is TWinControl then begin
        LoadSubControls(TWinControl(Ctrl), IniStream);
      end;
    end;
  end;
end;

procedure TFrmJSYDlg.chkAutoPickupItemClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoPickUpItem] := chkAutoPickupItem.Checked;
end;

procedure TFrmJSYDlg.chkHeroShowNumberStateClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHeroShowNumberState] := chkHeroShowNumberState.Checked;
end;

procedure TFrmJSYDlg.lblItemsExportClick(Sender:TObject);
var
  SaveDlg:TSaveDialog;
  FileName:string;
begin
  SaveDlg := TSaveDialog.Create(nil);
  try
    SaveDlg.Filter := '文本文件(*.txt)|*.txt';
    SaveDlg.Title := DecodeResStr(SExportItemTitle);
    SaveDlg.FileName := DecodeResStr(SExportItemFileName);
    if SaveDlg.Execute then begin
      FileName := ChangeFileExt(SaveDlg.FileName, '.txt');
      g_FileItemDB.ExportToFile(FileName);
      DScreen.AddChatBoardString(DecodeResStr(SExportItemOK), GetRGB(219), clWhite);
    end;
  finally
    SaveDlg.Free;
  end;
end;

procedure TFrmJSYDlg.lblItemImportClick(Sender:TObject);
var
  IsLoad:Boolean;
  OpenDlg:TOpenDialog;
begin
  IsLoad := False;
  OpenDlg := TOpenDialog.Create(nil);
  try
    OpenDlg.Filter := '文本文件(*.txt)|*.txt';
    OpenDlg.Title := DecodeResStr(SImportItemTitle);
    OpenDlg.FileName := DecodeResStr(SExportItemFileName);
    if OpenDlg.Execute then begin
      g_FileItemDB.ImportFormFile(OpenDlg.FileName);
      IsLoad := True;
    end;
  finally
    OpenDlg.Free;
  end;

  if IsLoad then begin
    RefShowItem;

    g_IsClientPickItemsChanged := True;

    g_FileItemDB.SaveToFile;
    g_DropItemsMgr.RefreshDrawList(True);
    DScreen.AddChatBoardString(DecodeResStr(SImportItemOK), GetRGB(219), clWhite);
  end;
end;

procedure TFrmJSYDlg.lblItemsExportMouseEnter(Sender:TObject);
begin
  (Sender as TLabel).Font.Style := [fsUnderline];
end;

procedure TFrmJSYDlg.lblItemsExportMouseLeave(Sender:TObject);
begin
  (Sender as TLabel).Font.Style := [];
end;

procedure TFrmJSYDlg.chkShowTargetApertureClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowTargetAperture] := chkShowTargetAperture.Checked;
end;

procedure TFrmJSYDlg.chkNoCatonClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckNoCaton] := chkNoCaton.Checked;
end;

procedure TFrmJSYDlg.chkSmart113HitClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmart113Hit] := chkSmart113Hit.Checked;
end;

procedure TFrmJSYDlg.chkAutoOpenSpellClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoOpenSpell] := chkAutoOpenSpell.Checked;
end;

procedure TFrmJSYDlg.hkCustom1KeyPress(Sender:TObject; var Key:Char);
begin
  Key := #0;
end;

procedure TFrmJSYDlg.chkSimpleShowBBClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSimpleShowBB] := chkSimpleShowBB.Checked;
end;

procedure TFrmJSYDlg.lblItemEditClick(Sender:TObject);
var
  SL:TStringList;
begin
  SL := TStringList.Create;
  try
    g_FileItemDB.ExportToStrings(SL);
    if ShowFrmNGItemEdit(SL) then begin
      g_FileItemDB.ImportFormStrings(SL);
      RefShowItem;
      g_IsClientPickItemsChanged := True;
      g_FileItemDB.SaveToFile;
    end;
  finally
    SL.Free;
  end;

  if g_IsClientPickItemsChanged then
    g_DropItemsMgr.RefreshDrawList(True);
end;

procedure TFrmJSYDlg.chkSmartCustomHit1Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartCustomHit1] := chkSmartCustomHit1.Checked;
end;

procedure TFrmJSYDlg.chkSmartCustomHit2Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartCustomHit2] := chkSmartCustomHit2.Checked;
end;

procedure TFrmJSYDlg.chkSmartCustomHit3Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartCustomHit3] := chkSmartCustomHit3.Checked;
end;

procedure TFrmJSYDlg.chkSmartCustomHit4Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartCustomHit4] := chkSmartCustomHit4.Checked;
end;

procedure TFrmJSYDlg.chkSmartCustomHit5Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartCustomHit5] := chkSmartCustomHit5.Checked;
end;

procedure TFrmJSYDlg.chkSmartCustomHit6Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartCustomHit6] := chkSmartCustomHit6.Checked;
end;

procedure TFrmJSYDlg.chkSmartCustomHit7Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartCustomHit7] := chkSmartCustomHit7.Checked;
end;

procedure TFrmJSYDlg.chkSmartCustomHit8Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSmartCustomHit7] := chkSmartCustomHit7.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallyCustomHit1Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit1] := chkHumManuallyCustomHit1.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallyCustomHit2Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit2] := chkHumManuallyCustomHit2.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallyCustomHit3Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit3] := chkHumManuallyCustomHit3.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallyCustomHit4Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit4] := chkHumManuallyCustomHit4.Checked;
end;

procedure TFrmJSYDlg.chkHumManuallyCustomHit5Click(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHumManuallyCustomHit5] := chkHumManuallyCustomHit5.Checked;
end;

procedure TFrmJSYDlg.chkShowValueItemEffectClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckShowValueItemEffect] := chkShowValueItemEffect.Checked;
end;

procedure TFrmJSYDlg.chkAutoContinueAttackClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckAutoContinueAttack] := chkAutoContinueAttack.Checked;
end;

procedure TFrmJSYDlg.chkHideActorIconsClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckHideActorIcons] := chkHideActorIcons.Checked;
end;

procedure TFrmJSYDlg.chkSimpleShowHumanWeaponClick(Sender:TObject);
begin
  PlugInObject.ConfigCheckeds[ckSimpleShowHumanWeapon] := chkSimpleShowHumanWeapon.Checked;
end;

end.

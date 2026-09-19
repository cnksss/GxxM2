unit MirsConfigDlg;

interface
uses
  Windows, Controls, SysUtils, StrUtils, Classes, Dialogs, Graphics, Types, Grids, DxImageForm, DxImageButton, DxPageControl,
  DxEdit, DxLabel, DxMemo, DxImageGrid, DxPopupMenu, DxComboBox, DxControls, DxLine,
  DxComponents, Grobal2, GameConfigDlgs,
  GameConfigDlg, SDK;
type
  TMirsConfigDlg = class(TGameConfigObject)
    PlugConfigDlg: TDxImageForm;
    PlugConfigDlgClose: TDxImageButton;
    PlugPageControlConfig: TDxPageControl;
    PlugTabSheetConfig1: TDxTabSheet;
    PlugMemoConfig1: TDxScrollBox;
    PlugCheckBoxShowActorName: TDxImageButton;
    PlugCheckBoxHideDescUserName: TDxImageButton;
    PlugCheckBoxDuraWarning: TDxImageButton;
    PlugCheckBoxNoShift: TDxImageButton;
    PlugCheckBoxExpFilter: TDxImageButton;
    PlugEditExpFilter: TDxEdit;
    PlugCheckBoxShowMimiMapDesc: TDxImageButton;
    PlugCheckBoxShowHighlightHPLabel: TDxImageButton;
    PlugCheckBoxHideGhost: TDxImageButton;
    PlugCheckBoxShowHealthNumber: TDxImageButton;
    PlugCheckBoxHideHumEffect: TDxImageButton;
    PlugCheckBoxHideWeaponEffect: TDxImageButton;
    PlugCheckBoxAutoOrderItem: TDxImageButton;
    PlugCheckBoxMagicLock: TDxImageButton;
    PlugCheckBoxShowHPLabel: TDxImageButton;
    PlugCheckBoxNumberLable: TDxImageButton;
    PlugCheckBoxJobAndLevel: TDxImageButton;
    PlugCheckBoxShowGreenHint: TDxImageButton;
    PlugCheckBoxDisableSelfStruck: TDxImageButton;
    PlugCheckBoxSpeedSlow: TDxImageButton;
    PlugCheckBoxBGMusic: TDxImageButton;
    PlugCheckBoxShowItemName: TDxImageButton;
    PlugCheckBoxShowFilterItem: TDxImageButton;
    PlugCheckBoxItemHint: TDxImageButton;
    PlugCheckBoxSound: TDxImageButton;
    PlugCheckBoxAutoPickUpItem: TDxImageButton;
    PlugCheckBoxRepeatBGMusic: TDxImageButton;
    PlugCheckBoxShowMonName: TDxImageButton;
    PlugCheckBoxNotParaly: TDxImageButton;
    PlugTabSheetConfig2: TDxTabSheet;
    PlugMemoConfig2Label24: TDxLabel;
    PlugMemoConfig2Label25: TDxLabel;
    PlugMemoConfig2Label26: TDxLabel;
    PlugMemoConfig2Label27: TDxLabel;
    PlugComboBoxItemStdMode: TDxComboBox;
    PlugEditSearchItem: TDxEdit;
    PlugLabelDefaultItem: TDxLabel;
    PlugCheckBoxPickUpAll: TDxImageButton;
    PlugMemoConfig2: TDxListView;
    PlugMemoConfig2Line1: TDxLine;
    PlugTabSheetConfig3: TDxTabSheet;
    PlugMemoConfig4: TDxScrollBox;
    PlugMemoConfig4Label1: TDxLabel;
    PlugCheckBoxRenewHPIsAuto: TDxImageButton;
    PlugCheckBoxRenewMPIsAuto: TDxImageButton;
    PlugEditRenewHPPercent: TDxEdit;
    PlugEditRenewHPTime: TDxEdit;
    PlugEditRenewMPPercent: TDxEdit;
    PlugEditRenewMPTime: TDxEdit;
    PlugCheckBoxUseSuperMedica: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName1: TDxImageButton;
    PlugEditSuperMedicaHP1: TDxEdit;
    PlugEditSuperMedicaHPTime1: TDxEdit;
    PlugCheckBoxUseSuperMedicaItemName2: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName3: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName4: TDxImageButton;
    PlugEditSuperMedicaHP2: TDxEdit;
    PlugEditSuperMedicaHP3: TDxEdit;
    PlugEditSuperMedicaHP4: TDxEdit;
    PlugEditSuperMedicaHPTime2: TDxEdit;
    PlugEditSuperMedicaHPTime3: TDxEdit;
    PlugEditSuperMedicaHPTime4: TDxEdit;
    PlugCheckBoxRenewSpecialHPIsAuto: TDxImageButton;
    PlugCheckBoxRenewSpecialMPIsAuto: TDxImageButton;
    PlugEditRenewSpecialHPPercent: TDxEdit;
    PlugEditRenewSpecialMPPercent: TDxEdit;
    PlugEditRenewSpecialHPTime: TDxEdit;
    PlugEditRenewSpecialMPTime: TDxEdit;
    PlugCheckBoxUseSuperMedicaItemName5: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName6: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName7: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName8: TDxImageButton;
    PlugEditSuperMedicaHP5: TDxEdit;
    PlugEditSuperMedicaHP6: TDxEdit;
    PlugEditSuperMedicaHP7: TDxEdit;
    PlugEditSuperMedicaHP8: TDxEdit;
    PlugEditSuperMedicaHPTime5: TDxEdit;
    PlugEditSuperMedicaHPTime6: TDxEdit;
    PlugEditSuperMedicaHPTime7: TDxEdit;
    PlugEditSuperMedicaHPTime8: TDxEdit;
    PlugMemoConfig4Line2: TDxLine;
    PlugCheckBoxCheckHPIsAuto: TDxImageButton;
    PlugCheckBoxCheckMPIsAuto: TDxImageButton;
    PlugEditCheckHPPercent: TDxEdit;
    PlugMemoConfig4Label3: TDxLabel;
    PlugComboBoxCheckHPValue: TDxComboBox;
    PlugEditCheckMPPercent: TDxEdit;
    PlugMemoConfig4Label4: TDxLabel;
    PlugComboBoxCheckMPValue: TDxComboBox;
    PlugMemoConfig4Line3: TDxLine;
    PlugMemoConfig4Label2: TDxLabel;
    PlugMemoConfig4Line4: TDxLine;
    PlugCheckBoxUseSuperMedicaItemName0: TDxImageButton;
    PlugEditSuperMedicaHP0: TDxEdit;
    PlugEditSuperMedicaHPTime0: TDxEdit;
    PlugEditSuperMedicaMP0: TDxEdit;
    PlugEditSuperMedicaMPTime0: TDxEdit;
    PlugEditSuperMedicaMP1: TDxEdit;
    PlugEditSuperMedicaMP2: TDxEdit;
    PlugEditSuperMedicaMP3: TDxEdit;
    PlugEditSuperMedicaMP4: TDxEdit;
    PlugEditSuperMedicaMPTime1: TDxEdit;
    PlugEditSuperMedicaMPTime2: TDxEdit;
    PlugEditSuperMedicaMPTime3: TDxEdit;
    PlugEditSuperMedicaMPTime4: TDxEdit;
    PlugEditSuperMedicaMP5: TDxEdit;
    PlugEditSuperMedicaMP6: TDxEdit;
    PlugEditSuperMedicaMP7: TDxEdit;
    PlugEditSuperMedicaMP8: TDxEdit;
    PlugEditSuperMedicaMPTime5: TDxEdit;
    PlugEditSuperMedicaMPTime6: TDxEdit;
    PlugEditSuperMedicaMPTime7: TDxEdit;
    PlugEditSuperMedicaMPTime8: TDxEdit;
    PlugMemoConfig4Line1: TDxLine;
    PlugMemoConfig4Button1: TDxImageButton;
    PlugMemoConfig4Button2: TDxImageButton;
    PlugMemoConfig4Button3: TDxImageButton;
    PlugMemoConfig4Button4: TDxImageButton;
    PlugMemoConfig4Button5: TDxImageButton;
    PlugTabSheetConfig4: TDxTabSheet;
    PlugMemoConfig5: TDxScrollBox;
    PlugMemoConfig5Label19: TDxLabel;
    PlugMemoConfig5Label20: TDxLabel;
    PlugMemoConfig5Label21: TDxLabel;
    PlugCheckBoxSmartLongHit: TDxImageButton;
    PlugCheckBoxSmartWideHit: TDxImageButton;
    PlugCheckBoxSmartFireHit: TDxImageButton;
    PlugCheckBoxSmartSwordHit: TDxImageButton;
    PlugMemoConfig5Label22: TDxLabel;
    PlugCheckBoxHumAutoShield: TDxImageButton;
    PlugCheckBoxAutoHideMode: TDxImageButton;
    PlugCheckBoxAutoMagic: TDxImageButton;
    PlugEditAutoMagicTime: TDxEdit;
    PlugMemoConfig5Label23: TDxLabel;
    PlugComboBoxAutoMagic: TDxComboBox;
    PlugCheckBoxAutoTakeOnItem: TDxImageButton;
    PlugCheckBoxHeroAutoShield: TDxImageButton;
    PlugCheckBoxAssistantHeroAutoShield: TDxImageButton;
    PlugCheckBoxHumStruckShield: TDxImageButton;
    PlugCheckBoxSmartPosLongHit: TDxImageButton;
    PlugCheckBoxSmartWalkLongHit: TDxImageButton;
    PlugCheckBoxHumManuallySnowWind: TDxImageButton;
    PlugCheckBoxHumManuallyFireBoom: TDxImageButton;
    PlugCheckBoxHumShootLightenLockTarget: TDxImageButton;
    PlugCheckBoxHumManuallyMeteorShower: TDxImageButton;
    PlugCheckBoxAutoCHangePoison: TDxImageButton;
    PlugCheckBoxSmartKTZHit: TDxImageButton;
    PlugTabSheetConfig5: TDxTabSheet;
    PlugMemoConfig6: TDxScrollBox;
    PlugCheckBoxUseKeyBoard: TDxImageButton;
    PlugMemoConfig6Label1: TDxLabel;
    PlugMemoConfig6Label2: TDxLabel;
    PlugMemoConfig6Label3: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc9: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal3: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc1: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal2: TDxLabel;
    PlugMemoConfig6LabelKeyBoard9: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc2: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal4: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal1: TDxLabel;
    PlugMemoConfig6LabelKeyBoard10: TDxLabel;
    PlugMemoConfig6LabelKeyBoard7: TDxLabel;
    PlugMemoConfig6LabelKeyBoard8: TDxLabel;
    PlugMemoConfig6LabelKeyBoard1: TDxLabel;
    PlugMemoConfig6LabelKeyBoard2: TDxLabel;
    PlugMemoConfig6LabelKeyBoard3: TDxLabel;
    PlugMemoConfig6LabelKeyBoard4: TDxLabel;
    PlugMemoConfig6LabelKeyBoard5: TDxLabel;
    PlugMemoConfig6LabelKeyBoard6: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc3: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc4: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc5: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc6: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc7: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc8: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc10: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal5: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal6: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal7: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal8: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal9: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal10: TDxLabel;
    PlugMemoConfig6Line1: TDxLine;
    PlugTabSheetConfig6: TDxTabSheet;
    PlugWhisperMemo: TDxChatMemo;
    PlugTabSheetConfig7: TDxTabSheet;
    PlugTabSheetConfig8: TDxTabSheet;
    PlugMemoConfigHelp: TDxChatMemo;

  private
    FLoadControl: Boolean;
    FLoadConfig: Boolean;
    FHandle: THandle;
    FScreenMode: Byte;
    FClientVersion: TClientVersion;
    FWindowMode: Boolean;


    FEnabled: Boolean;
    FConfigCheckeds: array[TConfigChecked] of Boolean;
    FInitializeed: Boolean;

    //FProtectList: TStringList;
    FHintItemDuraTick: LongWord;
    FClientConfig: TClientConfig;
    FProtectEnabled: Boolean;
    FProtectEnabledTick: LongWord;
    procedure MouseMoveEvent(Sender: TObject; Shift: TShiftState;
      X, Y: Integer); stdcall;
    procedure PlugConfigDlgCloseClickEx(Sender: TObject; X, Y: Integer); stdcall;
    procedure PlugPageControlConfigActivePageChange(Sender: TObject); stdcall;
    procedure PlugPageControlConfigInRealArea(Sender: TObject; X, Y: Integer; var IsRealArea: Boolean); stdcall;
    procedure CheckBoxClickEx(Sender: TObject; X, Y: Integer); stdcall;
    procedure RefUseItemConfigClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure RefUseItemConfig(nObj: Integer);
    procedure RefConfig;

    procedure DEditChange(Sender: TObject); stdcall;
    procedure DComboBoxItemStdModeSelect(Sender: TObject); stdcall;
    procedure DEditSearchItemChange(Sender: TObject); stdcall;
    procedure DLabelDefaultItemClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure ListViewItemClick(Sender: TObject; ARow, ACol: Integer; ListItem: TObject; ViewItem: Pointer); stdcall;

    procedure DEditCheckHPPercentChange(Sender: TObject); stdcall;
    procedure DEditCheckMPPercentChange(Sender: TObject); stdcall;
    procedure ComboBoxCheckHPValueChange(Sender: TObject); stdcall;
    procedure ComboBoxCheckMPValueChange(Sender: TObject); stdcall;
    procedure DCheckBoxCheckHPIsAutoClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DCheckBoxCheckMPIsAutoClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DEditRenewHPPercentChange(Sender: TObject); stdcall;
    procedure DEditRenewMPPercentChange(Sender: TObject); stdcall;
    procedure DEditRenewSpecialHPPercentChange(Sender: TObject); stdcall;
    procedure DEditRenewSpecialMPPercentChange(Sender: TObject); stdcall;
    procedure DEditRenewHPTimeChange(Sender: TObject); stdcall;

    procedure DEditRenewMPTimeChange(Sender: TObject); stdcall;
    procedure DEditRenewSpecialHPTimeChange(Sender: TObject); stdcall;
    procedure DEditRenewSpecialMPTimeChange(Sender: TObject); stdcall;
    procedure DCheckBoxRenewHPIsAutoClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DCheckBoxRenewMPIsAutoClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DCheckBoxRenewSpecialHPIsAutoClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DCheckBoxRenewSpecialMPIsAutoClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DEditSuperMedicaHPChange(Sender: TObject); stdcall;
    procedure DEditSuperMedicaHPTimeChange(Sender: TObject); stdcall;
    procedure DEditSuperMedicaMPChange(Sender: TObject); stdcall;
    procedure DEditSuperMedicaMPTimeChange(Sender: TObject); stdcall;
    procedure DCheckBoxUseSuperMedicaItemNameClick(Sender: TObject; X, Y: Integer); stdcall;



    procedure LoadHelpFile;

    procedure DuraWarning();                                                                        // 持久警告
    procedure AutoUseMagic(Sender: TObject);
    procedure AutoProtect(Sender: TObject);
    procedure AutoUseItem(Sender: TObject);
    procedure AutoEatHPItem(Sender: TObject);
    procedure AutoEatMPItem(Sender: TObject);
    procedure AutoEatSpecialHPItem(Sender: TObject);
    procedure AutoEatSpecialMPItem(Sender: TObject);
    procedure DamageHPUseItem(nObj, nDamage: Integer);
    procedure DamageMPUseItem(nObj, nDamage: Integer);

    procedure LoadConfigFile();
    procedure SaveConfigFile();
  public
    function GetType: TConfigDlgType; override;
    function GetConfigChecked(Index: TConfigChecked): Boolean; override;
    procedure SetConfigChecked(Index: TConfigChecked; Value: Boolean); override;
    function GetVisible: Boolean; override;
    procedure SetVisible(Value: Boolean); override;
    function GetEnabled: Boolean; override;
    procedure SetEnabled(Value: Boolean); override;
    function GetProtectEnabled: Boolean; override;
    procedure SetProtectEnabled(Value: Boolean); override;
    procedure Open; override;
    procedure Close; override;
    procedure ClearShowItem; override;
    procedure RefShowItem; override;
    procedure LoadConfig(const CharName: string); override;
    procedure Initialize(Handle: THandle; ScreenMode: Byte; ClientVersion: TClientVersion; WindowMode: Boolean); override;
    procedure Finalize; override;
    procedure Logon(const ServerName: string); override;
    procedure Logout; override;
    function FormKeyDown(var Key: Word; Shift: TShiftState): Boolean; override;
    function FormKeyPress(var Key: Char): Boolean; override;
    procedure RefreshMySelfAbil; override;
    procedure RefreshMyHeroAbil; override;
    procedure RefreshMySelfMagicList; override;
    procedure RefreshMyHeroMagicList; override;
    procedure RefreshUnBindItemList; override;
    procedure Struck(Actor: TObject; HP, MaxHP: LongInt); override;
    procedure HealthChange(Actor: TObject; HP, MP, MaxHP: LongInt); override;
    procedure LoadClientConfig(ClientConfig: pTClientConfig); override;
    procedure Run; override;
    function CanFilterExp(Exp: LongWord): Boolean; override;
    function GetShowItem(const ItemName: string): pTShowItem; override;
    function FindShowItem(const ItemName: string): Boolean; override;
    function FindHintItem(const ItemName: string): Boolean; override;
    function FindPickItem(const ItemName: string): Boolean; override;
    procedure HintItem(const ItemName: string; X, Y: Integer); override;

    constructor Create();
    destructor Destroy; override;
  end;
implementation
uses
  MShare, ConfigShare, IniFiles, FState, ClMain, Math, FilterItems;
// {$R ..\..\DxComponent\MirsConfigDlg.res}
type
  TConfig = record
    nFilterMinExp: Integer;
    nAutoUseMagicTime: Integer;
    dwAutoUseMagicTick: LongWord;

    boRenewSpecialIsAuto: Boolean;
    nRenewSpecialPercent: Integer;
    nRenewSpecialTime: Integer;

    boRenewBookIsAuto: Boolean;
    nRenewBookPercent: Integer;
    nRenewBookTime: Integer;
    nRenewBookNowBookIndex: Integer;
    sRenewBookNowBookItem: string;
// ============药品==================
    nRenewHeroHPTime: Integer;
    nRenewHeroHPPercent: Integer;

    nRenewHeroMPTime: Integer;
    nRenewHeroMPPercent: Integer;

    boRenewHeroSpecialIsAuto: Boolean;
    nRenewHeroSpecialTime: Integer;
    nRenewHeroSpecialPercent: Integer;

    boRenewHeroLogOutIsAuto: Boolean;
    nRenewHeroLogOutTime: Integer;
    nRenewHeroLogOutPercent: Integer;

    boRenewCloseIsAuto: Boolean;
    nRenewCloseTime: Integer;
    nRenewClosePercent: Integer;

    MedicaMode: Integer;

    CheckHpIsAutos: array[0..4] of Boolean;
    CheckHpPercents: array[0..4] of Integer;
    CheckHpValues: array[0..4] of Integer;
    CheckHpCheckTimes: array[0..4] of LongWord;
    CheckHpCheckTicks: array[0..4] of LongWord;
    CheckHpUseTimes: array[0..4] of LongWord;
    CheckHpUseTicks: array[0..4] of LongWord;


    CheckMpIsAutos: array[0..4] of Boolean;
    CheckMpPercents: array[0..4] of Integer;
    CheckMpValues: array[0..4] of Integer;
    CheckMpCheckTimes: array[0..4] of LongWord;
    CheckMpCheckTicks: array[0..4] of LongWord;
    CheckMpUseTimes: array[0..4] of LongWord;
    CheckMpUseTicks: array[0..4] of LongWord;

    RenewHPIsAutos: array[0..4] of Boolean;
    RenewHPPercents: array[0..4] of Integer;
    RenewHPTimes: array[0..4] of Integer;
    RenewHPTicks: array[0..4] of LongWord;

    RenewMPIsAutos: array[0..4] of Boolean;
    RenewMPPercents: array[0..4] of Integer;
    RenewMPTimes: array[0..4] of Integer;
    RenewMPTicks: array[0..4] of LongWord;

    RenewSpecialHPIsAutos: array[0..4] of Boolean;
    RenewSpecialHPPercents: array[0..4] of Integer;
    RenewSpecialHPTimes: array[0..4] of Integer;
    RenewSpecialHPTicks: array[0..4] of LongWord;

    RenewSpecialMPIsAutos: array[0..4] of Boolean;
    RenewSpecialMPPercents: array[0..4] of Integer;
    RenewSpecialMPTimes: array[0..4] of Integer;
    RenewSpecialMPTicks: array[0..4] of LongWord;

    UseSuperMedicas: array[0..4] of Boolean;
    SuperMedicaItemNames: array[0..8] of string;
    SuperMedicaUses: array[0..4] of array[0..8] of Boolean;
    SuperMedicaHPs: array[0..4] of array[0..8] of Integer;
    SuperMedicaHPTimes: array[0..4] of array[0..8] of Integer;
    SuperMedicaHPTicks: array[0..4] of array[0..8] of Integer;

    SuperMedicaMPs: array[0..4] of array[0..8] of Integer;
    SuperMedicaMPTimes: array[0..4] of array[0..8] of Integer;
    SuperMedicaMPTicks: array[0..4] of array[0..8] of Integer;

    {HumanWineIsAuto: Boolean;
    HumanWinePercent: Integer;
    HeroWineIsAuto: Boolean;
    HeroWinePercent: Integer;
    HumanMedicateWineIsAuto: Boolean;
    HumanMedicateWineTime: Integer;
    HeroMedicateWineIsAuto: Boolean;
    HeroMedicateWineTime: Integer;}
  end;
  pTConfig = ^TConfig;
var
  g_Config: TConfig = (
    nFilterMinExp: 0;
    nAutoUseMagicTime: 0;
    dwAutoUseMagicTick: 0;
  // ============保护=================
    boRenewSpecialIsAuto: False;
    nRenewSpecialPercent: 0;
    nRenewSpecialTime: 0;

    boRenewBookIsAuto: False;
    nRenewBookPercent: 0;
    nRenewBookTime: 0;
    nRenewBookNowBookIndex: 0;
    sRenewBookNowBookItem: '';
// ============药品==================

    MedicaMode: 0;                                                                                  // 0主体 1英雄 2战士副将 3法师副将 4道士副将

    CheckHpIsAutos: (False, False, False, False, False);
    CheckHpPercents: (0, 0, 0, 0, 0);
    CheckHpValues: (0, 0, 0, 0, 0);
    CheckHpCheckTimes: (1000, 1000, 1000, 1000, 1000);
    CheckHpCheckTicks: (0, 0, 0, 0, 0);
    CheckHpUseTimes: (10000, 10000, 10000, 10000, 10000);
    CheckHpUseTicks: (0, 0, 0, 0, 0);


    CheckMpIsAutos: (False, False, False, False, False);
    CheckMpPercents: (0, 0, 0, 0, 0);
    CheckMpValues: (0, 0, 0, 0, 0);
    CheckMpCheckTimes: (1000, 1000, 1000, 1000, 1000);
    CheckMpCheckTicks: (0, 0, 0, 0, 0);
    CheckMpUseTimes: (10000, 10000, 10000, 10000, 10000);
    CheckMpUseTicks: (0, 0, 0, 0, 0);

    RenewHPIsAutos: (False, False, False, False, False);
    RenewHPPercents: (10, 10, 10, 10, 10);
    RenewHPTimes: (1000, 1000, 1000, 1000, 1000);
    RenewHPTicks: (0, 0, 0, 0, 0);

    RenewMPIsAutos: (False, False, False, False, False);
    RenewMPPercents: (10, 10, 10, 10, 10);
    RenewMPTimes: (0, 0, 0, 0, 0);
    RenewMPTicks: (0, 0, 0, 0, 0);

    RenewSpecialHPIsAutos: (False, False, False, False, False);
    RenewSpecialHPPercents: (10, 10, 10, 10, 10);
    RenewSpecialHPTimes: (1000, 1000, 1000, 1000, 1000);
    RenewSpecialHPTicks: (0, 0, 0, 0, 0);

    RenewSpecialMPIsAutos: (False, False, False, False, False);
    RenewSpecialMPPercents: (10, 10, 10, 10, 10);
    RenewSpecialMPTimes: (1000, 1000, 1000, 1000, 1000);
    RenewSpecialMPTicks: (0, 0, 0, 0, 0);

    UseSuperMedicas: (False, False, False, False, False);

    SuperMedicaItemNames:
    ('太阳水',
    '强效太阳水',
    '万年雪霜',
    '疗伤药',
    '疗伤药(任务)',
    '强效万年雪霜',
    '强效疗伤药',
    '超级万年雪霜',
    '超级疗伤药');

    SuperMedicaUses: (
    (False, False, False, False, False, False, False, False, False),
    (False, False, False, False, False, False, False, False, False),
    (False, False, False, False, False, False, False, False, False),
    (False, False, False, False, False, False, False, False, False),
    (False, False, False, False, False, False, False, False, False)
    );

    SuperMedicaHPs: (
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0)
    );

    SuperMedicaHPTimes: (
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500)
    );

    SuperMedicaHPTicks: (
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0)
    );

    SuperMedicaMPs: (
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0)
    );

    SuperMedicaMPTimes: (
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500)
    );

    SuperMedicaMPTicks: (
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0));
    );

constructor TMirsConfigDlg.Create();
begin
  FEnabled := False;
  FProtectEnabled := True;
  FProtectEnabledTick := MyGetTickCount;
  FHandle := 0;

  FScreenMode := 0;
  FClientVersion := cvMirs;
  FWindowMode := True;
  FLoadControl := False;
  FLoadConfig := False;
  FInitializeed := False;
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
  FConfigCheckeds[ckShowUserName] := True;
  FConfigCheckeds[ckMagicLock] := True;
  FConfigCheckeds[ckAutoOrderItem] := True;
  FConfigCheckeds[ckNotNeedShift] := True;
  FConfigCheckeds[ckAutoPickUpItem] := True;
  FConfigCheckeds[ckBGMusic] := True;
  FConfigCheckeds[ckRepeatBGMusic] := True;
  FConfigCheckeds[ckNotParaly] := False;

  FConfigCheckeds[ckSmartLongHit] := False;                                                         // 刀刀刺杀
  FConfigCheckeds[ckSmartPosLongHit] := False;                                                      // 隔位刺杀
  FConfigCheckeds[ckSmartWalkLongHit] := False;                                                     // 走位刺杀
  FConfigCheckeds[ckSmartWideHit] := False;                                                         // 智能半月
  FConfigCheckeds[ckSmartFireHit] := False;                                                         // 自动烈火
  FConfigCheckeds[ckSmartSwordHit] := False;                                                        // 逐日剑法
  FConfigCheckeds[ckSmartCrsHit] := False;                                                          // 抱月刀 双龙斩
  FConfigCheckeds[ckSmartTwnHit] := False;                                                          // 龙影剑法

  FConfigCheckeds[ckHumAutoShield] := False;                                                        // 自动开盾
  FConfigCheckeds[ckHumStruckShield] := False;                                                      // 被攻击开盾
  FConfigCheckeds[ckHumShootLightenLockTarget] := True;                                             // 疾光电影锁定目标
  FConfigCheckeds[ckHumManuallyFireBoom] := False;                                                  // 手动控制爆裂火焰
  FConfigCheckeds[ckHumManuallySnowWind] := False;                                                  // 手动控制冰咆哮
  FConfigCheckeds[ckHumManuallyMeteorShower] := False;                                              // 手动控制流星火雨

  FConfigCheckeds[ckAutoTakeOnItem] := False;                                                       // 毒符互换

  FConfigCheckeds[ckMovePick] := False;
end;

destructor TMirsConfigDlg.Destroy;
begin
  if FEnabled then
    SaveConfigFile;
  //FProtectList.Free;
end;


function TMirsConfigDlg.GetType: TConfigDlgType;
begin
  Result := ptDefault;
end;

function TMirsConfigDlg.GetConfigChecked(Index: TConfigChecked): Boolean;
begin
  Result := FConfigCheckeds[Index];
end;

procedure TMirsConfigDlg.SetConfigChecked(Index: TConfigChecked; Value: Boolean);
begin
  if FConfigCheckeds[Index] <> Value then
  begin
    FConfigCheckeds[Index] := Value;
    RefConfig;
  end;
end;

procedure TMirsConfigDlg.Open;
begin

end;

procedure TMirsConfigDlg.Close;
begin
  FEnabled := False;
  if PlugConfigDlg <> nil then
    PlugConfigDlg.Visible := False;
  FileItemDB.BackUp;
end;

function TMirsConfigDlg.GetVisible: Boolean;
begin
  if PlugConfigDlg <> nil then
    Result := PlugConfigDlg.Visible;
end;

procedure TMirsConfigDlg.SetVisible(Value: Boolean);
begin
  if PlugConfigDlg <> nil then
    PlugConfigDlg.Visible := Value;
end;

function TMirsConfigDlg.GetEnabled: Boolean;
begin
  Result := FEnabled;
end;

procedure TMirsConfigDlg.SetEnabled(Value: Boolean);
begin
  FEnabled := Value;
  if not FEnabled then
    FLoadConfig := False;
end;

function TMirsConfigDlg.GetProtectEnabled: Boolean;
begin
  Result := FProtectEnabled;
end;

procedure TMirsConfigDlg.SetProtectEnabled(Value: Boolean);
begin
  FProtectEnabled := Value;
  if FProtectEnabled then
    FProtectEnabledTick := MyGetTickCount;
end;

function TMirsConfigDlg.FormKeyDown(var Key: Word; Shift: TShiftState): Boolean;
begin

end;

function TMirsConfigDlg.FormKeyPress(var Key: Char): Boolean;
begin

end;

procedure TMirsConfigDlg.RefreshMySelfAbil;
begin

end;

procedure TMirsConfigDlg.RefreshMyHeroAbil;
begin

end;

procedure TMirsConfigDlg.RefreshMySelfMagicList;
var
  I, nItemIndex: Integer;
begin
  if (g_MySelf <> nil) then
  begin
    nItemIndex := PlugComboBoxAutoMagic.ItemIndex;
    PlugComboBoxAutoMagic.Items.Clear;
    for I := 0 to g_MagicList.Count - 1 do
    begin
      PlugComboBoxAutoMagic.Items.AddObject(pTClientMagic(g_MagicList.Items[I]).Def.sMagicName,
        TObject(pTClientMagic(g_MagicList.Items[I])));
    end;
    if (nItemIndex >= 0) and (nItemIndex < PlugComboBoxAutoMagic.Items.Count) then
      PlugComboBoxAutoMagic.ItemIndex := nItemIndex
    else
      PlugComboBoxAutoMagic.ItemIndex := -1;
  end;
end;

procedure TMirsConfigDlg.RefreshMyHeroMagicList;
begin

end;

procedure TMirsConfigDlg.RefreshUnBindItemList;
begin

end;

procedure TMirsConfigDlg.PlugPageControlConfigInRealArea(Sender: TObject; X, Y: Integer; var IsRealArea: Boolean);
begin
  IsRealArea := not ((X >= PlugPageControlConfig.Width - 12) and (Y <= PlugPageControlConfig.Height + 20));
end;

procedure TMirsConfigDlg.PlugConfigDlgCloseClickEx(Sender: TObject; X, Y: Integer);
begin
  if PlugConfigDlg <> nil then
    PlugConfigDlg.Visible := False;
end;

procedure TMirsConfigDlg.PlugPageControlConfigActivePageChange(Sender: TObject);
begin
  // if (PlugConfigDlg <> nil) and (FInitializeed) then

end;

procedure TMirsConfigDlg.Logout;
begin
  FLoadConfig := False;
  FEnabled := False;
  SaveConfigFile;
end;

procedure TMirsConfigDlg.LoadHelpFile;
var
  I: Integer;
  ViewItem: pTViewItem;
begin
// ---------------------------------读取帮助文件---------------------------------
  if (PlugMemoConfigHelp <> nil) and FileExists('Data\explain2.dat') then
  begin
    try
      PlugMemoConfigHelp.LoadFromFile('Data\explain2.dat');
    except
    end;

    PlugMemoConfigHelp.FontBackTransparent := True;

    for I := 0 to PlugMemoConfigHelp.Lines.Count - 1 do
    begin
      ViewItem := TDxLines(PlugMemoConfigHelp.Lines).Items[I];
      if (Length(ViewItem.Caption) <> Length(Trim(ViewItem.Caption))) then
      begin
        ViewItem.Color.Up.Color := clSilver;
        ViewItem.Color.Up.BColor := clBlack;
        ViewItem.Color.Up.Bold := False;
        ViewItem.Color.Hot.Color := clSilver;
        ViewItem.Color.Hot.BColor := clBlack;
        ViewItem.Color.Hot.Bold := False;
        ViewItem.Color.Down.Color := clSilver;
        ViewItem.Color.Down.BColor := clBlack;
        ViewItem.Color.Down.Bold := False;
      end
      else
      begin
        ViewItem.Color.Up.Color := clWhite;
        ViewItem.Color.Up.BColor := clBlack;
        ViewItem.Color.Up.Bold := False;
        ViewItem.Color.Hot.Color := clWhite;
        ViewItem.Color.Hot.BColor := clBlack;
        ViewItem.Color.Hot.Bold := False;
        ViewItem.Color.Down.Color := clWhite;
        ViewItem.Color.Down.BColor := clBlack;
        ViewItem.Color.Down.Bold := False;
      end;
    end;
  end;
end;

procedure TMirsConfigDlg.ClearShowItem;
begin
  PlugMemoConfig2.Clear;
  PlugMemoConfig2.ColCount := 4;
end;

procedure TMirsConfigDlg.RefShowItem;
var
  I: Integer;
  ShowItem: pTShowItem;
  ListItem: TDxListItem;
  ViewItem: pTViewItem;
begin
  PlugMemoConfig2.Clear;
  PlugMemoConfig2.ColCount := 4;
  PlugMemoConfig2.Lock;
  try
    for I := 0 to FileItemDB.m_ShowItemList.Count - 1 do
    begin
      ShowItem := pTShowItem(FileItemDB.m_ShowItemList.Items[I]);

      ListItem := PlugMemoConfig2.Add;
      ViewItem := ListItem.AddItem('', nil);

      ViewItem.Caption := ShowItem.sItemName;
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsButton;                                                                   // bsRadio;
      ViewItem.Alignment := taLeftJustify;
      ViewItem.Color.Up.Color := clWhite;
      ViewItem.Color.Hot.Color := clRed;                                                            // clWhite;
      ViewItem.Color.Down.Color := clRed;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boHintMsg;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boPickup;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boShowName;
    end;
  finally
    PlugMemoConfig2.UnLock;
  end;
end;

procedure TMirsConfigDlg.ListViewItemClick(Sender: TObject; ARow, ACol: Integer; ListItem: TObject; ViewItem: Pointer);
var
  ShowItem: pTShowItem;
begin
  ShowItem := pTViewItem(ViewItem).Data;
  if ShowItem <> nil then
  begin
    case ACol of
      0: ;
      1: ShowItem.boHintMsg := pTViewItem(ViewItem).Checked;
      2: ShowItem.boPickup := pTViewItem(ViewItem).Checked;
      3: ShowItem.boShowName := pTViewItem(ViewItem).Checked;
    end;
    FileItemDB.SaveToFile;
  end;
end;

procedure TMirsConfigDlg.DLabelDefaultItemClick(Sender: TObject; X, Y: Integer);
var
  I: Integer;
  List: TList;

  ListItem: TDxListItem;
  ViewItem: pTViewItem;
  ShowItem: pTShowItem;
begin
  if mrOk = FrmDlg.DMessageDlg('你想恢复成系统默认设置吗 ?', [mbOk, mbCancel]) then
  begin
    FileItemDB.BackUp;

    List := TList.Create;
    FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List);
    PlugMemoConfig2.Clear;
    PlugMemoConfig2.ColCount := 4;
    PlugMemoConfig2.Lock;
    try
      for I := 0 to List.Count - 1 do
      begin
        ShowItem := List.Items[I];

        ListItem := PlugMemoConfig2.Add;
        ViewItem := ListItem.AddItem('', nil);

        ViewItem.Caption := ShowItem.sItemName;
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsButton;                                                                 // bsRadio;
        ViewItem.Alignment := taLeftJustify;
        ViewItem.Color.Up.Color := clWhite;
        ViewItem.Color.Hot.Color := clRed;                                                          // clWhite;
        ViewItem.Color.Down.Color := clRed;

        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        ViewItem.ImageIndex.ImageType := NewopUI_Pak;
        ViewItem.ImageIndex.Up := 228;
        ViewItem.ImageIndex.Down := 229;
        ViewItem.Checked := ShowItem.boHintMsg;

        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        ViewItem.ImageIndex.ImageType := NewopUI_Pak;
        ViewItem.ImageIndex.Up := 228;
        ViewItem.ImageIndex.Down := 229;
        ViewItem.Checked := ShowItem.boPickup;

        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        ViewItem.ImageIndex.ImageType := NewopUI_Pak;
        ViewItem.ImageIndex.Up := 228;
        ViewItem.ImageIndex.Down := 229;
        ViewItem.Checked := ShowItem.boShowName;
      end;
    finally
      PlugMemoConfig2.UnLock;
    end;
    List.Free;

    FileItemDB.SaveToFile;
    PlugMemoConfig2.First;
  end;
end;

procedure TMirsConfigDlg.DEditSearchItemChange(Sender: TObject);
var
  I: Integer;
  List: TList;

  ShowItem: pTShowItem;
  ListItem: TDxListItem;
  ViewItem: pTViewItem;
  sText: string;
begin
  if PlugEditSearchItem.Text = '' then
  begin
    DComboBoxItemStdModeSelect(Sender);
  end
  else
  begin
    sText := PlugEditSearchItem.Text;
    List := TList.Create;
    FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List);
    PlugMemoConfig2.Clear;
    PlugMemoConfig2.ColCount := 4;
    PlugMemoConfig2.Lock;
    try
      for I := 0 to List.Count - 1 do
      begin
        ShowItem := List.Items[I];
        if AnsiContainsText(sText, ShowItem.sItemName) or AnsiContainsText(ShowItem.sItemName, sText) then
        begin

          ListItem := PlugMemoConfig2.Add;
          ViewItem := ListItem.AddItem('', nil);

          ViewItem.Caption := ShowItem.sItemName;
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsButton;                                                               // bsRadio;
          ViewItem.Alignment := taLeftJustify;
          ViewItem.Color.Up.Color := clWhite;
          ViewItem.Color.Hot.Color := clRed;                                                        // clWhite;
          ViewItem.Color.Down.Color := clRed;

          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
          ViewItem.Checked := ShowItem.boHintMsg;

          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
          ViewItem.Checked := ShowItem.boPickup;

          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
          ViewItem.Checked := ShowItem.boShowName;
        end;
      end;
    finally
      PlugMemoConfig2.UnLock;
    end;
    List.Free;
  end;
end;

procedure TMirsConfigDlg.DComboBoxItemStdModeSelect(Sender: TObject);
var
  I: Integer;
  List: TList;

  ShowItem: pTShowItem;
  ListItem: TDxListItem;
  ViewItem: pTViewItem;
begin
  List := TList.Create;
  FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List);
  PlugMemoConfig2.Clear;
  PlugMemoConfig2.ColCount := 4;
  PlugMemoConfig2.Lock;
  try
    for I := 0 to List.Count - 1 do
    begin
      ShowItem := List.Items[I];
      ListItem := PlugMemoConfig2.Add;
      ViewItem := ListItem.AddItem('', nil);

      ViewItem.Caption := ShowItem.sItemName;
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsButton;                                                                   // bsRadio;
      ViewItem.Alignment := taLeftJustify;
      ViewItem.Color.Up.Color := clWhite;
      ViewItem.Color.Hot.Color := clRed;                                                            // clWhite;
      ViewItem.Color.Down.Color := clRed;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boHintMsg;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boPickup;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boShowName;
    end;
  finally
    PlugMemoConfig2.UnLock;
  end;
  List.Free;
end;

procedure TMirsConfigDlg.CheckBoxClickEx(Sender: TObject; X, Y: Integer);
begin
  if FClientConfig.boNotCanUseClientConfig then Exit;
  if PlugCheckBoxShowHPLabel = Sender then
  begin
    FConfigCheckeds[ckShowHPLabel] := PlugCheckBoxShowHPLabel.Checked;
  end
  else if PlugCheckBoxNumberLable = Sender then
  begin
    FConfigCheckeds[ckShowNumberLable] := PlugCheckBoxNumberLable.Checked;
  end
  else if PlugCheckBoxJobAndLevel = Sender then
  begin
    FConfigCheckeds[ckShowJobAndLevel] := PlugCheckBoxJobAndLevel.Checked;
  end
  else if PlugCheckBoxShowGreenHint = Sender then
  begin
    FConfigCheckeds[ckShowGreenHint] := PlugCheckBoxShowGreenHint.Checked;
  end
  else if PlugCheckBoxShowItemName = Sender then
  begin
    FConfigCheckeds[ckShowItemName] := PlugCheckBoxShowItemName.Checked;
  end
  else if PlugCheckBoxShowFilterItem = Sender then
  begin
    FConfigCheckeds[ckShowFilterItem] := PlugCheckBoxShowFilterItem.Checked;
  end
  else if PlugCheckBoxItemHint = Sender then
  begin
    FConfigCheckeds[ckItemHint] := PlugCheckBoxItemHint.Checked;
  end
  else if PlugCheckBoxShowActorName = Sender then
  begin
    FConfigCheckeds[ckShowUserName] := PlugCheckBoxShowActorName.Checked;
  end
  else if PlugCheckBoxHideDescUserName = Sender then
  begin
    FConfigCheckeds[ckOnlyShowCharName] := PlugCheckBoxHideDescUserName.Checked;
  end
  else if PlugCheckBoxDuraWarning = Sender then
  begin
    FConfigCheckeds[ckDuraWarning] := PlugCheckBoxDuraWarning.Checked;
  end
  else if PlugCheckBoxNoShift = Sender then
  begin
    FConfigCheckeds[ckNotNeedShift] := PlugCheckBoxNoShift.Checked;
  end
  else if PlugCheckBoxExpFilter = Sender then
  begin
    FConfigCheckeds[ckFilterExp] := PlugCheckBoxExpFilter.Checked;
  end
  else if PlugCheckBoxShowMimiMapDesc = Sender then
  begin
    FConfigCheckeds[ckShowMapDesc] := PlugCheckBoxShowMimiMapDesc.Checked;
  end
  else if PlugCheckBoxNotParaly = Sender then
  begin
    FConfigCheckeds[ckNotParaly] := PlugCheckBoxNotParaly.Checked;
  end
  else if PlugCheckBoxShowHighlightHPLabel = Sender then
  begin
    FConfigCheckeds[ckShowHighlightHPLabel] := PlugCheckBoxShowHighlightHPLabel.Checked;
  end
  else if PlugCheckBoxShowHealthNumber = Sender then
  begin
    FConfigCheckeds[ckShowMoveLable] := PlugCheckBoxShowHealthNumber.Checked;
  end
  else if PlugCheckBoxHideGhost = Sender then
  begin
    FConfigCheckeds[ckHideGhost] := PlugCheckBoxHideGhost.Checked;
  end
  else if PlugCheckBoxHideHumEffect = Sender then
  begin
    FConfigCheckeds[ckHideHumEffect] := PlugCheckBoxHideHumEffect.Checked;
  end
  else if PlugCheckBoxHideWeaponEffect = Sender then
  begin
    FConfigCheckeds[ckHideWeaponEffect] := PlugCheckBoxHideWeaponEffect.Checked;
  end
  else if PlugCheckBoxShowMonName = Sender then
  begin
    FConfigCheckeds[ckShowMonName] := PlugCheckBoxShowMonName.Checked;
  end
  else if PlugCheckBoxAutoOrderItem = Sender then
  begin
    FConfigCheckeds[ckAutoOrderItem] := PlugCheckBoxAutoOrderItem.Checked;
  end
  else if PlugCheckBoxMagicLock = Sender then
  begin
    FConfigCheckeds[ckMagicLock] := PlugCheckBoxMagicLock.Checked;
  end
  else if PlugCheckBoxSmartLongHit = Sender then
  begin
    FConfigCheckeds[ckSmartLongHit] := PlugCheckBoxSmartLongHit.Checked;
  end
  else if PlugCheckBoxSmartPosLongHit = Sender then
  begin
    FConfigCheckeds[ckSmartPosLongHit] := PlugCheckBoxSmartPosLongHit.Checked;
  end
  else if PlugCheckBoxSmartWalkLongHit = Sender then
  begin
    FConfigCheckeds[ckSmartWalkLongHit] := PlugCheckBoxSmartWalkLongHit.Checked;
  end
  else if PlugCheckBoxSmartWideHit = Sender then
  begin
    FConfigCheckeds[ckSmartWideHit] := PlugCheckBoxSmartWideHit.Checked;
  end
  else if PlugCheckBoxSmartFireHit = Sender then
  begin
    FConfigCheckeds[ckSmartFireHit] := PlugCheckBoxSmartFireHit.Checked;
  end
  else if PlugCheckBoxSmartSwordHit = Sender then
  begin
    FConfigCheckeds[ckSmartSwordHit] := PlugCheckBoxSmartSwordHit.Checked;
  end
  else if PlugCheckBoxSmartKTZHit = Sender then
  begin
    FConfigCheckeds[ckSmart66Hit] := PlugCheckBoxSmartKTZHit.Checked;
  end
  else if PlugCheckBoxAutoHideMode = Sender then
  begin
    FConfigCheckeds[ckAutoHideMode] := PlugCheckBoxAutoHideMode.Checked;
  end
  else if PlugCheckBoxAutoTakeOnItem = Sender then
  begin
    FConfigCheckeds[ckAutoTakeOnItem] := PlugCheckBoxAutoTakeOnItem.Checked;
  end
  else if PlugCheckBoxAutoCHangePoison = Sender then
  begin
    FConfigCheckeds[ckAutoChangePoison] := PlugCheckBoxAutoChangePoison.Checked;
  end
  else if PlugCheckBoxHumAutoShield = Sender then
  begin
    FConfigCheckeds[ckHumAutoShield] := PlugCheckBoxHumAutoShield.Checked;
  end
  else if PlugCheckBoxHumStruckShield = Sender then
  begin
    FConfigCheckeds[ckHumStruckShield] := PlugCheckBoxHumStruckShield.Checked;
  end
  else if PlugCheckBoxHeroAutoShield = Sender then
  begin
    FConfigCheckeds[ckHeroAutoShield] := PlugCheckBoxHeroAutoShield.Checked;
    frmMain.SendPlugInConfig(0);
  end
  else if PlugCheckBoxAssistantHeroAutoShield = Sender then
  begin
    FConfigCheckeds[ckAssistantHeroAutoShield] := PlugCheckBoxAssistantHeroAutoShield.Checked;
    frmMain.SendPlugInConfig(0);
  end
  else if PlugCheckBoxHumManuallySnowWind = Sender then
  begin
    FConfigCheckeds[ckHumManuallySnowWind] := PlugCheckBoxHumManuallySnowWind.Checked;
  end
  else if PlugCheckBoxHumManuallyFireBoom = Sender then
  begin
    FConfigCheckeds[ckHumManuallyFireBoom] := PlugCheckBoxHumManuallyFireBoom.Checked;
  end
  else if PlugCheckBoxHumShootLightenLockTarget = Sender then
  begin
    FConfigCheckeds[ckHumShootLightenLockTarget] := PlugCheckBoxHumShootLightenLockTarget.Checked;
  end
  else if PlugCheckBoxHumManuallyMeteorShower = Sender then
  begin
    FConfigCheckeds[ckHumManuallyMeteorShower] := PlugCheckBoxHumManuallyMeteorShower.Checked;
  end
  else if PlugCheckBoxAutoMagic = Sender then
  begin
    FConfigCheckeds[ckAutoUseMagic] := PlugCheckBoxAutoMagic.Checked;
  end
  else if PlugCheckBoxUseKeyBoard = Sender then
  begin
    FConfigCheckeds[ckUseKeyBoard] := PlugCheckBoxUseKeyBoard.Checked;
  end
  else if PlugCheckBoxUseSuperMedica = Sender then
  begin
    FConfigCheckeds[ckUseSuperMedica] := PlugCheckBoxUseSuperMedica.Checked;
    g_Config.UseSuperMedicas[g_Config.MedicaMode] := PlugCheckBoxUseSuperMedica.Checked;
  end
  else if PlugCheckBoxDisableSelfStruck = Sender then
  begin
    FConfigCheckeds[ckDisableSelfStruck] := PlugCheckBoxDisableSelfStruck.Checked;
  end
  else if PlugCheckBoxSpeedSlow = Sender then
  begin
    FConfigCheckeds[ckSpeedSlow] := PlugCheckBoxSpeedSlow.Checked;
  end
  else if PlugCheckBoxAutoPickUpItem = Sender then
  begin
    FConfigCheckeds[ckAutoPickUpItem] := PlugCheckBoxAutoPickUpItem.Checked;
  end
  else if PlugCheckBoxSound = Sender then
  begin
    g_boSound := PlugCheckBoxSound.Checked;
    if g_boSound then
    begin
      DScreen.AddChatBoardString('[音效 开]', clWhite, clBlack);                                 // clBlack
    end
    else
    begin
      DScreen.AddChatBoardString('[音效 关]', clWhite, clBlack)
    end;
  end
  else if PlugCheckBoxBGMusic = Sender then
  begin
    FConfigCheckeds[ckBGMusic] := PlugCheckBoxBGMusic.Checked;
    g_boBGSound := FConfigCheckeds[ckBGMusic];
  end
  else if PlugCheckBoxRepeatBGMusic = Sender then
  begin
    FConfigCheckeds[ckRepeatBGMusic] := PlugCheckBoxRepeatBGMusic.Checked;
    g_boRepeatBGSound := FConfigCheckeds[ckRepeatBGMusic];
    SetRepeatBGSound(g_boRepeatBGSound);
  end;
end;

procedure TMirsConfigDlg.RefUseItemConfigClick(Sender: TObject; X, Y: Integer);
begin
  if PlugMemoConfig4Button1 = Sender then
    g_Config.MedicaMode := 0
  else if PlugMemoConfig4Button2 = Sender then
    g_Config.MedicaMode := 1
  else if PlugMemoConfig4Button3 = Sender then
    g_Config.MedicaMode := 2
  else if PlugMemoConfig4Button4 = Sender then
    g_Config.MedicaMode := 3
  else if PlugMemoConfig4Button5 = Sender then
    g_Config.MedicaMode := 4
  else
    g_Config.MedicaMode := 0;

  PlugMemoConfig4Button1.Checked := g_Config.MedicaMode = 0;
  PlugMemoConfig4Button2.Checked := g_Config.MedicaMode = 1;
  PlugMemoConfig4Button3.Checked := g_Config.MedicaMode = 2;
  PlugMemoConfig4Button4.Checked := g_Config.MedicaMode = 3;
  PlugMemoConfig4Button5.Checked := g_Config.MedicaMode = 4;
  RefUseItemConfig(g_Config.MedicaMode);
end;

procedure TMirsConfigDlg.RefUseItemConfig(nObj: Integer);
begin
  if not FInitializeed then Exit;
  if nObj in [0..4] then
  begin
    if nObj = 0 then
    begin
      PlugMemoConfig4Label3.Caption := '时使用';
      PlugComboBoxCheckHPValue.Visible := True;
      PlugMemoConfig4Label4.Caption := '时使用';
      PlugComboBoxCheckMPValue.Visible := True;
    end
    else
    begin
      PlugMemoConfig4Label3.Caption := '收英雄';
      PlugComboBoxCheckHPValue.Visible := False;
      PlugMemoConfig4Label4.Caption := '收英雄';
      PlugComboBoxCheckMPValue.Visible := False;
    end;

    PlugCheckBoxCheckHPIsAuto.Checked := g_Config.CheckHpIsAutos[g_Config.MedicaMode];
    PlugEditCheckHPPercent.Value := g_Config.CheckHpPercents[g_Config.MedicaMode];
    PlugComboBoxCheckHPValue.ItemIndex := g_Config.CheckHpValues[g_Config.MedicaMode];

    PlugCheckBoxCheckMPIsAuto.Checked := g_Config.CheckMpIsAutos[g_Config.MedicaMode];
    PlugEditCheckMPPercent.Value := g_Config.CheckMpPercents[g_Config.MedicaMode];
    PlugComboBoxCheckMPValue.ItemIndex := g_Config.CheckMpValues[g_Config.MedicaMode];

    PlugCheckBoxRenewHPIsAuto.Checked := g_Config.RenewHPIsAutos[g_Config.MedicaMode];
    PlugCheckBoxRenewMPIsAuto.Checked := g_Config.RenewMPIsAutos[g_Config.MedicaMode];
    PlugCheckBoxRenewSpecialHPIsAuto.Checked := g_Config.RenewSpecialHPIsAutos[g_Config.MedicaMode];
    PlugCheckBoxRenewSpecialMPIsAuto.Checked := g_Config.RenewSpecialMPIsAutos[g_Config.MedicaMode];

    PlugEditRenewHPPercent.Value := g_Config.RenewHPPercents[g_Config.MedicaMode];
    PlugEditRenewMPPercent.Value := g_Config.RenewMPPercents[g_Config.MedicaMode];
    PlugEditRenewHPTime.Value := g_Config.RenewHPTimes[g_Config.MedicaMode];
    PlugEditRenewMPTime.Value := g_Config.RenewMPTimes[g_Config.MedicaMode];

    PlugEditRenewSpecialHPPercent.Value := g_Config.RenewSpecialHPPercents[g_Config.MedicaMode];
    PlugEditRenewSpecialMPPercent.Value := g_Config.RenewSpecialMPPercents[g_Config.MedicaMode];
    PlugEditRenewSpecialHPTime.Value := g_Config.RenewSpecialHPTimes[g_Config.MedicaMode];
    PlugEditRenewSpecialMPTime.Value := g_Config.RenewSpecialMPTimes[g_Config.MedicaMode];

    PlugCheckBoxUseSuperMedica.Checked := g_Config.UseSuperMedicas[g_Config.MedicaMode];
    PlugCheckBoxUseSuperMedicaItemName0.Checked := g_Config.SuperMedicaUses[g_Config.MedicaMode][0];
    PlugCheckBoxUseSuperMedicaItemName1.Checked := g_Config.SuperMedicaUses[g_Config.MedicaMode][1];
    PlugCheckBoxUseSuperMedicaItemName2.Checked := g_Config.SuperMedicaUses[g_Config.MedicaMode][2];
    PlugCheckBoxUseSuperMedicaItemName3.Checked := g_Config.SuperMedicaUses[g_Config.MedicaMode][3];
    PlugCheckBoxUseSuperMedicaItemName4.Checked := g_Config.SuperMedicaUses[g_Config.MedicaMode][4];
    PlugCheckBoxUseSuperMedicaItemName5.Checked := g_Config.SuperMedicaUses[g_Config.MedicaMode][5];
    PlugCheckBoxUseSuperMedicaItemName6.Checked := g_Config.SuperMedicaUses[g_Config.MedicaMode][6];
    PlugCheckBoxUseSuperMedicaItemName7.Checked := g_Config.SuperMedicaUses[g_Config.MedicaMode][7];
    PlugCheckBoxUseSuperMedicaItemName8.Checked := g_Config.SuperMedicaUses[g_Config.MedicaMode][8];

    PlugEditSuperMedicaHP0.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][0];
    PlugEditSuperMedicaHP1.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][1];
    PlugEditSuperMedicaHP2.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][2];
    PlugEditSuperMedicaHP3.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][3];
    PlugEditSuperMedicaHP4.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][4];
    PlugEditSuperMedicaHP5.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][5];
    PlugEditSuperMedicaHP6.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][6];
    PlugEditSuperMedicaHP7.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][7];
    PlugEditSuperMedicaHP8.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][8];

    PlugEditSuperMedicaHPTime0.Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][0];
    PlugEditSuperMedicaHPTime1.Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][1];
    PlugEditSuperMedicaHPTime2.Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][2];
    PlugEditSuperMedicaHPTime3.Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][3];
    PlugEditSuperMedicaHPTime4.Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][4];
    PlugEditSuperMedicaHPTime5.Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][5];
    PlugEditSuperMedicaHPTime6.Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][6];
    PlugEditSuperMedicaHPTime7.Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][7];
    PlugEditSuperMedicaHPTime8.Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][8];

    PlugEditSuperMedicaMP0.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][0];
    PlugEditSuperMedicaMP1.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][1];
    PlugEditSuperMedicaMP2.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][2];
    PlugEditSuperMedicaMP3.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][3];
    PlugEditSuperMedicaMP4.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][4];
    PlugEditSuperMedicaMP5.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][5];
    PlugEditSuperMedicaMP6.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][6];
    PlugEditSuperMedicaMP7.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][7];
    PlugEditSuperMedicaMP8.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][8];

    PlugEditSuperMedicaMPTime0.Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][0];
    PlugEditSuperMedicaMPTime1.Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][1];
    PlugEditSuperMedicaMPTime2.Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][2];
    PlugEditSuperMedicaMPTime3.Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][3];
    PlugEditSuperMedicaMPTime4.Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][4];
    PlugEditSuperMedicaMPTime5.Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][5];
    PlugEditSuperMedicaMPTime6.Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][6];
    PlugEditSuperMedicaMPTime7.Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][7];
    PlugEditSuperMedicaMPTime8.Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][8];
  end;
end;

procedure TMirsConfigDlg.RefConfig;
begin
  if not FInitializeed then Exit;
  PlugCheckBoxShowHPLabel.Checked := FConfigCheckeds[ckShowHPLabel];
  PlugCheckBoxNumberLable.Checked := FConfigCheckeds[ckShowNumberLable];
  PlugCheckBoxJobAndLevel.Checked := FConfigCheckeds[ckShowJobAndLevel];
  PlugCheckBoxShowGreenHint.Checked := FConfigCheckeds[ckShowGreenHint];
  PlugCheckBoxShowItemName.Checked := FConfigCheckeds[ckShowItemName];
  PlugCheckBoxShowFilterItem.Checked := FConfigCheckeds[ckShowFilterItem];
  PlugCheckBoxItemHint.Checked := FConfigCheckeds[ckItemHint];
  PlugCheckBoxDisableSelfStruck.Checked := FConfigCheckeds[ckDisableSelfStruck];
  PlugCheckBoxSpeedSlow.Checked := FConfigCheckeds[ckSpeedSlow];
  PlugCheckBoxBGMusic.Checked := FConfigCheckeds[ckBGMusic];
  PlugCheckBoxAutoPickUpItem.Checked := FConfigCheckeds[ckAutoPickUpItem];

  PlugCheckBoxShowActorName.Checked := FConfigCheckeds[ckShowUserName];
  PlugCheckBoxHideDescUserName.Checked := FConfigCheckeds[ckOnlyShowCharName];
  PlugCheckBoxDuraWarning.Checked := FConfigCheckeds[ckDuraWarning];
  PlugCheckBoxNoShift.Checked := FConfigCheckeds[ckNotNeedShift];
  PlugCheckBoxExpFilter.Checked := FConfigCheckeds[ckFilterExp];
  PlugCheckBoxShowMimiMapDesc.Checked := FConfigCheckeds[ckShowMapDesc];
  PlugCheckBoxShowHighlightHPLabel.Checked := FConfigCheckeds[ckShowHighlightHPLabel];
  PlugCheckBoxShowHealthNumber.Checked := FConfigCheckeds[ckShowMoveLable];

  PlugCheckBoxHideGhost.Checked := FConfigCheckeds[ckHideGhost];
  PlugCheckBoxHideHumEffect.Checked := FConfigCheckeds[ckHideHumEffect];
  PlugCheckBoxHideWeaponEffect.Checked := FConfigCheckeds[ckHideWeaponEffect];
  PlugCheckBoxShowMonName.Checked := FConfigCheckeds[ckShowMonName];
  PlugCheckBoxAutoOrderItem.Checked := FConfigCheckeds[ckAutoOrderItem];
  PlugCheckBoxMagicLock.Checked := FConfigCheckeds[ckMagicLock];

  PlugCheckBoxSound.Checked := g_boSound;
  PlugCheckBoxBGMusic.Checked := FConfigCheckeds[ckBGMusic];
  PlugCheckBoxRepeatBGMusic.Checked := FConfigCheckeds[ckRepeatBGMusic];
  PlugCheckBoxNotParaly.Checked := FConfigCheckeds[ckNotParaly];

  g_boBGSound := FConfigCheckeds[ckBGMusic];
  g_boRepeatBGSound := FConfigCheckeds[ckRepeatBGMusic];
  SetRepeatBGSound(g_boRepeatBGSound);

  PlugCheckBoxSmartLongHit.Checked := FConfigCheckeds[ckSmartLongHit];
  PlugCheckBoxSmartPosLongHit.Checked := FConfigCheckeds[ckSmartPosLongHit];
  PlugCheckBoxSmartWalkLongHit.Checked := FConfigCheckeds[ckSmartWalkLongHit];
  PlugCheckBoxSmartWideHit.Checked := FConfigCheckeds[ckSmartWideHit];
  PlugCheckBoxSmartFireHit.Checked := FConfigCheckeds[ckSmartFireHit];
  PlugCheckBoxSmartSwordHit.Checked := FConfigCheckeds[ckSmartSwordHit];
  PlugCheckBoxSmartKTZHit.Checked := FConfigCheckeds[ckSmart66Hit];
  PlugCheckBoxAutoHideMode.Checked := FConfigCheckeds[ckAutoHideMode];
  PlugCheckBoxAutoTakeOnItem.Checked := FConfigCheckeds[ckAutoTakeOnItem];
  PlugCheckBoxAutoChangePoison.Checked := FConfigCheckeds[ckAutoChangePoison];
  PlugCheckBoxHumAutoShield.Checked := FConfigCheckeds[ckHumAutoShield];

  PlugCheckBoxHumStruckShield.Checked := FConfigCheckeds[ckHumStruckShield];
  PlugCheckBoxHeroAutoShield.Checked := FConfigCheckeds[ckHeroAutoShield];
  PlugCheckBoxAssistantHeroAutoShield.Checked := FConfigCheckeds[ckAssistantHeroAutoShield];
  PlugCheckBoxHumManuallySnowWind.Checked := FConfigCheckeds[ckHumManuallySnowWind];
  PlugCheckBoxHumManuallyFireBoom.Checked := FConfigCheckeds[ckHumManuallyFireBoom];
  PlugCheckBoxHumShootLightenLockTarget.Checked := FConfigCheckeds[ckHumShootLightenLockTarget];
  PlugCheckBoxHumManuallyMeteorShower.Checked := FConfigCheckeds[ckHumManuallyMeteorShower];
  PlugCheckBoxAutoMagic.Checked := FConfigCheckeds[ckAutoUseMagic];
  PlugCheckBoxUseKeyBoard.Checked := FConfigCheckeds[ckUseKeyBoard];

  PlugCheckBoxUseSuperMedica.Checked := FConfigCheckeds[ckUseSuperMedica];

  PlugCheckBoxAutoMagic.Checked := FConfigCheckeds[ckAutoUseMagic];

  PlugEditExpFilter.Value := g_Config.nFilterMinExp;
  PlugEditAutoMagicTime.Value := g_Config.nAutoUseMagicTime;

  PlugMemoConfig4Button1.Checked := g_Config.MedicaMode = 0;
  PlugMemoConfig4Button2.Checked := g_Config.MedicaMode = 1;
  PlugMemoConfig4Button3.Checked := g_Config.MedicaMode = 2;
  PlugMemoConfig4Button4.Checked := g_Config.MedicaMode = 3;
  PlugMemoConfig4Button5.Checked := g_Config.MedicaMode = 4;
  RefUseItemConfig(g_Config.MedicaMode);
end;

procedure TMirsConfigDlg.DEditCheckHPPercentChange(Sender: TObject);
begin
  g_Config.CheckHpPercents[g_Config.MedicaMode] := PlugEditCheckHPPercent.Value;
end;

procedure TMirsConfigDlg.DEditCheckMPPercentChange(Sender: TObject);
begin
  g_Config.CheckHpPercents[g_Config.MedicaMode] := PlugEditCheckMPPercent.Value;
end;

procedure TMirsConfigDlg.ComboBoxCheckHPValueChange(Sender: TObject);
begin
  g_Config.CheckHpValues[g_Config.MedicaMode] := PlugComboBoxCheckHPValue.ItemIndex;
end;

procedure TMirsConfigDlg.ComboBoxCheckMPValueChange(Sender: TObject);
begin
  g_Config.CheckMpValues[g_Config.MedicaMode] := PlugComboBoxCheckMPValue.ItemIndex;
end;

procedure TMirsConfigDlg.DCheckBoxCheckHPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.CheckHpIsAutos[g_Config.MedicaMode] := PlugCheckBoxCheckHPIsAuto.Checked;
end;

procedure TMirsConfigDlg.DCheckBoxCheckMPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.CheckMpIsAutos[g_Config.MedicaMode] := PlugCheckBoxCheckMPIsAuto.Checked;
end;

procedure TMirsConfigDlg.DEditRenewHPPercentChange(Sender: TObject);
begin
  g_Config.RenewHPPercents[g_Config.MedicaMode] := PlugEditRenewHPPercent.Value;
end;

procedure TMirsConfigDlg.DEditRenewMPPercentChange(Sender: TObject);
begin
  g_Config.RenewMPPercents[g_Config.MedicaMode] := PlugEditRenewMPPercent.Value;
end;

procedure TMirsConfigDlg.DEditRenewSpecialHPPercentChange(Sender: TObject);
begin
  g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] := PlugEditRenewSpecialHPPercent.Value;
end;

procedure TMirsConfigDlg.DEditRenewSpecialMPPercentChange(Sender: TObject);
begin
  g_Config.RenewSpecialMPPercents[g_Config.MedicaMode] := PlugEditRenewSpecialMPPercent.Value;
end;

procedure TMirsConfigDlg.DEditRenewHPTimeChange(Sender: TObject);
begin
  g_Config.RenewHPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwUseItemIntervalTime, PlugEditRenewHPTime.Value);
  PlugEditRenewHPTime.Value := g_Config.RenewHPTimes[g_Config.MedicaMode];
end;

procedure TMirsConfigDlg.DEditRenewMPTimeChange(Sender: TObject);
begin
  g_Config.RenewMPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwUseItemIntervalTime, PlugEditRenewMPTime.Value);
  PlugEditRenewMPTime.Value := g_Config.RenewMPTimes[g_Config.MedicaMode];
end;

procedure TMirsConfigDlg.DEditRenewSpecialHPTimeChange(Sender: TObject);
begin
  g_Config.RenewSpecialHPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwUseItemIntervalTime, PlugEditRenewSpecialHPTime.Value);
  PlugEditRenewSpecialHPTime.Value := g_Config.RenewSpecialHPTimes[g_Config.MedicaMode];
end;

procedure TMirsConfigDlg.DEditRenewSpecialMPTimeChange(Sender: TObject);
begin
  g_Config.RenewSpecialMPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwUseItemIntervalTime, PlugEditRenewSpecialMPTime.Value);
  PlugEditRenewSpecialMPTime.Value := g_Config.RenewSpecialMPTimes[g_Config.MedicaMode];
end;

procedure TMirsConfigDlg.DCheckBoxRenewHPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.RenewHPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewHPIsAuto.Checked;
end;

procedure TMirsConfigDlg.DCheckBoxRenewMPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.RenewMPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewMPIsAuto.Checked;
end;

procedure TMirsConfigDlg.DCheckBoxRenewSpecialHPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.RenewSpecialHPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewSpecialHPIsAuto.Checked;
end;

procedure TMirsConfigDlg.DCheckBoxRenewSpecialMPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.RenewSpecialMPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewSpecialMPIsAuto.Checked;
end;

procedure TMirsConfigDlg.DEditSuperMedicaHPChange(Sender: TObject);
var
  Index, Value: Integer;
begin
  Index := -1;
  if PlugEditSuperMedicaHP0 = Sender then
  begin
    Index := 0;
    Value := PlugEditSuperMedicaHP0.Value;
  end
  else if PlugEditSuperMedicaHP1 = Sender then
  begin
    Index := 1;
    Value := PlugEditSuperMedicaHP1.Value;
  end
  else if PlugEditSuperMedicaHP2 = Sender then
  begin
    Index := 2;
    Value := PlugEditSuperMedicaHP2.Value;
  end
  else if PlugEditSuperMedicaHP3 = Sender then
  begin
    Index := 3;
    Value := PlugEditSuperMedicaHP3.Value;
  end
  else if PlugEditSuperMedicaHP4 = Sender then
  begin
    Index := 4;
    Value := PlugEditSuperMedicaHP4.Value;
  end
  else if PlugEditSuperMedicaHP5 = Sender then
  begin
    Index := 5;
    Value := PlugEditSuperMedicaHP5.Value;
  end
  else if PlugEditSuperMedicaHP6 = Sender then
  begin
    Index := 6;
    Value := PlugEditSuperMedicaHP6.Value;
  end
  else if PlugEditSuperMedicaHP7 = Sender then
  begin
    Index := 7;
    Value := PlugEditSuperMedicaHP7.Value;
  end
  else if PlugEditSuperMedicaHP8 = Sender then
  begin
    Index := 8;
    Value := PlugEditSuperMedicaHP8.Value;
  end;
  if Index in [0..8] then
  begin
    g_Config.SuperMedicaHPs[g_Config.MedicaMode][Index] := Value;
  end;
end;

procedure TMirsConfigDlg.DEditSuperMedicaHPTimeChange(Sender: TObject);
var
  Index, Value: Integer;
begin
  Index := -1;
  if PlugEditSuperMedicaHPTime0 = Sender then
  begin
    Index := 0;
    Value := PlugEditSuperMedicaHPTime0.Value;
  end
  else if PlugEditSuperMedicaHPTime1 = Sender then
  begin
    Index := 1;
    Value := PlugEditSuperMedicaHPTime1.Value;
  end
  else if PlugEditSuperMedicaHPTime2 = Sender then
  begin
    Index := 2;
    Value := PlugEditSuperMedicaHPTime2.Value;
  end
  else if PlugEditSuperMedicaHPTime3 = Sender then
  begin
    Index := 3;
    Value := PlugEditSuperMedicaHPTime3.Value;
  end
  else if PlugEditSuperMedicaHPTime4 = Sender then
  begin
    Index := 4;
    Value := PlugEditSuperMedicaHPTime4.Value;
  end
  else if PlugEditSuperMedicaHPTime5 = Sender then
  begin
    Index := 5;
    Value := PlugEditSuperMedicaHPTime5.Value;
  end
  else if PlugEditSuperMedicaHPTime6 = Sender then
  begin
    Index := 6;
    Value := PlugEditSuperMedicaHPTime6.Value;
  end
  else if PlugEditSuperMedicaHPTime7 = Sender then
  begin
    Index := 7;
    Value := PlugEditSuperMedicaHPTime7.Value;
  end
  else if PlugEditSuperMedicaHPTime8 = Sender then
  begin
    Index := 8;
    Value := PlugEditSuperMedicaHPTime8.Value;
  end;
  if Index in [0..8] then
  begin
    g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][Index] := Max(g_ClientConfig.dwUseItemIntervalTime, Value);
    TDxEdit(Sender).Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][Index];
  end;
end;

procedure TMirsConfigDlg.DEditSuperMedicaMPChange(Sender: TObject);
var
  Index, Value: Integer;
begin
  Index := -1;
  if PlugEditSuperMedicaMP0 = Sender then
  begin
    Index := 0;
    Value := PlugEditSuperMedicaMP0.Value;
  end
  else if PlugEditSuperMedicaMP1 = Sender then
  begin
    Index := 1;
    Value := PlugEditSuperMedicaMP1.Value;
  end
  else if PlugEditSuperMedicaMP2 = Sender then
  begin
    Index := 2;
    Value := PlugEditSuperMedicaMP2.Value;
  end
  else if PlugEditSuperMedicaMP3 = Sender then
  begin
    Index := 3;
    Value := PlugEditSuperMedicaMP3.Value;
  end
  else if PlugEditSuperMedicaMP4 = Sender then
  begin
    Index := 4;
    Value := PlugEditSuperMedicaMP4.Value;
  end
  else if PlugEditSuperMedicaMP5 = Sender then
  begin
    Index := 5;
    Value := PlugEditSuperMedicaMP5.Value;
  end
  else if PlugEditSuperMedicaMP6 = Sender then
  begin
    Index := 6;
    Value := PlugEditSuperMedicaMP6.Value;
  end
  else if PlugEditSuperMedicaMP7 = Sender then
  begin
    Index := 7;
    Value := PlugEditSuperMedicaMP7.Value;
  end
  else if PlugEditSuperMedicaMP8 = Sender then
  begin
    Index := 8;
    Value := PlugEditSuperMedicaMP8.Value;
  end;

  if Index in [0..8] then
  begin
    g_Config.SuperMedicaMPs[g_Config.MedicaMode][Index] := Value;
  end;
end;

procedure TMirsConfigDlg.DEditSuperMedicaMPTimeChange(Sender: TObject);
var
  Index, Value: Integer;
begin
  Index := -1;
  if PlugEditSuperMedicaMPTime0 = Sender then
  begin
    Index := 0;
    Value := PlugEditSuperMedicaMPTime0.Value;
  end
  else if PlugEditSuperMedicaMPTime1 = Sender then
  begin
    Index := 1;
    Value := PlugEditSuperMedicaMPTime1.Value;
  end
  else if PlugEditSuperMedicaMPTime2 = Sender then
  begin
    Index := 2;
    Value := PlugEditSuperMedicaMPTime2.Value;
  end
  else if PlugEditSuperMedicaMPTime3 = Sender then
  begin
    Index := 3;
    Value := PlugEditSuperMedicaMPTime3.Value;
  end
  else if PlugEditSuperMedicaMPTime4 = Sender then
  begin
    Index := 4;
    Value := PlugEditSuperMedicaMPTime4.Value;
  end
  else if PlugEditSuperMedicaMPTime5 = Sender then
  begin
    Index := 5;
    Value := PlugEditSuperMedicaMPTime5.Value;
  end
  else if PlugEditSuperMedicaMPTime6 = Sender then
  begin
    Index := 6;
    Value := PlugEditSuperMedicaMPTime6.Value;
  end
  else if PlugEditSuperMedicaMPTime7 = Sender then
  begin
    Index := 7;
    Value := PlugEditSuperMedicaMPTime7.Value;
  end
  else if PlugEditSuperMedicaMPTime8 = Sender then
  begin
    Index := 8;
    Value := PlugEditSuperMedicaMPTime8.Value;
  end;
  if Index in [0..8] then
  begin
    g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][Index] := Max(g_ClientConfig.dwUseItemIntervalTime, Value);
    TDxEdit(Sender).Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][Index];
  end;
end;

procedure TMirsConfigDlg.DCheckBoxUseSuperMedicaItemNameClick(Sender: TObject; X, Y: Integer);
var
  Index: Integer;
  Value: Boolean;
begin
  Index := -1;
  if PlugCheckBoxUseSuperMedicaItemName0 = Sender then
  begin
    Index := 0;
    Value := PlugCheckBoxUseSuperMedicaItemName0.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName1 = Sender then
  begin
    Index := 1;
    Value := PlugCheckBoxUseSuperMedicaItemName1.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName2 = Sender then
  begin
    Index := 2;
    Value := PlugCheckBoxUseSuperMedicaItemName2.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName3 = Sender then
  begin
    Index := 3;
    Value := PlugCheckBoxUseSuperMedicaItemName3.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName4 = Sender then
  begin
    Index := 4;
    Value := PlugCheckBoxUseSuperMedicaItemName4.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName5 = Sender then
  begin
    Index := 5;
    Value := PlugCheckBoxUseSuperMedicaItemName5.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName6 = Sender then
  begin
    Index := 6;
    Value := PlugCheckBoxUseSuperMedicaItemName6.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName7 = Sender then
  begin
    Index := 7;
    Value := PlugCheckBoxUseSuperMedicaItemName7.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName8 = Sender then
  begin
    Index := 8;
    Value := PlugCheckBoxUseSuperMedicaItemName8.Checked;
  end;
  if Index in [0..8] then
  begin
    g_Config.SuperMedicaUses[g_Config.MedicaMode][Index] := Value;
  end;
end;

procedure TMirsConfigDlg.DEditChange(Sender: TObject);
begin
  if PlugEditExpFilter = Sender then
  begin
    g_Config.nFilterMinExp := PlugEditExpFilter.Value;
  end
  else if PlugEditAutoMagicTime = Sender then
  begin
    g_Config.nAutoUseMagicTime := PlugEditAutoMagicTime.Value;
  end;
end;

procedure TMirsConfigDlg.MouseMoveEvent(Sender: TObject; Shift: TShiftState;
  X, Y: Integer);
begin
  DScreen.ClearHint;
end;

procedure TMirsConfigDlg.LoadClientConfig(ClientConfig: pTClientConfig);
var
  I, nTop: Integer;
  List: TList;
  DxControl: TDxImageButton;
begin
  FClientConfig := ClientConfig^;

  for I := 0 to Length(FClientConfig.UseSuperMedicaItemNames) - 1 do
  begin
    g_Config.SuperMedicaItemNames[I] := FClientConfig.UseSuperMedicaItemNames[I];
  end;

  List := TList.Create;
  List.Add(PlugCheckBoxUseSuperMedicaItemName0);
  List.Add(PlugCheckBoxUseSuperMedicaItemName1);
  List.Add(PlugCheckBoxUseSuperMedicaItemName2);
  List.Add(PlugCheckBoxUseSuperMedicaItemName3);
  List.Add(PlugCheckBoxUseSuperMedicaItemName4);
  List.Add(PlugCheckBoxUseSuperMedicaItemName5);
  List.Add(PlugCheckBoxUseSuperMedicaItemName6);
  List.Add(PlugCheckBoxUseSuperMedicaItemName7);
  List.Add(PlugCheckBoxUseSuperMedicaItemName8);

  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    DxControl.Caption := g_Config.SuperMedicaItemNames[I];
    DxControl.Visible := g_Config.SuperMedicaItemNames[I] <> '';
  end;
  List.Free;
  PlugEditSuperMedicaHP0.Visible := PlugCheckBoxUseSuperMedicaItemName0.Visible;
  PlugEditSuperMedicaHPTime0.Visible := PlugCheckBoxUseSuperMedicaItemName0.Visible;
  PlugEditSuperMedicaMP0.Visible := PlugCheckBoxUseSuperMedicaItemName0.Visible;
  PlugEditSuperMedicaMPTime0.Visible := PlugCheckBoxUseSuperMedicaItemName0.Visible;

  PlugEditSuperMedicaHP1.Visible := PlugCheckBoxUseSuperMedicaItemName1.Visible;
  PlugEditSuperMedicaHPTime1.Visible := PlugCheckBoxUseSuperMedicaItemName1.Visible;
  PlugEditSuperMedicaMP1.Visible := PlugCheckBoxUseSuperMedicaItemName1.Visible;
  PlugEditSuperMedicaMPTime1.Visible := PlugCheckBoxUseSuperMedicaItemName1.Visible;

  PlugEditSuperMedicaHP2.Visible := PlugCheckBoxUseSuperMedicaItemName2.Visible;
  PlugEditSuperMedicaHPTime2.Visible := PlugCheckBoxUseSuperMedicaItemName2.Visible;
  PlugEditSuperMedicaMP2.Visible := PlugCheckBoxUseSuperMedicaItemName2.Visible;
  PlugEditSuperMedicaMPTime2.Visible := PlugCheckBoxUseSuperMedicaItemName2.Visible;

  PlugEditSuperMedicaHP3.Visible := PlugCheckBoxUseSuperMedicaItemName3.Visible;
  PlugEditSuperMedicaHPTime3.Visible := PlugCheckBoxUseSuperMedicaItemName3.Visible;
  PlugEditSuperMedicaMP3.Visible := PlugCheckBoxUseSuperMedicaItemName3.Visible;
  PlugEditSuperMedicaMPTime3.Visible := PlugCheckBoxUseSuperMedicaItemName3.Visible;

  PlugEditSuperMedicaHP4.Visible := PlugCheckBoxUseSuperMedicaItemName4.Visible;
  PlugEditSuperMedicaHPTime4.Visible := PlugCheckBoxUseSuperMedicaItemName4.Visible;
  PlugEditSuperMedicaMP4.Visible := PlugCheckBoxUseSuperMedicaItemName4.Visible;
  PlugEditSuperMedicaMPTime4.Visible := PlugCheckBoxUseSuperMedicaItemName4.Visible;

  PlugEditSuperMedicaHP5.Visible := PlugCheckBoxUseSuperMedicaItemName5.Visible;
  PlugEditSuperMedicaHPTime5.Visible := PlugCheckBoxUseSuperMedicaItemName5.Visible;
  PlugEditSuperMedicaMP5.Visible := PlugCheckBoxUseSuperMedicaItemName5.Visible;
  PlugEditSuperMedicaMPTime5.Visible := PlugCheckBoxUseSuperMedicaItemName5.Visible;

  PlugEditSuperMedicaHP6.Visible := PlugCheckBoxUseSuperMedicaItemName6.Visible;
  PlugEditSuperMedicaHPTime6.Visible := PlugCheckBoxUseSuperMedicaItemName6.Visible;
  PlugEditSuperMedicaMP6.Visible := PlugCheckBoxUseSuperMedicaItemName6.Visible;
  PlugEditSuperMedicaMPTime6.Visible := PlugCheckBoxUseSuperMedicaItemName6.Visible;

  PlugEditSuperMedicaHP7.Visible := PlugCheckBoxUseSuperMedicaItemName7.Visible;
  PlugEditSuperMedicaHPTime7.Visible := PlugCheckBoxUseSuperMedicaItemName7.Visible;
  PlugEditSuperMedicaMP7.Visible := PlugCheckBoxUseSuperMedicaItemName7.Visible;
  PlugEditSuperMedicaMPTime7.Visible := PlugCheckBoxUseSuperMedicaItemName7.Visible;

  PlugEditSuperMedicaHP8.Visible := PlugCheckBoxUseSuperMedicaItemName8.Visible;
  PlugEditSuperMedicaHPTime8.Visible := PlugCheckBoxUseSuperMedicaItemName8.Visible;
  PlugEditSuperMedicaMP8.Visible := PlugCheckBoxUseSuperMedicaItemName8.Visible;
  PlugEditSuperMedicaMPTime8.Visible := PlugCheckBoxUseSuperMedicaItemName8.Visible;

  PlugTabSheetConfig1.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[0];
  PlugTabSheetConfig2.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[1];
  PlugTabSheetConfig3.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[2];
  PlugTabSheetConfig4.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[3];
  PlugTabSheetConfig5.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[4];
  PlugTabSheetConfig6.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[5];
  PlugTabSheetConfig7.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[6];
  PlugTabSheetConfig8.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[7];

  List := TList.Create;
  List.Add(PlugCheckBoxShowHPLabel);
  List.Add(PlugCheckBoxNumberLable);
  List.Add(PlugCheckBoxJobAndLevel);
  List.Add(PlugCheckBoxShowActorName);
  List.Add(PlugCheckBoxHideDescUserName);
  List.Add(PlugCheckBoxDuraWarning);
  List.Add(PlugCheckBoxNoShift);
  List.Add(PlugCheckBoxShowHighlightHPLabel);
  List.Add(PlugCheckBoxShowMimiMapDesc);
  List.Add(PlugCheckBoxShowMonName);
  List.Add(PlugCheckBoxNotParaly);

  PlugCheckBoxShowHPLabel.Visible := FClientConfig.boShowHPLabel;
  PlugCheckBoxNumberLable.Visible := FClientConfig.boShowNumberLable;
  PlugCheckBoxJobAndLevel.Visible := FClientConfig.boShowJobAndLevel;
  PlugCheckBoxShowActorName.Visible := FClientConfig.boShowUserName;
  PlugCheckBoxHideDescUserName.Visible := FClientConfig.boOnlyShowCharName;
  PlugCheckBoxDuraWarning.Visible := FClientConfig.boDuraWarning;
  PlugCheckBoxNoShift.Visible := FClientConfig.boNotNeedShift;
  PlugCheckBoxShowHighlightHPLabel.Visible := FClientConfig.boShowHighlightHPLabel;
  PlugCheckBoxShowMimiMapDesc.Visible := FClientConfig.boShowMapDesc;
  PlugCheckBoxShowMonName.Visible := FClientConfig.boShowMonName;
  PlugCheckBoxNotParaly.Visible := FClientConfig.boNotParaly;
  nTop := 12;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    if not DxControl.Visible then Continue;
    DxControl.Top := nTop;
    Inc(nTop, 22);
  end;

  List.Clear;
  List.Add(PlugCheckBoxShowGreenHint);
  List.Add(PlugCheckBoxShowHealthNumber);
  List.Add(PlugCheckBoxAutoOrderItem);
  List.Add(PlugCheckBoxMagicLock);
  List.Add(PlugCheckBoxShowItemName);
  List.Add(PlugCheckBoxShowFilterItem);
  List.Add(PlugCheckBoxItemHint);
  List.Add(PlugCheckBoxExpFilter);

  PlugCheckBoxShowGreenHint.Visible := FClientConfig.boShowGreenHint;
  PlugCheckBoxShowHealthNumber.Visible := FClientConfig.boShowMoveLable;
  PlugCheckBoxAutoOrderItem.Visible := FClientConfig.boAutoOrderItem;
  PlugCheckBoxMagicLock.Visible := FClientConfig.boMagicLock;
  PlugCheckBoxShowItemName.Visible := FClientConfig.boShowItemName;
  PlugCheckBoxShowFilterItem.Visible := FClientConfig.boShowFilterItem;
  PlugCheckBoxItemHint.Visible := FClientConfig.boItemHint;
  PlugCheckBoxExpFilter.Visible := FClientConfig.boFilterExp;

  nTop := 12;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    if not DxControl.Visible then Continue;
    DxControl.Top := nTop;
    Inc(nTop, 22);
  end;

  PlugEditExpFilter.Visible := PlugCheckBoxExpFilter.Visible;
  PlugEditExpFilter.Top := PlugCheckBoxExpFilter.Top + 20;

  List.Clear;
  List.Add(PlugCheckBoxAutoPickUpItem);
  List.Add(PlugCheckBoxDisableSelfStruck);
  List.Add(PlugCheckBoxSpeedSlow);
  List.Add(PlugCheckBoxBGMusic);
  List.Add(PlugCheckBoxRepeatBGMusic);
  List.Add(PlugCheckBoxHideGhost);
  List.Add(PlugCheckBoxHideHumEffect);
  List.Add(PlugCheckBoxHideWeaponEffect);
  List.Add(PlugCheckBoxSound);

  PlugCheckBoxAutoPickUpItem.Visible := FClientConfig.boAutoPickUpItem;
  PlugCheckBoxDisableSelfStruck.Visible := FClientConfig.boDisableSelfStruck;
  PlugCheckBoxSpeedSlow.Visible := FClientConfig.boSpeedSlow;
  PlugCheckBoxBGMusic.Visible := FClientConfig.boBGMusic;
  PlugCheckBoxRepeatBGMusic.Visible := FClientConfig.boRepeatBGMusic;
  PlugCheckBoxHideGhost.Visible := FClientConfig.boHideGhost;
  PlugCheckBoxHideHumEffect.Visible := FClientConfig.boHideHumEffect;
  PlugCheckBoxHideWeaponEffect.Visible := FClientConfig.boHideWeaponEffect;


  nTop := 12;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    if not DxControl.Visible then Continue;
    DxControl.Top := nTop;
    Inc(nTop, 22);
  end;

  List.Clear;
  List.Add(PlugCheckBoxSmartLongHit);
  List.Add(PlugCheckBoxSmartPosLongHit);
  List.Add(PlugCheckBoxSmartWalkLongHit);
  List.Add(PlugCheckBoxSmartWideHit);
  List.Add(PlugCheckBoxSmartFireHit);
  List.Add(PlugCheckBoxSmartSwordHit);
  List.Add(PlugCheckBoxSmartKTZHit);
  PlugCheckBoxSmartLongHit.Visible := FClientConfig.boSmartLongHit;
  PlugCheckBoxSmartPosLongHit.Visible := FClientConfig.boSmartPosLongHit;
  PlugCheckBoxSmartWalkLongHit.Visible := FClientConfig.boSmartWalkLongHit;
  PlugCheckBoxSmartWideHit.Visible := FClientConfig.boSmartWideHit;
  PlugCheckBoxSmartFireHit.Visible := FClientConfig.boSmartFireHit;
  PlugCheckBoxSmartSwordHit.Visible := FClientConfig.boSmartSwordHit;
  PlugCheckBoxSmartKTZHit.Visible := FClientConfig.boSmart66Hit;

  nTop := 36;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    if not DxControl.Visible then Continue;
    DxControl.Top := nTop;
    Inc(nTop, 24);
  end;

  List.Clear;
  List.Add(PlugCheckBoxAutoHideMode);
  List.Add(PlugCheckBoxAutoTakeOnItem);
  List.Add(PlugCheckBoxAutoCHangePoison);
  PlugCheckBoxAutoHideMode.Visible := FClientConfig.boAutoHideMode;
  PlugCheckBoxAutoTakeOnItem.Visible := FClientConfig.boAutoTakeOnItem;
  PlugCheckBoxAutoCHangePoison.Visible := FClientConfig.boAutoCHangePoison;
  nTop := 36;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    if not DxControl.Visible then Continue;
    DxControl.Top := nTop;
    Inc(nTop, 24);
  end;


  List.Clear;
  List.Add(PlugCheckBoxHumAutoShield);
  List.Add(PlugCheckBoxHumStruckShield);
  List.Add(PlugCheckBoxHeroAutoShield);
  List.Add(PlugCheckBoxAssistantHeroAutoShield);
  PlugCheckBoxHumAutoShield.Visible := FClientConfig.boHumAutoShield;
  PlugCheckBoxHumStruckShield.Visible := FClientConfig.boHumStruckShield;
  PlugCheckBoxHeroAutoShield.Visible := FClientConfig.boHeroAutoShield;
  PlugCheckBoxAssistantHeroAutoShield.Visible := FClientConfig.boAssistantHeroAutoShield;

  nTop := 132;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    if not DxControl.Visible then Continue;
    DxControl.Top := nTop;
    Inc(nTop, 24);
  end;

  List.Clear;
  List.Add(PlugCheckBoxHumManuallySnowWind);
  List.Add(PlugCheckBoxHumManuallyFireBoom);
  List.Add(PlugCheckBoxHumShootLightenLockTarget);
  List.Add(PlugCheckBoxHumManuallyMeteorShower);
  PlugCheckBoxHumManuallySnowWind.Visible := FClientConfig.boHumManuallySnowWind;
  PlugCheckBoxHumManuallyFireBoom.Visible := FClientConfig.boHumManuallyFireBoom;
  PlugCheckBoxHumShootLightenLockTarget.Visible := FClientConfig.boHumShootLightenLockTarget;
  PlugCheckBoxHumManuallyMeteorShower.Visible := FClientConfig.boHumManuallyMeteorShower;
  nTop := 108;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    if not DxControl.Visible then Continue;
    DxControl.Top := nTop;
    Inc(nTop, 24);
  end;
  List.Free;
  // PlugCheckBoxCanRunHuman.Checked := FClientConfig.boCanRunHuman;
  // PlugCheckBoxCanRunMon.Checked := FClientConfig.boCanRunMon;
  // PlugCheckBoxCanRunNpc.Checked := FClientConfig.boCanRunNpc;
end;

procedure TMirsConfigDlg.Initialize(Handle: THandle;
  ScreenMode: Byte; ClientVersion: TClientVersion; WindowMode: Boolean);
{$IF TESTMODE = 1}
var
  MemoryStream: TMemoryStream;
{$ELSEIF TESTMODE = 2}
var
  MemoryStream: TMemoryStream;
  Stream: TStream;
{$IFEND}
begin
  FHandle := Handle;

  FScreenMode := ScreenMode;
  FClientVersion := ClientVersion;
  FWindowMode := WindowMode;

  if not FLoadControl then
  begin
    FLoadControl := True;
  {  ResourceStream := TResourceStream.Create(HInstance, 'MirsConfigDlg', PChar('GUI'));
    MemoryStream := TMemoryStream.Create;
    ResourceStream.SaveToStream(MemoryStream);
    MemoryStream.Seek(0, soFromBeginning);   }
{$IF TESTMODE = 0}
    if g_ConfigDlgUIStream <> nil then
    begin
      g_ConfigDlgUIStream.Position := 0;
      LoadControlFromMemory(@PlugConfigDlg, g_ConfigDlgUIStream.Memory, g_ConfigDlgUIStream.Size);
      FreeAndNil(g_ConfigDlgUIStream);
    end;
{$ELSEIF TESTMODE = 1}
    MemoryStream := TMemoryStream.Create;
    if g_TestModeUseCustomUI then begin
        MemoryStream.LoadFromFile(g_TestModeUIPath + 'MirsConfigDlg.UI');
    end else begin
        MemoryStream.LoadFromFile(g_TestModeUIPath + 'MirsConfigDlg.GUI');
    end;
    MemoryStream.Position := 0;
    LoadControlFromMemory(@PlugConfigDlg, MemoryStream.Memory, MemoryStream.Size);
    MemoryStream.Free;
{$ELSEIF TESTMODE = 2}
    MemoryStream := TMemoryStream.Create;
    try
      Stream := TResourceStream.Create(Hinstance, 'MIRSCONFIGDLG', 'GUI');

      MemoryStream.LoadFromStream(Stream);
      MemoryStream.Position := 0;
      FrmDlg.LoadFromStream(MemoryStream);

      Stream.Free;
    finally
      MemoryStream.Free;
    end;
{$IFEND}

  end;

  FrmDlg.DWhisperMemo := PlugWhisperMemo;

  LoadHelpFile;

  PlugConfigDlg.OnMouseMove := MouseMoveEvent;
  PlugPageControlConfig.OnMouseMove := MouseMoveEvent;
  PlugMemoConfig1.OnMouseMove := MouseMoveEvent;
  // PlugMemoConfig3.OnMouseMove := MouseMoveEvent;
  PlugMemoConfig4.OnMouseMove := MouseMoveEvent;
  PlugTabSheetConfig4.OnMouseMove := MouseMoveEvent;
  PlugMemoConfig5.OnMouseMove := MouseMoveEvent;
  PlugMemoConfig6.OnMouseMove := MouseMoveEvent;
  PlugTabSheetConfig7.OnMouseMove := MouseMoveEvent;
  PlugMemoConfigHelp.OnMouseMove := MouseMoveEvent;

  PlugConfigDlgClose.OnClick := PlugConfigDlgCloseClickEx;

  PlugPageControlConfig.OnInRealArea := PlugPageControlConfigInRealArea;

  PlugCheckBoxShowHPLabel.OnClick := CheckBoxClickEx;
  PlugCheckBoxNumberLable.OnClick := CheckBoxClickEx;
  PlugCheckBoxJobAndLevel.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowGreenHint.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowItemName.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowFilterItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxItemHint.OnClick := CheckBoxClickEx;
  PlugCheckBoxDisableSelfStruck.OnClick := CheckBoxClickEx;
  PlugCheckBoxSpeedSlow.OnClick := CheckBoxClickEx;
  PlugCheckBoxBGMusic.OnClick := CheckBoxClickEx;
  PlugCheckBoxRepeatBGMusic.OnClick := CheckBoxClickEx;

  PlugCheckBoxShowActorName.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideDescUserName.OnClick := CheckBoxClickEx;
  PlugCheckBoxDuraWarning.OnClick := CheckBoxClickEx;
  PlugCheckBoxNoShift.OnClick := CheckBoxClickEx;
  PlugCheckBoxExpFilter.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowMimiMapDesc.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowHighlightHPLabel.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideGhost.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowHealthNumber.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideDescUserName.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideHumEffect.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideWeaponEffect.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowMonName.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoOrderItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxMagicLock.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoPickUpItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxSound.OnClick := CheckBoxClickEx;
  PlugCheckBoxNotParaly.OnClick := CheckBoxClickEx;

  PlugCheckBoxSmartLongHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartPosLongHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartWalkLongHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartWideHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartFireHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartSwordHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartKTZHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoHideMode.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoTakeOnItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoCHangePoison.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumAutoShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumStruckShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxHeroAutoShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxAssistantHeroAutoShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoMagic.OnClick := CheckBoxClickEx;
  PlugCheckBoxUseKeyBoard.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallySnowWind.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyFireBoom.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumShootLightenLockTarget.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyMeteorShower.OnClick := CheckBoxClickEx;



  PlugComboBoxItemStdMode.ItemIndex := 0;
  PlugComboBoxItemStdMode.OnSelect := DComboBoxItemStdModeSelect;
  PlugEditSearchItem.OnChange := DEditSearchItemChange;
  PlugLabelDefaultItem.OnClick := DLabelDefaultItemClick;
  PlugMemoConfig2.OnListItemClick := ListViewItemClick;

  PlugMemoConfig4Button1.OnClick := RefUseItemConfigClick;
  PlugMemoConfig4Button2.OnClick := RefUseItemConfigClick;
  PlugMemoConfig4Button3.OnClick := RefUseItemConfigClick;
  PlugMemoConfig4Button4.OnClick := RefUseItemConfigClick;
  PlugMemoConfig4Button5.OnClick := RefUseItemConfigClick;

  PlugEditExpFilter.OnChange := DEditChange;
  PlugEditAutoMagicTime.OnChange := DEditChange;

  PlugCheckBoxCheckHPIsAuto.OnClick := DCheckBoxCheckHPIsAutoClick;
  PlugCheckBoxCheckMPIsAuto.OnClick := DCheckBoxCheckMPIsAutoClick;
  PlugEditCheckHPPercent.OnChange := DEditCheckHPPercentChange;
  PlugEditCheckMPPercent.OnChange := DEditCheckMPPercentChange;
  PlugComboBoxCheckHPValue.OnSelect := ComboBoxCheckHPValueChange;
  PlugComboBoxCheckMPValue.OnSelect := ComboBoxCheckMPValueChange;

  PlugCheckBoxRenewHPIsAuto.OnClick := DCheckBoxRenewHPIsAutoClick;
  PlugCheckBoxRenewMPIsAuto.OnClick := DCheckBoxRenewMPIsAutoClick;
  PlugCheckBoxRenewSpecialHPIsAuto.OnClick := DCheckBoxRenewSpecialHPIsAutoClick;
  PlugCheckBoxRenewSpecialMPIsAuto.OnClick := DCheckBoxRenewSpecialMPIsAutoClick;

  PlugEditRenewHPPercent.OnChange := DEditRenewHPPercentChange;
  PlugEditRenewMPPercent.OnChange := DEditRenewMPPercentChange;
  PlugEditRenewHPTime.OnChange := DEditRenewHPTimeChange;
  PlugEditRenewMPTime.OnChange := DEditRenewMPTimeChange;

  PlugEditRenewSpecialHPPercent.OnChange := DEditRenewSpecialHPPercentChange;
  PlugEditRenewSpecialMPPercent.OnChange := DEditRenewSpecialMPPercentChange;
  PlugEditRenewSpecialHPTime.OnChange := DEditRenewSpecialHPTimeChange;
  PlugEditRenewSpecialMPTime.OnChange := DEditRenewSpecialMPTimeChange;

  PlugCheckBoxUseSuperMedica.OnClick := CheckBoxClickEx;

  PlugCheckBoxUseSuperMedicaItemName0.OnClick := DCheckBoxUseSuperMedicaItemNameClick;
  PlugCheckBoxUseSuperMedicaItemName1.OnClick := DCheckBoxUseSuperMedicaItemNameClick;
  PlugCheckBoxUseSuperMedicaItemName2.OnClick := DCheckBoxUseSuperMedicaItemNameClick;
  PlugCheckBoxUseSuperMedicaItemName3.OnClick := DCheckBoxUseSuperMedicaItemNameClick;
  PlugCheckBoxUseSuperMedicaItemName4.OnClick := DCheckBoxUseSuperMedicaItemNameClick;
  PlugCheckBoxUseSuperMedicaItemName5.OnClick := DCheckBoxUseSuperMedicaItemNameClick;
  PlugCheckBoxUseSuperMedicaItemName6.OnClick := DCheckBoxUseSuperMedicaItemNameClick;
  PlugCheckBoxUseSuperMedicaItemName7.OnClick := DCheckBoxUseSuperMedicaItemNameClick;
  PlugCheckBoxUseSuperMedicaItemName8.OnClick := DCheckBoxUseSuperMedicaItemNameClick;

  PlugEditSuperMedicaHP0.OnChange := DEditSuperMedicaHPChange;
  PlugEditSuperMedicaHP1.OnChange := DEditSuperMedicaHPChange;
  PlugEditSuperMedicaHP2.OnChange := DEditSuperMedicaHPChange;
  PlugEditSuperMedicaHP3.OnChange := DEditSuperMedicaHPChange;
  PlugEditSuperMedicaHP4.OnChange := DEditSuperMedicaHPChange;
  PlugEditSuperMedicaHP5.OnChange := DEditSuperMedicaHPChange;
  PlugEditSuperMedicaHP6.OnChange := DEditSuperMedicaHPChange;
  PlugEditSuperMedicaHP7.OnChange := DEditSuperMedicaHPChange;
  PlugEditSuperMedicaHP8.OnChange := DEditSuperMedicaHPChange;

  PlugEditSuperMedicaHPTime0.OnChange := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime1.OnChange := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime2.OnChange := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime3.OnChange := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime4.OnChange := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime5.OnChange := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime6.OnChange := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime7.OnChange := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime8.OnChange := DEditSuperMedicaHPTimeChange;

  PlugEditSuperMedicaMP0.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP1.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP2.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP3.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP4.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP5.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP6.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP7.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP8.OnChange := DEditSuperMedicaMPChange;

  PlugEditSuperMedicaMPTime0.OnChange := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime1.OnChange := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime2.OnChange := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime3.OnChange := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime4.OnChange := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime5.OnChange := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime6.OnChange := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime7.OnChange := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime8.OnChange := DEditSuperMedicaMPTimeChange;

  if FClientVersion < cvHero then
    PlugMemoConfig4Button1.Checked := True;

  PlugMemoConfig4Button2.Visible := FClientVersion >= cvHero;
  PlugMemoConfig4Button3.Visible := FClientVersion >= cvHero;
  PlugMemoConfig4Button4.Visible := FClientVersion >= cvHero;
  PlugMemoConfig4Button5.Visible := FClientVersion >= cvHero;

  PlugCheckBoxHeroAutoShield.Visible := FClientVersion >= cvHero;
  PlugCheckBoxAssistantHeroAutoShield.Visible := FClientVersion >= cvHero;

  FInitializeed := True;
end;

procedure TMirsConfigDlg.Finalize;
begin
  // FInitializeed := False;
end;

procedure TMirsConfigDlg.Logon(const ServerName: string);
begin
  g_sPlugServerName := ServerName;
end;

procedure TMirsConfigDlg.LoadConfig(const CharName: string);
var
  I: Integer;
begin
  g_sPlugUserName := CharName;
  for I := 1 to Length(g_sPlugUserName) do
  begin
    if g_sPlugUserName[I] in ['/', '\', ':', '*', '?', '"', '<', '>', '|'] then
    begin
      case g_sPlugUserName[I] of
        '/': g_sPlugUserName[I] := '{';
        '\': g_sPlugUserName[I] := '}';
        ':': g_sPlugUserName[I] := ';';
        '*': g_sPlugUserName[I] := '@';
        '?': g_sPlugUserName[I] := '!';
        '"': g_sPlugUserName[I] := '~';
        '<': g_sPlugUserName[I] := '(';
        '>': g_sPlugUserName[I] := ')';
        '|': g_sPlugUserName[I] := '-';
      end;
    end;
  end;
  if FLoadControl and (not FLoadConfig) then
  begin
    FLoadConfig := True;
    FileItemDB.LoadFormFile;
    LoadConfigFile;
    RefConfig;
    frmMain.SendPlugInConfig(0);
    RefShowItem;
    FEnabled := True;
  end;
end;

procedure TMirsConfigDlg.Run;
begin
  AutoUseItem(Self);
end;

function TMirsConfigDlg.GetShowItem(const ItemName: string): pTShowItem;
begin
  Result := FileItemDB.Find(ItemName);
end;

function TMirsConfigDlg.FindShowItem(const ItemName: string): Boolean;
var
  ShowItem: pTShowItem;
begin
  ShowItem := FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boShowName
  else
    Result := False;
end;

function TMirsConfigDlg.FindHintItem(const ItemName: string): Boolean;
var
  ShowItem: pTShowItem;
begin
  ShowItem := FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boHintMsg
  else
    Result := False;
end;

function TMirsConfigDlg.FindPickItem(const ItemName: string): Boolean;
var
  ShowItem: pTShowItem;
begin
  ShowItem := FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boPickup
  else
    Result := False;
end;

procedure TMirsConfigDlg.HintItem(const ItemName: string; X, Y: Integer);
begin
  FileItemDB.Hint(ItemName, X, Y);
end;

procedure TMirsConfigDlg.Struck(Actor: TObject; HP, MaxHP: LongInt);
var
  nDamage: Integer;
begin
  if FEnabled then
  begin
    if (g_MySelf <> nil) then
    begin
      if (Actor = g_MySelf) then
      begin
        nDamage := g_MySelf.m_Abil.HP - HP;
        if nDamage > 0 then
          DamageHPUseItem(0, nDamage);
      end;

      if (g_MyHero <> nil) then
      begin
        if (Actor = g_MyHero) then
        begin
          nDamage := g_MyHero.m_Abil.HP - HP;
          if nDamage > 0 then
            DamageHPUseItem(1, nDamage);
        end;
      end;
    end;
  end;
end;

procedure TMirsConfigDlg.HealthChange(Actor: TObject; HP, MP, MaxHP: LongInt);
var
  nDamage: Integer;
begin
  if FEnabled then
  begin
    if (g_MySelf <> nil) then
    begin
      if (Actor = g_MySelf) then
      begin
        nDamage := g_MySelf.m_Abil.HP - HP;
        if nDamage > 0 then
          DamageHPUseItem(0, nDamage);

        nDamage := g_MySelf.m_Abil.MP - MP;
        if nDamage > 0 then
          DamageMPUseItem(0, nDamage);
      end;

      if (g_MyHero <> nil) then
      begin
        if (Actor = g_MyHero) then
        begin
          nDamage := g_MyHero.m_Abil.HP - HP;
          if nDamage > 0 then
            DamageHPUseItem(1, nDamage);

          nDamage := g_MyHero.m_Abil.MP - MP;
          if nDamage > 0 then
            DamageMPUseItem(1, nDamage);
        end;
      end;
    end;
  end;
end;

procedure TMirsConfigDlg.AutoUseItem(Sender: TObject);
begin
  if FEnabled then
  begin
    if FProtectEnabled and (MyGetTickCount - FProtectEnabledTick > 2000) then
    begin
      AutoEatHPItem(Sender);
      AutoEatMPItem(Sender);
      AutoEatSpecialHPItem(Sender);
      AutoEatSpecialMPItem(Sender);
      AutoProtect(Sender);
    end;
    AutoUseMagic(Sender);
    DuraWarning();
  end;
end;

procedure TMirsConfigDlg.AutoEatHPItem(Sender: TObject);
var
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  Death: Boolean;

  function EatHumHPItem(flag: Boolean): Boolean;
  begin
    Result := False;
    if (MyGetTickCount - g_Config.RenewHPTicks[0] > g_Config.RenewHPTimes[0]) and (flag or (SelfAbil.HP < g_Config.RenewHPPercents[0])) then
    begin
      nIndex := FindHumHPItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewHPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end;
    end;
  end;

  function EatHeroHPItem(flag: Boolean): Boolean;
  begin
    Result := False;
    if (MyGetTickCount - g_Config.RenewHPTicks[1] > g_Config.RenewHPTimes[1]) and (flag or (HeroAbil.HP < g_Config.RenewHPPercents[1])) then
    begin
      nIndex := FindHeroHPItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewHPTicks[1] := MyGetTickCount;
        frmMain.HeroEatItem(nIndex);
        Result := True;
      end;
    end;
  end;
begin
  if (g_MySelf <> nil) then
  begin
    Death := g_MySelf.m_boDeath;
    SelfAbil := g_MySelf.m_Abil;
    if g_Config.RenewHPIsAutos[0] and

    (not Death) and (SelfAbil.HP > 0) then EatHumHPItem(False);

    if (g_MyHero <> nil) then
    begin

      Death := g_MyHero.m_boDeath;
      HeroAbil := g_MyHero.m_Abil;
      if g_Config.RenewHPIsAutos[1] and

      (not Death) and (HeroAbil.HP > 0) and (HeroAbil.MaxHP > 0) then EatHeroHPItem(False);
    end;
  end;
end;

procedure TMirsConfigDlg.AutoEatMPItem(Sender: TObject);
var
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  Death: Boolean;

  function EatHumMPItem(flag: Boolean): Boolean;
  begin
    Result := False;
    if (MyGetTickCount - g_Config.RenewMPTicks[0] > g_Config.RenewMPTimes[0]) and (flag or (SelfAbil.MP < g_Config.RenewMPPercents[0])) then
    begin
      nIndex := FindHumMPItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewMPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end;
    end;
  end;

  function EatHeroMPItem(flag: Boolean): Boolean;
  begin
    Result := False;
    if (MyGetTickCount - g_Config.RenewMPTicks[1] > g_Config.RenewMPTimes[1]) and (flag or (HeroAbil.MP < g_Config.RenewMPPercents[1])) then
    begin
      nIndex := FindHeroMPItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewMPTicks[1] := MyGetTickCount;
        frmMain.HeroEatItem(nIndex);
        Result := True;
      end;
    end;
  end;
begin
  if (g_MySelf <> nil) then
  begin
    SelfAbil := g_MySelf.m_Abil;
    Death := g_MySelf.m_boDeath;
    if g_Config.RenewMPIsAutos[0] and

    (not Death) and (SelfAbil.MaxMP > 0) then EatHumMPItem(False);

    if (g_MyHero <> nil) then
    begin
      HeroAbil := g_MyHero.m_Abil;
      Death := g_MyHero.m_boDeath;

      if g_Config.RenewMPIsAutos[1] and

      (not Death) and (HeroAbil.MaxMP > 0) then EatHeroMPItem(False);
    end;
  end;
end;

procedure TMirsConfigDlg.AutoEatSpecialHPItem(Sender: TObject);
var
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  Death: Boolean;

  function EatHumSpecialItem(flag: Boolean): Boolean;
  begin
    Result := False;
    if (MyGetTickCount - g_Config.RenewSpecialHPTicks[0] > g_Config.RenewSpecialHPTimes[0]) and (flag or (SelfAbil.HP < g_Config.RenewSpecialHPPercents[0])) then
    begin
      nIndex := FindHumSpecialItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewSpecialHPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end;
    end;
  end;

  function EatHeroSpecialItem(flag: Boolean): Boolean;
  begin
    Result := False;
    if (MyGetTickCount - g_Config.RenewSpecialHPTicks[1] > g_Config.RenewSpecialHPTimes[1]) and (flag or (HeroAbil.HP < g_Config.RenewSpecialHPPercents[1])) then
    begin
      nIndex := FindHeroSpecialItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewSpecialHPTicks[1] := MyGetTickCount;
        frmMain.HeroEatItem(nIndex);
        Result := True;
      end;
    end;
  end;
begin
  if (g_MySelf <> nil) then
  begin
    SelfAbil := g_MySelf.m_Abil;
    Death := g_MySelf.m_boDeath;
    if g_Config.RenewSpecialHPIsAutos[0] and

    (not Death) and (SelfAbil.HP > 0) then EatHumSpecialItem(False);
    if (g_MyHero <> nil) then
    begin
      HeroAbil := g_MyHero.m_Abil;
      Death := g_MyHero.m_boDeath;
      if g_Config.RenewSpecialHPIsAutos[1] and

      (not Death) and (HeroAbil.HP > 0) and (HeroAbil.MaxHP > 0) then EatHeroSpecialItem(False);
    end;
  end;
end;

procedure TMirsConfigDlg.AutoEatSpecialMPItem(Sender: TObject);
var
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  Death: Boolean;

  function EatHumSpecialItem(flag: Boolean): Boolean;
  begin
    Result := False;
    if (MyGetTickCount - g_Config.RenewSpecialMPTicks[0] > g_Config.RenewSpecialMPTimes[0]) and (flag or (SelfAbil.MP < g_Config.RenewSpecialMPPercents[0])) then
    begin
      nIndex := FindHumSpecialItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewSpecialMPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end;
    end;
  end;

  function EatHeroSpecialItem(flag: Boolean): Boolean;
  begin
    Result := False;
    if (MyGetTickCount - g_Config.RenewSpecialMPTicks[1] > g_Config.RenewSpecialMPTimes[1]) and (flag or (HeroAbil.MP < g_Config.RenewSpecialMPPercents[1])) then
    begin
      nIndex := FindHeroSpecialItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewSpecialMPTicks[1] := MyGetTickCount;
        frmMain.HeroEatItem(nIndex);
        Result := True;
      end;
    end;
  end;
begin
  if (g_MySelf <> nil) then
  begin
    SelfAbil := g_MySelf.m_Abil;
    Death := g_MySelf.m_boDeath;
    if g_Config.RenewSpecialMPIsAutos[0] and

    (not Death) and (SelfAbil.MP > 0) then EatHumSpecialItem(False);
    if (g_MyHero <> nil) then
    begin
      HeroAbil := g_MyHero.m_Abil;
      Death := g_MyHero.m_boDeath;
      if g_Config.RenewSpecialMPIsAutos[1] and

      (not Death) and (HeroAbil.MP > 0) and (HeroAbil.MaxMP > 0) then EatHeroSpecialItem(False);
    end;
  end;
end;

procedure TMirsConfigDlg.AutoProtect;
const
  CM_HEROLOGON = 1050;                                                                              // 召唤英雄
var
  I: Integer;
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  SelfDeath: Boolean;
  HeroDeath: Boolean;
begin
  if (g_ClientConfig.boCloseLogoutProtect) and (g_ClientConfig.boCloseBookProtect) then Exit;
  if g_GJActionMode.Count > 0 then
  begin
    if (g_MySelf <> nil) then
    begin
      SelfAbil := g_MySelf.m_Abil;
      SelfDeath := g_MySelf.m_boDeath;

      for I := 0 to Length(g_Config.CheckHpIsAutos) - 1 do
      begin
        if I = 0 then
        begin
          if g_Config.CheckHpIsAutos[I] and (not SelfDeath) and
            (g_Config.CheckHpValues[I] >= 0) and (g_Config.CheckHpValues[I] < g_GJActionMode.Count) and
            (SelfAbil.HP < g_Config.CheckHpPercents[I]) then
          begin
            if g_Config.CheckHpValues[I] = g_GJActionMode.Count - 1 then
            begin
              if MyGetTickCount - g_Config.CheckHpCheckTicks[I] > g_Config.CheckHpCheckTimes[I] then
              begin
                g_Config.CheckHpCheckTicks[I] := MyGetTickCount;
                if MyGetTickCount - g_Config.CheckHpUseTicks[I] > g_Config.CheckHpUseTimes[I] then
                begin
                  g_Config.CheckHpUseTicks[I] := MyGetTickCount;
                  if (not g_ClientConfig.boCloseLogoutProtect) then
                    frmMain.Logout;
                  Exit;
                end;
              end;
            end
            else
            begin
              if MyGetTickCount - g_Config.CheckHpCheckTicks[I] > g_Config.CheckHpCheckTimes[I] then
              begin
                g_Config.CheckHpCheckTicks[I] := MyGetTickCount;
                if MyGetTickCount - g_Config.CheckHpUseTicks[I] > g_Config.CheckHpUseTimes[I] then
                begin
                  nIndex := FindHumBookItemIndex(g_GJActionMode.Strings[g_Config.CheckHpValues[I]]);
                  if nIndex >= 0 then
                  begin
                    g_Config.CheckHpUseTicks[I] := MyGetTickCount;
                    if (not g_ClientConfig.boCloseBookProtect) then
                      frmMain.AutoEatItem(nIndex);
                    Break;
                  end;
                end;
              end;
            end;
          end;
        end
        else
        begin
          if (g_MyHero <> nil) then
          begin
            HeroAbil := g_MyHero.m_Abil;
            HeroDeath := g_MyHero.m_boDeath;
            if g_Config.CheckHpIsAutos[I] and (not HeroDeath) and                                   // 收回英雄
              (HeroAbil.HP < g_Config.CheckHpPercents[I]) and (MyGetTickCount - g_dwRenewHeroLogOutTick > 5000) then
            begin
              if MyGetTickCount - g_Config.CheckHpCheckTicks[I] > g_Config.CheckHpCheckTimes[I] then
              begin
                g_Config.CheckHpCheckTicks[I] := MyGetTickCount;
                if MyGetTickCount - g_Config.CheckHpUseTicks[I] > g_Config.CheckHpUseTimes[I] then
                begin
                  g_Config.CheckHpUseTicks[I] := MyGetTickCount;
                  if (not g_ClientConfig.boCloseLogoutProtect) then
                    frmMain.SendClientMessage(CM_HEROLOGON, 0, 0, 0, 0);
                  Break;
                end;
              end;
            end;
          end;
        end;
      end;
// -----------------------------------MP----------------------------------------
      for I := 0 to Length(g_Config.CheckMPIsAutos) - 1 do
      begin
        if I = 0 then
        begin
          if g_Config.CheckMPIsAutos[I] and (not SelfDeath) and
            (g_Config.CheckMPValues[I] >= 0) and (g_Config.CheckMPValues[I] < g_GJActionMode.Count) and
            (SelfAbil.MP < g_Config.CheckMPPercents[I]) then
          begin
            if g_Config.CheckMPValues[I] = g_GJActionMode.Count - 1 then
            begin
              if MyGetTickCount - g_Config.CheckMPCheckTicks[I] > g_Config.CheckMPCheckTimes[I] then
              begin
                g_Config.CheckMPCheckTicks[I] := MyGetTickCount;
                if MyGetTickCount - g_Config.CheckMPUseTicks[I] > g_Config.CheckMPUseTimes[I] then
                begin
                  g_Config.CheckMPUseTicks[I] := MyGetTickCount;
                  if (not g_ClientConfig.boCloseLogoutProtect) then
                    frmMain.Logout;
                  Exit;
                end;
              end;
            end
            else
            begin
              if MyGetTickCount - g_Config.CheckMPCheckTicks[I] > g_Config.CheckMPCheckTimes[I] then
              begin
                g_Config.CheckMPCheckTicks[I] := MyGetTickCount;
                if MyGetTickCount - g_Config.CheckMPUseTicks[I] > g_Config.CheckMPUseTimes[I] then
                begin
                  nIndex := FindHumBookItemIndex(g_GJActionMode.Strings[g_Config.CheckMPValues[I]]);
                  if nIndex >= 0 then
                  begin
                    g_Config.CheckMPUseTicks[I] := MyGetTickCount;
                    if (not g_ClientConfig.boCloseBookProtect) then
                      frmMain.AutoEatItem(nIndex);
                    Break;
                  end;
                end;
              end;
            end;
          end;
        end
        else
        begin
          if (g_MyHero <> nil) then
          begin
            HeroAbil := g_MyHero.m_Abil;
            HeroDeath := g_MyHero.m_boDeath;
            if g_Config.CheckMPIsAutos[I] and (not HeroDeath) and                                   // 收回英雄
              (HeroAbil.MP < g_Config.CheckMPPercents[I]) and (MyGetTickCount - g_dwRenewHeroLogOutTick > 5000) then
            begin
              if MyGetTickCount - g_Config.CheckMPCheckTicks[I] > g_Config.CheckMPCheckTimes[I] then
              begin
                g_Config.CheckMPCheckTicks[I] := MyGetTickCount;
                if MyGetTickCount - g_Config.CheckMPUseTicks[I] > g_Config.CheckMPUseTimes[I] then
                begin
                  g_Config.CheckMPUseTicks[I] := MyGetTickCount;
                  if (not g_ClientConfig.boCloseLogoutProtect) then
                    frmMain.SendClientMessage(CM_HEROLOGON, 0, 0, 0, 0);
                  Break;
                end;
              end;
            end;
          end;
        end;
      end;
    end;
  end;
end;

procedure TMirsConfigDlg.DuraWarning();
var
  I: Integer;
  sHint: string;
begin
  if FConfigCheckeds[ckDuraWarning] then
  begin
    if (g_MySelf <> nil) then
    begin
      if MyGetTickCount - FHintItemDuraTick > 1000 * 10 then
      begin
        FHintItemDuraTick := MyGetTickCount;
        for I := Low(TUseItems) to High(TUseItems) do
        begin
          if g_UseItems[I].S.Name <> '' then
          begin
            if g_UseItems[I].Dura <= Round(g_UseItems[I].DuraMax * 10 / 100) then
            begin
              sHint := g_UseItems[I].S.Name + ' 持久过低';
              DScreen.AddChatBoardString(sHint, clyellow, clRed);
            end;
          end;
        end;
      end;
    end;
  end;
end;

// 减血喝药

function NumberSort_1(List: TStringList; Index1, Index2: Integer): Integer;
var
  Value1, Value2: Integer;
begin
  Result := 0;
  try
    Value1 := StrToInt(List[Index1]);
    Value2 := StrToInt(List[Index2]);
    if Value1 > Value2 then
      Result := -1
    else if Value1 < Value2 then
      Result := 1
    else
      Result := 0;
  except
  end;
end;

procedure TMirsConfigDlg.DamageHPUseItem(nObj, nDamage: Integer);
var
  I, nIndex: Integer;
  sItemName: string;
  StringList: TStringList;
  MyHero: TObject;
  SelfDeath, HeroDeath: Boolean;
begin
  if FEnabled then
  begin
    if (nObj in [0..4]) and g_Config.UseSuperMedicas[nObj] then
    begin
      if (g_MySelf <> nil) then
      begin
        SelfDeath := g_MySelf.m_boDeath;
        MyHero := nil;
        HeroDeath := True;
        if (nObj > 0) and (g_MyHero <> nil) then
        begin
          HeroDeath := g_MyHero.m_boDeath;
        end;

        if ((nObj = 0) and (not SelfDeath)) or ((nObj > 0) and (MyHero <> nil) and (not HeroDeath)) then
        begin
          StringList := TStringList.Create;
          for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do
          begin
            StringList.AddObject(IntToStr(g_Config.SuperMedicaHps[nObj][I]), TObject(I));
          end;
          StringList.CustomSort(NumberSort_1);
          for I := 0 to StringList.Count - 1 do
          begin
            nIndex := Integer(StringList.Objects[I]);
            if g_Config.SuperMedicaUses[nObj][nIndex] and
              (nDamage >= g_Config.SuperMedicaHps[nObj][nIndex]) and
              (MyGetTickCount - g_Config.SuperMedicaHpTicks[nObj][nIndex] > g_Config.SuperMedicaHpTimes[nObj][nIndex]) then
            begin
              sItemName := g_Config.SuperMedicaItemNames[nIndex];
              if sItemName <> '' then
              begin
                if nObj = 0 then
                begin
                  nIndex := FindBagItemName(sItemName);
                  if nIndex >= 0 then
                  begin
                    frmMain.AutoEatItem(nIndex);
                    g_Config.SuperMedicaHpTicks[nObj][nIndex] := MyGetTickCount;
                    StringList.Free;
                    Exit;

                  end
                  else
                  begin
                    nIndex := FindHumBindItemIndex(sItemName);
                    if nIndex >= 0 then
                    begin
                      frmMain.AutoEatItem(nIndex);
                      g_Config.SuperMedicaHpTicks[nObj][nIndex] := MyGetTickCount;
                      StringList.Free;
                      Exit;
                    end;
                  end;
                end
                else
                begin
                  nIndex := FindHeroBagItemName(sItemName);
                  if nIndex >= 0 then
                  begin
                    frmMain.HeroEatItem(nIndex);
                    g_Config.SuperMedicaHpTicks[nObj][nIndex] := MyGetTickCount;
                    StringList.Free;
                    Exit;
                  end
                  else
                  begin
                    nIndex := FindHeroBindItemIndex(sItemName);
                    if nIndex >= 0 then
                    begin
                      frmMain.HeroEatItem(nIndex);
                      g_Config.SuperMedicaHpTicks[nObj][nIndex] := MyGetTickCount;
                      StringList.Free;
                      Exit;
                    end;
                  end;
                end;
              end;
            end;
          end;
          StringList.Free;
        end;
      end;
    end;
  end;
end;
// 减MP喝药

procedure TMirsConfigDlg.DamageMPUseItem(nObj, nDamage: Integer);
var
  I, nIndex: Integer;
  sItemName: string;
  StringList: TStringList;
begin
  if FEnabled then
  begin
    if (nObj in [0..4]) and g_Config.UseSuperMedicas[nObj] then
    begin
      if (g_MySelf <> nil) then
      begin

        if ((nObj = 0) and (not g_MySelf.m_boDeath)) or ((nObj > 0) and (g_MyHero <> nil) and (not g_MyHero.m_boDeath)) then
        begin
          StringList := TStringList.Create;
          for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do
          begin
            StringList.AddObject(IntToStr(g_Config.SuperMedicaMPs[nObj][I]), TObject(I));
          end;
          StringList.CustomSort(NumberSort_1);
          for I := 0 to StringList.Count - 1 do
          begin
            nIndex := Integer(StringList.Objects[I]);
            if g_Config.SuperMedicaUses[nObj][nIndex] and
              (nDamage >= g_Config.SuperMedicaMPs[nObj][nIndex]) and
              (MyGetTickCount - g_Config.SuperMedicaMPTicks[nObj][nIndex] > g_Config.SuperMedicaMPTimes[nObj][nIndex]) then
            begin
              sItemName := g_Config.SuperMedicaItemNames[nIndex];
              if sItemName <> '' then
              begin
                if nObj = 0 then
                begin
                  nIndex := FindBagItemName(sItemName);
                  if nIndex >= 0 then
                  begin
                    frmMain.AutoEatItem(nIndex);
                    g_Config.SuperMedicaMPTicks[nObj][nIndex] := MyGetTickCount;
                    StringList.Free;
                    Exit;

                  end
                  else
                  begin
                    nIndex := FindHumBindItemIndex(sItemName);
                    if nIndex >= 0 then
                    begin
                      frmMain.AutoEatItem(nIndex);
                      g_Config.SuperMedicaMPTicks[nObj][nIndex] := MyGetTickCount;
                      StringList.Free;
                      Exit;

                    end;
                  end;
                end
                else
                begin
                  nIndex := FindHeroBagItemName(sItemName);
                  if nIndex >= 0 then
                  begin
                    frmMain.HeroEatItem(nIndex);
                    g_Config.SuperMedicaMPTicks[nObj][nIndex] := MyGetTickCount;
                    StringList.Free;
                    Exit;
                  end
                  else
                  begin
                    nIndex := FindHeroBindItemIndex(sItemName);
                    if nIndex >= 0 then
                    begin
                      frmMain.HeroEatItem(nIndex);
                      g_Config.SuperMedicaMPTicks[nObj][nIndex] := MyGetTickCount;
                      StringList.Free;
                      Exit;
                    end;
                  end;
                end;
              end;
            end;
          end;
          StringList.Free;
        end;
      end;
    end;
  end;
end;


procedure TMirsConfigDlg.AutoUseMagic(Sender: TObject);
var
  ClientMagic: pTClientMagic;
begin
  if FConfigCheckeds[ckAutoUseMagic] then
  begin
    if (g_MySelf <> nil) and
      (not g_MySelf.m_boDeath) and
      (not g_MySelf.m_boShopStall) then
    begin
      if (PlugComboBoxAutoMagic.ItemIndex >= 0) and (PlugComboBoxAutoMagic.ItemIndex < PlugComboBoxAutoMagic.Items.Count) then
      begin
        if MyGetTickCount - g_Config.dwAutoUseMagicTick > g_Config.nAutoUseMagicTime * 1000 then
        begin
          g_Config.dwAutoUseMagicTick := MyGetTickCount;
          ClientMagic := pTClientMagic(PlugComboBoxAutoMagic.Items.Objects[PlugComboBoxAutoMagic.ItemIndex]);
          frmMain.ChangePoisonCharm(ClientMagic);
          frmMain.UseMagic(g_nMouseX, g_nMouseY, ClientMagic);
        end;
      end;
    end;
  end;
end;

function TMirsConfigDlg.CanFilterExp(Exp: LongWord): Boolean;
begin
  Result := False;
  if FConfigCheckeds[ckFilterExp] then
    Result := Exp < g_Config.nFilterMinExp;
end;

procedure TMirsConfigDlg.LoadConfigFile;
var
  I, II: Integer;
  ini: TIniFile;
  sDirectory, sFileName, sIdent, sIdent1, sIdent2, sIdent3, sIdent4, sIdent5, sIdent6: string;
  sIdent7, sIdent8, sIdent9, sIdent10, sIdent11, sIdent12: string;
begin
  sDirectory := ExtractFilePath(ParamStr(0)) + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then
  begin
    g_sPlugUserName := g_MySelf.m_sUserName;
    for I := 1 to Length(g_sPlugUserName) do
    begin
      if g_sPlugUserName[I] in ['/', '\', ':', '*', '?', '"', '<', '>', '|'] then
      begin
        case g_sPlugUserName[I] of
          '/': g_sPlugUserName[I] := '{';
          '\': g_sPlugUserName[I] := '}';
          ':': g_sPlugUserName[I] := ';';
          '*': g_sPlugUserName[I] := '@';
          '?': g_sPlugUserName[I] := '!';
          '"': g_sPlugUserName[I] := '~';
          '<': g_sPlugUserName[I] := '(';
          '>': g_sPlugUserName[I] := ')';
          '|': g_sPlugUserName[I] := '-';
        end;
      end;
    end;
  end;

  sFileName := ExtractFilePath(ParamStr(0)) + Format(CONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);

  ini := TIniFile.Create(sFileName);
  if ini <> nil then
  begin
// ==============================================================================================================
    g_boSound := ini.ReadBool('Setup', 'Sound', g_boSound);
    for I := 0 to Length(FConfigCheckeds) - 1 do
    begin
      FConfigCheckeds[TConfigChecked(I)] := ini.ReadBool('Setup', Format('Checked%d', [I]), FConfigCheckeds[TConfigChecked(I)]);
    end;
    FConfigCheckeds[ckShowRadarActor] := True;
    FConfigCheckeds[ckShowRadarNpc] := True;
    FConfigCheckeds[ckShowRadarAttackNpc] := True;
    FConfigCheckeds[ckShowNpcHPLabel] := True;
    g_Config.MedicaMode := ini.ReadInteger('Protect', 'MedicaMode', g_Config.MedicaMode);
    g_Config.MedicaMode := Max(g_Config.MedicaMode, 0);
    g_Config.MedicaMode := Min(g_Config.MedicaMode, 4);

    for I := 0 to 4 do
    begin
      case I of
        0:
          begin
            sIdent := 'BoUseSuperMedica';

            sIdent1 := 'RenewHPIsAuto';
            sIdent2 := 'RenewHPTime';
            sIdent3 := 'RenewHPPercent';
            sIdent4 := 'RenewSpecialHPIsAuto';
            sIdent5 := 'RenewSpecialHPTime';
            sIdent6 := 'RenewSpecialHPPercent';

            sIdent7 := 'RenewMPIsAuto';
            sIdent8 := 'RenewMPTime';
            sIdent9 := 'RenewMPPercent';
            sIdent10 := 'RenewSpecialMPIsAuto';
            sIdent11 := 'RenewSpecialMPTime';
            sIdent12 := 'RenewSpecialMPPercent';
          end;
        1:
          begin
            sIdent := 'hBoUseSuperMedica';

            sIdent1 := 'RenewHeroNormalHpIsAuto';
            sIdent2 := 'RenewHeroNormalHpTime';
            sIdent3 := 'RenewHeroNormalHpPercent';
            sIdent4 := 'RenewSpecialHeroNormalHpIsAuto';
            sIdent5 := 'RenewSpecialHeroNormalHpTime';
            sIdent6 := 'RenewSpecialHeroNormalHpPercent';

            sIdent7 := 'RenewHeroNormalMpIsAuto';
            sIdent8 := 'RenewHeroNormalMpTime';
            sIdent9 := 'RenewHeroNormalMpPercent';
            sIdent10 := 'RenewSpecialHeroNormalMpIsAuto';
            sIdent11 := 'RenewSpecialHeroNormalMpTime';
            sIdent12 := 'RenewSpecialHeroNormalMpPercent';
          end;
        2:
          begin
            sIdent := 'zBoUseSuperMedica';

            sIdent1 := 'RenewzHeroNormalHpIsAuto';
            sIdent2 := 'RenewzHeroNormalHpTime';
            sIdent3 := 'RenewzHeroNormalHpPercent';
            sIdent4 := 'RenewSpecialzHeroNormalHpIsAuto';
            sIdent5 := 'RenewSpecialzHeroNormalHpTime';
            sIdent6 := 'RenewSpecialzHeroNormalHpPercent';

            sIdent7 := 'RenewzHeroNormalMpIsAuto';
            sIdent8 := 'RenewzHeroNormalMpTime';
            sIdent9 := 'RenewzHeroNormalMpPercent';
            sIdent10 := 'RenewSpecialzHeroNormalMpIsAuto';
            sIdent11 := 'RenewSpecialzHeroNormalMpTime';
            sIdent12 := 'RenewSpecialzHeroNormalMpPercent';
          end;
        3:
          begin
            sIdent := 'fBoUseSuperMedica';

            sIdent1 := 'RenewfHeroNormalHpIsAuto';
            sIdent2 := 'RenewfHeroNormalHpTime';
            sIdent3 := 'RenewfHeroNormalHpPercent';
            sIdent4 := 'RenewSpecialfHeroNormalHpIsAuto';
            sIdent5 := 'RenewSpecialfHeroNormalHpTime';
            sIdent6 := 'RenewSpecialfHeroNormalHpPercent';

            sIdent7 := 'RenewfHeroNormalMpIsAuto';
            sIdent8 := 'RenewfHeroNormalMpTime';
            sIdent9 := 'RenewfHeroNormalMpPercent';
            sIdent10 := 'RenewSpecialfHeroNormalMpIsAuto';
            sIdent11 := 'RenewSpecialfHeroNormalMpTime';
            sIdent12 := 'RenewSpecialfHeroNormalMpPercent';
          end;
        4:
          begin
            sIdent := 'dBoUseSuperMedica';

            sIdent1 := 'RenewdHeroNormalHpIsAuto';
            sIdent2 := 'RenewdHeroNormalHpTime';
            sIdent3 := 'RenewdHeroNormalHpPercent';
            sIdent4 := 'RenewSpecialdHeroNormalHpIsAuto';
            sIdent5 := 'RenewSpecialdHeroNormalHpTime';
            sIdent6 := 'RenewSpecialdHeroNormalHpPercent';

            sIdent7 := 'RenewdHeroNormalMpIsAuto';
            sIdent8 := 'RenewdHeroNormalMpTime';
            sIdent9 := 'RenewdHeroNormalMpPercent';
            sIdent10 := 'RenewSpecialdHeroNormalMpIsAuto';
            sIdent11 := 'RenewSpecialdHeroNormalMpTime';
            sIdent12 := 'RenewSpecialdHeroNormalMpPercent';
          end;
      end;

      g_Config.CheckHpIsAutos[I] := ini.ReadBool('Protect', Format('Hp%dChk', [I + 1]), g_Config.CheckHpIsAutos[I]);
      g_Config.CheckHpPercents[I] := ini.ReadInteger('Protect', Format('Hp%dHp', [I + 1]), g_Config.CheckHpPercents[I]);
      g_Config.CheckHpValues[I] := ini.ReadInteger('Protect', Format('Hp%dMan', [I + 1]), g_Config.CheckHpValues[I]);
      g_Config.CheckMpIsAutos[I] := ini.ReadBool('Protect', Format('Mp%dChk', [I + 1]), g_Config.CheckMpIsAutos[I]);
      g_Config.CheckMpPercents[I] := ini.ReadInteger('Protect', Format('Mp%dHp', [I + 1]), g_Config.CheckMpPercents[I]);
      g_Config.CheckMpValues[I] := ini.ReadInteger('Protect', Format('Mp%dMan', [I + 1]), g_Config.CheckMpValues[I]);

      g_Config.RenewHPIsAutos[I] := ini.ReadBool('Protect', sIdent1, g_Config.RenewHPIsAutos[I]);
      g_Config.RenewHPTimes[I] := ini.ReadInteger('Protect', sIdent2, g_Config.RenewHPTimes[I]);
      g_Config.RenewHPPercents[I] := ini.ReadInteger('Protect', sIdent3, g_Config.RenewHPPercents[I]);

      g_Config.RenewSpecialHPIsAutos[I] := ini.ReadBool('Protect', sIdent4, g_Config.RenewSpecialHPIsAutos[I]);
      g_Config.RenewSpecialHPTimes[I] := ini.ReadInteger('Protect', sIdent5, g_Config.RenewSpecialHPTimes[I]);
      g_Config.RenewSpecialHPPercents[I] := ini.ReadInteger('Protect', sIdent6, g_Config.RenewSpecialHPPercents[I]);

      g_Config.RenewMPIsAutos[I] := ini.ReadBool('Protect', sIdent7, g_Config.RenewMPIsAutos[I]);
      g_Config.RenewMPTimes[I] := ini.ReadInteger('Protect', sIdent8, g_Config.RenewMPTimes[I]);
      g_Config.RenewMPPercents[I] := ini.ReadInteger('Protect', sIdent9, g_Config.RenewMPPercents[I]);

      g_Config.RenewSpecialMPIsAutos[I] := ini.ReadBool('Protect', sIdent10, g_Config.RenewSpecialMPIsAutos[I]);
      g_Config.RenewSpecialMPTimes[I] := ini.ReadInteger('Protect', sIdent11, g_Config.RenewSpecialMPTimes[I]);
      g_Config.RenewSpecialMPPercents[I] := ini.ReadInteger('Protect', sIdent12, g_Config.RenewSpecialMPPercents[I]);

      g_Config.UseSuperMedicas[I] := ini.ReadBool('Protect', sIdent, g_Config.UseSuperMedicas[I]);
    end;

    for I := 0 to 8 do
    begin
      for II := 0 to 4 do
      begin
        case II of
          0:
            begin
              sIdent1 := '%sBoUse';
              sIdent2 := '%sHp';
              sIdent3 := '%sHpTime';
              sIdent4 := '%sMp';
              sIdent5 := '%sMpTime';
            end;
          1:
            begin
              sIdent1 := '%shBoUse';
              sIdent2 := '%sHeroHp';
              sIdent3 := '%sHeroHpTime';
              sIdent4 := '%sHeroMp';
              sIdent5 := '%sHeroMpTime';
            end;
          2:
            begin
              sIdent1 := '%szBoUse';
              sIdent2 := '%szHeroHp';
              sIdent3 := '%szHeroHpTime';
              sIdent4 := '%szHeroMp';
              sIdent5 := '%szHeroMpTime';
            end;
          3:
            begin
              sIdent1 := '%sfBoUse';
              sIdent2 := '%sfHeroHp';
              sIdent3 := '%sfHeroHpTime';
              sIdent4 := '%sfHeroMp';
              sIdent5 := '%sfHeroMpTime';
            end;
          4:
            begin
              sIdent1 := '%sdBoUse';
              sIdent2 := '%sdHeroHp';
              sIdent3 := '%sdHeroHpTime';
              sIdent4 := '%sdHeroMp';
              sIdent5 := '%sdHeroMpTime';
            end;
        end;
        g_Config.SuperMedicaUses[II][I] := ini.ReadBool('Protect', Format(sIdent1, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaUses[II][I]);
        g_Config.SuperMedicaHPs[II][I] := ini.ReadInteger('Protect', Format(sIdent2, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaHPs[II][I]);
        g_Config.SuperMedicaHPTimes[II][I] := ini.ReadInteger('Protect', Format(sIdent3, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaHPTimes[II][I]);
        g_Config.SuperMedicaMPs[II][I] := ini.ReadInteger('Protect', Format(sIdent4, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaMPs[II][I]);
        g_Config.SuperMedicaMPTimes[II][I] := ini.ReadInteger('Protect', Format(sIdent5, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaMPTimes[II][I]);
      end;
    end;
    ini.Free;
  end;
end;

procedure TMirsConfigDlg.SaveConfigFile;
var
  I, II: Integer;
  ini: TIniFile;
  sDirectory, sFileName, sIdent, sIdent1, sIdent2, sIdent3, sIdent4, sIdent5, sIdent6: string;
  sIdent7, sIdent8, sIdent9, sIdent10, sIdent11, sIdent12: string;
begin
  sDirectory := ExtractFilePath(ParamStr(0)) + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);
  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then
  begin
    g_sPlugUserName := g_MySelf.m_sUserName;
    for I := 1 to Length(g_sPlugUserName) do
    begin
      if g_sPlugUserName[I] in ['/', '\', ':', '*', '?', '"', '<', '>', '|'] then
      begin
        case g_sPlugUserName[I] of
          '/': g_sPlugUserName[I] := '{';
          '\': g_sPlugUserName[I] := '}';
          ':': g_sPlugUserName[I] := ';';
          '*': g_sPlugUserName[I] := '@';
          '?': g_sPlugUserName[I] := '!';
          '"': g_sPlugUserName[I] := '~';
          '<': g_sPlugUserName[I] := '(';
          '>': g_sPlugUserName[I] := ')';
          '|': g_sPlugUserName[I] := '-';
        end;
      end;
    end;
  end;

  sFileName := ExtractFilePath(ParamStr(0)) + Format(CONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);

  ini := TIniFile.Create(sFileName);
  if ini <> nil then
  begin
    try
      ini.WriteBool('Setup', 'Sound', g_boSound);

      for I := 0 to Length(FConfigCheckeds) - 1 do
      begin
        ini.WriteBool('Setup', Format('Checked%d', [I]), FConfigCheckeds[TConfigChecked(I)]);
      end;

      for I := 0 to 4 do
      begin
        case I of
          0:
            begin
              sIdent := 'BoUseSuperMedica';

              sIdent1 := 'RenewHPIsAuto';
              sIdent2 := 'RenewHPTime';
              sIdent3 := 'RenewHPPercent';
              sIdent4 := 'RenewSpecialHPIsAuto';
              sIdent5 := 'RenewSpecialHPTime';
              sIdent6 := 'RenewSpecialHPPercent';

              sIdent7 := 'RenewMPIsAuto';
              sIdent8 := 'RenewMPTime';
              sIdent9 := 'RenewMPPercent';
              sIdent10 := 'RenewSpecialMPIsAuto';
              sIdent11 := 'RenewSpecialMPTime';
              sIdent12 := 'RenewSpecialMPPercent';
            end;
          1:
            begin
              sIdent := 'hBoUseSuperMedica';

              sIdent1 := 'RenewHeroNormalHpIsAuto';
              sIdent2 := 'RenewHeroNormalHpTime';
              sIdent3 := 'RenewHeroNormalHpPercent';
              sIdent4 := 'RenewSpecialHeroNormalHpIsAuto';
              sIdent5 := 'RenewSpecialHeroNormalHpTime';
              sIdent6 := 'RenewSpecialHeroNormalHpPercent';

              sIdent7 := 'RenewHeroNormalMpIsAuto';
              sIdent8 := 'RenewHeroNormalMpTime';
              sIdent9 := 'RenewHeroNormalMpPercent';
              sIdent10 := 'RenewSpecialHeroNormalMpIsAuto';
              sIdent11 := 'RenewSpecialHeroNormalMpTime';
              sIdent12 := 'RenewSpecialHeroNormalMpPercent';
            end;
          2:
            begin
              sIdent := 'zBoUseSuperMedica';

              sIdent1 := 'RenewzHeroNormalHpIsAuto';
              sIdent2 := 'RenewzHeroNormalHpTime';
              sIdent3 := 'RenewzHeroNormalHpPercent';
              sIdent4 := 'RenewSpecialzHeroNormalHpIsAuto';
              sIdent5 := 'RenewSpecialzHeroNormalHpTime';
              sIdent6 := 'RenewSpecialzHeroNormalHpPercent';

              sIdent7 := 'RenewzHeroNormalMpIsAuto';
              sIdent8 := 'RenewzHeroNormalMpTime';
              sIdent9 := 'RenewzHeroNormalMpPercent';
              sIdent10 := 'RenewSpecialzHeroNormalMpIsAuto';
              sIdent11 := 'RenewSpecialzHeroNormalMpTime';
              sIdent12 := 'RenewSpecialzHeroNormalMpPercent';
            end;
          3:
            begin
              sIdent := 'fBoUseSuperMedica';

              sIdent1 := 'RenewfHeroNormalHpIsAuto';
              sIdent2 := 'RenewfHeroNormalHpTime';
              sIdent3 := 'RenewfHeroNormalHpPercent';
              sIdent4 := 'RenewSpecialfHeroNormalHpIsAuto';
              sIdent5 := 'RenewSpecialfHeroNormalHpTime';
              sIdent6 := 'RenewSpecialfHeroNormalHpPercent';

              sIdent7 := 'RenewfHeroNormalMpIsAuto';
              sIdent8 := 'RenewfHeroNormalMpTime';
              sIdent9 := 'RenewfHeroNormalMpPercent';
              sIdent10 := 'RenewSpecialfHeroNormalMpIsAuto';
              sIdent11 := 'RenewSpecialfHeroNormalMpTime';
              sIdent12 := 'RenewSpecialfHeroNormalMpPercent';
            end;
          4:
            begin
              sIdent := 'dBoUseSuperMedica';

              sIdent1 := 'RenewdHeroNormalHpIsAuto';
              sIdent2 := 'RenewdHeroNormalHpTime';
              sIdent3 := 'RenewdHeroNormalHpPercent';
              sIdent4 := 'RenewSpecialdHeroNormalHpIsAuto';
              sIdent5 := 'RenewSpecialdHeroNormalHpTime';
              sIdent6 := 'RenewSpecialdHeroNormalHpPercent';

              sIdent7 := 'RenewdHeroNormalMpIsAuto';
              sIdent8 := 'RenewdHeroNormalMpTime';
              sIdent9 := 'RenewdHeroNormalMpPercent';
              sIdent10 := 'RenewSpecialdHeroNormalMpIsAuto';
              sIdent11 := 'RenewSpecialdHeroNormalMpTime';
              sIdent12 := 'RenewSpecialdHeroNormalMpPercent';
            end;
        end;

        ini.WriteBool('Protect', Format('Hp%dChk', [I + 1]), g_Config.CheckHpIsAutos[I]);
        ini.WriteInteger('Protect', Format('Hp%dHp', [I + 1]), g_Config.CheckHpPercents[I]);
        ini.WriteInteger('Protect', Format('Hp%dMan', [I + 1]), g_Config.CheckHpValues[I]);
        ini.WriteBool('Protect', Format('Mp%dChk', [I + 1]), g_Config.CheckMpIsAutos[I]);
        ini.WriteInteger('Protect', Format('Mp%dHp', [I + 1]), g_Config.CheckMpPercents[I]);
        ini.WriteInteger('Protect', Format('Mp%dMan', [I + 1]), g_Config.CheckMpValues[I]);

        ini.WriteBool('Protect', sIdent1, g_Config.RenewHPIsAutos[I]);
        ini.WriteInteger('Protect', sIdent2, g_Config.RenewHPTimes[I]);
        ini.WriteInteger('Protect', sIdent3, g_Config.RenewHPPercents[I]);

        ini.WriteBool('Protect', sIdent4, g_Config.RenewSpecialHPIsAutos[I]);
        ini.WriteInteger('Protect', sIdent5, g_Config.RenewSpecialHPTimes[I]);
        ini.WriteInteger('Protect', sIdent6, g_Config.RenewSpecialHPPercents[I]);

        ini.WriteBool('Protect', sIdent7, g_Config.RenewMPIsAutos[I]);
        ini.WriteInteger('Protect', sIdent8, g_Config.RenewMPTimes[I]);
        ini.WriteInteger('Protect', sIdent9, g_Config.RenewMPPercents[I]);

        ini.WriteBool('Protect', sIdent10, g_Config.RenewSpecialMPIsAutos[I]);
        ini.WriteInteger('Protect', sIdent11, g_Config.RenewSpecialMPTimes[I]);
        ini.WriteInteger('Protect', sIdent12, g_Config.RenewSpecialMPPercents[I]);

        ini.WriteBool('Protect', sIdent, g_Config.UseSuperMedicas[I]);
      end;

      for I := 0 to 8 do
      begin
        for II := 0 to 4 do
        begin
          case II of
            0:
              begin
                sIdent1 := '%sBoUse';
                sIdent2 := '%sHp';
                sIdent3 := '%sHpTime';
                sIdent4 := '%sMp';
                sIdent5 := '%sMpTime';
              end;
            1:
              begin
                sIdent1 := '%shBoUse';
                sIdent2 := '%sHeroHp';
                sIdent3 := '%sHeroHpTime';
                sIdent4 := '%sHeroMp';
                sIdent5 := '%sHeroMpTime';
              end;
            2:
              begin
                sIdent1 := '%szBoUse';
                sIdent2 := '%szHeroHp';
                sIdent3 := '%szHeroHpTime';
                sIdent4 := '%szHeroMp';
                sIdent5 := '%szHeroMpTime';
              end;
            3:
              begin
                sIdent1 := '%sfBoUse';
                sIdent2 := '%sfHeroHp';
                sIdent3 := '%sfHeroHpTime';
                sIdent4 := '%sfHeroMp';
                sIdent5 := '%sfHeroMpTime';
              end;
            4:
              begin
                sIdent1 := '%sdBoUse';
                sIdent2 := '%sdHeroHp';
                sIdent3 := '%sdHeroHpTime';
                sIdent4 := '%sdHeroMp';
                sIdent5 := '%sdHeroMpTime';
              end;
          end;
          ini.WriteBool('Protect', Format(sIdent1, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaUses[II][I]);
          ini.WriteInteger('Protect', Format(sIdent2, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaHPs[II][I]);
          ini.WriteInteger('Protect', Format(sIdent3, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaHPTimes[II][I]);
          ini.WriteInteger('Protect', Format(sIdent4, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaMPs[II][I]);
          ini.WriteInteger('Protect', Format(sIdent5, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaMPTimes[II][I]);
        end;
      end;
    except
      on E: Exception do
      begin
        DebugOutStr('[Exception] TMirsConfigDlg::SaveConfigFile');
        DebugOutStr(E.Message);
      end;
    end;
    ini.Free;
  end;
end;

end.

unit MirReturnConfigDlg;

interface
uses
  Windows, Controls, SysUtils, StrUtils, Classes, StdCtrls, Dialogs, Graphics, Types, Grids, DxImageForm, DxImageButton, DxPageControl,
  DxEdit, DxLabel, DxMemo, DxImageGrid, DxPopupMenu, DxComboBox, DxControls, DxLine,
  DxComponents, Grobal2, GameConfigDlgs, GameConfigDlg, SDK, SoundUtil, DxTrackBar;
type
  TMirReturnConfigDlg = class(TGameConfigObject)
    PlugConfigDlg: TDxImageForm;
    PlugConfigDlgClose: TDxImageButton;
    PlugPageControlConfig: TDxPageControl;
    PlugTabSheetConfig1: TDxTabSheet;
    PlugMemoConfig1: TDxScrollBox;
    PlugCheckBoxAutoOrderItem: TDxImageButton;
    PlugCheckBoxAutoPickUpItem: TDxImageButton;
    PlugCheckBoxBGMusic: TDxImageButton;
    PlugCheckBoxContinueButchItem: TDxImageButton;
    PlugCheckBoxDisableSelfStruck: TDxImageButton;
    PlugCheckBoxDuraWarning: TDxImageButton;
    PlugCheckBoxExpFilter: TDxImageButton;
    PlugCheckBoxHideDescUserName: TDxImageButton;
    PlugCheckBoxHideGhost: TDxImageButton;
    PlugCheckBoxHideHumEffect: TDxImageButton;
    PlugCheckBoxHideTitle: TDxImageButton;
    PlugCheckBoxHideWeaponEffect: TDxImageButton;
    PlugCheckBoxItemHint: TDxImageButton;
    PlugCheckBoxJobAndLevel: TDxImageButton;
    PlugCheckBoxMagicLock: TDxImageButton;
    PlugCheckBoxMovePick: TDxImageButton;
    PlugCheckBoxNoShift: TDxImageButton;
    PlugCheckBoxNotParaly: TDxImageButton;
    PlugCheckBoxNumberLable: TDxImageButton;
    PlugCheckBoxRepeatBGMusic: TDxImageButton;
    PlugCheckBoxSceneShake: TDxImageButton;
    PlugCheckBoxShiftSwitch: TDxImageButton;
    PlugCheckBoxShowActorName: TDxImageButton;
    PlugCheckBoxShowFilterItem: TDxImageButton;
    PlugCheckBoxShowGreenHint: TDxImageButton;
    PlugCheckBoxShowHPLabel: TDxImageButton;
    PlugCheckBoxShowHealthNumber: TDxImageButton;
    PlugCheckBoxShowHighlightHPLabel: TDxImageButton;
    PlugCheckBoxShowItemName: TDxImageButton;
    PlugCheckBoxShowMimiMapDesc: TDxImageButton;
    PlugCheckBoxShowMonName: TDxImageButton;
    PlugCheckBoxShowNGLabel: TDxImageButton;
    PlugCheckBoxShowNpcHPLabel: TDxImageButton;
    PlugCheckBoxShowNpcName: TDxImageButton;
    PlugCheckBoxSpeedSlow: TDxImageButton;
    PlugCheckBoxVolume: TDxImageButton;
    PlugCheckDisableChartMemoSize: TDxImageButton;
    PlugEditExpFilter: TDxEdit;
    TrackBarVolume: TDxTrackBar;
    PlugTabSheetConfig2: TDxTabSheet;
    PlugBtnDiyAdd: TDxImageButton;
    PlugBtnDiyDel: TDxImageButton;
    PlugBtnDiyLoad: TDxImageButton;
    PlugBtnDiySave: TDxImageButton;
    PlugCheckBoxItemCmp: TDxImageButton;
    PlugCheckBoxPickUpAll: TDxImageButton;
    PlugCheckBoxSpecialQuickFlashing: TDxImageButton;
    PlugComboBoxItemStdMode: TDxComboBox;
    PlugEditSearchItem: TDxEdit;
    PlugEditSpecialColor: TDxEdit;
    PlugEditSpecialName: TDxEdit;
    PlugLabelDefaultItem: TDxLabel;
    PlugLabelSpecialColor: TDxLabel;
    PlugMemoConfig2: TDxListView;
    PlugMemoConfig2Label24: TDxLabel;
    PlugMemoConfig2Label25: TDxLabel;
    PlugMemoConfig2Label26: TDxLabel;
    PlugMemoConfig2Label27: TDxLabel;
    PlugMemoConfig2Label28: TDxLabel;
    PlugMemoConfig2Label29: TDxLabel;
    PlugMemoConfig2Line1: TDxLine;
    PlugMemoConfig2Line2: TDxLine;
    PlugTabSheetConfig3: TDxTabSheet;
    PlugMemoConfig3: TDxScrollBox;
    PlugCheckBoxHeroContinuousNoHitMon: TDxImageButton;
    PlugCheckBoxHeroRenewAlcoholIsAuto: TDxImageButton;
    PlugCheckBoxHeroRenewMedicineAlcoholIsAuto: TDxImageButton;
    PlugCheckBoxHeroShowNumberState: TDxImageButton;
    PlugCheckBoxRenewAlcoholIsAuto: TDxImageButton;
    PlugCheckBoxRenewDeliriaIsAuto: TDxImageButton;
    PlugCheckBoxRenewMedicineAlcoholIsAuto: TDxImageButton;
    PlugEditHeroDodgeHPPercent: TDxEdit;
    PlugEditHeroRenewAlcoholPercent: TDxEdit;
    PlugEditHeroRenewMedicineAlcoholPercent: TDxEdit;
    PlugEditRenewAlcoholPercent: TDxEdit;
    PlugEditRenewMedicineAlcoholPercent: TDxEdit;
    PlugMemoConfig3Label1: TDxLabel;
    PlugMemoConfig3Label2: TDxLabel;
    PlugMemoConfig3Label3: TDxLabel;
    PlugMemoConfig3Label4: TDxLabel;
    PlugMemoConfig3Label5: TDxLabel;
    PlugMemoConfig3Label6: TDxLabel;
    PlugMemoConfig3Label7: TDxLabel;
    PlugMemoConfig3Label8: TDxLabel;
    PlugTabSheetConfig4: TDxTabSheet;
    PlugMemoConfig4: TDxScrollBox;
    PlugCheckBoxAutoPercent: TDxImageButton;
    PlugCheckBoxCheckDuraIsAuto: TDxImageButton;
    PlugCheckBoxCheckHPIsAuto: TDxImageButton;
    PlugCheckBoxCheckMPIsAuto: TDxImageButton;
    PlugCheckBoxRenewAutoPercent: TDxImageButton;
    PlugCheckBoxRenewHPIsAuto: TDxImageButton;
    PlugCheckBoxRenewMPIsAuto: TDxImageButton;
    PlugCheckBoxRenewSpecialHPIsAuto: TDxImageButton;
    PlugCheckBoxRenewSpecialMPIsAuto: TDxImageButton;
    PlugCheckBoxSuperMedicaPercent: TDxImageButton;
    PlugCheckBoxUseSuperMedica: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName0: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName1: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName2: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName3: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName4: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName5: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName6: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName7: TDxImageButton;
    PlugCheckBoxUseSuperMedicaItemName8: TDxImageButton;
    PlugComboBoxCheckHPValue: TDxComboBox;
    PlugComboBoxCheckMPValue: TDxComboBox;
    PlugEditCheckDura: TDxEdit;
    PlugEditCheckDuraTime: TDxEdit;
    PlugEditCheckDuraValue: TDxEdit;
    PlugEditCheckHPPercent: TDxEdit;
    PlugEditCheckMPPercent: TDxEdit;
    PlugEditRenewHPPercent: TDxEdit;
    PlugEditRenewHPTime: TDxEdit;
    PlugEditRenewMPPercent: TDxEdit;
    PlugEditRenewMPTime: TDxEdit;
    PlugEditRenewSpecialHPPercent: TDxEdit;
    PlugEditRenewSpecialHPTime: TDxEdit;
    PlugEditRenewSpecialMPPercent: TDxEdit;
    PlugEditRenewSpecialMPTime: TDxEdit;
    PlugEditSuperMedicaHP0: TDxEdit;
    PlugEditSuperMedicaHP1: TDxEdit;
    PlugEditSuperMedicaHP2: TDxEdit;
    PlugEditSuperMedicaHP3: TDxEdit;
    PlugEditSuperMedicaHP4: TDxEdit;
    PlugEditSuperMedicaHP5: TDxEdit;
    PlugEditSuperMedicaHP6: TDxEdit;
    PlugEditSuperMedicaHP7: TDxEdit;
    PlugEditSuperMedicaHP8: TDxEdit;
    PlugEditSuperMedicaHPTime0: TDxEdit;
    PlugEditSuperMedicaHPTime1: TDxEdit;
    PlugEditSuperMedicaHPTime2: TDxEdit;
    PlugEditSuperMedicaHPTime3: TDxEdit;
    PlugEditSuperMedicaHPTime4: TDxEdit;
    PlugEditSuperMedicaHPTime5: TDxEdit;
    PlugEditSuperMedicaHPTime6: TDxEdit;
    PlugEditSuperMedicaHPTime7: TDxEdit;
    PlugEditSuperMedicaHPTime8: TDxEdit;
    PlugEditSuperMedicaMP0: TDxEdit;
    PlugEditSuperMedicaMP1: TDxEdit;
    PlugEditSuperMedicaMP2: TDxEdit;
    PlugEditSuperMedicaMP3: TDxEdit;
    PlugEditSuperMedicaMP4: TDxEdit;
    PlugEditSuperMedicaMP5: TDxEdit;
    PlugEditSuperMedicaMP6: TDxEdit;
    PlugEditSuperMedicaMP7: TDxEdit;
    PlugEditSuperMedicaMP8: TDxEdit;
    PlugEditSuperMedicaMPTime0: TDxEdit;
    PlugEditSuperMedicaMPTime1: TDxEdit;
    PlugEditSuperMedicaMPTime2: TDxEdit;
    PlugEditSuperMedicaMPTime3: TDxEdit;
    PlugEditSuperMedicaMPTime4: TDxEdit;
    PlugEditSuperMedicaMPTime5: TDxEdit;
    PlugEditSuperMedicaMPTime6: TDxEdit;
    PlugEditSuperMedicaMPTime7: TDxEdit;
    PlugEditSuperMedicaMPTime8: TDxEdit;
    PlugMemoConfig4Label1: TDxLabel;
    PlugMemoConfig4Label2: TDxLabel;
    PlugMemoConfig4Label3: TDxLabel;
    PlugMemoConfig4Label4: TDxLabel;
    PlugMemoConfig4Label5: TDxLabel;
    PlugMemoConfig4Label6: TDxLabel;
    PlugMemoConfig4Label7: TDxLabel;
    PlugMemoConfig4Label8: TDxLabel;
    PlugMemoConfig4LabelHint: TDxLabel;
    PlugMemoConfig4LabelHint2: TDxLabel;
    PlugMemoConfig4Line2: TDxLine;
    PlugMemoConfig4Line3: TDxLine;
    PlugMemoConfig4Line4: TDxLine;
    PlugMemoConfig4Line5: TDxLine;
    PlugMemoConfig4Button1: TDxImageButton;
    PlugMemoConfig4Button2: TDxImageButton;
    PlugMemoConfig4Button3: TDxImageButton;
    PlugMemoConfig4Button4: TDxImageButton;
    PlugMemoConfig4Button5: TDxImageButton;
    PlugMemoConfig4Line1: TDxLine;
    PlugTabSheetConfig5: TDxTabSheet;
    PlugMemoConfig5: TDxScrollBox;
    PlugCheckBoxAssistantHeroAutoShield: TDxImageButton;
    PlugCheckBoxAutoCHangePoison: TDxImageButton;
    PlugCheckBoxAutoHideMode: TDxImageButton;
    PlugCheckBoxAutoMagic: TDxImageButton;
    PlugCheckBoxAutoTakeOnItem: TDxImageButton;
    PlugCheckBoxHeroAutoShield: TDxImageButton;
    PlugCheckBoxHumAutoShield: TDxImageButton;
    PlugCheckBoxHumManuallyFireBoom: TDxImageButton;
    PlugCheckBoxHumManuallyMeteorShower: TDxImageButton;
    PlugCheckBoxHumManuallySnowWind: TDxImageButton;
    PlugCheckBoxHumShootLightenLockTarget: TDxImageButton;
    PlugCheckBoxHumStruckShield: TDxImageButton;
    PlugCheckBoxSmartCRSHit: TDxImageButton;
    PlugCheckBoxSmartFireHit: TDxImageButton;
    PlugCheckBoxSmartKTZHit: TDxImageButton;
    PlugCheckBoxSmartLongHit: TDxImageButton;
    PlugCheckBoxSmartPosLongHit: TDxImageButton;
    PlugCheckBoxSmartSwordHit: TDxImageButton;
    PlugCheckBoxSmartTWNHit: TDxImageButton;
    PlugCheckBoxSmartWalkLongHit: TDxImageButton;
    PlugCheckBoxSmartWideHit: TDxImageButton;
    PlugComboBoxAutoMagic: TDxComboBox;
    PlugEditAutoMagicTime: TDxEdit;
    PlugMemoConfig5Label19: TDxLabel;
    PlugMemoConfig5Label20: TDxLabel;
    PlugMemoConfig5Label21: TDxLabel;
    PlugMemoConfig5Label22: TDxLabel;
    PlugMemoConfig5Label23: TDxLabel;
    PlugTabSheetConfig6: TDxTabSheet;
    PlugMemoConfig6: TDxScrollBox;
    PlugCheckBoxUseKeyBoard: TDxImageButton;
    PlugMemoConfig6Label1: TDxLabel;
    PlugMemoConfig6Label2: TDxLabel;
    PlugMemoConfig6Label3: TDxLabel;
    PlugMemoConfig6LabelKeyBoard1: TDxLabel;
    PlugMemoConfig6LabelKeyBoard10: TDxLabel;
    PlugMemoConfig6LabelKeyBoard11: TDxLabel;
    PlugMemoConfig6LabelKeyBoard12: TDxLabel;
    PlugMemoConfig6LabelKeyBoard2: TDxLabel;
    PlugMemoConfig6LabelKeyBoard3: TDxLabel;
    PlugMemoConfig6LabelKeyBoard4: TDxLabel;
    PlugMemoConfig6LabelKeyBoard5: TDxLabel;
    PlugMemoConfig6LabelKeyBoard6: TDxLabel;
    PlugMemoConfig6LabelKeyBoard7: TDxLabel;
    PlugMemoConfig6LabelKeyBoard8: TDxLabel;
    PlugMemoConfig6LabelKeyBoard9: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc1: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc10: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc11: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc12: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc2: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc3: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc4: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc5: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc6: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc7: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc8: TDxLabel;
    PlugMemoConfig6LabelKeyBoardDesc9: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal1: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal10: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal11: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal12: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal2: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal3: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal4: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal5: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal6: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal7: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal8: TDxLabel;
    PlugMemoConfig6LabelKeyBoardNormal9: TDxLabel;
    PlugMemoConfig6Line1: TDxLine;
    PlugTabSheetConfig7: TDxTabSheet;
    PlugMemoConfigBoss: TDxChatMemo;
    PlugBtnBossAdd: TDxImageButton;
    PlugBtnBossDel: TDxImageButton;
    PlugBtnBossModify: TDxImageButton;
    PlugCheckBoxAutoDownHorse: TDxImageButton;
    PlugCheckBoxAutoLock: TDxImageButton;
    PlugCheckBoxBlacklistHit: TDxImageButton;
    PlugCheckBoxColorShow: TDxImageButton;
    PlugCheckBoxFriendHit: TDxImageButton;
    PlugCheckBoxNearHint: TDxImageButton;
    PlugComboBoxColorShow: TDxComboBox;
    PlugEditBoss: TDxEdit;
    PlugLblBoss: TDxLabel;
    PlugScrollBoxBoss: TDxChatMemo;
    PlugTabSheetConfig8: TDxTabSheet;
    PlugMemoConfig8: TDxScrollBox;
    PlugButtonGJRun: TDxImageButton;
    PlugCheckBoxAutoPickup: TDxImageButton;
    PlugCheckBoxBagFull: TDxImageButton;
    PlugCheckBoxDFAvoid: TDxImageButton;
    PlugCheckBoxLimitScreen: TDxImageButton;
    PlugCheckBoxNoBluePoison: TDxImageButton;
    PlugCheckBoxNoDuFu: TDxImageButton;
    PlugCheckBoxNoRedPoison: TDxImageButton;
    PlugCheckBoxNotRushMon: TDxImageButton;
    PlugCheckBoxPlayAttack: TDxImageButton;
    PlugComboBoxBagFullValue: TDxComboBox;
    PlugComboBoxNoBluePoisonValue: TDxComboBox;
    PlugComboBoxNoDuFuValue: TDxComboBox;
    PlugComboBoxNoRedPoisonValue: TDxComboBox;
    PlugComboBoxPlayAttackValue: TDxComboBox;
    PlugEditNotRushMonRange: TDxEdit;
    PlugLabelNotRushMon: TDxLabel;
    PlugMemoConfig8Button1: TDxImageButton;
    PlugMemoConfig8Button2: TDxImageButton;
    PlugMemoConfig8Button3: TDxImageButton;
    PlugMemoConfig8Line1: TDxLine;
    PlugMemoConfig8Line2: TDxLine;
    PlugMemoConfig8Page: TDxPageControl;
    PlugTabSheetConfig81: TDxTabSheet;
    PlugButtonMonNameAdd: TDxImageButton;
    PlugButtonMonNameDel: TDxImageButton;
    PlugButtonMonNameEdit: TDxImageButton;
    PlugEditMonName: TDxEdit;
    PlugLabelMonName: TDxLabel;
    PlugLabelShortKey: TDxLabel;
    PlugScrollBoxMons: TDxChatMemo;
    PlugTabSheetConfig82: TDxTabSheet;
    PlugMemoConfig82: TDxListView;
    PlugLabelConfig82C1: TDxLabel;
    PlugLabelConfig82C2: TDxLabel;
    PlugLineConfig82C: TDxLine;
    PlugTabSheetConfig83: TDxTabSheet;
    Line1: TDxLine;
    PlugCheckBoxGroupAttack: TDxImageButton;
    PlugEditNotGroupAttackCount: TDxEdit;
    PlugLabelGroupAttack: TDxLabel;
    PlugMemoConfig83: TDxListView;
    PlugLabelConfig83C1: TDxLabel;
    PlugLabelConfig83C2: TDxLabel;
    PlugLineConfig83C: TDxLine;
    PlugTabSheetConfig9: TDxTabSheet;
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
    procedure RefKeyBoardConfig;
    procedure DEditChange(Sender: TObject); stdcall;
    procedure DComboBoxItemStdModeSelect(Sender: TObject); stdcall;
    // BOSS变色显示事件
    procedure DComboBoxColorShow(Sender: TObject); stdcall;

    procedure DEditSearchItemChange(Sender: TObject); stdcall;
    procedure DLabelDefaultItemClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure ListViewItemClick(Sender: TObject; ARow, ACol: Integer; ListItem: TObject; ViewItem: Pointer); stdcall;

    procedure ListViewGJMagicItemClick(Sender: TObject; ARow, ACol: Integer; ListItem: TObject; ViewItem: Pointer); stdcall;


    procedure OnChanggingVolumePosition(Sender: TObject); stdcall;
    procedure OnChangedVolumePosition(Sender: TObject); stdcall;

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

    procedure PlugEditHeroDodgeHPPercentChange(Sender: TObject); stdcall;

    procedure DCheckBoxCheckDuraIsAuto(Sender: TObject; X, Y: Integer); stdcall;

    procedure DEditCheckDuraChange(Sender: TObject); stdcall;
    procedure DEditCheckDuraValueChange(Sender: TObject); stdcall;
    procedure DEditCheckDuraTimeChange(Sender: TObject); stdcall;

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

    procedure DCheckBoxAutoPercentClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DCheckBoxRenewAutoPercentClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DCheckBoxSuperMedicaPercentClick(Sender: TObject; X, Y: Integer); stdcall;

    procedure DLabelKeyBoardKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState); stdcall;
    procedure DLabelKeyBoardMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer); stdcall;

    { TODO -opiaoyun -c新增 : Boss 【2013-08-03】}
    function CheckBossNameExists(Name: string; CurIndex: Integer = -1): Boolean;
    procedure DMemoBossListClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DBtnBossAddClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DBtnBossModifyClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DBtnBossDelClick(Sender: TObject; X, Y: Integer); stdcall;

    procedure SaveOrLoadBossList(IsSave: Boolean); stdcall;
    // 特殊物品颜色 piaoyun 2013-09-09
    procedure DEditSpecialColorChange(Sender: TObject); stdcall;
    // 增加特殊物品事件 piaoyun 2013-09-09

    procedure DBtnDiyAddClick(Sender: TObject; X, Y: Integer); stdcall;

    // 删除特殊物品事件 piaoyun 2013-09-10
    procedure DBtnDiyDelClick(Sender: TObject; X, Y: Integer); stdcall;

    procedure DBtnDiyMyLoadOrSaveClick(Sender: TObject; X, Y: Integer); stdcall;

    {--------------------------------------- 挂机相关按钮 --------------------------- }

    procedure SaveOrLoadGJMonList(IsSave: Boolean); stdcall;

    procedure SaveOrLoadGJMagicList1(IsSave: Boolean); stdcall;
    procedure SaveOrLoadGJMagicList2(IsSave: Boolean); stdcall;

    // 挂机页面功能分组点击事件
    procedure DBtnGJPageControlClick(Sender: TObject; X, Y: Integer); stdcall;

    procedure DMemoGJMonListClick(Sender: TObject; X, Y: Integer); stdcall;

    function CheckGJMonNameExists(Name: string; CurIndex: Integer = -1): Boolean;

    // 挂机页面"不打怪"添加
    procedure DBtnGJMonNameAddClick(Sender: TObject; X, Y: Integer); stdcall;

    // 挂机页面"不打怪"编辑
    procedure DBtnGJMonNameEditClick(Sender: TObject; X, Y: Integer); stdcall;

    // 挂机页面"不打怪"删除
    procedure DBtnGJMonNameDelClick(Sender: TObject; X, Y: Integer); stdcall;

    procedure DEditNotRushMonRangeChange(Sender: TObject); stdcall;
    procedure ComboBoxPlayAttackValueSelect(Sender: TObject); stdcall;
    procedure DEditGroupAttackCountChanged(Sender: TObject); stdcall;

    procedure DCheckBoxGroupAttackMouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer); stdcall;

    procedure DMemoConfig8MouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer); stdcall;

    // 开始挂机/停止挂机
    procedure DBtnGJRunClick(Sender: TObject; X, Y: Integer); stdcall;

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
    procedure RefActorList; override;
    function CanFilterExp(Exp: LongWord): Boolean; override;
    function GetShowItem(const ItemName: string): pTShowItem; override;
    function FindShowItem(const ItemName: string): Boolean; override;
    function FindHintItem(const ItemName: string): Boolean; override;
    function FindPickItem(const ItemName: string): Boolean; override;
    procedure HintItem(const ItemName: string; X, Y: Integer); override;

    constructor Create();
    destructor Destroy; override;

    procedure RefreshGJMagic;
  end;

implementation
uses
  MShare, ConfigShare, IniFiles, FState, ClMain, Math, FilterItems, HUtil32, BassSound;
  
// {$R ..\..\DxComponent\MirReturnConfigDlg.res}
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

    ChkAutoPercents: array[0..4] of Boolean;
    ChkRenewAutoPercents: array[0..4] of Boolean;
    ChkSuperMedicaPercents: array[0..4] of Boolean;

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

    CheckDuraIsAutos: array[0..4] of Boolean;
    CheckDuraMin: array[0..4] of Integer;
    CheckDuraValue: array[0..4] of string[20];
    CheckDuraTime: array[0..4] of Integer;
    CheckDuraCheckTicks: array[0..4] of LongWord;

    nHeroDodgeHPPercent: Integer;
    nColorShowEff: Byte;                                                                            // BOSS变色显示 piaoyun 2013-09-09
    nSpecialColor: Byte;                                                                            // 特殊物品颜色 piaoyun 2013-09-09

    nGJPlayAttackOption: Integer;                                                                   // 挂机 - 受玩家攻击后的操作
    nGJNoRedPoisonOption: Integer;                                                                  // 挂机 - 红药用完后动作 chongchong 2014-12-06
    nGJNoBluePoisonOption: Integer;                                                                 // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
    nGJNoDuFuOption: Integer;                                                                       // 挂机 - 毒符用完后动作 chongchong 2014-12-06
    nGJBagFullOption: Integer;                                                                      // 挂机 - 包裹满后动作 chongchong 2014-12-06
    nGJNotRushMonRange: Integer;                                                                    // 挂机 - 怪物周围几格有玩家
    nGJGroupAttackCount: Integer;                                                                   // 挂机 - 当被怪物包围时的操作
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

    ChkAutoPercents: (False, False, False, False, False);
    ChkRenewAutoPercents: (False, False, False, False, False);
    ChkSuperMedicaPercents: (False, False, False, False, False);

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


    CheckDuraIsAutos: (False, False, False, False, False);
    CheckDuraMin: (2, 2, 2, 2, 2);
    CheckDuraValue: ('', '', '', '', '');
    CheckDuraTime: (10, 10, 10, 10, 10);
    CheckDuraCheckTicks: (0, 0, 0, 0, 0);

    nHeroDodgeHPPercent: 0;
    nColorShowEff: 3;                                                                               // BOSS变色显示 piaoyun 2013-09-09
    nSpecialColor: 249;

    nGJPlayAttackOption: 0;                                                                         // 挂机 - 受玩家攻击后的操作
    nGJNoRedPoisonOption: 0;                                                                        // 挂机 - 红药用完后动作 chongchong 2014-12-06
    nGJNoBluePoisonOption: 0;                                                                       // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
    nGJNoDuFuOption: 0;                                                                             // 挂机 - 毒符用完后动作 chongchong 2014-12-06
    nGJBagFullOption: 0;                                                                            // 挂机 - 包裹满后动作 chongchong 2014-12-06

    nGJNotRushMonRange: 7;                                                                          // 挂机 - 怪物周围几格有玩家
    nGJGroupAttackCount: 3;                                                                         // 挂机 - 目标周围有几个怪群攻
    );

constructor TMirReturnConfigDlg.Create();
begin
  FEnabled := False;
  FProtectEnabled := True;
  FProtectEnabledTick := MyGetTickCount;
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

  FConfigCheckeds[ckShowNpcName] := False;
  FConfigCheckeds[ckShowNpcHPLabel] := False;
  FConfigCheckeds[ckShowNGLabel] := False;

  FConfigCheckeds[ckNearHint] := True;
  FConfigCheckeds[ckAutoLock] := False;
  FConfigCheckeds[ckColorShow] := False;
  FConfigCheckeds[ckSpecialQuickFlashing] := False;
  FConfigCheckeds[ckBlacklistHit] := False;
  FConfigCheckeds[ckFriendHit] := False;
  FConfigCheckeds[ckSceneShake] := False;
  FConfigCheckeds[ckAutoDownHorse] := False;

  FConfigCheckeds[ckMovePick] := False;
end;

destructor TMirReturnConfigDlg.Destroy;
begin
  if FEnabled then
    SaveConfigFile;

  //FProtectList.Free;
  inherited;
end;

function TMirReturnConfigDlg.GetType: TConfigDlgType;
begin
  Result := ptDefault;
end;

function TMirReturnConfigDlg.GetConfigChecked(Index: TConfigChecked): Boolean;
begin
  Result := FConfigCheckeds[Index];
end;

procedure TMirReturnConfigDlg.SetConfigChecked(Index: TConfigChecked; Value: Boolean);
begin
  if FConfigCheckeds[Index] <> Value then
  begin
    FConfigCheckeds[Index] := Value;
    RefConfig;
  end;
end;

procedure TMirReturnConfigDlg.Open;
begin

end;

procedure TMirReturnConfigDlg.Close;
begin
  FLoadConfig := False;
  FEnabled := False;
  if PlugConfigDlg <> nil then
    PlugConfigDlg.Visible := False;
  FileItemDB.BackUp;
end;

function TMirReturnConfigDlg.GetVisible: Boolean;
begin
  if PlugConfigDlg <> nil then
    Result := PlugConfigDlg.Visible;
end;

procedure TMirReturnConfigDlg.SetVisible(Value: Boolean);
begin
  if PlugConfigDlg <> nil then
  begin
    PlugConfigDlg.Visible := Value;
    if PlugCheckBoxShowHPLabel.Visible then
      PlugCheckBoxShowHPLabel.SetFocus
    else if PlugCheckBoxNumberLable.Visible then
      PlugCheckBoxNumberLable.SetFocus
    else if PlugCheckBoxJobAndLevel.Visible then
      PlugCheckBoxJobAndLevel.SetFocus;

    if Value then
    begin
      RefreshGJMagic;
    end;
  end;
end;

function TMirReturnConfigDlg.GetEnabled: Boolean;
begin
  Result := FEnabled;
end;

procedure TMirReturnConfigDlg.SetEnabled(Value: Boolean);
begin
  FEnabled := Value;
  if not FEnabled then
    FLoadConfig := False;
end;

function TMirReturnConfigDlg.GetProtectEnabled: Boolean;
begin
  Result := FProtectEnabled;
end;

procedure TMirReturnConfigDlg.SetProtectEnabled(Value: Boolean);
begin
  FProtectEnabled := Value;
  if FProtectEnabled then
    FProtectEnabledTick := MyGetTickCount;
end;

function TMirReturnConfigDlg.FormKeyDown(var Key: Word; Shift: TShiftState): Boolean;
begin

end;

function TMirReturnConfigDlg.FormKeyPress(var Key: Char): Boolean;
begin

end;

procedure TMirReturnConfigDlg.RefreshMySelfAbil;
begin

end;

procedure TMirReturnConfigDlg.RefreshMyHeroAbil;
begin

end;

procedure TMirReturnConfigDlg.RefreshMySelfMagicList;
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

  RefreshGJMagic;
end;

procedure TMirReturnConfigDlg.RefreshMyHeroMagicList;
begin

end;

procedure TMirReturnConfigDlg.RefreshUnBindItemList;
begin

end;

procedure TMirReturnConfigDlg.PlugPageControlConfigInRealArea(Sender: TObject; X, Y: Integer; var IsRealArea: Boolean);
begin
  IsRealArea := not ((X >= PlugPageControlConfig.Width - 12) and (Y <= PlugPageControlConfig.Height + 20));
end;

procedure TMirReturnConfigDlg.PlugConfigDlgCloseClickEx(Sender: TObject; X, Y: Integer);
begin
  if PlugConfigDlg <> nil then
    PlugConfigDlg.Visible := False;
end;

procedure TMirReturnConfigDlg.PlugPageControlConfigActivePageChange(Sender: TObject);
begin
  if PlugPageControlConfig.ActivePageIndex = 0 then
  begin
    if PlugCheckBoxShowHPLabel.Visible then
      PlugCheckBoxShowHPLabel.SetFocus
    else if PlugCheckBoxNumberLable.Visible then
      PlugCheckBoxNumberLable.SetFocus
    else if PlugCheckBoxJobAndLevel.Visible then
      PlugCheckBoxJobAndLevel.SetFocus;
  end;
end;

procedure TMirReturnConfigDlg.Logout;
begin
  FLoadConfig := False;
  FEnabled := False;
  SaveConfigFile;
end;

procedure TMirReturnConfigDlg.LoadHelpFile;
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

procedure TMirReturnConfigDlg.ClearShowItem;
begin
  PlugMemoConfig2.Clear;
  PlugMemoConfig2.ColCount := 6;
end;

procedure TMirReturnConfigDlg.RefShowItem;
var
  I: Integer;
  ShowItem: pTShowItem;
  ListItem: TDxListItem;
  ViewItem: pTViewItem;
begin
  PlugMemoConfig2.Clear;
  PlugMemoConfig2.ColCount := 6;

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

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boShowSpecial;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boAutoMove;
    end;
  finally
    PlugMemoConfig2.UnLock;
  end;
end;

procedure TMirReturnConfigDlg.ListViewItemClick(Sender: TObject; ARow, ACol: Integer; ListItem: TObject; ViewItem: Pointer);
var
  ShowItem: pTShowItem;
begin
  ShowItem := pTViewItem(ViewItem).Data;
  if ShowItem <> nil then
  begin
    case ACol of
      0: PlugEditSpecialName.Text := ShowItem.sItemName;
      1: ShowItem.boHintMsg := pTViewItem(ViewItem).Checked;
      2: ShowItem.boPickup := pTViewItem(ViewItem).Checked;
      3: ShowItem.boShowName := pTViewItem(ViewItem).Checked;
      4: ShowItem.boShowSpecial := pTViewItem(ViewItem).Checked;
      5: ShowItem.boAutoMove := pTViewItem(ViewItem).Checked;
    end;
    FileItemDB.SaveToFile;
  end;
end;

procedure TMirReturnConfigDlg.DLabelDefaultItemClick(Sender: TObject; X, Y: Integer);
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
    PlugMemoConfig2.ColCount := 6;

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

        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        ViewItem.ImageIndex.ImageType := NewopUI_Pak;
        ViewItem.ImageIndex.Up := 228;
        ViewItem.ImageIndex.Down := 229;
        ViewItem.Checked := ShowItem.boShowSpecial;

        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        ViewItem.ImageIndex.ImageType := NewopUI_Pak;
        ViewItem.ImageIndex.Up := 228;
        ViewItem.ImageIndex.Down := 229;
        ViewItem.Checked := ShowItem.boAutoMove;
      end;
    finally
      PlugMemoConfig2.UnLock;
    end;

    List.Free;

    FileItemDB.SaveToFile;
    PlugMemoConfig2.First;
  end;
end;

procedure TMirReturnConfigDlg.DEditSearchItemChange(Sender: TObject);
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
    PlugMemoConfig2.ColCount := 6;

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

          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
          ViewItem.Checked := ShowItem.boShowSpecial;

          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
          ViewItem.Checked := ShowItem.boAutoMove;
        end;
      end;
    finally
      PlugMemoConfig2.UnLock;
    end;
    List.Free;
  end;
end;

procedure TMirReturnConfigDlg.DComboBoxItemStdModeSelect(Sender: TObject);
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
  PlugMemoConfig2.ColCount := 6;
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

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boShowSpecial;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := ShowItem.boAutoMove;
    end;
  finally
    PlugMemoConfig2.UnLock;
  end;
  List.Free;
end;

// BOSS颜色事件 piaoyun 2013-09-09

procedure TMirReturnConfigDlg.DComboBoxColorShow(Sender: TObject); stdcall;
begin
  g_Config.nColorShowEff := PlugComboBoxColorShow.ItemIndex;
  frmMain.nColorShowEff := g_Config.nColorShowEff;
end;

procedure TMirReturnConfigDlg.CheckBoxClickEx(Sender: TObject; X, Y: Integer);
var
  FileName: string;
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
  else if PlugCheckBoxShiftSwitch = Sender then
  begin
    FConfigCheckeds[ckShiftSwitch] := PlugCheckBoxShiftSwitch.Checked;
  end
  else if PlugCheckBoxExpFilter = Sender then
  begin
    FConfigCheckeds[ckFilterExp] := PlugCheckBoxExpFilter.Checked;
  end
  else if PlugCheckBoxShowMimiMapDesc = Sender then
  begin
    FConfigCheckeds[ckShowMapDesc] := PlugCheckBoxShowMimiMapDesc.Checked;
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
  else if PlugCheckBoxShowNpcName = Sender then                                                     // 显示NPC名 piaoyun 2013-07-31
  begin
    FConfigCheckeds[ckShowNpcName] := PlugCheckBoxShowNpcName.Checked;
  end
  else if PlugCheckBoxShowNpcHPLabel = Sender then                                                  // 显示NPC血条 piaoyun 2013-07-31
  begin
    FConfigCheckeds[ckShowNpcHPLabel] := PlugCheckBoxShowNpcHPLabel.Checked;
  end
  else if PlugCheckBoxShowNGLabel = Sender then                                                     // 显示NG黄条 piaoyun 2013-09-09
  begin
    FConfigCheckeds[ckShowNGLabel] := PlugCheckBoxShowNGLabel.Checked;
  end
  else if PlugCheckBoxAutoOrderItem = Sender then
  begin
    FConfigCheckeds[ckAutoOrderItem] := PlugCheckBoxAutoOrderItem.Checked;
  end
  else if PlugCheckBoxMagicLock = Sender then
  begin
    FConfigCheckeds[ckMagicLock] := PlugCheckBoxMagicLock.Checked;
  end
  else if PlugCheckBoxNotParaly = Sender then
  begin
    FConfigCheckeds[ckNotParaly] := PlugCheckBoxNotParaly.Checked;
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
  else if PlugCheckBoxSmartCRSHit = Sender then
  begin
    FConfigCheckeds[ckSmartCrsHit] := PlugCheckBoxSmartCRSHit.Checked;
  end
  else if PlugCheckBoxSmartTWNHit = Sender then
  begin
    FConfigCheckeds[ckSmartTwnHit] := PlugCheckBoxSmartTWNHit.Checked;
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
    FConfigCheckeds[ckAutoCHangePoison] := PlugCheckBoxAutoCHangePoison.Checked;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
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
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
  end
  else if PlugCheckBoxHeroContinuousNoHitMon = Sender then
  begin
    FConfigCheckeds[ckHeroContinuousNoHitMon] := PlugCheckBoxHeroContinuousNoHitMon.Checked;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
  end
  else if PlugCheckBoxAssistantHeroAutoShield = Sender then
  begin
    FConfigCheckeds[ckAssistantHeroAutoShield] := PlugCheckBoxAssistantHeroAutoShield.Checked;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
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
  else if PlugCheckBoxMovePick = Sender then
  begin
    FConfigCheckeds[ckMovePick] := PlugCheckBoxMovePick.Checked;
  end
  else if PlugCheckDisableChartMemoSize = Sender then
  begin
    FConfigCheckeds[ckDisableChartMemoSize] := PlugCheckDisableChartMemoSize.Checked;
  end
  else if PlugCheckBoxItemCmp = Sender then
  begin
    FConfigCheckeds[ckItemCompare] := PlugCheckBoxItemCmp.Checked;
  end
  else if PlugCheckBoxVolume = Sender then
  begin
    FConfigCheckeds[ckVolume] := PlugCheckBoxVolume.Checked;
    g_boSound := PlugCheckBoxVolume.Checked;

    if g_boSound then
    begin
      DScreen.AddChatBoardString('[音效 开]', clWhite, clBlack);                                   // clBlack

      // 声音开后，开地图背景 chongchong 2014-10-15
      if FileExists(g_sMapMusic) then
        PlayMp3(g_sMapMusic, True);

      // D:\热血传奇\wav\105.wav
      FileName := g_sSelfFilePath + g_SoundList[s_glass_button_click];
      if FileExists(FileName) then
      begin
        try
          g_BassSound.Play(FileName);
        except
        end;
      end;
    end
    else
    begin
      DScreen.AddChatBoardString('[音效 关]', clWhite, clBlack);
      g_PlaySound.Clear;

      // D:\热血传奇\wav\105.wav
      FileName := g_sSelfFilePath + g_SoundList[s_glass_button_click];
      if FileExists(FileName) then
      begin
        try
          g_BassSound.Play(FileName);
        except
        end;
      end;

      // 声音开后，开地图背景音乐 chongchong 2014-10-15
      if FileExists(g_sMapMusic) then
        PlayMp3(g_sMapMusic, True);
    end;
  end
  else if PlugCheckBoxBGMusic = Sender then
  begin
    FConfigCheckeds[ckBGMusic] := PlugCheckBoxBGMusic.Checked;

    if g_boBGSound <> FConfigCheckeds[ckBGMusic] then
    begin
      g_boBGSound := FConfigCheckeds[ckBGMusic];


      // 地图背景音乐也在这里控制 chongchong 2015-03-28
      if g_boBGSound then
      begin
        if FileExists(g_sMapMusic) then
          PlayMp3(g_sMapMusic, True);
      end
      else
      begin
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
  end
  else if PlugCheckBoxRepeatBGMusic = Sender then
  begin
    FConfigCheckeds[ckRepeatBGMusic] := PlugCheckBoxRepeatBGMusic.Checked;
    g_boRepeatBGSound := FConfigCheckeds[ckRepeatBGMusic];
  end
  else if PlugCheckBoxNearHint = Sender then                                                        // 接近提示 piaoyun 2013-09-09
  begin
    FConfigCheckeds[ckNearHint] := PlugCheckBoxNearHint.Checked;
  end
  else if PlugCheckBoxAutoLock = Sender then                                                        // 自动锁定 piaoyun 2013-09-09
  begin
    FConfigCheckeds[ckAutoLock] := PlugCheckBoxAutoLock.Checked;
  end
  else if PlugCheckBoxColorShow = Sender then                                                       // 变色显示 piaoyun 2013-09-09
  begin
    FConfigCheckeds[ckColorShow] := PlugCheckBoxColorShow.Checked;
  end
  else if PlugCheckBoxSpecialQuickFlashing = Sender then                                            // 特殊物品快闪 piaoyun 2013-09-10
  begin
    FConfigCheckeds[ckSpecialQuickFlashing] := PlugCheckBoxSpecialQuickFlashing.Checked;
  end
  else if PlugCheckBoxBlacklistHit = Sender then                                                    // 黑名单近身提示 piaoyun 2013-09-11
  begin
    FConfigCheckeds[ckBlacklistHit] := PlugCheckBoxBlacklistHit.Checked;
  end
  else if PlugCheckBoxFriendHit = Sender then                                                       // 好友近身提示 piaoyun 2013-09-11
  begin
    FConfigCheckeds[ckFriendHit] := PlugCheckBoxFriendHit.Checked;
  end
  else if PlugCheckBoxSceneShake = Sender then                                                      // 屏幕震动 piaoyun 2013-09-14
  begin
    FConfigCheckeds[ckSceneShake] := PlugCheckBoxSceneShake.Checked;
  end
  else if PlugCheckBoxAutoDownHorse = Sender then
  begin
    FConfigCheckeds[ckAutoDownHorse] := PlugCheckBoxAutoDownHorse.Checked;
  end
  else if PlugCheckBoxHideTitle = Sender then
  begin
    FConfigCheckeds[ckHideTitle] := PlugCheckBoxHideTitle.Checked;
  end
  else if PlugCheckBoxContinueButchItem = Sender then
  begin
    FConfigCheckeds[ckContinueButchItem] := PlugCheckBoxContinueButchItem.Checked;
  end
  {
  else if PlugCheckBoxDisableDeal = Sender then
  begin
    FConfigCheckeds[ckDisableDeal] := PlugCheckBoxDisableDeal.Checked;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
  end
  }
  else if PlugCheckBoxNoRedPoison = Sender then
  begin
    FConfigCheckeds[ckGJ_NoRedPoison] := PlugCheckBoxNoRedPoison.Checked;
  end
  else if PlugCheckBoxNoBluePoison = Sender then
  begin
    FConfigCheckeds[ckGJ_NoBluePoison] := PlugCheckBoxNoBluePoison.Checked;
  end
  else if PlugCheckBoxPlayAttack = Sender then
  begin
    FConfigCheckeds[ckGJ_PlayAttack] := PlugCheckBoxPlayAttack.Checked;
  end
  else if PlugCheckBoxNotRushMon = Sender then
  begin
    FConfigCheckeds[ckGJ_NotRushMon] := PlugCheckBoxNotRushMon.Checked;
  end
  else if PlugCheckBoxNoDuFu = Sender then
  begin
    FConfigCheckeds[ckGJ_NoDuFu] := PlugCheckBoxNoDuFu.Checked;
  end
  else if PlugCheckBoxBagFull = Sender then
  begin
    FConfigCheckeds[ckGJ_BagFull] := PlugCheckBoxBagFull.Checked;
  end
  else if PlugCheckBoxAutoPickup = Sender then
  begin
    FConfigCheckeds[ckGJ_AutoPickup] := PlugCheckBoxAutoPickup.Checked;
  end
  else if PlugCheckBoxLimitScreen = Sender then
  begin
    FConfigCheckeds[ckGJ_LimitScreen] := PlugCheckBoxLimitScreen.Checked;
  end
  else if PlugCheckBoxGroupAttack = Sender then
  begin
    FConfigCheckeds[ckGJ_GroupAttack] := PlugCheckBoxGroupAttack.Checked;
  end
  else if PlugCheckBoxDFAvoid = Sender then
  begin
    FConfigCheckeds[ckGJ_DFAvoid] := PlugCheckBoxDFAvoid.Checked;
  end
end;

procedure TMirReturnConfigDlg.RefUseItemConfigClick(Sender: TObject; X, Y: Integer);
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

procedure TMirReturnConfigDlg.RefUseItemConfig(nObj: Integer);
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

    PlugCheckBoxAutoPercent.Checked := g_Config.ChkAutoPercents[g_Config.MedicaMode];
    PlugCheckBoxRenewAutoPercent.Checked := g_Config.ChkRenewAutoPercents[g_Config.MedicaMode];
    PlugCheckBoxSuperMedicaPercent.Checked := g_Config.ChkSuperMedicaPercents[g_Config.MedicaMode];

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

    PlugCheckBoxCheckDuraIsAuto.Checked := g_Config.CheckDuraIsAutos[g_Config.MedicaMode];
    PlugEditCheckDura.Value := g_Config.CheckDuraMin[g_Config.MedicaMode];
    PlugEditCheckDuraValue.Text := g_Config.CheckDuraValue[g_Config.MedicaMode];
    PlugEditCheckDuraTime.Value := g_Config.CheckDuraTime[g_Config.MedicaMode];

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

    PlugEditHeroDodgeHPPercent.Value := g_Config.nHeroDodgeHPPercent;
  end;
end;

procedure TMirReturnConfigDlg.RefKeyBoardConfig;
begin
  PlugMemoConfig6LabelKeyBoard1.Caption := GetKeyDownStr(g_ShortcutKeys[0].Key, g_ShortcutKeys[0].Shift);
  PlugMemoConfig6LabelKeyBoard2.Caption := GetKeyDownStr(g_ShortcutKeys[1].Key, g_ShortcutKeys[1].Shift);
  PlugMemoConfig6LabelKeyBoard3.Caption := GetKeyDownStr(g_ShortcutKeys[2].Key, g_ShortcutKeys[2].Shift);
  PlugMemoConfig6LabelKeyBoard4.Caption := GetKeyDownStr(g_ShortcutKeys[3].Key, g_ShortcutKeys[3].Shift);
  PlugMemoConfig6LabelKeyBoard5.Caption := GetKeyDownStr(g_ShortcutKeys[4].Key, g_ShortcutKeys[4].Shift);
  PlugMemoConfig6LabelKeyBoard6.Caption := GetKeyDownStr(g_ShortcutKeys[5].Key, g_ShortcutKeys[5].Shift);
  PlugMemoConfig6LabelKeyBoard7.Caption := GetKeyDownStr(g_ShortcutKeys[6].Key, g_ShortcutKeys[6].Shift);
  PlugMemoConfig6LabelKeyBoard8.Caption := GetKeyDownStr(g_ShortcutKeys[7].Key, g_ShortcutKeys[7].Shift);
  PlugMemoConfig6LabelKeyBoard9.Caption := GetKeyDownStr(g_ShortcutKeys[8].Key, g_ShortcutKeys[9].Shift);
  PlugMemoConfig6LabelKeyBoard10.Caption := GetKeyDownStr(g_ShortcutKeys[9].Key, g_ShortcutKeys[9].Shift);
  PlugMemoConfig6LabelKeyBoard11.Caption := GetKeyDownStr(g_ShortcutKeys[10].Key, g_ShortcutKeys[10].Shift);
  PlugMemoConfig6LabelKeyBoard12.Caption := GetKeyDownStr(g_ShortcutKeys[11].Key, g_ShortcutKeys[11].Shift);
end;

procedure TMirReturnConfigDlg.RefConfig;
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
  PlugCheckBoxShiftSwitch.Checked := FConfigCheckeds[ckShiftSwitch];
  PlugCheckBoxExpFilter.Checked := FConfigCheckeds[ckFilterExp];
  PlugCheckBoxShowMimiMapDesc.Checked := FConfigCheckeds[ckShowMapDesc];
  PlugCheckBoxShowHighlightHPLabel.Checked := FConfigCheckeds[ckShowHighlightHPLabel];
  PlugCheckBoxShowHealthNumber.Checked := FConfigCheckeds[ckShowMoveLable];
  PlugCheckBoxNotParaly.Checked := FConfigCheckeds[ckNotParaly];

  PlugCheckBoxHideGhost.Checked := FConfigCheckeds[ckHideGhost];
  PlugCheckBoxHideHumEffect.Checked := FConfigCheckeds[ckHideHumEffect];
  PlugCheckBoxHideWeaponEffect.Checked := FConfigCheckeds[ckHideWeaponEffect];
  PlugCheckBoxShowMonName.Checked := FConfigCheckeds[ckShowMonName];
  PlugCheckBoxShowNpcName.Checked := FConfigCheckeds[ckShowNpcName];                                // 显示NPC名 piaoyun 2013-07-31
  PlugCheckBoxShowNpcHPLabel.Checked := FConfigCheckeds[ckShowNpcHPLabel];                          // 显示NPC血条 piaoyun 2013-07-31
  PlugCheckBoxShowNGLabel.Checked := FConfigCheckeds[ckShowNGLabel];                                // 显示内功黄条 piaoyun 2013-09-09
  PlugCheckBoxNearHint.Checked := FConfigCheckeds[ckNearHint];                                      // 接近提示 piaoyun 2013-09-09
  PlugCheckBoxAutoLock.Checked := FConfigCheckeds[ckAutoLock];                                      // 自动锁定 piaoyun 2013-09-09
  PlugCheckBoxColorShow.Checked := FConfigCheckeds[ckColorShow];                                    // 变色显示 piaoyun 2013-09-09
  PlugCheckBoxSpecialQuickFlashing.Checked := FConfigCheckeds[ckSpecialQuickFlashing];              // 特殊物品快闪 piaoyun 2013-09-10

  PlugCheckBoxBlacklistHit.Checked := FConfigCheckeds[ckBlacklistHit];                              // 黑名单近身提示 piaoyun 2013-09-11
  PlugCheckBoxFriendHit.Checked := FConfigCheckeds[ckFriendHit];                                    // 好友近身提示 piaoyun 2013-09-11
  PlugCheckBoxSceneShake.Checked := ConfigCheckeds[ckSceneShake];                                   // 屏幕震动 piaoyun 2013-09-14
  PlugCheckBoxAutoDownHorse.Checked := FConfigCheckeds[ckAutoDownHorse];                            // 魔法攻击自动下马 chongchong 2013-10-19

  PlugCheckBoxAutoOrderItem.Checked := FConfigCheckeds[ckAutoOrderItem];
  PlugCheckBoxMagicLock.Checked := FConfigCheckeds[ckMagicLock];

  PlugCheckBoxBGMusic.Checked := FConfigCheckeds[ckBGMusic];
  PlugCheckBoxRepeatBGMusic.Checked := FConfigCheckeds[ckRepeatBGMusic];
  PlugCheckDisableChartMemoSize.Checked := FConfigCheckeds[ckDisableChartMemoSize];
  PlugCheckBoxItemCmp.Checked := FConfigCheckeds[ckItemCompare];

  PlugCheckBoxVolume.Checked := FConfigCheckeds[ckVolume];
  g_boSound := PlugCheckBoxVolume.Checked;

  TrackBarVolume.Max := 100;
  TrackBarVolume.Min := 0;
  TrackBarVolume.Position := Round(g_SoundVolume);

  g_boBGSound := FConfigCheckeds[ckBGMusic];
  g_boRepeatBGSound := FConfigCheckeds[ckRepeatBGMusic];

  PlugCheckBoxNotParaly.Checked := FConfigCheckeds[ckNotParaly];


  PlugCheckBoxSmartLongHit.Checked := FConfigCheckeds[ckSmartLongHit];
  PlugCheckBoxSmartPosLongHit.Checked := FConfigCheckeds[ckSmartPosLongHit];
  PlugCheckBoxSmartWalkLongHit.Checked := FConfigCheckeds[ckSmartWalkLongHit];
  PlugCheckBoxSmartWideHit.Checked := FConfigCheckeds[ckSmartWideHit];
  PlugCheckBoxSmartFireHit.Checked := FConfigCheckeds[ckSmartFireHit];
  PlugCheckBoxSmartSwordHit.Checked := FConfigCheckeds[ckSmartSwordHit];
  PlugCheckBoxSmartKTZHit.Checked := FConfigCheckeds[ckSmart66Hit];
  PlugCheckBoxSmartCRSHit.Checked := FConfigCheckeds[ckSmartCrsHit];
  PlugCheckBoxSmartTWNHit.Checked := FConfigCheckeds[ckSmartTwnHit];
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

  PlugCheckBoxHeroContinuousNoHitMon.Checked := FConfigCheckeds[ckHeroContinuousNoHitMon];

  PlugCheckBoxNoRedPoison.Checked := FConfigCheckeds[ckGJ_NoRedPoison];
  PlugCheckBoxNoBluePoison.Checked := FConfigCheckeds[ckGJ_NoBluePoison];

  PlugCheckBoxHumManuallySnowWind.Checked := FConfigCheckeds[ckHumManuallySnowWind];
  PlugCheckBoxHumManuallyFireBoom.Checked := FConfigCheckeds[ckHumManuallyFireBoom];
  PlugCheckBoxHumShootLightenLockTarget.Checked := FConfigCheckeds[ckHumShootLightenLockTarget];
  PlugCheckBoxHumManuallyMeteorShower.Checked := FConfigCheckeds[ckHumManuallyMeteorShower];


  PlugCheckBoxAutoMagic.Checked := FConfigCheckeds[ckAutoUseMagic];
  PlugCheckBoxUseKeyBoard.Checked := FConfigCheckeds[ckUseKeyBoard];

  PlugCheckBoxUseSuperMedica.Checked := FConfigCheckeds[ckUseSuperMedica];

  PlugCheckBoxAutoMagic.Checked := FConfigCheckeds[ckAutoUseMagic];

  PlugCheckBoxHideTitle.Checked := FConfigCheckeds[ckHideTitle];
  PlugCheckBoxContinueButchItem.Checked := FConfigCheckeds[ckContinueButchItem];

  PlugEditExpFilter.Value := g_Config.nFilterMinExp;
  PlugEditAutoMagicTime.Value := g_Config.nAutoUseMagicTime;
  PlugComboBoxColorShow.ItemIndex := g_Config.nColorShowEff;                                        // BOSS变色显示 piaoyun 2013-09-09
  frmMain.nColorShowEff := g_Config.nColorShowEff;

  PlugEditSpecialColor.Value := g_Config.nSpecialColor;
  PlugLabelSpecialColor.CaptionColor.Up.Color := GetRGB(g_Config.nSpecialColor);
  frmMain.nSpecialColor := g_Config.nSpecialColor;

  PlugCheckBoxPlayAttack.Checked := FConfigCheckeds[ckGJ_PlayAttack];                             // 挂机 - 受玩家攻击

  PlugComboBoxPlayAttackValue.ItemIndex := g_Config.nGJPlayAttackOption;                          // 挂机 - 受玩家攻击后的操作
  FrmMain.nGJPlayAttackOption := g_Config.nGJPlayAttackOption;

  PlugComboBoxNoRedPoisonValue.ItemIndex := g_Config.nGJNoRedPoisonOption;                        // 挂机 - 红药用完后动作 chongchong 2014-12-06
  FrmMain.nGJNoRedPoisonOption := g_Config.nGJNoRedPoisonOption;

  PlugComboBoxNoBluePoisonValue.ItemIndex := g_Config.nGJNoBluePoisonOption;                      // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
  FrmMain.nGJNoBluePoisonOption := g_Config.nGJNoBluePoisonOption;

  PlugComboBoxNoDuFuValue.ItemIndex := g_Config.nGJNoDuFuOption;                                  // 挂机 - 毒符用完后动作 chongchong 2014-12-06
  FrmMain.nGJNoDuFuOption := g_Config.nGJNoDuFuOption;

  PlugComboBoxBagFullValue.ItemIndex := g_Config.nGJBagFullOption;                                // 挂机 - 包裹满后动作 chongchong 2014-12-06
  FrmMain.nGJBagFullOption := g_Config.nGJBagFullOption;

  PlugCheckBoxNotRushMon.Checked := FConfigCheckeds[ckGJ_NotRushMon];                             // 挂机 - 不抢怪
  PlugEditNotRushMonRange.Value := g_Config.nGJNotRushMonRange;                                   // 挂机 - 怪物周围几格有玩家
  FrmMain.nGJNotRushMonRange := g_Config.nGJNotRushMonRange;

  PlugCheckBoxNoDuFu.Checked := FConfigCheckeds[ckGJ_NoDuFu];
  PlugCheckBoxBagFull.Checked := FConfigCheckeds[ckGJ_BagFull];
  PlugCheckBoxAutoPickup.Checked := FConfigCheckeds[ckGJ_AutoPickup];

  PlugCheckBoxGroupAttack.Checked := FConfigCheckeds[ckGJ_GroupAttack];
  PlugEditNotGroupAttackCount.Value := g_Config.nGJGroupAttackCount;
  FrmMain.nGJGroupAttackCount := g_Config.nGJGroupAttackCount;
  PlugCheckBoxLimitScreen.Checked := FConfigCheckeds[ckGJ_LimitScreen];
  PlugCheckBoxDFAvoid.Checked := FConfigCheckeds[ckGJ_DFAvoid];

  RefKeyBoardConfig;
  PlugMemoConfig4Button1.Checked := g_Config.MedicaMode = 0;
  PlugMemoConfig4Button2.Checked := g_Config.MedicaMode = 1;
  PlugMemoConfig4Button3.Checked := g_Config.MedicaMode = 2;
  PlugMemoConfig4Button4.Checked := g_Config.MedicaMode = 3;
  PlugMemoConfig4Button5.Checked := g_Config.MedicaMode = 4;
  RefUseItemConfig(g_Config.MedicaMode);
end;

procedure TMirReturnConfigDlg.DEditCheckHPPercentChange(Sender: TObject);
begin
  g_Config.CheckHpPercents[g_Config.MedicaMode] := PlugEditCheckHPPercent.Value;
end;

procedure TMirReturnConfigDlg.PlugEditHeroDodgeHPPercentChange(Sender: TObject);
begin
  g_Config.nHeroDodgeHPPercent := PlugEditHeroDodgeHPPercent.Value;
  frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
end;

procedure TMirReturnConfigDlg.DEditCheckMPPercentChange(Sender: TObject);
begin
  g_Config.CheckMpPercents[g_Config.MedicaMode] := PlugEditCheckMPPercent.Value;
end;

procedure TMirReturnConfigDlg.ComboBoxCheckHPValueChange(Sender: TObject);
begin
  g_Config.CheckHpValues[g_Config.MedicaMode] := PlugComboBoxCheckHPValue.ItemIndex;
end;

procedure TMirReturnConfigDlg.ComboBoxCheckMPValueChange(Sender: TObject);
begin
  g_Config.CheckMpValues[g_Config.MedicaMode] := PlugComboBoxCheckMPValue.ItemIndex;
end;

procedure TMirReturnConfigDlg.DCheckBoxCheckHPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.CheckHpIsAutos[g_Config.MedicaMode] := PlugCheckBoxCheckHPIsAuto.Checked;
end;

procedure TMirReturnConfigDlg.DCheckBoxCheckMPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.CheckMpIsAutos[g_Config.MedicaMode] := PlugCheckBoxCheckMPIsAuto.Checked;
end;

procedure TMirReturnConfigDlg.DCheckBoxCheckDuraIsAuto(Sender: TObject; X, Y: Integer);
begin
  g_Config.CheckDuraIsAutos[g_Config.MedicaMode] := PlugCheckBoxCheckDuraIsAuto.Checked;
end;

procedure TMirReturnConfigDlg.DEditCheckDuraChange(Sender: TObject);
begin
  g_Config.CheckDuraMin[g_Config.MedicaMode] := Max(1, PlugEditCheckDura.Value);
  PlugEditCheckDura.Value := g_Config.CheckDuraMin[g_Config.MedicaMode];
end;

procedure TMirReturnConfigDlg.DEditCheckDuraValueChange(Sender: TObject);
begin
  g_Config.CheckDuraValue[g_Config.MedicaMode] := PlugEditCheckDuraValue.Text;
end;

procedure TMirReturnConfigDlg.DEditCheckDuraTimeChange(Sender: TObject);
begin
  g_Config.CheckDuraTime[g_Config.MedicaMode] := Max(2, PlugEditCheckDuraTime.Value);
  PlugEditCheckDuraTime.Value := g_Config.CheckDuraTime[g_Config.MedicaMode];
end;


procedure TMirReturnConfigDlg.DEditRenewHPPercentChange(Sender: TObject);
begin
  g_Config.RenewHPPercents[g_Config.MedicaMode] := PlugEditRenewHPPercent.Value;
end;

procedure TMirReturnConfigDlg.DEditRenewMPPercentChange(Sender: TObject);
begin
  g_Config.RenewMPPercents[g_Config.MedicaMode] := PlugEditRenewMPPercent.Value;
end;

procedure TMirReturnConfigDlg.DEditRenewSpecialHPPercentChange(Sender: TObject);
begin
  g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] := PlugEditRenewSpecialHPPercent.Value;
end;

procedure TMirReturnConfigDlg.DEditRenewSpecialMPPercentChange(Sender: TObject);
begin
  g_Config.RenewSpecialMPPercents[g_Config.MedicaMode] := PlugEditRenewSpecialMPPercent.Value;
end;

procedure TMirReturnConfigDlg.DEditRenewHPTimeChange(Sender: TObject);
begin
  g_Config.RenewHPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwPluginMinEatItemTime, PlugEditRenewHPTime.Value);
  PlugEditRenewHPTime.Value := g_Config.RenewHPTimes[g_Config.MedicaMode];
end;

procedure TMirReturnConfigDlg.DEditRenewMPTimeChange(Sender: TObject);
begin
  g_Config.RenewMPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwPluginMinEatItemTime, PlugEditRenewMPTime.Value);
  PlugEditRenewMPTime.Value := g_Config.RenewMPTimes[g_Config.MedicaMode];
end;

procedure TMirReturnConfigDlg.DEditRenewSpecialHPTimeChange(Sender: TObject);
begin
  g_Config.RenewSpecialHPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwPluginMinEatItemTime, PlugEditRenewSpecialHPTime.Value);
  PlugEditRenewSpecialHPTime.Value := g_Config.RenewSpecialHPTimes[g_Config.MedicaMode];
end;

procedure TMirReturnConfigDlg.DEditRenewSpecialMPTimeChange(Sender: TObject);
begin
  g_Config.RenewSpecialMPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwPluginMinEatItemTime, PlugEditRenewSpecialMPTime.Value);
  PlugEditRenewSpecialMPTime.Value := g_Config.RenewSpecialMPTimes[g_Config.MedicaMode];
end;

procedure TMirReturnConfigDlg.DCheckBoxRenewHPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.RenewHPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewHPIsAuto.Checked;
end;

procedure TMirReturnConfigDlg.DCheckBoxRenewMPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.RenewMPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewMPIsAuto.Checked;
end;

procedure TMirReturnConfigDlg.DCheckBoxRenewSpecialHPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.RenewSpecialHPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewSpecialHPIsAuto.Checked;
end;

procedure TMirReturnConfigDlg.DCheckBoxRenewSpecialMPIsAutoClick(Sender: TObject; X, Y: Integer);
begin
  g_Config.RenewSpecialMPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewSpecialMPIsAuto.Checked;
end;

procedure TMirReturnConfigDlg.DEditSuperMedicaHPChange(Sender: TObject);
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

procedure TMirReturnConfigDlg.DEditSuperMedicaHPTimeChange(Sender: TObject);
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
    g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][Index] := Max(g_ClientConfig.dwPluginMinEatItemTime, Value);
    TDxEdit(Sender).Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][Index];
  end;
end;

procedure TMirReturnConfigDlg.DEditSuperMedicaMPChange(Sender: TObject);
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

procedure TMirReturnConfigDlg.DEditSuperMedicaMPTimeChange(Sender: TObject);
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
    g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][Index] := Max(g_ClientConfig.dwPluginMinEatItemTime, Value);
    TDxEdit(Sender).Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][Index];
  end;
end;

procedure TMirReturnConfigDlg.DCheckBoxUseSuperMedicaItemNameClick(Sender: TObject; X, Y: Integer);
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

procedure TMirReturnConfigDlg.DEditChange(Sender: TObject);
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

procedure TMirReturnConfigDlg.MouseMoveEvent(Sender: TObject; Shift: TShiftState;
  X, Y: Integer);
begin
  DScreen.ClearHint;
end;

procedure TMirReturnConfigDlg.LoadClientConfig(ClientConfig: pTClientConfig);
var
  I, nTop: Integer;
  List: TList;
  DxControl: TDxImageButton;
  VisibleCount: Integer;
begin
  FClientConfig := ClientConfig^;
  PlugMemoConfig1.Position := 0;
  PlugMemoConfig2.Position := 0;
  PlugMemoConfig3.Position := 0;
  PlugMemoConfig4.Position := 0;
  PlugMemoConfig5.Position := 0;


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

  VisibleCount := 0;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    DxControl.Caption := g_Config.SuperMedicaItemNames[I];
    DxControl.Visible := g_Config.SuperMedicaItemNames[I] <> '';
    if DxControl.Visible then Inc(VisibleCount);
  end;
  //List.Free;
  // 重排下面一块的坐标------------------------------------------------------------
  nTop := PlugMemoConfig4Line4.Top;
  nTop := nTop + VisibleCount * 24 + 17;
  PlugCheckBoxAutoPercent.Top := nTop;
  PlugMemoConfig4Label1.Top := nTop + 19;
  PlugMemoConfig4Line2.Top := nTop + 29;
  PlugCheckBoxCheckHPIsAuto.Top := nTop + 46;
  PlugEditCheckHPPercent.Top := nTop + 45;
  PlugMemoConfig4Label3.Top := nTop + 47;
  PlugComboBoxCheckHPValue.Top := nTop + 44;

  PlugCheckBoxCheckMPIsAuto.Top := nTop + 71;
  PlugEditCheckMPPercent.Top := nTop + 70;
  PlugMemoConfig4Label4.Top := nTop + 72;
  PlugComboBoxCheckMPValue.Top := nTop + 69;

  PlugMemoConfig4Label5.Top := nTop + 98;
  PlugMemoConfig4Line5.Top := nTop + 108;

  PlugCheckBoxCheckDuraIsAuto.Top := nTop + 125;
  PlugEditCheckDura.Top := nTop + 122;
  PlugMemoConfig4Label7.Top := nTop + 125;
  PlugEditCheckDuraValue.Top := nTop + 122;

  PlugMemoConfig4Label6.Top := nTop + 150;
  PlugEditCheckDuraTime.Top := nTop + 146;
  PlugMemoConfig4Label8.Top := nTop + 150;

  PlugMemoConfig4LabelHint.Top := nTop + 176;
  PlugMemoConfig4LabelHint2.Top := nTop + 192;
  //---------------------------------------------------------------------------

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
  PlugTabSheetConfig9.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[8];

  //List := TList.Create;
  List.Clear;
  List.Add(PlugCheckBoxShowHPLabel);
  List.Add(PlugCheckBoxNumberLable);
  List.Add(PlugCheckBoxJobAndLevel);
  List.Add(PlugCheckBoxShowActorName);
  List.Add(PlugCheckBoxHideDescUserName);
  List.Add(PlugCheckBoxDuraWarning);
  List.Add(PlugCheckBoxNoShift);
  List.Add(PlugCheckBoxShiftSwitch);
  List.Add(PlugCheckBoxShowHighlightHPLabel);
  List.Add(PlugCheckBoxShowMimiMapDesc);
  List.Add(PlugCheckBoxNotParaly);
  List.Add(PlugCheckBoxHideTitle);
  List.Add(PlugCheckBoxContinueButchItem);

  PlugCheckBoxShowHPLabel.Visible := FClientConfig.boShowHPLabel;
  PlugCheckBoxNumberLable.Visible := FClientConfig.boShowNumberLable;
  PlugCheckBoxJobAndLevel.Visible := FClientConfig.boShowJobAndLevel;
  PlugCheckBoxShowActorName.Visible := FClientConfig.boShowUserName;
  PlugCheckBoxHideDescUserName.Visible := FClientConfig.boOnlyShowCharName;
  PlugCheckBoxDuraWarning.Visible := FClientConfig.boDuraWarning;
  PlugCheckBoxNoShift.Visible := FClientConfig.boNotNeedShift;
  PlugCheckBoxShiftSwitch.Visible := FClientConfig.boShiftSwitch;
  PlugCheckBoxShowHighlightHPLabel.Visible := FClientConfig.boShowHighlightHPLabel;
  PlugCheckBoxShowMimiMapDesc.Visible := FClientConfig.boShowMapDesc;
  PlugCheckBoxNotParaly.Visible := FClientConfig.boNotParaly;
  PlugCheckBoxHideTitle.Visible := FClientConfig.boHideTitle;
  PlugCheckBoxContinueButchItem.Visible := FClientConfig.boContinueButchItem;

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
  List.Add(PlugEditExpFilter);
  List.Add(PlugCheckBoxShowNpcName);
  List.Add(PlugCheckBoxSceneShake);
  List.Add(PlugCheckBoxMovePick);

  PlugCheckBoxShowGreenHint.Visible := FClientConfig.boShowGreenHint;
  PlugCheckBoxShowHealthNumber.Visible := FClientConfig.boShowMoveLable;
  PlugCheckBoxAutoOrderItem.Visible := FClientConfig.boAutoOrderItem;
  PlugCheckBoxMagicLock.Visible := FClientConfig.boMagicLock;
  PlugCheckBoxShowItemName.Visible := FClientConfig.boShowItemName;
  PlugCheckBoxShowFilterItem.Visible := FClientConfig.boShowFilterItem;
  PlugCheckBoxItemHint.Visible := FClientConfig.boItemHint;
  PlugCheckBoxExpFilter.Visible := FClientConfig.boFilterExp;
  PlugEditExpFilter.Visible := PlugCheckBoxExpFilter.Visible;
  PlugCheckBoxShowNpcName.Visible := FClientConfig.boShowNpcName;                                   // 显示NPC名 piaoyun 2013-07-31
  PlugCheckBoxMovePick.Visible := FClientConfig.boMovePick;
  
  PlugCheckBoxSceneShake.Visible := FClientConfig.boSceneShake;                                     // 屏幕震动控件是否可用-- 同步引擎 piaoyun 2013-09-15
  nTop := 12;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    if not DxControl.Visible then Continue;
    DxControl.Top := nTop;
    Inc(nTop, 22);
  end;

  List.Clear;
  List.Add(PlugCheckBoxAutoPickUpItem);
  List.Add(PlugCheckBoxDisableSelfStruck);
  List.Add(PlugCheckBoxSpeedSlow);
  List.Add(PlugCheckBoxBGMusic);
  List.Add(PlugCheckBoxRepeatBGMusic);
  List.Add(PlugCheckBoxHideGhost);
  List.Add(PlugCheckBoxHideHumEffect);
  List.Add(PlugCheckBoxHideWeaponEffect);
  List.Add(PlugCheckBoxShowMonName);
  List.Add(PlugCheckBoxShowNpcHPLabel);
  List.Add(PlugCheckBoxShowNGLabel);
  List.Add(PlugCheckDisableChartMemoSize);
  List.Add(PlugCheckBoxVolume);

  PlugCheckBoxAutoPickUpItem.Visible := FClientConfig.boAutoPickUpItem;
  PlugCheckBoxDisableSelfStruck.Visible := FClientConfig.boDisableSelfStruck;
  PlugCheckBoxSpeedSlow.Visible := FClientConfig.boSpeedSlow;
  PlugCheckBoxBGMusic.Visible := FClientConfig.boBGMusic;
  PlugCheckBoxRepeatBGMusic.Visible := FClientConfig.boRepeatBGMusic;
  PlugCheckBoxHideGhost.Visible := FClientConfig.boHideGhost;
  PlugCheckBoxHideHumEffect.Visible := FClientConfig.boHideHumEffect;
  PlugCheckBoxHideWeaponEffect.Visible := FClientConfig.boHideWeaponEffect;
  PlugCheckBoxShowMonName.Visible := FClientConfig.boShowMonName;
  PlugCheckBoxShowNpcHPLabel.Visible := FClientConfig.boShowNpcHPLabel;
  PlugCheckBoxShowNGLabel.Visible := FClientConfig.boShowNGLabel;                                   // 显示内功黄条 piaoyun 2013-09-09
  PlugCheckDisableChartMemoSize.Visible := FClientConfig.boDisableChartMemoSize;
  PlugCheckBoxItemCmp.Visible := FClientConfig.boItemCompare;
  PlugCheckBoxVolume.Visible := FClientConfig.boVolume;

  nTop := 12;
  for I := 0 to List.Count - 1 do
  begin
    DxControl := TDxImageButton(List.Items[I]);
    if not DxControl.Visible then Continue;
    DxControl.Top := nTop;
    Inc(nTop, 22);
  end;

  TrackBarVolume.Visible := PlugCheckBoxVolume.Visible;
  TrackBarVolume.Top := PlugCheckBoxVolume.Top + 2;

  List.Clear;
  List.Add(PlugCheckBoxSmartLongHit);
  List.Add(PlugCheckBoxSmartPosLongHit);
  List.Add(PlugCheckBoxSmartWalkLongHit);
  List.Add(PlugCheckBoxSmartWideHit);
  List.Add(PlugCheckBoxSmartFireHit);
  List.Add(PlugCheckBoxSmartSwordHit);
  List.Add(PlugCheckBoxSmartKTZHit);
  List.Add(PlugCheckBoxSmartCRSHit);
  List.Add(PlugCheckBoxSmartTWNHit);

  PlugCheckBoxSmartLongHit.Visible := FClientConfig.boSmartLongHit;
  PlugCheckBoxSmartPosLongHit.Visible := FClientConfig.boSmartPosLongHit;
  PlugCheckBoxSmartWalkLongHit.Visible := FClientConfig.boSmartWalkLongHit;
  PlugCheckBoxSmartWideHit.Visible := FClientConfig.boSmartWideHit;
  PlugCheckBoxSmartFireHit.Visible := FClientConfig.boSmartFireHit;
  PlugCheckBoxSmartSwordHit.Visible := FClientConfig.boSmartSwordHit;
  PlugCheckBoxSmartKTZHit.Visible := FClientConfig.boSmart66Hit;
  PlugCheckBoxSmartCRSHit.Visible := FClientConfig.boSmartCrsHit;
  PlugCheckBoxSmartTWNHit.Visible := FClientConfig.boSmartTwnHit;

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

procedure TMirReturnConfigDlg.Initialize(Handle: THandle;
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
        MemoryStream.LoadFromFile(g_TestModeUIPath + 'MirReturnConfigDlg.UI');
    end else begin
        MemoryStream.LoadFromFile(g_TestModeUIPath + 'MirReturnConfigDlg.GUI');
    end;
    MemoryStream.Position := 0;
    LoadControlFromMemory(@PlugConfigDlg, MemoryStream.Memory, MemoryStream.Size);
    MemoryStream.Free;
{$ELSEIF TESTMODE = 2}
    MemoryStream := TMemoryStream.Create;
    try
      Stream := TResourceStream.Create(Hinstance, 'MirReturnConfigDlg', 'GUI');

      MemoryStream.LoadFromStream(Stream);
      MemoryStream.Position := 0;
      LoadControlFromMemory(@PlugConfigDlg, MemoryStream.Memory, MemoryStream.Size);

      Stream.Free;
    finally
      MemoryStream.Free;
    end;
{$IFEND}
  end;

  LoadHelpFile;

  PlugConfigDlg.OnMouseMove := MouseMoveEvent;
  PlugPageControlConfig.OnMouseMove := MouseMoveEvent;
  PlugMemoConfig1.OnMouseMove := MouseMoveEvent;
  PlugMemoConfig3.OnMouseMove := MouseMoveEvent;
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
  PlugCheckBoxShiftSwitch.OnClick := CheckBoxClickEx;
  PlugCheckBoxExpFilter.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowMimiMapDesc.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowHighlightHPLabel.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideGhost.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowHealthNumber.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideDescUserName.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideHumEffect.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideWeaponEffect.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowMonName.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowNpcName.OnClick := CheckBoxClickEx;                                               // 显示NPC名 piaoyun 2013-07-31
  PlugCheckBoxShowNpcHPLabel.OnClick := CheckBoxClickEx;                                            // 显示NPC血条 piaoyun 2013-07-31
  PlugCheckBoxShowNGLabel.OnClick := CheckBoxClickEx;                                               // 显示内功黄条 piaoyun 2013-09-09

  PlugCheckBoxAutoOrderItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxMagicLock.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoPickUpItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxNotParaly.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideTitle.OnClick := CheckBoxClickEx;
  PlugCheckBoxContinueButchItem.OnClick := CheckBoxClickEx;

  PlugCheckBoxSmartLongHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartPosLongHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartWalkLongHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartWideHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartFireHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartSwordHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartKTZHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartCRSHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmartTWNHit.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoHideMode.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoTakeOnItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoCHangePoison.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumAutoShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumStruckShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxHeroAutoShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxAssistantHeroAutoShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallySnowWind.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyFireBoom.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumShootLightenLockTarget.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyMeteorShower.OnClick := CheckBoxClickEx;
  PlugCheckBoxHeroContinuousNoHitMon.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoMagic.OnClick := CheckBoxClickEx;
  PlugCheckBoxUseKeyBoard.OnClick := CheckBoxClickEx;

  PlugCheckBoxNearHint.OnClick := CheckBoxClickEx;                                                  // 接近提示 piaoyun 2013-09-09
  PlugCheckBoxAutoLock.OnClick := CheckBoxClickEx;                                                  // 自动锁定 piaoyun 2013-09-09
  PlugCheckBoxColorShow.OnClick := CheckBoxClickEx;                                                 // 变色显示 piaoyun 2013-09-09
  PlugCheckBoxSpecialQuickFlashing.OnClick := CheckBoxClickEx;                                      // 特殊物品快闪 piaoyun 2013-09-10
  PlugCheckBoxBlacklistHit.OnClick := CheckBoxClickEx;                                              // 黑名单近身提示 piaoyun 2013-09-11
  PlugCheckBoxFriendHit.OnClick := CheckBoxClickEx;                                                 // 好友近身提示 piaoyun 2013-09-11
  PlugCheckBoxSceneShake.OnClick := CheckBoxClickEx;                                                // 屏幕震动 piaoyun 2013-09-14
  PlugCheckBoxAutoDownHorse.OnClick := CheckBoxClickEx;                                             // 魔法攻击自动下马 chongchong 2013-10-19

  PlugCheckDisableChartMemoSize.OnClick := CheckBoxClickEx;
  PlugCheckBoxItemCmp.OnClick := CheckBoxClickEx;
  PlugCheckBoxMovePick.OnClick := CheckBoxClickEx;
  PlugCheckBoxVolume.OnClick := CheckBoxClickEx;

  TrackBarVolume.OnChanggingPosition := OnChanggingVolumePosition;
  TrackBarVolume.OnChangedPosition := OnChangedVolumePosition;

  PlugComboBoxColorShow.OnSelect := DComboBoxColorShow;

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

  PlugCheckBoxAutoPercent.OnClick := DCheckBoxAutoPercentClick;
  PlugCheckBoxRenewAutoPercent.OnClick := DCheckBoxRenewAutoPercentClick;
  PlugCheckBoxSuperMedicaPercent.OnClick := DCheckBoxSuperMedicaPercentClick;

  PlugCheckBoxCheckHPIsAuto.OnClick := DCheckBoxCheckHPIsAutoClick;
  PlugCheckBoxCheckMPIsAuto.OnClick := DCheckBoxCheckMPIsAutoClick;
  PlugEditCheckHPPercent.OnChange := DEditCheckHPPercentChange;
  PlugEditCheckMPPercent.OnChange := DEditCheckMPPercentChange;
  PlugComboBoxCheckHPValue.OnSelect := ComboBoxCheckHPValueChange;
  PlugComboBoxCheckMPValue.OnSelect := ComboBoxCheckMPValueChange;

  PlugCheckBoxCheckDuraIsAuto.OnClick := DCheckBoxCheckDuraIsAuto;
  PlugEditCheckDura.OnUnFocused := DEditCheckDuraChange;
  PlugEditCheckDuraValue.OnChange := DEditCheckDuraValueChange;
  PlugEditCheckDuraTime.OnUnFocused := DEditCheckDuraTimeChange;

  PlugComboBoxCheckHPValue.Items.Text := g_GJActionMode.Text;
  PlugComboBoxCheckMPValue.Items.Text := g_GJActionMode.Text;

  PlugCheckBoxRenewHPIsAuto.OnClick := DCheckBoxRenewHPIsAutoClick;
  PlugCheckBoxRenewMPIsAuto.OnClick := DCheckBoxRenewMPIsAutoClick;
  PlugCheckBoxRenewSpecialHPIsAuto.OnClick := DCheckBoxRenewSpecialHPIsAutoClick;
  PlugCheckBoxRenewSpecialMPIsAuto.OnClick := DCheckBoxRenewSpecialMPIsAutoClick;

  PlugEditRenewHPPercent.OnChange := DEditRenewHPPercentChange;
  PlugEditRenewMPPercent.OnChange := DEditRenewMPPercentChange;
  PlugEditRenewHPTime.OnUnFocused := DEditRenewHPTimeChange;
  PlugEditRenewMPTime.OnUnFocused := DEditRenewMPTimeChange;

  PlugEditRenewSpecialHPPercent.OnChange := DEditRenewSpecialHPPercentChange;
  PlugEditRenewSpecialMPPercent.OnChange := DEditRenewSpecialMPPercentChange;
  PlugEditRenewSpecialHPTime.OnUnFocused := DEditRenewSpecialHPTimeChange;
  PlugEditRenewSpecialMPTime.OnUnFocused := DEditRenewSpecialMPTimeChange;

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

  PlugEditSuperMedicaHPTime0.OnUnFocused := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime1.OnUnFocused := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime2.OnUnFocused := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime3.OnUnFocused := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime4.OnUnFocused := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime5.OnUnFocused := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime6.OnUnFocused := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime7.OnUnFocused := DEditSuperMedicaHPTimeChange;
  PlugEditSuperMedicaHPTime8.OnUnFocused := DEditSuperMedicaHPTimeChange;

  PlugEditSuperMedicaMP0.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP1.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP2.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP3.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP4.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP5.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP6.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP7.OnChange := DEditSuperMedicaMPChange;
  PlugEditSuperMedicaMP8.OnChange := DEditSuperMedicaMPChange;

  PlugEditSuperMedicaMPTime0.OnUnFocused := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime1.OnUnFocused := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime2.OnUnFocused := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime3.OnUnFocused := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime4.OnUnFocused := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime5.OnUnFocused := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime6.OnUnFocused := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime7.OnUnFocused := DEditSuperMedicaMPTimeChange;
  PlugEditSuperMedicaMPTime8.OnUnFocused := DEditSuperMedicaMPTimeChange;

  PlugEditHeroDodgeHPPercent.OnChange := PlugEditHeroDodgeHPPercentChange;

{-------------------------快捷键----------------------------------------}
  PlugMemoConfig6LabelKeyBoard1.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard2.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard3.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard4.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard5.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard6.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard7.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard8.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard9.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard10.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard11.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard12.OnKeyDown := DLabelKeyBoardKeyDown;

  PlugMemoConfig6LabelKeyBoard1.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard2.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard3.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard4.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard5.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard6.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard7.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard8.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard9.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard10.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard11.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard12.OnMouseDown := DLabelKeyBoardMouseDown;

  PlugMemoConfig6LabelKeyBoard1.Tag := 0;
  PlugMemoConfig6LabelKeyBoard2.Tag := 1;
  PlugMemoConfig6LabelKeyBoard3.Tag := 2;
  PlugMemoConfig6LabelKeyBoard4.Tag := 3;
  PlugMemoConfig6LabelKeyBoard5.Tag := 4;
  PlugMemoConfig6LabelKeyBoard6.Tag := 5;
  PlugMemoConfig6LabelKeyBoard7.Tag := 6;
  PlugMemoConfig6LabelKeyBoard8.Tag := 7;
  PlugMemoConfig6LabelKeyBoard9.Tag := 8;
  PlugMemoConfig6LabelKeyBoard10.Tag := 9;
  PlugMemoConfig6LabelKeyBoard11.Tag := 10;
  PlugMemoConfig6LabelKeyBoard12.Tag := 11;

  if FClientVersion < cvHero then
    PlugMemoConfig4Button1.Checked := True;

  PlugMemoConfig4Button2.Visible := FClientVersion >= cvHero;
  PlugMemoConfig4Button3.Visible := FClientVersion >= cvHero;
  PlugMemoConfig4Button4.Visible := FClientVersion >= cvHero;
  PlugMemoConfig4Button5.Visible := FClientVersion >= cvHero;

  PlugCheckBoxHeroAutoShield.Visible := FClientVersion >= cvHero;
  PlugCheckBoxAssistantHeroAutoShield.Visible := FClientVersion >= cvHero;
  PlugCheckBoxHeroContinuousNoHitMon.Visible := FClientVersion >= cvHero;

  PlugScrollBoxBoss.DrawSelect := True;
  //PlugScrollBoxBoss.AutoScroll := True;
  PlugScrollBoxBoss.ShowScroll := True;
  PlugScrollBoxBoss.ScrollBars := ssBoth;

  PlugScrollBoxBoss.OnClick := DMemoBossListClick;
  PlugBtnBossAdd.OnClick := DBtnBossAddClick;
  PlugBtnBossModify.OnClick := DBtnBossModifyClick;
  PlugBtnBossDel.OnClick := DBtnBossDelClick;

  PlugEditSpecialColor.OnChange := DEditSpecialColorChange;
  PlugBtnDiyAdd.OnClick := DBtnDiyAddClick;
  PlugBtnDiyDel.OnClick := DBtnDiyDelClick;
  PlugBtnDiyLoad.OnClick := DBtnDiyMyLoadOrSaveClick;
  PlugBtnDiySave.OnClick := DBtnDiyMyLoadOrSaveClick;

  {--------------------------------挂机相关---------------------------------------}
  PlugPageControlConfig.OnActivePageChange := PlugPageControlConfigActivePageChange;

  PlugScrollBoxMons.DrawSelect := True;
  PlugScrollBoxMons.ShowScroll := True;
  PlugScrollBoxMons.ScrollBars := ssBoth;
  PlugScrollBoxMons.OnClick := DMemoGJMonListClick;

  PlugMemoConfig8Button1.Tag := 0;
  PlugMemoConfig8Button2.Tag := 1;
  PlugMemoConfig8Button3.Tag := 2;

  PlugMemoConfig8Button1.OnClick := DBtnGJPageControlClick;
  PlugMemoConfig8Button2.OnClick := DBtnGJPageControlClick;
  PlugMemoConfig8Button3.OnClick := DBtnGJPageControlClick;

  PlugButtonMonNameAdd.OnClick := DBtnGJMonNameAddClick;
  PlugButtonMonNameEdit.OnClick := DBtnGJMonNameEditClick;
  PlugButtonMonNameDel.OnClick := DBtnGJMonNameDelClick;

  PlugCheckBoxNoRedPoison.OnClick := CheckBoxClickEx;
  PlugCheckBoxNoBluePoison.OnClick := CheckBoxClickEx;

  PlugCheckBoxPlayAttack.OnClick := CheckBoxClickEx;
  PlugCheckBoxNotRushMon.OnClick := CheckBoxClickEx;
  PlugCheckBoxNoDuFu.OnClick := CheckBoxClickEx;
  PlugCheckBoxBagFull.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoPickup.OnClick := CheckBoxClickEx;
  PlugCheckBoxLimitScreen.OnClick := CheckBoxClickEx;
  PlugCheckBoxDFAvoid.OnClick := CheckBoxClickEx;

  PlugEditNotRushMonRange.OnChange := DEditNotRushMonRangeChange;
  PlugComboBoxPlayAttackValue.OnSelect := ComboBoxPlayAttackValueSelect;
  PlugComboBoxNoRedPoisonValue.OnSelect := ComboBoxPlayAttackValueSelect;
  PlugComboBoxNoBluePoisonValue.OnSelect := ComboBoxPlayAttackValueSelect;
  PlugComboBoxNoDuFuValue.OnSelect := ComboBoxPlayAttackValueSelect;
  PlugComboBoxBagFullValue.OnSelect := ComboBoxPlayAttackValueSelect;

  PlugMemoConfig82.OnListItemClick := ListViewGJMagicItemClick;
  PlugMemoConfig83.OnListItemClick := ListViewGJMagicItemClick;

  PlugCheckBoxGroupAttack.OnClick := CheckBoxClickEx;
  PlugCheckBoxGroupAttack.OnMouseMove := DCheckBoxGroupAttackMouseMove;
  PlugEditNotGroupAttackCount.OnChange := DEditGroupAttackCountChanged;
  PlugMemoConfig8.OnMouseMove := DMemoConfig8MouseMove;
  PlugButtonGJRun.OnClick := DBtnGJRunClick;

  FInitializeed := True;
end;

procedure TMirReturnConfigDlg.Finalize;
begin
  SaveConfigFile;
  // FInitializeed := False;
end;

procedure TMirReturnConfigDlg.Logon(const ServerName: string);
begin
  g_sPlugServerName := ServerName;
end;

procedure TMirReturnConfigDlg.LoadConfig(const CharName: string);
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
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
    RefShowItem;
    FEnabled := True;
  end;
end;

procedure TMirReturnConfigDlg.Run;
begin
  AutoUseItem(Self);
end;

procedure TMirReturnConfigDlg.RefActorList;
begin

end;

function TMirReturnConfigDlg.GetShowItem(const ItemName: string): pTShowItem;
begin
  Result := FileItemDB.Find(ItemName);
end;

function TMirReturnConfigDlg.FindShowItem(const ItemName: string): Boolean;
var
  ShowItem: pTShowItem;
begin
  ShowItem := FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boShowName
  else
    Result := False;
end;

function TMirReturnConfigDlg.FindHintItem(const ItemName: string): Boolean;
var
  ShowItem: pTShowItem;
begin
  ShowItem := FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boHintMsg
  else
    Result := False;
end;

function TMirReturnConfigDlg.FindPickItem(const ItemName: string): Boolean;
var
  ShowItem: pTShowItem;
begin
  ShowItem := FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boPickup
  else
    Result := False;
end;

procedure TMirReturnConfigDlg.HintItem(const ItemName: string; X, Y: Integer);
begin
  FileItemDB.Hint(ItemName, X, Y);
end;

procedure TMirReturnConfigDlg.Struck(Actor: TObject; HP, MaxHP: LongInt);
var
  nDamage: Integer;
begin
  // 外挂吃药  物品--自动使用药品 (当HP减一定的值时，使用指定的药品) chongchong 2013-11-13
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
          begin
            DamageHPUseItem(1, nDamage);
          end;
        end;
      end;
    end;
  end;
end;

procedure TMirReturnConfigDlg.HealthChange(Actor: TObject; HP, MP, MaxHP: LongInt);
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

procedure TMirReturnConfigDlg.AutoUseItem(Sender: TObject);
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

procedure TMirReturnConfigDlg.AutoEatHPItem(Sender: TObject);
var
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  Death: Boolean;

  function EatHumHPItem(flag: Boolean): Boolean;
  var
    Value: LongWord;
  begin
    Result := False;
    if not g_Config.ChkRenewAutoPercents[0] then
      Value := Min(g_Config.RenewHPPercents[0], SelfAbil.MaxHP)
    else
      Value := Round(SelfAbil.MaxHP / 100 * Min(g_Config.RenewHPPercents[0], 99));

    if (MyGetTickCount - g_Config.RenewHPTicks[0] > g_Config.RenewHPTimes[0]) and (flag or (SelfAbil.HP < Value)) then
    begin
      nIndex := FindHumHPItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewHPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end
      else
      begin
        DScreen.AddChatBoardString('你的金创药已使用完', clWhite, clBlue);
        g_Config.RenewHPTicks[0] := MyGetTickCount;
      end;
    end;
  end;

  function EatHeroHPItem(flag: Boolean): Boolean;
  var
    Value: LongWord;
  begin
    Result := False;
    if not g_Config.ChkRenewAutoPercents[1] then
      Value := Min(g_Config.RenewHPPercents[1], HeroAbil.MaxHP)
    else
      Value := Round(HeroAbil.MaxHP / 100 * Min(g_Config.RenewHPPercents[1], 99));
    if (MyGetTickCount - g_Config.RenewHPTicks[1] > g_Config.RenewHPTimes[1]) and (flag or (HeroAbil.HP < Value)) then
    begin
      nIndex := FindHeroHPItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewHPTicks[1] := MyGetTickCount;
        frmMain.HeroEatItem(nIndex);
        Result := True;
      end
      else
      begin
        DScreen.AddChatBoardString('你英雄的金创药已使用完', clWhite, clBlue);
        g_Config.RenewHPTicks[1] := MyGetTickCount;
      end;
    end;
  end;
begin
  // 外挂吃药  物品--普通体力药 (当HP小于指定值时，自动吃药) chongchong 2013-11-13
  if (g_MySelf <> nil) then
  begin
    Death := g_MySelf.m_boDeath;
    SelfAbil := g_MySelf.m_Abil;
    if g_Config.RenewHPIsAutos[0] and (not Death) and (SelfAbil.HP > 0) then
      EatHumHPItem(False);

    if (g_MyHero <> nil) then
    begin
      Death := g_MyHero.m_boDeath;
      HeroAbil := g_MyHero.m_Abil;
      if g_Config.RenewHPIsAutos[1] and (not Death) and (HeroAbil.HP > 0) and (HeroAbil.MaxHP > 0) then
        EatHeroHPItem(False);
    end;
  end;
end;

procedure TMirReturnConfigDlg.AutoEatMPItem(Sender: TObject);
var
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  Death: Boolean;

  function EatHumMPItem(flag: Boolean): Boolean;
  var
    Value: LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[0] then
      Value := Min(g_Config.RenewMPPercents[0], SelfAbil.MaxMP)
    else
      Value := Round(SelfAbil.MaxMP / 100 * Min(g_Config.RenewMPPercents[0], 99));

    if (MyGetTickCount - g_Config.RenewMPTicks[0] > g_Config.RenewMPTimes[0]) and (flag or (SelfAbil.MP < Value)) then
    begin
      // DScreen.AddChatBoardString(Format('2 %d/%d %d/%d', [MyGetTickCount - g_Config.RenewMPTicks[0], g_Config.RenewMPTimes[0], SelfAbil.MP, g_Config.RenewMPPercents[0]]), clGreen, clWhite);
      nIndex := FindHumMPItemIndex;
      if nIndex >= 0 then
      begin
        // DScreen.AddChatBoardString(Format('3 %d/%d %d/%d', [MyGetTickCount - g_Config.RenewMPTicks[0], g_Config.RenewMPTimes[0], SelfAbil.MP, g_Config.RenewMPPercents[0]]), clGreen, clWhite);
        g_Config.RenewMPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end
      else
      begin
        DScreen.AddChatBoardString('你的魔法药已使用完', clWhite, clBlue);
        g_Config.RenewMPTicks[0] := MyGetTickCount;
      end;
    end;
  end;

  function EatHeroMPItem(flag: Boolean): Boolean;
  var
    Value: LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[1] then
      Value := Min(g_Config.RenewMPPercents[1], HeroAbil.MaxMP)
    else
      Value := Round(HeroAbil.MaxMP / 100 * Min(g_Config.RenewMPPercents[1], 99));

    if (MyGetTickCount - g_Config.RenewMPTicks[1] > g_Config.RenewMPTimes[1]) and (flag or (HeroAbil.MP < Value)) then
    begin
      nIndex := FindHeroMPItemIndex;
      if nIndex >= 0 then
      begin
        g_Config.RenewMPTicks[1] := MyGetTickCount;
        frmMain.HeroEatItem(nIndex);
        Result := True;
      end
      else
      begin
        DScreen.AddChatBoardString('你英雄的魔法药已使用完', clWhite, clBlue);
        g_Config.RenewMPTicks[1] := MyGetTickCount;
      end;
    end;
  end;
begin
  // 外挂吃药  物品--普通魔法药 (当MP小于指定值时，自动吃药) chongchong 2013-11-13
  if (g_MySelf <> nil) then
  begin
    SelfAbil := g_MySelf.m_Abil;
    Death := g_MySelf.m_boDeath;
    if g_Config.RenewMPIsAutos[0] and (not Death) and (SelfAbil.MaxMP > 0) then EatHumMPItem(False);

    if (g_MyHero <> nil) then
    begin
      HeroAbil := g_MyHero.m_Abil;
      Death := g_MyHero.m_boDeath;

      if g_Config.RenewMPIsAutos[1] and (not Death) and (HeroAbil.MaxMP > 0) then EatHeroMPItem(False);
    end;
  end;
end;

procedure TMirReturnConfigDlg.AutoEatSpecialHPItem(Sender: TObject);
var
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  Death: Boolean;

  function EatHumSpecialItem(flag: Boolean): Boolean;
  var
    Value: LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[0] then
      Value := Min(g_Config.RenewSpecialHPPercents[0], SelfAbil.MaxHP)
    else
      Value := Round(SelfAbil.MaxHP / 100 * Min(g_Config.RenewSpecialHPPercents[0], 99));

    if (MyGetTickCount - g_Config.RenewSpecialHPTicks[0] > g_Config.RenewSpecialHPTimes[0]) and (flag or (SelfAbil.HP < Value)) then
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
  var
    Value: LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[1] then
      Value := Min(g_Config.RenewSpecialHPPercents[1], HeroAbil.MaxHP)
    else
      Value := Round(HeroAbil.MaxHP / 100 * Min(g_Config.RenewSpecialHPPercents[1], 99));

    if (MyGetTickCount - g_Config.RenewSpecialHPTicks[1] > g_Config.RenewSpecialHPTimes[1]) and (flag or (HeroAbil.HP < Value)) then
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
  // 外挂吃药  物品--特殊体力药 (当HP小于指定值时，自动吃药) chongchong 2013-11-13
  if (g_MySelf <> nil) then
  begin
    SelfAbil := g_MySelf.m_Abil;
    Death := g_MySelf.m_boDeath;
    if g_Config.RenewSpecialHPIsAutos[0] and (not Death) and (SelfAbil.HP > 0) then EatHumSpecialItem(False);

    if (g_MyHero <> nil) then
    begin
      HeroAbil := g_MyHero.m_Abil;
      Death := g_MyHero.m_boDeath;
      if g_Config.RenewSpecialHPIsAutos[1] and (not Death) and (HeroAbil.HP > 0) and (HeroAbil.MaxHP > 0) then EatHeroSpecialItem(False);
    end;
  end;
end;

procedure TMirReturnConfigDlg.AutoEatSpecialMPItem(Sender: TObject);
var
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  Death: Boolean;

  function EatHumSpecialItem(flag: Boolean): Boolean;
  var
    Value: LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[0] then
      Value := Min(g_Config.RenewSpecialMPPercents[0], SelfAbil.MaxMP)
    else
      Value := Round(SelfAbil.MaxMP / 100 * Min(g_Config.RenewSpecialMPPercents[0], 99));

    if (MyGetTickCount - g_Config.RenewSpecialMPTicks[0] > g_Config.RenewSpecialMPTimes[0]) and (flag or (SelfAbil.MP < Value)) then
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
  var
    Value: LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[1] then
      Value := Min(g_Config.RenewSpecialMPPercents[1], HeroAbil.MaxMP)
    else
      Value := Round(HeroAbil.MaxMP / 100 * Min(g_Config.RenewSpecialMPPercents[1], 99));

    if (MyGetTickCount - g_Config.RenewSpecialMPTicks[1] > g_Config.RenewSpecialMPTimes[1]) and (flag or (HeroAbil.MP < Value)) then
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
  // 外挂吃药  物品--特殊魔法药 (当MP小于指定值时，自动吃药) chongchong 2013-11-13
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

procedure TMirReturnConfigDlg.AutoProtect;
const
  CM_HEROLOGON = 1050;                                                                              // 召唤英雄
var
  I, J: Integer;
  nIndex: Integer;
  SelfAbil: TAbility;
  HeroAbil: TAbility;
  SelfDeath: Boolean;
  HeroDeath: Boolean;

  Value: LongWord;
begin
  if (g_ClientConfig.boCloseLogoutProtect) and (g_ClientConfig.boCloseBookProtect) then Exit;
  if g_GJActionMode.Count > 0 then
  begin
    if (g_MySelf <> nil) then
    begin
      SelfAbil := g_MySelf.m_Abil;
      SelfDeath := g_MySelf.m_boDeath;

      // HP低于某值时执行操作
      for I := 0 to Length(g_Config.CheckHpIsAutos) - 1 do
      begin
        if I = 0 then
        begin
          if not g_Config.ChkAutoPercents[I] then
            Value := Min(g_Config.CheckHpPercents[I], SelfAbil.MaxHP)
          else
            Value := Round(SelfAbil.MaxHP / 100 * Min(g_Config.CheckHpPercents[I], 99));

          if g_Config.CheckHpIsAutos[I] and (not SelfDeath) and
            (g_Config.CheckHpValues[I] >= 0) and (g_Config.CheckHpValues[I] < g_GJActionMode.Count) and
            (SelfAbil.HP < Value) then
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

            if not g_Config.ChkAutoPercents[I] then
              Value := Min(g_Config.CheckHpPercents[I], HeroAbil.MaxHP)
            else
              Value := Round(HeroAbil.MaxHP / 100 * Min(g_Config.CheckHpPercents[I], 99));

            if g_Config.CheckHpIsAutos[I] and (not HeroDeath) and                                   // 收回英雄
              (HeroAbil.HP < Value) and (MyGetTickCount - g_dwRenewHeroLogOutTick > 5000) then
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
          if not g_Config.ChkAutoPercents[I] then
            Value := Min(g_Config.CheckMPPercents[I], SelfAbil.MaxMP)
          else
            Value := Round(SelfAbil.MP / 100 * Min(g_Config.CheckMPPercents[I], 99));

          if g_Config.CheckMPIsAutos[I] and (not SelfDeath) and
            (g_Config.CheckMPValues[I] >= 0) and (g_Config.CheckMPValues[I] < g_GJActionMode.Count) and
            (SelfAbil.MP < Value) then
          begin
            if g_Config.CheckMPValues[I] = g_GJActionMode.Count - 1 then
            begin
              if (MyGetTickCount - g_Config.CheckMPCheckTicks[I] > g_Config.CheckMPCheckTimes[I]) and (MyGetTickCount - g_dwRenewSelfLogOutTick > 30000) then
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

            if not g_Config.ChkAutoPercents[I] then
              Value := Min(g_Config.CheckMPPercents[I], HeroAbil.MaxMP)
            else
              Value := Round(HeroAbil.MaxMP / 100 * Min(g_Config.CheckMPPercents[I], 99));

            if g_Config.CheckMPIsAutos[I] and (not HeroDeath) and                                   // 收回英雄
              (HeroAbil.MP < Value) and (MyGetTickCount - g_dwRenewHeroLogOutTick > 5000) then
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

      // 持久保护
      for I := 0 to Length(g_Config.CheckDuraIsAutos) - 1 do
      begin
        Value := g_Config.CheckDuraMin[I] * 1000;

        if (Value > 0) and g_Config.CheckDuraIsAutos[I] and
          (g_Config.CheckDuraTime[I] > 0) and (Length(g_Config.CheckDuraValue[I]) > 0) and
          (MyGetTickCount - g_Config.CheckDuraCheckTicks[I] > g_Config.CheckDuraTime[I] * 1000) then
        begin
          g_Config.CheckDuraCheckTicks[I] := MyGetTickCount;

          if I = 0 then
          begin
            if (not SelfDeath) then
            begin
              nIndex := FindHumUnBindBookItemIndex(g_Config.CheckDuraValue[I]);

              if nIndex >= 0 then
              begin

                J := Low(g_UseItems);
                while J <= High(g_UseItems) do
                begin
                  if g_UseItems[J].S.Name <> '' then
                  begin
                    if g_UseItems[J].Dura < Value then
                    begin
                      frmMain.AutoEatItem(nIndex);
                      Exit;
                    end;
                  end;
                  Inc(J);
                end;

                J := Low(g_JewelryBoxItems);
                while J <=  High(g_JewelryBoxItems) do
                begin
                  if Length(g_JewelryBoxItems[J].S.Name) > 0 then
                  begin
                    if g_JewelryBoxItems[J].Dura < Value then
                    begin
                      frmMain.AutoEatItem(nIndex);
                      Exit;
                    end;
                  end;
                  Inc(J);
                end;

                J := Low(g_GodBlessItems);
                while J <= High(g_GodBlessItems) do
                begin
                  if Length(g_GodBlessItems[J].S.Name) > 0 then
                  begin
                    if g_GodBlessItems[J].Dura < Value then
                    begin
                      frmMain.AutoEatItem(nIndex);
                      Exit;
                    end;
                  end;
                  Inc(J);
                end;
              end;
            end;
          end
          else if (g_MyHero <> nil) then
          begin
            HeroDeath := g_MyHero.m_boDeath;
            if (not HeroDeath) then
            begin
              nIndex := FindHeroBagItemName(g_Config.CheckDuraValue[I]);

              if nIndex >= 0 then
              begin
                J := Low(g_HeroUseItems);
                while J <=  High(g_HeroUseItems) do
                begin
                  if Length(g_HeroUseItems[J].S.Name) > 0 then
                  begin
                    if g_HeroUseItems[J].Dura < Value then
                    begin
                      frmMain.HeroEatItem(nIndex);
                      Exit;
                    end;
                  end;
                  Inc(J);
                end;

                J := Low(g_HeroJewelryBoxItems);
                while J <=  High(g_HeroJewelryBoxItems) do
                begin
                  if Length(g_HeroJewelryBoxItems[J].S.Name) > 0 then
                  begin
                    if g_HeroJewelryBoxItems[J].Dura < Value then
                    begin
                      frmMain.HeroEatItem(nIndex);
                      Exit;
                    end;
                  end;
                  Inc(J);
                end;

                J := Low(g_HeroGodBlessItems);
                while J <= High(g_HeroGodBlessItems) do
                begin
                  if Length(g_HeroGodBlessItems[J].S.Name) > 0 then
                  begin
                    if g_HeroGodBlessItems[J].Dura < Value then
                    begin
                      frmMain.HeroEatItem(nIndex);
                      Exit;
                    end;
                  end;
                  Inc(J);
                end;
              end;
            end;
          end;
        end;
      end;
    end;
  end;
end;

procedure TMirReturnConfigDlg.DuraWarning();
var
  I: Integer;
  sHint: string;
  DuraValue: Integer;
begin
  if FConfigCheckeds[ckDuraWarning] then
  begin
    if MyGetTickCount - FHintItemDuraTick > 1000 * 20 then
    begin
      FHintItemDuraTick := MyGetTickCount;
      if (g_MySelf <> nil) then
      begin
        I := Low(g_UseItems);
        while I <= High(g_UseItems) do
        begin
          if g_UseItems[I].S.Name <> '' then
          begin
            DuraValue := Round(g_UseItems[I].Dura / 1000);

            // 传送符
            if (g_UseItems[I].s.StdMode = 25) and (g_UseItems[I].s.Shape = 6) then
              DuraValue := Round(g_UseItems[I].Dura / 100);

            // 修复神水
            if (g_UseItems[I].s.StdMode = 2) and (g_UseItems[I].s.Shape = 9) then
              DuraValue := Round(g_UseItems[I].Dura / 100);

            if g_UseItems[I].Dura <= Round(g_UseItems[I].DuraMax * 10 / 100) then
            begin
              sHint := Format('你的[%s]持久已到%d，请及时修理或更换！', [g_UseItems[I].S.Name, DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;

        I := Low(g_JewelryBoxItems);
        while I <=  High(g_JewelryBoxItems) do
        begin
          if Length(g_JewelryBoxItems[I].S.Name) > 0 then
          begin
            DuraValue := Round(g_JewelryBoxItems[I].Dura / 1000);
            if g_JewelryBoxItems[I].Dura <= Round(g_JewelryBoxItems[I].DuraMax * 10 / 100) then
            begin
              sHint := Format('你的[%s]持久已到%d，请及时修理或更换！', [g_JewelryBoxItems[I].S.Name, DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;

        I := Low(g_GodBlessItems);
        while I <= High(g_GodBlessItems) do
        begin
          if Length(g_GodBlessItems[I].S.Name) > 0 then
          begin
            DuraValue := Round(g_GodBlessItems[I].Dura / 1000);
            if g_GodBlessItems[I].Dura <= Round(g_GodBlessItems[I].DuraMax * 10 / 100) then
            begin
              sHint := Format('你的[%s]持久已到%d，请及时修理或更换！', [g_GodBlessItems[I].S.Name, DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;
      end;

      if g_MyHero <> nil then
      begin
        I := Low(g_HeroUseItems);
        while I <= High(g_HeroUseItems) do
        begin
          if g_HeroUseItems[I].S.Name <> '' then
          begin
            DuraValue := Round(g_HeroUseItems[I].Dura / 1000);

            // 传送符
            if (g_HeroUseItems[I].s.StdMode = 25) and (g_HeroUseItems[I].s.Shape = 6) then
              DuraValue := Round(g_HeroUseItems[I].Dura / 100);

            // 修复神水
            if (g_HeroUseItems[I].s.StdMode = 2) and (g_HeroUseItems[I].s.Shape = 9) then
              DuraValue := Round(g_HeroUseItems[I].Dura / 100);

            if g_HeroUseItems[I].Dura <= Round(g_HeroUseItems[I].DuraMax * 10 / 100) then
            begin
              sHint := Format('英雄的[%s]持久已到%d，请及时修理或更换！', [g_HeroUseItems[I].S.Name, DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;

        I := Low(g_HeroJewelryBoxItems);
        while I <=  High(g_HeroJewelryBoxItems) do
        begin
          if Length(g_HeroJewelryBoxItems[I].S.Name) > 0 then
          begin
            DuraValue := Round(g_HeroJewelryBoxItems[I].Dura / 1000);
            if g_HeroJewelryBoxItems[I].Dura <= Round(g_HeroJewelryBoxItems[I].DuraMax * 10 / 100) then
            begin
              sHint := Format('英雄的[%s]持久已到%d，请及时修理或更换！', [g_HeroJewelryBoxItems[I].S.Name, DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;

        I := Low(g_HeroGodBlessItems);
        while I <= High(g_HeroGodBlessItems) do
        begin
          if Length(g_HeroGodBlessItems[I].S.Name) > 0 then
          begin
            DuraValue := Round(g_HeroGodBlessItems[I].Dura / 1000);
            if g_HeroGodBlessItems[I].Dura <= Round(g_HeroGodBlessItems[I].DuraMax * 10 / 100) then
            begin
              sHint := Format('英雄的[%s]持久已到%d，请及时修理或更换！', [g_HeroGodBlessItems[I].S.Name, DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
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

procedure TMirReturnConfigDlg.DamageHPUseItem(nObj, nDamage: Integer);
var
  I, nIndex: Integer;
  sItemName: string;
  StringList: TStringList;
  MyHero: TObject;
  SelfDeath, HeroDeath: Boolean;

  Value: LongWord;
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
          MyHero := g_MyHero;
          HeroDeath := g_MyHero.m_boDeath;
        end;

        if ((nObj = 0) and (not SelfDeath)) or ((nObj > 0) and (MyHero <> nil) and (not HeroDeath)) then
        begin
          StringList := TStringList.Create;

          if nObj = 0 then
          begin
            for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do
            begin
              if not g_Config.ChkSuperMedicaPercents[nObj] then
                Value := Min(g_Config.SuperMedicaHps[nObj][I], g_MySelf.m_Abil.MaxHP)
              else
                Value := Round(g_MySelf.m_Abil.MaxHP / 100 * Min(g_Config.SuperMedicaHps[nObj][I], 99));

              StringList.AddObject(IntToStr(Value), TObject(I));
            end;
          end
          else
          begin
            for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do
            begin
              if not g_Config.ChkSuperMedicaPercents[nObj] then
                Value := Min(g_Config.SuperMedicaHps[nObj][I], g_MyHero.m_Abil.MaxHP)
              else
                Value := Round(g_MyHero.m_Abil.MaxHP / 100 * Min(g_Config.SuperMedicaHps[nObj][I], 99));

              StringList.AddObject(IntToStr(Value), TObject(I));
            end;
          end;
          StringList.CustomSort(NumberSort_1);
          for I := 0 to StringList.Count - 1 do
          begin
            nIndex := Integer(StringList.Objects[I]);
            Value := StrToIntDef(StringList.Strings[I], 0);

            if g_Config.SuperMedicaUses[nObj][nIndex] and
              (nDamage >= Value) and
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
                    end
                    else
                    begin
                      DScreen.AddChatBoardString('你的' + sItemName + '已使用完', clWhite, clBlue);
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
                    end
                    else
                    begin
                      DScreen.AddChatBoardString('你英雄的' + sItemName + '已使用完', clWhite, clBlue);
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
procedure TMirReturnConfigDlg.DamageMPUseItem(nObj, nDamage: Integer);
var
  I, nIndex: Integer;
  sItemName: string;
  StringList: TStringList;
  Value: LongWord;
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

          if nObj = 0 then
          begin
            for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do
            begin
              if not g_Config.ChkSuperMedicaPercents[nObj] then
                Value := Min(g_Config.SuperMedicaMPs[nObj][I], g_MySelf.m_Abil.MaxMP)
              else
                Value := Round(g_MySelf.m_Abil.MaxMP / 100 * Min(g_Config.SuperMedicaMPs[nObj][I], 99));
              StringList.AddObject(IntToStr(Value), TObject(I));
            end;
          end
          else
          begin
            for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do
            begin
              if not g_Config.ChkSuperMedicaPercents[nObj] then
                Value := Min(g_Config.SuperMedicaMPs[nObj][I], g_MyHero.m_Abil.MaxMP)
              else
                Value := Round(g_MyHero.m_Abil.MaxMP / 100 * Min(g_Config.SuperMedicaMPs[nObj][I], 99));
              StringList.AddObject(IntToStr(Value), TObject(I));
            end;
          end;

          StringList.CustomSort(NumberSort_1);
          for I := 0 to StringList.Count - 1 do
          begin
            nIndex := Integer(StringList.Objects[I]);
            Value := StrToIntDef(StringList.Strings[I], 0);

            if g_Config.SuperMedicaUses[nObj][nIndex] and
              (nDamage >= Value) and
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
                    end
                    else
                    begin
                      DScreen.AddChatBoardString('你的' + sItemName + '已使用完', clWhite, clBlue);
                    end;
                  end;;
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
                    end
                    else
                    begin
                      DScreen.AddChatBoardString('你英雄的' + sItemName + '已使用完', clWhite, clBlue);
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

procedure TMirReturnConfigDlg.AutoUseMagic(Sender: TObject);
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

function TMirReturnConfigDlg.CanFilterExp(Exp: LongWord): Boolean;
begin
  Result := False;
  if FConfigCheckeds[ckFilterExp] then
    Result := Exp < g_Config.nFilterMinExp;
end;

procedure TMirReturnConfigDlg.LoadConfigFile;
var
  I, II: Integer;
  nShift: Integer;
  ini: TIniFile;
  sDirectory, sFileName, sIdent, sIdent1, sIdent2, sIdent3, sIdent4, sIdent5, sIdent6: string;
  sIdent7, sIdent8, sIdent9, sIdent10, sIdent11, sIdent12, sIdent13, sIdent14, sIdent15: string;
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

    g_SoundVolume := Ini.ReadInteger('Setup', 'Volume', g_SoundVolume);

    for I := 0 to Length(FConfigCheckeds) - 1 do
    begin
      FConfigCheckeds[TConfigChecked(I)] := ini.ReadBool('Setup', Format('Checked%d', [I]), FConfigCheckeds[TConfigChecked(I)]);
    end;

    g_Config.nColorShowEff := ini.ReadInteger('Setup', 'ColorShowEff', g_Config.nColorShowEff);
    frmMain.nColorShowEff := g_Config.nColorShowEff;

    g_Config.nSpecialColor := ini.ReadInteger('Setup', 'SpecialColor', g_Config.nSpecialColor);
    frmMain.nSpecialColor := g_Config.nSpecialColor;

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

            sIdent13 := 'RenewDuraMin';
            sIdent14 := 'RenewDuraItemName';
            sIdent15 := 'RenewDuraTime';
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

            sIdent13 := 'RenewHeroDuraMin';
            sIdent14 := 'RenewHeroDuraItemName';
            sIdent15 := 'RenewHeroDuraTime';
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

            sIdent13 := 'RenewzHeroDuraMin';
            sIdent14 := 'RenewzHeroDuraItemName';
            sIdent15 := 'RenewzHeroDuraTime';
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

            sIdent13 := 'RenewfHeroDuraMin';
            sIdent14 := 'RenewfHeroDuraItemName';
            sIdent15 := 'RenewfHeroDuraTime';
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

            sIdent13 := 'RenewdHeroDuraMin';
            sIdent14 := 'RenewdHeroDuraItemName';
            sIdent15 := 'RenewdHeroDuraTime'
          end;
      end;

      g_Config.ChkAutoPercents[I] := ini.ReadBool('Protect', Format('AutoPercents%dChk', [I + 1]), g_Config.ChkAutoPercents[I]);
      g_Config.ChkRenewAutoPercents[I] := ini.ReadBool('Protect', Format('RenewAutoPercents%dChk', [I + 1]), g_Config.ChkRenewAutoPercents[I]);
      g_Config.ChkSuperMedicaPercents[I] := ini.ReadBool('Protect', Format('SuperMedicaPercents%dChk', [I + 1]), g_Config.ChkSuperMedicaPercents[I]);

      g_Config.CheckHpIsAutos[I] := ini.ReadBool('Protect', Format('Hp%dChk', [I + 1]), g_Config.CheckHpIsAutos[I]);
      g_Config.CheckHpPercents[I] := ini.ReadInteger('Protect', Format('Hp%dHp', [I + 1]), g_Config.CheckHpPercents[I]);
      g_Config.CheckHpValues[I] := ini.ReadInteger('Protect', Format('Hp%dMan', [I + 1]), g_Config.CheckHpValues[I]);
      g_Config.CheckMpIsAutos[I] := ini.ReadBool('Protect', Format('Mp%dChk', [I + 1]), g_Config.CheckMpIsAutos[I]);
      g_Config.CheckMpPercents[I] := ini.ReadInteger('Protect', Format('Mp%dHp', [I + 1]), g_Config.CheckMpPercents[I]);
      g_Config.CheckMpValues[I] := ini.ReadInteger('Protect', Format('Mp%dMan', [I + 1]), g_Config.CheckMpValues[I]);

      g_Config.RenewHPIsAutos[I] := ini.ReadBool('Protect', sIdent1, g_Config.RenewHPIsAutos[I]);
      g_Config.RenewHPTimes[I] := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', sIdent2, g_Config.RenewHPTimes[I]));
      g_Config.RenewHPPercents[I] := ini.ReadInteger('Protect', sIdent3, g_Config.RenewHPPercents[I]);

      g_Config.RenewSpecialHPIsAutos[I] := ini.ReadBool('Protect', sIdent4, g_Config.RenewSpecialHPIsAutos[I]);
      g_Config.RenewSpecialHPTimes[I] := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', sIdent5, g_Config.RenewSpecialHPTimes[I]));
      g_Config.RenewSpecialHPPercents[I] := ini.ReadInteger('Protect', sIdent6, g_Config.RenewSpecialHPPercents[I]);

      g_Config.RenewMPIsAutos[I] := ini.ReadBool('Protect', sIdent7, g_Config.RenewMPIsAutos[I]);
      g_Config.RenewMPTimes[I] := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', sIdent8, g_Config.RenewMPTimes[I]));
      g_Config.RenewMPPercents[I] := ini.ReadInteger('Protect', sIdent9, g_Config.RenewMPPercents[I]);

      g_Config.RenewSpecialMPIsAutos[I] := ini.ReadBool('Protect', sIdent10, g_Config.RenewSpecialMPIsAutos[I]);
      g_Config.RenewSpecialMPTimes[I] := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', sIdent11, g_Config.RenewSpecialMPTimes[I]));
      g_Config.RenewSpecialMPPercents[I] := ini.ReadInteger('Protect', sIdent12, g_Config.RenewSpecialMPPercents[I]);

      g_Config.CheckDuraIsAutos[I] := ini.ReadBool('Protect', Format('Dura%dChk', [I + 1]), g_Config.CheckDuraIsAutos[I]);
      g_Config.CheckDuraMin[I] := Max(Ini.ReadInteger('Protect', sIdent13, g_Config.CheckDuraMin[I]), 1);
      g_Config.CheckDuraValue[I] := Ini.ReadString('Protect', sIdent14, g_Config.CheckDuraValue[I]);
      g_Config.CheckDuraTime[I] := Max(Ini.ReadInteger('Protect', sIdent15, g_Config.CheckDuraTime[I]), 2);

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
        g_Config.SuperMedicaHPTimes[II][I] := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', Format(sIdent3, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaHPTimes[II][I]));
        g_Config.SuperMedicaMPs[II][I] := ini.ReadInteger('Protect', Format(sIdent4, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaMPs[II][I]);
        g_Config.SuperMedicaMPTimes[II][I] := Max(g_ClientConfig.dwPluginMinEatItemTime, ini.ReadInteger('Protect', Format(sIdent5, [g_Config.SuperMedicaItemNames[I]]), g_Config.SuperMedicaMPTimes[II][I]));
      end;
    end;
    for I := 0 to Length(g_ShortcutKeys) - 1 do
    begin
      g_ShortcutKeys[I].Use := ini.ReadBool('Hotkey', 'Use' + IntToStr(I), g_ShortcutKeys[I].Use);
      g_ShortcutKeys[I].Key := ini.ReadInteger('Hotkey', 'Key' + IntToStr(I), g_ShortcutKeys[I].Key);
      nShift := ini.ReadInteger('Hotkey', 'Shift' + IntToStr(I), nShift);
      Move(nShift, g_ShortcutKeys[I].Shift, SizeOf(TShiftState));
    end;

    g_Config.nHeroDodgeHPPercent := ini.ReadInteger('Protect', 'HeroDodgeHPPercent', g_Config.nHeroDodgeHPPercent);

    g_Config.nGJPlayAttackOption := ini.ReadInteger('GJ', 'GJPlayAttackOption', g_Config.nGJPlayAttackOption);          // 挂机 - 受玩家攻击后的操作
    g_Config.nGJNoRedPoisonOption := ini.ReadInteger('GJ', 'GJNoRedPoisonOption', g_Config.nGJNoRedPoisonOption);       // 挂机 - 红药用完后动作 chongchong 2014-12-06
    g_Config.nGJNoBluePoisonOption := ini.ReadInteger('GJ', 'GJNoBluePoisonOption', g_Config.nGJNoBluePoisonOption);    // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
    g_Config.nGJNoDuFuOption := ini.ReadInteger('GJ', 'GJNoDuFuOption', g_Config.nGJNoDuFuOption);                      // 挂机 - 毒符用完后动作 chongchong 2014-12-06
    g_Config.nGJBagFullOption := ini.ReadInteger('GJ', 'GJBagFullOption', g_Config.nGJBagFullOption);                   // 挂机 - 包裹满后动作 chongchong 2014-12-06

    g_Config.nGJNotRushMonRange := ini.ReadInteger('GJ', 'GJNotRushMonRange', g_Config.nGJNotRushMonRange);             // 挂机 - 怪物周围几格有玩家
    g_Config.nGJGroupAttackCount := ini.ReadInteger('GJ', 'GJGroupAttackCount', g_Config.nGJGroupAttackCount);          // 挂机 - 目标周围有几个怪视为群攻

    FrmMain.nGJPlayAttackOption  := g_Config.nGJPlayAttackOption;
    FrmMain.nGJNotRushMonRange := g_Config.nGJNotRushMonRange;
    FrmMain.nGJGroupAttackCount := g_Config.nGJGroupAttackCount;
    FrmMain.nGJNoRedPoisonOption := g_Config.nGJNoRedPoisonOption;
    FrmMain.nGJNoBluePoisonOption := g_Config.nGJNoBluePoisonOption;
    FrmMain.nGJNoDuFuOption := g_Config.nGJNoDuFuOption;
    FrmMain.nGJBagFullOption := g_Config.nGJBagFullOption;

    ini.Free;
  end;

  SaveOrLoadBossList(False);
  SaveOrLoadGJMonList(False);
  SaveOrLoadGJMagicList1(False);
  SaveOrLoadGJMagicList2(False);

  PlugComboBoxNoRedPoisonValue.Items.Text := g_GJActionMode.Text;
  PlugComboBoxNoBluePoisonValue.Items.Text := g_GJActionMode.Text;
  PlugComboBoxNoDuFuValue.Items.Text := g_GJActionMode.Text;
  PlugComboBoxBagFullValue.Items.Text := g_GJActionMode.Text;
  PlugComboBoxPlayAttackValue.Items.Text := g_GJActionMode.Text;
end;

procedure TMirReturnConfigDlg.SaveConfigFile;
var
  I, II: Integer;
  nShift: Integer;
  ini: TIniFile;
  sDirectory, sFileName, sIdent, sIdent1, sIdent2, sIdent3, sIdent4, sIdent5, sIdent6: string;
  sIdent7, sIdent8, sIdent9, sIdent10, sIdent11, sIdent12, sIdent13, sIdent14, sIdent15: string;
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
      Ini.WriteInteger('Setup', 'Volume', g_SoundVolume);

      for I := 0 to Length(FConfigCheckeds) - 1 do
      begin
        // 这4个选项会把及时雨的冲掉 chongchong 2015-04-21
        if not (TConfigChecked(I) in [ckShowRadarPlayer, ckShowRadarActor, ckShowRadarNpc, ckShowRadarAttackNpc]) then
          ini.WriteBool('Setup', Format('Checked%d', [I]), FConfigCheckeds[TConfigChecked(I)]);
      end;
      ini.WriteInteger('Setup', 'ColorShowEff', g_Config.nColorShowEff);
      ini.WriteInteger('Setup', 'SpecialColor', g_Config.nSpecialColor);

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

              sIdent13 := 'RenewDuraMin';
              sIdent14 := 'RenewDuraItemName';
              sIdent15 := 'RenewDuraTime';
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

              sIdent13 := 'RenewHeroDuraMin';
              sIdent14 := 'RenewHeroDuraItemName';
              sIdent15 := 'RenewHeroDuraTime';
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

              sIdent13 := 'RenewzHeroDuraMin';
              sIdent14 := 'RenewzHeroDuraItemName';
              sIdent15 := 'RenewzHeroDuraTime';
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

              sIdent13 := 'RenewfHeroDuraMin';
              sIdent14 := 'RenewfHeroDuraItemName';
              sIdent15 := 'RenewfHeroDuraTime';
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

              sIdent13 := 'RenewdHeroDuraMin';
              sIdent14 := 'RenewdHeroDuraItemName';
              sIdent15 := 'RenewdHeroDuraTime';
            end;
        end;

        ini.WriteBool('Protect', Format('AutoPercents%dChk', [I + 1]), g_Config.ChkAutoPercents[I]);
        ini.WriteBool('Protect', Format('RenewAutoPercents%dChk', [I + 1]), g_Config.ChkRenewAutoPercents[I]);
        ini.WriteBool('Protect', Format('SuperMedicaPercents%dChk', [I + 1]), g_Config.ChkSuperMedicaPercents[I]);

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

        ini.WriteBool('Protect', Format('Dura%dChk', [I + 1]), g_Config.CheckDuraIsAutos[I]);
        Ini.WriteInteger('Protect', sIdent13, g_Config.CheckDuraMin[I]);
        Ini.WriteString('Protect', sIdent14, g_Config.CheckDuraValue[I]);
        Ini.WriteInteger('Protect', sIdent15, g_Config.CheckDuraTime[I]);

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

      for I := 0 to Length(g_ShortcutKeys) - 1 do
      begin
        ini.WriteBool('Hotkey', 'Use' + IntToStr(I), g_ShortcutKeys[I].Use);
        ini.WriteInteger('Hotkey', 'Key' + IntToStr(I), g_ShortcutKeys[I].Key);
        Move(g_ShortcutKeys[I].Shift, nShift, SizeOf(TShiftState));
        ini.WriteInteger('Hotkey', 'Shift' + IntToStr(I), nShift);
      end;

      ini.WriteInteger('Protect', 'HeroDodgeHPPercent', g_Config.nHeroDodgeHPPercent);

      ini.WriteInteger('GJ', 'GJPlayAttackOption', g_Config.nGJPlayAttackOption);      // 挂机 - 受玩家攻击后的操作
      ini.WriteInteger('GJ', 'GJNoRedPoisonOption', g_Config.nGJNoRedPoisonOption);    // 挂机 - 红药用完后动作 chongchong 2014-12-06
      ini.WriteInteger('GJ', 'GJNoBluePoisonOption', g_Config.nGJNoBluePoisonOption);  // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
      ini.WriteInteger('GJ', 'GJNoDuFuOption', g_Config.nGJNoDuFuOption);              // 挂机 - 毒符用完后动作 chongchong 2014-12-06
      ini.WriteInteger('GJ', 'GJBagFullOption', g_Config.nGJBagFullOption);            // 挂机 - 包裹满后动作 chongchong 2014-12-06

      ini.WriteInteger('GJ', 'GJNotRushMonRange', g_Config.nGJNotRushMonRange);        // 挂机 - 怪物周围几格有玩家
      ini.WriteInteger('GJ', 'GJGroupAttackCount', g_Config.nGJGroupAttackCount);      // 挂机 - 目标周围有几个怪群攻
    except
      on E: Exception do
      begin
        DebugOutStr('[Exception] TMirReturnConfigDlg::SaveConfigFile');
        DebugOutStr(E.Message);
      end;
    end;
    ini.Free;
  end;
end;

procedure TMirReturnConfigDlg.DLabelKeyBoardKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
var
  I: Integer;
  DxLabel: TDxLabel;
begin
  DxLabel := TDxLabel(Sender);
  if (DxLabel.Tag >= 0) and (DxLabel.Tag < Length(g_ShortcutKeys)) then
  begin
    for I := 0 to Length(g_ShortcutKeys) - 1 do
    begin
      if (g_ShortcutKeys[I].Key = Key) and (g_ShortcutKeys[I].Shift = Shift) then
      begin
      	g_ShortcutKeys[I].Use := False;
        g_ShortcutKeys[I].Key := 0;
        g_ShortcutKeys[I].Shift := [];
      end;
    end;
    g_ShortcutKeys[DxLabel.Tag].Use := True;
    g_ShortcutKeys[DxLabel.Tag].Key := Key;
    g_ShortcutKeys[DxLabel.Tag].Shift := Shift;

    RefKeyBoardConfig;
    Key := 0;
  end;
end;

procedure TMirReturnConfigDlg.DLabelKeyBoardMouseDown(Sender: TObject; Button: TMouseButton;
  Shift: TShiftState; X, Y: Integer);
var
  DxLabel: TDxLabel;
begin
  if Button = mbRight then
  begin
    DxLabel := TDxLabel(Sender);
    if (DxLabel.Tag >= 0) and (DxLabel.Tag < Length(g_ShortcutKeys)) then
    begin
      g_ShortcutKeys[DxLabel.Tag].Use := False;
      g_ShortcutKeys[DxLabel.Tag].Key := 0;
      g_ShortcutKeys[DxLabel.Tag].Shift := [];
      RefKeyBoardConfig;
    end;
  end;
end;

procedure TMirReturnConfigDlg.DMemoBossListClick(Sender: TObject; X,
  Y: Integer);
begin
  //if PlugScrollBoxBoss.ScrollMouseDown(Button, X, Y) then Exit;
  if (PlugScrollBoxBoss.ItemIndex >= 0) and (PlugScrollBoxBoss.ItemIndex <= PlugScrollBoxBoss.Lines.Count - 1) then
  begin
    PlugEditBoss.Text := PlugScrollBoxBoss.Lines[PlugScrollBoxBoss.ItemIndex];
    PlugBtnBossModify.Enabled := True;
    PlugBtnBossDel.Enabled := True;
  end
  else
  begin
    PlugEditBoss.Text := '';
    PlugBtnBossModify.Enabled := False;
    PlugBtnBossDel.Enabled := False;
  end;
end;

function TMirReturnConfigDlg.CheckBossNameExists(Name: string; CurIndex: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to PlugScrollBoxBoss.Lines.Count - 1 do
  begin
    if SameText(Name, PlugScrollBoxBoss.Lines[I]) and (CurIndex <> I) then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

procedure TMirReturnConfigDlg.DBtnBossAddClick(Sender: TObject; X, Y: Integer);
var
  BossName: string;
begin
  BossName := Trim(PlugEditBoss.Text);
  if Length(BossName) = 0 then
  begin
    FrmDlg.DMessageDlg('Boss名不能为空', [mbOk]);
    Exit;
  end;
  if not CheckBossNameExists(BossName) then
  begin
    PlugScrollBoxBoss.Lines.Add(BossName);
    begin
      SaveOrLoadBossList(True);
      //PlugScrollBoxBoss.ItemIndex := PlugScrollBoxBoss.Lines.Count - 1;
    end;
  end
  else
    FrmDlg.DMessageDlg('Boss名已存在，添加失败', [mbOk]);
end;

procedure TMirReturnConfigDlg.DBtnBossDelClick(Sender: TObject; X, Y: Integer);
begin
  if (PlugScrollBoxBoss.ItemIndex >= 0) and (PlugScrollBoxBoss.ItemIndex <= PlugScrollBoxBoss.Lines.Count - 1) then
  begin
    PlugScrollBoxBoss.Lines.Delete(PlugScrollBoxBoss.ItemIndex);
    PlugEditBoss.Text := '';
    SaveOrLoadBossList(True);
  end;
end;

procedure TMirReturnConfigDlg.DBtnBossModifyClick(Sender: TObject; X,
  Y: Integer);
var
  BossName: string;
begin
  BossName := Trim(PlugEditBoss.Text);
  if Length(BossName) = 0 then
  begin
    FrmDlg.DMessageDlg('Boss名不能为空', [mbOk]);
    Exit;
  end;
  if not CheckBossNameExists(BossName, PlugScrollBoxBoss.ItemIndex) then
  begin
    PlugScrollBoxBoss.Lines[PlugScrollBoxBoss.ItemIndex] := BossName;
    PlugScrollBoxBoss.Lines := PlugScrollBoxBoss.Lines;

    SaveOrLoadBossList(True);
  end
  else
    FrmDlg.DMessageDlg('Boss名已存在，修改失败', [mbOk]);
end;

procedure TMirReturnConfigDlg.SaveOrLoadBossList(IsSave: Boolean);
var
  I: Integer;
  sDirectory, sFileName: string;
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

  sFileName := ExtractFilePath(ParamStr(0)) + Format(BOSSCONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);
  if IsSave then
  begin
    PlugScrollBoxBoss.Lines.SaveToFile(sFileName);
    //PlugScrollBoxBoss.Position := 0;
  end
  else
  begin
    PlugScrollBoxBoss.Lines.Clear;
    if FileExists(sFileName) then
      PlugScrollBoxBoss.Lines.LoadFromFile(sFileName);
    //PlugScrollBoxBoss.Position := 0;
    PlugScrollBoxBoss.ItemIndex := -1;
    PlugBtnBossModify.Enabled := False;
    PlugBtnBossDel.Enabled := False;
  end;

  g_BossList.Text := PlugScrollBoxBoss.Lines.Text;
end;

procedure TMirReturnConfigDlg.DEditSpecialColorChange(Sender: TObject);
begin
  //g_Config.nAutoUseMagicTime := PlugEditAutoMagicTime.Value;
  if PlugEditSpecialColor.Value > 255 then PlugEditSpecialColor.Value := 255;
  g_Config.nSpecialColor := PlugEditSpecialColor.Value;
  frmMain.nSpecialColor := g_Config.nSpecialColor;
  PlugLabelSpecialColor.CaptionColor.Up.Color := GetRGB(PlugEditSpecialColor.Value);
end;

procedure TMirReturnConfigDlg.DBtnDiyAddClick(Sender: TObject; X, Y: Integer);
var
  ShowItem: pTShowItem;
  sItemName: string;
  FileItem: pTShowItem;
begin
  sItemName := PlugEditSpecialName.Text;
  ShowItem := FileItemDB.Find(sItemName);
  if ShowItem = nil then
  begin
    New(ShowItem);
    ShowItem.ItemType := i_diy;                                                                     //GetItemType(sItemType);
    ShowItem.sItemType := '自定类';                                                                 //sItemType;
    ShowItem.sItemName := sItemName;
    ShowItem.boHintMsg := False;
    ShowItem.boPickup := False;
    ShowItem.boShowName := False;
    ShowItem.boShowSpecial := False;
    //m_ShowItemList.Add(ShowItem);
    FileItemDB.Add(ShowItem);
    New(FileItem);
    FileItem^ := ShowItem^;
    FileItemDB.m_FileItemList.Add(FileItem);
    //FileItemDB.SaveToFile;
    PlugComboBoxItemStdMode.ItemIndex := 8;
    DComboBoxItemStdModeSelect(Sender);
    PlugMemoConfig2.Last;
  end;
end;

procedure TMirReturnConfigDlg.DBtnDiyDelClick(Sender: TObject; X, Y: Integer);
var
  sItemName: string;
begin
  sItemName := PlugEditSpecialName.Text;
  if sItemName <> '' then
  begin
    FileItemDB.Del(sItemName);
    PlugComboBoxItemStdMode.ItemIndex := 8;
    DComboBoxItemStdModeSelect(Sender);
    PlugMemoConfig2.Last;
  end;
end;

procedure TMirReturnConfigDlg.DBtnDiyMyLoadOrSaveClick(Sender: TObject; X, Y: Integer);
var
  I: Integer;
  List: TList;

  S: string;
  ListItem: TDxListItem;
  ViewItem: pTViewItem;
  ShowItem: pTShowItem;
begin
  if Sender = PlugBtnDiyLoad then
  begin
    FileItemDB.LoadFormFile(True);

    List := TList.Create;
    FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List);
    PlugMemoConfig2.Clear;
    PlugMemoConfig2.ColCount := 6;

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

        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        ViewItem.ImageIndex.ImageType := NewopUI_Pak;
        ViewItem.ImageIndex.Up := 228;
        ViewItem.ImageIndex.Down := 229;
        ViewItem.Checked := ShowItem.boShowSpecial;

        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        ViewItem.ImageIndex.ImageType := NewopUI_Pak;
        ViewItem.ImageIndex.Up := 228;
        ViewItem.ImageIndex.Down := 229;
        ViewItem.Checked := ShowItem.boAutoMove;
      end;
    finally
      PlugMemoConfig2.UnLock;
    end;

    S := '读取上次保存的配置成功.如果没获取到请检查选择分区名是否一致.';
    DScreen.AddChatBoardString(S, GetRGB(219), clWhite);

    List.Free;
  end
  else if Sender = plugBtnDiySave then
  begin
    FileItemDB.SaveToFile(True);
    S := '保存成功，下次进入游戏本区所有玩家只需点读取即可获取本次保存的设置.';
    DScreen.AddChatBoardString(S, GetRGB(219), clWhite);
  end;
end;

procedure TMirReturnConfigDlg.DBtnGJPageControlClick(Sender: TObject; X, Y: Integer);
begin
  PlugMemoConfig8Page.ActivePageIndex := (Sender as TDxImageButton).Tag;
end;

procedure TMirReturnConfigDlg.DMemoGJMonListClick(Sender: TObject; X,
  Y: Integer);
begin
  //if PlugScrollBoxBoss.ScrollMouseDown(Button, X, Y) then Exit;
  if (PlugScrollBoxMons.ItemIndex >= 0) and (PlugScrollBoxMons.ItemIndex <= PlugScrollBoxMons.Lines.Count - 1) then
  begin
    PlugEditMonName.Text := PlugScrollBoxMons.Lines[PlugScrollBoxMons.ItemIndex];
    PlugButtonMonNameEdit.Enabled := True;
    PlugButtonMonNameDel.Enabled := True;
  end
  else
  begin
    PlugEditMonName.Text := '';
    PlugButtonMonNameEdit.Enabled := False;
    PlugButtonMonNameDel.Enabled := False;
  end;
end;
function TMirReturnConfigDlg.CheckGJMonNameExists(Name: string; CurIndex: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to PlugScrollBoxMons.Lines.Count - 1 do
  begin
    if SameText(Name, PlugScrollBoxMons.Lines[I]) and (CurIndex <> I) then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

procedure TMirReturnConfigDlg.DBtnGJMonNameAddClick(Sender: TObject; X, Y: Integer);
var
  MonName: string;
begin
  MonName := Trim(PlugEditMonName.Text);
  if Length(MonName) = 0 then
  begin
    FrmDlg.DMessageDlg('怪物名不能为空', [mbOk]);
    Exit;
  end;
  if not CheckGJMonNameExists(MonName) then
  begin
    PlugScrollBoxMons.Lines.Add(MonName);
    begin
      SaveOrLoadGJMonList(True);
      //PlugScrollBoxMons.ItemIndex := PlugScrollBoxMons.Lines.Count - 1;
    end;
  end
  else
    FrmDlg.DMessageDlg('怪物名已存在，添加失败', [mbOk]);
end;

procedure TMirReturnConfigDlg.DBtnGJMonNameDelClick(Sender: TObject; X, Y: Integer);
begin
  if (PlugScrollBoxMons.ItemIndex >= 0) and (PlugScrollBoxMons.ItemIndex <= PlugScrollBoxMons.Lines.Count - 1) then
  begin
    PlugScrollBoxMons.Lines.Delete(PlugScrollBoxMons.ItemIndex);
    PlugEditMonName.Text := '';
    SaveOrLoadGJMonList(True);
  end;
end;

procedure TMirReturnConfigDlg.DBtnGJMonNameEditClick(Sender: TObject; X,
  Y: Integer);
var
  MonName: string;
begin
  MonName := Trim(PlugEditMonName.Text);
  if Length(MonName) = 0 then
  begin
    FrmDlg.DMessageDlg('怪物名不能为空', [mbOk]);
    Exit;
  end;
  if not CheckGJMonNameExists(MonName, PlugScrollBoxMons.ItemIndex) then
  begin
    PlugScrollBoxMons.Lines[PlugScrollBoxMons.ItemIndex] := MonName;
    PlugScrollBoxMons.Lines := PlugScrollBoxMons.Lines;
    SaveOrLoadGJMonList(True);
  end
  else
    FrmDlg.DMessageDlg('怪物名已存在，修改失败', [mbOk]);
end;

procedure TMirReturnConfigDlg.SaveOrLoadGJMonList(IsSave: Boolean);
var
  I: Integer;
  sDirectory, sFileName: string;
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

  sFileName := ExtractFilePath(ParamStr(0)) + Format(GJMONCONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);
  if IsSave then
  begin
    PlugScrollBoxMons.Lines.SaveToFile(sFileName);
    //PlugScrollBoxMons.Position := 0;
  end
  else
  begin
    PlugScrollBoxMons.Lines.Clear;
    if FileExists(sFileName) then
      PlugScrollBoxMons.Lines.LoadFromFile(sFileName);
    //PlugScrollBoxMons.Position := 0;
    PlugScrollBoxMons.ItemIndex := -1;
    PlugButtonMonNameEdit.Enabled := False;
    PlugButtonMonNameDel.Enabled := False;
  end;

  g_GJMonList.Text := PlugScrollBoxMons.Lines.Text;
end;

procedure TMirReturnConfigDlg.SaveOrLoadGJMagicList1(IsSave: Boolean);
var
  I, Value: Integer;
  sDirectory, sFileName: string;
  SL: TStringList;

  ListItem: TDxListItem;
  ViewItem: pTViewItem;
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

  sFileName := ExtractFilePath(ParamStr(0)) + Format(GJNAGICCONFIGFILE1, [g_sPlugServerName, g_sPlugUserName]);
  g_GJUseMagic1.Clear;
  if IsSave then
  begin
    SL := TStringList.Create;
    try
      for I := 0 to PlugMemoConfig82.Count - 1 do
      begin
        ListItem := PlugMemoConfig82.Items[I];
        if ListItem.Count = 2 then
        begin
          ViewItem := ListItem.Items[1];
          if ViewItem.Checked then
          begin
            g_GJUseMagic1.Add(ViewItem.Data);
            SL.Add(IntToStr(Integer(ViewItem.Data)));
          end;
        end;
      end;
      SL.SaveToFile(sFileName);
    finally
      SL.Free;
    end;
  end
  else
  begin
    PlugMemoConfig82.Clear;
    if not FileExists(sFileName) then Exit;

    SL := TStringList.Create;
    try
      SL.LoadFromFile(sFileName);

      for I := 0 to SL.Count - 1 do
      begin
        Value := StrToIntDef(SL[I], 0);
        if Value <> 0 then
        begin
          g_GJUseMagic1.Add(Pointer(Value));
        end;
      end;
    finally
      SL.Free;
    end;
  end;
end;

procedure TMirReturnConfigDlg.SaveOrLoadGJMagicList2(IsSave: Boolean);
var
  I, Value: Integer;
  sDirectory, sFileName: string;
  SL: TStringList;

  ListItem: TDxListItem;
  ViewItem: pTViewItem;
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

  sFileName := ExtractFilePath(ParamStr(0)) + Format(GJNAGICCONFIGFILE2, [g_sPlugServerName, g_sPlugUserName]);
  g_GJUseMagic2.Clear;
  if IsSave then
  begin
    SL := TStringList.Create;
    try
      for I := 0 to PlugMemoConfig83.Count - 1 do
      begin
        ListItem := PlugMemoConfig83.Items[I];
        if ListItem.Count = 2 then
        begin
          ViewItem := ListItem.Items[1];
          if ViewItem.Checked then
          begin
            g_GJUseMagic2.Add(ViewItem.Data);
            SL.Add(IntToStr(Integer(ViewItem.Data)));
          end;
        end;
      end;
      SL.SaveToFile(sFileName);
    finally
      SL.Free;
    end;
  end
  else
  begin
    PlugMemoConfig83.Clear;
    if not FileExists(sFileName) then Exit;
    SL := TStringList.Create;
    try
      SL.LoadFromFile(sFileName);

      for I := 0 to SL.Count - 1 do
      begin
        Value := StrToIntDef(SL[I], 0);
        if Value <> 0 then
        begin
          g_GJUseMagic2.Add(Pointer(Value));
        end;
      end;
    finally
      SL.Free;
    end;
  end;
end;

procedure TMirReturnConfigDlg.DEditNotRushMonRangeChange(Sender: TObject);
begin
  g_Config.nGJNotRushMonRange := PlugEditNotRushMonRange.Value;
  FrmMain.nGJNotRushMonRange := g_Config.nGJNotRushMonRange;
end;

procedure TMirReturnConfigDlg.ComboBoxPlayAttackValueSelect(Sender: TObject);
begin
  if Sender = PlugComboBoxPlayAttackValue then
  begin
    g_Config.nGJPlayAttackOption := PlugComboBoxPlayAttackValue.ItemIndex;
    FrmMain.nGJPlayAttackOption := g_Config.nGJPlayAttackOption;
  end
  else if Sender = PlugComboBoxNoRedPoisonValue then
  begin
    g_Config.nGJNoRedPoisonOption := PlugComboBoxNoRedPoisonValue.ItemIndex;
    FrmMain.nGJNoRedPoisonOption := g_Config.nGJNoRedPoisonOption;
  end
  else if Sender = PlugComboBoxNoBluePoisonValue then
  begin
    g_Config.nGJNoBluePoisonOption := PlugComboBoxNoBluePoisonValue.ItemIndex;
    FrmMain.nGJNoBluePoisonOption := g_Config.nGJNoBluePoisonOption;
  end
  else if Sender = PlugComboBoxNoDuFuValue then
  begin
    g_Config.nGJNoDuFuOption := PlugComboBoxNoDuFuValue.ItemIndex;
    FrmMain.nGJNoDuFuOption := g_Config.nGJNoDuFuOption;
  end
  else if Sender = PlugComboBoxBagFullValue then
  begin
    g_Config.nGJBagFullOption := PlugComboBoxBagFullValue.ItemIndex;
    FrmMain.nGJBagFullOption := g_Config.nGJBagFullOption;
  end
end;

procedure TMirReturnConfigDlg.DCheckBoxGroupAttackMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer); stdcall;
var
  R: TRect;
begin
  DScreen.ClearHint;
  HintWindows.Clear;
  R := PlugCheckBoxGroupAttack.VirtualRect;
  HintWindows.Show(R.Left, R.Bottom + 26, '该选项用于控制是否使用群攻魔法列表勾选的技能', clWhite, True);
end;

procedure TMirReturnConfigDlg.DMemoConfig8MouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer); stdcall;
begin
  DScreen.ClearHint;
  HintWindows.Clear;
end;

procedure TMirReturnConfigDlg.DEditGroupAttackCountChanged(Sender: TObject);
begin
  g_Config.nGJGroupAttackCount := PlugEditNotGroupAttackCount.Value;
  FrmMain.nGJGroupAttackCount := g_Config.nGJGroupAttackCount;
end;

procedure TMirReturnConfigDlg.ListViewGJMagicItemClick(Sender: TObject; ARow,
  ACol: Integer; ListItem: TObject; ViewItem: Pointer);
begin
  if Sender = PlugMemoConfig82 then
    SaveOrLoadGJMagicList1(True)
  else if Sender = PlugMemoConfig83 then
    SaveOrLoadGJMagicList2(True);
end;

procedure TMirReturnConfigDlg.DBtnGJRunClick(Sender: TObject; X, Y: Integer);
begin
  if not g_boGJRun then
    FrmMain.SendStartGJ
  else
    FrmMain.SendStopGJ;
end;

procedure TMirReturnConfigDlg.RefreshGJMagic;
var
  I: Integer;
  Magic: PTClientMagic;
  ListItem: TDxListItem;
  ViewItem: pTViewItem;
begin
  PlugMemoConfig82.Clear;
  PlugMemoConfig82.Lock;
  try
    for I := 0 to g_MagicList.Count - 1 do
    begin
      Magic := g_MagicList.Items[I];

      ListItem := PlugMemoConfig82.Add;
      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Caption := Magic.Def.sMagicName;
      ViewItem.Data := Pointer(Magic.Def.wMagicId);
      ViewItem.Style := bsButton;                                                                   // bsRadio;
      ViewItem.Alignment := taLeftJustify;
      ViewItem.Color.Up.Color := clWhite;
      ViewItem.Color.Hot.Color := clRed;                                                            // clWhite;
      ViewItem.Color.Down.Color := clRed;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := Pointer(Magic.Def.wMagicId);
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := g_GJUseMagic1.IndexOf(Pointer(Magic.Def.wMagicId)) <> -1;
    end;
  finally
    PlugMemoConfig82.UnLock;
  end;

  PlugMemoConfig83.Clear;
  PlugMemoConfig83.Lock;
  try
    for I := 0 to g_MagicList.Count - 1 do
    begin
      Magic := g_MagicList.Items[I];

      ListItem := PlugMemoConfig83.Add;
      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Caption := Magic.Def.sMagicName;
      ViewItem.Data := Pointer(Magic.Def.wMagicId);
      ViewItem.Style := bsButton;                                                                   // bsRadio;
      ViewItem.Alignment := taLeftJustify;
      ViewItem.Color.Up.Color := clWhite;
      ViewItem.Color.Hot.Color := clRed;                                                            // clWhite;
      ViewItem.Color.Down.Color := clRed;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := Pointer(Magic.Def.wMagicId);
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := g_GJUseMagic2.IndexOf(Pointer(Magic.Def.wMagicId)) <> -1;;
    end;
  finally
    PlugMemoConfig83.UnLock;
  end;
end;

procedure TMirReturnConfigDlg.DCheckBoxAutoPercentClick(Sender: TObject; X,
  Y: Integer);
begin
  g_Config.ChkAutoPercents[g_Config.MedicaMode] := PlugCheckBoxAutoPercent.Checked;

  if g_Config.ChkAutoPercents[g_Config.MedicaMode] then
  begin
    if g_Config.CheckHpPercents[g_Config.MedicaMode] > 99 then
    begin
      g_Config.CheckHpPercents[g_Config.MedicaMode] := 50;
      PlugEditCheckHPPercent.Value := 50;
    end;

    if g_Config.CheckMpPercents[g_Config.MedicaMode] > 99 then
    begin
      g_Config.CheckMpPercents[g_Config.MedicaMode] := 50;
      PlugEditCheckMpPercent.Value := 50;
    end;
  end;
end;

procedure TMirReturnConfigDlg.DCheckBoxRenewAutoPercentClick(Sender: TObject; X,
  Y: Integer);
begin
  g_Config.ChkRenewAutoPercents[g_Config.MedicaMode] := PlugCheckBoxRenewAutoPercent.Checked;

  if g_Config.ChkRenewAutoPercents[g_Config.MedicaMode] then
  begin
    if g_Config.RenewHPPercents[g_Config.MedicaMode] > 99 then
    begin
      g_Config.RenewHPPercents[g_Config.MedicaMode] := 50;
      PlugEditRenewHPPercent.Value := 50;
    end;

    if g_Config.RenewMpPercents[g_Config.MedicaMode] > 99 then
    begin
      g_Config.RenewMpPercents[g_Config.MedicaMode] := 50;
      PlugEditRenewMpPercent.Value := 50;
    end;

    if g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] > 99 then
    begin
      g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] := 30;
      PlugEditRenewSpecialHPPercent.Value := 30;
    end;

    if g_Config.RenewSpecialMpPercents[g_Config.MedicaMode] > 99 then
    begin
      g_Config.RenewSpecialMpPercents[g_Config.MedicaMode] := 30;
      PlugEditRenewSpecialMpPercent.Value := 30;
    end;
  end;
end;

procedure TMirReturnConfigDlg.DCheckBoxSuperMedicaPercentClick(Sender: TObject;
  X, Y: Integer);
var
  I: Integer;
begin
  g_Config.ChkSuperMedicaPercents[g_Config.MedicaMode] := PlugCheckBoxSuperMedicaPercent.Checked;

  if g_Config.ChkSuperMedicaPercents[g_Config.MedicaMode] then
  begin
    for I := Low(g_Config.SuperMedicaHPs[g_Config.MedicaMode]) to High(g_Config.SuperMedicaHPs[g_Config.MedicaMode]) do
    begin
      if g_Config.SuperMedicaHPs[g_Config.MedicaMode][I] > 99 then
      begin
        g_Config.SuperMedicaHPs[g_Config.MedicaMode][I] := 50;
      end;
    end;

    PlugEditSuperMedicaHP1.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][0];
    PlugEditSuperMedicaHP2.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][1];
    PlugEditSuperMedicaHP3.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][2];
    PlugEditSuperMedicaHP4.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][3];
    PlugEditSuperMedicaHP5.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][4];
    PlugEditSuperMedicaHP6.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][5];
    PlugEditSuperMedicaHP7.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][6];
    PlugEditSuperMedicaHP8.Value := g_Config.SuperMedicaHPs[g_Config.MedicaMode][7];


    for I := Low(g_Config.SuperMedicaMPs[g_Config.MedicaMode]) to High(g_Config.SuperMedicaMPs[g_Config.MedicaMode]) do
    begin
      if g_Config.SuperMedicaMPs[g_Config.MedicaMode][I] > 99 then
      begin
        g_Config.SuperMedicaMPs[g_Config.MedicaMode][I] := 50;
      end;
    end;

    PlugEditSuperMedicaMP1.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][0];
    PlugEditSuperMedicaMP2.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][1];
    PlugEditSuperMedicaMP3.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][2];
    PlugEditSuperMedicaMP4.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][3];
    PlugEditSuperMedicaMP5.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][4];
    PlugEditSuperMedicaMP6.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][5];
    PlugEditSuperMedicaMP7.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][6];
    PlugEditSuperMedicaMP8.Value := g_Config.SuperMedicaMPs[g_Config.MedicaMode][7];
  end;
end;

procedure TMirReturnConfigDlg.OnChangedVolumePosition(Sender: TObject);
begin
  g_SoundVolume := TrackBarVolume.Position;
  PlugCheckBoxVolume.Caption := '音量';
end;

procedure TMirReturnConfigDlg.OnChanggingVolumePosition(Sender: TObject);
begin
  g_SoundVolume := TrackBarVolume.Position;
  PlugCheckBoxVolume.Caption := IntToStr(g_SoundVolume);
end;

end.

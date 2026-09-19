unit MirConfigDlg;

interface
uses
  Windows,
  Controls,
  SysUtils,
  StrUtils,
  Classes,
  StdCtrls,
  Dialogs,
  Graphics,
  IniFiles,
  Types,
  Grids,
  DxImageForm,
  DxImageButton,
  DxPageControl,
  DxEdit,
  DxLabel,
  DxMemo,
  DxImageGrid,
  DxPopupMenu,
  DxComboBox,
  DxControls,
  DxLine,
  DxComponents,
  Grobal2,
  GameConfigDlgs,
  GameConfigDlg,
  SDK,
  SoundUtil,
  DxTrackBar,
  HGE,
  HGECanvas,
  Forms,
  UnitDes,
  GlobalString,
  uFrmNGItemEdit;

type
TMirConfigDlg = class(TGameConfigObject)
  PlugConfigDlg:TDxImageForm;
  PlugConfigDlgClose:TDxImageButton;
  PlugPageControlConfig:TDxPageControl;
  PlugTabSheetConfig1:TDxTabSheet;
  PlugMemoConfig1:TDxScrollBox;
  PLugUIScaleLabel:TDxLabel;
  PlugCheckBoxAutoDetourPath:TDxImageButton; //HZQ 20230601 自动绕行
  PlugCheckBoxAutoOrderItem:TDxImageButton;
  PlugCheckBoxAutoPickUpItem:TDxImageButton;
  PlugCheckBoxBGMusic:TDxImageButton;
  PlugCheckBoxContinueButchItem:TDxImageButton;
  PlugCheckBoxDimFireEffect:TDxImageButton; //HZQ 20230601 火墙淡化
  PlugCheckBoxDisableDeal:TDxImageButton;
  PlugCheckBoxDuraWarning:TDxImageButton;
  PlugCheckBoxExpFilter:TDxImageButton;
  PlugCheckBoxHideActorIcons:TDxImageButton;
  PlugCheckBoxHideDescUserName:TDxImageButton;
  PlugCheckBoxHideGhost:TDxImageButton;
  PlugCheckBoxHideHumEffect:TDxImageButton;
  PlugCheckBoxHideMonsterIcons:TDxImageButton; //HZQ 20230601 隐藏怪物顶戴花翎
  PlugCheckBoxHideTitle:TDxImageButton;
  PlugCheckBoxHideWeaponEffect:TDxImageButton;
  PlugCheckBoxJobAndLevel:TDxImageButton;
  PlugCheckBoxNoShift:TDxImageButton;
  PlugCheckBoxNotParaly:TDxImageButton;
  PlugCheckBoxNumberLable:TDxImageButton;
  PlugCheckBoxRepeatBGMusic:TDxImageButton;
  PlugCheckBoxSceneShake:TDxImageButton;
  PlugCheckBoxShiftSwitch:TDxImageButton;
  PlugCheckBoxShowActorName:TDxImageButton;
  PlugCheckBoxShowGreenHint:TDxImageButton;
  PlugCheckBoxShowHPLabel:TDxImageButton;
  PlugCheckBoxShowHPUnit:TDxImageButton;
  PlugCheckBoxShowHealthNumber:TDxImageButton;
  PlugCheckBoxShowHighlightHPLabel:TDxImageButton;
  PlugCheckBoxShowMimiMapDesc:TDxImageButton;
  PlugCheckBoxShowMonName:TDxImageButton;
  PlugCheckBoxShowNGLabel:TDxImageButton;
  PlugCheckBoxShowNpcHPLabel:TDxImageButton;
  PlugCheckBoxShowNpcName:TDxImageButton;
  PlugCheckBoxSpeedSlow:TDxImageButton;
  PlugCheckBoxUpdateStatus:TDxImageButton;
  PlugCheckBoxVolume:TDxImageButton;
  PlugCheckDisableChartMemoSize:TDxImageButton;
  PlugCheckSimpleShowActor:TDxImageButton;
  PlugCheckSimpleShowBB:TDxImageButton;
  PlugCheckSimpleShowHumanDress:TDxImageButton;
  PlugCheckSimpleShowHumanWeapon:TDxImageButton;
  PlugEditExpFilter:TDxEdit;
  PlugMapScaleBar:TDxTrackBar;
  PlugMapScaleLabel:TDxLabel;
  PlugUIScaleBar:TDxTrackBar;
  TrackBarVolume:TDxTrackBar;
  PlugTabSheetConfig2:TDxTabSheet;
  PlugBtnDiyAdd:TDxImageButton;
  PlugBtnDiyDel:TDxImageButton;
  PlugBtnDiyEdit:TDxImageButton;
  PlugBtnDiyExport:TDxImageButton;
  PlugBtnDiyImport:TDxImageButton;
  PlugCheckBoxBagFastItemCmp:TDxImageButton;
  PlugCheckBoxHideItemEffect:TDxImageButton;
  PlugCheckBoxItemCmp:TDxImageButton;
  PlugCheckBoxPickAll:TDxImageButton;
  PlugCheckBoxShowValueItemEffect:TDxImageButton;
  PlugCheckBoxSpecialQuickFlashing:TDxImageButton;
  PlugComboBoxItemStdMode:TDxComboBox;
  PlugEditSearchItem:TDxEdit;
  PlugEditSpecialColor:TDxEdit;
  PlugEditSpecialName:TDxEdit;
  PlugLabelDefaultItem:TDxLabel;
  PlugLabelSpecialColor:TDxLabel;
  PlugMemoConfig2:TDxListView;
  PlugMemoConfig2Label24:TDxLabel;
  PlugMemoConfig2Label25:TDxLabel;
  PlugMemoConfig2Label26:TDxLabel;
  PlugMemoConfig2Label27:TDxLabel;
  PlugMemoConfig2Label28:TDxLabel;
  PlugMemoConfig2Label29:TDxLabel;
  PlugMemoConfig2Line1:TDxLine;
  PlugMemoConfig2Line2:TDxLine;
  PlugTabSheetConfig3:TDxTabSheet;
  PlugMemoConfig3:TDxScrollBox;
  PlugBtnUnbindItemAdd:TDxImageButton;
  PlugBtnUnbindItemDel:TDxImageButton;
  PlugBtnUnbindItemEdit:TDxImageButton;
  PlugBtnUnbindItemSave:TDxImageButton;
  PlugCheckBoxHeroContinuousNoHitMon:TDxImageButton;
  PlugCheckBoxHeroRenewAlcoholIsAuto:TDxImageButton;
  PlugCheckBoxHeroRenewMedicineAlcoholIsAuto:TDxImageButton;
  PlugCheckBoxHeroShowNumberState:TDxImageButton;
  PlugCheckBoxRenewAlcoholIsAuto:TDxImageButton;
  PlugCheckBoxRenewDeliriaIsAuto:TDxImageButton;
  PlugCheckBoxRenewMedicineAlcoholIsAuto:TDxImageButton;
  PlugComboGroupUnBindItem:TDxComboBox;
  PlugEditHeroDodgeHPPercent:TDxEdit;
  PlugEditHeroRenewAlcoholPercent:TDxEdit;
  PlugEditHeroRenewMedicineAlcoholPercent:TDxEdit;
  PlugEditItemName:TDxEdit;
  PlugEditRenewAlcoholPercent:TDxEdit;
  PlugEditRenewMedicineAlcoholPercent:TDxEdit;
  PlugEditUnbindName:TDxEdit;
  PlugLblGroupUnBindItem:TDxLabel;
  PlugLblItemName:TDxLabel;
  PlugLblUnbindName:TDxLabel;
  PlugMemoConfig3Label1:TDxLabel;
  PlugMemoConfig3Label3:TDxLabel;
  PlugMemoConfig3Label4:TDxLabel;
  PlugMemoConfig3Label5:TDxLabel;
  PlugMemoConfig3Label6:TDxLabel;
  PlugMemoConfig3Label7:TDxLabel;
  PlugMemoConfig3Label8:TDxLabel;
  PlugScrollBoxUnbindItems:TDxChatMemo;
  PlugTabSheetConfig4:TDxTabSheet;
  PlugMemoConfig4:TDxScrollBox;
  PlugCheckBoxAutoPercent:TDxImageButton;
  PlugCheckBoxCheckDuraIsAuto:TDxImageButton;
  PlugCheckBoxCheckHPIsAuto:TDxImageButton;
  PlugCheckBoxCheckMPIsAuto:TDxImageButton;
  PlugCheckBoxRenewAutoPercent:TDxImageButton;
  PlugCheckBoxRenewHPIsAuto:TDxImageButton;
  PlugCheckBoxRenewMPIsAuto:TDxImageButton;
  PlugCheckBoxRenewSpecialHPIsAuto:TDxImageButton;
  PlugCheckBoxRenewSpecialMPIsAuto:TDxImageButton;
  PlugCheckBoxSuperMedicaPercent:TDxImageButton;
  PlugCheckBoxUseSuperMedica:TDxImageButton;
  PlugCheckBoxUseSuperMedicaItemName0:TDxImageButton;
  PlugCheckBoxUseSuperMedicaItemName1:TDxImageButton;
  PlugCheckBoxUseSuperMedicaItemName2:TDxImageButton;
  PlugCheckBoxUseSuperMedicaItemName3:TDxImageButton;
  PlugCheckBoxUseSuperMedicaItemName4:TDxImageButton;
  PlugCheckBoxUseSuperMedicaItemName5:TDxImageButton;
  PlugCheckBoxUseSuperMedicaItemName6:TDxImageButton;
  PlugCheckBoxUseSuperMedicaItemName7:TDxImageButton;
  PlugCheckBoxUseSuperMedicaItemName8:TDxImageButton;
  PlugComboBoxCheckHPValue:TDxComboBox;
  PlugComboBoxCheckMPValue:TDxComboBox;
  PlugEditCheckDura:TDxEdit;
  PlugEditCheckDuraTime:TDxEdit;
  PlugEditCheckDuraValue:TDxEdit;
  PlugEditCheckHPPercent:TDxEdit;
  PlugEditCheckMPPercent:TDxEdit;
  PlugEditRenewHPPercent:TDxEdit;
  PlugEditRenewHPTime:TDxEdit;
  PlugEditRenewMPPercent:TDxEdit;
  PlugEditRenewMPTime:TDxEdit;
  PlugEditRenewSpecialHPPercent:TDxEdit;
  PlugEditRenewSpecialHPTime:TDxEdit;
  PlugEditRenewSpecialMPPercent:TDxEdit;
  PlugEditRenewSpecialMPTime:TDxEdit;
  PlugEditSuperMedicaHP0:TDxEdit;
  PlugEditSuperMedicaHP1:TDxEdit;
  PlugEditSuperMedicaHP2:TDxEdit;
  PlugEditSuperMedicaHP3:TDxEdit;
  PlugEditSuperMedicaHP4:TDxEdit;
  PlugEditSuperMedicaHP5:TDxEdit;
  PlugEditSuperMedicaHP6:TDxEdit;
  PlugEditSuperMedicaHP7:TDxEdit;
  PlugEditSuperMedicaHP8:TDxEdit;
  PlugEditSuperMedicaHPTime0:TDxEdit;
  PlugEditSuperMedicaHPTime1:TDxEdit;
  PlugEditSuperMedicaHPTime2:TDxEdit;
  PlugEditSuperMedicaHPTime3:TDxEdit;
  PlugEditSuperMedicaHPTime4:TDxEdit;
  PlugEditSuperMedicaHPTime5:TDxEdit;
  PlugEditSuperMedicaHPTime6:TDxEdit;
  PlugEditSuperMedicaHPTime7:TDxEdit;
  PlugEditSuperMedicaHPTime8:TDxEdit;
  PlugEditSuperMedicaMP0:TDxEdit;
  PlugEditSuperMedicaMP1:TDxEdit;
  PlugEditSuperMedicaMP2:TDxEdit;
  PlugEditSuperMedicaMP3:TDxEdit;
  PlugEditSuperMedicaMP4:TDxEdit;
  PlugEditSuperMedicaMP5:TDxEdit;
  PlugEditSuperMedicaMP6:TDxEdit;
  PlugEditSuperMedicaMP7:TDxEdit;
  PlugEditSuperMedicaMP8:TDxEdit;
  PlugEditSuperMedicaMPTime0:TDxEdit;
  PlugEditSuperMedicaMPTime1:TDxEdit;
  PlugEditSuperMedicaMPTime2:TDxEdit;
  PlugEditSuperMedicaMPTime3:TDxEdit;
  PlugEditSuperMedicaMPTime4:TDxEdit;
  PlugEditSuperMedicaMPTime5:TDxEdit;
  PlugEditSuperMedicaMPTime6:TDxEdit;
  PlugEditSuperMedicaMPTime7:TDxEdit;
  PlugEditSuperMedicaMPTime8:TDxEdit;
  PlugMemoConfig4Label1:TDxLabel;
  PlugMemoConfig4Label2:TDxLabel;
  PlugMemoConfig4Label3:TDxLabel;
  PlugMemoConfig4Label4:TDxLabel;
  PlugMemoConfig4Label5:TDxLabel;
  PlugMemoConfig4Label6:TDxLabel;
  PlugMemoConfig4Label7:TDxLabel;
  PlugMemoConfig4Label8:TDxLabel;
  PlugMemoConfig4LabelHint:TDxLabel;
  PlugMemoConfig4LabelHint2:TDxLabel;
  PlugMemoConfig4Line2:TDxLine;
  PlugMemoConfig4Line3:TDxLine;
  PlugMemoConfig4Line4:TDxLine;
  PlugMemoConfig4Line5:TDxLine;
  PlugMemoConfig4Button1:TDxImageButton;
  PlugMemoConfig4Button2:TDxImageButton;
  PlugMemoConfig4Button3:TDxImageButton;
  PlugMemoConfig4Button4:TDxImageButton;
  PlugMemoConfig4Button5:TDxImageButton;
  PlugMemoConfig4Line1:TDxLine;
  PlugTabSheetConfig5:TDxTabSheet;
  PlugMemoConfig5:TDxScrollBox;
  PlugCheckBoxAssistantHeroAutoShield:TDxImageButton;
  PlugCheckBoxAutoCHangePoison:TDxImageButton;
  PlugCheckBoxAutoContinueAttack:TDxImageButton;
  PlugCheckBoxAutoCustomHit1:TDxImageButton;
  PlugCheckBoxAutoCustomHit2:TDxImageButton;
  PlugCheckBoxAutoCustomHit3:TDxImageButton;
  PlugCheckBoxAutoCustomHit4:TDxImageButton;
  PlugCheckBoxAutoCustomHit5:TDxImageButton;
  PlugCheckBoxAutoCustomHit6:TDxImageButton;
  PlugCheckBoxAutoCustomHit7:TDxImageButton;
  PlugCheckBoxAutoCustomHit8:TDxImageButton;
  PlugCheckBoxAutoGroupAttack:TDxImageButton;
  PlugCheckBoxAutoGroupNoAttackMon:TDxImageButton;
  PlugCheckBoxAutoHideMode:TDxImageButton;
  PlugCheckBoxAutoMagic:TDxImageButton;
  PlugCheckBoxAutoOpenSpell:TDxImageButton;
  PlugCheckBoxHeroAutoShield:TDxImageButton;
  PlugCheckBoxHumAutoShield:TDxImageButton;
  PlugCheckBoxHumManuallyCustomHit1:TDxImageButton;
  PlugCheckBoxHumManuallyCustomHit2:TDxImageButton;
  PlugCheckBoxHumManuallyCustomHit3:TDxImageButton;
  PlugCheckBoxHumManuallyCustomHit4:TDxImageButton;
  PlugCheckBoxHumManuallyCustomHit5:TDxImageButton;
  PlugCheckBoxHumManuallyFire:TDxImageButton;
  PlugCheckBoxHumManuallyFireBoom:TDxImageButton;
  PlugCheckBoxHumManuallyMeteorShower:TDxImageButton;
  PlugCheckBoxHumManuallyMove10Attack:TDxImageButton;
  PlugCheckBoxHumManuallySnowWind:TDxImageButton;
  PlugCheckBoxHumShootLightenLockTarget:TDxImageButton;
  PlugCheckBoxHumStruckShield:TDxImageButton;
  PlugCheckBoxSmart113Hit:TDxImageButton;
  PlugCheckBoxSmartCRSHit:TDxImageButton;
  PlugCheckBoxSmartFireHit:TDxImageButton;
  PlugCheckBoxSmartKTZHit:TDxImageButton;
  PlugCheckBoxSmartLongHit:TDxImageButton;
  PlugCheckBoxSmartPosLongHit:TDxImageButton;
  PlugCheckBoxSmartSwordHit:TDxImageButton;
  PlugCheckBoxSmartTWNHit:TDxImageButton;
  PlugCheckBoxSmartWalkLongHit:TDxImageButton;
  PlugCheckBoxSmartWideHit:TDxImageButton;
  PlugComboBoxAutoMagic:TDxComboBox;
  PlugEditAutoMagicTime:TDxEdit;
  PlugMemoConfig5Label19:TDxLabel;
  PlugMemoConfig5Label20:TDxLabel;
  PlugMemoConfig5Label21:TDxLabel;
  PlugMemoConfig5Label22:TDxLabel;
  PlugMemoConfig5Label23:TDxLabel;
  PlugMemoConfig5LabelLongHit:TDxLabel;
  PlugTabSheetConfig6:TDxTabSheet;
  PlugMemoConfig6:TDxScrollBox;
  PlugCheckBoxUseKeyBoard:TDxImageButton;
  PlugMemoConfig6Label1:TDxLabel;
  PlugMemoConfig6Label2:TDxLabel;
  PlugMemoConfig6Label3:TDxLabel;
  PlugMemoConfig6LabelKeyBoard1:TDxLabel;
  PlugMemoConfig6LabelKeyBoard10:TDxLabel;
  PlugMemoConfig6LabelKeyBoard11:TDxLabel;
  PlugMemoConfig6LabelKeyBoard12:TDxLabel;
  PlugMemoConfig6LabelKeyBoard13:TDxLabel;
  PlugMemoConfig6LabelKeyBoard14:TDxLabel;
  PlugMemoConfig6LabelKeyBoard15:TDxLabel;
  PlugMemoConfig6LabelKeyBoard16:TDxLabel;
  PlugMemoConfig6LabelKeyBoard2:TDxLabel;
  PlugMemoConfig6LabelKeyBoard3:TDxLabel;
  PlugMemoConfig6LabelKeyBoard4:TDxLabel;
  PlugMemoConfig6LabelKeyBoard5:TDxLabel;
  PlugMemoConfig6LabelKeyBoard6:TDxLabel;
  PlugMemoConfig6LabelKeyBoard7:TDxLabel;
  PlugMemoConfig6LabelKeyBoard8:TDxLabel;
  PlugMemoConfig6LabelKeyBoard9:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc1:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc10:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc11:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc12:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc13:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc14:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc15:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc16:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc2:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc3:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc4:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc5:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc6:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc7:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc8:TDxLabel;
  PlugMemoConfig6LabelKeyBoardDesc9:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal1:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal10:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal11:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal12:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal13:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal14:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal15:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal16:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal2:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal3:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal4:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal5:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal6:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal7:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal8:TDxLabel;
  PlugMemoConfig6LabelKeyBoardNormal9:TDxLabel;
  PlugMemoConfig6Line1:TDxLine;
  PlugTabSheetConfig7:TDxTabSheet;
  PlugMemoConfigBoss:TDxChatMemo;
  PlugBtnBossAdd:TDxImageButton;
  PlugBtnBossDel:TDxImageButton;
  PlugBtnBossModify:TDxImageButton;
  PlugCheckBoxAutoDownHorse:TDxImageButton;
  PlugCheckBoxAutoLock:TDxImageButton;
  PlugCheckBoxBlacklistHit:TDxImageButton;
  PlugCheckBoxColorShow:TDxImageButton;
  PlugCheckBoxDisableSelfStruck:TDxImageButton;
  PlugCheckBoxFriendHit:TDxImageButton;
  PlugCheckBoxHideBigHPProgress:TDxImageButton;
  PlugCheckBoxMagicLock:TDxImageButton;
  PlugCheckBoxNearHint:TDxImageButton;
  PlugCheckBoxNoCaton:TDxImageButton;
  PlugCheckBoxShowTargetAperture:TDxImageButton;
  PlugComboBoxColorShow:TDxComboBox;
  PlugEditBoss:TDxEdit;
  PlugLblBoss:TDxLabel;
  PlugScrollBoxBoss:TDxChatMemo;
  PlugCheckBoxNearEffect:TDxImageButton; //HZQ 20230828 增加
  PlugTabSheetConfig8:TDxTabSheet;
  PlugMemoConfig8:TDxScrollBox;
  PlugBtnGJPoint:TDxImageButton;
  PlugButtonGJRun:TDxImageButton;
  PlugCheckBoxAutoPickup:TDxImageButton;
  PlugCheckBoxBagFull:TDxImageButton;
  PlugCheckBoxDFAvoid:TDxImageButton;
  PlugCheckBoxLimitScreen:TDxImageButton;
  PlugCheckBoxNoBluePoison:TDxImageButton;
  PlugCheckBoxNoDuFu:TDxImageButton;
  PlugCheckBoxNoRedPoison:TDxImageButton;
  PlugCheckBoxNotRushMon:TDxImageButton;
  PlugCheckBoxPlayAttack:TDxImageButton;
  PlugComboBoxBagFullValue:TDxComboBox;
  PlugComboBoxNoBluePoisonValue:TDxComboBox;
  PlugComboBoxNoDuFuValue:TDxComboBox;
  PlugComboBoxNoRedPoisonValue:TDxComboBox;
  PlugComboBoxPlayAttackValue:TDxComboBox;
  PlugEditNotRushMonRange:TDxEdit;
  PlugLabelNotRushMon:TDxLabel;
  PlugMemoConfig8Button1:TDxImageButton;
  PlugMemoConfig8Button2:TDxImageButton;
  PlugMemoConfig8Button3:TDxImageButton;
  PlugMemoConfig8Line1:TDxLine;
  PlugMemoConfig8Line2:TDxLine;
  PlugMemoConfig8Page:TDxPageControl;
  PlugTabSheetConfig81:TDxTabSheet;
  PlugButtonMonNameAdd:TDxImageButton;
  PlugButtonMonNameDel:TDxImageButton;
  PlugButtonMonNameEdit:TDxImageButton;
  PlugEditMonName:TDxEdit;
  PlugLabelMonName:TDxLabel;
  PlugLabelShortKey:TDxLabel;
  PlugScrollBoxMons:TDxChatMemo;
  PlugTabSheetConfig82:TDxTabSheet;
  PlugMemoConfig82:TDxListView;
  PlugLabelConfig82C1:TDxLabel;
  PlugLabelConfig82C2:TDxLabel;
  PlugLineConfig82C:TDxLine;
  PlugTabSheetConfig83:TDxTabSheet;
  Line1:TDxLine;
  PlugCheckBoxGroupAttack:TDxImageButton;
  PlugEditNotGroupAttackCount:TDxEdit;
  PlugLabelGroupAttack:TDxLabel;
  PlugMemoConfig83:TDxListView;
  PlugLabelConfig83C1:TDxLabel;
  PlugLabelConfig83C2:TDxLabel;
  PlugLineConfig83C:TDxLine;
  PlugTabSheetConfig80:TDxTabSheet;
  PlugMemoConfig10ButtonCancel:TDxImageButton;
  PlugMemoConfig10ButtonEdit:TDxImageButton;
  PlugMemoConfig10ButtonSave:TDxImageButton;
  PlugMemoConfigNotes:TDxChatMemo;
  PlugTabSheetConfig9:TDxTabSheet;
  PlugMemoConfigHelp:TDxChatMemo;
  PopupMenuItems:TDxPopupMenu;
  DOptBtnSKiilIcon:TDxImageButton;
  DOptBtnSkillLine:TDxImageButton;
  DOptFrm:TDxImageForm;
  DOptBtnClose:TDxImageButton;
  DOptPgc:TDxPageControl;
  DOptTs1:TDxTabSheet;
  ScrollBox1:TDxScrollBox;
  DOptChkAutoRepair:TDxImageButton;
  DOptChkAutoTakeOn:TDxImageButton;
  DOptChkDuraHint:TDxImageButton;
  DOptChkExpFilter:TDxImageButton;
  DOptChkHPNumber:TDxImageButton;
  DOptChkHealthHint:TDxImageButton;
  DOptChkItemCompare:TDxImageButton;
  DOptChkJobLevel:TDxImageButton;
  DOptChkJoyStick:TDxImageButton;
  DOptChkNumberDecHP:TDxImageButton;
  DOptChkOnlyShowHumName:TDxImageButton;
  DOptChkShowActorName:TDxImageButton;
  DOptChkShowHealthBar:TDxImageButton;
  DOptChkShowMonName:TDxImageButton;
  DOptChkSimpleDress:TDxImageButton;
  DOptChkSimpleMon:TDxImageButton;
  DOptEdtExpFilter:TDxEdit;
  DOptLblBGM:TDxLabel;
  DOptLblBasic:TDxLabel;
  DOptLblMapScale:TDxLabel;
  DOptLblSound:TDxLabel;
  DOptTrckbrBGM:TDxTrackBar;
  DOptTrckbrMapScale:TDxTrackBar;
  DOptTrckbrSound:TDxTrackBar;
  DOptTs2:TDxTabSheet;
  ScrollBox2:TDxScrollBox;
  DOptLblHPGoHome:TDxLabel;
  DOptLblProtect:TDxLabel;
  DOptLine1:TDxLine;
  Edit2:TDxEdit;
  Edit3:TDxEdit;
  Edit4:TDxEdit;
  Edit5:TDxEdit;
  Edit6:TDxEdit;
  Edit7:TDxEdit;
  Edit8:TDxEdit;
  Edit9:TDxEdit;
  ImageButton11:TDxImageButton;
  ImageButton18:TDxImageButton;
  ImageButton19:TDxImageButton;
  ImageButton20:TDxImageButton;
  ImageButton21:TDxImageButton;
  ImageButton22:TDxImageButton;
  ImageButton23:TDxImageButton;
  ImageButton24:TDxImageButton;
  Label10:TDxLabel;
  Label11:TDxLabel;
  Label12:TDxLabel;
  Label13:TDxLabel;
  Label14:TDxLabel;
  Label15:TDxLabel;
  Label16:TDxLabel;
  Label17:TDxLabel;
  Label18:TDxLabel;
  Label19:TDxLabel;
  Label20:TDxLabel;
  Label6:TDxLabel;
  Label7:TDxLabel;
  Label8:TDxLabel;
  Label9:TDxLabel;
  TrackBar10:TDxTrackBar;
  TrackBar2:TDxTrackBar;
  TrackBar4:TDxTrackBar;
  TrackBar5:TDxTrackBar;
  TrackBar6:TDxTrackBar;
  TrackBar7:TDxTrackBar;
  TrackBar8:TDxTrackBar;
  TrackBar9:TDxTrackBar;
  DOptTs3:TDxTabSheet;
  ScrollBox3:TDxScrollBox;
  DOptChkSlaveFlowMaster:TDxImageButton;
  Edit10:TDxEdit;
  ImageButton12:TDxImageButton;
  ImageButton2:TDxImageButton;
  ImageButton26:TDxImageButton;
  ImageButton27:TDxImageButton;
  ImageButton28:TDxImageButton;
  ImageButton29:TDxImageButton;
  ImageButton3:TDxImageButton;
  ImageButton30:TDxImageButton;
  ImageButton31:TDxImageButton;
  ImageButton32:TDxImageButton;
  ImageButton33:TDxImageButton;
  ImageButton34:TDxImageButton;
  ImageButton35:TDxImageButton;
  ImageButton36:TDxImageButton;
  ImageButton37:TDxImageButton;
  ImageButton38:TDxImageButton;
  ImageButton39:TDxImageButton;
  ImageButton4:TDxImageButton;
  ImageButton40:TDxImageButton;
  ImageButton41:TDxImageButton;
  ImageButton42:TDxImageButton;
  ImageButton43:TDxImageButton;
  ImageButton44:TDxImageButton;
  ImageButton45:TDxImageButton;
  ImageButton46:TDxImageButton;
  ImageButton47:TDxImageButton;
  ImageButton48:TDxImageButton;
  ImageButton50:TDxImageButton;
  ImageButton53:TDxImageButton;
  ImageButton54:TDxImageButton;
  ImageButton55:TDxImageButton;
  ImageButton56:TDxImageButton;
  ImageButton57:TDxImageButton;
  ImageButton58:TDxImageButton;
  ImageButton59:TDxImageButton;
  ImageButton61:TDxImageButton;
  Label1:TDxLabel;
  Label21:TDxLabel;
  Label22:TDxLabel;
  Label23:TDxLabel;
  Label24:TDxLabel;
  DOptTs4:TDxTabSheet;
  Label25:TDxLabel;
  Label26:TDxLabel;
  Label27:TDxLabel;
  Label28:TDxLabel;
  Label29:TDxLabel;
  ListView1:TDxListView;
  Label30:TDxLabel;
  Label31:TDxLabel;

private
  FLoadControl:Boolean;
  FLoadConfig:Boolean;
  FHandle:THandle;
  FScreenMode:Byte;
  FClientVersion:TClientVersion;
  FWindowMode:Boolean;

  FEnabled:Boolean;
  FConfigCheckeds:array[TConfigChecked] of Boolean;
  FInitializeed:Boolean;

  //FProtectList: TStringList;
  FHintItemDuraTick:LongWord;
  FClientConfig:TClientConfig;
  FProtectEnabled:Boolean;
  FProtectEnabledTick:LongWord;

  procedure PlugConfigDlgCloseClickEx(Sender:TObject; X, Y:Integer); stdcall;
  procedure PlugPageControlConfigActivePageChange(Sender:TObject); stdcall;
  procedure PlugPageControlConfigInRealArea(Sender:TObject; X, Y:Integer; var IsRealArea:Boolean); stdcall;
  procedure CheckBoxClickEx(Sender:TObject; X, Y:Integer); stdcall;
  procedure RefUseItemConfigClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure RefUseItemConfig(nObj:Integer);
  procedure RefConfig;
  procedure DEditChange(Sender:TObject); stdcall;
  procedure DComboBoxItemStdModeSelect(Sender:TObject); stdcall;
  // BOSS变色显示事件
  procedure DComboBoxColorShow(Sender:TObject); stdcall;

  procedure DEditSearchItemChange(Sender:TObject); stdcall;
  procedure DLabelDefaultItemClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure ListViewItemClick(Sender:TObject; ARow, ACol:Integer; ListItem:TObject; ViewItem:Pointer); stdcall;

  procedure ListViewGJMagicItemClick(Sender:TObject; ARow, ACol:Integer; ListItem:TObject; ViewItem:Pointer); stdcall;

  procedure OnChanggingVolumePosition(Sender:TObject); stdcall;
  procedure OnChangedVolumePosition(Sender:TObject); stdcall;

  procedure DEditCheckHPPercentChange(Sender:TObject); stdcall;
  procedure DEditCheckMPPercentChange(Sender:TObject); stdcall;
  procedure ComboBoxCheckHPValueChange(Sender:TObject); stdcall;
  procedure ComboBoxCheckMPValueChange(Sender:TObject); stdcall;
  procedure DCheckBoxCheckHPIsAutoClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DCheckBoxCheckMPIsAutoClick(Sender:TObject; X, Y:Integer); stdcall;

  procedure DEditRenewHPPercentChange(Sender:TObject); stdcall;
  procedure DEditRenewMPPercentChange(Sender:TObject); stdcall;
  procedure DEditRenewSpecialHPPercentChange(Sender:TObject); stdcall;
  procedure DEditRenewSpecialMPPercentChange(Sender:TObject); stdcall;
  procedure DEditRenewHPTimeChange(Sender:TObject); stdcall;

  procedure PlugEditHeroDodgeHPPercentChange(Sender:TObject); stdcall;

  procedure DCheckBoxCheckDuraIsAuto(Sender:TObject; X, Y:Integer); stdcall;

  procedure DEditCheckDuraChange(Sender:TObject); stdcall;
  procedure DEditCheckDuraValueChange(Sender:TObject); stdcall;
  procedure DEditCheckDuraTimeChange(Sender:TObject); stdcall;

  procedure DEditRenewMPTimeChange(Sender:TObject); stdcall;
  procedure DEditRenewSpecialHPTimeChange(Sender:TObject); stdcall;
  procedure DEditRenewSpecialMPTimeChange(Sender:TObject); stdcall;
  procedure DCheckBoxRenewHPIsAutoClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DCheckBoxRenewMPIsAutoClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DCheckBoxRenewSpecialHPIsAutoClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DCheckBoxRenewSpecialMPIsAutoClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DEditSuperMedicaHPChange(Sender:TObject); stdcall;
  procedure DEditSuperMedicaHPTimeChange(Sender:TObject); stdcall;
  procedure DEditSuperMedicaMPChange(Sender:TObject); stdcall;
  procedure DEditSuperMedicaMPTimeChange(Sender:TObject); stdcall;
  procedure DCheckBoxUseSuperMedicaItemNameClick(Sender:TObject; X, Y:Integer); stdcall;

  procedure DCheckBoxAutoPercentClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DCheckBoxRenewAutoPercentClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DCheckBoxSuperMedicaPercentClick(Sender:TObject; X, Y:Integer); stdcall;

  procedure DLabelKeyBoardKeyDown(Sender:TObject; var Key:Word;
    Shift:TShiftState); stdcall;
  procedure DLabelKeyBoardMouseDown(Sender:TObject; Button:TMouseButton;
    Shift:TShiftState; X, Y:Integer); stdcall;

  { TODO -opiaoyun -c新增 : Boss 【2013-08-03】}
  function CheckBossNameExists(Name:string; CurIndex:Integer = -1):Boolean;
  procedure DMemoBossListClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DBtnBossAddClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DBtnBossModifyClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DBtnBossDelClick(Sender:TObject; X, Y:Integer); stdcall;

  procedure SaveOrLoadBossList(IsSave:Boolean); stdcall;
  // 特殊物品颜色 piaoyun 2013-09-09
  procedure DEditSpecialColorChange(Sender:TObject); stdcall;
  // 增加特殊物品事件 piaoyun 2013-09-09

  procedure DBtnDiyAddClick(Sender:TObject; X, Y:Integer); stdcall;

  // 删除特殊物品事件 piaoyun 2013-09-10
  procedure DBtnDiyDelClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure DBtnItemsImportOrExportClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure PlugBtnDiyEditClick(Sender:TObject; X, Y:Integer); stdcall;

  procedure PlugBtnUnbindItemClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure PlugScrollBoxUnbindItemsClick(Sender:TObject; X, Y:Integer); stdcall;

  {--------------------------------------- 挂机相关按钮 --------------------------- }

  procedure SaveOrLoadGJMonList(IsSave:Boolean); stdcall;

  procedure SaveOrLoadGJMagicList1(IsSave:Boolean); stdcall;
  procedure SaveOrLoadGJMagicList2(IsSave:Boolean); stdcall;

  // 挂机页面功能分组点击事件
  procedure DBtnGJPageControlClick(Sender:TObject; X, Y:Integer); stdcall;

  procedure DMemoGJMonListClick(Sender:TObject; X, Y:Integer); stdcall;

  function CheckGJMonNameExists(Name:string; CurIndex:Integer = -1):Boolean;

  // 挂机页面"不打怪"添加
  procedure DBtnGJMonNameAddClick(Sender:TObject; X, Y:Integer); stdcall;

  // 挂机页面"不打怪"编辑
  procedure DBtnGJMonNameEditClick(Sender:TObject; X, Y:Integer); stdcall;

  // 挂机页面"不打怪"删除
  procedure DBtnGJMonNameDelClick(Sender:TObject; X, Y:Integer); stdcall;

  procedure DEditNotRushMonRangeChange(Sender:TObject); stdcall;
  procedure ComboBoxPlayAttackValueSelect(Sender:TObject); stdcall;
  procedure DEditGroupAttackCountChanged(Sender:TObject); stdcall;

  procedure DControlMouseMoveShowHint(Sender:TObject; Shift:TShiftState; X,
    Y:Integer); stdcall;

  // 开始挂机/停止挂机
  procedure DBtnGJRunClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure PlugBtnGJPointClick(Sender:TObject; X, Y:Integer); stdcall;

  procedure OnPlugMemoConfig10ButtonEditClick(Sender:TObject; X, Y:Integer); stdcall;
  procedure OnPlugPageControlConfigActivePageChange(Sender:TObject); stdcall;
  procedure OnPopupMenuItemsClick(Sender:TObject; X, Y:Integer); stdcall;

  procedure ShowViewNotes;

  procedure LoadHelpFile;

  procedure DuraWarning(); // 持久警告
  procedure AutoUseMagic(Sender:TObject);
  procedure AutoProtect(Sender:TObject);
  procedure AutoUseItem(Sender:TObject);
  procedure AutoEatHPItem(Sender:TObject);
  procedure AutoEatMPItem(Sender:TObject);
  procedure AutoEatSpecialHPItem(Sender:TObject);
  procedure AutoEatSpecialMPItem(Sender:TObject);
  procedure DamageHPUseItem(nObj, nDamage:Integer);
  procedure DamageMPUseItem(nObj, nDamage:Integer);

  procedure LoadConfigFile();
  procedure SaveConfigFile();

  procedure DoInitAllComponentsMouseMove(ParentCtrl:TDxControl);

  procedure LoadNotesFile;
  procedure SaveNotesFile;

protected
  procedure MouseMoveEvent(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall; //HZQ 20230525 From private
  function MakeControlAddressList:THashedStringList; //HZQ 为了兼容性放弃从RTTI信息读取控件地址，改为初始化生成地址
  procedure PatchAddNearEffectCheckBox(); //HZQ20230828
  //procedure PatchMirConfigDlgUI(ControlAddrList:THashedStringList);
public
  FMemo:TMemo;

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
  procedure ClearShowItem; override;
  procedure RefShowItem; override;
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
  procedure Run; override;
  procedure RefActorList; override;
  function CanFilterExp(Exp:LongWord):Boolean; override;
  function GetShowItem(const ItemName:string):pTShowItem; override;
  function FindShowItem(const ItemName:string):Boolean; override;
  function FindHintItem(const ItemName:string):Boolean; override;
  function FindPickItem(const ItemName:string):Boolean; override;
  procedure HintItem(const ItemName:string; X, Y:Integer); override;

  procedure RefKeyboardConfig; override;

  constructor Create();
  destructor Destroy; override;

  procedure RefreshGJMagic;
  procedure RefBindItemList;
  procedure AddToBossList(sName:string); override; //20230829
  procedure RemoveFromBossList(sName:string); override;
  procedure AddOrRemoveBossList(sName:string); override;
end;

implementation
uses
  MShare,
  ConfigShare,
  //IniFiles,
  FState,
  ClMain,
  Math,
  FilterItems,
  HUtil32,
  BassSound,
  ClFunc;

// {$R ..\..\DxComponent\MirConfigDlg.res}
type
  TConfig = record
    nFilterMinExp:Integer;
    nAutoUseMagicTime:Integer;
    dwAutoUseMagicTick:LongWord;

    boRenewSpecialIsAuto:Boolean;
    nRenewSpecialPercent:Integer;
    nRenewSpecialTime:Integer;

    boRenewBookIsAuto:Boolean;
    nRenewBookPercent:Integer;
    nRenewBookTime:Integer;
    nRenewBookNowBookIndex:Integer;
    sRenewBookNowBookItem:string;
    // ============药品==================
    nRenewHeroHPTime:Integer;
    nRenewHeroHPPercent:Integer;

    nRenewHeroMPTime:Integer;
    nRenewHeroMPPercent:Integer;

    boRenewHeroSpecialIsAuto:Boolean;
    nRenewHeroSpecialTime:Integer;
    nRenewHeroSpecialPercent:Integer;

    boRenewHeroLogOutIsAuto:Boolean;
    nRenewHeroLogOutTime:Integer;
    nRenewHeroLogOutPercent:Integer;

    boRenewCloseIsAuto:Boolean;
    nRenewCloseTime:Integer;
    nRenewClosePercent:Integer;

    MedicaMode:Integer;

    ChkAutoPercents:array[0..4] of Boolean;
    ChkRenewAutoPercents:array[0..4] of Boolean;
    ChkSuperMedicaPercents:array[0..4] of Boolean;

    CheckHpIsAutos:array[0..4] of Boolean;
    CheckHpPercents:array[0..4] of Integer;
    CheckHpValues:array[0..4] of Integer;
    CheckHpCheckTimes:array[0..4] of LongWord;
    CheckHpCheckTicks:array[0..4] of LongWord;
    CheckHpUseTimes:array[0..4] of LongWord;
    CheckHpUseTicks:array[0..4] of LongWord;

    CheckMpIsAutos:array[0..4] of Boolean;
    CheckMpPercents:array[0..4] of Integer;
    CheckMpValues:array[0..4] of Integer;
    CheckMpCheckTimes:array[0..4] of LongWord;
    CheckMpCheckTicks:array[0..4] of LongWord;
    CheckMpUseTimes:array[0..4] of LongWord;
    CheckMpUseTicks:array[0..4] of LongWord;

    RenewHPIsAutos:array[0..4] of Boolean;
    RenewHPPercents:array[0..4] of Integer;
    RenewHPTimes:array[0..4] of Integer;
    RenewHPTicks:array[0..4] of LongWord;
    RenewHPChatStringTicks:array[0..4] of LongWord;

    RenewMPIsAutos:array[0..4] of Boolean;
    RenewMPPercents:array[0..4] of Integer;
    RenewMPTimes:array[0..4] of Integer;
    RenewMPTicks:array[0..4] of LongWord;
    RenewMPChatStringTicks:array[0..4] of LongWord;

    RenewSpecialHPIsAutos:array[0..4] of Boolean;
    RenewSpecialHPPercents:array[0..4] of Integer;
    RenewSpecialHPTimes:array[0..4] of Integer;
    RenewSpecialHPTicks:array[0..4] of LongWord;
    RenewSpecialHPChatStringTicks:array[0..4] of LongWord;

    RenewSpecialMPIsAutos:array[0..4] of Boolean;
    RenewSpecialMPPercents:array[0..4] of Integer;
    RenewSpecialMPTimes:array[0..4] of Integer;
    RenewSpecialMPTicks:array[0..4] of LongWord;
    RenewSpecialMPChatStringTicks:array[0..4] of LongWord;

    UseSuperMedicas:array[0..4] of Boolean;
    SuperMedicaItemNames:array[0..8] of string;
    SuperMedicaUses:array[0..4] of array[0..8] of Boolean;
    SuperMedicaHPs:array[0..4] of array[0..8] of Integer;
    SuperMedicaHPTimes:array[0..4] of array[0..8] of Integer;
    SuperMedicaHPTicks:array[0..4] of array[0..8] of Integer;

    SuperMedicaMPs:array[0..4] of array[0..8] of Integer;
    SuperMedicaMPTimes:array[0..4] of array[0..8] of Integer;
    SuperMedicaMPTicks:array[0..4] of array[0..8] of Integer;

    CheckDuraIsAutos:array[0..4] of Boolean;
    CheckDuraMin:array[0..4] of Integer;
    CheckDuraValue:array[0..4] of string[20];
    CheckDuraTime:array[0..4] of Integer;
    CheckDuraCheckTicks:array[0..4] of LongWord;

    nHeroDodgeHPPercent:Integer;
    nColorShowEff:Byte; // BOSS变色显示 piaoyun 2013-09-09
    nSpecialColor:Byte; // 特殊物品颜色 piaoyun 2013-09-09

    nGJPlayAttackOption:Integer; // 挂机 - 受玩家攻击后的操作
    nGJNoRedPoisonOption:Integer; // 挂机 - 红药用完后动作 chongchong 2014-12-06
    nGJNoBluePoisonOption:Integer; // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
    nGJNoDuFuOption:Integer; // 挂机 - 毒符用完后动作 chongchong 2014-12-06
    nGJBagFullOption:Integer; // 挂机 - 包裹满后动作 chongchong 2014-12-06
    nGJNotRushMonRange:Integer; // 挂机 - 怪物周围几格有玩家
    nGJGroupAttackCount:Integer; // 挂机 - 当被怪物包围时的操作
  end;
  pTConfig = ^TConfig;

var
  g_Config:TConfig = (
    nFilterMinExp:0;
    nAutoUseMagicTime:0;
    dwAutoUseMagicTick:0;
    // ============保护=================
    boRenewSpecialIsAuto:False;
    nRenewSpecialPercent:0;
    nRenewSpecialTime:0;

    boRenewBookIsAuto:False;
    nRenewBookPercent:0;
    nRenewBookTime:0;
    nRenewBookNowBookIndex:0;
    sRenewBookNowBookItem: '';
    // ============药品==================

    MedicaMode:0; // 0主体 1英雄 2战士副将 3法师副将 4道士副将

    ChkAutoPercents:(False, False, False, False, False);
    ChkRenewAutoPercents:(False, True, True, True, True);
    ChkSuperMedicaPercents:(False, False, False, False, False);

    CheckHpIsAutos:(False, False, False, False, False);
    CheckHpPercents:(0, 0, 0, 0, 0);
    CheckHpValues:(0, 0, 0, 0, 0);
    CheckHpCheckTimes:(1000, 1000, 1000, 1000, 1000);
    CheckHpCheckTicks:(0, 0, 0, 0, 0);
    CheckHpUseTimes:(10000, 10000, 10000, 10000, 10000);
    CheckHpUseTicks:(0, 0, 0, 0, 0);

    CheckMpIsAutos:(False, False, False, False, False);
    CheckMpPercents:(0, 0, 0, 0, 0);
    CheckMpValues:(0, 0, 0, 0, 0);
    CheckMpCheckTimes:(1000, 1000, 1000, 1000, 1000);
    CheckMpCheckTicks:(0, 0, 0, 0, 0);
    CheckMpUseTimes:(10000, 10000, 10000, 10000, 10000);
    CheckMpUseTicks:(0, 0, 0, 0, 0);

    RenewHPIsAutos:(False, False, False, False, False);
    RenewHPPercents:(10, 97, 97, 97, 97);
    RenewHPTimes:(1000, 1000, 1000, 1000, 1000);
    RenewHPTicks:(0, 0, 0, 0, 0);
    RenewHPChatStringTicks:(0, 0, 0, 0, 0);

    RenewMPIsAutos:(False, False, False, False, False);
    RenewMPPercents:(10, 97, 97, 97, 97);
    RenewMPTimes:(1000, 1000, 1000, 1000, 1000);
    RenewMPTicks:(0, 0, 0, 0, 0);
    RenewMPChatStringTicks:(0, 0, 0, 0, 0);

    RenewSpecialHPIsAutos:(False, False, False, False, False);
    RenewSpecialHPPercents:(10, 88, 88, 88, 88);
    RenewSpecialHPTimes:(1000, 3000, 3000, 3000, 3000);
    RenewSpecialHPTicks:(0, 0, 0, 0, 0);
    RenewSpecialHPChatStringTicks:(0, 0, 0, 0, 0);

    RenewSpecialMPIsAutos:(False, False, False, False, False);
    RenewSpecialMPPercents:(10, 88, 88, 88, 88);
    RenewSpecialMPTimes:(1000, 3000, 3000, 3000, 3000);
    RenewSpecialMPTicks:(0, 0, 0, 0, 0);
    RenewSpecialMPChatStringTicks:(0, 0, 0, 0, 0);

    UseSuperMedicas:(False, False, False, False, False);

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

    SuperMedicaUses:(
    (False, False, False, False, False, False, False, False, False),
    (False, False, False, False, False, False, False, False, False),
    (False, False, False, False, False, False, False, False, False),
    (False, False, False, False, False, False, False, False, False),
    (False, False, False, False, False, False, False, False, False)
    );

    SuperMedicaHPs:(
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0)
    );

    SuperMedicaHPTimes:(
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500)
    );

    SuperMedicaHPTicks:(
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0)
    );

    SuperMedicaMPs:(
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0)
    );

    SuperMedicaMPTimes:(
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500),
    (500, 500, 500, 500, 500, 500, 500, 500, 500)
    );

    SuperMedicaMPTicks:(
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0),
    (0, 0, 0, 0, 0, 0, 0, 0, 0));

    CheckDuraIsAutos:(False, False, False, False, False);
    CheckDuraMin:(20, 20, 20, 20, 20);
    CheckDuraValue:('修复神水', '修复神水', '修复神水', '修复神水', '修复神水');
    CheckDuraTime:(30, 30, 30, 30, 30);
    CheckDuraCheckTicks:(0, 0, 0, 0, 0);

    nHeroDodgeHPPercent:0;
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

constructor TMirConfigDlg.Create();
{$IF TESTMODE = 0}
var
  I:Integer;
  {$IFEND}
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

  //HZQ 20230602 此处未设置GUI内挂默认选项
  FConfigCheckeds[ckShowHPLabel] := True;
  FConfigCheckeds[ckShowUserName] := False;
  FConfigCheckeds[ckMagicLock] := True;
  FConfigCheckeds[ckAutoOrderItem] := True;
  FConfigCheckeds[ckNotNeedShift] := True;
  FConfigCheckeds[ckPickUpAll] := False;
  FConfigCheckeds[ckBGMusic] := True;
  FConfigCheckeds[ckRepeatBGMusic] := True;
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
  FConfigCheckeds[ckHumShootLightenLockTarget] := True; // 疾光电影锁定目标
  FConfigCheckeds[ckHumManuallyFireBoom] := False; // 手动控制爆裂火焰
  FConfigCheckeds[ckHumManuallySnowWind] := False; // 手动控制冰咆哮
  FConfigCheckeds[ckHumManuallyMeteorShower] := False; // 手动控制流星火雨
  FConfigCheckeds[ckHumManuallyMove10Attack] := False; // 手动控制流星火雨

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

  FConfigCheckeds[ckAutoPickUpItem] := True;
  FConfigCheckeds[ckNoCaton] := False;

  //HZQ 20230601 设置 火墙淡化、隐藏怪物顶戴、自动绕行 默认值
  FConfigCheckeds[ckAutoDetourPath] := True;
  FConfigCheckeds[ckHideMonsterIcons] := False;
  FConfigCheckeds[ckDimFireEffect] := False;
  FConfigCheckeds[ckObjectHintEffect] := False; //HZQ 20230829

  FMemo := TMemo.Create(frmMain.Owner); //
  with FMemo do begin
    Parent := frmMain;
    Color := clblack;
    Font.Color := clWhite;
    Font.Size := 10;
    Ctl3D := False;
    BorderStyle := bsSingle; {OnKeyPress := EdDlgEditKeyPress;}
    Visible := False;
  end;

  {$IF TESTMODE = 0}
  for I := Low(g_Config.CheckDuraIsAutos) to High(g_Config.CheckDuraIsAutos) do begin
    g_Config.CheckDuraIsAutos[I] := g_ConfigClient.ClientConfigs_Ex[0];
  end;
  {$IFEND}
end;

destructor TMirConfigDlg.Destroy;
begin
  if FEnabled then
    SaveConfigFile;

  //FProtectList.Free;
  inherited;
end;

function TMirConfigDlg.GetType:TConfigDlgType;
begin
  Result := ptDefault;
end;

function TMirConfigDlg.GetConfigChecked(Index:TConfigChecked):Boolean;
begin
  Result := FConfigCheckeds[Index];
end;

procedure TMirConfigDlg.SetConfigChecked(Index:TConfigChecked; Value:Boolean);
begin
  if FConfigCheckeds[Index] <> Value then begin
    FConfigCheckeds[Index] := Value;
    RefConfig;
  end;
end;

procedure TMirConfigDlg.Open;
begin

end;

procedure TMirConfigDlg.Close;
begin
  FLoadConfig := False;
  FEnabled := False;
  if PlugConfigDlg <> nil then
    PlugConfigDlg.Visible := False;
  g_FileItemDB.BackUp;

  if FMemo.Visible then begin
    PlugMemoConfigNotes.Lines.Assign(FMemo.Lines);
    SaveNotesFile;

    ShowViewNotes;
  end;
end;

function TMirConfigDlg.GetVisible:Boolean;
begin
  if PlugConfigDlg <> nil then begin
    Result := PlugConfigDlg.Visible;
  end else begin
    Result := False; //HZQ 20230525
  end;
end;

procedure TMirConfigDlg.SetVisible(Value:Boolean);
begin
  if PlugConfigDlg <> nil then begin
    PlugConfigDlg.Visible := Value;
    if PlugCheckBoxShowHPLabel.Visible then
      PlugCheckBoxShowHPLabel.SetFocus
    else if PlugCheckBoxNumberLable.Visible then
      PlugCheckBoxNumberLable.SetFocus
    else if PlugCheckBoxJobAndLevel.Visible then
      PlugCheckBoxJobAndLevel.SetFocus;

    if Value then begin
      RefreshGJMagic;
      RefBindItemList;

      PlugEditItemName.Text := '';
      PlugEditUnbindName.Text := '';
      PlugComboGroupUnBindItem.ItemIndex := 0;
      PlugBtnUnbindItemDel.Enabled := PlugScrollBoxUnbindItems.ItemIndex >= 0;
      PlugBtnUnbindItemEdit.Enabled := PlugScrollBoxUnbindItems.ItemIndex >= 0;
    end;
  end;
end;

function TMirConfigDlg.GetEnabled:Boolean;
begin
  Result := FEnabled;
end;

procedure TMirConfigDlg.SetEnabled(Value:Boolean);
begin
  FEnabled := Value;
  if not FEnabled then
    FLoadConfig := False;
end;

function TMirConfigDlg.GetProtectEnabled:Boolean;
begin
  Result := FProtectEnabled;
end;

procedure TMirConfigDlg.SetProtectEnabled(Value:Boolean);
begin
  FProtectEnabled := Value;
  if FProtectEnabled then
    FProtectEnabledTick := MyGetTickCount;
end;

function TMirConfigDlg.FormKeyDown(var Key:Word; Shift:TShiftState):Boolean;
begin
  Result := False; //HZQ 20230525
end;

function TMirConfigDlg.FormKeyPress(var Key:Char):Boolean;
begin
  Result := False; //HZQ 20230525
end;

procedure TMirConfigDlg.RefreshMySelfAbil;
begin

end;

procedure TMirConfigDlg.RefreshMyHeroAbil;
begin

end;

procedure TMirConfigDlg.RefreshMySelfMagicList;
var
  I, nItemIndex:Integer;
begin
  if (g_MySelf <> nil) then begin
    nItemIndex := PlugComboBoxAutoMagic.ItemIndex;
    PlugComboBoxAutoMagic.Items.Clear;
    for I := 0 to g_MagicList.Count - 1 do begin
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

procedure TMirConfigDlg.RefreshMyHeroMagicList;
begin

end;

procedure TMirConfigDlg.RefreshUnBindItemList;
var
  I:Integer;
  BindItem:pTCustomBindItem;
  IsDelExit:Boolean;
begin
  if g_ClientConfig.boCloseBookProtect and g_ClientConfig.boCloseLogoutProtect then begin
    PlugComboBoxCheckHPValue.Items.Clear;
    PlugComboBoxCheckMPValue.Items.Clear;

    PlugComboBoxCheckHPValue.ItemIndex := -1;
    PlugComboBoxCheckMPValue.ItemIndex := -1;

    g_Config.CheckHPValues[g_Config.MedicaMode] := -1;
    g_Config.CheckMPValues[g_Config.MedicaMode] := -1;

    PlugComboBoxCheckHPValue.Text := '';
    PlugComboBoxCheckMPValue.Text := '';

    PlugComboBoxCheckHPValue.Enabled := False;
    PlugComboBoxCheckMPValue.Enabled := False;
  end else if g_ClientConfig.boCloseBookProtect then begin
    PlugComboBoxCheckHPValue.Enabled := True;
    PlugComboBoxCheckMPValue.Enabled := True;

    PlugComboBoxCheckHPValue.Items.Clear;
    PlugComboBoxCheckMPValue.Items.Clear;

    for I := 0 to g_NGProtectItems.Count - 1 do begin
      if SameText(g_NGProtectItems.Strings[I], '小退') then begin
        PlugComboBoxCheckHPValue.Items.Add(g_NGProtectItems.Strings[I]);
        PlugComboBoxCheckMPValue.Items.Add(g_NGProtectItems.Strings[I]);
      end;
    end;
  end else if g_ClientConfig.boCloseLogoutProtect then begin
    PlugComboBoxCheckHPValue.Enabled := True;
    PlugComboBoxCheckMPValue.Enabled := True;

    PlugComboBoxCheckHPValue.Items.Clear;
    PlugComboBoxCheckMPValue.Items.Clear;

    for I := 0 to g_NGProtectItems.Count - 1 do begin
      if not SameText(g_NGProtectItems.Strings[I], '小退') then begin
        PlugComboBoxCheckHPValue.Items.Add(g_NGProtectItems.Strings[I]);
        PlugComboBoxCheckMPValue.Items.Add(g_NGProtectItems.Strings[I]);
      end;
    end;

    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      BindItem := g_CustomUnbindItemList.Items[I];
      if BindItem.UnBindItemType = t_Book then begin
        if PlugComboBoxCheckHPValue.Items.IndexOf(BindItem.sItemName) < 0 then begin
          PlugComboBoxCheckHPValue.Items.Add(BindItem.sItemName);
          PlugComboBoxCheckMPValue.Items.Add(BindItem.sItemName);
        end;
      end;
    end;
  end else begin
    PlugComboBoxCheckHPValue.Items.Text := g_NGProtectItems.Text;
    PlugComboBoxCheckMPValue.Items.Text := g_NGProtectItems.Text;

    IsDelExit := False;
    if (g_NGProtectItems.Count > 0) and SameText(g_NGProtectItems.Strings[g_NGProtectItems.Count - 1], '小退') then begin
      PlugComboBoxCheckHPValue.Items.Delete(PlugComboBoxCheckHPValue.Items.Count - 1);
      PlugComboBoxCheckMPValue.Items.Delete(PlugComboBoxCheckMPValue.Items.Count - 1);
      IsDelExit := True;
    end;

    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      BindItem := g_CustomUnbindItemList.Items[I];
      if BindItem.UnBindItemType = t_Book then begin
        if PlugComboBoxCheckHPValue.Items.IndexOf(BindItem.sItemName) < 0 then begin
          PlugComboBoxCheckHPValue.Items.Add(BindItem.sItemName);
          PlugComboBoxCheckMPValue.Items.Add(BindItem.sItemName);
        end;
      end;
    end;

    if IsDelExit then begin
      PlugComboBoxCheckHPValue.Items.Add('小退');
      PlugComboBoxCheckMPValue.Items.Add('小退');
    end;
  end;
end;

procedure TMirConfigDlg.PlugPageControlConfigInRealArea(Sender:TObject; X, Y:Integer; var IsRealArea:Boolean);
begin
  IsRealArea := not ((X >= PlugPageControlConfig.Width - 12) and (Y <= PlugPageControlConfig.Height + 20));
end;

procedure TMirConfigDlg.PlugConfigDlgCloseClickEx(Sender:TObject; X, Y:Integer);
begin
  if PlugConfigDlg <> nil then
    PlugConfigDlg.Visible := False;

  if FMemo.Visible then begin
    PlugMemoConfigNotes.Lines.Assign(FMemo.Lines);
    SaveNotesFile;

    ShowViewNotes;
  end;
end;

procedure TMirConfigDlg.PlugPageControlConfigActivePageChange(Sender:TObject);
begin
  if PlugPageControlConfig.ActivePageIndex = 0 then begin
    if PlugCheckBoxShowHPLabel.Visible then
      PlugCheckBoxShowHPLabel.SetFocus
    else if PlugCheckBoxNumberLable.Visible then
      PlugCheckBoxNumberLable.SetFocus
    else if PlugCheckBoxJobAndLevel.Visible then
      PlugCheckBoxJobAndLevel.SetFocus;
  end;
end;

procedure TMirConfigDlg.Logout;
begin
  FLoadConfig := False;
  FEnabled := False;
  SaveConfigFile;
end;

procedure TMirConfigDlg.LoadHelpFile;
var
  I:Integer;
  ViewItem:pTViewItem;
begin
  // ---------------------------------读取帮助文件---------------------------------
  if (PlugMemoConfigHelp <> nil) and FileExists('Data\explain2.dat') then begin
    try
      PlugMemoConfigHelp.LoadFromFile('Data\explain2.dat');
    except
    end;

    PlugMemoConfigHelp.FontBackTransparent := True;

    for I := 0 to PlugMemoConfigHelp.Lines.Count - 1 do begin
      ViewItem := TDxLines(PlugMemoConfigHelp.Lines).Items[I];
      if (Length(ViewItem.Caption) <> Length(Trim(ViewItem.Caption))) then begin
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
      else begin
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

procedure TMirConfigDlg.ClearShowItem;
begin
  PlugMemoConfig2.Clear;
  PlugMemoConfig2.ColCount := 6;
end;

procedure TMirConfigDlg.RefShowItem;
var
  I:Integer;
  ShowItem:pTShowItem;
  ListItem:TDxListItem;
  ViewItem:pTViewItem;
begin
  PlugMemoConfig2.Clear;
  PlugMemoConfig2.ColCount := 6;

  PlugMemoConfig2.Lock;
  try
    for I := 0 to g_FileItemDB.m_ShowItemList.Count - 1 do begin
      ShowItem := pTShowItem(g_FileItemDB.m_ShowItemList.Items[I]);

      ListItem := PlugMemoConfig2.Add;
      ViewItem := ListItem.AddItem('', nil);

      ViewItem.Caption := ShowItem.sItemName;
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsButton; // bsRadio;
      ViewItem.Alignment := taLeftJustify;
      ViewItem.Color.Up.Color := clWhite;
      ViewItem.Color.Hot.Color := clRed; // clWhite;
      ViewItem.Color.Down.Color := clRed;

      if PlugMemoConfig2Label25.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boHintMsg;
      end;

      if PlugMemoConfig2Label26.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boPickup;
      end;

      if PlugMemoConfig2Label27.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boShowName;
      end;

      if PlugMemoConfig2Label28.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boShowSpecial;
      end;

      if PlugMemoConfig2Label29.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boAutoMove;
      end;
    end;
  finally
    PlugMemoConfig2.UnLock;
  end;
end;

procedure TMirConfigDlg.ListViewItemClick(Sender:TObject; ARow, ACol:Integer; ListItem:TObject; ViewItem:Pointer);
var
  ShowItem:pTShowItem;
  Index:Integer;
  IntArr:array[0..5] of Integer;
begin
  FillChar(IntArr, SizeOf(IntArr), 0);
  Index := 0;

  if PlugMemoConfig2Label25.Visible then begin
    Inc(Index);
    IntArr[Index] := 1;
  end;

  if PlugMemoConfig2Label26.Visible then begin
    Inc(Index);
    IntArr[Index] := 2;
  end;

  if PlugMemoConfig2Label27.Visible then begin
    Inc(Index);
    IntArr[Index] := 3;
  end;

  if PlugMemoConfig2Label28.Visible then begin
    Inc(Index);
    IntArr[Index] := 4;
  end;

  if PlugMemoConfig2Label29.Visible then begin
    Inc(Index);
    IntArr[Index] := 5;
  end;

  ShowItem := pTViewItem(ViewItem).Data;
  if ShowItem <> nil then begin
    (*
    case ACol of
      0: PlugEditSpecialName.Text := ShowItem.sItemName;
      1: ShowItem.boHintMsg := pTViewItem(ViewItem).Checked;
      2: ShowItem.boPickup := pTViewItem(ViewItem).Checked;
      3: ShowItem.boShowName := pTViewItem(ViewItem).Checked;
      4: ShowItem.boShowSpecial := pTViewItem(ViewItem).Checked;
      5: ShowItem.boAutoMove := pTViewItem(ViewItem).Checked;
    end;
    *)

    if ACol <= Index then begin
      if ACol = 0 then begin
        PlugEditSpecialName.Text := ShowItem.sItemName;
      end
      else begin
        case IntArr[ACol] of
          1:begin
              ShowItem.boHintMsg := pTViewItem(ViewItem).Checked;
            end;
          2:begin
              ShowItem.boPickup := pTViewItem(ViewItem).Checked;
              g_IsClientPickItemsChanged := True;
            end;
          3:begin
              ShowItem.boShowName := pTViewItem(ViewItem).Checked;
            end;
          4:begin
              ShowItem.boShowSpecial := pTViewItem(ViewItem).Checked;
              g_IsClientPickItemsChanged := True;
            end;
          5:begin
              ShowItem.boAutoMove := pTViewItem(ViewItem).Checked;
            end;
        end;
      end;
    end;

    g_FileItemDB.SaveToFile;
    g_DropItemsMgr.RefreshDrawList;
  end;
end;

procedure TMirConfigDlg.DLabelDefaultItemClick(Sender:TObject; X, Y:Integer);
var
  I:Integer;
  List:TList;

  ListItem:TDxListItem;
  ViewItem:pTViewItem;
  ShowItem:pTShowItem;
begin
  if mrOk = FrmDlg.DMessageDlg(DecodeResStr(SRestoreDefSettingAsk), [mbOk, mbCancel]) then begin
    g_FileItemDB.BackUp;

    List := TList.Create;
    g_FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List);
    PlugMemoConfig2.Clear;
    PlugMemoConfig2.ColCount := 6;

    PlugMemoConfig2.Lock;
    try
      for I := 0 to List.Count - 1 do begin
        ShowItem := List.Items[I];

        ListItem := PlugMemoConfig2.Add;
        ViewItem := ListItem.AddItem('', nil);

        ViewItem.Caption := ShowItem.sItemName;
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsButton; // bsRadio;
        ViewItem.Alignment := taLeftJustify;
        ViewItem.Color.Up.Color := clWhite;
        ViewItem.Color.Hot.Color := clRed; // clWhite;
        ViewItem.Color.Down.Color := clRed;

        if PlugMemoConfig2Label25.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ViewItem.Checked := ShowItem.boHintMsg;
        end;

        if PlugMemoConfig2Label26.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ViewItem.Checked := ShowItem.boPickup;
        end;

        if PlugMemoConfig2Label27.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ViewItem.Checked := ShowItem.boShowName;
        end;

        if PlugMemoConfig2Label28.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ViewItem.Checked := ShowItem.boShowSpecial;
        end;

        if PlugMemoConfig2Label29.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ;
          ViewItem.Checked := ShowItem.boAutoMove;
        end;
      end;
    finally
      PlugMemoConfig2.UnLock;
    end;

    List.Free;

    g_IsClientPickItemsChanged := True;
    g_FileItemDB.SaveToFile;
    g_DropItemsMgr.RefreshDrawList;
    PlugMemoConfig2.First;
  end;
end;

procedure TMirConfigDlg.DEditSearchItemChange(Sender:TObject);
var
  I:Integer;
  List:TList;

  ShowItem:pTShowItem;
  ListItem:TDxListItem;
  ViewItem:pTViewItem;
  sText:string;
begin
  if PlugEditSearchItem.Text = '' then begin
    DComboBoxItemStdModeSelect(Sender);
  end
  else begin
    sText := PlugEditSearchItem.Text;
    List := TList.Create;
    g_FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List);
    PlugMemoConfig2.Clear;
    PlugMemoConfig2.ColCount := 6;

    PlugMemoConfig2.Lock;
    try
      for I := 0 to List.Count - 1 do begin
        ShowItem := List.Items[I];
        if AnsiContainsText(sText, ShowItem.sItemName) or AnsiContainsText(ShowItem.sItemName, sText) then begin

          ListItem := PlugMemoConfig2.Add;
          ViewItem := ListItem.AddItem('', nil);

          ViewItem.Caption := ShowItem.sItemName;
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsButton; // bsRadio;
          ViewItem.Alignment := taLeftJustify;
          ViewItem.Color.Up.Color := clWhite;
          ViewItem.Color.Hot.Color := clRed; // clWhite;
          ViewItem.Color.Down.Color := clRed;

          if PlugMemoConfig2Label25.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boHintMsg;
          end;

          if PlugMemoConfig2Label26.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boPickup;
          end;

          if PlugMemoConfig2Label27.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boShowName;
          end;

          if PlugMemoConfig2Label28.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boShowSpecial;
          end;

          if PlugMemoConfig2Label29.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boAutoMove;
          end;
        end;
      end;
    finally
      PlugMemoConfig2.UnLock;
    end;
    List.Free;
  end;
end;

procedure TMirConfigDlg.DComboBoxItemStdModeSelect(Sender:TObject);
var
  I:Integer;
  List:TList;

  ShowItem:pTShowItem;
  ListItem:TDxListItem;
  ViewItem:pTViewItem;
begin
  List := TList.Create;
  g_FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List);
  PlugMemoConfig2.Clear;
  PlugMemoConfig2.ColCount := 6;
  PlugMemoConfig2.Lock;
  try
    for I := 0 to List.Count - 1 do begin
      ShowItem := List.Items[I];
      ListItem := PlugMemoConfig2.Add;
      ViewItem := ListItem.AddItem('', nil);

      ViewItem.Caption := ShowItem.sItemName;
      ViewItem.Data := ShowItem;
      ViewItem.Style := bsButton; // bsRadio;
      ViewItem.Alignment := taLeftJustify;
      ViewItem.Color.Up.Color := clWhite;
      ViewItem.Color.Hot.Color := clRed; // clWhite;
      ViewItem.Color.Down.Color := clRed;

      if PlugMemoConfig2Label25.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boHintMsg;
      end;

      if PlugMemoConfig2Label26.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boPickup;
      end;

      if PlugMemoConfig2Label27.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boShowName;
      end;

      if PlugMemoConfig2Label28.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boShowSpecial;
      end;

      if PlugMemoConfig2Label29.Visible then begin
        ViewItem := ListItem.AddItem('', nil);
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsCheckBox;
        if (g_ClientVersion <> cvMirNewUI205) then begin
          ViewItem.ImageIndex.ImageType := NewopUI_Pak;
          ViewItem.ImageIndex.Up := 228;
          ViewItem.ImageIndex.Down := 229;
        end
        else begin
          ViewItem.ImageIndex.ImageType := UICommon_wil;
          ViewItem.ImageIndex.Up := 28;
          ViewItem.ImageIndex.Down := 30;
        end;
        ViewItem.Checked := ShowItem.boAutoMove;
      end;
    end;
  finally
    PlugMemoConfig2.UnLock;
  end;
  List.Free;
end;

// BOSS颜色事件 piaoyun 2013-09-09

procedure TMirConfigDlg.DComboBoxColorShow(Sender:TObject); stdcall;
begin
  g_Config.nColorShowEff := PlugComboBoxColorShow.ItemIndex;
  frmMain.nColorShowEff := g_Config.nColorShowEff;
end;

procedure TMirConfigDlg.CheckBoxClickEx(Sender:TObject; X, Y:Integer);
var
  FileName:string;
begin
  if FClientConfig.boNotCanUseClientConfig then Exit;
  if PlugCheckBoxShowHPLabel = Sender then begin
    FConfigCheckeds[ckShowHPLabel] := PlugCheckBoxShowHPLabel.Checked;
  end else if PlugCheckBoxNumberLable = Sender then begin
    FConfigCheckeds[ckShowNumberLable] := PlugCheckBoxNumberLable.Checked;
  end else if PlugCheckBoxJobAndLevel = Sender then begin
    FConfigCheckeds[ckShowJobAndLevel] := PlugCheckBoxJobAndLevel.Checked;
  end else if PlugCheckBoxShowGreenHint = Sender then begin
    FConfigCheckeds[ckShowGreenHint] := PlugCheckBoxShowGreenHint.Checked;
    FrmDlg.ReshowNewGroupMember;
  end else if PlugCheckBoxShowActorName = Sender then begin
    FConfigCheckeds[ckShowUserName] := PlugCheckBoxShowActorName.Checked;
  end else if PlugCheckBoxHideDescUserName = Sender then begin
    FConfigCheckeds[ckOnlyShowCharName] := PlugCheckBoxHideDescUserName.Checked;
  end else if PlugCheckBoxDuraWarning = Sender then begin
    FConfigCheckeds[ckDuraWarning] := PlugCheckBoxDuraWarning.Checked;
  end else if PlugCheckBoxNoShift = Sender then begin
    FConfigCheckeds[ckNotNeedShift] := PlugCheckBoxNoShift.Checked;
  end else if PlugCheckBoxShiftSwitch = Sender then begin
    FConfigCheckeds[ckShiftSwitch] := PlugCheckBoxShiftSwitch.Checked;
  end else if PlugCheckBoxExpFilter = Sender then begin
    FConfigCheckeds[ckFilterExp] := PlugCheckBoxExpFilter.Checked;
  end else if PlugCheckBoxShowMimiMapDesc = Sender then begin
    FConfigCheckeds[ckShowMapDesc] := PlugCheckBoxShowMimiMapDesc.Checked;
  end else if PlugCheckBoxShowHighlightHPLabel = Sender then begin
    FConfigCheckeds[ckShowHighlightHPLabel] := PlugCheckBoxShowHighlightHPLabel.Checked;
  end else if PlugCheckBoxShowHealthNumber = Sender then begin
    FConfigCheckeds[ckShowMoveLable] := PlugCheckBoxShowHealthNumber.Checked;
  end else if PlugCheckBoxHideGhost = Sender then begin
    FConfigCheckeds[ckHideGhost] := PlugCheckBoxHideGhost.Checked;
  end else if PlugCheckBoxHideHumEffect = Sender then begin
    FConfigCheckeds[ckHideHumEffect] := PlugCheckBoxHideHumEffect.Checked;
  end else if PlugCheckBoxHideWeaponEffect = Sender then begin
    FConfigCheckeds[ckHideWeaponEffect] := PlugCheckBoxHideWeaponEffect.Checked;
  end else if PlugCheckBoxHideActorIcons = Sender then begin
    FConfigCheckeds[ckHideActorIcons] := PlugCheckBoxHideActorIcons.Checked;
  end else if PlugCheckBoxShowMonName = Sender then begin
    FConfigCheckeds[ckShowMonName] := PlugCheckBoxShowMonName.Checked;
  end else if PlugCheckBoxShowNpcName = Sender then begin // 显示NPC名 piaoyun 2013-07-31
    FConfigCheckeds[ckShowNpcName] := PlugCheckBoxShowNpcName.Checked;
  end else if PlugCheckBoxShowNpcHPLabel = Sender then begin // 显示NPC血条 piaoyun 2013-07-31
    FConfigCheckeds[ckShowNpcHPLabel] := PlugCheckBoxShowNpcHPLabel.Checked;
  end else if PlugCheckBoxShowNGLabel = Sender then begin // 显示NG黄条 piaoyun 2013-09-09
    FConfigCheckeds[ckShowNGLabel] := PlugCheckBoxShowNGLabel.Checked;
  end else if PlugCheckBoxAutoOrderItem = Sender then begin
    FConfigCheckeds[ckAutoOrderItem] := PlugCheckBoxAutoOrderItem.Checked;
  end else if PlugCheckBoxMagicLock = Sender then begin
    FConfigCheckeds[ckMagicLock] := PlugCheckBoxMagicLock.Checked;
  end else if PlugCheckBoxNotParaly = Sender then begin
    FConfigCheckeds[ckNotParaly] := PlugCheckBoxNotParaly.Checked;
  end else if PlugCheckBoxSmartLongHit = Sender then begin
    FConfigCheckeds[ckSmartLongHit] := PlugCheckBoxSmartLongHit.Checked;
  end else if PlugCheckBoxSmartPosLongHit = Sender then begin
    FConfigCheckeds[ckSmartPosLongHit] := PlugCheckBoxSmartPosLongHit.Checked;
  end else if PlugCheckBoxSmartWalkLongHit = Sender then begin
    FConfigCheckeds[ckSmartWalkLongHit] := PlugCheckBoxSmartWalkLongHit.Checked;
  end else if PlugCheckBoxSmartWideHit = Sender then begin
    FConfigCheckeds[ckSmartWideHit] := PlugCheckBoxSmartWideHit.Checked;
  end else if PlugCheckBoxSmartFireHit = Sender then begin
    FConfigCheckeds[ckSmartFireHit] := PlugCheckBoxSmartFireHit.Checked;
  end else if PlugCheckBoxSmart113Hit = Sender then begin
    FConfigCheckeds[ckSmart113Hit] := PlugCheckBoxSmart113Hit.Checked;
  end else if PlugCheckBoxAutoCustomHit1 = Sender then begin
    FConfigCheckeds[ckSmartCustomHit1] := PlugCheckBoxAutoCustomHit1.Checked;
  end else if PlugCheckBoxAutoCustomHit2 = Sender then begin
    FConfigCheckeds[ckSmartCustomHit2] := PlugCheckBoxAutoCustomHit2.Checked;
  end else if PlugCheckBoxAutoCustomHit3 = Sender then begin
    FConfigCheckeds[ckSmartCustomHit3] := PlugCheckBoxAutoCustomHit3.Checked;
  end else if PlugCheckBoxAutoCustomHit4 = Sender then begin
    FConfigCheckeds[ckSmartCustomHit4] := PlugCheckBoxAutoCustomHit4.Checked;
  end else if PlugCheckBoxAutoCustomHit5 = Sender then begin
    FConfigCheckeds[ckSmartCustomHit5] := PlugCheckBoxAutoCustomHit5.Checked;
  end else if PlugCheckBoxAutoCustomHit6 = Sender then begin
    FConfigCheckeds[ckSmartCustomHit6] := PlugCheckBoxAutoCustomHit6.Checked;
  end else if PlugCheckBoxAutoCustomHit7 = Sender then begin
    FConfigCheckeds[ckSmartCustomHit7] := PlugCheckBoxAutoCustomHit7.Checked;
  end else if PlugCheckBoxAutoCustomHit8 = Sender then begin
    FConfigCheckeds[ckSmartCustomHit8] := PlugCheckBoxAutoCustomHit8.Checked;
  end else if PlugCheckBoxHumManuallyCustomHit1 = Sender then begin
    FConfigCheckeds[ckHumManuallyCustomHit1] := PlugCheckBoxHumManuallyCustomHit1.Checked;
  end else if PlugCheckBoxHumManuallyCustomHit2 = Sender then begin
    FConfigCheckeds[ckHumManuallyCustomHit2] := PlugCheckBoxHumManuallyCustomHit2.Checked;
  end else if PlugCheckBoxHumManuallyCustomHit3 = Sender then begin
    FConfigCheckeds[ckHumManuallyCustomHit3] := PlugCheckBoxHumManuallyCustomHit3.Checked;
  end else if PlugCheckBoxHumManuallyCustomHit4 = Sender then begin
    FConfigCheckeds[ckHumManuallyCustomHit4] := PlugCheckBoxHumManuallyCustomHit4.Checked;
  end else if PlugCheckBoxHumManuallyCustomHit5 = Sender then begin
    FConfigCheckeds[ckHumManuallyCustomHit5] := PlugCheckBoxHumManuallyCustomHit5.Checked;
  end else if PlugCheckBoxShowValueItemEffect = Sender then begin
    FConfigCheckeds[ckShowValueItemEffect] := PlugCheckBoxShowValueItemEffect.Checked;
  end else if PlugCheckBoxAutoOpenSpell = Sender then begin
    FConfigCheckeds[ckAutoOpenSpell] := PlugCheckBoxAutoOpenSpell.Checked;
  end else if PlugCheckBoxAutoContinueAttack = Sender then begin
    FConfigCheckeds[ckAutoContinueAttack] := PlugCheckBoxAutoContinueAttack.Checked;
  end else if PlugCheckBoxSmartSwordHit = Sender then begin
    FConfigCheckeds[ckSmartSwordHit] := PlugCheckBoxSmartSwordHit.Checked;
  end else if PlugCheckBoxSmartKTZHit = Sender then begin
    FConfigCheckeds[ckSmart66Hit] := PlugCheckBoxSmartKTZHit.Checked;
  end else if PlugCheckBoxSmartCRSHit = Sender then begin
    FConfigCheckeds[ckSmartCrsHit] := PlugCheckBoxSmartCRSHit.Checked;
  end else if PlugCheckBoxSmartTWNHit = Sender then begin
    FConfigCheckeds[ckSmartTwnHit] := PlugCheckBoxSmartTWNHit.Checked;
  end else if PlugCheckBoxAutoGroupAttack = Sender then begin
    FConfigCheckeds[ckAutoGroupAttack] := PlugCheckBoxAutoGroupAttack.Checked;
  end else if PlugCheckBoxAutoGroupNoAttackMon = Sender then begin
    FConfigCheckeds[ckAutoGroupNoAttackMon] := PlugCheckBoxAutoGroupNoAttackMon.Checked;
  end else if PlugCheckBoxAutoHideMode = Sender then begin
    FConfigCheckeds[ckAutoHideMode] := PlugCheckBoxAutoHideMode.Checked;
  end else if PlugCheckBoxAutoCHangePoison = Sender then begin
    FConfigCheckeds[ckAutoCHangePoison] := PlugCheckBoxAutoCHangePoison.Checked;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
  end else if PlugCheckBoxHumAutoShield = Sender then begin
    FConfigCheckeds[ckHumAutoShield] := PlugCheckBoxHumAutoShield.Checked;
  end else if PlugCheckBoxHumStruckShield = Sender then begin
    FConfigCheckeds[ckHumStruckShield] := PlugCheckBoxHumStruckShield.Checked;
  end else if PlugCheckBoxHeroAutoShield = Sender then begin
    FConfigCheckeds[ckHeroAutoShield] := PlugCheckBoxHeroAutoShield.Checked;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
  end else if PlugCheckBoxHeroContinuousNoHitMon = Sender then begin
    FConfigCheckeds[ckHeroContinuousNoHitMon] := PlugCheckBoxHeroContinuousNoHitMon.Checked;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
  end else if PlugCheckBoxHeroShowNumberState = Sender then begin
    FConfigCheckeds[ckHeroShowNumberState] := PlugCheckBoxHeroShowNumberState.Checked;
  end else if PlugCheckBoxAssistantHeroAutoShield = Sender then begin
    FConfigCheckeds[ckAssistantHeroAutoShield] := PlugCheckBoxAssistantHeroAutoShield.Checked;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
  end else if PlugCheckBoxHumManuallySnowWind = Sender then begin
    FConfigCheckeds[ckHumManuallySnowWind] := PlugCheckBoxHumManuallySnowWind.Checked;
  end else if PlugCheckBoxHumManuallyMove10Attack = Sender then begin
    FConfigCheckeds[ckHumManuallyMove10Attack] := PlugCheckBoxHumManuallyMove10Attack.Checked;
  end else if PlugCheckBoxHumManuallyFire = Sender then begin
    FConfigCheckeds[ckHumManuallyFire] := PlugCheckBoxHumManuallyFire.Checked;
  end else if PlugCheckBoxHumManuallyFireBoom = Sender then begin
    FConfigCheckeds[ckHumManuallyFireBoom] := PlugCheckBoxHumManuallyFireBoom.Checked;
  end else if PlugCheckBoxHumShootLightenLockTarget = Sender then begin
    FConfigCheckeds[ckHumShootLightenLockTarget] := PlugCheckBoxHumShootLightenLockTarget.Checked;
  end else if PlugCheckBoxHumManuallyMeteorShower = Sender then begin
    FConfigCheckeds[ckHumManuallyMeteorShower] := PlugCheckBoxHumManuallyMeteorShower.Checked;
  end else if PlugCheckBoxAutoMagic = Sender then begin
    FConfigCheckeds[ckAutoUseMagic] := PlugCheckBoxAutoMagic.Checked;
  end else if PlugCheckBoxUseKeyBoard = Sender then begin
    FConfigCheckeds[ckUseKeyBoard] := PlugCheckBoxUseKeyBoard.Checked;
  end else if PlugCheckBoxUseSuperMedica = Sender then begin
    FConfigCheckeds[ckUseSuperMedica] := PlugCheckBoxUseSuperMedica.Checked;
    g_Config.UseSuperMedicas[g_Config.MedicaMode] := PlugCheckBoxUseSuperMedica.Checked;
  end else if PlugCheckBoxDisableSelfStruck = Sender then begin
    FConfigCheckeds[ckDisableSelfStruck] := PlugCheckBoxDisableSelfStruck.Checked;
  end else if PlugCheckBoxSpeedSlow = Sender then begin
    FConfigCheckeds[ckSpeedSlow] := PlugCheckBoxSpeedSlow.Checked;
  end else if PlugCheckBoxPickAll = Sender then begin
    FConfigCheckeds[ckPickupAll] := PlugCheckBoxPickAll.Checked;
  end else if PlugCheckBoxAutoPickUpItem = Sender then begin // 显示NPC血条 piaoyun 2013-07-31
    FConfigCheckeds[ckAutoPickUpItem] := PlugCheckBoxAutoPickUpItem.Checked;
  end else if PlugCheckBoxNoCaton = Sender then begin
    FConfigCheckeds[ckNoCaton] := PlugCheckBoxNoCaton.Checked;
  end else if PlugCheckBoxDisableDeal = Sender then begin
    FConfigCheckeds[ckDisableDeal] := PlugCheckBoxDisableDeal.Checked;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
  end else if PlugCheckDisableChartMemoSize = Sender then begin
    FConfigCheckeds[ckDisableChartMemoSize] := PlugCheckDisableChartMemoSize.Checked;
  end else if PlugCheckBoxItemCmp = Sender then begin
    FConfigCheckeds[ckItemCompare] := PlugCheckBoxItemCmp.Checked;
  end else if PlugCheckBoxBagFastItemCmp = Sender then begin
    FConfigCheckeds[ckBagFastItemCompare] := PlugCheckBoxBagFastItemCmp.Checked;
  end else if PlugCheckBoxHideItemEffect = Sender then begin
    FConfigCheckeds[ckHideItemEffect] := PlugCheckBoxHideItemEffect.Checked;
  end else if PlugCheckBoxVolume = Sender then begin
    FConfigCheckeds[ckVolume] := PlugCheckBoxVolume.Checked;
    g_boSound := PlugCheckBoxVolume.Checked;

    if g_boSound then begin
      DScreen.AddChatBoardString('[音效 开]', clWhite, clBlack); // clBlack

      // 声音开后，开地图背景 chongchong 2014-10-15
      if FileExists(g_sMapMusic) then
        PlayMp3(g_sMapMusic, True);

      // D:\热血传奇\wav\105.wav
      FileName := g_sSelfFilePath + g_SoundList[s_glass_button_click];
      if FileExists(FileName) then begin
        try
          g_BassSound.Play(FileName);
        except
        end;
      end;
    end else begin
      DScreen.AddChatBoardString('[音效 关]', clWhite, clBlack);
      g_PlaySound.Clear;

      // D:\热血传奇\wav\105.wav
      FileName := g_sSelfFilePath + g_SoundList[s_glass_button_click];
      if FileExists(FileName) then begin
        try
          g_BassSound.Play(FileName);
        except
        end;
      end;

      // 声音开后，开地图背景音乐 chongchong 2014-10-15
      if FileExists(g_sMapMusic) then
        PlayMp3(g_sMapMusic, True);
    end;
  end else if PlugCheckBoxBGMusic = Sender then begin
    FConfigCheckeds[ckBGMusic] := PlugCheckBoxBGMusic.Checked;

    if g_boBGSound <> FConfigCheckeds[ckBGMusic] then begin
      g_boBGSound := FConfigCheckeds[ckBGMusic];

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
  end else if PlugCheckBoxRepeatBGMusic = Sender then begin
    FConfigCheckeds[ckRepeatBGMusic] := PlugCheckBoxRepeatBGMusic.Checked;
    g_boRepeatBGSound := FConfigCheckeds[ckRepeatBGMusic];
  end else if PlugCheckBoxNearHint = Sender then begin // 接近提示 piaoyun 2013-09-09
    FConfigCheckeds[ckNearHint] := PlugCheckBoxNearHint.Checked;
  end else if PlugCheckBoxAutoLock = Sender then begin // 自动锁定 piaoyun 2013-09-09
    FConfigCheckeds[ckAutoLock] := PlugCheckBoxAutoLock.Checked;
  end else if PlugCheckBoxColorShow = Sender then begin // 变色显示 piaoyun 2013-09-09
    FConfigCheckeds[ckColorShow] := PlugCheckBoxColorShow.Checked;
  end else if PlugCheckBoxSpecialQuickFlashing = Sender then begin // 特殊物品快闪 piaoyun 2013-09-10
    FConfigCheckeds[ckSpecialQuickFlashing] := PlugCheckBoxSpecialQuickFlashing.Checked;
  end else if PlugCheckBoxBlacklistHit = Sender then begin // 黑名单近身提示 piaoyun 2013-09-11
    FConfigCheckeds[ckBlacklistHit] := PlugCheckBoxBlacklistHit.Checked;
  end else if PlugCheckBoxFriendHit = Sender then begin // 好友近身提示 piaoyun 2013-09-11
    FConfigCheckeds[ckFriendHit] := PlugCheckBoxFriendHit.Checked;
  end else if PlugCheckBoxSceneShake = Sender then begin // 屏幕震动 piaoyun 2013-09-14
    FConfigCheckeds[ckSceneShake] := PlugCheckBoxSceneShake.Checked;
  end else if PlugCheckBoxAutoDownHorse = Sender then begin
    FConfigCheckeds[ckAutoDownHorse] := PlugCheckBoxAutoDownHorse.Checked;
  end else if PlugCheckBoxHideTitle = Sender then begin
    FConfigCheckeds[ckHideTitle] := PlugCheckBoxHideTitle.Checked;
  end else if PlugCheckBoxContinueButchItem = Sender then begin
    FConfigCheckeds[ckContinueButchItem] := PlugCheckBoxContinueButchItem.Checked;
  end else if PlugCheckBoxUpdateStatus = Sender then begin
    FConfigCheckeds[ckShowUpdateStatus] := PlugCheckBoxUpdateStatus.Checked;
    frmMain.ShowUpdateStatusDlg(PlugCheckBoxUpdateStatus.Checked);
  end else if PlugCheckBoxNoRedPoison = Sender then begin
    FConfigCheckeds[ckGJ_NoRedPoison] := PlugCheckBoxNoRedPoison.Checked;
  end else if PlugCheckBoxNoBluePoison = Sender then begin
    FConfigCheckeds[ckGJ_NoBluePoison] := PlugCheckBoxNoBluePoison.Checked;
  end else if PlugCheckBoxPlayAttack = Sender then begin
    FConfigCheckeds[ckGJ_PlayAttack] := PlugCheckBoxPlayAttack.Checked;
  end else if PlugCheckBoxNotRushMon = Sender then begin
    FConfigCheckeds[ckGJ_NotRushMon] := PlugCheckBoxNotRushMon.Checked;
  end else if PlugCheckBoxNoDuFu = Sender then begin
    FConfigCheckeds[ckGJ_NoDuFu] := PlugCheckBoxNoDuFu.Checked;
  end else if PlugCheckBoxBagFull = Sender then begin
    FConfigCheckeds[ckGJ_BagFull] := PlugCheckBoxBagFull.Checked;
  end else if PlugCheckBoxAutoPickup = Sender then begin
    FConfigCheckeds[ckGJ_AutoPickup] := PlugCheckBoxAutoPickup.Checked;
  end else if PlugCheckBoxLimitScreen = Sender then begin
    FConfigCheckeds[ckGJ_LimitScreen] := PlugCheckBoxLimitScreen.Checked;
  end else if PlugCheckBoxGroupAttack = Sender then begin
    FConfigCheckeds[ckGJ_GroupAttack] := PlugCheckBoxGroupAttack.Checked;
  end else if PlugCheckBoxDFAvoid = Sender then begin
    FConfigCheckeds[ckGJ_DFStopAvoid] := PlugCheckBoxDFAvoid.Checked;
  end else if PlugCheckSimpleShowActor = Sender then begin
    FConfigCheckeds[ckSimpleShowActor] := PlugCheckSimpleShowActor.Checked;
  end else if PlugCheckSimpleShowHumanDress = Sender then begin
    FConfigCheckeds[ckSimpleShowHumanDress] := PlugCheckSimpleShowHumanDress.Checked;
  end else if PlugCheckSimpleShowHumanWeapon = Sender then begin
    FConfigCheckeds[ckSimpleShowHumanWeapon] := PlugCheckSimpleShowHumanWeapon.Checked;
  end else if PlugCheckSimpleShowBB = Sender then begin
    FConfigCheckeds[ckSimpleShowBB] := PlugCheckSimpleShowBB.Checked;
  end else if PlugCheckBoxHideBigHPProgress = Sender then begin
    FConfigCheckeds[ckHideBigHPProgress] := PlugCheckBoxHideBigHPProgress.Checked;
  end else if PlugCheckBoxShowHPUnit = Sender then begin
    FConfigCheckeds[ckShowHPUnit] := PlugCheckBoxShowHPUnit.Checked;
  end else if PlugCheckBoxShowTargetAperture = Sender then begin
    FConfigCheckeds[ckShowTargetAperture] := PlugCheckBoxShowTargetAperture.Checked;
  end else if PlugCheckBoxHideMonsterIcons = Sender then begin
    FConfigCheckeds[ckHideMonsterIcons] := PlugCheckBoxHideMonsterIcons.Checked;
  end else if PlugCheckBoxDimFireEffect = Sender then begin
    FConfigCheckeds[ckDimFireEffect] := PlugCheckBoxDimFireEffect.Checked;
  end else if PlugCheckBoxAutoDetourPath = Sender then begin
    FConfigCheckeds[ckAutoDetourPath] := PlugCheckBoxAutoDetourPath.Checked;
  end else if PlugCheckBoxNearEffect = Sender then begin
    FConfigCheckeds[ckObjectHintEffect] := PlugCheckBoxNearEffect.Checked;  
  end;
end;

procedure TMirConfigDlg.RefUseItemConfigClick(Sender:TObject; X, Y:Integer);
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

procedure TMirConfigDlg.RefUseItemConfig(nObj:Integer);
var
  I, VisibleCount, nTop:Integer;
begin
  if not FInitializeed then Exit;

  VisibleCount := 0;
  for I := 0 to Length(g_Config.SuperMedicaItemNames) - 1 do begin
    if g_Config.SuperMedicaItemNames[I] <> '' then Inc(VisibleCount);
  end;

  if nObj in [0..4] then begin

    if nObj = 0 then begin
      PlugMemoConfig4Label3.Caption := '时使用';
      PlugComboBoxCheckHPValue.Visible := True;
      PlugMemoConfig4Label4.Caption := '时使用';
      PlugComboBoxCheckMPValue.Visible := True;

      // 重排下面一块的坐标------------------------------------------------------------
      if (not g_ConfigClient.boCustomUI) or (not g_ConfigClient.boCustomConfigDlg) then begin
        if (g_ClientVersion <> cvMirNewUI205) then begin
          nTop := PlugMemoConfig4Line4.Top;
          nTop := nTop + VisibleCount * 24 + 17;

          if g_ClientConfig.boCloseBookProtect and g_ClientConfig.boCloseLogoutProtect then begin
            PlugCheckBoxAutoPercent.Visible := False;
            PlugMemoConfig4Label1.Visible := False;
            PlugMemoConfig4Line2.Visible := False;
            PlugCheckBoxCheckHPIsAuto.Visible := False;
            PlugEditCheckHPPercent.Visible := False;
            PlugMemoConfig4Label3.Visible := False;

            PlugCheckBoxCheckMPIsAuto.Visible := False;
            PlugEditCheckMPPercent.Visible := False;
            PlugMemoConfig4Label4.Visible := False;

            nTop := nTop - 98;
          end
          else begin
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

            PlugCheckBoxAutoPercent.Visible := True;
            PlugMemoConfig4Label1.Visible := True;
            PlugMemoConfig4Line2.Visible := True;
            PlugCheckBoxCheckHPIsAuto.Visible := True;
            PlugEditCheckHPPercent.Visible := True;
            PlugMemoConfig4Label3.Visible := True;

            PlugCheckBoxCheckMPIsAuto.Visible := True;
            PlugEditCheckMPPercent.Visible := True;
            PlugMemoConfig4Label4.Visible := True;
          end;

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
        end
        else begin
          nTop := PlugMemoConfig4Line4.Top;
          nTop := nTop + VisibleCount * 25 + 20;

          if g_ClientConfig.boCloseBookProtect and g_ClientConfig.boCloseLogoutProtect then begin
            PlugCheckBoxAutoPercent.Visible := False;
            PlugMemoConfig4Label1.Visible := False;
            PlugMemoConfig4Line2.Visible := False;
            PlugCheckBoxCheckHPIsAuto.Visible := False;
            PlugEditCheckHPPercent.Visible := False;
            PlugMemoConfig4Label3.Visible := False;

            PlugCheckBoxCheckMPIsAuto.Visible := False;
            PlugEditCheckMPPercent.Visible := False;
            PlugMemoConfig4Label4.Visible := False;

            nTop := nTop - 110;
          end
          else begin
            PlugCheckBoxAutoPercent.Top := nTop;
            PlugMemoConfig4Label1.Top := nTop + 25;
            PlugMemoConfig4Line2.Top := nTop + 40;

            PlugCheckBoxCheckHPIsAuto.Top := nTop + 50;
            PlugEditCheckHPPercent.Top := nTop + 51;
            PlugMemoConfig4Label3.Top := nTop + 54;
            PlugComboBoxCheckHPValue.Top := nTop + 51;

            PlugCheckBoxCheckMPIsAuto.Top := nTop + 75;
            PlugEditCheckMPPercent.Top := nTop + 76;
            PlugMemoConfig4Label4.Top := nTop + 79;
            PlugComboBoxCheckMPValue.Top := nTop + 76;

            PlugCheckBoxAutoPercent.Visible := True;
            PlugMemoConfig4Label1.Visible := True;
            PlugMemoConfig4Line2.Visible := True;
            PlugCheckBoxCheckHPIsAuto.Visible := True;
            PlugEditCheckHPPercent.Visible := True;
            PlugMemoConfig4Label3.Visible := True;

            PlugCheckBoxCheckMPIsAuto.Visible := True;
            PlugEditCheckMPPercent.Visible := True;
            PlugMemoConfig4Label4.Visible := True;
          end;

          PlugMemoConfig4Label5.Top := nTop + 110;
          PlugMemoConfig4Line5.Top := nTop + 125;

          PlugCheckBoxCheckDuraIsAuto.Top := nTop + 135;
          PlugEditCheckDura.Top := nTop + 136;
          PlugMemoConfig4Label7.Top := nTop + 139;
          PlugEditCheckDuraValue.Top := nTop + 136;

          PlugMemoConfig4Label6.Top := nTop + 165;
          PlugEditCheckDuraTime.Top := nTop + 161;
          PlugMemoConfig4Label8.Top := nTop + 165;

          PlugMemoConfig4LabelHint.Top := nTop + 200;
          PlugMemoConfig4LabelHint2.Top := nTop + 220;
        end;
      end;
    end
    else begin
      PlugMemoConfig4Label3.Caption := '收英雄';
      PlugComboBoxCheckHPValue.Visible := False;
      PlugMemoConfig4Label4.Caption := '收英雄';
      PlugComboBoxCheckMPValue.Visible := False;

      // 重排下面一块的坐标------------------------------------------------------------
      if (not g_ConfigClient.boCustomUI) or (not g_ConfigClient.boCustomConfigDlg) then begin
        if (g_ClientVersion <> cvMirNewUI205) then begin
          nTop := PlugMemoConfig4Line4.Top;
          nTop := nTop + VisibleCount * 24 + 17;

          if g_ClientConfig.boCloseLogoutProtect then begin
            PlugCheckBoxAutoPercent.Visible := False;
            PlugMemoConfig4Label1.Visible := False;
            PlugMemoConfig4Line2.Visible := False;
            PlugCheckBoxCheckHPIsAuto.Visible := False;
            PlugEditCheckHPPercent.Visible := False;
            PlugMemoConfig4Label3.Visible := False;

            PlugCheckBoxCheckMPIsAuto.Visible := False;
            PlugEditCheckMPPercent.Visible := False;
            PlugMemoConfig4Label4.Visible := False;

            nTop := nTop - 98;
          end
          else begin
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

            PlugCheckBoxAutoPercent.Visible := True;
            PlugMemoConfig4Label1.Visible := True;
            PlugMemoConfig4Line2.Visible := True;
            PlugCheckBoxCheckHPIsAuto.Visible := True;
            PlugEditCheckHPPercent.Visible := True;
            PlugMemoConfig4Label3.Visible := True;

            PlugCheckBoxCheckMPIsAuto.Visible := True;
            PlugEditCheckMPPercent.Visible := True;
            PlugMemoConfig4Label4.Visible := True;
          end;

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
        end
        else begin
          nTop := PlugMemoConfig4Line4.Top;
          nTop := nTop + VisibleCount * 25 + 20;

          if g_ClientConfig.boCloseLogoutProtect then begin
            PlugCheckBoxAutoPercent.Visible := False;
            PlugMemoConfig4Label1.Visible := False;
            PlugMemoConfig4Line2.Visible := False;
            PlugCheckBoxCheckHPIsAuto.Visible := False;
            PlugEditCheckHPPercent.Visible := False;
            PlugMemoConfig4Label3.Visible := False;

            PlugCheckBoxCheckMPIsAuto.Visible := False;
            PlugEditCheckMPPercent.Visible := False;
            PlugMemoConfig4Label4.Visible := False;

            nTop := nTop - 110;
          end
          else begin
            PlugCheckBoxAutoPercent.Top := nTop;
            PlugMemoConfig4Label1.Top := nTop + 25;
            PlugMemoConfig4Line2.Top := nTop + 40;

            PlugCheckBoxCheckHPIsAuto.Top := nTop + 50;
            PlugEditCheckHPPercent.Top := nTop + 51;
            PlugMemoConfig4Label3.Top := nTop + 54;
            PlugComboBoxCheckHPValue.Top := nTop + 51;

            PlugCheckBoxCheckMPIsAuto.Top := nTop + 75;
            PlugEditCheckMPPercent.Top := nTop + 76;
            PlugMemoConfig4Label4.Top := nTop + 79;
            PlugComboBoxCheckMPValue.Top := nTop + 76;

            PlugCheckBoxAutoPercent.Visible := True;
            PlugMemoConfig4Label1.Visible := True;
            PlugMemoConfig4Line2.Visible := True;
            PlugCheckBoxCheckHPIsAuto.Visible := True;
            PlugEditCheckHPPercent.Visible := True;
            PlugMemoConfig4Label3.Visible := True;

            PlugCheckBoxCheckMPIsAuto.Visible := True;
            PlugEditCheckMPPercent.Visible := True;
            PlugMemoConfig4Label4.Visible := True;
          end;

          PlugMemoConfig4Label5.Top := nTop + 110;
          PlugMemoConfig4Line5.Top := nTop + 125;

          PlugCheckBoxCheckDuraIsAuto.Top := nTop + 135;
          PlugEditCheckDura.Top := nTop + 136;
          PlugMemoConfig4Label7.Top := nTop + 139;
          PlugEditCheckDuraValue.Top := nTop + 136;

          PlugMemoConfig4Label6.Top := nTop + 165;
          PlugEditCheckDuraTime.Top := nTop + 161;
          PlugMemoConfig4Label8.Top := nTop + 165;

          PlugMemoConfig4LabelHint.Top := nTop + 200;
          PlugMemoConfig4LabelHint2.Top := nTop + 220;
        end;
      end;
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

procedure TMirConfigDlg.RefKeyBoardConfig;
begin
  PlugMemoConfig6LabelKeyBoard1.Caption := GetKeyDownStr(g_ShortcutKeys[0].Key, g_ShortcutKeys[0].Shift, True);
  PlugMemoConfig6LabelKeyBoard2.Caption := GetKeyDownStr(g_ShortcutKeys[1].Key, g_ShortcutKeys[1].Shift, True);
  PlugMemoConfig6LabelKeyBoard3.Caption := GetKeyDownStr(g_ShortcutKeys[2].Key, g_ShortcutKeys[2].Shift, True);
  PlugMemoConfig6LabelKeyBoard4.Caption := GetKeyDownStr(g_ShortcutKeys[3].Key, g_ShortcutKeys[3].Shift, True);
  PlugMemoConfig6LabelKeyBoard5.Caption := GetKeyDownStr(g_ShortcutKeys[4].Key, g_ShortcutKeys[4].Shift, True);
  PlugMemoConfig6LabelKeyBoard6.Caption := GetKeyDownStr(g_ShortcutKeys[5].Key, g_ShortcutKeys[5].Shift, True);
  PlugMemoConfig6LabelKeyBoard7.Caption := GetKeyDownStr(g_ShortcutKeys[6].Key, g_ShortcutKeys[6].Shift, True);
  PlugMemoConfig6LabelKeyBoard8.Caption := GetKeyDownStr(g_ShortcutKeys[7].Key, g_ShortcutKeys[7].Shift, True);
  PlugMemoConfig6LabelKeyBoard9.Caption := GetKeyDownStr(g_ShortcutKeys[8].Key, g_ShortcutKeys[9].Shift, True);
  PlugMemoConfig6LabelKeyBoard10.Caption := GetKeyDownStr(g_ShortcutKeys[9].Key, g_ShortcutKeys[9].Shift, True);
  PlugMemoConfig6LabelKeyBoard11.Caption := GetKeyDownStr(g_ShortcutKeys[10].Key, g_ShortcutKeys[10].Shift, True);
  PlugMemoConfig6LabelKeyBoard12.Caption := GetKeyDownStr(g_ShortcutKeys[11].Key, g_ShortcutKeys[11].Shift, True);
  PlugMemoConfig6LabelKeyBoard13.Caption := GetKeyDownStr(g_ShortcutKeys[12].Key, g_ShortcutKeys[12].Shift, True);
  PlugMemoConfig6LabelKeyBoard14.Caption := GetKeyDownStr(g_ShortcutKeys[13].Key, g_ShortcutKeys[13].Shift, True);
  PlugMemoConfig6LabelKeyBoard15.Caption := GetKeyDownStr(g_ShortcutKeys[14].Key, g_ShortcutKeys[14].Shift, True);
  PlugMemoConfig6LabelKeyBoard16.Caption := GetKeyDownStr(g_ShortcutKeys[15].Key, g_ShortcutKeys[15].Shift, True);
end;

procedure TMirConfigDlg.RefConfig;
begin
  if not FInitializeed then Exit;

  //HZQ 20230601 新加火墙淡化、隐藏怪物顶戴、自动绕行选项
  PlugCheckBoxHideMonsterIcons.Checked := FConfigCheckeds[ckHideMonsterIcons];
  PlugCheckBoxAutoDetourPath.Checked := FConfigCheckeds[ckAutoDetourPath];
  PlugCheckBoxDimFireEffect.Checked := FConfigCheckeds[ckDimFireEffect];
  PlugCheckBoxNearEffect.Checked := FConfigCheckeds[ckObjectHintEffect];

  PlugCheckBoxShowHPLabel.Checked := FConfigCheckeds[ckShowHPLabel];
  PlugCheckBoxNumberLable.Checked := FConfigCheckeds[ckShowNumberLable];
  PlugCheckBoxJobAndLevel.Checked := FConfigCheckeds[ckShowJobAndLevel];
  PlugCheckBoxShowGreenHint.Checked := FConfigCheckeds[ckShowGreenHint];
  PlugCheckBoxDisableSelfStruck.Checked := FConfigCheckeds[ckDisableSelfStruck];
  PlugCheckBoxSpeedSlow.Checked := FConfigCheckeds[ckSpeedSlow];
  PlugCheckBoxBGMusic.Checked := FConfigCheckeds[ckBGMusic];
  PlugCheckBoxPickAll.Checked := FConfigCheckeds[ckPickupAll];
  PlugCheckBoxAutoPickUpItem.Checked := FConfigCheckeds[ckAutoPickUpItem];
  PlugCheckBoxNoCaton.Checked := FConfigCheckeds[ckNoCaton];

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
  PlugCheckBoxHideActorIcons.Checked := FConfigCheckeds[ckHideActorIcons];
  PlugCheckBoxShowMonName.Checked := FConfigCheckeds[ckShowMonName];
  PlugCheckBoxShowNpcName.Checked := FConfigCheckeds[ckShowNpcName]; // 显示NPC名 piaoyun 2013-07-31
  PlugCheckBoxShowNpcHPLabel.Checked := FConfigCheckeds[ckShowNpcHPLabel]; // 显示NPC血条 piaoyun 2013-07-31
  PlugCheckBoxShowNGLabel.Checked := FConfigCheckeds[ckShowNGLabel]; // 显示内功黄条 piaoyun 2013-09-09
  PlugCheckBoxNearHint.Checked := FConfigCheckeds[ckNearHint]; // 接近提示 piaoyun 2013-09-09
  PlugCheckBoxAutoLock.Checked := FConfigCheckeds[ckAutoLock]; // 自动锁定 piaoyun 2013-09-09
  PlugCheckBoxColorShow.Checked := FConfigCheckeds[ckColorShow]; // 变色显示 piaoyun 2013-09-09
  PlugCheckBoxSpecialQuickFlashing.Checked := FConfigCheckeds[ckSpecialQuickFlashing]; // 特殊物品快闪 piaoyun 2013-09-10

  PlugCheckBoxBlacklistHit.Checked := FConfigCheckeds[ckBlacklistHit]; // 黑名单近身提示 piaoyun 2013-09-11
  PlugCheckBoxFriendHit.Checked := FConfigCheckeds[ckFriendHit]; // 好友近身提示 piaoyun 2013-09-11
  PlugCheckBoxSceneShake.Checked := ConfigCheckeds[ckSceneShake]; // 屏幕震动 piaoyun 2013-09-14
  PlugCheckBoxAutoDownHorse.Checked := FConfigCheckeds[ckAutoDownHorse]; // 魔法攻击自动下马 chongchong 2013-10-19

  PlugCheckBoxAutoOrderItem.Checked := FConfigCheckeds[ckAutoOrderItem];
  PlugCheckBoxMagicLock.Checked := FConfigCheckeds[ckMagicLock];

  PlugCheckBoxBGMusic.Checked := FConfigCheckeds[ckBGMusic];
  PlugCheckBoxRepeatBGMusic.Checked := FConfigCheckeds[ckRepeatBGMusic];
  PlugCheckDisableChartMemoSize.Checked := FConfigCheckeds[ckDisableChartMemoSize];
  PlugCheckBoxItemCmp.Checked := FConfigCheckeds[ckItemCompare];
  PlugCheckBoxBagFastItemCmp.Checked := FConfigCheckeds[ckBagFastItemCompare];
  PlugCheckBoxHideItemEffect.Checked := FConfigCheckeds[ckHideItemEffect];

  PlugCheckBoxVolume.Checked := FConfigCheckeds[ckVolume];
  PlugCheckBoxHideBigHPProgress.Checked := FConfigCheckeds[ckHideBigHPProgress];
  g_boSound := PlugCheckBoxVolume.Checked;

  TrackBarVolume.Max := 100;
  TrackBarVolume.Min := 0;
  TrackBarVolume.Position := g_SoundVolume;

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
  PlugCheckBoxAutoChangePoison.Checked := FConfigCheckeds[ckAutoChangePoison];
  PlugCheckBoxHumAutoShield.Checked := FConfigCheckeds[ckHumAutoShield];
  PlugCheckBoxSmart113Hit.Checked := FConfigCheckeds[ckSmart113Hit];

  PlugCheckBoxAutoCustomHit1.Checked := FConfigCheckeds[ckSmartCustomHit1];
  PlugCheckBoxAutoCustomHit2.Checked := FConfigCheckeds[ckSmartCustomHit2];
  PlugCheckBoxAutoCustomHit3.Checked := FConfigCheckeds[ckSmartCustomHit3];
  PlugCheckBoxAutoCustomHit4.Checked := FConfigCheckeds[ckSmartCustomHit4];
  PlugCheckBoxAutoCustomHit5.Checked := FConfigCheckeds[ckSmartCustomHit5];
  PlugCheckBoxAutoCustomHit6.Checked := FConfigCheckeds[ckSmartCustomHit6];
  PlugCheckBoxAutoCustomHit7.Checked := FConfigCheckeds[ckSmartCustomHit7];
  PlugCheckBoxAutoCustomHit8.Checked := FConfigCheckeds[ckSmartCustomHit8];

  PlugCheckBoxHumManuallyCustomHit1.Checked := FConfigCheckeds[ckHumManuallyCustomHit1];
  PlugCheckBoxHumManuallyCustomHit2.Checked := FConfigCheckeds[ckHumManuallyCustomHit2];
  PlugCheckBoxHumManuallyCustomHit3.Checked := FConfigCheckeds[ckHumManuallyCustomHit3];
  PlugCheckBoxHumManuallyCustomHit4.Checked := FConfigCheckeds[ckHumManuallyCustomHit4];
  PlugCheckBoxHumManuallyCustomHit5.Checked := FConfigCheckeds[ckHumManuallyCustomHit5];

  PlugCheckBoxShowValueItemEffect.Checked := FConfigCheckeds[ckShowValueItemEffect];

  PlugCheckBoxAutoOpenSpell.Checked := FConfigCheckeds[ckAutoOpenSpell];
  PlugCheckBoxAutoGroupAttack.Checked := FConfigCheckeds[ckAutoGroupAttack];
  PlugCheckBoxAutoGroupNoAttackMon.Checked := FConfigCheckeds[ckAutoGroupNoAttackMon];

  PlugCheckBoxAutoContinueAttack.Checked := FConfigCheckeds[ckAutoContinueAttack];

  PlugCheckBoxHumStruckShield.Checked := FConfigCheckeds[ckHumStruckShield];
  PlugCheckBoxHeroAutoShield.Checked := FConfigCheckeds[ckHeroAutoShield];
  PlugCheckBoxAssistantHeroAutoShield.Checked := FConfigCheckeds[ckAssistantHeroAutoShield];
  PlugCheckBoxHumManuallySnowWind.Checked := FConfigCheckeds[ckHumManuallySnowWind];
  PlugCheckBoxHumManuallyFireBoom.Checked := FConfigCheckeds[ckHumManuallyFireBoom];
  PlugCheckBoxHumShootLightenLockTarget.Checked := FConfigCheckeds[ckHumShootLightenLockTarget];
  PlugCheckBoxHumManuallyMeteorShower.Checked := FConfigCheckeds[ckHumManuallyMeteorShower];
  PlugCheckBoxHumManuallyMove10Attack.Checked := FConfigCheckeds[ckHumManuallyMove10Attack];

  PlugCheckBoxHeroContinuousNoHitMon.Checked := FConfigCheckeds[ckHeroContinuousNoHitMon];
  PlugCheckBoxHeroShowNumberState.Checked := FConfigCheckeds[ckHeroShowNumberState];

  PlugCheckBoxNoRedPoison.Checked := FConfigCheckeds[ckGJ_NoRedPoison];
  PlugCheckBoxNoBluePoison.Checked := FConfigCheckeds[ckGJ_NoBluePoison];

  PlugCheckBoxAutoMagic.Checked := FConfigCheckeds[ckAutoUseMagic];
  PlugCheckBoxUseKeyBoard.Checked := FConfigCheckeds[ckUseKeyBoard];

  PlugCheckBoxUseSuperMedica.Checked := FConfigCheckeds[ckUseSuperMedica];

  PlugCheckBoxAutoMagic.Checked := FConfigCheckeds[ckAutoUseMagic];

  PlugCheckBoxHideTitle.Checked := FConfigCheckeds[ckHideTitle];
  PlugCheckBoxContinueButchItem.Checked := FConfigCheckeds[ckContinueButchItem];
  PlugCheckBoxDisableDeal.Checked := FConfigCheckeds[ckDisableDeal];

  PlugCheckBoxUpdateStatus.Checked := FConfigCheckeds[ckShowUpdateStatus];

  PlugEditExpFilter.Value := g_Config.nFilterMinExp;
  PlugEditAutoMagicTime.Value := g_Config.nAutoUseMagicTime;
  PlugComboBoxColorShow.ItemIndex := g_Config.nColorShowEff; // BOSS变色显示 piaoyun 2013-09-09
  frmMain.nColorShowEff := g_Config.nColorShowEff;

  PlugEditSpecialColor.Value := g_Config.nSpecialColor;
  PlugLabelSpecialColor.CaptionColor.Up.Color := GetRGB(g_Config.nSpecialColor);
  frmMain.nSpecialColor := g_Config.nSpecialColor;

  PlugCheckBoxPlayAttack.Checked := FConfigCheckeds[ckGJ_PlayAttack]; // 挂机 - 受玩家攻击

  PlugComboBoxPlayAttackValue.ItemIndex := g_Config.nGJPlayAttackOption; // 挂机 - 受玩家攻击后的操作
  FrmMain.nGJPlayAttackOption := g_Config.nGJPlayAttackOption;

  PlugComboBoxNoRedPoisonValue.ItemIndex := g_Config.nGJNoRedPoisonOption; // 挂机 - 红药用完后动作 chongchong 2014-12-06
  FrmMain.nGJNoRedPoisonOption := g_Config.nGJNoRedPoisonOption;

  PlugComboBoxNoBluePoisonValue.ItemIndex := g_Config.nGJNoBluePoisonOption; // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
  FrmMain.nGJNoBluePoisonOption := g_Config.nGJNoBluePoisonOption;

  PlugComboBoxNoDuFuValue.ItemIndex := g_Config.nGJNoDuFuOption; // 挂机 - 毒符用完后动作 chongchong 2014-12-06
  FrmMain.nGJNoDuFuOption := g_Config.nGJNoDuFuOption;

  PlugComboBoxBagFullValue.ItemIndex := g_Config.nGJBagFullOption; // 挂机 - 包裹满后动作 chongchong 2014-12-06
  FrmMain.nGJBagFullOption := g_Config.nGJBagFullOption;

  PlugCheckBoxNotRushMon.Checked := FConfigCheckeds[ckGJ_NotRushMon]; // 挂机 - 不抢怪
  PlugEditNotRushMonRange.Value := g_Config.nGJNotRushMonRange; // 挂机 - 怪物周围几格有玩家
  FrmMain.nGJNotRushMonRange := g_Config.nGJNotRushMonRange;

  PlugCheckBoxNoDuFu.Checked := FConfigCheckeds[ckGJ_NoDuFu];
  PlugCheckBoxBagFull.Checked := FConfigCheckeds[ckGJ_BagFull];
  PlugCheckBoxAutoPickup.Checked := FConfigCheckeds[ckGJ_AutoPickup];

  PlugCheckBoxGroupAttack.Checked := FConfigCheckeds[ckGJ_GroupAttack];
  PlugEditNotGroupAttackCount.Value := g_Config.nGJGroupAttackCount;
  FrmMain.nGJGroupAttackCount := g_Config.nGJGroupAttackCount;
  PlugCheckBoxLimitScreen.Checked := FConfigCheckeds[ckGJ_LimitScreen];
  PlugCheckBoxDFAvoid.Checked := FConfigCheckeds[ckGJ_DFStopAvoid];
  PlugCheckSimpleShowActor.Checked := FConfigCheckeds[ckSimpleShowActor];
  PlugCheckSimpleShowHumanDress.Checked := FConfigCheckeds[ckSimpleShowHumanDress];
  PlugCheckSimpleShowHumanWeapon.Checked := FConfigCheckeds[ckSimpleShowHumanWeapon];
  PlugCheckSimpleShowBB.Checked := FConfigCheckeds[ckSimpleShowBB];

  PlugCheckBoxShowHPUnit.Checked := FConfigCheckeds[ckShowHPUnit];
  PlugCheckBoxHumManuallyFire.Checked := FConfigCheckeds[ckHumManuallyFire];

  PlugCheckBoxShowTargetAperture.Checked := FConfigCheckeds[ckShowTargetAperture];

  RefKeyBoardConfig;
  PlugMemoConfig4Button1.Checked := g_Config.MedicaMode = 0;
  PlugMemoConfig4Button2.Checked := g_Config.MedicaMode = 1;
  PlugMemoConfig4Button3.Checked := g_Config.MedicaMode = 2;
  PlugMemoConfig4Button4.Checked := g_Config.MedicaMode = 3;
  PlugMemoConfig4Button5.Checked := g_Config.MedicaMode = 4;
  RefUseItemConfig(g_Config.MedicaMode);

  RefreshUnBindItemList;
end;

procedure TMirConfigDlg.DEditCheckHPPercentChange(Sender:TObject);
begin
  g_Config.CheckHpPercents[g_Config.MedicaMode] := PlugEditCheckHPPercent.Value;
end;

procedure TMirConfigDlg.PlugEditHeroDodgeHPPercentChange(Sender:TObject);
begin
  g_Config.nHeroDodgeHPPercent := PlugEditHeroDodgeHPPercent.Value;
  frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
end;

procedure TMirConfigDlg.DEditCheckMPPercentChange(Sender:TObject);
begin
  g_Config.CheckMpPercents[g_Config.MedicaMode] := PlugEditCheckMPPercent.Value;
end;

procedure TMirConfigDlg.ComboBoxCheckHPValueChange(Sender:TObject);
begin
  g_Config.CheckHpValues[g_Config.MedicaMode] := PlugComboBoxCheckHPValue.ItemIndex;
end;

procedure TMirConfigDlg.ComboBoxCheckMPValueChange(Sender:TObject);
begin
  g_Config.CheckMpValues[g_Config.MedicaMode] := PlugComboBoxCheckMPValue.ItemIndex;
end;

procedure TMirConfigDlg.DCheckBoxCheckHPIsAutoClick(Sender:TObject; X, Y:Integer);
begin
  g_Config.CheckHpIsAutos[g_Config.MedicaMode] := PlugCheckBoxCheckHPIsAuto.Checked;
end;

procedure TMirConfigDlg.DCheckBoxCheckMPIsAutoClick(Sender:TObject; X, Y:Integer);
begin
  g_Config.CheckMpIsAutos[g_Config.MedicaMode] := PlugCheckBoxCheckMPIsAuto.Checked;
end;

procedure TMirConfigDlg.DCheckBoxCheckDuraIsAuto(Sender:TObject; X, Y:Integer);
begin
  g_Config.CheckDuraIsAutos[g_Config.MedicaMode] := PlugCheckBoxCheckDuraIsAuto.Checked;
end;

procedure TMirConfigDlg.DEditCheckDuraChange(Sender:TObject);
begin
  g_Config.CheckDuraMin[g_Config.MedicaMode] := Max(1, PlugEditCheckDura.Value);
  PlugEditCheckDura.Value := g_Config.CheckDuraMin[g_Config.MedicaMode];
end;

procedure TMirConfigDlg.DEditCheckDuraValueChange(Sender:TObject);
begin
  g_Config.CheckDuraValue[g_Config.MedicaMode] := PlugEditCheckDuraValue.Text;
end;

procedure TMirConfigDlg.DEditCheckDuraTimeChange(Sender:TObject);
begin
  g_Config.CheckDuraTime[g_Config.MedicaMode] := Max(2, PlugEditCheckDuraTime.Value);
  PlugEditCheckDuraTime.Value := g_Config.CheckDuraTime[g_Config.MedicaMode];
end;

procedure TMirConfigDlg.DEditRenewHPPercentChange(Sender:TObject);
begin
  g_Config.RenewHPPercents[g_Config.MedicaMode] := PlugEditRenewHPPercent.Value;
end;

procedure TMirConfigDlg.DEditRenewMPPercentChange(Sender:TObject);
begin
  g_Config.RenewMPPercents[g_Config.MedicaMode] := PlugEditRenewMPPercent.Value;
end;

procedure TMirConfigDlg.DEditRenewSpecialHPPercentChange(Sender:TObject);
begin
  g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] := PlugEditRenewSpecialHPPercent.Value;
end;

procedure TMirConfigDlg.DEditRenewSpecialMPPercentChange(Sender:TObject);
begin
  g_Config.RenewSpecialMPPercents[g_Config.MedicaMode] := PlugEditRenewSpecialMPPercent.Value;
end;

procedure TMirConfigDlg.DEditRenewHPTimeChange(Sender:TObject);
begin
  g_Config.RenewHPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwPluginMinEatItemTime, PlugEditRenewHPTime.Value);
  PlugEditRenewHPTime.Value := g_Config.RenewHPTimes[g_Config.MedicaMode];
end;

procedure TMirConfigDlg.DEditRenewMPTimeChange(Sender:TObject);
begin
  g_Config.RenewMPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwPluginMinEatItemTime, PlugEditRenewMPTime.Value);
  PlugEditRenewMPTime.Value := g_Config.RenewMPTimes[g_Config.MedicaMode];
end;

procedure TMirConfigDlg.DEditRenewSpecialHPTimeChange(Sender:TObject);
begin
  g_Config.RenewSpecialHPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwPluginMinEatItemTime, PlugEditRenewSpecialHPTime.Value);
  PlugEditRenewSpecialHPTime.Value := g_Config.RenewSpecialHPTimes[g_Config.MedicaMode];
end;

procedure TMirConfigDlg.DEditRenewSpecialMPTimeChange(Sender:TObject);
begin
  g_Config.RenewSpecialMPTimes[g_Config.MedicaMode] := Max(g_ClientConfig.dwPluginMinEatItemTime, PlugEditRenewSpecialMPTime.Value);
  PlugEditRenewSpecialMPTime.Value := g_Config.RenewSpecialMPTimes[g_Config.MedicaMode];
end;

procedure TMirConfigDlg.DCheckBoxRenewHPIsAutoClick(Sender:TObject; X, Y:Integer);
begin
  g_Config.RenewHPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewHPIsAuto.Checked;
end;

procedure TMirConfigDlg.DCheckBoxRenewMPIsAutoClick(Sender:TObject; X, Y:Integer);
begin
  g_Config.RenewMPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewMPIsAuto.Checked;
end;

procedure TMirConfigDlg.DCheckBoxRenewSpecialHPIsAutoClick(Sender:TObject; X, Y:Integer);
begin
  g_Config.RenewSpecialHPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewSpecialHPIsAuto.Checked;
end;

procedure TMirConfigDlg.DCheckBoxRenewSpecialMPIsAutoClick(Sender:TObject; X, Y:Integer);
begin
  g_Config.RenewSpecialMPIsAutos[g_Config.MedicaMode] := PlugCheckBoxRenewSpecialMPIsAuto.Checked;
end;

procedure TMirConfigDlg.DEditSuperMedicaHPChange(Sender:TObject);
var
  Index, Value:Integer;
begin
  Index := -1;
  Value := 0; //HZQ 20230525
  if PlugEditSuperMedicaHP0 = Sender then begin
    Index := 0;
    Value := PlugEditSuperMedicaHP0.Value;
  end else if PlugEditSuperMedicaHP1 = Sender then begin
    Index := 1;
    Value := PlugEditSuperMedicaHP1.Value;
  end else if PlugEditSuperMedicaHP2 = Sender then begin
    Index := 2;
    Value := PlugEditSuperMedicaHP2.Value;
  end else if PlugEditSuperMedicaHP3 = Sender then begin
    Index := 3;
    Value := PlugEditSuperMedicaHP3.Value;
  end else if PlugEditSuperMedicaHP4 = Sender then begin
    Index := 4;
    Value := PlugEditSuperMedicaHP4.Value;
  end else if PlugEditSuperMedicaHP5 = Sender then begin
    Index := 5;
    Value := PlugEditSuperMedicaHP5.Value;
  end else if PlugEditSuperMedicaHP6 = Sender then begin
    Index := 6;
    Value := PlugEditSuperMedicaHP6.Value;
  end else if PlugEditSuperMedicaHP7 = Sender then begin
    Index := 7;
    Value := PlugEditSuperMedicaHP7.Value;
  end else if PlugEditSuperMedicaHP8 = Sender then begin
    Index := 8;
    Value := PlugEditSuperMedicaHP8.Value;
  end;
  if Index in [0..8] then begin
    g_Config.SuperMedicaHPs[g_Config.MedicaMode][Index] := Value;
  end;
end;

procedure TMirConfigDlg.DEditSuperMedicaHPTimeChange(Sender:TObject);
var
  Index, Value:Integer;
begin
  Index := -1;
  Value := 0; //HZQ 20230525
  if PlugEditSuperMedicaHPTime0 = Sender then begin
    Index := 0;
    Value := PlugEditSuperMedicaHPTime0.Value;
  end
  else if PlugEditSuperMedicaHPTime1 = Sender then begin
    Index := 1;
    Value := PlugEditSuperMedicaHPTime1.Value;
  end
  else if PlugEditSuperMedicaHPTime2 = Sender then begin
    Index := 2;
    Value := PlugEditSuperMedicaHPTime2.Value;
  end
  else if PlugEditSuperMedicaHPTime3 = Sender then begin
    Index := 3;
    Value := PlugEditSuperMedicaHPTime3.Value;
  end
  else if PlugEditSuperMedicaHPTime4 = Sender then begin
    Index := 4;
    Value := PlugEditSuperMedicaHPTime4.Value;
  end
  else if PlugEditSuperMedicaHPTime5 = Sender then begin
    Index := 5;
    Value := PlugEditSuperMedicaHPTime5.Value;
  end
  else if PlugEditSuperMedicaHPTime6 = Sender then begin
    Index := 6;
    Value := PlugEditSuperMedicaHPTime6.Value;
  end
  else if PlugEditSuperMedicaHPTime7 = Sender then begin
    Index := 7;
    Value := PlugEditSuperMedicaHPTime7.Value;
  end
  else if PlugEditSuperMedicaHPTime8 = Sender then begin
    Index := 8;
    Value := PlugEditSuperMedicaHPTime8.Value;
  end;
  if Index in [0..8] then begin
    g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][Index] := Max(g_ClientConfig.dwPluginMinEatItemTime, Value);
    TDxEdit(Sender).Value := g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][Index];
  end;
end;

procedure TMirConfigDlg.DEditSuperMedicaMPChange(Sender:TObject);
var
  Index, Value:Integer;
begin
  Index := -1;
  Value := 0; //HZQ 20230525
  if PlugEditSuperMedicaMP0 = Sender then begin
    Index := 0;
    Value := PlugEditSuperMedicaMP0.Value;
  end
  else if PlugEditSuperMedicaMP1 = Sender then begin
    Index := 1;
    Value := PlugEditSuperMedicaMP1.Value;
  end
  else if PlugEditSuperMedicaMP2 = Sender then begin
    Index := 2;
    Value := PlugEditSuperMedicaMP2.Value;
  end
  else if PlugEditSuperMedicaMP3 = Sender then begin
    Index := 3;
    Value := PlugEditSuperMedicaMP3.Value;
  end
  else if PlugEditSuperMedicaMP4 = Sender then begin
    Index := 4;
    Value := PlugEditSuperMedicaMP4.Value;
  end
  else if PlugEditSuperMedicaMP5 = Sender then begin
    Index := 5;
    Value := PlugEditSuperMedicaMP5.Value;
  end
  else if PlugEditSuperMedicaMP6 = Sender then begin
    Index := 6;
    Value := PlugEditSuperMedicaMP6.Value;
  end
  else if PlugEditSuperMedicaMP7 = Sender then begin
    Index := 7;
    Value := PlugEditSuperMedicaMP7.Value;
  end
  else if PlugEditSuperMedicaMP8 = Sender then begin
    Index := 8;
    Value := PlugEditSuperMedicaMP8.Value;
  end;

  if Index in [0..8] then begin
    g_Config.SuperMedicaMPs[g_Config.MedicaMode][Index] := Value;
  end;
end;

procedure TMirConfigDlg.DEditSuperMedicaMPTimeChange(Sender:TObject);
var
  Index, Value:Integer;
begin
  Index := -1;
  Value := 0; //HZQ 20230525
  if PlugEditSuperMedicaMPTime0 = Sender then begin
    Index := 0;
    Value := PlugEditSuperMedicaMPTime0.Value;
  end
  else if PlugEditSuperMedicaMPTime1 = Sender then begin
    Index := 1;
    Value := PlugEditSuperMedicaMPTime1.Value;
  end
  else if PlugEditSuperMedicaMPTime2 = Sender then begin
    Index := 2;
    Value := PlugEditSuperMedicaMPTime2.Value;
  end
  else if PlugEditSuperMedicaMPTime3 = Sender then begin
    Index := 3;
    Value := PlugEditSuperMedicaMPTime3.Value;
  end
  else if PlugEditSuperMedicaMPTime4 = Sender then begin
    Index := 4;
    Value := PlugEditSuperMedicaMPTime4.Value;
  end
  else if PlugEditSuperMedicaMPTime5 = Sender then begin
    Index := 5;
    Value := PlugEditSuperMedicaMPTime5.Value;
  end
  else if PlugEditSuperMedicaMPTime6 = Sender then begin
    Index := 6;
    Value := PlugEditSuperMedicaMPTime6.Value;
  end
  else if PlugEditSuperMedicaMPTime7 = Sender then begin
    Index := 7;
    Value := PlugEditSuperMedicaMPTime7.Value;
  end
  else if PlugEditSuperMedicaMPTime8 = Sender then begin
    Index := 8;
    Value := PlugEditSuperMedicaMPTime8.Value;
  end;
  if Index in [0..8] then begin
    g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][Index] := Max(g_ClientConfig.dwPluginMinEatItemTime, Value);
    TDxEdit(Sender).Value := g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][Index];
  end;
end;

procedure TMirConfigDlg.DCheckBoxUseSuperMedicaItemNameClick(Sender:TObject; X, Y:Integer);
var
  Index:Integer;
  Value:Boolean;
begin
  Index := -1;
  Value := False; //HZQ 20230525
  if PlugCheckBoxUseSuperMedicaItemName0 = Sender then begin
    Index := 0;
    Value := PlugCheckBoxUseSuperMedicaItemName0.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName1 = Sender then begin
    Index := 1;
    Value := PlugCheckBoxUseSuperMedicaItemName1.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName2 = Sender then begin
    Index := 2;
    Value := PlugCheckBoxUseSuperMedicaItemName2.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName3 = Sender then begin
    Index := 3;
    Value := PlugCheckBoxUseSuperMedicaItemName3.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName4 = Sender then begin
    Index := 4;
    Value := PlugCheckBoxUseSuperMedicaItemName4.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName5 = Sender then begin
    Index := 5;
    Value := PlugCheckBoxUseSuperMedicaItemName5.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName6 = Sender then begin
    Index := 6;
    Value := PlugCheckBoxUseSuperMedicaItemName6.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName7 = Sender then begin
    Index := 7;
    Value := PlugCheckBoxUseSuperMedicaItemName7.Checked;
  end
  else if PlugCheckBoxUseSuperMedicaItemName8 = Sender then begin
    Index := 8;
    Value := PlugCheckBoxUseSuperMedicaItemName8.Checked;
  end;
  if Index in [0..8] then begin
    g_Config.SuperMedicaUses[g_Config.MedicaMode][Index] := Value;
  end;
end;

procedure TMirConfigDlg.DEditChange(Sender:TObject);
begin
  if PlugEditExpFilter = Sender then begin
    g_Config.nFilterMinExp := PlugEditExpFilter.Value;
  end
  else if PlugEditAutoMagicTime = Sender then begin
    g_Config.nAutoUseMagicTime := PlugEditAutoMagicTime.Value;
  end;
end; 

function TMirConfigDlg.MakeControlAddressList: THashedStringList;
begin
  Result := THashedStringList.Create;
  Result.AddObject('PlugConfigDlg', Pointer(@PlugConfigDlg));
  Result.AddObject('PlugConfigDlgClose', Pointer(@PlugConfigDlgClose));
  Result.AddObject('PlugPageControlConfig', Pointer(@PlugPageControlConfig));
  Result.AddObject('PlugTabSheetConfig1', Pointer(@PlugTabSheetConfig1));
  Result.AddObject('PlugMemoConfig1', Pointer(@PlugMemoConfig1));
  Result.AddObject('PLugUIScaleLabel', Pointer(@PLugUIScaleLabel));
  Result.AddObject('PlugCheckBoxAutoDetourPath', Pointer(@PlugCheckBoxAutoDetourPath));
  Result.AddObject('PlugCheckBoxAutoOrderItem', Pointer(@PlugCheckBoxAutoOrderItem));
  Result.AddObject('PlugCheckBoxAutoPickUpItem', Pointer(@PlugCheckBoxAutoPickUpItem));
  Result.AddObject('PlugCheckBoxBGMusic', Pointer(@PlugCheckBoxBGMusic));
  Result.AddObject('PlugCheckBoxContinueButchItem', Pointer(@PlugCheckBoxContinueButchItem));
  Result.AddObject('PlugCheckBoxDimFireEffect', Pointer(@PlugCheckBoxDimFireEffect));
  Result.AddObject('PlugCheckBoxDisableDeal', Pointer(@PlugCheckBoxDisableDeal));
  Result.AddObject('PlugCheckBoxDuraWarning', Pointer(@PlugCheckBoxDuraWarning));
  Result.AddObject('PlugCheckBoxExpFilter', Pointer(@PlugCheckBoxExpFilter));
  Result.AddObject('PlugCheckBoxHideActorIcons', Pointer(@PlugCheckBoxHideActorIcons));
  Result.AddObject('PlugCheckBoxHideDescUserName', Pointer(@PlugCheckBoxHideDescUserName));
  Result.AddObject('PlugCheckBoxHideGhost', Pointer(@PlugCheckBoxHideGhost));
  Result.AddObject('PlugCheckBoxHideHumEffect', Pointer(@PlugCheckBoxHideHumEffect));
  Result.AddObject('PlugCheckBoxHideMonsterIcons', Pointer(@PlugCheckBoxHideMonsterIcons));
  Result.AddObject('PlugCheckBoxHideTitle', Pointer(@PlugCheckBoxHideTitle));
  Result.AddObject('PlugCheckBoxHideWeaponEffect', Pointer(@PlugCheckBoxHideWeaponEffect));
  Result.AddObject('PlugCheckBoxJobAndLevel', Pointer(@PlugCheckBoxJobAndLevel));
  Result.AddObject('PlugCheckBoxNoShift', Pointer(@PlugCheckBoxNoShift));
  Result.AddObject('PlugCheckBoxNotParaly', Pointer(@PlugCheckBoxNotParaly));
  Result.AddObject('PlugCheckBoxNumberLable', Pointer(@PlugCheckBoxNumberLable));
  Result.AddObject('PlugCheckBoxRepeatBGMusic', Pointer(@PlugCheckBoxRepeatBGMusic));
  Result.AddObject('PlugCheckBoxSceneShake', Pointer(@PlugCheckBoxSceneShake));
  Result.AddObject('PlugCheckBoxShiftSwitch', Pointer(@PlugCheckBoxShiftSwitch));
  Result.AddObject('PlugCheckBoxShowActorName', Pointer(@PlugCheckBoxShowActorName));
  Result.AddObject('PlugCheckBoxShowGreenHint', Pointer(@PlugCheckBoxShowGreenHint));
  Result.AddObject('PlugCheckBoxShowHPLabel', Pointer(@PlugCheckBoxShowHPLabel));
  Result.AddObject('PlugCheckBoxShowHPUnit', Pointer(@PlugCheckBoxShowHPUnit));
  Result.AddObject('PlugCheckBoxShowHealthNumber', Pointer(@PlugCheckBoxShowHealthNumber));
  Result.AddObject('PlugCheckBoxShowHighlightHPLabel', Pointer(@PlugCheckBoxShowHighlightHPLabel));
  Result.AddObject('PlugCheckBoxShowMimiMapDesc', Pointer(@PlugCheckBoxShowMimiMapDesc));
  Result.AddObject('PlugCheckBoxShowMonName', Pointer(@PlugCheckBoxShowMonName));
  Result.AddObject('PlugCheckBoxShowNGLabel', Pointer(@PlugCheckBoxShowNGLabel));
  Result.AddObject('PlugCheckBoxShowNpcHPLabel', Pointer(@PlugCheckBoxShowNpcHPLabel));
  Result.AddObject('PlugCheckBoxShowNpcName', Pointer(@PlugCheckBoxShowNpcName));
  Result.AddObject('PlugCheckBoxSpeedSlow', Pointer(@PlugCheckBoxSpeedSlow));
  Result.AddObject('PlugCheckBoxUpdateStatus', Pointer(@PlugCheckBoxUpdateStatus));
  Result.AddObject('PlugCheckBoxVolume', Pointer(@PlugCheckBoxVolume));
  Result.AddObject('PlugCheckDisableChartMemoSize', Pointer(@PlugCheckDisableChartMemoSize));
  Result.AddObject('PlugCheckSimpleShowActor', Pointer(@PlugCheckSimpleShowActor));
  Result.AddObject('PlugCheckSimpleShowBB', Pointer(@PlugCheckSimpleShowBB));
  Result.AddObject('PlugCheckSimpleShowHumanDress', Pointer(@PlugCheckSimpleShowHumanDress));
  Result.AddObject('PlugCheckSimpleShowHumanWeapon', Pointer(@PlugCheckSimpleShowHumanWeapon));
  Result.AddObject('PlugEditExpFilter', Pointer(@PlugEditExpFilter));
  Result.AddObject('PlugMapScaleBar', Pointer(@PlugMapScaleBar));
  Result.AddObject('PlugMapScaleLabel', Pointer(@PlugMapScaleLabel));
  Result.AddObject('PlugUIScaleBar', Pointer(@PlugUIScaleBar));
  Result.AddObject('TrackBarVolume', Pointer(@TrackBarVolume));
  Result.AddObject('PlugTabSheetConfig2', Pointer(@PlugTabSheetConfig2));
  Result.AddObject('PlugBtnDiyAdd', Pointer(@PlugBtnDiyAdd));
  Result.AddObject('PlugBtnDiyDel', Pointer(@PlugBtnDiyDel));
  Result.AddObject('PlugBtnDiyEdit', Pointer(@PlugBtnDiyEdit));
  Result.AddObject('PlugBtnDiyExport', Pointer(@PlugBtnDiyExport));
  Result.AddObject('PlugBtnDiyImport', Pointer(@PlugBtnDiyImport));
  Result.AddObject('PlugCheckBoxBagFastItemCmp', Pointer(@PlugCheckBoxBagFastItemCmp));
  Result.AddObject('PlugCheckBoxHideItemEffect', Pointer(@PlugCheckBoxHideItemEffect));
  Result.AddObject('PlugCheckBoxItemCmp', Pointer(@PlugCheckBoxItemCmp));
  Result.AddObject('PlugCheckBoxPickAll', Pointer(@PlugCheckBoxPickAll));
  Result.AddObject('PlugCheckBoxShowValueItemEffect', Pointer(@PlugCheckBoxShowValueItemEffect));
  Result.AddObject('PlugCheckBoxSpecialQuickFlashing', Pointer(@PlugCheckBoxSpecialQuickFlashing));
  Result.AddObject('PlugComboBoxItemStdMode', Pointer(@PlugComboBoxItemStdMode));
  Result.AddObject('PlugEditSearchItem', Pointer(@PlugEditSearchItem));
  Result.AddObject('PlugEditSpecialColor', Pointer(@PlugEditSpecialColor));
  Result.AddObject('PlugEditSpecialName', Pointer(@PlugEditSpecialName));
  Result.AddObject('PlugLabelDefaultItem', Pointer(@PlugLabelDefaultItem));
  Result.AddObject('PlugLabelSpecialColor', Pointer(@PlugLabelSpecialColor));
  Result.AddObject('PlugMemoConfig2', Pointer(@PlugMemoConfig2));
  Result.AddObject('PlugMemoConfig2Label24', Pointer(@PlugMemoConfig2Label24));
  Result.AddObject('PlugMemoConfig2Label25', Pointer(@PlugMemoConfig2Label25));
  Result.AddObject('PlugMemoConfig2Label26', Pointer(@PlugMemoConfig2Label26));
  Result.AddObject('PlugMemoConfig2Label27', Pointer(@PlugMemoConfig2Label27));
  Result.AddObject('PlugMemoConfig2Label28', Pointer(@PlugMemoConfig2Label28));
  Result.AddObject('PlugMemoConfig2Label29', Pointer(@PlugMemoConfig2Label29));
  Result.AddObject('PlugMemoConfig2Line1', Pointer(@PlugMemoConfig2Line1));
  Result.AddObject('PlugMemoConfig2Line2', Pointer(@PlugMemoConfig2Line2));
  Result.AddObject('PlugTabSheetConfig3', Pointer(@PlugTabSheetConfig3));
  Result.AddObject('PlugMemoConfig3', Pointer(@PlugMemoConfig3));
  Result.AddObject('PlugBtnUnbindItemAdd', Pointer(@PlugBtnUnbindItemAdd));
  Result.AddObject('PlugBtnUnbindItemDel', Pointer(@PlugBtnUnbindItemDel));
  Result.AddObject('PlugBtnUnbindItemEdit', Pointer(@PlugBtnUnbindItemEdit));
  Result.AddObject('PlugBtnUnbindItemSave', Pointer(@PlugBtnUnbindItemSave));
  Result.AddObject('PlugCheckBoxHeroContinuousNoHitMon', Pointer(@PlugCheckBoxHeroContinuousNoHitMon));
  Result.AddObject('PlugCheckBoxHeroRenewAlcoholIsAuto', Pointer(@PlugCheckBoxHeroRenewAlcoholIsAuto));
  Result.AddObject('PlugCheckBoxHeroRenewMedicineAlcoholIsAuto', Pointer(@PlugCheckBoxHeroRenewMedicineAlcoholIsAuto));
  Result.AddObject('PlugCheckBoxHeroShowNumberState', Pointer(@PlugCheckBoxHeroShowNumberState));
  Result.AddObject('PlugCheckBoxRenewAlcoholIsAuto', Pointer(@PlugCheckBoxRenewAlcoholIsAuto));
  Result.AddObject('PlugCheckBoxRenewDeliriaIsAuto', Pointer(@PlugCheckBoxRenewDeliriaIsAuto));
  Result.AddObject('PlugCheckBoxRenewMedicineAlcoholIsAuto', Pointer(@PlugCheckBoxRenewMedicineAlcoholIsAuto));
  Result.AddObject('PlugComboGroupUnBindItem', Pointer(@PlugComboGroupUnBindItem));
  Result.AddObject('PlugEditHeroDodgeHPPercent', Pointer(@PlugEditHeroDodgeHPPercent));
  Result.AddObject('PlugEditHeroRenewAlcoholPercent', Pointer(@PlugEditHeroRenewAlcoholPercent));
  Result.AddObject('PlugEditHeroRenewMedicineAlcoholPercent', Pointer(@PlugEditHeroRenewMedicineAlcoholPercent));
  Result.AddObject('PlugEditItemName', Pointer(@PlugEditItemName));
  Result.AddObject('PlugEditRenewAlcoholPercent', Pointer(@PlugEditRenewAlcoholPercent));
  Result.AddObject('PlugEditRenewMedicineAlcoholPercent', Pointer(@PlugEditRenewMedicineAlcoholPercent));
  Result.AddObject('PlugEditUnbindName', Pointer(@PlugEditUnbindName));
  Result.AddObject('PlugLblGroupUnBindItem', Pointer(@PlugLblGroupUnBindItem));
  Result.AddObject('PlugLblItemName', Pointer(@PlugLblItemName));
  Result.AddObject('PlugLblUnbindName', Pointer(@PlugLblUnbindName));
  Result.AddObject('PlugMemoConfig3Label1', Pointer(@PlugMemoConfig3Label1));
  Result.AddObject('PlugMemoConfig3Label3', Pointer(@PlugMemoConfig3Label3));
  Result.AddObject('PlugMemoConfig3Label4', Pointer(@PlugMemoConfig3Label4));
  Result.AddObject('PlugMemoConfig3Label5', Pointer(@PlugMemoConfig3Label5));
  Result.AddObject('PlugMemoConfig3Label6', Pointer(@PlugMemoConfig3Label6));
  Result.AddObject('PlugMemoConfig3Label7', Pointer(@PlugMemoConfig3Label7));
  Result.AddObject('PlugMemoConfig3Label8', Pointer(@PlugMemoConfig3Label8));
  Result.AddObject('PlugScrollBoxUnbindItems', Pointer(@PlugScrollBoxUnbindItems));
  Result.AddObject('PlugTabSheetConfig4', Pointer(@PlugTabSheetConfig4));
  Result.AddObject('PlugMemoConfig4', Pointer(@PlugMemoConfig4));
  Result.AddObject('PlugCheckBoxAutoPercent', Pointer(@PlugCheckBoxAutoPercent));
  Result.AddObject('PlugCheckBoxCheckDuraIsAuto', Pointer(@PlugCheckBoxCheckDuraIsAuto));
  Result.AddObject('PlugCheckBoxCheckHPIsAuto', Pointer(@PlugCheckBoxCheckHPIsAuto));
  Result.AddObject('PlugCheckBoxCheckMPIsAuto', Pointer(@PlugCheckBoxCheckMPIsAuto));
  Result.AddObject('PlugCheckBoxRenewAutoPercent', Pointer(@PlugCheckBoxRenewAutoPercent));
  Result.AddObject('PlugCheckBoxRenewHPIsAuto', Pointer(@PlugCheckBoxRenewHPIsAuto));
  Result.AddObject('PlugCheckBoxRenewMPIsAuto', Pointer(@PlugCheckBoxRenewMPIsAuto));
  Result.AddObject('PlugCheckBoxRenewSpecialHPIsAuto', Pointer(@PlugCheckBoxRenewSpecialHPIsAuto));
  Result.AddObject('PlugCheckBoxRenewSpecialMPIsAuto', Pointer(@PlugCheckBoxRenewSpecialMPIsAuto));
  Result.AddObject('PlugCheckBoxSuperMedicaPercent', Pointer(@PlugCheckBoxSuperMedicaPercent));
  Result.AddObject('PlugCheckBoxUseSuperMedica', Pointer(@PlugCheckBoxUseSuperMedica));
  Result.AddObject('PlugCheckBoxUseSuperMedicaItemName0', Pointer(@PlugCheckBoxUseSuperMedicaItemName0));
  Result.AddObject('PlugCheckBoxUseSuperMedicaItemName1', Pointer(@PlugCheckBoxUseSuperMedicaItemName1));
  Result.AddObject('PlugCheckBoxUseSuperMedicaItemName2', Pointer(@PlugCheckBoxUseSuperMedicaItemName2));
  Result.AddObject('PlugCheckBoxUseSuperMedicaItemName3', Pointer(@PlugCheckBoxUseSuperMedicaItemName3));
  Result.AddObject('PlugCheckBoxUseSuperMedicaItemName4', Pointer(@PlugCheckBoxUseSuperMedicaItemName4));
  Result.AddObject('PlugCheckBoxUseSuperMedicaItemName5', Pointer(@PlugCheckBoxUseSuperMedicaItemName5));
  Result.AddObject('PlugCheckBoxUseSuperMedicaItemName6', Pointer(@PlugCheckBoxUseSuperMedicaItemName6));
  Result.AddObject('PlugCheckBoxUseSuperMedicaItemName7', Pointer(@PlugCheckBoxUseSuperMedicaItemName7));
  Result.AddObject('PlugCheckBoxUseSuperMedicaItemName8', Pointer(@PlugCheckBoxUseSuperMedicaItemName8));
  Result.AddObject('PlugComboBoxCheckHPValue', Pointer(@PlugComboBoxCheckHPValue));
  Result.AddObject('PlugComboBoxCheckMPValue', Pointer(@PlugComboBoxCheckMPValue));
  Result.AddObject('PlugEditCheckDura', Pointer(@PlugEditCheckDura));
  Result.AddObject('PlugEditCheckDuraTime', Pointer(@PlugEditCheckDuraTime));
  Result.AddObject('PlugEditCheckDuraValue', Pointer(@PlugEditCheckDuraValue));
  Result.AddObject('PlugEditCheckHPPercent', Pointer(@PlugEditCheckHPPercent));
  Result.AddObject('PlugEditCheckMPPercent', Pointer(@PlugEditCheckMPPercent));
  Result.AddObject('PlugEditRenewHPPercent', Pointer(@PlugEditRenewHPPercent));
  Result.AddObject('PlugEditRenewHPTime', Pointer(@PlugEditRenewHPTime));
  Result.AddObject('PlugEditRenewMPPercent', Pointer(@PlugEditRenewMPPercent));
  Result.AddObject('PlugEditRenewMPTime', Pointer(@PlugEditRenewMPTime));
  Result.AddObject('PlugEditRenewSpecialHPPercent', Pointer(@PlugEditRenewSpecialHPPercent));
  Result.AddObject('PlugEditRenewSpecialHPTime', Pointer(@PlugEditRenewSpecialHPTime));
  Result.AddObject('PlugEditRenewSpecialMPPercent', Pointer(@PlugEditRenewSpecialMPPercent));
  Result.AddObject('PlugEditRenewSpecialMPTime', Pointer(@PlugEditRenewSpecialMPTime));
  Result.AddObject('PlugEditSuperMedicaHP0', Pointer(@PlugEditSuperMedicaHP0));
  Result.AddObject('PlugEditSuperMedicaHP1', Pointer(@PlugEditSuperMedicaHP1));
  Result.AddObject('PlugEditSuperMedicaHP2', Pointer(@PlugEditSuperMedicaHP2));
  Result.AddObject('PlugEditSuperMedicaHP3', Pointer(@PlugEditSuperMedicaHP3));
  Result.AddObject('PlugEditSuperMedicaHP4', Pointer(@PlugEditSuperMedicaHP4));
  Result.AddObject('PlugEditSuperMedicaHP5', Pointer(@PlugEditSuperMedicaHP5));
  Result.AddObject('PlugEditSuperMedicaHP6', Pointer(@PlugEditSuperMedicaHP6));
  Result.AddObject('PlugEditSuperMedicaHP7', Pointer(@PlugEditSuperMedicaHP7));
  Result.AddObject('PlugEditSuperMedicaHP8', Pointer(@PlugEditSuperMedicaHP8));
  Result.AddObject('PlugEditSuperMedicaHPTime0', Pointer(@PlugEditSuperMedicaHPTime0));
  Result.AddObject('PlugEditSuperMedicaHPTime1', Pointer(@PlugEditSuperMedicaHPTime1));
  Result.AddObject('PlugEditSuperMedicaHPTime2', Pointer(@PlugEditSuperMedicaHPTime2));
  Result.AddObject('PlugEditSuperMedicaHPTime3', Pointer(@PlugEditSuperMedicaHPTime3));
  Result.AddObject('PlugEditSuperMedicaHPTime4', Pointer(@PlugEditSuperMedicaHPTime4));
  Result.AddObject('PlugEditSuperMedicaHPTime5', Pointer(@PlugEditSuperMedicaHPTime5));
  Result.AddObject('PlugEditSuperMedicaHPTime6', Pointer(@PlugEditSuperMedicaHPTime6));
  Result.AddObject('PlugEditSuperMedicaHPTime7', Pointer(@PlugEditSuperMedicaHPTime7));
  Result.AddObject('PlugEditSuperMedicaHPTime8', Pointer(@PlugEditSuperMedicaHPTime8));
  Result.AddObject('PlugEditSuperMedicaMP0', Pointer(@PlugEditSuperMedicaMP0));
  Result.AddObject('PlugEditSuperMedicaMP1', Pointer(@PlugEditSuperMedicaMP1));
  Result.AddObject('PlugEditSuperMedicaMP2', Pointer(@PlugEditSuperMedicaMP2));
  Result.AddObject('PlugEditSuperMedicaMP3', Pointer(@PlugEditSuperMedicaMP3));
  Result.AddObject('PlugEditSuperMedicaMP4', Pointer(@PlugEditSuperMedicaMP4));
  Result.AddObject('PlugEditSuperMedicaMP5', Pointer(@PlugEditSuperMedicaMP5));
  Result.AddObject('PlugEditSuperMedicaMP6', Pointer(@PlugEditSuperMedicaMP6));
  Result.AddObject('PlugEditSuperMedicaMP7', Pointer(@PlugEditSuperMedicaMP7));
  Result.AddObject('PlugEditSuperMedicaMP8', Pointer(@PlugEditSuperMedicaMP8));
  Result.AddObject('PlugEditSuperMedicaMPTime0', Pointer(@PlugEditSuperMedicaMPTime0));
  Result.AddObject('PlugEditSuperMedicaMPTime1', Pointer(@PlugEditSuperMedicaMPTime1));
  Result.AddObject('PlugEditSuperMedicaMPTime2', Pointer(@PlugEditSuperMedicaMPTime2));
  Result.AddObject('PlugEditSuperMedicaMPTime3', Pointer(@PlugEditSuperMedicaMPTime3));
  Result.AddObject('PlugEditSuperMedicaMPTime4', Pointer(@PlugEditSuperMedicaMPTime4));
  Result.AddObject('PlugEditSuperMedicaMPTime5', Pointer(@PlugEditSuperMedicaMPTime5));
  Result.AddObject('PlugEditSuperMedicaMPTime6', Pointer(@PlugEditSuperMedicaMPTime6));
  Result.AddObject('PlugEditSuperMedicaMPTime7', Pointer(@PlugEditSuperMedicaMPTime7));
  Result.AddObject('PlugEditSuperMedicaMPTime8', Pointer(@PlugEditSuperMedicaMPTime8));
  Result.AddObject('PlugMemoConfig4Label1', Pointer(@PlugMemoConfig4Label1));
  Result.AddObject('PlugMemoConfig4Label2', Pointer(@PlugMemoConfig4Label2));
  Result.AddObject('PlugMemoConfig4Label3', Pointer(@PlugMemoConfig4Label3));
  Result.AddObject('PlugMemoConfig4Label4', Pointer(@PlugMemoConfig4Label4));
  Result.AddObject('PlugMemoConfig4Label5', Pointer(@PlugMemoConfig4Label5));
  Result.AddObject('PlugMemoConfig4Label6', Pointer(@PlugMemoConfig4Label6));
  Result.AddObject('PlugMemoConfig4Label7', Pointer(@PlugMemoConfig4Label7));
  Result.AddObject('PlugMemoConfig4Label8', Pointer(@PlugMemoConfig4Label8));
  Result.AddObject('PlugMemoConfig4LabelHint', Pointer(@PlugMemoConfig4LabelHint));
  Result.AddObject('PlugMemoConfig4LabelHint2', Pointer(@PlugMemoConfig4LabelHint2));
  Result.AddObject('PlugMemoConfig4Line2', Pointer(@PlugMemoConfig4Line2));
  Result.AddObject('PlugMemoConfig4Line3', Pointer(@PlugMemoConfig4Line3));
  Result.AddObject('PlugMemoConfig4Line4', Pointer(@PlugMemoConfig4Line4));
  Result.AddObject('PlugMemoConfig4Line5', Pointer(@PlugMemoConfig4Line5));
  Result.AddObject('PlugMemoConfig4Button1', Pointer(@PlugMemoConfig4Button1));
  Result.AddObject('PlugMemoConfig4Button2', Pointer(@PlugMemoConfig4Button2));
  Result.AddObject('PlugMemoConfig4Button3', Pointer(@PlugMemoConfig4Button3));
  Result.AddObject('PlugMemoConfig4Button4', Pointer(@PlugMemoConfig4Button4));
  Result.AddObject('PlugMemoConfig4Button5', Pointer(@PlugMemoConfig4Button5));
  Result.AddObject('PlugMemoConfig4Line1', Pointer(@PlugMemoConfig4Line1));
  Result.AddObject('PlugTabSheetConfig5', Pointer(@PlugTabSheetConfig5));
  Result.AddObject('PlugMemoConfig5', Pointer(@PlugMemoConfig5));
  Result.AddObject('PlugCheckBoxAssistantHeroAutoShield', Pointer(@PlugCheckBoxAssistantHeroAutoShield));
  Result.AddObject('PlugCheckBoxAutoCHangePoison', Pointer(@PlugCheckBoxAutoCHangePoison));
  Result.AddObject('PlugCheckBoxAutoContinueAttack', Pointer(@PlugCheckBoxAutoContinueAttack));
  Result.AddObject('PlugCheckBoxAutoCustomHit1', Pointer(@PlugCheckBoxAutoCustomHit1));
  Result.AddObject('PlugCheckBoxAutoCustomHit2', Pointer(@PlugCheckBoxAutoCustomHit2));
  Result.AddObject('PlugCheckBoxAutoCustomHit3', Pointer(@PlugCheckBoxAutoCustomHit3));
  Result.AddObject('PlugCheckBoxAutoCustomHit4', Pointer(@PlugCheckBoxAutoCustomHit4));
  Result.AddObject('PlugCheckBoxAutoCustomHit5', Pointer(@PlugCheckBoxAutoCustomHit5));
  Result.AddObject('PlugCheckBoxAutoCustomHit6', Pointer(@PlugCheckBoxAutoCustomHit6));
  Result.AddObject('PlugCheckBoxAutoCustomHit7', Pointer(@PlugCheckBoxAutoCustomHit7));
  Result.AddObject('PlugCheckBoxAutoCustomHit8', Pointer(@PlugCheckBoxAutoCustomHit8));
  Result.AddObject('PlugCheckBoxAutoGroupAttack', Pointer(@PlugCheckBoxAutoGroupAttack));
  Result.AddObject('PlugCheckBoxAutoGroupNoAttackMon', Pointer(@PlugCheckBoxAutoGroupNoAttackMon));
  Result.AddObject('PlugCheckBoxAutoHideMode', Pointer(@PlugCheckBoxAutoHideMode));
  Result.AddObject('PlugCheckBoxAutoMagic', Pointer(@PlugCheckBoxAutoMagic));
  Result.AddObject('PlugCheckBoxAutoOpenSpell', Pointer(@PlugCheckBoxAutoOpenSpell));
  Result.AddObject('PlugCheckBoxHeroAutoShield', Pointer(@PlugCheckBoxHeroAutoShield));
  Result.AddObject('PlugCheckBoxHumAutoShield', Pointer(@PlugCheckBoxHumAutoShield));
  Result.AddObject('PlugCheckBoxHumManuallyCustomHit1', Pointer(@PlugCheckBoxHumManuallyCustomHit1));
  Result.AddObject('PlugCheckBoxHumManuallyCustomHit2', Pointer(@PlugCheckBoxHumManuallyCustomHit2));
  Result.AddObject('PlugCheckBoxHumManuallyCustomHit3', Pointer(@PlugCheckBoxHumManuallyCustomHit3));
  Result.AddObject('PlugCheckBoxHumManuallyCustomHit4', Pointer(@PlugCheckBoxHumManuallyCustomHit4));
  Result.AddObject('PlugCheckBoxHumManuallyCustomHit5', Pointer(@PlugCheckBoxHumManuallyCustomHit5));
  Result.AddObject('PlugCheckBoxHumManuallyFire', Pointer(@PlugCheckBoxHumManuallyFire));
  Result.AddObject('PlugCheckBoxHumManuallyFireBoom', Pointer(@PlugCheckBoxHumManuallyFireBoom));
  Result.AddObject('PlugCheckBoxHumManuallyMeteorShower', Pointer(@PlugCheckBoxHumManuallyMeteorShower));
  Result.AddObject('PlugCheckBoxHumManuallyMove10Attack', Pointer(@PlugCheckBoxHumManuallyMove10Attack));
  Result.AddObject('PlugCheckBoxHumManuallySnowWind', Pointer(@PlugCheckBoxHumManuallySnowWind));
  Result.AddObject('PlugCheckBoxHumShootLightenLockTarget', Pointer(@PlugCheckBoxHumShootLightenLockTarget));
  Result.AddObject('PlugCheckBoxHumStruckShield', Pointer(@PlugCheckBoxHumStruckShield));
  Result.AddObject('PlugCheckBoxSmart113Hit', Pointer(@PlugCheckBoxSmart113Hit));
  Result.AddObject('PlugCheckBoxSmartCRSHit', Pointer(@PlugCheckBoxSmartCRSHit));
  Result.AddObject('PlugCheckBoxSmartFireHit', Pointer(@PlugCheckBoxSmartFireHit));
  Result.AddObject('PlugCheckBoxSmartKTZHit', Pointer(@PlugCheckBoxSmartKTZHit));
  Result.AddObject('PlugCheckBoxSmartLongHit', Pointer(@PlugCheckBoxSmartLongHit));
  Result.AddObject('PlugCheckBoxSmartPosLongHit', Pointer(@PlugCheckBoxSmartPosLongHit));
  Result.AddObject('PlugCheckBoxSmartSwordHit', Pointer(@PlugCheckBoxSmartSwordHit));
  Result.AddObject('PlugCheckBoxSmartTWNHit', Pointer(@PlugCheckBoxSmartTWNHit));
  Result.AddObject('PlugCheckBoxSmartWalkLongHit', Pointer(@PlugCheckBoxSmartWalkLongHit));
  Result.AddObject('PlugCheckBoxSmartWideHit', Pointer(@PlugCheckBoxSmartWideHit));
  Result.AddObject('PlugComboBoxAutoMagic', Pointer(@PlugComboBoxAutoMagic));
  Result.AddObject('PlugEditAutoMagicTime', Pointer(@PlugEditAutoMagicTime));
  Result.AddObject('PlugMemoConfig5Label19', Pointer(@PlugMemoConfig5Label19));
  Result.AddObject('PlugMemoConfig5Label20', Pointer(@PlugMemoConfig5Label20));
  Result.AddObject('PlugMemoConfig5Label21', Pointer(@PlugMemoConfig5Label21));
  Result.AddObject('PlugMemoConfig5Label22', Pointer(@PlugMemoConfig5Label22));
  Result.AddObject('PlugMemoConfig5Label23', Pointer(@PlugMemoConfig5Label23));
  Result.AddObject('PlugMemoConfig5LabelLongHit', Pointer(@PlugMemoConfig5LabelLongHit));
  Result.AddObject('PlugTabSheetConfig6', Pointer(@PlugTabSheetConfig6));
  Result.AddObject('PlugMemoConfig6', Pointer(@PlugMemoConfig6));
  Result.AddObject('PlugCheckBoxUseKeyBoard', Pointer(@PlugCheckBoxUseKeyBoard));
  Result.AddObject('PlugMemoConfig6Label1', Pointer(@PlugMemoConfig6Label1));
  Result.AddObject('PlugMemoConfig6Label2', Pointer(@PlugMemoConfig6Label2));
  Result.AddObject('PlugMemoConfig6Label3', Pointer(@PlugMemoConfig6Label3));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard1', Pointer(@PlugMemoConfig6LabelKeyBoard1));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard10', Pointer(@PlugMemoConfig6LabelKeyBoard10));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard11', Pointer(@PlugMemoConfig6LabelKeyBoard11));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard12', Pointer(@PlugMemoConfig6LabelKeyBoard12));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard13', Pointer(@PlugMemoConfig6LabelKeyBoard13));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard14', Pointer(@PlugMemoConfig6LabelKeyBoard14));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard15', Pointer(@PlugMemoConfig6LabelKeyBoard15));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard16', Pointer(@PlugMemoConfig6LabelKeyBoard16));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard2', Pointer(@PlugMemoConfig6LabelKeyBoard2));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard3', Pointer(@PlugMemoConfig6LabelKeyBoard3));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard4', Pointer(@PlugMemoConfig6LabelKeyBoard4));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard5', Pointer(@PlugMemoConfig6LabelKeyBoard5));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard6', Pointer(@PlugMemoConfig6LabelKeyBoard6));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard7', Pointer(@PlugMemoConfig6LabelKeyBoard7));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard8', Pointer(@PlugMemoConfig6LabelKeyBoard8));
  Result.AddObject('PlugMemoConfig6LabelKeyBoard9', Pointer(@PlugMemoConfig6LabelKeyBoard9));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc1', Pointer(@PlugMemoConfig6LabelKeyBoardDesc1));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc10', Pointer(@PlugMemoConfig6LabelKeyBoardDesc10));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc11', Pointer(@PlugMemoConfig6LabelKeyBoardDesc11));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc12', Pointer(@PlugMemoConfig6LabelKeyBoardDesc12));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc13', Pointer(@PlugMemoConfig6LabelKeyBoardDesc13));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc14', Pointer(@PlugMemoConfig6LabelKeyBoardDesc14));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc15', Pointer(@PlugMemoConfig6LabelKeyBoardDesc15));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc16', Pointer(@PlugMemoConfig6LabelKeyBoardDesc16));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc2', Pointer(@PlugMemoConfig6LabelKeyBoardDesc2));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc3', Pointer(@PlugMemoConfig6LabelKeyBoardDesc3));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc4', Pointer(@PlugMemoConfig6LabelKeyBoardDesc4));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc5', Pointer(@PlugMemoConfig6LabelKeyBoardDesc5));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc6', Pointer(@PlugMemoConfig6LabelKeyBoardDesc6));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc7', Pointer(@PlugMemoConfig6LabelKeyBoardDesc7));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc8', Pointer(@PlugMemoConfig6LabelKeyBoardDesc8));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardDesc9', Pointer(@PlugMemoConfig6LabelKeyBoardDesc9));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal1', Pointer(@PlugMemoConfig6LabelKeyBoardNormal1));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal10', Pointer(@PlugMemoConfig6LabelKeyBoardNormal10));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal11', Pointer(@PlugMemoConfig6LabelKeyBoardNormal11));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal12', Pointer(@PlugMemoConfig6LabelKeyBoardNormal12));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal13', Pointer(@PlugMemoConfig6LabelKeyBoardNormal13));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal14', Pointer(@PlugMemoConfig6LabelKeyBoardNormal14));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal15', Pointer(@PlugMemoConfig6LabelKeyBoardNormal15));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal16', Pointer(@PlugMemoConfig6LabelKeyBoardNormal16));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal2', Pointer(@PlugMemoConfig6LabelKeyBoardNormal2));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal3', Pointer(@PlugMemoConfig6LabelKeyBoardNormal3));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal4', Pointer(@PlugMemoConfig6LabelKeyBoardNormal4));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal5', Pointer(@PlugMemoConfig6LabelKeyBoardNormal5));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal6', Pointer(@PlugMemoConfig6LabelKeyBoardNormal6));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal7', Pointer(@PlugMemoConfig6LabelKeyBoardNormal7));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal8', Pointer(@PlugMemoConfig6LabelKeyBoardNormal8));
  Result.AddObject('PlugMemoConfig6LabelKeyBoardNormal9', Pointer(@PlugMemoConfig6LabelKeyBoardNormal9));
  Result.AddObject('PlugMemoConfig6Line1', Pointer(@PlugMemoConfig6Line1));
  Result.AddObject('PlugTabSheetConfig7', Pointer(@PlugTabSheetConfig7));
  Result.AddObject('PlugMemoConfigBoss', Pointer(@PlugMemoConfigBoss));
  Result.AddObject('PlugBtnBossAdd', Pointer(@PlugBtnBossAdd));
  Result.AddObject('PlugBtnBossDel', Pointer(@PlugBtnBossDel));
  Result.AddObject('PlugBtnBossModify', Pointer(@PlugBtnBossModify));
  Result.AddObject('PlugCheckBoxAutoDownHorse', Pointer(@PlugCheckBoxAutoDownHorse));
  Result.AddObject('PlugCheckBoxAutoLock', Pointer(@PlugCheckBoxAutoLock));
  Result.AddObject('PlugCheckBoxBlacklistHit', Pointer(@PlugCheckBoxBlacklistHit));
  Result.AddObject('PlugCheckBoxColorShow', Pointer(@PlugCheckBoxColorShow));
  Result.AddObject('PlugCheckBoxDisableSelfStruck', Pointer(@PlugCheckBoxDisableSelfStruck));
  Result.AddObject('PlugCheckBoxFriendHit', Pointer(@PlugCheckBoxFriendHit));
  Result.AddObject('PlugCheckBoxHideBigHPProgress', Pointer(@PlugCheckBoxHideBigHPProgress));
  Result.AddObject('PlugCheckBoxMagicLock', Pointer(@PlugCheckBoxMagicLock));
  Result.AddObject('PlugCheckBoxNearHint', Pointer(@PlugCheckBoxNearHint));
  Result.AddObject('PlugCheckBoxNoCaton', Pointer(@PlugCheckBoxNoCaton));
  Result.AddObject('PlugCheckBoxShowTargetAperture', Pointer(@PlugCheckBoxShowTargetAperture));
  Result.AddObject('PlugComboBoxColorShow', Pointer(@PlugComboBoxColorShow));
  Result.AddObject('PlugEditBoss', Pointer(@PlugEditBoss));
  Result.AddObject('PlugLblBoss', Pointer(@PlugLblBoss));
  Result.AddObject('PlugScrollBoxBoss', Pointer(@PlugScrollBoxBoss));
  Result.AddObject('PlugCheckBoxNearEffect', Pointer(@PlugCheckBoxNearEffect)); //HZQ 20230828， 增加目标接近开关
  Result.AddObject('PlugTabSheetConfig8', Pointer(@PlugTabSheetConfig8));
  Result.AddObject('PlugMemoConfig8', Pointer(@PlugMemoConfig8));
  Result.AddObject('PlugBtnGJPoint', Pointer(@PlugBtnGJPoint));
  Result.AddObject('PlugButtonGJRun', Pointer(@PlugButtonGJRun));
  Result.AddObject('PlugCheckBoxAutoPickup', Pointer(@PlugCheckBoxAutoPickup));
  Result.AddObject('PlugCheckBoxBagFull', Pointer(@PlugCheckBoxBagFull));
  Result.AddObject('PlugCheckBoxDFAvoid', Pointer(@PlugCheckBoxDFAvoid));
  Result.AddObject('PlugCheckBoxLimitScreen', Pointer(@PlugCheckBoxLimitScreen));
  Result.AddObject('PlugCheckBoxNoBluePoison', Pointer(@PlugCheckBoxNoBluePoison));
  Result.AddObject('PlugCheckBoxNoDuFu', Pointer(@PlugCheckBoxNoDuFu));
  Result.AddObject('PlugCheckBoxNoRedPoison', Pointer(@PlugCheckBoxNoRedPoison));
  Result.AddObject('PlugCheckBoxNotRushMon', Pointer(@PlugCheckBoxNotRushMon));
  Result.AddObject('PlugCheckBoxPlayAttack', Pointer(@PlugCheckBoxPlayAttack));
  Result.AddObject('PlugComboBoxBagFullValue', Pointer(@PlugComboBoxBagFullValue));
  Result.AddObject('PlugComboBoxNoBluePoisonValue', Pointer(@PlugComboBoxNoBluePoisonValue));
  Result.AddObject('PlugComboBoxNoDuFuValue', Pointer(@PlugComboBoxNoDuFuValue));
  Result.AddObject('PlugComboBoxNoRedPoisonValue', Pointer(@PlugComboBoxNoRedPoisonValue));
  Result.AddObject('PlugComboBoxPlayAttackValue', Pointer(@PlugComboBoxPlayAttackValue));
  Result.AddObject('PlugEditNotRushMonRange', Pointer(@PlugEditNotRushMonRange));
  Result.AddObject('PlugLabelNotRushMon', Pointer(@PlugLabelNotRushMon));
  Result.AddObject('PlugMemoConfig8Button1', Pointer(@PlugMemoConfig8Button1));
  Result.AddObject('PlugMemoConfig8Button2', Pointer(@PlugMemoConfig8Button2));
  Result.AddObject('PlugMemoConfig8Button3', Pointer(@PlugMemoConfig8Button3));
  Result.AddObject('PlugMemoConfig8Line1', Pointer(@PlugMemoConfig8Line1));
  Result.AddObject('PlugMemoConfig8Line2', Pointer(@PlugMemoConfig8Line2));
  Result.AddObject('PlugMemoConfig8Page', Pointer(@PlugMemoConfig8Page));
  Result.AddObject('PlugTabSheetConfig81', Pointer(@PlugTabSheetConfig81));
  Result.AddObject('PlugButtonMonNameAdd', Pointer(@PlugButtonMonNameAdd));
  Result.AddObject('PlugButtonMonNameDel', Pointer(@PlugButtonMonNameDel));
  Result.AddObject('PlugButtonMonNameEdit', Pointer(@PlugButtonMonNameEdit));
  Result.AddObject('PlugEditMonName', Pointer(@PlugEditMonName));
  Result.AddObject('PlugLabelMonName', Pointer(@PlugLabelMonName));
  Result.AddObject('PlugLabelShortKey', Pointer(@PlugLabelShortKey));
  Result.AddObject('PlugScrollBoxMons', Pointer(@PlugScrollBoxMons));
  Result.AddObject('PlugTabSheetConfig82', Pointer(@PlugTabSheetConfig82));
  Result.AddObject('PlugMemoConfig82', Pointer(@PlugMemoConfig82));
  Result.AddObject('PlugLabelConfig82C1', Pointer(@PlugLabelConfig82C1));
  Result.AddObject('PlugLabelConfig82C2', Pointer(@PlugLabelConfig82C2));
  Result.AddObject('PlugLineConfig82C', Pointer(@PlugLineConfig82C));
  Result.AddObject('PlugTabSheetConfig83', Pointer(@PlugTabSheetConfig83));
  Result.AddObject('Line1', Pointer(@Line1));
  Result.AddObject('PlugCheckBoxGroupAttack', Pointer(@PlugCheckBoxGroupAttack));
  Result.AddObject('PlugEditNotGroupAttackCount', Pointer(@PlugEditNotGroupAttackCount));
  Result.AddObject('PlugLabelGroupAttack', Pointer(@PlugLabelGroupAttack));
  Result.AddObject('PlugMemoConfig83', Pointer(@PlugMemoConfig83));
  Result.AddObject('PlugLabelConfig83C1', Pointer(@PlugLabelConfig83C1));
  Result.AddObject('PlugLabelConfig83C2', Pointer(@PlugLabelConfig83C2));
  Result.AddObject('PlugLineConfig83C', Pointer(@PlugLineConfig83C));
  Result.AddObject('PlugTabSheetConfig80', Pointer(@PlugTabSheetConfig80));
  Result.AddObject('PlugMemoConfig10ButtonCancel', Pointer(@PlugMemoConfig10ButtonCancel));
  Result.AddObject('PlugMemoConfig10ButtonEdit', Pointer(@PlugMemoConfig10ButtonEdit));
  Result.AddObject('PlugMemoConfig10ButtonSave', Pointer(@PlugMemoConfig10ButtonSave));
  Result.AddObject('PlugMemoConfigNotes', Pointer(@PlugMemoConfigNotes));
  Result.AddObject('PlugTabSheetConfig9', Pointer(@PlugTabSheetConfig9));
  Result.AddObject('PlugMemoConfigHelp', Pointer(@PlugMemoConfigHelp));
  Result.AddObject('PopupMenuItems', Pointer(@PopupMenuItems));
  Result.AddObject('DOptBtnSKiilIcon', Pointer(@DOptBtnSKiilIcon));
  Result.AddObject('DOptBtnSkillLine', Pointer(@DOptBtnSkillLine));
  Result.AddObject('DOptFrm', Pointer(@DOptFrm));
  Result.AddObject('DOptBtnClose', Pointer(@DOptBtnClose));
  Result.AddObject('DOptPgc', Pointer(@DOptPgc));
  Result.AddObject('DOptTs1', Pointer(@DOptTs1));
  Result.AddObject('ScrollBox1', Pointer(@ScrollBox1));
  Result.AddObject('DOptChkAutoRepair', Pointer(@DOptChkAutoRepair));
  Result.AddObject('DOptChkAutoTakeOn', Pointer(@DOptChkAutoTakeOn));
  Result.AddObject('DOptChkDuraHint', Pointer(@DOptChkDuraHint));
  Result.AddObject('DOptChkExpFilter', Pointer(@DOptChkExpFilter));
  Result.AddObject('DOptChkHPNumber', Pointer(@DOptChkHPNumber));
  Result.AddObject('DOptChkHealthHint', Pointer(@DOptChkHealthHint));
  Result.AddObject('DOptChkItemCompare', Pointer(@DOptChkItemCompare));
  Result.AddObject('DOptChkJobLevel', Pointer(@DOptChkJobLevel));
  Result.AddObject('DOptChkJoyStick', Pointer(@DOptChkJoyStick));
  Result.AddObject('DOptChkNumberDecHP', Pointer(@DOptChkNumberDecHP));
  Result.AddObject('DOptChkOnlyShowHumName', Pointer(@DOptChkOnlyShowHumName));
  Result.AddObject('DOptChkShowActorName', Pointer(@DOptChkShowActorName));
  Result.AddObject('DOptChkShowHealthBar', Pointer(@DOptChkShowHealthBar));
  Result.AddObject('DOptChkShowMonName', Pointer(@DOptChkShowMonName));
  Result.AddObject('DOptChkSimpleDress', Pointer(@DOptChkSimpleDress));
  Result.AddObject('DOptChkSimpleMon', Pointer(@DOptChkSimpleMon));
  Result.AddObject('DOptEdtExpFilter', Pointer(@DOptEdtExpFilter));
  Result.AddObject('DOptLblBGM', Pointer(@DOptLblBGM));
  Result.AddObject('DOptLblBasic', Pointer(@DOptLblBasic));
  Result.AddObject('DOptLblMapScale', Pointer(@DOptLblMapScale));
  Result.AddObject('DOptLblSound', Pointer(@DOptLblSound));
  Result.AddObject('DOptTrckbrBGM', Pointer(@DOptTrckbrBGM));
  Result.AddObject('DOptTrckbrMapScale', Pointer(@DOptTrckbrMapScale));
  Result.AddObject('DOptTrckbrSound', Pointer(@DOptTrckbrSound));
  Result.AddObject('DOptTs2', Pointer(@DOptTs2));
  Result.AddObject('ScrollBox2', Pointer(@ScrollBox2));
  Result.AddObject('DOptLblHPGoHome', Pointer(@DOptLblHPGoHome));
  Result.AddObject('DOptLblProtect', Pointer(@DOptLblProtect));
  Result.AddObject('DOptLine1', Pointer(@DOptLine1));
  Result.AddObject('Edit2', Pointer(@Edit2));
  Result.AddObject('Edit3', Pointer(@Edit3));
  Result.AddObject('Edit4', Pointer(@Edit4));
  Result.AddObject('Edit5', Pointer(@Edit5));
  Result.AddObject('Edit6', Pointer(@Edit6));
  Result.AddObject('Edit7', Pointer(@Edit7));
  Result.AddObject('Edit8', Pointer(@Edit8));
  Result.AddObject('Edit9', Pointer(@Edit9));
  Result.AddObject('ImageButton11', Pointer(@ImageButton11));
  Result.AddObject('ImageButton18', Pointer(@ImageButton18));
  Result.AddObject('ImageButton19', Pointer(@ImageButton19));
  Result.AddObject('ImageButton20', Pointer(@ImageButton20));
  Result.AddObject('ImageButton21', Pointer(@ImageButton21));
  Result.AddObject('ImageButton22', Pointer(@ImageButton22));
  Result.AddObject('ImageButton23', Pointer(@ImageButton23));
  Result.AddObject('ImageButton24', Pointer(@ImageButton24));
  Result.AddObject('Label10', Pointer(@Label10));
  Result.AddObject('Label11', Pointer(@Label11));
  Result.AddObject('Label12', Pointer(@Label12));
  Result.AddObject('Label13', Pointer(@Label13));
  Result.AddObject('Label14', Pointer(@Label14));
  Result.AddObject('Label15', Pointer(@Label15));
  Result.AddObject('Label16', Pointer(@Label16));
  Result.AddObject('Label17', Pointer(@Label17));
  Result.AddObject('Label18', Pointer(@Label18));
  Result.AddObject('Label19', Pointer(@Label19));
  Result.AddObject('Label20', Pointer(@Label20));
  Result.AddObject('Label6', Pointer(@Label6));
  Result.AddObject('Label7', Pointer(@Label7));
  Result.AddObject('Label8', Pointer(@Label8));
  Result.AddObject('Label9', Pointer(@Label9));
  Result.AddObject('TrackBar10', Pointer(@TrackBar10));
  Result.AddObject('TrackBar2', Pointer(@TrackBar2));
  Result.AddObject('TrackBar4', Pointer(@TrackBar4));
  Result.AddObject('TrackBar5', Pointer(@TrackBar5));
  Result.AddObject('TrackBar6', Pointer(@TrackBar6));
  Result.AddObject('TrackBar7', Pointer(@TrackBar7));
  Result.AddObject('TrackBar8', Pointer(@TrackBar8));
  Result.AddObject('TrackBar9', Pointer(@TrackBar9));
  Result.AddObject('DOptTs3', Pointer(@DOptTs3));
  Result.AddObject('ScrollBox3', Pointer(@ScrollBox3));
  Result.AddObject('DOptChkSlaveFlowMaster', Pointer(@DOptChkSlaveFlowMaster));
  Result.AddObject('Edit10', Pointer(@Edit10));
  Result.AddObject('ImageButton12', Pointer(@ImageButton12));
  Result.AddObject('ImageButton2', Pointer(@ImageButton2));
  Result.AddObject('ImageButton26', Pointer(@ImageButton26));
  Result.AddObject('ImageButton27', Pointer(@ImageButton27));
  Result.AddObject('ImageButton28', Pointer(@ImageButton28));
  Result.AddObject('ImageButton29', Pointer(@ImageButton29));
  Result.AddObject('ImageButton3', Pointer(@ImageButton3));
  Result.AddObject('ImageButton30', Pointer(@ImageButton30));
  Result.AddObject('ImageButton31', Pointer(@ImageButton31));
  Result.AddObject('ImageButton32', Pointer(@ImageButton32));
  Result.AddObject('ImageButton33', Pointer(@ImageButton33));
  Result.AddObject('ImageButton34', Pointer(@ImageButton34));
  Result.AddObject('ImageButton35', Pointer(@ImageButton35));
  Result.AddObject('ImageButton36', Pointer(@ImageButton36));
  Result.AddObject('ImageButton37', Pointer(@ImageButton37));
  Result.AddObject('ImageButton38', Pointer(@ImageButton38));
  Result.AddObject('ImageButton39', Pointer(@ImageButton39));
  Result.AddObject('ImageButton4', Pointer(@ImageButton4));
  Result.AddObject('ImageButton40', Pointer(@ImageButton40));
  Result.AddObject('ImageButton41', Pointer(@ImageButton41));
  Result.AddObject('ImageButton42', Pointer(@ImageButton42));
  Result.AddObject('ImageButton43', Pointer(@ImageButton43));
  Result.AddObject('ImageButton44', Pointer(@ImageButton44));
  Result.AddObject('ImageButton45', Pointer(@ImageButton45));
  Result.AddObject('ImageButton46', Pointer(@ImageButton46));
  Result.AddObject('ImageButton47', Pointer(@ImageButton47));
  Result.AddObject('ImageButton48', Pointer(@ImageButton48));
  Result.AddObject('ImageButton50', Pointer(@ImageButton50));
  Result.AddObject('ImageButton53', Pointer(@ImageButton53));
  Result.AddObject('ImageButton54', Pointer(@ImageButton54));
  Result.AddObject('ImageButton55', Pointer(@ImageButton55));
  Result.AddObject('ImageButton56', Pointer(@ImageButton56));
  Result.AddObject('ImageButton57', Pointer(@ImageButton57));
  Result.AddObject('ImageButton58', Pointer(@ImageButton58));
  Result.AddObject('ImageButton59', Pointer(@ImageButton59));
  Result.AddObject('ImageButton61', Pointer(@ImageButton61));
  Result.AddObject('Label1', Pointer(@Label1));
  Result.AddObject('Label21', Pointer(@Label21));
  Result.AddObject('Label22', Pointer(@Label22));
  Result.AddObject('Label23', Pointer(@Label23));
  Result.AddObject('Label24', Pointer(@Label24));
  Result.AddObject('DOptTs4', Pointer(@DOptTs4));
  Result.AddObject('Label25', Pointer(@Label25));
  Result.AddObject('Label26', Pointer(@Label26));
  Result.AddObject('Label27', Pointer(@Label27));
  Result.AddObject('Label28', Pointer(@Label28));
  Result.AddObject('Label29', Pointer(@Label29));
  Result.AddObject('ListView1', Pointer(@ListView1));
  Result.AddObject('Label30', Pointer(@Label30));
  Result.AddObject('Label31', Pointer(@Label31));
end;

procedure TMirConfigDlg.MouseMoveEvent(Sender:TObject; Shift:TShiftState; X, Y:Integer);
begin
  DScreen.ClearHint;
end;

procedure TMirConfigDlg.LoadClientConfig(ClientConfig:pTClientConfig);
var
  I, nTop:Integer;
  List:TList;
  DxControl:TDxImageButton;
  VisibleCount:Integer;
begin
  FClientConfig := ClientConfig^;
  PlugMemoConfig1.Position := 0;
  PlugMemoConfig2.Position := 0;
  PlugMemoConfig3.Position := 0;
  PlugMemoConfig4.Position := 0;
  PlugMemoConfig5.Position := 0;

  for I := 0 to Length(FClientConfig.UseSuperMedicaItemNames) - 1 do begin
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
  for I := 0 to List.Count - 1 do begin
    DxControl := TDxImageButton(List.Items[I]);
    DxControl.Caption := g_Config.SuperMedicaItemNames[I];
    DxControl.Visible := g_Config.SuperMedicaItemNames[I] <> '';
    if DxControl.Visible then Inc(VisibleCount);
  end;

  // 重排下面一块的坐标------------------------------------------------------------
  if (not g_ConfigClient.boCustomUI) or (not g_ConfigClient.boCustomConfigDlg) then begin
    //HZQ 20230601 重排内挂的各个控件的坐标在此进行，新加的火墙淡化，自动绕路，隐藏怪物顶戴花翎还没有加入重排
    if (g_ClientVersion <> cvMirNewUI205) then begin
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
    end else begin
      nTop := PlugMemoConfig4Line4.Top;
      nTop := nTop + VisibleCount * 25 + 20;

      PlugCheckBoxAutoPercent.Top := nTop;
      PlugMemoConfig4Label1.Top := nTop + 25;
      PlugMemoConfig4Line2.Top := nTop + 40;

      PlugCheckBoxCheckHPIsAuto.Top := nTop + 50;
      PlugEditCheckHPPercent.Top := nTop + 51;
      PlugMemoConfig4Label3.Top := nTop + 54;
      PlugComboBoxCheckHPValue.Top := nTop + 51;

      PlugCheckBoxCheckMPIsAuto.Top := nTop + 75;
      PlugEditCheckMPPercent.Top := nTop + 76;
      PlugMemoConfig4Label4.Top := nTop + 79;
      PlugComboBoxCheckMPValue.Top := nTop + 76;

      PlugMemoConfig4Label5.Top := nTop + 110;
      PlugMemoConfig4Line5.Top := nTop + 125;

      PlugCheckBoxCheckDuraIsAuto.Top := nTop + 135;
      PlugEditCheckDura.Top := nTop + 136;
      PlugMemoConfig4Label7.Top := nTop + 139;
      PlugEditCheckDuraValue.Top := nTop + 136;

      PlugMemoConfig4Label6.Top := nTop + 165;
      PlugEditCheckDuraTime.Top := nTop + 161;
      PlugMemoConfig4Label8.Top := nTop + 165;

      PlugMemoConfig4LabelHint.Top := nTop + 200;
      PlugMemoConfig4LabelHint2.Top := nTop + 220;
    end;
  end;

  //---------------------------------------------------------------------------

  RefreshUnBindItemList;

  if PlugComboBoxCheckHPValue.ItemIndex >= PlugComboBoxCheckHPValue.Items.Count then begin
    PlugComboBoxCheckHPValue.ItemIndex := -1;
    g_Config.CheckHPValues[g_Config.MedicaMode] := -1;
  end else if PlugComboBoxCheckHPValue.ItemIndex >= 0 then begin
    PlugComboBoxCheckHPValue.Text := PlugComboBoxCheckHPValue.Items[PlugComboBoxCheckHPValue.ItemIndex];
  end else begin
    PlugComboBoxCheckHPValue.Text := '';
  end;

  if PlugComboBoxCheckMPValue.ItemIndex >= PlugComboBoxCheckMPValue.Items.Count then begin
    PlugComboBoxCheckMPValue.ItemIndex := -1;
    g_Config.CheckMPValues[g_Config.MedicaMode] := -1;
  end else if PlugComboBoxCheckMPValue.ItemIndex >= 0 then begin
    PlugComboBoxCheckMPValue.Text := PlugComboBoxCheckMPValue.Items[PlugComboBoxCheckMPValue.ItemIndex];
  end else begin
    PlugComboBoxCheckMPValue.Text := '';
  end;

  RefUseItemConfig(g_Config.MedicaMode);

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
  PlugTabSheetConfig80.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[8];
  PlugTabSheetConfig9.TabVisible := FClientConfig.ClientConfigTabSheetVisibles[9];

  if (g_ConfigClient.boCustomUI) and (g_ConfigClient.boCustomConfigDlg) then begin
    //HZQ 2023-06-02 10:12:44 原来的代码赋值逻辑是反的，此处已更正
    PlugCheckBoxHideItemEffect.Visible := g_ClientConfig.boHideItemEffect;

    PlugCheckBoxItemCmp.Visible := g_ClientConfig.boItemCompare;
    PlugCheckBoxBagFastItemCmp.Visible := g_ClientConfig.boBagFastItemCompare;
    PlugCheckBoxPickAll.Visible := g_ClientConfig.boPickupAll;
    PlugCheckBoxMagicLock.Visible := g_ClientConfig.boMagicLock;
    PlugCheckBoxDisableSelfStruck.Visible := g_ClientConfig.boDisableSelfStruck;
    //PlugCheckBoxShowValueItemEffect.Visible := g_ClientConfig.boShowDropValueItemEff;

    PlugCheckBoxShowActorName.Visible := g_ClientConfig.boShowUserName;
    PlugCheckBoxHideDescUserName.Visible := g_ClientConfig.boOnlyShowCharName;
    PlugCheckBoxNumberLable.Visible := g_ClientConfig.boShowNumberLable;
    PlugCheckBoxShowHealthNumber.Visible := g_ClientConfig.boShowMoveLable;
    PlugCheckBoxShowHPLabel.Visible := g_ClientConfig.boShowHPLabel;
    PlugCheckBoxShowHighlightHPLabel.Visible := g_ClientConfig.boShowHighlightHPLabel;
    PlugCheckBoxShowHPUnit.Visible := g_ClientConfig.boShowHPUnit;
    PlugCheckBoxShowNGLabel.Visible := g_ClientConfig.boShowNGLabel;
    PlugCheckBoxJobAndLevel.Visible := g_ClientConfig.boShowJobAndLevel;
    PlugCheckBoxShowMonName.Visible := g_ClientConfig.boShowMonName;
    PlugCheckBoxShowNpcName.Visible := g_ClientConfig.boShowNpcName;
    PlugCheckBoxShowNpcHPLabel.Visible := g_ClientConfig.boShowNpcHPLabel;
    PlugCheckBoxAutoPickUpItem.Visible := g_ClientConfig.boAutoPickUpItem;
    PlugCheckBoxNoCaton.Visible := g_ClientConfig.boNoCaton;

    PlugCheckBoxNoShift.Visible := g_ClientConfig.boNotNeedShift;
    PlugCheckBoxShiftSwitch.Visible := g_ClientConfig.boShiftSwitch;
    PlugCheckBoxExpFilter.Visible := g_ClientConfig.boFilterExp;
    PlugEditExpFilter.Visible := PlugCheckBoxExpFilter.Visible;
    PlugCheckBoxHideTitle.Visible := g_ClientConfig.boHideTitle;
    PlugCheckBoxHideGhost.Visible := g_ClientConfig.boHideGhost;
    PlugCheckBoxHideHumEffect.Visible := g_ClientConfig.boHideHumEffect;
    PlugCheckBoxHideWeaponEffect.Visible := g_ClientConfig.boHideWeaponEffect;
    PlugCheckBoxHideActorIcons.Visible := g_ClientConfig.boHideActorIcons;
    PlugCheckDisableChartMemoSize.Visible := g_ClientConfig.boDisableChartMemoSize;
    PlugCheckBoxBGMusic.Visible := g_ClientConfig.boBGMusic;
    PlugCheckBoxRepeatBGMusic.Visible := g_ClientConfig.boRepeatBGMusic;
    PlugCheckBoxVolume.Visible := g_ClientConfig.boVolume;
    TrackBarVolume.Visible := g_ClientConfig.boVolume; //HZQ 20230602 音量控制条和CheckBox一起控制

    PlugCheckBoxShowGreenHint.Visible := g_ClientConfig.boShowGreenHint;
    PlugCheckBoxSpeedSlow.Visible := g_ClientConfig.boSpeedSlow;
    PlugCheckBoxDuraWarning.Visible := g_ClientConfig.boDuraWarning;
    PlugCheckBoxShowMimiMapDesc.Visible := g_ClientConfig.boShowMapDesc;
    PlugCheckBoxDisableDeal.Visible := g_ClientConfig.boDisableDeal;
    PlugCheckBoxNotParaly.Visible := g_ClientConfig.boNotParaly;
    PlugCheckBoxContinueButchItem.Visible := g_ClientConfig.boContinueButchItem;
    PlugCheckBoxAutoOrderItem.Visible := g_ClientConfig.boAutoOrderItem;
    PlugCheckBoxSceneShake.Visible := g_ClientConfig.boSceneShake;
    PlugCheckBoxUpdateStatus.Visible := g_ClientConfig.boShowUpdateStatus;
    PlugCheckSimpleShowHumanDress.Visible := g_ClientConfig.boSimpleShowHumanDress;
    PlugCheckSimpleShowHumanWeapon.Visible := g_ClientConfig.boSimpleShowHumanWeapon;
    PlugCheckSimpleShowActor.Visible := g_ClientConfig.boSimpleShowActor;
    PlugCheckSimpleShowBB.Visible := g_ClientConfig.boSimpleShowBB;

    PlugCheckBoxSmartLongHit.Visible := g_ClientConfig.boSmartLongHit;
    PlugCheckBoxSmartPosLongHit.Visible := g_ClientConfig.boSmartPosLongHit;
    PlugCheckBoxSmartWalkLongHit.Visible := g_ClientConfig.boSmartWalkLongHit;
    PlugCheckBoxSmartWideHit.Visible := g_ClientConfig.boSmartWideHit;
    PlugCheckBoxSmartFireHit.Visible := g_ClientConfig.boSmartFireHit;
    PlugCheckBoxSmartSwordHit.Visible := g_ClientConfig.boSmartSwordHit;
    PlugCheckBoxSmartKTZHit.Visible := g_ClientConfig.boSmart66Hit;
    PlugCheckBoxSmartCRSHit.Visible := g_ClientConfig.boSmartCrsHit;
    PlugCheckBoxSmartTWNHit.Visible := g_ClientConfig.boSmartTwnHit;
    PlugCheckBoxAutoGroupAttack.Visible := g_ClientConfig.boAutoGroupAttack;
    PlugCheckBoxAutoGroupNoAttackMon.Visible := g_ClientConfig.boAutoGroupNoAttackMon;

    PlugCheckBoxAutoHideMode.Visible := g_ClientConfig.boAutoHideMode;
    PlugCheckBoxAutoCHangePoison.Visible := g_ClientConfig.boAutoCHangePoison;

    PlugCheckBoxHumAutoShield.Visible := g_ClientConfig.boHumAutoShield;
    PlugCheckBoxHumStruckShield.Visible := g_ClientConfig.boHumStruckShield;
    PlugCheckBoxHeroAutoShield.Visible := g_ClientConfig.boHeroAutoShield;
    PlugCheckBoxAssistantHeroAutoShield.Visible := g_ClientConfig.boAssistantHeroAutoShield;

    PlugCheckBoxHumManuallySnowWind.Visible := g_ClientConfig.boHumManuallySnowWind;
    PlugCheckBoxHumManuallyFireBoom.Visible := g_ClientConfig.boHumManuallyFireBoom;
    PlugCheckBoxHumShootLightenLockTarget.Visible := g_ClientConfig.boHumShootLightenLockTarget;
    PlugCheckBoxHumManuallyMeteorShower.Visible := g_ClientConfig.boHumManuallyMeteorShower;
    PlugCheckBoxHumManuallyMove10Attack.Visible := g_ClientConfig.boHumManuallyMove10Attack;
    PlugCheckBoxHumManuallyFire.Visible := g_ClientConfig.boHumManuallyFire;

    PlugCheckBoxHideMonsterIcons.Visible := g_ClientConfig.boHideMonsterIcons; //HZQ 20230601  隐藏怪物顶戴花翎
    PlugCheckBoxDimFireEffect.Visible := g_ClientConfig.boDimFireEffect; //HZQ 20230601  火墙淡化
    PlugCheckBoxAutoDetourPath.Visible := g_ClientConfig.boAutoDetourPath; //HZQ 20230601 自动绕路

    //HZQ 20230602 赋值交换到此结束

  end else begin
    PlugCheckBoxHideItemEffect.Visible := FClientConfig.boHideItemEffect;

    PlugCheckBoxItemCmp.Visible := FClientConfig.boItemCompare;
    PlugCheckBoxBagFastItemCmp.Visible := FClientConfig.boBagFastItemCompare;
    PlugCheckBoxPickAll.Visible := FClientConfig.boPickupAll;
    PlugCheckBoxMagicLock.Visible := FClientConfig.boMagicLock;
    PlugCheckBoxDisableSelfStruck.Visible := FClientConfig.boDisableSelfStruck;
    PlugCheckBoxShowValueItemEffect.Visible := FClientConfig.boShowDropValueItemEff;

    if (g_ClientVersion <> cvMirNewUI205) then begin //所有版本都走这个分支，不单独区分205，205分支注释掉了
      PlugCheckBoxNoCaton.Visible := FClientConfig.boNoCaton; //HZQ ????攻击不卡断不在这一列

      //HZQ 非205效果
      List.Clear;
      List.Add(PlugCheckBoxShowActorName);
      List.Add(PlugCheckBoxHideDescUserName);
      List.Add(PlugCheckBoxNumberLable);
      List.Add(PlugCheckBoxShowHealthNumber);
      List.Add(PlugCheckBoxShowHPLabel);
      List.Add(PlugCheckBoxShowHighlightHPLabel);
      List.Add(PlugCheckBoxShowHPUnit);
      List.Add(PlugCheckBoxShowNGLabel);
      List.Add(PlugCheckBoxJobAndLevel);
      List.Add(PlugCheckBoxShowMonName);
      List.Add(PlugCheckBoxShowNpcName);
      List.Add(PlugCheckBoxShowNpcHPLabel);
      List.Add(PlugCheckBoxAutoPickUpItem);
      List.Add(PlugCheckBoxDimFireEffect); //HZQ 20230601 墙淡化

      PlugCheckBoxShowActorName.Visible := FClientConfig.boShowUserName;
      PlugCheckBoxHideDescUserName.Visible := FClientConfig.boOnlyShowCharName;
      PlugCheckBoxNumberLable.Visible := FClientConfig.boShowNumberLable;
      PlugCheckBoxShowHealthNumber.Visible := FClientConfig.boShowMoveLable;
      PlugCheckBoxShowHPLabel.Visible := FClientConfig.boShowHPLabel;
      PlugCheckBoxShowHighlightHPLabel.Visible := FClientConfig.boShowHighlightHPLabel;
      PlugCheckBoxShowHPUnit.Visible := FClientConfig.boShowHPUnit;
      PlugCheckBoxShowNGLabel.Visible := FClientConfig.boShowNGLabel;
      PlugCheckBoxJobAndLevel.Visible := FClientConfig.boShowJobAndLevel;
      PlugCheckBoxShowMonName.Visible := FClientConfig.boShowMonName;
      PlugCheckBoxShowNpcName.Visible := FClientConfig.boShowNpcName; // 显示NPC名 piaoyun 2013-07-31
      PlugCheckBoxShowNpcHPLabel.Visible := FClientConfig.boShowNpcHPLabel;
      PlugCheckBoxAutoPickUpItem.Visible := FClientConfig.boAutoPickUpItem;
      PlugCheckBoxDimFireEffect.Visible := FClientConfig.boDimFireEffect; //HZQ 20230601

      nTop := 12;
      for I := 0 to List.Count - 1 do begin
        DxControl := TDxImageButton(List.Items[I]);
        if not DxControl.Visible then Continue;
        DxControl.Top := nTop;
        Inc(nTop, 22); //此时可能需要减少间距或别的处理方法 //HZQ20230601 没列添加了一个Checkbox
      end;

      List.Clear;
      List.Add(PlugCheckBoxNoShift);
      List.Add(PlugCheckBoxShiftSwitch);
      List.Add(PlugCheckBoxExpFilter);
      List.Add(PlugEditExpFilter);
      List.Add(PlugCheckBoxHideTitle);
      List.Add(PlugCheckBoxHideGhost);
      List.Add(PlugCheckBoxHideHumEffect);
      List.Add(PlugCheckBoxHideWeaponEffect);
      List.Add(PlugCheckBoxHideActorIcons);
      List.Add(PlugCheckBoxHideMonsterIcons); //HZQ 20230601 隐藏怪物顶戴花翎
      List.Add(PlugCheckDisableChartMemoSize);
      List.Add(PlugCheckBoxBGMusic);
      List.Add(PlugCheckBoxRepeatBGMusic);
      List.Add(PlugCheckBoxVolume);

      PlugCheckBoxNoShift.Visible := FClientConfig.boNotNeedShift;
      PlugCheckBoxShiftSwitch.Visible := FClientConfig.boShiftSwitch;
      PlugCheckBoxExpFilter.Visible := FClientConfig.boFilterExp;
      PlugEditExpFilter.Visible := PlugCheckBoxExpFilter.Visible;
      PlugCheckBoxHideTitle.Visible := FClientConfig.boHideTitle;
      PlugCheckBoxHideGhost.Visible := FClientConfig.boHideGhost;
      PlugCheckBoxHideHumEffect.Visible := FClientConfig.boHideHumEffect;
      PlugCheckBoxHideWeaponEffect.Visible := FClientConfig.boHideWeaponEffect;
      PlugCheckBoxHideActorIcons.Visible := FClientConfig.boHideActorIcons;
      PlugCheckBoxHideMonsterIcons.Visible := FClientConfig.boHideMonsterIcons; //HZQ 20230601 隐藏怪物顶戴
      PlugCheckDisableChartMemoSize.Visible := FClientConfig.boDisableChartMemoSize;
      PlugCheckBoxBGMusic.Visible := FClientConfig.boBGMusic;
      PlugCheckBoxRepeatBGMusic.Visible := FClientConfig.boRepeatBGMusic;
      PlugCheckBoxVolume.Visible := FClientConfig.boVolume;

      nTop := 12;
      for I := 0 to List.Count - 1 do begin
        DxControl := TDxImageButton(List.Items[I]);
        if not DxControl.Visible then Continue;
        DxControl.Top := nTop;
        Inc(nTop, 22);
      end;

      TrackBarVolume.Visible := PlugCheckBoxVolume.Visible;
      TrackBarVolume.Top := PlugCheckBoxVolume.Top + 2;

      List.Clear;
      List.Add(PlugCheckBoxShowGreenHint);
      List.Add(PlugCheckBoxSpeedSlow);
      List.Add(PlugCheckBoxDuraWarning);
      List.Add(PlugCheckBoxShowMimiMapDesc);
      List.Add(PlugCheckBoxDisableDeal);
      List.Add(PlugCheckBoxNotParaly);
      List.Add(PlugCheckBoxContinueButchItem);
      List.Add(PlugCheckBoxAutoOrderItem);
      List.Add(PlugCheckBoxSceneShake);
      List.Add(PlugCheckBoxUpdateStatus);
      List.Add(PlugCheckSimpleShowHumanDress);
      List.Add(PlugCheckSimpleShowHumanWeapon);
      List.Add(PlugCheckSimpleShowActor);
      List.Add(PlugCheckSimpleShowBB);
      List.Add(PlugCheckBoxAutoDetourPath); //HZQ 20230601 自动绕路

      PlugCheckBoxShowGreenHint.Visible := FClientConfig.boShowGreenHint;
      PlugCheckBoxSpeedSlow.Visible := FClientConfig.boSpeedSlow;
      PlugCheckBoxDuraWarning.Visible := FClientConfig.boDuraWarning;
      PlugCheckBoxShowMimiMapDesc.Visible := FClientConfig.boShowMapDesc;
      PlugCheckBoxDisableDeal.Visible := FClientConfig.boDisableDeal;
      PlugCheckBoxNotParaly.Visible := FClientConfig.boNotParaly;
      PlugCheckBoxContinueButchItem.Visible := FClientConfig.boContinueButchItem;
      PlugCheckBoxAutoOrderItem.Visible := FClientConfig.boAutoOrderItem;
      PlugCheckBoxSceneShake.Visible := FClientConfig.boSceneShake; // 屏幕震动控件是否可用-- 同步引擎 piaoyun 2013-09-15
      PlugCheckBoxUpdateStatus.Visible := FClientConfig.boShowUpdateStatus;
      PlugCheckSimpleShowHumanDress.Visible := FClientConfig.boSimpleShowHumanDress;
      PlugCheckSimpleShowHumanWeapon.Visible := FClientConfig.boSimpleShowHumanWeapon;
      PlugCheckSimpleShowActor.Visible := FClientConfig.boSimpleShowActor;
      PlugCheckSimpleShowBB.Visible := FClientConfig.boSimpleShowBB;
      PlugCheckBoxAutoDetourPath.VisiBle := FClientConfig.boAutoDetourPath; //HZQ 20230601 自动绕路

      nTop := 12;
      for I := 0 to List.Count - 1 do begin
        DxControl := TDxImageButton(List.Items[I]);
        if not DxControl.Visible then Continue;
        DxControl.Top := nTop;
        Inc(nTop, 22);
      end;
    end else begin
      //HZQ MIRConfig205应该时有些选项没有，需要在服务器上设置隐藏,

      //第一列
      List.Clear;
      List.Add(PlugCheckBoxShowActorName);
      List.Add(PlugCheckBoxHideDescUserName);
      List.Add(PlugCheckBoxNumberLable);
      List.Add(PlugCheckBoxShowHealthNumber);
      List.Add(PlugCheckBoxShowHPLabel);
      List.Add(PlugCheckBoxShowHighlightHPLabel);
      List.Add(PlugCheckBoxShowHPUnit);
      List.Add(PlugCheckBoxShowNGLabel);
      List.Add(PlugCheckBoxJobAndLevel);
      List.Add(PlugCheckBoxAutoPickUpItem);
      List.Add(PlugCheckBoxDimFireEffect); //HZQ 20230601 火墙淡化

      PlugCheckBoxShowActorName.Visible := FClientConfig.boShowUserName;
      PlugCheckBoxHideDescUserName.Visible := FClientConfig.boOnlyShowCharName;
      PlugCheckBoxNumberLable.Visible := FClientConfig.boShowNumberLable;
      PlugCheckBoxShowHealthNumber.Visible := FClientConfig.boShowMoveLable;
      PlugCheckBoxShowHPLabel.Visible := FClientConfig.boShowHPLabel;
      PlugCheckBoxShowHighlightHPLabel.Visible := FClientConfig.boShowHighlightHPLabel;
      PlugCheckBoxShowHPUnit.Visible := FClientConfig.boShowHPUnit;
      PlugCheckBoxShowNGLabel.Visible := FClientConfig.boShowNGLabel;                                   // 显示内功黄条 piaoyun 2013-09-09
      PlugCheckBoxJobAndLevel.Visible := FClientConfig.boShowJobAndLevel;
      PlugCheckBoxAutoPickUpItem.Visible := FClientConfig.boAutoPickUpItem;
      PlugCheckBoxDimFireEffect.Visible := FClientConfig.boDimFireEffect; //HZQ 20230601 火墙淡化

      PlugCheckBoxNoCaton.Visible := FClientConfig.boNoCaton;

      nTop := 12;
      for I := 0 to List.Count - 1 do begin
        DxControl := TDxImageButton(List.Items[I]);
        if not DxControl.Visible then Continue;
        DxControl.Top := nTop;
        Inc(nTop, 25);
      end;

      //第二列
      List.Clear;
      List.Add(PlugCheckBoxShowMonName);
      List.Add(PlugCheckBoxShowNpcName);
      List.Add(PlugCheckBoxShowNpcHPLabel);
      List.Add(PlugCheckSimpleShowHumanDress);
      List.Add(PlugCheckSimpleShowHumanWeapon);
      List.Add(PlugCheckSimpleShowActor);
      List.Add(PlugCheckSimpleShowBB);
      List.Add(PlugCheckBoxHideTitle);
      List.Add(PlugCheckBoxHideGhost);
      List.Add(PlugCheckBoxHideHumEffect);
      List.Add(PlugCheckBoxHideWeaponEffect);
      List.Add(PlugCheckBoxHideActorIcons);


      PlugCheckBoxShowMonName.Visible := FClientConfig.boShowMonName;
      PlugCheckBoxShowNpcName.Visible := FClientConfig.boShowNpcName;
      PlugCheckBoxShowNpcHPLabel.Visible := FClientConfig.boShowNpcHPLabel;
      PlugCheckSimpleShowHumanDress.Visible := FClientConfig.boSimpleShowHumanDress;
      PlugCheckSimpleShowHumanWeapon.Visible := FClientConfig.boSimpleShowHumanWeapon;
      PlugCheckSimpleShowActor.Visible := FClientConfig.boSimpleShowActor;
      PlugCheckSimpleShowBB.Visible := FClientConfig.boSimpleShowBB;
      PlugCheckBoxHideTitle.Visible := FClientConfig.boHideTitle;
      PlugCheckBoxHideGhost.Visible := FClientConfig.boHideGhost;
      PlugCheckBoxHideHumEffect.Visible := FClientConfig.boHideHumEffect;
      PlugCheckBoxHideWeaponEffect.Visible := FClientConfig.boHideWeaponEffect;
      PlugCheckBoxHideActorIcons.Visible := FClientConfig.boHideActorIcons;

      

      nTop := 12;
      for I := 0 to List.Count - 1 do begin
        DxControl := TDxImageButton(List.Items[I]);
        if not DxControl.Visible then Continue;
        DxControl.Top := nTop;
        Inc(nTop, 25);
      end;

      //第三列
      List.Clear;
      List.Add(PlugCheckBoxSpeedSlow);
      List.Add(PlugCheckBoxNoShift);
      List.Add(PlugCheckBoxShiftSwitch);
      List.Add(PlugCheckBoxDuraWarning);
      List.Add(PlugCheckBoxShowMimiMapDesc);
      List.Add(PlugCheckBoxDisableDeal);
      List.Add(PlugCheckBoxNotParaly);
      List.Add(PlugCheckBoxContinueButchItem);
      List.Add(PlugCheckBoxAutoOrderItem);
      List.Add(PlugCheckBoxAutoDetourPath); //HZQ 20230601 自动绕行
      List.Add(PlugCheckBoxHideMonsterIcons); //HZQ 20230601 隐藏怪物顶戴
      
      PlugCheckBoxSpeedSlow.Visible := FClientConfig.boSpeedSlow;
      PlugCheckBoxNoShift.Visible := FClientConfig.boNotNeedShift;
      PlugCheckBoxShiftSwitch.Visible := FClientConfig.boShiftSwitch;
      PlugCheckBoxDuraWarning.Visible := FClientConfig.boDuraWarning;
      PlugCheckBoxShowMimiMapDesc.Visible := FClientConfig.boShowMapDesc;
      PlugCheckBoxDisableDeal.Visible := FClientConfig.boDisableDeal;
      PlugCheckBoxNotParaly.Visible := FClientConfig.boNotParaly;
      PlugCheckBoxContinueButchItem.Visible := FClientConfig.boContinueButchItem;
      PlugCheckBoxAutoOrderItem.Visible := FClientConfig.boAutoOrderItem;
      PlugCheckBoxHideMonsterIcons.Visible := FClientConfig.boHideMonsterIcons; //HZQ 20230601 隐藏怪物顶戴
      PlugCheckBoxAutoDetourPath.Visible := FClientConfig.boAutoDetourPath; //HZQ 20230601 自动绕行

      nTop := 12;
      for I := 0 to List.Count - 1 do begin
        DxControl := TDxImageButton(List.Items[I]);
        if not DxControl.Visible then Continue;
        DxControl.Top := nTop;
        Inc(nTop, 25);
      end;

      //第四列
      List.Clear;
      List.Add(PlugCheckBoxShowGreenHint);
      List.Add(PlugCheckBoxSceneShake);
      List.Add(PlugCheckBoxUpdateStatus);
      List.Add(PlugCheckBoxExpFilter);
      List.Add(PlugEditExpFilter);
      List.Add(PlugCheckDisableChartMemoSize);
      List.Add(PlugCheckBoxBGMusic);
      List.Add(PlugCheckBoxRepeatBGMusic);
      List.Add(PlugCheckBoxVolume);

      PlugCheckBoxShowGreenHint.Visible := FClientConfig.boShowGreenHint;
      PlugCheckBoxSceneShake.Visible := FClientConfig.boSceneShake;                                     // 屏幕震动控件是否可用-- 同步引擎 piaoyun 2013-09-15
      PlugCheckBoxUpdateStatus.Visible := FClientConfig.boShowUpdateStatus;
      PlugCheckBoxExpFilter.Visible := FClientConfig.boFilterExp;
      PlugEditExpFilter.Visible := PlugCheckBoxExpFilter.Visible;
      PlugCheckDisableChartMemoSize.Visible := FClientConfig.boDisableChartMemoSize;
      PlugCheckBoxBGMusic.Visible := FClientConfig.boBGMusic;
      PlugCheckBoxRepeatBGMusic.Visible := FClientConfig.boRepeatBGMusic;
      PlugCheckBoxVolume.Visible := FClientConfig.boVolume;

      nTop := 12;
      for I := 0 to List.Count - 1 do begin
        DxControl := TDxImageButton(List.Items[I]);
        if not DxControl.Visible then Continue;
        DxControl.Top := nTop;
        Inc(nTop, 25);
      end;

      TrackBarVolume.Visible := PlugCheckBoxVolume.Visible;
      TrackBarVolume.Top := PlugCheckBoxVolume.Top + 2;

    end;

    List.Clear;
    List.Add(PlugCheckBoxSmartWideHit);
    List.Add(PlugCheckBoxSmartFireHit);
    List.Add(PlugCheckBoxSmartSwordHit);
    List.Add(PlugCheckBoxSmartKTZHit);
    List.Add(PlugCheckBoxSmartCRSHit);
    List.Add(PlugCheckBoxSmartTWNHit);
    List.Add(PlugCheckBoxSmart113Hit);

    List.Add(PlugCheckBoxAutoCustomHit1);
    List.Add(PlugCheckBoxAutoCustomHit2);
    List.Add(PlugCheckBoxAutoCustomHit3);
    List.Add(PlugCheckBoxAutoCustomHit4);
    List.Add(PlugCheckBoxAutoCustomHit5);
    List.Add(PlugCheckBoxAutoCustomHit6);
    List.Add(PlugCheckBoxAutoCustomHit7);
    List.Add(PlugCheckBoxAutoCustomHit8);

    List.Add(PlugCheckBoxAutoGroupAttack);
    List.Add(PlugCheckBoxAutoGroupNoAttackMon);
    List.Add(PlugCheckBoxAutoOpenSpell);
    List.Add(PlugCheckBoxAutoContinueAttack);

    PlugCheckBoxSmartWideHit.Visible := FClientConfig.boSmartWideHit;
    PlugCheckBoxSmartFireHit.Visible := FClientConfig.boSmartFireHit;
    PlugCheckBoxSmartSwordHit.Visible := FClientConfig.boSmartSwordHit;
    PlugCheckBoxSmartKTZHit.Visible := FClientConfig.boSmart66Hit;
    PlugCheckBoxSmartCRSHit.Visible := FClientConfig.boSmartCrsHit;
    PlugCheckBoxSmartTWNHit.Visible := FClientConfig.boSmartTwnHit;
    PlugCheckBoxSmart113Hit.Visible := FClientConfig.boSmart113Hit;

    PlugCheckBoxAutoCustomHit1.Visible := FClientConfig.boSmartCustomHit1;
    PlugCheckBoxAutoCustomHit2.Visible := FClientConfig.boSmartCustomHit2;
    PlugCheckBoxAutoCustomHit3.Visible := FClientConfig.boSmartCustomHit3;
    PlugCheckBoxAutoCustomHit4.Visible := FClientConfig.boSmartCustomHit4;
    PlugCheckBoxAutoCustomHit5.Visible := FClientConfig.boSmartCustomHit5;
    PlugCheckBoxAutoCustomHit6.Visible := FClientConfig.boSmartCustomHit6;
    PlugCheckBoxAutoCustomHit7.Visible := FClientConfig.boSmartCustomHit7;
    PlugCheckBoxAutoCustomHit8.Visible := FClientConfig.boSmartCustomHit8;

    PlugCheckBoxAutoGroupAttack.Visible := FClientConfig.boAutoGroupAttack;
    PlugCheckBoxAutoGroupNoAttackMon.Visible := FClientConfig.boAutoGroupNoAttackMon;
    PlugCheckBoxAutoOpenSpell.Visible := FClientConfig.boAutoOpenSpell;
    PlugCheckBoxAutoContinueAttack.Visible := FClientConfig.boAutoContinueAttack;

    nTop := 36;
    for I := 0 to List.Count - 1 do begin
      DxControl := TDxImageButton(List.Items[I]);
      if not DxControl.Visible then Continue;
      DxControl.Top := nTop;
      Inc(nTop, 24);
    end;

    List.Clear;
    List.Add(PlugCheckBoxSmartLongHit);
    List.Add(PlugCheckBoxSmartPosLongHit);
    List.Add(PlugCheckBoxSmartWalkLongHit);

    PlugCheckBoxSmartLongHit.Visible := FClientConfig.boSmartLongHit;
    PlugCheckBoxSmartPosLongHit.Visible := FClientConfig.boSmartPosLongHit;
    PlugCheckBoxSmartWalkLongHit.Visible := FClientConfig.boSmartWalkLongHit;

    nTop := 36;
    for I := 0 to List.Count - 1 do begin
      DxControl := TDxImageButton(List.Items[I]);
      if not DxControl.Visible then Continue;
      DxControl.Top := nTop;
      Inc(nTop, 24);
    end;

    List.Clear;
    List.Add(PlugCheckBoxAutoHideMode);
    List.Add(PlugCheckBoxAutoCHangePoison);
    PlugCheckBoxAutoHideMode.Visible := FClientConfig.boAutoHideMode;
    PlugCheckBoxAutoCHangePoison.Visible := FClientConfig.boAutoCHangePoison;

    nTop := 132;
    for I := 0 to List.Count - 1 do begin
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

    nTop := 204;
    for I := 0 to List.Count - 1 do begin
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
    List.Add(PlugCheckBoxHumManuallyMove10Attack);
    List.Add(PlugCheckBoxHumManuallyFire);
    PlugCheckBoxHumManuallySnowWind.Visible := FClientConfig.boHumManuallySnowWind;
    PlugCheckBoxHumManuallyFireBoom.Visible := FClientConfig.boHumManuallyFireBoom;
    PlugCheckBoxHumShootLightenLockTarget.Visible := FClientConfig.boHumShootLightenLockTarget;
    PlugCheckBoxHumManuallyMeteorShower.Visible := FClientConfig.boHumManuallyMeteorShower;
    PlugCheckBoxHumManuallyMove10Attack.Visible := FClientConfig.boHumManuallyMove10Attack;
    PlugCheckBoxHumManuallyFire.Visible := FClientConfig.boHumManuallyFire;

    nTop := 108;
    for I := 0 to List.Count - 1 do begin
      DxControl := TDxImageButton(List.Items[I]);
      if not DxControl.Visible then Continue;
      DxControl.Top := nTop;
      Inc(nTop, 24);
    end;
  end;

  List.Free;
end;

procedure TMirConfigDlg.Initialize(Handle:THandle; ScreenMode:Byte; ClientVersion:TClientVersion; WindowMode:Boolean);
{$IF TESTMODE = 1}
var
  MemoryStream:TMemoryStream;
{$ELSEIF TESTMODE = 2}
var
  MemoryStream:TMemoryStream;
  Stream:TStream;
{$IFEND}
var
   ControlAddrList:THashedStringList;
begin
  FHandle := Handle;

  FScreenMode := ScreenMode;
  FClientVersion := ClientVersion;
  FWindowMode := WindowMode;

  if not FLoadControl then begin
    FLoadControl := True;
    {$IF TESTMODE = 0}
    if g_ConfigDlgUIStream <> nil then begin
      g_ConfigDlgUIStream.Position := 0;

      ControlAddrList := Self.MakeControlAddressList;
      GameConfigDlgs.LoadControlFromStream(ControlAddrList, g_ConfigDlgUIStream); //HZQ 20230615 把按地址加载，改为按名称加载，使用地址表，不用读取RTTI信息
      PatchAddNearEffectCheckBox(); //HZQ 20230829 补丁增加一个CheckBox
      ControlAddrList.Free;

      FreeAndNil(g_ConfigDlgUIStream);
    end;
    {$ELSEIF TESTMODE = 1}
    MemoryStream := TMemoryStream.Create;

    if g_ClientVersion <> cvMirNewUI205 then begin
      if g_TestModeUseCustomUI then begin
        MemoryStream.LoadFromFile(g_TestModeUIPath + 'MirConfigDlg.UI')
      end else begin
        MemoryStream.LoadFromFile(g_TestModeUIPath + 'MirConfigDlg.GUI')
      end;
    end else begin
      if g_TestModeUseCustomUI then begin
        MemoryStream.LoadFromFile(g_TestModeUIPath + 'MirConfigDlg205.UI');
      end else begin
        MemoryStream.LoadFromFile(g_TestModeUIPath + 'MirConfigDlg205.GUI');
      end;
    end;

    MemoryStream.Position := 0;
    ControlAddrList := Self.MakeControlAddressList;
    GameConfigDlgs.LoadControlFromStream(ControlAddrList, MemoryStream, 'MirConfigDlg');
    PatchAddNearEffectCheckBox(); //HZQ 20230829 补丁增加一个CheckBox
    ControlAddrList.Free;
    MemoryStream.Free; 

    {$ELSEIF TESTMODE = 2}
    MemoryStream := TMemoryStream.Create;
    try
      Stream := TResourceStream.Create(Hinstance, 'MIRCONFIGDLG', 'GUI');

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

  {
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
  }

  PlugConfigDlgClose.OnClick := PlugConfigDlgCloseClickEx;

  PlugPageControlConfig.OnInRealArea := PlugPageControlConfigInRealArea;

  //HZQ 20230602 火墙淡化、隐藏怪物顶戴、自动绕行添加事件
  PlugCheckBoxHideMonsterIcons.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoDetourPath.OnClick := CheckBoxClickEx;
  PlugCheckBoxDimFireEffect.OnClick := CheckBoxClickEx;

  PlugCheckBoxShowHPLabel.OnClick := CheckBoxClickEx;
  PlugCheckBoxNumberLable.OnClick := CheckBoxClickEx;
  PlugCheckBoxJobAndLevel.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowGreenHint.OnClick := CheckBoxClickEx;
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
  PlugCheckBoxHideActorIcons.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowMonName.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowNpcName.OnClick := CheckBoxClickEx; // 显示NPC名 piaoyun 2013-07-31
  PlugCheckBoxShowNpcHPLabel.OnClick := CheckBoxClickEx; // 显示NPC血条 piaoyun 2013-07-31
  PlugCheckBoxShowNGLabel.OnClick := CheckBoxClickEx; // 显示内功黄条 piaoyun 2013-09-09
  PlugCheckBoxAutoPickUpItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoPickUpItem.OnMouseMove := DControlMouseMoveShowHint;
  PlugCheckBoxNoCaton.OnClick := CheckBoxClickEx;

  PlugCheckBoxAutoOrderItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxMagicLock.OnClick := CheckBoxClickEx;

  PlugCheckBoxPickAll.OnClick := CheckBoxClickEx;
  PlugCheckBoxPickAll.OnMouseMove := DControlMouseMoveShowHint;

  PlugCheckBoxShowValueItemEffect.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowValueItemEffect.OnMouseMove := DControlMouseMoveShowHint;

  PlugCheckBoxNotParaly.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideTitle.OnClick := CheckBoxClickEx;
  PlugCheckBoxContinueButchItem.OnClick := CheckBoxClickEx;
  PlugCheckBoxUpdateStatus.OnClick := CheckBoxClickEx;

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
  PlugCheckBoxAutoCHangePoison.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumAutoShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumStruckShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxHeroAutoShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxAssistantHeroAutoShield.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallySnowWind.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyFireBoom.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumShootLightenLockTarget.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyMeteorShower.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyMove10Attack.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyFire.OnClick := CheckBoxClickEx;
  PlugCheckBoxHeroContinuousNoHitMon.OnClick := CheckBoxClickEx;
  PlugCheckBoxHeroShowNumberState.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoMagic.OnClick := CheckBoxClickEx;
  PlugCheckBoxUseKeyBoard.OnClick := CheckBoxClickEx;
  PlugCheckBoxShowHPUnit.OnClick := CheckBoxClickEx;
  PlugCheckBoxSmart113Hit.OnClick := CheckBoxClickEx;

  PlugCheckBoxAutoCustomHit1.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoCustomHit2.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoCustomHit3.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoCustomHit4.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoCustomHit5.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoCustomHit6.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoCustomHit7.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoCustomHit8.OnClick := CheckBoxClickEx;

  PlugCheckBoxHumManuallyCustomHit1.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyCustomHit2.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyCustomHit3.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyCustomHit4.OnClick := CheckBoxClickEx;
  PlugCheckBoxHumManuallyCustomHit5.OnClick := CheckBoxClickEx;

  PlugCheckBoxAutoOpenSpell.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoContinueAttack.OnClick := CheckBoxClickEx;

  PlugCheckBoxAutoGroupAttack.OnClick := CheckBoxClickEx;
  PlugCheckBoxAutoGroupNoAttackMon.OnClick := CheckBoxClickEx;

  PlugCheckBoxNearEffect.OnClick := CheckBoxClickEx; //HZQ 20230828 目标效果开关
  PlugCheckBoxNearHint.OnClick := CheckBoxClickEx; // 接近提示 piaoyun 2013-09-09
  PlugCheckBoxAutoLock.OnClick := CheckBoxClickEx; // 自动锁定 piaoyun 2013-09-09
  PlugCheckBoxColorShow.OnClick := CheckBoxClickEx; // 变色显示 piaoyun 2013-09-09
  PlugCheckBoxSpecialQuickFlashing.OnClick := CheckBoxClickEx; // 特殊物品快闪 piaoyun 2013-09-10
  PlugCheckBoxBlacklistHit.OnClick := CheckBoxClickEx; // 黑名单近身提示 piaoyun 2013-09-11
  PlugCheckBoxFriendHit.OnClick := CheckBoxClickEx; // 好友近身提示 piaoyun 2013-09-11
  PlugCheckBoxSceneShake.OnClick := CheckBoxClickEx; // 屏幕震动 piaoyun 2013-09-14
  PlugCheckBoxAutoDownHorse.OnClick := CheckBoxClickEx; // 魔法攻击自动下马 chongchong 2013-10-19

  PlugCheckDisableChartMemoSize.OnClick := CheckBoxClickEx;
  PlugCheckBoxItemCmp.OnClick := CheckBoxClickEx;
  PlugCheckBoxItemCmp.OnMouseMove := DControlMouseMoveShowHint;

  PlugCheckBoxBagFastItemCmp.OnClick := CheckBoxClickEx;
  PlugCheckBoxBagFastItemCmp.OnMouseMove := DControlMouseMoveShowHint;

  PlugCheckBoxHideItemEffect.OnClick := CheckBoxClickEx;
  PlugCheckBoxVolume.OnClick := CheckBoxClickEx;
  PlugCheckBoxDisableDeal.OnClick := CheckBoxClickEx;
  PlugCheckBoxHideBigHPProgress.OnClick := CheckBoxClickEx;

  PlugCheckBoxShowTargetAperture.OnClick := CheckBoxClickEx;

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
  PlugEditCheckDura.OnChange := DEditCheckDuraChange;
  PlugEditCheckDuraValue.OnChange := DEditCheckDuraValueChange;
  PlugEditCheckDuraTime.OnUnFocused := DEditCheckDuraTimeChange;

  //PlugComboBoxCheckHPValue.Items.Text := g_NGProtectItems.Text;
  //PlugComboBoxCheckMPValue.Items.Text := g_NGProtectItems.Text;

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

  if g_ClientVersion = cvMirNewUI205 then begin
    PlugCheckBoxUseSuperMedicaItemName0.ExpandWidth := 5;
    PlugCheckBoxUseSuperMedicaItemName1.ExpandWidth := 5;
    PlugCheckBoxUseSuperMedicaItemName2.ExpandWidth := 5;
    PlugCheckBoxUseSuperMedicaItemName3.ExpandWidth := 5;
    PlugCheckBoxUseSuperMedicaItemName4.ExpandWidth := 5;
    PlugCheckBoxUseSuperMedicaItemName5.ExpandWidth := 5;
    PlugCheckBoxUseSuperMedicaItemName6.ExpandWidth := 5;
    PlugCheckBoxUseSuperMedicaItemName7.ExpandWidth := 5;
    PlugCheckBoxUseSuperMedicaItemName8.ExpandWidth := 5;
  end;

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
  PlugMemoConfig6LabelKeyBoard13.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard14.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard15.OnKeyDown := DLabelKeyBoardKeyDown;
  PlugMemoConfig6LabelKeyBoard16.OnKeyDown := DLabelKeyBoardKeyDown;

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
  PlugMemoConfig6LabelKeyBoard13.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard14.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard15.OnMouseDown := DLabelKeyBoardMouseDown;
  PlugMemoConfig6LabelKeyBoard16.OnMouseDown := DLabelKeyBoardMouseDown;

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
  PlugMemoConfig6LabelKeyBoard13.Tag := 12;
  PlugMemoConfig6LabelKeyBoard14.Tag := 13;
  PlugMemoConfig6LabelKeyBoard15.Tag := 14;
  PlugMemoConfig6LabelKeyBoard16.Tag := 15;

  if FClientVersion < cvHero then
    PlugMemoConfig4Button1.Checked := True;

  PlugMemoConfig4Button2.Visible := FClientVersion >= cvHero;
  PlugMemoConfig4Button3.Visible := FClientVersion >= cvHero;
  PlugMemoConfig4Button4.Visible := FClientVersion >= cvHero;
  PlugMemoConfig4Button5.Visible := FClientVersion >= cvHero;

  PlugCheckBoxHeroAutoShield.Visible := FClientVersion >= cvHero;
  PlugCheckBoxAssistantHeroAutoShield.Visible := FClientVersion >= cvHero;
  PlugCheckBoxHeroContinuousNoHitMon.Visible := FClientVersion >= cvHero;
  PlugCheckBoxHeroShowNumberState.Visible := FClientVersion >= cvHero;

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
  PlugBtnDiyImport.OnClick := DBtnItemsImportOrExportClick;
  PlugBtnDiyExport.OnClick := DBtnItemsImportOrExportClick;
  PlugBtnDiyEdit.OnClick := PlugBtnDiyEditClick;

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
  PlugCheckSimpleShowActor.OnClick := CheckBoxClickEx;
  PlugCheckSimpleShowHumanDress.OnClick := CheckBoxClickEx;
  PlugCheckSimpleShowHumanWeapon.OnClick := CheckBoxClickEx;
  PlugCheckSimpleShowBB.OnClick := CheckBoxClickEx;

  PlugEditNotRushMonRange.OnChange := DEditNotRushMonRangeChange;
  PlugComboBoxPlayAttackValue.OnSelect := ComboBoxPlayAttackValueSelect;
  PlugComboBoxNoRedPoisonValue.OnSelect := ComboBoxPlayAttackValueSelect;
  PlugComboBoxNoBluePoisonValue.OnSelect := ComboBoxPlayAttackValueSelect;
  PlugComboBoxNoDuFuValue.OnSelect := ComboBoxPlayAttackValueSelect;
  PlugComboBoxBagFullValue.OnSelect := ComboBoxPlayAttackValueSelect;

  PlugMemoConfig82.OnListItemClick := ListViewGJMagicItemClick;
  PlugMemoConfig83.OnListItemClick := ListViewGJMagicItemClick;

  PlugCheckBoxGroupAttack.OnClick := CheckBoxClickEx;
  PlugEditNotGroupAttackCount.OnChange := DEditGroupAttackCountChanged;
  PlugButtonGJRun.OnClick := DBtnGJRunClick;

  PlugBtnGJPoint.OnClick := PlugBtnGJPointClick;

  PlugMemoConfig10ButtonEdit.OnClick := OnPlugMemoConfig10ButtonEditClick;
  PlugMemoConfig10ButtonSave.OnClick := OnPlugMemoConfig10ButtonEditClick;
  PlugMemoConfig10ButtonCancel.OnClick := OnPlugMemoConfig10ButtonEditClick;

  PlugMemoConfig10ButtonEdit.Visible := True;
  PlugMemoConfig10ButtonSave.Visible := False;
  PlugMemoConfig10ButtonCancel.Visible := False;
  PlugPageControlConfig.OnActivePageChange := OnPlugPageControlConfigActivePageChange;

  PlugMemoConfig2.PopupMenu := PopupMenuItems;
  PlugMemoConfig2.CanSelect := True;
  PopupMenuItems.OnClick := OnPopupMenuItemsClick;

  PopupMenuItems.Items.Enabled[0] := PlugMemoConfig2Label25.Visible;
  PopupMenuItems.Items.Enabled[1] := PlugMemoConfig2Label25.Visible;

  PopupMenuItems.Items.Enabled[3] := PlugMemoConfig2Label26.Visible;
  PopupMenuItems.Items.Enabled[4] := PlugMemoConfig2Label26.Visible;

  PopupMenuItems.Items.Enabled[6] := PlugMemoConfig2Label27.Visible;
  PopupMenuItems.Items.Enabled[7] := PlugMemoConfig2Label27.Visible;

  PopupMenuItems.Items.Enabled[9] := PlugMemoConfig2Label28.Visible;
  PopupMenuItems.Items.Enabled[10] := PlugMemoConfig2Label28.Visible;

  PopupMenuItems.Items.Enabled[12] := PlugMemoConfig2Label29.Visible;
  PopupMenuItems.Items.Enabled[13] := PlugMemoConfig2Label29.Visible;

  PlugScrollBoxUnbindItems.DrawSelect := True;
  PlugScrollBoxUnbindItems.DrawBorder := True;
  PlugScrollBoxUnbindItems.ShowScroll := True;
  PlugScrollBoxUnbindItems.ScrollBars := ssBoth;
  PlugScrollBoxUnbindItems.OnClick := PlugScrollBoxUnbindItemsClick;
  PlugBtnUnbindItemAdd.OnClick := PlugBtnUnbindItemClick;
  PlugBtnUnbindItemDel.OnClick := PlugBtnUnbindItemClick;
  PlugBtnUnbindItemEdit.OnClick := PlugBtnUnbindItemClick;
  PlugBtnUnbindItemSave.OnClick := PlugBtnUnbindItemClick;
  PlugBtnUnbindItemSave.Enabled := False;

  FInitializeed := True;

  DoInitAllComponentsMouseMove(PlugConfigDlg);
end;

procedure TMirConfigDlg.Finalize;
begin
  SaveConfigFile;
  // FInitializeed := False;
end;

procedure TMirConfigDlg.Logon(const ServerName:string);
var
  sDirectory, sFileName:string;
begin
  g_sPlugServerName := ServerName;

  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);
  sFileName := sDirectory + g_sPlugServerName + DecodeResStr(SBindItemFileName);
  LoadNGCustomUnbindItemList(sFileName);
  RefBindItemList;
end;

procedure TMirConfigDlg.LoadConfig(const CharName:string);
begin
  g_sPlugUserName := ProcessFileNameSpecialChar(CharName);

  if FLoadControl and (not FLoadConfig) then begin
    FLoadConfig := True;
    g_FileItemDB.LoadFormFile;
    LoadConfigFile;
    RefConfig;
    frmMain.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);
    RefShowItem;
    FEnabled := True;
  end;
end;

procedure TMirConfigDlg.Run;
begin
  AutoUseItem(Self);
end;

procedure TMirConfigDlg.RefActorList;
begin

end;

function TMirConfigDlg.GetShowItem(const ItemName:string):pTShowItem;
begin
  Result := g_FileItemDB.Find(ItemName);
end;

function TMirConfigDlg.FindShowItem(const ItemName:string):Boolean;
var
  ShowItem:pTShowItem;
begin
  ShowItem := g_FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boShowName
  else
    Result := False;
end;

function TMirConfigDlg.FindHintItem(const ItemName:string):Boolean;
var
  ShowItem:pTShowItem;
begin
  ShowItem := g_FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boHintMsg
  else
    Result := False;
end;

function TMirConfigDlg.FindPickItem(const ItemName:string):Boolean;
var
  ShowItem:pTShowItem;
begin
  ShowItem := g_FileItemDB.Find(ItemName);
  if ShowItem <> nil then
    Result := ShowItem.boPickup
  else
    Result := False;
end;

procedure TMirConfigDlg.HintItem(const ItemName:string; X, Y:Integer);
begin
  g_FileItemDB.Hint(ItemName, X, Y);
end;

procedure TMirConfigDlg.Struck(Actor:TObject; HP, MaxHP:LongInt);
var
  nDamage:Integer;
begin
  // 外挂吃药  物品--自动使用药品 (当HP减一定的值时，使用指定的药品) chongchong 2013-11-13
  if FEnabled then begin
    if (g_MySelf <> nil) then begin
      if (Actor = g_MySelf) then begin
        nDamage := Integer(g_MySelf.m_Abil.HP) - HP;
        if nDamage > 0 then
          DamageHPUseItem(0, nDamage);
      end;

      if (g_MyHero <> nil) then begin
        if (Actor = g_MyHero) then begin
          nDamage := HP;
          if nDamage > 0 then begin
            DamageHPUseItem(1, nDamage);
          end;
        end;
      end;
    end;
  end;
end;

procedure TMirConfigDlg.HealthChange(Actor:TObject; HP, MP, MaxHP:LongInt);
var
  nDamage:Integer;
begin
  if FEnabled then begin
    if (g_MySelf <> nil) then begin
      if (Actor = g_MySelf) then begin
        nDamage := Integer(g_MySelf.m_Abil.HP) - HP;
        if nDamage > 0 then
          DamageHPUseItem(0, nDamage);

        nDamage := Integer(g_MySelf.m_Abil.MP) - MP;
        if nDamage > 0 then
          DamageMPUseItem(0, nDamage);
      end;

      if (g_MyHero <> nil) then begin
        if (Actor = g_MyHero) then begin
          nDamage := Integer(g_MyHero.m_Abil.HP) - HP;
          if nDamage > 0 then
            DamageHPUseItem(1, nDamage);

          nDamage := Integer(g_MyHero.m_Abil.MP) - MP;
          if nDamage > 0 then
            DamageMPUseItem(1, nDamage);
        end;
      end;
    end;
  end;
end;

procedure TMirConfigDlg.AutoUseItem(Sender:TObject);
begin
  if FEnabled then begin
    // 吃药问题 chongchong 2018-01-23
    //if FProtectEnabled and (MyGetTickCount - FProtectEnabledTick > 2000) then
    begin
      AutoEatSpecialHPItem(Sender); //HZQ把吃特殊药放置到前面，优先检测先吃特殊药
      AutoEatSpecialMPItem(Sender);
      AutoEatHPItem(Sender);
      AutoEatMPItem(Sender);
      //AutoEatSpecialHPItem(Sender);
      //AutoEatSpecialMPItem(Sender);
      AutoProtect(Sender);
    end;
    AutoUseMagic(Sender);
    DuraWarning();
  end;
end;

function FindHumCustomBindItemIndex(BindItemType:TUnBindItemType; boSpecialMP:Boolean):Integer;
var
  I, II:Integer;
  UnBindItem:pTCustomBindItem;
  boCheckOK:Boolean;
begin
  Result := -1;
  if HumBagNoUseItemCount > 6 then begin
    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      UnBindItem := g_CustomUnbindItemList.Items[I];
      if (BindItemType = t_Special) then
        boCheckOK := (UnBindItem.UnBindItemType = BindItemType) and (UnBindItem.boSpecialMP = boSpecialMP)
      else
        boCheckOK := (UnBindItem.UnBindItemType = BindItemType);

      if (UnBindItem.sItemName <> '') and boCheckOK then begin
        for II := Low(g_ItemArr) to GetMaxBagCount - 1 do begin
          if (CompareText(UnBindItem.sItemName, g_ItemArr[II].s.Name) = 0) then begin
            Result := II;
            Exit;
          end;
        end;
      end;
    end;

    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      UnBindItem := g_CustomUnbindItemList.Items[I];
      if (BindItemType = t_Special) then
        boCheckOK := (UnBindItem.UnBindItemType = BindItemType) and (UnBindItem.boSpecialMP = boSpecialMP)
      else
        boCheckOK := (UnBindItem.UnBindItemType = BindItemType);

      if (UnBindItem.sBindItemName <> '') and boCheckOK then begin
        for II := Low(g_ItemArr) to GetMaxBagCount - 1 do begin
          if (CompareText(UnBindItem.sBindItemName, g_ItemArr[II].s.Name) = 0) then begin
            Result := II;
            Exit;
          end;
        end;
      end;
    end;
  end;
end;

function FindHeroCustomBindItemIndex(BindItemType:TUnBindItemType; boSpecialMP:Boolean):Integer;
var
  I, II:Integer;
  boCheckOK:Boolean;
  UnBindItem:pTCustomBindItem;
begin
  Result := -1;
  if g_MyHero = nil then Exit;

  if g_MyHero.m_nBagCount - HeroBagItemCount >= 6 then begin
    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      UnBindItem := g_CustomUnbindItemList.Items[I];

      if (BindItemType = t_Special) then
        boCheckOK := (UnBindItem.UnBindItemType = BindItemType) and (UnBindItem.boSpecialMP = boSpecialMP)
      else
        boCheckOK := (UnBindItem.UnBindItemType = BindItemType);

      if (UnBindItem.sItemName <> '') and boCheckOK then begin
        for II := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
          if (CompareText(UnBindItem.sItemName, g_HeroItemArr[II].s.Name) = 0) then begin
            Result := II;
            Exit;
          end;
        end;
      end;
    end;

    for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
      UnBindItem := g_CustomUnbindItemList.Items[I];

      if (BindItemType = t_Special) then
        boCheckOK := (UnBindItem.UnBindItemType = BindItemType) and (UnBindItem.boSpecialMP = boSpecialMP)
      else
        boCheckOK := (UnBindItem.UnBindItemType = BindItemType);

      if (UnBindItem.sBindItemName <> '') and boCheckOK then begin
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

procedure TMirConfigDlg.AutoEatHPItem(Sender:TObject);
var
  nIndex:Integer;
  SelfAbil:TAbility;
  HeroAbil:TAbility;
  Death:Boolean;

  function EatHumHPItem(flag:Boolean):Boolean;
  var
    Value:LongWord;
  begin
    Result := False;
    if not g_Config.ChkRenewAutoPercents[0] then
      Value := Min(g_Config.RenewHPPercents[0], SelfAbil.MaxHP)
    else
      Value := Round(SelfAbil.MaxHP / 100 * Min(g_Config.RenewHPPercents[0], 99));

    if (MyGetTickCount - g_Config.RenewHPTicks[0] > Cardinal(g_Config.RenewHPTimes[0])) and (flag or (SelfAbil.HP < Value)) then begin
      nIndex := FindHumHPItemIndex;
      if nIndex >= 0 then begin
        g_Config.RenewHPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHumCustomBindItemIndex(t_HP, False);
        if nIndex >= 0 then begin
          g_Config.RenewHPTicks[0] := MyGetTickCount;
          frmMain.AutoEatItem(nIndex);
          Result := True;
        end;
      end;

      if not Result then begin
        if MyGetTickCount - g_Config.RenewHPChatStringTicks[0] >= 5000 then begin
          DScreen.AddChatBoardString(DecodeResStr(SHPItemNoMsg), clWhite, clBlue);
          g_Config.RenewHPChatStringTicks[0] := MyGetTickCount;
        end;

        g_Config.RenewHPTicks[0] := MyGetTickCount;
      end;
    end;
  end;

  function EatHeroHPItem(flag:Boolean):Boolean;
  var
    Value:LongWord;
  begin
    Result := False;
    if not g_Config.ChkRenewAutoPercents[1] then
      Value := Min(g_Config.RenewHPPercents[1], HeroAbil.MaxHP)
    else
      Value := Round(HeroAbil.MaxHP / 100 * Min(g_Config.RenewHPPercents[1], 99));
    if (MyGetTickCount - g_Config.RenewHPTicks[1] > Cardinal(g_Config.RenewHPTimes[1])) and (flag or (HeroAbil.HP < Value)) then begin
      nIndex := FindHeroHPItemIndex;
      if nIndex >= 0 then begin
        g_Config.RenewHPTicks[1] := MyGetTickCount;
        frmMain.AutoHeroEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHeroCustomBindItemIndex(t_HP, False);
        if nIndex >= 0 then begin
          g_Config.RenewHPTicks[0] := MyGetTickCount;
          frmMain.AutoHeroEatItem(nIndex);
          Result := True;
        end;
      end;

      if not Result then begin
        if MyGetTickCount - g_Config.RenewHPChatStringTicks[1] >= 5000 then begin
          DScreen.AddChatBoardString(DecodeResStr(SHeroHPItemNoMsg), clWhite, clBlue);
          g_Config.RenewHPChatStringTicks[1] := MyGetTickCount;
        end;

        g_Config.RenewHPTicks[1] := MyGetTickCount;
      end;
    end;
  end;
begin
  // 外挂吃药  物品--普通体力药 (当HP小于指定值时，自动吃药) chongchong 2013-11-13
  if (g_MySelf <> nil) then begin
    Death := g_MySelf.m_boDeath;
    SelfAbil := g_MySelf.m_Abil;
    if g_Config.RenewHPIsAutos[0] and (not Death) and (SelfAbil.HP > 0) then
      EatHumHPItem(False);

    if (g_MyHero <> nil) then begin
      Death := g_MyHero.m_boDeath;
      HeroAbil := g_MyHero.m_Abil;
      if g_Config.RenewHPIsAutos[1] and (not Death) and (HeroAbil.HP > 0) and (HeroAbil.MaxHP > 0) then
        EatHeroHPItem(False);
    end;
  end;
end;

procedure TMirConfigDlg.AutoEatMPItem(Sender:TObject);
var
  nIndex:Integer;
  SelfAbil:TAbility;
  HeroAbil:TAbility;
  Death:Boolean;

  function EatHumMPItem(flag:Boolean):Boolean;
  var
    Value:LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[0] then
      Value := Min(g_Config.RenewMPPercents[0], SelfAbil.MaxMP)
    else
      Value := Round(SelfAbil.MaxMP / 100 * Min(g_Config.RenewMPPercents[0], 99));

    if (MyGetTickCount - g_Config.RenewMPTicks[0] > Cardinal(g_Config.RenewMPTimes[0])) and (flag or (SelfAbil.MP < Value)) then begin
      // DScreen.AddChatBoardString(Format('2 %d/%d %d/%d', [MyGetTickCount - g_Config.RenewMPTicks[0], g_Config.RenewMPTimes[0], SelfAbil.MP, g_Config.RenewMPPercents[0]]), clGreen, clWhite);
      nIndex := FindHumMPItemIndex;
      if nIndex >= 0 then begin
        // DScreen.AddChatBoardString(Format('3 %d/%d %d/%d', [MyGetTickCount - g_Config.RenewMPTicks[0], g_Config.RenewMPTimes[0], SelfAbil.MP, g_Config.RenewMPPercents[0]]), clGreen, clWhite);
        g_Config.RenewMPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHumCustomBindItemIndex(t_MP, False);
        if nIndex >= 0 then begin
          g_Config.RenewMPTicks[0] := MyGetTickCount;
          frmMain.AutoEatItem(nIndex);
          Result := True;
        end;
      end;

      if not Result then begin
        if MyGetTickCount - g_Config.RenewMPChatStringTicks[0] >= 5000 then begin
          DScreen.AddChatBoardString(DecodeResStr(SMPItemNoMsg), clWhite, clBlue);
          g_Config.RenewMPChatStringTicks[0] := MyGetTickCount;
        end;

        g_Config.RenewMPTicks[0] := MyGetTickCount;
      end;
    end;
  end;

  function EatHeroMPItem(flag:Boolean):Boolean;
  var
    Value:LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[1] then
      Value := Min(g_Config.RenewMPPercents[1], HeroAbil.MaxMP)
    else
      Value := Round(HeroAbil.MaxMP / 100 * Min(g_Config.RenewMPPercents[1], 99));

    if (MyGetTickCount - g_Config.RenewMPTicks[1] > Cardinal(g_Config.RenewMPTimes[1])) and (flag or (HeroAbil.MP < Value)) then begin
      nIndex := FindHeroMPItemIndex;
      if nIndex >= 0 then begin
        g_Config.RenewMPTicks[1] := MyGetTickCount;
        frmMain.AutoHeroEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHeroCustomBindItemIndex(t_MP, False);
        if nIndex >= 0 then begin
          g_Config.RenewMPTicks[0] := MyGetTickCount;
          frmMain.AutoHeroEatItem(nIndex);
          Result := True;
        end;
      end;

      if not Result then begin
        if MyGetTickCount - g_Config.RenewMPChatStringTicks[1] >= 5000 then begin
          DScreen.AddChatBoardString(DecodeResStr(SHeroMPItemNoMsg), clWhite, clBlue);
          g_Config.RenewMPChatStringTicks[1] := MyGetTickCount;
        end;

        g_Config.RenewMPTicks[1] := MyGetTickCount;
      end;
    end;
  end;
begin
  // 外挂吃药  物品--普通魔法药 (当MP小于指定值时，自动吃药) chongchong 2013-11-13
  if (g_MySelf <> nil) then begin
    SelfAbil := g_MySelf.m_Abil;
    Death := g_MySelf.m_boDeath;
    if g_Config.RenewMPIsAutos[0] and (not Death) and (SelfAbil.MaxMP > 0) then EatHumMPItem(False);

    if (g_MyHero <> nil) then begin
      HeroAbil := g_MyHero.m_Abil;
      Death := g_MyHero.m_boDeath;

      if g_Config.RenewMPIsAutos[1] and (not Death) and (HeroAbil.MaxMP > 0) then EatHeroMPItem(False);
    end;
  end;
end;

procedure TMirConfigDlg.AutoEatSpecialHPItem(Sender:TObject);
var
  nIndex:Integer;
  SelfAbil:TAbility;
  HeroAbil:TAbility;
  Death:Boolean;

  function EatHumSpecialItem(flag:Boolean):Boolean;
  var
    Value:LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[0] then
      Value := Min(g_Config.RenewSpecialHPPercents[0], SelfAbil.MaxHP)
    else
      Value := Round(SelfAbil.MaxHP / 100 * Min(g_Config.RenewSpecialHPPercents[0], 99));

    if (MyGetTickCount - g_Config.RenewSpecialHPTicks[0] > Cardinal(g_Config.RenewSpecialHPTimes[0])) and (flag or (SelfAbil.HP < Value)) then begin
      nIndex := FindHumSpecialItemIndex;
      if nIndex >= 0 then begin
        g_Config.RenewSpecialHPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHumCustomBindItemIndex(t_Special, False);
        if nIndex >= 0 then begin
          g_Config.RenewSpecialHPTicks[0] := MyGetTickCount;
          frmMain.AutoEatItem(nIndex);
          Result := True;
        end;
      end;

      if not Result then begin
        if MyGetTickCount - g_Config.RenewSpecialHPChatStringTicks[0] >= 5000 then begin
          DScreen.AddChatBoardString(DecodeResStr(SSpecialHPItemNoMsg), clWhite, clBlue);
          g_Config.RenewSpecialHPChatStringTicks[0] := MyGetTickCount;
        end;

        g_Config.RenewSpecialHPTicks[0] := MyGetTickCount;
      end;
    end;
  end;

  function EatHeroSpecialItem(flag:Boolean):Boolean;
  var
    Value:LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[1] then
      Value := Min(g_Config.RenewSpecialHPPercents[1], HeroAbil.MaxHP)
    else
      Value := Round(HeroAbil.MaxHP / 100 * Min(g_Config.RenewSpecialHPPercents[1], 99));

    if (MyGetTickCount - g_Config.RenewSpecialHPTicks[1] > Cardinal(g_Config.RenewSpecialHPTimes[1])) and (flag or (HeroAbil.HP < Value)) then begin
      nIndex := FindHeroSpecialItemIndex;
      if nIndex >= 0 then begin
        g_Config.RenewSpecialHPTicks[1] := MyGetTickCount;
        frmMain.AutoHeroEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHeroCustomBindItemIndex(t_Special, False);
        if nIndex >= 0 then begin
          g_Config.RenewSpecialHPTicks[1] := MyGetTickCount;
          frmMain.AutoHeroEatItem(nIndex);
          Result := True;
        end;
      end;

      if not Result then begin
        if MyGetTickCount - g_Config.RenewSpecialHPChatStringTicks[1] >= 5000 then begin
          DScreen.AddChatBoardString(DecodeResStr(SHeroSpecialHPItemNoMsg), clWhite, clBlue);
          g_Config.RenewSpecialHPChatStringTicks[1] := MyGetTickCount;
        end;

        g_Config.RenewSpecialHPTicks[1] := MyGetTickCount;
      end;
    end;
  end;
begin
  // 外挂吃药  物品--特殊体力药 (当HP小于指定值时，自动吃药) chongchong 2013-11-13
  if (g_MySelf <> nil) then begin
    SelfAbil := g_MySelf.m_Abil;
    Death := g_MySelf.m_boDeath;
    if g_Config.RenewSpecialHPIsAutos[0] and (not Death) and (SelfAbil.HP > 0) then EatHumSpecialItem(False);

    if (g_MyHero <> nil) then begin
      HeroAbil := g_MyHero.m_Abil;
      Death := g_MyHero.m_boDeath;
      if g_Config.RenewSpecialHPIsAutos[1] and (not Death) and (HeroAbil.HP > 0) and (HeroAbil.MaxHP > 0) then EatHeroSpecialItem(False);
    end;
  end;
end;

procedure TMirConfigDlg.AutoEatSpecialMPItem(Sender:TObject);
var
  nIndex:Integer;
  SelfAbil:TAbility;
  HeroAbil:TAbility;
  Death:Boolean;

  function EatHumSpecialItem(flag:Boolean):Boolean;
  var
    Value:LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[0] then
      Value := Min(g_Config.RenewSpecialMPPercents[0], SelfAbil.MaxMP)
    else
      Value := Round(SelfAbil.MaxMP / 100 * Min(g_Config.RenewSpecialMPPercents[0], 99));

    if (MyGetTickCount - g_Config.RenewSpecialMPTicks[0] > Cardinal(g_Config.RenewSpecialMPTimes[0])) and (flag or (SelfAbil.MP < Value)) then begin
      nIndex := FindHumSpecialItemIndex;
      if nIndex >= 0 then begin
        g_Config.RenewSpecialMPTicks[0] := MyGetTickCount;
        frmMain.AutoEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHumCustomBindItemIndex(t_Special, True);
        if nIndex >= 0 then begin
          g_Config.RenewSpecialMPTicks[0] := MyGetTickCount;
          frmMain.AutoEatItem(nIndex);
          Result := True;
        end;
      end;

      if not Result then begin
        if MyGetTickCount - g_Config.RenewSpecialMPChatStringTicks[0] >= 5000 then begin
          DScreen.AddChatBoardString(DecodeResStr(SSpecialMPItemNoMsg), clWhite, clBlue);
          g_Config.RenewSpecialMPChatStringTicks[0] := MyGetTickCount;
        end;

        g_Config.RenewSpecialMPTicks[0] := MyGetTickCount;
      end;
    end;
  end;

  function EatHeroSpecialItem(flag:Boolean):Boolean;
  var
    Value:LongWord;
  begin
    Result := False;

    if not g_Config.ChkRenewAutoPercents[1] then
      Value := Min(g_Config.RenewSpecialMPPercents[1], HeroAbil.MaxMP)
    else
      Value := Round(HeroAbil.MaxMP / 100 * Min(g_Config.RenewSpecialMPPercents[1], 99));

    if (MyGetTickCount - g_Config.RenewSpecialMPTicks[1] > Cardinal(g_Config.RenewSpecialMPTimes[1])) and (flag or (HeroAbil.MP < Value)) then begin
      nIndex := FindHeroSpecialItemIndex;
      if nIndex >= 0 then begin
        g_Config.RenewSpecialMPTicks[1] := MyGetTickCount;
        frmMain.AutoHeroEatItem(nIndex);
        Result := True;
      end
      else begin
        nIndex := FindHeroCustomBindItemIndex(t_Special, True);
        if nIndex >= 0 then begin
          g_Config.RenewSpecialMPTicks[1] := MyGetTickCount;
          frmMain.AutoHeroEatItem(nIndex);
          Result := True;
        end;
      end;

      if not Result then begin
        if MyGetTickCount - g_Config.RenewSpecialMPChatStringTicks[1] >= 5000 then begin
          DScreen.AddChatBoardString(DecodeResStr(SHeroSpecialMPItemNoMsg), clWhite, clBlue);
          g_Config.RenewSpecialMPChatStringTicks[1] := MyGetTickCount;
        end;

        g_Config.RenewSpecialMPTicks[1] := MyGetTickCount;
      end;
    end;
  end;
begin
  // 外挂吃药  物品--特殊魔法药 (当MP小于指定值时，自动吃药) chongchong 2013-11-13
  if (g_MySelf <> nil) then begin
    SelfAbil := g_MySelf.m_Abil;
    Death := g_MySelf.m_boDeath;
    if g_Config.RenewSpecialMPIsAutos[0] and

    (not Death) and (SelfAbil.MP > 0) then EatHumSpecialItem(False);
    if (g_MyHero <> nil) then begin
      HeroAbil := g_MyHero.m_Abil;
      Death := g_MyHero.m_boDeath;
      if g_Config.RenewSpecialMPIsAutos[1] and

      (not Death) and (HeroAbil.MP > 0) and (HeroAbil.MaxMP > 0) then EatHeroSpecialItem(False);
    end;
  end;
end;

procedure TMirConfigDlg.AutoProtect;
var
  I, J:Integer;
  nIndex:Integer;
  SelfAbil:TAbility;
  HeroAbil:TAbility;
  SelfDeath:Boolean;
  HeroDeath:Boolean;

  Value:LongWord;
begin
  if g_NGProtectItems.Count > 0 then begin
    if (g_MySelf <> nil) then begin
      SelfAbil := g_MySelf.m_Abil;
      SelfDeath := g_MySelf.m_boDeath;

      if (not g_ClientConfig.boCloseLogoutProtect) or (not g_ClientConfig.boCloseBookProtect) then begin
        // HP低于某值时执行操作
        for I := 0 to Length(g_Config.CheckHpIsAutos) - 1 do begin
          if I = 0 then begin
            if not g_Config.ChkAutoPercents[I] then
              Value := Min(g_Config.CheckHpPercents[I], SelfAbil.MaxHP)
            else
              Value := Round(SelfAbil.MaxHP / 100 * Min(g_Config.CheckHpPercents[I], 99));

            if g_Config.CheckHpIsAutos[I] and (not SelfDeath) and
              (g_Config.CheckHpValues[I] >= 0) and (g_Config.CheckHpValues[I] < PlugComboBoxCheckHPValue.Items.Count) and
              (SelfAbil.HP < Value) then begin
              if SameText(PlugComboBoxCheckHPValue.Items[g_Config.CheckHpValues[I]], '小退') then begin

                if MyGetTickCount - g_Config.CheckHpCheckTicks[I] > g_Config.CheckHpCheckTimes[I] then begin
                  g_Config.CheckHpCheckTicks[I] := MyGetTickCount;
                  if MyGetTickCount - g_Config.CheckHpUseTicks[I] > g_Config.CheckHpUseTimes[I] then begin
                    g_Config.CheckHpUseTicks[I] := MyGetTickCount;
                    if (not g_ClientConfig.boCloseLogoutProtect) then begin
                      g_IsWaitLogout := True;
                      frmMain.Logout;
                    end;
                    Exit;
                  end;
                end;
              end  else begin
                if MyGetTickCount - g_Config.CheckHpCheckTicks[I] > g_Config.CheckHpCheckTimes[I] then begin
                  g_Config.CheckHpCheckTicks[I] := MyGetTickCount;
                  if MyGetTickCount - g_Config.CheckHpUseTicks[I] > g_Config.CheckHpUseTimes[I] then begin
                    nIndex := FindHumBookItemIndex(PlugComboBoxCheckHPValue.Items[g_Config.CheckHpValues[I]]);
                    if nIndex >= 0 then begin
                      g_Config.CheckHpUseTicks[I] := MyGetTickCount;
                      if (not g_ClientConfig.boCloseBookProtect) then
                        frmMain.AutoEatItem(nIndex);
                      Break;
                    end;
                  end;
                end;
              end;
            end;
          end else begin
            if (g_MyHero <> nil) then begin
              HeroAbil := g_MyHero.m_Abil;
              HeroDeath := g_MyHero.m_boDeath;

              if not g_Config.ChkAutoPercents[I] then
                Value := Min(g_Config.CheckHpPercents[I], HeroAbil.MaxHP)
              else
                Value := Round(HeroAbil.MaxHP / 100 * Min(g_Config.CheckHpPercents[I], 99));

              if g_Config.CheckHpIsAutos[I] and (not HeroDeath) and // 收回英雄
              (HeroAbil.HP < Value) and (MyGetTickCount - g_dwRenewHeroLogOutTick > 2000) then begin
                if MyGetTickCount - g_Config.CheckHpCheckTicks[I] > 1000 {g_Config.CheckHpCheckTimes[I]} then begin
                  g_Config.CheckHpCheckTicks[I] := MyGetTickCount;
                  if MyGetTickCount - g_Config.CheckHpUseTicks[I] > 1000 {g_Config.CheckHpUseTimes[I]} then begin
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
        for I := 0 to Length(g_Config.CheckMPIsAutos) - 1 do begin
          if I = 0 then begin
            if not g_Config.ChkAutoPercents[I] then
              Value := Min(g_Config.CheckMPPercents[I], SelfAbil.MaxMP)
            else
              Value := Round(SelfAbil.MaxMP / 100 * Min(g_Config.CheckMPPercents[I], 99)); //HZQ 20230725 Self.Abil.MP --> SelfAbil.MaxMP

            if g_Config.CheckMPIsAutos[I] and (not SelfDeath) and
              (g_Config.CheckMPValues[I] >= 0) and (g_Config.CheckMPValues[I] < PlugComboBoxCheckMPValue.Items.Count) and
              (SelfAbil.MP < Value) then begin
              if SameText(PlugComboBoxCheckMPValue.Items[g_Config.CheckMPValues[I]], '小退') then begin
                if (MyGetTickCount - g_Config.CheckMPCheckTicks[I] > g_Config.CheckMPCheckTimes[I]) and (MyGetTickCount - g_dwRenewSelfLogOutTick > 30000) then begin
                  g_Config.CheckMPCheckTicks[I] := MyGetTickCount;
                  if MyGetTickCount - g_Config.CheckMPUseTicks[I] > g_Config.CheckMPUseTimes[I] then begin
                    g_Config.CheckMPUseTicks[I] := MyGetTickCount;
                    if (not g_ClientConfig.boCloseLogoutProtect) then begin
                      g_IsWaitLogout := True;
                      frmMain.Logout;
                    end;
                    Exit;
                  end;
                end;
              end
              else begin
                if MyGetTickCount - g_Config.CheckMPCheckTicks[I] > g_Config.CheckMPCheckTimes[I] then begin
                  g_Config.CheckMPCheckTicks[I] := MyGetTickCount;
                  if MyGetTickCount - g_Config.CheckMPUseTicks[I] > g_Config.CheckMPUseTimes[I] then begin
                    nIndex := FindHumBookItemIndex(PlugComboBoxCheckMPValue.Items[g_Config.CheckMPValues[I]]);
                    if nIndex >= 0 then begin
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
          else begin
            if (g_MyHero <> nil) then begin
              HeroAbil := g_MyHero.m_Abil;
              HeroDeath := g_MyHero.m_boDeath;

              if not g_Config.ChkAutoPercents[I] then
                Value := Min(g_Config.CheckMPPercents[I], HeroAbil.MaxMP)
              else
                Value := Round(HeroAbil.MaxMP / 100 * Min(g_Config.CheckMPPercents[I], 99));

              if g_Config.CheckMPIsAutos[I] and (not HeroDeath) and // 收回英雄
              (HeroAbil.MP < Value) and (MyGetTickCount - g_dwRenewHeroLogOutTick > 2000) then begin
                if MyGetTickCount - g_Config.CheckMPCheckTicks[I] > 1000 {g_Config.CheckMPCheckTimes[I]} then begin
                  g_Config.CheckMPCheckTicks[I] := MyGetTickCount;
                  if MyGetTickCount - g_Config.CheckMPUseTicks[I] > 1000 {g_Config.CheckMPUseTimes[I]} then begin
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

      // 持久保护
      for I := 0 to Length(g_Config.CheckDuraIsAutos) - 1 do begin
        // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
        //Value := g_Config.CheckDuraMin[I] * 1000;

        if g_Config.CheckDuraIsAutos[I] and (g_Config.CheckDuraTime[I] > 0) and (Length(g_Config.CheckDuraValue[I]) > 0)
          and (MyGetTickCount - g_Config.CheckDuraCheckTicks[I] > Cardinal(g_Config.CheckDuraTime[I]) * 1000) then begin
          g_Config.CheckDuraCheckTicks[I] := MyGetTickCount;

          if I = 0 then begin
            if (not SelfDeath) then begin
              nIndex := FindHumUnBindBookItemIndex(g_Config.CheckDuraValue[I]);

              if nIndex >= 0 then begin
                J := Low(g_UseItems);
                while J <= High(g_UseItems) do begin
                  if (Length(g_UseItems[J].S.Name) > 0) and
                    (not (g_UseItems[J].s.StdMode in [7, 2, 25, 96, 97]))
                    and (not ((g_UseItems[J].s.StdMode = 53) and (g_UseItems[J].s.AniCount in [1, 2, 3]))) then begin
                    // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
                    Value := Round(g_UseItems[J].DuraMax / 100 * g_config.CheckDuraMin[I]);

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
                    Value := Round(g_JewelryBoxItems[J].DuraMax / 100 * g_config.CheckDuraMin[I]);

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
                    (not ((g_JewelryBoxItems[J].s.StdMode = 53) and (g_JewelryBoxItems[J].s.AniCount in [1, 2, 3]))) then begin
                    // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
                    Value := Round(g_GodBlessItems[J].DuraMax / 100 * g_config.CheckDuraMin[I]);

                    if (g_GodBlessItems[J].Dura < g_GodBlessItems[J].DuraMax) and (g_GodBlessItems[J].Dura < Value) then begin
                      frmMain.AutoEatItem(nIndex);
                      Exit;
                    end;
                  end;
                  Inc(J);
                end;
              end;
            end;
          end
          else if (g_MyHero <> nil) then begin
            HeroDeath := g_MyHero.m_boDeath;
            if (not HeroDeath) then begin
              nIndex := FindHeroBagItemName(g_Config.CheckDuraValue[I]);

              if nIndex >= 0 then begin
                J := Low(g_HeroUseItems);
                while J <= High(g_HeroUseItems) do begin
                  if (Length(g_HeroUseItems[J].S.Name) > 0) and
                    (not (g_HeroUseItems[J].s.StdMode in [7, 2, 25, 96, 97])) and
                    (not ((g_HeroUseItems[J].s.StdMode = 53) and (g_HeroUseItems[J].s.AniCount in [1, 2, 3]))) then begin
                    // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
                    Value := Round(g_HeroUseItems[J].DuraMax / 100 * g_config.CheckDuraMin[I]);

                    if (g_HeroUseItems[J].Dura < g_HeroUseItems[J].DuraMax) and (g_HeroUseItems[J].Dura < Value) then begin
                      frmMain.AutoHeroEatItem(nIndex);
                      Exit;
                    end;
                  end;
                  Inc(J);
                end;

                J := Low(g_HeroJewelryBoxItems);
                while J <= High(g_HeroJewelryBoxItems) do begin
                  if (Length(g_HeroJewelryBoxItems[J].S.Name) > 0) and
                    (not (g_HeroJewelryBoxItems[J].s.StdMode in [7, 2, 25, 96, 97])) and
                    (not ((g_HeroJewelryBoxItems[J].s.StdMode = 53) and (g_HeroJewelryBoxItems[J].s.AniCount in [1, 2, 3]))) then begin
                    // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
                    Value := Round(g_HeroJewelryBoxItems[J].DuraMax / 100 * g_config.CheckDuraMin[I]);

                    if (g_HeroJewelryBoxItems[J].Dura < g_HeroJewelryBoxItems[J].DuraMax) and (g_HeroJewelryBoxItems[J].Dura < Value) then begin
                      frmMain.AutoHeroEatItem(nIndex);
                      Exit;
                    end;
                  end;
                  Inc(J);
                end;

                J := Low(g_HeroGodBlessItems);
                while J <= High(g_HeroGodBlessItems) do begin
                  if (Length(g_HeroGodBlessItems[J].S.Name) > 0) and
                    (not (g_HeroGodBlessItems[J].s.StdMode in [7, 2, 25, 96, 97])) and
                    (not ((g_HeroGodBlessItems[J].s.StdMode = 53) and (g_HeroGodBlessItems[J].s.AniCount in [1, 2, 3]))) then begin
                    // 持久按%百分比来算 chongchong 2018-06-18 01:24:48
                    Value := Round(g_HeroGodBlessItems[J].DuraMax / 100 * g_config.CheckDuraMin[I]);

                    if (g_HeroGodBlessItems[J].Dura < g_HeroGodBlessItems[J].DuraMax) and (g_HeroGodBlessItems[J].Dura < Value) then begin
                      frmMain.AutoHeroEatItem(nIndex);
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

procedure TMirConfigDlg.DuraWarning();
var
  I:Integer;
  sHint:string;
  DuraValue:Integer;
begin
  if FConfigCheckeds[ckDuraWarning] then begin
    if MyGetTickCount - FHintItemDuraTick > 1000 * 60 then begin
      FHintItemDuraTick := MyGetTickCount;
      if (g_MySelf <> nil) then begin
        I := Low(g_UseItems);
        while I <= High(g_UseItems) do begin
          if (g_UseItems[I].S.Name <> '') and (g_UseItems[I].S.StdMode <> 96) and (g_UseItems[I].S.StdMode <> 97) and
            (not ((g_UseItems[I].S.StdMode in [7, 53]) and (g_UseItems[I].S.Shape in [1, 2, 3]))) and // 魔血石、气血石、幻魔石
          (not ((g_UseItems[I].S.StdMode = 25) and (g_UseItems[I].S.Shape = 9))) and // 火龙之心
          (not ((g_UseItems[I].S.StdMode = 7) and (g_UseItems[I].S.Shape = 0) and (g_UseItems[I].S.AniCount > 0))) then {// 千里传音/传音筒} begin
            DuraValue := Round(g_UseItems[I].Dura / 1000);

            // 传送符
            if (g_UseItems[I].s.StdMode = 25) and (g_UseItems[I].s.Shape = 6) then
              DuraValue := Round(g_UseItems[I].Dura / 100);

            // 修复神水
            if (g_UseItems[I].s.StdMode = 2) and (g_UseItems[I].s.Shape = 9) then
              DuraValue := Round(g_UseItems[I].Dura / 100);

            if g_UseItems[I].Dura <= Round(g_UseItems[I].DuraMax * 10 / 100) then begin
              sHint := Format(DecodeResStr(SItemDuraWarning), [ProcessItemName(g_UseItems[I].S.Name), DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;

        I := Low(g_JewelryBoxItems);
        while I <= High(g_JewelryBoxItems) do begin
          if Length(g_JewelryBoxItems[I].S.Name) > 0 then begin
            DuraValue := Round(g_JewelryBoxItems[I].Dura / 1000);
            if g_JewelryBoxItems[I].Dura <= Round(g_JewelryBoxItems[I].DuraMax * 10 / 100) then begin
              sHint := Format(DecodeResStr(SItemDuraWarning), [ProcessItemName(g_JewelryBoxItems[I].S.Name), DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;

        I := Low(g_GodBlessItems);
        while I <= High(g_GodBlessItems) do begin
          if Length(g_GodBlessItems[I].S.Name) > 0 then begin
            DuraValue := Round(g_GodBlessItems[I].Dura / 1000);
            if g_GodBlessItems[I].Dura <= Round(g_GodBlessItems[I].DuraMax * 10 / 100) then begin
              sHint := Format(DecodeResStr(SItemDuraWarning), [ProcessItemName(g_GodBlessItems[I].S.Name), DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;
      end;

      if g_MyHero <> nil then begin
        I := Low(g_HeroUseItems);
        while I <= High(g_HeroUseItems) do begin
          if (g_HeroUseItems[I].S.Name <> '') and (g_HeroUseItems[I].S.StdMode <> 96) and (g_HeroUseItems[I].S.StdMode <> 97) and
            (not ((g_HeroUseItems[I].S.StdMode in [7, 53]) and (g_HeroUseItems[I].S.Shape in [1, 2, 3]))) and // 魔血石、气血石、幻魔石
          (not ((g_HeroUseItems[I].S.StdMode = 25) and (g_HeroUseItems[I].S.Shape = 9))) and // 火龙之心
          (not ((g_HeroUseItems[I].S.StdMode = 7) and (g_HeroUseItems[I].S.Shape = 0) and (g_HeroUseItems[I].S.AniCount > 0))) then {// 千里传音/传音筒} begin
            DuraValue := Round(g_HeroUseItems[I].Dura / 1000);

            // 传送符
            if (g_HeroUseItems[I].s.StdMode = 25) and (g_HeroUseItems[I].s.Shape = 6) then
              DuraValue := Round(g_HeroUseItems[I].Dura / 100);

            // 修复神水
            if (g_HeroUseItems[I].s.StdMode = 2) and (g_HeroUseItems[I].s.Shape = 9) then
              DuraValue := Round(g_HeroUseItems[I].Dura / 100);

            if g_HeroUseItems[I].Dura <= Round(g_HeroUseItems[I].DuraMax * 10 / 100) then begin
              sHint := Format(DecodeResStr(SHeroItemDuraWarning), [ProcessItemName(g_HeroUseItems[I].S.Name), DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;

        I := Low(g_HeroJewelryBoxItems);
        while I <= High(g_HeroJewelryBoxItems) do begin
          if Length(g_HeroJewelryBoxItems[I].S.Name) > 0 then begin
            DuraValue := Round(g_HeroJewelryBoxItems[I].Dura / 1000);
            if g_HeroJewelryBoxItems[I].Dura <= Round(g_HeroJewelryBoxItems[I].DuraMax * 10 / 100) then begin
              sHint := Format(DecodeResStr(SHeroItemDuraWarning), [ProcessItemName(g_HeroJewelryBoxItems[I].S.Name), DuraValue]);
              DScreen.AddChatBoardString(sHint, clWhite, clBlue);
            end;
          end;
          Inc(I);
        end;

        I := Low(g_HeroGodBlessItems);
        while I <= High(g_HeroGodBlessItems) do begin
          if Length(g_HeroGodBlessItems[I].S.Name) > 0 then begin
            DuraValue := Round(g_HeroGodBlessItems[I].Dura / 1000);
            if g_HeroGodBlessItems[I].Dura <= Round(g_HeroGodBlessItems[I].DuraMax * 10 / 100) then begin
              sHint := Format(DecodeResStr(SHeroItemDuraWarning), [ProcessItemName(g_HeroGodBlessItems[I].S.Name), DuraValue]);
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

function NumberSort_1(List:TStringList; Index1, Index2:Integer):Integer;
var
  Value1, Value2:Integer;
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

procedure TMirConfigDlg.DamageHPUseItem(nObj, nDamage:Integer);
var
  I, nIndex:Integer;
  sItemName:string;
  StringList:TStringList;
  MyHero:TObject;
  SelfDeath, HeroDeath:Boolean;

  Value:LongWord;
begin
  if FEnabled then begin
    if (nObj in [0..4]) and g_Config.UseSuperMedicas[nObj] then begin
      if (g_MySelf <> nil) then begin
        SelfDeath := g_MySelf.m_boDeath;
        MyHero := nil;
        HeroDeath := True;
        if (nObj > 0) and (g_MyHero <> nil) then begin
          MyHero := g_MyHero;
          HeroDeath := g_MyHero.m_boDeath;
        end;

        if ((nObj = 0) and (not SelfDeath)) or ((nObj > 0) and (MyHero <> nil) and (not HeroDeath)) then begin
          StringList := TStringList.Create;

          if nObj = 0 then begin
            for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do begin
              if not g_Config.ChkSuperMedicaPercents[nObj] then
                Value := Min(g_Config.SuperMedicaHps[nObj][I], g_MySelf.m_Abil.MaxHP)
              else
                Value := Round(g_MySelf.m_Abil.MaxHP / 100 * Min(g_Config.SuperMedicaHps[nObj][I], 99));

              StringList.AddObject(IntToStr(Value), TObject(I));
            end;
          end else begin
            for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do begin
              if not g_Config.ChkSuperMedicaPercents[nObj] then
                Value := Min(g_Config.SuperMedicaHps[nObj][I], g_MyHero.m_Abil.MaxHP)
              else
                Value := Round(g_MyHero.m_Abil.MaxHP / 100 * Min(g_Config.SuperMedicaHps[nObj][I], 99));

              StringList.AddObject(IntToStr(Value), TObject(I));
            end;
          end;
          StringList.CustomSort(NumberSort_1);
          for I := 0 to StringList.Count - 1 do begin
            nIndex := Integer(StringList.Objects[I]);
            Value := StrToIntDef(StringList.Strings[I], 0);

            if g_Config.SuperMedicaUses[nObj][nIndex] and (Value > 0) and (nDamage >= Integer(Value)) and
              (MyGetTickCount - Cardinal(g_Config.SuperMedicaHpTicks[nObj][nIndex]) > Cardinal(g_Config.SuperMedicaHpTimes[nObj][nIndex])) then begin
              sItemName := g_Config.SuperMedicaItemNames[nIndex];
              if sItemName <> '' then begin
                if nObj = 0 then begin
                  nIndex := FindBagItemName(sItemName);
                  if nIndex >= 0 then begin
                    frmMain.AutoEatItem(nIndex);
                    g_Config.SuperMedicaHpTicks[nObj][nIndex] := MyGetTickCount;
                    StringList.Free;
                    Exit;
                  end
                  else begin
                    nIndex := FindHumBindItemIndex(sItemName);
                    if nIndex >= 0 then begin
                      frmMain.AutoEatItem(nIndex);
                      g_Config.SuperMedicaHpTicks[nObj][nIndex] := MyGetTickCount;
                      StringList.Free;
                      Exit;
                    end
                    else begin
                      DScreen.AddChatBoardString(Format(DecodeResStr(SItemUseNoMsg), [sItemName]), clWhite, clBlue);
                    end;
                  end;
                end
                else begin
                  nIndex := FindHeroBagItemName(sItemName);
                  if nIndex >= 0 then begin
                    frmMain.AutoHeroEatItem(nIndex);
                    g_Config.SuperMedicaHpTicks[nObj][nIndex] := MyGetTickCount;
                    StringList.Free;
                    Exit;
                  end
                  else begin
                    nIndex := FindHeroBindItemIndex(sItemName);
                    if nIndex >= 0 then begin
                      frmMain.AutoHeroEatItem(nIndex);
                      g_Config.SuperMedicaHpTicks[nObj][nIndex] := MyGetTickCount;
                      StringList.Free;
                      Exit;
                    end
                    else begin
                      DScreen.AddChatBoardString(Format(DecodeResStr(SHeroItemUseNoMsg), [sItemName]), clWhite, clBlue);
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

procedure TMirConfigDlg.DamageMPUseItem(nObj, nDamage:Integer);
var
  I, nIndex:Integer;
  sItemName:string;
  StringList:TStringList;
  Value:LongWord;
begin
  if FEnabled then begin
    if (nObj in [0..4]) and g_Config.UseSuperMedicas[nObj] then begin
      if (g_MySelf <> nil) then begin

        if ((nObj = 0) and (not g_MySelf.m_boDeath)) or ((nObj > 0) and (g_MyHero <> nil) and (not g_MyHero.m_boDeath)) then begin
          StringList := TStringList.Create;

          if nObj = 0 then begin
            for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do begin
              if not g_Config.ChkSuperMedicaPercents[nObj] then
                Value := Min(g_Config.SuperMedicaMPs[nObj][I], g_MySelf.m_Abil.MaxMP)
              else
                Value := Round(g_MySelf.m_Abil.MaxMP / 100 * Min(g_Config.SuperMedicaMPs[nObj][I], 99));
              StringList.AddObject(IntToStr(Value), TObject(I));
            end;
          end
          else begin
            for I := 0 to Length(g_Config.SuperMedicaUses) - 1 do begin
              if not g_Config.ChkSuperMedicaPercents[nObj] then
                Value := Min(g_Config.SuperMedicaMPs[nObj][I], g_MyHero.m_Abil.MaxMP)
              else
                Value := Round(g_MyHero.m_Abil.MaxMP / 100 * Min(g_Config.SuperMedicaMPs[nObj][I], 99));
              StringList.AddObject(IntToStr(Value), TObject(I));
            end;
          end;

          StringList.CustomSort(NumberSort_1);
          for I := 0 to StringList.Count - 1 do begin
            nIndex := Integer(StringList.Objects[I]);
            Value := StrToIntDef(StringList.Strings[I], 0);

            if g_Config.SuperMedicaUses[nObj][nIndex] and (Value > 0) and (nDamage >= Integer(Value))
              and (MyGetTickCount - Cardinal(g_Config.SuperMedicaMPTicks[nObj][nIndex]) > Cardinal(g_Config.SuperMedicaMPTimes[nObj][nIndex])) then begin
              sItemName := g_Config.SuperMedicaItemNames[nIndex];
              if sItemName <> '' then begin
                if nObj = 0 then begin
                  nIndex := FindBagItemName(sItemName);
                  if nIndex >= 0 then begin
                    frmMain.AutoEatItem(nIndex);
                    g_Config.SuperMedicaMPTicks[nObj][nIndex] := MyGetTickCount;
                    StringList.Free;
                    Exit;
                  end
                  else begin
                    nIndex := FindHumBindItemIndex(sItemName);
                    if nIndex >= 0 then begin
                      frmMain.AutoEatItem(nIndex);
                      g_Config.SuperMedicaMPTicks[nObj][nIndex] := MyGetTickCount;
                      StringList.Free;
                      Exit;
                    end
                    else begin
                      DScreen.AddChatBoardString(Format(DecodeResStr(SItemUseNoMsg), [sItemName]), clWhite, clBlue);
                    end;
                  end;
                  ;
                end
                else begin
                  nIndex := FindHeroBagItemName(sItemName);
                  if nIndex >= 0 then begin
                    frmMain.AutoHeroEatItem(nIndex);
                    g_Config.SuperMedicaMPTicks[nObj][nIndex] := MyGetTickCount;
                    StringList.Free;
                    Exit;
                  end
                  else begin
                    nIndex := FindHeroBindItemIndex(sItemName);
                    if nIndex >= 0 then begin
                      frmMain.AutoHeroEatItem(nIndex);
                      g_Config.SuperMedicaMPTicks[nObj][nIndex] := MyGetTickCount;
                      StringList.Free;
                      Exit;
                    end
                    else begin
                      DScreen.AddChatBoardString(Format(DecodeResStr(SHeroItemUseNoMsg), [sItemName]), clWhite, clBlue);
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

procedure TMirConfigDlg.AutoUseMagic(Sender:TObject);
var
  ClientMagic:pTClientMagic;
begin
  if FConfigCheckeds[ckAutoUseMagic] then begin
    if (g_MySelf <> nil) and
      (not g_MySelf.m_boDeath) and
      (not g_MySelf.m_boShopStall) then begin
      if (PlugComboBoxAutoMagic.ItemIndex >= 0) and (PlugComboBoxAutoMagic.ItemIndex < PlugComboBoxAutoMagic.Items.Count) then begin
        if MyGetTickCount - g_Config.dwAutoUseMagicTick > Cardinal(g_Config.nAutoUseMagicTime) * 1000 then begin
          g_Config.dwAutoUseMagicTick := MyGetTickCount;

          ClientMagic := pTClientMagic(PlugComboBoxAutoMagic.Items.Objects[PlugComboBoxAutoMagic.ItemIndex]);
          frmMain.AutoTakeOnItem(ClientMagic);
          frmMain.UseMagic(g_nMouseX, g_nMouseY, ClientMagic);
        end;
      end;
    end;
  end;
end;

function TMirConfigDlg.CanFilterExp(Exp:LongWord):Boolean;
begin
  Result := False;
  if FConfigCheckeds[ckFilterExp] then begin
    Result := Exp < Cardinal(g_Config.nFilterMinExp);
  end;
end;

procedure TMirConfigDlg.LoadConfigFile;
var
  I, II:Integer;
  nShift:Integer;
  ini:TIniFile;
  sDirectory, sFileName, sIdent, sIdent1, sIdent2, sIdent3, sIdent4, sIdent5, sIdent6:string;
  sIdent7, sIdent8, sIdent9, sIdent10, sIdent11, sIdent12, sIdent13, sIdent14, sIdent15:string;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  sFileName := g_sSelfFilePath + Format(CONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);

  ini := TIniFile.Create(sFileName);
  if ini <> nil then begin
    // ==============================================================================================================

    g_SoundVolume := Ini.ReadInteger('Setup', 'Volume', g_SoundVolume);

    for I := 0 to Length(FConfigCheckeds) - 1 do begin
      FConfigCheckeds[TConfigChecked(I)] := ini.ReadBool('Setup', Format('Checked%d', [I]), FConfigCheckeds[TConfigChecked(I)]);
    end;

    g_Config.nFilterMinExp := ini.ReadInteger('Setup', 'FilterMinExp', g_Config.nFilterMinExp);

    g_Config.nColorShowEff := ini.ReadInteger('Setup', 'ColorShowEff', g_Config.nColorShowEff);
    frmMain.nColorShowEff := g_Config.nColorShowEff;

    g_Config.nSpecialColor := ini.ReadInteger('Setup', 'SpecialColor', g_Config.nSpecialColor);
    frmMain.nSpecialColor := g_Config.nSpecialColor;

    g_Config.MedicaMode := ini.ReadInteger('Protect', 'MedicaMode', g_Config.MedicaMode);
    g_Config.MedicaMode := Max(g_Config.MedicaMode, 0);
    g_Config.MedicaMode := Min(g_Config.MedicaMode, 4);

    for I := 0 to 4 do begin
      case I of
        0:begin
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
        1:begin
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
        2:begin
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
        3:begin
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
        4:begin
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

      if Length(Trim(g_Config.CheckDuraValue[I])) = 0 then begin
        g_Config.CheckDuraValue[I] := '修复神水';
      end;

      g_Config.CheckDuraTime[I] := Max(Ini.ReadInteger('Protect', sIdent15, g_Config.CheckDuraTime[I]), 2);

      g_Config.UseSuperMedicas[I] := ini.ReadBool('Protect', sIdent, g_Config.UseSuperMedicas[I]);
    end;

    for I := 0 to 8 do begin
      for II := 0 to 4 do begin
        case II of
          0:begin
              sIdent1 := '%sBoUse';
              sIdent2 := '%sHp';
              sIdent3 := '%sHpTime';
              sIdent4 := '%sMp';
              sIdent5 := '%sMpTime';
            end;
          1:begin
              sIdent1 := '%shBoUse';
              sIdent2 := '%sHeroHp';
              sIdent3 := '%sHeroHpTime';
              sIdent4 := '%sHeroMp';
              sIdent5 := '%sHeroMpTime';
            end;
          2:begin
              sIdent1 := '%szBoUse';
              sIdent2 := '%szHeroHp';
              sIdent3 := '%szHeroHpTime';
              sIdent4 := '%szHeroMp';
              sIdent5 := '%szHeroMpTime';
            end;
          3:begin
              sIdent1 := '%sfBoUse';
              sIdent2 := '%sfHeroHp';
              sIdent3 := '%sfHeroHpTime';
              sIdent4 := '%sfHeroMp';
              sIdent5 := '%sfHeroMpTime';
            end;
          4:begin
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
    for I := 0 to Length(g_ShortcutKeys) - 1 do begin
      g_ShortcutKeys[I].Use := ini.ReadBool('Hotkey', 'Use' + IntToStr(I), g_ShortcutKeys[I].Use);
      g_ShortcutKeys[I].Key := ini.ReadInteger('Hotkey', 'Key' + IntToStr(I), g_ShortcutKeys[I].Key);
      nShift := ini.ReadInteger('Hotkey', 'Shift' + IntToStr(I), nShift);
      Move(nShift, g_ShortcutKeys[I].Shift, SizeOf(TShiftState));
    end;

    g_Config.nHeroDodgeHPPercent := ini.ReadInteger('Protect', 'HeroDodgeHPPercent', g_Config.nHeroDodgeHPPercent);

    g_Config.nGJPlayAttackOption := ini.ReadInteger('GJ', 'GJPlayAttackOption', g_Config.nGJPlayAttackOption); // 挂机 - 受玩家攻击后的操作
    g_Config.nGJNoRedPoisonOption := ini.ReadInteger('GJ', 'GJNoRedPoisonOption', g_Config.nGJNoRedPoisonOption); // 挂机 - 红药用完后动作 chongchong 2014-12-06
    g_Config.nGJNoBluePoisonOption := ini.ReadInteger('GJ', 'GJNoBluePoisonOption', g_Config.nGJNoBluePoisonOption); // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
    g_Config.nGJNoDuFuOption := ini.ReadInteger('GJ', 'GJNoDuFuOption', g_Config.nGJNoDuFuOption); // 挂机 - 毒符用完后动作 chongchong 2014-12-06
    g_Config.nGJBagFullOption := ini.ReadInteger('GJ', 'GJBagFullOption', g_Config.nGJBagFullOption); // 挂机 - 包裹满后动作 chongchong 2014-12-06

    g_Config.nGJNotRushMonRange := ini.ReadInteger('GJ', 'GJNotRushMonRange', g_Config.nGJNotRushMonRange); // 挂机 - 怪物周围几格有玩家
    g_Config.nGJGroupAttackCount := ini.ReadInteger('GJ', 'GJGroupAttackCount', g_Config.nGJGroupAttackCount); // 挂机 - 目标周围有几个怪视为群攻

    FrmMain.nGJPlayAttackOption := g_Config.nGJPlayAttackOption;
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

  LoadNotesFile;

  if not PlugCheckBoxHumManuallyCustomHit1.Visible then begin
    FConfigCheckeds[ckHumManuallyCustomHit1] := False;
  end;

  if not PlugCheckBoxHumManuallyCustomHit2.Visible then begin
    FConfigCheckeds[ckHumManuallyCustomHit2] := False;
  end;

  if not PlugCheckBoxHumManuallyCustomHit3.Visible then begin
    FConfigCheckeds[ckHumManuallyCustomHit3] := False;
  end;

  if not PlugCheckBoxHumManuallyCustomHit4.Visible then begin
    FConfigCheckeds[ckHumManuallyCustomHit4] := False;
  end;

  if not PlugCheckBoxHumManuallyCustomHit5.Visible then begin
    FConfigCheckeds[ckHumManuallyCustomHit5] := False;
  end;

  PlugComboBoxNoRedPoisonValue.Items.Text := g_NGProtectItems.Text;
  PlugComboBoxNoBluePoisonValue.Items.Text := g_NGProtectItems.Text;
  PlugComboBoxNoDuFuValue.Items.Text := g_NGProtectItems.Text;
  PlugComboBoxBagFullValue.Items.Text := g_NGProtectItems.Text;
  PlugComboBoxPlayAttackValue.Items.Text := g_NGProtectItems.Text;
end;

procedure TMirConfigDlg.SaveConfigFile;
var
  I, II:Integer;
  nShift:Integer;
  ini:TIniFile;
  sDirectory, sFileName, sIdent, sIdent1, sIdent2, sIdent3, sIdent4, sIdent5, sIdent6:string;
  sIdent7, sIdent8, sIdent9, sIdent10, sIdent11, sIdent12, sIdent13, sIdent14, sIdent15:string;
begin
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    g_sPlugUserName := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end;

  sFileName := g_sSelfFilePath + Format(CONFIGFILE, [g_sPlugServerName, g_sPlugUserName]);

  ini := TIniFile.Create(sFileName);
  if ini <> nil then begin
    try
      Ini.WriteInteger('Setup', 'Volume', g_SoundVolume);

      for I := 0 to Length(FConfigCheckeds) - 1 do begin
        // 这4个选项会把及时雨的冲掉 chongchong 2015-04-21
        if not (TConfigChecked(I) in [ckShowRadarPlayer, ckShowRadarActor, ckShowRadarNpc, ckShowRadarAttackNpc]) then
          ini.WriteBool('Setup', Format('Checked%d', [I]), FConfigCheckeds[TConfigChecked(I)]);
      end;
      ini.WriteInteger('Setup', 'FilterMinExp', g_Config.nFilterMinExp);

      ini.WriteInteger('Setup', 'ColorShowEff', g_Config.nColorShowEff);
      ini.WriteInteger('Setup', 'SpecialColor', g_Config.nSpecialColor);

      for I := 0 to 4 do begin
        case I of
          0:begin
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
          1:begin
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
          2:begin
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
          3:begin
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
          4:begin
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

      for I := 0 to 8 do begin
        for II := 0 to 4 do begin
          case II of
            0:begin
                sIdent1 := '%sBoUse';
                sIdent2 := '%sHp';
                sIdent3 := '%sHpTime';
                sIdent4 := '%sMp';
                sIdent5 := '%sMpTime';
              end;
            1:begin
                sIdent1 := '%shBoUse';
                sIdent2 := '%sHeroHp';
                sIdent3 := '%sHeroHpTime';
                sIdent4 := '%sHeroMp';
                sIdent5 := '%sHeroMpTime';
              end;
            2:begin
                sIdent1 := '%szBoUse';
                sIdent2 := '%szHeroHp';
                sIdent3 := '%szHeroHpTime';
                sIdent4 := '%szHeroMp';
                sIdent5 := '%szHeroMpTime';
              end;
            3:begin
                sIdent1 := '%sfBoUse';
                sIdent2 := '%sfHeroHp';
                sIdent3 := '%sfHeroHpTime';
                sIdent4 := '%sfHeroMp';
                sIdent5 := '%sfHeroMpTime';
              end;
            4:begin
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

      for I := 0 to Length(g_ShortcutKeys) - 1 do begin
        ini.WriteBool('Hotkey', 'Use' + IntToStr(I), g_ShortcutKeys[I].Use);
        ini.WriteInteger('Hotkey', 'Key' + IntToStr(I), g_ShortcutKeys[I].Key);
        Move(g_ShortcutKeys[I].Shift, nShift, SizeOf(TShiftState));
        ini.WriteInteger('Hotkey', 'Shift' + IntToStr(I), nShift);
      end;

      ini.WriteInteger('Protect', 'HeroDodgeHPPercent', g_Config.nHeroDodgeHPPercent);

      ini.WriteInteger('GJ', 'GJPlayAttackOption', g_Config.nGJPlayAttackOption); // 挂机 - 受玩家攻击后的操作
      ini.WriteInteger('GJ', 'GJNoRedPoisonOption', g_Config.nGJNoRedPoisonOption); // 挂机 - 红药用完后动作 chongchong 2014-12-06
      ini.WriteInteger('GJ', 'GJNoBluePoisonOption', g_Config.nGJNoBluePoisonOption); // 挂机 - 蓝药用完后动作 chongchong 2014-12-06
      ini.WriteInteger('GJ', 'GJNoDuFuOption', g_Config.nGJNoDuFuOption); // 挂机 - 毒符用完后动作 chongchong 2014-12-06
      ini.WriteInteger('GJ', 'GJBagFullOption', g_Config.nGJBagFullOption); // 挂机 - 包裹满后动作 chongchong 2014-12-06

      ini.WriteInteger('GJ', 'GJNotRushMonRange', g_Config.nGJNotRushMonRange); // 挂机 - 怪物周围几格有玩家
      ini.WriteInteger('GJ', 'GJGroupAttackCount', g_Config.nGJGroupAttackCount); // 挂机 - 目标周围有几个怪群攻
    except
      on E:Exception do begin
        //DebugOutStr('[Exception] TMirConfigDlg::SaveConfigFile');
        DebugOutStr(DecodeResStr(SMirConfigDlgSaveErr));
        DebugOutStr(E.Message);
      end;
    end;
    ini.Free;
  end;
end;

procedure TMirConfigDlg.DLabelKeyBoardKeyDown(Sender:TObject; var Key:Word;
  Shift:TShiftState);
var
  I:Integer;
  DxLabel:TDxLabel;

  Magic:PTClientMagic;
  TempKey:Char;
begin
  DxLabel := TDxLabel(Sender);
  if (DxLabel.Tag >= 0) and (DxLabel.Tag < Length(g_ShortcutKeys)) then begin
    for I := 0 to Length(g_ShortcutKeys) - 1 do begin
      if (g_ShortcutKeys[I].Key = Key) and (g_ShortcutKeys[I].Shift = Shift) then begin
        g_ShortcutKeys[I].Use := False;
        g_ShortcutKeys[I].Key := 0;
        g_ShortcutKeys[I].Shift := [];
      end;
    end;
    g_ShortcutKeys[DxLabel.Tag].Use := True;
    g_ShortcutKeys[DxLabel.Tag].Key := Key;
    g_ShortcutKeys[DxLabel.Tag].Shift := Shift;

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

procedure TMirConfigDlg.DLabelKeyBoardMouseDown(Sender:TObject; Button:TMouseButton;
  Shift:TShiftState; X, Y:Integer);
var
  DxLabel:TDxLabel;
begin
  if Button = mbRight then begin
    DxLabel := TDxLabel(Sender);
    if (DxLabel.Tag >= 0) and (DxLabel.Tag < Length(g_ShortcutKeys)) then begin
      g_ShortcutKeys[DxLabel.Tag].Use := False;
      g_ShortcutKeys[DxLabel.Tag].Key := 0;
      g_ShortcutKeys[DxLabel.Tag].Shift := [];
      RefKeyBoardConfig;
    end;
  end;
end;

procedure TMirConfigDlg.DMemoBossListClick(Sender:TObject; X,
  Y:Integer);
begin
  //if PlugScrollBoxBoss.ScrollMouseDown(Button, X, Y) then Exit;
  if (PlugScrollBoxBoss.ItemIndex >= 0) and (PlugScrollBoxBoss.ItemIndex <= PlugScrollBoxBoss.Lines.Count - 1) then begin
    PlugEditBoss.Text := PlugScrollBoxBoss.Lines[PlugScrollBoxBoss.ItemIndex];
    PlugBtnBossModify.Enabled := True;
    PlugBtnBossDel.Enabled := True;
  end
  else begin
    PlugEditBoss.Text := '';
    PlugBtnBossModify.Enabled := False;
    PlugBtnBossDel.Enabled := False;
  end;
end;

function TMirConfigDlg.CheckBossNameExists(Name:string; CurIndex:Integer):Boolean;
var
  I:Integer;
begin
  Result := False;
  for I := 0 to PlugScrollBoxBoss.Lines.Count - 1 do begin
    if SameText(Name, PlugScrollBoxBoss.Lines[I]) and (CurIndex <> I) then begin
      Result := True;
      Exit;
    end;
  end;
end;

procedure TMirConfigDlg.DBtnBossAddClick(Sender:TObject; X, Y:Integer);
var
  BossName:string;
begin
  BossName := Trim(PlugEditBoss.Text);
  if Length(BossName) = 0 then begin
    FrmDlg.DMessageDlg(DecodeResStr(SBossNameEmpty), [mbOk]);
    Exit;
  end;
  if not CheckBossNameExists(BossName) then begin
    PlugScrollBoxBoss.Lines.Add(BossName);
    begin
      SaveOrLoadBossList(True);
      //PlugScrollBoxBoss.ItemIndex := PlugScrollBoxBoss.Lines.Count - 1;
    end;
  end
  else
    FrmDlg.DMessageDlg(DecodeResStr(SAddBossNameExists), [mbOk]);
end;

procedure TMirConfigDlg.DBtnBossDelClick(Sender:TObject; X, Y:Integer);
begin
  if (PlugScrollBoxBoss.ItemIndex >= 0) and (PlugScrollBoxBoss.ItemIndex <= PlugScrollBoxBoss.Lines.Count - 1) then begin
    PlugScrollBoxBoss.Lines.Delete(PlugScrollBoxBoss.ItemIndex);
    PlugEditBoss.Text := '';
    SaveOrLoadBossList(True);
  end;
end;

procedure TMirConfigDlg.DBtnBossModifyClick(Sender:TObject; X, Y:Integer);
var
  BossName:string;
begin
  BossName := Trim(PlugEditBoss.Text);
  if Length(BossName) = 0 then begin
    FrmDlg.DMessageDlg(DecodeResStr(SBossNameEmpty), [mbOk]);
    Exit;
  end;
  if not CheckBossNameExists(BossName, PlugScrollBoxBoss.ItemIndex) then begin
    PlugScrollBoxBoss.Lines[PlugScrollBoxBoss.ItemIndex] := BossName;
    PlugScrollBoxBoss.Lines := PlugScrollBoxBoss.Lines;

    SaveOrLoadBossList(True);
  end
  else
    FrmDlg.DMessageDlg(DecodeResStr(SEditBossNameExists), [mbOk]);
end;

procedure TMirConfigDlg.SaveOrLoadBossList(IsSave:Boolean);
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
    PlugScrollBoxBoss.Lines.SaveToFile(sFileName);
    //PlugScrollBoxBoss.Position := 0;
  end else begin
    PlugScrollBoxBoss.Lines.Clear;
    if FileExists(sFileName) then
      PlugScrollBoxBoss.Lines.LoadFromFile(sFileName);

    if PlugScrollBoxBoss.Lines.Count = 0 then begin
      PlugScrollBoxBoss.Lines.Text := g_NGBossList.Text;
    end;

    PlugScrollBoxBoss.ItemIndex := -1;
    PlugBtnBossModify.Enabled := False;
    PlugBtnBossDel.Enabled := False;
  end;

  g_BossList.Text := PlugScrollBoxBoss.Lines.Text;
end;

procedure TMirConfigDlg.DEditSpecialColorChange(Sender:TObject);
begin
  //g_Config.nAutoUseMagicTime := PlugEditAutoMagicTime.Value;
  if PlugEditSpecialColor.Value > 255 then PlugEditSpecialColor.Value := 255;
  g_Config.nSpecialColor := PlugEditSpecialColor.Value;
  frmMain.nSpecialColor := g_Config.nSpecialColor;
  PlugLabelSpecialColor.CaptionColor.Up.Color := GetRGB(PlugEditSpecialColor.Value);
end;

procedure TMirConfigDlg.DBtnDiyAddClick(Sender:TObject; X, Y:Integer);
var
  ShowItem:pTShowItem;
  sItemName:string;
  FileItem:pTShowItem;
begin
  sItemName := PlugEditSpecialName.Text;
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
    //m_ShowItemList.Add(ShowItem);
    g_FileItemDB.Add(ShowItem);
    New(FileItem);
    FileItem^ := ShowItem^;
    g_FileItemDB.m_FileItemList.Add(FileItem);
    //g_FileItemDB.SaveToFile;
    PlugComboBoxItemStdMode.ItemIndex := 8;
    DComboBoxItemStdModeSelect(Sender);
    PlugMemoConfig2.Last;
  end;
end;

procedure TMirConfigDlg.DBtnDiyDelClick(Sender:TObject; X, Y:Integer);
var
  I, Index:Integer;
  ListItem:TDxListItem;
  ShowItem:pTShowItem;
begin
  for I := PlugMemoConfig2.Count - 1 downto 0 do begin
    ListItem := PlugMemoConfig2.Items[I];
    if not ListItem.Selected then Continue;

    if ListItem.Count > 0 then begin
      ShowItem := pTShowItem(ListItem.Items[0].Data);

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

        PlugMemoConfig2.Delete(I);
      end;
    end;
  end;

  g_FileItemDB.SaveToFile;
  g_DropItemsMgr.RefreshDrawList(True);
end;

procedure TMirConfigDlg.DBtnItemsImportOrExportClick(Sender:TObject; X, Y:Integer);
var
  I:Integer;
  List:TList;

  ListItem:TDxListItem;
  ViewItem:pTViewItem;
  ShowItem:pTShowItem;

  FileName:string;
  OpenDlg:TOpenDialog;
  SaveDlg:TSaveDialog;

  IsLoad:Boolean;
begin
  if (Sender = PlugBtnDiyImport) then begin
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

    if not IsLoad then Exit;

    List := TList.Create;
    g_FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List);
    PlugMemoConfig2.Clear;
    PlugMemoConfig2.ColCount := 6;

    PlugMemoConfig2.Lock;
    try
      for I := 0 to List.Count - 1 do begin
        ShowItem := List.Items[I];

        ListItem := PlugMemoConfig2.Add;
        ViewItem := ListItem.AddItem('', nil);

        ViewItem.Caption := ShowItem.sItemName;
        ViewItem.Data := ShowItem;
        ViewItem.Style := bsButton; // bsRadio;
        ViewItem.Alignment := taLeftJustify;
        ViewItem.Color.Up.Color := clWhite;
        ViewItem.Color.Hot.Color := clRed; // clWhite;
        ViewItem.Color.Down.Color := clRed;

        if PlugMemoConfig2Label25.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ViewItem.Checked := ShowItem.boHintMsg;
        end;

        if PlugMemoConfig2Label26.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ViewItem.Checked := ShowItem.boPickup;
        end;

        if PlugMemoConfig2Label27.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ViewItem.Checked := ShowItem.boShowName;
        end;

        if PlugMemoConfig2Label28.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ViewItem.Checked := ShowItem.boShowSpecial;
        end;

        if PlugMemoConfig2Label29.Visible then begin
          ViewItem := ListItem.AddItem('', nil);
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsCheckBox;
          if (g_ClientVersion <> cvMirNewUI205) then begin
            ViewItem.ImageIndex.ImageType := NewopUI_Pak;
            ViewItem.ImageIndex.Up := 228;
            ViewItem.ImageIndex.Down := 229;
          end
          else begin
            ViewItem.ImageIndex.ImageType := UICommon_wil;
            ViewItem.ImageIndex.Up := 28;
            ViewItem.ImageIndex.Down := 30;
          end;
          ViewItem.Checked := ShowItem.boAutoMove;
        end;
      end;
    finally
      PlugMemoConfig2.UnLock;
    end;

    g_IsClientPickItemsChanged := True;

    g_FileItemDB.SaveToFile;
    g_DropItemsMgr.RefreshDrawList(True);
    DScreen.AddChatBoardString(DecodeResStr(SImportItemOK), GetRGB(219), clWhite);

    List.Free;
  end
  else if (Sender = PlugBtnDiyExport) then begin
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
end;

procedure TMirConfigDlg.PatchAddNearEffectCheckBox;
begin
    if Self.PlugCheckBoxNearEffect = nil then begin
        if PlugScrollBoxBoss <> nil then begin
            PlugCheckBoxNearEffect := TDxImageButton.Create(PlugMemoConfigBoss);
            PlugCheckBoxNearEffect.Height := PlugCheckBoxAutoLock.Height;
            PlugScrollBoxBoss.Height := PlugScrollBoxBoss.Height - PlugCheckBoxNearEffect.Height - 2; 

            PlugCheckBoxNearEffect.Name := 'PlugCheckBoxNearEffect';
            PlugCheckBoxNearEffect.GuiType := PlugCheckBoxAutoLock.GuiType;
            PlugCheckBoxNearEffect.OnGetImage := PlugCheckBoxAutoLock.OnGetImage;
            PlugCheckBoxNearEffect.Designing := False;
            PlugCheckBoxNearEffect.AutoSize := False;

            PlugCheckBoxNearEffect.Caption := '目标特效';
            PlugCheckBoxNearEffect.Hint := '内挂战斗页面目标列表的特效';
            PlugCheckBoxNearEffect.Transparent := PlugCheckBoxAutoLock.Transparent;
            PlugCheckBoxNearEffect.Style := PlugCheckBoxAutoLock.Style;
            PlugCheckBoxNearEffect.Align := PlugCheckBoxAutoLock.Align;
            PlugCheckBoxNearEffect.Alignment := PlugCheckBoxAutoLock.Alignment;

            PlugCheckBoxNearEffect.CaptionDownOffsetX := PlugCheckBoxAutoLock.CaptionDownOffsetX;
            PlugCheckBoxNearEffect.CaptionDownOffsetY := PlugCheckBoxAutoLock.CaptionDownOffsetY;

            PlugCheckBoxNearEffect.ImageIndex.ImageType := PlugCheckBoxAutoLock.ImageIndex.ImageType;
            PlugCheckBoxNearEffect.ImageIndex.Up := PlugCheckBoxAutoLock.ImageIndex.Up;
            PlugCheckBoxNearEffect.ImageIndex.Hot := PlugCheckBoxAutoLock.ImageIndex.Hot;
            PlugCheckBoxNearEffect.ImageIndex.Down := PlugCheckBoxAutoLock.ImageIndex.Down;
            PlugCheckBoxNearEffect.ImageIndex.Disabled := PlugCheckBoxAutoLock.ImageIndex.Disabled;

            PlugCheckBoxNearEffect.CaptionColor.Up := PlugCheckBoxAutoLock.CaptionColor.Up;
            PlugCheckBoxNearEffect.CaptionColor.Hot := PlugCheckBoxAutoLock.CaptionColor.Hot;
            PlugCheckBoxNearEffect.CaptionColor.Down := PlugCheckBoxAutoLock.CaptionColor.Down;
            PlugCheckBoxNearEffect.CaptionColor.Disabled := PlugCheckBoxAutoLock.CaptionColor.Disabled;

            PlugCheckBoxNearEffect.ClickCount := PlugCheckBoxAutoLock.ClickCount;

            PlugCheckBoxNearEffect.Checked := False;

            PlugCheckBoxNearEffect.Width := PlugCheckBoxAutoLock.Width;
            PlugCheckBoxNearEffect.Left := PlugScrollBoxBoss.Left;
            PlugCheckBoxNearEffect.Top := PlugScrollBoxBoss.Top + PlugScrollBoxBoss.Height + 2;
        end;
    end;
end;

procedure TMirConfigDlg.PlugBtnDiyEditClick(Sender:TObject; X, Y:Integer);
var
  I:Integer;
  List:TList;

  ListItem:TDxListItem;
  ViewItem:pTViewItem;
  ShowItem:pTShowItem;
  SL:TStringList;
begin
  SL := TStringList.Create;
  try
    g_FileItemDB.ExportToStrings(SL);
    if ShowFrmNGItemEdit(SL) then begin
      g_FileItemDB.ImportFormStrings(SL);

      List := TList.Create;
      g_FileItemDB.Get(TItemType(PlugComboBoxItemStdMode.ItemIndex), List);
      PlugMemoConfig2.Clear;
      PlugMemoConfig2.ColCount := 6;

      PlugMemoConfig2.Lock;
      try
        for I := 0 to List.Count - 1 do begin
          ShowItem := List.Items[I];

          ListItem := PlugMemoConfig2.Add;
          ViewItem := ListItem.AddItem('', nil);

          ViewItem.Caption := ShowItem.sItemName;
          ViewItem.Data := ShowItem;
          ViewItem.Style := bsButton; // bsRadio;
          ViewItem.Alignment := taLeftJustify;
          ViewItem.Color.Up.Color := clWhite;
          ViewItem.Color.Hot.Color := clRed; // clWhite;
          ViewItem.Color.Down.Color := clRed;

          if PlugMemoConfig2Label25.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boHintMsg;
          end;

          if PlugMemoConfig2Label26.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boPickup;
          end;

          if PlugMemoConfig2Label27.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boShowName;
          end;

          if PlugMemoConfig2Label28.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boShowSpecial;
          end;

          if PlugMemoConfig2Label29.Visible then begin
            ViewItem := ListItem.AddItem('', nil);
            ViewItem.Data := ShowItem;
            ViewItem.Style := bsCheckBox;
            if (g_ClientVersion <> cvMirNewUI205) then begin
              ViewItem.ImageIndex.ImageType := NewopUI_Pak;
              ViewItem.ImageIndex.Up := 228;
              ViewItem.ImageIndex.Down := 229;
            end
            else begin
              ViewItem.ImageIndex.ImageType := UICommon_wil;
              ViewItem.ImageIndex.Up := 28;
              ViewItem.ImageIndex.Down := 30;
            end;
            ViewItem.Checked := ShowItem.boAutoMove;
          end;
        end;
      finally
        PlugMemoConfig2.UnLock;
      end;

      g_IsClientPickItemsChanged := True;
      g_FileItemDB.SaveToFile;
      List.Free;

      g_DropItemsMgr.RefreshDrawList(True);
    end;
  finally
    SL.Free;
  end;
end;

procedure TMirConfigDlg.DBtnGJPageControlClick(Sender:TObject; X, Y:Integer);
begin
  PlugMemoConfig8Page.ActivePageIndex := (Sender as TDxImageButton).Tag;
end;

procedure TMirConfigDlg.DMemoGJMonListClick(Sender:TObject; X,
  Y:Integer);
begin
  //if PlugScrollBoxBoss.ScrollMouseDown(Button, X, Y) then Exit;
  if (PlugScrollBoxMons.ItemIndex >= 0) and (PlugScrollBoxMons.ItemIndex <= PlugScrollBoxMons.Lines.Count - 1) then begin
    PlugEditMonName.Text := PlugScrollBoxMons.Lines[PlugScrollBoxMons.ItemIndex];
    PlugButtonMonNameEdit.Enabled := True;
    PlugButtonMonNameDel.Enabled := True;
  end
  else begin
    PlugEditMonName.Text := '';
    PlugButtonMonNameEdit.Enabled := False;
    PlugButtonMonNameDel.Enabled := False;
  end;
end;

function TMirConfigDlg.CheckGJMonNameExists(Name:string; CurIndex:Integer):Boolean;
var
  I:Integer;
begin
  Result := False;
  for I := 0 to PlugScrollBoxMons.Lines.Count - 1 do begin
    if SameText(Name, PlugScrollBoxMons.Lines[I]) and (CurIndex <> I) then begin
      Result := True;
      Exit;
    end;
  end;
end;

procedure TMirConfigDlg.DBtnGJMonNameAddClick(Sender:TObject; X, Y:Integer);
var
  MonName:string;
begin
  MonName := Trim(PlugEditMonName.Text);
  if Length(MonName) = 0 then begin
    FrmDlg.DMessageDlg(DecodeResStr(SMonNameEmpty), [mbOk]);
    Exit;
  end;
  if not CheckGJMonNameExists(MonName) then begin
    PlugScrollBoxMons.Lines.Add(MonName);
    begin
      SaveOrLoadGJMonList(True);
      //PlugScrollBoxMons.ItemIndex := PlugScrollBoxMons.Lines.Count - 1;
    end;
  end
  else
    FrmDlg.DMessageDlg(DecodeResStr(SAddMonNameExists), [mbOk]);
end;

procedure TMirConfigDlg.DBtnGJMonNameDelClick(Sender:TObject; X, Y:Integer);
begin
  if (PlugScrollBoxMons.ItemIndex >= 0) and (PlugScrollBoxMons.ItemIndex <= PlugScrollBoxMons.Lines.Count - 1) then begin
    PlugScrollBoxMons.Lines.Delete(PlugScrollBoxMons.ItemIndex);
    PlugEditMonName.Text := '';
    SaveOrLoadGJMonList(True);
  end;
end;

procedure TMirConfigDlg.DBtnGJMonNameEditClick(Sender:TObject; X,
  Y:Integer);
var
  MonName:string;
begin
  MonName := Trim(PlugEditMonName.Text);
  if Length(MonName) = 0 then begin
    FrmDlg.DMessageDlg(DecodeResStr(SMonNameEmpty), [mbOk]);
    Exit;
  end;
  if not CheckGJMonNameExists(MonName, PlugScrollBoxMons.ItemIndex) then begin
    PlugScrollBoxMons.Lines[PlugScrollBoxMons.ItemIndex] := MonName;
    PlugScrollBoxMons.Lines := PlugScrollBoxMons.Lines;
    SaveOrLoadGJMonList(True);
  end
  else
    FrmDlg.DMessageDlg(DecodeResStr(SEditMonNameExists), [mbOk]);
end;

procedure TMirConfigDlg.SaveOrLoadGJMonList(IsSave:Boolean);
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
    PlugScrollBoxMons.Lines.SaveToFile(sFileName);
    //PlugScrollBoxMons.Position := 0;
  end
  else begin
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

procedure TMirConfigDlg.SaveOrLoadGJMagicList1(IsSave:Boolean);
var
  I, Value:Integer;
  sDirectory, sFileName:string;
  SL:TStringList;

  ListItem:TDxListItem;
  ViewItem:pTViewItem;
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
      for I := 0 to PlugMemoConfig82.Count - 1 do begin
        ListItem := PlugMemoConfig82.Items[I];
        if ListItem.Count = 2 then begin
          ViewItem := ListItem.Items[1];
          if ViewItem.Checked then begin
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
  else begin
    PlugMemoConfig82.Clear;
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

procedure TMirConfigDlg.SaveOrLoadGJMagicList2(IsSave:Boolean);
var
  I, Value:Integer;
  sDirectory, sFileName:string;
  SL:TStringList;

  ListItem:TDxListItem;
  ViewItem:pTViewItem;
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
      for I := 0 to PlugMemoConfig83.Count - 1 do begin
        ListItem := PlugMemoConfig83.Items[I];
        if ListItem.Count = 2 then begin
          ViewItem := ListItem.Items[1];
          if ViewItem.Checked then begin
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
  else begin
    PlugMemoConfig83.Clear;
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

procedure TMirConfigDlg.DEditNotRushMonRangeChange(Sender:TObject);
begin
  g_Config.nGJNotRushMonRange := PlugEditNotRushMonRange.Value;
  FrmMain.nGJNotRushMonRange := g_Config.nGJNotRushMonRange;
end;

procedure TMirConfigDlg.ComboBoxPlayAttackValueSelect(Sender:TObject);
begin
  if Sender = PlugComboBoxPlayAttackValue then begin
    g_Config.nGJPlayAttackOption := PlugComboBoxPlayAttackValue.ItemIndex;
    FrmMain.nGJPlayAttackOption := g_Config.nGJPlayAttackOption;
  end
  else if Sender = PlugComboBoxNoRedPoisonValue then begin
    g_Config.nGJNoRedPoisonOption := PlugComboBoxNoRedPoisonValue.ItemIndex;
    FrmMain.nGJNoRedPoisonOption := g_Config.nGJNoRedPoisonOption;
  end
  else if Sender = PlugComboBoxNoBluePoisonValue then begin
    g_Config.nGJNoBluePoisonOption := PlugComboBoxNoBluePoisonValue.ItemIndex;
    FrmMain.nGJNoBluePoisonOption := g_Config.nGJNoBluePoisonOption;
  end
  else if Sender = PlugComboBoxNoDuFuValue then begin
    g_Config.nGJNoDuFuOption := PlugComboBoxNoDuFuValue.ItemIndex;
    FrmMain.nGJNoDuFuOption := g_Config.nGJNoDuFuOption;
  end
  else if Sender = PlugComboBoxBagFullValue then begin
    g_Config.nGJBagFullOption := PlugComboBoxBagFullValue.ItemIndex;
    FrmMain.nGJBagFullOption := g_Config.nGJBagFullOption;
  end
end;

procedure TMirConfigDlg.DControlMouseMoveShowHint(Sender:TObject;
  Shift:TShiftState; X, Y:Integer); stdcall;
var
  Ctrl:TDxControl;

  nHintX, nHintY:Integer;
  vtRect:TRect;

  sHint:string;
begin
  Ctrl := Sender as TDxControl;

  sHint := '';
  if Length(Ctrl.Hint) > 0 then
    sHint := Ctrl.Hint
  else if Sender = PlugCheckBoxAutoHideMode then
    sHint := DecodeResStr(SNGHintAutoHideMode)
  else if Sender = PlugCheckBoxGroupAttack then
    sHint := DecodeResStr(SNGHintGroupAttack)
  else if Sender = PlugCheckBoxDisableSelfStruck then
    sHint := DecodeResStr(SNGHintDisableSelfStruck)
  else if Sender = PlugCheckBoxContinueButchItem then
    sHint := DecodeResStr(SNGHintContinueButchItem)
  else if Sender = PlugCheckBoxExpFilter then
    sHint := DecodeResStr(SNGHintExpFilter)
  else if Sender = PlugCheckBoxDuraWarning then
    sHint := DecodeResStr(SNGHintDuraWarning)
  else if Sender = PlugCheckBoxSpecialQuickFlashing then
    sHint := DecodeResStr(SNGHintSpecialQuickFlash)
  else if Sender = PlugCheckBoxHideItemEffect then
    sHint := DecodeResStr(SNGHintHideItemEffect)
  else if Sender = PlugCheckBoxHumManuallySnowWind then
    sHint := DecodeResStr(SNGHintHumManuallySnowWind)
  else if Sender = PlugCheckBoxBagFastItemCmp then
    sHint := DecodeResStr(SNGHintBagFastItemCmp)
  else if Sender = PlugCheckBoxShowHPUnit then
    sHint := DecodeResStr(SNGHintShowHPUnit)
  else if Sender = PlugLabelDefaultItem then
    sHint := DecodeResStr(SNGHintDefaultItem)
  else if Sender = PlugCheckBoxHideDescUserName then
    sHint := DecodeResStr(SNGHintOnlyShowUserName)
  else if Sender = PlugCheckBoxAutoOpenSpell then
    sHint := DecodeResStr(SNGHintAutoOpenSpell)
  else if Sender = PlugCheckBoxItemCmp then
    sHint := '背包中鼠标指向物品与身上物品属性对比'
  else if Sender = PlugCheckBoxPickAll then
    sHint := '勾选后无论物品是否设置"自动拾取"全部捡取'
  else if Sender = PlugCheckBoxAutoPickUpItem then
    sHint := '勾选后将自动捡起设置有"自动拾取"的物品'
  else if (Sender = PlugCheckBoxAutoGroupAttack) or (Sender = PlugCheckBoxAutoGroupNoAttackMon) then
    sHint := '人物职业为战士时有效';

  DScreen.ClearHint;
  HintWindows.Clear;

  if Length(sHint) > 0 then begin
    vtRect := Ctrl.VirtualRect;
    nHintX := vtRect.Left;
    nHintY := vtRect.Bottom;

    HintWindows.Show(nHintX, nHintY, sHint, clYellow);
  end;
end;

procedure TMirConfigDlg.DEditGroupAttackCountChanged(Sender:TObject);
begin
  g_Config.nGJGroupAttackCount := PlugEditNotGroupAttackCount.Value;
  FrmMain.nGJGroupAttackCount := g_Config.nGJGroupAttackCount;
end;

procedure TMirConfigDlg.ListViewGJMagicItemClick(Sender:TObject; ARow,
  ACol:Integer; ListItem:TObject; ViewItem:Pointer);
begin
  if Sender = PlugMemoConfig82 then
    SaveOrLoadGJMagicList1(True)
  else if Sender = PlugMemoConfig83 then
    SaveOrLoadGJMagicList2(True);
end;

procedure TMirConfigDlg.DBtnGJRunClick(Sender:TObject; X, Y:Integer);
begin
  if not g_boGJRun then
    FrmMain.SendStartGJ
  else
    FrmMain.SendStopGJ;
end;

procedure TMirConfigDlg.PlugBtnGJPointClick(Sender:TObject; X, Y:Integer);
begin
  FrmMain.SendWantMiniMap(True);
end;

procedure TMirConfigDlg.RefreshGJMagic;
var
  I:Integer;
  Magic:PTClientMagic;
  ListItem:TDxListItem;
  ViewItem:pTViewItem;
begin
  PlugMemoConfig82.Clear;
  PlugMemoConfig82.Lock;
  try
    for I := 0 to g_MagicList.Count - 1 do begin
      Magic := g_MagicList.Items[I];

      ListItem := PlugMemoConfig82.Add;
      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Caption := Magic.Def.sMagicName;
      ViewItem.Data := Pointer(Magic.Def.wMagicId);
      ViewItem.Style := bsButton; // bsRadio;
      ViewItem.Alignment := taLeftJustify;
      ViewItem.Color.Up.Color := clWhite;
      ViewItem.Color.Hot.Color := clRed; // clWhite;
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
    for I := 0 to g_MagicList.Count - 1 do begin
      Magic := g_MagicList.Items[I];

      ListItem := PlugMemoConfig83.Add;
      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Caption := Magic.Def.sMagicName;
      ViewItem.Data := Pointer(Magic.Def.wMagicId);
      ViewItem.Style := bsButton; // bsRadio;
      ViewItem.Alignment := taLeftJustify;
      ViewItem.Color.Up.Color := clWhite;
      ViewItem.Color.Hot.Color := clRed; // clWhite;
      ViewItem.Color.Down.Color := clRed;

      ViewItem := ListItem.AddItem('', nil);
      ViewItem.Data := Pointer(Magic.Def.wMagicId);
      ViewItem.Style := bsCheckBox;
      ViewItem.ImageIndex.ImageType := NewopUI_Pak;
      ViewItem.ImageIndex.Up := 228;
      ViewItem.ImageIndex.Down := 229;
      ViewItem.Checked := g_GJUseMagic2.IndexOf(Pointer(Magic.Def.wMagicId)) <> -1;
      ;
    end;
  finally
    PlugMemoConfig83.UnLock;
  end;
end;

procedure TMirConfigDlg.DCheckBoxAutoPercentClick(Sender:TObject; X,
  Y:Integer);
begin
  g_Config.ChkAutoPercents[g_Config.MedicaMode] := PlugCheckBoxAutoPercent.Checked;

  if g_Config.ChkAutoPercents[g_Config.MedicaMode] then begin
    if g_Config.CheckHpPercents[g_Config.MedicaMode] > 99 then begin
      g_Config.CheckHpPercents[g_Config.MedicaMode] := 50;
      PlugEditCheckHPPercent.Value := 50;
    end;

    if g_Config.CheckMpPercents[g_Config.MedicaMode] > 99 then begin
      g_Config.CheckMpPercents[g_Config.MedicaMode] := 50;
      PlugEditCheckMpPercent.Value := 50;
    end;
  end;
end;

procedure TMirConfigDlg.DCheckBoxRenewAutoPercentClick(Sender:TObject; X,
  Y:Integer);
begin
  g_Config.ChkRenewAutoPercents[g_Config.MedicaMode] := PlugCheckBoxRenewAutoPercent.Checked;

  if g_Config.ChkRenewAutoPercents[g_Config.MedicaMode] then begin
    if g_Config.RenewHPPercents[g_Config.MedicaMode] > 99 then begin
      g_Config.RenewHPPercents[g_Config.MedicaMode] := 50;
      PlugEditRenewHPPercent.Value := 50;
    end;

    if g_Config.RenewMpPercents[g_Config.MedicaMode] > 99 then begin
      g_Config.RenewMpPercents[g_Config.MedicaMode] := 50;
      PlugEditRenewMpPercent.Value := 50;
    end;

    if g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] > 99 then begin
      g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] := 30;
      PlugEditRenewSpecialHPPercent.Value := 30;
    end;

    if g_Config.RenewSpecialMpPercents[g_Config.MedicaMode] > 99 then begin
      g_Config.RenewSpecialMpPercents[g_Config.MedicaMode] := 30;
      PlugEditRenewSpecialMpPercent.Value := 30;
    end;
  end;
end;

procedure TMirConfigDlg.DCheckBoxSuperMedicaPercentClick(Sender:TObject;
  X, Y:Integer);
var
  I:Integer;
begin
  g_Config.ChkSuperMedicaPercents[g_Config.MedicaMode] := PlugCheckBoxSuperMedicaPercent.Checked;

  if g_Config.ChkSuperMedicaPercents[g_Config.MedicaMode] then begin
    for I := Low(g_Config.SuperMedicaHPs[g_Config.MedicaMode]) to High(g_Config.SuperMedicaHPs[g_Config.MedicaMode]) do begin
      if g_Config.SuperMedicaHPs[g_Config.MedicaMode][I] > 99 then begin
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

    for I := Low(g_Config.SuperMedicaMPs[g_Config.MedicaMode]) to High(g_Config.SuperMedicaMPs[g_Config.MedicaMode]) do begin
      if g_Config.SuperMedicaMPs[g_Config.MedicaMode][I] > 99 then begin
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

procedure TMirConfigDlg.OnChangedVolumePosition(Sender:TObject);
begin
  g_SoundVolume := TrackBarVolume.Position;
  PlugCheckBoxVolume.Caption := '音量';
end;

procedure TMirConfigDlg.OnChanggingVolumePosition(Sender:TObject);
begin
  g_SoundVolume := TrackBarVolume.Position;
  PlugCheckBoxVolume.Caption := IntToStr(g_SoundVolume);
end;

procedure TMirConfigDlg.DoInitAllComponentsMouseMove(ParentCtrl:TDxControl);
var
  I:Integer;
  DxCtrl:TDxControl;
begin
  for I := 0 to ParentCtrl.ComponentCount - 1 do begin
    DxCtrl := ParentCtrl.Components[I];

    DxCtrl.OnMouseMove := DControlMouseMoveShowHint;

    if DxCtrl.ComponentCount > 0 then begin
      DoInitAllComponentsMouseMove(DxCtrl);
    end;
  end;
end;

procedure TMirConfigDlg.LoadNotesFile;
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
    PlugMemoConfigNotes.LoadFromFile(sFileName);
  end;
end;

procedure TMirConfigDlg.SaveNotesFile;
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
  PlugMemoConfigNotes.Lines.SaveToFile(sFileName);
end;

procedure TMirConfigDlg.OnPlugMemoConfig10ButtonEditClick(Sender:TObject;
  X, Y:Integer);
var
  vtRect:TRect;
begin
  if Sender = PlugMemoConfig10ButtonEdit then begin
    FMemo.Font.Name := '宋体';
    FMemo.Font.Size := 9;

    vtRect := PlugMemoConfigNotes.VirtualRect;

    FMemo.Left := vtRect.Left + 4;
    FMemo.Top := vtRect.Top + 2;
    FMemo.Width := PlugMemoConfigNotes.Width - 20;
    FMemo.Height := PlugMemoConfigNotes.Height - 36;
    FMemo.Lines.Assign(PlugMemoConfigNotes.Lines);
    FMemo.Visible := True;

    PlugConfigDlg.Floating := False;
    //PlugPageControlConfig.Enabled := False;

    PlugMemoConfig10ButtonEdit.Visible := False;
    PlugMemoConfig10ButtonSave.Visible := True;
    PlugMemoConfig10ButtonCancel.Visible := True;
  end
  else if (Sender = PlugMemoConfig10ButtonSave) then begin
    PlugMemoConfigNotes.Lines.Assign(FMemo.Lines);
    SaveNotesFile;
    ShowViewNotes;
  end
  else begin
    ShowViewNotes;
  end;
end;

procedure TMirConfigDlg.OnPlugPageControlConfigActivePageChange(Sender:TObject);
begin
  if PlugPageControlConfig.ActivePage <> PlugTabSheetConfig80 then begin
    if FMemo.Visible then begin
      PlugMemoConfigNotes.Lines.Assign(FMemo.Lines);
      SaveNotesFile;

      ShowViewNotes;
    end;
  end;
end;

procedure TMirConfigDlg.ShowViewNotes;
begin
  PlugMemoConfig10ButtonEdit.Visible := True;
  PlugMemoConfig10ButtonSave.Visible := False;
  PlugMemoConfig10ButtonCancel.Visible := False;

  FMemo.Visible := False;

  PlugConfigDlg.Floating := True;
  //PlugPageControlConfig.Enabled := True;
end;

procedure TMirConfigDlg.OnPopupMenuItemsClick(Sender:TObject; X,
  Y:Integer);
var
  I, MenuItemIndex:Integer;
  ListItem:TDxListItem;
  ShowItem:pTShowItem;
begin
  MenuItemIndex := (Sender as TDxPopupMenu).ItemIndex;
  for I := 0 to PlugMemoConfig2.Count - 1 do begin
    ListItem := PlugMemoConfig2.Items[I];
    if not ListItem.Selected then Continue;

    if ListItem.Count > 0 then begin
      ShowItem := pTShowItem(ListItem.Items[0].Data);

      case MenuItemIndex of
        0:begin
            ShowItem.boHintMsg := True;
            ListItem.Items[1].Checked := ShowItem.boHintMsg;
          end;
        1:begin
            ShowItem.boHintMsg := False;
            ListItem.Items[1].Checked := ShowItem.boHintMsg;
          end;
        3:begin
            ShowItem.boPickup := True;
            ListItem.Items[2].Checked := ShowItem.boPickup;

            g_IsClientPickItemsChanged := True;
          end;
        4:begin
            ShowItem.boPickup := False;
            ListItem.Items[2].Checked := ShowItem.boPickup;

            g_IsClientPickItemsChanged := True;
          end;
        6:begin
            ShowItem.boShowName := True;
            ListItem.Items[3].Checked := ShowItem.boShowName;
          end;
        7:begin
            ShowItem.boShowName := False;
            ListItem.Items[3].Checked := ShowItem.boShowName;
          end;
        9:begin
            ShowItem.boShowSpecial := True;
            ListItem.Items[4].Checked := ShowItem.boShowSpecial;

            g_IsClientPickItemsChanged := True;
          end;
        10:begin
            ShowItem.boShowSpecial := False;
            ListItem.Items[4].Checked := ShowItem.boShowSpecial;

            g_IsClientPickItemsChanged := True;
          end;
        12:begin
            ShowItem.boAutoMove := True;
            ListItem.Items[5].Checked := ShowItem.boAutoMove;
          end;
        13:begin
            ShowItem.boAutoMove := False;
            ListItem.Items[5].Checked := ShowItem.boAutoMove;
          end;
      end;
    end;
  end;

  g_FileItemDB.SaveToFile;
  g_DropItemsMgr.RefreshDrawList;
end;

procedure TMirConfigDlg.RefBindItemList;
var
  I:Integer;
  BindItem:pTCustomBindItem;
begin
  PlugScrollBoxUnbindItems.Clear;
  for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
    BindItem := g_CustomUnbindItemList.Items[I];
    PlugScrollBoxUnbindItems.Lines.AddObject(BindItem.sItemName, TObject(BindItem));
  end;
end;

procedure TMirConfigDlg.AddToBossList(sName:string);
begin
    //PlugScrollBoxBoss.Lines.Text := g_BossList.Text;
    if PlugScrollBoxBoss.Lines.IndexOf(sName) < 0 then begin
        PlugScrollBoxBoss.Lines.Add(sName);
        SaveOrLoadBossList(True);
    end;
end;

procedure TMirConfigDlg.RemoveFromBossList(sName:string);
var
   nIndex:Integer;
begin
   nIndex := PlugScrollBoxBoss.Lines.IndexOf(sName);
   if nIndex >= 0 then begin
       PlugScrollBoxBoss.Lines.Delete(nIndex);
       SaveOrLoadBossList(True);
   end;
end;

procedure TMirConfigDlg.AddOrRemoveBossList(sName:string);
var
    nIndex:Integer;
begin
    nIndex := PlugScrollBoxBoss.Lines.IndexOf(sName);
    if nIndex >= 0 then begin
        PlugScrollBoxBoss.Lines.Delete(nIndex);
    end else begin
        PlugScrollBoxBoss.Lines.Add(sName);
    end;
    SaveOrLoadBossList(True);
end;

procedure TMirConfigDlg.PlugScrollBoxUnbindItemsClick(Sender:TObject; X,
  Y:Integer);
var
  BindItem:pTCustomBindItem;
begin
  if (PlugScrollBoxUnbindItems.ItemIndex >= 0) and (PlugScrollBoxUnbindItems.ItemIndex <= PlugScrollBoxUnbindItems.Lines.Count - 1) then begin
    PlugBtnUnbindItemDel.Enabled := True;
    PlugBtnUnbindItemEdit.Enabled := True;
    BindItem := pTCustomBindItem(PlugScrollBoxUnbindItems.Lines.Objects[PlugScrollBoxUnbindItems.ItemIndex]);

    if BindItem.UnBindItemType = t_Special then begin
      if not BindItem.boSpecialMP then
        PlugComboGroupUnBindItem.ItemIndex := Integer(BindItem.UnBindItemType) - 1
      else
        PlugComboGroupUnBindItem.ItemIndex := Integer(BindItem.UnBindItemType);
    end
    else if BindItem.UnBindItemType = t_Book then begin
      PlugComboGroupUnBindItem.ItemIndex := 4;
    end
    else begin
      PlugComboGroupUnBindItem.ItemIndex := Integer(BindItem.UnBindItemType) - 1;
    end;

    PlugEditItemName.Text := BindItem.sItemName;
    PlugEditUnbindName.Text := BindItem.sBindItemName;
  end
  else begin
    PlugBtnUnbindItemDel.Enabled := False;
    PlugBtnUnbindItemEdit.Enabled := False;
  end;
end;

procedure TMirConfigDlg.PlugBtnUnbindItemClick(Sender:TObject; X,
  Y:Integer);
var
  I:Integer;
  BindItem:pTCustomBindItem;
  sItemName:string;
  sBindItemName:string;
  sDirectory, sFileName:string;
begin
  if Sender = PlugBtnUnbindItemAdd then begin
    sItemName := PlugEditItemName.Text;
    sBindItemName := PlugEditUnbindName.Text;

    if (sItemName = '') then begin
      FrmDlg.DMessageDlg('请输入物品名称！', [mbOk]);
      PlugEditItemName.SetFocus;
      Exit;
    end;

    New(BindItem);
    BindItem.UnBindItemType := TUnBindItemType(PlugComboGroupUnBindItem.ItemIndex + 1);
    BindItem.sItemName := sItemName;
    BindItem.sBindItemName := sBindItemName;
    BindItem.boSpecialMP := False;
    if BindItem.UnBindItemType = t_Book then begin
      BindItem.UnBindItemType := t_Special;
      BindItem.boSpecialMP := True;
    end
    else if BindItem.UnBindItemType = t_Poison then
      BindItem.UnBindItemType := t_Book;

    g_CustomUnbindItemList.Add(BindItem);

    RefBindItemList;

    PlugBtnUnbindItemSave.Enabled := True;
    RefreshUnBindItemList;
  end
  else if Sender = PlugBtnUnbindItemDel then begin
    if PlugScrollBoxUnbindItems.ItemIndex >= 0 then begin
      BindItem := pTCustomBindItem(PlugScrollBoxUnbindItems.Lines.Objects[PlugScrollBoxUnbindItems.ItemIndex]);
      for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
        if BindItem = g_CustomUnbindItemList.Items[I] then begin
          g_CustomUnbindItemList.Delete(I);
          Dispose(BindItem);
          break;
        end;
      end;
      RefBindItemList;
      PlugBtnUnbindItemSave.Enabled := True;
    end
    else begin
      PlugBtnUnbindItemDel.Enabled := False;
      PlugBtnUnbindItemEdit.Enabled := False;
    end;
  end
  else if Sender = PlugBtnUnbindItemEdit then begin
    sItemName := PlugEditItemName.Text;
    sBindItemName := PlugEditUnbindName.Text;

    if (sItemName = '') then begin
      FrmDlg.DMessageDlg('请输入物品名称！', [mbOk]);
      PlugEditItemName.SetFocus;
      Exit;
    end;

    if PlugScrollBoxUnbindItems.ItemIndex >= 0 then begin
      BindItem := pTCustomBindItem(PlugScrollBoxUnbindItems.Lines.Objects[PlugScrollBoxUnbindItems.ItemIndex]);
      BindItem.UnBindItemType := TUnBindItemType(PlugComboGroupUnBindItem.ItemIndex + 1);
      BindItem.sItemName := sItemName;
      BindItem.sBindItemName := sBindItemName;

      BindItem.boSpecialMP := False;
      if BindItem.UnBindItemType = t_Book then begin
        BindItem.UnBindItemType := t_Special;
        BindItem.boSpecialMP := True;
      end
      else if BindItem.UnBindItemType = t_Poison then
        BindItem.UnBindItemType := t_Book;

      RefBindItemList;
      PlugBtnUnbindItemSave.Enabled := True;

      RefreshUnBindItemList;
    end;
  end
  else if Sender = PlugBtnUnbindItemSave then begin
    sDirectory := g_sSelfFilePath + 'Config\';
    if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);
    sFileName := sDirectory + g_sPlugServerName + DecodeResStr(SBindItemFileName);
    SaveNGCustomUnbindItemList(sFileName);
    PlugBtnUnbindItemSave.Enabled := False;
  end;
end;

end.

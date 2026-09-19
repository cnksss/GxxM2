unit StateWindows;

interface
uses
  Windows,
  Messages,
  SysUtils,
  StrUtils,
  Classes,
  Graphics,
  Controls,
  Forms,
  Dialogs,
  StdCtrls,
  Grids,
  IniFiles,
  DxImageForm,
  DxImageButton,
  DxPageControl,
  DxEdit,
  DxLabel,
  DxMemo,
  DxImageGrid,
  DxPopupMenu,
  DxComboBox,
  DxLine,
  DxControls,
  DxComponents,
  Grobal2,
  ClFunc,
  HUtil32,
  MapUnit,
  SoundUtil,
  DxSexPanel,
  DxSwitchButton,
  DxImageProgress,
  HGE,
  HGEFontEx,
  ComCtrls,
  Actor,
  GameImages,
  DxCanvas,
  HGECanvas,
  DrawScrn,
  EDcode;

type
  TStateWindows = class
    ContinuousMagicMenu:TDxPopupMenu;
    DSWGodBlessDlg:TDxImageForm;
    DSWGodBless1:TDxImageButton;
    DSWGodBless10:TDxImageButton;
    DSWGodBless11:TDxImageButton;
    DSWGodBless12:TDxImageButton;
    DSWGodBless2:TDxImageButton;
    DSWGodBless3:TDxImageButton;
    DSWGodBless4:TDxImageButton;
    DSWGodBless5:TDxImageButton;
    DSWGodBless6:TDxImageButton;
    DSWGodBless7:TDxImageButton;
    DSWGodBless8:TDxImageButton;
    DSWGodBless9:TDxImageButton;
    DSWGodBlessClose:TDxImageButton;
    DSWGodBlessCusBtn1:TDxImageButton;
    DSWGodBlessCusBtn2:TDxImageButton;
    DSWGodBlessCusBtn3:TDxImageButton;
    DSWGodBlessInfo:TDxImageButton;
    DSWJewelryBoxDlg:TDxImageForm;
    DSWJewelry1:TDxImageButton;
    DSWJewelry2:TDxImageButton;
    DSWJewelry3:TDxImageButton;
    DSWJewelry4:TDxImageButton;
    DSWJewelry5:TDxImageButton;
    DSWJewelry6:TDxImageButton;
    DSWJewelryBoxCusBtn1:TDxImageButton;
    DSWJewelryBoxCusBtn2:TDxImageButton;
    DSWJewelryBoxCusBtn3:TDxImageButton;
    DSWJewelryClose:TDxImageButton;
    DSWPetsBagDlg:TDxImageForm;
    DSWPetsBagClose:TDxImageButton;
    DSWPetsGrid:TDxImageGrid;
    DSWPetsDlg:TDxImageForm;
    DSWPetsAC:TDxLabel;
    DSWPetsACTitle:TDxLabel;
    DSWPetsClose:TDxImageButton;
    DSWPetsCusBtn1:TDxImageButton;
    DSWPetsCusBtn2:TDxImageButton;
    DSWPetsCusBtn3:TDxImageButton;
    DSWPetsCusBtn4:TDxImageButton;
    DSWPetsCusBtn5:TDxImageButton;
    DSWPetsCusBtn6:TDxImageButton;
    DSWPetsCusBtn7:TDxImageButton;
    DSWPetsCusBtn8:TDxImageButton;
    DSWPetsCusBtn9:TDxImageButton;
    DSWPetsDC:TDxLabel;
    DSWPetsDCTitle:TDxLabel;
    DSWPetsExp:TDxLabel;
    DSWPetsExpTitle:TDxLabel;
    DSWPetsFree:TDxImageButton;
    DSWPetsHP:TDxLabel;
    DSWPetsHPTitle:TDxLabel;
    DSWPetsLevel:TDxLabel;
    DSWPetsLevelTitle:TDxLabel;
    DSWPetsList:TDxListView;
    DSWPetsMAC:TDxLabel;
    DSWPetsMACTitle:TDxLabel;
    DSWPetsMC:TDxLabel;
    DSWPetsMCTitle:TDxLabel;
    DSWPetsMagic1:TDxImageButton;
    DSWPetsMagic2:TDxImageButton;
    DSWPetsMagic3:TDxImageButton;
    DSWPetsMagic4:TDxImageButton;
    DSWPetsMagic5:TDxImageButton;
    DSWPetsMagic6:TDxImageButton;
    DSWPetsMagic7:TDxImageButton;
    DSWPetsMagic8:TDxImageButton;
    DSWPetsName:TDxLabel;
    DSWPetsNameTitle:TDxLabel;
    DSWPetsRecall:TDxImageButton;
    DSWPetsRetake:TDxImageButton;
    DSWPetsSC:TDxLabel;
    DSWPetsSCTitle:TDxLabel;
    DSWPetsShow:TDxImageForm;
    DSWPetsTakeBag:TDxImageButton;
    DSWPetsViewBag:TDxImageButton;
    DStateWin:TDxImageForm;
    DCloseState:TDxImageButton;
    DStatePageControl:TDxPageControl;
    MainTabSheet1:TDxTabSheet;
    DBasicPageControl:TDxPageControl;
    DStateTabSheet1:TDxTabSheet;
    DStateBasic:TDxSexPanel;
    DSWArmRingL:TDxImageButton;
    DSWArmRingR:TDxImageButton;
    DSWBelt:TDxImageButton;
    DSWBoots:TDxImageButton;
    DSWBujuk:TDxImageButton;
    DSWCharm:TDxImageButton;
    DSWCustomButton1:TDxImageButton;
    DSWCustomButton2:TDxImageButton;
    DSWCustomButton3:TDxImageButton;
    DSWDress:TDxImageButton;
    DSWDrum:TDxImageButton;
    DSWGodBless:TDxImageButton;
    DSWHelmet:TDxImageButton;
    DSWHorse:TDxImageButton;
    DSWJade:TDxImageButton;
    DSWJewelryBox:TDxImageButton;
    DSWLight:TDxImageButton;
    DSWNecklace:TDxImageButton;
    DSWPets:TDxImageButton;
    DSWRingL:TDxImageButton;
    DSWRingR:TDxImageButton;
    DSWShield:TDxImageButton;
    DSWWeapon:TDxImageButton;
    LabelDStateWinRankName:TDxLabel;
    DStateTabSheet2:TDxTabSheet;
    DStateFashion:TDxSexPanel;
    DSWFashionArmRingL:TDxImageButton;
    DSWFashionArmRingR:TDxImageButton;
    DSWFashionBelt:TDxImageButton;
    DSWFashionBoots:TDxImageButton;
    DSWFashionCustomButton1:TDxImageButton;
    DSWFashionCustomButton2:TDxImageButton;
    DSWFashionCustomButton3:TDxImageButton;
    DSWFashionDress:TDxImageButton;
    DSWFashionDrum:TDxImageButton;
    DSWFashionHelmet:TDxImageButton;
    DSWFashionLight:TDxImageButton;
    DSWFashionNecklace:TDxImageButton;
    DSWFashionRingL:TDxImageButton;
    DSWFashionRingR:TDxImageButton;
    DSWFashionWeapon:TDxImageButton;
    DSWShowFashion:TDxImageButton;
    DStateTabSheet3:TDxTabSheet;
    DStateInfo:TDxImageForm;
    DStateAlcoholLabel:TDxLabel;
    DStateBigGoldLabel:TDxLabel;
    DStateCreditPointLabel:TDxLabel;
    DStateCustomButton1:TDxImageButton;
    DStateCustomButton2:TDxImageButton;
    DStateCustomButton3:TDxImageButton;
    DStateExpLabel:TDxLabel;
    DStateGameDiamondLabel:TDxLabel;
    DStateGameGirdLabel:TDxLabel;
    DStateGameGoldLabel:TDxLabel;
    DStateGameTimeGirdLabel:TDxLabel;
    DStateHPLabel:TDxLabel;
    DStateJobLabel:TDxLabel;
    DStateLevelLabel:TDxLabel;
    DStateLiquorLabel:TDxLabel;
    DStateMPLabel:TDxLabel;
    DStateMaxExpLabel:TDxLabel;
    DStatePotencyLabel:TDxLabel;
    DStateRefurbishLabel:TDxLabel;
    DStateTabSheet4:TDxTabSheet;
    DStateAbil:TDxImageForm;
    DAbilCustomButton1:TDxImageButton;
    DAbilCustomButton2:TDxImageButton;
    DAbilCustomButton3:TDxImageButton;
    DStateAntiMagicLabel:TDxLabel;
    DStateAntiPoisonLabel:TDxLabel;
    DStateHandWeightLabel:TDxLabel;
    DStateHealthRecoverLabel:TDxLabel;
    DStateHitPointLabel:TDxLabel;
    DStateHitSpeedLabel:TDxLabel;
    DStateLabelAC:TDxLabel;
    DStateLabelDC:TDxLabel;
    DStateLabelMAC:TDxLabel;
    DStateLabelMC:TDxLabel;
    DStateLabelSC:TDxLabel;
    DStateNewAbilLabel1:TDxLabel;
    DStateNewAbilLabel10:TDxLabel;
    DStateNewAbilLabel11:TDxLabel;
    DStateNewAbilLabel12:TDxLabel;
    DStateNewAbilLabel13:TDxLabel;
    DStateNewAbilLabel14:TDxLabel;
    DStateNewAbilLabel15:TDxLabel;
    DStateNewAbilLabel16:TDxLabel;
    DStateNewAbilLabel2:TDxLabel;
    DStateNewAbilLabel3:TDxLabel;
    DStateNewAbilLabel4:TDxLabel;
    DStateNewAbilLabel5:TDxLabel;
    DStateNewAbilLabel6:TDxLabel;
    DStateNewAbilLabel7:TDxLabel;
    DStateNewAbilLabel8:TDxLabel;
    DStateNewAbilLabel9:TDxLabel;
    DStatePoisonRecoverLabel:TDxLabel;
    DStateSpeedPointLabel:TDxLabel;
    DStateSpellRecoverLabel:TDxLabel;
    DStateWearWeightLabel:TDxLabel;
    DStateWeightLabel:TDxLabel;
    DStateTabSheet5:TDxTabSheet;
    DStateTitle:TDxImageForm;
    DSTitleActive:TDxImageButton;
    DSTitleButton1:TDxImageButton;
    DSTitleButton2:TDxImageButton;
    DSTitleButton3:TDxImageButton;
    DSTitleButton4:TDxImageButton;
    DSTitleButton5:TDxImageButton;
    DSTitleButton6:TDxImageButton;
    DSTitleItemDesc:TDxImageButton;
    DSTitleName1:TDxLabel;
    DSTitleName2:TDxLabel;
    DSTitleName3:TDxLabel;
    DSTitleName4:TDxLabel;
    DSTitleName5:TDxLabel;
    DSTitleName6:TDxLabel;
    DSTitleNameActive:TDxLabel;
    DSTitlePageDown:TDxImageButton;
    DSTitlePageUp:TDxImageButton;
    DTitleCustomButton1:TDxImageButton;
    DTitleCustomButton2:TDxImageButton;
    DTitleCustomButton3:TDxImageButton;
    DStateTabSheet6:TDxTabSheet;
    DMagCustomButton1:TDxImageButton;
    DMagCustomButton2:TDxImageButton;
    DMagCustomButton3:TDxImageButton;
    DStMagicPage:TDxLabel;
    DStPageDown:TDxImageButton;
    DStPageUp:TDxImageButton;
    DStateMagic:TDxScrollBox;
    DStMagBack1:TDxImageForm;
    DStMag1:TDxImageButton;
    DStMagBtn1:TDxImageButton;
    DStMagExpIcon1:TDxImageButton;
    DStMagExpText1:TDxLabel;
    DStMagLblName1:TDxLabel;
    DStMagLvIcon1:TDxImageButton;
    DStMagLvText1:TDxLabel;
    DStMagBack2:TDxImageForm;
    DStMag2:TDxImageButton;
    DStMagBtn2:TDxImageButton;
    DStMagBack3:TDxImageForm;
    DStMag3:TDxImageButton;
    DStMagBtn3:TDxImageButton;
    DStMagBack4:TDxImageForm;
    DStMag4:TDxImageButton;
    DStMagBtn4:TDxImageButton;
    DStMagBack5:TDxImageForm;
    DStMag5:TDxImageButton;
    DStMagBtn5:TDxImageButton;
    DStMagBack6:TDxImageForm;
    DStMag6:TDxImageButton;
    DStMagBtn6:TDxImageButton;
    DStateTabSheet7:TDxTabSheet;
    DStateDeputyHero:TDxImageForm;
    DBotStateCustomButton1:TDxImageButton;
    DBotStateCustomButton10:TDxImageButton;
    DBotStateCustomButton2:TDxImageButton;
    DBotStateCustomButton3:TDxImageButton;
    DBotStateCustomButton4:TDxImageButton;
    DBotStateCustomButton5:TDxImageButton;
    DBotStateCustomButton6:TDxImageButton;
    DBotStateCustomButton7:TDxImageButton;
    DBotStateCustomButton8:TDxImageButton;
    DBotStateCustomButton9:TDxImageButton;
    DBotStateDeputyHeroJob0:TDxImageButton;
    DBotStateDeputyHeroJob1:TDxImageButton;
    DBotStateDeputyHeroJob2:TDxImageButton;
    MainTabSheet2:TDxTabSheet;
    DNGPageControl:TDxPageControl;
    DNGStateTabSheet1:TDxTabSheet;
    DStateNGInfo:TDxImageForm;
    DStateHJAntiPoisonLabel:TDxLabel;
    DStateHJAntiPoisonValue:TDxLabel;
    DStateNGAntiPoisonLabel:TDxLabel;
    DStateNGAntiPoisonValue:TDxLabel;
    DStateNGCusBtn1:TDxImageButton;
    DStateNGCusBtn2:TDxImageButton;
    DStateNGCusBtn3:TDxImageButton;
    DStateNGExpLabel:TDxLabel;
    DStateNGExpValue:TDxLabel;
    DStateNGHitLabel:TDxLabel;
    DStateNGHitValue:TDxLabel;
    DStateNGLabel:TDxLabel;
    DStateNGLevelLabel:TDxLabel;
    DStateNGLevelValue:TDxLabel;
    DStateNGMaxExpLabel:TDxLabel;
    DStateNGMaxExpValue:TDxLabel;
    DStateNGRecoverLabel:TDxLabel;
    DStateNGRecoverValue:TDxLabel;
    DStateNGValue:TDxLabel;
    DNGStateTabSheet2:TDxTabSheet;
    DStNGMagicPage:TDxLabel;
    DStNGPageDown:TDxImageButton;
    DStNGPageUp:TDxImageButton;
    DStateNGMagic:TDxScrollBox;
    DStNGMagBack1:TDxImageForm;
    DStNGMag1:TDxImageButton;
    DStNGMagExpIcon1:TDxImageButton;
    DStNGMagExpText1:TDxLabel;
    DStNGMagLblName1:TDxLabel;
    DStNGMagLvIcon1:TDxImageButton;
    DStNGMagLvText1:TDxLabel;
    DStNGMagBack2:TDxImageForm;
    DStNGMag2:TDxImageButton;
    DStNGMagBack3:TDxImageForm;
    DStNGMag3:TDxImageButton;
    DStNGMagBack4:TDxImageForm;
    DStNGMag4:TDxImageButton;
    DStNGMagBack5:TDxImageForm;
    DStNGMag5:TDxImageButton;
    DStNGMagBack6:TDxImageForm;
    DStNGMag6:TDxImageButton;
    DStateNGMagicCusBnt1:TDxImageButton;
    DStateNGMagicCusBnt2:TDxImageButton;
    DStateNGMagicCusBnt3:TDxImageButton;
    DNGStateTabSheet3:TDxTabSheet;
    DStateNGMeridians:TDxSexPanel;
    DBotMeridians0:TDxImageButton;
    DBotMeridians1:TDxImageButton;
    DBotMeridians2:TDxImageButton;
    DBotMeridians3:TDxImageButton;
    DBotMeridians4:TDxImageButton;
    DMeridiansPageControl:TDxPageControl;
    DMeridiansTabSheet0:TDxTabSheet;
    DMeridiansLine0:TDxImageFormShape;
    DBotAcupoints0_0:TDxImageButton;
    DBotAcupoints0_1:TDxImageButton;
    DBotAcupoints0_2:TDxImageButton;
    DBotAcupoints0_3:TDxImageButton;
    DBotAcupoints0_4:TDxImageButton;
    DLabelAcupoints0_0:TDxLabel;
    DLabelAcupoints0_1:TDxLabel;
    DLabelAcupoints0_2:TDxLabel;
    DLabelAcupoints0_3:TDxLabel;
    DLabelAcupoints0_4:TDxLabel;
    DMeridiansTabSheet1:TDxTabSheet;
    DMeridiansLine1:TDxImageFormShape;
    DBotAcupoints1_0:TDxImageButton;
    DBotAcupoints1_1:TDxImageButton;
    DBotAcupoints1_2:TDxImageButton;
    DBotAcupoints1_3:TDxImageButton;
    DBotAcupoints1_4:TDxImageButton;
    DLabelAcupoints1_0:TDxLabel;
    DLabelAcupoints1_1:TDxLabel;
    DLabelAcupoints1_2:TDxLabel;
    DLabelAcupoints1_3:TDxLabel;
    DLabelAcupoints1_4:TDxLabel;
    DLabelMeridiansStatus1:TDxLabel;
    DMeridiansTabSheet2:TDxTabSheet;
    DMeridiansLine2:TDxImageFormShape;
    DBotAcupoints2_0:TDxImageButton;
    DBotAcupoints2_1:TDxImageButton;
    DBotAcupoints2_2:TDxImageButton;
    DBotAcupoints2_3:TDxImageButton;
    DBotAcupoints2_4:TDxImageButton;
    DLabelAcupoints2_0:TDxLabel;
    DLabelAcupoints2_1:TDxLabel;
    DLabelAcupoints2_2:TDxLabel;
    DLabelAcupoints2_3:TDxLabel;
    DLabelAcupoints2_4:TDxLabel;
    DLabelMeridiansStatus2:TDxLabel;
    DMeridiansTabSheet3:TDxTabSheet;
    DMeridiansLine3:TDxImageFormShape;
    DBotAcupoints3_0:TDxImageButton;
    DBotAcupoints3_1:TDxImageButton;
    DBotAcupoints3_2:TDxImageButton;
    DBotAcupoints3_3:TDxImageButton;
    DBotAcupoints3_4:TDxImageButton;
    DLabelAcupoints3_0:TDxLabel;
    DLabelAcupoints3_1:TDxLabel;
    DLabelAcupoints3_2:TDxLabel;
    DLabelAcupoints3_3:TDxLabel;
    DLabelAcupoints3_4:TDxLabel;
    DLabelMeridiansStatus3:TDxLabel;
    DMeridiansTabSheet4:TDxTabSheet;
    DMeridiansLine4:TDxImageFormShape;
    DBotAcupoints4_0:TDxImageButton;
    DBotAcupoints4_1:TDxImageButton;
    DBotAcupoints4_2:TDxImageButton;
    DBotAcupoints4_3:TDxImageButton;
    DBotAcupoints4_4:TDxImageButton;
    DLabelAcupoints4_0:TDxLabel;
    DLabelAcupoints4_1:TDxLabel;
    DLabelAcupoints4_2:TDxLabel;
    DLabelAcupoints4_3:TDxLabel;
    DLabelAcupoints4_4:TDxLabel;
    DLabelMeridiansStatus4:TDxLabel;
    DStateNGMeridiansCusBtn1:TDxImageButton;
    DStateNGMeridiansCusBtn2:TDxImageButton;
    DStateNGMeridiansCusBtn3:TDxImageButton;
    DTrainingMeridian:TDxImageButton;
    DNGStateTabSheet4:TDxTabSheet;
    DStateNGContinuousMagic:TDxImageForm;
    DContinuousMagicList:TDxScrollBox;
    DStLJMagBack1:TDxImageForm;
    DStLJMag1:TDxImageButton;
    DStLJMagExpIcon1:TDxImageButton;
    DStLJMagExpText1:TDxLabel;
    DStLJMagLblName1:TDxLabel;
    DStLJMagLvIcon1:TDxImageButton;
    DStLJMagLvText1:TDxLabel;
    DStLJMagBack2:TDxImageForm;
    DStLJMag2:TDxImageButton;
    DStLJMagBack3:TDxImageForm;
    DStLJMag3:TDxImageButton;
    DStLJMagBack4:TDxImageForm;
    DStLJMag4:TDxImageButton;
    DSMB1:TDxImageButton;
    DSMB2:TDxImageButton;
    DSMB3:TDxImageButton;
    DSMB4:TDxImageButton;
    DSMBShortcut:TDxImageButton;
    DStateNGMagic2CusBtn1:TDxImageButton;
    DStateNGMagic2CusBtn2:TDxImageButton;
    DStateNGMagic2CusBtn3:TDxImageButton;
    MainTabSheet3:TDxTabSheet;
    DXFPageControl:TDxPageControl;
    LabelDStateWinCharName:TDxLabel;
    btnDStateWinEx:TDxSwitchButton;
    DStateWinEx:TDxImageForm;
    btnStateExtExp:TDxImageProgress;
    btnStateExtHP:TDxImageProgress;
    btnStateExtHandWeight:TDxImageProgress;
    btnStateExtMC:TDxImageProgress;
    btnStateExtNG:TDxImageProgress;
    btnStateExtWearWeight:TDxImageProgress;
    btnStateExtWeight:TDxImageProgress;
    lblStateExtAC:TDxLabel;
    lblStateExtAntiMagic:TDxLabel;
    lblStateExtAntiPoiston:TDxLabel;
    lblStateExtAttackSpeed:TDxLabel;
    lblStateExtDC:TDxLabel;
    lblStateExtGuild:TDxLabel;
    lblStateExtHPRecover:TDxLabel;
    lblStateExtHitPoint:TDxLabel;
    lblStateExtJob:TDxLabel;
    lblStateExtLevel:TDxLabel;
    lblStateExtMAC:TDxLabel;
    lblStateExtMC:TDxLabel;
    lblStateExtMPRecover:TDxLabel;
    lblStateExtNewValue0:TDxLabel;
    lblStateExtNewValue1:TDxLabel;
    lblStateExtNewValue10:TDxLabel;
    lblStateExtNewValue11:TDxLabel;
    lblStateExtNewValue12:TDxLabel;
    lblStateExtNewValue13:TDxLabel;
    lblStateExtNewValue14:TDxLabel;
    lblStateExtNewValue15:TDxLabel;
    lblStateExtNewValue16:TDxLabel;
    lblStateExtNewValue17:TDxLabel;
    lblStateExtNewValue18:TDxLabel;
    lblStateExtNewValue19:TDxLabel;
    lblStateExtNewValue2:TDxLabel;
    lblStateExtNewValue20:TDxLabel;
    lblStateExtNewValue21:TDxLabel;
    lblStateExtNewValue22:TDxLabel;
    lblStateExtNewValue23:TDxLabel;
    lblStateExtNewValue3:TDxLabel;
    lblStateExtNewValue4:TDxLabel;
    lblStateExtNewValue5:TDxLabel;
    lblStateExtNewValue6:TDxLabel;
    lblStateExtNewValue7:TDxLabel;
    lblStateExtNewValue8:TDxLabel;
    lblStateExtNewValue9:TDxLabel;
    lblStateExtPosionRecover:TDxLabel;
    lblStateExtSC:TDxLabel;
    lblStateExtSpeedPoint:TDxLabel;
    DHeroSWGodBlessDlg:TDxImageForm;
    DHeroSWGodBless1:TDxImageButton;
    DHeroSWGodBless10:TDxImageButton;
    DHeroSWGodBless11:TDxImageButton;
    DHeroSWGodBless12:TDxImageButton;
    DHeroSWGodBless2:TDxImageButton;
    DHeroSWGodBless3:TDxImageButton;
    DHeroSWGodBless4:TDxImageButton;
    DHeroSWGodBless5:TDxImageButton;
    DHeroSWGodBless6:TDxImageButton;
    DHeroSWGodBless7:TDxImageButton;
    DHeroSWGodBless8:TDxImageButton;
    DHeroSWGodBless9:TDxImageButton;
    DHeroSWGodBlessClose:TDxImageButton;
    DHeroSWGodBlessInfo:TDxImageButton;
    DHeroSWJewelryBoxDlg:TDxImageForm;
    DHeroSWJewelry1:TDxImageButton;
    DHeroSWJewelry2:TDxImageButton;
    DHeroSWJewelry3:TDxImageButton;
    DHeroSWJewelry4:TDxImageButton;
    DHeroSWJewelry5:TDxImageButton;
    DHeroSWJewelry6:TDxImageButton;
    DHeroSWJewelryClose:TDxImageButton;
    DHeroStateWin:TDxImageForm;
    DHeroCloseState:TDxImageButton;
    DHeroLabelDStateWinCharName:TDxLabel;
    DHeroStatePageControl:TDxPageControl;
    HeroMainTabSheet1:TDxTabSheet;
    DHeroBasicPageControl:TDxPageControl;
    DHeroStateTabSheet1:TDxTabSheet;
    DHeroStateBasic:TDxSexPanel;
    DHeroSWArmRingL:TDxImageButton;
    DHeroSWArmRingR:TDxImageButton;
    DHeroSWBelt:TDxImageButton;
    DHeroSWBoots:TDxImageButton;
    DHeroSWBujuk:TDxImageButton;
    DHeroSWCharm:TDxImageButton;
    DHeroSWDress:TDxImageButton;
    DHeroSWDrum:TDxImageButton;
    DHeroSWGodBless:TDxImageButton;
    DHeroSWHelmet:TDxImageButton;
    DHeroSWHorse:TDxImageButton;
    DHeroSWJade:TDxImageButton;
    DHeroSWJewelryBox:TDxImageButton;
    DHeroSWLight:TDxImageButton;
    DHeroSWNecklace:TDxImageButton;
    DHeroSWRingL:TDxImageButton;
    DHeroSWRingR:TDxImageButton;
    DHeroSWShield:TDxImageButton;
    DHeroSWWeapon:TDxImageButton;
    DHeroStateTabSheet2:TDxTabSheet;
    DHeroStateFashion:TDxSexPanel;
    DHeroSWFashionArmRingL:TDxImageButton;
    DHeroSWFashionArmRingR:TDxImageButton;
    DHeroSWFashionBelt:TDxImageButton;
    DHeroSWFashionBoots:TDxImageButton;
    DHeroSWFashionDress:TDxImageButton;
    DHeroSWFashionDrum:TDxImageButton;
    DHeroSWFashionHelmet:TDxImageButton;
    DHeroSWFashionLight:TDxImageButton;
    DHeroSWFashionNecklace:TDxImageButton;
    DHeroSWFashionRingL:TDxImageButton;
    DHeroSWFashionRingR:TDxImageButton;
    DHeroSWFashionWeapon:TDxImageButton;
    DHeroSWShowFashion:TDxImageButton;
    DHeroStateTabSheet3:TDxTabSheet;
    DHeroStateInfo:TDxImageForm;
    DHeroStateAlcoholLabel:TDxLabel;
    DHeroStateCreditPointLabel:TDxLabel;
    DHeroStateExpLabel:TDxLabel;
    DHeroStateHPLabel:TDxLabel;
    DHeroStateJobLabel:TDxLabel;
    DHeroStateLevelLabel:TDxLabel;
    DHeroStateLiquorLabel:TDxLabel;
    DHeroStateMPLabel:TDxLabel;
    DHeroStateMaxExpLabel:TDxLabel;
    DHeroStatePotencyLabel:TDxLabel;
    DHeroStateTabSheet4:TDxTabSheet;
    DHeroStateAbil:TDxImageForm;
    DHeroStateAntiMagicLabel:TDxLabel;
    DHeroStateAntiPoisonLabel:TDxLabel;
    DHeroStateHandWeightLabel:TDxLabel;
    DHeroStateHealthRecoverLabel:TDxLabel;
    DHeroStateHitPointLabel:TDxLabel;
    DHeroStateHitSpeedLabel:TDxLabel;
    DHeroStateLabelAC:TDxLabel;
    DHeroStateLabelDC:TDxLabel;
    DHeroStateLabelMAC:TDxLabel;
    DHeroStateLabelMC:TDxLabel;
    DHeroStateLabelSC:TDxLabel;
    DHeroStateNewAbilLabel1:TDxLabel;
    DHeroStateNewAbilLabel10:TDxLabel;
    DHeroStateNewAbilLabel11:TDxLabel;
    DHeroStateNewAbilLabel12:TDxLabel;
    DHeroStateNewAbilLabel13:TDxLabel;
    DHeroStateNewAbilLabel14:TDxLabel;
    DHeroStateNewAbilLabel15:TDxLabel;
    DHeroStateNewAbilLabel16:TDxLabel;
    DHeroStateNewAbilLabel2:TDxLabel;
    DHeroStateNewAbilLabel3:TDxLabel;
    DHeroStateNewAbilLabel4:TDxLabel;
    DHeroStateNewAbilLabel5:TDxLabel;
    DHeroStateNewAbilLabel6:TDxLabel;
    DHeroStateNewAbilLabel7:TDxLabel;
    DHeroStateNewAbilLabel8:TDxLabel;
    DHeroStateNewAbilLabel9:TDxLabel;
    DHeroStatePoisonRecoverLabel:TDxLabel;
    DHeroStateSpeedPointLabel:TDxLabel;
    DHeroStateSpellRecoverLabel:TDxLabel;
    DHeroStateWearWeightLabel:TDxLabel;
    DHeroStateWeightLabel:TDxLabel;
    DHeroStateTabSheet5:TDxTabSheet;
    DHeroStateTitle:TDxImageForm;
    DSHeroTitleActive:TDxImageButton;
    DSHeroTitleButton1:TDxImageButton;
    DSHeroTitleButton2:TDxImageButton;
    DSHeroTitleButton3:TDxImageButton;
    DSHeroTitleButton4:TDxImageButton;
    DSHeroTitleButton5:TDxImageButton;
    DSHeroTitleButton6:TDxImageButton;
    DSHeroTitleItemDesc:TDxImageButton;
    DSHeroTitleName1:TDxLabel;
    DSHeroTitleName2:TDxLabel;
    DSHeroTitleName3:TDxLabel;
    DSHeroTitleName4:TDxLabel;
    DSHeroTitleName5:TDxLabel;
    DSHeroTitleName6:TDxLabel;
    DSHeroTitleNameActive:TDxLabel;
    DSHeroTitlePageDown:TDxImageButton;
    DSHeroTitlePageUp:TDxImageButton;
    DHeroStateTabSheet6:TDxTabSheet;
    DHeroStMagicPage:TDxLabel;
    DHeroStPageDown:TDxImageButton;
    DHeroStPageUp:TDxImageButton;
    DHeroStateMagic:TDxScrollBox;
    DHeroStMagBack1:TDxImageForm;
    DHeroStMag1:TDxImageButton;
    DHeroStMagBtn1:TDxImageButton;
    DStHeroMagExpIcon1:TDxImageButton;
    DStHeroMagExpText1:TDxLabel;
    DStHeroMagLblName1:TDxLabel;
    DStHeroMagLvIcon1:TDxImageButton;
    DStHeroMagLvText1:TDxLabel;
    DHeroStMagBack2:TDxImageForm;
    DHeroStMag2:TDxImageButton;
    DHeroStMagBtn2:TDxImageButton;
    DHeroStMagBack3:TDxImageForm;
    DHeroStMag3:TDxImageButton;
    DHeroStMagBtn3:TDxImageButton;
    DHeroStMagBack4:TDxImageForm;
    DHeroStMag4:TDxImageButton;
    DHeroStMagBtn4:TDxImageButton;
    DHeroStMagBack5:TDxImageForm;
    DHeroStMag5:TDxImageButton;
    DHeroStMagBtn5:TDxImageButton;
    DHeroStMagBack6:TDxImageForm;
    DHeroStMag6:TDxImageButton;
    DHeroStMagBtn6:TDxImageButton;
    HeroMainTabSheet2:TDxTabSheet;
    DHeroNGPageControl:TDxPageControl;
    DHeroNGStateTabSheet1:TDxTabSheet;
    DHeroStateNGInfo:TDxImageForm;
    DHeroStateHJAntiPoisonLabel:TDxLabel;
    DHeroStateHJAntiPoisonValue:TDxLabel;
    DHeroStateNGAntiPoisonLabel:TDxLabel;
    DHeroStateNGAntiPoisonValue:TDxLabel;
    DHeroStateNGExpLabel:TDxLabel;
    DHeroStateNGExpValue:TDxLabel;
    DHeroStateNGHitLabel:TDxLabel;
    DHeroStateNGHitValue:TDxLabel;
    DHeroStateNGLabel:TDxLabel;
    DHeroStateNGLevelLabel:TDxLabel;
    DHeroStateNGLevelValue:TDxLabel;
    DHeroStateNGMaxExpLabel:TDxLabel;
    DHeroStateNGMaxExpValue:TDxLabel;
    DHeroStateNGRecoverLabel:TDxLabel;
    DHeroStateNGRecoverValue:TDxLabel;
    DHeroStateNGValue:TDxLabel;
    DHeroNGStateTabSheet2:TDxTabSheet;
    DHeroStNGMagicPage:TDxLabel;
    DHeroStNGPageDown:TDxImageButton;
    DHeroStNGPageUp:TDxImageButton;
    DHeroStateNGMagic:TDxScrollBox;
    DHeroStNGMagBack1:TDxImageForm;
    DHeroStNGExpIcon1:TDxImageButton;
    DHeroStNGExpText1:TDxLabel;
    DHeroStNGLblName1:TDxLabel;
    DHeroStNGLvIcon1:TDxImageButton;
    DHeroStNGLvText1:TDxLabel;
    DHeroStNGMag1:TDxImageButton;
    DHeroStNGMagBack2:TDxImageForm;
    DHeroStNGMag2:TDxImageButton;
    DHeroStNGMagBack3:TDxImageForm;
    DHeroStNGMag3:TDxImageButton;
    DHeroStNGMagBack4:TDxImageForm;
    DHeroStNGMag4:TDxImageButton;
    DHeroStNGMagBack5:TDxImageForm;
    DHeroStNGMag5:TDxImageButton;
    DHeroStNGMagBack6:TDxImageForm;
    DHeroStNGMag6:TDxImageButton;
    DHeroNGStateTabSheet3:TDxTabSheet;
    DHeroStateNGMeridians:TDxSexPanel;
    DHeroBotMeridians0:TDxImageButton;
    DHeroBotMeridians1:TDxImageButton;
    DHeroBotMeridians2:TDxImageButton;
    DHeroBotMeridians3:TDxImageButton;
    DHeroBotMeridians4:TDxImageButton;
    DHeroMeridiansPageControl:TDxPageControl;
    DHeroMeridiansTabSheet0:TDxTabSheet;
    DHeroMeridiansLine0:TDxImageFormShape;
    DHeroBotAcupoints0_0:TDxImageButton;
    DHeroBotAcupoints0_1:TDxImageButton;
    DHeroBotAcupoints0_2:TDxImageButton;
    DHeroBotAcupoints0_3:TDxImageButton;
    DHeroBotAcupoints0_4:TDxImageButton;
    DHeroLabelAcupoints0_0:TDxLabel;
    DHeroLabelAcupoints0_1:TDxLabel;
    DHeroLabelAcupoints0_2:TDxLabel;
    DHeroLabelAcupoints0_3:TDxLabel;
    DHeroLabelAcupoints0_4:TDxLabel;
    DHeroMeridiansTabSheet1:TDxTabSheet;
    DHeroMeridiansLine1:TDxImageFormShape;
    DHeroBotAcupoints1_0:TDxImageButton;
    DHeroBotAcupoints1_1:TDxImageButton;
    DHeroBotAcupoints1_2:TDxImageButton;
    DHeroBotAcupoints1_3:TDxImageButton;
    DHeroBotAcupoints1_4:TDxImageButton;
    DHeroLabelAcupoints1_0:TDxLabel;
    DHeroLabelAcupoints1_1:TDxLabel;
    DHeroLabelAcupoints1_2:TDxLabel;
    DHeroLabelAcupoints1_3:TDxLabel;
    DHeroLabelAcupoints1_4:TDxLabel;
    DHeroLabelMeridiansStatus1:TDxLabel;
    DHeroMeridiansTabSheet2:TDxTabSheet;
    DHeroMeridiansLine2:TDxImageFormShape;
    DHeroBotAcupoints2_0:TDxImageButton;
    DHeroBotAcupoints2_1:TDxImageButton;
    DHeroBotAcupoints2_2:TDxImageButton;
    DHeroBotAcupoints2_3:TDxImageButton;
    DHeroBotAcupoints2_4:TDxImageButton;
    DHeroLabelAcupoints2_0:TDxLabel;
    DHeroLabelAcupoints2_1:TDxLabel;
    DHeroLabelAcupoints2_2:TDxLabel;
    DHeroLabelAcupoints2_3:TDxLabel;
    DHeroLabelAcupoints2_4:TDxLabel;
    DHeroLabelMeridiansStatus2:TDxLabel;
    DHeroMeridiansTabSheet3:TDxTabSheet;
    DHeroMeridiansLine3:TDxImageFormShape;
    DHeroBotAcupoints3_0:TDxImageButton;
    DHeroBotAcupoints3_1:TDxImageButton;
    DHeroBotAcupoints3_2:TDxImageButton;
    DHeroBotAcupoints3_3:TDxImageButton;
    DHeroBotAcupoints3_4:TDxImageButton;
    DHeroLabelAcupoints3_0:TDxLabel;
    DHeroLabelAcupoints3_1:TDxLabel;
    DHeroLabelAcupoints3_2:TDxLabel;
    DHeroLabelAcupoints3_3:TDxLabel;
    DHeroLabelAcupoints3_4:TDxLabel;
    DHeroLabelMeridiansStatus3:TDxLabel;
    DHeroMeridiansTabSheet4:TDxTabSheet;
    DHeroMeridiansLine4:TDxImageFormShape;
    DHeroBotAcupoints4_0:TDxImageButton;
    DHeroBotAcupoints4_1:TDxImageButton;
    DHeroBotAcupoints4_2:TDxImageButton;
    DHeroBotAcupoints4_3:TDxImageButton;
    DHeroBotAcupoints4_4:TDxImageButton;
    DHeroLabelAcupoints4_0:TDxLabel;
    DHeroLabelAcupoints4_1:TDxLabel;
    DHeroLabelAcupoints4_2:TDxLabel;
    DHeroLabelAcupoints4_3:TDxLabel;
    DHeroLabelAcupoints4_4:TDxLabel;
    DHeroLabelMeridiansStatus4:TDxLabel;
    DHeroTrainingMeridian:TDxImageButton;
    DHeroNGStateTabSheet4:TDxTabSheet;
    DHeroStateNGContinuousMagic:TDxImageForm;
    DHeroContinuousMagicList:TDxScrollBox;
    DHeroStLJMagBack1:TDxImageForm;
    DHeroStLJExpIcon1:TDxImageButton;
    DHeroStLJExpText1:TDxLabel;
    DHeroStLJLblName1:TDxLabel;
    DHeroStLJLvIcon1:TDxImageButton;
    DHeroStLJLvText1:TDxLabel;
    DHeroStLJMag1:TDxImageButton;
    DHeroStLJMagBack2:TDxImageForm;
    DHeroStLJMag2:TDxImageButton;
    DHeroStLJMagBack3:TDxImageForm;
    DHeroStLJMag3:TDxImageButton;
    DHeroStLJMagBack4:TDxImageForm;
    DHeroStLJMag4:TDxImageButton;
    DHeroSMB1:TDxImageButton;
    DHeroSMB2:TDxImageButton;
    DHeroSMB3:TDxImageButton;
    DHeroSMB4:TDxImageButton;
    HeroMainTabSheet3:TDxTabSheet;
    DHeroXFPageControl:TDxPageControl;
    HeroContinuousMagicMenu:TDxPopupMenu;
    DGodBlessDlgUS1:TDxImageForm;
    DGodBless10US1:TDxImageButton;
    DGodBless11US1:TDxImageButton;
    DGodBless12US1:TDxImageButton;
    DGodBless1US1:TDxImageButton;
    DGodBless2US1:TDxImageButton;
    DGodBless3US1:TDxImageButton;
    DGodBless4US1:TDxImageButton;
    DGodBless5US1:TDxImageButton;
    DGodBless6US1:TDxImageButton;
    DGodBless7US1:TDxImageButton;
    DGodBless8US1:TDxImageButton;
    DGodBless9US1:TDxImageButton;
    DGodBlessCloseUS1:TDxImageButton;
    DGodBlessInfoUS1:TDxImageButton;
    DUSGodBlessCusBtn1:TDxImageButton;
    DUSGodBlessCusBtn2:TDxImageButton;
    DUSGodBlessCusBtn3:TDxImageButton;
    DJewelryBoxDlgUS1:TDxImageForm;
    DJewelry1US1:TDxImageButton;
    DJewelry2US1:TDxImageButton;
    DJewelry3US1:TDxImageButton;
    DJewelry4US1:TDxImageButton;
    DJewelry5US1:TDxImageButton;
    DJewelry6US1:TDxImageButton;
    DJewelryCloseUS1:TDxImageButton;
    DUSJewelryBoxCusBtn1:TDxImageButton;
    DUSJewelryBoxCusBtn2:TDxImageButton;
    DUSJewelryBoxCusBtn3:TDxImageButton;
    DUserState1:TDxImageForm;
    DCloseUS1:TDxImageButton;
    DUserState1BasicPageControl:TDxPageControl;
    DUserState1StateTabSheet1:TDxTabSheet;
    DUserState1StateBasic:TDxSexPanel;
    DArmRingLUS1:TDxImageButton;
    DArmRingRUS1:TDxImageButton;
    DBeltUS1:TDxImageButton;
    DBootsUS1:TDxImageButton;
    DBujukUS1:TDxImageButton;
    DCharmUS1:TDxImageButton;
    DDressUS1:TDxImageButton;
    DDrumUS1:TDxImageButton;
    DGodBlessUS1:TDxImageButton;
    DHelmetUS1:TDxImageButton;
    DHorseUS1:TDxImageButton;
    DJadeUS1:TDxImageButton;
    DJewelryBoxUS1:TDxImageButton;
    DLightUS1:TDxImageButton;
    DNecklaceUS1:TDxImageButton;
    DRingLUS1:TDxImageButton;
    DRingRUS1:TDxImageButton;
    DShieldUS1:TDxImageButton;
    DUSCustomButton1:TDxImageButton;
    DUSCustomButton2:TDxImageButton;
    DUSCustomButton3:TDxImageButton;
    DUserState1LabelDStateWinRankName:TDxLabel;
    DWeaponUS1:TDxImageButton;
    DUserState1StateTabSheet2:TDxTabSheet;
    DUserState1StateFashion:TDxSexPanel;
    DFashionArmRingLUS1:TDxImageButton;
    DFashionArmRingRUS1:TDxImageButton;
    DFashionBeltUS1:TDxImageButton;
    DFashionBootsUS1:TDxImageButton;
    DFashionDressUS1:TDxImageButton;
    DFashionDrumUS1:TDxImageButton;
    DFashionHelmetUS1:TDxImageButton;
    DFashionLightUS1:TDxImageButton;
    DFashionNecklaceUS1:TDxImageButton;
    DFashionRingLUS1:TDxImageButton;
    DFashionRingRUS1:TDxImageButton;
    DFashionWeaponUS1:TDxImageButton;
    DUSFashionCustomButton1:TDxImageButton;
    DUSFashionCustomButton2:TDxImageButton;
    DUSFashionCustomButton3:TDxImageButton;
    DUserState1StateTabSheet5:TDxTabSheet;
    DUserState1StateTitle:TDxImageForm;
    DUSTitleActive:TDxImageButton;
    DUSTitleButton1:TDxImageButton;
    DUSTitleButton2:TDxImageButton;
    DUSTitleButton3:TDxImageButton;
    DUSTitleButton4:TDxImageButton;
    DUSTitleButton5:TDxImageButton;
    DUSTitleButton6:TDxImageButton;
    DUSTitleCustomBtn1:TDxImageButton;
    DUSTitleCustomBtn2:TDxImageButton;
    DUSTitleCustomBtn3:TDxImageButton;
    DUSTitleItemDesc:TDxImageButton;
    DUSTitleName1:TDxLabel;
    DUSTitleName2:TDxLabel;
    DUSTitleName3:TDxLabel;
    DUSTitleName4:TDxLabel;
    DUSTitleName5:TDxLabel;
    DUSTitleName6:TDxLabel;
    DUSTitleNameActive:TDxLabel;
    DUSTitlePageDown:TDxImageButton;
    DUSTitlePageUp:TDxImageButton;
    DUserState1LabelDStateWinCharName:TDxLabel;
  private
    Initialized:Boolean;
    MagicIndex:Integer;
    MagicNGIndex:Integer;
    HeroMagicIndex:Integer;
    HeroMagicNGIndex:Integer;
    FengHaoIndex:Integer;
    USFengHaoIndex:Integer;
    HeroFengHaoIndex:Integer;

    FengHaoHintWindow:THintWindow;

    MeridiansImageIndexArray:array[0..4] of Integer;
    MeridiansFormArray:array[0..4] of TDxImageFormShape;
    HeroMeridiansFormArray:array[0..4] of TDxImageFormShape;

    AcupointArray:array[0..4, 0..4] of TDxImageButton;
    HeroAcupointArray:array[0..4, 0..4] of TDxImageButton;

    FJewelryBoxUpImageIndex:Integer;
    FJewelryBoxDownImageIndex:Integer;

    FHeroJewelryBoxUpImageIndex:Integer;
    FHeroJewelryBoxDownImageIndex:Integer;

    FLableCaption_BigGold:string;
    FLabelCaption_AC:string;
    FLabelCaption_MAC:string;
    FLabelCaption_DC:string;
    FLabelCaption_MC:string;
    FLabelCaption_SC:string;
    FLabelCaption_Weight:string;
    FLabelCaption_WearWeight:string;
    FLabelCaption_HandWeight:string;
    FLabelCaption_AntiMagic:string;
    FLabelCaption_AntiPoison:string;
    FLabelCaption_HealthRecover:string;
    FLabelCaption_SpellRecover:string;
    FLabelCaption_HitSpeed:string;
    FLabelCaption_HitPoint:string;
    FLabelCaption_SpeedPoint:string;
    FLabelCaption_PoisonRecover:string;

    FLabelCaption_HeroAC:string;
    FLabelCaption_HeroMAC:string;
    FLabelCaption_HeroDC:string;
    FLabelCaption_HeroMC:string;
    FLabelCaption_HeroSC:string;
    FLabelCaption_HeroWeight:string;
    FLabelCaption_HeroWearWeight:string;
    FLabelCaption_HeroHandWeight:string;
    FLabelCaption_HeroAntiMagic:string;
    FLabelCaption_HeroAntiPoison:string;
    FLabelCaption_HeroHealthRecover:string;
    FLabelCaption_HeroSpellRecover:string;
    FLabelCaption_HeroHitSpeed:string;
    FLabelCaption_HeroHitPoint:string;
    FLabelCaption_HeroSpeedPoint:string;
    FLabelCaption_HeroPoisonRecover:string;

    FSelectGamePetIndex:Integer;
    FPetShowIndex1:Integer;
    FPetShowTick1:LongWord;

    FPetShowIndex2:Integer;
    FPetShowTick2:LongWord;

    FStateWeightLabelColor:TSaveUIColor;
    FStateWearWeightLabelColor:TSaveUIColor;
    FStateHandWeightLabelColor:TSaveUIColor;

    procedure InitSelf;
    procedure InitHero;
    procedure InitUserState1;

    procedure DRecallDeputyHeroClick(Sender:TObject; X, Y:Integer); stdcall; // 召唤副将英雄
    {------------------------------------------------------------------------------}
    procedure DStateWinUpDate(Sender:TObject); stdcall;
    procedure OnDStateWinBringToFront(Sender:TObject); stdcall;
    procedure DBotAcupointsClick(Sender:TObject; X, Y:Integer); stdcall; // 点击穴位
    procedure DBotMeridiansClick(Sender:TObject; X, Y:Integer); stdcall; // 选择经络
    procedure DMeridiansLineDirectPaint(Sender:TObject); stdcall;
    procedure DBotAcupointsMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall; // 穴位鼠标移动事件

    procedure DTrainingMeridianClick(Sender:TObject; X, Y:Integer); stdcall; // 修炼经络

    procedure DStateWinDirectPaint(Sender:TObject); stdcall;

    procedure DStateWinFashionDirectPaint(Sender:TObject); stdcall;

    procedure DSWWeaponMouseMove(Sender:TObject; Shift:TShiftState;
      X, Y:Integer); stdcall;

    procedure DCloseStateClick(Sender:TObject; X, Y:Integer); stdcall;
    {------------------------------------------------------------------------------}
    procedure DUserState1DirectPaint(Sender:TObject); stdcall;
    procedure DUserState1FashionDirectPaint(Sender:TObject); stdcall;

    procedure DWeaponUS1MouseMove(Sender:TObject; Shift:TShiftState; X,
      Y:Integer); stdcall;
    procedure DCloseUS1Click(Sender:TObject; X, Y:Integer); stdcall;

    {------------------------------------------------------------------------------}
    procedure DHeroCloseStateClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DHeroStateWinUpDate(Sender:TObject); stdcall;
    procedure DHeroBotAcupointsClick(Sender:TObject; X, Y:Integer); stdcall; // 点击穴位
    procedure DHeroBotMeridiansClick(Sender:TObject; X, Y:Integer); stdcall; // 选择经络
    procedure DHeroMeridiansLineDirectPaint(Sender:TObject); stdcall; // 绘制穴位
    procedure DHeroBotAcupointsMouseMove(Sender:TObject; Shift:TShiftState; X, // 穴位鼠标移动事件
      Y:Integer); stdcall;

    procedure DHeroStateWinDirectPaint(Sender:TObject); stdcall;
    procedure DHeroStateWinFashionDirectPaint(Sender:TObject); stdcall;

    procedure DHeroSWWeaponMouseMove(Sender:TObject; Shift:TShiftState;
      X, Y:Integer); stdcall;
    procedure DHeroSWLightDirectPaint(Sender:TObject); stdcall;
    procedure DHeroSWWeaponClick(Sender:TObject; X, Y:Integer); stdcall;

    procedure DHeroTrainingMeridianClick(Sender:TObject; X, Y:Integer); stdcall; // 修炼经脉按钮
    {------------------------------------------------------------------------------}
    procedure DStateMagicDirectPaint(Sender:TObject); stdcall;
    procedure DStMag1DirectPaint(Sender:TObject); stdcall;
    procedure DStMag1Click(Sender:TObject; X, Y:Integer); stdcall;
    procedure DStMag1MouseDown(Sender:TObject; Button:TMouseButton; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DStMag1MouseUp(Sender:TObject; Button:TMouseButton; Shift:TShiftState; X, Y:Integer); stdcall;

    procedure DStPageUpClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DStMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure MagicPageChange;

    {------------------------------------------------------------------------------}
    procedure DStateNGMagicDirectPaint(Sender:TObject); stdcall;
    procedure DStNGMag1DirectPaint(Sender:TObject); stdcall;
    procedure DStNGMag1Click(Sender:TObject; X, Y:Integer); stdcall;
    procedure DStNGPageUpClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DStNGMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure NGMagicPageChange;
    {------------------------------------------------------------------------------}
    procedure DStateLJMagicDirectPaint(Sender:TObject); stdcall;
    procedure DStLJMag1DirectPaint(Sender:TObject); stdcall;
    procedure DStLJMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DSMB1DirectPaint(Sender:TObject); stdcall;
    procedure DSMB1Click(Sender:TObject; X, Y:Integer); stdcall;
    procedure ContinuousMagicMenuClick(Sender:TObject; X, Y:Integer); stdcall;
    {------------------------------------------------------------------------------}
    procedure DHeroStateMagicDirectPaint(Sender:TObject); stdcall;
    procedure DHeroStMag1DirectPaint(Sender:TObject); stdcall;
    procedure DHeroStMag1Click(Sender:TObject; X, Y:Integer); stdcall;
    procedure DHeroStPageUpClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DHeroStMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure HeroMagicPageChange;
    {------------------------------------------------------------------------------}
    procedure DHeroStateNGMagicDirectPaint(Sender:TObject); stdcall;
    procedure DHeroStNGMag1DirectPaint(Sender:TObject); stdcall;
    procedure DHeroStNGMag1Click(Sender:TObject; X, Y:Integer); stdcall;
    procedure DHeroStNGPageUpClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DHeroStNGMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure HeroNGMagicPageChange;
    {------------------------------------------------------------------------------}
    procedure DHeroStateLJMagicDirectPaint(Sender:TObject); stdcall;
    procedure DHeroStLJMag1DirectPaint(Sender:TObject); stdcall;
    procedure DHeroStLJMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DHeroSMB1DirectPaint(Sender:TObject); stdcall;
    procedure DHeroSMB1Click(Sender:TObject; X, Y:Integer); stdcall;
    procedure HeroContinuousMagicMenuClick(Sender:TObject; X, Y:Integer); stdcall;

    {------------------------------------------------------------------------------}
    procedure DSMBShortcutClick(Sender:TObject; X, Y:Integer); stdcall;

    procedure LabelDStateWinCharNameClick(Sender:TObject; X, Y:Integer); stdcall;

    procedure DSWJewelryBoxClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSWJewelryBoxMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;

    procedure DSWJewelryBoxItemClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSWJewelryBoxItemMouseMove(Sender:TObject; Shift:TShiftState;
      X, Y:Integer); stdcall;
    procedure DSWJewelryBoxItemPaint(Sender:TObject); stdcall;

    procedure DHeroSWJewelryBoxItemClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DHeroSWJewelryBoxItemMouseMove(Sender:TObject; Shift:TShiftState;
      X, Y:Integer); stdcall;
    procedure DHeroSWJewelryBoxItemPaint(Sender:TObject); stdcall;

    procedure DJewelryBoxItemUS1MouseMove(Sender:TObject; Shift:TShiftState;
      X, Y:Integer); stdcall;
    procedure DJewelryBoxItemPaintUS1(Sender:TObject); stdcall;

    procedure DSWGodBlessClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSWGodBlessItemClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSWGodBlessItemMouseMove(Sender:TObject; Shift:TShiftState;
      X, Y:Integer); stdcall;
    procedure DSWGodBlessItemPaint(Sender:TObject); stdcall;

    procedure DSWGodBlessUpgradeClick(Sender:TObject; X, Y:Integer); stdcall;

    procedure DHeroSWGodBlessItemClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DHeroSWGodBlessItemMouseMove(Sender:TObject; Shift:TShiftState;
      X, Y:Integer); stdcall;
    procedure DHeroSWGodBlessItemPaint(Sender:TObject); stdcall;

    procedure DGodBlessItemUS1MouseMove(Sender:TObject; Shift:TShiftState;
      X, Y:Integer); stdcall;
    procedure DGodBlessItemUS1Paint(Sender:TObject); stdcall;

    procedure DSWShowFashionClick(Sender:TObject; X, Y:Integer); stdcall;

    // 称号相关
    procedure DStateTitleWinMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;

    procedure DSTitleItemDescOnPaint(Sender:TObject); stdcall;

    procedure DSTitleActiveClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSTitleActiveMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DSTitleActiveDirectPaint(Sender:TObject); stdcall;

    procedure DSTitleButtonClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSTitleButtonMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DSTitleButtonDirectPaint(Sender:TObject); stdcall;

    procedure DSTitleNameActiveDirectPaint(Sender:TObject); stdcall;
    procedure DSTitleNameDirectPaint(Sender:TObject); stdcall;
    procedure DSTitlePageClick(Sender:TObject; X, Y:Integer); stdcall;

    procedure DUSTitleItemDescOnPaint(Sender:TObject); stdcall;
    procedure DUSTitleActiveDirectPaint(Sender:TObject); stdcall;
    procedure DUSTitleAcitveMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DUSTitleButtonMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DUSTitleButtonDirectPaint(Sender:TObject); stdcall;
    procedure DUSTitleNameActiveDirectPaint(Sender:TObject); stdcall;
    procedure DUSTitleNameDirectPaint(Sender:TObject); stdcall;
    procedure DUSTitlePageClick(Sender:TObject; X, Y:Integer); stdcall;

    procedure DSHeroTitleActiveClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSHeroTitleActiveMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DSHeroTitleActiveDirectPaint(Sender:TObject); stdcall;

    procedure DSHeroTitleButtonClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSHeroTitleButtonMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DSHeroTitleButtonDirectPaint(Sender:TObject); stdcall;

    procedure DSHeroTitleNameActiveDirectPaint(Sender:TObject); stdcall;
    procedure DSHeroTitleNameDirectPaint(Sender:TObject); stdcall;
    procedure DSHeroTitlePageClick(Sender:TObject; X, Y:Integer); stdcall;

    procedure UpdateSelectPetsInfo;

    //procedure DSWPetsDlgMouseMove(Sender: TObject; Shift: TShiftState; X, Y: Integer); stdcall; //HZQ移动到下面Protected中

    procedure DSWPetsClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSWPetsCloseClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSWPetsButtonClick(Sender:TObject; X, Y:Integer); stdcall;

    procedure DSWPetsShowStopPaint(Sender:TObject); stdcall;

    procedure DSWPetsListViewItemPaint(Sender:TObject; ACol, ARow:Integer; ARect:TRect; ViewItem:Pointer; var PaintOverride:Boolean); stdcall;
    procedure DSWPetsListViewItemClick(Sender:TObject; ARow, ACol:Integer; ListItem:TObject; ViewItem:Pointer); stdcall;

    {
    procedure DSWPetsItemMouseMove(Sender: TObject; Shift: TShiftState; X, Y: Integer); stdcall;
    procedure DSWPetsItemClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure DSWPetsItemPaint(Sender: TObject); stdcall;
    }

    procedure DSWPetsMagicMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
    procedure DSWPetsMagicClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSWPetsMagicPaint(Sender:TObject); stdcall;

    procedure DSWPetsBagCloseClick(Sender:TObject; X, Y:Integer); stdcall;

    procedure DSWPetsGridGridSelect(Sender:TObject; ACol, ARow:Integer;
      Button:TMouseButton; Shift:TShiftState); stdcall;
    procedure DSWPetsGridGridPaint(Sender:TObject; ACol, ARow:Integer;
      Rect:TRect; State:TGridDrawState); stdcall;
    procedure DSWPetsGridDblClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DSWPetsGridGridMouseMove(Sender:TObject; ACol, ARow:Integer;
      Shift:TShiftState); stdcall;

    procedure DControlMouseMoveShowHint(Sender:TObject; Shift:TShiftState; X,
      Y:Integer); stdcall;

    procedure OnDStateWinExClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DStateWinMove(Sender:TObject); stdcall;
    procedure RefreshDStateWinExInfo;
  protected
    procedure DSWPetsDlgMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer); stdcall;
  public
    constructor Create();
    destructor Destroy; override;
    procedure Initialize;
    procedure Close;
    procedure UpDate;
    procedure LoadFromStream(MemoryStream:TMemoryStream);
    function MakeControlAddressList:THashedStringlist;

    procedure RefreshGamePetList;

    procedure OpenStateWinDlg;
    procedure CloseStateWinDlg;

    procedure OpenUserState1Dlg;
    procedure CloseUserState1Dlg;

    procedure OpenMyStatus;
    procedure OpenMyMagic;
    procedure OpenUserState();

    procedure OpenGamePetDlg();

    procedure MySelfAbilChange;

    procedure MyHeroAbilChange;

    procedure OpenHeroStateWinDlg;
    procedure CloseHeroStateWinDlg;

    procedure SetMeridiansLevel(Meridian, Level:Integer); // 设置经脉等级 多少重
    procedure SetHeroMeridiansLevel(Meridian, Level:Integer); // 设置经脉等级 多少重

    procedure RefreshMySelfMagicList;
    procedure RefreshMyHeroMagicList;

    procedure SetDeputyHeroJob(btJob:Byte);

    procedure SetMySelfActivePageIndexCount;
    procedure SetMyHeroActivePageIndexCount;

    procedure ChangeMySelfMeridiansctivePage;
    procedure ChangeMyHeroMeridiansctivePage;

    procedure SetMySelfLastContinuousButton;
    procedure SetMyHeroLastContinuousButton;

    procedure SetMySelfJewelryBoxButton;
    procedure SetMyHeroJewelryBoxButton;

    procedure RefreshUpgradeButtons;
    procedure RefreshHeroUpgradeButtons;

    procedure RefreshFashionJewelryInfo;
    procedure RefreshMyFashionJewelryInfo;
    procedure RefreshHeroFashionJewelryInfo;
    procedure RefreshUserFashionJewelryInfo;
  end;

implementation

uses
  Math,
  ClMain,
  MShare,
  FState,
  LoadDxControlEx,
  SDK,
  SerialWindowsDlg,
  MirConfigDlg,
  GameConfigDlg,
  JSYConfigDlg,
  ConfigShare;

constructor TStateWindows.Create();
begin
  inherited;
  MagicIndex := 0;
  MagicNGIndex := 0;
  HeroMagicIndex := 0;
  HeroMagicNGIndex := 0;
  FengHaoIndex := 0;
  USFengHaoIndex := 0;
  HeroFengHaoIndex := 0;
  Initialized := False;
  FengHaoHintWindow := THintWindow.Create;

  FSelectGamePetIndex := 0;
  FPetShowIndex1 := 0;
  FPetShowTick1 := MyGetTickCount;

  FPetShowIndex2 := 0;
  FPetShowTick2 := MyGetTickCount;
end;

destructor TStateWindows.Destroy;
begin
  FengHaoHintWindow.Free;
  inherited;
end;

function TStateWindows.MakeControlAddressList:THashedStringlist;
begin
    Result := THashedStringList.Create;
    Result.AddObject('ContinuousMagicMenu', Pointer(@ContinuousMagicMenu));
    Result.AddObject('DSWGodBlessDlg', Pointer(@DSWGodBlessDlg));
    Result.AddObject('DSWGodBless1', Pointer(@DSWGodBless1));
    Result.AddObject('DSWGodBless10', Pointer(@DSWGodBless10));
    Result.AddObject('DSWGodBless11', Pointer(@DSWGodBless11));
    Result.AddObject('DSWGodBless12', Pointer(@DSWGodBless12));
    Result.AddObject('DSWGodBless2', Pointer(@DSWGodBless2));
    Result.AddObject('DSWGodBless3', Pointer(@DSWGodBless3));
    Result.AddObject('DSWGodBless4', Pointer(@DSWGodBless4));
    Result.AddObject('DSWGodBless5', Pointer(@DSWGodBless5));
    Result.AddObject('DSWGodBless6', Pointer(@DSWGodBless6));
    Result.AddObject('DSWGodBless7', Pointer(@DSWGodBless7));
    Result.AddObject('DSWGodBless8', Pointer(@DSWGodBless8));
    Result.AddObject('DSWGodBless9', Pointer(@DSWGodBless9));
    Result.AddObject('DSWGodBlessClose', Pointer(@DSWGodBlessClose));
    Result.AddObject('DSWGodBlessCusBtn1', Pointer(@DSWGodBlessCusBtn1));
    Result.AddObject('DSWGodBlessCusBtn2', Pointer(@DSWGodBlessCusBtn2));
    Result.AddObject('DSWGodBlessCusBtn3', Pointer(@DSWGodBlessCusBtn3));
    Result.AddObject('DSWGodBlessInfo', Pointer(@DSWGodBlessInfo));
    Result.AddObject('DSWJewelryBoxDlg', Pointer(@DSWJewelryBoxDlg));
    Result.AddObject('DSWJewelry1', Pointer(@DSWJewelry1));
    Result.AddObject('DSWJewelry2', Pointer(@DSWJewelry2));
    Result.AddObject('DSWJewelry3', Pointer(@DSWJewelry3));
    Result.AddObject('DSWJewelry4', Pointer(@DSWJewelry4));
    Result.AddObject('DSWJewelry5', Pointer(@DSWJewelry5));
    Result.AddObject('DSWJewelry6', Pointer(@DSWJewelry6));
    Result.AddObject('DSWJewelryBoxCusBtn1', Pointer(@DSWJewelryBoxCusBtn1));
    Result.AddObject('DSWJewelryBoxCusBtn2', Pointer(@DSWJewelryBoxCusBtn2));
    Result.AddObject('DSWJewelryBoxCusBtn3', Pointer(@DSWJewelryBoxCusBtn3));
    Result.AddObject('DSWJewelryClose', Pointer(@DSWJewelryClose));
    Result.AddObject('DSWPetsBagDlg', Pointer(@DSWPetsBagDlg));
    Result.AddObject('DSWPetsBagClose', Pointer(@DSWPetsBagClose));
    Result.AddObject('DSWPetsGrid', Pointer(@DSWPetsGrid));
    Result.AddObject('DSWPetsDlg', Pointer(@DSWPetsDlg));
    Result.AddObject('DSWPetsAC', Pointer(@DSWPetsAC));
    Result.AddObject('DSWPetsACTitle', Pointer(@DSWPetsACTitle));
    Result.AddObject('DSWPetsClose', Pointer(@DSWPetsClose));
    Result.AddObject('DSWPetsCusBtn1', Pointer(@DSWPetsCusBtn1));
    Result.AddObject('DSWPetsCusBtn2', Pointer(@DSWPetsCusBtn2));
    Result.AddObject('DSWPetsCusBtn3', Pointer(@DSWPetsCusBtn3));
    Result.AddObject('DSWPetsCusBtn4', Pointer(@DSWPetsCusBtn4));
    Result.AddObject('DSWPetsCusBtn5', Pointer(@DSWPetsCusBtn5));
    Result.AddObject('DSWPetsCusBtn6', Pointer(@DSWPetsCusBtn6));
    Result.AddObject('DSWPetsCusBtn7', Pointer(@DSWPetsCusBtn7));
    Result.AddObject('DSWPetsCusBtn8', Pointer(@DSWPetsCusBtn8));
    Result.AddObject('DSWPetsCusBtn9', Pointer(@DSWPetsCusBtn9));
    Result.AddObject('DSWPetsDC', Pointer(@DSWPetsDC));
    Result.AddObject('DSWPetsDCTitle', Pointer(@DSWPetsDCTitle));
    Result.AddObject('DSWPetsExp', Pointer(@DSWPetsExp));
    Result.AddObject('DSWPetsExpTitle', Pointer(@DSWPetsExpTitle));
    Result.AddObject('DSWPetsFree', Pointer(@DSWPetsFree));
    Result.AddObject('DSWPetsHP', Pointer(@DSWPetsHP));
    Result.AddObject('DSWPetsHPTitle', Pointer(@DSWPetsHPTitle));
    Result.AddObject('DSWPetsLevel', Pointer(@DSWPetsLevel));
    Result.AddObject('DSWPetsLevelTitle', Pointer(@DSWPetsLevelTitle));
    Result.AddObject('DSWPetsList', Pointer(@DSWPetsList));
    Result.AddObject('DSWPetsMAC', Pointer(@DSWPetsMAC));
    Result.AddObject('DSWPetsMACTitle', Pointer(@DSWPetsMACTitle));
    Result.AddObject('DSWPetsMC', Pointer(@DSWPetsMC));
    Result.AddObject('DSWPetsMCTitle', Pointer(@DSWPetsMCTitle));
    Result.AddObject('DSWPetsMagic1', Pointer(@DSWPetsMagic1));
    Result.AddObject('DSWPetsMagic2', Pointer(@DSWPetsMagic2));
    Result.AddObject('DSWPetsMagic3', Pointer(@DSWPetsMagic3));
    Result.AddObject('DSWPetsMagic4', Pointer(@DSWPetsMagic4));
    Result.AddObject('DSWPetsMagic5', Pointer(@DSWPetsMagic5));
    Result.AddObject('DSWPetsMagic6', Pointer(@DSWPetsMagic6));
    Result.AddObject('DSWPetsMagic7', Pointer(@DSWPetsMagic7));
    Result.AddObject('DSWPetsMagic8', Pointer(@DSWPetsMagic8));
    Result.AddObject('DSWPetsName', Pointer(@DSWPetsName));
    Result.AddObject('DSWPetsNameTitle', Pointer(@DSWPetsNameTitle));
    Result.AddObject('DSWPetsRecall', Pointer(@DSWPetsRecall));
    Result.AddObject('DSWPetsRetake', Pointer(@DSWPetsRetake));
    Result.AddObject('DSWPetsSC', Pointer(@DSWPetsSC));
    Result.AddObject('DSWPetsSCTitle', Pointer(@DSWPetsSCTitle));
    Result.AddObject('DSWPetsShow', Pointer(@DSWPetsShow));
    Result.AddObject('DSWPetsTakeBag', Pointer(@DSWPetsTakeBag));
    Result.AddObject('DSWPetsViewBag', Pointer(@DSWPetsViewBag));
    Result.AddObject('DStateWin', Pointer(@DStateWin));
    Result.AddObject('DCloseState', Pointer(@DCloseState));
    Result.AddObject('DStatePageControl', Pointer(@DStatePageControl));
    Result.AddObject('MainTabSheet1', Pointer(@MainTabSheet1));
    Result.AddObject('DBasicPageControl', Pointer(@DBasicPageControl));
    Result.AddObject('DStateTabSheet1', Pointer(@DStateTabSheet1));
    Result.AddObject('DStateBasic', Pointer(@DStateBasic));
    Result.AddObject('DSWArmRingL', Pointer(@DSWArmRingL));
    Result.AddObject('DSWArmRingR', Pointer(@DSWArmRingR));
    Result.AddObject('DSWBelt', Pointer(@DSWBelt));
    Result.AddObject('DSWBoots', Pointer(@DSWBoots));
    Result.AddObject('DSWBujuk', Pointer(@DSWBujuk));
    Result.AddObject('DSWCharm', Pointer(@DSWCharm));
    Result.AddObject('DSWCustomButton1', Pointer(@DSWCustomButton1));
    Result.AddObject('DSWCustomButton2', Pointer(@DSWCustomButton2));
    Result.AddObject('DSWCustomButton3', Pointer(@DSWCustomButton3));
    Result.AddObject('DSWDress', Pointer(@DSWDress));
    Result.AddObject('DSWDrum', Pointer(@DSWDrum));
    Result.AddObject('DSWGodBless', Pointer(@DSWGodBless));
    Result.AddObject('DSWHelmet', Pointer(@DSWHelmet));
    Result.AddObject('DSWHorse', Pointer(@DSWHorse));
    Result.AddObject('DSWJade', Pointer(@DSWJade));
    Result.AddObject('DSWJewelryBox', Pointer(@DSWJewelryBox));
    Result.AddObject('DSWLight', Pointer(@DSWLight));
    Result.AddObject('DSWNecklace', Pointer(@DSWNecklace));
    Result.AddObject('DSWPets', Pointer(@DSWPets));
    Result.AddObject('DSWRingL', Pointer(@DSWRingL));
    Result.AddObject('DSWRingR', Pointer(@DSWRingR));
    Result.AddObject('DSWShield', Pointer(@DSWShield));
    Result.AddObject('DSWWeapon', Pointer(@DSWWeapon));
    Result.AddObject('LabelDStateWinRankName', Pointer(@LabelDStateWinRankName));
    Result.AddObject('DStateTabSheet2', Pointer(@DStateTabSheet2));
    Result.AddObject('DStateFashion', Pointer(@DStateFashion));
    Result.AddObject('DSWFashionArmRingL', Pointer(@DSWFashionArmRingL));
    Result.AddObject('DSWFashionArmRingR', Pointer(@DSWFashionArmRingR));
    Result.AddObject('DSWFashionBelt', Pointer(@DSWFashionBelt));
    Result.AddObject('DSWFashionBoots', Pointer(@DSWFashionBoots));
    Result.AddObject('DSWFashionCustomButton1', Pointer(@DSWFashionCustomButton1));
    Result.AddObject('DSWFashionCustomButton2', Pointer(@DSWFashionCustomButton2));
    Result.AddObject('DSWFashionCustomButton3', Pointer(@DSWFashionCustomButton3));
    Result.AddObject('DSWFashionDress', Pointer(@DSWFashionDress));
    Result.AddObject('DSWFashionDrum', Pointer(@DSWFashionDrum));
    Result.AddObject('DSWFashionHelmet', Pointer(@DSWFashionHelmet));
    Result.AddObject('DSWFashionLight', Pointer(@DSWFashionLight));
    Result.AddObject('DSWFashionNecklace', Pointer(@DSWFashionNecklace));
    Result.AddObject('DSWFashionRingL', Pointer(@DSWFashionRingL));
    Result.AddObject('DSWFashionRingR', Pointer(@DSWFashionRingR));
    Result.AddObject('DSWFashionWeapon', Pointer(@DSWFashionWeapon));
    Result.AddObject('DSWShowFashion', Pointer(@DSWShowFashion));
    Result.AddObject('DStateTabSheet3', Pointer(@DStateTabSheet3));
    Result.AddObject('DStateInfo', Pointer(@DStateInfo));
    Result.AddObject('DStateAlcoholLabel', Pointer(@DStateAlcoholLabel));
    Result.AddObject('DStateBigGoldLabel', Pointer(@DStateBigGoldLabel));
    Result.AddObject('DStateCreditPointLabel', Pointer(@DStateCreditPointLabel));
    Result.AddObject('DStateCustomButton1', Pointer(@DStateCustomButton1));
    Result.AddObject('DStateCustomButton2', Pointer(@DStateCustomButton2));
    Result.AddObject('DStateCustomButton3', Pointer(@DStateCustomButton3));
    Result.AddObject('DStateExpLabel', Pointer(@DStateExpLabel));
    Result.AddObject('DStateGameDiamondLabel', Pointer(@DStateGameDiamondLabel));
    Result.AddObject('DStateGameGirdLabel', Pointer(@DStateGameGirdLabel));
    Result.AddObject('DStateGameGoldLabel', Pointer(@DStateGameGoldLabel));
    Result.AddObject('DStateGameTimeGirdLabel', Pointer(@DStateGameTimeGirdLabel));
    Result.AddObject('DStateHPLabel', Pointer(@DStateHPLabel));
    Result.AddObject('DStateJobLabel', Pointer(@DStateJobLabel));
    Result.AddObject('DStateLevelLabel', Pointer(@DStateLevelLabel));
    Result.AddObject('DStateLiquorLabel', Pointer(@DStateLiquorLabel));
    Result.AddObject('DStateMPLabel', Pointer(@DStateMPLabel));
    Result.AddObject('DStateMaxExpLabel', Pointer(@DStateMaxExpLabel));
    Result.AddObject('DStatePotencyLabel', Pointer(@DStatePotencyLabel));
    Result.AddObject('DStateRefurbishLabel', Pointer(@DStateRefurbishLabel));
    Result.AddObject('DStateTabSheet4', Pointer(@DStateTabSheet4));
    Result.AddObject('DStateAbil', Pointer(@DStateAbil));
    Result.AddObject('DAbilCustomButton1', Pointer(@DAbilCustomButton1));
    Result.AddObject('DAbilCustomButton2', Pointer(@DAbilCustomButton2));
    Result.AddObject('DAbilCustomButton3', Pointer(@DAbilCustomButton3));
    Result.AddObject('DStateAntiMagicLabel', Pointer(@DStateAntiMagicLabel));
    Result.AddObject('DStateAntiPoisonLabel', Pointer(@DStateAntiPoisonLabel));
    Result.AddObject('DStateHandWeightLabel', Pointer(@DStateHandWeightLabel));
    Result.AddObject('DStateHealthRecoverLabel', Pointer(@DStateHealthRecoverLabel));
    Result.AddObject('DStateHitPointLabel', Pointer(@DStateHitPointLabel));
    Result.AddObject('DStateHitSpeedLabel', Pointer(@DStateHitSpeedLabel));
    Result.AddObject('DStateLabelAC', Pointer(@DStateLabelAC));
    Result.AddObject('DStateLabelDC', Pointer(@DStateLabelDC));
    Result.AddObject('DStateLabelMAC', Pointer(@DStateLabelMAC));
    Result.AddObject('DStateLabelMC', Pointer(@DStateLabelMC));
    Result.AddObject('DStateLabelSC', Pointer(@DStateLabelSC));
    Result.AddObject('DStateNewAbilLabel1', Pointer(@DStateNewAbilLabel1));
    Result.AddObject('DStateNewAbilLabel10', Pointer(@DStateNewAbilLabel10));
    Result.AddObject('DStateNewAbilLabel11', Pointer(@DStateNewAbilLabel11));
    Result.AddObject('DStateNewAbilLabel12', Pointer(@DStateNewAbilLabel12));
    Result.AddObject('DStateNewAbilLabel13', Pointer(@DStateNewAbilLabel13));
    Result.AddObject('DStateNewAbilLabel14', Pointer(@DStateNewAbilLabel14));
    Result.AddObject('DStateNewAbilLabel15', Pointer(@DStateNewAbilLabel15));
    Result.AddObject('DStateNewAbilLabel16', Pointer(@DStateNewAbilLabel16));
    Result.AddObject('DStateNewAbilLabel2', Pointer(@DStateNewAbilLabel2));
    Result.AddObject('DStateNewAbilLabel3', Pointer(@DStateNewAbilLabel3));
    Result.AddObject('DStateNewAbilLabel4', Pointer(@DStateNewAbilLabel4));
    Result.AddObject('DStateNewAbilLabel5', Pointer(@DStateNewAbilLabel5));
    Result.AddObject('DStateNewAbilLabel6', Pointer(@DStateNewAbilLabel6));
    Result.AddObject('DStateNewAbilLabel7', Pointer(@DStateNewAbilLabel7));
    Result.AddObject('DStateNewAbilLabel8', Pointer(@DStateNewAbilLabel8));
    Result.AddObject('DStateNewAbilLabel9', Pointer(@DStateNewAbilLabel9));
    Result.AddObject('DStatePoisonRecoverLabel', Pointer(@DStatePoisonRecoverLabel));
    Result.AddObject('DStateSpeedPointLabel', Pointer(@DStateSpeedPointLabel));
    Result.AddObject('DStateSpellRecoverLabel', Pointer(@DStateSpellRecoverLabel));
    Result.AddObject('DStateWearWeightLabel', Pointer(@DStateWearWeightLabel));
    Result.AddObject('DStateWeightLabel', Pointer(@DStateWeightLabel));
    Result.AddObject('DStateTabSheet5', Pointer(@DStateTabSheet5));
    Result.AddObject('DStateTitle', Pointer(@DStateTitle));
    Result.AddObject('DSTitleActive', Pointer(@DSTitleActive));
    Result.AddObject('DSTitleButton1', Pointer(@DSTitleButton1));
    Result.AddObject('DSTitleButton2', Pointer(@DSTitleButton2));
    Result.AddObject('DSTitleButton3', Pointer(@DSTitleButton3));
    Result.AddObject('DSTitleButton4', Pointer(@DSTitleButton4));
    Result.AddObject('DSTitleButton5', Pointer(@DSTitleButton5));
    Result.AddObject('DSTitleButton6', Pointer(@DSTitleButton6));
    Result.AddObject('DSTitleItemDesc', Pointer(@DSTitleItemDesc));
    Result.AddObject('DSTitleName1', Pointer(@DSTitleName1));
    Result.AddObject('DSTitleName2', Pointer(@DSTitleName2));
    Result.AddObject('DSTitleName3', Pointer(@DSTitleName3));
    Result.AddObject('DSTitleName4', Pointer(@DSTitleName4));
    Result.AddObject('DSTitleName5', Pointer(@DSTitleName5));
    Result.AddObject('DSTitleName6', Pointer(@DSTitleName6));
    Result.AddObject('DSTitleNameActive', Pointer(@DSTitleNameActive));
    Result.AddObject('DSTitlePageDown', Pointer(@DSTitlePageDown));
    Result.AddObject('DSTitlePageUp', Pointer(@DSTitlePageUp));
    Result.AddObject('DTitleCustomButton1', Pointer(@DTitleCustomButton1));
    Result.AddObject('DTitleCustomButton2', Pointer(@DTitleCustomButton2));
    Result.AddObject('DTitleCustomButton3', Pointer(@DTitleCustomButton3));
    Result.AddObject('DStateTabSheet6', Pointer(@DStateTabSheet6));
    Result.AddObject('DMagCustomButton1', Pointer(@DMagCustomButton1));
    Result.AddObject('DMagCustomButton2', Pointer(@DMagCustomButton2));
    Result.AddObject('DMagCustomButton3', Pointer(@DMagCustomButton3));
    Result.AddObject('DStMagicPage', Pointer(@DStMagicPage));
    Result.AddObject('DStPageDown', Pointer(@DStPageDown));
    Result.AddObject('DStPageUp', Pointer(@DStPageUp));
    Result.AddObject('DStateMagic', Pointer(@DStateMagic));
    Result.AddObject('DStMagBack1', Pointer(@DStMagBack1));
    Result.AddObject('DStMag1', Pointer(@DStMag1));
    Result.AddObject('DStMagBtn1', Pointer(@DStMagBtn1));
    Result.AddObject('DStMagExpIcon1', Pointer(@DStMagExpIcon1));
    Result.AddObject('DStMagExpText1', Pointer(@DStMagExpText1));
    Result.AddObject('DStMagLblName1', Pointer(@DStMagLblName1));
    Result.AddObject('DStMagLvIcon1', Pointer(@DStMagLvIcon1));
    Result.AddObject('DStMagLvText1', Pointer(@DStMagLvText1));
    Result.AddObject('DStMagBack2', Pointer(@DStMagBack2));
    Result.AddObject('DStMag2', Pointer(@DStMag2));
    Result.AddObject('DStMagBtn2', Pointer(@DStMagBtn2));
    Result.AddObject('DStMagBack3', Pointer(@DStMagBack3));
    Result.AddObject('DStMag3', Pointer(@DStMag3));
    Result.AddObject('DStMagBtn3', Pointer(@DStMagBtn3));
    Result.AddObject('DStMagBack4', Pointer(@DStMagBack4));
    Result.AddObject('DStMag4', Pointer(@DStMag4));
    Result.AddObject('DStMagBtn4', Pointer(@DStMagBtn4));
    Result.AddObject('DStMagBack5', Pointer(@DStMagBack5));
    Result.AddObject('DStMag5', Pointer(@DStMag5));
    Result.AddObject('DStMagBtn5', Pointer(@DStMagBtn5));
    Result.AddObject('DStMagBack6', Pointer(@DStMagBack6));
    Result.AddObject('DStMag6', Pointer(@DStMag6));
    Result.AddObject('DStMagBtn6', Pointer(@DStMagBtn6));
    Result.AddObject('DStateTabSheet7', Pointer(@DStateTabSheet7));
    Result.AddObject('DStateDeputyHero', Pointer(@DStateDeputyHero));
    Result.AddObject('DBotStateCustomButton1', Pointer(@DBotStateCustomButton1));
    Result.AddObject('DBotStateCustomButton10', Pointer(@DBotStateCustomButton10));
    Result.AddObject('DBotStateCustomButton2', Pointer(@DBotStateCustomButton2));
    Result.AddObject('DBotStateCustomButton3', Pointer(@DBotStateCustomButton3));
    Result.AddObject('DBotStateCustomButton4', Pointer(@DBotStateCustomButton4));
    Result.AddObject('DBotStateCustomButton5', Pointer(@DBotStateCustomButton5));
    Result.AddObject('DBotStateCustomButton6', Pointer(@DBotStateCustomButton6));
    Result.AddObject('DBotStateCustomButton7', Pointer(@DBotStateCustomButton7));
    Result.AddObject('DBotStateCustomButton8', Pointer(@DBotStateCustomButton8));
    Result.AddObject('DBotStateCustomButton9', Pointer(@DBotStateCustomButton9));
    Result.AddObject('DBotStateDeputyHeroJob0', Pointer(@DBotStateDeputyHeroJob0));
    Result.AddObject('DBotStateDeputyHeroJob1', Pointer(@DBotStateDeputyHeroJob1));
    Result.AddObject('DBotStateDeputyHeroJob2', Pointer(@DBotStateDeputyHeroJob2));
    Result.AddObject('MainTabSheet2', Pointer(@MainTabSheet2));
    Result.AddObject('DNGPageControl', Pointer(@DNGPageControl));
    Result.AddObject('DNGStateTabSheet1', Pointer(@DNGStateTabSheet1));
    Result.AddObject('DStateNGInfo', Pointer(@DStateNGInfo));
    Result.AddObject('DStateHJAntiPoisonLabel', Pointer(@DStateHJAntiPoisonLabel));
    Result.AddObject('DStateHJAntiPoisonValue', Pointer(@DStateHJAntiPoisonValue));
    Result.AddObject('DStateNGAntiPoisonLabel', Pointer(@DStateNGAntiPoisonLabel));
    Result.AddObject('DStateNGAntiPoisonValue', Pointer(@DStateNGAntiPoisonValue));
    Result.AddObject('DStateNGCusBtn1', Pointer(@DStateNGCusBtn1));
    Result.AddObject('DStateNGCusBtn2', Pointer(@DStateNGCusBtn2));
    Result.AddObject('DStateNGCusBtn3', Pointer(@DStateNGCusBtn3));
    Result.AddObject('DStateNGExpLabel', Pointer(@DStateNGExpLabel));
    Result.AddObject('DStateNGExpValue', Pointer(@DStateNGExpValue));
    Result.AddObject('DStateNGHitLabel', Pointer(@DStateNGHitLabel));
    Result.AddObject('DStateNGHitValue', Pointer(@DStateNGHitValue));
    Result.AddObject('DStateNGLabel', Pointer(@DStateNGLabel));
    Result.AddObject('DStateNGLevelLabel', Pointer(@DStateNGLevelLabel));
    Result.AddObject('DStateNGLevelValue', Pointer(@DStateNGLevelValue));
    Result.AddObject('DStateNGMaxExpLabel', Pointer(@DStateNGMaxExpLabel));
    Result.AddObject('DStateNGMaxExpValue', Pointer(@DStateNGMaxExpValue));
    Result.AddObject('DStateNGRecoverLabel', Pointer(@DStateNGRecoverLabel));
    Result.AddObject('DStateNGRecoverValue', Pointer(@DStateNGRecoverValue));
    Result.AddObject('DStateNGValue', Pointer(@DStateNGValue));
    Result.AddObject('DNGStateTabSheet2', Pointer(@DNGStateTabSheet2));
    Result.AddObject('DStNGMagicPage', Pointer(@DStNGMagicPage));
    Result.AddObject('DStNGPageDown', Pointer(@DStNGPageDown));
    Result.AddObject('DStNGPageUp', Pointer(@DStNGPageUp));
    Result.AddObject('DStateNGMagic', Pointer(@DStateNGMagic));
    Result.AddObject('DStNGMagBack1', Pointer(@DStNGMagBack1));
    Result.AddObject('DStNGMag1', Pointer(@DStNGMag1));
    Result.AddObject('DStNGMagExpIcon1', Pointer(@DStNGMagExpIcon1));
    Result.AddObject('DStNGMagExpText1', Pointer(@DStNGMagExpText1));
    Result.AddObject('DStNGMagLblName1', Pointer(@DStNGMagLblName1));
    Result.AddObject('DStNGMagLvIcon1', Pointer(@DStNGMagLvIcon1));
    Result.AddObject('DStNGMagLvText1', Pointer(@DStNGMagLvText1));
    Result.AddObject('DStNGMagBack2', Pointer(@DStNGMagBack2));
    Result.AddObject('DStNGMag2', Pointer(@DStNGMag2));
    Result.AddObject('DStNGMagBack3', Pointer(@DStNGMagBack3));
    Result.AddObject('DStNGMag3', Pointer(@DStNGMag3));
    Result.AddObject('DStNGMagBack4', Pointer(@DStNGMagBack4));
    Result.AddObject('DStNGMag4', Pointer(@DStNGMag4));
    Result.AddObject('DStNGMagBack5', Pointer(@DStNGMagBack5));
    Result.AddObject('DStNGMag5', Pointer(@DStNGMag5));
    Result.AddObject('DStNGMagBack6', Pointer(@DStNGMagBack6));
    Result.AddObject('DStNGMag6', Pointer(@DStNGMag6));
    Result.AddObject('DStateNGMagicCusBnt1', Pointer(@DStateNGMagicCusBnt1));
    Result.AddObject('DStateNGMagicCusBnt2', Pointer(@DStateNGMagicCusBnt2));
    Result.AddObject('DStateNGMagicCusBnt3', Pointer(@DStateNGMagicCusBnt3));
    Result.AddObject('DNGStateTabSheet3', Pointer(@DNGStateTabSheet3));
    Result.AddObject('DStateNGMeridians', Pointer(@DStateNGMeridians));
    Result.AddObject('DBotMeridians0', Pointer(@DBotMeridians0));
    Result.AddObject('DBotMeridians1', Pointer(@DBotMeridians1));
    Result.AddObject('DBotMeridians2', Pointer(@DBotMeridians2));
    Result.AddObject('DBotMeridians3', Pointer(@DBotMeridians3));
    Result.AddObject('DBotMeridians4', Pointer(@DBotMeridians4));
    Result.AddObject('DMeridiansPageControl', Pointer(@DMeridiansPageControl));
    Result.AddObject('DMeridiansTabSheet0', Pointer(@DMeridiansTabSheet0));
    Result.AddObject('DMeridiansLine0', Pointer(@DMeridiansLine0));
    Result.AddObject('DBotAcupoints0_0', Pointer(@DBotAcupoints0_0));
    Result.AddObject('DBotAcupoints0_1', Pointer(@DBotAcupoints0_1));
    Result.AddObject('DBotAcupoints0_2', Pointer(@DBotAcupoints0_2));
    Result.AddObject('DBotAcupoints0_3', Pointer(@DBotAcupoints0_3));
    Result.AddObject('DBotAcupoints0_4', Pointer(@DBotAcupoints0_4));
    Result.AddObject('DLabelAcupoints0_0', Pointer(@DLabelAcupoints0_0));
    Result.AddObject('DLabelAcupoints0_1', Pointer(@DLabelAcupoints0_1));
    Result.AddObject('DLabelAcupoints0_2', Pointer(@DLabelAcupoints0_2));
    Result.AddObject('DLabelAcupoints0_3', Pointer(@DLabelAcupoints0_3));
    Result.AddObject('DLabelAcupoints0_4', Pointer(@DLabelAcupoints0_4));
    Result.AddObject('DMeridiansTabSheet1', Pointer(@DMeridiansTabSheet1));
    Result.AddObject('DMeridiansLine1', Pointer(@DMeridiansLine1));
    Result.AddObject('DBotAcupoints1_0', Pointer(@DBotAcupoints1_0));
    Result.AddObject('DBotAcupoints1_1', Pointer(@DBotAcupoints1_1));
    Result.AddObject('DBotAcupoints1_2', Pointer(@DBotAcupoints1_2));
    Result.AddObject('DBotAcupoints1_3', Pointer(@DBotAcupoints1_3));
    Result.AddObject('DBotAcupoints1_4', Pointer(@DBotAcupoints1_4));
    Result.AddObject('DLabelAcupoints1_0', Pointer(@DLabelAcupoints1_0));
    Result.AddObject('DLabelAcupoints1_1', Pointer(@DLabelAcupoints1_1));
    Result.AddObject('DLabelAcupoints1_2', Pointer(@DLabelAcupoints1_2));
    Result.AddObject('DLabelAcupoints1_3', Pointer(@DLabelAcupoints1_3));
    Result.AddObject('DLabelAcupoints1_4', Pointer(@DLabelAcupoints1_4));
    Result.AddObject('DLabelMeridiansStatus1', Pointer(@DLabelMeridiansStatus1));
    Result.AddObject('DMeridiansTabSheet2', Pointer(@DMeridiansTabSheet2));
    Result.AddObject('DMeridiansLine2', Pointer(@DMeridiansLine2));
    Result.AddObject('DBotAcupoints2_0', Pointer(@DBotAcupoints2_0));
    Result.AddObject('DBotAcupoints2_1', Pointer(@DBotAcupoints2_1));
    Result.AddObject('DBotAcupoints2_2', Pointer(@DBotAcupoints2_2));
    Result.AddObject('DBotAcupoints2_3', Pointer(@DBotAcupoints2_3));
    Result.AddObject('DBotAcupoints2_4', Pointer(@DBotAcupoints2_4));
    Result.AddObject('DLabelAcupoints2_0', Pointer(@DLabelAcupoints2_0));
    Result.AddObject('DLabelAcupoints2_1', Pointer(@DLabelAcupoints2_1));
    Result.AddObject('DLabelAcupoints2_2', Pointer(@DLabelAcupoints2_2));
    Result.AddObject('DLabelAcupoints2_3', Pointer(@DLabelAcupoints2_3));
    Result.AddObject('DLabelAcupoints2_4', Pointer(@DLabelAcupoints2_4));
    Result.AddObject('DLabelMeridiansStatus2', Pointer(@DLabelMeridiansStatus2));
    Result.AddObject('DMeridiansTabSheet3', Pointer(@DMeridiansTabSheet3));
    Result.AddObject('DMeridiansLine3', Pointer(@DMeridiansLine3));
    Result.AddObject('DBotAcupoints3_0', Pointer(@DBotAcupoints3_0));
    Result.AddObject('DBotAcupoints3_1', Pointer(@DBotAcupoints3_1));
    Result.AddObject('DBotAcupoints3_2', Pointer(@DBotAcupoints3_2));
    Result.AddObject('DBotAcupoints3_3', Pointer(@DBotAcupoints3_3));
    Result.AddObject('DBotAcupoints3_4', Pointer(@DBotAcupoints3_4));
    Result.AddObject('DLabelAcupoints3_0', Pointer(@DLabelAcupoints3_0));
    Result.AddObject('DLabelAcupoints3_1', Pointer(@DLabelAcupoints3_1));
    Result.AddObject('DLabelAcupoints3_2', Pointer(@DLabelAcupoints3_2));
    Result.AddObject('DLabelAcupoints3_3', Pointer(@DLabelAcupoints3_3));
    Result.AddObject('DLabelAcupoints3_4', Pointer(@DLabelAcupoints3_4));
    Result.AddObject('DLabelMeridiansStatus3', Pointer(@DLabelMeridiansStatus3));
    Result.AddObject('DMeridiansTabSheet4', Pointer(@DMeridiansTabSheet4));
    Result.AddObject('DMeridiansLine4', Pointer(@DMeridiansLine4));
    Result.AddObject('DBotAcupoints4_0', Pointer(@DBotAcupoints4_0));
    Result.AddObject('DBotAcupoints4_1', Pointer(@DBotAcupoints4_1));
    Result.AddObject('DBotAcupoints4_2', Pointer(@DBotAcupoints4_2));
    Result.AddObject('DBotAcupoints4_3', Pointer(@DBotAcupoints4_3));
    Result.AddObject('DBotAcupoints4_4', Pointer(@DBotAcupoints4_4));
    Result.AddObject('DLabelAcupoints4_0', Pointer(@DLabelAcupoints4_0));
    Result.AddObject('DLabelAcupoints4_1', Pointer(@DLabelAcupoints4_1));
    Result.AddObject('DLabelAcupoints4_2', Pointer(@DLabelAcupoints4_2));
    Result.AddObject('DLabelAcupoints4_3', Pointer(@DLabelAcupoints4_3));
    Result.AddObject('DLabelAcupoints4_4', Pointer(@DLabelAcupoints4_4));
    Result.AddObject('DLabelMeridiansStatus4', Pointer(@DLabelMeridiansStatus4));
    Result.AddObject('DStateNGMeridiansCusBtn1', Pointer(@DStateNGMeridiansCusBtn1));
    Result.AddObject('DStateNGMeridiansCusBtn2', Pointer(@DStateNGMeridiansCusBtn2));
    Result.AddObject('DStateNGMeridiansCusBtn3', Pointer(@DStateNGMeridiansCusBtn3));
    Result.AddObject('DTrainingMeridian', Pointer(@DTrainingMeridian));
    Result.AddObject('DNGStateTabSheet4', Pointer(@DNGStateTabSheet4));
    Result.AddObject('DStateNGContinuousMagic', Pointer(@DStateNGContinuousMagic));
    Result.AddObject('DContinuousMagicList', Pointer(@DContinuousMagicList));
    Result.AddObject('DStLJMagBack1', Pointer(@DStLJMagBack1));
    Result.AddObject('DStLJMag1', Pointer(@DStLJMag1));
    Result.AddObject('DStLJMagExpIcon1', Pointer(@DStLJMagExpIcon1));
    Result.AddObject('DStLJMagExpText1', Pointer(@DStLJMagExpText1));
    Result.AddObject('DStLJMagLblName1', Pointer(@DStLJMagLblName1));
    Result.AddObject('DStLJMagLvIcon1', Pointer(@DStLJMagLvIcon1));
    Result.AddObject('DStLJMagLvText1', Pointer(@DStLJMagLvText1));
    Result.AddObject('DStLJMagBack2', Pointer(@DStLJMagBack2));
    Result.AddObject('DStLJMag2', Pointer(@DStLJMag2));
    Result.AddObject('DStLJMagBack3', Pointer(@DStLJMagBack3));
    Result.AddObject('DStLJMag3', Pointer(@DStLJMag3));
    Result.AddObject('DStLJMagBack4', Pointer(@DStLJMagBack4));
    Result.AddObject('DStLJMag4', Pointer(@DStLJMag4));
    Result.AddObject('DSMB1', Pointer(@DSMB1));
    Result.AddObject('DSMB2', Pointer(@DSMB2));
    Result.AddObject('DSMB3', Pointer(@DSMB3));
    Result.AddObject('DSMB4', Pointer(@DSMB4));
    Result.AddObject('DSMBShortcut', Pointer(@DSMBShortcut));
    Result.AddObject('DStateNGMagic2CusBtn1', Pointer(@DStateNGMagic2CusBtn1));
    Result.AddObject('DStateNGMagic2CusBtn2', Pointer(@DStateNGMagic2CusBtn2));
    Result.AddObject('DStateNGMagic2CusBtn3', Pointer(@DStateNGMagic2CusBtn3));
    Result.AddObject('MainTabSheet3', Pointer(@MainTabSheet3));
    Result.AddObject('DXFPageControl', Pointer(@DXFPageControl));
    Result.AddObject('LabelDStateWinCharName', Pointer(@LabelDStateWinCharName));
    Result.AddObject('btnDStateWinEx', Pointer(@btnDStateWinEx));
    Result.AddObject('DStateWinEx', Pointer(@DStateWinEx));
    Result.AddObject('btnStateExtExp', Pointer(@btnStateExtExp));
    Result.AddObject('btnStateExtHP', Pointer(@btnStateExtHP));
    Result.AddObject('btnStateExtHandWeight', Pointer(@btnStateExtHandWeight));
    Result.AddObject('btnStateExtMC', Pointer(@btnStateExtMC));
    Result.AddObject('btnStateExtNG', Pointer(@btnStateExtNG));
    Result.AddObject('btnStateExtWearWeight', Pointer(@btnStateExtWearWeight));
    Result.AddObject('btnStateExtWeight', Pointer(@btnStateExtWeight));
    Result.AddObject('lblStateExtAC', Pointer(@lblStateExtAC));
    Result.AddObject('lblStateExtAntiMagic', Pointer(@lblStateExtAntiMagic));
    Result.AddObject('lblStateExtAntiPoiston', Pointer(@lblStateExtAntiPoiston));
    Result.AddObject('lblStateExtAttackSpeed', Pointer(@lblStateExtAttackSpeed));
    Result.AddObject('lblStateExtDC', Pointer(@lblStateExtDC));
    Result.AddObject('lblStateExtGuild', Pointer(@lblStateExtGuild));
    Result.AddObject('lblStateExtHPRecover', Pointer(@lblStateExtHPRecover));
    Result.AddObject('lblStateExtHitPoint', Pointer(@lblStateExtHitPoint));
    Result.AddObject('lblStateExtJob', Pointer(@lblStateExtJob));
    Result.AddObject('lblStateExtLevel', Pointer(@lblStateExtLevel));
    Result.AddObject('lblStateExtMAC', Pointer(@lblStateExtMAC));
    Result.AddObject('lblStateExtMC', Pointer(@lblStateExtMC));
    Result.AddObject('lblStateExtMPRecover', Pointer(@lblStateExtMPRecover));
    Result.AddObject('lblStateExtNewValue0', Pointer(@lblStateExtNewValue0));
    Result.AddObject('lblStateExtNewValue1', Pointer(@lblStateExtNewValue1));
    Result.AddObject('lblStateExtNewValue10', Pointer(@lblStateExtNewValue10));
    Result.AddObject('lblStateExtNewValue11', Pointer(@lblStateExtNewValue11));
    Result.AddObject('lblStateExtNewValue12', Pointer(@lblStateExtNewValue12));
    Result.AddObject('lblStateExtNewValue13', Pointer(@lblStateExtNewValue13));
    Result.AddObject('lblStateExtNewValue14', Pointer(@lblStateExtNewValue14));
    Result.AddObject('lblStateExtNewValue15', Pointer(@lblStateExtNewValue15));
    Result.AddObject('lblStateExtNewValue16', Pointer(@lblStateExtNewValue16));
    Result.AddObject('lblStateExtNewValue17', Pointer(@lblStateExtNewValue17));
    Result.AddObject('lblStateExtNewValue18', Pointer(@lblStateExtNewValue18));
    Result.AddObject('lblStateExtNewValue19', Pointer(@lblStateExtNewValue19));
    Result.AddObject('lblStateExtNewValue2', Pointer(@lblStateExtNewValue2));
    Result.AddObject('lblStateExtNewValue20', Pointer(@lblStateExtNewValue20));
    Result.AddObject('lblStateExtNewValue21', Pointer(@lblStateExtNewValue21));
    Result.AddObject('lblStateExtNewValue22', Pointer(@lblStateExtNewValue22));
    Result.AddObject('lblStateExtNewValue23', Pointer(@lblStateExtNewValue23));
    Result.AddObject('lblStateExtNewValue3', Pointer(@lblStateExtNewValue3));
    Result.AddObject('lblStateExtNewValue4', Pointer(@lblStateExtNewValue4));
    Result.AddObject('lblStateExtNewValue5', Pointer(@lblStateExtNewValue5));
    Result.AddObject('lblStateExtNewValue6', Pointer(@lblStateExtNewValue6));
    Result.AddObject('lblStateExtNewValue7', Pointer(@lblStateExtNewValue7));
    Result.AddObject('lblStateExtNewValue8', Pointer(@lblStateExtNewValue8));
    Result.AddObject('lblStateExtNewValue9', Pointer(@lblStateExtNewValue9));
    Result.AddObject('lblStateExtPosionRecover', Pointer(@lblStateExtPosionRecover));
    Result.AddObject('lblStateExtSC', Pointer(@lblStateExtSC));
    Result.AddObject('lblStateExtSpeedPoint', Pointer(@lblStateExtSpeedPoint));
    Result.AddObject('DHeroSWGodBlessDlg', Pointer(@DHeroSWGodBlessDlg));
    Result.AddObject('DHeroSWGodBless1', Pointer(@DHeroSWGodBless1));
    Result.AddObject('DHeroSWGodBless10', Pointer(@DHeroSWGodBless10));
    Result.AddObject('DHeroSWGodBless11', Pointer(@DHeroSWGodBless11));
    Result.AddObject('DHeroSWGodBless12', Pointer(@DHeroSWGodBless12));
    Result.AddObject('DHeroSWGodBless2', Pointer(@DHeroSWGodBless2));
    Result.AddObject('DHeroSWGodBless3', Pointer(@DHeroSWGodBless3));
    Result.AddObject('DHeroSWGodBless4', Pointer(@DHeroSWGodBless4));
    Result.AddObject('DHeroSWGodBless5', Pointer(@DHeroSWGodBless5));
    Result.AddObject('DHeroSWGodBless6', Pointer(@DHeroSWGodBless6));
    Result.AddObject('DHeroSWGodBless7', Pointer(@DHeroSWGodBless7));
    Result.AddObject('DHeroSWGodBless8', Pointer(@DHeroSWGodBless8));
    Result.AddObject('DHeroSWGodBless9', Pointer(@DHeroSWGodBless9));
    Result.AddObject('DHeroSWGodBlessClose', Pointer(@DHeroSWGodBlessClose));
    Result.AddObject('DHeroSWGodBlessInfo', Pointer(@DHeroSWGodBlessInfo));
    Result.AddObject('DHeroSWJewelryBoxDlg', Pointer(@DHeroSWJewelryBoxDlg));
    Result.AddObject('DHeroSWJewelry1', Pointer(@DHeroSWJewelry1));
    Result.AddObject('DHeroSWJewelry2', Pointer(@DHeroSWJewelry2));
    Result.AddObject('DHeroSWJewelry3', Pointer(@DHeroSWJewelry3));
    Result.AddObject('DHeroSWJewelry4', Pointer(@DHeroSWJewelry4));
    Result.AddObject('DHeroSWJewelry5', Pointer(@DHeroSWJewelry5));
    Result.AddObject('DHeroSWJewelry6', Pointer(@DHeroSWJewelry6));
    Result.AddObject('DHeroSWJewelryClose', Pointer(@DHeroSWJewelryClose));
    Result.AddObject('DHeroStateWin', Pointer(@DHeroStateWin));
    Result.AddObject('DHeroCloseState', Pointer(@DHeroCloseState));
    Result.AddObject('DHeroLabelDStateWinCharName', Pointer(@DHeroLabelDStateWinCharName));
    Result.AddObject('DHeroStatePageControl', Pointer(@DHeroStatePageControl));
    Result.AddObject('HeroMainTabSheet1', Pointer(@HeroMainTabSheet1));
    Result.AddObject('DHeroBasicPageControl', Pointer(@DHeroBasicPageControl));
    Result.AddObject('DHeroStateTabSheet1', Pointer(@DHeroStateTabSheet1));
    Result.AddObject('DHeroStateBasic', Pointer(@DHeroStateBasic));
    Result.AddObject('DHeroSWArmRingL', Pointer(@DHeroSWArmRingL));
    Result.AddObject('DHeroSWArmRingR', Pointer(@DHeroSWArmRingR));
    Result.AddObject('DHeroSWBelt', Pointer(@DHeroSWBelt));
    Result.AddObject('DHeroSWBoots', Pointer(@DHeroSWBoots));
    Result.AddObject('DHeroSWBujuk', Pointer(@DHeroSWBujuk));
    Result.AddObject('DHeroSWCharm', Pointer(@DHeroSWCharm));
    Result.AddObject('DHeroSWDress', Pointer(@DHeroSWDress));
    Result.AddObject('DHeroSWDrum', Pointer(@DHeroSWDrum));
    Result.AddObject('DHeroSWGodBless', Pointer(@DHeroSWGodBless));
    Result.AddObject('DHeroSWHelmet', Pointer(@DHeroSWHelmet));
    Result.AddObject('DHeroSWHorse', Pointer(@DHeroSWHorse));
    Result.AddObject('DHeroSWJade', Pointer(@DHeroSWJade));
    Result.AddObject('DHeroSWJewelryBox', Pointer(@DHeroSWJewelryBox));
    Result.AddObject('DHeroSWLight', Pointer(@DHeroSWLight));
    Result.AddObject('DHeroSWNecklace', Pointer(@DHeroSWNecklace));
    Result.AddObject('DHeroSWRingL', Pointer(@DHeroSWRingL));
    Result.AddObject('DHeroSWRingR', Pointer(@DHeroSWRingR));
    Result.AddObject('DHeroSWShield', Pointer(@DHeroSWShield));
    Result.AddObject('DHeroSWWeapon', Pointer(@DHeroSWWeapon));
    Result.AddObject('DHeroStateTabSheet2', Pointer(@DHeroStateTabSheet2));
    Result.AddObject('DHeroStateFashion', Pointer(@DHeroStateFashion));
    Result.AddObject('DHeroSWFashionArmRingL', Pointer(@DHeroSWFashionArmRingL));
    Result.AddObject('DHeroSWFashionArmRingR', Pointer(@DHeroSWFashionArmRingR));
    Result.AddObject('DHeroSWFashionBelt', Pointer(@DHeroSWFashionBelt));
    Result.AddObject('DHeroSWFashionBoots', Pointer(@DHeroSWFashionBoots));
    Result.AddObject('DHeroSWFashionDress', Pointer(@DHeroSWFashionDress));
    Result.AddObject('DHeroSWFashionDrum', Pointer(@DHeroSWFashionDrum));
    Result.AddObject('DHeroSWFashionHelmet', Pointer(@DHeroSWFashionHelmet));
    Result.AddObject('DHeroSWFashionLight', Pointer(@DHeroSWFashionLight));
    Result.AddObject('DHeroSWFashionNecklace', Pointer(@DHeroSWFashionNecklace));
    Result.AddObject('DHeroSWFashionRingL', Pointer(@DHeroSWFashionRingL));
    Result.AddObject('DHeroSWFashionRingR', Pointer(@DHeroSWFashionRingR));
    Result.AddObject('DHeroSWFashionWeapon', Pointer(@DHeroSWFashionWeapon));
    Result.AddObject('DHeroSWShowFashion', Pointer(@DHeroSWShowFashion));
    Result.AddObject('DHeroStateTabSheet3', Pointer(@DHeroStateTabSheet3));
    Result.AddObject('DHeroStateInfo', Pointer(@DHeroStateInfo));
    Result.AddObject('DHeroStateAlcoholLabel', Pointer(@DHeroStateAlcoholLabel));
    Result.AddObject('DHeroStateCreditPointLabel', Pointer(@DHeroStateCreditPointLabel));
    Result.AddObject('DHeroStateExpLabel', Pointer(@DHeroStateExpLabel));
    Result.AddObject('DHeroStateHPLabel', Pointer(@DHeroStateHPLabel));
    Result.AddObject('DHeroStateJobLabel', Pointer(@DHeroStateJobLabel));
    Result.AddObject('DHeroStateLevelLabel', Pointer(@DHeroStateLevelLabel));
    Result.AddObject('DHeroStateLiquorLabel', Pointer(@DHeroStateLiquorLabel));
    Result.AddObject('DHeroStateMPLabel', Pointer(@DHeroStateMPLabel));
    Result.AddObject('DHeroStateMaxExpLabel', Pointer(@DHeroStateMaxExpLabel));
    Result.AddObject('DHeroStatePotencyLabel', Pointer(@DHeroStatePotencyLabel));
    Result.AddObject('DHeroStateTabSheet4', Pointer(@DHeroStateTabSheet4));
    Result.AddObject('DHeroStateAbil', Pointer(@DHeroStateAbil));
    Result.AddObject('DHeroStateAntiMagicLabel', Pointer(@DHeroStateAntiMagicLabel));
    Result.AddObject('DHeroStateAntiPoisonLabel', Pointer(@DHeroStateAntiPoisonLabel));
    Result.AddObject('DHeroStateHandWeightLabel', Pointer(@DHeroStateHandWeightLabel));
    Result.AddObject('DHeroStateHealthRecoverLabel', Pointer(@DHeroStateHealthRecoverLabel));
    Result.AddObject('DHeroStateHitPointLabel', Pointer(@DHeroStateHitPointLabel));
    Result.AddObject('DHeroStateHitSpeedLabel', Pointer(@DHeroStateHitSpeedLabel));
    Result.AddObject('DHeroStateLabelAC', Pointer(@DHeroStateLabelAC));
    Result.AddObject('DHeroStateLabelDC', Pointer(@DHeroStateLabelDC));
    Result.AddObject('DHeroStateLabelMAC', Pointer(@DHeroStateLabelMAC));
    Result.AddObject('DHeroStateLabelMC', Pointer(@DHeroStateLabelMC));
    Result.AddObject('DHeroStateLabelSC', Pointer(@DHeroStateLabelSC));
    Result.AddObject('DHeroStateNewAbilLabel1', Pointer(@DHeroStateNewAbilLabel1));
    Result.AddObject('DHeroStateNewAbilLabel10', Pointer(@DHeroStateNewAbilLabel10));
    Result.AddObject('DHeroStateNewAbilLabel11', Pointer(@DHeroStateNewAbilLabel11));
    Result.AddObject('DHeroStateNewAbilLabel12', Pointer(@DHeroStateNewAbilLabel12));
    Result.AddObject('DHeroStateNewAbilLabel13', Pointer(@DHeroStateNewAbilLabel13));
    Result.AddObject('DHeroStateNewAbilLabel14', Pointer(@DHeroStateNewAbilLabel14));
    Result.AddObject('DHeroStateNewAbilLabel15', Pointer(@DHeroStateNewAbilLabel15));
    Result.AddObject('DHeroStateNewAbilLabel16', Pointer(@DHeroStateNewAbilLabel16));
    Result.AddObject('DHeroStateNewAbilLabel2', Pointer(@DHeroStateNewAbilLabel2));
    Result.AddObject('DHeroStateNewAbilLabel3', Pointer(@DHeroStateNewAbilLabel3));
    Result.AddObject('DHeroStateNewAbilLabel4', Pointer(@DHeroStateNewAbilLabel4));
    Result.AddObject('DHeroStateNewAbilLabel5', Pointer(@DHeroStateNewAbilLabel5));
    Result.AddObject('DHeroStateNewAbilLabel6', Pointer(@DHeroStateNewAbilLabel6));
    Result.AddObject('DHeroStateNewAbilLabel7', Pointer(@DHeroStateNewAbilLabel7));
    Result.AddObject('DHeroStateNewAbilLabel8', Pointer(@DHeroStateNewAbilLabel8));
    Result.AddObject('DHeroStateNewAbilLabel9', Pointer(@DHeroStateNewAbilLabel9));
    Result.AddObject('DHeroStatePoisonRecoverLabel', Pointer(@DHeroStatePoisonRecoverLabel));
    Result.AddObject('DHeroStateSpeedPointLabel', Pointer(@DHeroStateSpeedPointLabel));
    Result.AddObject('DHeroStateSpellRecoverLabel', Pointer(@DHeroStateSpellRecoverLabel));
    Result.AddObject('DHeroStateWearWeightLabel', Pointer(@DHeroStateWearWeightLabel));
    Result.AddObject('DHeroStateWeightLabel', Pointer(@DHeroStateWeightLabel));
    Result.AddObject('DHeroStateTabSheet5', Pointer(@DHeroStateTabSheet5));
    Result.AddObject('DHeroStateTitle', Pointer(@DHeroStateTitle));
    Result.AddObject('DSHeroTitleActive', Pointer(@DSHeroTitleActive));
    Result.AddObject('DSHeroTitleButton1', Pointer(@DSHeroTitleButton1));
    Result.AddObject('DSHeroTitleButton2', Pointer(@DSHeroTitleButton2));
    Result.AddObject('DSHeroTitleButton3', Pointer(@DSHeroTitleButton3));
    Result.AddObject('DSHeroTitleButton4', Pointer(@DSHeroTitleButton4));
    Result.AddObject('DSHeroTitleButton5', Pointer(@DSHeroTitleButton5));
    Result.AddObject('DSHeroTitleButton6', Pointer(@DSHeroTitleButton6));
    Result.AddObject('DSHeroTitleItemDesc', Pointer(@DSHeroTitleItemDesc));
    Result.AddObject('DSHeroTitleName1', Pointer(@DSHeroTitleName1));
    Result.AddObject('DSHeroTitleName2', Pointer(@DSHeroTitleName2));
    Result.AddObject('DSHeroTitleName3', Pointer(@DSHeroTitleName3));
    Result.AddObject('DSHeroTitleName4', Pointer(@DSHeroTitleName4));
    Result.AddObject('DSHeroTitleName5', Pointer(@DSHeroTitleName5));
    Result.AddObject('DSHeroTitleName6', Pointer(@DSHeroTitleName6));
    Result.AddObject('DSHeroTitleNameActive', Pointer(@DSHeroTitleNameActive));
    Result.AddObject('DSHeroTitlePageDown', Pointer(@DSHeroTitlePageDown));
    Result.AddObject('DSHeroTitlePageUp', Pointer(@DSHeroTitlePageUp));
    Result.AddObject('DHeroStateTabSheet6', Pointer(@DHeroStateTabSheet6));
    Result.AddObject('DHeroStMagicPage', Pointer(@DHeroStMagicPage));
    Result.AddObject('DHeroStPageDown', Pointer(@DHeroStPageDown));
    Result.AddObject('DHeroStPageUp', Pointer(@DHeroStPageUp));
    Result.AddObject('DHeroStateMagic', Pointer(@DHeroStateMagic));
    Result.AddObject('DHeroStMagBack1', Pointer(@DHeroStMagBack1));
    Result.AddObject('DHeroStMag1', Pointer(@DHeroStMag1));
    Result.AddObject('DHeroStMagBtn1', Pointer(@DHeroStMagBtn1));
    Result.AddObject('DStHeroMagExpIcon1', Pointer(@DStHeroMagExpIcon1));
    Result.AddObject('DStHeroMagExpText1', Pointer(@DStHeroMagExpText1));
    Result.AddObject('DStHeroMagLblName1', Pointer(@DStHeroMagLblName1));
    Result.AddObject('DStHeroMagLvIcon1', Pointer(@DStHeroMagLvIcon1));
    Result.AddObject('DStHeroMagLvText1', Pointer(@DStHeroMagLvText1));
    Result.AddObject('DHeroStMagBack2', Pointer(@DHeroStMagBack2));
    Result.AddObject('DHeroStMag2', Pointer(@DHeroStMag2));
    Result.AddObject('DHeroStMagBtn2', Pointer(@DHeroStMagBtn2));
    Result.AddObject('DHeroStMagBack3', Pointer(@DHeroStMagBack3));
    Result.AddObject('DHeroStMag3', Pointer(@DHeroStMag3));
    Result.AddObject('DHeroStMagBtn3', Pointer(@DHeroStMagBtn3));
    Result.AddObject('DHeroStMagBack4', Pointer(@DHeroStMagBack4));
    Result.AddObject('DHeroStMag4', Pointer(@DHeroStMag4));
    Result.AddObject('DHeroStMagBtn4', Pointer(@DHeroStMagBtn4));
    Result.AddObject('DHeroStMagBack5', Pointer(@DHeroStMagBack5));
    Result.AddObject('DHeroStMag5', Pointer(@DHeroStMag5));
    Result.AddObject('DHeroStMagBtn5', Pointer(@DHeroStMagBtn5));
    Result.AddObject('DHeroStMagBack6', Pointer(@DHeroStMagBack6));
    Result.AddObject('DHeroStMag6', Pointer(@DHeroStMag6));
    Result.AddObject('DHeroStMagBtn6', Pointer(@DHeroStMagBtn6));
    Result.AddObject('HeroMainTabSheet2', Pointer(@HeroMainTabSheet2));
    Result.AddObject('DHeroNGPageControl', Pointer(@DHeroNGPageControl));
    Result.AddObject('DHeroNGStateTabSheet1', Pointer(@DHeroNGStateTabSheet1));
    Result.AddObject('DHeroStateNGInfo', Pointer(@DHeroStateNGInfo));
    Result.AddObject('DHeroStateHJAntiPoisonLabel', Pointer(@DHeroStateHJAntiPoisonLabel));
    Result.AddObject('DHeroStateHJAntiPoisonValue', Pointer(@DHeroStateHJAntiPoisonValue));
    Result.AddObject('DHeroStateNGAntiPoisonLabel', Pointer(@DHeroStateNGAntiPoisonLabel));
    Result.AddObject('DHeroStateNGAntiPoisonValue', Pointer(@DHeroStateNGAntiPoisonValue));
    Result.AddObject('DHeroStateNGExpLabel', Pointer(@DHeroStateNGExpLabel));
    Result.AddObject('DHeroStateNGExpValue', Pointer(@DHeroStateNGExpValue));
    Result.AddObject('DHeroStateNGHitLabel', Pointer(@DHeroStateNGHitLabel));
    Result.AddObject('DHeroStateNGHitValue', Pointer(@DHeroStateNGHitValue));
    Result.AddObject('DHeroStateNGLabel', Pointer(@DHeroStateNGLabel));
    Result.AddObject('DHeroStateNGLevelLabel', Pointer(@DHeroStateNGLevelLabel));
    Result.AddObject('DHeroStateNGLevelValue', Pointer(@DHeroStateNGLevelValue));
    Result.AddObject('DHeroStateNGMaxExpLabel', Pointer(@DHeroStateNGMaxExpLabel));
    Result.AddObject('DHeroStateNGMaxExpValue', Pointer(@DHeroStateNGMaxExpValue));
    Result.AddObject('DHeroStateNGRecoverLabel', Pointer(@DHeroStateNGRecoverLabel));
    Result.AddObject('DHeroStateNGRecoverValue', Pointer(@DHeroStateNGRecoverValue));
    Result.AddObject('DHeroStateNGValue', Pointer(@DHeroStateNGValue));
    Result.AddObject('DHeroNGStateTabSheet2', Pointer(@DHeroNGStateTabSheet2));
    Result.AddObject('DHeroStNGMagicPage', Pointer(@DHeroStNGMagicPage));
    Result.AddObject('DHeroStNGPageDown', Pointer(@DHeroStNGPageDown));
    Result.AddObject('DHeroStNGPageUp', Pointer(@DHeroStNGPageUp));
    Result.AddObject('DHeroStateNGMagic', Pointer(@DHeroStateNGMagic));
    Result.AddObject('DHeroStNGMagBack1', Pointer(@DHeroStNGMagBack1));
    Result.AddObject('DHeroStNGExpIcon1', Pointer(@DHeroStNGExpIcon1));
    Result.AddObject('DHeroStNGExpText1', Pointer(@DHeroStNGExpText1));
    Result.AddObject('DHeroStNGLblName1', Pointer(@DHeroStNGLblName1));
    Result.AddObject('DHeroStNGLvIcon1', Pointer(@DHeroStNGLvIcon1));
    Result.AddObject('DHeroStNGLvText1', Pointer(@DHeroStNGLvText1));
    Result.AddObject('DHeroStNGMag1', Pointer(@DHeroStNGMag1));
    Result.AddObject('DHeroStNGMagBack2', Pointer(@DHeroStNGMagBack2));
    Result.AddObject('DHeroStNGMag2', Pointer(@DHeroStNGMag2));
    Result.AddObject('DHeroStNGMagBack3', Pointer(@DHeroStNGMagBack3));
    Result.AddObject('DHeroStNGMag3', Pointer(@DHeroStNGMag3));
    Result.AddObject('DHeroStNGMagBack4', Pointer(@DHeroStNGMagBack4));
    Result.AddObject('DHeroStNGMag4', Pointer(@DHeroStNGMag4));
    Result.AddObject('DHeroStNGMagBack5', Pointer(@DHeroStNGMagBack5));
    Result.AddObject('DHeroStNGMag5', Pointer(@DHeroStNGMag5));
    Result.AddObject('DHeroStNGMagBack6', Pointer(@DHeroStNGMagBack6));
    Result.AddObject('DHeroStNGMag6', Pointer(@DHeroStNGMag6));
    Result.AddObject('DHeroNGStateTabSheet3', Pointer(@DHeroNGStateTabSheet3));
    Result.AddObject('DHeroStateNGMeridians', Pointer(@DHeroStateNGMeridians));
    Result.AddObject('DHeroBotMeridians0', Pointer(@DHeroBotMeridians0));
    Result.AddObject('DHeroBotMeridians1', Pointer(@DHeroBotMeridians1));
    Result.AddObject('DHeroBotMeridians2', Pointer(@DHeroBotMeridians2));
    Result.AddObject('DHeroBotMeridians3', Pointer(@DHeroBotMeridians3));
    Result.AddObject('DHeroBotMeridians4', Pointer(@DHeroBotMeridians4));
    Result.AddObject('DHeroMeridiansPageControl', Pointer(@DHeroMeridiansPageControl));
    Result.AddObject('DHeroMeridiansTabSheet0', Pointer(@DHeroMeridiansTabSheet0));
    Result.AddObject('DHeroMeridiansLine0', Pointer(@DHeroMeridiansLine0));
    Result.AddObject('DHeroBotAcupoints0_0', Pointer(@DHeroBotAcupoints0_0));
    Result.AddObject('DHeroBotAcupoints0_1', Pointer(@DHeroBotAcupoints0_1));
    Result.AddObject('DHeroBotAcupoints0_2', Pointer(@DHeroBotAcupoints0_2));
    Result.AddObject('DHeroBotAcupoints0_3', Pointer(@DHeroBotAcupoints0_3));
    Result.AddObject('DHeroBotAcupoints0_4', Pointer(@DHeroBotAcupoints0_4));
    Result.AddObject('DHeroLabelAcupoints0_0', Pointer(@DHeroLabelAcupoints0_0));
    Result.AddObject('DHeroLabelAcupoints0_1', Pointer(@DHeroLabelAcupoints0_1));
    Result.AddObject('DHeroLabelAcupoints0_2', Pointer(@DHeroLabelAcupoints0_2));
    Result.AddObject('DHeroLabelAcupoints0_3', Pointer(@DHeroLabelAcupoints0_3));
    Result.AddObject('DHeroLabelAcupoints0_4', Pointer(@DHeroLabelAcupoints0_4));
    Result.AddObject('DHeroMeridiansTabSheet1', Pointer(@DHeroMeridiansTabSheet1));
    Result.AddObject('DHeroMeridiansLine1', Pointer(@DHeroMeridiansLine1));
    Result.AddObject('DHeroBotAcupoints1_0', Pointer(@DHeroBotAcupoints1_0));
    Result.AddObject('DHeroBotAcupoints1_1', Pointer(@DHeroBotAcupoints1_1));
    Result.AddObject('DHeroBotAcupoints1_2', Pointer(@DHeroBotAcupoints1_2));
    Result.AddObject('DHeroBotAcupoints1_3', Pointer(@DHeroBotAcupoints1_3));
    Result.AddObject('DHeroBotAcupoints1_4', Pointer(@DHeroBotAcupoints1_4));
    Result.AddObject('DHeroLabelAcupoints1_0', Pointer(@DHeroLabelAcupoints1_0));
    Result.AddObject('DHeroLabelAcupoints1_1', Pointer(@DHeroLabelAcupoints1_1));
    Result.AddObject('DHeroLabelAcupoints1_2', Pointer(@DHeroLabelAcupoints1_2));
    Result.AddObject('DHeroLabelAcupoints1_3', Pointer(@DHeroLabelAcupoints1_3));
    Result.AddObject('DHeroLabelAcupoints1_4', Pointer(@DHeroLabelAcupoints1_4));
    Result.AddObject('DHeroLabelMeridiansStatus1', Pointer(@DHeroLabelMeridiansStatus1));
    Result.AddObject('DHeroMeridiansTabSheet2', Pointer(@DHeroMeridiansTabSheet2));
    Result.AddObject('DHeroMeridiansLine2', Pointer(@DHeroMeridiansLine2));
    Result.AddObject('DHeroBotAcupoints2_0', Pointer(@DHeroBotAcupoints2_0));
    Result.AddObject('DHeroBotAcupoints2_1', Pointer(@DHeroBotAcupoints2_1));
    Result.AddObject('DHeroBotAcupoints2_2', Pointer(@DHeroBotAcupoints2_2));
    Result.AddObject('DHeroBotAcupoints2_3', Pointer(@DHeroBotAcupoints2_3));
    Result.AddObject('DHeroBotAcupoints2_4', Pointer(@DHeroBotAcupoints2_4));
    Result.AddObject('DHeroLabelAcupoints2_0', Pointer(@DHeroLabelAcupoints2_0));
    Result.AddObject('DHeroLabelAcupoints2_1', Pointer(@DHeroLabelAcupoints2_1));
    Result.AddObject('DHeroLabelAcupoints2_2', Pointer(@DHeroLabelAcupoints2_2));
    Result.AddObject('DHeroLabelAcupoints2_3', Pointer(@DHeroLabelAcupoints2_3));
    Result.AddObject('DHeroLabelAcupoints2_4', Pointer(@DHeroLabelAcupoints2_4));
    Result.AddObject('DHeroLabelMeridiansStatus2', Pointer(@DHeroLabelMeridiansStatus2));
    Result.AddObject('DHeroMeridiansTabSheet3', Pointer(@DHeroMeridiansTabSheet3));
    Result.AddObject('DHeroMeridiansLine3', Pointer(@DHeroMeridiansLine3));
    Result.AddObject('DHeroBotAcupoints3_0', Pointer(@DHeroBotAcupoints3_0));
    Result.AddObject('DHeroBotAcupoints3_1', Pointer(@DHeroBotAcupoints3_1));
    Result.AddObject('DHeroBotAcupoints3_2', Pointer(@DHeroBotAcupoints3_2));
    Result.AddObject('DHeroBotAcupoints3_3', Pointer(@DHeroBotAcupoints3_3));
    Result.AddObject('DHeroBotAcupoints3_4', Pointer(@DHeroBotAcupoints3_4));
    Result.AddObject('DHeroLabelAcupoints3_0', Pointer(@DHeroLabelAcupoints3_0));
    Result.AddObject('DHeroLabelAcupoints3_1', Pointer(@DHeroLabelAcupoints3_1));
    Result.AddObject('DHeroLabelAcupoints3_2', Pointer(@DHeroLabelAcupoints3_2));
    Result.AddObject('DHeroLabelAcupoints3_3', Pointer(@DHeroLabelAcupoints3_3));
    Result.AddObject('DHeroLabelAcupoints3_4', Pointer(@DHeroLabelAcupoints3_4));
    Result.AddObject('DHeroLabelMeridiansStatus3', Pointer(@DHeroLabelMeridiansStatus3));
    Result.AddObject('DHeroMeridiansTabSheet4', Pointer(@DHeroMeridiansTabSheet4));
    Result.AddObject('DHeroMeridiansLine4', Pointer(@DHeroMeridiansLine4));
    Result.AddObject('DHeroBotAcupoints4_0', Pointer(@DHeroBotAcupoints4_0));
    Result.AddObject('DHeroBotAcupoints4_1', Pointer(@DHeroBotAcupoints4_1));
    Result.AddObject('DHeroBotAcupoints4_2', Pointer(@DHeroBotAcupoints4_2));
    Result.AddObject('DHeroBotAcupoints4_3', Pointer(@DHeroBotAcupoints4_3));
    Result.AddObject('DHeroBotAcupoints4_4', Pointer(@DHeroBotAcupoints4_4));
    Result.AddObject('DHeroLabelAcupoints4_0', Pointer(@DHeroLabelAcupoints4_0));
    Result.AddObject('DHeroLabelAcupoints4_1', Pointer(@DHeroLabelAcupoints4_1));
    Result.AddObject('DHeroLabelAcupoints4_2', Pointer(@DHeroLabelAcupoints4_2));
    Result.AddObject('DHeroLabelAcupoints4_3', Pointer(@DHeroLabelAcupoints4_3));
    Result.AddObject('DHeroLabelAcupoints4_4', Pointer(@DHeroLabelAcupoints4_4));
    Result.AddObject('DHeroLabelMeridiansStatus4', Pointer(@DHeroLabelMeridiansStatus4));
    Result.AddObject('DHeroTrainingMeridian', Pointer(@DHeroTrainingMeridian));
    Result.AddObject('DHeroNGStateTabSheet4', Pointer(@DHeroNGStateTabSheet4));
    Result.AddObject('DHeroStateNGContinuousMagic', Pointer(@DHeroStateNGContinuousMagic));
    Result.AddObject('DHeroContinuousMagicList', Pointer(@DHeroContinuousMagicList));
    Result.AddObject('DHeroStLJMagBack1', Pointer(@DHeroStLJMagBack1));
    Result.AddObject('DHeroStLJExpIcon1', Pointer(@DHeroStLJExpIcon1));
    Result.AddObject('DHeroStLJExpText1', Pointer(@DHeroStLJExpText1));
    Result.AddObject('DHeroStLJLblName1', Pointer(@DHeroStLJLblName1));
    Result.AddObject('DHeroStLJLvIcon1', Pointer(@DHeroStLJLvIcon1));
    Result.AddObject('DHeroStLJLvText1', Pointer(@DHeroStLJLvText1));
    Result.AddObject('DHeroStLJMag1', Pointer(@DHeroStLJMag1));
    Result.AddObject('DHeroStLJMagBack2', Pointer(@DHeroStLJMagBack2));
    Result.AddObject('DHeroStLJMag2', Pointer(@DHeroStLJMag2));
    Result.AddObject('DHeroStLJMagBack3', Pointer(@DHeroStLJMagBack3));
    Result.AddObject('DHeroStLJMag3', Pointer(@DHeroStLJMag3));
    Result.AddObject('DHeroStLJMagBack4', Pointer(@DHeroStLJMagBack4));
    Result.AddObject('DHeroStLJMag4', Pointer(@DHeroStLJMag4));
    Result.AddObject('DHeroSMB1', Pointer(@DHeroSMB1));
    Result.AddObject('DHeroSMB2', Pointer(@DHeroSMB2));
    Result.AddObject('DHeroSMB3', Pointer(@DHeroSMB3));
    Result.AddObject('DHeroSMB4', Pointer(@DHeroSMB4));
    Result.AddObject('HeroMainTabSheet3', Pointer(@HeroMainTabSheet3));
    Result.AddObject('DHeroXFPageControl', Pointer(@DHeroXFPageControl));
    Result.AddObject('HeroContinuousMagicMenu', Pointer(@HeroContinuousMagicMenu));
    Result.AddObject('DGodBlessDlgUS1', Pointer(@DGodBlessDlgUS1));
    Result.AddObject('DGodBless10US1', Pointer(@DGodBless10US1));
    Result.AddObject('DGodBless11US1', Pointer(@DGodBless11US1));
    Result.AddObject('DGodBless12US1', Pointer(@DGodBless12US1));
    Result.AddObject('DGodBless1US1', Pointer(@DGodBless1US1));
    Result.AddObject('DGodBless2US1', Pointer(@DGodBless2US1));
    Result.AddObject('DGodBless3US1', Pointer(@DGodBless3US1));
    Result.AddObject('DGodBless4US1', Pointer(@DGodBless4US1));
    Result.AddObject('DGodBless5US1', Pointer(@DGodBless5US1));
    Result.AddObject('DGodBless6US1', Pointer(@DGodBless6US1));
    Result.AddObject('DGodBless7US1', Pointer(@DGodBless7US1));
    Result.AddObject('DGodBless8US1', Pointer(@DGodBless8US1));
    Result.AddObject('DGodBless9US1', Pointer(@DGodBless9US1));
    Result.AddObject('DGodBlessCloseUS1', Pointer(@DGodBlessCloseUS1));
    Result.AddObject('DGodBlessInfoUS1', Pointer(@DGodBlessInfoUS1));
    Result.AddObject('DUSGodBlessCusBtn1', Pointer(@DUSGodBlessCusBtn1));
    Result.AddObject('DUSGodBlessCusBtn2', Pointer(@DUSGodBlessCusBtn2));
    Result.AddObject('DUSGodBlessCusBtn3', Pointer(@DUSGodBlessCusBtn3));
    Result.AddObject('DJewelryBoxDlgUS1', Pointer(@DJewelryBoxDlgUS1));
    Result.AddObject('DJewelry1US1', Pointer(@DJewelry1US1));
    Result.AddObject('DJewelry2US1', Pointer(@DJewelry2US1));
    Result.AddObject('DJewelry3US1', Pointer(@DJewelry3US1));
    Result.AddObject('DJewelry4US1', Pointer(@DJewelry4US1));
    Result.AddObject('DJewelry5US1', Pointer(@DJewelry5US1));
    Result.AddObject('DJewelry6US1', Pointer(@DJewelry6US1));
    Result.AddObject('DJewelryCloseUS1', Pointer(@DJewelryCloseUS1));
    Result.AddObject('DUSJewelryBoxCusBtn1', Pointer(@DUSJewelryBoxCusBtn1));
    Result.AddObject('DUSJewelryBoxCusBtn2', Pointer(@DUSJewelryBoxCusBtn2));
    Result.AddObject('DUSJewelryBoxCusBtn3', Pointer(@DUSJewelryBoxCusBtn3));
    Result.AddObject('DUserState1', Pointer(@DUserState1));
    Result.AddObject('DCloseUS1', Pointer(@DCloseUS1));
    Result.AddObject('DUserState1BasicPageControl', Pointer(@DUserState1BasicPageControl));
    Result.AddObject('DUserState1StateTabSheet1', Pointer(@DUserState1StateTabSheet1));
    Result.AddObject('DUserState1StateBasic', Pointer(@DUserState1StateBasic));
    Result.AddObject('DArmRingLUS1', Pointer(@DArmRingLUS1));
    Result.AddObject('DArmRingRUS1', Pointer(@DArmRingRUS1));
    Result.AddObject('DBeltUS1', Pointer(@DBeltUS1));
    Result.AddObject('DBootsUS1', Pointer(@DBootsUS1));
    Result.AddObject('DBujukUS1', Pointer(@DBujukUS1));
    Result.AddObject('DCharmUS1', Pointer(@DCharmUS1));
    Result.AddObject('DDressUS1', Pointer(@DDressUS1));
    Result.AddObject('DDrumUS1', Pointer(@DDrumUS1));
    Result.AddObject('DGodBlessUS1', Pointer(@DGodBlessUS1));
    Result.AddObject('DHelmetUS1', Pointer(@DHelmetUS1));
    Result.AddObject('DHorseUS1', Pointer(@DHorseUS1));
    Result.AddObject('DJadeUS1', Pointer(@DJadeUS1));
    Result.AddObject('DJewelryBoxUS1', Pointer(@DJewelryBoxUS1));
    Result.AddObject('DLightUS1', Pointer(@DLightUS1));
    Result.AddObject('DNecklaceUS1', Pointer(@DNecklaceUS1));
    Result.AddObject('DRingLUS1', Pointer(@DRingLUS1));
    Result.AddObject('DRingRUS1', Pointer(@DRingRUS1));
    Result.AddObject('DShieldUS1', Pointer(@DShieldUS1));
    Result.AddObject('DUSCustomButton1', Pointer(@DUSCustomButton1));
    Result.AddObject('DUSCustomButton2', Pointer(@DUSCustomButton2));
    Result.AddObject('DUSCustomButton3', Pointer(@DUSCustomButton3));
    Result.AddObject('DUserState1LabelDStateWinRankName', Pointer(@DUserState1LabelDStateWinRankName));
    Result.AddObject('DWeaponUS1', Pointer(@DWeaponUS1));
    Result.AddObject('DUserState1StateTabSheet2', Pointer(@DUserState1StateTabSheet2));
    Result.AddObject('DUserState1StateFashion', Pointer(@DUserState1StateFashion));
    Result.AddObject('DFashionArmRingLUS1', Pointer(@DFashionArmRingLUS1));
    Result.AddObject('DFashionArmRingRUS1', Pointer(@DFashionArmRingRUS1));
    Result.AddObject('DFashionBeltUS1', Pointer(@DFashionBeltUS1));
    Result.AddObject('DFashionBootsUS1', Pointer(@DFashionBootsUS1));
    Result.AddObject('DFashionDressUS1', Pointer(@DFashionDressUS1));
    Result.AddObject('DFashionDrumUS1', Pointer(@DFashionDrumUS1));
    Result.AddObject('DFashionHelmetUS1', Pointer(@DFashionHelmetUS1));
    Result.AddObject('DFashionLightUS1', Pointer(@DFashionLightUS1));
    Result.AddObject('DFashionNecklaceUS1', Pointer(@DFashionNecklaceUS1));
    Result.AddObject('DFashionRingLUS1', Pointer(@DFashionRingLUS1));
    Result.AddObject('DFashionRingRUS1', Pointer(@DFashionRingRUS1));
    Result.AddObject('DFashionWeaponUS1', Pointer(@DFashionWeaponUS1));
    Result.AddObject('DUSFashionCustomButton1', Pointer(@DUSFashionCustomButton1));
    Result.AddObject('DUSFashionCustomButton2', Pointer(@DUSFashionCustomButton2));
    Result.AddObject('DUSFashionCustomButton3', Pointer(@DUSFashionCustomButton3));
    Result.AddObject('DUserState1StateTabSheet5', Pointer(@DUserState1StateTabSheet5));
    Result.AddObject('DUserState1StateTitle', Pointer(@DUserState1StateTitle));
    Result.AddObject('DUSTitleActive', Pointer(@DUSTitleActive));
    Result.AddObject('DUSTitleButton1', Pointer(@DUSTitleButton1));
    Result.AddObject('DUSTitleButton2', Pointer(@DUSTitleButton2));
    Result.AddObject('DUSTitleButton3', Pointer(@DUSTitleButton3));
    Result.AddObject('DUSTitleButton4', Pointer(@DUSTitleButton4));
    Result.AddObject('DUSTitleButton5', Pointer(@DUSTitleButton5));
    Result.AddObject('DUSTitleButton6', Pointer(@DUSTitleButton6));
    Result.AddObject('DUSTitleCustomBtn1', Pointer(@DUSTitleCustomBtn1));
    Result.AddObject('DUSTitleCustomBtn2', Pointer(@DUSTitleCustomBtn2));
    Result.AddObject('DUSTitleCustomBtn3', Pointer(@DUSTitleCustomBtn3));
    Result.AddObject('DUSTitleItemDesc', Pointer(@DUSTitleItemDesc));
    Result.AddObject('DUSTitleName1', Pointer(@DUSTitleName1));
    Result.AddObject('DUSTitleName2', Pointer(@DUSTitleName2));
    Result.AddObject('DUSTitleName3', Pointer(@DUSTitleName3));
    Result.AddObject('DUSTitleName4', Pointer(@DUSTitleName4));
    Result.AddObject('DUSTitleName5', Pointer(@DUSTitleName5));
    Result.AddObject('DUSTitleName6', Pointer(@DUSTitleName6));
    Result.AddObject('DUSTitleNameActive', Pointer(@DUSTitleNameActive));
    Result.AddObject('DUSTitlePageDown', Pointer(@DUSTitlePageDown));
    Result.AddObject('DUSTitlePageUp', Pointer(@DUSTitlePageUp));
    Result.AddObject('DUserState1LabelDStateWinCharName', Pointer(@DUserState1LabelDStateWinCharName));  
end;

procedure TStateWindows.LoadFromStream(MemoryStream:TMemoryStream);
var
  Index:Integer;
  ControlAddrList:THashedStringList;
  msDefaultUI:TMemoryStream;
begin
  msDefaultUI := LoadDxControlEx.LoadCompressedUIData('STATE_WIN_UI', 'ZDAT'); //必须存在，否则报错
  ControlAddrList := Self.MakeControlAddressList;
  try
     LoadDxControlEx.LoadControlFromStream(MemoryStream, FrmDlg.DBackground, ControlAddrList, 'StateWin');
     LoadDxControlEx.PatchLoadControlFromStream(msDefaultUI, FrmDlg.DBackground, ControlAddrList, 'StateWin');
  finally
     msDefaultUI.Free;
     ControlAddrList.Free;
  end;

  Initialized := True;
  Initialize;

  DSTitleButton6.Visible := g_ClientVersion <> cvMirNewUI205;
  DSTitleName6.Visible := g_ClientVersion <> cvMirNewUI205;

  DSHeroTitleButton6.Visible := g_ClientVersion <> cvMirNewUI205;
  DSHeroTitleName6.Visible := g_ClientVersion <> cvMirNewUI205;

  DUSTitleButton6.Visible := g_ClientVersion <> cvMirNewUI205;
  DUSTitleName6.Visible := g_ClientVersion <> cvMirNewUI205;

  FJewelryBoxUpImageIndex := DSWJewelryBox.ImageIndex.Up;
  FJewelryBoxDownImageIndex := DSWJewelryBox.ImageIndex.Down;

  FHeroJewelryBoxUpImageIndex := DHeroSWJewelryBox.ImageIndex.Up;
  FHeroJewelryBoxDownImageIndex := DHeroSWJewelryBox.ImageIndex.Down;

  FLableCaption_BigGold := Trim(DStateBigGoldLabel.Caption);
  if FLableCaption_BigGold <> '' then begin
    for Index := Length(FLableCaption_BigGold) downto 1 do begin
      if not (FLableCaption_BigGold[Index] in ['0'..'9']) then begin
        FLableCaption_BigGold := Copy(FLableCaption_BigGold, 1, Index);
        Break;
      end;
    end;
  end;

  FLabelCaption_AC := DStateLabelAC.Caption;
  FLabelCaption_MAC := DStateLabelMAC.Caption;
  FLabelCaption_DC := DStateLabelDC.Caption;
  FLabelCaption_MC := DStateLabelMC.Caption;
  FLabelCaption_SC := DStateLabelSC.Caption;
  FLabelCaption_Weight := DStateWeightLabel.Caption;
  FLabelCaption_WearWeight := DStateWearWeightLabel.Caption;
  FLabelCaption_HandWeight := DStateHandWeightLabel.Caption;
  FLabelCaption_AntiMagic := DStateAntiMagicLabel.Caption;
  FLabelCaption_AntiPoison := DStateAntiPoisonLabel.Caption;
  FLabelCaption_HealthRecover := DStateHealthRecoverLabel.Caption;
  FLabelCaption_SpellRecover := DStateSpellRecoverLabel.Caption;
  FLabelCaption_HitSpeed := DStateHitSpeedLabel.Caption;
  FLabelCaption_HitPoint := DStateHitPointLabel.Caption;
  FLabelCaption_SpeedPoint := DStateSpeedPointLabel.Caption;
  FLabelCaption_PoisonRecover := DStatePoisonRecoverLabel.Caption;

  FLabelCaption_HeroAC := DHeroStateLabelAC.Caption;
  FLabelCaption_HeroMAC := DHeroStateLabelMAC.Caption;
  FLabelCaption_HeroDC := DHeroStateLabelDC.Caption;
  FLabelCaption_HeroMC := DHeroStateLabelMC.Caption;
  FLabelCaption_HeroSC := DHeroStateLabelSC.Caption;
  FLabelCaption_HeroWeight := DHeroStateWeightLabel.Caption;
  FLabelCaption_HeroWearWeight := DHeroStateWearWeightLabel.Caption;
  FLabelCaption_HeroHandWeight := DHeroStateHandWeightLabel.Caption;
  FLabelCaption_HeroAntiMagic := DHeroStateAntiMagicLabel.Caption;
  FLabelCaption_HeroAntiPoison := DHeroStateAntiPoisonLabel.Caption;
  FLabelCaption_HeroHealthRecover := DHeroStateHealthRecoverLabel.Caption;
  FLabelCaption_HeroSpellRecover := DHeroStateSpellRecoverLabel.Caption;
  FLabelCaption_HeroHitSpeed := DHeroStateHitSpeedLabel.Caption;
  FLabelCaption_HeroHitPoint := DHeroStateHitPointLabel.Caption;
  FLabelCaption_HeroSpeedPoint := DHeroStateSpeedPointLabel.Caption;
  FLabelCaption_HeroPoisonRecover := DHeroStatePoisonRecoverLabel.Caption;

  // 装备
  DSWCustomButton1.Tag := 60;
  DSWCustomButton2.Tag := 61;
  DSWCustomButton3.Tag := 62;

  DSWCustomButton1.OnClick := FrmDlg.DCustomButtonClick;
  DSWCustomButton2.OnClick := FrmDlg.DCustomButtonClick;
  DSWCustomButton3.OnClick := FrmDlg.DCustomButtonClick;

  DSWCustomButton1.OnMouseMove := DControlMouseMoveShowHint;
  DSWCustomButton2.OnMouseMove := DControlMouseMoveShowHint;
  DSWCustomButton3.OnMouseMove := DControlMouseMoveShowHint;

  // 时装
  DSWFashionCustomButton1.Tag := 70;
  DSWFashionCustomButton2.Tag := 71;
  DSWFashionCustomButton3.Tag := 72;

  DSWFashionCustomButton1.OnClick := FrmDlg.DCustomButtonClick;
  DSWFashionCustomButton2.OnClick := FrmDlg.DCustomButtonClick;
  DSWFashionCustomButton3.OnClick := FrmDlg.DCustomButtonClick;

  DSWFashionCustomButton1.OnMouseMove := DControlMouseMoveShowHint;
  DSWFashionCustomButton2.OnMouseMove := DControlMouseMoveShowHint;
  DSWFashionCustomButton3.OnMouseMove := DControlMouseMoveShowHint;

  // 状态
  DStateCustomButton1.Tag := 80;
  DStateCustomButton2.Tag := 81;
  DStateCustomButton3.Tag := 82;

  DStateCustomButton1.OnClick := FrmDlg.DCustomButtonClick;
  DStateCustomButton2.OnClick := FrmDlg.DCustomButtonClick;
  DStateCustomButton3.OnClick := FrmDlg.DCustomButtonClick;

  DStateCustomButton1.OnMouseMove := DControlMouseMoveShowHint;
  DStateCustomButton2.OnMouseMove := DControlMouseMoveShowHint;
  DStateCustomButton3.OnMouseMove := DControlMouseMoveShowHint;

  // 属性
  DAbilCustomButton1.Tag := 90;
  DAbilCustomButton2.Tag := 91;
  DAbilCustomButton3.Tag := 92;

  DAbilCustomButton1.OnClick := FrmDlg.DCustomButtonClick;
  DAbilCustomButton2.OnClick := FrmDlg.DCustomButtonClick;
  DAbilCustomButton3.OnClick := FrmDlg.DCustomButtonClick;

  DAbilCustomButton1.OnMouseMove := DControlMouseMoveShowHint;
  DAbilCustomButton2.OnMouseMove := DControlMouseMoveShowHint;
  DAbilCustomButton3.OnMouseMove := DControlMouseMoveShowHint;

  // 称号
  DTitleCustomButton1.Tag := 100;
  DTitleCustomButton2.Tag := 101;
  DTitleCustomButton3.Tag := 102;

  DTitleCustomButton1.OnClick := FrmDlg.DCustomButtonClick;
  DTitleCustomButton2.OnClick := FrmDlg.DCustomButtonClick;
  DTitleCustomButton3.OnClick := FrmDlg.DCustomButtonClick;

  DTitleCustomButton1.OnMouseMove := DControlMouseMoveShowHint;
  DTitleCustomButton2.OnMouseMove := DControlMouseMoveShowHint;
  DTitleCustomButton3.OnMouseMove := DControlMouseMoveShowHint;

  // 技能
  DMagCustomButton1.Tag := 110;
  DMagCustomButton2.Tag := 111;
  DMagCustomButton3.Tag := 112;

  DMagCustomButton1.OnClick := FrmDlg.DCustomButtonClick;
  DMagCustomButton2.OnClick := FrmDlg.DCustomButtonClick;
  DMagCustomButton3.OnClick := FrmDlg.DCustomButtonClick;

  DMagCustomButton1.OnMouseMove := DControlMouseMoveShowHint;
  DMagCustomButton2.OnMouseMove := DControlMouseMoveShowHint;
  DMagCustomButton3.OnMouseMove := DControlMouseMoveShowHint;

  // 出战
  DBotStateCustomButton1.Tag := 120;
  DBotStateCustomButton2.Tag := 121;
  DBotStateCustomButton3.Tag := 122;
  DBotStateCustomButton4.Tag := 123;
  DBotStateCustomButton5.Tag := 124;
  DBotStateCustomButton6.Tag := 125;
  DBotStateCustomButton7.Tag := 126;
  DBotStateCustomButton8.Tag := 127;
  DBotStateCustomButton9.Tag := 128;
  DBotStateCustomButton10.Tag := 129;

  DBotStateCustomButton1.OnClick := FrmDlg.DCustomButtonClick;
  DBotStateCustomButton2.OnClick := FrmDlg.DCustomButtonClick;
  DBotStateCustomButton3.OnClick := FrmDlg.DCustomButtonClick;
  DBotStateCustomButton4.OnClick := FrmDlg.DCustomButtonClick;
  DBotStateCustomButton5.OnClick := FrmDlg.DCustomButtonClick;
  DBotStateCustomButton6.OnClick := FrmDlg.DCustomButtonClick;
  DBotStateCustomButton7.OnClick := FrmDlg.DCustomButtonClick;
  DBotStateCustomButton8.OnClick := FrmDlg.DCustomButtonClick;
  DBotStateCustomButton9.OnClick := FrmDlg.DCustomButtonClick;
  DBotStateCustomButton10.OnClick := FrmDlg.DCustomButtonClick;

  DBotStateCustomButton1.OnMouseMove := DControlMouseMoveShowHint;
  DBotStateCustomButton2.OnMouseMove := DControlMouseMoveShowHint;
  DBotStateCustomButton3.OnMouseMove := DControlMouseMoveShowHint;
  DBotStateCustomButton4.OnMouseMove := DControlMouseMoveShowHint;
  DBotStateCustomButton5.OnMouseMove := DControlMouseMoveShowHint;
  DBotStateCustomButton6.OnMouseMove := DControlMouseMoveShowHint;
  DBotStateCustomButton7.OnMouseMove := DControlMouseMoveShowHint;
  DBotStateCustomButton8.OnMouseMove := DControlMouseMoveShowHint;
  DBotStateCustomButton9.OnMouseMove := DControlMouseMoveShowHint;
  DBotStateCustomButton10.OnMouseMove := DControlMouseMoveShowHint;

  FStateWeightLabelColor.Up := DStateWeightLabel.CaptionColor.Up.Color;
  FStateWeightLabelColor.Hot := DStateWeightLabel.CaptionColor.Hot.Color;
  FStateWeightLabelColor.Down := DStateWeightLabel.CaptionColor.Down.Color;
  FStateWeightLabelColor.Disabled := DStateWeightLabel.CaptionColor.Disabled.Color;

  FStateWearWeightLabelColor.Up := DStateWearWeightLabel.CaptionColor.Up.Color;
  FStateWearWeightLabelColor.Hot := DStateWearWeightLabel.CaptionColor.Hot.Color;
  FStateWearWeightLabelColor.Down := DStateWearWeightLabel.CaptionColor.Down.Color;
  FStateWearWeightLabelColor.Disabled := DStateWearWeightLabel.CaptionColor.Disabled.Color;

  FStateHandWeightLabelColor.Up := DStateHandWeightLabel.CaptionColor.Up.Color;
  FStateHandWeightLabelColor.Hot := DStateHandWeightLabel.CaptionColor.Hot.Color;
  FStateHandWeightLabelColor.Down := DStateHandWeightLabel.CaptionColor.Down.Color;
  FStateHandWeightLabelColor.Disabled := DStateHandWeightLabel.CaptionColor.Disabled.Color;

  // 神佑袋
  DSWGodBlessCusBtn1.Tag := 130;
  DSWGodBlessCusBtn2.Tag := 131;
  DSWGodBlessCusBtn3.Tag := 132;

  DSWGodBlessCusBtn1.OnClick := FrmDlg.DCustomButtonClick;
  DSWGodBlessCusBtn2.OnClick := FrmDlg.DCustomButtonClick;
  DSWGodBlessCusBtn3.OnClick := FrmDlg.DCustomButtonClick;

  DSWGodBlessCusBtn1.OnMouseMove := DControlMouseMoveShowHint;
  DSWGodBlessCusBtn2.OnMouseMove := DControlMouseMoveShowHint;
  DSWGodBlessCusBtn3.OnMouseMove := DControlMouseMoveShowHint;

  // 首饰盒
  DSWJewelryBoxCusBtn1.Tag := 140;
  DSWJewelryBoxCusBtn2.Tag := 141;
  DSWJewelryBoxCusBtn3.Tag := 142;

  DSWJewelryBoxCusBtn1.OnClick := FrmDlg.DCustomButtonClick;
  DSWJewelryBoxCusBtn2.OnClick := FrmDlg.DCustomButtonClick;
  DSWJewelryBoxCusBtn3.OnClick := FrmDlg.DCustomButtonClick;

  DSWJewelryBoxCusBtn1.OnMouseMove := DControlMouseMoveShowHint;
  DSWJewelryBoxCusBtn2.OnMouseMove := DControlMouseMoveShowHint;
  DSWJewelryBoxCusBtn3.OnMouseMove := DControlMouseMoveShowHint;

  // 内功状态
  DStateNGCusBtn1.Tag := 300;
  DStateNGCusBtn2.Tag := 301;
  DStateNGCusBtn3.Tag := 302;

  DStateNGCusBtn1.OnClick := FrmDlg.DCustomButtonClick;
  DStateNGCusBtn2.OnClick := FrmDlg.DCustomButtonClick;
  DStateNGCusBtn3.OnClick := FrmDlg.DCustomButtonClick;

  DStateNGCusBtn1.OnMouseMove := DControlMouseMoveShowHint;
  DStateNGCusBtn2.OnMouseMove := DControlMouseMoveShowHint;
  DStateNGCusBtn3.OnMouseMove := DControlMouseMoveShowHint;

  // 内功技能
  DStateNGMagicCusBnt1.Tag := 310;
  DStateNGMagicCusBnt2.Tag := 311;
  DStateNGMagicCusBnt3.Tag := 312;
  DStateNGMagicCusBnt1.BringToFront;
  DStateNGMagicCusBnt2.BringToFront;
  DStateNGMagicCusBnt3.BringToFront;

  DStateNGMagicCusBnt1.OnClick := FrmDlg.DCustomButtonClick;
  DStateNGMagicCusBnt2.OnClick := FrmDlg.DCustomButtonClick;
  DStateNGMagicCusBnt3.OnClick := FrmDlg.DCustomButtonClick;

  DStateNGMagicCusBnt1.OnMouseMove := DControlMouseMoveShowHint;
  DStateNGMagicCusBnt2.OnMouseMove := DControlMouseMoveShowHint;
  DStateNGMagicCusBnt3.OnMouseMove := DControlMouseMoveShowHint;

  // 内功技能
  DStateNGMeridiansCusBtn1.Tag := 320;
  DStateNGMeridiansCusBtn2.Tag := 321;
  DStateNGMeridiansCusBtn3.Tag := 322;

  DStateNGMeridiansCusBtn1.OnClick := FrmDlg.DCustomButtonClick;
  DStateNGMeridiansCusBtn2.OnClick := FrmDlg.DCustomButtonClick;
  DStateNGMeridiansCusBtn3.OnClick := FrmDlg.DCustomButtonClick;

  DStateNGMeridiansCusBtn1.OnMouseMove := DControlMouseMoveShowHint;
  DStateNGMeridiansCusBtn2.OnMouseMove := DControlMouseMoveShowHint;
  DStateNGMeridiansCusBtn3.OnMouseMove := DControlMouseMoveShowHint;

  // 内功连击
  DStateNGMagic2CusBtn1.Tag := 330;
  DStateNGMagic2CusBtn2.Tag := 331;
  DStateNGMagic2CusBtn3.Tag := 332;

  DStateNGMagic2CusBtn1.OnClick := FrmDlg.DCustomButtonClick;
  DStateNGMagic2CusBtn2.OnClick := FrmDlg.DCustomButtonClick;
  DStateNGMagic2CusBtn3.OnClick := FrmDlg.DCustomButtonClick;

  DStateNGMagic2CusBtn1.OnMouseMove := DControlMouseMoveShowHint;
  DStateNGMagic2CusBtn2.OnMouseMove := DControlMouseMoveShowHint;
  DStateNGMagic2CusBtn3.OnMouseMove := DControlMouseMoveShowHint;

  DUSCustomButton1.Tag := 800;
  DUSCustomButton2.Tag := 801;
  DUSCustomButton3.Tag := 802;

  DUSCustomButton1.OnClick := FrmDlg.DCustomButtonClick;
  DUSCustomButton2.OnClick := FrmDlg.DCustomButtonClick;
  DUSCustomButton3.OnClick := FrmDlg.DCustomButtonClick;

  DUSCustomButton1.OnMouseMove := DControlMouseMoveShowHint;
  DUSCustomButton2.OnMouseMove := DControlMouseMoveShowHint;
  DUSCustomButton3.OnMouseMove := DControlMouseMoveShowHint;

  DUSFashionCustomButton1.Tag := 810;
  DUSFashionCustomButton2.Tag := 811;
  DUSFashionCustomButton3.Tag := 812;

  DUSFashionCustomButton1.OnClick := FrmDlg.DCustomButtonClick;
  DUSFashionCustomButton2.OnClick := FrmDlg.DCustomButtonClick;
  DUSFashionCustomButton3.OnClick := FrmDlg.DCustomButtonClick;

  DUSFashionCustomButton1.OnMouseMove := DControlMouseMoveShowHint;
  DUSFashionCustomButton2.OnMouseMove := DControlMouseMoveShowHint;
  DUSFashionCustomButton3.OnMouseMove := DControlMouseMoveShowHint;

  DUSTitleCustomBtn1.Tag := 820;
  DUSTitleCustomBtn2.Tag := 821;
  DUSTitleCustomBtn3.Tag := 822;

  DUSTitleCustomBtn1.OnClick := FrmDlg.DCustomButtonClick;
  DUSTitleCustomBtn2.OnClick := FrmDlg.DCustomButtonClick;
  DUSTitleCustomBtn3.OnClick := FrmDlg.DCustomButtonClick;

  DUSTitleCustomBtn1.OnMouseMove := DControlMouseMoveShowHint;
  DUSTitleCustomBtn2.OnMouseMove := DControlMouseMoveShowHint;
  DUSTitleCustomBtn3.OnMouseMove := DControlMouseMoveShowHint;

  DUSGodBlessCusBtn1.tag := 830;
  DUSGodBlessCusBtn2.tag := 831;
  DUSGodBlessCusBtn3.tag := 832;

  DUSGodBlessCusBtn1.OnClick := FrmDlg.DCustomButtonClick;
  DUSGodBlessCusBtn2.OnClick := FrmDlg.DCustomButtonClick;
  DUSGodBlessCusBtn3.OnClick := FrmDlg.DCustomButtonClick;

  DUSGodBlessCusBtn1.OnMouseMove := DControlMouseMoveShowHint;
  DUSGodBlessCusBtn2.OnMouseMove := DControlMouseMoveShowHint;
  DUSGodBlessCusBtn3.OnMouseMove := DControlMouseMoveShowHint;

  DUSJewelryBoxCusBtn1.Tag := 840;
  DUSJewelryBoxCusBtn2.Tag := 841;
  DUSJewelryBoxCusBtn3.Tag := 842;

  DUSJewelryBoxCusBtn1.OnClick := FrmDlg.DCustomButtonClick;
  DUSJewelryBoxCusBtn2.OnClick := FrmDlg.DCustomButtonClick;
  DUSJewelryBoxCusBtn3.OnClick := FrmDlg.DCustomButtonClick;

  DUSJewelryBoxCusBtn1.OnMouseMove := DControlMouseMoveShowHint;
  DUSJewelryBoxCusBtn2.OnMouseMove := DControlMouseMoveShowHint;
  DUSJewelryBoxCusBtn3.OnMouseMove := DControlMouseMoveShowHint;

  DSWPetsCusBtn1.Tag := 850;
  DSWPetsCusBtn2.Tag := 851;
  DSWPetsCusBtn3.Tag := 852;
  DSWPetsCusBtn4.Tag := 853;
  DSWPetsCusBtn5.Tag := 854;
  DSWPetsCusBtn6.Tag := 855;
  DSWPetsCusBtn7.Tag := 856;
  DSWPetsCusBtn8.Tag := 857;
  DSWPetsCusBtn9.Tag := 858;

  DSWPetsCusBtn1.OnClick := FrmDlg.DCustomButtonClick;
  DSWPetsCusBtn2.OnClick := FrmDlg.DCustomButtonClick;
  DSWPetsCusBtn3.OnClick := FrmDlg.DCustomButtonClick;
  DSWPetsCusBtn4.OnClick := FrmDlg.DCustomButtonClick;
  DSWPetsCusBtn5.OnClick := FrmDlg.DCustomButtonClick;
  DSWPetsCusBtn6.OnClick := FrmDlg.DCustomButtonClick;
  DSWPetsCusBtn7.OnClick := FrmDlg.DCustomButtonClick;
  DSWPetsCusBtn8.OnClick := FrmDlg.DCustomButtonClick;
  DSWPetsCusBtn9.OnClick := FrmDlg.DCustomButtonClick;

  DSWPetsCusBtn1.OnMouseMove := DControlMouseMoveShowHint;
  DSWPetsCusBtn2.OnMouseMove := DControlMouseMoveShowHint;
  DSWPetsCusBtn3.OnMouseMove := DControlMouseMoveShowHint;
  DSWPetsCusBtn4.OnMouseMove := DControlMouseMoveShowHint;
  DSWPetsCusBtn5.OnMouseMove := DControlMouseMoveShowHint;
  DSWPetsCusBtn6.OnMouseMove := DControlMouseMoveShowHint;
  DSWPetsCusBtn7.OnMouseMove := DControlMouseMoveShowHint;
  DSWPetsCusBtn8.OnMouseMove := DControlMouseMoveShowHint;
  DSWPetsCusBtn9.OnMouseMove := DControlMouseMoveShowHint;

end;

procedure TStateWindows.Close;
begin
  if not Initialized then Exit;
  MagicIndex := 0;
  MagicNGIndex := 0;
  HeroMagicIndex := 0;
  HeroMagicNGIndex := 0;
  FengHaoIndex := 0;
  USFengHaoIndex := 0;
  HeroFengHaoIndex := 0;
  DStateWin.Visible := False;
  DUserState1.Visible := False;
  DHeroStateWin.Visible := False;
  DStateWinEx.Visible := False;

  FengHaoHintWindow.Clear;

  DSWJewelryBoxDlg.Visible := False;
  DHeroSWJewelryBoxDlg.Visible := False;
  DJewelryBoxDlgUS1.Visible := False;

  DSWGodBlessDlg.Visible := False;
  DHeroSWGodBlessDlg.Visible := False;
  DGodBlessDlgUS1.Visible := False;

  g_CurrentRecallGamePetIndex := -1;
  DSWPetsDlg.Visible := False;
  DSWPetsBagDlg.Visible := False;
end;

procedure TStateWindows.UpDate;
begin
  if not Initialized then Exit;
  if g_MySelf <> nil then
    DStateWin.Update;

  if g_MyHero <> nil then
    DHeroStateWin.Update;
end;

procedure TStateWindows.InitSelf;
var
  I:Integer;
begin
  DStateWin.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateBasic.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateFashion.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateInfo.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateAbil.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateTitle.OnMouseMove := DStateTitleWinMouseMove; //FrmDlg.DStateWinMouseMove;
  DStateMagic.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStMagBack6.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStMagBack5.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateDeputyHero.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateNGInfo.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateNGMagic.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStNGMagBack6.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStNGMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStNGMagBack5.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStNGMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStNGMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStNGMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateNGMeridians.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DMeridiansLine0.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DMeridiansLine1.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DMeridiansLine2.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DMeridiansLine3.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DMeridiansLine4.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStateNGContinuousMagic.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DContinuousMagicList.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStLJMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStLJMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStLJMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DStLJMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStMagBtn1.Tag := 0;
  DStMagBtn1.Visible := False;
  DStMagBtn2.Tag := 0;
  DStMagBtn2.Visible := False;
  DStMagBtn3.Tag := 0;
  DStMagBtn3.Visible := False;
  DStMagBtn4.Tag := 0;
  DStMagBtn4.Visible := False;
  DStMagBtn5.Tag := 0;
  DStMagBtn5.Visible := False;
  DStMagBtn6.Tag := 0;
  DStMagBtn6.Visible := False;

  DStMagBtn1.OnClick := FrmDlg.DStUpgradeMagicButtonClick;
  DStMagBtn2.OnClick := FrmDlg.DStUpgradeMagicButtonClick;
  DStMagBtn3.OnClick := FrmDlg.DStUpgradeMagicButtonClick;
  DStMagBtn4.OnClick := FrmDlg.DStUpgradeMagicButtonClick;
  DStMagBtn5.OnClick := FrmDlg.DStUpgradeMagicButtonClick;
  DStMagBtn6.OnClick := FrmDlg.DStUpgradeMagicButtonClick;

  DStMagBtn1.OnMouseMove := FrmDlg.DStUpgradeMagicMouseMove;
  DStMagBtn2.OnMouseMove := FrmDlg.DStUpgradeMagicMouseMove;
  DStMagBtn3.OnMouseMove := FrmDlg.DStUpgradeMagicMouseMove;
  DStMagBtn4.OnMouseMove := FrmDlg.DStUpgradeMagicMouseMove;
  DStMagBtn5.OnMouseMove := FrmDlg.DStUpgradeMagicMouseMove;
  DStMagBtn6.OnMouseMove := FrmDlg.DStUpgradeMagicMouseMove;

  DStateRefurbishLabel.OnClick := FrmDlg.DStateRefurbishLabelClick;

  {------------------------------------------------------------------------------}
  if (not g_ConfigClient.boCustomUI) then begin
    DStateWin.Left := SCREENWIDTH - DStateWin.Width;
    DStateWin.Top := 0;
  end;

  MainTabSheet1.TabOrder := 0;
  MainTabSheet2.TabOrder := 1;
  MainTabSheet3.TabOrder := 2;

  DStateTabSheet1.TabOrder := 0;
  DStateTabSheet2.TabOrder := 1;
  DStateTabSheet3.TabOrder := 2;
  DStateTabSheet4.TabOrder := 3;
  DStateTabSheet5.TabOrder := 4;
  DStateTabSheet6.TabOrder := 5;
  DStateTabSheet7.TabOrder := 6;

  DNGStateTabSheet1.TabOrder := 0;
  DNGStateTabSheet2.TabOrder := 1;
  DNGStateTabSheet3.TabOrder := 2;
  DNGStateTabSheet4.TabOrder := 3;

  HeroMainTabSheet1.TabOrder := 0;
  HeroMainTabSheet2.TabOrder := 1;
  HeroMainTabSheet3.TabOrder := 2;

  DHeroStateTabSheet1.TabOrder := 0;
  DHeroStateTabSheet2.TabOrder := 1;
  DHeroStateTabSheet3.TabOrder := 2;
  DHeroStateTabSheet4.TabOrder := 3;
  DHeroStateTabSheet5.TabOrder := 4;

  DHeroNGStateTabSheet1.TabOrder := 0;
  DHeroNGStateTabSheet2.TabOrder := 1;
  DHeroNGStateTabSheet3.TabOrder := 2;
  DHeroNGStateTabSheet4.TabOrder := 3;

  DUserState1StateTabSheet1.TabOrder := 0;
  DUserState1StateTabSheet2.TabOrder := 1;
  DUserState1StateTabSheet5.TabOrder := 2;

  DStatePageControl.ActivePageIndex := 0;
  DBasicPageControl.ActivePageIndex := 0;
  DBasicPageControl.ActivePageIndex := 0;

  DMeridiansPageControl.ActivePageIndex := 1;
  DBotMeridians1.Checked := True;

  DCloseState.ClickCount := csNorm;
  DCloseState.OnClickSound := FrmDlg.DLoginNewClickSound;
  DCloseState.OnClick := DCloseStateClick;

  {--------------------------------------------------------------------------}
  MeridiansFormArray[0] := DMeridiansLine1;
  MeridiansFormArray[1] := DMeridiansLine2;
  MeridiansFormArray[2] := DMeridiansLine3;
  MeridiansFormArray[3] := DMeridiansLine4;
  MeridiansFormArray[4] := DMeridiansLine0;

  DBotMeridians0.OnClick := DBotMeridiansClick;
  DBotMeridians1.OnClick := DBotMeridiansClick;
  DBotMeridians2.OnClick := DBotMeridiansClick;
  DBotMeridians3.OnClick := DBotMeridiansClick;
  DBotMeridians4.OnClick := DBotMeridiansClick;

  DBotMeridians0.Tag := 0;
  DBotMeridians1.Tag := 1;
  DBotMeridians2.Tag := 2;
  DBotMeridians3.Tag := 3;
  DBotMeridians4.Tag := 4;

  for I := 0 to Length(MeridiansFormArray) - 1 do begin
    MeridiansFormArray[I].OnStartSubPaint := DMeridiansLineDirectPaint;
    MeridiansFormArray[I].Tag := I;
  end;

  DTrainingMeridian.OnClick := DTrainingMeridianClick; // 修炼经脉按钮
  {---------------------------------奇经-----------------------------------------}
  AcupointArray[0, 0] := DBotAcupoints1_0;
  AcupointArray[0, 1] := DBotAcupoints1_1;
  AcupointArray[0, 2] := DBotAcupoints1_2;
  AcupointArray[0, 3] := DBotAcupoints1_3;
  AcupointArray[0, 4] := DBotAcupoints1_4;

  AcupointArray[1, 0] := DBotAcupoints2_0;
  AcupointArray[1, 1] := DBotAcupoints2_1;
  AcupointArray[1, 2] := DBotAcupoints2_2;
  AcupointArray[1, 3] := DBotAcupoints2_3;
  AcupointArray[1, 4] := DBotAcupoints2_4;

  AcupointArray[2, 0] := DBotAcupoints3_0;
  AcupointArray[2, 1] := DBotAcupoints3_1;
  AcupointArray[2, 2] := DBotAcupoints3_2;
  AcupointArray[2, 3] := DBotAcupoints3_3;
  AcupointArray[2, 4] := DBotAcupoints3_4;

  AcupointArray[3, 0] := DBotAcupoints4_0;
  AcupointArray[3, 1] := DBotAcupoints4_1;
  AcupointArray[3, 2] := DBotAcupoints4_2;
  AcupointArray[3, 3] := DBotAcupoints4_3;
  AcupointArray[3, 4] := DBotAcupoints4_4;

  AcupointArray[4, 0] := DBotAcupoints0_0;
  AcupointArray[4, 1] := DBotAcupoints0_1;
  AcupointArray[4, 2] := DBotAcupoints0_2;
  AcupointArray[4, 3] := DBotAcupoints0_3;
  AcupointArray[4, 4] := DBotAcupoints0_4;

  DBotAcupoints0_0.OnClick := DBotAcupointsClick;
  DBotAcupoints0_1.OnClick := DBotAcupointsClick;
  DBotAcupoints0_2.OnClick := DBotAcupointsClick;
  DBotAcupoints0_3.OnClick := DBotAcupointsClick;
  DBotAcupoints0_4.OnClick := DBotAcupointsClick;
  DBotAcupoints0_0.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints0_1.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints0_2.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints0_3.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints0_4.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints0_0.Tag := 0;
  DBotAcupoints0_1.Tag := 1;
  DBotAcupoints0_2.Tag := 2;
  DBotAcupoints0_3.Tag := 3;
  DBotAcupoints0_4.Tag := 4;

  DLabelAcupoints0_0.OnClick := DBotAcupointsClick;
  DLabelAcupoints0_1.OnClick := DBotAcupointsClick;
  DLabelAcupoints0_2.OnClick := DBotAcupointsClick;
  DLabelAcupoints0_3.OnClick := DBotAcupointsClick;
  DLabelAcupoints0_4.OnClick := DBotAcupointsClick;
  DLabelAcupoints0_0.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints0_1.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints0_2.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints0_3.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints0_4.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints0_0.Tag := 0;
  DLabelAcupoints0_1.Tag := 1;
  DLabelAcupoints0_2.Tag := 2;
  DLabelAcupoints0_3.Tag := 3;
  DLabelAcupoints0_4.Tag := 4;
  {---------------------------------冲脉-----------------------------------------}
  DBotAcupoints1_0.OnClick := DBotAcupointsClick;
  DBotAcupoints1_1.OnClick := DBotAcupointsClick;
  DBotAcupoints1_2.OnClick := DBotAcupointsClick;
  DBotAcupoints1_3.OnClick := DBotAcupointsClick;
  DBotAcupoints1_4.OnClick := DBotAcupointsClick;
  DBotAcupoints1_0.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints1_1.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints1_2.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints1_3.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints1_4.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints1_0.Tag := 0;
  DBotAcupoints1_1.Tag := 1;
  DBotAcupoints1_2.Tag := 2;
  DBotAcupoints1_3.Tag := 3;
  DBotAcupoints1_4.Tag := 4;

  DLabelAcupoints1_0.OnClick := DBotAcupointsClick;
  DLabelAcupoints1_1.OnClick := DBotAcupointsClick;
  DLabelAcupoints1_2.OnClick := DBotAcupointsClick;
  DLabelAcupoints1_3.OnClick := DBotAcupointsClick;
  DLabelAcupoints1_4.OnClick := DBotAcupointsClick;
  DLabelAcupoints1_0.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints1_1.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints1_2.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints1_3.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints1_4.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints1_0.Tag := 0;
  DLabelAcupoints1_1.Tag := 1;
  DLabelAcupoints1_2.Tag := 2;
  DLabelAcupoints1_3.Tag := 3;
  DLabelAcupoints1_4.Tag := 4;
  {---------------------------------阴跷-----------------------------------------}
  DBotAcupoints2_0.OnClick := DBotAcupointsClick;
  DBotAcupoints2_1.OnClick := DBotAcupointsClick;
  DBotAcupoints2_2.OnClick := DBotAcupointsClick;
  DBotAcupoints2_3.OnClick := DBotAcupointsClick;
  DBotAcupoints2_4.OnClick := DBotAcupointsClick;
  DBotAcupoints2_0.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints2_1.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints2_2.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints2_3.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints2_4.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints2_0.Tag := 0;
  DBotAcupoints2_1.Tag := 1;
  DBotAcupoints2_2.Tag := 2;
  DBotAcupoints2_3.Tag := 3;
  DBotAcupoints2_4.Tag := 4;

  DLabelAcupoints2_0.OnClick := DBotAcupointsClick;
  DLabelAcupoints2_1.OnClick := DBotAcupointsClick;
  DLabelAcupoints2_2.OnClick := DBotAcupointsClick;
  DLabelAcupoints2_3.OnClick := DBotAcupointsClick;
  DLabelAcupoints2_4.OnClick := DBotAcupointsClick;
  DLabelAcupoints2_0.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints2_1.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints2_2.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints2_3.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints2_4.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints2_0.Tag := 0;
  DLabelAcupoints2_1.Tag := 1;
  DLabelAcupoints2_2.Tag := 2;
  DLabelAcupoints2_3.Tag := 3;
  DLabelAcupoints2_4.Tag := 4;
  {---------------------------------阴维-----------------------------------------}
  DBotAcupoints3_0.OnClick := DBotAcupointsClick;
  DBotAcupoints3_1.OnClick := DBotAcupointsClick;
  DBotAcupoints3_2.OnClick := DBotAcupointsClick;
  DBotAcupoints3_3.OnClick := DBotAcupointsClick;
  DBotAcupoints3_4.OnClick := DBotAcupointsClick;
  DBotAcupoints3_0.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints3_1.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints3_2.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints3_3.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints3_4.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints3_0.Tag := 0;
  DBotAcupoints3_1.Tag := 1;
  DBotAcupoints3_2.Tag := 2;
  DBotAcupoints3_3.Tag := 3;
  DBotAcupoints3_4.Tag := 4;

  DLabelAcupoints3_0.OnClick := DBotAcupointsClick;
  DLabelAcupoints3_1.OnClick := DBotAcupointsClick;
  DLabelAcupoints3_2.OnClick := DBotAcupointsClick;
  DLabelAcupoints3_3.OnClick := DBotAcupointsClick;
  DLabelAcupoints3_4.OnClick := DBotAcupointsClick;
  DLabelAcupoints3_0.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints3_1.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints3_2.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints3_3.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints3_4.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints3_0.Tag := 0;
  DLabelAcupoints3_1.Tag := 1;
  DLabelAcupoints3_2.Tag := 2;
  DLabelAcupoints3_3.Tag := 3;
  DLabelAcupoints3_4.Tag := 4;
  {---------------------------------任脉-----------------------------------------}
  DBotAcupoints4_0.OnClick := DBotAcupointsClick;
  DBotAcupoints4_1.OnClick := DBotAcupointsClick;
  DBotAcupoints4_2.OnClick := DBotAcupointsClick;
  DBotAcupoints4_3.OnClick := DBotAcupointsClick;
  DBotAcupoints4_4.OnClick := DBotAcupointsClick;
  DBotAcupoints4_0.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints4_1.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints4_2.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints4_3.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints4_4.OnMouseMove := DBotAcupointsMouseMove;
  DBotAcupoints4_0.Tag := 0;
  DBotAcupoints4_1.Tag := 1;
  DBotAcupoints4_2.Tag := 2;
  DBotAcupoints4_3.Tag := 3;
  DBotAcupoints4_4.Tag := 4;

  DLabelAcupoints4_0.OnClick := DBotAcupointsClick;
  DLabelAcupoints4_1.OnClick := DBotAcupointsClick;
  DLabelAcupoints4_2.OnClick := DBotAcupointsClick;
  DLabelAcupoints4_3.OnClick := DBotAcupointsClick;
  DLabelAcupoints4_4.OnClick := DBotAcupointsClick;
  DLabelAcupoints4_0.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints4_1.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints4_2.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints4_3.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints4_4.OnMouseMove := DBotAcupointsMouseMove;
  DLabelAcupoints4_0.Tag := 0;
  DLabelAcupoints4_1.Tag := 1;
  DLabelAcupoints4_2.Tag := 2;
  DLabelAcupoints4_3.Tag := 3;
  DLabelAcupoints4_4.Tag := 4;
  {-------------------------------------------------------------------------------}

  DStateTabSheet1.Caption := g_ConfigClient.sHumPropertyGroupCaption[0];
  DStateTabSheet2.Caption := g_ConfigClient.sHumPropertyGroupCaption[1];
  DStateTabSheet3.Caption := g_ConfigClient.sHumPropertyGroupCaption[2];
  DStateTabSheet4.Caption := g_ConfigClient.sHumPropertyGroupCaption[3];
  DStateTabSheet5.Caption := g_ConfigClient.sHumPropertyGroupCaption[4];
  DStateTabSheet6.Caption := g_ConfigClient.sHumPropertyGroupCaption[5];
  DStateTabSheet7.Caption := g_ConfigClient.sHumPropertyGroupCaption[6];

  DHeroStateTabSheet1.Caption := g_ConfigClient.sHumPropertyGroupCaption[0];
  DHeroStateTabSheet2.Caption := g_ConfigClient.sHumPropertyGroupCaption[1];
  DHeroStateTabSheet3.Caption := g_ConfigClient.sHumPropertyGroupCaption[2];
  DHeroStateTabSheet4.Caption := g_ConfigClient.sHumPropertyGroupCaption[3];
  DHeroStateTabSheet5.Caption := g_ConfigClient.sHumPropertyGroupCaption[4];
  DHeroStateTabSheet6.Caption := g_ConfigClient.sHumPropertyGroupCaption[5];

  DUserState1StateTabSheet1.Caption := g_ConfigClient.sHumPropertyGroupCaption[0];
  DUserState1StateTabSheet2.Caption := g_ConfigClient.sHumPropertyGroupCaption[1];
  DUserState1StateTabSheet5.Caption := g_ConfigClient.sHumPropertyGroupCaption[4];

  DStateWin.OnBringToFront := OnDStateWinBringToFront;
  DStateWinEx.OnBringToFront := OnDStateWinBringToFront;
  DStateWin.OnUpDate := DStateWinUpDate;
  DStateBasic.OnStartSubPaint := DStateWinDirectPaint;
  DStateFashion.OnStartSubPaint := DStateWinFashionDirectPaint;

  DSWArmRingL.OnClick := FrmDlg.DSWWeaponClick;
  DSWArmRingR.OnClick := FrmDlg.DSWWeaponClick;
  DSWBelt.OnClick := FrmDlg.DSWWeaponClick;
  DSWBoots.OnClick := FrmDlg.DSWWeaponClick;
  DSWBujuk.OnClick := FrmDlg.DSWWeaponClick;
  DSWCharm.OnClick := FrmDlg.DSWWeaponClick;
  DSWDress.OnClick := FrmDlg.DSWWeaponClick;
  DSWHelmet.OnClick := FrmDlg.DSWWeaponClick;
  DSWLight.OnClick := FrmDlg.DSWWeaponClick;
  DSWNecklace.OnClick := FrmDlg.DSWWeaponClick;
  DSWRingL.OnClick := FrmDlg.DSWWeaponClick;
  DSWRingR.OnClick := FrmDlg.DSWWeaponClick;
  DSWWeapon.OnClick := FrmDlg.DSWWeaponClick;
  DSWDrum.OnClick := FrmDlg.DSWWeaponClick;
  DSWHorse.OnClick := FrmDlg.DSWWeaponClick;
  DSWShield.OnClick := FrmDlg.DSWWeaponClick; // 盾牌 chongchong 2013-09-16
  DSWJade.OnClick := FrmDlg.DSWWeaponClick;

  DSWFashionDress.OnClick := FrmDlg.DSWWeaponClick; // 时装 衣服 chongchong 2013-09-16
  DSWFashionWeapon.OnClick := FrmDlg.DSWWeaponClick; // 时装 武器 chongchong 2013-09-16
  DSWFashionHelmet.OnClick := FrmDlg.DSWWeaponClick;
  DSWFashionNecklace.OnClick := FrmDlg.DSWWeaponClick;
  DSWFashionArmRingR.OnClick := FrmDlg.DSWWeaponClick;
  DSWFashionArmRingL.OnClick := FrmDlg.DSWWeaponClick;
  DSWFashionRingR.OnClick := FrmDlg.DSWWeaponClick;
  DSWFashionRingL.OnClick := FrmDlg.DSWWeaponClick;
  DSWFashionLight.OnClick := FrmDlg.DSWWeaponClick;
  DSWFashionBelt.OnClick := FrmDlg.DSWWeaponClick;
  DSWFashionBoots.OnClick := FrmDlg.DSWWeaponClick;
  DSWFashionDrum.OnClick := FrmDlg.DSWWeaponClick;

  DSWShowFashion.OnClick := DSWShowFashionClick;
  DSWShowFashion.Checked := False;

  // 置前，不然被盾牌挡住 chongchong 2013-10-18
  DSWLight.BringToFront;
  DSWArmRingL.BringToFront;

  DSWArmRingL.OnMouseMove := DSWWeaponMouseMove;
  DSWArmRingR.OnMouseMove := DSWWeaponMouseMove;
  DSWBelt.OnMouseMove := DSWWeaponMouseMove;
  DSWBoots.OnMouseMove := DSWWeaponMouseMove;
  DSWBujuk.OnMouseMove := DSWWeaponMouseMove;
  DSWCharm.OnMouseMove := DSWWeaponMouseMove;
  DSWDress.OnMouseMove := DSWWeaponMouseMove;
  DSWHelmet.OnMouseMove := DSWWeaponMouseMove;
  DSWLight.OnMouseMove := DSWWeaponMouseMove;
  DSWNecklace.OnMouseMove := DSWWeaponMouseMove;
  DSWRingL.OnMouseMove := DSWWeaponMouseMove;
  DSWRingR.OnMouseMove := DSWWeaponMouseMove;
  DSWWeapon.OnMouseMove := DSWWeaponMouseMove;
  DSWDrum.OnMouseMove := DSWWeaponMouseMove;
  DSWHorse.OnMouseMove := DSWWeaponMouseMove;
  DSWShield.OnMouseMove := DSWWeaponMouseMove; // 盾牌 chongchong 2013-09-16
  DSWJade.OnMouseMove := DSWWeaponMouseMove;

  DSWFashionDress.OnMouseMove := DSWWeaponMouseMove; // 时装 衣服 chongchong 2013-09-16
  DSWFashionWeapon.OnMouseMove := DSWWeaponMouseMove; // 时装 武器 chongchong 2013-09-16
  DSWFashionHelmet.OnMouseMove := DSWWeaponMouseMove;
  DSWFashionNecklace.OnMouseMove := DSWWeaponMouseMove;
  DSWFashionArmRingR.OnMouseMove := DSWWeaponMouseMove;
  DSWFashionArmRingL.OnMouseMove := DSWWeaponMouseMove;
  DSWFashionRingR.OnMouseMove := DSWWeaponMouseMove;
  DSWFashionRingL.OnMouseMove := DSWWeaponMouseMove;
  DSWFashionLight.OnMouseMove := DSWWeaponMouseMove;
  DSWFashionBelt.OnMouseMove := DSWWeaponMouseMove;
  DSWFashionBoots.OnMouseMove := DSWWeaponMouseMove;
  DSWFashionDrum.OnMouseMove := DSWWeaponMouseMove;

  DSWArmRingL.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWArmRingR.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWBelt.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWBoots.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWBujuk.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWCharm.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWDress.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWHelmet.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWLight.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWNecklace.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWRingL.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWRingR.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWWeapon.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWDrum.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWHorse.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWShield.OnInRealArea := FrmDlg.DBottomInRealArea; // 盾牌 chongchong 2013-09-16
  DSWJade.OnInRealArea := FrmDlg.DBottomInRealArea;

  DSWFashionDress.OnInRealArea := FrmDlg.DBottomInRealArea; // 时装 衣服 chongchong 2013-09-16
  DSWFashionWeapon.OnInRealArea := FrmDlg.DBottomInRealArea; // 时装 武器 chongchong 2013-09-16
  DSWFashionHelmet.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWFashionNecklace.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWFashionArmRingR.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWFashionArmRingL.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWFashionRingR.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWFashionRingL.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWFashionLight.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWFashionBelt.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWFashionBoots.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWFashionDrum.OnInRealArea := FrmDlg.DBottomInRealArea;

  DSWArmRingL.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWArmRingR.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWBelt.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWBoots.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWBujuk.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWCharm.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWLight.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWNecklace.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWRingL.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWRingR.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWDrum.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWHorse.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWJade.OnPaint := FrmDlg.DSWLightDirectPaint;

  //DSWFashionHelmet.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWFashionNecklace.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWFashionArmRingR.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWFashionArmRingL.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWFashionRingR.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWFashionRingL.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWFashionLight.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWFashionBelt.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWFashionBoots.OnPaint := FrmDlg.DSWLightDirectPaint;
  DSWFashionDrum.OnPaint := FrmDlg.DSWLightDirectPaint;

  DSWDress.Tag := U_DRESS;
  DSWWeapon.Tag := U_WEAPON;
  DSWHelmet.Tag := U_HELMET;
  DSWNecklace.Tag := U_NECKLACE;
  DSWLight.Tag := U_RIGHTHAND;
  DSWArmRingR.Tag := U_ARMRINGR;
  DSWArmRingL.Tag := U_ARMRINGL;
  DSWRingR.Tag := U_RINGR;
  DSWRingL.Tag := U_RINGL;
  DSWBujuk.Tag := U_BUJUK;
  DSWBelt.Tag := U_BELT;
  DSWBoots.Tag := U_BOOTS;
  DSWCharm.Tag := U_CHARM;
  DSWDrum.Tag := U_DRUM;
  DSWHorse.Tag := U_HORSE;
  DSWShield.Tag := U_SHIELD;
  DSWJade.Tag := U_JADE;

  DSWFashionDress.Tag := U_FASHIONDRESS; // 时装 衣服 chongchong 2013-09-16
  DSWFashionWeapon.Tag := U_FASHIONWEAPON; // 时装 武器 chongchong 2013-09-16

  DSWFashionHelmet.Tag := U_FASHIONHELMET;
  DSWFashionNecklace.Tag := U_FASHIONNECKLACE;
  DSWFashionArmRingR.Tag := U_FASHIONARMRINGR;
  DSWFashionArmRingL.Tag := U_FASHIONARMRINGL;
  DSWFashionRingR.Tag := U_FASHIONRINGR;
  DSWFashionRingL.Tag := U_FASHIONRINGL;
  DSWFashionLight.Tag := U_FASHIONRIGHTHAND;
  DSWFashionBelt.Tag := U_FASHIONBELT;
  DSWFashionBoots.Tag := U_FASHIONBOOTS;
  DSWFashionDrum.Tag := U_FASHIONCHARM;

  {------------------------------------------------------------------------------}
  DUserState1StateBasic.OnStartSubPaint := DUserState1DirectPaint;
  DUserState1StateFashion.OnStartSubPaint := DUserState1FashionDirectPaint;
  DNecklaceUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHelmetUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DLightUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DArmringRUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DArmringLUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DRingRUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DRingLUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DWeaponUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DDressUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DBujukUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DBeltUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DBootsUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DCharmUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DDrumUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHorseUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DShieldUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DJadeUS1.OnInRealArea := FrmDlg.DBottomInRealArea;

  DJewelryBoxUS1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DFashionDressUS1.OnInRealArea := FrmDlg.DBottomInRealArea; // 时装 衣服 chongchong 2013-09-16
  DFashionWeaponUS1.OnInRealArea := FrmDlg.DBottomInRealArea;

  DNecklaceUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DLightUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DArmringRUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DArmringLUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DRingRUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DRingLUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DBujukUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DBeltUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DBootsUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DCharmUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DDrumUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DHorseUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DJadeUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  //DShieldUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;

  //DFashionHelmetUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DFashionNecklaceUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DFashionArmRingRUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DFashionArmRingLUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DFashionRingRUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DFashionRingLUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DFashionLightUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DFashionBeltUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DFashionBootsUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;
  DFashionDrumUS1.OnPaint := FrmDlg.DNecklaceUS1DirectPaint;

  DNecklaceUS1.OnMouseMove := DWeaponUS1MouseMove;
  DLightUS1.OnMouseMove := DWeaponUS1MouseMove;
  DArmringRUS1.OnMouseMove := DWeaponUS1MouseMove;
  DArmringLUS1.OnMouseMove := DWeaponUS1MouseMove;
  DRingRUS1.OnMouseMove := DWeaponUS1MouseMove;
  DRingLUS1.OnMouseMove := DWeaponUS1MouseMove;
  DWeaponUS1.OnMouseMove := DWeaponUS1MouseMove;
  DDressUS1.OnMouseMove := DWeaponUS1MouseMove;
  DBujukUS1.OnMouseMove := DWeaponUS1MouseMove;
  DBeltUS1.OnMouseMove := DWeaponUS1MouseMove;
  DBootsUS1.OnMouseMove := DWeaponUS1MouseMove;
  DCharmUS1.OnMouseMove := DWeaponUS1MouseMove;
  DHelmetUS1.OnMouseMove := DWeaponUS1MouseMove;
  DDrumUS1.OnMouseMove := DWeaponUS1MouseMove;
  DHorseUS1.OnMouseMove := DWeaponUS1MouseMove;
  DShieldUS1.OnMouseMove := DWeaponUS1MouseMove;
  DJadeUS1.OnMouseMove := DWeaponUS1MouseMove;

  DFashionHelmetUS1.OnMouseMove := DWeaponUS1MouseMove;
  DFashionNecklaceUS1.OnMouseMove := DWeaponUS1MouseMove;
  DFashionArmRingRUS1.OnMouseMove := DWeaponUS1MouseMove;
  DFashionArmRingLUS1.OnMouseMove := DWeaponUS1MouseMove;
  DFashionRingRUS1.OnMouseMove := DWeaponUS1MouseMove;
  DFashionRingLUS1.OnMouseMove := DWeaponUS1MouseMove;
  DFashionLightUS1.OnMouseMove := DWeaponUS1MouseMove;
  DFashionBeltUS1.OnMouseMove := DWeaponUS1MouseMove;
  DFashionBootsUS1.OnMouseMove := DWeaponUS1MouseMove;
  DFashionDrumUS1.OnMouseMove := DWeaponUS1MouseMove;

  DJewelryBoxUS1.OnMouseMove := DSWJewelryBoxMouseMove;
  DJewelryBoxUS1.OnClick := DSWJewelryBoxClick;

  DFashionDressUS1.OnMouseMove := DWeaponUS1MouseMove; // 时装 衣服 chongchong 2013-09-16
  DFashionWeaponUS1.OnMouseMove := DWeaponUS1MouseMove;

  DDressUS1.Tag := U_DRESS;
  DWeaponUS1.Tag := U_WEAPON;
  DHelmetUS1.Tag := U_HELMET;
  DNecklaceUS1.Tag := U_NECKLACE;
  DLightUS1.Tag := U_RIGHTHAND;
  DArmRingRUS1.Tag := U_ARMRINGR;
  DArmRingLUS1.Tag := U_ARMRINGL;
  DRingRUS1.Tag := U_RINGR;
  DRingLUS1.Tag := U_RINGL;
  DBujukUS1.Tag := U_BUJUK;
  DBeltUS1.Tag := U_BELT;
  DBootsUS1.Tag := U_BOOTS;
  DCharmUS1.Tag := U_CHARM;
  DDrumUS1.Tag := U_DRUM;
  DHorseUS1.Tag := U_HORSE;
  DShieldUS1.Tag := U_SHIELD;
  DJadeUS1.Tag := U_JADE;
  DFashionDressUS1.Tag := U_FASHIONDRESS; // 时装 衣服 chongchong 2013-09-16
  DFashionWeaponUS1.Tag := U_FASHIONWEAPON;

  DFashionHelmetUS1.Tag := U_FASHIONHELMET;
  DFashionNecklaceUS1.Tag := U_FASHIONNECKLACE;
  DFashionArmRingRUS1.Tag := U_FASHIONARMRINGR;
  DFashionArmRingLUS1.Tag := U_FASHIONARMRINGL;
  DFashionRingRUS1.Tag := U_FASHIONRINGR;
  DFashionRingLUS1.Tag := U_FASHIONRINGL;
  DFashionLightUS1.Tag := U_FASHIONRIGHTHAND;
  DFashionBeltUS1.Tag := U_FASHIONBELT;
  DFashionBootsUS1.Tag := U_FASHIONBOOTS;
  DFashionDrumUS1.Tag := U_FASHIONCHARM;

  if FrmDlg is TSerialWindows then begin
    TSerialWindows(FrmDlg).DRecallDeputyHero.OnClick := DRecallDeputyHeroClick;
  end;
  DStMagBack1.Tag := 0;
  DStMagBack1.OnStartPaint := DStateMagicDirectPaint;
  DStMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStMagBack2.Tag := 1;
  DStMagBack2.OnStartPaint := DStateMagicDirectPaint;
  DStMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStMagBack3.Tag := 2;
  DStMagBack3.OnStartPaint := DStateMagicDirectPaint;
  DStMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStMagBack4.Tag := 3;
  DStMagBack4.OnStartPaint := DStateMagicDirectPaint;
  DStMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStMagBack5.Tag := 4;
  DStMagBack5.OnStartPaint := DStateMagicDirectPaint;
  DStMagBack5.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStMagBack6.Tag := 5;
  DStMagBack6.OnStartPaint := DStateMagicDirectPaint;
  DStMagBack6.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStMag1.ClickCount := csStone;
  DStMag1.OnClick := DStMag1Click;
  DStMag1.OnMouseDown := DStMag1MouseDown;
  DStMag1.OnMouseUp := DStMag1MouseUp;
  DStMag1.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStMag1.OnMouseMove := DStMagMouseMove;
  DStMag1.OnPaint := DStMag1DirectPaint;
  DStMag1.Tag := 0;

  DStMagLblName1.Visible := False;
  DStMagLvIcon1.Visible := False;
  DStMagLvText1.Visible := False;
  DStMagExpIcon1.Visible := False;
  DStMagExpText1.Visible := False;

  DStMag2.ClickCount := csStone;
  DStMag2.OnClick := DStMag1Click;
  DStMag2.OnMouseDown := DStMag1MouseDown;
  DStMag2.OnMouseUp := DStMag1MouseUp;
  DStMag2.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStMag2.OnMouseMove := DStMagMouseMove;
  DStMag2.OnPaint := DStMag1DirectPaint;
  DStMag2.Tag := 1;

  DStMag3.ClickCount := csStone;
  DStMag3.OnClick := DStMag1Click;
  DStMag3.OnMouseDown := DStMag1MouseDown;
  DStMag3.OnMouseUp := DStMag1MouseUp;
  DStMag3.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStMag3.OnMouseMove := DStMagMouseMove;
  DStMag3.OnPaint := DStMag1DirectPaint;
  DStMag3.Tag := 2;

  DStMag4.ClickCount := csStone;
  DStMag4.OnClick := DStMag1Click;
  DStMag4.OnMouseDown := DStMag1MouseDown;
  DStMag4.OnMouseUp := DStMag1MouseUp;
  DStMag4.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStMag4.OnMouseMove := DStMagMouseMove;
  DStMag4.OnPaint := DStMag1DirectPaint;
  DStMag4.Tag := 3;

  DStMag5.ClickCount := csStone;
  DStMag5.OnClick := DStMag1Click;
  DStMag5.OnMouseDown := DStMag1MouseDown;
  DStMag5.OnMouseUp := DStMag1MouseUp;
  DStMag5.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStMag5.OnMouseMove := DStMagMouseMove;
  DStMag5.OnPaint := DStMag1DirectPaint;
  DStMag5.Tag := 4;

  DStMag6.ClickCount := csStone;
  DStMag6.OnClick := DStMag1Click;
  DStMag6.OnMouseDown := DStMag1MouseDown;
  DStMag6.OnMouseUp := DStMag1MouseUp;
  DStMag6.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStMag6.OnMouseMove := DStMagMouseMove;
  DStMag6.OnPaint := DStMag1DirectPaint;
  DStMag6.Tag := 5;

  DStPageUp.ClickCount := csNorm;
  DStPageDown.ClickCount := csNorm;
  DStPageUp.OnClick := DStPageUpClick;
  DStPageUp.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStPageDown.OnClick := DStPageUpClick;
  DStPageDown.OnClickSound := FrmDlg.DLoginNewClickSound;
  {------------------------------------------------------------------------------}
  DStNGMagBack1.Tag := 0;
  DStNGMagBack1.OnStartPaint := DStateNGMagicDirectPaint;
  DStNGMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStNGMagBack2.Tag := 1;
  DStNGMagBack2.OnStartPaint := DStateNGMagicDirectPaint;
  DStNGMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStNGMagBack3.Tag := 2;
  DStNGMagBack3.OnStartPaint := DStateNGMagicDirectPaint;
  DStNGMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStNGMagBack4.Tag := 3;
  DStNGMagBack4.OnStartPaint := DStateNGMagicDirectPaint;
  DStNGMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStNGMagBack5.Tag := 4;
  DStNGMagBack5.OnStartPaint := DStateNGMagicDirectPaint;
  DStNGMagBack5.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStNGMagBack6.Tag := 5;
  DStNGMagBack6.OnStartPaint := DStateNGMagicDirectPaint;
  DStNGMagBack6.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStNGMag1.ClickCount := csStone;
  DStNGMag1.OnClick := DStNGMag1Click;
  DStNGMag1.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStNGMag1.OnMouseMove := DStNGMagMouseMove;
  DStNGMag1.OnPaint := DStNGMag1DirectPaint;
  DStNGMag1.Tag := 0;

  DstNGMagLblName1.Visible := False;
  DstNGMagLvIcon1.Visible := False;
  DstNGMagLvText1.Visible := False;
  DstNGMagExpIcon1.Visible := False;
  DstNGMagExpText1.Visible := False;

  DStNGMag2.ClickCount := csStone;
  DStNGMag2.OnClick := DStNGMag1Click;
  DStNGMag2.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStNGMag2.OnMouseMove := DStNGMagMouseMove;
  DStNGMag2.OnPaint := DStNGMag1DirectPaint;
  DStNGMag2.Tag := 1;

  DStNGMag3.ClickCount := csStone;
  DStNGMag3.OnClick := DStNGMag1Click;
  DStNGMag3.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStNGMag3.OnMouseMove := DStNGMagMouseMove;
  DStNGMag3.OnPaint := DStNGMag1DirectPaint;
  DStNGMag3.Tag := 2;

  DStNGMag4.ClickCount := csStone;
  DStNGMag4.OnClick := DStNGMag1Click;
  DStNGMag4.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStNGMag4.OnMouseMove := DStNGMagMouseMove;
  DStNGMag4.OnPaint := DStNGMag1DirectPaint;
  DStNGMag4.Tag := 3;

  DStNGMag5.ClickCount := csStone;
  DStNGMag5.OnClick := DStNGMag1Click;
  DStNGMag5.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStNGMag5.OnMouseMove := DStNGMagMouseMove;
  DStNGMag5.OnPaint := DStNGMag1DirectPaint;
  DStNGMag5.Tag := 4;

  DStNGMag6.ClickCount := csStone;
  DStNGMag6.OnClick := DStNGMag1Click;
  DStNGMag6.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStNGMag6.OnMouseMove := DStNGMagMouseMove;
  DStNGMag6.OnPaint := DStNGMag1DirectPaint;
  DStNGMag6.Tag := 5;

  DStNGPageUp.ClickCount := csNorm;
  DStNGPageDown.ClickCount := csNorm;
  DStNGPageUp.OnClick := DStNGPageUpClick;
  DStNGPageUp.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStNGPageDown.OnClick := DStNGPageUpClick;
  DStNGPageDown.OnClickSound := FrmDlg.DLoginNewClickSound;

  {------------------------------------------------------------------------------}
  DStLJMagBack1.Tag := 0;
  DStLJMagBack1.OnStartPaint := DStateLJMagicDirectPaint;
  DStLJMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStLJMagBack2.Tag := 1;
  DStLJMagBack2.OnStartPaint := DStateLJMagicDirectPaint;
  DStLJMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStLJMagBack3.Tag := 2;
  DStLJMagBack3.OnStartPaint := DStateLJMagicDirectPaint;
  DStLJMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStLJMagBack4.Tag := 3;
  DStLJMagBack4.OnStartPaint := DStateLJMagicDirectPaint;
  DStLJMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DStLJMag1.ClickCount := csStone;
  DStLJMag1.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStLJMag1.OnMouseMove := DStLJMagMouseMove;
  DStLJMag1.OnPaint := DStLJMag1DirectPaint;
  DStLJMag1.Tag := 0;

  DstLJMagLblName1.Visible := False;
  DstLJMagLvIcon1.Visible := False;
  DstLJMagLvText1.Visible := False;
  DstLJMagExpIcon1.Visible := False;
  DstLJMagExpText1.Visible := False;

  DStLJMag2.ClickCount := csStone;
  DStLJMag2.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStLJMag2.OnMouseMove := DStLJMagMouseMove;
  DStLJMag2.OnPaint := DStLJMag1DirectPaint;
  DStLJMag2.Tag := 1;

  DStLJMag3.ClickCount := csStone;
  DStLJMag3.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStLJMag3.OnMouseMove := DStLJMagMouseMove;
  DStLJMag3.OnPaint := DStLJMag1DirectPaint;
  DStLJMag3.Tag := 2;

  DStLJMag4.ClickCount := csStone;
  DStLJMag4.OnClickSound := FrmDlg.DLoginNewClickSound;
  DStLJMag4.OnMouseMove := DStLJMagMouseMove;
  DStLJMag4.OnPaint := DStLJMag1DirectPaint;
  DStLJMag4.Tag := 3;

  DSMB1.OnStopPaint := DSMB1DirectPaint;
  DSMB2.OnStopPaint := DSMB1DirectPaint;
  DSMB3.OnStopPaint := DSMB1DirectPaint;
  DSMB4.OnStopPaint := DSMB1DirectPaint;
  DSMB1.OnClick := DSMB1Click;
  DSMB2.OnClick := DSMB1Click;
  DSMB3.OnClick := DSMB1Click;
  DSMB4.OnClick := DSMB1Click;
  DSMB1.Tag := 0;
  DSMB2.Tag := 1;
  DSMB3.Tag := 2;
  DSMB4.Tag := 3;

  { 首饰盒 chongchong 2013-10-20 }
  DSWJewelryBox.Visible := False;
  DHeroSWJewelryBox.Visible := False;
  DJewelryBoxUS1.Visible := False;

  DSWGodBless.Visible := False;
  DHeroSWGodBless.Visible := False;
  DGodBlessUS1.Visible := False;

  DSWJewelryBoxDlg.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroSWJewelryBoxDlg.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DSWJewelryBox.OnClick := DSWJewelryBoxClick;
  DHeroSWJewelryBox.OnClick := DSWJewelryBoxClick;

  DSWJewelryClose.OnClick := DSWJewelryBoxClick;
  DHeroSWJewelryClose.OnClick := DSWJewelryBoxClick;
  DJewelryCloseUS1.OnClick := DSWJewelryBoxClick;

  DSWJewelryBox.OnMouseMove := DSWJewelryBoxMouseMove;
  DHeroSWJewelryBox.OnMouseMove := DSWJewelryBoxMouseMove;
  DJewelryCloseUS1.OnMouseMove := DSWJewelryBoxMouseMove;

  DSWGodBlessDlg.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroSWGodBlessDlg.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DGodBlessDlgUS1.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DSWGodBless.OnClick := DSWGodBlessClick;
  DSWGodBless.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless.OnMouseMove := DControlMouseMoveShowHint;
  DHeroSWGodBless.OnClick := DSWGodBlessClick;
  DHeroSWGodBless.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBlessUS1.OnClick := DSWGodBlessClick;
  DGodBlessUS1.OnInRealArea := FrmDlg.DBottomInRealArea;

  DSWGodBlessClose.OnClick := DSWGodBlessClick;
  DHeroSWGodBlessClose.OnClick := DSWGodBlessClick;
  DGodBlessCloseUS1.OnClick := DSWGodBlessClick;

  //----------------------------------------------------------------------
  DSWJewelry1.OnClick := DSWJewelryBoxItemClick;
  DSWJewelry2.OnClick := DSWJewelryBoxItemClick;
  DSWJewelry3.OnClick := DSWJewelryBoxItemClick;
  DSWJewelry4.OnClick := DSWJewelryBoxItemClick;
  DSWJewelry5.OnClick := DSWJewelryBoxItemClick;
  DSWJewelry6.OnClick := DSWJewelryBoxItemClick;

  DSWJewelry1.OnMouseMove := DSWJewelryBoxItemMouseMove;
  DSWJewelry2.OnMouseMove := DSWJewelryBoxItemMouseMove;
  DSWJewelry3.OnMouseMove := DSWJewelryBoxItemMouseMove;
  DSWJewelry4.OnMouseMove := DSWJewelryBoxItemMouseMove;
  DSWJewelry5.OnMouseMove := DSWJewelryBoxItemMouseMove;
  DSWJewelry6.OnMouseMove := DSWJewelryBoxItemMouseMove;

  DSWJewelry1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWJewelry2.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWJewelry3.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWJewelry4.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWJewelry5.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWJewelry6.OnInRealArea := FrmDlg.DBottomInRealArea;

  DSWJewelry1.OnPaint := DSWJewelryBoxItemPaint;
  DSWJewelry2.OnPaint := DSWJewelryBoxItemPaint;
  DSWJewelry3.OnPaint := DSWJewelryBoxItemPaint;
  DSWJewelry4.OnPaint := DSWJewelryBoxItemPaint;
  DSWJewelry5.OnPaint := DSWJewelryBoxItemPaint;
  DSWJewelry6.OnPaint := DSWJewelryBoxItemPaint;

  DSWJewelry1.Tag := 0;
  DSWJewelry2.Tag := 1;
  DSWJewelry3.Tag := 2;
  DSWJewelry4.Tag := 3;
  DSWJewelry5.Tag := 4;
  DSWJewelry6.Tag := 5;

  //----------------------------------------------------------------------
  DHeroSWJewelry1.OnClick := DHeroSWJewelryBoxItemClick;
  DHeroSWJewelry2.OnClick := DHeroSWJewelryBoxItemClick;
  DHeroSWJewelry3.OnClick := DHeroSWJewelryBoxItemClick;
  DHeroSWJewelry4.OnClick := DHeroSWJewelryBoxItemClick;
  DHeroSWJewelry5.OnClick := DHeroSWJewelryBoxItemClick;
  DHeroSWJewelry6.OnClick := DHeroSWJewelryBoxItemClick;

  DHeroSWJewelry1.OnMouseMove := DHeroSWJewelryBoxItemMouseMove;
  DHeroSWJewelry2.OnMouseMove := DHeroSWJewelryBoxItemMouseMove;
  DHeroSWJewelry3.OnMouseMove := DHeroSWJewelryBoxItemMouseMove;
  DHeroSWJewelry4.OnMouseMove := DHeroSWJewelryBoxItemMouseMove;
  DHeroSWJewelry5.OnMouseMove := DHeroSWJewelryBoxItemMouseMove;
  DHeroSWJewelry6.OnMouseMove := DHeroSWJewelryBoxItemMouseMove;

  DHeroSWJewelry1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWJewelry2.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWJewelry3.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWJewelry4.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWJewelry5.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWJewelry6.OnInRealArea := FrmDlg.DBottomInRealArea;

  DHeroSWJewelry1.OnPaint := DHeroSWJewelryBoxItemPaint;
  DHeroSWJewelry2.OnPaint := DHeroSWJewelryBoxItemPaint;
  DHeroSWJewelry3.OnPaint := DHeroSWJewelryBoxItemPaint;
  DHeroSWJewelry4.OnPaint := DHeroSWJewelryBoxItemPaint;
  DHeroSWJewelry5.OnPaint := DHeroSWJewelryBoxItemPaint;
  DHeroSWJewelry6.OnPaint := DHeroSWJewelryBoxItemPaint;

  DHeroSWJewelry1.Tag := 0;
  DHeroSWJewelry2.Tag := 1;
  DHeroSWJewelry3.Tag := 2;
  DHeroSWJewelry4.Tag := 3;
  DHeroSWJewelry5.Tag := 4;
  DHeroSWJewelry6.Tag := 5;

  //----------------------------------------------------------------------
  DJewelry1US1.OnMouseMove := DJewelryBoxItemUS1MouseMove;
  DJewelry2US1.OnMouseMove := DJewelryBoxItemUS1MouseMove;
  DJewelry3US1.OnMouseMove := DJewelryBoxItemUS1MouseMove;
  DJewelry4US1.OnMouseMove := DJewelryBoxItemUS1MouseMove;
  DJewelry5US1.OnMouseMove := DJewelryBoxItemUS1MouseMove;
  DJewelry6US1.OnMouseMove := DJewelryBoxItemUS1MouseMove;

  DJewelry1US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DJewelry2US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DJewelry3US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DJewelry4US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DJewelry5US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DJewelry6US1.OnInRealArea := FrmDlg.DBottomInRealArea;

  DJewelry1US1.OnPaint := DJewelryBoxItemPaintUS1;
  DJewelry2US1.OnPaint := DJewelryBoxItemPaintUS1;
  DJewelry3US1.OnPaint := DJewelryBoxItemPaintUS1;
  DJewelry4US1.OnPaint := DJewelryBoxItemPaintUS1;
  DJewelry5US1.OnPaint := DJewelryBoxItemPaintUS1;
  DJewelry6US1.OnPaint := DJewelryBoxItemPaintUS1;

  DJewelry1US1.Tag := 0;
  DJewelry2US1.Tag := 1;
  DJewelry3US1.Tag := 2;
  DJewelry4US1.Tag := 3;
  DJewelry5US1.Tag := 4;
  DJewelry6US1.Tag := 5;

  //----------------------------------------------------------------------
  DSWGodBless1.OnClick := DSWGodBlessItemClick;
  DSWGodBless2.OnClick := DSWGodBlessItemClick;
  DSWGodBless3.OnClick := DSWGodBlessItemClick;
  DSWGodBless4.OnClick := DSWGodBlessItemClick;
  DSWGodBless5.OnClick := DSWGodBlessItemClick;
  DSWGodBless6.OnClick := DSWGodBlessItemClick;
  DSWGodBless7.OnClick := DSWGodBlessItemClick;
  DSWGodBless8.OnClick := DSWGodBlessItemClick;
  DSWGodBless9.OnClick := DSWGodBlessItemClick;
  DSWGodBless10.OnClick := DSWGodBlessItemClick;
  DSWGodBless11.OnClick := DSWGodBlessItemClick;
  DSWGodBless12.OnClick := DSWGodBlessItemClick;

  DSWGodBless1.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless2.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless3.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless4.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless5.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless6.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless7.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless8.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless9.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless10.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless11.OnMouseMove := DSWGodBlessItemMouseMove;
  DSWGodBless12.OnMouseMove := DSWGodBlessItemMouseMove;

  DSWGodBless1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless2.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless3.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless4.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless5.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless6.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless7.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless8.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless9.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless10.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless11.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWGodBless12.OnInRealArea := FrmDlg.DBottomInRealArea;

  DSWGodBless1.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless2.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless3.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless4.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless5.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless6.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless7.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless8.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless9.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless10.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless11.OnPaint := DSWGodBlessItemPaint;
  DSWGodBless12.OnPaint := DSWGodBlessItemPaint;

  DSWGodBless1.Tag := 0;
  DSWGodBless2.Tag := 1;
  DSWGodBless3.Tag := 2;
  DSWGodBless4.Tag := 3;
  DSWGodBless5.Tag := 4;
  DSWGodBless6.Tag := 5;
  DSWGodBless7.Tag := 6;
  DSWGodBless8.Tag := 7;
  DSWGodBless9.Tag := 8;
  DSWGodBless10.Tag := 9;
  DSWGodBless11.Tag := 10;
  DSWGodBless12.Tag := 11;

  DSWGodBlessInfo.OnClick := DSWGodBlessUpgradeClick;
  //----------------------------------------------------------------------

  DHeroSWGodBless1.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless2.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless3.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless4.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless5.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless6.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless7.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless8.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless9.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless10.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless11.OnClick := DHeroSWGodBlessItemClick;
  DHeroSWGodBless12.OnClick := DHeroSWGodBlessItemClick;

  DHeroSWGodBless1.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless2.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless3.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless4.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless5.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless6.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless7.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless8.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless9.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless10.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless11.OnMouseMove := DHeroSWGodBlessItemMouseMove;
  DHeroSWGodBless12.OnMouseMove := DHeroSWGodBlessItemMouseMove;

  DHeroSWGodBless1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless2.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless3.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless4.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless5.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless6.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless7.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless8.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless9.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless10.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless11.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWGodBless12.OnInRealArea := FrmDlg.DBottomInRealArea;

  DHeroSWGodBless1.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless2.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless3.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless4.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless5.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless6.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless7.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless8.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless9.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless10.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless11.OnPaint := DHeroSWGodBlessItemPaint;
  DHeroSWGodBless12.OnPaint := DHeroSWGodBlessItemPaint;

  DHeroSWGodBless1.Tag := 0;
  DHeroSWGodBless2.Tag := 1;
  DHeroSWGodBless3.Tag := 2;
  DHeroSWGodBless4.Tag := 3;
  DHeroSWGodBless5.Tag := 4;
  DHeroSWGodBless6.Tag := 5;
  DHeroSWGodBless7.Tag := 6;
  DHeroSWGodBless8.Tag := 7;
  DHeroSWGodBless9.Tag := 8;
  DHeroSWGodBless10.Tag := 9;
  DHeroSWGodBless11.Tag := 10;
  DHeroSWGodBless12.Tag := 11;

  DHeroSWGodBlessInfo.OnClick := DSWGodBlessUpgradeClick;
  //----------------------------------------------------------------------

  DGodBless1US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless2US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless3US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless4US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless5US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless6US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless7US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless8US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless9US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless10US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless11US1.OnMouseMove := DGodBlessItemUS1MouseMove;
  DGodBless12US1.OnMouseMove := DGodBlessItemUS1MouseMove;

  DGodBless1US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless2US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless3US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless4US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless5US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless6US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless7US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless8US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless9US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless10US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless11US1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DGodBless12US1.OnInRealArea := FrmDlg.DBottomInRealArea;

  DGodBless1US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless2US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless3US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless4US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless5US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless6US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless7US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless8US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless9US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless10US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless11US1.OnPaint := DGodBlessItemUS1Paint;
  DGodBless12US1.OnPaint := DGodBlessItemUS1Paint;

  DGodBless1US1.Tag := 0;
  DGodBless2US1.Tag := 1;
  DGodBless3US1.Tag := 2;
  DGodBless4US1.Tag := 3;
  DGodBless5US1.Tag := 4;
  DGodBless6US1.Tag := 5;
  DGodBless7US1.Tag := 6;
  DGodBless8US1.Tag := 7;
  DGodBless9US1.Tag := 8;
  DGodBless10US1.Tag := 9;
  DGodBless11US1.Tag := 10;
  DGodBless12US1.Tag := 11;
  //------------------------------------------------------------------------
  DStateTitle.OnMouseMove := DStateTitleWinMouseMove;
  DSTitleItemDesc.OnPaint := DSTitleItemDescOnPaint;

  DSTitleActive.ClickCount := csStone;
  DSTitleActive.OnClick := DSTitleActiveClick;
  DSTitleActive.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSTitleActive.OnMouseMove := DSTitleActiveMouseMove;
  DSTitleActive.OnPaint := DSTitleActiveDirectPaint;

  DSTitleButton1.Tag := 0;
  DSTitleButton1.ClickCount := csStone;
  DSTitleButton1.OnClick := DSTitleButtonClick;
  DSTitleButton1.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSTitleButton1.OnMouseMove := DSTitleButtonMouseMove;
  DSTitleButton1.OnPaint := DSTitleButtonDirectPaint;

  DSTitleButton2.Tag := 1;
  DSTitleButton2.ClickCount := csStone;
  DSTitleButton2.OnClick := DSTitleButtonClick;
  DSTitleButton2.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSTitleButton2.OnMouseMove := DSTitleButtonMouseMove;
  DSTitleButton2.OnPaint := DSTitleButtonDirectPaint;

  DSTitleButton3.Tag := 2;
  DSTitleButton3.ClickCount := csStone;
  DSTitleButton3.OnClick := DSTitleButtonClick;
  DSTitleButton3.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSTitleButton3.OnMouseMove := DSTitleButtonMouseMove;
  DSTitleButton3.OnPaint := DSTitleButtonDirectPaint;

  DSTitleButton4.Tag := 3;
  DSTitleButton4.ClickCount := csStone;
  DSTitleButton4.OnClick := DSTitleButtonClick;
  DSTitleButton4.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSTitleButton4.OnMouseMove := DSTitleButtonMouseMove;
  DSTitleButton4.OnPaint := DSTitleButtonDirectPaint;

  DSTitleButton5.Tag := 4;
  DSTitleButton5.ClickCount := csStone;
  DSTitleButton5.OnClick := DSTitleButtonClick;
  DSTitleButton5.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSTitleButton5.OnMouseMove := DSTitleButtonMouseMove;
  DSTitleButton5.OnPaint := DSTitleButtonDirectPaint;

  DSTitleButton6.Tag := 5;
  DSTitleButton6.ClickCount := csStone;
  DSTitleButton6.OnClick := DSTitleButtonClick;
  DSTitleButton6.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSTitleButton6.OnMouseMove := DSTitleButtonMouseMove;
  DSTitleButton6.OnPaint := DSTitleButtonDirectPaint;

  DSTitleNameActive.Alignment := taLeftJustify;
  DSTitleNameActive.OnPaint := DSTitleNameActiveDirectPaint;

  DSTitleName1.Tag := 0;
  DSTitleName1.Alignment := taLeftJustify;
  DSTitleName1.OnPaint := DSTitleNameDirectPaint;

  DSTitleName2.Tag := 1;
  DSTitleName2.Alignment := taLeftJustify;
  DSTitleName2.OnPaint := DSTitleNameDirectPaint;

  DSTitleName3.Tag := 2;
  DSTitleName3.Alignment := taLeftJustify;
  DSTitleName3.OnPaint := DSTitleNameDirectPaint;

  DSTitleName4.Tag := 3;
  DSTitleName4.Alignment := taLeftJustify;
  DSTitleName4.OnPaint := DSTitleNameDirectPaint;

  DSTitleName5.Tag := 4;
  DSTitleName5.Alignment := taLeftJustify;
  DSTitleName5.OnPaint := DSTitleNameDirectPaint;

  DSTitleName6.Tag := 5;
  DSTitleName6.Alignment := taLeftJustify;
  DSTitleName6.OnPaint := DSTitleNameDirectPaint;

  DSTitlePageUp.OnClick := DSTitlePageClick;
  DSTitlePageDown.OnClick := DSTitlePageClick;

  DSTitlePageUp.BringToFront;
  DSTitlePageDown.BringToFront;

  // -------------------------------------------------------------------------
  DUserState1StateTitle.OnMouseMove := DStateTitleWinMouseMove;
  DUSTitleItemDesc.OnPaint := DUSTitleItemDescOnPaint;

  DUSTitleActive.OnPaint := DUSTitleActiveDirectPaint;
  DUSTitleActive.OnMouseMove := DUSTitleAcitveMouseMove;

  DUSTitleButton1.Tag := 0;
  DUSTitleButton1.OnMouseMove := DUSTitleButtonMouseMove;
  DUSTitleButton1.OnPaint := DUSTitleButtonDirectPaint;

  DUSTitleButton2.Tag := 1;
  DUSTitleButton2.OnMouseMove := DUSTitleButtonMouseMove;
  DUSTitleButton2.OnPaint := DUSTitleButtonDirectPaint;

  DUSTitleButton3.Tag := 2;
  DUSTitleButton3.OnMouseMove := DUSTitleButtonMouseMove;
  DUSTitleButton3.OnPaint := DUSTitleButtonDirectPaint;

  DUSTitleButton4.Tag := 3;
  DUSTitleButton4.OnMouseMove := DUSTitleButtonMouseMove;
  DUSTitleButton4.OnPaint := DUSTitleButtonDirectPaint;

  DUSTitleButton5.Tag := 4;
  DUSTitleButton5.OnMouseMove := DUSTitleButtonMouseMove;
  DUSTitleButton5.OnPaint := DUSTitleButtonDirectPaint;

  DUSTitleButton6.Tag := 5;
  DUSTitleButton6.OnMouseMove := DUSTitleButtonMouseMove;
  DUSTitleButton6.OnPaint := DUSTitleButtonDirectPaint;

  DUSTitleNameActive.Alignment := taLeftJustify;
  DUSTitleNameActive.OnPaint := DUSTitleNameActiveDirectPaint;

  DUSTitleName1.Tag := 0;
  DUSTitleName1.Alignment := taLeftJustify;
  DUSTitleName1.OnPaint := DUSTitleNameDirectPaint;

  DUSTitleName2.Tag := 1;
  DUSTitleName2.Alignment := taLeftJustify;
  DUSTitleName2.OnPaint := DUSTitleNameDirectPaint;

  DUSTitleName3.Tag := 2;
  DUSTitleName3.Alignment := taLeftJustify;
  DUSTitleName3.OnPaint := DUSTitleNameDirectPaint;

  DUSTitleName4.Tag := 3;
  DUSTitleName4.Alignment := taLeftJustify;
  DUSTitleName4.OnPaint := DUSTitleNameDirectPaint;

  DUSTitleName5.Tag := 4;
  DUSTitleName5.Alignment := taLeftJustify;
  DUSTitleName5.OnPaint := DUSTitleNameDirectPaint;

  DUSTitleName6.Tag := 5;
  DUSTitleName6.Alignment := taLeftJustify;
  DUSTitleName6.OnPaint := DUSTitleNameDirectPaint;

  DUSTitlePageUp.OnClick := DUSTitlePageClick;
  DUSTitlePageDown.OnClick := DUSTitlePageClick;

  DUSTitlePageUp.BringToFront;
  DUSTitlePageDown.BringToFront;

  //----------------------------------------------------------------------

  DHeroStateTitle.OnMouseMove := DStateTitleWinMouseMove;
  DSHeroTitleItemDesc.OnPaint := DSTitleItemDescOnPaint;

  DSHeroTitleActive.ClickCount := csStone;
  DSHeroTitleActive.OnClick := DSHeroTitleActiveClick;
  DSHeroTitleActive.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSHeroTitleActive.OnMouseMove := DSHeroTitleActiveMouseMove;
  DSHeroTitleActive.OnPaint := DSHeroTitleActiveDirectPaint;

  DSHeroTitleButton1.Tag := 0;
  DSHeroTitleButton1.ClickCount := csStone;
  DSHeroTitleButton1.OnClick := DSHeroTitleButtonClick;
  DSHeroTitleButton1.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSHeroTitleButton1.OnMouseMove := DSHeroTitleButtonMouseMove;
  DSHeroTitleButton1.OnPaint := DSHeroTitleButtonDirectPaint;

  DSHeroTitleButton2.Tag := 1;
  DSHeroTitleButton2.ClickCount := csStone;
  DSHeroTitleButton2.OnClick := DSHeroTitleButtonClick;
  DSHeroTitleButton2.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSHeroTitleButton2.OnMouseMove := DSHeroTitleButtonMouseMove;
  DSHeroTitleButton2.OnPaint := DSHeroTitleButtonDirectPaint;

  DSHeroTitleButton3.Tag := 2;
  DSHeroTitleButton3.ClickCount := csStone;
  DSHeroTitleButton3.OnClick := DSHeroTitleButtonClick;
  DSHeroTitleButton3.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSHeroTitleButton3.OnMouseMove := DSHeroTitleButtonMouseMove;
  DSHeroTitleButton3.OnPaint := DSHeroTitleButtonDirectPaint;

  DSHeroTitleButton4.Tag := 3;
  DSHeroTitleButton4.ClickCount := csStone;
  DSHeroTitleButton4.OnClick := DSHeroTitleButtonClick;
  DSHeroTitleButton4.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSHeroTitleButton4.OnMouseMove := DSHeroTitleButtonMouseMove;
  DSHeroTitleButton4.OnPaint := DSHeroTitleButtonDirectPaint;

  DSHeroTitleButton5.Tag := 4;
  DSHeroTitleButton5.ClickCount := csStone;
  DSHeroTitleButton5.OnClick := DSHeroTitleButtonClick;
  DSHeroTitleButton5.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSHeroTitleButton5.OnMouseMove := DSHeroTitleButtonMouseMove;
  DSHeroTitleButton5.OnPaint := DSHeroTitleButtonDirectPaint;

  DSHeroTitleButton6.Tag := 5;
  DSHeroTitleButton6.ClickCount := csStone;
  DSHeroTitleButton6.OnClick := DSHeroTitleButtonClick;
  DSHeroTitleButton6.OnClickSound := FrmDlg.DLoginNewClickSound;
  DSHeroTitleButton6.OnMouseMove := DSHeroTitleButtonMouseMove;
  DSHeroTitleButton6.OnPaint := DSHeroTitleButtonDirectPaint;

  DSHeroTitleNameActive.Alignment := taLeftJustify;
  DSHeroTitleNameActive.OnPaint := DSHeroTitleNameActiveDirectPaint;

  DSHeroTitleName1.Tag := 0;
  DSHeroTitleName1.Alignment := taLeftJustify;
  DSHeroTitleName1.OnPaint := DSHeroTitleNameDirectPaint;

  DSHeroTitleName2.Tag := 1;
  DSHeroTitleName2.Alignment := taLeftJustify;
  DSHeroTitleName2.OnPaint := DSHeroTitleNameDirectPaint;

  DSHeroTitleName3.Tag := 2;
  DSHeroTitleName3.Alignment := taLeftJustify;
  DSHeroTitleName3.OnPaint := DSHeroTitleNameDirectPaint;

  DSHeroTitleName4.Tag := 3;
  DSHeroTitleName4.Alignment := taLeftJustify;
  DSHeroTitleName4.OnPaint := DSHeroTitleNameDirectPaint;

  DSHeroTitleName5.Tag := 4;
  DSHeroTitleName5.Alignment := taLeftJustify;
  DSHeroTitleName5.OnPaint := DSHeroTitleNameDirectPaint;

  DSHeroTitleName6.Tag := 5;
  DSHeroTitleName6.Alignment := taLeftJustify;
  DSHeroTitleName6.OnPaint := DSHeroTitleNameDirectPaint;

  DSHeroTitlePageUp.OnClick := DSHeroTitlePageClick;
  DSHeroTitlePageDown.OnClick := DSHeroTitlePageClick;

  DSHeroTitlePageUp.BringToFront;
  DSHeroTitlePageDown.BringToFront;
  //-----------------------------------------------------------------------

  ContinuousMagicMenu.OnClick := ContinuousMagicMenuClick;

  DSMBShortcut.OnClick := DSMBShortcutClick;

  DStatePageControl.ComponentIndex := 0;
  // DStatePageControl.BringToFront;
  LabelDStateWinCharName.OnClick := LabelDStateWinCharNameClick;
  LabelDStateWinRankName.OnClick := LabelDStateWinCharNameClick;
  DUserState1LabelDStateWinCharName.OnClick := LabelDStateWinCharNameClick;

  DUserState1LabelDStateWinRankName.OnClick := LabelDStateWinCharNameClick;
  DUserState1LabelDStateWinRankName.BringToFront;

  //------------------------------------------------------------------------

  DSWPets.OnClick := DSWPetsClick;
  DSWPets.OnMouseMove := DControlMouseMoveShowHint;
  DSWPetsDlg.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DSWPetsClose.OnClick := DSWPetsCloseClick;
  DSWPetsList.OnViewItemPaint := DSWPetsListViewItemPaint;
  DSWPetsList.OnListItemClick := DSWPetsListViewItemClick;

  DSWPetsShow.OnStopPaint := DSWPetsShowStopPaint;

  DSWPetsTakeBag.OnClick := DSWPetsButtonClick;
  DSWPetsRecall.OnClick := DSWPetsButtonClick;
  DSWPetsRetake.OnClick := DSWPetsButtonClick;
  DSWPetsFree.OnClick := DSWPetsButtonClick;
  DSWPetsViewBag.OnClick := DSWPetsButtonClick;
  DSWPetsViewBag.OnClick := DSWPetsButtonClick;

  {
  DSWPetsItem1.Tag := 0;
  DSWPetsItem2.Tag := 1;
  DSWPetsItem3.Tag := 2;

  DSWPetsItem1.OnMouseMove := DSWPetsItemMouseMove;
  DSWPetsItem2.OnMouseMove := DSWPetsItemMouseMove;
  DSWPetsItem3.OnMouseMove := DSWPetsItemMouseMove;

  DSWPetsItem1.OnClick := DSWPetsItemClick;
  DSWPetsItem2.OnClick := DSWPetsItemClick;
  DSWPetsItem3.OnClick := DSWPetsItemClick;

  DSWPetsItem1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWPetsItem2.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWPetsItem3.OnInRealArea := FrmDlg.DBottomInRealArea;

  DSWPetsItem1.OnPaint := DSWPetsItemPaint;
  DSWPetsItem2.OnPaint := DSWPetsItemPaint;
  DSWPetsItem3.OnPaint := DSWPetsItemPaint;
  }

  DSWPetsBagClose.OnClick := DSWPetsBagCloseClick;

  DSWPetsGrid.OnDblClick := DSWPetsGridDblClick;
  DSWPetsGrid.OnGridSelect := DSWPetsGridGridSelect;
  DSWPetsGrid.OnGridPaint := DSWPetsGridGridPaint;
  DSWPetsGrid.OnGridMouseMove := DSWPetsGridGridMouseMove;

  //----------------------------------------

  DSWPetsMagic1.Tag := 0;
  DSWPetsMagic2.Tag := 1;
  DSWPetsMagic3.Tag := 2;
  DSWPetsMagic4.Tag := 3;
  DSWPetsMagic5.Tag := 4;
  DSWPetsMagic6.Tag := 5;
  DSWPetsMagic7.Tag := 6;
  DSWPetsMagic8.Tag := 7;

  DSWPetsMagic1.OnMouseMove := DSWPetsMagicMouseMove;
  DSWPetsMagic2.OnMouseMove := DSWPetsMagicMouseMove;
  DSWPetsMagic3.OnMouseMove := DSWPetsMagicMouseMove;
  DSWPetsMagic4.OnMouseMove := DSWPetsMagicMouseMove;
  DSWPetsMagic5.OnMouseMove := DSWPetsMagicMouseMove;
  DSWPetsMagic6.OnMouseMove := DSWPetsMagicMouseMove;
  DSWPetsMagic7.OnMouseMove := DSWPetsMagicMouseMove;
  DSWPetsMagic8.OnMouseMove := DSWPetsMagicMouseMove;

  DSWPetsMagic1.OnClick := DSWPetsMagicClick;
  DSWPetsMagic2.OnClick := DSWPetsMagicClick;
  DSWPetsMagic3.OnClick := DSWPetsMagicClick;
  DSWPetsMagic4.OnClick := DSWPetsMagicClick;
  DSWPetsMagic5.OnClick := DSWPetsMagicClick;
  DSWPetsMagic6.OnClick := DSWPetsMagicClick;
  DSWPetsMagic7.OnClick := DSWPetsMagicClick;
  DSWPetsMagic8.OnClick := DSWPetsMagicClick;

  DSWPetsMagic1.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWPetsMagic2.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWPetsMagic3.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWPetsMagic4.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWPetsMagic5.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWPetsMagic6.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWPetsMagic7.OnInRealArea := FrmDlg.DBottomInRealArea;
  DSWPetsMagic8.OnInRealArea := FrmDlg.DBottomInRealArea;

  DSWPetsMagic1.OnPaint := DSWPetsMagicPaint;
  DSWPetsMagic2.OnPaint := DSWPetsMagicPaint;
  DSWPetsMagic3.OnPaint := DSWPetsMagicPaint;
  DSWPetsMagic4.OnPaint := DSWPetsMagicPaint;
  DSWPetsMagic5.OnPaint := DSWPetsMagicPaint;
  DSWPetsMagic6.OnPaint := DSWPetsMagicPaint;
  DSWPetsMagic7.OnPaint := DSWPetsMagicPaint;
  DSWPetsMagic8.OnPaint := DSWPetsMagicPaint;

  DCloseState.BringToFront;

  btnDStateWinEx.BringToFront;
  btnDStateWinEx.OnClick := OnDStateWinExClick;
  DStateWin.OnMove := DStateWinMove;
  DStateWinEx.NoMove := True;
end;

procedure TStateWindows.DRecallDeputyHeroClick(Sender:TObject; X, Y:Integer); // 召唤副将英雄
var
  btJob:Byte;
begin
  btJob := 0;
  if DBotStateDeputyHeroJob0.Checked then
    btJob := 0
  else if DBotStateDeputyHeroJob1.Checked then
    btJob := 1
  else if DBotStateDeputyHeroJob2.Checked then
    btJob := 2;
  frmMain.SendClientMessage(CM_HEROLOGON, 1, btJob, 0, 0);
end;

procedure TStateWindows.InitHero;
var
  I:Integer;
begin
  DHeroStateFashion.OnStartSubPaint := DHeroStateWinFashionDirectPaint;

  DHeroStateWin.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStateBasic.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStateFashion.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStateInfo.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStateAbil.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStateMagic.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStMagBack6.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStMagBack5.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStateNGInfo.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStateNGMagic.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStateNGMeridians.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroMeridiansLine0.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroMeridiansLine1.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroMeridiansLine2.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroMeridiansLine3.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroMeridiansLine4.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStateNGContinuousMagic.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroContinuousMagicList.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStLJMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStLJMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStLJMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DHeroStLJMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStMagBtn1.Tag := 0;
  DHeroStMagBtn1.Visible := False;
  DHeroStMagBtn2.Tag := 0;
  DHeroStMagBtn2.Visible := False;
  DHeroStMagBtn3.Tag := 0;
  DHeroStMagBtn3.Visible := False;
  DHeroStMagBtn4.Tag := 0;
  DHeroStMagBtn4.Visible := False;
  DHeroStMagBtn5.Tag := 0;
  DHeroStMagBtn5.Visible := False;
  DHeroStMagBtn6.Tag := 0;
  DHeroStMagBtn6.Visible := False;

  DHeroStMagBtn1.OnClick := FrmDlg.DStHeroUpgradeMagicButtonClick;
  DHeroStMagBtn2.OnClick := FrmDlg.DStHeroUpgradeMagicButtonClick;
  DHeroStMagBtn3.OnClick := FrmDlg.DStHeroUpgradeMagicButtonClick;
  DHeroStMagBtn4.OnClick := FrmDlg.DStHeroUpgradeMagicButtonClick;
  DHeroStMagBtn5.OnClick := FrmDlg.DStHeroUpgradeMagicButtonClick;
  DHeroStMagBtn6.OnClick := FrmDlg.DStHeroUpgradeMagicButtonClick;

  DHeroStMagBtn1.OnMouseMove := FrmDlg.DHeroStUpgradeMagicMouseMove;
  DHeroStMagBtn2.OnMouseMove := FrmDlg.DHeroStUpgradeMagicMouseMove;
  DHeroStMagBtn3.OnMouseMove := FrmDlg.DHeroStUpgradeMagicMouseMove;
  DHeroStMagBtn4.OnMouseMove := FrmDlg.DHeroStUpgradeMagicMouseMove;
  DHeroStMagBtn5.OnMouseMove := FrmDlg.DHeroStUpgradeMagicMouseMove;
  DHeroStMagBtn6.OnMouseMove := FrmDlg.DHeroStUpgradeMagicMouseMove;

  {------------------------------------------------------------------------------}
  DHeroStatePageControl.ActivePageIndex := 0;
  DHeroBasicPageControl.ActivePageIndex := 0;
  DHeroNGPageControl.ActivePageIndex := 0;
  DHeroMeridiansPageControl.ActivePageIndex := 0;
  DHeroMeridiansPageControl.ActivePageIndex := 1;
  DHeroBotMeridians1.Checked := True;

  {------------------------------------------------------------------------------}
  HeroMeridiansFormArray[0] := DHeroMeridiansLine1;
  HeroMeridiansFormArray[1] := DHeroMeridiansLine2;
  HeroMeridiansFormArray[2] := DHeroMeridiansLine3;
  HeroMeridiansFormArray[3] := DHeroMeridiansLine4;
  HeroMeridiansFormArray[4] := DHeroMeridiansLine0;

  for I := 0 to Length(HeroMeridiansFormArray) - 1 do begin
    HeroMeridiansFormArray[I].OnStartSubPaint := DHeroMeridiansLineDirectPaint;
    HeroMeridiansFormArray[I].Tag := I;
  end;

  DHeroCloseState.OnClick := DHeroCloseStateClick;
  DHeroCloseState.ClickCount := csNorm;
  DHeroCloseState.OnClickSound := FrmDlg.DLoginNewClickSound;

  {--------------------------------------------------------------------------}
  DHeroTrainingMeridian.OnClick := DHeroTrainingMeridianClick; // 修炼经脉按钮

  DHeroBotMeridians0.OnClick := DHeroBotMeridiansClick;
  DHeroBotMeridians1.OnClick := DHeroBotMeridiansClick;
  DHeroBotMeridians2.OnClick := DHeroBotMeridiansClick;
  DHeroBotMeridians3.OnClick := DHeroBotMeridiansClick;
  DHeroBotMeridians4.OnClick := DHeroBotMeridiansClick;
  DHeroBotMeridians0.Tag := 0;
  DHeroBotMeridians1.Tag := 1;
  DHeroBotMeridians2.Tag := 2;
  DHeroBotMeridians3.Tag := 3;
  DHeroBotMeridians4.Tag := 4;

  HeroAcupointArray[0, 0] := DHeroBotAcupoints1_0;
  HeroAcupointArray[0, 1] := DHeroBotAcupoints1_1;
  HeroAcupointArray[0, 2] := DHeroBotAcupoints1_2;
  HeroAcupointArray[0, 3] := DHeroBotAcupoints1_3;
  HeroAcupointArray[0, 4] := DHeroBotAcupoints1_4;

  HeroAcupointArray[1, 0] := DHeroBotAcupoints2_0;
  HeroAcupointArray[1, 1] := DHeroBotAcupoints2_1;
  HeroAcupointArray[1, 2] := DHeroBotAcupoints2_2;
  HeroAcupointArray[1, 3] := DHeroBotAcupoints2_3;
  HeroAcupointArray[1, 4] := DHeroBotAcupoints2_4;

  HeroAcupointArray[2, 0] := DHeroBotAcupoints3_0;
  HeroAcupointArray[2, 1] := DHeroBotAcupoints3_1;
  HeroAcupointArray[2, 2] := DHeroBotAcupoints3_2;
  HeroAcupointArray[2, 3] := DHeroBotAcupoints3_3;
  HeroAcupointArray[2, 4] := DHeroBotAcupoints3_4;

  HeroAcupointArray[3, 0] := DHeroBotAcupoints4_0;
  HeroAcupointArray[3, 1] := DHeroBotAcupoints4_1;
  HeroAcupointArray[3, 2] := DHeroBotAcupoints4_2;
  HeroAcupointArray[3, 3] := DHeroBotAcupoints4_3;
  HeroAcupointArray[3, 4] := DHeroBotAcupoints4_4;

  HeroAcupointArray[4, 0] := DHeroBotAcupoints0_0;
  HeroAcupointArray[4, 1] := DHeroBotAcupoints0_1;
  HeroAcupointArray[4, 2] := DHeroBotAcupoints0_2;
  HeroAcupointArray[4, 3] := DHeroBotAcupoints0_3;
  HeroAcupointArray[4, 4] := DHeroBotAcupoints0_4;

  {---------------------------------奇经-----------------------------------------}
  DHeroBotAcupoints0_0.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints0_1.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints0_2.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints0_3.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints0_4.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints0_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints0_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints0_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints0_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints0_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints0_0.Tag := 0;
  DHeroBotAcupoints0_1.Tag := 1;
  DHeroBotAcupoints0_2.Tag := 2;
  DHeroBotAcupoints0_3.Tag := 3;
  DHeroBotAcupoints0_4.Tag := 4;

  DHeroLabelAcupoints0_0.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints0_1.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints0_2.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints0_3.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints0_4.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints0_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints0_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints0_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints0_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints0_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints0_0.Tag := 0;
  DHeroLabelAcupoints0_1.Tag := 1;
  DHeroLabelAcupoints0_2.Tag := 2;
  DHeroLabelAcupoints0_3.Tag := 3;
  DHeroLabelAcupoints0_4.Tag := 4;
  {---------------------------------冲脉-----------------------------------------}
  DHeroBotAcupoints1_0.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints1_1.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints1_2.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints1_3.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints1_4.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints1_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints1_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints1_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints1_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints1_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints1_0.Tag := 0;
  DHeroBotAcupoints1_1.Tag := 1;
  DHeroBotAcupoints1_2.Tag := 2;
  DHeroBotAcupoints1_3.Tag := 3;
  DHeroBotAcupoints1_4.Tag := 4;

  DHeroLabelAcupoints1_0.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints1_1.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints1_2.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints1_3.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints1_4.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints1_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints1_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints1_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints1_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints1_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints1_0.Tag := 0;
  DHeroLabelAcupoints1_1.Tag := 1;
  DHeroLabelAcupoints1_2.Tag := 2;
  DHeroLabelAcupoints1_3.Tag := 3;
  DHeroLabelAcupoints1_4.Tag := 4;
  {---------------------------------阴跷-----------------------------------------}
  DHeroBotAcupoints2_0.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints2_1.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints2_2.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints2_3.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints2_4.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints2_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints2_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints2_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints2_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints2_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints2_0.Tag := 0;
  DHeroBotAcupoints2_1.Tag := 1;
  DHeroBotAcupoints2_2.Tag := 2;
  DHeroBotAcupoints2_3.Tag := 3;
  DHeroBotAcupoints2_4.Tag := 4;

  DHeroLabelAcupoints2_0.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints2_1.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints2_2.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints2_3.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints2_4.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints2_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints2_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints2_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints2_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints2_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints2_0.Tag := 0;
  DHeroLabelAcupoints2_1.Tag := 1;
  DHeroLabelAcupoints2_2.Tag := 2;
  DHeroLabelAcupoints2_3.Tag := 3;
  DHeroLabelAcupoints2_4.Tag := 4;
  {---------------------------------阴维-----------------------------------------}
  DHeroBotAcupoints3_0.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints3_1.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints3_2.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints3_3.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints3_4.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints3_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints3_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints3_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints3_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints3_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints3_0.Tag := 0;
  DHeroBotAcupoints3_1.Tag := 1;
  DHeroBotAcupoints3_2.Tag := 2;
  DHeroBotAcupoints3_3.Tag := 3;
  DHeroBotAcupoints3_4.Tag := 4;

  DHeroLabelAcupoints3_0.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints3_1.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints3_2.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints3_3.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints3_4.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints3_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints3_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints3_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints3_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints3_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints3_0.Tag := 0;
  DHeroLabelAcupoints3_1.Tag := 1;
  DHeroLabelAcupoints3_2.Tag := 2;
  DHeroLabelAcupoints3_3.Tag := 3;
  DHeroLabelAcupoints3_4.Tag := 4;
  {---------------------------------任脉-----------------------------------------}
  DHeroBotAcupoints4_0.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints4_1.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints4_2.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints4_3.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints4_4.OnClick := DHeroBotAcupointsClick;
  DHeroBotAcupoints4_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints4_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints4_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints4_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints4_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroBotAcupoints4_0.Tag := 0;
  DHeroBotAcupoints4_1.Tag := 1;
  DHeroBotAcupoints4_2.Tag := 2;
  DHeroBotAcupoints4_3.Tag := 3;
  DHeroBotAcupoints4_4.Tag := 4;

  DHeroLabelAcupoints4_0.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints4_1.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints4_2.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints4_3.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints4_4.OnClick := DHeroBotAcupointsClick;
  DHeroLabelAcupoints4_0.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints4_1.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints4_2.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints4_3.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints4_4.OnMouseMove := DHeroBotAcupointsMouseMove;
  DHeroLabelAcupoints4_0.Tag := 0;
  DHeroLabelAcupoints4_1.Tag := 1;
  DHeroLabelAcupoints4_2.Tag := 2;
  DHeroLabelAcupoints4_3.Tag := 3;
  DHeroLabelAcupoints4_4.Tag := 4;
  {-------------------------------------------------------------------------------}
  DHeroStateWin.OnUpDate := DHeroStateWinUpDate;
  DHeroStateBasic.OnStartSubPaint := DHeroStateWinDirectPaint;

  DHeroSWArmRingL.OnClick := DHeroSWWeaponClick;
  DHeroSWArmRingR.OnClick := DHeroSWWeaponClick;
  DHeroSWBelt.OnClick := DHeroSWWeaponClick;
  DHeroSWBoots.OnClick := DHeroSWWeaponClick;
  DHeroSWBujuk.OnClick := DHeroSWWeaponClick;
  DHeroSWCharm.OnClick := DHeroSWWeaponClick;
  DHeroSWDress.OnClick := DHeroSWWeaponClick;
  DHeroSWHelmet.OnClick := DHeroSWWeaponClick;
  DHeroSWLight.OnClick := DHeroSWWeaponClick;
  DHeroSWNecklace.OnClick := DHeroSWWeaponClick;
  DHeroSWRingL.OnClick := DHeroSWWeaponClick;
  DHeroSWRingR.OnClick := DHeroSWWeaponClick;
  DHeroSWWeapon.OnClick := DHeroSWWeaponClick;
  DHeroSWDrum.OnClick := DHeroSWWeaponClick;
  DHeroSWHorse.OnClick := DHeroSWWeaponClick;
  DHeroSWShield.OnClick := DHeroSWWeaponClick;
  DHeroSWJade.OnClick := DHeroSWWeaponClick;

  DHeroSWFashionDress.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionWeapon.OnClick := DHeroSWWeaponClick;
  DHeroSWShowFashion.OnClick := DSWShowFashionClick;
  DHeroSWShowFashion.Checked := False;

  DHeroSWFashionHelmet.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionNecklace.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionArmRingR.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionArmRingL.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionRingR.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionRingL.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionLight.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionBelt.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionBoots.OnClick := DHeroSWWeaponClick;
  DHeroSWFashionDrum.OnClick := DHeroSWWeaponClick;

  DHeroSWArmRingL.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWArmRingR.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWBelt.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWBoots.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWBujuk.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWCharm.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWDress.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWHelmet.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWLight.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWNecklace.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWRingL.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWRingR.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWWeapon.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWDrum.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWHorse.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWShield.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWJade.OnMouseMove := DHeroSWWeaponMouseMove;

  DHeroSWFashionDress.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionWeapon.OnMouseMove := DHeroSWWeaponMouseMove;

  DHeroSWFashionHelmet.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionNecklace.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionArmRingR.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionArmRingL.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionRingR.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionRingL.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionLight.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionBelt.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionBoots.OnMouseMove := DHeroSWWeaponMouseMove;
  DHeroSWFashionDrum.OnMouseMove := DHeroSWWeaponMouseMove;

  DHeroSWArmRingL.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWArmRingR.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWBelt.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWBoots.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWBujuk.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWCharm.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWDress.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWHelmet.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWLight.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWNecklace.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWRingL.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWRingR.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWWeapon.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWDrum.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWHorse.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWShield.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWJade.OnInRealArea := FrmDlg.DBottomInRealArea;

  DHeroSWFashionDress.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionWeapon.OnInRealArea := FrmDlg.DBottomInRealArea;

  DHeroSWFashionHelmet.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionNecklace.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionArmRingR.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionArmRingL.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionRingR.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionRingL.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionLight.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionBelt.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionBoots.OnInRealArea := FrmDlg.DBottomInRealArea;
  DHeroSWFashionDrum.OnInRealArea := FrmDlg.DBottomInRealArea;

  DHeroSWArmRingL.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWArmRingR.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWBelt.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWBoots.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWBujuk.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWCharm.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWLight.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWNecklace.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWRingL.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWRingR.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWDrum.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWHorse.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWJade.OnPaint := DHeroSWLightDirectPaint;
  //DHeroSWShield.OnPaint := DHeroSWLightDirectPaint;

  //DHeroSWFashionHelmet.Tag := U_FASHIONHELMET;
  DHeroSWFashionNecklace.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWFashionArmRingR.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWFashionArmRingL.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWFashionRingR.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWFashionRingL.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWFashionLight.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWFashionBelt.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWFashionBoots.OnPaint := DHeroSWLightDirectPaint;
  DHeroSWFashionDrum.OnPaint := DHeroSWLightDirectPaint;

  DHeroSWDress.Tag := U_DRESS;
  DHeroSWWeapon.Tag := U_WEAPON;
  DHeroSWHelmet.Tag := U_HELMET;
  DHeroSWNecklace.Tag := U_NECKLACE;
  DHeroSWLight.Tag := U_RIGHTHAND;
  DHeroSWArmRingR.Tag := U_ARMRINGR;
  DHeroSWArmRingL.Tag := U_ARMRINGL;
  DHeroSWRingR.Tag := U_RINGR;
  DHeroSWRingL.Tag := U_RINGL;
  DHeroSWBujuk.Tag := U_BUJUK;
  DHeroSWBelt.Tag := U_BELT;
  DHeroSWBoots.Tag := U_BOOTS;
  DHeroSWCharm.Tag := U_CHARM;
  DHeroSWDrum.Tag := U_DRUM;
  DHeroSWHorse.Tag := U_HORSE;
  DHeroSWShield.Tag := U_SHIELD;
  DHeroSWJade.Tag := U_JADE;
  DHeroSWFashionDress.Tag := U_FASHIONDRESS;
  DHeroSWFashionWeapon.Tag := U_FASHIONWEAPON;

  DHeroSWFashionHelmet.Tag := U_FASHIONHELMET;
  DHeroSWFashionNecklace.Tag := U_FASHIONNECKLACE;
  DHeroSWFashionArmRingR.Tag := U_FASHIONARMRINGR;
  DHeroSWFashionArmRingL.Tag := U_FASHIONARMRINGL;
  DHeroSWFashionRingR.Tag := U_FASHIONRINGR;
  DHeroSWFashionRingL.Tag := U_FASHIONRINGL;
  DHeroSWFashionLight.Tag := U_FASHIONRIGHTHAND;
  DHeroSWFashionBelt.Tag := U_FASHIONBELT;
  DHeroSWFashionBoots.Tag := U_FASHIONBOOTS;
  DHeroSWFashionDrum.Tag := U_FASHIONCHARM;

  {------------------------------------------------------------------------------}
  DHeroStMagBack1.Tag := 0;
  DHeroStMagBack1.OnStartPaint := DHeroStateMagicDirectPaint;
  DHeroStMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStMagBack2.Tag := 1;
  DHeroStMagBack2.OnStartPaint := DHeroStateMagicDirectPaint;
  DHeroStMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStMagBack3.Tag := 2;
  DHeroStMagBack3.OnStartPaint := DHeroStateMagicDirectPaint;
  DHeroStMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStMagBack4.Tag := 3;
  DHeroStMagBack4.OnStartPaint := DHeroStateMagicDirectPaint;
  DHeroStMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStMagBack5.Tag := 4;
  DHeroStMagBack5.OnStartPaint := DHeroStateMagicDirectPaint;
  DHeroStMagBack5.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStMagBack6.Tag := 5;
  DHeroStMagBack6.OnStartPaint := DHeroStateMagicDirectPaint;
  DHeroStMagBack6.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStMag1.ClickCount := csStone;
  DHeroStMag1.OnClick := DHeroStMag1Click;
  DHeroStMag1.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStMag1.OnMouseMove := DHeroStMagMouseMove;
  DHeroStMag1.OnPaint := DHeroStMag1DirectPaint;
  DHeroStMag1.Tag := 0;

  DStHeroMagLblName1.Visible := False;
  DStHeroMagLvIcon1.Visible := False;
  DStHeroMagLvText1.Visible := False;
  DStHeroMagExpIcon1.Visible := False;
  DStHeroMagExpText1.Visible := False;

  DHeroStMag2.ClickCount := csStone;
  DHeroStMag2.OnClick := DHeroStMag1Click;
  DHeroStMag2.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStMag2.OnMouseMove := DHeroStMagMouseMove;
  DHeroStMag2.OnPaint := DHeroStMag1DirectPaint;
  DHeroStMag2.Tag := 1;

  DHeroStMag3.ClickCount := csStone;
  DHeroStMag3.OnClick := DHeroStMag1Click;
  DHeroStMag3.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStMag3.OnMouseMove := DHeroStMagMouseMove;
  DHeroStMag3.OnPaint := DHeroStMag1DirectPaint;
  DHeroStMag3.Tag := 2;

  DHeroStMag4.ClickCount := csStone;
  DHeroStMag4.OnClick := DHeroStMag1Click;
  DHeroStMag4.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStMag4.OnMouseMove := DHeroStMagMouseMove;
  DHeroStMag4.OnPaint := DHeroStMag1DirectPaint;
  DHeroStMag4.Tag := 3;

  DHeroStMag5.ClickCount := csStone;
  DHeroStMag5.OnClick := DHeroStMag1Click;
  DHeroStMag5.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStMag5.OnMouseMove := DHeroStMagMouseMove;
  DHeroStMag5.OnPaint := DHeroStMag1DirectPaint;
  DHeroStMag5.Tag := 4;

  DHeroStMag6.ClickCount := csStone;
  DHeroStMag6.OnClick := DHeroStMag1Click;
  DHeroStMag6.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStMag6.OnMouseMove := DHeroStMagMouseMove;
  DHeroStMag6.OnPaint := DHeroStMag1DirectPaint;
  DHeroStMag6.Tag := 5;

  DHeroStPageUp.ClickCount := csNorm;
  DHeroStPageDown.ClickCount := csNorm;
  DHeroStPageUp.OnClick := DHeroStPageUpClick;
  DHeroStPageUp.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStPageDown.OnClick := DHeroStPageUpClick;
  DHeroStPageDown.OnClickSound := FrmDlg.DLoginNewClickSound;

  {------------------------------------------------------------------------------}
  DHeroStNGMagBack1.Tag := 0;
  DHeroStNGMagBack1.OnStartPaint := DHeroStateNGMagicDirectPaint;
  DHeroStNGMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStNGMagBack2.Tag := 1;
  DHeroStNGMagBack2.OnStartPaint := DHeroStateNGMagicDirectPaint;
  DHeroStNGMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStNGMagBack3.Tag := 2;
  DHeroStNGMagBack3.OnStartPaint := DHeroStateNGMagicDirectPaint;
  DHeroStNGMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStNGMagBack4.Tag := 3;
  DHeroStNGMagBack4.OnStartPaint := DHeroStateNGMagicDirectPaint;
  DHeroStNGMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStNGMagBack5.Tag := 4;
  DHeroStNGMagBack5.OnStartPaint := DHeroStateNGMagicDirectPaint;
  DHeroStNGMagBack5.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStNGMagBack6.Tag := 5;
  DHeroStNGMagBack6.OnStartPaint := DHeroStateNGMagicDirectPaint;
  DHeroStNGMagBack6.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStNGMag1.ClickCount := csStone;
  DHeroStNGMag1.OnClick := DHeroStNGMag1Click;
  DHeroStNGMag1.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStNGMag1.OnMouseMove := DHeroStNGMagMouseMove;
  DHeroStNGMag1.OnPaint := DHeroStNGMag1DirectPaint;
  DHeroStNGMag1.Tag := 0;

  DHeroStNGLblName1.Visible := False;
  DHeroStNGLvIcon1.Visible := False;
  DHeroStNGLvText1.Visible := False;
  DHeroStNGExpIcon1.Visible := False;
  DHeroStNGExpText1.Visible := False;

  DHeroStNGMag2.ClickCount := csStone;
  DHeroStNGMag2.OnClick := DHeroStNGMag1Click;
  DHeroStNGMag2.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStNGMag2.OnMouseMove := DHeroStNGMagMouseMove;
  DHeroStNGMag2.OnPaint := DHeroStNGMag1DirectPaint;
  DHeroStNGMag2.Tag := 1;

  DHeroStNGMag3.ClickCount := csStone;
  DHeroStNGMag3.OnClick := DHeroStNGMag1Click;
  DHeroStNGMag3.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStNGMag3.OnMouseMove := DHeroStNGMagMouseMove;
  DHeroStNGMag3.OnPaint := DHeroStNGMag1DirectPaint;
  DHeroStNGMag3.Tag := 2;

  DHeroStNGMag4.ClickCount := csStone;
  DHeroStNGMag4.OnClick := DHeroStNGMag1Click;
  DHeroStNGMag4.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStNGMag4.OnMouseMove := DHeroStNGMagMouseMove;
  DHeroStNGMag4.OnPaint := DHeroStNGMag1DirectPaint;
  DHeroStNGMag4.Tag := 3;

  DHeroStNGMag5.ClickCount := csStone;
  DHeroStNGMag5.OnClick := DHeroStNGMag1Click;
  DHeroStNGMag5.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStNGMag5.OnMouseMove := DHeroStNGMagMouseMove;
  DHeroStNGMag5.OnPaint := DHeroStNGMag1DirectPaint;
  DHeroStNGMag5.Tag := 4;

  DHeroStNGMag6.ClickCount := csStone;
  DHeroStNGMag6.OnClick := DHeroStNGMag1Click;
  DHeroStNGMag6.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStNGMag6.OnMouseMove := DHeroStNGMagMouseMove;
  DHeroStNGMag6.OnPaint := DHeroStNGMag1DirectPaint;
  DHeroStNGMag6.Tag := 5;

  DHeroStNGPageUp.ClickCount := csNorm;
  DHeroStNGPageDown.ClickCount := csNorm;
  DHeroStNGPageUp.OnClick := DHeroStNGPageUpClick;
  DHeroStNGPageUp.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStNGPageDown.OnClick := DHeroStNGPageUpClick;
  DHeroStNGPageDown.OnClickSound := FrmDlg.DLoginNewClickSound;

  {------------------------------------------------------------------------------}
  DHeroStLJMagBack1.Tag := 0;
  DHeroStLJMagBack1.OnStartPaint := DHeroStateLJMagicDirectPaint;
  DHeroStLJMagBack1.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStLJMagBack2.Tag := 1;
  DHeroStLJMagBack2.OnStartPaint := DHeroStateLJMagicDirectPaint;
  DHeroStLJMagBack2.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStLJMagBack3.Tag := 2;
  DHeroStLJMagBack3.OnStartPaint := DHeroStateLJMagicDirectPaint;
  DHeroStLJMagBack3.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStLJMagBack4.Tag := 3;
  DHeroStLJMagBack4.OnStartPaint := DHeroStateLJMagicDirectPaint;
  DHeroStLJMagBack4.OnMouseMove := FrmDlg.DStateWinMouseMove;

  DHeroStLJMag1.ClickCount := csStone;
  DHeroStLJMag1.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStLJMag1.OnMouseMove := DHeroStLJMagMouseMove;
  DHeroStLJMag1.OnPaint := DHeroStLJMag1DirectPaint;
  DHeroStLJMag1.Tag := 0;

  DHeroStLJLblName1.Visible := False;
  DHeroStLJLvIcon1.Visible := False;
  DHeroStLJLvText1.Visible := False;
  DHeroStLJExpIcon1.Visible := False;
  DHeroStLJExpText1.Visible := False;

  DHeroStLJMag2.ClickCount := csStone;
  DHeroStLJMag2.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStLJMag2.OnMouseMove := DHeroStLJMagMouseMove;
  DHeroStLJMag2.OnPaint := DHeroStLJMag1DirectPaint;
  DHeroStLJMag2.Tag := 1;

  DHeroStLJMag3.ClickCount := csStone;
  DHeroStLJMag3.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStLJMag3.OnMouseMove := DHeroStLJMagMouseMove;
  DHeroStLJMag3.OnPaint := DHeroStLJMag1DirectPaint;
  DHeroStLJMag3.Tag := 2;

  DHeroStLJMag4.ClickCount := csStone;
  DHeroStLJMag4.OnClickSound := FrmDlg.DLoginNewClickSound;
  DHeroStLJMag4.OnMouseMove := DHeroStLJMagMouseMove;
  DHeroStLJMag4.OnPaint := DHeroStLJMag1DirectPaint;
  DHeroStLJMag4.Tag := 3;

  DHeroSMB1.OnPaint := DHeroSMB1DirectPaint;
  DHeroSMB2.OnPaint := DHeroSMB1DirectPaint;
  DHeroSMB3.OnPaint := DHeroSMB1DirectPaint;
  DHeroSMB4.OnPaint := DHeroSMB1DirectPaint;
  DHeroSMB1.OnClick := DHeroSMB1Click;
  DHeroSMB2.OnClick := DHeroSMB1Click;
  DHeroSMB3.OnClick := DHeroSMB1Click;
  DHeroSMB4.OnClick := DHeroSMB1Click;
  DHeroSMB1.Tag := 0;
  DHeroSMB2.Tag := 1;
  DHeroSMB3.Tag := 2;
  DHeroSMB4.Tag := 3;
  HeroContinuousMagicMenu.OnClick := HeroContinuousMagicMenuClick;
  DHeroStatePageControl.ComponentIndex := 0;
  DHeroStatePageControl.BringToFront;

  DHeroCloseState.BringToFront;
end;

procedure TStateWindows.InitUserState1;
begin
  DUserState1.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DUserState1StateBasic.OnMouseMove := FrmDlg.DStateWinMouseMove;
  DUserState1StateFashion.OnMouseMove := FrmDlg.DStateWinMouseMove;
  {------------------------------------------------------------------------------}
  DUserState1BasicPageControl.ActivePageIndex := 0;

  DCloseUS1.ClickCount := csNorm;
  DCloseUS1.OnClickSound := FrmDlg.DLoginNewClickSound;
  DCloseUS1.OnClick := DCloseUS1Click;

  DCloseUS1.BringToFront;
end;

procedure TStateWindows.Initialize;
begin
  MeridiansImageIndexArray[0] := 860;
  MeridiansImageIndexArray[1] := 870;
  MeridiansImageIndexArray[2] := 880;
  MeridiansImageIndexArray[3] := 890;
  MeridiansImageIndexArray[4] := 1180;

  InitSelf;
  InitHero;
  InitUserState1;
end;

function GetMeridianStateInfo(nPage:Integer; Meridians:THumMeridians):string;
const
  MeridianLevelStrings:array[1..10] of string = ('一', '二', '三', '四', '五', '六', '七', '八', '九', '十');
begin
  if Meridians[nPage].Acupoints[4] > 0 then begin
    if Meridians[nPage].Level > 0 then begin
      Result := MeridianLevelStrings[Meridians[nPage].Level] + #13 + '重' + #13 + '经' + #13 + '络';
    end
    else
      Result := '经' + #13 + '络' + #13 + '已' + #13 + '通';
  end
  else
    Result := '经' + #13 + '络' + #13 + '未' + #13 + '通';
end;

procedure TStateWindows.DSMBShortcutClick(Sender:TObject; X, Y:Integer); // 快捷键弹出内挂
begin
  if g_ClientConfig.boCanOpenGameConfigDlg then begin
    if (g_ConfigDlg is TMirConfigDlg) and g_ClientConfig.ClientConfigTabSheetVisibles[4] then begin
      // TMirConfigDlg(g_ConfigDlg).PlugTabSheetConfig6.TabVisible := True;
      TMirConfigDlg(g_ConfigDlg).PlugPageControlConfig.ActivePageIndex := 5;
      g_ConfigDlg.Visible := True;
    end
    else if (FrmJSYDlg <> nil) and (g_ConfigDlg is TJSYConfigDlg) and g_ClientConfig.ClientConfigTabSheetVisibles[7] then begin
      FrmJSYDlg.pgMain.ActivePage := FrmJSYDlg.tsKey;
      g_ConfigDlg.Visible := True;
    end;
  end;
end;
{------------------------------------------------------------------------------}

procedure TStateWindows.DHeroStateWinUpDate(Sender:TObject);
begin
  if g_MyHero = nil then Exit;
  DHeroLabelDStateWinCharName.Caption := g_MyHero.m_sUserName;
  DHeroLabelDStateWinCharName.CaptionColor.Up.Color := g_MyHero.m_nNameColor;
end;

procedure TStateWindows.DHeroSWWeaponClick(Sender:TObject; X, Y:Integer);
var
  where, n, sel:Integer;
  flag:Boolean;
  DButton:TDxImageButton;
  Msg:TDefaultMessage;
begin
  if g_MyHero = nil then Exit;
  DButton := TDxImageButton(Sender);
  if g_boItemMoving then begin
    flag := False;

    if not (g_MovingItem.ItemType in [mtHeroBagItem, mtHeroUseItem]) then Exit;
    if (g_MovingItem.Item.S.Name = '') or (g_WaitingHeroUseItem.Item.S.Name <> '') then Exit;

    if (g_MovingItem.Item.S.OverLap <> 0) and (g_MovingItem.Item.S.OverLap and 1 = 0) then
      Exit;

    // 往祝福罐放入物品 chongchong 2017-07-08
    if (DButton.Tag = U_BUJUK) and
      (Length(g_HeroUseItems[U_BUJUK].s.Name) > 0) and
      (g_HeroUseItems[U_BUJUK].s.StdMode = 96) and
      (g_MovingItem.Item.S.AniCount = g_HeroUseItems[U_BUJUK].S.Shape) and (not (g_MovingItem.Item.S.StdMode in [5, 6, 10, 11, 66, 67 {时装衣服}, 68, 69 {时装武器}])) then begin
      g_WaitingHeroUseItem := g_MovingItem;
      g_MovingItem.Item.S.Name := '';
      g_boItemMoving := False;
      Msg := MakeDefaultMsg(CM_AddItemToJar, g_WaitingHeroUseItem.Item.MakeIndex, 1, 0, 0);
      FrmMain.SendSocket(EncodeMessage(Msg) + Encodestring(g_WaitingHeroUseItem.Item.S.Name));
      Exit;
    end;

    where := GetTakeOnPosition(g_MovingItem.Item.S.StdMode, g_MovingItem.Item.S.Shape);
    if g_MovingItem.Index >= 0 then begin
      case where of
        U_DRESS, U_FASHIONDRESS:begin
            if (where = U_FASHIONDRESS) and (g_ClientConfig.boUseOldSerialWindows) then Exit;

            if where = DButton.Tag then begin
              {
              case g_MyHero.m_btSex of
                0:
                  begin
                    if not (g_MovingItem.Item.S.StdMode in [10, 66]) then
                    begin
                      //DScreen.AddChatBoardString('非男性用品', clRed, clWhite);
                      Exit;
                    end;
                  end;
                1:
                  begin
                    if not (g_MovingItem.Item.S.StdMode in [11, 67]) then
                    begin
                      //DScreen.AddChatBoardString('非女性用品', clRed, clWhite);
                      Exit;
                    end;
                  end;
              end;
              }

              flag := True;
            end;
          end;
        U_WEAPON:begin
            if U_WEAPON = DButton.Tag then begin
              flag := True;
            end;
          end;
        U_NECKLACE:begin
            if U_NECKLACE = DButton.Tag then begin
              flag := True;
            end;
          end;
        U_RIGHTHAND:begin
            if U_RIGHTHAND = DButton.Tag then begin
              flag := True;
            end;
          end;
        U_HELMET:begin
            if U_HELMET = DButton.Tag then begin
              flag := True;
            end;
          end;
        U_RINGR, U_RINGL:begin
            if U_RINGL = DButton.Tag then begin
              where := U_RINGL;
              flag := True;
            end;

            if U_RINGR = DButton.Tag then begin
              where := U_RINGR;
              flag := True;
            end;
          end;
        U_ARMRINGR, U_ARMRINGL:begin
            if U_ARMRINGL = DButton.Tag then begin
              where := U_ARMRINGL;
              flag := True;
            end;
            if U_ARMRINGR = DButton.Tag then begin
              where := U_ARMRINGR;
              flag := True;
            end;
          end;
        U_BUJUK:begin
            if U_BUJUK = DButton.Tag then begin
              where := U_BUJUK;
              flag := True;
            end;
            if (not g_ClientConfig.boDisableDuFuTakeArmRingL) and (U_ARMRINGL = DButton.Tag) then begin
              where := U_ARMRINGL;
              flag := True;
            end;
          end;
        {U_BUJUK:
          begin
            if U_BUJUK = DButton.Tag then
            begin
              where := U_BUJUK;
              flag := True;
            end;
            if U_ARMRINGL = DButton.Tag then
            begin
              where := U_ARMRINGL;
              flag := True;
            end;
          end;    }
        U_BELT:begin
            if U_BELT = DButton.Tag then begin
              where := U_BELT;
              flag := True;
            end;
          end;
        U_BOOTS:begin
            if U_BOOTS = DButton.Tag then begin
              where := U_BOOTS;
              flag := True;
            end;
          end;
        U_CHARM:begin
            if U_CHARM = DButton.Tag then begin
              where := U_CHARM;
              flag := True;
            end;
          end;
        U_HAT:begin
            if U_HELMET = DButton.Tag then begin
              where := U_HAT;
              flag := True;
            end;
          end;
        U_DRUM:begin // 鼓
            if U_DRUM = DButton.Tag then begin
              where := U_DRUM;
              flag := True;
            end;
          end;
        U_HORSE:begin // 马
            if U_HORSE = DButton.Tag then begin
              where := U_HORSE;
              flag := True;
            end;
          end;
        U_SHIELD:begin
            if U_SHIELD = DButton.Tag then begin
              where := U_SHIELD;
              flag := True;
            end;
          end;
        U_JADE:begin
            if U_JADE = DButton.Tag then begin
              where := U_JADE;
              flag := True;
            end;
          end;
        U_FASHIONWEAPON:begin
            if U_FASHIONWEAPON = DButton.Tag then begin
              where := U_FASHIONWEAPON;
              flag := True;
            end;
          end;
        U_FASHIONHELMET:begin
            if U_FASHIONHELMET = DButton.Tag then begin
              where := U_FASHIONHELMET;
              flag := True;
            end;
          end;
        U_FASHIONARMRINGL, U_FASHIONARMRINGR:begin
            if U_FASHIONARMRINGL = DButton.Tag then begin
              where := U_FASHIONARMRINGL;
              flag := True;
            end;

            if U_FASHIONARMRINGR = DButton.Tag then begin
              where := U_FASHIONARMRINGR;
              flag := True;
            end;
          end;
        U_FASHIONRINGL, U_FASHIONRINGR:begin
            if U_FASHIONRINGL = DButton.Tag then begin
              where := U_FASHIONRINGL;
              flag := True;
            end;

            if U_FASHIONRINGR = DButton.Tag then begin
              where := U_FASHIONRINGR;
              flag := True;
            end;
          end;
        U_FASHIONRIGHTHAND: {// 时装照明物品} begin
            if U_FASHIONRIGHTHAND = DButton.Tag then begin
              where := U_FASHIONRIGHTHAND;
              flag := True;
            end;
          end;
        U_FASHIONBELT: {// 时装腰带} begin
            if U_FASHIONBELT = DButton.Tag then begin
              where := U_FASHIONBELT;
              flag := True;
            end;
          end;
        U_FASHIONBOOTS: {// 时装鞋} begin
            if U_FASHIONBOOTS = DButton.Tag then begin
              where := U_FASHIONBOOTS;
              flag := True;
            end;
          end;
        U_FASHIONCHARM: {// 时装宝石} begin
            if U_FASHIONCHARM = DButton.Tag then begin
              where := U_FASHIONCHARM;
              flag := True;
            end;
          end;
      end;
    end
    else begin
      n := -(g_MovingItem.Index + 1);

      if n in [Low(TUseItems)..High(TUseItems)] then begin
        ItemClickSound(g_MovingItem.Item.S);
        g_HeroUseItems[n] := g_MovingItem.Item;
        g_MovingItem.Item.S.Name := '';
        g_boItemMoving := False;
      end;
    end;
    if flag then begin
      ItemClickSound(g_MovingItem.Item.S);
      g_WaitingHeroUseItem := g_MovingItem;
      g_WaitingHeroUseItem.Index := where;

      frmMain.SendHeroTakeOnItem(where, g_MovingItem.Item.MakeIndex, g_MovingItem.Item.S.Name);
      g_MovingItem.Item.S.Name := '';

      g_boItemMoving := False;
    end;
  end
  else begin
    if (g_MovingItem.Item.S.Name <> '') or (g_WaitingHeroUseItem.Item.S.Name <> '') then Exit;
    sel := DButton.Tag;
    if sel >= 0 then begin
      if (sel = U_HELMET) and (g_HeroUseItems[U_HAT].S.Name <> '') then begin
        ItemClickSound(g_HeroUseItems[U_HAT].S);
        g_MovingItem.Index := -(U_HAT + 1);
        g_MovingItem.Item := g_HeroUseItems[U_HAT];
        g_MovingItem.ItemType := mtHeroUseItem;
        g_HeroUseItems[U_HAT].S.Name := '';
        g_boItemMoving := True;
      end
      else begin
        if g_HeroUseItems[sel].S.Name <> '' then begin
          ItemClickSound(g_HeroUseItems[sel].S);
          g_MovingItem.Index := -(sel + 1);
          g_MovingItem.Item := g_HeroUseItems[sel];
          g_MovingItem.ItemType := mtHeroUseItem;
          g_HeroUseItems[sel].S.Name := '';
          g_boItemMoving := True;
        end;
      end;
    end;
  end;
end;

procedure TStateWindows.DHeroSWLightDirectPaint(Sender:TObject);
var
  nWhere:Integer;
begin
  nWhere := TDxImageButton(Sender).Tag;
  if nWhere >= 0 then begin
    if (nWhere in [U_DRESS, U_WEAPON]) then
      // FrmDlg.DrawGridItem(TDxImageButton(Sender).VirtualRect, @g_HeroUseItems[nWhere].s, @g_HeroUseItemsEffect[nWhere])
    else
      FrmDlg.DrawBodyItem(TDxImageButton(Sender).VirtualRect, @g_HeroUseItems[nWhere].s, g_HeroUseItems[nWhere].btHeroM2Light, @g_HeroUseItemsEffect[nWhere], nWhere <> U_RIGHTHAND);
  end;
end;

{ TODO -ochongchong -c修改 : 英雄漂浮装备信息显示增加选择随鼠标位置显示 连击界面)  【2013-07-14】}

procedure TStateWindows.DHeroSWWeaponMouseMove(Sender:TObject; Shift:TShiftState;
  X, Y:Integer);
var
  I, sel, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  DrawLeft:Boolean;
  HintWindow:THintWindow;
  HintList:TList;
  TzHintWindows:TList;
  ArrHintWindows:TArrHintWindows;
begin
  if g_MyHero = nil then Exit;
  sel := TDxImageButton(Sender).Tag;
  if (sel = U_HELMET) and (g_HeroUseItems[U_HELMET].s.Name = '') and (g_HeroUseItems[U_HAT].s.Name <> '') then // 优先显示斗笠 GetMouseItemInfoWindow
    sel := U_HAT;

  if (sel >= 0) and (g_HeroUseItems[sel].s.Name <> '') then begin
    g_MouseStateItem := g_HeroUseItems[sel];

    // 免得不停的重新创建提示窗口 chongchong 2015-01-22
    if (g_MouseStateItem.MakeIndex <> g_LastHintMakeIndex) then begin
      g_boShowBagInfo := False;

      with DHeroStateWin do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;

          // 衣服的属性上移 chongchong 2014-11-10
          if Sender = DHeroSWDress then begin
            nY := nY - (CtrlRect.Bottom - CtrlRect.Top) div 2;
          end;
        end else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 50;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left + 40
          else
            nX := vtRect.Right - 20;
        end;

        if g_MouseStateItem.s.Name = '' then begin
          HintWindows.Show(nX, nY, GetUseItemName(sel), clWhite, False, DrawLeft);
        end else begin
          HintWindows.Clear;
          HintList := TList.Create;
          // 两列显示 piaoyun 2013-10-08
          if not g_ClientConfig.boSingleHint then begin
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MyHero, @g_MouseStateItem, nX, nY, False, False, DrawLeft);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then
                HintList.Add(HintWindow)
              else
                HintWindow.Free;
            end;

            if not g_ClientConfig.boTZSupportRenameItem then
              TzHintWindows := FrmDlg.GetTzItemHintWindow(1, g_MyHero.m_btSex, g_MouseStateItem.s.DBName)
            else
              TzHintWindows := FrmDlg.GetTzItemHintWindow(1, g_MyHero.m_btSex, g_MouseStateItem.s.Name);

            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, DrawLeft);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end;
            if (sel = U_HELMET) and (g_HeroUseItems[U_HAT].s.Name <> '') then begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MyHero, @g_HeroUseItems[U_HAT], nX, nY, False, False, DrawLeft);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then begin
                  HintList.Add(HintWindow);
                  HintWindow.Show(nX, nY, False, DrawLeft);
                end else begin
                  HintWindow.Free;
                end;
              end;

              if not g_ClientConfig.boTZSupportRenameItem then
                TzHintWindows := FrmDlg.GetTzItemHintWindow(1, g_MyHero.m_btSex, g_HeroUseItems[U_HAT].s.DBName)
              else
                TzHintWindows := FrmDlg.GetTzItemHintWindow(1, g_MyHero.m_btSex, g_HeroUseItems[U_HAT].s.Name);

              if TzHintWindows <> nil then begin
                for I := 0 to TzHintWindows.Count - 1 do begin
                  THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, DrawLeft);
                  HintList.Add(TzHintWindows.Items[I]);
                end;
                TzHintWindows.Free;
              end;
            end;
          end else begin
            // 单列显示 piaoyun 2013-10-08
            TzHintWindows := FrmDlg.GetTzItemHintWindowEx(1, g_MyHero.m_btSex, g_MyHero, @g_MouseStateItem, False);
            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end else begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MyHero, @g_MouseStateItem, nX, nY, False, False, DrawLeft);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then
                  HintList.Add(HintWindow)
                else
                  HintWindow.Free;
              end;
            end;

            if (sel = U_HELMET) and (g_HeroUseItems[U_HAT].s.Name <> '') then begin
              TzHintWindows := FrmDlg.GetTzItemHintWindowEx(1, g_MyHero.m_btSex, g_MyHero, @g_HeroUseItems[U_HAT], False);
              if TzHintWindows <> nil then begin
                for I := 0 to TzHintWindows.Count - 1 do begin
                  THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, DrawLeft);
                  HintList.Add(TzHintWindows.Items[I]);
                end;
                TzHintWindows.Free;
              end else begin
                for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                  ArrHintWindows[I] := THintWindow.Create;
                end;

                FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MyHero, @g_HeroUseItems[U_HAT], nX, nY, False, False, DrawLeft);

                for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                  HintWindow := THintWindow(ArrHintWindows[I]);
                  if HintWindow.Count > 0 then begin
                    HintList.Add(HintWindow);
                    HintWindow.Show(nX, nY, False, DrawLeft);
                  end else begin
                    HintWindow.Free;
                  end;
                end;
              end;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
          HintList.Free;
        end;
      end;

      g_LastHintMakeIndex := g_MouseStateItem.MakeIndex;
    end;
  end else begin
    g_boShowBagInfo := False;
    g_MouseStateItem.s.Name := '';
    DScreen.ClearHint;
    HintWindows.Clear;
  end;
end;

procedure TStateWindows.DHeroStateWinDirectPaint(Sender:TObject);
var
  bbx, bby, nIdx, ax, ay:Integer;
  d:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  IsShowHelmet, IsShowHair:Boolean;
begin
  if g_MyHero = nil then Exit;

  // 把这个B底层播放放到衣服下面 太阳.... chongchong 2016-08-04
  FrmDlg.DrawBodyItemBelowEffect(DHeroSWLight.VirtualRect, @g_HeroUseItems[DHeroSWLight.Tag].s, @g_HeroUseItemsEffect[DHeroSWLight.Tag]);

  with Sender as TDxControl do begin
    vbRect := VisibleRect;
    if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
    vtRect := VirtualRect;

    if g_ClientVersion = cvMirNewUI205 then begin
      bbx := vtRect.Left + 55 + g_ConfigClient.nHeroUserHairOffsetX;
      bby := vtRect.Top + 94 + g_ConfigClient.nHeroUserHairOffsetY;
    end
    else begin
      bbx := vtRect.Left + 52 + g_ConfigClient.nHeroUserHairOffsetX;
      bby := vtRect.Top + 78 + g_ConfigClient.nHeroUserHairOffsetY;
    end;

    IsShowHair := True;
    if g_HeroUseItems[U_DRESS].S.Name <> '' then begin
      // 个位用来表示是否显示发型，Expand5 div 10用来表示使提示框文字使用哪个图片作为背景 2019-12-15 10:05:20
      IsShowHair := g_HeroUseItems[U_DRESS].S.Expand5 mod 10 = 0;
    end;

    if IsShowHair then begin
      if g_MyHero.m_btHair < 4 then begin
        nIdx := 440 + g_MyHero.m_btHair;
        if nIdx > 0 then begin
          d := g_WMainImages.GetCachedImage(nIdx, ax, ay);
          if d <> nil then
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
        end;
      end
      else if (g_MyHero.m_btHair >= 50) and (g_MyHero.m_btHair <= 69) then begin
        nIdx := 350 + g_MyHero.m_btHair;
        d := g_WNewopUIImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
    end;

    if g_HeroUseItems[U_DRESS].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_HeroUseItems[U_DRESS].s, @g_HeroUseItemsEffect[U_DRESS], True);

      nIdx := g_HeroUseItems[U_DRESS].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_HeroUseItems[U_DRESS].s, @g_HeroUseItemsEffect[U_DRESS], False);
    end;

    if g_HeroUseItems[U_WEAPON].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_HeroUseItems[U_WEAPON].s, @g_HeroUseItemsEffect[U_WEAPON], True);

      nIdx := g_HeroUseItems[U_WEAPON].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_HeroUseItems[U_WEAPON].s, @g_HeroUseItemsEffect[U_WEAPON], False);
    end;
    if g_HeroUseItems[U_HELMET].S.Name <> '' then begin
      nIdx := g_HeroUseItems[U_HELMET].S.looks;
      { TODO -ochongchong -c修改 : 穿戴16的头盔 15的头盔在内观上不再显示 【2013-08-26】 }
      (*
      if nIdx >= 0 then
      begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      { TODO -c修正 -opiaoyun : 修复英雄装备栏特效 【2013-4-26】}
      FrmDlg.DrawBodyItemEffect(DHeroSWHelmet.VirtualRect, @g_HeroUseItems[U_HELMET].s, @g_HeroUseItemsEffect[U_HELMET]);
      *)

      IsShowHelmet := True;
      if (g_HeroUseItems[U_HELMET].s.StdMode = 15) then begin
        if (g_HeroUseItems[U_HAT].S.Name <> '') and (g_HeroUseItems[U_HAT].S.StdMode = 16) and (g_HeroUseItems[U_HAT].S.Shape <> 1000) {1000 为面巾，面巾和头盔可同时显示} then
          IsShowHelmet := False;
      end;
      if (nIdx >= 0) and IsShowHelmet then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then begin
          if g_ClientConfig.boHelmetShowInBox then begin
            ax := DHeroSWHelmet.VirtualRect.Left + (DHeroSWHelmet.Width - d.Width) div 2;
            ay := DHeroSWHelmet.VirtualRect.Top + (DHeroSWHelmet.Height - d.Height) div 2;
            GameCanvas.Draw(ax, ay, d.ClientRect, d);
          end
          else begin
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
          end;
        end;
      end;
      if IsShowHelmet then
        FrmDlg.DrawBodyItemEffect(DHeroSWHelmet.VirtualRect, @g_HeroUseItems[U_HELMET].s, @g_HeroUseItemsEffect[U_HELMET], False);
    end;

    if g_HeroUseItems[U_HAT].S.Name <> '' then begin
      nIdx := g_HeroUseItems[U_HAT].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      // 同时修改。。。
      FrmDlg.DrawBodyItemEffect(DHeroSWHelmet.VirtualRect, @g_HeroUseItems[U_HAT].s, @g_HeroUseItemsEffect[U_HAT], False);
    end;

    if g_HeroUseItems[U_SHIELD].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(DHeroSWShield.VirtualRect, @g_HeroUseItems[U_SHIELD].s, @g_HeroUseItemsEffect[U_SHIELD], True);
      nIdx := g_HeroUseItems[U_SHIELD].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(DHeroSWShield.VirtualRect, @g_HeroUseItems[U_SHIELD].s, @g_HeroUseItemsEffect[U_SHIELD], False);
    end;
  end;
end;

procedure TStateWindows.DHeroStateWinFashionDirectPaint(Sender:TObject);
var
  bbx, bby, nIdx, ax, ay:Integer;
  d:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  IsShowHair:Boolean;
begin
  if g_MyHero = nil then Exit;
  with Sender as TDxControl do begin
    vbRect := VisibleRect;
    if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
    vtRect := VirtualRect;

    if g_ClientVersion = cvMirNewUI205 then begin
      bbx := vtRect.Left + 55 + g_ConfigClient.nHeroUserHairOffsetX;
      bby := vtRect.Top + 94 + g_ConfigClient.nHeroUserHairOffsetY;
    end
    else begin
      bbx := vtRect.Left + 52 + g_ConfigClient.nHeroUserHairOffsetX;
      ;
      bby := vtRect.Top + 78 + g_ConfigClient.nHeroUserHairOffsetY;
    end;

    IsShowHair := True;
    if g_HeroUseItems[U_FASHIONDRESS].S.Name <> '' then begin
      // 个位用来表示是否显示发型，Expand5 div 10用来表示使提示框文字使用哪个图片作为背景 2019-12-15 10:05:20
      IsShowHair := g_HeroUseItems[U_FASHIONDRESS].S.Expand5 mod 10 = 0;
    end;

    if IsShowHair then begin
      if g_MyHero.m_btOldHair < 4 then begin
        nIdx := 440 + g_MyHero.m_btOldHair;
        if nIdx > 0 then begin
          d := g_WMainImages.GetCachedImage(nIdx, ax, ay);
          if d <> nil then
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
        end;
      end
      else if (g_MyHero.m_btOldHair >= 50) and (g_MyHero.m_btOldHair <= 69) then begin
        nIdx := 350 + g_MyHero.m_btOldHair;
        d := g_WNewopUIImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
    end;

    if g_HeroUseItems[U_FASHIONDRESS].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_HeroUseItems[U_FASHIONDRESS].s, @g_HeroUseItemsEffect[U_FASHIONDRESS], True);
      nIdx := g_HeroUseItems[U_FASHIONDRESS].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_HeroUseItems[U_FASHIONDRESS].s, @g_HeroUseItemsEffect[U_FASHIONDRESS], False);
    end;

    if g_HeroUseItems[U_FASHIONWEAPON].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_HeroUseItems[U_FASHIONWEAPON].s, @g_HeroUseItemsEffect[U_FASHIONWEAPON], True);
      nIdx := g_HeroUseItems[U_FASHIONWEAPON].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_HeroUseItems[U_FASHIONWEAPON].s, @g_HeroUseItemsEffect[U_FASHIONWEAPON], False);
    end;

    if g_ClientConfig.boFashionJewelryOpen and (g_HeroUseItems[U_FASHIONHELMET].S.Name <> '') then begin
      FrmDlg.DrawBodyItemEffect(DHeroSWFashionHelmet.VirtualRect, @g_HeroUseItems[U_FASHIONHELMET].s, @g_HeroUseItemsEffect[U_FASHIONHELMET], True);

      nIdx := g_HeroUseItems[U_FASHIONHELMET].S.looks;
      if (nIdx >= 0) then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if (d <> nil) then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;

      FrmDlg.DrawBodyItemEffect(DHeroSWFashionHelmet.VirtualRect, @g_HeroUseItems[U_FASHIONHELMET].s, @g_HeroUseItemsEffect[U_FASHIONHELMET], False);
    end;
  end;
end;

procedure TStateWindows.SetHeroMeridiansLevel(Meridian, Level:Integer); // 设置经脉等级 多少重
begin
  if not Initialized then Exit;
  if (Meridian in [0..4]) and (Level in [0..5]) then begin
    HeroMeridiansFormArray[Meridian].Draw1.ImageIndex := MeridiansImageIndexArray[Meridian] + Level;
  end;
end;

procedure TStateWindows.DHeroBotAcupointsClick(Sender:TObject; X, Y:Integer); // 点击穴位
var
  nPage:Integer;
begin
  if g_MyHero = nil then Exit;
  nPage := DHeroMeridiansPageControl.ActivePageIndex - 1;
  if nPage < 0 then nPage := 4;
  frmMain.SendClientMessage(CM_SENDACUPOINTCLICK, 0, nPage, TDxControl(Sender).Tag, 1);
end;

procedure TStateWindows.DHeroBotAcupointsMouseMove(Sender:TObject; Shift:TShiftState; X, // 穴位鼠标移动事件
  Y:Integer);
var
  nPage:Integer;
  sMsg1, sMsg2:string;
  btn:TDxImageButton;
  nCurLevel:Integer; // 当前等级
  lst:TStringList;

  procedure FunA(lst:TStringList; nIndex:Integer);
  var
    i:Integer;
  const
    DxImageButtonName = 'DHeroBotAcupoints%d_%d';
    NeedLev = '需要内功等级%d级';
  begin
    btn := Sender as TDxImageButton;
    for i := 0 to lst.Count - 1 do begin
      if btn.Name = Format(DxImageButtonName, [nIndex, i]) then begin
        if g_MyHero.m_HumMeridians[nPage].Acupoints[i] > 0 then begin
          sMsg1 := lst[i] + '：已打通';
          Break;
        end { else if g_MySelf.m_HumMeridians[nPage].Level = 0 then}
        else begin
          if nCurLevel >= g_AcupointLevels[nPage, i] then begin
            sMsg1 := lst[i] + '：待打通\' + Format(NeedLev, [g_AcupointLevels[nPage, i]]);
            Break;
          end
          else begin
            sMsg2 := lst[i] + '：待打通\' + Format(NeedLev, [g_AcupointLevels[nPage, i]]);
            Break;
          end;
        end
      end
    end;
  end;

begin
  if g_MyHero = nil then Exit;

  nPage := DHeroMeridiansPageControl.ActivePageIndex - 1; // 经络页面

  sMsg1 := '';
  sMsg2 := '';
  nCurLevel := g_MyHero.m_AbilNG.Level;

  lst := TStringList.Create;
  try
    lst.Clear;
    case nPage of
      0:begin
          lst.Add('幽门穴');
          lst.Add('通骨穴');
          lst.Add('商曲穴');
          lst.Add('四满穴');
          lst.Add('横骨穴');
        end;
      1:begin
          lst.Add('睛明穴');
          lst.Add('盘缺穴');
          lst.Add('交信穴');
          lst.Add('照海穴');
          lst.Add('然骨穴');
        end;
      2:begin
          lst.Add('廉泉穴');
          lst.Add('期门穴');
          lst.Add('府舍穴');
          lst.Add('冲门穴');
          lst.Add('筑宾穴');
        end;
      3:begin
          lst.Add('承浆穴');
          lst.Add('天突穴');
          lst.Add('鸠尾穴');
          lst.Add('气海穴');
          lst.Add('骨曲穴');
        end;
    end;
    FunA(lst, nPage + 1);
  finally
    lst.Free;
  end;

  if sMsg1 <> '' then begin
    DScreen.ShowHint(X, Y, sMsg1, clYellow, FALSE, False, False);
  end;

  if sMsg2 <> '' then begin
    DScreen.ShowHint(X, Y, sMsg2, clRed, FALSE, False, False);
  end;
end;

procedure TStateWindows.DHeroMeridiansLineDirectPaint(Sender:TObject); // 绘制穴位
var
  I:Integer;
  nC:Integer;
  d:TTexture;
  vtRect:TRect;
begin
  if g_MyHero = nil then Exit;
  with Sender as TDxImageForm do begin
    vtRect := VirtualRect;
    nC := 1;
    for I := 0 to 4 do begin
      if g_MyHero.m_HumMeridians[Tag].Acupoints[I] > 0 then begin
        vtRect := HeroAcupointArray[Tag, I].VirtualRect;
        d := g_WMainImages.Images[851];
        if d <> nil then
          GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
        Inc(nC);
      end;
    end;

    Dec(nC);
    if nC in [0..4] then begin
      Inc(g_nHeroAcupointTicks);
      if (g_dwHeroDrawAcupointTick = 0) or ((Cardinal(g_nHeroAcupointTicks) div g_dwHeroDrawAcupointTick) and $01 = 0) then begin
        vtRect := HeroAcupointArray[Tag, nC].VirtualRect;
        d := g_WMainImages.Images[853];
        if d <> nil then
          GameCanvas.DrawBlend(vtRect.Left, vtRect.Top, d);
      end;
    end;
  end;
end;

procedure TStateWindows.DHeroTrainingMeridianClick(Sender:TObject; X, Y:Integer); // 修炼经脉按钮
var
  nPage:Integer;
begin
  if g_MyHero = nil then Exit;
  nPage := DHeroMeridiansPageControl.ActivePageIndex - 1;
  if nPage < 0 then nPage := 4;
  frmMain.SendClientMessage(CM_SENDTRAININGMERIDIANCLICK, 0, nPage, 0, 1);
end;

procedure TStateWindows.DHeroBotMeridiansClick(Sender:TObject; X, Y:Integer); // 选择经络
var
  nPage:Integer;
  sCaption:string;
begin
  if g_MyHero = nil then Exit;
  DHeroMeridiansPageControl.ActivePageIndex := TDxControl(Sender).Tag;
  DHeroStateNGMeridians.UseSetting2 := DHeroMeridiansPageControl.ActivePageIndex = 1;
  case DHeroMeridiansPageControl.ActivePageIndex of
    0:DHeroTrainingMeridian.Caption := '修炼穴位';
    1:DHeroTrainingMeridian.Caption := '修炼冲脉';
    2:DHeroTrainingMeridian.Caption := '修炼阴跷';
    3:DHeroTrainingMeridian.Caption := '修炼阴维';
    4:DHeroTrainingMeridian.Caption := '修炼任脉';
  end;
  nPage := DHeroMeridiansPageControl.ActivePageIndex - 1;
  if nPage < 0 then nPage := 4;
  {
  if g_MyHero.m_HumMeridians[nPage].Acupoints[4] > 0 then
    sCaption := '经' + #13 + '络' + #13 + '已' + #13 + '通'
  else
    sCaption := '经' + #13 + '络' + #13 + '未' + #13 + '通';    }
  sCaption := GetMeridianStateInfo(nPage, g_MyHero.m_HumMeridians);
  case DHeroMeridiansPageControl.ActivePageIndex of
    0:;
    1:DHeroLabelMeridiansStatus1.Caption := sCaption;
    2:DHeroLabelMeridiansStatus2.Caption := sCaption;
    3:DHeroLabelMeridiansStatus3.Caption := sCaption;
    4:DHeroLabelMeridiansStatus4.Caption := sCaption;
  end;
end;

{------------------------------------------------------------------------------}

procedure TStateWindows.DStateWinUpDate(Sender:TObject);
begin
  if g_MySelf = nil then Exit;
  LabelDStateWinCharName.Caption := g_MySelf.m_sUserName;
  LabelDStateWinCharName.CaptionColor.Up.Color := g_MySelf.m_nNameColor;
  LabelDStateWinRankName.Caption := g_sGuildName + ' ' + g_sGuildRankName;
end;

procedure TStateWindows.OnDStateWinBringToFront(Sender:TObject);
begin
  if Sender = DStateWin then begin
    if DStateWinEx.Visible then begin
      DStateWinEx.BringToFrontEx;
    end;
  end
  else begin
    if DStateWin.Visible then begin
      DStateWin.BringToFrontEx;
    end;
  end;
end;

{ TODO -ochongchong -c修改 : 人物漂浮装备信息显示增加选择随鼠标位置显示 (不使用合击界面) 【2013-07-13】}

procedure TStateWindows.DSWWeaponMouseMove
  (Sender:TObject; Shift:TShiftState;
  X, Y:Integer);
var
  I, sel, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  DrawLeft:Boolean;

  HintWindow:THintWindow;
  HintList:TList;
  TzHintWindows:TList;
  ArrHintWindows:TArrHintWindows;
begin
  sel := TDxImageButton(Sender).Tag;
  if (sel = U_HELMET) and (g_UseItems[U_HELMET].s.Name = '') and (g_UseItems[U_HAT].s.Name <> '') then // 优先显示斗笠 GetMouseItemInfoWindow
    sel := U_HAT;

  if (sel >= 0) and (g_UseItems[sel].s.Name <> '') then begin
    g_MouseStateItem := g_UseItems[sel];

    // 免得不停的重新创建提示窗口 chongchong 2015-01-22
    if (g_MouseStateItem.MakeIndex <> g_LastHintMakeIndex) then begin
      g_boShowBagInfo := False;

      with DStateWin do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;

          // 衣服的属性上移 chongchong 2014-11-10
          if Sender = DSWDress then begin
            nY := nY - (CtrlRect.Bottom - CtrlRect.Top) div 2;
          end;
        end else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left + 40
          else
            nX := vtRect.Right - 20;
        end;

        if Length(g_MouseStateItem.s.Name) = 0 then begin
          HintWindows.Show(nX, nY, GetUseItemName(sel), clWhite, False, DrawLeft);
        end else begin
          HintWindows.Clear;
          HintList := TList.Create;

          // 两列显示 piaoyun 2013-09-15
          if not g_ClientConfig.boSingleHint then begin
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then
                HintList.Add(HintWindow)
              else
                HintWindow.Free;
            end;

            if not g_ClientConfig.boTZSupportRenameItem then
              TzHintWindows := FrmDlg.GetTzItemHintWindow(0, g_MySelf.m_btSex, g_MouseStateItem.s.DBName)
            else
              TzHintWindows := FrmDlg.GetTzItemHintWindow(0, g_MySelf.m_btSex, g_MouseStateItem.s.Name);

            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end;

            if (sel = U_HELMET) and (g_UseItems[U_HAT].s.Name <> '') then begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_UseItems[U_HAT], nX, nY, False, False, False);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then begin
                  HintList.Add(HintWindow);
                  HintWindow.Show(nX, nY, False, DrawLeft);
                end else begin
                  HintWindow.Free;
                end;
              end;

              if not g_ClientConfig.boTZSupportRenameItem then
                TzHintWindows := FrmDlg.GetTzItemHintWindow(0, g_MySelf.m_btSex, g_UseItems[U_HAT].s.DBName)
              else
                TzHintWindows := FrmDlg.GetTzItemHintWindow(0, g_MySelf.m_btSex, g_UseItems[U_HAT].s.Name);

              if TzHintWindows <> nil then begin
                for I := 0 to TzHintWindows.Count - 1 do begin
                  THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, DrawLeft);
                  HintList.Add(TzHintWindows.Items[I]);
                end;
                TzHintWindows.Free;
              end;
            end;
          end else begin
            // 单列显示 piaoyun 2013-09-15
            TzHintWindows := FrmDlg.GetTzItemHintWindowEx(0, g_MySelf.m_btSex, g_MySelf, @g_MouseStateItem, False);
            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end else begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then
                  HintList.Add(HintWindow)
                else
                  HintWindow.Free;
              end;
            end;

            if (sel = U_HELMET) and (g_UseItems[U_HAT].s.Name <> '') then begin
              TzHintWindows := FrmDlg.GetTzItemHintWindowEx(0, g_MySelf.m_btSex, g_MySelf, @g_UseItems[U_HAT], False);
              if TzHintWindows <> nil then begin
                for I := 0 to TzHintWindows.Count - 1 do begin
                  THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, DrawLeft);
                  HintList.Add(TzHintWindows.Items[I]);
                end;
                TzHintWindows.Free;
              end else begin
                for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                  ArrHintWindows[I] := THintWindow.Create;
                end;

                FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_UseItems[U_HAT], nX, nY, False, False, False);

                for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                  HintWindow := THintWindow(ArrHintWindows[I]);
                  if HintWindow.Count > 0 then begin
                    HintList.Add(HintWindow);
                    HintWindow.Show(nX, nY, False, DrawLeft);
                  end else begin
                    HintWindow.Free;
                  end;
                end;
              end;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - nWidth) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end else begin
            if vtRect.Left >= (SCREENWIDTH - nWidth) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
          HintList.Free;

        end;
      end;

      g_LastHintMakeIndex := g_MouseStateItem.MakeIndex;
    end;
  end else begin
    g_boShowBagInfo := False;
    g_MouseStateItem.s.Name := '';
    DScreen.ClearHint;
    HintWindows.Clear;
  end;
end;

procedure TStateWindows.DStateWinDirectPaint(Sender:TObject);
var
  bbx, bby, nIdx, ax, ay:Integer;
  d:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  IsShowHelmet:Boolean;
  IsShowHair:Boolean;
begin
  if g_MySelf = nil then Exit;

  // 把这个B底层播放放到衣服下面 太阳.... chongchong 2016-08-04
  if (g_UseItems[U_RIGHTHAND].s.Name <> '') then begin
    if g_UseItems[U_RIGHTHAND].s.StdMode = 30 then begin
      FrmDlg.DrawBodyItemBelowEffect(DSWLight.VirtualRect, @g_UseItems[U_RIGHTHAND].s, @g_UseItemsEffect[U_RIGHTHAND])
    end else begin
      if g_UseItems[U_RIGHTHAND].s.StdMode = 29 then begin
        FrmDlg.DrawBodyItemBelowEffect(DSWLight.VirtualRect, @g_UseItems[U_RIGHTHAND].s, @g_UseItemsEffect[U_RIGHTHAND]);
      end;

      {
      with Sender as TDxControl do
      begin
        vbRect := VisibleRect;
        if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
        vtRect := VirtualRect;

        if g_ClientVersion = cvMirNewUI205 then
        begin
          bbx := vtRect.Left + 55 + g_ConfigClient.nUserHairOffsetX;
          bby := vtRect.Top + 94 + g_ConfigClient.nUserHairOffsetY;
        end
        else
        begin
          bbx := vtRect.Left + 52 + g_ConfigClient.nUserHairOffsetX;
          bby := vtRect.Top + 78 + g_ConfigClient.nUserHairOffsetY;
        end;
      end;

      FrmDlg.DrawBodyItemEffect(bbx + 158, bby + 210, @g_UseItems[U_RIGHTHAND].s, @g_UseItemsEffect[U_RIGHTHAND], True);
      }
    end;
  end;

  with Sender as TDxControl do begin
    vbRect := VisibleRect;
    if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
    vtRect := VirtualRect;

    if g_ClientVersion = cvMirNewUI205 then begin
      bbx := vtRect.Left + 55 + g_ConfigClient.nUserHairOffsetX;
      bby := vtRect.Top + 94 + g_ConfigClient.nUserHairOffsetY;
    end else begin
      bbx := vtRect.Left + 52 + g_ConfigClient.nUserHairOffsetX;
      bby := vtRect.Top + 78 + g_ConfigClient.nUserHairOffsetY;
    end;

    IsShowHair := True;
    if g_UseItems[U_DRESS].S.Name <> '' then begin
      // 个位用来表示是否显示发型，Expand5 div 10用来表示使提示框文字使用哪个图片作为背景 2019-12-15 10:05:20
      IsShowHair := g_UseItems[U_DRESS].S.Expand5 mod 10 = 0;
    end;

    if IsShowHair then begin
      if g_MySelf.m_btHair < 4 then begin
        nIdx := 440 + g_MySelf.m_btHair;
        if nIdx > 0 then begin
          d := g_WMainImages.GetCachedImage(nIdx, ax, ay);
          if d <> nil then
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
        end;
      end else if (g_MySelf.m_btHair >= 50) and (g_MySelf.m_btHair <= 69) then begin
        nIdx := 350 + g_MySelf.m_btHair;
        d := g_WNewopUIImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
    end;

    if g_UseItems[U_DRESS].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UseItems[U_DRESS].s, @g_UseItemsEffect[U_DRESS], True);
      nIdx := g_UseItems[U_DRESS].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then begin
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
        end;
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UseItems[U_DRESS].s, @g_UseItemsEffect[U_DRESS], False);
    end;

    if g_UseItems[U_WEAPON].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UseItems[U_WEAPON].s, @g_UseItemsEffect[U_WEAPON], True);
      nIdx := g_UseItems[U_WEAPON].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UseItems[U_WEAPON].s, @g_UseItemsEffect[U_WEAPON], False);
    end;

    if g_UseItems[U_HELMET].S.Name <> '' then begin
      nIdx := g_UseItems[U_HELMET].S.looks;

      IsShowHelmet := True;

      if (g_UseItems[U_HELMET].s.StdMode = 15) then begin
        if (g_UseItems[U_HAT].S.Name <> '') and (g_UseItems[U_HAT].S.StdMode = 16) and (g_UseItems[U_HAT].S.Shape <> 1000) {1000 为面巾，面巾和头盔可同时显示} then
          IsShowHelmet := False;
      end;

      if (nIdx >= 0) and IsShowHelmet then begin
        FrmDlg.DrawBodyItemEffect(DSWHelmet.VirtualRect, @g_UseItems[U_HELMET].s, @g_UseItemsEffect[U_HELMET], True);
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if (d <> nil) then begin
          if g_ClientConfig.boHelmetShowInBox then begin
            ax := DSWHelmet.VirtualRect.Left + (DSWHelmet.Width - d.Width) div 2;
            ay := DSWHelmet.VirtualRect.Top + (DSWHelmet.Height - d.Height) div 2;
            GameCanvas.Draw(ax, ay, d.ClientRect, d);
          end else begin
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
          end;
        end;
      end;
      if IsShowHelmet then
        FrmDlg.DrawBodyItemEffect(DSWHelmet.VirtualRect, @g_UseItems[U_HELMET].s, @g_UseItemsEffect[U_HELMET], False);
    end;

    if g_UseItems[U_HAT].S.Name <> '' then begin
      nIdx := g_UseItems[U_HAT].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(DSWHelmet.VirtualRect, @g_UseItems[U_HAT].s, @g_UseItemsEffect[U_HAT], False);
    end;

    if g_UseItems[U_SHIELD].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(DSWShield.VirtualRect, @g_UseItems[U_SHIELD].s, @g_UseItemsEffect[U_SHIELD], True);
      nIdx := g_UseItems[U_SHIELD].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(DSWShield.VirtualRect, @g_UseItems[U_SHIELD].s, @g_UseItemsEffect[U_SHIELD], False);
    end;
  end;
end;

procedure TStateWindows.DStateWinFashionDirectPaint(Sender:TObject);
var
  bbx, bby, nIdx, ax, ay:Integer;
  d:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  IsShowHair:Boolean;
begin
  if g_MySelf = nil then Exit;
  with Sender as TDxControl do begin
    vbRect := VisibleRect;
    if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
    vtRect := VirtualRect;

    if g_ClientVersion = cvMirNewUI205 then begin
      bbx := vtRect.Left + 55 + g_ConfigClient.nUserHairOffsetX;
      bby := vtRect.Top + 94 + g_ConfigClient.nUserHairOffsetY;
    end
    else begin
      bbx := vtRect.Left + 52 + g_ConfigClient.nUserHairOffsetX;
      bby := vtRect.Top + 78 + g_ConfigClient.nUserHairOffsetY;
    end;

    IsShowHair := True;
    if g_UseItems[U_FASHIONDRESS].S.Name <> '' then begin
      // 个位用来表示是否显示发型，Expand5 div 10用来表示使提示框文字使用哪个图片作为背景 2019-12-15 10:05:20
      IsShowHair := g_UseItems[U_FASHIONDRESS].S.Expand5 mod 10 = 0;
    end;

    if IsShowHair then begin
      if g_MySelf.m_btOldHair < 4 then begin
        nIdx := 440 + g_MySelf.m_btOldHair;
        if nIdx > 0 then begin
          d := g_WMainImages.GetCachedImage(nIdx, ax, ay);
          if d <> nil then
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
        end;
      end
      else if (g_MySelf.m_btOldHair >= 50) and (g_MySelf.m_btOldHair <= 69) then begin
        nIdx := 350 + g_MySelf.m_btOldHair;
        d := g_WNewopUIImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
    end;

    if g_UseItems[U_FASHIONDRESS].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UseItems[U_FASHIONDRESS].s, @g_UseItemsEffect[U_FASHIONDRESS], True);
      nIdx := g_UseItems[U_FASHIONDRESS].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UseItems[U_FASHIONDRESS].s, @g_UseItemsEffect[U_FASHIONDRESS], False);
    end;

    if g_UseItems[U_FASHIONWEAPON].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UseItems[U_FASHIONWEAPON].s, @g_UseItemsEffect[U_FASHIONWEAPON], True);
      nIdx := g_UseItems[U_FASHIONWEAPON].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UseItems[U_FASHIONWEAPON].s, @g_UseItemsEffect[U_FASHIONWEAPON], False);
    end;

    if g_ClientConfig.boFashionJewelryOpen and (g_UseItems[U_FASHIONHELMET].S.Name <> '') then begin
      nIdx := g_UseItems[U_FASHIONHELMET].S.looks;
      if (nIdx >= 0) then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if (d <> nil) then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;

      FrmDlg.DrawBodyItemEffect(DSWFashionHelmet.VirtualRect, @g_UseItems[U_FASHIONHELMET].s, @g_UseItemsEffect[U_FASHIONHELMET], False);
    end;
  end;
end;

procedure TStateWindows.SetMeridiansLevel(Meridian, Level:Integer); // 设置经脉等级 多少重
begin
  if not Initialized then Exit;
  if (Meridian in [0..4]) and (Level in [0..5]) then begin
    MeridiansFormArray[Meridian].Draw1.ImageIndex := MeridiansImageIndexArray[Meridian] + Level;
  end;
end;

procedure TStateWindows.DTrainingMeridianClick(Sender:TObject; X, Y:Integer); // 修炼经络
{
const
  Arr_Names: array[0..4] of string = ('冲脉', '阴跷', '阴维', '任脉', '奇经');
}
var
  nPage:Integer;
begin
  if g_MySelf = nil then Exit;
  nPage := DMeridiansPageControl.ActivePageIndex - 1;
  if nPage < 0 then nPage := 4;

  {
  if g_MySelf.m_HumMeridians[nPage].Acupoints[4] > 0 then
  begin
    FrmDlg.DMessageDlg('你“' + Arr_Names[nPage] + '”下的所有穴位已全部打通', [mbOk]);
    Exit;
  end;
  }

  frmMain.SendClientMessage(CM_SENDTRAININGMERIDIANCLICK, 0, nPage, 0, 0);
end;

procedure TStateWindows.DBotAcupointsClick(Sender:TObject; X, Y:Integer); // 点击穴位
var
  nPage:Integer;
begin
  if g_MySelf = nil then Exit;
  nPage := DMeridiansPageControl.ActivePageIndex - 1;
  if nPage < 0 then nPage := 4;
  frmMain.SendClientMessage(CM_SENDACUPOINTCLICK, 0, nPage, TDxControl(Sender).Tag, 0);
end;

procedure TStateWindows.DBotAcupointsMouseMove(Sender:TObject; Shift:TShiftState; X, // 穴位鼠标移动事件
  Y:Integer);
var
  sMsg1, sMsg2:string;
  btn:TDxImageButton;
  nCurLevel:Integer; // 当前等级
  lst:TStringList;

  procedure FunA(lst:TStringList; Page, NameIndex:Integer);
  var
    i:Integer;
  const
    DxImageButtonName = 'DBotAcupoints%d_%d';
    NeedLev = '需要内功等级%d级';
  begin
    btn := Sender as TDxImageButton;
    for i := 0 to lst.Count - 1 do begin
      if btn.Name = Format(DxImageButtonName, [NameIndex, i]) then begin
        if g_MySelf.m_HumMeridians[Page].Acupoints[i] > 0 then begin
          sMsg1 := lst[i] + '：已打通';
          Break;
        end { else if g_MySelf.m_HumMeridians[nPage].Level = 0 then}
        else begin
          if nCurLevel >= g_AcupointLevels[Page, i] then begin
            sMsg1 := lst[i] + '：待打通\' + Format(NeedLev, [g_AcupointLevels[Page, i]]);
            Break;
          end
          else begin
            sMsg2 := lst[i] + '：待打通\' + Format(NeedLev, [g_AcupointLevels[Page, i]]);
            Break;
          end;
        end
      end
    end;
  end;

begin
  if g_MySelf = nil then Exit;

  sMsg1 := '';
  sMsg2 := '';
  nCurLevel := g_MySelf.m_AbilNG.Level;

  lst := TStringList.Create;
  try
    lst.Clear;
    case DMeridiansPageControl.ActivePageIndex of
      0:begin
          lst.Add('神冲穴');
          lst.Add('二百穴');
          lst.Add('夹脊穴');
          lst.Add('八风穴');
          lst.Add('涌泉穴');

          FunA(lst, 4, 0);
        end;
      1:begin
          lst.Add('幽门穴');
          lst.Add('通骨穴');
          lst.Add('商曲穴');
          lst.Add('四满穴');
          lst.Add('横骨穴');

          FunA(lst, 0, 1);
        end;
      2:begin
          lst.Add('睛明穴');
          lst.Add('盘缺穴');
          lst.Add('交信穴');
          lst.Add('照海穴');
          lst.Add('然骨穴');

          FunA(lst, 1, 2);
        end;
      3:begin
          lst.Add('廉泉穴');
          lst.Add('期门穴');
          lst.Add('府舍穴');
          lst.Add('冲门穴');
          lst.Add('筑宾穴');

          FunA(lst, 2, 3);
        end;
      4:begin
          lst.Add('承浆穴');
          lst.Add('天突穴');
          lst.Add('鸠尾穴');
          lst.Add('气海穴');
          lst.Add('骨曲穴');

          FunA(lst, 3, 4);
        end;
    end;

  finally
    lst.Free;
  end;

  if sMsg1 <> '' then begin
    DScreen.ShowHint(X, Y, sMsg1, clYellow, FALSE, False, False);
  end;

  if sMsg2 <> '' then begin
    DScreen.ShowHint(X, Y, sMsg2, clRed, FALSE, False, False);
  end;
end;

procedure TStateWindows.DMeridiansLineDirectPaint(Sender:TObject); // 绘制穴位
var
  I:Integer;
  nC:Integer;
  d:TTexture;
  vtRect:TRect;
begin
  if g_MySelf = nil then Exit;
  with Sender as TDxImageForm do begin
    vtRect := VirtualRect;
    nC := 1;
    for I := 0 to 4 do begin
      if g_MySelf.m_HumMeridians[Tag].Acupoints[I] > 0 then begin
        vtRect := AcupointArray[Tag, I].VirtualRect;
        d := g_WMainImages.Images[851];
        if d <> nil then
          GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
        Inc(nC);
      end;
    end;

    Dec(nC);
    if nC in [0..4] then begin
      Inc(g_nAcupointTicks);
      if (g_dwDrawAcupointTick = 0) or ((Cardinal(g_nAcupointTicks) div g_dwDrawAcupointTick) and $01 = 0) then begin
        vtRect := AcupointArray[Tag, nC].VirtualRect;
        d := g_WMainImages.Images[853];
        if d <> nil then
          GameCanvas.DrawBlend(vtRect.Left, vtRect.Top, d);
      end;
    end;
  end;
end;

procedure TStateWindows.DBotMeridiansClick(Sender:TObject; X, Y:Integer); // 选择经络
var
  nPage:Integer;
  sCaption:string;
begin
  if g_MySelf = nil then Exit;
  DMeridiansPageControl.ActivePageIndex := TDxControl(Sender).Tag;

  DStateNGMeridians.UseSetting2 := DMeridiansPageControl.ActivePageIndex = 1;

  case DMeridiansPageControl.ActivePageIndex of
    0:DTrainingMeridian.Caption := '修炼穴位';
    1:DTrainingMeridian.Caption := '修炼冲脉';
    2:DTrainingMeridian.Caption := '修炼阴跷';
    3:DTrainingMeridian.Caption := '修炼阴维';
    4:DTrainingMeridian.Caption := '修炼任脉';
  end;
  nPage := DMeridiansPageControl.ActivePageIndex - 1;
  if nPage < 0 then nPage := 4;

  sCaption := GetMeridianStateInfo(nPage, g_MySelf.m_HumMeridians);
  case DMeridiansPageControl.ActivePageIndex of
    0:;
    1:DLabelMeridiansStatus1.Caption := sCaption;
    2:DLabelMeridiansStatus2.Caption := sCaption;
    3:DLabelMeridiansStatus3.Caption := sCaption;
    4:DLabelMeridiansStatus4.Caption := sCaption;
  end;
end;

procedure TStateWindows.DCloseStateClick(Sender:TObject; X, Y:Integer);
begin
  CloseStateWinDlg;
end;

procedure TStateWindows.DWeaponUS1MouseMove(Sender:TObject; Shift:TShiftState; X,
  Y:Integer);
var
  I, sel, nX, nY, nWidth, nHeight:Integer;
  vtRect, CtrlRect:TRect;
  DrawLeft:Boolean;
  HintWindow:THintWindow;
  HintList:TList;
  TzHintWindows:TList;
  ArrHintWindows:TArrHintWindows;
begin
  sel := TDxImageButton(Sender).Tag;
  if (sel = U_HELMET) and (g_UserState1.UseItems[U_HELMET].s.Name = '') and (g_UserState1.UseItems[U_HAT].s.Name <> '') then // 优先显示斗笠
    sel := U_HAT;

  if sel >= 0 then begin
    g_MouseUserStateItem := g_UserState1.UseItems[sel];

    // 免得不停的重新创建提示窗口 chongchong 2015-01-22
    if (g_MouseUserStateItem.MakeIndex <> g_LastHintMakeIndex) then begin
      with DUserState1 do begin
        if g_MouseUserStateItem.s.Name <> '' then begin
          if g_ClientConfig.boHintWithMouse then begin
            CtrlRect := TDxControl(Sender).VirtualRect;
            vtRect := VirtualRect;
            nY := CtrlRect.Bottom;
            nX := CtrlRect.Left;
            DrawLeft := False;

            // 衣服的属性上移 chongchong 2014-11-10
            if Sender = DDressUS1 then begin
              nY := nY - (CtrlRect.Bottom - CtrlRect.Top) div 2;
            end;
          end
          else begin
            vtRect := VirtualRect;
            nY := vtRect.Top + 40;

            DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
            if DrawLeft then
              nX := vtRect.Left + 40
            else
              nX := vtRect.Right - 20;
          end;

          HintWindows.Clear;
          HintList := TList.Create;

          // 两列显示 piaoyun 2013-10-08
          if not g_ClientConfig.boSingleHint then begin
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseUserStateItem, nX, nY, True, False, DrawLeft);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then
                HintList.Add(HintWindow)
              else
                HintWindow.Free;
            end;

            if not g_ClientConfig.boTZSupportRenameItem then
              TzHintWindows := FrmDlg.GetTzItemHintWindow(2, g_MySelf.m_btSex, g_MouseUserStateItem.s.DBName)
            else
              TzHintWindows := FrmDlg.GetTzItemHintWindow(2, g_MySelf.m_btSex, g_MouseUserStateItem.s.Name);

            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, DrawLeft);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end;

            if (sel = U_HELMET) and (g_UserState1.UseItems[U_HAT].s.Name <> '') then begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_UserState1.UseItems[U_HAT], nX, nY, True, False, DrawLeft);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then begin
                  HintList.Add(HintWindow);
                  HintWindow.Show(nX, nY, False, DrawLeft);
                end
                else
                  HintWindow.Free;
              end;

              if not g_ClientConfig.boTZSupportRenameItem then
                TzHintWindows := FrmDlg.GetTzItemHintWindow(2, pTHumFeature(@g_UserState1.Feature.Buffer).btGender, g_UserState1.UseItems[U_HAT].s.DBName)
              else
                TzHintWindows := FrmDlg.GetTzItemHintWindow(2, pTHumFeature(@g_UserState1.Feature.Buffer).btGender, g_UserState1.UseItems[U_HAT].s.Name);

              if TzHintWindows <> nil then begin
                for I := 0 to TzHintWindows.Count - 1 do begin
                  THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, DrawLeft);
                  HintList.Add(TzHintWindows.Items[I]);
                end;
                TzHintWindows.Free;
              end;
            end;
          end
          else begin
            // 单列显示 piaoyun 2013-10-08
            TzHintWindows := FrmDlg.GetTzItemHintWindowEx(2, g_MySelf.m_btSex, g_MySelf, @g_MouseUserStateItem, True);
            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, DrawLeft);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end
            else begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseUserStateItem, nX, nY, True, False, DrawLeft);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then
                  HintList.Add(HintWindow)
                else
                  HintWindow.Free;
              end;
            end;

            if (sel = U_HELMET) and (g_UserState1.UseItems[U_HAT].s.Name <> '') then begin
              TzHintWindows := FrmDlg.GetTzItemHintWindowEx(2, g_MySelf.m_btSex, g_MySelf, @g_UserState1.UseItems[U_HAT], True);
              if TzHintWindows <> nil then begin
                for I := 0 to TzHintWindows.Count - 1 do begin
                  THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, DrawLeft);
                  HintList.Add(TzHintWindows.Items[I]);
                end;
                TzHintWindows.Free;
              end
              else begin
                for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                  ArrHintWindows[I] := THintWindow.Create;
                end;

                FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_UserState1.UseItems[U_HAT], nX, nY, True, False, DrawLeft);

                for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                  HintWindow := THintWindow(ArrHintWindows[I]);
                  if HintWindow.Count > 0 then begin
                    HintList.Add(HintWindow);
                    HintWindow.Show(nX, nY, False, DrawLeft);
                  end
                  else
                    HintWindow.Free;
                end;
              end;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
          HintList.Free;

        end;
        // FrmDlg.ShowMouseItemInfo(g_MySelf, @g_MouseUserStateItem, vtRect.Left - 30, vtRect.Top + 50, True, 2);
      end;
      g_LastHintMakeIndex := g_MouseUserStateItem.MakeIndex;
    end;
  end
  else begin
    g_MouseUserStateItem.s.Name := '';
    HintWindows.Clear;
  end;
end;

procedure TStateWindows.DUserState1DirectPaint(Sender:TObject);
var
  bbx, bby, nIdx, ax, ay, hair:Integer;
  d:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  IsShowHelmet, IsShowHair:Boolean;
begin

  // 把这个B底层播放放到衣服下面 太阳.... chongchong 2016-08-04
  FrmDlg.DrawBodyItemBelowEffect(DLightUS1.VirtualRect, @g_UserState1.UseItems[DLightUS1.Tag].s, @g_UseItemsEffect1[DLightUS1.Tag]);

  with Sender as TDxControl do begin
    vbRect := VisibleRect;
    if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
    vtRect := VirtualRect;
    //sex := pTHumFeature(@g_UserState1.Feature.Buffer).btGender;
    hair := pTHumFeature(@g_UserState1.Feature.Buffer).btHair;

    if g_ClientVersion = cvMirNewUI205 then begin
      bbx := vtRect.Left + 55 + g_ConfigClient.nOtherUserHairOffsetX;
      bby := vtRect.Top + 94 + g_ConfigClient.nOtherUserHairOffsetY;
    end
    else begin
      bbx := vtRect.Left + 52 + g_ConfigClient.nOtherUserHairOffsetX;
      bby := vtRect.Top + 78 + g_ConfigClient.nOtherUserHairOffsetY;
    end;

    IsShowHair := True;
    if g_UserState1.UseItems[U_DRESS].S.Name <> '' then begin
      // 个位用来表示是否显示发型，Expand5 div 10用来表示使提示框文字使用哪个图片作为背景 2019-12-15 10:05:20
      IsShowHair := g_UserState1.UseItems[U_DRESS].S.Expand5 mod 10 = 0;
    end;

    if IsShowHair then begin
      if hair < 4 then begin
        nIdx := 440 + hair;
        if nIdx > 0 then begin
          d := g_WMainImages.GetCachedImage(nIdx, ax, ay);
          if d <> nil then
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
        end;
      end
      else if (hair >= 50) and (hair <= 69) then begin
        nIdx := 350 + hair;
        d := g_WNewopUIImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
    end;

    if g_UserState1.UseItems[U_DRESS].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UserState1.UseItems[U_DRESS].s, @g_UseItemsEffect1[U_DRESS], True);
      nIdx := g_UserState1.UseItems[U_DRESS].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UserState1.UseItems[U_DRESS].s, @g_UseItemsEffect1[U_DRESS], False);
    end;

    if g_UserState1.UseItems[U_WEAPON].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UserState1.UseItems[U_WEAPON].s, @g_UseItemsEffect1[U_WEAPON], True);
      nIdx := g_UserState1.UseItems[U_WEAPON].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UserState1.UseItems[U_WEAPON].s, @g_UseItemsEffect1[U_WEAPON], False);
    end;
    if g_UserState1.UseItems[U_HELMET].S.Name <> '' then begin
      nIdx := g_UserState1.UseItems[U_HELMET].S.looks;
      { TODO -ochongchong -c修改 : 穿戴16的头盔 15的头盔在内观上不再显示 【2013-08-26】 }
      (*
      if nIdx >= 0 then
      begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(DHelmetUS1.VirtualRect, @g_UserState1.UseItems[U_HELMET].s, @g_UseItemsEffect1[U_HELMET]);
      *)

      IsShowHelmet := True;
      if (g_UserState1.UseItems[U_HELMET].s.StdMode = 15) then begin
        if (g_UserState1.UseItems[U_HAT].S.Name <> '') and (g_UserState1.UseItems[U_HAT].S.StdMode = 16) and (g_UserState1.UseItems[U_HAT].S.Shape <> 1000) {1000 为面巾，面巾和头盔可同时显示} then
          IsShowHelmet := False;
      end;
      if (nIdx >= 0) and IsShowHelmet then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then begin
          if g_ClientConfig.boHelmetShowInBox then begin
            ax := DHelmetUS1.VirtualRect.Left + (DHelmetUS1.Width - d.Width) div 2;
            ay := DHelmetUS1.VirtualRect.Top + (DHelmetUS1.Height - d.Height) div 2;
            GameCanvas.Draw(ax, ay, d.ClientRect, d);
          end
          else begin
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
          end;
        end;
      end;
      if IsShowHelmet then
        FrmDlg.DrawBodyItemEffect(DHelmetUS1.VirtualRect, @g_UserState1.UseItems[U_HELMET].s, @g_UseItemsEffect1[U_HELMET], False);
    end;
    if g_UserState1.UseItems[U_HAT].S.Name <> '' then begin
      nIdx := g_UserState1.UseItems[U_HAT].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(DHelmetUS1.VirtualRect, @g_UserState1.UseItems[U_HAT].s, @g_UseItemsEffect1[U_HAT], False);
    end;

    if g_UserState1.UseItems[U_SHIELD].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(DShieldUS1.VirtualRect, @g_UserState1.UseItems[U_SHIELD].s, @g_UseItemsEffect1[U_SHIELD], True);
      nIdx := g_UserState1.UseItems[U_SHIELD].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(DShieldUS1.VirtualRect, @g_UserState1.UseItems[U_SHIELD].s, @g_UseItemsEffect1[U_SHIELD], False);
    end;

    {
    if g_UserState1.UseItems[U_HAT].S.Name <> '' then begin
      nIdx := g_UserState1.UseItems[U_HAT].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(DHelmetUS1.VirtualRect, @g_UserState1.UseItems[U_HAT].s, @g_UseItemsEffect1[U_HAT]);
    end else begin
      if g_UserState1.UseItems[U_HELMET].S.Name <> '' then begin
        nIdx := g_UserState1.UseItems[U_HELMET].S.looks;
        if nIdx >= 0 then begin
          d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
          if d <> nil then
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
        end;
        FrmDlg.DrawBodyItemEffect(DHelmetUS1.VirtualRect, @g_UserState1.UseItems[U_HELMET].s, @g_UseItemsEffect1[U_HELMET]);
      end;
    end;
    }
  end;
end;

procedure TStateWindows.DUserState1FashionDirectPaint(Sender:TObject);
var
  bbx, bby, nIdx, ax, ay, hair:Integer;
  d:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  IsShowHair:Boolean;
begin
  with Sender as TDxControl do begin
    vbRect := VisibleRect;
    if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
    vtRect := VirtualRect;
    //sex := pTHumFeature(@g_UserState1.Feature.Buffer).btGender;
    hair := pTHumFeature(@g_UserState1.Feature.Buffer).btOldHair;

    if g_ClientVersion = cvMirNewUI205 then begin
      bbx := vtRect.Left + 55 + g_ConfigClient.nOtherUserHairOffsetX;
      ;
      bby := vtRect.Top + 94 + g_ConfigClient.nOtherUserHairOffsetY;
    end
    else begin
      bbx := vtRect.Left + 52 + g_ConfigClient.nOtherUserHairOffsetX;
      bby := vtRect.Top + 78 + g_ConfigClient.nOtherUserHairOffsetY;
    end;

    IsShowHair := True;
    if g_UserState1.UseItems[U_FASHIONDRESS].S.Name <> '' then begin
      // 个位用来表示是否显示发型，Expand5 div 10用来表示使提示框文字使用哪个图片作为背景 2019-12-15 10:05:20
      IsShowHair := g_UserState1.UseItems[U_FASHIONDRESS].S.Expand5 mod 10 = 0;
    end;

    if IsShowHair then begin
      if hair < 4 then begin
        nIdx := 440 + hair;

        if nIdx > 0 then begin
          d := g_WMainImages.GetCachedImage(nIdx, ax, ay);
          if d <> nil then
            GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
        end;
      end
      else if (hair >= 50) and (hair <= 69) then begin
        nIdx := 350 + hair;
        d := g_WNewopUIImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
    end;

    if g_UserState1.UseItems[U_FASHIONDRESS].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UserState1.UseItems[U_FASHIONDRESS].s, @g_UseItemsEffect1[U_FASHIONDRESS], True);
      nIdx := g_UserState1.UseItems[U_FASHIONDRESS].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UserState1.UseItems[U_FASHIONDRESS].s, @g_UseItemsEffect1[U_FASHIONDRESS], False);
    end;

    if g_UserState1.UseItems[U_FASHIONWEAPON].S.Name <> '' then begin
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UserState1.UseItems[U_FASHIONWEAPON].s, @g_UseItemsEffect1[U_FASHIONWEAPON], True);
      nIdx := g_UserState1.UseItems[U_FASHIONWEAPON].S.looks;
      if nIdx >= 0 then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if d <> nil then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;
      FrmDlg.DrawBodyItemEffect(bbx, bby, @g_UserState1.UseItems[U_FASHIONWEAPON].s, @g_UseItemsEffect1[U_FASHIONWEAPON], False);
    end;

    if g_ClientConfig.boFashionJewelryOpen and (g_UserState1.UseItems[U_FASHIONHELMET].S.Name <> '') then begin
      nIdx := g_UserState1.UseItems[U_FASHIONHELMET].S.looks;
      if (nIdx >= 0) then begin
        d := g_WStateItemImages.GetCachedImage(nIdx, ax, ay);
        if (d <> nil) then
          GameCanvas.Draw(bbx + ax, bby + ay, d.ClientRect, d);
      end;

      FrmDlg.DrawBodyItemEffect(DFashionHelmetUS1.VirtualRect, @g_UserState1.UseItems[U_FASHIONHELMET].s, @g_UseItemsEffect1[U_FASHIONHELMET], False);
    end;
  end;
end;

procedure TStateWindows.DHeroCloseStateClick(Sender:TObject; X, Y:Integer);
begin
  CloseHeroStateWinDlg;
end;

procedure TStateWindows.DCloseUS1Click(Sender:TObject; X, Y:Integer);
begin
  CloseUserState1Dlg;
end;

procedure TStateWindows.OpenStateWinDlg;
begin
  if not Initialized then Exit;
  DStateWin.Visible := True;
  btnDStateWinEx.IsOpen := False;
end;

procedure TStateWindows.CloseStateWinDlg;
begin
  if not Initialized then Exit;
  DStateWin.Visible := False;
  DStateWinEx.Visible := False;
  btnDStateWinEx.IsOpen := False;
  FengHaoHintWindow.Clear;
end;

procedure TStateWindows.OpenUserState1Dlg;
begin
  if not Initialized then Exit;
  DUserState1.Visible := True;
end;

procedure TStateWindows.CloseUserState1Dlg;
begin
  if not Initialized then Exit;
  DUserState1.Visible := False;
  FengHaoHintWindow.Clear;
end;

procedure TStateWindows.OpenMyStatus; //打开人物属性对话框
begin
  if not Initialized then Exit;
  if g_MySelf = nil then Exit;
  DStateWin.Visible := not DStateWin.Visible;
  if DStateWin.Visible then begin
    DStatePageControl.ActivePageIndex := 0;
    DBasicPageControl.ActivePageIndex := 0;

    SetMySelfActivePageIndexCount;

    DStateBasic.IsMale := g_MySelf.m_btSex = 0;
    RefreshMyFashionJewelryInfo;

    LabelDStateWinCharName.Caption := g_MySelf.m_sUserName;
    LabelDStateWinCharName.CaptionColor.Up.Color := g_MySelf.m_nNameColor;

    if btnDStateWinEx.Visible and g_ConfigClient.boDefShowStateWinEx then begin
      btnDStateWinEx.IsOpen := True;

      if DStateWin.Left + DStateWin.Width + DStateWinEx.Width > SCREENWIDTH then
        DStateWin.Left := SCREENWIDTH - DStateWin.Width - DStateWinEx.Width;
      DStateWinEx.Left := DStateWin.Left + DStateWin.Width;
      DStateWinEx.Top := DStateWin.Top;
      DStateWinEx.Visible := True;

      RefreshDStateWinExInfo;
    end
  end else begin
    DStateWinEx.Visible := False;
    btnDStateWinEx.IsOpen := False;
  end;
end;

procedure TStateWindows.ChangeMySelfMeridiansctivePage;
var
  ActivePageIndex:Integer;
begin
  if not Initialized then Exit;
  if g_MySelf = nil then Exit;
  ActivePageIndex := DMeridiansPageControl.ActivePageIndex;

  DMeridiansTabSheet1.TabVisible := g_MySelf.m_HumMeridians[4].Acupoints[4] > 0;
  DMeridiansTabSheet2.TabVisible := g_MySelf.m_HumMeridians[0].Acupoints[4] > 0;
  DMeridiansTabSheet3.TabVisible := g_MySelf.m_HumMeridians[1].Acupoints[4] > 0;
  DMeridiansTabSheet4.TabVisible := g_MySelf.m_HumMeridians[2].Acupoints[4] > 0;

  DBotMeridians1.Visible := g_MySelf.m_HumMeridians[4].Acupoints[4] > 0;
  DBotMeridians2.Visible := g_MySelf.m_HumMeridians[0].Acupoints[4] > 0;
  DBotMeridians3.Visible := g_MySelf.m_HumMeridians[1].Acupoints[4] > 0;
  DBotMeridians4.Visible := g_MySelf.m_HumMeridians[2].Acupoints[4] > 0;

  if (not DMeridiansTabSheet4.TabVisible) and (DMeridiansPageControl.ActivePageIndex >= 4) then begin
    DMeridiansPageControl.ActivePageIndex := 3;
    DBotMeridians3.Checked := True;
    DBotMeridiansClick(DBotMeridians3, 0, 0);
  end;
  if (not DMeridiansTabSheet3.TabVisible) and (DMeridiansPageControl.ActivePageIndex >= 3) then begin
    DMeridiansPageControl.ActivePageIndex := 2;
    DBotMeridians2.Checked := True;
    DBotMeridiansClick(DBotMeridians2, 0, 0);
  end;
  if (not DMeridiansTabSheet2.TabVisible) and (DMeridiansPageControl.ActivePageIndex >= 2) then begin
    DMeridiansPageControl.ActivePageIndex := 1;
    DBotMeridians1.Checked := True;
    DBotMeridiansClick(DBotMeridians1, 0, 0);
  end;

  DMeridiansPageControl.ActivePageIndex := ActivePageIndex;
  case DMeridiansPageControl.ActivePageIndex of
    0:begin
        DBotMeridians0.Checked := True;
      end;
    1:begin
        DBotMeridians1.Checked := True;
      end;
    2:begin
        DBotMeridians2.Checked := True;
      end;
    3:begin
        DBotMeridians3.Checked := True;
      end;
    4:begin
        DBotMeridians4.Checked := True;
      end;
  end;
end;

procedure TStateWindows.ChangeMyHeroMeridiansctivePage;
var
  ActivePageIndex:Integer;
begin
  if not Initialized then Exit;
  if g_MyHero = nil then Exit;
  ActivePageIndex := DHeroMeridiansPageControl.ActivePageIndex;
  DHeroMeridiansTabSheet2.TabVisible := g_MyHero.m_HumMeridians[0].Acupoints[4] > 0;
  DHeroMeridiansTabSheet3.TabVisible := g_MyHero.m_HumMeridians[1].Acupoints[4] > 0;
  DHeroMeridiansTabSheet4.TabVisible := g_MyHero.m_HumMeridians[2].Acupoints[4] > 0;
  DHeroBotMeridians2.Visible := g_MyHero.m_HumMeridians[0].Acupoints[4] > 0;
  DHeroBotMeridians3.Visible := g_MyHero.m_HumMeridians[1].Acupoints[4] > 0;
  DHeroBotMeridians4.Visible := g_MyHero.m_HumMeridians[2].Acupoints[4] > 0;

  if (not DHeroMeridiansTabSheet4.TabVisible) and (DHeroMeridiansPageControl.ActivePageIndex >= 4) then begin
    DHeroMeridiansPageControl.ActivePageIndex := 3;
    DHeroBotMeridians3.Checked := True;
    DHeroBotMeridiansClick(DHeroBotMeridians3, 0, 0);
  end;
  if (not DHeroMeridiansTabSheet3.TabVisible) and (DHeroMeridiansPageControl.ActivePageIndex >= 3) then begin
    DHeroMeridiansPageControl.ActivePageIndex := 2;
    DHeroBotMeridians2.Checked := True;
    DHeroBotMeridiansClick(DBotMeridians2, 0, 0);
  end;
  if (not DHeroMeridiansTabSheet2.TabVisible) and (DHeroMeridiansPageControl.ActivePageIndex >= 2) then begin
    DHeroMeridiansPageControl.ActivePageIndex := 1;
    DHeroBotMeridians1.Checked := True;
    DHeroBotMeridiansClick(DHeroBotMeridians1, 0, 0);
  end;
  DHeroMeridiansPageControl.ActivePageIndex := ActivePageIndex;
  case DHeroMeridiansPageControl.ActivePageIndex of
    0:begin
        DHeroBotMeridians0.Checked := True;
      end;
    1:begin
        DHeroBotMeridians1.Checked := True;
      end;
    2:begin
        DHeroBotMeridians2.Checked := True;
      end;
    3:begin
        DHeroBotMeridians3.Checked := True;
      end;
    4:begin
        DHeroBotMeridians4.Checked := True;
      end;
  end;
end;

procedure TStateWindows.SetMySelfLastContinuousButton;
begin
  if not Initialized then Exit;
  DSMB4.Enabled := g_boOpenLastContinuous;
end;

procedure TStateWindows.SetMyHeroLastContinuousButton;
begin
  if not Initialized then Exit;

  if not g_ConfigClient.boCustomUI then begin
    if g_ClientVersion <> cvMirNewUI205 then begin
      if g_boHeroOpenLastContinuous then begin
        DHeroSMB4.Left := 214;
        DHeroSMB4.Top := 13;
      end
      else begin
        DHeroSMB4.Left := 210;
        DHeroSMB4.Top := 8;
      end;
    end
    else begin
      if g_boHeroOpenLastContinuous then begin
        DHeroSMB4.Left := 213;
        DHeroSMB4.Top := 22;
      end
      else begin
        DHeroSMB4.Left := 208;
        DHeroSMB4.Top := 17;
      end;
    end;
  end;
end;

procedure TStateWindows.SetMySelfActivePageIndexCount;
begin
  if not Initialized then Exit;
  if DStateWin.Visible then begin
    if g_MySelf.m_boTrainingXF then {// 三个} begin
      MainTabSheet1.TabVisible := True;
      MainTabSheet2.TabVisible := True;
      MainTabSheet3.TabVisible := True;

      DStatePageControl.DrawButton := True;
    end
    else if g_MySelf.m_boTrainingNG then begin // 二个
      MainTabSheet1.TabVisible := True;
      MainTabSheet2.TabVisible := True;
      MainTabSheet3.TabVisible := False;

      DStatePageControl.DrawButton := True;
    end
    else begin
      MainTabSheet1.TabVisible := True;
      MainTabSheet2.TabVisible := False;
      MainTabSheet3.TabVisible := False;

      DStatePageControl.DrawButton := False;
    end;

    LabelDStateWinCharName.Caption := g_MySelf.m_sUserName;
    LabelDStateWinCharName.CaptionColor.Up.Color := g_MySelf.m_nNameColor;
  end;

end;

procedure TStateWindows.SetMyHeroActivePageIndexCount;
begin
  if not Initialized then Exit;
  if g_MyHero = nil then Exit;

  if DHeroStateWin.Visible then begin
    RefreshHeroFashionJewelryInfo;
    if g_MyHero.m_boTrainingXF then begin // 三个
      HeroMainTabSheet1.TabVisible := True;
      HeroMainTabSheet2.TabVisible := True;
      HeroMainTabSheet3.TabVisible := True;

      DHeroStatePageControl.DrawButton := True;
    end
    else if g_MyHero.m_boTrainingNG then begin // 二个
      HeroMainTabSheet1.TabVisible := True;
      HeroMainTabSheet2.TabVisible := True;
      HeroMainTabSheet3.TabVisible := False;

      DHeroStatePageControl.DrawButton := True;
    end
    else begin
      HeroMainTabSheet1.TabVisible := True;
      HeroMainTabSheet2.TabVisible := False;
      HeroMainTabSheet3.TabVisible := False;

      DHeroStatePageControl.DrawButton := False;
    end;
    DHeroStatePageControl.ActivePageIndex := 0;
  end;

  ////
end;

procedure TStateWindows.OpenMyMagic;
begin
  if not Initialized then Exit;
  if g_MySelf = nil then Exit;
  DStateWin.Visible := not DStateWin.Visible;
  if DStateWin.Visible then begin
    DStatePageControl.ActivePage := MainTabSheet1;
    DBasicPageControl.ActivePage := DStateTabSheet6;

    SetMySelfActivePageIndexCount;

    if btnDStateWinEx.Visible and g_ConfigClient.boDefShowStateWinEx then begin
      btnDStateWinEx.IsOpen := True;

      if DStateWin.Left + DStateWin.Width + DStateWinEx.Width > SCREENWIDTH then
        DStateWin.Left := SCREENWIDTH - DStateWin.Width - DStateWinEx.Width;
      DStateWinEx.Left := DStateWin.Left + DStateWin.Width;
      DStateWinEx.Top := DStateWin.Top;
      DStateWinEx.Visible := True;

      RefreshDStateWinExInfo;
    end
  end
  else begin
    DStateWinEx.Visible := False;
    btnDStateWinEx.IsOpen := False;
  end;
end;

procedure TStateWindows.OpenUserState();
begin
  if not Initialized then Exit;

  if g_UserState1.RaceServer = RC_HEROOBJECT then begin
    DUserState1StateTabSheet5.TabVisible := not g_ClientConfig.boHeroHideTabSheet5;
    DUserState1StateTabSheet2.TabVisible := not g_ClientConfig.boHeroHideTabSheet2;
  end
  else begin
    DUserState1StateTabSheet5.TabVisible := not g_ClientConfig.boHideTabSheet5;
    DUserState1StateTabSheet2.TabVisible := not g_ClientConfig.boHideTabSheet2;
  end;

  DUserState1StateBasic.IsMale := pTHumFeature(@g_UserState1.feature.Buffer).btGender = 0;

  RefreshUserFashionJewelryInfo;

  DUserState1BasicPageControl.ActivePageIndex := 0;

  DUserState1LabelDStateWinCharName.Caption := g_UserState1.UserName;
  DUserState1LabelDStateWinCharName.CaptionColor.Up.Color := g_UserState1.NameColor;

  DUserState1LabelDStateWinRankName.Caption := g_UserState1.GuildName + ' ' + g_UserState1.GuildRankName;

  DJewelryBoxUS1.Visible := g_UserState1.JewelryBoxStatus = jbsOpen;
  DGodBlessUS1.Visible := g_UserState1.ShowGodBless;

  DUserState1.Visible := True;
end;

procedure TStateWindows.MySelfAbilChange;

  function AddSpace(Str:string):string;
  var
    Len:Integer;
    P:PChar;
  begin
    Result := Str;
    if Length(Result) < 74 then
      SetLength(Result, 74);
    P := PChar(Result);
    Len := Length(Str);
    P := P + Len;
    while Len < 74 do begin
      P^ := ' ';
      Inc(Len);
      Inc(P);
    end;
  end;
var
  I:Integer;
begin
  if not Initialized then Exit;

  if g_MySelf = nil then Exit;

  DStateBasic.IsMale := g_MySelf.m_btSex = 0;
  RefreshMyFashionJewelryInfo;

  DStateNGMeridians.IsMale := g_MySelf.m_btSex = 0;
  DStateNGMeridians.UseSetting2 := DMeridiansPageControl.ActivePageIndex = 1;

  DStateJobLabel.Caption := '职业      ：' + GetJobName(g_MySelf.m_btJob);
  DStateLevelLabel.Caption := '等级      ：' + IntToStr(g_MySelf.m_Abil.Level);
  DStateExpLabel.Caption := '当前经验  ：' + IntToStr(g_MySelf.m_Abil.Exp);
  DStateMaxExpLabel.Caption := '升级经验  ：' + IntToStr(g_MySelf.m_Abil.MaxExp);
  DStateHPLabel.Caption := '体力值    ：' + IntToStr(g_MySelf.m_Abil.HP);
  DStateMPLabel.Caption := '魔法值    ：' + IntToStr(g_MySelf.m_Abil.MP);
  DStatePotencyLabel.Caption := '药力值    ：' + IntToStr(g_MySelf.m_Alcohol.MedicineValue);
  DStateLiquorLabel.Caption := '酒量      ：' + IntToStr(g_MySelf.m_Alcohol.Alcohol);
  DStateGameDiamondLabel.Caption := Format('%-10s：', [g_sGameDiamondName]) + IntToStr(g_nGameDiamond);
  DStateGameGirdLabel.Caption := Format('%-10s：', [g_sGameGirdName]) + IntToStr(g_nGameGird);
  DStateGameGoldLabel.Caption := Format('%-10s：', [g_sGameGoldName]) + IntToStr(g_MySelf.m_nGameGold);
  DStateGameTimeGirdLabel.Caption := Format('%-10s：', [g_sGamePointName]) + IntToStr(g_MySelf.m_nGamePoint);
  DStateCreditPointLabel.Caption := Format('%-10s：', [g_sCreditPointName]) + IntToStr(g_MySelf.m_Abil.CreditPoint);
  DStateBigGoldLabel.Caption := FLableCaption_BigGold + IntToStr(g_nGameGlory);

  DStateLabelAC.Caption := FLabelCaption_AC + ' ' + IntToStr(g_MySelf.m_Abil.AC1) + '-' + IntToStr(g_MySelf.m_Abil.AC2);
  DStateLabelMAC.Caption := FLabelCaption_MAC + ' ' + IntToStr(g_MySelf.m_Abil.MAC1) + '-' + IntToStr(g_MySelf.m_Abil.MAC2);
  DStateLabelDC.Caption := FLabelCaption_DC + ' ' + IntToStr(g_MySelf.m_Abil.DC1) + '-' + IntToStr(g_MySelf.m_Abil.DC2);
  DStateLabelMC.Caption := FLabelCaption_MC + ' ' + IntToStr(g_MySelf.m_Abil.MC1) + '-' + IntToStr(g_MySelf.m_Abil.MC2);
  DStateLabelSC.Caption := FLabelCaption_SC + ' ' + IntToStr(g_MySelf.m_Abil.SC1) + '-' + IntToStr(g_MySelf.m_Abil.SC2);

  DStateHitSpeedLabel.Caption := FLabelCaption_HitSpeed + ' ' + IntToStr(g_MySelf.m_nAttackSpeed);
  DStateHitPointLabel.Caption := FLabelCaption_HitPoint + ' ' + IntToStr(g_nMyHitPoint);
  DStateSpeedPointLabel.Caption := FLabelCaption_SpeedPoint + ' ' + IntToStr(g_nMySpeedPoint);

  DStateWeightLabel.Caption := FLabelCaption_Weight + ' ' + IntToStr(g_MySelf.m_Abil.Weight) + '/' + IntToStr(g_MySelf.m_Abil.MaxWeight);
  if g_MySelf.m_Abil.Weight > g_MySelf.m_Abil.MaxWeight then begin
    DStateWeightLabel.CaptionColor.Up.Color := clRed;
    DStateWeightLabel.CaptionColor.Hot.Color := clRed;
    DStateWeightLabel.CaptionColor.Down.Color := clRed;
  end
  else begin
    DStateWeightLabel.CaptionColor.Up.Color := FStateWeightLabelColor.Up;
    DStateWeightLabel.CaptionColor.Hot.Color := FStateWeightLabelColor.Hot;
    DStateWeightLabel.CaptionColor.Down.Color := FStateWeightLabelColor.Down;
    DStateWeightLabel.CaptionColor.Disabled.Color := FStateWeightLabelColor.Disabled;
  end;

  DStateWearWeightLabel.Caption := FLabelCaption_WearWeight + ' ' + IntToStr(g_MySelf.m_Abil.WearWeight) + '/' + IntToStr(g_MySelf.m_Abil.MaxWearWeight);
  if g_MySelf.m_Abil.WearWeight > g_MySelf.m_Abil.MaxWearWeight then begin
    DStateWearWeightLabel.CaptionColor.Up.Color := clRed;
    DStateWearWeightLabel.CaptionColor.Hot.Color := clRed;
    DStateWearWeightLabel.CaptionColor.Down.Color := clRed;
  end
  else begin
    DStateWearWeightLabel.CaptionColor.Up.Color := FStateWearWeightLabelColor.Up;
    DStateWearWeightLabel.CaptionColor.Hot.Color := FStateWearWeightLabelColor.Hot;
    DStateWearWeightLabel.CaptionColor.Down.Color := FStateWearWeightLabelColor.Down;
    DStateWearWeightLabel.CaptionColor.Disabled.Color := FStateWearWeightLabelColor.Disabled;
  end;

  DStateHandWeightLabel.Caption := FLabelCaption_HandWeight + ' ' + IntToStr(g_MySelf.m_Abil.HandWeight) + '/' + IntToStr(g_MySelf.m_Abil.MaxHandWeight);
  if g_MySelf.m_Abil.HandWeight > g_MySelf.m_Abil.MaxHandWeight then begin
    DStateHandWeightLabel.CaptionColor.Up.Color := clRed;
    DStateHandWeightLabel.CaptionColor.Hot.Color := clRed;
    DStateHandWeightLabel.CaptionColor.Down.Color := clRed;
  end
  else begin
    DStateHandWeightLabel.CaptionColor.Up.Color := FStateHandWeightLabelColor.Up;
    DStateHandWeightLabel.CaptionColor.Hot.Color := FStateHandWeightLabelColor.Hot;
    DStateHandWeightLabel.CaptionColor.Down.Color := FStateHandWeightLabelColor.Down;
    DStateHandWeightLabel.CaptionColor.Disabled.Color := FStateHandWeightLabelColor.Disabled;
  end;

  DStateAntiMagicLabel.Caption := FLabelCaption_AntiMagic + ' +' + IntToStr(g_nMyAntiMagic * 10) + '%';
  DStateAntiPoisonLabel.Caption := FLabelCaption_AntiPoison + ' +' + IntToStr(g_nMyAntiPoison * 10) + '%';

  DStatePoisonRecoverLabel.Caption := FLabelCaption_PoisonRecover + ' +' + IntToStr(g_nMyPoisonRecover * 10) + '%';
  DStateHealthRecoverLabel.Caption := FLabelCaption_HealthRecover + ' +' + IntToStr(g_nMyHealthRecover * 10) + '%';
  DStateSpellRecoverLabel.Caption := FLabelCaption_SpellRecover + ' +' + IntToStr(g_nMySpellRecover * 10) + '%';

  {
  DStateNewAbilLabel1.Caption := Format('暴击几率 +%d', [g_MySelf.m_Abil.NewValue[0]]) + '%';
  DStateNewAbilLabel2.Caption := Format('攻击伤害 +%d', [g_MySelf.m_Abil.NewValue[1]]) + '%';
  DStateNewAbilLabel3.Caption := Format('伤害吸收 +%d', [g_MySelf.m_Abil.NewValue[2]]) + '%';
  DStateNewAbilLabel4.Caption := Format('魔法防御 +%d', [g_MySelf.m_Abil.NewValue[3]]) + '%';
  DStateNewAbilLabel5.Caption := Format('忽视防御 +%d', [g_MySelf.m_Abil.NewValue[4]]) + '%';
  DStateNewAbilLabel6.Caption := Format('伤害反弹 +%d', [g_MySelf.m_Abil.NewValue[5]]) + '%';
  DStateNewAbilLabel7.Caption := Format('体力增加 +%d', [g_MySelf.m_Abil.NewValue[7]]) + '%';
  DStateNewAbilLabel8.Caption := Format('魔力增加 +%d', [g_MySelf.m_Abil.NewValue[8]]) + '%';
  DStateNewAbilLabel9.Caption := Format('怒气恢复 +%d', [g_MySelf.m_Abil.NewValue[9]]) + '%';
  DStateNewAbilLabel10.Caption := Format('合击伤害 +%d', [g_MySelf.m_Abil.NewValue[10]]) + '%';
  DStateNewAbilLabel11.Caption := Format('目标暴率 +%d', [g_MySelf.m_Abil.NewValue[6]]) + '%';
  }
  DStateNewAbilLabel1.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel1.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel1.Tag]]);
  DStateNewAbilLabel2.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel2.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel2.Tag]]);
  DStateNewAbilLabel3.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel3.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel3.Tag]]);
  DStateNewAbilLabel4.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel4.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel4.Tag]]);
  DStateNewAbilLabel5.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel5.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel5.Tag]]);
  DStateNewAbilLabel6.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel6.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel6.Tag]]);
  DStateNewAbilLabel7.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel7.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel7.Tag]]);
  DStateNewAbilLabel8.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel8.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel8.Tag]]);
  DStateNewAbilLabel9.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel9.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel9.Tag]]);
  DStateNewAbilLabel10.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel10.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel10.Tag]]);
  DStateNewAbilLabel11.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel11.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel11.Tag]]);
  DStateNewAbilLabel12.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel12.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel12.Tag]]);
  DStateNewAbilLabel13.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel13.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel13.Tag]]);
  DStateNewAbilLabel14.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel14.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel14.Tag]]);
  DStateNewAbilLabel15.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel15.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel15.Tag]]);
  DStateNewAbilLabel16.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DStateNewAbilLabel16.Tag], g_MySelf.m_Abil.NewValue[DStateNewAbilLabel16.Tag]]);

  // -----------------------------------------------------------------------------
  DStateNGLevelValue.Caption := IntToStr(g_MySelf.m_AbilNG.Level);
  DStateNGExpValue.Caption := IntToStr(g_MySelf.m_AbilNG.Exp);
  DStateNGMaxExpValue.Caption := IntToStr(g_MySelf.m_AbilNG.MaxExp);
  DStateNGValue.Caption := IntToStr(g_MySelf.m_AbilNG.NH) + '/' + IntToStr(g_MySelf.m_AbilNG.MaxNH);
  DStateNGRecoverValue.Caption := IntToStr(g_nMyNPRecoverTime);
  DStateNGHitValue.Caption := IntToStr(g_MySelf.m_AbilNG.NGDamage);
  DStateNGAntiPoisonValue.Caption := IntToStr(g_MySelf.m_AbilNG.UnNGDamage);
  DStateHJAntiPoisonValue.Caption := ''; // DStateHJAntiPoisonLabel.Caption := '合击伤害减免';
  // -----------------------------------------------------------------------------

   { nPage := DMeridiansPageControl.ActivePageIndex - 1;
    if nPage < 0 then nPage := 4;

    if g_MySelf.m_HumMeridians[nPage].Acupoints[4] > 0 then
      sCaption := '经'#13 + '络'#13 + '已'#13 + '通'
    else
      sCaption := '经'#13 + '络'#13 + '未'#13 + '通';

    case DMeridiansPageControl.ActivePageIndex of
      0: ;
      1: DLabelMeridiansStatus1.Caption := sCaption;
      2: DLabelMeridiansStatus2.Caption := sCaption;
      3: DLabelMeridiansStatus3.Caption := sCaption;
      4: DLabelMeridiansStatus4.Caption := sCaption;
    end; }
  for I := 0 to Length(g_MySelf.m_HumMeridians) - 1 do begin
    SetMeridiansLevel(I, g_MySelf.m_HumMeridians[I].Level);
  end;

  ChangeMySelfMeridiansctivePage;

  DLabelMeridiansStatus1.Caption := GetMeridianStateInfo(0, g_MySelf.m_HumMeridians);
  DLabelMeridiansStatus2.Caption := GetMeridianStateInfo(1, g_MySelf.m_HumMeridians);
  DLabelMeridiansStatus3.Caption := GetMeridianStateInfo(2, g_MySelf.m_HumMeridians);
  DLabelMeridiansStatus4.Caption := GetMeridianStateInfo(3, g_MySelf.m_HumMeridians);

  RefreshDStateWinExInfo;
end;

procedure TStateWindows.MyHeroAbilChange;
var
  I:Integer;
begin
  if not Initialized then Exit;
  if g_MyHero = nil then Exit;

  DHeroStateBasic.IsMale := g_MyHero.m_btSex = 0;
  RefreshHeroFashionJewelryInfo;

  DHeroStateNGMeridians.IsMale := g_MyHero.m_btSex = 0;
  DHeroStateNGMeridians.UseSetting2 := DHeroMeridiansPageControl.ActivePageIndex = 1;

  DHeroStateJobLabel.Caption := '职业      ：' + GetJobName(g_MyHero.m_btJob);
  DHeroStateLevelLabel.Caption := '等级      ：' + IntToStr(g_MyHero.m_Abil.Level);

  DHeroStateCreditPointLabel.Caption := Format('%-10s：', [g_sCreditPointName]) + IntToStr(g_MyHero.m_Abil.CreditPoint);
  DHeroStateExpLabel.Caption := '当前经验  ：' + IntToStr(g_MyHero.m_Abil.Exp);
  DHeroStateMaxExpLabel.Caption := '升级经验  ：' + IntToStr(g_MyHero.m_Abil.MaxExp);
  DHeroStateHPLabel.Caption := '体力值    ：' + IntToStr(g_MyHero.m_Abil.HP);
  DHeroStateMPLabel.Caption := '魔法值    ：' + IntToStr(g_MyHero.m_Abil.MP);
  DHeroStatePotencyLabel.Caption := '药力值    ：' + IntToStr(g_MyHero.m_Alcohol.MedicineValue);
  DHeroStateLiquorLabel.Caption := '酒量      ：' + IntToStr(g_MyHero.m_Alcohol.Alcohol);

  DHeroStateLabelAC.Caption := FLabelCaption_HeroAC + ' ' + IntToStr(g_MyHero.m_Abil.AC1) + '-' + IntToStr(g_MyHero.m_Abil.AC2);
  DHeroStateLabelMAC.Caption := FLabelCaption_HeroMAC + ' ' + IntToStr(g_MyHero.m_Abil.MAC1) + '-' + IntToStr(g_MyHero.m_Abil.MAC2);
  DHeroStateLabelDC.Caption := FLabelCaption_HeroDC + ' ' + IntToStr(g_MyHero.m_Abil.DC1) + '-' + IntToStr(g_MyHero.m_Abil.DC2);
  DHeroStateLabelMC.Caption := FLabelCaption_HeroMC + ' ' + IntToStr(g_MyHero.m_Abil.MC1) + '-' + IntToStr(g_MyHero.m_Abil.MC2);
  DHeroStateLabelSC.Caption := FLabelCaption_HeroSC + ' ' + IntToStr(g_MyHero.m_Abil.SC1) + '-' + IntToStr(g_MyHero.m_Abil.SC2);

  DHeroStateHitSpeedLabel.Caption := FLabelCaption_HeroHitSpeed + ' ' + IntToStr(g_MyHero.m_nAttackSpeed);
  DHeroStateHitPointLabel.Caption := FLabelCaption_HeroHitPoint + ' ' + IntToStr(g_nHeroHitPoint);
  DHeroStateSpeedPointLabel.Caption := FLabelCaption_HeroSpeedPoint + ' ' + IntToStr(g_nHeroSpeedPoint);

  DHeroStateWeightLabel.Caption := FLabelCaption_HeroWeight + ' ' + IntToStr(g_MyHero.m_Abil.Weight) + '/' + IntToStr(g_MyHero.m_Abil.MaxWeight);
  if g_MyHero.m_Abil.Weight > g_MyHero.m_Abil.MaxWeight then begin
    DHeroStateWeightLabel.CaptionColor.Up.Color := clRed;
    DHeroStateWeightLabel.CaptionColor.Hot.Color := clRed;
    DHeroStateWeightLabel.CaptionColor.Down.Color := clRed;
  end
  else begin
    DHeroStateWeightLabel.CaptionColor.Up.Color := clSilver;
    DHeroStateWeightLabel.CaptionColor.Hot.Color := clSilver;
    DHeroStateWeightLabel.CaptionColor.Down.Color := clSilver;
  end;

  DHeroStateWearWeightLabel.Caption := FLabelCaption_HeroWearWeight + ' ' + IntToStr(g_MyHero.m_Abil.WearWeight) + '/' + IntToStr(g_MyHero.m_Abil.MaxWearWeight);
  if g_MyHero.m_Abil.WearWeight > g_MyHero.m_Abil.MaxWearWeight then begin
    DHeroStateWearWeightLabel.CaptionColor.Up.Color := clRed;
    DHeroStateWearWeightLabel.CaptionColor.Hot.Color := clRed;
    DHeroStateWearWeightLabel.CaptionColor.Down.Color := clRed;
  end
  else begin
    DHeroStateWearWeightLabel.CaptionColor.Up.Color := clSilver;
    DHeroStateWearWeightLabel.CaptionColor.Hot.Color := clSilver;
    DHeroStateWearWeightLabel.CaptionColor.Down.Color := clSilver;
  end;

  DHeroStateHandWeightLabel.Caption := FLabelCaption_HeroHandWeight + ' ' + IntToStr(g_MyHero.m_Abil.HandWeight) + '/' + IntToStr(g_MyHero.m_Abil.MaxHandWeight);
  if g_MyHero.m_Abil.HandWeight > g_MyHero.m_Abil.MaxHandWeight then begin
    DHeroStateHandWeightLabel.CaptionColor.Up.Color := clRed;
    DHeroStateHandWeightLabel.CaptionColor.Hot.Color := clRed;
    DHeroStateHandWeightLabel.CaptionColor.Down.Color := clRed;
  end
  else begin
    DHeroStateHandWeightLabel.CaptionColor.Up.Color := clSilver;
    DHeroStateHandWeightLabel.CaptionColor.Hot.Color := clSilver;
    DHeroStateHandWeightLabel.CaptionColor.Down.Color := clSilver;
  end;

  DHeroStateAntiMagicLabel.Caption := FLabelCaption_HeroAntiMagic + ' +' + IntToStr(g_nHeroAntiMagic * 10) + '%';
  DHeroStateAntiPoisonLabel.Caption := FLabelCaption_HeroAntiPoison + ' +' + IntToStr(g_nHeroAntiPoison * 10) + '%';

  DHeroStatePoisonRecoverLabel.Caption := FLabelCaption_HeroPoisonRecover + ' +' + IntToStr(g_nHeroPoisonRecover * 10) + '%';
  DHeroStateHealthRecoverLabel.Caption := FLabelCaption_HeroHealthRecover + ' +' + IntToStr(g_nHeroHealthRecover * 10) + '%';
  DHeroStateSpellRecoverLabel.Caption := FLabelCaption_HeroSpellRecover + ' +' + IntToStr(g_nHeroSpellRecover * 10) + '%';

  // -----------------------------------------------------------------------------
  DHeroStateNGLevelValue.Caption := IntToStr(g_MyHero.m_AbilNG.Level);
  DHeroStateNGExpValue.Caption := IntToStr(g_MyHero.m_AbilNG.Exp);
  DHeroStateNGMaxExpValue.Caption := IntToStr(g_MyHero.m_AbilNG.MaxExp);
  DHeroStateNGValue.Caption := IntToStr(g_MyHero.m_AbilNG.NH) + '/' + IntToStr(g_MyHero.m_AbilNG.MaxNH);
  DHeroStateNGRecoverValue.Caption := IntToStr(g_nHeroNPRecoverTime);
  DHeroStateNGHitValue.Caption := IntToStr(g_MyHero.m_AbilNG.NGDamage); // + IntToStr(g_MySelf.m_AbilNG.Level);
  DHeroStateNGAntiPoisonValue.Caption := IntToStr(g_MyHero.m_AbilNG.UnNGDamage); // + IntToStr(g_MySelf.m_AbilNG.Level);
  DHeroStateHJAntiPoisonValue.Caption := ''; // '合击伤害减免    ';                                        // + IntToStr(g_MySelf.m_AbilNG.Level);

  // -----------------------------------------------------------------------------

    {
    DHeroStateNewAbilLabel1.Caption := Format('暴击几率 +%d', [g_MyHero.m_Abil.NewValue[0]]) + '%';
    DHeroStateNewAbilLabel2.Caption := Format('攻击伤害 +%d', [g_MyHero.m_Abil.NewValue[1]]) + '%';
    DHeroStateNewAbilLabel3.Caption := Format('伤害吸收 +%d', [g_MyHero.m_Abil.NewValue[2]]) + '%';
    DHeroStateNewAbilLabel4.Caption := Format('魔法防御 +%d', [g_MyHero.m_Abil.NewValue[3]]) + '%';
    DHeroStateNewAbilLabel5.Caption := Format('忽视防御 +%d', [g_MyHero.m_Abil.NewValue[4]]) + '%';
    DHeroStateNewAbilLabel6.Caption := Format('伤害反弹 +%d', [g_MyHero.m_Abil.NewValue[5]]) + '%';
    DHeroStateNewAbilLabel7.Caption := Format('体力增加 +%d', [g_MyHero.m_Abil.NewValue[7]]) + '%';
    DHeroStateNewAbilLabel8.Caption := Format('魔力增加 +%d', [g_MyHero.m_Abil.NewValue[8]]) + '%';
    DHeroStateNewAbilLabel9.Caption := Format('怒气恢复 +%d', [g_MyHero.m_Abil.NewValue[9]]) + '%';
    DHeroStateNewAbilLabel10.Caption := Format('合击伤害 +%d', [g_MyHero.m_Abil.NewValue[10]]) + '%';
    DHeroStateNewAbilLabel11.Caption := Format('目标暴率 +%d', [g_MyHero.m_Abil.NewValue[6]]) + '%';
    }

  DHeroStateNewAbilLabel1.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel1.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel1.Tag]]);
  DHeroStateNewAbilLabel2.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel2.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel2.Tag]]);
  DHeroStateNewAbilLabel3.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel3.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel3.Tag]]);
  DHeroStateNewAbilLabel4.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel4.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel4.Tag]]);
  DHeroStateNewAbilLabel5.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel5.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel5.Tag]]);
  DHeroStateNewAbilLabel6.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel6.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel6.Tag]]);
  DHeroStateNewAbilLabel7.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel7.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel7.Tag]]);
  DHeroStateNewAbilLabel8.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel8.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel8.Tag]]);
  DHeroStateNewAbilLabel9.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel9.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel9.Tag]]);
  DHeroStateNewAbilLabel10.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel10.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel10.Tag]]);
  DHeroStateNewAbilLabel11.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel11.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel11.Tag]]);
  DHeroStateNewAbilLabel12.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel12.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel12.Tag]]);
  DHeroStateNewAbilLabel13.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel13.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel13.Tag]]);
  DHeroStateNewAbilLabel14.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel14.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel14.Tag]]);
  DHeroStateNewAbilLabel15.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel15.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel15.Tag]]);
  DHeroStateNewAbilLabel16.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[DHeroStateNewAbilLabel16.Tag], g_MyHero.m_Abil.NewValue[DHeroStateNewAbilLabel16.Tag]]);

  {
  DHeroStateNewAbilLabel1.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[0], g_MyHero.m_Abil.NewValue[0]]);
  DHeroStateNewAbilLabel2.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[1], g_MyHero.m_Abil.NewValue[1]]);
  DHeroStateNewAbilLabel3.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[2], g_MyHero.m_Abil.NewValue[2]]);
  DHeroStateNewAbilLabel4.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[3], g_MyHero.m_Abil.NewValue[3]]);
  DHeroStateNewAbilLabel5.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[4], g_MyHero.m_Abil.NewValue[4]]);
  DHeroStateNewAbilLabel6.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[5], g_MyHero.m_Abil.NewValue[5]]);
  DHeroStateNewAbilLabel7.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[7], g_MyHero.m_Abil.NewValue[7]]);
  DHeroStateNewAbilLabel8.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[8], g_MyHero.m_Abil.NewValue[8]]);
  DHeroStateNewAbilLabel9.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[9], g_MyHero.m_Abil.NewValue[9]]);
  DHeroStateNewAbilLabel10.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[10], g_MyHero.m_Abil.NewValue[10]]);
  DHeroStateNewAbilLabel11.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[6], g_MyHero.m_Abil.NewValue[6]]);
  DHeroStateNewAbilLabel12.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[11], g_MyHero.m_Abil.NewValue[11]]);
  DHeroStateNewAbilLabel13.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[13], g_MyHero.m_Abil.NewValue[13]]);
  DHeroStateNewAbilLabel14.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[18], g_MyHero.m_Abil.NewValue[18]]);
  DHeroStateNewAbilLabel15.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[19], g_MyHero.m_Abil.NewValue[19]]);
  DHeroStateNewAbilLabel16.Caption := Format('%s +%d%%', [g_sElementNewPropertyTexts[16], g_MyHero.m_Abil.NewValue[16]]);
  }

  {nPage := DHeroMeridiansPageControl.ActivePageIndex - 1;
  if nPage < 0 then nPage := 4;

  if g_MyHero.m_HumMeridians[nPage].Acupoints[4] > 0 then
    sCaption := '经' + #13 + '络' + #13 + '已' + #13 + '通'
  else
    sCaption := '经' + #13 + '络' + #13 + '未' + #13 + '通';

  case DHeroMeridiansPageControl.ActivePageIndex of
    0: ;
    1: DHeroLabelMeridiansStatus1.Caption := sCaption;
    2: DHeroLabelMeridiansStatus2.Caption := sCaption;
    3: DHeroLabelMeridiansStatus3.Caption := sCaption;
    4: DHeroLabelMeridiansStatus4.Caption := sCaption;
  end;
        }
  for I := 0 to Length(g_MyHero.m_HumMeridians) - 1 do begin
    SetHeroMeridiansLevel(I, g_MyHero.m_HumMeridians[I].Level);
  end;

  ChangeMyHeroMeridiansctivePage;

  DHeroLabelMeridiansStatus1.Caption := GetMeridianStateInfo(0, g_MyHero.m_HumMeridians);
  DHeroLabelMeridiansStatus2.Caption := GetMeridianStateInfo(1, g_MyHero.m_HumMeridians);
  DHeroLabelMeridiansStatus3.Caption := GetMeridianStateInfo(2, g_MyHero.m_HumMeridians);
  DHeroLabelMeridiansStatus4.Caption := GetMeridianStateInfo(3, g_MyHero.m_HumMeridians);
end;

procedure TStateWindows.OpenHeroStateWinDlg;
begin
  if not Initialized then Exit;
  if g_MyHero = nil then Exit;

  DHeroStateWin.Visible := not DHeroStateWin.Visible;
  if DHeroStateWin.Visible then begin
    SetMyHeroActivePageIndexCount;
  end;
end;

procedure TStateWindows.CloseHeroStateWinDlg;
begin
  if not Initialized then Exit;
  DHeroStateWin.Visible := False;
  FengHaoHintWindow.Clear;
end;

procedure TStateWindows.DStMag1DirectPaint(Sender:TObject);
var
  DxButton:TDxImageButton;

  Magic:TClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;
  sKey:string;

  nIcoIndex:Integer;
  Icon:TGameImages;
begin
  if g_MySelf = nil then Exit;

  Magic.Def.wMagicId := 0;

  DxButton := Sender as TDxImageButton;
  g_MagicList.Lock;
  try
    if (DxButton.Tag + MagicIndex >= 0) and (DxButton.Tag + MagicIndex < g_MagicList.Count) then begin
      Magic := PTClientMagic(g_MagicList.Items[DxButton.Tag + MagicIndex])^;
    end;
  finally
    g_MagicList.UnLock;
  end;

  if Magic.Def.wMagicId = 0 then Exit;

  vbRect := DxButton.VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := DxButton.VirtualRect;

  nIcoIndex := GetNewMagicLevelIconOffset(@Magic, Icon);

  { TODO -opiaoyun -c技能 : 技能图标扩展一个g_WMagIcon2Images 2013-06-24 }
  if DxButton.MouseDowned then begin
    if Icon = nil then begin
      if nIcoIndex < 2000 then
        d := g_WMagIconImages.Images[nIcoIndex + 1]
      else
        d := g_WMagIcon2Images.Images[nIcoIndex - 2000 + 1];
    end
    else
      d := Icon.Images[nIcoIndex + 1];
  end
  else begin
    if Icon = nil then begin
      if nIcoIndex < 2000 then
        d := g_WMagIconImages.Images[nIcoIndex]
      else
        d := g_WMagIcon2Images.Images[nIcoIndex - 2000];
    end
    else
      d := Icon.Images[nIcoIndex];
  end;

  if d <> nil then begin
    GameCanvas.Draw(vtRect.Left, vtRect.Top, d);

    // 技能冷却时间 chongchong 2016-10-10
    if (Magic.dwInterval > 0) and (MyGetTickCount > Magic.dwLastUseTick) then begin
      if (MyGetTickCount <= Magic.dwLastUseTick + Magic.dwInterval) then begin
        nIcoIndex := Round(29 / ((Magic.dwInterval) / (MyGetTickCount - Magic.dwLastUseTick)));

        D := g_WNewopUIImages.Images[1380 + nIcoIndex];

        if D <> nil then begin
          GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
        end;
      end;
    end;

    // 红色遮罩效果（技能使用间隔) chongchong 2016-10-10
    if g_boLatestSpell and ((MyGetTickCount - g_dwLatestSpellTick) < g_dwMagicDelayTime) then begin
      nIcoIndex := Round(29 / ((g_dwMagicDelayTime) / (MyGetTickCount - g_dwLatestSpellTick)));
      D := g_WNewopUIImages.Images[1410 + nIcoIndex];

      if D <> nil then begin
        GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
      end;
    end;
  end;

  case Magic.Key of
    'E':sKey := 'C+F1';
    'F':sKey := 'C+F2';
    'G':sKey := 'C+F3';
    'H':sKey := 'C+F4';
    'I':sKey := 'C+F5';
    'J':sKey := 'C+F6';
    'K':sKey := 'C+F7';
    'L':sKey := 'C+F8';

    '1':sKey := 'F1';
    '2':sKey := 'F2';
    '3':sKey := 'F3';
    '4':sKey := 'F4';
    '5':sKey := 'F5';
    '6':sKey := 'F6';
    '7':sKey := 'F7';
    '8':sKey := 'F8';
    else
      if Magic.Key in ['a'..'z'] then
        sKey := Chr(Ord(Magic.Key) - 32)
      else
        sKey := '';
  end;

  if sKey <> '' then
    BoldTextOut(vtRect.Left + (DxButton.Width - CurrentFont.TextWidth(sKey)), vtRect.Top + (DxButton.Height - g_CurrentFontHeight), sKey, clLime); // 技能按键
end;

procedure TStateWindows.DStateMagicDirectPaint(Sender:TObject);
var
  Magic:TClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;
  DxForm:TDxImageForm;
  HGEFont:THGEFont;
begin
  if g_MySelf = nil then Exit;

  DxForm := Sender as TDxImageForm;

  Magic.Def.wMagicId := 0;
  g_MagicList.Lock;
  try
    if (DxForm.Tag + MagicIndex >= 0) and (DxForm.Tag + MagicIndex < g_MagicList.Count) then begin
      Magic := PTClientMagic(g_MagicList.Items[DxForm.Tag + MagicIndex])^;
    end;
  finally
    g_MagicList.UnLock;
  end;

  if Magic.Def.wMagicId = 0 then Exit;

  vbRect := DxForm.VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := DxForm.VirtualRect;

  {
  DStMagLblName1.Visible := False;
  DStMagLvIcon1.Visible := False;
  DStMagLvText1.Visible := False;
  DStMagExpIcon1.Visible := False;
  DStMagExpText1.Visible := False;
  }

  // 技能名称
  HGEFont := TextureFonts.FindFont(DStMagLblName1.CaptionColor.Up.Name, DStMagLblName1.CaptionColor.Up.Size, DStMagLblName1.CaptionColor.Up.Style);
  if HGEFont = nil then
    HGEFont := CurrentFont;
  HGEFont.TextOut(vtRect.Left + DStMagLblName1.Left, vtRect.Top + DStMagLblName1.Top, Magic.Def.sMagicName, DStMagLblName1.CaptionColor.Up.Color);

  if Magic.NewLevel <= 0 then begin
    // Level 等级
    // d := g_WMainImages.Images[112];
    d := DStMagLvIcon1.ImageIndex.Image.Images[DStMagLvIcon1.ImageIndex.Up];
    if d <> nil then begin
      GameCanvas.Draw(vtRect.Left + DStMagLvIcon1.Left, vtRect.Top + DStMagLvIcon1.Top, d.ClientRect, d);
    end;

    HGEFont := TextureFonts.FindFont(DStMagLvText1.CaptionColor.Up.Name, DStMagLvText1.CaptionColor.Up.Size, DStMagLvText1.CaptionColor.Up.Style);
    if HGEFont = nil then
      HGEFont := CurrentFont;
    HGEFont.TextOut(vtRect.Left + DStMagLvText1.Left, vtRect.Top + DStMagLvText1.Top, IntToStr(Magic.Level), DStMagLvText1.CaptionColor.Up.Color);

    // exp 升级经验
    //d := g_WMainImages.Images[111];
    d := DStMagExpIcon1.ImageIndex.Image.Images[DStMagExpIcon1.ImageIndex.Up];
    if d <> nil then begin
      GameCanvas.Draw(vtRect.Left + DStMagExpIcon1.Left, vtRect.Top + DStMagExpIcon1.Top, d.ClientRect, d);
    end;

    HGEFont := TextureFonts.FindFont(DStMagExpText1.CaptionColor.Up.Name, DStMagExpText1.CaptionColor.Up.Size, DStMagExpText1.CaptionColor.Up.Style);
    if HGEFont = nil then
      HGEFont := CurrentFont;
    if Magic.Level < Magic.Def.btTrainLv then
      HGEFont.TextOut(vtRect.Left + DStMagExpText1.Left, vtRect.Top + DStMagExpText1.Top, IntToStr(Magic.CurTrain) + '/' + IntToStr(Magic.Def.MaxTrain[Magic.Level]), DStMagExpText1.CaptionColor.Up.Color)
    else
      HGEFont.TextOut(vtRect.Left + DStMagExpText1.Left, vtRect.Top + DStMagExpText1.Top, '-', DStMagExpText1.CaptionColor.Up.Color);
  end
  else begin
    HGEFont := TextureFonts.FindFont(DStMagLvText1.CaptionColor.Up.Name, DStMagLvText1.CaptionColor.Up.Size, DStMagLvText1.CaptionColor.Up.Style);
    if HGEFont = nil then
      HGEFont := CurrentFont;

    HGEFont.TextOut(vtRect.Left + DStMagLvIcon1.Left, vtRect.Top + DStMagLvIcon1.Top, GetNewMagicLevelString(Magic.NewLevel), DStMagLvText1.CaptionColor.Up.Color); // 强化技能
  end;
end;

procedure TStateWindows.DStMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
var
  I, Index:Integer;
  DxButton:TDxImageButton;
  CtrlRect, vtRect:TRect;
  pm:TClientMagic;
  SkillDescList:TStrings;
  HintLines:THintLines;
  HintWindow:THintWindow;

  D:TDxControl;
  nX, nY:Integer;
begin
  if (not g_ConfigClient.boDisableDrogMagicIcon) and g_boMagicIconDown and ((Abs(g_ptMagicIconDownPt.X - X) >= 3) or (Abs(g_ptMagicIconDownPt.Y - Y) >= 3)) then begin
    g_boMagicMoving := True;
  end;

  DScreen.ClearHint;
  HintWindows.Clear;
  DxButton := TDxImageButton(Sender);

  pm.Def.wMagicId := 0;

  Index := DxButton.Tag + MagicIndex;
  g_MagicList.Lock;
  try
    if (Index >= 0) and (Index < g_MagicList.Count) then begin
      pm := PTClientMagic(g_MagicList.Items[Index])^;
    end;
  finally
    g_MagicList.UnLock;
  end;

  if pm.Def.wMagicId = 0 then Exit;

  SkillDescList := nil;
  HintWindow := nil;

  GetSkillDesc(pm.Def.sMagicName, SkillDescList);
  if SkillDescList <> nil then begin
    HintWindow := THintWindow.Create;
    for I := 0 to SkillDescList.Count - 1 do begin
      HintLines := THintLines.Create;
      HintLines.Add(SkillDescList.Strings[I], TColor(SkillDescList.Objects[I]), GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
      HintWindow.Add(HintLines);
    end;

    if (pm.dwInterval > 0) and (not (g_ConfigClient.boDisableShowFireHitCDTime and (pm.Def.wMagicId in [26, 27]))) {烈火/野蛮不提示 2020-11-02} then begin
      HintLines := THintLines.Create;
      HintLines.Add('-', clRed);
      HintWindow.Add(HintLines);

      HintLines := THintLines.Create;
      HintLines.Add('冷却间隔: ' + IntToStr(pm.dwInterval) + '毫秒', clLime, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
      HintWindow.Add(HintLines);
    end;
  end
  else if (pm.dwInterval > 0) and (not (g_ConfigClient.boDisableShowFireHitCDTime and (pm.Def.wMagicId in [26, 27]))) {烈火/野蛮不提示 2020-11-02} then begin
    HintWindow := THintWindow.Create;

    HintLines := THintLines.Create;
    HintLines.Add('冷却间隔: ' + IntToStr(pm.dwInterval) + '毫秒', clLime, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
    HintWindow.Add(HintLines);
  end;

  if HintWindow = nil then Exit;

  // 2019-08-17 15:00:04

  D := TDxControl(Sender);

  while True do begin
    if (D.Owner <> nil) and (D.Owner is TDxControlEngine) then
      break;
    D := D.Owner;
  end;

  CtrlRect := TDxControl(Sender).VirtualRect;

  if g_ClientConfig.boHintWithMouse then begin
    CtrlRect := TDxControl(Sender).VirtualRect;
    vtRect := D.VirtualRect;
    nY := CtrlRect.Bottom;
  end
  else begin
    vtRect := D.VirtualRect;
    nY := vtRect.Top + 40; // + 40;
  end;

  HintWindow.Show(0, 0);

  if g_ClientConfig.boHintWithMouse then begin
    if CtrlRect.Left >= (SCREENWIDTH - HintWindow.Width) then begin
      nX := CtrlRect.Left; // HintWindow.Width;
    end
    else begin
      nX := CtrlRect.Right;
    end; // HintWindow.Width;
  end
  else begin
    if vtRect.Left >= (SCREENWIDTH - D.Width) div 2 then begin
      nX := vtRect.Left - HintWindow.Width; // HintWindow.Width;
    end
    else begin
      nX := vtRect.Right;
    end;
  end;

  if nX + HintWindow.Width > SCREENWIDTH then
    nX := SCREENWIDTH - HintWindow.Width;

  if nY + HintWindow.Height > SCREENHEIGHT then
    nY := SCREENHEIGHT - HintWindow.Height;

  if nX < 0 then nX := 0;
  if nY < 0 then nY := 0;

  HintWindow.X := nX;
  HintWindow.Y := nY;
  HintWindows.Add(HintWindow);

end;

procedure TStateWindows.DStMag1Click(Sender:TObject; X, Y:Integer);
var
  I:Integer;

  Magic:PTClientMagic;

  ArrDelKey:array of Word;
  MagicID:Word;
  MagicName:string;
  OldMagicKeyChr, MagicKeyChr:Char;
  Icon:TGameImages;
  IconIndex:Integer;

  Shift:TShiftState;
  TempKey:Word;
  IsChange:Boolean;
begin
  MagicID := 0;
  OldMagicKeyChr := #0;
  IconIndex := 0;
  g_MagicList.Lock;
  try
    if (TDxImageButton(Sender).Tag + MagicIndex >= 0) and (TDxImageButton(Sender).Tag + MagicIndex < g_MagicList.Count) then begin
      Magic := g_MagicList.Items[TDxImageButton(Sender).Tag + MagicIndex];

      MagicID := Magic.Def.wMagicId;
      MagicName := Magic.Def.sMagicName;
      MagicKeyChr := Magic.Key;
      OldMagicKeyChr := MagicKeyChr;
      IconIndex := GetNewMagicLevelIconOffset(Magic, Icon);
    end;
  finally
    g_MagicList.UnLock;
  end;

  if MagicID > 0 then begin
    FrmDlg.SetMagicKeyDlg(MagicName, Icon, IconIndex, MagicKeyChr);

    if MagicKeyChr <> #0 then begin
      for I := 0 to g_MagicList.Count - 1 do begin
        Magic := PTClientMagic(g_MagicList[I]);

        if (Magic.Def.wMagicId <> MagicID) then begin
          if (Magic.Key = MagicKeyChr) then begin
            Magic.Key := #0;
            SetLength(ArrDelKey, Length(ArrDelKey) + 1);
            ArrDelKey[Length(ArrDelKey) - 1] := Magic.Def.wMagicId;
          end;
        end else if OldMagicKeyChr <> MagicKeyChr then begin
          Magic.Key := MagicKeyChr;
        end;
      end;

      // 取消掉相同的自定义快捷键 2019-11-19 23:56:33
      if MagicKeyChr in ['1'..'8'] then begin
        Shift := [];
        TempKey := VK_F1 + Ord(MagicKeyChr) - Ord('1');
      end else if MagicKeyChr in ['E'..'L'] then begin
        Shift := [ssCtrl];
        TempKey := VK_F1 + Ord(MagicKeyChr) - Ord('E');
      end else if (Ord(MagicKeyChr) >= Ord('a')) and (Ord(MagicKeyChr) <= Ord('z')) then begin
        Shift := [];
        TempKey := Ord('A') + (Ord(MagicKeyChr) - Ord('a'));
      end else begin
        TempKey := 0; //HZQ 20230524
      end;

      IsChange := False;
      for I := 0 to Length(g_ShortcutKeys) - 1 do begin
        if (g_ShortcutKeys[I].Key = TempKey) and (g_ShortcutKeys[I].Shift = Shift) then begin
          g_ShortcutKeys[I].Use := False;
          g_ShortcutKeys[I].Key := 0;
          g_ShortcutKeys[I].Shift := [];
          IsChange := True;
        end;
      end;

      if IsChange then
        g_ConfigDlg.RefKeyboardConfig;
      // 取消掉相同的自定义快捷键 2019-11-19 23:56:33

    end else if OldMagicKeyChr <> MagicKeyChr then begin
      for I := 0 to g_MagicList.Count - 1 do begin
        Magic := PTClientMagic(g_MagicList[I]);

        if (Magic.Def.wMagicId = MagicID) then begin
          Magic.Key := MagicKeyChr;
          Break;
        end;
      end;
    end;

    for I := 0 to Length(ArrDelKey) - 1 do begin
      frmMain.SendMagicKeyChange(ArrDelKey[I], #0);
      FrmDlg.DelScreenMagicButton(ArrDelKey[I]);
    end;

    if OldMagicKeyChr <> MagicKeyChr then begin
      frmMain.SendMagicKeyChange(MagicId, MagicKeyChr);
    end;
  end;

  g_boMagicMoving := False;
  g_boMagicIconDown := False;
end;

procedure TStateWindows.DStMag1MouseDown(Sender:TObject; Button:TMouseButton;
  Shift:TShiftState; X, Y:Integer);
var
  selkey:Word;
  SelMagic:PTClientMagic;
begin
  g_MagicList.Lock;
  try
    if (TDxImageButton(Sender).Tag + MagicIndex >= 0) and (TDxImageButton(Sender).Tag + MagicIndex < g_MagicList.Count) then begin
      SelMagic := g_MagicList.Items[TDxImageButton(Sender).Tag + MagicIndex];
      selkey := Word(SelMagic.Key);
    end else begin
      SelMagic := nil;
      selkey := 0; //HZQ 20230524
    end;
  finally
    g_MagicList.UnLock;
  end;

  if (not g_ConfigClient.boDisableDrogMagicIcon)
    and (SelMagic <> nil) and (selKey <> 0) {and (FrmDlg.FindMagicButton(SelMagic) = nil)} then begin
    g_boMagicMoving := False;
    g_boMagicIconDown := True;
    g_ptMagicIconDownPt := Point(X, Y);
    g_MovingMagic := SelMagic;
  end;
end;

procedure TStateWindows.DStMag1MouseUp(Sender:TObject; Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  g_boMagicMoving := False;
  g_boMagicIconDown := False;
  g_MovingMagic := nil;
end;

procedure TStateWindows.MagicPageChange;
var
  nPage, nPageCount:Integer;
  CountOnPage:Integer;
begin
  // 176界面技能分页不对 chongchong 2013-11-08
  if g_ClientConfig.boUseOldSerialWindows and (g_ClientConfig.boStateWindowsType = 0) then
    CountOnPage := 5
  else
    CountOnPage := 6;

  g_MagicList.Lock;
  try
    if g_MagicList.Count > 0 then begin
      nPage := (MagicIndex + 1) div CountOnPage + 1;
      nPageCount := g_MagicList.Count div CountOnPage;
      if g_MagicList.Count mod CountOnPage > 0 then Inc(nPageCount);
      if nPageCount <= 0 then nPageCount := 1;

      { TODO -ochongchong -c修改 : 第二页只有一个魔法，删除魔法后，不重定位到第一页 【2013-09-04】 }
      if nPage > nPageCount then begin
        nPage := nPageCount;

        if MagicIndex > 0 then Dec(MagicIndex, CountOnPage);
        if MagicIndex < 0 then MagicIndex := 0;
      end;
    end else begin
      nPage := 0;
      nPageCount := 0;
    end;

    RefreshUpgradeButtons;
  finally
    g_MagicList.UnLock;
  end;

  DStMagicPage.Caption := Format('%d/%d', [nPage, nPageCount]);
end;

procedure TStateWindows.DHeroStMag1DirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;

  IconIndex:Integer;
  Icon:TGameImages;
begin
  if g_MyHero = nil then Exit;
  g_HeroMagicList.Lock;
  try
    with Sender as TDxImageButton do begin
      if (Tag + HeroMagicIndex >= 0) and (Tag + HeroMagicIndex < g_HeroMagicList.Count) then begin
        Magic := g_HeroMagicList.Items[Tag + HeroMagicIndex];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          IconIndex := GetNewMagicLevelIconOffset(Magic, Icon);
          if Byte(Magic.Key) = VK_F1 then begin
            if MouseDowned then begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Images[IconIndex + 1]
                else
                  d := g_WMagIcon2Images.Images[IconIndex - 2000 + 1];
              end
              else
                d := Icon.Images[IconIndex + 1];
            end
            else begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Images[IconIndex]
                else
                  d := g_WMagIcon2Images.Images[IconIndex - 2000];
              end
              else
                d := Icon.Images[IconIndex];
            end;
          end
          else begin
            if MouseDowned then begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Grays[IconIndex + 1]
                else
                  d := g_WMagIcon2Images.Grays[IconIndex - 2000 + 1];
              end
              else
                d := Icon.Grays[IconIndex + 1];
            end
            else begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Grays[IconIndex]
                else
                  d := g_WMagIcon2Images.Grays[IconIndex - 2000];
              end
              else
                d := Icon.Grays[IconIndex];
            end;
          end;

          if d <> nil then
            GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
        end;
      end;
    end;
  finally
    g_HeroMagicList.UnLock;
  end;
end;

procedure TStateWindows.DHeroStateMagicDirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;
  sMagicName:string;
  Color:TColor;
  HGEFont:THGEFont;
begin
  if g_MyHero = nil then Exit;
  g_HeroMagicList.Lock;
  try
    with Sender as TDxImageForm do begin
      if (Tag + HeroMagicIndex >= 0) and (Tag + HeroMagicIndex < g_HeroMagicList.Count) then begin
        Magic := g_HeroMagicList.Items[Tag + HeroMagicIndex];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          // MagicName
          if Byte(Magic.Key) = VK_F1 then begin // Byte('1')
            Color := DStHeroMagLblName1.CaptionColor.Up.Color;
            sMagicName := Magic.Def.sMagicName;
          end
          else begin
            Color := DStHeroMagLblName1.CaptionColor.Disabled.Color;
            sMagicName := Magic.Def.sMagicName + '[关闭]';
          end;

          // 技能名称
          HGEFont := TextureFonts.FindFont(DStHeroMagLblName1.CaptionColor.Up.Name, DStHeroMagLblName1.CaptionColor.Up.Size, DStHeroMagLblName1.CaptionColor.Up.Style);
          if HGEFont = nil then
            HGEFont := CurrentFont;
          HGEFont.TextOut(vtRect.Left + DStHeroMagLblName1.Left, vtRect.Top + DStHeroMagLblName1.Top, sMagicName, Color);

          if Magic.NewLevel <= 0 then begin
            // Level 等级
            // d := g_WMainImages.Images[112];
            d := DStHeroMagLvIcon1.ImageIndex.Image.Images[DStHeroMagLvIcon1.ImageIndex.Up];
            if d <> nil then begin
              GameCanvas.Draw(vtRect.Left + DStHeroMagLvIcon1.Left, vtRect.Top + DStHeroMagLvIcon1.Top, d.ClientRect, d);
            end;

            HGEFont := TextureFonts.FindFont(DStHeroMagLvText1.CaptionColor.Up.Name, DStHeroMagLvText1.CaptionColor.Up.Size, DStHeroMagLvText1.CaptionColor.Up.Style);
            if HGEFont = nil then
              HGEFont := CurrentFont;
            HGEFont.TextOut(vtRect.Left + DStHeroMagLvText1.Left, vtRect.Top + DStHeroMagLvText1.Top, IntToStr(Magic.Level), DStHeroMagLvText1.CaptionColor.Up.Color);

            // exp 升级经验
            //d := g_WMainImages.Images[111];
            d := DStHeroMagExpIcon1.ImageIndex.Image.Images[DStHeroMagExpIcon1.ImageIndex.Up];
            if d <> nil then begin
              GameCanvas.Draw(vtRect.Left + DStHeroMagExpIcon1.Left, vtRect.Top + DStHeroMagExpIcon1.Top, d.ClientRect, d);
            end;

            HGEFont := TextureFonts.FindFont(DStHeroMagExpText1.CaptionColor.Up.Name, DStHeroMagExpText1.CaptionColor.Up.Size, DStHeroMagExpText1.CaptionColor.Up.Style);
            if HGEFont = nil then
              HGEFont := CurrentFont;
            if Magic.Level < Magic.Def.btTrainLv then
              HGEFont.TextOut(vtRect.Left + DStHeroMagExpText1.Left, vtRect.Top + DStHeroMagExpText1.Top, IntToStr(Magic.CurTrain) + '/' + IntToStr(Magic.Def.MaxTrain[Magic.Level]), DStHeroMagExpText1.CaptionColor.Up.Color)
            else
              HGEFont.TextOut(vtRect.Left + DStHeroMagExpText1.Left, vtRect.Top + DStHeroMagExpText1.Top, '-', DStHeroMagExpText1.CaptionColor.Up.Color);
          end
          else begin
            HGEFont := TextureFonts.FindFont(DStHeroMagLvText1.CaptionColor.Up.Name, DStHeroMagLvText1.CaptionColor.Up.Size, DStHeroMagLvText1.CaptionColor.Up.Style);
            if HGEFont = nil then
              HGEFont := CurrentFont;

            HGEFont.TextOut(vtRect.Left + DStHeroMagLvIcon1.Left, vtRect.Top + DStHeroMagLvIcon1.Top, GetNewMagicLevelString(Magic.NewLevel), DStHeroMagLvText1.CaptionColor.Up.Color); // 强化技能
          end;
        end;
      end;
    end;
  finally
    g_HeroMagicList.UnLock;
  end;
end;

procedure TStateWindows.DHeroStMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
var
  I, Index:Integer;
  DxButton:TDxImageButton;
  CtrlRect, vtRect:TRect;
  pm:PTClientMagic;
  SkillDescList:TStrings;

  HintWindow:THintWindow;
  HintLines:THintLines;

  D:TDxControl;
  nX, nY:Integer;
begin
  DScreen.ClearHint;
  HintWindows.Clear;
  DxButton := TDxImageButton(Sender);
  g_HeroMagicList.Lock;
  try
    Index := DxButton.Tag + HeroMagicIndex;
    if (Index >= 0) and (Index < g_HeroMagicList.Count) then begin
      pm := PTClientMagic(g_HeroMagicList.Items[Index]);
      SkillDescList := nil;
      GetSkillDesc(pm.Def.sMagicName, SkillDescList);
      if SkillDescList <> nil then begin
        HintWindow := THintWindow.Create;
        for I := 0 to SkillDescList.Count - 1 do begin
          HintLines := THintLines.Create;
          HintLines.Add(SkillDescList.Strings[I], TColor(SkillDescList.Objects[I]), GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
          HintWindow.Add(HintLines);
        end;

        // 2019-08-17 14:59:46
        D := TDxControl(Sender);

        while True do begin
          if (D.Owner <> nil) and (D.Owner is TDxControlEngine) then
            break;
          D := D.Owner;
        end;

        CtrlRect := TDxControl(Sender).VirtualRect;

        if g_ClientConfig.boHintWithMouse then begin
          vtRect := D.VirtualRect;
          nY := CtrlRect.Bottom;
        end
        else begin
          vtRect := D.VirtualRect;
          nY := vtRect.Top + 40; // + 40;
        end;

        HintWindow.Show(0, 0);

        if g_ClientConfig.boHintWithMouse then begin
          if CtrlRect.Left >= (SCREENWIDTH - HintWindow.Width) then begin
            nX := CtrlRect.Left; // HintWindow.Width;
          end
          else begin
            nX := CtrlRect.Right;
          end;
        end
        else begin
          if vtRect.Left >= (SCREENWIDTH - D.Width) div 2 then begin
            nX := vtRect.Left - HintWindow.Width; // HintWindow.Width;
          end
          else begin
            nX := vtRect.Right;
          end;
        end;

        if nX + HintWindow.Width > SCREENWIDTH then
          nX := SCREENWIDTH - HintWindow.Width;

        if nY + HintWindow.Height > SCREENHEIGHT then
          nY := SCREENHEIGHT - HintWindow.Height;

        if nX < 0 then nX := 0;
        if nY < 0 then nY := 0;

        HintWindow.X := nX;
        HintWindow.Y := nY;
        HintWindows.Add(HintWindow);
      end;
    end;
  finally
    g_HeroMagicList.UnLock;
  end;
end;

procedure TStateWindows.DHeroStMag1Click(Sender:TObject; X, Y:Integer);
var
  Magic:PTClientMagic;
begin
  g_HeroMagicList.Lock;
  try
    if (TDxImageButton(Sender).Tag + HeroMagicIndex >= 0) and (TDxImageButton(Sender).Tag + HeroMagicIndex < g_HeroMagicList.Count) then begin
      Magic := g_HeroMagicList.Items[TDxImageButton(Sender).Tag + HeroMagicIndex];
      if Ord(Magic.Key) = VK_F1 then
        Magic.Key := Chr(0)
      else
        Magic.Key := Chr(VK_F1);
      frmMain.SendHeroMagicKeyChange(Magic.Def.wMagicId);
    end;
  finally
    g_HeroMagicList.UnLock;
  end;
end;

procedure TStateWindows.HeroMagicPageChange;
var
  nPage, nPageCount:Integer;
begin
  g_HeroMagicList.Lock;
  try
    if g_HeroMagicList.Count > 0 then begin
      nPage := (HeroMagicIndex + 1) div 6 + 1;
      nPageCount := g_HeroMagicList.Count div 6;
      if g_HeroMagicList.Count mod 6 > 0 then
        Inc(nPageCount);
      if nPageCount <= 0 then nPageCount := 1;

      { TODO -ochongchong -c修改 : 第二页只有一个魔法，删除魔法后，不重定位到第一页 【2013-09-04】 }
      if nPage > nPageCount then begin
        nPage := nPageCount;

        if HeroMagicIndex > 0 then Dec(HeroMagicIndex, 6);
        if HeroMagicIndex < 0 then HeroMagicIndex := 0;
      end;

      RefreshHeroUpgradeButtons;
    end
    else begin
      nPage := 0;
      nPageCount := 0;
    end;
  finally
    g_HeroMagicList.UnLock;
  end;
  DHeroStMagicPage.Caption := Format('%d/%d', [nPage, nPageCount]);
end;

procedure TStateWindows.DHeroStPageUpClick(Sender:TObject; X, Y:Integer);
begin
  g_HeroMagicList.Lock;
  try
    if Sender = DHeroStPageUp then begin
      if HeroMagicIndex > 0 then
        Dec(HeroMagicIndex, 6);
      if HeroMagicIndex < 0 then HeroMagicIndex := 0;
    end
    else begin
      if HeroMagicIndex + 6 < g_HeroMagicList.Count then begin
        Inc(HeroMagicIndex, 6);
      end;
    end;
  finally
    g_HeroMagicList.UnLock;
  end;
  HeroMagicPageChange;
end;

procedure TStateWindows.DStPageUpClick(Sender:TObject; X, Y:Integer);
begin
  g_MagicList.Lock;
  try
    if Sender = DStPageUp then begin
      if MagicIndex > 0 then
        Dec(MagicIndex, 6);
      if MagicIndex < 0 then MagicIndex := 0;
    end
    else begin
      if MagicIndex + 6 < g_MagicList.Count then begin
        Inc(MagicIndex, 6);
      end;
    end;
  finally
    g_MagicList.UnLock;
  end;
  MagicPageChange;
  // showmessage(inttostr(g_MagicList.Count));
end;

procedure TStateWindows.RefreshMySelfMagicList;
begin
  if not Initialized then Exit;
  MagicPageChange;
  NGMagicPageChange;
  RefreshUpgradeButtons;
end;

procedure TStateWindows.RefreshMyHeroMagicList;
begin
  if not Initialized then Exit;
  HeroMagicPageChange;
  HeroNGMagicPageChange;
end;
{------------------------------------------------------------------------------}

procedure TStateWindows.DStateNGMagicDirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;
  sMagicName:string;
  Color:TColor;
  HGEFont:THGEFont;
begin
  if g_MySelf = nil then Exit;
  g_MagicNGList.Lock;
  try
    with Sender as TDxImageForm do begin
      if (Tag + MagicNGIndex >= 0) and (Tag + MagicNGIndex < g_MagicNGList.Count) then begin
        Magic := g_MagicNGList.Items[Tag + MagicNGIndex];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          // MagicName
          if Byte(Magic.Key) = VK_F1 then begin // Byte('1')
            Color := DstNGMagLblName1.CaptionColor.Up.Color;
            sMagicName := Magic.Def.sMagicName;
          end
          else begin
            Color := DstNGMagLblName1.CaptionColor.Disabled.Color;
            sMagicName := Magic.Def.sMagicName + '[关闭]';
          end;

          HGEFont := TextureFonts.FindFont(DstNGMagLblName1.CaptionColor.Up.Name, DstNGMagLblName1.CaptionColor.Up.Size, DstNGMagLblName1.CaptionColor.Up.Style);
          if HGEFont = nil then
            HGEFont := CurrentFont;
          HGEFont.TextOut(vtRect.Left + DstNGMagLblName1.Left, vtRect.Top + DstNGMagLblName1.Top, sMagicName, Color);

          if Magic.NewLevel <= 0 then begin
            // Level
            //d := g_WMainImages.Images[112];
            d := DstNGMagLvIcon1.ImageIndex.Image.Images[DstNGMagLvIcon1.ImageIndex.Up];
            if d <> nil then begin
              GameCanvas.Draw(vtRect.Left + DstNGMagLvIcon1.Left, vtRect.Top + DstNGMagLvIcon1.Top, d.ClientRect, d);
            end;

            if Byte(Magic.Key) = VK_F1 then // Byte('1')
              Color := DstNGMagLvText1.CaptionColor.Up.Color
            else
              Color := DstNGMagLvText1.CaptionColor.Disabled.Color;

            HGEFont := TextureFonts.FindFont(DstNGMagLvText1.CaptionColor.Up.Name, DstNGMagLvText1.CaptionColor.Up.Size, DstNGMagLvText1.CaptionColor.Up.Style);
            if HGEFont = nil then
              HGEFont := CurrentFont;
            HGEFont.TextOut(vtRect.Left + DstNGMagLvText1.Left, vtRect.Top + DstNGMagLvText1.Top, IntToStr(Magic.Level), Color);

            // exp
            //d := g_WMainImages.Images[111];
            d := DstNGMagExpIcon1.ImageIndex.Image.Images[DstNGMagExpIcon1.ImageIndex.Up];
            if d <> nil then begin
              GameCanvas.Draw(vtRect.Left + DstNGMagExpIcon1.Left, vtRect.Top + DstNGMagExpIcon1.Top, d.ClientRect, d);
            end;

            if Byte(Magic.Key) = VK_F1 then // Byte('1')
              Color := DstNGMagExpText1.CaptionColor.Up.Color
            else
              Color := DstNGMagExpText1.CaptionColor.Disabled.Color;

            HGEFont := TextureFonts.FindFont(DstNGMagExpText1.CaptionColor.Up.Name, DstNGMagExpText1.CaptionColor.Up.Size, DstNGMagExpText1.CaptionColor.Up.Style);
            if HGEFont = nil then
              HGEFont := CurrentFont;

            if Magic.Level < Magic.Def.btTrainLv then
              HGEFont.TextOut(vtRect.Left + DstNGMagExpText1.Left, vtRect.Top + DstNGMagExpText1.Top, IntToStr(Magic.CurTrain) + '/' + IntToStr(Magic.Def.MaxTrain[Magic.Level]), Color)
            else
              HGEFont.TextOut(vtRect.Left + DstNGMagExpText1.Left, vtRect.Top + DstNGMagExpText1.Top, '-', Color);
          end
          else begin
            if Byte(Magic.Key) = VK_F1 then // Byte('1')
              Color := DstNGMagLvText1.CaptionColor.Up.Color
            else
              Color := DstNGMagLvText1.CaptionColor.Disabled.Color;

            HGEFont := TextureFonts.FindFont(DstNGMagLvText1.CaptionColor.Up.Name, DstNGMagLvText1.CaptionColor.Up.Size, DstNGMagLvText1.CaptionColor.Up.Style);
            if HGEFont = nil then
              HGEFont := CurrentFont;

            HGEFont.TextOut(vtRect.Left + DstNGMagLvIcon1.Left, vtRect.Top + DstNGMagLvIcon1.Top, GetNewMagicLevelString(Magic.NewLevel), Color); // 强化技能
          end;

        end;
      end;
    end;
  finally
    g_MagicNGList.UnLock;
  end;
end;

procedure TStateWindows.DStNGMag1DirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;

  IconIndex:Integer;
  Icon:TGameImages;
begin
  if g_MySelf = nil then Exit;
  g_MagicNGList.Lock;
  try
    with Sender as TDxImageButton do begin
      if (Tag + MagicNGIndex >= 0) and (Tag + MagicNGIndex < g_MagicNGList.Count) then begin
        Magic := g_MagicNGList.Items[Tag + MagicNGIndex];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          IconIndex := GetNewMagicLevelIconOffset(Magic, Icon);
          if Byte(Magic.Key) = VK_F1 then begin
            if MouseDowned then begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Images[IconIndex + 1]
                else
                  d := g_WMagIcon2Images.Images[IconIndex - 2000 + 1];
              end
              else
                d := Icon.Images[IconIndex + 1];
            end
            else begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Images[IconIndex]
                else
                  d := g_WMagIcon2Images.Images[IconIndex - 2000];
              end
              else
                d := Icon.Images[IconIndex];
            end;
          end
          else begin
            if MouseDowned then begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Grays[IconIndex + 1]
                else
                  d := g_WMagIcon2Images.Grays[IconIndex - 2000 + 1];
              end
              else
                d := Icon.Grays[IconIndex + 1];
            end
            else begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Grays[IconIndex]
                else
                  d := g_WMagIcon2Images.Grays[IconIndex - 2000];
              end
              else
                d := Icon.Images[IconIndex];
            end;
          end;

          if d <> nil then
            GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
        end;
      end;
    end;
  finally
    g_MagicNGList.UnLock;
  end;
end;

procedure TStateWindows.DStNGMag1Click(Sender:TObject; X, Y:Integer);
var
  Magic:PTClientMagic;
begin
  g_MagicNGList.Lock;
  try
    if (TDxImageButton(Sender).Tag + MagicNGIndex >= 0) and (TDxImageButton(Sender).Tag + MagicNGIndex < g_MagicNGList.Count) then begin
      Magic := g_MagicNGList.Items[TDxImageButton(Sender).Tag + MagicNGIndex];
      if Ord(Magic.Key) = VK_F1 then
        Magic.Key := Chr(0)
      else
        Magic.Key := Chr(VK_F1);
      frmMain.SendMagicKeyChange(Magic.Def.wMagicId, Magic.Key, Magic.Def.MagicAttr);
    end;
  finally
    g_MagicNGList.UnLock;
  end;
end;

procedure TStateWindows.DStNGPageUpClick(Sender:TObject; X, Y:Integer);
begin
  g_MagicNGList.Lock;
  try
    if Sender = DStNGPageUp then begin
      if MagicNGIndex > 0 then
        Dec(MagicNGIndex, 6);
      if MagicNGIndex < 0 then MagicNGIndex := 0;
    end
    else begin
      if MagicNGIndex + 6 < g_MagicNGList.Count then begin
        Inc(MagicNGIndex, 6);
      end;
    end;
  finally
    g_MagicNGList.UnLock;
  end;
  NGMagicPageChange;
end;

procedure TStateWindows.DStNGMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
var
  I, Index:Integer;
  DxButton:TDxImageButton;
  vtRect:TRect;
  pm:PTClientMagic;
  SkillDescList:TStrings;

  HintWindow:THintWindow;
  HintLines:THintLines;
begin
  DScreen.ClearHint;
  HintWindows.Clear;
  DxButton := TDxImageButton(Sender);
  g_MagicNGList.Lock;
  try
    Index := DxButton.Tag + MagicNGIndex;
    if (Index >= 0) and (Index < g_MagicNGList.Count) then begin
      pm := PTClientMagic(g_MagicNGList.Items[Index]);
      SkillDescList := nil;
      GetSkillDesc(pm.Def.sMagicName, SkillDescList);
      if SkillDescList <> nil then begin
        HintWindow := THintWindow.Create;
        for I := 0 to SkillDescList.Count - 1 do begin
          HintLines := THintLines.Create;
          HintLines.Add(SkillDescList.Strings[I], TColor(SkillDescList.Objects[I]), GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
          HintWindow.Add(HintLines);
        end;

        if (pm.dwInterval > 0) and (not (g_ConfigClient.boDisableShowFireHitCDTime and (pm.Def.wMagicId in [26, 27]))) {烈火/野蛮不提示 2020-11-02} then begin
          HintLines := THintLines.Create;
          HintLines.Add('-', clRed, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
          HintWindow.Add(HintLines);

          HintLines := THintLines.Create;
          HintLines.Add('冷却间隔: ' + IntToStr(pm.dwInterval) + '毫秒', clLime, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
          HintWindow.Add(HintLines);
        end;

        vtRect := DxButton.VirtualRect;
        HintWindow.Show(vtRect.Left - 100, vtRect.Top);
        HintWindows.Add(HintWindow);
      end
      else if (pm.dwInterval > 0) and (not (g_ConfigClient.boDisableShowFireHitCDTime and (pm.Def.wMagicId in [26, 27]))) {烈火/野蛮不提示 2020-11-02} then begin
        HintWindow := THintWindow.Create;

        HintLines := THintLines.Create;
        HintLines.Add('冷却间隔: ' + IntToStr(pm.dwInterval) + '毫秒', clLime, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
        HintWindow.Add(HintLines);

        vtRect := DxButton.VirtualRect;
        HintWindow.Show(vtRect.Left - 100, vtRect.Top);
        HintWindows.Add(HintWindow);
      end;
    end;
  finally
    g_MagicNGList.UnLock;
  end;
end;

procedure TStateWindows.NGMagicPageChange;
var
  nPage, nPageCount:Integer;
begin
  g_MagicNGList.Lock;
  try
    if g_MagicNGList.Count > 0 then begin
      nPage := (MagicNGIndex + 1) div 6 + 1;
      nPageCount := g_MagicNGList.Count div 6;
      if g_MagicNGList.Count mod 6 > 0 then
        Inc(nPageCount);
      if nPageCount <= 0 then nPageCount := 1;

      { TODO -ochongchong -c修改 : 第二页只有一个魔法，删除魔法后，不重定位到第一页 【2013-09-04】 }
      if nPage > nPageCount then begin
        nPage := nPageCount;

        if MagicNGIndex > 0 then
          Dec(MagicNGIndex, 6);
        if MagicNGIndex < 0 then MagicNGIndex := 0;
      end;
    end
    else begin
      nPage := 0;
      nPageCount := 0;
    end;
  finally
    g_MagicNGList.UnLock;
  end;
  DStNGMagicPage.Caption := Format('%d/%d', [nPage, nPageCount]);
end;

{------------------------------------------------------------------------------}

{ 获取连击技能增加暴击率文字 chongchong 2013-08-19 }

function GetContinuousBlastStormsHitText(Magic:PTClientMagic; IsHero:Boolean = False):string;
var
  I, Value1, Value2:Integer;
begin
  Result := '';
  {
    连击技能说明：
      战士： 100 追心刺     101 三绝杀    102 断岳斩       103 横扫千军
      法师： 104 凤舞祭     105 惊雷爆    106 冰天雪地     107 双龙破
      道士： 108 虎啸诀     109 八卦掌    110 三焰咒       111 万剑归宗
  }
  if (Magic.Def.wMagicId >= 100) and (Magic.Def.wMagicId <= 111) and
    (Magic.Level >= 1) and (Magic.Level <= 5) then begin
    { 服务器配置的爆击 }
    Value1 := g_ClientConfig.SkillContinuousBlastHitRates[Magic.Def.wMagicId - 100][Magic.Level - 1];

    { 对应连击位置的附加爆击率 }
    Value2 := 0;
    if IsHero then begin
      for I := Low(g_HeroContinuousMagicOrder) to High(g_HeroContinuousMagicOrder) do begin
        if g_HeroContinuousMagicOrder[I] = Magic.Def.wMagicId then begin
          // 自定义连击顺序暴击几率 2019-08-07 23:26:18
          {
          case I of
            0: Value2 := 10;
            1: Value2 := 15;
            2: Value2 := 25;
            3: Value2 := 25;
          end;
          }

          Value2 := g_ClientConfig.SkillContinueOrderBlastRates[I];
          Break;
        end;
      end;
    end
    else begin
      for I := Low(g_ContinuousMagicOrder) to High(g_ContinuousMagicOrder) do begin
        if g_ContinuousMagicOrder[I] = Magic.Def.wMagicId then begin
          // 自定义连击顺序暴击几率 2019-08-07 23:26:18
          {
          case I of
            0: Value2 := 10;
            1: Value2 := 15;
            2: Value2 := 25;
            3: Value2 := 25;
          end;
          }

          Value2 := g_ClientConfig.SkillContinueOrderBlastRates[I];

          Break;
        end;
      end;
    end;

    if Value1 > 0 then begin
      Result := IntToStr(Value1) + '%';
      if Value2 > 0 then
        Result := Result + '+' + IntToStr(Value2) + '%暴击'
      else
        Result := Result + '暴击';
    end
    else begin
      if Value2 > 0 then
        Result := IntToStr(Value2) + '%暴击'
    end;
  end;
end;

procedure TStateWindows.DStateLJMagicDirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;
  sMagicName, sStormsHit:string;

  HGEFont:THGEFont;
begin
  if g_MySelf = nil then Exit;
  g_ContinuousMagicList.Lock;
  try
    with Sender as TDxImageForm do begin
      if (Tag >= 0) and (Tag < g_ContinuousMagicList.Count) then begin
        Magic := g_ContinuousMagicList.Items[Tag];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          // MagicName
          sMagicName := Magic.Def.sMagicName;

          { TODO -ochongchong -c新增 : 连击技能显示时增加暴击率 }
          sStormsHit := GetContinuousBlastStormsHitText(Magic);
          if Length(sStormsHit) > 0 then
            sMagicName := sMagicName + ':' + sStormsHit;

          HGEFont := TextureFonts.FindFont(DstLJMagLblName1.CaptionColor.Up.Name, DstLJMagLblName1.CaptionColor.Up.Size, DstLJMagLblName1.CaptionColor.Up.Style);
          if HGEFont = nil then
            HGEFont := CurrentFont;
          HGEFont.TextOut(vtRect.Left + DstLJMagLblName1.Left, vtRect.Top + DstLJMagLblName1.Top, sMagicName, DstLJMagLblName1.CaptionColor.Up.Color);

          // Level
          //d := g_WMainImages.Images[112];
          d := DstLJMagLvIcon1.ImageIndex.Image.Images[DstLJMagLvIcon1.ImageIndex.Up];
          if d <> nil then begin
            GameCanvas.Draw(vtRect.Left + DstLJMagLvIcon1.Left, vtRect.Top + DstLJMagLvIcon1.Top, d.ClientRect, d);
          end;

          HGEFont := TextureFonts.FindFont(DstLJMagLvText1.CaptionColor.Up.Name, DstLJMagLvText1.CaptionColor.Up.Size, DstLJMagLvText1.CaptionColor.Up.Style);
          if HGEFont = nil then
            HGEFont := CurrentFont;
          HGEFont.TextOut(vtRect.Left + DstLJMagLvText1.Left, vtRect.Top + DstLJMagLvText1.Top, IntToStr(Magic.Level), DstLJMagLvText1.CaptionColor.Up.Color);

          // exp
          //d := g_WMainImages.Images[111];
          d := DstLJMagExpIcon1.ImageIndex.Image.Images[DstLJMagExpIcon1.ImageIndex.Up];
          if d <> nil then begin
            GameCanvas.Draw(vtRect.Left + DstLJMagExpIcon1.Left, vtRect.Top + DstLJMagExpIcon1.Top, d.ClientRect, d);
          end;

          // 连击技能不显示经验值 piaoyun 2013-08-16

          HGEFont := TextureFonts.FindFont(DstNGMagExpText1.CaptionColor.Up.Name, DstNGMagExpText1.CaptionColor.Up.Size, DstNGMagExpText1.CaptionColor.Up.Style);
          if HGEFont = nil then
            HGEFont := CurrentFont;
          HGEFont.TextOut(vtRect.Left + DstNGMagExpText1.Left, vtRect.Top + DstNGMagExpText1.Top, '-', DstNGMagExpText1.CaptionColor.Up.Color);
        end;
      end;
    end;
  finally
    g_ContinuousMagicList.UnLock;
  end;
end;

procedure TStateWindows.DStLJMag1DirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  Index:Integer;
  d:TTexture;
begin
  if g_MySelf = nil then Exit;
  g_ContinuousMagicList.Lock;
  try
    with Sender as TDxImageButton do begin
      if (Tag >= 0) and (Tag < g_ContinuousMagicList.Count) then begin
        Magic := g_ContinuousMagicList.Items[Tag];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          case Magic.Def.wMagicId of
            100:Index := 950;
            101:Index := 952;
            102:Index := 956;
            103:Index := 954;
            104:Index := 942;
            105:Index := 946;
            106:Index := 940;
            107:Index := 944;
            108:Index := 934;
            109:Index := 936;
            110:Index := 932;
            111:Index := 930;
            else
              Index := -1;
          end;

          if MouseDowned then begin
            d := g_WMainImages.Images[Index + 1];
          end
          else begin
            d := g_WMainImages.Images[Index];
          end;

          if d <> nil then
            GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
        end;
      end;
    end;
  finally
    g_ContinuousMagicList.UnLock;
  end;
end;

procedure TStateWindows.DStLJMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
var
  I, Index:Integer;
  DxButton:TDxImageButton;
  vtRect:TRect;
  pm:PTClientMagic;
  SkillDescList:TStrings;

  HintWindow:THintWindow;
  HintLines:THintLines;
begin
  DScreen.ClearHint;
  HintWindows.Clear;
  DxButton := TDxImageButton(Sender);
  g_ContinuousMagicList.Lock;
  try
    Index := DxButton.Tag;
    if (Index >= 0) and (Index < g_ContinuousMagicList.Count) then begin
      pm := PTClientMagic(g_ContinuousMagicList.Items[Index]);
      SkillDescList := nil;
      GetSkillDesc(pm.Def.sMagicName, SkillDescList);
      if SkillDescList <> nil then begin
        HintWindow := THintWindow.Create;
        for I := 0 to SkillDescList.Count - 1 do begin
          HintLines := THintLines.Create;
          HintLines.Add(SkillDescList.Strings[I], TColor(SkillDescList.Objects[I]), GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
          HintWindow.Add(HintLines);
        end;
        vtRect := DxButton.VirtualRect;
        HintWindow.Show(vtRect.Left - 100, vtRect.Top);
        HintWindows.Add(HintWindow);
      end;
    end;
  finally
    g_ContinuousMagicList.UnLock;
  end;
end;

procedure TStateWindows.DSMB1DirectPaint(Sender:TObject);
var
  Index:Integer;
  DxButton:TDxImageButton;
  vtRect:TRect;
  d:TTexture;
  HGEFont:THGEFont;
  S:string;
  TextW, TextH:Integer;
  nTop:Integer;
begin
  if g_MySelf = nil then Exit; // 连击顺序     0=空 1=随机

  DxButton := TDxImageButton(Sender);
  vtRect := DxButton.VirtualRect;

  if not DxButton.Enabled then begin
    if DxButton.ImageIndex.Image <> nil then begin
      d := DxButton.ImageIndex.Image.Images[DxButton.ImageIndex.Disabled];

      if d <> nil then
        GameCanvas.Draw(vtRect.Left + (DxButton.Width - d.Width) div 2, vtRect.Top + (DxButton.Height - d.Height) div 2, d);
    end;

    Exit;
  end;

  if DxButton.Tag in [0..3] then begin
    if g_ContinuousMagicOrder[DxButton.Tag] = 0 then begin
      // 自定义连击顺序暴击几率 2019-08-07 23:26:18
      if not DxButton.MouseDowned then begin
        HGEFont := TextureFonts.FindFont(DxButton.CaptionColor.Up.Name, DxButton.CaptionColor.Up.Size, DxButton.CaptionColor.Up.Style);
        if HGEFont = nil then
          HGEFont := CurrentFont;
      end
      else begin
        HGEFont := TextureFonts.FindFont(DxButton.CaptionColor.Down.Name, DxButton.CaptionColor.Down.Size, DxButton.CaptionColor.Down.Style);
        if HGEFont = nil then
          HGEFont := CurrentFont;
      end;

      if HGEFont <> nil then begin
        TextH := HGEFont.TextHeight(S) * 2 + 2;
        nTop := (DxButton.Height - TextH) div 2;

        S := '暴击';
        TextW := HGEFont.TextWidth(S);
        TextH := HGEFont.TextHeight(S);

        if not DxButton.MouseDowned then begin
          if DxButton.CaptionColor.Up.Bold then
            BoldTextOut(HGEFont, vtRect.Left + (DxButton.Width - TextW) div 2, vtRect.Top + nTop, S, DxButton.CaptionColor.Up.Color, DxButton.CaptionColor.Up.BColor)
          else
            HGEFont.TextOut(vtRect.Left + (DxButton.Width - TextW) div 2, vtRect.Top + nTop, S, DxButton.CaptionColor.Up.Color);
        end
        else begin
          if DxButton.CaptionColor.Down.Bold then
            BoldTextOut(HGEFont, vtRect.Left + (DxButton.Width - TextW) div 2 + 1, vtRect.Top + nTop + 1, S, DxButton.CaptionColor.Down.Color, DxButton.CaptionColor.Down.BColor)
          else
            HGEFont.TextOut(vtRect.Left + (DxButton.Width - TextW) div 2 + 1, vtRect.Top + nTop + 1, S, DxButton.CaptionColor.Down.Color);
        end;

        nTop := nTop + TextH + 2;

        S := '+' + IntToStr(g_ClientConfig.SkillContinueOrderBlastRates[DxButton.Tag]) + '%';
        TextW := HGEFont.TextWidth(S);

        if not DxButton.MouseDowned then begin
          if DxButton.CaptionColor.Up.Bold then
            BoldTextOut(HGEFont, vtRect.Left + (DxButton.Width - TextW) div 2, vtRect.Top + nTop, S, DxButton.CaptionColor.Up.Color, DxButton.CaptionColor.Up.BColor)
          else
            HGEFont.TextOut(vtRect.Left + (DxButton.Width - TextW) div 2, vtRect.Top + nTop, S, DxButton.CaptionColor.Up.Color);
        end
        else begin
          if DxButton.CaptionColor.Down.Bold then
            BoldTextOut(HGEFont, vtRect.Left + (DxButton.Width - TextW) div 2 + 1, vtRect.Top + nTop + 1, S, DxButton.CaptionColor.Down.Color, DxButton.CaptionColor.Down.BColor)
          else
            HGEFont.TextOut(vtRect.Left + (DxButton.Width - TextW) div 2 + 1, vtRect.Top + nTop + 1, S, DxButton.CaptionColor.Down.Color);
        end;
      end;
    end
    else if g_ContinuousMagicOrder[DxButton.Tag] = 1 then {// 1=随机} begin
      if DxButton.ImageIndex.Image <> nil then begin
        d := DxButton.ImageIndex.Image.Images[DxButton.ImageIndex.Checked];

        if d <> nil then begin
          if not DxButton.MouseDowned then
            GameCanvas.Draw(vtRect.Left + (DxButton.Width - d.Width) div 2, vtRect.Top + (DxButton.Height - d.Height) div 2, d)
          else
            GameCanvas.Draw(vtRect.Left + (DxButton.Width - d.Width) div 2 + 1, vtRect.Top + (DxButton.Height - d.Height) div 2 + 1, d)
        end;
      end;
    end
    else begin //
      case g_ContinuousMagicOrder[DxButton.Tag] of
        100:Index := 950;
        101:Index := 952;
        102:Index := 956;
        103:Index := 954;
        104:Index := 942;
        105:Index := 946;
        106:Index := 940;
        107:Index := 944;
        108:Index := 934;
        109:Index := 936;
        110:Index := 932;
        111:Index := 930;
        else
          Index := -1;
      end;

      if Index > 0 then begin
        if DxButton.MouseDowned then begin
          d := g_WMainImages.Images[Index + 1];
          GameCanvas.Draw(vtRect.Left + (DxButton.Width - d.Width) div 2, vtRect.Top + (DxButton.Height - d.Height) div 2, d)
        end
        else begin
          d := g_WMainImages.Images[Index];
          GameCanvas.Draw(vtRect.Left + (DxButton.Width - d.Width) div 2, vtRect.Top + (DxButton.Height - d.Height) div 2, d)
        end;
      end;
    end;
  end;
end;

procedure TStateWindows.DSMB1Click(Sender:TObject; X, Y:Integer);
var
  I:Integer;
  vtRect:TRect;
  pm:PTClientMagic;
begin
  if (TDxImageButton(Sender).Tag = 3) and (not g_boOpenLastContinuous) then Exit;
  g_ContinuousMagicList.Lock;
  try
    if g_ContinuousMagicList.Count > 0 then begin
      vtRect := TDxImageButton(Sender).VirtualRect;
      ContinuousMagicMenu.Items.Clear;
      for I := 0 to g_ContinuousMagicList.Count - 1 do begin
        pm := g_ContinuousMagicList.Items[I];
        ContinuousMagicMenu.Items.AddObject(pm.Def.sMagicName, TObject(pm.Def.wMagicId));
      end;
      ContinuousMagicMenu.Items.AddObject('空', TObject(0));
      ContinuousMagicMenu.Items.AddObject('随机', TObject(1));
      ContinuousMagicMenu.Tag := TDxImageButton(Sender).Tag;
      ContinuousMagicMenu.Left := vtRect.Left;
      ContinuousMagicMenu.Top := vtRect.Bottom;
      ContinuousMagicMenu.Show;
    end;
  finally
    g_ContinuousMagicList.UnLock;
  end;
end;

procedure TStateWindows.ContinuousMagicMenuClick(Sender:TObject; X, Y:Integer);
var
  I:Integer;
  ItemMenu:pTDxItemMenu;
begin
  if ContinuousMagicMenu.ItemIndex >= 0 then begin
    ItemMenu := pTDxItemMenu(ContinuousMagicMenu.Items.Objects[ContinuousMagicMenu.ItemIndex]);
    if ContinuousMagicMenu.Tag in [0..3] then begin
      g_ContinuousMagicOrder[ContinuousMagicMenu.Tag] := Integer(ItemMenu.AObject);
      if g_ContinuousMagicOrder[ContinuousMagicMenu.Tag] > 1 then begin
        for I := 0 to Length(g_ContinuousMagicOrder) - 1 do begin
          if (I <> ContinuousMagicMenu.Tag) and (g_ContinuousMagicOrder[I] = g_ContinuousMagicOrder[ContinuousMagicMenu.Tag]) then begin
            g_ContinuousMagicOrder[I] := 0;
          end;
        end;
      end;
      frmMain.SendChangeContinuousMagicOrder(False); // 改变连击顺序
    end;
  end;
end;
{------------------------------------------------------------------------------}

procedure TStateWindows.DHeroStateNGMagicDirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;
  sMagicName:string;
  Color:TColor;
  HGEFont:THGEFont;
begin
  if g_MyHero = nil then Exit;
  g_HeroMagicNGList.Lock;
  try
    with Sender as TDxImageForm do begin
      if (Tag + HeroMagicNGIndex >= 0) and (Tag + HeroMagicNGIndex < g_HeroMagicNGList.Count) then begin
        Magic := g_HeroMagicNGList.Items[Tag + HeroMagicNGIndex];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          // MagicName
          if Byte(Magic.Key) = VK_F1 then begin // Byte('1')
            Color := DHeroStNGLblName1.CaptionColor.Up.Color;
            sMagicName := Magic.Def.sMagicName;
          end
          else begin
            Color := DHeroStNGLblName1.CaptionColor.Disabled.Color;
            sMagicName := Magic.Def.sMagicName + '[关闭]';
          end;

          // 技能名称
          HGEFont := TextureFonts.FindFont(DHeroStNGLblName1.CaptionColor.Up.Name, DHeroStNGLblName1.CaptionColor.Up.Size, DHeroStNGLblName1.CaptionColor.Up.Style);
          if HGEFont = nil then
            HGEFont := CurrentFont;
          HGEFont.TextOut(vtRect.Left + DHeroStNGLblName1.Left, vtRect.Top + DHeroStNGLblName1.Top, sMagicName, Color);

          if Magic.NewLevel <= 0 then begin
            // Level
            // d := g_WMainImages.Images[112];
            d := DHeroStNGLvIcon1.ImageIndex.Image.Images[DHeroStNGLvIcon1.ImageIndex.Up];
            if d <> nil then begin
              GameCanvas.Draw(vtRect.Left + DHeroStNGLvIcon1.Left, vtRect.Top + DHeroStNGLvIcon1.Top, d.ClientRect, d);
            end;

            HGEFont := TextureFonts.FindFont(DHeroStNGLvText1.CaptionColor.Up.Name, DHeroStNGLvText1.CaptionColor.Up.Size, DHeroStNGLvText1.CaptionColor.Up.Style);
            if HGEFont = nil then
              HGEFont := CurrentFont;
            HGEFont.TextOut(vtRect.Left + DHeroStNGLvText1.Left, vtRect.Top + DHeroStNGLvText1.Top, IntToStr(Magic.Level), DHeroStNGLvText1.CaptionColor.Up.Color);

            // exp 升级经验
            //d := g_WMainImages.Images[111];
            d := DHeroStNGExpIcon1.ImageIndex.Image.Images[DHeroStNGExpIcon1.ImageIndex.Up];
            if d <> nil then begin
              GameCanvas.Draw(vtRect.Left + DHeroStNGExpIcon1.Left, vtRect.Top + DHeroStNGExpIcon1.Top, d.ClientRect, d);
            end;

            HGEFont := TextureFonts.FindFont(DHeroStNGExpText1.CaptionColor.Up.Name, DHeroStNGExpText1.CaptionColor.Up.Size, DHeroStNGExpText1.CaptionColor.Up.Style);
            if HGEFont = nil then
              HGEFont := CurrentFont;
            if Magic.Level < Magic.Def.btTrainLv then
              HGEFont.TextOut(vtRect.Left + DHeroStNGExpText1.Left, vtRect.Top + DHeroStNGExpText1.Top, IntToStr(Magic.CurTrain) + '/' + IntToStr(Magic.Def.MaxTrain[Magic.Level]), DHeroStNGExpText1.CaptionColor.Up.Color)
            else
              HGEFont.TextOut(vtRect.Left + DHeroStNGExpText1.Left, vtRect.Top + DHeroStNGExpText1.Top, '-', DHeroStNGExpText1.CaptionColor.Up.Color);
          end
          else begin
            HGEFont := TextureFonts.FindFont(DHeroStNGLvText1.CaptionColor.Up.Name, DHeroStNGLvText1.CaptionColor.Up.Size, DHeroStNGLvText1.CaptionColor.Up.Style);
            if HGEFont = nil then
              HGEFont := CurrentFont;

            HGEFont.TextOut(vtRect.Left + DHeroStNGLvIcon1.Left, vtRect.Top + DHeroStNGLvIcon1.Top, GetNewMagicLevelString(Magic.NewLevel), DHeroStNGLvText1.CaptionColor.Up.Color); // 强化技能
          end;

        end;
      end;
    end;
  finally
    g_HeroMagicNGList.UnLock;
  end;
end;

procedure TStateWindows.DHeroStNGMag1DirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;

  IconIndex:Integer;
  Icon:TGameImages;
begin
  if g_MyHero = nil then Exit;
  g_HeroMagicNGList.Lock;
  try
    with Sender as TDxImageButton do begin
      if (Tag + HeroMagicNGIndex >= 0) and (Tag + HeroMagicNGIndex < g_HeroMagicNGList.Count) then begin
        Magic := g_HeroMagicNGList.Items[Tag + HeroMagicNGIndex];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          IconIndex := GetNewMagicLevelIconOffset(Magic, Icon);

          if Byte(Magic.Key) = VK_F1 then begin
            if MouseDowned then begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Images[IconIndex + 1]
                else
                  d := g_WMagIcon2Images.Images[IconIndex - 2000 + 1];
              end
              else
                d := Icon.Images[IconIndex + 1];
            end
            else begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Images[IconIndex]
                else
                  d := g_WMagIcon2Images.Images[IconIndex - 2000];
              end
              else
                d := Icon.Images[IconIndex];
            end;
          end
          else begin
            if MouseDowned then begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Grays[IconIndex + 1]
                else
                  d := g_WMagIcon2Images.Grays[IconIndex - 2000 + 1];
              end
              else
                d := Icon.Grays[IconIndex + 1];
            end
            else begin
              if Icon = nil then begin
                if IconIndex < 2000 then
                  d := g_WMagIconImages.Grays[IconIndex]
                else
                  d := g_WMagIcon2Images.Grays[IconIndex - 2000];
              end
              else
                d := Icon.Grays[IconIndex];
            end;
          end;

          if d <> nil then
            GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
        end;
      end;
    end;
  finally
    g_HeroMagicNGList.UnLock;
  end;
end;

procedure TStateWindows.DHeroStNGMag1Click(Sender:TObject; X, Y:Integer);
var
  Magic:PTClientMagic;
begin
  g_HeroMagicNGList.Lock;
  try
    if (TDxImageButton(Sender).Tag + HeroMagicNGIndex >= 0) and (TDxImageButton(Sender).Tag + HeroMagicNGIndex < g_HeroMagicNGList.Count) then begin
      Magic := g_HeroMagicNGList.Items[TDxImageButton(Sender).Tag + HeroMagicNGIndex];
      if Ord(Magic.Key) = VK_F1 then
        Magic.Key := Chr(0)
      else
        Magic.Key := Chr(VK_F1);
      frmMain.SendHeroMagicKeyChange(Magic.Def.wMagicId, Magic.Def.MagicAttr);
    end;
  finally
    g_HeroMagicNGList.UnLock;
  end;
end;

procedure TStateWindows.DHeroStNGPageUpClick(Sender:TObject; X, Y:Integer);
begin
  g_HeroMagicNGList.Lock;
  try
    if Sender = DHeroStNGPageUp then begin
      if HeroMagicNGIndex > 0 then
        Dec(HeroMagicNGIndex, 6);
      if HeroMagicNGIndex < 0 then HeroMagicNGIndex := 0;
    end
    else begin
      if HeroMagicNGIndex + 6 < g_HeroMagicNGList.Count then begin
        Inc(HeroMagicNGIndex, 6);
      end;
    end;
  finally
    g_HeroMagicNGList.UnLock;
  end;
  HeroNGMagicPageChange;
end;

procedure TStateWindows.DHeroStNGMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
var
  I, Index:Integer;
  DxButton:TDxImageButton;
  vtRect:TRect;
  pm:PTClientMagic;
  SkillDescList:TStrings;

  HintWindow:THintWindow;
  HintLines:THintLines;
begin
  DScreen.ClearHint;
  HintWindows.Clear;
  DxButton := TDxImageButton(Sender);
  g_HeroMagicNGList.Lock;
  try
    Index := DxButton.Tag + HeroMagicNGIndex;
    if (Index >= 0) and (Index < g_HeroMagicNGList.Count) then begin
      pm := PTClientMagic(g_HeroMagicNGList.Items[Index]);
      SkillDescList := nil;
      GetSkillDesc(pm.Def.sMagicName, SkillDescList);
      if SkillDescList <> nil then begin
        HintWindow := THintWindow.Create;
        for I := 0 to SkillDescList.Count - 1 do begin
          HintLines := THintLines.Create;
          HintLines.Add(SkillDescList.Strings[I], TColor(SkillDescList.Objects[I]), GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
          HintWindow.Add(HintLines);
        end;
        vtRect := DxButton.VirtualRect;
        HintWindow.Show(vtRect.Left - 100, vtRect.Top);
        HintWindows.Add(HintWindow);
      end;
    end;
  finally
    g_HeroMagicNGList.UnLock;
  end;
end;

procedure TStateWindows.HeroNGMagicPageChange;
var
  nPage, nPageCount:Integer;
begin
  g_HeroMagicNGList.Lock;
  try
    if g_HeroMagicNGList.Count > 0 then begin
      nPage := (HeroMagicNGIndex + 1) div 6 + 1;
      nPageCount := g_HeroMagicNGList.Count div 6;
      if g_HeroMagicNGList.Count mod 6 > 0 then
        Inc(nPageCount);
      if nPageCount <= 0 then nPageCount := 1;

      { TODO -ochongchong -c修改 : 第二页只有一个魔法，删除魔法后，不重定位到第一页 【2013-09-04】 }
      if nPage > nPageCount then begin
        nPage := nPageCount;

        if HeroMagicNGIndex > 0 then
          Dec(HeroMagicNGIndex, 6);
        if HeroMagicNGIndex < 0 then HeroMagicNGIndex := 0;
      end;
    end
    else begin
      nPage := 0;
      nPageCount := 0;
    end;
  finally
    g_HeroMagicNGList.UnLock;
  end;
  DHeroStNGMagicPage.Caption := Format('%d/%d', [nPage, nPageCount]);
end;

{------------------------------------------------------------------------------}

procedure TStateWindows.DHeroStateLJMagicDirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  d:TTexture;
  sMagicName, sStormsHit:string;
  HGEFont:THGEFont;
begin
  if g_MyHero = nil then Exit;
  g_HeroContinuousMagicList.Lock;
  try
    with Sender as TDxImageForm do begin
      if (Tag >= 0) and (Tag < g_HeroContinuousMagicList.Count) then begin
        Magic := g_HeroContinuousMagicList.Items[Tag];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          // MagicName
          sMagicName := Magic.Def.sMagicName;

          { TODO -ochongchong -c新增 : 连击技能显示时增加暴击率 }
          sStormsHit := GetContinuousBlastStormsHitText(Magic, True);
          if Length(sStormsHit) > 0 then
            sMagicName := sMagicName + ':' + sStormsHit;

          // 技能名称
          HGEFont := TextureFonts.FindFont(DHeroStLJLblName1.CaptionColor.Up.Name, DHeroStLJLblName1.CaptionColor.Up.Size, DHeroStLJLblName1.CaptionColor.Up.Style);
          if HGEFont = nil then
            HGEFont := CurrentFont;
          HGEFont.TextOut(vtRect.Left + DHeroStLJLblName1.Left, vtRect.Top + DHeroStLJLblName1.Top, sMagicName, DHeroStLJLblName1.CaptionColor.Up.Color);

          // Level
          // d := g_WMainImages.Images[112];
          d := DHeroStLJLvIcon1.ImageIndex.Image.Images[DHeroStLJLvIcon1.ImageIndex.Up];
          if d <> nil then begin
            GameCanvas.Draw(vtRect.Left + DHeroStLJLvIcon1.Left, vtRect.Top + DHeroStLJLvIcon1.Top, d.ClientRect, d);
          end;

          HGEFont := TextureFonts.FindFont(DHeroStLJLvText1.CaptionColor.Up.Name, DHeroStLJLvText1.CaptionColor.Up.Size, DHeroStLJLvText1.CaptionColor.Up.Style);
          if HGEFont = nil then
            HGEFont := CurrentFont;
          HGEFont.TextOut(vtRect.Left + DHeroStLJLvText1.Left, vtRect.Top + DHeroStLJLvText1.Top, IntToStr(Magic.Level), DHeroStLJLvText1.CaptionColor.Up.Color);

          // exp
          // d := g_WMainImages.Images[111];
          d := DHeroStLJExpIcon1.ImageIndex.Image.Images[DHeroStLJExpIcon1.ImageIndex.Up];
          if d <> nil then begin
            GameCanvas.Draw(vtRect.Left + DHeroStLJExpIcon1.Left, vtRect.Top + DHeroStLJExpIcon1.Top, d.ClientRect, d);
          end;

          // 连击技能不显示经验值 chongchong 2013-11-09
          HGEFont := TextureFonts.FindFont(DHeroStLJExpText1.CaptionColor.Up.Name, DHeroStLJExpText1.CaptionColor.Up.Size, DHeroStLJExpText1.CaptionColor.Up.Style);
          if HGEFont = nil then
            HGEFont := CurrentFont;
          HGEFont.TextOut(vtRect.Left + DHeroStLJExpText1.Left, vtRect.Top + DHeroStLJExpText1.Top, '-', DHeroStLJExpText1.CaptionColor.Up.Color);
        end;
      end;
    end;
  finally
    g_HeroContinuousMagicList.UnLock;
  end;
end;

procedure TStateWindows.DHeroStLJMag1DirectPaint(Sender:TObject);
var
  Magic:PTClientMagic;
  vtRect:TRect;
  vbRect:TRect;
  Index:Integer;
  d:TTexture;
begin
  if g_MyHero = nil then Exit;
  g_HeroContinuousMagicList.Lock;
  try
    with Sender as TDxImageButton do begin
      if (Tag >= 0) and (Tag < g_HeroContinuousMagicList.Count) then begin
        Magic := g_HeroContinuousMagicList.Items[Tag];

        if Magic <> nil then begin
          vbRect := VisibleRect;
          if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
          vtRect := VirtualRect;

          case Magic.Def.wMagicId of
            100:Index := 950;
            101:Index := 952;
            102:Index := 956;
            103:Index := 954;
            104:Index := 942;
            105:Index := 946;
            106:Index := 940;
            107:Index := 944;
            108:Index := 934;
            109:Index := 936;
            110:Index := 932;
            111:Index := 930;
            else
              Index := -1;
          end;

          if MouseDowned then begin
            d := g_WMainImages.Images[Index + 1];
          end
          else begin
            d := g_WMainImages.Images[Index];
          end;

          if d <> nil then
            GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
        end;
      end;
    end;
  finally
    g_HeroContinuousMagicList.UnLock;
  end;
end;

procedure TStateWindows.DHeroStLJMagMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
var
  I, Index:Integer;
  DxButton:TDxImageButton;
  vtRect:TRect;
  pm:PTClientMagic;
  SkillDescList:TStrings;

  HintWindow:THintWindow;
  HintLines:THintLines;
begin
  DScreen.ClearHint;
  HintWindows.Clear;
  DxButton := TDxImageButton(Sender);
  g_HeroContinuousMagicList.Lock;
  try
    Index := DxButton.Tag;
    if (Index >= 0) and (Index < g_HeroContinuousMagicList.Count) then begin
      pm := PTClientMagic(g_HeroContinuousMagicList.Items[Index]);
      SkillDescList := nil;
      GetSkillDesc(pm.Def.sMagicName, SkillDescList);
      if SkillDescList <> nil then begin
        HintWindow := THintWindow.Create;
        for I := 0 to SkillDescList.Count - 1 do begin
          HintLines := THintLines.Create;
          HintLines.Add(SkillDescList.Strings[I], TColor(SkillDescList.Objects[I]), GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
          HintWindow.Add(HintLines);
        end;
        vtRect := DxButton.VirtualRect;
        HintWindow.Show(vtRect.Left - 100, vtRect.Top);
        HintWindows.Add(HintWindow);
      end;
    end;
  finally
    g_HeroContinuousMagicList.UnLock;
  end;
end;

procedure TStateWindows.DHeroSMB1DirectPaint(Sender:TObject);
var
  Index:Integer;
  DxButton:TDxImageButton;
  vtRect:TRect;
  d:TTexture;

  HGEFont:THGEFont;
  S:string;
  TextW, TextH:Integer;
  nTop:Integer;
begin
  if g_MyHero = nil then Exit; // 连击顺序     0=空 1=随机
  DxButton := TDxImageButton(Sender);
  if DxButton.Tag in [0..3] then begin
    vtRect := DxButton.VirtualRect;

    if (DxButton.Tag = 3) and (not g_boHeroOpenLastContinuous) then begin // 第四个连击是否开启
      d := g_WMainImages.Images[912];
      if d <> nil then
        GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
      Exit;
    end;

    if g_HeroContinuousMagicOrder[DxButton.Tag] = 0 then begin // 0=空
      {                                                                                          // 0=空
      if (DxButton.Tag = 3) then
      begin
        Index := 907;
      end
      else
      begin
        Index := 903 + DxButton.Tag * 2;
      end;

      if DxButton.MouseDowned then
        d := g_WMainImages.Images[Index + 1]
      else
        d := g_WMainImages.Images[Index];
      }

      // 自定义连击顺序暴击几率 2019-08-07 23:26:18
      if not DxButton.MouseDowned then begin
        d := g_WNewopUIImages.Images[1760];

        HGEFont := TextureFonts.FindFont(DxButton.CaptionColor.Up.Name, DxButton.CaptionColor.Up.Size, DxButton.CaptionColor.Up.Style);
        if HGEFont = nil then
          HGEFont := CurrentFont;
      end
      else begin
        d := g_WNewopUIImages.Images[1761];

        HGEFont := TextureFonts.FindFont(DxButton.CaptionColor.Down.Name, DxButton.CaptionColor.Down.Size, DxButton.CaptionColor.Down.Style);
        if HGEFont = nil then
          HGEFont := CurrentFont;
      end;

      if d <> nil then
        GameCanvas.Draw(vtRect.Left, vtRect.Top, d);

      if HGEFont <> nil then begin
        S := '暴击';
        TextW := HGEFont.TextWidth(S);
        TextH := HGEFont.TextHeight(S);
        nTop := 2;

        if not DxButton.MouseDowned then begin
          if DxButton.CaptionColor.Up.Bold then
            BoldTextOut(HGEFont, vtRect.Left + (vtRect.Right - vtRect.Left - TextW) div 2 - 2, vtRect.Top + nTop, S, DxButton.CaptionColor.Up.Color, DxButton.CaptionColor.Up.BColor)
          else
            HGEFont.TextOut(vtRect.Left + (vtRect.Right - vtRect.Left - TextW) div 2, vtRect.Top + nTop, S, DxButton.CaptionColor.Up.Color);
        end
        else begin
          if DxButton.CaptionColor.Down.Bold then
            BoldTextOut(HGEFont, vtRect.Left + (vtRect.Right - vtRect.Left - TextW) div 2 - 2 + 1, vtRect.Top + nTop + 1, S, DxButton.CaptionColor.Down.Color, DxButton.CaptionColor.Down.BColor)
          else
            HGEFont.TextOut(vtRect.Left + (vtRect.Right - vtRect.Left - TextW) div 2 + 1, vtRect.Top + nTop + 1, S, DxButton.CaptionColor.Down.Color);
        end;

        nTop := nTop + TextH + 2;

        S := '+' + IntToStr(g_ClientConfig.SkillContinueOrderBlastRates[DxButton.Tag]) + '%';
        TextW := HGEFont.TextWidth(S);

        if not DxButton.MouseDowned then begin
          if DxButton.CaptionColor.Up.Bold then
            BoldTextOut(HGEFont, vtRect.Left + (vtRect.Right - vtRect.Left - TextW) div 2 - 2, vtRect.Top + nTop, S, DxButton.CaptionColor.Up.Color, DxButton.CaptionColor.Up.BColor)
          else
            HGEFont.TextOut(vtRect.Left + (vtRect.Right - vtRect.Left - TextW) div 2, vtRect.Top + nTop, S, DxButton.CaptionColor.Up.Color);
        end
        else begin
          if DxButton.CaptionColor.Down.Bold then
            BoldTextOut(HGEFont, vtRect.Left + (vtRect.Right - vtRect.Left - TextW) div 2 - 2 + 1, vtRect.Top + nTop + 1, S, DxButton.CaptionColor.Down.Color, DxButton.CaptionColor.Down.BColor)
          else
            HGEFont.TextOut(vtRect.Left + (vtRect.Right - vtRect.Left - TextW) div 2 + 1, vtRect.Top + nTop + 1, S, DxButton.CaptionColor.Down.Color);
        end;
      end;
    end
    else if g_HeroContinuousMagicOrder[DxButton.Tag] = 1 then begin // 1=随机
      if DxButton.MouseDowned then begin
        d := g_WMainImages.Images[910];
      end
      else begin
        d := g_WMainImages.Images[909];
      end;
      if d <> nil then
        GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
    end
    else begin //
      case g_HeroContinuousMagicOrder[DxButton.Tag] of
        100:Index := 950;
        101:Index := 952;
        102:Index := 956;
        103:Index := 954;
        104:Index := 942;
        105:Index := 946;
        106:Index := 940;
        107:Index := 944;
        108:Index := 934;
        109:Index := 936;
        110:Index := 932;
        111:Index := 930;
        else
          Index := -1;
      end;

      if Index > 0 then begin
        if DxButton.MouseDowned then begin
          d := g_WMainImages.Images[Index + 1];
        end
        else begin
          d := g_WMainImages.Images[Index];
        end;
        if d <> nil then
          GameCanvas.Draw(vtRect.Left, vtRect.Top, d);
      end;
    end;
  end;
end;

procedure TStateWindows.DHeroSMB1Click(Sender:TObject; X, Y:Integer);
var
  I:Integer;
  vtRect:TRect;
  pm:PTClientMagic;
begin
  if (TDxImageButton(Sender).Tag = 3) and (not g_boOpenLastContinuous) then Exit;
  g_HeroContinuousMagicList.Lock;
  try
    if g_HeroContinuousMagicList.Count > 0 then begin
      vtRect := TDxImageButton(Sender).VirtualRect;
      HeroContinuousMagicMenu.Items.Clear;
      for I := 0 to g_HeroContinuousMagicList.Count - 1 do begin
        pm := g_HeroContinuousMagicList.Items[I];
        HeroContinuousMagicMenu.Items.AddObject(pm.Def.sMagicName, TObject(pm.Def.wMagicId));
      end;
      HeroContinuousMagicMenu.Items.AddObject('空', TObject(0));
      HeroContinuousMagicMenu.Items.AddObject('随机', TObject(1));

      HeroContinuousMagicMenu.Tag := TDxImageButton(Sender).Tag;
      HeroContinuousMagicMenu.Left := vtRect.Left;
      HeroContinuousMagicMenu.Top := vtRect.Bottom;
      HeroContinuousMagicMenu.Show;
    end;
  finally
    g_HeroContinuousMagicList.UnLock;
  end;
end;

procedure TStateWindows.HeroContinuousMagicMenuClick(Sender:TObject; X, Y:Integer);
var
  I:Integer;
  ItemMenu:pTDxItemMenu;
begin
  if HeroContinuousMagicMenu.ItemIndex >= 0 then begin
    ItemMenu := pTDxItemMenu(HeroContinuousMagicMenu.Items.Objects[HeroContinuousMagicMenu.ItemIndex]);
    if HeroContinuousMagicMenu.Tag in [0..3] then begin
      g_HeroContinuousMagicOrder[HeroContinuousMagicMenu.Tag] := Integer(ItemMenu.AObject);
      if g_HeroContinuousMagicOrder[HeroContinuousMagicMenu.Tag] > 1 then begin
        for I := 0 to Length(g_HeroContinuousMagicOrder) - 1 do begin
          if (I <> HeroContinuousMagicMenu.Tag) and (g_HeroContinuousMagicOrder[I] = g_HeroContinuousMagicOrder[HeroContinuousMagicMenu.Tag]) then begin
            g_HeroContinuousMagicOrder[I] := 0;
          end;
        end;
      end;
      frmMain.SendChangeContinuousMagicOrder(True); // 改变连击顺序
    end;
  end;
end;

procedure TStateWindows.SetDeputyHeroJob(btJob:Byte);
begin
  if not Initialized then Exit;
  case btJob of
    0:DBotStateDeputyHeroJob0.Checked := True;
    1:DBotStateDeputyHeroJob1.Checked := True;
    2:DBotStateDeputyHeroJob2.Checked := True;
  end;
end;

procedure TStateWindows.LabelDStateWinCharNameClick(Sender:TObject; X,
  Y:Integer);
begin
  FrmDlg.LabelDStateWinCharNameClick(Sender, x, y);
end;

procedure TStateWindows.SetMySelfJewelryBoxButton;
begin
  if not Initialized then Exit;

  //if DStateWin.Visible then
  begin
    case g_MySelf.m_nJewelryBoxStatus of
      jbsNoActive:begin
          DSWJewelryBox.Visible := False;
        end;
      jbsActive:begin
          DSWJewelryBox.Visible := True;

          DSWJewelryBox.ImageIndex.Up := DSWJewelryBox.ImageIndex.Disabled;
          DSWJewelryBox.ImageIndex.Down := DSWJewelryBox.ImageIndex.Disabled;
        end;
      jbsOpen:begin
          DSWJewelryBox.Visible := True;

          DSWJewelryBox.ImageIndex.Up := FJewelryBoxUpImageIndex;
          DSWJewelryBox.ImageIndex.Down := FJewelryBoxDownImageIndex;
        end;
    end;
  end;
end;

procedure TStateWindows.SetMyHeroJewelryBoxButton;
begin
  if not Initialized then Exit;
  if g_MyHero = nil then Exit;

  //if DHeroStateWin.Visible then
  begin
    case g_MyHero.m_nJewelryBoxStatus of
      jbsNoActive:begin
          DHeroSWJewelryBox.Visible := False;
        end;
      jbsActive:begin
          DHeroSWJewelryBox.Visible := True;

          DHeroSWJewelryBox.ImageIndex.Up := DHeroSWJewelryBox.ImageIndex.Disabled;
          DHeroSWJewelryBox.ImageIndex.Down := DHeroSWJewelryBox.ImageIndex.Disabled;
        end;
      jbsOpen:begin
          DHeroSWJewelryBox.Visible := True;

          DHeroSWJewelryBox.ImageIndex.Up := FHeroJewelryBoxUpImageIndex;
          DHeroSWJewelryBox.ImageIndex.Down := FHeroJewelryBoxDownImageIndex;
        end;
    end;
  end;
end;

procedure TStateWindows.DSWJewelryBoxClick(Sender:TObject; X, Y:Integer);
begin
  if Sender = DSWJewelryClose then
    DSWJewelryBoxDlg.Visible := False
  else if Sender = DHeroSWJewelryClose then
    DHeroSWJewelryBoxDlg.Visible := False
  else if Sender = DJewelryCloseUS1 then
    DJewelryBoxDlgUS1.Visible := False
  else if Sender = DSWJewelryBox then begin
    if g_MySelf.m_nJewelryBoxStatus = jbsActive then begin
      frmMain.SendOpenJewelryBox;
    end
    else if g_MySelf.m_nJewelryBoxStatus = jbsOpen then begin
      if not DSWJewelryBoxDlg.Visible then
        DSWJewelryBoxDlg.Left := Max(0, DStateWin.Left - DSWJewelryBoxDlg.Width);
      DSWJewelryBoxDlg.Visible := not DSWJewelryBoxDlg.Visible;
    end;
  end
  else if Sender = DHeroSWJewelryBox then begin
    if g_MyHero = nil then Exit;

    if g_MyHero.m_nJewelryBoxStatus = jbsActive then begin
      frmMain.SendHeroOpenJewelryBox;
    end
    else if g_MyHero.m_nJewelryBoxStatus = jbsOpen then begin
      if not DSWJewelryBoxDlg.Visible then
        DHeroSWJewelryBoxDlg.Left := Max(0, DHeroStateWin.Left - DHeroSWJewelryBoxDlg.Width);
      DHeroSWJewelryBoxDlg.Visible := not DHeroSWJewelryBoxDlg.Visible;
    end;
  end
  else if Sender = DJewelryBoxUS1 then begin
    if g_UserState1.JewelryBoxStatus <> jbsOpen then Exit;

    if not DSWJewelryBoxDlg.Visible then
      DJewelryBoxDlgUS1.Left := Max(0, DUserState1.Left - DJewelryBoxDlgUS1.Width);
    DJewelryBoxDlgUS1.Visible := not DJewelryBoxDlgUS1.Visible;
  end;
end;

procedure TStateWindows.DSWJewelryBoxMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  nLocalX, nLocalY:Integer;
  nHintX, nHintY:Integer;
  Butt:TDxImageButton;
  vtRect:TRect;
  smsg:string;
begin
  Butt := TDxImageButton(Sender);
  //if Sender = DBotMiniMap then
  smsg := g_ClientConfig.sJewelryBoxHint;
  if smsg = '' then
    smsg := Butt.Hint;

  nLocalX := -(((CurrentFont.TextWidth(smsg) - Butt.Width) div 2) + 8);
  nLocalY := 0;

  vtRect := Butt.VirtualRect;
  nHintX := vtRect.Left + nLocalX;
  nHintY := vtRect.Top + nLocalY;
  DScreen.ClearHint;
  HintWindows.Clear;
  if smsg <> '' then
    HintWindows.Show(nHintX, nHintY, smsg, clWhite, True);
end;

procedure TStateWindows.DSWJewelryBoxItemMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  I, Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  DrawLeft:Boolean;

  HintWindow:THintWindow;
  HintList:TList;
  TzHintWindows:TList;

  ArrHintWindows:TArrHintWindows;
begin
  Index := TDxImageButton(Sender).Tag;

  if (Index >= 0) and (g_JewelryBoxItems[Index].s.Name <> '') then begin
    g_MouseStateItem := g_JewelryBoxItems[Index];
    g_boShowBagInfo := False;

    // 免得不停的重新创建提示窗口 chongchong 2015-01-22
    if (g_MouseStateItem.MakeIndex <> g_LastHintMakeIndex) then begin
      with DStateWin do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left + 40
          else
            nX := vtRect.Right - 20;
        end;

        if Length(g_MouseStateItem.s.Name) = 0 then begin
          HintWindows.Show(nX, nY, '首饰盒物品', clWhite, False, DrawLeft);
        end
        else begin
          HintWindows.Clear;
          HintList := TList.Create;

          // 两列显示 piaoyun 2013-09-15
          if not g_ClientConfig.boSingleHint then begin
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then
                HintList.Add(HintWindow)
              else
                HintWindow.Free;
            end;

            if not g_ClientConfig.boTZSupportRenameItem then
              TzHintWindows := FrmDlg.GetTzItemHintWindow(0, g_MySelf.m_btSex, g_MouseStateItem.s.DBName)
            else
              TzHintWindows := FrmDlg.GetTzItemHintWindow(0, g_MySelf.m_btSex, g_MouseStateItem.s.Name);

            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end;
          end
          else begin
            // 单列显示 piaoyun 2013-09-15
            TzHintWindows := FrmDlg.GetTzItemHintWindowEx(0, g_MySelf.m_btSex, g_MySelf, @g_MouseStateItem, False);
            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end
            else begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then
                  HintList.Add(HintWindow)
                else
                  HintWindow.Free;
              end;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
          HintList.Free;

        end;
      end;
      g_LastHintMakeIndex := g_MouseStateItem.MakeIndex;
    end;
  end
  else begin
    g_LastHintMakeIndex := -1;
    g_boShowBagInfo := False;
    g_MouseStateItem.s.Name := '';
    DScreen.ClearHint;
    HintWindows.Clear;
  end;
end;

// 首饰盒，放入/拿出 chongchong 2013-10-20

procedure TStateWindows.DSWJewelryBoxItemClick(Sender:TObject; X,
  Y:Integer);
var
  where, SelIndex:Integer;
  Flag:Boolean;
  DButton:TDxImageButton;
begin
  if g_MySelf = nil then Exit;

  DButton := TDxImageButton(Sender);
  SelIndex := DButton.Tag;

  if g_boItemMoving then begin
    Flag := False;
    if not (g_MovingItem.ItemType in [mtBagItem {, mtJewelryItem}]) then Exit;

    if (g_MovingItem.Item.S.Name = '') or (g_WaitingUseItem.Item.S.Name <> '') then Exit;

    where := GetTakeOnPosition(g_MovingItem.Item.S.StdMode, g_MovingItem.Item.S.Shape);
    if (g_MovingItem.Index >= 0) and (g_MovingItem.Item.S.OverLap and 2 <> 0) and
      ((g_MovingItem.Item.S.Expand1 = SelIndex + 1) or (g_MovingItem.Item.S.Expand1 in [0, 7])) and
      (where in [U_HELMET {头盔}, U_NECKLACE {项链}, U_ARMRINGL {左手镯}, U_ARMRINGR {右手镯}, U_RINGL {左戒指}, U_RINGL {右戒指}]) then
      Flag := True;

    if Flag then begin
      {
      // 不允许交换位置，限定位置的首饰和无限定的交换会有问题，而且无触发 2020-10-23
      if g_MovingItem.ItemType = mtJewelryItem then
      begin
        if g_MovingItem.Index in [Low(TJewelryBoxItems)..High(TJewelryBoxItems)] then
        begin
          if SelIndex <> g_MovingItem.Index then
          begin
            g_JewelryBoxItems[g_MovingItem.Index] := g_JewelryBoxItems[SelIndex];
            g_JewelryBoxItems[SelIndex] := g_MovingItem.Item;
            frmMain.SendSwapJewelryItem(SelIndex, g_MovingItem.Index);
          end
          else
            g_JewelryBoxItems[SelIndex] := g_MovingItem.Item;

          g_MovingItem.Item.S.Name := '';
          g_boItemMoving := False;
        end;
      end
      else}
      if g_MovingItem.ItemType = mtBagItem then begin
        ItemClickSound(g_MovingItem.Item.S);
        g_WaitingUseItem := g_MovingItem;
        g_WaitingUseItem.Index := SelIndex;

        frmMain.SendTakeOnJewelry(SelIndex, g_MovingItem.Item.MakeIndex, g_MovingItem.Item.S.Name);
        g_MovingItem.Item.S.Name := '';
        g_boItemMoving := False;
      end;
    end;
  end
  else begin
    if (g_MovingItem.Item.S.Name <> '') or (g_WaitingUseItem.Item.S.Name <> '') then Exit;
    if (SelIndex >= 0) and (g_JewelryBoxItems[SelIndex].S.Name <> '') then begin
      ItemClickSound(g_JewelryBoxItems[SelIndex].S);
      g_MovingItem.Index := SelIndex;
      g_MovingItem.Item := g_JewelryBoxItems[SelIndex];
      g_MovingItem.ItemType := mtJewelryItem;
      g_JewelryBoxItems[SelIndex].S.Name := '';
      g_boItemMoving := True;
    end;
  end;
end;

procedure TStateWindows.DSWJewelryBoxItemPaint(Sender:TObject);
var
  Index:Integer;
begin
  if not DSWJewelryBoxDlg.Visible then Exit;
  Index := TDxImageButton(Sender).Tag;

  if Index >= 0 then begin
    FrmDlg.DrawBodyItem(TDxImageButton(Sender).VirtualRect, @g_JewelryBoxItems[Index].s, g_JewelryBoxItems[Index].btHeroM2Light, @g_JewelryBoxItemsEffect[Index], True);
  end;
end;

procedure TStateWindows.DHeroSWJewelryBoxItemClick(Sender:TObject; X,
  Y:Integer);
var
  where, SelIndex:Integer;
  Flag:Boolean;
  DButton:TDxImageButton;
begin
  if g_MyHero = nil then Exit;

  DButton := TDxImageButton(Sender);
  SelIndex := DButton.Tag;

  if g_boItemMoving then begin
    Flag := False;
    if not (g_MovingItem.ItemType in [mtHeroBagItem, mtHeroJewelryItem]) then Exit;

    if (g_MovingItem.Item.S.Name = '') or (g_WaitingHeroUseItem.Item.S.Name <> '') then Exit;

    where := GetTakeOnPosition(g_MovingItem.Item.S.StdMode, g_MovingItem.Item.S.Shape);
    if (g_MovingItem.Index >= 0) and (g_MovingItem.Item.S.OverLap and 2 <> 0) and
      ((g_MovingItem.Item.S.Expand1 = SelIndex + 1) or (g_MovingItem.Item.S.Expand1 in [0, 7])) and
      (where in [U_HELMET {头盔}, U_NECKLACE {项链}, U_ARMRINGL {左手镯}, U_ARMRINGR {右手镯}, U_RINGL {左戒指}, U_RINGL {右戒指}]) then
      Flag := True;

    if Flag then begin
      if g_MovingItem.ItemType = mtHeroJewelryItem then begin
        if g_MovingItem.Index in [Low(TJewelryBoxItems)..High(TJewelryBoxItems)] then begin
          if SelIndex <> g_MovingItem.Index then begin
            g_HeroJewelryBoxItems[g_MovingItem.Index] := g_HeroJewelryBoxItems[SelIndex];
            g_HeroJewelryBoxItems[SelIndex] := g_MovingItem.Item;
            frmMain.SendHeroSwapJewelryItem(SelIndex, g_MovingItem.Index);
          end
          else
            g_HeroJewelryBoxItems[SelIndex] := g_MovingItem.Item;

          g_MovingItem.Item.S.Name := '';
          g_boItemMoving := False;
        end;
      end
      else if g_MovingItem.ItemType = mtHeroBagItem then begin
        ItemClickSound(g_MovingItem.Item.S);
        g_WaitingHeroUseItem := g_MovingItem;
        g_WaitingHeroUseItem.Index := SelIndex;

        frmMain.SendHeroTakeOnJewelry(SelIndex, g_MovingItem.Item.MakeIndex, g_MovingItem.Item.S.Name);
        g_MovingItem.Item.S.Name := '';
        g_boItemMoving := False;
      end;
    end;
  end
  else begin
    if (g_MovingItem.Item.S.Name <> '') or (g_WaitingHeroUseItem.Item.S.Name <> '') then Exit;
    if (SelIndex >= 0) and (g_HeroJewelryBoxItems[SelIndex].S.Name <> '') then begin
      ItemClickSound(g_HeroJewelryBoxItems[SelIndex].S);
      g_MovingItem.Index := SelIndex;
      g_MovingItem.Item := g_HeroJewelryBoxItems[SelIndex];
      g_MovingItem.ItemType := mtHeroJewelryItem;
      g_HeroJewelryBoxItems[SelIndex].S.Name := '';
      g_boItemMoving := True;
    end;
  end;
end;

procedure TStateWindows.DHeroSWJewelryBoxItemMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  I, Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  DrawLeft:Boolean;

  HintWindow:THintWindow;
  HintList:TList;
  TzHintWindows:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := TDxImageButton(Sender).Tag;

  if (Index >= 0) and (g_HeroJewelryBoxItems[Index].s.Name <> '') then begin
    g_MouseStateItem := g_HeroJewelryBoxItems[Index];
    g_boShowBagInfo := False;

    // 免得不停的重新创建提示窗口 chongchong 2015-01-22
    if (g_MouseStateItem.MakeIndex <> g_LastHintMakeIndex) then begin
      with DHeroSWJewelryBoxDlg do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left + 40
          else
            nX := vtRect.Right - 20;
        end;

        if Length(g_MouseStateItem.s.Name) = 0 then begin
          HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
        end
        else begin
          HintWindows.Clear;
          HintList := TList.Create;

          // 两列显示 piaoyun 2013-09-15
          if not g_ClientConfig.boSingleHint then begin
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MyHero, @g_MouseStateItem, nX, nY, False, False, False);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then
                HintList.Add(HintWindow)
              else
                HintWindow.Free;
            end;

            if not g_ClientConfig.boTZSupportRenameItem then
              TzHintWindows := FrmDlg.GetTzItemHintWindow(1, g_MyHero.m_btSex, g_MouseStateItem.s.DBName)
            else
              TzHintWindows := FrmDlg.GetTzItemHintWindow(1, g_MyHero.m_btSex, g_MouseStateItem.s.Name);

            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end;
          end
          else begin
            // 单列显示 piaoyun 2013-09-15
            TzHintWindows := FrmDlg.GetTzItemHintWindowEx(1, g_MyHero.m_btSex, g_MyHero, @g_MouseStateItem, False);
            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end
            else begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MyHero, @g_MouseStateItem, nX, nY, False, False, False);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then
                  HintList.Add(HintWindow)
                else
                  HintWindow.Free;
              end;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
          HintList.Free;
        end;
      end;

      g_LastHintMakeIndex := g_MouseStateItem.MakeIndex;
    end;
  end
  else begin
    g_LastHintMakeIndex := -1;
    g_boShowBagInfo := False;
    g_MouseStateItem.s.Name := '';
    DScreen.ClearHint;
    HintWindows.Clear;
  end;
end;

procedure TStateWindows.DHeroSWJewelryBoxItemPaint(Sender:TObject);
var
  Index:Integer;
begin
  if not DHeroSWJewelryBoxDlg.Visible then Exit;
  Index := TDxImageButton(Sender).Tag;

  if Index >= 0 then begin
    FrmDlg.DrawBodyItem(TDxImageButton(Sender).VirtualRect, @g_HeroJewelryBoxItems[Index].s, g_HeroJewelryBoxItems[Index].btHeroM2Light, @g_HeroJewelryBoxItemsEffect[Index], True);
  end;
end;

procedure TStateWindows.DJewelryBoxItemUS1MouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  I, Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  DrawLeft:Boolean;

  HintWindow:THintWindow;
  HintList:TList;

  TzHintWindows:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := TDxImageButton(Sender).Tag;

  if (Index >= 0) and (g_UserState1.JewelryItems[Index].s.Name <> '') then begin
    g_MouseStateItem := g_UserState1.JewelryItems[Index];
    g_boShowBagInfo := False;

    // 免得不停的重新创建提示窗口 chongchong 2015-01-22
    if (g_MouseStateItem.MakeIndex <> g_LastHintMakeIndex) then begin
      with DJewelryBoxDlgUS1 do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left + 40
          else
            nX := vtRect.Right - 20;
        end;

        if Length(g_MouseStateItem.s.Name) = 0 then begin
          HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
        end
        else begin
          // 增加套装显示 chongchong 2014-10-23
          {
          HintWindows.Clear;
          HintList := TList.Create;

          HintWindow := THintWindow(FrmDlg.GetMouseItemInfoWindow(g_MySelf, @g_MouseStateItem, nX, nY, False, False, False));
          HintList.Add(HintWindow);
          }

          HintWindows.Clear;
          HintList := TList.Create;

          // 两列显示 piaoyun 2013-09-15
          if not g_ClientConfig.boSingleHint then begin
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then
                HintList.Add(HintWindow)
              else
                HintWindow.Free;
            end;

            if not g_ClientConfig.boTZSupportRenameItem then
              TzHintWindows := FrmDlg.GetTzItemHintWindow(2, g_MySelf.m_btSex, g_MouseStateItem.s.DBName)
            else
              TzHintWindows := FrmDlg.GetTzItemHintWindow(2, g_MySelf.m_btSex, g_MouseStateItem.s.Name);

            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end;
          end
          else begin
            // 单列显示 piaoyun 2013-09-15
            TzHintWindows := FrmDlg.GetTzItemHintWindowEx(2, g_MySelf.m_btSex, g_MySelf, @g_MouseStateItem, False);
            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end
            else begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then
                  HintList.Add(HintWindow)
                else
                  HintWindow.Free;
              end;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
          HintList.Free;
        end;
      end;
      g_LastHintMakeIndex := g_MouseStateItem.MakeIndex;
    end;
  end
  else begin
    g_LastHintMakeIndex := -1;
    g_boShowBagInfo := False;
    g_MouseStateItem.s.Name := '';
    DScreen.ClearHint;
    HintWindows.Clear;
  end;
end;

procedure TStateWindows.DJewelryBoxItemPaintUS1(Sender:TObject);
var
  Index:Integer;
begin
  if not DJewelryBoxDlgUS1.Visible then Exit;
  Index := TDxImageButton(Sender).Tag;

  if Index >= 0 then begin
    FrmDlg.DrawBodyItem(TDxImageButton(Sender).VirtualRect, @g_UserState1.JewelryItems[Index].s, g_UserState1.JewelryItems[Index].btHeroM2Light, @g_JewelryBoxItemsUS1Effect[Index], True);
  end;
end;

procedure TStateWindows.DSWGodBlessClick(Sender:TObject; X, Y:Integer);
begin
  if Sender = DSWGodBlessClose then
    DSWGodBlessDlg.Visible := False
  else if Sender = DHeroSWGodBlessClose then
    DHeroSWGodBlessDlg.Visible := False
  else if Sender = DGodBlessCloseUS1 then
    DGodBlessDlgUS1.Visible := False
  else if Sender = DSWGodBless then begin
    if not DSWGodBlessDlg.Visible then
      DSWGodBlessDlg.Left := Max(0, DStateWin.Left - DSWGodBlessDlg.Width);
    DSWGodBlessDlg.Visible := not DSWGodBlessDlg.Visible;
  end
  else if Sender = DHeroSWGodBless then begin
    if g_MyHero = nil then Exit;

    if not DSWGodBlessDlg.Visible then
      DHeroSWGodBlessDlg.Left := Max(0, DHeroStateWin.Left - DHeroSWGodBlessDlg.Width);
    DHeroSWGodBlessDlg.Visible := not DHeroSWGodBlessDlg.Visible;
  end
  else if Sender = DGodBlessUS1 then begin
    if not g_UserState1.ShowGodBless then Exit;

    if not DSWGodBlessDlg.Visible then
      DGodBlessDlgUS1.Left := Max(0, DUserState1.Left - DGodBlessDlgUS1.Width);
    DGodBlessDlgUS1.Visible := not DGodBlessDlgUS1.Visible;
  end;
end;

procedure TStateWindows.DSWGodBlessItemClick(Sender:TObject; X, Y:Integer);
var
  SelIndex:Integer;
  Flag:Boolean;
  DButton:TDxImageButton;
begin
  if g_MySelf = nil then Exit;

  DButton := TDxImageButton(Sender);
  SelIndex := DButton.Tag;

  if g_boItemMoving then begin
    if g_GodBlessItemsState[SelIndex] = 1 then begin
      Flag := False;

      // 神佑盒支持原位放回 chongchong 2015-02-06
      if (g_MovingItem.ItemType = mtGodBlessItem) and (g_MovingItem.Index = SelIndex) then begin
        g_GodBlessItems[SelIndex].S.Name := g_MovingItem.Item.s.Name;
        g_MovingItem.Item.S.Name := '';
        g_boItemMoving := False;
        Exit;
      end;

      if not (g_MovingItem.ItemType in [mtBagItem]) then Exit;

      if (g_MovingItem.Item.S.Name = '') or (g_WaitingUseItem.Item.S.Name <> '') then Exit;

      if (g_MovingItem.Index >= 0) and (g_MovingItem.Item.S.OverLap and 4 <> 0) and ((g_MovingItem.Item.S.Expand1 = SelIndex + 1) or (g_MovingItem.Item.S.Expand1 = 13)) then
        Flag := True;

      if Flag then begin
        if g_MovingItem.ItemType = mtBagItem then begin
          ItemClickSound(g_MovingItem.Item.S);
          g_WaitingUseItem := g_MovingItem;
          g_WaitingUseItem.Index := SelIndex;

          frmMain.SendTakeOnGodBless(False, SelIndex, g_MovingItem.Item.MakeIndex, g_MovingItem.Item.S.Name);
          g_MovingItem.Item.S.Name := '';
          g_boItemMoving := False;
        end;
      end;
    end;
  end
  else begin
    if g_GodBlessItemsState[SelIndex] <> 0 then begin
      if (g_MovingItem.Item.S.Name <> '') or (g_WaitingUseItem.Item.S.Name <> '') then Exit;
      if (SelIndex >= 0) and (g_GodBlessItems[SelIndex].S.Name <> '') then begin
        ItemClickSound(g_GodBlessItems[SelIndex].S);
        g_MovingItem.Index := SelIndex;
        g_MovingItem.Item := g_GodBlessItems[SelIndex];
        g_MovingItem.ItemType := mtGodBlessItem;
        g_GodBlessItems[SelIndex].S.Name := '';
        g_boItemMoving := True;
      end;
    end
    else begin
      frmMain.SendGodBlessItemClick(False, SelIndex);
    end;
  end;
end;

function GetGodBlessItemCaption(Index:Integer):string;
const
  Captions:array[0..11] of string = (
    '子鼠',
    '丑牛',
    '寅虎',
    '卯兔',
    '辰龙',
    '已蛇',
    '午马',
    '未羊',
    '申猴',
    '酉鸡',
    '戌狗',
    '亥猪');
var
  ItemDesc:TStringList;
begin
  // 神佑格提示标题从文件配置 chongchong 2014-04-23
  if (Index >= Low(Captions)) and (Index <= High(Captions)) then begin
    ItemDesc := GetGodBlessItem('title' + IntToStr(Index + 1));
    if (ItemDesc <> nil) and (ItemDesc.Count > 0) then
      Result := ItemDesc.Strings[0]
    else
      Result := Captions[index] + '神佑格';
  end
  else
    Result := '神佑格' + IntToStr(Index + 1);
end;

// 神佑格提示文字一行可以有多个颜色 chongchong 2014-04-23

procedure GetHitLines(HintLines:THintLines; S:string; DefColor:TColor);
var
  nPos:Integer;
  Color:TColor;
  I:Integer;
begin
  nPos := Pos('/', S);
  if nPos = 0 then
    HintLines.Add(S, DefColor, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke(True))
  else begin
    Color := DefColor;
    while nPos > 0 do begin
      for I := nPos - 1 downto 1 do begin
        if not (S[I] in ['0'..'9']) then Break;
      end;

      if I < nPos - 4 then
        I := nPos - 4;
      if I = 0 then begin
        S := Copy(S, nPos + 1, MaxInt);
        nPos := Pos('/', S);
      end
      else begin
        HintLines.Add(Copy(S, 1, I), Color, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke(True));
        Color := GetRGB(StrToIntDef(Copy(S, I + 1, nPos - I - 1), 255));
        S := Copy(S, nPos + 1, MaxInt);
        nPos := Pos('/', S);
      end;
    end;

    if Length(S) > 0 then
      HintLines.Add(S, Color, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke(True));
  end;
end;

procedure TStateWindows.DSWGodBlessItemMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
var
  I, Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  DrawLeft:Boolean;

  HintWindow:THintWindow;
  HintList:TList;
  ItemDesc:TStringList;
  HintLines:THintLines;
  TzHintWindows:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := TDxImageButton(Sender).Tag;
  if Index < 0 then Exit;

  if g_GodBlessItemsState[Index] = 0 then begin
    HintWindows.Clear;

    ItemDesc := GetGodBlessItem(IntToStr(Index + 1));
    if (ItemDesc <> nil) and (ItemDesc.Count > 0) then begin
      HintWindow := THintWindow.Create;

      HintLines := THintLines.Create;
      HintLines.Add(GetGodBlessItemCaption(Index), GetRGB(251), GetHintNameFontSize, GetHintNameFontStyle([fsBold]), GetHintNameFontStroke(True), GetHintNameFontName);
      HintWindow.Add(HintLines);

      if g_ClientConfig.boShowHintLines then begin
        HintLines := THintLines.Create;
        HintLines.Add('-');
        HintWindow.Add(HintLines);
      end;

      for I := 0 to ItemDesc.Count - 1 do begin
        HintLines := THintLines.Create;
        //HintLines.Add(ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]), 9, [], True);
        GetHitLines(HintLines, ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]));
        HintWindow.Add(HintLines);
      end;

      HintWindow.Show(X, Y, True, True);
      HintWindows.Add(HintWindow);
    end;
  end
  else if Length(g_GodBlessItems[Index].s.Name) = 0 then begin
    HintWindows.Clear;

    ItemDesc := GetGodBlessItem(IntToStr(Index + 1 + 100));
    if (ItemDesc <> nil) and (ItemDesc.Count > 0) then begin
      HintWindow := THintWindow.Create;

      HintLines := THintLines.Create;
      HintLines.Add(GetGodBlessItemCaption(Index), GetRGB(251), GetHintNameFontSize, GetHintNameFontStyle([fsBold]), GetHintNameFontStroke(True), GetHintNameFontName);
      HintWindow.Add(HintLines);

      if g_ClientConfig.boShowHintLines then begin
        HintLines := THintLines.Create;
        HintLines.Add('-');
        HintWindow.Add(HintLines);
      end;

      for I := 0 to ItemDesc.Count - 1 do begin
        HintLines := THintLines.Create;
        //HintLines.Add(ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]), 9, [], True);
        GetHitLines(HintLines, ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]));
        HintWindow.Add(HintLines);
      end;

      HintWindow.Show(X, Y, True, True);
      HintWindows.Add(HintWindow);
    end;
  end
  else begin
    g_MouseStateItem := g_GodBlessItems[Index];

    // 免得不停的重新创建提示窗口 chongchong 2015-01-22
    if (g_MouseStateItem.MakeIndex <> g_LastHintMakeIndex) then begin
      g_boShowBagInfo := False;

      with DSWGodBlessDlg do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left + 40
          else
            nX := vtRect.Right - 20;
        end;

        if Length(g_MouseStateItem.s.Name) = 0 then begin
          HintWindows.Show(nX, nY, '神佑袋物品', clWhite, False, DrawLeft);
        end
        else begin
          HintWindows.Clear;
          HintList := TList.Create;

          // 两列显示 piaoyun 2013-09-15
          if not g_ClientConfig.boSingleHint then begin
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then
                HintList.Add(HintWindow)
              else
                HintWindow.Free;
            end;

            if not g_ClientConfig.boTZSupportRenameItem then
              TzHintWindows := FrmDlg.GetTzItemHintWindow(0, g_MySelf.m_btSex, g_MouseStateItem.s.DBName)
            else
              TzHintWindows := FrmDlg.GetTzItemHintWindow(0, g_MySelf.m_btSex, g_MouseStateItem.s.Name);

            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end;
          end
          else begin
            // 单列显示 piaoyun 2013-09-15
            TzHintWindows := FrmDlg.GetTzItemHintWindowEx(0, g_MySelf.m_btSex, g_MySelf, @g_MouseStateItem, False);
            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end
            else begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then
                  HintList.Add(HintWindow)
                else
                  HintWindow.Free;
              end;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
          HintList.Free;
        end;
      end;
      g_LastHintMakeIndex := g_MouseStateItem.MakeIndex;
    end;
  end;
  {
  else
  begin
    g_boShowBagInfo := False;
    g_MouseStateItem.s.Name := '';
    DScreen.ClearHint;
    HintWindows.Clear;
  end;
  }
end;

procedure TStateWindows.DSWGodBlessItemPaint(Sender:TObject);
var
  Index:Integer;
  d:TTexture;
  R:TRect;
begin
  if not DSWGodBlessDlg.Visible then Exit;
  Index := TDxImageButton(Sender).Tag;
  R := TDxImageButton(Sender).VirtualRect;

  if Index >= 0 then begin
    if g_GodBlessItemsState[Index] = 0 then begin
      d := g_WNewopUIImages.Images[124];
      GameCanvas.Draw(R.Left + ((R.Right - R.Left) - d.Width) div 2,
        R.Top + ((R.Bottom - R.Top) - d.Height) div 2,
        d.ClientRect,
        d);
    end
    else
      FrmDlg.DrawBodyItem(TDxImageButton(Sender).VirtualRect, @g_GodBlessItems[Index].s, g_GodBlessItems[Index].btHeroM2Light, @g_GodBlessItemsEffect[Index], True);
  end;
end;

procedure TStateWindows.DSWGodBlessUpgradeClick(Sender:TObject; X, Y:Integer); stdcall;
begin
  frmMain.SendGodBlessUpgradeClick(Sender = DHeroSWGodBlessInfo);
end;

procedure TStateWindows.DHeroSWGodBlessItemClick(Sender:TObject; X, Y:Integer);
var
  SelIndex:Integer;
  Flag:Boolean;
  DButton:TDxImageButton;
begin
  if g_MySelf = nil then Exit;

  DButton := TDxImageButton(Sender);
  SelIndex := DButton.Tag;

  if g_boItemMoving then begin
    if g_HeroGodBlessItemsState[SelIndex] = 1 then begin
      Flag := False;
      if not (g_MovingItem.ItemType in [mtHeroBagItem]) then Exit;

      if (g_MovingItem.Item.S.Name = '') or (g_WaitingHeroUseItem.Item.S.Name <> '') then Exit;

      if (g_MovingItem.Index >= 0) and (g_MovingItem.Item.S.OverLap and 4 <> 0) and ((g_MovingItem.Item.S.Expand1 = SelIndex + 1) or (g_MovingItem.Item.S.Expand1 = 13)) then
        Flag := True;

      if Flag then begin
        if g_MovingItem.ItemType = mtHeroBagItem then begin
          ItemClickSound(g_MovingItem.Item.S);
          g_WaitingHeroUseItem := g_MovingItem;
          g_WaitingHeroUseItem.Index := SelIndex;

          frmMain.SendTakeOnGodBless(True, SelIndex, g_MovingItem.Item.MakeIndex, g_MovingItem.Item.S.Name);
          g_MovingItem.Item.S.Name := '';
          g_boItemMoving := False;
        end;
      end;
    end;
  end
  else begin
    if g_HeroGodBlessItemsState[SelIndex] <> 0 then begin
      if (g_MovingItem.Item.S.Name <> '') or (g_WaitingHeroUseItem.Item.S.Name <> '') then Exit;
      if (SelIndex >= 0) and (g_HeroGodBlessItems[SelIndex].S.Name <> '') then begin
        ItemClickSound(g_HeroGodBlessItems[SelIndex].S);
        g_MovingItem.Index := SelIndex;
        g_MovingItem.Item := g_HeroGodBlessItems[SelIndex];
        g_MovingItem.ItemType := mtHeroGodBlessItem;
        g_HeroGodBlessItems[SelIndex].S.Name := '';
        g_boItemMoving := True;
      end;
    end
    else begin
      frmMain.SendGodBlessItemClick(True, SelIndex);
    end;
  end;
end;

procedure TStateWindows.DHeroSWGodBlessItemMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
var
  I, Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  DrawLeft:Boolean;

  HintWindow:THintWindow;
  HintList:TList;
  ItemDesc:TStringList;
  HintLines:THintLines;
  TzHintWindows:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := TDxImageButton(Sender).Tag;
  if Index < 0 then Exit;

  if g_HeroGodBlessItemsState[Index] = 0 then begin
    HintWindows.Clear;

    ItemDesc := GetGodBlessItem(IntToStr(Index + 1));
    if (ItemDesc <> nil) and (ItemDesc.Count > 0) then begin
      HintWindow := THintWindow.Create;
      HintLines := THintLines.Create;
      HintLines.Add(GetGodBlessItemCaption(Index), GetRGB(251), GetHintNameFontSize, GetHintNameFontStyle([fsBold]), GetHintNameFontStroke(True), GetHintNameFontName);
      HintWindow.Add(HintLines);

      if g_ClientConfig.boShowHintLines then begin
        HintLines := THintLines.Create;
        HintLines.Add('-');
        HintWindow.Add(HintLines);
      end;

      for I := 0 to ItemDesc.Count - 1 do begin
        HintLines := THintLines.Create;
        //HintLines.Add(ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]), 9, [], True);
        GetHitLines(HintLines, ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]));
        HintWindow.Add(HintLines);
      end;
      HintWindow.Show(X, Y, True, True);
      HintWindows.Add(HintWindow);
    end;
  end
  else if Length(g_HeroGodBlessItems[Index].s.Name) = 0 then begin
    HintWindows.Clear;

    ItemDesc := GetGodBlessItem(IntToStr(Index + 1 + 100));
    if (ItemDesc <> nil) and (ItemDesc.Count > 0) then begin
      HintWindow := THintWindow.Create;

      HintLines := THintLines.Create;
      HintLines.Add(GetGodBlessItemCaption(Index), GetRGB(251), GetHintNameFontSize, GetHintNameFontStyle([fsBold]), GetHintNameFontStroke(True), GetHintNameFontName);
      HintWindow.Add(HintLines);

      if g_ClientConfig.boShowHintLines then begin
        HintLines := THintLines.Create;
        HintLines.Add('-');
        HintWindow.Add(HintLines);
      end;

      for I := 0 to ItemDesc.Count - 1 do begin
        HintLines := THintLines.Create;
        //HintLines.Add(ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]), 9, [], True);
        GetHitLines(HintLines, ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]));
        HintWindow.Add(HintLines);
      end;

      HintWindow.Show(X, Y, True, True);
      HintWindows.Add(HintWindow);
    end;
  end
  else {//if (Index >= 0) and (g_HeroGodBlessItems[Index].s.Name <> '') then} begin
    g_MouseStateItem := g_HeroGodBlessItems[Index];

    if (g_MouseStateItem.MakeIndex <> g_LastHintMakeIndex) then begin
      g_boShowBagInfo := False;

      with DHeroSWGodBlessDlg do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left + 40
          else
            nX := vtRect.Right - 20;
        end;

        if Length(g_MouseStateItem.s.Name) = 0 then begin
          HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
        end
        else begin
          HintWindows.Clear;
          HintList := TList.Create;

          // 两列显示 piaoyun 2013-09-15
          if not g_ClientConfig.boSingleHint then begin
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MyHero, @g_MouseStateItem, nX, nY, False, False, False);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then
                HintList.Add(HintWindow)
              else
                HintWindow.Free;
            end;

            if not g_ClientConfig.boTZSupportRenameItem then
              TzHintWindows := FrmDlg.GetTzItemHintWindow(1, g_MyHero.m_btSex, g_MouseStateItem.s.DBName)
            else
              TzHintWindows := FrmDlg.GetTzItemHintWindow(1, g_MyHero.m_btSex, g_MouseStateItem.s.Name);

            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end;
          end
          else begin
            // 单列显示 piaoyun 2013-09-15
            TzHintWindows := FrmDlg.GetTzItemHintWindowEx(1, g_MyHero.m_btSex, g_MyHero, @g_MouseStateItem, False);
            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end
            else begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MyHero, @g_MouseStateItem, nX, nY, False, False, False);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then
                  HintList.Add(HintWindow)
                else
                  HintWindow.Free;
              end;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
          HintList.Free;
        end;
      end;
      g_LastHintMakeIndex := g_MouseStateItem.MakeIndex;
    end;
  end;
  {
  else
  begin
    g_boShowBagInfo := False;
    g_MouseStateItem.s.Name := '';
    DScreen.ClearHint;
    HintWindows.Clear;
  end;
  }
end;

procedure TStateWindows.DHeroSWGodBlessItemPaint(Sender:TObject);
var
  Index:Integer;
  d:TTexture;
  R:TRect;
begin
  if not DHeroSWGodBlessDlg.Visible then Exit;
  Index := TDxImageButton(Sender).Tag;
  R := TDxImageButton(Sender).VirtualRect;

  if Index >= 0 then begin
    if g_HeroGodBlessItemsState[Index] = 0 then begin
      d := g_WNewopUIImages.Images[124];
      GameCanvas.Draw(R.Left + ((R.Right - R.Left) - d.Width) div 2,
        R.Top + ((R.Bottom - R.Top) - d.Height) div 2,
        d.ClientRect,
        d);
    end
    else
      FrmDlg.DrawBodyItem(TDxImageButton(Sender).VirtualRect, @g_HeroGodBlessItems[Index].s, g_HeroGodBlessItems[Index].btHeroM2Light, @g_HeroGodBlessItemsEffect[Index], True);
  end;
end;

procedure TStateWindows.DGodBlessItemUS1MouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
var
  I, Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  DrawLeft:Boolean;

  HintWindow:THintWindow;
  HintList:TList;
  TzHintWindows:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := TDxImageButton(Sender).Tag;

  if (Index >= 0) and (g_UserState1.GodBlessItemsState[Index] <> 0) and (g_UserState1.GodBlessItems[Index].s.Name <> '') then begin
    g_MouseStateItem := g_UserState1.GodBlessItems[Index];

    if (g_MouseStateItem.MakeIndex <> g_LastHintMakeIndex) then begin
      g_boShowBagInfo := False;

      with DGodBlessDlgUS1 do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left + 40
          else
            nX := vtRect.Right - 20;
        end;

        if Length(g_MouseStateItem.s.Name) = 0 then begin
          HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
        end
        else begin
          // 神佑显示套装备注 chongchong 2014-10-23
          {
          HintWindows.Clear;
          HintList := TList.Create;
          HintWindow := THintWindow(FrmDlg.GetMouseItemInfoWindow(g_MySelf, @g_MouseStateItem, nX, nY, False, False, False));
          HintList.Add(HintWindow);
          }

          HintWindows.Clear;
          HintList := TList.Create;
          // 两列显示 piaoyun 2013-09-15
          if not g_ClientConfig.boSingleHint then begin
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then
                HintList.Add(HintWindow)
              else
                HintWindow.Free;
            end;

            if not g_ClientConfig.boTZSupportRenameItem then
              TzHintWindows := FrmDlg.GetTzItemHintWindow(2, g_MySelf.m_btSex, g_MouseStateItem.s.DBName)
            else
              TzHintWindows := FrmDlg.GetTzItemHintWindow(2, g_MySelf.m_btSex, g_MouseStateItem.s.Name);

            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end;
          end
          else begin
            // 单列显示 piaoyun 2013-09-15
            TzHintWindows := FrmDlg.GetTzItemHintWindowEx(2, g_MySelf.m_btSex, g_MySelf, @g_MouseStateItem, False);
            if TzHintWindows <> nil then begin
              for I := 0 to TzHintWindows.Count - 1 do begin
                THintWindow(TzHintWindows.Items[I]).Show(nX, nY, False, False);
                HintList.Add(TzHintWindows.Items[I]);
              end;
              TzHintWindows.Free;
            end
            else begin
              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                ArrHintWindows[I] := THintWindow.Create;
              end;

              FrmDlg.GetMouseItemInfoWindow(ArrHintWindows, g_MySelf, @g_MouseStateItem, nX, nY, False, False, False);

              for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
                HintWindow := THintWindow(ArrHintWindows[I]);
                if HintWindow.Count > 0 then
                  HintList.Add(HintWindow)
                else
                  HintWindow.Free;
              end;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
          HintList.Free;
        end;
      end;

      g_LastHintMakeIndex := g_MouseStateItem.MakeIndex;
    end;
  end
  else begin
    g_boShowBagInfo := False;
    g_MouseStateItem.s.Name := '';
    DScreen.ClearHint;
    HintWindows.Clear;
  end;
end;

procedure TStateWindows.DGodBlessItemUS1Paint(Sender:TObject);
var
  Index:Integer;
  d:TTexture;
  R:TRect;
begin
  if not DGodBlessDlgUS1.Visible then Exit;
  Index := TDxImageButton(Sender).Tag;
  R := TDxImageButton(Sender).VirtualRect;

  if Index >= 0 then begin
    if g_UserState1.GodBlessItemsState[Index] = 0 then begin
      d := g_WNewopUIImages.Images[124];
      GameCanvas.Draw(R.Left + ((R.Right - R.Left) - d.Width) div 2,
        R.Top + ((R.Bottom - R.Top) - d.Height) div 2,
        d.ClientRect,
        d);
    end
    else
      FrmDlg.DrawBodyItem(TDxImageButton(Sender).VirtualRect, @g_UserState1.GodBlessItems[Index].s, g_UserState1.GodBlessItems[Index].btHeroM2Light, @g_GodBlessItemsUS1Effect[Index], True);
  end;
end;

procedure TStateWindows.DSWShowFashionClick(Sender:TObject; X, Y:Integer);
begin
  frmMain.SendSetShowFashion(DSWShowFashion.Checked, DHeroSWShowFashion.Checked);
  g_ConfigDlg.ConfigCheckeds[ckShowFashion] := DSWShowFashion.Checked;
end;

procedure TStateWindows.RefreshUpgradeButtons;
var
  I:Integer;
  ImageButtons:array[0..5] of TDxImageButton;
  ImageButton:TDxImageButton;
  Magic:PTClientMagic;
  CountOnPage:Integer;
begin
  // 176界面技能分页不对 chongchong 2013-11-08
  if g_ClientConfig.boUseOldSerialWindows and (g_ClientConfig.boStateWindowsType = 0) then
    CountOnPage := 5
  else
    CountOnPage := 6;

  ImageButtons[0] := DStMagBtn1;
  ImageButtons[1] := DStMagBtn2;
  ImageButtons[2] := DStMagBtn3;
  ImageButtons[3] := DStMagBtn4;
  ImageButtons[4] := DStMagBtn5;
  ImageButtons[5] := DStMagBtn6;
  for I := 0 to 5 do begin
    ImageButton := ImageButtons[I];
    ImageButton.Visible := False;
    ImageButton.Tag := 0;
  end;

  for I := MagicIndex to MagicIndex + CountOnPage - 1 do begin
    if (I >= 0) and (I < g_MagicList.Count) then begin
      Magic := g_MagicList.Items[I];

      if Magic <> nil then begin
        ImageButton := ImageButtons[I - MagicIndex];
        ImageButton.Visible := (Magic.Def.CanUpgrade <> 0) and (Magic.NewLevel < Magic.Def.MaxUpgradeLevel);
        ImageButton.Enabled := (Magic.Def.CanUpgrade = 1);
        ImageButton.Tag := Magic.Def.wMagicId;
      end;
    end;
  end;
end;

procedure TStateWindows.RefreshHeroUpgradeButtons;
var
  I:Integer;
  ImageButtons:array[0..5] of TDxImageButton;
  ImageButton:TDxImageButton;
  Magic:PTClientMagic;
  CountOnPage:Integer;
begin
  // 176界面技能分面不对 chongchong 2013-11-08
  if g_ClientConfig.boUseOldSerialWindows and (g_ClientConfig.boStateWindowsType = 0) then
    CountOnPage := 5
  else
    CountOnPage := 6;

  ImageButtons[0] := DHeroStMagBtn1;
  ImageButtons[1] := DHeroStMagBtn2;
  ImageButtons[2] := DHeroStMagBtn3;
  ImageButtons[3] := DHeroStMagBtn4;
  ImageButtons[4] := DHeroStMagBtn5;
  ImageButtons[5] := DHeroStMagBtn6;
  for I := 0 to 5 do begin
    ImageButton := ImageButtons[I];
    ImageButton.Visible := False;
    ImageButton.Tag := 0;
  end;

  for I := HeroMagicIndex to HeroMagicIndex + CountOnPage - 1 do begin
    if (I >= 0) and (I < g_HeroMagicList.Count) then begin
      Magic := g_HeroMagicList.Items[I];

      if Magic <> nil then begin
        ImageButton := ImageButtons[I - HeroMagicIndex];
        ImageButton.Visible := (Magic.Def.CanUpgrade <> 0) and (Magic.NewLevel < Magic.Def.MaxUpgradeLevel);
        ImageButton.Enabled := (Magic.Def.CanUpgrade = 1);
        ImageButton.Tag := Magic.Def.wMagicId;
      end;
    end;
  end;
end;

procedure TStateWindows.DStateTitleWinMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
begin
  DScreen.ClearHint;
  HintWindows.Clear;
  FengHaoHintWindow.Clear;
end;

procedure TStateWindows.DSTitleItemDescOnPaint(Sender:TObject);
var
  I:Integer;
  nY:Integer;
  HintLines:THintLines;
  PaintRect:TRect;
  DrawButton:TDxImageButton;
begin
  if FengHaoHintWindow = nil then Exit;
  DrawButton := Sender as TDxImageButton;
  PaintRect := DrawButton.VisibleRect;

  nY := PaintRect.Top;
  for I := 0 to FengHaoHintWindow.Count - 1 do begin
    HintLines := THintLines(FengHaoHintWindow.Items[I]);
    HintLines.Paint(PaintRect, Bounds(PaintRect.Left, nY, PaintRect.Right - PaintRect.Left, HintLines.ItemHeight));
    nY := nY + HintLines.ItemHeight;

    // 优化提示信息过多时占CPU资源 chongchong 2013-11-16
    if nY > SCREENHEIGHT then Break;
  end;
end;

procedure TStateWindows.DSTitleActiveClick(Sender:TObject; X, Y:Integer);
begin
  if (g_ActiveFengHaoIndex < 0) or (g_ActiveFengHaoIndex >= g_FengHaoItems.Count) then Exit;
  FrmMain.SendActiveFengHao(-1, False);
end;

procedure TStateWindows.DSTitleActiveMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  I, Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  D:TDxControl;

  DrawLeft:Boolean;
  HintWindow:THintWindow;

  ClientItem:TClientItem;

  HintList:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := g_ActiveFengHaoIndex;
  if (Index < 0) or (Index >= g_FengHaoItems.Count) then begin
    DScreen.ClearHint;
    HintWindows.Clear;
    Exit;
  end;

  ClientItem := PTClientItem(g_FengHaoItems.Items[Index])^;

  if (ClientItem.MakeIndex <> g_LastHintMakeIndex) then begin
    { TODO -c注释 -opiaoyun : 添加注释 装备栏信息显示悬浮新模式 【2013-4-24】}
    if (g_ClientConfig.btSuspensionShowItem > 0) or (g_ClientVersion > cvSerial) then {cvHero} begin
      g_boShowBagInfo := False;

      D := TDxControl(Sender);

      while True do begin
        if (D.Owner <> nil) and (D.Owner is TDxControlEngine) then
          break;
        D := D.Owner;
      end;

      // with Sender as TDxControl do begin
      // vtRect := D.VirtualRect;

      with D do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40; // + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left // + 40
          else
            nX := vtRect.Right;
        end;

        HintList := TList.Create;
        try
          if Length(ClientItem.s.Name) = 0 then begin
            HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
            Exit;
          end
          else begin
            HintWindows.Clear;

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseFengHaoItemInfoWindow(ArrHintWindows, g_MySelf, @ClientItem, nX, nY, False, False, DrawLeft);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then begin
                HintList.Add(HintWindow);
                HintWindow.Show(0, 0);
              end
              else
                HintWindow.Free;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;

        finally
          HintList.Free;
        end;
      end;
    end;

    g_LastHintMakeIndex := ClientItem.MakeIndex;
  end;
end;

procedure TStateWindows.DSTitleActiveDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  Index:Integer;
  Button:TDxImageButton;
  Images:TGameImages;
  d:TTexture;
  dx, dy:Integer;
begin
  if (g_ActiveFengHaoIndex < 0) or (g_ActiveFengHaoIndex >= g_FengHaoItems.Count) then Exit;
  if g_ClientConfig.nTitleFileIndex < 0 then Exit;
  if g_ClientConfig.nTitleFileIndex >= g_EffectImageList.Count then Exit;
  Button := Sender as TDxImageButton;

  ClientItem := g_FengHaoItems.Items[g_ActiveFengHaoIndex];
  if Button.MouseDowned then
    Index := ClientItem.s.Looks + 4
  else
    Index := ClientItem.s.Looks + 3;
  Images := TGameImages(g_EffectImageList.Objects[g_ClientConfig.nTitleFileIndex]);

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := Images.Grays[Index]
  else
    d := Images.Images[Index];

  if d <> nil then begin
    dx := Button.VirtualRect.Left + (Button.VirtualRect.Right - Button.VirtualRect.Left - d.Width) div 2;
    dy := Button.VirtualRect.Top + (Button.VirtualRect.Bottom - Button.VirtualRect.Top - d.Height) div 2;
    if d <> nil then GameCanvas.DrawBlend(dx, dy, d);
  end;
end;

procedure TStateWindows.DSTitleButtonClick(Sender:TObject; X, Y:Integer);
var
  ClientItem:PTClientItem;
  Index:Integer;
  Button:TDxImageButton;
begin
  if g_ClientConfig.nTitleFileIndex < 0 then Exit;
  if g_ClientConfig.nTitleFileIndex >= g_EffectImageList.Count then Exit;
  Button := Sender as TDxImageButton;
  Index := Button.Tag + FengHaoIndex;
  if (Index < 0) or (Index >= g_FengHaoItems.Count) then Exit;

  ClientItem := g_FengHaoItems.Items[Index];
  FrmMain.SendActiveFengHao(ClientItem.MakeIndex, False);
end;

procedure TStateWindows.DSTitleButtonMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  D:TDxControl;

  DrawLeft:Boolean;
  HintWindow:THintWindow;

  ClientItem:TClientItem;

  I:Integer;
  ItemDesc:TStringList;
  HintLines:THintLines;

  HintList:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := TDxImageButton(Sender).Tag + FengHaoIndex;
  if (Index < 0) or (Index >= g_FengHaoItems.Count) then begin
    g_LastHintMakeIndex := -1;
    DScreen.ClearHint;
    HintWindows.Clear;
    Exit;
  end;

  ClientItem := PTClientItem(g_FengHaoItems.Items[Index])^;

  if (ClientItem.MakeIndex <> g_LastHintMakeIndex) then begin
    { TODO -c注释 -opiaoyun : 添加注释 装备栏信息显示悬浮新模式 【2013-4-24】}
    if (g_ClientConfig.btSuspensionShowItem > 0) or (g_ClientVersion > cvSerial) then {cvHero} begin
      g_boShowBagInfo := False;

      D := TDxControl(Sender);

      while True do begin
        if (D.Owner <> nil) and (D.Owner is TDxControlEngine) then
          break;
        D := D.Owner;
      end;

      // with Sender as TDxControl do begin
      // vtRect := D.VirtualRect;

      with D do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40; // + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left // + 40
          else
            nX := vtRect.Right;
        end;

        HintList := TList.Create;
        try
          if Length(ClientItem.s.Name) = 0 then begin
            HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
            Exit;
          end
          else begin
            HintWindows.Clear;

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseFengHaoItemInfoWindow(ArrHintWindows, g_MySelf, @ClientItem, nX, nY, False, False, DrawLeft);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then begin
                HintList.Add(HintWindow);
                HintWindow.Show(0, 0);
              end
              else
                HintWindow.Free;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
        finally
          HintList.Free;
        end;
      end;
    end;

    FengHaoHintWindow.Clear;

    ItemDesc := GetFengHaoItem(ClientItem.S.DBName);
    if (ItemDesc <> nil) and (ItemDesc.Count > 0) then begin
      for I := 0 to ItemDesc.Count - 1 do begin
        HintLines := THintLines.Create;
        // HintLines.Add(ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]), 9, [], True);
        GetHitLines(HintLines, ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]));
        FengHaoHintWindow.Add(HintLines);
      end;
    end;
    g_LastHintMakeIndex := ClientItem.MakeIndex;
  end;
end;

procedure TStateWindows.DSTitleButtonDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  Index:Integer;
  Button:TDxImageButton;
  Images:TGameImages;
  d:TTexture;
  dx, dy:Integer;
begin
  if g_ClientConfig.nTitleFileIndex < 0 then Exit;
  if g_ClientConfig.nTitleFileIndex >= g_EffectImageList.Count then Exit;
  Button := Sender as TDxImageButton;
  Index := Button.Tag + FengHaoIndex;
  if (Index < 0) or (Index >= g_FengHaoItems.Count) then Exit;

  ClientItem := g_FengHaoItems.Items[Index];
  if Button.MouseDowned then
    Index := ClientItem.s.Looks + 2
  else
    Index := ClientItem.s.Looks + 1;
  Images := TGameImages(g_EffectImageList.Objects[g_ClientConfig.nTitleFileIndex]);

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := Images.Grays[Index]
  else
    d := Images.Images[Index];

  if d <> nil then begin
    dx := Button.VirtualRect.Left + (Button.VirtualRect.Right - Button.VirtualRect.Left - d.Width) div 2;
    dy := Button.VirtualRect.Top + (Button.VirtualRect.Bottom - Button.VirtualRect.Top - d.Height) div 2;
    if d <> nil then GameCanvas.DrawBlend(dx, dy, d);
  end;
end;

procedure TStateWindows.DSTitleNameActiveDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  lbl:TDxLabel;
  HGEFont:THGEFont;

  sName:string;
begin
  if (g_ActiveFengHaoIndex < 0) or (g_ActiveFengHaoIndex >= g_FengHaoItems.Count) then Exit;
  lbl := Sender as TDxLabel;
  ClientItem := g_FengHaoItems.Items[g_ActiveFengHaoIndex];

  sName := ProcessItemName(ClientItem.s.Name);
  if Length(sName) > 0 then begin
    HGEFont := TextureFonts.FindFont(g_sCurFontName, 11, [fsBold]);
    HGEFont.TextOut(lbl.VirtualRect.Left, lbl.VirtualRect.Top, sName);
  end;
end;

procedure TStateWindows.DSTitleNameDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  lbl:TDxLabel;
  HGEFont:THGEFont;
  Index:Integer;
  sName:string;
begin
  lbl := Sender as TDxLabel;
  Index := lbl.Tag + FengHaoIndex;
  if (Index < 0) or (Index >= g_FengHaoItems.Count) then Exit;
  ClientItem := g_FengHaoItems.Items[Index];

  sName := ProcessItemName(ClientItem.s.Name);
  if Length(sName) > 0 then begin
    HGEFont := TextureFonts.FindFont(g_sCurFontName, 9, []);
    HGEFont.TextOut(lbl.VirtualRect.Left, lbl.VirtualRect.Top, sName);
  end;
end;

procedure TStateWindows.DSTitlePageClick(Sender:TObject; X, Y:Integer);
var
  PageCount:Integer;
begin
  if g_ClientVersion <> cvMirNewUI205 then
    PageCount := 6
  else
    PageCount := 5;

  if Sender = DSTitlePageUp then begin
    if FengHaoIndex > 0 then
      Dec(FengHaoIndex, PageCount);
    if FengHaoIndex < 0 then FengHaoIndex := 0;
  end
  else begin
    if FengHaoIndex + PageCount < g_FengHaoItems.Count then begin
      Inc(FengHaoIndex, PageCount);
    end;
  end;
end;

procedure TStateWindows.DUSTitleActiveDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  Index:Integer;
  Button:TDxImageButton;
  Images:TGameImages;
  d:TTexture;
  dx, dy:Integer;
begin
  if (g_USActiveFengHaoIndex < 0) or (g_USActiveFengHaoIndex >= g_USFengHaoItems.Count) then Exit;
  if g_ClientConfig.nTitleFileIndex < 0 then Exit;
  if g_ClientConfig.nTitleFileIndex >= g_EffectImageList.Count then Exit;
  Button := Sender as TDxImageButton;

  ClientItem := g_USFengHaoItems.Items[g_USActiveFengHaoIndex];
  {
  if Button.MouseDowned then
    Index := ClientItem.s.Looks + 4
  else
  }
  Index := ClientItem.s.Looks + 3;

  Images := TGameImages(g_EffectImageList.Objects[g_ClientConfig.nTitleFileIndex]);

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := Images.Grays[Index]
  else
    d := Images.Images[Index];

  if d <> nil then begin
    dx := Button.VirtualRect.Left + (Button.VirtualRect.Right - Button.VirtualRect.Left - d.Width) div 2;
    dy := Button.VirtualRect.Top + (Button.VirtualRect.Bottom - Button.VirtualRect.Top - d.Height) div 2;
    if d <> nil then GameCanvas.DrawBlend(dx, dy, d);
  end;
end;

procedure TStateWindows.DUSTitleAcitveMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  I, Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  D:TDxControl;

  DrawLeft:Boolean;
  HintWindow:THintWindow;

  ClientItem:TClientItem;

  HintList:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := g_USActiveFengHaoIndex;

  if (Index < 0) or (Index >= g_USFengHaoItems.Count) then begin
    g_LastHintMakeIndex := -1;
    DScreen.ClearHint;
    HintWindows.Clear;
    Exit;
  end;

  ClientItem := PTClientItem(g_USFengHaoItems.Items[Index])^;

  { TODO -c注释 -opiaoyun : 添加注释 装备栏信息显示悬浮新模式 【2013-4-24】}
  if (ClientItem.MakeIndex <> g_LastHintMakeIndex) then begin
    if (g_ClientConfig.btSuspensionShowItem > 0) or (g_ClientVersion > cvSerial) then {cvHero} begin
      g_boShowBagInfo := False;

      D := TDxControl(Sender);

      while True do begin
        if (D.Owner <> nil) and (D.Owner is TDxControlEngine) then
          break;
        D := D.Owner;
      end;

      // with Sender as TDxControl do begin
      // vtRect := D.VirtualRect;

      with D do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40; // + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left // + 40
          else
            nX := vtRect.Right;
        end;

        HintList := TList.Create;
        try
          if Length(ClientItem.s.Name) = 0 then begin
            HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
            Exit;
          end
          else begin
            HintWindows.Clear;

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseFengHaoItemInfoWindow(ArrHintWindows, g_MySelf, @ClientItem, nX, nY, False, False, DrawLeft);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then begin
                HintList.Add(HintWindow);
                HintWindow.Show(0, 0);
              end
              else
                HintWindow.Free;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
        finally
          HintList.Free;
        end;
      end;
    end;
    g_LastHintMakeIndex := ClientItem.MakeIndex;
  end;
end;

procedure TStateWindows.DUSTitleButtonMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  D:TDxControl;

  DrawLeft:Boolean;
  HintWindow:THintWindow;

  ClientItem:TClientItem;

  I:Integer;
  ItemDesc:TStringList;
  HintLines:THintLines;

  HintList:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := TDxImageButton(Sender).Tag + USFengHaoIndex;
  if (Index < 0) or (Index >= g_USFengHaoItems.Count) then begin
    g_LastHintMakeIndex := -1;
    DScreen.ClearHint;
    HintWindows.Clear;
    Exit;
  end;

  ClientItem := PTClientItem(g_USFengHaoItems.Items[Index])^;

  { TODO -c注释 -opiaoyun : 添加注释 装备栏信息显示悬浮新模式 【2013-4-24】}
  if (ClientItem.MakeIndex <> g_LastHintMakeIndex) then begin
    if (g_ClientConfig.btSuspensionShowItem > 0) or (g_ClientVersion > cvSerial) then {cvHero} begin
      g_boShowBagInfo := False;

      D := TDxControl(Sender);

      while True do begin
        if (D.Owner <> nil) and (D.Owner is TDxControlEngine) then
          break;
        D := D.Owner;
      end;

      // with Sender as TDxControl do begin
      // vtRect := D.VirtualRect;

      with D do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40; // + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left // + 40
          else
            nX := vtRect.Right;
        end;

        HintList := TList.Create;
        try
          if Length(ClientItem.s.Name) = 0 then begin
            HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
            Exit;
          end
          else begin
            HintWindows.Clear;

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseFengHaoItemInfoWindow(ArrHintWindows, g_MySelf, @ClientItem, nX, nY, False, False, DrawLeft);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then begin
                HintList.Add(HintWindow);
                HintWindow.Show(0, 0);
              end
              else
                HintWindow.Free;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
        finally
          HintList.Free;
        end;
      end;
    end;

    FengHaoHintWindow.Clear;

    ItemDesc := GetFengHaoItem(ClientItem.S.DBName);
    if (ItemDesc <> nil) and (ItemDesc.Count > 0) then begin
      for I := 0 to ItemDesc.Count - 1 do begin
        HintLines := THintLines.Create;
        // HintLines.Add(ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]), 9, [], True);
        GetHitLines(HintLines, ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]));
        FengHaoHintWindow.Add(HintLines);
      end;
    end;

    g_LastHintMakeIndex := ClientItem.MakeIndex;
  end;
end;

procedure TStateWindows.DUSTitleButtonDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  Index:Integer;
  Button:TDxImageButton;
  Images:TGameImages;
  d:TTexture;
  dx, dy:Integer;
begin
  if g_ClientConfig.nTitleFileIndex < 0 then Exit;
  if g_ClientConfig.nTitleFileIndex >= g_EffectImageList.Count then Exit;
  Button := Sender as TDxImageButton;
  Index := Button.Tag + USFengHaoIndex;
  if (Index < 0) or (Index >= g_USFengHaoItems.Count) then Exit;

  ClientItem := g_USFengHaoItems.Items[Index];
  {
  if Button.MouseDowned then
    Index := ClientItem.s.Looks + 2
  else
  }
  Index := ClientItem.s.Looks + 1;
  Images := TGameImages(g_EffectImageList.Objects[g_ClientConfig.nTitleFileIndex]);

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := Images.Grays[Index]
  else
    d := Images.Images[Index];

  if d <> nil then begin
    dx := Button.VirtualRect.Left + (Button.VirtualRect.Right - Button.VirtualRect.Left - d.Width) div 2;
    dy := Button.VirtualRect.Top + (Button.VirtualRect.Bottom - Button.VirtualRect.Top - d.Height) div 2;
    if d <> nil then GameCanvas.DrawBlend(dx, dy, d);
  end;
end;

procedure TStateWindows.DUSTitleNameActiveDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  lbl:TDxLabel;
  HGEFont:THGEFont;
begin
  if (g_USActiveFengHaoIndex < 0) or (g_USActiveFengHaoIndex >= g_USFengHaoItems.Count) then Exit;
  lbl := Sender as TDxLabel;
  ClientItem := g_USFengHaoItems.Items[g_USActiveFengHaoIndex];
  if Length(ClientItem.s.Name) > 0 then begin
    HGEFont := TextureFonts.FindFont(g_sCurFontName, 11, [fsBold]);
    HGEFont.TextOut(lbl.VirtualRect.Left, lbl.VirtualRect.Top, ClientItem.s.Name);
  end;
end;

procedure TStateWindows.DUSTitleNameDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  lbl:TDxLabel;
  HGEFont:THGEFont;
  Index:Integer;
begin
  lbl := Sender as TDxLabel;
  Index := lbl.Tag + USFengHaoIndex;
  if (Index < 0) or (Index >= g_USFengHaoItems.Count) then Exit;
  ClientItem := g_USFengHaoItems.Items[Index];
  if Length(ClientItem.s.Name) > 0 then begin
    HGEFont := TextureFonts.FindFont(g_sCurFontName, 9, []);
    HGEFont.TextOut(lbl.VirtualRect.Left, lbl.VirtualRect.Top, ClientItem.s.Name);
  end;
end;

procedure TStateWindows.DUSTitlePageClick(Sender:TObject; X, Y:Integer);
var
  PageCount:Integer;
begin
  if g_ClientVersion <> cvMirNewUI205 then
    PageCount := 6
  else
    PageCount := 5;

  if Sender = DUSTitlePageUp then begin
    if USFengHaoIndex > 0 then
      Dec(USFengHaoIndex, PageCount);
    if USFengHaoIndex < 0 then USFengHaoIndex := 0;
  end
  else begin
    if USFengHaoIndex + PageCount < g_USFengHaoItems.Count then begin
      Inc(USFengHaoIndex, PageCount);
    end;
  end;
end;

procedure TStateWindows.DUSTitleItemDescOnPaint(Sender:TObject);
var
  I:Integer;
  nY:Integer;
  HintLines:THintLines;
  PaintRect:TRect;
  DrawButton:TDxImageButton;
begin
  if FengHaoHintWindow = nil then Exit;
  DrawButton := Sender as TDxImageButton;
  PaintRect := DrawButton.VisibleRect;

  nY := PaintRect.Top;
  for I := 0 to FengHaoHintWindow.Count - 1 do begin
    HintLines := THintLines(FengHaoHintWindow.Items[I]);
    HintLines.Paint(PaintRect, Bounds(PaintRect.Left, nY, PaintRect.Right - PaintRect.Left, HintLines.ItemHeight));
    nY := nY + HintLines.ItemHeight;

    // 优化提示信息过多时占CPU资源 chongchong 2013-11-16
    if nY > SCREENHEIGHT then Break;
  end;
end;

procedure TStateWindows.DSHeroTitleActiveClick(Sender:TObject; X, Y:Integer);
begin
  if (g_HeroActiveFengHaoIndex < 0) or (g_HeroActiveFengHaoIndex >= g_HeroFengHaoItems.Count) then Exit;
  FrmMain.SendActiveFengHao(-1, True);
end;

procedure TStateWindows.DSHeroTitleActiveMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  I, Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  D:TDxControl;

  DrawLeft:Boolean;
  HintWindow:THintWindow;

  ClientItem:TClientItem;

  HintList:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := g_HeroActiveFengHaoIndex;
  if (Index < 0) or (Index >= g_HeroFengHaoItems.Count) then begin
    g_LastHintMakeIndex := -1;
    DScreen.ClearHint;
    HintWindows.Clear;
    Exit;
  end;

  ClientItem := PTClientItem(g_HeroFengHaoItems.Items[Index])^;

  { TODO -c注释 -opiaoyun : 添加注释 装备栏信息显示悬浮新模式 【2013-4-24】}
  if (ClientItem.MakeIndex <> g_LastHintMakeIndex) then begin
    if (g_ClientConfig.btSuspensionShowItem > 0) or (g_ClientVersion > cvSerial) then {cvHero} begin
      g_boShowBagInfo := False;

      D := TDxControl(Sender);

      while True do begin
        if (D.Owner <> nil) and (D.Owner is TDxControlEngine) then
          break;
        D := D.Owner;
      end;

      // with Sender as TDxControl do begin
      // vtRect := D.VirtualRect;

      with D do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40; // + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left // + 40
          else
            nX := vtRect.Right;
        end;

        HintList := TList.Create;
        try
          if Length(ClientItem.s.Name) = 0 then begin
            HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
            Exit;
          end
          else begin
            HintWindows.Clear;

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseFengHaoItemInfoWindow(ArrHintWindows, g_MySelf, @ClientItem, nX, nY, False, False, DrawLeft);

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then begin
                HintList.Add(HintWindow);
                HintWindow.Show(0, 0);
              end
              else
                HintWindow.Free;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
        finally
          HintList.Free;
        end;
      end;
    end;

    g_LastHintMakeIndex := ClientItem.MakeIndex;
  end;
end;

procedure TStateWindows.DSHeroTitleActiveDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  Index:Integer;
  Button:TDxImageButton;
  Images:TGameImages;
  d:TTexture;
  dx, dy:Integer;
begin
  if (g_HeroActiveFengHaoIndex < 0) or (g_HeroActiveFengHaoIndex >= g_HeroFengHaoItems.Count) then Exit;
  if g_ClientConfig.nTitleFileIndex < 0 then Exit;
  if g_ClientConfig.nTitleFileIndex >= g_EffectImageList.Count then Exit;
  Button := Sender as TDxImageButton;

  ClientItem := g_HeroFengHaoItems.Items[g_HeroActiveFengHaoIndex];
  if Button.MouseDowned then
    Index := ClientItem.s.Looks + 4
  else
    Index := ClientItem.s.Looks + 3;
  Images := TGameImages(g_EffectImageList.Objects[g_ClientConfig.nTitleFileIndex]);

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := Images.Grays[Index]
  else
    d := Images.Images[Index];

  if d <> nil then begin
    dx := Button.VirtualRect.Left + (Button.VirtualRect.Right - Button.VirtualRect.Left - d.Width) div 2;
    dy := Button.VirtualRect.Top + (Button.VirtualRect.Bottom - Button.VirtualRect.Top - d.Height) div 2;
    if d <> nil then GameCanvas.DrawBlend(dx, dy, d);
  end;
end;

procedure TStateWindows.DSHeroTitleButtonClick(Sender:TObject; X, Y:Integer);
var
  ClientItem:PTClientItem;
  Index:Integer;
  Button:TDxImageButton;
begin
  if g_ClientConfig.nTitleFileIndex < 0 then Exit;
  if g_ClientConfig.nTitleFileIndex >= g_EffectImageList.Count then Exit;
  Button := Sender as TDxImageButton;
  Index := Button.Tag + HeroFengHaoIndex;
  if (Index < 0) or (Index >= g_HeroFengHaoItems.Count) then Exit;

  ClientItem := g_HeroFengHaoItems.Items[Index];
  FrmMain.SendActiveFengHao(ClientItem.MakeIndex, True);
end;

procedure TStateWindows.DSHeroTitleButtonMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  Index, nX, nY, nWidth, nHeight:Integer;
  vtRect:TRect;
  CtrlRect:TRect;

  D:TDxControl;

  DrawLeft:Boolean;
  HintWindow:THintWindow;

  ClientItem:TClientItem;

  I:Integer;
  ItemDesc:TStringList;
  HintLines:THintLines;

  HintList:TList;
  ArrHintWindows:TArrHintWindows;
begin
  Index := TDxImageButton(Sender).Tag + HeroFengHaoIndex;
  if (Index < 0) or (Index >= g_HeroFengHaoItems.Count) then begin
    g_LastHintMakeIndex := -1;
    DScreen.ClearHint;
    HintWindows.Clear;
    Exit;
  end;

  ClientItem := PTClientItem(g_HeroFengHaoItems.Items[Index])^;

  { TODO -c注释 -opiaoyun : 添加注释 装备栏信息显示悬浮新模式 【2013-4-24】}
  if (ClientItem.MakeIndex <> g_LastHintMakeIndex) then begin
    if (g_ClientConfig.btSuspensionShowItem > 0) or (g_ClientVersion > cvSerial) then {cvHero} begin
      g_boShowBagInfo := False;

      D := TDxControl(Sender);

      while True do begin
        if (D.Owner <> nil) and (D.Owner is TDxControlEngine) then
          break;
        D := D.Owner;
      end;

      // with Sender as TDxControl do begin
      // vtRect := D.VirtualRect;

      with D do begin
        if g_ClientConfig.boHintWithMouse then begin
          CtrlRect := TDxControl(Sender).VirtualRect;
          vtRect := VirtualRect;
          nY := CtrlRect.Bottom;
          nX := CtrlRect.Left;
          DrawLeft := False;
        end
        else begin
          vtRect := VirtualRect;
          nY := vtRect.Top + 40; // + 40;

          DrawLeft := vtRect.Right + 160 > SCREENWIDTH;
          if DrawLeft then
            nX := vtRect.Left // + 40
          else
            nX := vtRect.Right;
        end;

        HintList := TList.Create;
        try
          if Length(ClientItem.s.Name) = 0 then begin
            HintWindows.Show(nX, nY, GetUseItemName(Index), clWhite, False, DrawLeft);
            Exit;
          end
          else begin
            HintWindows.Clear;

            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              ArrHintWindows[I] := THintWindow.Create;
            end;

            FrmDlg.GetMouseFengHaoItemInfoWindow(ArrHintWindows, g_MySelf, @ClientItem, nX, nY, False, False, DrawLeft);
            for I := Low(ArrHintWindows) to High(ArrHintWindows) do begin
              HintWindow := THintWindow(ArrHintWindows[I]);
              if HintWindow.Count > 0 then begin
                HintList.Add(HintWindow);
                HintWindow.Show(0, 0);
              end
              else
                HintWindow.Free;
            end;
          end;

          nWidth := 0;
          nHeight := 0;

          for I := 0 to HintList.Count - 1 do begin
            HintWindow := THintWindow(HintList.Items[I]);
            nWidth := nWidth + HintWindow.Width;
            nHeight := Max(HintWindow.Height, nHeight);
          end;

          if g_ClientConfig.boHintWithMouse then begin
            if CtrlRect.Left >= (SCREENWIDTH - Width) then begin
              nX := CtrlRect.Left; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := CtrlRect.Right;
              DrawLeft := False;
            end;
          end
          else begin
            if vtRect.Left >= (SCREENWIDTH - Width) div 2 then begin
              nX := vtRect.Left - nWidth; // HintWindow.Width;
              DrawLeft := True;
            end
            else begin
              nX := vtRect.Right;
              DrawLeft := False;
            end;
          end;

          if nX + nWidth > SCREENWIDTH then
            nX := SCREENWIDTH - nWidth;

          if nY + nHeight > SCREENHEIGHT then
            nY := SCREENHEIGHT - nHeight;

          if nX < 0 then nX := 0;
          if nY < 0 then nY := 0;
          if DrawLeft then begin
            for I := HintList.Count - 1 downto 0 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end
          else begin
            for I := 0 to HintList.Count - 1 do begin
              HintWindow := THintWindow(HintList.Items[I]);
              HintWindow.X := nX;
              HintWindow.Y := nY;
              Inc(nX, HintWindow.Width);
              HintWindows.Add(HintWindow);
            end;
          end;
        finally
          HintList.Free;
        end;
      end;
    end;

    FengHaoHintWindow.Clear;

    ItemDesc := GetFengHaoItem(ClientItem.S.DBName);
    if (ItemDesc <> nil) and (ItemDesc.Count > 0) then begin
      for I := 0 to ItemDesc.Count - 1 do begin
        HintLines := THintLines.Create;
        // HintLines.Add(ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]), 9, [], True);
        GetHitLines(HintLines, ItemDesc.Strings[I], TColor(ItemDesc.Objects[I]));
        FengHaoHintWindow.Add(HintLines);
      end;
    end;

    g_LastHintMakeIndex := ClientItem.MakeIndex;
  end;
end;

procedure TStateWindows.DSHeroTitleButtonDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  Index:Integer;
  Button:TDxImageButton;
  Images:TGameImages;
  d:TTexture;
  dx, dy:Integer;
begin
  if g_ClientConfig.nTitleFileIndex < 0 then Exit;
  if g_ClientConfig.nTitleFileIndex >= g_EffectImageList.Count then Exit;
  Button := Sender as TDxImageButton;
  Index := Button.Tag + HeroFengHaoIndex;
  if (Index < 0) or (Index >= g_HeroFengHaoItems.Count) then Exit;

  ClientItem := g_HeroFengHaoItems.Items[Index];
  if Button.MouseDowned then
    Index := ClientItem.s.Looks + 2
  else
    Index := ClientItem.s.Looks + 1;
  Images := TGameImages(g_EffectImageList.Objects[g_ClientConfig.nTitleFileIndex]);

  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    d := Images.Grays[Index]
  else
    d := Images.Images[Index];

  if d <> nil then begin
    dx := Button.VirtualRect.Left + (Button.VirtualRect.Right - Button.VirtualRect.Left - d.Width) div 2;
    dy := Button.VirtualRect.Top + (Button.VirtualRect.Bottom - Button.VirtualRect.Top - d.Height) div 2;
    if d <> nil then GameCanvas.DrawBlend(dx, dy, d);
  end;
end;

procedure TStateWindows.DSHeroTitleNameActiveDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  lbl:TDxLabel;
  HGEFont:THGEFont;
begin
  if (g_HeroActiveFengHaoIndex < 0) or (g_HeroActiveFengHaoIndex >= g_HeroFengHaoItems.Count) then Exit;
  lbl := Sender as TDxLabel;
  ClientItem := g_HeroFengHaoItems.Items[g_HeroActiveFengHaoIndex];
  if Length(ClientItem.s.Name) > 0 then begin
    HGEFont := TextureFonts.FindFont(g_sCurFontName, 11, [fsBold]);
    HGEFont.TextOut(lbl.VirtualRect.Left, lbl.VirtualRect.Top, ClientItem.s.Name);
  end;
end;

procedure TStateWindows.DSHeroTitleNameDirectPaint(Sender:TObject);
var
  ClientItem:PTClientItem;
  lbl:TDxLabel;
  HGEFont:THGEFont;
  Index:Integer;
begin
  lbl := Sender as TDxLabel;
  Index := lbl.Tag + HeroFengHaoIndex;
  if (Index < 0) or (Index >= g_HeroFengHaoItems.Count) then Exit;
  ClientItem := g_HeroFengHaoItems.Items[Index];
  if Length(ClientItem.s.Name) > 0 then begin
    HGEFont := TextureFonts.FindFont(g_sCurFontName, 9, []);
    HGEFont.TextOut(lbl.VirtualRect.Left, lbl.VirtualRect.Top, ClientItem.s.Name);
  end;
end;

procedure TStateWindows.DSHeroTitlePageClick(Sender:TObject; X, Y:Integer);
var
  PageCount:Integer;
begin
  if g_ClientVersion <> cvMirNewUI205 then
    PageCount := 6
  else
    PageCount := 5;

  if Sender = DSHeroTitlePageUp then begin
    if HeroFengHaoIndex > 0 then
      Dec(HeroFengHaoIndex, PageCount);
    if HeroFengHaoIndex < 0 then HeroFengHaoIndex := 0;
  end
  else begin
    if HeroFengHaoIndex + PageCount < g_HeroFengHaoItems.Count then begin
      Inc(HeroFengHaoIndex, PageCount);
    end;
  end;
end;

procedure TStateWindows.RefreshFashionJewelryInfo;
begin
  if DStateWin.Visible then
    RefreshMyFashionJewelryInfo;

  if DHeroStateWin.Visible then
    RefreshHeroFashionJewelryInfo;

  if DUserState1.Visible then
    RefreshUserFashionJewelryInfo;
end;

procedure TStateWindows.RefreshMyFashionJewelryInfo;
begin
  DStateFashion.IsMale := g_MySelf.m_btSex = 0;
  DStateFashion.UseSetting2 := not g_ClientConfig.boFashionJewelryOpen;

  DSWFashionHelmet.Visible := g_ClientConfig.boFashionJewelryOpen;
  DSWFashionNecklace.Visible := g_ClientConfig.boFashionJewelryOpen;
  DSWFashionArmRingR.Visible := g_ClientConfig.boFashionJewelryOpen;
  DSWFashionArmRingL.Visible := g_ClientConfig.boFashionJewelryOpen;
  DSWFashionRingR.Visible := g_ClientConfig.boFashionJewelryOpen;
  DSWFashionRingL.Visible := g_ClientConfig.boFashionJewelryOpen;
  DSWFashionLight.Visible := g_ClientConfig.boFashionJewelryOpen;
  DSWFashionBelt.Visible := g_ClientConfig.boFashionJewelryOpen;
  DSWFashionBoots.Visible := g_ClientConfig.boFashionJewelryOpen;
  DSWFashionDrum.Visible := g_ClientConfig.boFashionJewelryOpen;
end;

procedure TStateWindows.RefreshHeroFashionJewelryInfo;
begin
  DHeroStateFashion.IsMale := g_MyHero.m_btSex = 0;
  DHeroStateFashion.UseSetting2 := not g_ClientConfig.boFashionJewelryOpen;

  DHeroSWFashionHelmet.Visible := g_ClientConfig.boFashionJewelryOpen;
  DHeroSWFashionNecklace.Visible := g_ClientConfig.boFashionJewelryOpen;
  DHeroSWFashionArmRingR.Visible := g_ClientConfig.boFashionJewelryOpen;
  DHeroSWFashionArmRingL.Visible := g_ClientConfig.boFashionJewelryOpen;
  DHeroSWFashionRingR.Visible := g_ClientConfig.boFashionJewelryOpen;
  DHeroSWFashionRingL.Visible := g_ClientConfig.boFashionJewelryOpen;
  DHeroSWFashionLight.Visible := g_ClientConfig.boFashionJewelryOpen;
  DHeroSWFashionBelt.Visible := g_ClientConfig.boFashionJewelryOpen;
  DHeroSWFashionBoots.Visible := g_ClientConfig.boFashionJewelryOpen;
  DHeroSWFashionDrum.Visible := g_ClientConfig.boFashionJewelryOpen;
end;

procedure TStateWindows.RefreshUserFashionJewelryInfo;
begin
  DUserState1StateFashion.IsMale := pTHumFeature(@g_UserState1.feature.Buffer).btGender = 0;
  DUserState1StateFashion.UseSetting2 := not g_ClientConfig.boFashionJewelryOpen;

  DFashionHelmetUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
  DFashionNecklaceUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
  DFashionArmRingRUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
  DFashionArmRingLUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
  DFashionRingRUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
  DFashionRingLUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
  DFashionLightUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
  DFashionBeltUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
  DFashionBootsUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
  DFashionDrumUS1.Visible := g_ClientConfig.boFashionJewelryOpen;
end;

procedure TStateWindows.UpdateSelectPetsInfo;
var
  GamePetData:pTClientGamePetData;
begin
  if (FSelectGamePetIndex < 0) or (FSelectGamePetIndex >= DSWPetsList.Count) then begin
    Exit;
  end;

  GamePetData := g_GamePetList.Items[FSelectGamePetIndex];

  DSWPetsName.Caption := GamePetData.sName;
  DSWPetsLevel.Caption := IntToStr(GamePetData.GamePetAbility.Level);
  DSWPetsExp.Caption := Format('%u/%u', [GamePetData.GamePetAbility.Exp, GamePetData.GamePetAbility.MaxExp]);
  DSWPetsHP.Caption := Format('%u/%u', [GamePetData.GamePetAbility.HP, GamePetData.GamePetAbility.MaxHP]);
  DSWPetsDC.Caption := Format('%d-%d', [GamePetData.GamePetAbility.DC1, GamePetData.GamePetAbility.DC2]);
  DSWPetsMC.Caption := Format('%d-%d', [GamePetData.GamePetAbility.MC1, GamePetData.GamePetAbility.MC2]);
  DSWPetsSC.Caption := Format('%d-%d', [GamePetData.GamePetAbility.SC1, GamePetData.GamePetAbility.SC2]);
  DSWPetsAC.Caption := Format('%d-%d', [GamePetData.GamePetAbility.AC1, GamePetData.GamePetAbility.AC2]);
  DSWPetsMAC.Caption := Format('%d-%d', [GamePetData.GamePetAbility.MAC1, GamePetData.GamePetAbility.MAC2]);

  DSWPetsRecall.Enabled := False;
  DSWPetsRetake.Enabled := False;
  DSWPetsFree.Enabled := False;

  if FSelectGamePetIndex = g_CurrentRecallGamePetIndex then begin
    DSWPetsRecall.Enabled := False;
    DSWPetsRetake.Enabled := True;
    DSWPetsFree.Enabled := True;
  end
  else begin
    DSWPetsRecall.Enabled := True;
    DSWPetsRetake.Enabled := False;
    DSWPetsFree.Enabled := True;
  end;
end;

procedure TStateWindows.DSWPetsDlgMouseMove(Sender:TObject; Shift:TShiftState; X, Y:Integer);
begin
  DSWPetsList.MoveViewItem := nil;
  DScreen.ClearHint;
  HintWindows.Clear;
  g_MouseStateItem.S.Name := '';
  g_boShowBagInfo := False;
  g_boShowHeroBagInfo := False;
end;

procedure TStateWindows.RefreshGamePetList;
var
  I:Integer;
  ListItem:TDxListItem;
  ViewItem:pTViewItem;

  GamePetData:pTClientGamePetData;
begin
  DSWPetsList.Clear;
  for I := 0 to g_GamePetList.Count - 1 do begin
    GamePetData := g_GamePetList.Items[I];

    ListItem := DSWPetsList.Add;

    ViewItem := ListItem.AddItem('', nil);

    ViewItem.Caption := '宠物名: ' + GamePetData.sName + sLineBreak + '宠物等级: ' + IntToStr(GamePetData.GamePetAbility.Level);
    ViewItem.Style := bsButton; //bsButton;                                                                   // bsRadio;
    ViewItem.Alignment := taCenter;
    ViewItem.ImageIndex.ImageType := NewopUI_Pak;
    ViewItem.ImageIndex.Up := 1050;
    ViewItem.ImageIndex.Hot := 1051;
    ViewItem.ImageIndex.Down := 1052;
    ViewItem.ImageIndex.Checked := 1052;

    ViewItem.Data := Pointer(I);
  end;

  DSWPetsViewBag.Enabled := g_GamePetList.Count > 0;
  if g_GamePetList.Count > 0 then
    UpdateSelectPetsInfo
  else begin
    DSWPetsName.Caption := '';
    DSWPetsLevel.Caption := '';
    DSWPetsExp.Caption := '';
    DSWPetsHP.Caption := '';
    DSWPetsDC.Caption := '';
    DSWPetsMC.Caption := '';
    DSWPetsSC.Caption := '';
    DSWPetsAC.Caption := '';
    DSWPetsMAC.Caption := '';
  end;
end;

procedure TStateWindows.OpenGamePetDlg;
var
  Msg:TDefaultMessage;
begin
  RefreshGamePetList;
  DSWPetsDlg.Visible := True;

  if (FSelectGamePetIndex >= 0) and (FSelectGamePetIndex < DSWPetsList.Count) then begin
    Msg := MakeDefaultMsg(CM_GAMEPET_SELECT_CHANGE, FSelectGamePetIndex, 0, 0, 0);
    FrmMain.SendSocket(EncodeMessage(Msg));
  end;
end;

procedure TStateWindows.DSWPetsClick(Sender:TObject; X, Y:Integer);
begin
  OpenGamePetDlg;
end;

procedure TStateWindows.DSWPetsCloseClick(Sender:TObject; X, Y:Integer);
begin
  DSWPetsDlg.Visible := False;
end;

procedure TStateWindows.DSWPetsButtonClick(Sender:TObject; X, Y:Integer);
var
  Msg:TDefaultMessage;
begin
  if Sender = DSWPetsTakeBag then begin
    Msg := MakeDefaultMsg(CM_GAMEPET_TOBAG, FSelectGamePetIndex, g_GamePetList.Count, 0, 0);
    FrmMain.SendSocket(EncodeMessage(Msg));
  end
  else if Sender = DSWPetsRecall then begin
    Msg := MakeDefaultMsg(CM_GAMEPET_RECALL, FSelectGamePetIndex, g_GamePetList.Count, 0, 0);
    FrmMain.SendSocket(EncodeMessage(Msg));
  end
  else if Sender = DSWPetsRetake then begin
    Msg := MakeDefaultMsg(CM_GAMEPET_RETAKE, FSelectGamePetIndex, g_GamePetList.Count, 0, 0);
    FrmMain.SendSocket(EncodeMessage(Msg));
  end
  else if Sender = DSWPetsFree then begin
    if FrmDlg.DMessageDlg('你确定要放生该宠物吗？', [mbOk, mbCancel]) = mrOk then begin
      Msg := MakeDefaultMsg(CM_GAMEPET_FREE, FSelectGamePetIndex, g_GamePetList.Count, 0, 0);
      FrmMain.SendSocket(EncodeMessage(Msg));
    end;
  end
  else if Sender = DSWPetsViewBag then begin
    DSWPetsBagDlg.Visible := True;
    DSWPetsBagDlg.Left := Max(0, DSWPetsDlg.Left - DSWPetsBagDlg.Width);
    DSWPetsBagDlg.Top := Max(0, DSWPetsDlg.Top);
  end;
end;

procedure TStateWindows.DSWPetsListViewItemClick(Sender:TObject; ARow, ACol:Integer; ListItem:TObject; ViewItem:Pointer);
var
  OldSelectGamePetIndex:Integer;
  Msg:TDefaultMessage;
begin
  OldSelectGamePetIndex := FSelectGamePetIndex;

  FSelectGamePetIndex := Integer(pTViewItem(ViewItem).Data);

  if FSelectGamePetIndex < 0 then FSelectGamePetIndex := 0;
  if FSelectGamePetIndex >= DSWPetsList.Count then
    FSelectGamePetIndex := DSWPetsList.Count - 1;

  if OldSelectGamePetIndex <> FSelectGamePetIndex then begin
    UpdateSelectPetsInfo;

    Msg := MakeDefaultMsg(CM_GAMEPET_SELECT_CHANGE, FSelectGamePetIndex, 0, 0, 0);
    FrmMain.SendSocket(EncodeMessage(Msg));
  end;
end;

procedure TStateWindows.DSWPetsShowStopPaint(Sender:TObject);
var
  GamePetData:pTClientGamePetData;
  Image:TGameImages;
  D:TTexture;
  nX, nY:Integer;
  vR:TRect;
begin
  if (FSelectGamePetIndex >= 0) and (FSelectGamePetIndex < DSWPetsList.Count) then begin
    vR := DSWPetsShow.VirtualRect;

    GamePetData := g_GamePetList.Items[FSelectGamePetIndex];
    if (GamePetData.ShowConfig.ShowCount1 = GamePetData.ShowConfig.ShowCount2) and
      (GamePetData.ShowConfig.ShowTime1 = GamePetData.ShowConfig.ShowTime2) then begin
      if GamePetData.ShowConfig.ShowCount1 > 0 then begin
        if FPetShowIndex1 < 0 then
          FPetShowIndex1 := 0
        else if FPetShowIndex1 >= GamePetData.ShowConfig.ShowCount1 then
          FPetShowIndex1 := 0;

        if MyGetTickCount - FPetShowTick1 >= Cardinal(GamePetData.ShowConfig.ShowTime1) then begin
          FPetShowTick1 := MyGetTickCount;
          Inc(FPetShowIndex1);
          if FPetShowIndex1 >= GamePetData.ShowConfig.ShowCount1 then
            FPetShowIndex1 := 0
        end;

        if (GamePetData.ShowConfig.ShowFile1 > 0) and (GamePetData.ShowConfig.ShowFile1 <= g_EffectImageList.Count) then
          Image := TGameImages(g_EffectImageList.Objects[GamePetData.ShowConfig.ShowFile1 - 1])
        else
          Image := g_WMonImages.Images[GamePetData.ShowConfig.wAppr];

        D := Image.GetCachedImage(GamePetData.ShowConfig.ShowStart1 + FPetShowIndex1, nX, nY);

        if D <> nil then
          GameCanvas.Draw(vR.Left + GamePetData.ShowConfig.ShowOffsetX1 + nX, vR.Top + GamePetData.ShowConfig.ShowOffsetY1 + nY, D);

        if (GamePetData.ShowConfig.ShowFile2 > 0) and (GamePetData.ShowConfig.ShowFile2 <= g_EffectImageList.Count) then
          Image := TGameImages(g_EffectImageList.Objects[GamePetData.ShowConfig.ShowFile2 - 1])
        else
          Image := g_WMonImages.Images[GamePetData.ShowConfig.wAppr];

        D := Image.GetCachedImage(GamePetData.ShowConfig.ShowStart2 + FPetShowIndex1, nX, nY);

        if D <> nil then
          GameCanvas.Draw(vR.Left + GamePetData.ShowConfig.ShowOffsetX2 + nX, vR.Top + GamePetData.ShowConfig.ShowOffsetY2 + nY, D);
      end;
    end
    else begin
      if GamePetData.ShowConfig.ShowCount1 > 0 then begin
        if FPetShowIndex1 < 0 then
          FPetShowIndex1 := 0
        else if FPetShowIndex1 >= GamePetData.ShowConfig.ShowCount1 then
          FPetShowIndex1 := 0;

        if MyGetTickCount - FPetShowTick1 >= Cardinal(GamePetData.ShowConfig.ShowTime1) then begin
          FPetShowTick1 := MyGetTickCount;
          Inc(FPetShowIndex1);
          if FPetShowIndex1 >= GamePetData.ShowConfig.ShowCount1 then
            FPetShowIndex1 := 0
        end;

        if (GamePetData.ShowConfig.ShowFile1 > 0) and (GamePetData.ShowConfig.ShowFile1 <= g_EffectImageList.Count) then
          Image := TGameImages(g_EffectImageList.Objects[GamePetData.ShowConfig.ShowFile1 - 1])
        else
          Image := g_WMonImages.Images[GamePetData.ShowConfig.wAppr];

        D := Image.GetCachedImage(GamePetData.ShowConfig.ShowStart1 + FPetShowIndex1, nX, nY);

        if D <> nil then
          GameCanvas.Draw(vR.Left + GamePetData.ShowConfig.ShowOffsetX1 + nX, vR.Top + GamePetData.ShowConfig.ShowOffsetY1 + nY, D);
      end;

      if GamePetData.ShowConfig.ShowCount2 > 0 then begin
        if FPetShowIndex2 < 0 then
          FPetShowIndex2 := 0
        else if FPetShowIndex2 >= GamePetData.ShowConfig.ShowCount2 then
          FPetShowIndex2 := 0;

        if MyGetTickCount - FPetShowTick2 >= Cardinal(GamePetData.ShowConfig.ShowTime2) then begin
          FPetShowTick2 := MyGetTickCount;
          Inc(FPetShowIndex2);
          if FPetShowIndex2 >= GamePetData.ShowConfig.ShowCount2 then
            FPetShowIndex2 := 0
        end;

        if (GamePetData.ShowConfig.ShowFile2 > 0) and (GamePetData.ShowConfig.ShowFile2 <= g_EffectImageList.Count) then
          Image := TGameImages(g_EffectImageList.Objects[GamePetData.ShowConfig.ShowFile2 - 1])
        else
          Image := g_WMonImages.Images[GamePetData.ShowConfig.wAppr];

        D := Image.GetCachedImage(GamePetData.ShowConfig.ShowStart2 + FPetShowIndex2, nX, nY);

        if D <> nil then
          GameCanvas.Draw(vR.Left + GamePetData.ShowConfig.ShowOffsetX2 + nX, vR.Top + GamePetData.ShowConfig.ShowOffsetY2 + nY, D);
      end;
    end;
  end;
end;

procedure TStateWindows.DSWPetsListViewItemPaint(Sender:TObject; ACol, ARow:Integer; ARect:TRect; ViewItem:Pointer; var PaintOverride:Boolean);
var
  Item:pTViewItem;
  FaceIndex:Integer;
  Texture:TTexture;
  PaintRect, vbRect, DestRect:TRect;
  Font:TDxFont;
  HGEFont:THGEFont;
  nX, nY, Index:Integer;
  S1, S2:string;
  Line2:TImageInfo;

  OldSelectGamePetIndex, ViewItemIndex:Integer;
begin
  Item := ViewItem;
  PaintOverride := True;
  ARect.Right := ARect.Right - DSWPetsList.ScrollSize;

  ViewItemIndex := Integer(pTViewItem(ViewItem).Data);

  vbRect := DSWPetsList.VirtualRect;
  if Item.ImageIndex.Image <> nil then begin
    FaceIndex := -1; //HZQ 20230524 初始化变量

    if Item.Style = bsButton then begin
      OldSelectGamePetIndex := FSelectGamePetIndex;

      if FSelectGamePetIndex < 0 then FSelectGamePetIndex := 0;
      if FSelectGamePetIndex >= DSWPetsList.Count then
        FSelectGamePetIndex := DSWPetsList.Count - 1;

      if OldSelectGamePetIndex <> FSelectGamePetIndex then begin
        UpdateSelectPetsInfo;
      end;

      if (Integer(Item.Data) = FSelectGamePetIndex) or (Item = DSWPetsList.DownViewItem) then begin
        FaceIndex := Item.ImageIndex.Down;
        if (FaceIndex < 0) and (Item.ImageIndex.Up >= 0) then
          FaceIndex := Item.ImageIndex.Up;
      end else if Item = DSWPetsList.MoveViewItem then begin
        FaceIndex := Item.ImageIndex.Hot;
        if (FaceIndex < 0) and (Item.ImageIndex.Up >= 0) then
          FaceIndex := Item.ImageIndex.Up;
      end else begin
        FaceIndex := Item.ImageIndex.Up;
      end;
    end;

    if FaceIndex >= 0 then begin
      Texture := Item.ImageIndex.Image.Images[FaceIndex];
      if Texture <> nil then begin
        PaintRect := Bounds(ARect.Left, ARect.Top + (DSWPetsList.ItemHeight - Texture.Height) div 2, Texture.Width, Texture.Height);
        DSWPetsList.DrawRect(PaintRect, ARect, vbRect, Texture);
      end;
    end;
  end;

  //GameCanvas.FillRectAlpha(ARect, Random(High(Integer)), 100);

  if Item.Caption <> '' then begin
    if Item = DSWPetsList.DownViewItem then begin
      Font := Item.Color.Down;
    end
    else if Item = DSWPetsList.MoveViewItem then begin
      Font := Item.Color.Hot;
    end
    else begin
      Font := Item.Color.Up;
    end;
    HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);
    if HGEFont <> nil then begin
      Index := Pos(sLineBreak, Item.Caption);
      if Index > 0 then begin
        S1 := Copy(Item.Caption, 1, Index - 1);
        S2 := Copy(Item.Caption, Index + 2, MaxInt);

        Item.CaptionTexture := HGEFont.GetImageInfo(S1);
        if Length(Item.CaptionTexture.ImageIndexs) > 0 then begin
          DestRect := Bounds(ARect.Left + (ARect.Right - ARect.Left - Item.CaptionTexture.Width) div 2, ARect.Top + (DSWPetsList.ItemHeight - Item.CaptionTexture.Height) div 2, Item.CaptionTexture.Width, Item.CaptionTexture.Height);

          PaintRect := Rect(0, 0, Item.CaptionTexture.Width, Item.CaptionTexture.Height);
          PaintRect := DSWPetsList.ReallyPaintRect(DestRect, PaintRect, ARect, vbRect, nX, nY);
          nY := nY - 10;
          if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
            if Font.Bold then begin
              HGEFont.TextRect(nX - 1, nY, PaintRect, Item.CaptionTexture.ImageIndexs, Font.BColor);
              HGEFont.TextRect(nX + 1, nY, PaintRect, Item.CaptionTexture.ImageIndexs, Font.BColor);
              HGEFont.TextRect(nX, nY - 1, PaintRect, Item.CaptionTexture.ImageIndexs, Font.BColor);
              HGEFont.TextRect(nX, nY + 1, PaintRect, Item.CaptionTexture.ImageIndexs, Font.BColor);

              if g_CurrentRecallGamePetIndex = ViewItemIndex then
                HGEFont.TextRect(nX, nY, PaintRect, Item.CaptionTexture.ImageIndexs, clYellow)
              else
                HGEFont.TextRect(nX, nY, PaintRect, Item.CaptionTexture.ImageIndexs, 9215927);
            end
            else begin
              if g_CurrentRecallGamePetIndex = ViewItemIndex then
                HGEFont.TextRect(nX, nY, PaintRect, Line2.ImageIndexs, clYellow)
              else
                HGEFont.TextRect(nX, nY, PaintRect, Item.CaptionTexture.ImageIndexs, 9215927);
            end;
          end;
        end;

        Line2 := HGEFont.GetImageInfo(S2);
        if Length(Line2.ImageIndexs) > 0 then begin
          DestRect := Bounds(ARect.Left + (ARect.Right - ARect.Left - Line2.Width) div 2, ARect.Top + (DSWPetsList.ItemHeight - Line2.Height) div 2, Line2.Width, Line2.Height);

          PaintRect := Rect(0, 0, Line2.Width, Line2.Height);
          PaintRect := DSWPetsList.ReallyPaintRect(DestRect, PaintRect, ARect, vbRect, nX, nY);
          nY := nY + 10;
          if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
            if Font.Bold then begin
              HGEFont.TextRect(nX - 1, nY, PaintRect, Line2.ImageIndexs, Font.BColor);
              HGEFont.TextRect(nX + 1, nY, PaintRect, Line2.ImageIndexs, Font.BColor);
              HGEFont.TextRect(nX, nY - 1, PaintRect, Line2.ImageIndexs, Font.BColor);
              HGEFont.TextRect(nX, nY + 1, PaintRect, Line2.ImageIndexs, Font.BColor);

              if g_CurrentRecallGamePetIndex = ViewItemIndex then
                HGEFont.TextRect(nX, nY, PaintRect, Line2.ImageIndexs, clYellow)
              else
                HGEFont.TextRect(nX, nY, PaintRect, Line2.ImageIndexs, 9215927);
            end
            else begin
              if g_CurrentRecallGamePetIndex = ViewItemIndex then
                HGEFont.TextRect(nX, nY, PaintRect, Line2.ImageIndexs, clYellow)
              else
                HGEFont.TextRect(nX, nY, PaintRect, Line2.ImageIndexs, 9215927);
            end;
          end;
        end;
      end;
    end;
  end;
end;

(*
procedure TStateWindows.DSWPetsItemClick(Sender: TObject; X, Y: Integer);
begin

end;

procedure TStateWindows.DSWPetsItemMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin

end;

procedure TStateWindows.DSWPetsItemPaint(Sender: TObject);
begin

end;
*)

procedure TStateWindows.DSWPetsMagicClick(Sender:TObject; X, Y:Integer);
begin

end;

procedure TStateWindows.DSWPetsMagicMouseMove(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  Ctrl:TDxControl;
  GamePetData:pTClientGamePetData;
  vR:TRect;
  Item:PTClientItem;
begin
  Ctrl := Sender as TDxControl;
  if (FSelectGamePetIndex >= 0) and (FSelectGamePetIndex < DSWPetsList.Count) and
    (Ctrl.Tag >= 0) and (Ctrl.Tag < MAX_GAMEPET_MAGIC_COUNT) then begin
    vR := Ctrl.VirtualRect;
    GamePetData := g_GamePetList.Items[FSelectGamePetIndex];

    if GamePetData.wMagics[Ctrl.Tag] > 0 then begin
      Item := GetStdItem(GamePetData.wMagics[Ctrl.Tag]);
      g_MouseItem := Item^;
      if (Item <> nil) and (Item.S.Name <> '') and (Item.S.StdMode = 93) then begin
        FrmDlg.ShowMouseItemInfo(g_MySelf, @g_MouseItem, vR.Right, vR.Top, True, 0, False, False, True);
      end;
    end
    else begin
      g_MouseItem.S.Name := '';
      DScreen.ClearHint;
      HintWindows.Clear;
    end;
  end
  else begin
    g_MouseItem.S.Name := '';
    DScreen.ClearHint;
    HintWindows.Clear;
  end;
end;

procedure TStateWindows.DSWPetsMagicPaint(Sender:TObject);
var
  Ctrl:TDxControl;
  GamePetData:pTClientGamePetData;
  vR:TRect;
  Item:PTClientItem;
begin
  Ctrl := Sender as TDxControl;
  if (FSelectGamePetIndex >= 0) and (FSelectGamePetIndex < DSWPetsList.Count) and
    (Ctrl.Tag >= 0) and (Ctrl.Tag < MAX_GAMEPET_MAGIC_COUNT) then begin
    vR := Ctrl.VirtualRect;
    GamePetData := g_GamePetList.Items[FSelectGamePetIndex];

    if GamePetData.wMagics[Ctrl.Tag] > 0 then begin
      Item := GetStdItem(GamePetData.wMagics[Ctrl.Tag]);
      if (Item <> nil) and (Item.S.Name <> '') and (Item.S.StdMode = 93) then begin
        FrmDlg.DrawGridItem(vR, Item, @g_PetMagicEffect);
      end;
    end;
  end;
end;

procedure TStateWindows.DSWPetsBagCloseClick(Sender:TObject; X,
  Y:Integer);
begin
  DSWPetsBagDlg.Visible := False;
end;

procedure TStateWindows.DSWPetsGridDblClick(Sender:TObject; X,
  Y:Integer);
var
  idx:Integer;
  keyvalue:TKeyBoardState;
  cu:TClientItem;
  DItemGrid:TDxImageGrid;
begin
  if g_MySelf = nil then Exit;

  DItemGrid := TDxImageGrid(Sender);
  idx := DItemGrid.Col + DItemGrid.row * DItemGrid.ColCount;
  if idx in [0..MAX_GAMEPET_BAG_COUNT - 1] then begin
    if g_PetItemArr[idx].S.Name <> '' then begin
      FillChar(keyvalue, SizeOf(TKeyBoardState), #0);
      GetKeyboardState(keyvalue);
      if keyvalue[VK_CONTROL] = $80 then begin
        cu := g_PetItemArr[idx];
        g_PetItemArr[idx].S.Name := '';
        AddPetItemBag(cu);
      end
      else if (g_PetItemArr[idx].S.StdMode in [0, 31, 93]) or
        ((g_PetItemArr[idx].S.StdMode = 49) and (g_PetItemArr[idx].Dura >= g_PetItemArr[idx].DuraMax)) then begin
        frmMain.PetUseItem(idx);
      end;
    end
    else begin
      if g_boItemMoving and (g_MovingItem.Item.S.Name <> '') then begin
        FillChar(keyvalue, SizeOf(TKeyBoardState), #0);
        GetKeyboardState(keyvalue);
        if keyvalue[VK_CONTROL] = $80 then begin
          cu := g_MovingItem.Item;
          g_MovingItem.Item.S.Name := '';
          g_boItemMoving := False;
          AddPetItemBag(cu);
        end
        else if (g_MovingItem.Index = idx) and
          (
          (g_MovingItem.Item.S.StdMode in [0, 31, 93]) or
          (g_MovingItem.Item.S.StdMode = 49) and (g_MovingItem.Item.Dura >= g_MovingItem.Item.DuraMax)
          ) then begin
          frmMain.PetUseItem(-1);
        end;
      end;
    end;
  end;
end;

procedure TStateWindows.DSWPetsGridGridPaint(Sender:TObject; ACol,
  ARow:Integer; Rect:TRect; State:TGridDrawState);
var
  nIdx:Integer;
  DItemGrid:TDxImageGrid;
  R:TRect;
begin
  DItemGrid := TDxImageGrid(Sender);
  nIdx := ACol + ARow * DItemGrid.ColCount;
  if nIdx in [0..MAX_GAMEPET_BAG_COUNT - 1] then begin
    R := Bounds(Rect.Left, Rect.Top, DItemGrid.ColWidth, DItemGrid.RowHeight);
    FrmDlg.DrawGridItem(R, @g_PetItemArr[nIdx], @g_PetItemArrEffect[nIdx]);
  end;
end;

procedure TStateWindows.DSWPetsGridGridSelect(Sender:TObject; ACol,
  ARow:Integer; Button:TMouseButton; Shift:TShiftState);
var
  nIdx:Integer;
  temp:TClientItem;
  DItemGrid:TDxImageGrid;
  // bytes: array[0..62 - 1] of Byte;
begin
  DItemGrid := TDxImageGrid(Sender);

  nIdx := ACol + ARow * DItemGrid.ColCount;
  if nIdx in [0..MAX_GAMEPET_BAG_COUNT - 1] then begin
    if mbRight = Button then begin

      Exit;
    end;

    if not g_boItemMoving then begin
      if g_PetItemArr[nIdx].S.Name <> '' then begin
        g_dwMoveItemTick := MyGetTickCount;
        g_boItemMoving := True;
        g_MovingItem.Index := nIdx;
        g_MovingItem.Item := g_PetItemArr[nIdx];
        g_MovingItem.ItemType := mtGamePetBagItem;
        g_PetItemArr[nIdx].S.Name := '';

        ItemClickSound(g_PetItemArr[nIdx].S);
      end;
    end
    else begin
      if not (g_MovingItem.ItemType in [mtBagItem, mtGamePetBagItem]) then Exit;

      if g_MovingItem.ItemType = mtBagItem then begin
        if g_WaitingUseItem.Item.s.Name = '' then begin
          ItemClickSound(g_MovingItem.Item.S);
          g_WaitingUseItem := g_MovingItem;

          frmMain.SendBagItemToPetBag(g_MovingItem.Item.MakeIndex, g_MovingItem.Item.S.Name);
          g_MovingItem.Item.S.Name := '';
          g_boItemMoving := False;
          //g_PetItemArr[nIdx] := g_MovingItem.Item;
        end;
      end
      else begin
        if g_PetItemArr[nIdx].S.Name <> '' then begin
          if (g_MovingItem.ItemType = mtGamePetBagItem) and
            IsOverLapItem(@g_PetItemArr[nIdx], @g_MovingItem.Item) then begin // 开始重叠物品
            if g_WaitingUseItem.Item.s.Name = '' then begin
              g_WaitingUseItem := g_MovingItem;
              g_MovingItem.Item.S.Name := '';
              g_boItemMoving := False;
              frmMain.SendPetOverLapItem(g_WaitingUseItem.Item.MakeIndex,
                g_PetItemArr[nIdx].MakeIndex, g_WaitingUseItem.Item.s.Name);
            end;
          end
          else begin
            temp := g_PetItemArr[nIdx];
            g_PetItemArr[nIdx] := g_MovingItem.Item;
            g_MovingItem.Index := nIdx;
            g_MovingItem.Item := temp;
          end;
        end
        else begin
          g_PetItemArr[nIdx] := g_MovingItem.Item;
          g_MovingItem.Item.S.Name := '';
          g_boItemMoving := False;
        end;
      end;
    end;
  end;
  ArrangePetItembag;
end;

procedure TStateWindows.DSWPetsGridGridMouseMove(Sender:TObject; ACol,
  ARow:Integer; Shift:TShiftState);
var
  nIdx:Integer;

  vtRect:TRect;
  DItemGrid:TDxImageGrid;
begin
  if ssRight in Shift then begin
    if g_boItemMoving then
      DSWPetsGridGridSelect(Self, ACol, ARow, mbLeft, Shift);
  end
  else begin
    DItemGrid := TDxImageGrid(Sender);
    nIdx := ACol + ARow * DItemGrid.ColCount;

    if (nIdx in [0..MAX_GAMEPET_BAG_COUNT - 1]) and (g_PetItemArr[nIdx].s.Name <> '') then begin
      g_MouseItem := g_PetItemArr[nIdx];
      if (g_ClientConfig.btSuspensionShowItem > 0) or (g_ClientVersion > cvSerial) then begin
        g_boShowBagInfo := False;
        vtRect := DItemGrid.VirtualRect;
        FrmDlg.ShowMouseItemInfo(g_MySelf, @g_MouseItem, vtRect.Left + ACol *
          DItemGrid.ColWidth, vtRect.Top + +(ARow + 1) * DItemGrid.RowHeight,
          True, 0, False, False, True);
      end
      else begin
        g_boShowBagInfo := True;
      end;
    end
    else begin
      g_boShowBagInfo := False;
      g_MouseItem.S.Name := '';
      DScreen.ClearHint;
      HintWindows.Clear;
    end;
  end;
end;

procedure TStateWindows.OnDStateWinExClick(Sender:TObject; X, Y:Integer);
begin
  if btnDStateWinEx.IsOpen then begin
    if DStateWin.Left + DStateWin.Width + DStateWinEx.Width > SCREENWIDTH then
      DStateWin.Left := SCREENWIDTH - DStateWin.Width - DStateWinEx.Width;
    DStateWinEx.Visible := True;
    DStateWinEx.Left := DStateWin.Left + DStateWin.Width;
    DStateWinEx.Top := DStateWin.Top;

    RefreshDStateWinExInfo;
  end
  else begin
    DStateWinEx.Visible := False;
  end;
end;

procedure TStateWindows.DStateWinMove(Sender:TObject);
begin
  if DStateWinEx.Visible then begin
    DStateWinEx.Left := DStateWin.Left + DStateWin.Width;
    DStateWinEx.Top := DStateWin.Top;
  end;
end;

procedure TStateWindows.RefreshDStateWinExInfo;
begin
  if not DStateWinEx.Visible then Exit;
  lblStateExtJob.Caption := GetJobName(g_MySelf.m_btJob);
  lblStateExtLevel.Caption := IntToStr(g_MySelf.m_Abil.Level);
  lblStateExtGuild.Caption := g_sGuildName + ' ' + g_sGuildRankName;

  // HP
  btnStateExtHP.ProgressSetting.Max := g_MySelf.m_Abil.MaxHP;
  btnStateExtHP.ProgressSetting.Value := g_MySelf.m_Abil.HP;

  // MP
  btnStateExtMC.ProgressSetting.Max := g_MySelf.m_Abil.MaxMP;
  btnStateExtMC.ProgressSetting.Value := g_MySelf.m_Abil.MP;

  // 内功
  if g_MySelf.m_boTrainingNG or g_MySelf.m_boTrainingXF then begin
    btnStateExtNG.ProgressSetting.Max := g_MySelf.m_AbilNG.MaxNH;
    btnStateExtNG.ProgressSetting.Value := g_MySelf.m_AbilNG.NH;
  end
  else begin
    btnStateExtNG.ProgressSetting.Max := 0;
    btnStateExtNG.ProgressSetting.Value := 0;
  end;

  // 背包重量
  btnStateExtWeight.ProgressSetting.Max := g_MySelf.m_Abil.MaxWeight;
  btnStateExtWeight.ProgressSetting.Value := g_MySelf.m_Abil.Weight;

  // 穿戴重量
  btnStateExtWearWeight.ProgressSetting.Max := g_MySelf.m_Abil.MaxWearWeight;
  btnStateExtWearWeight.ProgressSetting.Value := g_MySelf.m_Abil.WearWeight;

  // 腕力
  btnStateExtHandWeight.ProgressSetting.Max := g_MySelf.m_Abil.MaxHandWeight;
  btnStateExtHandWeight.ProgressSetting.Value := g_MySelf.m_Abil.HandWeight;

  // 升级经验
  btnStateExtExp.ProgressSetting.Max := g_MySelf.m_Abil.MaxExp;
  btnStateExtExp.ProgressSetting.Value := g_MySelf.m_Abil.Exp;

  lblStateExtDC.Caption := IntToStr(g_MySelf.m_Abil.DC1) + '-' + IntToStr(g_MySelf.m_Abil.DC2);
  lblStateExtAC.Caption := IntToStr(g_MySelf.m_Abil.AC1) + '-' + IntToStr(g_MySelf.m_Abil.AC2);
  lblStateExtMAC.Caption := IntToStr(g_MySelf.m_Abil.MAC1) + '-' + IntToStr(g_MySelf.m_Abil.MAC2);
  lblStateExtMC.Caption := IntToStr(g_MySelf.m_Abil.MC1) + '-' + IntToStr(g_MySelf.m_Abil.MC2);
  lblStateExtSC.Caption := IntToStr(g_MySelf.m_Abil.SC1) + '-' + IntToStr(g_MySelf.m_Abil.SC2);

  lblStateExtHPRecover.Caption := IntToStr(g_nMyHealthRecover * 10) + '%';
  lblStateExtMPRecover.Caption := IntToStr(g_nMySpellRecover * 10) + '%';
  lblStateExtPosionRecover.Caption := IntToStr(g_nMyPoisonRecover * 10) + '%';

  lblStateExtHitPoint.Caption := IntToStr(g_nMyHitPoint); // 准确
  lblStateExtSpeedPoint.Caption := IntToStr(g_nMySpeedPoint); // 敏捷
  lblStateExtAttackSpeed.Caption := IntToStr(g_MySelf.m_nAttackSpeed); // 攻击速度

  lblStateExtAntiMagic.Caption := IntToStr(g_nMyAntiMagic * 10) + '%';
  lblStateExtAntiPoiston.Caption := IntToStr(g_nMyAntiPoison * 10) + '%';

  lblStateExtNewValue0.Caption := IntToStr(g_MySelf.m_Abil.NewValue[0]) + '%';
  lblStateExtNewValue1.Caption := IntToStr(g_MySelf.m_Abil.NewValue[1]) + '%';
  lblStateExtNewValue2.Caption := IntToStr(g_MySelf.m_Abil.NewValue[2]) + '%';
  lblStateExtNewValue3.Caption := IntToStr(g_MySelf.m_Abil.NewValue[3]) + '%';
  lblStateExtNewValue4.Caption := IntToStr(g_MySelf.m_Abil.NewValue[4]) + '%';
  lblStateExtNewValue5.Caption := IntToStr(g_MySelf.m_Abil.NewValue[5]) + '%';
  lblStateExtNewValue6.Caption := IntToStr(g_MySelf.m_Abil.NewValue[6]) + '%';
  lblStateExtNewValue7.Caption := IntToStr(g_MySelf.m_Abil.NewValue[7]) + '%';
  lblStateExtNewValue8.Caption := IntToStr(g_MySelf.m_Abil.NewValue[8]) + '%';
  lblStateExtNewValue9.Caption := IntToStr(g_MySelf.m_Abil.NewValue[9]) + '%';
  lblStateExtNewValue10.Caption := IntToStr(g_MySelf.m_Abil.NewValue[10]) + '%';
  lblStateExtNewValue11.Caption := IntToStr(g_MySelf.m_Abil.NewValue[11]) + '%';
  lblStateExtNewValue12.Caption := IntToStr(g_MySelf.m_Abil.NewValue[12]) + '%';
  lblStateExtNewValue13.Caption := IntToStr(g_MySelf.m_Abil.NewValue[13]) + '%';
  lblStateExtNewValue14.Caption := IntToStr(g_MySelf.m_Abil.NewValue[14]) + '%';
  lblStateExtNewValue15.Caption := IntToStr(g_MySelf.m_Abil.NewValue[15]) + '%';
  lblStateExtNewValue16.Caption := IntToStr(g_MySelf.m_Abil.NewValue[16]) + '%';
  lblStateExtNewValue17.Caption := IntToStr(g_MySelf.m_Abil.NewValue[17]) + '%';
  lblStateExtNewValue18.Caption := IntToStr(g_MySelf.m_Abil.NewValue[18]) + '%';
  lblStateExtNewValue19.Caption := IntToStr(g_MySelf.m_Abil.NewValue[19]) + '%';
  lblStateExtNewValue20.Caption := IntToStr(g_MySelf.m_Abil.NewValue[20]) + '%';
  lblStateExtNewValue21.Caption := IntToStr(g_MySelf.m_Abil.NewValue[21]) + '%';
  lblStateExtNewValue22.Caption := IntToStr(g_MySelf.m_Abil.NewValue[22]) + '%';
  lblStateExtNewValue23.Caption := IntToStr(g_MySelf.m_Abil.NewValue[23]) + '%';
end;

procedure TStateWindows.DControlMouseMoveShowHint(Sender:TObject;
  Shift:TShiftState; X, Y:Integer);
var
  vtRect:TRect;
  i:Integer;
  HintLines:THintLines;
  HintWindow:THintWindow;
  slLines:TStringList;
  sHintText, sHintColor:string;
  btHintColor:Byte;
begin
  if g_LastHintMakeIndex <> Integer(Sender) then begin
    DScreen.ClearHint;
    HintWindows.Clear;

    with Sender as TDxControl do begin
      if Length(Hint) <> 0 then begin
        vtRect := VirtualRect;
        HintWindow := THintWindow.Create;

        slLines := TStringList.Create;
        try
          slLines.Delimiter := '\';
          slLines.DelimitedText := Hint;
          for i := 0 to slLines.Count - 1 do begin
            //ShowMessage(slLines[i]);
            slLines[i] := GetValidStr3_Ex(slLines[i], sHintColor, '#');
            if slLines[i] = '' then begin
              btHintColor := 255;
              sHintText := sHintColor;
            end
            else begin
              sHintText := slLines[i];
              btHintColor := StrToIntDef(sHintColor, 0);
            end;
            HintLines := THintLines.Create;
            HintLines.Add(sHintText, GetRGB(btHintColor), GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke());
            HintWindow.Add(HintLines);
          end;
        finally
          slLines.Free;
        end;

        HintWindow.Show(vtRect.Left, vtRect.Bottom);

        HintWindows.Add(HintWindow);
      end;
    end;

    g_LastHintMakeIndex := Integer(Sender);
  end;
end;

end.

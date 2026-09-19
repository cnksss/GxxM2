unit FunctionConfig;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ComCtrls, StdCtrls, Spin, Grids, Grobal2,
  ExtCtrls,
{$IFDEF CPUX64}
  // VMProtectSDK,
{$ENDIF}
  ColorIndexEdit, SpinEditEx, uMagicACUtils, M2Definition, uCustomMagicUtils;

type
  TLevelExpScheme = (s_OldLevelExp, s_StdLevelExp, s_2Mult, s_5Mult, s_8Mult, s_10Mult, s_20Mult, s_30Mult, s_40Mult, s_50Mult,
    s_60Mult, s_70Mult, s_80Mult, s_90Mult, s_100Mult, s_150Mult, s_200Mult, s_250Mult, s_300Mult);

  TfrmFunctionConfig = class(TForm)
    FunctionConfigControl: TPageControl;
    Label14: TLabel;
    MonSaySheet: TTabSheet;
    TabSheet1: TTabSheet;
    PasswordSheet: TTabSheet;
    GroupBox1: TGroupBox;
    CheckBoxEnablePasswordLock: TCheckBox;
    GroupBox2: TGroupBox;
    CheckBoxLockGetBackItem: TCheckBox;
    GroupBox3: TGroupBox;
    Label1: TLabel;
    EditErrorPasswordCount: TSpinEditEx;
    CheckBoxErrorCountKick: TCheckBox;
    ButtonPasswordLockSave: TButton;
    GroupBox4: TGroupBox;
    CheckBoxLockWalk: TCheckBox;
    CheckBoxLockRun: TCheckBox;
    CheckBoxLockHit: TCheckBox;
    CheckBoxLockSpell: TCheckBox;
    CheckBoxLockSendMsg: TCheckBox;
    CheckBoxLockInObMode: TCheckBox;
    CheckBoxLockLogin: TCheckBox;
    CheckBoxLockUseItem: TCheckBox;
    CheckBoxLockDropItem: TCheckBox;
    CheckBoxLockDealItem: TCheckBox;
    TabSheetGeneral: TTabSheet;
    GroupBox7: TGroupBox;
    CheckBoxHungerSystem: TCheckBox;
    ButtonGeneralSave: TButton;
    ButtonSkillSave: TButton;
    TabSheet32: TTabSheet;
    TabSheet33: TTabSheet;
    TabSheet34: TTabSheet;
    TabSheet35: TTabSheet;
    GroupBox8: TGroupBox;
    Label13: TLabel;
    EditUpgradeWeaponMaxPoint: TSpinEditEx;
    Label15: TLabel;
    EditUpgradeWeaponPrice: TSpinEditEx;
    Label16: TLabel;
    EditUPgradeWeaponGetBackTime: TSpinEditEx;
    Label17: TLabel;
    EditClearExpireUpgradeWeaponDays: TSpinEditEx;
    Label18: TLabel;
    Label19: TLabel;
    GroupBox18: TGroupBox;
    ScrollBarUpgradeWeaponDCRate: TScrollBar;
    Label20: TLabel;
    EditUpgradeWeaponDCRate: TEdit;
    Label21: TLabel;
    ScrollBarUpgradeWeaponDCTwoPointRate: TScrollBar;
    EditUpgradeWeaponDCTwoPointRate: TEdit;
    Label22: TLabel;
    ScrollBarUpgradeWeaponDCThreePointRate: TScrollBar;
    EditUpgradeWeaponDCThreePointRate: TEdit;
    GroupBox19: TGroupBox;
    Label23: TLabel;
    Label24: TLabel;
    Label25: TLabel;
    ScrollBarUpgradeWeaponSCRate: TScrollBar;
    EditUpgradeWeaponSCRate: TEdit;
    ScrollBarUpgradeWeaponSCTwoPointRate: TScrollBar;
    EditUpgradeWeaponSCTwoPointRate: TEdit;
    ScrollBarUpgradeWeaponSCThreePointRate: TScrollBar;
    EditUpgradeWeaponSCThreePointRate: TEdit;
    GroupBox20: TGroupBox;
    Label26: TLabel;
    Label27: TLabel;
    Label28: TLabel;
    ScrollBarUpgradeWeaponMCRate: TScrollBar;
    EditUpgradeWeaponMCRate: TEdit;
    ScrollBarUpgradeWeaponMCTwoPointRate: TScrollBar;
    EditUpgradeWeaponMCTwoPointRate: TEdit;
    ScrollBarUpgradeWeaponMCThreePointRate: TScrollBar;
    EditUpgradeWeaponMCThreePointRate: TEdit;
    ButtonUpgradeWeaponSave: TButton;
    GroupBox21: TGroupBox;
    ButtonMasterSave: TButton;
    GroupBox22: TGroupBox;
    EditMasterOKLevel: TSpinEditEx;
    Label29: TLabel;
    GroupBox23: TGroupBox;
    EditMasterOKCreditPoint: TSpinEditEx;
    Label30: TLabel;
    EditMasterOKBonusPoint: TSpinEditEx;
    Label31: TLabel;
    GroupBox24: TGroupBox;
    ScrollBarMakeMineHitRate: TScrollBar;
    EditMakeMineHitRate: TEdit;
    Label32: TLabel;
    Label33: TLabel;
    ScrollBarMakeMineRate: TScrollBar;
    EditMakeMineRate: TEdit;
    GroupBox25: TGroupBox;
    Label34: TLabel;
    Label35: TLabel;
    ScrollBarStoneTypeRate: TScrollBar;
    EditStoneTypeRate: TEdit;
    ScrollBarGoldStoneMax: TScrollBar;
    EditGoldStoneMax: TEdit;
    Label36: TLabel;
    ScrollBarSilverStoneMax: TScrollBar;
    EditSilverStoneMax: TEdit;
    Label37: TLabel;
    ScrollBarSteelStoneMax: TScrollBar;
    EditSteelStoneMax: TEdit;
    Label38: TLabel;
    EditBlackStoneMax: TEdit;
    ScrollBarBlackStoneMax: TScrollBar;
    ButtonMakeMineSave: TButton;
    GroupBox26: TGroupBox;
    Label39: TLabel;
    EditStoneMinDura: TSpinEditEx;
    Label40: TLabel;
    EditStoneGeneralDuraRate: TSpinEditEx;
    Label41: TLabel;
    EditStoneAddDuraRate: TSpinEditEx;
    Label42: TLabel;
    EditStoneAddDuraMax: TSpinEditEx;
    TabSheet37: TTabSheet;
    GroupBox27: TGroupBox;
    Label43: TLabel;
    Label44: TLabel;
    Label45: TLabel;
    Label46: TLabel;
    Label47: TLabel;
    ScrollBarWinLottery1Max: TScrollBar;
    EditWinLottery1Max: TEdit;
    ScrollBarWinLottery2Max: TScrollBar;
    EditWinLottery2Max: TEdit;
    ScrollBarWinLottery3Max: TScrollBar;
    EditWinLottery3Max: TEdit;
    ScrollBarWinLottery4Max: TScrollBar;
    EditWinLottery4Max: TEdit;
    EditWinLottery5Max: TEdit;
    ScrollBarWinLottery5Max: TScrollBar;
    Label48: TLabel;
    ScrollBarWinLottery6Max: TScrollBar;
    EditWinLottery6Max: TEdit;
    EditWinLotteryRate: TEdit;
    ScrollBarWinLotteryRate: TScrollBar;
    Label49: TLabel;
    GroupBox28: TGroupBox;
    Label50: TLabel;
    Label51: TLabel;
    Label52: TLabel;
    Label53: TLabel;
    EditWinLottery1Gold: TSpinEditEx;
    EditWinLottery2Gold: TSpinEditEx;
    EditWinLottery3Gold: TSpinEditEx;
    EditWinLottery4Gold: TSpinEditEx;
    Label54: TLabel;
    EditWinLottery5Gold: TSpinEditEx;
    Label55: TLabel;
    EditWinLottery6Gold: TSpinEditEx;
    ButtonWinLotterySave: TButton;
    TabSheet38: TTabSheet;
    GroupBox29: TGroupBox;
    Label56: TLabel;
    EditReNewNameColor1: TColorIndexEdit;
    Label58: TLabel;
    EditReNewNameColor2: TColorIndexEdit;
    Label60: TLabel;
    EditReNewNameColor3: TColorIndexEdit;
    Label62: TLabel;
    EditReNewNameColor4: TColorIndexEdit;
    Label64: TLabel;
    EditReNewNameColor5: TColorIndexEdit;
    Label66: TLabel;
    EditReNewNameColor6: TColorIndexEdit;
    Label68: TLabel;
    EditReNewNameColor7: TColorIndexEdit;
    Label70: TLabel;
    EditReNewNameColor8: TColorIndexEdit;
    Label72: TLabel;
    EditReNewNameColor9: TColorIndexEdit;
    Label74: TLabel;
    EditReNewNameColor10: TColorIndexEdit;
    ButtonReNewLevelSave: TButton;
    GroupBox30: TGroupBox;
    Label57: TLabel;
    EditReNewNameColorTime: TSpinEditEx;
    Label59: TLabel;
    TabSheet39: TTabSheet;
    ButtonMonUpgradeSave: TButton;
    GroupBox32: TGroupBox;
    Label65: TLabel;
    Label67: TLabel;
    Label69: TLabel;
    Label71: TLabel;
    Label73: TLabel;
    Label75: TLabel;
    Label76: TLabel;
    Label77: TLabel;
    EditMonUpgradeColor1: TColorIndexEdit;
    EditMonUpgradeColor2: TColorIndexEdit;
    EditMonUpgradeColor3: TColorIndexEdit;
    EditMonUpgradeColor4: TColorIndexEdit;
    EditMonUpgradeColor5: TColorIndexEdit;
    EditMonUpgradeColor6: TColorIndexEdit;
    EditMonUpgradeColor7: TColorIndexEdit;
    EditMonUpgradeColor8: TColorIndexEdit;
    GroupBox31: TGroupBox;
    Label61: TLabel;
    Label63: TLabel;
    Label78: TLabel;
    Label79: TLabel;
    Label80: TLabel;
    Label81: TLabel;
    Label82: TLabel;
    Label83: TLabel;
    EditMonUpgradeKillCount1: TSpinEditEx;
    EditMonUpgradeKillCount2: TSpinEditEx;
    EditMonUpgradeKillCount3: TSpinEditEx;
    EditMonUpgradeKillCount4: TSpinEditEx;
    EditMonUpgradeKillCount5: TSpinEditEx;
    EditMonUpgradeKillCount6: TSpinEditEx;
    EditMonUpgradeKillCount7: TSpinEditEx;
    EditMonUpLvNeedKillBase: TSpinEditEx;
    EditMonUpLvRate: TSpinEditEx;
    Label84: TLabel;
    CheckBoxReNewChangeColor: TCheckBox;
    GroupBox33: TGroupBox;
    CheckBoxReNewLevelClearExp: TCheckBox;
    GroupBox34: TGroupBox;
    Label85: TLabel;
    EditPKFlagNameColor: TColorIndexEdit;
    Label87: TLabel;
    EditPKLevel1NameColor: TColorIndexEdit;
    Label89: TLabel;
    EditPKLevel2NameColor: TColorIndexEdit;
    Label91: TLabel;
    EditAllyAndGuildNameColor: TColorIndexEdit;
    Label93: TLabel;
    EditWarGuildNameColor: TColorIndexEdit;
    Label95: TLabel;
    EditInFreePKAreaNameColor: TColorIndexEdit;
    TabSheet40: TTabSheet;
    Label86: TLabel;
    EditMonUpgradeColor9: TColorIndexEdit;
    GroupBox35: TGroupBox;
    CheckBoxMasterDieMutiny: TCheckBox;
    Label88: TLabel;
    EditMasterDieMutinyRate: TSpinEditEx;
    Label90: TLabel;
    EditMasterDieMutinyPower: TSpinEditEx;
    Label92: TLabel;
    EditMasterDieMutinySpeed: TSpinEditEx;
    GroupBox36: TGroupBox;
    Label94: TLabel;
    Label96: TLabel;
    CheckBoxSpiritMutiny: TCheckBox;
    EditSpiritMutinyTime: TSpinEditEx;
    EditSpiritPowerRate: TSpinEditEx;
    ButtonSpiritMutinySave: TButton;
    GroupBox40: TGroupBox;
    CheckBoxMonSayMsg: TCheckBox;
    ButtonMonSayMsgSave: TButton;
    ButtonUpgradeWeaponDefaulf: TButton;
    ButtonMakeMineDefault: TButton;
    ButtonWinLotteryDefault: TButton;
    TabSheet42: TTabSheet;
    GroupBox44: TGroupBox;
    Label105: TLabel;
    Label106: TLabel;
    Label107: TLabel;
    Label108: TLabel;
    Label109: TLabel;
    ScrollBarWeaponMakeUnLuckRate: TScrollBar;
    EditWeaponMakeUnLuckRate: TEdit;
    ScrollBarWeaponMakeLuckPoint1: TScrollBar;
    EditWeaponMakeLuckPoint1: TEdit;
    ScrollBarWeaponMakeLuckPoint2: TScrollBar;
    EditWeaponMakeLuckPoint2: TEdit;
    ScrollBarWeaponMakeLuckPoint2Rate: TScrollBar;
    EditWeaponMakeLuckPoint2Rate: TEdit;
    EditWeaponMakeLuckPoint3: TEdit;
    ScrollBarWeaponMakeLuckPoint3: TScrollBar;
    Label110: TLabel;
    ScrollBarWeaponMakeLuckPoint3Rate: TScrollBar;
    EditWeaponMakeLuckPoint3Rate: TEdit;
    ButtonWeaponMakeLuckDefault: TButton;
    ButtonWeaponMakeLuckSave: TButton;
    GroupBox47: TGroupBox;
    Label112: TLabel;
    CheckBoxBBMonAutoChangeColor: TCheckBox;
    EditBBMonAutoChangeColorTime: TSpinEditEx;
    TabSheet50: TTabSheet;
    ButtonHeroOptionSave: TButton;
    Label113: TLabel;
    EditMerchantNameColor: TColorIndexEdit;
    GroupBox49: TGroupBox;
    MagicPageControl: TPageControl;
    TabSheet61: TTabSheet;
    GroupBox17: TGroupBox;
    Label12: TLabel;
    EditMagicAttackRage: TSpinEditEx;
    GroupBox53: TGroupBox;
    Label117: TLabel;
    SpinEditMagDelayTime: TSpinEditEx;
    TabSheet62: TTabSheet;
    PageControl4: TPageControl;
    TabSheet2: TTabSheet;
    GroupBox56: TGroupBox;
    Label119: TLabel;
    Label120: TLabel;
    seDedingMagicCD: TSpinEditEx;
    TabSheet7: TTabSheet;
    GroupBox9: TGroupBox;
    CheckBoxLimitSwordLong: TCheckBox;
    GroupBox10: TGroupBox;
    Label4: TLabel;
    Label10: TLabel;
    EditSwordLongPowerRate: TSpinEditEx;
    TabSheet8: TTabSheet;
    TabSheet9: TTabSheet;
    TabSheet10: TTabSheet;
    TabSheet63: TTabSheet;
    PageControl5: TPageControl;
    TabSheet14: TTabSheet;
    GroupBox38: TGroupBox;
    Label98: TLabel;
    EditMagTammingLevel: TSpinEditEx;
    GroupBox45: TGroupBox;
    Label111: TLabel;
    EditTammingCount: TSpinEditEx;
    GroupBox39: TGroupBox;
    Label99: TLabel;
    Label100: TLabel;
    EditMagTammingTargetLevel: TSpinEditEx;
    EditMagTammingHPRate: TSpinEditEx;
    TabSheet15: TTabSheet;
    GroupBox46: TGroupBox;
    CheckBoxFireCrossInSafeZone: TCheckBox;
    TabSheet16: TTabSheet;
    GroupBox37: TGroupBox;
    Label97: TLabel;
    seMagTurnUndeadLevel: TSpinEditEx;
    TabSheet17: TTabSheet;
    GroupBox15: TGroupBox;
    Label9: TLabel;
    seElecBlizzardRange: TSpinEditEx;
    TabSheet19: TTabSheet;
    GroupBox13: TGroupBox;
    Label7: TLabel;
    seFireBoomRage: TSpinEditEx;
    TabSheet23: TTabSheet;
    GroupBox14: TGroupBox;
    Label8: TLabel;
    seSnowWindRange: TSpinEditEx;
    TabSheet18: TTabSheet;
    GroupBox51: TGroupBox;
    chkPlayObjectReduceMP: TCheckBox;
    TabSheet64: TTabSheet;
    PageControl3: TPageControl;
    PageControl6: TPageControl;
    TabSheet20: TTabSheet;
    GroupBox16: TGroupBox;
    Label11: TLabel;
    EditAmyOunsulPoint: TSpinEditEx;
    TabSheet21: TTabSheet;
    GroupBox5: TGroupBox;
    Label2: TLabel;
    Label3: TLabel;
    EditBoneFammName: TEdit;
    EditBoneFammCount: TSpinEditEx;
    GroupBox6: TGroupBox;
    GridBoneFamm: TStringGrid;
    TabSheet22: TTabSheet;
    GroupBox11: TGroupBox;
    Label5: TLabel;
    Label6: TLabel;
    EditDogzName: TEdit;
    EditDogzCount: TSpinEditEx;
    GroupBox12: TGroupBox;
    GridDogz: TStringGrid;
    TabSheet24: TTabSheet;
    TabSheet13: TTabSheet;
    GroupBox59: TGroupBox;
    Label121: TLabel;
    Label122: TLabel;
    seFireHitWaitTime: TSpinEditEx;
    TabSheet3: TTabSheet;
    GroupBox60: TGroupBox;
    Label123: TLabel;
    Label124: TLabel;
    EditSkill56PowerRate: TSpinEditEx;
    TabSheet4: TTabSheet;
    TabSheet6: TTabSheet;
    GroupBox61: TGroupBox;
    Label125: TLabel;
    Label126: TLabel;
    EditSkill58PowerRate: TSpinEditEx;
    TabSheet11: TTabSheet;
    TabSheet25: TTabSheet;
    GroupBox58: TGroupBox;
    Label128: TLabel;
    Label129: TLabel;
    seSkill42PowerRate: TSpinEditEx;
    TabSheet26: TTabSheet;
    GroupBox68: TGroupBox;
    Label130: TLabel;
    Label131: TLabel;
    seSkill40PowerRate: TSpinEditEx;
    TabSheet27: TTabSheet;
    GroupBox77: TGroupBox;
    Label132: TLabel;
    Label133: TLabel;
    EditSkill43PowerRate: TSpinEditEx;
    TabSheet28: TTabSheet;
    GroupBox78: TGroupBox;
    Label153: TLabel;
    Label155: TLabel;
    seHeroSkill66PowerRate: TSpinEditEx;
    PageControl2: TPageControl;
    TabSheet29: TTabSheet;
    GroupBox79: TGroupBox;
    Label157: TLabel;
    seCopySelfMaxCount: TSpinEditEx;
    Label159: TLabel;
    seCopySelfExistTime: TSpinEditEx;
    Label160: TLabel;
    chkCopySelfNonUseSpellPoint: TCheckBox;
    TabSheet30: TTabSheet;
    GroupBox80: TGroupBox;
    lbl1: TLabel;
    EditMagicItemRate: TSpinEditEx;
    TabSheet31: TTabSheet;
    GroupBox81: TGroupBox;
    CheckBoxOffLineLoginSafeArea: TCheckBox;
    RadioButtonOffLineLoginMapName1: TRadioButton;
    RadioButtonOffLineLoginMapName2: TRadioButton;
    ButtonOffLineSave: TButton;
    RadioGroupHumNeedMagicItem: TRadioGroup;
    GroupBox83: TGroupBox;
    Label186: TLabel;
    Label187: TLabel;
    EditSWordHitWaitTime: TSpinEditEx;
    GroupBox85: TGroupBox;
    Label190: TLabel;
    Label191: TLabel;
    EditSkill66HitWaitTime: TSpinEditEx;
    GroupBox86: TGroupBox;
    Label192: TLabel;
    EditSkill58AttackRange: TSpinEditEx;
    TabSheet36: TTabSheet;
    TabSheet43: TTabSheet;
    GroupBox87: TGroupBox;
    Label193: TLabel;
    Label194: TLabel;
    seSkill72HitWaitTime: TSpinEditEx;
    GroupBox89: TGroupBox;
    Label197: TLabel;
    Label198: TLabel;
    seSuperShiledValidTime: TSpinEditEx;
    GroupBox90: TGroupBox;
    Label199: TLabel;
    Label200: TLabel;
    seSuperShiledPowerRate: TSpinEditEx;
    GroupBoxCloseSuperShiled: TGroupBox;
    chkUseSkillCloseSuperShiled0: TCheckBox;
    chkUseSkillCloseSuperShiled1: TCheckBox;
    chkUseSkillCloseSuperShiled2: TCheckBox;
    chkUseSkillCloseSuperShiled3: TCheckBox;
    GroupBox93: TGroupBox;
    CheckBoxAutoOpenSuperShiled: TCheckBox;
    chkShowSuperShiledEffect: TCheckBox;
    GroupBox50: TGroupBox;
    chkSkill71PullPlayObject: TCheckBox;
    chkSkill71PullCrossInSafeZone: TCheckBox;
    Label203: TLabel;
    seLastSuperShiledTime: TSpinEditEx;
    Label204: TLabel;
    GroupBox95: TGroupBox;
    Label205: TLabel;
    seHeroSkill66HighAttackRate: TSpinEditEx;
    GroupBox96: TGroupBox;
    Label208: TLabel;
    EditSkill57AddHPRate: TSpinEditEx;
    Label209: TLabel;
    GroupBox97: TGroupBox;
    Label210: TLabel;
    Label211: TLabel;
    edtMonthSpiritName: TEdit;
    seMonthSpiritCount: TSpinEditEx;
    GroupBox98: TGroupBox;
    GridMonthSpirit: TStringGrid;
    GroupBox99: TGroupBox;
    TabSheet12: TTabSheet;
    Label212: TLabel;
    EditMonthSpiritHighAttackRate: TSpinEditEx;
    Label213: TLabel;
    EditMonthSpiritHighPowerRate: TSpinEditEx;
    Label214: TLabel;
    GroupBox100: TGroupBox;
    Label215: TLabel;
    Label216: TLabel;
    EditBigDogzName: TEdit;
    EditBigDogzCount: TSpinEditEx;
    GroupBox101: TGroupBox;
    GridBigDogz: TStringGrid;
    Label217: TLabel;
    EditRecallBigDogWaitTime: TSpinEditEx;
    Label218: TLabel;
    chkUseSkillCloseSuperShiled4: TCheckBox;
    GroupBox92: TGroupBox;
    chkRecallManySlave1: TCheckBox;
    TabSheet41: TTabSheet;
    GroupBox102: TGroupBox;
    Label219: TLabel;
    Label220: TLabel;
    EditMaxMyShopSellingItemCount: TSpinEditEx;
    EditMaxMyShopStorageItemCount: TSpinEditEx;
    ButtonMyShopSave: TButton;
    GroupBox103: TGroupBox;
    Label221: TLabel;
    EditSlavePowerRate: TSpinEditEx;
    GroupBox104: TGroupBox;
    Label222: TLabel;
    EditMagicLockRange: TSpinEditEx;
    GroupBox117: TGroupBox;
    CheckBoxDisableChangeMapFireCross: TCheckBox;
    GroupBox118: TGroupBox;
    Label240: TLabel;
    Label241: TLabel;
    EditFireCrossMaxTime: TSpinEditEx;
    Label242: TLabel;
    EditFireCrossPowerRate: TSpinEditEx;
    Label243: TLabel;
    TabSheet49: TTabSheet;
    GroupBox120: TGroupBox;
    Label244: TLabel;
    EditMasterRoyaltyTime: TSpinEditEx;
    TabSheet51: TTabSheet;
    GroupBox41: TGroupBox;
    Label101: TLabel;
    Label102: TLabel;
    EditMabMabeHitRandRate: TSpinEditEx;
    EditMabMabeHitMinLvLimit: TSpinEditEx;
    GroupBox43: TGroupBox;
    Label104: TLabel;
    EditMabMabeHitMabeTimeRate: TSpinEditEx;
    GroupBox42: TGroupBox;
    Label103: TLabel;
    EditMabMabeHitSucessRate: TSpinEditEx;
    GroupBox122: TGroupBox;
    Label247: TLabel;
    Label248: TLabel;
    seFireHitPowerRate: TSpinEditEx;
    chkRecallManySlave2: TCheckBox;
    TabSheet5: TTabSheet;
    GroupBox121: TGroupBox;
    Label245: TLabel;
    Label246: TLabel;
    EditSkill52PowerRate: TSpinEditEx;
    GroupBox123: TGroupBox;
    Label249: TLabel;
    EditSkill52AttackRange: TSpinEditEx;
    TabSheet52: TTabSheet;
    TabSheet53: TTabSheet;
    PageControl7: TPageControl;
    TabSheet65: TTabSheet;
    GroupBox129: TGroupBox;
    Label272: TLabel;
    Label273: TLabel;
    Label274: TLabel;
    GridMedicineExp: TStringGrid;
    EditDecMedicineValue: TSpinEditEx;
    EditDecMedicineTime: TSpinEditEx;
    GroupBox130: TGroupBox;
    Label275: TLabel;
    Label276: TLabel;
    Label277: TLabel;
    Label278: TLabel;
    Label279: TLabel;
    Label280: TLabel;
    EditIncAlcoholTime: TSpinEditEx;
    EditDecDrinkTime: TSpinEditEx;
    EditMaxAlcoholValue: TSpinEditEx;
    EditIncAlcoholValue: TSpinEditEx;
    ButtonSaveWine: TButton;
    TabSheet66: TTabSheet;
    GroupBox131: TGroupBox;
    Label281: TLabel;
    Label284: TLabel;
    EditUseContinuousMagicTime: TSpinEditEx;
    TabSheet68: TTabSheet;
    GroupBox132: TGroupBox;
    Label282: TLabel;
    Label283: TLabel;
    Label285: TLabel;
    EditNGLevelValue: TSpinEditEx;
    EditNGLevelExpValue: TSpinEditEx;
    EditNGHeroLevelExpValue: TSpinEditEx;
    GroupBox133: TGroupBox;
    EditNGIncTime: TSpinEditEx;
    GroupBox134: TGroupBox;
    Label287: TLabel;
    EditNGSkillPowerRate: TSpinEditEx;
    GroupBox135: TGroupBox;
    Label288: TLabel;
    edtNGLevelPowerAdd_Level: TSpinEditEx;
    GroupBox136: TGroupBox;
    Label289: TLabel;
    EditNGKillMonExpMultiple: TSpinEditEx;
    GroupBox137: TGroupBox;
    Label290: TLabel;
    Label291: TLabel;
    EditNGDrinkIncExp: TSpinEditEx;
    EditNGHitStruckDecNG: TSpinEditEx;
    PageControl12: TPageControl;
    TabSheet84: TTabSheet;
    TabSheet85: TTabSheet;
    TabSheet86: TTabSheet;
    TabSheet87: TTabSheet;
    TabSheet88: TTabSheet;
    GroupBox125: TGroupBox;
    Label251: TLabel;
    Label252: TLabel;
    Label253: TLabel;
    Label254: TLabel;
    Label255: TLabel;
    EditAcupoints0_0: TSpinEditEx;
    EditAcupoints0_1: TSpinEditEx;
    EditAcupoints0_2: TSpinEditEx;
    EditAcupoints0_3: TSpinEditEx;
    EditAcupoints0_4: TSpinEditEx;
    GroupBox126: TGroupBox;
    Label256: TLabel;
    Label257: TLabel;
    Label258: TLabel;
    Label259: TLabel;
    Label260: TLabel;
    EditAcupoints1_0: TSpinEditEx;
    EditAcupoints1_1: TSpinEditEx;
    EditAcupoints1_2: TSpinEditEx;
    EditAcupoints1_3: TSpinEditEx;
    EditAcupoints1_4: TSpinEditEx;
    GroupBox127: TGroupBox;
    Label261: TLabel;
    Label262: TLabel;
    Label263: TLabel;
    Label264: TLabel;
    Label265: TLabel;
    EditAcupoints2_0: TSpinEditEx;
    EditAcupoints2_1: TSpinEditEx;
    EditAcupoints2_2: TSpinEditEx;
    EditAcupoints2_3: TSpinEditEx;
    EditAcupoints2_4: TSpinEditEx;
    GroupBox128: TGroupBox;
    Label266: TLabel;
    Label267: TLabel;
    Label268: TLabel;
    Label269: TLabel;
    Label270: TLabel;
    EditAcupoints3_0: TSpinEditEx;
    EditAcupoints3_1: TSpinEditEx;
    EditAcupoints3_2: TSpinEditEx;
    EditAcupoints3_3: TSpinEditEx;
    EditAcupoints3_4: TSpinEditEx;
    GroupBox174: TGroupBox;
    Label556: TLabel;
    Label557: TLabel;
    Label558: TLabel;
    Label559: TLabel;
    Label560: TLabel;
    EditAcupoints4_0: TSpinEditEx;
    EditAcupoints4_1: TSpinEditEx;
    EditAcupoints4_2: TSpinEditEx;
    EditAcupoints4_3: TSpinEditEx;
    EditAcupoints4_4: TSpinEditEx;
    Label286: TLabel;
    TabSheet89: TTabSheet;
    GroupBox175: TGroupBox;
    Label561: TLabel;
    seSkill114HitWaitTime: TSpinEditEx;
    TabSheet90: TTabSheet;
    CheckBoxViewRangeCanMagicAttack: TCheckBox;
    chkDogzGotoMaster: TCheckBox;
    CheckBoxMonthSpiritGotoMaster: TCheckBox;
    CheckBoxBigDogzGotoMaster: TCheckBox;
    CheckBoxOfflineCloseMyShop: TCheckBox;
    GroupBox179: TGroupBox;
    chkEnableDoubleFireHitSkill: TCheckBox;
    GroupBox181: TGroupBox;
    CheckBoxMasterRoyaltyDie: TCheckBox;
    GroupBox182: TGroupBox;
    Label570: TLabel;
    Label571: TLabel;
    EditAmyOunsulTimeRate: TSpinEditEx;
    Label572: TLabel;
    EditAmyOunsulMaxTime: TSpinEditEx;
    Label573: TLabel;
    TabSheet92: TTabSheet;
    GroupBox183: TGroupBox;
    Label574: TLabel;
    Label575: TLabel;
    EditElectrodelessWaitTime: TSpinEditEx;
    Label576: TLabel;
    EditElectrodelessBase: TSpinEditEx;
    GroupBox185: TGroupBox;
    Label579: TLabel;
    Label580: TLabel;
    EditSkill57PowerRate: TSpinEditEx;
    TabSheet93: TTabSheet;
    GroupBox186: TGroupBox;
    CheckBoxQigongPushSameLevel: TCheckBox;
    TabSheet94: TTabSheet;
    GroupBox187: TGroupBox;
    CheckBoxFireWindPushSameLevel: TCheckBox;
    RadioGroupShopType: TRadioGroup;
    GroupBox190: TGroupBox;
    Label583: TLabel;
    seSkill41CD: TSpinEditEx;
    CheckBoxWeaponUpgradeFailNotDelete: TCheckBox;
    GroupBox191: TGroupBox;
    Label585: TLabel;
    Label586: TLabel;
    seSkill58WaitTime: TSpinEditEx;
    GroupBox193: TGroupBox;
    chkShowDoMotaeboMsg: TCheckBox;
    GroupBox195: TGroupBox;
    CheckBoxShowYouPoisoned: TCheckBox;
    grp2: TGroupBox;
    edtInfinityStorageCount: TSpinEditEx;
    chkInfinityStorage: TCheckBox;
    lblInfinityStorageCount: TLabel;
    ts1: TTabSheet;
    grp3: TGroupBox;
    lbl3: TLabel;
    seSkill202BaseCount: TSpinEditEx;
    lbl2: TLabel;
    seSkill202LevelupCount: TSpinEditEx;
    ts2: TTabSheet;
    grp4: TGroupBox;
    lbl4: TLabel;
    chkSkill203MbAttackMon: TCheckBox;
    chkSkill203MbAttackHuman: TCheckBox;
    chkSkill203MbAttackSlave: TCheckBox;
    chkSkill203Damagearmor: TCheckBox;
    chkSkill203DecHealth: TCheckBox;
    chkSkill203MbFastParalysis: TCheckBox;
    grp5: TGroupBox;
    lbl5: TLabel;
    lbl6: TLabel;
    seSkill203LevelupPowerRate: TSpinEditEx;
    lbl7: TLabel;
    seSkill203LevelupMbTimer: TSpinEditEx;
    lbl8: TLabel;
    lbl9: TLabel;
    ts3: TTabSheet;
    grp6: TGroupBox;
    chkSkill204MbAttackMon: TCheckBox;
    chkSkill204MbAttackHuman: TCheckBox;
    chkSkill204MbAttackSlave: TCheckBox;
    chkSkill204MbFastParalysis: TCheckBox;
    chkSkill204RunGuard: TCheckBox;
    chkSkill204RunNpc: TCheckBox;
    chkSkill204RunMon: TCheckBox;
    chkSkill204RunHum: TCheckBox;
    chkSkill204WarDisHumRun: TCheckBox;
    grp7: TGroupBox;
    lbl10: TLabel;
    lbl13: TLabel;
    seSkill204LevelupPowerRate: TSpinEditEx;
    lbl11: TLabel;
    lbl12: TLabel;
    seSkill204LevelupMbTimer: TSpinEditEx;
    ts4: TTabSheet;
    ts5: TTabSheet;
    grp8: TGroupBox;
    chkSkill205ReduceMP: TCheckBox;
    grp9: TGroupBox;
    chkSkill206MbAttackMon: TCheckBox;
    chkSkill206MbAttackHuman: TCheckBox;
    chkSkill206MbAttackSlave: TCheckBox;
    chkSkill206MbFastParalysis: TCheckBox;
    seSkill204BasicMbTimer: TSpinEditEx;
    seSkill203BasicMbTimer: TSpinEditEx;
    seSkill203BasicPowerRate: TSpinEditEx;
    seSkill204BasicPowerRate: TSpinEditEx;
    grp10: TGroupBox;
    lbl14: TLabel;
    lbl15: TLabel;
    lbl16: TLabel;
    lbl17: TLabel;
    seSkill206BasicPowerRate: TSpinEditEx;
    seSkill206LevelupPowerRate: TSpinEditEx;
    seSkill206BasicMbTimer: TSpinEditEx;
    seSkill206LevelupMbTimer: TSpinEditEx;
    lbl18: TLabel;
    seSkill202CD: TSpinEditEx;
    lbl19: TLabel;
    seSkill205CD: TSpinEditEx;
    seSkill206CD: TSpinEditEx;
    lbl20: TLabel;
    lbl21: TLabel;
    seSkill203CD: TSpinEditEx;
    lbl22: TLabel;
    seSkill204CD: TSpinEditEx;
    lbl24: TLabel;
    seSkill114PowerRate: TSpinEditEx;
    lbl25: TLabel;
    lbl23: TLabel;
    seSkill114AttackRange: TSpinEditEx;
    lbl26: TLabel;
    lbl27: TLabel;
    seSkill41MbTimer0: TSpinEditEx;
    chkSkill41MbAttackSlave: TCheckBox;
    chkSkill41MbAttackPlayObject: TCheckBox;
    seMonthSpiritAttackRange: TSpinEditEx;
    lbl29: TLabel;
    lbl30: TLabel;
    seDeDingMagicBasicPowerRate: TSpinEditEx;
    lbl31: TLabel;
    seDeDingMagicAttackRange: TSpinEditEx;
    chkDedingAllowPK: TCheckBox;
    chkSkill71PullSlave: TCheckBox;
    lbl32: TLabel;
    seSkill71CD: TSpinEditEx;
    grp12: TGroupBox;
    lbl33: TLabel;
    sePosionDecHealthTime: TSpinEditEx;
    grp13: TGroupBox;
    lbl34: TLabel;
    sePosionDamagarmor: TSpinEditEx;
    Label145: TLabel;
    Label147: TLabel;
    Label149: TLabel;
    seHMPRockDecValue: TSpinEditEx;
    seHMPRockAddValue: TSpinEditEx;
    seHMPRockTime: TSpinEditEx;
    seHMPRockRate: TSpinEditEx;
    CheckBoxDropOverLapItem: TCheckBox;
    CheckBoxOpenMapEvent: TCheckBox;
    GroupBox57: TGroupBox;
    chkOpenSelfShop: TCheckBox;
    chkSafeZoneShop: TCheckBox;
    chkMapShop: TCheckBox;
    GroupBox48: TGroupBox;
    Label195: TLabel;
    Label196: TLabel;
    Label562: TLabel;
    Label563: TLabel;
    seSellOffGoldTaxRate: TSpinEditEx;
    seSellOffGameGoldTaxRate: TSpinEditEx;
    chkLockSummonHero: TCheckBox;
    chkLockShop: TCheckBox;
    chkLockChallenge: TCheckBox;
    chkLockStall: TCheckBox;
    lbl38: TLabel;
    seSkill41MbRange0: TSpinEditEx;
    Label564: TLabel;
    seSkill205Rage: TSpinEditEx;
    seSkill206Rage: TSpinEditEx;
    Label584: TLabel;
    Label588: TLabel;
    seSkill204Rage: TSpinEditEx;
    seSkill203Rage: TSpinEditEx;
    Label589: TLabel;
    pgcBonusAbilof: TPageControl;
    ts6: TTabSheet;
    Label581: TLabel;
    ButtonOther: TButton;
    GroupBox124: TGroupBox;
    Label250: TLabel;
    EditMysteriousManName: TEdit;
    GroupBox178: TGroupBox;
    Label567: TLabel;
    Label568: TLabel;
    EditDamageItemDuraRate: TSpinEditEx;
    GroupBox194: TGroupBox;
    CheckBoxDuraChangeLight: TCheckBox;
    GroupBox196: TGroupBox;
    CheckBoxPoisonWeaponCanMagicAttack: TCheckBox;
    CheckBoxPoisonWeaponCanHitAllTarget: TCheckBox;
    GroupBox197: TGroupBox;
    Label587: TLabel;
    EditQueryBagItemsTime: TSpinEditEx;
    EditMaxLuckMaxPower: TSpinEditEx;
    chkGroupReCallNotInSafeZone: TCheckBox;
    chkNewHumanAttatckMode_HAM_PEACE: TCheckBox;
    chkAutoGroupMaster: TCheckBox;
    chkGuardNotAttackPlayMoster: TCheckBox;
    Label590: TLabel;
    seElecBlizzardPowerRate: TSpinEditEx;
    Label591: TLabel;
    lbl40: TLabel;
    seFireBoomRagePowerRate: TSpinEditEx;
    lbl41: TLabel;
    lbl42: TLabel;
    seSnowWindPowerRate: TSpinEditEx;
    lbl43: TLabel;
    lbl44: TLabel;
    seMakeFireDayPowerRate: TSpinEditEx;
    lbl45: TLabel;
    chkDoMotaeboPushSameLevel: TCheckBox;
    Label565: TLabel;
    seDoMotaeboCD: TSpinEditEx;
    Label566: TLabel;
    ts7: TTabSheet;
    grp16: TGroupBox;
    lbl46: TLabel;
    lbl48: TLabel;
    seSkill15PowerRate: TSpinEditEx;
    lbl47: TLabel;
    chkWarNoDropUseItem: TCheckBox;
    CheckBoxDeleteItemDuraZero: TCheckBox;
    lbl49: TLabel;
    lbl50: TLabel;
    seSkill15TimeRate: TSpinEditEx;
    lbl51: TLabel;
    seSkill205PowerRate: TSpinEditEx;
    TabSheet91: TTabSheet;
    GroupBox54: TGroupBox;
    Label569: TLabel;
    seBonusAbilofWarrDC: TSpinEditEx;
    Label592: TLabel;
    seBonusAbilofWarrMC: TSpinEditEx;
    Label593: TLabel;
    seBonusAbilofWarrSC: TSpinEditEx;
    Label594: TLabel;
    seBonusAbilofWarrAC: TSpinEditEx;
    Label595: TLabel;
    seBonusAbilofWarrMAC: TSpinEditEx;
    Label596: TLabel;
    seBonusAbilofWarrHP: TSpinEditEx;
    Label597: TLabel;
    seBonusAbilofWarrMP: TSpinEditEx;
    Label598: TLabel;
    seBonusAbilofWarrHit: TSpinEditEx;
    Label599: TLabel;
    seBonusAbilofWarrSpeed: TSpinEditEx;
    GroupBox62: TGroupBox;
    seBonusAbilofWizardDC: TSpinEditEx;
    seBonusAbilofWizardMC: TSpinEditEx;
    seBonusAbilofWizardSC: TSpinEditEx;
    seBonusAbilofWizardAC: TSpinEditEx;
    seBonusAbilofWizardMAC: TSpinEditEx;
    seBonusAbilofWizardHP: TSpinEditEx;
    seBonusAbilofWizardMP: TSpinEditEx;
    seBonusAbilofWizardHit: TSpinEditEx;
    seBonusAbilofWizardSpeed: TSpinEditEx;
    GroupBox63: TGroupBox;
    seBonusAbilofTaosDC: TSpinEditEx;
    seBonusAbilofTaosMC: TSpinEditEx;
    seBonusAbilofTaosSC: TSpinEditEx;
    seBonusAbilofTaosAC: TSpinEditEx;
    seBonusAbilofTaosMAC: TSpinEditEx;
    seBonusAbilofTaosHP: TSpinEditEx;
    seBonusAbilofTaosMP: TSpinEditEx;
    seBonusAbilofTaosHit: TSpinEditEx;
    seBonusAbilofTaosSpeed: TSpinEditEx;
    btnBonusAbilofSave: TButton;
    Label618: TLabel;
    Label600: TLabel;
    Label601: TLabel;
    Label602: TLabel;
    Label603: TLabel;
    Label604: TLabel;
    Label605: TLabel;
    Label606: TLabel;
    Label607: TLabel;
    Label608: TLabel;
    Label609: TLabel;
    Label610: TLabel;
    Label611: TLabel;
    Label612: TLabel;
    Label613: TLabel;
    Label614: TLabel;
    Label615: TLabel;
    Label616: TLabel;
    Label617: TLabel;
    chkCloseSuperShiledHint: TCheckBox;
    GroupBox88: TGroupBox;
    Label619: TLabel;
    Panel1: TPanel;
    Panel3: TPanel;
    Label620: TLabel;
    Label621: TLabel;
    Label622: TLabel;
    seElectrodelessTimeL0: TSpinEditEx;
    seElectrodelessTimeL1: TSpinEditEx;
    seElectrodelessTimeL2: TSpinEditEx;
    seElectrodelessTimeL3: TSpinEditEx;
    ts8: TTabSheet;
    pgc1: TPageControl;
    ts9: TTabSheet;
    ts10: TTabSheet;
    ts11: TTabSheet;
    grp17: TGroupBox;
    lbl52: TLabel;
    lbl53: TLabel;
    seMagicNewLevelPower0: TSpinEditEx;
    cbbMagicNewLevel0: TComboBox;
    grp18: TGroupBox;
    lbl54: TLabel;
    lbl55: TLabel;
    seMagicNewLevelPower1: TSpinEditEx;
    cbbMagicNewLevel1: TComboBox;
    grp19: TGroupBox;
    lbl56: TLabel;
    lbl57: TLabel;
    seMagicNewLevelPower2: TSpinEditEx;
    cbbMagicNewLevel2: TComboBox;
    grp20: TGroupBox;
    lbl58: TLabel;
    lbl59: TLabel;
    seMagicNewLevelPower3: TSpinEditEx;
    cbbMagicNewLevel3: TComboBox;
    grp21: TGroupBox;
    lbl60: TLabel;
    lbl61: TLabel;
    seMagicNewLevelPower4: TSpinEditEx;
    cbbMagicNewLevel4: TComboBox;
    grp22: TGroupBox;
    lbl62: TLabel;
    lbl63: TLabel;
    seMagicNewLevelPower5: TSpinEditEx;
    cbbMagicNewLevel5: TComboBox;
    grp23: TGroupBox;
    lbl64: TLabel;
    lbl65: TLabel;
    seMagicNewLevelPower20: TSpinEditEx;
    cbbMagicNewLevel20: TComboBox;
    grp24: TGroupBox;
    lbl66: TLabel;
    lbl67: TLabel;
    seMagicNewLevelPower21: TSpinEditEx;
    cbbMagicNewLevel21: TComboBox;
    grp25: TGroupBox;
    lbl68: TLabel;
    lbl69: TLabel;
    seMagicNewLevelPower22: TSpinEditEx;
    cbbMagicNewLevel22: TComboBox;
    grp26: TGroupBox;
    lbl70: TLabel;
    lbl71: TLabel;
    seMagicNewLevelPower23: TSpinEditEx;
    cbbMagicNewLevel23: TComboBox;
    grp27: TGroupBox;
    lbl72: TLabel;
    lbl73: TLabel;
    seMagicNewLevelPower24: TSpinEditEx;
    cbbMagicNewLevel24: TComboBox;
    grp28: TGroupBox;
    lbl74: TLabel;
    lbl75: TLabel;
    seMagicNewLevelPower40: TSpinEditEx;
    cbbMagicNewLevel40: TComboBox;
    grp29: TGroupBox;
    lbl76: TLabel;
    lbl77: TLabel;
    seMagicNewLevelPower41: TSpinEditEx;
    cbbMagicNewLevel41: TComboBox;
    grp30: TGroupBox;
    lbl78: TLabel;
    lbl79: TLabel;
    seMagicNewLevelPower42: TSpinEditEx;
    cbbMagicNewLevel42: TComboBox;
    grp31: TGroupBox;
    lbl80: TLabel;
    lbl81: TLabel;
    seMagicNewLevelPower43: TSpinEditEx;
    cbbMagicNewLevel43: TComboBox;
    grp32: TGroupBox;
    lbl82: TLabel;
    lbl83: TLabel;
    seMagicNewLevelPower44: TSpinEditEx;
    cbbMagicNewLevel44: TComboBox;
    grp33: TGroupBox;
    lbl84: TLabel;
    lbl85: TLabel;
    seMagicNewLevelPower45: TSpinEditEx;
    cbbMagicNewLevel45: TComboBox;
    GroupBox75: TGroupBox;
    Label178: TLabel;
    Label179: TLabel;
    Label180: TLabel;
    Label181: TLabel;
    EditMaxAngryValue: TSpinEditEx;
    EditAddAngryValue: TSpinEditEx;
    EditDecFirDragonPoint: TSpinEditEx;
    EditAddAngryValueTime: TSpinEditEx;
    GroupBox69: TGroupBox;
    Label165: TLabel;
    seSkill60PowerRate: TSpinEditEx;
    GroupBox70: TGroupBox;
    Label168: TLabel;
    seSkill61PowerRate: TSpinEditEx;
    GroupBox71: TGroupBox;
    Label170: TLabel;
    seSkill62PowerRate: TSpinEditEx;
    GroupBox72: TGroupBox;
    Label172: TLabel;
    seSkill63PowerRate: TSpinEditEx;
    GroupBox73: TGroupBox;
    Label174: TLabel;
    seSkill64PowerRate: TSpinEditEx;
    GroupBox74: TGroupBox;
    Label176: TLabel;
    seSkill65PowerRate: TSpinEditEx;
    grp34: TGroupBox;
    chkHeroJointAttack: TCheckBox;
    Label627: TLabel;
    seSkill65PowerRange: TSpinEditEx;
    Label628: TLabel;
    seSkill64PowerRange: TSpinEditEx;
    Label629: TLabel;
    seSkill60PowerRange: TSpinEditEx;
    Label630: TLabel;
    seSkill63PowerRange: TSpinEditEx;
    chkSkill63GreenPoison: TCheckBox;
    chkShowMsgMagicRangeExceed: TCheckBox;
    chkShopStallCanNotAttack: TCheckBox;
    chkShowMysteriousMan: TCheckBox;
    ts14: TTabSheet;
    GroupBox82: TGroupBox;
    Label161: TLabel;
    Label185: TLabel;
    seMagicNewLevelPower: TSpinEditEx;
    cbbMagicNewLevel: TComboBox;
    lbl91: TLabel;
    seNewLevelMagicPowerRatesAfter9: TSpinEditEx;
    ts15: TTabSheet;
    grp38: TGroupBox;
    lbl92: TLabel;
    seOrdinarySkill31Rate: TSpinEditEx;
    grp39: TGroupBox;
    lbl93: TLabel;
    seSkill31Rate: TSpinEditEx;
    PageControlHeroConfig: TPageControl;
    TabSheet44: TTabSheet;
    GroupBox64: TGroupBox;
    Label183: TLabel;
    Label184: TLabel;
    EditNeedGuardLevel: TSpinEditEx;
    EditGuardRange: TSpinEditEx;
    chkHeroDisableSafeZoneProtect: TCheckBox;
    GroupBox67: TGroupBox;
    Label158: TLabel;
    Label162: TLabel;
    Label163: TLabel;
    EditHeroWarrorAttackTime: TSpinEditEx;
    EditHeroTaoistAttackTime: TSpinEditEx;
    EditHeroWizardAttackTime: TSpinEditEx;
    GroupBox65: TGroupBox;
    Label164: TLabel;
    Label166: TLabel;
    CheckBoxHeroShowMasterName: TCheckBox;
    EditHeroNameColor: TColorIndexEdit;
    EditHeroSuffixName: TEdit;
    GroupBox76: TGroupBox;
    Label182: TLabel;
    EditNeedLevel: TSpinEditEx;
    ComboBoxBagItemCount: TComboBox;
    GroupBox66: TGroupBox;
    Label152: TLabel;
    Label154: TLabel;
    Label156: TLabel;
    EditHeroWarrorWalkTime: TSpinEditEx;
    EditHeroWizardWalkTime: TSpinEditEx;
    EditHeroTaoistWalkTime: TSpinEditEx;
    GroupBox201: TGroupBox;
    chkHeroDisableStruck: TCheckBox;
    chkHeroDisableSelfStruck: TCheckBox;
    GroupBox202: TGroupBox;
    Label151: TLabel;
    Label271: TLabel;
    EditHeroRecallTime: TSpinEditEx;
    EditRecallDeputyHeroTime: TSpinEditEx;
    grp35: TGroupBox;
    lbl88: TLabel;
    Label633: TLabel;
    seHeroMasterStartLevel: TSpinEditEx;
    seHeroSlaveStartLevel: TSpinEditEx;
    grp40: TGroupBox;
    chkHeroCalcWeaponSpeed: TCheckBox;
    chkCreditPointWithLevel: TCheckBox;
    CheckBoxHeroPickUpItem: TCheckBox;
    TabSheet45: TTabSheet;
    GroupBox106: TGroupBox;
    CheckBoxKillByMonstDropHeroUseItem: TCheckBox;
    CheckBoxKillByHumanDropHeroUseItem: TCheckBox;
    CheckBoxDieScatterHeroBag: TCheckBox;
    CheckBoxDieRedScatterHeroBagAll: TCheckBox;
    GroupBox105: TGroupBox;
    Label223: TLabel;
    Label224: TLabel;
    Label225: TLabel;
    ScrollBarDieDropHeroUseItemRate: TScrollBar;
    EditDieDropHeroUseItemRate: TEdit;
    ScrollBarDieRedDropHeroUseItemRate: TScrollBar;
    EditDieRedDropHeroUseItemRate: TEdit;
    ScrollBarDieScatterHeroBagRate: TScrollBar;
    EditDieScatterHeroBagRate: TEdit;
    GroupBox177: TGroupBox;
    Label631: TLabel;
    lbl86: TLabel;
    seHeroDieExpRate: TSpinEditEx;
    ts12: TTabSheet;
    GroupBox176: TGroupBox;
    chkHeroCallBB: TCheckBox;
    seHeroCallBBCount: TSpinEditEx;
    chkHero700HPUseBaseAttack: TCheckBox;
    chkHeroTaosAutoChangePoison: TCheckBox;
    GroupBox180: TGroupBox;
    Label640: TLabel;
    Label641: TLabel;
    seHeroGotoLV4: TSpinEditEx;
    seHeroPowerLV4: TSpinEditEx;
    GroupBox192: TGroupBox;
    Label642: TLabel;
    Label643: TLabel;
    Label644: TLabel;
    Label645: TLabel;
    Label646: TLabel;
    seHeroFealtyCallAdd: TSpinEditEx;
    seHeroFealtyExp: TSpinEditEx;
    seHeroFealtyExpAdd: TSpinEditEx;
    seHeroFealtyCallBackDel: TSpinEditEx;
    seHeroFealtyDeathDel: TSpinEditEx;
    grp36: TGroupBox;
    Label636: TLabel;
    Label637: TLabel;
    Label638: TLabel;
    lbl87: TLabel;
    Label632: TLabel;
    lbl89: TLabel;
    seHeroWarrHPMPRate: TSpinEditEx;
    seHeroWizardHPMPRate: TSpinEditEx;
    seHeroTaosHPMPRate: TSpinEditEx;
    ts13: TTabSheet;
    GroupBox198: TGroupBox;
    Label647: TLabel;
    GridLevelExp: TStringGrid;
    ComboBoxLevelExp: TComboBox;
    GroupBox199: TGroupBox;
    Label648: TLabel;
    Label649: TLabel;
    seHeroHighLevel: TSpinEditEx;
    seHeroHighLevelGetExp: TSpinEditEx;
    GroupBox200: TGroupBox;
    Label650: TLabel;
    seHeroLevel1000FixedExp: TSpinEditLongWord;
    grp37: TGroupBox;
    Label150: TLabel;
    Label127: TLabel;
    lbl90: TLabel;
    Label639: TLabel;
    chkHeroGetAllExp: TCheckBox;
    seHeroKillMonExpRate: TSpinEditEx;
    seHeroNotKillMonExpRate: TSpinEditEx;
    GroupBox203: TGroupBox;
    lbl94: TLabel;
    edtCopySelfSuffix: TEdit;
    chkShowCopySelfSuffix: TCheckBox;
    chkAlwaysFollowMasterAttack: TCheckBox;
    grp41: TGroupBox;
    chkKillHeroWeaponUnlock: TCheckBox;
    Label634: TLabel;
    seKillHeroWeaponUnlockRate: TSpinEditEx;
    chkHumanGetAllExp: TCheckBox;
    Label623: TLabel;
    seClearHeroGhostTick: TSpinEditEx;
    Label188: TLabel;
    seSkill42HitWaitTime: TSpinEditEx;
    Label189: TLabel;
    lbl95: TLabel;
    seSkill42Range: TSpinEditEx;
    lbl96: TLabel;
    seSkill72Rate: TSpinEditEx;
    grp42: TGroupBox;
    chkContinuousAttackUseNG: TCheckBox;
    GroupBox84: TGroupBox;
    chkSkill114AttackUseNG: TCheckBox;
    ts16: TTabSheet;
    btnOther3: TButton;
    grp43: TGroupBox;
    chkRevivalTouch: TCheckBox;
    seRevivalTime: TSpinEditEx;
    lbl97: TLabel;
    lbl98: TLabel;
    seLimitScriptGotoCount: TSpinEditEx;
    lbl99: TLabel;
    GroupBox119: TGroupBox;
    chkFBDisableDelay30s: TCheckBox;
    chkFBExitCreaterOffline: TCheckBox;
    chkShowSuperShiledSound: TCheckBox;
    chkShowSuperShiledEffect2: TCheckBox;
    chkShowSuperShiledSound2: TCheckBox;
    chkHeroAutoSuperShiled: TCheckBox;
    chkMagicNotHinder: TCheckBox;
    chkMagicDefinition: TCheckBox;
    grp44: TGroupBox;
    chkHeroCanUseMootebo: TCheckBox;
    chkWarrorAttack: TCheckBox;
    grp45: TGroupBox;
    lbl100: TLabel;
    seHeroAttackRange: TSpinEditEx;
    chkShopHeadPic: TCheckBox;
    ts17: TTabSheet;
    GroupBox205: TGroupBox;
    Label652: TLabel;
    Label653: TLabel;
    Label654: TLabel;
    seSKILL209CD: TSpinEditEx;
    seSkill209Rage: TSpinEditEx;
    seSkill209PowerRate: TSpinEditEx;
    ts18: TTabSheet;
    grp46: TGroupBox;
    lbl101: TLabel;
    lbl102: TLabel;
    lbl103: TLabel;
    seSKILL210CD: TSpinEditEx;
    seSkill210Rage: TSpinEditEx;
    seSkill210PowerRate: TSpinEditEx;
    ts19: TTabSheet;
    GroupBox206: TGroupBox;
    Label655: TLabel;
    Label656: TLabel;
    Label657: TLabel;
    seSKILL208CD: TSpinEditEx;
    seSkill208Rage: TSpinEditEx;
    seSkill208PowerRate: TSpinEditEx;
    chkSkill204SameLevel: TCheckBox;
    chkSkill206Frozen: TCheckBox;
    chkMagTurnUndeadSameLevel: TCheckBox;
    chkSkill206SameLevel: TCheckBox;
    GroupBox207: TGroupBox;
    chkJewelryCalcBasicAbilitys: TCheckBox;
    lbl104: TLabel;
    seAngryAgainValue: TSpinEditEx;
    Label658: TLabel;
    chkSKILL208HeroDuanJin: TCheckBox;
    chkSKILL208PlayMosterDuanJin: TCheckBox;
    GroupBox208: TGroupBox;
    Label659: TLabel;
    seRecallCopySelfWaitTime: TSpinEditEx;
    Label660: TLabel;
    Label661: TLabel;
    seRecallDogzWaitTime: TSpinEditEx;
    Label662: TLabel;
    Label663: TLabel;
    seRecallBoneFammWaitTime: TSpinEditEx;
    Label664: TLabel;
    Label665: TLabel;
    seRecallMonthSpiritWaitTime: TSpinEditEx;
    Label666: TLabel;
    CheckBoxMonthSpiritUseMasterMP: TCheckBox;
    CheckBoxMonthSpiritAttackSame: TCheckBox;
    Label667: TLabel;
    seHeroTaoUsePoisonMinHP: TSpinEditEx;
    GroupBox209: TGroupBox;
    Label668: TLabel;
    seContinuousProtect: TSpinEditEx;
    Label669: TLabel;
    seContinuousProtectRandom: TSpinEditEx;
    Label670: TLabel;
    Label671: TLabel;
    chkHeroFollowMasterWithDiffScreen: TCheckBox;
    GroupBox210: TGroupBox;
    chkSlaveNotAttackHuman: TCheckBox;
    chkSlaveNotAttackHero: TCheckBox;
    grp49: TGroupBox;
    lbl108: TLabel;
    edtPlusBoneFammName1_3: TEdit;
    Label674: TLabel;
    edtPlusBoneFammName4_6: TEdit;
    lbl109: TLabel;
    edtPlusBoneFammName7_9: TEdit;
    GroupBox211: TGroupBox;
    Label675: TLabel;
    Label676: TLabel;
    Label677: TLabel;
    edtPlusDogzName1_3: TEdit;
    edtPlusDogzName4_6: TEdit;
    edtPlusDogzName7_9: TEdit;
    GroupBox212: TGroupBox;
    Label678: TLabel;
    Label679: TLabel;
    seMagicNewLevelPower25: TSpinEditEx;
    cbbMagicNewLevel25: TComboBox;
    GroupBox213: TGroupBox;
    Label680: TLabel;
    Label681: TLabel;
    Label682: TLabel;
    sePlusBoneFammLevel1: TSpinEditEx;
    Label683: TLabel;
    sePlusBoneFammLevel2: TSpinEditEx;
    Label684: TLabel;
    sePlusBoneFammLevel3: TSpinEditEx;
    sePlusBoneFammLevel4: TSpinEditEx;
    Label685: TLabel;
    sePlusBoneFammLevel5: TSpinEditEx;
    Label686: TLabel;
    sePlusBoneFammLevel6: TSpinEditEx;
    sePlusBoneFammLevel7: TSpinEditEx;
    Label687: TLabel;
    sePlusBoneFammLevel8: TSpinEditEx;
    Label688: TLabel;
    sePlusBoneFammLevel9: TSpinEditEx;
    GroupBox214: TGroupBox;
    Label689: TLabel;
    Label690: TLabel;
    Label691: TLabel;
    Label692: TLabel;
    Label693: TLabel;
    Label694: TLabel;
    Label695: TLabel;
    Label696: TLabel;
    Label697: TLabel;
    sePlusDogzLevel1: TSpinEditEx;
    sePlusDogzLevel2: TSpinEditEx;
    sePlusDogzLevel3: TSpinEditEx;
    sePlusDogzLevel4: TSpinEditEx;
    sePlusDogzLevel5: TSpinEditEx;
    sePlusDogzLevel6: TSpinEditEx;
    sePlusDogzLevel7: TSpinEditEx;
    sePlusDogzLevel8: TSpinEditEx;
    sePlusDogzLevel9: TSpinEditEx;
    Label698: TLabel;
    edtPlusBoneFammName9_N: TEdit;
    lbl110: TLabel;
    sePlusBoneFammLevelAfter9: TSpinEditEx;
    Label699: TLabel;
    edtPlusDogzName9_N: TEdit;
    Label700: TLabel;
    sePlusDogzLevelAfter9: TSpinEditEx;
    Label701: TLabel;
    Label702: TLabel;
    seHeroSkill114HitWaitTime: TSpinEditEx;
    Label703: TLabel;
    Label704: TLabel;
    seHeroSkill66HitWaitTime: TSpinEditEx;
    grp50: TGroupBox;
    Label114: TLabel;
    Label136: TLabel;
    Label137: TLabel;
    seHPRockRate: TSpinEditEx;
    seHPRockTime: TSpinEditEx;
    seHPRockAddValue: TSpinEditEx;
    seHPRockDecValue: TSpinEditEx;
    lbl111: TLabel;
    grp51: TGroupBox;
    Label139: TLabel;
    Label141: TLabel;
    Label143: TLabel;
    seMPRockDecValue: TSpinEditEx;
    seMPRockAddValue: TSpinEditEx;
    seMPRockTime: TSpinEditEx;
    seMPRockRate: TSpinEditEx;
    GroupBoxHunger: TGroupBox;
    CheckBoxHungerDecPower: TCheckBox;
    CheckBoxHungerDecHP: TCheckBox;
    GroupBox55: TGroupBox;
    Label118: TLabel;
    CheckBoxItemName: TCheckBox;
    EditItemName: TEdit;
    GroupBox215: TGroupBox;
    Label134: TLabel;
    Label135: TLabel;
    seSnowwindWaitTime: TSpinEditEx;
    GroupBox216: TGroupBox;
    Label705: TLabel;
    Label706: TLabel;
    seJSOfGameGoldTaxRate: TSpinEditEx;
    Label140: TLabel;
    EditMonUpgradeColor0: TColorIndexEdit;
    chkJewelryCalcGroupAbilitys: TCheckBox;
    chkSaveRevivalTime: TCheckBox;
    lbl35: TLabel;
    seNewLevelMagic43LSFRate: TSpinEditEx;
    Label146: TLabel;
    seSkill57Time: TSpinEditEx;
    Label707: TLabel;
    Label708: TLabel;
    seMakeFireDayTime: TSpinEditEx;
    Label709: TLabel;
    chkSkill204RunObstacle: TCheckBox;
    grp52: TGroupBox;
    Label710: TLabel;
    seMasterCount: TSpinEditEx;
    CheckBoxSlaveRelaxCanStruck: TCheckBox;
    GroupBox189: TGroupBox;
    Label714: TLabel;
    Label715: TLabel;
    seBBAttrPlusNewLevelRate: TSpinEditEx;
    cbbBBAttrPlusNewLevel: TComboBox;
    grp53: TGroupBox;
    GroupBox217: TGroupBox;
    Label711: TLabel;
    seBBAttrPlusAddAttackRate: TSpinEditEx;
    GroupBox218: TGroupBox;
    Label712: TLabel;
    chkBBAttrPlusAddDefence: TCheckBox;
    chkBBAttrPlusAddMagicDefence: TCheckBox;
    seBBAttrPlusAddDefenceRate: TSpinEditEx;
    GroupBox219: TGroupBox;
    Label713: TLabel;
    chkBBAttrPlusAddHP: TCheckBox;
    seBBAttrPlusAddHPRate: TSpinEditEx;
    chkBBAttrPlusAddOnlyMagic: TCheckBox;
    cbbBBAttrPlusAddAttackForm: TComboBox;
    chkBBAttrPlusAddAttack: TCheckBox;
    chkShowRefreshBagMsg: TCheckBox;
    chkKillByHumanDropHeroJewelryBoxItem: TCheckBox;
    chkKillByMonstDropHeroJewelryBoxItem: TCheckBox;
    chkKillByHumanDropHeroGodBlessItem: TCheckBox;
    chkKillByMonstDropHeroGodBlessItem: TCheckBox;
    Label716: TLabel;
    Label717: TLabel;
    scrlbrHeroJewelryBoxItem: TScrollBar;
    edtHeroJewelryBoxItem: TEdit;
    scrlbrHeroGodBlessItem: TScrollBar;
    edtHeroGodBlessItem: TEdit;
    Label718: TLabel;
    Label719: TLabel;
    seSellOffGameDiamondTaxRate: TSpinEditEx;
    Label720: TLabel;
    Label721: TLabel;
    seSellOffGameGirdTaxRate: TSpinEditEx;
    grp54: TGroupBox;
    chkMyShopGold: TCheckBox;
    chkMyShopGameGold: TCheckBox;
    chkMyShopGameDiamond: TCheckBox;
    chkMyShopGameGird: TCheckBox;
    chkMyShopGamePoint: TCheckBox;
    Label722: TLabel;
    seSellOffGamePointTaxRate: TSpinEditEx;
    Label723: TLabel;
    lbl36: TLabel;
    seSkill206FrozenRate: TSpinEditEx;
    grp55: TGroupBox;
    Label724: TLabel;
    seSkill43LDMBRate: TSpinEditEx;
    Label725: TLabel;
    Label726: TLabel;
    seSkill43LDMBTime: TSpinEditEx;
    Label727: TLabel;
    Label728: TLabel;
    seSkill43LDMBPowerAdd: TSpinEditEx;
    Label729: TLabel;
    GroupBox220: TGroupBox;
    Label730: TLabel;
    Label731: TLabel;
    seSkill43HitWaitTime: TSpinEditEx;
    chkSkill43LockParaly: TCheckBox;
    chkSkill58PowerTwoAttack: TCheckBox;
    grp56: TGroupBox;
    Label732: TLabel;
    seHeroLimit: TSpinEditEx;
    Label733: TLabel;
    seHeroRunTime: TSpinEditEx;
    grp57: TGroupBox;
    Label734: TLabel;
    seHeroWarrAttackMoveRate: TSpinEditEx;
    GroupBox221: TGroupBox;
    chkMySellShopItemTime: TCheckBox;
    seMySellShopItemTime: TSpinEditEx;
    lbl37: TLabel;
    chkDedingDisabledPK: TCheckBox;
    GroupBox222: TGroupBox;
    Label735: TLabel;
    seNearAttackPowerRate: TSpinEditEx;
    chkHeroJointAttackFly: TCheckBox;
    chkHeroTargetAgainNoMove: TCheckBox;
    chkHeroTargetAgainNoMoveDF: TCheckBox;
    chkSkill205PowerTwoAttack: TCheckBox;
    grp58: TGroupBox;
    chkSkill31UseNewEffect: TCheckBox;
    grp59: TGroupBox;
    lbl112: TLabel;
    Label736: TLabel;
    seSkill46PowerBase: TSpinEditEx;
    seSkill46SecRate: TSpinEditEx;
    Label737: TLabel;
    Label740: TLabel;
    Label741: TLabel;
    seSkill46Time: TSpinEditEx;
    lbl113: TLabel;
    Label738: TLabel;
    seGuardNameColor: TColorIndexEdit;
    grp60: TGroupBox;
    chkOpenNewGuild: TCheckBox;
    grp61: TGroupBox;
    chkMonNoAttackOffLinePlayer: TCheckBox;
    chkSkill64MakeStone: TCheckBox;
    grp62: TGroupBox;
    lbl114: TLabel;
    Label743: TLabel;
    seSkillJointAttackLevelRate: TSpinEditEx;
    GroupBox223: TGroupBox;
    Label744: TLabel;
    Label745: TLabel;
    seContinuousAttackLevelRate: TSpinEditEx;
    ts20: TTabSheet;
    GroupBox224: TGroupBox;
    Label748: TLabel;
    Label749: TLabel;
    seSkill69CD: TSpinEditEx;
    Label747: TLabel;
    Label750: TLabel;
    seSkill69AddTime: TSpinEditEx;
    Label751: TLabel;
    Label752: TLabel;
    seSkill69AddRange: TSpinEditEx;
    Label754: TLabel;
    seHeroWarrAttacSkillErgumRate: TSpinEditEx;
    Label746: TLabel;
    seHeroWarrAttakNear: TSpinEditEx;
    cbbHeroWarriorDefaultSkill: TComboBox;
    Label753: TLabel;
    chkSkill69SameLevel: TCheckBox;
    lbl115: TLabel;
    edtJewelryBoxHint: TEdit;
    lbl116: TLabel;
    Label755: TLabel;
    seCopySelfNameColor: TColorIndexEdit;
    grp63: TGroupBox;
    lbl117: TLabel;
    seHeroRecallCopySelfHPRate: TSpinEditEx;
    lbl118: TLabel;
    chkSkill61NotMagBubbleDefence: TCheckBox;
    grp64: TGroupBox;
    seMySellShowItemNamLen: TSpinEditEx;
    lbl119: TLabel;
    chkCloseNPCNoItemMsg: TCheckBox;
    Label756: TLabel;
    seMerchant273NameColor: TColorIndexEdit;
    chkSkill15OfflineClear: TCheckBox;
    GroupBox112: TGroupBox;
    Label773: TLabel;
    seMagicMsgX: TSpinEditEx;
    Label774: TLabel;
    seMagicMsgY: TSpinEditEx;
    grp65: TGroupBox;
    lbl121: TLabel;
    seStarBaseNum: TSpinEditEx;
    chkJewelryDecDura: TCheckBox;
    grp66: TGroupBox;
    chkRecordBeadExp: TCheckBox;
    chkSlaveLockTarget: TCheckBox;
    chkSkill62NotMagBubbleDefence: TCheckBox;
    ts21: TTabSheet;
    grp67: TGroupBox;
    lstMagicAC: TListBox;
    grp68: TGroupBox;
    chkMagicACEnabled: TCheckBox;
    lbl122: TLabel;
    Label775: TLabel;
    seMagicACHum: TSpinEditEx;
    Label776: TLabel;
    seMagicACMon: TSpinEditEx;
    Label777: TLabel;
    Label778: TLabel;
    seMagicACHero: TSpinEditEx;
    Label779: TLabel;
    lbl123: TLabel;
    grp69: TGroupBox;
    lbl39: TLabel;
    seHongMoSuiteRate: TSpinEditEx;
    chkHongMoSuiteWithPower: TCheckBox;
    grp70: TGroupBox;
    chkDisableDuFuTakeArmRingL: TCheckBox;
    GroupBox113: TGroupBox;
    Label780: TLabel;
    sePosionDecMACRate: TSpinEditEx;
    chkEnabledPosionDecMAC: TCheckBox;
    chkPosionStopIncHealth: TCheckBox;
    lbl124: TLabel;
    GroupBox227: TGroupBox;
    Label781: TLabel;
    Label782: TLabel;
    seSkillYedoPowerRate: TSpinEditEx;
    chkDogzPlugSettingPriority: TCheckBox;
    chkBonePlugSettingPriority: TCheckBox;
    ts22: TTabSheet;
    GroupBox229: TGroupBox;
    GroupBox230: TGroupBox;
    Label786: TLabel;
    seSpiritualismRoyaltyTime: TSpinEditEx;
    GroupBox188: TGroupBox;
    chkSpiritualismLevelDiff: TCheckBox;
    seSpiritualismLevelDiff: TSpinEditEx;
    lbl125: TLabel;
    cbbSpiritualismMagicLevel: TComboBox;
    Label582: TLabel;
    seSpiritualismRate: TSpinEditEx;
    lbl126: TLabel;
    GroupBox228: TGroupBox;
    Label783: TLabel;
    seSpiritualismBBCount: TSpinEditEx;
    GroupBox231: TGroupBox;
    Label784: TLabel;
    lbl127: TLabel;
    lbl128: TLabel;
    lbl129: TLabel;
    lbl130: TLabel;
    lbl131: TLabel;
    seSlave9HP: TSpinEditEx;
    seSlave9AC: TSpinEditEx;
    seSlave9MAC: TSpinEditEx;
    seSlave9DC: TSpinEditEx;
    seSlave9MoveSpeed: TSpinEditEx;
    seSlave9HitSpeed: TSpinEditEx;
    Label785: TLabel;
    seSkill202Rate: TSpinEditEx;
    chkSkill202RateOnlyMon: TCheckBox;
    GroupBox232: TGroupBox;
    Label787: TLabel;
    seSkillAmyounsulCDTime: TSpinEditEx;
    Label788: TLabel;
    Label789: TLabel;
    seSkillGroupAmyounsulCDTime: TSpinEditEx;
    Label790: TLabel;
    GroupBox233: TGroupBox;
    chkSkillGroupAmyounsulRed: TCheckBox;
    chkSkillGroupAmyounsulGreen: TCheckBox;
    edtSetOffLineLoginMapName: TEdit;
    lbl132: TLabel;
    chkSlaveKillHumanIncPK: TCheckBox;
    chkSlaveLevelupUseNewAttr: TCheckBox;
    chkSlaveLevelupAddLowerAttr: TCheckBox;
    tsHeroExt: TTabSheet;
    chkHeroForcePeaceMode: TCheckBox;
    grp71: TGroupBox;
    lbl133: TLabel;
    seHeroAttackHumPowerRate: TSpinEditEx;
    lbl134: TLabel;
    Label651: TLabel;
    seHeroAttackMonPowerRate: TSpinEditEx;
    Label791: TLabel;
    grp72: TGroupBox;
    chkHeroStatus0: TCheckBox;
    chkHeroStatus1: TCheckBox;
    chkHeroStatus2: TCheckBox;
    chkHeroStatus3: TCheckBox;
    chkNoNeedFirDragon: TCheckBox;
    grp73: TGroupBox;
    Label792: TLabel;
    Label793: TLabel;
    seMonAttackHeroPowerRate: TSpinEditEx;
    Label794: TLabel;
    seSkill60AttackHumPowerRate: TSpinEditEx;
    Label796: TLabel;
    seSkill61AttackHumPowerRate: TSpinEditEx;
    Label798: TLabel;
    seSkill62AttackHumPowerRate: TSpinEditEx;
    Label800: TLabel;
    seSkill65AttackHumPowerRate: TSpinEditEx;
    Label802: TLabel;
    seSkill64AttackHumPowerRate: TSpinEditEx;
    Label804: TLabel;
    seSkill63AttackHumPowerRate: TSpinEditEx;
    Label167: TLabel;
    seSkill204BasicMbRate: TSpinEditEx;
    GroupBox204: TGroupBox;
    Label169: TLabel;
    Label171: TLabel;
    seHumanAttackHeroPowerRate: TSpinEditEx;
    GroupBox234: TGroupBox;
    Label175: TLabel;
    edtHeroSayPrefix: TEdit;
    chkSlaveAlwaysShowName: TCheckBox;
    chkMasterRoyaltyFullHP: TCheckBox;
    chkSlaveDisableStruck: TCheckBox;
    TabSheet54: TTabSheet;
    GroupBox235: TGroupBox;
    Label173: TLabel;
    seAuctioningItemsCount: TSpinEditEx;
    GroupBox236: TGroupBox;
    Label177: TLabel;
    lbl135: TLabel;
    btnAuction: TButton;
    seAuctionGoldTaxRate: TSpinEditEx;
    GroupBox237: TGroupBox;
    Label626: TLabel;
    Label795: TLabel;
    seAuctionBroadcastPrice: TSpinEditEx;
    cbbAuctionBroadcastCurrencyType: TComboBox;
    GroupBox238: TGroupBox;
    seAuctionItemColor1: TColorIndexEdit;
    Label625: TLabel;
    seAuctionItemColor2: TColorIndexEdit;
    Label797: TLabel;
    seAuctionItemColor3: TColorIndexEdit;
    Label799: TLabel;
    seAuctionItemColor4: TColorIndexEdit;
    Label801: TLabel;
    seAuctionItemColor5: TColorIndexEdit;
    Label803: TLabel;
    seAuctionItemColor6: TColorIndexEdit;
    Label805: TLabel;
    lbl137: TLabel;
    Label806: TLabel;
    seAuctionBroadcastShowTime: TSpinEditEx;
    edtAuctionBroadcastText: TEdit;
    chkOpenAuctionItemColors: TCheckBox;
    GroupBox239: TGroupBox;
    Label807: TLabel;
    Label808: TLabel;
    seSkill25PowerRate: TSpinEditEx;
    chkHeroNoMoveOnSleep: TCheckBox;
    lbl138: TLabel;
    cbbHPRockAddType: TComboBox;
    cbbHPRockType: TComboBox;
    Label115: TLabel;
    cbbMPRockType: TComboBox;
    Label138: TLabel;
    cbbMPRockAddType: TComboBox;
    Label142: TLabel;
    cbbHMPRockType: TComboBox;
    Label144: TLabel;
    cbbHMPRockAddType: TComboBox;
    chkCloseFireHitSkillFailHint: TCheckBox;
    Label148: TLabel;
    Label809: TLabel;
    seSkill41MbTimer1: TSpinEditEx;
    seSkill41MbRange1: TSpinEditEx;
    Label810: TLabel;
    Label811: TLabel;
    seSkill41MbTimer2: TSpinEditEx;
    seSkill41MbRange2: TSpinEditEx;
    Label812: TLabel;
    Label813: TLabel;
    seSkill41MbTimer3: TSpinEditEx;
    seSkill41MbRange3: TSpinEditEx;
    chkElectrodelessTimeSet: TCheckBox;
    Panel2: TPanel;
    seElectrodelessPowerRateL0: TSpinEditEx;
    seElectrodelessPowerRateL1: TSpinEditEx;
    seElectrodelessPowerRateL2: TSpinEditEx;
    seElectrodelessPowerRateL3: TSpinEditEx;
    chkHMPUse2Times: TCheckBox;
    TabSheet55: TTabSheet;
    GroupBox184: TGroupBox;
    Label577: TLabel;
    Label578: TLabel;
    seSkill70CD: TSpinEditEx;
    chkElectrodelessUseMaxSC: TCheckBox;
    chkLuckUseNewAlgorism: TCheckBox;
    Label116: TLabel;
    Label814: TLabel;
    seCopySelfLevelUpAddExistTime: TSpinEditEx;
    Label815: TLabel;
    seSkill72LevelUpRateAdd: TSpinEditEx;
    Label816: TLabel;
    Label817: TLabel;
    seSuperShiledLevelUpAddValidTime: TSpinEditEx;
    Label818: TLabel;
    Label819: TLabel;
    seSuperShiledLevelUpDecPowerRate: TSpinEditEx;
    Label201: TLabel;
    seCloseSuperShiledRate: TSpinEditEx;
    Label821: TLabel;
    seCloseSuperShiledLevelUpDecRate: TSpinEditEx;
    Label202: TLabel;
    seOpenSuperShiledRate: TSpinEditEx;
    Label822: TLabel;
    seOpenSuperShiledLevelUpAddRate: TSpinEditEx;
    Label823: TLabel;
    Label824: TLabel;
    seSkill114LevelUpAddPowerRate: TSpinEditEx;
    Label825: TLabel;
    Label826: TLabel;
    Label820: TLabel;
    GroupBox52: TGroupBox;
    chkDisHeroRun: TCheckBox;
    chkHeroRunMon: TCheckBox;
    chkHeroRunNpc: TCheckBox;
    chkHeroRunGuard: TCheckBox;
    chkHeroSafeArea: TCheckBox;
    chkHeroSafeAreaDisNpcRun: TCheckBox;
    chkSafeAreaDisShopStallHeroRun: TCheckBox;
    chkSafeAreaDisOffLineHeroRun: TCheckBox;
    chkHeroWarDisHumRun: TCheckBox;
    chkHeroWarHreoRun: TCheckBox;
    GroupBox94: TGroupBox;
    chkDisableMoveParalysisHuman: TCheckBox;
    chkHeroNoTargetRecallBB: TCheckBox;
    grp74: TGroupBox;
    Label827: TLabel;
    seHeroAvoidTime: TSpinEditEx;
    chkM2CacheRankData: TCheckBox;
    ts23: TTabSheet;
    GroupBox241: TGroupBox;
    Label829: TLabel;
    seSkill37Range: TSpinEditEx;
    Label828: TLabel;
    seSkill37RangeAdd: TSpinEditEx;
    chkSkill204DisableStopItem: TCheckBox;
    chkHeroDFAvoidTargetRight: TCheckBox;
    GroupBox240: TGroupBox;
    Label830: TLabel;
    Label831: TLabel;
    seSkill113PowerRate: TSpinEditEx;
    GroupBox242: TGroupBox;
    Label832: TLabel;
    Label833: TLabel;
    Label834: TLabel;
    Label835: TLabel;
    seSkill113HitWaitTime: TSpinEditEx;
    seHeroSkill113HitWaitTime: TSpinEditEx;
    ts24: TTabSheet;
    GroupBox244: TGroupBox;
    Label850: TLabel;
    Label851: TLabel;
    Label852: TLabel;
    Label853: TLabel;
    Label844: TLabel;
    Label845: TLabel;
    seSkill117HitWaitTime: TSpinEditEx;
    seHeroSkill117HitWaitTime: TSpinEditEx;
    seSkill117PowerRate: TSpinEditEx;
    grp75: TGroupBox;
    Label836: TLabel;
    Label837: TLabel;
    Label838: TLabel;
    Label839: TLabel;
    Label840: TLabel;
    Label841: TLabel;
    seSkill115PowerRate: TSpinEditEx;
    seSkill115HitWaitTime: TSpinEditEx;
    seHeroSkill115HitWaitTime: TSpinEditEx;
    grp76: TGroupBox;
    Label842: TLabel;
    Label843: TLabel;
    Label846: TLabel;
    Label847: TLabel;
    Label848: TLabel;
    Label849: TLabel;
    seSkill116PowerRate: TSpinEditEx;
    seSkill116HitWaitTime: TSpinEditEx;
    seHeroSkill116HitWaitTime: TSpinEditEx;
    Label854: TLabel;
    seSkill116Range: TSpinEditEx;
    Label855: TLabel;
    seSkill117Range: TSpinEditEx;
    Label858: TLabel;
    seSkill115LevelUpAddPowerRate: TSpinEditEx;
    Label859: TLabel;
    Label856: TLabel;
    seSkill116LevelUpAddPowerRate: TSpinEditEx;
    Label857: TLabel;
    Label860: TLabel;
    seSkill117LevelUpAddPowerRate: TSpinEditEx;
    Label861: TLabel;
    GroupBox243: TGroupBox;
    chkSkill115UseNG: TCheckBox;
    chkSkill115NGNoEnoughDecHP: TCheckBox;
    seSkill115NGNoEnoughDecHPValue: TSpinEditEx;
    cbbSkill115NGNoEnoughDecHPType: TComboBox;
    chkEnableDoubleFireHitDelayClose: TCheckBox;
    cbbDoubleFireHitDelayCloseType: TComboBox;
    lbl139: TLabel;
    seDoubleFireHitDelayCloseValue: TSpinEditEx;
    lbl140: TLabel;
    lbl141: TLabel;
    Label862: TLabel;
    seMaxMabMabeHitMabeTime: TSpinEditEx;
    lbl142: TLabel;
    GroupBox245: TGroupBox;
    Label863: TLabel;
    Label864: TLabel;
    Label865: TLabel;
    seSkillGroupLighteningPowerRate: TSpinEditEx;
    seSkillLighteningPowerRate: TSpinEditEx;
    Label866: TLabel;
    lbl143: TLabel;
    chkAuctionCurrencyType1: TCheckBox;
    chkAuctionCurrencyType2: TCheckBox;
    chkAuctionCurrencyType3: TCheckBox;
    chkAuctionCurrencyType4: TCheckBox;
    chkAuctionCurrencyType5: TCheckBox;
    Label867: TLabel;
    Label868: TLabel;
    seAuctionGameGoldTaxRate: TSpinEditEx;
    Label869: TLabel;
    Label870: TLabel;
    seAuctionGameDiamondTaxRate: TSpinEditEx;
    Label871: TLabel;
    Label872: TLabel;
    seAuctionGameGirdTaxRate: TSpinEditEx;
    Label873: TLabel;
    Label874: TLabel;
    seAuctionGamePointTaxRate: TSpinEditEx;
    grp77: TGroupBox;
    chkCopyMonWarrorAttack: TCheckBox;
    chkCopyMon700HPUseBaseAttack: TCheckBox;
    Label875: TLabel;
    Label876: TLabel;
    seMagicFailMsgFColor: TColorIndexEdit;
    seMagicFailMsgBColor: TColorIndexEdit;
    Label877: TLabel;
    Label878: TLabel;
    chkMagicMsgAddChatBoardMsg: TCheckBox;
    chkMagicMsgXRightToLeft: TCheckBox;
    Label879: TLabel;
    Label880: TLabel;
    seMagicOKMsgFColor: TColorIndexEdit;
    seMagicOKMsgBColor: TColorIndexEdit;
    GroupBox246: TGroupBox;
    Label881: TLabel;
    seWarrCmpInvTime: TSpinEdit;
    chkHeroHitCmp: TCheckBox;
    chkHeroRunHum: TCheckBox;
    chkMagicMsgYBottomToTop: TCheckBox;
    chkHeroNotAvoidLastHinter: TCheckBox;
    chkSlaveNoLockHuman: TCheckBox;
    Label882: TLabel;
    Label883: TLabel;
    seHeroSkill58WaitTime: TSpinEditEx;
    Label884: TLabel;
    seSkill203PoisonRate: TSpinEditEx;
    GroupBox247: TGroupBox;
    Label885: TLabel;
    seSkill113RateAddWithSkill63: TSpinEditEx;
    chkSkill63UseSpeedPoint: TCheckBox;
    chkSkill31UseLEGEffect: TCheckBox;
    chkNoShowNewGuildHumanCount: TCheckBox;
    seHero700HPValue: TSpinEditEx;
    chkDisableSkill41MbSameLevel: TCheckBox;
    chkSkill71DisableAttackSameLevel: TCheckBox;
    chkSkill71DisableAttackFriend: TCheckBox;
    Label886: TLabel;
    seSkill204Distance: TSpinEditEx;
    Label226: TLabel;
    seStarLineMaxCount: TSpinEditEx;
    GroupBox91: TGroupBox;
    Label228: TLabel;
    Label229: TLabel;
    seSkill202PowerRate: TSpinEditEx;
    Label227: TLabel;
    Label230: TLabel;
    seSkillFireCharmPowerRate: TSpinEditEx;
    Label206: TLabel;
    Label207: TLabel;
    seHumSkill66HighPowerRate: TSpinEditEx;
    Label231: TLabel;
    Label232: TLabel;
    seHeroSkill66HighPowerRate: TSpinEditEx;
    chkHeroSkill66HighAttackNoUseRate: TCheckBox;
    Label233: TLabel;
    Label234: TLabel;
    Label235: TLabel;
    Label236: TLabel;
    Label237: TLabel;
    Label238: TLabel;
    seDefenceHum: TSpinEditEx;
    seDefenceMon: TSpinEditEx;
    seDefenceHero: TSpinEditEx;
    chkSkill60NotMagBubbleDefence: TCheckBox;
    Label239: TLabel;
    chkDisableMonsterAttackHero: TCheckBox;
    chkDisableHeroAttackMonster: TCheckBox;
    chkSkill72DisableStopItem: TCheckBox;
    seUseSkillCloseSuperShiled0_Rate: TSpinEditEx;
    seUseSkillCloseSuperShiled1_Rate: TSpinEditEx;
    seUseSkillCloseSuperShiled2_Rate: TSpinEditEx;
    seUseSkillCloseSuperShiled3_Rate: TSpinEditEx;
    seUseSkillCloseSuperShiled4_Rate: TSpinEditEx;
    seUseSkillCloseSuperShiled0_RateAdd: TSpinEditEx;
    seUseSkillCloseSuperShiled1_RateAdd: TSpinEditEx;
    seUseSkillCloseSuperShiled2_RateAdd: TSpinEditEx;
    seUseSkillCloseSuperShiled3_RateAdd: TSpinEditEx;
    seUseSkillCloseSuperShiled4_RateAdd: TSpinEditEx;
    lbl144: TLabel;
    Label757: TLabel;
    Label758: TLabel;
    Label759: TLabel;
    Label760: TLabel;
    Label761: TLabel;
    lbl145: TLabel;
    edtNGLevelPowerAdd_Power: TSpinEditEx;
    Label762: TLabel;
    edtNGLevelPowerDec_Level: TSpinEditEx;
    Label763: TLabel;
    edtNGLevelPowerDec_Power: TSpinEditEx;
    ts25: TTabSheet;
    PageControl9: TPageControl;
    TabSheet72: TTabSheet;
    GroupBox138: TGroupBox;
    Label292: TLabel;
    Label293: TLabel;
    Label764: TLabel;
    Label765: TLabel;
    EditSkillContinuousPowerRate100: TSpinEditEx;
    edtSkillContinuousCloseDefenseRates100: TSpinEditEx;
    GroupBox172: TGroupBox;
    Label536: TLabel;
    Label537: TLabel;
    Label538: TLabel;
    Label539: TLabel;
    Label540: TLabel;
    Label541: TLabel;
    Label542: TLabel;
    Label543: TLabel;
    Label544: TLabel;
    Label545: TLabel;
    EditSkillContinuousBlastHitRate100_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate100_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate100_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate100_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate100_4: TSpinEditEx;
    GroupBox173: TGroupBox;
    Label546: TLabel;
    Label547: TLabel;
    Label548: TLabel;
    Label549: TLabel;
    Label550: TLabel;
    Label551: TLabel;
    Label552: TLabel;
    Label553: TLabel;
    Label554: TLabel;
    Label555: TLabel;
    EditSkillContinuousBlastHitPowerRates100_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates100_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates100_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates100_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates100_4: TSpinEditEx;
    grp15: TGroupBox;
    lbl28: TLabel;
    chkDoMotaebo100PushSameLevel: TCheckBox;
    seDoMotaebo100PushDistance: TSpinEditEx;
    TabSheet73: TTabSheet;
    GroupBox139: TGroupBox;
    Label294: TLabel;
    Label295: TLabel;
    Label766: TLabel;
    Label767: TLabel;
    EditSkillContinuousPowerRate101: TSpinEditEx;
    edtSkillContinuousCloseDefenseRates101: TSpinEditEx;
    GroupBox170: TGroupBox;
    Label516: TLabel;
    Label517: TLabel;
    Label518: TLabel;
    Label519: TLabel;
    Label520: TLabel;
    Label521: TLabel;
    Label522: TLabel;
    Label523: TLabel;
    Label524: TLabel;
    Label525: TLabel;
    EditSkillContinuousBlastHitRate101_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate101_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate101_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate101_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate101_4: TSpinEditEx;
    GroupBox171: TGroupBox;
    Label526: TLabel;
    Label527: TLabel;
    Label528: TLabel;
    Label529: TLabel;
    Label530: TLabel;
    Label531: TLabel;
    Label532: TLabel;
    Label533: TLabel;
    Label534: TLabel;
    Label535: TLabel;
    EditSkillContinuousBlastHitPowerRates101_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates101_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates101_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates101_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates101_4: TSpinEditEx;
    TabSheet74: TTabSheet;
    GroupBox140: TGroupBox;
    Label296: TLabel;
    Label297: TLabel;
    Label768: TLabel;
    Label769: TLabel;
    EditSkillContinuousPowerRate102: TSpinEditEx;
    edtSkillContinuousCloseDefenseRates102: TSpinEditEx;
    GroupBox168: TGroupBox;
    Label496: TLabel;
    Label497: TLabel;
    Label498: TLabel;
    Label499: TLabel;
    Label500: TLabel;
    Label501: TLabel;
    Label502: TLabel;
    Label503: TLabel;
    Label504: TLabel;
    Label505: TLabel;
    EditSkillContinuousBlastHitRate102_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate102_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate102_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate102_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate102_4: TSpinEditEx;
    GroupBox169: TGroupBox;
    Label506: TLabel;
    Label507: TLabel;
    Label508: TLabel;
    Label509: TLabel;
    Label510: TLabel;
    Label511: TLabel;
    Label512: TLabel;
    Label513: TLabel;
    Label514: TLabel;
    Label515: TLabel;
    EditSkillContinuousBlastHitPowerRates102_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates102_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates102_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates102_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates102_4: TSpinEditEx;
    TabSheet75: TTabSheet;
    GroupBox141: TGroupBox;
    Label298: TLabel;
    Label299: TLabel;
    Label770: TLabel;
    Label771: TLabel;
    EditSkillContinuousPowerRate103: TSpinEditEx;
    edtSkillContinuousCloseDefenseRates103: TSpinEditEx;
    GroupBox166: TGroupBox;
    Label476: TLabel;
    Label477: TLabel;
    Label478: TLabel;
    Label479: TLabel;
    Label480: TLabel;
    Label481: TLabel;
    Label482: TLabel;
    Label483: TLabel;
    Label484: TLabel;
    Label485: TLabel;
    EditSkillContinuousBlastHitRate103_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate103_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate103_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate103_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate103_4: TSpinEditEx;
    GroupBox167: TGroupBox;
    Label486: TLabel;
    Label487: TLabel;
    Label488: TLabel;
    Label489: TLabel;
    Label490: TLabel;
    Label491: TLabel;
    Label492: TLabel;
    Label493: TLabel;
    Label494: TLabel;
    Label495: TLabel;
    EditSkillContinuousBlastHitPowerRates103_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates103_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates103_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates103_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates103_4: TSpinEditEx;
    ts26: TTabSheet;
    PageControl10: TPageControl;
    TabSheet76: TTabSheet;
    GroupBox145: TGroupBox;
    Label306: TLabel;
    Label307: TLabel;
    EditSkillContinuousPowerRate104: TSpinEditEx;
    GroupBox164: TGroupBox;
    Label456: TLabel;
    Label457: TLabel;
    Label458: TLabel;
    Label459: TLabel;
    Label460: TLabel;
    Label461: TLabel;
    Label462: TLabel;
    Label463: TLabel;
    Label464: TLabel;
    Label465: TLabel;
    EditSkillContinuousBlastHitRate104_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate104_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate104_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate104_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate104_4: TSpinEditEx;
    GroupBox165: TGroupBox;
    Label466: TLabel;
    Label467: TLabel;
    Label468: TLabel;
    Label469: TLabel;
    Label470: TLabel;
    Label471: TLabel;
    Label472: TLabel;
    Label473: TLabel;
    Label474: TLabel;
    Label475: TLabel;
    EditSkillContinuousBlastHitPowerRates104_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates104_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates104_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates104_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates104_4: TSpinEditEx;
    TabSheet77: TTabSheet;
    GroupBox143: TGroupBox;
    Label302: TLabel;
    Label303: TLabel;
    EditSkillContinuousPowerRate105: TSpinEditEx;
    GroupBox162: TGroupBox;
    Label436: TLabel;
    Label437: TLabel;
    Label438: TLabel;
    Label439: TLabel;
    Label440: TLabel;
    Label441: TLabel;
    Label442: TLabel;
    Label443: TLabel;
    Label444: TLabel;
    Label445: TLabel;
    EditSkillContinuousBlastHitRate105_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate105_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate105_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate105_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate105_4: TSpinEditEx;
    GroupBox163: TGroupBox;
    Label446: TLabel;
    Label447: TLabel;
    Label448: TLabel;
    Label449: TLabel;
    Label450: TLabel;
    Label451: TLabel;
    Label452: TLabel;
    Label453: TLabel;
    Label454: TLabel;
    Label455: TLabel;
    EditSkillContinuousBlastHitPowerRates105_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates105_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates105_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates105_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates105_4: TSpinEditEx;
    TabSheet78: TTabSheet;
    GroupBox144: TGroupBox;
    Label304: TLabel;
    Label305: TLabel;
    EditSkillContinuousPowerRate106: TSpinEditEx;
    GroupBox160: TGroupBox;
    Label416: TLabel;
    Label417: TLabel;
    Label418: TLabel;
    Label419: TLabel;
    Label420: TLabel;
    Label421: TLabel;
    Label422: TLabel;
    Label423: TLabel;
    Label424: TLabel;
    Label425: TLabel;
    EditSkillContinuousBlastHitRate106_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate106_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate106_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate106_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate106_4: TSpinEditEx;
    GroupBox161: TGroupBox;
    Label426: TLabel;
    Label427: TLabel;
    Label428: TLabel;
    Label429: TLabel;
    Label430: TLabel;
    Label431: TLabel;
    Label432: TLabel;
    Label433: TLabel;
    Label434: TLabel;
    Label435: TLabel;
    EditSkillContinuousBlastHitPowerRates106_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates106_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates106_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates106_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates106_4: TSpinEditEx;
    TabSheet79: TTabSheet;
    GroupBox142: TGroupBox;
    Label300: TLabel;
    Label301: TLabel;
    EditSkillContinuousPowerRate107: TSpinEditEx;
    GroupBox158: TGroupBox;
    Label396: TLabel;
    Label397: TLabel;
    Label398: TLabel;
    Label399: TLabel;
    Label400: TLabel;
    Label401: TLabel;
    Label402: TLabel;
    Label403: TLabel;
    Label404: TLabel;
    Label405: TLabel;
    EditSkillContinuousBlastHitRate107_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate107_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate107_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate107_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate107_4: TSpinEditEx;
    GroupBox159: TGroupBox;
    Label406: TLabel;
    Label407: TLabel;
    Label408: TLabel;
    Label409: TLabel;
    Label410: TLabel;
    Label411: TLabel;
    Label412: TLabel;
    Label413: TLabel;
    Label414: TLabel;
    Label415: TLabel;
    EditSkillContinuousBlastHitPowerRates107_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates107_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates107_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates107_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates107_4: TSpinEditEx;
    ts27: TTabSheet;
    PageControl11: TPageControl;
    TabSheet80: TTabSheet;
    GroupBox149: TGroupBox;
    Label314: TLabel;
    Label315: TLabel;
    EditSkillContinuousPowerRate108: TSpinEditEx;
    GroupBox156: TGroupBox;
    Label376: TLabel;
    Label377: TLabel;
    Label378: TLabel;
    Label379: TLabel;
    Label380: TLabel;
    Label381: TLabel;
    Label382: TLabel;
    Label383: TLabel;
    Label384: TLabel;
    Label385: TLabel;
    EditSkillContinuousBlastHitRate108_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate108_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate108_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate108_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate108_4: TSpinEditEx;
    GroupBox157: TGroupBox;
    Label386: TLabel;
    Label387: TLabel;
    Label388: TLabel;
    Label389: TLabel;
    Label390: TLabel;
    Label391: TLabel;
    Label392: TLabel;
    Label393: TLabel;
    Label394: TLabel;
    Label395: TLabel;
    EditSkillContinuousBlastHitPowerRates108_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates108_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates108_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates108_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates108_4: TSpinEditEx;
    TabSheet81: TTabSheet;
    GroupBox148: TGroupBox;
    Label312: TLabel;
    Label313: TLabel;
    EditSkillContinuousPowerRate109: TSpinEditEx;
    GroupBox154: TGroupBox;
    Label356: TLabel;
    Label357: TLabel;
    Label358: TLabel;
    Label359: TLabel;
    Label360: TLabel;
    Label361: TLabel;
    Label362: TLabel;
    Label363: TLabel;
    Label364: TLabel;
    Label365: TLabel;
    EditSkillContinuousBlastHitRate109_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate109_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate109_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate109_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate109_4: TSpinEditEx;
    GroupBox155: TGroupBox;
    Label366: TLabel;
    Label367: TLabel;
    Label368: TLabel;
    Label369: TLabel;
    Label370: TLabel;
    Label371: TLabel;
    Label372: TLabel;
    Label373: TLabel;
    Label374: TLabel;
    Label375: TLabel;
    EditSkillContinuousBlastHitPowerRates109_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates109_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates109_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates109_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates109_4: TSpinEditEx;
    TabSheet82: TTabSheet;
    GroupBox147: TGroupBox;
    Label310: TLabel;
    Label311: TLabel;
    EditSkillContinuousPowerRate110: TSpinEditEx;
    GroupBox152: TGroupBox;
    Label336: TLabel;
    Label337: TLabel;
    Label338: TLabel;
    Label339: TLabel;
    Label340: TLabel;
    Label341: TLabel;
    Label342: TLabel;
    Label343: TLabel;
    Label344: TLabel;
    Label345: TLabel;
    EditSkillContinuousBlastHitRate110_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate110_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate110_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate110_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate110_4: TSpinEditEx;
    GroupBox153: TGroupBox;
    Label346: TLabel;
    Label347: TLabel;
    Label348: TLabel;
    Label349: TLabel;
    Label350: TLabel;
    Label351: TLabel;
    Label352: TLabel;
    Label353: TLabel;
    Label354: TLabel;
    Label355: TLabel;
    EditSkillContinuousBlastHitPowerRates110_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates110_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates110_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates110_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates110_4: TSpinEditEx;
    TabSheet83: TTabSheet;
    GroupBox146: TGroupBox;
    Label308: TLabel;
    Label309: TLabel;
    EditSkillContinuousPowerRate111: TSpinEditEx;
    GroupBox150: TGroupBox;
    Label316: TLabel;
    Label317: TLabel;
    Label318: TLabel;
    Label319: TLabel;
    Label320: TLabel;
    Label321: TLabel;
    Label322: TLabel;
    Label323: TLabel;
    Label324: TLabel;
    Label325: TLabel;
    EditSkillContinuousBlastHitRate111_0: TSpinEditEx;
    EditSkillContinuousBlastHitRate111_1: TSpinEditEx;
    EditSkillContinuousBlastHitRate111_2: TSpinEditEx;
    EditSkillContinuousBlastHitRate111_3: TSpinEditEx;
    EditSkillContinuousBlastHitRate111_4: TSpinEditEx;
    GroupBox151: TGroupBox;
    Label326: TLabel;
    Label327: TLabel;
    Label328: TLabel;
    Label329: TLabel;
    Label330: TLabel;
    Label331: TLabel;
    Label332: TLabel;
    Label333: TLabel;
    Label334: TLabel;
    Label335: TLabel;
    EditSkillContinuousBlastHitPowerRates111_0: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates111_1: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates111_2: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates111_3: TSpinEditEx;
    EditSkillContinuousBlastHitPowerRates111_4: TSpinEditEx;
    grp47: TGroupBox;
    lbl105: TLabel;
    lbl106: TLabel;
    lbl107: TLabel;
    Label739: TLabel;
    Label742: TLabel;
    chkToxicSmoke: TCheckBox;
    cbbSkillContinuousPowerRate111: TComboBox;
    seToxicSmokeTime: TSpinEditEx;
    seToxicSmokeDecHPRate: TSpinEditEx;
    Label772: TLabel;
    Label887: TLabel;
    seSkill100BreakDefenceUpRate: TSpinEditEx;
    GroupBox107: TGroupBox;
    Label888: TLabel;
    Label889: TLabel;
    Label890: TLabel;
    seWarrContinuousStatusLock100: TSpinEditEx;
    seWarrContinuousStatusLockTime100: TSpinEditEx;
    Label891: TLabel;
    GroupBox108: TGroupBox;
    Label892: TLabel;
    Label893: TLabel;
    Label894: TLabel;
    Label895: TLabel;
    seWarrContinuousStatusLock101: TSpinEditEx;
    seWarrContinuousStatusLockTime101: TSpinEditEx;
    GroupBox109: TGroupBox;
    Label896: TLabel;
    Label897: TLabel;
    Label898: TLabel;
    Label899: TLabel;
    seWarrContinuousStatusLock102: TSpinEditEx;
    seWarrContinuousStatusLockTime102: TSpinEditEx;
    GroupBox110: TGroupBox;
    Label900: TLabel;
    Label901: TLabel;
    Label902: TLabel;
    Label903: TLabel;
    seWarrContinuousStatusLock103: TSpinEditEx;
    seWarrContinuousStatusLockTime103: TSpinEditEx;
    grp1: TGroupBox;
    lblContinueBlastRateOrder1: TLabel;
    lblContinueBlastRateOrder2: TLabel;
    lblContinueBlastRateOrder3: TLabel;
    lblContinueBlastRateOrder4: TLabel;
    Label635: TLabel;
    seSkillContinueOrderBlastRates1: TSpinEditEx;
    Label672: TLabel;
    seSkillContinueOrderBlastRates2: TSpinEditEx;
    Label673: TLabel;
    seSkillContinueOrderBlastRates3: TSpinEditEx;
    Label904: TLabel;
    seSkillContinueOrderBlastRates4: TSpinEditEx;
    lbl146: TLabel;
    seSkill102AttackRange: TSpinEditEx;
    Label905: TLabel;
    seSkill103AttackRange: TSpinEditEx;
    Label906: TLabel;
    seSkill105AttackRange: TSpinEditEx;
    Label907: TLabel;
    seNGMaxLevelLimte: TSpinEditEx;
    Label913: TLabel;
    GroupBox111: TGroupBox;
    Label908: TLabel;
    Label910: TLabel;
    seSkill106AddFrozenRate: TSpinEditEx;
    Label916: TLabel;
    Label918: TLabel;
    seSkill106AddFrozenRate2: TSpinEditEx;
    Label909: TLabel;
    seSkill106AddFrozenTime: TSpinEditEx;
    Label915: TLabel;
    Label912: TLabel;
    seSkill106AddFrozenTime2: TSpinEditEx;
    Label911: TLabel;
    GroupBox114: TGroupBox;
    Label925: TLabel;
    Label926: TLabel;
    Label927: TLabel;
    Label928: TLabel;
    Label929: TLabel;
    Label930: TLabel;
    Label931: TLabel;
    Label932: TLabel;
    seSkill110PushedRate: TSpinEditEx;
    seSkill110PushedRate2: TSpinEditEx;
    seSkill110PushedRange: TSpinEditEx;
    seSkill110PushedRange2: TSpinEditEx;
    chkSkill110PushedHighLevel: TCheckBox;
    Label917: TLabel;
    cbbSkill31Level: TComboBox;
    lbl136: TLabel;
    Label914: TLabel;
    GroupBox115: TGroupBox;
    Label920: TLabel;
    edtHumSkill7PowerLV4: TSpinEditEx;
    chkRecallManySlave3: TCheckBox;
    chkSpiritualismDisableUndeadMon: TCheckBox;
    Label919: TLabel;
    seRecallMonCount: TSpinEditEx;
    Label921: TLabel;
    edtHumSkill45PowerLV4: TSpinEditEx;
    Label922: TLabel;
    edtHumSkill13PowerLV4: TSpinEditEx;
    GroupBox116: TGroupBox;
    Label923: TLabel;
    Label924: TLabel;
    seMyShopOperateInterval: TSpinEditEx;
    lbl147: TLabel;
    cbbHeroNeedMagicItem: TComboBox;
    lbl120: TLabel;
    seHeroLogonTimeMasterDie: TSpinEditEx;
    chkHeroStateDlgNoMove: TCheckBox;
    Label624: TLabel;
    seHeroWarrNearFireSword: TSpinEditEx;
    seHMPDivDura: TSpinEditEx;
    Label933: TLabel;
    ts28: TTabSheet;
    grp11: TGroupBox;
    lbl148: TLabel;
    lbl149: TLabel;
    lbl150: TLabel;
    lbl151: TLabel;
    lbl152: TLabel;
    lbl153: TLabel;
    lbl154: TLabel;
    lbl155: TLabel;
    lbl156: TLabel;
    lbl157: TLabel;
    seSellPlayerGoldTaxRate: TSpinEditEx;
    chkSellPlayerCurrencyType1: TCheckBox;
    chkSellPlayerCurrencyType2: TCheckBox;
    chkSellPlayerCurrencyType3: TCheckBox;
    chkSellPlayerCurrencyType4: TCheckBox;
    chkSellPlayerCurrencyType5: TCheckBox;
    seSellPlayerGameGoldTaxRate: TSpinEditEx;
    seSellPlayerGameDiamondTaxRate: TSpinEditEx;
    seSellPlayerGameGirdTaxRate: TSpinEditEx;
    seSellPlayerGamePointTaxRate: TSpinEditEx;
    lbl159: TLabel;
    seSellPlayerTime: TSpinEditEx;
    lbl160: TLabel;
    btnSellPlayerOK: TButton;
    chkSellPlayerViewStorage: TCheckBox;
    chkSellPlayerViewStorageEx: TCheckBox;
    grp14: TGroupBox;
    lbl158: TLabel;
    Label934: TLabel;
    seSellPlayerViewOtherInfoTextOffsetY: TSpinEditEx;
    seSellPlayerViewOtherInfoTextOffsetX: TSpinEditEx;
    chkHeroOnlyPickMonsterItem: TCheckBox;
    grp48: TGroupBox;
    chkDisableWarrContinueHit: TCheckBox;
    lbl161: TLabel;
    edtDisableWarrContinueHitIDs: TEdit;
    lbl162: TLabel;
    seWarrContinueHitMinInterval: TSpinEditEx;
    lbl163: TLabel;
    Label935: TLabel;
    seElectrodelessTimeL4: TSpinEditEx;
    seElectrodelessPowerRateL4: TSpinEditEx;
    Label936: TLabel;
    cbbElectrodelessNewLevel: TComboBox;
    seElectrodelessTimeLNew: TSpinEditEx;
    seElectrodelessPowerRateLNew: TSpinEditEx;
    chkHeroKillMonTrigger: TCheckBox;
    chkCopyMonInheritedMasterSpeed: TCheckBox;
    chkGroupUseOldMode: TCheckBox;
    lbl164: TLabel;
    lbl165: TLabel;
    chkSellPlayerAutoRecallHero: TCheckBox;
    Label937: TLabel;
    seHeroTargetRangeLimit: TSpinEditEx;
    Label938: TLabel;
    Label939: TLabel;
    Label940: TLabel;
    Label941: TLabel;
    seSkill202PowerMin: TSpinEditEx;
    seSkill202PowerDec: TSpinEditEx;
    TabSheet46: TTabSheet;
    GroupBox225: TGroupBox;
    Label942: TLabel;
    Label944: TLabel;
    seHeroProtectFlyRange: TSpinEditEx;
    seHeroLockFlyRange: TSpinEditEx;
    Label943: TLabel;
    seHeroJoinAttackFlyRange: TSpinEditEx;
    chkProhibitModifyPrices: TCheckBox;
    chkBarbaricSeptum: TCheckBox;
    procedure CheckBoxEnablePasswordLockClick(Sender: TObject);
    procedure CheckBoxLockGetBackItemClick(Sender: TObject);
    procedure CheckBoxLockDealItemClick(Sender: TObject);
    procedure CheckBoxLockDropItemClick(Sender: TObject);
    procedure CheckBoxLockWalkClick(Sender: TObject);
    procedure CheckBoxLockRunClick(Sender: TObject);
    procedure CheckBoxLockHitClick(Sender: TObject);
    procedure CheckBoxLockSpellClick(Sender: TObject);
    procedure CheckBoxLockSendMsgClick(Sender: TObject);
    procedure CheckBoxLockInObModeClick(Sender: TObject);
    procedure EditErrorPasswordCountChange(Sender: TObject);
    procedure ButtonPasswordLockSaveClick(Sender: TObject);
    procedure CheckBoxErrorCountKickClick(Sender: TObject);
    procedure CheckBoxLockLoginClick(Sender: TObject);
    procedure CheckBoxLockUseItemClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure CheckBoxHungerSystemClick(Sender: TObject);
    procedure CheckBoxHungerDecHPClick(Sender: TObject);
    procedure CheckBoxHungerDecPowerClick(Sender: TObject);
    procedure ButtonGeneralSaveClick(Sender: TObject);
    procedure CheckBoxLimitSwordLongClick(Sender: TObject);
    procedure ButtonSkillSaveClick(Sender: TObject);
    procedure EditBoneFammNameChange(Sender: TObject);
    procedure EditBoneFammCountChange(Sender: TObject);
    procedure EditSwordLongPowerRateChange(Sender: TObject);
    procedure seFireBoomRageChange(Sender: TObject);
    procedure seSnowWindRangeChange(Sender: TObject);
    procedure seElecBlizzardRangeChange(Sender: TObject);
    procedure EditDogzCountChange(Sender: TObject);
    procedure EditDogzNameChange(Sender: TObject);
    procedure GridBoneFammSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
    procedure EdiAmyOunsulPointChange(Sender: TObject);
    procedure EditMagicAttackRageChange(Sender: TObject);
    procedure ScrollBarUpgradeWeaponDCRateChange(Sender: TObject);
    procedure ScrollBarUpgradeWeaponDCTwoPointRateChange(Sender: TObject);
    procedure ScrollBarUpgradeWeaponDCThreePointRateChange(Sender: TObject);
    procedure ScrollBarUpgradeWeaponSCRateChange(Sender: TObject);
    procedure ScrollBarUpgradeWeaponSCTwoPointRateChange(Sender: TObject);
    procedure ScrollBarUpgradeWeaponSCThreePointRateChange(Sender: TObject);
    procedure ScrollBarUpgradeWeaponMCRateChange(Sender: TObject);
    procedure ScrollBarUpgradeWeaponMCTwoPointRateChange(Sender: TObject);
    procedure ScrollBarUpgradeWeaponMCThreePointRateChange(Sender: TObject);
    procedure EditUpgradeWeaponMaxPointChange(Sender: TObject);
    procedure EditUpgradeWeaponPriceChange(Sender: TObject);
    procedure EditUPgradeWeaponGetBackTimeChange(Sender: TObject);
    procedure EditClearExpireUpgradeWeaponDaysChange(Sender: TObject);
    procedure ButtonUpgradeWeaponSaveClick(Sender: TObject);
    procedure EditMasterOKLevelChange(Sender: TObject);
    procedure ButtonMasterSaveClick(Sender: TObject);
    procedure EditMasterOKCreditPointChange(Sender: TObject);
    procedure EditMasterOKBonusPointChange(Sender: TObject);
    procedure ScrollBarMakeMineHitRateChange(Sender: TObject);
    procedure ScrollBarMakeMineRateChange(Sender: TObject);
    procedure ScrollBarStoneTypeRateChange(Sender: TObject);
    procedure ScrollBarGoldStoneMaxChange(Sender: TObject);
    procedure ScrollBarSilverStoneMaxChange(Sender: TObject);
    procedure ScrollBarSteelStoneMaxChange(Sender: TObject);
    procedure ScrollBarBlackStoneMaxChange(Sender: TObject);
    procedure ButtonMakeMineSaveClick(Sender: TObject);
    procedure EditStoneMinDuraChange(Sender: TObject);
    procedure EditStoneGeneralDuraRateChange(Sender: TObject);
    procedure EditStoneAddDuraRateChange(Sender: TObject);
    procedure EditStoneAddDuraMaxChange(Sender: TObject);
    procedure ButtonWinLotterySaveClick(Sender: TObject);
    procedure EditWinLottery1GoldChange(Sender: TObject);
    procedure EditWinLottery2GoldChange(Sender: TObject);
    procedure EditWinLottery3GoldChange(Sender: TObject);
    procedure EditWinLottery4GoldChange(Sender: TObject);
    procedure EditWinLottery5GoldChange(Sender: TObject);
    procedure EditWinLottery6GoldChange(Sender: TObject);
    procedure ScrollBarWinLottery1MaxChange(Sender: TObject);
    procedure ScrollBarWinLottery2MaxChange(Sender: TObject);
    procedure ScrollBarWinLottery3MaxChange(Sender: TObject);
    procedure ScrollBarWinLottery4MaxChange(Sender: TObject);
    procedure ScrollBarWinLottery5MaxChange(Sender: TObject);
    procedure ScrollBarWinLottery6MaxChange(Sender: TObject);
    procedure ScrollBarWinLotteryRateChange(Sender: TObject);
    procedure ButtonReNewLevelSaveClick(Sender: TObject);
    procedure EditReNewNameColor1Change(Sender: TObject);
    procedure EditReNewNameColor2Change(Sender: TObject);
    procedure EditReNewNameColor3Change(Sender: TObject);
    procedure EditReNewNameColor4Change(Sender: TObject);
    procedure EditReNewNameColor5Change(Sender: TObject);
    procedure EditReNewNameColor6Change(Sender: TObject);
    procedure EditReNewNameColor7Change(Sender: TObject);
    procedure EditReNewNameColor8Change(Sender: TObject);
    procedure EditReNewNameColor9Change(Sender: TObject);
    procedure EditReNewNameColor10Change(Sender: TObject);
    procedure EditReNewNameColorTimeChange(Sender: TObject);
    procedure FunctionConfigControlChanging(Sender: TObject; var AllowChange: Boolean);
    procedure ButtonMonUpgradeSaveClick(Sender: TObject);
    procedure EditMonUpgradeColor1Change(Sender: TObject);
    procedure CheckBoxReNewChangeColorClick(Sender: TObject);
    procedure CheckBoxReNewLevelClearExpClick(Sender: TObject);
    procedure EditPKFlagNameColorChange(Sender: TObject);
    procedure EditPKLevel1NameColorChange(Sender: TObject);
    procedure EditPKLevel2NameColorChange(Sender: TObject);
    procedure EditAllyAndGuildNameColorChange(Sender: TObject);
    procedure EditWarGuildNameColorChange(Sender: TObject);
    procedure EditInFreePKAreaNameColorChange(Sender: TObject);
    procedure EditMonUpgradeKillCount1Change(Sender: TObject);
    procedure EditMonUpgradeKillCount2Change(Sender: TObject);
    procedure EditMonUpgradeKillCount3Change(Sender: TObject);
    procedure EditMonUpgradeKillCount4Change(Sender: TObject);
    procedure EditMonUpgradeKillCount5Change(Sender: TObject);
    procedure EditMonUpgradeKillCount6Change(Sender: TObject);
    procedure EditMonUpgradeKillCount7Change(Sender: TObject);
    procedure EditMonUpLvNeedKillBaseChange(Sender: TObject);
    procedure EditMonUpLvRateChange(Sender: TObject);
    procedure CheckBoxMasterDieMutinyClick(Sender: TObject);
    procedure EditMasterDieMutinyRateChange(Sender: TObject);
    procedure EditMasterDieMutinyPowerChange(Sender: TObject);
    procedure EditMasterDieMutinySpeedChange(Sender: TObject);
    procedure ButtonSpiritMutinySaveClick(Sender: TObject);
    procedure CheckBoxSpiritMutinyClick(Sender: TObject);
    procedure EditSpiritMutinyTimeChange(Sender: TObject);
    procedure EditSpiritPowerRateChange(Sender: TObject);
    procedure seMagTurnUndeadLevelChange(Sender: TObject);
    procedure EditMagTammingLevelChange(Sender: TObject);
    procedure EditMagTammingTargetLevelChange(Sender: TObject);
    procedure EditMagTammingHPRateChange(Sender: TObject);
    procedure ButtonMonSayMsgSaveClick(Sender: TObject);
    procedure CheckBoxMonSayMsgClick(Sender: TObject);
    procedure ButtonUpgradeWeaponDefaulfClick(Sender: TObject);
    procedure ButtonMakeMineDefaultClick(Sender: TObject);
    procedure ButtonWinLotteryDefaultClick(Sender: TObject);
    procedure EditMabMabeHitRandRateChange(Sender: TObject);
    procedure EditMabMabeHitMinLvLimitChange(Sender: TObject);
    procedure EditMabMabeHitSucessRateChange(Sender: TObject);
    procedure EditMabMabeHitMabeTimeRateChange(Sender: TObject);
    procedure ButtonWeaponMakeLuckDefaultClick(Sender: TObject);
    procedure ButtonWeaponMakeLuckSaveClick(Sender: TObject);
    procedure ScrollBarWeaponMakeUnLuckRateChange(Sender: TObject);
    procedure ScrollBarWeaponMakeLuckPoint1Change(Sender: TObject);
    procedure ScrollBarWeaponMakeLuckPoint2Change(Sender: TObject);
    procedure ScrollBarWeaponMakeLuckPoint2RateChange(Sender: TObject);
    procedure ScrollBarWeaponMakeLuckPoint3Change(Sender: TObject);
    procedure ScrollBarWeaponMakeLuckPoint3RateChange(Sender: TObject);
    procedure EditTammingCountChange(Sender: TObject);
    procedure CheckBoxFireCrossInSafeZoneClick(Sender: TObject);
    procedure CheckBoxBBMonAutoChangeColorClick(Sender: TObject);
    procedure EditBBMonAutoChangeColorTimeChange(Sender: TObject);
    procedure chkSkill41MbAttackPlayObjectClick(Sender: TObject);
    procedure chkSkill71PullPlayObjectClick(Sender: TObject);
    procedure chkPlayObjectReduceMPClick(Sender: TObject);
    procedure SpinEditMagDelayTimeChange(Sender: TObject);
    procedure chkSkill41MbAttackSlaveClick(Sender: TObject);
    procedure EditItemNameChange(Sender: TObject);
    procedure seDedingMagicCDChange(Sender: TObject);
    procedure chkDedingAllowPKClick(Sender: TObject);
    procedure chkSkill71PullCrossInSafeZoneClick(Sender: TObject);
    procedure EditMerchantNameColorChange(Sender: TObject);
    procedure seHPRockRateChange(Sender: TObject);
    procedure seHPRockTimeChange(Sender: TObject);
    procedure seHPRockAddValueChange(Sender: TObject);
    procedure seHPRockDecValueChange(Sender: TObject);
    procedure seMPRockRateChange(Sender: TObject);
    procedure seMPRockTimeChange(Sender: TObject);
    procedure seMPRockAddValueChange(Sender: TObject);
    procedure seMPRockDecValueChange(Sender: TObject);
    procedure seHMPRockRateChange(Sender: TObject);
    procedure seHMPRockTimeChange(Sender: TObject);
    procedure seHMPRockAddValueChange(Sender: TObject);
    procedure seHMPRockDecValueChange(Sender: TObject);
    procedure CheckBoxHeroPickUpItemClick(Sender: TObject);
    procedure CheckBoxHeroShowMasterNameClick(Sender: TObject);
    procedure seHeroKillMonExpRateChange(Sender: TObject);
    procedure EditHeroRecallTimeChange(Sender: TObject);
    procedure EditHeroWarrorAttackTimeChange(Sender: TObject);
    procedure EditHeroWizardAttackTimeChange(Sender: TObject);
    procedure EditHeroTaoistAttackTimeChange(Sender: TObject);
    procedure EditHeroWarrorWalkTimeChange(Sender: TObject);
    procedure EditHeroWizardWalkTimeChange(Sender: TObject);
    procedure EditHeroTaoistWalkTimeChange(Sender: TObject);
    procedure EditHeroSuffixNameChange(Sender: TObject);
    procedure EditHeroNameColorChange(Sender: TObject);
    procedure ButtonHeroOptionSaveClick(Sender: TObject);
    procedure seSkill60PowerRateChange(Sender: TObject);
    procedure seSkill61PowerRateChange(Sender: TObject);
    procedure seSkill62PowerRateChange(Sender: TObject);
    procedure seSkill63PowerRateChange(Sender: TObject);
    procedure seSkill64PowerRateChange(Sender: TObject);
    procedure seSkill65PowerRateChange(Sender: TObject);
    procedure EditMaxAngryValueChange(Sender: TObject);
    procedure EditAddAngryValueChange(Sender: TObject);
    procedure EditDecFirDragonPointChange(Sender: TObject);
    procedure EditAddAngryValueTimeChange(Sender: TObject);
    procedure ComboBoxBagItemCountChange(Sender: TObject);
    procedure EditNeedLevelChange(Sender: TObject);
    procedure chkWarrorAttackClick(Sender: TObject);
    procedure EditNeedGuardLevelChange(Sender: TObject);
    procedure EditGuardRangeChange(Sender: TObject);
    procedure chkHeroGetAllExpClick(Sender: TObject);
    procedure EditSkill56PowerRateChange(Sender: TObject);
    procedure EditSkill58PowerRateChange(Sender: TObject);
    procedure seFireHitWaitTimeChange(Sender: TObject);
    procedure seSkill42PowerRateChange(Sender: TObject);
    procedure seSkill40PowerRateChange(Sender: TObject);
    procedure EditSkill43PowerRateChange(Sender: TObject);
    procedure seHeroSkill66PowerRateChange(Sender: TObject);
    procedure seCopySelfMaxCountChange(Sender: TObject);
    procedure seCopySelfExistTimeChange(Sender: TObject);
    procedure chkCopySelfNonUseSpellPointClick(Sender: TObject);
    procedure EditMagicItemRateChange(Sender: TObject);
    procedure ButtonOffLineSaveClick(Sender: TObject);
    procedure CheckBoxOffLineLoginSafeAreaClick(Sender: TObject);
    procedure RadioButtonOffLineLoginMapName1Click(Sender: TObject);
    procedure RadioButtonOffLineLoginMapName2Click(Sender: TObject);
    procedure RadioGroupHumNeedMagicItemClick(Sender: TObject);
    procedure seMagicNewLevelPowerChange(Sender: TObject);
    procedure cbbMagicNewLevelChange(Sender: TObject);
    procedure EditSWordHitWaitTimeChange(Sender: TObject);
    procedure seSkill42HitWaitTimeChange(Sender: TObject);
    procedure EditSkill66HitWaitTimeChange(Sender: TObject);
    procedure seSkill72HitWaitTimeChange(Sender: TObject);
    procedure seSkill71CDChange(Sender: TObject);
    procedure seHeroSkill66HighAttackRateChange(Sender: TObject);
    procedure seHumSkill66HighPowerRateChange(Sender: TObject);
    procedure EditSkill57AddHPRateChange(Sender: TObject);
    procedure EditSkill58AttackRangeChange(Sender: TObject);
    procedure EditRecallBigDogWaitTimeChange(Sender: TObject);
    procedure EditMonthSpiritHighPowerRateChange(Sender: TObject);
    procedure EditMonthSpiritHighAttackRateChange(Sender: TObject);
    procedure CheckBoxMonthSpiritUseMasterMPClick(Sender: TObject);
    procedure CheckBoxMonthSpiritAttackSameClick(Sender: TObject);
    procedure seMonthSpiritCountChange(Sender: TObject);
    procedure EditBigDogzCountChange(Sender: TObject);
    procedure seSuperShiledValidTimeChange(Sender: TObject);
    procedure seLastSuperShiledTimeChange(Sender: TObject);
    procedure seSuperShiledPowerRateChange(Sender: TObject);
    procedure seCloseSuperShiledRateChange(Sender: TObject);
    procedure seOpenSuperShiledRateChange(Sender: TObject);
    procedure chkUseSkillCloseSuperShiled0Click(Sender: TObject);
    procedure CheckBoxAutoOpenSuperShiledClick(Sender: TObject);
    procedure chkShowSuperShiledEffectClick(Sender: TObject);
    procedure chkRecallManySlave1Click(Sender: TObject);
    procedure CheckBoxOpenMapEventClick(Sender: TObject);
    procedure ButtonMyShopSaveClick(Sender: TObject);
    procedure EditMaxMyShopSellingItemCountChange(Sender: TObject);
    procedure EditMaxMyShopStorageItemCountChange(Sender: TObject);
    procedure EditSlavePowerRateChange(Sender: TObject);
    procedure EditMagicLockRangeChange(Sender: TObject);
    procedure CheckBoxKillByMonstDropHeroUseItemClick(Sender: TObject);
    procedure CheckBoxKillByHumanDropHeroUseItemClick(Sender: TObject);
    procedure CheckBoxDieScatterHeroBagClick(Sender: TObject);
    procedure CheckBoxDieRedScatterHeroBagAllClick(Sender: TObject);
    procedure ScrollBarDieDropHeroUseItemRateChange(Sender: TObject);
    procedure ScrollBarDieRedDropHeroUseItemRateChange(Sender: TObject);
    procedure ScrollBarDieScatterHeroBagRateChange(Sender: TObject);
    procedure CheckBoxDisableChangeMapFireCrossClick(Sender: TObject);
    procedure EditFireCrossMaxTimeChange(Sender: TObject);
    procedure EditFireCrossPowerRateChange(Sender: TObject);
    procedure ButtonOtherClick(Sender: TObject);
    procedure CheckBoxDeleteItemDuraZeroClick(Sender: TObject);
    procedure EditMasterRoyaltyTimeChange(Sender: TObject);
    procedure seFireHitPowerRateChange(Sender: TObject);
    procedure chkRecallManySlave2Click(Sender: TObject);
    procedure EditSkill52PowerRateChange(Sender: TObject);
    procedure EditSkill52AttackRangeChange(Sender: TObject);
    procedure EditMysteriousManNameChange(Sender: TObject);
    procedure EditRecallDeputyHeroTimeChange(Sender: TObject);
    procedure ButtonSaveWineClick(Sender: TObject);
    procedure GridMedicineExpEnter(Sender: TObject);
    procedure EditDecMedicineTimeChange(Sender: TObject);
    procedure EditDecMedicineValueChange(Sender: TObject);
    procedure EditIncAlcoholTimeChange(Sender: TObject);
    procedure EditDecDrinkTimeChange(Sender: TObject);
    procedure EditMaxAlcoholValueChange(Sender: TObject);
    procedure EditIncAlcoholValueChange(Sender: TObject);
    procedure EditUseContinuousMagicTimeChange(Sender: TObject);
    procedure EditNGLevelValueChange(Sender: TObject);
    procedure EditNGLevelExpValueChange(Sender: TObject);
    procedure EditNGHeroLevelExpValueChange(Sender: TObject);
    procedure EditNGIncTimeChange(Sender: TObject);
    procedure EditNGSkillPowerRateChange(Sender: TObject);
    procedure EditNGDrinkIncExpChange(Sender: TObject);
    procedure EditNGHitStruckDecNGChange(Sender: TObject);
    procedure EditNGKillMonExpMultipleChange(Sender: TObject);
    procedure edtNGLevelPowerAdd_LevelChange(Sender: TObject);
    procedure EditSkillContinuousPowerRate100Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate100_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate101_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate102_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate103_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate104_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate105_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate106_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate107_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate108_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate109_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate110_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitRate111_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates100_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates101_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates102_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates103_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates104_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates105_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates106_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates107_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates108_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates109_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates110_0Change(Sender: TObject);
    procedure EditSkillContinuousBlastHitPowerRates111_0Change(Sender: TObject);
    procedure EditAcupoints0_0Change(Sender: TObject);
    procedure EditAcupoints1_0Change(Sender: TObject);
    procedure EditAcupoints2_0Change(Sender: TObject);
    procedure EditAcupoints3_0Change(Sender: TObject);
    procedure EditAcupoints4_0Change(Sender: TObject);
    procedure seSkill114HitWaitTimeChange(Sender: TObject);
    procedure seSkill114PowerRateChange(Sender: TObject);
    procedure seDoMotaeboCDChange(Sender: TObject);
    procedure EditDamageItemDuraRateChange(Sender: TObject);
    procedure CheckBoxViewRangeCanMagicAttackClick(Sender: TObject);
    procedure chkDogzGotoMasterClick(Sender: TObject);
    procedure CheckBoxMonthSpiritGotoMasterClick(Sender: TObject);
    procedure CheckBoxBigDogzGotoMasterClick(Sender: TObject);
    procedure CheckBoxOfflineCloseMyShopClick(Sender: TObject);
    procedure chkEnableDoubleFireHitSkillClick(Sender: TObject);
    procedure CheckBoxMasterRoyaltyDieClick(Sender: TObject);
    procedure EditAmyOunsulTimeRateChange(Sender: TObject);
    procedure EditAmyOunsulMaxTimeChange(Sender: TObject);
    procedure EditElectrodelessBaseChange(Sender: TObject);
    procedure EditElectrodelessWaitTimeChange(Sender: TObject);
    procedure seElectrodelessPowerRateL0Chnge(Sender: TObject);
    procedure EditSkill57PowerRateChange(Sender: TObject);
    procedure CheckBoxQigongPushSameLevelClick(Sender: TObject);
    procedure CheckBoxFireWindPushSameLevelClick(Sender: TObject);
    procedure EditMaxLuckMaxPowerChange(Sender: TObject);
    procedure RadioGroupShopTypeClick(Sender: TObject);
    procedure CheckBoxSlaveRelaxCanStruckClick(Sender: TObject);
    procedure seSkill41CDChange(Sender: TObject);
    procedure CheckBoxWeaponUpgradeFailNotDeleteClick(Sender: TObject);
    procedure seSkill58WaitTimeChange(Sender: TObject);
    procedure CheckBoxDropOverLapItemClick(Sender: TObject);
    procedure chkShowDoMotaeboMsgClick(Sender: TObject);
    procedure CheckBoxDuraChangeLightClick(Sender: TObject);
    procedure CheckBoxShowYouPoisonedClick(Sender: TObject);
    procedure CheckBoxPoisonWeaponCanMagicAttackClick(Sender: TObject);
    procedure CheckBoxPoisonWeaponCanHitAllTargetClick(Sender: TObject);
    procedure EditQueryBagItemsTimeChange(Sender: TObject);
    procedure chkInfinityStorageClick(Sender: TObject);
    procedure edtInfinityStorageCountChange(Sender: TObject);
    procedure seSkill202BaseCountChange(Sender: TObject);
    procedure seSkill202LevelupCountChange(Sender: TObject);
    procedure seSkill203BasicMbTimerChange(Sender: TObject);
    procedure seSkill203LevelupMbTimerChange(Sender: TObject);
    procedure chkSkill203MbAttackMonClick(Sender: TObject);
    procedure chkSkill203MbAttackHumanClick(Sender: TObject);
    procedure chkSkill203MbAttackSlaveClick(Sender: TObject);
    procedure chkSkill203DamagearmorClick(Sender: TObject);
    procedure chkSkill203DecHealthClick(Sender: TObject);
    procedure chkSkill203MbFastParalysisClick(Sender: TObject);
    procedure chkSkill204MbAttackMonClick(Sender: TObject);
    procedure chkSkill204MbAttackHumanClick(Sender: TObject);
    procedure chkSkill204MbAttackSlaveClick(Sender: TObject);
    procedure chkSkill204MbFastParalysisClick(Sender: TObject);
    procedure chkSkill204RunHumClick(Sender: TObject);
    procedure chkSkill204RunMonClick(Sender: TObject);
    procedure chkSkill204RunNpcClick(Sender: TObject);
    procedure chkSkill204RunGuardClick(Sender: TObject);
    procedure chkSkill204WarDisHumRunClick(Sender: TObject);
    procedure seSkill204BasicPowerRateChange(Sender: TObject);
    procedure seSkill204LevelupPowerRateChange(Sender: TObject);
    procedure seSkill204LevelupMbTimerChange(Sender: TObject);
    procedure chkSkill205ReduceMPClick(Sender: TObject);
    procedure seSkill204BasicMbTimerChange(Sender: TObject);
    procedure seSkill203BasicPowerRateChange(Sender: TObject);
    procedure seSkill203LevelupPowerRateChange(Sender: TObject);
    procedure chkSkill206MbAttackMonClick(Sender: TObject);
    procedure chkSkill206MbAttackHumanClick(Sender: TObject);
    procedure chkSkill206MbAttackSlaveClick(Sender: TObject);
    procedure chkSkill206MbFastParalysisClick(Sender: TObject);
    procedure seSkill206BasicPowerRateChange(Sender: TObject);
    procedure seSkill206LevelupPowerRateChange(Sender: TObject);
    procedure seSkill206BasicMbTimerChange(Sender: TObject);
    procedure seSkill206LevelupMbTimerChange(Sender: TObject);
    procedure seSkill202CDChange(Sender: TObject);
    procedure seSkill205CDChange(Sender: TObject);
    procedure seSkill206CDChange(Sender: TObject);
    procedure seSkill203CDChange(Sender: TObject);
    procedure seSkill204CDChange(Sender: TObject);
    procedure seSkill114AttackRangeChange(Sender: TObject);
    procedure seSkill41MbTimer0Change(Sender: TObject);
    procedure seMonthSpiritAttackRangeChange(Sender: TObject);
    procedure seDeDingMagicBasicPowerRateChange(Sender: TObject);
    procedure seDeDingMagicAttackRangeChange(Sender: TObject);
    procedure chkSkill71PullSlaveClick(Sender: TObject);
    procedure sePosionDecHealthTimeChange(Sender: TObject);
    procedure sePosionDamagarmorChange(Sender: TObject);
    procedure chkOpenSelfShopClick(Sender: TObject);
    procedure chkSafeZoneShopClick(Sender: TObject);
    procedure chkMapShopClick(Sender: TObject);
    procedure seSellOffGoldTaxRateChange(Sender: TObject);
    procedure seSellOffGameGoldTaxRateChange(Sender: TObject);
    procedure chkLockChallengeClick(Sender: TObject);
    procedure chkLockSummonHeroClick(Sender: TObject);
    procedure chkLockShopClick(Sender: TObject);
    procedure chkLockStallClick(Sender: TObject);
    procedure seSkill41MbRange0Change(Sender: TObject);
    procedure seSkill205RageChange(Sender: TObject);
    procedure seSkill206RageChange(Sender: TObject);
    procedure seSkill203RageChange(Sender: TObject);
    procedure seSkill204RageChange(Sender: TObject);
    procedure chkGroupReCallNotInSafeZoneClick(Sender: TObject);
    procedure chkNewHumanAttatckMode_HAM_PEACEClick(Sender: TObject);
    procedure chkAutoGroupMasterClick(Sender: TObject);
    procedure chkGuardNotAttackPlayMosterClick(Sender: TObject);
    procedure seHongMoSuiteRateChange(Sender: TObject);
    procedure chkCloseFireHitSkillFailHintClick(Sender: TObject);
    procedure seElecBlizzardPowerRateChange(Sender: TObject);
    procedure seFireBoomRagePowerRateChange(Sender: TObject);
    procedure seMakeFireDayPowerRateChange(Sender: TObject);
    procedure seSnowWindPowerRateChange(Sender: TObject);
    procedure chkDoMotaeboPushSameLevelClick(Sender: TObject);
    procedure seSkill15PowerRateChange(Sender: TObject);
    procedure chkWarNoDropUseItemClick(Sender: TObject);
    procedure seSkill15TimeRateChange(Sender: TObject);
    procedure seSkill205PowerRateChange(Sender: TObject);
    procedure seBonusAbilofWarrDCChange(Sender: TObject);
    procedure seBonusAbilofWarrMCChange(Sender: TObject);
    procedure seBonusAbilofWarrSCChange(Sender: TObject);
    procedure seBonusAbilofWarrACChange(Sender: TObject);
    procedure seBonusAbilofWarrMACChange(Sender: TObject);
    procedure seBonusAbilofWarrHPChange(Sender: TObject);
    procedure seBonusAbilofWarrMPChange(Sender: TObject);
    procedure seBonusAbilofWarrHitChange(Sender: TObject);
    procedure seBonusAbilofWarrSpeedChange(Sender: TObject);
    procedure seBonusAbilofWizardDCChange(Sender: TObject);
    procedure seBonusAbilofWizardMCChange(Sender: TObject);
    procedure seBonusAbilofWizardSCChange(Sender: TObject);
    procedure seBonusAbilofWizardACChange(Sender: TObject);
    procedure seBonusAbilofWizardMACChange(Sender: TObject);
    procedure seBonusAbilofWizardHPChange(Sender: TObject);
    procedure seBonusAbilofWizardMPChange(Sender: TObject);
    procedure seBonusAbilofWizardHitChange(Sender: TObject);
    procedure seBonusAbilofWizardSpeedChange(Sender: TObject);
    procedure seBonusAbilofTaosDCChange(Sender: TObject);
    procedure seBonusAbilofTaosMCChange(Sender: TObject);
    procedure seBonusAbilofTaosSCChange(Sender: TObject);
    procedure seBonusAbilofTaosACChange(Sender: TObject);
    procedure seBonusAbilofTaosMACChange(Sender: TObject);
    procedure seBonusAbilofTaosHPChange(Sender: TObject);
    procedure seBonusAbilofTaosMPChange(Sender: TObject);
    procedure seBonusAbilofTaosHitChange(Sender: TObject);
    procedure seBonusAbilofTaosSpeedChange(Sender: TObject);
    procedure btnBonusAbilofSaveClick(Sender: TObject);
    procedure chkCloseSuperShiledHintClick(Sender: TObject);
    procedure seElectrodelessTimeL0Change(Sender: TObject);
    procedure cbbMagicNewLevel0Change(Sender: TObject);
    procedure seMagicNewLevelPower0Change(Sender: TObject);
    procedure cbbMagicNewLevel1Change(Sender: TObject);
    procedure seMagicNewLevelPower1Change(Sender: TObject);
    procedure cbbMagicNewLevel2Change(Sender: TObject);
    procedure seMagicNewLevelPower2Change(Sender: TObject);
    procedure cbbMagicNewLevel3Change(Sender: TObject);
    procedure seMagicNewLevelPower3Change(Sender: TObject);
    procedure cbbMagicNewLevel4Change(Sender: TObject);
    procedure seMagicNewLevelPower4Change(Sender: TObject);
    procedure cbbMagicNewLevel5Change(Sender: TObject);
    procedure seMagicNewLevelPower5Change(Sender: TObject);
    procedure cbbMagicNewLevel20Change(Sender: TObject);
    procedure seMagicNewLevelPower20Change(Sender: TObject);
    procedure cbbMagicNewLevel21Change(Sender: TObject);
    procedure seMagicNewLevelPower21Change(Sender: TObject);
    procedure cbbMagicNewLevel22Change(Sender: TObject);
    procedure seMagicNewLevelPower22Change(Sender: TObject);
    procedure cbbMagicNewLevel23Change(Sender: TObject);
    procedure seMagicNewLevelPower23Change(Sender: TObject);
    procedure cbbMagicNewLevel24Change(Sender: TObject);
    procedure seMagicNewLevelPower24Change(Sender: TObject);
    procedure cbbMagicNewLevel40Change(Sender: TObject);
    procedure seMagicNewLevelPower40Change(Sender: TObject);
    procedure cbbMagicNewLevel41Change(Sender: TObject);
    procedure seMagicNewLevelPower41Change(Sender: TObject);
    procedure cbbMagicNewLevel42Change(Sender: TObject);
    procedure seMagicNewLevelPower42Change(Sender: TObject);
    procedure cbbMagicNewLevel43Change(Sender: TObject);
    procedure seMagicNewLevelPower43Change(Sender: TObject);
    procedure cbbMagicNewLevel44Change(Sender: TObject);
    procedure seMagicNewLevelPower44Change(Sender: TObject);
    procedure cbbMagicNewLevel45Change(Sender: TObject);
    procedure seMagicNewLevelPower45Change(Sender: TObject);
    procedure seClearHeroGhostTickChange(Sender: TObject);
    procedure chkHeroCallBBClick(Sender: TObject);
    procedure seHeroCallBBCountChange(Sender: TObject);
    procedure chkHeroCanUseMooteboClick(Sender: TObject);
    procedure chkHeroJointAttackClick(Sender: TObject);
    procedure seSkill60PowerRangeChange(Sender: TObject);
    procedure seSkill65PowerRangeChange(Sender: TObject);
    procedure seSkill64PowerRangeChange(Sender: TObject);
    procedure seSkill63PowerRangeChange(Sender: TObject);
    procedure chkSkill63GreenPoisonClick(Sender: TObject);
    procedure seHeroDieExpRateChange(Sender: TObject);
    procedure seHeroMasterStartLevelChange(Sender: TObject);
    procedure seHeroSlaveStartLevelChange(Sender: TObject);
    procedure chkHeroDisableSafeZoneProtectClick(Sender: TObject);
    procedure chkHeroNoMoveOnSleepClick(Sender: TObject);
    procedure seHeroWarrHPMPRateChange(Sender: TObject);
    procedure seHeroWizardHPMPRateChange(Sender: TObject);
    procedure seHeroTaosHPMPRateChange(Sender: TObject);
    procedure chkHero700HPUseBaseAttackClick(Sender: TObject);
    procedure chkCreditPointWithLevelClick(Sender: TObject);
    procedure seHeroNotKillMonExpRateChange(Sender: TObject);
    procedure seHeroFealtyCallBackDelChange(Sender: TObject);
    procedure seHeroFealtyDeathDelChange(Sender: TObject);
    procedure chkHeroCalcWeaponSpeedClick(Sender: TObject);
    procedure seHeroFealtyExpChange(Sender: TObject);
    procedure seHeroFealtyCallAddChange(Sender: TObject);
    procedure seHeroFealtyExpAddChange(Sender: TObject);
    procedure chkHeroTaosAutoChangePoisonClick(Sender: TObject);
    procedure GridLevelExpSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
    procedure seHeroGotoLV4Change(Sender: TObject);
    procedure seHeroPowerLV4Change(Sender: TObject);
    procedure ComboBoxLevelExpClick(Sender: TObject);
    procedure chkShowMsgMagicRangeExceedClick(Sender: TObject);
    procedure chkShowMysteriousManClick(Sender: TObject);
    procedure chkShopStallCanNotAttackClick(Sender: TObject);
    procedure chkHeroFollowMasterWithDiffScreenClick(Sender: TObject);
    procedure seHeroHighLevelChange(Sender: TObject);
    procedure seHeroHighLevelGetExpChange(Sender: TObject);
    procedure seHeroLevel1000FixedExpChange(Sender: TObject);
    procedure chkHeroDisableStruckClick(Sender: TObject);
    procedure chkHeroDisableSelfStruckClick(Sender: TObject);
    procedure seNewLevelMagicPowerRatesAfter9Change(Sender: TObject);
    procedure seOrdinarySkill31RateChange(Sender: TObject);
    procedure seSkill31RateChange(Sender: TObject);
    procedure chkElectrodelessTimeSetClick(Sender: TObject);
    procedure edtCopySelfSuffixChange(Sender: TObject);
    procedure chkShowCopySelfSuffixClick(Sender: TObject);
    procedure chkAlwaysFollowMasterAttackClick(Sender: TObject);
    procedure chkKillHeroWeaponUnlockClick(Sender: TObject);
    procedure seKillHeroWeaponUnlockRateChange(Sender: TObject);
    procedure chkHumanGetAllExpClick(Sender: TObject);
    procedure seSkill42RangeChange(Sender: TObject);
    procedure seSkill72RateChange(Sender: TObject);
    procedure chkSkill114AttackUseNGClick(Sender: TObject);
    procedure chkContinuousAttackUseNGClick(Sender: TObject);
    procedure seRevivalTimeChange(Sender: TObject);
    procedure chkRevivalTouchClick(Sender: TObject);
    procedure btnOther3Click(Sender: TObject);
    procedure seLimitScriptGotoCountChange(Sender: TObject);
    procedure chkFBExitCreaterOfflineClick(Sender: TObject);
    procedure chkFBDisableDelay30sClick(Sender: TObject);
    procedure chkShowSuperShiledSoundClick(Sender: TObject);
    procedure chkShowSuperShiledEffect2Click(Sender: TObject);
    procedure chkShowSuperShiledSound2Click(Sender: TObject);
    procedure chkHeroAutoSuperShiledClick(Sender: TObject);
    procedure chkMagicNotHinderClick(Sender: TObject);
    procedure chkMagicDefinitionClick(Sender: TObject);
    procedure seHeroAttackRangeChange(Sender: TObject);
    procedure chkShopHeadPicClick(Sender: TObject);
    procedure seACAttackSpeedChange(Sender: TObject);
    procedure seSKILL209CDChange(Sender: TObject);
    procedure seSkill209RageChange(Sender: TObject);
    procedure seSkill209PowerRateChange(Sender: TObject);
    procedure seSKILL210CDChange(Sender: TObject);
    procedure seSkill210RageChange(Sender: TObject);
    procedure seSkill210PowerRateChange(Sender: TObject);
    procedure seSKILL208CDChange(Sender: TObject);
    procedure seSkill208RageChange(Sender: TObject);
    procedure seSkill208PowerRateChange(Sender: TObject);
    procedure chkSkill204SameLevelClick(Sender: TObject);
    procedure chkSkill206SameLevelClick(Sender: TObject);
    procedure chkSkill206FrozenClick(Sender: TObject);
    procedure chkJewelryCalcBasicAbilitysClick(Sender: TObject);
    procedure seAngryAgainValueChange(Sender: TObject);
    procedure chkSKILL208HeroDuanJinClick(Sender: TObject);
    procedure chkSKILL208PlayMosterDuanJinClick(Sender: TObject);
    procedure seRecallCopySelfWaitTimeChange(Sender: TObject);
    procedure seRecallDogzWaitTimeChange(Sender: TObject);
    procedure seRecallBoneFammWaitTimeChange(Sender: TObject);
    procedure seRecallMonthSpiritWaitTimeChange(Sender: TObject);
    procedure seHeroTaoUsePoisonMinHPChange(Sender: TObject);
    procedure cbbSkillContinuousPowerRate111Change(Sender: TObject);
    procedure seToxicSmokeTimeChange(Sender: TObject);
    procedure chkToxicSmokeClick(Sender: TObject);
    procedure seContinuousProtectChange(Sender: TObject);
    procedure seContinuousProtectRandomChange(Sender: TObject);
    procedure seWarrContinuousStatusLockChange(Sender: TObject);
    procedure chkSlaveNotAttackHumanClick(Sender: TObject);
    procedure chkSlaveNotAttackHeroClick(Sender: TObject);
    procedure cbbMagicNewLevel25Change(Sender: TObject);
    procedure seMagicNewLevelPower25Change(Sender: TObject);
    procedure seHeroSkill114HitWaitTimeChange(Sender: TObject);
    procedure seHeroSkill66HitWaitTimeChange(Sender: TObject);
    procedure seSnowwindWaitTimeChange(Sender: TObject);
    procedure seJSOfGameGoldTaxRateChange(Sender: TObject);
    procedure chkJewelryCalcGroupAbilitysClick(Sender: TObject);
    procedure chkSaveRevivalTimeClick(Sender: TObject);
    procedure seNewLevelMagic43LSFRateChange(Sender: TObject);
    procedure seMakeFireDayTimeChange(Sender: TObject);
    procedure seSkill57TimeChange(Sender: TObject);
    procedure chkSkill204RunObstacleClick(Sender: TObject);
    procedure chkDoMotaebo100PushSameLevelClick(Sender: TObject);
    procedure seMasterCountChange(Sender: TObject);
    procedure seBBAttrPlusAddAttackRateChange(Sender: TObject);
    procedure chkBBAttrPlusAddDefenceClick(Sender: TObject);
    procedure chkBBAttrPlusAddMagicDefenceClick(Sender: TObject);
    procedure seBBAttrPlusAddDefenceRateChange(Sender: TObject);
    procedure chkBBAttrPlusAddHPClick(Sender: TObject);
    procedure seBBAttrPlusAddHPRateChange(Sender: TObject);
    procedure cbbBBAttrPlusNewLevelChange(Sender: TObject);
    procedure seBBAttrPlusNewLevelRateChange(Sender: TObject);
    procedure cbbBBAttrPlusAddAttackFormChange(Sender: TObject);
    procedure chkBBAttrPlusAddAttackClick(Sender: TObject);
    procedure chkBBAttrPlusAddOnlyMagicClick(Sender: TObject);
    procedure chkShowRefreshBagMsgClick(Sender: TObject);
    procedure chkKillByMonstDropHeroJewelryBoxItemClick(Sender: TObject);
    procedure chkKillByHumanDropHeroJewelryBoxItemClick(Sender: TObject);
    procedure chkKillByMonstDropHeroGodBlessItemClick(Sender: TObject);
    procedure chkKillByHumanDropHeroGodBlessItemClick(Sender: TObject);
    procedure scrlbrHeroJewelryBoxItemChange(Sender: TObject);
    procedure scrlbrHeroGodBlessItemChange(Sender: TObject);
    procedure seSellOffGameDiamondTaxRateChange(Sender: TObject);
    procedure seSellOffGameGirdTaxRateChange(Sender: TObject);
    procedure chkMyShopGoldClick(Sender: TObject);
    procedure chkMyShopGameGoldClick(Sender: TObject);
    procedure chkMyShopGameDiamondClick(Sender: TObject);
    procedure chkMyShopGameGirdClick(Sender: TObject);
    procedure chkMyShopGamePointClick(Sender: TObject);
    procedure seSellOffGamePointTaxRateChange(Sender: TObject);
    procedure seSkill206FrozenRateChange(Sender: TObject);
    procedure seSkill43LDMBRateChange(Sender: TObject);
    procedure seSkill43LDMBTimeChange(Sender: TObject);
    procedure seSkill43LDMBPowerAddChange(Sender: TObject);
    procedure seSkill43HitWaitTimeChange(Sender: TObject);
    procedure chkSkill43LockParalyClick(Sender: TObject);
    procedure chkSkill58PowerTwoAttackClick(Sender: TObject);
    procedure seHeroLimitChange(Sender: TObject);
    procedure seHeroRunTimeChange(Sender: TObject);
    procedure seHeroWarrAttackMoveRateChange(Sender: TObject);
    procedure seMySellShopItemTimeChange(Sender: TObject);
    procedure chkMySellShopItemTimeClick(Sender: TObject);
    procedure chkHeroTargetAgainNoMoveClick(Sender: TObject);
    procedure chkDedingDisabledPKClick(Sender: TObject);
    procedure chkHeroJointAttackFlyClick(Sender: TObject);
    procedure chkHeroTargetAgainNoMoveDFClick(Sender: TObject);
    procedure chkSkill205PowerTwoAttackClick(Sender: TObject);
    procedure chkSkill31UseNewEffectClick(Sender: TObject);
    procedure seSkill46PowerBaseChange(Sender: TObject);
    procedure seSkill46SecRateChange(Sender: TObject);
    procedure seSkill46TimeChange(Sender: TObject);
    procedure seGuardNameColorChange(Sender: TObject);
    procedure chkOpenNewGuildClick(Sender: TObject);
    procedure chkMonNoAttackOffLinePlayerClick(Sender: TObject);
    procedure chkSkill64MakeStoneClick(Sender: TObject);
    procedure seSkillJointAttackLevelRateChange(Sender: TObject);
    procedure seContinuousAttackLevelRateChange(Sender: TObject);
    procedure seSkill69CDChange(Sender: TObject);
    procedure seHeroWarrAttacSkillErgumRateChange(Sender: TObject);
    procedure seSkill69AddTimeChange(Sender: TObject);
    procedure seSkill69AddRangeChange(Sender: TObject);
    procedure seHeroWarrAttakNearChange(Sender: TObject);
    procedure CheckBoxItemNameClick(Sender: TObject);
    procedure chkSkill69SameLevelClick(Sender: TObject);
    procedure edtJewelryBoxHintChange(Sender: TObject);
    procedure seCopySelfNameColorChange(Sender: TObject);
    procedure seHeroRecallCopySelfHPRateChange(Sender: TObject);
    procedure chkSkill61NotMagBubbleDefenceClick(Sender: TObject);
    procedure seMySellShowItemNamLenChange(Sender: TObject);
    procedure chkCloseNPCNoItemMsgClick(Sender: TObject);
    procedure seMerchant273NameColorChange(Sender: TObject);
    procedure seHeroLogonTimeMasterDieChange(Sender: TObject);
    procedure chkSkill15OfflineClearClick(Sender: TObject);
    procedure cbbHeroWarriorDefaultSkillChange(Sender: TObject);
    procedure seMagicMsgXChange(Sender: TObject);
    procedure seMagicMsgYChange(Sender: TObject);
    procedure chkHeroStateDlgNoMoveClick(Sender: TObject);
    procedure seStarBaseNumChange(Sender: TObject);
    procedure chkJewelryDecDuraClick(Sender: TObject);
    procedure chkRecordBeadExpClick(Sender: TObject);
    procedure chkSlaveLockTargetClick(Sender: TObject);
    procedure chkSkill62NotMagBubbleDefenceClick(Sender: TObject);
    procedure lstMagicACClick(Sender: TObject);
    procedure chkMagicACEnabledClick(Sender: TObject);
    procedure seMagicACHumChange(Sender: TObject);
    procedure seMagicACMonChange(Sender: TObject);
    procedure seMagicACHeroChange(Sender: TObject);
    procedure chkHongMoSuiteWithPowerClick(Sender: TObject);
    procedure chkDisableDuFuTakeArmRingLClick(Sender: TObject);
    procedure chkPosionStopIncHealthClick(Sender: TObject);
    procedure chkEnabledPosionDecMACClick(Sender: TObject);
    procedure sePosionDecMACRateChange(Sender: TObject);
    procedure seNearAttackPowerRateChange(Sender: TObject);
    procedure seSkillYedoPowerRateChange(Sender: TObject);
    procedure chkDogzPlugSettingPriorityClick(Sender: TObject);
    procedure chkBonePlugSettingPriorityClick(Sender: TObject);
    procedure chkSpiritualismLevelDiffClick(Sender: TObject);
    procedure seSpiritualismLevelDiffChange(Sender: TObject);
    procedure cbbSpiritualismMagicLevelChange(Sender: TObject);
    procedure seSpiritualismRateChange(Sender: TObject);
    procedure seSpiritualismRoyaltyTimeChange(Sender: TObject);
    procedure seSpiritualismBBCountChange(Sender: TObject);
    procedure seSlave9HPChange(Sender: TObject);
    procedure seSlave9ACChange(Sender: TObject);
    procedure seSlave9MACChange(Sender: TObject);
    procedure seSlave9DCChange(Sender: TObject);
    procedure seSlave9MoveSpeedChange(Sender: TObject);
    procedure seSlave9HitSpeedChange(Sender: TObject);
    procedure seSkill202RateChange(Sender: TObject);
    procedure chkSkill202RateOnlyMonClick(Sender: TObject);
    procedure seSkillAmyounsulCDTimeChange(Sender: TObject);
    procedure seSkillGroupAmyounsulCDTimeChange(Sender: TObject);
    procedure chkSkillGroupAmyounsulRedClick(Sender: TObject);
    procedure chkSkillGroupAmyounsulGreenClick(Sender: TObject);
    procedure edtSetOffLineLoginMapNameChange(Sender: TObject);
    procedure chkSlaveKillHumanIncPKClick(Sender: TObject);
    procedure chkSlaveLevelupUseNewAttrClick(Sender: TObject);
    procedure chkHeroForcePeaceModeClick(Sender: TObject);
    procedure chkSlaveAlwaysShowNameClick(Sender: TObject);
    procedure chkSlaveLevelupAddLowerAttrClick(Sender: TObject);
    procedure seHeroAttackHumPowerRateChange(Sender: TObject);
    procedure seHeroAttackMonPowerRatehange(Sender: TObject);
    procedure chkNoNeedFirDragonClick(Sender: TObject);
    procedure chkHeroStatus0Click(Sender: TObject);
    procedure chkHeroStatus1Click(Sender: TObject);
    procedure chkHeroStatus2Click(Sender: TObject);
    procedure chkHeroStatus3Click(Sender: TObject);
    procedure seMonAttackHeroPowerRateChange(Sender: TObject);
    procedure seSkill60AttackHumPowerRateChange(Sender: TObject);
    procedure seSkill62AttackHumPowerRateChange(Sender: TObject);
    procedure seSkill61AttackHumPowerRateChange(Sender: TObject);
    procedure seSkill65AttackHumPowerRateChange(Sender: TObject);
    procedure seSkill64AttackHumPowerRateChange(Sender: TObject);
    procedure seSkill63AttackHumPowerRateChange(Sender: TObject);
    procedure seSkill204BasicMbRateChange(Sender: TObject);
    procedure seHumanAttackHeroPowerRateChange(Sender: TObject);
    procedure edtHeroSayPrefixChange(Sender: TObject);
    procedure chkMasterRoyaltyFullHPClick(Sender: TObject);
    procedure chkSlaveDisableStruckClick(Sender: TObject);
    procedure btnAuctionClick(Sender: TObject);
    procedure seAuctionGoldTaxRateChange(Sender: TObject);
    procedure cbbAuctionBroadcastCurrencyTypeChange(Sender: TObject);
    procedure seAuctionBroadcastPriceChange(Sender: TObject);
    procedure seAuctionItemColor1Change(Sender: TObject);
    procedure seAuctionBroadcastShowTimeChange(Sender: TObject);
    procedure edtAuctionBroadcastTextChange(Sender: TObject);
    procedure chkOpenAuctionItemColorsClick(Sender: TObject);
    procedure chkMagTurnUndeadSameLevelClick(Sender: TObject);
    procedure seSkill25PowerRateChange(Sender: TObject);
    procedure cbbHPRockAddTypeChange(Sender: TObject);
    procedure cbbMPRockAddTypeChange(Sender: TObject);
    procedure cbbHMPRockAddTypeChange(Sender: TObject);
    procedure cbbHPRockTypeChange(Sender: TObject);
    procedure cbbMPRockTypeChange(Sender: TObject);
    procedure cbbHMPRockTypeChange(Sender: TObject);
    procedure chkHMPUse2TimesClick(Sender: TObject);
    procedure seSkill70CDChange(Sender: TObject);
    procedure chkElectrodelessUseMaxSCClick(Sender: TObject);
    procedure chkLuckUseNewAlgorismClick(Sender: TObject);
    procedure seCopySelfLevelUpAddExistTimeChange(Sender: TObject);
    procedure seSkill72LevelUpRateAddChange(Sender: TObject);
    procedure seSuperShiledLevelUpAddValidTimeChange(Sender: TObject);
    procedure seSuperShiledLevelUpDecPowerRateChange(Sender: TObject);
    procedure seCloseSuperShiledLevelUpDecRateChange(Sender: TObject);
    procedure seOpenSuperShiledLevelUpAddRateChange(Sender: TObject);
    procedure seSkill114LevelUpAddPowerRateChange(Sender: TObject);
    procedure chkDisHeroRunClick(Sender: TObject);
    procedure chkHeroRunHumClick(Sender: TObject);
    procedure chkHeroRunMonClick(Sender: TObject);
    procedure chkHeroRunNpcClick(Sender: TObject);
    procedure chkHeroRunGuardClick(Sender: TObject);
    procedure chkHeroSafeAreaClick(Sender: TObject);
    procedure chkHeroSafeAreaDisNpcRunClick(Sender: TObject);
    procedure chkSafeAreaDisShopStallHeroRunClick(Sender: TObject);
    procedure chkSafeAreaDisOffLineHeroRunClick(Sender: TObject);
    procedure chkHeroWarDisHumRunClick(Sender: TObject);
    procedure chkHeroWarHreoRunClick(Sender: TObject);
    procedure seDoMotaebo100PushDistanceChange(Sender: TObject);
    procedure chkDisableMoveParalysisHumanClick(Sender: TObject);
    procedure seWarrContinuousStatusLockTimeChange(Sender: TObject);
    procedure chkHeroNoTargetRecallBBClick(Sender: TObject);
    procedure seHeroAvoidTimeChange(Sender: TObject);
    procedure chkM2CacheRankDataClick(Sender: TObject);
    procedure seSkill37RangeChange(Sender: TObject);
    procedure seSkill37RangeAddChange(Sender: TObject);
    procedure chkSkill204DisableStopItemClick(Sender: TObject);
    procedure chkHeroDFAvoidTargetRightClick(Sender: TObject);
    procedure seSkill113PowerRateChange(Sender: TObject);
    procedure seSkill113HitWaitTimeChange(Sender: TObject);
    procedure seHeroSkill113HitWaitTimeChange(Sender: TObject);
    procedure seSkill115PowerRateChange(Sender: TObject);
    procedure seSkill116PowerRateChange(Sender: TObject);
    procedure seSkill117PowerRateChange(Sender: TObject);
    procedure seSkill115HitWaitTimeChange(Sender: TObject);
    procedure seHeroSkill115HitWaitTimeChange(Sender: TObject);
    procedure seSkill116HitWaitTimeChange(Sender: TObject);
    procedure seHeroSkill116HitWaitTimeChange(Sender: TObject);
    procedure seSkill117HitWaitTimeChange(Sender: TObject);
    procedure seHeroSkill117HitWaitTimeChange(Sender: TObject);
    procedure seSkill117RangeChange(Sender: TObject);
    procedure seSkill116RangeChange(Sender: TObject);
    procedure seSkill116LevelUpAddPowerRateChange(Sender: TObject);
    procedure seSkill115LevelUpAddPowerRateChange(Sender: TObject);
    procedure seSkill117LevelUpAddPowerRateChange(Sender: TObject);
    procedure chkSkill115UseNGClick(Sender: TObject);
    procedure chkSkill115NGNoEnoughDecHPClick(Sender: TObject);
    procedure seSkill115NGNoEnoughDecHPValueChange(Sender: TObject);
    procedure cbbSkill115NGNoEnoughDecHPTypeChange(Sender: TObject);
    procedure chkEnableDoubleFireHitDelayCloseClick(Sender: TObject);
    procedure cbbDoubleFireHitDelayCloseTypeChange(Sender: TObject);
    procedure seDoubleFireHitDelayCloseValueChange(Sender: TObject);
    procedure seMaxMabMabeHitMabeTimeChange(Sender: TObject);
    procedure seSkillLighteningPowerRateChange(Sender: TObject);
    procedure seSkillGroupLighteningPowerRateChange(Sender: TObject);
    procedure seAuctionGameGoldTaxRateChange(Sender: TObject);
    procedure seAuctionGameDiamondTaxRateChange(Sender: TObject);
    procedure seAuctionGameGirdTaxRateChange(Sender: TObject);
    procedure seAuctionGamePointTaxRateChange(Sender: TObject);
    procedure chkAuctionCurrencyType1Click(Sender: TObject);
    procedure chkCopyMonWarrorAttackClick(Sender: TObject);
    procedure chkCopyMon700HPUseBaseAttackClick(Sender: TObject);
    procedure seMagicFailMsgFColorChange(Sender: TObject);
    procedure seMagicFailMsgBColorChange(Sender: TObject);
    procedure chkMagicMsgXRightToLeftClick(Sender: TObject);
    procedure chkMagicMsgAddChatBoardMsgClick(Sender: TObject);
    procedure seMagicOKMsgFColorChange(Sender: TObject);
    procedure seMagicOKMsgBColorChange(Sender: TObject);
    procedure chkHeroHitCmpClick(Sender: TObject);
    procedure seWarrCmpInvTimeChange(Sender: TObject);
    procedure chkMagicMsgYBottomToTopClick(Sender: TObject);
    procedure chkHeroNotAvoidLastHinterClick(Sender: TObject);
    procedure chkSlaveNoLockHumanClick(Sender: TObject);
    procedure seHeroSkill58WaitTimeChange(Sender: TObject);
    procedure seSkill203PoisonRateChange(Sender: TObject);
    procedure seToxicSmokeDecHPRateChange(Sender: TObject);
    procedure seSkill113RateAddWithSkill63Change(Sender: TObject);
    procedure chkSkill63UseSpeedPointClick(Sender: TObject);
    procedure chkSkill31UseLEGEffectClick(Sender: TObject);
    procedure chkNoShowNewGuildHumanCountClick(Sender: TObject);
    procedure seHero700HPValueChange(Sender: TObject);
    procedure chkDisableSkill41MbSameLevelClick(Sender: TObject);
    procedure chkSkill71DisableAttackSameLevelClick(Sender: TObject);
    procedure chkSkill71DisableAttackFriendClick(Sender: TObject);
    procedure seSkill204DistanceChange(Sender: TObject);
    procedure seStarLineMaxCountChange(Sender: TObject);
    procedure seAuctioningItemsCountChange(Sender: TObject);
    procedure seSkillFireCharmPowerRateChange(Sender: TObject);
    procedure seSkill202PowerRateChange(Sender: TObject);
    procedure seHeroSkill66HighPowerRateChange(Sender: TObject);
    procedure chkHeroSkill66HighAttackNoUseRateClick(Sender: TObject);
    procedure seDefenceHumChange(Sender: TObject);
    procedure seDefenceMonChange(Sender: TObject);
    procedure seDefenceHeroChange(Sender: TObject);
    procedure chkSkill60NotMagBubbleDefenceClick(Sender: TObject);
    procedure chkDisableMonsterAttackHeroClick(Sender: TObject);
    procedure chkDisableHeroAttackMonsterClick(Sender: TObject);
    procedure chkSkill72DisableStopItemClick(Sender: TObject);
    procedure seUseSkillCloseSuperShiled0_RateChange(Sender: TObject);
    procedure seUseSkillCloseSuperShiled0_RateAddChange(Sender: TObject);
    procedure edtNGLevelPowerAdd_PowerChange(Sender: TObject);
    procedure edtNGLevelPowerDec_LevelChange(Sender: TObject);
    procedure edtNGLevelPowerDec_PowerChange(Sender: TObject);
    procedure edtSkillContinuousCloseDefenseRates100Change(Sender: TObject);
    procedure seSkill100BreakDefenceUpRateChange(Sender: TObject);
    procedure seSkillContinueOrderBlastRates1Change(Sender: TObject);
    procedure seSkill102AttackRangeChange(Sender: TObject);
    procedure seSkill103AttackRangeChange(Sender: TObject);
    procedure seSkill105AttackRangeChange(Sender: TObject);
    procedure seNGMaxLevelLimteChange(Sender: TObject);
    procedure seSkill106AddFrozenRateChange(Sender: TObject);
    procedure seSkill106AddFrozenRate2Change(Sender: TObject);
    procedure seSkill106AddFrozenTimeChange(Sender: TObject);
    procedure seSkill106AddFrozenTime2Change(Sender: TObject);
    procedure seSkill110PushedRateChange(Sender: TObject);
    procedure seSkill110PushedRate2Change(Sender: TObject);
    procedure seSkill110PushedRangeChange(Sender: TObject);
    procedure seSkill110PushedRange2Change(Sender: TObject);
    procedure chkSkill110PushedHighLevelClick(Sender: TObject);
    procedure cbbSkill31LevelChange(Sender: TObject);
    procedure edtHumSkill7PowerLV4Change(Sender: TObject);
    procedure chkRecallManySlave3Click(Sender: TObject);
    procedure chkSpiritualismDisableUndeadMonClick(Sender: TObject);
    procedure seRecallMonCountChange(Sender: TObject);
    procedure edtHumSkill45PowerLV4Change(Sender: TObject);
    procedure edtHumSkill13PowerLV4Change(Sender: TObject);
    procedure seMyShopOperateIntervalChange(Sender: TObject);
    procedure cbbHeroNeedMagicItemChange(Sender: TObject);
    procedure seHeroWarrNearFireSwordChange(Sender: TObject);
    procedure seHMPDivDuraChange(Sender: TObject);
    procedure chkSellPlayerCurrencyType1Click(Sender: TObject);
    procedure seSellPlayerGoldTaxRateChange(Sender: TObject);
    procedure seSellPlayerGameGoldTaxRateChange(Sender: TObject);
    procedure seSellPlayerGameDiamondTaxRateChange(Sender: TObject);
    procedure seSellPlayerGameGirdTaxRateChange(Sender: TObject);
    procedure seSellPlayerGamePointTaxRateChange(Sender: TObject);
    procedure btnSellPlayerOKClick(Sender: TObject);
    procedure seSellPlayerTimeChange(Sender: TObject);
    procedure chkSellPlayerViewStorageClick(Sender: TObject);
    procedure chkSellPlayerViewStorageExClick(Sender: TObject);
    procedure seSellPlayerViewOtherInfoTextOffsetXChange(Sender: TObject);
    procedure seSellPlayerViewOtherInfoTextOffsetYChange(Sender: TObject);
    procedure chkHeroOnlyPickMonsterItemClick(Sender: TObject);
    procedure chkDisableWarrContinueHitClick(Sender: TObject);
    procedure edtDisableWarrContinueHitIDsChange(Sender: TObject);
    procedure seWarrContinueHitMinIntervalChange(Sender: TObject);
    procedure cbbElectrodelessNewLevelChange(Sender: TObject);
    procedure seElectrodelessTimeLNewChange(Sender: TObject);
    procedure seElectrodelessPowerRateLNewChange(Sender: TObject);
    procedure chkHeroKillMonTriggerClick(Sender: TObject);
    procedure chkCopyMonInheritedMasterSpeedClick(Sender: TObject);
    procedure chkGroupUseOldModeClick(Sender: TObject);
    procedure chkSellPlayerAutoRecallHeroClick(Sender: TObject);
    procedure seHeroTargetRangeLimitClick(Sender: TObject);
    procedure seSkill202PowerDecChange(Sender: TObject);
    procedure seSkill202PowerMinChange(Sender: TObject);
    procedure seHeroProtectFlyRangeChange(Sender: TObject);
    procedure seHeroLockFlyRangeChange(Sender: TObject);
    procedure seHeroJoinAttackFlyRangeChange(Sender: TObject);
    procedure chkProhibitModifyPricesClick(Sender: TObject);
    procedure chkBarbaricSeptumClick(Sender: TObject);

  private
    boOpened: Boolean;
    boModValued: Boolean;
    boDropOverLapItem: Boolean;

    boSendServerConfig: Boolean;
    procedure ModValue();
    procedure uModValue();
    procedure RefReNewLevelConf;
    procedure RefUpgradeWeapon;
    procedure RefMakeMine;
    procedure RefWinLottery;
    procedure RefMonUpgrade;
    procedure RefGeneral;
    procedure RefSpiritMutiny;
    procedure RefMagicSkill;
    procedure RefMonSayMsg;
    procedure RefWeaponMakeLuck();
    { Private declarations }
  public
    procedure DoOpen;
    { Public declarations }
  end;

var
  frmFunctionConfig: TfrmFunctionConfig;

implementation

uses
  M2Share;

{$R *.dfm}
{ TfrmFunctionConfig }

procedure TfrmFunctionConfig.ModValue;
begin
  boModValued := True;
  ButtonPasswordLockSave.Enabled := True;
  ButtonGeneralSave.Enabled := True;
  ButtonSkillSave.Enabled := True;
  ButtonUpgradeWeaponSave.Enabled := True;
  ButtonMasterSave.Enabled := True;
  ButtonMakeMineSave.Enabled := True;
  ButtonWinLotterySave.Enabled := True;
  ButtonReNewLevelSave.Enabled := True;
  ButtonMonUpgradeSave.Enabled := True;
  ButtonSpiritMutinySave.Enabled := True;
  ButtonMonSayMsgSave.Enabled := True;
  ButtonHeroOptionSave.Enabled := True;
  ButtonOffLineSave.Enabled := True;
  ButtonMyShopSave.Enabled := True;
  ButtonOther.Enabled := True;
  btnBonusAbilofSave.Enabled := True;
  btnOther3.Enabled := True;
  ButtonWeaponMakeLuckSave.Enabled := True;
end;

procedure TfrmFunctionConfig.uModValue;
begin
  boModValued := False;
  ButtonPasswordLockSave.Enabled := False;
  ButtonGeneralSave.Enabled := False;
  ButtonSkillSave.Enabled := False;
  ButtonUpgradeWeaponSave.Enabled := False;
  ButtonMasterSave.Enabled := False;
  ButtonMakeMineSave.Enabled := False;
  ButtonWinLotterySave.Enabled := False;
  ButtonReNewLevelSave.Enabled := False;
  ButtonMonUpgradeSave.Enabled := False;
  ButtonSpiritMutinySave.Enabled := False;
  ButtonMonSayMsgSave.Enabled := False;
  ButtonHeroOptionSave.Enabled := False;
  ButtonOffLineSave.Enabled := False;
  ButtonMyShopSave.Enabled := False;
  ButtonOther.Enabled := False;
  btnBonusAbilofSave.Enabled := False;
  boSendServerConfig := False;
  btnOther3.Enabled := False;
  ButtonWeaponMakeLuckSave.Enabled := False;
end;

procedure TfrmFunctionConfig.FunctionConfigControlChanging(Sender: TObject; var AllowChange: Boolean);
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

// 打开基础设置框，读取配置

procedure TfrmFunctionConfig.DoOpen;
var
  I, MagicID: Integer;
  s01: string;
  MagicACInfo: PMagicACInfo;
  Magic: pTMagic;
  CustomMagicConfig: TCustomMagicConfig;
begin
  boOpened := False;

  RefGeneral();
  CheckBoxHungerSystem.Checked := g_Config.boHungerSystem;
  CheckBoxHungerDecHP.Checked := g_Config.boHungerDecHP;
  CheckBoxHungerDecPower.Checked := g_Config.boHungerDecPower;

  CheckBoxHungerSystemClick(CheckBoxHungerSystem);

  CheckBoxEnablePasswordLock.Checked := g_Config.boPasswordLockSystem;
  CheckBoxLockGetBackItem.Checked := g_Config.boLockGetBackItemAction;
  CheckBoxLockDealItem.Checked := g_Config.boLockDealAction;
  CheckBoxLockDropItem.Checked := g_Config.boLockDropAction;
  CheckBoxLockWalk.Checked := g_Config.boLockWalkAction;
  CheckBoxLockRun.Checked := g_Config.boLockRunAction;
  CheckBoxLockHit.Checked := g_Config.boLockHitAction;
  CheckBoxLockSpell.Checked := g_Config.boLockSpellAction;
  CheckBoxLockSendMsg.Checked := g_Config.boLockSendMsgAction;
  CheckBoxLockInObMode.Checked := g_Config.boLockInObModeAction;

  CheckBoxLockLogin.Checked := g_Config.boLockHumanLogin;
  CheckBoxLockUseItem.Checked := g_Config.boLockUserItemAction;
  chkLockChallenge.Checked := g_Config.boLockChallenge;
  // 禁止挑战 chongchong 2013-07-22
  chkLockSummonHero.Checked := g_Config.boLockSummonHero;
  // 禁止召唤英雄 chongchong 2013-07-22
  chkLockShop.Checked := g_Config.boLockShop;
  // 禁止商铺 chongchong 2013-07-22
  chkLockStall.Checked := g_Config.boLockStall;
  // 禁止摆摊 chongchong 2013-07-22

  CheckBoxEnablePasswordLockClick(CheckBoxEnablePasswordLock);
  CheckBoxLockLoginClick(CheckBoxLockLogin);

  EditErrorPasswordCount.Value := g_Config.nPasswordErrorCountLock;

  EditBoneFammName.Text := g_Config.sBoneFamm;
  EditBoneFammCount.Value := g_Config.nBoneFammCount;

  SpinEditMagDelayTime.Value := g_Config.nMagDelayTimeDoubly;
  // SpinEditMagPower.Value := g_Config.nMagPowerDoubly;
  seNearAttackPowerRate.Value := g_Config.nNearAttackPowerRate;

  for I := Low(g_Config.BoneFammArray) to High(g_Config.BoneFammArray) do
  begin
    if g_Config.BoneFammArray[I].nHumLevel <= 0 then
      Break;

    GridBoneFamm.Cells[0, I + 1] := IntToStr(g_Config.BoneFammArray[I].nHumLevel);
    GridBoneFamm.Cells[1, I + 1] := g_Config.BoneFammArray[I].sMonName;
    GridBoneFamm.Cells[2, I + 1] := IntToStr(g_Config.BoneFammArray[I].nCount);
    GridBoneFamm.Cells[3, I + 1] := IntToStr(g_Config.BoneFammArray[I].nLevel);
  end;

  EditDogzName.Text := g_Config.sDogz;
  EditDogzCount.Value := g_Config.nDogzCount;
  for I := Low(g_Config.DogzArray) to High(g_Config.DogzArray) do
  begin
    if g_Config.DogzArray[I].nHumLevel <= 0 then
      Break;
    GridDogz.Cells[0, I + 1] := IntToStr(g_Config.DogzArray[I].nHumLevel);
    GridDogz.Cells[1, I + 1] := g_Config.DogzArray[I].sMonName;
    GridDogz.Cells[2, I + 1] := IntToStr(g_Config.DogzArray[I].nCount);
    GridDogz.Cells[3, I + 1] := IntToStr(g_Config.DogzArray[I].nLevel);
  end;

  EditBigDogzName.Text := g_Config.sBigDogz;
  EditBigDogzCount.Value := g_Config.nBigDogzCount;
  for I := Low(g_Config.BigDogzArray) to High(g_Config.BigDogzArray) do
  begin
    if g_Config.BigDogzArray[I].nHumLevel <= 0 then
      Break;
    GridBigDogz.Cells[0, I + 1] := IntToStr(g_Config.BigDogzArray[I].nHumLevel);
    GridBigDogz.Cells[1, I + 1] := g_Config.BigDogzArray[I].sMonName;
    GridBigDogz.Cells[2, I + 1] := IntToStr(g_Config.BigDogzArray[I].nCount);
    GridBigDogz.Cells[3, I + 1] := IntToStr(g_Config.BigDogzArray[I].nLevel);
  end;

  edtMonthSpiritName.Text := g_Config.sMonthSpirit;
  seMonthSpiritCount.Value := g_Config.nMonthSpiritCount;
  seMonthSpiritAttackRange.Value := g_Config.nMonthSpiritAttackRange;

  for I := Low(g_Config.MonthSpiritArray) to High(g_Config.MonthSpiritArray) do
  begin
    if g_Config.MonthSpiritArray[I].nHumLevel <= 0 then
      Break;
    GridMonthSpirit.Cells[0, I + 1] := IntToStr(g_Config.MonthSpiritArray[I].nHumLevel);
    GridMonthSpirit.Cells[1, I + 1] := g_Config.MonthSpiritArray[I].sMonName;
    GridMonthSpirit.Cells[2, I + 1] := IntToStr(g_Config.MonthSpiritArray[I].nCount);
    GridMonthSpirit.Cells[3, I + 1] := IntToStr(g_Config.MonthSpiritArray[I].nLevel);
  end;

  ComboBoxBagItemCount.Items.Clear;

  for I := Low(g_Config.HeroBagItemCounts) to High(g_Config.HeroBagItemCounts) do
  begin
    case I of
      0:
        s01 := '10格';
      1:
        s01 := '20格';
      2:
        s01 := '30格';
      3:
        s01 := '35格';
      4:
        s01 := '40格';
    end;
    ComboBoxBagItemCount.Items.AddObject(s01, TObject(g_Config.HeroBagItemCounts[I]));
  end;

  for I := 1 to GridMedicineExp.RowCount - 1 do
  begin // 药力值 20080623
    GridMedicineExp.Cells[1, I] := IntToStr(g_Config.dwMedicineLevelNeedExps[I]);
  end;

  EditDecMedicineTime.Value := g_Config.nDecMedicineTime;
  EditDecMedicineValue.Value := g_Config.nDecMedicineValue;
  EditIncAlcoholTime.Value := g_Config.nIncAlcoholTime;
  EditDecDrinkTime.Value := g_Config.nDecDrinkTime;
  EditMaxAlcoholValue.Value := g_Config.nMaxAlcoholValue;
  EditIncAlcoholValue.Value := g_Config.nIncAlcoholValue;

  RefMagicSkill();

  RefUpgradeWeapon();
  RefMakeMine();
  RefWinLottery();
  EditMasterOKLevel.Value := g_Config.nMasterOKLevel;
  EditMasterOKCreditPoint.Value := g_Config.nMasterOKCreditPoint;
  EditMasterOKBonusPoint.Value := g_Config.nMasterOKBonusPoint;
  seMasterCount.Value := g_Config.nMasterCount;

  chkSkill71PullPlayObject.Checked := g_Config.boSkill71PullPlayObject;
  chkSkill71PullSlave.Checked := g_Config.boSkill71PullSlave;
  chkSkill71PullCrossInSafeZone.Checked := g_Config.boSkill71PullCrossInSafeZone;
  chkSkill71DisableAttackSameLevel.Checked := g_Config.boSkill71DisableAttackSameLevel;
  chkSkill71DisableAttackFriend.Checked := g_Config.boSkill71DisableAttackFriend;

  // 灭天火 piaoyun 2013-07-25
  chkPlayObjectReduceMP.Checked := g_Config.boPlayObjectReduceMP;
  seMakeFireDayPowerRate.Value := g_Config.nMakeFireDayPowerRate;
  seMakeFireDayTime.Value := g_Config.nMakeFireDayTime;

  CheckBoxItemName.Checked := g_Config.boChangeUseItemNameByPlayName;
  EditItemName.Text := g_Config.sChangeUseItemName;

  // seSkill41CD.Value := g_Config.nSkill41CD;

  seHPRockRate.Value := g_Config.nHPRockRate;
  cbbHPRockType.ItemIndex := g_Config.nHPRockType;
  seHPRockTime.Value := g_Config.nHPRockTime;
  cbbHPRockAddType.ItemIndex := g_Config.nHPRockAddType;
  seHPRockAddValue.Value := g_Config.nHPRockAddValue;
  seHPRockDecValue.Value := g_Config.nHPRockDecValue;

  seMPRockRate.Value := g_Config.nMPRockRate;
  cbbMPRockType.ItemIndex := g_Config.nMPRockType;
  seMPRockTime.Value := g_Config.nMPRockTime;
  cbbMPRockAddType.ItemIndex := g_Config.nMPRockAddType;
  seMPRockAddValue.Value := g_Config.nMPRockAddValue;
  seMPRockDecValue.Value := g_Config.nMPRockDecValue;

  seHMPRockRate.Value := g_Config.nHMPRockRate;
  cbbHMPRockType.ItemIndex := g_Config.nHMPRockType;
  seHMPRockTime.Value := g_Config.nHMPRockTime;
  cbbHMPRockAddType.ItemIndex := g_Config.nHMPRockAddType;
  seHMPRockAddValue.Value := g_Config.nHMPRockAddValue;
  seHMPRockDecValue.Value := g_Config.nHMPRockDecValue;
  chkHMPUse2Times.Checked := g_Config.boHMPUse2Times;
  seHMPDivDura.Value := g_Config.nHMPDivDura;

  CheckBoxHeroPickUpItem.Checked := g_Config.boHeroPickUpItem;
  cbbHeroNeedMagicItem.ItemIndex := g_Config.nHeroNeedMagicItem;

  chkHeroOnlyPickMonsterItem.Checked := g_Config.boHeroOnlyPickMonsterItem;

  CheckBoxHeroShowMasterName.Checked := g_Config.boHeroShowMasterName;
  seHeroKillMonExpRate.Value := g_Config.nHeroKillMonExpRate;
  seHeroNotKillMonExpRate.Value := g_Config.nHeroNotKillMonExpRate;

  EditHeroRecallTime.Value := g_Config.nRecallHeroTime;
  EditRecallDeputyHeroTime.Value := g_Config.nRecallDeputyHeroTime;
  seClearHeroGhostTick.Value := g_Config.nClearHeroGhostTick;
  EditHeroWarrorAttackTime.Value := g_Config.dwHeroWarrorAttackTime;
  EditHeroWizardAttackTime.Value := g_Config.dwHeroWizardAttackTime;
  EditHeroTaoistAttackTime.Value := g_Config.dwHeroTaoistAttackTime;
  seHeroAvoidTime.Value := g_Config.dwHeroAvoidTime;

  EditHeroWarrorWalkTime.Value := g_Config.dwHeroWarrorWalkTime;
  EditHeroWizardWalkTime.Value := g_Config.dwHeroWizardWalkTime;
  EditHeroTaoistWalkTime.Value := g_Config.dwHeroTaoistWalkTime;

  chkHeroHitCmp.Checked := g_Config.boHeroHitCmp;
  seWarrCmpInvTime.Value := g_Config.nWarrCmpInvTime;

  EditHeroNameColor.Value := g_Config.btHeroNameColor;
  EditHeroSuffixName.Text := g_Config.sHeroSuffixName;
  edtHeroSayPrefix.Text := g_Config.sHeroSayPrefix;

  // seHeroAttackRange.Value := g_Config.dwHeroAttackRange;

  // 龙影剑法 piaoyun 2013-07-25
  seSkill42PowerRate.Value := g_Config.nSkill42PowerRate;
  seSkill42HitWaitTime.Value := g_Config.nSkill42HitWaitTime;
  seSkill42Range.Value := g_Config.nSkill42Range;

  // 双龙斩 piaoyun 2013-07-25
  seSkill40PowerRate.Value := g_Config.nSkill40PowerRate;
  seSkill25PowerRate.Value := g_Config.nSkill25PowerRate;

  seSkill43HitWaitTime.Value := g_Config.nSkill43HitWaitTime;
  EditSkill43PowerRate.Value := g_Config.nSkill43PowerRate;
  seSkill43LDMBRate.Value := g_Config.nSkill43LDMBRate;
  seSkill43LDMBTime.Value := g_Config.nSkill43LDMBTime;
  seSkill43LDMBPowerAdd.Value := g_Config.nSkill43LDMBPowerAdd;
  chkSkill43LockParaly.Checked := g_Config.boSkill43LockParaly;

  EditSkill56PowerRate.Value := g_Config.nSkill56PowerRate;
  EditSkill58PowerRate.Value := g_Config.nSkill58PowerRate;
  chkSkill58PowerTwoAttack.Checked := g_Config.boSkill58PowerTwoAttack;

  seSkill60PowerRate.Value := g_Config.nSkill60PowerRate;
  seSkill60AttackHumPowerRate.Value := g_Config.nSkill60AttackHumPowerRate;
  seSkill60PowerRange.Value := g_Config.nSkill60PowerRange;
  chkSkill60NotMagBubbleDefence.Checked := g_Config.boSkill60NotMagBubbleDefence;

  seSkill61PowerRate.Value := g_Config.nSkill61PowerRate;
  seSkill61AttackHumPowerRate.Value := g_Config.nSkill61AttackHumPowerRate;
  chkSkill61NotMagBubbleDefence.Checked := g_Config.boSkill61NotMagBubbleDefence;
  chkSkill62NotMagBubbleDefence.Checked := g_Config.boSkill62NotMagBubbleDefence;

  seSkill62PowerRate.Value := g_Config.nSkill62PowerRate;
  seSkill62AttackHumPowerRate.Value := g_Config.nSkill62AttackHumPowerRate;

  seSkill63PowerRate.Value := g_Config.nSkill63PowerRate;
  seSkill63AttackHumPowerRate.Value := g_Config.nSkill63AttackHumPowerRate;
  seSkill63PowerRange.Value := g_Config.nSkill63PowerRange;
  chkSkill63GreenPoison.Checked := g_Config.boSkill63GreenPoison;
  chkSkill63UseSpeedPoint.Checked := g_Config.boSkill63UseSpeedPoint;

  seSkillJointAttackLevelRate.Value := g_Config.nSkillJointAttackLevelRate;

  seSkill64PowerRate.Value := g_Config.nSkill64PowerRate;
  seSkill64AttackHumPowerRate.Value := g_Config.nSkill64AttackHumPowerRate;
  seSkill64PowerRange.Value := g_Config.nSkill64PowerRange;
  chkSkill64MakeStone.Checked := g_Config.boSkill64MakeStone;

  seSkill65PowerRate.Value := g_Config.nSkill65PowerRate;
  seSkill65AttackHumPowerRate.Value := g_Config.nSkill65AttackHumPowerRate;
  seSkill65PowerRange.Value := g_Config.nSkill65PowerRange;

  EditSkill52PowerRate.Value := g_Config.nSkill52PowerRate;
  EditSkill52AttackRange.Value := g_Config.nSkill52AttackRange;

  EditMaxAngryValue.Value := g_Config.btMaxAngryValue;
  EditAddAngryValue.Value := g_Config.nAddAngryValue;
  EditDecFirDragonPoint.Value := g_Config.nDecFirDragonPoint;
  EditAddAngryValueTime.Value := g_Config.nAddAngryValueTime;
  chkWarrorAttack.Checked := g_Config.boWarrorAttack;
  chkHeroNotAvoidLastHinter.Checked := g_Config.boHeroNotAvoidLastHinter;

  chkNoNeedFirDragon.Visible := g_nKey_HeroExt = 1;
  if chkNoNeedFirDragon.Visible then
  begin
    chkNoNeedFirDragon.Checked := g_Config.boNoNeedFirDragon;
  end;

  TabSheet54.Visible := g_nKey_Auction = 1;

  seFireHitWaitTime.Value := g_Config.nFireHitWaitTime;
  seFireHitPowerRate.Value := g_Config.nFireHitPowerRate;

  EditNeedGuardLevel.Value := g_Config.nNeedGuardLevel;
  EditGuardRange.Value := g_Config.nGuardRange;

  seCopySelfMaxCount.Value := g_Config.nCopySelfMaxCount;
  seCopySelfExistTime.Value := g_Config.nCopySelfExistTime;
  seCopySelfLevelUpAddExistTime.Value := g_Config.nCopySelfLevelUpAddExistTime;
  chkCopySelfNonUseSpellPoint.Checked := g_Config.boCopySelfNonUseSpellPoint;
  chkAlwaysFollowMasterAttack.Checked := g_Config.boAlwaysFollowMasterAttack;

  EditMagicItemRate.Value := g_Config.nMagicItemRate;

  seCopySelfNameColor.Value := g_Config.btCopySelfNameColor;
  edtCopySelfSuffix.Text := g_Config.sCopySelfSuffix;
  chkShowCopySelfSuffix.Checked := g_Config.boShowCopySelfSuffix;

  CheckBoxOffLineLoginSafeArea.Checked := g_Config.boOffLineLoginSafeArea;
  RadioButtonOffLineLoginMapName1.Checked := g_Config.btOffLineLoginMapName = 0;
  RadioButtonOffLineLoginMapName2.Checked := g_Config.btOffLineLoginMapName = 1;
  edtSetOffLineLoginMapName.Text := g_Config.sSetOffLineLoginMapName;

  chkMonNoAttackOffLinePlayer.Checked := g_Config.boMonNoAttackOffLinePlayer;

  RadioGroupHumNeedMagicItem.ItemIndex := g_Config.nHumNeedMagicItem;

  EditSWordHitWaitTime.Value := g_Config.nSWordHitWaitTime;

  seSkill69CD.Value := g_Config.nSkill69CD;
  seSkill69AddTime.Value := g_Config.nSkill69AddTime;
  seSkill69AddRange.Value := g_Config.nSkill69AddRange;
  chkSkill69SameLevel.Checked := g_Config.boSkill69SameLevel;

  seSkill70CD.Value := g_Config.nSkill70CD;

  EditSkill66HitWaitTime.Value := g_Config.nSkill66CD;
  seSkill71CD.Value := g_Config.nSkill71CD;
  seHeroSkill66HitWaitTime.Value := g_Config.nHeroSkill66CD;

  // 乾坤大挪移
  seSkill72HitWaitTime.Value := g_Config.nSkill72CD;
  seSkill72Rate.Value := g_Config.nSkill72Rate;
  seSkill72LevelUpRateAdd.Value := g_Config.nSkill72LevelUpRateAdd;
  chkSkill72DisableStopItem.Checked := g_Config.boSkill72DisableStopItem;

  EditSkill57AddHPRate.Value := g_Config.nSkill57AddHPRate;
  EditSkill57PowerRate.Value := g_Config.nSkill57PowerRate;
  seSkill57Time.Value := g_Config.nSkill57Time;

  seSkill46PowerBase.Value := g_Config.nSkill46PowerBase;
  seSkill46SecRate.Value := g_Config.nSkill46SecRate;
  seSkill46Time.Value := g_Config.nSkill46Time;

  EditSkill58AttackRange.Value := g_Config.nSkill58AttackRange;

  seHumSkill66HighPowerRate.Value := g_Config.nHumSkill66HighPowerRate;
  seHeroSkill66HighPowerRate.Value := g_Config.nHeroSkill66HighPowerRate;
  seHeroSkill66PowerRate.Value := g_Config.nHeroSkill66PowerRate;

  chkHeroSkill66HighAttackNoUseRate.Checked := g_Config.boHeroSkill66HighAttackNoUseRate;
  seHeroSkill66HighAttackRate.Value := g_Config.nHeroSkill66HighAttackRate;

  EditMonthSpiritHighAttackRate.Value := g_Config.nMonthSpiritHighAttackRate;

  seSkill113PowerRate.Value := g_Config.nSkill113PowerRate;
  seSkill113HitWaitTime.Value := g_Config.nSkill113CD;
  seHeroSkill113HitWaitTime.Value := g_Config.nHeroSkill113CD;

  seSkill115PowerRate.Value := g_Config.nSkill115PowerRate;
  seSkill115HitWaitTime.Value := g_Config.nSkill115CD;
  seHeroSkill115HitWaitTime.Value := g_Config.nHeroSkill115CD;
  seSkill115LevelUpAddPowerRate.Value := g_Config.nSkill115LevelUpAddPowerRate;

  seSkill116PowerRate.Value := g_Config.nSkill116PowerRate;
  seSkill116HitWaitTime.Value := g_Config.nSkill116CD;
  seHeroSkill116HitWaitTime.Value := g_Config.nHeroSkill116CD;
  seSkill116Range.Value := g_Config.nSkill116Range;
  seSkill116LevelUpAddPowerRate.Value := g_Config.nSkill116LevelUpAddPowerRate;

  seSkill117PowerRate.Value := g_Config.nSkill117PowerRate;
  seSkill117HitWaitTime.Value := g_Config.nSkill117CD;
  seHeroSkill117HitWaitTime.Value := g_Config.nHeroSkill117CD;
  seSkill117Range.Value := g_Config.nSkill117Range;
  seSkill117LevelUpAddPowerRate.Value := g_Config.nSkill117LevelUpAddPowerRate;

  chkSkill115UseNG.Checked := g_Config.boSkill115UseNG;
  chkSkill115NGNoEnoughDecHP.Checked := g_Config.boSkill115NGNoEnoughDecHP;
  seSkill115NGNoEnoughDecHPValue.Value := g_Config.nSkill115NGNoEnoughDecHPValue;
  cbbSkill115NGNoEnoughDecHPType.ItemIndex := g_Config.nSkill115NGNoEnoughDecHPType;

  EditMonthSpiritHighPowerRate.Value := g_Config.nMonthSpiritHighPowerRate;
  EditRecallBigDogWaitTime.Value := g_Config.nRecallBigDogWaitTime;
  CheckBoxMonthSpiritAttackSame.Checked := g_Config.boMonthSpiritAttackSame;
  CheckBoxMonthSpiritUseMasterMP.Checked := g_Config.boMonthSpiritUseMasterMP;

  seSuperShiledValidTime.Value := g_Config.nSuperShiledValidTime;
  seSuperShiledLevelUpAddValidTime.Value := g_Config.nSuperShiledLevelUpAddValidTime;
  seLastSuperShiledTime.Value := g_Config.nLastSuperShiledTime;
  seSuperShiledPowerRate.Value := g_Config.nSuperShiledPowerRate;
  seSuperShiledLevelUpDecPowerRate.Value := g_Config.nSuperShiledLevelUpDecPowerRate;
  seOpenSuperShiledRate.Value := g_Config.nOpenSuperShiledRate;
  seOpenSuperShiledLevelUpAddRate.Value := g_Config.nOpenSuperShiledLevelUpAddRate;
  seCloseSuperShiledRate.Value := g_Config.nCloseSuperShiledRate;
  seCloseSuperShiledLevelUpDecRate.Value := g_Config.nCloseSuperShiledLevelUpDecRate;

  CheckBoxAutoOpenSuperShiled.Checked := g_Config.boAutoOpenSuperShiled;
  chkShowSuperShiledEffect.Checked := g_Config.boShowSuperShiledEffect;
  chkShowSuperShiledSound.Checked := g_Config.boShowSuperShiledSound;
  chkShowSuperShiledEffect2.Checked := g_Config.boShowSuperShiledEffect2;
  chkShowSuperShiledSound2.Checked := g_Config.boShowSuperShiledSound2;

  // 护体神盾提示 piaoyun 2013-07-26
  chkCloseSuperShiledHint.Checked := g_Config.boCloseSuperShiledHint;

  chkUseSkillCloseSuperShiled0.Checked := g_Config.UseSkillCloseSuperShileds[0];
  chkUseSkillCloseSuperShiled1.Checked := g_Config.UseSkillCloseSuperShileds[1];
  chkUseSkillCloseSuperShiled2.Checked := g_Config.UseSkillCloseSuperShileds[2];
  chkUseSkillCloseSuperShiled3.Checked := g_Config.UseSkillCloseSuperShileds[3];
  chkUseSkillCloseSuperShiled4.Checked := g_Config.UseSkillCloseSuperShileds[4];

  seUseSkillCloseSuperShiled0_Rate.Value := g_Config.UseSkillCloseSuperShileds_Rate[0];
  seUseSkillCloseSuperShiled1_Rate.Value := g_Config.UseSkillCloseSuperShileds_Rate[1];
  seUseSkillCloseSuperShiled2_Rate.Value := g_Config.UseSkillCloseSuperShileds_Rate[2];
  seUseSkillCloseSuperShiled3_Rate.Value := g_Config.UseSkillCloseSuperShileds_Rate[3];
  seUseSkillCloseSuperShiled4_Rate.Value := g_Config.UseSkillCloseSuperShileds_Rate[4];

  seUseSkillCloseSuperShiled0_RateAdd.Value := g_Config.UseSkillCloseSuperShileds_RateAdd[0];
  seUseSkillCloseSuperShiled1_RateAdd.Value := g_Config.UseSkillCloseSuperShileds_RateAdd[1];
  seUseSkillCloseSuperShiled2_RateAdd.Value := g_Config.UseSkillCloseSuperShileds_RateAdd[2];
  seUseSkillCloseSuperShiled3_RateAdd.Value := g_Config.UseSkillCloseSuperShileds_RateAdd[3];
  seUseSkillCloseSuperShiled4_RateAdd.Value := g_Config.UseSkillCloseSuperShileds_RateAdd[4];


  // 刺杀忽视物理防御 piaoyun 2013-08-18
  // chkSwordLongNotDefence.Checked := g_Config.boSwordLongNotDefence;

  chkRecallManySlave1.Checked := g_Config.boRecallManySlave1;
  chkRecallManySlave2.Checked := g_Config.boRecallManySlave2;
  chkRecallManySlave3.Checked := g_Config.boRecallManySlave3;
  seRecallMonCount.Value := g_Config.dwRecallMonCount;

  CheckBoxOpenMapEvent.Checked := g_Config.boOpenMapEvent;
  CheckBoxDropOverLapItem.Checked := g_Config.boDropOverLapItem;

  EditMaxMyShopSellingItemCount.Value := g_Config.nMaxMyShopSellingItemCount;
  EditMaxMyShopStorageItemCount.Value := g_Config.nMaxMyShopStorageItemCount;
  EditSlavePowerRate.Value := g_Config.nSlavePowerRate;

  EditMagicLockRange.Value := g_Config.nMagicLockRange;
  edtHumSkill7PowerLV4.Value := g_Config.dwHumSkill7PowerLV4;
  edtHumSkill13PowerLV4.Value := g_Config.dwHumSkill13PowerLV4;
  edtHumSkill45PowerLV4.Value := g_Config.dwHumSkill45PowerLV4;

  seMagicFailMsgFColor.Value := g_Config.btMagicFailMsgFColor;
  seMagicFailMsgBColor.Value := g_Config.btMagicFailMsgBColor;
  seMagicOKMsgFColor.Value := g_Config.btMagicOKMsgFColor;
  seMagicOKMsgBColor.Value := g_Config.btMagicOKMsgBColor;
  seMagicMsgX.Value := g_Config.nMagicMsgX;
  seMagicMsgY.Value := g_Config.nMagicMsgY;
  chkMagicMsgXRightToLeft.Checked := g_Config.boMagicMsgXRightToLeft;
  chkMagicMsgYBottomToTop.Checked := g_Config.boMagicMsgYBottomToTop;
  chkMagicMsgAddChatBoardMsg.Checked := g_Config.boMagicMsgAddChatBoardMsg;

  chkDisableWarrContinueHit.Checked := g_Config.boDisableWarrContinueHit;
  edtDisableWarrContinueHitIDs.Text := g_Config.sDisableWarrContinueHitIDs;
  seWarrContinueHitMinInterval.Value := g_Config.nWarrContinueHitMinInterval;

  CheckBoxKillByMonstDropHeroUseItem.Checked := g_Config.boKillByMonstDropHeroUseItem;
  CheckBoxKillByHumanDropHeroUseItem.Checked := g_Config.boKillByHumanDropHeroUseItem;
  CheckBoxDieScatterHeroBag.Checked := g_Config.boDieScatterHeroBag;
  CheckBoxDieRedScatterHeroBagAll.Checked := g_Config.boDieRedScatterHeroBagAll;

  chkKillByMonstDropHeroJewelryBoxItem.Checked := g_Config.boKillByMonstDropHeroJewelryBoxItem;
  chkKillByHumanDropHeroJewelryBoxItem.Checked := g_Config.boKillByHumanDropHeroJewelryBoxItem;

  chkKillByMonstDropHeroGodBlessItem.Checked := g_Config.boKillByMonstDropHeroGodBlessItem;
  chkKillByHumanDropHeroGodBlessItem.Checked := g_Config.boKillByHumanDropHeroGodBlessItem;

  ScrollBarDieDropHeroUseItemRate.Min := 1;
  ScrollBarDieDropHeroUseItemRate.Max := 200;
  ScrollBarDieDropHeroUseItemRate.Position := g_Config.nDieDropHeroUseItemRate;
  ScrollBarDieRedDropHeroUseItemRate.Min := 1;
  ScrollBarDieRedDropHeroUseItemRate.Max := 200;
  ScrollBarDieRedDropHeroUseItemRate.Position := g_Config.nDieRedDropHeroUseItemRate;
  ScrollBarDieScatterHeroBagRate.Min := 1;
  ScrollBarDieScatterHeroBagRate.Max := 200;
  ScrollBarDieScatterHeroBagRate.Position := g_Config.nDieScatterHeroBagRate;

  scrlbrHeroJewelryBoxItem.Min := 1;
  scrlbrHeroJewelryBoxItem.Max := 200;
  scrlbrHeroJewelryBoxItem.Position := g_Config.nDropHeroJewelryBoxItemRate;

  scrlbrHeroGodBlessItem.Min := 1;
  scrlbrHeroGodBlessItem.Max := 200;
  scrlbrHeroGodBlessItem.Position := g_Config.nDropHeroGodBlessItemRate;

  EditMysteriousManName.Text := g_Config.sMysteriousManName;
  chkShowMysteriousMan.Checked := g_Config.boShowMysteriousMan;

  RefReNewLevelConf();
  RefMonUpgrade();
  RefSpiritMutiny();
  RefMonSayMsg();
  RefWeaponMakeLuck();

  CheckBoxDeleteItemDuraZero.Checked := g_Config.boDeleteItemDuraZero;

  chkDogzGotoMaster.Checked := g_Config.boDogzGotoMaster;
  CheckBoxBigDogzGotoMaster.Checked := g_Config.boBigDogzGotoMaster;
  CheckBoxMonthSpiritGotoMaster.Checked := g_Config.boMonthSpiritGotoMaster;
  chkDogzPlugSettingPriority.Checked := g_Config.boDogzPlugSettingPriority;
  chkBonePlugSettingPriority.Checked := g_Config.boBonePlugSettingPriority;

  EditUseContinuousMagicTime.Value := g_Config.nUseContinuousMagicTime;

  edtNGLevelPowerAdd_Level.Value := g_Config.nNGLevelPowerAdd_Level;
  edtNGLevelPowerAdd_Power.Value := g_Config.nNGLevelPowerAdd_Power;

  edtNGLevelPowerDec_Level.Value := g_Config.nNGLevelPowerDec_Level;
  edtNGLevelPowerDec_Power.Value := g_Config.nNGLevelPowerDec_Power;

  EditNGKillMonExpMultiple.Value := g_Config.nNGKillMonExpMultiple;
  EditNGHitStruckDecNG.Value := g_Config.nNGHitStruckDecNG;
  EditNGDrinkIncExp.Value := g_Config.nNGDrinkIncExp;
  EditNGSkillPowerRate.Value := g_Config.nNGSkillPowerRate;
  EditNGIncTime.Value := g_Config.nNGIncTime;
  EditNGHeroLevelExpValue.Value := g_Config.nNGHeroLevelExpValue;
  EditNGLevelExpValue.Value := g_Config.nNGLevelExpValue;
  EditNGLevelValue.Value := g_Config.nNGLevelValue;
  seNGMaxLevelLimte.Value := g_Config.nNGMaxLevelLimte;

  seSkill114HitWaitTime.Value := g_Config.nSkill114HitWaitTime;
  seSkill114PowerRate.Value := g_Config.nSkill114PowerRate;
  seSkill114LevelUpAddPowerRate.Value := g_Config.nSkill114LevelUpAddPowerRate;
  seSkill114AttackRange.Value := g_Config.nSkill114AttackRange;
  seHeroSkill114HitWaitTime.Value := g_Config.nHeroSkill114HitWaitTime;

  chkSpiritualismLevelDiff.Checked := g_Config.boSpiritualismLevelDiff;
  seSpiritualismLevelDiff.Value := g_Config.nSpiritualismLevelDiff;
  seSpiritualismRoyaltyTime.Value := g_Config.nSpiritualismRoyaltyTime;
  seSpiritualismBBCount.Value := g_Config.nSpiritualismBBCount;
  cbbSpiritualismMagicLevel.ItemIndex := 0;
  seSpiritualismRate.Value := g_Config.nSpiritualismRates[0];
  chkSpiritualismDisableUndeadMon.Checked := g_Config.boSpiritualismDisableUndeadMon;

  seDoMotaeboCD.Value := g_Config.nDoMotaeboCD;
  EditDamageItemDuraRate.Value := g_Config.nDamageItemDuraRate;
  chkBarbaricSeptum.Checked := g_Config.boBarbaricSeptum;

  CheckBoxOfflineCloseMyShop.Checked := g_Config.boOfflineCloseMyShop;
  chkProhibitModifyPrices.Checked := g_Config.boProhibitModifyPrices; // Cursor 2023-05-30 09:14:04

  // 烈火剑法 piaoyun 2013-07-25
  chkEnableDoubleFireHitSkill.Checked := g_Config.boEnableDoubleFireHitSkill;

  chkEnableDoubleFireHitDelayClose.Checked := g_Config.boEnableDoubleFireHitDelayClose;
  cbbDoubleFireHitDelayCloseType.ItemIndex := g_Config.nDoubleFireHitDelayCloseType;
  seDoubleFireHitDelayCloseValue.Value := g_Config.nDoubleFireHitDelayCloseValue;

  chkEnableDoubleFireHitDelayClose.Enabled := chkEnableDoubleFireHitSkill.Checked;
  cbbDoubleFireHitDelayCloseType.Enabled := chkEnableDoubleFireHitSkill.Checked;
  seDoubleFireHitDelayCloseValue.Enabled := chkEnableDoubleFireHitSkill.Checked;

  chkCloseFireHitSkillFailHint.Checked := g_Config.boCloseFireHitSkillFailHint;

  // 是否开启摆摊 piaoyun 2013-07-21
  chkOpenSelfShop.Checked := g_Config.boOpenSelfShop;
  // 只允许安全区摆摊 piaoyun 2013-07-21
  chkSafeZoneShop.Checked := g_Config.boSafeZoneShop;
  // 只允许指定地图摆摊 piaoyun 2013-07-21
  chkMapShop.Checked := g_Config.boMapShop;
  // 摆摊期间无敌模式 piaoyun 2013-08-17
  chkShopStallCanNotAttack.Checked := g_Config.boShopStallCanNotAttack;
  // 摆摊显示头顶个人商店图片 piaoyun 2013-09-13
  chkShopHeadPic.Checked := g_Config.boShopHeadPic;

  // 金币税收 piaoyun 2013-07-21
  seSellOffGoldTaxRate.Value := g_Config.dwSellOffGoldTaxRate;
  // 元宝税收 piaoyun 2013-07-21
  seSellOffGameGoldTaxRate.Value := g_Config.dwSellOffGameGoldTaxRate;

  seSellOffGameDiamondTaxRate.Value := g_Config.dwSellOffGameDiamondTaxRate;

  seSellOffGameGirdTaxRate.Value := g_Config.dwSellOffGameGirdTaxRate;

  seSellOffGamePointTaxRate.Value := g_Config.dwSellOffGamePointTaxRate;

  // 寄售系统扣税 chongchong 2014-01-06
  seJSOfGameGoldTaxRate.Value := g_Config.dwJSOfGameGoldTaxRate;

  chkMySellShopItemTime.Checked := g_Config.boEnabledMySellShopItemTime;
  seMySellShopItemTime.Value := g_Config.nMySellShopItemTime;
  seMyShopOperateInterval.Value := g_Config.nMyShopOperateInterval;
  seMySellShowItemNamLen.Value := g_Config.nMySellShowItemNamLen;

  chkInfinityStorage.Checked := g_Config.boInfinityStorage;
  edtInfinityStorageCount.Value := g_Config.nInfinityStorageCount;
  edtInfinityStorageCount.Enabled := chkInfinityStorage.Checked;
  if chkInfinityStorage.Checked then
    // 仓库默认容量可存44件物品,扩展后可存< > 件物品
    lblInfinityStorageCount.Caption := '无限仓库可存放 ' + IntToStr(g_Config.nInfinityStorageCount) + ' 件物品'
  else
    // lblInfinityStorageCount.Caption := '默认容量为' + IntToStr(MAXSTOREITEM); // + '扩展后为：' + IntToStr(MAXSTOREITEM + g_Config.nInfinityStorageCount);
    lblInfinityStorageCount.Caption := '默认可存 0 件物品';

  CheckBoxMasterRoyaltyDie.Checked := g_Config.boMasterRoyaltyDie;
  chkMasterRoyaltyFullHP.Checked := g_Config.boMasterRoyaltyFullHP;
  CheckBoxSlaveRelaxCanStruck.Checked := g_Config.boSlaveRelaxCanStruck;
  EditMaxLuckMaxPower.Value := g_Config.nMaxLuckMaxPower;
  chkLuckUseNewAlgorism.Checked := g_Config.boLuckUseNewAlgorism;

  chkSlaveNotAttackHuman.Checked := g_Config.boSlaveNotAttackHuman;
  chkSlaveNotAttackHero.Checked := g_Config.boSlaveNotAttackHero;
  chkSlaveLockTarget.Checked := g_Config.boSlaveLockTarget;
  chkSlaveNoLockHuman.Checked := g_Config.boSlaveNoLockHuman;

  seSlave9HP.Value := g_Config.nSlave9HP;
  seSlave9AC.Value := g_Config.nSlave9AC;
  seSlave9MAC.Value := g_Config.nSlave9MAC;
  seSlave9DC.Value := g_Config.nSlave9DC;
  seSlave9MoveSpeed.Value := g_Config.nSlave9MoveSpeed;
  seSlave9HitSpeed.Value := g_Config.nSlave9HitSpeed;

  seLimitScriptGotoCount.Value := g_Config.nLimitScriptGotoCount;
  chkM2CacheRankData.Checked := g_Config.boM2CacheRankData;

  // 虹魔，吸血排除NPC piaoyun 2013-07-24
  chkHongMoSuiteWithPower.Checked := g_Config.boHongMoSuiteWithPower;

  // 禁止记忆传送安全区人物 piaoyun 2013-07-24
  chkGroupReCallNotInSafeZone.Checked := g_Config.boGroupReCallNotInSafeZone;
  // 新人和平攻击模式 piaoyun 2013-07-24
  chkNewHumanAttatckMode_HAM_PEACE.Checked := g_Config.boNewHumanAttatckMode_HAM_PEACE;
  // 自动替换组长 piaoyun 2013-07-24
  chkAutoGroupMaster.Checked := g_Config.boAutoGroupMaster;
  // 大刀不攻击人形怪、分身 2013-07-24
  chkGuardNotAttackPlayMoster.Checked := g_Config.boGuardNotAttackPlayMoster;
  // 安全区域不掉装备 piaoyun 2013-07-25
  chkWarNoDropUseItem.Checked := g_Config.boWarNoDropUseItem;

  chkGroupUseOldMode.Checked := g_Config.boGroupUseOldMode;

  // 吸血武器吸血倍率 piaoyun 2013-07-24
  seHongMoSuiteRate.Value := g_Config.nHongMoSuiteRateChange;

  chkJewelryCalcBasicAbilitys.Checked := g_Config.boJewelryCalcBasicAbilitys;
  chkJewelryCalcGroupAbilitys.Checked := g_Config.boJewelryCalcGroupAbilitys;
  chkJewelryDecDura.Checked := g_Config.boJewelryDecDura;
  edtJewelryBoxHint.Text := g_Config.sJewelryBoxHint;

  seRevivalTime.Value := g_Config.dwRevivalTime div 1000;
  chkRevivalTouch.Checked := g_Config.boRevivalTouch;
  chkSaveRevivalTime.Checked := g_Config.boSaveRevivalTime;

  chkDisableMoveParalysisHuman.Checked := g_Config.boDisableMoveParalysisHuman;

  chkFBExitCreaterOffline.Checked := g_Config.boFBExitCreaterOffline;
  chkFBDisableDelay30s.Checked := g_Config.boFBDisableDelay30s;

  CheckBoxDuraChangeLight.Checked := g_Config.boDuraChangeLight;
  boDropOverLapItem := g_Config.boDropOverLapItem;

  chkOpenNewGuild.Checked := g_Config.boOpenNewGuildTemp;
  chkNoShowNewGuildHumanCount.Checked := g_Config.boNoShowNewGuildHumanCount;

  chkRecordBeadExp.Checked := g_Config.boRecordBeadExp;

  chkCloseNPCNoItemMsg.Checked := g_Config.boCloseNPCNoItemMsg;

  // 野蛮冲撞 piaoyun 2013-07-25
  chkShowDoMotaeboMsg.Checked := g_Config.boShowDoMotaeboMsg;
  chkDoMotaeboPushSameLevel.Checked := g_Config.boDoMotaeboPushSameLevel;
  chkShowDoMotaeboMsg.Checked := g_Config.boShowDoMotaeboMsg;

  CheckBoxPoisonWeaponCanMagicAttack.Checked := g_Config.boPoisonWeaponCanMagicAttack;
  CheckBoxPoisonWeaponCanHitAllTarget.Checked := g_Config.boPoisonWeaponCanHitAllTarget;

  EditQueryBagItemsTime.Value := g_Config.nQueryBagItemsTime;
  chkShowRefreshBagMsg.Checked := g_Config.boShowRefreshBagMsg;

  if g_Config.boUseHeroM2Shop then
    RadioGroupShopType.ItemIndex := 1
  else
    RadioGroupShopType.ItemIndex := 0;

  chkMyShopGold.Checked := g_Config.boMyShopGold;
  chkMyShopGameGold.Checked := g_Config.boMyShopGameGold;
  chkMyShopGameDiamond.Checked := g_Config.boMyShopGameDiamond;
  chkMyShopGameGird.Checked := g_Config.boMyShopGameGird;
  chkMyShopGamePoint.Checked := g_Config.boMyShopGamePoint;

  chkAuctionCurrencyType1.Checked := g_Config.nAuctionCurrencyTypeEx and 1 <> 0;
  chkAuctionCurrencyType2.Checked := g_Config.nAuctionCurrencyTypeEx and 2 <> 0;
  chkAuctionCurrencyType3.Checked := g_Config.nAuctionCurrencyTypeEx and 4 <> 0;
  chkAuctionCurrencyType4.Checked := g_Config.nAuctionCurrencyTypeEx and 8 <> 0;
  chkAuctionCurrencyType5.Checked := g_Config.nAuctionCurrencyTypeEx and 16 <> 0;

  seAuctionGoldTaxRate.Value := g_Config.dwAuctionGoldTaxRate;
  seAuctionGameGoldTaxRate.Value := g_Config.dwAuctionGameGoldTaxRate;
  seAuctionGameDiamondTaxRate.Value := g_Config.dwAuctionGameDiamondTaxRate;
  seAuctionGameGirdTaxRate.Value := g_Config.dwAuctionGameGirdTaxRate;
  seAuctionGamePointTaxRate.Value := g_Config.dwAuctionGamePointTaxRate;

  cbbAuctionBroadcastCurrencyType.Clear;
  cbbAuctionBroadcastCurrencyType.Items.Add(g_Config.sGameGoldName); // 元宝
  cbbAuctionBroadcastCurrencyType.Items.Add(g_Config.sGamePointName); // 游戏点
  cbbAuctionBroadcastCurrencyType.Items.Add(sSTRING_GOLDNAME); // 金币
  cbbAuctionBroadcastCurrencyType.Items.Add(g_Config.sGameDiamondName); // 金刚石
  cbbAuctionBroadcastCurrencyType.Items.Add(g_Config.sGameGirdName); // 灵符

  cbbAuctionBroadcastCurrencyType.ItemIndex := g_Config.nAuctionBroadcastCurrencyType;
  seAuctionBroadcastPrice.Value := g_Config.nAuctionBroadcastPrice;
  seAuctionBroadcastShowTime.Value := g_Config.nAuctionBroadcastShowTime;
  edtAuctionBroadcastText.Text := g_Config.sAuctionBroadcastText;

  seAuctioningItemsCount.Value := g_Config.nAuctioningItemsCount;
  chkOpenAuctionItemColors.Checked := g_Config.boOpenAuctionItemColors;
  seAuctionItemColor1.Value := g_Config.btAuctionItemColors[0];
  seAuctionItemColor2.Value := g_Config.btAuctionItemColors[1];
  seAuctionItemColor3.Value := g_Config.btAuctionItemColors[2];
  seAuctionItemColor4.Value := g_Config.btAuctionItemColors[3];
  seAuctionItemColor5.Value := g_Config.btAuctionItemColors[4];
  seAuctionItemColor6.Value := g_Config.btAuctionItemColors[5];

  chkSellPlayerCurrencyType1.Checked := g_Config.nSellPlayerCurrencyTypeEx and 1 <> 0;
  chkSellPlayerCurrencyType2.Checked := g_Config.nSellPlayerCurrencyTypeEx and 2 <> 0;
  chkSellPlayerCurrencyType3.Checked := g_Config.nSellPlayerCurrencyTypeEx and 4 <> 0;
  chkSellPlayerCurrencyType4.Checked := g_Config.nSellPlayerCurrencyTypeEx and 8 <> 0;
  chkSellPlayerCurrencyType5.Checked := g_Config.nSellPlayerCurrencyTypeEx and 16 <> 0;

  seSellPlayerTime.Value := g_Config.nSellPlayerTime;
  seSellPlayerGoldTaxRate.Value := g_Config.dwSellPlayerGoldTaxRate;
  seSellPlayerGameGoldTaxRate.Value := g_Config.dwSellPlayerGameGoldTaxRate;
  seSellPlayerGameDiamondTaxRate.Value := g_Config.dwSellPlayerGameDiamondTaxRate;
  seSellPlayerGameGirdTaxRate.Value := g_Config.dwSellPlayerGameGirdTaxRate;
  seSellPlayerGamePointTaxRate.Value := g_Config.dwSellPlayerGamePointTaxRate;

  chkSellPlayerViewStorage.Checked := g_Config.boSellPlayerViewStorage;
  chkSellPlayerViewStorageEx.Checked := g_Config.boSellPlayerViewStorageEx;
  chkSellPlayerAutoRecallHero.Checked := g_Config.boSellPlayerAutoRecallHero;

  seSellPlayerViewOtherInfoTextOffsetX.Value := g_Config.nSellPlayerViewOtherInfoTextOffsetX;
  seSellPlayerViewOtherInfoTextOffsetY.Value := g_Config.nSellPlayerViewOtherInfoTextOffsetY;

  { 属性消耗点控制 chongchong 2013-07-26 }
  seBonusAbilofWarrDC.Value := g_Config.BonusAbilofWarr.DC;
  seBonusAbilofWarrMC.Value := g_Config.BonusAbilofWarr.MC;
  seBonusAbilofWarrSC.Value := g_Config.BonusAbilofWarr.SC;
  seBonusAbilofWarrAC.Value := g_Config.BonusAbilofWarr.AC;
  seBonusAbilofWarrMAC.Value := g_Config.BonusAbilofWarr.MAC;
  seBonusAbilofWarrHP.Value := g_Config.BonusAbilofWarr.HP;
  seBonusAbilofWarrMP.Value := g_Config.BonusAbilofWarr.MP;
  seBonusAbilofWarrHit.Value := g_Config.BonusAbilofWarr.Hit;
  seBonusAbilofWarrSpeed.Value := g_Config.BonusAbilofWarr.Speed;
  seBonusAbilofWizardDC.Value := g_Config.BonusAbilofWizard.DC;
  seBonusAbilofWizardMC.Value := g_Config.BonusAbilofWizard.MC;
  seBonusAbilofWizardSC.Value := g_Config.BonusAbilofWizard.SC;
  seBonusAbilofWizardAC.Value := g_Config.BonusAbilofWizard.AC;
  seBonusAbilofWizardMAC.Value := g_Config.BonusAbilofWizard.MAC;
  seBonusAbilofWizardHP.Value := g_Config.BonusAbilofWizard.HP;
  seBonusAbilofWizardMP.Value := g_Config.BonusAbilofWizard.MP;
  seBonusAbilofWizardHit.Value := g_Config.BonusAbilofWizard.Hit;
  seBonusAbilofWizardSpeed.Value := g_Config.BonusAbilofWizard.Speed;
  seBonusAbilofTaosDC.Value := g_Config.BonusAbilofTaos.DC;
  seBonusAbilofTaosMC.Value := g_Config.BonusAbilofTaos.MC;
  seBonusAbilofTaosSC.Value := g_Config.BonusAbilofTaos.SC;
  seBonusAbilofTaosAC.Value := g_Config.BonusAbilofTaos.AC;
  seBonusAbilofTaosMAC.Value := g_Config.BonusAbilofTaos.MAC;
  seBonusAbilofTaosHP.Value := g_Config.BonusAbilofTaos.HP;
  seBonusAbilofTaosMP.Value := g_Config.BonusAbilofTaos.MP;
  seBonusAbilofTaosHit.Value := g_Config.BonusAbilofTaos.Hit;
  seBonusAbilofTaosSpeed.Value := g_Config.BonusAbilofTaos.Speed;

  chkHeroCanUseMootebo.Checked := g_Config.boHeroCanUseMootebo;
  chkHeroCallBB.Checked := g_Config.boHeroCanCallBB;
  seHeroCallBBCount.Value := g_Config.dwHeroCallBBCount;
  chkHeroNoTargetRecallBB.Checked := g_Config.boHeroNoTargetRecallBB;

  chkHeroJointAttack.Checked := g_Config.boHeroJointAttack;
  chkHeroJointAttackFly.Checked := g_Config.boHeroJointAttackFly;
  seAngryAgainValue.Value := g_Config.dwAngryAgainValue;

  seHeroDieExpRate.Value := g_Config.dwHeroDieExpRate;

  seHeroMasterStartLevel.Value := g_Config.dwHeroMasterStartLevel;
  seHeroSlaveStartLevel.Value := g_Config.dwHeroSlaveStartLevel;

  chkHeroDisableSafeZoneProtect.Checked := g_Config.boHeroDisableSafeZoneProtect;
  // 禁止安全区守护 chongchong 2013-08-11
  chkHeroNoMoveOnSleep.Checked := g_Config.boHeroNoMoveOnSleep;
  // 休息时不随主人移动 chongchong 2013-08-11

    // chkHeroNoSkillUseBaseAttack.Checked := g_Config.boHeroNoSkillUseBaseAttack;                       // 道法无技能时使用物理攻击 chongchong 2013-08-11
  chkHero700HPUseBaseAttack.Checked := g_Config.boHero700HPUseBaseAttack;
  seHero700HPValue.Value := g_Config.dwHero700HPValue;
  seHeroWarrHPMPRate.Value := g_Config.dwHeroWarrHPMPRate;
  // 战士英雄HP/MP倍数 chongchong 2013-08-11
  seHeroWizardHPMPRate.Value := g_Config.dwHeroWizardHPMPRate;
  // 法师英雄HP/MP倍数 chongchong 2013-08-11
  seHeroTaosHPMPRate.Value := g_Config.dwHeroTaosHPMPRate;
  // 道士英雄HP/MP倍数 chongchong 2013-08-11
  chkCreditPointWithLevel.Checked := g_Config.boCreditPointWithLevel; // 声望按等级计算
  chkHeroCalcWeaponSpeed.Checked := g_Config.boHeroCalcWeaponSpeed;
  seHeroGotoLV4.Value := g_Config.dwHeroGotoLV4;
  seHeroPowerLV4.Value := g_Config.dwHeroPowerLV4;
  seHeroFealtyCallAdd.Value := g_Config.dwHeroFealtyCallAdd;
  seHeroFealtyCallBackDel.Value := g_Config.dwHeroFealtyCallBackDel;
  seHeroFealtyExp.Value := g_Config.dwHeroFealtyExp;
  seHeroFealtyExpAdd.Value := g_Config.dwHeroFealtyExpAdd;
  seHeroFealtyDeathDel.Value := g_Config.dwHeroFealtyDeathDel;
  chkHeroTaosAutoChangePoison.Checked := g_Config.boHeroTaosAutoChangePoison;
  seHeroTaoUsePoisonMinHP.Value := g_Config.dwHeroTaoUsePoisonMinHP;
  chkHeroFollowMasterWithDiffScreen.Checked := g_Config.boHeroFollowMasterWithDiffScreen;
  chkHeroAutoSuperShiled.Checked := g_Config.boHeroAutoSuperShiled;

  chkHeroDFAvoidTargetRight.Checked := g_Config.boHeroDFAvoidTargetRight;

  // 魔法忽略障碍 piaoyun 2013-09-12
  chkMagicNotHinder.Checked := g_Config.boMagicNotHinder;
  // 提高魔法精确度 piaoyun 2013-09-12
  chkMagicDefinition.Checked := g_Config.boMagicDefinition;

  GridLevelExp.RowCount := Length(g_Config.dwHeroNeedExps) + 1;
  GridLevelExp.Cells[0, 0] := '等级';
  GridLevelExp.Cells[1, 0] := '经验值';
  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    GridLevelExp.Cells[0, I] := IntToStr(I);
    GridLevelExp.Cells[1, I] := IntToStr(g_Config.dwHeroNeedExps[I]);
  end;

  seHeroHighLevel.Value := g_Config.dwHeroHighLevel;
  seHeroHighLevelGetExp.Value := g_Config.dwHeroHighLevelGetExp;
  seHeroLevel1000FixedExp.Value := g_Config.dwHeroLevel1000FixedExp;

  chkHeroDisableStruck.Checked := g_Config.boHeroDisableStruck;
  chkHeroDisableSelfStruck.Checked := g_Config.boHeroDisableSelfStruck;

  chkKillHeroWeaponUnlock.Checked := g_Config.boKillHeroWeaponUnlock;
  seKillHeroWeaponUnlockRate.Value := g_Config.dwKillHeroWeaponUnlockRate;
  chkHeroGetAllExp.Checked := g_Config.boHeroGetAllExp;
  chkHumanGetAllExp.Checked := g_Config.boHumanGetAllExp;
  if chkHumanGetAllExp.Checked then
  begin
    chkHeroGetAllExp.Checked := False;
    g_Config.boHeroGetAllExp := False;
  end;
  seHeroKillMonExpRate.Enabled := not chkHeroGetAllExp.Checked;

  cbbHeroWarriorDefaultSkill.Clear;
  cbbHeroWarriorDefaultSkill.AddItem('普通攻击', TObject(0));
  cbbHeroWarriorDefaultSkill.AddItem('刀刀刺杀', TObject(SKILL_ERGUM));
  cbbHeroWarriorDefaultSkill.AddItem('半月弯刀', TObject(SKILL_BANWOL));

  for I := 0 to UserEngine.m_MagicList.Count - 1 do
  begin
    Magic := UserEngine.m_MagicList.Items[I];

    if CheckIsCustomMagic(Magic.wMagicId) and (Magic.MagicAttr = mtHero) then
    begin
      CustomMagicConfig := GetCustomMagicConfig(Magic.wMagicId);
      if (CustomMagicConfig <> nil) and (CustomMagicConfig.IsMagicWarr) then
      begin
        cbbHeroWarriorDefaultSkill.AddItem(Magic.sMagicName, TObject(Magic.wMagicId));
      end;
    end;
  end;

  cbbHeroWarriorDefaultSkill.ItemIndex := 0;
  for I := 0 to cbbHeroWarriorDefaultSkill.Items.Count - 1 do
  begin
    MagicID := Integer(cbbHeroWarriorDefaultSkill.Items.Objects[I]);
    if MagicID = g_Config.dwHeroWarriorDefaultSkill then
    begin
      cbbHeroWarriorDefaultSkill.ItemIndex := I;
      Break;
    end;
  end;

  g_Config.dwHeroWarriorDefaultSkill := Integer(cbbHeroWarriorDefaultSkill.Items.Objects[cbbHeroWarriorDefaultSkill.ItemIndex]);

  seKillHeroWeaponUnlockRate.Enabled := chkKillHeroWeaponUnlock.Checked;
  seHeroLimit.Value := g_dwHeroLimit;
  seHeroRunTime.Value := g_Config.dwHeroRunTime;
  seHeroWarrAttackMoveRate.Value := g_Config.dwHeroWarrAttackMoveRate;
  chkHeroTargetAgainNoMove.Checked := g_Config.boHeroTargetAgainNoMove;

  seHeroLogonTimeMasterDie.Value := g_Config.dwHeroLogonTimeMasterDie;

  chkHeroStateDlgNoMove.Checked := g_Config.boHeroStateDlgNoMove;

  chkHeroForcePeaceMode.Checked := g_Config.boHeroForcePeaceMode;
  seHeroAttackHumPowerRate.Value := g_Config.nHeroAttackHumPowerRate;
  seHeroAttackMonPowerRate.Value := g_Config.nHeroAttackMonPowerRate;
  chkHeroStatus0.Checked := g_Config.boHeroStatus[0];
  chkHeroStatus1.Checked := g_Config.boHeroStatus[1];
  chkHeroStatus2.Checked := g_Config.boHeroStatus[2];
  chkHeroStatus3.Checked := g_Config.boHeroStatus[3];

  seMonAttackHeroPowerRate.Value := g_Config.nMonAttackHeroPowerRate;
  seHumanAttackHeroPowerRate.Value := g_Config.nHumanAttackHeroPowerRate;

  chkDisHeroRun.Checked := not g_Config.boDiableHeroRun;

  if not chkDisHeroRun.Checked then
  begin
    chkHeroRunHum.Checked := False;
    chkHeroRunHum.Enabled := False;

    chkHeroRunMon.Checked := False;
    chkHeroRunMon.Enabled := False;

    chkHeroRunNpc.Checked := False;
    chkHeroRunNpc.Enabled := False;

    chkHeroRunGuard.Checked := False;
    chkHeroRunGuard.Enabled := False;

    // chkHeroWarDisHumRun.Checked := False;
    // chkHeroWarDisHumRun.Enabled := False;

    // chkHeroWarHreoRun.Checked := False;
    // chkHeroWarHreoRun.Enabled := False;

    chkHeroSafeArea.Checked := False;
    chkHeroSafeArea.Enabled := False;

    chkHeroSafeAreaDisNpcRun.Checked := False;
    chkHeroSafeAreaDisNpcRun.Enabled := False;

    chkSafeAreaDisShopStallHeroRun.Checked := False;
    chkSafeAreaDisShopStallHeroRun.Enabled := False;

    chkSafeAreaDisOffLineHeroRun.Enabled := False;
    chkSafeAreaDisOffLineHeroRun.Checked := False;

    g_Config.boHeroRunHum := False;
    g_Config.boHeroRunMon := False;
    g_Config.boHeroRunNpc := False;
    g_Config.boHeroRunGuard := False;

    // g_Config.boHeroWarDisHumRun := False;
    // g_Config.boHeroWarHreoRun := False;

    g_Config.boHeroSafeAreaLimited := False;
    g_Config.boHeroSafeAreaDisNpcRun := False;
    g_Config.boSafeAreaDisShopStallHeroRun := False;
    g_Config.boSafeAreaDisOffLineHeroRun := False;
  end
  else
  begin
    chkHeroRunHum.Checked := g_Config.boHeroRunHum;
    chkHeroRunMon.Checked := g_Config.boHeroRunMon;
    chkHeroRunNpc.Checked := g_Config.boHeroRunNpc;
    chkHeroRunGuard.Checked := g_Config.boHeroRunGuard;
    chkHeroWarDisHumRun.Checked := g_Config.boHeroWarDisHumRun;
    chkHeroWarHreoRun.Checked := g_Config.boHeroWarHreoRun;
    chkHeroSafeArea.Checked := g_Config.boHeroSafeAreaLimited;
    chkHeroSafeAreaDisNpcRun.Checked := g_Config.boHeroSafeAreaDisNpcRun;
    chkSafeAreaDisShopStallHeroRun.Checked := g_Config.boSafeAreaDisShopStallHeroRun;
    chkSafeAreaDisOffLineHeroRun.Checked := g_Config.boSafeAreaDisOffLineHeroRun;
  end;

  chkDisableMonsterAttackHero.Checked := g_Config.boDisableMonsterAttackHero;
  chkDisableHeroAttackMonster.Checked := g_Config.boDisableHeroAttackMonster;

  chkHeroKillMonTrigger.Checked := g_Config.boHeroKillMonTrigger;
  seHeroTargetRangeLimit.Value := g_Config.nHeroTargetRangeLimit;

  seStarBaseNum.Value := g_Config.nStarBaseNum;
  seStarLineMaxCount.Value := g_Config.nStarLineMaxCount;
  chkDisableDuFuTakeArmRingL.Checked := g_Config.boDisableDuFuTakeArmRingL;

  chkHeroTargetAgainNoMoveDF.Enabled := g_Config.boHeroTargetAgainNoMove;
  chkHeroTargetAgainNoMoveDF.Checked := g_Config.boHeroTargetAgainNoMoveDF;

  seHeroWarrAttacSkillErgumRate.Value := g_Config.dwHeroWarrAttacSkillErgumRate;

  seHeroWarrAttakNear.Value := g_Config.dwHeroWarrAttakNear;
  seHeroWarrNearFireSword.Value := g_Config.dwHeroWarrNearFireSword;

  chkContinuousAttackUseNG.Checked := g_Config.boContinuousAttackUseNG;
  chkSkill114AttackUseNG.Checked := g_Config.boSkill114AttackUseNG;

  seContinuousProtect.Value := g_Config.btContinuousProtect;
  seContinuousProtectRandom.Value := g_Config.btContinuousProtectRandom;
  seWarrContinuousStatusLock100.Value := g_Config.btWarrContinuousStatusLocks[0];
  seWarrContinuousStatusLockTime100.Value := g_Config.nWarrContinuousStatusLockTimes[0];

  seWarrContinuousStatusLock101.Value := g_Config.btWarrContinuousStatusLocks[1];
  seWarrContinuousStatusLockTime101.Value := g_Config.nWarrContinuousStatusLockTimes[1];

  seWarrContinuousStatusLock102.Value := g_Config.btWarrContinuousStatusLocks[2];
  seWarrContinuousStatusLockTime102.Value := g_Config.nWarrContinuousStatusLockTimes[2];

  seWarrContinuousStatusLock103.Value := g_Config.btWarrContinuousStatusLocks[3];
  seWarrContinuousStatusLockTime103.Value := g_Config.nWarrContinuousStatusLockTimes[3];

  seSkill102AttackRange.Value := g_Config.nSkill102AttackRange;
  seSkill103AttackRange.Value := g_Config.nSkill103AttackRange;
  seSkill105AttackRange.Value := g_Config.nSkill105AttackRange;

  seSkill106AddFrozenRate.Value := g_Config.nSkill106AddFrozenRate;
  seSkill106AddFrozenRate2.Value := g_Config.nSkill106AddFrozenRate2;
  seSkill106AddFrozenTime.Value := g_Config.nSkill106AddFrozenTime;
  seSkill106AddFrozenTime2.Value := g_Config.nSkill106AddFrozenTime2;

  seSkill110PushedRate.Value := g_Config.nSkill110PushedRate;
  seSkill110PushedRate2.Value := g_Config.nSkill110PushedRate2;
  seSkill110PushedRange.Value := g_Config.nSkill110PushedRange;
  seSkill110PushedRange2.Value := g_Config.nSkill110PushedRange2;
  chkSkill110PushedHighLevel.Checked := g_Config.boSkill110PushedHighLevel;

  seContinuousAttackLevelRate.Value := g_Config.nContinuousAttackLevelRate;

  seSkillContinueOrderBlastRates1.Value := g_Config.SkillContinueOrderBlastRates[0];
  seSkillContinueOrderBlastRates2.Value := g_Config.SkillContinueOrderBlastRates[1];
  seSkillContinueOrderBlastRates3.Value := g_Config.SkillContinueOrderBlastRates[2];
  seSkillContinueOrderBlastRates4.Value := g_Config.SkillContinueOrderBlastRates[3];

  seRecallCopySelfWaitTime.Value := g_Config.dwRecallCopySelfWaitTime;
  // 召唤分身间隔 chongchong 2013-10-28
  seHeroRecallCopySelfHPRate.Value := g_Config.btHeroRecallCopySelfHPRate;

  chkCopyMonWarrorAttack.Checked := g_Config.boCopyMonWarrorAttack;
  chkCopyMon700HPUseBaseAttack.Checked := g_Config.boCopyMon700HPUseBaseAttack;
  chkCopyMonInheritedMasterSpeed.Checked := g_Config.boCopyMonInheritedMasterSpeed;

  seRecallDogzWaitTime.Value := g_Config.dwRecallDogzWaitTime;
  // 召唤神兽间隔 chongchong 2013-10-28
  seRecallBoneFammWaitTime.Value := g_Config.dwRecallBoneFammWaitTime;
  // 召唤骷髅间隔 chongchong 2013-10-28
  seRecallMonthSpiritWaitTime.Value := g_Config.dwRecallMonthSpiritWaitTime;
  // 召唤月灵间隔 chongchong 2013-10-28

  edtPlusBoneFammName1_3.Text := g_Config.sPlusBoneFammName1_3;
  edtPlusBoneFammName4_6.Text := g_Config.sPlusBoneFammName4_6;
  edtPlusBoneFammName7_9.Text := g_Config.sPlusBoneFammName7_9;
  edtPlusBoneFammName9_N.Text := g_Config.sPlusBoneFammName9_N;

  sePlusBoneFammLevel1.Value := g_Config.dwPlusBoneFammLevels[0];
  sePlusBoneFammLevel2.Value := g_Config.dwPlusBoneFammLevels[1];
  sePlusBoneFammLevel3.Value := g_Config.dwPlusBoneFammLevels[2];
  sePlusBoneFammLevel4.Value := g_Config.dwPlusBoneFammLevels[3];
  sePlusBoneFammLevel5.Value := g_Config.dwPlusBoneFammLevels[4];
  sePlusBoneFammLevel6.Value := g_Config.dwPlusBoneFammLevels[5];
  sePlusBoneFammLevel7.Value := g_Config.dwPlusBoneFammLevels[6];
  sePlusBoneFammLevel8.Value := g_Config.dwPlusBoneFammLevels[7];
  sePlusBoneFammLevel9.Value := g_Config.dwPlusBoneFammLevels[8];
  sePlusBoneFammLevelAfter9.Value := g_Config.dwPlusBoneFammAddLevelAfter9;

  edtPlusDogzName1_3.Text := g_Config.sPlusDogzName1_3;
  edtPlusDogzName4_6.Text := g_Config.sPlusDogzName4_6;
  edtPlusDogzName7_9.Text := g_Config.sPlusDogzName7_9;
  edtPlusDogzName9_N.Text := g_Config.sPlusDogzName9_N;

  sePlusDogzLevel1.Value := g_Config.dwPlusDogzLevels[0];
  sePlusDogzLevel2.Value := g_Config.dwPlusDogzLevels[1];
  sePlusDogzLevel3.Value := g_Config.dwPlusDogzLevels[2];
  sePlusDogzLevel4.Value := g_Config.dwPlusDogzLevels[3];
  sePlusDogzLevel5.Value := g_Config.dwPlusDogzLevels[4];
  sePlusDogzLevel6.Value := g_Config.dwPlusDogzLevels[5];
  sePlusDogzLevel7.Value := g_Config.dwPlusDogzLevels[6];
  sePlusDogzLevel8.Value := g_Config.dwPlusDogzLevels[7];
  sePlusDogzLevel9.Value := g_Config.dwPlusDogzLevels[8];
  sePlusDogzLevelAfter9.Value := g_Config.dwPlusDogzAddLevelAfter9;

  chkBBAttrPlusAddOnlyMagic.Checked := g_Config.boBBAttrPlusAddOnlyMagic;

  chkBBAttrPlusAddAttack.Checked := g_Config.boBBAttrPlusAddAttack;
  if (g_Config.nBBAttrPlusAddAttackForm < 0) or (g_Config.nBBAttrPlusAddAttackForm >= cbbBBAttrPlusAddAttackForm.Items.Count) then
    g_Config.nBBAttrPlusAddAttackForm := 0;
  cbbBBAttrPlusAddAttackForm.ItemIndex := g_Config.nBBAttrPlusAddAttackForm;
  seBBAttrPlusAddAttackRate.Value := g_Config.nBBAttrPlusAddAttackRate;

  chkBBAttrPlusAddDefence.Checked := g_Config.boBBAttrPlusAddDefence;
  chkBBAttrPlusAddMagicDefence.Checked := g_Config.boBBAttrPlusAddMagicDefence;
  seBBAttrPlusAddDefenceRate.Value := g_Config.nBBAttrPlusAddDefenceRate;

  chkBBAttrPlusAddHP.Checked := g_Config.boBBAttrPlusAddHP;
  seBBAttrPlusAddHPRate.Value := g_Config.nBBAttrPlusAddHPRate;
  chkSlaveKillHumanIncPK.Checked := g_Config.boSlaveKillHumanIncPK;
  chkSlaveDisableStruck.Checked := g_Config.boSlaveDisableStruck;
  chkSlaveAlwaysShowName.Checked := g_Config.boSlaveAlwaysShowName;

  chkSlaveLevelupUseNewAttr.Checked := g_Config.boSlaveLevelupUseNewAttr;
  chkSlaveLevelupAddLowerAttr.Checked := g_Config.boSlaveLevelupAddLowerAttr;

  seSlave9HP.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9AC.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9MAC.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9DC.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9MoveSpeed.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9HitSpeed.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  chkSlaveLevelupAddLowerAttr.Enabled := chkSlaveLevelupUseNewAttr.Checked;

  seHeroProtectFlyRange.Value := g_Config.nHeroJoinAttackFlyRange;
  seHeroLockFlyRange.Value := g_Config.nHeroLockFlyRange;
  seHeroProtectFlyRange.Value := g_Config.nHeroProtectFlyRange;

  boOpened := True;

  PageControlHeroConfig.ActivePageIndex := 0;

  PageControl7.ActivePageIndex := 0;
  PageControl9.ActivePageIndex := 0;
  PageControl10.ActivePageIndex := 0;
  PageControl11.ActivePageIndex := 0;
  PageControl12.ActivePageIndex := 0;

  pgcBonusAbilof.ActivePageIndex := 0;
  pgc1.ActivePageIndex := 0;

  lstMagicAC.Items.Clear;

  for I := 0 to UserEngine.m_MagicACList.Count - 1 do
  begin
    MagicACInfo := UserEngine.m_MagicACList.Items[I];
    lstMagicAC.Items.AddObject(MagicACInfo.sMagicName, TObject(MagicACInfo));
  end;

  FunctionConfigControl.ActivePageIndex := 1;
  FunctionConfigControl.ActivePage := TabSheetGeneral;

  chkMagicACEnabled.Enabled := False;
  seMagicACHum.Enabled := False;
  seMagicACMon.Enabled := False;
  seMagicACHero.Enabled := False;

  uModValue();

  ShowModal;
end;

procedure TfrmFunctionConfig.FormCreate(Sender: TObject);
var
  I: Integer;
begin
  GridBoneFamm.Cells[0, 0] := '人物等级';
  GridBoneFamm.Cells[1, 0] := '怪物名称';
  GridBoneFamm.Cells[2, 0] := '数量';
  GridBoneFamm.Cells[3, 0] := '等级';

  GridDogz.Cells[0, 0] := '人物等级';
  GridDogz.Cells[1, 0] := '怪物名称';
  GridDogz.Cells[2, 0] := '数量';
  GridDogz.Cells[3, 0] := '等级';

  GridBigDogz.Cells[0, 0] := '人物等级';
  GridBigDogz.Cells[1, 0] := '怪物名称';
  GridBigDogz.Cells[2, 0] := '数量';
  GridBigDogz.Cells[3, 0] := '等级';

  GridMonthSpirit.Cells[0, 0] := '人物等级';
  GridMonthSpirit.Cells[1, 0] := '怪物名称';
  GridMonthSpirit.Cells[2, 0] := '数量';
  GridMonthSpirit.Cells[3, 0] := '等级';

  GridMedicineExp.RowCount := Length(g_Config.dwMedicineLevelNeedExps) + 1;
  GridMedicineExp.Cells[0, 0] := '等级';
  GridMedicineExp.Cells[1, 0] := '药力值';
  for I := 1 to GridMedicineExp.RowCount - 1 do
  begin
    GridMedicineExp.Cells[0, I] := IntToStr(I);
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

  boSendServerConfig := False;

  MagicPageControl.ActivePageIndex := 0;
  PageControl4.ActivePageIndex := 0;
  PageControl5.ActivePageIndex := 0;
  PageControl6.ActivePageIndex := 0;
  PageControl2.ActivePageIndex := 0;
  CheckBoxHungerDecPower.Visible := True;

  FunctionConfigControl.ActivePage := TabSheetGeneral;
end;

procedure TfrmFunctionConfig.CheckBoxEnablePasswordLockClick(Sender: TObject);
begin
  case CheckBoxEnablePasswordLock.Checked of
    True:
      begin
        CheckBoxLockGetBackItem.Enabled := True;
        CheckBoxLockLogin.Enabled := True;
      end;
    False:
      begin
        CheckBoxLockGetBackItem.Checked := False;
        CheckBoxLockLogin.Checked := False;

        CheckBoxLockGetBackItem.Enabled := False;
        CheckBoxLockLogin.Enabled := False;
      end;
  end;
  if not boOpened then
    Exit;
  g_Config.boPasswordLockSystem := CheckBoxEnablePasswordLock.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockGetBackItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockGetBackItemAction := CheckBoxLockGetBackItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockDealItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockDealAction := CheckBoxLockDealItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockDropItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockDropAction := CheckBoxLockDropItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockUseItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockUserItemAction := CheckBoxLockUseItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockLoginClick(Sender: TObject);
begin
  case CheckBoxLockLogin.Checked of //
    True:
      begin
        CheckBoxLockWalk.Enabled := True;
        CheckBoxLockRun.Enabled := True;
        CheckBoxLockHit.Enabled := True;
        CheckBoxLockSpell.Enabled := True;
        CheckBoxLockInObMode.Enabled := True;
        CheckBoxLockSendMsg.Enabled := True;
        CheckBoxLockDealItem.Enabled := True;
        CheckBoxLockDropItem.Enabled := True;
        CheckBoxLockUseItem.Enabled := True;

        chkLockChallenge.Enabled := True;
        chkLockSummonHero.Enabled := True;
        chkLockShop.Enabled := True;
        chkLockStall.Enabled := True;
      end;

    False:
      begin
        CheckBoxLockWalk.Checked := False;
        CheckBoxLockRun.Checked := False;
        CheckBoxLockHit.Checked := False;
        CheckBoxLockSpell.Checked := False;
        CheckBoxLockInObMode.Checked := False;
        CheckBoxLockSendMsg.Checked := False;
        CheckBoxLockDealItem.Checked := False;
        CheckBoxLockDropItem.Checked := False;
        CheckBoxLockUseItem.Checked := False;
        chkLockChallenge.Checked := False;
        chkLockSummonHero.Checked := False;
        chkLockShop.Checked := False;
        chkLockStall.Checked := False;

        CheckBoxLockWalk.Enabled := False;
        CheckBoxLockRun.Enabled := False;
        CheckBoxLockHit.Enabled := False;
        CheckBoxLockSpell.Enabled := False;
        CheckBoxLockInObMode.Enabled := False;
        CheckBoxLockSendMsg.Enabled := False;
        CheckBoxLockDealItem.Enabled := False;
        CheckBoxLockDropItem.Enabled := False;
        CheckBoxLockUseItem.Enabled := False;
        chkLockChallenge.Enabled := False;
        chkLockSummonHero.Enabled := False;
        chkLockShop.Enabled := False;
        chkLockStall.Enabled := False;
      end;
  end;
  if not boOpened then
    Exit;
  g_Config.boLockHumanLogin := CheckBoxLockLogin.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockWalkClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockWalkAction := CheckBoxLockWalk.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockRunAction := CheckBoxLockRun.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockHitClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockHitAction := CheckBoxLockHit.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockSpellClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockSpellAction := CheckBoxLockSpell.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockSendMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockSendMsgAction := CheckBoxLockSendMsg.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLockInObModeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockInObModeAction := CheckBoxLockInObMode.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditErrorPasswordCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPasswordErrorCountLock := EditErrorPasswordCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxErrorCountKickClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPasswordErrorCountLock := EditErrorPasswordCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.ButtonPasswordLockSaveClick(Sender: TObject);
begin
  Config.WriteBool('Setup', 'PasswordLockSystem', g_Config.boPasswordLockSystem);
  Config.WriteBool('Setup', 'PasswordLockDealAction', g_Config.boLockDealAction);
  Config.WriteBool('Setup', 'PasswordLockDropAction', g_Config.boLockDropAction);
  Config.WriteBool('Setup', 'PasswordLockGetBackItemAction', g_Config.boLockGetBackItemAction);
  Config.WriteBool('Setup', 'PasswordLockWalkAction', g_Config.boLockWalkAction);
  Config.WriteBool('Setup', 'PasswordLockRunAction', g_Config.boLockRunAction);
  Config.WriteBool('Setup', 'PasswordLockHitAction', g_Config.boLockHitAction);
  Config.WriteBool('Setup', 'PasswordLockSpellAction', g_Config.boLockSpellAction);
  Config.WriteBool('Setup', 'PasswordLockSendMsgAction', g_Config.boLockSendMsgAction);
  Config.WriteBool('Setup', 'PasswordLockInObModeAction', g_Config.boLockInObModeAction);
  Config.WriteBool('Setup', 'PasswordLockUserItemAction', g_Config.boLockUserItemAction);

  Config.WriteBool('Setup', 'LockChallenge', g_Config.boLockChallenge);
  // 禁止挑战 chongchong 2013-07-22
  Config.WriteBool('Setup', 'LockSummonHero', g_Config.boLockSummonHero);
  // 禁止召唤英雄 chongchong 2013-07-22
  Config.WriteBool('Setup', 'LockShop', g_Config.boLockShop);
  // 禁止商铺 chongchong 2013-07-22
  Config.WriteBool('Setup', 'LockStall', g_Config.boLockStall);
  // 禁止摆摊 chongchong 2013-07-22

  Config.WriteBool('Setup', 'PasswordLockHumanLogin', g_Config.boLockHumanLogin);
  Config.WriteInteger('Setup', 'PasswordErrorCountLock', g_Config.nPasswordErrorCountLock);

  if boSendServerConfig then
    UserEngine.SendServerConfig();

  uModValue();
end;

procedure TfrmFunctionConfig.RefGeneral();
begin
  EditPKFlagNameColor.Value := g_Config.btPKFlagNameColor;
  EditPKLevel1NameColor.Value := g_Config.btPKLevel1NameColor;
  EditPKLevel2NameColor.Value := g_Config.btPKLevel2NameColor;
  EditAllyAndGuildNameColor.Value := g_Config.btAllyAndGuildNameColor;
  EditWarGuildNameColor.Value := g_Config.btWarGuildNameColor;
  EditInFreePKAreaNameColor.Value := g_Config.btInFreePKAreaNameColor;
  EditMerchantNameColor.Value := g_Config.btMerchantNameColor;
  seMerchant273NameColor.Value := g_Config.btMerchant273NameColor;
  seGuardNameColor.Value := g_Config.btGuardNameColor;
end;

procedure TfrmFunctionConfig.CheckBoxHungerSystemClick(Sender: TObject);
begin
  if CheckBoxHungerSystem.Checked then
  begin
    CheckBoxHungerDecHP.Enabled := True;
    CheckBoxHungerDecPower.Enabled := True;
  end
  else
  begin
    CheckBoxHungerDecHP.Checked := False;
    CheckBoxHungerDecPower.Checked := False;
    CheckBoxHungerDecHP.Enabled := False;
    CheckBoxHungerDecPower.Enabled := False;
  end;

  if not boOpened then
    Exit;
  g_Config.boHungerSystem := CheckBoxHungerSystem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxHungerDecHPClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHungerDecHP := CheckBoxHungerDecHP.Checked;
  ModValue();

end;

procedure TfrmFunctionConfig.CheckBoxHungerDecPowerClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHungerDecPower := CheckBoxHungerDecPower.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.ButtonGeneralSaveClick(Sender: TObject);
begin
  { TODO -ochongchong -c新增 : 加武器前缀时过滤名称 【2013-07-24】 }
  if CheckBoxItemName.Checked and GetNameInFilterList(EditItemName.Text) then
  begin
    ShowMessage('装备刻名自定义前缀包含非法字符');
    if EditItemName.CanFocus then
      EditItemName.SetFocus;
    Exit;
  end;

  Config.WriteBool('Setup', 'HungerSystem', g_Config.boHungerSystem);
  Config.WriteBool('Setup', 'HungerDecHP', g_Config.boHungerDecHP);
  Config.WriteBool('Setup', 'HungerDecPower', g_Config.boHungerDecPower);

  Config.WriteInteger('Setup', 'PKFlagNameColor', g_Config.btPKFlagNameColor);
  Config.WriteInteger('Setup', 'AllyAndGuildNameColor', g_Config.btAllyAndGuildNameColor);
  Config.WriteInteger('Setup', 'WarGuildNameColor', g_Config.btWarGuildNameColor);
  Config.WriteInteger('Setup', 'InFreePKAreaNameColor', g_Config.btInFreePKAreaNameColor);
  Config.WriteInteger('Setup', 'PKLevel1NameColor', g_Config.btPKLevel1NameColor);
  Config.WriteInteger('Setup', 'PKLevel2NameColor', g_Config.btPKLevel2NameColor);
  Config.WriteInteger('Setup', 'MerchantNameColor', g_Config.btMerchantNameColor);
  Config.WriteInteger('Setup', 'Merchant273NameColor', g_Config.btMerchant273NameColor);
  Config.WriteInteger('Setup', 'GuardNameColor', g_Config.btGuardNameColor);

  Config.WriteInteger('Setup', 'HPRockRate', g_Config.nHPRockRate);
  Config.WriteInteger('Setup', 'HPRockType', g_Config.nHPRockType);
  Config.WriteInteger('Setup', 'HPRockTime', g_Config.nHPRockTime);
  Config.WriteInteger('Setup', 'HPRockAddType', g_Config.nHPRockAddType);
  Config.WriteInteger('Setup', 'HPRockAddValue', g_Config.nHPRockAddValue);
  Config.WriteInteger('Setup', 'HPRockDecValue', g_Config.nHPRockDecValue);

  Config.WriteInteger('Setup', 'MPRockRate', g_Config.nMPRockRate);
  Config.WriteInteger('Setup', 'MPRockType', g_Config.nMPRockType);
  Config.WriteInteger('Setup', 'MPRockTime', g_Config.nMPRockTime);
  Config.WriteInteger('Setup', 'MPRockAddType', g_Config.nMPRockAddType);
  Config.WriteInteger('Setup', 'MPRockAddValue', g_Config.nMPRockAddValue);
  Config.WriteInteger('Setup', 'MPRockDecValue', g_Config.nMPRockDecValue);

  Config.WriteInteger('Setup', 'HMPRockRate', g_Config.nHMPRockRate);
  Config.WriteInteger('Setup', 'HMPRockType', g_Config.nHMPRockType);
  Config.WriteInteger('Setup', 'HMPRockTime', g_Config.nHMPRockTime);
  Config.WriteInteger('Setup', 'HMPRockAddType', g_Config.nHMPRockAddType);
  Config.WriteInteger('Setup', 'HMPRockAddValue', g_Config.nHMPRockAddValue);
  Config.WriteInteger('Setup', 'HMPRockDecValue', g_Config.nHMPRockDecValue);
  Config.WriteBool('Setup', 'HMPUse2Times', g_Config.boHMPUse2Times);
  Config.WriteInteger('Setup', 'HMPDivDura', g_Config.nHMPDivDura);

  Config.WriteBool('Setup', 'DropOverLapItem', g_Config.boDropOverLapItem);
  Config.WriteBool('Setup', 'OpenMapEvent', g_Config.boOpenMapEvent);
  uModValue();
  UserEngine.SendServerConfig();
end;

procedure TfrmFunctionConfig.EditMagicAttackRageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagicAttackRage := EditMagicAttackRage.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.RefMagicSkill;
begin
  EditSwordLongPowerRate.Value := g_Config.nSwordLongPowerRate;
  CheckBoxLimitSwordLong.Checked := g_Config.boLimitSwordLong;

  seSkillYedoPowerRate.Value := g_Config.nSkillYedoPowerRate;

  // 冰咆哮 piaoyun 2013-07-25
  seSnowWindRange.Value := g_Config.nSnowWindRange;
  seSnowWindPowerRate.Value := g_Config.nSnowWindPowerRate;
  seSnowwindWaitTime.Value := g_Config.nSnowwindWaitTime;

  // 爆裂火焰 piaoyun 2013-07-25
  seFireBoomRage.Value := g_Config.nFireBoomRage;
  seFireBoomRagePowerRate.Value := g_Config.nFireBoomRagePowerRate;

  // 地狱雷光 piaoyun 2013-07-25
  seElecBlizzardRange.Value := g_Config.nElecBlizzardRange;
  seElecBlizzardPowerRate.Value := g_Config.nElecBlizzardPowerRate;

  CheckBoxViewRangeCanMagicAttack.Checked := g_Config.boViewRangeCanMagicAttack;
  chkShowMsgMagicRangeExceed.Checked := g_Config.boShowMsgMagicRangeExceed;

  EditMagicAttackRage.Value := g_Config.nMagicAttackRage;
  EditAmyOunsulPoint.Value := g_Config.nAmyOunsulPoint;
  EditAmyOunsulTimeRate.Value := g_Config.nAmyOunsulTimeRate;
  EditAmyOunsulMaxTime.Value := g_Config.nAmyOunsulMaxTime;

  seSkillAmyounsulCDTime.Value := g_Config.nSkillAmyounsulCDTime;
  seSkillGroupAmyounsulCDTime.Value := g_Config.nSkillGroupAmyounsulCDTime;
  chkSkillGroupAmyounsulRed.Checked := g_Config.boSkillGroupAmyounsulRed;
  chkSkillGroupAmyounsulGreen.Checked := g_Config.boSkillGroupAmyounsulGreen;

  sePosionDecHealthTime.Value := g_Config.dwPosionDecHealthTime;
  sePosionDamagarmor.Value := g_Config.nPosionDamagarmor;
  chkEnabledPosionDecMAC.Checked := g_Config.boEnabledPosionDecMAC;
  sePosionDecMACRate.Value := g_Config.nPosionDecMACRate;
  chkPosionStopIncHealth.Checked := g_Config.boPosionStopIncHealth;

  CheckBoxShowYouPoisoned.Checked := g_Config.boShowYouPoisoned;

  seMagTurnUndeadLevel.Value := g_Config.nMagTurnUndeadLevel;
  chkMagTurnUndeadSameLevel.Checked := g_Config.boMagTurnUndeadSameLevel;

  EditMagTammingLevel.Value := g_Config.nMagTammingLevel;
  EditMagTammingTargetLevel.Value := g_Config.nMagTammingTargetLevel;
  EditMagTammingHPRate.Value := g_Config.nMagTammingHPRate;
  EditTammingCount.Value := g_Config.nMagTammingCount;
  EditMasterRoyaltyTime.Value := g_Config.nMasterRoyaltyTime;
  EditMabMabeHitRandRate.Value := g_Config.nMabMabeHitRandRate;
  EditMabMabeHitMinLvLimit.Value := g_Config.nMabMabeHitMinLvLimit;
  EditMabMabeHitSucessRate.Value := g_Config.nMabMabeHitSucessRate;
  EditMabMabeHitMabeTimeRate.Value := g_Config.nMabMabeHitMabeTimeRate;
  seMaxMabMabeHitMabeTime.Value := g_Config.nMaxMabMabeHitMabeTime;

  // 魔法盾防御倍率 piaoyun 2013-08-18
  seOrdinarySkill31Rate.Value := g_Config.nOrdinarySkill31Rate;
  // 强化魔法盾防御倍率 piaoyun 2013-08-18

  cbbSkill31Level.ItemIndex := 0;
  seSkill31Rate.Value := g_Config.nSkill31Rates[cbbSkill31Level.ItemIndex];

  seSkill113RateAddWithSkill63.Value := g_Config.nSkill113RateAddWithSkill63;
  chkSkill31UseNewEffect.Checked := g_Config.boSkill31UseNewEffect;
  chkSkill31UseLEGEffect.Checked := g_Config.boSkill31UseLEGEffect;

  CheckBoxFireCrossInSafeZone.Checked := g_Config.boDisableInSafeZoneFireCross;
  CheckBoxDisableChangeMapFireCross.Checked := g_Config.boDisableChangeMapFireCross;
  EditFireCrossMaxTime.Value := g_Config.nFireCrossMaxTime;
  EditFireCrossPowerRate.Value := g_Config.nFireCrossPowerRate;

  chkSkill41MbAttackPlayObject.Checked := g_Config.boSkill41MbAttackPlayObject;
  chkDisableSkill41MbSameLevel.Checked := g_Config.boDisableSkill41MbSameLevel;
  chkSkill41MbAttackSlave.Checked := g_Config.boSkill41MbAttackSlave;
  seSkill41CD.Value := g_Config.nSkill41CD;
  seSkill41MbTimer0.Value := g_Config.nSkill41MbTimers[0];
  seSkill41MbTimer1.Value := g_Config.nSkill41MbTimers[1];
  seSkill41MbTimer2.Value := g_Config.nSkill41MbTimers[2];
  seSkill41MbTimer3.Value := g_Config.nSkill41MbTimers[3];

  seSkill41MbRange0.Value := g_Config.nSkill41MbRanges[0];
  seSkill41MbRange1.Value := g_Config.nSkill41MbRanges[1];
  seSkill41MbRange2.Value := g_Config.nSkill41MbRanges[2];
  seSkill41MbRange3.Value := g_Config.nSkill41MbRanges[3];

  EditElectrodelessBase.Value := g_Config.nElectrodelessBase;
  EditElectrodelessWaitTime.Value := g_Config.nElectrodelessWaitTime;
  seElectrodelessPowerRateL0.Value := g_Config.nElectrodelessPowerRates[0];
  seElectrodelessPowerRateL1.Value := g_Config.nElectrodelessPowerRates[1];
  seElectrodelessPowerRateL2.Value := g_Config.nElectrodelessPowerRates[2];
  seElectrodelessPowerRateL3.Value := g_Config.nElectrodelessPowerRates[3];
  seElectrodelessPowerRateL4.Value := g_Config.nElectrodelessPowerRates[4];

  chkElectrodelessTimeSet.Checked := g_Config.boElectrodelessTimeSet;
  chkElectrodelessUseMaxSC.Checked := g_Config.boElectrodelessUseMaxSC;
  seElectrodelessTimeL0.Value := g_Config.dwElectrodelessTimes[0];
  seElectrodelessTimeL1.Value := g_Config.dwElectrodelessTimes[1];
  seElectrodelessTimeL2.Value := g_Config.dwElectrodelessTimes[2];
  seElectrodelessTimeL3.Value := g_Config.dwElectrodelessTimes[3];
  seElectrodelessTimeL4.Value := g_Config.dwElectrodelessTimes[4];

  // 神圣战甲术、幽灵盾 piaoyun 2013-07-25
  seSkill15PowerRate.Value := g_Config.nSkill15PowerRate;
  seSkill15TimeRate.Value := g_Config.nSkill15TimeRate;
  chkSkill15OfflineClear.Checked := g_Config.boSkill15OfflineClear;

  CheckBoxQigongPushSameLevel.Checked := g_Config.boQigongPushSameLevel;
  CheckBoxFireWindPushSameLevel.Checked := g_Config.boFireWindPushSameLevel;
  seSkill58WaitTime.Value := g_Config.nSkill58WaitTime;
  seHeroSkill58WaitTime.Value := g_Config.nHeroSkill58WaitTime;

  EditSkillContinuousPowerRate100.Value := g_Config.SkillContinuousPowerRates[0];
  EditSkillContinuousPowerRate101.Value := g_Config.SkillContinuousPowerRates[1];
  EditSkillContinuousPowerRate102.Value := g_Config.SkillContinuousPowerRates[2];
  EditSkillContinuousPowerRate103.Value := g_Config.SkillContinuousPowerRates[3];
  EditSkillContinuousPowerRate104.Value := g_Config.SkillContinuousPowerRates[4];
  EditSkillContinuousPowerRate105.Value := g_Config.SkillContinuousPowerRates[5];
  EditSkillContinuousPowerRate106.Value := g_Config.SkillContinuousPowerRates[6];
  EditSkillContinuousPowerRate107.Value := g_Config.SkillContinuousPowerRates[7];
  EditSkillContinuousPowerRate108.Value := g_Config.SkillContinuousPowerRates[8];
  EditSkillContinuousPowerRate109.Value := g_Config.SkillContinuousPowerRates[9];
  EditSkillContinuousPowerRate110.Value := g_Config.SkillContinuousPowerRates[10];
  EditSkillContinuousPowerRate111.Value := g_Config.SkillContinuousPowerRates[11];

  edtSkillContinuousCloseDefenseRates100.Value := g_Config.SkillContinuousCloseDefenseRates[0];
  edtSkillContinuousCloseDefenseRates101.Value := g_Config.SkillContinuousCloseDefenseRates[1];
  edtSkillContinuousCloseDefenseRates102.Value := g_Config.SkillContinuousCloseDefenseRates[2];
  edtSkillContinuousCloseDefenseRates103.Value := g_Config.SkillContinuousCloseDefenseRates[3];

  seSkill100BreakDefenceUpRate.Value := g_Config.Skill100BreakDefenceUpRate;

  chkToxicSmoke.Checked := g_Config.boToxicSmoke;
  cbbSkillContinuousPowerRate111.ItemIndex := 0;
  seToxicSmokeTime.Value := g_Config.dwToxicSmokeTimes[0];
  seToxicSmokeDecHPRate.Value := g_Config.nToxicSmokeDecHPRate;

  EditSkillContinuousBlastHitRate100_0.Value := g_Config.SkillContinuousBlastHitRates[0, 0];
  EditSkillContinuousBlastHitRate100_1.Value := g_Config.SkillContinuousBlastHitRates[0, 1];
  EditSkillContinuousBlastHitRate100_2.Value := g_Config.SkillContinuousBlastHitRates[0, 2];
  EditSkillContinuousBlastHitRate100_3.Value := g_Config.SkillContinuousBlastHitRates[0, 3];
  EditSkillContinuousBlastHitRate100_4.Value := g_Config.SkillContinuousBlastHitRates[0, 4];

  EditSkillContinuousBlastHitRate101_0.Value := g_Config.SkillContinuousBlastHitRates[1, 0];
  EditSkillContinuousBlastHitRate101_1.Value := g_Config.SkillContinuousBlastHitRates[1, 1];
  EditSkillContinuousBlastHitRate101_2.Value := g_Config.SkillContinuousBlastHitRates[1, 2];
  EditSkillContinuousBlastHitRate101_3.Value := g_Config.SkillContinuousBlastHitRates[1, 3];
  EditSkillContinuousBlastHitRate101_4.Value := g_Config.SkillContinuousBlastHitRates[1, 4];

  EditSkillContinuousBlastHitRate102_0.Value := g_Config.SkillContinuousBlastHitRates[2, 0];
  EditSkillContinuousBlastHitRate102_1.Value := g_Config.SkillContinuousBlastHitRates[2, 1];
  EditSkillContinuousBlastHitRate102_2.Value := g_Config.SkillContinuousBlastHitRates[2, 2];
  EditSkillContinuousBlastHitRate102_3.Value := g_Config.SkillContinuousBlastHitRates[2, 3];
  EditSkillContinuousBlastHitRate102_4.Value := g_Config.SkillContinuousBlastHitRates[2, 4];

  EditSkillContinuousBlastHitRate103_0.Value := g_Config.SkillContinuousBlastHitRates[3, 0];
  EditSkillContinuousBlastHitRate103_1.Value := g_Config.SkillContinuousBlastHitRates[3, 1];
  EditSkillContinuousBlastHitRate103_2.Value := g_Config.SkillContinuousBlastHitRates[3, 2];
  EditSkillContinuousBlastHitRate103_3.Value := g_Config.SkillContinuousBlastHitRates[3, 3];
  EditSkillContinuousBlastHitRate103_4.Value := g_Config.SkillContinuousBlastHitRates[3, 4];

  EditSkillContinuousBlastHitRate104_0.Value := g_Config.SkillContinuousBlastHitRates[4, 0];
  EditSkillContinuousBlastHitRate104_1.Value := g_Config.SkillContinuousBlastHitRates[4, 1];
  EditSkillContinuousBlastHitRate104_2.Value := g_Config.SkillContinuousBlastHitRates[4, 2];
  EditSkillContinuousBlastHitRate104_3.Value := g_Config.SkillContinuousBlastHitRates[4, 3];
  EditSkillContinuousBlastHitRate104_4.Value := g_Config.SkillContinuousBlastHitRates[4, 4];

  EditSkillContinuousBlastHitRate105_0.Value := g_Config.SkillContinuousBlastHitRates[5, 0];
  EditSkillContinuousBlastHitRate105_1.Value := g_Config.SkillContinuousBlastHitRates[5, 1];
  EditSkillContinuousBlastHitRate105_2.Value := g_Config.SkillContinuousBlastHitRates[5, 2];
  EditSkillContinuousBlastHitRate105_3.Value := g_Config.SkillContinuousBlastHitRates[5, 3];
  EditSkillContinuousBlastHitRate105_4.Value := g_Config.SkillContinuousBlastHitRates[5, 4];

  EditSkillContinuousBlastHitRate106_0.Value := g_Config.SkillContinuousBlastHitRates[6, 0];
  EditSkillContinuousBlastHitRate106_1.Value := g_Config.SkillContinuousBlastHitRates[6, 1];
  EditSkillContinuousBlastHitRate106_2.Value := g_Config.SkillContinuousBlastHitRates[6, 2];
  EditSkillContinuousBlastHitRate106_3.Value := g_Config.SkillContinuousBlastHitRates[6, 3];
  EditSkillContinuousBlastHitRate106_4.Value := g_Config.SkillContinuousBlastHitRates[6, 4];

  EditSkillContinuousBlastHitRate107_0.Value := g_Config.SkillContinuousBlastHitRates[7, 0];
  EditSkillContinuousBlastHitRate107_1.Value := g_Config.SkillContinuousBlastHitRates[7, 1];
  EditSkillContinuousBlastHitRate107_2.Value := g_Config.SkillContinuousBlastHitRates[7, 2];
  EditSkillContinuousBlastHitRate107_3.Value := g_Config.SkillContinuousBlastHitRates[7, 3];
  EditSkillContinuousBlastHitRate107_4.Value := g_Config.SkillContinuousBlastHitRates[7, 4];

  EditSkillContinuousBlastHitRate108_0.Value := g_Config.SkillContinuousBlastHitRates[8, 0];
  EditSkillContinuousBlastHitRate108_1.Value := g_Config.SkillContinuousBlastHitRates[8, 1];
  EditSkillContinuousBlastHitRate108_2.Value := g_Config.SkillContinuousBlastHitRates[8, 2];
  EditSkillContinuousBlastHitRate108_3.Value := g_Config.SkillContinuousBlastHitRates[8, 3];
  EditSkillContinuousBlastHitRate108_4.Value := g_Config.SkillContinuousBlastHitRates[8, 4];

  EditSkillContinuousBlastHitRate109_0.Value := g_Config.SkillContinuousBlastHitRates[9, 0];
  EditSkillContinuousBlastHitRate109_1.Value := g_Config.SkillContinuousBlastHitRates[9, 1];
  EditSkillContinuousBlastHitRate109_2.Value := g_Config.SkillContinuousBlastHitRates[9, 2];
  EditSkillContinuousBlastHitRate109_3.Value := g_Config.SkillContinuousBlastHitRates[9, 3];
  EditSkillContinuousBlastHitRate109_4.Value := g_Config.SkillContinuousBlastHitRates[9, 4];

  EditSkillContinuousBlastHitRate110_0.Value := g_Config.SkillContinuousBlastHitRates[10, 0];
  EditSkillContinuousBlastHitRate110_1.Value := g_Config.SkillContinuousBlastHitRates[10, 1];
  EditSkillContinuousBlastHitRate110_2.Value := g_Config.SkillContinuousBlastHitRates[10, 2];
  EditSkillContinuousBlastHitRate110_3.Value := g_Config.SkillContinuousBlastHitRates[10, 3];
  EditSkillContinuousBlastHitRate110_4.Value := g_Config.SkillContinuousBlastHitRates[10, 4];

  EditSkillContinuousBlastHitRate111_0.Value := g_Config.SkillContinuousBlastHitRates[11, 0];
  EditSkillContinuousBlastHitRate111_1.Value := g_Config.SkillContinuousBlastHitRates[11, 1];
  EditSkillContinuousBlastHitRate111_2.Value := g_Config.SkillContinuousBlastHitRates[11, 2];
  EditSkillContinuousBlastHitRate111_3.Value := g_Config.SkillContinuousBlastHitRates[11, 3];
  EditSkillContinuousBlastHitRate111_4.Value := g_Config.SkillContinuousBlastHitRates[11, 4];

  EditSkillContinuousBlastHitPowerRates100_0.Value := g_Config.SkillContinuousBlastHitPowerRates[0, 0];
  EditSkillContinuousBlastHitPowerRates100_1.Value := g_Config.SkillContinuousBlastHitPowerRates[0, 1];
  EditSkillContinuousBlastHitPowerRates100_2.Value := g_Config.SkillContinuousBlastHitPowerRates[0, 2];
  EditSkillContinuousBlastHitPowerRates100_3.Value := g_Config.SkillContinuousBlastHitPowerRates[0, 3];
  EditSkillContinuousBlastHitPowerRates100_4.Value := g_Config.SkillContinuousBlastHitPowerRates[0, 4];

  EditSkillContinuousBlastHitPowerRates101_0.Value := g_Config.SkillContinuousBlastHitPowerRates[1, 0];
  EditSkillContinuousBlastHitPowerRates101_1.Value := g_Config.SkillContinuousBlastHitPowerRates[1, 1];
  EditSkillContinuousBlastHitPowerRates101_2.Value := g_Config.SkillContinuousBlastHitPowerRates[1, 2];
  EditSkillContinuousBlastHitPowerRates101_3.Value := g_Config.SkillContinuousBlastHitPowerRates[1, 3];
  EditSkillContinuousBlastHitPowerRates101_4.Value := g_Config.SkillContinuousBlastHitPowerRates[1, 4];

  EditSkillContinuousBlastHitPowerRates102_0.Value := g_Config.SkillContinuousBlastHitPowerRates[2, 0];
  EditSkillContinuousBlastHitPowerRates102_1.Value := g_Config.SkillContinuousBlastHitPowerRates[2, 1];
  EditSkillContinuousBlastHitPowerRates102_2.Value := g_Config.SkillContinuousBlastHitPowerRates[2, 2];
  EditSkillContinuousBlastHitPowerRates102_3.Value := g_Config.SkillContinuousBlastHitPowerRates[2, 3];
  EditSkillContinuousBlastHitPowerRates102_4.Value := g_Config.SkillContinuousBlastHitPowerRates[2, 4];

  EditSkillContinuousBlastHitPowerRates103_0.Value := g_Config.SkillContinuousBlastHitPowerRates[3, 0];
  EditSkillContinuousBlastHitPowerRates103_1.Value := g_Config.SkillContinuousBlastHitPowerRates[3, 1];
  EditSkillContinuousBlastHitPowerRates103_2.Value := g_Config.SkillContinuousBlastHitPowerRates[3, 2];
  EditSkillContinuousBlastHitPowerRates103_3.Value := g_Config.SkillContinuousBlastHitPowerRates[3, 3];
  EditSkillContinuousBlastHitPowerRates103_4.Value := g_Config.SkillContinuousBlastHitPowerRates[3, 4];

  EditSkillContinuousBlastHitPowerRates104_0.Value := g_Config.SkillContinuousBlastHitPowerRates[4, 0];
  EditSkillContinuousBlastHitPowerRates104_1.Value := g_Config.SkillContinuousBlastHitPowerRates[4, 1];
  EditSkillContinuousBlastHitPowerRates104_2.Value := g_Config.SkillContinuousBlastHitPowerRates[4, 2];
  EditSkillContinuousBlastHitPowerRates104_3.Value := g_Config.SkillContinuousBlastHitPowerRates[4, 3];
  EditSkillContinuousBlastHitPowerRates104_4.Value := g_Config.SkillContinuousBlastHitPowerRates[4, 4];

  EditSkillContinuousBlastHitPowerRates105_0.Value := g_Config.SkillContinuousBlastHitPowerRates[5, 0];
  EditSkillContinuousBlastHitPowerRates105_1.Value := g_Config.SkillContinuousBlastHitPowerRates[5, 1];
  EditSkillContinuousBlastHitPowerRates105_2.Value := g_Config.SkillContinuousBlastHitPowerRates[5, 2];
  EditSkillContinuousBlastHitPowerRates105_3.Value := g_Config.SkillContinuousBlastHitPowerRates[5, 3];
  EditSkillContinuousBlastHitPowerRates105_4.Value := g_Config.SkillContinuousBlastHitPowerRates[5, 4];

  EditSkillContinuousBlastHitPowerRates106_0.Value := g_Config.SkillContinuousBlastHitPowerRates[6, 0];
  EditSkillContinuousBlastHitPowerRates106_1.Value := g_Config.SkillContinuousBlastHitPowerRates[6, 1];
  EditSkillContinuousBlastHitPowerRates106_2.Value := g_Config.SkillContinuousBlastHitPowerRates[6, 2];
  EditSkillContinuousBlastHitPowerRates106_3.Value := g_Config.SkillContinuousBlastHitPowerRates[6, 3];
  EditSkillContinuousBlastHitPowerRates106_4.Value := g_Config.SkillContinuousBlastHitPowerRates[6, 4];

  EditSkillContinuousBlastHitPowerRates107_0.Value := g_Config.SkillContinuousBlastHitPowerRates[7, 0];
  EditSkillContinuousBlastHitPowerRates107_1.Value := g_Config.SkillContinuousBlastHitPowerRates[7, 1];
  EditSkillContinuousBlastHitPowerRates107_2.Value := g_Config.SkillContinuousBlastHitPowerRates[7, 2];
  EditSkillContinuousBlastHitPowerRates107_3.Value := g_Config.SkillContinuousBlastHitPowerRates[7, 3];
  EditSkillContinuousBlastHitPowerRates107_4.Value := g_Config.SkillContinuousBlastHitPowerRates[7, 4];

  EditSkillContinuousBlastHitPowerRates108_0.Value := g_Config.SkillContinuousBlastHitPowerRates[8, 0];
  EditSkillContinuousBlastHitPowerRates108_1.Value := g_Config.SkillContinuousBlastHitPowerRates[8, 1];
  EditSkillContinuousBlastHitPowerRates108_2.Value := g_Config.SkillContinuousBlastHitPowerRates[8, 2];
  EditSkillContinuousBlastHitPowerRates108_3.Value := g_Config.SkillContinuousBlastHitPowerRates[8, 3];
  EditSkillContinuousBlastHitPowerRates108_4.Value := g_Config.SkillContinuousBlastHitPowerRates[8, 4];

  EditSkillContinuousBlastHitPowerRates109_0.Value := g_Config.SkillContinuousBlastHitPowerRates[9, 0];
  EditSkillContinuousBlastHitPowerRates109_1.Value := g_Config.SkillContinuousBlastHitPowerRates[9, 1];
  EditSkillContinuousBlastHitPowerRates109_2.Value := g_Config.SkillContinuousBlastHitPowerRates[9, 2];
  EditSkillContinuousBlastHitPowerRates109_3.Value := g_Config.SkillContinuousBlastHitPowerRates[9, 3];
  EditSkillContinuousBlastHitPowerRates109_4.Value := g_Config.SkillContinuousBlastHitPowerRates[9, 4];

  EditSkillContinuousBlastHitPowerRates110_0.Value := g_Config.SkillContinuousBlastHitPowerRates[10, 0];
  EditSkillContinuousBlastHitPowerRates110_1.Value := g_Config.SkillContinuousBlastHitPowerRates[10, 1];
  EditSkillContinuousBlastHitPowerRates110_2.Value := g_Config.SkillContinuousBlastHitPowerRates[10, 2];
  EditSkillContinuousBlastHitPowerRates110_3.Value := g_Config.SkillContinuousBlastHitPowerRates[10, 3];
  EditSkillContinuousBlastHitPowerRates110_4.Value := g_Config.SkillContinuousBlastHitPowerRates[10, 4];

  EditSkillContinuousBlastHitPowerRates111_0.Value := g_Config.SkillContinuousBlastHitPowerRates[11, 0];
  EditSkillContinuousBlastHitPowerRates111_1.Value := g_Config.SkillContinuousBlastHitPowerRates[11, 1];
  EditSkillContinuousBlastHitPowerRates111_2.Value := g_Config.SkillContinuousBlastHitPowerRates[11, 2];
  EditSkillContinuousBlastHitPowerRates111_3.Value := g_Config.SkillContinuousBlastHitPowerRates[11, 3];
  EditSkillContinuousBlastHitPowerRates111_4.Value := g_Config.SkillContinuousBlastHitPowerRates[11, 4];

  chkDoMotaebo100PushSameLevel.Checked := g_Config.boDoMotaebo100PushSameLevel;
  seDoMotaebo100PushDistance.Value := g_Config.nDoMotaebo100PushDistance;

  EditAcupoints0_0.Value := g_Config.AcupointLevels[0, 0];
  EditAcupoints0_1.Value := g_Config.AcupointLevels[0, 1];
  EditAcupoints0_2.Value := g_Config.AcupointLevels[0, 2];
  EditAcupoints0_3.Value := g_Config.AcupointLevels[0, 3];
  EditAcupoints0_4.Value := g_Config.AcupointLevels[0, 4];

  EditAcupoints1_0.Value := g_Config.AcupointLevels[1, 0];
  EditAcupoints1_1.Value := g_Config.AcupointLevels[1, 1];
  EditAcupoints1_2.Value := g_Config.AcupointLevels[1, 2];
  EditAcupoints1_3.Value := g_Config.AcupointLevels[1, 3];
  EditAcupoints1_4.Value := g_Config.AcupointLevels[1, 4];

  EditAcupoints2_0.Value := g_Config.AcupointLevels[2, 0];
  EditAcupoints2_1.Value := g_Config.AcupointLevels[2, 1];
  EditAcupoints2_2.Value := g_Config.AcupointLevels[2, 2];
  EditAcupoints2_3.Value := g_Config.AcupointLevels[2, 3];
  EditAcupoints2_4.Value := g_Config.AcupointLevels[2, 4];

  EditAcupoints3_0.Value := g_Config.AcupointLevels[3, 0];
  EditAcupoints3_1.Value := g_Config.AcupointLevels[3, 1];
  EditAcupoints3_2.Value := g_Config.AcupointLevels[3, 2];
  EditAcupoints3_3.Value := g_Config.AcupointLevels[3, 3];
  EditAcupoints3_4.Value := g_Config.AcupointLevels[3, 4];

  EditAcupoints4_0.Value := g_Config.AcupointLevels[4, 0];
  EditAcupoints4_1.Value := g_Config.AcupointLevels[4, 1];
  EditAcupoints4_2.Value := g_Config.AcupointLevels[4, 2];
  EditAcupoints4_3.Value := g_Config.AcupointLevels[4, 3];
  EditAcupoints4_4.Value := g_Config.AcupointLevels[4, 4];

  // 彻地钉
  seDedingMagicCD.Value := g_Config.nDedingMagicCD;
  seDeDingMagicBasicPowerRate.Value := g_Config.nDeDingMagicBasicPowerRate;
  seDeDingMagicAttackRange.Value := g_Config.nDeDingMagicAttackRange;
  chkDedingAllowPK.Checked := g_Config.boDedingAllowPK;
  chkDedingDisabledPK.Checked := g_Config.boDedingDisabledPK;

  // 新技能，控件值 --- piaoyun 2013-06-24
  // 裂神符
  seSkill202BaseCount.Value := g_Config.nSkill202BaseCount;
  seSkill202LevelupCount.Value := g_Config.nSkill202LevelupCount;
  seSkill202CD.Value := g_Config.nSkill202CD;
  seSkill202Rate.Value := g_Config.nSkill202Rate;
  chkSkill202RateOnlyMon.Checked := g_Config.boSkill202RateOnlyMon;

  seSkillFireCharmPowerRate.Value := g_Config.nSkillFireCharmPowerRate;
  seSkill202PowerRate.Value := g_Config.nSkill202PowerRate;

  seSkill202PowerDec.Value := g_Config.nSkill202PowerDec;
  seSkill202PowerMin.Value := g_Config.nSkill202PowerMin;

  // 死亡之眼
  seSkill203PoisonRate.Value := g_Config.nSkill203PoisonRate;
  seSkill203BasicMbTimer.Value := g_Config.nSkill203BasicMbTimer;
  seSkill203LevelupMbTimer.Value := g_Config.nSkill203LevelupMbTimer;
  seSkill203BasicPowerRate.Value := g_Config.nSkill203BasicPowerRate;
  seSkill203LevelupPowerRate.Value := g_Config.nSkill203LevelupPowerRate;
  chkSkill203MbAttackMon.Checked := g_Config.boSkill203MbAttackMon;
  chkSkill203MbAttackHuman.Checked := g_Config.boSkill203MbAttackHuman;
  chkSkill203MbAttackSlave.Checked := g_Config.boSkill203MbAttackSlave;
  chkSkill203Damagearmor.Checked := g_Config.boSkill203Damagearmor;
  chkSkill203DecHealth.Checked := g_Config.boSkill203DecHealth;
  chkSkill203MbFastParalysis.Checked := g_Config.boSkill203MbFastParalysis;
  seSkill203CD.Value := g_Config.nSkill203CD;
  seSkill203Rage.Value := g_Config.nSkill203Rage;

  // 十步一杀
  seSkill204BasicMbTimer.Value := g_Config.nSkill204BasicMbTimer;
  seSkill204BasicMbRate.Value := g_Config.nSkill204BasicMbRate;
  seSkill204LevelupMbTimer.Value := g_Config.nSkill204LevelupMbTimer;
  seSkill204BasicPowerRate.Value := g_Config.nSkill204BasicPowerRate;
  seSkill204LevelupPowerRate.Value := g_Config.nSkill204LevelupPowerRate;
  chkSkill204MbAttackMon.Checked := g_Config.boSkill204MbAttackMon;
  chkSkill204MbAttackHuman.Checked := g_Config.boSkill204MbAttackHuman;
  chkSkill204MbAttackSlave.Checked := g_Config.boSkill204MbAttackSlave;
  chkSkill204MbFastParalysis.Checked := g_Config.boSkill204MbFastParalysis;
  chkSkill204RunHum.Checked := g_Config.boSkill204RunHum;
  chkSkill204RunMon.Checked := g_Config.boSkill204RunMon;
  chkSkill204RunNpc.Checked := g_Config.boSkill204RunNpc;
  chkSkill204RunGuard.Checked := g_Config.boSkill204RunGuard;
  chkSkill204RunObstacle.Checked := g_Config.boSkill204RunObstacle;

  chkSkill204WarDisHumRun.Checked := g_Config.boSkill204WarDisHumRun;
  seSkill204CD.Value := g_Config.nSkill204CD;
  seSkill204Rage.Value := g_Config.nSkill204Rage;
  seSkill204Distance.Value := g_Config.nSkill204Distance;
  chkSkill204SameLevel.Checked := g_Config.boSkill204SameLevel;
  chkSkill204DisableStopItem.Checked := g_Config.boSkill204DisableStopItem;

  // 冰霜雪雨
  chkSkill205ReduceMP.Checked := g_Config.boSkill205ReduceMP;
  seSkill205CD.Value := g_Config.nSkill205CD;
  seSkill205Rage.Value := g_Config.nSkill205Rage;
  seSkill205PowerRate.Value := g_Config.nSkill205PowerRate;
  chkSkill205PowerTwoAttack.Checked := g_Config.boSkill205PowerTwoAttack;

  // 冰霜群雨
  seSkill206BasicMbTimer.Value := g_Config.nSkill206BasicMbTimer;
  seSkill206LevelupMbTimer.Value := g_Config.nSkill206LevelupMbTimer;
  seSkill206BasicPowerRate.Value := g_Config.nSkill206BasicPowerRate;
  seSkill206LevelupPowerRate.Value := g_Config.nSkill206LevelupPowerRate;
  chkSkill206MbAttackMon.Checked := g_Config.boSkill206MbAttackMon;
  chkSkill206MbAttackHuman.Checked := g_Config.boSkill206MbAttackHuman;
  chkSkill206MbAttackSlave.Checked := g_Config.boSkill206MbAttackSlave;
  chkSkill206MbFastParalysis.Checked := g_Config.boSkill206MbFastParalysis;
  seSkill206CD.Value := g_Config.nSkill206CD;
  seSkill206Rage.Value := g_Config.nSkill206Rage;
  chkSkill206SameLevel.Checked := g_Config.boSkill206SameLevel;
  chkSkill206Frozen.Checked := g_Config.boSkill206Frozen;
  seSkill206FrozenRate.Value := g_Config.nSkill206FrozenRate;
  seSkill206FrozenRate.Enabled := g_Config.boSkill206Frozen;

  // 旋风斩 piaoyun 2013-09-15
  seSKILL208CD.Value := g_Config.nSkill208CD;
  seSkill208Rage.Value := g_Config.nSkill208Rage;
  seSkill208PowerRate.Value := g_Config.nSkill208PowerRate;
  chkSKILL208HeroDuanJin.Checked := g_Config.boSKILL208HeroDuanJin;
  chkSKILL208PlayMosterDuanJin.Checked := g_Config.boSKILL208PlayMosterDuanJin;

  // 五雷轰 piaoyun 2013-09-14
  seSKILL209CD.Value := g_Config.nSkill209CD;
  seSkill209Rage.Value := g_Config.nSkill209Rage;
  seSkill209PowerRate.Value := g_Config.nSkill209PowerRate;

  // 幽冥火符 piaoyun 2013-09-14
  seSKILL210CD.Value := g_Config.nSkill210CD;
  seSkill210Rage.Value := g_Config.nSkill210Rage;
  seSkill210PowerRate.Value := g_Config.nSkill210PowerRate;

  seSkill37Range.Value := g_Config.nSkill37Range;
  seSkill37RangeAdd.Value := g_Config.nSkill37RangeAdd;

  seSkillLighteningPowerRate.Value := g_Config.nSkillLighteningPowerRate;
  seSkillGroupLighteningPowerRate.Value := g_Config.nSkillGroupLighteningPowerRate;

  seNewLevelMagicPowerRatesAfter9.Value := g_Config.NewLevelMagicPowerRatesAfter9;
  // /////////////////////////////////////////////////////////
end;

procedure TfrmFunctionConfig.EditBoneFammCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBoneFammCount := EditBoneFammCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditDogzCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDogzCount := EditDogzCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxLimitSwordLongClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLimitSwordLong := CheckBoxLimitSwordLong.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSwordLongPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSwordLongPowerRate := EditSwordLongPowerRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.EditBoneFammNameChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmFunctionConfig.EditDogzNameChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmFunctionConfig.seFireBoomRageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFireBoomRage := seFireBoomRage.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSnowWindRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSnowWindRange := seSnowWindRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seElecBlizzardRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nElecBlizzardRange := seElecBlizzardRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMagTurnUndeadLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagTurnUndeadLevel := seMagTurnUndeadLevel.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.GridBoneFammSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmFunctionConfig.EdiAmyOunsulPointChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAmyOunsulPoint := EditAmyOunsulPoint.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxFireCrossInSafeZoneClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableInSafeZoneFireCross := CheckBoxFireCrossInSafeZone.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill41MbAttackPlayObjectClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill41MbAttackPlayObject := chkSkill41MbAttackPlayObject.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.ButtonSkillSaveClick(Sender: TObject);
var
  I, II: Integer;
  RecallArray: array[0..9] of TRecallMigic;
  Rect: TGridRect;
  StrSection: string;
begin
  FillChar(RecallArray, SizeOf(RecallArray), #0);

  g_Config.sBoneFamm := Trim(EditBoneFammName.Text);
  g_Config.sDogz := Trim(EditDogzName.Text);
  g_Config.sBigDogz := Trim(EditBigDogzName.Text);
  g_Config.sMonthSpirit := Trim(edtMonthSpiritName.Text);

  g_Config.sPlusBoneFammName1_3 := Trim(edtPlusBoneFammName1_3.Text);
  g_Config.sPlusBoneFammName4_6 := Trim(edtPlusBoneFammName4_6.Text);
  g_Config.sPlusBoneFammName7_9 := Trim(edtPlusBoneFammName7_9.Text);
  g_Config.sPlusBoneFammName9_N := Trim(edtPlusBoneFammName9_N.Text);

  g_Config.sPlusDogzName1_3 := Trim(edtPlusDogzName1_3.Text);
  g_Config.sPlusDogzName4_6 := Trim(edtPlusDogzName4_6.Text);
  g_Config.sPlusDogzName7_9 := Trim(edtPlusDogzName7_9.Text);
  g_Config.sPlusDogzName9_N := Trim(edtPlusDogzName9_N.Text);

  g_Config.dwPlusBoneFammLevels[0] := sePlusBoneFammLevel1.Value;
  g_Config.dwPlusBoneFammLevels[1] := sePlusBoneFammLevel2.Value;
  g_Config.dwPlusBoneFammLevels[2] := sePlusBoneFammLevel3.Value;
  g_Config.dwPlusBoneFammLevels[3] := sePlusBoneFammLevel4.Value;
  g_Config.dwPlusBoneFammLevels[4] := sePlusBoneFammLevel5.Value;
  g_Config.dwPlusBoneFammLevels[5] := sePlusBoneFammLevel6.Value;
  g_Config.dwPlusBoneFammLevels[6] := sePlusBoneFammLevel7.Value;
  g_Config.dwPlusBoneFammLevels[7] := sePlusBoneFammLevel8.Value;
  g_Config.dwPlusBoneFammLevels[8] := sePlusBoneFammLevel9.Value;
  g_Config.dwPlusBoneFammAddLevelAfter9 := sePlusBoneFammLevelAfter9.Value;

  g_Config.dwPlusDogzLevels[0] := sePlusDogzLevel1.Value;
  g_Config.dwPlusDogzLevels[1] := sePlusDogzLevel2.Value;
  g_Config.dwPlusDogzLevels[2] := sePlusDogzLevel3.Value;
  g_Config.dwPlusDogzLevels[3] := sePlusDogzLevel4.Value;
  g_Config.dwPlusDogzLevels[4] := sePlusDogzLevel5.Value;
  g_Config.dwPlusDogzLevels[5] := sePlusDogzLevel6.Value;
  g_Config.dwPlusDogzLevels[6] := sePlusDogzLevel7.Value;
  g_Config.dwPlusDogzLevels[7] := sePlusDogzLevel8.Value;
  g_Config.dwPlusDogzLevels[8] := sePlusDogzLevel9.Value;
  g_Config.dwPlusDogzAddLevelAfter9 := sePlusDogzLevelAfter9.Value;

  if UserEngine.GetMonRace(g_Config.sBoneFamm) <= 0 then
  begin
    Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);
    begin
      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet21;

      if EditBoneFammName.CanFocus then
        EditBoneFammName.SetFocus;
    end;
    Exit;
  end;

  if UserEngine.GetMonRace(g_Config.sDogz) <= 0 then
  begin
    Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);
    FunctionConfigControl.ActivePage := TabSheet1;
    MagicPageControl.ActivePage := TabSheet64;
    PageControl6.ActivePage := TabSheet22;
    if EditDogzName.CanFocus then
      EditDogzName.SetFocus;
    Exit;
  end;

  if UserEngine.GetMonRace(g_Config.sPlusBoneFammName1_3) <= 0 then
  begin
    Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);
    FunctionConfigControl.ActivePage := TabSheet1;
    MagicPageControl.ActivePage := TabSheet64;
    PageControl6.ActivePage := TabSheet21;
    if edtPlusBoneFammName1_3.CanFocus then
      edtPlusBoneFammName1_3.SetFocus;
    Exit;
  end;

  if UserEngine.GetMonRace(g_Config.sPlusBoneFammName4_6) <= 0 then
  begin
    Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);
    FunctionConfigControl.ActivePage := TabSheet1;
    MagicPageControl.ActivePage := TabSheet64;
    PageControl6.ActivePage := TabSheet21;
    if edtPlusBoneFammName4_6.CanFocus then
      edtPlusBoneFammName4_6.SetFocus;
    Exit;
  end;

  if UserEngine.GetMonRace(g_Config.sPlusBoneFammName7_9) <= 0 then
  begin
    Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);

    FunctionConfigControl.ActivePage := TabSheet1;
    MagicPageControl.ActivePage := TabSheet64;
    PageControl6.ActivePage := TabSheet21;

    if edtPlusBoneFammName7_9.CanFocus then
      edtPlusBoneFammName7_9.SetFocus;
    Exit;
  end;

  if UserEngine.GetMonRace(g_Config.sPlusDogzName1_3) <= 0 then
  begin
    Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);

    FunctionConfigControl.ActivePage := TabSheet1;
    MagicPageControl.ActivePage := TabSheet64;
    PageControl6.ActivePage := TabSheet22;

    if edtPlusDogzName1_3.CanFocus then
      edtPlusDogzName1_3.SetFocus;
    Exit;
  end;

  if UserEngine.GetMonRace(g_Config.sPlusDogzName4_6) <= 0 then
  begin
    Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);

    FunctionConfigControl.ActivePage := TabSheet1;
    MagicPageControl.ActivePage := TabSheet64;
    PageControl6.ActivePage := TabSheet22;

    if edtPlusDogzName4_6.CanFocus then
      edtPlusDogzName4_6.SetFocus;
    Exit;
  end;

  if UserEngine.GetMonRace(g_Config.sPlusDogzName7_9) <= 0 then
  begin
    Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);

    FunctionConfigControl.ActivePage := TabSheet1;
    MagicPageControl.ActivePage := TabSheet64;
    PageControl6.ActivePage := TabSheet22;

    if edtPlusDogzName7_9.CanFocus then
      edtPlusDogzName7_9.SetFocus;
    Exit;
  end;

  for I := Low(RecallArray) to High(RecallArray) do
  begin
    RecallArray[I].nHumLevel := StrToIntDef(GridBoneFamm.Cells[0, I + 1], -1);
    RecallArray[I].sMonName := Trim(GridBoneFamm.Cells[1, I + 1]);
    RecallArray[I].nCount := StrToIntDef(GridBoneFamm.Cells[2, I + 1], -1);
    RecallArray[I].nLevel := StrToIntDef(GridBoneFamm.Cells[3, I + 1], -1);
    if GridBoneFamm.Cells[0, I + 1] = '' then
      Break;
    if (RecallArray[I].nHumLevel <= 0) then
    begin
      Application.MessageBox('人物等级设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet21;

      Rect.Left := 0;
      Rect.Top := I + 1;
      Rect.Right := 0;
      Rect.Bottom := I + 1;
      GridBoneFamm.Selection := Rect;
      Exit;
    end;
    if UserEngine.GetMonRace(RecallArray[I].sMonName) <= 0 then
    begin
      Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet21;

      Rect.Left := 1;
      Rect.Top := I + 1;
      Rect.Right := 1;
      Rect.Bottom := I + 1;
      GridBoneFamm.Selection := Rect;
      Exit;
    end;
    if RecallArray[I].nCount <= 0 then
    begin
      Application.MessageBox('召唤数量设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet21;

      Rect.Left := 2;
      Rect.Top := I + 1;
      Rect.Right := 2;
      Rect.Bottom := I + 1;
      GridBoneFamm.Selection := Rect;
      Exit;
    end;
    if RecallArray[I].nLevel < 0 then
    begin
      Application.MessageBox('召唤等级设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet21;

      Rect.Left := 3;
      Rect.Top := I + 1;
      Rect.Right := 3;
      Rect.Bottom := I + 1;
      GridBoneFamm.Selection := Rect;
      Exit;
    end;
  end;

  for I := Low(RecallArray) to High(RecallArray) do
  begin
    RecallArray[I].nHumLevel := StrToIntDef(GridDogz.Cells[0, I + 1], -1);
    RecallArray[I].sMonName := Trim(GridDogz.Cells[1, I + 1]);
    RecallArray[I].nCount := StrToIntDef(GridDogz.Cells[2, I + 1], -1);
    RecallArray[I].nLevel := StrToIntDef(GridDogz.Cells[3, I + 1], -1);
    if GridDogz.Cells[0, I + 1] = '' then
      Break;
    if (RecallArray[I].nHumLevel <= 0) then
    begin
      Application.MessageBox('人物等级设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet22;

      Rect.Left := 0;
      Rect.Top := I + 1;
      Rect.Right := 0;
      Rect.Bottom := I + 1;
      GridDogz.Selection := Rect;
      Exit;
    end;
    if UserEngine.GetMonRace(RecallArray[I].sMonName) <= 0 then
    begin
      Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet22;

      Rect.Left := 1;
      Rect.Top := I + 1;
      Rect.Right := 1;
      Rect.Bottom := I + 1;
      GridDogz.Selection := Rect;
      Exit;
    end;
    if RecallArray[I].nCount <= 0 then
    begin
      Application.MessageBox('召唤数量设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet22;

      Rect.Left := 2;
      Rect.Top := I + 1;
      Rect.Right := 2;
      Rect.Bottom := I + 1;
      GridDogz.Selection := Rect;
      Exit;
    end;
    if RecallArray[I].nLevel < 0 then
    begin
      Application.MessageBox('召唤等级设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet22;

      Rect.Left := 3;
      Rect.Top := I + 1;
      Rect.Right := 3;
      Rect.Bottom := I + 1;
      GridDogz.Selection := Rect;
      Exit;
    end;
  end;

  for I := Low(RecallArray) to High(RecallArray) do
  begin
    RecallArray[I].nHumLevel := StrToIntDef(GridBigDogz.Cells[0, I + 1], -1);
    RecallArray[I].sMonName := Trim(GridBigDogz.Cells[1, I + 1]);
    RecallArray[I].nCount := StrToIntDef(GridBigDogz.Cells[2, I + 1], -1);
    RecallArray[I].nLevel := StrToIntDef(GridBigDogz.Cells[3, I + 1], -1);
    if GridBigDogz.Cells[0, I + 1] = '' then
      Break;
    if (RecallArray[I].nHumLevel <= 0) then
    begin
      Application.MessageBox('人物等级设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet12;

      Rect.Left := 0;
      Rect.Top := I + 1;
      Rect.Right := 0;
      Rect.Bottom := I + 1;
      MagicPageControl.ActivePageIndex := 3;
      PageControl6.ActivePageIndex := 9;
      GridBigDogz.Selection := Rect;
      Exit;
    end;
    if UserEngine.GetMonRace(RecallArray[I].sMonName) <= 0 then
    begin
      Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet12;

      Rect.Left := 1;
      Rect.Top := I + 1;
      Rect.Right := 1;
      Rect.Bottom := I + 1;
      MagicPageControl.ActivePageIndex := 3;
      PageControl6.ActivePageIndex := 9;
      GridBigDogz.Selection := Rect;
      Exit;
    end;
    if RecallArray[I].nCount <= 0 then
    begin
      Application.MessageBox('召唤数量设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet12;

      Rect.Left := 2;
      Rect.Top := I + 1;
      Rect.Right := 2;
      Rect.Bottom := I + 1;
      MagicPageControl.ActivePageIndex := 3;
      PageControl6.ActivePageIndex := 9;
      GridBigDogz.Selection := Rect;
      Exit;
    end;
    if RecallArray[I].nLevel < 0 then
    begin
      Application.MessageBox('召唤等级设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet12;

      Rect.Left := 3;
      Rect.Top := I + 1;
      Rect.Right := 3;
      Rect.Bottom := I + 1;
      MagicPageControl.ActivePageIndex := 3;
      PageControl6.ActivePageIndex := 9;
      GridBigDogz.Selection := Rect;
      Exit;
    end;
  end;

  for I := Low(RecallArray) to High(RecallArray) do
  begin
    RecallArray[I].nHumLevel := StrToIntDef(GridMonthSpirit.Cells[0, I + 1], -1);
    RecallArray[I].sMonName := Trim(GridMonthSpirit.Cells[1, I + 1]);
    RecallArray[I].nCount := StrToIntDef(GridMonthSpirit.Cells[2, I + 1], -1);
    RecallArray[I].nLevel := StrToIntDef(GridMonthSpirit.Cells[3, I + 1], -1);
    if GridMonthSpirit.Cells[0, I + 1] = '' then
      Break;
    if (RecallArray[I].nHumLevel <= 0) then
    begin
      Application.MessageBox('人物等级设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet30;

      Rect.Left := 0;
      Rect.Top := I + 1;
      Rect.Right := 0;
      Rect.Bottom := I + 1;
      MagicPageControl.ActivePageIndex := 3;
      PageControl6.ActivePageIndex := 5;
      GridMonthSpirit.Selection := Rect;
      Exit;
    end;
    if UserEngine.GetMonRace(RecallArray[I].sMonName) <= 0 then
    begin
      Application.MessageBox('怪物名称设置错误！！！' + sLineBreak + '道士技能宝宝名字在数据库中不存在', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet30;

      Rect.Left := 1;
      Rect.Top := I + 1;
      Rect.Right := 1;
      Rect.Bottom := I + 1;
      MagicPageControl.ActivePageIndex := 3;
      PageControl6.ActivePageIndex := 5;
      GridMonthSpirit.Selection := Rect;
      Exit;
    end;
    if RecallArray[I].nCount <= 0 then
    begin
      Application.MessageBox('召唤数量设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet30;

      Rect.Left := 2;
      Rect.Top := I + 1;
      Rect.Right := 2;
      Rect.Bottom := I + 1;
      MagicPageControl.ActivePageIndex := 3;
      PageControl6.ActivePageIndex := 5;
      GridMonthSpirit.Selection := Rect;
      Exit;
    end;
    if RecallArray[I].nLevel < 0 then
    begin
      Application.MessageBox('召唤等级设置错误！！！', '错误信息', MB_OK + MB_ICONERROR);

      FunctionConfigControl.ActivePage := TabSheet1;
      MagicPageControl.ActivePage := TabSheet64;
      PageControl6.ActivePage := TabSheet30;

      Rect.Left := 3;
      Rect.Top := I + 1;
      Rect.Right := 3;
      Rect.Bottom := I + 1;
      MagicPageControl.ActivePageIndex := 3;
      PageControl6.ActivePageIndex := 5;
      GridMonthSpirit.Selection := Rect;
      Exit;
    end;
  end;

  FillChar(g_Config.BoneFammArray, SizeOf(g_Config.BoneFammArray), #0);
  for I := Low(g_Config.BoneFammArray) to High(g_Config.BoneFammArray) do
  begin
    Config.WriteInteger('Setup', 'BoneFammHumLevel' + IntToStr(I), 0);
    Config.WriteString('Names', 'BoneFamm' + IntToStr(I), '');
    Config.WriteInteger('Setup', 'BoneFammCount' + IntToStr(I), 0);
    Config.WriteInteger('Setup', 'BoneFammLevel' + IntToStr(I), 0);
  end;
  for I := Low(g_Config.BoneFammArray) to High(g_Config.BoneFammArray) do
  begin
    if GridBoneFamm.Cells[0, I + 1] = '' then
      Break;
    g_Config.BoneFammArray[I].nHumLevel := StrToIntDef(GridBoneFamm.Cells[0, I + 1], -1);
    g_Config.BoneFammArray[I].sMonName := Trim(GridBoneFamm.Cells[1, I + 1]);
    g_Config.BoneFammArray[I].nCount := StrToIntDef(GridBoneFamm.Cells[2, I + 1], -1);
    g_Config.BoneFammArray[I].nLevel := StrToIntDef(GridBoneFamm.Cells[3, I + 1], -1);

    Config.WriteInteger('Setup', 'BoneFammHumLevel' + IntToStr(I), g_Config.BoneFammArray[I].nHumLevel);
    Config.WriteString('Names', 'BoneFamm' + IntToStr(I), g_Config.BoneFammArray[I].sMonName);
    Config.WriteInteger('Setup', 'BoneFammCount' + IntToStr(I), g_Config.BoneFammArray[I].nCount);
    Config.WriteInteger('Setup', 'BoneFammLevel' + IntToStr(I), g_Config.BoneFammArray[I].nLevel);
  end;

  FillChar(g_Config.DogzArray, SizeOf(g_Config.DogzArray), #0);
  for I := Low(g_Config.DogzArray) to High(g_Config.DogzArray) do
  begin
    Config.WriteInteger('Setup', 'DogzHumLevel' + IntToStr(I), 0);
    Config.WriteString('Names', 'Dogz' + IntToStr(I), '');
    Config.WriteInteger('Setup', 'DogzCount' + IntToStr(I), 0);
    Config.WriteInteger('Setup', 'DogzLevel' + IntToStr(I), 0);
  end;
  for I := Low(g_Config.DogzArray) to High(g_Config.DogzArray) do
  begin
    if GridDogz.Cells[0, I + 1] = '' then
      Break;

    g_Config.DogzArray[I].nHumLevel := StrToIntDef(GridDogz.Cells[0, I + 1], -1);
    g_Config.DogzArray[I].sMonName := Trim(GridDogz.Cells[1, I + 1]);
    g_Config.DogzArray[I].nCount := StrToIntDef(GridDogz.Cells[2, I + 1], -1);
    g_Config.DogzArray[I].nLevel := StrToIntDef(GridDogz.Cells[3, I + 1], -1);

    Config.WriteInteger('Setup', 'DogzHumLevel' + IntToStr(I), g_Config.DogzArray[I].nHumLevel);
    Config.WriteString('Names', 'Dogz' + IntToStr(I), g_Config.DogzArray[I].sMonName);
    Config.WriteInteger('Setup', 'DogzCount' + IntToStr(I), g_Config.DogzArray[I].nCount);
    Config.WriteInteger('Setup', 'DogzLevel' + IntToStr(I), g_Config.DogzArray[I].nLevel);
  end;

  FillChar(g_Config.BigDogzArray, SizeOf(g_Config.BigDogzArray), #0);
  for I := Low(g_Config.BigDogzArray) to High(g_Config.BigDogzArray) do
  begin
    Config.WriteInteger('Setup', 'BigDogzHumLevel' + IntToStr(I), 0);
    Config.WriteString('Names', 'BigDogz' + IntToStr(I), '');
    Config.WriteInteger('Setup', 'BigDogzCount' + IntToStr(I), 0);
    Config.WriteInteger('Setup', 'BigDogzLevel' + IntToStr(I), 0);
  end;
  for I := Low(g_Config.BigDogzArray) to High(g_Config.BigDogzArray) do
  begin
    if GridBigDogz.Cells[0, I + 1] = '' then
      Break;

    g_Config.BigDogzArray[I].nHumLevel := StrToIntDef(GridBigDogz.Cells[0, I + 1], -1);
    g_Config.BigDogzArray[I].sMonName := Trim(GridBigDogz.Cells[1, I + 1]);
    g_Config.BigDogzArray[I].nCount := StrToIntDef(GridBigDogz.Cells[2, I + 1], -1);
    g_Config.BigDogzArray[I].nLevel := StrToIntDef(GridBigDogz.Cells[3, I + 1], -1);

    Config.WriteInteger('Setup', 'BigDogzHumLevel' + IntToStr(I), g_Config.BigDogzArray[I].nHumLevel);
    Config.WriteString('Names', 'BigDogz' + IntToStr(I), g_Config.BigDogzArray[I].sMonName);
    Config.WriteInteger('Setup', 'BigDogzCount' + IntToStr(I), g_Config.BigDogzArray[I].nCount);
    Config.WriteInteger('Setup', 'BigDogzLevel' + IntToStr(I), g_Config.BigDogzArray[I].nLevel);
  end;

  FillChar(g_Config.MonthSpiritArray, SizeOf(g_Config.MonthSpiritArray), #0);
  for I := Low(g_Config.MonthSpiritArray) to High(g_Config.MonthSpiritArray) do
  begin
    Config.WriteInteger('Setup', 'MonthSpiritHumLevel' + IntToStr(I), 0);
    Config.WriteString('Names', 'MonthSpirit' + IntToStr(I), '');
    Config.WriteInteger('Setup', 'MonthSpiritCount' + IntToStr(I), 0);
    Config.WriteInteger('Setup', 'MonthSpiritLevel' + IntToStr(I), 0);
  end;
  for I := Low(g_Config.MonthSpiritArray) to High(g_Config.MonthSpiritArray) do
  begin
    if GridMonthSpirit.Cells[0, I + 1] = '' then
      Break;

    g_Config.MonthSpiritArray[I].nHumLevel := StrToIntDef(GridMonthSpirit.Cells[0, I + 1], -1);
    g_Config.MonthSpiritArray[I].sMonName := Trim(GridMonthSpirit.Cells[1, I + 1]);
    g_Config.MonthSpiritArray[I].nCount := StrToIntDef(GridMonthSpirit.Cells[2, I + 1], -1);
    g_Config.MonthSpiritArray[I].nLevel := StrToIntDef(GridMonthSpirit.Cells[3, I + 1], -1);

    Config.WriteInteger('Setup', 'MonthSpiritHumLevel' + IntToStr(I), g_Config.MonthSpiritArray[I].nHumLevel);
    Config.WriteString('Names', 'MonthSpirit' + IntToStr(I), g_Config.MonthSpiritArray[I].sMonName);
    Config.WriteInteger('Setup', 'MonthSpiritCount' + IntToStr(I), g_Config.MonthSpiritArray[I].nCount);
    Config.WriteInteger('Setup', 'MonthSpiritLevel' + IntToStr(I), g_Config.MonthSpiritArray[I].nLevel);
  end;

  Config.WriteBool('Setup', 'LimitSwordLong', g_Config.boLimitSwordLong);
  Config.WriteInteger('Setup', 'SwordLongPowerRate', g_Config.nSwordLongPowerRate);
  Config.WriteInteger('Setup', 'SkillYedoPowerRate', g_Config.nSkillYedoPowerRate);

  Config.WriteInteger('Setup', 'BoneFammCount', g_Config.nBoneFammCount);
  Config.WriteString('Names', 'BoneFamm', g_Config.sBoneFamm);
  Config.WriteInteger('Setup', 'DogzCount', g_Config.nDogzCount);
  Config.WriteString('Names', 'Dogz', g_Config.sDogz);
  Config.WriteInteger('Setup', 'BigDogzCount', g_Config.nBigDogzCount);
  Config.WriteString('Names', 'BigDogz', g_Config.sBigDogz);

  Config.WriteString('Names', 'MonthSpirit', g_Config.sMonthSpirit);
  Config.WriteInteger('Setup', 'MonthSpiritCount', g_Config.nMonthSpiritCount);
  Config.WriteInteger('Setup', 'MonthSpiritAttackRange', g_Config.nMonthSpiritAttackRange);

  // 爆裂火焰 piaoyun 2013-07-25
  Config.WriteInteger('Setup', 'FireBoomRage', g_Config.nFireBoomRage);
  Config.WriteInteger('Setup', 'FireBoomRagePowerRate', g_Config.nFireBoomRagePowerRate);

  // 冰咆哮 piaoyun 2013-07-25
  Config.WriteInteger('Setup', 'SnowWindRange', g_Config.nSnowWindRange);
  Config.WriteInteger('Setup', 'SnowWindPowerRate', g_Config.nSnowWindPowerRate);
  Config.WriteInteger('Setup', 'SnowwindWaitTime', g_Config.nSnowwindWaitTime);

  // 地狱雷光 piaoyun 2013-07-25
  Config.WriteInteger('Setup', 'ElecBlizzardRange', g_Config.nElecBlizzardRange);
  Config.WriteInteger('Setup', 'ElecBlizzardPowerRate', g_Config.nElecBlizzardPowerRate);

  Config.WriteInteger('Setup', 'AmyOunsulPoint', g_Config.nAmyOunsulPoint);
  Config.WriteInteger('Setup', 'AmyOunsulTimeRate', g_Config.nAmyOunsulTimeRate);
  Config.WriteInteger('Setup', 'AmyOunsulMaxTime', g_Config.nAmyOunsulMaxTime);
  Config.WriteBool('Setup', 'ShowYouPoisoned', g_Config.boShowYouPoisoned);

  Config.WriteInteger('Setup', 'SkillAmyounsulCDTime', g_Config.nSkillAmyounsulCDTime);
  Config.WriteInteger('Setup', 'SkillGroupAmyounsulCDTime', g_Config.nSkillGroupAmyounsulCDTime);
  Config.WriteBool('Setup', 'SkillGroupAmyounsulRed', g_Config.boSkillGroupAmyounsulRed);
  Config.WriteBool('Setup', 'SkillGroupAmyounsulGreen', g_Config.boSkillGroupAmyounsulGreen);

  Config.WriteBool('Setup', 'ViewRangeCanMagicAttack', g_Config.boViewRangeCanMagicAttack);
  Config.WriteBool('Setup', 'ShowMsgMagicRangeExceed', g_Config.boShowMsgMagicRangeExceed);
  Config.WriteInteger('Setup', 'MagicAttackRage', g_Config.nMagicAttackRage);
  Config.WriteInteger('Setup', 'MagTurnUndeadLevel', g_Config.nMagTurnUndeadLevel);
  Config.WriteBool('Setup', 'MagTurnUndeadSameLevel', g_Config.boMagTurnUndeadSameLevel);
  Config.WriteInteger('Setup', 'MagTammingLevel', g_Config.nMagTammingLevel);
  Config.WriteInteger('Setup', 'MagTammingTargetLevel', g_Config.nMagTammingTargetLevel);
  Config.WriteInteger('Setup', 'MagTammingTargetHPRate', g_Config.nMagTammingHPRate);
  Config.WriteInteger('Setup', 'MagTammingCount', g_Config.nMagTammingCount);
  Config.WriteInteger('Setup', 'MasterRoyaltyTime', g_Config.nMasterRoyaltyTime);

  Config.WriteInteger('Setup', 'MabMabeHitRandRate', g_Config.nMabMabeHitRandRate);
  Config.WriteInteger('Setup', 'MabMabeHitMinLvLimit', g_Config.nMabMabeHitMinLvLimit);
  Config.WriteInteger('Setup', 'MabMabeHitSucessRate', g_Config.nMabMabeHitSucessRate);
  Config.WriteInteger('Setup', 'MabMabeHitMabeTimeRate', g_Config.nMabMabeHitMabeTimeRate);
  Config.WriteInteger('Setup', 'MaxMabMabeHitMabeTime', g_Config.nMaxMabMabeHitMabeTime);

  // 魔法盾防御倍率 piaoyun 2013-08-18
  Config.WriteInteger('Setup', 'OrdinarySkill31Rate', g_Config.nOrdinarySkill31Rate);

  // 强化魔法盾防御倍率 piaoyun 2013-08-18
  for I := Low(g_Config.nSkill31Rates) to High(g_Config.nSkill31Rates) do
  begin
    Config.WriteInteger('Setup', 'Skill31Rate' + IntToStr(I + 1), g_Config.nSkill31Rates[I]);
  end;

  Config.WriteBool('Setup', 'Skill31UseNewEffect', g_Config.boSkill31UseNewEffect);
  Config.WriteBool('Setup', 'Skill31UseLEGEffect', g_Config.boSkill31UseLEGEffect);
  Config.WriteInteger('Setup', 'Skill113RateAddWithSkill63', g_Config.nSkill113RateAddWithSkill63);

  Config.WriteBool('Setup', 'DisableInSafeZoneFireCross', g_Config.boDisableInSafeZoneFireCross);
  Config.WriteBool('Setup', 'DisableChangeMapFireCross', g_Config.boDisableChangeMapFireCross);
  Config.WriteInteger('Setup', 'FireCrossMaxTime', g_Config.nFireCrossMaxTime);
  Config.WriteInteger('Setup', 'FireCrossPowerRate', g_Config.nFireCrossPowerRate);

  Config.WriteBool('Setup', 'Skill41MbAttackPlayObject', g_Config.boSkill41MbAttackPlayObject);
  Config.WriteBool('Setup', 'DisableSkill41MbSameLevel', g_Config.boDisableSkill41MbSameLevel);
  Config.WriteBool('Setup', 'Skill41MbAttackSlave', g_Config.boSkill41MbAttackSlave);
  Config.WriteInteger('Setup', 'Skill41CD', g_Config.nSkill41CD);

  for I := Low(g_Config.nSkill41MbTimers) to High(g_Config.nSkill41MbTimers) do
  begin
    Config.WriteInteger('Setup', 'Skill41MbTimers' + IntToStr(I), g_Config.nSkill41MbTimers[I]);
  end;

  for I := Low(g_Config.nSkill41MbRanges) to High(g_Config.nSkill41MbRanges) do
  begin
    Config.WriteInteger('Setup', 'Skill41MbRanges' + IntToStr(I), g_Config.nSkill41MbRanges[I]);
  end;

  Config.WriteBool('Setup', 'Skill71PullPlayObject', g_Config.boSkill71PullPlayObject);
  Config.WriteBool('Setup', 'Skill71PullSlave', g_Config.boSkill71PullSlave);
  Config.WriteBool('Setup', 'Skill71PullCrossInSafeZone', g_Config.boSkill71PullCrossInSafeZone);
  Config.WriteBool('Setup', 'Skill71DisableAttackSameLevel', g_Config.boSkill71DisableAttackSameLevel);
  Config.WriteBool('Setup', 'Skill71DisableAttackFriend', g_Config.boSkill71DisableAttackFriend);

  // 灭天火 piaoyun 2013-07-25
  Config.WriteBool('Setup', 'DamageMP', g_Config.boPlayObjectReduceMP);
  Config.WriteInteger('Setup', 'MakeFireDayPowerRate', g_Config.nMakeFireDayPowerRate);
  Config.WriteInteger('Setup', 'MakeFireDayTime', g_Config.nMakeFireDayTime);

  Config.WriteInteger('Setup', 'MagicValidTimeRate', g_Config.nMagDelayTimeDoubly);
  // Config.WriteInteger('Setup', 'MagicPowerRate', g_Config.nMagPowerDoubly);
  Config.WriteInteger('Setup', 'NearAttackPowerRate', g_Config.nNearAttackPowerRate);

  // 彻地钉
  Config.WriteInteger('Setup', 'DedingMagicCD', g_Config.nDedingMagicCD);
  Config.WriteInteger('Setup', 'DeDingMagicAttackRange', g_Config.nDeDingMagicAttackRange);
  Config.WriteInteger('Setup', 'DeDingMagicBasicPowerRate', g_Config.nDeDingMagicBasicPowerRate);
  Config.WriteBool('Setup', 'DedingAllowPK', g_Config.boDedingAllowPK);
  Config.WriteBool('Setup', 'DedingDisabledPK', g_Config.boDedingDisabledPK);
  {
    Config.WriteInteger('Setup', 'WarrorAttackTime', g_Config.dwWarrorAttackTime);
    Config.WriteInteger('Setup', 'WizardAttackTime', g_Config.dwWizardAttackTime);
    Config.WriteInteger('Setup', 'TaoistAttackTime', g_Config.dwTaoistAttackTime); }

  Config.WriteBool('Setup', 'AllowReCallMobOtherHum', g_Config.boAllowReCallMobOtherHum);
  Config.WriteBool('Setup', 'NeedLevelHighTarget', g_Config.boNeedLevelHighTarget);

  Config.WriteInteger('Setup', 'Skill60PowerRate', g_Config.nSkill60PowerRate);
  Config.WriteInteger('Setup', 'Skill60AttackHumPowerRate', g_Config.nSkill60AttackHumPowerRate);
  Config.WriteInteger('Setup', 'Skill60PowerRange', g_Config.nSkill60PowerRange);
  Config.WriteBool('Setup', 'Skill60NotMagBubbleDefence', g_Config.boSkill60NotMagBubbleDefence);

  Config.WriteInteger('Setup', 'Skill61PowerRate', g_Config.nSkill61PowerRate);
  Config.WriteInteger('Setup', 'Skill61AttackHumPowerRate', g_Config.nSkill61AttackHumPowerRate);
  Config.WriteBool('Setup', 'Skill61NotMagBubbleDefence', g_Config.boSkill61NotMagBubbleDefence);

  Config.WriteInteger('Setup', 'Skill62PowerRate', g_Config.nSkill62PowerRate);
  Config.WriteInteger('Setup', 'Skill62AttackHumPowerRate', g_Config.nSkill62AttackHumPowerRate);
  Config.WriteBool('Setup', 'Skill62NotMagBubbleDefence', g_Config.boSkill62NotMagBubbleDefence);

  Config.WriteInteger('Setup', 'Skill63PowerRate', g_Config.nSkill63PowerRate);
  Config.WriteInteger('Setup', 'Skill63AttackHumPowerRate', g_Config.nSkill63AttackHumPowerRate);
  Config.WriteInteger('Setup', 'Skill63PowerRange', g_Config.nSkill63PowerRange);
  Config.WriteBool('Setup', 'Skill63GreenPoison', g_Config.boSkill63GreenPoison);
  Config.WriteBool('Setup', 'Skill63UseSpeedPoint', g_Config.boSkill63UseSpeedPoint);
  Config.WriteInteger('Setup', 'SkillJointAttackLevelRate', g_Config.nSkillJointAttackLevelRate);

  Config.WriteInteger('Setup', 'Skill64PowerRate', g_Config.nSkill64PowerRate);
  Config.WriteInteger('Setup', 'Skill64AttackHumPowerRate', g_Config.nSkill64AttackHumPowerRate);
  Config.WriteInteger('Setup', 'Skill64PowerRange', g_Config.nSkill64PowerRange);
  Config.WriteBool('Setup', 'Skill64MakeStone', g_Config.boSkill64MakeStone);

  Config.WriteInteger('Setup', 'Skill65PowerRate', g_Config.nSkill65PowerRate);
  Config.WriteInteger('Setup', 'Skill65AttackHumPowerRate', g_Config.nSkill65AttackHumPowerRate);
  Config.WriteInteger('Setup', 'Skill65PowerRange', g_Config.nSkill65PowerRange);

  Config.WriteInteger('Setup', 'Skill56PowerRate', g_Config.nSkill56PowerRate);
  Config.WriteInteger('Setup', 'Skill58PowerRate', g_Config.nSkill58PowerRate);
  Config.WriteBool('Setup', 'Skill58PowerTwoAttack', g_Config.boSkill58PowerTwoAttack);
  Config.WriteInteger('Setup', 'Skill52PowerRate', g_Config.nSkill58PowerRate);

  Config.WriteInteger('Setup', 'AddAngryValue', g_Config.nAddAngryValue);
  Config.WriteInteger('Setup', 'DecFirDragonPoint', g_Config.nDecFirDragonPoint);
  Config.WriteInteger('Setup', 'AddAngryValueTime', g_Config.nAddAngryValueTime);

  if g_nKey_HeroExt = 1 then
  begin
    Config.WriteBool('Setup', 'NoNeedFirDragon', g_Config.boNoNeedFirDragon);
  end;

  Config.WriteInteger('Setup', 'FireHitWaitTime', g_Config.nFireHitWaitTime);
  Config.WriteInteger('Setup', 'FireHitPowerRate', g_Config.nFireHitPowerRate);
  Config.WriteInteger('Setup', 'MaxAngryValue', g_Config.btMaxAngryValue);

  Config.WriteInteger('Setup', 'SWordHitWaitTime', g_Config.nSWordHitWaitTime);

  // 龙影剑法 piaoyun 2013-07-25
  Config.WriteInteger('Setup', 'Skill42PowerRate', g_Config.nSkill42PowerRate);
  Config.WriteInteger('Setup', 'Skill42HitWaitTime', g_Config.nSkill42HitWaitTime);
  Config.WriteInteger('Setup', 'Skill42Range', g_Config.nSkill42Range);

  Config.WriteInteger('Setup', 'Skill25PowerRate', g_Config.nSkill25PowerRate);

  // 双龙斩 piaoyun 2013-07-25
  Config.WriteInteger('Setup', 'Skill40PowerRate', g_Config.nSkill40PowerRate);

  Config.WriteInteger('Setup', 'Skill69CD', g_Config.nSkill69CD);
  Config.WriteInteger('Setup', 'Skill69AddTime', g_Config.nSkill69AddTime);
  Config.WriteInteger('Setup', 'Skill69AddRange', g_Config.nSkill69AddRange);
  Config.WriteBool('Setup', 'Skill69SameLevel', g_Config.boSkill69SameLevel);

  Config.WriteInteger('Setup', 'Skill70CD', g_Config.nSkill70CD);

  Config.WriteInteger('Setup', 'Skill66HitWaitTime', g_Config.nSkill66CD);
  Config.WriteInteger('Setup', 'Skill71CD', g_Config.nSkill71CD);
  Config.WriteInteger('Setup', 'HeroSkill66HitWaitTime', g_Config.nHeroSkill66CD);

  // 乾坤大挪移 piaoyun 2013-08-24
  Config.WriteInteger('Setup', 'Skill72HitWaitTime', g_Config.nSkill72CD);
  Config.WriteInteger('Setup', 'Skill72Rate', g_Config.nSkill72Rate);
  Config.WriteInteger('Setup', 'Skill72LevelUpRateAdd', g_Config.nSkill72LevelUpRateAdd);
  Config.WriteBool('Setup', 'Skill72DisableStopItem', g_Config.boSkill72DisableStopItem);

  Config.WriteInteger('Setup', 'Skill57AddHPRate', g_Config.nSkill57AddHPRate);
  Config.WriteInteger('Setup', 'Skill57PowerRate', g_Config.nSkill57PowerRate);
  Config.WriteInteger('Setup', 'Skill57Time', g_Config.nSkill57Time);
  Config.WriteInteger('Setup', 'Skill52PowerRate', g_Config.nSkill52PowerRate);
  Config.WriteInteger('Setup', 'Skill52AttackRange', g_Config.nSkill52AttackRange);
  Config.WriteInteger('Setup', 'Skill58AttackRange', g_Config.nSkill58AttackRange);

  Config.WriteInteger('Setup', 'Skill46PowerBase', g_Config.nSkill46PowerBase);
  Config.WriteInteger('Setup', 'Skill46SecRate', g_Config.nSkill46SecRate);
  Config.WriteInteger('Setup', 'Skill46Time', g_Config.nSkill46Time);

  Config.WriteInteger('Setup', 'HumSkill66HighPowerRate', g_Config.nHumSkill66HighPowerRate);
  Config.WriteInteger('Setup', 'HeroSkill66HighPowerRate', g_Config.nHeroSkill66HighPowerRate);
  Config.WriteInteger('Setup', 'HeroSkill66PowerRate', g_Config.nHeroSkill66PowerRate);
  Config.WriteBool('Setup', 'HeroSkill66HighAttackNoUseRate', g_Config.boHeroSkill66HighAttackNoUseRate);
  Config.WriteInteger('Setup', 'HeroSkill66HighAttackRate', g_Config.nHeroSkill66HighAttackRate);

  Config.WriteInteger('Setup', 'MonthSpiritHighAttackRate', g_Config.nMonthSpiritHighAttackRate);
  Config.WriteInteger('Setup', 'MonthSpiritHighPowerRate', g_Config.nMonthSpiritHighPowerRate);
  Config.WriteInteger('Setup', 'RecallBigDogWaitTime', g_Config.nRecallBigDogWaitTime);
  Config.WriteBool('Setup', 'QigongPushSameLevel', g_Config.boQigongPushSameLevel);
  Config.WriteBool('Setup', 'FireWindPushSameLevel', g_Config.boFireWindPushSameLevel);
  Config.WriteBool('Setup', 'MonthSpiritAttackSame', g_Config.boMonthSpiritAttackSame);
  Config.WriteBool('Setup', 'MonthSpiritUseMasterMP', g_Config.boMonthSpiritUseMasterMP);
  Config.WriteInteger('Setup', 'Skill58WaitTime', g_Config.nSkill58WaitTime);
  Config.WriteInteger('Setup', 'HeroSkill58WaitTime', g_Config.nHeroSkill58WaitTime);

  Config.WriteInteger('Setup', 'Skill113PowerRate', g_Config.nSkill113PowerRate);
  Config.WriteInteger('Setup', 'Skill113CD', g_Config.nSkill113CD);
  Config.WriteInteger('Setup', 'HeroSkill113CD', g_Config.nHeroSkill113CD);

  Config.WriteInteger('Setup', 'Skill115PowerRate', g_Config.nSkill115PowerRate);
  Config.WriteInteger('Setup', 'Skill115CD', g_Config.nSkill115CD);
  Config.WriteInteger('Setup', 'HeroSkill115CD', g_Config.nHeroSkill115CD);
  Config.WriteInteger('Setup', 'Skill115LevelUpAddPowerRate', g_Config.nSkill115LevelUpAddPowerRate);

  Config.WriteInteger('Setup', 'Skill116PowerRate', g_Config.nSkill116PowerRate);
  Config.WriteInteger('Setup', 'Skill116CD', g_Config.nSkill116CD);
  Config.WriteInteger('Setup', 'HeroSkill116CD', g_Config.nHeroSkill116CD);
  Config.WriteInteger('Setup', 'Skill116Range', g_Config.nSkill116Range);
  Config.WriteInteger('Setup', 'Skill116LevelUpAddPowerRate', g_Config.nSkill116LevelUpAddPowerRate);

  Config.WriteInteger('Setup', 'Skill117PowerRate', g_Config.nSkill117PowerRate);
  Config.WriteInteger('Setup', 'Skill117CD', g_Config.nSkill117CD);
  Config.WriteInteger('Setup', 'HeroSkill117CD', g_Config.nHeroSkill117CD);
  Config.WriteInteger('Setup', 'Skill117Range', g_Config.nSkill117Range);
  Config.WriteInteger('Setup', 'Skill117LevelUpAddPowerRate', g_Config.nSkill117LevelUpAddPowerRate);

  Config.WriteBool('Setup', 'Skill115UseNG', g_Config.boSkill115UseNG);
  Config.WriteBool('Setup', 'Skill115NGNoEnoughDecHP', g_Config.boSkill115NGNoEnoughDecHP);
  Config.WriteInteger('Setup', 'Skill115NGNoEnoughDecHPValue', g_Config.nSkill115NGNoEnoughDecHPValue);
  Config.WriteInteger('Setup', 'Skill115NGNoEnoughDecHPType', g_Config.nSkill115NGNoEnoughDecHPType);

  Config.WriteInteger('Setup', 'CopySelfMaxCount', g_Config.nCopySelfMaxCount);
  Config.WriteInteger('Setup', 'CopySelfExistTime', g_Config.nCopySelfExistTime);
  Config.WriteInteger('Setup', 'CopySelfLevelUpAddExistTime', g_Config.nCopySelfLevelUpAddExistTime);
  Config.WriteBool('Setup', 'CopySelfNonUseSpellPoint', g_Config.boCopySelfNonUseSpellPoint);
  Config.WriteBool('Setup', 'AlwaysFollowMasterAttack', g_Config.boAlwaysFollowMasterAttack);

  Config.WriteInteger('Setup', 'CopySelfNameColor', g_Config.btCopySelfNameColor);
  Config.WriteString('Setup', 'CopySelfSuffix', g_Config.sCopySelfSuffix);
  Config.WriteBool('Setup', 'ShowCopySelfSuffix', g_Config.boShowCopySelfSuffix);

  Config.WriteInteger('Setup', 'MagicItemRate', g_Config.nMagicItemRate);
  Config.WriteInteger('Setup', 'HumNeedMagicItem', g_Config.nHumNeedMagicItem);

  for I := 0 to Length(g_Config.NewLevelMagicPowerRates) - 1 do
  begin
    Config.WriteInteger('Setup', 'NewLevelMagicPowerRates' + IntToStr(I), g_Config.NewLevelMagicPowerRates[I]);
  end;
  Config.WriteInteger('Setup', 'NewLevelMagicPowerRatesAfter9', g_Config.NewLevelMagicPowerRatesAfter9);

  Config.WriteInteger('Setup', 'SuperShiledValidTime', g_Config.nSuperShiledValidTime);
  Config.WriteInteger('Setup', 'SuperShiledLevelUpAddValidTime', g_Config.nSuperShiledLevelUpAddValidTime);
  Config.WriteInteger('Setup', 'LastSuperShiledTime', g_Config.nLastSuperShiledTime);
  Config.WriteInteger('Setup', 'SuperShiledPowerRate', g_Config.nSuperShiledPowerRate);
  Config.WriteInteger('Setup', 'SuperShiledLevelUpDecPowerRate', g_Config.nSuperShiledLevelUpDecPowerRate);
  Config.WriteInteger('Setup', 'OpenSuperShiledRate', g_Config.nOpenSuperShiledRate);
  Config.WriteInteger('Setup', 'OpenSuperShiledLevelUpAddRate', g_Config.nOpenSuperShiledLevelUpAddRate);
  Config.WriteInteger('Setup', 'CloseSuperShiledRate', g_Config.nCloseSuperShiledRate);
  Config.WriteInteger('Setup', 'CloseSuperShiledLevelUpDecRate', g_Config.nCloseSuperShiledLevelUpDecRate);
  Config.WriteBool('Setup', 'AutoOpenSuperShiled', g_Config.boAutoOpenSuperShiled);

  Config.WriteBool('Setup', 'ShowSuperShiledEffect', g_Config.boShowSuperShiledEffect);
  Config.WriteBool('Setup', 'ShowSuperShiledSound', g_Config.boShowSuperShiledSound);

  Config.WriteBool('Setup', 'ShowSuperShiledEffect2', g_Config.boShowSuperShiledEffect2);
  Config.WriteBool('Setup', 'ShowSuperShiledSound2', g_Config.boShowSuperShiledSound2);

  // 护体神盾提示 piaoyun 2013-07-26
  Config.WriteBool('Setup', 'CloseSuperShiledHint', g_Config.boCloseSuperShiledHint);

  for I := 0 to Length(g_Config.UseSkillCloseSuperShileds) - 1 do
  begin
    Config.WriteBool('Setup', 'UseSkillCloseSuperShileds' + IntToStr(I), g_Config.UseSkillCloseSuperShileds[I]);
  end;

  for I := 0 to Length(g_Config.UseSkillCloseSuperShileds_Rate) - 1 do
  begin
    Config.WriteInteger('Setup', 'UseSkillCloseSuperShileds_Rate' + IntToStr(I), g_Config.UseSkillCloseSuperShileds_Rate[I]);
  end;

  for I := 0 to Length(g_Config.UseSkillCloseSuperShileds_RateAdd) - 1 do
  begin
    Config.WriteInteger('Setup', 'UseSkillCloseSuperShileds_RateAdd' + IntToStr(I), g_Config.UseSkillCloseSuperShileds_RateAdd[I]);
  end;

  Config.WriteBool('Setup', 'RecallManySlave', g_Config.boRecallManySlave1);
  Config.WriteBool('Setup', 'RecallManySlave2', g_Config.boRecallManySlave2);
  Config.WriteBool('Setup', 'RecallManySlave3', g_Config.boRecallManySlave3);
  Config.WriteInteger('Setup', 'RecallMonCount', g_Config.dwRecallMonCount);
  Config.WriteInteger('Setup', 'MagicLockRange', g_Config.nMagicLockRange);

  Config.WriteInteger('Setup', 'HumSkill7PowerLV4', g_Config.dwHumSkill7PowerLV4);
  Config.WriteInteger('Setup', 'HumSkill13PowerLV4', g_Config.dwHumSkill13PowerLV4);
  Config.WriteInteger('Setup', 'HumSkill45PowerLV4', g_Config.dwHumSkill45PowerLV4);

  Config.WriteInteger('Setup', 'MagicFailMsgFColor', g_Config.btMagicFailMsgFColor);
  Config.WriteInteger('Setup', 'MagicFailMsgBColor', g_Config.btMagicFailMsgBColor);
  Config.WriteInteger('Setup', 'MagicOKMsgFColor', g_Config.btMagicOKMsgFColor);
  Config.WriteInteger('Setup', 'MagicOKMsgBColor', g_Config.btMagicOKMsgBColor);
  Config.WriteInteger('Setup', 'MagicMsgX', g_Config.nMagicMsgX);
  Config.WriteInteger('Setup', 'MagicMsgY', g_Config.nMagicMsgY);
  Config.WriteBool('Setup', 'MagicMsgXRightToLeft', g_Config.boMagicMsgXRightToLeft);
  Config.WriteBool('Setup', 'MagicMsgYBottomToTop', g_Config.boMagicMsgYBottomToTop);
  Config.WriteBool('Setup', 'MagicMsgAddChatBoardMsg', g_Config.boMagicMsgAddChatBoardMsg);

  Config.WriteBool('Setup', 'DisableWarrContinueHit', g_Config.boDisableWarrContinueHit);
  Config.WriteString('Setup', 'DisableWarrContinueHitIDs', g_Config.sDisableWarrContinueHitIDs);
  Config.WriteInteger('Setup', 'WarrContinueHitMinInterval', g_Config.nWarrContinueHitMinInterval);

  Reset_g_WarrContinueMagicIDList;

  if (g_nMagicItemRate <> g_Config.nMagicItemRate) or (g_nHumNeedMagicItem <> g_Config.nHumNeedMagicItem) then
  begin
    g_nMagicItemRate := g_Config.nMagicItemRate;
    g_nHumNeedMagicItem := g_Config.nHumNeedMagicItem;
    // UserEngine.SendServerConfig();
  end;

  Config.WriteBool('Setup', 'DogzGotoMaster', g_Config.boDogzGotoMaster);
  Config.WriteBool('Setup', 'BigDogzGotoMaster', g_Config.boBigDogzGotoMaster);
  Config.WriteBool('Setup', 'MonthSpiritGotoMaster', g_Config.boMonthSpiritGotoMaster);
  Config.WriteBool('Setup', 'DogzPlugSettingPriority', g_Config.boDogzPlugSettingPriority);
  Config.WriteBool('Setup', 'BonePlugSettingPriority', g_Config.boBonePlugSettingPriority);

  Config.WriteInteger('Setup', 'UseContinuousMagicTime', g_Config.nUseContinuousMagicTime);

  Config.WriteInteger('Setup', 'NGIncTime', g_Config.nNGIncTime);
  Config.WriteInteger('Setup', 'NGSkillPowerRate', g_Config.nNGSkillPowerRate);
  Config.WriteInteger('Setup', 'NGDrinkIncExp', g_Config.nNGDrinkIncExp);
  Config.WriteInteger('Setup', 'NGHitStruckDecNG', g_Config.nNGHitStruckDecNG);
  Config.WriteInteger('Setup', 'NGKillMonExpMultiple', g_Config.nNGKillMonExpMultiple);

  Config.WriteInteger('Setup', 'NGLevelPowerAdd_Level', g_Config.nNGLevelPowerAdd_Level);
  Config.WriteInteger('Setup', 'NGLevelPowerAdd_Power', g_Config.nNGLevelPowerAdd_Power);

  Config.WriteInteger('Setup', 'NGLevelPowerDec_Level', g_Config.nNGLevelPowerDec_Level);
  Config.WriteInteger('Setup', 'NGLevelPowerDec_Power', g_Config.nNGLevelPowerDec_Power);

  Config.WriteInteger('Setup', 'NGLevelValue', g_Config.nNGLevelValue);
  Config.WriteInteger('Setup', 'NGLevelExpValue', g_Config.nNGLevelExpValue);
  Config.WriteInteger('Setup', 'NGHeroLevelExpValue', g_Config.nNGHeroLevelExpValue);
  Config.WriteInteger('Setup', 'NGMaxLevelLimte', g_Config.nNGMaxLevelLimte);

  for I := 0 to Length(g_Config.SkillContinuousPowerRates) - 1 do
  begin
    Config.WriteInteger('Setup', 'SkillContinuousPowerRates' + IntToStr(I), g_Config.SkillContinuousPowerRates[I]);
  end;

  for I := 0 to Length(g_Config.SkillContinuousCloseDefenseRates) - 1 do
  begin
    Config.WriteInteger('Setup', 'SkillContinuousCloseDefenseRates' + IntToStr(I), g_Config.SkillContinuousCloseDefenseRates[I]);
  end;

  Config.WriteInteger('Setup', 'Skill100BreakDefenceUpRate', g_Config.Skill100BreakDefenceUpRate);

  Config.WriteBool('Setup', 'ToxicSmoke', g_Config.boToxicSmoke);
  Config.WriteInteger('Setup', 'ToxicSmokeDecHPRate', g_Config.nToxicSmokeDecHPRate);

  // 毒烟持续时间 chongchong 2013-11-10
  for I := Low(g_Config.dwToxicSmokeTimes) to High(g_Config.dwToxicSmokeTimes) do
  begin
    Config.WriteInteger('Setup', 'ToxicSmokeTime' + IntToStr(I), g_Config.dwToxicSmokeTimes[I]);
  end;

  for I := 0 to 11 do
  begin
    for II := 0 to 4 do
    begin
      Config.WriteInteger('Setup', 'SkillContinuousBlastHitRates' + IntToStr(I) + '-' + IntToStr(II), g_Config.SkillContinuousBlastHitRates
        [I, II]);
    end;
  end;

  for I := 0 to 11 do
  begin
    for II := 0 to 4 do
    begin
      Config.WriteInteger('Setup', 'SkillContinuousBlastHitPowerRates' + IntToStr(I) + '-' + IntToStr(II), g_Config.SkillContinuousBlastHitPowerRates
        [I, II]);
    end;
  end;

  Config.WriteBool('Setup', 'DoMotaebo100PushSameLevel', g_Config.boDoMotaebo100PushSameLevel);
  Config.WriteInteger('Setup', 'DoMotaebo100PushDistance', g_Config.nDoMotaebo100PushDistance);

  for I := 0 to 4 do
  begin
    for II := 0 to 4 do
    begin
      Config.WriteInteger('Setup', 'AcupointLevels' + IntToStr(I) + '-' + IntToStr(II), g_Config.AcupointLevels[I, II]);
    end;
  end;

  // 雷霆剑法 piaoyun 2013-07-23
  Config.WriteInteger('Setup', 'Skill43HitWaitTime', g_Config.nSkill43HitWaitTime);
  Config.WriteInteger('Setup', 'Skill43PowerRate', g_Config.nSkill43PowerRate);
  Config.WriteInteger('Setup', 'Skill43LDMBRate', g_Config.nSkill43LDMBRate);
  Config.WriteInteger('Setup', 'Skill43LDMBTime', g_Config.nSkill43LDMBTime);
  Config.WriteInteger('Setup', 'Skill43LDMBPowerAdd', g_Config.nSkill43LDMBPowerAdd);
  Config.WriteBool('Setup', 'Skill43LockParaly', g_Config.boSkill43LockParaly);

  Config.WriteInteger('Setup', 'Skill114PowerRate', g_Config.nSkill114PowerRate);
  Config.WriteInteger('Setup', 'Skill114LevelUpAddPowerRate', g_Config.nSkill114LevelUpAddPowerRate);
  Config.WriteInteger('Setup', 'Skill114HitWaitTime', g_Config.nSkill114HitWaitTime);
  Config.WriteInteger('Setup', 'Skill114AttackRange', g_Config.nSkill114AttackRange);
  Config.WriteInteger('Setup', 'HeroSkill114HitWaitTime', g_Config.nHeroSkill114HitWaitTime);

  // 野蛮冲撞 piaoyun 2013-07-25
  Config.WriteBool('Setup', 'ShowDoMotaeboMsg', g_Config.boShowDoMotaeboMsg);
  Config.WriteBool('Setup', 'DoMotaeboPushSameLevel', g_Config.boDoMotaeboPushSameLevel);
  Config.WriteInteger('Setup', 'DoMotaeboCD', g_Config.nDoMotaeboCD);
  Config.WriteBool('Setup', 'BarbaricSeptum', g_Config.boBarbaricSeptum);

  // 烈火剑法 piaoyun 2013-07-25
  Config.WriteBool('Setup', 'EnableDoubleFireHitSkill', g_Config.boEnableDoubleFireHitSkill);

  Config.WriteBool('Setup', 'EnableDoubleFireHitDelayClose', g_Config.boEnableDoubleFireHitDelayClose);
  Config.WriteInteger('Setup', 'DoubleFireHitDelayCloseType', g_Config.nDoubleFireHitDelayCloseType);
  Config.WriteInteger('Setup', 'DoubleFireHitDelayCloseValue', g_Config.nDoubleFireHitDelayCloseValue);

  Config.WriteBool('Setup', 'CloseFireHitSkillFailHint', g_Config.boCloseFireHitSkillFailHint);

  // 无极真气 piaoyun 2013-07-25
  Config.WriteInteger('Setup', 'ElectrodelessBase', g_Config.nElectrodelessBase);
  Config.WriteInteger('Setup', 'ElectrodelessWaitTime', g_Config.nElectrodelessWaitTime);

  for I := Low(g_Config.nElectrodelessPowerRates) to High(g_Config.nElectrodelessPowerRates) do
  begin
    Config.WriteInteger('Setup', 'ElectrodelessPowerRateL' + IntToStr(I), g_Config.nElectrodelessPowerRates[I]);
  end;

  for I := Low(g_Config.dwElectrodelessTimes) to High(g_Config.dwElectrodelessTimes) do
  begin
    Config.WriteInteger('Setup', 'ElectrodelessTimeL' + IntToStr(I), g_Config.dwElectrodelessTimes[I]);
  end;

  for I := 0 to High(g_Config.NewLevelMagicPowerRatesSpecific[46]) do
  begin
    StrSection := Format('NewLevelMagicPowerRatesSpecific%d_%d', [46, I]);
    Config.WriteInteger('Setup', StrSection, g_Config.NewLevelMagicPowerRatesSpecific[46][I])
  end;

  for I := 0 to High(g_Config.NewLevelMagicElectrodelessTime) do
  begin
    Config.WriteInteger('Setup', 'NewLevelMagicElectrodelessTime' + IntToStr(I), g_Config.NewLevelMagicElectrodelessTime[I])
  end;

  Config.WriteBool('Setup', 'ElectrodelessTimeSet', g_Config.boElectrodelessTimeSet);
  Config.WriteBool('Setup', 'ElectrodelessUseMaxSC', g_Config.boElectrodelessUseMaxSC);

  // 神圣战甲术、幽灵盾 piaoyun 2013-07-25
  Config.WriteInteger('Setup', 'Skill15PowerRate', g_Config.nSkill15PowerRate);
  Config.WriteInteger('Setup', 'Skill15TimeRate', g_Config.nSkill15TimeRate);
  Config.WriteBool('Setup', 'Skill15OfflineClear', g_Config.boSkill15OfflineClear);

  // Config.WriteBool('Setup', 'FuDuSysMsg', g_Config.boFuDuSysMsg);
  // Config.WriteBool('Setup', 'FuDuScreenMsg', g_Config.boFuDuScreenMsg);
  // Config.WriteBool('Setup', 'NeedMagicSysMsg', g_Config.boNeedMagicSysMsg);

  Config.WriteInteger('Setup', 'PosionDecHealthTime', g_Config.dwPosionDecHealthTime);
  Config.WriteInteger('Setup', 'PosionDamagarmor', g_Config.nPosionDamagarmor);
  Config.WriteBool('Setup', 'PosionStopIncHealth', g_Config.boPosionStopIncHealth);
  Config.WriteBool('Setup', 'EnabledPosionDecMAC', g_Config.boEnabledPosionDecMAC);
  Config.WriteInteger('Setup', 'PosionDecMACRate', g_Config.nPosionDecMACRate);

  // 裂神符
  Config.WriteInteger('Setup', 'Skill202BaseCount', g_Config.nSkill202BaseCount);
  Config.WriteInteger('Setup', 'Skill202LevelupCount', g_Config.nSkill202LevelupCount);
  Config.WriteInteger('Setup', 'Skill202CD', g_Config.nSkill202CD);
  Config.WriteInteger('Setup', 'Skill202Rate', g_Config.nSkill202Rate);
  Config.WriteBool('Setup', 'Skill202RateOnlyMon', g_Config.boSkill202RateOnlyMon);

  Config.WriteInteger('Setup', 'SkillFireCharmPowerRate', g_Config.nSkillFireCharmPowerRate);
  Config.WriteInteger('Setup', 'Skill202PowerRate', g_Config.nSkill202PowerRate);

  Config.WriteInteger('Setup', 'Skill202PowerDec', g_Config.nSkill202PowerDec);
  Config.WriteInteger('Setup', 'Skill202PowerMin', g_Config.nSkill202PowerMin);

  Config.WriteBool('Setup', 'SpiritualismLevelDiff', g_Config.boSpiritualismLevelDiff);
  Config.WriteInteger('Setup', 'SpiritualismLevelDiffValue', g_Config.nSpiritualismLevelDiff);
  Config.WriteInteger('Setup', 'SpiritualismRoyaltyTime', g_Config.nSpiritualismRoyaltyTime);
  Config.WriteInteger('Setup', 'SpiritualismBBCount', g_Config.nSpiritualismBBCount);

  for I := Low(g_Config.nSpiritualismRates) to High(g_Config.nSpiritualismRates) do
  begin
    Config.WriteInteger('Setup', 'SpiritualismRate' + IntToStr(I), g_Config.nSpiritualismRates[I]);
  end;
  Config.WriteBool('Setup', 'SpiritualismDisableUndeadMon', g_Config.boSpiritualismDisableUndeadMon);


  // 死亡之眼

  Config.WriteInteger('Setup', 'Skill203PoisonRate', g_Config.nSkill203PoisonRate);
  Config.WriteInteger('Setup', 'Skill203BasicMbTimer', g_Config.nSkill203BasicMbTimer);
  Config.WriteInteger('Setup', 'Skill203LevelupMbTimer', g_Config.nSkill203LevelupMbTimer);
  Config.WriteInteger('Setup', 'Skill203BasicPowerRate', g_Config.nSkill203BasicPowerRate);
  Config.WriteInteger('Setup', 'Skill203LevelupPowerRate', g_Config.nSkill203LevelupPowerRate);

  Config.WriteBool('Setup', 'Skill203MbAttackMon', g_Config.boSkill203MbAttackMon);
  Config.WriteBool('Setup', 'Skill203MbAttackHuman', g_Config.boSkill203MbAttackHuman);
  Config.WriteBool('Setup', 'Skill203MbAttackSlave', g_Config.boSkill203MbAttackSlave);
  Config.WriteBool('Setup', 'Skill203Damagearmor', g_Config.boSkill203Damagearmor);
  Config.WriteBool('Setup', 'Skill203DecHealth', g_Config.boSkill203DecHealth);
  Config.WriteBool('Setup', 'Skill203MbFastParalysis', g_Config.boSkill203MbFastParalysis);
  Config.WriteInteger('Setup', 'Skill203CD', g_Config.nSkill203CD);
  Config.WriteInteger('Setup', 'Skill203Rage', g_Config.nSkill203Rage);

  // 十步一杀
  Config.WriteInteger('Setup', 'Skill204BasicMbTimer', g_Config.nSkill204BasicMbTimer);
  Config.WriteInteger('Setup', 'Skill204BasicMbRate', g_Config.nSkill204BasicMbRate);
  Config.WriteInteger('Setup', 'Skill204LevelupMbTimer', g_Config.nSkill204LevelupMbTimer);
  Config.WriteInteger('Setup', 'Skill204BasicPowerRate', g_Config.nSkill204BasicPowerRate);
  Config.WriteInteger('Setup', 'Skill204LevelupPowerRate', g_Config.nSkill204LevelupPowerRate);
  Config.WriteBool('Setup', 'Skill204MbAttackMon', g_Config.boSkill204MbAttackMon);
  Config.WriteBool('Setup', 'Skill204MbAttackHuman', g_Config.boSkill204MbAttackHuman);
  Config.WriteBool('Setup', 'Skill204MbAttackSlave', g_Config.boSkill204MbAttackSlave);
  Config.WriteBool('Setup', 'Skill204MbFastParalysis', g_Config.boSkill204MbFastParalysis);
  Config.WriteBool('Setup', 'Skill204RunHum', g_Config.boSkill204RunHum);
  Config.WriteBool('Setup', 'Skill204RunMon', g_Config.boSkill204RunMon);
  Config.WriteBool('Setup', 'Skill204RunNpc', g_Config.boSkill204RunNpc);
  Config.WriteBool('Setup', 'Skill204RunGuard', g_Config.boSkill204RunGuard);
  Config.WriteBool('Setup', 'Skill204RunObstacle', g_Config.boSkill204RunObstacle);
  Config.WriteBool('Setup', 'Skill204WarDisHumRun', g_Config.boSkill204WarDisHumRun);
  Config.WriteInteger('Setup', 'Skill204CD', g_Config.nSkill204CD);
  Config.WriteInteger('Setup', 'Skill204Rage', g_Config.nSkill204Rage);
  Config.WriteInteger('Setup', 'Skill204Distance', g_Config.nSkill204Distance);
  Config.WriteBool('Setup', 'Skill204SameLevel', g_Config.boSkill204SameLevel);
  Config.WriteBool('Setup', 'Skill204DisableStopItem', g_Config.boSkill204DisableStopItem);

  // 冰霜雪雨
  Config.WriteBool('Setup', 'Skill205ReduceMP', g_Config.boSkill205ReduceMP);
  Config.WriteInteger('Setup', 'Skill205CD', g_Config.nSkill205CD);
  Config.WriteInteger('Setup', 'Skill205Rage', g_Config.nSkill205Rage);
  Config.WriteInteger('Setup', 'Skill205PowerRate', g_Config.nSkill205PowerRate);
  Config.WriteBool('Setup', 'Skill205PowerTwoAttack', g_Config.boSkill205PowerTwoAttack);

  // 冰霜群雨
  Config.WriteInteger('Setup', 'Skill206BasicMbTimer', g_Config.nSkill206BasicMbTimer);
  Config.WriteInteger('Setup', 'Skill206LevelupMbTimer', g_Config.nSkill206LevelupMbTimer);
  Config.WriteInteger('Setup', 'Skill206BasicPowerRate', g_Config.nSkill206BasicPowerRate);
  Config.WriteInteger('Setup', 'Skill206LevelupPowerRate', g_Config.nSkill206LevelupPowerRate);
  Config.WriteBool('Setup', 'Skill206MbAttackMon', g_Config.boSkill206MbAttackMon);
  Config.WriteBool('Setup', 'Skill206MbAttackHuman', g_Config.boSkill206MbAttackHuman);
  Config.WriteBool('Setup', 'Skill206MbAttackSlave', g_Config.boSkill206MbAttackSlave);
  Config.WriteBool('Setup', 'Skill206MbFastParalysis', g_Config.boSkill206MbFastParalysis);
  Config.WriteInteger('Setup', 'Skill206CD', g_Config.nSkill206CD);
  Config.WriteInteger('Setup', 'Skill206Rage', g_Config.nSkill206Rage);
  Config.WriteBool('Setup', 'Skill206SameLevel', g_Config.boSkill206SameLevel);
  Config.WriteBool('Setup', 'Skill206Frozen', g_Config.boSkill206Frozen);
  Config.WriteInteger('Setup', 'Skill206FrozenRate', g_Config.nSkill206FrozenRate);

  { 战士目前只用了6个 }
  for I := 0 to 5 do
  begin
    for II := 0 to High(g_Config.NewLevelMagicPowerRatesSpecific[I]) do
    begin
      StrSection := Format('NewLevelMagicPowerRatesSpecific%d_%d', [I, II]);
      Config.WriteInteger('Setup', StrSection, g_Config.NewLevelMagicPowerRatesSpecific[I][II]);
    end;
  end;

  { 法师目前只用了6个 }
  for I := 20 to 25 do
  begin
    for II := 0 to High(g_Config.NewLevelMagicPowerRatesSpecific[I]) do
    begin
      StrSection := Format('NewLevelMagicPowerRatesSpecific%d_%d', [I, II]);
      Config.WriteInteger('Setup', StrSection, g_Config.NewLevelMagicPowerRatesSpecific[I][II]);
    end;
  end;

  { 道士目前只用了7个 }
  for I := 40 to 46 do
  begin
    for II := 0 to High(g_Config.NewLevelMagicPowerRatesSpecific[I]) do
    begin
      StrSection := Format('NewLevelMagicPowerRatesSpecific%d_%d', [I, II]);
      Config.WriteInteger('Setup', StrSection, g_Config.NewLevelMagicPowerRatesSpecific[I][II]);
    end;
  end;

  for I := Low(g_Config.NewLevelMagic43LSFRate) to High(g_Config.NewLevelMagic43LSFRate) do
  begin
    Config.WriteInteger('Setup', 'NewLevelMagic43LSFRate' + IntToStr(I), g_Config.NewLevelMagic43LSFRate[I])
  end;

  // 是否允许使用英雄合击 chongchong 2013-08-11
  Config.WriteBool('Setup', 'HeroJointAttack', g_Config.boHeroJointAttack);

  Config.WriteBool('Setup', 'HeroJointAttackFly', g_Config.boHeroJointAttackFly);

  // 连击使用内功值释放 chongchong 2013-09-02
  Config.WriteBool('Setup', 'ContinuousAttackUseNG', g_Config.boContinuousAttackUseNG);

  // 倚天辟地使用内功值释放 chongchong 2013-09-02
  Config.WriteBool('Setup', 'Skill114AttackUseNG', g_Config.boSkill114AttackUseNG);

  // 魔法忽略障碍 piaoyun 2013-09-12
  Config.WriteBool('Setup', 'MagicNotHinder', g_Config.boMagicNotHinder);

  // 提高魔法精确度 piaoyun 2013-09-12
  Config.WriteBool('Setup', 'MagicDefinition', g_Config.boMagicDefinition);

  // 旋风斩 piaoyun 2013-09-15
  Config.WriteInteger('Setup', 'Skill208CD', g_Config.nSkill208CD);
  Config.WriteInteger('Setup', 'Skill208Rage', g_Config.nSkill208Rage);
  Config.WriteInteger('Setup', 'Skill208PowerRate', g_Config.nSkill208PowerRate);
  Config.WriteBool('Setup', 'SKILL208HeroDuanJin', g_Config.boSKILL208HeroDuanJin);
  Config.WriteBool('Setup', 'SKILL208PlayMosterDuanJin', g_Config.boSKILL208PlayMosterDuanJin);

  // 五雷轰 piaoyun 2013-09-14
  Config.WriteInteger('Setup', 'Skill209CD', g_Config.nSkill209CD);
  Config.WriteInteger('Setup', 'Skill209Rage', g_Config.nSkill209Rage);
  Config.WriteInteger('Setup', 'Skill209PowerRate', g_Config.nSkill209PowerRate);

  Config.WriteInteger('Setup', 'Skill37Range', g_Config.nSkill37Range);
  Config.WriteInteger('Setup', 'Skill37RangeAdd', g_Config.nSkill37RangeAdd);

  Config.WriteInteger('Setup', 'SkillLighteningPowerRate', g_Config.nSkillLighteningPowerRate);
  Config.WriteInteger('Setup', 'SkillGroupLighteningPowerRate', g_Config.nSkillGroupLighteningPowerRate);

  // 幽冥火符 piaoyun 2013-09-14
  Config.WriteInteger('Setup', 'Skill210CD', g_Config.nSkill210CD);
  Config.WriteInteger('Setup', 'Skill210Rage', g_Config.nSkill210Rage);
  Config.WriteInteger('Setup', 'Skill210PowerRate', g_Config.nSkill210PowerRate);

  // 合击失败怒槽下降再次合击允许怒槽值 chongchong 2013-10-26
  Config.WriteInteger('Setup', 'AngryAgainValue', g_Config.dwAngryAgainValue);

  // 召唤分身间隔 chongchong 2013-10-28
  Config.WriteInteger('Setup', 'RecallCopySelfWaitTime', g_Config.dwRecallCopySelfWaitTime);

  Config.WriteInteger('Setup', 'HeroRecallCopySelfHPRate', g_Config.btHeroRecallCopySelfHPRate);

  Config.WriteBool('Setup', 'CopyMonWarrorAttack', g_Config.boCopyMonWarrorAttack);
  Config.WriteBool('Setup', 'CopyMon700HPUseBaseAttack', g_Config.boCopyMon700HPUseBaseAttack);

  Config.WriteBool('Setup', 'CopyMonInheritedMasterSpeed', g_Config.boCopyMonInheritedMasterSpeed);

  // 召唤神兽间隔 chongchong 2013-10-28
  Config.WriteInteger('Setup', 'RecallDogzWaitTime', g_Config.dwRecallDogzWaitTime);

  // 召唤骷髅间隔 chongchong 2013-10-28
  Config.WriteInteger('Setup', 'RecallBoneFammWaitTime', g_Config.dwRecallBoneFammWaitTime);

  // 召唤月灵间隔 chongchong 2013-10-28
  Config.WriteInteger('Setup', 'RecallMonthSpiritWaitTime', g_Config.dwRecallMonthSpiritWaitTime);

  // 道法连击固定保护 chongchong 2013-11-10
  Config.WriteInteger('Setup', 'ContinuousProtect', g_Config.btContinuousProtect);

  // 道法连击随机保护 chongchong 2013-11-10
  Config.WriteInteger('Setup', 'ContinuousProtectRandom', g_Config.btContinuousProtectRandom);

  // 战士连击技能状态锁定的几率 chongchong 2013-11-11

  for I := Low(g_Config.btWarrContinuousStatusLocks) to High(g_Config.btWarrContinuousStatusLocks) do
  begin
    Config.WriteInteger('Setup', 'WarrContinuousStatusLock' + IntToStr(I + 1), g_Config.btWarrContinuousStatusLocks[I]);
  end;

  for I := Low(g_Config.nWarrContinuousStatusLockTimes) to High(g_Config.nWarrContinuousStatusLockTimes) do
  begin
    Config.WriteInteger('Setup', 'WarrContinuousStatusLockTime' + IntToStr(I + 1), g_Config.nWarrContinuousStatusLockTimes[I]);
  end;

  Config.WriteInteger('Setup', 'Skill102AttackRange', g_Config.nSkill102AttackRange);
  Config.WriteInteger('Setup', 'Skill103AttackRange', g_Config.nSkill103AttackRange);
  Config.WriteInteger('Setup', 'Skill105AttackRange', g_Config.nSkill105AttackRange);

  Config.WriteInteger('Setup', 'Skill106AddFrozenRate', g_Config.nSkill106AddFrozenRate);
  Config.WriteInteger('Setup', 'Skill106AddFrozenRate2', g_Config.nSkill106AddFrozenRate2);
  Config.WriteInteger('Setup', 'Skill106AddFrozenTime', g_Config.nSkill106AddFrozenTime);
  Config.WriteInteger('Setup', 'Skill106AddFrozenTime2', g_Config.nSkill106AddFrozenTime2);

  Config.WriteInteger('Setup', 'Skill110PushedRate', g_Config.nSkill110PushedRate);
  Config.WriteInteger('Setup', 'Skill110PushedRate2', g_Config.nSkill110PushedRate2);
  Config.WriteInteger('Setup', 'Skill110PushedRange', g_Config.nSkill110PushedRange);
  Config.WriteInteger('Setup', 'Skill110PushedRange2', g_Config.nSkill110PushedRange2);
  Config.WriteBool('Setup', 'Skill110PushedHighLevel', g_Config.boSkill110PushedHighLevel);

  Config.WriteInteger('Setup', 'ContinuousAttackLevelRate', g_Config.nContinuousAttackLevelRate);

  for I := 0 to Length(g_Config.SkillContinueOrderBlastRates) - 1 do
  begin
    Config.WriteInteger('Setup', 'SkillContinueOrderBlastRates' + IntToStr(I), g_Config.SkillContinueOrderBlastRates[I]);
  end;

  Config.WriteString('Setup', 'PlusBoneFammName1_3', g_Config.sPlusBoneFammName1_3);
  Config.WriteString('Setup', 'PlusBoneFammName4_6', g_Config.sPlusBoneFammName4_6);
  Config.WriteString('Setup', 'PlusBoneFammName7_9', g_Config.sPlusBoneFammName7_9);
  Config.WriteString('Setup', 'PlusBoneFammName9_N', g_Config.sPlusBoneFammName9_N);

  // 每重强化召唤骷髅的级别 chongchong 2013-12-09
  for I := Low(g_Config.dwPlusBoneFammLevels) to High(g_Config.dwPlusBoneFammLevels) do
    Config.WriteInteger('Setup', 'PlusBoneFammLevel' + IntToStr(I + 1), g_Config.dwPlusBoneFammLevels[I]);

  // 强化召唤骷髅9重以后每重增加等级 chongchong 2013-12-11
  Config.WriteInteger('Setup', 'PlusBoneFammAddLevelAfter9', g_Config.dwPlusBoneFammAddLevelAfter9);

  Config.WriteString('Setup', 'PlusDogzName1_3', g_Config.sPlusDogzName1_3);
  Config.WriteString('Setup', 'PlusDogzName4_6', g_Config.sPlusDogzName4_6);
  Config.WriteString('Setup', 'PlusDogzName7_9', g_Config.sPlusDogzName7_9);
  Config.WriteString('Setup', 'PlusDogzName9_N', g_Config.sPlusDogzName9_N);

  // 每重强化召唤神兽的级别 chongchong 2013-12-09
  for I := Low(g_Config.dwPlusDogzLevels) to High(g_Config.dwPlusDogzLevels) do
    Config.WriteInteger('Setup', 'PlusDogzLevel' + IntToStr(I + 1), g_Config.dwPlusDogzLevels[I]);

  // 强化召唤神兽9重以后每重增加等级 chongchong 2013-12-11
  Config.WriteInteger('Setup', 'PlusDogzAddLevelAfter9', g_Config.dwPlusDogzAddLevelAfter9);

  // 召唤技能强化加属性倍率
  for I := Low(g_Config.nBBAttrPlusNewLevelRates) to High(g_Config.nBBAttrPlusNewLevelRates) do
  begin
    Config.WriteInteger('Setup', 'BBAttrPlusNewLevelRate' + IntToStr(I + 1), g_Config.nBBAttrPlusNewLevelRates[I]);
  end;

  if boSendServerConfig then
  begin
    UserEngine.SendServerConfig();
    boSendServerConfig := False;
  end;

  SaveMagicACList;

  ResetMagicCDList;

  uModValue();
end;

procedure TfrmFunctionConfig.RefUpgradeWeapon();
begin
  ScrollBarUpgradeWeaponDCRate.Position := g_Config.nUpgradeWeaponDCRate;
  ScrollBarUpgradeWeaponDCTwoPointRate.Position := g_Config.nUpgradeWeaponDCTwoPointRate;
  ScrollBarUpgradeWeaponDCThreePointRate.Position := g_Config.nUpgradeWeaponDCThreePointRate;

  ScrollBarUpgradeWeaponMCRate.Position := g_Config.nUpgradeWeaponMCRate;
  ScrollBarUpgradeWeaponMCTwoPointRate.Position := g_Config.nUpgradeWeaponMCTwoPointRate;
  ScrollBarUpgradeWeaponMCThreePointRate.Position := g_Config.nUpgradeWeaponMCThreePointRate;

  ScrollBarUpgradeWeaponSCRate.Position := g_Config.nUpgradeWeaponSCRate;
  ScrollBarUpgradeWeaponSCTwoPointRate.Position := g_Config.nUpgradeWeaponSCTwoPointRate;
  ScrollBarUpgradeWeaponSCThreePointRate.Position := g_Config.nUpgradeWeaponSCThreePointRate;

  EditUpgradeWeaponMaxPoint.Value := g_Config.nUpgradeWeaponMaxPoint;
  EditUpgradeWeaponPrice.Value := g_Config.nUpgradeWeaponPrice;
  EditUPgradeWeaponGetBackTime.Value := g_Config.dwUPgradeWeaponGetBackTime div 1000;
  EditClearExpireUpgradeWeaponDays.Value := g_Config.nClearExpireUpgradeWeaponDays;

  CheckBoxWeaponUpgradeFailNotDelete.Checked := g_Config.boWeaponUpgradeFailNotDelete;
end;

procedure TfrmFunctionConfig.ScrollBarUpgradeWeaponDCRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarUpgradeWeaponDCRate.Position;
  EditUpgradeWeaponDCRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponDCRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarUpgradeWeaponDCTwoPointRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarUpgradeWeaponDCTwoPointRate.Position;
  EditUpgradeWeaponDCTwoPointRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponDCTwoPointRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarUpgradeWeaponDCThreePointRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarUpgradeWeaponDCThreePointRate.Position;
  EditUpgradeWeaponDCThreePointRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponDCThreePointRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarUpgradeWeaponSCRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarUpgradeWeaponSCRate.Position;
  EditUpgradeWeaponSCRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponSCRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarUpgradeWeaponSCTwoPointRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarUpgradeWeaponSCTwoPointRate.Position;
  EditUpgradeWeaponSCTwoPointRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponSCTwoPointRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarUpgradeWeaponSCThreePointRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarUpgradeWeaponSCThreePointRate.Position;
  EditUpgradeWeaponSCThreePointRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponSCThreePointRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarUpgradeWeaponMCRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarUpgradeWeaponMCRate.Position;
  EditUpgradeWeaponMCRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponMCRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarUpgradeWeaponMCTwoPointRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarUpgradeWeaponMCTwoPointRate.Position;
  EditUpgradeWeaponMCTwoPointRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponMCTwoPointRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarUpgradeWeaponMCThreePointRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarUpgradeWeaponMCThreePointRate.Position;
  EditUpgradeWeaponMCThreePointRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponMCThreePointRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.EditUpgradeWeaponMaxPointChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponMaxPoint := EditUpgradeWeaponMaxPoint.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditUpgradeWeaponPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUpgradeWeaponPrice := EditUpgradeWeaponPrice.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditUPgradeWeaponGetBackTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwUPgradeWeaponGetBackTime := EditUPgradeWeaponGetBackTime.Value * 1000;
  ModValue();
end;

procedure TfrmFunctionConfig.EditClearExpireUpgradeWeaponDaysChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nClearExpireUpgradeWeaponDays := EditClearExpireUpgradeWeaponDays.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.ButtonUpgradeWeaponSaveClick(Sender: TObject);
begin

  Config.WriteInteger('Setup', 'UpgradeWeaponMaxPoint', g_Config.nUpgradeWeaponMaxPoint);
  Config.WriteInteger('Setup', 'UpgradeWeaponPrice', g_Config.nUpgradeWeaponPrice);
  Config.WriteInteger('Setup', 'ClearExpireUpgradeWeaponDays', g_Config.nClearExpireUpgradeWeaponDays);
  Config.WriteInteger('Setup', 'UPgradeWeaponGetBackTime', g_Config.dwUPgradeWeaponGetBackTime);

  Config.WriteInteger('Setup', 'UpgradeWeaponDCRate', g_Config.nUpgradeWeaponDCRate);
  Config.WriteInteger('Setup', 'UpgradeWeaponDCTwoPointRate', g_Config.nUpgradeWeaponDCTwoPointRate);
  Config.WriteInteger('Setup', 'UpgradeWeaponDCThreePointRate', g_Config.nUpgradeWeaponDCThreePointRate);

  Config.WriteInteger('Setup', 'UpgradeWeaponMCRate', g_Config.nUpgradeWeaponMCRate);
  Config.WriteInteger('Setup', 'UpgradeWeaponMCTwoPointRate', g_Config.nUpgradeWeaponMCTwoPointRate);
  Config.WriteInteger('Setup', 'UpgradeWeaponMCThreePointRate', g_Config.nUpgradeWeaponMCThreePointRate);

  Config.WriteInteger('Setup', 'UpgradeWeaponSCRate', g_Config.nUpgradeWeaponSCRate);
  Config.WriteInteger('Setup', 'UpgradeWeaponSCTwoPointRate', g_Config.nUpgradeWeaponSCTwoPointRate);
  Config.WriteInteger('Setup', 'UpgradeWeaponSCThreePointRate', g_Config.nUpgradeWeaponSCThreePointRate);
  Config.WriteBool('Setup', 'WeaponUpgradeFailNotDelete', g_Config.boWeaponUpgradeFailNotDelete);
  uModValue();
end;

procedure TfrmFunctionConfig.ButtonUpgradeWeaponDefaulfClick(Sender: TObject);
begin
  if Application.MessageBox('是否确认恢复默认设置？', '确认信息', MB_YESNO + MB_ICONQUESTION) <> IDYES then
  begin
    Exit;
  end;
  g_Config.nUpgradeWeaponMaxPoint := 20;
  g_Config.nUpgradeWeaponPrice := 10000;
  g_Config.nClearExpireUpgradeWeaponDays := 8;
  g_Config.dwUPgradeWeaponGetBackTime := 60 * 60 * 1000;

  g_Config.nUpgradeWeaponDCRate := 100;
  g_Config.nUpgradeWeaponDCTwoPointRate := 30;
  g_Config.nUpgradeWeaponDCThreePointRate := 200;

  g_Config.nUpgradeWeaponMCRate := 100;
  g_Config.nUpgradeWeaponMCTwoPointRate := 30;
  g_Config.nUpgradeWeaponMCThreePointRate := 200;

  g_Config.nUpgradeWeaponSCRate := 100;
  g_Config.nUpgradeWeaponSCTwoPointRate := 30;
  g_Config.nUpgradeWeaponSCThreePointRate := 200;

  g_Config.boWeaponUpgradeFailNotDelete := False;
  RefUpgradeWeapon();
end;

procedure TfrmFunctionConfig.EditMasterOKLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMasterOKLevel := EditMasterOKLevel.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMasterOKCreditPointChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMasterOKCreditPoint := EditMasterOKCreditPoint.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMasterOKBonusPointChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMasterOKBonusPoint := EditMasterOKBonusPoint.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.ButtonMasterSaveClick(Sender: TObject);
begin

  Config.WriteInteger('Setup', 'MasterOKLevel', g_Config.nMasterOKLevel);
  Config.WriteInteger('Setup', 'MasterOKCreditPoint', g_Config.nMasterOKCreditPoint);
  Config.WriteInteger('Setup', 'MasterOKBonusPoint', g_Config.nMasterOKBonusPoint);
  Config.WriteInteger('Setup', 'MasterCount', g_Config.nMasterCount);

  uModValue();
end;

procedure TfrmFunctionConfig.ButtonMakeMineSaveClick(Sender: TObject);
begin

  Config.WriteInteger('Setup', 'MakeMineHitRate', g_Config.nMakeMineHitRate);
  Config.WriteInteger('Setup', 'MakeMineRate', g_Config.nMakeMineRate);
  Config.WriteInteger('Setup', 'StoneTypeRate', g_Config.nStoneTypeRate);
  Config.WriteInteger('Setup', 'StoneTypeRateMin', g_Config.nStoneTypeRateMin);
  Config.WriteInteger('Setup', 'GoldStoneMin', g_Config.nGoldStoneMin);
  Config.WriteInteger('Setup', 'GoldStoneMax', g_Config.nGoldStoneMax);
  Config.WriteInteger('Setup', 'SilverStoneMin', g_Config.nSilverStoneMin);
  Config.WriteInteger('Setup', 'SilverStoneMax', g_Config.nSilverStoneMax);
  Config.WriteInteger('Setup', 'SteelStoneMin', g_Config.nSteelStoneMin);
  Config.WriteInteger('Setup', 'SteelStoneMax', g_Config.nSteelStoneMax);
  Config.WriteInteger('Setup', 'BlackStoneMin', g_Config.nBlackStoneMin);
  Config.WriteInteger('Setup', 'BlackStoneMax', g_Config.nBlackStoneMax);
  Config.WriteInteger('Setup', 'StoneMinDura', g_Config.nStoneMinDura);
  Config.WriteInteger('Setup', 'StoneGeneralDuraRate', g_Config.nStoneGeneralDuraRate);
  Config.WriteInteger('Setup', 'StoneAddDuraRate', g_Config.nStoneAddDuraRate);
  Config.WriteInteger('Setup', 'StoneAddDuraMax', g_Config.nStoneAddDuraMax);

  uModValue();
end;

procedure TfrmFunctionConfig.ButtonMakeMineDefaultClick(Sender: TObject);
begin
  if Application.MessageBox('是否确认恢复默认设置？', '确认信息', MB_YESNO + MB_ICONQUESTION) <> IDYES then
  begin
    Exit;
  end;
  g_Config.nMakeMineHitRate := 4;
  g_Config.nMakeMineRate := 12;
  g_Config.nStoneTypeRate := 120;
  g_Config.nStoneTypeRateMin := 56;
  g_Config.nGoldStoneMin := 1;
  g_Config.nGoldStoneMax := 2;
  g_Config.nSilverStoneMin := 3;
  g_Config.nSilverStoneMax := 20;
  g_Config.nSteelStoneMin := 21;
  g_Config.nSteelStoneMax := 45;
  g_Config.nBlackStoneMin := 46;
  g_Config.nBlackStoneMax := 56;
  g_Config.nStoneMinDura := 3000;
  g_Config.nStoneGeneralDuraRate := 13000;
  g_Config.nStoneAddDuraRate := 20;
  g_Config.nStoneAddDuraMax := 10000;
  RefMakeMine();
end;

procedure TfrmFunctionConfig.RefMakeMine();
begin
  ScrollBarMakeMineHitRate.Position := g_Config.nMakeMineHitRate;
  ScrollBarMakeMineHitRate.Min := 0;
  ScrollBarMakeMineHitRate.Max := 10;

  ScrollBarMakeMineRate.Position := g_Config.nMakeMineRate;
  ScrollBarMakeMineRate.Min := 0;
  ScrollBarMakeMineRate.Max := 50;

  ScrollBarStoneTypeRate.Position := g_Config.nStoneTypeRate;
  ScrollBarStoneTypeRate.Min := g_Config.nStoneTypeRateMin;
  ScrollBarStoneTypeRate.Max := 500;

  ScrollBarGoldStoneMax.Min := 1;
  ScrollBarGoldStoneMax.Max := g_Config.nSilverStoneMax;

  ScrollBarSilverStoneMax.Min := g_Config.nGoldStoneMax;
  ScrollBarSilverStoneMax.Max := g_Config.nSteelStoneMax;

  ScrollBarSteelStoneMax.Min := g_Config.nSilverStoneMax;
  ScrollBarSteelStoneMax.Max := g_Config.nBlackStoneMax;

  ScrollBarBlackStoneMax.Min := g_Config.nSteelStoneMax;
  ScrollBarBlackStoneMax.Max := g_Config.nStoneTypeRate;

  ScrollBarGoldStoneMax.Position := g_Config.nGoldStoneMax;
  ScrollBarSilverStoneMax.Position := g_Config.nSilverStoneMax;
  ScrollBarSteelStoneMax.Position := g_Config.nSteelStoneMax;
  ScrollBarBlackStoneMax.Position := g_Config.nBlackStoneMax;

  EditStoneMinDura.Value := g_Config.nStoneMinDura div 1000;
  EditStoneGeneralDuraRate.Value := g_Config.nStoneGeneralDuraRate div 1000;
  EditStoneAddDuraRate.Value := g_Config.nStoneAddDuraRate;
  EditStoneAddDuraMax.Value := g_Config.nStoneAddDuraMax div 1000;
end;

procedure TfrmFunctionConfig.ScrollBarMakeMineHitRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarMakeMineHitRate.Position;
  EditMakeMineHitRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nMakeMineHitRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarMakeMineRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarMakeMineRate.Position;
  EditMakeMineRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nMakeMineRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarStoneTypeRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarStoneTypeRate.Position;
  EditStoneTypeRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  ScrollBarBlackStoneMax.Max := nPostion;
  g_Config.nStoneTypeRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarGoldStoneMaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarGoldStoneMax.Position;
  EditGoldStoneMax.Text := IntToStr(g_Config.nGoldStoneMin) + '-' + IntToStr(g_Config.nGoldStoneMax);
  if not boOpened then
    Exit;
  g_Config.nSilverStoneMin := nPostion + 1;
  ScrollBarSilverStoneMax.Min := nPostion + 1;
  g_Config.nGoldStoneMax := nPostion;
  EditSilverStoneMax.Text := IntToStr(g_Config.nSilverStoneMin) + '-' + IntToStr(g_Config.nSilverStoneMax);
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarSilverStoneMaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarSilverStoneMax.Position;
  EditSilverStoneMax.Text := IntToStr(g_Config.nSilverStoneMin) + '-' + IntToStr(g_Config.nSilverStoneMax);
  if not boOpened then
    Exit;
  ScrollBarGoldStoneMax.Max := nPostion - 1;
  g_Config.nSteelStoneMin := nPostion + 1;
  ScrollBarSteelStoneMax.Min := nPostion + 1;
  g_Config.nSilverStoneMax := nPostion;
  EditGoldStoneMax.Text := IntToStr(g_Config.nGoldStoneMin) + '-' + IntToStr(g_Config.nGoldStoneMax);
  EditSteelStoneMax.Text := IntToStr(g_Config.nSteelStoneMin) + '-' + IntToStr(g_Config.nSteelStoneMax);
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarSteelStoneMaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarSteelStoneMax.Position;
  EditSteelStoneMax.Text := IntToStr(g_Config.nSteelStoneMin) + '-' + IntToStr(g_Config.nSteelStoneMax);
  if not boOpened then
    Exit;
  ScrollBarSilverStoneMax.Max := nPostion - 1;
  g_Config.nBlackStoneMin := nPostion + 1;
  ScrollBarBlackStoneMax.Min := nPostion + 1;
  g_Config.nSteelStoneMax := nPostion;
  EditSilverStoneMax.Text := IntToStr(g_Config.nSilverStoneMin) + '-' + IntToStr(g_Config.nSilverStoneMax);
  EditBlackStoneMax.Text := IntToStr(g_Config.nBlackStoneMin) + '-' + IntToStr(g_Config.nBlackStoneMax);
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarBlackStoneMaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarBlackStoneMax.Position;
  EditBlackStoneMax.Text := IntToStr(g_Config.nBlackStoneMin) + '-' + IntToStr(g_Config.nBlackStoneMax);
  if not boOpened then
    Exit;
  ScrollBarSteelStoneMax.Max := nPostion - 1;
  ScrollBarStoneTypeRate.Min := nPostion;
  g_Config.nBlackStoneMax := nPostion;
  EditSteelStoneMax.Text := IntToStr(g_Config.nSteelStoneMin) + '-' + IntToStr(g_Config.nSteelStoneMax);
  ModValue();
end;

procedure TfrmFunctionConfig.EditStoneMinDuraChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStoneMinDura := EditStoneMinDura.Value * 1000;
  ModValue();
end;

procedure TfrmFunctionConfig.EditStoneGeneralDuraRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStoneGeneralDuraRate := EditStoneGeneralDuraRate.Value * 1000;
  ModValue();
end;

procedure TfrmFunctionConfig.EditStoneAddDuraRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStoneAddDuraRate := EditStoneAddDuraRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditStoneAddDuraMaxChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStoneAddDuraMax := EditStoneAddDuraMax.Value * 1000;
  ModValue();
end;

procedure TfrmFunctionConfig.RefWinLottery;
begin
  ScrollBarWinLotteryRate.Max := 100000;
  ScrollBarWinLotteryRate.Position := g_Config.nWinLotteryRate;
  ScrollBarWinLottery1Max.Max := g_Config.nWinLotteryRate;
  ScrollBarWinLottery1Max.Min := g_Config.nWinLottery1Min;
  ScrollBarWinLottery2Max.Max := g_Config.nWinLottery1Max;
  ScrollBarWinLottery2Max.Min := g_Config.nWinLottery2Min;
  ScrollBarWinLottery3Max.Max := g_Config.nWinLottery2Max;
  ScrollBarWinLottery3Max.Min := g_Config.nWinLottery3Min;
  ScrollBarWinLottery4Max.Max := g_Config.nWinLottery3Max;
  ScrollBarWinLottery4Max.Min := g_Config.nWinLottery4Min;
  ScrollBarWinLottery5Max.Max := g_Config.nWinLottery4Max;
  ScrollBarWinLottery5Max.Min := g_Config.nWinLottery5Min;
  ScrollBarWinLottery6Max.Max := g_Config.nWinLottery5Max;
  ScrollBarWinLottery6Max.Min := g_Config.nWinLottery6Min;
  ScrollBarWinLotteryRate.Min := g_Config.nWinLottery1Max;

  ScrollBarWinLottery1Max.Position := g_Config.nWinLottery1Max;
  ScrollBarWinLottery2Max.Position := g_Config.nWinLottery2Max;
  ScrollBarWinLottery3Max.Position := g_Config.nWinLottery3Max;
  ScrollBarWinLottery4Max.Position := g_Config.nWinLottery4Max;
  ScrollBarWinLottery5Max.Position := g_Config.nWinLottery5Max;
  ScrollBarWinLottery6Max.Position := g_Config.nWinLottery6Max;

  EditWinLottery1Gold.Value := g_Config.nWinLottery1Gold;
  EditWinLottery2Gold.Value := g_Config.nWinLottery2Gold;
  EditWinLottery3Gold.Value := g_Config.nWinLottery3Gold;
  EditWinLottery4Gold.Value := g_Config.nWinLottery4Gold;
  EditWinLottery5Gold.Value := g_Config.nWinLottery5Gold;
  EditWinLottery6Gold.Value := g_Config.nWinLottery6Gold;
end;

procedure TfrmFunctionConfig.ButtonWinLotterySaveClick(Sender: TObject);
begin

  Config.WriteInteger('Setup', 'WinLottery1Gold', g_Config.nWinLottery1Gold);
  Config.WriteInteger('Setup', 'WinLottery2Gold', g_Config.nWinLottery2Gold);
  Config.WriteInteger('Setup', 'WinLottery3Gold', g_Config.nWinLottery3Gold);
  Config.WriteInteger('Setup', 'WinLottery4Gold', g_Config.nWinLottery4Gold);
  Config.WriteInteger('Setup', 'WinLottery5Gold', g_Config.nWinLottery5Gold);
  Config.WriteInteger('Setup', 'WinLottery6Gold', g_Config.nWinLottery6Gold);
  Config.WriteInteger('Setup', 'WinLottery1Min', g_Config.nWinLottery1Min);
  Config.WriteInteger('Setup', 'WinLottery1Max', g_Config.nWinLottery1Max);
  Config.WriteInteger('Setup', 'WinLottery2Min', g_Config.nWinLottery2Min);
  Config.WriteInteger('Setup', 'WinLottery2Max', g_Config.nWinLottery2Max);
  Config.WriteInteger('Setup', 'WinLottery3Min', g_Config.nWinLottery3Min);
  Config.WriteInteger('Setup', 'WinLottery3Max', g_Config.nWinLottery3Max);
  Config.WriteInteger('Setup', 'WinLottery4Min', g_Config.nWinLottery4Min);
  Config.WriteInteger('Setup', 'WinLottery4Max', g_Config.nWinLottery4Max);
  Config.WriteInteger('Setup', 'WinLottery5Min', g_Config.nWinLottery5Min);
  Config.WriteInteger('Setup', 'WinLottery5Max', g_Config.nWinLottery5Max);
  Config.WriteInteger('Setup', 'WinLottery6Min', g_Config.nWinLottery6Min);
  Config.WriteInteger('Setup', 'WinLottery6Max', g_Config.nWinLottery6Max);
  Config.WriteInteger('Setup', 'WinLotteryRate', g_Config.nWinLotteryRate);

  uModValue();
end;

procedure TfrmFunctionConfig.ButtonWinLotteryDefaultClick(Sender: TObject);
begin
  if Application.MessageBox('是否确认恢复默认设置？', '确认信息', MB_YESNO + MB_ICONQUESTION) <> IDYES then
  begin
    Exit;
  end;

  g_Config.nWinLottery1Gold := 1000000;
  g_Config.nWinLottery2Gold := 200000;
  g_Config.nWinLottery3Gold := 100000;
  g_Config.nWinLottery4Gold := 10000;
  g_Config.nWinLottery5Gold := 1000;
  g_Config.nWinLottery6Gold := 500;
  g_Config.nWinLottery6Min := 1;
  g_Config.nWinLottery6Max := 4999;
  g_Config.nWinLottery5Min := 14000;
  g_Config.nWinLottery5Max := 15999;
  g_Config.nWinLottery4Min := 16000;
  g_Config.nWinLottery4Max := 16149;
  g_Config.nWinLottery3Min := 16150;
  g_Config.nWinLottery3Max := 16169;
  g_Config.nWinLottery2Min := 16170;
  g_Config.nWinLottery2Max := 16179;
  g_Config.nWinLottery1Min := 16180;
  g_Config.nWinLottery1Max := 16185;
  g_Config.nWinLotteryRate := 30000;
  RefWinLottery();
end;

procedure TfrmFunctionConfig.EditWinLottery1GoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWinLottery1Gold := EditWinLottery1Gold.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditWinLottery2GoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWinLottery2Gold := EditWinLottery2Gold.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditWinLottery3GoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWinLottery3Gold := EditWinLottery3Gold.Value;
  ModValue();

end;

procedure TfrmFunctionConfig.EditWinLottery4GoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWinLottery4Gold := EditWinLottery4Gold.Value;
  ModValue();

end;

procedure TfrmFunctionConfig.EditWinLottery5GoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWinLottery5Gold := EditWinLottery5Gold.Value;
  ModValue();

end;

procedure TfrmFunctionConfig.EditWinLottery6GoldChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWinLottery6Gold := EditWinLottery6Gold.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWinLottery1MaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarWinLottery1Max.Position;
  EditWinLottery1Max.Text := IntToStr(g_Config.nWinLottery1Min) + '-' + IntToStr(g_Config.nWinLottery1Max);
  if not boOpened then
    Exit;
  g_Config.nWinLottery1Max := nPostion;
  ScrollBarWinLottery2Max.Max := nPostion - 1;
  ScrollBarWinLotteryRate.Min := nPostion;
  EditWinLottery1Max.Text := IntToStr(g_Config.nWinLottery1Min) + '-' + IntToStr(g_Config.nWinLottery1Max);
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWinLottery2MaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarWinLottery2Max.Position;
  EditWinLottery2Max.Text := IntToStr(g_Config.nWinLottery2Min) + '-' + IntToStr(g_Config.nWinLottery2Max);
  if not boOpened then
    Exit;
  g_Config.nWinLottery1Min := nPostion + 1;
  ScrollBarWinLottery1Max.Min := nPostion + 1;
  g_Config.nWinLottery2Max := nPostion;
  EditWinLottery2Max.Text := IntToStr(g_Config.nWinLottery2Min) + '-' + IntToStr(g_Config.nWinLottery2Max);
  EditWinLottery1Max.Text := IntToStr(g_Config.nWinLottery1Min) + '-' + IntToStr(g_Config.nWinLottery1Max);
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWinLottery3MaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarWinLottery3Max.Position;
  EditWinLottery3Max.Text := IntToStr(g_Config.nWinLottery3Min) + '-' + IntToStr(g_Config.nWinLottery3Max);
  if not boOpened then
    Exit;
  g_Config.nWinLottery2Min := nPostion + 1;
  ScrollBarWinLottery2Max.Min := nPostion + 1;
  g_Config.nWinLottery3Max := nPostion;
  EditWinLottery3Max.Text := IntToStr(g_Config.nWinLottery3Min) + '-' + IntToStr(g_Config.nWinLottery3Max);
  EditWinLottery2Max.Text := IntToStr(g_Config.nWinLottery2Min) + '-' + IntToStr(g_Config.nWinLottery2Max);
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWinLottery4MaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarWinLottery4Max.Position;
  EditWinLottery4Max.Text := IntToStr(g_Config.nWinLottery4Min) + '-' + IntToStr(g_Config.nWinLottery4Max);
  if not boOpened then
    Exit;
  g_Config.nWinLottery3Min := nPostion + 1;
  ScrollBarWinLottery3Max.Min := nPostion + 1;
  g_Config.nWinLottery4Max := nPostion;
  EditWinLottery4Max.Text := IntToStr(g_Config.nWinLottery4Min) + '-' + IntToStr(g_Config.nWinLottery4Max);
  EditWinLottery3Max.Text := IntToStr(g_Config.nWinLottery3Min) + '-' + IntToStr(g_Config.nWinLottery3Max);
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWinLottery5MaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarWinLottery5Max.Position;
  EditWinLottery5Max.Text := IntToStr(g_Config.nWinLottery5Min) + '-' + IntToStr(g_Config.nWinLottery5Max);
  if not boOpened then
    Exit;
  g_Config.nWinLottery4Min := nPostion + 1;
  ScrollBarWinLottery4Max.Min := nPostion + 1;
  g_Config.nWinLottery5Max := nPostion;
  EditWinLottery5Max.Text := IntToStr(g_Config.nWinLottery5Min) + '-' + IntToStr(g_Config.nWinLottery5Max);
  EditWinLottery4Max.Text := IntToStr(g_Config.nWinLottery4Min) + '-' + IntToStr(g_Config.nWinLottery4Max);
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWinLottery6MaxChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarWinLottery6Max.Position;
  EditWinLottery6Max.Text := IntToStr(g_Config.nWinLottery6Min) + '-' + IntToStr(g_Config.nWinLottery6Max);
  if not boOpened then
    Exit;
  g_Config.nWinLottery5Min := nPostion + 1;
  ScrollBarWinLottery5Max.Min := nPostion + 1;
  g_Config.nWinLottery6Max := nPostion;
  EditWinLottery6Max.Text := IntToStr(g_Config.nWinLottery6Min) + '-' + IntToStr(g_Config.nWinLottery6Max);
  EditWinLottery5Max.Text := IntToStr(g_Config.nWinLottery5Min) + '-' + IntToStr(g_Config.nWinLottery5Max);
  ModValue();

end;

procedure TfrmFunctionConfig.ScrollBarWinLotteryRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarWinLotteryRate.Position;
  EditWinLotteryRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  ScrollBarWinLottery1Max.Max := nPostion;
  g_Config.nWinLotteryRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.RefReNewLevelConf();
begin
  EditReNewNameColor1.Value := g_Config.ReNewNameColor[0];
  EditReNewNameColor2.Value := g_Config.ReNewNameColor[1];
  EditReNewNameColor3.Value := g_Config.ReNewNameColor[2];
  EditReNewNameColor4.Value := g_Config.ReNewNameColor[3];
  EditReNewNameColor5.Value := g_Config.ReNewNameColor[4];
  EditReNewNameColor6.Value := g_Config.ReNewNameColor[5];
  EditReNewNameColor7.Value := g_Config.ReNewNameColor[6];
  EditReNewNameColor8.Value := g_Config.ReNewNameColor[7];
  EditReNewNameColor9.Value := g_Config.ReNewNameColor[8];
  EditReNewNameColor10.Value := g_Config.ReNewNameColor[9];
  EditReNewNameColorTime.Value := g_Config.dwReNewNameColorTime div 1000;
  CheckBoxReNewChangeColor.Checked := g_Config.boReNewChangeColor;
  CheckBoxReNewLevelClearExp.Checked := g_Config.boReNewLevelClearExp;
end;

procedure TfrmFunctionConfig.ButtonReNewLevelSaveClick(Sender: TObject);
var
  I: Integer;
begin

  for I := Low(g_Config.ReNewNameColor) to High(g_Config.ReNewNameColor) do
  begin
    Config.WriteInteger('Setup', 'ReNewNameColor' + IntToStr(I), g_Config.ReNewNameColor[I]);
  end;
  Config.WriteInteger('Setup', 'ReNewNameColorTime', g_Config.dwReNewNameColorTime);
  Config.WriteBool('Setup', 'ReNewChangeColor', g_Config.boReNewChangeColor);
  Config.WriteBool('Setup', 'ReNewLevelClearExp', g_Config.boReNewLevelClearExp);

  uModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor1Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor1.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[0] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor2Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor2.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[1] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor3Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor3.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[2] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor4Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor4.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[3] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor5Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor5.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[4] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor6Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor6.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[5] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor7Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor7.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[6] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor8Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor8.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[7] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor9Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor9.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[8] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColor10Change(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditReNewNameColor10.Value;
  if not boOpened then
    Exit;
  g_Config.ReNewNameColor[9] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditReNewNameColorTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwReNewNameColorTime := EditReNewNameColorTime.Value * 1000;
  ModValue();
end;

procedure TfrmFunctionConfig.RefMonUpgrade();
begin
  EditMonUpgradeColor0.Value := g_Config.SlaveColor[0];
  EditMonUpgradeColor1.Value := g_Config.SlaveColor[1];
  EditMonUpgradeColor2.Value := g_Config.SlaveColor[2];
  EditMonUpgradeColor3.Value := g_Config.SlaveColor[3];
  EditMonUpgradeColor4.Value := g_Config.SlaveColor[4];
  EditMonUpgradeColor5.Value := g_Config.SlaveColor[5];
  EditMonUpgradeColor6.Value := g_Config.SlaveColor[6];
  EditMonUpgradeColor7.Value := g_Config.SlaveColor[7];
  EditMonUpgradeColor8.Value := g_Config.SlaveColor[8];
  EditMonUpgradeColor9.Value := g_Config.SlaveColor[9];
  EditMonUpgradeKillCount1.Value := g_Config.MonUpLvNeedKillCount[0];
  EditMonUpgradeKillCount2.Value := g_Config.MonUpLvNeedKillCount[1];
  EditMonUpgradeKillCount3.Value := g_Config.MonUpLvNeedKillCount[2];
  EditMonUpgradeKillCount4.Value := g_Config.MonUpLvNeedKillCount[3];
  EditMonUpgradeKillCount5.Value := g_Config.MonUpLvNeedKillCount[4];
  EditMonUpgradeKillCount6.Value := g_Config.MonUpLvNeedKillCount[5];
  EditMonUpgradeKillCount7.Value := g_Config.MonUpLvNeedKillCount[6];
  EditMonUpLvNeedKillBase.Value := g_Config.nMonUpLvNeedKillBase;
  EditMonUpLvRate.Value := g_Config.nMonUpLvRate;

  CheckBoxMasterDieMutiny.Checked := g_Config.boMasterDieMutiny;
  EditMasterDieMutinyRate.Value := g_Config.nMasterDieMutinyRate;
  EditMasterDieMutinyPower.Value := g_Config.nMasterDieMutinyPower;
  EditMasterDieMutinySpeed.Value := g_Config.nMasterDieMutinySpeed;

  CheckBoxMasterDieMutinyClick(CheckBoxMasterDieMutiny);

  CheckBoxBBMonAutoChangeColor.Checked := g_Config.boBBMonAutoChangeColor;
  EditBBMonAutoChangeColorTime.Value := g_Config.dwBBMonAutoChangeColorTime div 1000;
end;

procedure TfrmFunctionConfig.ButtonMonUpgradeSaveClick(Sender: TObject);
var
  I: Integer;
begin

  Config.WriteInteger('Setup', 'MonUpLvNeedKillBase', g_Config.nMonUpLvNeedKillBase);
  Config.WriteInteger('Setup', 'MonUpLvRate', g_Config.nMonUpLvRate);
  for I := Low(g_Config.MonUpLvNeedKillCount) to High(g_Config.MonUpLvNeedKillCount) do
  begin
    Config.WriteInteger('Setup', 'MonUpLvNeedKillCount' + IntToStr(I), g_Config.MonUpLvNeedKillCount[I]);
  end;

  for I := Low(g_Config.SlaveColor) to High(g_Config.SlaveColor) do
  begin
    Config.WriteInteger('Setup', 'SlaveColor' + IntToStr(I), g_Config.SlaveColor[I]);
  end;
  Config.WriteBool('Setup', 'MasterDieMutiny', g_Config.boMasterDieMutiny);
  Config.WriteInteger('Setup', 'MasterDieMutinyRate', g_Config.nMasterDieMutinyRate);
  Config.WriteInteger('Setup', 'MasterDieMutinyPower', g_Config.nMasterDieMutinyPower);
  Config.WriteInteger('Setup', 'MasterDieMutinyPower', g_Config.nMasterDieMutinySpeed);

  Config.WriteBool('Setup', 'BBMonAutoChangeColor', g_Config.boBBMonAutoChangeColor);
  Config.WriteInteger('Setup', 'BBMonAutoChangeColorTime', g_Config.dwBBMonAutoChangeColorTime);
  Config.WriteInteger('Setup', 'SlavePowerRate', g_Config.nSlavePowerRate);
  Config.WriteBool('Setup', 'MasterRoyaltyDie', g_Config.boMasterRoyaltyDie);
  Config.WriteBool('Setup', 'MasterRoyaltyFullHP', g_Config.boMasterRoyaltyFullHP);
  Config.WriteBool('Setup', 'SlaveRelaxCanStruck', g_Config.boSlaveRelaxCanStruck);

  Config.WriteBool('Setup', 'SlaveNotAttackHuman', g_Config.boSlaveNotAttackHuman);
  Config.WriteBool('Setup', 'SlaveNotAttackHero', g_Config.boSlaveNotAttackHero);
  Config.WriteBool('Setup', 'SlaveLockTarget', g_Config.boSlaveLockTarget);
  Config.WriteBool('Setup', 'SlaveNoLockHuman', g_Config.boSlaveNoLockHuman);

  Config.WriteInteger('Setup', 'Slave9HP', g_Config.nSlave9HP);
  Config.WriteInteger('Setup', 'Slave9AC', g_Config.nSlave9AC);
  Config.WriteInteger('Setup', 'Slave9MAC', g_Config.nSlave9MAC);
  Config.WriteInteger('Setup', 'Slave9DC', g_Config.nSlave9DC);
  Config.WriteInteger('Setup', 'Slave9MoveSpeed', g_Config.nSlave9MoveSpeed);
  Config.WriteInteger('Setup', 'Slave9HitSpeed', g_Config.nSlave9HitSpeed);

  Config.WriteBool('Setup', 'BBAttrPlusAddOnlyMagic', g_Config.boBBAttrPlusAddOnlyMagic);

  Config.WriteBool('Setup', 'BBAttrPlusAddAttack', g_Config.boBBAttrPlusAddAttack);

  // 叠加人物攻击给宝宝(人物属性) chongchong 2014-12-30
  Config.WriteInteger('Setup', 'BBAttrPlusAddAttackForm', g_Config.nBBAttrPlusAddAttackForm);

  // 加攻击倍率
  Config.WriteInteger('Setup', 'BBAttrPlusAddAttackRate', g_Config.nBBAttrPlusAddAttackRate);

  // 加防御 chongchong 2014-12-30
  Config.WriteBool('Setup', 'BBAttrPlusAddDefence', g_Config.boBBAttrPlusAddDefence);

  // 加魔御 chongchong 2014-12-30
  Config.WriteBool('Setup', 'BBAttrPlusAddMagicDefence', g_Config.boBBAttrPlusAddMagicDefence);

  // 加防御倍率
  Config.WriteInteger('Setup', 'BBAttrPlusAddDefenceRate', g_Config.nBBAttrPlusAddDefenceRate);

  // 加HP chongchong 2014-12-30
  Config.WriteBool('Setup', 'BBAttrPlusAddHP', g_Config.boBBAttrPlusAddHP);

  // 加HP倍率
  Config.WriteInteger('Setup', 'BBAttrPlusAddHPRate', g_Config.nBBAttrPlusAddHPRate);

  Config.WriteBool('Setup', 'SlaveKillHumanIncPK', g_Config.boSlaveKillHumanIncPK);
  Config.WriteBool('Setup', 'SlaveDisableStruck', g_Config.boSlaveDisableStruck);
  Config.WriteBool('Setup', 'SlaveAlwaysShowName', g_Config.boSlaveAlwaysShowName);

  Config.WriteBool('Setup', 'SlaveLevelupUseNewAttr', g_Config.boSlaveLevelupUseNewAttr);
  Config.WriteBool('Setup', 'SlaveLevelupAddLowerAttr', g_Config.boSlaveLevelupAddLowerAttr);

  if boSendServerConfig then
  begin
    UserEngine.SendServerConfig();
  end;

  boSendServerConfig := False;
  uModValue();
end;

procedure TfrmFunctionConfig.EditMonUpgradeColor1Change(Sender: TObject);
var
  btColor: Byte;
  seColor: TColorIndexEdit;
begin
  seColor := Sender as TColorIndexEdit;
  btColor := seColor.Value;
  if not boOpened then
    Exit;
  g_Config.SlaveColor[seColor.Tag] := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxReNewChangeColorClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boReNewChangeColor := CheckBoxReNewChangeColor.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxReNewLevelClearExpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boReNewLevelClearExp := CheckBoxReNewLevelClearExp.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditPKFlagNameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditPKFlagNameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btPKFlagNameColor := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditPKLevel1NameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditPKLevel1NameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btPKLevel1NameColor := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditPKLevel2NameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditPKLevel2NameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btPKLevel2NameColor := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditAllyAndGuildNameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditAllyAndGuildNameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btAllyAndGuildNameColor := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditWarGuildNameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditWarGuildNameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btWarGuildNameColor := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditInFreePKAreaNameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditInFreePKAreaNameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btInFreePKAreaNameColor := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonUpgradeKillCount1Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.MonUpLvNeedKillCount[0] := EditMonUpgradeKillCount1.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonUpgradeKillCount2Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.MonUpLvNeedKillCount[1] := EditMonUpgradeKillCount2.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonUpgradeKillCount3Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.MonUpLvNeedKillCount[2] := EditMonUpgradeKillCount3.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonUpgradeKillCount4Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.MonUpLvNeedKillCount[3] := EditMonUpgradeKillCount4.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonUpgradeKillCount5Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.MonUpLvNeedKillCount[4] := EditMonUpgradeKillCount5.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonUpgradeKillCount6Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.MonUpLvNeedKillCount[5] := EditMonUpgradeKillCount6.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonUpgradeKillCount7Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.MonUpLvNeedKillCount[6] := EditMonUpgradeKillCount7.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonUpLvNeedKillBaseChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonUpLvNeedKillBase := EditMonUpLvNeedKillBase.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonUpLvRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonUpLvRate := EditMonUpLvRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxMasterDieMutinyClick(Sender: TObject);
begin
  if CheckBoxMasterDieMutiny.Checked then
  begin
    EditMasterDieMutinyRate.Enabled := True;
    EditMasterDieMutinyPower.Enabled := True;
    EditMasterDieMutinySpeed.Enabled := True;
  end
  else
  begin
    EditMasterDieMutinyRate.Enabled := False;
    EditMasterDieMutinyPower.Enabled := False;
    EditMasterDieMutinySpeed.Enabled := False;
  end;
  if not boOpened then
    Exit;
  g_Config.boMasterDieMutiny := CheckBoxMasterDieMutiny.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMasterDieMutinyRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMasterDieMutinyRate := EditMasterDieMutinyRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMasterDieMutinyPowerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMasterDieMutinyPower := EditMasterDieMutinyPower.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMasterDieMutinySpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMasterDieMutinySpeed := EditMasterDieMutinySpeed.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxBBMonAutoChangeColorClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBBMonAutoChangeColor := CheckBoxBBMonAutoChangeColor.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditBBMonAutoChangeColorTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwBBMonAutoChangeColorTime := EditBBMonAutoChangeColorTime.Value * 1000;
  ModValue();
end;

procedure TfrmFunctionConfig.RefSpiritMutiny();
begin
  CheckBoxSpiritMutiny.Checked := g_Config.boSpiritMutiny;
  EditSpiritMutinyTime.Value := g_Config.dwSpiritMutinyTime div (60 * 1000);
  EditSpiritPowerRate.Value := g_Config.nSpiritPowerRate;
  CheckBoxSpiritMutinyClick(CheckBoxSpiritMutiny);
end;

procedure TfrmFunctionConfig.ButtonSpiritMutinySaveClick(Sender: TObject);
begin

  Config.WriteBool('Setup', 'SpiritMutiny', g_Config.boSpiritMutiny);
  Config.WriteInteger('Setup', 'SpiritMutinyTime', g_Config.dwSpiritMutinyTime);
  Config.WriteInteger('Setup', 'SpiritPowerRate', g_Config.nSpiritPowerRate);

  uModValue();
end;

procedure TfrmFunctionConfig.CheckBoxSpiritMutinyClick(Sender: TObject);
begin
  if CheckBoxSpiritMutiny.Checked then
  begin
    EditSpiritMutinyTime.Enabled := True;
    // EditSpiritPowerRate.Enabled:=True;
  end
  else
  begin
    EditSpiritMutinyTime.Enabled := False;
    EditSpiritPowerRate.Enabled := False;
  end;
  if not boOpened then
    Exit;
  g_Config.boSpiritMutiny := CheckBoxSpiritMutiny.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSpiritMutinyTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSpiritMutinyTime := EditSpiritMutinyTime.Value * 60 * 1000;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSpiritPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSpiritPowerRate := EditSpiritPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMagTammingLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagTammingLevel := EditMagTammingLevel.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMagTammingTargetLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagTammingTargetLevel := EditMagTammingTargetLevel.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMagTammingHPRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagTammingHPRate := EditMagTammingHPRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditTammingCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagTammingCount := EditTammingCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMabMabeHitRandRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMabMabeHitRandRate := EditMabMabeHitRandRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMabMabeHitMinLvLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMabMabeHitMinLvLimit := EditMabMabeHitMinLvLimit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMabMabeHitSucessRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMabMabeHitSucessRate := EditMabMabeHitSucessRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMabMabeHitMabeTimeRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMabMabeHitMabeTimeRate := EditMabMabeHitMabeTimeRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMaxMabMabeHitMabeTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxMabMabeHitMabeTime := seMaxMabMabeHitMabeTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.RefMonSayMsg;
begin
  CheckBoxMonSayMsg.Checked := g_Config.boMonSayMsg;
end;

procedure TfrmFunctionConfig.ButtonMonSayMsgSaveClick(Sender: TObject);
begin

  Config.WriteBool('Setup', 'MonSayMsg', g_Config.boMonSayMsg);

  uModValue();
end;

procedure TfrmFunctionConfig.CheckBoxMonSayMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMonSayMsg := CheckBoxMonSayMsg.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.RefWeaponMakeLuck;
begin
  ScrollBarWeaponMakeUnLuckRate.Min := 1;
  ScrollBarWeaponMakeUnLuckRate.Max := 50;
  ScrollBarWeaponMakeUnLuckRate.Position := g_Config.nWeaponMakeUnLuckRate;

  ScrollBarWeaponMakeLuckPoint1.Min := 1;
  ScrollBarWeaponMakeLuckPoint1.Max := 10;
  ScrollBarWeaponMakeLuckPoint1.Position := g_Config.nWeaponMakeLuckPoint1;

  ScrollBarWeaponMakeLuckPoint2.Min := 1;
  ScrollBarWeaponMakeLuckPoint2.Max := 10;
  ScrollBarWeaponMakeLuckPoint2.Position := g_Config.nWeaponMakeLuckPoint2;

  ScrollBarWeaponMakeLuckPoint3.Min := 1;
  ScrollBarWeaponMakeLuckPoint3.Max := 10;
  ScrollBarWeaponMakeLuckPoint3.Position := g_Config.nWeaponMakeLuckPoint3;

  ScrollBarWeaponMakeLuckPoint2Rate.Min := 1;
  ScrollBarWeaponMakeLuckPoint2Rate.Max := 50;
  ScrollBarWeaponMakeLuckPoint2Rate.Position := g_Config.nWeaponMakeLuckPoint2Rate;

  ScrollBarWeaponMakeLuckPoint3Rate.Min := 1;
  ScrollBarWeaponMakeLuckPoint3Rate.Max := 50;
  ScrollBarWeaponMakeLuckPoint3Rate.Position := g_Config.nWeaponMakeLuckPoint3Rate;
end;

procedure TfrmFunctionConfig.ButtonWeaponMakeLuckDefaultClick(Sender: TObject);
begin
  if Application.MessageBox('是否确认恢复默认设置？', '确认信息', MB_YESNO + MB_ICONQUESTION) <> IDYES then
  begin
    Exit;
  end;
  g_Config.nWeaponMakeUnLuckRate := 20;
  g_Config.nWeaponMakeLuckPoint1 := 1;
  g_Config.nWeaponMakeLuckPoint2 := 3;
  g_Config.nWeaponMakeLuckPoint3 := 7;
  g_Config.nWeaponMakeLuckPoint2Rate := 6;
  g_Config.nWeaponMakeLuckPoint3Rate := 40;
  RefWeaponMakeLuck();
end;

procedure TfrmFunctionConfig.ButtonWeaponMakeLuckSaveClick(Sender: TObject);
begin

  Config.WriteInteger('Setup', 'WeaponMakeUnLuckRate', g_Config.nWeaponMakeUnLuckRate);
  Config.WriteInteger('Setup', 'WeaponMakeLuckPoint1', g_Config.nWeaponMakeLuckPoint1);
  Config.WriteInteger('Setup', 'WeaponMakeLuckPoint2', g_Config.nWeaponMakeLuckPoint2);
  Config.WriteInteger('Setup', 'WeaponMakeLuckPoint3', g_Config.nWeaponMakeLuckPoint3);
  Config.WriteInteger('Setup', 'WeaponMakeLuckPoint2Rate', g_Config.nWeaponMakeLuckPoint2Rate);
  Config.WriteInteger('Setup', 'WeaponMakeLuckPoint3Rate', g_Config.nWeaponMakeLuckPoint3Rate);

  uModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWeaponMakeUnLuckRateChange(Sender: TObject);
var
  nInteger: Integer;
begin
  nInteger := ScrollBarWeaponMakeUnLuckRate.Position;
  EditWeaponMakeUnLuckRate.Text := IntToStr(nInteger);
  if not boOpened then
    Exit;
  g_Config.nWeaponMakeUnLuckRate := nInteger;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWeaponMakeLuckPoint1Change(Sender: TObject);
var
  nInteger: Integer;
begin
  nInteger := ScrollBarWeaponMakeLuckPoint1.Position;
  EditWeaponMakeLuckPoint1.Text := IntToStr(nInteger);
  if not boOpened then
    Exit;
  g_Config.nWeaponMakeLuckPoint1 := nInteger;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWeaponMakeLuckPoint2Change(Sender: TObject);
var
  nInteger: Integer;
begin
  nInteger := ScrollBarWeaponMakeLuckPoint2.Position;
  EditWeaponMakeLuckPoint2.Text := IntToStr(nInteger);
  if not boOpened then
    Exit;
  g_Config.nWeaponMakeLuckPoint2 := nInteger;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWeaponMakeLuckPoint2RateChange(Sender: TObject);
var
  nInteger: Integer;
begin
  nInteger := ScrollBarWeaponMakeLuckPoint2Rate.Position;
  EditWeaponMakeLuckPoint2Rate.Text := IntToStr(nInteger);
  if not boOpened then
    Exit;
  g_Config.nWeaponMakeLuckPoint2Rate := nInteger;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWeaponMakeLuckPoint3Change(Sender: TObject);
var
  nInteger: Integer;
begin
  nInteger := ScrollBarWeaponMakeLuckPoint3.Position;
  EditWeaponMakeLuckPoint3.Text := IntToStr(nInteger);
  if not boOpened then
    Exit;
  g_Config.nWeaponMakeLuckPoint3 := nInteger;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarWeaponMakeLuckPoint3RateChange(Sender: TObject);
var
  nInteger: Integer;
begin
  nInteger := ScrollBarWeaponMakeLuckPoint3Rate.Position;
  EditWeaponMakeLuckPoint3Rate.Text := IntToStr(nInteger);
  if not boOpened then
    Exit;
  g_Config.nWeaponMakeLuckPoint3Rate := nInteger;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill71PullPlayObjectClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill71PullPlayObject := chkSkill71PullPlayObject.Checked;
  // chkSkill71PullCrossInSafeZone.Enabled := chkSkill71PullPlayObject.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkPlayObjectReduceMPClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPlayObjectReduceMP := chkPlayObjectReduceMP.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroTargetRangeLimitClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroTargetRangeLimit := seHeroTargetRangeLimit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.SpinEditMagDelayTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagDelayTimeDoubly := SpinEditMagDelayTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill41MbAttackSlaveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill41MbAttackSlave := chkSkill41MbAttackSlave.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditItemNameChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sChangeUseItemName := Trim(EditItemName.Text);
  ModValue();
end;

procedure TfrmFunctionConfig.seDedingMagicCDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDedingMagicCD := seDedingMagicCD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDedingAllowPKClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDedingAllowPK := chkDedingAllowPK.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDedingDisabledPKClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDedingDisabledPK := chkDedingDisabledPK.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill71PullCrossInSafeZoneClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill71PullCrossInSafeZone := chkSkill71PullCrossInSafeZone.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMerchantNameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditMerchantNameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btMerchantNameColor := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.seMerchant273NameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seMerchant273NameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btMerchant273NameColor := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.seGuardNameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := seGuardNameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btGuardNameColor := btColor;
  ModValue();
end;

procedure TfrmFunctionConfig.seHPRockRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHPRockRate := seHPRockRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHPRockTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHPRockTime := seHPRockTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHPRockAddValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHPRockAddValue := seHPRockAddValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHPRockDecValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHPRockDecValue := seHPRockDecValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMPRockRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMPRockRate := seMPRockRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMPRockTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMPRockTime := seMPRockTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMPRockAddValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMPRockAddValue := seMPRockAddValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMPRockDecValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMPRockDecValue := seMPRockDecValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHMPRockRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHMPRockRate := seHMPRockRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHMPRockTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHMPRockTime := seHMPRockTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHMPRockAddValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHMPRockAddValue := seHMPRockAddValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHMPRockDecValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHMPRockDecValue := seHMPRockDecValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxHeroPickUpItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroPickUpItem := CheckBoxHeroPickUpItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroOnlyPickMonsterItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroOnlyPickMonsterItem := chkHeroOnlyPickMonsterItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxHeroShowMasterNameClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroShowMasterName := CheckBoxHeroShowMasterName.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroKillMonExpRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroKillMonExpRate := seHeroKillMonExpRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroNotKillMonExpRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroNotKillMonExpRate := seHeroNotKillMonExpRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditHeroRecallTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRecallHeroTime := EditHeroRecallTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditHeroWarrorAttackTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWarrorAttackTime := EditHeroWarrorAttackTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditHeroWizardAttackTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWizardAttackTime := EditHeroWizardAttackTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditHeroTaoistAttackTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroTaoistAttackTime := EditHeroTaoistAttackTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditHeroWarrorWalkTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWarrorWalkTime := EditHeroWarrorWalkTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditHeroWizardWalkTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWizardWalkTime := EditHeroWizardWalkTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditHeroTaoistWalkTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroTaoistWalkTime := EditHeroTaoistWalkTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditHeroSuffixNameChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sHeroSuffixName := Trim(EditHeroSuffixName.Text);
  ModValue();
end;

procedure TfrmFunctionConfig.EditHeroNameColorChange(Sender: TObject);
var
  btColor: Byte;
begin
  btColor := EditHeroNameColor.Value;
  if not boOpened then
    Exit;
  g_Config.btHeroNameColor := btColor;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.ButtonHeroOptionSaveClick(Sender: TObject);
var
  I: Integer;
  dwExp: Int64;
  NeedExps: TLevelNeedExp;
begin
  { { TODO -ochongchong -c新增 : 英雄升级经验配置 【2013-08-14】 }
  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    dwExp := StrToInt64Def(GridLevelExp.Cells[1, I], 0);
    if (dwExp <= 0) or (dwExp > High(LongWord)) then
    begin
      Application.MessageBox(PChar('等级 ' + IntToStr(I) + ' 升级经验设置错误！！！'), '错误信息', MB_OK + MB_ICONERROR);
      GridLevelExp.Row := I;
      GridLevelExp.SetFocus;
      Exit;
    end;
    NeedExps[I] := dwExp;
  end;

  g_Config.dwHeroNeedExps := NeedExps;
  for I := 1 to 1000 do
    ExpConfig.WriteString('HeroExp', 'Level' + IntToStr(I), IntToStr(g_Config.dwHeroNeedExps[I]));

  Config.WriteBool('Setup', 'HeroGetAllExp', g_Config.boHeroGetAllExp);
  Config.WriteBool('Setup', 'HumanGetAllExp', g_Config.boHumanGetAllExp);

  Config.WriteInteger('Setup', 'HeroKillMonExpRate', g_Config.nHeroKillMonExpRate);
  Config.WriteInteger('Setup', 'HeroNotKillMonExpRate', g_Config.nHeroNotKillMonExpRate);

  Config.WriteBool('Setup', 'AllowCopySelf', g_Config.boAllowCopySelf);
  Config.WriteBool('Setup', 'HeroUseBagItem', g_Config.boAllowCopySelf);
  Config.WriteBool('Setup', 'MonUseBagItem', g_Config.boAllowCopySelf);
  Config.WriteInteger('Setup', 'LimitExpLevel', g_Config.nLimitExpLevel);
  Config.WriteInteger('Setup', 'LimitExpValue', g_Config.nLimitExpValue);

  Config.WriteInteger('Setup', 'WarrorAttackTime', g_Config.dwHeroWarrorAttackTime);
  Config.WriteInteger('Setup', 'WizardAttackTime', g_Config.dwHeroWizardAttackTime);
  Config.WriteInteger('Setup', 'TaoistAttackTime', g_Config.dwHeroTaoistAttackTime);

  Config.WriteInteger('Setup', 'WarrorWalkTime', g_Config.dwHeroWarrorWalkTime);
  Config.WriteInteger('Setup', 'WizardWalkTime', g_Config.dwHeroWizardWalkTime);
  Config.WriteInteger('Setup', 'TaoistWalkTime', g_Config.dwHeroTaoistWalkTime);

  Config.WriteInteger('Setup', 'HeroAvoidTime', g_Config.dwHeroAvoidTime);

  Config.WriteBool('Setup', 'HeroHitCmp', g_Config.boHeroHitCmp);
  Config.WriteInteger('Setup', 'WarrCmpInvTime', g_Config.nWarrCmpInvTime);
  // Config.WriteInteger('Setup', 'WizaCmpInvTime', g_Config.nWizaCmpInvTime);
  // Config.WriteInteger('Setup', 'TaosCmpInvTime', g_Config.nTaosCmpInvTime);

  Config.WriteInteger('Setup', 'RecallHeroTime', g_Config.nRecallHeroTime);
  Config.WriteInteger('Setup', 'RecallDeputyHeroTime', g_Config.nRecallDeputyHeroTime);
  // 英雄尸体清理间隔 piaoyun 2013-08-10
  Config.WriteInteger('Setup', 'ClearHeroGhostTick', g_Config.nClearHeroGhostTick);
  Config.WriteInteger('Setup', 'HeroNameColor', g_Config.btHeroNameColor);
  Config.WriteBool('Setup', 'HeroShowMasterName', g_Config.boHeroShowMasterName);
  Config.WriteInteger('Setup', 'HeroNeedMagicItem', g_Config.nHeroNeedMagicItem);
  Config.WriteBool('Setup', 'HeroPickUpItem', g_Config.boHeroPickUpItem);
  Config.WriteBool('Setup', 'HeroWarrorAttack', g_Config.boWarrorAttack);
  Config.WriteBool('Setup', 'HeroNotAvoidLastHinter', g_Config.boHeroNotAvoidLastHinter);
  Config.WriteBool('Setup', 'HeroOnlyPickMonsterItem', g_Config.boHeroOnlyPickMonsterItem);

  for I := Low(g_Config.HeroBagItemCounts) to High(g_Config.HeroBagItemCounts) do
  begin
    Config.WriteInteger('Setup', 'HeroBagItemCount' + IntToStr(I), g_Config.HeroBagItemCounts[I]);
  end;
  Config.WriteInteger('Setup', 'NeedGuardLevel', g_Config.nNeedGuardLevel);
  Config.WriteInteger('Setup', 'GuardRange', g_Config.nGuardRange);

  Config.WriteBool('Setup', 'DieScatterHeroBag', g_Config.boDieScatterHeroBag);
  Config.WriteInteger('Setup', 'DieScatterHeroBagRate', g_Config.nDieScatterHeroBagRate);
  Config.WriteBool('Setup', 'DieRedScatterHeroBagAll', g_Config.boDieRedScatterHeroBagAll);
  Config.WriteInteger('Setup', 'DieDropHeroUseItemRate', g_Config.nDieDropHeroUseItemRate);
  Config.WriteInteger('Setup', 'DieRedDropHeroUseItemRate', g_Config.nDieRedDropHeroUseItemRate);
  Config.WriteBool('Setup', 'KillByHumanDropHeroUseItem', g_Config.boKillByHumanDropHeroUseItem);
  Config.WriteBool('Setup', 'KillByMonstDropHeroUseItem', g_Config.boKillByMonstDropHeroUseItem);

  Config.WriteBool('Setup', 'KillByMonstDropHeroJewelryBoxItem', g_Config.boKillByMonstDropHeroJewelryBoxItem);
  Config.WriteBool('Setup', 'KillByHumanDropHeroJewelryBoxItem', g_Config.boKillByHumanDropHeroJewelryBoxItem);

  Config.WriteBool('Setup', 'KillByMonstDropHeroGodBlessItem', g_Config.boKillByMonstDropHeroGodBlessItem);
  Config.WriteBool('Setup', 'KillByHumanDropHeroGodBlessItem', g_Config.boKillByHumanDropHeroGodBlessItem);

  Config.WriteInteger('Setup', 'DropHeroJewelryBoxItemRate', g_Config.nDropHeroJewelryBoxItemRate);
  Config.WriteInteger('Setup', 'DropHeroGodBlessItemRate', g_Config.nDropHeroGodBlessItemRate);

  Config.WriteBool('Setup', 'HeroCanUseMootebo', g_Config.boHeroCanUseMootebo);
  Config.WriteBool('Setup', 'HeroCanCallBB', g_Config.boHeroCanCallBB);
  Config.WriteInteger('Setup', 'HeroCallBBCount', g_Config.dwHeroCallBBCount);
  Config.WriteBool('Setup', 'HeroNoTargetRecallBB', g_Config.boHeroNoTargetRecallBB);

  Config.WriteInteger('Setup', 'HeroDieExpRate', g_Config.dwHeroDieExpRate);

  Config.WriteInteger('Setup', 'HeroMasterStartLevel', g_Config.dwHeroMasterStartLevel);
  Config.WriteInteger('Setup', 'HeroSlaveStartLevel', g_Config.dwHeroSlaveStartLevel);

  Config.WriteBool('Setup', 'HeroDisableSafeZoneProtect', g_Config.boHeroDisableSafeZoneProtect);
  Config.WriteBool('Setup', 'HeroNoMoveOnSleep', g_Config.boHeroNoMoveOnSleep);
  // Config.WriteBool('Setup', 'HeroNoSkillUseBaseAttack', g_Config.boHeroNoSkillUseBaseAttack);

  Config.WriteInteger('Setup', 'Hero700HPValue', g_Config.dwHero700HPValue);
  Config.WriteBool('Setup', 'Hero700HPUseBaseAttack', g_Config.boHero700HPUseBaseAttack);

  Config.WriteInteger('Setup', 'HeroWarrHPMPRate', g_Config.dwHeroWarrHPMPRate);
  Config.WriteInteger('Setup', 'HeroWizardHPMPRate', g_Config.dwHeroWizardHPMPRate);
  Config.WriteInteger('Setup', 'HeroTaosHPMPRate', g_Config.dwHeroTaosHPMPRate);
  Config.WriteBool('Setup', 'CreditPointWithLevel', g_Config.boCreditPointWithLevel);

  Config.WriteBool('Setup', 'HeroFollowMasterWithDiffScreen', g_Config.boHeroFollowMasterWithDiffScreen);
  Config.WriteBool('Setup', 'HeroAutoSuperShiled', g_Config.boHeroAutoSuperShiled);

  Config.WriteBool('Setup', 'HeroDFAvoidTargetRight', g_Config.boHeroDFAvoidTargetRight);

  // 英雄计算武器速度 chongchong 2013-08-13
  Config.WriteBool('Setup', 'HeroCalcWeaponSpeed', g_Config.boHeroCalcWeaponSpeed);

  // 英雄技能四级触发 chongchong 2013-08-13
  Config.WriteInteger('Setup', 'HeroGotoLV4', g_Config.dwHeroGotoLV4);

  // 英雄技能四级触发杀伤力增加 chongchong 2013-08-13
  Config.WriteInteger('Setup', 'HeroPowerLV4', g_Config.dwHeroPowerLV4);

  // 英雄忠诚度 - 召唤增加 chongchong 2013-08-13
  Config.WriteInteger('Setup', 'HeroFealtyCallAdd', g_Config.dwHeroFealtyCallAdd);

  // 英雄忠诚度 - 召回减少 chongchong 2013-08-13
  Config.WriteInteger('Setup', 'HeroFealtyCallBackDel', g_Config.dwHeroFealtyCallBackDel);

  // 英雄忠诚度 - 获得经验点 chongchong 2013-08-13
  Config.WriteInteger('Setup', 'HeroFealtyExp', g_Config.dwHeroFealtyExp);

  // 英雄忠诚度 - 获得经验点增加的忠诚度 chongchong 2013-08-13
  Config.WriteInteger('Setup', 'HeroFealtyExpAdd', g_Config.dwHeroFealtyExpAdd);

  // 英雄忠诚度 - 死亡减少 chongchong 2013-08-13
  Config.WriteInteger('Setup', 'HeroFealtyDeathDel', g_Config.dwHeroFealtyDeathDel);

  Config.WriteBool('Setup', 'HeroTaosAutoChangePoison', g_Config.boHeroTaosAutoChangePoison);

  Config.WriteInteger('Setup', 'HeroTaoUsePoisonMinHP', g_Config.dwHeroTaoUsePoisonMinHP);

  Config.WriteBool('Setup', 'HeroDisableStruck', g_Config.boHeroDisableStruck);

  Config.WriteBool('Setup', 'HeroDisableSelfStruck', g_Config.boHeroDisableSelfStruck);

  Config.WriteString('Setup', 'HeroSuffixName', g_Config.sHeroSuffixName);
  Config.WriteString('Setup', 'HeroSayPrefix', g_Config.sHeroSayPrefix);

  // 杀英雄武器诅咒 chongchong 2013-08-24
  Config.WriteBool('Setup', 'KillHeroWeaponUnlock', g_Config.boKillHeroWeaponUnlock);

  // 杀英雄武器诅咒机率 chongchong 2013-08-24
  Config.WriteInteger('Setup', 'KillHeroWeaponUnlockRate', g_Config.dwKillHeroWeaponUnlockRate);

  // 英雄战士默认技能 chongchong 2013-08-24
  Config.WriteInteger('Setup', 'HeroWarriorDefaultSkill', g_Config.dwHeroWarriorDefaultSkill);

  // 全职英雄攻击范围 chongchong 2013-09-13
  // Config.WriteInteger('Setup', 'HeroAttackRange', g_Config.dwHeroAttackRange);

  Config.WriteInteger('Server', 'HeroLimit', g_dwHeroLimit);
  Config.WriteInteger('Setup', 'HeroRunIntervalTime', g_Config.dwHeroRunTime);
  Config.WriteInteger('Setup', 'HeroWarrAttackMoveRate', g_Config.dwHeroWarrAttackMoveRate);
  Config.WriteBool('Setup', 'HeroTargetAgainNoMove', g_Config.boHeroTargetAgainNoMove);
  Config.WriteBool('Setup', 'HeroTargetAgainNoMoveDF', g_Config.boHeroTargetAgainNoMoveDF);

  Config.WriteInteger('Setup', 'HeroLogonTimeMasterDie', g_Config.dwHeroLogonTimeMasterDie);

  Config.WriteInteger('Setup', 'HeroWarrAttacSkillErgumRate', g_Config.dwHeroWarrAttacSkillErgumRate);

  Config.WriteInteger('Setup', 'HeroWarrAttakNear', g_Config.dwHeroWarrAttakNear);
  Config.WriteInteger('Setup', 'HeroWarrNearFireSword', g_Config.dwHeroWarrNearFireSword);

  Config.WriteBool('Setup', 'HeroStateDlgNoMove', g_Config.boHeroStateDlgNoMove);
  Config.WriteBool('Setup', 'HeroForcePeaceMode', g_Config.boHeroForcePeaceMode);
  Config.WriteInteger('Setup', 'HeroAttackHumPowerRate', g_Config.nHeroAttackHumPowerRate);
  Config.WriteInteger('Setup', 'HeroAttackMonPowerRate', g_Config.nHeroAttackMonPowerRate);
  Config.WriteInteger('Setup', 'MonAttackHeroPowerRate', g_Config.nMonAttackHeroPowerRate);
  Config.WriteInteger('Setup', 'HumanAttackHeroPowerRate', g_Config.nHumanAttackHeroPowerRate);

  for I := 0 to Length(g_Config.boHeroStatus) - 1 do
  begin
    Config.WriteBool('Setup', 'boHeroStatus' + IntToStr(I), g_Config.boHeroStatus[I]);
  end;

  Config.WriteBool('Setup', 'DiableHeroRun', g_Config.boDiableHeroRun);
  Config.WriteBool('Setup', 'HeroRunHum', g_Config.boHeroRunHum);
  Config.WriteBool('Setup', 'HeroRunMon', g_Config.boHeroRunMon);
  Config.WriteBool('Setup', 'HeroRunNpc', g_Config.boHeroRunNpc);
  Config.WriteBool('Setup', 'HeroRunGuard', g_Config.boHeroRunGuard);
  Config.WriteBool('Setup', 'HeroWarDisHumRun', g_Config.boHeroWarDisHumRun);
  Config.WriteBool('Setup', 'HeroWarHreoRun', g_Config.boHeroWarHreoRun);
  Config.WriteBool('Setup', 'HeroSafeAreaLimited', g_Config.boHeroSafeAreaLimited);
  Config.WriteBool('Setup', 'HeroSafeAreaDisNpcRun', g_Config.boHeroSafeAreaDisNpcRun);
  Config.WriteBool('Setup', 'SafeAreaDisShopStallHeroRun', g_Config.boSafeAreaDisShopStallHeroRun);
  Config.WriteBool('Setup', 'SafeAreaDisOffLineHeroRun', g_Config.boSafeAreaDisOffLineHeroRun);
  Config.WriteBool('Setup', 'DisableMonsterAttackHero', g_Config.boDisableMonsterAttackHero);
  Config.WriteBool('Setup', 'DisableHeroAttackMonster', g_Config.boDisableHeroAttackMonster);

  Config.WriteBool('Setup', 'HeroKillMonTrigger', g_Config.boHeroKillMonTrigger);

  Config.WriteInteger('Setup', 'HeroTargetRangeLimit', g_Config.nHeroTargetRangeLimit);

  ExpConfig.WriteInteger('Setup', 'HeroHighLevel', g_Config.dwHeroHighLevel);
  ExpConfig.WriteInteger('Setup', 'HeroHighLevelGetExp', g_Config.dwHeroHighLevelGetExp);
  ExpConfig.WriteInteger('Setup', 'HeroLevel1000FixedExp', g_Config.dwHeroLevel1000FixedExp);

  Config.WriteInteger('Setup', 'HeroJoinAttackFlyRange', g_Config.nHeroJoinAttackFlyRange);
  Config.WriteInteger('Setup', 'HeroLockFlyRange', g_Config.nHeroLockFlyRange);
  Config.WriteInteger('Setup', 'HeroProtectFlyRange', g_Config.nHeroProtectFlyRange);
  if boSendServerConfig then
  begin
    UserEngine.SendServerConfig();
    boSendServerConfig := False;
  end;

  uModValue();
end;

procedure TfrmFunctionConfig.seSkill60PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill60PowerRate := seSkill60PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill60PowerRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill60PowerRange := seSkill60PowerRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill60NotMagBubbleDefenceClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill60NotMagBubbleDefence := chkSkill60NotMagBubbleDefence.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill61PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill61PowerRate := seSkill61PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill61NotMagBubbleDefenceClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill61NotMagBubbleDefence := chkSkill61NotMagBubbleDefence.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill62PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill62PowerRate := seSkill62PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill63PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill63PowerRate := seSkill63PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill63PowerRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill63PowerRange := seSkill63PowerRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill63GreenPoisonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill63GreenPoison := chkSkill63GreenPoison.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill64PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill64PowerRate := seSkill64PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill64PowerRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill64PowerRange := seSkill64PowerRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill64MakeStoneClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill64MakeStone := chkSkill64MakeStone.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill65PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill65PowerRate := seSkill65PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill65PowerRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill65PowerRange := seSkill65PowerRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMaxAngryValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMaxAngryValue := EditMaxAngryValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditAddAngryValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAddAngryValue := EditAddAngryValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditDecFirDragonPointChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDecFirDragonPoint := EditDecFirDragonPoint.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditAddAngryValueTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAddAngryValueTime := EditAddAngryValueTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.ComboBoxBagItemCountChange(Sender: TObject);
begin
  EditNeedLevel.Value := Integer(ComboBoxBagItemCount.Items.Objects[ComboBoxBagItemCount.ItemIndex]);
  EditNeedLevel.Enabled := True;
end;

procedure TfrmFunctionConfig.EditNeedLevelChange(Sender: TObject);

  procedure RefBagcount;
  var
    I: Integer;
  begin
    for I := 0 to ComboBoxBagItemCount.Items.Count - 1 do
    begin
      g_Config.HeroBagItemCounts[I] := Integer(ComboBoxBagItemCount.Items.Objects[I]);
    end;
  end;

begin
  if not boOpened then
    Exit;
  ComboBoxBagItemCount.Items.Objects[ComboBoxBagItemCount.ItemIndex] := TObject(EditNeedLevel.Value);
  RefBagcount;
  ModValue();
end;

procedure TfrmFunctionConfig.chkWarrorAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boWarrorAttack := chkWarrorAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditNeedGuardLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeedGuardLevel := EditNeedGuardLevel.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditGuardRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGuardRange := EditGuardRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroGetAllExpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroGetAllExp := chkHeroGetAllExp.Checked;
  seHeroKillMonExpRate.Enabled := not chkHeroGetAllExp.Checked;

  if chkHeroGetAllExp.Checked then
  begin
    chkHumanGetAllExp.Checked := False;
    g_Config.boHumanGetAllExp := False;
  end;

  ModValue();
end;

procedure TfrmFunctionConfig.EditSkill56PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill56PowerRate := EditSkill56PowerRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.EditSkill58PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill58PowerRate := EditSkill58PowerRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.seFireHitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFireHitWaitTime := seFireHitWaitTime.Value;
  boSendServerConfig := True;
  ModValue()
end;

procedure TfrmFunctionConfig.seSkill42PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill42PowerRate := seSkill42PowerRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.seSkill40PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill40PowerRate := seSkill40PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill43HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill43HitWaitTime := seSkill43HitWaitTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSkill43PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill43PowerRate := EditSkill43PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seCopySelfMaxCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCopySelfMaxCount := seCopySelfMaxCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seCopySelfExistTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCopySelfExistTime := seCopySelfExistTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkCopySelfNonUseSpellPointClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCopySelfNonUseSpellPoint := chkCopySelfNonUseSpellPoint.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkAlwaysFollowMasterAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boAlwaysFollowMasterAttack := chkAlwaysFollowMasterAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMagicItemRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagicItemRate := EditMagicItemRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.ButtonOffLineSaveClick(Sender: TObject);
begin
  g_Config.sSetOffLineLoginMapName := Trim(edtSetOffLineLoginMapName.Text);

  Config.WriteBool('Setup', 'OffLineLoginSafeArea', g_Config.boOffLineLoginSafeArea);
  Config.WriteInteger('Setup', 'OffLineLoginMapName', g_Config.btOffLineLoginMapName);
  Config.WriteString('Setup', 'SetOffLineLoginMapName', g_Config.sSetOffLineLoginMapName);
  Config.WriteBool('Setup', 'MonNoAttackOffLinePlayer', g_Config.boMonNoAttackOffLinePlayer);
  uModValue();
end;

procedure TfrmFunctionConfig.CheckBoxOffLineLoginSafeAreaClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOffLineLoginSafeArea := CheckBoxOffLineLoginSafeArea.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.RadioButtonOffLineLoginMapName1Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if RadioButtonOffLineLoginMapName1.Checked then
    g_Config.btOffLineLoginMapName := 0
  else
    g_Config.btOffLineLoginMapName := 1;
  ModValue();
end;

procedure TfrmFunctionConfig.RadioButtonOffLineLoginMapName2Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if RadioButtonOffLineLoginMapName2.Checked then
    g_Config.btOffLineLoginMapName := 1
  else
    g_Config.btOffLineLoginMapName := 0;
  ModValue();
end;

procedure TfrmFunctionConfig.RadioGroupHumNeedMagicItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumNeedMagicItem := RadioGroupHumNeedMagicItem.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.seMagicNewLevelPowerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRates[cbbMagicNewLevel.ItemIndex] <> seMagicNewLevelPower.Value then
    begin
      g_Config.NewLevelMagicPowerRates[cbbMagicNewLevel.ItemIndex] := seMagicNewLevelPower.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevelChange(Sender: TObject);
begin
  seMagicNewLevelPower.Value := g_Config.NewLevelMagicPowerRates[cbbMagicNewLevel.ItemIndex];
end;

procedure TfrmFunctionConfig.EditSWordHitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSWordHitWaitTime := EditSWordHitWaitTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill42HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill42HitWaitTime := seSkill42HitWaitTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSkill66HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill66CD := EditSkill66HitWaitTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill66HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill66CD := seHeroSkill66HitWaitTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill72HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill72CD := seSkill72HitWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill71CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill71CD := seSkill71CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHumSkill66HighPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumSkill66HighPowerRate := seHumSkill66HighPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill66HighPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill66HighPowerRate := seHeroSkill66HighPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill66PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill66PowerRate := seHeroSkill66PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroSkill66HighAttackNoUseRateClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroSkill66HighAttackNoUseRate := chkHeroSkill66HighAttackNoUseRate.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill66HighAttackRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill66HighAttackRate := seHeroSkill66HighAttackRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSkill57AddHPRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill57AddHPRate := EditSkill57AddHPRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSkill58AttackRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill58AttackRange := EditSkill58AttackRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditRecallBigDogWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRecallBigDogWaitTime := EditRecallBigDogWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMonthSpiritHighPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonthSpiritHighPowerRate := EditMonthSpiritHighPowerRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.EditMonthSpiritHighAttackRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonthSpiritHighAttackRate := EditMonthSpiritHighAttackRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxMonthSpiritUseMasterMPClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMonthSpiritUseMasterMP := CheckBoxMonthSpiritUseMasterMP.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxMonthSpiritAttackSameClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMonthSpiritAttackSame := CheckBoxMonthSpiritAttackSame.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seMonthSpiritCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonthSpiritCount := seMonthSpiritCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditBigDogzCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBigDogzCount := EditBigDogzCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSuperShiledValidTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSuperShiledValidTime := seSuperShiledValidTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seLastSuperShiledTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nLastSuperShiledTime := seLastSuperShiledTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSuperShiledPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSuperShiledPowerRate := seSuperShiledPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seCloseSuperShiledRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCloseSuperShiledRate := seCloseSuperShiledRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seOpenSuperShiledRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nOpenSuperShiledRate := seOpenSuperShiledRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkUseSkillCloseSuperShiled0Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.UseSkillCloseSuperShileds[TCheckBox(Sender).Tag] := TCheckBox(Sender).Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxAutoOpenSuperShiledClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boAutoOpenSuperShiled := CheckBoxAutoOpenSuperShiled.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkShowSuperShiledEffectClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowSuperShiledEffect := chkShowSuperShiledEffect.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkRecallManySlave1Click(Sender: TObject);
begin
  if not boOpened then
    Exit;

  g_Config.boRecallManySlave1 := chkRecallManySlave1.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxOpenMapEventClick(Sender: TObject);
begin
  if not boOpened then
    Exit;

  g_Config.boOpenMapEvent := CheckBoxOpenMapEvent.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.ButtonMyShopSaveClick(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'MaxMyShopSellingItemCount', g_Config.nMaxMyShopSellingItemCount);
  Config.WriteInteger('Setup', 'MaxMyShopStorageItemCount', g_Config.nMaxMyShopStorageItemCount);
  Config.WriteBool('Setup', 'OfflineCloseMyShop', g_Config.boOfflineCloseMyShop);
  Config.WriteBool('Setup', 'ProhibitModifyPrices', g_Config.boProhibitModifyPrices);
  Config.WriteBool('Setup', 'UseHeroM2Shop', g_Config.boUseHeroM2Shop);
  Config.WriteBool('Setup', 'InfinityStorage', g_Config.boInfinityStorage);
  Config.WriteInteger('Setup', 'InfinityStorageCount', g_Config.nInfinityStorageCount);

  Config.WriteBool('Setup', 'MyShopGold', g_Config.boMyShopGold);
  Config.WriteBool('Setup', 'MyShopGameGold', g_Config.boMyShopGameGold);
  Config.WriteBool('Setup', 'MyShopGameDiamond', g_Config.boMyShopGameDiamond);
  Config.WriteBool('Setup', 'MyShopGameGird', g_Config.boMyShopGameGird);
  Config.WriteBool('Setup', 'MyShopGamePoint', g_Config.boMyShopGamePoint);

  Config.WriteBool('Setup', 'OpenSelfShop', g_Config.boOpenSelfShop);
  Config.WriteBool('Setup', 'SafeZoneShop', g_Config.boSafeZoneShop);
  Config.WriteBool('Setup', 'MapShop', g_Config.boMapShop);

  // 摆摊期间无敌模式 piaoyun 2013-08-17
  Config.WriteBool('Setup', 'ShopStallCanNotAttack', g_Config.boShopStallCanNotAttack);

  Config.WriteInteger('Setup', 'SellOffGoldTaxRate', g_Config.dwSellOffGoldTaxRate);
  Config.WriteInteger('Setup', 'SellOffGameGoldTaxRate', g_Config.dwSellOffGameGoldTaxRate);

  Config.WriteInteger('Setup', 'SellOffGameDiamondTaxRate', g_Config.dwSellOffGameDiamondTaxRate);
  Config.WriteInteger('Setup', 'SellOffGameGirdTaxRate', g_Config.dwSellOffGameGirdTaxRate);
  Config.WriteInteger('Setup', 'SellOffGamePointTaxRate', g_Config.dwSellOffGamePointTaxRate);

  Config.WriteBool('Setup', 'ShopHeadPic', g_Config.boShopHeadPic);
  Config.WriteInteger('Setup', 'JSOfGameGoldTaxRate', g_Config.dwJSOfGameGoldTaxRate);

  Config.WriteBool('Setup', 'EnabledMySellShopItemTime', g_Config.boEnabledMySellShopItemTime);
  Config.WriteInteger('Setup', 'MySellShopItemTime', g_Config.nMySellShopItemTime);
  Config.WriteInteger('Setup', 'MyShopOperateInterval', g_Config.nMyShopOperateInterval);

  Config.WriteInteger('Setup', 'MySellShowItemNamLen', g_Config.nMySellShowItemNamLen);

  UserEngine.ChangeMyShopType;
  UserEngine.SendServerConfig;
  uModValue();
end;

procedure TfrmFunctionConfig.EditMaxMyShopSellingItemCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxMyShopSellingItemCount := EditMaxMyShopSellingItemCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMaxMyShopStorageItemCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxMyShopStorageItemCount := EditMaxMyShopStorageItemCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSlavePowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSlavePowerRate := EditSlavePowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMagicLockRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagicLockRange := EditMagicLockRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxKillByMonstDropHeroUseItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByMonstDropHeroUseItem := CheckBoxKillByMonstDropHeroUseItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxKillByHumanDropHeroUseItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByHumanDropHeroUseItem := CheckBoxKillByHumanDropHeroUseItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxDieScatterHeroBagClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDieScatterHeroBag := CheckBoxDieScatterHeroBag.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxDieRedScatterHeroBagAllClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDieRedScatterHeroBagAll := CheckBoxDieRedScatterHeroBagAll.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarDieDropHeroUseItemRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarDieDropHeroUseItemRate.Position;
  EditDieDropHeroUseItemRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDieDropHeroUseItemRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarDieRedDropHeroUseItemRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarDieRedDropHeroUseItemRate.Position;
  EditDieRedDropHeroUseItemRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDieRedDropHeroUseItemRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.ScrollBarDieScatterHeroBagRateChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := ScrollBarDieScatterHeroBagRate.Position;
  EditDieScatterHeroBagRate.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDieScatterHeroBagRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxDisableChangeMapFireCrossClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableChangeMapFireCross := CheckBoxDisableChangeMapFireCross.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditFireCrossMaxTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFireCrossMaxTime := EditFireCrossMaxTime.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.EditFireCrossPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFireCrossPowerRate := EditFireCrossPowerRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.ButtonOtherClick(Sender: TObject);
begin
  g_Config.sMysteriousManName := EditMysteriousManName.Text;
  if Trim(g_Config.sMysteriousManName) = '' then
  begin
    g_Config.sMysteriousManName := '神秘人';
    EditMysteriousManName.Text := '神秘人';
  end;
  Config.WriteBool('Setup', 'DeleteItemDuraZero', g_Config.boDeleteItemDuraZero);
  Config.WriteString('Setup', 'MysteriousManName', g_Config.sMysteriousManName);
  Config.WriteInteger('Setup', 'DamageItemDuraRate', g_Config.nDamageItemDuraRate);

  Config.WriteBool('Setup', 'DuraChangeLight', g_Config.boDuraChangeLight);
  Config.WriteBool('Setup', 'PoisonWeaponCanMagicAttack', g_Config.boPoisonWeaponCanMagicAttack);
  Config.WriteBool('Setup', 'PoisonWeaponCanHitAllTarget', g_Config.boPoisonWeaponCanHitAllTarget);
  Config.WriteInteger('Setup', 'QueryBagItemsTime', g_Config.nQueryBagItemsTime);
  Config.WriteBool('Setup', 'ShowRefreshBagMsg', g_Config.boShowRefreshBagMsg);

  Config.WriteInteger('Setup', 'MaxLuckMaxPower', g_Config.nMaxLuckMaxPower);
  Config.WriteBool('Setup', 'LuckUseNewAlgorism', g_Config.boLuckUseNewAlgorism);

  // 虹魔，吸血排除NPC piaoyun 2013-07-24
  Config.WriteBool('Setup', 'HongMoSuiteWithPower', g_Config.boHongMoSuiteWithPower);

  // 禁止记忆传送安全区人物 piaoyun 2013-07-24
  Config.WriteBool('Setup', 'GroupReCallNotInSafeZone', g_Config.boGroupReCallNotInSafeZone);
  // 新人和平攻击模式 piaoyun 2013-07-24
  Config.WriteBool('Setup', 'NewHumanAttatckMode_HAM_PEACE', g_Config.boNewHumanAttatckMode_HAM_PEACE);
  // 自动替换组长 piaoyun 2013-07-24
  Config.WriteBool('Setup', 'AutoGroupMaster', g_Config.boAutoGroupMaster);
  // 大刀不攻击人形怪、分身 piaoyun 2013-07-24
  Config.WriteBool('Setup', 'GuardNotAttackPlayMoster', g_Config.boGuardNotAttackPlayMoster);
  // 安全区域不掉装备 piaoyun 2013-07-25
  Config.WriteBool('Setup', 'WarNoDropUseItem', g_Config.boWarNoDropUseItem);
  // 吸血武器吸血倍率 piaoyun 2013-07-24
  Config.WriteInteger('Setup', 'HongMoSuiteRateChange', g_Config.nHongMoSuiteRateChange);

  Config.WriteBool('Setup', 'ShowMysteriousMan', g_Config.boShowMysteriousMan);
  // 脚本循环次数 piaoyun 2013-09-05
  Config.WriteInteger('Setup', 'LimitScriptGotoCount', g_Config.nLimitScriptGotoCount);

  Config.WriteBool('Setup', 'M2CacheRankData', g_Config.boM2CacheRankData);

  Config.WriteBool('Setup', 'GroupUseOldMode', g_Config.boGroupUseOldMode);

  if boDropOverLapItem <> g_Config.boDropOverLapItem then
  begin
    boDropOverLapItem := g_Config.boDropOverLapItem;
    UserEngine.SendServerConfig();
  end;
  uModValue();
end;

procedure TfrmFunctionConfig.CheckBoxDeleteItemDuraZeroClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDeleteItemDuraZero := CheckBoxDeleteItemDuraZero.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMasterRoyaltyTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMasterRoyaltyTime := EditMasterRoyaltyTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seFireHitPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFireHitPowerRate := seFireHitPowerRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.chkRecallManySlave2Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRecallManySlave2 := chkRecallManySlave2.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSkill52PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill52PowerRate := EditSkill52PowerRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.EditSkill52AttackRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill52AttackRange := EditSkill52AttackRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMysteriousManNameChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmFunctionConfig.EditRecallDeputyHeroTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRecallDeputyHeroTime := EditRecallDeputyHeroTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.ButtonSaveWineClick(Sender: TObject);
var
  I: Integer;
  dwExp: LongWord;
  NeedExps: TMedicineNeedExps;
begin
  for I := 1 to GridMedicineExp.RowCount - 1 do
  begin
    dwExp := StrToIntDef(GridMedicineExp.Cells[1, I], 0);
    if (dwExp <= 0) then
    begin // 20080522
      Application.MessageBox(PChar('等级 ' + IntToStr(I) + ' 药力值设置错误！！！'), '错误信息', MB_OK + MB_ICONERROR);
      GridMedicineExp.Row := I;
      GridMedicineExp.SetFocus;
      Exit;
    end;
    NeedExps[I] := dwExp;
  end;
  g_Config.dwMedicineLevelNeedExps := NeedExps;
  for I := 1 to 1000 do
  begin
    ExpConfig.WriteString('MedicineExp', 'Level' + IntToStr(I), IntToStr(g_Config.dwMedicineLevelNeedExps[I]));
  end;

  ExpConfig.WriteInteger('Setup', 'IncAlcoholTime', g_Config.nIncAlcoholTime);
  ExpConfig.WriteInteger('Setup', 'DecDrinkTime', g_Config.nDecDrinkTime);
  ExpConfig.WriteInteger('Setup', 'MaxAlcoholValue', g_Config.nMaxAlcoholValue);
  ExpConfig.WriteInteger('Setup', 'IncAlcoholValue', g_Config.nIncAlcoholValue);
  ExpConfig.WriteInteger('Setup', 'DecMedicineValue', g_Config.nDecMedicineValue);
  ExpConfig.WriteInteger('Setup', 'DecMedicineTime', g_Config.nDecMedicineTime);
end;

procedure TfrmFunctionConfig.GridMedicineExpEnter(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmFunctionConfig.EditDecMedicineTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDecMedicineTime := EditDecMedicineTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditDecMedicineValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDecMedicineValue := EditDecMedicineValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditIncAlcoholTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nIncAlcoholTime := EditIncAlcoholTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditDecDrinkTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDecDrinkTime := EditDecDrinkTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMaxAlcoholValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxAlcoholValue := EditMaxAlcoholValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditIncAlcoholValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nIncAlcoholValue := EditIncAlcoholValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditUseContinuousMagicTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUseContinuousMagicTime := EditUseContinuousMagicTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditNGLevelValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGLevelValue := EditNGLevelValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditNGLevelExpValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGLevelExpValue := EditNGLevelExpValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditNGHeroLevelExpValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGHeroLevelExpValue := EditNGHeroLevelExpValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditNGIncTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGIncTime := EditNGIncTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditNGSkillPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGSkillPowerRate := EditNGSkillPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditNGDrinkIncExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGDrinkIncExp := EditNGDrinkIncExp.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditNGHitStruckDecNGChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGHitStruckDecNG := EditNGHitStruckDecNG.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditNGKillMonExpMultipleChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGKillMonExpMultiple := EditNGKillMonExpMultiple.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.edtNGLevelPowerAdd_LevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGLevelPowerAdd_Level := edtNGLevelPowerAdd_Level.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.edtNGLevelPowerAdd_PowerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGLevelPowerAdd_Power := edtNGLevelPowerAdd_Power.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.edtNGLevelPowerDec_LevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGLevelPowerDec_Level := edtNGLevelPowerDec_Level.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.edtNGLevelPowerDec_PowerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGLevelPowerDec_Power := edtNGLevelPowerDec_Power.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSkillContinuousPowerRate100Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < Length(g_Config.SkillContinuousPowerRates)) then
  begin
    g_Config.SkillContinuousPowerRates[TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.edtSkillContinuousCloseDefenseRates100Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < Length(g_Config.SkillContinuousCloseDefenseRates)) then
  begin
    g_Config.SkillContinuousCloseDefenseRates[TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.seSkill100BreakDefenceUpRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.Skill100BreakDefenceUpRate := seSkill100BreakDefenceUpRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate100_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[0, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate101_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[1, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate102_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[2, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate103_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[3, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate104_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[4, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate105_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[5, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate106_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[6, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate107_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[7, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate108_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[8, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate109_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[9, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate110_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[10, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitRate111_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitRates[11, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates100_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[0, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates101_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[1, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates102_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[2, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates103_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[3, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates104_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[4, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates105_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[5, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates106_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[6, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates107_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[7, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates108_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[8, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates109_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[9, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates110_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[10, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditSkillContinuousBlastHitPowerRates111_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.SkillContinuousBlastHitPowerRates[11, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditAcupoints0_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.AcupointLevels[0, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditAcupoints1_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.AcupointLevels[1, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditAcupoints2_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.AcupointLevels[2, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditAcupoints3_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.AcupointLevels[3, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.EditAcupoints4_0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < 5) then
  begin
    g_Config.AcupointLevels[4, TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.seSkill114HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill114HitWaitTime := seSkill114HitWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill114HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill114HitWaitTime := seHeroSkill114HitWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill114PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill114PowerRate := seSkill114PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill114AttackRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill114AttackRange := seSkill114AttackRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seDoMotaeboCDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDoMotaeboCD := seDoMotaeboCD.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.EditDamageItemDuraRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDamageItemDuraRate := EditDamageItemDuraRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.CheckBoxViewRangeCanMagicAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boViewRangeCanMagicAttack := CheckBoxViewRangeCanMagicAttack.Checked;
  EditMagicAttackRage.Enabled := not CheckBoxViewRangeCanMagicAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDogzGotoMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDogzGotoMaster := chkDogzGotoMaster.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxMonthSpiritGotoMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMonthSpiritGotoMaster := CheckBoxMonthSpiritGotoMaster.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxBigDogzGotoMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBigDogzGotoMaster := CheckBoxBigDogzGotoMaster.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxOfflineCloseMyShopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOfflineCloseMyShop := CheckBoxOfflineCloseMyShop.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkEnableDoubleFireHitSkillClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnableDoubleFireHitSkill := chkEnableDoubleFireHitSkill.Checked;

  chkEnableDoubleFireHitDelayClose.Enabled := chkEnableDoubleFireHitSkill.Checked;
  cbbDoubleFireHitDelayCloseType.Enabled := chkEnableDoubleFireHitSkill.Checked;
  seDoubleFireHitDelayCloseValue.Enabled := chkEnableDoubleFireHitSkill.Checked;

  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.chkEnableDoubleFireHitDelayCloseClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnableDoubleFireHitDelayClose := chkEnableDoubleFireHitDelayClose.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbDoubleFireHitDelayCloseTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDoubleFireHitDelayCloseType := cbbDoubleFireHitDelayCloseType.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.seDoubleFireHitDelayCloseValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDoubleFireHitDelayCloseValue := seDoubleFireHitDelayCloseValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxMasterRoyaltyDieClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMasterRoyaltyDie := CheckBoxMasterRoyaltyDie.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditAmyOunsulTimeRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAmyOunsulTimeRate := EditAmyOunsulTimeRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditAmyOunsulMaxTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAmyOunsulMaxTime := EditAmyOunsulMaxTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkillAmyounsulCDTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkillAmyounsulCDTime := seSkillAmyounsulCDTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkillGroupAmyounsulCDTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkillGroupAmyounsulCDTime := seSkillGroupAmyounsulCDTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkillGroupAmyounsulRedClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkillGroupAmyounsulRed := chkSkillGroupAmyounsulRed.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkillGroupAmyounsulGreenClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkillGroupAmyounsulGreen := chkSkillGroupAmyounsulGreen.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditElectrodelessBaseChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nElectrodelessBase := EditElectrodelessBase.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditElectrodelessWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nElectrodelessWaitTime := EditElectrodelessWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seElectrodelessPowerRateL0Chnge(Sender: TObject);
var
  Edit: TSpinEditEx;
begin
  Edit := Sender as TSpinEditEx;
  if not boOpened then
    Exit;
  g_Config.nElectrodelessPowerRates[Edit.Tag] := Edit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.EditSkill57PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill57PowerRate := EditSkill57PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxQigongPushSameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boQigongPushSameLevel := CheckBoxQigongPushSameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxFireWindPushSameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boFireWindPushSameLevel := CheckBoxFireWindPushSameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditMaxLuckMaxPowerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMaxLuckMaxPower := EditMaxLuckMaxPower.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.RadioGroupShopTypeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boUseHeroM2Shop := RadioGroupShopType.ItemIndex = 1;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxSlaveRelaxCanStruckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSlaveRelaxCanStruck := CheckBoxSlaveRelaxCanStruck.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill41CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill41CD := seSkill41CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxWeaponUpgradeFailNotDeleteClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boWeaponUpgradeFailNotDelete := CheckBoxWeaponUpgradeFailNotDelete.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill58WaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill58WaitTime := seSkill58WaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxDropOverLapItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDropOverLapItem := CheckBoxDropOverLapItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkShowDoMotaeboMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowDoMotaeboMsg := chkShowDoMotaeboMsg.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxDuraChangeLightClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDuraChangeLight := CheckBoxDuraChangeLight.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxShowYouPoisonedClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowYouPoisoned := CheckBoxShowYouPoisoned.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxPoisonWeaponCanMagicAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPoisonWeaponCanMagicAttack := CheckBoxPoisonWeaponCanMagicAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxPoisonWeaponCanHitAllTargetClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPoisonWeaponCanHitAllTarget := CheckBoxPoisonWeaponCanHitAllTarget.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.EditQueryBagItemsTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nQueryBagItemsTime := EditQueryBagItemsTime.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.chkInfinityStorageClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boInfinityStorage := chkInfinityStorage.Checked;
  edtInfinityStorageCount.Enabled := chkInfinityStorage.Checked;

  if chkInfinityStorage.Checked then
    // 仓库默认容量可存44件物品,扩展后可存< > 件物品
    lblInfinityStorageCount.Caption := '无限仓库可存放 ' + IntToStr(g_Config.nInfinityStorageCount) + ' 件物品'
  else
    // lblInfinityStorageCount.Caption := '默认容量为' + IntToStr(MAXSTOREITEM); // + '扩展后为：' + IntToStr(MAXSTOREITEM + g_Config.nInfinityStorageCount);
    lblInfinityStorageCount.Caption := '默认可存 0 件物品';

  ModValue();
end;

procedure TfrmFunctionConfig.edtInfinityStorageCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nInfinityStorageCount := edtInfinityStorageCount.Value;

  if chkInfinityStorage.Checked then
    // 仓库默认容量可存44件物品,扩展后可存< > 件物品
    lblInfinityStorageCount.Caption := '无限仓库可存放 ' + IntToStr(g_Config.nInfinityStorageCount) + ' 件物品'
  else
    // lblInfinityStorageCount.Caption := '默认容量为' + IntToStr(MAXSTOREITEM); // + '扩展后为：' + IntToStr(MAXSTOREITEM + g_Config.nInfinityStorageCount);
    lblInfinityStorageCount.Caption := '默认可存 0 件物品';

  ModValue();
end;

procedure TfrmFunctionConfig.seSkill202BaseCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill202BaseCount := seSkill202BaseCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill202LevelupCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill202LevelupCount := seSkill202LevelupCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill203PoisonRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill203PoisonRate := seSkill203PoisonRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.seSkill203BasicMbTimerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill203BasicMbTimer := seSkill203BasicMbTimer.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.seSkill203LevelupMbTimerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill203LevelupMbTimer := seSkill203LevelupMbTimer.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.chkSkill203MbAttackMonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill203MbAttackMon := chkSkill203MbAttackMon.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill203MbAttackHumanClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill203MbAttackHuman := chkSkill203MbAttackHuman.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill203MbAttackSlaveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill203MbAttackSlave := chkSkill203MbAttackSlave.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill203DamagearmorClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill203Damagearmor := chkSkill203Damagearmor.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill203DecHealthClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill203DecHealth := chkSkill203DecHealth.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill203MbFastParalysisClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill203MbFastParalysis := chkSkill203MbFastParalysis.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204MbAttackMonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204MbAttackMon := chkSkill204MbAttackMon.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204MbAttackHumanClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204MbAttackHuman := chkSkill204MbAttackHuman.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204MbAttackSlaveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204MbAttackSlave := chkSkill204MbAttackSlave.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204MbFastParalysisClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204MbFastParalysis := chkSkill204MbFastParalysis.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204RunHumClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204RunHum := chkSkill204RunHum.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204RunMonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204RunMon := chkSkill204RunMon.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204RunNpcClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204RunNpc := chkSkill204RunNpc.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204RunGuardClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204RunGuard := chkSkill204RunGuard.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204WarDisHumRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204WarDisHumRun := chkSkill204WarDisHumRun.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill204BasicPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill204BasicPowerRate := seSkill204BasicPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill204LevelupPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill204LevelupPowerRate := seSkill204LevelupPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill204LevelupMbTimerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill204LevelupMbTimer := seSkill204LevelupMbTimer.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill205ReduceMPClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill205ReduceMP := chkSkill205ReduceMP.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill204BasicMbTimerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill204BasicMbTimer := seSkill204BasicMbTimer.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill203BasicPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill203BasicPowerRate := seSkill203BasicPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill203LevelupPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill203LevelupPowerRate := seSkill203LevelupPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill206MbAttackMonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill206MbAttackMon := chkSkill206MbAttackMon.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill206MbAttackHumanClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill206MbAttackHuman := chkSkill206MbAttackHuman.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill206MbAttackSlaveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill206MbAttackSlave := chkSkill206MbAttackSlave.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill206MbFastParalysisClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill206MbFastParalysis := chkSkill206MbFastParalysis.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill206BasicPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill206BasicPowerRate := seSkill206BasicPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill206LevelupPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill206LevelupPowerRate := seSkill206LevelupPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill206BasicMbTimerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill206BasicMbTimer := seSkill206BasicMbTimer.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill206LevelupMbTimerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill206LevelupMbTimer := seSkill206LevelupMbTimer.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill202CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill202CD := seSkill202CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill202RateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill202Rate := seSkill202Rate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill202RateOnlyMonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill202RateOnlyMon := chkSkill202RateOnlyMon.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill205CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill205CD := seSkill205CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill206CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill206CD := seSkill206CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill203CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill203CD := seSkill203CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill204CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill204CD := seSkill204CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill41MbTimer0Change(Sender: TObject);
var
  Edit: TSpinEditEx;
begin
  if not boOpened then
    Exit;
  Edit := Sender as TSpinEditEx;
  g_Config.nSkill41MbTimers[Edit.Tag] := Edit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMonthSpiritAttackRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonthSpiritAttackRange := seMonthSpiritAttackRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seDeDingMagicBasicPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDeDingMagicBasicPowerRate := seDeDingMagicBasicPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seDeDingMagicAttackRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDeDingMagicAttackRange := seDeDingMagicAttackRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill71PullSlaveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill71PullSlave := chkSkill71PullSlave.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.sePosionDecHealthTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwPosionDecHealthTime := sePosionDecHealthTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.sePosionDamagarmorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPosionDamagarmor := sePosionDamagarmor.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkOpenSelfShopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOpenSelfShop := chkOpenSelfShop.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSafeZoneShopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSafeZoneShop := chkSafeZoneShop.Checked;
  if chkSafeZoneShop.Checked then
    chkMapShop.Checked := False;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMapShopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMapShop := chkMapShop.Checked;
  if chkMapShop.Checked then
    chkSafeZoneShop.Checked := False;
  ModValue();
end;

procedure TfrmFunctionConfig.seSellOffGoldTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellOffGoldTaxRate := seSellOffGoldTaxRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSellOffGameGoldTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellOffGameGoldTaxRate := seSellOffGameGoldTaxRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkLockChallengeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockChallenge := chkLockChallenge.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkLockSummonHeroClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockSummonHero := chkLockSummonHero.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkLockShopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockShop := chkLockShop.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.chkLockStallClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLockStall := chkLockStall.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill41MbRange0Change(Sender: TObject);
var
  Edit: TSpinEditEx;
begin
  if not boOpened then
    Exit;
  Edit := Sender as TSpinEditEx;
  g_Config.nSkill41MbRanges[Edit.Tag] := Edit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill205RageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill205Rage := seSkill205Rage.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill206RageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill206Rage := seSkill206Rage.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill203RageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill203Rage := seSkill203Rage.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill204RageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill204Rage := seSkill204Rage.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkGroupReCallNotInSafeZoneClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGroupReCallNotInSafeZone := chkGroupReCallNotInSafeZone.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkNewHumanAttatckMode_HAM_PEACEClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNewHumanAttatckMode_HAM_PEACE := chkNewHumanAttatckMode_HAM_PEACE.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkAutoGroupMasterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boAutoGroupMaster := chkAutoGroupMaster.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkGuardNotAttackPlayMosterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGuardNotAttackPlayMoster := chkGuardNotAttackPlayMoster.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHongMoSuiteRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHongMoSuiteRateChange := seHongMoSuiteRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkCloseFireHitSkillFailHintClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCloseFireHitSkillFailHint := chkCloseFireHitSkillFailHint.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seElecBlizzardPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nElecBlizzardPowerRate := seElecBlizzardPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seFireBoomRagePowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFireBoomRagePowerRate := seFireBoomRagePowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMakeFireDayPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMakeFireDayPowerRate := seMakeFireDayPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSnowWindPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSnowWindPowerRate := seSnowWindPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDoMotaeboPushSameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDoMotaeboPushSameLevel := chkDoMotaeboPushSameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill15PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill15PowerRate := seSkill15PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkWarNoDropUseItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boWarNoDropUseItem := chkWarNoDropUseItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill15TimeRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill15TimeRate := seSkill15TimeRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill205PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill205PowerRate := seSkill205PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWarrDCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWarr.DC := seBonusAbilofWarrDC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWarrMCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWarr.MC := seBonusAbilofWarrMC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWarrSCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWarr.SC := seBonusAbilofWarrSC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWarrACChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWarr.AC := seBonusAbilofWarrAC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWarrMACChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWarr.MAC := seBonusAbilofWarrMAC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWarrHPChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWarr.HP := seBonusAbilofWarrHP.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWarrMPChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWarr.MP := seBonusAbilofWarrMP.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWarrHitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWarr.Hit := seBonusAbilofWarrHit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWarrSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWarr.Speed := seBonusAbilofWarrSpeed.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWizardDCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWizard.DC := seBonusAbilofWizardDC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWizardMCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWizard.MC := seBonusAbilofWizardMC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWizardSCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWizard.SC := seBonusAbilofWizardSC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWizardACChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWizard.AC := seBonusAbilofWizardAC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWizardMACChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWizard.MAC := seBonusAbilofWizardMAC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWizardHPChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWizard.HP := seBonusAbilofWizardHP.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWizardMPChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWizard.MP := seBonusAbilofWizardMP.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWizardHitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWizard.Hit := seBonusAbilofWizardHit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofWizardSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofWizard.Speed := seBonusAbilofWizardSpeed.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofTaosDCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofTaos.DC := seBonusAbilofTaosDC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofTaosMCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofTaos.MC := seBonusAbilofTaosMC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofTaosSCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofTaos.SC := seBonusAbilofTaosSC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofTaosACChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofTaos.AC := seBonusAbilofTaosAC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofTaosMACChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofTaos.MAC := seBonusAbilofTaosMAC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofTaosHPChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofTaos.HP := seBonusAbilofTaosHP.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofTaosMPChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofTaos.MP := seBonusAbilofTaosMP.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofTaosHitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofTaos.Hit := seBonusAbilofTaosHit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seBonusAbilofTaosSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.BonusAbilofTaos.Speed := seBonusAbilofTaosSpeed.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.btnBonusAbilofSaveClick(Sender: TObject);
begin
  { 属性消耗点控制 chongchong 2013-07-26 }
  Config.WriteInteger('Setup', 'BonusAbilofWarrDC', g_Config.BonusAbilofWarr.DC);
  Config.WriteInteger('Setup', 'BonusAbilofWarrMC', g_Config.BonusAbilofWarr.MC);
  Config.WriteInteger('Setup', 'BonusAbilofWarrSC', g_Config.BonusAbilofWarr.SC);
  Config.WriteInteger('Setup', 'BonusAbilofWarrAC', g_Config.BonusAbilofWarr.AC);
  Config.WriteInteger('Setup', 'BonusAbilofWarrMAC', g_Config.BonusAbilofWarr.MAC);
  Config.WriteInteger('Setup', 'BonusAbilofWarrHP', g_Config.BonusAbilofWarr.HP);
  Config.WriteInteger('Setup', 'BonusAbilofWarrMP', g_Config.BonusAbilofWarr.MP);
  Config.WriteInteger('Setup', 'BonusAbilofWarrHit', g_Config.BonusAbilofWarr.Hit);
  Config.WriteInteger('Setup', 'BonusAbilofWarrSpeed', g_Config.BonusAbilofWarr.Speed);
  Config.WriteInteger('Setup', 'BonusAbilofWizardDC', g_Config.BonusAbilofWizard.DC);
  Config.WriteInteger('Setup', 'BonusAbilofWizardMC', g_Config.BonusAbilofWizard.MC);
  Config.WriteInteger('Setup', 'BonusAbilofWizardSC', g_Config.BonusAbilofWizard.SC);
  Config.WriteInteger('Setup', 'BonusAbilofWizardAC', g_Config.BonusAbilofWizard.AC);
  Config.WriteInteger('Setup', 'BonusAbilofWizardMAC', g_Config.BonusAbilofWizard.MAC);
  Config.WriteInteger('Setup', 'BonusAbilofWizardHP', g_Config.BonusAbilofWizard.HP);
  Config.WriteInteger('Setup', 'BonusAbilofWizardMP', g_Config.BonusAbilofWizard.MP);
  Config.WriteInteger('Setup', 'BonusAbilofWizardHit', g_Config.BonusAbilofWizard.Hit);
  Config.WriteInteger('Setup', 'BonusAbilofWizardSpeed', g_Config.BonusAbilofWizard.Speed);
  Config.WriteInteger('Setup', 'BonusAbilofTaosDC', g_Config.BonusAbilofTaos.DC);
  Config.WriteInteger('Setup', 'BonusAbilofTaosMC', g_Config.BonusAbilofTaos.MC);
  Config.WriteInteger('Setup', 'BonusAbilofTaosSC', g_Config.BonusAbilofTaos.SC);
  Config.WriteInteger('Setup', 'BonusAbilofTaosAC', g_Config.BonusAbilofTaos.AC);
  Config.WriteInteger('Setup', 'BonusAbilofTaosMAC', g_Config.BonusAbilofTaos.MAC);
  Config.WriteInteger('Setup', 'BonusAbilofTaosHP', g_Config.BonusAbilofTaos.HP);
  Config.WriteInteger('Setup', 'BonusAbilofTaosMP', g_Config.BonusAbilofTaos.MP);
  Config.WriteInteger('Setup', 'BonusAbilofTaosHit', g_Config.BonusAbilofTaos.Hit);
  Config.WriteInteger('Setup', 'BonusAbilofTaosSpeed', g_Config.BonusAbilofTaos.Speed);

  uModValue();
end;

procedure TfrmFunctionConfig.chkCloseSuperShiledHintClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCloseSuperShiledHint := chkCloseSuperShiledHint.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seElectrodelessTimeL0Change(Sender: TObject);
var
  Edit: TSpinEditEx;
begin
  Edit := Sender as TSpinEditEx;
  if not boOpened then
    Exit;
  g_Config.dwElectrodelessTimes[Edit.Tag] := Edit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel0Change(Sender: TObject);
begin
  seMagicNewLevelPower0.Value := g_Config.NewLevelMagicPowerRatesSpecific[0][cbbMagicNewLevel0.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower0Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel0.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[0][cbbMagicNewLevel0.ItemIndex] <> seMagicNewLevelPower0.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[0][cbbMagicNewLevel0.ItemIndex] := seMagicNewLevelPower0.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel1Change(Sender: TObject);
begin
  seMagicNewLevelPower1.Value := g_Config.NewLevelMagicPowerRatesSpecific[1][cbbMagicNewLevel1.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower1Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel1.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[1][cbbMagicNewLevel1.ItemIndex] <> seMagicNewLevelPower1.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[1][cbbMagicNewLevel1.ItemIndex] := seMagicNewLevelPower1.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel2Change(Sender: TObject);
begin
  seMagicNewLevelPower2.Value := g_Config.NewLevelMagicPowerRatesSpecific[2][cbbMagicNewLevel2.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower2Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel2.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[2][cbbMagicNewLevel2.ItemIndex] <> seMagicNewLevelPower2.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[2][cbbMagicNewLevel2.ItemIndex] := seMagicNewLevelPower2.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel3Change(Sender: TObject);
begin
  seMagicNewLevelPower3.Value := g_Config.NewLevelMagicPowerRatesSpecific[3][cbbMagicNewLevel3.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower3Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel3.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[3][cbbMagicNewLevel3.ItemIndex] <> seMagicNewLevelPower3.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[3][cbbMagicNewLevel3.ItemIndex] := seMagicNewLevelPower3.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel4Change(Sender: TObject);
begin
  seMagicNewLevelPower4.Value := g_Config.NewLevelMagicPowerRatesSpecific[4][cbbMagicNewLevel4.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower4Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel4.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[4][cbbMagicNewLevel4.ItemIndex] <> seMagicNewLevelPower4.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[4][cbbMagicNewLevel4.ItemIndex] := seMagicNewLevelPower4.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel5Change(Sender: TObject);
begin
  seMagicNewLevelPower5.Value := g_Config.NewLevelMagicPowerRatesSpecific[5][cbbMagicNewLevel5.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower5Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel5.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[5][cbbMagicNewLevel5.ItemIndex] <> seMagicNewLevelPower5.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[5][cbbMagicNewLevel5.ItemIndex] := seMagicNewLevelPower5.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel20Change(Sender: TObject);
begin
  seMagicNewLevelPower20.Value := g_Config.NewLevelMagicPowerRatesSpecific[20][cbbMagicNewLevel20.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower20Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel20.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[20][cbbMagicNewLevel20.ItemIndex] <> seMagicNewLevelPower20.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[20][cbbMagicNewLevel20.ItemIndex] := seMagicNewLevelPower20.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel21Change(Sender: TObject);
begin
  seMagicNewLevelPower21.Value := g_Config.NewLevelMagicPowerRatesSpecific[21][cbbMagicNewLevel21.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower21Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel21.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[21][cbbMagicNewLevel21.ItemIndex] <> seMagicNewLevelPower21.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[21][cbbMagicNewLevel21.ItemIndex] := seMagicNewLevelPower21.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel22Change(Sender: TObject);
begin
  seMagicNewLevelPower22.Value := g_Config.NewLevelMagicPowerRatesSpecific[22][cbbMagicNewLevel22.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower22Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel22.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[22][cbbMagicNewLevel22.ItemIndex] <> seMagicNewLevelPower22.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[22][cbbMagicNewLevel22.ItemIndex] := seMagicNewLevelPower22.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel23Change(Sender: TObject);
begin
  seMagicNewLevelPower23.Value := g_Config.NewLevelMagicPowerRatesSpecific[23][cbbMagicNewLevel23.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower23Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel23.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[23][cbbMagicNewLevel23.ItemIndex] <> seMagicNewLevelPower23.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[23][cbbMagicNewLevel23.ItemIndex] := seMagicNewLevelPower23.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel24Change(Sender: TObject);
begin
  seMagicNewLevelPower24.Value := g_Config.NewLevelMagicPowerRatesSpecific[24][cbbMagicNewLevel24.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower24Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel24.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[24][cbbMagicNewLevel24.ItemIndex] <> seMagicNewLevelPower24.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[24][cbbMagicNewLevel24.ItemIndex] := seMagicNewLevelPower24.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel40Change(Sender: TObject);
begin
  seMagicNewLevelPower40.Value := g_Config.NewLevelMagicPowerRatesSpecific[40][cbbMagicNewLevel40.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower40Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel40.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[40][cbbMagicNewLevel40.ItemIndex] <> seMagicNewLevelPower40.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[40][cbbMagicNewLevel40.ItemIndex] := seMagicNewLevelPower40.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel41Change(Sender: TObject);
begin
  seMagicNewLevelPower41.Value := g_Config.NewLevelMagicPowerRatesSpecific[41][cbbMagicNewLevel41.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower41Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel41.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[41][cbbMagicNewLevel41.ItemIndex] <> seMagicNewLevelPower41.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[41][cbbMagicNewLevel41.ItemIndex] := seMagicNewLevelPower41.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel42Change(Sender: TObject);
begin
  seMagicNewLevelPower42.Value := g_Config.NewLevelMagicPowerRatesSpecific[42][cbbMagicNewLevel42.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower42Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel42.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[42][cbbMagicNewLevel42.ItemIndex] <> seMagicNewLevelPower42.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[42][cbbMagicNewLevel42.ItemIndex] := seMagicNewLevelPower42.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel43Change(Sender: TObject);
begin
  seMagicNewLevelPower43.Value := g_Config.NewLevelMagicPowerRatesSpecific[43][cbbMagicNewLevel43.ItemIndex];
  seNewLevelMagic43LSFRate.Value := g_Config.NewLevelMagic43LSFRate[cbbMagicNewLevel43.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower43Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel43.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[43][cbbMagicNewLevel43.ItemIndex] <> seMagicNewLevelPower43.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[43][cbbMagicNewLevel43.ItemIndex] := seMagicNewLevelPower43.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel44Change(Sender: TObject);
begin
  seMagicNewLevelPower44.Value := g_Config.NewLevelMagicPowerRatesSpecific[44][cbbMagicNewLevel44.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower44Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel44.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[44][cbbMagicNewLevel44.ItemIndex] <> seMagicNewLevelPower44.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[44][cbbMagicNewLevel44.ItemIndex] := seMagicNewLevelPower44.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel45Change(Sender: TObject);
begin
  seMagicNewLevelPower45.Value := g_Config.NewLevelMagicPowerRatesSpecific[45][cbbMagicNewLevel45.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower45Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel45.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[45][cbbMagicNewLevel45.ItemIndex] <> seMagicNewLevelPower45.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[45][cbbMagicNewLevel45.ItemIndex] := seMagicNewLevelPower45.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.seClearHeroGhostTickChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nClearHeroGhostTick := seClearHeroGhostTick.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroCallBBClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroCanCallBB := chkHeroCallBB.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroCallBBCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroCallBBCount := seHeroCallBBCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroCanUseMooteboClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroCanUseMootebo := chkHeroCanUseMootebo.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroJointAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroJointAttack := chkHeroJointAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroDieExpRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroDieExpRate := seHeroDieExpRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.seHeroMasterStartLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroMasterStartLevel := seHeroMasterStartLevel.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.seHeroSlaveStartLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroSlaveStartLevel := seHeroSlaveStartLevel.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.chkHeroDisableSafeZoneProtectClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroDisableSafeZoneProtect := chkHeroDisableSafeZoneProtect.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroNoMoveOnSleepClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroNoMoveOnSleep := chkHeroNoMoveOnSleep.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHero700HPUseBaseAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHero700HPUseBaseAttack := chkHero700HPUseBaseAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroWarrHPMPRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWarrHPMPRate := seHeroWarrHPMPRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroWizardHPMPRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWizardHPMPRate := seHeroWizardHPMPRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroTaosHPMPRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroTaosHPMPRate := seHeroTaosHPMPRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkCreditPointWithLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCreditPointWithLevel := chkCreditPointWithLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroFealtyCallAddChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroFealtyCallAdd := seHeroFealtyCallAdd.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroFealtyCallBackDelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroFealtyCallBackDel := seHeroFealtyCallBackDel.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroFealtyExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroFealtyCallBackDel := seHeroFealtyCallBackDel.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroFealtyDeathDelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroFealtyDeathDel := seHeroFealtyDeathDel.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroCalcWeaponSpeedClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroCalcWeaponSpeed := chkHeroCalcWeaponSpeed.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroFealtyExpAddChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroFealtyExpAdd := seHeroFealtyExpAdd.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroTaosAutoChangePoisonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroTaosAutoChangePoison := chkHeroTaosAutoChangePoison.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.GridLevelExpSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroGotoLV4Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroGotoLV4 := seHeroGotoLV4.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroPowerLV4Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroPowerLV4 := seHeroPowerLV4.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroProtectFlyRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroProtectFlyRange := seHeroProtectFlyRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.ComboBoxLevelExpClick(Sender: TObject);
var
  I: Integer;
  LevelExpScheme: TLevelExpScheme;
  dwOneLevelExp: LongWord;
  dwExp: LongWord;
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
  case LevelExpScheme of //
    s_OldLevelExp:
      g_Config.dwHeroNeedExps := g_dwOldHeroNeedExps;
    s_StdLevelExp:
      begin
        g_Config.dwHeroNeedExps := g_dwOldHeroNeedExps;
        dwOneLevelExp := 4000000000 div (High(g_Config.dwHeroNeedExps) div 2);
        for I := 1 to MAXCHANGELEVEL do
        begin
          if (26 + I) > MAXCHANGELEVEL then
            Break;
          dwExp := dwOneLevelExp * LongWord(I);
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[26 + I] := dwExp;
        end;
      end;
    s_2Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 2;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_5Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 5;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_8Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 8;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_10Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 10;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_20Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 20;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_30Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 30;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_40Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 40;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_50Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 50;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_60Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 60;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_70Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 70;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_80Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 80;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_90Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 90;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_100Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 100;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_150Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 150;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_200Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 200;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_250Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 250;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
    s_300Mult:
      begin
        for I := 1 to MAXCHANGELEVEL do
        begin
          dwExp := g_Config.dwHeroNeedExps[I] div 300;
          if dwExp = 0 then
            dwExp := 1;
          g_Config.dwHeroNeedExps[I] := dwExp;
        end;
      end;
  end;
  for I := 1 to GridLevelExp.RowCount - 1 do
  begin
    GridLevelExp.Cells[1, I] := IntToStr(g_Config.dwHeroNeedExps[I]);
  end;
  ModValue();
end;

procedure TfrmFunctionConfig.chkShowMsgMagicRangeExceedClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowMsgMagicRangeExceed := chkShowMsgMagicRangeExceed.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkShowMysteriousManClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowMysteriousMan := chkShowMysteriousMan.Checked;
  ModValue();
  UserEngine.RefShowName();
end;

procedure TfrmFunctionConfig.chkShopStallCanNotAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShopStallCanNotAttack := chkShopStallCanNotAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroFollowMasterWithDiffScreenClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroFollowMasterWithDiffScreen := chkHeroFollowMasterWithDiffScreen.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroHighLevelChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroHighLevel := seHeroHighLevel.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroHighLevelGetExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroHighLevelGetExp := seHeroHighLevelGetExp.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroJoinAttackFlyRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroJoinAttackFlyRange := seHeroJoinAttackFlyRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroLevel1000FixedExpChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroLevel1000FixedExp := seHeroLevel1000FixedExp.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroDisableStruckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroDisableStruck := chkHeroDisableStruck.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroDisableSelfStruckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroDisableSelfStruck := chkHeroDisableSelfStruck.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seNewLevelMagicPowerRatesAfter9Change(Sender: TObject);
begin
  g_Config.NewLevelMagicPowerRatesAfter9 := seNewLevelMagicPowerRatesAfter9.Value;
end;

procedure TfrmFunctionConfig.seOrdinarySkill31RateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nOrdinarySkill31Rate := seOrdinarySkill31Rate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill31RateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill31Rates[cbbSkill31Level.ItemIndex] := seSkill31Rate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkElectrodelessTimeSetClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boElectrodelessTimeSet := chkElectrodelessTimeSet.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seCopySelfNameColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btCopySelfNameColor := seCopySelfNameColor.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.edtCopySelfSuffixChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sCopySelfSuffix := edtCopySelfSuffix.Text;
  ModValue();
end;

procedure TfrmFunctionConfig.chkShowCopySelfSuffixClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowCopySelfSuffix := chkShowCopySelfSuffix.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkKillHeroWeaponUnlockClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillHeroWeaponUnlock := chkKillHeroWeaponUnlock.Checked;
  seKillHeroWeaponUnlockRate.Enabled := chkKillHeroWeaponUnlock.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seKillHeroWeaponUnlockRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwKillHeroWeaponUnlockRate := seKillHeroWeaponUnlockRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHumanGetAllExpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHumanGetAllExp := chkHumanGetAllExp.Checked;
  if chkHumanGetAllExp.Checked then
  begin
    chkHeroGetAllExp.Checked := False;
    g_Config.boHeroGetAllExp := False;
  end;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill42RangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill42Range := seSkill42Range.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill72RateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill72Rate := seSkill72Rate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill114AttackUseNGClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill114AttackUseNG := chkSkill114AttackUseNG.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkContinuousAttackUseNGClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boContinuousAttackUseNG := chkContinuousAttackUseNG.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seRevivalTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwRevivalTime := seRevivalTime.Value * 1000;
  ModValue();
end;

procedure TfrmFunctionConfig.chkRevivalTouchClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRevivalTouch := chkRevivalTouch.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.btnOther3Click(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'RevivalTime', g_Config.dwRevivalTime);
  Config.WriteBool('Setup', 'RevivalTouch', g_Config.boRevivalTouch);
  Config.WriteBool('Setup', 'SaveRevivalTime', g_Config.boSaveRevivalTime);
  Config.WriteBool('Setup', 'FBExitCreaterOffline', g_Config.boFBExitCreaterOffline);
  Config.WriteBool('Setup', 'FBDisableDelay30s', g_Config.boFBDisableDelay30s);

  Config.WriteBool('Setup', 'JewelryCalcBasicAbilitys', g_Config.boJewelryCalcBasicAbilitys);
  Config.WriteBool('Setup', 'JewelryCalcGroupAbilitys', g_Config.boJewelryCalcGroupAbilitys);
  Config.WriteBool('Setup', 'JewelryDecDura', g_Config.boJewelryDecDura);
  Config.WriteString('Setup', 'JewelryBoxHint', g_Config.sJewelryBoxHint);

  Config.WriteBool('Setup', 'OpenNewGuild', g_Config.boOpenNewGuildTemp);
  Config.WriteBool('Setup', 'NoShowNewGuildHumanCount', g_Config.boNoShowNewGuildHumanCount);

  Config.WriteBool('Setup', 'CloseNPCNoItemMsg', g_Config.boCloseNPCNoItemMsg);

  Config.WriteBool('Setup', 'ChangeUseItemNameByPlayName', g_Config.boChangeUseItemNameByPlayName);
  Config.WriteString('Setup', 'ChangeUseItemName', g_Config.sChangeUseItemName);

  Config.WriteInteger('Setup', 'StarBaseNum', g_Config.nStarBaseNum);
  Config.WriteInteger('Setup', 'StarLineMaxCount', g_Config.nStarLineMaxCount);
  Config.WriteBool('Setup', 'DisableDuFuTakeArmRingL', g_Config.boDisableDuFuTakeArmRingL);

  Config.WriteBool('Setup', 'RecordBeadExp', g_Config.boRecordBeadExp);

  Config.WriteBool('Setup', 'DisableMoveParalysisHuman', g_Config.boDisableMoveParalysisHuman);

  UserEngine.SendServerConfig;
  uModValue();
end;

procedure TfrmFunctionConfig.seLimitScriptGotoCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nLimitScriptGotoCount := seLimitScriptGotoCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkFBExitCreaterOfflineClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boFBExitCreaterOffline := chkFBExitCreaterOffline.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkFBDisableDelay30sClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boFBDisableDelay30s := chkFBDisableDelay30s.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkShowSuperShiledSoundClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowSuperShiledSound := chkShowSuperShiledSound.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkShowSuperShiledEffect2Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowSuperShiledEffect2 := chkShowSuperShiledEffect2.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkShowSuperShiledSound2Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowSuperShiledSound2 := chkShowSuperShiledSound2.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroAutoSuperShiledClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroAutoSuperShiled := chkHeroAutoSuperShiled.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMagicNotHinderClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMagicNotHinder := chkMagicNotHinder.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMagicDefinitionClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMagicDefinition := chkMagicDefinition.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkShopHeadPicClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShopHeadPic := chkShopHeadPic.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seACAttackSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroAttackRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  // g_Config.dwHeroAttackRange := seHeroAttackRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSKILL209CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill209CD := seSKILL209CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill209RageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill209Rage := seSkill209Rage.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill209PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill209PowerRate := seSkill209PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSKILL210CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill210CD := seSKILL210CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill210RageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill210Rage := seSkill210Rage.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill210PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill210PowerRate := seSkill210PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSKILL208CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill208CD := seSKILL208CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill208RageChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill208Rage := seSkill208Rage.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill208PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill208PowerRate := seSkill208PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204SameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204SameLevel := chkSkill204SameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill206SameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill206SameLevel := chkSkill206SameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill206FrozenClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill206Frozen := chkSkill206Frozen.Checked;
  seSkill206FrozenRate.Enabled := g_Config.boSkill206Frozen;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill206FrozenRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill206FrozenRate := seSkill206FrozenRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkJewelryCalcBasicAbilitysClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boJewelryCalcBasicAbilitys := chkJewelryCalcBasicAbilitys.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seAngryAgainValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwAngryAgainValue := seAngryAgainValue.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.chkSKILL208HeroDuanJinClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSKILL208HeroDuanJin := chkSKILL208HeroDuanJin.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSKILL208PlayMosterDuanJinClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSKILL208PlayMosterDuanJin := chkSKILL208PlayMosterDuanJin.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seRecallCopySelfWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwRecallCopySelfWaitTime := seRecallCopySelfWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seRecallDogzWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwRecallDogzWaitTime := seRecallDogzWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seRecallBoneFammWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwRecallBoneFammWaitTime := seRecallBoneFammWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seRecallMonthSpiritWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwRecallMonthSpiritWaitTime := seRecallMonthSpiritWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroTaoUsePoisonMinHPChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroTaoUsePoisonMinHP := seHeroTaoUsePoisonMinHP.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbSkillContinuousPowerRate111Change(Sender: TObject);
begin
  seToxicSmokeTime.Value := g_Config.dwToxicSmokeTimes[cbbSkillContinuousPowerRate111.ItemIndex];
end;

procedure TfrmFunctionConfig.seToxicSmokeTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwToxicSmokeTimes[cbbSkillContinuousPowerRate111.ItemIndex] := seToxicSmokeTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkToxicSmokeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boToxicSmoke := chkToxicSmoke.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seContinuousProtectChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btContinuousProtect := seContinuousProtect.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seContinuousProtectRandomChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btContinuousProtectRandom := seContinuousProtectRandom.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seWarrContinuousStatusLockChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < Length(g_Config.SkillContinuousPowerRates)) then
  begin
    g_Config.btWarrContinuousStatusLocks[TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.chkSlaveNotAttackHumanClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSlaveNotAttackHuman := chkSlaveNotAttackHuman.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSlaveNotAttackHeroClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSlaveNotAttackHero := chkSlaveNotAttackHero.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbMagicNewLevel25Change(Sender: TObject);
begin
  seMagicNewLevelPower25.Value := g_Config.NewLevelMagicPowerRatesSpecific[25][cbbMagicNewLevel25.ItemIndex];
end;

procedure TfrmFunctionConfig.seMagicNewLevelPower25Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel25.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[25][cbbMagicNewLevel25.ItemIndex] <> seMagicNewLevelPower25.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[25][cbbMagicNewLevel25.ItemIndex] := seMagicNewLevelPower25.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.seSnowwindWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSnowwindWaitTime := seSnowwindWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seJSOfGameGoldTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwJSOfGameGoldTaxRate := seJSOfGameGoldTaxRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkJewelryCalcGroupAbilitysClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boJewelryCalcGroupAbilitys := chkJewelryCalcGroupAbilitys.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSaveRevivalTimeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSaveRevivalTime := chkSaveRevivalTime.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seNewLevelMagic43LSFRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbMagicNewLevel43.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagic43LSFRate[cbbMagicNewLevel43.ItemIndex] <> seNewLevelMagic43LSFRate.Value then
    begin
      g_Config.NewLevelMagic43LSFRate[cbbMagicNewLevel43.ItemIndex] := seNewLevelMagic43LSFRate.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.seMakeFireDayTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMakeFireDayTime := seMakeFireDayTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill57TimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill57Time := seSkill57Time.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204RunObstacleClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204RunObstacle := chkSkill204RunObstacle.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDoMotaebo100PushSameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDoMotaebo100PushSameLevel := chkDoMotaebo100PushSameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seMasterCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMasterCount := seMasterCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkBBAttrPlusAddOnlyMagicClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBBAttrPlusAddOnlyMagic := chkBBAttrPlusAddOnlyMagic.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkBarbaricSeptumClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBarbaricSeptum := chkBarbaricSeptum.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkBBAttrPlusAddAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBBAttrPlusAddAttack := chkBBAttrPlusAddAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbBBAttrPlusAddAttackFormChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBBAttrPlusAddAttackForm := cbbBBAttrPlusAddAttackForm.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.seBBAttrPlusAddAttackRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBBAttrPlusAddAttackRate := seBBAttrPlusAddAttackRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkBBAttrPlusAddDefenceClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBBAttrPlusAddDefence := chkBBAttrPlusAddDefence.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkBBAttrPlusAddMagicDefenceClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBBAttrPlusAddMagicDefence := chkBBAttrPlusAddMagicDefence.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seBBAttrPlusAddDefenceRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBBAttrPlusAddDefenceRate := seBBAttrPlusAddDefenceRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkBBAttrPlusAddHPClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBBAttrPlusAddHP := chkBBAttrPlusAddHP.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seBBAttrPlusAddHPRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBBAttrPlusAddHPRate := seBBAttrPlusAddHPRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbBBAttrPlusNewLevelChange(Sender: TObject);
begin
  seBBAttrPlusNewLevelRate.Value := g_Config.nBBAttrPlusNewLevelRates[cbbBBAttrPlusNewLevel.ItemIndex];
end;

procedure TfrmFunctionConfig.seBBAttrPlusNewLevelRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbBBAttrPlusNewLevel.ItemIndex >= 0 then
  begin
    if g_Config.nBBAttrPlusNewLevelRates[cbbBBAttrPlusNewLevel.ItemIndex] <> seBBAttrPlusNewLevelRate.Value then
    begin
      g_Config.nBBAttrPlusNewLevelRates[cbbBBAttrPlusNewLevel.ItemIndex] := seBBAttrPlusNewLevelRate.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.chkShowRefreshBagMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boShowRefreshBagMsg := chkShowRefreshBagMsg.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkKillByMonstDropHeroJewelryBoxItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByMonstDropHeroJewelryBoxItem := chkKillByMonstDropHeroJewelryBoxItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkKillByHumanDropHeroJewelryBoxItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByHumanDropHeroJewelryBoxItem := chkKillByHumanDropHeroJewelryBoxItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkKillByMonstDropHeroGodBlessItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByMonstDropHeroGodBlessItem := chkKillByMonstDropHeroGodBlessItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkKillByHumanDropHeroGodBlessItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boKillByHumanDropHeroGodBlessItem := chkKillByHumanDropHeroGodBlessItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.scrlbrHeroJewelryBoxItemChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := scrlbrHeroJewelryBoxItem.Position;
  edtHeroJewelryBoxItem.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDropHeroJewelryBoxItemRate := nPostion;
  ModValue();

end;

procedure TfrmFunctionConfig.scrlbrHeroGodBlessItemChange(Sender: TObject);
var
  nPostion: Integer;
begin
  nPostion := scrlbrHeroGodBlessItem.Position;
  edtHeroGodBlessItem.Text := IntToStr(nPostion);
  if not boOpened then
    Exit;
  g_Config.nDropHeroGodBlessItemRate := nPostion;
  ModValue();
end;

procedure TfrmFunctionConfig.seSellOffGameDiamondTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellOffGameDiamondTaxRate := seSellOffGameDiamondTaxRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSellOffGameGirdTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellOffGameGirdTaxRate := seSellOffGameGirdTaxRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSellOffGamePointTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellOffGamePointTaxRate := seSellOffGamePointTaxRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMyShopGoldClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMyShopGold := chkMyShopGold.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMyShopGameGoldClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMyShopGameGold := chkMyShopGameGold.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMyShopGameDiamondClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMyShopGameDiamond := chkMyShopGameDiamond.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMyShopGameGirdClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMyShopGameGird := chkMyShopGameGird.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMyShopGamePointClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMyShopGamePoint := chkMyShopGamePoint.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill43LDMBRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill43LDMBRate := seSkill43LDMBRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill43LDMBTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill43LDMBTime := seSkill43LDMBTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill43LDMBPowerAddChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill43LDMBPowerAdd := seSkill43LDMBPowerAdd.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill43LockParalyClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill43LockParaly := chkSkill43LockParaly.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill58PowerTwoAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill58PowerTwoAttack := chkSkill58PowerTwoAttack.Checked;
  ModValue()
end;

procedure TfrmFunctionConfig.seHeroLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_dwHeroLimit := seHeroLimit.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroRunTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroRunTime := seHeroRunTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroWarrAttackMoveRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWarrAttackMoveRate := seHeroWarrAttackMoveRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMySellShopItemTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMySellShopItemTime := seMySellShopItemTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMySellShopItemTimeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnabledMySellShopItemTime := chkMySellShopItemTime.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroTargetAgainNoMoveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroTargetAgainNoMove := chkHeroTargetAgainNoMove.Checked;
  ModValue();

  chkHeroTargetAgainNoMoveDF.Enabled := g_Config.boHeroTargetAgainNoMove;
end;

procedure TfrmFunctionConfig.chkHeroJointAttackFlyClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroJointAttackFly := chkHeroJointAttackFly.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroTargetAgainNoMoveDFClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroTargetAgainNoMoveDF := chkHeroTargetAgainNoMoveDF.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill205PowerTwoAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill205PowerTwoAttack := chkSkill205PowerTwoAttack.Checked;
  ModValue()
end;

procedure TfrmFunctionConfig.chkSkill31UseNewEffectClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill31UseNewEffect := chkSkill31UseNewEffect.Checked;
  ModValue();
  boSendServerConfig := True;
end;

procedure TfrmFunctionConfig.seSkill46PowerBaseChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill46PowerBase := seSkill46PowerBase.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill46SecRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill46SecRate := seSkill46SecRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill46TimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill46Time := seSkill46Time.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkOpenNewGuildClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOpenNewGuildTemp := chkOpenNewGuild.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbHeroNeedMagicItemChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroNeedMagicItem := cbbHeroNeedMagicItem.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMonNoAttackOffLinePlayerClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMonNoAttackOffLinePlayer := chkMonNoAttackOffLinePlayer.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkillJointAttackLevelRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkillJointAttackLevelRate := seSkillJointAttackLevelRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seContinuousAttackLevelRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nContinuousAttackLevelRate := seContinuousAttackLevelRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill69CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill69CD := seSkill69CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroWarrAttacSkillErgumRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWarrAttacSkillErgumRate := seHeroWarrAttacSkillErgumRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill69AddTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill69AddTime := seSkill69AddTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill69AddRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill69AddRange := seSkill69AddRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroWarrAttakNearChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWarrAttakNear := seHeroWarrAttakNear.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroWarrNearFireSwordChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWarrNearFireSword := seHeroWarrNearFireSword.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.CheckBoxItemNameClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boChangeUseItemNameByPlayName := CheckBoxItemName.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill69SameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill69SameLevel := chkSkill69SameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.edtJewelryBoxHintChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sJewelryBoxHint := edtJewelryBoxHint.Text;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroRecallCopySelfHPRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btHeroRecallCopySelfHPRate := seHeroRecallCopySelfHPRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMySellShowItemNamLenChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMySellShowItemNamLen := seMySellShowItemNamLen.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkCloseNPCNoItemMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCloseNPCNoItemMsg := chkCloseNPCNoItemMsg.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroLockFlyRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroLockFlyRange := seHeroLockFlyRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroLogonTimeMasterDieChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroLogonTimeMasterDie := seHeroLogonTimeMasterDie.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill15OfflineClearClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill15OfflineClear := chkSkill15OfflineClear.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbHeroWarriorDefaultSkillChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroWarriorDefaultSkill := Integer(cbbHeroWarriorDefaultSkill.Items.Objects[cbbHeroWarriorDefaultSkill.ItemIndex]);
  ModValue();
end;

procedure TfrmFunctionConfig.seMagicFailMsgFColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMagicFailMsgFColor := seMagicFailMsgFColor.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seMagicFailMsgBColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMagicFailMsgBColor := seMagicFailMsgBColor.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seMagicOKMsgFColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMagicOKMsgFColor := seMagicOKMsgFColor.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seMagicOKMsgBColorChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.btMagicOKMsgBColor := seMagicOKMsgBColor.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seMagicMsgXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagicMsgX := seMagicMsgX.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seMagicMsgYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMagicMsgY := seMagicMsgY.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMagicMsgXRightToLeftClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMagicMsgXRightToLeft := chkMagicMsgXRightToLeft.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMagicMsgAddChatBoardMsgClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMagicMsgAddChatBoardMsg := chkMagicMsgAddChatBoardMsg.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroStateDlgNoMoveClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroStateDlgNoMove := chkHeroStateDlgNoMove.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seStarBaseNumChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStarBaseNum := seStarBaseNum.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.chkJewelryDecDuraClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boJewelryDecDura := chkJewelryDecDura.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkRecordBeadExpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRecordBeadExp := chkRecordBeadExp.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSlaveLockTargetClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSlaveLockTarget := chkSlaveLockTarget.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill62NotMagBubbleDefenceClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill62NotMagBubbleDefence := chkSkill62NotMagBubbleDefence.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.lstMagicACClick(Sender: TObject);
var
  MagicACInfo: PMagicACInfo;
begin
  if lstMagicAC.ItemIndex >= 0 then
  begin
    MagicACInfo := PMagicACInfo(lstMagicAC.Items.Objects[lstMagicAC.ItemIndex]);
    chkMagicACEnabled.Checked := MagicACInfo.boEnabled;
    seMagicACHum.Value := MagicACInfo.btHum;
    seMagicACMon.Value := MagicACInfo.btMon;
    seMagicACHero.Value := MagicACInfo.btHero;

    seDefenceHum.Value := MagicACInfo.btDefenceHum;
    seDefenceMon.Value := MagicACInfo.btDefenceMon;
    seDefenceHero.Value := MagicACInfo.btDefenceHero;

    chkMagicACEnabled.Enabled := True;
    seMagicACHum.Enabled := True;
    seMagicACMon.Enabled := True;
    seMagicACHero.Enabled := True;
  end;
end;

procedure TfrmFunctionConfig.chkMagicACEnabledClick(Sender: TObject);
var
  MagicACInfo: PMagicACInfo;
begin
  if not boOpened then
    Exit;
  if lstMagicAC.ItemIndex >= 0 then
  begin
    MagicACInfo := PMagicACInfo(lstMagicAC.Items.Objects[lstMagicAC.ItemIndex]);
    MagicACInfo.boEnabled := chkMagicACEnabled.Checked;

    ModValue();
  end;
end;

procedure TfrmFunctionConfig.seMagicACHumChange(Sender: TObject);
var
  MagicACInfo: PMagicACInfo;
begin
  if not boOpened then
    Exit;
  if lstMagicAC.ItemIndex >= 0 then
  begin
    MagicACInfo := PMagicACInfo(lstMagicAC.Items.Objects[lstMagicAC.ItemIndex]);
    MagicACInfo.btHum := seMagicACHum.Value;

    ModValue();
  end;
end;

procedure TfrmFunctionConfig.seMagicACMonChange(Sender: TObject);
var
  MagicACInfo: PMagicACInfo;
begin
  if not boOpened then
    Exit;
  if lstMagicAC.ItemIndex >= 0 then
  begin
    MagicACInfo := PMagicACInfo(lstMagicAC.Items.Objects[lstMagicAC.ItemIndex]);
    MagicACInfo.btMon := seMagicACMon.Value;

    ModValue();
  end;
end;

procedure TfrmFunctionConfig.seMagicACHeroChange(Sender: TObject);
var
  MagicACInfo: PMagicACInfo;
begin
  if not boOpened then
    Exit;
  if lstMagicAC.ItemIndex >= 0 then
  begin
    MagicACInfo := PMagicACInfo(lstMagicAC.Items.Objects[lstMagicAC.ItemIndex]);
    MagicACInfo.btHero := seMagicACHero.Value;

    ModValue();
  end;
end;

procedure TfrmFunctionConfig.chkHongMoSuiteWithPowerClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHongMoSuiteWithPower := chkHongMoSuiteWithPower.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDisableDuFuTakeArmRingLClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableDuFuTakeArmRingL := chkDisableDuFuTakeArmRingL.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkPosionStopIncHealthClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boPosionStopIncHealth := chkPosionStopIncHealth.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkProhibitModifyPricesClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boProhibitModifyPrices := chkProhibitModifyPrices.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkEnabledPosionDecMACClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnabledPosionDecMAC := chkEnabledPosionDecMAC.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.sePosionDecMACRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nPosionDecMACRate := sePosionDecMACRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seNearAttackPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNearAttackPowerRate := seNearAttackPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkillYedoPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkillYedoPowerRate := seSkillYedoPowerRate.Value;
  ModValue()
end;

procedure TfrmFunctionConfig.chkDogzPlugSettingPriorityClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDogzPlugSettingPriority := chkDogzPlugSettingPriority.Checked;
  ModValue()
end;

procedure TfrmFunctionConfig.chkBonePlugSettingPriorityClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boBonePlugSettingPriority := chkBonePlugSettingPriority.Checked;
  ModValue()
end;

procedure TfrmFunctionConfig.chkSpiritualismLevelDiffClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSpiritualismLevelDiff := chkSpiritualismLevelDiff.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSpiritualismLevelDiffChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSpiritualismLevelDiff := seSpiritualismLevelDiff.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbSpiritualismMagicLevelChange(Sender: TObject);
begin
  if (cbbSpiritualismMagicLevel.ItemIndex >= Low(g_Config.nSpiritualismRates)) and (cbbSpiritualismMagicLevel.ItemIndex <= High(g_Config.nSpiritualismRates))
    then
  begin
    seSpiritualismRate.OnChange := nil;
    seSpiritualismRate.Value := g_Config.nSpiritualismRates[cbbSpiritualismMagicLevel.ItemIndex];
    seSpiritualismRate.OnChange := seSpiritualismRateChange;
  end;
end;

procedure TfrmFunctionConfig.seSpiritualismRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;

  if (cbbSpiritualismMagicLevel.ItemIndex >= Low(g_Config.nSpiritualismRates)) and (cbbSpiritualismMagicLevel.ItemIndex <= High(g_Config.nSpiritualismRates))
    then
    g_Config.nSpiritualismRates[cbbSpiritualismMagicLevel.ItemIndex] := seSpiritualismRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSpiritualismRoyaltyTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSpiritualismRoyaltyTime := seSpiritualismRoyaltyTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSpiritualismBBCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSpiritualismBBCount := seSpiritualismBBCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSlave9HPChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSlave9HP := seSlave9HP.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSlave9ACChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSlave9AC := seSlave9AC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSlave9MACChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSlave9MAC := seSlave9MAC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSlave9DCChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSlave9DC := seSlave9DC.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSlave9MoveSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSlave9MoveSpeed := seSlave9MoveSpeed.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSlave9HitSpeedChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSlave9HitSpeed := seSlave9HitSpeed.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.edtSetOffLineLoginMapNameChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSlaveKillHumanIncPKClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSlaveKillHumanIncPK := chkSlaveKillHumanIncPK.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSlaveLevelupUseNewAttrClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSlaveLevelupUseNewAttr := chkSlaveLevelupUseNewAttr.Checked;
  ModValue();

  chkSlaveLevelupUseNewAttr.Checked := g_Config.boSlaveLevelupUseNewAttr;
  seSlave9HP.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9AC.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9MAC.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9DC.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9MoveSpeed.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  seSlave9HitSpeed.Enabled := chkSlaveLevelupUseNewAttr.Checked;
  chkSlaveLevelupAddLowerAttr.Enabled := chkSlaveLevelupUseNewAttr.Checked;
end;

procedure TfrmFunctionConfig.chkHeroForcePeaceModeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroForcePeaceMode := chkHeroForcePeaceMode.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSlaveAlwaysShowNameClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSlaveAlwaysShowName := chkSlaveAlwaysShowName.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSlaveLevelupAddLowerAttrClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSlaveLevelupAddLowerAttr := chkSlaveLevelupAddLowerAttr.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroAttackHumPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroAttackHumPowerRate := seHeroAttackHumPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroAttackMonPowerRatehange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroAttackMonPowerRate := seHeroAttackMonPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkNoNeedFirDragonClick(Sender: TObject);
begin
  boSendServerConfig := True;
  if not boOpened then
    Exit;
  g_Config.boNoNeedFirDragon := chkNoNeedFirDragon.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroStatus0Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroStatus[0] := chkHeroStatus0.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroStatus1Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroStatus[1] := chkHeroStatus1.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroStatus2Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroStatus[2] := chkHeroStatus2.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroStatus3Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroStatus[3] := chkHeroStatus3.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seMonAttackHeroPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonAttackHeroPowerRate := seMonAttackHeroPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill60AttackHumPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill60AttackHumPowerRate := seSkill60AttackHumPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill62AttackHumPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill62AttackHumPowerRate := seSkill62AttackHumPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill61AttackHumPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill61AttackHumPowerRate := seSkill61AttackHumPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill65AttackHumPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill65AttackHumPowerRate := seSkill65AttackHumPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill64AttackHumPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill64AttackHumPowerRate := seSkill64AttackHumPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill63AttackHumPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill63AttackHumPowerRate := seSkill63AttackHumPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill204BasicMbRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill204BasicMbRate := seSkill204BasicMbRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHumanAttackHeroPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHumanAttackHeroPowerRate := seHumanAttackHeroPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.edtHeroSayPrefixChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sHeroSayPrefix := edtHeroSayPrefix.Text;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMasterRoyaltyFullHPClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMasterRoyaltyFullHP := chkMasterRoyaltyFullHP.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSlaveDisableStruckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSlaveDisableStruck := chkSlaveDisableStruck.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.btnAuctionClick(Sender: TObject);
var
  I: Integer;
begin
  Config.WriteInteger('Setup', 'AuctioningItemsCount', g_Config.nAuctioningItemsCount);
  Config.WriteInteger('Setup', 'AuctionCurrencyTypeEx', g_Config.nAuctionCurrencyTypeEx);

  Config.WriteInteger('Setup', 'AuctionGoldTaxRate', g_Config.dwAuctionGoldTaxRate);
  Config.WriteInteger('Setup', 'AuctionGameGoldTaxRate', g_Config.dwAuctionGameGoldTaxRate);
  Config.WriteInteger('Setup', 'AuctionGameDiamondTaxRate', g_Config.dwAuctionGameDiamondTaxRate);
  Config.WriteInteger('Setup', 'AuctionGameGirdTaxRate', g_Config.dwAuctionGameGirdTaxRate);
  Config.WriteInteger('Setup', 'AuctionGamePointTaxRate', g_Config.dwAuctionGamePointTaxRate);

  Config.WriteInteger('Setup', 'AuctionBroadcastCurrencyType', g_Config.nAuctionBroadcastCurrencyType);
  Config.WriteInteger('Setup', 'AuctionBroadcastPrice', g_Config.nAuctionBroadcastPrice);
  Config.WriteString('Setup', 'AuctionBroadcastText', g_Config.sAuctionBroadcastText);
  Config.WriteInteger('Setup', 'AuctionBroadcastShowTime', g_Config.nAuctionBroadcastShowTime);
  Config.WriteBool('Setup', 'OpenAuctionItemColors', g_Config.boOpenAuctionItemColors);
  for I := Low(g_Config.btAuctionItemColors) to High(g_Config.btAuctionItemColors) do
  begin
    Config.WriteInteger('Setup', 'AuctionItemColor' + IntToStr(I + 1), g_Config.btAuctionItemColors[I]);
  end;

  UserEngine.SendServerConfig;

  btnAuction.Enabled := False;
end;

procedure TfrmFunctionConfig.seAuctionGoldTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwAuctionGoldTaxRate := seAuctionGoldTaxRate.Value;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.cbbAuctionBroadcastCurrencyTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAuctionBroadcastCurrencyType := cbbAuctionBroadcastCurrencyType.ItemIndex;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.seAuctionBroadcastPriceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAuctionBroadcastPrice := seAuctionBroadcastPrice.Value;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.seAuctionItemColor1Change(Sender: TObject);
var
  Ctrl: TColorIndexEdit;
begin
  if not boOpened then
    Exit;
  Ctrl := Sender as TColorIndexEdit;
  if (Ctrl.Tag >= Low(g_Config.btAuctionItemColors)) and (Ctrl.Tag <= High(g_Config.btAuctionItemColors)) then
  begin
    g_Config.btAuctionItemColors[Ctrl.Tag] := Ctrl.Value;
    btnAuction.Enabled := True;
  end;
end;

procedure TfrmFunctionConfig.seAuctionBroadcastShowTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAuctionBroadcastShowTime := seAuctionBroadcastShowTime.Value;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.edtAuctionBroadcastTextChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sAuctionBroadcastText := edtAuctionBroadcastText.Text;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.chkOpenAuctionItemColorsClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOpenAuctionItemColors := chkOpenAuctionItemColors.Checked;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.chkMagTurnUndeadSameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMagTurnUndeadSameLevel := chkMagTurnUndeadSameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill25PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill25PowerRate := seSkill25PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbHPRockAddTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHPRockAddType := cbbHPRockAddType.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbMPRockAddTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMPRockAddType := cbbMPRockAddType.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbHMPRockAddTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHMPRockAddType := cbbHMPRockAddType.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbHPRockTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHPRockType := cbbHPRockType.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbMPRockTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMPRockType := cbbMPRockType.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbHMPRockTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHMPRockType := cbbHMPRockType.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHMPUse2TimesClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHMPUse2Times := chkHMPUse2Times.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill70CDChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill70CD := seSkill70CD.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkElectrodelessUseMaxSCClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boElectrodelessUseMaxSC := chkElectrodelessUseMaxSC.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkLuckUseNewAlgorismClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boLuckUseNewAlgorism := chkLuckUseNewAlgorism.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seCopySelfLevelUpAddExistTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCopySelfLevelUpAddExistTime := seCopySelfLevelUpAddExistTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill72LevelUpRateAddChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill72LevelUpRateAdd := seSkill72LevelUpRateAdd.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSuperShiledLevelUpAddValidTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSuperShiledLevelUpAddValidTime := seSuperShiledLevelUpAddValidTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSuperShiledLevelUpDecPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSuperShiledLevelUpDecPowerRate := seSuperShiledLevelUpDecPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seCloseSuperShiledLevelUpDecRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nCloseSuperShiledLevelUpDecRate := seCloseSuperShiledLevelUpDecRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seOpenSuperShiledLevelUpAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nOpenSuperShiledLevelUpAddRate := seOpenSuperShiledLevelUpAddRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill114LevelUpAddPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill114LevelUpAddPowerRate := seSkill114LevelUpAddPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDisHeroRunClick(Sender: TObject);
var
  boChecked: Boolean;
begin
  boChecked := not chkDisHeroRun.Checked;
  if boChecked then
  begin
    chkHeroRunHum.Checked := False;
    chkHeroRunHum.Enabled := False;

    chkHeroRunMon.Checked := False;
    chkHeroRunMon.Enabled := False;

    chkHeroRunNpc.Checked := False;
    chkHeroRunNpc.Enabled := False;

    chkHeroRunGuard.Checked := False;
    chkHeroRunGuard.Enabled := False;

    chkHeroSafeArea.Checked := False;
    chkHeroSafeArea.Enabled := False;

    // chkHeroWarDisHumRun.Checked := False;
    // chkHeroWarDisHumRun.Enabled := False;

    // chkHeroWarHreoRun.Checked := False;
    // chkHeroWarHreoRun.Enabled := False;

    chkHeroSafeAreaDisNpcRun.Checked := False;
    chkHeroSafeAreaDisNpcRun.Enabled := False;

    chkSafeAreaDisShopStallHeroRun.Checked := False;
    chkSafeAreaDisShopStallHeroRun.Enabled := False;

    chkSafeAreaDisOffLineHeroRun.Enabled := False;
    chkSafeAreaDisOffLineHeroRun.Checked := False;
  end
  else
  begin
    chkHeroRunHum.Enabled := True;
    chkHeroRunMon.Enabled := True;
    chkHeroRunNpc.Enabled := True;
    chkHeroRunGuard.Enabled := True;
    chkHeroSafeArea.Enabled := True;
    // chkHeroWarDisHumRun.Enabled := True;
    // chkHeroWarHreoRun.Checked := True;
    chkSafeAreaDisShopStallHeroRun.Enabled := True;
    chkSafeAreaDisOffLineHeroRun.Enabled := True;
    chkHeroSafeAreaDisNpcRun.Enabled := True;
  end;

  if not boOpened then
    Exit;
  g_Config.boDiableHeroRun := boChecked;

  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroRunHumClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroRunHum := chkHeroRunHum.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroRunMonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroRunMon := chkHeroRunMon.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroRunNpcClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroRunNpc := chkHeroRunNpc.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroRunGuardClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroRunGuard := chkHeroRunGuard.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroSafeAreaClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroSafeAreaLimited := chkHeroSafeArea.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroWarDisHumRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroWarDisHumRun := chkHeroWarDisHumRun.Checked;

  chkHeroWarHreoRun.Enabled := chkHeroWarDisHumRun.Checked;
  g_Config.boHeroWarHreoRun := chkHeroWarHreoRun.Enabled and chkHeroWarHreoRun.Checked;

  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroWarHreoRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroWarHreoRun := chkHeroWarHreoRun.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroSafeAreaDisNpcRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroSafeAreaDisNpcRun := chkHeroSafeAreaDisNpcRun.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSafeAreaDisShopStallHeroRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSafeAreaDisShopStallHeroRun := chkSafeAreaDisShopStallHeroRun.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSafeAreaDisOffLineHeroRunClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSafeAreaDisOffLineHeroRun := chkSafeAreaDisOffLineHeroRun.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seDoMotaebo100PushDistanceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDoMotaebo100PushDistance := seDoMotaebo100PushDistance.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDisableMoveParalysisHumanClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableMoveParalysisHuman := chkDisableMoveParalysisHuman.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seWarrContinuousStatusLockTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < Length(g_Config.SkillContinuousPowerRates)) then
  begin
    g_Config.nWarrContinuousStatusLockTimes[TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
  end;
end;

procedure TfrmFunctionConfig.chkHeroNoTargetRecallBBClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroNoTargetRecallBB := chkHeroNoTargetRecallBB.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroAvoidTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHeroAvoidTime := seHeroAvoidTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkM2CacheRankDataClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boM2CacheRankData := chkM2CacheRankData.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill37RangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill37Range := seSkill37Range.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill37RangeAddChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill37RangeAdd := seSkill37RangeAdd.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill204DisableStopItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill204DisableStopItem := chkSkill204DisableStopItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroDFAvoidTargetRightClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroDFAvoidTargetRight := chkHeroDFAvoidTargetRight.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill113PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill113PowerRate := seSkill113PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill113HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill113CD := seSkill113HitWaitTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill113HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill113CD := seHeroSkill113HitWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill115PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill115PowerRate := seSkill115PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill116PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill116PowerRate := seSkill116PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill117PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill117PowerRate := seSkill117PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill115HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill115CD := seSkill115HitWaitTime.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill115HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill115CD := seHeroSkill115HitWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill116HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill116CD := seSkill116HitWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill116HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill116CD := seHeroSkill116HitWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill117HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill117CD := seSkill117HitWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill117HitWaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill117CD := seHeroSkill117HitWaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill116RangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill116Range := seSkill116Range.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill117RangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill117Range := seSkill117Range.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill115LevelUpAddPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill115LevelUpAddPowerRate := seSkill115LevelUpAddPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill116LevelUpAddPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill116LevelUpAddPowerRate := seSkill116LevelUpAddPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill117LevelUpAddPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill117LevelUpAddPowerRate := seSkill117LevelUpAddPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill115UseNGClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill115UseNG := chkSkill115UseNG.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill115NGNoEnoughDecHPClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill115NGNoEnoughDecHP := chkSkill115NGNoEnoughDecHP.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill115NGNoEnoughDecHPValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill115NGNoEnoughDecHPValue := seSkill115NGNoEnoughDecHPValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbSkill115NGNoEnoughDecHPTypeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill115NGNoEnoughDecHPType := cbbSkill115NGNoEnoughDecHPType.ItemIndex;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkillLighteningPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkillLighteningPowerRate := seSkillLighteningPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkillGroupLighteningPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkillGroupLighteningPowerRate := seSkillGroupLighteningPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seAuctionGameGoldTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwAuctionGameGoldTaxRate := seAuctionGameGoldTaxRate.Value;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.seAuctionGameDiamondTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwAuctionGameDiamondTaxRate := seAuctionGameDiamondTaxRate.Value;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.seAuctionGameGirdTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwAuctionGameGirdTaxRate := seAuctionGameGirdTaxRate.Value;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.seAuctionGamePointTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwAuctionGamePointTaxRate := seAuctionGamePointTaxRate.Value;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.chkAuctionCurrencyType1Click(Sender: TObject);
var
  dwTemp: LongWord;
begin
  if not boOpened then
    Exit;
  dwTemp := 0;

  if chkAuctionCurrencyType1.Checked then
    dwTemp := dwTemp or 1;

  if chkAuctionCurrencyType2.Checked then
    dwTemp := dwTemp or 2;

  if chkAuctionCurrencyType3.Checked then
    dwTemp := dwTemp or 4;

  if chkAuctionCurrencyType4.Checked then
    dwTemp := dwTemp or 8;

  if chkAuctionCurrencyType5.Checked then
    dwTemp := dwTemp or 16;

  g_Config.nAuctionCurrencyTypeEx := dwTemp;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.chkCopyMonWarrorAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCopyMonWarrorAttack := chkCopyMonWarrorAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkCopyMon700HPUseBaseAttackClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCopyMon700HPUseBaseAttack := chkCopyMon700HPUseBaseAttack.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroHitCmpClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroHitCmp := chkHeroHitCmp.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seWarrCmpInvTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWarrCmpInvTime := seWarrCmpInvTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkMagicMsgYBottomToTopClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boMagicMsgYBottomToTop := chkMagicMsgYBottomToTop.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.chkHeroNotAvoidLastHinterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroNotAvoidLastHinter := chkHeroNotAvoidLastHinter.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSlaveNoLockHumanClick(Sender: TObject);
begin
  g_Config.boSlaveNoLockHuman := chkSlaveNoLockHuman.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHeroSkill58WaitTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHeroSkill58WaitTime := seHeroSkill58WaitTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seToxicSmokeDecHPRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nToxicSmokeDecHPRate := seToxicSmokeDecHPRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill113RateAddWithSkill63Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill113RateAddWithSkill63 := seSkill113RateAddWithSkill63.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill63UseSpeedPointClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill63UseSpeedPoint := chkSkill63UseSpeedPoint.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill31UseLEGEffectClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill31UseLEGEffect := chkSkill31UseLEGEffect.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkNoShowNewGuildHumanCountClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boNoShowNewGuildHumanCount := chkNoShowNewGuildHumanCount.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seHero700HPValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHero700HPValue := seHero700HPValue.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDisableSkill41MbSameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableSkill41MbSameLevel := chkDisableSkill41MbSameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill71DisableAttackSameLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill71DisableAttackSameLevel := chkSkill71DisableAttackSameLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill71DisableAttackFriendClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill71DisableAttackFriend := chkSkill71DisableAttackFriend.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill204DistanceChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill204Distance := seSkill204Distance.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seStarLineMaxCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nStarLineMaxCount := seStarLineMaxCount.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seAuctioningItemsCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAuctioningItemsCount := seAuctioningItemsCount.Value;
  btnAuction.Enabled := True;
end;

procedure TfrmFunctionConfig.seSkillFireCharmPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkillFireCharmPowerRate := seSkillFireCharmPowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill202PowerDecChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill202PowerDec := seSkill202PowerDec.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill202PowerMinChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill202PowerMin := seSkill202PowerMin.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill202PowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill202PowerRate := seSkill202PowerRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seDefenceHumChange(Sender: TObject);
var
  MagicACInfo: PMagicACInfo;
begin
  if not boOpened then
    Exit;
  if lstMagicAC.ItemIndex >= 0 then
  begin
    MagicACInfo := PMagicACInfo(lstMagicAC.Items.Objects[lstMagicAC.ItemIndex]);
    MagicACInfo.btDefenceHum := seDefenceHum.Value;

    ModValue();
  end;
end;

procedure TfrmFunctionConfig.seDefenceMonChange(Sender: TObject);
var
  MagicACInfo: PMagicACInfo;
begin
  if not boOpened then
    Exit;
  if lstMagicAC.ItemIndex >= 0 then
  begin
    MagicACInfo := PMagicACInfo(lstMagicAC.Items.Objects[lstMagicAC.ItemIndex]);
    MagicACInfo.btDefenceMon := seDefenceMon.Value;

    ModValue();
  end;
end;

procedure TfrmFunctionConfig.seDefenceHeroChange(Sender: TObject);
var
  MagicACInfo: PMagicACInfo;
begin
  if not boOpened then
    Exit;
  if lstMagicAC.ItemIndex >= 0 then
  begin
    MagicACInfo := PMagicACInfo(lstMagicAC.Items.Objects[lstMagicAC.ItemIndex]);
    MagicACInfo.btDefenceHero := seDefenceHero.Value;

    ModValue();
  end;
end;

procedure TfrmFunctionConfig.chkDisableMonsterAttackHeroClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableMonsterAttackHero := chkDisableMonsterAttackHero.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkDisableHeroAttackMonsterClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableHeroAttackMonster := chkDisableHeroAttackMonster.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill72DisableStopItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill72DisableStopItem := chkSkill72DisableStopItem.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seUseSkillCloseSuperShiled0_RateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.UseSkillCloseSuperShileds_Rate[TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seUseSkillCloseSuperShiled0_RateAddChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.UseSkillCloseSuperShileds_RateAdd[TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkillContinueOrderBlastRates1Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if (TSpinEditEx(Sender).Tag >= 0) and (TSpinEditEx(Sender).Tag < Length(g_Config.SkillContinueOrderBlastRates)) then
  begin
    g_Config.SkillContinueOrderBlastRates[TSpinEditEx(Sender).Tag] := TSpinEditEx(Sender).Value;
    ModValue();
    boSendServerConfig := True;
  end;
end;

procedure TfrmFunctionConfig.seSkill102AttackRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill102AttackRange := seSkill102AttackRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill103AttackRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill103AttackRange := seSkill103AttackRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill105AttackRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill105AttackRange := seSkill105AttackRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seNGMaxLevelLimteChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNGMaxLevelLimte := seNGMaxLevelLimte.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill106AddFrozenRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill106AddFrozenRate := seSkill106AddFrozenRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill106AddFrozenRate2Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill106AddFrozenRate2 := seSkill106AddFrozenRate2.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill106AddFrozenTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill106AddFrozenTime := seSkill106AddFrozenTime.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill106AddFrozenTime2Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill106AddFrozenTime2 := seSkill106AddFrozenTime2.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill110PushedRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill110PushedRate := seSkill110PushedRate.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill110PushedRate2Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill110PushedRate2 := seSkill110PushedRate2.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill110PushedRangeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill110PushedRange := seSkill110PushedRange.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seSkill110PushedRange2Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSkill110PushedRange2 := seSkill110PushedRange2.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSkill110PushedHighLevelClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSkill110PushedHighLevel := chkSkill110PushedHighLevel.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbSkill31LevelChange(Sender: TObject);
begin
  seSkill31Rate.OnChange := nil;
  seSkill31Rate.Value := g_Config.nSkill31Rates[cbbSkill31Level.ItemIndex];
  seSkill31Rate.OnChange := seSkill31RateChange;
end;

procedure TfrmFunctionConfig.edtHumSkill7PowerLV4Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHumSkill7PowerLV4 := edtHumSkill7PowerLV4.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.chkRecallManySlave3Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boRecallManySlave3 := chkRecallManySlave3.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSpiritualismDisableUndeadMonClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSpiritualismDisableUndeadMon := chkSpiritualismDisableUndeadMon.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.seRecallMonCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwRecallMonCount := seRecallMonCount.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.edtHumSkill45PowerLV4Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHumSkill45PowerLV4 := edtHumSkill45PowerLV4.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.edtHumSkill13PowerLV4Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwHumSkill13PowerLV4 := edtHumSkill13PowerLV4.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seMyShopOperateIntervalChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMyShopOperateInterval := seMyShopOperateInterval.Value;
  ModValue();
end;

procedure TfrmFunctionConfig.seHMPDivDuraChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHMPDivDura := seHMPDivDura.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.chkSellPlayerCurrencyType1Click(Sender: TObject);
var
  dwTemp: LongWord;
begin
  if not boOpened then
    Exit;
  dwTemp := 0;

  if chkSellPlayerCurrencyType1.Checked then
    dwTemp := dwTemp or 1;

  if chkSellPlayerCurrencyType2.Checked then
    dwTemp := dwTemp or 2;

  if chkSellPlayerCurrencyType3.Checked then
    dwTemp := dwTemp or 4;

  if chkSellPlayerCurrencyType4.Checked then
    dwTemp := dwTemp or 8;

  if chkSellPlayerCurrencyType5.Checked then
    dwTemp := dwTemp or 16;

  g_Config.nSellPlayerCurrencyTypeEx := dwTemp;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.seSellPlayerGoldTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellPlayerGoldTaxRate := seSellPlayerGoldTaxRate.Value;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.seSellPlayerGameGoldTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellPlayerGameGoldTaxRate := seSellPlayerGameGoldTaxRate.Value;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.seSellPlayerGameDiamondTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellPlayerGameDiamondTaxRate := seSellPlayerGameDiamondTaxRate.Value;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.seSellPlayerGameGirdTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellPlayerGameGirdTaxRate := seSellPlayerGameGirdTaxRate.Value;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.seSellPlayerGamePointTaxRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwSellPlayerGamePointTaxRate := seSellPlayerGamePointTaxRate.Value;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.seSellPlayerTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSellPlayerTime := seSellPlayerTime.Value;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.chkSellPlayerViewStorageClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSellPlayerViewStorage := chkSellPlayerViewStorage.Checked;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.chkSellPlayerViewStorageExClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSellPlayerViewStorageEx := chkSellPlayerViewStorageEx.Checked;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.chkSellPlayerAutoRecallHeroClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boSellPlayerAutoRecallHero := chkSellPlayerAutoRecallHero.Checked;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.btnSellPlayerOKClick(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'SellPlayerCurrencyTypeEx', g_Config.nSellPlayerCurrencyTypeEx);

  Config.WriteInteger('Setup', 'SellPlayerGoldTaxRate', g_Config.dwSellPlayerGoldTaxRate);
  Config.WriteInteger('Setup', 'SellPlayerGameGoldTaxRate', g_Config.dwSellPlayerGameGoldTaxRate);
  Config.WriteInteger('Setup', 'SellPlayerGameDiamondTaxRate', g_Config.dwSellPlayerGameDiamondTaxRate);
  Config.WriteInteger('Setup', 'SellPlayerGameGirdTaxRate', g_Config.dwSellPlayerGameGirdTaxRate);
  Config.WriteInteger('Setup', 'SellPlayerGamePointTaxRate', g_Config.dwSellPlayerGamePointTaxRate);

  Config.WriteInteger('Setup', 'SellPlayerTime', g_Config.nSellPlayerTime);

  Config.WriteBool('Setup', 'SellPlayerViewStorage', g_Config.boSellPlayerViewStorage);
  Config.WriteBool('Setup', 'SellPlayerViewStorageEx', g_Config.boSellPlayerViewStorageEx);
  Config.WriteBool('Setup', 'SellPlayerAutoRecallHero', g_Config.boSellPlayerAutoRecallHero);

  Config.WriteInteger('Setup', 'SellPlayerViewOtherInfoTextOffsetX', g_Config.nSellPlayerViewOtherInfoTextOffsetX);
  Config.WriteInteger('Setup', 'SellPlayerViewOtherInfoTextOffsetY', g_Config.nSellPlayerViewOtherInfoTextOffsetY);

  btnSellPlayerOK.Enabled := False;
end;

procedure TfrmFunctionConfig.seSellPlayerViewOtherInfoTextOffsetXChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSellPlayerViewOtherInfoTextOffsetX := seSellPlayerViewOtherInfoTextOffsetX.Value;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.seSellPlayerViewOtherInfoTextOffsetYChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nSellPlayerViewOtherInfoTextOffsetY := seSellPlayerViewOtherInfoTextOffsetY.Value;
  btnSellPlayerOK.Enabled := True;
end;

procedure TfrmFunctionConfig.chkDisableWarrContinueHitClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableWarrContinueHit := chkDisableWarrContinueHit.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.edtDisableWarrContinueHitIDsChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.sDisableWarrContinueHitIDs := edtDisableWarrContinueHitIDs.Text;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.seWarrContinueHitMinIntervalChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWarrContinueHitMinInterval := seWarrContinueHitMinInterval.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmFunctionConfig.cbbElectrodelessNewLevelChange(Sender: TObject);
begin
  seElectrodelessTimeLNew.Value := g_Config.NewLevelMagicElectrodelessTime[cbbElectrodelessNewLevel.ItemIndex];
  seElectrodelessPowerRateLNew.Value := g_Config.NewLevelMagicPowerRatesSpecific[46][cbbElectrodelessNewLevel.ItemIndex];
end;

procedure TfrmFunctionConfig.seElectrodelessTimeLNewChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbElectrodelessNewLevel.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicElectrodelessTime[cbbElectrodelessNewLevel.ItemIndex] <> seElectrodelessTimeLNew.Value then
    begin
      g_Config.NewLevelMagicElectrodelessTime[cbbElectrodelessNewLevel.ItemIndex] := seElectrodelessTimeLNew.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.seElectrodelessPowerRateLNewChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if cbbElectrodelessNewLevel.ItemIndex >= 0 then
  begin
    if g_Config.NewLevelMagicPowerRatesSpecific[46][cbbElectrodelessNewLevel.ItemIndex] <> seElectrodelessPowerRateLNew.Value then
    begin
      g_Config.NewLevelMagicPowerRatesSpecific[46][cbbElectrodelessNewLevel.ItemIndex] := seElectrodelessPowerRateLNew.Value;
      ModValue();
    end;
  end;
end;

procedure TfrmFunctionConfig.chkHeroKillMonTriggerClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boHeroKillMonTrigger := chkHeroKillMonTrigger.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkCopyMonInheritedMasterSpeedClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCopyMonInheritedMasterSpeed := chkCopyMonInheritedMasterSpeed.Checked;
  ModValue();
end;

procedure TfrmFunctionConfig.chkGroupUseOldModeClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGroupUseOldMode := chkGroupUseOldMode.Checked;
  ModValue();
end;

end.


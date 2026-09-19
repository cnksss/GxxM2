unit ItemSet;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ComCtrls, StdCtrls, ExtCtrls, RzRadGrp, Math,
  SpinEditEx, RzPanel, Vcl.Samples.Spin;

type
  TfrmItemSet = class(TForm)
    PageControl: TPageControl;
    TabSheet8: TTabSheet;
    ItemSetPageControl: TPageControl;
    TabSheet1: TTabSheet;
    GroupBox141: TGroupBox;
    Label108: TLabel;
    Label109: TLabel;
    EditItemExpRate: TSpinEditEx;
    GroupBox1: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    TabSheet2: TTabSheet;
    GroupBox142: TGroupBox;
    Label110: TLabel;
    Label3: TLabel;
    EditItemPowerRate: TSpinEditEx;
    GroupBox2: TGroupBox;
    Label4: TLabel;
    Label5: TLabel;
    TabSheet4: TTabSheet;
    TabSheet5: TTabSheet;
    TabSheet6: TTabSheet;
    ButtonItemSetSave: TButton;
    TabSheet9: TTabSheet;
    AddValuePageControl: TPageControl;
    TabSheet10: TTabSheet;
    TabSheet11: TTabSheet;
    TabSheet12: TTabSheet;
    TabSheet13: TTabSheet;
    TabSheet14: TTabSheet;
    TabSheet15: TTabSheet;
    TabSheet16: TTabSheet;
    ButtonAddValueSave: TButton;
    TabSheet17: TTabSheet;
    TabSheet18: TTabSheet;
    GroupBox3: TGroupBox;
    Label6: TLabel;
    EditMonRandomAddValue: TSpinEditEx;
    Label7: TLabel;
    EditMakeRandomAddValue: TSpinEditEx;
    GroupBox7: TGroupBox;
    Label14: TLabel;
    Label15: TLabel;
    EditDressDCAddValueMaxLimit: TSpinEditEx;
    EditDressDCAddValueRate: TSpinEditEx;
    GroupBox8: TGroupBox;
    Label16: TLabel;
    Label17: TLabel;
    EditDressMCAddValueMaxLimit: TSpinEditEx;
    EditDressMCAddValueRate: TSpinEditEx;
    GroupBox9: TGroupBox;
    Label18: TLabel;
    Label19: TLabel;
    EditDressSCAddValueMaxLimit: TSpinEditEx;
    EditDressSCAddValueRate: TSpinEditEx;
    EditDressDCAddRate: TSpinEditEx;
    Label20: TLabel;
    EditDressMCAddRate: TSpinEditEx;
    Label21: TLabel;
    Label22: TLabel;
    EditDressSCAddRate: TSpinEditEx;
    GroupBox10: TGroupBox;
    Label23: TLabel;
    Label24: TLabel;
    Label25: TLabel;
    EditNeckLace19DCAddValueMaxLimit: TSpinEditEx;
    EditNeckLace19DCAddValueRate: TSpinEditEx;
    EditNeckLace19DCAddRate: TSpinEditEx;
    GroupBox11: TGroupBox;
    Label26: TLabel;
    Label27: TLabel;
    Label28: TLabel;
    EditNeckLace19MCAddValueMaxLimit: TSpinEditEx;
    EditNeckLace19MCAddValueRate: TSpinEditEx;
    EditNeckLace19MCAddRate: TSpinEditEx;
    GroupBox12: TGroupBox;
    Label29: TLabel;
    Label30: TLabel;
    Label31: TLabel;
    EditNeckLace19SCAddValueMaxLimit: TSpinEditEx;
    EditNeckLace19SCAddValueRate: TSpinEditEx;
    EditNeckLace19SCAddRate: TSpinEditEx;
    Label32: TLabel;
    Label33: TLabel;
    Label34: TLabel;
    Label35: TLabel;
    GroupBox13: TGroupBox;
    Label36: TLabel;
    Label37: TLabel;
    Label38: TLabel;
    EditNeckLace202124DCAddValueMaxLimit: TSpinEditEx;
    EditNeckLace202124DCAddValueRate: TSpinEditEx;
    EditNeckLace202124DCAddRate: TSpinEditEx;
    GroupBox14: TGroupBox;
    Label39: TLabel;
    Label40: TLabel;
    Label41: TLabel;
    EditNeckLace202124MCAddValueMaxLimit: TSpinEditEx;
    EditNeckLace202124MCAddValueRate: TSpinEditEx;
    EditNeckLace202124MCAddRate: TSpinEditEx;
    GroupBox15: TGroupBox;
    Label42: TLabel;
    Label43: TLabel;
    Label44: TLabel;
    EditNeckLace202124SCAddValueMaxLimit: TSpinEditEx;
    EditNeckLace202124SCAddValueRate: TSpinEditEx;
    EditNeckLace202124SCAddRate: TSpinEditEx;
    GroupBox16: TGroupBox;
    Label45: TLabel;
    Label46: TLabel;
    Label47: TLabel;
    EditArmRing26MCAddValueMaxLimit: TSpinEditEx;
    EditArmRing26MCAddValueRate: TSpinEditEx;
    EditArmRing26MCAddRate: TSpinEditEx;
    GroupBox17: TGroupBox;
    Label48: TLabel;
    Label49: TLabel;
    Label50: TLabel;
    EditArmRing26DCAddValueMaxLimit: TSpinEditEx;
    EditArmRing26DCAddValueRate: TSpinEditEx;
    EditArmRing26DCAddRate: TSpinEditEx;
    GroupBox18: TGroupBox;
    Label51: TLabel;
    Label52: TLabel;
    Label53: TLabel;
    EditArmRing26SCAddValueMaxLimit: TSpinEditEx;
    EditArmRing26SCAddValueRate: TSpinEditEx;
    EditArmRing26SCAddRate: TSpinEditEx;
    Label54: TLabel;
    GroupBox19: TGroupBox;
    Label55: TLabel;
    Label56: TLabel;
    Label57: TLabel;
    EditRing22DCAddValueMaxLimit: TSpinEditEx;
    EditRing22DCAddValueRate: TSpinEditEx;
    EditRing22DCAddRate: TSpinEditEx;
    GroupBox20: TGroupBox;
    Label58: TLabel;
    Label59: TLabel;
    Label60: TLabel;
    EditRing22SCAddValueMaxLimit: TSpinEditEx;
    EditRing22SCAddValueRate: TSpinEditEx;
    EditRing22SCAddRate: TSpinEditEx;
    GroupBox21: TGroupBox;
    Label61: TLabel;
    Label62: TLabel;
    Label63: TLabel;
    EditRing22MCAddValueMaxLimit: TSpinEditEx;
    EditRing22MCAddValueRate: TSpinEditEx;
    EditRing22MCAddRate: TSpinEditEx;
    Label64: TLabel;
    GroupBox25: TGroupBox;
    Label75: TLabel;
    Label76: TLabel;
    Label77: TLabel;
    EditHelMetDCAddValueMaxLimit: TSpinEditEx;
    EditHelMetDCAddValueRate: TSpinEditEx;
    EditHelMetDCAddRate: TSpinEditEx;
    GroupBox26: TGroupBox;
    Label78: TLabel;
    Label79: TLabel;
    Label80: TLabel;
    EditHelMetMCAddValueMaxLimit: TSpinEditEx;
    EditHelMetMCAddValueRate: TSpinEditEx;
    EditHelMetMCAddRate: TSpinEditEx;
    GroupBox27: TGroupBox;
    Label81: TLabel;
    Label82: TLabel;
    Label83: TLabel;
    EditHelMetSCAddValueMaxLimit: TSpinEditEx;
    EditHelMetSCAddValueRate: TSpinEditEx;
    EditHelMetSCAddRate: TSpinEditEx;
    Label84: TLabel;
    GroupBox28: TGroupBox;
    Label85: TLabel;
    Label86: TLabel;
    EditGuildRecallTime: TSpinEditEx;
    GroupBox29: TGroupBox;
    Label87: TLabel;
    Label88: TLabel;
    TabSheet19: TTabSheet;
    PageControl1: TPageControl;
    TabSheet25: TTabSheet;
    TabSheet27: TTabSheet;
    GroupBox49: TGroupBox;
    Label152: TLabel;
    Label153: TLabel;
    EditUnknowRingDCAddRate: TSpinEditEx;
    EditUnknowRingDCAddValueMaxLimit: TSpinEditEx;
    GroupBox50: TGroupBox;
    Label155: TLabel;
    Label156: TLabel;
    EditUnknowRingMCAddRate: TSpinEditEx;
    EditUnknowRingMCAddValueMaxLimit: TSpinEditEx;
    GroupBox51: TGroupBox;
    Label158: TLabel;
    Label159: TLabel;
    EditUnknowRingSCAddRate: TSpinEditEx;
    EditUnknowRingSCAddValueMaxLimit: TSpinEditEx;
    GroupBox30: TGroupBox;
    Label89: TLabel;
    Label90: TLabel;
    EditUnknowRingACAddRate: TSpinEditEx;
    EditUnknowRingACAddValueMaxLimit: TSpinEditEx;
    GroupBox31: TGroupBox;
    Label91: TLabel;
    Label92: TLabel;
    EditUnknowRingMACAddRate: TSpinEditEx;
    EditUnknowRingMACAddValueMaxLimit: TSpinEditEx;
    ButtonUnKnowItemSave: TButton;
    GroupBox32: TGroupBox;
    Label93: TLabel;
    Label94: TLabel;
    EditUnknowNecklaceSCAddRate: TSpinEditEx;
    EditUnknowNecklaceSCAddValueMaxLimit: TSpinEditEx;
    GroupBox33: TGroupBox;
    Label95: TLabel;
    Label96: TLabel;
    EditUnknowNecklaceMACAddRate: TSpinEditEx;
    EditUnknowNecklaceMACAddValueMaxLimit: TSpinEditEx;
    GroupBox34: TGroupBox;
    Label97: TLabel;
    Label98: TLabel;
    EditUnknowNecklaceACAddRate: TSpinEditEx;
    EditUnknowNecklaceACAddValueMaxLimit: TSpinEditEx;
    GroupBox35: TGroupBox;
    Label99: TLabel;
    Label100: TLabel;
    EditUnknowNecklaceDCAddRate: TSpinEditEx;
    EditUnknowNecklaceDCAddValueMaxLimit: TSpinEditEx;
    GroupBox36: TGroupBox;
    Label101: TLabel;
    Label102: TLabel;
    EditUnknowNecklaceMCAddRate: TSpinEditEx;
    EditUnknowNecklaceMCAddValueMaxLimit: TSpinEditEx;
    TabSheet20: TTabSheet;
    GroupBox37: TGroupBox;
    Label103: TLabel;
    Label104: TLabel;
    EditUnknowHelMetSCAddRate: TSpinEditEx;
    EditUnknowHelMetSCAddValueMaxLimit: TSpinEditEx;
    GroupBox38: TGroupBox;
    Label105: TLabel;
    Label106: TLabel;
    EditUnknowHelMetMCAddRate: TSpinEditEx;
    EditUnknowHelMetMCAddValueMaxLimit: TSpinEditEx;
    GroupBox39: TGroupBox;
    Label107: TLabel;
    Label111: TLabel;
    EditUnknowHelMetDCAddRate: TSpinEditEx;
    EditUnknowHelMetDCAddValueMaxLimit: TSpinEditEx;
    GroupBox40: TGroupBox;
    Label112: TLabel;
    Label113: TLabel;
    EditUnknowHelMetACAddRate: TSpinEditEx;
    EditUnknowHelMetACAddValueMaxLimit: TSpinEditEx;
    GroupBox41: TGroupBox;
    Label114: TLabel;
    Label115: TLabel;
    EditUnknowHelMetMACAddRate: TSpinEditEx;
    EditUnknowHelMetMACAddValueMaxLimit: TSpinEditEx;
    GroupBox43: TGroupBox;
    GroupBox46: TGroupBox;
    Label117: TLabel;
    Label118: TLabel;
    GroupBox47: TGroupBox;
    CheckBoxUserMoveCanDupObj: TCheckBox;
    CheckBoxUserMoveCanOnItem: TCheckBox;
    Label119: TLabel;
    EditUserMoveTime: TSpinEditEx;
    Label121: TLabel;
    TabSheet21: TTabSheet;
    GroupBox77: TGroupBox;
    chkItemNewAbilAllowUse: TCheckBox;
    ButtonNewAbilSave: TButton;
    CheckGroupNewAbil: TRzCheckGroup;
    GroupBox63: TGroupBox;
    Label132: TLabel;
    Label136: TLabel;
    Label184: TLabel;
    EditItemNewAbilMonRandomAddRate: TSpinEditEx;
    EditItemNewAbilMakeRandomAddRate: TSpinEditEx;
    EditItemNewAbilScriptRandomAddRate: TSpinEditEx;
    GroupBoxNewAbil: TGroupBox;
    Label213: TLabel;
    Label215: TLabel;
    Label216: TLabel;
    EditItemNewAbilAddValueMaxLimit: TSpinEditEx;
    EditItemNewAbilAddValueRate: TSpinEditEx;
    EditItemNewAbilAddRate: TSpinEditEx;
    ComboBoxNewAbilItemType: TComboBox;
    GroupBox4: TGroupBox;
    Label8: TLabel;
    Label9: TLabel;
    Label186: TLabel;
    EditWeaponDCAddValueMaxLimit: TSpinEditEx;
    EditWeaponDCAddValueRate: TSpinEditEx;
    EditWeaponDCAddRate: TSpinEditEx;
    GroupBox6: TGroupBox;
    Label12: TLabel;
    Label13: TLabel;
    Label187: TLabel;
    EditWeaponSCAddValueMaxLimit: TSpinEditEx;
    EditWeaponSCAddValueRate: TSpinEditEx;
    EditWeaponSCAddRate: TSpinEditEx;
    GroupBox5: TGroupBox;
    Label10: TLabel;
    Label11: TLabel;
    Label185: TLabel;
    EditWeaponMCAddValueMaxLimit: TSpinEditEx;
    EditWeaponMCAddValueRate: TSpinEditEx;
    EditWeaponMCAddRate: TSpinEditEx;
    GroupBox67: TGroupBox;
    Label179: TLabel;
    Label180: TLabel;
    Label181: TLabel;
    EditDressACAddValueMaxLimit: TSpinEditEx;
    EditDressACAddValueRate: TSpinEditEx;
    EditDressACAddRate: TSpinEditEx;
    GroupBox68: TGroupBox;
    Label182: TLabel;
    Label183: TLabel;
    Label125: TLabel;
    EditDressMACAddValueMaxLimit: TSpinEditEx;
    EditDressMACAddValueRate: TSpinEditEx;
    EditDressMACAddRate: TSpinEditEx;
    GroupBox61: TGroupBox;
    Label161: TLabel;
    Label162: TLabel;
    Label163: TLabel;
    EditNeckLace19ACAddValueMaxLimit: TSpinEditEx;
    EditNeckLace19ACAddValueRate: TSpinEditEx;
    EditNeckLace19ACAddRate: TSpinEditEx;
    GroupBox62: TGroupBox;
    Label164: TLabel;
    Label165: TLabel;
    Label166: TLabel;
    EditNeckLace19MACAddValueMaxLimit: TSpinEditEx;
    EditNeckLace19MACAddValueRate: TSpinEditEx;
    EditNeckLace19MACAddRate: TSpinEditEx;
    GroupBox65: TGroupBox;
    Label173: TLabel;
    Label174: TLabel;
    Label175: TLabel;
    EditNeckLace202124ACAddValueMaxLimit: TSpinEditEx;
    EditNeckLace202124ACAddValueRate: TSpinEditEx;
    EditNeckLace202124ACAddRate: TSpinEditEx;
    GroupBox66: TGroupBox;
    Label176: TLabel;
    Label177: TLabel;
    Label178: TLabel;
    EditNeckLace202124MACAddValueMaxLimit: TSpinEditEx;
    EditNeckLace202124MACAddValueRate: TSpinEditEx;
    EditNeckLace202124MACAddRate: TSpinEditEx;
    GroupBox64: TGroupBox;
    Label170: TLabel;
    Label171: TLabel;
    Label172: TLabel;
    EditArmRing26MACAddValueMaxLimit: TSpinEditEx;
    EditArmRing26MACAddValueRate: TSpinEditEx;
    EditArmRing26MACAddRate: TSpinEditEx;
    GroupBox48: TGroupBox;
    Label167: TLabel;
    Label168: TLabel;
    Label169: TLabel;
    EditArmRing26ACAddValueMaxLimit: TSpinEditEx;
    EditArmRing26ACAddValueRate: TSpinEditEx;
    EditArmRing26ACAddRate: TSpinEditEx;
    GroupBox57: TGroupBox;
    Label143: TLabel;
    Label144: TLabel;
    Label145: TLabel;
    EditHelMetACAddValueMaxLimit: TSpinEditEx;
    EditHelMetACAddValueRate: TSpinEditEx;
    EditHelMetACAddRate: TSpinEditEx;
    GroupBox58: TGroupBox;
    Label146: TLabel;
    Label147: TLabel;
    Label148: TLabel;
    EditHelMetMACAddValueMaxLimit: TSpinEditEx;
    EditHelMetMACAddValueRate: TSpinEditEx;
    EditHelMetMACAddRate: TSpinEditEx;
    TabSheet22: TTabSheet;
    GroupBox53: TGroupBox;
    Label129: TLabel;
    Label130: TLabel;
    Label131: TLabel;
    EditBootsDCAddValueMaxLimit: TSpinEditEx;
    EditBootsDCAddValueRate: TSpinEditEx;
    EditBootsDCAddRate: TSpinEditEx;
    GroupBox52: TGroupBox;
    Label126: TLabel;
    Label127: TLabel;
    Label128: TLabel;
    EditBootsSCAddValueMaxLimit: TSpinEditEx;
    EditBootsSCAddValueRate: TSpinEditEx;
    EditBootsSCAddRate: TSpinEditEx;
    GroupBox54: TGroupBox;
    Label133: TLabel;
    Label134: TLabel;
    Label135: TLabel;
    EditBootsMCAddValueMaxLimit: TSpinEditEx;
    EditBootsMCAddValueRate: TSpinEditEx;
    EditBootsMCAddRate: TSpinEditEx;
    GroupBox56: TGroupBox;
    Label140: TLabel;
    Label141: TLabel;
    Label142: TLabel;
    EditBootsMACAddValueMaxLimit: TSpinEditEx;
    EditBootsMACAddValueRate: TSpinEditEx;
    EditBootsMACAddRate: TSpinEditEx;
    GroupBox55: TGroupBox;
    Label137: TLabel;
    Label138: TLabel;
    Label139: TLabel;
    EditBootsACAddValueMaxLimit: TSpinEditEx;
    EditBootsACAddValueRate: TSpinEditEx;
    EditBootsACAddRate: TSpinEditEx;
    Label149: TLabel;
    Label150: TLabel;
    Label74: TLabel;
    GroupBox60: TGroupBox;
    Label154: TLabel;
    Label157: TLabel;
    Label160: TLabel;
    EditRing23MACAddValueMaxLimit: TSpinEditEx;
    EditRing23MACAddValueRate: TSpinEditEx;
    EditRing23MACAddRate: TSpinEditEx;
    GroupBox59: TGroupBox;
    Label65: TLabel;
    Label66: TLabel;
    Label151: TLabel;
    EditRing23ACAddValueMaxLimit: TSpinEditEx;
    EditRing23ACAddValueRate: TSpinEditEx;
    EditRing23ACAddRate: TSpinEditEx;
    GroupBox22: TGroupBox;
    Label67: TLabel;
    Label68: TLabel;
    Label69: TLabel;
    EditRing23DCAddValueMaxLimit: TSpinEditEx;
    EditRing23DCAddValueRate: TSpinEditEx;
    EditRing23DCAddRate: TSpinEditEx;
    GroupBox24: TGroupBox;
    Label71: TLabel;
    Label72: TLabel;
    Label73: TLabel;
    EditRing23SCAddValueMaxLimit: TSpinEditEx;
    EditRing23SCAddValueRate: TSpinEditEx;
    EditRing23SCAddRate: TSpinEditEx;
    GroupBox23: TGroupBox;
    Label70: TLabel;
    Label188: TLabel;
    Label189: TLabel;
    EditRing23MCAddValueMaxLimit: TSpinEditEx;
    EditRing23MCAddValueRate: TSpinEditEx;
    EditRing23MCAddRate: TSpinEditEx;
    GroupBox69: TGroupBox;
    Label190: TLabel;
    Label191: TLabel;
    Label192: TLabel;
    EditWeaponHitSpeedAddValueMaxLimit: TSpinEditEx;
    EditWeaponHitSpeedAddValueRate: TSpinEditEx;
    EditWeaponHitSpeedAddRate: TSpinEditEx;
    lbl1: TLabel;
    ts1: TTabSheet;
    lbl2: TLabel;
    grp1: TGroupBox;
    lbl3: TLabel;
    lbl4: TLabel;
    lbl5: TLabel;
    grp2: TGroupBox;
    lbl6: TLabel;
    lbl7: TLabel;
    lbl8: TLabel;
    grp3: TGroupBox;
    lbl9: TLabel;
    lbl10: TLabel;
    lbl11: TLabel;
    grp4: TGroupBox;
    lbl12: TLabel;
    lbl13: TLabel;
    lbl14: TLabel;
    seFashionWeaponDCAddValueMaxLimit: TSpinEditEx;
    seFashionWeaponDCAddValueRate: TSpinEditEx;
    seFashionWeaponDCAddRate: TSpinEditEx;
    seFashionWeaponHitSpeedAddValueMaxLimit: TSpinEditEx;
    seFashionWeaponHitSpeedAddValueRate: TSpinEditEx;
    seFashionWeaponHitSpeedAddRate: TSpinEditEx;
    seFashionWeaponSCAddValueMaxLimit: TSpinEditEx;
    seFashionWeaponSCAddValueRate: TSpinEditEx;
    seFashionWeaponSCAddRate: TSpinEditEx;
    seFashionWeaponMCAddRate: TSpinEditEx;
    seFashionWeaponMCAddValueRate: TSpinEditEx;
    seFashionWeaponMCAddValueMaxLimit: TSpinEditEx;
    ts2: TTabSheet;
    grp5: TGroupBox;
    lbl15: TLabel;
    lbl16: TLabel;
    lbl17: TLabel;
    grp6: TGroupBox;
    lbl18: TLabel;
    lbl19: TLabel;
    lbl20: TLabel;
    grp7: TGroupBox;
    lbl21: TLabel;
    lbl22: TLabel;
    lbl23: TLabel;
    grp8: TGroupBox;
    lbl24: TLabel;
    lbl25: TLabel;
    lbl26: TLabel;
    grp9: TGroupBox;
    lbl27: TLabel;
    lbl28: TLabel;
    lbl29: TLabel;
    lbl30: TLabel;
    seFashionDressDCAddValueMaxLimit: TSpinEditEx;
    seFashionDressDCAddValueRate: TSpinEditEx;
    seFashionDressDCAddRate: TSpinEditEx;
    seFashionDressSCAddValueMaxLimit: TSpinEditEx;
    seFashionDressSCAddValueRate: TSpinEditEx;
    seFashionDressSCAddRate: TSpinEditEx;
    seFashionDressMACAddValueMaxLimit: TSpinEditEx;
    seFashionDressMACAddValueRate: TSpinEditEx;
    seFashionDressMCAddValueMaxLimit: TSpinEditEx;
    seFashionDressMCAddValueRate: TSpinEditEx;
    seFashionDressMCAddRate: TSpinEditEx;
    seFashionDressMACAddRate: TSpinEditEx;
    seFashionDressACAddValueMaxLimit: TSpinEditEx;
    seFashionDressACAddValueRate: TSpinEditEx;
    seFashionDressACAddRate: TSpinEditEx;
    lbl31: TLabel;
    grp10: TGroupBox;
    lbl32: TLabel;
    lbl33: TLabel;
    lbl34: TLabel;
    grp11: TGroupBox;
    lbl35: TLabel;
    lbl36: TLabel;
    lbl37: TLabel;
    grp12: TGroupBox;
    lbl38: TLabel;
    lbl39: TLabel;
    lbl40: TLabel;
    grp13: TGroupBox;
    lbl41: TLabel;
    lbl42: TLabel;
    lbl43: TLabel;
    grp14: TGroupBox;
    lbl44: TLabel;
    lbl45: TLabel;
    lbl46: TLabel;
    ts4: TTabSheet;
    lbl47: TLabel;
    grp15: TGroupBox;
    lbl48: TLabel;
    lbl49: TLabel;
    lbl50: TLabel;
    grp16: TGroupBox;
    lbl51: TLabel;
    lbl52: TLabel;
    lbl53: TLabel;
    grp17: TGroupBox;
    lbl54: TLabel;
    lbl55: TLabel;
    lbl56: TLabel;
    grp18: TGroupBox;
    lbl57: TLabel;
    lbl58: TLabel;
    lbl59: TLabel;
    grp19: TGroupBox;
    lbl60: TLabel;
    lbl61: TLabel;
    lbl62: TLabel;
    ts5: TTabSheet;
    lbl63: TLabel;
    grp20: TGroupBox;
    lbl64: TLabel;
    lbl65: TLabel;
    lbl66: TLabel;
    grp21: TGroupBox;
    lbl67: TLabel;
    lbl68: TLabel;
    lbl69: TLabel;
    grp22: TGroupBox;
    lbl70: TLabel;
    lbl71: TLabel;
    lbl72: TLabel;
    grp23: TGroupBox;
    lbl73: TLabel;
    lbl74: TLabel;
    lbl75: TLabel;
    grp24: TGroupBox;
    lbl76: TLabel;
    lbl77: TLabel;
    lbl78: TLabel;
    seHorseDCAddValueMaxLimit: TSpinEditEx;
    seHorseDCAddValueRate: TSpinEditEx;
    seHorseDCAddRate: TSpinEditEx;
    seHorseSCAddValueMaxLimit: TSpinEditEx;
    seHorseSCAddValueRate: TSpinEditEx;
    seHorseSCAddRate: TSpinEditEx;
    seHorseMACAddValueMaxLimit: TSpinEditEx;
    seHorseMACAddValueRate: TSpinEditEx;
    seHorseMACAddRate: TSpinEditEx;
    seHorseMCAddValueMaxLimit: TSpinEditEx;
    seHorseMCAddValueRate: TSpinEditEx;
    seHorseMCAddRate: TSpinEditEx;
    seHorseACAddValueMaxLimit: TSpinEditEx;
    seHorseACAddValueRate: TSpinEditEx;
    seHorseACAddRate: TSpinEditEx;
    seDrumDCAddValueRate: TSpinEditEx;
    seDrumDCAddRate: TSpinEditEx;
    seDrumACAddValueMaxLimit: TSpinEditEx;
    seDrumACAddValueRate: TSpinEditEx;
    seDrumACAddRate: TSpinEditEx;
    seDrumSCAddValueMaxLimit: TSpinEditEx;
    seDrumSCAddValueRate: TSpinEditEx;
    seDrumSCAddRate: TSpinEditEx;
    seDrumMACAddValueMaxLimit: TSpinEditEx;
    seDrumMACAddRate: TSpinEditEx;
    seDrumMCAddValueMaxLimit: TSpinEditEx;
    seDrumMCAddValueRate: TSpinEditEx;
    seDrumMCAddRate: TSpinEditEx;
    seDrumDCAddValueMaxLimit: TSpinEditEx;
    seDrumMACAddValueRate: TSpinEditEx;
    seShieldDCAddValueMaxLimit: TSpinEditEx;
    seShieldDCAddValueRate: TSpinEditEx;
    seShieldDCAddRate: TSpinEditEx;
    seShieldACAddValueMaxLimit: TSpinEditEx;
    seShieldACAddValueRate: TSpinEditEx;
    seShieldACAddRate: TSpinEditEx;
    seShieldSCAddValueMaxLimit: TSpinEditEx;
    seShieldSCAddValueRate: TSpinEditEx;
    seShieldSCAddRate: TSpinEditEx;
    seShieldMACAddValueMaxLimit: TSpinEditEx;
    seShieldMACAddValueRate: TSpinEditEx;
    seShieldMACAddRate: TSpinEditEx;
    seShieldMCAddValueMaxLimit: TSpinEditEx;
    seShieldMCAddValueRate: TSpinEditEx;
    seShieldMCAddRate: TSpinEditEx;
    GroupBox42: TGroupBox;
    Label120: TLabel;
    Label116: TLabel;
    Label124: TLabel;
    EditAttackPosionRate: TSpinEditEx;
    EditAttackPosionTime: TSpinEditEx;
    grpMD: TGroupBox;
    Label193: TLabel;
    Label194: TLabel;
    Label195: TLabel;
    seMDParalysisRate: TSpinEditEx;
    seMDParalysisTime: TSpinEditEx;
    Label196: TLabel;
    grpBD: TGroupBox;
    Label122: TLabel;
    Label123: TLabel;
    Label197: TLabel;
    seFrozenRate: TSpinEditEx;
    seFrozenTime: TSpinEditEx;
    grpZW: TGroupBox;
    Label198: TLabel;
    Label199: TLabel;
    Label200: TLabel;
    seCobwebWindingRate: TSpinEditEx;
    seCobwebWindingTime: TSpinEditEx;
    chkFrozenUseMagicStruck: TCheckBox;
    chkCobwebWindingUseMagicStruck: TCheckBox;
    GroupBox44: TGroupBox;
    Label201: TLabel;
    seCritAttackHurtRate: TSpinEditEx;
    GroupBox45: TGroupBox;
    Label203: TLabel;
    Label204: TLabel;
    Label205: TLabel;
    EditRing22ACAddValueMaxLimit: TSpinEditEx;
    EditRing22ACAddValueRate: TSpinEditEx;
    EditRing22ACAddRate: TSpinEditEx;
    GroupBox70: TGroupBox;
    Label206: TLabel;
    Label207: TLabel;
    Label208: TLabel;
    EditRing22MACAddValueMaxLimit: TSpinEditEx;
    EditRing22MACAddValueRate: TSpinEditEx;
    EditRing22MACAddRate: TSpinEditEx;
    tsBase: TTabSheet;
    grp25: TGroupBox;
    chkOpenItemFlute: TCheckBox;
    Label209: TLabel;
    seItemFluteStoneCount: TSpinEditEx;
    Label210: TLabel;
    seDamageReboundRate: TSpinEditEx;
    Label202: TLabel;
    EditItemNewAbilAddValueMaxLimit2: TSpinEditEx;
    lbl80: TLabel;
    chkDisableRightClickFluteStone: TCheckBox;
    Label211: TLabel;
    seItemFluteStoneIdxCount: TSpinEditEx;
    lbl79: TLabel;
    Label212: TLabel;
    seFatalBlowBasePower: TSpinEditEx;
    Label214: TLabel;
    seFatalBlowNeedPower1: TSpinEditEx;
    Label217: TLabel;
    seFatalBlowNeedPower2: TSpinEditEx;
    Label218: TLabel;
    seFatalBlowNeedPower3: TSpinEditEx;
    chkCloseDefenseUseScale: TCheckBox;
    chkReboundUseScale: TCheckBox;
    Label219: TLabel;
    seItemFluteStoneOverlapCount: TSpinEditEx;
    lbl81: TLabel;
    Label220: TLabel;
    Label221: TLabel;
    Label222: TLabel;
    Label223: TLabel;
    Label224: TLabel;
    procedure EditItemExpRateChange(Sender: TObject);
    procedure EditItemPowerRateChange(Sender: TObject);
    procedure ButtonItemSetSaveClick(Sender: TObject);
    procedure ButtonAddValueSaveClick(Sender: TObject);
    procedure EditMonRandomAddValueChange(Sender: TObject);
    procedure EditMakeRandomAddValueChange(Sender: TObject);
    procedure EditWeaponDCAddValueMaxLimitChange(Sender: TObject);
    procedure EditWeaponDCAddValueRateChange(Sender: TObject);
    procedure EditWeaponMCAddValueMaxLimitChange(Sender: TObject);
    procedure EditWeaponMCAddValueRateChange(Sender: TObject);
    procedure EditWeaponSCAddValueMaxLimitChange(Sender: TObject);
    procedure EditWeaponSCAddValueRateChange(Sender: TObject);
    procedure EditDressDCAddValueMaxLimitChange(Sender: TObject);
    procedure EditDressDCAddValueRateChange(Sender: TObject);
    procedure EditDressMCAddValueMaxLimitChange(Sender: TObject);
    procedure EditDressMCAddValueRateChange(Sender: TObject);
    procedure EditDressSCAddValueMaxLimitChange(Sender: TObject);
    procedure EditDressSCAddValueRateChange(Sender: TObject);
    procedure EditDressDCAddRateChange(Sender: TObject);
    procedure EditDressMCAddRateChange(Sender: TObject);
    procedure EditDressSCAddRateChange(Sender: TObject);
    procedure EditNeckLace19DCAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace19DCAddValueRateChange(Sender: TObject);
    procedure EditNeckLace19DCAddRateChange(Sender: TObject);
    procedure EditNeckLace19SCAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace19SCAddValueRateChange(Sender: TObject);
    procedure EditNeckLace19SCAddRateChange(Sender: TObject);
    procedure EditNeckLace19MCAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace19MCAddValueRateChange(Sender: TObject);
    procedure EditNeckLace19MCAddRateChange(Sender: TObject);
    procedure EditNeckLace202124DCAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace202124DCAddValueRateChange(Sender: TObject);
    procedure EditNeckLace202124DCAddRateChange(Sender: TObject);
    procedure EditNeckLace202124SCAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace202124SCAddValueRateChange(Sender: TObject);
    procedure EditNeckLace202124SCAddRateChange(Sender: TObject);
    procedure EditNeckLace202124MCAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace202124MCAddValueRateChange(Sender: TObject);
    procedure EditNeckLace202124MCAddRateChange(Sender: TObject);
    procedure EditArmRing26DCAddValueMaxLimitChange(Sender: TObject);
    procedure EditArmRing26DCAddValueRateChange(Sender: TObject);
    procedure EditArmRing26DCAddRateChange(Sender: TObject);
    procedure EditArmRing26SCAddValueMaxLimitChange(Sender: TObject);
    procedure EditArmRing26SCAddValueRateChange(Sender: TObject);
    procedure EditArmRing26SCAddRateChange(Sender: TObject);
    procedure EditArmRing26MCAddValueMaxLimitChange(Sender: TObject);
    procedure EditArmRing26MCAddValueRateChange(Sender: TObject);
    procedure EditArmRing26MCAddRateChange(Sender: TObject);
    procedure EditRing22DCAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing22DCAddValueRateChange(Sender: TObject);
    procedure EditRing22DCAddRateChange(Sender: TObject);
    procedure EditRing22SCAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing22SCAddValueRateChange(Sender: TObject);
    procedure EditRing22SCAddRateChange(Sender: TObject);
    procedure EditRing22MCAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing22MCAddValueRateChange(Sender: TObject);
    procedure EditRing22MCAddRateChange(Sender: TObject);
    procedure EditRing23DCAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing23DCAddValueRateChange(Sender: TObject);
    procedure EditRing23DCAddRateChange(Sender: TObject);
    procedure EditRing23SCAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing23SCAddValueRateChange(Sender: TObject);
    procedure EditRing23SCAddRateChange(Sender: TObject);
    procedure EditRing23MCAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing23MCAddValueRateChange(Sender: TObject);
    procedure EditRing23MCAddRateChange(Sender: TObject);
    procedure EditHelMetDCAddValueMaxLimitChange(Sender: TObject);
    procedure EditHelMetDCAddValueRateChange(Sender: TObject);
    procedure EditHelMetDCAddRateChange(Sender: TObject);
    procedure EditHelMetSCAddValueMaxLimitChange(Sender: TObject);
    procedure EditHelMetSCAddValueRateChange(Sender: TObject);
    procedure EditHelMetSCAddRateChange(Sender: TObject);
    procedure EditHelMetMCAddValueMaxLimitChange(Sender: TObject);
    procedure EditHelMetMCAddValueRateChange(Sender: TObject);
    procedure EditHelMetMCAddRateChange(Sender: TObject);
    procedure EditGuildRecallTimeChange(Sender: TObject);
    procedure ButtonUnKnowItemSaveClick(Sender: TObject);
    procedure EditUnknowRingDCAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowRingDCAddRateChange(Sender: TObject);
    procedure EditUnknowRingMCAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowRingMCAddRateChange(Sender: TObject);
    procedure EditUnknowRingSCAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowRingSCAddRateChange(Sender: TObject);
    procedure EditUnknowRingACAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowRingACAddRateChange(Sender: TObject);
    procedure EditUnknowRingMACAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowRingMACAddRateChange(Sender: TObject);
    procedure EditUnknowNecklaceDCAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowNecklaceDCAddRateChange(Sender: TObject);
    procedure EditUnknowNecklaceMCAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowNecklaceMCAddRateChange(Sender: TObject);
    procedure EditUnknowNecklaceSCAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowNecklaceSCAddRateChange(Sender: TObject);
    procedure EditUnknowNecklaceACAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowNecklaceACAddRateChange(Sender: TObject);
    procedure EditUnknowNecklaceMACAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowNecklaceMACAddRateChange(Sender: TObject);
    procedure EditUnknowHelMetDCAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowHelMetDCAddRateChange(Sender: TObject);
    procedure EditUnknowHelMetMCAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowHelMetMCAddRateChange(Sender: TObject);
    procedure EditUnknowHelMetSCAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowHelMetSCAddRateChange(Sender: TObject);
    procedure EditUnknowHelMetACAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowHelMetACAddRateChange(Sender: TObject);
    procedure EditUnknowHelMetMACAddValueMaxLimitChange(Sender: TObject);
    procedure EditUnknowHelMetMACAddRateChange(Sender: TObject);
    procedure EditAttackPosionRateChange(Sender: TObject);
    procedure EditAttackPosionTimeChange(Sender: TObject);
    procedure CheckBoxUserMoveCanDupObjClick(Sender: TObject);
    procedure CheckBoxUserMoveCanOnItemClick(Sender: TObject);
    procedure EditUserMoveTimeChange(Sender: TObject);
    procedure chkItemNewAbilAllowUseClick(Sender: TObject);
    procedure EditItemNewAbilMonRandomAddRateChange(Sender: TObject);
    procedure EditItemNewAbilMakeRandomAddRateChange(Sender: TObject);
    procedure EditItemNewAbilScriptRandomAddRateChange(Sender: TObject);
    procedure ComboBoxNewAbilItemTypeChange(Sender: TObject);
    procedure CheckGroupNewAbilChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
    procedure EditItemNewAbilAddValueMaxLimitChange(Sender: TObject);
    procedure EditItemNewAbilAddValueRateChange(Sender: TObject);
    procedure EditItemNewAbilAddRateChange(Sender: TObject);
    procedure ButtonNewAbilSaveClick(Sender: TObject);
    procedure EditWeaponDCAddRateChange(Sender: TObject);
    procedure EditWeaponSCAddRateChange(Sender: TObject);
    procedure EditWeaponMCAddRateChange(Sender: TObject);
    procedure EditDressACAddValueMaxLimitChange(Sender: TObject);
    procedure EditDressACAddValueRateChange(Sender: TObject);
    procedure EditDressACAddRateChange(Sender: TObject);
    procedure EditDressMACAddValueMaxLimitChange(Sender: TObject);
    procedure EditDressMACAddValueRateChange(Sender: TObject);
    procedure EditDressMACAddRateChange(Sender: TObject);
    procedure EditNeckLace19ACAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace19ACAddValueRateChange(Sender: TObject);
    procedure EditNeckLace19ACAddRateChange(Sender: TObject);
    procedure EditNeckLace19MACAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace19MACAddValueRateChange(Sender: TObject);
    procedure EditNeckLace19MACAddRateChange(Sender: TObject);
    procedure EditNeckLace202124ACAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace202124ACAddValueRateChange(Sender: TObject);
    procedure EditNeckLace202124ACAddRateChange(Sender: TObject);
    procedure EditNeckLace202124MACAddValueMaxLimitChange(Sender: TObject);
    procedure EditNeckLace202124MACAddValueRateChange(Sender: TObject);
    procedure EditNeckLace202124MACAddRateChange(Sender: TObject);
    procedure EditArmRing26ACAddValueMaxLimitChange(Sender: TObject);
    procedure EditArmRing26ACAddValueRateChange(Sender: TObject);
    procedure EditArmRing26ACAddRateChange(Sender: TObject);
    procedure EditArmRing26MACAddValueMaxLimitChange(Sender: TObject);
    procedure EditArmRing26MACAddValueRateChange(Sender: TObject);
    procedure EditArmRing26MACAddRateChange(Sender: TObject);
    procedure EditRing23ACAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing23ACAddValueRateChange(Sender: TObject);
    procedure EditRing23ACAddRateChange(Sender: TObject);
    procedure EditRing23MACAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing23MACAddValueRateChange(Sender: TObject);
    procedure EditRing23MACAddRateChange(Sender: TObject);
    procedure EditHelMetACAddValueMaxLimitChange(Sender: TObject);
    procedure EditHelMetACAddValueRateChange(Sender: TObject);
    procedure EditHelMetACAddRateChange(Sender: TObject);
    procedure EditHelMetMACAddValueMaxLimitChange(Sender: TObject);
    procedure EditHelMetMACAddValueRateChange(Sender: TObject);
    procedure EditHelMetMACAddRateChange(Sender: TObject);
    procedure EditBootsDCAddValueMaxLimitChange(Sender: TObject);
    procedure EditBootsDCAddValueRateChange(Sender: TObject);
    procedure EditBootsDCAddRateChange(Sender: TObject);
    procedure EditBootsSCAddValueMaxLimitChange(Sender: TObject);
    procedure EditBootsSCAddValueRateChange(Sender: TObject);
    procedure EditBootsSCAddRateChange(Sender: TObject);
    procedure EditBootsMCAddValueMaxLimitChange(Sender: TObject);
    procedure EditBootsMCAddValueRateChange(Sender: TObject);
    procedure EditBootsMCAddRateChange(Sender: TObject);
    procedure EditBootsACAddValueMaxLimitChange(Sender: TObject);
    procedure EditBootsACAddValueRateChange(Sender: TObject);
    procedure EditBootsACAddRateChange(Sender: TObject);
    procedure EditBootsMACAddValueMaxLimitChange(Sender: TObject);
    procedure EditBootsMACAddValueRateChange(Sender: TObject);
    procedure EditBootsMACAddRateChange(Sender: TObject);
    procedure EditWeaponHitSpeedAddValueMaxLimitChange(Sender: TObject);
    procedure EditWeaponHitSpeedAddValueRateChange(Sender: TObject);
    procedure EditWeaponHitSpeedAddRateChange(Sender: TObject);
    procedure seFashionDressDCAddValueMaxLimitChange(Sender: TObject);
    procedure seFashionWeaponDCAddValueMaxLimitChange(Sender: TObject);
    procedure seFashionWeaponDCAddValueRateChange(Sender: TObject);
    procedure seFashionWeaponDCAddRateChange(Sender: TObject);
    procedure seFashionWeaponHitSpeedAddValueMaxLimitChange(Sender: TObject);
    procedure seFashionWeaponHitSpeedAddValueRateChange(Sender: TObject);
    procedure seFashionWeaponHitSpeedAddRateChange(Sender: TObject);
    procedure seFashionWeaponSCAddValueMaxLimitChange(Sender: TObject);
    procedure seFashionWeaponSCAddValueRateChange(Sender: TObject);
    procedure seFashionWeaponSCAddRateChange(Sender: TObject);
    procedure seFashionWeaponMCAddValueMaxLimitChange(Sender: TObject);
    procedure seFashionWeaponMCAddValueRateChange(Sender: TObject);
    procedure seFashionWeaponMCAddRateChange(Sender: TObject);
    procedure seFashionDressDCAddValueRateChange(Sender: TObject);
    procedure seFashionDressDCAddRateChange(Sender: TObject);
    procedure seFashionDressACAddValueMaxLimitChange(Sender: TObject);
    procedure seFashionDressACAddValueRateChange(Sender: TObject);
    procedure seFashionDressACAddRateChange(Sender: TObject);
    procedure seFashionDressSCAddValueMaxLimitChange(Sender: TObject);
    procedure seFashionDressSCAddValueRateChange(Sender: TObject);
    procedure seFashionDressSCAddRateChange(Sender: TObject);
    procedure seFashionDressMACAddValueMaxLimitChange(Sender: TObject);
    procedure seFashionDressMACAddValueRateChange(Sender: TObject);
    procedure seFashionDressMACAddRateChange(Sender: TObject);
    procedure seFashionDressMCAddValueMaxLimitChange(Sender: TObject);
    procedure seFashionDressMCAddValueRateChange(Sender: TObject);
    procedure seFashionDressMCAddRateChange(Sender: TObject);
    procedure seHorseDCAddValueMaxLimitChange(Sender: TObject);
    procedure seHorseDCAddValueRateChange(Sender: TObject);
    procedure seHorseDCAddRateChange(Sender: TObject);
    procedure seHorseACAddValueMaxLimitChange(Sender: TObject);
    procedure seHorseACAddValueRateChange(Sender: TObject);
    procedure seHorseACAddRateChange(Sender: TObject);
    procedure seHorseSCAddValueMaxLimitChange(Sender: TObject);
    procedure seHorseSCAddValueRateChange(Sender: TObject);
    procedure seHorseSCAddRateChange(Sender: TObject);
    procedure seHorseMACAddValueMaxLimitChange(Sender: TObject);
    procedure seHorseMACAddValueRateChange(Sender: TObject);
    procedure seHorseMACAddRateChange(Sender: TObject);
    procedure seHorseMCAddValueMaxLimitChange(Sender: TObject);
    procedure seHorseMCAddValueRateChange(Sender: TObject);
    procedure seHorseMCAddRateChange(Sender: TObject);
    procedure seDrumDCAddValueRateChange(Sender: TObject);
    procedure seDrumDCAddValueMaxLimitChange(Sender: TObject);
    procedure seDrumDCAddRateChange(Sender: TObject);
    procedure seDrumACAddValueMaxLimitChange(Sender: TObject);
    procedure seDrumACAddValueRateChange(Sender: TObject);
    procedure seDrumACAddRateChange(Sender: TObject);
    procedure seDrumSCAddValueMaxLimitChange(Sender: TObject);
    procedure seDrumSCAddValueRateChange(Sender: TObject);
    procedure seDrumSCAddRateChange(Sender: TObject);
    procedure seDrumMACAddValueMaxLimitChange(Sender: TObject);
    procedure seDrumMACAddValueRateChange(Sender: TObject);
    procedure seDrumMACAddRateChange(Sender: TObject);
    procedure seDrumMCAddValueMaxLimitChange(Sender: TObject);
    procedure seDrumMCAddValueRateChange(Sender: TObject);
    procedure seDrumMCAddRateChange(Sender: TObject);
    procedure seShieldDCAddValueMaxLimitChange(Sender: TObject);
    procedure seShieldDCAddValueRateChange(Sender: TObject);
    procedure seShieldDCAddRateChange(Sender: TObject);
    procedure seShieldACAddValueMaxLimitChange(Sender: TObject);
    procedure seShieldACAddValueRateChange(Sender: TObject);
    procedure seShieldACAddRateChange(Sender: TObject);
    procedure seShieldSCAddValueMaxLimitChange(Sender: TObject);
    procedure seShieldSCAddValueRateChange(Sender: TObject);
    procedure seShieldSCAddRateChange(Sender: TObject);
    procedure seShieldMACAddValueMaxLimitChange(Sender: TObject);
    procedure seShieldMACAddValueRateChange(Sender: TObject);
    procedure seShieldMACAddRateChange(Sender: TObject);
    procedure seShieldMCAddValueMaxLimitChange(Sender: TObject);
    procedure seShieldMCAddValueRateChange(Sender: TObject);
    procedure seShieldMCAddRateChange(Sender: TObject);
    procedure seMDParalysisRateChange(Sender: TObject);
    procedure seMDParalysisTimeChange(Sender: TObject);
    procedure seFrozenRateChange(Sender: TObject);
    procedure seFrozenTimeChange(Sender: TObject);
    procedure seCobwebWindingRateChange(Sender: TObject);
    procedure seCobwebWindingTimeChange(Sender: TObject);
    procedure chkFrozenUseMagicStruckClick(Sender: TObject);
    procedure chkCobwebWindingUseMagicStruckClick(Sender: TObject);
    procedure seCritAttackHurtRateChange(Sender: TObject);
    procedure EditRing22ACAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing22ACAddValueRateChange(Sender: TObject);
    procedure EditRing22ACAddRateChange(Sender: TObject);
    procedure EditRing22MACAddValueMaxLimitChange(Sender: TObject);
    procedure EditRing22MACAddValueRateChange(Sender: TObject);
    procedure EditRing22MACAddRateChange(Sender: TObject);
    procedure chkOpenItemFluteClick(Sender: TObject);
    procedure seItemFluteStoneCountChange(Sender: TObject);
    procedure seDamageReboundRateChange(Sender: TObject);
    procedure EditItemNewAbilAddValueMaxLimit2Change(Sender: TObject);
    procedure chkDisableRightClickFluteStoneClick(Sender: TObject);
    procedure seItemFluteStoneIdxCountChange(Sender: TObject);
    procedure seFatalBlowBasePowerChange(Sender: TObject);
    procedure seFatalBlowNeedPower1Change(Sender: TObject);
    procedure seFatalBlowNeedPower2Change(Sender: TObject);
    procedure seFatalBlowNeedPower3Change(Sender: TObject);
    procedure chkCloseDefenseUseScaleClick(Sender: TObject);
    procedure chkReboundUseScaleClick(Sender: TObject);
    procedure seItemFluteStoneOverlapCountChange(Sender: TObject);
  private
    boOpened: Boolean;
    boModValued: Boolean;
    boSendServerConfig: Boolean;
    procedure ModValue();
    procedure uModValue();
    procedure RefUnknowItem();
    procedure RefShapeItem();

    procedure RefItemNewAbil();
    { Private declarations }
  public
    procedure Open();
    { Public declarations }
  end;

var
  frmItemSet: TfrmItemSet;

implementation

uses
  M2Share;

{$R *.dfm}

{ TfrmItemSet }

procedure TfrmItemSet.ModValue;
begin
  boModValued := True;
  ButtonItemSetSave.Enabled := True;
  ButtonAddValueSave.Enabled := True;
  ButtonUnKnowItemSave.Enabled := True;
  ButtonNewAbilSave.Enabled := True;
end;

procedure TfrmItemSet.uModValue;
begin
  boModValued := False;
  ButtonItemSetSave.Enabled := False;
  ButtonAddValueSave.Enabled := False;
  ButtonUnKnowItemSave.Enabled := False;
  ButtonNewAbilSave.Enabled := False;
end;

procedure TfrmItemSet.RefItemNewAbil();
begin
  chkItemNewAbilAllowUse.Checked := g_Config.boItemNewAbilAllowUse;
  EditItemNewAbilMonRandomAddRate.Value := g_Config.nItemNewAbilMonRandomAddValue;
  EditItemNewAbilMakeRandomAddRate.Value := g_Config.nItemNewAbilMakeRandomAddValue;
  EditItemNewAbilScriptRandomAddRate.Value := g_Config.nItemNewAbilScriptRandomAddValue;
  ComboBoxNewAbilItemType.ItemIndex := -1;

  chkCloseDefenseUseScale.Checked := g_Config.boCloseDefenseUseScale;
  chkReboundUseScale.Checked := g_Config.boReboundUseScale;
end;

procedure TfrmItemSet.Open;
begin
  boOpened := False;
  uModValue();
  boSendServerConfig := False;

  chkOpenItemFlute.Checked := g_Config.boOpenItemFlute;
  chkDisableRightClickFluteStone.Checked := g_Config.boDisableRightClickFluteStone;

  seItemFluteStoneCount.Value := g_Config.nItemFluteStoneCount;
  seItemFluteStoneIdxCount.Value := g_Config.nItemFluteStoneIdxCount;
  seItemFluteStoneOverlapCount.Value := g_Config.nItemFluteStoneOverlapCount;

  EditItemExpRate.Value := g_Config.nItemExpRate;
  EditItemPowerRate.Value := g_Config.nItemPowerRate;

  EditMonRandomAddValue.Value := g_Config.nMonRandomAddValue;
  EditMakeRandomAddValue.Value := g_Config.nMakeRandomAddValue;

  EditWeaponDCAddValueMaxLimit.Value := g_Config.nWeaponDCAddValueMaxLimit;
  EditWeaponDCAddValueRate.Value := g_Config.nWeaponDCAddValueRate;
  EditWeaponMCAddValueMaxLimit.Value := g_Config.nWeaponMCAddValueMaxLimit;
  EditWeaponMCAddValueRate.Value := g_Config.nWeaponMCAddValueRate;
  EditWeaponSCAddValueMaxLimit.Value := g_Config.nWeaponSCAddValueMaxLimit;
  EditWeaponSCAddValueRate.Value := g_Config.nWeaponSCAddValueRate;
  EditWeaponDCAddRate.Value := g_Config.nWeaponDCAddRate;
  EditWeaponSCAddRate.Value := g_Config.nWeaponSCAddRate;
  EditWeaponMCAddRate.Value := g_Config.nWeaponMCAddRate;

  EditWeaponHitSpeedAddValueMaxLimit.Value := g_Config.nWeaponHitSpeedAddValueMaxLimit;
  EditWeaponHitSpeedAddValueRate.Value := g_Config.nWeaponHitSpeedAddValueRate;
  EditWeaponHitSpeedAddRate.Value := g_Config.nWeaponHitSpeedAddRate;

  EditDressDCAddRate.Value := g_Config.nDressDCAddRate;
  EditDressDCAddValueMaxLimit.Value := g_Config.nDressDCAddValueMaxLimit;
  EditDressDCAddValueRate.Value := g_Config.nDressDCAddValueRate;
  EditDressMCAddRate.Value := g_Config.nDressMCAddRate;
  EditDressMCAddValueMaxLimit.Value := g_Config.nDressMCAddValueMaxLimit;
  EditDressMCAddValueRate.Value := g_Config.nDressMCAddValueRate;
  EditDressSCAddRate.Value := g_Config.nDressSCAddRate;
  EditDressSCAddValueMaxLimit.Value := g_Config.nDressSCAddValueMaxLimit;
  EditDressSCAddValueRate.Value := g_Config.nDressSCAddValueRate;
  EditDressACAddValueMaxLimit.Value := g_Config.nDressACAddValueMaxLimit;
  EditDressACAddValueRate.Value := g_Config.nDressACAddValueRate;
  EditDressACAddRate.Value := g_Config.nDressACAddRate;
  EditDressMACAddValueMaxLimit.Value := g_Config.nDressMACAddValueMaxLimit;
  EditDressMACAddValueRate.Value := g_Config.nDressMACAddValueRate;
  EditDressMACAddRate.Value := g_Config.nDressMACAddRate;

  //////////////////////////////////////////////////////////////////////////////

  seFashionWeaponDCAddValueMaxLimit.Value := g_Config.nFashionWeaponDCAddValueMaxLimit;
  seFashionWeaponDCAddValueRate.Value := g_Config.nFashionWeaponDCAddValueRate;
  seFashionWeaponMCAddValueMaxLimit.Value := g_Config.nFashionWeaponMCAddValueMaxLimit;
  seFashionWeaponMCAddValueRate.Value := g_Config.nFashionWeaponMCAddValueRate;
  seFashionWeaponSCAddValueMaxLimit.Value := g_Config.nFashionWeaponSCAddValueMaxLimit;
  seFashionWeaponSCAddValueRate.Value := g_Config.nFashionWeaponSCAddValueRate;
  seFashionWeaponDCAddRate.Value := g_Config.nFashionWeaponDCAddRate;
  seFashionWeaponSCAddRate.Value := g_Config.nFashionWeaponSCAddRate;
  seFashionWeaponMCAddRate.Value := g_Config.nFashionWeaponMCAddRate;

  seFashionWeaponHitSpeedAddValueMaxLimit.Value := g_Config.nFashionWeaponHitSpeedAddValueMaxLimit;
  seFashionWeaponHitSpeedAddValueRate.Value := g_Config.nFashionWeaponHitSpeedAddValueRate;
  seFashionWeaponHitSpeedAddRate.Value := g_Config.nFashionWeaponHitSpeedAddRate;

  seFashionDressDCAddRate.Value := g_Config.nFashionDressDCAddRate;
  seFashionDressDCAddValueMaxLimit.Value := g_Config.nFashionDressDCAddValueMaxLimit;
  seFashionDressDCAddValueRate.Value := g_Config.nFashionDressDCAddValueRate;
  seFashionDressMCAddRate.Value := g_Config.nFashionDressMCAddRate;
  seFashionDressMCAddValueMaxLimit.Value := g_Config.nFashionDressMCAddValueMaxLimit;
  seFashionDressMCAddValueRate.Value := g_Config.nFashionDressMCAddValueRate;
  seFashionDressSCAddRate.Value := g_Config.nFashionDressSCAddRate;
  seFashionDressSCAddValueMaxLimit.Value := g_Config.nFashionDressSCAddValueMaxLimit;
  seFashionDressSCAddValueRate.Value := g_Config.nFashionDressSCAddValueRate;
  seFashionDressACAddValueMaxLimit.Value := g_Config.nFashionDressACAddValueMaxLimit;
  seFashionDressACAddValueRate.Value := g_Config.nFashionDressACAddValueRate;
  seFashionDressACAddRate.Value := g_Config.nFashionDressACAddRate;
  seFashionDressMACAddValueMaxLimit.Value := g_Config.nFashionDressMACAddValueMaxLimit;
  seFashionDressMACAddValueRate.Value := g_Config.nFashionDressMACAddValueRate;
  seFashionDressMACAddRate.Value := g_Config.nFashionDressMACAddRate;

  seHorseDCAddRate.Value := g_Config.nHorseDCAddRate;
  seHorseDCAddValueMaxLimit.Value := g_Config.nHorseDCAddValueMaxLimit;
  seHorseDCAddValueRate.Value := g_Config.nHorseDCAddValueRate;
  seHorseMCAddRate.Value := g_Config.nHorseMCAddRate;
  seHorseMCAddValueMaxLimit.Value := g_Config.nHorseMCAddValueMaxLimit;
  seHorseMCAddValueRate.Value := g_Config.nHorseMCAddValueRate;
  seHorseSCAddRate.Value := g_Config.nHorseSCAddRate;
  seHorseSCAddValueMaxLimit.Value := g_Config.nHorseSCAddValueMaxLimit;
  seHorseSCAddValueRate.Value := g_Config.nHorseSCAddValueRate;
  seHorseACAddValueMaxLimit.Value := g_Config.nHorseACAddValueMaxLimit;
  seHorseACAddValueRate.Value := g_Config.nHorseACAddValueRate;
  seHorseACAddRate.Value := g_Config.nHorseACAddRate;
  seHorseMACAddValueMaxLimit.Value := g_Config.nHorseMACAddValueMaxLimit;
  seHorseMACAddValueRate.Value := g_Config.nHorseMACAddValueRate;
  seHorseMACAddRate.Value := g_Config.nHorseMACAddRate;

  seDrumDCAddRate.Value := g_Config.nDrumDCAddRate;
  seDrumDCAddValueMaxLimit.Value := g_Config.nDrumDCAddValueMaxLimit;
  seDrumDCAddValueRate.Value := g_Config.nDrumDCAddValueRate;
  seDrumMCAddRate.Value := g_Config.nDrumMCAddRate;
  seDrumMCAddValueMaxLimit.Value := g_Config.nDrumMCAddValueMaxLimit;
  seDrumMCAddValueRate.Value := g_Config.nDrumMCAddValueRate;
  seDrumSCAddRate.Value := g_Config.nDrumSCAddRate;
  seDrumSCAddValueMaxLimit.Value := g_Config.nDrumSCAddValueMaxLimit;
  seDrumSCAddValueRate.Value := g_Config.nDrumSCAddValueRate;
  seDrumACAddValueMaxLimit.Value := g_Config.nDrumACAddValueMaxLimit;
  seDrumACAddValueRate.Value := g_Config.nDrumACAddValueRate;
  seDrumACAddRate.Value := g_Config.nDrumACAddRate;
  seDrumMACAddValueMaxLimit.Value := g_Config.nDrumMACAddValueMaxLimit;
  seDrumMACAddValueRate.Value := g_Config.nDrumMACAddValueRate;
  seDrumMACAddRate.Value := g_Config.nDrumMACAddRate;

  seShieldDCAddRate.Value := g_Config.nShieldDCAddRate;
  seShieldDCAddValueMaxLimit.Value := g_Config.nShieldDCAddValueMaxLimit;
  seShieldDCAddValueRate.Value := g_Config.nShieldDCAddValueRate;
  seShieldMCAddRate.Value := g_Config.nShieldMCAddRate;
  seShieldMCAddValueMaxLimit.Value := g_Config.nShieldMCAddValueMaxLimit;
  seShieldMCAddValueRate.Value := g_Config.nShieldMCAddValueRate;
  seShieldSCAddRate.Value := g_Config.nShieldSCAddRate;
  seShieldSCAddValueMaxLimit.Value := g_Config.nShieldSCAddValueMaxLimit;
  seShieldSCAddValueRate.Value := g_Config.nShieldSCAddValueRate;
  seShieldACAddValueMaxLimit.Value := g_Config.nShieldACAddValueMaxLimit;
  seShieldACAddValueRate.Value := g_Config.nShieldACAddValueRate;
  seShieldACAddRate.Value := g_Config.nShieldACAddRate;
  seShieldMACAddValueMaxLimit.Value := g_Config.nShieldMACAddValueMaxLimit;
  seShieldMACAddValueRate.Value := g_Config.nShieldMACAddValueRate;
  seShieldMACAddRate.Value := g_Config.nShieldMACAddRate;

  //////////////////////////////////////////////////////////////////////////////

  EditNeckLace19DCAddRate.Value := g_Config.nNeckLace19DCAddRate;
  EditNeckLace19DCAddValueMaxLimit.Value := g_Config.nNeckLace19DCAddValueMaxLimit;
  EditNeckLace19DCAddValueRate.Value := g_Config.nNeckLace19DCAddValueRate;
  EditNeckLace19MCAddRate.Value := g_Config.nNeckLace19MCAddRate;
  EditNeckLace19MCAddValueMaxLimit.Value := g_Config.nNeckLace19MCAddValueMaxLimit;
  EditNeckLace19MCAddValueRate.Value := g_Config.nNeckLace19MCAddValueRate;
  EditNeckLace19SCAddRate.Value := g_Config.nNeckLace19SCAddRate;
  EditNeckLace19SCAddValueMaxLimit.Value := g_Config.nNeckLace19SCAddValueMaxLimit;
  EditNeckLace19SCAddValueRate.Value := g_Config.nNeckLace19SCAddValueRate;
  EditNeckLace19ACAddValueMaxLimit.Value := g_Config.nNeckLace19ACAddValueMaxLimit;
  EditNeckLace19ACAddValueRate.Value := g_Config.nNeckLace19ACAddValueRate;
  EditNeckLace19ACAddRate.Value := g_Config.nNeckLace19ACAddRate;
  EditNeckLace19MACAddValueMaxLimit.Value := g_Config.nNeckLace19MACAddValueMaxLimit;
  EditNeckLace19MACAddValueRate.Value := g_Config.nNeckLace19MACAddValueRate;
  EditNeckLace19MACAddRate.Value := g_Config.nNeckLace19MACAddRate;
  EditNeckLace202124DCAddRate.Value := g_Config.nNeckLace202124DCAddRate;
  EditNeckLace202124DCAddValueMaxLimit.Value := g_Config.nNeckLace202124DCAddValueMaxLimit;
  EditNeckLace202124DCAddValueRate.Value := g_Config.nNeckLace202124DCAddValueRate;
  EditNeckLace202124MCAddRate.Value := g_Config.nNeckLace202124MCAddRate;
  EditNeckLace202124MCAddValueMaxLimit.Value := g_Config.nNeckLace202124MCAddValueMaxLimit;
  EditNeckLace202124MCAddValueRate.Value := g_Config.nNeckLace202124MCAddValueRate;
  EditNeckLace202124SCAddRate.Value := g_Config.nNeckLace202124SCAddRate;
  EditNeckLace202124SCAddValueMaxLimit.Value := g_Config.nNeckLace202124SCAddValueMaxLimit;
  EditNeckLace202124SCAddValueRate.Value := g_Config.nNeckLace202124SCAddValueRate;
  EditNeckLace202124ACAddValueMaxLimit.Value := g_Config.nNeckLace202124ACAddValueMaxLimit;
  EditNeckLace202124ACAddValueRate.Value := g_Config.nNeckLace202124ACAddValueRate;
  EditNeckLace202124ACAddRate.Value := g_Config.nNeckLace202124ACAddRate;
  EditNeckLace202124MACAddValueMaxLimit.Value := g_Config.nNeckLace202124MACAddValueMaxLimit;
  EditNeckLace202124MACAddValueRate.Value := g_Config.nNeckLace202124MACAddValueRate;
  EditNeckLace202124MACAddRate.Value := g_Config.nNeckLace202124MACAddRate;
  EditArmRing26DCAddRate.Value := g_Config.nArmRing26DCAddRate;
  EditArmRing26DCAddValueMaxLimit.Value := g_Config.nArmRing26DCAddValueMaxLimit;
  EditArmRing26DCAddValueRate.Value := g_Config.nArmRing26DCAddValueRate;
  EditArmRing26MCAddRate.Value := g_Config.nArmRing26MCAddRate;
  EditArmRing26MCAddValueMaxLimit.Value := g_Config.nArmRing26MCAddValueMaxLimit;
  EditArmRing26MCAddValueRate.Value := g_Config.nArmRing26MCAddValueRate;
  EditArmRing26SCAddRate.Value := g_Config.nArmRing26SCAddRate;
  EditArmRing26SCAddValueMaxLimit.Value := g_Config.nArmRing26SCAddValueMaxLimit;
  EditArmRing26SCAddValueRate.Value := g_Config.nArmRing26SCAddValueRate;
  EditArmRing26ACAddValueMaxLimit.Value := g_Config.nArmRing26ACAddValueMaxLimit;
  EditArmRing26ACAddValueRate.Value := g_Config.nArmRing26ACAddValueRate;
  EditArmRing26ACAddRate.Value := g_Config.nArmRing26ACAddRate;
  EditArmRing26MACAddValueMaxLimit.Value := g_Config.nArmRing26MACAddValueMaxLimit;
  EditArmRing26MACAddValueRate.Value := g_Config.nArmRing26MACAddValueRate;
  EditArmRing26MACAddRate.Value := g_Config.nArmRing26MACAddRate;

  EditRing22DCAddRate.Value := g_Config.nRing22DCAddRate;
  EditRing22DCAddValueMaxLimit.Value := g_Config.nRing22DCAddValueMaxLimit;
  EditRing22DCAddValueRate.Value := g_Config.nRing22DCAddValueRate;
  EditRing22MCAddRate.Value := g_Config.nRing22MCAddRate;
  EditRing22MCAddValueMaxLimit.Value := g_Config.nRing22MCAddValueMaxLimit;
  EditRing22MCAddValueRate.Value := g_Config.nRing22MCAddValueRate;
  EditRing22SCAddRate.Value := g_Config.nRing22SCAddRate;
  EditRing22SCAddValueMaxLimit.Value := g_Config.nRing22SCAddValueMaxLimit;
  EditRing22SCAddValueRate.Value := g_Config.nRing22SCAddValueRate;
  EditRing22ACAddValueMaxLimit.Value := g_Config.nRing22ACAddValueMaxLimit;
  EditRing22ACAddValueRate.Value := g_Config.nRing22ACAddValueRate;
  EditRing22ACAddRate.Value := g_Config.nRing22ACAddRate;
  EditRing22MACAddValueMaxLimit.Value := g_Config.nRing22MACAddValueMaxLimit;
  EditRing22MACAddValueRate.Value := g_Config.nRing22MACAddValueRate;
  EditRing22MACAddRate.Value := g_Config.nRing22MACAddRate;

  EditRing23DCAddRate.Value := g_Config.nRing23DCAddRate;
  EditRing23DCAddValueMaxLimit.Value := g_Config.nRing23DCAddValueMaxLimit;
  EditRing23DCAddValueRate.Value := g_Config.nRing23DCAddValueRate;
  EditRing23MCAddRate.Value := g_Config.nRing23MCAddRate;
  EditRing23MCAddValueMaxLimit.Value := g_Config.nRing23MCAddValueMaxLimit;
  EditRing23MCAddValueRate.Value := g_Config.nRing23MCAddValueRate;
  EditRing23SCAddRate.Value := g_Config.nRing23SCAddRate;
  EditRing23SCAddValueMaxLimit.Value := g_Config.nRing23SCAddValueMaxLimit;
  EditRing23SCAddValueRate.Value := g_Config.nRing23SCAddValueRate;
  EditRing23ACAddValueMaxLimit.Value := g_Config.nRing23ACAddValueMaxLimit;
  EditRing23ACAddValueRate.Value := g_Config.nRing23ACAddValueRate;
  EditRing23ACAddRate.Value := g_Config.nRing23ACAddRate;
  EditRing23MACAddValueMaxLimit.Value := g_Config.nRing23MACAddValueMaxLimit;
  EditRing23MACAddValueRate.Value := g_Config.nRing23MACAddValueRate;
  EditRing23MACAddRate.Value := g_Config.nRing23MACAddRate;

  EditBootsDCAddValueMaxLimit.Value := g_Config.nBootsDCAddValueMaxLimit;                           // 20080503 极品鞋子加攻最高点
  EditBootsDCAddValueRate.Value := g_Config.nBootsDCAddValueRate;
  EditBootsDCAddRate.Value := g_Config.nBootsDCAddRate;
  // 道术
  EditBootsSCAddValueMaxLimit.Value := g_Config.nBootsSCAddValueMaxLimit;
  EditBootsSCAddValueRate.Value := g_Config.nBootsSCAddValueRate;
  EditBootsSCAddRate.Value := g_Config.nBootsSCAddRate;
  // 魔法
  EditBootsMCAddValueMaxLimit.Value := g_Config.nBootsMCAddValueMaxLimit;
  EditBootsMCAddValueRate.Value := g_Config.nBootsMCAddValueRate;
  EditBootsMCAddRate.Value := g_Config.nBootsMCAddRate;
  // 防御
  EditBootsACAddValueMaxLimit.Value := g_Config.nBootsACAddValueMaxLimit;
  EditBootsACAddValueRate.Value := g_Config.nBootsACAddValueRate;
  EditBootsACAddRate.Value := g_Config.nBootsACAddRate;
  // 魔御
  EditBootsMACAddValueMaxLimit.Value := g_Config.nBootsMACAddValueMaxLimit;
  EditBootsMACAddValueRate.Value := g_Config.nBootsMACAddValueRate;
  EditBootsMACAddRate.Value := g_Config.nBootsMACAddRate;

  EditHelMetDCAddRate.Value := g_Config.nHelMetDCAddRate;
  EditHelMetDCAddValueMaxLimit.Value := g_Config.nHelMetDCAddValueMaxLimit;
  EditHelMetDCAddValueRate.Value := g_Config.nHelMetDCAddValueRate;
  EditHelMetMCAddRate.Value := g_Config.nHelMetMCAddRate;
  EditHelMetMCAddValueMaxLimit.Value := g_Config.nHelMetMCAddValueMaxLimit;
  EditHelMetMCAddValueRate.Value := g_Config.nHelMetMCAddValueRate;
  EditHelMetSCAddRate.Value := g_Config.nHelMetSCAddRate;
  EditHelMetSCAddValueMaxLimit.Value := g_Config.nHelMetSCAddValueMaxLimit;
  EditHelMetSCAddValueRate.Value := g_Config.nHelMetSCAddValueRate;
  EditHelMetACAddValueMaxLimit.Value := g_Config.nHelMetACAddValueMaxLimit;
  EditHelMetACAddValueRate.Value := g_Config.nHelMetACAddValueRate;
  EditHelMetACAddRate.Value := g_Config.nHelMetACAddRate;
  EditHelMetMACAddValueMaxLimit.Value := g_Config.nHelMetMACAddValueMaxLimit;
  EditHelMetMACAddValueRate.Value := g_Config.nHelMetMACAddValueRate;
  EditHelMetMACAddRate.Value := g_Config.nHelMetMACAddRate;

  EditGuildRecallTime.Value := g_Config.nGuildRecallTime;

  seCritAttackHurtRate.Value := g_Config.dwCritAttackHurtRate;
  seDamageReboundRate.Value := g_Config.dwDamageReboundRate;

  seFatalBlowBasePower.Value := g_Config.dwFatalBlowBasePower;
  seFatalBlowNeedPower1.Value := g_Config.dwFatalBlowNeedPower1;
  seFatalBlowNeedPower2.Value := g_Config.dwFatalBlowNeedPower2;
  seFatalBlowNeedPower3.Value := g_Config.dwFatalBlowNeedPower3;

  RefUnknowItem();
  RefShapeItem();
  RefItemNewAbil();
  boOpened := True;
  PageControl.ActivePageIndex := 0;
  AddValuePageControl.ActivePageIndex := 0;
  ItemSetPageControl.ActivePageIndex := 0;
  ShowModal;
end;

procedure TfrmItemSet.ButtonItemSetSaveClick(Sender: TObject);
begin
  Config.WriteBool('Setup', 'OpenItemFlute', g_Config.boOpenItemFlute);
  Config.WriteBool('Setup', 'DisableRightClickFluteStone', g_Config.boDisableRightClickFluteStone);
  Config.WriteInteger('Setup', 'ItemFluteStoneCount', g_Config.nItemFluteStoneCount);
  Config.WriteInteger('Setup', 'ItemFluteStoneIdxCount', g_Config.nItemFluteStoneIdxCount);
  Config.WriteInteger('Setup', 'ItemFluteStoneOverlapCount', g_Config.nItemFluteStoneOverlapCount);

  Config.WriteInteger('Setup', 'ItemPowerRate', g_Config.nItemPowerRate);
  Config.WriteInteger('Setup', 'ItemExpRate', g_Config.nItemExpRate);
  Config.WriteInteger('Setup', 'GuildRecallTime', g_Config.nGuildRecallTime);
  //Config.WriteInteger('Setup', 'GroupRecallTime', g_Config.nGroupRecallTime);
  Config.WriteInteger('Setup', 'GroupRecallTime', g_Config.nAttackPosionRate);
  Config.WriteInteger('Setup', 'AttackPosionRate', g_Config.nAttackPosionRate);
  Config.WriteInteger('Setup', 'AttackPosionTime', g_Config.nAttackPosionTime);
  Config.WriteBool('Setup', 'UserMoveCanDupObj', g_Config.boUserMoveCanDupObj);
  Config.WriteBool('Setup', 'UserMoveCanOnItem', g_Config.boUserMoveCanOnItem);
  Config.WriteInteger('Setup', 'UserMoveTime', g_Config.dwUserMoveTime);

  Config.WriteInteger('Setup', 'MDParalysisRate', g_Config.dwMDParalysisRate);
  Config.WriteInteger('Setup', 'MDParalysisTime', g_Config.dwMDParalysisTime);
  Config.WriteInteger('Setup', 'FrozenRate', g_Config.dwFrozenRate);
  Config.WriteInteger('Setup', 'FrozenTime', g_Config.dwFrozenTime);
  Config.WriteBool('Setup', 'FrozenUseMagicStruck', g_Config.boFrozenUseMagicStruck);

  Config.WriteInteger('Setup', 'CobwebWindingRate', g_Config.dwCobwebWindingRate);
  Config.WriteInteger('Setup', 'CobwebWindingTime', g_Config.dwCobwebWindingTime);
  Config.WriteBool('Setup', 'CobwebWindingUseMagicStruck', g_Config.boCobwebWindingUseMagicStruck);

  uModValue();

  if boSendServerConfig then
  begin
    UserEngine.SendServerConfig();
    boSendServerConfig := False;
  end;
end;

procedure TfrmItemSet.EditItemExpRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemExpRate := EditItemExpRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditItemPowerRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemPowerRate := EditItemPowerRate.Value;
  ModValue();
end;

procedure TfrmItemSet.ButtonAddValueSaveClick(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'MonRandomAddValue', g_Config.nMonRandomAddValue);
  Config.WriteInteger('Setup', 'MakeRandomAddValue', g_Config.nMakeRandomAddValue);

  Config.WriteInteger('Setup', 'WeaponDCAddValueMaxLimit', g_Config.nWeaponDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'WeaponDCAddValueRate', g_Config.nWeaponDCAddValueRate);
  Config.WriteInteger('Setup', 'WeaponMCAddValueMaxLimit', g_Config.nWeaponMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'WeaponMCAddValueRate', g_Config.nWeaponMCAddValueRate);
  Config.WriteInteger('Setup', 'WeaponSCAddValueMaxLimit', g_Config.nWeaponSCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'WeaponSCAddValueRate', g_Config.nWeaponSCAddValueRate);
  Config.WriteInteger('Setup', 'WeaponDCAddRate', g_Config.nWeaponDCAddRate);
  Config.WriteInteger('Setup', 'WeaponSCAddRate', g_Config.nWeaponSCAddRate);
  Config.WriteInteger('Setup', 'WeaponMCAddRate', g_Config.nWeaponMCAddRate);
  Config.WriteInteger('Setup', 'WeaponHitSpeedAddValueMaxLimit', g_Config.nWeaponHitSpeedAddValueMaxLimit);
  Config.WriteInteger('Setup', 'WeaponHitSpeedAddValueRate', g_Config.nWeaponHitSpeedAddValueRate);
  Config.WriteInteger('Setup', 'WeaponHitSpeedAddRate', g_Config.nWeaponHitSpeedAddRate);

  Config.WriteInteger('Setup', 'DressDCAddRate', g_Config.nDressDCAddRate);
  Config.WriteInteger('Setup', 'DressDCAddValueMaxLimit', g_Config.nDressDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DressDCAddValueRate', g_Config.nDressDCAddValueRate);
  Config.WriteInteger('Setup', 'DressMCAddRate', g_Config.nDressMCAddRate);
  Config.WriteInteger('Setup', 'DressMCAddValueMaxLimit', g_Config.nDressMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DressMCAddValueRate', g_Config.nDressMCAddValueRate);
  Config.WriteInteger('Setup', 'DressSCAddRate', g_Config.nDressSCAddRate);
  Config.WriteInteger('Setup', 'DressSCAddValueMaxLimit', g_Config.nDressSCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DressSCAddValueRate', g_Config.nDressSCAddValueRate);
  Config.WriteInteger('Setup', 'DressACAddValueMaxLimit', g_Config.nDressACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DressACAddValueRate', g_Config.nDressACAddValueRate);
  Config.WriteInteger('Setup', 'DressACAddRate', g_Config.nDressACAddRate);
  Config.WriteInteger('Setup', 'DressMACAddValueMaxLimit', g_Config.nDressMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DressMACAddValueRate', g_Config.nDressMACAddValueRate);
  Config.WriteInteger('Setup', 'DressMACAddRate', g_Config.nDressMACAddRate);

  Config.WriteInteger('Setup', 'NeckLace19DCAddRate', g_Config.nNeckLace19DCAddRate);
  Config.WriteInteger('Setup', 'NeckLace19DCAddValueMaxLimit', g_Config.nNeckLace19DCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace19DCAddValueRate', g_Config.nNeckLace19DCAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace19MCAddRate', g_Config.nNeckLace19MCAddRate);
  Config.WriteInteger('Setup', 'NeckLace19MCAddValueMaxLimit', g_Config.nNeckLace19MCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace19MCAddValueRate', g_Config.nNeckLace19MCAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace19SCAddRate', g_Config.nNeckLace19SCAddRate);
  Config.WriteInteger('Setup', 'NeckLace19SCAddValueMaxLimit', g_Config.nNeckLace19SCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace19SCAddValueRate', g_Config.nNeckLace19SCAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace19ACAddValueMaxLimit', g_Config.nNeckLace19ACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace19ACAddValueRate', g_Config.nNeckLace19ACAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace19ACAddRate', g_Config.nNeckLace19ACAddRate);
  Config.WriteInteger('Setup', 'NeckLace19MACAddValueMaxLimit', g_Config.nNeckLace19MACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace19MACAddValueRate', g_Config.nNeckLace19MACAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace19MACAddRate', g_Config.nNeckLace19MACAddRate);

  Config.WriteInteger('Setup', 'NeckLace202124DCAddRate', g_Config.nNeckLace202124DCAddRate);
  Config.WriteInteger('Setup', 'NeckLace202124DCAddValueMaxLimit', g_Config.nNeckLace202124DCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace202124DCAddValueRate', g_Config.nNeckLace202124DCAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace202124MCAddRate', g_Config.nNeckLace202124MCAddRate);
  Config.WriteInteger('Setup', 'NeckLace202124MCAddValueMaxLimit', g_Config.nNeckLace202124MCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace202124MCAddValueRate', g_Config.nNeckLace202124MCAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace202124SCAddRate', g_Config.nNeckLace202124SCAddRate);
  Config.WriteInteger('Setup', 'NeckLace202124SCAddValueMaxLimit', g_Config.nNeckLace202124SCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace202124SCAddValueRate', g_Config.nNeckLace202124SCAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace202124ACAddValueMaxLimit', g_Config.nNeckLace202124ACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace202124ACAddValueRate', g_Config.nNeckLace202124ACAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace202124ACAddRate', g_Config.nNeckLace202124ACAddRate);
  Config.WriteInteger('Setup', 'NeckLace202124MACAddValueMaxLimit', g_Config.nNeckLace202124MACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'NeckLace202124MACAddValueRate', g_Config.nNeckLace202124MACAddValueRate);
  Config.WriteInteger('Setup', 'NeckLace202124MACAddRate', g_Config.nNeckLace202124MACAddRate);

  Config.WriteInteger('Setup', 'Ring22DCAddValueMaxLimit', g_Config.nRing22DCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring22DCAddValueRate', g_Config.nRing22DCAddValueRate);
  Config.WriteInteger('Setup', 'Ring22DCAddRate', g_Config.nRing22DCAddRate);
  Config.WriteInteger('Setup', 'Ring22MCAddValueMaxLimit', g_Config.nRing22MCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring22MCAddValueRate', g_Config.nRing22MCAddValueRate);
  Config.WriteInteger('Setup', 'Ring22MCAddRate', g_Config.nRing22MCAddRate);
  Config.WriteInteger('Setup', 'Ring22SCAddValueMaxLimit', g_Config.nRing22SCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring22SCAddValueRate', g_Config.nRing22SCAddValueRate);
  Config.WriteInteger('Setup', 'Ring22SCAddRate', g_Config.nRing22SCAddRate);
  Config.WriteInteger('Setup', 'Ring22ACAddValueMaxLimit', g_Config.nRing22ACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring22ACAddValueRate', g_Config.nRing22ACAddValueRate);
  Config.WriteInteger('Setup', 'Ring22ACAddRate', g_Config.nRing22ACAddRate);
  Config.WriteInteger('Setup', 'Ring22MACAddValueMaxLimit', g_Config.nRing22MACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring22MACAddValueRate', g_Config.nRing22MACAddValueRate);
  Config.WriteInteger('Setup', 'Ring22MACAddRate', g_Config.nRing22MACAddRate);

  Config.WriteInteger('Setup', 'ArmRing26DCAddRate', g_Config.nArmRing26DCAddRate);
  Config.WriteInteger('Setup', 'ArmRing26DCAddValueMaxLimit', g_Config.nArmRing26DCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ArmRing26DCAddValueRate', g_Config.nArmRing26DCAddValueRate);
  Config.WriteInteger('Setup', 'ArmRing26MCAddRate', g_Config.nArmRing26MCAddRate);
  Config.WriteInteger('Setup', 'ArmRing26MCAddValueMaxLimit', g_Config.nArmRing26MCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ArmRing26MCAddValueRate', g_Config.nArmRing26MCAddValueRate);
  Config.WriteInteger('Setup', 'ArmRing26SCAddRate', g_Config.nArmRing26SCAddRate);
  Config.WriteInteger('Setup', 'ArmRing26SCAddValueMaxLimit', g_Config.nArmRing26SCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ArmRing26SCAddValueRate', g_Config.nArmRing26SCAddValueRate);
  Config.WriteInteger('Setup', 'ArmRing26ACAddValueMaxLimit', g_Config.nArmRing26ACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ArmRing26ACAddValueRate', g_Config.nArmRing26ACAddValueRate);
  Config.WriteInteger('Setup', 'ArmRing26ACAddRate', g_Config.nArmRing26ACAddRate);
  Config.WriteInteger('Setup', 'ArmRing26MACAddValueMaxLimit', g_Config.nArmRing26MACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ArmRing26MACAddValueRate', g_Config.nArmRing26MACAddValueRate);
  Config.WriteInteger('Setup', 'ArmRing26MACAddRate', g_Config.nArmRing26MACAddRate);

  Config.WriteInteger('Setup', 'Ring23DCAddRate', g_Config.nRing23DCAddRate);
  Config.WriteInteger('Setup', 'Ring23DCAddValueMaxLimit', g_Config.nRing23DCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring23DCAddValueRate', g_Config.nRing23DCAddValueRate);
  Config.WriteInteger('Setup', 'Ring23MCAddRate', g_Config.nRing23MCAddRate);
  Config.WriteInteger('Setup', 'Ring23MCAddValueMaxLimit', g_Config.nRing23MCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring23MCAddValueRate', g_Config.nRing23MCAddValueRate);
  Config.WriteInteger('Setup', 'Ring23SCAddRate', g_Config.nRing23SCAddRate);
  Config.WriteInteger('Setup', 'Ring23SCAddValueMaxLimit', g_Config.nRing23SCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring23SCAddValueRate', g_Config.nRing23SCAddValueRate);
  Config.WriteInteger('Setup', 'Ring23ACAddValueMaxLimit', g_Config.nRing23ACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring23ACAddValueRate', g_Config.nRing23ACAddValueRate);
  Config.WriteInteger('Setup', 'Ring23ACAddRate', g_Config.nRing23ACAddRate);
  Config.WriteInteger('Setup', 'Ring23MACAddValueMaxLimit', g_Config.nRing23MACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'Ring23MACAddValueRate', g_Config.nRing23MACAddValueRate);
  Config.WriteInteger('Setup', 'Ring23MACAddRate', g_Config.nRing23MACAddRate);

  Config.WriteInteger('Setup', 'BootsDCAddValueMaxLimit', g_Config.nBootsDCAddValueMaxLimit);       // 20080503 极品鞋子加攻最高点
  Config.WriteInteger('Setup', 'BootsDCAddValueRate', g_Config.nBootsDCAddValueRate);
  Config.WriteInteger('Setup', 'BootsDCAddRate', g_Config.nBootsDCAddRate);
  // 极品鞋子加道术
  Config.WriteInteger('Setup', 'BootsSCAddValueMaxLimit', g_Config.nBootsSCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'BootsSCAddValueRate', g_Config.nBootsSCAddValueRate);
  Config.WriteInteger('Setup', 'BootsSCAddRate', g_Config.nBootsSCAddRate);
  // 极品鞋子加魔法
  Config.WriteInteger('Setup', 'BootsMCAddValueMaxLimit', g_Config.nBootsMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'BootsMCAddValueRate', g_Config.nBootsMCAddValueRate);
  Config.WriteInteger('Setup', 'BootsMCAddRate', g_Config.nBootsMCAddRate);
  // 极品鞋子加防御
  Config.WriteInteger('Setup', 'BootsACAddValueMaxLimit', g_Config.nBootsACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'BootsACAddValueRate', g_Config.nBootsACAddValueRate);
  Config.WriteInteger('Setup', 'BootsACAddRate', g_Config.nBootsACAddRate);
  // 极品鞋子加魔御
  Config.WriteInteger('Setup', 'BootsMACAddValueMaxLimit', g_Config.nBootsMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'BootsMACAddValueRate', g_Config.nBootsMACAddValueRate);
  Config.WriteInteger('Setup', 'BootsMACAddRate', g_Config.nBootsMACAddRate);

  Config.WriteInteger('Setup', 'HelMetDCAddRate', g_Config.nHelMetDCAddRate);
  Config.WriteInteger('Setup', 'HelMetDCAddValueMaxLimit', g_Config.nHelMetDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HelMetDCAddValueRate', g_Config.nHelMetDCAddValueRate);
  Config.WriteInteger('Setup', 'HelMetMCAddRate', g_Config.nHelMetMCAddRate);
  Config.WriteInteger('Setup', 'HelMetMCAddValueMaxLimit', g_Config.nHelMetMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HelMetMCAddValueRate', g_Config.nHelMetMCAddValueRate);
  Config.WriteInteger('Setup', 'HelMetSCAddRate', g_Config.nHelMetSCAddRate);
  Config.WriteInteger('Setup', 'HelMetSCAddValueMaxLimit', g_Config.nHelMetSCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HelMetSCAddValueRate', g_Config.nHelMetSCAddValueRate);
  Config.WriteInteger('Setup', 'HelMetACAddValueMaxLimit', g_Config.nHelMetACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HelMetACAddValueRate', g_Config.nHelMetACAddValueRate);
  Config.WriteInteger('Setup', 'HelMetACAddRate', g_Config.nHelMetACAddRate);
  Config.WriteInteger('Setup', 'HelMetMACAddValueMaxLimit', g_Config.nHelMetMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HelMetMACAddValueRate', g_Config.nHelMetMACAddValueRate);
  Config.WriteInteger('Setup', 'HelMetMACAddRate', g_Config.nHelMetMACAddRate);

  // 时装武器 piaoyun 2013-10-27
  Config.WriteInteger('Setup', 'FashionWeaponDCAddValueMaxLimit', g_Config.nFashionWeaponDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'FashionWeaponDCAddValueRate', g_Config.nFashionWeaponDCAddValueRate);
  Config.WriteInteger('Setup', 'FashionWeaponMCAddValueMaxLimit', g_Config.nFashionWeaponMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'FashionWeaponMCAddValueRate', g_Config.nFashionWeaponMCAddValueRate);
  Config.WriteInteger('Setup', 'FashionWeaponSCAddValueMaxLimit', g_Config.nFashionWeaponSCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'FashionWeaponSCAddValueRate', g_Config.nFashionWeaponSCAddValueRate);
  Config.WriteInteger('Setup', 'FashionWeaponDCAddRate', g_Config.nFashionWeaponDCAddRate);
  Config.WriteInteger('Setup', 'FashionWeaponSCAddRate', g_Config.nFashionWeaponSCAddRate);
  Config.WriteInteger('Setup', 'FashionWeaponMCAddRate', g_Config.nFashionWeaponMCAddRate);
  Config.WriteInteger('Setup', 'FashionWeaponHitSpeedAddValueMaxLimit', g_Config.nFashionWeaponHitSpeedAddValueMaxLimit);
  Config.WriteInteger('Setup', 'FashionWeaponHitSpeedAddValueRate', g_Config.nFashionWeaponHitSpeedAddValueRate);
  Config.WriteInteger('Setup', 'FashionWeaponHitSpeedAddRate', g_Config.nFashionWeaponHitSpeedAddRate);

  // 时装衣服 piaoyun 2013-10-27
  Config.WriteInteger('Setup', 'FashionDressDCAddRate', g_Config.nFashionDressDCAddRate);
  Config.WriteInteger('Setup', 'FashionDressDCAddValueMaxLimit', g_Config.nFashionDressDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'FashionDressDCAddValueRate', g_Config.nFashionDressDCAddValueRate);
  Config.WriteInteger('Setup', 'FashionDressMCAddRate', g_Config.nFashionDressMCAddRate);
  Config.WriteInteger('Setup', 'FashionDressMCAddValueMaxLimit', g_Config.nFashionDressMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'FashionDressMCAddValueRate', g_Config.nFashionDressMCAddValueRate);
  Config.WriteInteger('Setup', 'FashionDressSCAddRate', g_Config.nFashionDressSCAddRate);
  Config.WriteInteger('Setup', 'FashionDressSCAddValueMaxLimit', g_Config.nFashionDressSCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'FashionDressSCAddValueRate', g_Config.nFashionDressSCAddValueRate);
  Config.WriteInteger('Setup', 'FashionDressACAddValueMaxLimit', g_Config.nFashionDressACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'FashionDressACAddValueRate', g_Config.nFashionDressACAddValueRate);
  Config.WriteInteger('Setup', 'FashionDressACAddRate', g_Config.nFashionDressACAddRate);
  Config.WriteInteger('Setup', 'FashionDressMACAddValueMaxLimit', g_Config.nFashionDressMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'FashionDressMACAddValueRate', g_Config.nFashionDressMACAddValueRate);
  Config.WriteInteger('Setup', 'FashionDressMACAddRate', g_Config.nFashionDressMACAddRate);

  // 马牌 piaoyun 2013-10-27
  Config.WriteInteger('Setup', 'HorseDCAddRate', g_Config.nHorseDCAddRate);
  Config.WriteInteger('Setup', 'HorseDCAddValueMaxLimit', g_Config.nHorseDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HorseDCAddValueRate', g_Config.nHorseDCAddValueRate);
  Config.WriteInteger('Setup', 'HorseMCAddRate', g_Config.nHorseMCAddRate);
  Config.WriteInteger('Setup', 'HorseMCAddValueMaxLimit', g_Config.nHorseMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HorseMCAddValueRate', g_Config.nHorseMCAddValueRate);
  Config.WriteInteger('Setup', 'HorseSCAddRate', g_Config.nHorseSCAddRate);
  Config.WriteInteger('Setup', 'HorseSCAddValueMaxLimit', g_Config.nHorseSCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HorseSCAddValueRate', g_Config.nHorseSCAddValueRate);
  Config.WriteInteger('Setup', 'HorseACAddValueMaxLimit', g_Config.nHorseACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HorseACAddValueRate', g_Config.nHorseACAddValueRate);
  Config.WriteInteger('Setup', 'HorseACAddRate', g_Config.nHorseACAddRate);
  Config.WriteInteger('Setup', 'HorseMACAddValueMaxLimit', g_Config.nHorseMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'HorseMACAddValueRate', g_Config.nHorseMACAddValueRate);
  Config.WriteInteger('Setup', 'HorseMACAddRate', g_Config.nHorseMACAddRate);

  // 军鼓 piaoyun 2013-10-27
  Config.WriteInteger('Setup', 'DrumDCAddRate', g_Config.nDrumDCAddRate);
  Config.WriteInteger('Setup', 'DrumDCAddValueMaxLimit', g_Config.nDrumDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DrumDCAddValueRate', g_Config.nDrumDCAddValueRate);
  Config.WriteInteger('Setup', 'DrumMCAddRate', g_Config.nDrumMCAddRate);
  Config.WriteInteger('Setup', 'DrumMCAddValueMaxLimit', g_Config.nDrumMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DrumMCAddValueRate', g_Config.nDrumMCAddValueRate);
  Config.WriteInteger('Setup', 'DrumSCAddRate', g_Config.nDrumSCAddRate);
  Config.WriteInteger('Setup', 'DrumSCAddValueMaxLimit', g_Config.nDrumSCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DrumSCAddValueRate', g_Config.nDrumSCAddValueRate);
  Config.WriteInteger('Setup', 'DrumACAddValueMaxLimit', g_Config.nDrumACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DrumACAddValueRate', g_Config.nDrumACAddValueRate);
  Config.WriteInteger('Setup', 'DrumACAddRate', g_Config.nDrumACAddRate);
  Config.WriteInteger('Setup', 'DrumMACAddValueMaxLimit', g_Config.nDrumMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'DrumMACAddValueRate', g_Config.nDrumMACAddValueRate);
  Config.WriteInteger('Setup', 'DrumMACAddRate', g_Config.nDrumMACAddRate);

  // 盾牌 piaoyun 2013-10-27
  Config.WriteInteger('Setup', 'ShieldDCAddRate', g_Config.nShieldDCAddRate);
  Config.WriteInteger('Setup', 'ShieldDCAddValueMaxLimit', g_Config.nShieldDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ShieldDCAddValueRate', g_Config.nShieldDCAddValueRate);
  Config.WriteInteger('Setup', 'ShieldMCAddRate', g_Config.nShieldMCAddRate);
  Config.WriteInteger('Setup', 'ShieldMCAddValueMaxLimit', g_Config.nShieldMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ShieldMCAddValueRate', g_Config.nShieldMCAddValueRate);
  Config.WriteInteger('Setup', 'ShieldSCAddRate', g_Config.nShieldSCAddRate);
  Config.WriteInteger('Setup', 'ShieldSCAddValueMaxLimit', g_Config.nShieldSCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ShieldSCAddValueRate', g_Config.nShieldSCAddValueRate);
  Config.WriteInteger('Setup', 'ShieldACAddValueMaxLimit', g_Config.nShieldACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ShieldACAddValueRate', g_Config.nShieldACAddValueRate);
  Config.WriteInteger('Setup', 'ShieldACAddRate', g_Config.nShieldACAddRate);
  Config.WriteInteger('Setup', 'ShieldMACAddValueMaxLimit', g_Config.nShieldMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'ShieldMACAddValueRate', g_Config.nShieldMACAddValueRate);
  Config.WriteInteger('Setup', 'ShieldMACAddRate', g_Config.nShieldMACAddRate);

  uModValue();
end;

procedure TfrmItemSet.EditMonRandomAddValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMonRandomAddValue := EditMonRandomAddValue.Value;
  ModValue();
end;

procedure TfrmItemSet.EditMakeRandomAddValueChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nMakeRandomAddValue := EditMakeRandomAddValue.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponDCAddValueMaxLimit := EditWeaponDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponDCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponDCAddValueRate := EditWeaponDCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponMCAddValueMaxLimit := EditWeaponMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponMCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponMCAddValueRate := EditWeaponMCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponSCAddValueMaxLimit := EditWeaponSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponSCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponSCAddValueRate := EditWeaponSCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressDCAddValueMaxLimit := EditDressDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressDCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressDCAddValueRate := EditDressDCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressMCAddValueMaxLimit := EditDressMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressMCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressMCAddValueRate := EditDressMCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressSCAddValueMaxLimit := EditDressSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressSCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressSCAddValueRate := EditDressSCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressDCAddRate := EditDressDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressMCAddRate := EditDressMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressSCAddRate := EditDressSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19DCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19DCAddValueMaxLimit := EditNeckLace19DCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19DCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19DCAddValueRate := EditNeckLace19DCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19MCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19MCAddValueMaxLimit := EditNeckLace19MCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19MCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19MCAddValueRate := EditNeckLace19MCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19SCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19SCAddValueMaxLimit := EditNeckLace19SCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19SCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19SCAddValueRate := EditNeckLace19SCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19DCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19DCAddRate := EditNeckLace19DCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19MCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19MCAddRate := EditNeckLace19MCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19SCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19SCAddRate := EditNeckLace19SCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124DCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124DCAddValueMaxLimit := EditNeckLace202124DCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124DCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124DCAddValueRate := EditNeckLace202124DCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124MCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124MCAddValueMaxLimit := EditNeckLace202124MCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124MCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124MCAddValueRate := EditNeckLace202124MCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124SCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124SCAddValueMaxLimit := EditNeckLace202124SCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124SCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124SCAddValueRate := EditNeckLace202124SCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124DCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124DCAddRate := EditNeckLace202124DCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124MCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124MCAddRate := EditNeckLace202124MCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124SCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124SCAddRate := EditNeckLace202124SCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26DCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26DCAddValueMaxLimit := EditArmRing26DCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26DCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26DCAddValueRate := EditArmRing26DCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26MCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26MCAddValueMaxLimit := EditArmRing26MCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26MCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26MCAddValueRate := EditArmRing26MCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26SCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26SCAddValueMaxLimit := EditArmRing26SCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26SCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26SCAddValueRate := EditArmRing26SCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26DCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26DCAddRate := EditArmRing26DCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26MCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26MCAddRate := EditArmRing26MCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26SCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26SCAddRate := EditArmRing26SCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22DCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22DCAddValueMaxLimit := EditRing22DCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22DCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22DCAddValueRate := EditRing22DCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22MCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22MCAddValueMaxLimit := EditRing22MCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22MCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22MCAddValueRate := EditRing22MCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22SCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22SCAddValueMaxLimit := EditRing22SCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22SCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22SCAddValueRate := EditRing22SCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22DCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22DCAddRate := EditRing22DCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22MCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22MCAddRate := EditRing22MCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22SCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22SCAddRate := EditRing22SCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23DCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23DCAddValueMaxLimit := EditRing23DCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23DCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23DCAddValueRate := EditRing23DCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23MCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23MCAddValueMaxLimit := EditRing23MCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23MCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23MCAddValueRate := EditRing23MCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23SCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23SCAddValueMaxLimit := EditRing23SCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23SCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23SCAddValueRate := EditRing23SCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23DCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23DCAddRate := EditRing23DCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23MCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23MCAddRate := EditRing23MCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23SCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23SCAddRate := EditRing23SCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetDCAddValueMaxLimit := EditHelMetDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetDCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetDCAddValueRate := EditHelMetDCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetMCAddValueMaxLimit := EditHelMetMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetMCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetMCAddValueRate := EditHelMetMCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetSCAddValueMaxLimit := EditHelMetSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetSCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetSCAddValueRate := EditHelMetSCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetDCAddRate := EditHelMetDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetMCAddRate := EditHelMetMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetSCAddRate := EditHelMetSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditGuildRecallTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nGuildRecallTime := EditGuildRecallTime.Value;
  ModValue();
end;

procedure TfrmItemSet.ButtonUnKnowItemSaveClick(Sender: TObject);
begin
  Config.WriteInteger('Setup', 'UnknowRingACAddRate', g_Config.nUnknowRingACAddRate);
  Config.WriteInteger('Setup', 'UnknowRingACAddValueMaxLimit', g_Config.nUnknowRingACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowRingMACAddRate', g_Config.nUnknowRingMACAddRate);
  Config.WriteInteger('Setup', 'UnknowRingMACAddValueMaxLimit', g_Config.nUnknowRingMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowRingDCAddRate', g_Config.nUnknowRingDCAddRate);
  Config.WriteInteger('Setup', 'UnknowRingDCAddValueMaxLimit', g_Config.nUnknowRingDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowRingMCAddRate', g_Config.nUnknowRingMCAddRate);
  Config.WriteInteger('Setup', 'UnknowRingMCAddValueMaxLimit', g_Config.nUnknowRingMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowRingSCAddRate', g_Config.nUnknowRingSCAddRate);
  Config.WriteInteger('Setup', 'UnknowRingSCAddValueMaxLimit', g_Config.nUnknowRingSCAddValueMaxLimit);

  Config.WriteInteger('Setup', 'UnknowNecklaceACAddRate', g_Config.nUnknowNecklaceACAddRate);
  Config.WriteInteger('Setup', 'UnknowNecklaceACAddValueMaxLimit', g_Config.nUnknowNecklaceACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowNecklaceMACAddRate', g_Config.nUnknowNecklaceMACAddRate);
  Config.WriteInteger('Setup', 'UnknowNecklaceMACAddValueMaxLimit', g_Config.nUnknowNecklaceMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowNecklaceDCAddRate', g_Config.nUnknowNecklaceDCAddRate);
  Config.WriteInteger('Setup', 'UnknowNecklaceDCAddValueMaxLimit', g_Config.nUnknowNecklaceDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowNecklaceMCAddRate', g_Config.nUnknowNecklaceMCAddRate);
  Config.WriteInteger('Setup', 'UnknowNecklaceMCAddValueMaxLimit', g_Config.nUnknowNecklaceMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowNecklaceSCAddRate', g_Config.nUnknowNecklaceSCAddRate);
  Config.WriteInteger('Setup', 'UnknowNecklaceSCAddValueMaxLimit', g_Config.nUnknowNecklaceSCAddValueMaxLimit);

  Config.WriteInteger('Setup', 'UnknowHelMetACAddRate', g_Config.nUnknowHelMetACAddRate);
  Config.WriteInteger('Setup', 'UnknowHelMetACAddValueMaxLimit', g_Config.nUnknowHelMetACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowHelMetMACAddRate', g_Config.nUnknowHelMetMACAddRate);
  Config.WriteInteger('Setup', 'UnknowHelMetMACAddValueMaxLimit', g_Config.nUnknowHelMetMACAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowHelMetDCAddRate', g_Config.nUnknowHelMetDCAddRate);
  Config.WriteInteger('Setup', 'UnknowHelMetDCAddValueMaxLimit', g_Config.nUnknowHelMetDCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowHelMetMCAddRate', g_Config.nUnknowHelMetMCAddRate);
  Config.WriteInteger('Setup', 'UnknowHelMetMCAddValueMaxLimit', g_Config.nUnknowHelMetMCAddValueMaxLimit);
  Config.WriteInteger('Setup', 'UnknowHelMetSCAddRate', g_Config.nUnknowHelMetSCAddRate);
  Config.WriteInteger('Setup', 'UnknowHelMetSCAddValueMaxLimit', g_Config.nUnknowHelMetSCAddValueMaxLimit);
  uModValue();
end;

procedure TfrmItemSet.RefUnknowItem;
begin
  EditUnknowRingDCAddValueMaxLimit.Value := g_Config.nUnknowRingDCAddValueMaxLimit;
  EditUnknowRingDCAddRate.Value := g_Config.nUnknowRingDCAddRate;
  EditUnknowRingMCAddValueMaxLimit.Value := g_Config.nUnknowRingMCAddValueMaxLimit;
  EditUnknowRingMCAddRate.Value := g_Config.nUnknowRingMCAddRate;
  EditUnknowRingSCAddValueMaxLimit.Value := g_Config.nUnknowRingSCAddValueMaxLimit;
  EditUnknowRingSCAddRate.Value := g_Config.nUnknowRingSCAddRate;
  EditUnknowRingACAddValueMaxLimit.Value := g_Config.nUnknowRingACAddValueMaxLimit;
  EditUnknowRingACAddRate.Value := g_Config.nUnknowRingACAddRate;
  EditUnknowRingMACAddValueMaxLimit.Value := g_Config.nUnknowRingMACAddValueMaxLimit;
  EditUnknowRingMACAddRate.Value := g_Config.nUnknowRingMACAddRate;

  EditUnknowNecklaceDCAddValueMaxLimit.Value := g_Config.nUnknowNecklaceDCAddValueMaxLimit;
  EditUnknowNecklaceDCAddRate.Value := g_Config.nUnknowNecklaceDCAddRate;
  EditUnknowNecklaceMCAddValueMaxLimit.Value := g_Config.nUnknowNecklaceMCAddValueMaxLimit;
  EditUnknowNecklaceMCAddRate.Value := g_Config.nUnknowNecklaceMCAddRate;
  EditUnknowNecklaceSCAddValueMaxLimit.Value := g_Config.nUnknowNecklaceSCAddValueMaxLimit;
  EditUnknowNecklaceSCAddRate.Value := g_Config.nUnknowNecklaceSCAddRate;
  EditUnknowNecklaceACAddValueMaxLimit.Value := g_Config.nUnknowNecklaceACAddValueMaxLimit;
  EditUnknowNecklaceACAddRate.Value := g_Config.nUnknowNecklaceACAddRate;
  EditUnknowNecklaceMACAddValueMaxLimit.Value := g_Config.nUnknowNecklaceMACAddValueMaxLimit;
  EditUnknowNecklaceMACAddRate.Value := g_Config.nUnknowNecklaceMACAddRate;

  EditUnknowHelMetDCAddValueMaxLimit.Value := g_Config.nUnknowHelMetDCAddValueMaxLimit;
  EditUnknowHelMetDCAddRate.Value := g_Config.nUnknowHelMetDCAddRate;
  EditUnknowHelMetMCAddValueMaxLimit.Value := g_Config.nUnknowHelMetMCAddValueMaxLimit;
  EditUnknowHelMetMCAddRate.Value := g_Config.nUnknowHelMetMCAddRate;
  EditUnknowHelMetSCAddValueMaxLimit.Value := g_Config.nUnknowHelMetSCAddValueMaxLimit;
  EditUnknowHelMetSCAddRate.Value := g_Config.nUnknowHelMetSCAddRate;
  EditUnknowHelMetACAddValueMaxLimit.Value := g_Config.nUnknowHelMetACAddValueMaxLimit;
  EditUnknowHelMetACAddRate.Value := g_Config.nUnknowHelMetACAddRate;
  EditUnknowHelMetMACAddValueMaxLimit.Value := g_Config.nUnknowHelMetMACAddValueMaxLimit;
  EditUnknowHelMetMACAddRate.Value := g_Config.nUnknowHelMetMACAddRate;
end;

procedure TfrmItemSet.EditUnknowRingDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingDCAddValueMaxLimit := EditUnknowRingDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowRingDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingDCAddRate := EditUnknowRingDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowRingMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingMCAddValueMaxLimit := EditUnknowRingMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowRingMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingMCAddRate := EditUnknowRingMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowRingSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingSCAddValueMaxLimit := EditUnknowRingSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowRingSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingSCAddRate := EditUnknowRingSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowRingACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingACAddValueMaxLimit := EditUnknowRingACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowRingACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingACAddRate := EditUnknowRingACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowRingMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingMACAddValueMaxLimit := EditUnknowRingMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowRingMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowRingMACAddRate := EditUnknowRingMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceDCAddValueMaxLimit := EditUnknowNecklaceDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceDCAddRate := EditUnknowNecklaceDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceMCAddValueMaxLimit := EditUnknowNecklaceMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceMCAddRate := EditUnknowNecklaceMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceSCAddValueMaxLimit := EditUnknowNecklaceSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceSCAddRate := EditUnknowNecklaceSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceACAddValueMaxLimit := EditUnknowNecklaceACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceACAddRate := EditUnknowNecklaceACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceMACAddValueMaxLimit := EditUnknowNecklaceMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowNecklaceMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowNecklaceMACAddRate := EditUnknowNecklaceMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetDCAddValueMaxLimit := EditUnknowHelMetDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetDCAddRate := EditUnknowHelMetDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetMCAddValueMaxLimit := EditUnknowHelMetMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetMCAddRate := EditUnknowHelMetMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetSCAddValueMaxLimit := EditUnknowHelMetSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetSCAddRate := EditUnknowHelMetSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetACAddValueMaxLimit := EditUnknowHelMetACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetACAddRate := EditUnknowHelMetACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetMACAddValueMaxLimit := EditUnknowHelMetMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditUnknowHelMetMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nUnknowHelMetMACAddRate := EditUnknowHelMetMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.RefShapeItem;
begin
  EditAttackPosionRate.Value := g_Config.nAttackPosionRate;
  EditAttackPosionTime.Value := g_Config.nAttackPosionTime;
  CheckBoxUserMoveCanDupObj.Checked := g_Config.boUserMoveCanDupObj;
  CheckBoxUserMoveCanOnItem.Checked := g_Config.boUserMoveCanOnItem;
  EditUserMoveTime.Value := g_Config.dwUserMoveTime;

  seMDParalysisRate.Value := g_Config.dwMDParalysisRate;
  seMDParalysisTime.Value := g_Config.dwMDParalysisTime;
  seFrozenRate.Value := g_Config.dwFrozenRate;
  seFrozenTime.Value := g_Config.dwFrozenTime;
  chkFrozenUseMagicStruck.Checked := g_Config.boFrozenUseMagicStruck;
  seCobwebWindingRate.Value := g_Config.dwCobwebWindingRate;
  seCobwebWindingTime.Value := g_Config.dwCobwebWindingTime;
  chkCobwebWindingUseMagicStruck.Checked := g_Config.boCobwebWindingUseMagicStruck;
end;

procedure TfrmItemSet.EditAttackPosionRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAttackPosionRate := EditAttackPosionRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditAttackPosionTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nAttackPosionTime := EditAttackPosionTime.Value;
  ModValue();
end;

procedure TfrmItemSet.CheckBoxUserMoveCanDupObjClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boUserMoveCanDupObj := CheckBoxUserMoveCanDupObj.Checked;
  ModValue();
end;

procedure TfrmItemSet.CheckBoxUserMoveCanOnItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boUserMoveCanOnItem := CheckBoxUserMoveCanOnItem.Checked;
  ModValue();
end;

procedure TfrmItemSet.EditUserMoveTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwUserMoveTime := EditUserMoveTime.Value;
  ModValue();
end;

procedure TfrmItemSet.chkItemNewAbilAllowUseClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  boSendServerConfig := True;
  g_Config.boItemNewAbilAllowUse := chkItemNewAbilAllowUse.Checked;
  ModValue();
end;

procedure TfrmItemSet.EditItemNewAbilMonRandomAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemNewAbilMonRandomAddValue := EditItemNewAbilMonRandomAddRate.Value;

  ModValue();
end;

procedure TfrmItemSet.EditItemNewAbilMakeRandomAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemNewAbilMakeRandomAddValue := EditItemNewAbilMakeRandomAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditItemNewAbilScriptRandomAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemNewAbilScriptRandomAddValue := EditItemNewAbilScriptRandomAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.ComboBoxNewAbilItemTypeChange(Sender: TObject);
var
  I: Integer;
  boEnabled: Boolean;
begin
  boEnabled := ButtonNewAbilSave.Enabled;
  CheckGroupNewAbil.Caption := ComboBoxNewAbilItemType.Items.Strings[ComboBoxNewAbilItemType.ItemIndex] + ' 属性';
  GroupBoxNewAbil.Caption := ComboBoxNewAbilItemType.Items.Strings[ComboBoxNewAbilItemType.ItemIndex] + ' 点数控制';
  for I := 0 to CheckGroupNewAbil.Items.Count - 1 do
  begin
    CheckGroupNewAbil.ItemChecked[I] := g_Config.ItemNewAbil[ComboBoxNewAbilItemType.ItemIndex][I];
  end;

  EditItemNewAbilAddValueMaxLimit.Value := g_Config.ItemNewAbilAddValueMaxLimit[ComboBoxNewAbilItemType.ItemIndex];
  EditItemNewAbilAddValueMaxLimit2.Value := g_Config.ItemNewAbilAddValueMaxLimit2[ComboBoxNewAbilItemType.ItemIndex];
  EditItemNewAbilAddValueRate.Value := g_Config.ItemNewAbilAddValueRate[ComboBoxNewAbilItemType.ItemIndex];
  EditItemNewAbilAddRate.Value := g_Config.ItemNewAbilAddRate[ComboBoxNewAbilItemType.ItemIndex];

  ButtonNewAbilSave.Enabled := boEnabled;
end;

procedure TfrmItemSet.CheckGroupNewAbilChange(Sender: TObject; Index: Integer; NewState: TCheckBoxState);
var
  I: Integer;
begin
  if not boOpened then
    Exit;
  if ComboBoxNewAbilItemType.ItemIndex < 0 then
  begin
    ComboBoxNewAbilItemType.SetFocus;
    Application.MessageBox('请选择一个装备类型！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;
  g_Config.ItemNewAbil[ComboBoxNewAbilItemType.ItemIndex][Index] := CheckGroupNewAbil.ItemChecked[Index];

  if Index = CheckGroupNewAbil.Items.Count - 1 then
  begin
    for I := 0 to CheckGroupNewAbil.Items.Count - 2 do
    begin
      CheckGroupNewAbil.ItemEnabled[I] := not CheckGroupNewAbil.ItemChecked[Index];
    end;
  end;
  ModValue();
end;

procedure TfrmItemSet.EditItemNewAbilAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ComboBoxNewAbilItemType.ItemIndex < 0 then
  begin
    ComboBoxNewAbilItemType.SetFocus;
    Application.MessageBox('请选择一个装备类型！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;
  g_Config.ItemNewAbilAddValueMaxLimit[ComboBoxNewAbilItemType.ItemIndex] := EditItemNewAbilAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditItemNewAbilAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ComboBoxNewAbilItemType.ItemIndex < 0 then
  begin
    ComboBoxNewAbilItemType.SetFocus;
    Application.MessageBox('请选择一个装备类型！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;
  g_Config.ItemNewAbilAddValueRate[ComboBoxNewAbilItemType.ItemIndex] := EditItemNewAbilAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditItemNewAbilAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ComboBoxNewAbilItemType.ItemIndex < 0 then
  begin
    ComboBoxNewAbilItemType.SetFocus;
    Application.MessageBox('请选择一个装备类型！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;
  g_Config.ItemNewAbilAddRate[ComboBoxNewAbilItemType.ItemIndex] := EditItemNewAbilAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.ButtonNewAbilSaveClick(Sender: TObject);
var
  I, II: Integer;
begin
  Config.WriteBool('Setup', 'ItemNewAbilAllowUse', g_Config.boItemNewAbilAllowUse);
  Config.WriteInteger('Setup', 'ItemNewAbilMonRandomAddValue', g_Config.nItemNewAbilMonRandomAddValue);
  Config.WriteInteger('Setup', 'ItemNewAbilMakeRandomAddValue', g_Config.nItemNewAbilMakeRandomAddValue);
  Config.WriteInteger('Setup', 'ItemNewAbilScriptRandomAddValue', g_Config.nItemNewAbilScriptRandomAddValue);

  Config.WriteBool('Setup', 'CloseDefenseUseScale', g_Config.boCloseDefenseUseScale);
  Config.WriteBool('Setup', 'ReboundUseScale', g_Config.boReboundUseScale);

  for I := Low(g_Config.ItemNewAbil) to High(g_Config.ItemNewAbil) do
  begin
    for II := Low(g_Config.ItemNewAbil[I]) to High(g_Config.ItemNewAbil[I]) do
    begin
      Config.WriteBool('Setup', 'ItemNewAbil' + IntToStr(I) + '-' + IntToStr(II), g_Config.ItemNewAbil[I][II]);
    end;
  end;

  for I := Low(g_Config.ItemNewAbilAddRate) to High(g_Config.ItemNewAbilAddRate) do
  begin
    Config.WriteInteger('Setup', 'ItemNewAbilAddRate' + IntToStr(I), g_Config.ItemNewAbilAddRate[I]);
  end;

  for I := Low(g_Config.ItemNewAbilAddValueMaxLimit) to High(g_Config.ItemNewAbilAddValueMaxLimit) do
  begin
    Config.WriteInteger('Setup', 'ItemNewAbilAddValueMaxLimit' + IntToStr(I), g_Config.ItemNewAbilAddValueMaxLimit[I]);
  end;

  for I := Low(g_Config.ItemNewAbilAddValueMaxLimit2) to High(g_Config.ItemNewAbilAddValueMaxLimit2) do
  begin
    Config.WriteInteger('Setup', 'ItemNewAbilAddValueMaxLimit2' + IntToStr(I), g_Config.ItemNewAbilAddValueMaxLimit2[I]);
  end;

  for I := Low(g_Config.ItemNewAbilAddValueRate) to High(g_Config.ItemNewAbilAddValueRate) do
  begin
    Config.WriteInteger('Setup', 'ItemNewAbilAddValueRate' + IntToStr(I), g_Config.ItemNewAbilAddValueRate[I]);
  end;

  Config.WriteInteger('Setup', 'CritAttackHurtRate', g_Config.dwCritAttackHurtRate);

  Config.WriteInteger('Setup', 'DamageReboundRate', g_Config.dwDamageReboundRate);

  Config.WriteInteger('Setup', 'FatalBlowBasePower', g_Config.dwFatalBlowBasePower);
  Config.WriteInteger('Setup', 'FatalBlowNeedPower1', g_Config.dwFatalBlowNeedPower1);
  Config.WriteInteger('Setup', 'FatalBlowNeedPower2', g_Config.dwFatalBlowNeedPower2);
  Config.WriteInteger('Setup', 'FatalBlowNeedPower3', g_Config.dwFatalBlowNeedPower3);

  if boSendServerConfig then
    UserEngine.SendServerConfig;

  boSendServerConfig := False;

  uModValue();
end;

procedure TfrmItemSet.EditWeaponDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponDCAddRate := EditWeaponDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponSCAddRate := EditWeaponSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponMCAddRate := EditWeaponMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressACAddValueMaxLimit := EditDressACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressACAddValueRate := EditDressACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressACAddRate := EditDressACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressMACAddValueMaxLimit := EditDressMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressMACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressMACAddValueRate := EditDressMACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditDressMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDressMACAddRate := EditDressMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19ACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19ACAddValueMaxLimit := EditNeckLace19ACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19ACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19ACAddValueRate := EditNeckLace19ACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19ACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19ACAddRate := EditNeckLace19ACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19MACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19MACAddValueMaxLimit := EditNeckLace19MACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19MACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19MACAddValueRate := EditNeckLace19MACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace19MACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace19MACAddRate := EditNeckLace19MACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124ACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124ACAddValueMaxLimit := EditNeckLace202124ACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124ACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124ACAddValueRate := EditNeckLace202124ACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124ACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124ACAddRate := EditNeckLace202124ACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124MACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124MACAddValueMaxLimit := EditNeckLace202124MACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124MACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124MACAddValueRate := EditNeckLace202124MACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditNeckLace202124MACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nNeckLace202124MACAddRate := EditNeckLace202124MACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26ACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26ACAddValueMaxLimit := EditArmRing26ACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26ACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26ACAddValueRate := EditArmRing26ACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26ACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26ACAddRate := EditArmRing26ACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26MACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26MACAddValueMaxLimit := EditArmRing26MACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26MACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26MACAddValueRate := EditArmRing26MACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditArmRing26MACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nArmRing26MACAddRate := EditArmRing26MACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23ACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23ACAddValueMaxLimit := EditRing23ACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23ACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23ACAddValueRate := EditRing23ACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23ACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23ACAddRate := EditRing23ACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23MACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23MACAddValueMaxLimit := EditRing23MACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23MACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23MACAddValueRate := EditRing23MACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing23MACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing23MACAddRate := EditRing23MACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetACAddValueMaxLimit := EditHelMetACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetACAddValueRate := EditHelMetACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetACAddRate := EditHelMetACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetMACAddValueMaxLimit := EditHelMetMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetMACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetMACAddValueRate := EditHelMetMACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditHelMetMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHelMetMACAddRate := EditHelMetMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsDCAddValueMaxLimit := EditBootsDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsDCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsDCAddValueRate := EditBootsDCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsDCAddRate := EditBootsDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsSCAddValueMaxLimit := EditBootsSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsSCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsSCAddValueRate := EditBootsSCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsSCAddRate := EditBootsSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsMCAddValueMaxLimit := EditBootsMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsMCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsMCAddValueRate := EditBootsMCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsMCAddRate := EditBootsMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsACAddValueMaxLimit := EditBootsACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsACAddValueRate := EditBootsACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsACAddRate := EditBootsACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsMACAddValueMaxLimit := EditBootsMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsMACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsMACAddValueRate := EditBootsMACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditBootsMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nBootsMACAddRate := EditBootsMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponHitSpeedAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponHitSpeedAddValueMaxLimit := EditWeaponHitSpeedAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponHitSpeedAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponHitSpeedAddValueRate := EditWeaponHitSpeedAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditWeaponHitSpeedAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nWeaponHitSpeedAddRate := EditWeaponHitSpeedAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressDCAddValueMaxLimit := seFashionDressDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponDCAddValueMaxLimit := seFashionWeaponDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponDCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponDCAddValueRate := seFashionWeaponDCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponDCAddRate := seFashionWeaponDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponHitSpeedAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponHitSpeedAddValueMaxLimit := seFashionWeaponHitSpeedAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponHitSpeedAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponHitSpeedAddValueRate := seFashionWeaponHitSpeedAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponHitSpeedAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponHitSpeedAddRate := seFashionWeaponHitSpeedAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponSCAddValueMaxLimit := seFashionWeaponSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponSCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponSCAddValueRate := seFashionWeaponSCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponSCAddRate := seFashionWeaponSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponMCAddValueMaxLimit := seFashionWeaponMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponMCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponMCAddValueRate := seFashionWeaponMCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionWeaponMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionWeaponMCAddRate := seFashionWeaponMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressDCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressDCAddValueRate := seFashionDressDCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressDCAddRate := seFashionDressDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressACAddValueMaxLimit := seFashionDressACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressACAddValueRate := seFashionDressACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressACAddRate := seFashionDressACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressSCAddValueMaxLimit := seFashionDressSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressSCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressSCAddValueRate := seFashionDressSCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressSCAddRate := seFashionDressSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressMACAddValueMaxLimit := seFashionDressMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressMACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressMACAddValueRate := seFashionDressMACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressMACAddRate := seFashionDressMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressMCAddValueMaxLimit := seFashionDressMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressMCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressMCAddValueRate := seFashionDressMCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFashionDressMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nFashionDressMCAddRate := seFashionDressMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseDCAddValueMaxLimit := seHorseDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseDCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseDCAddValueRate := seHorseDCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseDCAddRate := seHorseDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseACAddValueMaxLimit := seHorseACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseACAddValueRate := seHorseACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseACAddRate := seHorseACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseSCAddValueMaxLimit := seHorseSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseSCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseSCAddValueRate := seHorseSCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseSCAddRate := seHorseSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseMACAddValueMaxLimit := seHorseMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseMACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseMACAddValueRate := seHorseMACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseMACAddRate := seHorseMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseMCAddValueMaxLimit := seHorseMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseMCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseMCAddValueRate := seHorseMCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seHorseMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nHorseMCAddRate := seHorseMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumDCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumDCAddValueRate := seDrumDCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumDCAddValueMaxLimit := seDrumDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumDCAddRate := seDrumDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumACAddValueMaxLimit := seDrumACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumACAddValueRate := seDrumACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumACAddRate := seDrumACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumSCAddValueMaxLimit := seDrumSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumSCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumSCAddValueRate := seDrumSCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumSCAddRate := seDrumSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumMACAddValueMaxLimit := seDrumMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumMACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumMACAddValueRate := seDrumMACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumMACAddRate := seDrumMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumMCAddValueMaxLimit := seDrumMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumMCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumMCAddValueRate := seDrumMCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seDrumMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nDrumMCAddRate := seDrumMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldDCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldDCAddValueMaxLimit := seShieldDCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldDCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldDCAddValueRate := seShieldDCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldDCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldDCAddRate := seShieldDCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldACAddValueMaxLimit := seShieldACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldACAddValueRate := seShieldACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldACAddRate := seShieldACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldSCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldSCAddValueMaxLimit := seShieldSCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldSCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldSCAddValueRate := seShieldSCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldSCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldSCAddRate := seShieldSCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldMACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldMACAddValueMaxLimit := seShieldMACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldMACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldMACAddValueRate := seShieldMACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldMACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldMACAddRate := seShieldMACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldMCAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldMCAddValueMaxLimit := seShieldMCAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldMCAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldMCAddValueRate := seShieldMCAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seShieldMCAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nShieldMCAddRate := seShieldMCAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seMDParalysisRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMDParalysisRate := seMDParalysisRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seMDParalysisTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwMDParalysisTime := seMDParalysisTime.Value;
  ModValue();
end;

procedure TfrmItemSet.seFrozenRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwFrozenRate := seFrozenRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seFrozenTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwFrozenTime := seFrozenTime.Value;
  ModValue();
end;

procedure TfrmItemSet.seCobwebWindingRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwCobwebWindingRate := seCobwebWindingRate.Value;
  ModValue();
end;

procedure TfrmItemSet.seCobwebWindingTimeChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwCobwebWindingTime := seCobwebWindingTime.Value;
  ModValue();
end;

procedure TfrmItemSet.chkFrozenUseMagicStruckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boFrozenUseMagicStruck := chkFrozenUseMagicStruck.Checked;
  ModValue();
end;

procedure TfrmItemSet.chkCobwebWindingUseMagicStruckClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCobwebWindingUseMagicStruck := chkCobwebWindingUseMagicStruck.Checked;
  ModValue();
end;

procedure TfrmItemSet.seCritAttackHurtRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwCritAttackHurtRate := seCritAttackHurtRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22ACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22ACAddValueMaxLimit := EditRing22ACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22ACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22ACAddValueRate := EditRing22ACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22ACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22ACAddRate := EditRing22ACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22MACAddValueMaxLimitChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22MACAddValueMaxLimit := EditRing22MACAddValueMaxLimit.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22MACAddValueRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22MACAddValueRate := EditRing22MACAddValueRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditRing22MACAddRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nRing22MACAddRate := EditRing22MACAddRate.Value;
  ModValue();
end;

procedure TfrmItemSet.chkOpenItemFluteClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boOpenItemFlute := chkOpenItemFlute.Checked;
  ModValue();
end;

procedure TfrmItemSet.chkDisableRightClickFluteStoneClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boDisableRightClickFluteStone := chkDisableRightClickFluteStone.Checked;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmItemSet.seItemFluteStoneCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemFluteStoneCount := seItemFluteStoneCount.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmItemSet.seDamageReboundRateChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwDamageReboundRate := seDamageReboundRate.Value;
  ModValue();
end;

procedure TfrmItemSet.EditItemNewAbilAddValueMaxLimit2Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ComboBoxNewAbilItemType.ItemIndex < 0 then
  begin
    ComboBoxNewAbilItemType.SetFocus;
    Application.MessageBox('请选择一个装备类型！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;
  g_Config.ItemNewAbilAddValueMaxLimit2[ComboBoxNewAbilItemType.ItemIndex] := EditItemNewAbilAddValueMaxLimit2.Value;
  ModValue();
end;

procedure TfrmItemSet.seItemFluteStoneIdxCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemFluteStoneIdxCount := seItemFluteStoneIdxCount.Value;
  boSendServerConfig := True;
  ModValue();
end;

procedure TfrmItemSet.seFatalBlowBasePowerChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwFatalBlowBasePower := seFatalBlowBasePower.Value;
  ModValue();
end;

procedure TfrmItemSet.seFatalBlowNeedPower1Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwFatalBlowNeedPower1 := seFatalBlowNeedPower1.Value;
  ModValue();
end;

procedure TfrmItemSet.seFatalBlowNeedPower2Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwFatalBlowNeedPower2 := seFatalBlowNeedPower2.Value;
  ModValue();
end;

procedure TfrmItemSet.seFatalBlowNeedPower3Change(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.dwFatalBlowNeedPower3 := seFatalBlowNeedPower3.Value;
  ModValue();
end;

procedure TfrmItemSet.chkCloseDefenseUseScaleClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boCloseDefenseUseScale := chkCloseDefenseUseScale.Checked;
  ModValue();
end;

procedure TfrmItemSet.chkReboundUseScaleClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boReboundUseScale := chkReboundUseScale.Checked;
  ModValue();
end;

procedure TfrmItemSet.seItemFluteStoneOverlapCountChange(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.nItemFluteStoneOverlapCount := seItemFluteStoneOverlapCount.Value;
  boSendServerConfig := True;
  ModValue();
end;

end.


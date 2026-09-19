unit ViewList2;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls, Spin, ComCtrls, Grobal2, Menus,
  Grids, GroupItems, SpinEditEx, M2Threads, ExtCtrls, StrUtils,
{$IFDEF CPUX64}
  // VMProtectSDK,
{$ENDIF}
  uCustomMagicUtils, M2Definition, System.Types, VirtualTrees;

const
  WM_STARTEDITING_CUSTOMMONEY = WM_USER + 790;

type
  TCustomPropertyEditLink = class(TInterfacedObject, IVTEditLink)
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

  TFrmViewList2 = class(TForm)
    PageControl: TPageControl;
    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    GroupBox11: TGroupBox;
    ListBoxitemList: TListBox;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    ComboBoxShopType: TComboBox;
    EditShopItemName: TEdit;
    EditShopItemPrice: TSpinEditEx;
    Label4: TLabel;
    Label5: TLabel;
    EditImageIndex: TSpinEditEx;
    EditImageCount: TSpinEditEx;
    Label6: TLabel;
    EditItemMemo1: TEdit;
    MemoShop: TMemo;
    Label7: TLabel;
    ButtonAddShopItem: TButton;
    ButtonDelShopItem: TButton;
    ButtonShopRefresh: TButton;
    ButtonShopChgItem: TButton;
    GroupBox1: TGroupBox;
    ListBoxitemList1: TListBox;
    GroupBox2: TGroupBox;
    ListBoxBoxItem: TListBox;
    GroupBoxBoxItem: TGroupBox;
    Label10: TLabel;
    TabSheet3: TTabSheet;
    TabSheet4: TTabSheet;
    TabSheet5: TTabSheet;
    TabSheet6: TTabSheet;
    GroupBox22: TGroupBox;
    ListViewMsgFilter: TListView;
    GroupBox23: TGroupBox;
    Label22: TLabel;
    Label23: TLabel;
    EditFilterMsg: TEdit;
    EditNewMsg: TEdit;
    ButtonMsgFilterAdd: TButton;
    ButtonMsgFilterDel: TButton;
    ButtonMsgFilterChg: TButton;
    GroupBox5: TGroupBox;
    ListBoxUserCommand: TListBox;
    GroupBox6: TGroupBox;
    ListBoxItemRuleList: TListBox;
    GroupBox7: TGroupBox;
    ListBoxItemList2: TListBox;
    GroupBox8: TGroupBox;
    Label13: TLabel;
    EditRuleItemName: TEdit;
    GroupBoxItemRule: TGroupBox;
    CheckBox1: TCheckBox;
    CheckBox3: TCheckBox;
    CheckBox4: TCheckBox;
    CheckBox5: TCheckBox;
    CheckBox6: TCheckBox;
    CheckBox7: TCheckBox;
    CheckBox2: TCheckBox;
    CheckBox8: TCheckBox;
    chkNoLevel: TCheckBox;
    chkNoSell: TCheckBox;
    chkHeroItem: TCheckBox;
    CheckBox12: TCheckBox;
    CheckBox13: TCheckBox;
    CheckBox14: TCheckBox;
    CheckBox15: TCheckBox;
    CheckBox16: TCheckBox;
    CheckBox17: TCheckBox;
    CheckBox18: TCheckBox;
    chkHeroBag: TCheckBox;
    chkButchItem: TCheckBox;
    chkTriggerHint: TCheckBox;
    chkNoGift: TCheckBox;
    ButtonItemRuleSelAll: TButton;
    ButtonItemRuleNotSelAll: TButton;
    ButtonItemRuleAdd: TButton;
    ButtonItemRuleDel: TButton;
    ButtonItemRuleAddAll: TButton;
    ButtonItemRuleDelAll: TButton;
    ButtonItemRuleChg: TButton;
    ButtonItemRuleSave: TButton;
    CheckBox24: TCheckBox;
    CheckBox25: TCheckBox;
    CheckBox26: TCheckBox;
    CheckBox27: TCheckBox;
    CheckBox28: TCheckBox;
    CheckBox29: TCheckBox;
    GroupBox10: TGroupBox;
    EditCommandName: TEdit;
    EditCommandIdx: TSpinEditEx;
    ButtonUserCommandAdd: TButton;
    ButtonUserCommandDel: TButton;
    Label11: TLabel;
    Label12: TLabel;
    TabSheet7: TTabSheet;
    TabSheet8: TTabSheet;
    GroupBox3: TGroupBox;
    ListBoxWilNameList: TListBox;
    GroupBox4: TGroupBox;
    ListBoxStdItemList1: TListBox;
    PopupMenuItem: TPopupMenu;
    MenuItem_ShowAll: TMenuItem;
    MenuItem_ShowDress: TMenuItem;
    MenuItem_ShowNECKLACE: TMenuItem;
    MenuItem_ShowWEAPON: TMenuItem;
    MenuItem_ShowRING: TMenuItem;
    MenuItem_ShowRIGHTHAND: TMenuItem;
    MenuItem_ShowHELMET: TMenuItem;
    MenuItem_ShowARMRING: TMenuItem;
    MenuItem_ShowBOOTS: TMenuItem;
    MenuItem_ShowBUJUK: TMenuItem;
    MenuItem_BELT: TMenuItem;
    MenuItem_CHARM: TMenuItem;
    MenuItem_ShowDrugIrem: TMenuItem;
    MenuItem_ShowBindIrem: TMenuItem;
    MenuItem_ShowOTHERITEM: TMenuItem;
    btnWilNameUP: TButton;
    btnWilNameDown: TButton;
    EditWilName: TEdit;
    Label27: TLabel;
    btnWilAdd: TButton;
    btnWilDel: TButton;
    btnWilSave: TButton;
    ButtonSendEffectImageList: TButton;
    Label44: TLabel;
    GroupBox12: TGroupBox;
    ListBoxEffectItemList: TListBox;
    ComboBoxGameMoney: TComboBox;
    Label55: TLabel;
    PopupMenu1: TPopupMenu;
    MenuItem1: TMenuItem;
    MenuItem2: TMenuItem;
    MenuItem3: TMenuItem;
    MenuItem4: TMenuItem;
    MenuItem5: TMenuItem;
    MenuItem6: TMenuItem;
    MenuItem7: TMenuItem;
    MenuItem8: TMenuItem;
    MenuItem9: TMenuItem;
    MenuItem10: TMenuItem;
    MenuItem11: TMenuItem;
    MenuItem12: TMenuItem;
    MenuItem13: TMenuItem;
    MenuItem14: TMenuItem;
    MenuItem15: TMenuItem;
    PopupMenu2: TPopupMenu;
    MenuItem16: TMenuItem;
    MenuItem17: TMenuItem;
    MenuItem18: TMenuItem;
    MenuItem19: TMenuItem;
    MenuItem20: TMenuItem;
    MenuItem21: TMenuItem;
    MenuItem22: TMenuItem;
    MenuItem23: TMenuItem;
    MenuItem24: TMenuItem;
    MenuItem25: TMenuItem;
    MenuItem26: TMenuItem;
    MenuItem27: TMenuItem;
    MenuItem28: TMenuItem;
    MenuItem29: TMenuItem;
    MenuItem30: TMenuItem;
    PopupMenu3: TPopupMenu;
    MenuItem31: TMenuItem;
    MenuItem32: TMenuItem;
    MenuItem33: TMenuItem;
    MenuItem34: TMenuItem;
    MenuItem35: TMenuItem;
    MenuItem36: TMenuItem;
    MenuItem37: TMenuItem;
    MenuItem38: TMenuItem;
    MenuItem39: TMenuItem;
    MenuItem40: TMenuItem;
    MenuItem41: TMenuItem;
    MenuItem42: TMenuItem;
    MenuItem43: TMenuItem;
    MenuItem44: TMenuItem;
    MenuItem45: TMenuItem;
    PopupMenu4: TPopupMenu;
    MenuItem46: TMenuItem;
    MenuItem47: TMenuItem;
    MenuItem48: TMenuItem;
    MenuItem49: TMenuItem;
    MenuItem50: TMenuItem;
    MenuItem51: TMenuItem;
    MenuItem52: TMenuItem;
    MenuItem53: TMenuItem;
    MenuItem54: TMenuItem;
    MenuItem55: TMenuItem;
    MenuItem56: TMenuItem;
    MenuItem57: TMenuItem;
    MenuItem58: TMenuItem;
    MenuItem59: TMenuItem;
    MenuItem60: TMenuItem;
    PageControlShop: TPageControl;
    TabSheetShop1: TTabSheet;
    TabSheetShop2: TTabSheet;
    TabSheetShop3: TTabSheet;
    TabSheetShop4: TTabSheet;
    TabSheetShop5: TTabSheet;
    TabSheetShop6: TTabSheet;
    ListViewShop1: TListView;
    ListViewShop2: TListView;
    ListViewShop3: TListView;
    ListViewShop4: TListView;
    ListViewShop5: TListView;
    ListViewShop6: TListView;
    LabelFileIndex: TLabel;
    TabSheet9: TTabSheet;
    GroupBox15: TGroupBox;
    lstEffectList: TListBox;
    Label45: TLabel;
    ComboBoxEffectIndex: TComboBox;
    lblEffectMemo: TLabel;
    ButtonEffectItemAdd: TButton;
    ButtonEffectItemChg: TButton;
    ButtonEffectItemDel: TButton;
    ButtonEffectItemSave: TButton;
    Labellbl1: TLabel;
    Label47: TLabel;
    EditEffectItemName: TEdit;
    Label48: TLabel;
    Label49: TLabel;
    TabSheet10: TTabSheet;
    GroupBox16: TGroupBox;
    ListViewFoundryNeedItemList: TListView;
    GroupBox17: TGroupBox;
    ListViewFoundryItemList: TListView;
    GroupBox18: TGroupBox;
    ListBoxFoundryItemList: TListBox;
    ButtonFoundryItemAdd: TButton;
    ButtonFoundryItemDel: TButton;
    ButtonFoundryItemChg: TButton;
    ButtonFoundryItemSave: TButton;
    Label50: TLabel;
    EditFoundryItemRate: TSpinEditEx;
    Label52: TLabel;
    EditFoundryGiveItemCount: TSpinEditEx;
    ButtonFoundryNeedItemAdd: TButton;
    ButtonFoundryNeedItemDel: TButton;
    ButtonFoundryNeedItemChg: TButton;
    Label53: TLabel;
    EditFoundryNeedItemDel: TSpinEditEx;
    Label54: TLabel;
    ComboBoxFoundryNeedItemName: TComboBox;
    Label56: TLabel;
    EditFoundryNeedItemCount: TSpinEditEx;
    PopupMenu5: TPopupMenu;
    MenuItem61: TMenuItem;
    MenuItem62: TMenuItem;
    MenuItem63: TMenuItem;
    MenuItem64: TMenuItem;
    MenuItem65: TMenuItem;
    MenuItem66: TMenuItem;
    MenuItem67: TMenuItem;
    MenuItem68: TMenuItem;
    MenuItem69: TMenuItem;
    MenuItem70: TMenuItem;
    MenuItem71: TMenuItem;
    MenuItem72: TMenuItem;
    MenuItem73: TMenuItem;
    MenuItem74: TMenuItem;
    MenuItem75: TMenuItem;
    ListBoxGiveItem: TListBox;
    ListBoxNoGiveItem: TListBox;
    ListBoxCenterItem: TListBox;
    ListBoxEndNoGiveItem: TListBox;
    Label57: TLabel;
    Label58: TLabel;
    Label59: TLabel;
    ButtonShopItemUP: TButton;
    ButtonShopItemDOWN: TButton;
    ButtonShopSaveItem: TButton;
    ButtonMsgFilterSave: TButton;
    ButtonUserCommandSave: TButton;
    LabelMsg: TLabel;
    TabSheet11: TTabSheet;
    TabSheet12: TTabSheet;
    ListViewFilterItem: TListView;
    GroupBox20: TGroupBox;
    ListBoxitemList4: TListBox;
    MemoItemDesc: TMemo;
    btnItemDescSave: TButton;
    LabelItemName: TLabel;
    LabelItemType: TLabel;
    ComboBoxItemFilter: TComboBox;
    EditFilterItemName: TEdit;
    ButtonFilterAdd: TButton;
    ButtonFilterDel: TButton;
    GroupBox19: TGroupBox;
    CheckBoxHintItem: TCheckBox;
    CheckBoxPickUpItem: TCheckBox;
    CheckBoxShowItemName: TCheckBox;
    PopupMenu6: TPopupMenu;
    MenuItem76: TMenuItem;
    MenuItem77: TMenuItem;
    MenuItem78: TMenuItem;
    MenuItem79: TMenuItem;
    MenuItem80: TMenuItem;
    MenuItem81: TMenuItem;
    MenuItem82: TMenuItem;
    MenuItem83: TMenuItem;
    MenuItem84: TMenuItem;
    MenuItem85: TMenuItem;
    MenuItem86: TMenuItem;
    MenuItem87: TMenuItem;
    MenuItem88: TMenuItem;
    MenuItem89: TMenuItem;
    MenuItem90: TMenuItem;
    ButtonFilterChg: TButton;
    ButtonFilterSave: TButton;
    PageControlTzItem: TPageControl;
    TabSheet13: TTabSheet;
    TabSheet14: TTabSheet;
    GroupBox24: TGroupBox;
    ListViewGroupItemList: TListView;
    GroupBoxGroupItem: TGroupBox;
    CheckBoxGroupItemFlag1: TCheckBox;
    CheckBoxGroupItemFlag2: TCheckBox;
    CheckBoxGroupItemFlag3: TCheckBox;
    CheckBoxGroupItemFlag4: TCheckBox;
    CheckBoxGroupItemFlag8: TCheckBox;
    CheckBoxGroupItemFlag7: TCheckBox;
    CheckBoxGroupItemFlag6: TCheckBox;
    CheckBoxGroupItemFlag5: TCheckBox;
    CheckBoxGroupItemFlag16: TCheckBox;
    CheckBoxGroupItemFlag15: TCheckBox;
    CheckBoxGroupItemFlag14: TCheckBox;
    CheckBoxGroupItemFlag13: TCheckBox;
    CheckBoxGroupItemFlag12: TCheckBox;
    CheckBoxGroupItemFlag11: TCheckBox;
    CheckBoxGroupItemFlag10: TCheckBox;
    CheckBoxGroupItemFlag9: TCheckBox;
    PageControl2: TPageControl;
    TabSheetRate: TTabSheet;
    lbl19: TLabel;
    lbl20: TLabel;
    Label14: TLabel;
    Label20: TLabel;
    Label19: TLabel;
    Label18: TLabel;
    Label17: TLabel;
    Label16: TLabel;
    Label15: TLabel;
    Label25: TLabel;
    Label26: TLabel;
    EditGroupItemMPRate: TSpinEditEx;
    EditGroupItemHPRate: TSpinEditEx;
    EditSpellRecoverRate: TSpinEditEx;
    EditHealthRecoverRate: TSpinEditEx;
    EditPoisonRecoverRate: TSpinEditEx;
    EditAntiPoisonRate: TSpinEditEx;
    EditAntiMagicRate: TSpinEditEx;
    EditSpeedPointRate: TSpinEditEx;
    EditHitPointRate: TSpinEditEx;
    TabSheetValue: TTabSheet;
    Label69: TLabel;
    Label70: TLabel;
    Label71: TLabel;
    Label72: TLabel;
    Label73: TLabel;
    seGroupItemSCValue: TSpinEditEx;
    seGroupItemMCValue: TSpinEditEx;
    seGroupItemDCValue: TSpinEditEx;
    seGroupItemMACValue: TSpinEditEx;
    seGroupItemACValue: TSpinEditEx;
    RadioButtonRate: TRadioButton;
    RadioButtonValue: TRadioButton;
    EditGroupItemIndex: TSpinEditEx;
    Label32: TLabel;
    Label51: TLabel;
    Label33: TLabel;
    lbl29: TLabel;
    EditGroupItemHint: TEdit;
    EditGroupItemName: TEdit;
    EditGroupItemCount: TSpinEditEx;
    Label31: TLabel;
    EditGroupItemDesc: TEdit;
    ButtonGroupItemAdd: TButton;
    ButtonGroupItemDel: TButton;
    ButtonGroupItemChg: TButton;
    ButtonGroupItemSave: TButton;
    MemoTzItemDesc: TMemo;
    ButtonTzItemDescSave: TButton;
    ButtonFilterAddAll: TButton;
    ButtonFilterDelAll: TButton;
    ButtonGroupItemSkillPower: TButton;
    TabSheet15: TTabSheet;
    GroupBox21: TGroupBox;
    ListBoxitemList5: TListBox;
    GroupBoxSkillPowerItem: TGroupBox;
    StringGridSkillPower: TStringGrid;
    GroupBox26: TGroupBox;
    ListBoxSkillPowerItem: TListBox;
    PopupMenu7: TPopupMenu;
    MenuItem91: TMenuItem;
    MenuItem92: TMenuItem;
    MenuItem93: TMenuItem;
    MenuItem94: TMenuItem;
    MenuItem95: TMenuItem;
    MenuItem96: TMenuItem;
    MenuItem97: TMenuItem;
    MenuItem98: TMenuItem;
    MenuItem99: TMenuItem;
    MenuItem100: TMenuItem;
    MenuItem101: TMenuItem;
    MenuItem102: TMenuItem;
    MenuItem103: TMenuItem;
    MenuItem104: TMenuItem;
    MenuItem105: TMenuItem;
    ButtonAddSkillPowerItem: TButton;
    ButtonDelSkillPowerItem: TButton;
    ButtonChgSkillPowerItem: TButton;
    ButtonSaveSkillPowerItem: TButton;
    seGroupItemACValue2: TSpinEditEx;
    seGroupItemMACValue2: TSpinEditEx;
    seGroupItemDCValue2: TSpinEditEx;
    seGroupItemMCValue2: TSpinEditEx;
    seGroupItemSCValue2: TSpinEditEx;
    Label67: TLabel;
    Label68: TLabel;
    Label74: TLabel;
    Label75: TLabel;
    Label76: TLabel;
    Label77: TLabel;
    Label78: TLabel;
    Label79: TLabel;
    Label80: TLabel;
    seGroupItemMPValue: TSpinEditEx;
    seGroupItemHPValue: TSpinEditEx;
    seSpellRecoverValue: TSpinEditEx;
    seHealthRecoverValue: TSpinEditEx;
    sePoisonRecoverValue: TSpinEditEx;
    seAntiPoisonValue: TSpinEditEx;
    seAntiMagicValue: TSpinEditEx;
    seSpeedPointValue: TSpinEditEx;
    seHitPointValue: TSpinEditEx;
    Label36: TLabel;
    SpinEditEx6: TSpinEditEx;
    Label37: TLabel;
    Label38: TLabel;
    lbl25: TLabel;
    lbl27: TLabel;
    lbl9: TLabel;
    lbl11: TLabel;
    lbl14: TLabel;
    Label39: TLabel;
    Label40: TLabel;
    Label41: TLabel;
    Label42: TLabel;
    Label43: TLabel;
    EditGroupItemSCRate: TSpinEditEx;
    EditGroupItemMCRate: TSpinEditEx;
    EditGroupItemDCRate: TSpinEditEx;
    EditGroupItemMACRate: TSpinEditEx;
    EditGroupItemACRate: TSpinEditEx;
    Label61: TLabel;
    lblEffectDesc: TLabel;
    Label8: TLabel;
    Label60: TLabel;
    ComboBoxBoxItemType: TComboBox;
    edtBoxItemName: TEdit;
    lbl1: TLabel;
    lbl13: TLabel;
    cbbOtherItem: TComboBox;
    lbl15: TLabel;
    btnAddBoxItem: TButton;
    btnDelBoxItem: TButton;
    btnSaveBoxItem: TButton;
    grp1: TGroupBox;
    chkNext: TCheckBox;
    lbl16: TLabel;
    lbl17: TLabel;
    lbl18: TLabel;
    lbl21: TLabel;
    lbl22: TLabel;
    lbl23: TLabel;
    lbl24: TLabel;
    seCount: TSpinEditEx;
    seBoxItemCount: TSpinEditEx;
    seGold: TSpinEditEx;
    seGameGold: TSpinEditEx;
    seAddGold: TSpinEditEx;
    seAddGameGold: TSpinEditEx;
    seEndGold: TSpinEditEx;
    seEndGameGold: TSpinEditEx;
    chkSendFilterItemList: TCheckBox;
    chkSendItemDescList: TCheckBox;
    chkSendTzItemDescList: TCheckBox;
    CheckBox9: TCheckBox;
    CheckBox10: TCheckBox;
    CheckBox11: TCheckBox;
    CheckBox19: TCheckBox;
    chk1: TCheckBox;
    lbl28: TLabel;
    seItemCount: TSpinEditEx;
    chkShowSpecial: TCheckBox;
    grp2: TGroupBox;
    chkSkillPowerItemUseHum: TCheckBox;
    chkSkillPowerItemUseMon: TCheckBox;
    Label9: TLabel;
    Label21: TLabel;
    Label24: TLabel;
    Label28: TLabel;
    Label29: TLabel;
    EditGroupItemSCRate2: TSpinEditEx;
    EditGroupItemMCRate2: TSpinEditEx;
    EditGroupItemDCRate2: TSpinEditEx;
    EditGroupItemMACRate2: TSpinEditEx;
    EditGroupItemACRate2: TSpinEditEx;
    chkAutoMove: TCheckBox;
    chkSingleHint: TCheckBox;
    pmFilterItem: TPopupMenu;
    mniN1: TMenuItem;
    mniN6: TMenuItem;
    mniN7: TMenuItem;
    mniN10: TMenuItem;
    mniN2: TMenuItem;
    mniN3: TMenuItem;
    mniN4: TMenuItem;
    mniS1: TMenuItem;
    mniS2: TMenuItem;
    mniS3: TMenuItem;
    mniS4: TMenuItem;
    mniC1: TMenuItem;
    mniC2: TMenuItem;
    mniC3: TMenuItem;
    mniC4: TMenuItem;
    chkEnabledBuyShopItemGive: TCheckBox;
    lbl30: TLabel;
    pmRuleList: TPopupMenu;
    lbl31: TLabel;
    lbl32: TLabel;
    Label84: TLabel;
    btnWilEdit: TButton;
    PageControl1: TPageControl;
    tsItemDesc: TTabSheet;
    tsItemDescTop: TTabSheet;
    mmoItemDescTop: TMemo;
    chkSendItemDescTopList: TCheckBox;
    btnItemDescTopSave: TButton;
    chkBulkBuy: TCheckBox;
    ButtonEffectAdd: TButton;
    ButtonEffectChg: TButton;
    ButtonEffectDel: TButton;
    ButtonEffectSave: TButton;
    Label137: TLabel;
    Label46: TLabel;
    GroupBox13: TGroupBox;
    lbl2: TLabel;
    lbl3: TLabel;
    lbl4: TLabel;
    Label62: TLabel;
    Label63: TLabel;
    Label34: TLabel;
    lbl33: TLabel;
    seItemEffectOffset1: TSpinEditEx;
    seItemEffectImageCount1: TSpinEditEx;
    cbbEffectFileIndex1: TComboBox;
    seItemEffectOffsetX1: TSpinEditEx;
    seItemEffectOffsetY1: TSpinEditEx;
    seItemEffectTime1: TSpinEditEx;
    chkDrawCenter1: TCheckBox;
    chkNoBlendMode1: TCheckBox;
    chkEfectBelowItem1: TCheckBox;
    GroupBox14: TGroupBox;
    lbl5: TLabel;
    lbl6: TLabel;
    lbl7: TLabel;
    Label85: TLabel;
    seItemEffectOffset2: TSpinEditEx;
    cbbEffectFileIndex2: TComboBox;
    chkNoBlendMode2: TCheckBox;
    chkNoSex2: TCheckBox;
    GroupBox25: TGroupBox;
    lbl8: TLabel;
    lbl10: TLabel;
    lbl12: TLabel;
    Label64: TLabel;
    Label65: TLabel;
    Label35: TLabel;
    Label87: TLabel;
    seItemEffectOffset3: TSpinEditEx;
    seItemEffectImageCount3: TSpinEditEx;
    cbbEffectFileIndex3: TComboBox;
    seItemEffectOffsetX3: TSpinEditEx;
    seItemEffectOffsetY3: TSpinEditEx;
    seItemEffectTime3: TSpinEditEx;
    chkDrawCenter3: TCheckBox;
    chkNoBlendMode3: TCheckBox;
    EditEffectDesc: TEdit;
    EditEffectIndex: TSpinEditEx;
    GroupBox27: TGroupBox;
    Label30: TLabel;
    Label81: TLabel;
    Label66: TLabel;
    Label82: TLabel;
    lbl26: TLabel;
    Label83: TLabel;
    Label86: TLabel;
    seAddEffectOffset: TSpinEditEx;
    cbbAddEffectFileIndex: TComboBox;
    seAddEffectPlayCount: TSpinEditEx;
    seAddEffectPlayTime: TSpinEditEx;
    chkAddEffectNoBlend: TCheckBox;
    cbbAddEffectDrawOrder: TComboBox;
    chkAddEffectDrawCenter: TCheckBox;
    GroupBox28: TGroupBox;
    Label88: TLabel;
    Label89: TLabel;
    Label90: TLabel;
    Label91: TLabel;
    Label92: TLabel;
    Label93: TLabel;
    Label94: TLabel;
    seItemEffectOffset5: TSpinEditEx;
    seItemEffectImageCount5: TSpinEditEx;
    cbbEffectFileIndex5: TComboBox;
    seItemEffectOffsetX5: TSpinEditEx;
    seItemEffectOffsetY5: TSpinEditEx;
    seItemEffectTime5: TSpinEditEx;
    chkDrawCenter5: TCheckBox;
    chkNoBlendMode5: TCheckBox;
    Bevel1: TBevel;
    chkEfectBelowItem5: TCheckBox;
    chkTZSupportRenameItem: TCheckBox;
    chkDescSupportRenamItem: TCheckBox;
    chkNoRenameDescReadDefault: TCheckBox;
    Label95: TLabel;
    seBulkBuyCount: TSpinEditEx;
    CheckBox20: TCheckBox;
    lbl34: TLabel;
    cbbAuctionPricesType: TComboBox;
    lbl35: TLabel;
    seAuctionPrices_Min: TSpinEditLongWord;
    seAuctionPrices_Max: TSpinEditLongWord;
    Label96: TLabel;
    lbl36: TLabel;
    chkEnableHeroUseClientPickItems: TCheckBox;
    chkEnablePlayerUseClientPickItems: TCheckBox;
    chkTriggerGetBoxsItem: TCheckBox;
    chkDisablePreviewMonItem: TCheckBox;
    CheckBox21: TCheckBox;
    N1: TMenuItem;
    seItemGroupIndex: TSpinEdit;
    Label97: TLabel;
    TabSheet16: TTabSheet;
    vstCustomMoney: TVirtualStringTree;
    Label98: TLabel;
    Label99: TLabel;
    btnSaveCustomMoney: TButton;
    Label100: TLabel;
    chkHumDrop: TCheckBox;
    procedure ListBoxitemListClick(Sender: TObject);
    procedure ButtonShopRefreshClick(Sender: TObject);
    procedure ButtonAddShopItemClick(Sender: TObject);
    procedure ButtonDelShopItemClick(Sender: TObject);
    procedure ListViewShop1Click(Sender: TObject);
    procedure ListBoxBoxItemClick(Sender: TObject);
    procedure btnAddBoxItemClick(Sender: TObject);
    procedure btnDelBoxItemClick(Sender: TObject);
    procedure btnSaveBoxItemClick(Sender: TObject);
    procedure ListBoxitemList1KeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure ListBoxitemListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure ButtonMsgFilterAddClick(Sender: TObject);
    procedure ButtonMsgFilterDelClick(Sender: TObject);
    procedure ButtonMsgFilterChgClick(Sender: TObject);
    procedure ListViewMsgFilterClick(Sender: TObject);
    procedure ListBoxItemList2Click(Sender: TObject);
    procedure ListBoxItemRuleListClick(Sender: TObject);
    procedure ButtonItemRuleSelAllClick(Sender: TObject);
    procedure ButtonItemRuleNotSelAllClick(Sender: TObject);
    procedure ButtonItemRuleAddClick(Sender: TObject);
    procedure ButtonItemRuleChgClick(Sender: TObject);
    procedure ButtonItemRuleAddAllClick(Sender: TObject);
    procedure ButtonItemRuleDelAllClick(Sender: TObject);
    procedure ButtonItemRuleDelClick(Sender: TObject);
    procedure ListBoxUserCommandClick(Sender: TObject);
    procedure ButtonUserCommandAddClick(Sender: TObject);
    procedure ButtonUserCommandDelClick(Sender: TObject);
    procedure ButtonGroupItemAddClick(Sender: TObject);
    procedure ListViewGroupItemListClick(Sender: TObject);
    procedure ButtonGroupItemDelClick(Sender: TObject);
    procedure ButtonGroupItemChgClick(Sender: TObject);
    procedure ButtonShopChgItemClick(Sender: TObject);
    procedure MenuItem_ShowAllClick(Sender: TObject);
    procedure btnWilAddClick(Sender: TObject);
    procedure btnWilNameUPClick(Sender: TObject);
    procedure btnWilNameDownClick(Sender: TObject);
    procedure ListBoxWilNameListClick(Sender: TObject);
    procedure btnWilSaveClick(Sender: TObject);
    procedure ButtonSendEffectImageListClick(Sender: TObject);
    procedure btnWilDelClick(Sender: TObject);
    procedure ButtonEffectAddClick(Sender: TObject);
    procedure ButtonEffectDelClick(Sender: TObject);
    procedure ButtonEffectChgClick(Sender: TObject);
    procedure PageControlShopChange(Sender: TObject);
    procedure lstEffectListClick(Sender: TObject);
    procedure ListBoxEffectItemListClick(Sender: TObject);
    procedure ComboBoxEffectIndexChange(Sender: TObject);
    procedure ListBoxStdItemList1Click(Sender: TObject);
    procedure ButtonEffectItemAddClick(Sender: TObject);
    procedure ButtonEffectItemChgClick(Sender: TObject);
    procedure ButtonEffectItemDelClick(Sender: TObject);
    procedure ButtonEffectItemSaveClick(Sender: TObject);
    procedure ListViewFoundryItemListClick(Sender: TObject);
    procedure ListViewFoundryNeedItemListClick(Sender: TObject);
    procedure ButtonFoundryItemAddClick(Sender: TObject);
    procedure ListBoxFoundryItemListClick(Sender: TObject);
    procedure ButtonFoundryItemChgClick(Sender: TObject);
    procedure ButtonFoundryItemDelClick(Sender: TObject);
    procedure ButtonFoundryNeedItemAddClick(Sender: TObject);
    procedure ButtonFoundryNeedItemChgClick(Sender: TObject);
    procedure ButtonFoundryNeedItemDelClick(Sender: TObject);
    procedure ButtonFoundryItemSaveClick(Sender: TObject);
    procedure CheckBox5Click(Sender: TObject);
    procedure CheckBox18Click(Sender: TObject);
    procedure ListBoxGiveItemClick(Sender: TObject);
    procedure ListBoxCenterItemClick(Sender: TObject);
    procedure ListBoxNoGiveItemClick(Sender: TObject);
    procedure ListBoxitemList1Click(Sender: TObject);
    procedure ButtonShopItemUPClick(Sender: TObject);
    procedure ButtonShopItemDOWNClick(Sender: TObject);
    procedure RadioButtonRateClick(Sender: TObject);
    procedure RadioButtonValueClick(Sender: TObject);
    procedure ButtonShopSaveItemClick(Sender: TObject);
    procedure ButtonMsgFilterSaveClick(Sender: TObject);
    procedure ButtonItemRuleSaveClick(Sender: TObject);
    procedure ButtonUserCommandSaveClick(Sender: TObject);
    procedure ButtonEffectSaveClick(Sender: TObject);
    procedure ButtonGroupItemSaveClick(Sender: TObject);
    procedure ListViewFilterItemClick(Sender: TObject);
    procedure ComboBoxItemFilterChange(Sender: TObject);
    procedure ButtonFilterAddClick(Sender: TObject);
    procedure ButtonFilterDelClick(Sender: TObject);
    procedure ButtonFilterChgClick(Sender: TObject);
    procedure ButtonFilterSaveClick(Sender: TObject);
    procedure MemoTzItemDescChange(Sender: TObject);
    procedure ButtonTzItemDescSaveClick(Sender: TObject);
    procedure MemoItemDescChange(Sender: TObject);
    procedure btnItemDescSaveClick(Sender: TObject);
    procedure ButtonFilterAddAllClick(Sender: TObject);
    procedure ButtonFilterDelAllClick(Sender: TObject);
    procedure ButtonGroupItemSkillPowerClick(Sender: TObject);
    procedure ListBoxitemList5Click(Sender: TObject);
    procedure ListBoxSkillPowerItemClick(Sender: TObject);
    procedure ButtonDelSkillPowerItemClick(Sender: TObject);
    procedure ButtonChgSkillPowerItemClick(Sender: TObject);
    procedure ButtonSaveSkillPowerItemClick(Sender: TObject);
    procedure ButtonAddSkillPowerItemClick(Sender: TObject);
    procedure StringGridSkillPowerSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
    procedure chkNextClick(Sender: TObject);
    procedure ListBoxEndNoGiveItemClick(Sender: TObject);
    procedure cbbOtherItemChange(Sender: TObject);
    procedure seCountChange(Sender: TObject);
    procedure seGoldChange(Sender: TObject);
    procedure seGameGoldChange(Sender: TObject);
    procedure seAddGoldChange(Sender: TObject);
    procedure seAddGameGoldChange(Sender: TObject);
    procedure seEndGoldChange(Sender: TObject);
    procedure seEndGameGoldChange(Sender: TObject);
    procedure chkSendFilterItemListClick(Sender: TObject);
    procedure chkSendItemDescListClick(Sender: TObject);
    procedure chkSendTzItemDescListClick(Sender: TObject);
    procedure chkSingleHintClick(Sender: TObject);
    procedure seBoxItemCountChange(Sender: TObject);
    procedure chkSkillPowerItemUseHumClick(Sender: TObject);
    procedure chkSkillPowerItemUseMonClick(Sender: TObject);
    procedure mniN3Click(Sender: TObject);
    procedure chkEnabledBuyShopItemGiveClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure lbl33Click(Sender: TObject);
    procedure Label85Click(Sender: TObject);
    procedure Label86Click(Sender: TObject);
    procedure Label87Click(Sender: TObject);
    procedure lbl33MouseEnter(Sender: TObject);
    procedure lbl33MouseLeave(Sender: TObject);
    procedure btnWilEditClick(Sender: TObject);
    procedure mmoItemDescTopChange(Sender: TObject);
    procedure chkSendItemDescTopListClick(Sender: TObject);
    procedure btnItemDescTopSaveClick(Sender: TObject);
    procedure Label94Click(Sender: TObject);
    procedure chkDescSupportRenamItemClick(Sender: TObject);
    procedure chkTZSupportRenameItemClick(Sender: TObject);
    procedure chkNoRenameDescReadDefaultClick(Sender: TObject);
    procedure ComboBoxFoundryNeedItemNameChange(Sender: TObject);
    procedure cbbAuctionPricesTypeChange(Sender: TObject);
    procedure seAuctionPrices_MinChange(Sender: TObject);
    procedure seAuctionPrices_MaxChange(Sender: TObject);
    procedure chkEnablePlayerUseClientPickItemsClick(Sender: TObject);
    procedure chkEnableHeroUseClientPickItemsClick(Sender: TObject);
    procedure N1Click(Sender: TObject);
    procedure vstCustomMoneyCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; out EditLink:
      IVTEditLink);
    procedure vstCustomMoneyGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
    procedure vstCustomMoneyGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure vstCustomMoneyNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
    procedure vstCustomMoneyEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed: Boolean);
    procedure vstCustomMoneyKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure btnSaveCustomMoneyClick(Sender: TObject);

  private
    FAcutionPricesLime: TAcutionItemPricesLime;

    boOpened: Boolean;
    boModValued: Boolean;
    procedure FilterItemListBox(MenuItem: TMenuItem; ListBox: TListBox);
    // procedure ModValue();
    procedure uModValue();
    procedure RefShopList();
    procedure RefBoxList(Source: Integer);
    procedure RefFilterMsgList();
    procedure RefItemRuleList();
    procedure RefUserCommandList();
    procedure RefGroupItemList();
    procedure RefItemEffectList();

    procedure RefFoundryItemList();
    procedure RefFoundryNeedItemList(FoundryItem: pTFoundryItem);

    procedure RefFilterItem(ItemType: TFilterItemType);

    procedure RefCustomMoneyList();
    procedure WMStartEditingCustomMoney(var Message: TMessage); message WM_STARTEDITING_CUSTOMMONEY;

    procedure OnItemRuleBathSettingClick(Sender: TObject);
  public { Public declarations }
    procedure Open;
  end;

var
  FrmViewList2: TFrmViewList2;
  SelGroupItem: pTGroupItem = nil;
  SelAttackSkillPercent: array[1..114] of Byte; // 技能威力攻击百分比
  SelDefenseSkillPercent: array[1..114] of Byte; // 技能威力防御百分比

implementation

uses
  M2Share, UsrEngn, HUtil32, SndaShop, Boxs, FilterTexts, ItemRules, UserCmds, ItemEffects, GroupItemSkillPowerConfig;

{$R *.dfm}

type
  PCustomMoneyNodeData = ^CustomMoneyNodeData;

  CustomMoneyNodeData = record
    Money: pTCustomMoney;
  end;

var
  SelShopListView: TListView = nil;
  SelShopItem: pTShopItem = nil;
  SelItemRule: pTItemRule = nil;
  SelFoundryItem: pTFoundryItem = nil;
  SelFoundryNeedItem: pTFoundryNeedItem = nil;
  SelBox: pTBox = nil;
  SelItemEffect: pTItemEffect = nil;
  SelSkillPowerItem: pTSkillPowerItem = nil;

function GetShopType(nType: Integer): string;
begin
  case nType of
    0:
      Result := '装饰';
    1:
      Result := '补给';
    2:
      Result := '强化';
    3:
      Result := '好友';
    4:
      Result := '限量';
    5:
      Result := '奇珍';
  end;
end;

function GetGameMoney(nType: Integer): string;
begin
  case nType of
    0:
      Result := g_Config.sGameGoldName;
    1:
      Result := sSTRING_GOLDNAME;
    2:
      Result := g_Config.sGamePointName;
    3:
      Result := g_Config.sGameDiamondName;
    4:
      Result := g_Config.sGameGirdName;
  end;
end;

(*
  procedure TfrmViewList2.ModValue;
  begin
  boModValued := True;
  ButtonWilSave.Enabled := True;
  // ButtonMsgFilterSave.Enabled := True;
  // ButtonEnableMakeSave.Enabled := True;
  end;
*)

procedure TFrmViewList2.uModValue;
begin
  boModValued := False;
  btnWilSave.Enabled := False;
  // ButtonMsgFilterSave.Enabled := False;
end;

procedure TFrmViewList2.vstCustomMoneyCreateEditor(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; out
  EditLink: IVTEditLink);
begin
  EditLink := TCustomPropertyEditLink.Create;
end;

procedure TFrmViewList2.vstCustomMoneyEditing(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; var Allowed:
  Boolean);
begin
  Allowed := Node <> nil;
end;

procedure TFrmViewList2.vstCustomMoneyGetNodeDataSize(Sender: TBaseVirtualTree; var NodeDataSize: Integer);
begin
  NodeDataSize := SizeOf(pTCustomMoney);
end;

procedure TFrmViewList2.vstCustomMoneyGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType:
  TVSTTextType; var CellText: string);
var
  NodeData: PCustomMoneyNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData <> nil then
  begin
    case Column of
      0:
        CellText := NodeData.Money.sName;
      1:
        CellText := NodeData.Money.nGroup.ToString;
      2:
        CellText := NodeData.Money.nIndex.ToString;
      3:
        CellText := IfThen(NodeData.Money.boLog, '记录日志', '不做记录');
      4:
        CellText := IfThen(NodeData.Money.boCanMyShop, '可以使用', '不可使用');
      5:
        CellText := IfThen(NodeData.Money.boCanGameShop, '可以使用', '不可使用');
      6:
        CellText := IfThen(NodeData.Money.boCanAuction, '可以使用', '不可使用');
      7:
        CellText := IfThen(NodeData.Money.boCanSellPlayer, '可以使用', '不可使用');
    end;
  end;
end;

procedure TFrmViewList2.vstCustomMoneyKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
var
  CustomMoney: pTCustomMoney;
  Node: PVirtualNode;
  NodeData: PCustomMoneyNodeData;
begin
  if (ssCtrl in Shift) then
  begin
    if Key = VK_INSERT then
    begin
      New(CustomMoney);
      CustomMoney.sName := '输入货币名称';
      CustomMoney.nGroup := 0;
      CustomMoney.nIndex := 0;
      CustomMoney.boLog := False;
      CustomMoney.boCanMyShop := False;
      CustomMoney.boCanGameShop := False;
      CustomMoney.boCanAuction := False;
      CustomMoney.boCanSellPlayer := False;

      g_CustomMoneyList.Add(CustomMoney);

      Node := vstCustomMoney.AddChild(nil);

      NodeData := vstCustomMoney.GetNodeData(Node);
      NodeData.Money := CustomMoney;

      vstCustomMoney.EditNode(Node, 0);
      btnSaveCustomMoney.Enabled := True;
    end
    else if Key = VK_DELETE then
    begin
      if vstCustomMoney.FocusedNode <> nil then
      begin
        NodeData := vstCustomMoney.GetNodeData(vstCustomMoney.FocusedNode);
        Node := vstCustomMoney.GetNext(vstCustomMoney.FocusedNode);
        if Node = nil then
          Node := vstCustomMoney.GetPrevious(vstCustomMoney.FocusedNode);

        if g_CustomMoneyList.IndexOf(NodeData.Money) <> -1 then
        begin
          g_CustomMoneyList.Delete(g_CustomMoneyList.IndexOf(NodeData.Money));
          Dispose(NodeData.Money);
          vstCustomMoney.DeleteNode(vstCustomMoney.FocusedNode);
          if Node <> nil then
          begin
            vstCustomMoney.FocusedNode := Node;
            vstCustomMoney.Selected[Node] := True;
          end;
          btnSaveCustomMoney.Enabled := True;
        end;
      end;
    end;
  end;
end;

procedure TFrmViewList2.vstCustomMoneyNodeClick(Sender: TBaseVirtualTree; const HitInfo: THitInfo);
begin
  if Assigned(HitInfo.HitNode) and (HitInfo.HitColumn >= 0) then
  begin
    PostMessage(Self.Handle, WM_STARTEDITING_CUSTOMMONEY, WPARAM(HitInfo.HitNode), HitInfo.HitColumn);
  end;
end;

procedure TFrmViewList2.RefFilterMsgList();
var
  I, nIndex: Integer;
  ListItem: TListItem;
  FilterMsg: pTFilterMsg;
begin
  nIndex := ListViewMsgFilter.ItemIndex;
  ListViewMsgFilter.Items.Clear;
  for I := 0 to g_FilterTexts.Count - 1 do
  begin
    FilterMsg := g_FilterTexts.Items[I];
    ListItem := ListViewMsgFilter.Items.Add;
    ListItem.Caption := FilterMsg.Msg;
    ListItem.SubItems.AddObject(FilterMsg.Replace, TObject(FilterMsg));
  end;
  if (nIndex >= 0) and (nIndex < ListViewMsgFilter.Items.Count) then
    ListViewMsgFilter.ItemIndex := nIndex;
end;

procedure TFrmViewList2.RefItemRuleList();
var
  I: Integer;
  ItemRule: pTItemRule;
begin
  ListBoxItemRuleList.Items.BeginUpdate;
  try
    ListBoxItemRuleList.Items.Clear;
    for I := 0 to g_ItemRules.Count - 1 do
    begin
      ItemRule := g_ItemRules.Items[I];
      ListBoxItemRuleList.Items.AddObject(ItemRule.ItemName, TObject(ItemRule));
    end;
  finally
    ListBoxItemRuleList.Items.EndUpdate;
  end;
end;

procedure TFrmViewList2.RefShopList();
var
  I, II: Integer;
  ListItem: TListItem;
  ItemList: TList;
  ShopItem: pTShopItem;
begin
  ListViewShop1.Items.Clear;
  ListViewShop2.Items.Clear;
  ListViewShop3.Items.Clear;
  ListViewShop4.Items.Clear;
  ListViewShop5.Items.Clear;
  ListViewShop6.Items.Clear;

  for I := 0 to 5 do
  begin
    ItemList := g_SndaShopList.Items[I];
    for II := 0 to ItemList.Count - 1 do
    begin
      ShopItem := pTShopItem(ItemList.Items[II]);

      case ShopItem.ShopType of
        0:
          ListItem := ListViewShop1.Items.Add;
        1:
          ListItem := ListViewShop2.Items.Add;
        2:
          ListItem := ListViewShop3.Items.Add;
        3:
          ListItem := ListViewShop4.Items.Add;
        4:
          ListItem := ListViewShop5.Items.Add;
        5:
          ListItem := ListViewShop6.Items.Add;
      else
        ListItem := nil;
      end;
      if ListItem <> nil then
      begin
        ListItem.Caption := GetShopType(I);
        ListItem.Data := ShopItem;
        ListItem.SubItems.AddObject(ShopItem.StdItem.Name, TObject(ShopItem));
        ListItem.SubItems.Add(IntToStr(ShopItem.ItemCount));
        ListItem.SubItems.Add(GetGameMoney(ShopItem.GameMoney));

        ListItem.SubItems.Add(IntToStr(ShopItem.StdItem.Price));
        ListItem.SubItems.Add(IntToStr(ShopItem.ImageIndex));
        ListItem.SubItems.Add(IntToStr(ShopItem.ImageCount));
        ListItem.SubItems.Add(ShopItem.Memo1);
        ListItem.SubItems.Add(ShopItem.Memo2);
      end;
    end;
  end;
end;

procedure TFrmViewList2.RefBoxList(Source: Integer);
var
  I: Integer;
  Box: pTBox;
  BoxItem: pTBoxItem;
begin
  ListBoxGiveItem.Items.Clear;
  ListBoxCenterItem.Items.Clear;
  ListBoxNoGiveItem.Items.Clear;
  ListBoxEndNoGiveItem.Items.Clear;
  Box := g_BoxsList.Find(Source);
  if Box <> nil then
  begin
    chkNext.Checked := Box.BoxSet.boNext;
    seGold.Value := Box.BoxSet.nGold;
    seGameGold.Value := Box.BoxSet.nGameGold;
    seAddGold.Value := Box.BoxSet.nAddGold;
    seAddGameGold.Value := Box.BoxSet.nAddGameGold;
    seEndGold.Value := Box.BoxSet.nEndGold;
    seEndGameGold.Value := Box.BoxSet.nEndGameGold;
    seCount.Value := Box.BoxSet.nCount;

    for I := 0 to Box.Give.Count - 1 do
    begin
      BoxItem := pTBoxItem(Box.Give.Items[I]);
      ListBoxGiveItem.Items.AddObject(BoxItem.ItemName, TObject(BoxItem));
    end;
    for I := 0 to Box.Center.Count - 1 do
    begin
      BoxItem := pTBoxItem(Box.Center.Items[I]);
      ListBoxCenterItem.Items.AddObject(BoxItem.ItemName, TObject(BoxItem));
    end;
    for I := 0 to Box.NoGive.Count - 1 do
    begin
      BoxItem := pTBoxItem(Box.NoGive.Items[I]);
      ListBoxNoGiveItem.Items.AddObject(BoxItem.ItemName, TObject(BoxItem));
    end;
    // 永不可得物品 piaoyun 2013-08-23
    for I := 0 to Box.EndNoGive.Count - 1 do
    begin
      BoxItem := pTBoxItem(Box.EndNoGive.Items[I]);
      ListBoxEndNoGiveItem.Items.AddObject(BoxItem.ItemName, TObject(BoxItem));
    end;
  end;
end;

procedure TFrmViewList2.RefUserCommandList();
var
  I: Integer;
begin
  ListBoxUserCommand.Items.Clear;
  for I := 0 to g_UserCmds.Count - 1 do
  begin
    ListBoxUserCommand.Items.AddObject(g_UserCmds.Strings[I], g_UserCmds.Objects[I]);
  end;
end;

procedure TFrmViewList2.RefGroupItemList();
var
  I, II, nIndex: Integer;
  ListItem: TListItem;
  GroupItem: pTGroupItem;
  sItemName: string;
begin
  ListViewGroupItemList.Items.BeginUpdate;
  try
    nIndex := ListViewGroupItemList.ItemIndex;
    ListViewGroupItemList.Items.Clear;
    if g_GroupItems <> nil then
    begin
      for I := 0 to g_GroupItems.Count - 1 do
      begin
        GroupItem := g_GroupItems.Items[I];
        ListItem := ListViewGroupItemList.Items.Add;
        ListItem.Caption := IntToStr(GroupItem.FLD_INDEX);
        ListItem.SubItems.AddObject(GroupItem.FLD_DESC, TObject(GroupItem));
        ListItem.SubItems.Add(IntToStr(GroupItem.FLD_COUNT));
        sItemName := '';
        for II := 0 to GroupItem.FLD_ITEMNAMES.Count - 1 do
        begin
          sItemName := sItemName + GroupItem.FLD_ITEMNAMES.Strings[II] + '|';
        end;
        ListItem.SubItems.Add(sItemName);
      end;
    end;
    if (nIndex >= 0) and (nIndex < ListViewGroupItemList.Items.Count) then
      ListViewGroupItemList.ItemIndex := nIndex;
  finally
    ListViewGroupItemList.Items.EndUpdate;
  end;
end;

procedure TFrmViewList2.RefItemEffectList();
var
  I, nIndex, nIndex1: Integer;
  ItemEffect: pTItemEffect;
begin
  nIndex := lstEffectList.ItemIndex;
  nIndex1 := ComboBoxEffectIndex.ItemIndex;
  lstEffectList.Items.Clear;
  ComboBoxEffectIndex.Items.Clear;

  for I := 0 to g_ItemEffects.Count - 1 do
  begin
    ItemEffect := g_ItemEffects.Items[I];
    lstEffectList.Items.AddObject('特效编号:' + IntToStr(ItemEffect.Index), TObject(ItemEffect));
    ComboBoxEffectIndex.Items.AddObject('特效编号:' + IntToStr(ItemEffect.Index), TObject(ItemEffect));
  end;

  if (nIndex >= 0) and (nIndex < ListBoxEffectItemList.Items.Count) then
    lstEffectList.ItemIndex := nIndex;

  if (nIndex1 >= 0) and (nIndex < ComboBoxEffectIndex.Items.Count) then
    ComboBoxEffectIndex.ItemIndex := nIndex1;
end;

procedure TFrmViewList2.RefFoundryItemList();
var
  I, nIndex: Integer;
  ListItem: TListItem;
  FoundryItem: pTFoundryItem;
begin
  ListViewFoundryItemList.Items.BeginUpdate;
  try
    nIndex := ListViewFoundryItemList.ItemIndex;
    ListViewFoundryItemList.Items.Clear;
    if g_FoundryItemList <> nil then
    begin
      for I := 0 to g_FoundryItemList.Count - 1 do
      begin
        FoundryItem := g_FoundryItemList.Items[I];
        ListItem := ListViewFoundryItemList.Items.Add;
        ListItem.Caption := IntToStr(I);
        ListItem.Data := FoundryItem;
        ListItem.SubItems.AddObject(FoundryItem.sItemName, TObject(FoundryItem));
        ListItem.SubItems.Add(IntToStr(FoundryItem.nItemCount));
        ListItem.SubItems.Add(IntToStr(FoundryItem.nItemRate));
      end;
    end;
    if (nIndex >= 0) and (nIndex < ListViewFoundryItemList.Items.Count) then
      ListViewFoundryItemList.ItemIndex := nIndex;
  finally
    ListViewFoundryItemList.Items.EndUpdate;
  end;
end;

procedure TFrmViewList2.RefFoundryNeedItemList(FoundryItem: pTFoundryItem);
var
  I: Integer;
  ListItem: TListItem;
  FoundryNeedItem: pTFoundryNeedItem;
begin
  ListViewFoundryNeedItemList.Items.Clear;
  if FoundryItem <> nil then
  begin
    for I := 0 to FoundryItem.ItemList.Count - 1 do
    begin
      FoundryNeedItem := FoundryItem.ItemList.Items[I];
      ListItem := ListViewFoundryNeedItemList.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.Data := FoundryNeedItem;
      ListItem.SubItems.AddObject(FoundryNeedItem.sItemName, TObject(FoundryNeedItem));
      ListItem.SubItems.Add(IntToStr(FoundryNeedItem.nItemCount));
      ListItem.SubItems.Add(IntToStr(FoundryNeedItem.btDelete));
    end;
  end;
end;

procedure TFrmViewList2.Open;
var
  I, MagicID: Integer;
  StdItem: pTStdItem;
  ItemEffect: pTItemEffect;
  Magic: pTMagic;
  CustomMagicConfig: TCustomMagicConfig;
begin
  PageControl.ActivePageIndex := 0;
  PageControlTzItem.ActivePageIndex := 0;
  RadioButtonRate.Checked := True;
  boOpened := False;
  uModValue();

  FillChar(FAcutionPricesLime, SizeOf(FAcutionPricesLime), 0);

  FillChar(SelAttackSkillPercent, SizeOf(SelAttackSkillPercent), 0); // 技能威力攻击百分比
  FillChar(SelDefenseSkillPercent, SizeOf(SelDefenseSkillPercent), 0); // 技能威力防御百分比

  ComboBoxGameMoney.Items.BeginUpdate;
  try
    ComboBoxGameMoney.Items.Clear;
    ComboBoxGameMoney.Items.Add(g_Config.sGameGoldName);
    ComboBoxGameMoney.Items.Add(sSTRING_GOLDNAME);
    ComboBoxGameMoney.Items.Add(g_Config.sGamePointName);
    ComboBoxGameMoney.Items.Add(g_Config.sGameDiamondName);
    ComboBoxGameMoney.Items.Add(g_Config.sGameGirdName);
  finally
    ComboBoxGameMoney.Items.EndUpdate;
  end;

  ListBoxitemList.Items.Clear;
  ListBoxitemList1.Items.Clear;
  ListBoxBoxItem.Items.Clear;
  ListBoxStdItemList1.Items.Clear;
  ListBoxFoundryItemList.Items.Clear;
  ListBoxWilNameList.Items.Clear;
  cbbEffectFileIndex1.Items.Clear;
  cbbEffectFileIndex2.Items.Clear;
  cbbEffectFileIndex3.Items.Clear;
  cbbEffectFileIndex5.Items.Clear;

  ListViewFoundryNeedItemList.Items.Clear;
  ListBoxFoundryItemList.Items.Clear;
  cbbAddEffectFileIndex.Items.Clear;

  ComboBoxFoundryNeedItemName.Items.Clear;
  ComboBoxFoundryNeedItemName.Items.Add(sSTRING_GOLDNAME);
  ComboBoxFoundryNeedItemName.Items.Add(g_Config.sGameGoldName);
  ComboBoxFoundryNeedItemName.Items.Add(g_Config.sCreditPointName);
  ComboBoxFoundryNeedItemName.Items.Add(g_Config.sGamePointName);
  ComboBoxFoundryNeedItemName.Items.Add(g_Config.sGameGirdName);
  ComboBoxFoundryNeedItemName.Items.Add(g_Config.sGameDiamondName);

  ListBoxitemList.Items.BeginUpdate;
  ListBoxitemList1.Items.BeginUpdate;
  ListBoxItemList2.Items.BeginUpdate;
  ListBoxStdItemList1.Items.BeginUpdate;
  ListBoxFoundryItemList.Items.BeginUpdate;
  ListBoxitemList4.Items.BeginUpdate;
  ListBoxitemList5.Items.BeginUpdate;
  ListBoxBoxItem.Items.BeginUpdate;
  try
    if g_MultiThreadRun then
      UserEngine.StdItemList.LockR(15);
    try
      for I := 0 to UserEngine.StdItemList.Count - 1 do
      begin
        StdItem := UserEngine.StdItemList.Items[I];
        ListBoxitemList.Items.AddObject(StdItem.Name, TObject(StdItem));
        ListBoxitemList1.Items.AddObject(StdItem.Name, TObject(StdItem));
        ListBoxItemList2.Items.AddObject(StdItem.Name, TObject(StdItem));
        ListBoxStdItemList1.Items.AddObject(StdItem.Name, TObject(StdItem));
        ListBoxFoundryItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
        ListBoxitemList4.Items.AddObject(StdItem.Name, TObject(StdItem));
        ListBoxitemList5.Items.AddObject(StdItem.Name, TObject(StdItem));
        if (StdItem.StdMode = 31) and (StdItem.Shape in [15..49]) then
          ListBoxBoxItem.Items.AddObject(StdItem.Name, TObject(StdItem));
      end;
    finally
      if g_MultiThreadRun then
        UserEngine.StdItemList.UnLockR;
    end;
  finally
    ListBoxitemList.Items.EndUpdate;
    ListBoxitemList1.Items.EndUpdate;
    ListBoxItemList2.Items.EndUpdate;
    ListBoxStdItemList1.Items.EndUpdate;
    ListBoxFoundryItemList.Items.EndUpdate;
    ListBoxitemList4.Items.EndUpdate;
    ListBoxitemList5.Items.EndUpdate;
    ListBoxBoxItem.Items.EndUpdate;
  end;

  ListBoxSkillPowerItem.Clear;
  g_SkillPowerItemList.Lock;
  try
    ListBoxSkillPowerItem.Items.AddStrings(g_SkillPowerItemList);
  finally
    g_SkillPowerItemList.UnLock;
  end;

  ListBoxWilNameList.Items.Text := g_EffectImageList.Text;

  cbbEffectFileIndex1.Items.Add('关闭特效');
  cbbEffectFileIndex2.Items.Add('关闭特效');
  cbbEffectFileIndex3.Items.Add('关闭特效');
  cbbAddEffectFileIndex.Items.Add('关闭特效');
  cbbEffectFileIndex5.Items.Add('关闭特效');

  cbbEffectFileIndex1.Items.AddStrings(g_EffectImageList);
  cbbEffectFileIndex2.Items.AddStrings(g_EffectImageList);
  cbbEffectFileIndex3.Items.AddStrings(g_EffectImageList);
  cbbAddEffectFileIndex.Items.AddStrings(g_EffectImageList);
  cbbEffectFileIndex5.Items.AddStrings(g_EffectImageList);

  ListBoxEffectItemList.Items.BeginUpdate;
  try
    for I := g_EffectItemList.Count - 1 downto 0 do
    begin
      if (Integer(g_EffectItemList.Objects[I]) > 0) then
      begin
        ItemEffect := g_ItemEffects.Get(Integer(g_EffectItemList.Objects[I]));
        if ItemEffect <> nil then
        begin
          ListBoxEffectItemList.Items.AddObject(g_EffectItemList.Strings[I], TObject(ItemEffect));
        end
        else
        begin
          g_EffectItemList.Delete(I);
        end;
      end;
    end;
  finally
    ListBoxEffectItemList.Items.EndUpdate;
  end;

  ButtonFoundryItemAdd.Enabled := False;
  ButtonFoundryItemChg.Enabled := False;
  ButtonFoundryItemDel.Enabled := False;
  ButtonFoundryItemSave.Enabled := False;

  ButtonFoundryNeedItemAdd.Enabled := False;
  ButtonFoundryNeedItemChg.Enabled := False;
  ButtonFoundryNeedItemDel.Enabled := False;

  btnAddBoxItem.Enabled := False;
  btnDelBoxItem.Enabled := False;
  btnSaveBoxItem.Enabled := False;

  ButtonAddSkillPowerItem.Enabled := False;
  ButtonChgSkillPowerItem.Enabled := False;
  ButtonDelSkillPowerItem.Enabled := False;
  ButtonSaveSkillPowerItem.Enabled := False;

  RadioButtonValue.Checked := g_Config.boGroupItemRule;

  chkSkillPowerItemUseHum.Checked := g_Config.boSkillPowerItemUseHum;
  chkSkillPowerItemUseMon.Checked := g_Config.boSkillPowerItemUseMon;

  RefShopList();
  RefFilterMsgList();
  RefItemRuleList();
  RefUserCommandList();
  RefGroupItemList();
  RefItemEffectList();
  RefFoundryItemList();

  ButtonEffectChg.Enabled := False;
  ButtonEffectDel.Enabled := False;
  ButtonEffectItemChg.Enabled := False;
  ButtonEffectItemDel.Enabled := False;
  ButtonEffectItemSave.Enabled := False;

  ButtonDelShopItem.Enabled := False;
  ButtonShopChgItem.Enabled := False;
  ButtonShopSaveItem.Enabled := False;

  ButtonMsgFilterChg.Enabled := False;
  ButtonMsgFilterDel.Enabled := False;
  ButtonMsgFilterSave.Enabled := False;

  ButtonItemRuleChg.Enabled := False;
  ButtonItemRuleDel.Enabled := False;
  ButtonItemRuleSave.Enabled := False;

  ButtonUserCommandDel.Enabled := False;
  ButtonUserCommandSave.Enabled := False;

  ButtonEffectChg.Enabled := False;
  ButtonEffectDel.Enabled := False;
  ButtonEffectSave.Enabled := False;

  chkEnabledBuyShopItemGive.Checked := g_Config.boEnabledBuyShopItemGive;

  MemoTzItemDesc.Lines.Text := g_TzItemDescList.Text;
  MemoItemDesc.Lines.Text := g_ItemDescList.Text;
  mmoItemDescTop.Lines.Text := g_ItemDescTopList.Text;

  btnItemDescSave.Enabled := False;
  btnItemDescTopSave.Enabled := False;
  ButtonTzItemDescSave.Enabled := False;

  RefFilterItem(i_All);

  RefCustomMoneyList;

  ButtonFilterChg.Enabled := False;
  ButtonFilterDel.Enabled := False;
  ButtonFilterSave.Enabled := False;

  chkSendFilterItemList.Checked := g_Config.boSendFilterItemList;
  chkSendItemDescList.Checked := g_Config.boSendItemDescList;
  chkSendItemDescTopList.Checked := g_Config.boSendItemDescTopList;
  chkSendTzItemDescList.Checked := g_Config.boSendTzItemDescList;
  chkSingleHint.Checked := g_Config.boSingleHint;

{$IF NEED_KEY <> 2}
  // VMProtectBegin('VMProtect_UseClientPickItems');

  chkEnablePlayerUseClientPickItems.Enabled := False;
  chkEnableHeroUseClientPickItems.Enabled := False;

  if g_nKey_UseClientPickItems = 1 then
  begin
    chkEnablePlayerUseClientPickItems.Enabled := True;
    chkEnableHeroUseClientPickItems.Enabled := True;

    chkEnablePlayerUseClientPickItems.Checked := g_Config.boEnablePlayerUseClientPickItems;
    chkEnableHeroUseClientPickItems.Checked := g_Config.boEnableHeroUseClientPickItems;
  end;

  // VMProtectEnd();

{$ELSE}
  chkEnablePlayerUseClientPickItems.Enabled := True;
  chkEnableHeroUseClientPickItems.Enabled := True;

  chkEnablePlayerUseClientPickItems.Checked := g_Config.boEnablePlayerUseClientPickItems;
  chkEnableHeroUseClientPickItems.Checked := g_Config.boEnableHeroUseClientPickItems;
{$IFEND}
  chkTZSupportRenameItem.Checked := g_Config.boTZSupportRenameItem;
  chkDescSupportRenamItem.Checked := g_Config.boDescSupportRenamItem;
  chkNoRenameDescReadDefault.Checked := g_Config.boNoRenameDescReadDefault;

  StringGridSkillPower.Cells[0, 0] := '技能名称';
  StringGridSkillPower.Cells[1, 0] := '增加技能伤害百分比';
  StringGridSkillPower.Cells[2, 0] := '增加技能防御百分比';

  StringGridSkillPower.RowCount := DEF_MAGIC_COUNT + CUSTOM_MAGIC_COUNT;

  for I := 0 to DEF_MAGIC_COUNT - 1 do
  begin
    Magic := UserEngine.FindMagic(I + 1, mtHum);
    if Magic = nil then
    begin
      Magic := UserEngine.FindMagic(I + 1, mtContinuous);
    end;

    if (I >= 59) and (I <= 64) and (Magic = nil) then
    begin
      Magic := UserEngine.FindMagic(I + 1, mtHero);
    end;

    if Magic <> nil then
      StringGridSkillPower.Cells[0, I + 1] := Magic.sMagicName
    else
      StringGridSkillPower.Cells[0, I + 1] := '';
    StringGridSkillPower.Cells[1, I + 1] := '0';
    StringGridSkillPower.Cells[2, I + 1] := '0';
    if (Magic <> nil) and (not IsValidMagicInSkillPowerItem(Magic.wMagicId)) then
      StringGridSkillPower.Cells[0, I + 1] := StringGridSkillPower.Cells[0, I + 1] + '[无效]';
  end;

  for I := DEF_MAGIC_COUNT to StringGridSkillPower.RowCount - 1 do
  begin
    MagicID := CUSTOM_MAGIC_START_ID + I - DEF_MAGIC_COUNT;
    Magic := UserEngine.FindMagic(MagicID, mtHum);

    // if Magic = nil then
    // Magic := UserEngine.FindMagic(I + 1, mtContinuous);

    if Magic <> nil then
      StringGridSkillPower.Cells[0, I + 1] := Magic.sMagicName
    else
      StringGridSkillPower.Cells[0, I + 1] := '';

    StringGridSkillPower.Cells[1, I + 1] := '0';
    StringGridSkillPower.Cells[2, I + 1] := '0';

    if (Magic <> nil) then
    begin
      CustomMagicConfig := GetCustomMagicConfig(MagicID);
      if (CustomMagicConfig <> nil) and (CustomMagicConfig.ServerConfig.OperateMode = momProtect) then
      begin
        StringGridSkillPower.Cells[0, I + 1] := StringGridSkillPower.Cells[0, I + 1] + '[无效]';
      end;
    end;
  end;

  boOpened := True;
  ShowModal;
end;

procedure TFrmViewList2.ListBoxitemListClick(Sender: TObject);
begin
  EditShopItemName.Text := ListBoxitemList.Items.Strings[ListBoxitemList.ItemIndex];
end;

procedure TFrmViewList2.ButtonShopRefreshClick(Sender: TObject);
begin
  g_SndaShopList.LoadFromFile;
  RefShopList();
end;

procedure TFrmViewList2.ButtonAddShopItemClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
  sMemo: string;
  nPrice: Integer;
  ShopItem: pTShopItem;
  StdItem: pTStdItem;
  nCount: Integer;
begin
  sItemName := Trim(EditShopItemName.Text);

  nPrice := EditShopItemPrice.Value;
  sMemo := Trim(EditItemMemo1.Text);
  if not (PageControlShop.ActivePageIndex in [0..5]) then
  begin
    Application.MessageBox('请选择物品类别！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;
  StdItem := UserEngine.GetStdItem(sItemName);
  if (sItemName = '') or (StdItem = nil) then
  begin
    Application.MessageBox('请选择一个正确的物品！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;
  if ComboBoxGameMoney.ItemIndex < 0 then
  begin
    Application.MessageBox('请选择一个正确的交易货币！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;
  if sMemo = '' then
  begin
    Application.MessageBox('请输入物品功能！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;
  if nPrice <= 0 then
  begin
    Application.MessageBox('请输入正确的物品价格！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;
  if g_SndaShopList.Get(sItemName) <> nil then
  begin
    Application.MessageBox('该物品已经在列表中了！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end;
  New(ShopItem);
  FillChar(ShopItem^, SizeOf(TShopItem), #0);
  nCount := _MIN(MemoShop.Lines.Count, 6);
  ShopItem.ShopType := PageControlShop.ActivePageIndex;
  // Showmessage(inttostr(PageControlShop.ActivePageIndex));
  ShopItem.StdItem := StdItem^;
  ShopItem.StdItem.Price := nPrice;
  ShopItem.GameMoney := ComboBoxGameMoney.ItemIndex;
  ShopItem.ImageIndex := EditImageIndex.Value;
  ShopItem.ImageCount := EditImageCount.Value;
  ShopItem.Memo1 := sMemo;
  ShopItem.ItemCount := seItemCount.Value;
  ShopItem.boBulkBuy := chkBulkBuy.Checked;
  ShopItem.nBulkBuyCount := seBulkBuyCount.Value;
  seBulkBuyCount.Enabled := ShopItem.boBulkBuy;

  sMemo := '';
  for I := 0 to nCount - 1 do
  begin
    sMemo := sMemo + MemoShop.Lines.Strings[I] + #13#10;
  end;
  ShopItem.Memo2 := sMemo;
  // ShopItem.Memo2 := MemoShop.Lines.Text;
  if g_SndaShopList.Add(ShopItem) then
  begin
    RefShopList;
    ButtonShopSaveItem.Enabled := True;
    // Application.MessageBox('增加成功！', '提示信息', MB_OK + MB_ICONWARNING);
  end
  else
  begin
    Dispose(ShopItem);
    Application.MessageBox('增加失败！', '错误信息', MB_OK + MB_ICONERROR);
  end;
end;

procedure TFrmViewList2.ButtonDelShopItemClick(Sender: TObject);
begin
  if SelShopItem <> nil then
  begin
    if g_SndaShopList.Delete(SelShopItem) then
    begin
      SelShopItem := nil;
      ButtonShopSaveItem.Enabled := True;
      ButtonShopChgItem.Enabled := False;
      RefShopList;
    end;
  end;
end;

procedure TFrmViewList2.ListViewShop1Click(Sender: TObject);
var
  ListItem: TListItem;
begin
  SelShopListView := TListView(Sender);
  ListItem := SelShopListView.Selected;
  if ListItem <> nil then
  begin
    SelShopItem := ListItem.Data; // pTShopItem(ListItem.SubItems.Objects[0]);
    ComboBoxShopType.ItemIndex := SelShopItem.ShopType;
    ComboBoxGameMoney.ItemIndex := SelShopItem.GameMoney;
    EditShopItemName.Text := SelShopItem.StdItem.Name;
    EditShopItemPrice.Value := SelShopItem.StdItem.Price;
    EditImageIndex.Value := SelShopItem.ImageIndex;
    EditImageCount.Value := SelShopItem.ImageCount;
    EditItemMemo1.Text := SelShopItem.Memo1;
    MemoShop.Lines.Text := SelShopItem.Memo2;
    seItemCount.Value := SelShopItem.ItemCount;
    chkBulkBuy.Checked := SelShopItem.boBulkBuy;
    seBulkBuyCount.Value := SelShopItem.nBulkBuyCount;

    seBulkBuyCount.Enabled := SelShopItem.boBulkBuy;

    ButtonDelShopItem.Enabled := True;
    ButtonShopChgItem.Enabled := True;
    ButtonShopItemDOWN.Enabled := (SelShopListView.ItemIndex >= 0) and (SelShopListView.ItemIndex < SelShopListView.Items.Count -
      1);
    ButtonShopItemUP.Enabled := (SelShopListView.ItemIndex > 0);
  end
  else
  begin
    SelShopItem := nil;
    SelShopListView := nil;
    ButtonDelShopItem.Enabled := False;
    ButtonShopChgItem.Enabled := False;
  end;
end;

procedure TFrmViewList2.ListBoxBoxItemClick(Sender: TObject);
begin
  if (ListBoxBoxItem.ItemIndex >= 0) then
  begin
    SelBox := g_BoxsList.Find(pTStdItem(ListBoxBoxItem.Items.Objects[ListBoxBoxItem.ItemIndex]).Source);
    if SelBox <> nil then
    begin
      GroupBoxBoxItem.Caption := SelBox.Name;
      RefBoxList(SelBox.Source);
      btnAddBoxItem.Enabled := True;
      btnDelBoxItem.Enabled := False;
    end
    else
    begin
      SelBox := nil;
      btnAddBoxItem.Enabled := False;
      btnDelBoxItem.Enabled := False;
    end;
  end
  else
  begin
    SelBox := nil;
    btnAddBoxItem.Enabled := False;
    btnDelBoxItem.Enabled := False;
  end;
end;

procedure TFrmViewList2.btnAddBoxItemClick(Sender: TObject);
var
  sItemName: string;
  BoxItem: pTBoxItem;
  BoxList: TBoxList;
  nItemCount: Integer;
begin
  if SelBox = nil then
  begin
    Application.MessageBox('请在宝箱列表中选择一个物品！', '错误信息', MB_OK + MB_ICONERROR);
    ListBoxBoxItem.SetFocus;
    Exit;
  end;

  // 宝箱物品支持物品数量 +  chongchong 2014-01-06
  nItemCount := seBoxItemCount.Value;

  if (Trim(edtBoxItemName.Text) <> '') and (Trim(edtBoxItemName.Text) = cbbOtherItem.Items[cbbOtherItem.ItemIndex]) then
  begin
    // 添加特殊物品
    sItemName := Trim(edtBoxItemName.Text);

    // 宝箱物品支持物品数量 -  chongchong 2014-01-06
    // nItemCount := seBoxItemCount.Value;
  end
  else
  begin
    if ListBoxitemList1.ItemIndex < 0 then
    begin
      Application.MessageBox('请在物品列表中选择一个物品！', '错误信息', MB_OK + MB_ICONERROR);
      ListBoxitemList1.SetFocus;
      Exit;
    end;
    sItemName := Trim(ListBoxitemList1.Items.Strings[ListBoxitemList1.ItemIndex]);
  end;

  if not (ComboBoxBoxItemType.ItemIndex in [0..3]) then
  begin
    Application.MessageBox('请选择物品种类！', '错误信息', MB_OK + MB_ICONERROR);
    ComboBoxBoxItemType.SetFocus;
    Exit;
  end;
  BoxList := nil;

  case ComboBoxBoxItemType.ItemIndex of
    0:
      BoxList := SelBox.Give; // 可得
    1:
      BoxList := SelBox.NoGive; // 不可得
    2:
      BoxList := SelBox.Center; // 中间一格
    3:
      BoxList := SelBox.EndNoGive; // 永不可得
  end;
  if BoxList = nil then
    Exit;

  if BoxList.GetByName(sItemName, nItemCount) <> nil then
  begin
    Application.MessageBox('该物品已经在列表中了！', '错误信息', MB_OK + MB_ICONERROR);
    edtBoxItemName.SetFocus;
    Exit;
  end;

  New(BoxItem);
  BoxItem.ItemName := sItemName;
  BoxItem.ItemCount := nItemCount;
  BoxList.Add(BoxItem);
  RefBoxList(SelBox.Source);
  btnSaveBoxItem.Enabled := True;
end;

procedure TFrmViewList2.btnDelBoxItemClick(Sender: TObject);
var
  sItemName: string;
  nItemCount: Integer;
  BoxItem: pTBoxItem;
  BoxList: TBoxList;
  // Box: pTBox;
begin
  if not (ComboBoxBoxItemType.ItemIndex in [0..3]) then
  begin
    Application.MessageBox('请选择物品种类！', '错误信息', MB_OK + MB_ICONERROR);
    ComboBoxBoxItemType.SetFocus;
    Exit;
  end;
  if SelBox = nil then
  begin
    Application.MessageBox('请选择一个宝箱！', '错误信息', MB_OK + MB_ICONERROR);
    ListBoxBoxItem.SetFocus;
    Exit;
  end;
  BoxList := nil;
  sItemName := '';
  BoxItem := nil;
  nItemCount := 0;
  case ComboBoxBoxItemType.ItemIndex of
    0: // 可得
      begin
        BoxList := SelBox.Give;
        if ListBoxGiveItem.ItemIndex >= 0 then
          // sItemName := Trim(ListBoxGiveItem.Items.Strings[ListBoxGiveItem.ItemIndex]);
          BoxItem := pTBoxItem(ListBoxGiveItem.Items.Objects[ListBoxGiveItem.ItemIndex]);
      end;
    1: // 不可得
      begin
        BoxList := SelBox.NoGive;
        if ListBoxNoGiveItem.ItemIndex >= 0 then
          BoxItem := pTBoxItem(ListBoxNoGiveItem.Items.Objects[ListBoxNoGiveItem.ItemIndex]);
      end;
    2: // 中间一格
      begin
        BoxList := SelBox.Center;
        if ListBoxCenterItem.ItemIndex >= 0 then
          // sItemName := Trim(ListBoxCenterItem.Items.Strings[ListBoxCenterItem.ItemIndex]);
          BoxItem := pTBoxItem(ListBoxCenterItem.Items.Objects[ListBoxCenterItem.ItemIndex]);
      end;
    3: // 永不可得
      begin
        BoxList := SelBox.EndNoGive;
        if ListBoxEndNoGiveItem.ItemIndex >= 0 then
          // sItemName := Trim(ListBoxEndNoGiveItem.Items.Strings[ListBoxEndNoGiveItem.ItemIndex]);
          BoxItem := pTBoxItem(ListBoxEndNoGiveItem.Items.Objects[ListBoxEndNoGiveItem.ItemIndex]);
      end;
  end;

  if BoxItem <> nil then
  begin
    sItemName := BoxItem.ItemName;
    nItemCount := BoxItem.ItemCount;
  end;

  if sItemName = '' then
  begin
    Application.MessageBox('请选择要删除的物品！', '错误信息', MB_OK + MB_ICONERROR);
    Exit;
  end
  else
  begin
    BoxList.DeleteByName(sItemName, nItemCount);
    RefBoxList(SelBox.Source);
    btnSaveBoxItem.Enabled := True;
    btnDelBoxItem.Enabled := False;
  end;
end;

procedure TFrmViewList2.btnSaveBoxItemClick(Sender: TObject);
begin
  g_BoxsList.SaveToFile;
  btnSaveBoxItem.Enabled := False;
end;

procedure TFrmViewList2.btnSaveCustomMoneyClick(Sender: TObject);
begin
  SaveCustomMoney();
  btnSaveCustomMoney.Enabled := False;
end;

procedure TFrmViewList2.ListBoxitemList1KeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
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
          for I := 0 to ListBoxitemList1.Items.Count - 1 do
          begin
            if ListBoxitemList1.Items.Strings[I] = sItemName then
            begin
              ListBoxitemList1.ItemIndex := I;
              Break;
            end;
          end;
        end;
      end;
  end;
end;

procedure TFrmViewList2.ListBoxitemListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
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

procedure TFrmViewList2.ButtonMsgFilterAddClick(Sender: TObject);
var
  sFilterMsg, sNewFilterMsg: string;
begin
  sFilterMsg := Trim(EditFilterMsg.Text);
  sNewFilterMsg := Trim(EditNewMsg.Text);
  if sFilterMsg = '' then
  begin
    Application.MessageBox('请输入过滤消息！', '错误信息', MB_OK + MB_ICONERROR);
    EditFilterMsg.SetFocus;
    Exit;
  end;
  if g_FilterTexts.Find(sFilterMsg) then
  begin
    Application.MessageBox('此过滤消息已经存在！', '错误信息', MB_OK + MB_ICONERROR);
    EditFilterMsg.SetFocus;
    Exit;
  end;
  if g_FilterTexts.Add(sFilterMsg, sNewFilterMsg) then
  begin
    RefFilterMsgList;
    ButtonMsgFilterSave.Enabled := True;
  end
  else
  begin
    Application.MessageBox('增加失败！', '错误信息', MB_OK + MB_ICONERROR);
  end;
end;

procedure TFrmViewList2.ButtonMsgFilterDelClick(Sender: TObject);
var
  ListItem: TListItem;
  FilterMsg: pTFilterMsg;
begin
  ListItem := ListViewMsgFilter.Selected;
  if ListItem <> nil then
  begin
    FilterMsg := pTFilterMsg(ListItem.SubItems.Objects[0]);
    if g_FilterTexts.Delete(FilterMsg.Msg) then
    begin
      RefFilterMsgList;
      ButtonMsgFilterChg.Enabled := False;
      ButtonMsgFilterDel.Enabled := False;
      ButtonMsgFilterSave.Enabled := True;
    end
    else
    begin
      Application.MessageBox('删除失败！', '错误信息', MB_OK + MB_ICONERROR);
    end;
  end;
end;

procedure TFrmViewList2.ButtonMsgFilterChgClick(Sender: TObject);
var
  ListItem: TListItem;
  FilterMsg: pTFilterMsg;
  sFilterMsg, sNewFilterMsg: string;
begin
  sFilterMsg := Trim(EditFilterMsg.Text);
  sNewFilterMsg := Trim(EditNewMsg.Text);
  if g_FilterTexts.Find(sFilterMsg) then
  begin
    ListItem := ListViewMsgFilter.Selected;
    if ListItem <> nil then
    begin
      FilterMsg := pTFilterMsg(ListItem.SubItems.Objects[0]);
      FilterMsg.Replace := sNewFilterMsg;
      RefFilterMsgList;
      ButtonMsgFilterSave.Enabled := True;
    end;
  end;
end;

procedure TFrmViewList2.ListViewMsgFilterClick(Sender: TObject);
var
  ListItem: TListItem;
  FilterMsg: pTFilterMsg;
begin
  ListItem := ListViewMsgFilter.Selected;
  if ListItem <> nil then
  begin
    FilterMsg := pTFilterMsg(ListItem.SubItems.Objects[0]);
    EditFilterMsg.Text := FilterMsg.Msg;
    EditNewMsg.Text := FilterMsg.Replace;
    ButtonMsgFilterChg.Enabled := True;
    ButtonMsgFilterDel.Enabled := True;
  end
  else
  begin
    ButtonMsgFilterChg.Enabled := False;
    ButtonMsgFilterDel.Enabled := False;
  end;
end;

procedure TFrmViewList2.ListBoxItemList2Click(Sender: TObject);
begin
  ButtonMsgFilterChg.Enabled := False;
  ButtonMsgFilterDel.Enabled := False;

  if ListBoxItemList2.ItemIndex >= 0 then
    EditRuleItemName.Text := ListBoxItemList2.Items.Strings[ListBoxItemList2.ItemIndex];
end;

procedure TFrmViewList2.ListBoxItemRuleListClick(Sender: TObject);
var
  I: Integer;
  CheckBox: TCheckBox;
begin
  if ListBoxItemRuleList.ItemIndex >= 0 then
  begin
    SelItemRule := pTItemRule(ListBoxItemRuleList.Items.Objects[ListBoxItemRuleList.ItemIndex]);
    EditRuleItemName.Text := SelItemRule.ItemName;
    for I := 0 to GroupBoxItemRule.ControlCount - 1 do
    begin
      if GroupBoxItemRule.Controls[I] is TCheckBox then
      begin
        CheckBox := TCheckBox(GroupBoxItemRule.Controls[I]);
        if CheckBox.Tag >= 0 then
          CheckBox.Checked := SelItemRule.FlagArray[CheckBox.Tag];
      end;
    end;

    cbbAuctionPricesType.ItemIndex := 0;

    FAcutionPricesLime := SelItemRule.PricesLime;

    seAuctionPrices_Min.Value := FAcutionPricesLime.Min[cbbAuctionPricesType.ItemIndex];
    seAuctionPrices_Max.Value := FAcutionPricesLime.Max[cbbAuctionPricesType.ItemIndex];

    ButtonItemRuleChg.Enabled := True;
    ButtonItemRuleDel.Enabled := True;
  end
  else
  begin
    SelItemRule := nil;
    ButtonItemRuleChg.Enabled := False;
    ButtonItemRuleDel.Enabled := False;
  end;
end;

procedure TFrmViewList2.ButtonItemRuleSelAllClick(Sender: TObject);
var
  I: Integer;
  CheckBox: TCheckBox;
begin
  for I := 0 to GroupBoxItemRule.ControlCount - 1 do
  begin
    if GroupBoxItemRule.Controls[I] is TCheckBox then
    begin
      CheckBox := TCheckBox(GroupBoxItemRule.Controls[I]);
      if CheckBox.Enabled then
        CheckBox.Checked := True;
    end;
  end;
end;

procedure TFrmViewList2.ButtonItemRuleNotSelAllClick(Sender: TObject);
var
  I: Integer;
  CheckBox: TCheckBox;
begin
  for I := 0 to GroupBoxItemRule.ControlCount - 1 do
  begin
    if GroupBoxItemRule.Controls[I] is TCheckBox then
    begin
      CheckBox := TCheckBox(GroupBoxItemRule.Controls[I]);
      if CheckBox.Enabled then
        CheckBox.Checked := False;
    end;
  end;
end;

procedure TFrmViewList2.ButtonItemRuleAddClick(Sender: TObject);
var
  I, J: Integer;
  FlagArray: TFlagArray;
  CheckBox: TCheckBox;
  sItemName: string;
  ItemRule: pTItemRule;
begin
  for J := 0 to ListBoxItemList2.Items.Count - 1 do
  begin
    if not ListBoxItemList2.Selected[J] then
      Continue;
    sItemName := ListBoxItemList2.Items[J];
    if sItemName = '' then
    begin
      Application.MessageBox('请输入物品名称！', '错误信息', MB_OK + MB_ICONERROR);
      EditRuleItemName.SetFocus;
      Exit;
    end;

    if g_ItemRules.Find(sItemName) <> nil then
    begin
      Application.MessageBox(PChar('物品 ' + sItemName + ' 已经存在！'), '错误信息', MB_OK + MB_ICONERROR);
      EditRuleItemName.SetFocus;
      Continue;
    end;

    for I := 0 to Length(FAcutionPricesLime.Min) - 1 do
    begin
      if (FAcutionPricesLime.Min[I] <> 0) and (FAcutionPricesLime.Max[I] <> 0) and (FAcutionPricesLime.Min[I] > FAcutionPricesLime.Max
        [I]) then
      begin
        cbbAuctionPricesType.ItemIndex := I;
        Application.MessageBox('拍卖最低价不能大于最高价！', '错误信息', MB_OK + MB_ICONERROR);

        seAuctionPrices_Min.Value := FAcutionPricesLime.Min[I];
        seAuctionPrices_Max.Value := FAcutionPricesLime.Max[I];

        seAuctionPrices_Max.SetFocus;
        Exit;
      end;
    end;

    FillChar(FlagArray, SizeOf(TFlagArray), 0);
    for I := 0 to GroupBoxItemRule.ControlCount - 1 do
    begin
      if GroupBoxItemRule.Controls[I] is TCheckBox then
      begin
        CheckBox := TCheckBox(GroupBoxItemRule.Controls[I]);
        if CheckBox.Tag >= 0 then
          FlagArray[CheckBox.Tag] := CheckBox.Checked;
      end;
    end;

    ItemRule := g_ItemRules.Add(sItemName, FlagArray, @FAcutionPricesLime);
    if ItemRule <> nil then
    begin
      ListBoxItemRuleList.Items.AddObject(ItemRule.ItemName, TObject(ItemRule));
      ButtonItemRuleSave.Enabled := True;
      SelItemRule := ItemRule;
      ListBoxItemRuleList.ItemIndex := ListBoxItemRuleList.Items.Count - 1;
      ListBoxItemRuleList.ClearSelection;
      ListBoxItemRuleList.Selected[ListBoxItemRuleList.ItemIndex] := True;
    end
    else
    begin
      Application.MessageBox('增加失败！', '错误信息', MB_OK + MB_ICONERROR);
    end;
  end;
end;

procedure TFrmViewList2.ButtonItemRuleChgClick(Sender: TObject);
var
  I: Integer;
  CheckBox: TCheckBox;
begin
  if SelItemRule = nil then
  begin
    Application.MessageBox('请选择一个需要修改的物品！', '错误信息', MB_OK + MB_ICONERROR);
    ListBoxItemRuleList.SetFocus;
    Exit;
  end;

  for I := 0 to Length(FAcutionPricesLime.Min) - 1 do
  begin
    if (FAcutionPricesLime.Min[I] <> 0)     //
      and (FAcutionPricesLime.Max[I] <> 0)       //
      and (FAcutionPricesLime.Min[I] > FAcutionPricesLime.Max[I]) then
    begin
      cbbAuctionPricesType.ItemIndex := I;
      Application.MessageBox('拍卖最低价不能大于最高价！', '错误信息', MB_OK + MB_ICONERROR);

      seAuctionPrices_Min.Value := FAcutionPricesLime.Min[I];
      seAuctionPrices_Max.Value := FAcutionPricesLime.Max[I];

      seAuctionPrices_Max.SetFocus;
      Exit;
    end;
  end;

  for I := 0 to GroupBoxItemRule.ControlCount - 1 do
  begin
    if GroupBoxItemRule.Controls[I] is TCheckBox then
    begin
      CheckBox := TCheckBox(GroupBoxItemRule.Controls[I]);
      if CheckBox.Tag >= 0 then
        SelItemRule.FlagArray[CheckBox.Tag] := CheckBox.Checked;

      SelItemRule.PricesLime := FAcutionPricesLime;
    end;
  end;
  // RefItemRuleList;
  ButtonItemRuleSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonItemRuleAddAllClick(Sender: TObject);
var
  I: Integer;
  FlagArray: TFlagArray;
  CheckBox: TCheckBox;
  sItemName: string;
begin
  if g_ItemRules.Find(sItemName) <> nil then
  begin
    Application.MessageBox('此物品已经存在！', '错误信息', MB_OK + MB_ICONERROR);
    EditRuleItemName.SetFocus;
    Exit;
  end;

  for I := 0 to Length(FAcutionPricesLime.Min) - 1 do
  begin
    if (FAcutionPricesLime.Min[I] <> 0) and (FAcutionPricesLime.Max[I] <> 0) and (FAcutionPricesLime.Min[I] > FAcutionPricesLime.Max
      [I]) then
    begin
      cbbAuctionPricesType.ItemIndex := I;
      Application.MessageBox('拍卖最低价不能大于最高价！', '错误信息', MB_OK + MB_ICONERROR);

      seAuctionPrices_Min.Value := FAcutionPricesLime.Min[I];
      seAuctionPrices_Max.Value := FAcutionPricesLime.Max[I];

      seAuctionPrices_Max.SetFocus;
      Exit;
    end;
  end;

  FillChar(FlagArray, SizeOf(TFlagArray), 0);
  for I := 0 to GroupBoxItemRule.ControlCount - 1 do
  begin
    if GroupBoxItemRule.Controls[I] is TCheckBox then
    begin
      CheckBox := TCheckBox(GroupBoxItemRule.Controls[I]);
      if CheckBox.Tag >= 0 then
        FlagArray[CheckBox.Tag] := CheckBox.Checked;
    end;
  end;

  for I := 0 to ListBoxItemList2.Items.Count - 1 do
  begin
    sItemName := ListBoxItemList2.Items.Strings[I];
    if g_ItemRules.Find(sItemName) = nil then
    begin
      g_ItemRules.Add(sItemName, FlagArray, @FAcutionPricesLime);
    end;
  end;
  RefItemRuleList;
  ButtonItemRuleSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonItemRuleDelAllClick(Sender: TObject);
begin
  ButtonItemRuleDelAll.Enabled := False;
  SelItemRule := nil;
  g_ItemRules.Clear;
  RefItemRuleList;
  ButtonItemRuleDelAll.Enabled := True;
  ButtonItemRuleSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonItemRuleDelClick(Sender: TObject);
begin
  if SelItemRule = nil then
  begin
    Application.MessageBox('请选择一个需要修改的物品！', '错误信息', MB_OK + MB_ICONERROR);
    ListBoxItemRuleList.SetFocus;
    Exit;
  end;

  if g_ItemRules.Delete(SelItemRule.ItemName) then
  begin
    SelItemRule := nil;
    RefItemRuleList;
    ButtonItemRuleSave.Enabled := True;
  end
  else
  begin
    Application.MessageBox('删除失败！', '错误信息', MB_OK + MB_ICONERROR);
  end;
end;

procedure TFrmViewList2.ListBoxUserCommandClick(Sender: TObject);
begin
  if ListBoxUserCommand.ItemIndex >= 0 then
  begin
    EditCommandName.Text := ListBoxUserCommand.Items.Strings[ListBoxUserCommand.ItemIndex];
    EditCommandIdx.Value := Integer(ListBoxUserCommand.Items.Objects[ListBoxUserCommand.ItemIndex]);
    ButtonUserCommandDel.Enabled := True;
    LabelMsg.Caption := Format('输入“@%s”触发 QFunction-0.txt 脚本中的 [@UserCmd%d] 字段', [ListBoxUserCommand.Items.Strings[ListBoxUserCommand.ItemIndex],
      Integer(ListBoxUserCommand.Items.Objects[ListBoxUserCommand.ItemIndex])]);
  end
  else
  begin
    ButtonUserCommandDel.Enabled := False;
    LabelMsg.Caption := '';
  end;
end;

procedure TFrmViewList2.ButtonUserCommandAddClick(Sender: TObject);
var
  sCmdName: string;
  nCmdIndex: Integer;
begin
  sCmdName := Trim(EditCommandName.Text);
  nCmdIndex := EditCommandIdx.Value;
  if g_UserCmds.Find(sCmdName) then
  begin
    Application.MessageBox('此命令名称已经存在！', '错误信息', MB_OK + MB_ICONERROR);
    EditCommandName.SetFocus;
    Exit;
  end;
  if g_UserCmds.Find(nCmdIndex) then
  begin
    Application.MessageBox('此命令编号已经存在！', '错误信息', MB_OK + MB_ICONERROR);
    EditCommandIdx.SetFocus;
    Exit;
  end;
  if g_UserCmds.Add(sCmdName, nCmdIndex) then
  begin
    RefUserCommandList;
    ButtonUserCommandSave.Enabled := True;
  end
  else
  begin
    Application.MessageBox('增加失败！', '错误信息', MB_OK + MB_ICONERROR);
  end;
end;

procedure TFrmViewList2.ButtonUserCommandDelClick(Sender: TObject);
var
  sCmdName: string;
  nCmdIndex: Integer;
begin
  sCmdName := Trim(EditCommandName.Text);
  nCmdIndex := EditCommandIdx.Value;
  if g_UserCmds.Find(sCmdName) then
  begin
    if g_UserCmds.Delete(sCmdName) then
    begin
      RefUserCommandList;
      ButtonUserCommandSave.Enabled := True;
      ButtonUserCommandDel.Enabled := False;
    end
    else
    begin
      if g_UserCmds.Delete(nCmdIndex) then
      begin
        RefUserCommandList;
        ButtonUserCommandSave.Enabled := True;
        ButtonUserCommandDel.Enabled := False;
      end
      else
      begin
        Application.MessageBox('删除失败！', '错误信息', MB_OK + MB_ICONERROR);
      end;
    end;
  end;
end;

procedure TFrmViewList2.ButtonGroupItemAddClick(Sender: TObject);
var
  I: Integer;
  SpinEdit: TSpinEditEx;
  CheckBox: TCheckBox;
  GroupItem: pTGroupItem;
  ListItem: TListItem;
  sItemName: string;
begin
  if g_GroupItems.FindIndex(EditGroupItemIndex.Value) then
  begin
    Application.MessageBox('套装编号已经存在，请重新输入！', '错误信息', MB_OK + MB_ICONERROR);
    EditGroupItemIndex.SetFocus;
    Exit;
  end;
  if EditGroupItemDesc.Text = '' then
  begin
    Application.MessageBox('请输入套装说明！', '错误信息', MB_OK + MB_ICONERROR);
    EditGroupItemDesc.SetFocus;
    Exit;
  end;
  if EditGroupItemName.Text = '' then
  begin
    Application.MessageBox('请输入套装物品！', '错误信息', MB_OK + MB_ICONERROR);
    EditGroupItemName.SetFocus;
    Exit;
  end;
  if EditGroupItemCount.Value <= 0 then
  begin
    Application.MessageBox('套装数量输入不正确！', '错误信息', MB_OK + MB_ICONERROR);
    EditGroupItemCount.SetFocus;
    Exit;
  end;
  New(GroupItem);
  FillChar(GroupItem^, SizeOf(TGroupItem), #0);
  GroupItem.FLD_ITEMNAMES := TStringList.Create;
  GroupItem.FLD_INDEX := EditGroupItemIndex.Value;
  GroupItem.FLD_COUNT := EditGroupItemCount.Value;
  GroupItem.FLD_DESC := EditGroupItemDesc.Text;
  GroupItem.FLD_HINTMSG := Trim(EditGroupItemHint.Text);
  ExtractStrings(['|'], [], PChar(Trim(EditGroupItemName.Text)), GroupItem.FLD_ITEMNAMES);
  TrimStringList(GroupItem.FLD_ITEMNAMES);

  for I := 0 to GroupBoxGroupItem.ControlCount - 1 do
  begin
    if GroupBoxGroupItem.Controls[I] is TCheckBox then
    begin
      CheckBox := TCheckBox(GroupBoxGroupItem.Controls[I]);
      GroupItem.FLD_FLAG[CheckBox.Tag] := CheckBox.Checked;
    end;
  end;

  for I := 0 to TabSheetRate.ControlCount - 1 do
  begin
    if TabSheetRate.Controls[I] is TSpinEditEx then
    begin
      SpinEdit := TSpinEditEx(TabSheetRate.Controls[I]);
      GroupItem.FLD_RATE[SpinEdit.Tag] := SpinEdit.Value;
    end;
  end;

  for I := 0 to TabSheetValue.ControlCount - 1 do
  begin
    if TabSheetValue.Controls[I] is TSpinEditEx then
    begin
      SpinEdit := TSpinEditEx(TabSheetValue.Controls[I]);
      GroupItem.FLD_VALUE[SpinEdit.Tag] := SpinEdit.Value;
    end;
  end;

  if g_GroupItems.Add(GroupItem) then
  begin
    // RefGroupItemList;

    ListItem := ListViewGroupItemList.Items.Add;
    ListItem.Caption := IntToStr(GroupItem.FLD_INDEX);
    ListItem.SubItems.AddObject(GroupItem.FLD_DESC, TObject(GroupItem));
    ListItem.SubItems.Add(IntToStr(GroupItem.FLD_COUNT));
    sItemName := '';
    for I := 0 to GroupItem.FLD_ITEMNAMES.Count - 1 do
    begin
      sItemName := sItemName + GroupItem.FLD_ITEMNAMES.Strings[I] + '|';
    end;
    ListItem.SubItems.Add(sItemName);

    SelGroupItem := GroupItem;
    ListItem.Selected := True;

    ButtonGroupItemSave.Enabled := True;
  end
  else
  begin
    GroupItem.FLD_ITEMNAMES.Free;
    Dispose(GroupItem);
    Application.MessageBox('增加失败！', '错误信息', MB_OK + MB_ICONERROR);
  end;
end;

procedure TFrmViewList2.ListViewGroupItemListClick(Sender: TObject);
var
  I: Integer;
  ListItem: TListItem;
  SpinEdit: TSpinEditEx;
  CheckBox: TCheckBox;
  sItemName: string;
begin
  ListItem := ListViewGroupItemList.Selected;
  if ListItem <> nil then
  begin
    SelGroupItem := pTGroupItem(ListItem.SubItems.Objects[0]);
    EditGroupItemIndex.Value := SelGroupItem.FLD_INDEX;
    EditGroupItemCount.Value := SelGroupItem.FLD_COUNT;
    EditGroupItemHint.Text := SelGroupItem.FLD_HINTMSG;
    EditGroupItemDesc.Text := SelGroupItem.FLD_DESC;
    Move(SelGroupItem.AttackSkillPercent, SelAttackSkillPercent, SizeOf(SelAttackSkillPercent));
    Move(SelGroupItem.DefenseSkillPercent, SelDefenseSkillPercent, SizeOf(SelDefenseSkillPercent));
    sItemName := '';
    for I := 0 to SelGroupItem.FLD_ITEMNAMES.Count - 1 do
      sItemName := sItemName + SelGroupItem.FLD_ITEMNAMES.Strings[I] + '|';
    EditGroupItemName.Text := sItemName;

    for I := 0 to GroupBoxGroupItem.ControlCount - 1 do
    begin
      if GroupBoxGroupItem.Controls[I] is TCheckBox then
      begin
        CheckBox := TCheckBox(GroupBoxGroupItem.Controls[I]);
        CheckBox.Checked := SelGroupItem.FLD_FLAG[CheckBox.Tag];
      end;
    end;

    for I := 0 to TabSheetRate.ControlCount - 1 do
    begin
      if TabSheetRate.Controls[I] is TSpinEditEx then
      begin
        SpinEdit := TSpinEditEx(TabSheetRate.Controls[I]);
        SpinEdit.Value := SelGroupItem.FLD_RATE[SpinEdit.Tag];
      end;
    end;

    for I := 0 to TabSheetValue.ControlCount - 1 do
    begin
      if TabSheetValue.Controls[I] is TSpinEditEx then
      begin
        SpinEdit := TSpinEditEx(TabSheetValue.Controls[I]);
        SpinEdit.Value := SelGroupItem.FLD_VALUE[SpinEdit.Tag];
      end;
    end;

    ButtonGroupItemDel.Enabled := True;
    ButtonGroupItemChg.Enabled := True;
    ButtonGroupItemSkillPower.Enabled := True;
  end
  else
  begin
    SelGroupItem := nil;
    ButtonGroupItemDel.Enabled := False;
    ButtonGroupItemChg.Enabled := False;
    ButtonGroupItemSkillPower.Enabled := False;
  end;
end;

procedure TFrmViewList2.ButtonGroupItemDelClick(Sender: TObject);
begin
  if SelGroupItem <> nil then
  begin
    ButtonGroupItemDel.Enabled := False;
    if g_GroupItems.Delete(SelGroupItem) then
    begin
      SelGroupItem := nil;
      RefGroupItemList;
      ButtonGroupItemChg.Enabled := False;
      ButtonGroupItemSave.Enabled := True;
    end
    else
    begin
      Application.MessageBox('删除失败！', '错误信息', MB_OK + MB_ICONERROR);
    end;
  end;
end;

procedure TFrmViewList2.ButtonGroupItemChgClick(Sender: TObject);
var
  I: Integer;
  SpinEdit: TSpinEditEx;
  CheckBox: TCheckBox;
  ListItem: TListItem;
  sItemName: string;
begin
  if SelGroupItem <> nil then
  begin
    Move(SelAttackSkillPercent, SelGroupItem.AttackSkillPercent, SizeOf(SelAttackSkillPercent));
    Move(SelDefenseSkillPercent, SelGroupItem.DefenseSkillPercent, SizeOf(SelDefenseSkillPercent));
    for I := 0 to GroupBoxGroupItem.ControlCount - 1 do
    begin
      if GroupBoxGroupItem.Controls[I] is TCheckBox then
      begin
        CheckBox := TCheckBox(GroupBoxGroupItem.Controls[I]);
        SelGroupItem.FLD_FLAG[CheckBox.Tag] := CheckBox.Checked;
      end;
    end;

    for I := 0 to TabSheetRate.ControlCount - 1 do
    begin
      if TabSheetRate.Controls[I] is TSpinEditEx then
      begin
        SpinEdit := TSpinEditEx(TabSheetRate.Controls[I]);
        SelGroupItem.FLD_RATE[SpinEdit.Tag] := SpinEdit.Value;
      end;
    end;

    for I := 0 to TabSheetValue.ControlCount - 1 do
    begin
      if TabSheetValue.Controls[I] is TSpinEditEx then
      begin
        SpinEdit := TSpinEditEx(TabSheetValue.Controls[I]);
        SelGroupItem.FLD_VALUE[SpinEdit.Tag] := SpinEdit.Value;
      end;
    end;

    if EditGroupItemDesc.Text = '' then
    begin
      Application.MessageBox('请输入套装说明！', '错误信息', MB_OK + MB_ICONERROR);
      EditGroupItemDesc.SetFocus;
      Exit;
    end;
    if EditGroupItemName.Text = '' then
    begin
      Application.MessageBox('请输入套装物品！', '错误信息', MB_OK + MB_ICONERROR);
      EditGroupItemName.SetFocus;
      Exit;
    end;
    if EditGroupItemCount.Value <= 0 then
    begin
      Application.MessageBox('套装数量输入不正确！', '错误信息', MB_OK + MB_ICONERROR);
      EditGroupItemCount.SetFocus;
      Exit;
    end;

    SelGroupItem.FLD_COUNT := EditGroupItemCount.Value;
    SelGroupItem.FLD_DESC := EditGroupItemDesc.Text;
    SelGroupItem.FLD_HINTMSG := Trim(EditGroupItemHint.Text);
    SelGroupItem.FLD_ITEMNAMES.Clear;
    ExtractStrings(['|'], [], PChar(Trim(EditGroupItemName.Text)), SelGroupItem.FLD_ITEMNAMES);
    TrimStringList(SelGroupItem.FLD_ITEMNAMES);

    // RefGroupItemList;

    ListItem := ListViewGroupItemList.Selected;
    if ListItem <> nil then
    begin
      ListItem.Caption := IntToStr(SelGroupItem.FLD_INDEX);
      ListItem.SubItems.Strings[0] := SelGroupItem.FLD_DESC;
      ListItem.SubItems.Strings[1] := IntToStr(SelGroupItem.FLD_COUNT);
      sItemName := '';
      for I := 0 to SelGroupItem.FLD_ITEMNAMES.Count - 1 do
      begin
        sItemName := sItemName + SelGroupItem.FLD_ITEMNAMES.Strings[I] + '|';
      end;
      ListItem.SubItems.Strings[2] := sItemName;
    end;

    ButtonGroupItemSave.Enabled := True;
  end
  else
    Application.MessageBox('请选择要修改的套装！', '提示信息', MB_OK + MB_ICONWARNING);
end;

procedure TFrmViewList2.ButtonShopChgItemClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
  sMemo: string;
  nPrice: Integer;
  StdItem: pTStdItem;
  nCount: Integer;
  nShopType: Integer;
begin
  if (SelShopItem <> nil) then
  begin
    sItemName := Trim(EditShopItemName.Text);
    nShopType := PageControlShop.ActivePageIndex;
    nPrice := EditShopItemPrice.Value;
    sMemo := Trim(EditItemMemo1.Text);
    if not (nShopType in [0..5]) then
    begin
      Application.MessageBox('请选择物品类别！', '错误信息', MB_OK + MB_ICONERROR);
      Exit;
    end;
    StdItem := UserEngine.GetStdItem(sItemName);
    if (sItemName = '') or (StdItem = nil) then
    begin
      Application.MessageBox('请选择一个正确的物品！', '错误信息', MB_OK + MB_ICONERROR);
      Exit;
    end;
    if ComboBoxGameMoney.ItemIndex < 0 then
    begin
      Application.MessageBox('请选择一个正确的交易货币！', '错误信息', MB_OK + MB_ICONERROR);
      Exit;
    end;
    if sMemo = '' then
    begin
      Application.MessageBox('请输入物品功能！', '错误信息', MB_OK + MB_ICONERROR);
      Exit;
    end;
    if nPrice <= 0 then
    begin
      Application.MessageBox('请输入正确的物品价格！', '错误信息', MB_OK + MB_ICONERROR);
      Exit;
    end;
    nCount := _MIN(MemoShop.Lines.Count, 6);
    SelShopItem^.StdItem.Price := nPrice;
    SelShopItem^.GameMoney := ComboBoxGameMoney.ItemIndex;
    SelShopItem^.ImageIndex := EditImageIndex.Value;
    SelShopItem^.ImageCount := EditImageCount.Value;
    SelShopItem^.Memo1 := sMemo;
    SelShopItem^.ItemCount := seItemCount.Value;
    SelShopItem^.boBulkBuy := chkBulkBuy.Checked;
    SelShopItem^.nBulkBuyCount := seBulkBuyCount.Value;

    seBulkBuyCount.Enabled := SelShopItem.boBulkBuy;

    // SelShopItem.ShopType := nShopType;
    // showmessage(inttostr(SelShopItem.StdItem.Price));
    sMemo := '';
    for I := 0 to nCount - 1 do
      sMemo := sMemo + MemoShop.Lines.Strings[I] + #13#10;

    SelShopItem^.Memo2 := sMemo;

    RefShopList;
    ButtonShopSaveItem.Enabled := True;
    // Application.MessageBox('修改成功！', '提示信息', MB_OK + MB_ICONWARNING);

  end
  else
    Application.MessageBox('请选择一个商品！', '错误信息', MB_OK + MB_ICONERROR);
end;

procedure TFrmViewList2.FilterItemListBox(MenuItem: TMenuItem; ListBox: TListBox);
var
  I, II: Integer;
  StdItem: pTStdItem;
  boFind: Boolean;
begin
  ListBox.Clear;
  if g_MultiThreadRun then
    UserEngine.StdItemList.LockR(16);
  try
    for I := 0 to UserEngine.StdItemList.Count - 1 do
    begin
      StdItem := UserEngine.StdItemList.Items[I];
      if MenuItem.Tag < 0 then
        ListBox.Items.AddObject(StdItem.Name, TObject(StdItem))
      else
      begin
        case MenuItem.Tag of
          U_DRESS:
            if StdItem.StdMode in [10, 11] then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_WEAPON:
            if (StdItem.StdMode = 5) or (StdItem.StdMode = 6) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_RIGHTHAND:
            if (StdItem.StdMode = 29) or (StdItem.StdMode = 30) or (StdItem.StdMode = 28) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_NECKLACE:
            if (StdItem.StdMode = 19) or (StdItem.StdMode = 20) or (StdItem.StdMode = 21) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_HELMET:
            if StdItem.StdMode = 15 then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_ARMRINGR, U_ARMRINGL:
            if (StdItem.StdMode = 24) or (StdItem.StdMode = 25) or (StdItem.StdMode = 26) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));

          U_RINGL, U_RINGR:
            if (StdItem.StdMode = 22) or (StdItem.StdMode = 23) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_BUJUK:
            if (StdItem.StdMode = 25) or (StdItem.StdMode = 51) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_BELT:
            if (StdItem.StdMode = 54) or (StdItem.StdMode = 64) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_BOOTS:
            if (StdItem.StdMode = 52) or (StdItem.StdMode = 62) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_CHARM:
            if (StdItem.StdMode = 53) or (StdItem.StdMode = 63) or (StdItem.StdMode = 7) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          U_HAT:
            if (StdItem.StdMode = 16) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));

          31:
            if (StdItem.StdMode = 31) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          100:
            if (StdItem.StdMode in [0..3]) or (StdItem.StdMode = 25) then
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
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
              ListBox.Items.AddObject(StdItem.Name, TObject(StdItem));
          end;
        end;
      end;
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.StdItemList.UnLockR;
  end;
end;

procedure TFrmViewList2.MenuItem_ShowAllClick(Sender: TObject);
var
  I: Integer;
  MenuItem: TMenuItem;
  PopupMenu: TPopupMenu;
begin
  PopupMenu := TPopupMenu(TMenuItem(Sender).GetParentMenu);
  for I := 0 to PopupMenu.Items.Count - 1 do
  begin
    MenuItem := PopupMenu.Items[I];
    if MenuItem <> Sender then
    begin
      MenuItem.Checked := False;
    end;
  end;

  MenuItem := TMenuItem(Sender);
  if not MenuItem.Checked then
  begin
    MenuItem.Checked := True;
    if PopupMenu = PopupMenu1 then
      FilterItemListBox(MenuItem, ListBoxitemList);
    if PopupMenu = PopupMenu2 then
      FilterItemListBox(MenuItem, ListBoxitemList1);
    if PopupMenu = PopupMenu3 then
      FilterItemListBox(MenuItem, ListBoxItemList2);
    if PopupMenu = PopupMenuItem then
      FilterItemListBox(MenuItem, ListBoxStdItemList1);

    if PopupMenu = PopupMenu5 then
      FilterItemListBox(MenuItem, ListBoxFoundryItemList);
    if PopupMenu = PopupMenu6 then
      FilterItemListBox(MenuItem, ListBoxitemList4);
    if PopupMenu = PopupMenu7 then
      FilterItemListBox(MenuItem, ListBoxitemList5);
  end;
end;

procedure TFrmViewList2.btnWilAddClick(Sender: TObject);
var
  I: Integer;
  sWilName: string;
begin
  sWilName := Trim(EditWilName.Text);
  if sWilName = '' then
  begin
    Application.MessageBox('请输入WIL文件名称！', '错误信息', MB_OK + MB_ICONERROR);
    EditWilName.SetFocus;
    Exit;
  end;
  for I := 0 to ListBoxWilNameList.Items.Count - 1 do
  begin
    if CompareText(ListBoxWilNameList.Items.Strings[I], sWilName) = 0 then
    begin
      Application.MessageBox('此WIL文件名称已经在列表中了！', '错误信息', MB_OK + MB_ICONERROR);
      EditWilName.SetFocus;
      Exit;
    end;
  end;
  { if GetWilName(sWilName) then begin
    Application.MessageBox('此WIL文件名称已经在列表中了！', '错误信息', MB_OK + MB_ICONERROR);
    EditWilName.SetFocus;
    Exit;
    end; }
  ListBoxWilNameList.Items.Add(sWilName);
  btnWilSave.Enabled := True;
end;

procedure TFrmViewList2.btnWilNameUPClick(Sender: TObject);
var
  sWilName: string;
  ItemIndex: Integer;
begin
  ItemIndex := ListBoxWilNameList.ItemIndex;
  if ItemIndex > 0 then
  begin
    sWilName := ListBoxWilNameList.Items[ItemIndex];
    ListBoxWilNameList.DeleteSelected;
    ListBoxWilNameList.Items.Insert(ItemIndex - 1, sWilName);
    ListBoxWilNameList.ItemIndex := ItemIndex - 1;
    btnWilNameDown.Enabled := (ListBoxWilNameList.ItemIndex >= 0) and (ListBoxWilNameList.ItemIndex < ListBoxWilNameList.Count - 1);
    btnWilNameUP.Enabled := (ListBoxWilNameList.ItemIndex > 0);
    btnWilSave.Enabled := True;
  end;
end;

procedure TFrmViewList2.btnWilNameDownClick(Sender: TObject);
var
  sWilName: string;
  ItemIndex: Integer;
begin
  ItemIndex := ListBoxWilNameList.ItemIndex;
  if (ItemIndex >= 0) and (ItemIndex < ListBoxWilNameList.Count - 1) then
  begin
    sWilName := ListBoxWilNameList.Items[ItemIndex];
    ListBoxWilNameList.DeleteSelected;
    ListBoxWilNameList.Items.Insert(ItemIndex + 1, sWilName);
    ListBoxWilNameList.ItemIndex := ItemIndex + 1;
    btnWilNameDown.Enabled := (ListBoxWilNameList.ItemIndex >= 0) and (ListBoxWilNameList.ItemIndex < ListBoxWilNameList.Count - 1);
    btnWilNameUP.Enabled := (ListBoxWilNameList.ItemIndex > 0);
    btnWilSave.Enabled := True;
  end;
end;

procedure TFrmViewList2.ListBoxWilNameListClick(Sender: TObject);
begin
  if ListBoxWilNameList.ItemIndex >= 0 then
  begin
    btnWilNameDown.Enabled := (ListBoxWilNameList.ItemIndex >= 0) and (ListBoxWilNameList.ItemIndex < ListBoxWilNameList.Count - 1);
    btnWilNameUP.Enabled := (ListBoxWilNameList.ItemIndex > 0);
    LabelFileIndex.Caption := ListBoxWilNameList.Items[ListBoxWilNameList.ItemIndex] + ' 编号:' + IntToStr(ListBoxWilNameList.ItemIndex);
    EditWilName.Text := ListBoxWilNameList.Items[ListBoxWilNameList.ItemIndex];
    btnWilDel.Enabled := True;
    btnWilEdit.Enabled := True;
  end
  else
  begin
    btnWilDel.Enabled := False;
    btnWilEdit.Enabled := False;
  end;
end;

procedure TFrmViewList2.btnWilSaveClick(Sender: TObject);
begin
  g_EffectImageList.Clear;
  g_EffectImageList.AddStrings(ListBoxWilNameList.Items);

  cbbEffectFileIndex1.Clear;
  cbbEffectFileIndex2.Clear;
  cbbEffectFileIndex3.Clear;
  cbbAddEffectFileIndex.Clear;
  cbbEffectFileIndex5.Clear;

  cbbEffectFileIndex1.Items.Add('关闭特效');
  cbbEffectFileIndex2.Items.Add('关闭特效');
  cbbEffectFileIndex3.Items.Add('关闭特效');
  cbbAddEffectFileIndex.Items.Add('关闭特效');
  cbbEffectFileIndex5.Items.Add('关闭特效');

  cbbEffectFileIndex1.Items.AddStrings(g_EffectImageList);
  cbbEffectFileIndex2.Items.AddStrings(g_EffectImageList);
  cbbEffectFileIndex3.Items.AddStrings(g_EffectImageList);
  cbbAddEffectFileIndex.Items.AddStrings(g_EffectImageList);
  cbbEffectFileIndex5.Items.AddStrings(g_EffectImageList);

  SaveEffectImageList();
  btnWilSave.Enabled := False;
end;

procedure TFrmViewList2.ButtonSendEffectImageListClick(Sender: TObject);
begin
  UserEngine.SendEffectImageList();
end;

procedure TFrmViewList2.btnWilDelClick(Sender: TObject);
begin
  if ListBoxWilNameList.ItemIndex >= 0 then
  begin
    ListBoxWilNameList.DeleteSelected;
    btnWilDel.Enabled := False;
    btnWilSave.Enabled := True;
    btnWilNameDown.Enabled := (ListBoxWilNameList.ItemIndex >= 0) and (ListBoxWilNameList.ItemIndex < ListBoxWilNameList.Count - 1);
    btnWilNameUP.Enabled := (ListBoxWilNameList.ItemIndex > 0);
  end;
end;

procedure TFrmViewList2.ButtonEffectAddClick(Sender: TObject);
var
  ItemEffect: pTItemEffect;
begin
  if EditEffectIndex.Value <= 0 then
  begin
    Application.MessageBox('特效编号必须大于0！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;
  New(ItemEffect);
  FillChar(ItemEffect^, SizeOf(TItemEffect), 0);
  ItemEffect.Index := EditEffectIndex.Value;
  ItemEffect.EffectDesc := EditEffectDesc.Text;

  ItemEffect.StartIndex1 := seItemEffectOffset1.Value;
  ItemEffect.StartIndex2 := seItemEffectOffset2.Value;
  ItemEffect.StartIndex3 := seItemEffectOffset3.Value;
  ItemEffect.StartIndex4 := seAddEffectOffset.Value;
  ItemEffect.StartIndex5 := seItemEffectOffset5.Value;

  ItemEffect.FileIndex1 := _MAX(cbbEffectFileIndex1.ItemIndex - 1, -1);
  ItemEffect.FileIndex2 := _MAX(cbbEffectFileIndex2.ItemIndex - 1, -1);
  ItemEffect.FileIndex3 := _MAX(cbbEffectFileIndex3.ItemIndex - 1, -1);
  ItemEffect.FileIndex4 := _MAX(cbbAddEffectFileIndex.ItemIndex - 1, -1);
  ItemEffect.FileIndex5 := _MAX(cbbEffectFileIndex5.ItemIndex - 1, -1);

  ItemEffect.ImageCount1 := seItemEffectImageCount1.Value;
  ItemEffect.ImageCount3 := seItemEffectImageCount3.Value;
  ItemEffect.ImageCount4 := seAddEffectPlayCount.Value;
  ItemEffect.ImageCount5 := seItemEffectImageCount5.Value;

  ItemEffect.OffSetX1 := seItemEffectOffsetX1.Value; // 内观
  ItemEffect.OffSetY1 := seItemEffectOffsetY1.Value; // 内观

  ItemEffect.OffSetX3 := seItemEffectOffsetX3.Value; // 包裹
  ItemEffect.OffSetY3 := seItemEffectOffsetY3.Value; // 包裹

  ItemEffect.OffSetX5 := seItemEffectOffsetX5.Value;
  ItemEffect.OffSetY5 := seItemEffectOffsetY5.Value;

  ItemEffect.Time1 := seItemEffectTime1.Value;
  ItemEffect.Time3 := seItemEffectTime3.Value;
  ItemEffect.Time4 := seAddEffectPlayTime.Value;
  ItemEffect.Time5 := seItemEffectTime5.Value;

  ItemEffect.NoBlendMode2 := chkNoBlendMode2.Checked;
  ItemEffect.NoSex2 := chkNoSex2.Checked;
  // ItemEffect.DressEffect := chkDressEffect.Checked;

  ItemEffect.AddEffectDrawOrder := cbbAddEffectDrawOrder.ItemIndex;
  ItemEffect.AddEffectNoBlendMode := chkAddEffectNoBlend.Checked;
  ItemEffect.AddEffectDrawCenter := chkAddEffectDrawCenter.Checked;

  ItemEffect.DrawCenter1 := chkDrawCenter1.Checked;
  ItemEffect.DrawCenter3 := chkDrawCenter3.Checked;
  ItemEffect.DrawCenter5 := chkDrawCenter5.Checked;

  ItemEffect.NoBlendMode1 := chkNoBlendMode1.Checked;
  ItemEffect.NoBlendMode3 := chkNoBlendMode3.Checked;
  ItemEffect.NoBlendMode5 := chkNoBlendMode5.Checked;

  ItemEffect.boEfectBelowItem1 := chkEfectBelowItem1.Checked;
  ItemEffect.boEfectBelowItem5 := chkEfectBelowItem5.Checked;

  if g_ItemEffects.Add(ItemEffect) then
  begin
    // SelItemEffect := pTItemEffect(lstEffectList.Items.Objects[nIndex]);

    lstEffectList.Items.AddObject('特效编号:' + IntToStr(ItemEffect.Index), TObject(ItemEffect));
    ComboBoxEffectIndex.Items.AddObject('特效编号:' + IntToStr(ItemEffect.Index), TObject(ItemEffect));
    ButtonEffectSave.Enabled := True;
  end
  else
  begin
    Dispose(ItemEffect);
    Application.MessageBox('增加失败,此特效编号已经存在！', '错误信息', MB_OK + MB_ICONERROR);
  end;
end;

procedure TFrmViewList2.ButtonEffectDelClick(Sender: TObject);
var
  I: Integer;
  StdItem: pTStdItem;
  ItemEffect: pTItemEffect;
begin
  if SelItemEffect <> nil then
  begin
    for I := 0 to g_EffectItemList.Count - 1 do
    begin
      if Integer(g_EffectItemList.Objects[I]) = SelItemEffect.Index then
      begin
        StdItem := UserEngine.GetStdItem(g_EffectItemList.Strings[I]);
        if (StdItem <> nil) and (StdItem.Effect <> Int64(nil)) and (pTItemEffect(StdItem.Effect).Index = SelItemEffect.Index) then
        begin
          StdItem.BagEffect.FileIndex := -1; // 包裹中的物品发光效果 文件编号 0
          StdItem.BagEffect.ImageStart := 0; // 包裹中的物品发光效果 读取位置
          StdItem.BagEffect.ImageCount := 0; // 包裹中的物品发光效果 读取张数
          StdItem.BagEffect.OffsetX := 0; // 包裹中的物品发光效果 微调X
          StdItem.BagEffect.OffsetY := 0; // 包裹中的物品发光效果 微调Y

          StdItem.BodyEffect.FileIndex := -1; // 内观物品发光效果 文件编号 0
          StdItem.BodyEffect.ImageStart := 0; // 内观物品发光效果 读取位置
          StdItem.BodyEffect.ImageCount := 0; // 内观物品发光效果 读取张数
          StdItem.BodyEffect.OffsetX := 0; // 内观物品发光效果 微调X
          StdItem.BodyEffect.OffsetY := 0; // 内观物品发光效果 微调Y

          StdItem.Effect := Int64(nil);

        end;
        Break;
      end;
    end;

    for I := ListBoxEffectItemList.Count - 1 downto 0 do
    begin
      if ListBoxEffectItemList.Items.Objects[I] = TObject(SelItemEffect) then
      begin
        ListBoxEffectItemList.Items.Delete(I);
      end;
    end;

    if g_ItemEffects.Delete(SelItemEffect) then
    begin
      SelItemEffect := nil;
      RefItemEffectList;
      ButtonEffectDel.Enabled := False;
      ButtonEffectChg.Enabled := False;
      ButtonEffectSave.Enabled := True;
    end
    else
    begin
      Application.MessageBox('删除失败！', '错误信息', MB_OK + MB_ICONERROR);
    end;

    for I := g_EffectItemList.Count - 1 downto 0 do
    begin
      if (Integer(g_EffectItemList.Objects[I]) > 0) then
      begin
        ItemEffect := g_ItemEffects.Get(Integer(g_EffectItemList.Objects[I]));
        if ItemEffect = nil then
        begin
          g_EffectItemList.Delete(I);
          g_ItemEffects.DeleteIndex(I);
        end;
      end;
    end;
  end;
end;

procedure TFrmViewList2.ButtonEffectChgClick(Sender: TObject);
var
  I: Integer;
  StdItem: pTStdItem;
begin
  if SelItemEffect = nil then
  begin
    Application.MessageBox('请在特效物品列表中选择一个特效！', '错误信息', MB_OK + MB_ICONERROR);
    ListBoxEffectItemList.SetFocus;
    Exit;
  end;

  SelItemEffect.StartIndex1 := seItemEffectOffset1.Value;
  SelItemEffect.StartIndex2 := seItemEffectOffset2.Value;
  SelItemEffect.StartIndex3 := seItemEffectOffset3.Value;
  SelItemEffect.StartIndex4 := seAddEffectOffset.Value;
  SelItemEffect.StartIndex5 := seItemEffectOffset5.Value;

  SelItemEffect.FileIndex1 := _MAX(cbbEffectFileIndex1.ItemIndex - 1, -1);
  SelItemEffect.FileIndex2 := _MAX(cbbEffectFileIndex2.ItemIndex - 1, -1);
  SelItemEffect.FileIndex3 := _MAX(cbbEffectFileIndex3.ItemIndex - 1, -1);
  SelItemEffect.FileIndex4 := _MAX(cbbAddEffectFileIndex.ItemIndex - 1, -1);
  SelItemEffect.FileIndex5 := _MAX(cbbEffectFileIndex5.ItemIndex - 1, -1);

  SelItemEffect.ImageCount1 := seItemEffectImageCount1.Value;
  SelItemEffect.ImageCount3 := seItemEffectImageCount3.Value;
  SelItemEffect.ImageCount4 := seAddEffectPlayCount.Value;
  SelItemEffect.ImageCount5 := seItemEffectImageCount5.Value;

  SelItemEffect.OffSetX1 := seItemEffectOffsetX1.Value; // 内观
  SelItemEffect.OffSetY1 := seItemEffectOffsetY1.Value; // 内观

  SelItemEffect.OffSetX3 := seItemEffectOffsetX3.Value; // 包裹
  SelItemEffect.OffSetY3 := seItemEffectOffsetY3.Value; // 包裹

  SelItemEffect.OffSetX5 := seItemEffectOffsetX5.Value;
  SelItemEffect.OffSetY5 := seItemEffectOffsetY5.Value;

  SelItemEffect.Time1 := seItemEffectTime1.Value;
  SelItemEffect.Time3 := seItemEffectTime3.Value;
  SelItemEffect.Time4 := seAddEffectPlayTime.Value;
  SelItemEffect.Time5 := seItemEffectTime5.Value;

  SelItemEffect.EffectDesc := EditEffectDesc.Text;

  SelItemEffect.NoBlendMode2 := chkNoBlendMode2.Checked;
  SelItemEffect.NoSex2 := chkNoSex2.Checked;
  // SelItemEffect.DressEffect := chkDressEffect.Checked;

  SelItemEffect.DrawCenter1 := chkDrawCenter1.Checked;
  SelItemEffect.DrawCenter3 := chkDrawCenter3.Checked;
  SelItemEffect.DrawCenter5 := chkDrawCenter5.Checked;

  SelItemEffect.NoBlendMode1 := chkNoBlendMode1.Checked;
  SelItemEffect.NoBlendMode3 := chkNoBlendMode3.Checked;
  SelItemEffect.NoBlendMode5 := chkNoBlendMode5.Checked;

  SelItemEffect.boEfectBelowItem1 := chkEfectBelowItem1.Checked;
  SelItemEffect.boEfectBelowItem5 := chkEfectBelowItem5.Checked;

  SelItemEffect.AddEffectDrawOrder := cbbAddEffectDrawOrder.ItemIndex;
  SelItemEffect.AddEffectNoBlendMode := chkAddEffectNoBlend.Checked;
  SelItemEffect.AddEffectDrawCenter := chkAddEffectDrawCenter.Checked;

  for I := 0 to g_EffectItemList.Count - 1 do
  begin
    if Integer(g_EffectItemList.Objects[I]) = SelItemEffect.Index then
    begin
      StdItem := UserEngine.GetStdItem(g_EffectItemList.Strings[I]);
      if (StdItem <> nil) and (StdItem.Effect <> Int64(nil)) and (pTItemEffect(StdItem.Effect).Index = SelItemEffect.Index) then
      begin
        StdItem.BodyEffect.FileIndex := SelItemEffect.FileIndex1; // 内观物品发光效果 文件编号 0
        StdItem.BodyEffect.ImageStart := SelItemEffect.StartIndex1; // 内观物品发光效果 读取位置
        StdItem.BodyEffect.ImageCount := SelItemEffect.ImageCount1; // 内观物品发光效果 读取张数
        StdItem.BodyEffect.OffsetX := SelItemEffect.OffSetX1; // 内观物品发光效果 微调X
        StdItem.BodyEffect.OffsetY := SelItemEffect.OffSetY1; // 内观物品发光效果 微调Y
        StdItem.BodyEffect.Time := SelItemEffect.Time1;
        StdItem.BodyEffect.IsDrawCenter := SelItemEffect.DrawCenter1;
        StdItem.BodyEffect.IsDrawNoBlend := SelItemEffect.NoBlendMode1;
        StdItem.BodyEffect.IsDrawBelow := SelItemEffect.boEfectBelowItem1;

        StdItem.BagEffect.FileIndex := SelItemEffect.FileIndex3; // 包裹中的物品发光效果 文件编号 0
        StdItem.BagEffect.ImageStart := SelItemEffect.StartIndex3; // 包裹中的物品发光效果 读取位置
        StdItem.BagEffect.ImageCount := SelItemEffect.ImageCount3; // 包裹中的物品发光效果 读取张数
        StdItem.BagEffect.OffsetX := SelItemEffect.OffSetX3; // 包裹中的物品发光效果 微调X
        StdItem.BagEffect.OffsetY := SelItemEffect.OffSetY3; // 包裹中的物品发光效果 微调Y
        StdItem.BagEffect.Time := SelItemEffect.Time3;
        StdItem.BagEffect.IsDrawCenter := SelItemEffect.DrawCenter3;
        StdItem.BagEffect.IsDrawNoBlend := SelItemEffect.NoBlendMode3;
      end;
      // break;
    end;
  end;
  RefItemEffectList;
  ButtonEffectSave.Enabled := True;
end;

procedure TFrmViewList2.PageControlShopChange(Sender: TObject);
begin
  ComboBoxShopType.ItemIndex := PageControlShop.ActivePageIndex;
end;

procedure TFrmViewList2.lstEffectListClick(Sender: TObject);
var
  nIndex: Integer;
begin
  nIndex := lstEffectList.ItemIndex;
  if nIndex >= 0 then
  begin
    SelItemEffect := pTItemEffect(lstEffectList.Items.Objects[nIndex]);
    EditEffectIndex.Value := SelItemEffect.Index;
    // GroupBoxEffectItem.Caption := '特效编号:' + IntToStr(SelItemEffect.Index);

    seItemEffectOffset1.Value := SelItemEffect.StartIndex1;
    seItemEffectOffset2.Value := SelItemEffect.StartIndex2;
    seItemEffectOffset3.Value := SelItemEffect.StartIndex3;
    seAddEffectOffset.Value := SelItemEffect.StartIndex4;
    seItemEffectOffset5.Value := SelItemEffect.StartIndex5;

    cbbEffectFileIndex1.ItemIndex := SelItemEffect.FileIndex1 + 1;
    cbbEffectFileIndex2.ItemIndex := SelItemEffect.FileIndex2 + 1;
    cbbEffectFileIndex3.ItemIndex := SelItemEffect.FileIndex3 + 1;
    cbbAddEffectFileIndex.ItemIndex := SelItemEffect.FileIndex4 + 1;
    cbbEffectFileIndex5.ItemIndex := SelItemEffect.FileIndex5 + 1;

    seItemEffectImageCount1.Value := SelItemEffect.ImageCount1;
    seItemEffectImageCount3.Value := SelItemEffect.ImageCount3;
    seAddEffectPlayCount.Value := SelItemEffect.ImageCount4;
    seItemEffectImageCount5.Value := SelItemEffect.ImageCount5;

    seItemEffectOffsetX1.Value := SelItemEffect.OffSetX1;
    seItemEffectOffsetY1.Value := SelItemEffect.OffSetY1;

    seItemEffectOffsetX3.Value := SelItemEffect.OffSetX3;
    seItemEffectOffsetY3.Value := SelItemEffect.OffSetY3;

    seItemEffectOffsetX5.Value := SelItemEffect.OffSetX5;
    seItemEffectOffsetY5.Value := SelItemEffect.OffSetY5;

    seItemEffectTime1.Value := SelItemEffect.Time1;
    seItemEffectTime3.Value := SelItemEffect.Time3;
    seAddEffectPlayTime.Value := SelItemEffect.Time4;
    seItemEffectTime5.Value := SelItemEffect.Time5;

    EditEffectDesc.Text := SelItemEffect.EffectDesc;

    chkNoBlendMode2.Checked := SelItemEffect.NoBlendMode2;
    chkNoSex2.Checked := SelItemEffect.NoSex2;
    // chkDressEffect.Checked := SelItemEffect.DressEffect;

    chkDrawCenter1.Checked := SelItemEffect.DrawCenter1;
    chkDrawCenter3.Checked := SelItemEffect.DrawCenter3;
    chkDrawCenter5.Checked := SelItemEffect.DrawCenter5;

    chkNoBlendMode1.Checked := SelItemEffect.NoBlendMode1;
    chkNoBlendMode3.Checked := SelItemEffect.NoBlendMode3;
    chkNoBlendMode5.Checked := SelItemEffect.NoBlendMode5;

    chkEfectBelowItem1.Checked := SelItemEffect.boEfectBelowItem1;
    chkEfectBelowItem5.Checked := SelItemEffect.boEfectBelowItem5;

    cbbAddEffectDrawOrder.ItemIndex := SelItemEffect.AddEffectDrawOrder;
    chkAddEffectNoBlend.Checked := SelItemEffect.AddEffectNoBlendMode;
    chkAddEffectDrawCenter.Checked := SelItemEffect.AddEffectDrawCenter;

    ButtonEffectChg.Enabled := True;
    ButtonEffectDel.Enabled := True;
  end
  else
  begin
    SelItemEffect := nil;
    ButtonEffectChg.Enabled := False;
    ButtonEffectDel.Enabled := False;
  end;
end;

procedure TFrmViewList2.ListBoxEffectItemListClick(Sender: TObject);
var
  I, nIndex: Integer;
  ItemEffect: pTItemEffect;
begin
  nIndex := ListBoxEffectItemList.ItemIndex;
  if nIndex >= 0 then
  begin
    EditEffectItemName.Text := ListBoxEffectItemList.Items.Strings[nIndex];
    ItemEffect := pTItemEffect(ListBoxEffectItemList.Items.Objects[nIndex]);
    for I := 0 to ComboBoxEffectIndex.Items.Count - 1 do
    begin
      if pTItemEffect(ComboBoxEffectIndex.Items.Objects[I]) = ItemEffect then
      begin
        ButtonEffectItemChg.Enabled := True;
        ButtonEffectItemDel.Enabled := True;
        ComboBoxEffectIndex.ItemIndex := I;
        ComboBoxEffectIndexChange(ComboBoxEffectIndex);
        Exit;
      end;
    end;
  end;
  ButtonEffectItemChg.Enabled := False;
  ButtonEffectItemDel.Enabled := False;
end;

procedure TFrmViewList2.ComboBoxEffectIndexChange(Sender: TObject);

  function GetImageName(Index: Integer): string;
  begin
    if (Index >= 0) and (Index < g_EffectImageList.Count) then
      Result := g_EffectImageList.Strings[Index]
    else
      Result := '无'
  end;

var
  nIndex: Integer;
  S: string;
  ItemEffect: pTItemEffect;
begin
  nIndex := ComboBoxEffectIndex.ItemIndex;
  if nIndex >= 0 then
  begin
    ItemEffect := pTItemEffect(ComboBoxEffectIndex.Items.Objects[nIndex]);
    S := ComboBoxEffectIndex.Items.Strings[nIndex] + ':' + #13#10;
    if ItemEffect.FileIndex1 < 0 then
    begin
      S := S + '内观特效: 无' + #13#10;
    end
    else
    begin
      S := S + Format('内观特效: 读取%s 开始图片:%d 播放图片张数:%d', [GetImageName(ItemEffect.FileIndex1), ItemEffect.StartIndex1, ItemEffect.ImageCount1])
        + #13#10;
    end;

    if ItemEffect.FileIndex2 < 0 then
    begin
      S := S + '外观特效: 无' + #13#10;
    end
    else
    begin
      S := S + Format('外观特效: 读取%s 开始图片:%d', [GetImageName(ItemEffect.FileIndex2), ItemEffect.StartIndex2]) + #13#10;
    end;

    if ItemEffect.FileIndex3 < 0 then
    begin
      S := S + '包裹中的特效: 无';
    end
    else
    begin
      S := S + Format('包裹中的特效: 读取%s 开始图片:%d 播放图片张数:%d', [GetImageName(ItemEffect.FileIndex3), ItemEffect.StartIndex3, ItemEffect.ImageCount3]);
    end;
    lblEffectMemo.Caption := Trim(S);
    lblEffectDesc.Caption := Trim(ItemEffect.EffectDesc);
    if ListBoxEffectItemList.ItemIndex >= 0 then
    begin
      ButtonEffectItemChg.Enabled := True;
      ButtonEffectItemDel.Enabled := True;
    end;
  end;
end;

procedure TFrmViewList2.ListBoxStdItemList1Click(Sender: TObject);
begin
  if ListBoxStdItemList1.ItemIndex >= 0 then
    EditEffectItemName.Text := ListBoxStdItemList1.Items.Strings[ListBoxStdItemList1.ItemIndex];
end;

procedure TFrmViewList2.ButtonEffectItemAddClick(Sender: TObject);
var
  nIndex: Integer;
  sItemName: string;
  ItemEffect: pTItemEffect;
  StdItem: pTStdItem;
begin
  sItemName := Trim(EditEffectItemName.Text);
  StdItem := UserEngine.GetStdItem(sItemName);
  if StdItem = nil then
  begin
    Application.MessageBox('请输入正确的物品名称！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  if GetEffectItemList(sItemName) >= 0 then
  begin
    Application.MessageBox('此物品已经添加过了！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  nIndex := ComboBoxEffectIndex.ItemIndex;
  if nIndex < 0 then
  begin
    Application.MessageBox('请选择正确的特效编号！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  ItemEffect := pTItemEffect(ComboBoxEffectIndex.Items.Objects[nIndex]);

  g_EffectItemList.AddObject(sItemName, TObject(ItemEffect.Index));
  StdItem.BodyEffect.FileIndex := ItemEffect.FileIndex1; // 内观物品发光效果 文件编号 0
  StdItem.BodyEffect.ImageStart := ItemEffect.StartIndex1; // 内观物品发光效果 读取位置
  StdItem.BodyEffect.ImageCount := ItemEffect.ImageCount1; // 内观物品发光效果 读取张数
  StdItem.BodyEffect.OffsetX := ItemEffect.OffSetX1; // 内观物品发光效果 微调X
  StdItem.BodyEffect.OffsetY := ItemEffect.OffSetY1; // 内观物品发光效果 微调Y
  StdItem.BodyEffect.Time := ItemEffect.Time1;

  StdItem.BagEffect.FileIndex := ItemEffect.FileIndex3; // 包裹中的物品发光效果 文件编号 0
  StdItem.BagEffect.ImageStart := ItemEffect.StartIndex3; // 包裹中的物品发光效果 读取位置
  StdItem.BagEffect.ImageCount := ItemEffect.ImageCount3; // 包裹中的物品发光效果 读取张数
  StdItem.BagEffect.OffsetX := ItemEffect.OffSetX3; // 包裹中的物品发光效果 微调X
  StdItem.BagEffect.OffsetY := ItemEffect.OffSetY3; // 包裹中的物品发光效果 微调Y
  StdItem.BagEffect.Time := ItemEffect.Time3;
  StdItem.BagEffect.IsDrawCenter := ItemEffect.DrawCenter3;
  StdItem.BagEffect.IsDrawNoBlend := ItemEffect.NoBlendMode3;

    // StdItem.DropEffectFileIndex := ItemEffect.FileIndex5;                                                 // 地面中的物品发光效果 文件编号 0

    // StdItem.DropEffectImageStart := ItemEffect.OffSet5;                                                  // 地面中的物品发光效果 读取位置

    // StdItem.DropEffectImageCount := ItemEffect.ImageCount5;                                               // 地面中的物品发光效果 读取张数

    // StdItem.DropEffectOffsetX := ItemEffect.OffSetX5;                                                     // 地面中的物品发光效果 微调X

    // StdItem.DropEffectOffsetY := ItemEffect.OffSetY5;                                                     // 地面中的物品发光效果 微调Y
  // StdItem.DropEffectTime := ItemEffect.Time5;
  // StdItem.DropEffectDrawCenter := ItemEffect.DrawCenter5;
  // StdItem.DropEffectNoBlend := ItemEffect.NoBlendMode5;

  StdItem.Effect := Int64(ItemEffect);
  ListBoxEffectItemList.Items.AddObject(sItemName, TObject(ItemEffect));
  ButtonEffectItemSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonEffectItemChgClick(Sender: TObject);
var
  I, nIndex: Integer;
  sItemName: string;
  ItemEffect: pTItemEffect;
  StdItem: pTStdItem;
begin
  sItemName := Trim(EditEffectItemName.Text);
  StdItem := UserEngine.GetStdItem(sItemName);
  if StdItem = nil then
  begin
    Application.MessageBox('请输入正确的物品名称！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  if GetEffectItemList(sItemName) < 0 then
  begin
    Application.MessageBox('此物品没有在列表中！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  nIndex := ComboBoxEffectIndex.ItemIndex;
  if nIndex < 0 then
  begin
    Application.MessageBox('请选择正确的特效编号！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  ItemEffect := pTItemEffect(ComboBoxEffectIndex.Items.Objects[nIndex]);
  nIndex := GetEffectItemList(sItemName);
  if nIndex >= 0 then
  begin
    g_EffectItemList.Objects[nIndex] := TObject(ItemEffect.Index);

    StdItem.BodyEffect.FileIndex := ItemEffect.FileIndex1; // 内观物品发光效果 文件编号 0
    StdItem.BodyEffect.ImageStart := ItemEffect.StartIndex1; // 内观物品发光效果 读取位置
    StdItem.BodyEffect.ImageCount := ItemEffect.ImageCount1; // 内观物品发光效果 读取张数
    StdItem.BodyEffect.OffsetX := ItemEffect.OffSetX1; // 内观物品发光效果 微调X
    StdItem.BodyEffect.OffsetY := ItemEffect.OffSetY1; // 内观物品发光效果 微调Y
    StdItem.BodyEffect.Time := ItemEffect.Time1;

    StdItem.BagEffect.FileIndex := ItemEffect.FileIndex3; // 包裹中的物品发光效果 文件编号 0
    StdItem.BagEffect.ImageStart := ItemEffect.StartIndex3; // 包裹中的物品发光效果 读取位置
    StdItem.BagEffect.ImageCount := ItemEffect.ImageCount3; // 包裹中的物品发光效果 读取张数
    StdItem.BagEffect.OffsetX := ItemEffect.OffSetX3; // 包裹中的物品发光效果 微调X
    StdItem.BagEffect.OffsetY := ItemEffect.OffSetY3; // 包裹中的物品发光效果 微调Y
    StdItem.BagEffect.Time := ItemEffect.Time3;
    StdItem.BagEffect.IsDrawCenter := ItemEffect.DrawCenter3;
    StdItem.BagEffect.IsDrawNoBlend := ItemEffect.NoBlendMode3;

    StdItem.Effect := Int64(ItemEffect);
    for I := 0 to ListBoxEffectItemList.Items.Count - 1 do
    begin
      if CompareText(ListBoxEffectItemList.Items.Strings[I], sItemName) = 0 then
      begin
        ListBoxEffectItemList.Items.Objects[I] := TObject(ItemEffect);
        Break;
      end;
    end;
    // ListBoxEffectItemList.Items.AddObject(sItemName, TObject(ItemEffect));
    ButtonEffectItemSave.Enabled := True;
  end
  else
    Application.MessageBox('修改失败！', '提示信息', MB_OK + MB_ICONWARNING);
end;

procedure TFrmViewList2.ButtonEffectItemDelClick(Sender: TObject);
var
  nIndex: Integer;
  sItemName: string;
  StdItem: pTStdItem;
begin
  if ListBoxEffectItemList.ItemIndex < 0 then
  begin
    Application.MessageBox('请先选择要删除特效的物品', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  sItemName := Trim(ListBoxEffectItemList.Items.Strings[ListBoxEffectItemList.ItemIndex]);
  StdItem := UserEngine.GetStdItem(sItemName);
  { if StdItem = nil then begin
    Application.MessageBox('请输入正确的物品名称！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
    end; }
  nIndex := GetEffectItemList(sItemName);
  if nIndex < 0 then
  begin
    Application.MessageBox('此物品没有在列表中！', '提示信息', MB_OK + MB_ICONWARNING);
    Exit;
  end;

  if StdItem <> nil then
  begin
    if pTItemEffect(ListBoxEffectItemList.Items.Objects[ListBoxEffectItemList.ItemIndex]).Index = Integer(g_EffectItemList.Objects
      [nIndex]) then
    begin
      StdItem.BagEffect.FileIndex := -1; // 包裹中的物品发光效果 文件编号 0
      StdItem.BagEffect.ImageStart := 0; // 包裹中的物品发光效果 读取位置
      StdItem.BagEffect.ImageCount := 0; // 包裹中的物品发光效果 读取张数
      StdItem.BagEffect.OffsetX := 0; // 包裹中的物品发光效果 微调X
      StdItem.BagEffect.OffsetY := 0; // 包裹中的物品发光效果 微调Y

      StdItem.BodyEffect.FileIndex := -1; // 内观物品发光效果 文件编号 0
      StdItem.BodyEffect.ImageStart := 0; // 内观物品发光效果 读取位置
      StdItem.BodyEffect.ImageCount := 0; // 内观物品发光效果 读取张数
      StdItem.BodyEffect.OffsetX := 0; // 内观物品发光效果 微调X
      StdItem.BodyEffect.OffsetY := 0; // 内观物品发光效果 微调Y

      StdItem.Effect := Int64(nil);

      g_EffectItemList.Delete(nIndex);
      ListBoxEffectItemList.Items.Delete(ListBoxEffectItemList.ItemIndex);

      ButtonEffectItemSave.Enabled := True;
      Application.MessageBox('删除成功！', '提示信息', MB_OK + MB_ICONWARNING);
      Exit;
    end;
  end
  else
  begin
    g_EffectItemList.Delete(nIndex);
    ListBoxEffectItemList.Items.Delete(nIndex);

    ButtonEffectItemSave.Enabled := True;
    Exit;
  end;
  Application.MessageBox('删除失败！', '提示信息', MB_OK + MB_ICONWARNING);
end;

procedure TFrmViewList2.ButtonEffectItemSaveClick(Sender: TObject);
var
  I, nIndex: Integer;
  ItemEffect: pTItemEffect;
begin
  ButtonEffectItemSave.Enabled := False;
  ButtonEffectItemChg.Enabled := False;
  ButtonEffectItemDel.Enabled := False;
  SaveEffectItemList();
  RebuildDropItemsEffect;
  UserEngine.SendDropItemEffectList;

  g_DropEffectItemList.Clear;
  for I := 0 to g_EffectItemList.Count - 1 do
  begin
    nIndex := Integer(g_EffectItemList.Objects[I]);
    if (nIndex > 0) then
    begin
      ItemEffect := g_ItemEffects.Get(nIndex);

      if (ItemEffect <> nil) and (ItemEffect.FileIndex5 >= 0) and (ItemEffect.ImageCount5 > 0) then
      begin
        g_DropEffectItemList.AddObject(g_EffectItemList.Strings[I], TObject(nIndex));
      end;
    end;
  end;
  g_DropEffectItemList.Sorted := True;
end;

procedure TFrmViewList2.ListViewFoundryItemListClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  ListItem := ListViewFoundryItemList.Selected;
  if ListItem <> nil then
  begin
    SelFoundryItem := pTFoundryItem(ListItem.SubItems.Objects[0]);
    EditFoundryItemRate.Value := SelFoundryItem.nItemRate;
    EditFoundryGiveItemCount.Value := SelFoundryItem.nItemCount;
    RefFoundryNeedItemList(SelFoundryItem);
    ButtonFoundryItemChg.Enabled := True;
    ButtonFoundryItemDel.Enabled := True;
    ButtonFoundryNeedItemAdd.Enabled := True;
  end
  else
  begin
    ButtonFoundryItemChg.Enabled := False;
    ButtonFoundryItemDel.Enabled := False;
    ButtonFoundryNeedItemAdd.Enabled := False;
    SelFoundryItem := nil;
  end;
end;

procedure TFrmViewList2.ListViewFoundryNeedItemListClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  if SelFoundryItem <> nil then
  begin
    ListItem := ListViewFoundryNeedItemList.Selected;
    if ListItem <> nil then
    begin
      SelFoundryNeedItem := pTFoundryNeedItem(ListItem.Data);
      EditFoundryNeedItemDel.Value := SelFoundryNeedItem.btDelete;
      EditFoundryNeedItemCount.Value := SelFoundryNeedItem.nItemCount;
      ComboBoxFoundryNeedItemName.Text := SelFoundryNeedItem.sItemName;
      ButtonFoundryNeedItemChg.Enabled := True;
      ButtonFoundryNeedItemDel.Enabled := True;
    end
    else
    begin
      SelFoundryNeedItem := nil;
      ButtonFoundryNeedItemChg.Enabled := False;
      ButtonFoundryNeedItemDel.Enabled := False;
    end;
  end
  else
  begin
    SelFoundryNeedItem := nil;
    ButtonFoundryNeedItemAdd.Enabled := False;
    ButtonFoundryNeedItemChg.Enabled := False;
    ButtonFoundryNeedItemDel.Enabled := False;
  end;
end;

procedure TFrmViewList2.ListBoxFoundryItemListClick(Sender: TObject);
begin
  ButtonFoundryItemAdd.Enabled := ListBoxFoundryItemList.ItemIndex >= 0;
  if (SelFoundryItem <> nil) and (ListBoxFoundryItemList.ItemIndex >= 0) then
  begin
    ComboBoxFoundryNeedItemName.OnChange := nil;
    ComboBoxFoundryNeedItemName.Text := ListBoxFoundryItemList.Items.Strings[ListBoxFoundryItemList.ItemIndex];
    ComboBoxFoundryNeedItemName.OnChange := ComboBoxFoundryNeedItemNameChange;
  end;
end;

procedure TFrmViewList2.ButtonFoundryItemAddClick(Sender: TObject);
var
  ItemIndex: Integer;
  sItemName: string;
  FoundryItem: pTFoundryItem;
begin
  ItemIndex := ListBoxFoundryItemList.ItemIndex;
  if (ItemIndex >= 0) and (ItemIndex < ListBoxFoundryItemList.Items.Count) then
  begin
    sItemName := ListBoxFoundryItemList.Items.Strings[ItemIndex];
    FoundryItem := GetFoundryItem(sItemName);
    if FoundryItem <> nil then
    begin
      Application.MessageBox('铸造物品列表中已经存在该物品！', '提示信息', MB_OK + MB_ICONWARNING);
      Exit;
    end;
    New(FoundryItem);
    FoundryItem.sItemName := sItemName;
    FoundryItem.nItemCount := EditFoundryGiveItemCount.Value;
    FoundryItem.nItemRate := EditFoundryItemRate.Value;
    FoundryItem.ItemList := TList.Create;
    g_FoundryItemList.Add(FoundryItem);
    ButtonFoundryItemSave.Enabled := True;
    RefFoundryItemList;
  end
  else
  begin
    ButtonFoundryItemAdd.Enabled := False;
    Application.MessageBox('请在物品列表中选择一个需要铸造的物品！', '提示信息', MB_OK + MB_ICONWARNING);
  end;
end;

procedure TFrmViewList2.ButtonFoundryItemChgClick(Sender: TObject);
begin
  if SelFoundryItem <> nil then
  begin
    SelFoundryItem.nItemCount := EditFoundryGiveItemCount.Value;
    SelFoundryItem.nItemRate := EditFoundryItemRate.Value;
    ButtonFoundryItemSave.Enabled := True;
    RefFoundryItemList;
  end;
end;

procedure TFrmViewList2.ButtonFoundryItemDelClick(Sender: TObject);
begin
  if SelFoundryItem <> nil then
  begin
    if DeleteFoundryItem(SelFoundryItem.sItemName) then
    begin
      SelFoundryItem := nil;
      SelFoundryNeedItem := nil;
      ButtonFoundryItemDel.Enabled := False;
      ButtonFoundryItemChg.Enabled := False;
      ButtonFoundryItemSave.Enabled := True;
      RefFoundryItemList;
    end;
  end;
end;

procedure TFrmViewList2.ButtonFoundryNeedItemAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
  FoundryNeedItem: pTFoundryNeedItem;
begin
  if SelFoundryItem <> nil then
  begin
    if ListBoxFoundryItemList.SelCount > 0 then
    begin
      for I := 0 to ListBoxFoundryItemList.Items.Count - 1 do
      begin
        if not ListBoxFoundryItemList.Selected[I] then
          Continue;
        sItemName := ListBoxFoundryItemList.Items[I];
        if sItemName = '' then
        begin
          Application.MessageBox('物品名称不能为空！', '错误信息', MB_OK + MB_ICONERROR);
          ComboBoxFoundryNeedItemName.SetFocus;
          Exit;
        end;

        FoundryNeedItem := GetFoundryNeedItem(SelFoundryItem, sItemName);
        if FoundryNeedItem <> nil then
        begin
          Application.MessageBox('物品名称重复！', '提示信息', MB_OK + MB_ICONWARNING);
          Continue;
        end;

        New(FoundryNeedItem);
        FoundryNeedItem.sItemName := sItemName;
        FoundryNeedItem.nItemCount := EditFoundryNeedItemCount.Value;
        FoundryNeedItem.btDelete := EditFoundryNeedItemDel.Value;
        SelFoundryItem.ItemList.Add(FoundryNeedItem);
        ButtonFoundryItemSave.Enabled := True;
        RefFoundryNeedItemList(SelFoundryItem);
      end;
    end
    else
    begin
      sItemName := ComboBoxFoundryNeedItemName.Text;
      if sItemName = '' then
      begin
        Application.MessageBox('物品名称不能为空！', '错误信息', MB_OK + MB_ICONERROR);
        ComboBoxFoundryNeedItemName.SetFocus;
        Exit;
      end;

      FoundryNeedItem := GetFoundryNeedItem(SelFoundryItem, sItemName);
      if FoundryNeedItem <> nil then
      begin
        Application.MessageBox('物品名称重复！', '提示信息', MB_OK + MB_ICONWARNING);
        Exit;
      end;

      New(FoundryNeedItem);
      FoundryNeedItem.sItemName := sItemName;
      FoundryNeedItem.nItemCount := EditFoundryNeedItemCount.Value;
      FoundryNeedItem.btDelete := EditFoundryNeedItemDel.Value;
      SelFoundryItem.ItemList.Add(FoundryNeedItem);
      ButtonFoundryItemSave.Enabled := True;
      RefFoundryNeedItemList(SelFoundryItem);
    end;
  end;
end;

procedure TFrmViewList2.ButtonFoundryNeedItemChgClick(Sender: TObject);
var
  sItemName: string;
begin
  if (SelFoundryItem <> nil) and (SelFoundryNeedItem <> nil) then
  begin
    sItemName := ComboBoxFoundryNeedItemName.Text;

    if sItemName = '' then
    begin
      Application.MessageBox('物品名称不能为空！', '提示信息', MB_OK + MB_ICONWARNING);
      ComboBoxFoundryNeedItemName.SetFocus;
      Exit;
    end;

    if CompareText(SelFoundryNeedItem.sItemName, sItemName) <> 0 then
    begin
      if GetFoundryNeedItem(SelFoundryItem, sItemName) <> nil then
      begin
        Application.MessageBox('物品名称重复！', '提示信息', MB_OK + MB_ICONWARNING);
        Exit;
      end;
    end;

    SelFoundryNeedItem.sItemName := sItemName;
    SelFoundryNeedItem.nItemCount := EditFoundryNeedItemCount.Value;
    SelFoundryNeedItem.btDelete := EditFoundryNeedItemDel.Value;
    ButtonFoundryNeedItemChg.Enabled := False;
    ButtonFoundryItemSave.Enabled := True;
    RefFoundryNeedItemList(SelFoundryItem);
  end;
end;

procedure TFrmViewList2.ButtonFoundryNeedItemDelClick(Sender: TObject);
var
  sItemName: string;
begin
  if (SelFoundryItem <> nil) and (SelFoundryNeedItem <> nil) then
  begin
    sItemName := SelFoundryNeedItem.sItemName;
    if DeleteFoundryNeedItem(SelFoundryItem, sItemName) then
    begin
      SelFoundryNeedItem := nil;
      ButtonFoundryItemSave.Enabled := True;
      ButtonFoundryNeedItemChg.Enabled := False;
      ButtonFoundryNeedItemDel.Enabled := False;
      RefFoundryNeedItemList(SelFoundryItem);
    end;
  end;
end;

procedure TFrmViewList2.ButtonFoundryItemSaveClick(Sender: TObject);
begin
  ButtonFoundryItemSave.Enabled := False;
  SaveFoundryItemList();
end;

procedure TFrmViewList2.CheckBox5Click(Sender: TObject);
var
  bo12: Boolean;
begin
  if (not CheckBox18.Checked) or CheckBox5.Checked then
    bo12 := False
  else
    bo12 := True;
  CheckBox18.Checked := bo12;
end;

procedure TFrmViewList2.CheckBox18Click(Sender: TObject);
var
  bo12: Boolean;
begin
  if (not CheckBox5.Checked) or CheckBox18.Checked then
    bo12 := False
  else
    bo12 := True;
  CheckBox5.Checked := bo12;
end;

procedure TFrmViewList2.ListBoxGiveItemClick(Sender: TObject);
var
  BoxItem: pTBoxItem;
begin
  if ListBoxGiveItem.ItemIndex >= 0 then
  begin
    // edtBoxItemName.Text := ListBoxGiveItem.Items.Strings[ListBoxGiveItem.ItemIndex];
    BoxItem := pTBoxItem(ListBoxGiveItem.Items.Objects[ListBoxGiveItem.ItemIndex]);
    if BoxItem <> nil then
    begin
      edtBoxItemName.Text := BoxItem.ItemName;
      seBoxItemCount.OnChange := nil;
      seBoxItemCount.Value := BoxItem.ItemCount;
      seBoxItemCount.OnChange := seBoxItemCountChange;
    end;
    ComboBoxBoxItemType.ItemIndex := 0;
    btnDelBoxItem.Enabled := True;
  end
  else
    btnDelBoxItem.Enabled := False;
end;

procedure TFrmViewList2.ListBoxCenterItemClick(Sender: TObject);
var
  BoxItem: pTBoxItem;
begin
  if ListBoxCenterItem.ItemIndex >= 0 then
  begin
    // edtBoxItemName.Text := ListBoxGiveItem.Items.Strings[ListBoxGiveItem.ItemIndex];
    BoxItem := pTBoxItem(ListBoxCenterItem.Items.Objects[ListBoxCenterItem.ItemIndex]);
    if BoxItem <> nil then
    begin
      edtBoxItemName.Text := BoxItem.ItemName;
      seBoxItemCount.OnChange := nil;
      seBoxItemCount.Value := BoxItem.ItemCount;
      seBoxItemCount.OnChange := seBoxItemCountChange;
    end;
    ComboBoxBoxItemType.ItemIndex := 2;
    btnDelBoxItem.Enabled := True;
  end
  else
    btnDelBoxItem.Enabled := False;
end;

procedure TFrmViewList2.ListBoxNoGiveItemClick(Sender: TObject);
var
  BoxItem: pTBoxItem;
begin
  if ListBoxNoGiveItem.ItemIndex >= 0 then
  begin
    // edtBoxItemName.Text := ListBoxGiveItem.Items.Strings[ListBoxGiveItem.ItemIndex];
    BoxItem := pTBoxItem(ListBoxNoGiveItem.Items.Objects[ListBoxNoGiveItem.ItemIndex]);
    if BoxItem <> nil then
    begin
      edtBoxItemName.Text := BoxItem.ItemName;
      seBoxItemCount.OnChange := nil;
      seBoxItemCount.Value := BoxItem.ItemCount;
      seBoxItemCount.OnChange := seBoxItemCountChange;
    end;
    ComboBoxBoxItemType.ItemIndex := 1;
    btnDelBoxItem.Enabled := True;
  end
  else
    btnDelBoxItem.Enabled := False;
end;

procedure TFrmViewList2.ListBoxitemList1Click(Sender: TObject);
begin
  if ListBoxitemList1.ItemIndex >= 0 then
    edtBoxItemName.Text := ListBoxitemList1.Items.Strings[ListBoxitemList1.ItemIndex]
  else
    edtBoxItemName.Text := '';
end;

procedure TFrmViewList2.ButtonShopItemUPClick(Sender: TObject);
var
  ItemIndex: Integer;
  List: TList;
begin
  if (SelShopListView <> nil) and (SelShopItem <> nil) then
  begin
    ItemIndex := SelShopListView.ItemIndex;
    if ItemIndex > 0 then
    begin
      List := g_SndaShopList.Items[PageControlShop.ActivePageIndex];
      List.Remove(SelShopItem);
      List.Insert(ItemIndex - 1, SelShopItem);
      g_SndaShopList.Up(SelShopItem);
      ButtonShopSaveItem.Enabled := True;
      RefShopList;
      ButtonShopItemDOWN.Enabled := (SelShopListView.ItemIndex >= 0) and (SelShopListView.ItemIndex < SelShopListView.Items.Count
        - 1);
      ButtonShopItemUP.Enabled := (SelShopListView.ItemIndex > 0);
      SelShopListView.ItemIndex := ItemIndex - 1;
    end;
  end;
end;

procedure TFrmViewList2.ButtonShopItemDOWNClick(Sender: TObject);
var
  ItemIndex: Integer;
  List: TList;
begin
  if (SelShopListView <> nil) and (SelShopItem <> nil) then
  begin
    ItemIndex := SelShopListView.ItemIndex;
    if (ItemIndex >= 0) and (ItemIndex < SelShopListView.Items.Count - 1) then
    begin
      List := g_SndaShopList.Items[PageControlShop.ActivePageIndex];
      List.Remove(SelShopItem);
      List.Insert(ItemIndex + 1, SelShopItem);
      g_SndaShopList.Down(SelShopItem);
      ButtonShopSaveItem.Enabled := True;
      RefShopList;
      ButtonShopItemDOWN.Enabled := (SelShopListView.ItemIndex >= 0) and (SelShopListView.ItemIndex < SelShopListView.Items.Count
        - 1);
      ButtonShopItemUP.Enabled := (SelShopListView.ItemIndex > 0);
      SelShopListView.ItemIndex := ItemIndex + 1;
    end;
  end;
end;

procedure TFrmViewList2.RadioButtonRateClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGroupItemRule := not RadioButtonRate.Checked;
  Config.WriteBool('Setup', 'GroupItemRule', g_Config.boGroupItemRule);
end;

procedure TFrmViewList2.RadioButtonValueClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boGroupItemRule := RadioButtonValue.Checked;
  Config.WriteBool('Setup', 'GroupItemRule', g_Config.boGroupItemRule);
end;

procedure TFrmViewList2.ButtonShopSaveItemClick(Sender: TObject);
begin
  g_SndaShopList.SaveToFile;
  ButtonShopSaveItem.Enabled := False;
end;

procedure TFrmViewList2.ButtonMsgFilterSaveClick(Sender: TObject);
begin
  g_FilterTexts.SaveToFile;
  ButtonMsgFilterSave.Enabled := False;
end;

procedure TFrmViewList2.ButtonItemRuleSaveClick(Sender: TObject);
begin
  g_ItemRules.SaveToFile;
  ButtonItemRuleSave.Enabled := False;
end;

procedure TFrmViewList2.ButtonUserCommandSaveClick(Sender: TObject);
begin
  g_UserCmds.SaveToFile;
  ButtonUserCommandSave.Enabled := False;
end;

procedure TFrmViewList2.ButtonEffectSaveClick(Sender: TObject);
var
  I, nIndex: Integer;
  ItemEffect: pTItemEffect;
begin
  g_ItemEffects.SaveToFile;
  ButtonEffectSave.Enabled := False;
  RebuildDropItemsEffect;
  UserEngine.SendDropItemEffectList;

  g_DropEffectItemList.Clear;
  for I := 0 to g_EffectItemList.Count - 1 do
  begin
    nIndex := Integer(g_EffectItemList.Objects[I]);
    if (nIndex > 0) then
    begin
      ItemEffect := g_ItemEffects.Get(nIndex);
      if (ItemEffect <> nil) and (ItemEffect.FileIndex5 >= 0) and (ItemEffect.ImageCount5 > 0) then
      begin
        g_DropEffectItemList.AddObject(g_EffectItemList.Strings[I], TObject(nIndex));
      end;
    end;
  end;
  g_DropEffectItemList.Sorted := True;
end;

procedure TFrmViewList2.ButtonGroupItemSaveClick(Sender: TObject);
begin
  g_GroupItems.SaveToFile;
  ButtonGroupItemSave.Enabled := False;
end;

procedure TFrmViewList2.RefFilterItem(ItemType: TFilterItemType);
var
  I: Integer;
  ListItem: TListItem;
  FilterItem: pTFilterItem;
begin
  ListViewFilterItem.Items.BeginUpdate;
  try
    ListViewFilterItem.Clear;
    for I := 0 to g_FilterItemList.Count - 1 do
    begin
      FilterItem := pTFilterItem(g_FilterItemList.Items[I]);
      if (ItemType = i_All) or (FilterItem.ItemType = ItemType) then
      begin
        ListItem := ListViewFilterItem.Items.Add;
        ListItem.Caption := GetFilterItemType(FilterItem.ItemType);
        ListItem.SubItems.AddObject(FilterItem.sItemName, TObject(FilterItem));
        ListItem.Data := FilterItem;
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boHintMsg));
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boPickup));
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boShowName));
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boShowSpecial));
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boAutoMove));
        ListItem.SubItems.Add(IntToStr(FilterItem.btGroupIndex));
      end;
    end;
  finally
    ListViewFilterItem.Items.EndUpdate;
  end;
end;

procedure TFrmViewList2.ListViewFilterItemClick(Sender: TObject);
var
  ListItem: TListItem;
  FilterItem: pTFilterItem;
begin
  ButtonFilterDel.Enabled := False;
  ButtonFilterChg.Enabled := False;
  ListItem := ListViewFilterItem.Selected;
  if ListItem = nil then
    Exit;
  // FilterItem := pTFilterItem(ListItem.SubItems.Objects[0]);
  FilterItem := ListItem.Data;
  if FilterItem = nil then
    Exit;

  if ListViewFilterItem.SelCount = 1 then
  begin
    ComboBoxItemFilter.ItemIndex := Integer(FilterItem.ItemType);
    CheckBoxHintItem.Checked := FilterItem.boHintMsg;
    CheckBoxPickUpItem.Checked := FilterItem.boPickup;
    CheckBoxShowItemName.Checked := FilterItem.boShowName;
    chkShowSpecial.Checked := FilterItem.boShowSpecial;
    chkAutoMove.Checked := FilterItem.boAutoMove;
    EditFilterItemName.Text := FilterItem.sItemName;
    seItemGroupIndex.Value := FilterItem.btGroupIndex;
    ButtonFilterDel.Enabled := True;
    ButtonFilterChg.Enabled := True;
  end;
end;

procedure TFrmViewList2.ComboBoxItemFilterChange(Sender: TObject);
begin
  RefFilterItem(TFilterItemType(ComboBoxItemFilter.ItemIndex));
end;

procedure TFrmViewList2.ButtonFilterAddClick(Sender: TObject);
var
  I: Integer;
  FilterItem: pTFilterItem;
  sItemName: string;
  nType: Integer;
  ListItem: TListItem;
begin
  sItemName := EditFilterItemName.Text;
  nType := ComboBoxItemFilter.ItemIndex;

  { if sItemName = '' then begin
    Application.MessageBox('请输入物品名称！', '提示信息', MB_ICONQUESTION);
    Exit;
    end;
    if FindFilterItemName(sItemName) <> '' then begin
    Application.MessageBox('此物品已经添加！', '提示信息', MB_ICONQUESTION);
    Exit;
    end; }

  if nType <= 0 then
  begin
    Application.MessageBox('请选择物品类型！', '提示信息', MB_ICONQUESTION);
    Exit;
  end;

  for I := 0 to ListBoxitemList4.Items.Count - 1 do
  begin
    if ListBoxitemList4.Selected[I] then
    begin
      if FindFilterItemName(ListBoxitemList4.Items[I]) = '' then
      begin
        New(FilterItem);
        FilterItem.sItemName := ListBoxitemList4.Items[I];
        FilterItem.ItemType := TFilterItemType(nType);
        FilterItem.boHintMsg := CheckBoxHintItem.Checked;
        FilterItem.boPickup := CheckBoxPickUpItem.Checked;
        FilterItem.boShowName := CheckBoxShowItemName.Checked;
        FilterItem.boShowSpecial := chkShowSpecial.Checked;
        FilterItem.boAutoMove := chkAutoMove.Checked;
        FilterItem.btGroupIndex := seItemGroupIndex.Value;
        g_FilterItemList.Add(FilterItem);

        ListItem := ListViewFilterItem.Items.Add;
        ListItem.Caption := GetFilterItemType(FilterItem.ItemType);
        ListItem.SubItems.AddObject(FilterItem.sItemName, TObject(FilterItem));
        ListItem.Data := FilterItem;
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boHintMsg));
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boPickup));
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boShowName));
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boShowSpecial));
        ListItem.SubItems.Add(BooleanToStr(FilterItem.boAutoMove));
        ListItem.SubItems.Add(IntToStr(FilterItem.btGroupIndex));
      end;
    end;
  end;
  // RefFilterItem(TFilterItemType(nType));
  ButtonFilterSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonFilterDelClick(Sender: TObject);
var
  I: Integer;
  ListItem: TListItem;
  FilterItem: pTFilterItem;
begin
  ListItem := ListViewFilterItem.Selected;
  if ListItem = nil then
    Exit;
  // FilterItem := pTFilterItem(ListItem.SubItems.Objects[0]);
  FilterItem := ListItem.Data;
  if FilterItem = nil then
    Exit;
  for I := 0 to g_FilterItemList.Count - 1 do
  begin
    if g_FilterItemList.Items[I] = FilterItem then
    begin
      Dispose(pTFilterItem(g_FilterItemList.Items[I]));
      g_FilterItemList.Delete(I);
      Break;
    end;
  end;
  RefFilterItem(TFilterItemType(ComboBoxItemFilter.ItemIndex));
  ButtonFilterDel.Enabled := False;
  ButtonFilterSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonFilterChgClick(Sender: TObject);
var
  ListItem: TListItem;
  FilterItem: pTFilterItem;
begin
  ListItem := ListViewFilterItem.Selected;
  if ListItem = nil then
    Exit;
  FilterItem := ListItem.Data;
  if FilterItem = nil then
    Exit;
  FilterItem.boHintMsg := CheckBoxHintItem.Checked;
  FilterItem.boPickup := CheckBoxPickUpItem.Checked;
  FilterItem.boShowName := CheckBoxShowItemName.Checked;
  FilterItem.boShowSpecial := chkShowSpecial.Checked;
  FilterItem.boAutoMove := chkAutoMove.Checked;
  FilterItem.btGroupIndex := seItemGroupIndex.Value;

  ListItem.SubItems.Strings[1] := BooleanToStr(FilterItem.boHintMsg);
  ListItem.SubItems.Strings[2] := BooleanToStr(FilterItem.boPickup);
  ListItem.SubItems.Strings[3] := BooleanToStr(FilterItem.boShowName);
  ListItem.SubItems.Strings[4] := BooleanToStr(FilterItem.boShowSpecial);
  ListItem.SubItems.Strings[5] := BooleanToStr(FilterItem.boAutoMove);
  ListItem.SubItems.Strings[5] := IntToStr(FilterItem.btGroupIndex);

  ButtonFilterSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonFilterSaveClick(Sender: TObject);
begin
  SaveFilterItemList;
  ButtonFilterSave.Enabled := False;
  UserEngine.SendFilterItemList;
end;

procedure TFrmViewList2.MemoTzItemDescChange(Sender: TObject);
begin
  ButtonTzItemDescSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonTzItemDescSaveClick(Sender: TObject);
begin
  g_TzItemDescList.Clear;
  g_TzItemDescList.Text := MemoTzItemDesc.Lines.Text;
  SaveTzItemDescList;
  ButtonTzItemDescSave.Enabled := False;
  UserEngine.SendTzItemDescList;
end;

procedure TFrmViewList2.MemoItemDescChange(Sender: TObject);
begin
  btnItemDescSave.Enabled := True;
end;

procedure TFrmViewList2.btnItemDescSaveClick(Sender: TObject);
begin
  g_ItemDescList.Clear;
  g_ItemDescList.Text := MemoItemDesc.Lines.Text;
  SaveItemDescList;
  btnItemDescSave.Enabled := False;
  UserEngine.SendItemDescList;
end;

procedure TFrmViewList2.ButtonFilterAddAllClick(Sender: TObject);

  function GetItemType(nStdMode: Integer): TFilterItemType;
  begin
    case nStdMode of
      0..2, 25:
        Result := i_HPMPDurg;
      10, 11:
        Result := i_Dress;
      5, 6:
        Result := i_Weapon;
      28, 30:
        Result := i_Decorate;
      19, 20, 21:
        Result := i_Jewelry;
      15:
        Result := i_Jewelry;
      24, 26:
        Result := i_Jewelry;
      22, 23:
        Result := i_Jewelry;
      51:
        Result := i_Decoration;
      54, 64:
        Result := i_Decorate;
      52, 62:
        Result := i_Decorate;
      53, 63:
        Result := i_Decorate;
    else
      Result := i_Other;
    end;
  end;

var
  I: Integer;
  boFind: Boolean;
  FilterItem: pTFilterItem;
begin

  for I := 0 to g_FilterItemList.Count - 1 do
  begin
    Dispose(pTFilterItem(g_FilterItemList.Items[I]));
  end;
  g_FilterItemList.Clear;

  for I := 0 to ListBoxitemList4.Items.Count - 1 do
  begin
    New(FilterItem);
    FilterItem.sItemName := ListBoxitemList4.Items[I];
    FilterItem.ItemType := GetItemType(pTStdItem(ListBoxitemList4.Items.Objects[I]).StdMode);
    FilterItem.boHintMsg := CheckBoxHintItem.Checked;
    FilterItem.boPickup := CheckBoxPickUpItem.Checked;
    FilterItem.boShowName := CheckBoxShowItemName.Checked;
    FilterItem.boShowSpecial := chkShowSpecial.Checked;
    FilterItem.boAutoMove := chkAutoMove.Checked;
    FilterItem.btGroupIndex := seItemGroupIndex.Value;
    g_FilterItemList.Add(FilterItem);
  end;

  boFind := False;
  for I := 0 to g_FilterItemList.Count - 1 do
  begin
    FilterItem := g_FilterItemList.Items[I];
    if CompareText(FilterItem.sItemName, '金币') = 0 then
    begin
      boFind := True;
      Break;
    end;
  end;

  if not boFind then
  begin
    New(FilterItem);
    FilterItem.ItemType := i_Other;
    FilterItem.sItemName := '金币';
    FilterItem.boHintMsg := CheckBoxHintItem.Checked;
    FilterItem.boPickup := CheckBoxPickUpItem.Checked;
    FilterItem.boShowName := CheckBoxShowItemName.Checked;
    FilterItem.boShowSpecial := chkShowSpecial.Checked;
    FilterItem.boAutoMove := chkAutoMove.Checked;
    FilterItem.btGroupIndex := seItemGroupIndex.Value;
    g_FilterItemList.Add(FilterItem);
  end;

  RefFilterItem(i_All);
  ButtonFilterSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonFilterDelAllClick(Sender: TObject);
var
  I: Integer;
  FilterItem: pTFilterItem;
begin
  for I := 0 to g_FilterItemList.Count - 1 do
  begin
    Dispose(pTFilterItem(g_FilterItemList.Items[I]));
  end;
  g_FilterItemList.Clear;
  New(FilterItem);
  FilterItem.sItemName := '金币';
  FilterItem.ItemType := i_Other;
  FilterItem.boHintMsg := CheckBoxHintItem.Checked;
  FilterItem.boPickup := CheckBoxPickUpItem.Checked;
  FilterItem.boShowName := CheckBoxShowItemName.Checked;
  FilterItem.boShowSpecial := chkShowSpecial.Checked;
  FilterItem.boAutoMove := chkAutoMove.Checked;
  FilterItem.btGroupIndex := seItemGroupIndex.Value;
  g_FilterItemList.Add(FilterItem);

  RefFilterItem(i_All);
  ButtonFilterSave.Enabled := True;
end;

procedure TFrmViewList2.ButtonGroupItemSkillPowerClick(Sender: TObject);
begin
  if SelGroupItem <> nil then
  begin
    if ShowFrmGroupItemSkillPower then
    begin
      Move(SelAttackSkillPercent, SelGroupItem.AttackSkillPercent, SizeOf(SelAttackSkillPercent));
      Move(SelDefenseSkillPercent, SelGroupItem.DefenseSkillPercent, SizeOf(SelDefenseSkillPercent));

      ButtonGroupItemSave.Enabled := True;
    end;
  end;
end;

procedure TFrmViewList2.ListBoxitemList5Click(Sender: TObject);
begin
  ButtonAddSkillPowerItem.Enabled := True;
end;

procedure TFrmViewList2.ListBoxSkillPowerItemClick(Sender: TObject);
var
  I: Integer;
  SkillPowerItem: pTSkillPowerItem;
begin
  SelSkillPowerItem := nil;
  ButtonDelSkillPowerItem.Enabled := False;
  if ListBoxSkillPowerItem.ItemIndex >= 0 then
  begin
    GroupBoxSkillPowerItem.Caption := Format('%s的技能威力百分比设置', [ListBoxSkillPowerItem.Items.Strings[ListBoxSkillPowerItem.ItemIndex]]);
    SkillPowerItem := pTSkillPowerItem(ListBoxSkillPowerItem.Items.Objects[ListBoxSkillPowerItem.ItemIndex]);
    for I := 1 to High(SkillPowerItem.AttackSkillPercent) do
    begin
      StringGridSkillPower.Cells[1, I] := IntToStr(SkillPowerItem.AttackSkillPercent[I]);
      StringGridSkillPower.Cells[2, I] := IntToStr(SkillPowerItem.DefenseSkillPercent[I]);
    end;
    SelSkillPowerItem := SkillPowerItem;
    ButtonDelSkillPowerItem.Enabled := True;
  end;
end;

procedure TFrmViewList2.ButtonDelSkillPowerItemClick(Sender: TObject);
var
  I: Integer;
begin
  if SelSkillPowerItem <> nil then
  begin
    g_SkillPowerItemList.Lock;
    try
      for I := 0 to g_SkillPowerItemList.Count - 1 do
      begin
        if pTSkillPowerItem(g_SkillPowerItemList.Objects[I]) = SelSkillPowerItem then
        begin
          Dispose(SelSkillPowerItem);
          SelSkillPowerItem := nil;
          g_SkillPowerItemList.Delete(I);

          ListBoxSkillPowerItem.Clear;
          ListBoxSkillPowerItem.Items.AddStrings(g_SkillPowerItemList);

          ButtonDelSkillPowerItem.Enabled := False;
          ButtonSaveSkillPowerItem.Enabled := True;
          Break;
        end;
      end;
    finally
      g_SkillPowerItemList.UnLock;
    end;
  end;
end;

procedure TFrmViewList2.ButtonChgSkillPowerItemClick(Sender: TObject);
var
  I: Integer;
begin
  if SelSkillPowerItem <> nil then
  begin
    for I := 1 to High(SelSkillPowerItem.AttackSkillPercent) do
    begin
      SelSkillPowerItem.AttackSkillPercent[I] := StrToIntDef(Trim(StringGridSkillPower.Cells[1, I]), 0);
      SelSkillPowerItem.DefenseSkillPercent[I] := StrToIntDef(Trim(StringGridSkillPower.Cells[2, I]), 0);
    end;
    ButtonChgSkillPowerItem.Enabled := False;
    ButtonSaveSkillPowerItem.Enabled := True;
  end;
end;

procedure TFrmViewList2.ButtonSaveSkillPowerItemClick(Sender: TObject);
begin
  ButtonSaveSkillPowerItem.Enabled := False;
  SaveSkillPowerItemList;
end;

procedure TFrmViewList2.ButtonAddSkillPowerItemClick(Sender: TObject);
var
  I: Integer;
  SkillPowerItem: pTSkillPowerItem;
  sItemName: string;
begin
  if ListBoxitemList5.ItemIndex >= 0 then
  begin
    sItemName := ListBoxitemList5.Items.Strings[ListBoxitemList5.ItemIndex];
    if GetSkillPowerItem(sItemName) <> nil then
    begin
      Application.MessageBox('该物品已经在装备技能威力列表中了！', '提示信息', MB_ICONQUESTION);
      Exit;
    end;
    g_SkillPowerItemList.Lock;
    try
      New(SkillPowerItem);
      FillChar(SkillPowerItem^.AttackSkillPercent, SizeOf(SkillPowerItem^.AttackSkillPercent), 0);
      FillChar(SkillPowerItem^.DefenseSkillPercent, SizeOf(SkillPowerItem^.DefenseSkillPercent), 0);
      for I := 1 to High(SkillPowerItem^.AttackSkillPercent) do
      begin
        SkillPowerItem^.AttackSkillPercent[I] := StrToIntDef(Trim(StringGridSkillPower.Cells[1, I]), 0);
        SkillPowerItem^.DefenseSkillPercent[I] := StrToIntDef(Trim(StringGridSkillPower.Cells[2, I]), 0);
        // if I=0 then

          // showmessage(Format('%d %d %s',[SkillPowerItem.AttackSkillPercent[I +1],SkillPowerItem.DefenseSkillPercent[I +1],StringGridSkillPower.Cells[2, I +1]]));
      end;
      g_SkillPowerItemList.AddObject(sItemName, TObject(SkillPowerItem));
      ListBoxSkillPowerItem.Clear;
      ListBoxSkillPowerItem.Items.AddStrings(g_SkillPowerItemList);
    finally
      g_SkillPowerItemList.UnLock;
    end;
      {
      SkillPowerItem := pTSkillPowerItem(ListBoxSkillPowerItem.Items.Objects[0]);
      showmessage(Format('%d %d %s',[SkillPowerItem.AttackSkillPercent[1],SkillPowerItem.DefenseSkillPercent[1],StringGridSkillPower.Cells[2, 1]])); }
    // SkillPowerItem := pTSkillPowerItem(g_SkillPowerItemList.Objects[0]);
    // SkillPowerItem.DefenseSkillPercent[1]:=10;
    // showmessage(Format('%d %d', [SkillPowerItem.AttackSkillPercent[1], SkillPowerItem.DefenseSkillPercent[1]]));

    ButtonSaveSkillPowerItem.Enabled := True;
  end;
end;

procedure TFrmViewList2.StringGridSkillPowerSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
begin
  ButtonChgSkillPowerItem.Enabled := (SelSkillPowerItem <> nil); // and (StringGridSkillPower.Cells[ACol, ARow] <> Value)
end;

procedure TFrmViewList2.chkNextClick(Sender: TObject);
begin
  seCount.Enabled := chkNext.Checked;
  if SelBox <> nil then
  begin
    SelBox.BoxSet.boNext := chkNext.Checked;
    btnSaveBoxItem.Enabled := True;
  end;
end;

procedure TFrmViewList2.ListBoxEndNoGiveItemClick(Sender: TObject);
var
  BoxItem: pTBoxItem;
begin
  if ListBoxEndNoGiveItem.ItemIndex >= 0 then
  begin
    // edtBoxItemName.Text := ListBoxGiveItem.Items.Strings[ListBoxGiveItem.ItemIndex];
    BoxItem := pTBoxItem(ListBoxEndNoGiveItem.Items.Objects[ListBoxEndNoGiveItem.ItemIndex]);
    if BoxItem <> nil then
    begin
      edtBoxItemName.Text := BoxItem.ItemName;
      seBoxItemCount.OnChange := nil;
      seBoxItemCount.Value := BoxItem.ItemCount;
      seBoxItemCount.OnChange := seBoxItemCountChange;
    end;
    ComboBoxBoxItemType.ItemIndex := 3;
    btnDelBoxItem.Enabled := True;
  end
  else
    btnDelBoxItem.Enabled := False;
end;

procedure TFrmViewList2.cbbOtherItemChange(Sender: TObject);
begin
  edtBoxItemName.Text := cbbOtherItem.Items[cbbOtherItem.ItemIndex];
end;

procedure TFrmViewList2.seCountChange(Sender: TObject);
begin
  if SelBox <> nil then
  begin
    SelBox.BoxSet.nCount := seCount.Value;
    btnSaveBoxItem.Enabled := True;
  end;

end;

procedure TFrmViewList2.seGoldChange(Sender: TObject);
begin
  if SelBox <> nil then
  begin
    SelBox.BoxSet.nGold := seGold.Value;
    btnSaveBoxItem.Enabled := True;
  end;
end;

procedure TFrmViewList2.seGameGoldChange(Sender: TObject);
begin
  if SelBox <> nil then
  begin
    SelBox.BoxSet.nGameGold := seGameGold.Value;
    btnSaveBoxItem.Enabled := True;
  end;
end;

procedure TFrmViewList2.seAddGoldChange(Sender: TObject);
begin
  if SelBox <> nil then
  begin
    SelBox.BoxSet.nAddGold := seAddGold.Value;
    btnSaveBoxItem.Enabled := True;
  end;

end;

procedure TFrmViewList2.seAddGameGoldChange(Sender: TObject);
begin
  if SelBox <> nil then
  begin
    SelBox.BoxSet.nAddGameGold := seAddGameGold.Value;
    btnSaveBoxItem.Enabled := True;
  end;
end;

procedure TFrmViewList2.seEndGoldChange(Sender: TObject);
begin
  if SelBox <> nil then
  begin
    SelBox.BoxSet.nEndGold := seEndGold.Value;
    btnSaveBoxItem.Enabled := True;
  end;
end;

procedure TFrmViewList2.seEndGameGoldChange(Sender: TObject);
begin
  if SelBox <> nil then
  begin
    SelBox.BoxSet.nEndGameGold := seEndGameGold.Value;
    btnSaveBoxItem.Enabled := True;
  end;
end;

procedure TFrmViewList2.chkSendFilterItemListClick(Sender: TObject);
begin
  g_Config.boSendFilterItemList := chkSendFilterItemList.Checked;
  Config.WriteBool('Setup', 'SendFilterItemList', g_Config.boSendFilterItemList);
end;

procedure TFrmViewList2.chkEnablePlayerUseClientPickItemsClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnablePlayerUseClientPickItems := chkEnablePlayerUseClientPickItems.Checked;


  // VMProtectBegin('VMProtect_UseClientPickItems2');

  if g_nKey_UseClientPickItems = 1 then
  begin
    Config.WriteBool('Setup', 'EnablePlayerUseClientPickItems', g_Config.boEnablePlayerUseClientPickItems);
  end;

  // VMProtectEnd();

end;

procedure TFrmViewList2.chkEnableHeroUseClientPickItemsClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  g_Config.boEnableHeroUseClientPickItems := chkEnableHeroUseClientPickItems.Checked;

  // VMProtectBegin('VMProtect_UseClientPickItems3');

  if g_nKey_UseClientPickItems = 1 then
  begin
    Config.WriteBool('Setup', 'EnableHeroUseClientPickItems', g_Config.boEnableHeroUseClientPickItems);
  end;

  // VMProtectEnd();
end;

procedure TFrmViewList2.chkSendItemDescListClick(Sender: TObject);
begin
  g_Config.boSendItemDescList := chkSendItemDescList.Checked;
  Config.WriteBool('Setup', 'SendItemDescList', g_Config.boSendItemDescList);
end;

procedure TFrmViewList2.chkSendTzItemDescListClick(Sender: TObject);
begin
  g_Config.boSendTzItemDescList := chkSendTzItemDescList.Checked;
  Config.WriteBool('Setup', 'SendTzItemDescList', g_Config.boSendTzItemDescList);
end;

procedure TFrmViewList2.chkSingleHintClick(Sender: TObject);
begin
  g_Config.boSingleHint := chkSingleHint.Checked;
  Config.WriteBool('Setup', 'SingleHint', g_Config.boSingleHint);
  UserEngine.SendServerConfig;
end;

procedure TFrmViewList2.seBoxItemCountChange(Sender: TObject);
var
  BoxItem: pTBoxItem;
  ListBox: TListBox;
begin
  if ComboBoxBoxItemType.ItemIndex >= 0 then
  begin
    case ComboBoxBoxItemType.ItemIndex of
      0:
        ListBox := ListBoxGiveItem;
      1:
        ListBox := ListBoxNoGiveItem;
      2:
        ListBox := ListBoxCenterItem;
    else
      ListBox := ListBoxEndNoGiveItem;
    end;

    if ListBox.ItemIndex >= 0 then
    begin
      BoxItem := pTBoxItem(ListBox.Items.Objects[ListBox.ItemIndex]);
      if BoxItem <> nil then
      begin
        BoxItem.ItemCount := seBoxItemCount.Value;
      end;
    end;
  end;
end;

procedure TFrmViewList2.chkSkillPowerItemUseHumClick(Sender: TObject);
begin
  g_Config.boSkillPowerItemUseHum := chkSkillPowerItemUseHum.Checked;
  Config.WriteBool('Setup', 'SkillPowerItemUseHum', g_Config.boSkillPowerItemUseHum);
end;

procedure TFrmViewList2.chkSkillPowerItemUseMonClick(Sender: TObject);
begin
  g_Config.boSkillPowerItemUseMon := chkSkillPowerItemUseMon.Checked;
  Config.WriteBool('Setup', 'SkillPowerItemUseMon', g_Config.boSkillPowerItemUseMon);
end;

procedure TFrmViewList2.mniN3Click(Sender: TObject);
var
  ListItem: TListItem;
  FilterItem: pTFilterItem;
  MenuItem: TMenuItem;
  boValue: Boolean;
  I: Integer;
begin
  MenuItem := Sender as TMenuItem;
  boValue := MenuItem.Tag = 0;

  for I := 0 to ListViewFilterItem.Items.Count - 1 do
  begin
    ListItem := ListViewFilterItem.Items[I];
    if not ListItem.Selected then
      Continue;

    FilterItem := ListItem.Data;

    if (MenuItem.Parent <> nil) and (MenuItem.Parent.Tag = 0) then
    begin
      FilterItem.boHintMsg := boValue;
      ListItem.SubItems.Strings[1] := BooleanToStr(boValue);
    end
    else if (MenuItem.Parent <> nil) and (MenuItem.Parent.Tag = 1) then
    begin
      FilterItem.boPickup := boValue;
      ListItem.SubItems.Strings[2] := BooleanToStr(boValue);
    end
    else if (MenuItem.Parent <> nil) and (MenuItem.Parent.Tag = 2) then
    begin
      FilterItem.boShowName := boValue;
      ListItem.SubItems.Strings[3] := BooleanToStr(boValue);
    end
    else if (MenuItem.Parent <> nil) and (MenuItem.Parent.Tag = 3) then
    begin
      FilterItem.boShowSpecial := boValue;
      ListItem.SubItems.Strings[4] := BooleanToStr(boValue);
    end
    else if (MenuItem.Parent <> nil) and (MenuItem.Parent.Tag = 4) then
    begin
      FilterItem.boAutoMove := boValue;
      ListItem.SubItems.Strings[5] := BooleanToStr(boValue);
    end;
  end;

  ButtonFilterSave.Enabled := True;
end;

procedure TFrmViewList2.N1Click(Sender: TObject);
var
  I: Integer;
  IntPutStr: string;
  ListItem: TListItem;
  FilterItem: pTFilterItem;
begin
  if InputQuery('批量设置分组序号', '请输入序号(0~255)', IntPutStr) then
  begin
    if not (StrToIntDef(IntPutStr, -1) in [0..255]) then
    begin
      Application.MessageBox('输入的内容只接受纯数字，范围[0~255]', '信息输入有误', MB_ICONERROR);
      Exit;
    end;

    for I := 0 to ListViewFilterItem.Items.Count - 1 do
    begin
      ListItem := ListViewFilterItem.Items[I];
      if not ListItem.Selected then
        Continue;

      FilterItem := ListItem.Data;
      FilterItem.btGroupIndex := StrToIntDef(IntPutStr, -1);
      ListItem.SubItems.Strings[6] := IntPutStr;
    end;
  end;
end;

procedure TFrmViewList2.chkEnabledBuyShopItemGiveClick(Sender: TObject);
begin
  g_Config.boEnabledBuyShopItemGive := chkEnabledBuyShopItemGive.Checked;
  Config.WriteBool('Setup', 'EnabledBuyShopItemGive', g_Config.boEnabledBuyShopItemGive);
  UserEngine.SendServerConfig;
end;

procedure TFrmViewList2.FormCreate(Sender: TObject);
var
  I: Integer;
  CheckBox: TCheckBox;
  MenuItem, SubMenuIem: TMenuItem;
begin
  pmRuleList.Items.Clear;
  for I := 0 to GroupBoxItemRule.ControlCount - 1 do
  begin
    if GroupBoxItemRule.Controls[I] is TCheckBox then
    begin
      CheckBox := TCheckBox(GroupBoxItemRule.Controls[I]);
      if CheckBox.Tag >= 0 then
      begin
        MenuItem := TMenuItem.Create(Self);
        MenuItem.Caption := CheckBox.Caption;
        pmRuleList.Items.Add(MenuItem);

        SubMenuIem := TMenuItem.Create(Self);
        SubMenuIem.Caption := '批量设置';
        SubMenuIem.Tag := CheckBox.Tag * 1000 + 1;
        SubMenuIem.OnClick := OnItemRuleBathSettingClick;

        MenuItem.Add(SubMenuIem);

        SubMenuIem := TMenuItem.Create(Self);
        SubMenuIem.Caption := '批量取消';
        SubMenuIem.Tag := CheckBox.Tag * 1000 + 0;

        MenuItem.Add(SubMenuIem);
        SubMenuIem.OnClick := OnItemRuleBathSettingClick;
      end;
    end;
  end;

  cbbAuctionPricesType.Items.Add(g_Config.sGameGoldName);
  cbbAuctionPricesType.Items.Add(g_Config.sGamePointName);
  cbbAuctionPricesType.Items.Add(sSTRING_GOLDNAME);
  cbbAuctionPricesType.Items.Add(g_Config.sGameDiamondName);
  cbbAuctionPricesType.Items.Add(g_Config.sGameGirdName);
  cbbAuctionPricesType.ItemIndex := 0;
end;

procedure TFrmViewList2.OnItemRuleBathSettingClick(Sender: TObject);
var
  I, Index, Value: Integer;
  MenuItem: TMenuItem;
  ItemRule: pTItemRule;
begin
  if Sender is TMenuItem then
    MenuItem := Sender as TMenuItem
  else
    Exit;

  Value := MenuItem.Tag mod 1000;
  Index := (MenuItem.Tag - Value) div 1000;

  if (Index < Low(TFlagArray)) or (Index > High(TFlagArray)) then
    Exit;

  for I := 0 to ListBoxItemRuleList.Count - 1 do
  begin
    if not ListBoxItemRuleList.Selected[I] then
      Continue;

    ItemRule := g_ItemRules.Find(ListBoxItemRuleList.Items[I]);
    if ItemRule <> nil then
    begin
      ItemRule.FlagArray[Index] := Value <> 0;
    end;
  end;
  RefItemRuleList;
  ButtonItemRuleSave.Enabled := False;
  g_ItemRules.SaveToFile;
end;

procedure TFrmViewList2.lbl33Click(Sender: TObject);
begin
  cbbEffectFileIndex1.ItemIndex := 0;
  seItemEffectOffset1.Value := -1;
  seItemEffectImageCount1.Value := 0;
  seItemEffectOffsetX1.Value := 0;
  seItemEffectOffsetY1.Value := 0;
end;

procedure TFrmViewList2.Label85Click(Sender: TObject);
begin
  cbbEffectFileIndex2.ItemIndex := 0;
  seItemEffectOffset2.Value := -1;
end;

procedure TFrmViewList2.Label86Click(Sender: TObject);
begin
  cbbAddEffectFileIndex.ItemIndex := 0;
  seAddEffectOffset.Value := -1;
end;

procedure TFrmViewList2.Label87Click(Sender: TObject);
begin
  cbbEffectFileIndex3.ItemIndex := 0;
  seItemEffectOffset3.Value := -1;
  seItemEffectImageCount3.Value := 0;
  seItemEffectOffsetX3.Value := 0;
  seItemEffectOffsetY3.Value := 0;
end;

procedure TFrmViewList2.lbl33MouseEnter(Sender: TObject);
begin
  (Sender as TLabel).Font.Style := [fsUnderline];
end;

procedure TFrmViewList2.lbl33MouseLeave(Sender: TObject);
begin
  (Sender as TLabel).Font.Style := [];
end;

procedure TFrmViewList2.btnWilEditClick(Sender: TObject);
var
  I: Integer;
  sWilName: string;
begin
  if ListBoxWilNameList.ItemIndex >= 0 then
  begin
    sWilName := Trim(EditWilName.Text);
    if Length(sWilName) = 0 then
    begin
      Application.MessageBox('请输入WIL文件名称！', '错误信息', MB_OK + MB_ICONERROR);
      EditWilName.SetFocus;
      Exit;
    end;

    for I := 0 to ListBoxWilNameList.Items.Count - 1 do
    begin
      if (I <> ListBoxWilNameList.ItemIndex) and SameText(ListBoxWilNameList.Items.Strings[I], sWilName) then
      begin
        Application.MessageBox('此WIL文件名称已经在列表中了！', '错误信息', MB_OK + MB_ICONERROR);
        EditWilName.SetFocus;
        Exit;
      end;
    end;

    ListBoxWilNameList.Items[ListBoxWilNameList.ItemIndex] := sWilName;
    btnWilSave.Enabled := True;
    btnWilNameDown.Enabled := (ListBoxWilNameList.ItemIndex >= 0) and (ListBoxWilNameList.ItemIndex < ListBoxWilNameList.Count - 1);
    btnWilNameUP.Enabled := (ListBoxWilNameList.ItemIndex > 0);
  end;
end;

procedure TFrmViewList2.mmoItemDescTopChange(Sender: TObject);
begin
  btnItemDescTopSave.Enabled := True;
end;

procedure TFrmViewList2.chkSendItemDescTopListClick(Sender: TObject);
begin
  g_Config.boSendItemDescTopList := chkSendItemDescTopList.Checked;
  Config.WriteBool('Setup', 'SendItemDescTopList', g_Config.boSendItemDescTopList);
end;

procedure TFrmViewList2.btnItemDescTopSaveClick(Sender: TObject);
begin
  g_ItemDescTopList.Clear;
  g_ItemDescTopList.Text := mmoItemDescTop.Lines.Text;
  SaveItemDescTopList;
  btnItemDescTopSave.Enabled := False;
  UserEngine.SendItemDescTopList;
end;

procedure TFrmViewList2.Label94Click(Sender: TObject);
begin
  cbbEffectFileIndex5.ItemIndex := 0;
  seItemEffectOffset5.Value := -1;
  seItemEffectImageCount5.Value := 0;
  seItemEffectOffsetX5.Value := 0;
  seItemEffectOffsetY5.Value := 0;
end;

procedure TFrmViewList2.chkDescSupportRenamItemClick(Sender: TObject);
begin
  g_Config.boDescSupportRenamItem := chkDescSupportRenamItem.Checked;
  Config.WriteBool('Setup', 'DescSupportRenamItem', g_Config.boDescSupportRenamItem);
  UserEngine.SendServerConfig;
end;

procedure TFrmViewList2.chkTZSupportRenameItemClick(Sender: TObject);
begin
  g_Config.boTZSupportRenameItem := chkTZSupportRenameItem.Checked;
  Config.WriteBool('Setup', 'TZSupportRenameItem', g_Config.boTZSupportRenameItem);
  UserEngine.SendServerConfig;
end;

procedure TFrmViewList2.chkNoRenameDescReadDefaultClick(Sender: TObject);
begin
  g_Config.boNoRenameDescReadDefault := chkNoRenameDescReadDefault.Checked;
  Config.WriteBool('Setup', 'NoRenameDescReadDefault', g_Config.boNoRenameDescReadDefault);
  UserEngine.SendServerConfig;
end;

procedure TFrmViewList2.ComboBoxFoundryNeedItemNameChange(Sender: TObject);
begin
  ListBoxFoundryItemList.ClearSelection;
end;

procedure TFrmViewList2.cbbAuctionPricesTypeChange(Sender: TObject);
begin

  seAuctionPrices_Min.Value := FAcutionPricesLime.Min[cbbAuctionPricesType.ItemIndex];
  seAuctionPrices_Max.Value := FAcutionPricesLime.Max[cbbAuctionPricesType.ItemIndex];
end;

procedure TFrmViewList2.seAuctionPrices_MinChange(Sender: TObject);
begin
  FAcutionPricesLime.Min[cbbAuctionPricesType.ItemIndex] := seAuctionPrices_Min.Value;
end;

procedure TFrmViewList2.seAuctionPrices_MaxChange(Sender: TObject);
begin
  FAcutionPricesLime.Max[cbbAuctionPricesType.ItemIndex] := seAuctionPrices_Max.Value;
end;

procedure TFrmViewList2.WMStartEditingCustomMoney(var Message: TMessage);
var
  Node: PVirtualNode;
begin
  Node := Pointer(Message.WPARAM);
  // Note: the test whether a node can really be edited is done in the OnEditing event.
  vstCustomMoney.EditNode(Node, Message.LParam);
end;

procedure TFrmViewList2.RefCustomMoneyList();
var
  I: Integer;
  Node: PVirtualNode;
  NodeData: PCustomMoneyNodeData;
begin
  if vstCustomMoney.IsEditing then
    vstCustomMoney.EndEditNode;
  vstCustomMoney.Clear;
  for I := 0 to g_CustomMoneyList.Count - 1 do
  begin
    Node := vstCustomMoney.AddChild(nil);
    NodeData := vstCustomMoney.GetNodeData(Node);
    NodeData.Money := pTCustomMoney(g_CustomMoneyList.Items[I]); // @FCurrentMonsterCustomConfig.ClientActions[ActionType];
    // Node.CheckType := ctCheckBox;
    // if FCurrentMonsterCustomConfig.ClientActions[ActionType].CalcDir then
    // vstCustomMoney.CheckState[Node] := csCheckedNormal
    // else
    // vstCustomMoney.CheckState[Node] := csUnCheckedNormal;
  end;
end;

destructor TCustomPropertyEditLink.Destroy;
begin
  FEdit.Free;
  inherited;
end;

// ----------------------------------------------------------------------------------------------------------------------

procedure TCustomPropertyEditLink.EditKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
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

procedure TCustomPropertyEditLink.EditKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState);
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

function TCustomPropertyEditLink.BeginEdit: Boolean;
begin
  Result := True;
  FEdit.Show;
  FEdit.SetFocus;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TCustomPropertyEditLink.CancelEdit: Boolean;
begin
  Result := True;
  FEdit.Hide;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TCustomPropertyEditLink.EndEdit: Boolean;
var
  NodeData: PCustomMoneyNodeData;
  TempValue: Integer;
  TempString: string;
  TempBoolean: Boolean;
  IsChanged: Boolean;
begin
  Result := True;
  IsChanged := False;

  NodeData := FTree.GetNodeData(FNode);
  if FEdit is TEdit then
  begin
    TempString := (FEdit as TEdit).Text;
    case FColumn of
      0:
        if NodeData.Money.sName <> TempString then
        begin
          NodeData.Money.sName := TempString;
          IsChanged := True;
        end;
    end;
  end
  else if FEdit is TSpinEditEx then
  begin
    TempValue := (FEdit as TSpinEditEx).Value;
    case FColumn of
      1:
        if NodeData.Money.nGroup <> TempValue then
        begin
          NodeData.Money.nGroup := TempValue;
          IsChanged := True;
        end;
      2:
        if NodeData.Money.nIndex <> TempValue then
        begin
          NodeData.Money.nIndex := TempValue;
          IsChanged := True;
        end;
    end;
  end
  else if FEdit is TCheckBox then
  begin
    TempBoolean := (FEdit as TCheckBox).Checked;
    case FColumn of
      3:
        if NodeData.Money.boLog <> TempBoolean then
        begin
          NodeData.Money.boLog := TempBoolean;
          IsChanged := True;
        end;
      4:
        if NodeData.Money.boCanMyShop <> TempBoolean then
        begin
          NodeData.Money.boCanMyShop := TempBoolean;
          IsChanged := True;
        end;
      5:
        if NodeData.Money.boCanGameShop <> TempBoolean then
        begin
          NodeData.Money.boCanGameShop := TempBoolean;
          IsChanged := True;
        end;
      6:
        if NodeData.Money.boCanAuction <> TempBoolean then
        begin
          NodeData.Money.boCanAuction := TempBoolean;
          IsChanged := True;
        end;
      7:
        if NodeData.Money.boCanSellPlayer <> TempBoolean then
        begin
          NodeData.Money.boCanSellPlayer := TempBoolean;
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
    TFrmViewList2(FTree.Owner).btnSaveCustomMoney.Enabled := True;
    // if (FTree.Owner is TFrmMonsterConfig) then
    // begin
    // TFrmMonsterConfig(FTree.Owner).SetMonsterConfigChanged();
    // end;
  end;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TCustomPropertyEditLink.GetBounds: TRect;
begin
  Result := FEdit.BoundsRect;
end;

// ----------------------------------------------------------------------------------------------------------------------

function TCustomPropertyEditLink.PrepareEdit(Tree: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex): Boolean;
var
  NodeData: PCustomMoneyNodeData;
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
    0:
      begin
        FEdit := TEdit.Create(nil);
        with FEdit as TEdit do
        begin
          Visible := False;
          Parent := Tree;
          Text := NodeData.Money.sName;
        end;
      end;
    1, 2:
      begin
        FEdit := TSpinEditEx.Create(nil);
        with FEdit as TSpinEditEx do
        begin
          Visible := False;
          Parent := Tree;
          MinValue := 0;
          MaxValue := High(Integer);

          case FColumn of
            1:
              Value := NodeData.Money.nGroup;
            2:
              Value := NodeData.Money.nIndex;
          end;

          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
    3, 4, 5, 6, 7:
      begin
        FEdit := TCheckBox.Create(nil);

        with FEdit as TCheckBox do
        begin
          Visible := False;
          Parent := Tree;
          // Checked := True;
          case FColumn of
            3:
              Checked := NodeData.Money.boLog;
            4:
              Checked := NodeData.Money.boCanMyShop;
            5:
              Checked := NodeData.Money.boCanGameShop;
            6:
              Checked := NodeData.Money.boCanAuction;
            7:
              Checked := NodeData.Money.boCanSellPlayer;
          end;
          OnKeyDown := EditKeyDown;
          OnKeyUp := EditKeyUp;
        end;
      end;
  else
    Result := False;
  end;
end;

// ----------------------------------------------------------------------------------------------------------------------

procedure TCustomPropertyEditLink.ProcessMessage(var Message: TMessage);
begin
  FEdit.WindowProc(Message);
end;

// ----------------------------------------------------------------------------------------------------------------------

procedure TCustomPropertyEditLink.SetBounds(R: TRect);
var
  Dummy: Integer;
begin
  // Since we don't want to activate grid extensions in the tree (this would influence how the selection is drawn)
  // we have to set the edit's width explicitly to the width of the column.
  FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right);
  FEdit.BoundsRect := R;
end;

/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

end.


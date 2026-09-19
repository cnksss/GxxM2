unit ViewList;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ComCtrls, StdCtrls, Grids, Grobal2,
  SpinEditEx, M2Threads, M2Definition, Vcl.ExtCtrls, Vcl.Samples.Spin;

type
  TfrmViewList = class(TForm)
    tvPage: TTreeView;
    pnlClient: TPanel;
    pgcViewList: TPageControl;
    ts00: TTabSheet;
    GroupBox3: TGroupBox;
    ListBoxDisableMakeList: TListBox;
    GroupBox4: TGroupBox;
    ListBoxitemList1: TListBox;
    btnAddDisableMakeItem: TButton;
    btnDelDisableMakeItem: TButton;
    btnSaveDisableMakeItem: TButton;
    btnAddAllDisableMakeItem: TButton;
    btnDelAllDisableMakeItem: TButton;
    ts01: TTabSheet;
    GroupBox2: TGroupBox;
    ListBoxItemList: TListBox;
    GroupBox1: TGroupBox;
    ListBoxEnableMakeList: TListBox;
    btnAddEnableMakeItem: TButton;
    btnDelEnableMakeItem: TButton;
    btnSaveEnableMakeItem: TButton;
    btnAddAllEnableMakeItem: TButton;
    btnDelAllEnableMakeItem: TButton;
    ts02: TTabSheet;
    GroupBox8: TGroupBox;
    ListBoxGameLogList: TListBox;
    btnAddLogItem: TButton;
    btnDelLogItem: TButton;
    btnAddAllLogItem: TButton;
    btnDelAllLogItem: TButton;
    btnSaveLogItem: TButton;
    GroupBox9: TGroupBox;
    ListBoxitemList2: TListBox;
    ts03: TTabSheet;
    GroupBox5: TGroupBox;
    ListBoxDisableMoveMap: TListBox;
    btnAddDisabelMoveMap: TButton;
    btnDelDisabelMoveMap: TButton;
    btnAddAllDisabelMoveMap: TButton;
    btnDelAllDisabelMoveMap: TButton;
    btnSaveDisabelMoveMap: TButton;
    GroupBox6: TGroupBox;
    ListBoxMapList: TListBox;
    ts04: TTabSheet;
    GridItemBindAccount: TStringGrid;
    GroupBox16: TGroupBox;
    Label6: TLabel;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    ButtonItemBindAcountMod: TButton;
    EditItemBindAccountItemIdx: TSpinEditEx;
    EditItemBindAccountItemMakeIdx: TSpinEditEx;
    EditItemBindAccountItemName: TEdit;
    ButtonItemBindAcountAdd: TButton;
    ButtonItemBindAcountRef: TButton;
    ButtonItemBindAcountDel: TButton;
    EditItemBindAccountName: TEdit;
    ts05: TTabSheet;
    GridItemBindCharName: TStringGrid;
    GroupBox17: TGroupBox;
    Label10: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    Label13: TLabel;
    ButtonItemBindCharNameMod: TButton;
    EditItemBindCharNameItemIdx: TSpinEditEx;
    EditItemBindCharNameItemMakeIdx: TSpinEditEx;
    EditItemBindCharNameItemName: TEdit;
    ButtonItemBindCharNameAdd: TButton;
    ButtonItemBindCharNameRef: TButton;
    ButtonItemBindCharNameDel: TButton;
    EditItemBindCharNameName: TEdit;
    ts06: TTabSheet;
    GridItemBindIPaddr: TStringGrid;
    GroupBox18: TGroupBox;
    Label14: TLabel;
    Label15: TLabel;
    Label16: TLabel;
    Label17: TLabel;
    ButtonItemBindIPaddrMod: TButton;
    EditItemBindIPaddrItemIdx: TSpinEditEx;
    EditItemBindIPaddrItemMakeIdx: TSpinEditEx;
    EditItemBindIPaddrItemName: TEdit;
    ButtonItemBindIPaddrAdd: TButton;
    ButtonItemBindIPaddrRef: TButton;
    ButtonItemBindIPaddrDel: TButton;
    EditItemBindIPaddrName: TEdit;
    ts07: TTabSheet;
    GroupBox19: TGroupBox;
    lstMoveGuardAllItem: TListBox;
    GroupBox24: TGroupBox;
    lstMoveGuardPickItemList: TListBox;
    btnMoveGuardPickItemAdd: TButton;
    btnMoveGuardPickItemDel: TButton;
    btnMoveGuardPickItemAddAll: TButton;
    btnMoveGuardPickItemDelAll: TButton;
    btnMoveGuardPickItemSave: TButton;
    grp1: TGroupBox;
    chkMoveSuperGuardPickItem: TCheckBox;
    chkMoveSuperGuardAttackMon: TCheckBox;
    chkMoveSuperGuardAttackBB: TCheckBox;
    GroupBox25: TGroupBox;
    chkMoveArcherGuardPickItem: TCheckBox;
    ts08: TTabSheet;
    StringGridMonDropLimit: TStringGrid;
    GroupBox7: TGroupBox;
    Label29: TLabel;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label18: TLabel;
    ButtonMonDropLimitSave: TButton;
    EditDropCount: TSpinEditEx;
    EditCountLimit: TSpinEditEx;
    EditNoDropCount: TSpinEditEx;
    EditItemName: TEdit;
    ButtonMonDropLimitAdd: TButton;
    ButtonMonDropLimitRef: TButton;
    ButtonMonDropLimitDel: TButton;
    seClearDay: TSpinEditEx;
    ts09: TTabSheet;
    GroupBox10: TGroupBox;
    ListBoxDisableTakeOffList: TListBox;
    ButtonDisableTakeOffAdd: TButton;
    ButtonDisableTakeOffDel: TButton;
    ButtonDisableTakeOffAddAll: TButton;
    ButtonDisableTakeOffDelAll: TButton;
    ButtonDisableTakeOffSave: TButton;
    GroupBox11: TGroupBox;
    ListBoxitemList3: TListBox;
    ts10: TTabSheet;
    GroupBox13: TGroupBox;
    ListBoxNoClearMonList: TListBox;
    ButtonNoClearMonAdd: TButton;
    ButtonNoClearMonDel: TButton;
    ButtonNoClearMonAddAll: TButton;
    ButtonNoClearMonDelAll: TButton;
    ButtonNoClearMonSave: TButton;
    GroupBox14: TGroupBox;
    ListBoxMonList: TListBox;
    ts11: TTabSheet;
    GroupBox12: TGroupBox;
    ListBoxAdminList: TListBox;
    GroupBox15: TGroupBox;
    Label4: TLabel;
    Label5: TLabel;
    LabelAdminIPaddr: TLabel;
    EditAdminName: TEdit;
    EditAdminPremission: TSpinEditEx;
    ButtonAdminListAdd: TButton;
    ButtonAdminListChange: TButton;
    ButtonAdminListDel: TButton;
    EditAdminIPaddr: TEdit;
    ButtonAdminLitsSave: TButton;
    ts12: TTabSheet;
    Label22: TLabel;
    GroupBox20: TGroupBox;
    ListBoxEnablePickUpList: TListBox;
    GroupBox21: TGroupBox;
    ListBoxitemList5: TListBox;
    ButtonEnablePickUpAdd: TButton;
    ButtonEnablePickUpDelete: TButton;
    ButtonEnablePickUpAddAll: TButton;
    ButtonEnablePickUpDeleteAll: TButton;
    ButtonEnablePickUpSave: TButton;
    ts13: TTabSheet;
    Label23: TLabel;
    GroupBox22: TGroupBox;
    ListBoxPriorityPickUpList: TListBox;
    GroupBox23: TGroupBox;
    ListBoxitemList6: TListBox;
    ButtonPriorityPickUpAdd: TButton;
    ButtonPriorityPickUpDelete: TButton;
    ButtonPriorityPickUpAddAll: TButton;
    ButtonPriorityPickUpDeleteAll: TButton;
    ButtonPriorityPickUpSave: TButton;
    ts14: TTabSheet;
    lbl1: TLabel;
    lbl2: TLabel;
    Label24: TLabel;
    mmoNameFilterList: TMemo;
    btnNameFilterSave: TButton;
    mmoInputBoxFilterList: TMemo;
    ts15: TTabSheet;
    GroupBox26: TGroupBox;
    lstDisableShowItemFrom: TListBox;
    btnDisableItemFromAdd: TButton;
    btnDisableItemFromDel: TButton;
    btnDisableItemFromAddAll: TButton;
    btnDisableItemFromDelAll: TButton;
    btnDisableItemFromSave: TButton;
    GroupBox27: TGroupBox;
    ListBoxitemList4: TListBox;
    ts16: TTabSheet;
    GroupBox28: TGroupBox;
    mmoVerifyCodeChrs: TMemo;
    grp2: TGroupBox;
    lbl3: TLabel;
    Label19: TLabel;
    Label20: TLabel;
    Label21: TLabel;
    seVerifyCodeLen: TSpinEditEx;
    seVerifyCodeTimeOut: TSpinEditEx;
    seVerifyCodeRefreshCount: TSpinEditEx;
    seVerifyCodeFailCount: TSpinEditEx;
    btnVerifyCodeOK: TButton;
    ts17: TTabSheet;
    Label937: TLabel;
    Label938: TLabel;
    grp3: TGroupBox;
    lvPreviewItemMon: TListView;
    btnPreviewItemMonAdd: TButton;
    btnPreviewItemMonDel: TButton;
    btnPreviewItemMonAddAll: TButton;
    btnPreviewItemMonDelAll: TButton;
    btnPreviewItemMonSave: TButton;
    grp4: TGroupBox;
    lbl4: TLabel;
    lbl5: TLabel;
    lstPreviewItemMonAll: TListBox;
    sePreviewItemMonRefreshTime: TSpinEditEx;
    sePreviewMonItemShowTime: TSpinEditEx;
    ts18: TTabSheet;
    GroupBox29: TGroupBox;
    lstDisableRangePickItem: TListBox;
    btnDelDisableRangePickItem: TButton;
    btnAddAllDisableRangePickItem: TButton;
    btnDelAllDisableRangePickItem: TButton;
    btnSaveDisableRangePickItem: TButton;
    GroupBox30: TGroupBox;
    lstDisableRangePickItems: TListBox;
    btnAddDisableRangePickItem: TButton;
    ts19: TTabSheet;
    GroupBox31: TGroupBox;
    lstDisableDropToBagItem: TListBox;
    btnDelDisableDropToBagItem: TButton;
    btnAddAllDisableDropToBagItem: TButton;
    btnDelAllDisableDropToBagItem: TButton;
    btnSaveDisableDropToBagItem: TButton;
    GroupBox32: TGroupBox;
    lstDisableDropToBagItems: TListBox;
    btnAddDisableDropToBagItem: TButton;
    pnlTitle: TPanel;
    procedure FormCreate(Sender: TObject);
    procedure ListBoxItemListClick(Sender: TObject);
    procedure ListBoxEnableMakeListClick(Sender: TObject);
    procedure btnAddEnableMakeItemClick(Sender: TObject);
    procedure btnDelEnableMakeItemClick(Sender: TObject);
    procedure btnSaveEnableMakeItemClick(Sender: TObject);
    procedure btnAddAllDisableMakeItemClick(Sender: TObject);
    procedure btnDelAllDisableMakeItemClick(Sender: TObject);
    procedure btnAddAllEnableMakeItemClick(Sender: TObject);
    procedure btnDelAllEnableMakeItemClick(Sender: TObject);
    procedure ListBoxitemList1Click(Sender: TObject);
    procedure ListBoxDisableMakeListClick(Sender: TObject);
    procedure btnAddDisableMakeItemClick(Sender: TObject);
    procedure btnDelDisableMakeItemClick(Sender: TObject);
    procedure btnSaveDisableMakeItemClick(Sender: TObject);
    procedure btnAddDisabelMoveMapClick(Sender: TObject);
    procedure btnDelDisabelMoveMapClick(Sender: TObject);
    procedure btnAddAllDisabelMoveMapClick(Sender: TObject);
    procedure btnSaveDisabelMoveMapClick(Sender: TObject);
    procedure btnDelAllDisabelMoveMapClick(Sender: TObject);
    procedure ListBoxMapListClick(Sender: TObject);
    procedure ListBoxDisableMoveMapClick(Sender: TObject);
    procedure ButtonMonDropLimitRefClick(Sender: TObject);
    procedure StringGridMonDropLimitClick(Sender: TObject);
    procedure ButtonMonDropLimitSaveClick(Sender: TObject);
    procedure ListBoxGameLogListClick(Sender: TObject);
    procedure ListBoxitemList2Click(Sender: TObject);
    procedure btnAddLogItemClick(Sender: TObject);
    procedure btnDelLogItemClick(Sender: TObject);
    procedure btnAddAllLogItemClick(Sender: TObject);
    procedure btnDelAllLogItemClick(Sender: TObject);
    procedure btnSaveLogItemClick(Sender: TObject);
    procedure ButtonDisableTakeOffAddClick(Sender: TObject);
    procedure ButtonDisableTakeOffDelClick(Sender: TObject);
    procedure ListBoxDisableTakeOffListClick(Sender: TObject);
    procedure ListBoxitemList3Click(Sender: TObject);
    procedure ButtonDisableTakeOffAddAllClick(Sender: TObject);
    procedure ButtonDisableTakeOffDelAllClick(Sender: TObject);
    procedure ButtonDisableTakeOffSaveClick(Sender: TObject);
    procedure ButtonNoClearMonAddClick(Sender: TObject);
    procedure ButtonNoClearMonDelClick(Sender: TObject);
    procedure ButtonNoClearMonAddAllClick(Sender: TObject);
    procedure ButtonNoClearMonDelAllClick(Sender: TObject);
    procedure ButtonNoClearMonSaveClick(Sender: TObject);
    procedure ListBoxNoClearMonListClick(Sender: TObject);
    procedure ListBoxMonListClick(Sender: TObject);
    procedure ButtonAdminLitsSaveClick(Sender: TObject);
    procedure ListBoxAdminListClick(Sender: TObject);
    procedure ButtonAdminListChangeClick(Sender: TObject);
    procedure ButtonAdminListAddClick(Sender: TObject);
    procedure ButtonAdminListDelClick(Sender: TObject);
    procedure ButtonMonDropLimitAddClick(Sender: TObject);
    procedure ButtonMonDropLimitDelClick(Sender: TObject);
    procedure GridItemBindAccountClick(Sender: TObject);
    procedure EditItemBindAccountItemIdxChange(Sender: TObject);
    procedure EditItemBindAccountItemMakeIdxChange(Sender: TObject);
    procedure ButtonItemBindAcountModClick(Sender: TObject);
    procedure EditItemBindAccountNameChange(Sender: TObject);
    procedure ButtonItemBindAcountRefClick(Sender: TObject);
    procedure ButtonItemBindAcountAddClick(Sender: TObject);
    procedure ButtonItemBindAcountDelClick(Sender: TObject);
    procedure GridItemBindCharNameClick(Sender: TObject);
    procedure EditItemBindCharNameItemIdxChange(Sender: TObject);
    procedure EditItemBindCharNameItemMakeIdxChange(Sender: TObject);
    procedure EditItemBindCharNameNameChange(Sender: TObject);
    procedure ButtonItemBindCharNameAddClick(Sender: TObject);
    procedure ButtonItemBindCharNameModClick(Sender: TObject);
    procedure ButtonItemBindCharNameDelClick(Sender: TObject);
    procedure ButtonItemBindCharNameRefClick(Sender: TObject);
    procedure GridItemBindIPaddrClick(Sender: TObject);
    procedure EditItemBindIPaddrItemIdxChange(Sender: TObject);
    procedure EditItemBindIPaddrItemMakeIdxChange(Sender: TObject);
    procedure EditItemBindIPaddrNameChange(Sender: TObject);
    procedure ButtonItemBindIPaddrAddClick(Sender: TObject);
    procedure ButtonItemBindIPaddrModClick(Sender: TObject);
    procedure ButtonItemBindIPaddrDelClick(Sender: TObject);
    procedure ButtonItemBindIPaddrRefClick(Sender: TObject);
    procedure ListBoxItemListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure ListBoxEnablePickUpListClick(Sender: TObject);
    procedure ListBoxitemList5Click(Sender: TObject);
    procedure ButtonEnablePickUpAddClick(Sender: TObject);
    procedure ButtonEnablePickUpDeleteClick(Sender: TObject);
    procedure ButtonEnablePickUpAddAllClick(Sender: TObject);
    procedure ButtonEnablePickUpDeleteAllClick(Sender: TObject);
    procedure ButtonEnablePickUpSaveClick(Sender: TObject);
    procedure ButtonPriorityPickUpAddClick(Sender: TObject);
    procedure ButtonPriorityPickUpDeleteClick(Sender: TObject);
    procedure ButtonPriorityPickUpAddAllClick(Sender: TObject);
    procedure ButtonPriorityPickUpDeleteAllClick(Sender: TObject);
    procedure ButtonPriorityPickUpSaveClick(Sender: TObject);
    procedure ListBoxPriorityPickUpListClick(Sender: TObject);
    procedure ListBoxitemList6Click(Sender: TObject);
    procedure btnNameFilterSaveClick(Sender: TObject);
    procedure mmoNameFilterListChange(Sender: TObject);
    procedure btnMoveGuardPickItemAddClick(Sender: TObject);
    procedure btnMoveGuardPickItemDelClick(Sender: TObject);
    procedure btnMoveGuardPickItemAddAllClick(Sender: TObject);
    procedure btnMoveGuardPickItemDelAllClick(Sender: TObject);
    procedure btnMoveGuardPickItemSaveClick(Sender: TObject);
    procedure lstMoveGuardPickItemListClick(Sender: TObject);
    procedure chkMoveSuperGuardPickItemClick(Sender: TObject);
    procedure chkMoveSuperGuardAttackMonClick(Sender: TObject);
    procedure chkMoveSuperGuardAttackBBClick(Sender: TObject);
    procedure chkMoveArcherGuardPickItemClick(Sender: TObject);
    procedure btnDisableItemFromAddClick(Sender: TObject);
    procedure btnDisableItemFromDelClick(Sender: TObject);
    procedure btnDisableItemFromAddAllClick(Sender: TObject);
    procedure btnDisableItemFromDelAllClick(Sender: TObject);
    procedure btnDisableItemFromSaveClick(Sender: TObject);
    procedure ListBoxitemList4Click(Sender: TObject);
    procedure seVerifyCodeLenChange(Sender: TObject);
    procedure btnVerifyCodeOKClick(Sender: TObject);
    procedure btnPreviewItemMonAddClick(Sender: TObject);
    procedure btnPreviewItemMonDelClick(Sender: TObject);
    procedure btnPreviewItemMonAddAllClick(Sender: TObject);
    procedure btnPreviewItemMonDelAllClick(Sender: TObject);
    procedure btnPreviewItemMonSaveClick(Sender: TObject);
    procedure sePreviewMonItemShowTimeChange(Sender: TObject);
    procedure lvPreviewItemMonSelectItem(Sender: TObject; Item: TListItem; Selected: Boolean);
    procedure lstDisableShowItemFromClick(Sender: TObject);
    procedure btnAddDisableRangePickItemClick(Sender: TObject);
    procedure btnDelDisableRangePickItemClick(Sender: TObject);
    procedure btnAddAllDisableRangePickItemClick(Sender: TObject);
    procedure btnDelAllDisableRangePickItemClick(Sender: TObject);
    procedure btnSaveDisableRangePickItemClick(Sender: TObject);
    procedure lstDisableRangePickItemClick(Sender: TObject);
    procedure lstDisableRangePickItemsClick(Sender: TObject);
    procedure lstDisableDropToBagItemClick(Sender: TObject);
    procedure btnAddDisableDropToBagItemClick(Sender: TObject);
    procedure btnDelDisableDropToBagItemClick(Sender: TObject);
    procedure btnAddAllDisableDropToBagItemClick(Sender: TObject);
    procedure btnDelAllDisableDropToBagItemClick(Sender: TObject);
    procedure btnSaveDisableDropToBagItemClick(Sender: TObject);
    procedure lstDisableDropToBagItemsClick(Sender: TObject);
    procedure tvPageChange(Sender: TObject; Node: TTreeNode);
  private
    boOpened: Boolean;
    boModValued: Boolean;

    procedure ModValue();
    procedure uModValue();
    procedure RefMonDropLimit();
    procedure RefAdminList;
    procedure RefNoClearMonList();
    procedure RefItemBindAccount();
    procedure RefItemBindCharName();
    procedure RefItemBindIPaddr();
    procedure RefMsgFilterList();
    procedure RefNameFilterList();
    { Private declarations }
  public
    procedure Open();
    { Public declarations }
  end;

var
  frmViewList: TfrmViewList;

implementation

uses
  M2Share, UsrEngn, Envir, HUtil32, LocalDB;

{$R *.dfm}

{ TfrmViewList }

procedure TfrmViewList.ModValue;
begin
  boModValued := True;
  btnSaveEnableMakeItem.Enabled := True;
  btnSaveDisableMakeItem.Enabled := True;
  btnSaveDisabelMoveMap.Enabled := True;
  btnSaveLogItem.Enabled := True;
  ButtonDisableTakeOffSave.Enabled := True;
  ButtonNoClearMonSave.Enabled := True;
  ButtonEnablePickUpSave.Enabled := True;
  ButtonPriorityPickUpSave.Enabled := True;
  btnNameFilterSave.Enabled := True;
  btnSaveDisableRangePickItem.Enabled := True;
  btnSaveDisableDropToBagItem.Enabled := True;
end;

procedure TfrmViewList.uModValue;
begin
  boModValued := False;
  btnSaveEnableMakeItem.Enabled := False;
  btnSaveDisableMakeItem.Enabled := False;
  btnSaveDisabelMoveMap.Enabled := False;
  btnSaveLogItem.Enabled := False;
  ButtonDisableTakeOffSave.Enabled := False;
  ButtonNoClearMonSave.Enabled := False;
  ButtonEnablePickUpSave.Enabled := False;
  ButtonPriorityPickUpSave.Enabled := False;
  btnNameFilterSave.Enabled := False;
  btnSaveDisableRangePickItem.Enabled := False;
  btnSaveDisableDropToBagItem.Enabled := False;
end;

procedure TfrmViewList.Open;
var
  I: Integer;
  StdItem: pTStdItem;
  Envir: TEnvirnoment;
  Item: PSortItem;
  Monster: pTMonInfo;
  ListItem: TListItem;
  dwRefreshTime: LongWord;
begin
  boOpened := False;
  uModValue();
  ListBoxMapList.Items.Clear;
  ListBoxitemList2.Items.Clear;
  ListBoxitemList3.Items.Clear;
  ListBoxitemList4.Items.Clear;
  ListBoxitemList5.Items.Clear;
  ListBoxitemList6.Items.Clear;
  ListBoxGameLogList.Items.Clear;
  ListBoxDisableTakeOffList.Items.Clear;
  lstDisableShowItemFrom.Items.Clear;
  ListBoxDisableMoveMap.Items.Clear;
  ListBoxPriorityPickUpList.Items.Clear;
  lstMoveGuardAllItem.Items.Clear;

  ListBoxDisableMakeList.Items.Clear;
  ListBoxEnableMakeList.Items.Clear;
  ListBoxItemList.Items.Clear;
  ListBoxitemList1.Items.Clear;
  ListBoxEnablePickUpList.Items.Clear;

  lstDisableRangePickItem.Items.Clear;
  lstDisableDropToBagItem.Items.Clear;
  lstDisableRangePickItems.Items.Clear;
  lstDisableDropToBagItems.Items.Clear;

  ListBoxitemList2.Items.AddObject(g_sHumanDieEvent, TObject(nil));
  ListBoxitemList2.Items.AddObject(sSTRING_GOLDNAME, TObject(nil));
  ListBoxitemList2.Items.AddObject(g_Config.sGameGoldName, TObject(nil));
  ListBoxitemList2.Items.AddObject(g_Config.sGamePointName, TObject(nil));
  ListBoxitemList5.Items.AddObject(g_Config.sGameGoldName, TObject(nil));
  ListBoxitemList6.Items.AddObject(g_Config.sGameGoldName, TObject(nil));

  if g_MultiThreadRun then
    UserEngine.StdItemList.LockR(12);
  try
    for I := 0 to UserEngine.StdItemList.Count - 1 do
    begin
      StdItem := UserEngine.StdItemList.Items[I];
      ListBoxItemList.Items.AddObject(StdItem.Name, TObject(StdItem));
      ListBoxitemList1.Items.AddObject(StdItem.Name, TObject(StdItem));
      ListBoxitemList2.Items.AddObject(StdItem.Name, TObject(StdItem));
      ListBoxitemList5.Items.AddObject(StdItem.Name, TObject(StdItem));
      ListBoxitemList6.Items.AddObject(StdItem.Name, TObject(StdItem));
      ListBoxitemList3.Items.AddObject(StdItem.Name, TObject(I));
      ListBoxitemList4.Items.AddObject(StdItem.Name, TObject(I));
      lstDisableRangePickItems.Items.AddObject(StdItem.Name, TObject(I));
      lstDisableDropToBagItems.Items.AddObject(StdItem.Name, TObject(I));

      lstMoveGuardAllItem.Items.AddObject(StdItem.Name, TObject(I));
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.StdItemList.UnLockR;
  end;

  for I := 0 to g_MapManager.Count - 1 do
  begin
    Envir := TEnvirnoment(g_MapManager.Items[I]);
    ListBoxMapList.Items.Add(Envir.sMapName);
  end;

  g_EnableMakeItemList.Lock;
  try
    for I := 0 to g_EnableMakeItemList.Count - 1 do
    begin
      ListBoxEnableMakeList.Items.Add(g_EnableMakeItemList.Strings[I]);
    end;
  finally
    g_EnableMakeItemList.UnLock;
  end;

  g_DisableMakeItemList.Lock;
  try
    for I := 0 to g_DisableMakeItemList.Count - 1 do
    begin
      ListBoxDisableMakeList.Items.Add(g_DisableMakeItemList.Strings[I]);
    end;
  finally
    g_DisableMakeItemList.UnLock;
  end;

  g_GameLogItemNameList.Lock;
  try
    for I := 0 to g_GameLogItemNameList.Count - 1 do
    begin
      ListBoxGameLogList.Items.Add(g_GameLogItemNameList.Strings[I]);
    end;
  finally
    g_GameLogItemNameList.UnLock;
  end;

  g_DisableTakeOffList.Lock;
  try
    for I := 0 to g_DisableTakeOffList.Count - 1 do
    begin
      Item := g_DisableTakeOffList.Items[I];

      ListBoxDisableTakeOffList.Items.AddObject(IntToStr(Item.ItemIdx) + '  ' + Item.ItemName, TObject(Item.ItemIdx));
    end;
  finally
    g_DisableTakeOffList.UnLock;
  end;

  g_DisableShowItemFromList.Lock;
  try
    for I := 0 to g_DisableShowItemFromList.Count - 1 do
    begin
      Item := g_DisableShowItemFromList.Items[I];

      lstDisableShowItemFrom.Items.AddObject(IntToStr(Item.ItemIdx) + '  ' + Item.ItemName, TObject(Item.ItemIdx));
    end;
  finally
    g_DisableShowItemFromList.UnLock;
  end;

  g_DisableMoveMapList.Lock;
  try
    for I := 0 to g_DisableMoveMapList.Count - 1 do
    begin
      ListBoxDisableMoveMap.Items.Add(g_DisableMoveMapList.Strings[I]);
    end;
  finally
    g_DisableMoveMapList.UnLock;
  end;

  g_EnablePickUpItemList.Lock;
  try
    for I := 0 to g_EnablePickUpItemList.Count - 1 do
    begin
      ListBoxEnablePickUpList.Items.Add(g_EnablePickUpItemList.Strings[I]);
    end;
  finally
    g_EnablePickUpItemList.UnLock;
  end;

  g_MoveGuardPickItemList.Lock;
  try
    for I := 0 to g_MoveGuardPickItemList.Count - 1 do
    begin
      lstMoveGuardPickItemList.Items.Add(g_MoveGuardPickItemList.Strings[I]);
    end;
  finally
    g_MoveGuardPickItemList.UnLock;
  end;

  g_PriorityPickUpItemList.Lock;
  try
    for I := 0 to g_PriorityPickUpItemList.Count - 1 do
    begin
      ListBoxPriorityPickUpList.Items.Add(g_PriorityPickUpItemList.Strings[I]);
    end;
  finally
    g_PriorityPickUpItemList.UnLock;
  end;

  g_DisableRangePickItemList.Lock;
  try
    for I := 0 to g_DisableRangePickItemList.Count - 1 do
    begin
      lstDisableRangePickItem.Items.Add(g_DisableRangePickItemList.Strings[I]);
    end;
  finally
    g_DisableRangePickItemList.UnLock;
  end;

  g_DisableDropToBagItemList.Lock;
  try
    for I := 0 to g_DisableDropToBagItemList.Count - 1 do
    begin
      lstDisableDropToBagItem.Items.Add(g_DisableDropToBagItemList.Strings[I]);
    end;
  finally
    g_DisableDropToBagItemList.UnLock;
  end;

  lstPreviewItemMonAll.Clear;
  for I := 0 to UserEngine.MonsterList.Count - 1 do
  begin
    Monster := UserEngine.MonsterList.Items[I];
    lstPreviewItemMonAll.Items.Add(Monster.sName);
  end;

  lvPreviewItemMon.Clear;
  g_PreviewItemMonList.Lock;
  try
    for I := 0 to g_PreviewItemMonList.Count - 1 do
    begin
      ListItem := lvPreviewItemMon.Items.Add;
      ListItem.Caption := g_PreviewItemMonList.Strings[I];

      dwRefreshTime := DWORD(g_PreviewItemMonList.Objects[I]);

      ListItem.SubItems.Add(IntToStr(dwRefreshTime));
    end;
  finally
    g_PreviewItemMonList.UnLock;
  end;
  btnPreviewItemMonDel.Enabled := False;
  btnPreviewItemMonSave.Enabled := False;
  sePreviewMonItemShowTime.Value := g_Config.nPreviewMonItemShowTime;

  RefItemBindAccount();

  RefItemBindCharName();

  RefItemBindIPaddr();

  RefMonDropLimit();
  RefAdminList();
  RefNoClearMonList();
  RefMsgFilterList();

  RefNameFilterList();

  boOpened := True;
  pgcViewList.ActivePageIndex := 0;
  pnlTitle.Caption := pgcViewList.ActivePage.Caption;

  for I := 0 to tvPage.Items.Count - 1 do
  begin
    if tvPage.Items[I].HasChildren then
      tvPage.Items[I].Expanded := True;
  end;

  chkMoveSuperGuardPickItem.Checked := g_Config.boMoveSuperGuardPickItem;
  chkMoveSuperGuardAttackMon.Checked := g_Config.boMoveSuperGuardAttackMon;
  chkMoveSuperGuardAttackBB.Checked := g_Config.boMoveSuperGuardAttackBB;
  chkMoveArcherGuardPickItem.Checked := g_Config.boMoveArcherGuardPickItem;

  mmoVerifyCodeChrs.Text := g_sVerifyCodeChrs;
  seVerifyCodeLen.Value := g_Config.nVerifyCodeLen;
  seVerifyCodeTimeOut.Value := g_Config.nVerifyCodeTimeOut;
  seVerifyCodeRefreshCount.Value := g_Config.nVerifyCodeRefreshCount;
  seVerifyCodeFailCount.Value := g_Config.nVerifyCodeFailCount;
  btnVerifyCodeOK.Enabled := False;

  ShowModal;
end;

procedure TfrmViewList.FormCreate(Sender: TObject);
begin
  GridItemBindAccount.Cells[0, 0] := '物品名称';
  GridItemBindAccount.Cells[1, 0] := '物品IDX';
  GridItemBindAccount.Cells[2, 0] := '物品系列号';
  GridItemBindAccount.Cells[3, 0] := '绑定帐号';

  GridItemBindCharName.Cells[0, 0] := '物品名称';
  GridItemBindCharName.Cells[1, 0] := '物品IDX';
  GridItemBindCharName.Cells[2, 0] := '物品系列号';
  GridItemBindCharName.Cells[3, 0] := '绑定人物';

  GridItemBindIPaddr.Cells[0, 0] := '物品名称';
  GridItemBindIPaddr.Cells[1, 0] := '物品IDX';
  GridItemBindIPaddr.Cells[2, 0] := '物品系列号';
  GridItemBindIPaddr.Cells[3, 0] := '绑定IP';

  StringGridMonDropLimit.Cells[0, 0] := '物品名称';
  StringGridMonDropLimit.Cells[1, 0] := '爆数量';
  StringGridMonDropLimit.Cells[2, 0] := '限制数量';
  StringGridMonDropLimit.Cells[3, 0] := '未爆数量';
  StringGridMonDropLimit.Cells[4, 0] := '清零时间';

  btnAddEnableMakeItem.Enabled := False;
  btnDelEnableMakeItem.Enabled := False;
  btnAddDisableMakeItem.Enabled := False;
  btnDelDisableMakeItem.Enabled := False;
  btnAddDisabelMoveMap.Enabled := False;
  btnDelDisabelMoveMap.Enabled := False;
  btnAddLogItem.Enabled := False;
  btnDelLogItem.Enabled := False;

  ButtonNoClearMonAdd.Enabled := False;
  ButtonDisableTakeOffDel.Enabled := False;

  ButtonDisableTakeOffAdd.Enabled := False;
  ButtonNoClearMonDel.Enabled := False;

  EditAdminIPaddr.Visible := True;
  LabelAdminIPaddr.Visible := True;
end;

procedure TfrmViewList.ListBoxItemListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxItemList.ItemIndex >= 0 then
    btnAddEnableMakeItem.Enabled := True;
end;

procedure TfrmViewList.ListBoxEnableMakeListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxEnableMakeList.ItemIndex >= 0 then
    btnDelEnableMakeItem.Enabled := True;
end;

procedure TfrmViewList.btnAddEnableMakeItemClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if ListBoxItemList.Items.Count >= 0 then
  begin
    for I := 0 to ListBoxItemList.Items.Count - 1 do
    begin
      if not ListBoxItemList.Selected[I] then
        Continue;

      sItemName := ListBoxItemList.Items[I];
      if ListBoxEnableMakeList.Items.IndexOf(sItemName) < 0 then
      begin
        ListBoxEnableMakeList.Items.Add(sItemName);
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.btnAddAllEnableMakeItemClick(Sender: TObject);
var
  I: Integer;
begin
  ListBoxEnableMakeList.Items.Clear;
  for I := 0 to ListBoxItemList.Items.Count - 1 do
  begin
    ListBoxEnableMakeList.Items.Add(ListBoxItemList.Items.Strings[I]);
  end;
  ModValue();
end;

procedure TfrmViewList.btnDelAllEnableMakeItemClick(Sender: TObject);
begin
  ListBoxEnableMakeList.Items.Clear;
  btnDelEnableMakeItem.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.btnDelEnableMakeItemClick(Sender: TObject);
begin
  if ListBoxEnableMakeList.ItemIndex >= 0 then
  begin
    ListBoxEnableMakeList.Items.Delete(ListBoxEnableMakeList.ItemIndex);
    ModValue();
  end;
  if ListBoxEnableMakeList.ItemIndex < 0 then
    btnDelEnableMakeItem.Enabled := False;
end;

procedure TfrmViewList.btnSaveEnableMakeItemClick(Sender: TObject);
var
  I: Integer;
begin
  g_EnableMakeItemList.Lock;
  try
    g_EnableMakeItemList.Clear;
    for I := 0 to ListBoxEnableMakeList.Items.Count - 1 do
    begin
      g_EnableMakeItemList.Add(ListBoxEnableMakeList.Items.Strings[I])
    end;

    g_EnableMakeItemList.Sorted := True;
  finally
    g_EnableMakeItemList.UnLock;
  end;
  SaveEnableMakeItem();
  uModValue();
end;

procedure TfrmViewList.ListBoxitemList1Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxitemList1.ItemIndex >= 0 then
    btnAddDisableMakeItem.Enabled := True;
end;

procedure TfrmViewList.ListBoxDisableMakeListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxDisableMakeList.ItemIndex >= 0 then
    btnDelDisableMakeItem.Enabled := True;
end;

procedure TfrmViewList.btnAddDisableMakeItemClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if ListBoxitemList1.Items.Count >= 0 then
  begin
    for I := 0 to ListBoxitemList1.Items.Count - 1 do
    begin
      if not ListBoxitemList1.Selected[I] then
        Continue;

      sItemName := ListBoxitemList1.Items[I];
      if ListBoxDisableMakeList.Items.IndexOf(sItemName) < 0 then
      begin
        ListBoxDisableMakeList.Items.Add(sItemName);
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.btnDelDisableMakeItemClick(Sender: TObject);
begin
  if ListBoxDisableMakeList.ItemIndex >= 0 then
  begin
    ListBoxDisableMakeList.Items.Delete(ListBoxDisableMakeList.ItemIndex);
    ModValue();
  end;
  if ListBoxDisableMakeList.ItemIndex < 0 then
    btnDelDisableMakeItem.Enabled := False;
end;

procedure TfrmViewList.btnAddAllDisableMakeItemClick(Sender: TObject);
var
  I: Integer;
begin
  ListBoxDisableMakeList.Items.Clear;
  for I := 0 to ListBoxitemList1.Items.Count - 1 do
  begin
    ListBoxDisableMakeList.Items.Add(ListBoxitemList1.Items.Strings[I]);
  end;
  ModValue();
end;

procedure TfrmViewList.btnDelAllDisableMakeItemClick(Sender: TObject);
begin
  ListBoxDisableMakeList.Items.Clear;
  btnDelDisableMakeItem.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.btnSaveDisableMakeItemClick(Sender: TObject);
var
  I: Integer;
begin
  g_DisableMakeItemList.Lock;
  try
    g_DisableMakeItemList.Clear;
    for I := 0 to ListBoxDisableMakeList.Items.Count - 1 do
    begin
      g_DisableMakeItemList.Add(ListBoxDisableMakeList.Items.Strings[I])
    end;

    g_DisableMakeItemList.Sorted := True;
  finally
    g_DisableMakeItemList.UnLock;
  end;
  SaveDisableMakeItem();
  uModValue();
end;

procedure TfrmViewList.btnAddDisabelMoveMapClick(Sender: TObject);
var
  I: Integer;
  sMapName: string;
begin
  if ListBoxMapList.Items.Count >= 0 then
  begin
    for I := 0 to ListBoxMapList.Items.Count - 1 do
    begin
      if not ListBoxMapList.Selected[I] then
        Continue;

      sMapName := ListBoxMapList.Items[I];
      if ListBoxDisableMoveMap.Items.IndexOf(sMapName) < 0 then
      begin
        ListBoxDisableMoveMap.Items.Add(sMapName);
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.btnDelDisabelMoveMapClick(Sender: TObject);
begin
  if ListBoxDisableMoveMap.ItemIndex >= 0 then
  begin
    ListBoxDisableMoveMap.Items.Delete(ListBoxDisableMoveMap.ItemIndex);
    ModValue();
  end;
  if ListBoxDisableMoveMap.ItemIndex < 0 then
    btnDelDisabelMoveMap.Enabled := False;
end;

procedure TfrmViewList.btnAddAllDisabelMoveMapClick(Sender: TObject);
var
  I: Integer;
begin
  ListBoxDisableMoveMap.Items.Clear;
  for I := 0 to ListBoxMapList.Items.Count - 1 do
  begin
    ListBoxDisableMoveMap.Items.Add(ListBoxMapList.Items.Strings[I]);
  end;
  ModValue();
end;

procedure TfrmViewList.btnSaveDisabelMoveMapClick(Sender: TObject);
var
  I: Integer;
begin
  g_DisableMoveMapList.Lock;
  try
    g_DisableMoveMapList.Clear;
    for I := 0 to ListBoxDisableMoveMap.Items.Count - 1 do
    begin
      g_DisableMoveMapList.Add(ListBoxDisableMoveMap.Items.Strings[I])
    end;

    g_DisableMoveMapList.Sorted := True;
  finally
    g_DisableMoveMapList.UnLock;
  end;
  SaveDisableMoveMap();
  uModValue();
end;

procedure TfrmViewList.btnDelAllDisabelMoveMapClick(Sender: TObject);
begin
  ListBoxDisableMoveMap.Items.Clear;
  btnDelDisabelMoveMap.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.ListBoxMapListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxMapList.ItemIndex >= 0 then
    btnAddDisabelMoveMap.Enabled := True;
end;

procedure TfrmViewList.ListBoxDisableMoveMapClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxDisableMoveMap.ItemIndex >= 0 then
    btnDelDisabelMoveMap.Enabled := True;
end;

procedure TfrmViewList.RefMsgFilterList;
begin

end;

procedure TfrmViewList.RefMonDropLimit;
var
  I: Integer;
  MonDrop: pTMonDrop;
begin
  g_MonDropLimitList.Lock;
  try
    StringGridMonDropLimit.RowCount := g_MonDropLimitList.Count + 1;
    if StringGridMonDropLimit.RowCount > 1 then
      StringGridMonDropLimit.FixedRows := 1;

    for I := 0 to g_MonDropLimitList.Count - 1 do
    begin
      MonDrop := pTMonDrop(g_MonDropLimitList.Objects[I]);
      StringGridMonDropLimit.Cells[0, I + 1] := MonDrop.sItemName;
      StringGridMonDropLimit.Cells[1, I + 1] := IntToStr(MonDrop.nDropCount);
      StringGridMonDropLimit.Cells[2, I + 1] := IntToStr(MonDrop.nCountLimit);
      StringGridMonDropLimit.Cells[3, I + 1] := IntToStr(MonDrop.nNoDropCount);
      if MonDrop.ClearDay = 0 then
        StringGridMonDropLimit.Cells[4, I + 1] := '-'
      else
      begin
        StringGridMonDropLimit.Cells[4, I + 1] := FormatDateTime('yyyy/mm/dd hh:nn:ss', MonDrop.LastClearDate + MonDrop.ClearDay +
          (g_StartRunTime - Trunc(g_StartRunTime)));
      end;
    end;
  finally
    g_MonDropLimitList.UnLock;
  end;
end;

procedure TfrmViewList.ButtonMonDropLimitRefClick(Sender: TObject);
begin
  RefMonDropLimit();
end;

procedure TfrmViewList.StringGridMonDropLimitClick(Sender: TObject);
var
  nItemIndex: Integer;
  MonDrop: pTMonDrop;
begin
  nItemIndex := StringGridMonDropLimit.Row - 1;
  if nItemIndex < 0 then
    Exit;

  g_MonDropLimitList.Lock;
  try
    if nItemIndex >= g_MonDropLimitList.Count then
      Exit;
    MonDrop := pTMonDrop(g_MonDropLimitList.Objects[nItemIndex]);
    EditItemName.Text := MonDrop.sItemName;
    EditDropCount.Value := MonDrop.nDropCount;
    EditCountLimit.Value := MonDrop.nCountLimit;
    EditNoDropCount.Value := MonDrop.nNoDropCount;
    seClearDay.Value := MonDrop.ClearDay;
  finally
    g_MonDropLimitList.UnLock;
  end;
end;

procedure TfrmViewList.tvPageChange(Sender: TObject; Node: TTreeNode);
begin
  if Node.StateIndex >= 0 then
  begin
    pgcViewList.ActivePageIndex := Node.StateIndex;
    pnlTitle.Caption := pgcViewList.ActivePage.Caption;
  end;
end;

procedure TfrmViewList.ButtonMonDropLimitSaveClick(Sender: TObject);
var
  sItemName: string;
  nNoDropCount: Integer;
  nDropCount: Integer;
  nDropLimit: Integer;
  nSelIndex: Integer;
  MonDrop: pTMonDrop;
begin
  sItemName := Trim(EditItemName.Text);
  nDropCount := EditDropCount.Value;
  nDropLimit := EditCountLimit.Value;
  nNoDropCount := EditNoDropCount.Value;

  nSelIndex := StringGridMonDropLimit.Row - 1;
  if nSelIndex < 0 then
    Exit;
  g_MonDropLimitList.Lock;
  try
    if nSelIndex >= g_MonDropLimitList.Count then
      Exit;
    MonDrop := pTMonDrop(g_MonDropLimitList.Objects[nSelIndex]);
    MonDrop.sItemName := sItemName;
    MonDrop.nDropCount := nDropCount;
    MonDrop.nNoDropCount := nNoDropCount;
    MonDrop.nCountLimit := nDropLimit;

    if MonDrop.LastClearDate <= 0 then
    begin
      MonDrop.LastClearDate := Trunc(Now);
    end;

    MonDrop.ClearDay := seClearDay.Value;
  finally
    g_MonDropLimitList.UnLock;
  end;
  SaveMonDropLimitList();
  RefMonDropLimit();
end;

procedure TfrmViewList.ButtonMonDropLimitAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
  nNoDropCount: Integer;
  nDropCount: Integer;
  nDropLimit: Integer;
  MonDrop: pTMonDrop;
begin
  sItemName := Trim(EditItemName.Text);
  nDropCount := EditDropCount.Value;
  nDropLimit := EditCountLimit.Value;
  nNoDropCount := EditNoDropCount.Value;

  g_MonDropLimitList.Lock;
  try
    for I := 0 to g_MonDropLimitList.Count - 1 do
    begin
      MonDrop := pTMonDrop(g_MonDropLimitList.Objects[I]);
      if CompareText(MonDrop.sItemName, sItemName) = 0 then
      begin
        Application.MessageBox('输入的物品名已经在列表中！', '提示信息', MB_OK + MB_ICONERROR);
        Exit;
      end;
    end;
    New(MonDrop);
    MonDrop.sItemName := sItemName;
    MonDrop.nDropCount := nDropCount;
    MonDrop.nNoDropCount := nNoDropCount;
    MonDrop.nCountLimit := nDropLimit;
    MonDrop.LastClearDate := Trunc(Now());
    MonDrop.ClearDay := seClearDay.Value;

    g_MonDropLimitList.AddObject(sItemName, TObject(MonDrop));
  finally
    g_MonDropLimitList.UnLock;
  end;
  SaveMonDropLimitList();
  RefMonDropLimit();
end;

procedure TfrmViewList.ButtonMonDropLimitDelClick(Sender: TObject);
var
  nSelIndex: Integer;
  MonDrop: pTMonDrop;
begin

  nSelIndex := StringGridMonDropLimit.Row - 1;
  if nSelIndex < 0 then
    Exit;
  g_MonDropLimitList.Lock;
  try
    if nSelIndex >= g_MonDropLimitList.Count then
      Exit;
    MonDrop := pTMonDrop(g_MonDropLimitList.Objects[nSelIndex]);
    Dispose(MonDrop);
    g_MonDropLimitList.Delete(nSelIndex);
  finally
    g_MonDropLimitList.UnLock;
  end;
  SaveMonDropLimitList();
  RefMonDropLimit();
end;

procedure TfrmViewList.ListBoxGameLogListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxGameLogList.ItemIndex >= 0 then
    btnDelLogItem.Enabled := True;
end;

procedure TfrmViewList.ListBoxitemList2Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxitemList2.ItemIndex >= 0 then
    btnAddLogItem.Enabled := True;
end;

procedure TfrmViewList.btnAddLogItemClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if ListBoxitemList2.Items.Count >= 0 then
  begin
    for I := 0 to ListBoxitemList2.Items.Count - 1 do
    begin
      if not ListBoxitemList2.Selected[I] then
        Continue;

      sItemName := ListBoxitemList2.Items[I];
      if ListBoxGameLogList.Items.IndexOf(sItemName) < 0 then
      begin
        ListBoxGameLogList.Items.Add(sItemName);
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.btnDelLogItemClick(Sender: TObject);
begin
  if ListBoxGameLogList.ItemIndex >= 0 then
  begin
    ListBoxGameLogList.Items.Delete(ListBoxGameLogList.ItemIndex);
    ModValue();
  end;
  if ListBoxGameLogList.ItemIndex < 0 then
    btnDelLogItem.Enabled := False;
end;

procedure TfrmViewList.btnAddAllLogItemClick(Sender: TObject);
var
  I: Integer;
begin
  ListBoxGameLogList.Items.Clear;
  for I := 0 to ListBoxitemList2.Items.Count - 1 do
  begin
    ListBoxGameLogList.Items.Add(ListBoxitemList2.Items.Strings[I]);
  end;
  ModValue();
end;

procedure TfrmViewList.btnDelAllLogItemClick(Sender: TObject);
begin
  ListBoxGameLogList.Items.Clear;
  btnDelLogItem.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.btnSaveLogItemClick(Sender: TObject);
var
  I: Integer;
begin

  g_GameLogItemNameList.Lock;
  try
    g_GameLogItemNameList.Clear;
    for I := 0 to ListBoxGameLogList.Items.Count - 1 do
    begin
      g_GameLogItemNameList.Add(ListBoxGameLogList.Items.Strings[I]);
    end;

    g_GameLogItemNameList.Sorted := True;
  finally
    g_GameLogItemNameList.UnLock;
  end;
  uModValue();

  SaveGameLogItemNameList();

  if Application.MessageBox('此设置必须重新加载物品数据库才能生效，是否重新加载？', '确认信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
  begin
    FrmDB.LoadItemsDB();
  end;
end;

procedure TfrmViewList.ButtonDisableTakeOffAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if ListBoxitemList3.Items.Count >= 0 then
  begin
    for I := 0 to ListBoxitemList3.Items.Count - 1 do
    begin
      if not ListBoxitemList3.Selected[I] then
        Continue;

      sItemName := IntToStr(I) + '  ' + ListBoxitemList3.Items.Strings[I];
      if ListBoxDisableTakeOffList.Items.IndexOf(sItemName) < 0 then
      begin
        ListBoxDisableTakeOffList.Items.AddObject(sItemName, TObject(I));
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.ButtonDisableTakeOffDelClick(Sender: TObject);
begin
  if ListBoxDisableTakeOffList.ItemIndex >= 0 then
  begin
    ListBoxDisableTakeOffList.Items.Delete(ListBoxDisableTakeOffList.ItemIndex);
    ModValue();
  end;
  if ListBoxDisableTakeOffList.ItemIndex < 0 then
    ButtonDisableTakeOffDel.Enabled := False;
end;

procedure TfrmViewList.ListBoxDisableTakeOffListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxDisableTakeOffList.ItemIndex >= 0 then
    ButtonDisableTakeOffDel.Enabled := True;
end;

procedure TfrmViewList.ListBoxitemList3Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxitemList3.ItemIndex >= 0 then
    ButtonDisableTakeOffAdd.Enabled := True;
end;

procedure TfrmViewList.ButtonDisableTakeOffAddAllClick(Sender: TObject);
var
  I: Integer;
begin
  ListBoxDisableTakeOffList.Items.Clear;
  for I := 0 to ListBoxitemList3.Items.Count - 1 do
  begin
    ListBoxDisableTakeOffList.Items.AddObject(IntToStr(I) + '  ' + ListBoxitemList3.Items.Strings[I], TObject(I));
  end;
  ModValue();
end;

procedure TfrmViewList.ButtonDisableTakeOffDelAllClick(Sender: TObject);
begin
  ListBoxDisableTakeOffList.Items.Clear;
  ButtonDisableTakeOffDel.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.ButtonDisableTakeOffSaveClick(Sender: TObject);
var
  I: Integer;
  sItemIdx: string;
begin
  g_DisableTakeOffList.Lock;
  try
    g_DisableTakeOffList.Clear;
    for I := 0 to ListBoxDisableTakeOffList.Items.Count - 1 do
    begin
      g_DisableTakeOffList.Add(Integer(ListBoxDisableTakeOffList.Items.Objects[I]), Trim(GetValidStr3_Ex(ListBoxDisableTakeOffList.Items.Strings
        [I], sItemIdx, ' ')));
    end;

    g_DisableTakeOffList.Sort;
  finally
    g_DisableTakeOffList.UnLock;
  end;
  SaveDisableTakeOffList();
  uModValue();
end;

procedure TfrmViewList.RefAdminList();
var
  I: Integer;
  AdminInfo: pTAdminInfo;
begin
  ListBoxAdminList.Clear;
  EditAdminName.Text := '';
  EditAdminIPaddr.Text := '';
  EditAdminPremission.Value := 0;
  ButtonAdminListChange.Enabled := False;
  ButtonAdminListDel.Enabled := False;
  UserEngine.m_AdminList.LockR(4);
  try
    for I := 0 to UserEngine.m_AdminList.Count - 1 do
    begin
      AdminInfo := pTAdminInfo(UserEngine.m_AdminList.Items[I]);
      ListBoxAdminList.Items.Add(AdminInfo.sChrName + ' - ' + IntToStr(AdminInfo.nLv))
    end;
  finally
    UserEngine.m_AdminList.UnLockR;
  end;
end;

procedure TfrmViewList.RefNoClearMonList;
var
  MonInfo: pTMonInfo;
  I: Integer;
begin
  if g_MultiThreadRun then
    UserEngine.MonsterList.LockR(6);
  try
    for I := 0 to UserEngine.MonsterList.Count - 1 do
    begin
      MonInfo := UserEngine.MonsterList.Items[I];
      ListBoxMonList.Items.AddObject(MonInfo.sName, TObject(MonInfo));
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.MonsterList.UnLockR;
  end;

  g_NoClearMonList.Lock;
  try
    for I := 0 to g_NoClearMonList.Count - 1 do
    begin
      ListBoxNoClearMonList.Items.Add(g_NoClearMonList.Strings[I]);
    end;
  finally
    g_NoClearMonList.UnLock;
  end;
end;

procedure TfrmViewList.RefNameFilterList;
begin
  g_NameFilterList.Lock;
  try
    mmoNameFilterList.Text := g_NameFilterList.Text;
  finally
    g_NameFilterList.UnLock;
  end;

  g_InputBoxFilterList.Lock;
  try
    mmoInputBoxFilterList.Text := g_InputBoxFilterList.Text;
  finally
    g_InputBoxFilterList.UnLock;
  end;
end;

procedure TfrmViewList.ButtonNoClearMonAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if ListBoxMonList.Items.Count >= 0 then
  begin
    for I := 0 to ListBoxMonList.Items.Count - 1 do
    begin
      if not ListBoxMonList.Selected[I] then
        Continue;

      sItemName := ListBoxMonList.Items[I];
      if ListBoxNoClearMonList.Items.IndexOf(sItemName) < 0 then
      begin
        ListBoxNoClearMonList.Items.Add(sItemName);
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.ButtonNoClearMonDelClick(Sender: TObject);
begin
  if ListBoxNoClearMonList.ItemIndex >= 0 then
  begin
    ListBoxNoClearMonList.Items.Delete(ListBoxNoClearMonList.ItemIndex);
    ModValue();
  end;
  if ListBoxNoClearMonList.ItemIndex < 0 then
    ButtonNoClearMonDel.Enabled := False;
end;

procedure TfrmViewList.ButtonNoClearMonAddAllClick(Sender: TObject);
var
  I: Integer;
begin
  ListBoxNoClearMonList.Items.Clear;
  for I := 0 to ListBoxMonList.Items.Count - 1 do
  begin
    ListBoxNoClearMonList.Items.Add(ListBoxMonList.Items.Strings[I]);
  end;
  ModValue();
end;

procedure TfrmViewList.ButtonNoClearMonDelAllClick(Sender: TObject);
begin
  ListBoxNoClearMonList.Items.Clear;
  ButtonNoClearMonDel.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.ButtonNoClearMonSaveClick(Sender: TObject);
var
  I: Integer;
begin
  g_NoClearMonList.Lock;
  try
    g_NoClearMonList.Clear;
    for I := 0 to ListBoxNoClearMonList.Items.Count - 1 do
    begin
      g_NoClearMonList.Add(ListBoxNoClearMonList.Items.Strings[I]);
    end;

    g_NoClearMonList.Sorted := True;
  finally
    g_NoClearMonList.UnLock;
  end;
  SaveNoClearMonList();
  uModValue();
end;

procedure TfrmViewList.ListBoxNoClearMonListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxNoClearMonList.ItemIndex >= 0 then
    ButtonNoClearMonDel.Enabled := True;
end;

procedure TfrmViewList.ListBoxMonListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxMonList.ItemIndex >= 0 then
    ButtonNoClearMonAdd.Enabled := True;
end;

procedure TfrmViewList.ButtonAdminLitsSaveClick(Sender: TObject);
begin
  SaveAdminList();
  ButtonAdminLitsSave.Enabled := False;
end;

procedure TfrmViewList.ListBoxAdminListClick(Sender: TObject);
var
  nIndex: Integer;
  AdminInfo: pTAdminInfo;
begin
  nIndex := ListBoxAdminList.ItemIndex;
  UserEngine.m_AdminList.LockR(5);
  try
    if (nIndex < 0) and (nIndex >= UserEngine.m_AdminList.Count) then
      Exit;
    ButtonAdminListChange.Enabled := True;
    ButtonAdminListDel.Enabled := True;
    AdminInfo := UserEngine.m_AdminList.Items[nIndex];
    EditAdminName.Text := AdminInfo.sChrName;
    EditAdminIPaddr.Text := AdminInfo.sIPaddr;
    EditAdminPremission.Value := AdminInfo.nLv;
  finally
    UserEngine.m_AdminList.UnLockR;
  end;
end;

procedure TfrmViewList.ButtonAdminListAddClick(Sender: TObject);
var
  I: Integer;
  sAdminName: string;
  sAdminIPaddr: string;
  nAdminPerMission: Integer;
  AdminInfo: pTAdminInfo;
begin
  sAdminName := Trim(EditAdminName.Text);
  sAdminIPaddr := Trim(EditAdminIPaddr.Text);
  nAdminPerMission := EditAdminPremission.Value;
  if (nAdminPerMission < 1) or (sAdminName = '') or not (nAdminPerMission in [0..10]) then
  begin
    Application.MessageBox('输入不正确！', '提示信息', MB_OK + MB_ICONERROR);
    EditAdminName.SetFocus;
    Exit;
  end;

  UserEngine.m_AdminList.LockW(6);
  try
    for I := 0 to UserEngine.m_AdminList.Count - 1 do
    begin
      if CompareText(pTAdminInfo(UserEngine.m_AdminList.Items[I]).sChrName, sAdminName) = 0 then
      begin
        Application.MessageBox('输入的角色名已经在GM列表中！', '提示信息', MB_OK + MB_ICONERROR);
        Exit;
      end;
    end;

    New(AdminInfo);
    AdminInfo.nLv := nAdminPerMission;
    AdminInfo.sChrName := sAdminName;
    AdminInfo.sIPaddr := sAdminIPaddr;
    UserEngine.m_AdminList.Add(AdminInfo);
  finally
    UserEngine.m_AdminList.UnLockW;
  end;

  RefAdminList();
  ButtonAdminLitsSave.Enabled := True;
end;

procedure TfrmViewList.ButtonAdminListChangeClick(Sender: TObject);
var
  nIndex: Integer;
  sAdminName: string;
  sAdminIPaddr: string;
  nAdminPerMission: Integer;
  AdminInfo: pTAdminInfo;
begin
  nIndex := ListBoxAdminList.ItemIndex;
  if nIndex < 0 then
    Exit;

  sAdminName := Trim(EditAdminName.Text);
  sAdminIPaddr := Trim(EditAdminIPaddr.Text);
  nAdminPerMission := EditAdminPremission.Value;
  if (nAdminPerMission < 1) or (sAdminName = '') or not (nAdminPerMission in [0..10]) then
  begin
    Application.MessageBox('输入不正确！', '提示信息', MB_OK + MB_ICONERROR);
    EditAdminName.SetFocus;
    Exit;
  end;

  UserEngine.m_AdminList.LockR(7);
  try
    if (nIndex < 0) and (nIndex >= UserEngine.m_AdminList.Count) then
      Exit;
    AdminInfo := UserEngine.m_AdminList.Items[nIndex];
    AdminInfo.sChrName := sAdminName;
    AdminInfo.nLv := nAdminPerMission;
    AdminInfo.sIPaddr := sAdminIPaddr;
  finally
    UserEngine.m_AdminList.UnLockR;
  end;
  RefAdminList();
  ButtonAdminLitsSave.Enabled := True;
end;

procedure TfrmViewList.ButtonAdminListDelClick(Sender: TObject);
var
  nIndex: Integer;
begin
  nIndex := ListBoxAdminList.ItemIndex;
  if nIndex < 0 then
    Exit;
  UserEngine.m_AdminList.LockW(8);
  try
    if (nIndex < 0) and (nIndex >= UserEngine.m_AdminList.Count) then
      Exit;
    Dispose(pTAdminInfo(UserEngine.m_AdminList.Items[nIndex]));
    UserEngine.m_AdminList.Delete(nIndex);
  finally
    UserEngine.m_AdminList.UnLockW;
  end;
  RefAdminList();
  ButtonAdminLitsSave.Enabled := True;
end;

procedure TfrmViewList.RefItemBindAccount;
var
  I: Integer;
  ItemBind: pTItemBind;
begin
  GridItemBindAccount.RowCount := 2;
  GridItemBindAccount.Cells[0, 1] := '';
  GridItemBindAccount.Cells[1, 1] := '';
  GridItemBindAccount.Cells[2, 1] := '';
  GridItemBindAccount.Cells[3, 1] := '';
  ButtonItemBindAcountMod.Enabled := False;
  ButtonItemBindAcountDel.Enabled := False;

  g_ItemBindAccount.Lock;
  try
    GridItemBindAccount.RowCount := g_ItemBindAccount.Count + 1;
    for I := 0 to g_ItemBindAccount.Count - 1 do
    begin
      ItemBind := g_ItemBindAccount.Items[I];
      if ItemBind <> nil then
      begin
        GridItemBindAccount.Cells[0, I + 1] := UserEngine.GetStdItemName(ItemBind.nItemIdx);
        GridItemBindAccount.Cells[1, I + 1] := IntToStr(ItemBind.nItemIdx);
        GridItemBindAccount.Cells[2, I + 1] := IntToStr(ItemBind.nMakeIdex);
        GridItemBindAccount.Cells[3, I + 1] := ItemBind.sBindName;
      end;
    end;
  finally
    g_ItemBindAccount.UnLock;
  end;
end;

procedure TfrmViewList.GridItemBindAccountClick(Sender: TObject);
var
  nIndex: Integer;
  ItemBind: pTItemBind;
begin

  nIndex := GridItemBindAccount.Row - 1;
  if nIndex < 0 then
    Exit;

  g_ItemBindAccount.Lock;
  try
    if nIndex >= g_ItemBindAccount.Count then
      Exit;
    ItemBind := pTItemBind(g_ItemBindAccount.Items[nIndex]);
    EditItemBindAccountItemName.Text := UserEngine.GetStdItemName(ItemBind.nItemIdx);
    EditItemBindAccountItemIdx.Value := ItemBind.nItemIdx;
    EditItemBindAccountItemMakeIdx.Value := ItemBind.nMakeIdex;
    EditItemBindAccountName.Text := ItemBind.sBindName;
  finally
    g_ItemBindAccount.UnLock;
  end;
  ButtonItemBindAcountDel.Enabled := True;
end;

procedure TfrmViewList.EditItemBindAccountItemIdxChange(Sender: TObject);
begin
  if EditItemBindAccountItemIdx.Text = '' then
  begin
    EditItemBindAccountItemIdx.Text := '0';
    Exit;
  end;
  EditItemBindAccountItemName.Text := UserEngine.GetStdItemName(EditItemBindAccountItemIdx.Value);
  ButtonItemBindAcountMod.Enabled := True;
end;

procedure TfrmViewList.EditItemBindAccountItemMakeIdxChange(Sender: TObject);
begin
  if EditItemBindAccountItemIdx.Text = '' then
  begin
    EditItemBindAccountItemIdx.Text := '0';
    Exit;
  end;
  ButtonItemBindAcountMod.Enabled := True;
end;

procedure TfrmViewList.EditItemBindAccountNameChange(Sender: TObject);
begin
  ButtonItemBindAcountMod.Enabled := True;
end;

procedure TfrmViewList.ButtonItemBindAcountModClick(Sender: TObject);
var
  nSelIndex: Integer;
  nMakeIdex: Integer;
  nItemIdx: Integer;
  sBindName: string;
  ItemBind: pTItemBind;
begin
  nItemIdx := EditItemBindAccountItemIdx.Value;
  nMakeIdex := EditItemBindAccountItemMakeIdx.Value;
  sBindName := Trim(EditItemBindAccountName.Text);
  nSelIndex := GridItemBindAccount.Row - 1;
  if nSelIndex < 0 then
    Exit;
  g_ItemBindAccount.Lock;
  try
    if nSelIndex >= g_ItemBindAccount.Count then
      Exit;
    ItemBind := g_ItemBindAccount.Items[nSelIndex];
    ItemBind.nItemIdx := nItemIdx;
    ItemBind.nMakeIdex := nMakeIdex;
    ItemBind.sBindName := sBindName;
  finally
    g_ItemBindAccount.UnLock;
  end;
  SaveItemBindAccount();
  RefItemBindAccount();

end;

procedure TfrmViewList.ButtonItemBindAcountRefClick(Sender: TObject);
begin
  RefItemBindAccount();
end;

procedure TfrmViewList.ButtonItemBindAcountAddClick(Sender: TObject);
var
  I: Integer;
  nMakeIdex: Integer;
  nItemIdx: Integer;
  sBindName: string;
  ItemBind: pTItemBind;
begin
  nItemIdx := EditItemBindAccountItemIdx.Value;
  nMakeIdex := EditItemBindAccountItemMakeIdx.Value;
  sBindName := Trim(EditItemBindAccountName.Text);

  if (nItemIdx <= 0) or (nMakeIdex < 0) or (sBindName = '') then
  begin
    Application.MessageBox('输入的信息不正确！', '提示信息', MB_OK + MB_ICONERROR);
    Exit;
  end;

  g_ItemBindAccount.Lock;
  try
    for I := 0 to g_ItemBindAccount.Count - 1 do
    begin
      ItemBind := g_ItemBindAccount.Items[I];
      if (ItemBind.nItemIdx = nItemIdx) and (ItemBind.nMakeIdex = nMakeIdex) then
      begin
        Application.MessageBox('此物品已经绑定到其他的帐号了！', '提示信息', MB_OK + MB_ICONERROR);
        Exit;
      end;
    end;
    New(ItemBind);
    ItemBind.nItemIdx := nItemIdx;
    ItemBind.nMakeIdex := nMakeIdex;
    ItemBind.sBindName := sBindName;
    g_ItemBindAccount.Insert(0, ItemBind);
  finally
    g_ItemBindAccount.UnLock;
  end;
  SaveItemBindAccount();
  RefItemBindAccount();
end;

procedure TfrmViewList.ButtonItemBindAcountDelClick(Sender: TObject);
var
  ItemBind: pTItemBind;
  nSelIndex: Integer;
begin

  nSelIndex := GridItemBindAccount.Row - 1;
  if nSelIndex < 0 then
    Exit;
  g_ItemBindAccount.Lock;
  try
    if nSelIndex >= g_ItemBindAccount.Count then
      Exit;
    ItemBind := g_ItemBindAccount.Items[nSelIndex];
    Dispose(ItemBind);
    g_ItemBindAccount.Delete(nSelIndex);
  finally
    g_ItemBindAccount.UnLock;
  end;
  SaveItemBindAccount();
  RefItemBindAccount();
end;

procedure TfrmViewList.RefItemBindCharName;
var
  I: Integer;
  ItemBind: pTItemBind;
begin
  GridItemBindCharName.RowCount := 2;
  GridItemBindCharName.Cells[0, 1] := '';
  GridItemBindCharName.Cells[1, 1] := '';
  GridItemBindCharName.Cells[2, 1] := '';
  GridItemBindCharName.Cells[3, 1] := '';
  ButtonItemBindCharNameMod.Enabled := False;
  ButtonItemBindCharNameDel.Enabled := False;
  g_ItemBindCharName.Lock;
  try
    GridItemBindCharName.RowCount := g_ItemBindCharName.Count + 1;
    for I := 0 to g_ItemBindCharName.Count - 1 do
    begin
      ItemBind := g_ItemBindCharName.Items[I];
      if ItemBind <> nil then
      begin
        GridItemBindCharName.Cells[0, I + 1] := UserEngine.GetStdItemName(ItemBind.nItemIdx);
        GridItemBindCharName.Cells[1, I + 1] := IntToStr(ItemBind.nItemIdx);
        GridItemBindCharName.Cells[2, I + 1] := IntToStr(ItemBind.nMakeIdex);
        GridItemBindCharName.Cells[3, I + 1] := ItemBind.sBindName;
      end;
    end;
  finally
    g_ItemBindCharName.UnLock;
  end;
end;

procedure TfrmViewList.GridItemBindCharNameClick(Sender: TObject);
var
  nIndex: Integer;
  ItemBind: pTItemBind;
begin

  nIndex := GridItemBindCharName.Row - 1;
  if nIndex < 0 then
    Exit;

  g_ItemBindCharName.Lock;
  try
    if nIndex >= g_ItemBindCharName.Count then
      Exit;
    ItemBind := pTItemBind(g_ItemBindCharName.Items[nIndex]);
    EditItemBindCharNameItemName.Text := UserEngine.GetStdItemName(ItemBind.nItemIdx);
    EditItemBindCharNameItemIdx.Value := ItemBind.nItemIdx;
    EditItemBindCharNameItemMakeIdx.Value := ItemBind.nMakeIdex;
    EditItemBindCharNameName.Text := ItemBind.sBindName;
  finally
    g_ItemBindCharName.UnLock;
  end;
  ButtonItemBindCharNameDel.Enabled := True;
end;

procedure TfrmViewList.EditItemBindCharNameItemIdxChange(Sender: TObject);
begin
  if EditItemBindCharNameItemIdx.Text = '' then
  begin
    EditItemBindCharNameItemIdx.Text := '0';
    Exit;
  end;
  EditItemBindCharNameItemName.Text := UserEngine.GetStdItemName(EditItemBindCharNameItemIdx.Value);
  ButtonItemBindCharNameMod.Enabled := True;
end;

procedure TfrmViewList.EditItemBindCharNameItemMakeIdxChange(Sender: TObject);
begin
  if EditItemBindCharNameItemMakeIdx.Text = '' then
  begin
    EditItemBindCharNameItemMakeIdx.Text := '0';
    Exit;
  end;
  ButtonItemBindCharNameMod.Enabled := True;
end;

procedure TfrmViewList.EditItemBindCharNameNameChange(Sender: TObject);
begin
  ButtonItemBindCharNameMod.Enabled := True;
end;

procedure TfrmViewList.ButtonItemBindCharNameAddClick(Sender: TObject);
var
  I: Integer;
  nMakeIdex: Integer;
  nItemIdx: Integer;
  sBindName: string;
  ItemBind: pTItemBind;
begin
  nItemIdx := EditItemBindCharNameItemIdx.Value;
  nMakeIdex := EditItemBindCharNameItemMakeIdx.Value;
  sBindName := Trim(EditItemBindCharNameName.Text);

  if (nItemIdx <= 0) or (nMakeIdex < 0) or (sBindName = '') then
  begin
    Application.MessageBox('输入的信息不正确！', '提示信息', MB_OK + MB_ICONERROR);
    Exit;
  end;

  g_ItemBindCharName.Lock;
  try
    for I := 0 to g_ItemBindCharName.Count - 1 do
    begin
      ItemBind := g_ItemBindCharName.Items[I];
      if (ItemBind.nItemIdx = nItemIdx) and (ItemBind.nMakeIdex = nMakeIdex) then
      begin
        Application.MessageBox('此物品已经绑定到其他的角色上了！', '提示信息', MB_OK + MB_ICONERROR);
        Exit;
      end;
    end;
    New(ItemBind);
    ItemBind.nItemIdx := nItemIdx;
    ItemBind.nMakeIdex := nMakeIdex;
    ItemBind.sBindName := sBindName;
    g_ItemBindCharName.Insert(0, ItemBind);
  finally
    g_ItemBindCharName.UnLock;
  end;
  SaveItemBindCharName();
  RefItemBindCharName();
end;

procedure TfrmViewList.ButtonItemBindCharNameModClick(Sender: TObject);
var
  nSelIndex: Integer;
  nMakeIdex: Integer;
  nItemIdx: Integer;
  sBindName: string;
  ItemBind: pTItemBind;
begin

  nItemIdx := EditItemBindCharNameItemIdx.Value;
  nMakeIdex := EditItemBindCharNameItemMakeIdx.Value;
  sBindName := Trim(EditItemBindCharNameName.Text);
  nSelIndex := GridItemBindCharName.Row - 1;
  if nSelIndex < 0 then
    Exit;

  g_ItemBindCharName.Lock;
  try
    if nSelIndex >= g_ItemBindCharName.Count then
      Exit;
    ItemBind := g_ItemBindCharName.Items[nSelIndex];
    ItemBind.nItemIdx := nItemIdx;
    ItemBind.nMakeIdex := nMakeIdex;
    ItemBind.sBindName := sBindName;
  finally
    g_ItemBindCharName.UnLock;
  end;

  SaveItemBindCharName();
  RefItemBindCharName();

end;

procedure TfrmViewList.ButtonItemBindCharNameDelClick(Sender: TObject);
var
  ItemBind: pTItemBind;
  nSelIndex: Integer;
begin

  nSelIndex := GridItemBindCharName.Row - 1;
  if nSelIndex < 0 then
    Exit;
  g_ItemBindCharName.Lock;
  try
    if nSelIndex >= g_ItemBindCharName.Count then
      Exit;
    ItemBind := g_ItemBindCharName.Items[nSelIndex];
    Dispose(ItemBind);
    g_ItemBindCharName.Delete(nSelIndex);
  finally
    g_ItemBindCharName.UnLock;
  end;
  SaveItemBindCharName();
  RefItemBindCharName();
end;

procedure TfrmViewList.ButtonItemBindCharNameRefClick(Sender: TObject);
begin
  RefItemBindCharName();
end;

procedure TfrmViewList.RefItemBindIPaddr;
var
  I: Integer;
  ItemBind: pTItemBind;
begin
  GridItemBindIPaddr.RowCount := 2;
  GridItemBindIPaddr.Cells[0, 1] := '';
  GridItemBindIPaddr.Cells[1, 1] := '';
  GridItemBindIPaddr.Cells[2, 1] := '';
  GridItemBindIPaddr.Cells[3, 1] := '';
  ButtonItemBindIPaddrMod.Enabled := False;
  ButtonItemBindIPaddrDel.Enabled := False;
  g_ItemBindIPaddr.Lock;
  try
    GridItemBindIPaddr.RowCount := g_ItemBindIPaddr.Count + 1;
    for I := 0 to g_ItemBindIPaddr.Count - 1 do
    begin
      ItemBind := g_ItemBindIPaddr.Items[I];
      if ItemBind <> nil then
      begin
        GridItemBindIPaddr.Cells[0, I + 1] := UserEngine.GetStdItemName(ItemBind.nItemIdx);
        GridItemBindIPaddr.Cells[1, I + 1] := IntToStr(ItemBind.nItemIdx);
        GridItemBindIPaddr.Cells[2, I + 1] := IntToStr(ItemBind.nMakeIdex);
        GridItemBindIPaddr.Cells[3, I + 1] := ItemBind.sBindName;
      end;
    end;
  finally
    g_ItemBindIPaddr.UnLock;
  end;
end;

procedure TfrmViewList.GridItemBindIPaddrClick(Sender: TObject);
var
  nIndex: Integer;
  ItemBind: pTItemBind;
begin

  nIndex := GridItemBindIPaddr.Row - 1;
  if nIndex < 0 then
    Exit;

  g_ItemBindIPaddr.Lock;
  try
    if nIndex >= g_ItemBindIPaddr.Count then
      Exit;
    ItemBind := pTItemBind(g_ItemBindIPaddr.Items[nIndex]);
    EditItemBindIPaddrItemName.Text := UserEngine.GetStdItemName(ItemBind.nItemIdx);
    EditItemBindIPaddrItemIdx.Value := ItemBind.nItemIdx;
    EditItemBindIPaddrItemMakeIdx.Value := ItemBind.nMakeIdex;
    EditItemBindIPaddrName.Text := ItemBind.sBindName;
  finally
    g_ItemBindIPaddr.UnLock;
  end;
  ButtonItemBindIPaddrDel.Enabled := True;
end;

procedure TfrmViewList.EditItemBindIPaddrItemIdxChange(Sender: TObject);
begin
  if EditItemBindIPaddrItemIdx.Text = '' then
  begin
    EditItemBindIPaddrItemIdx.Text := '0';
    Exit;
  end;
  EditItemBindIPaddrItemName.Text := UserEngine.GetStdItemName(EditItemBindIPaddrItemIdx.Value);
  ButtonItemBindIPaddrMod.Enabled := True;
end;

procedure TfrmViewList.EditItemBindIPaddrItemMakeIdxChange(Sender: TObject);
begin
  if EditItemBindIPaddrItemMakeIdx.Text = '' then
  begin
    EditItemBindIPaddrItemMakeIdx.Text := '0';
    Exit;
  end;
  ButtonItemBindIPaddrMod.Enabled := True;
end;

procedure TfrmViewList.EditItemBindIPaddrNameChange(Sender: TObject);
begin
  ButtonItemBindIPaddrMod.Enabled := True;
end;

procedure TfrmViewList.ButtonItemBindIPaddrAddClick(Sender: TObject);
var
  I: Integer;
  nMakeIdex: Integer;
  nItemIdx: Integer;
  sBindName: string;
  ItemBind: pTItemBind;
begin
  nItemIdx := EditItemBindIPaddrItemIdx.Value;
  nMakeIdex := EditItemBindIPaddrItemMakeIdx.Value;
  sBindName := Trim(EditItemBindIPaddrName.Text);

  if not IsIPaddr(sBindName) then
  begin
    Application.MessageBox('IP地址格式输入不正确！', '提示信息', MB_OK + MB_ICONERROR);
    EditItemBindIPaddrName.SetFocus;
    Exit;
  end;

  if (nItemIdx <= 0) or (nMakeIdex < 0) then
  begin
    Application.MessageBox('输入的信息不正确！', '提示信息', MB_OK + MB_ICONERROR);
    Exit;
  end;

  g_ItemBindIPaddr.Lock;
  try
    for I := 0 to g_ItemBindIPaddr.Count - 1 do
    begin
      ItemBind := g_ItemBindIPaddr.Items[I];
      if (ItemBind.nItemIdx = nItemIdx) and (ItemBind.nMakeIdex = nMakeIdex) then
      begin
        Application.MessageBox('此物品已经绑定到其他的IP地址上了！', '提示信息', MB_OK + MB_ICONERROR);
        Exit;
      end;
    end;
    New(ItemBind);
    ItemBind.nItemIdx := nItemIdx;
    ItemBind.nMakeIdex := nMakeIdex;
    ItemBind.sBindName := sBindName;
    g_ItemBindIPaddr.Insert(0, ItemBind);
  finally
    g_ItemBindIPaddr.UnLock;
  end;
  SaveItemBindIPaddr();
  RefItemBindIPaddr();
end;

procedure TfrmViewList.ButtonItemBindIPaddrModClick(Sender: TObject);
var
  nSelIndex: Integer;
  nMakeIdex: Integer;
  nItemIdx: Integer;
  sBindName: string;
  ItemBind: pTItemBind;
begin

  nItemIdx := EditItemBindIPaddrItemIdx.Value;
  nMakeIdex := EditItemBindIPaddrItemMakeIdx.Value;
  sBindName := Trim(EditItemBindIPaddrName.Text);
  if not IsIPaddr(sBindName) then
  begin
    Application.MessageBox('IP地址格式输入不正确！', '提示信息', MB_OK + MB_ICONERROR);
    EditItemBindIPaddrName.SetFocus;
    Exit;
  end;
  nSelIndex := GridItemBindIPaddr.Row - 1;
  if nSelIndex < 0 then
    Exit;

  g_ItemBindIPaddr.Lock;
  try
    if nSelIndex >= g_ItemBindIPaddr.Count then
      Exit;
    ItemBind := g_ItemBindIPaddr.Items[nSelIndex];
    ItemBind.nItemIdx := nItemIdx;
    ItemBind.nMakeIdex := nMakeIdex;
    ItemBind.sBindName := sBindName;
  finally
    g_ItemBindIPaddr.UnLock;
  end;
  SaveItemBindIPaddr();
  RefItemBindIPaddr();
end;

procedure TfrmViewList.ButtonItemBindIPaddrDelClick(Sender: TObject);
var
  ItemBind: pTItemBind;
  nSelIndex: Integer;
begin

  nSelIndex := GridItemBindIPaddr.Row - 1;
  if nSelIndex < 0 then
    Exit;
  g_ItemBindIPaddr.Lock;
  try
    if nSelIndex >= g_ItemBindIPaddr.Count then
      Exit;
    ItemBind := g_ItemBindIPaddr.Items[nSelIndex];
    Dispose(ItemBind);
    g_ItemBindIPaddr.Delete(nSelIndex);
  finally
    g_ItemBindIPaddr.UnLock;
  end;
  SaveItemBindIPaddr();
  RefItemBindIPaddr();
end;

procedure TfrmViewList.ButtonItemBindIPaddrRefClick(Sender: TObject);
begin
  RefItemBindIPaddr();
end;

procedure TfrmViewList.ListBoxItemListKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
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
          if not InputQuery('查找', '输入名称:', sItemName) then
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

procedure TfrmViewList.ListBoxEnablePickUpListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxEnablePickUpList.ItemIndex >= 0 then
    ButtonEnablePickUpDelete.Enabled := True;
end;

procedure TfrmViewList.ListBoxitemList5Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxitemList5.ItemIndex >= 0 then
    ButtonEnablePickUpAdd.Enabled := True;
end;

procedure TfrmViewList.ButtonEnablePickUpAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if ListBoxItemList5.Items.Count >= 0 then
  begin
    for I := 0 to ListBoxItemList5.Items.Count - 1 do
    begin
      if not ListBoxItemList5.Selected[I] then
        Continue;

      sItemName := ListBoxItemList5.Items[I];
      if ListBoxEnablePickUpList.Items.IndexOf(sItemName) < 0 then
      begin
        ListBoxEnablePickUpList.Items.Add(sItemName);
      end;
    end;

    if not ListBoxEnablePickUpList.Items.IndexOf(sSTRING_GOLDNAME) < 0 then
    begin
      ListBoxEnablePickUpList.Items.Add(sSTRING_GOLDNAME);
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.ButtonEnablePickUpDeleteClick(Sender: TObject);
begin
  if ListBoxEnablePickUpList.ItemIndex >= 0 then
  begin
    ListBoxEnablePickUpList.Items.Delete(ListBoxEnablePickUpList.ItemIndex);
    ModValue();
  end;
  if ListBoxEnablePickUpList.ItemIndex < 0 then
    ButtonEnablePickUpDelete.Enabled := False;
end;

procedure TfrmViewList.ButtonEnablePickUpAddAllClick(Sender: TObject);
var
  I: Integer;
begin
  ListBoxEnablePickUpList.Items.Clear;
  for I := 0 to ListBoxItemList5.Items.Count - 1 do
  begin
    ListBoxEnablePickUpList.Items.Add(ListBoxItemList5.Items.Strings[I]);
  end;
  ModValue();
end;

procedure TfrmViewList.ButtonEnablePickUpDeleteAllClick(Sender: TObject);
begin
  ListBoxEnablePickUpList.Items.Clear;
  ButtonEnablePickUpDelete.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.ButtonEnablePickUpSaveClick(Sender: TObject);
var
  I: Integer;
begin
  g_EnablePickUpItemList.Lock;
  try
    g_EnablePickUpItemList.Clear;
    for I := 0 to ListBoxEnablePickUpList.Items.Count - 1 do
    begin
      g_EnablePickUpItemList.Add(ListBoxEnablePickUpList.Items.Strings[I])
    end;

    g_EnablePickUpItemList.Sorted := True;
  finally
    g_EnablePickUpItemList.UnLock;
  end;
  SaveEnablePickUpItem();
  uModValue();
end;

procedure TfrmViewList.ButtonPriorityPickUpAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if ListBoxItemList6.Items.Count >= 0 then
  begin
    for I := 0 to ListBoxItemList6.Items.Count - 1 do
    begin
      if not ListBoxItemList6.Selected[I] then
        Continue;

      sItemName := ListBoxItemList6.Items[I];
      if ListBoxPriorityPickUpList.Items.IndexOf(sItemName) < 0 then
      begin
        ListBoxPriorityPickUpList.Items.Add(sItemName);
      end;
    end;

    if not ListBoxPriorityPickUpList.Items.IndexOf(sSTRING_GOLDNAME) < 0 then
    begin
      ListBoxPriorityPickUpList.Items.Add(sSTRING_GOLDNAME);
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.ButtonPriorityPickUpDeleteClick(Sender: TObject);
begin
  if ListBoxPriorityPickUpList.ItemIndex >= 0 then
  begin
    ListBoxPriorityPickUpList.Items.Delete(ListBoxPriorityPickUpList.ItemIndex);
    ModValue();
  end;
  if ListBoxPriorityPickUpList.ItemIndex < 0 then
    ButtonPriorityPickUpDelete.Enabled := False;
end;

procedure TfrmViewList.ButtonPriorityPickUpAddAllClick(Sender: TObject);
var
  I: Integer;
begin
  ListBoxPriorityPickUpList.Items.Clear;
  for I := 0 to ListBoxItemList6.Items.Count - 1 do
  begin
    ListBoxPriorityPickUpList.Items.Add(ListBoxItemList6.Items.Strings[I]);
  end;
  ModValue();
end;

procedure TfrmViewList.ButtonPriorityPickUpDeleteAllClick(Sender: TObject);
begin
  ListBoxPriorityPickUpList.Items.Clear;
  ButtonPriorityPickUpDelete.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.ButtonPriorityPickUpSaveClick(Sender: TObject);
var
  I: Integer;
begin
  g_PriorityPickUpItemList.Lock;
  try
    g_PriorityPickUpItemList.Clear;
    for I := 0 to ListBoxPriorityPickUpList.Items.Count - 1 do
    begin
      g_PriorityPickUpItemList.Add(ListBoxPriorityPickUpList.Items.Strings[I])
    end;

    g_PriorityPickUpItemList.Sorted := True;
  finally
    g_PriorityPickUpItemList.UnLock;
  end;
  SavePriorityPickUpItem();
  uModValue();
end;

procedure TfrmViewList.ListBoxPriorityPickUpListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxPriorityPickUpList.ItemIndex >= 0 then
    ButtonPriorityPickUpDelete.Enabled := True;
end;

procedure TfrmViewList.ListBoxitemList6Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxitemList6.ItemIndex >= 0 then
    ButtonPriorityPickUpAdd.Enabled := True;
end;

procedure TfrmViewList.btnNameFilterSaveClick(Sender: TObject);
var
  I, J: Integer;
  IsFound: Boolean;
  S: string;
begin
  g_NameFilterList.Lock;
  try
    g_NameFilterList.Clear;
    for I := 0 to mmoNameFilterList.Lines.Count - 1 do
    begin
      S := LowerCase(mmoNameFilterList.Lines.Strings[I]);

      IsFound := False;
      for J := 0 to g_NameFilterList.Count - 1 do
      begin
        if SameText(g_NameFilterList[J], S) then
        begin
          IsFound := True;
          Break;
        end;
      end;

      if not IsFound then
        g_NameFilterList.Add(S);
    end;
  finally
    g_NameFilterList.UnLock;
  end;
  SaveNameFilterList();

  g_InputBoxFilterList.Lock;
  try
    g_InputBoxFilterList.Clear;
    for I := 0 to mmoInputBoxFilterList.Lines.Count - 1 do
    begin
      S := LowerCase(mmoInputBoxFilterList.Lines.Strings[I]);

      IsFound := False;
      for J := 0 to g_InputBoxFilterList.Count - 1 do
      begin
        if SameText(g_InputBoxFilterList[J], S) then
        begin
          IsFound := True;
          Break;
        end;
      end;

      if not IsFound then
        g_InputBoxFilterList.Add(S);
    end;
  finally
    g_InputBoxFilterList.UnLock;
  end;
  SaveInputBoxFilterList();

  uModValue();
end;

procedure TfrmViewList.mmoNameFilterListChange(Sender: TObject);
begin
  ModValue();
end;

procedure TfrmViewList.btnMoveGuardPickItemAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if lstMoveGuardAllItem.Items.Count >= 0 then
  begin
    for I := 0 to lstMoveGuardAllItem.Items.Count - 1 do
    begin
      if not lstMoveGuardAllItem.Selected[I] then
        Continue;

      sItemName := lstMoveGuardAllItem.Items[I];
      if lstMoveGuardPickItemList.Items.IndexOf(sItemName) < 0 then
      begin
        lstMoveGuardPickItemList.Items.AddObject(sItemName, TObject(I));
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.btnMoveGuardPickItemDelClick(Sender: TObject);
begin
  if lstMoveGuardPickItemList.ItemIndex >= 0 then
  begin
    lstMoveGuardPickItemList.Items.Delete(lstMoveGuardPickItemList.ItemIndex);
    ModValue();
  end;
  if lstMoveGuardPickItemList.ItemIndex < 0 then
    btnMoveGuardPickItemDel.Enabled := False;
end;

procedure TfrmViewList.btnMoveGuardPickItemAddAllClick(Sender: TObject);
var
  I: Integer;
begin
  lstMoveGuardPickItemList.Items.Clear;
  for I := 0 to lstMoveGuardAllItem.Items.Count - 1 do
  begin
    lstMoveGuardPickItemList.Items.AddObject(lstMoveGuardAllItem.Items.Strings[I], TObject(I));
  end;
  ModValue();
end;

procedure TfrmViewList.btnMoveGuardPickItemDelAllClick(Sender: TObject);
begin
  lstMoveGuardPickItemList.Items.Clear;
  btnMoveGuardPickItemDel.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.btnMoveGuardPickItemSaveClick(Sender: TObject);
var
  I: Integer;
begin
  g_MoveGuardPickItemList.Lock;
  try
    g_MoveGuardPickItemList.Clear;
    for I := 0 to lstMoveGuardPickItemList.Items.Count - 1 do
    begin
      g_MoveGuardPickItemList.Add(lstMoveGuardPickItemList.Items.Strings[I]);
    end;

    g_MoveGuardPickItemList.Sorted := True;
  finally
    g_MoveGuardPickItemList.UnLock;
  end;
  SaveMoveGuardPickItem();
  uModValue();
end;

procedure TfrmViewList.lstMoveGuardPickItemListClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if lstMoveGuardPickItemList.ItemIndex >= 0 then
    btnMoveGuardPickItemDel.Enabled := True;
end;

procedure TfrmViewList.chkMoveSuperGuardPickItemClick(Sender: TObject);
begin
  g_Config.boMoveSuperGuardPickItem := chkMoveSuperGuardPickItem.Checked;
  Config.WriteBool('Setup', 'MoveSuperGuardPickItem', g_Config.boMoveSuperGuardPickItem);
end;

procedure TfrmViewList.chkMoveSuperGuardAttackMonClick(Sender: TObject);
begin
  g_Config.boMoveSuperGuardAttackMon := chkMoveSuperGuardAttackMon.Checked;
  Config.WriteBool('Setup', 'MoveSuperGuardAttackMon', g_Config.boMoveSuperGuardAttackMon);
end;

procedure TfrmViewList.chkMoveSuperGuardAttackBBClick(Sender: TObject);
begin
  g_Config.boMoveSuperGuardAttackBB := chkMoveSuperGuardAttackBB.Checked;
  Config.WriteBool('Setup', 'MoveSuperGuardAttackBB', g_Config.boMoveSuperGuardAttackBB);
end;

procedure TfrmViewList.chkMoveArcherGuardPickItemClick(Sender: TObject);
begin
  g_Config.boMoveArcherGuardPickItem := chkMoveArcherGuardPickItem.Checked;
  Config.WriteBool('Setup', 'MoveArcherGuardPickItem', g_Config.boMoveArcherGuardPickItem);
end;

procedure TfrmViewList.btnDisableItemFromAddClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if ListBoxitemList4.Items.Count >= 0 then
  begin
    for I := 0 to ListBoxitemList4.Items.Count - 1 do
    begin
      if not ListBoxitemList4.Selected[I] then
        Continue;

      sItemName := IntToStr(I) + '  ' + ListBoxitemList4.Items.Strings[I];
      if lstDisableShowItemFrom.Items.IndexOf(sItemName) < 0 then
      begin
        lstDisableShowItemFrom.Items.AddObject(sItemName, TObject(I));
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.btnDisableItemFromDelClick(Sender: TObject);
begin
  if lstDisableShowItemFrom.ItemIndex >= 0 then
  begin
    lstDisableShowItemFrom.Items.Delete(lstDisableShowItemFrom.ItemIndex);
    ModValue();
  end;
  if lstDisableShowItemFrom.ItemIndex < 0 then
    btnDisableItemFromDel.Enabled := False;
end;

procedure TfrmViewList.btnDisableItemFromAddAllClick(Sender: TObject);
var
  I: Integer;
begin
  lstDisableShowItemFrom.Items.Clear;
  for I := 0 to ListBoxitemList4.Items.Count - 1 do
  begin
    lstDisableShowItemFrom.Items.AddObject(IntToStr(I) + '  ' + ListBoxitemList4.Items.Strings[I], TObject(I));
  end;
  ModValue();
end;

procedure TfrmViewList.btnDisableItemFromDelAllClick(Sender: TObject);
begin
  lstDisableShowItemFrom.Items.Clear;
  btnDisableItemFromDel.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.btnDisableItemFromSaveClick(Sender: TObject);
var
  I: Integer;
  sItemIdx: string;
begin
  g_DisableShowItemFromList.Lock;
  try
    g_DisableShowItemFromList.Clear;
    for I := 0 to lstDisableShowItemFrom.Items.Count - 1 do
    begin
      g_DisableShowItemFromList.Add(Integer(lstDisableShowItemFrom.Items.Objects[I]), Trim(GetValidStr3_Ex(lstDisableShowItemFrom.Items.Strings
        [I], sItemIdx, ' ')));
    end;

    g_DisableShowItemFromList.Sort;
  finally
    g_DisableShowItemFromList.UnLock;
  end;
  SaveDisableShowItemFromList();
  uModValue();
end;

procedure TfrmViewList.ListBoxitemList4Click(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if ListBoxitemList4.ItemIndex >= 0 then
    btnDisableItemFromAdd.Enabled := True;
end;

procedure TfrmViewList.seVerifyCodeLenChange(Sender: TObject);
begin
  btnVerifyCodeOK.Enabled := True;
end;

procedure TfrmViewList.btnVerifyCodeOKClick(Sender: TObject);
begin
  g_sVerifyCodeChrs := mmoVerifyCodeChrs.Text;
  g_Config.nVerifyCodeLen := seVerifyCodeLen.Value;
  g_Config.nVerifyCodeTimeOut := seVerifyCodeTimeOut.Value;
  g_Config.nVerifyCodeRefreshCount := seVerifyCodeRefreshCount.Value;
  g_Config.nVerifyCodeFailCount := seVerifyCodeFailCount.Value;

  Config.WriteInteger('Setup', 'VerifyCodeLen', g_Config.nVerifyCodeLen);
  Config.WriteInteger('Setup', 'VerifyCodeTimeOut', g_Config.nVerifyCodeTimeOut);
  Config.WriteInteger('Setup', 'VerifyCodeRefreshCount', g_Config.nVerifyCodeRefreshCount);
  Config.WriteInteger('Setup', 'VerifyCodeFailCount', g_Config.nVerifyCodeFailCount);

  SaveVerifyCodeChrs;
  btnVerifyCodeOK.Enabled := False;
end;

procedure TfrmViewList.btnPreviewItemMonAddClick(Sender: TObject);

  function CheckMonInList(MonName: string): Boolean;
  var
    I: Integer;
    ListItem: TListItem;
  begin
    Result := False;
    for I := 0 to lvPreviewItemMon.Items.Count - 1 do
    begin
      ListItem := lvPreviewItemMon.Items[I];
      if SameText(MonName, ListItem.Caption) then
      begin
        Result := True;
        Break;
      end;
    end;
  end;

var
  I: Integer;
  sMonName: string;
  ListItem: TListItem;
  IsAdd: Boolean;
begin
  if lstPreviewItemMonAll.Items.Count >= 0 then
  begin
    IsAdd := False;
    for I := 0 to lstPreviewItemMonAll.Items.Count - 1 do
    begin
      if not lstPreviewItemMonAll.Selected[I] then
        Continue;
      sMonName := lstPreviewItemMonAll.Items.Strings[I];

      if not CheckMonInList(sMonName) then
      begin
        ListItem := lvPreviewItemMon.Items.Add;
        ListItem.Caption := sMonName;
        ListItem.SubItems.Add(IntToStr(sePreviewItemMonRefreshTime.Value));
        IsAdd := True;
      end;
    end;

    if IsAdd then
      btnPreviewItemMonSave.Enabled := True;
  end;
end;

procedure TfrmViewList.btnPreviewItemMonDelClick(Sender: TObject);
begin
  if lvPreviewItemMon.ItemIndex >= 0 then
  begin
    lvPreviewItemMon.Items.Delete(lvPreviewItemMon.ItemIndex);
    btnPreviewItemMonSave.Enabled := True;
  end;

  if lvPreviewItemMon.ItemIndex < 0 then
    btnPreviewItemMonDel.Enabled := False;
end;

procedure TfrmViewList.btnPreviewItemMonAddAllClick(Sender: TObject);
var
  I: Integer;
  ListItem: TListItem;
begin
  lvPreviewItemMon.Items.Clear;
  for I := 0 to lstPreviewItemMonAll.Items.Count - 1 do
  begin
    ListItem := lvPreviewItemMon.Items.Add;
    ListItem.Caption := lstPreviewItemMonAll.Items[I];
    ListItem.SubItems.Add(IntToStr(sePreviewItemMonRefreshTime.Value));
  end;

  btnPreviewItemMonSave.Enabled := True;
end;

procedure TfrmViewList.btnPreviewItemMonDelAllClick(Sender: TObject);
begin
  lvPreviewItemMon.Items.Clear;
  btnPreviewItemMonDel.Enabled := False;
  btnPreviewItemMonSave.Enabled := True;
end;

procedure TfrmViewList.btnPreviewItemMonSaveClick(Sender: TObject);
var
  I: Integer;
  ListItem: TListItem;
  dwValue: Integer;
begin
  g_PreviewItemMonList.Lock;
  try
    g_PreviewItemMonList.Clear;
    for I := 0 to lvPreviewItemMon.Items.Count - 1 do
    begin
      ListItem := lvPreviewItemMon.Items[I];

      dwValue := StrToIntDef(ListItem.SubItems[0], 0);
      g_PreviewItemMonList.AddObject(ListItem.Caption, TObject(dwValue));
    end;

    g_PreviewItemMonList.Sort;
  finally
    g_PreviewItemMonList.UnLock;
  end;

  SavePreviewItemMonList();
  btnPreviewItemMonSave.Enabled := False;
end;

procedure TfrmViewList.sePreviewMonItemShowTimeChange(Sender: TObject);
begin
  g_Config.nPreviewMonItemShowTime := sePreviewMonItemShowTime.Value;
  Config.WriteInteger('Setup', 'PreviewMonItemShowTime', g_Config.nPreviewMonItemShowTime);
end;

procedure TfrmViewList.lvPreviewItemMonSelectItem(Sender: TObject; Item: TListItem; Selected: Boolean);
begin
  btnPreviewItemMonDel.Enabled := lvPreviewItemMon.ItemIndex >= 0;
end;

procedure TfrmViewList.lstDisableShowItemFromClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if lstDisableShowItemFrom.ItemIndex >= 0 then
    btnDisableItemFromDel.Enabled := True;
end;

procedure TfrmViewList.btnAddDisableRangePickItemClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if lstDisableRangePickItems.Items.Count >= 0 then
  begin
    for I := 0 to lstDisableRangePickItems.Items.Count - 1 do
    begin
      if not lstDisableRangePickItems.Selected[I] then
        Continue;

      sItemName := lstDisableRangePickItems.Items[I];
      if lstDisableRangePickItem.Items.IndexOf(sItemName) < 0 then
      begin
        lstDisableRangePickItem.Items.Add(sItemName);
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.btnDelDisableRangePickItemClick(Sender: TObject);
begin
  if lstDisableRangePickItem.ItemIndex >= 0 then
  begin
    lstDisableRangePickItem.Items.Delete(lstDisableRangePickItem.ItemIndex);
    ModValue();
  end;
  if lstDisableRangePickItem.ItemIndex < 0 then
    btnDelDisableRangePickItem.Enabled := False;
end;

procedure TfrmViewList.btnAddAllDisableRangePickItemClick(Sender: TObject);
var
  I: Integer;
begin
  lstDisableRangePickItem.Items.Clear;
  for I := 0 to lstDisableRangePickItems.Items.Count - 1 do
  begin
    lstDisableRangePickItem.Items.Add(lstDisableRangePickItems.Items.Strings[I]);
  end;
  ModValue();
end;

procedure TfrmViewList.btnDelAllDisableRangePickItemClick(Sender: TObject);
begin
  lstDisableRangePickItem.Items.Clear;
  btnDelDisableRangePickItem.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.btnSaveDisableRangePickItemClick(Sender: TObject);
var
  I: Integer;
begin
  g_DisableRangePickItemList.Lock;
  try
    g_DisableRangePickItemList.Clear;
    for I := 0 to lstDisableRangePickItem.Items.Count - 1 do
    begin
      g_DisableRangePickItemList.Add(lstDisableRangePickItem.Items.Strings[I]);
    end;

    g_DisableRangePickItemList.Sort;
  finally
    g_DisableRangePickItemList.UnLock;
  end;

  SaveDisableRangePickItem();
  uModValue();
end;

procedure TfrmViewList.lstDisableRangePickItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if lstDisableRangePickItem.ItemIndex >= 0 then
    btnDelDisableRangePickItem.Enabled := True;
end;

procedure TfrmViewList.lstDisableRangePickItemsClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if lstDisableRangePickItems.ItemIndex >= 0 then
    btnAddDisableRangePickItem.Enabled := True;
end;

procedure TfrmViewList.btnAddDisableDropToBagItemClick(Sender: TObject);
var
  I: Integer;
  sItemName: string;
begin
  if lstDisableDropToBagItems.Items.Count >= 0 then
  begin
    for I := 0 to lstDisableDropToBagItems.Items.Count - 1 do
    begin
      if not lstDisableDropToBagItems.Selected[I] then
        Continue;

      sItemName := lstDisableDropToBagItems.Items[I];
      if lstDisableDropToBagItem.Items.IndexOf(sItemName) < 0 then
      begin
        lstDisableDropToBagItem.Items.Add(sItemName);
      end;
    end;

    ModValue();
  end;
end;

procedure TfrmViewList.btnDelDisableDropToBagItemClick(Sender: TObject);
begin
  if lstDisableDropToBagItem.ItemIndex >= 0 then
  begin
    lstDisableDropToBagItem.Items.Delete(lstDisableDropToBagItem.ItemIndex);
    ModValue();
  end;
  if lstDisableDropToBagItem.ItemIndex < 0 then
    btnDelDisableDropToBagItem.Enabled := False;
end;

procedure TfrmViewList.btnAddAllDisableDropToBagItemClick(Sender: TObject);
var
  I: Integer;
begin
  lstDisableDropToBagItem.Items.Clear;
  for I := 0 to lstDisableDropToBagItems.Items.Count - 1 do
  begin
    lstDisableDropToBagItem.Items.Add(lstDisableDropToBagItems.Items.Strings[I]);
  end;
  ModValue();
end;

procedure TfrmViewList.btnDelAllDisableDropToBagItemClick(Sender: TObject);
begin
  lstDisableDropToBagItem.Items.Clear;
  btnDelDisableDropToBagItem.Enabled := False;
  ModValue();
end;

procedure TfrmViewList.btnSaveDisableDropToBagItemClick(Sender: TObject);
var
  I: Integer;
begin
  g_DisableDropToBagItemList.Lock;
  try
    g_DisableDropToBagItemList.Clear;
    for I := 0 to lstDisableDropToBagItem.Items.Count - 1 do
    begin
      g_DisableDropToBagItemList.Add(lstDisableDropToBagItem.Items.Strings[I]);
    end;

    g_DisableDropToBagItemList.Sort;
  finally
    g_DisableDropToBagItemList.UnLock;
  end;

  SaveDisableDropToBagItem();
  uModValue();
end;

procedure TfrmViewList.lstDisableDropToBagItemClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if lstDisableDropToBagItem.ItemIndex >= 0 then
    btnDelDisableDropToBagItem.Enabled := True;
end;

procedure TfrmViewList.lstDisableDropToBagItemsClick(Sender: TObject);
begin
  if not boOpened then
    Exit;
  if lstDisableDropToBagItems.ItemIndex >= 0 then
    btnAddDisableDropToBagItem.Enabled := True;
end;

end.


unit HumanInfo;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ObjBase, ObjHero, StdCtrls, Spin, ComCtrls, ExtCtrls, Grids,
  SpinEditEx, ObjPlayer, Math;

type
  TfrmHumanInfo = class(TForm)
    PageControl1: TPageControl;
    tsBaseInfo: TTabSheet;
    TabSheet2: TTabSheet;
    GroupBox1: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Label5: TLabel;
    Label6: TLabel;
    Label7: TLabel;
    EditName: TEdit;
    EditMap: TEdit;
    EditXY: TEdit;
    EditAccount: TEdit;
    EditIPaddr: TEdit;
    EditLogonTime: TEdit;
    EditLogonLong: TEdit;
    GroupBox2: TGroupBox;
    Label12: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    Label10: TLabel;
    EditGold: TSpinEditLongWord;
    EditPKPoint: TSpinEditEx;
    EditExp: TSpinEditEx;
    TabSheet3: TTabSheet;
    TabSheet4: TTabSheet;
    TabSheet5: TTabSheet;
    TabSheet6: TTabSheet;
    GroupBox3: TGroupBox;
    Label11: TLabel;
    EditAC: TEdit;
    Label13: TLabel;
    EditMAC: TEdit;
    Label14: TLabel;
    EditDC: TEdit;
    EditMC: TEdit;
    Label15: TLabel;
    EditSC: TEdit;
    Label16: TLabel;
    EditHP: TEdit;
    Label17: TLabel;
    Label18: TLabel;
    EditMP: TEdit;
    Timer: TTimer;
    GroupBox6: TGroupBox;
    CheckBoxGameMaster: TCheckBox;
    CheckBoxSuperMan: TCheckBox;
    CheckBoxObserver: TCheckBox;
    GroupBox9: TGroupBox;
    Label26: TLabel;
    Label27: TLabel;
    Label28: TLabel;
    Label29: TLabel;
    EditGameGold: TSpinEditLongWord;
    EditGamePoint: TSpinEditLongWord;
    EditCreditPoint: TSpinEditEx;
    EditBonusPoint: TSpinEditEx;
    Label19: TLabel;
    EditEditBonusPointUsed: TSpinEditEx;
    ButtonSave: TButton;
    GroupBox11: TGroupBox;
    Label20: TLabel;
    EditSayMsg: TEdit;
    Label21: TLabel;
    EditMaxExp: TSpinEditEx;
    Label22: TLabel;
    EditNGLevel: TSpinEditEx;
    Label34: TLabel;
    EditNGExp: TSpinEditEx;
    Label23: TLabel;
    EditNGMaxExp: TSpinEditEx;
    Label24: TLabel;
    seGameDiamond: TSpinEditLongWord;
    Label25: TLabel;
    seGameGird: TSpinEditLongWord;
    EditLevel: TSpinEditLongWord;
    pgc1: TPageControl;
    ts3: TTabSheet;
    ts4: TTabSheet;
    ts5: TTabSheet;
    GridUserItem: TStringGrid;
    GridJewelryBoxItems: TStringGrid;
    GridGodBlessItems: TStringGrid;
    EditHumanStatus: TEdit;
    lbl1: TLabel;
    ButtonKick: TButton;
    CheckBoxMonitor: TCheckBox;
    Label30: TLabel;
    EditGameGlory: TSpinEditEx;
    TabSheet7: TTabSheet;
    tsVarU: TTabSheet;
    tsVarT: TTabSheet;
    GridBagItem: TStringGrid;
    GridStorageItemEx: TStringGrid;
    strGridVarU: TStringGrid;
    strGridVarT: TStringGrid;
    pnl1: TPanel;
    btnSaveT: TButton;
    Panel1: TPanel;
    btnSaveU: TButton;
    tbcStorage: TTabControl;
    GridStorageItem: TStringGrid;
    GroupBox4: TGroupBox;
    lbl2: TLabel;
    edtNewValue11: TEdit;
    Label31: TLabel;
    edtKillMonBurstRate: TEdit;
    TabSheet1: TTabSheet;
    TabSheet8: TTabSheet;
    Panel2: TPanel;
    btnSaveJ: TButton;
    Panel3: TPanel;
    btnSaveZ: TButton;
    strGridVarZ: TStringGrid;
    strGridVarJ: TStringGrid;
    procedure TimerTimer(Sender: TObject);
    procedure CheckBoxMonitorClick(Sender: TObject);
    procedure ButtonKickClick(Sender: TObject);
    procedure ButtonSaveClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure btnSaveUClick(Sender: TObject);
    procedure btnSaveTClick(Sender: TObject);
    procedure strGridVarUSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: String);
    procedure strGridVarTSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: String);
    procedure btnSaveJClick(Sender: TObject);
    procedure btnSaveZClick(Sender: TObject);
    procedure strGridVarJSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
    procedure strGridVarZSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
  private
    procedure RefHumanInfo();
    { Private declarations }
  public
    BaseObject: TBaseObject;

    FVarUChanged: array [0 .. 499] of Boolean;
    FVarTChanged: array [0 .. 499] of Boolean;
    FVarJChanged: array [0 .. 499] of Boolean;
    FVarZChanged: array [0 .. 499] of Boolean;

    procedure Open();
    { Public declarations }
  end;

var
  frmHumanInfo: TfrmHumanInfo;

implementation

uses UsrEngn, M2Share, Grobal2;

{$R *.dfm}

var
  boRefHuman: Boolean = False;
  { TfrmHumanInfo }

procedure TfrmHumanInfo.FormCreate(Sender: TObject);
var
  I: Integer;
begin
  GridUserItem.Cells[0, 0] := '装备位置';
  GridUserItem.Cells[1, 0] := '装备名称';
  GridUserItem.Cells[2, 0] := '系列号';
  GridUserItem.Cells[3, 0] := '持久';
  GridUserItem.Cells[4, 0] := '攻';
  GridUserItem.Cells[5, 0] := '魔';
  GridUserItem.Cells[6, 0] := '道';
  GridUserItem.Cells[7, 0] := '防';
  GridUserItem.Cells[8, 0] := '魔防';
  GridUserItem.Cells[9, 0] := '附加属性';

  for I := 0 to Length(g_UserItemNames) - 1 do
  begin
    GridUserItem.Cells[0, I + 1] := g_UserItemNames[I];
  end;

  GridBagItem.Cells[0, 0] := '序号';
  GridBagItem.Cells[1, 0] := '装备名称';
  GridBagItem.Cells[2, 0] := '系列号';
  GridBagItem.Cells[3, 0] := '持久';
  GridBagItem.Cells[4, 0] := '攻';
  GridBagItem.Cells[5, 0] := '魔';
  GridBagItem.Cells[6, 0] := '道';
  GridBagItem.Cells[7, 0] := '防';
  GridBagItem.Cells[8, 0] := '魔防';
  GridBagItem.Cells[9, 0] := '附加属性';

  GridJewelryBoxItems.Cells[0, 0] := '序号';
  GridJewelryBoxItems.Cells[1, 0] := '装备名称';
  GridJewelryBoxItems.Cells[2, 0] := '系列号';
  GridJewelryBoxItems.Cells[3, 0] := '持久';
  GridJewelryBoxItems.Cells[4, 0] := '攻';
  GridJewelryBoxItems.Cells[5, 0] := '魔';
  GridJewelryBoxItems.Cells[6, 0] := '道';
  GridJewelryBoxItems.Cells[7, 0] := '防';
  GridJewelryBoxItems.Cells[8, 0] := '魔防';
  GridJewelryBoxItems.Cells[9, 0] := '附加属性';

  GridGodBlessItems.Cells[0, 0] := '序号';
  GridGodBlessItems.Cells[1, 0] := '装备名称';
  GridGodBlessItems.Cells[2, 0] := '系列号';
  GridGodBlessItems.Cells[3, 0] := '持久';
  GridGodBlessItems.Cells[4, 0] := '攻';
  GridGodBlessItems.Cells[5, 0] := '魔';
  GridGodBlessItems.Cells[6, 0] := '道';
  GridGodBlessItems.Cells[7, 0] := '防';
  GridGodBlessItems.Cells[8, 0] := '魔防';
  GridGodBlessItems.Cells[9, 0] := '附加属性';

  GridStorageItem.Cells[0, 0] := '序号';
  GridStorageItem.Cells[1, 0] := '装备名称';
  GridStorageItem.Cells[2, 0] := '系列号';
  GridStorageItem.Cells[3, 0] := '持久';
  GridStorageItem.Cells[4, 0] := '攻';
  GridStorageItem.Cells[5, 0] := '魔';
  GridStorageItem.Cells[6, 0] := '道';
  GridStorageItem.Cells[7, 0] := '防';
  GridStorageItem.Cells[8, 0] := '魔防';
  GridStorageItem.Cells[9, 0] := '附加属性';

  GridStorageItemEx.Cells[0, 0] := '序号';
  GridStorageItemEx.Cells[1, 0] := '装备名称';
  GridStorageItemEx.Cells[2, 0] := '系列号';
  GridStorageItemEx.Cells[3, 0] := '持久';
  GridStorageItemEx.Cells[4, 0] := '攻';
  GridStorageItemEx.Cells[5, 0] := '魔';
  GridStorageItemEx.Cells[6, 0] := '道';
  GridStorageItemEx.Cells[7, 0] := '防';
  GridStorageItemEx.Cells[8, 0] := '魔防';
  GridStorageItemEx.Cells[9, 0] := '附加属性';

  PageControl1.ActivePageIndex := 0;
end;

procedure TfrmHumanInfo.Open;
begin
  FillChar(FVarUChanged, SizeOf(FVarUChanged), 0);
  FillChar(FVarTChanged, SizeOf(FVarTChanged), 0);
  FillChar(FVarJChanged, SizeOf(FVarJChanged), 0);
  FillChar(FVarZChanged, SizeOf(FVarZChanged), 0);

  btnSaveU.Enabled := False;
  btnSaveT.Enabled := False;

  RefHumanInfo();
  ButtonKick.Enabled := True;
  Timer.Enabled := True;
  ShowModal;
  CheckBoxMonitor.Checked := False;
  Timer.Enabled := False;
end;

procedure TfrmHumanInfo.RefHumanInfo;
var
  I: Integer;
  nTotleUsePoint: Integer;
  StdItem: pTStdItem;
  Item: TStdItem;
  UserItem: pTUserItem;
  PlayObject: TPlayObject;
  HeroObject: THeroObject;
  SmartObject: TSmartObject;

begin
  if (BaseObject = nil) then
  begin
    Exit;
  end;

  TabSheet3.TabVisible := True;
  TabSheet6.TabVisible := True;
  tsVarU.TabVisible := True;
  tsVarT.TabVisible := True;

  GroupBox6.Visible := True;
  GroupBox9.Visible := True;
  GroupBox11.Visible := True;

  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
    SmartObject := TSmartObject(BaseObject)
  else
    Exit;

  HeroObject := nil;
  PlayObject := nil;
  if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
    PlayObject := TPlayObject(BaseObject)
  else if (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
    HeroObject := THeroObject(BaseObject);
  if (PlayObject <> nil) then
  begin
    if PlayObject.m_boOffLine then
      EditSayMsg.Enabled := True
    else
      EditSayMsg.Enabled := False;
    EditSayMsg.Text := PlayObject.m_sAutoSendMsg;
  end
  else
  begin
    TabSheet3.TabVisible := False;
    TabSheet6.TabVisible := False;
    GroupBox6.Visible := False;
    GroupBox9.Visible := False;
    GroupBox11.Visible := False;
    tsVarU.TabVisible := False;
    tsVarT.TabVisible := False;
  end;

  EditName.Text := BaseObject.m_sCharName;
  EditMap.Text := BaseObject.m_sMapName + '(' + BaseObject.m_PEnvir.sMapDesc + ')';
  EditXY.Text := IntToStr(BaseObject.m_nCurrX) + ':' + IntToStr(BaseObject.m_nCurrY);

  EditGold.Enabled := True;

  if (PlayObject <> nil) then
  begin
    EditAccount.Text := PlayObject.m_sUserID;
    EditIPaddr.Text := PlayObject.m_sIPaddr;
    EditLogonTime.Text := DateTimeToStr(PlayObject.m_dLogonTime);
    EditLogonLong.Text := IntToStr((MyGetTickCount - PlayObject.m_dwLogonTick) div (60 * 1000)) + ' 分钟';

  end
  else if HeroObject <> nil then
  begin
    EditAccount.Text := HeroObject.m_sUserID;
    EditIPaddr.Text := TPlayObject(HeroObject.m_Master).m_sIPaddr;
    EditLogonTime.Text := DateTimeToStr(HeroObject.m_dLogonTime);
    EditLogonLong.Text := IntToStr((MyGetTickCount - HeroObject.m_dwLogonTick) div (60 * 1000)) + ' 分钟';

    EditGold.Enabled := False;
  end;

  edtNewValue11.Text := IntToStr(Min(BaseObject.m_WAbil.NewValue[11], 100)) + '%';
  edtKillMonBurstRate.Text := IntToStr(SmartObject.m_dwKillMonBurstRate);

  EditLevel.Value := BaseObject.m_Abil.Level;
  EditGold.Value := BaseObject.m_nGold;
  EditPKPoint.Value := SmartObject.m_nPkPoint;
  EditExp.Value := BaseObject.m_Abil.Exp;
  EditMaxExp.Value := BaseObject.m_Abil.MaxExp;

  EditNGMaxExp.Enabled := False;

  if SmartObject.m_boTrainingNG then
  begin
    EditNGLevel.Enabled := True;
    EditNGExp.Enabled := False;
    EditNGLevel.Value := SmartObject.m_AbilNG.Level;
    EditNGExp.Value := SmartObject.m_AbilNG.Exp;
    EditNGMaxExp.Value := SmartObject.m_AbilNG.MaxExp;
  end
  else
  begin
    EditNGLevel.Enabled := False;
    EditNGExp.Enabled := False;
    EditNGExp.Value := 0;
    EditNGMaxExp.Value := 0;
  end;

  EditAC.Text := IntToStr(BaseObject.m_WAbil.AC1) + '/' + IntToStr(BaseObject.m_WAbil.AC2);
  EditMAC.Text := IntToStr(BaseObject.m_WAbil.MAC1) + '/' + IntToStr(BaseObject.m_WAbil.MAC2);
  EditDC.Text := IntToStr(BaseObject.m_WAbil.DC1) + '/' + IntToStr(BaseObject.m_WAbil.DC2);
  EditMC.Text := IntToStr(BaseObject.m_WAbil.MC1) + '/' + IntToStr(BaseObject.m_WAbil.MC2);
  EditSC.Text := IntToStr(BaseObject.m_WAbil.SC1) + '/' + IntToStr(BaseObject.m_WAbil.SC2);
  EditHP.Text := IntToStr(BaseObject.m_WAbil.HP) + '/' + IntToStr(BaseObject.m_WAbil.MaxHP);
  EditMP.Text := IntToStr(BaseObject.m_WAbil.MP) + '/' + IntToStr(BaseObject.m_WAbil.MaxMP);

  if (PlayObject <> nil) then
  begin
    EditGameGold.Value := PlayObject.m_nGameGold;
    EditGamePoint.Value := PlayObject.m_nGamePoint;
    EditCreditPoint.Value := PlayObject.m_WAbil.CreditPoint;
    EditGameGlory.Value := PlayObject.m_nGameGlory;
    EditBonusPoint.Value := PlayObject.m_nBonusPoint;
    seGameDiamond.Value := PlayObject.m_nGameDiamond;
    seGameGird.Value := PlayObject.m_nGameGird;

    nTotleUsePoint := PlayObject.m_BonusAbil.DC + PlayObject.m_BonusAbil.MC + PlayObject.m_BonusAbil.SC +
      PlayObject.m_BonusAbil.AC + PlayObject.m_BonusAbil.MAC + PlayObject.m_BonusAbil.HP + PlayObject.m_BonusAbil.MP +
      PlayObject.m_BonusAbil.Hit + PlayObject.m_BonusAbil.Speed + PlayObject.m_BonusAbil.X2;

    EditEditBonusPointUsed.Value := nTotleUsePoint;

    CheckBoxGameMaster.Checked := PlayObject.m_boAdminMode;
    CheckBoxSuperMan.Checked := PlayObject.m_boSuperMan;
    CheckBoxObserver.Checked := PlayObject.m_boObMode;
  end;

  if BaseObject.m_boDeath then
  begin
    EditHumanStatus.Text := '死亡';
  end
  else if BaseObject.m_boGhost then
  begin
    EditHumanStatus.Text := '下线';
    BaseObject := nil;
  end
  else
  begin
    if (PlayObject <> nil) and (PlayObject.m_boOffLine) then
      EditHumanStatus.Text := '离线'
    else
      EditHumanStatus.Text := '在线';
  end;

  if BaseObject = nil then
    Exit;

  for I := Low(SmartObject.m_UseItems) to High(SmartObject.m_UseItems) do
  begin
    UserItem := @SmartObject.m_UseItems[I];
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if StdItem = nil then
    begin
      GridUserItem.Cells[1, I + 1] := '';
      GridUserItem.Cells[2, I + 1] := '';
      GridUserItem.Cells[3, I + 1] := '';
      GridUserItem.Cells[4, I + 1] := '';
      GridUserItem.Cells[5, I + 1] := '';
      GridUserItem.Cells[6, I + 1] := '';
      GridUserItem.Cells[7, I + 1] := '';
      GridUserItem.Cells[8, I + 1] := '';
      GridUserItem.Cells[9, I + 1] := '';
      Continue;
    end;
    Item := StdItem^;
    ItemUnit.GetItemAddValue(UserItem, Item);

    GridUserItem.Cells[1, I + 1] := Item.Name;
    GridUserItem.Cells[2, I + 1] := IntToStr(UserItem.MakeIndex);
    GridUserItem.Cells[3, I + 1] := Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]);
    GridUserItem.Cells[4, I + 1] := Format('%d/%d', [Item.DC1, Item.DC2]);
    GridUserItem.Cells[5, I + 1] := Format('%d/%d', [Item.MC1, Item.MC2]);
    GridUserItem.Cells[6, I + 1] := Format('%d/%d', [Item.SC1, Item.SC2]);
    GridUserItem.Cells[7, I + 1] := Format('%d/%d', [Item.AC1, Item.AC2]);
    GridUserItem.Cells[8, I + 1] := Format('%d/%d', [Item.MAC1, Item.MAC2]);
    GridUserItem.Cells[9, I + 1] := Format('%d/%d/%d/%d/%d/%d/%d', [UserItem.btValue[0], UserItem.btValue[1], UserItem.btValue[2],
      UserItem.btValue[3], UserItem.btValue[4], UserItem.btValue[5], UserItem.btValue[6]]);
  end;

  if SmartObject.m_ItemList.Count <= 0 then
    GridBagItem.RowCount := 2
  else
    GridBagItem.RowCount := SmartObject.m_ItemList.Count + 1;

  for I := 0 to SmartObject.m_ItemList.Count - 1 do
  begin
    UserItem := SmartObject.m_ItemList.Items[I];
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if StdItem = nil then
    begin
      GridBagItem.Cells[1, I + 1] := '';
      GridBagItem.Cells[2, I + 1] := '';
      GridBagItem.Cells[3, I + 1] := '';
      GridBagItem.Cells[4, I + 1] := '';
      GridBagItem.Cells[5, I + 1] := '';
      GridBagItem.Cells[6, I + 1] := '';
      GridBagItem.Cells[7, I + 1] := '';
      GridBagItem.Cells[8, I + 1] := '';
      GridBagItem.Cells[9, I + 1] := '';
      Continue;
    end;
    Item := StdItem^;
    ItemUnit.GetItemAddValue(UserItem, Item);
    GridBagItem.Cells[0, I + 1] := IntToStr(I);
    GridBagItem.Cells[1, I + 1] := Item.Name;
    GridBagItem.Cells[2, I + 1] := IntToStr(UserItem.MakeIndex);
    GridBagItem.Cells[3, I + 1] := Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]);
    GridBagItem.Cells[4, I + 1] := Format('%d/%d', [Item.DC1, Item.DC2]);
    GridBagItem.Cells[5, I + 1] := Format('%d/%d', [Item.MC1, Item.MC2]);
    GridBagItem.Cells[6, I + 1] := Format('%d/%d', [Item.SC1, Item.SC2]);
    GridBagItem.Cells[7, I + 1] := Format('%d/%d', [Item.AC1, Item.AC2]);
    GridBagItem.Cells[8, I + 1] := Format('%d/%d', [Item.MAC1, Item.MAC2]);
    GridBagItem.Cells[9, I + 1] := Format('%d/%d/%d/%d/%d/%d/%d', [UserItem.btValue[0], UserItem.btValue[1], UserItem.btValue[2],
      UserItem.btValue[3], UserItem.btValue[4], UserItem.btValue[5], UserItem.btValue[6]]);
  end;

  if (PlayObject <> nil) and (tbcStorage.TabIndex >= 0) and (tbcStorage.TabIndex < Length(PlayObject.m_StorageItemList)) then
  begin
    if PlayObject.m_StorageItemList[tbcStorage.TabIndex].Count <= 0 then
      GridStorageItem.RowCount := 2
    else
      GridStorageItem.RowCount := PlayObject.m_StorageItemList[tbcStorage.TabIndex].Count + 1;

    for I := 0 to PlayObject.m_StorageItemList[tbcStorage.TabIndex].Count - 1 do
    begin
      UserItem := PlayObject.m_StorageItemList[tbcStorage.TabIndex].Items[I];
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if StdItem = nil then
      begin
        GridStorageItem.Cells[1, I + 1] := '';
        GridStorageItem.Cells[2, I + 1] := '';
        GridStorageItem.Cells[3, I + 1] := '';
        GridStorageItem.Cells[4, I + 1] := '';
        GridStorageItem.Cells[5, I + 1] := '';
        GridStorageItem.Cells[6, I + 1] := '';
        GridStorageItem.Cells[7, I + 1] := '';
        GridStorageItem.Cells[8, I + 1] := '';
        GridStorageItem.Cells[9, I + 1] := '';
        Continue;
      end;
      Item := StdItem^;
      ItemUnit.GetItemAddValue(UserItem, Item);

      GridStorageItem.Cells[0, I + 1] := IntToStr(I);
      GridStorageItem.Cells[1, I + 1] := Item.Name;
      GridStorageItem.Cells[2, I + 1] := IntToStr(UserItem.MakeIndex);
      GridStorageItem.Cells[3, I + 1] := Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]);
      GridStorageItem.Cells[4, I + 1] := Format('%d/%d', [Item.DC1, Item.DC2]);
      GridStorageItem.Cells[5, I + 1] := Format('%d/%d', [Item.MC1, Item.MC2]);
      GridStorageItem.Cells[6, I + 1] := Format('%d/%d', [Item.SC1, Item.SC2]);
      GridStorageItem.Cells[7, I + 1] := Format('%d/%d', [Item.AC1, Item.AC2]);
      GridStorageItem.Cells[8, I + 1] := Format('%d/%d', [Item.MAC1, Item.MAC2]);
      GridStorageItem.Cells[9, I + 1] := Format('%d/%d/%d/%d/%d/%d/%d', [UserItem.btValue[0], UserItem.btValue[1],
        UserItem.btValue[2], UserItem.btValue[3], UserItem.btValue[4], UserItem.btValue[5], UserItem.btValue[6]]);
    end;

    if PlayObject.m_BigStorageItemList.Count <= 0 then
      GridStorageItemEx.RowCount := 2
    else
      GridStorageItemEx.RowCount := PlayObject.m_BigStorageItemList.Count + 1;

    for I := 0 to PlayObject.m_BigStorageItemList.Count - 1 do
    begin
      UserItem := PlayObject.m_BigStorageItemList.Items[I];
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if StdItem = nil then
      begin
        GridStorageItemEx.Cells[1, I + 1] := '';
        GridStorageItemEx.Cells[2, I + 1] := '';
        GridStorageItemEx.Cells[3, I + 1] := '';
        GridStorageItemEx.Cells[4, I + 1] := '';
        GridStorageItemEx.Cells[5, I + 1] := '';
        GridStorageItemEx.Cells[6, I + 1] := '';
        GridStorageItemEx.Cells[7, I + 1] := '';
        GridStorageItemEx.Cells[8, I + 1] := '';
        GridStorageItemEx.Cells[9, I + 1] := '';
        Continue;
      end;
      Item := StdItem^;
      ItemUnit.GetItemAddValue(UserItem, Item);

      GridStorageItemEx.Cells[0, I + 1] := IntToStr(I);
      GridStorageItemEx.Cells[1, I + 1] := Item.Name;
      GridStorageItemEx.Cells[2, I + 1] := IntToStr(UserItem.MakeIndex);
      GridStorageItemEx.Cells[3, I + 1] := Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]);
      GridStorageItemEx.Cells[4, I + 1] := Format('%d/%d', [Item.DC1, Item.DC2]);
      GridStorageItemEx.Cells[5, I + 1] := Format('%d/%d', [Item.MC1, Item.MC2]);
      GridStorageItemEx.Cells[6, I + 1] := Format('%d/%d', [Item.SC1, Item.SC2]);
      GridStorageItemEx.Cells[7, I + 1] := Format('%d/%d', [Item.AC1, Item.AC2]);
      GridStorageItemEx.Cells[8, I + 1] := Format('%d/%d', [Item.MAC1, Item.MAC2]);
      GridStorageItemEx.Cells[9, I + 1] := Format('%d/%d/%d/%d/%d/%d/%d', [UserItem.btValue[0], UserItem.btValue[1],
        UserItem.btValue[2], UserItem.btValue[3], UserItem.btValue[4], UserItem.btValue[5], UserItem.btValue[6]]);
    end;

    strGridVarU.RowCount := Length(PlayObject.m_UVal) + 1;
    for I := Low(PlayObject.m_UVal) to High(PlayObject.m_UVal) do
    begin
      strGridVarU.Cells[0, I + 1] := 'U' + IntToStr(I);
      strGridVarU.Cells[1, I + 1] := IntToStr(PlayObject.m_UVal[I]);
    end;

    strGridVarT.RowCount := Length(PlayObject.m_TVal) + 1;
    for I := Low(PlayObject.m_TVal) to High(PlayObject.m_TVal) do
    begin
      strGridVarT.Cells[0, I + 1] := 'T' + IntToStr(I);
      strGridVarT.Cells[1, I + 1] := PlayObject.m_TVal[I];
    end;

    strGridVarJ.RowCount := Length(PlayObject.m_JVal) + 1;
    for I := Low(PlayObject.m_JVal) to High(PlayObject.m_JVal) do
    begin
      strGridVarJ.Cells[0, I + 1] := 'J' + IntToStr(I);
      strGridVarJ.Cells[1, I + 1] := IntToStr(PlayObject.m_JVal[I]);
    end;

    strGridVarZ.RowCount := Length(PlayObject.m_ZVal) + 1;
    for I := Low(PlayObject.m_ZVal) to High(PlayObject.m_ZVal) do
    begin
      strGridVarZ.Cells[0, I + 1] := 'Z' + IntToStr(I);
      strGridVarZ.Cells[1, I + 1] := PlayObject.m_ZVal[I];
    end;
  end;

  for I := Low(SmartObject.m_JewelryBoxItems) to High(SmartObject.m_JewelryBoxItems) do
  begin
    UserItem := @SmartObject.m_JewelryBoxItems[I];
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);

    GridJewelryBoxItems.Cells[0, I + 1] := IntToStr(I + 1);

    if StdItem = nil then
    begin
      GridJewelryBoxItems.Cells[1, I + 1] := '';
      GridJewelryBoxItems.Cells[2, I + 1] := '';
      GridJewelryBoxItems.Cells[3, I + 1] := '';
      GridJewelryBoxItems.Cells[4, I + 1] := '';
      GridJewelryBoxItems.Cells[5, I + 1] := '';
      GridJewelryBoxItems.Cells[6, I + 1] := '';
      GridJewelryBoxItems.Cells[7, I + 1] := '';
      GridJewelryBoxItems.Cells[8, I + 1] := '';
      GridJewelryBoxItems.Cells[9, I + 1] := '';
      Continue;
    end;
    Item := StdItem^;
    ItemUnit.GetItemAddValue(UserItem, Item);

    GridJewelryBoxItems.Cells[1, I + 1] := Item.Name;
    GridJewelryBoxItems.Cells[2, I + 1] := IntToStr(UserItem.MakeIndex);
    GridJewelryBoxItems.Cells[3, I + 1] := Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]);
    GridJewelryBoxItems.Cells[4, I + 1] := Format('%d/%d', [Item.DC1, Item.DC2]);
    GridJewelryBoxItems.Cells[5, I + 1] := Format('%d/%d', [Item.MC1, Item.MC2]);
    GridJewelryBoxItems.Cells[6, I + 1] := Format('%d/%d', [Item.SC1, Item.SC2]);
    GridJewelryBoxItems.Cells[7, I + 1] := Format('%d/%d', [Item.AC1, Item.AC2]);
    GridJewelryBoxItems.Cells[8, I + 1] := Format('%d/%d', [Item.MAC1, Item.MAC2]);
    GridJewelryBoxItems.Cells[9, I + 1] := Format('%d/%d/%d/%d/%d/%d/%d', [UserItem.btValue[0], UserItem.btValue[1],
      UserItem.btValue[2], UserItem.btValue[3], UserItem.btValue[4], UserItem.btValue[5], UserItem.btValue[6]]);
  end;

  for I := Low(SmartObject.m_GodBlessItems) to High(SmartObject.m_GodBlessItems) do
  begin
    UserItem := @SmartObject.m_GodBlessItems[I];
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);

    GridGodBlessItems.Cells[0, I + 1] := IntToStr(I + 1);

    if StdItem = nil then
    begin
      GridGodBlessItems.Cells[1, I + 1] := '';
      GridGodBlessItems.Cells[2, I + 1] := '';
      GridGodBlessItems.Cells[3, I + 1] := '';
      GridGodBlessItems.Cells[4, I + 1] := '';
      GridGodBlessItems.Cells[5, I + 1] := '';
      GridGodBlessItems.Cells[6, I + 1] := '';
      GridGodBlessItems.Cells[7, I + 1] := '';
      GridGodBlessItems.Cells[8, I + 1] := '';
      GridGodBlessItems.Cells[9, I + 1] := '';
      Continue;
    end;
    Item := StdItem^;
    ItemUnit.GetItemAddValue(UserItem, Item);

    GridGodBlessItems.Cells[1, I + 1] := Item.Name;
    GridGodBlessItems.Cells[2, I + 1] := IntToStr(UserItem.MakeIndex);
    GridGodBlessItems.Cells[3, I + 1] := Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]);
    GridGodBlessItems.Cells[4, I + 1] := Format('%d/%d', [Item.DC1, Item.DC2]);
    GridGodBlessItems.Cells[5, I + 1] := Format('%d/%d', [Item.MC1, Item.MC2]);
    GridGodBlessItems.Cells[6, I + 1] := Format('%d/%d', [Item.SC1, Item.SC2]);
    GridGodBlessItems.Cells[7, I + 1] := Format('%d/%d', [Item.AC1, Item.AC2]);
    GridGodBlessItems.Cells[8, I + 1] := Format('%d/%d', [Item.MAC1, Item.MAC2]);
    GridGodBlessItems.Cells[9, I + 1] := Format('%d/%d/%d/%d/%d/%d/%d', [UserItem.btValue[0], UserItem.btValue[1],
      UserItem.btValue[2], UserItem.btValue[3], UserItem.btValue[4], UserItem.btValue[5], UserItem.btValue[6]]);
  end;
end;

procedure TfrmHumanInfo.TimerTimer(Sender: TObject);
begin
  if BaseObject = nil then
    Exit;
  if BaseObject.m_boGhost then
  begin
    EditHumanStatus.Text := '下线';
    BaseObject := nil;
    Exit;
  end;
  if boRefHuman then
    RefHumanInfo();
end;

procedure TfrmHumanInfo.CheckBoxMonitorClick(Sender: TObject);
begin
  boRefHuman := CheckBoxMonitor.Checked;
  ButtonSave.Enabled := not boRefHuman;
end;

procedure TfrmHumanInfo.ButtonKickClick(Sender: TObject);
var
  PlayObject: TPlayObject;
  HeroObject: THeroObject;
begin
  if BaseObject = nil then
    Exit;
  if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    PlayObject := TPlayObject(BaseObject);
    PlayObject.m_boOffLine := False;
    PlayObject.m_boPlayOffLine := False;
    PlayObject.m_boEmergencyClose := True;

  end
  else
  begin
    HeroObject := THeroObject(BaseObject);
    HeroObject.LogOut;
  end;
  ButtonKick.Enabled := False;
end;

procedure TfrmHumanInfo.ButtonSaveClick(Sender: TObject);
var
  nOLevel: Integer;
  nLevel: LongWord;

  nONGLevel: Integer;
  nNGLevel: Integer;

  nOldBonusPoint: Integer;

  nGold: LongWord;
  nPKPOINT: Integer;
  nGameGold: LongWord;
  nGamePoint: LongWord;
  nCreditPoint: Integer;
  nGameDiamond, nGameGrid: LongWord;
  nBonusPoint: Integer;
  nGameGlory: Integer;
  boGameMaster: Boolean;
  boObServer: Boolean;
  boSuperman: Boolean;
  sAutoSendMsg: string;

  SmartObject: TSmartObject;
  MaxLevel: LongWord;
begin
  if BaseObject = nil then
    Exit;

  if not(BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
    Exit;

  sAutoSendMsg := Trim(EditSayMsg.Text);
  nLevel := EditLevel.Value;
  nGold := EditGold.Value;
  nPKPOINT := EditPKPoint.Value;
  nGameGold := EditGameGold.Value;
  nGamePoint := EditGamePoint.Value;
  nGameDiamond := seGameDiamond.Value;
  nGameGrid := seGameGird.Value;
  nCreditPoint := EditCreditPoint.Value;
  nGameGlory := EditGameGlory.Value;
  nBonusPoint := EditBonusPoint.Value;
  boGameMaster := CheckBoxGameMaster.Checked;
  boObServer := CheckBoxObserver.Checked;
  boSuperman := CheckBoxSuperMan.Checked;

  nNGLevel := EditNGLevel.Value;
  if g_Config.btMaxLevel = 0 then
    MaxLevel := High(Word)
  else if g_Config.btMaxLevel = 1 then
    MaxLevel := High(Integer)
  else
    MaxLevel := High(LongWord);

  if (nLevel > MaxLevel) or (nNGLevel < 0) or (nNGLevel > MAXNG_LEVEL) { or (nGold > 200000000) } or (nPKPOINT < 0) or
    (nPKPOINT > 2000000) or (nCreditPoint < 0) or (nGameGlory < 0) or (nBonusPoint < 0) or (nBonusPoint > 20000000) then
  begin
    MessageBox(Handle, '输入数据不正确！', '错误信息', MB_OK);
    Exit;
  end;

  SmartObject := TPlayObject(BaseObject);
  if (SmartObject.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    if g_boGameLogGold and (SmartObject.m_nGold <> nGold) then
    begin
      AddGameDataLog(LOG_GoldChange, LOG_ActionNone, SmartObject, sSTRING_GOLDNAME, 0, '0', SmartObject.m_nGold, nGold,
        'M2调整 ' + IntToStr(SmartObject.m_nGold) + '->' + IntToStr(nGold));
    end;

    if g_boGameLogGameGold and (TPlayObject(SmartObject).m_nGameGold <> nGameGold) then
    begin
      AddGameDataLog(LOG_GameGoldChange, LOG_ActionNone, SmartObject, g_Config.sGameGoldName, 0, '0', nGameGold,
        TPlayObject(SmartObject).m_nGameGold, 'M2调整 ' + IntToStr(TPlayObject(SmartObject).m_nGameGold) + '->' +
        IntToStr(nGameGold));
    end;

    if g_boGameLogGamePoint and (TPlayObject(SmartObject).m_nGamePoint <> nGamePoint) then
    begin
      AddGameDataLog(LOG_GamePointChange, LOG_ActionNone, SmartObject, g_Config.sGamePointName, 0, '0', nGamePoint,
        TPlayObject(SmartObject).m_nGamePoint, 'M2调整 ' + IntToStr(TPlayObject(SmartObject).m_nGamePoint) + '->' +
        IntToStr(nGamePoint));
    end;

    if (TPlayObject(SmartObject).m_nGameDiamond <> nGameDiamond) then
    begin
      AddGameDataLog(LOG_GameDiamondChange, LOG_ActionNone, SmartObject, g_Config.sGameDiamondName, 0, '0', nGameDiamond,
        TPlayObject(SmartObject).m_nGameDiamond, 'M2调整 ' + IntToStr(TPlayObject(SmartObject).m_nGameDiamond) + '->' +
        IntToStr(nGameDiamond));
    end;

    if (TPlayObject(SmartObject).m_nGameGird <> nGameGrid) then
    begin
      AddGameDataLog(LOG_GameGirdChange, LOG_ActionNone, SmartObject, g_Config.sGameGirdName, 0, '0', nGameGrid,
        TPlayObject(SmartObject).m_nGameGird, 'M2调整 ' + IntToStr(TPlayObject(SmartObject).m_nGameGird) + '->' +
        IntToStr(nGameGrid));
    end;

    if TPlayObject(SmartObject).m_nGameGlory <> nGameGlory then
    begin
      AddGameDataLog(LOG_GamegLoryChange, LOG_ActionNone, SmartObject, '荣誉值', 0, '0', nGameGlory,
        TPlayObject(SmartObject).m_nGameGlory, 'M2调整 ' + IntToStr(TPlayObject(SmartObject).m_nGameGlory) + '->' +
        IntToStr(nGameGlory));
    end;

    if SmartObject.m_WAbil.CreditPoint <> nCreditPoint then
    begin
      AddGameDataLog(LOG_CreditPointChange, LOG_ActionNone, SmartObject, g_Config.sCreditPointName, 0, '0', nCreditPoint,
        SmartObject.m_WAbil.CreditPoint, 'M2调整 ' + IntToStr(SmartObject.m_WAbil.CreditPoint) + '->' + IntToStr(nCreditPoint));
    end;

    TPlayObject(SmartObject).m_sAutoSendMsg := sAutoSendMsg;
    SmartObject.m_nGold := nGold;
    if SmartObject.m_nPkPoint <> nPKPOINT then
    begin
      SmartObject.m_nPkPoint := nPKPOINT;
      SmartObject.RefNameColor();
    end;

    nOldBonusPoint := TPlayObject(SmartObject).m_nBonusPoint;
    TPlayObject(SmartObject).m_nGameGold := nGameGold;
    TPlayObject(SmartObject).m_nGamePoint := nGamePoint;
    TPlayObject(SmartObject).m_nGameDiamond := nGameDiamond;
    TPlayObject(SmartObject).m_nGameGird := nGameGrid;
    SmartObject.m_WAbil.CreditPoint := nCreditPoint;
    TPlayObject(SmartObject).m_nGameGlory := nGameGlory;
    TPlayObject(SmartObject).m_nBonusPoint := nBonusPoint;
    SmartObject.m_boAdminMode := boGameMaster;
    SmartObject.m_boObMode := boObServer;
    SmartObject.m_boSuperMan := boSuperman;
    TPlayObject(SmartObject).GameGoldChanged;
    TPlayObject(SmartObject).NewGamePointChanged;
    TPlayObject(SmartObject).SendGameGlory;
    SmartObject.SendMsg(SmartObject, RM_ABILITY, 0, 0, 0, 0, ''); // 声望值变化通知到客户端

    if nOldBonusPoint <> TPlayObject(SmartObject).m_nBonusPoint then
    begin
      AddGameDataLog(LOG_AbilPointChange, LOG_ActionNone, SmartObject, '未分配置属性点', 0, '0', TPlayObject(SmartObject).m_nBonusPoint,
        nOldBonusPoint, 'M2调整 ' + IntToStr(nOldBonusPoint) + '->' + IntToStr(TPlayObject(SmartObject).m_nBonusPoint));
    end;
  end
  else
  begin
    if SmartObject.m_nPkPoint <> nPKPOINT then
    begin
      SmartObject.m_nPkPoint := nPKPOINT;
      SmartObject.RefNameColor();
    end;
  end;

  nOLevel := SmartObject.m_Abil.Level;
  SmartObject.m_Abil.Level := nLevel;
  if nOLevel <> SmartObject.m_Abil.Level then
  begin
    AddGameDataLog(LOG_LevelChange, LOG_ActionNone, SmartObject, '等级', 0, '0', nLevel, nOLevel,
      'M2调整 ' + IntToStr(nOLevel) + '->' + IntToStr(nLevel));
    SmartObject.HasLevelUp(0);
  end;

  if SmartObject.m_boTrainingNG then
  begin
    nONGLevel := SmartObject.m_AbilNG.Level;
    SmartObject.m_AbilNG.Level := nNGLevel;
    if nONGLevel <> SmartObject.m_AbilNG.Level then
    begin
      AddGameDataLog(LOG_LevelChange, LOG_ActionNone, SmartObject, '内功等级', 0, '0', nNGLevel, nONGLevel,
        'M2调整 ' + IntToStr(nONGLevel) + '->' + IntToStr(nNGLevel));

      SmartObject.HasLevelUpNG(0);
    end;
  end;

  MessageBox(Handle, '角色数据已保存。', '提示信息', MB_OK);
end;

procedure TfrmHumanInfo.btnSaveUClick(Sender: TObject);
var
  I: Integer;
begin
  if BaseObject = nil then
    Exit;
  if not(BaseObject.m_btRaceServer in [RC_PLAYOBJECT]) then
    Exit;

  for I := Low(TPlayObject(BaseObject).m_UVal) to High(TPlayObject(BaseObject).m_UVal) do
  begin
    if FVarUChanged[I] then
    begin
      TPlayObject(BaseObject).m_UVal[I] := StrToIntDef(strGridVarU.Cells[1, I + 1], 0);
    end;
  end;

  FillChar(FVarUChanged, SizeOf(FVarUChanged), 0);
  btnSaveU.Enabled := False;
end;

procedure TfrmHumanInfo.btnSaveZClick(Sender: TObject);
var
  I: Integer;
begin
  if BaseObject = nil then
    Exit;
  if not(BaseObject.m_btRaceServer in [RC_PLAYOBJECT]) then
    Exit;

  for I := Low(TPlayObject(BaseObject).m_ZVal) to High(TPlayObject(BaseObject).m_ZVal) do
  begin
    if FVarZChanged[I] then
    begin
      TPlayObject(BaseObject).m_ZVal[I] := strGridVarZ.Cells[1, I + 1];
    end;
  end;
  FillChar(FVarZChanged, SizeOf(FVarZChanged), 0);
  btnSaveZ.Enabled := False;
end;

procedure TfrmHumanInfo.btnSaveJClick(Sender: TObject);
var
  I: Integer;
begin
  if BaseObject = nil then
    Exit;
  if not(BaseObject.m_btRaceServer in [RC_PLAYOBJECT]) then
    Exit;

  for I := Low(TPlayObject(BaseObject).m_JVal) to High(TPlayObject(BaseObject).m_JVal) do
  begin
    if FVarJChanged[I] then
    begin
      TPlayObject(BaseObject).m_JVal[I] := StrToIntDef(strGridVarJ.Cells[1, I + 1], 0);
    end;
  end;
  FillChar(FVarJChanged, SizeOf(FVarJChanged), 0);
  btnSaveJ.Enabled := False;
end;

procedure TfrmHumanInfo.btnSaveTClick(Sender: TObject);
var
  I: Integer;
begin
  if BaseObject = nil then
    Exit;
  if not(BaseObject.m_btRaceServer in [RC_PLAYOBJECT]) then
    Exit;

  for I := Low(TPlayObject(BaseObject).m_TVal) to High(TPlayObject(BaseObject).m_TVal) do
  begin
    if FVarTChanged[I] then
    begin
      TPlayObject(BaseObject).m_TVal[I] := strGridVarT.Cells[1, I + 1];
    end;
  end;
  FillChar(FVarTChanged, SizeOf(FVarTChanged), 0);
  btnSaveT.Enabled := False;
end;

procedure TfrmHumanInfo.strGridVarUSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: String);
var
  Index: Integer;
begin
  if ACol = 1 then
  begin
    Index := ARow - 1;
    if (Index >= Low(FVarUChanged)) and (Index <= High(FVarUChanged)) then
    begin
      FVarUChanged[Index] := True;
      btnSaveU.Enabled := True;
    end;
  end;
end;

procedure TfrmHumanInfo.strGridVarZSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
var
  Index: Integer;
begin
  if ACol = 1 then
  begin
    Index := ARow - 1;
    if (Index >= Low(FVarZChanged)) and (Index <= High(FVarZChanged)) then
    begin
      FVarZChanged[Index] := True;
      btnSaveZ.Enabled := True;
    end;
  end;
end;

procedure TfrmHumanInfo.strGridVarJSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: string);
var
  Index: Integer;
begin
  if ACol = 1 then
  begin
    Index := ARow - 1;
    if (Index >= Low(FVarJChanged)) and (Index <= High(FVarJChanged)) then
    begin
      FVarJChanged[Index] := True;
      btnSaveJ.Enabled := True;
    end;
  end;
end;

procedure TfrmHumanInfo.strGridVarTSetEditText(Sender: TObject; ACol, ARow: Integer; const Value: String);
var
  Index: Integer;
begin
  if ACol = 1 then
  begin
    Index := ARow - 1;
    if (Index >= Low(FVarTChanged)) and (Index <= High(FVarTChanged)) then
    begin
      FVarTChanged[Index] := True;
      btnSaveT.Enabled := True;
    end;
  end;
end;

end.

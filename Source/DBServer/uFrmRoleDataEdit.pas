unit uFrmRoleDataEdit;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, Grobal2, ComCtrls, StdCtrls, Spin, DBShare, SpinEditEx,
  Grids, SqliteRoleDB;

type
  TFrmRoleDataEdit = class(TForm)
    PageControl: TPageControl;
    tsBase: TTabSheet;
    tsInfo: TTabSheet;
    tsMagic: TTabSheet;
    tsUserItem: TTabSheet;
    tsSorage: TTabSheet;
    ButtonSaveData: TButton;
    ButtonExportData: TButton;
    ButtonImportData: TButton;
    SaveDialog: TSaveDialog;
    OpenDialog: TOpenDialog;
    tsVarU: TTabSheet;
    tsVarT: TTabSheet;
    strGridVarU: TStringGrid;
    strGridVarT: TStringGrid;
    lbl11111: TLabel;
    lbl2: TLabel;
    lbl3: TLabel;
    lbl4: TLabel;
    lbl5: TLabel;
    lbl6: TLabel;
    lbl1: TLabel;
    lbl7: TLabel;
    lbl8: TLabel;
    lbl9: TLabel;
    lbl10: TLabel;
    edtChrName: TEdit;
    edtAccount: TEdit;
    edtPassword: TEdit;
    edtDearName: TEdit;
    edtMasterName: TEdit;
    edtID: TEdit;
    edtCurMap: TEdit;
    seCurX: TSpinEditEx;
    seCurY: TSpinEditEx;
    edtHomeMap: TEdit;
    seHomeX: TSpinEditEx;
    seHomeY: TSpinEditEx;
    chkIsMaster: TCheckBox;
    lbl11: TLabel;
    lbl12: TLabel;
    lbl13: TLabel;
    lbl14: TLabel;
    lbl18: TLabel;
    lbl17: TLabel;
    lbl19: TLabel;
    lbl20: TLabel;
    lbl15: TLabel;
    lbl16: TLabel;
    seLevel: TSpinEditEx;
    seGold: TSpinEditLongWord;
    seGameGold: TSpinEditLongWord;
    seGamePoint: TSpinEditEx;
    seCreditPoint: TSpinEditEx;
    sePayPoint: TSpinEditEx;
    sePKPoint: TSpinEditEx;
    seContribution: TSpinEditEx;
    GroupBox6: TGroupBox;
    lbl22: TLabel;
    lbl23: TLabel;
    lbl24: TLabel;
    lbl25: TLabel;
    lbl26: TLabel;
    lbl27: TLabel;
    lbl28: TLabel;
    lbl29: TLabel;
    lbl30: TLabel;
    lbl31: TLabel;
    EditDC: TSpinEditEx;
    EditMC: TSpinEditEx;
    EditSC: TSpinEditEx;
    EditAC: TSpinEditEx;
    EditMAC: TSpinEditEx;
    EditHP: TSpinEditEx;
    EditMP: TSpinEditEx;
    EditHit: TSpinEditEx;
    EditSpeed: TSpinEditEx;
    EditX2: TSpinEditEx;
    seGameDiamond: TSpinEditLongWord;
    seGameGird: TSpinEditLongWord;
    lvMagic: TListView;
    lvUserItem: TListView;
    lbl21: TLabel;
    seBonusPoint: TSpinEditEx;
    lvStorage: TListView;
    tsFenghao: TTabSheet;
    lvFenghaoItem: TListView;
    procedure ButtonExportDataClick(Sender: TObject);
    procedure edtPasswordChange(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure ButtonSaveDataClick(Sender: TObject);
  private
    FIsHuman: Boolean;
    FHumData: THumData;
    FID: Integer;
    FHeroData: THeroData;

    procedure DoOpen;

    procedure RefreshShow();
    procedure RefreshBaseInfo();
    procedure RefreshMagicInfo();
    procedure RefreshUserItems();
    procedure RefreshFenghaoItems();
    procedure RefreshStorages();
    procedure RefreshUserVar();
    procedure ProcessSaveDataToFile();
    procedure ProcessLoadDataformFile();
    { Private declarations }
  public
    { Public declarations }
  end;

  procedure ShowFrmRoleDataEdit(ID: Integer; HumData: PTHumData; HeroData: PTHeroData);

implementation

{$R *.dfm}

const
  TItemWhereNames: array[Low(THumanUseItems)..High(THumanUseItems)] of string = (
    '衣服',
    '武器',
    '照明物',
    '项链',
    '头盔',
    '左手镯',
    '右手镯',
    '左戒指',
    '右戒指',
    '符',
    '腰带',
    '鞋子',
    '宝石',
    '斗笠',
    '军鼓',
    '马牌',
    '盾牌',
    '灵玉',
    '时装衣服',
    '时装武器',
    '时装项链',
    '时装头盔',
    '时装左手镯',
    '时装右手镯',
    '时装左戒指',
    '时装右戒指',
    '时装照明物',
    '时装腰带',
    '时装鞋子',
    '时装宝石');

procedure ShowFrmRoleDataEdit(ID: Integer; HumData: PTHumData; HeroData: PTHeroData);
var
  FrmRoleDataEdit: TFrmRoleDataEdit;
begin
  FrmRoleDataEdit := TFrmRoleDataEdit.Create(nil);
  try
    FrmRoleDataEdit.FID := ID;
    FrmRoleDataEdit.edtID.Text := IntToStr(ID);
    if HumData <> nil then
    begin
      FrmRoleDataEdit.FIsHuman := True;
      FrmRoleDataEdit.FHumData := HumData^;
    end
    else
    begin
      FrmRoleDataEdit.FIsHuman := False;
      FrmRoleDataEdit.FHeroData := HeroData^;
    end;
    FrmRoleDataEdit.DoOpen;
    FrmRoleDataEdit.ShowModal;
  finally
    FrmRoleDataEdit.Free;
  end;
end;

procedure TFrmRoleDataEdit.DoOpen;
var
  I: Integer;
begin
  strGridVarU.Cells[0, 0] := '变量名';
  strGridVarU.Cells[1, 0] := '变量值';

  strGridVarT.Cells[0, 0] := '变量名';
  strGridVarT.Cells[1, 0] := '变量值';

  tsVarU.TabVisible := FIsHuman;
  tsVarT.TabVisible := FIsHuman;

  if FIsHuman then
  begin
    Caption := Format('编辑人物数据 [%s]', [FHumData.sChrName]);

    strGridVarU.RowCount := Length(FHumData.UValues) + 1;
    for I := Low(FHumData.UValues) to High(FHumData.UValues) do
    begin
      strGridVarU.Cells[0, I + 1] := 'U' + IntToStr(I);
    end;

    strGridVarT.RowCount := Length(FHumData.TValues) + 1;
    for I := Low(FHumData.TValues) to High(FHumData.TValues) do
    begin
      strGridVarT.Cells[0, I + 1] := 'T' + IntToStr(I);
    end;
  end
  else
  begin
    Caption := Format('编辑英雄数据 [%s]', [FHeroData.sChrName])
  end;

  RefreshShow();
  PageControl.ActivePageIndex := 0;
end;

procedure TFrmRoleDataEdit.RefreshBaseInfo;
begin
  //------------------------------------------------------------
  edtPassword.Text := '';
  edtDearName.Text := '';
  edtMasterName.Text := '';
  chkIsMaster.Checked := False;

  edtHomeMap.Text := '';
  seHomeX.Value := 0;
  seHomeY.Value := 0;

  seGold.Value := 0;
  seGameGold.Value := 0;
  seGamePoint.Value := 0;
  sePayPoint.Value := 0;
  seCreditPoint.Value := 0;

  seContribution.Value := 0;
  seBonusPoint.Value := 0;
  seGameDiamond.Value := 0;
  seGameGird.Value := 0;

  edtPassword.Enabled := FIsHuman;
  edtDearName.Enabled := FIsHuman;
  edtMasterName.Enabled := FIsHuman;
  chkIsMaster.Enabled := FIsHuman;

  edtHomeMap.Enabled := FIsHuman;
  seHomeX.Enabled := FIsHuman;
  seHomeY.Enabled := FIsHuman;

  seGold.Enabled := FIsHuman;
  seGameGold.Enabled := FIsHuman;
  seGamePoint.Enabled := FIsHuman;
  sePayPoint.Enabled := FIsHuman;
  seCreditPoint.Enabled := FIsHuman;

  seContribution.Enabled := FIsHuman;
  seBonusPoint.Enabled := FIsHuman;
  seGameDiamond.Enabled := FIsHuman;
  seGameGird.Enabled := FIsHuman;

  if FIsHuman then
  begin
    edtChrName.Text := FHumData.sChrName;
    edtAccount.Text := FHumData.sAccount;
    edtPassword.Text := FHumData.sStoragePwd;
    edtDearName.Text := FHumData.sDearName;
    edtMasterName.Text := FHumData.sMasterName;
    chkIsMaster.Checked := FHumData.boMaster;

    edtCurMap.Text := FHumData.sCurMap;
    seCurX.Value := FHumData.wCurX;
    seCurY.Value := FHumData.wCurY;

    edtHomeMap.Text := FHumData.sHomeMap;
    seHomeX.Value := FHumData.wHomeX;
    seHomeY.Value := FHumData.wHomeY;

    seLevel.Value := FHumData.Abil.Level;
    seGold.Value := FHumData.nGold;
    seGameGold.Value := FHumData.nGameGold;
    seGamePoint.Value := FHumData.nGamePoint;
    sePayPoint.Value := FHumData.nPayMentPoint;
    seCreditPoint.Value := FHumData.Abil.CreditPoint;
    sePKPoint.Value := FHumData.nPKPoint;
    seContribution.Value := FHumData.wContribution;

    seBonusPoint.Value := FHumData.nBonusPoint;
    seGameDiamond.Value := FHumData.nGameDiamond;
    seGameGird.Value := FHumData.nGameGird;

    EditDC.Value := FHumData.BonusAbil.DC;
    EditMC.Value := FHumData.BonusAbil.MC;
    EditSC.Value := FHumData.BonusAbil.SC;
    EditAC.Value := FHumData.BonusAbil.AC;
    EditMAC.Value := FHumData.BonusAbil.MAC;
    EditHP.Value := FHumData.BonusAbil.HP;
    EditMP.Value := FHumData.BonusAbil.MP;
    EditHit.Value := FHumData.BonusAbil.Hit;
    EditSpeed.Value := FHumData.BonusAbil.Speed;
    EditX2.Value := FHumData.BonusAbil.X2;
  end
  else
  begin
    edtChrName.Text := FHeroData.sChrName;
    //edtAccount.Text := FHeroData.sAccount;

    edtCurMap.Text := FHeroData.sCurMap;
    seCurX.Value := FHeroData.wCurX;
    seCurY.Value := FHeroData.wCurY;

    seLevel.Value := FHeroData.Abil.Level;
    sePKPoint.Value := FHeroData.nPKPoint;
  end;
end;

procedure TFrmRoleDataEdit.RefreshShow;
begin
  RefreshBaseInfo();
  RefreshMagicInfo();
  RefreshUserItems();
  RefreshFenghaoItems();
  RefreshStorages();

  if FIsHuman then
  begin
    RefreshUserVar;
  end;
end;

procedure TFrmRoleDataEdit.RefreshMagicInfo;
var
  I: Integer;
  ListItem: TListItem;
  MagicInfo: THumMagic;
begin
  lvMagic.Clear;
  if FIsHuman then
  begin
    for I := Low(FHumData.Magics) to High(FHumData.Magics) do
    begin
      MagicInfo := FHumData.Magics[I];
      if MagicInfo.wMagIdx = 0 then break;

      ListItem := lvMagic.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(IntToStr(MagicInfo.wMagIdx));
      ListItem.SubItems.Add(GetMagicName(MagicInfo.wMagIdx, MagicInfo.MagicAttr));
      ListItem.SubItems.Add(IntToStr(MagicInfo.btLevel));
      ListItem.SubItems.Add(IntToStr(MagicInfo.nTranPoint));
      ListItem.SubItems.Add(IntToStr(MagicInfo.btKey));
    end;
  end
  else
  begin
    for I := Low(FHeroData.Magics) to High(FHeroData.Magics) do
    begin
      MagicInfo := FHeroData.Magics[I];
      if MagicInfo.wMagIdx = 0 then break;

      ListItem := lvMagic.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(IntToStr(MagicInfo.wMagIdx));
      ListItem.SubItems.Add(GetMagicName(MagicInfo.wMagIdx, MagicInfo.MagicAttr));
      ListItem.SubItems.Add(IntToStr(MagicInfo.btLevel));
      ListItem.SubItems.Add(IntToStr(MagicInfo.nTranPoint));
      ListItem.SubItems.Add(IntToStr(MagicInfo.btKey));
    end;
  end;
end;

procedure TFrmRoleDataEdit.RefreshUserItems;
var
  I: Integer;
  ListItem: TListItem;
  UserItem: TUserItem;
resourcestring
  sItemValue = '%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d';
  //'%d/%d/%d/%d/%d/%d/%d/%d/%d/%d/%d/%d/%d/%d'
begin
  lvUserItem.Clear;
  if FIsHuman then
  begin
    for I := Low(FHumData.HumItems) to High(FHumData.HumItems) do
    begin
      UserItem := FHumData.HumItems[I];
      if (UserItem.wIndex = 0) or (UserItem.MakeIndex = 0) then Continue;

      ListItem := lvUserItem.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(TItemWhereNames[I]);
      ListItem.SubItems.Add(GetStdItemName(UserItem.wIndex));
      ListItem.SubItems.Add(IntToStr(UserItem.wIndex - 1));
      ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
      ListItem.SubItems.Add(Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]));

      ListItem.SubItems.Add(Format(sItemValue, [
        UserItem.btValue[0],
          UserItem.btValue[1],
          UserItem.btValue[2],
          UserItem.btValue[3],
          UserItem.btValue[4],
          UserItem.btValue[5],
          UserItem.btValue[6],
          UserItem.btValue[7],
          UserItem.btValue[8],
          UserItem.btValue[9],
          UserItem.btValue[10],
          UserItem.btValue[11],
          UserItem.btValue[12],
          UserItem.btValue[13]
          ]));

    end;

    for I := Low(FHumData.JewelryBoxItems) to High(FHumData.JewelryBoxItems) do
    begin
      UserItem := FHumData.JewelryBoxItems[I];
      if (UserItem.wIndex = 0) or (UserItem.MakeIndex = 0) then Continue;

      ListItem := lvUserItem.Items.Add;
      ListItem.Caption := IntToStr(I + Length(FHumData.HumItems));
      ListItem.SubItems.Add('首饰盒' + IntToStr(I + 1));
      ListItem.SubItems.Add(GetStdItemName(UserItem.wIndex));
      ListItem.SubItems.Add(IntToStr(UserItem.wIndex - 1));
      ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
      ListItem.SubItems.Add(Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]));

      ListItem.SubItems.Add(Format(sItemValue, [
        UserItem.btValue[0],
          UserItem.btValue[1],
          UserItem.btValue[2],
          UserItem.btValue[3],
          UserItem.btValue[4],
          UserItem.btValue[5],
          UserItem.btValue[6],
          UserItem.btValue[7],
          UserItem.btValue[8],
          UserItem.btValue[9],
          UserItem.btValue[10],
          UserItem.btValue[11],
          UserItem.btValue[12],
          UserItem.btValue[13]
          ]));
    end;

    for I := Low(FHumData.GodBlessItems) to High(FHumData.GodBlessItems) do
    begin
      UserItem := FHumData.GodBlessItems[I];
      if (UserItem.wIndex = 0) or (UserItem.MakeIndex = 0) then Continue;

      ListItem := lvUserItem.Items.Add;
      ListItem.Caption := IntToStr(I + Length(FHumData.HumItems) + Length(FHumData.JewelryBoxItems));
      ListItem.SubItems.Add('神佑盒' + IntToStr(I + 1));
      ListItem.SubItems.Add(GetStdItemName(UserItem.wIndex));
      ListItem.SubItems.Add(IntToStr(UserItem.wIndex - 1));
      ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
      ListItem.SubItems.Add(Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]));

      ListItem.SubItems.Add(Format(sItemValue, [
        UserItem.btValue[0],
          UserItem.btValue[1],
          UserItem.btValue[2],
          UserItem.btValue[3],
          UserItem.btValue[4],
          UserItem.btValue[5],
          UserItem.btValue[6],
          UserItem.btValue[7],
          UserItem.btValue[8],
          UserItem.btValue[9],
          UserItem.btValue[10],
          UserItem.btValue[11],
          UserItem.btValue[12],
          UserItem.btValue[13]
          ]));
    end;
  end
  else
  begin
    for I := Low(FHeroData.HumItems) to High(FHeroData.HumItems) do
    begin
      UserItem := FHeroData.HumItems[I];

      if (UserItem.wIndex = 0) or (UserItem.MakeIndex = 0) then Continue;

      ListItem := lvUserItem.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(TItemWhereNames[I]);
      ListItem.SubItems.Add(GetStdItemName(UserItem.wIndex));
      ListItem.SubItems.Add(IntToStr(UserItem.wIndex - 1));
      ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
      ListItem.SubItems.Add(Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]));

      ListItem.SubItems.Add(Format(sItemValue, [
        UserItem.btValue[0],
          UserItem.btValue[1],
          UserItem.btValue[2],
          UserItem.btValue[3],
          UserItem.btValue[4],
          UserItem.btValue[5],
          UserItem.btValue[6],
          UserItem.btValue[7],
          UserItem.btValue[8],
          UserItem.btValue[9],
          UserItem.btValue[10],
          UserItem.btValue[11],
          UserItem.btValue[12],
          UserItem.btValue[13]
          ]));
    end;

    for I := Low(FHeroData.JewelryBoxItems) to High(FHeroData.JewelryBoxItems) do
    begin
      UserItem := FHeroData.JewelryBoxItems[I];
      if (UserItem.wIndex = 0) or (UserItem.MakeIndex = 0) then Continue;

      ListItem := lvUserItem.Items.Add;
      ListItem.Caption := IntToStr(I + Length(FHeroData.HumItems));
      ListItem.SubItems.Add('首饰盒' + IntToStr(I + 1));
      ListItem.SubItems.Add(GetStdItemName(UserItem.wIndex));
      ListItem.SubItems.Add(IntToStr(UserItem.wIndex - 1));
      ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
      ListItem.SubItems.Add(Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]));

      ListItem.SubItems.Add(Format(sItemValue, [
        UserItem.btValue[0],
          UserItem.btValue[1],
          UserItem.btValue[2],
          UserItem.btValue[3],
          UserItem.btValue[4],
          UserItem.btValue[5],
          UserItem.btValue[6],
          UserItem.btValue[7],
          UserItem.btValue[8],
          UserItem.btValue[9],
          UserItem.btValue[10],
          UserItem.btValue[11],
          UserItem.btValue[12],
          UserItem.btValue[13]
          ]));
    end;

    for I := Low(FHeroData.GodBlessItems) to High(FHeroData.GodBlessItems) do
    begin
      UserItem := FHeroData.GodBlessItems[I];
      if (UserItem.wIndex = 0) or (UserItem.MakeIndex = 0) then Continue;

      ListItem := lvUserItem.Items.Add;
      ListItem.Caption := IntToStr(I + Length(FHeroData.HumItems) + Length(FHeroData.JewelryBoxItems));
      ListItem.SubItems.Add('神佑盒' + IntToStr(I + 1));
      ListItem.SubItems.Add(GetStdItemName(UserItem.wIndex));
      ListItem.SubItems.Add(IntToStr(UserItem.wIndex - 1));
      ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
      ListItem.SubItems.Add(Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]));

      ListItem.SubItems.Add(Format(sItemValue, [
        UserItem.btValue[0],
          UserItem.btValue[1],
          UserItem.btValue[2],
          UserItem.btValue[3],
          UserItem.btValue[4],
          UserItem.btValue[5],
          UserItem.btValue[6],
          UserItem.btValue[7],
          UserItem.btValue[8],
          UserItem.btValue[9],
          UserItem.btValue[10],
          UserItem.btValue[11],
          UserItem.btValue[12],
          UserItem.btValue[13]
          ]));
    end;
  end;
end;

procedure TFrmRoleDataEdit.RefreshFenghaoItems;
var
  I: Integer;
  ListItem: TListItem;
  UserItem: TUserItem;
resourcestring
  sItemValue = '%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d';
begin
  lvFenghaoItem.Clear;
  if FIsHuman then
  begin
    for I := Low(FHumData.FengHaoItems) to High(FHumData.FengHaoItems) do
    begin
      UserItem := FHumData.FengHaoItems[I];

      if (UserItem.wIndex = 0) or (UserItem.MakeIndex = 0) then Continue;

      ListItem := lvFenghaoItem.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(GetStdItemName(UserItem.wIndex));
      ListItem.SubItems.Add(IntToStr(UserItem.wIndex - 1));
      ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
      ListItem.SubItems.Add(Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]));

      ListItem.SubItems.Add(Format(sItemValue, [
        UserItem.btValue[0],
          UserItem.btValue[1],
          UserItem.btValue[2],
          UserItem.btValue[3],
          UserItem.btValue[4],
          UserItem.btValue[5],
          UserItem.btValue[6],
          UserItem.btValue[7],
          UserItem.btValue[8],
          UserItem.btValue[9],
          UserItem.btValue[10],
          UserItem.btValue[11],
          UserItem.btValue[12],
          UserItem.btValue[13]
          ]));
    end;
  end
  else
  begin
    for I := Low(FHeroData.FengHaoItems) to High(FHeroData.FengHaoItems) do
    begin
      UserItem := FHeroData.FengHaoItems[I];

      if (UserItem.wIndex = 0) or (UserItem.MakeIndex = 0) then Continue;

      ListItem := lvFenghaoItem.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.Add(GetStdItemName(UserItem.wIndex));
      ListItem.SubItems.Add(IntToStr(UserItem.wIndex - 1));
      ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
      ListItem.SubItems.Add(Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]));

      ListItem.SubItems.Add(Format(sItemValue, [
        UserItem.btValue[0],
          UserItem.btValue[1],
          UserItem.btValue[2],
          UserItem.btValue[3],
          UserItem.btValue[4],
          UserItem.btValue[5],
          UserItem.btValue[6],
          UserItem.btValue[7],
          UserItem.btValue[8],
          UserItem.btValue[9],
          UserItem.btValue[10],
          UserItem.btValue[11],
          UserItem.btValue[12],
          UserItem.btValue[13]
          ]));
    end;
  end;
end;

procedure TFrmRoleDataEdit.RefreshStorages;
var
  I: Integer;
  ListItem: TListItem;
  UserItem: TUserItem;
resourcestring
  sItemValue = '%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d';
begin
  lvStorage.Clear;

  for I := Low(FHumData.StorageItems) to High(FHumData.StorageItems) do
  begin
    UserItem := FHumData.StorageItems[I];

    if (UserItem.wIndex = 0) or (UserItem.MakeIndex = 0) then Continue;

    ListItem := lvStorage.Items.Add;
    ListItem.Caption := IntToStr(I);
    ListItem.SubItems.Add(GetStdItemName(UserItem.wIndex));
    ListItem.SubItems.Add(IntToStr(UserItem.wIndex - 1));
    ListItem.SubItems.Add(IntToStr(UserItem.MakeIndex));
    ListItem.SubItems.Add(Format('%d/%d', [UserItem.Dura, UserItem.DuraMax]));

    ListItem.SubItems.Add(Format(sItemValue, [
      UserItem.btValue[0],
        UserItem.btValue[1],
        UserItem.btValue[2],
        UserItem.btValue[3],
        UserItem.btValue[4],
        UserItem.btValue[5],
        UserItem.btValue[6],
        UserItem.btValue[7],
        UserItem.btValue[8],
        UserItem.btValue[9],
        UserItem.btValue[10],
        UserItem.btValue[11],
        UserItem.btValue[12],
        UserItem.btValue[13]
        ]));
  end;
end;

procedure TFrmRoleDataEdit.RefreshUserVar();
var
  I: Integer;
  S: string;
begin
  for I := Low(FHumData.UValues) to High(FHumData.UValues) do
  begin
    strGridVarU.Cells[1, I + 1] := IntToStr(FHumData.UValues[I]);
  end;

  for I := Low(FHumData.TValues) to High(FHumData.TValues) do
  begin
    S := FHumData.TValues[I];
    strGridVarT.Cells[1, I + 1] := S;
  end;
end;

procedure TFrmRoleDataEdit.ButtonExportDataClick(Sender: TObject);
begin
  if Sender = ButtonExportData then
  begin
    ProcessSaveDataToFile();
  end
  else if Sender = ButtonImportData then
  begin
    ProcessLoadDataformFile();
  end
  else if Sender = ButtonSaveData then
  begin

  end;
end;

procedure TFrmRoleDataEdit.ProcessSaveDataToFile;
var
  sSaveFileName: string;
  nFileHandle: Integer;
begin
  if FIsHuman then
    SaveDialog.FileName := FHumData.sChrName
  else
    SaveDialog.FileName := FHeroData.sChrName;

  SaveDialog.InitialDir := '.\';
  if not SaveDialog.Execute then Exit;
  sSaveFileName := SaveDialog.FileName;
  if FileExists(sSaveFileName) then
    nFileHandle := FileOpen(sSaveFileName, fmOpenReadWrite or fmShareDenyNone)
  else
    nFileHandle := FileCreate(sSaveFileName);

  if nFileHandle <= 0 then
  begin
    MessageBox(Handle, '保存文件出现错误！！！', '错误信息', MB_OK + MB_ICONEXCLAMATION);
    Exit;
  end;

  if FIsHuman then
    FileWrite(nFileHandle, FHumData, SizeOf(THumData))
  else
    FileWrite(nFileHandle, FHeroData, SizeOf(THeroData));

  FileClose(nFileHandle);
  MessageBox(Handle, '角色数据导出成功！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
end;

procedure TFrmRoleDataEdit.ProcessLoadDataformFile;
var
  sLoadFileName: string;
  nFileHandle: Integer;
  ReadBuf: PChar;
begin
  if FIsHuman then
    OpenDialog.FileName := FHumData.sChrName
  else
    OpenDialog.FileName := FHeroData.sChrName;

  OpenDialog.InitialDir := '.\';
  if not OpenDialog.Execute then Exit;
  sLoadFileName := OpenDialog.FileName;

  if not FileExists(sLoadFileName) then
  begin
    MessageBox(Handle, '指定的文件未找到！！！', '错误信息', MB_OK + MB_ICONEXCLAMATION);
    Exit;
  end;

  nFileHandle := FileOpen(sLoadFileName, fmOpenReadWrite or fmShareDenyNone);
  if nFileHandle <= 0 then
  begin
    MessageBox(Handle, '打开文件出现错误！！！', '错误信息', MB_OK + MB_ICONEXCLAMATION);
    Exit;
  end;


  if FIsHuman then
  begin
    GetMem(ReadBuf, SizeOf(THumData));
    try
      if not FileRead(nFileHandle, ReadBuf^, SizeOf(THumData)) = SizeOf(THumData) then
      begin
        MessageBox(Handle, '读取文件出现错误！！！'#13#13'文件格式可能不正确', '错误信息', MB_OK + MB_ICONEXCLAMATION);
        Exit;
      end;

      pTHumData(ReadBuf).sAccount := FHumData.sAccount;
      pTHumData(ReadBuf).sChrName := FHumData.sChrName;
      pTHumData(ReadBuf).sDearName := FHumData.sDearName;
      pTHumData(ReadBuf).sHeroName := FHumData.sHeroName;
      pTHumData(ReadBuf).sDeputyHeroName := FHumData.sDeputyHeroName;

      FHumData := pTHumData(ReadBuf)^;
    finally
      FreeMem(ReadBuf);
    end;
  end
  else
  begin
    GetMem(ReadBuf, SizeOf(THumData));
    try
      if not FileRead(nFileHandle, ReadBuf^, SizeOf(THeroData)) = SizeOf(THeroData) then
      begin
        MessageBox(Handle, '读取文件出现错误！！！'#13#13'文件格式可能不正确', '错误信息', MB_OK + MB_ICONEXCLAMATION);
        Exit;
      end;

      PTHeroData(ReadBuf).sAccount := FHeroData.sAccount;
      PTHeroData(ReadBuf).sChrName := FHeroData.sChrName;
      FHeroData := PTHeroData(ReadBuf)^;
    finally
      FreeMem(ReadBuf);
    end;
  end;

  FileClose(nFileHandle);
  RefreshShow();
  MessageBox(Handle, '角色数据导入成功！！！', '提示信息', MB_OK + MB_ICONINFORMATION);
end;

procedure TFrmRoleDataEdit.edtPasswordChange(Sender: TObject);
begin
  if Sender = edtPassword then
  begin
    FHumData.sStoragePwd := Trim(edtPassword.Text);
  end
  else if Sender = edtDearName then
  begin
    FHumData.sDearName := Trim(edtDearName.Text);
  end
  else if Sender = edtMasterName then
  begin
    FHumData.sMasterName := Trim(edtMasterName.Text);
  end
  else if Sender = chkIsMaster then
  begin
    FHumData.boMaster := chkIsMaster.Checked;
  end
  else if Sender = edtCurMap then
  begin
    FHumData.sCurMap := Trim(edtCurMap.Text);
  end
  else if Sender = seCurX then
  begin
    FHumData.wCurX := seCurX.Value;
  end
  else if Sender = seCurY then
  begin
    FHumData.wCurY := seCurY.Value;
  end
  else if Sender = edtHomeMap then
  begin
    FHumData.sHomeMap := Trim(edtHomeMap.Text);
  end
  else if Sender = seHomeX then
  begin
    FHumData.wHomeX := seHomeX.Value;
  end
  else if Sender = seCurY then
  begin
    FHumData.wHomeY := seHomeY.Value;
  end
  else if Sender = seLevel then
  begin
    if FIsHuman then
      FHumData.Abil.Level := seLevel.Value
    else
      FHeroData.Abil.Level := seLevel.Value;
  end
  else if Sender = seGold then
  begin
    FHumData.nGold := seGold.Value;
  end
  else if Sender = seGameGold then
  begin
    FHumData.nGameGold := seGameGold.Value;
  end
  else if Sender = seGamePoint then
  begin
    FHumData.nGamePoint := seGamePoint.Value;
  end
  else if Sender = sePayPoint then
  begin
    FHumData.nPayMentPoint := sePayPoint.Value;
  end
  else if Sender = seCreditPoint then
  begin
    FHumData.Abil.CreditPoint := seCreditPoint.Value;
  end
  else if Sender = sePKPoint then
  begin
    if FIsHuman then
      FHumData.nPKPoint := sePKPoint.Value
    else
      FHeroData.nPKPoint := sePKPoint.Value;
  end
  else if Sender = seContribution then
  begin
    FHumData.wContribution := seContribution.Value;
  end
  else if Sender = seBonusPoint then
  begin
    FHumData.nBonusPoint := seBonusPoint.Value;
  end
  else if Sender = seGameDiamond then
  begin
    FHumData.nGameDiamond := seGameDiamond.Value
  end
  else if Sender = seGameGird then
  begin
    FHumData.nGameGird := seGameGird.Value;
  end;
end;

procedure TFrmRoleDataEdit.FormCreate(Sender: TObject);
begin
  seLevel.MaxValue := High(Word);
end;

procedure TFrmRoleDataEdit.ButtonSaveDataClick(Sender: TObject);
var
  I: Integer;
  IsOK: Boolean;
begin
  if FIsHuman then
  begin
    for I := Low(FHumData.UValues) to High(FHumData.UValues) do
    begin
      FHumData.UValues[I] := StrToIntDef(strGridVarU.Cells[1, I + 1], 0);
    end;

    for I := Low(FHumData.TValues) to High(FHumData.TValues) do
    begin
      FHumData.TValues[I] := strGridVarT.Cells[1, I + 1];
    end;
  end;

  if FIsHuman then
  begin
    IsOK := g_RoleDB.HumanDB.Save(FID, @FHumData);
  end
  else
  begin
    IsOK := g_RoleDB.HeroDB.Save(FID, @FHeroData);
  end;

  if IsOK then
    MessageBox(Handle, '角色数据保存成功！！！', '提示信息', MB_OK + MB_ICONINFORMATION)
  else
    MessageBox(Handle, '角色数据保存失败！！！', '错误信息', MB_OK + MB_ICONEXCLAMATION);
end;

end.

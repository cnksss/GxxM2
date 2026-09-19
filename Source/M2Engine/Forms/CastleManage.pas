unit CastleManage;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ComCtrls, StdCtrls, Guild, Castle, ExtCtrls,
  SpinEditEx, Vcl.Samples.Spin;

type
  TfrmCastleManage = class(TForm)
    GroupBox1: TGroupBox;
    ListViewCastle: TListView;
    GroupBox2: TGroupBox;
    PageControlCastle: TPageControl;
    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    GroupBox3: TGroupBox;
    Label2: TLabel;
    EditOwenGuildName: TEdit;
    GroupBox4: TGroupBox;
    Label4: TLabel;
    Label5: TLabel;
    Label6: TLabel;
    EditCastleName: TEdit;
    EditCastleOfGuild: TEdit;
    EditHomeMap: TEdit;
    Label1: TLabel;
    Label3: TLabel;
    EditTotalGold: TSpinEditEx;
    EditTodayIncome: TSpinEditEx;
    Label7: TLabel;
    EditTechLevel: TSpinEditEx;
    Label8: TLabel;
    EditPower: TSpinEditEx;
    TabSheet3: TTabSheet;
    GroupBox5: TGroupBox;
    ListViewGuard: TListView;
    ButtonRefresh: TButton;
    TabSheet4: TTabSheet;
    GroupBox6: TGroupBox;
    ListViewAttackSabukWall: TListView;
    ButtonAttackAdd: TButton;
    ButtonAttackEdit: TButton;
    ButtonAttackDel: TButton;
    ButtonRefAttackSabukWall: TButton;
    Label9: TLabel;
    Label10: TLabel;
    EditTunnelMap: TEdit;
    Label11: TLabel;
    EditPalace: TEdit;
    SpinEditNomeX: TSpinEditEx;
    SpinEditNomeY: TSpinEditEx;
    ButtonSave: TButton;
    ButtonStartWar: TButton;
    ButtonStopWar: TButton;
    Label12: TLabel;
    EditWarStatus: TEdit;
    Timer1: TTimer;
    procedure ListViewCastleClick(Sender: TObject);
    procedure ButtonRefreshClick(Sender: TObject);
    procedure ButtonAttackAddClick(Sender: TObject);
    procedure ButtonAttackEditClick(Sender: TObject);
    procedure ListViewAttackSabukWallClick(Sender: TObject);
    procedure ButtonRefAttackSabukWallClick(Sender: TObject);
    procedure ButtonAttackDelClick(Sender: TObject);
    procedure ButtonSaveClick(Sender: TObject);
    procedure ButtonStartWarClick(Sender: TObject);
    procedure ButtonStopWarClick(Sender: TObject);
    procedure Timer1Timer(Sender: TObject);
  private
    procedure RefCastleList;
    procedure RefCastleInfo;

    { Private declarations }
  public
    procedure Open();
    procedure RefCastleAttackSabukWall;
    { Public declarations }
  end;

var
  frmCastleManage: TfrmCastleManage;
  SelAttackGuildInfo: pTAttackerInfo;
  CurCastle: TUserCastle;

implementation

uses
  AttackSabukWallConfig, M2Share, ObjMon2;

{$R *.dfm}
var
  boRefing: Boolean;

  { TfrmCastleManage }

procedure TfrmCastleManage.Open;
begin
  // nCount := 0;
  ButtonSave.Enabled := True;
  PageControlCastle.ActivePageIndex := 0;
  SelAttackGuildInfo := nil;
  RefCastleList();
  Timer1.Enabled := True;
  ShowModal;
end;

procedure TfrmCastleManage.RefCastleInfo;
var
  I, II: Integer;
  ListItem: TListItem;
  ObjUnit: pTObjUnit;
  CastleDoor: TCastleDoor;
begin
  if CurCastle = nil then
    Exit;
  boRefing := True;
  if CurCastle.m_MasterGuild = nil then
    EditOwenGuildName.Text := ''
  else
    EditOwenGuildName.Text := CurCastle.m_MasterGuild.sGuildName;
  EditTotalGold.Value := CurCastle.m_nTotalGold;
  EditTodayIncome.Value := CurCastle.m_nTodayIncome;
  EditTechLevel.Value := CurCastle.m_nTechLevel;
  EditPower.Value := CurCastle.m_nPower;
  ListViewGuard.Clear;
  ListItem := ListViewGuard.Items.Add;
  ListItem.Caption := '0';
  if CurCastle.m_MainDoor.BaseObj <> nil then
  begin
    CastleDoor := TCastleDoor(CurCastle.m_MainDoor.BaseObj);

    ListItem.SubItems.Add(CurCastle.m_MainDoor.BaseObj.m_sCharName);
    ListItem.SubItems.Add(Format('%d:%d', [CurCastle.m_MainDoor.BaseObj.m_nCurrX, CurCastle.m_MainDoor.BaseObj.m_nCurrY]));
    ListItem.SubItems.Add(Format('%d/%d', [CurCastle.m_MainDoor.BaseObj.m_WAbil.HP, CurCastle.m_MainDoor.BaseObj.m_WAbil.MaxHP]));
    if CastleDoor.m_boDeath then
    begin
      ListItem.SubItems.Add('损坏');
    end
    else if CastleDoor.m_boOpened then
    begin
      ListItem.SubItems.Add('开启');
    end
    else
    begin
      ListItem.SubItems.Add('关闭');
    end;
  end
  else
  begin
    ListItem.SubItems.Add(CurCastle.m_MainDoor.sName);
    ListItem.SubItems.Add(Format('%d:%d', [CurCastle.m_MainDoor.nX, CurCastle.m_MainDoor.nY]));
    ListItem.SubItems.Add(Format('%d/%d', [0, 0]));
  end;

  ListItem := ListViewGuard.Items.Add;
  ListItem.Caption := '1';
  if CurCastle.m_LeftWall.BaseObj <> nil then
  begin
    ListItem.SubItems.Add(CurCastle.m_LeftWall.BaseObj.m_sCharName);
    ListItem.SubItems.Add(Format('%d:%d', [CurCastle.m_LeftWall.BaseObj.m_nCurrX, CurCastle.m_LeftWall.BaseObj.m_nCurrY]));
    ListItem.SubItems.Add(Format('%d/%d', [CurCastle.m_LeftWall.BaseObj.m_WAbil.HP, CurCastle.m_LeftWall.BaseObj.m_WAbil.MaxHP]));
  end
  else
  begin
    ListItem.SubItems.Add(CurCastle.m_LeftWall.sName);
    ListItem.SubItems.Add(Format('%d:%d', [CurCastle.m_LeftWall.nX, CurCastle.m_LeftWall.nY]));
    ListItem.SubItems.Add(Format('%d/%d', [0, 0]));
  end;

  ListItem := ListViewGuard.Items.Add;
  ListItem.Caption := '2';
  if CurCastle.m_CenterWall.BaseObj <> nil then
  begin
    ListItem.SubItems.Add(CurCastle.m_CenterWall.BaseObj.m_sCharName);
    ListItem.SubItems.Add(Format('%d:%d', [CurCastle.m_CenterWall.BaseObj.m_nCurrX, CurCastle.m_CenterWall.BaseObj.m_nCurrY]));
    ListItem.SubItems.Add(Format('%d/%d', [CurCastle.m_CenterWall.BaseObj.m_WAbil.HP, CurCastle.m_CenterWall.BaseObj.m_WAbil.MaxHP]));
  end
  else
  begin
    ListItem.SubItems.Add(CurCastle.m_CenterWall.sName);
    ListItem.SubItems.Add(Format('%d:%d', [CurCastle.m_CenterWall.nX, CurCastle.m_CenterWall.nY]));
    ListItem.SubItems.Add(Format('%d/%d', [0, 0]));
  end;

  ListItem := ListViewGuard.Items.Add;
  ListItem.Caption := '3';
  if CurCastle.m_RightWall.BaseObj <> nil then
  begin
    ListItem.SubItems.Add(CurCastle.m_RightWall.BaseObj.m_sCharName);
    ListItem.SubItems.Add(Format('%d:%d', [CurCastle.m_RightWall.BaseObj.m_nCurrX, CurCastle.m_RightWall.BaseObj.m_nCurrY]));
    ListItem.SubItems.Add(Format('%d/%d', [CurCastle.m_RightWall.BaseObj.m_WAbil.HP, CurCastle.m_RightWall.BaseObj.m_WAbil.MaxHP]));
  end
  else
  begin
    ListItem.SubItems.Add(CurCastle.m_RightWall.sName);
    ListItem.SubItems.Add(Format('%d:%d', [CurCastle.m_RightWall.nX, CurCastle.m_RightWall.nY]));
    ListItem.SubItems.Add(Format('%d/%d', [0, 0]));
  end;
  for I := Low(CurCastle.m_Archer) to High(CurCastle.m_Archer) do
  begin
    ObjUnit := @CurCastle.m_Archer[I];
    ListItem := ListViewGuard.Items.Add;
    ListItem.Caption := IntToStr(I + 4);
    if ObjUnit.BaseObj <> nil then
    begin
      ListItem.SubItems.Add(ObjUnit.BaseObj.m_sCharName);
      ListItem.SubItems.Add(Format('%d:%d', [ObjUnit.BaseObj.m_nCurrX, ObjUnit.BaseObj.m_nCurrY]));
      ListItem.SubItems.Add(Format('%d/%d', [ObjUnit.BaseObj.m_WAbil.HP, ObjUnit.BaseObj.m_WAbil.MaxHP]));
    end
    else
    begin
      ListItem.SubItems.Add(ObjUnit.sName);
      ListItem.SubItems.Add(Format('%d:%d', [ObjUnit.nX, ObjUnit.nY]));
      ListItem.SubItems.Add(Format('%d/%d', [0, 0]));
    end;
  end;
  for II := Low(CurCastle.m_Guard) to High(CurCastle.m_Guard) do
  begin
    ObjUnit := @CurCastle.m_Guard[II];
    ListItem := ListViewGuard.Items.Add;
    ListItem.Caption := IntToStr(II + 4);
    if ObjUnit.BaseObj <> nil then
    begin
      ListItem.SubItems.Add(ObjUnit.BaseObj.m_sCharName);
      ListItem.SubItems.Add(Format('%d:%d', [ObjUnit.BaseObj.m_nCurrX, ObjUnit.BaseObj.m_nCurrY]));
      ListItem.SubItems.Add(Format('%d/%d', [ObjUnit.BaseObj.m_WAbil.HP, ObjUnit.BaseObj.m_WAbil.MaxHP]));
    end
    else
    begin
      ListItem.SubItems.Add(ObjUnit.sName);
      ListItem.SubItems.Add(Format('%d:%d', [ObjUnit.nX, ObjUnit.nY]));
      ListItem.SubItems.Add(Format('%d/%d', [0, 0]));
    end;
  end;
  EditCastleName.Text := CurCastle.m_sName;
  if CurCastle.m_MasterGuild <> nil then
    EditCastleOfGuild.Text := CurCastle.m_MasterGuild.sGuildName
  else
    EditCastleOfGuild.Text := '';
  EditPalace.Text := CurCastle.m_sPalaceMap;
  EditHomeMap.Text := CurCastle.m_sHomeMap;
  SpinEditNomeX.Value := CurCastle.m_nHomeX;
  SpinEditNomeY.Value := CurCastle.m_nHomeY;
  EditTunnelMap.Text := CurCastle.m_sSecretMap;

  ButtonStartWar.Enabled := not CurCastle.m_boUnderWar;
  ButtonStopWar.Enabled := not ButtonStartWar.Enabled;

  ButtonAttackAdd.Enabled := True;
  ButtonRefAttackSabukWall.Enabled := True;

  if CurCastle.m_boUnderWar then
    EditWarStatus.Text := '攻城中...'
  else
    EditWarStatus.Text := '停战中...';

  RefCastleAttackSabukWall;
  boRefing := False;
end;

procedure TfrmCastleManage.RefCastleList;
var
  I: Integer;
  UserCastle: TUserCastle;
  ListItem: TListItem;
begin
  g_CastleManager.Lock;
  try
    for I := 0 to g_CastleManager.m_CastleList.Count - 1 do
    begin
      UserCastle := TUserCastle(g_CastleManager.m_CastleList.Items[I]);
      ListItem := ListViewCastle.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.AddObject(UserCastle.m_sConfigDir, UserCastle);
      ListItem.SubItems.Add(UserCastle.m_sName)
    end;
  finally
    g_CastleManager.UnLock;
  end;

end;

procedure TfrmCastleManage.RefCastleAttackSabukWall;
var
  I: Integer;
  ListItem: TListItem;
  AttackerInfo: pTAttackerInfo;
begin
  if CurCastle = nil then
    Exit;
  ListViewAttackSabukWall.Items.Clear;
  ListViewAttackSabukWall.Items.BeginUpdate;
  try
    for I := 0 to CurCastle.m_AttackWarList.Count - 1 do
    begin
      AttackerInfo := pTAttackerInfo(CurCastle.m_AttackWarList.Items[I]);
      ListItem := ListViewAttackSabukWall.Items.Add;
      ListItem.Caption := IntToStr(I);
      ListItem.SubItems.AddObject(AttackerInfo.sGuildName, TObject(AttackerInfo));
      ListItem.SubItems.Add(DateToStr(AttackerInfo.AttackDate));
    end;
  finally
    ListViewAttackSabukWall.Items.EndUpdate;
  end;
end;

procedure TfrmCastleManage.ListViewCastleClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  CurCastle := nil;
  ListItem := ListViewCastle.Selected;
  if ListItem = nil then
    Exit;
  CurCastle := TUserCastle(ListItem.SubItems.Objects[0]);
  RefCastleInfo();
end;

procedure TfrmCastleManage.ButtonRefreshClick(Sender: TObject);
begin
  RefCastleInfo();
end;

procedure TfrmCastleManage.ButtonAttackAddClick(Sender: TObject);
begin
  FrmAttackSabukWall := TFrmAttackSabukWall.Create(Owner);
  // FrmAttackSabukWall.Caption := '增加攻城申请';
  FrmAttackSabukWall.Top := frmCastleManage.Top - 50;
  FrmAttackSabukWall.Left := frmCastleManage.Left + 150;
  FrmAttackSabukWall.Open(True);
  FrmAttackSabukWall.Free;
end;

procedure TfrmCastleManage.ButtonAttackEditClick(Sender: TObject);
begin
  if CurCastle = nil then
    Exit;
  if SelAttackGuildInfo = nil then
    Exit;
  FrmAttackSabukWall := TFrmAttackSabukWall.Create(Owner);
  // FrmAttackSabukWall.Caption := '编辑攻城申请';
  FrmAttackSabukWall.Top := frmCastleManage.Top - 50;
  FrmAttackSabukWall.Left := frmCastleManage.Left + 150;
  // m_sGuildName := SelAttackGuildInfo.sGuildName;
  // m_AttackDate := SelAttackGuildInfo.AttackDate;
  FrmAttackSabukWall.Open(False);
  FrmAttackSabukWall.Free;
end;

procedure TfrmCastleManage.ListViewAttackSabukWallClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  ButtonAttackEdit.Enabled := False;
  ButtonAttackDel.Enabled := False;
  SelAttackGuildInfo := nil;
  ListItem := ListViewAttackSabukWall.Selected;
  if ListItem = nil then
    Exit;
  SelAttackGuildInfo := pTAttackerInfo(ListItem.SubItems.Objects[0]);
  ButtonAttackEdit.Enabled := True;
  ButtonAttackDel.Enabled := True;
end;

procedure TfrmCastleManage.ButtonRefAttackSabukWallClick(Sender: TObject);
begin
  if CurCastle = nil then
    Exit;
  RefCastleAttackSabukWall;
end;

procedure TfrmCastleManage.ButtonAttackDelClick(Sender: TObject);
var
  I: Integer;
  AttackerInfo: pTAttackerInfo;
begin
  if CurCastle = nil then
    Exit;
  if SelAttackGuildInfo = nil then
    Exit;
  if Application.MessageBox(PChar('是否确认删除此行会攻城申请？' + #10#10 + '行会名称：' + SelAttackGuildInfo.sGuildName + #10 + '攻城时间：' + DateToStr(SelAttackGuildInfo.AttackDate)),
    '确认信息', MB_YESNO + MB_ICONQUESTION) = IDYES then
  begin
    for I := CurCastle.m_AttackWarList.Count - 1 downto 0 do
    begin
      AttackerInfo := pTAttackerInfo(CurCastle.m_AttackWarList.Items[I]);
      if AttackerInfo = SelAttackGuildInfo then
      begin
        CurCastle.m_AttackWarList.Delete(I);
        CurCastle.Save;
        Dispose(AttackerInfo);
        SelAttackGuildInfo := nil;
        Break;
      end;
    end;
    RefCastleAttackSabukWall;
  end;
end;

procedure TfrmCastleManage.ButtonSaveClick(Sender: TObject);
begin
  if CurCastle = nil then
    Exit;
  CurCastle.m_sHomeMap := EditHomeMap.Text;
  CurCastle.m_sPalaceMap := EditPalace.Text;
  CurCastle.m_sHomeMap := EditHomeMap.Text;
  CurCastle.m_nHomeX := SpinEditNomeX.Value;
  CurCastle.m_nHomeY := SpinEditNomeY.Value;
  CurCastle.m_sSecretMap := EditTunnelMap.Text;
  CurCastle.Save;
  ButtonSave.Enabled := False;
end;

procedure TfrmCastleManage.ButtonStartWarClick(Sender: TObject);
var
  I: Integer;
  Guild: TGUild;
begin
  if CurCastle <> nil then
  begin
    if not CurCastle.m_boUnderWar then
    begin
      for I := 0 to g_GuildManager.GuildList.Count - 1 do
      begin
        Guild := TGUild(g_GuildManager.GuildList.Items[I]);
        CurCastle.AddAttackerInfo(Guild, 0);
      end;
      CurCastle.Save;
      CurCastle.m_boStartWar := False;
      CurCastle.StartWar;
      RefCastleInfo();
    end;
  end;
end;

procedure TfrmCastleManage.ButtonStopWarClick(Sender: TObject);
begin
  if CurCastle <> nil then
  begin
    if CurCastle.m_boUnderWar then
    begin
      CurCastle.StopWar;
      RefCastleInfo();
    end;
  end;
end;

procedure TfrmCastleManage.Timer1Timer(Sender: TObject);
begin
  Timer1.Enabled := False;
  if (ListViewCastle.ItemIndex < 0) and (ListViewCastle.Items.Count > 0) then
  begin
    ListViewCastle.ItemIndex := 0;
    ListViewCastleClick(Self);
  end;
end;

end.


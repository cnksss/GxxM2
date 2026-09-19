
unit uFrmDataManager;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics,
  Controls, Forms, Dialogs, StdCtrls, DB, DBTables, Grids,
  Buttons, Grobal2, RoleDB, VirtualTrees, SqliteRoleDB;

type
  TFrmDataManager = class(TForm)
    lblRole: TLabel;
    edtRole: TEdit;
    BtnCreateChr: TButton;
    btnDeleteRole: TButton;
    btnSearchRole: TButton;
    btnDisableHuman: TButton;
    btnEnableHuman: TButton;
    LabelCount: TLabel;
    btnEditData: TButton;
    vstRole: TVirtualStringTree;
    chkRoleFuzzy: TCheckBox;
    lblAccount: TLabel;
    btnSearchAccount: TButton;
    edtAccount: TEdit;
    chkAccountFuzzy: TCheckBox;
    procedure FormCreate(Sender: TObject);

    procedure BtnCreateChrClick(Sender: TObject);
    //procedure RefChrGrid(n08: Integer; HumDBRecord: THumInfo);

    procedure edtRoleKeyPress(Sender: TObject; var Key: Char);
    procedure edtAccountKeyPress(Sender: TObject; var Key: Char);
    procedure btnEditDataClick(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure vstRoleGetText(Sender: TBaseVirtualTree; Node: PVirtualNode;
      Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: WideString);
    procedure btnSearchAccountClick(Sender: TObject);
    procedure btnSearchRoleClick(Sender: TObject);
    procedure vstRoleFocusChanged(Sender: TBaseVirtualTree;
      Node: PVirtualNode; Column: TColumnIndex);
    procedure btnDisableHumanClick(Sender: TObject);
    procedure btnEnableHumanClick(Sender: TObject);
    procedure btnDeleteRoleClick(Sender: TObject);
    procedure vstRoleNodeDblClick(Sender: TBaseVirtualTree;
      const HitInfo: THitInfo);
  private
    FSerarchRoleList: TSerarchRoleList;
    procedure RefreshRoleList;
  private
    function ShowAskMsg(const S: string): Boolean;

  protected
    procedure ShowInfoMsg(const S: string);
    //procedure ShowErrorMsg(const S: string);
  end;

  procedure ShowFrmDataManager;

implementation

uses HUtil32, MudUtil, CreateChr, uFrmRoleDataEdit, DBShare;

{$R *.DFM}
procedure ShowFrmDataManager;
var
  FrmDataManager: TFrmDataManager;
begin
  FrmDataManager := TFrmDataManager.Create(nil);
  try
    FrmDataManager.ShowModal;
  finally
    FrmDataManager.Free;
  end;
end;

procedure TFrmDataManager.FormCreate(Sender: TObject);
begin
  FSerarchRoleList := TSerarchRoleList.Create;
end;

procedure TFrmDataManager.FormDestroy(Sender: TObject);
begin
  FSerarchRoleList.Free;
end;

function TFrmDataManager.ShowAskMsg(const S: string): Boolean;
begin
  Result := Application.MessageBox(PChar(S), '询问', MB_YESNO or MB_ICONQUESTION) = mrYes;
end;

procedure TFrmDataManager.ShowInfoMsg(const S: string);
begin
  Application.MessageBox(PChar(S), '提示', MB_OK or MB_ICONINFORMATION);
end;

{
procedure TFrmDataManager.ShowErrorMsg(const S: string);
begin
  Application.MessageBox(PChar(S), '错误', MB_OK or MB_ICONWARNING);
end;
}

procedure TFrmDataManager.edtAccountKeyPress(Sender: TObject; var Key: Char);
begin
  if Key = #13 then
  begin
    Key := #0;
    btnSearchAccount.Click;
  end;
end;

procedure TFrmDataManager.RefreshRoleList;
var
  I: Integer;
  RoleData: PTSerarchRoleData;
begin
  vstRole.Clear;
  for I := 0 to FSerarchRoleList.Count - 1 do
  begin
    RoleData := FSerarchRoleList.Items[I];
    vstRole.AddChild(nil, RoleData);
  end;
end;

procedure TFrmDataManager.edtRoleKeyPress(Sender: TObject; var Key: Char);
begin
  if Key = #13 then
  begin
    Key := #0;
    btnSearchRole.Click;
  end;
end;

procedure TFrmDataManager.BtnCreateChrClick(Sender: TObject);
begin
(*
var
  nCheckCode: Integer;
  HumRecord: THumInfo;
begin
  if not FrmCreateChr.IncputChrInfo then Exit;
  nCheckCode := 0;
  try
    if g_HumCharDB.Open then
    begin
      if g_HumCharDB.ChrCountOfAccount(FrmCreateChr.sUserId) < 2 then
      begin
        HumRecord.Header.boDeleted := False;
        HumRecord.Header.boIsHero := False;
        HumRecord.Header.sName := FrmCreateChr.sChrName;
        HumRecord.Header.nSelectID := FrmCreateChr.nSelectID;
        HumRecord.boIsHero := False;
        //HumRecord.boSelected := True;
        HumRecord.sChrName := FrmCreateChr.sChrName;
        HumRecord.sAccount := FrmCreateChr.sUserId;
        HumRecord.boDeleted := False;
        HumRecord.btCount := 0;
        if HumRecord.Header.sName <> '' then
        begin
          if not g_HumCharDB.Add(@HumRecord) then nCheckCode := 2;
        end;
      end
      else
        nCheckCode := 3;
    end;
  finally
    g_HumCharDB.Close;
  end;
  if nCheckCode = 0 then
    ShowMessage('人物创建成功...')
  else
    ShowMessage('人物创建失败！！！')
  *)
end;

procedure TFrmDataManager.btnEditDataClick(Sender: TObject);
var
  P: Pointer;
  RoleData: PTSerarchRoleData;
  HumData: PTHumData;
  HumanID: Integer;

  HeroData: THeroData;
  HeroID: Integer;
begin
  if vstRole.FocusedNode = nil then Exit;
  P := vstRole.GetNodeData(vstRole.FocusedNode);
  if P = nil then Exit;
  RoleData := PTSerarchRoleData(P^);
  if not RoleData.IsHero then
  begin
    GetMem(HumData, SizeOf(THumData));
    try
      if g_RoleDB.HumanDB.Get(RoleData.Account, RoleData.RoleName, HumData^, HumanID) then
      begin
        ShowFrmRoleDataEdit(HumanID, HumData, nil);
      end;
    finally
      FreeMem(HumData);
    end;
  end
  else if g_RoleDB.HeroDB.Get(RoleData.RoleName, HeroData, HeroID) then
  begin
    ShowFrmRoleDataEdit(HeroID, nil, @HeroData);
  end;
end;

procedure TFrmDataManager.btnSearchAccountClick(Sender: TObject);
var
  I: Integer;
  Account: string;
  TempRoleList: TSerarchRoleList;
  RoleData: PTSerarchRoleData;
begin
  Account := edtAccount.Text;
  if Length(Account) > 0 then
  begin
    FSerarchRoleList.Clear;
    
    btnDisableHuman.Enabled := False;
    btnEnableHuman.Enabled := False;
    btnEditData.Enabled := False;

    TempRoleList := TSerarchRoleList.Create;
    try
      if chkAccountFuzzy.Checked then
      begin
        g_RoleDB.HumanDB.SearchByAccount(Account, smtFuzzy, FSerarchRoleList);
        g_RoleDB.HeroDB.SearchByAccount(Account, smtFuzzy, TempRoleList);
      end
      else
      begin
        g_RoleDB.HumanDB.SearchByAccount(Account, smtComplete, FSerarchRoleList);
        g_RoleDB.HeroDB.SearchByAccount(Account, smtComplete, TempRoleList);
      end;

      for I := 0 to TempRoleList.Count - 1 do
      begin
        RoleData := TempRoleList.Items[I];
        FSerarchRoleList.Add(RoleData);
      end;
    finally
      TempRoleList.Free;
    end;

    RefreshRoleList;
  end;
end;

procedure TFrmDataManager.btnSearchRoleClick(Sender: TObject);
var
  I: Integer;
  Role: string;
  TempRoleList: TSerarchRoleList;
  RoleData: PTSerarchRoleData;
begin
  Role := edtRole.Text;
  if Length(Role) > 0 then
  begin
    FSerarchRoleList.Clear;

    TempRoleList := TSerarchRoleList.Create;
    try
      if chkRoleFuzzy.Checked then
      begin
        g_RoleDB.HumanDB.SearchByName(Role, smtFuzzy, FSerarchRoleList);
        g_RoleDB.HeroDB.SearchByName(Role, smtFuzzy, TempRoleList);
      end
      else
      begin
        g_RoleDB.HumanDB.SearchByName(Role, smtComplete, FSerarchRoleList);
        g_RoleDB.HeroDB.SearchByName(Role, smtComplete, TempRoleList);
      end;

      for I := 0 to TempRoleList.Count - 1 do
      begin
        RoleData := TempRoleList.Items[I];
        FSerarchRoleList.Add(RoleData);
      end;
    finally
      TempRoleList.Free;
    end;

    RefreshRoleList;
  end;
end;

function GetJobName(nJob: Integer): string;
begin
  case nJob of
    0: Result := '战士';
    1: Result := '法师';
    2: Result := '道士';
  else
    Result := '-';
  end;
end;

procedure TFrmDataManager.vstRoleGetText(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
  var CellText: WideString);
const
  SEX_NAME: array[Boolean] of string = ('男', '女');
  BOOL_NAME: array[Boolean] of string = ('', '√');
var
  P: Pointer;
  RoleData: PTSerarchRoleData;
begin
  P := Sender.GetNodeData(Node);
  if P = nil then Exit;
  RoleData := PTSerarchRoleData(P^);
  case Column of
    0: CellText := BOOL_NAME[RoleData.IsDelete and 2 = 0];
    1: CellText := RoleData.Account;
    2: CellText := RoleData.RoleName;
    3: CellText := BOOL_NAME[RoleData.IsHero];
    4: CellText := BOOL_NAME[RoleData.IsDelete and 1 <> 0];
    5: CellText := SEX_NAME[RoleData.Sex = 1];
    6: CellText := GetJobName(RoleData.Job);
    7: CellText := IntToStr(RoleData.Level);
  end;
end;

procedure TFrmDataManager.vstRoleFocusChanged(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex);
var
  P: Pointer;
  RoleData: PTSerarchRoleData;
begin
  if Node = nil then
  begin
    btnDisableHuman.Enabled := False;
    btnEnableHuman.Enabled := False;
    btnEditData.Enabled := False;
    Exit;
  end;
  
  P := Sender.GetNodeData(Node);
  if P = nil then Exit;
  RoleData := PTSerarchRoleData(P^);
  btnDisableHuman.Enabled := (not RoleData.IsHero) and (RoleData.IsDelete and 2 = 0);
  btnEnableHuman.Enabled := (not RoleData.IsHero) and (RoleData.IsDelete and 2 <> 0);
  btnEditData.Enabled := True;
end;

procedure TFrmDataManager.btnDisableHumanClick(Sender: TObject);
var
  P: Pointer;
  RoleData: PTSerarchRoleData;
begin
  if vstRole.FocusedNode = nil then Exit;

  P := vstRole.GetNodeData(vstRole.FocusedNode);
  if P = nil then Exit;
  RoleData := PTSerarchRoleData(P^);

  if (RoleData.IsHero) or (RoleData.IsDelete and 2 <> 0) then Exit;

  if ShowAskMsg('被禁用人物在登录器中无法恢复' + sLineBreak + sLineBreak + '是否禁用人物 "' + RoleData.RoleName + '" ?') then
  begin
    if g_RoleDB.HumanDB.SetEnabled(RoleData.Account, RoleData.RoleName, RoleData.IsDelete or 2) then
    begin
      RoleData.IsDelete := RoleData.IsDelete or 2;
      vstRole.InvalidateNode(vstRole.FocusedNode);
      btnDisableHuman.Enabled := False;
      btnEnableHuman.Enabled := True;
    end;
  end;
end;

procedure TFrmDataManager.btnEnableHumanClick(Sender: TObject);
var
  P: Pointer;
  RoleData: PTSerarchRoleData;
begin
  if vstRole.FocusedNode = nil then Exit;

  P := vstRole.GetNodeData(vstRole.FocusedNode);
  if P = nil then Exit;
  RoleData := PTSerarchRoleData(P^);

  if (RoleData.IsHero) or (RoleData.IsDelete and 2 = 0) then Exit;

  if ShowAskMsg('是否启用人物 "' + RoleData.RoleName + '" ?') then
  begin
    if g_RoleDB.HumanDB.SetEnabled(RoleData.Account, RoleData.RoleName, RoleData.IsDelete and (not 2)) then
    begin
      RoleData.IsDelete := RoleData.IsDelete and (not 2);
      vstRole.InvalidateNode(vstRole.FocusedNode);
      btnDisableHuman.Enabled := True;
      btnEnableHuman.Enabled := False;
    end;
  end;
end;

procedure TFrmDataManager.btnDeleteRoleClick(Sender: TObject);
const
  RoleTypeNames: array[Boolean] of string = ('人物', '英雄');
var
  P: Pointer;
  RoleData: PTSerarchRoleData;

  IsOK: Boolean;
begin
  if vstRole.FocusedNode = nil then Exit;

  P := vstRole.GetNodeData(vstRole.FocusedNode);
  if P = nil then Exit;
  RoleData := PTSerarchRoleData(P^);

  if ShowAskMsg('执行删除操作将会删除角色对应的数据并且不可恢复！' + sLineBreak + sLineBreak +
    '你确定要删除' + RoleTypeNames[RoleData.IsHero] + ' "' + RoleData.RoleName + '" 吗?') then
  begin
    if not RoleData.IsHero then
    begin
      IsOK := g_RoleDB.HumanDB.Erase(RoleData.Account, RoleData.RoleName);
    end
    else
    begin
      IsOK := g_RoleDB.HeroDB.Erase(RoleData.RoleName);
    end;

    if IsOK then
    begin
      vstRole.DeleteNode(vstRole.FocusedNode);
    end;
  end;
end;

procedure TFrmDataManager.vstRoleNodeDblClick(Sender: TBaseVirtualTree;
  const HitInfo: THitInfo);
var
  P: Pointer;
  RoleData: PTSerarchRoleData;
  HumData: PTHumData;
  HumanID: Integer;

  HeroData: THeroData;
  HeroID: Integer;
begin
  if HitInfo.HitNode = nil then Exit;
  P := vstRole.GetNodeData(HitInfo.HitNode);
  if P = nil then Exit;
  RoleData := PTSerarchRoleData(P^);
  if not RoleData.IsHero then
  begin
    GetMem(HumData, SizeOf(THumData));
    try
      if g_RoleDB.HumanDB.Get(RoleData.Account, RoleData.RoleName, HumData^, HumanID) then
      begin
        ShowFrmRoleDataEdit(HumanID, HumData, nil);
      end;
    finally
      FreeMem(HumData);
    end;
  end
  else if g_RoleDB.HeroDB.Get(RoleData.RoleName, HeroData, HeroID) then
  begin
    ShowFrmRoleDataEdit(HeroID, nil, @HeroData);
  end;
end;

end.


unit uFrmLogClientPacketSetting;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, GateShare, IniFiles;

type
  TFrmLogClientPacketSetting = class(TForm)
    grpPacketType: TGroupBox;
    chkLogMove: TCheckBox;
    chkLogHit: TCheckBox;
    chkLogSpell: TCheckBox;
    chkLogQuery: TCheckBox;
    chkLogTeam: TCheckBox;
    chkLogGuild: TCheckBox;
    chkLogShop: TCheckBox;
    chkLogOther: TCheckBox;
    grpLogUser: TGroupBox;
    lstLogUser: TListBox;
    lbl1: TLabel;
    edtUserName: TEdit;
    btnAddUser: TButton;
    btnDelUser: TButton;
    btnOK: TButton;
    btnCancel: TButton;
    chkLogClientPacket: TCheckBox;
    procedure btnOKClick(Sender: TObject);
    procedure btnAddUserClick(Sender: TObject);
    procedure btnDelUserClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure lstLogUserClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

  procedure ShowFrmLogClientPacketSetting;

implementation

{$R *.dfm}

procedure ShowFrmLogClientPacketSetting;
var
  FrmLogClientPacketSetting: TFrmLogClientPacketSetting;
begin
  FrmLogClientPacketSetting := TFrmLogClientPacketSetting.Create(nil);
  try
    FrmLogClientPacketSetting.ShowModal;
  finally
    FrmLogClientPacketSetting.Free;
  end;
end;

procedure TFrmLogClientPacketSetting.btnOKClick(Sender: TObject);
var
  LogClientPacketType: Integer;
  IniFile: TIniFile;
begin
  LogClientPacketType := 0;
  if chkLogMove.Checked then LogClientPacketType := LogClientPacketType or CPT_MOVE;
  if chkLogHit.Checked then LogClientPacketType := LogClientPacketType or CPT_HIT;
  if chkLogSpell.Checked then LogClientPacketType := LogClientPacketType or CPT_SPELL;
  if chkLogQuery.Checked then LogClientPacketType := LogClientPacketType or CPT_QUERY;
  if chkLogTeam.Checked then LogClientPacketType := LogClientPacketType or CPT_TEAM;
  if chkLogGuild.Checked then LogClientPacketType := LogClientPacketType or CPT_GUILD;
  if chkLogShop.Checked then LogClientPacketType := LogClientPacketType or CPT_SHOP;

  g_nLogClientPacketType := LogClientPacketType;
  g_boLogClientPacket := chkLogClientPacket.Checked;

  g_LogClientPacketUser.Lock;
  try
    g_LogClientPacketUser.Text := lstLogUser.Items.Text;
  finally
    g_LogClientPacketUser.UnLock;
  end;
  g_LogClientPacketUser.SaveToFile(g_sLogClientPakcetUserFile);

  IniFile := TIniFile.Create(g_sIniFileName);
  try
    IniFile.WriteBool(GateClass, 'LogClientPacket', g_boLogClientPacket);
    IniFile.WriteInteger(GateClass, 'LogClientPacketType', g_nLogClientPacketType);
  finally
    IniFile.Free;
  end;

  ModalResult := mrOK;
end;

procedure TFrmLogClientPacketSetting.btnAddUserClick(Sender: TObject);
var
  UserName: string;
begin
  UserName := Trim(edtUserName.Text);
  if Length(UserName) = 0 then
  begin
    Application.MessageBox('人物名称不能为空', '提示', MB_OK or MB_ICONINFORMATION);
    edtUserName.SetFocus;
    Exit;
  end;

  if lstLogUser.Items.IndexOf(UserName) >= 0 then
  begin
    Application.MessageBox('人物名称已经在列表中存在', '提示', MB_OK or MB_ICONINFORMATION);
    edtUserName.SetFocus;
    Exit;
  end;

  lstLogUser.Items.Add(UserName);  
end;

procedure TFrmLogClientPacketSetting.btnDelUserClick(Sender: TObject);
begin
  if (lstLogUser.Count > 0) and (lstLogUser.ItemIndex >= 0) then
  begin
    lstLogUser.DeleteSelected;
  end;
end;

procedure TFrmLogClientPacketSetting.FormCreate(Sender: TObject);
begin
  if g_nLogClientPacketType and CPT_MOVE <> 0 then chkLogMove.Checked := True;
  if g_nLogClientPacketType and CPT_HIT <> 0 then chkLogHit.Checked := True;
  if g_nLogClientPacketType and CPT_SPELL <> 0 then chkLogSpell.Checked := True;
  if g_nLogClientPacketType and CPT_QUERY <> 0 then chkLogQuery.Checked := True;
  if g_nLogClientPacketType and CPT_TEAM <> 0 then chkLogTeam.Checked := True;
  if g_nLogClientPacketType and CPT_GUILD <> 0 then chkLogGuild.Checked := True;
  if g_nLogClientPacketType and CPT_SHOP <> 0 then chkLogShop.Checked := True;

  chkLogClientPacket.Checked := g_boLogClientPacket;

  lstLogUser.Items.Text := g_LogClientPacketUser.Text;
  btnDelUser.Enabled := (lstLogUser.Count > 0) and (lstLogUser.ItemIndex >= 0);
end;

procedure TFrmLogClientPacketSetting.lstLogUserClick(Sender: TObject);
begin
  btnDelUser.Enabled := (lstLogUser.Count > 0) and (lstLogUser.ItemIndex >= 0);
end;

end.

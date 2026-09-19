unit AttackSabukWallConfig;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls, CastleManage, Castle, Guild,
  ComCtrls;

type
  TFrmAttackSabukWall = class(TForm)
    GroupBox1: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    EditGuildName: TEdit;
    RzDateTimeEditAttackDate: TDateTimePicker;
    ButtonOK: TButton;
    ListBoxGuild: TListBox;
    CheckBoxAll: TCheckBox;
    ButtonCancel: TButton;
    procedure ButtonOKClick(Sender: TObject);
    procedure ListBoxGuildClick(Sender: TObject);
    procedure CheckBoxAllClick(Sender: TObject);
    procedure ButtonCancelClick(Sender: TObject);
  private
    FAddAttackGuild: Boolean;
    procedure LoadGuildList();
  public
    { Public declarations }
    procedure Open(boAdd: Boolean);
  end;

var
  FrmAttackSabukWall: TFrmAttackSabukWall;
  // nStute: Integer;
  // m_sGuildName: string;
  // m_AttackDate: TDate;

implementation

uses
  M2Share;
{$R *.dfm}

procedure TFrmAttackSabukWall.LoadGuildList();
var
  I: Integer;
  Guild: TGUild;
begin
  ListBoxGuild.Items.Clear;
  for I := 0 to g_GuildManager.GuildList.Count - 1 do
  begin
    Guild := TGUild(g_GuildManager.GuildList.Items[I]);
    ListBoxGuild.Items.AddObject(Guild.sGuildName, TObject(Guild));
  end;
end;

procedure TFrmAttackSabukWall.Open(boAdd: Boolean);
begin
  FAddAttackGuild := boAdd;

  case FAddAttackGuild of
    True:
      begin
        Caption := '增加攻城行会';
        EditGuildName.Text := '';
        RzDateTimeEditAttackDate.Date := Date;
      end;
    False:
      begin
        if SelAttackGuildInfo <> nil then
        begin
          Caption := '编辑攻城行会 ' + SelAttackGuildInfo.sGuildName;
          EditGuildName.Text := SelAttackGuildInfo.sGuildName;
          RzDateTimeEditAttackDate.Date := SelAttackGuildInfo.AttackDate;
        end
        else
        begin
          Caption := '编辑攻城行会';
          EditGuildName.Text := '';
          RzDateTimeEditAttackDate.Date := Now;
        end;
      end;
  end;
  LoadGuildList();
  ShowModal;
end;

procedure TFrmAttackSabukWall.ButtonOKClick(Sender: TObject);
var
  I: Integer;
  sGuildName: string;
  AttackDate: TDate;
  AttackerInfo: pTAttackerInfo;
  Guild: TGUild;
begin
  if CurCastle <> nil then
  begin
    ButtonOK.Enabled := False;
    sGuildName := Trim(EditGuildName.Text);
    AttackDate := RzDateTimeEditAttackDate.Date;
    case FAddAttackGuild of
      True:
        begin
          if CheckBoxAll.Checked then
          begin
            for I := 0 to g_GuildManager.GuildList.Count - 1 do
            begin
              Guild := TGUild(g_GuildManager.GuildList.Items[I]);
              CurCastle.AddAttackerInfo(Guild, AttackDate);
            end;
            CurCastle.Save;
          end
          else
          begin
            Guild := g_GuildManager.FindGuild(sGuildName);
            if Guild <> nil then
            begin
              CurCastle.AddAttackerInfo(Guild, AttackDate);
              CurCastle.Save;
            end
            else
            begin
              Application.MessageBox('输入的行会不存在！', '提示信息', MB_ICONQUESTION);
              Exit;
            end;
          end;
        end;
      False:
        begin
          if CheckBoxAll.Checked then
          begin
            for I := 0 to CurCastle.m_AttackWarList.Count - 1 do
            begin
              AttackerInfo := pTAttackerInfo(CurCastle.m_AttackWarList.Items[I]);
              AttackerInfo.AttackDate := AttackDate;
            end;
            CurCastle.Save;
          end
          else
          begin
            if SelAttackGuildInfo <> nil then
            begin
              SelAttackGuildInfo.AttackDate := AttackDate;
              CurCastle.Save;
            end;
          end;
        end;
    end;
    frmCastleManage.RefCastleAttackSabukWall;
  end;
  Close;
end;

procedure TFrmAttackSabukWall.ListBoxGuildClick(Sender: TObject);
begin
  if (ListBoxGuild.ItemIndex >= 0) and (ListBoxGuild.ItemIndex < ListBoxGuild.Items.Count) then
    EditGuildName.Text := ListBoxGuild.Items.Strings[ListBoxGuild.ItemIndex]
  else
    EditGuildName.Text := '';
end;

procedure TFrmAttackSabukWall.CheckBoxAllClick(Sender: TObject);
begin
  EditGuildName.Enabled := not CheckBoxAll.Checked;
end;

procedure TFrmAttackSabukWall.ButtonCancelClick(Sender: TObject);
begin
  Close;
end;

end.


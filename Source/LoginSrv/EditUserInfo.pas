unit EditUserInfo;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics,
  Controls, Forms, Dialogs, StdCtrls, AccountDB, Grobal2, LSShare;
type
  TFrmUserInfoEdit = class(TForm)
    Label1: TLabel;
    edtAccountName: TEdit;
    Label2: TLabel;
    edtPassword: TEdit;
    Label3: TLabel;
    edtUserName: TEdit;
    Label4: TLabel;
    edtBirthday: TEdit;
    Label5: TLabel;
    edtPhone: TEdit;
    Button1: TButton;
    Button2: TButton;
    chkEditAccount: TCheckBox;
    Label9: TLabel;
    edtIDCard: TEdit;
    Label6: TLabel;
    Label7: TLabel;
    edtQuestions1: TEdit;
    edtAnswers1: TEdit;
    Label10: TLabel;
    Label11: TLabel;
    edtQuestions2: TEdit;
    edtAnswers2: TEdit;
    Label12: TLabel;
    edtMobilePhone: TEdit;
    Label13: TLabel;
    edtMemo: TEdit;
    Label14: TLabel;
    edtL2Password: TEdit;
    Label8: TLabel;
    edtMail: TEdit;
    Button3: TButton;
    procedure chkEditAccountClick(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure Button1Click(Sender: TObject);

  private
    { Private declarations }
  public
    function InputAccountInfo(boNew: Boolean; var AccountInfo: TAccountInfo): Boolean;
    { Public declarations }
  end;

var
  FrmUserInfoEdit: TFrmUserInfoEdit;


implementation

{$R *.DFM}
//00467148

procedure TFrmUserInfoEdit.chkEditAccountClick(Sender: TObject);
var
  IsEdit: Boolean;
begin
  IsEdit := chkEditAccount.Checked;
  edtUserName.Enabled := IsEdit;
  edtIDCard.Enabled := IsEdit;
  edtBirthday.Enabled := IsEdit;
  edtQuestions1.Enabled := IsEdit;
  edtAnswers1.Enabled := IsEdit;
  edtQuestions2.Enabled := IsEdit;
  edtAnswers2.Enabled := IsEdit;
  edtPhone.Enabled := IsEdit;
  edtMobilePhone.Enabled := IsEdit;
  edtMemo.Enabled := IsEdit;
  edtL2Password.Enabled := IsEdit;
  edtMail.Enabled := IsEdit;

end;
//00466B10

function TFrmUserInfoEdit.InputAccountInfo(boNew: Boolean; var AccountInfo: TAccountInfo): Boolean;
begin
  Result := False;
  if not boNew then
  begin
    chkEditAccount.Enabled := True;
    chkEditAccount.Checked := False;
    chkEditAccountClick(Self);
    edtAccountName.Enabled := False;
  end
  else
  begin
    chkEditAccount.Enabled := False;
    chkEditAccount.Checked := True;
    chkEditAccountClick(Self);
    edtAccountName.Enabled := True;
  end;

  if Label14.Visible then
  begin
    Label14.Caption := '二级密码:';
  end;

  edtAccountName.Text := AccountInfo.AccountName;
  edtPassword.Text := AccountInfo.Password;
  edtUserName.Text := AccountInfo.UserName;
  edtIDCard.Text := AccountInfo.IDCard;
  edtBirthday.Text := AccountInfo.BirthDay;
  edtQuestions1.Text := AccountInfo.Questions1;
  edtAnswers1.Text := AccountInfo.Answers1;
  edtQuestions2.Text := AccountInfo.Questions2;
  edtAnswers2.Text := AccountInfo.Answers2;
  edtPhone.Text := AccountInfo.Phone;
  edtMobilePhone.Text := AccountInfo.MobilePhone;
  edtMemo.Text := AccountInfo.Memo;
  edtL2Password.Text := AccountInfo.L2Password;
  edtMail.Text := AccountInfo.Mail;
  if ShowModal <> mrOK then exit;
  if boNew then
  begin
    AccountInfo.AccountName := Trim(edtAccountName.Text);
  end;
  AccountInfo.Password := Trim(edtPassword.Text);
  AccountInfo.UserName := Trim(edtUserName.Text);
  AccountInfo.IDCard := Trim(edtIDCard.Text);
  AccountInfo.BirthDay := Trim(edtBirthday.Text);
  AccountInfo.Questions1 := Trim(edtQuestions1.Text);
  AccountInfo.Answers1 := Trim(edtAnswers1.Text);
  AccountInfo.Questions2 := Trim(edtQuestions2.Text);
  AccountInfo.Answers2 := Trim(edtAnswers2.Text);
  AccountInfo.Phone := Trim(edtPhone.Text);
  AccountInfo.MobilePhone := Trim(edtMobilePhone.Text);
  AccountInfo.Memo := Trim(edtMemo.Text);
  AccountInfo.L2Password := Trim(edtL2Password.Text);
  AccountInfo.Mail := Trim(edtMail.Text);
  Result := True;
end;

procedure TFrmUserInfoEdit.Button3Click(Sender: TObject);
//var
  //n10: Integer;
  //AccountInfo: TAccountInfo;
begin
  if g_AccountDB.UnLockAccount(edtAccountName.Text) then
  begin
    Showmessage('解锁成功');
  end;
end;

procedure TFrmUserInfoEdit.Button1Click(Sender: TObject);
begin
  if edtAccountName.Enabled then
  begin
    if Length(Trim(edtAccountName.Text)) <= MIN_ACCOUNT_LEN then
    begin
      Showmessage('帐户名称长度最低4个字符或2汉字');
      Exit;
    end;
  end;

  ModalResult := mrOK;
end;

end.

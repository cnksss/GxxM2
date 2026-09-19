unit uFrmMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, acsAPIClass, acsUtils, StdCtrls, acsParams, SendSmsRequest;

type
  TForm1 = class(TForm)
    btn1: TButton;
    Memo1: TMemo;
    Edit1: TEdit;
    Edit2: TEdit;
    Memo2: TMemo;
    procedure btn1Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.dfm}

procedure TForm1.btn1Click(Sender: TObject);
var
  Sms: TSendSmsRequest;
  ErrorMsg: string;
begin
  g_AcsUtil.KeyID := 'LTAI63GJtkWaeX3r';
  g_AcsUtil.KeySecret := 'FWTVlj9oFfRm2VfsodNNAk6MNqroJF';

  Sms := TSendSmsRequest.Create;
  try
    Sms.SignName := 'GEE单职业';
    Sms.TemplateCode := 'SMS_62600003';

    if Sms.Request('13607110319', '{"name":"欢哥", "code":"4445555"}', ErrorMsg) then
    begin
      ShowMessage('发送短信成功')
    end
    else
    begin
      Showmessage('发送短信失败' + sLineBreak + sLineBreak + ErrorMsg);
    end;

    Edit1.Text := sms.GetParam('Timestamp');
    Edit2.Text := sms.GetParam('SignatureNonce');
    Memo1.Text := g_SignStr;
    Memo2.Text := g_RequestStr;

    
  finally
    Sms.Free;
  end;
end;

end.

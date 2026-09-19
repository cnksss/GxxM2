program SendSMS;

uses
  Forms,
  uFrmMain in 'uFrmMain.pas' {Form1},
  acsAPIClass in '..\acsAPIClass.pas',
  acsCommon in '..\acsCommon.pas',
  acsError in '..\acsError.pas',
  acsParams in '..\acsParams.pas',
  acsUtils in '..\acsUtils.pas',
  superobject in '..\superobject.pas',
  uMD5 in '..\uMD5.pas',
  uWinHttp in '..\uWinHttp.pas',
  flcHash in '..\flcHash.pas',
  SendSmsRequest in '..\SendSmsRequest.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.

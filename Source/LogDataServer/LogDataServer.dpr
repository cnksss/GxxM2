program LogDataServer;

uses
  Forms,
  LogDataMain in 'LogDataMain.pas' {FrmLogData},
  LDShare in 'LDShare.pas',
  LogManage in 'LogManage.pas' {FrmLogManage},
  HUtil32 in '..\Common\HUtil32.pas',
  Grobal2 in '..\Common\Grobal2.pas',
  Common in '..\Common\Common.pas',
  ThreadPool in 'ThreadPool.pas',
  FileSearchPool in 'FileSearchPool.pas',
  uFrmRemoteQuerySetting in 'uFrmRemoteQuerySetting.pas' {FrmRemoteQuerySetting};

{$R *.res}

begin
  Application.MainFormOnTaskBar := True;
  Application.Initialize;
  Application.Title := '日志服务器';
  Application.CreateForm(TFrmLogData, FrmLogData);
  Application.CreateForm(TFrmLogManage, FrmLogManage);
  Application.Run;
end.

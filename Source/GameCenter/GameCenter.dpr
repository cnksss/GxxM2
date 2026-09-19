program GameCenter;



uses
  Forms,
  GMain in 'GMain.pas' {frmMain},
  GShare in 'GShare.pas',
  HUtil32 in '..\Common\HUtil32.pas',
  Grobal2 in '..\Common\Grobal2.pas',
  GLoginServer in 'GLoginServer.pas' {frmLoginServerConfig},
  Common in '..\Common\Common.pas',
  EDcode in '..\Common\EDcode.pas',
  GHeroDB in 'GHeroDB.pas',
  GHeroDBConfig in 'GHeroDBConfig.pas' {FrmHeroDB},
  CheckPrevious in 'CheckPrevious.pas',
  DataBackUp in 'DataBackUp.pas',
  uSqliteDB in 'uSqliteDB.pas',
  GBDEtoSqlite in 'GBDEtoSqlite.pas' {FrmBDEToSqlite};

{$R *.res}

begin
  if not CheckPrevious.RestoreIfRunning(Application.Handle, 1) then begin
    Application.Initialize;
    Application.Title := 'ÒýÇæ¿ØÖÆÌ¨';
    Application.HintPause := 100;
    Application.HintShortPause := 100;
    Application.HintHidePause := 15000;
    Application.CreateForm(TfrmMain, frmMain);
  Application.CreateForm(TfrmLoginServerConfig, frmLoginServerConfig);
  Application.CreateForm(TFrmBDEToSqlite, FrmBDEToSqlite);
  Application.Run;
  end;
end.


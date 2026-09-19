program DBServer;

uses
  Forms,
  SysUtils,
  uFrmMain in 'uFrmMain.pas' {FrmMain},
  DBShare in 'DBShare.pas',
  SelectClient in 'SelectClient.pas',
  IDSocCli in 'IDSocCli.pas' {FrmIDSoc},
  ServerClient in 'ServerClient.pas',
  SDK in '..\Common\SDK.pas',
  Ranking in 'Ranking.pas' {FrmRankingDlg},
  uFrmDataManager in '..\LoginSrv\uFrmDataManager.pas' {FrmDataManager},
  CreateChr in 'CreateChr.pas' {FrmCreateChr},
  uFrmRoleDataEdit in 'uFrmRoleDataEdit.pas' {FrmRoleDataEdit},
  RouteEdit in 'RouteEdit.pas' {frmRouteEdit},
  RouteManage in 'RouteManage.pas' {frmRouteManage},
  TestSelGate in 'TestSelGate.pas' {frmTestSelGate},
  Grobal2 in '..\Common\Grobal2.pas',
  EDcode in '..\Common\EDcode.pas',
  Setting in 'Setting.pas' {FrmSetting},
  uFrmHumanExport in 'uFrmHumanExport.pas' {FrmHumanExport},
  HUtil32 in '..\Common\HUtil32.pas',
  uRunGateList in 'uRunGateList.pas',
  RoleDB in 'RoleDB.pas',
  SqliteRoleDB in 'SqliteRoleDB.pas',
  MySqlRoleDB in 'MySqlRoleDB.pas';

{$R *.res}
{$R RoleData.RES }
begin
  Application.MainFormOnTaskBar := True;
  Application.Initialize;
  Application.Title := '数据库服务器';
  Application.CreateForm(TFrmMain, FrmMain);
  Application.CreateForm(TFrmIDSoc, FrmIDSoc);
  Application.CreateForm(TFrmCreateChr, FrmCreateChr);
  Application.CreateForm(TfrmRouteManage, frmRouteManage);
  Application.CreateForm(TFrmRankingDlg, FrmRankingDlg);
  Application.Run;
end.


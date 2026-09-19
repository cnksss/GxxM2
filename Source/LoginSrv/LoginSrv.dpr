program LoginSrv;

uses
  Forms,
  GateSet in 'GateSet.pas' {FrmGateSetting},
  MasSock in 'MasSock.pas' {FrmMasSoc},
  EditUserInfo in 'EditUserInfo.pas' {FrmUserInfoEdit},
  FrmFindId in 'FrmFindId.pas' {FrmFindUserId},
  FAccountView in 'FAccountView.pas' {FrmAccountView},
  LMain in 'LMain.pas' {FrmMain},
  MonSoc in 'MonSoc.pas' {FrmMonSoc},
  LSShare in 'LSShare.pas',
  Parse in 'Parse.pas',
  Grobal2 in '..\Common\Grobal2.pas',
  HUtil32 in '..\Common\HUtil32.pas',
  MudUtil in '..\Common\MudUtil.pas',
  GrobalSession in 'GrobalSession.pas' {frmGrobalSession},
  EDcode in '..\Common\EDCode.pas',
  SDK in '..\Common\SDK.pas',
  BasicSet in 'BasicSet.pas' {FrmBasicSet},
  Common in '..\Common\Common.pas',
  EncryptUnit in '..\Common\EncryptUnit.pas',
  AccountDB in 'AccountDB.pas',
  SqliteAccountDB in 'SqliteAccountDB.pas',
  uFrmBatchEditAccountInfo in 'uFrmBatchEditAccountInfo.pas' {FrmBatchEditAccountInfo},
  VerifyCodeUtils in 'VerifyCodeUtils.pas',
  MySqlAccountDB in 'MySqlAccountDB.pas';

{$R *.RES}

begin
  Application.MainFormOnTaskBar := True;
  Application.Initialize;
  Application.Title := 'µÇÂ¼·þÎñÆ÷';
  Application.CreateForm(TFrmMain, FrmMain);
  Application.CreateForm(TFrmMasSoc, FrmMasSoc);
  Application.CreateForm(TFrmUserInfoEdit, FrmUserInfoEdit);
  Application.CreateForm(TFrmFindUserId, FrmFindUserId);
  Application.CreateForm(TFrmAccountView, FrmAccountView);
  Application.CreateForm(TFrmMonSoc, FrmMonSoc);
  Application.Run;
end.


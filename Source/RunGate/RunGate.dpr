program RunGate;

uses
  Fastmm4,
  Forms,
  uFrmMain in 'uFrmMain.pas' {FrmMain},
  RunGateUtils in 'RunGateUtils.pas',
  MirClientContext in 'MirClientContext.pas',
  uFrmMessageFilter in 'uFrmMessageFilter.pas' {FrmMessageFilter},
  uFrmSafeFilter in 'uFrmSafeFilter.pas' {FrmSafeFilter},
  uFrmGameSpeed in 'uFrmGameSpeed.pas' {FrmGameSpeed},
  GateShare in 'GateShare.pas',
  uFrmReadFileIP in 'uFrmReadFileIP.pas' {FrmReadFileIP},
  uIPDownThread in 'uIPDownThread.pas',
  uFrmInterval in 'uFrmInterval.pas' {FrmInterval},
  Grobal2_Ex in 'Grobal2_Ex.pas',
  EDcode in 'EDcode.pas',
  EncryptUnit_LF in 'EncryptUnit_LF.pas',
  uFrmProcessBlacklist in 'uFrmProcessBlacklist.pas' {FrmProcessBlacklist},
  uFrmAddProcessBlack in 'uFrmAddProcessBlack.pas' {FrmAddProcessBlack},
  uFrmMagicCD in 'uFrmMagicCD.pas' {FrmMagicCD},
  MagicIntervalUtils in 'MagicIntervalUtils.pas',
  uFrmItemEatCD in 'uFrmItemEatCD.pas' {FrmItemEatCD},
  BagItemList in 'BagItemList.pas',
  uFrmLogClientPacketSetting in 'uFrmLogClientPacketSetting.pas' {FrmLogClientPacketSetting},
  RunGatePluginInterface in 'RunGatePluginInterface.pas',
  DesNew2 in 'DesNew2.pas';

{$R *.res}

begin
  Application.MainFormOnTaskBar := True;
  Application.Initialize;
  Application.HintPause := 0;
  Application.HintShortPause := 0;
  Application.HintHidePause := 5000;
  Application.CreateForm(TFrmMain, FrmMain);
  Application.Run;
end.

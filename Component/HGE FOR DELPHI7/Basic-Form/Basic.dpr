program Basic;

uses
  Forms,
  Main in 'Main.pas' {FrmMain},
  HGE in '..\Source\HGE.pas',
  HGECanvas in '..\Source\HGECanvas.pas',
  HGEImages in '..\Source\HGEImages.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TFrmMain, FrmMain);
  Application.Run;
end.

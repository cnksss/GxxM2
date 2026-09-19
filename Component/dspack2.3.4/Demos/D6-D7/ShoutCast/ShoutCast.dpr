program ShoutCast;

uses
  Forms,
  Main in 'Main.pas' {Form1},
  Definitions in 'Definitions.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.


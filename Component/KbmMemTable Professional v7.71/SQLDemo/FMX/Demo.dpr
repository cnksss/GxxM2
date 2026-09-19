program Demo;

uses
  FMX.Forms,
  uDemo in 'uDemo.pas' {Form1};

{$R *.res}

begin
{$IFDEF MACOS}
   FMX.Types.GlobalUseHWEffects:=false;
{$ENDIF}

  Application.Initialize;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.

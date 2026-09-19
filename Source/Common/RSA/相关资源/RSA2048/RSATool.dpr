program RSATool;

uses
  Forms,
  RSAToolMain in 'RSAToolMain.pas' {Form1};

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.

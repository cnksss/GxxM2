program pSimpleDemo;

uses
  Forms,
  uSimpleDemo in 'uSimpleDemo.pas' {frmDemo},
  uFormatRichEditText in 'uFormatRichEditText.pas';

{$R *.RES}

begin
  Application.Initialize;
  Application.CreateForm(TfrmDemo, frmDemo);
  Application.Run;
end.

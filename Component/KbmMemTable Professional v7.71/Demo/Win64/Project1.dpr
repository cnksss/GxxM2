program Project1;

{%ToDo 'Project1.todo'}

uses
  Forms,
  Unit1 in 'Unit1.pas' {Form1},
  kbmCompress in 'kbmCompress.pas';

{$R *.RES}

begin
  Application.Initialize;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.


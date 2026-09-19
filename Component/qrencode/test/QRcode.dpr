program QRcode;

uses
  Forms,
  frmuQRcode in 'frmuQRcode.pas' {frmQRcode},
  bitstream in '..\bitstream.pas',
  qrmask in '..\qrmask.pas',
  mmask in '..\mmask.pas',
  mqrspec in '..\mqrspec.pas',
  qrenc in '..\qrenc.pas',
  qrencode in '..\qrencode.pas',
  qrinput in '..\qrinput.pas',
  qrspec in '..\qrspec.pas',
  rscode in '..\rscode.pas',
  split in '..\split.pas',
  struct in '..\struct.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TfrmQRcode, frmQRcode);
  Application.Run;
end.

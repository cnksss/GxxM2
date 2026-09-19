program GuiEdit;

uses
  FastMM4,
  Forms,
  Controls,
  Share in 'Share.pas',
  Structure in 'Structure.pas' {StructureDlg},
  Objects in 'Objects.pas' {ObjectsDlg},
  DxPopupMenu in 'DxPopupMenu.pas',
  DxComponents in 'DxComponents.pas',
  DxControls in 'DxControls.pas',
  DxEdit in 'DxEdit.pas',
  DxImageButton in 'DxImageButton.pas',
  DxImageForm in 'DxImageForm.pas',
  DxImageGrid in 'DxImageGrid.pas',
  DxLabel in 'DxLabel.pas',
  DxMemo in 'DxMemo.pas',
  DxPageControl in 'DxPageControl.pas',
  DxComboBox in 'DxComboBox.pas',
  LoginDlg in 'LoginDlg.pas' {FrmLogin},
  StreamClipbrd in 'StreamClipbrd.pas',
  DxCanvas in 'DxCanvas.pas',
  DxLine in 'DxLine.pas',
  Main in 'Main.pas' {FrmMain},
  TextureImages in '..\..\..\Component\HGE FOR DELPHI7\Source\TextureImages.pas',
  GameImages in '..\ReadResources\GameImages.pas',
  Pak in '..\ReadResources\Pak.pas',
  Wil in '..\ReadResources\Wil.pas',
  Wis in '..\ReadResources\Wis.pas',
  Wzl in '..\ReadResources\Wzl.pas',
  UnitDes in '..\..\Common\UnitDes.pas';

{$R *.res}

begin
  FrmLogin := TFrmLogin.Create(nil);
  if FrmLogin.ShowModal = mrYes then begin
    FrmLogin.Free;
    Application.Initialize;
    Application.CreateForm(TFrmMain, FrmMain);
  Application.Run;
  end else FrmLogin.Free;
end.


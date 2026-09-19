unit uFrmCustomMagicCopySetting;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Grobal2, uCustomMagicUtils;

type
  TFrmCustomMagicCopySetting = class(TForm)
    grp1: TGroupBox;
    lbl1: TLabel;
    edtSource: TEdit;
    lbl2: TLabel;
    cbbDest: TComboBox;
    btnOK: TButton;
    Label1: TLabel;
    btnCancel: TButton;
    procedure btnOKClick(Sender: TObject);
  private
    { Private declarations }
    FDestLevel: TMagicPlusLevel;
  public
    { Public declarations }
  end;

  function ShowCustomMagicCopySetting(Source: TMagicPlusLevel; var Dest: TMagicPlusLevel): Boolean;

implementation

{$R *.dfm}

function ShowCustomMagicCopySetting(Source: TMagicPlusLevel; var Dest: TMagicPlusLevel): Boolean;
var
  FrmCustomMagicCopySetting: TFrmCustomMagicCopySetting;
  PlusLevel: TMagicPlusLevel;
begin
  FrmCustomMagicCopySetting := TFrmCustomMagicCopySetting.Create(nil);
  try
    FrmCustomMagicCopySetting.edtSource.Text := MagicPlusLevelNames[Source];

    FrmCustomMagicCopySetting.cbbDest.Items.Clear;
    for PlusLevel := Low(TMagicPlusLevel) to High(TMagicPlusLevel) do
    begin
      if PlusLevel <> Source then
      begin
        FrmCustomMagicCopySetting.cbbDest.Items.AddObject(MagicPlusLevelNames[PlusLevel], TObject(PlusLevel));
      end;
    end;

    FrmCustomMagicCopySetting.cbbDest.ItemIndex := 0;

    Result := FrmCustomMagicCopySetting.ShowModal = mrOk;
    if Result then
    begin
      Dest := FrmCustomMagicCopySetting.FDestLevel;
    end;
  finally
    FrmCustomMagicCopySetting.Free;
  end;
end;

procedure TFrmCustomMagicCopySetting.btnOKClick(Sender: TObject);
begin
  if cbbDest.ItemIndex < 0 then
  begin
    Application.MessageBox('请先指定目标配置', '提示', MB_OK or MB_ICONINFORMATION);
    cbbDest.SetFocus;
    Exit;
  end;

  FDestLevel := TMagicPlusLevel(cbbDest.Items.Objects[cbbDest.ItemIndex]);
  ModalResult := mrOk;
end;

end.

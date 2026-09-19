unit uFrmNGItemEdit;

interface

uses
  Windows,
  Messages,
  SysUtils,
  Variants,
  Classes,
  Graphics,
  Controls,
  Forms,
  Dialogs,
  StdCtrls;

type
  TFrmNGItemEdit = class(TForm)
    mmoItems:TMemo;
    btnOK:TButton;
  private
    { Private declarations }
  public
    { Public declarations }
  end;

function ShowFrmNGItemEdit(Items:TStrings):Boolean;

implementation

{$R *.dfm}

function ShowFrmNGItemEdit(Items:TStrings):Boolean;
var
  FrmNGItemEdit:TFrmNGItemEdit;
begin
  //Result := False; HZQ 20230519
  FrmNGItemEdit := TFrmNGItemEdit.Create(nil);
  try
    FrmNGItemEdit.mmoItems.Text := Items.Text;
    Result := FrmNGItemEdit.ShowModal = mrOk;
    if Result then
      Items.Text := FrmNGItemEdit.mmoItems.Text;
  finally
    FrmNGItemEdit.Free;
  end;
end;

end.

unit RSAToolMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, LbRSA, LbAsym, ComCtrls;

type
  TForm1 = class(TForm)
    GroupBox1: TGroupBox;
    Label1: TLabel;
    Edit1: TEdit;
    Label2: TLabel;
    Edit2: TEdit;
    GroupBox2: TGroupBox;
    Label3: TLabel;
    Edit3: TEdit;
    Edit4: TEdit;
    Label4: TLabel;
    Button1: TButton;
    Button2: TButton;
    Label5: TLabel;
    ComboBox1: TComboBox;
    StatusBar1: TStatusBar;
    procedure Button2Click(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure Button1Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.dfm}

procedure TForm1.Button2Click(Sender: TObject);
begin
  Close;
end;

procedure TForm1.FormCreate(Sender: TObject);
begin
  Combobox1.ItemIndex:=0; 
end;

procedure TForm1.Button1Click(Sender: TObject);
var
  Str:TLbRSA;
begin
  Str:=TLbRSA.Create(nil);
  Screen.Cursor := crHourglass;
  try
    Str.PrimeTestIterations := StrToIntDef('20', 20);
    Str.KeySize := TLbAsymKeySize(Combobox1.ItemIndex);
    Str.GenerateKeyPair;
    Edit2.Text  := Str.PublicKey.ExponentAsString;
    Edit1.Text  := Str.PublicKey.ModulusAsString;
    Edit4.Text := Str.PrivateKey.ExponentAsString;
    Edit3.Text := Str.PrivateKey.ModulusAsString;
  finally
    Str.Free;
    Screen.Cursor := crDefault;
  end;
end;

end.

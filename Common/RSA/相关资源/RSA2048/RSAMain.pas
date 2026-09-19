unit RSAMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, ComCtrls, LbRSA, LbAsym;

type
  TForm1 = class(TForm)
    Label1: TLabel;
    MText: TMemo;
    Label2: TLabel;
    KeyText1: TEdit;
    Label3: TLabel;
    KeyText2: TEdit;
    Label4: TLabel;
    ModText: TEdit;
    Label5: TLabel;
    CText: TMemo;
    Label6: TLabel;
    PText: TMemo;
    Label7: TLabel;
    ComboBox1: TComboBox;
    Button1: TButton;
    Button2: TButton;
    StatusBar1: TStatusBar;
    chk1: TCheckBox;
    Button3: TButton;
    procedure FormCreate(Sender: TObject);
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.dfm}


var
  MyArray:array[0..$00DF] of Byte =(
		$ee, $64, $5f, $93, $da, $08, $9d, $58, $1d, $e3, $1c, $fe, $03, $5b, $bf, $3b,
		$bc, $32, $7c, $b5, $36, $95, $e0, $ff, $0c, $9e, $d6, $36, $22, $cf, $f2, $46,
		$2b, $09, $d7, $a6, $03, $09, $9e, $8c, $4c, $08, $64, $81, $ef, $3b, $cc, $49,
		$19, $9f, $c7, $7d, $6a, $7b, $c9, $96, $88, $74, $f9, $28, $e9, $08, $94, $3c,
		$7c, $d2, $cb, $c2, $cd, $3e, $46, $81, $d3, $68, $d9, $dc, $1c, $e2, $b7, $bb,
		$3b, $f4, $d4, $3d, $bf, $a4, $07, $72, $6e, $5a, $3d, $22, $ad, $8f, $05, $2f,
		$56, $bf, $69, $a5, $de, $49, $86, $e2, $36, $56, $c5, $a1, $fd, $cc, $fc, $db,
		$e9, $37, $6b, $f0, $e8, $8f, $46, $b3, $04, $67, $df, $5d, $c8, $45, $05, $01,
		$ad, $37, $8c, $dd, $6d, $78, $0a, $1d, $9e, $5a, $99, $95, $61, $99, $74, $5d,
		$9a, $f6, $8b, $ff, $50, $f0, $e4, $4b, $7a, $2e, $a7, $1a, $1f, $df, $ec, $d7,
		$de, $39, $49, $b8, $3b, $0b, $a3, $53, $b8, $ec, $01, $1b, $0f, $2a, $34, $21,
		$fd, $fc, $c3, $d4, $9b, $81, $ef, $31, $03, $e2, $9c, $f4, $89, $71, $19, $9d,
		$f5, $47, $cc, $98, $7c, $7e, $1a, $4f, $38, $9e, $46, $84, $91, $5a, $08, $b2,
		$a8, $09, $ea, $4b, $16, $b6, $13, $ec, $b1, $1f, $05, $d0, $47, $88, $2f, $48);


function StrToHex(InStr:String):String;
var
  StrResult,Temp:String;
  i:Integer;
begin
  StrResult:='';Temp:='';
  for I := 0 to Length(InStr) - 1 do
    begin
      Temp := Format('%x', [Ord(InStr[I + 1])]);
      if Length(Temp) = 1 then Temp := '0' + Temp;
        StrResult := StrResult + Temp;
    end;
  Result:=StrResult;
end;

function HexToInt(Hex: String): Integer;
var
  I, Res: Integer;
  ch: Char;
begin
  Res := 0;
  for I := 0 to Length(Hex) - 1 do
  begin
    ch := Hex[I + 1];
    if (ch >= '0') and (ch <= '9') then
      Res := Res * 16 + Ord(ch) - Ord('0')
    else if (ch >= 'A') and (ch <= 'F') then
      Res := Res * 16 + Ord(ch) - Ord('A') + 10
    else if (ch >= 'a') and (ch <= 'f') then
      Res := Res * 16 + Ord(ch) - Ord('a') + 10
    else raise Exception.Create('Error: not a Hex String');
  end;
  Result := Res;
end;

function RSAEncryption(InStr,ModStr,KeyStr:String;KeyBit:TLbAsymKeySize):AnsiString;
var
  RSA:TLbRSA;
  TempResult:AnsiString;
begin
  TempResult:='';
  RSA:=TLbRSA.Create(nil);
  if KeyBit=aks128 then//128λ
    RSA.KeySize:=aks128;
  if KeyBit=aks256 then//256λ
    RSA.KeySize:=aks256;
  if KeyBit=aks512 then//512λ
    RSA.KeySize:=aks512;
  if KeyBit=aks768 then//768λ
    RSA.KeySize:=aks768;
  if KeyBit=aks1024 then//1024λ
    RSA.KeySize:=aks1024;
  if KeyBit=aks2048 then//2048λ
    RSA.KeySize:=aks2048;
  RSA.PublicKey.ModulusAsString:=ModStr;
  RSA.PublicKey.ExponentAsString:=KeyStr;
  TempResult:= RSA.EncryptString(InStr);
  RSA.Free;
  Result:=StrToHex(TempResult);
end;

function RSADecryption(InStr,ModStr,KeyStr:String;PrivateKey:TLbAsymKeySize):AnsiString;
var
  Str, Temp: AnsiString;
  I: Integer;
  RSA:TLbRSA;
begin
  Str := '';
  for I := 0 to Length(InStr) div 2 - 1 do
  begin
    Temp := Copy(InStr, I * 2 + 1, 2);
    Str := Str + Chr(HexToInt(Temp));
  end;

  RSA:=TLbRSA.Create(nil);
  if PrivateKey=aks128 then//128λ
    RSA.KeySize := aks128;
  if PrivateKey=aks256 then//256λ
    RSA.KeySize := aks256;
  if PrivateKey=aks512 then//512λ
    RSA.KeySize := aks512;
  if PrivateKey=aks768 then//768λ
    RSA.KeySize := aks768;
  if PrivateKey=aks1024 then//1024λ
    RSA.KeySize := aks1024;
  if PrivateKey=aks2048 then//2048λ
    RSA.KeySize := aks2048;
  RSA.PrivateKey.ModulusAsString:=ModStr;
  RSA.PrivateKey.ExponentAsString:=KeyStr;
  Result:=RSA.DecryptString(Str);
end;

procedure TForm1.FormCreate(Sender: TObject);
begin
  Combobox1.ItemIndex:=0;
end;

procedure TForm1.Button1Click(Sender: TObject);
begin
  Case Combobox1.ItemIndex of
    0:
      CText.Text:=RSAEncryption(MText.Text,ModText.Text,KeyText2.Text,aks128);
    1:
      CText.Text:=RSAEncryption(MText.Text,ModText.Text,KeyText2.Text,aks256);
    2:
      CText.Text:=RSAEncryption(MText.Text,ModText.Text,KeyText2.Text,aks512);
    3:
      CText.Text:=RSAEncryption(MText.Text,ModText.Text,KeyText2.Text,aks768);
    4:
      CText.Text:=RSAEncryption(MText.Text,ModText.Text,KeyText2.Text,aks1024);
    5:
      CText.Text:=RSAEncryption(MText.Text,ModText.Text,KeyText2.Text,aks2048);
  end;
end;

procedure TForm1.Button2Click(Sender: TObject);
   var
     RSA:TLbRSA;
     vm,om:TMemoryStream;
     x:pByte;
begin
  {
         RSA := TLbRSA.Create(nil);
         RSA.PrivateKey.ModulusAsString := '7559B955E2049972AFEA1EE705C937E6';
         RSA.PublicKey.ModulusAsString := 'F518';
         vm := TMemoryStream.Create;

         om := TMemoryStream.Create;
         om.Position := 0;

         vm.Position := 0;
         vm.Write(MyArray,$E0);

         RSA.DecryptStream(vm,om);

         GetMem(x,$E0);
         FillChar(x^,$E0,0);
         om.Position := 0;
         om.Read(x^,40);
         
        ShowMessage('ok');

             }

  Case Combobox1.ItemIndex of
    0:
      PText.Text:=RSADecryption(CText.Text,ModText.Text,KeyText1.Text,aks128);
    1:
      PText.Text:=RSADecryption(CText.Text,ModText.Text,KeyText1.Text,aks256);
    2:
      PText.Text:=RSADecryption(CText.Text,ModText.Text,KeyText1.Text,aks512);
    3:
      PText.Text:=RSADecryption(CText.Text,ModText.Text,KeyText1.Text,aks768);
    4:
      PText.Text:=RSADecryption(CText.Text,ModText.Text,KeyText1.Text,aks1024);
    5:
      PText.Text:=RSADecryption(CText.Text,ModText.Text,KeyText1.Text,aks2048);
  end;
end;

procedure TForm1.Button3Click(Sender: TObject);
var
  I, Len, J: Integer;
  S: string;
begin
  for I := 0 to 100000 do
  begin
    Len := 10 + Random(1000);

    SetLength(S, Len);
    for J := 0 to Len - 1 do
    begin
      S[J] := Chr(65 + Random(26));
    end;

    MText.Text := S;

    Button1.Click;

    Button2.Click;

    if not SameText(MText.Text, PText.Text) then
    begin
      ShowMessage('aaa');
    end;

    Caption := IntToStr(I);
    Application.ProcessMessages;
       
  end;
end;

end.

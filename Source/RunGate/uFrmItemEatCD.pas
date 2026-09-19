unit uFrmItemEatCD;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, VirtualTrees, StdCtrls, Spin, SpinEditEx, IniFiles, GateShare;

type
  TFrmItemEatCD = class(TForm)
    grpHum: TGroupBox;
    grp2: TGroupBox;
    lbl1: TLabel;
    Label1: TLabel;
    Label2: TLabel;
    Label4: TLabel;
    seHumNormalHP0: TSpinEditEx;
    seHumNormalMP0: TSpinEditEx;
    seHumNormalHPMP0: TSpinEditEx;
    btnOK: TButton;
    seHumOther0: TSpinEditEx;
    Label30: TLabel;
    Label31: TLabel;
    Label32: TLabel;
    seHumSpecialHP0: TSpinEditEx;
    seHumSpecialMP0: TSpinEditEx;
    seHumSpecialHPMP0: TSpinEditEx;
    GroupBox1: TGroupBox;
    Label5: TLabel;
    Label6: TLabel;
    Label7: TLabel;
    Label9: TLabel;
    Label10: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    seHumNormalHP1: TSpinEditEx;
    seHumNormalMP1: TSpinEditEx;
    seHumNormalHPMP1: TSpinEditEx;
    seHumOther1: TSpinEditEx;
    seHumSpecialHP1: TSpinEditEx;
    seHumSpecialMP1: TSpinEditEx;
    seHumSpecialHPMP1: TSpinEditEx;
    GroupBox2: TGroupBox;
    Label13: TLabel;
    Label14: TLabel;
    Label33: TLabel;
    Label35: TLabel;
    Label36: TLabel;
    Label37: TLabel;
    Label38: TLabel;
    seHumNormalHP2: TSpinEditEx;
    seHumNormalMP2: TSpinEditEx;
    seHumNormalHPMP2: TSpinEditEx;
    seHumOther2: TSpinEditEx;
    seHumSpecialHP2: TSpinEditEx;
    seHumSpecialMP2: TSpinEditEx;
    seHumSpecialHPMP2: TSpinEditEx;
    grpHero: TGroupBox;
    GroupBox4: TGroupBox;
    Label3: TLabel;
    Label8: TLabel;
    Label15: TLabel;
    Label16: TLabel;
    Label17: TLabel;
    Label18: TLabel;
    Label19: TLabel;
    seHeroNormalHP0: TSpinEditEx;
    seHeroNormalMP0: TSpinEditEx;
    seHeroNormalHPMP0: TSpinEditEx;
    seHeroOther0: TSpinEditEx;
    seHeroSpecialHP0: TSpinEditEx;
    seHeroSpecialMP0: TSpinEditEx;
    seHeroSpecialHPMP0: TSpinEditEx;
    GroupBox5: TGroupBox;
    Label20: TLabel;
    Label21: TLabel;
    Label22: TLabel;
    Label23: TLabel;
    Label24: TLabel;
    Label25: TLabel;
    Label26: TLabel;
    seHeroNormalHP1: TSpinEditEx;
    seHeroNormalMP1: TSpinEditEx;
    seHeroNormalHPMP1: TSpinEditEx;
    seHeroOther1: TSpinEditEx;
    seHeroSpecialHP1: TSpinEditEx;
    seHeroSpecialMP1: TSpinEditEx;
    seHeroSpecialHPMP1: TSpinEditEx;
    GroupBox6: TGroupBox;
    Label27: TLabel;
    Label28: TLabel;
    Label29: TLabel;
    Label34: TLabel;
    Label39: TLabel;
    Label40: TLabel;
    Label41: TLabel;
    seHeroNormalHP2: TSpinEditEx;
    seHeroNormalMP2: TSpinEditEx;
    seHeroNormalHPMP2: TSpinEditEx;
    seHeroOther2: TSpinEditEx;
    seHeroSpecialHP2: TSpinEditEx;
    seHeroSpecialMP2: TSpinEditEx;
    seHeroSpecialHPMP2: TSpinEditEx;
    procedure btnOKClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

  function ShowFrmItemEatCD: Boolean;

implementation

{$R *.dfm}

function ShowFrmItemEatCD: Boolean;
var
  FrmItemEatCD: TFrmItemEatCD;
begin
  FrmItemEatCD := TFrmItemEatCD.Create(nil);
  try
    Result := FrmItemEatCD.ShowModal = mrOK;
  finally
    FrmItemEatCD.Free;
  end;
end;

procedure TFrmItemEatCD.btnOKClick(Sender: TObject);
var
  I: Integer;
  StrIndex: string;
  IniFile: TIniFile;
begin
  g_EatItemCDConfig.Hum[0].NormalHP := seHumNormalHP0.Value;
  g_EatItemCDConfig.Hum[0].NormalMP := seHumNormalMP0.Value;
  g_EatItemCDConfig.Hum[0].NormalHPMP := seHumNormalHPMP0.Value;
  g_EatItemCDConfig.Hum[0].SpecialHP := seHumSpecialHP0.Value;
  g_EatItemCDConfig.Hum[0].SpecialMP := seHumSpecialMP0.Value;
  g_EatItemCDConfig.Hum[0].SpecialHPMP := seHumSpecialHPMP0.Value;
  g_EatItemCDConfig.Hum[0].Other := seHumOther0.Value;

  g_EatItemCDConfig.Hum[1].NormalHP := seHumNormalHP1.Value;
  g_EatItemCDConfig.Hum[1].NormalMP := seHumNormalMP1.Value;
  g_EatItemCDConfig.Hum[1].NormalHPMP := seHumNormalHPMP1.Value;
  g_EatItemCDConfig.Hum[1].SpecialHP := seHumSpecialHP1.Value;
  g_EatItemCDConfig.Hum[1].SpecialMP := seHumSpecialMP1.Value;
  g_EatItemCDConfig.Hum[1].SpecialHPMP := seHumSpecialHPMP1.Value;
  g_EatItemCDConfig.Hum[1].Other := seHumOther1.Value;

  g_EatItemCDConfig.Hum[2].NormalHP := seHumNormalHP2.Value;
  g_EatItemCDConfig.Hum[2].NormalMP := seHumNormalMP2.Value;
  g_EatItemCDConfig.Hum[2].NormalHPMP := seHumNormalHPMP2.Value;
  g_EatItemCDConfig.Hum[2].SpecialHP := seHumSpecialHP2.Value;
  g_EatItemCDConfig.Hum[2].SpecialMP := seHumSpecialMP2.Value;
  g_EatItemCDConfig.Hum[2].SpecialHPMP := seHumSpecialHPMP2.Value;
  g_EatItemCDConfig.Hum[2].Other := seHumOther2.Value;

  g_EatItemCDConfig.Hero[0].NormalHP := seHeroNormalHP0.Value;
  g_EatItemCDConfig.Hero[0].NormalMP := seHeroNormalMP0.Value;
  g_EatItemCDConfig.Hero[0].NormalHPMP := seHeroNormalHPMP0.Value;
  g_EatItemCDConfig.Hero[0].SpecialHP := seHeroSpecialHP0.Value;
  g_EatItemCDConfig.Hero[0].SpecialMP := seHeroSpecialMP0.Value;
  g_EatItemCDConfig.Hero[0].SpecialHPMP := seHeroSpecialHPMP0.Value;
  g_EatItemCDConfig.Hero[0].Other := seHeroOther0.Value;

  g_EatItemCDConfig.Hero[1].NormalHP := seHeroNormalHP1.Value;
  g_EatItemCDConfig.Hero[1].NormalMP := seHeroNormalMP1.Value;
  g_EatItemCDConfig.Hero[1].NormalHPMP := seHeroNormalHPMP1.Value;
  g_EatItemCDConfig.Hero[1].SpecialHP := seHeroSpecialHP1.Value;
  g_EatItemCDConfig.Hero[1].SpecialMP := seHeroSpecialMP1.Value;
  g_EatItemCDConfig.Hero[1].SpecialHPMP := seHeroSpecialHPMP1.Value;
  g_EatItemCDConfig.Hero[1].Other := seHeroOther1.Value;

  g_EatItemCDConfig.Hero[2].NormalHP := seHeroNormalHP2.Value;
  g_EatItemCDConfig.Hero[2].NormalMP := seHeroNormalMP2.Value;
  g_EatItemCDConfig.Hero[2].NormalHPMP := seHeroNormalHPMP2.Value;
  g_EatItemCDConfig.Hero[2].SpecialHP := seHeroSpecialHP2.Value;
  g_EatItemCDConfig.Hero[2].SpecialMP := seHeroSpecialMP2.Value;
  g_EatItemCDConfig.Hero[2].SpecialHPMP := seHeroSpecialHPMP2.Value;
  g_EatItemCDConfig.Hero[2].Other := seHeroOther2.Value;

  IniFile := TIniFile.Create(g_sIniFileName);
  for I := 0 to Length(g_EatItemCDConfig.Hum) - 1 do
  begin
    StrIndex := IntToStr(I + 1);
    IniFile.WriteInteger('HumanItemEatCD', StrIndex + 'NormalHP', g_EatItemCDConfig.Hum[I].NormalHP);
    IniFile.WriteInteger('HumanItemEatCD', StrIndex + 'NormalMP', g_EatItemCDConfig.Hum[I].NormalMP);
    IniFile.WriteInteger('HumanItemEatCD', StrIndex + 'NormalHPMP', g_EatItemCDConfig.Hum[I].NormalHPMP);
    IniFile.WriteInteger('HumanItemEatCD', StrIndex + 'SpecialHP', g_EatItemCDConfig.Hum[I].SpecialHP);
    IniFile.WriteInteger('HumanItemEatCD', StrIndex + 'SpecialMP', g_EatItemCDConfig.Hum[I].SpecialMP);
    IniFile.WriteInteger('HumanItemEatCD', StrIndex + 'SpecialHPMP', g_EatItemCDConfig.Hum[I].SpecialHPMP);
    IniFile.WriteInteger('HumanItemEatCD', StrIndex + 'Other', g_EatItemCDConfig.Hum[I].Other);
  end;

  for I := 0 to Length(g_EatItemCDConfig.Hero) - 1 do
  begin
    StrIndex := IntToStr(I + 1);
    IniFile.WriteInteger('HeroItemEatCD', StrIndex + 'NormalHP', g_EatItemCDConfig.Hero[I].NormalHP);
    IniFile.WriteInteger('HeroItemEatCD', StrIndex + 'NormalMP', g_EatItemCDConfig.Hero[I].NormalMP);
    IniFile.WriteInteger('HeroItemEatCD', StrIndex + 'NormalHPMP', g_EatItemCDConfig.Hero[I].NormalHPMP);
    IniFile.WriteInteger('HeroItemEatCD', StrIndex + 'SpecialHP', g_EatItemCDConfig.Hero[I].SpecialHP);
    IniFile.WriteInteger('HeroItemEatCD', StrIndex + 'SpecialMP', g_EatItemCDConfig.Hero[I].SpecialMP);
    IniFile.WriteInteger('HeroItemEatCD', StrIndex + 'SpecialHPMP', g_EatItemCDConfig.Hero[I].SpecialHPMP);
    IniFile.WriteInteger('HeroItemEatCD', StrIndex + 'Other', g_EatItemCDConfig.Hero[I].Other);
  end;
  IniFile.Free;

  ModalResult := mrOK;
end;

procedure TFrmItemEatCD.FormCreate(Sender: TObject);
begin
  seHumNormalHP0.Value := g_EatItemCDConfig.Hum[0].NormalHP;
  seHumNormalMP0.Value := g_EatItemCDConfig.Hum[0].NormalMP;
  seHumNormalHPMP0.Value := g_EatItemCDConfig.Hum[0].NormalHPMP;
  seHumSpecialHP0.Value := g_EatItemCDConfig.Hum[0].SpecialHP;
  seHumSpecialMP0.Value := g_EatItemCDConfig.Hum[0].SpecialMP;
  seHumSpecialHPMP0.Value := g_EatItemCDConfig.Hum[0].SpecialHPMP;
  seHumOther0.Value := g_EatItemCDConfig.Hum[0].Other;

  seHumNormalHP1.Value := g_EatItemCDConfig.Hum[1].NormalHP;
  seHumNormalMP1.Value := g_EatItemCDConfig.Hum[1].NormalMP;
  seHumNormalHPMP1.Value := g_EatItemCDConfig.Hum[1].NormalHPMP;
  seHumSpecialHP1.Value := g_EatItemCDConfig.Hum[1].SpecialHP;
  seHumSpecialMP1.Value := g_EatItemCDConfig.Hum[1].SpecialMP;
  seHumSpecialHPMP1.Value := g_EatItemCDConfig.Hum[1].SpecialHPMP;
  seHumOther1.Value := g_EatItemCDConfig.Hum[1].Other;

  seHumNormalHP2.Value := g_EatItemCDConfig.Hum[2].NormalHP;
  seHumNormalMP2.Value := g_EatItemCDConfig.Hum[2].NormalMP;
  seHumNormalHPMP2.Value := g_EatItemCDConfig.Hum[2].NormalHPMP;
  seHumSpecialHP2.Value := g_EatItemCDConfig.Hum[2].SpecialHP;
  seHumSpecialMP2.Value := g_EatItemCDConfig.Hum[2].SpecialMP;
  seHumSpecialHPMP2.Value := g_EatItemCDConfig.Hum[2].SpecialHPMP;
  seHumOther2.Value := g_EatItemCDConfig.Hum[2].Other;

  seHeroNormalHP0.Value := g_EatItemCDConfig.Hero[0].NormalHP;
  seHeroNormalMP0.Value := g_EatItemCDConfig.Hero[0].NormalMP;
  seHeroNormalHPMP0.Value := g_EatItemCDConfig.Hero[0].NormalHPMP;
  seHeroSpecialHP0.Value := g_EatItemCDConfig.Hero[0].SpecialHP;
  seHeroSpecialMP0.Value := g_EatItemCDConfig.Hero[0].SpecialMP;
  seHeroSpecialHPMP0.Value := g_EatItemCDConfig.Hero[0].SpecialHPMP;
  seHeroOther0.Value := g_EatItemCDConfig.Hero[0].Other;

  seHeroNormalHP1.Value := g_EatItemCDConfig.Hero[1].NormalHP;
  seHeroNormalMP1.Value := g_EatItemCDConfig.Hero[1].NormalMP;
  seHeroNormalHPMP1.Value := g_EatItemCDConfig.Hero[1].NormalHPMP;
  seHeroSpecialHP1.Value := g_EatItemCDConfig.Hero[1].SpecialHP;
  seHeroSpecialMP1.Value := g_EatItemCDConfig.Hero[1].SpecialMP;
  seHeroSpecialHPMP1.Value := g_EatItemCDConfig.Hero[1].SpecialHPMP;
  seHeroOther1.Value := g_EatItemCDConfig.Hero[1].Other;

  seHeroNormalHP2.Value := g_EatItemCDConfig.Hero[2].NormalHP;
  seHeroNormalMP2.Value := g_EatItemCDConfig.Hero[2].NormalMP;
  seHeroNormalHPMP2.Value := g_EatItemCDConfig.Hero[2].NormalHPMP;
  seHeroSpecialHP2.Value := g_EatItemCDConfig.Hero[2].SpecialHP;
  seHeroSpecialMP2.Value := g_EatItemCDConfig.Hero[2].SpecialMP;
  seHeroSpecialHPMP2.Value := g_EatItemCDConfig.Hero[2].SpecialHPMP;
  seHeroOther2.Value := g_EatItemCDConfig.Hero[2].Other;
end;

end.

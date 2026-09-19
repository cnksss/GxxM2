unit uFrmCustomItemProperty;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, M2Share, ExtCtrls, ComCtrls;

type
  TFrmCustomItemProperty = class(TForm)
    pgcMain: TPageControl;
    tsBindAttr: TTabSheet;
    tsText: TTabSheet;
    lbl3: TLabel;
    chk01: TCheckBox;
    chk02: TCheckBox;
    chk03: TCheckBox;
    chk04: TCheckBox;
    chk05: TCheckBox;
    chk06: TCheckBox;
    chk07: TCheckBox;
    chk08: TCheckBox;
    chk09: TCheckBox;
    chk10: TCheckBox;
    chk11: TCheckBox;
    chk12: TCheckBox;
    chk13: TCheckBox;
    chk14: TCheckBox;
    chk15: TCheckBox;
    chk16: TCheckBox;
    chk17: TCheckBox;
    chk18: TCheckBox;
    chk19: TCheckBox;
    chk20: TCheckBox;
    chk21: TCheckBox;
    chk22: TCheckBox;
    chk23: TCheckBox;
    chk24: TCheckBox;
    chk25: TCheckBox;
    chk26: TCheckBox;
    chk27: TCheckBox;
    chk28: TCheckBox;
    chk29: TCheckBox;
    chk30: TCheckBox;
    chk31: TCheckBox;
    chk32: TCheckBox;
    chk33: TCheckBox;
    chk34: TCheckBox;
    chk35: TCheckBox;
    chk36: TCheckBox;
    chk37: TCheckBox;
    chk38: TCheckBox;
    chk39: TCheckBox;
    chk40: TCheckBox;
    chk41: TCheckBox;
    chk42: TCheckBox;
    chk43: TCheckBox;
    chk44: TCheckBox;
    chk45: TCheckBox;
    chk46: TCheckBox;
    chk47: TCheckBox;
    chk48: TCheckBox;
    chk49: TCheckBox;
    chk50: TCheckBox;
    chk51: TCheckBox;
    chk52: TCheckBox;
    chk53: TCheckBox;
    chk54: TCheckBox;
    chk55: TCheckBox;
    chk56: TCheckBox;
    chk57: TCheckBox;
    chk58: TCheckBox;
    chk59: TCheckBox;
    chk60: TCheckBox;
    edtShowName01: TEdit;
    edtShowName02: TEdit;
    edtShowName03: TEdit;
    edtShowName04: TEdit;
    edtShowName05: TEdit;
    edtShowName06: TEdit;
    edtShowName07: TEdit;
    edtShowName08: TEdit;
    edtShowName09: TEdit;
    edtShowName10: TEdit;
    edtShowName11: TEdit;
    edtShowName12: TEdit;
    edtShowName13: TEdit;
    edtShowName14: TEdit;
    edtShowName15: TEdit;
    edtShowName16: TEdit;
    edtShowName17: TEdit;
    edtShowName18: TEdit;
    edtShowName19: TEdit;
    edtShowName20: TEdit;
    edtShowName21: TEdit;
    edtShowName22: TEdit;
    edtShowName23: TEdit;
    edtShowName24: TEdit;
    edtShowName25: TEdit;
    edtShowName26: TEdit;
    edtShowName27: TEdit;
    edtShowName28: TEdit;
    edtShowName29: TEdit;
    edtShowName30: TEdit;
    edtShowName31: TEdit;
    edtShowName32: TEdit;
    edtShowName33: TEdit;
    edtShowName34: TEdit;
    edtShowName35: TEdit;
    edtShowName36: TEdit;
    edtShowName37: TEdit;
    edtShowName38: TEdit;
    edtShowName39: TEdit;
    edtShowName40: TEdit;
    edtShowName41: TEdit;
    edtShowName42: TEdit;
    edtShowName43: TEdit;
    edtShowName44: TEdit;
    edtShowName45: TEdit;
    edtShowName46: TEdit;
    edtShowName47: TEdit;
    edtShowName48: TEdit;
    edtShowName49: TEdit;
    edtShowName50: TEdit;
    edtShowName51: TEdit;
    edtShowName52: TEdit;
    edtShowName53: TEdit;
    edtShowName54: TEdit;
    edtShowName55: TEdit;
    edtShowName56: TEdit;
    edtShowName57: TEdit;
    edtShowName58: TEdit;
    edtShowName59: TEdit;
    edtShowName60: TEdit;
    pnlBottom1: TPanel;
    lbl1: TLabel;
    btnOK: TButton;
    pnlBottom2: TPanel;
    lbl2: TLabel;
    btnOK2: TButton;
    mmoVar: TMemo;
    lblLineNum: TLabel;
    procedure FormCreate(Sender: TObject);
    procedure btnOKClick(Sender: TObject);
    procedure chk01Click(Sender: TObject);
    procedure edtShowName01Change(Sender: TObject);
    procedure mmoVarChange(Sender: TObject);
    procedure btnOK2Click(Sender: TObject);
    procedure mmoVarKeyUp(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure mmoVarMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

  procedure ShowFrmCustomItemProperty;

implementation

{$R *.dfm}

procedure ShowFrmCustomItemProperty;
var
  FrmCustomItemProperty: TFrmCustomItemProperty;
begin
  if not boStartReady then Exit;
  FrmCustomItemProperty := TFrmCustomItemProperty.Create(nil);
  try
    FrmCustomItemProperty.ShowModal;
  finally
    FrmCustomItemProperty.Free;
  end;
end;

procedure TFrmCustomItemProperty.FormCreate(Sender: TObject);
begin
  chk01.Checked := g_CustomItemPropertyChecks[01];
  chk02.Checked := g_CustomItemPropertyChecks[02];
  chk03.Checked := g_CustomItemPropertyChecks[03];
  chk04.Checked := g_CustomItemPropertyChecks[04];
  chk05.Checked := g_CustomItemPropertyChecks[05];
  chk06.Checked := g_CustomItemPropertyChecks[06];
  chk07.Checked := g_CustomItemPropertyChecks[07];
  chk08.Checked := g_CustomItemPropertyChecks[08];
  chk09.Checked := g_CustomItemPropertyChecks[09];
  chk10.Checked := g_CustomItemPropertyChecks[10];
  chk11.Checked := g_CustomItemPropertyChecks[11];
  chk12.Checked := g_CustomItemPropertyChecks[12];
  chk13.Checked := g_CustomItemPropertyChecks[13];
  chk14.Checked := g_CustomItemPropertyChecks[14];
  chk15.Checked := g_CustomItemPropertyChecks[15];
  chk16.Checked := g_CustomItemPropertyChecks[16];
  chk17.Checked := g_CustomItemPropertyChecks[17];
  chk18.Checked := g_CustomItemPropertyChecks[18];
  chk19.Checked := g_CustomItemPropertyChecks[19];
  chk20.Checked := g_CustomItemPropertyChecks[20];
  chk21.Checked := g_CustomItemPropertyChecks[21];
  chk22.Checked := g_CustomItemPropertyChecks[22];
  chk23.Checked := g_CustomItemPropertyChecks[23];
  chk24.Checked := g_CustomItemPropertyChecks[24];
  chk25.Checked := g_CustomItemPropertyChecks[25];
  chk26.Checked := g_CustomItemPropertyChecks[26];
  chk27.Checked := g_CustomItemPropertyChecks[27];
  chk28.Checked := g_CustomItemPropertyChecks[28];
  chk29.Checked := g_CustomItemPropertyChecks[29];
  chk30.Checked := g_CustomItemPropertyChecks[30];
  chk31.Checked := g_CustomItemPropertyChecks[31];
  chk32.Checked := g_CustomItemPropertyChecks[32];
  chk33.Checked := g_CustomItemPropertyChecks[33];
  chk34.Checked := g_CustomItemPropertyChecks[34];
  chk35.Checked := g_CustomItemPropertyChecks[35];
  chk36.Checked := g_CustomItemPropertyChecks[36];
  chk37.Checked := g_CustomItemPropertyChecks[37];
  chk38.Checked := g_CustomItemPropertyChecks[38];
  chk39.Checked := g_CustomItemPropertyChecks[39];
  chk40.Checked := g_CustomItemPropertyChecks[40];
  chk41.Checked := g_CustomItemPropertyChecks[41];
  chk42.Checked := g_CustomItemPropertyChecks[42];
  chk43.Checked := g_CustomItemPropertyChecks[43];
  chk44.Checked := g_CustomItemPropertyChecks[44];
  chk45.Checked := g_CustomItemPropertyChecks[45];
  chk46.Checked := g_CustomItemPropertyChecks[46];
  chk47.Checked := g_CustomItemPropertyChecks[47];
  chk48.Checked := g_CustomItemPropertyChecks[48];
  chk49.Checked := g_CustomItemPropertyChecks[49];
  chk50.Checked := g_CustomItemPropertyChecks[50];
  chk51.Checked := g_CustomItemPropertyChecks[51];
  chk52.Checked := g_CustomItemPropertyChecks[52];
  chk53.Checked := g_CustomItemPropertyChecks[53];
  chk54.Checked := g_CustomItemPropertyChecks[54];
  chk55.Checked := g_CustomItemPropertyChecks[55];
  chk56.Checked := g_CustomItemPropertyChecks[56];
  chk57.Checked := g_CustomItemPropertyChecks[57];
  chk58.Checked := g_CustomItemPropertyChecks[58];
  chk59.Checked := g_CustomItemPropertyChecks[59];
  chk60.Checked := g_CustomItemPropertyChecks[60];

  edtShowName01.Text := g_CustomItemPropertyBindNames[01];
  edtShowName02.Text := g_CustomItemPropertyBindNames[02];
  edtShowName03.Text := g_CustomItemPropertyBindNames[03];
  edtShowName04.Text := g_CustomItemPropertyBindNames[04];
  edtShowName05.Text := g_CustomItemPropertyBindNames[05];
  edtShowName06.Text := g_CustomItemPropertyBindNames[06];
  edtShowName07.Text := g_CustomItemPropertyBindNames[07];
  edtShowName08.Text := g_CustomItemPropertyBindNames[08];
  edtShowName09.Text := g_CustomItemPropertyBindNames[09];
  edtShowName10.Text := g_CustomItemPropertyBindNames[10];
  edtShowName11.Text := g_CustomItemPropertyBindNames[11];
  edtShowName12.Text := g_CustomItemPropertyBindNames[12];
  edtShowName13.Text := g_CustomItemPropertyBindNames[13];
  edtShowName14.Text := g_CustomItemPropertyBindNames[14];
  edtShowName15.Text := g_CustomItemPropertyBindNames[15];
  edtShowName16.Text := g_CustomItemPropertyBindNames[16];
  edtShowName17.Text := g_CustomItemPropertyBindNames[17];
  edtShowName18.Text := g_CustomItemPropertyBindNames[18];
  edtShowName19.Text := g_CustomItemPropertyBindNames[19];
  edtShowName20.Text := g_CustomItemPropertyBindNames[20];
  edtShowName21.Text := g_CustomItemPropertyBindNames[21];
  edtShowName22.Text := g_CustomItemPropertyBindNames[22];
  edtShowName23.Text := g_CustomItemPropertyBindNames[23];
  edtShowName24.Text := g_CustomItemPropertyBindNames[24];
  edtShowName25.Text := g_CustomItemPropertyBindNames[25];
  edtShowName26.Text := g_CustomItemPropertyBindNames[26];
  edtShowName27.Text := g_CustomItemPropertyBindNames[27];
  edtShowName28.Text := g_CustomItemPropertyBindNames[28];
  edtShowName29.Text := g_CustomItemPropertyBindNames[29];
  edtShowName30.Text := g_CustomItemPropertyBindNames[30];
  edtShowName31.Text := g_CustomItemPropertyBindNames[31];
  edtShowName32.Text := g_CustomItemPropertyBindNames[32];
  edtShowName33.Text := g_CustomItemPropertyBindNames[33];
  edtShowName34.Text := g_CustomItemPropertyBindNames[34];
  edtShowName35.Text := g_CustomItemPropertyBindNames[35];
  edtShowName36.Text := g_CustomItemPropertyBindNames[36];
  edtShowName37.Text := g_CustomItemPropertyBindNames[37];
  edtShowName38.Text := g_CustomItemPropertyBindNames[38];
  edtShowName39.Text := g_CustomItemPropertyBindNames[39];
  edtShowName40.Text := g_CustomItemPropertyBindNames[40];
  edtShowName41.Text := g_CustomItemPropertyBindNames[41];
  edtShowName42.Text := g_CustomItemPropertyBindNames[42];
  edtShowName43.Text := g_CustomItemPropertyBindNames[43];
  edtShowName44.Text := g_CustomItemPropertyBindNames[44];
  edtShowName45.Text := g_CustomItemPropertyBindNames[45];
  edtShowName46.Text := g_CustomItemPropertyBindNames[46];
  edtShowName47.Text := g_CustomItemPropertyBindNames[47];
  edtShowName48.Text := g_CustomItemPropertyBindNames[48];
  edtShowName49.Text := g_CustomItemPropertyBindNames[49];
  edtShowName50.Text := g_CustomItemPropertyBindNames[50];
  edtShowName51.Text := g_CustomItemPropertyBindNames[51];
  edtShowName52.Text := g_CustomItemPropertyBindNames[52];
  edtShowName53.Text := g_CustomItemPropertyBindNames[53];
  edtShowName54.Text := g_CustomItemPropertyBindNames[54];
  edtShowName55.Text := g_CustomItemPropertyBindNames[55];
  edtShowName56.Text := g_CustomItemPropertyBindNames[56];
  edtShowName57.Text := g_CustomItemPropertyBindNames[57];
  edtShowName58.Text := g_CustomItemPropertyBindNames[58];
  edtShowName59.Text := g_CustomItemPropertyBindNames[59];
  edtShowName60.Text := g_CustomItemPropertyBindNames[60];

  btnOK.Enabled := False;

  mmoVar.Text := g_CustomItemPropertyTextVarList.Text;
  btnOK2.Enabled := False;
end;

procedure TFrmCustomItemProperty.btnOKClick(Sender: TObject);
var
  OldCrc: LongWord;
  I: Integer;
begin
  g_CustomItemPropertyChecks[01] := chk01.Checked;
  g_CustomItemPropertyChecks[02] := chk02.Checked;
  g_CustomItemPropertyChecks[03] := chk03.Checked;
  g_CustomItemPropertyChecks[04] := chk04.Checked;
  g_CustomItemPropertyChecks[05] := chk05.Checked;
  g_CustomItemPropertyChecks[06] := chk06.Checked;
  g_CustomItemPropertyChecks[07] := chk07.Checked;
  g_CustomItemPropertyChecks[08] := chk08.Checked;
  g_CustomItemPropertyChecks[09] := chk09.Checked;
  g_CustomItemPropertyChecks[10] := chk10.Checked;
  g_CustomItemPropertyChecks[11] := chk11.Checked;
  g_CustomItemPropertyChecks[12] := chk12.Checked;
  g_CustomItemPropertyChecks[13] := chk13.Checked;
  g_CustomItemPropertyChecks[14] := chk14.Checked;
  g_CustomItemPropertyChecks[15] := chk15.Checked;
  g_CustomItemPropertyChecks[16] := chk16.Checked;
  g_CustomItemPropertyChecks[17] := chk17.Checked;
  g_CustomItemPropertyChecks[18] := chk18.Checked;
  g_CustomItemPropertyChecks[19] := chk19.Checked;
  g_CustomItemPropertyChecks[20] := chk20.Checked;
  g_CustomItemPropertyChecks[21] := chk21.Checked;
  g_CustomItemPropertyChecks[22] := chk22.Checked;
  g_CustomItemPropertyChecks[23] := chk23.Checked;
  g_CustomItemPropertyChecks[24] := chk24.Checked;
  g_CustomItemPropertyChecks[25] := chk25.Checked;
  g_CustomItemPropertyChecks[26] := chk26.Checked;
  g_CustomItemPropertyChecks[27] := chk27.Checked;
  g_CustomItemPropertyChecks[28] := chk28.Checked;
  g_CustomItemPropertyChecks[29] := chk29.Checked;
  g_CustomItemPropertyChecks[30] := chk30.Checked;
  g_CustomItemPropertyChecks[31] := chk31.Checked;
  g_CustomItemPropertyChecks[32] := chk32.Checked;
  g_CustomItemPropertyChecks[33] := chk33.Checked;
  g_CustomItemPropertyChecks[34] := chk34.Checked;
  g_CustomItemPropertyChecks[35] := chk35.Checked;
  g_CustomItemPropertyChecks[36] := chk36.Checked;
  g_CustomItemPropertyChecks[37] := chk37.Checked;
  g_CustomItemPropertyChecks[38] := chk38.Checked;
  g_CustomItemPropertyChecks[39] := chk39.Checked;
  g_CustomItemPropertyChecks[40] := chk40.Checked;
  g_CustomItemPropertyChecks[41] := chk41.Checked;
  g_CustomItemPropertyChecks[42] := chk42.Checked;
  g_CustomItemPropertyChecks[43] := chk43.Checked;
  g_CustomItemPropertyChecks[44] := chk44.Checked;
  g_CustomItemPropertyChecks[45] := chk45.Checked;
  g_CustomItemPropertyChecks[46] := chk46.Checked;
  g_CustomItemPropertyChecks[47] := chk47.Checked;
  g_CustomItemPropertyChecks[48] := chk48.Checked;
  g_CustomItemPropertyChecks[49] := chk49.Checked;
  g_CustomItemPropertyChecks[50] := chk50.Checked;
  g_CustomItemPropertyChecks[51] := chk51.Checked;
  g_CustomItemPropertyChecks[52] := chk52.Checked;
  g_CustomItemPropertyChecks[53] := chk53.Checked;
  g_CustomItemPropertyChecks[54] := chk54.Checked;
  g_CustomItemPropertyChecks[55] := chk55.Checked;
  g_CustomItemPropertyChecks[56] := chk56.Checked;
  g_CustomItemPropertyChecks[57] := chk57.Checked;
  g_CustomItemPropertyChecks[58] := chk58.Checked;
  g_CustomItemPropertyChecks[59] := chk59.Checked;
  g_CustomItemPropertyChecks[60] := chk60.Checked;

  g_CustomItemPropertyBindNames[01] := edtShowName01.Text;
  g_CustomItemPropertyBindNames[02] := edtShowName02.Text;
  g_CustomItemPropertyBindNames[03] := edtShowName03.Text;
  g_CustomItemPropertyBindNames[04] := edtShowName04.Text;
  g_CustomItemPropertyBindNames[05] := edtShowName05.Text;
  g_CustomItemPropertyBindNames[06] := edtShowName06.Text;
  g_CustomItemPropertyBindNames[07] := edtShowName07.Text;
  g_CustomItemPropertyBindNames[08] := edtShowName08.Text;
  g_CustomItemPropertyBindNames[09] := edtShowName09.Text;
  g_CustomItemPropertyBindNames[10] := edtShowName10.Text;
  g_CustomItemPropertyBindNames[11] := edtShowName11.Text;
  g_CustomItemPropertyBindNames[12] := edtShowName12.Text;
  g_CustomItemPropertyBindNames[13] := edtShowName13.Text;
  g_CustomItemPropertyBindNames[14] := edtShowName14.Text;
  g_CustomItemPropertyBindNames[15] := edtShowName15.Text;
  g_CustomItemPropertyBindNames[16] := edtShowName16.Text;
  g_CustomItemPropertyBindNames[17] := edtShowName17.Text;
  g_CustomItemPropertyBindNames[18] := edtShowName18.Text;
  g_CustomItemPropertyBindNames[19] := edtShowName19.Text;
  g_CustomItemPropertyBindNames[20] := edtShowName20.Text;
  g_CustomItemPropertyBindNames[21] := edtShowName21.Text;
  g_CustomItemPropertyBindNames[22] := edtShowName22.Text;
  g_CustomItemPropertyBindNames[23] := edtShowName23.Text;
  g_CustomItemPropertyBindNames[24] := edtShowName24.Text;
  g_CustomItemPropertyBindNames[25] := edtShowName25.Text;
  g_CustomItemPropertyBindNames[26] := edtShowName26.Text;
  g_CustomItemPropertyBindNames[27] := edtShowName27.Text;
  g_CustomItemPropertyBindNames[28] := edtShowName28.Text;
  g_CustomItemPropertyBindNames[29] := edtShowName29.Text;
  g_CustomItemPropertyBindNames[30] := edtShowName30.Text;
  g_CustomItemPropertyBindNames[31] := edtShowName31.Text;
  g_CustomItemPropertyBindNames[32] := edtShowName32.Text;
  g_CustomItemPropertyBindNames[33] := edtShowName33.Text;
  g_CustomItemPropertyBindNames[34] := edtShowName34.Text;
  g_CustomItemPropertyBindNames[35] := edtShowName35.Text;
  g_CustomItemPropertyBindNames[36] := edtShowName36.Text;
  g_CustomItemPropertyBindNames[37] := edtShowName37.Text;
  g_CustomItemPropertyBindNames[38] := edtShowName38.Text;
  g_CustomItemPropertyBindNames[39] := edtShowName39.Text;
  g_CustomItemPropertyBindNames[40] := edtShowName40.Text;
  g_CustomItemPropertyBindNames[41] := edtShowName41.Text;
  g_CustomItemPropertyBindNames[42] := edtShowName42.Text;
  g_CustomItemPropertyBindNames[43] := edtShowName43.Text;
  g_CustomItemPropertyBindNames[44] := edtShowName44.Text;
  g_CustomItemPropertyBindNames[45] := edtShowName45.Text;
  g_CustomItemPropertyBindNames[46] := edtShowName46.Text;
  g_CustomItemPropertyBindNames[47] := edtShowName47.Text;
  g_CustomItemPropertyBindNames[48] := edtShowName48.Text;
  g_CustomItemPropertyBindNames[49] := edtShowName49.Text;
  g_CustomItemPropertyBindNames[50] := edtShowName50.Text;
  g_CustomItemPropertyBindNames[51] := edtShowName51.Text;
  g_CustomItemPropertyBindNames[52] := edtShowName52.Text;
  g_CustomItemPropertyBindNames[53] := edtShowName53.Text;
  g_CustomItemPropertyBindNames[54] := edtShowName54.Text;
  g_CustomItemPropertyBindNames[55] := edtShowName55.Text;
  g_CustomItemPropertyBindNames[56] := edtShowName56.Text;
  g_CustomItemPropertyBindNames[57] := edtShowName57.Text;
  g_CustomItemPropertyBindNames[58] := edtShowName58.Text;
  g_CustomItemPropertyBindNames[59] := edtShowName59.Text;
  g_CustomItemPropertyBindNames[60] := edtShowName60.Text;

  for I := Low(g_CustomItemPropertyBindNames) to High(g_CustomItemPropertyBindNames) do
  begin
    Config.WriteBool('Setup', 'CustomItemPropertyCheck' + IntToStr(I), g_CustomItemPropertyChecks[I]);
    Config.WriteString('Setup', 'CustomItemPropertyBindName' + IntToStr(I), g_CustomItemPropertyBindNames[I]);
  end;

  OldCrc := g_CustomItemPropertyCRC;
  RebuildCustomItemPropertyConfig;

  if OldCrc <> g_CustomItemPropertyCRC then
  begin
    UserEngine.SendCustomItemPropertyConfig;
  end;

  btnOK.Enabled := False;
end;

procedure TFrmCustomItemProperty.chk01Click(Sender: TObject);
begin
  btnOK.Enabled := True;
end;

procedure TFrmCustomItemProperty.edtShowName01Change(
  Sender: TObject);
begin
  btnOK.Enabled := True;
end;

procedure TFrmCustomItemProperty.mmoVarChange(Sender: TObject);
begin
  btnOK2.Enabled := True;
end;

procedure TFrmCustomItemProperty.btnOK2Click(Sender: TObject);
var
  OldCrc: LongWord;
begin
  g_CustomItemPropertyTextVarList.Text := mmoVar.Text;

  OldCrc := g_CustomItemPropertyTextVarListTextCRC;

  SaveCustomItemPropertyTextVarList;

  if OldCrc <> g_CustomItemPropertyTextVarListTextCRC then
  begin
    UserEngine.SendCustomItemPropertyTextVarList;
  end;

  btnOK2.Enabled := False;
end;

procedure TFrmCustomItemProperty.mmoVarKeyUp(Sender: TObject;
  var Key: Word; Shift: TShiftState);
begin
  lblLineNum.Caption := '当前行：' + IntToStr(mmoVar.CaretPos.Y + 1);
end;

procedure TFrmCustomItemProperty.mmoVarMouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  lblLineNum.Caption := '当前行：' + IntToStr(mmoVar.CaretPos.Y + 1);
end;

end.

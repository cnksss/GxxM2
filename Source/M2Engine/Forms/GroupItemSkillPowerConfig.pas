unit GroupItemSkillPowerConfig;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ViewList2, StdCtrls, Grids, Grobal2,
  M2Definition;

type
  TFrmGroupItemSkillPower = class(TForm)
    StringGridSkillPower: TStringGrid;
    Button1: TButton;
    procedure Button1Click(Sender: TObject);
    procedure FormCreate(Sender: TObject);
  private
    { Private declarations }
  public
  end;

function ShowFrmGroupItemSkillPower: Boolean;

implementation

uses
  Math, M2Share;

{$R *.dfm}

function ShowFrmGroupItemSkillPower: Boolean;
var
  FrmGroupItemSkillPower: TFrmGroupItemSkillPower;
begin
  FrmGroupItemSkillPower := TFrmGroupItemSkillPower.Create(nil);
  try
    Result := FrmGroupItemSkillPower.ShowModal = mrOK;
  finally
    FrmGroupItemSkillPower.Free;
  end;
end;

procedure TFrmGroupItemSkillPower.FormCreate(Sender: TObject);
var
  I: Integer;
  Magic: pTMagic;
begin
  if SelGroupItem <> nil then
    Caption := Format('套装编号%d的技能威力百分比设置', [SelGroupItem.FLD_INDEX]);
  StringGridSkillPower.Cells[0, 0] := '技能名称';
  StringGridSkillPower.Cells[1, 0] := '增加技能伤害百分比';
  StringGridSkillPower.Cells[2, 0] := '增加技能防御百分比';
  StringGridSkillPower.RowCount := 115;

  for I := 0 to StringGridSkillPower.RowCount - 1 do
  begin
    Magic := UserEngine.FindMagic(I + 1, mtHum);
    if Magic = nil then
      Magic := UserEngine.FindMagic(I + 1, mtContinuous);
    if Magic <> nil then
      StringGridSkillPower.Cells[0, I + 1] := Magic.sMagicName
    else
      StringGridSkillPower.Cells[0, I + 1] := '';
    StringGridSkillPower.Cells[1, I + 1] := IntToStr(SelAttackSkillPercent[I + 1]);
    StringGridSkillPower.Cells[2, I + 1] := IntToStr(SelDefenseSkillPercent[I + 1]);
    if (Magic <> nil) and (Magic.wMagicId in [2, 3, 4, 8, 14..21, 29, 28, 30, 31, 32, 34, 38, 41, 48, 49, 50, 55, 68, 67, 70..80])
      then
      StringGridSkillPower.Cells[0, I + 1] := StringGridSkillPower.Cells[0, I + 1] + '[无效]';
  end;
end;

procedure TFrmGroupItemSkillPower.Button1Click(Sender: TObject);
var
  I: Integer;
begin
  for I := 0 to StringGridSkillPower.RowCount - 1 do
  begin
    SelAttackSkillPercent[I + 1] := StrToIntDef(Trim(StringGridSkillPower.Cells[1, I + 1]), 0);
    SelDefenseSkillPercent[I + 1] := StrToIntDef(Trim(StringGridSkillPower.Cells[2, I + 1]), 0);
  end;

  ModalResult := mrOK;
end;

end.


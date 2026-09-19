unit uFrmHitInterval;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, Grids, ValEdit, ExtCtrls, StdCtrls, GateShare, IniFiles;

type
  TFrmHitInterval = class(TForm)
    grpSetting: TGroupBox;
    grdHitInterval: TStringGrid;
    pnlBottom: TPanel;
    btn1: TButton;
    procedure btn1Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

  function ShowFrmHitInterval: Boolean;

implementation

{$R *.dfm}

function ShowFrmHitInterval: Boolean;
var
  I: Integer;
  FrmHitInterval: TFrmHitInterval;
begin
  FrmHitInterval := TFrmHitInterval.Create(nil);
  try
    FrmHitInterval.grdHitInterval.RowCount := Length(g_dwHitIntervals) + 1;
    FrmHitInterval.grdHitInterval.Cells[0, 0] := '攻击加速';
    FrmHitInterval.grdHitInterval.Cells[1, 0] := '间隔检测';

    for I := 0 to Length(g_dwHitIntervals) - 1 do
    begin
      FrmHitInterval.grdHitInterval.Cells[0, I + 1] := '攻击加速+' + IntToStr(I);
      FrmHitInterval.grdHitInterval.Cells[1, I + 1] := IntToStr(g_dwHitIntervals[I]);
    end;

    Result := FrmHitInterval.ShowModal = mrOK;
  finally
    FrmHitInterval.Free;
  end;
end;


procedure TFrmHitInterval.btn1Click(Sender: TObject);
var
  I: Integer;
  Value: Integer;
  IniFile: TIniFile;
begin
  for I := 0 to Length(g_dwHitIntervals) - 1 do
  begin
    Value := StrToIntDef(grdHitInterval.Cells[1, I + 1], 0);
    if Value = 0 then
    begin
      MessageBox(Handle, '输入的数据必须大于0', '错误', MB_OK or MB_ICONERROR);
      grdHitInterval.Row := I + 1;
      if grdHitInterval.CanFocus then
        grdHitInterval.SetFocus;
      Exit;
    end;

    g_dwHitIntervals[I] := Value;
  end;

  IniFile := TIniFile.Create(g_sHitIntervalsFileName);
  try
    for I := 0 to Length(g_dwHitIntervals) - 1 do
      IniFile.WriteInteger('Intervals', 'Speed' + IntToStr(I), g_dwHitIntervals[I]);
  finally
    IniFile.Free;
  end;

  ModalResult := mrOK;
end;

end.

unit uFrmInterval;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, Grids, ValEdit, ExtCtrls, StdCtrls, GateShare, IniFiles, Spin,
  SpinEditEx, Math, Buttons;

type
  TFrmInterval = class(TForm)
    grpSetting: TGroupBox;
    grdInterval: TStringGrid;
    pnlBottom: TPanel;
    btnOK: TButton;
    grpBatch: TGroupBox;
    btnAll: TButton;
    lblSpeed0: TLabel;
    seSpeed0: TSpinEditEx;
    lblIncSpeedDecTime: TLabel;
    seIncSpeedDecTime: TSpinEditEx;
    chkSendSpeedIntervalsToClient: TCheckBox;
    btnZero: TSpeedButton;
    procedure btnOKClick(Sender: TObject);
    procedure btnAllClick(Sender: TObject);
    procedure btnZeroClick(Sender: TObject);
    procedure FormShow(Sender: TObject);
  private
    { Private declarations }
    FActionMode: TAntiPlugActionMode;
  public
    { Public declarations }
  end;

  function ShowFrmInterval(ActionMode: TAntiPlugActionMode): Boolean;

implementation

{$R *.dfm}

function ShowFrmInterval(ActionMode: TAntiPlugActionMode): Boolean;
var
  I: Integer;
  FrmInterval: TFrmInterval;
begin
  Result := False;
  if not ActionModeUseSpeedIntervals(ActionMode) then Exit;
  FrmInterval := TFrmInterval.Create(nil);
  try
    FrmInterval.grdInterval.RowCount := SPEED_INTERVALS_COUNT + 1;
    FrmInterval.FActionMode := ActionMode;
    FrmInterval.chkSendSpeedIntervalsToClient.Visible := True;
    
    if ActionMode = amHit then
    begin
      FrmInterval.Caption := '攻击间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '攻击加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';

      FrmInterval.chkSendSpeedIntervalsToClient.Visible := False;
    end
    else if ActionMode = amSpell then
    begin
      FrmInterval.Caption := '魔法间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '魔法加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';

      FrmInterval.chkSendSpeedIntervalsToClient.Visible := False;
    end
    else if ActionMode = amWalk then
    begin
      FrmInterval.Caption := '走路间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '走路加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';

      FrmInterval.chkSendSpeedIntervalsToClient.Visible := False;
    end
    else if ActionMode = amRun then
    begin
      FrmInterval.Caption := '跑步间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '跑步加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';

      FrmInterval.chkSendSpeedIntervalsToClient.Visible := False;
    end
    else if ActionMode = amWalkToHit then
    begin
      FrmInterval.Caption := '走路到攻击间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '攻击加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amHitToWalk then
    begin
      FrmInterval.Caption := '攻击到走路间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '走路加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amRunToHit then
    begin
      FrmInterval.Caption := '跑步到攻击间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '攻击加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amHitToRun then
    begin
      FrmInterval.Caption := '攻击到跑步间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '跑步加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amWalkToSpell then
    begin
      FrmInterval.Caption := '走路到魔法间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '魔法加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amSpellToWalk then
    begin
      FrmInterval.Caption := '魔法到走路间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '走路加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amRunToSpell then
    begin
      FrmInterval.Caption := '跑步到魔法间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '魔法加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amSpellToRun then
    begin
      FrmInterval.Caption := '魔法到跑步间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '跑步加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amTurnToHit then
    begin
      FrmInterval.Caption := '转向到攻击间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '攻击加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amTurnToSpell then
    begin
      FrmInterval.Caption := '转向到魔法间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '魔法加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amCutMeatToHit then
    begin
      FrmInterval.Caption := '挖肉到攻击间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '攻击加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amCutMeatToSpell then
    begin
      FrmInterval.Caption := '挖肉到魔法间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '魔法加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amTurnToMove then
    begin
      FrmInterval.Caption := '转向到移动间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '移动加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end
    else if ActionMode = amCutMeatToMove then
    begin
      FrmInterval.Caption := '挖肉到移动间隔设置';
      FrmInterval.grdInterval.Cells[0, 0] := '移动加速';
      FrmInterval.grdInterval.Cells[1, 0] := '间隔检测';
    end;

    if FrmInterval.chkSendSpeedIntervalsToClient.Visible then
    begin
      FrmInterval.chkSendSpeedIntervalsToClient.Checked := g_boSendSpeedIntervalsToClient[ActionMode];
    end;

    for I := 0 to SPEED_INTERVALS_COUNT - 1 do
    begin
      if I >= HALF_SPEED_INTERVALS_COUNT then
      begin
        FrmInterval.grdInterval.Cells[0, I + 1] := '+' + IntToStr(I - HALF_SPEED_INTERVALS_COUNT);
      end
      else
      begin
        FrmInterval.grdInterval.Cells[0, I + 1] := IntToStr(I - HALF_SPEED_INTERVALS_COUNT);
      end;
      FrmInterval.grdInterval.Cells[1, I + 1] := IntToStr(g_wActionSpeedIntervals[ActionMode][I]);
    end;

    Result := FrmInterval.ShowModal = mrOK;
  finally
    FrmInterval.Free;
  end;
end;


procedure TFrmInterval.btnOKClick(Sender: TObject);
var
  I: Integer;
  Value: Integer;
  IniFile: TIniFile;
  Section: string;
begin
  for I := 0 to grdInterval.RowCount - 2 do
  begin
    Value := StrToIntDef(grdInterval.Cells[1, I + 1], 0);
    if Value <= 0 then
    begin
      MessageBox(Handle, '输入的数据必须大于0', '错误', MB_OK or MB_ICONERROR);
      grdInterval.Row := I + 1;
      if grdInterval.CanFocus then
        grdInterval.SetFocus;
      Exit;
    end;

    if I < SPEED_INTERVALS_COUNT then
    begin
      g_wActionSpeedIntervals[FActionMode][I] := Value;
    end;
  end;

  if Length(g_sActionIntervalsFileNames[FActionMode]) > 0 then
  begin
    IniFile := TIniFile.Create(g_sActionIntervalsFileNames[FActionMode]);
    try
      for I := 0 to SPEED_INTERVALS_COUNT - 1 do
        IniFile.WriteInteger('Intervals', 'Speed' + IntToStr(I - HALF_SPEED_INTERVALS_COUNT), g_wActionSpeedIntervals[FActionMode][I]);
    finally
      IniFile.Free;
    end;
  end;

  if not (FActionMode in [amHit {攻击}, amSpell {魔法}, amWalk {走路}, amRun {跑步}]) then
  begin
    g_boSendSpeedIntervalsToClient[FActionMode] := chkSendSpeedIntervalsToClient.Checked;
    IniFile := TIniFile.Create(g_sIniFileName);
    try
      Section := AntiPlugActionModeSections[FActionMode];
      IniFile.WriteBool(Section, 'SendSpeedIntervalsToClient', g_boSendSpeedIntervalsToClient[FActionMode]);
    finally
      IniFile.Free;
    end;
  end;

  RebuildSendToClientSpeedIntervalsText;

  ModalResult := mrOK;
end;

procedure TFrmInterval.btnAllClick(Sender: TObject);
var
  I: Integer;
begin
  if seSpeed0.Value <= 0 then
  begin
    MessageBox(Handle, '输入的数据必须大于0', '错误', MB_OK or MB_ICONERROR);
    if seSpeed0.CanFocus then seSpeed0.SetFocus;
    Exit;
  end;

  for I := 0 to grdInterval.RowCount - 2 do
  begin
    grdInterval.Cells[1, I + 1] := IntToStr(Max(5, seSpeed0.Value - seIncSpeedDecTime.Value * (I - HALF_SPEED_INTERVALS_COUNT)));
  end;
end;

procedure TFrmInterval.btnZeroClick(Sender: TObject);
begin
  grdInterval.Row := 0;
  grdInterval.Row := HALF_SPEED_INTERVALS_COUNT + 1;
end;

procedure TFrmInterval.FormShow(Sender: TObject);
begin
  grdInterval.Row := 0;
  grdInterval.Row := HALF_SPEED_INTERVALS_COUNT + 1;
end;

end.

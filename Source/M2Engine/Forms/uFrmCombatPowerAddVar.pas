unit uFrmCombatPowerAddVar;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Spin, SpinEditEx;

type
  TFrmCombatPowerAddVar = class(TForm)
    grp1: TGroupBox;
    cbbVarName: TComboBox;
    lbl1: TLabel;
    lbl2: TLabel;
    seVarIndex: TSpinEditEx;
    Label1: TLabel;
    seVarCount: TSpinEditEx;
    btnOK: TButton;
    chkBatch: TCheckBox;
    procedure chkBatchClick(Sender: TObject);
    procedure btnOKClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

  function CheckCombatPowerVarSupport(VarName: string): Boolean;

  function ShowFrmCombatPowerAddVar(var VarName: string; var IsBatch: Boolean; var VarIndex, VarCount: Integer): Boolean;

implementation

{$R *.dfm}

function ShowFrmCombatPowerAddVar(var VarName: string; var IsBatch: Boolean; var VarIndex, VarCount: Integer): Boolean;
var
  FrmCombatPowerAddVar: TFrmCombatPowerAddVar;
begin
  FrmCombatPowerAddVar := TFrmCombatPowerAddVar.Create(nil);
  try
    Result := FrmCombatPowerAddVar.ShowModal = mrOk;
    if Result then
    begin
      VarName := Trim(FrmCombatPowerAddVar.cbbVarName.Text);
      IsBatch := FrmCombatPowerAddVar.chkBatch.Checked;
      VarIndex := FrmCombatPowerAddVar.seVarIndex.Value;
      VarCount := FrmCombatPowerAddVar.seVarCount.Value;
    end;
  finally
    FrmCombatPowerAddVar.Free;
  end;
end;

function CheckCombatPowerVarSupport(VarName: string): Boolean;
begin
  Result := False;
  if Length(VarName) = 0 then Exit;

  VarName := UpperCase(VarName);
  {$IF CompilerVersion >= 22}
    if CharInSet(VarName[1], ['D', 'M', 'N', 'U', 'J']) then
      Result := True
    else if (Length(VarName) > 2) and (Copy(VarName, 1, 2) = 'N$') then
      Result := True;
  {$ELSE}
    if VarName[1] in ['D', 'M', 'N', 'U', 'J'] then
      Result := True
    else if (Length(VarName) > 2) and (Copy(VarName, 1, 2) = 'N$') then
      Result := True;
  {$IFEND}
end;

procedure TFrmCombatPowerAddVar.chkBatchClick(Sender: TObject);
begin
  seVarIndex.Enabled := chkBatch.Checked;
  seVarCount.Enabled := chkBatch.Checked;  
end;

procedure TFrmCombatPowerAddVar.btnOKClick(Sender: TObject);
var
  VarName: string;
begin
  VarName := Trim(cbbVarName.Text);
  if VarName = '' then
  begin
    ShowMessage('变量名不能为空');
    cbbVarName.SetFocus;
    Exit;
  end;

  if not CheckCombatPowerVarSupport(VarName) then
  begin
    ShowMessage('不支持的变量');
    cbbVarName.SetFocus;
    Exit;
  end;

  ModalResult := mrOk;
end;

end.

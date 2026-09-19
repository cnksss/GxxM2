unit uFrmGlobalVarEdit;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Grids, IniFiles;

type
  TFrmGlobalVarEdit = class(TForm)
    strngrdVar: TStringGrid;
    btnClearVar: TButton;
    btnRefreshVar: TButton;
    btnSave: TButton;
    btnSaveDesc: TButton;
    procedure strngrdVarSetEditText(Sender: TObject; ACol, ARow: Integer;
      const Value: String);
    procedure btnClearVarClick(Sender: TObject);
    procedure btnRefreshVarClick(Sender: TObject);
    procedure btnSaveClick(Sender: TObject);
    procedure btnSaveDescClick(Sender: TObject);
  private
    { Private declarations }
    FVarType: Integer;

    procedure LoadVarDesc;
  public
    { Public declarations }
  end;

  function ShowFrmGlobalVarEdit(VarType: Integer): Boolean;

implementation

uses
  M2Share;

{$R *.dfm}

function ShowFrmGlobalVarEdit(VarType: Integer): Boolean;
var
  I: Integer;
  Form: TFrmGlobalVarEdit;
begin
  Form := TFrmGlobalVarEdit.Create(nil);
  try
    Form.FVarType := VarType;
    Form.btnSave.Enabled := False;
    Form.btnSaveDesc.Enabled := False;

    Form.strngrdVar.Cells[0, 0] := '变量名';
    Form.strngrdVar.Cells[1, 0] := '变量值';
    Form.strngrdVar.Cells[2, 0] := '变量备注';
    if VarType = 0 then
    begin
      Form.Caption := '全局G变量编辑';
      Form.strngrdVar.RowCount := Length(g_Config.GlobalVal) + 1;
      for I := 0 to Length(g_Config.GlobalVal) - 1 do
      begin
        Form.strngrdVar.Cells[0, I + 1] := 'G' + IntToStr(I);
        Form.strngrdVar.Cells[1, I + 1] := IntToStr(g_Config.GlobalVal[I]);
      end;
    end
    else
    begin
      Form.Caption := '全局A变量编辑';
      Form.strngrdVar.RowCount := Length(g_Config.GlobalAVal) + 1;
      for I := 0 to Length(g_Config.GlobalAVal) - 1 do
      begin
        Form.strngrdVar.Cells[0, I + 1] := 'A' + IntToStr(I);
        Form.strngrdVar.Cells[1, I + 1] := g_Config.GlobalAVal[I];
      end;
    end;

    Form.LoadVarDesc;

    Result := Form.ShowModal = mrOk;
  finally
    Form.Free;
  end;
end;

procedure TFrmGlobalVarEdit.strngrdVarSetEditText(Sender: TObject; ACol,
  ARow: Integer; const Value: String);
begin
  if ACol = 1 then
  begin
    btnSave.Enabled := True;
  end
  else
  begin
    btnSaveDesc.Enabled := True;
  end;
end;

procedure TFrmGlobalVarEdit.btnClearVarClick(Sender: TObject);
var
  I: Integer;
begin
  if FVarType = 0 then
  begin
    if Application.MessageBox('是否确定清除所有G变量？', '提示', MB_YESNO + MB_ICONQUESTION) = mrYes then
    begin
      for I := 0 to Length(g_Config.GlobalVal) - 1 do
      begin
        g_Config.GlobalVal[I] := 0;
        strngrdVar.Cells[1, I + 1] := IntToStr(g_Config.GlobalVal[I]);
      end;
    end;
  end
  else
  begin
    if Application.MessageBox('是否确定清除所有A变量？', '提示', MB_YESNO + MB_ICONQUESTION) = mrYes then
    begin
      for I := 0 to Length(g_Config.GlobalAVal) - 1 do
      begin
        g_Config.GlobalAVal[I] := '';
        strngrdVar.Cells[1, I + 1] := g_Config.GlobalAVal[I];
      end;
    end;
  end;
  btnSave.Enabled := True;
end;

procedure TFrmGlobalVarEdit.btnRefreshVarClick(Sender: TObject);
var
  I: Integer;
begin
  if FVarType = 0 then
  begin
    for I := 0 to Length(g_Config.GlobalVal) - 1 do
    begin
      strngrdVar.Cells[1, I + 1] := IntToStr(g_Config.GlobalVal[I]);
    end;
  end
  else
  begin
    for I := 0 to Length(g_Config.GlobalAVal) - 1 do
    begin
      strngrdVar.Cells[1, I + 1] := g_Config.GlobalAVal[I];
    end;
  end;
  btnSave.Enabled := False;
end;

procedure TFrmGlobalVarEdit.btnSaveClick(Sender: TObject);
var
  I: Integer;
begin
  if FVarType = 0 then
  begin
    if Application.MessageBox('是否保存G变量修改？', '提示', MB_YESNO + MB_ICONQUESTION) = mrYes then
    begin
      for I := 0 to Length(g_Config.GlobalVal) - 1 do
      begin
        g_Config.GlobalVal[I] := StrToIntDef(strngrdVar.Cells[1, I + 1], 0);
      end;
    end;
  end
  else
  begin
    if Application.MessageBox('是否保存A变量修改？', '提示', MB_YESNO + MB_ICONQUESTION) = mrYes then
    begin
      for I := 0 to Length(g_Config.GlobalAVal) - 1 do
      begin
        g_Config.GlobalAVal[I] := strngrdVar.Cells[1, I + 1];
      end;
    end;
  end;
  btnSave.Enabled := False;
end;

procedure TFrmGlobalVarEdit.LoadVarDesc;
var
  I: Integer;
  FileName, S: string;
  IniFile: TIniFile;
begin
  FileName := ExtractFilePath(Application.ExeName) + 'GlobalValDesc.ini';
  IniFile := TIniFile.Create(FileName);
  try
    if FVarType = 0 then
    begin
      for I := Low(g_Config.GlobalVal) to High(g_Config.GlobalVal) do
      begin
        S := IniFile.ReadString('VarG', IntToStr(I), '');
        strngrdVar.Cells[2, I + 1] := S;
      end;
    end
    else
    begin
      for I := Low(g_Config.GlobalAVal) to High(g_Config.GlobalAVal) do
      begin
        S := IniFile.ReadString('VarA', IntToStr(I), '');
        strngrdVar.Cells[2, I + 1] := S;
      end;
    end;
  finally
    IniFile.Free;
  end;
end;

procedure TFrmGlobalVarEdit.btnSaveDescClick(Sender: TObject);
var
  I: Integer;
  FileName: string;
  IniFile: TIniFile;
begin
  FileName := ExtractFilePath(Application.ExeName) + 'GlobalValDesc.ini';
  IniFile := TIniFile.Create(FileName);
  try
    if FVarType = 0 then
    begin
      for I := 0 to Length(g_Config.GlobalVal) - 1 do
      begin
        IniFile.WriteString('VarG', IntToStr(I), strngrdVar.Cells[2, I + 1]);
      end;
    end
    else
    begin
      for I := 0 to Length(g_Config.GlobalAVal) - 1 do
      begin
        IniFile.WriteString('VarA', IntToStr(I), strngrdVar.Cells[2, I + 1]);
      end;
    end;
  finally
    IniFile.Free;
  end;

  btnSaveDesc.Enabled := False;
end;

end.

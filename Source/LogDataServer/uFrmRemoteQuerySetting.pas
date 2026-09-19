unit uFrmRemoteQuerySetting;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Spin, SpinEditEx, LDShare, IniFiles, Menus, HUtil32;

type
  TFrmRemoteQuerySetting = class(TForm)
    lbl1: TLabel;
    lbl2: TLabel;
    grp2: TGroupBox;
    lbl3: TLabel;
    lbl4: TLabel;
    edtPassword: TEdit;
    grp1: TGroupBox;
    lstControlIPList: TListBox;
    sePort: TSpinEditEx;
    btnOK: TButton;
    pmControlIPList: TPopupMenu;
    mniIPAdd: TMenuItem;
    mniIPDelete: TMenuItem;
    mniIPClear: TMenuItem;
    procedure btnOKClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure mniIPAddClick(Sender: TObject);
    procedure mniIPDeleteClick(Sender: TObject);
    procedure mniIPClearClick(Sender: TObject);
  private
    { Private declarations }
    procedure ErrMessage(MsgStr: string);
  public
    { Public declarations }
  end;

  procedure ShowFrmRemoteQuerySetting;
  
implementation

{$R *.dfm}

procedure ShowFrmRemoteQuerySetting;
var
  FrmRemoteQuerySetting: TFrmRemoteQuerySetting;
begin
  FrmRemoteQuerySetting := TFrmRemoteQuerySetting.Create(nil);
  try
    FrmRemoteQuerySetting.ShowModal;
  finally
    FrmRemoteQuerySetting.Free;
  end;
end;

procedure TFrmRemoteQuerySetting.btnOKClick(Sender: TObject);
var
  Conf: TIniFile;
begin
  g_nControlPort := sePort.Value;
  g_sControlPassword := edtPassword.Text;

  Conf := TIniFile.Create('.\LogData.ini');
  if Conf <> nil then
  begin
    Conf.WriteInteger('Setup', 'ControlPort', g_nControlPort);
    Conf.WriteString('Setup', 'ControlPassword', g_sControlPassword);
    Conf.Free;
  end;

  ModalResult := mrOK;
end;

procedure TFrmRemoteQuerySetting.ErrMessage(MsgStr: string);
begin
  Application.MessageBox(PChar(MsgStr), '错误', MB_OK or MB_ICONERROR);
end;

procedure TFrmRemoteQuerySetting.FormCreate(Sender: TObject);
var
  I: Integer;
begin
  sePort.Value := g_nControlPort;
  edtPassword.Text := g_sControlPassword;

  g_ControlIPList.Lock;
  try
    for I := 0 to g_ControlIPList.Count - 1 do
    begin
      lstControlIPList.Items.Add(g_ControlIPList.Strings[I]);
    end;
  finally
    g_ControlIPList.UnLock;
  end;
end;

function GetAveCharSize(Canvas: TCanvas): TPoint;
var
  I: Integer;
  Buffer: array[0..51] of Char;
begin
  for I := 0 to 25 do Buffer[I] := Chr(I + Ord('A'));
  for I := 0 to 25 do Buffer[I + 26] := Chr(I + Ord('a'));
  GetTextExtentPoint(Canvas.Handle, Buffer, 52, TSize(Result));
  Result.X := Result.X div 52;
end;

function InputQueryEx(const ACaption, APrompt, AHint: string;
  var Value: string): Boolean;
const
  FORM_WIDTH = 280;
var
  Form: TForm;
  Prompt: TLabel;
  Edit: TEdit;
  DialogUnits: TPoint;
  ButtonTop, ButtonWidth, ButtonHeight: Integer;
begin
  Result := False;
  Form := TForm.Create(Application);
  with Form do
  begin
    try
      Canvas.Font := Font;
      DialogUnits := GetAveCharSize(Canvas);
      BorderStyle := bsDialog;
      Caption := ACaption;
      ClientWidth := MulDiv(FORM_WIDTH, DialogUnits.X, 4);
      Position := poScreenCenter;
      Prompt := TLabel.Create(Form);
      with Prompt do
      begin
        Parent := Form;
        Caption := APrompt;
        Left := MulDiv(8, DialogUnits.X, 4);
        Top := MulDiv(8, DialogUnits.Y, 8);
        Constraints.MaxWidth := MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4);
        WordWrap := True;
      end;
      Edit := TEdit.Create(Form);
      with Edit do
      begin
        Parent := Form;
        Left := Prompt.Left;
        Top := Prompt.Top + Prompt.Height + 5;
        Width := MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4);
        MaxLength := 255;
        Text := Value;
        SelectAll;
      end;

      ButtonTop := Edit.Top + Edit.Height + 8;
      ButtonWidth := MulDiv(50, DialogUnits.X, 4);
      ButtonHeight := MulDiv(14, DialogUnits.Y, 8);

      with TButton.Create(Form) do
      begin
        Parent := Form;
        Caption := '确定';
        ModalResult := mrOk;
        Default := True;
        Left := Edit.Left + Edit.Width - ButtonWidth * 2 - 6;
        Top := ButtonTop;
        Width := ButtonWidth;
        Height := ButtonHeight;
      end;

      with TButton.Create(Form) do
      begin
        Parent := Form;
        Caption := '取消';
        ModalResult := mrCancel;
        Cancel := True;
        Left := Edit.Left + Edit.Width - ButtonWidth;
        Top := ButtonTop;
        Width := ButtonWidth;
        Height := ButtonHeight;

        Form.ClientHeight := Top + Height + 10;
      end;

      Prompt := TLabel.Create(Form);
      with Prompt do
      begin
        Parent := Form;
        Caption := AHint;
        Font.Color := clBlue;
        Left := Edit.Left;
        Top := ButtonTop + (ButtonHeight - Height) div 2;
        Constraints.MaxWidth := MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4);
        WordWrap := True;
      end;

      if ShowModal = mrOk then
      begin
        Value := Edit.Text;
        Result := True;
      end;
    finally
      Form.Free;
    end;
  end;
end;

procedure TFrmRemoteQuerySetting.mniIPAddClick(Sender: TObject);
var
  sIPaddress: string;
begin
  sIPaddress := '';
  if not InputQueryEx('永久IP过滤', '请输入一个新的IP地址: ', '如：202.103.100.20', sIPaddress) then Exit;
  if not IsIPaddr(sIPaddress) then
  begin
    ErrMessage('输入的地址格式错误！');
    Exit;
  end;

  g_ControlIPList.Lock;
  try
    if g_ControlIPList.IndexOf(sIPaddress) < 0 then
    begin
      g_ControlIPList.Add(sIPaddress);
      lstControlIPList.Items.Add(sIPaddress);

      g_ControlIPList.SaveToFile(g_ControlIPFile);
    end;
  finally
    g_ControlIPList.UnLock;
  end;
end;

procedure TFrmRemoteQuerySetting.mniIPDeleteClick(Sender: TObject);
begin
  if (lstControlIPList.ItemIndex >= 0) and (lstControlIPList.ItemIndex < lstControlIPList.Items.Count) then
  begin
    g_ControlIPList.Lock;
    try
      g_ControlIPList.Delete(lstControlIPList.ItemIndex);
    finally
      g_ControlIPList.UnLock;
    end;
    lstControlIPList.Items.Delete(lstControlIPList.ItemIndex);

    g_ControlIPList.SaveToFile(g_ControlIPFile);
  end;
end;

procedure TFrmRemoteQuerySetting.mniIPClearClick(Sender: TObject);
begin
  g_ControlIPList.Lock;
  try
    g_ControlIPList.Clear;
  finally
    g_ControlIPList.UnLock;
  end;

  lstControlIPList.Clear;
  g_ControlIPList.SaveToFile(g_ControlIPFile);
end;

end.

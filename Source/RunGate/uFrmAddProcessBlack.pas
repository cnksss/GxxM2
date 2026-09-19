unit uFrmAddProcessBlack;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, GateShare;

type
  TFrmAddProcessBlack = class(TForm)
    grp1: TGroupBox;
    lbl1: TLabel;
    edtProcessName: TEdit;
    Label1: TLabel;
    edtProcessMD5: TEdit;
    btnOK: TButton;
    btnCancel: TButton;
    procedure btnOKClick(Sender: TObject);
  private
    { Private declarations }
    FProcessInfo: PTProcessInfo;
  public
    { Public declarations }
  end;

  function ShowAddProcessBlack(var ProcessInfo: PTProcessInfo): Boolean;

implementation

{$R *.dfm}

function ShowAddProcessBlack(var ProcessInfo: PTProcessInfo): Boolean;
var
  FrmAddProcessBlack: TFrmAddProcessBlack;
begin
  FrmAddProcessBlack := TFrmAddProcessBlack.Create(nil);
  try
    FrmAddProcessBlack.FProcessInfo := nil;
    Result := FrmAddProcessBlack.ShowModal = mrOK;
    if Result then
    begin
      ProcessInfo := FrmAddProcessBlack.FProcessInfo;
    end;
  finally
    FrmAddProcessBlack.Free;
  end;
end;

procedure TFrmAddProcessBlack.btnOKClick(Sender: TObject);
var
  MD5: string;
begin
  if g_ProcessBlackList.Count >= 80 then
  begin
    Application.MessageBox('已经达到最多数量，无法添加', '提示', MB_OK + MB_ICONINFORMATION);
    edtProcessName.SetFocus;
    Exit;
  end;

  if Length(edtProcessName.Text) = 0 then
  begin
    Application.MessageBox('进程名不能为空', '提示', MB_OK + MB_ICONINFORMATION);
    edtProcessName.SetFocus;
    Exit;
  end;

  MD5 := Trim(edtProcessMD5.Text);
  if Length(MD5) = 0 then
  begin
    Application.MessageBox('进程MD5不能为空', '提示', MB_OK + MB_ICONINFORMATION);
    edtProcessMD5.SetFocus;
    Exit;
  end;

  if Length(MD5) <> 32 then
  begin
    Application.MessageBox('进程MD5长度不对', '提示', MB_OK + MB_ICONINFORMATION);
    edtProcessMD5.SetFocus;
    Exit;
  end;

  if not IsHexString(MD5) then
  begin
    Application.MessageBox('进程MD5包含非法的字符串', '提示', MB_OK + MB_ICONINFORMATION);
    edtProcessMD5.SetFocus;
    Exit;
  end;

  g_ProcessBlackList.Lock;
  try
    FProcessInfo := g_ProcessBlackList.Add(edtProcessName.Text, MD5);
  finally
    g_ProcessBlackList.UnLock;
  end;

  if FProcessInfo = nil then
  begin
    Application.MessageBox('进程MD5已经存在于黑名单中', '提示', MB_OK + MB_ICONINFORMATION);
    edtProcessMD5.SetFocus;
    Exit;
  end;

  ModalResult := mrOK;
end;

end.

unit uFrmReadFileIP;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Spin, SpinEditEx, GateShare, IniFiles;

type
  TFrmReadFileIP = class(TForm)
    grp1: TGroupBox;
    lbl1: TLabel;
    lbl2: TLabel;
    Label1: TLabel;
    edtFYReadDenyIPFile: TEdit;
    seFYReadDenyIPTime: TSpinEditEx;
    GroupBox1: TGroupBox;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    edtFYReadPassIPFile: TEdit;
    seFYReadPassIPTime: TSpinEditEx;
    btnOK: TButton;
    GroupBox2: TGroupBox;
    Label5: TLabel;
    Label6: TLabel;
    Label7: TLabel;
    edtFYReadDenyMACFile: TEdit;
    seFYReadDenyMACTime: TSpinEditEx;
    lbl3: TLabel;
    edtFYDownDenyIPUrl: TEdit;
    Label8: TLabel;
    Label9: TLabel;
    seFYDownDenyIPTime: TSpinEditEx;
    Label10: TLabel;
    edtFYDownPassIPUrl: TEdit;
    Label11: TLabel;
    Label12: TLabel;
    seFYDownPassIPTime: TSpinEditEx;
    chkOnlyWhiteListLink: TCheckBox;
    Label13: TLabel;
    Label14: TLabel;
    Label15: TLabel;
    edtFYDownDenyMACUrl: TEdit;
    seFYDownDenyMACTime: TSpinEditEx;
    procedure btnOKClick(Sender: TObject);
  private
    { Private declarations }

    procedure Open;
  public
    { Public declarations }
  end;

  function ShowFrmReadFileIP: Boolean;

implementation

{$R *.dfm}

function ShowFrmReadFileIP: Boolean;
var
  FrmReadFileIP: TFrmReadFileIP;
begin
  FrmReadFileIP := TFrmReadFileIP.Create(nil);
  try
    FrmReadFileIP.Open;
    Result := FrmReadFileIP.ShowModal = mrOK;
  finally
    FrmReadFileIP.Free;
  end;
end;

procedure TFrmReadFileIP.Open;
begin
  Left := Application.MainForm.Left;
  Top := Application.MainForm.Top + 20;

  edtFYReadDenyIPFile.Text := g_sFYReadDenyIPFile;
  seFYReadDenyIPTime.Value := g_dwFYReadDenyIPTime;

  edtFYDownDenyIPUrl.Text := g_sFYDownDenyIPUrl;
  seFYDownDenyIPTime.Value := g_dwFYDownDenyIPTime;

  edtFYReadPassIPFile.Text := g_sFYReadPassIPFile;
  seFYReadPassIPTime.Value := g_dwFYReadPassIPTime;

  edtFYDownPassIPUrl.Text := g_sFYDownPassIPUrl;
  seFYDownPassIPTime.Value := g_dwFYDownPassIPTime;

  edtFYReadDenyMACFile.Text := g_sFYReadDenyMACFile;
  seFYReadDenyMACTime.Value := g_dwFYReadDenyMACTime;

  edtFYDownDenyMACUrl.Text := g_sFYDownDenyMACUrl;
  seFYDownDenyMACTime.Value := g_dwFYDownDenyMACTime;

  chkOnlyWhiteListLink.Checked := g_OnlyWhiteListLink;
end;

procedure TFrmReadFileIP.btnOKClick(Sender: TObject);
var
  IniFile: TIniFile;
begin
  g_sFYReadDenyIPFile := Trim(edtFYReadDenyIPFile.Text);
  g_dwFYReadDenyIPTime := seFYReadDenyIPTime.Value;

  g_sFYDownDenyIPUrl := Trim(edtFYDownDenyIPUrl.Text);
  g_dwFYDownDenyIPTime := seFYDownDenyIPTime.Value;

  g_sFYReadPassIPFile := Trim(edtFYReadPassIPFile.Text);
  g_dwFYReadPassIPTime := seFYReadPassIPTime.Value;

  g_sFYDownPassIPUrl := Trim(edtFYDownPassIPUrl.Text);
  g_dwFYDownPassIPTime := seFYDownPassIPTime.Value;

  g_sFYReadDenyMACFile := Trim(edtFYReadDenyMACFile.Text);
  g_dwFYReadDenyMACTime := seFYReadDenyMACTime.Value;

  g_sFYDownDenyMACUrl := Trim(edtFYDownDenyMACUrl.Text);
  g_dwFYDownDenyMACTime := seFYDownDenyMACTime.Value;

  g_OnlyWhiteListLink := chkOnlyWhiteListLink.Checked;

  IniFile := TIniFile.Create(g_sIniFileName);

  IniFile.WriteString(GateClass, 'FYReadDenyIPFile', g_sFYReadDenyIPFile);
  IniFile.WriteInteger(GateClass, 'FYReadDenyIPTime', g_dwFYReadDenyIPTime);

  IniFile.WriteString(GateClass, 'FYDownDenyIPUrl', g_sFYDownDenyIPUrl);
  IniFile.WriteInteger(GateClass, 'FYDownDenyIPTime', g_dwFYDownDenyIPTime);

  IniFile.WriteString(GateClass, 'FYReadPassIPFile', g_sFYReadPassIPFile);
  IniFile.WriteInteger(GateClass, 'FYReadPassIPTime', g_dwFYReadPassIPTime);

  IniFile.WriteString(GateClass, 'FYDownPassIPUrl', g_sFYDownPassIPUrl);
  IniFile.WriteInteger(GateClass, 'FYDownPassIPTime', g_dwFYDownPassIPTime);

  IniFile.WriteString(GateClass, 'FYReadDenyMACFile', g_sFYReadDenyMACFile);
  IniFile.WriteInteger(GateClass, 'FYReadDenyMACTime', g_dwFYReadDenyMACTime);

  IniFile.WriteString(GateClass, 'FYDownDenyMACUrl', g_sFYDownDenyMACUrl);
  IniFile.WriteInteger(GateClass, 'FYDownDenyMACTime', g_dwFYDownDenyMACTime);

  IniFile.WriteBool(GateClass, 'OnlyWhiteListLink', g_OnlyWhiteListLink);

  IniFile.Free;
  ModalResult := mrOK;;
end;



end.

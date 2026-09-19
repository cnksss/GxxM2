unit uFrmAntiPlugUpdateSetting;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Spin, SpinEditEx, GateShare, IniFiles;

type
  TFrmAntiPlugUpdateSetting = class(TForm)
    grpMain: TGroupBox;
    chkAntiPlugAutoUpdateCheck: TCheckBox;
    lbl1: TLabel;
    edtAntiPlugUpdateConfigUrl: TEdit;
    btnOK: TButton;
    lbl2: TLabel;
    seAntiPlugUpdateCheckInterval: TSpinEditEx;
    Label1: TLabel;
    procedure FormCreate(Sender: TObject);
    procedure btnOKClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

  procedure ShowFrmAntiPlugUpdateSetting;
  
implementation

{$R *.dfm}

procedure ShowFrmAntiPlugUpdateSetting;
var
  FrmAntiPlugUpdateSetting: TFrmAntiPlugUpdateSetting;
begin
  FrmAntiPlugUpdateSetting := TFrmAntiPlugUpdateSetting.Create(nil);
  try
    FrmAntiPlugUpdateSetting.ShowModal;
  finally
    FrmAntiPlugUpdateSetting.Free;
  end;
end;

procedure TFrmAntiPlugUpdateSetting.FormCreate(Sender: TObject);
begin
  chkAntiPlugAutoUpdateCheck.Checked := g_boAntiPlugAutoUpdateCheck;
  seAntiPlugUpdateCheckInterval.Value := g_wAntiPlugUpdateCheckInterval;
  edtAntiPlugUpdateConfigUrl.Text := g_sAntiPlugUpdateConfigUrl;
end;

procedure TFrmAntiPlugUpdateSetting.btnOKClick(Sender: TObject);
var
  IniFile: TIniFile;
begin
  g_boAntiPlugAutoUpdateCheck := chkAntiPlugAutoUpdateCheck.Checked;
  g_wAntiPlugUpdateCheckInterval := seAntiPlugUpdateCheckInterval.Value;
  g_sAntiPlugUpdateConfigUrl := edtAntiPlugUpdateConfigUrl.Text;

  IniFile := TIniFile.Create(g_sIniFileName);
  try
    IniFile.WriteBool(GateClass, 'AntiPlugAutoUpdateCheck', g_boAntiPlugAutoUpdateCheck);
    IniFile.WriteInteger(GateClass, 'AntiPlugUpdateCheckInterval', g_wAntiPlugUpdateCheckInterval);
    IniFile.WriteString(GateClass, 'AntiPlugUpdateConfigUrl5', g_sAntiPlugUpdateConfigUrl);
  finally
    IniFile.Free;
  end;

  ModalResult := mrOk;
end;

end.

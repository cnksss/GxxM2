unit BasicSet;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, ComCtrls, Spin, Menus, LSShare, Grobal2;

type
  TFrmBasicSet = class(TForm)
    PageControl1: TPageControl;
    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    TabSheet3: TTabSheet;
    GroupBox1: TGroupBox;
    GroupBox2: TGroupBox;
    CheckBoxTestServer: TCheckBox;
    CheckBoxEnableMakingID: TCheckBox;
    CheckBoxEnableGetbackPassword: TCheckBox;
    CheckBoxAutoClear: TCheckBox;
    Label1: TLabel;
    Label2: TLabel;
    ButtonSave: TButton;
    ButtonClose: TButton;
    SpinEditAutoClearTime: TSpinEdit;
    ButtonRestoreBasic: TButton;
    ButtonRestoreNet: TButton;
    GroupBox3: TGroupBox;
    GroupBox4: TGroupBox;
    GroupBox5: TGroupBox;
    Label3: TLabel;
    Label4: TLabel;
    EditGateAddr: TEdit;
    EditGatePort: TEdit;
    Label5: TLabel;
    Label6: TLabel;
    EditMonAddr: TEdit;
    EditMonPort: TEdit;
    Label7: TLabel;
    Label8: TLabel;
    EditServerAddr: TEdit;
    EditServerPort: TEdit;
    GroupBox8: TGroupBox;
    Label11: TLabel;
    chkRandomCodeLogin: TCheckBox;
    EditRandomCodeErrorMaxCount: TSpinEdit;
    chkDisableIDSamePassword: TCheckBox;
    chkDisableQuizSameAnswer: TCheckBox;
    CheckBoxDynamicIPMode: TCheckBox;
    GroupBox9: TGroupBox;
    Label12: TLabel;
    Label13: TLabel;
    edtControlPort: TEdit;
    edtControlPassword: TEdit;
    grp1: TGroupBox;
    lstControlIPList: TListBox;
    pmControlIPList: TPopupMenu;
    mniIPAdd: TMenuItem;
    mniIPDelete: TMenuItem;
    mniIPClear: TMenuItem;
    lbl1: TLabel;
    Label14: TLabel;
    CheckBoxGetbackPasswordCheckAll: TCheckBox;
    grpL2Password: TGroupBox;
    chkChangedMACCheckL2: TCheckBox;
    chkChangedIPCheckL2: TCheckBox;
    chkAlwaysCheckL2: TCheckBox;
    GroupBox7: TGroupBox;
    Label9: TLabel;
    Label10: TLabel;
    CheckBoxAutoUnLockAccount: TCheckBox;
    SpinEditUnLockAccountTime: TSpinEdit;
    chkDisableIDSameL2Password: TCheckBox;
    chkDisableL2SamePassword: TCheckBox;
    chkEnabledL2Password: TCheckBox;
    ts1: TTabSheet;
    chkDisablePwdSameChr: TCheckBox;
    chkDisablePwdAllNum: TCheckBox;
    Label15: TLabel;
    mmoDisablePassword: TMemo;
    chkDisablePwdAllLetter: TCheckBox;
    Label16: TLabel;
    seRandomCodeRefreshMaxCount: TSpinEdit;
    chkRandomCodePwdGetback: TCheckBox;
    chkRandomCodePwdChange: TCheckBox;
    chkRandomCodeReg: TCheckBox;
    lbl2: TLabel;
    seLoginWaveValue: TSpinEdit;
    Label17: TLabel;
    seOtherWaveValue: TSpinEdit;
    chkShowBlockIPLog: TCheckBox;
    GroupBox6: TGroupBox;
    chkNewLoginDlg: TCheckBox;
    chkNewLoginInto: TCheckBox;
    chkNewLoginPhone: TCheckBox;
    procedure CheckBoxTestServerClick(Sender: TObject);
    procedure CheckBoxEnableMakingIDClick(Sender: TObject);
    procedure CheckBoxEnableGetbackPasswordClick(Sender: TObject);
    procedure CheckBoxAutoClearClick(Sender: TObject);
    procedure SpinEditAutoClearTimeChange(Sender: TObject);
    procedure CheckBoxAutoUnLockAccountClick(Sender: TObject);
    procedure SpinEditUnLockAccountTimeChange(Sender: TObject);
    procedure ButtonRestoreBasicClick(Sender: TObject);
    procedure EditGateAddrChange(Sender: TObject);
    procedure EditGatePortChange(Sender: TObject);
    procedure EditMonAddrChange(Sender: TObject);
    procedure EditMonPortChange(Sender: TObject);
    procedure EditServerAddrChange(Sender: TObject);
    procedure EditServerPortChange(Sender: TObject);
    procedure CheckBoxDynamicIPModeClick(Sender: TObject);
    procedure ButtonRestoreNetClick(Sender: TObject);
    procedure ButtonSaveClick(Sender: TObject);
    procedure ButtonCloseClick(Sender: TObject);
    procedure chkRandomCodeLoginClick(Sender: TObject);
    procedure EditRandomCodeErrorMaxCountChange(Sender: TObject);
    procedure chkDisableIDSamePasswordClick(Sender: TObject);
    procedure chkDisableQuizSameAnswerClick(Sender: TObject);
    procedure edtControlPortChange(Sender: TObject);
    procedure edtControlPasswordChange(Sender: TObject);
    procedure mniIPAddClick(Sender: TObject);
    procedure mniIPDeleteClick(Sender: TObject);
    procedure mniIPClearClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure CheckBoxGetbackPasswordCheckAllClick(Sender: TObject);
    procedure chkChangedMACCheckL2Click(Sender: TObject);
    procedure chkChangedIPCheckL2Click(Sender: TObject);
    procedure chkAlwaysCheckL2Click(Sender: TObject);
    procedure chkDisableIDSameL2PasswordClick(Sender: TObject);
    procedure chkDisableL2SamePasswordClick(Sender: TObject);
    procedure chkEnabledL2PasswordClick(Sender: TObject);
    procedure chkDisablePwdSameChrClick(Sender: TObject);
    procedure chkDisablePwdAllNumClick(Sender: TObject);
    procedure chkDisablePwdAllLetterClick(Sender: TObject);
    procedure mmoDisablePasswordChange(Sender: TObject);
    procedure seRandomCodeRefreshMaxCountChange(Sender: TObject);
    procedure chkRandomCodeRegClick(Sender: TObject);
    procedure chkRandomCodePwdGetbackClick(Sender: TObject);
    procedure chkRandomCodePwdChangeClick(Sender: TObject);
    procedure seLoginWaveValueChange(Sender: TObject);
    procedure seOtherWaveValueChange(Sender: TObject);
    procedure chkShowBlockIPLogClick(Sender: TObject);
    procedure chkNewLoginDlgClick(Sender: TObject);
    procedure chkNewLoginIntoClick(Sender: TObject);
    procedure chkNewLoginPhoneClick(Sender: TObject);
    procedure chkNewLoginMustHasPhoneClick(Sender: TObject);
  private
    { Private declarations }
    procedure LockSaveButtonEnabled();
    procedure UnLockSaveButtonEnabled();

    procedure ErrMessage(MsgStr: string);
  public
    { Public declarations }
    procedure OpenBasicSet();
  end;

var
  FrmBasicSet: TFrmBasicSet;

resourcestring
  sSectionServer = 'Server';
  sSectionDB = 'DB';
  sSectionDataSaveDB = 'DataSaveDB';

  sIdentDBServer = 'DBServer';
  sIdentFeeServer = 'FeeServer';
  sIdentLogServer = 'LogServer';
  sIdentGateAddr = 'GateAddr';
  sIdentGatePort = 'GatePort';
  sIdentServerAddr = 'ServerAddr';
  sIdentServerName = 'ServerName';
  sIdentServerPort = 'ServerPort';
  sIdentMonAddr = 'MonAddr';
  sIdentMonPort = 'MonPort';
  sIdentDBSPort = 'DBSPort';
  sIdentFeePort = 'FeePort';
  sIdentLogPort = 'LogPort';

  sIdentControlPort = 'ControlPort';
  sIdentControlPassword = 'ControlPassword';

  sIdentReadyServers = 'ReadyServers';
  sIdentTestServer = 'TestServer';
  sIdentDynamicIPMode = 'DynamicIPMode';
  sIdentIdDir = 'IdDir';
  sIdentWebLogDir = 'WebLogDir';
  sIdentCountLogDir = 'CountLogDir';
  sIdentFeedIDList = 'FeedIDList';
  sIdentFeedIPList = 'FeedIPList';
  sIdentShowBlockIPLog = 'ShowBlockIPLog';

  sIdentEnableMakingID = 'EnableMakingID';
  sIdentEnableGetbackPassword = 'GetbackPassword';
  sIdentGetbackPasswordCheckAll = 'GetbackPasswordCheckAll';
  sIdentDisableIDSamePassword = 'DisableIDSamePassword';
  sIdentDisableQuizSameAnswer = 'DisableQuizSameAnswer';
  sIdentDisableIDSameL2Password = 'DisableIDSameL2Password';
  sIdentDisableL2SamePassword = 'sIdentDisableL2SamePassword';

  sIdentDisablePwdSameChr = 'DisablePasswordSameChr';
  sIdentDisablePwdAllNum = 'DisablePasswordAllNum';
  sIdentDisablePwdAllLetter = 'DisablePasswordAllLetter';

  sIdentAutoClearID = 'AutoClearID';
  sIdentAutoClearTime = 'AutoClearTime';
  sIdentUnLockAccount = 'UnLockAccount';
  sIdentUnLockAccountTime = 'UnLockAccountTime';
  sIdentMinimize = 'Minimize';

  sIdentRandomCodeErrorMaxCount = 'RandomCodeErrorMaxCount';
  sIdentRandomCodeRefreshMaxCount = 'RandomCodeRefreshMaxCount';

  sIdentLoginWaveValue = 'LoginWaveValue';
  sIdentOtherWaveValue = 'OtherWaveValue';

  sIdentEnabledL2Password = 'EnabledL2Password';
  sIdentChangedMACCheckL2 = 'ChangedMACCheckL2';
  sIdentChangedIPCheckL2 = 'ChangedIPCheckL2';
  sIdentAlwaysCheckL2 = 'AlwaysCheckL2';

  sIdentDataSaveDBType = 'DataSaveDBType';
  sIdentDataSaveDBServer = 'DataSaveDBServer';
  sIdentDataSaveDBPort = 'DataSaveDBPort';
  sIdentDataSaveDBUser = 'DataSaveDBUser';
  sIdentDataSaveDBPassword = 'DataSaveDBPassword';
  sIdentDataSaveDataBase = 'DataSaveDataBase';
                                       
  sIdentNewLoginDlg = 'NewLoginDlg';
  sIdentNewLoginInto = 'NewLoginInto';
  sIdentNewLoginPhone = 'NewLoginPhone';
  sIdentNewLoginMustHasPhone = 'NewLoginMustHasPhone';
const
  sIdentRandomCode: array[TRandCodeType] of string = ('RandCodeLogin', 'RandCodeLoginReg', 'RandCodePwdGetback', 'RandCodePwdChange');

implementation

uses HUtil32;

{$R *.dfm}

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


procedure TFrmBasicSet.LockSaveButtonEnabled();
begin
  ButtonSave.Enabled := False;
end;

procedure TFrmBasicSet.UnLockSaveButtonEnabled();
begin
  ButtonSave.Enabled := True;
end;

procedure TFrmBasicSet.OpenBasicSet();
var
  I: Integer;
begin
  CheckBoxTestServer.Checked := g_Config.boTestServer;
  CheckBoxEnableMakingID.Checked := g_Config.boEnableMakingID;
  CheckBoxEnableGetbackPassword.Checked := g_Config.boEnableGetbackPassword;
  CheckBoxGetbackPasswordCheckAll.Checked := g_Config.boGetbackPasswordCheckAll;
  chkDisableIDSamePassword.Checked := g_Config.boDisableIDSamePassword;

  chkDisableQuizSameAnswer.Checked := g_Config.boDisableQuizSameAnswer;

  CheckBoxAutoClear.Checked := g_Config.boAutoClearID;
  SpinEditAutoClearTime.Value := g_Config.dwAutoClearTime;

  CheckBoxAutoUnLockAccount.Checked := g_Config.boUnLockAccount;
  SpinEditUnLockAccountTime.Value := g_Config.dwUnLockAccountTime;

  EditGateAddr.Text := g_Config.sGateAddr;
  EditGatePort.Text := IntToStr(g_Config.nGatePort);

  EditServerAddr.Text := g_Config.sServerAddr;
  EditServerPort.Text := IntToStr(g_Config.nServerPort);

  EditMonAddr.Text := g_Config.sMonAddr;
  EditMonPort.Text := IntToStr(g_Config.nMonPort);

  edtControlPort.Text := IntToStr(g_Config.nControlPort);
  edtControlPassword.Text := g_Config.sControlPassword;

  CheckBoxDynamicIPMode.Checked := g_Config.boDynamicIPMode;
  //CheckBoxMinimize.Checked := Config.boMinimize;

  chkShowBlockIPLog.Checked := g_Config.boShowBlockIPLog;

  chkRandomCodeLogin.Checked := g_Config.boRandomCode[rctLogin];
  chkRandomCodeReg.Checked := g_Config.boRandomCode[rctRegister];
  chkRandomCodePwdGetback.Checked := g_Config.boRandomCode[rctPwdGetback];
  chkRandomCodePwdChange.Checked := g_Config.boRandomCode[rctPwdChange];

  seLoginWaveValue.Value := g_Config.btLoginWaveValue;
  seOtherWaveValue.Value := g_Config.btOtherWaveValue;

  EditRandomCodeErrorMaxCount.Value := g_Config.nRandomCodeErrorMaxCount;
  seRandomCodeRefreshMaxCount.Value := g_Config.nRandomCodeRefreshMaxCount;

  chkEnabledL2Password.Checked := g_Config.boEnabledL2Password;
  chkChangedMACCheckL2.Checked := g_Config.boChangedMACCheckL2;
  chkChangedIPCheckL2.Checked := g_Config.boChangedIPCheckL2;
  chkAlwaysCheckL2.Checked := g_config.boAlwaysCheckL2;

  chkDisableIDSameL2Password.Checked := g_Config.boDisableIDSameL2Password;
  chkDisableL2SamePassword.Checked := g_Config.boDisableL2SamePassword;

  chkDisablePwdSameChr.Checked := g_Config.boDisablePwdSameChr;
  chkDisablePwdAllNum.Checked := g_Config.boDisablePwdAllNum;
  chkDisablePwdAllLetter.Checked := g_Config.boDisablePwdAllLetter;
  mmoDisablePassword.Text := g_DisablePasswordList.Text;

  chkNewLoginDlg.Checked := g_Config.boNewLoginDlg;
  chkNewLoginInto.Checked := g_Config.boNewLoginInto;
  chkNewLoginPhone.Checked := g_Config.boNewLoginPhone;
//  chkNewLoginMustHasPhone.Checked := g_Config.boNewLoginMustHasPhone;

  LockSaveButtonEnabled();

  g_ControlIPList.Lock;
  try
    for I := 0 to g_ControlIPList.Count - 1 do
    begin
      lstControlIPList.Items.Add(g_ControlIPList.Strings[I]);
    end;
  finally
    g_ControlIPList.UnLock;
  end;
  ShowModal;
end;

procedure TFrmBasicSet.CheckBoxTestServerClick(Sender: TObject);
begin
  g_Config.boTestServer := CheckBoxTestServer.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.CheckBoxEnableMakingIDClick(Sender: TObject);
begin
  g_Config.boEnableMakingID := CheckBoxEnableMakingID.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.CheckBoxEnableGetbackPasswordClick(Sender: TObject);
begin
  g_Config.boEnableGetbackPassword := CheckBoxEnableGetbackPassword.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.CheckBoxGetbackPasswordCheckAllClick(
  Sender: TObject);
begin
  g_Config.boGetbackPasswordCheckAll := CheckBoxGetbackPasswordCheckAll.Checked;
  UnLockSaveButtonEnabled();
end;


procedure TFrmBasicSet.chkDisableIDSamePasswordClick(Sender: TObject);
begin
  g_Config.boDisableIDSamePassword := chkDisableIDSamePassword.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.CheckBoxAutoClearClick(Sender: TObject);
begin
  g_Config.boAutoClearID := CheckBoxAutoClear.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.SpinEditAutoClearTimeChange(Sender: TObject);
begin
  g_Config.dwAutoClearTime := SpinEditAutoClearTime.Value;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.CheckBoxAutoUnLockAccountClick(Sender: TObject);
begin
  g_Config.boUnLockAccount := CheckBoxAutoUnLockAccount.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.SpinEditUnLockAccountTimeChange(Sender: TObject);
begin
  g_Config.dwUnLockAccountTime := SpinEditUnLockAccountTime.Value;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.ButtonRestoreBasicClick(Sender: TObject);
begin
  CheckBoxTestServer.Checked := True;
  CheckBoxEnableMakingID.Checked := True;
  CheckBoxEnableGetbackPassword.Checked := True;
  CheckBoxAutoClear.Checked := True;
  SpinEditAutoClearTime.Value := 1;
  CheckBoxAutoUnLockAccount.Checked := False;
  SpinEditUnLockAccountTime.Value := 10;

  chkRandomCodeLogin.Checked := False;
  chkRandomCodeReg.Checked := False;
  chkRandomCodePwdGetback.Checked := False;
  chkRandomCodePwdChange.Checked := False;

  EditRandomCodeErrorMaxCount.Value := 3;
end;

procedure TFrmBasicSet.EditGateAddrChange(Sender: TObject);
begin
  g_Config.sGateAddr := Trim(EditGateAddr.Text);
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.EditGatePortChange(Sender: TObject);
begin
  g_Config.nGatePort := Str_ToInt(Trim(EditGatePort.Text), 5500);
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.EditMonAddrChange(Sender: TObject);
begin
  g_Config.sMonAddr := Trim(EditMonAddr.Text);
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.EditMonPortChange(Sender: TObject);
begin
  g_Config.nMonPort := Str_ToInt(Trim(EditMonPort.Text), 3000);
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.EditServerAddrChange(Sender: TObject);
begin
  g_Config.sServerAddr := Trim(EditServerAddr.Text);
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.EditServerPortChange(Sender: TObject);
begin
  g_Config.nServerPort := StrToIntDef(Trim(EditServerPort.Text), 5600);
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.CheckBoxDynamicIPModeClick(Sender: TObject);
begin
  g_Config.boDynamicIPMode := CheckBoxDynamicIPMode.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.ButtonRestoreNetClick(Sender: TObject);
begin
  EditGateAddr.Text := '0.0.0.0';
  EditGatePort.Text := '5500';
  EditServerAddr.Text := '0.0.0.0';
  EditServerPort.Text := '5600';
  EditMonAddr.Text := '0.0.0.0';
  EditMonPort.Text := '3000';
  CheckBoxDynamicIPMode.Checked := False;
end;

procedure WriteConfig(Config: pTConfig);
  procedure WriteConfigString(sSection, sIdent, sDefault: string);
  begin
    Config.IniConf.WriteString(sSection, sIdent, sDefault);
  end;
  procedure WriteConfigInteger(sSection, sIdent: string; nDefault: Integer);
  begin
    Config.IniConf.WriteInteger(sSection, sIdent, nDefault);
  end;
  procedure WriteConfigBoolean(sSection, sIdent: string; boDefault: Boolean);
  begin
    Config.IniConf.WriteBool(sSection, sIdent, boDefault);
  end;
var
  RandCodeType: TRandCodeType;
begin
  WriteConfigString(sSectionServer, sIdentDBServer, Config.sDBServer);
  WriteConfigString(sSectionServer, sIdentFeeServer, Config.sFeeServer);
  WriteConfigString(sSectionServer, sIdentLogServer, Config.sLogServer);

  WriteConfigString(sSectionServer, sIdentGateAddr, Config.sGateAddr);
  WriteConfigInteger(sSectionServer, sIdentGatePort, Config.nGatePort);
  WriteConfigString(sSectionServer, sIdentServerAddr, Config.sServerAddr);
  WriteConfigInteger(sSectionServer, sIdentServerPort, Config.nServerPort);
  WriteConfigString(sSectionServer, sIdentMonAddr, Config.sMonAddr);
  WriteConfigInteger(sSectionServer, sIdentMonPort, Config.nMonPort);

  WriteConfigString(sSectionServer, sIdentControlPassword, Config.sControlPassword);
  WriteConfigInteger(sSectionServer, sIdentControlPort, Config.nControlPort);

  WriteConfigBoolean(sSectionServer, sIdentShowBlockIPLog, g_Config.boShowBlockIPLog);

  WriteConfigInteger(sSectionServer, sIdentDBSPort, Config.nDBSPort);
  WriteConfigInteger(sSectionServer, sIdentFeePort, Config.nFeePort);
  WriteConfigInteger(sSectionServer, sIdentLogPort, Config.nLogPort);
  WriteConfigInteger(sSectionServer, sIdentReadyServers, Config.nReadyServers);
  WriteConfigBoolean(sSectionServer, sIdentEnableMakingID, Config.boEnableMakingID);
  WriteConfigBoolean(sSectionServer, sIdentTestServer, Config.boTestServer);

  WriteConfigBoolean(sSectionServer, sIdentEnableGetbackPassword, Config.boEnableGetbackPassword);
  WriteConfigBoolean(sSectionServer, sIdentGetbackPasswordCheckAll, Config.boGetbackPasswordCheckAll);
  WriteConfigBoolean(sSectionServer, sIdentDisableIDSamePassword, Config.boDisableIDSamePassword);
  WriteConfigBoolean(sSectionServer, sIdentDisableQuizSameAnswer, Config.boDisableQuizSameAnswer);

  WriteConfigBoolean(sSectionServer, sIdentDisableIDSameL2Password, Config.boDisableIDSameL2Password);
  WriteConfigBoolean(sSectionServer, sIdentDisableL2SamePassword, Config.boDisableL2SamePassword);

  WriteConfigBoolean(sSectionServer, sIdentDisablePwdSameChr, Config.boDisablePwdSameChr);
  WriteConfigBoolean(sSectionServer, sIdentDisablePwdAllNum, Config.boDisablePwdAllNum);
  WriteConfigBoolean(sSectionServer, sIdentDisablePwdAllLetter, Config.boDisablePwdAllLetter);

  WriteConfigBoolean(sSectionServer, sIdentAutoClearID, Config.boAutoClearID);
  WriteConfigInteger(sSectionServer, sIdentAutoClearTime, Config.dwAutoClearTime);
  WriteConfigBoolean(sSectionServer, sIdentUnLockAccount, Config.boUnLockAccount);
  WriteConfigInteger(sSectionServer, sIdentUnLockAccountTime, Config.dwUnLockAccountTime);

  WriteConfigBoolean(sSectionServer, sIdentDynamicIPMode, Config.boDynamicIPMode);
 // WriteConfigBoolean(sSectionServer, sIdentMinimize, Config.boMinimize);

  for RandCodeType := Low(TRandCodeType) to High(TRandCodeType) do
  begin
    WriteConfigBoolean(sSectionServer, sIdentRandomCode[RandCodeType], Config.boRandomCode[RandCodeType]);
  end;

  WriteConfigInteger(sSectionServer, sIdentLoginWaveValue, g_Config.btLoginWaveValue);
  WriteConfigInteger(sSectionServer, sIdentOtherWaveValue, g_Config.btOtherWaveValue);

  WriteConfigInteger(sSectionServer, sIdentRandomCodeErrorMaxCount, Config.nRandomCodeErrorMaxCount);
  WriteConfigInteger(sSectionServer, sIdentRandomCodeRefreshMaxCount, Config.nRandomCodeRefreshMaxCount);

  WriteConfigString(sSectionDB, sIdentIdDir, Config.sIdDir);
  WriteConfigString(sSectionDB, sIdentWebLogDir, Config.sWebLogDir);
  WriteConfigString(sSectionDB, sIdentCountLogDir, Config.sCountLogDir);
  WriteConfigString(sSectionDB, sIdentFeedIDList, Config.sFeedIDList);
  WriteConfigString(sSectionDB, sIdentFeedIPList, Config.sFeedIPList);

  WriteConfigBoolean(sSectionServer, sIdentEnabledL2Password, Config.boEnabledL2Password);
  WriteConfigBoolean(sSectionServer, sIdentChangedMACCheckL2, Config.boChangedMACCheckL2);
  WriteConfigBoolean(sSectionServer, sIdentChangedIPCheckL2, Config.boChangedIPCheckL2);
  WriteConfigBoolean(sSectionServer, sIdentAlwaysCheckL2, Config.boAlwaysCheckL2);
                                          
  WriteConfigBoolean(sSectionServer, sIdentNewLoginDlg, Config.boNewLoginDlg);
  WriteConfigBoolean(sSectionServer, sIdentNewLoginInto, Config.boNewLoginInto);
  WriteConfigBoolean(sSectionServer, sIdentNewLoginPhone, Config.boNewLoginPhone);
  WriteConfigBoolean(sSectionServer, sIdentNewLoginMustHasPhone, Config.boNewLoginMustHasPhone);
end;

procedure TFrmBasicSet.ButtonSaveClick(Sender: TObject);
begin
  WriteConfig(@g_Config);

  g_DisablePasswordList.Text := mmoDisablePassword.Text;
  g_DisablePasswordList.SaveToFile(g_DisablePasswordFile);

  LockSaveButtonEnabled();
end;

procedure TFrmBasicSet.ButtonCloseClick(Sender: TObject);
begin
 Close;
end;

procedure TFrmBasicSet.chkRandomCodeLoginClick(Sender: TObject);
begin
  g_Config.boRandomCode[rctLogin] := chkRandomCodeLogin.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.seRandomCodeRefreshMaxCountChange(Sender: TObject);
begin
  g_Config.nRandomCodeRefreshMaxCount := seRandomCodeRefreshMaxCount.Value;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkRandomCodeRegClick(Sender: TObject);
begin
  g_Config.boRandomCode[rctRegister] := chkRandomCodeReg.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkRandomCodePwdGetbackClick(Sender: TObject);
begin
  g_Config.boRandomCode[rctPwdGetback] := chkRandomCodePwdGetback.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkRandomCodePwdChangeClick(Sender: TObject);
begin
  g_Config.boRandomCode[rctPwdChange] := chkRandomCodePwdChange.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.EditRandomCodeErrorMaxCountChange(Sender: TObject);
begin
  g_Config.nRandomCodeErrorMaxCount := EditRandomCodeErrorMaxCount.Value;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkDisableQuizSameAnswerClick(Sender: TObject);
begin
  g_Config.boDisableQuizSameAnswer := chkDisableQuizSameAnswer.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.edtControlPortChange(Sender: TObject);
begin
  g_Config.nControlPort := Str_ToInt(Trim(edtControlPort.Text), 0);
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.edtControlPasswordChange(Sender: TObject);
begin
  g_Config.sControlPassword := Trim(edtControlPassword.Text);
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.mniIPAddClick(Sender: TObject);
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

procedure TFrmBasicSet.mniIPDeleteClick(Sender: TObject);
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

procedure TFrmBasicSet.mniIPClearClick(Sender: TObject);
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

procedure TFrmBasicSet.ErrMessage(MsgStr: string);
begin
  Application.MessageBox(PChar(MsgStr), '错误', MB_OK or MB_ICONERROR);
end;

procedure TFrmBasicSet.FormCreate(Sender: TObject);
begin
  PageControl1.ActivePageIndex := 0;
end;

procedure TFrmBasicSet.chkChangedMACCheckL2Click(Sender: TObject);
begin
  g_Config.boChangedMACCheckL2 := chkChangedMACCheckL2.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkChangedIPCheckL2Click(Sender: TObject);
begin
  g_Config.boChangedIPCheckL2 := chkChangedIPCheckL2.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkAlwaysCheckL2Click(Sender: TObject);
begin
  g_Config.boAlwaysCheckL2 := chkAlwaysCheckL2.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkDisableIDSameL2PasswordClick(Sender: TObject);
begin
  g_Config.boDisableIDSameL2Password := chkDisableIDSameL2Password.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkDisableL2SamePasswordClick(Sender: TObject);
begin
  g_Config.boDisableL2SamePassword := chkDisableL2SamePassword.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkEnabledL2PasswordClick(Sender: TObject);
begin
  g_Config.boEnabledL2Password := chkEnabledL2Password.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkDisablePwdSameChrClick(Sender: TObject);
begin
  g_Config.boDisablePwdSameChr := chkDisablePwdSameChr.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkDisablePwdAllNumClick(Sender: TObject);
begin
  g_Config.boDisablePwdAllNum := chkDisablePwdAllNum.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkDisablePwdAllLetterClick(Sender: TObject);
begin
  g_Config.boDisablePwdAllLetter := chkDisablePwdAllLetter.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.mmoDisablePasswordChange(Sender: TObject);
begin
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.seLoginWaveValueChange(Sender: TObject);
begin
  g_Config.btLoginWaveValue := seLoginWaveValue.Value;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.seOtherWaveValueChange(Sender: TObject);
begin
  g_Config.btOtherWaveValue := seOtherWaveValue.Value;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkShowBlockIPLogClick(Sender: TObject);
begin
  g_Config.boShowBlockIPLog := chkShowBlockIPLog.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkNewLoginDlgClick(Sender: TObject);
begin
  g_Config.boNewLoginDlg := chkNewLoginDlg.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkNewLoginIntoClick(Sender: TObject);
begin
  g_Config.boNewLoginInto := chkNewLoginInto.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkNewLoginPhoneClick(Sender: TObject);
begin
  g_Config.boNewLoginPhone := chkNewLoginPhone.Checked;
  UnLockSaveButtonEnabled();
end;

procedure TFrmBasicSet.chkNewLoginMustHasPhoneClick(Sender: TObject);
begin
//  g_Config.boNewLoginMustHasPhone := chkNewLoginMustHasPhone.Checked;
//  UnLockSaveButtonEnabled();
end;

end.

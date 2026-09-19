unit AppMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, Menus, StdCtrls, ExtCtrls, Grids, MsXML, DateUtils, ComCtrls, IniFiles,
  IOCPManager, ClientThread, AcceptExWorkedThread, Protocol, SDK, EDcode, Misc;

const
  WM_SET_PARENT_WINDOW = WM_USER + 123; // 嵌入到控制台


type
  TFormMain = class(TForm)
    MainMenu: TMainMenu;
    MENU_CONTROL: TMenuItem;
    MENU_CONTROL_START: TMenuItem;
    MENU_CONTROL_STOP: TMenuItem;
    MENU_CONTROL_RELOADCONFIG: TMenuItem;
    MENU_CONTROL_CLEAELOG: TMenuItem;
    MENU_CONTROL_EXIT: TMenuItem;
    MENU_OPTION: TMenuItem;
    MENU_OPTION_GENERAL: TMenuItem;
    MENU_OPTION_IPFILTER: TMenuItem;
    MENU_VIEW_HELP: TMenuItem;
    MENU_VIEW_HELP_ABOUT: TMenuItem;
    GridSocketInfo: TStringGrid;
    Splitter: TSplitter;
    MemoLog: TMemo;
    StatusBar: TStatusBar;
    tmrVerify: TTimer;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure MENU_VIEW_HELP_ABOUTClick(Sender: TObject);
    procedure MENU_CONTROL_STARTClick(Sender: TObject);
    procedure MENU_CONTROL_STOPClick(Sender: TObject);
    procedure MENU_CONTROL_EXITClick(Sender: TObject);
    procedure MemoLogDblClick(Sender: TObject);
    procedure GridSocketInfoDrawCell(Sender: TObject; ACol, ARow: Integer; Rect: TRect; State: TGridDrawState);
    procedure MENU_OPTION_GENERALClick(Sender: TObject);
    procedure MENU_OPTION_IPFILTERClick(Sender: TObject);
    procedure MENU_CONTROL_CLEAELOGClick(Sender: TObject);
    procedure MENU_CONTROL_RELOADCONFIGClick(Sender: TObject);
    procedure tmrVerifyTimer(Sender: TObject);
  private
    FIsEmbeddedGameCenter: Boolean;
    procedure WMSysCommand(var Message: TWMSysCommand); message WM_SYSCOMMAND;
    procedure WMSetParentWindow(var Message: TMessage); message WM_SET_PARENT_WINDOW;

    procedure OnAppModalBegin(Sender: TObject);
    procedure OnAppOnModalEnd(Sender: TObject);
  private
    { Private declarations }
    procedure OnProgramException(Sender: TObject; E: Exception);
    procedure MyMessage(var MsgData: TWmCopyData); message WM_COPYDATA;
{$IF VER_TYPE = 1}
  private
    FConnectVerifyServerTick: LongWord;
    FIsVerifyServerResult: Boolean;

    FConnectVerifyServerIndex: Integer;
    FVerifyDataKey: DWORD;
    FVerifyDataText: string;

    FRetryConnectVerifyServerTime: LongWord;
    FRetryConnectVerifyServerCount: Integer;
    FIsCheckRetryVerifyServerResult: Boolean;
    FIsRetryVerifyServerResult: Boolean;

    FTimeLeftStartTick: LongWord;
    FTimeLeft: Int64;
    FIsShowTimeLeft: Boolean;

    FClientSocetkVerify: TClientSocket;

    procedure OnClientSocetkVerifyConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure OnClientSocetkVerifyError(Sender: TObject;
      Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
      var ErrorCode: Integer);
    procedure OnClientSocetkVerifyRead(Sender: TObject; Socket: TCustomWinSocket);
{$IFEND}
  public
    m_xRunServerList: TIOCPManager;
    m_xGameServerList: TGameServerManager;
    procedure InitIOCPServer();
    procedure CltOnRead(ClientThread: TClientThread; const Buffer: PChar; const BufLen: UINT);
    procedure CltOnClose(Sender: TObject);
    procedure ClientClosed(UserOBJ: pTUserOBJ; wParam, lParam: DWORD);
    procedure UserEnterEvent(Sender: TUserManager; const UserOBJ: pTUserOBJ); //用户进入事件
    procedure UserLeaveEvent(Sender: TUserManager; const UserOBJ: pTUserOBJ); //用户离开事件
    procedure UserReadBuffer(UserOBJ: TObject; const Buffer: PChar; var BufLen: DWORD; var Succeed: BOOL);
  end;

var
  FormMain: TFormMain;

implementation

{$R *.dfm}

uses
  FuncForComm,
  ClientSession,
  Grobal2,
  ConfigManager,
  LogManager,
  HUtil32,
  IPAddrFilter,
  GeneralConfig,
  PacketRuleConfig,
  {$IF VER_TYPE = 1}
  WinlicenseSDK,
  DesUtils,
  CheckUnit,
  LbAsym,
  LbRSA,
  {$IFEND}
  SHSocket;

procedure TFormMain.OnProgramException(Sender: TObject; E: Exception);
begin
  g_pLogMgr.Add(E.Message);
end;

procedure TFormMain.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
begin
  if g_fCanClose then
    Exit;
  if Application.MessageBox('是否确认退出服务器？', '确认信息', MB_YESNO + MB_ICONQUESTION) = IDYES then
  begin
    if g_fServiceStarted then
    begin
      StopService();
      Sleep(20);
      CanClose := True;
    end;
  end
  else
    CanClose := False;
end;

const
  ArrKey: array[0..127] of Byte = (
    $03, $A2, $EC, $24, $C3, $B1, $45, $58, $E0, $43, $48, $D2, $9A, $52, $BE,
    $52, $67, $35, $AF, $30, $9D, $DF, $0B, $5B, $9B, $1B, $97, $8B, $22, $BF,
    $1D, $4E, $15, $9B, $38, $D3, $DF, $11, $71, $14, $EA, $5A, $CC, $D8, $B3,
    $38, $DB, $C2, $44, $2E, $6D, $92, $4A, $0F, $7B, $F3, $E4, $B7, $48, $0E,
    $65, $2C, $92, $B3, $DF, $F8, $93, $13, $BA, $54, $28, $99, $76, $39, $24,
    $23, $BC, $05, $DB, $C2, $30, $5C, $DD, $F5, $54, $9F, $D2, $59, $99, $96,
    $39, $5F, $C6, $A4, $D6, $C4, $A8, $CF, $3A, $F4, $42, $EB, $6C, $F4, $8B,
    $35, $F6, $C4, $9A, $57, $78, $FC, $CD, $F9, $2F, $51, $A8, $C9, $49, $72,
    $73, $B6, $09, $CE, $DA, $60, $8E, $66
    );

procedure TFormMain.FormCreate(Sender: TObject);
const
  RSA_KEY_ENC: array[0..127] of Byte = (
    $99, $51, $75, $D3, $38, $B3, $4D, $B8, $6F, $CC, $C7, $5D, $15, $DD, $31, $DD,
    $E8, $BA, $20, $BF, $E7, $40, $28, $1C, $13, $E0, $8B, $36, $4D, $73, $DA, $13,
    $00, $46, $09, $0E, $37, $AB, $51, $AB, $94, $D5, $4A, $E6, $79, $1D, $85, $2C,
    $09, $5D, $B7, $81, $4A, $49, $72, $FD, $D3, $1C, $19, $A5, $33, $7F, $E7, $3E,
    $21, $EB, $3B, $81, $B3, $09, $9F, $18, $3C, $70, $56, $DE, $6F, $19, $C2, $67,
    $65, $EB, $21, $D0, $88, $EA, $EC, $C8, $2A, $9F, $A6, $47, $FA, $D4, $80, $1A,
    $C7, $D6, $F8, $93, $18, $44, $20, $05, $F0, $46, $AA, $E7, $B0, $C8, $DE, $BB,
    $37, $2D, $AF, $4B, $6F, $1F, $B1, $E1, $09, $0C, $98, $DE, $AC, $2A, $85, $19
    );
var
  nX, nY: Integer;

{$IF VER_TYPE = 1}
  LicenseDataInfo: TLicenseDataInfo;
  Hash, EncKey: DWORD;
  SrvMsgClientFlag: TServerMessageClientFlag;
  I, Len, ExtendedInfo: Integer;

  HWID: array[0..100] of Char;
  UserName: array[0..100] of Char;
  Organization: array[0..100] of Char;
  CustomData: array[0..102400] of Char;

  MachineID: array[0..7] of DWORD;

  IsOK, IsKeyOK, IsEnabledAntiPlug: Boolean;
  C1, C2: Char;
  S, TempStr, Password: string;
  FileHandle: THandle;
  AddDataEnc: TLoginGateAddDataEnc;
  AddData: TLoginGateAddDataIn;

  RSA: TLbRSA;
  nKey: LongWord;
  RSAKey: array[0..127] of Byte;
  OutBufSize: Integer;
  MS: TMemoryStream;
{$IFEND}
begin
  {asm
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  end;
  }
  Application.OnException := OnProgramException;
  g_ProcMsgThread := TProcMsgThread.Create;

  g_hMainWnd := Self.Handle;
  g_hGameCenterHandle := Str_ToInt(ParamStr(1), 0); //运行到此，g_hGameCenterHandle值是 0
  nX := Str_ToInt(ParamStr(2), -1);
  nY := Str_ToInt(ParamStr(3), -1);
  if (nX >= 0) or (nY >= 0) then
  begin
    Left := nX;
    Top := nY;
  end;
  g_pLogMgr := TLogMgr.Create(MemoLog.Handle);
  g_pConfig := TConfigMgr.Create(_STR_CONFIG_FILE);

  FIsEmbeddedGameCenter := False;

  Application.OnModalBegin := OnAppModalBegin;
  Application.OnModalEnd := OnAppOnModalEnd;

  SendGameCenterMsg(SG_FORMHANDLE, IntToStr(g_hMainWnd));

  GridSocketInfo.Cells[0, 0] := _STR_GRID_IP;
  GridSocketInfo.Cells[1, 0] := _STR_GRID_PORT;
  GridSocketInfo.Cells[2, 0] := _STR_GRID_CONNECT_STATUS;
  GridSocketInfo.Cells[3, 0] := _STR_GRID_ONLINE_USER;

  mainhwnd := Handle;

{$IF VER_TYPE = 1}
{.$I Registered_Start.inc}
  if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
  begin
    FIsVerifyServerResult := False;
    FConnectVerifyServerTick := 0;
    FConnectVerifyServerIndex := 0;

    FRetryConnectVerifyServerTime := 8 * 60 * 60 * 1000 + Random(16 * 60 * 60 * 1000);
    FRetryConnectVerifyServerCount := 0;
    FIsCheckRetryVerifyServerResult := False;
    FIsRetryVerifyServerResult := False;

    FTimeLeft := 0;
    FIsShowTimeLeft := False;

    IsOK := False;
    IsKeyOK := False;
    IsEnabledAntiPlug := False;

    FillChar(UserName, SizeOf(UserName), 0);
    FillChar(Organization, SizeOf(Organization), 0);
    FillChar(CustomData, SizeOf(CustomData), 0);

    WLRegGetLicenseInfo(UserName, Organization, CustomData);
    S := StrPas(CustomData);

    if (Length(S) > 0) and (Length(S) mod 2 = 0) then
    begin
      Len := Length(S) div 2;
      SetLength(TempStr, Len);

      IsOK := True;
      if Len <> SizeOf(LicenseDataInfo) then
        IsOK := False
      else
      begin
        for I := 1 to Len do
        begin
          C1 := S[I * 2 - 1];
          C2 := S[I * 2];
          if (C1 in ['0'..'9', 'a'..'f', 'A'..'F']) and (C2 in ['0'..'9', 'a'..'f', 'A'..'F']) then
          begin
            TempStr[I] := Chr(StrToInt('$' + C1 + C2))
          end
          else
          begin
            IsOK := False;
            Break;
          end;
        end;
      end;

      if IsOK then
      begin
        Password := '!~@#geem220160125*&^';
        WLBufferDecrypt(PChar(TempStr), Length(TempStr), PChar(Password));
        Move(TempStr[1], LicenseDataInfo, SizeOf(LicenseDataInfo));

        WLHardwareGetID(HWID);

        for I := 0 to 7 do
        begin
          MachineID[I] := StrToIntDef('$' + Copy(HWID, I * 5 + 1, 4), 0);
        end;

        if LicenseDataInfo.SrvMsgClientFlagCRC_Enc = BufferCrc(@LicenseDataInfo.SrvMsgClientFlag, SizeOf(TServerMessageClientFlag)) then
        begin
          Hash := 0;
          for I := Low(LicenseDataInfo.RandomCode1Arr) to High(LicenseDataInfo.RandomCode1Arr) do
          begin
            Hash := Hash xor (not ((Hash shl 11) xor LicenseDataInfo.RandomCode1Arr[I] xor (Hash shr 5)));
          end;

          EncKey := Hash and LicenseDataInfo.RandomCode1Arr[1] or LicenseDataInfo.RandomCode1Arr[7];
          EncKey := EncKey and (LicenseDataInfo.RandomCode1Arr[2] shl 2);
          EncKey := EncKey or LicenseDataInfo.RandomCode1Arr[5];
          EncKey := EncKey or LicenseDataInfo.RandomCode1Arr[4];
          EncKey := EncKey xor LicenseDataInfo.RandomCode1Arr[10];
          EncKey := (EncKey shr 2) or LicenseDataInfo.RandomCode1Arr[12];
          EncKey := EncKey and (LicenseDataInfo.RandomCode1Arr[3] shr 5);
          EncKey := EncKey or (LicenseDataInfo.RandomCode1Arr[15] shl 3);
          EncKey := (EncKey and LicenseDataInfo.RandomCode1Arr[19]) shl 7;
          EncKey := EncKey and LicenseDataInfo.RandomCode1Arr[18];
          EncKey := (EncKey or LicenseDataInfo.RandomCode1Arr[9]) shr 3;
          EncKey := EncKey and LicenseDataInfo.RandomCode1Arr[7];
          EncKey := EncKey xor LicenseDataInfo.RandomCode1Arr[6];
          EncKey := EncKey or (LicenseDataInfo.RandomCode1Arr[14] shl 8);
          EncKey := EncKey and LicenseDataInfo.RandomCode1Arr[13];
          EncKey := EncKey xor LicenseDataInfo.RandomCode1Arr[11];

          DecryptDes_New(LicenseDataInfo.SrvMsgClientFlag, SrvMsgClientFlag, SizeOf(TServerMessageClientFlag), IntToStr((EncKey xor MachineID[6] xor MachineID[5]) shl 3));

          if LicenseDataInfo.EnabledAntiPlug then
          begin
            IsEnabledAntiPlug := True;
            nKey := MakeLong(SrvMsgClientFlag.smSendNotice, SrvMsgClientFlag.smSendCustomMagicConfig) xor MakeLong(SrvMsgClientFlag.smModuleMd5Cache, SrvMsgClientFlag.smStdItemList);
          end;

{$IF VER_TYPE1_TEST = 1}
          g_pLogMgr.Add('nKey: ' + IntToStr(nKey));
{$IFEND}
        end;
      end;
    end;

{$IF VER_TYPE1_TEST = 1}
    g_pLogMgr.Add('~~~~~~~~~~~~~~~1111');
{$IFEND}

    if IsOK and IsEnabledAntiPlug then
    begin
{$IF VER_TYPE1_TEST = 1}
      FileHandle := FileOpen(ExtractFilePath(Application.ExeName) + 'LoginGate.tst', FmOpenRead or fmShareDenyNone);
{$ELSE}
      FileHandle := FileOpen(ParamStr(0), FmOpenRead or fmShareDenyNone);
{$IFEND}

      if FileHandle <> THandle(-1) then
      begin
{$IF VER_TYPE1_TEST = 1}
        g_pLogMgr.Add('~~~~~~~~~~~~~~~2222');
{$IFEND}

        FileSeek(FileHandle, -SizeOf(TLoginGateAddDataEnc), soFromEnd);
        if (FileRead(FileHandle, AddDataEnc, SizeOf(AddDataEnc)) = SizeOf(AddDataEnc)) and
          (AddDataEnc.HeaderFlag = $F4516A51) and (AddDataEnc.CRC = BufferCrc(@AddDataEnc.Data[0], SizeOf(AddDataEnc.Data))) then
        begin
            // 解密rsa的私钥
            //nKey := MakeLong(SM_SENDNOTICE, SM_SENDCUSTOMMAGICCONFIG) xor MakeLong(SM_MODULEMD5_CACHE, SM_STDITEMLIST);
          DecryptDes_New(RSA_KEY_ENC[0], RSAKey[0], SizeOf(RSA_KEY_ENC), IntToStr(nKey));

{$IF VER_TYPE1_TEST = 1}
          g_pLogMgr.Add('~~~~~~~~~~~~~~~3333');
{$IFEND}

          RSA := TLbRSA.Create(nil);
          try
            RSA.KeySize := aks1024;
            RSA.PrivateKey.ModulusAsString :=
              '1D688E95039F11EB594DCE404DDBFDCA' +
              'CCD8F657BA2350DF4363B75979964B0D' +
              '919039C4BECE6BF29703EC4E49262A9D' +
              'B7C006DBC59588EF00BE34D01AF75226' +
              '3132D88DB78B4F9E4BDBCF561AFB2DD4' +
              'E04ACCB30F6EE7823FAABE781A653887' +
              'B4F0DE62A26E990F8E012C304D45FAD8' +
              '7F14CD00BEF0A7788E51F9AD0F6AAF84';

            RSA.PrivateKey.Exponent.AppendBuffer(RSAKey[0], SizeOf(RSAKey));

              {
              RSA.PrivateKey.ExponentAsString :=
                '03A2EC24C3B14558E04348D29A52BE52' +
                '6735AF309DDF0B5B9B1B978B22BF1D4E' +
                '159B38D3DF117114EA5ACCD8B338DBC2' +
                '442E6D924A0F7BF3E4B7480E652C92B3' +
                'DFF89313BA54289976392423BC05DBC2' +
                '305CDDF5549FD2599996395FC6A4D6C4' +
                'A8CF3AF442EB6CF48B35F6C49A5778FC' +
                'CDF92F51A8C9497273B609CEDA608E66';

              // publickey  AB20
              }

            OutBufSize := RSA.DecryptBuffer(AddDataEnc.Data[0], SizeOf(AddDataEnc.Data), CustomData[0]);

{$IF VER_TYPE1_TEST = 1}
            g_pLogMgr.Add('~~~~~~~~~~~~~~~4444');
{$IFEND}

            if OutBufSize = SizeOf(AddData) then
            begin
              Move(CustomData[0], AddData, SizeOf(AddData));

              g_nProtocolKey1 := AddData.Key1;
              g_nProtocolKey2 := AddData.Key2;

{$IF VER_TYPE1_TEST = 1}
              g_pLogMgr.Add('Key1: ' + IntToStr(AddData.Key1));
              g_pLogMgr.Add('Key2: ' + IntToStr(AddData.Key2));
{$IFEND}
              IsKeyOK := True;

              tmrVerify.Enabled := True;
            end;
          finally
            RSA.Free;
          end;
        end;
      end;
    end;

    if not IsKeyOK then
    begin
      if not IsOK then
        Register_Str := WLStringDecrypt('授权失败')
      else if not IsEnabledAntiPlug then
        Register_Str := WLStringDecrypt('授权文件不支持此版本')
      else if not IsKeyOK then
        Register_Str := WLStringDecrypt('程序损坏');
    end;
  end
  else
  begin
    Register_Str := WLStringDecrypt('未授权');
  end;
{.$I Registered_end.inc}

{$IFEND}

  LoadBlockIPList();
  LoadBlockIPAreaList();

  SetTimer(g_hMainWnd, _IDM_TIMER_STARTSERVICE, 100, Pointer(@OnTimerProc));
end;

procedure TFormMain.FormDestroy(Sender: TObject);
begin
  g_pConfig.Free;
  g_pLogMgr.Free;
  g_ProcMsgThread.Free;
end;

procedure TFormMain.GridSocketInfoDrawCell(Sender: TObject; ACol, ARow: Integer; Rect: TRect; State: TGridDrawState);
begin
  with Sender as TStringGrid do
  begin
    Canvas.FillRect(Rect);
    DrawText(Canvas.Handle,
      PChar(Cells[ACol, ARow]),
      Length(Cells[ACol, ARow]),
      Rect, // 包含文字的矩形
      DT_CENTER or // 水平居中 DT_RIGHT 水平居右
      DT_SINGLELINE or // 不折行
      DT_VCENTER); // 垂直居中
  end;
end;

procedure TFormMain.MemoLogDblClick(Sender: TObject);
begin
  if MemoLog.Lines.Count = 0 then
    Exit;
  if Application.MessageBox('是否确认清除显示的日志信息？', '确认信息', MB_OKCANCEL + MB_ICONQUESTION) <> IDOK then
    Exit;
  MemoLog.Clear;
end;

procedure TFormMain.MENU_CONTROL_CLEAELOGClick(Sender: TObject);
begin
  MemoLogDblClick(nil);
end;

procedure TFormMain.MENU_CONTROL_EXITClick(Sender: TObject);
begin
  Close;
end;

procedure TFormMain.MENU_CONTROL_RELOADCONFIGClick(Sender: TObject);
begin
  g_pConfig.LoadConfig();
  if g_pConfig.m_szTitle <> '' then begin
      Caption := Format('LoginGate - [%s]', [g_pConfig.m_szTitle]);
  end else begin
      Caption := 'LoginGate';
  end;

{$IF VER_TYPE = 1}
  StrEncryptStart;
  if Register_Str <> '' then
  begin
    Caption := Caption + '【' + Register_Str + '】';
  end;
  StrEncryptEnd;
{$IFEND}

  LoadBlockIPList();
  LoadBlockIPAreaList();

  g_pLogMgr.Add('重新加载配置完成...');
end;

procedure TFormMain.MENU_CONTROL_STARTClick(Sender: TObject);
begin
  StartService();
end;

procedure TFormMain.MENU_CONTROL_STOPClick(Sender: TObject);
begin
  StopService();
end;

procedure TFormMain.MENU_OPTION_GENERALClick(Sender: TObject);
begin
  with frmGeneralConfig, g_pConfig do
  begin
    frmGeneralConfig.Top := Self.Top + 20;
    frmGeneralConfig.Left := Self.Left;
    m_Showed := False;
    EditTitle.Text := m_szTitle;
    TrackBarLogLevel.Position := m_nShowLogLevel;

    chkClientSoftVer.Checked := m_boCheckVersion;
    edtClientSoftVer.Text := m_sClientSoftVer;

    speGateCount.Value := m_nGateCount;
    speGateIdx.Value := 1;
    EditServerIPaddr.Text := g_pConfig.m_xGameGateList[1].sServerAdress;
    EditServerPort.Text := IntToStr(g_pConfig.m_xGameGateList[1].nServerPort);
    EditGatePort.Text := IntToStr(g_pConfig.m_xGameGateList[1].nGatePort);
    m_Showed := True;
    ShowModal;
  end;
end;

procedure TFormMain.MENU_OPTION_IPFILTERClick(Sender: TObject);
var
  I, n: Integer;
  UserOBJ: TSessionObj;
begin
  with frmPacketRule, g_pConfig do
  begin
    m_ShowOpen := False;
    frmPacketRule.Top := Self.Top + 20;
    frmPacketRule.Left := Self.Left;

    ListBoxActiveList.Clear;
    if g_fServiceStarted then
    begin
      for n := 0 to USER_ARRAY_COUNT - 1 do
      begin
        UserOBJ := g_UserList[n];
        if (UserOBJ <> nil) and (UserOBJ.m_tLastGameSvr <> nil) and (UserOBJ.m_tLastGameSvr.Active) and not UserOBJ.m_fKickFlag then
          ListBoxActiveList.Items.AddObject(Trim(UserOBJ.m_pUserOBJ.pszIPAddr), TObject(UserOBJ));
      end;
    end;
    ListBoxTempList.Clear;
    for I := 0 to g_TempBlockIPList.Count - 1 do
      ListBoxTempList.Items.AddObject(g_TempBlockIPList.Strings[I], g_TempBlockIPList.Objects[I]);

    ListBoxBlockList.Clear;
    for I := 0 to g_BlockIPList.Count - 1 do
      ListBoxBlockList.Items.AddObject(g_BlockIPList.Strings[I], g_BlockIPList.Objects[I]);

    ListBoxIPAreaFilter.Clear;
    for I := 0 to g_BlockIPAreaList.Count - 1 do
      ListBoxIPAreaFilter.Items.AddObject(g_BlockIPAreaList.Strings[I], g_BlockIPAreaList.Objects[I]);

    etMaxConnectOfIP.Value := m_nMaxConnectOfIP;
    etClientTimeOutTime.Value := m_nClientTimeOutTime div 1000;
    case m_tBlockIPMethod of
      mDisconnect: rdDisConnect.Checked := True;
      mBlock: rdAddTempList.Checked := True;
      mBlockList: rdAddBlockList.Checked := True;
    end;
    etNomClientPacketSize.Value := m_nNomClientPacketSize;
    etMaxClientMsgCount.Value := m_nMaxClientPacketCount;
    cbKickOverPacketSize.Checked := m_fKickOverPacketSize;
    cbCheckNullConnect.Checked := m_fCheckNullSession;
    cbDefenceCC.Checked := m_fDefenceCCPacket;
    cbCheckNewIDOfIP.Checked := m_fCheckNewIDOfIP;

    TrackBarIDLimitLevel.Position := m_nCheckNewIDOfIP;
    TrackBarIDLimitLevelChange(TrackBarIDLimitLevel);

    pcProcessPack.ActivePageIndex := 0;
    m_ShowOpen := True;
    btnSave.Enabled := False;
    ShowModal;
  end;
end;

procedure TFormMain.MENU_VIEW_HELP_ABOUTClick(Sender: TObject);
begin
{$IF VER_TYPE = 0}
   g_pLogMgr.Add('程序名称: ' + PROGRAM_NAME);
{$ELSE}
  if Register_Str = '' then
    g_pLogMgr.Add('程序名称: ' + PROGRAM_NAME)
  else
    g_pLogMgr.Add('程序名称: ' + PROGRAM_NAME + '【' + Register_Str + '】');
{$IFEND}
  g_pLogMgr.Add('程序版本: ' + VER_VERSION);
//  g_pLogMgr.Add('程序网站: http://www.bmm2.com');
end;

procedure TFormMain.InitIOCPServer;
var
  I: Integer;
  ClientThread: TClientThread;
begin
  with m_xRunServerList.IOCPServer do
  begin
    UserManager.OnUserEnter := Self.UserEnterEvent;
    UserManager.OnUserLeave := Self.UserLeaveEvent;
    Reader.OnReadEvent := Self.UserReadBuffer;
  end;

  if g_pConfig.m_nGateCount > 0 then
  begin
    for I := 1 to g_pConfig.m_nGateCount do
    begin
      ClientThread := m_xGameServerList.InitGameServer(g_pConfig.m_xGameGateList[I].nServerPort, g_pConfig.m_xGameGateList[I].sServerAdress);
      ClientThread.m_nPos := I;
      ClientThread.OnReadEvent := CltOnRead;
      ClientThread.OnCloseEvent := CltOnClose;
      m_xRunServerList.InitServer(g_pConfig.m_xGameGateList[I].nGatePort, ClientThread);
      ClientThread.Active := True;
    end;

    m_xRunServerList.IOCPServer.StartService;
  end
  else
  begin
    MessageBox(0, '监听端口数量不能少于1个', '错误', MB_OK);
  end;
end;

procedure TFormMain.CltOnRead(ClientThread: TClientThread; const Buffer: PChar; const BufLen: UINT);
var
  I, nPos, nSock, nBufLen, ExecLen: Integer;
  pTRBuf: PChar;
  pTREnd: PChar;
  pTRBuffer: PChar;
  UserOBJ: TSessionObj;
  CmdPack: TCmdPack;
  IsSendToClient: Boolean;
  AddressInfo: pTAddressInfo;
  SendMsg, S: string;
label
  LOOP;
begin
  //ExecLen := 0;
  pTRBuf := Buffer;
  pTREnd := Buffer + BufLen;

  LOOP:
  while DWORD(pTRBuf) < DWORD(pTREnd) do
  begin
    if pTRBuf^ <> '%' then
    begin
      Inc(pTRBuf);
      Continue;
    end;
    if DWORD(pTREnd) - DWORD(pTRBuf) <= 2 then
      Break;
    pTRBuffer := pTRBuf + 1;
    while DWORD(pTRBuffer) < DWORD(pTREnd) do
    begin
      if pTRBuffer^ <> '$' then
      begin
        Inc(pTRBuffer);
        Continue;
      end;
      Inc(pTRBuf, 1);
      nBufLen := UINT(pTRBuffer) - UINT(pTRBuf);

      if nBufLen >= 2 then
      begin
        with g_ProcMsgThread do
        begin
          if pTRBuf^ = '+' then
          begin
            if (pTRBuf + 1)^ = '-' then
            begin
              nSock := Misc.AnsiStrToVal(pTRBuf + 2, nPos);
              UserOBJ := GetSession(nSock);
              if UserOBJ <> nil then
              begin
                if g_pLogMgr.CheckLevel(5) then
                  g_pLogMgr.Add('断开客户端连接: ' + UserOBJ.m_pUserOBJ.pszIPAddr);
                  
                SHSocket.FreeSocket(UserOBJ.m_pUserOBJ._SendObj.Socket);
              end;
            end
            else
            begin
              ClientThread.m_dwKeepAliveTick := GetTickCount();
              //ClientThread.m_fKeepAliveTimcOut := False;
            end;
          end
          else
          begin
            IsSendToClient := True;
            nSock := Misc.AnsiStrToVal(pTRBuf, nPos);
            UserOBJ := GetSession(nSock);
            if (UserOBJ <> nil) and (UserOBJ.m_tLastGameSvr = ClientThread) then
            begin
              if nBufLen - nPos - 1 > DEF_BLOCK_SIZE then   // 2021-01-05
              begin
                SetLength(SendMsg, nBufLen - nPos - 1);
                Move(PChar(pTRBuf + nPos + 1)^, SendMsg[1], nBufLen - nPos - 1);
                ArrestStringEx(SendMsg, '#', '!', S);

                CmdPack := DecodeMessage(S);

                if (CmdPack.Ident = SM_SETL2PASSWORD) then
                  UserOBJ.m_IsCanSetL2Password := True
                else if (CmdPack.Ident = SM_CHECKL2PASSWORD) then
                  UserOBJ.m_IsCanCheckL2Password := True
                else if CmdPack.Ident = SM_SELECTSERVER_OK then
                  UserOBJ.DelayClose(8000)

                else if (CmdPack.Ident = SM_PASSWD_FAIL) then
                begin
                  // 将 ID不存在或未知错误 / 密码错误 统一成 帐户或密码错误！ chongchong 2016-10-31
                  if (CmdPack.Recog = 0 {ID不存在或未知错误}) then
                  begin
                    {
                    g_CurrIPaddrList.Lock;
                    try
                      AddressInfo := g_CurrIPaddrList.Find(UserOBJ.m_pUserOBJ.pszIPAddr);
                      if AddressInfo <> nil then
                      begin
                        Inc(AddressInfo.nIPAccountErrCount);

                        if GetTickCount - AddressInfo.dwIPAccountErrTick < dwIPAccountErrTime * 1000 then
                        begin
                          Inc(AddressInfo.nIPAccountErrCount);
                          if AddressInfo.nIPAccountErrCount >= dwIPAccountErrLimit then
                          begin
                            MainOutMessage('验证攻击: ' + UserSession.sRemoteIPaddr, 1);

                            case BlockMethod of
                              mDisconnect:
                                begin
                                  UserSession.Socket.Close;
                                end;
                              mBlock:
                                begin
                                  AddTempBlockIP(UserSession.sRemoteIPaddr);
                                  CloseConnect(UserSession.sRemoteIPaddr);
                                end;
                              mBlockList:
                                begin
                                  AddBlockIP(UserSession.sRemoteIPaddr);
                                  CloseConnect(UserSession.sRemoteIPaddr);
                                end;
                            end;

                            Exit;
                          end;
                        end
                        else
                        begin
                          AddressInfo.dwIPAccountErrTick := GetTickCount();
                          AddressInfo.nIPAccountErrCount := 0;
                        end;
                      end
                      else
                      begin
                        AddressInfo := g_CurrIPaddrList.Add(UserSession.sRemoteIPaddr);
                        AddressInfo.nIPAccountErrCount := 1;
                        AddressInfo.dwIPAccountErrTick := GetTickCount;
                      end;
                    finally
                      g_CurrIPaddrList.UnLock;
                    end;
                    }

                    CmdPack.Recog := -6;
                    SendMsg := '#' + EncodeMessage(MakeDefaultMsg(SM_PASSWD_FAIL, CmdPack.Recog, CmdPack.Param, CmdPack.Tag, CmdPack.Series)) + '!';
                  end
                  else if (CmdPack.Recog = -1 {密码错误}) then
                  begin
                    {
                    g_CurrIPaddrList.Lock;
                    try
                      AddressInfo := g_CurrIPaddrList.Find(UserSession.sRemoteIPaddr);
                      if AddressInfo <> nil then
                      begin
                        Inc(AddressInfo.nIPPasswordErrCount);

                        if GetTickCount - AddressInfo.dwIPPasswordErrTick < dwIPPasswordErrTime * 1000 then
                        begin
                          Inc(AddressInfo.nIPPasswordErrCount);
                          if AddressInfo.nIPPasswordErrCount >= dwIPPasswordErrLimit then
                          begin
                            MainOutMessage('验证攻击: ' + UserSession.sRemoteIPaddr, 1);
                            case BlockMethod of
                              mDisconnect:
                                begin
                                  UserSession.Socket.Close;
                                end;
                              mBlock:
                                begin
                                  AddTempBlockIP(UserSession.sRemoteIPaddr);
                                  CloseConnect(UserSession.sRemoteIPaddr);
                                end;
                              mBlockList:
                                begin
                                  AddBlockIP(UserSession.sRemoteIPaddr);
                                  CloseConnect(UserSession.sRemoteIPaddr);
                                end;
                            end;
                            Exit;
                          end;
                        end
                        else
                        begin
                          AddressInfo.dwIPPasswordErrTick := GetTickCount();
                          AddressInfo.nIPPasswordErrCount := 0;
                        end;
                      end
                      else
                      begin
                        AddressInfo := g_CurrIPaddrList.Add(UserSession.sRemoteIPaddr);
                        AddressInfo.nIPPasswordErrCount := 1;
                        AddressInfo.dwIPPasswordErrTick := GetTickCount;
                      end;
                    finally
                      g_CurrIPaddrList.UnLock;
                    end;
                    }

                    CmdPack.Recog := -6;
                    SendMsg := '#' + EncodeMessage(MakeDefaultMsg(SM_PASSWD_FAIL, CmdPack.Recog, CmdPack.Param, CmdPack.Tag, CmdPack.Series)) + '!';
                  end;
                end

                // 有人通过修改密码的方法来扫号 2019-09-21 00:59:07
                else if CmdPack.Ident = SM_CHGPASSWD_FAIL then
                begin
                  (*
                  if (CmdPack.Recog = 0 {ID不存在}) then
                  begin
                    g_CurrIPaddrList.Lock;
                    try
                      AddressInfo := g_CurrIPaddrList.Find(UserSession.sRemoteIPaddr);
                      if AddressInfo <> nil then
                      begin
                        Inc(AddressInfo.nIPAccountErrCount);

                        if GetTickCount - AddressInfo.dwIPAccountErrTick < dwIPAccountErrTime * 1000 then
                        begin
                          Inc(AddressInfo.nIPAccountErrCount);
                          if AddressInfo.nIPAccountErrCount >= dwIPAccountErrLimit then
                          begin
                            MainOutMessage('验证攻击: ' + UserSession.sRemoteIPaddr, 1);

                            case BlockMethod of
                              mDisconnect:
                                begin
                                  UserSession.Socket.Close;
                                end;
                              mBlock:
                                begin
                                  AddTempBlockIP(UserSession.sRemoteIPaddr);
                                  CloseConnect(UserSession.sRemoteIPaddr);
                                end;
                              mBlockList:
                                begin
                                  AddBlockIP(UserSession.sRemoteIPaddr);
                                  CloseConnect(UserSession.sRemoteIPaddr);
                                end;
                            end;

                            Exit;
                          end;
                        end
                        else
                        begin
                          AddressInfo.dwIPAccountErrTick := GetTickCount();
                          AddressInfo.nIPAccountErrCount := 0;
                        end;
                      end
                      else
                      begin
                        AddressInfo := g_CurrIPaddrList.Add(UserSession.sRemoteIPaddr);
                        AddressInfo.nIPAccountErrCount := 1;
                        AddressInfo.dwIPAccountErrTick := GetTickCount;
                      end;
                    finally
                      g_CurrIPaddrList.UnLock;
                    end;

                  end;
                  *)
                end
                else if CmdPack.Ident = SM_GETBACKPASSWD_FAIL then
                begin
                  (*
                  g_CurrIPaddrList.Lock;
                  try
                    AddressInfo := g_CurrIPaddrList.Find(UserSession.sRemoteIPaddr);
                    if AddressInfo <> nil then
                    begin
                      Inc(AddressInfo.nIPPasswordProtectedErrCount);

                      if GetTickCount - AddressInfo.dwIPPasswordProtectedErrTick < dwIPPasswordProtectedErrTime * 1000 then
                      begin
                        Inc(AddressInfo.nIPPasswordProtectedErrCount);
                        if AddressInfo.nIPPasswordProtectedErrCount >= dwIPPasswordProtectedErrLimit then
                        begin
                          MainOutMessage('验证攻击: ' + UserSession.sRemoteIPaddr, 1);

                          case BlockMethod of
                            mDisconnect:
                              begin
                                UserSession.Socket.Close;
                              end;
                            mBlock:
                              begin
                                AddTempBlockIP(UserSession.sRemoteIPaddr);
                                CloseConnect(UserSession.sRemoteIPaddr);
                              end;
                            mBlockList:
                              begin
                                AddBlockIP(UserSession.sRemoteIPaddr);
                                CloseConnect(UserSession.sRemoteIPaddr);
                              end;
                          end;

                          Exit;
                        end;
                      end
                      else
                      begin
                        AddressInfo.dwIPPasswordProtectedErrTick := GetTickCount();
                        AddressInfo.nIPPasswordProtectedErrCount := 0;
                      end;
                    end
                    else
                    begin
                      AddressInfo := g_CurrIPaddrList.Add(UserSession.sRemoteIPaddr);
                      AddressInfo.nIPPasswordProtectedErrCount := 1;
                      AddressInfo.dwIPPasswordProtectedErrTick := GetTickCount;
                    end;
                  finally
                    g_CurrIPaddrList.UnLock;
                  end;
                  *)
                end;
              end;

              if IsSendToClient then
              begin
                UserOBJ.ProcessSvrData(ClientThread, Integer(@SendMsg[1]), Length(SendMsg));
              end;
            end;
          end;
        end;
      end;
      Inc(pTRBuf, nBufLen + 1);

      //ExecLen := DWORD(pTRBuf) - DWORD(Buffer);

      goto LOOP;
    end;
    Break;
  end;
  //if ExecLen > 0 then
  //  ClientThread.ReaderDone(ExecLen)
  //else
  ClientThread.ReaderDone(DWORD(pTRBuf) - DWORD(Buffer));
end;

procedure TFormMain.CltOnClose(Sender: TObject);
begin
  if g_pLogMgr.CheckLevel(5) then
    g_pLogMgr.Add('服务器连接已关闭: ' + TClientThread(Sender).ServerIP + ':' + IntToStr(TClientThread(Sender).ServerPort));
  with m_xRunServerList.IOCPServer do
    Writer.BroadUserMsg(0, Integer(Sender), ClientClosed);
end;

procedure TFormMain.ClientClosed(UserOBJ: pTUserOBJ; wParam, lParam: DWORD);
var
  ClientObj: TSessionObj;
begin
  ClientObj := TSessionObj(UserOBJ.tData);
  if ClientObj = nil then
    Exit;
  with ClientObj do
  begin
    if m_tLastGameSvr = TClientThread(lParam) then
    begin
      m_tIOCPSender.DeleteSocket(m_pOverlapSend);
    end;
  end;
end;

procedure TFormMain.UserEnterEvent(Sender: TUserManager; const UserOBJ: pTUserOBJ);
var
  CSession: TSessionObj;
begin
  CSession := TSessionObj(UserOBJ.tData);
  if CSession <> nil then
  begin
    CSession.ReCreate();
  end
  else
  begin
    CSession := TSessionObj.Create;
    UserOBJ.tData := CSession;
  end;
  with CSession do
  begin
    m_pUserOBJ := UserOBJ;
    m_tIOCPSender := UserOBJ.Writer;
    m_pOverlapSend := UserOBJ._SendObj;
    m_dwSessionID := UserOBJ.dwUID;
    InterlockedExchange(Integer(m_tLastGameSvr), Integer(UserOBJ.GameServ));
    InterlockedExchange(Integer(g_UserList[m_dwSessionID]), Integer(CSession));
    UserEnter();
  end;
end;

procedure TFormMain.UserLeaveEvent(Sender: TUserManager; const UserOBJ: pTUserOBJ);
begin
  if UserOBJ.tData <> nil then
  begin
    with TSessionObj(UserOBJ.tData) do
    begin
      UserLeave();
      InterlockedExchange(Integer(m_tLastGameSvr), 0);
    end;
  end;
end;

procedure TFormMain.UserReadBuffer(UserOBJ: TObject; const Buffer: PChar; var BufLen: DWORD; var Succeed: BOOL);
label
  LOOP;
var
  fCDPacket: Boolean;
  dwEndLoop: DWORD;
  iLen, ExecLen, nMsgCount: Integer;
  pTRBuf, pTRBuffer: PByte;
  dwEnd: DWORD;
begin
  fCDPacket := False;
  //ExecLen := 0;
  nMsgCount := 0;
  pTRBuf := PByte(Buffer);
  dwEnd := DWORD(Buffer) + BufLen;

  LOOP:
  while DWORD(pTRBuf) < dwEnd do
  begin
    if (pTRBuf^ <> $23) then
    begin //# &
      Inc(pTRBuf);
      Continue;
    end;
    if (dwEnd - DWORD(pTRBuf)) <= 2 then
      Break;
    pTRBuffer := Pointer(Integer(pTRBuf) + 2);
    while DWORD(pTRBuffer) < dwEnd do
    begin
      if pTRBuffer^ <> $21 then
      begin
        Inc(pTRBuffer);
        Continue;
      end;
      Inc(nMsgCount);
      Inc(pTRBuf, 2);
      iLen := UINT(pTRBuffer) - UINT(pTRBuf);
      TSessionObj(UserOBJ).ProcessCltData(Integer(pTRBuf), iLen, Succeed, fCDPacket);

      if not Succeed then
        Break;

      Inc(pTRBuf, iLen + 1);

      //ExecLen := DWORD(pTRBuf) - DWORD(Buffer);

      if nMsgCount >= g_pConfig.m_nMaxClientPacketCount then
      begin
        KickUser(TSessionObj(UserOBJ).m_pUserOBJ.nIPAddr);
        Break;
      end;
      goto LOOP;
    end;
    Break;
  end;
  if Succeed then
    BufLen := DWORD(pTRBuf) - DWORD(Buffer); //ExecLen;
end;

procedure TFormMain.MyMessage(var MsgData: TWmCopyData);
var
  sData: string;
  wIdent: Word;
begin
  wIdent := HiWord(MsgData.From);
  sData := StrPas(MsgData.CopyDataStruct^.lpData);
  case wIdent of
    GS_QUIT:
      begin
        if g_fServiceStarted then
        begin
          StopService();
          g_fCanClose := True;
          Close();
        end;
      end;
  end;
end;

procedure TFormMain.WMSetParentWindow(var Message: TMessage);
begin
  if Message.WParam = 1 then begin
    Windows.SetParent(Handle, Message.LParam);
    SetWindowLong(Application.Handle, GWL_EXSTYLE, GetWindowLong(Application.Handle, GWL_EXSTYLE) or WS_EX_TOOLWINDOW);
    FIsEmbeddedGameCenter := True;
    Self.Hide;
    Self.Show;
  end else begin
    Windows.SetParent(Handle, 0);
    SetWindowLong(Application.Handle, GWL_EXSTYLE, GetWindowLong(Application.Handle, GWL_EXSTYLE) and (not WS_EX_TOOLWINDOW));
    FIsEmbeddedGameCenter := False;
  end;
end;

procedure TFormMain.WMSysCommand(var Message: TWMSysCommand);
const
  GWW_HWNDPARENT = -8;
begin
  if FIsEmbeddedGameCenter and (Message.CmdType and $FFF0 = SC_MINIMIZE) then
  begin
    DefaultHandler(Message);
  end
  else
    inherited;
end;

procedure TFormMain.OnAppModalBegin(Sender: TObject);
begin
  if FIsEmbeddedGameCenter then
  begin
    Enabled := False;
  end;
end;

procedure TFormMain.OnAppOnModalEnd(Sender: TObject);
begin
  if FIsEmbeddedGameCenter then
  begin
    Enabled := True;
  end;
end;

{$IF VER_TYPE = 1}

procedure TFormMain.OnClientSocetkVerifyConnect(Sender: TObject;
  Socket: TCustomWinSocket);
var
  I, ExtendedInfo: Integer;
  RungateVerifyHeader: TRungateVerifyHeader;
  VerifyData: TRungateVerifyData;
  HWID: array[0..100] of Char;
  UserName: array[0..100] of Char;
  Organization: array[0..100] of Char;
  MachineID: array[0..7] of DWORD;
  S: string;
begin
{.$I Registered_Start.inc}
  FVerifyDataText := '';

  if (WLRegGetStatus(ExtendedInfo) = wlIsRegistered) then
  begin
    FillChar(VerifyData, SizeOf(VerifyData), 0);
    FillChar(UserName, SizeOf(UserName), 0);
    FillChar(Organization, SizeOf(Organization), 0);

    RungateVerifyHeader.dwCode := $5F3ED327;
    RungateVerifyHeader.dwCmd := 302;
    RungateVerifyHeader.nLength := SizeOf(TRungateVerifyHeader);

    WLRegGetLicenseInfo(UserName, Organization, nil);
    WLHardwareGetID(HWID);
    for I := 0 to 7 do
    begin
      MachineID[I] := StrToIntDef('$' + Copy(HWID, I * 5 + 1, 4), 0);
    end;

    Randomize;
    FVerifyDataKey := Random(High(Integer));

    VerifyData.Key := FVerifyDataKey;
    Move(Organization[0], VerifyData.IP[0], SizeOf(VerifyData.IP));
    Move(MachineID[0], VerifyData.HWID[0], SizeOf(VerifyData.HWID));

    SetLength(S, SizeOf(TRungateVerifyHeader) + SizeOf(TRungateVerifyData));
    Move(RungateVerifyHeader, S[1], SizeOf(TRungateVerifyHeader));
    Move(VerifyData, S[1 + SizeOf(TRungateVerifyHeader)], SizeOf(TRungateVerifyData));

    Socket.SendBuf(S[1], Length(S));
  end;
{.$I Registered_End.inc}
end;

procedure TFormMain.OnClientSocetkVerifyError(Sender: TObject;
  Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
end;

procedure TFormMain.OnClientSocetkVerifyRead(Sender: TObject;
  Socket: TCustomWinSocket);
type
  TLoginGateResponseData = record
    TimeLeft: Int64;
    Key: LongWord;
  end;
var
  I, ExtendedInfo: Integer;
  HWID: array[0..100] of Char;
  MachineID: array[0..7] of DWORD;

  RungateVerifyHeader: PRungateVerifyHeader;
  LoginGateResponseData: TLoginGateResponseData;
begin
{.$I Registered_Start.inc}
  if (WLRegGetStatus(ExtendedInfo) = wlIsRegistered) then
  begin
    WLHardwareGetID(HWID);
    for I := 0 to 7 do
    begin
      MachineID[I] := StrToIntDef('$' + Copy(HWID, I * 5 + 1, 4), 0);
    end;

    FVerifyDataText := FVerifyDataText + Socket.ReceiveText;
    if Length(FVerifyDataText) >= SizeOf(TRungateVerifyHeader) then
    begin
      RungateVerifyHeader := PRungateVerifyHeader(@FVerifyDataText[1]);

      if RungateVerifyHeader.dwCode = $5F3ED327 then
      begin
        if SizeOf(TRungateVerifyHeader) + RungateVerifyHeader.nLength = Length(FVerifyDataText) then
        begin
          if (RungateVerifyHeader.dwCmd = 123) and (RungateVerifyHeader.nLength = SizeOf(TLoginGateResponseData)) then
          begin
            Move(FVerifyDataText[1 + SizeOf(TRungateVerifyHeader)], LoginGateResponseData, Length(FVerifyDataText) - SizeOf(TRungateVerifyHeader));

            FTimeLeftStartTick := GetTickCount;
            FTimeLeft := LoginGateResponseData.TimeLeft;
            FIsShowTimeLeft := True;

            g_nProtocolKey3 := LoginGateResponseData.Key xor MachineID[0] xor MachineID[3] xor FVerifyDataKey;

{$IF VER_TYPE1_TEST = 1}
            g_pLogMgr.Add('Key3: ' + IntToStr(g_nProtocolKey3));
{$IFEND}
          end;


          (*
          if (RungateVerifyHeader.dwCmd = 1111) then
          begin
            {$IF VER_TYPE1_TEST = 1}
              //AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~复位数据RUNGATECODE' + IntToStr(RUNGATECODE), 1);
            {$IFEND}

            //Move(g_boVerifyFailTriggerScript, RUNGATECODE, 100);
            //GM_DATA := 10 + Random(10000);
          end;
          *)

          FVerifyDataText := '';
          FIsVerifyServerResult := True;
          FIsRetryVerifyServerResult := True;
          FRetryConnectVerifyServerCount := 0;
        end;
      end;
    end;
  end;
{.$I Registered_End.inc}
end;
{$IFEND}

/// <summary>
///   计算两个TickCount时间差，避免超出49天后，溢出
///      感谢 [佛山]沧海一笑  7041779 提供
///      copy自 qsl代码
/// </summary>

function tick_diff(tick_start, tick_end: Cardinal): Cardinal;
begin
  if tick_end >= tick_start then
    result := tick_end - tick_start
  else
    result := High(Cardinal) - tick_start + tick_end;
end;

procedure TFormMain.tmrVerifyTimer(Sender: TObject);
{$IF VER_TYPE = 1}
var
  ExtendedInfo: Integer;
{$IFEND}
begin
{$IF VER_TYPE = 1}
{.$I Registered_Start.inc}
  // 连接检测服务器，看是否是合法的注册 chongchong 2016-07-12
  if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
  begin
    if ((not FIsVerifyServerResult) and (tick_diff(FConnectVerifyServerTick, GetTickCount) >= 15000)) then
    begin
      FConnectVerifyServerTick := GetTickCount;

      if Assigned(FClientSocetkVerify) then
        FreeAndNil(FClientSocetkVerify);

      FClientSocetkVerify := TClientSocket.Create(nil);
      FClientSocetkVerify.OnConnect := OnClientSocetkVerifyConnect;
      FClientSocetkVerify.OnError := OnClientSocetkVerifyError;
      FClientSocetkVerify.OnRead := OnClientSocetkVerifyRead;

      FClientSocetkVerify.ClientType := ctNonBlocking;


      Randomize;
{$IF VER_TYPE1_TEST = 0}
      FClientSocetkVerify.Port := 16161 + Random(8); // 16161-16168 随机端口
{$ELSE}
      FClientSocetkVerify.Port := 16161; // + Random(8);         // 16161-16168 随机端口
{$IFEND}

      if FConnectVerifyServerIndex > 1 then
      begin
        FConnectVerifyServerIndex := 0;
        g_pLogMgr.Add('连接验证服务器失败，尝试重新连接');
      end;

{$IF VER_TYPE1_TEST = 0}
      case FConnectVerifyServerIndex of
        0: FClientSocetkVerify.Host := 'yz.fengwg.net'; //'183.2.195.111';
        1: FClientSocetkVerify.Host := 'yz1.fengwg.net';
      end;
{$ELSE}
      case FConnectVerifyServerIndex of
        0: FClientSocetkVerify.Address := '10.0.0.5'; //'183.2.195.111';
        1: FClientSocetkVerify.Address := '10.0.0.5'; //'61.175.226.179';;
      end;
{$IFEND}

      Inc(FConnectVerifyServerIndex);

      FClientSocetkVerify.Active := True;
    end;

    // 显示倒计时
    if FIsShowTimeLeft then
    begin
      if tick_diff(FTimeLeftStartTick, GetTickCount) >= 3600000 then
      begin
        Dec(FTimeLeft);
        FTimeLeftStartTick := GetTickCount;

        if FTimeLeft <= 0 then
        begin
{$IF VER_TYPE1_TEST = 1}
          g_pLogMgr.Add('~~~~~~~~~~~~~~~~~~~~~~~~~~~打乱RUNGATECODE');
{$IFEND}
        end;
      end;

      if FTimeLeft >= 0 then
        Caption := g_pConfig.m_szTitle + '  [剩余时间: ' + IntToStr(FTimeLeft) + ' 小时]'
    end;
  end;
{.$I Registered_end.inc}
{$IFEND}
end;

end.


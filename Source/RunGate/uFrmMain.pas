unit uFrmMain;

interface

//当注册失败时或非法破解时，不要断开连接，就这样搞。随机搞些链接加入到 FNoUseContextList
{
      TIocpClientContextPool.Instance.FOnlineContextList.Remove(Self);
      TIocpClientContextPool.Instance.FNoUseContextList.Add(Self);

}

{$I iocp.inc}

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ExtCtrls, StdCtrls, ComCtrls, Menus, IniFiles, GateShare,
  Spin, RunGateUtils, MirClientContext, DateUtils,
  IocpUtils, IocpTcpServer, IocpWinsock2, IocpCommon, IODataPool,
  uFrmMessageFilter, uFrmSafeFilter, uFrmGameSpeed, psApi, Math,
  Clipbrd, uFrmReadFileIP, uIPDownThread, JSocket, Grobal2_Ex,
  {$IF NEED_REGISTER = 1} //WinlicenseSDK,
  DesUtils,{$IFEND} DesNew2,
  CheckUnit, ShellAPI,
  uFrmProcessBlacklist, uFrmMagicCD, uFrmItemEatCD, MagicIntervalUtils, MD5Util,
  uFrmLogClientPacketSetting {$IF CLIENT_ANTIPLUG = 1}, uFrmAntiPlugUpdateSetting{$IFEND},
  AsyncCalls;

{$R uac.res}

resourcestring
  STR_LISTEN_FAIL = '未监听';
  STR_LINK_FAIL   = '未连接';
  STR_LISTEN_OK   = '监听成功';
  STR_LINK_OK     = '连接成功';

const
  WM_SET_PARENT_WINDOW = WM_USER + 123;

type
  TFrmMain = class(TForm)
    mmMain: TMainMenu;
    mniControl: TMenuItem;
    pgcMain: TPageControl;
    tsRunInfo: TTabSheet;
    tsOnline: TTabSheet;
    tsSysInfo: TTabSheet;
    lblInfoBase: TLabel;
    bvl1: TBevel;
    Label1: TLabel;
    Label6: TLabel;
    Label11: TLabel;
    Bevel1: TBevel;
    Label3: TLabel;
    Label4: TLabel;
    Label7: TLabel;
    Label8: TLabel;
    Label2: TLabel;
    Bevel3: TBevel;
    Label5: TLabel;
    Label9: TLabel;
    lblWorkThreadCount: TLabel;
    lblServerRunTime: TLabel;
    lblSendCount: TLabel;
    lblSendBytesSize: TLabel;
    lblRecvCount: TLabel;
    lblRecvBytesSize: TLabel;
    lblIODataUseCount: TLabel;
    lblIODataNoUseCount: TLabel;
    Label12: TLabel;
    Bevel2: TBevel;
    Label13: TLabel;
    Label14: TLabel;
    lblContextUseCount: TLabel;
    lblContextNoUseCount: TLabel;
    Label10: TLabel;
    lblContextMaxCount: TLabel;
    Label15: TLabel;
    lblUpdateTime: TLabel;
    Label17: TLabel;
    Label18: TLabel;
    mmoMainLog: TMemo;
    mmiStartServer: TMenuItem;
    mmiStopServer: TMenuItem;
    tsSetting: TTabSheet;
    tmrStart: TTimer;
    grpNetConfig: TGroupBox;
    lblGateIPaddr: TLabel;
    lblGatePort: TLabel;
    lblServerPort: TLabel;
    lblServerIPaddr: TLabel;
    edtGateIPaddr: TEdit;
    edtGatePort: TEdit;
    edtServerPort: TEdit;
    edtServerIPaddr: TEdit;
    chkClientPassword: TCheckBox;
    edtClientPassword: TEdit;
    grpBaseInfo: TGroupBox;
    lblTitleName: TLabel;
    lblShowLogLevel: TLabel;
    edtTitleName: TEdit;
    chkMinimize: TCheckBox;
    cbbShowLogLevel: TComboBox;
    grpPerformance: TGroupBox;
    lblServerCheckTimeOut: TLabel;
    Label21: TLabel;
    lblClientSendBlockSize: TLabel;
    Label22: TLabel;
    seClientSendBlockSize: TSpinEdit;
    btnSettingOK: TButton;
    tmrRefreshInfo: TTimer;
    lvOnLine: TListView;
    Label19: TLabel;
    lblIODataMemCount: TLabel;
    mniSetting: TMenuItem;
    mmiWordFilter: TMenuItem;
    mmiSafeSetting: TMenuItem;
    mmiWaiGua: TMenuItem;
    pnlInfoBottom: TPanel;
    splInfoBottom: TSplitter;
    lvRunGates: TListView;
    tsIocp: TTabSheet;
    mmoIocpLog: TMemo;
    tmrRefreshLog: TTimer;
    Label20: TLabel;
    lblAppMemorySize: TLabel;
    pmUser: TPopupMenu;
    mniKick: TMenuItem;
    mniAddToTempBlock: TMenuItem;
    mniAddToBlock: TMenuItem;
    mniAddToTempMacBlock: TMenuItem;
    mniN3: TMenuItem;
    N3: TMenuItem;
    mniAddToMacBlock: TMenuItem;
    N4: TMenuItem;
    mniCopyMac: TMenuItem;
    mniCopyIP: TMenuItem;
    mniHelp: TMenuItem;
    mniAbout: TMenuItem;
    mniReadFileIP: TMenuItem;
    Label16: TLabel;
    lblIODataMaxUseCount: TLabel;
    mniN00: TMenuItem;
    mniDBServerEnabledIP: TMenuItem;
    edtDBPort: TEdit;
    Label23: TLabel;
    lblClientPassword: TLabel;
    ServerSocketDB: TServerSocket;
    splOnlineUser: TSplitter;
    pnlOnlineUserBottom: TGroupBox;
    Label24: TLabel;
    Label25: TLabel;
    lblContextCharName: TLabel;
    Label27: TLabel;
    lblContextStatus: TLabel;
    lvContextProcessListInfo: TListView;
    edtSearch: TEdit;
    btnRefreshContextProcessList: TButton;
    btnScreenshotGame: TButton;
    btnScreenshot: TButton;
    btnSearch: TButton;
    btnNextSearch: TButton;
    Label26: TLabel;
    lblIODataMaxMemCount: TLabel;
    Label28: TLabel;
    Label29: TLabel;
    seClientAccumulateMaxSize: TSpinEdit;
    seCheckServerTimeOutTime: TSpinEdit;
    pnlOnlineUser: TPanel;
    pnlOnlineUserSearch: TPanel;
    edtSearchOnlineText: TEdit;
    lbl2: TLabel;
    cbbSearchOnlineField: TComboBox;
    lbl3: TLabel;
    chkSearchFuzzyMatch: TCheckBox;
    btnSearchOnline: TButton;
    btnSearchOnlineNext: TButton;
    pmProcessList: TPopupMenu;
    mniAddBlackProcess: TMenuItem;
    mniN11: TMenuItem;
    mniProcessBlacklist: TMenuItem;
    mniN12: TMenuItem;
    mniLogClientPacket: TMenuItem;
    Label30: TLabel;
    Label31: TLabel;
    sePreAllocatedCount: TSpinEdit;
    Label32: TLabel;
    edtPreAllocatedSize: TEdit;
    lblRecommendPreAllocatedCount: TLabel;
    lblPreAllocatedSizeHint: TLabel;
    grpAntiPlug: TGroupBox;
    lbl4: TLabel;
    seRecvAntiPlugHeartbeatTimeOutTime: TSpinEdit;
    lbl5: TLabel;
    mniMagicCD: TMenuItem;
    mniN10: TMenuItem;
    mniEatItemCD: TMenuItem;
    grpVerifyCode: TGroupBox;
    lbl6: TLabel;
    Label33: TLabel;
    Label34: TLabel;
    chkVerifyCode: TCheckBox;
    seVerifyCodeErrCount: TSpinEdit;
    seVerifyCodeWaitTime: TSpinEdit;
    seVerifyCodeRefreshCount: TSpinEdit;
    lstVerifyCodeExcludeMap: TListBox;
    Label35: TLabel;
    Label36: TLabel;
    Label37: TLabel;
    btnAdd: TButton;
    btnDel: TButton;
    btnClear: TButton;
    Label38: TLabel;
    seVerifyCodeInterval1: TSpinEdit;
    Label39: TLabel;
    chkAutoLoadNoVerifyChrList: TCheckBox;
    edtLoadNoVerifyChrListFile: TEdit;
    Label40: TLabel;
    seAutoLoadNoVerifyChrListInterval: TSpinEdit;
    lbl8: TLabel;
    btnLoadNoVerifyChrList: TButton;
    seVerifyCodeInterval2: TSpinEdit;
    Label41: TLabel;
    Label42: TLabel;
    seVerifySuccessAddInterval: TSpinEdit;
    chkVerifyCodeExcludeMap: TCheckBox;
    chkVerifyFailTriggerScript: TCheckBox;
    chkVerifyFailLoginVerify: TCheckBox;
    Label43: TLabel;
    Label44: TLabel;
    seAntiPlugStreamSendSpeed: TSpinEdit;
    mniSendFileToRungate: TMenuItem;
    Label45: TLabel;
    Label46: TLabel;
    cbbAntiPlugStreamSendBlockSize: TComboBox;
    chkLogoutNoResendAntiplugStream: TCheckBox;
    mniCopyUserName: TMenuItem;
    chkOneMACLimitePlayer: TCheckBox;
    seOneMACLimitePlayer: TSpinEdit;
    Label47: TLabel;
    chkAntiplugAllLog: TCheckBox;
    lblRooDir: TLabel;
    Label51: TLabel;
    mniSetRoot: TMenuItem;
    lblSaveDir: TLabel;
    Label52: TLabel;
    grpClientExitDaly: TGroupBox;
    Label50: TLabel;
    Label53: TLabel;
    Label54: TLabel;
    Label55: TLabel;
    seClientLogoutDelay: TSpinEdit;
    seClientCloseDelay: TSpinEdit;
    chkDelayCloseDisableMove: TCheckBox;
    chkDelayCloseDisableAttack: TCheckBox;
    chkDelayCloseDisableSpell: TCheckBox;
    chkDelayCloseDisableUseItem: TCheckBox;
    chkBreakClientLogoutHint: TCheckBox;
    edtBreakClientLogoutHint: TEdit;
    chkBreakClientCloseHint: TCheckBox;
    edtBreakClientCloseHint: TEdit;
    stat: TStatusBar;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure tmrStartTimer(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure mmiStartServerClick(Sender: TObject);
    procedure mmiStopServerClick(Sender: TObject);
    procedure tmrRefreshInfoTimer(Sender: TObject);
    procedure btnSettingOKClick(Sender: TObject);
    procedure edtGateIPaddrChange(Sender: TObject);
    procedure mmiWordFilterClick(Sender: TObject);
    procedure mmiSafeSettingClick(Sender: TObject);
    procedure tmrRefreshLogTimer(Sender: TObject);
    procedure mmiWaiGuaClick(Sender: TObject);
    procedure mmoMainLogDblClick(Sender: TObject);
    procedure mmoIocpLogDblClick(Sender: TObject);
    procedure pmUserPopup(Sender: TObject);
    procedure mniKickClick(Sender: TObject);
    procedure mniAddToTempBlockClick(Sender: TObject);
    procedure mniAddToBlockClick(Sender: TObject);
    procedure mniAddToTempMacBlockClick(Sender: TObject);
    procedure mniAddToMacBlockClick(Sender: TObject);
    procedure mniCopyMacClick(Sender: TObject);
    procedure mniCopyIPClick(Sender: TObject);
    procedure mniAboutClick(Sender: TObject);
    procedure mniReadFileIPClick(Sender: TObject);
    procedure mniDBServerEnabledIPClick(Sender: TObject);
    procedure ServerSocketDBClientConnect(Sender: TObject;
      Socket: TCustomWinSocket);
    procedure ServerSocketDBClientError(Sender: TObject;
      Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
      var ErrorCode: Integer);
    procedure ServerSocketDBClientRead(Sender: TObject;
      Socket: TCustomWinSocket);
    procedure btnSearchClick(Sender: TObject);
    procedure btnNextSearchClick(Sender: TObject);
    procedure btnRefreshContextProcessListClick(Sender: TObject);
    procedure lvOnLineClick(Sender: TObject);
    procedure btnScreenshotGameClick(Sender: TObject);
    procedure btnScreenshotClick(Sender: TObject);
    procedure lblContextStatusMouseEnter(Sender: TObject);
    procedure lblContextStatusMouseLeave(Sender: TObject);
    procedure lblContextStatusClick(Sender: TObject);
    procedure btnSearchOnlineClick(Sender: TObject);
    procedure btnSearchOnlineNextClick(Sender: TObject);
    procedure edtSearchOnlineTextKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure pmProcessListPopup(Sender: TObject);
    procedure mniAddBlackProcessClick(Sender: TObject);
    procedure mniProcessBlacklistClick(Sender: TObject);
    procedure mniLogClientPacketClick(Sender: TObject);
    procedure seClientSendBlockSizeChange(Sender: TObject);
    procedure lblRecommendPreAllocatedCountClick(Sender: TObject);
    procedure lblRecommendPreAllocatedCountMouseEnter(Sender: TObject);
    procedure lblRecommendPreAllocatedCountMouseLeave(Sender: TObject);
    procedure mniMagicCDClick(Sender: TObject);
    procedure mniEatItemCDClick(Sender: TObject);
    procedure btnAddClick(Sender: TObject);
    procedure lstVerifyCodeExcludeMapClick(Sender: TObject);
    procedure btnDelClick(Sender: TObject);
    procedure btnClearClick(Sender: TObject);
    procedure btnLoadNoVerifyChrListClick(Sender: TObject);
    procedure lvContextProcessListInfoDblClick(Sender: TObject);
    procedure mniSendFileToRungateClick(Sender: TObject);
    procedure mniCopyUserNameClick(Sender: TObject);
    procedure lvOnLineDblClick(Sender: TObject);
    procedure mniSetRootClick(Sender: TObject);
  private
    procedure ErrMessage(ErrMsg: string);
  private
    { Private declarations }
    FIsEmbeddedGameCenter: Boolean;
  {$IF NEED_REGISTER = 1}
    dwGetBaiduTimeTick: LongWord;
    dwGetBaiduTimeTime: LongWord;
  {$IFEND}

    FRunGateManager: TRunGateManager;
    FServicesStartTime: DWORD;
    FLastRefrshOnlineUserTick: DWORD;

    FRemoteBlackIPDown: TIPDownThread;
    FRemoteWhiteIPDown: TIPDownThread;
    FRemoteDenyMACDown: TMACDownThread;

{$IF CLIENT_ANTIPLUG = 1}
    FAntiPlugDown: TAntiPlugDownThread;
{$IFEND}

    FSelectContext: TMirClientContext;
    FSelectContextCharName: string;
    FInputPasswordCharName: string;
    FSearchIndex: Integer;

    FLogFileName: string;

    FIsProcessList: Boolean;

{$IF VERSION_TYPE = 1}
    FTimerSendADText: TTimer;
    procedure OnSendADTextTimer(Sender: TObject);
{$IFEND}

{$IF VERSION_TYPE <> 0}
    procedure DoInitCustomVersion;
    procedure DoFinalCustomVersion;
    procedure DllCheckMessage(var Message: TMessage); message 2013;
    procedure OnmmiPlugGxxClick(Sender: TObject);
{$IFEND}

{$IF CLIENT_ANTIPLUG = 1}
    function ReloadAntiPlug: Boolean;
    procedure OnmmiAntiPlugUpdateSettingClick(Sender: TObject);

    procedure SendAllContextUnloadPlugin(IsWaitLoad: Boolean);

    function DoReloadRungatePlugDll: Boolean;
    function DoUnloadRungatePlugDll: Boolean;

    procedure OnAntiPlugDownloadFinished(Sender: TObject; IsUpdateRungateDll, IsUpdateClientDll: Boolean);

    procedure OnmmiReloadRunGatePlugClick(Sender: TObject);
    procedure OnmmiUnloadRunGatePlugClick(Sender: TObject);
    procedure OnmmiRunGatePlugSettingClick(Sender: TObject);
{$IFEND}

{$IF NEED_REGISTER = 0}
    procedure WriteLog(S: string);
{$IFEND}

    procedure MyMessage(var MsgData: TWmCopyData); message WM_COPYDATA;
    procedure RecallPreAllocatedSize;

    procedure WMSysCommand(var Message: TWMSysCommand); message WM_SYSCOMMAND;
    procedure WMSetParentWindow(var Message: TMessage); message WM_SET_PARENT_WINDOW;
  public
    { Public declarations }
    procedure StartServices;
    procedure StopServices;

    procedure LoadConfig(Ports: TList);

    procedure ShowAbout;

    procedure RefreshContextProcessList(Context: TMirClientContext; IsProcessList: Boolean);
    procedure RefreshContextStatusText(S: string);

    function ExtractSelfTimeDateStamp: TDateTime;
  end;

var
  FrmMain: TFrmMain;

implementation

uses
  EDcode, HUtil32, EncryptUnit_LF, LbAsym, LbRSA;

var
  g_boClose: Boolean = False;

  function ZwTerminateProcess(thread: thandle; dwCode:Dword):Boolean; stdcall; external 'ntdll.dll';
  
{$R *.dfm}

function GetSizeString(B: Int64): string;
begin
  if B >= 1099511627776 then
    Result := Format('%.2fTB', [B / 1099511627776])
  else if B >= 1073741824 then
    Result := Format('%.2fGB', [B / 1073741824])
  else if B >= 1048576 then
    Result := Format('%.2fMB', [B / 1048576])
  else if B >= 1024 then
    Result := Format('%.2fKB', [B / 1024])
  else
    Result := Format('%dB', [B]);
end;

procedure TFrmMain.FormCreate(Sender: TObject);
var
  I: Integer;
  lstPorts: TList;
  ListItem: TListItem;
  LogFile: TextFile;
  FilePath, Version: string;
  BuildTime: TDateTime;
begin
  FIsEmbeddedGameCenter := False;

  lblUpdateTime.Caption := '最后更新日期：' + g_sUpdateTime;

  g_dwGameCenterHandle := Str_ToInt(ParamStr(1), 0);

  g_CurrIPList := TAddressListEx.Create({$IFDEF USE_SPINLOCK}'CurrIPLocker'{$ENDIF});
  g_TempIPList := TAddressList.Create({$IFDEF USE_SPINLOCK}'TempIPLocker'{$ENDIF});
  g_BlockIPList := TAddressList.Create({$IFDEF USE_SPINLOCK}'BlockIPLocker'{$ENDIF});
  g_IPSectionList := TSafeList.Create({$IFDEF USE_SPINLOCK}'IPSectionLocker'{$ENDIF});
  g_AttackIPaddrList := TAddressList.Create({$IFDEF USE_SPINLOCK}'AttackIPaddrLocker'{$ENDIF});

  g_TempMacList := TSafeStringList.Create({$IFDEF USE_SPINLOCK}'TempMacLocker'{$ENDIF});
  g_BlockMacList := TSafeStringList.Create({$IFDEF USE_SPINLOCK}'BlockMacLocker'{$ENDIF});

  g_ProcessBlacklist := TProcessBlacklist.Create({$IFDEF USE_SPINLOCK}'ProcessBlackLocker'{$ENDIF});

  g_FYDenyIPList := TAddressList.Create({$IFDEF USE_SPINLOCK}'FYDenyIPLocker'{$ENDIF});
  g_FYPassIPList := TAddressList.Create({$IFDEF USE_SPINLOCK}'FYPassIPLocker'{$ENDIF});
  g_FYDenyMACList := TSafeHashStringList.Create({$IFDEF USE_SPINLOCK}'FYDenyMACLocker'{$ENDIF});

  g_FYDownDenyIPList := TAddressList.Create({$IFDEF USE_SPINLOCK}'FYDownDenyIPLocker'{$ENDIF});
  g_FYDownPassIPList := TAddressList.Create({$IFDEF USE_SPINLOCK}'FYDownPassIPLocker'{$ENDIF});
  g_FYDownDenyMACList := TSafeHashStringList.Create({$IFDEF USE_SPINLOCK}'FYDownDenyMACLocker'{$ENDIF});

  g_LogClientPacketUser := TSafeHashStringList.Create({$IFDEF USE_SPINLOCK}'LogClientPacketUser'{$ENDIF});

  g_DBAddressList := TStringList.Create;

  g_MagicCDListFileName := ExtractFilePath(ParamStr(0)) + 'MagicCD.txt';
  g_MagicCDList := TMagicIntervalList.Create;
  g_MagicCDList.LoadFromFile(g_MagicCDListFileName);

  g_VerifyCodeMapList := TSafeHashStringList.Create({$IFDEF USE_SPINLOCK}'VerifyCodeMapList'{$ENDIF});

  g_LoadNoVerifyChrList := TSafeHashStringList.Create({$IFDEF USE_SPINLOCK}'LoadNoVerifyChrList'{$ENDIF});

  SendGameCenterMsg(SG_FORMHANDLE, IntToStr(Handle));

  pgcMain.ActivePageIndex := 0;

  ShowAbout;
  
  lvRunGates.Items.Clear;
  FRunGateManager := TRunGateManager.Create(Handle);
  lstPorts := TList.Create;
  try
    LoadConfig(lstPorts);
    if lstPorts.Count > 0 then
    begin
      for I := 0 to lstPorts.Count - 1 do
      begin
        FRunGateManager.Add(Integer(lstPorts.Items[I]), I + 1);

        ListItem := lvRunGates.Items.Add;
        ListItem.Caption := IntToStr(I + 1);
        ListItem.SubItems.Add(IntToStr(Integer(lstPorts.Items[I])));
        ListItem.SubItems.Add('-');
        ListItem.SubItems.Add('-');
        ListItem.SubItems.Add('-');
        ListItem.SubItems.Add('-');
        ListItem.SubItems.Add('-');
        ListItem.SubItems.Add('-');
        ListItem.SubItems.Add(STR_LINK_FAIL);
        ListItem.SubItems.Add(STR_LISTEN_FAIL);
      end;
    end
    else
    begin
      FRunGateManager.Add(g_wdGatePort, 1);
      ListItem := lvRunGates.Items.Add;
      ListItem.Caption := IntToStr(1);
      ListItem.SubItems.Add(IntToStr(g_wdGatePort));
      ListItem.SubItems.Add('-');
      ListItem.SubItems.Add('-');
      ListItem.SubItems.Add('-');
      ListItem.SubItems.Add('-');
      ListItem.SubItems.Add('-');
      ListItem.SubItems.Add('-');
      ListItem.SubItems.Add(STR_LINK_FAIL);
      ListItem.SubItems.Add(STR_LISTEN_FAIL);
    end;
  finally
    lstPorts.Free;
  end;

{$IF VERSION_TYPE = 1}
  DoInitCustomVersion;
{$IFEND}

  FilePath := ExtractFilePath(Application.ExeName) + 'Log\';
  if not DirectoryExists(FilePath) then
    SysUtils.ForceDirectories(FilePath);

  FLogFileName := FilePath + FormatDateTime('yyyy-mm-dd hh.nn', Now) + '.txt';
  AssignFile(LogFile, FLogFileName);
  Rewrite(LogFile);
  CloseFile(LogFile);

  FSearchIndex := 0;
  FSelectContext := nil;
  FSelectContextCharName := '';
  FInputPasswordCharName := '';

{$IF NEED_REGISTER = 1}
  Randomize;
  dwGetBaiduTimeTick := MyGetTickCount;
  dwGetBaiduTimeTime := 1800000 + Random(3600000);
{$IFEND}

  FRemoteBlackIPDown := nil;
  FRemoteWhiteIPDown := nil;
  FRemoteDenyMACDown := nil;

{$IF CLIENT_ANTIPLUG = 1}
  FAntiPlugDown := nil;
{$IFEND}

  FIsProcessList := True;

  Version := GetFileVersionStr(ParamStr(0));
  BuildTime := ExtractSelfTimeDateStamp;

  stat.Panels[0].Text := 'BuildTime：' + FormatDateTime('yyyy/dd hh:nn:ss', BuildTime);
  stat.Panels[1].Text := 'ver：' + Version;
end;

procedure TFrmMain.FormDestroy(Sender: TObject);
var
  I: Integer;
{$IF CLIENT_ANTIPLUG = 1}
  OldDoUninit: TRunGatePluginDoUninit;
{$IFEND}
begin
  // 没找出来偶尔出现，不知道在哪里 WaitFor 卡住，关闭不了
  //ExitProcess(0);

{$IF NEED_REGISTER = 0}
  WriteLog('[' + TimeToStr(Now) + '] ' + '正在释放网关及线程对象');
{$IFEND}

  FRunGateManager.Free;

  g_CurrIPList.Free;
  g_CurrIPList := nil;

  g_TempIPList.Free;
  g_TempIPList := nil;

  g_BlockIPList.Free;
  g_BlockIPList := nil;

  g_TempMacList.Free;
  g_TempMacList := nil;

  g_BlockMacList.Free;
  g_BlockMacList := nil;

  g_ProcessBlacklist.Free;

{$IF NEED_REGISTER = 0}
  WriteLog('[' + TimeToStr(Now) + '] ' + '释放网关完成');
{$IFEND}

  g_IPSectionList.Lock;
  try
    for I := 0 to g_IPSectionList.Count - 1 do
      Dispose(PTIPSection(g_IPSectionList.Items[I]));
    g_IPSectionList.Clear;
  finally
    g_IPSectionList.Unlock;
  end;
  g_IPSectionList.Free;

  g_AttackIPaddrList.Free;

{$IF CLIENT_ANTIPLUG = 1}
  if g_RunGatePlugDllHandle <> 0 then
  begin
    OldDoUninit := g_rgpDoUninit;

    g_rgpDoInit := nil;
    g_rgpDoUninit := nil;
    g_rgpStartContext := nil;
    g_rgpEndContext := nil;
    //g_rgpRunContext := nil;
    g_rgpRecvPacket := nil;
    g_rgpShowConfigForm := nil;

    if Assigned(OldDoUninit) then
      OldDoUninit();
    
    FreeLibrary(g_RunGatePlugDllHandle);
    g_RunGatePlugDllHandle := 0;
  end;
  DeleteCriticalSection(g_CSRunGatePlug);

  g_ClientAntiPlugStream.Free;
{$IFEND}

  g_FYDenyIPList.Free;
  g_FYPassIPList.Free;
  g_FYDenyMACList.Free;

  g_FYDownDenyIPList.Free;
  g_FYDownPassIPList.Free;
  g_FYDownDenyMACList.Free;

  g_DBAddressList.Free;

  g_MagicCDList.Free;

  g_LogClientPacketUser.Free;
  g_VerifyCodeMapList.Free;
  g_LoadNoVerifyChrList.Free;
end;

procedure TFrmMain.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
begin
  if g_boClose then
  begin
    CanClose := True;
    Exit;
  end;

  if Application.MessageBox('是否确认退出服务器？', '确认信息', MB_YESNO + MB_ICONQUESTION) = IDYES then
  begin
    if FRunGateManager.IsStart then
    begin
      tmrStart.Enabled := True;
      CanClose := False;
    end
    else
    begin
      CanClose := True;
      Exit;
    end;
  end
  else
    CanClose := False;
end;

procedure TFrmMain.StartServices;
var
  SystemInfo: TSystemInfo;
  //ThreadCount: Integer;
begin
  GetSystemInfo(SystemInfo);

  {
  case FRunGateManager.Count of
    0..2: ThreadCount := SystemInfo.dwNumberOfProcessors * 2;
    3..5: ThreadCount := SystemInfo.dwNumberOfProcessors;
  else
    begin
      ThreadCount := SystemInfo.dwNumberOfProcessors div 2;
      if ThreadCount = 0 then ThreadCount := 1;
    end
  end;
  }

  if FRunGateManager.StartRunGates(0) then
  begin
    Caption := GateName + '-' + g_sTitleName;

    mmiStartServer.Enabled := False;
    mmiStopServer.Enabled := True;

    FServicesStartTime := MyGetTickCount;
    FLastRefrshOnlineUserTick := 0;

    if g_wdDBPort > 0 then
    begin
      ServerSocketDB.Active := False;
      ServerSocketDB.Address := g_sGateAddr;
      ServerSocketDB.Port := g_wdDBPort;
      ServerSocketDB.Active := True;

      if ServerSocketDB.Active then
      begin
        AddMainLogMsg('监听DBServer连接端口[' + IntToStr(g_wdDBPort) + ']成功' , 1);
      end;
    end;

    FRemoteBlackIPDown := TIPDownThread.Create(g_FYDownDenyIPList, '重新下载远程IP过滤列表');
    FRemoteWhiteIPDown := TIPDownThread.Create(g_FYDownPassIPList, '重新下载远程IP绿色列表');
    FRemoteDenyMACDown := TMACDownThread.Create(g_FYDownDenyMACList, '重新下载远程机器码过滤列表');
{$IF CLIENT_ANTIPLUG = 1}
    FAntiPlugDown := TAntiPlugDownThread.Create();
    FAntiPlugDown.OnDownloadFinished := OnAntiPlugDownloadFinished;
{$IFEND}

    tmrRefreshInfo.Enabled := True;
    if g_boMinimize and (not FIsEmbeddedGameCenter) then Application.Minimize;
  end
  else
  begin
    Caption := GateName;
    mmiStartServer.Enabled := True;
    mmiStopServer.Enabled := False;
  end;
end;

procedure TFrmMain.StopServices;
var
  I: Integer;
  Thread: TDownloadThread;
  List: TList;
  IsStopAll: Boolean;
begin
  tmrRefreshInfo.Enabled := False;

{$IF NEED_REGISTER = 0}
  OutputDebugString('----------------------------');
{$IFEND}

  FRunGateManager.StopRunGates;
  SaveBlockIPList();

  Caption := GateName;
  mmiStartServer.Enabled := True;
  mmiStopServer.Enabled := False;

  List := TList.Create;
  try
    if FRemoteBlackIPDown <> nil then
    begin
      List.Add(FRemoteBlackIPDown);
      FRemoteBlackIPDown := nil;

    end;

    if FRemoteWhiteIPDown <> nil then
    begin
      List.Add(FRemoteWhiteIPDown);
      FRemoteWhiteIPDown := nil;
    end;

    if FRemoteDenyMACDown <> nil then
    begin
      List.Add(FRemoteDenyMACDown);
      FRemoteDenyMACDown := nil;
    end;

    {
    if FRunGatePlugLoadThread <> nil then
    begin
      List.Add(FRunGatePlugLoadThread);
      FRunGatePlugLoadThread := nil;
    end;
    }

  {$IF CLIENT_ANTIPLUG = 1}
    if FAntiPlugDown <> nil then
    begin
      List.Add(FAntiPlugDown);
      FAntiPlugDown := nil;
    end;
  {$IFEND}

    for I := 0 to List.Count - 1 do
    begin
      Thread := List.Items[I];
      Thread.Terminate;
    end;

  {$IF NEED_REGISTER = 0}
    OutputDebugString('等待更新相关线程退出');
  {$IFEND}

    while True do
    begin
      IsStopAll := True;
      
      for I := 0 to List.Count - 1 do
      begin
        Thread := List.Items[I];
        if Thread.IsRun then
        begin
          IsStopAll := False;
          Break;
        end;
      end;

      Sleep(10);
      Application.ProcessMessages;

      if IsStopAll then Break;
    end;

    for I := 0 to List.Count - 1 do
    begin
      Thread := List.Items[I];
      Thread.Free;
    end;
  finally
    List.Free;
  end;

{$IF NEED_REGISTER = 0}
  OutputDebugString('更新线程全释放');
{$IFEND}
end;

{$IF CLIENT_ANTIPLUG = 1}
procedure _SendDataToClient(ContextID: Integer; DefMsg: pTDefaultMessage; lpData: PChar; DataLen: Integer); stdcall;
var
  Context: TMirClientContext;
begin
  if (FrmMain <> nil) and (FrmMain.FRunGateManager <> nil) and (FrmMain.FRunGateManager.IsStart) then
  begin
    Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[ContextID]);

    if (Context <> nil) and (not Context.boDelayClose) and (not Context.IsPostedCloseQuest) then
    begin
      Context.AddServerMsg(DefMsg, lpData, DataLen);
    end;
  end;
end;

procedure _SendDataToM2(ContextID: Integer; DefMsg: pTDefaultMessage; lpData: PChar; DataLen: Integer); stdcall;
var
  Context: TMirClientContext;
begin
  if (DataLen > 0) and (FrmMain <> nil) and (FrmMain.FRunGateManager <> nil) and (FrmMain.FRunGateManager.IsStart) then
  begin
    Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[ContextID]);

    if (Context <> nil) and (not Context.boDelayClose) and (not Context.IsPostedCloseQuest) then
    begin
      Context.SendMessageToServer(DefMsg^, lpData, DataLen);
    end;
  end;
end;

procedure _EncodeMessage(Msg: PTDefaultMessage; OutText: PChar; var OutLen: Integer); stdcall;
var
  S: string;
begin
  S := EncodeMessage(Msg^);
  OutLen := Length(S);
  Move(S[1], OutText^, OutLen);
end;

procedure _DecodeMessage(InText: PChar; InLen: Integer; Msg: PTDefaultMessage); stdcall;
var
  DefMsg: TDefaultMessage;
  S: string;
begin
  SetLength(S, InLen);
  Move(InText^, S[1], InLen);
  DefMsg := DecodeMessage(S);
  Msg^ := DefMsg;
end;

function _EncodeBuffer(InBuf: PChar; InLen: Integer; OutBuf: PChar; var OutSize: Integer): BOOL; stdcall;
begin
  Result := EncodeBuffer(InBuf, InLen, OutBuf, OutSize) > 0;
end;

function _DecodeBuffer(InBuf: PChar; InLen: Integer; OutBuf: PChar; var OutSize: Integer): BOOL; stdcall;
begin
  Result := DecodeBuffer(InBuf, InLen, OutBuf, OutSize) > 0;
end;

function _ZLibEncodeBuffer(InBuf: PChar; InLen: Integer; OutBuf: PChar; var OutSize: Integer): BOOL; stdcall;
var
  S: string;
begin
  Result := False;
  if InLen > 0 then
  begin
    S := zLibEncodeBuffer(InBuf, InLen);
    Result := Length(S) > 0;
    if Result then
    begin
      OutSize := Length(S);
      if OutBuf <> nil then
      begin
        Move(S[1], OutBuf^, OutSize);
      end;
    end;
  end;
end;

function _ZLibDecodeBuffer(InBuf: PChar; InLen: Integer; OutBuf: PChar; var OutSize: Integer): BOOL; stdcall;
begin
  Result := False;
  if InLen > 0 then
  begin
    OutSize := zLibDecodeBuffer(InBuf, InLen, OutBuf, OutSize);
    Result := OutSize > 0;
  end;
end;

procedure _CloseClient(ClientID: Integer; DelayTime: LongWord{延时时间，毫秒}; Code: Integer{关闭代码}); stdcall;
var
  Context: TMirClientContext;
begin
  if (FrmMain <> nil) and (FrmMain.FRunGateManager <> nil) and (FrmMain.FRunGateManager.IsStart) then
  begin
    Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[ClientID]);

    if (Context <> nil) and (not Context.IsPostedCloseQuest) and (not Context.boDelayClose) then
    begin
      AddMainLogMsg(Format('网关插件即将关闭用户: %s; 代码: %d', [Context.sChrName, Code]), 0);
      Context.DelayClose(DelayTime);
    end;
  end;
end;

function _GetClientInfo(ClientID: Integer; ClientInfo: PRunGatePlugClientInfo): BOOL; stdcall;
var
  S: string;
  Context: TMirClientContext;
begin
  Result := False;

  if (FrmMain = nil) or (FrmMain.FRunGateManager = nil) or (not FrmMain.FRunGateManager.IsStart) then
  begin
    Exit;
  end;

  Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[ClientID]);

  if (Context = nil) then
  begin
    Exit;
  end;

  Result := True;

  FillChar(ClientInfo^, SizeOf(TRunGatePlugClientInfo), 0);

  S := Context.sAccount;
  if Length(S) > 0 then
  begin
    Move(S[1], ClientInfo.Account[0], Min(Length(S), Length(ClientInfo.Account) - 1));
  end;

  S := Context.sChrName;
  if Length(S) > 0 then
  begin
    Move(S[1], ClientInfo.ChrName[0], Min(Length(S), Length(ClientInfo.ChrName) - 1));
  end;

  S := Context.sMachineID;
  if Length(S) > 0 then
  begin
    Move(S[1], ClientInfo.MacID[0], Min(Length(S), Length(ClientInfo.MacID) - 1));
  end;

  S := Context.RemoteAddr;
  if Length(S) > 0 then
  begin
    Move(S[1], ClientInfo.IpAddr[0], Min(Length(S), Length(ClientInfo.IpAddr) - 1));
  end;

  ClientInfo.IPValue := Context.RemoteAddrValue;
  ClientInfo.Port := Context.RemotePort;

  ClientInfo.MoveSpeed := Context.nMoveSpeed;
  ClientInfo.AttackSpeed := Context.nAttackSpeed;
  ClientInfo.SpellSpeed := Context.nSpellSpeed;
  ClientInfo.RecogId := Context.nRecogId;
  
  ClientInfo.IsActive := (not Context.boDelayClose) and (not Context.IsPostedCloseQuest);
  ClientInfo.IsLoginNotice := Context.boLoginNoticeOK;
  ClientInfo.IsPlayGame := Context.boFirstClientQueryBagItems;
  ClientInfo.VerInfo := StrToIntDef(Context.sVersion, 0);

  Context.ContextDataLocker.Lock;
  try
    ClientInfo.DataLen := Context.nContextDataLen;
    ClientInfo.DataAdd := Context.pContextData;
  finally
    Context.ContextDataLocker.UnLock;
  end;
end;

procedure _AddMainLogMsg(lpData: PChar; nLevel: Integer); stdcall;
var
  S: string;
  Len: Integer;
begin
  //S := StrPas(lpData);

  Len := StrLen(lpData);
  SetLength(S, Len);
  Move(lpData^, S[1], Len);

  AddMainLogMsg(S, nLevel, True);
end;

procedure _LockClient(ClientID: Integer; LockTime: LongWord); stdcall;
var
  Context: TMirClientContext;
begin
  if (FrmMain <> nil) and (FrmMain.FRunGateManager <> nil) and (FrmMain.FRunGateManager.IsStart) then
  begin
    Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[ClientID]);

    if (Context <> nil) and (not Context.IsPostedCloseQuest) and (not Context.boDelayClose) then
    begin
      Context.LockUser(LockTime);
    end;
  end;
end;

procedure _SetClientPlugLoad(ClientID: Integer); stdcall;
var
  Context: TMirClientContext;
begin
  if (FrmMain <> nil) and (FrmMain.FRunGateManager <> nil) and (FrmMain.FRunGateManager.IsStart) then
  begin
    Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[ClientID]);

    if (Context <> nil) and (not Context.IsPostedCloseQuest) and (not Context.boDelayClose) then
    begin
      Context.dwRecvLoadAntiPlugTick := MyGetTickCount;
      Context.boRecvLoadAntiPlug := True;
    end;
  end;
end;

{$IFEND}

procedure TFrmMain.LoadConfig(Ports: TList);
var
  I, Count, IntValue: Integer;
  StrValue: string;
  IniFile: TIniFile;

  ActionMode: TAntiPlugActionMode;
  Action: PAntiPlugAction;
  Section: string;
{$IF CLIENT_ANTIPLUG = 1}
  MenuItem, SubMenuItem: TMenuItem;
  sDllName: string;
  PlugInitRecord: TPlugInitRecord;
{$IFEND}

{$IF NEED_REGISTER = 1}
//  ExtendedInfo: Integer;
//  HWID1, HWID2: array[0..100] of Char;
//  sHWID1, sHWID2: string;
{$IFEND}
begin
  g_DefaultConfig := g_Config;

  IniFile := TIniFile.Create(g_sIniFileName);

  g_sTitleName := IniFile.ReadString(GateClass, 'Title', g_sTitleName);
  g_sServerAddr := IniFile.ReadString(GateClass, 'Server1', g_sServerAddr);
  g_wdServerPort := IniFile.ReadInteger(GateClass, 'ServerPort', g_wdServerPort);
  g_sGateAddr := IniFile.ReadString(GateClass, 'GateAddr', g_sGateAddr);
  g_wdGatePort := IniFile.ReadInteger(GateClass, 'GatePort', g_wdGatePort);

  g_wdDBPort := IniFile.ReadInteger(GateClass, 'DBPort', g_wdDBPort);

  g_btShowLogLevel := IniFile.ReadInteger(GateClass, 'ShowLogLevel', g_btShowLogLevel);
  if g_btShowLogLevel <= 0 then
    g_btShowLogLevel := 0
  else if g_btShowLogLevel > cbbShowLogLevel.Items.Count then
    g_btShowLogLevel := cbbShowLogLevel.Items.Count;

  AddMainLogMsg('正在加载配置信息...', 3);

  g_boMinimize := IniFile.ReadBool(GateClass, 'Minimize', g_boMinimize); // 2009-5-26 Micro

  // 增加网关密码控制 piaoyun 2013-08-28
  g_boCheckClientPassWord := IniFile.ReadBool(GateClass, 'CheckClientPassWord', g_boCheckClientPassWord);
  g_sClientPassWord := IniFile.ReadString(GateClass, 'ClientPassWord', g_sClientPassWord);
  if g_sClientPassWord = '' then g_sClientPassWord := 'BmM2';

  g_dwCheckServerTimeOutTime := IniFile.ReadInteger(GateClass, 'CheckM2ServerTimeOut', g_dwCheckServerTimeOutTime);
  if g_dwCheckServerTimeOutTime <= seCheckServerTimeOutTime.MinValue then
    g_dwCheckServerTimeOutTime := seCheckServerTimeOutTime.MinValue
  else if g_dwCheckServerTimeOutTime >= seCheckServerTimeOutTime.MaxValue then
    g_dwCheckServerTimeOutTime := seCheckServerTimeOutTime.MaxValue;

  IntValue := IniFile.ReadInteger(GateClass, 'ClientSendBlockSize', MAX_OVERLAPPEDEX_BUFFER_SIZE);
  if (IntValue >= seClientSendBlockSize.MinValue) and
    (IntValue <= seClientSendBlockSize.MaxValue) then
  begin
    MAX_OVERLAPPEDEX_BUFFER_SIZE := IntValue;
  end;

  IntValue := IniFile.ReadInteger(GateClass, 'MaxPreallocatedMemorySize', MAX_PREALLOCATED_MEMORY_SIZE);
  if (IntValue >= sePreAllocatedCount.MinValue) and
    (IntValue <= sePreAllocatedCount.MaxValue) then
  begin
    MAX_PREALLOCATED_MEMORY_SIZE := IntValue;
  end;

  g_boLogoutNoResendAntiplugStream := IniFile.ReadBool(GateClass, 'LogoutNoResendAntiplugStream', g_boLogoutNoResendAntiplugStream);
  //g_boAntiplugAllLog := IniFile.ReadBool(GateClass, 'AntiplugAllLog', g_boAntiplugAllLog);
  
  IntValue := IniFile.ReadInteger(GateClass, 'RecvAntiPlugHeartbeatTimeOutTime', g_nRecvAntiPlugHeartbeatTimeOutTime);
  if (IntValue >= seRecvAntiPlugHeartbeatTimeOutTime.MinValue) and
    (IntValue <= seRecvAntiPlugHeartbeatTimeOutTime.MaxValue) then
  begin
    g_nRecvAntiPlugHeartbeatTimeOutTime := IntValue;
  end;

  IntValue := IniFile.ReadInteger(GateClass, 'AntiPlugStreamSendSpeed', g_nAntiPlugStreamSendSpeed);
  if (IntValue >= seAntiPlugStreamSendSpeed.MinValue) and
    (IntValue <= seAntiPlugStreamSendSpeed.MaxValue) then
  begin
    g_nAntiPlugStreamSendSpeed := IntValue;
  end;

  IntValue := IniFile.ReadInteger(GateClass, 'AntiPlugStreamSendBlockSize', g_nAntiPlugStreamSendBlockSize);
  if (IntValue >= 0) and
    (IntValue < cbbAntiPlugStreamSendBlockSize.Items.Count) then
  begin
    g_nAntiPlugStreamSendBlockSize := IntValue;
  end;

{$IF CLIENT_ANTIPLUG = 1}
  case g_nAntiPlugStreamSendBlockSize of
    0: // 128K计算方式
      begin
        g_ClientAntiPlugDllSendInterval := 100;
        g_ClientAntiPlugDllBlockSize := 128 * 1024 * g_nAntiPlugStreamSendSpeed div 10;  // 128K * 8个线程 = 1M, 1秒发10个（发送间隔 100)，因此要除以10
      end;
    1:  // 96K计算方式
      begin
        g_ClientAntiPlugDllSendInterval := 75;
        g_ClientAntiPlugDllBlockSize := 128 * 1024 * g_nAntiPlugStreamSendSpeed div 13;
      end;
    2:  // 64K计算方式
      begin
        g_ClientAntiPlugDllSendInterval := 50;
        g_ClientAntiPlugDllBlockSize := 128 * 1024 * g_nAntiPlugStreamSendSpeed div 20;
      end;
    3:  // 32K计算方式
      begin
        g_ClientAntiPlugDllSendInterval := 25;
        g_ClientAntiPlugDllBlockSize := 128 * 1024 * g_nAntiPlugStreamSendSpeed div 40;
      end;
  end;
{$IFEND}

  g_dwClientAccumulateMaxSize := IniFile.ReadInteger(GateClass, 'ClientAccumulateMaxSize', g_dwClientAccumulateMaxSize);

  if IniFile.ReadInteger(GateClass, 'AttackTick', -1) > 0 then
    g_dwAttackTick := IniFile.ReadInteger(GateClass, 'AttackTick', g_dwAttackTick);

  if IniFile.ReadInteger(GateClass, 'AttackCount', -1) > 0 then
    g_nAttackCount := IniFile.ReadInteger(GateClass, 'AttackCount', g_nAttackCount);

  g_nMaxConnOfIPaddr := IniFile.ReadInteger(GateClass, 'MaxConnOfIPaddr', g_nMaxConnOfIPaddr);
  g_BlockMethod := TBlockIPMethod(IniFile.ReadInteger(GateClass, 'BlockMethod', Integer(g_BlockMethod)));

  g_nMaxClientPacketSize := IniFile.ReadInteger(GateClass, 'MaxClientPacketSize', g_nMaxClientPacketSize);
  if g_nMaxClientPacketSize > 512 then g_nMaxClientPacketSize := 128;
  g_nMaxClientPacketCount := IniFile.ReadInteger(GateClass, 'MaxClientPacketCount', g_nMaxClientPacketCount);
  nMaxClientMsgCount := IniFile.ReadInteger(GateClass, 'MaxClientMsgCount', nMaxClientMsgCount);
  g_boKickOverPacketSize := IniFile.ReadBool(GateClass, 'kickOverPacket', g_boKickOverPacketSize);

  g_boCheckClientPacketLegal := IniFile.ReadBool(GateClass, 'CheckClientPacketLegal', g_boCheckClientPacketLegal);
  g_nCheckClientPacketCount := IniFile.ReadInteger(GateClass, 'CheckClientPacketCount', g_nCheckClientPacketCount);

  for ActionMode := Low(TAntiPlugActionMode) to High(TAntiPlugActionMode) do
  begin
    Section := AntiPlugActionModeSections[ActionMode];

    if ActionModeUseSpeedIntervals(ActionMode) and (not (ActionMode in [amHit {攻击}, amSpell {魔法}, amWalk {走路}, amRun {跑步}])) then
    begin
      g_boSendSpeedIntervalsToClient[ActionMode] := IniFile.ReadBool(Section, 'SendSpeedIntervalsToClient', g_boSendSpeedIntervalsToClient[ActionMode]);
    end;

    Action := @g_Config.ActionList[ActionMode];

    if (ActionMode >= amHitConcurrent) then
    begin
      Action.boEnabled := True;
    end
    else
    begin
      Action.boEnabled := IniFile.ReadBool(Section, 'Enabled', Action.boEnabled);
      Action.nInterval := IniFile.ReadInteger(Section, 'Interval', Action.nInterval);
      
      IntValue := IniFile.ReadInteger(Section, 'ProcessMode', Integer(Action.ProcessMode));
      if (IntValue >= Integer(Low(TActionProcessMode))) and (IntValue <= Integer(High(TActionProcessMode))) then
        Action.ProcessMode := TActionProcessMode(IntValue)
      else
        Action.ProcessMode := Low(TActionProcessMode);

      Action.boProcessScript := IniFile.ReadBool(Section, 'ProcessScript', Action.boProcessScript);

      IntValue := IniFile.ReadInteger(Section, 'SumProcessMode', Integer(Action.SumProcessMode));
      if (IntValue >= Integer(Low(TSumActionProcessMode))) and (IntValue <= Integer(High(TSumActionProcessMode))) then
        Action.SumProcessMode := TSumActionProcessMode(IntValue)
      else
        Action.SumProcessMode := Low(TSumActionProcessMode);
      //Action.nFloatingInterval := IniFile.ReadInteger(Section, 'FloatingInterval', Action.nFloatingInterval);
      Action.boShowHint := IniFile.ReadBool(Section, 'ShowHint', Action.boShowHint);
      Action.sHintText := IniFile.ReadString(Section, 'HintText', Action.sHintText);
      Action.nCompensationValue := IniFile.ReadInteger(Section, 'CompensationValue', Action.nCompensationValue);
      Action.boDebug := IniFile.ReadBool(Section, 'Debug', Action.boDebug);
    end;

    // 并发不让设置 chongchong 2018-11-12 22:30:21
    {
    Action.boProcessScript := IniFile.ReadBool(Section, 'ProcessScript', Action.boProcessScript);

    IntValue := IniFile.ReadInteger(Section, 'SumProcessMode', Integer(Action.SumProcessMode));
    if (IntValue >= Integer(Low(TSumActionProcessMode))) and (IntValue <= Integer(High(TSumActionProcessMode))) then
      Action.SumProcessMode := TSumActionProcessMode(IntValue)
    else
      Action.SumProcessMode := Low(TSumActionProcessMode);
    //Action.nFloatingInterval := IniFile.ReadInteger(Section, 'FloatingInterval', Action.nFloatingInterval);
    Action.boShowHint := IniFile.ReadBool(Section, 'ShowHint', Action.boShowHint);
    Action.sHintText := IniFile.ReadString(Section, 'HintText', Action.sHintText);
    Action.nCompensationValue := IniFile.ReadInteger(Section, 'CompensationValue', Action.nCompensationValue);
    Action.boDebug := IniFile.ReadBool(Section, 'Debug', Action.boDebug);
    }
  end;

  g_Config.nLockTime := IniFile.ReadInteger('Setup', 'LockTime', g_Config.nLockTime);
  g_Config.boSaveLockStatus := IniFile.ReadBool('Setup', 'SaveLockStatus', g_Config.boSaveLockStatus);
  g_Config.boShowLockLog := IniFile.ReadBool('Setup', 'ShowLockLog', g_Config.boShowLockLog);
  g_Config.sShowLockMsg := IniFile.ReadString('Setup', 'ShowLockMsg', g_Config.sShowLockMsg);

  g_Config.boSpeedClearData := IniFile.ReadBool('Setup', 'SpeedClearData', g_Config.boSpeedClearData);

  g_Config.btMsgType := IniFile.ReadInteger('Setup', 'MsgType', g_Config.btMsgType);
  g_Config.btMsgFColor := IniFile.ReadInteger('Setup', 'MsgFColor', g_Config.btMsgFColor);
  g_Config.btMsgBColor := IniFile.ReadInteger('Setup', 'MsgBColor', g_Config.btMsgBColor);

  g_Config.dwUserShop_Search_Interval := IniFile.ReadInteger('Setup', 'UserShopSearchInterval', g_Config.dwUserShop_Search_Interval);
  g_Config.boUserShop_Search_ShowHint := IniFile.ReadBool('Setup', 'UserShopSearchShowHint', g_Config.boUserShop_Search_ShowHint);

  g_Config.dwUserShop_Buy_Interval := IniFile.ReadInteger('Setup', 'UserShopBuyInterval', g_Config.dwUserShop_Buy_Interval);
  g_Config.boUserShop_Buy_ShowHint := IniFile.ReadBool('Setup', 'UserShopBuyShowHint', g_Config.boUserShop_Buy_ShowHint);

  g_Config.dwTakeOn_Item_Interval := IniFile.ReadInteger('Setup', 'TakeOnItemInterval', g_Config.dwTakeOn_Item_Interval);
  g_Config.boTakeOn_Item_ShowHint := IniFile.ReadBool('Setup', 'TakeOnItemShowHint', g_Config.boTakeOn_Item_ShowHint);

  g_Config.dwDealTry_Attack_Interval := IniFile.ReadInteger('Setup', 'DealTryAttackInterval', g_Config.dwDealTry_Attack_Interval);
  g_Config.boDealTry_Attack_ShowHint := IniFile.ReadBool('Setup', 'DealTryAttackShowHint', g_Config.boDealTry_Attack_ShowHint);

  g_Config.dwBrutal_Attack_Interval := IniFile.ReadInteger('Setup', 'BrutalAttackInterval', g_Config.dwBrutal_Attack_Interval);
  g_Config.boBrutal_Attack_ShowHint := IniFile.ReadBool('Setup', 'BrutalAttackShowHint', g_Config.boBrutal_Attack_ShowHint);

  g_Config.boShowAttackLog := IniFile.ReadBool('Setup', 'ShowAttackLog', g_Config.boShowAttackLog);
  //g_Config.boShowDropConcurrentLog := IniFile.ReadBool('Setup', 'ShowDropConcurrentLog', g_Config.boShowDropConcurrentLog);
  g_Config.dwContinueSpeedPassIncTime := IniFile.ReadInteger('Setup', 'ContinueSpeedPassIncTime', g_Config.dwContinueSpeedPassIncTime);

  IntValue := IniFile.ReadInteger('Setup', 'CollectCount', g_Config.dwCollectCount);
  if (IntValue >= 5) and (IntValue <= 20) then
  begin
    g_Config.dwCollectCount := IntValue;
  end;

  IntValue := IniFile.ReadInteger('Setup', 'SpeedValue', g_Config.dwSpeedValue);
  if (IntValue >= 2) and (IntValue <= 18) then
  begin
    g_Config.dwSpeedValue := IntValue;
  end;

  if g_Config.dwSpeedValue >= g_Config.dwCollectCount then
  begin
    g_Config.dwSpeedValue := g_Config.dwCollectCount - 1;
  end;

  g_Config.boZeroCompensationValueClearPool := IniFile.ReadBool('Setup', 'ZeroCompensationValueClearPool', g_Config.boZeroCompensationValueClearPool);

  g_Config.dwClientUploadPickItemsTime := IniFile.ReadInteger('Setup', 'ClientUploadPickItemsTime', g_Config.dwClientUploadPickItemsTime);

  g_Config.boContinueSpeedCloseSocket := IniFile.ReadBool('Setup', 'ContinueSpeedCloseSocket', g_Config.boContinueSpeedCloseSocket);
  IntValue := IniFile.ReadInteger('Setup', 'ContinueSpeedCount', g_Config.nContinueSpeedCount);
  if (IntValue >= 2) and (IntValue <= 8) then
    g_Config.nContinueSpeedCount := IntValue;

  g_Config.nSumSpeedCheckTime := IniFile.ReadInteger('Setup', 'SumSpeedCheckTime', g_Config.nSumSpeedCheckTime);
  g_Config.nSumSpeedMaxCount := IniFile.ReadInteger('Setup', 'SumSpeedMaxCount', g_Config.nSumSpeedMaxCount);

  g_boFilterSayMsg := IniFile.ReadBool(GateClass, 'FilterSayMsg', g_boFilterSayMsg);
  IntValue := IniFile.ReadInteger(GateClass, 'FilterSayMsgMode', Integer(g_FilterSayMsgMode));
  if (IntValue >= Integer(Low(TFilterSayMsgMode))) and (IntValue <= Integer(High(TFilterSayMsgMode))) then
    g_FilterSayMsgMode := TFilterSayMsgMode(IntValue);
  g_WarnSayMsg := IniFile.ReadString(GateClass, 'WarnSayMsg', '');

  g_boFilterSayTriggerScript := IniFile.ReadBool(GateClass, 'FilterSayTriggerScript', g_boFilterSayTriggerScript);

  {--------------------------- chongchong 2013-09-01 ---------------------}
  g_dwKeepConnectTimeOut := IniFile.ReadInteger(GateClass, 'KeepConnectTimeOut', g_dwKeepConnectTimeOut);

  // 是否开启发言控制
  g_boSayMsgControl := IniFile.ReadBool(GateClass, 'SayMsgControl', g_boSayMsgControl);

  // 发言文字最大长度
  g_dwSayMaxLen := IniFile.ReadInteger(GateClass, 'SayMaxLen', g_dwSayMaxLen);

  // 发言时间间隔
  g_dwSayTime := IniFile.ReadInteger(GateClass, 'SayTime', g_dwSayTime);

  // 发言次数
  g_dwSayMaxCount := IniFile.ReadInteger(GateClass, 'SayMaxCount', g_dwSayMaxCount);

  // 禁言时间
  g_dwSayDisableTime := IniFile.ReadInteger(GateClass, 'SayDisableTime', g_dwSayDisableTime);

  // 禁言时间内
  StrValue := IniFile.ReadString('String', 'DisableSayMsg', '');
  if Length(StrValue) = 0 then
    IniFile.WriteString('String', 'DisableSayMsg', g_sDisableSayMsg)
  else
    g_sDisableSayMsg := StrValue;

  // 开始禁言提示
  StrValue := IniFile.ReadString('String', 'DisableSayMsgBegin', '');
  if Length(StrValue) = 0 then
    IniFile.WriteString('String', 'DisableSayMsgBegin', g_sDisableSayMsgBegin)
  else
    g_sDisableSayMsgBegin := StrValue;

  g_dwIPCountLimitTime1 := IniFile.ReadInteger(GateClass, 'IPCountLimitTime1', g_dwIPCountLimitTime1);
  g_dwIPCountLimit1 := IniFile.ReadInteger(GateClass, 'IPCountLimit1', g_dwIPCountLimit1);
  g_dwIPCountLimitTime2 := IniFile.ReadInteger(GateClass, 'IPCountLimitTime2', g_dwIPCountLimitTime2);
  g_dwIPCountLimit2 := IniFile.ReadInteger(GateClass, 'IPCountLimit2', g_dwIPCountLimit2);

  // 防御等级
  g_dwDefenseLevel := IniFile.ReadInteger(GateClass, 'DefenseLevel', g_dwDefenseLevel);

  // 受攻击防御调为1级
  g_boDefenseToLevel1 := IniFile.ReadBool(GateClass, 'IsDefenseToLevel1', g_boDefenseToLevel1);

  // 受攻击防御调为1级 (攻击次数)
  g_dwDefenseToLevel1 := IniFile.ReadInteger(GateClass, 'DefenseToLevel1', g_dwDefenseToLevel1);

  // 无攻击还原防御等级
  g_boResotreDefense := IniFile.ReadBool(GateClass, 'IsResotreDefense', g_boResotreDefense);

  // 无攻击还原防御等级 (120秒后)
  g_dwResotreDefense := IniFile.ReadInteger(GateClass, 'ResotreDefense', g_dwResotreDefense);

  // 清除动态过滤列表
  g_boAutoClearTemp := IniFile.ReadBool(GateClass, 'IsAutoClearTemp', g_boAutoClearTemp);

  // 清除动态过滤列表 (自动清除间隔120秒)
  g_dwAutoClearTemp := IniFile.ReadInteger(GateClass, 'AutoClearTemp', g_dwAutoClearTemp);

  // 连接加入到动态过滤
  g_boAddAllToTemp := IniFile.ReadBool(GateClass, 'IsAddAllToTemp', g_boAddAllToTemp);

  // 连接加入到动态过滤 (连接数)
  g_dwAddAllToTemp := IniFile.ReadInteger(GateClass, 'AddAllToTemp', g_dwAddAllToTemp);

  // 启用客户端验证
  g_boOpenCheckClient := IniFile.ReadBool(GateClass, 'OpenCheckClient', g_boOpenCheckClient);

  g_CheckClientFailBlockMethod := TBlockIPMethod(IniFile.ReadInteger(GateClass, 'CheckClientFailBlockMethod', Integer(g_CheckClientFailBlockMethod)));

  // 读取多个网关 chongchong 2014-06-22
  Count := IniFile.ReadInteger('GameGates', 'Count', 0);
  for I := 1 to Count do
  begin
    IntValue := IniFile.ReadInteger('GameGates', 'Port' + IntToStr(I), 0);
    if (IntValue > 0) and (IntValue <= 65535) then
      Ports.Add(Pointer(IntValue)); 
  end;

  // 防御设置相关 chongchong 2015-01-01
  g_sFYReadDenyIPFile := IniFile.ReadString(GateClass, 'FYReadDenyIPFile', g_sFYReadDenyIPFile);
  g_dwFYReadDenyIPTime := IniFile.ReadInteger(GateClass, 'FYReadDenyIPTime', g_dwFYReadDenyIPTime);

  if IniFile.ValueExists(GateClass, 'FYDownDenyIPUrl') then
    g_sFYDownDenyIPUrl := IniFile.ReadString(GateClass, 'FYDownDenyIPUrl', '');

  g_dwFYDownDenyIPTime := IniFile.ReadInteger(GateClass, 'FYDownDenyIPTime', g_dwFYDownDenyIPTime);

  g_sFYReadPassIPFile := IniFile.ReadString(GateClass, 'FYReadPassIPFile', g_sFYReadPassIPFile);
  g_dwFYReadPassIPTime := IniFile.ReadInteger(GateClass, 'FYReadPassIPTime', g_dwFYReadPassIPTime);

  if IniFile.ValueExists(GateClass, 'FYDownPassIPUrl') then
    g_sFYDownPassIPUrl := IniFile.ReadString(GateClass, 'FYDownPassIPUrl', '');

  g_dwFYDownPassIPTime := IniFile.ReadInteger(GateClass, 'FYDownPassIPTime', g_dwFYDownPassIPTime);

  g_sFYReadDenyMACFile := IniFile.ReadString(GateClass, 'FYReadDenyMACFile', g_sFYReadDenyMACFile);
  g_dwFYReadDenyMACTime := IniFile.ReadInteger(GateClass, 'FYReadDenyMACTime', g_dwFYReadDenyMACTime);

  g_sFYDownDenyMACUrl := IniFile.ReadString(GateClass, 'FYDownDenyMACUrl', g_sFYDownDenyMACUrl);
  g_dwFYDownDenyMACTime := IniFile.ReadInteger(GateClass, 'FYDownDenyMACTime', g_dwFYDownDenyMACTime);

  g_OnlyWhiteListLink := IniFile.ReadBool(GateClass, 'OnlyWhiteListLink', g_OnlyWhiteListLink);

  g_boLogClientPacket := IniFile.ReadBool(GateClass, 'LogClientPacket', g_boLogClientPacket);
  g_nLogClientPacketType := IniFile.ReadInteger(GateClass, 'LogClientPacketType', g_nLogClientPacketType);

{$IF CLIENT_ANTIPLUG = 1}
  g_ClientAntiPlugStream := TMemoryStream.Create;
  InitializeCriticalSection(g_CSRunGatePlug);

  g_boAntiPlugAutoUpdateCheck := IniFile.ReadBool(GateClass, 'AntiPlugAutoUpdateCheck', g_boAntiPlugAutoUpdateCheck);
  g_wAntiPlugUpdateCheckInterval := IniFile.ReadInteger(GateClass, 'AntiPlugUpdateCheckInterval', g_wAntiPlugUpdateCheckInterval);
  if g_wAntiPlugUpdateCheckInterval < 1 then g_wAntiPlugUpdateCheckInterval := 1;
  g_sAntiPlugUpdateConfigUrl := IniFile.ReadString(GateClass, 'AntiPlugUpdateConfigUrl5', g_sAntiPlugUpdateConfigUrl);
{$IFEND}

  g_boOneMACLimitePlayer := IniFile.ReadBool(GateClass, 'OneMACLimitePlayer', g_boOneMACLimitePlayer);
  g_nOneMACLimitePlayerCount := IniFile.ReadInteger(GateClass, 'OneMACLimitePlayerCount', g_nOneMACLimitePlayerCount);

  g_nClientLogoutDelay := IniFile.ReadInteger(GateClass, 'ClientLogoutDelay', g_nClientLogoutDelay);
  g_nClientCloseDelay := IniFile.ReadInteger(GateClass, 'ClientCloseDelay', g_nClientCloseDelay);

  g_boDelayCloseDisableMove := IniFile.ReadBool(GateClass, 'DelayCloseDisableMove', g_boDelayCloseDisableMove);
  g_boDelayCloseDisableSpell := IniFile.ReadBool(GateClass, 'DelayCloseDisableSpell', g_boDelayCloseDisableSpell);
  g_boDelayCloseDisableAttack := IniFile.ReadBool(GateClass, 'DelayCloseDisableAttack', g_boDelayCloseDisableAttack);
  g_boDelayCloseDisableUseItem := IniFile.ReadBool(GateClass, 'DelayCloseDisableUseItem', g_boDelayCloseDisableUseItem);

  g_boBreakClientLogoutHint := IniFile.ReadBool(GateClass, 'ShowBreakClientLogoutHint', g_boBreakClientLogoutHint);
  g_sBreakClientLogoutHint := IniFile.ReadString(GateClass, 'BreakClientLogoutHint', g_sBreakClientLogoutHint);
  g_boBreakClientCloseHint := IniFile.ReadBool(GateClass, 'ShowBreakClientCloseHint', g_boBreakClientCloseHint);
  g_sBreakClientCloseHint := IniFile.ReadString(GateClass, 'BreakClientCloseHint', g_sBreakClientCloseHint);


  IntValue := IniFile.ReadInteger('MagicCD', 'MsgType', g_btMagicCDMsgType);
  if (IntValue >= 0) and (IntValue <= 2) then
  begin
    g_btMagicCDMsgType := IntValue;
  end;

  g_sMagicCDMsgText := IniFile.ReadString('MagicCD', 'MsgText', g_sMagicCDMsgText);
  g_btMagicCDFColor := IniFile.ReadInteger('MagicCD', 'FColor', g_btMagicCDFColor);
  g_btMagicCDBColor := IniFile.ReadInteger('MagicCD', 'BColor', g_btMagicCDBColor);
  g_nMagicCDShowX := IniFile.ReadInteger('MagicCD', 'ShowX', g_nMagicCDShowX);
  g_nMagicCDShowY := IniFile.ReadInteger('MagicCD', 'ShowY', g_nMagicCDShowY);


  for I := 0 to Length(g_EatItemCDConfig.Hum) - 1 do
  begin
    StrValue := IntToStr(I + 1);
    g_EatItemCDConfig.Hum[I].NormalHP := IniFile.ReadInteger('HumanItemEatCD', StrValue + 'NormalHP', g_EatItemCDConfig.Hum[I].NormalHP);
    g_EatItemCDConfig.Hum[I].NormalMP := IniFile.ReadInteger('HumanItemEatCD', StrValue + 'NormalMP', g_EatItemCDConfig.Hum[I].NormalMP);
    g_EatItemCDConfig.Hum[I].NormalHPMP := IniFile.ReadInteger('HumanItemEatCD', StrValue + 'NormalHPMP', g_EatItemCDConfig.Hum[I].NormalHPMP);
    g_EatItemCDConfig.Hum[I].SpecialHP := IniFile.ReadInteger('HumanItemEatCD', StrValue + 'SpecialHP', g_EatItemCDConfig.Hum[I].SpecialHP);
    g_EatItemCDConfig.Hum[I].SpecialMP := IniFile.ReadInteger('HumanItemEatCD', StrValue + 'SpecialMP', g_EatItemCDConfig.Hum[I].SpecialMP);
    g_EatItemCDConfig.Hum[I].SpecialHPMP := IniFile.ReadInteger('HumanItemEatCD', StrValue + 'SpecialHPMP', g_EatItemCDConfig.Hum[I].SpecialHPMP);
    g_EatItemCDConfig.Hum[I].Other := IniFile.ReadInteger('HumanItemEatCD', StrValue + 'Other', g_EatItemCDConfig.Hum[I].Other);
  end;

  for I := 0 to Length(g_EatItemCDConfig.Hero) - 1 do
  begin
    StrValue := IntToStr(I + 1);
    g_EatItemCDConfig.Hero[I].NormalHP := IniFile.ReadInteger('HeroItemEatCD', StrValue + 'NormalHP', g_EatItemCDConfig.Hero[I].NormalHP);
    g_EatItemCDConfig.Hero[I].NormalMP := IniFile.ReadInteger('HeroItemEatCD', StrValue + 'NormalMP', g_EatItemCDConfig.Hero[I].NormalMP);
    g_EatItemCDConfig.Hero[I].NormalHPMP := IniFile.ReadInteger('HeroItemEatCD', StrValue + 'NormalHPMP', g_EatItemCDConfig.Hero[I].NormalHPMP);
    g_EatItemCDConfig.Hero[I].SpecialHP := IniFile.ReadInteger('HeroItemEatCD', StrValue + 'SpecialHP', g_EatItemCDConfig.Hero[I].SpecialHP);
    g_EatItemCDConfig.Hero[I].SpecialMP := IniFile.ReadInteger('HeroItemEatCD', StrValue + 'SpecialMP', g_EatItemCDConfig.Hero[I].SpecialMP);
    g_EatItemCDConfig.Hero[I].SpecialHPMP := IniFile.ReadInteger('HeroItemEatCD', StrValue + 'SpecialHPMP', g_EatItemCDConfig.Hero[I].SpecialHPMP);
    g_EatItemCDConfig.Hero[I].Other := IniFile.ReadInteger('HeroItemEatCD', StrValue + 'Other', g_EatItemCDConfig.Hero[I].Other);
  end;
  
  IniFile.Free;

  edtGateIPaddr.Text := g_sGateAddr;
  edtGatePort.Text := IntToStr(g_wdGatePort);

  edtServerIPaddr.Text := g_sServerAddr;
  edtServerPort.Text := IntToStr(g_wdServerPort);

  edtDBPort.Text := IntToStr(g_wdDBPort);

  chkClientPassword.Checked := g_boCheckClientPassword;
  edtClientPassword.Text := g_sClientPassWord;

  edtTitleName.Text := g_sTitleName;
  cbbShowLogLevel.ItemIndex := g_btShowLogLevel;
  seRecvAntiPlugHeartbeatTimeOutTime.Value := g_nRecvAntiPlugHeartbeatTimeOutTime;
  seAntiPlugStreamSendSpeed.Value := g_nAntiPlugStreamSendSpeed;
  cbbAntiPlugStreamSendBlockSize.ItemIndex := g_nAntiPlugStreamSendBlockSize;
  chkLogoutNoResendAntiplugStream.Checked := g_boLogoutNoResendAntiplugStream;
  chkAntiplugAllLog.Checked := g_boAntiplugAllLog;

  chkMinimize.Checked := g_boMinimize;

  seCheckServerTimeOutTime.Value := g_dwCheckServerTimeOutTime;
  seClientSendBlockSize.Value := MAX_OVERLAPPEDEX_BUFFER_SIZE;
  sePreAllocatedCount.Value := MAX_PREALLOCATED_MEMORY_SIZE;
  seClientAccumulateMaxSize.Value := g_dwClientAccumulateMaxSize;

  chkOneMACLimitePlayer.Checked := g_boOneMACLimitePlayer;
  seOneMACLimitePlayer.Value := g_nOneMACLimitePlayerCount;

  seClientLogoutDelay.Value := g_nClientLogoutDelay;
  seClientCloseDelay.Value := g_nClientCloseDelay;

  chkDelayCloseDisableMove.Checked := g_boDelayCloseDisableMove;
  chkDelayCloseDisableSpell.Checked := g_boDelayCloseDisableSpell;
  chkDelayCloseDisableAttack.Checked := g_boDelayCloseDisableAttack;
  chkDelayCloseDisableUseItem.Checked := g_boDelayCloseDisableUseItem;

  chkBreakClientLogoutHint.Checked := g_boBreakClientLogoutHint;
  edtBreakClientLogoutHint.Text := g_sBreakClientLogoutHint;

  chkBreakClientCloseHint.Checked := g_boBreakClientCloseHint;
  edtBreakClientCloseHint.Text := g_sBreakClientCloseHint;
  
  RecallPreAllocatedSize;

  LoadFilterSayMsgFile();
  LoadBlockIPFile();
  LoadIPSectionList();
  LoadBlockMacFile();

  LoadProcessBlacklist;

  LoadNoVerifyChrList;
  g_dwAutoLoadNoVerifyChrListTick := MyGetTickCount;

  for ActionMode := Low(TAntiPlugActionMode) to High(TAntiPlugActionMode) do
  begin
    if (Length(g_sActionIntervalsFileNames[ActionMode]) > 0) and FileExists(g_sActionIntervalsFileNames[ActionMode]) then
    begin
      try
        IniFile := TIniFile.Create(g_sActionIntervalsFileNames[ActionMode]);
        for I := 0 to SPEED_INTERVALS_COUNT - 1 do
        begin
          g_wActionSpeedIntervals[ActionMode][I] := IniFile.ReadInteger('Intervals', 'Speed' + IntToStr(I - HALF_SPEED_INTERVALS_COUNT), g_wActionSpeedIntervals[ActionMode][I]);
        end;
        IniFile.Free;
      except
      end;
    end;
  end;

  RebuildSendToClientSpeedIntervalsText;

  LoadDBAddressTable();

  if FileExists(g_sLogClientPakcetUserFile) then
    g_LogClientPacketUser.LoadFromFile(g_sLogClientPakcetUserFile);

  AddMainLogMsg('加载配置信息完成', 3);

  grpVerifyCode.Visible := False;
  btnSettingOK.Left := grpClientExitDaly.Left + grpClientExitDaly.Width - btnSettingOK.Width;
  btnSettingOK.Top := grpClientExitDaly.Top + grpClientExitDaly.Height + 5;

{$IF CLIENT_ANTIPLUG = 1}
  {$IF NEED_REGISTER = 1}
//    {$I Registered_Start.inc}
//    if (WLRegGetStatus(ExtendedInfo) = wlIsRegistered) then
//    begin
//      WLHardwareGetID(HWID1);
//      WLRegGetLicenseHardwareID(HWID2);
//
//      sHWID1 := StrPas(HWID1);
//      sHWID2 := StrPas(HWID2);
//
//      if SameText(sHWID1, sHWID2) then
//      begin
        MenuItem := TMenuItem.Create(Self);
        MenuItem.Caption := '网关插件';
        mmMain.Items.Insert(2, MenuItem);

        SubMenuItem := TMenuItem.Create(Self);
        SubMenuItem.Caption := '重新加载';  // '重加载网关插件';
        SubMenuItem.OnClick := OnmmiReloadRunGatePlugClick;
        MenuItem.Add(SubMenuItem);

        SubMenuItem := TMenuItem.Create(Self);
        SubMenuItem.Caption := '卸载插件';  // '卸载网关插件';
        SubMenuItem.OnClick := OnmmiUnloadRunGatePlugClick;
        MenuItem.Add(SubMenuItem);

        SubMenuItem := TMenuItem.Create(Self);
        SubMenuItem.Caption := '-';
        MenuItem.Add(SubMenuItem);

        SubMenuItem := TMenuItem.Create(Self);
        SubMenuItem.Caption := '-';
        MenuItem.Add(SubMenuItem);

        SubMenuItem := TMenuItem.Create(Self);
        SubMenuItem.Caption := '更新设置';
        SubMenuItem.OnClick := OnmmiAntiPlugUpdateSettingClick;
        MenuItem.Add(SubMenuItem);

        SubMenuItem := TMenuItem.Create(Self);
        SubMenuItem.Caption := '-';
        MenuItem.Add(SubMenuItem);

        SubMenuItem := TMenuItem.Create(Self);
        SubMenuItem.Caption := '高级设置';
        SubMenuItem.OnClick := OnmmiRunGatePlugSettingClick;
        MenuItem.Add(SubMenuItem);

        if g_RunGatePlugDllHandle = 0 then
        begin
          sDllName := ExtractFilePath(Application.ExeName) + g_sRunGatePlusDllName;
          if FileExists(sDllName) then
            g_RunGatePlugDllHandle := LoadLibrary(PChar(sDllName));
        end;

        if g_RunGatePlugDllHandle <> 0 then
        begin                      
          g_rgpDoInit := GetProcAddress(g_RunGatePlugDllHandle, 'Init');

          if Assigned(g_rgpDoInit) then
          begin
            FillChar(PlugInitRecord, SizeOf(PlugInitRecord), 0);

            PlugInitRecord.MainFormHandle := Handle;
            PlugInitRecord.GetClientInfo := @_GetClientInfo;
            PlugInitRecord.AddMainLogMsg := @_AddMainLogMsg;
            PlugInitRecord.SendDataToClient := @_SendDataToClient;
            PlugInitRecord.EncodeBuffer := @_EncodeBuffer;
            PlugInitRecord.DecodeBuffer := @_DecodeBuffer;
            PlugInitRecord.CloseClient := @_CloseClient;
            PlugInitRecord.LockClient := @_LockClient;
            PlugInitRecord.SetClientPlugLoad := @_SetClientPlugLoad;

            g_rgpDoInit(@PlugInitRecord, False);
          end;

          g_rgpDoUninit := GetProcAddress(g_RunGatePlugDllHandle, 'Uninit');
          g_rgpStartContext := GetProcAddress(g_RunGatePlugDllHandle, 'ClientStart');
          g_rgpEndContext := GetProcAddress(g_RunGatePlugDllHandle, 'ClientEnd');
          g_rgpRecvPacket := GetProcAddress(g_RunGatePlugDllHandle, 'ClientRecvPacket');
          g_rgpShowConfigForm := GetProcAddress(g_RunGatePlugDllHandle, 'ShowConfigForm');
        end;

        if LoadClientAntiPlugDll then
        begin
          AddMainLogMsg('反外挂模块加载完成', 0);
        end;

        grpVerifyCode.Visible := True;

        btnSettingOK.Left := grpVerifyCode.Left + grpVerifyCode.Width - btnSettingOK.Width;
        btnSettingOK.Top := grpVerifyCode.Top + grpVerifyCode.Height + 5;

        IniFile := TIniFile.Create(g_sIniFileName);
        try
          g_boOpenVerifyCode := IniFile.ReadBool(GateClass, 'OpenVerifyCode', g_boOpenVerifyCode);
          g_nVerifyCodeErrCount := IniFile.ReadInteger(GateClass, 'VerifyCodeErrCount', g_nVerifyCodeErrCount);
          g_nVerifyCodeRefreshCount := IniFile.ReadInteger(GateClass, 'VerifyCodeRefreshCount', g_nVerifyCodeRefreshCount);
          g_nVerifyCodeWaitTime := IniFile.ReadInteger(GateClass, 'VerifyCodeWaitTime', g_nVerifyCodeWaitTime);
          g_dwVerifyCodeInterval1 := IniFile.ReadInteger(GateClass, 'VerifyCodeInterval1', g_dwVerifyCodeInterval1);
          g_dwVerifyCodeInterval2 := IniFile.ReadInteger(GateClass, 'VerifyCodeInterval2', g_dwVerifyCodeInterval2);
          g_dwVerifySuccessAddInterval := IniFile.ReadInteger(GateClass, 'VerifySuccessAddInterval', g_dwVerifySuccessAddInterval);
          g_boVerifyFailTriggerScript := IniFile.ReadBool(GateClass, 'VerifyFailTriggerScript', g_boVerifyFailTriggerScript);
          g_boVerifyFailLoginVerify := IniFile.ReadBool(GateClass, 'VerifyFailLoginVerify', g_boVerifyFailLoginVerify);
          g_boVerifyCodeExcludeMap := IniFile.ReadBool(GateClass, 'VerifyCodeExcludeMap', g_boVerifyCodeExcludeMap);

          g_boAutoLoadNoVerifyChrList := IniFile.ReadBool(GateClass, 'AutoLoadNoVerifyChrList', g_boAutoLoadNoVerifyChrList);
          g_sLoadNoVerifyChrListFile := IniFile.ReadString(GateClass, 'LoadNoVerifyChrListFile', g_sLoadNoVerifyChrListFile);
          g_nAutoLoadNoVerifyChrListInterval := IniFile.ReadInteger(GateClass, 'AutoLoadNoVerifyChrListInterval', g_nAutoLoadNoVerifyChrListInterval);
          chkVerifyCode.Checked := g_boOpenVerifyCode;
          seVerifyCodeErrCount.Value := g_nVerifyCodeErrCount;
          seVerifyCodeRefreshCount.Value := g_nVerifyCodeRefreshCount;
          seVerifyCodeWaitTime.Value := g_nVerifyCodeWaitTime;
          seVerifyCodeInterval1.Value := g_dwVerifyCodeInterval1;
          seVerifyCodeInterval2.Value := g_dwVerifyCodeInterval2;
          seVerifySuccessAddInterval.Value := g_dwVerifySuccessAddInterval;
          chkVerifyFailTriggerScript.Checked := g_boVerifyFailTriggerScript;
          chkVerifyFailLoginVerify.Checked := g_boVerifyFailLoginVerify;
          chkVerifyCodeExcludeMap.Checked := g_boVerifyCodeExcludeMap;

          if FileExists(g_sVerifyCodeExcludeMapFileName) then
          begin
            g_VerifyCodeMapList.LoadFromFile(g_sVerifyCodeExcludeMapFileName);
          end;
          lstVerifyCodeExcludeMap.Items.Text := g_VerifyCodeMapList.Text;

          chkAutoLoadNoVerifyChrList.Checked := g_boAutoLoadNoVerifyChrList;
          edtLoadNoVerifyChrListFile.Text := g_sLoadNoVerifyChrListFile;
          seAutoLoadNoVerifyChrListInterval.Value := g_nAutoLoadNoVerifyChrListInterval;
        finally
          IniFile.Free;
        end;
//      end;
//    end;
//    {$I Registered_End.inc}
  {$ELSE}
    MenuItem := TMenuItem.Create(Self);
    MenuItem.Caption := '网关插件';
    mmMain.Items.Insert(2, MenuItem);

    SubMenuItem := TMenuItem.Create(Self);
    SubMenuItem.Caption := '重新加载';  // '重加载网关插件';
    SubMenuItem.OnClick := OnmmiReloadRunGatePlugClick;
    MenuItem.Add(SubMenuItem);

    SubMenuItem := TMenuItem.Create(Self);
    SubMenuItem.Caption := '卸载插件';  // '卸载网关插件';
    SubMenuItem.OnClick := OnmmiUnloadRunGatePlugClick;
    MenuItem.Add(SubMenuItem);

    SubMenuItem := TMenuItem.Create(Self);
    SubMenuItem.Caption := '-';
    MenuItem.Add(SubMenuItem);

    SubMenuItem := TMenuItem.Create(Self);
    SubMenuItem.Caption := '-';
    MenuItem.Add(SubMenuItem);

    SubMenuItem := TMenuItem.Create(Self);
    SubMenuItem.Caption := '更新设置';
    SubMenuItem.OnClick := OnmmiAntiPlugUpdateSettingClick;
    MenuItem.Add(SubMenuItem);

    SubMenuItem := TMenuItem.Create(Self);
    SubMenuItem.Caption := '-';
    MenuItem.Add(SubMenuItem);

    SubMenuItem := TMenuItem.Create(Self);
    SubMenuItem.Caption := '高级设置';
    SubMenuItem.OnClick := OnmmiRunGatePlugSettingClick;
    MenuItem.Add(SubMenuItem);

    if g_RunGatePlugDllHandle = 0 then
    begin
      sDllName := ExtractFilePath(Application.ExeName) + g_sRunGatePlusDllName;
      if FileExists(sDllName) then
        g_RunGatePlugDllHandle := LoadLibrary(PChar(sDllName));
    end;

    if g_RunGatePlugDllHandle <> 0 then
    begin
      g_rgpDoInit := GetProcAddress(g_RunGatePlugDllHandle, 'Init');

      if Assigned(g_rgpDoInit) then
      begin
        FillChar(PlugInitRecord, SizeOf(PlugInitRecord), 0);

        PlugInitRecord.MainFormHandle := Handle;
        PlugInitRecord.GetClientInfo := @_GetClientInfo;
        PlugInitRecord.AddMainLogMsg := @_AddMainLogMsg;
        PlugInitRecord.SendDataToClient := @_SendDataToClient;
        PlugInitRecord.EncodeBuffer := @_EncodeBuffer;
        PlugInitRecord.DecodeBuffer := @_DecodeBuffer;
        PlugInitRecord.CloseClient := @_CloseClient;
        PlugInitRecord.LockClient := @_LockClient;
        PlugInitRecord.SetClientPlugLoad := @_SetClientPlugLoad;

        //PlugInitRecord.SendDataToM2 := @_SendDataToM2;
        //PlugInitRecord.GetClientInfo := @_GetClientInfo;
        //PlugInitRecord.ZLibEncodeBuffer := @_ZLibEncodeBuffer;
        //PlugInitRecord.ZLibDecodeBuffer := @_ZLibDecodeBuffer;

        g_rgpDoInit(@PlugInitRecord, False);
      end;

      g_rgpDoUninit := GetProcAddress(g_RunGatePlugDllHandle, 'Uninit');
      g_rgpStartContext := GetProcAddress(g_RunGatePlugDllHandle, 'ClientStart');
      g_rgpEndContext := GetProcAddress(g_RunGatePlugDllHandle, 'ClientEnd');
      g_rgpRecvPacket := GetProcAddress(g_RunGatePlugDllHandle, 'ClientRecvPacket');
      g_rgpShowConfigForm := GetProcAddress(g_RunGatePlugDllHandle, 'ShowConfigForm');
    end;

    if LoadClientAntiPlugDll then
    begin
      AddMainLogMsg('反外挂模块加载完成', 0);
    end;

    grpVerifyCode.Visible := True;
    btnSettingOK.Left := grpVerifyCode.Left + grpVerifyCode.Width - btnSettingOK.Width;
    btnSettingOK.Top := grpVerifyCode.Top + grpVerifyCode.Height + 5;

    IniFile := TIniFile.Create(g_sIniFileName);
    try
      g_boOpenVerifyCode := IniFile.ReadBool(GateClass, 'OpenVerifyCode', g_boOpenVerifyCode);
      g_nVerifyCodeErrCount := IniFile.ReadInteger(GateClass, 'VerifyCodeErrCount', g_nVerifyCodeErrCount);
      g_nVerifyCodeRefreshCount := IniFile.ReadInteger(GateClass, 'VerifyCodeRefreshCount', g_nVerifyCodeRefreshCount);
      g_nVerifyCodeWaitTime := IniFile.ReadInteger(GateClass, 'VerifyCodeWaitTime', g_nVerifyCodeWaitTime);
      g_dwVerifyCodeInterval1 := IniFile.ReadInteger(GateClass, 'VerifyCodeInterval1', g_dwVerifyCodeInterval1);
      g_dwVerifyCodeInterval2 := IniFile.ReadInteger(GateClass, 'VerifyCodeInterval2', g_dwVerifyCodeInterval2);
      g_dwVerifySuccessAddInterval := IniFile.ReadInteger(GateClass, 'VerifySuccessAddInterval', g_dwVerifySuccessAddInterval);
      g_boVerifyFailTriggerScript := IniFile.ReadBool(GateClass, 'VerifyFailTriggerScript', g_boVerifyFailTriggerScript);
      g_boVerifyFailLoginVerify := IniFile.ReadBool(GateClass, 'VerifyFailLoginVerify', g_boVerifyFailLoginVerify);
      g_boVerifyCodeExcludeMap := IniFile.ReadBool(GateClass, 'VerifyCodeExcludeMap', g_boVerifyCodeExcludeMap);

      g_boAutoLoadNoVerifyChrList := IniFile.ReadBool(GateClass, 'AutoLoadNoVerifyChrList', g_boAutoLoadNoVerifyChrList);
      g_sLoadNoVerifyChrListFile := IniFile.ReadString(GateClass, 'LoadNoVerifyChrListFile', g_sLoadNoVerifyChrListFile);
      g_nAutoLoadNoVerifyChrListInterval := IniFile.ReadInteger(GateClass, 'AutoLoadNoVerifyChrListInterval', g_nAutoLoadNoVerifyChrListInterval);
      chkVerifyCode.Checked := g_boOpenVerifyCode;
      seVerifyCodeErrCount.Value := g_nVerifyCodeErrCount;
      seVerifyCodeRefreshCount.Value := g_nVerifyCodeRefreshCount;
      seVerifyCodeWaitTime.Value := g_nVerifyCodeWaitTime;
      seVerifyCodeInterval1.Value := g_dwVerifyCodeInterval1;
      seVerifyCodeInterval2.Value := g_dwVerifyCodeInterval2;
      seVerifySuccessAddInterval.Value := g_dwVerifySuccessAddInterval;
      chkVerifyFailTriggerScript.Checked := g_boVerifyFailTriggerScript;
      chkVerifyFailLoginVerify.Checked := g_boVerifyFailLoginVerify;
      chkVerifyCodeExcludeMap.Checked := g_boVerifyCodeExcludeMap;

      if FileExists(g_sVerifyCodeExcludeMapFileName) then
      begin
        g_VerifyCodeMapList.LoadFromFile(g_sVerifyCodeExcludeMapFileName);
      end;
      lstVerifyCodeExcludeMap.Items.Text := g_VerifyCodeMapList.Text;

      chkAutoLoadNoVerifyChrList.Checked := g_boAutoLoadNoVerifyChrList;
      edtLoadNoVerifyChrListFile.Text := g_sLoadNoVerifyChrListFile;
      seAutoLoadNoVerifyChrListInterval.Value := g_nAutoLoadNoVerifyChrListInterval;
    finally
      IniFile.Free;
    end;
  {$IFEND}
{$IFEND}

  btnSettingOK.Enabled := False;
end;

procedure TFrmMain.ShowAbout;
var
  sAppCaption: string;
  sProgrammer: string;
  sWebSiteUrl: string;
{$IF NEED_REGISTER = 1}
//  RetStatus, ExtendedInfo: Integer;
//  ExpDate: _SYSTEMTIME;
//  HWID1, HWID2: array[0..100] of Char;
//  sHWID1, sHWID2: string;
{$IFEND}
begin
//{$I CodeReplace_Start.inc}
  AddMainLogMsg('──────────────────────────────────────', 0, False);

  sAppCaption := '游戏运行网关V1.0';
//  sProgrammer := '程序制作：BmM2';
//  sWebSiteUrl := '程序网站：www.BmM2.com';

  AddMainLogMsg(sAppCaption, 0);
//  AddMainLogMsg(sProgrammer, 0);
//  AddMainLogMsg(sWebSiteUrl, 0);
  AddMainLogMsg('更新日期：' + g_sUpdateTime, 0);

{$IF NEED_REGISTER = 1}
  {$IF REGISTER_TEST <> 0}
    AddMainLogMsg('', 0, False);
    AddMainLogMsg('', 0, False);
    AddMainLogMsg('           ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★', 0, False);
    AddMainLogMsg('           ★                                                                                    ★', 0, False);
    AddMainLogMsg('           ★                          这个是注册测试版本，请勿发布                              ★', 0, False);
    AddMainLogMsg('           ★                                                                                    ★', 0, False);
    AddMainLogMsg('           ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★', 0, False);
    AddMainLogMsg('', 0, False);
    AddMainLogMsg('', 0, False);
  {$IFEND}
//  RetStatus := WLRegGetStatus(ExtendedInfo);
//  if RetStatus = wlIsRegistered then
//  begin
//    WLHardwareGetID(HWID1);
//    WLRegGetLicenseHardwareID(HWID2);
//
//    sHWID1 := StrPas(HWID1);
//    sHWID2 := StrPas(HWID2);
//
//    if SameText(sHWID1, sHWID2) then
//    begin
//      WLRegExpirationDate(ExpDate);
//      AddMainLogMsg('授权到期：' + FormatDateTime('yyyy-mm-dd', SystemTimeToDateTime(ExpDate)), 0);
//    end
//    else
//    begin
//      AddMainLogMsg('无效的授权文件；机器码：' + sHWID1, 0);
//    end;
//  end
//  else if RetStatus = wlIsTrial then
//  begin
//    WLHardwareGetID(HWID1);
//    AddMainLogMsg('机 器 码：' + StrPas(HWID1), 0);
//  end
//  else
//  begin
//    WLHardwareGetID(HWID1);
//    AddMainLogMsg('授权到期或无效授权；机器码：' + StrPas(HWID1), 0);
//  end;
{$IFEND}

  AddMainLogMsg('──────────────────────────────────────', 0, False);

//{$I CodeReplace_end.inc}
end;

procedure TFrmMain.tmrStartTimer(Sender: TObject);
//{$IF NEED_REGISTER = 1}
//var
//  LicenseDataInfo: TLicenseDataInfo;
//  Hash, EncKey: DWORD;
//  SrvMsgClientFlag: TServerMessageClientFlag;
//  I, Len, ExtendedInfo: Integer;
//
//  HWID1, HWID2: array[0..100] of Char;
//  sHWID1, sHWID2: string;
//  UserName: array[0..100] of Char;
//  Organization: array[0..100] of Char;
//  CustomData: array[0..102400] of Char;
//
//  MachineID: array[0..7] of DWORD;
//
//  IsOK: Boolean;
//  C1, C2: Char;
//  S, TempStr,Password: string;
//{$IFEND}
begin
  tmrStart.Enabled := False;
  if FRunGateManager.IsStart then
  begin
    StopServices;
    g_boClose := True;
    Close;
  end
  else
  begin
{$IF NEED_REGISTER = 1}
//  {$I Registered_Start.inc}
//    if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
//    begin
//      FillChar(UserName, SizeOf(UserName), 0);
//      FillChar(Organization, SizeOf(Organization), 0);
//      FillChar(CustomData, SizeOf(CustomData), 0);
//
//      WLHardwareGetID(HWID1);
//      WLRegGetLicenseHardwareID(HWID2);
//
//      sHWID1 := StrPas(HWID1);
//      sHWID2 := StrPas(HWID2);
//
//      if SameText(sHWID1, sHWID2) then
//      begin
//        WLRegGetLicenseInfo(UserName, Organization, CustomData);
//        S := StrPas(CustomData);
//
//        if (Length(S) > 0) and (Length(S) mod 2 = 0) then
//        begin
//          Len := Length(S) div 2;
//          SetLength(TempStr, Len);
//
//          IsOK := True;
//          if Len <> SizeOf(LicenseDataInfo) then
//            IsOK := False
//          else
//          begin
//            for I := 1 to Len do
//            begin
//              C1 := S[I * 2 - 1];
//              C2 := S[I * 2];
//              if (C1 in ['0'..'9', 'a'..'f', 'A'..'F']) and (C2 in ['0'..'9', 'a'..'f', 'A'..'F']) then
//              begin
//                TempStr[I] := Chr(StrToInt('$' + C1 + C2))
//              end
//              else
//              begin
//                IsOK := False;
//                Break;
//              end;
//            end;
//          end;
//
//          if IsOK then
//          begin
//    {$IF RungateLEG_IOCP = 1}
//            Password := '%$$^leg*&&^iocp&&rungate(*&&^1_)(%';
//
//    {$ELSEIF RungateLEG_IOCP = 2}
//            Password := '%$!^leg*&&^iocp2&&rungate(*&&^2_)(%';
//
//    {$ELSEIF RungateLEG_IOCP = 3}
//            Password := '(*&*&~@@#32~@*(*#($(*@$)%#@&%(())*&';
//
//    {$ELSEIF RungateLEG_IOCP = 4}
//            Password := '%**!^leg*&&^iocp&&rungat22e(*&&^1_)(%';
//
//    {$ELSEIF RungateLEG_IOCP = 5}
//            Password := '<>(**&^^4)(*&^)_**&^%%^**())*&^$%#$&&(?/';
//
//    {$IFEND}
//
//            WLBufferDecrypt(PChar(TempStr), Length(TempStr), PChar(Password));
//            Move(TempStr[1], LicenseDataInfo, SizeOf(LicenseDataInfo));
//
//            for I := 0 to 7 do
//            begin
//              MachineID[I] := StrToIntDef('$' + Copy(HWID1, I * 5 + 1, 4), 0);
//            end;
//
//            if LicenseDataInfo.SrvMsgClientFlagCRC_Enc = BufferCrc(@LicenseDataInfo.SrvMsgClientFlag, SizeOf(TServerMessageClientFlag)) then
//            begin
//              Hash := 0;
//              for I := Low(LicenseDataInfo.RandomCode1Arr) to High(LicenseDataInfo.RandomCode1Arr) do
//              begin
//                Hash := Hash xor (not ((Hash shl 8) xor LicenseDataInfo.RandomCode1Arr[I] xor (Hash shr 3)));
//              end;
//
//              EncKey := Hash and LicenseDataInfo.RandomCode1Arr[2] or LicenseDataInfo.RandomCode1Arr[5];
//              EncKey := EncKey and (LicenseDataInfo.RandomCode1Arr[3] shr 3);
//              EncKey := EncKey or LicenseDataInfo.RandomCode1Arr[4];
//              EncKey := EncKey or LicenseDataInfo.RandomCode1Arr[5];
//              EncKey := EncKey xor LicenseDataInfo.RandomCode1Arr[8];
//              EncKey := (EncKey shr 2) or LicenseDataInfo.RandomCode1Arr[12];
//              EncKey := EncKey and (LicenseDataInfo.RandomCode1Arr[3] shr 5);
//              EncKey := EncKey or (LicenseDataInfo.RandomCode1Arr[15] shl 3);
//              EncKey := (EncKey and LicenseDataInfo.RandomCode1Arr[19]) shl 7;
//              EncKey := EncKey and LicenseDataInfo.RandomCode1Arr[18];
//              EncKey := (EncKey or LicenseDataInfo.RandomCode1Arr[9]) shr 3;
//              EncKey := EncKey and LicenseDataInfo.RandomCode1Arr[7];
//              EncKey := EncKey xor LicenseDataInfo.RandomCode1Arr[6];
//              EncKey := EncKey or (LicenseDataInfo.RandomCode1Arr[14] shl 8);
//              EncKey := EncKey and LicenseDataInfo.RandomCode1Arr[13];
//              EncKey := EncKey xor LicenseDataInfo.RandomCode1Arr[17];
//
//              DecryptDes_New(LicenseDataInfo.SrvMsgClientFlag, SrvMsgClientFlag, SizeOf(TServerMessageClientFlag), IntToStr((EncKey xor MachineID[2] xor MachineID[7]) shr 2));
//
//
//              SM_LOGON := SrvMsgClientFlag.smLogon; // 登录
//              SM_ABILITY := SrvMsgClientFlag.smAbility; // 属性
//              SM_WHISPER := SrvMsgClientFlag.smWhisper; // 私聊
//              SM_CHANGEMAP := SrvMsgClientFlag.smChangeMap; // 换地图
//              SM_EAT_FAIL := SrvMsgClientFlag.smEatFail; // 吃东西失败
//              SM_SENDNOTICE := SrvMsgClientFlag.smSendNotice;
//              SM_HEROEAT_FAIL := SrvMsgClientFlag.smHeroEatFail;
//
//
//              SM_MODULEMD5 := SrvMsgClientFlag.smModuleMD5; // 模块md5
//              SM_BLACKMODULEMD5 := SrvMsgClientFlag.smBlackModuleMd5; // 黑名单模块md5
//              SM_SENDCUSTOMMONSTERCONFIG := SrvMsgClientFlag.smSendCustomMonsterConfig; // 自定义怪物配置
//              SM_STDITEMLIST := SrvMsgClientFlag.smStdItemList; // 物品列表
//              SM_SENDITEMDESCLIST := SrvMsgClientFlag.smSendItemDescList; // 物品备注
//              SM_SENDTZITEMDESCLIST := SrvMsgClientFlag.smSendTZItemDescList; // 套装物品备注
//              SM_SENDFILTERITEMLIST := SrvMsgClientFlag.smSendFilterItemList; // 内挂捡取列表
//              SM_EFFECTIMAGELIST := SrvMsgClientFlag.smEffectImageList; // 特效文件列表
//              SM_SPECIALCMD := SrvMsgClientFlag.smSpecialCmd; // 特殊命令
//              SM_SENDCUSTOMMAGICCONFIG := SrvMsgClientFlag.smSendCustomMagicConfig; // 自定义技能配置
//              SM_SENDCUSTOMNPCCONFIG := SrvMsgClientFlag.smSendCustomNpcConfig;
//              SM_SENDITEMDESCTOPLIST := SrvMsgClientFlag.smSendItemDescTopList;
//
//              SM_PLUGFILE := SrvMsgClientFlag.smPlugFile; // 插件文件
//              SM_SERVERCONFIG := SrvMsgClientFlag.smServerConfig; // 服务器配置
//
//              SM_MODULEMD5_CACHE := SrvMsgClientFlag.smModuleMd5Cache; // 模块md5缓存
//              SM_SENDCUSTOMMONSTERCONFIG_CACHE := SrvMsgClientFlag.smSendCustomMonsterConfigCache; // 自定义怪物配置缓存
//              SM_STDITEMLIST_CACHE := SrvMsgClientFlag.smStdItemListCache; // 物品列表缓存
//              SM_SENDITEMDESCLIST_CACHE := SrvMsgClientFlag.smSendItemDescListCache; // 物品备注缓存
//              SM_SENDTZITEMDESCLIST_CACHE := SrvMsgClientFlag.smSendTZItemDescListCache; // 套装物品备注缓存
//              SM_SENDFILTERITEMLIST_CACHE := SrvMsgClientFlag.smSendFilterItemListCache; // 内挂捡取列表缓存
//              SM_EFFECTIMAGELIST_CACHE := SrvMsgClientFlag.smEffectImageListCache; // 特效文件列表缓存
//              SM_SPECIALCMD_CACHE := SrvMsgClientFlag.smSpecialCmdCache; // 特殊命令缓存
//              SM_SENDCUSTOMMAGICCONFIG_CACHE := SrvMsgClientFlag.smSendCustomMagicConfigCache; // 自定义技能配置缓存
//              SM_PLUGFILE_CACHE := SrvMsgClientFlag.smPlugFileCache; // 插件文件缓存
//              SM_SERVERCONFIG_CACHE := SrvMsgClientFlag.smServerConfigCache; // 服务器配置缓存
//              SM_SENDCUSTOMNPCCONFIG_CACHE := SrvMsgClientFlag.smSendCustomNpcConfigCache;
//              SM_SENDITEMDESCTOPLIST_CACHE := SrvMsgClientFlag.smSendItemDescTopListCache;
//
//              SM_PROCESSBLACKLIST := SrvMsgClientFlag.smProcessBlacklist; // 进程黑名单列表
//
//              g_nKeyLastDay1 := LicenseDataInfo.KeyLastDay xor LicenseDataInfo.RandomCode1Arr[12] xor $0B5F4B3E;
//
//            {$IF REGISTER_TEST <> 0}
//              AddMainLogMsg('*******************************到期时间' + FormatDateTime('yyyy-mm-dd', LicenseDataInfo.KeyLastDay xor LicenseDataInfo.RandomCode1Arr[12]), 0);
//              AddMainLogMsg('*******************************SM_PROCESSBLACKLIST: ' + IntToStr(SM_PROCESSBLACKLIST), 0);
//            {$IFEND}
//            end;
//          end;
//        end;
//      end;
//    end;
//  {$I Registered_end.inc}
{$IFEND}
    StartServices;
  end;
end;

procedure TFrmMain.mmiStartServerClick(Sender: TObject);
begin
  StartServices;
end;

procedure TFrmMain.mmiStopServerClick(Sender: TObject);
begin
  StopServices;
end;

procedure TFrmMain.tmrRefreshInfoTimer(Sender: TObject);
var
  TimeInterval, Remain: DWORD;
  Day, Hour, Min, Sec: Integer;
  SRunTime: string;
  I, Count: Integer;
  Item: TListItem;
  Context: TMirClientContext;
  I64: Int64;
  RunGate: TRunGate;
  RunGateObj: TObject;

  pmc: TProcessMemoryCounters;
  ErrorNum: Integer;

  List: TList;
begin
  if FRunGateManager = nil then Exit;

  ErrorNum := 0;
  try
    if lvRunGates.Items.Count = FRunGateManager.Count then
    begin
      for I := 0 to FRunGateManager.Count - 1 do
      begin
        RunGate := FRunGateManager.Items[I];
        RunGate.Run;

        //RunGate.OnlineUser.Lock;
        Count := RunGate.OnlineUser.Count;
        //RunGate.OnlineUser.UnLock;

        lvRunGates.Items[I].SubItems[1] := IntToStr(Count);
        lvRunGates.Items[I].SubItems[2] := IntToStr(RunGate.MaxOnlineUserCount);

        lvRunGates.Items[I].SubItems[3] := GetSizeString(RunGate.TcpServer.IocpCore.RealTimeDataSendRecvLog.RecvBytesSize);
        lvRunGates.Items[I].SubItems[4] := GetSizeString(RunGate.TcpServer.IocpCore.RealTimeDataSendRecvLog.SendBytesSize);
        RunGate.TcpServer.IocpCore.ClearRealTimeDataSendRecvLog;

        lvRunGates.Items[I].SubItems[5] := GetSizeString(RunGate.TcpServer.IocpCore.CumulativeDataSendRecvLog.RecvBytesSize);
        lvRunGates.Items[I].SubItems[6] := GetSizeString(RunGate.TcpServer.IocpCore.CumulativeDataSendRecvLog.SendBytesSize);

        if {$IF UseIocpClient = 0} RunGate.IsReady {$ELSE} RunGate.TcpClient.Active {$IFEND} then
          lvRunGates.Items[I].SubItems[7] := STR_LINK_OK
        else
          lvRunGates.Items[I].SubItems[7] := STR_LINK_FAIL;

        if {$IF UseIocpClient = 0} RunGate.IsStart {$ELSE} RunGate.TcpServer.Active {$IFEND} then
          lvRunGates.Items[I].SubItems[8] := STR_LISTEN_OK
        else
          lvRunGates.Items[I].SubItems[8] := STR_LISTEN_FAIL;
      end;
    end;

    ErrorNum := 1;
    // 刷新系统信息页面 chongchong 2017-04-30
    if pgcMain.ActivePage = tsSysInfo then
    begin
      TimeInterval := tick_diff(FServicesStartTime, MyGetTickCount);
      Day := TimeInterval div MSecsPerDay;
      Remain := TimeInterval mod MSecsPerDay;

      Hour := Remain div (MSecsPerSec * 60 * 60);
      Remain := Remain mod (MSecsPerSec * 60 * 60);

      Min := Remain div (MSecsPerSec * 60);
      Remain := Remain mod (MSecsPerSec * 60);
      Sec := Remain div (MSecsPerSec);

      SRunTime := '';
      if Day > 0 then
        SRunTime := Format('%d天%d时%d分%d秒', [Day, Hour, Min, Sec])
      else if Hour > 0 then
        SRunTime := Format('%d时%d分%d秒', [Hour, Min, Sec])
      else if Min > 0 then
        SRunTime := Format('%d分%d秒', [Min, Sec])
      else
        SRunTime := Format('%d秒', [Sec]);

      ErrorNum := 2;
      lblWorkThreadCount.Caption := IntToStr(FRunGateManager.WorkerThreadCount);
      lblServerRunTime.Caption := SRunTime;

      ErrorNum := 4;
      pmc.cb := SizeOf(TProcessMemoryCounters);
      ErrorNum := 5;
      if GetProcessMemoryInfo(GetCurrentProcess, @pmc, SizeOf(pmc)) then
        lblAppMemorySize.Caption := GetSizeString(pmc.WorkingSetSize)
      else
        lblAppMemorySize.Caption := '-';

      ErrorNum := 6;
      lblSendCount.Caption := IntToStr(FRunGateManager.SendBlockCount);
      ErrorNum := 7;
      I64 := FRunGateManager.SendBytesSize;
      ErrorNum := 8;
      lblSendBytesSize.Caption := Format('%-10s(%d Bytes)', [GetSizeString(I64), I64]);

      ErrorNum := 9;
      lblRecvCount.Caption := IntToStr(FRunGateManager.RecvBlockCount);
      ErrorNum := 10;
      I64 := FRunGateManager.RecvBytesSize;
      ErrorNum := 11;
      lblRecvBytesSize.Caption := Format('%-10s(%d Bytes)', [GetSizeString(I64), I64]);

      ErrorNum := 12;
      lblContextUseCount.Caption := IntToStr(TIocpClientContextPool.Instance.OnlineContextCount);
      lblContextMaxCount.Caption := IntToStr(TIocpClientContextPool.Instance.MaxOnlineContextCount);
      lblContextNoUseCount.Caption := IntToStr(TIocpClientContextPool.Instance.NoUseContextCount);

      lblIODataUseCount.Caption := IntToStr(TIODataPool.Instance.UseCount);
      lblIODataNoUseCount.Caption := IntToStr(TIODataPool.Instance.NoUseCount);

      lblIODataMaxUseCount.Caption := IntToStr(TIODataPool.Instance.MaxUseCount);
      lblIODataMaxMemCount.Caption := GetSizeString(TIODataPool.Instance.MaxAllocMemSize);
      lblIODataMemCount.Caption := GetSizeString(TIODataPool.Instance.AllocMemSize);
    end
    // 刷新在线用户列表 chongchong 2014-06-18
    else if pgcMain.ActivePage = tsOnline then
    begin
      ErrorNum := 13;
      if tick_diff(FLastRefrshOnlineUserTick, MyGetTickCount) >= 1000 then
      begin
        FLastRefrshOnlineUserTick := MyGetTickCount;

        ErrorNum := 14;
        lvOnLine.Items.BeginUpdate;
        List := TList.Create;
        try
          ErrorNum := 15;
          FRunGateManager.GetOnlineUser(List);

          ErrorNum := 16;
          for I := 0 to List.Count - 1 do
          begin
            if lvOnLine.Items.Count < I + 1 then
            begin
              Item := lvOnLine.Items.Add;
              Item.SubItems.Add('');
              Item.SubItems.Add('');
              Item.SubItems.Add('');
              Item.SubItems.Add('');
              Item.SubItems.Add('');
              Item.SubItems.Add('');
              Item.SubItems.Add('');
              Item.SubItems.Add('');
              Item.SubItems.Add('');
              Item.SubItems.Add('');
            end;

            ErrorNum := 17;
            Context := TMirClientContext(List.Items[I]);

            ErrorNum := 18;
            Item := lvOnLine.Items[I];

            ErrorNum := 19;
            Item.Data := Context;
            Item.Caption := IntToStr(Context.ContextID);
            Item.SubItems[0] := Context.RemoteAddr;
            Item.SubItems[1] := IntToStr(Context.Socket);
            Item.SubItems[2] := Context.sAccount;
            Item.SubItems[3] := Context.sChrName;
            Item.SubItems[4] := Context.sMachineID;
            Item.SubItems[5] := Context.sVersion;
            Item.SubItems[6] := GetSizeString(Context.RecvBytesSize);
            Item.SubItems[7] := Format('%d/%d', [GetAttackCountOfIP(Context.RemoteAddr), GetConnectCountOfIP(Context.RemoteAddr)]);

            RunGateObj := Context.GetRunGate;
            if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
            begin
              Item.SubItems[8] := IntToStr(TRunGate(RunGateObj).ID);
            end;

            if tick_diff(Context.ClientResponseFileTick, MyGetTickCount) <= 3000 then
            begin
              Item.SubItems[9] := Format('%d/%d', [Context.ClientResponseFileIndex, Context.ClientResponseFileCount]);
            end
            else
            begin
              Item.SubItems[9] := '';
            end;

          end;

          while List.Count < lvOnLine.Items.Count do
          begin
            ErrorNum := 20;
            lvOnLine.Items.Delete(lvOnLine.Items.Count - 1);
          end;
        finally
          List.Free;
          lvOnLine.Items.EndUpdate;
        end;
      end;
    end;
  except
    AddMainLogMsg('tmrRefreshInfoTimer Error, ErrorNum = ' + IntToStr(ErrorNum), 0);
  end;

  if tick_diff(g_dwFYReadDenyIPTick, MyGetTickCount) >= g_dwFYReadDenyIPTime * 1000 then
  begin
    g_dwFYReadDenyIPTick := MyGetTickCount;
    if ReadFYDenyIPListFile then
      AddMainLogMsg('重新载入过滤列表完成', 7);
  end;

  if tick_diff(g_dwFYReadPassIPTick, MyGetTickCount) >= g_dwFYReadPassIPTime * 1000 then
  begin
    g_dwFYReadPassIPTick := MyGetTickCount;
    if ReadFYPassIPListFile then
      AddMainLogMsg('重新载入绿色通道完成', 7);
  end;

  if tick_diff(g_dwFYReadDenyMACTick, MyGetTickCount) >= g_dwFYReadDenyMACTime * 1000 then
  begin
    g_dwFYReadDenyMACTick := MyGetTickCount;
    if ReadFYDenyMACListFile then
      AddMainLogMsg('重新载入机器码过滤列表完成', 7);
  end;

  if (FRemoteBlackIPDown <> nil) and (Length(g_sFYDownDenyIPUrl) > 0) and FRemoteBlackIPDown.Sleeping and
    (tick_diff(g_dwFYDownDenyIPTick, MyGetTickCount) >= g_dwFYDownDenyIPTime * 60000) then
  begin
    g_dwFYDownDenyIPTick := MyGetTickCount;
    FRemoteBlackIPDown.DownUrl := g_sFYDownDenyIPUrl;
    FRemoteBlackIPDown.TriggerEvent;
  end;

  if (FRemoteWhiteIPDown <> nil) and (Length(g_sFYDownPassIPUrl) > 0) and FRemoteWhiteIPDown.Sleeping and
    (tick_diff(g_dwFYDownPassIPTick, MyGetTickCount) >= g_dwFYDownPassIPTime * 60000) then
  begin
    g_dwFYDownPassIPTick := MyGetTickCount;
    FRemoteWhiteIPDown.DownUrl := g_sFYDownPassIPUrl;
    FRemoteWhiteIPDown.TriggerEvent;
  end;

  if (FRemoteDenyMACDown <> nil) and (Length(g_sFYDownDenyMACUrl) > 0) and FRemoteDenyMACDown.Sleeping and
    (tick_diff(g_dwFYDownDenyMACTick, MyGetTickCount) >= g_dwFYDownDenyMACTime * 60000) then
  begin
    g_dwFYDownDenyMACTick := MyGetTickCount;
    FRemoteDenyMACDown.DownUrl := g_sFYDownDenyMACUrl;
    FRemoteDenyMACDown.TriggerEvent;
  end;

{$IF CLIENT_ANTIPLUG = 1}
  if (FAntiPlugDown <> nil) and (g_boAntiPlugAutoUpdateCheck) and FAntiPlugDown.Sleeping and
    (tick_diff(g_dwAntiPlugUpdateCheckTick, MyGetTickCount) >= g_wAntiPlugUpdateCheckInterval * 60000) then
  begin   //更新RunGatePlug.dll
    g_dwAntiPlugUpdateCheckTick := MyGetTickCount;
    FAntiPlugDown.ConfigUrl := g_sAntiPlugUpdateConfigUrl;
    FAntiPlugDown.TriggerEvent;
  end;
{$IFEND}

  if g_boAutoLoadNoVerifyChrList and (tick_diff(g_dwAutoLoadNoVerifyChrListTick, MyGetTickCount) >= g_nAutoLoadNoVerifyChrListInterval * 1000) then
  begin
    g_dwAutoLoadNoVerifyChrListTick := MyGetTickCount;
    if LoadNoVerifyChrList then
      AddMainLogMsg('重新载入验证码角色白名单完成', 7);
  end;
end;

{$IF VERSION_TYPE = 1}
var
  g_SendADText: string;

procedure TFrmMain.OnSendADTextTimer(Sender: TObject);
var
  DefMsg: TDefaultMessage;
  sSendText: string;
begin
  FTimerSendADText.Enabled := False;

  DefMsg := MakeDefaultMsg(SM_SYSMESSAGE, 0, MakeWord(250, 0), 0, 1);
  sSendText := g_SendADText;
  FRunGateManager.SendOnLineUserMsg(DefMsg, PChar(sSendText), Length(sSendText), True);

  //FTimerSendADText.Interval := 3600000 + Random(3600000);
  FTimerSendADText.Enabled := True;
end;
{$IFEND}

{$IF VERSION_TYPE <> 0}
var
  g_PlugDllHandle: THandle;
  g_ActivePluginFunc: procedure; stdcall;

procedure TFrmMain.DoInitCustomVersion;
var
  sTemp: string;
  mmiPlugs: TMenuItem;
  mmiPlugGxx: TMenuItem;
  IntGxxPluginFun: procedure(Code, Hwnd: Integer); stdcall;
begin
{$I VMProtectBegin.inc}

{$IF VERSION_TYPE = 1}
  sTemp := 'HNycWB\uL_ESWo]oOaUtMq]BQ`mIMRXkPRubV_]rL^yURAUFNA=nVrhqLpPuYRT' +
    'pX_M]IRyuVpMlWBu^Z_YNWqEfMSMTNCYdQAPlPqABHRycYcYTI?`sHSYiWSMfNRANVA=PZ^iIX' +
    'QXrY?ImOriUIb\mMBe>WPqiPP]RP@MvXoECNA@uLseUNOQ^MP]=IbY>Yp`rNRiqZOAPMPUrQrY' +
    'tIqYJVbUmPB`qW@MgNq]CPR]PHpijVOa]NcdsOoADIse>HCMnTbqbVQYiUbAVXr]JN@q@N_=tM' +
    'a@sUqItHqU@MqEMXPylZ@iqTpafUBEDLbQnLbAMGpybWaXlI?MuZPaaWc]LMPIfNrllUCIOPCE' +
    'GJ@PqMBMbWqUCW_UkU?U?O?ABNCEaI@MjPS]aXaMNQQECVBIMKOp';
  g_SendADText := DecryString_LF(sTemp);

  FTimerSendADText := TTimer.Create(Self);
  FTimerSendADText.OnTimer := OnSendADTextTimer;
  //FTimerSendADText.Interval := 3600000 + Random(3600000);
  FTimerSendADText.Enabled := True;
  sTemp := ExtractFilePath(Application.ExeName) + 'Gxxplugin.dll';
  if FileExists(sTemp) then
  begin
    mmiPlugs := TMenuItem.Create(Self);
    mmiPlugs.Caption := '插件';
    mmMain.Items.Add(mmiPlugs);

    mmiPlugGxx := TMenuItem.Create(Self);
    mmiPlugGxx.Caption := 'Gxx插件';
    mmiPlugGxx.OnClick := OnmmiPlugGxxClick;
    mmiPlugs.Add(mmiPlugGxx);

    if g_PlugDllHandle = 0 then g_PlugDllHandle := LoadLibrary(PChar(sTemp));
    if g_PlugDllHandle = 0 then Exit;
    @IntGxxPluginFun := GetProcAddress(g_PlugDllHandle, 'IntGeePlugin');
    IntGxxPluginFun(88888888, Handle);

    @g_ActivePluginFunc := GetProcAddress(g_PlugDllHandle, 'ActivePlugin');
  end;

{$ELSEIF VERSION_TYPE = 2}
  sTemp := ExtractFilePath(Application.ExeName) + 'plugin.dll';
  if FileExists(sTemp) then
  begin
    mmiPlugs := TMenuItem.Create(Self);
    mmiPlugs.Caption := '插件';
    mmMain.Items.Add(mmiPlugs);

    mmiPlugGxx := TMenuItem.Create(Self);
    mmiPlugGxx.Caption := 'Gxx插件';
    mmiPlugGxx.OnClick := OnmmiPlugGxxClick;
    mmiPlugs.Add(mmiPlugGxx);

    if g_PlugDllHandle = 0 then g_PlugDllHandle := LoadLibrary(PChar(sTemp));
    if g_PlugDllHandle = 0 then Exit;
    @IntGxxPluginFun := GetProcAddress(g_PlugDllHandle, 'IntGeePlugin');
    IntGxxPluginFun(13579024, Handle);

    @g_ActivePluginFunc := GetProcAddress(g_PlugDllHandle, 'ActivePlugin');
  end;
{$IFEND}

{$I VMProtectEnd.inc}
end;

procedure TFrmMain.DoFinalCustomVersion;
begin
  if g_PlugDllHandle <> 0 then FreeLibrary(g_PlugDllHandle);
end;

procedure TFrmMain.DllCheckMessage(var Message: TMessage);
begin
  Message.Result := Message.LParam xor $2013;
end;

procedure TFrmMain.OnmmiPlugGxxClick(Sender: TObject);
begin
  if @g_ActivePluginFunc <> nil then g_ActivePluginFunc;
end;

{$IFEND}

{$IF CLIENT_ANTIPLUG = 1}

function TFrmMain.ReloadAntiPlug: Boolean;
{$IF NEED_REGISTER = 1}
var
  ExtendedInfo: Integer;
  HWID1, HWID2: array[0..100] of Char;
  sHWID1, sHWID2: string;
{$IFEND}
begin
//{$I Registered_Start.inc}
{$IF NEED_REGISTER = 1}
  Result := False;
//  if (WLRegGetStatus(ExtendedInfo) = wlIsRegistered) then
//  begin
//    WLHardwareGetID(HWID1);
//    WLRegGetLicenseHardwareID(HWID2);
//
//    sHWID1 := StrPas(HWID1);
//    sHWID2 := StrPas(HWID2);
//
//    if SameText(sHWID1, sHWID2) then
//    begin
      Result := LoadClientAntiPlugDll;
//    end;
//  end;
{$ELSE}
  Result := LoadClientAntiPlugDll;
{$IFEND}

  if Result then
  begin
    AddMainLogMsg('反外挂模块加载完成', 0);
  end;
//{$I Registered_End.inc}
end;

procedure TFrmMain.OnmmiAntiPlugUpdateSettingClick(Sender: TObject);
begin
  ShowFrmAntiPlugUpdateSetting;
end;

procedure TFrmMain.OnAntiPlugDownloadFinished(Sender: TObject; IsUpdateRungateDll, IsUpdateClientDll: Boolean);
var
  FileName: string;
  UnloadOK: Boolean;
begin
  if g_RunGatePlugDllHandle <> 0 then
  begin
    UnloadOK := False;

    if IsUpdateRungateDll then
    begin
      if Assigned(FAntiPlugDown) then
      begin
        UnloadOK := DoUnloadRungatePlugDll;

        FileName := ExtractFilePath(ParamStr(0)) + g_sRunGatePlusDllName;
        try
          FAntiPlugDown.SaveGxxRunGateDllStreamToFile(FileName);
        except
          on E: Exception do
          begin
            AddMainLogMsg('保存网关插件异常; ' + E.Message, 0);
          end;
        end;

        if UnloadOK then
        begin
          DoReloadRungatePlugDll;
        end;
      end;
    end;

    if IsUpdateClientDll then
    begin
      // 只更新了客户端插件，先通知已经加载的插件卸载 2020-01-08
      if (not UnloadOK) and (CheckClientAntiPlugDllChanged) then
      begin
        SendAllContextUnloadPlugin(False);
      end;

      ReloadAntiPlug;
    end;
  end
  else
  begin
    if IsUpdateRungateDll and Assigned(FAntiPlugDown) then
    begin
      FileName := ExtractFilePath(ParamStr(0)) + g_sRunGatePlusDllName;
      try
        FAntiPlugDown.SaveGxxRunGateDllStreamToFile(FileName);
      except
        on E: Exception do
        begin
          AddMainLogMsg('保存网关插件异常; ' + E.Message, 0);
        end;
      end;
    end;
  end;
end;

procedure TFrmMain.SendAllContextUnloadPlugin(IsWaitLoad: Boolean);
var
  I: Integer;
  List: TList;
  Context: TMirClientContext;
begin
  List := TList.Create;
  try
    FRunGateManager.GetOnlineUser(List);

    for I := 0 to List.Count - 1 do
    begin
      Context := TMirClientContext(List.Items[I]);

      if (Context.dwClientAntiPlugVersion = g_ClientAntiPlugVersion) and
       Context.boSendLoadAntiPlug and Context.boSendLoadAntiPlugFinished and
       Context.boRecvLoadAntiPlug then
      begin
        Context.SendAntiPlugStreamUnload(IsWaitLoad);
      end
      else if IsWaitLoad and (Context.dwClientAntiPlugVersion = g_ClientAntiPlugVersion) then
      begin
        Context.boWaitLoadAntiPlug := True;
        Context.dwWaitLoadAntiPlugTick := MyGetTickCount;
      end;
    end;
  finally
    List.Free;
  end;
end;

function TFrmMain.DoReloadRungatePlugDll: Boolean;
var
  sDllName: string;
  PlugInitRecord: TPlugInitRecord;
begin
{$I VMProtectBegin.inc}
  Result := False;
  EnterCriticalSection(g_CSRunGatePlug);
  try
    if g_RunGatePlugDllHandle = 0 then
    begin
      sDllName := ExtractFilePath(Application.ExeName) + g_sRunGatePlusDllName;
      if FileExists(sDllName) then
      begin
        g_RunGatePlugDllHandle := LoadLibrary(PChar(sDllName));

        if g_RunGatePlugDllHandle <> 0 then
        begin
          g_rgpDoInit := GetProcAddress(g_RunGatePlugDllHandle, 'Init');

          if Assigned(g_rgpDoInit) then
          begin
            FillChar(PlugInitRecord, SizeOf(PlugInitRecord), 0);

            PlugInitRecord.MainFormHandle := Handle;
            PlugInitRecord.GetClientInfo := @_GetClientInfo;
            PlugInitRecord.AddMainLogMsg := @_AddMainLogMsg;
            PlugInitRecord.SendDataToClient := @_SendDataToClient;
            PlugInitRecord.EncodeBuffer := @_EncodeBuffer;
            PlugInitRecord.DecodeBuffer := @_DecodeBuffer;
            PlugInitRecord.CloseClient := @_CloseClient;
            PlugInitRecord.LockClient := @_LockClient;
            PlugInitRecord.SetClientPlugLoad := @_SetClientPlugLoad;

            SendAllContextUnloadPlugin(True);

            g_rgpDoInit(@PlugInitRecord, True);

            Result := True;
          end;

          g_rgpDoUninit := GetProcAddress(g_RunGatePlugDllHandle, 'Uninit');
          g_rgpStartContext := GetProcAddress(g_RunGatePlugDllHandle, 'ClientStart');
          g_rgpEndContext := GetProcAddress(g_RunGatePlugDllHandle, 'ClientEnd');
          g_rgpRecvPacket := GetProcAddress(g_RunGatePlugDllHandle, 'ClientRecvPacket');
          g_rgpShowConfigForm := GetProcAddress(g_RunGatePlugDllHandle, 'ShowConfigForm');
        end;
      end;
    end;
  finally
    LeaveCriticalSection(g_CSRunGatePlug);
  end;
{$I VMProtectEnd.inc}
end;

procedure TFrmMain.OnmmiReloadRunGatePlugClick(Sender: TObject);
var
  IsReload: Boolean;
begin
  IsReload := DoReloadRungatePlugDll;

  // 在重新加载前，网关插件已经存在，并且插件发生了改变，先通知客户端卸载 2020-01-08
  if (not IsReload) and (g_RunGatePlugDllHandle <> 0) and (CheckClientAntiPlugDllChanged) then
  begin
    SendAllContextUnloadPlugin(False);
  end;

  ReloadAntiPlug;
end;

function TFrmMain.DoUnloadRungatePlugDll: Boolean;
var
  OldDoUninit: TRunGatePluginDoUninit;
  TempHandle: THandle;
begin
{$I VMProtectBegin.inc}
  Result := False;
  EnterCriticalSection(g_CSRunGatePlug);
  try
    if g_RunGatePlugDllHandle <> 0 then
    begin
      try
        OldDoUninit := g_rgpDoUninit;

        g_rgpDoInit := nil;
        g_rgpDoUninit := nil;
        g_rgpStartContext := nil;
        g_rgpEndContext := nil;
        g_rgpRecvPacket := nil;

        SendAllContextUnloadPlugin(False);

        OldDoUninit();

        TempHandle := g_RunGatePlugDllHandle;
        g_RunGatePlugDllHandle := 0;
        FreeLibrary(TempHandle);

        Result := True;
      except
        on E: Exception do
        begin
          AddMainLogMsg('卸载插件异常; ' + E.Message, 0);
        end;
      end;
    end;
  finally
    LeaveCriticalSection(g_CSRunGatePlug);
  end;
{$I VMProtectEnd.inc}
end;

procedure TFrmMain.OnmmiUnloadRunGatePlugClick(Sender: TObject);
begin
  DoUnloadRungatePlugDll;
end;

procedure TFrmMain.OnmmiRunGatePlugSettingClick(Sender: TObject);
begin
  if Assigned(g_rgpShowConfigForm) then
    g_rgpShowConfigForm();
end;

{$IFEND}

procedure TFrmMain.btnSettingOKClick(Sender: TObject);
var
  StrGateIP, StrSvrIP, StrTitleName: string;
  IntGatePort, IntSvrPort, IntDBPort: Integer;
  IniFile: TIniFile;
begin
  StrGateIP := Trim(edtGateIPaddr.Text);
  IntGatePort := StrToIntDef(Trim(edtGatePort.Text), -1);
  StrSvrIP := Trim(edtServerIPaddr.Text);
  IntSvrPort := StrToIntDef(Trim(edtServerPort.Text), -1);
  StrTitleName := Trim(edtTitleName.Text);
  IntDBPort := StrToIntDef(Trim(edtDBPort.Text), -1);

  if not IsIPaddr(StrGateIP) then
  begin
    ErrMessage('网关地址设置错误！');
    edtGateIPaddr.SetFocus;
    Exit;
  end;

  if (IntGatePort < 0) or (IntGatePort > 65535) then
  begin
    ErrMessage('网关端口设置错误！');
    edtGatePort.SetFocus;
    Exit;
  end;

  if not IsIPaddr(StrSvrIP) then
  begin
    ErrMessage('服务器地址设置错误！');
    edtServerIPaddr.SetFocus;
    Exit;
  end;

  if (IntSvrPort < 0) or (IntSvrPort > 65535) then
  begin
    ErrMessage('服务器端口设置错误！');
    edtServerPort.SetFocus;
    Exit;
  end;

  if Length(StrTitleName) = 0 then
  begin
    ErrMessage('应用程序标题不能为空！');
    edtTitleName.SetFocus;
    Exit;
  end;

  g_sGateAddr := StrGateIP;
  g_wdGatePort := IntGatePort;
  g_sServerAddr := StrSvrIP;
  g_wdServerPort := IntSvrPort;
  g_wdDBPort := IntDBPort;
  g_boCheckClientPassword := chkClientPassword.Checked;
  g_sClientPassWord := edtClientPassword.Text;
  if Length(g_sClientPassWord) = 0 then g_sClientPassWord := 'BmM2';

  g_sTitleName := StrTitleName;
  g_btShowLogLevel := cbbShowLogLevel.ItemIndex;
  g_nRecvAntiPlugHeartbeatTimeOutTime := seRecvAntiPlugHeartbeatTimeOutTime.Value;
  g_nAntiPlugStreamSendSpeed := seAntiPlugStreamSendSpeed.Value;
  g_boLogoutNoResendAntiplugStream := chkLogoutNoResendAntiplugStream.Checked;
  g_boAntiplugAllLog := chkAntiplugAllLog.Checked;

  // 重启生效，这里不加
  //g_nAntiPlugStreamSendBlockSize := cbbAntiPlugStreamSendBlockSize.ItemIndex;

  g_boMinimize := chkMinimize.Checked;

  g_dwCheckServerTimeOutTime := seCheckServerTimeOutTime.Value;
  g_dwClientAccumulateMaxSize := seClientAccumulateMaxSize.Value;

  g_boOpenVerifyCode := chkVerifyCode.Checked;
  g_nVerifyCodeErrCount := seVerifyCodeErrCount.Value;
  g_nVerifyCodeRefreshCount := seVerifyCodeRefreshCount.Value;
  g_nVerifyCodeWaitTime := seVerifyCodeWaitTime.Value;
  g_dwVerifyCodeInterval1 := seVerifyCodeInterval1.Value;
  g_dwVerifyCodeInterval2 := seVerifyCodeInterval2.Value;
  g_dwVerifySuccessAddInterval := seVerifySuccessAddInterval.Value;
  g_boVerifyFailTriggerScript := chkVerifyFailTriggerScript.Checked;
  g_boVerifyFailLoginVerify := chkVerifyFailLoginVerify.Checked;
  g_boVerifyCodeExcludeMap := chkVerifyCodeExcludeMap.Checked;

  g_VerifyCodeMapList.Lock;
  try
    g_VerifyCodeMapList.Text := lstVerifyCodeExcludeMap.Items.Text;
  finally
    g_VerifyCodeMapList.UnLock;
  end;
  g_VerifyCodeMapList.SaveToFile(g_sVerifyCodeExcludeMapFileName);

  g_boAutoLoadNoVerifyChrList := chkAutoLoadNoVerifyChrList.Checked;
  g_sLoadNoVerifyChrListFile := edtLoadNoVerifyChrListFile.Text;
  g_nAutoLoadNoVerifyChrListInterval := seAutoLoadNoVerifyChrListInterval.Value;

  g_boOneMACLimitePlayer := chkOneMACLimitePlayer.Checked;
  g_nOneMACLimitePlayerCount := seOneMACLimitePlayer.Value;

  g_nClientLogoutDelay := seClientLogoutDelay.Value;
  g_nClientCloseDelay := seClientCloseDelay.Value;

  g_boDelayCloseDisableMove := chkDelayCloseDisableMove.Checked;
  g_boDelayCloseDisableSpell := chkDelayCloseDisableSpell.Checked;
  g_boDelayCloseDisableAttack := chkDelayCloseDisableAttack.Checked;
  g_boDelayCloseDisableUseItem := chkDelayCloseDisableUseItem.Checked;

  g_boBreakClientLogoutHint := chkBreakClientLogoutHint.Checked;
  g_sBreakClientLogoutHint := edtBreakClientLogoutHint.Text;
  g_boBreakClientCloseHint := chkBreakClientCloseHint.Checked;
  g_sBreakClientCloseHint := edtBreakClientCloseHint.Text;


  IniFile := TIniFile.Create(g_sIniFileName);
  try
    IniFile.WriteString(GateClass, 'GateAddr', g_sGateAddr);
    IniFile.WriteInteger(GateClass, 'GatePort', g_wdGatePort);
    IniFile.WriteString(GateClass, 'Server1', g_sServerAddr);
    IniFile.WriteInteger(GateClass, 'ServerPort', g_wdServerPort);
    IniFile.WriteBool(GateClass, 'CheckClientPassword', g_boCheckClientPassword);
    IniFile.WriteString(GateClass, 'ClientPassWord', g_sClientPassWord);

    IniFile.WriteString(GateClass, 'Title', g_sTitleName);

    IniFile.WriteInteger(GateClass, 'ShowLogLevel', g_btShowLogLevel);
    IniFile.WriteBool(GateClass, 'Minimize', g_boMinimize);                // 2009-5-26 Micro

    IniFile.WriteInteger(GateClass, 'CheckM2ServerTimeOut', g_dwCheckServerTimeOutTime);
    IniFile.WriteInteger(GateClass, 'ClientSendBlockSize', seClientSendBlockSize.Value);
    IniFile.WriteInteger(GateClass, 'MaxPreallocatedMemorySize', sePreAllocatedCount.Value);
    IniFile.WriteInteger(GateClass, 'ClientAccumulateMaxSize', g_dwClientAccumulateMaxSize);

    IniFile.WriteInteger(GateClass, 'DBPort', g_wdDBPort);

    IniFile.WriteBool(GateClass, 'LogoutNoResendAntiplugStream', g_boLogoutNoResendAntiplugStream);
    IniFile.WriteBool(GateClass, 'AntiplugAllLog', g_boAntiplugAllLog);

    IniFile.WriteInteger(GateClass, 'RecvAntiPlugHeartbeatTimeOutTime', g_nRecvAntiPlugHeartbeatTimeOutTime);
    IniFile.WriteInteger(GateClass, 'AntiPlugStreamSendSpeed', g_nAntiPlugStreamSendSpeed);
    IniFile.WriteInteger(GateClass, 'AntiPlugStreamSendBlockSize', cbbAntiPlugStreamSendBlockSize.ItemIndex);

    IniFile.WriteBool(GateClass, 'OpenVerifyCode', g_boOpenVerifyCode);
    IniFile.WriteInteger(GateClass, 'VerifyCodeErrCount', g_nVerifyCodeErrCount);
    IniFile.WriteInteger(GateClass, 'VerifyCodeRefreshCount', g_nVerifyCodeRefreshCount);
    IniFile.WriteInteger(GateClass, 'VerifyCodeWaitTime', g_nVerifyCodeWaitTime);
    IniFile.WriteInteger(GateClass, 'VerifyCodeInterval1', g_dwVerifyCodeInterval1);
    IniFile.WriteInteger(GateClass, 'VerifyCodeInterval2', g_dwVerifyCodeInterval2);
    IniFile.WriteInteger(GateClass, 'VerifySuccessAddInterval', g_dwVerifySuccessAddInterval);
    IniFile.WriteBool(GateClass, 'VerifyFailTriggerScript', g_boVerifyFailTriggerScript);
    IniFile.WriteBool(GateClass, 'VerifyFailLoginVerify', g_boVerifyFailLoginVerify);
    IniFile.WriteBool(GateClass, 'VerifyCodeExcludeMap', g_boVerifyCodeExcludeMap);

    IniFile.WriteBool(GateClass, 'AutoLoadNoVerifyChrList', g_boAutoLoadNoVerifyChrList);
    IniFile.WriteString(GateClass, 'LoadNoVerifyChrListFile', g_sLoadNoVerifyChrListFile);
    IniFile.WriteInteger(GateClass, 'AutoLoadNoVerifyChrListInterval', g_nAutoLoadNoVerifyChrListInterval);

    IniFile.WriteBool(GateClass, 'OneMACLimitePlayer', g_boOneMACLimitePlayer);
    IniFile.WriteInteger(GateClass, 'OneMACLimitePlayerCount', g_nOneMACLimitePlayerCount);

    IniFile.WriteInteger(GateClass, 'ClientLogoutDelay', g_nClientLogoutDelay);
    IniFile.WriteInteger(GateClass, 'ClientCloseDelay', g_nClientCloseDelay);

    IniFile.WriteBool(GateClass, 'DelayCloseDisableMove', g_boDelayCloseDisableMove);
    IniFile.WriteBool(GateClass, 'DelayCloseDisableSpell', g_boDelayCloseDisableSpell);
    IniFile.WriteBool(GateClass, 'DelayCloseDisableAttack', g_boDelayCloseDisableAttack);
    IniFile.WriteBool(GateClass, 'DelayCloseDisableUseItem', g_boDelayCloseDisableUseItem);

    IniFile.WriteBool(GateClass, 'ShowBreakClientLogoutHint', g_boBreakClientLogoutHint);
    IniFile.WriteString(GateClass, 'BreakClientLogoutHint', g_sBreakClientLogoutHint);
    IniFile.WriteBool(GateClass, 'ShowBreakClientCloseHint', g_boBreakClientCloseHint);
    IniFile.WriteString(GateClass, 'BreakClientCloseHint', g_sBreakClientCloseHint);
  finally
    IniFile.Free;
  end;

  btnSettingOK.Enabled := False;
end;

procedure TFrmMain.ErrMessage(ErrMsg: string);
begin
  Application.MessageBox(PChar(ErrMsg), '错误', MB_OK or MB_ICONERROR)
end;

function TFrmMain.ExtractSelfTimeDateStamp: TDateTime;
var
  tmpMM: TMemoryStream;
  tmpDosHeader: PImageDosHeader;
  tmpNtHeader: PImageNtHeaders;

  function UnixDateToDateTime(const USec: Longint): TDateTime;
  const
    UnixStartDate: TDateTime = 25569.0; // 1970/01/01
  begin
    Result := (USec / 86400) + UnixStartDate;
    Result := IncHour(Result, 8);
  end;

begin
  tmpMM := TMemoryStream.Create;

  tmpMM.LoadFromFile(ParamStr(0));
  tmpDosHeader := Pointer(tmpMM.Memory);
  tmpNtHeader := Pointer(NativeUInt(tmpDosHeader^._lfanew) + NativeUInt(tmpDosHeader));
  Result := UnixDateToDateTime(tmpNtHeader.FileHeader.TimeDateStamp);
  tmpMM.Free;
end;

procedure TFrmMain.edtGateIPaddrChange(Sender: TObject);
begin
  btnSettingOK.Enabled := True;
end;

procedure TFrmMain.mmiWordFilterClick(Sender: TObject);
begin
  ShowFrmMessageFilter(Self);
end;

procedure TFrmMain.mmiSafeSettingClick(Sender: TObject);
begin
  ShowFrmSafeFilter(Self);
end;

procedure TFrmMain.mmiWaiGuaClick(Sender: TObject);
begin
  ShowFrmGameSpeed(Self, FRunGateManager);
end;

procedure TFrmMain.MyMessage(var MsgData: TWmCopyData);
var
  sData: string;
  wIdent: Word;
begin
  wIdent := HiWord(MsgData.From);
  sData := StrPas(MsgData.CopyDataStruct^.lpData);
  case wIdent of
    GS_QUIT:
      begin
        if FRunGateManager.IsStart then
        begin
          tmrStart.Enabled := True;
        end
        else
        begin
          g_boClose := True;
          Close();
        end;
      end;
    1: ;
    2: ;
    3: ;
  end;
end;

procedure TFrmMain.tmrRefreshLogTimer(Sender: TObject);
var
  I: Integer;
  boWriteLog: Boolean;
  LogFile: TextFile;
(*
{$IF NEED_REGISTER = 1}
  ExtendedInfo: Integer;
  ExpDate: _SYSTEMTIME;
  ExpDateTime: TDateTime;
  List: TList;
  MirContext: TMirClientContext;
{$IFEND}
*)
begin
  if g_MainLogStrings.Count > 0 then
  begin
    boWriteLog := True;
    try
      if not FileExists(FLogFileName) then
      begin
        AssignFile(LogFile, FLogFileName);
        Rewrite(LogFile);
      end
      else
      begin
        AssignFile(LogFile, FLogFileName);
        Append(LogFile);
      end;
    except
      boWriteLog := False;
      mmoMainLog.Lines.Add('保存日志信息出错！！！');
    end;

    g_MainLogStrings.Lock;
    try
      for I := 0 to g_MainLogStrings.Count - 1 do
      begin
        if mmoMainLog.Lines.Count >= 500 then mmoMainLog.Lines.Clear;
        mmoMainLog.Lines.Add(g_MainLogStrings[I]);

        if boWriteLog then
          Writeln(LogFile, g_MainLogStrings[I]);
      end;
      g_MainLogStrings.Clear;
    finally
      g_MainLogStrings.UnLock;
    end;

    if boWriteLog then CloseFile(LogFile);
  end;

  if g_IocpLogStrings.Count > 0 then
  begin
    g_IocpLogStrings.Lock;
    try
      for I := 0 to g_IocpLogStrings.Count - 1 do
      begin
        if mmoIocpLog.Lines.Count >= 500 then mmoIocpLog.Clear;
        mmoIocpLog.Lines.Add(g_IocpLogStrings[I]);
      end;
      g_IocpLogStrings.Clear;
    finally
      g_IocpLogStrings.UnLock;
    end;
  end;

(*
{$IF NEED_REGISTER = 1}
{$I Registered_Start.inc}
  // 暗桩：取baidu时间来和到期时间比较，如果baidu时间超过过期时间，则软件已过期，断客户端连接并将反外挂的配置清为0
  if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
  begin
    if (g_dtBaiduTime = 0) then
    begin
      if (tick_diff(g_dwBaiduTimeTick, MyGetTickCount) >= g_dwGetBaiduTimeTime) then
      begin
        g_dwBaiduTimeTick := MyGetTickCount;
        Randomize;
        g_dwGetBaiduTimeTime := 1800000 + Random(3600000);
        TGetBaiduTimeThread.Create(False);
      end;
    end
    else
    begin
      g_dtBaiduTime := 0;
      WLRegExpirationDate(ExpDate);
      ExpDateTime := SystemTimeToDateTime(ExpDate);

      if (Trunc(g_dtBaiduTime2) > Trunc(ExpDateTime)) then
      begin
        List := TList.Create;
        try
          TIocpClientContextPool.Instance.GetOnlineContextList(List);
          for I := List.Count - 1 downto 0 do
          begin
            MirContext := List.Items[I];
            if (MirContext.Socket <> INVALID_SOCKET) then
            begin
              MirContext.Close;
            end;
          end;
        finally
          List.Free;
        end;

        Randomize;
        FillChar(g_wHitIntervals, SizeOf(g_wHitIntervals), Random(High(Byte)));
        FillChar(g_wSpellIntervals, SizeOf(g_wSpellIntervals), Random(High(Byte)));
        FillChar(g_wWalkIntervals, SizeOf(g_wWalkIntervals), Random(High(Byte)));
        FillChar(g_wRunIntervals, SizeOf(g_wRunIntervals), Random(High(Byte)));
      end;
    end;
  end;
{$I Registered_end.inc}
{$IFEND}
*)
end;

procedure TFrmMain.mmoMainLogDblClick(Sender: TObject);
begin
  if MessageBox(Handle, '是否确定清除日志信息?', '提示信息', MB_YESNO or MB_ICONQUESTION) = IDYES then
  begin
    g_MainLogStrings.Lock;
    try
      mmoMainLog.Lines.Clear;
      g_MainLogStrings.Clear;
    finally
      g_MainLogStrings.UnLock;
    end;
  end;
end;

procedure TFrmMain.mmoIocpLogDblClick(Sender: TObject);
begin
  if MessageBox(Handle, '是否确定清除Iocp调试信息?', '提示信息', MB_YESNO or MB_ICONQUESTION) = IDYES then
  begin
    g_IocpLogStrings.Lock;
    try
      mmoIocpLog.Lines.Clear;
      g_IocpLogStrings.Clear;
    finally
      g_IocpLogStrings.UnLock;
    end;
  end;
end;

procedure TFrmMain.pmUserPopup(Sender: TObject);
begin
  mniKick.Enabled := lvOnLine.ItemIndex >= 0;
  mniAddToTempBlock.Enabled := mniKick.Enabled;
  mniAddToBlock.Enabled := mniKick.Enabled;
  mniAddToTempMacBlock.Enabled := mniKick.Enabled;
  mniAddToMacBlock.Enabled := mniKick.Enabled;
end;

procedure TFrmMain.mniKickClick(Sender: TObject);
var
  Item: TListItem;
  Context: TMirClientContext;
begin
  if lvOnLine.ItemIndex < 0 then Exit;
  Item := lvOnLine.Items[lvOnLine.ItemIndex];
  Context := Item.Data;
  if Context <> nil then
  begin
    Context.Close;
  end;
end;

procedure TFrmMain.mniAddToTempBlockClick(Sender: TObject);
var
  Item: TListItem;
  sMsg, sTitle, sIPaddr: string;
  Context: TIocpClientContext;
begin
  if lvOnLine.ItemIndex < 0 then Exit;
  Item := lvOnLine.Items[lvOnLine.ItemIndex];

  sMsg := '将此IP加入到过态过滤列表后，此IP建立的所有连接将被强行中断，是否继续？';
  sTitle := '确认信息 - ' + Item.SubItems[0];
  if Application.MessageBox(PChar(sMsg), PChar(sTitle), MB_OKCANCEL + MB_ICONQUESTION) <> IDOK then Exit;

  sIPaddr := Item.SubItems[0];
  AddTempBlockIP(sIPaddr);

  Context := TIocpClientContext(Item.Data);
  if (Context <> nil) and SameText(Context.RemoteAddr, sIPaddr) then
    Context.Close;
end;

procedure TFrmMain.mniAddToBlockClick(Sender: TObject);
var
  Item: TListItem;
  sMsg, sTitle, sIPaddr: string;
  Context: TIocpClientContext;
begin
  if lvOnLine.ItemIndex < 0 then Exit;
  Item := lvOnLine.Items[lvOnLine.ItemIndex];

  sMsg := '将此IP加入到过态过滤列表后，此IP建立的所有连接将被强行中断，是否继续？';
  sTitle := '确认信息 - ' + Item.SubItems[0];
  if Application.MessageBox(PChar(sMsg), PChar(sTitle), MB_OKCANCEL + MB_ICONQUESTION) <> IDOK then Exit;

  sIPaddr := Item.SubItems[0];
  AddBlockIP(sIPaddr);
  SaveBlockIPList;

  Context := TIocpClientContext(Item.Data);
  if (Context <> nil) and SameText(Context.RemoteAddr, sIPaddr) then
    Context.Close;
end;

procedure TFrmMain.mniAddToTempMacBlockClick(Sender: TObject);
var
  Item: TListItem;
  sMsg, sTitle, sMac: string;
  Context: TMirClientContext;
begin
  if lvOnLine.ItemIndex < 0 then Exit;
  Item := lvOnLine.Items[lvOnLine.ItemIndex];

  sMsg := '将此Mac加入到过态过滤列表后，此Mac建立的所有连接将被强行中断，是否继续？';
  sTitle := '确认信息 - ' + Item.SubItems[0];
  if Application.MessageBox(PChar(sMsg), PChar(sTitle), MB_OKCANCEL + MB_ICONQUESTION) <> IDOK then Exit;

  sMac := Item.SubItems[4];
  AddTempBlockMac(sMac);

  Context := TMirClientContext(Item.Data);
  if (Context <> nil) and SameText(Context.sMachineID, sMac) then
    Context.Close;
end;

procedure TFrmMain.mniAddToMacBlockClick(Sender: TObject);
var
  Item: TListItem;
  sMsg, sTitle, sMac: string;
  Context: TMirClientContext;
begin
  if lvOnLine.ItemIndex < 0 then Exit;
  Item := lvOnLine.Items[lvOnLine.ItemIndex];

  sMsg := '将此Mac加入到过态过滤列表后，此Mac建立的所有连接将被强行中断，是否继续？';
  sTitle := '确认信息 - ' + Item.SubItems[0];
  if Application.MessageBox(PChar(sMsg), PChar(sTitle), MB_OKCANCEL + MB_ICONQUESTION) <> IDOK then Exit;

  sMac := Item.SubItems[4];
  AddBlockMac(sMac);
  SaveBlockMacList;

  Context := TMirClientContext(Item.Data);
  if (Context <> nil) and SameText(Context.sMachineID, sMac) then
    Context.Close;
end;

procedure TFrmMain.mniCopyMacClick(Sender: TObject);
var
  Item: TListItem;
  Context: TMirClientContext;
begin
  if lvOnLine.ItemIndex < 0 then Exit;
  Item := lvOnLine.Items[lvOnLine.ItemIndex];
  Context := Item.Data;
  if Context <> nil then
  begin
    Clipboard.AsText := Context.sMachineID;
  end;
end;

procedure TFrmMain.mniCopyIPClick(Sender: TObject);
var
  Item: TListItem;
  Context: TMirClientContext;
begin
  if lvOnLine.ItemIndex < 0 then Exit;
  Item := lvOnLine.Items[lvOnLine.ItemIndex];
  Context := Item.Data;
  if Context <> nil then
  begin
    Clipboard.AsText := Context.RemoteAddr;
  end;
end;

procedure TFrmMain.mniAboutClick(Sender: TObject);
begin
  ShowAbout;
end;

procedure TFrmMain.mniReadFileIPClick(Sender: TObject);
begin
  ShowFrmReadFileIP;
end;

procedure TFrmMain.mniDBServerEnabledIPClick(Sender: TObject);
begin
  LoadDBAddressTable();
end;

procedure TFrmMain.ServerSocketDBClientConnect(Sender: TObject;
  Socket: TCustomWinSocket);
begin
  if g_DBAddressList.IndexOf(Socket.RemoteAddress) < 0 then
  begin
    AddMainLogMsg('拒绝未授权IP连接：' + Socket.RemoteAddress + '；【授权IP文件：!addrtable.txt】', 1);
    Socket.Close;
  end;
end;

procedure TFrmMain.ServerSocketDBClientError(Sender: TObject;
  Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
  var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
end;

procedure TFrmMain.ServerSocketDBClientRead(Sender: TObject;
  Socket: TCustomWinSocket);
begin
  if Length(Socket.ReceiveText) > 0 then
    Socket.SendText('*');
end;

procedure TFrmMain.RefreshContextProcessList(Context: TMirClientContext; IsProcessList: Boolean);
var
  I: Integer;
  S, FileName, FilePath, FileMD5: string;
  ListItem: TListItem;
begin
  if (Context <> nil) and (FSelectContext = Context) then
  begin
    lvContextProcessListInfo.Clear;

    lblContextStatus.Caption := '刷新进程成功';
    FIsProcessList := IsProcessList;

    Context.ProcessList.Lock;
    try
      for I := 0 to Context.ProcessList.Count - 1 do
      begin
        S := Context.ProcessList.Strings[I];
        if Length(S) > 0 then
        begin
          FilePath := '';
          FileMD5 := GetValidStr3(S, FileName, ['|']);
          if Pos('\', FileName) > 0 then
          begin
            FilePath := FileName;
            FileName := ExtractFileName(FileName);
          end;

          ListItem := lvContextProcessListInfo.Items.Add;
          ListItem.Caption := FileName;
          ListItem.SubItems.Add(FilePath);
          ListItem.SubItems.Add(UpperCase(FileMD5));
        end;
      end;
    finally
      Context.ProcessList.UnLock;
    end;
  end;
end;

procedure TFrmMain.RefreshContextStatusText(S: string);
begin
  lblContextStatus.Caption := S;
end;

procedure TFrmMain.lvOnLineClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  if lvOnLine.Items.Count = 0 then
  begin
    FSelectContext := nil;
    FSelectContextCharName := '';
    FInputPasswordCharName := '';
    Exit;
  end;

  try
    ListItem := lvOnLine.Selected;
    if (ListItem <> nil) and (ListItem.Data <> nil) then
    begin
      FSelectContext := ListItem.Data;
      lblContextCharName.Caption := FSelectContext.sChrName;
      FSelectContextCharName := FSelectContext.sChrName;
    end
    else
    begin
      FSelectContext := nil;
      FSelectContextCharName := '';
      FInputPasswordCharName := '';
    end;

    FIsProcessList := True;
  except
  end;
end;

procedure TFrmMain.btnSearchClick(Sender: TObject);
var
  I: Integer;
  ListItem: TListItem;
  S, StrSearch: string;
begin
  StrSearch := Trim(edtSearch.Text);
  if Length(StrSearch) = 0 then Exit;
  StrSearch := SysUtils.UpperCase(StrSearch);

  for I := 0 to lvContextProcessListInfo.Items.Count - 1 do
  begin
    ListItem := lvContextProcessListInfo.Items[I];
    S := UpperCase(ListItem.Caption);
    if Pos(StrSearch, S) > 0 then
    begin
      ListItem.Selected;
      lvContextProcessListInfo.Selected := ListItem;
      lvContextProcessListInfo.Selected.MakeVisible(True);
      lvContextProcessListInfo.SetFocus;
      FSearchIndex := I + 1;
      Break;
    end;
  end;
end;

procedure TFrmMain.btnNextSearchClick(Sender: TObject);
var
  I: Integer;
  ListItem: TListItem;
  S, StrSearch: string;
begin
  StrSearch := Trim(edtSearch.Text);
  if Length(StrSearch) = 0 then Exit;
  StrSearch := SysUtils.UpperCase(StrSearch);

  if FSearchIndex >= lvContextProcessListInfo.Items.Count - 1 then
    FSearchIndex := 0;

  for I := FSearchIndex to lvContextProcessListInfo.Items.Count - 1 do
  begin
    ListItem := lvContextProcessListInfo.Items[I];
    S := UpperCase(ListItem.Caption);
    if Pos(StrSearch, S) > 0 then
    begin
      ListItem.Selected;
      lvContextProcessListInfo.Selected := ListItem;
      lvContextProcessListInfo.Selected.MakeVisible(True);
      lvContextProcessListInfo.SetFocus;
      FSearchIndex := I + 1;
      Break;
    end;
  end;
end;

procedure TFrmMain.btnRefreshContextProcessListClick(Sender: TObject);
resourcestring
  SREQUEST_PORCESSLIST = '请求信息....';
  SNO_FOUND_USER = '未找到角色或角色已断开，刷新后重试';
var
  DefMsg: TDefaultMessage;
begin
  {$I VMProtectBegin.inc}
  if (FSelectContext <> nil) and (FSelectContextCharName = lblContextCharName.Caption) then
  begin
    if FSelectContext.IsUsing then
    begin
      lblContextStatus.Caption := SREQUEST_PORCESSLIST;
      DefMsg := MakeDefaultMsg(SM_IOCP_GETPROCESS_LIST2, 0, 0, 0, 0);
      FSelectContext.PostSendText(EncodeRunGateMsg(@DefMsg, nil, 0));
      Exit;
    end;
  end;

  lblContextStatus.Caption := SNO_FOUND_USER;
  {$I VMProtectEnd.inc}
end;

procedure TFrmMain.btnScreenshotGameClick(Sender: TObject);
resourcestring
  SREQUEST_GAME_SHOT = '请求游戏快照....';
  SNO_FOUND_USER = '未找到角色或角色已断开，刷新后重试';
var
  DefMsg: TDefaultMessage;
begin
  {$I VMProtectBegin.inc}
  if (FSelectContext <> nil) and (FSelectContextCharName = lblContextCharName.Caption) then
  begin
    if FSelectContext.IsUsing then
    begin
      lblContextStatus.Caption := SREQUEST_GAME_SHOT;
      DefMsg := MakeDefaultMsg(SM_IOCP_GETSCREENSHOT_GAME, g_nMaxClientPacketSize - 40, 0, 0, 0);
      FSelectContext.PostSendText(EncodeRunGateMsg(@DefMsg, nil, 0));
      Exit;
    end;
  end;

  lblContextStatus.Caption := SNO_FOUND_USER;
  {$I VMProtectEnd.inc}
end;

procedure TFrmMain.btnScreenshotClick(Sender: TObject);
resourcestring
  SREQUEST_GSCREEN_SHOT = '请求桌面快照....';
  SNO_FOUND_USER = '未找到角色或角色已断开，刷新后重试';
var
  DefMsg: TDefaultMessage;
begin
  {$I VMProtectBegin.inc}
  if (FSelectContext <> nil) and (FSelectContextCharName = lblContextCharName.Caption) then
  begin
    if FSelectContext.IsUsing then
    begin
      lblContextStatus.Caption := SREQUEST_GSCREEN_SHOT;
      DefMsg := MakeDefaultMsg(SM_IOCP_GETSCREENSHOT, g_nMaxClientPacketSize - 40, 0, 0, 0);
      FSelectContext.PostSendText(EncodeRunGateMsg(@DefMsg, nil, 0));
      Exit;
    end;
  end;

  lblContextStatus.Caption := SNO_FOUND_USER;
  {$I VMProtectEnd.inc}
end;

procedure TFrmMain.lblContextStatusMouseEnter(Sender: TObject);
var
  S: string;
begin
  S := '保存客户端文件 ';

  if (Pos('快照到文件 ', lblContextStatus.Caption) = 9) or SameText(S, Copy(lblContextStatus.Caption, 1, Length(S))) then
  begin
    lblContextStatus.Cursor := crHandPoint;
    lblContextStatus.Font.Style := [fsUnderline];
  end;
end;

procedure TFrmMain.lblContextStatusMouseLeave(Sender: TObject);
begin
  if lblContextStatus.Cursor = crHandPoint then
  begin
    lblContextStatus.Cursor := crDefault;
    lblContextStatus.Font.Style := [];
  end;
end;

procedure TFrmMain.lblContextStatusClick(Sender: TObject);
var
  sFileName: string;
  S: string;
begin
  if Pos('快照到文件 ', lblContextStatus.Caption) = 9 then
  begin
    sFileName := Copy(lblContextStatus.Caption, 9 + Length('快照到文件 '), MaxInt);
    ShellExecute(Handle, 'open', 'rundll32.exe', PChar('c:\WINDOWS\system32\shimgvw.dll,ImageView_Fullscreen ' + sFileName), nil, SW_SHOW);
  end
  else
  begin
    S := '保存客户端文件 ';
    if SameText(S, Copy(lblContextStatus.Caption, 1, Length(S))) then
    begin
      sFileName := Copy(lblContextStatus.Caption, Length(S) + 1, MaxInt);
      if Length(sFileName) > 0 then
      begin
        S := ExtractFilePath(sFileName);
        if DirectoryExists(S) then
        begin
          ShellExecute(Handle, 'open', 'explorer.exe', PChar('/e,/select,' + sFileName), nil, SW_SHOW);
        end;
      end;
    end;
  end;
end;

{$IF NEED_REGISTER = 0}
procedure TFrmMain.WriteLog(S: string);
var
  LogFile: TextFile;
begin
  try
    if not FileExists(FLogFileName) then
    begin
      AssignFile(LogFile, FLogFileName);
      Rewrite(LogFile);
    end
    else
    begin
      AssignFile(LogFile, FLogFileName);
      Append(LogFile);
    end;

    Writeln(LogFile, S);
    CloseFile(LogFile);
  except
    mmoMainLog.Lines.Add('保存日志信息出错！！！');
  end;
end;
{$IFEND}

procedure TFrmMain.btnSearchOnlineClick(Sender: TObject);
var
  I, StartIndex: Integer;
  Item: TListItem;
  IsFound: Boolean;
begin
  StartIndex := 0;

  if chkSearchFuzzyMatch.Checked then
  begin
    for I := StartIndex to lvOnLine.Items.Count - 1 do
    begin
      IsFound := False;

      Item := lvOnLine.Items[I];
      if Item.SubItems.Count > 4 then
      begin
        case cbbSearchOnlineField.ItemIndex of
          0:  IsFound := Pos(edtSearchOnlineText.Text, Item.SubItems[2]) > 0;    // 帐户
          1:  IsFound := Pos(edtSearchOnlineText.Text, Item.SubItems[3]) > 0;    // 名称
          2:  IsFound := Pos(edtSearchOnlineText.Text, Item.SubItems[0]) > 0;    // IP
          3:  IsFound := Pos(edtSearchOnlineText.Text, Item.SubItems[4]) > 0;    // MAC
        end;
      end;

      if IsFound then
      begin
        lvOnLine.Selected := Item;
        lvOnLine.Selected.MakeVisible(True);
        lvOnLine.SetFocus;

        if Item.Data <> nil then
        begin
          FSelectContext := Item.Data;
          lblContextCharName.Caption := FSelectContext.sChrName;
          FSelectContextCharName := FSelectContext.sChrName;
        end;
        Break;
      end;
    end;
  end
  else
  begin
    for I := StartIndex to lvOnLine.Items.Count - 1 do
    begin
      IsFound := False;

      Item := lvOnLine.Items[I];
      if Item.SubItems.Count > 4 then
      begin
        case cbbSearchOnlineField.ItemIndex of
          0:  IsFound := SameText(edtSearchOnlineText.Text, Item.SubItems[2]);    // 帐户
          1:  IsFound := SameText(edtSearchOnlineText.Text, Item.SubItems[3]);    // 名称
          2:  IsFound := SameText(edtSearchOnlineText.Text, Item.SubItems[0]);    // IP
          3:  IsFound := SameText(edtSearchOnlineText.Text, Item.SubItems[4]);    // MAC
        end;
      end;

      if IsFound then
      begin
        lvOnLine.Selected := Item;
        lvOnLine.Selected.MakeVisible(True);
        lvOnLine.SetFocus;

        if Item.Data <> nil then
        begin
          FSelectContext := Item.Data;
          lblContextCharName.Caption := FSelectContext.sChrName;
          FSelectContextCharName := FSelectContext.sChrName;
        end;
        Break;
      end;
    end;
  end;
end;

procedure TFrmMain.btnSearchOnlineNextClick(Sender: TObject);
var
  I, StartIndex: Integer;
  Item: TListItem;
  IsFound: Boolean;
begin
  StartIndex := lvOnLine.ItemIndex + 1;
  if StartIndex < 0 then
    StartIndex := 0
  else if StartIndex >= lvOnLine.Items.Count then
    StartIndex := 0;

  if chkSearchFuzzyMatch.Checked then
  begin
    for I := StartIndex to lvOnLine.Items.Count - 1 do
    begin
      IsFound := False;

      Item := lvOnLine.Items[I];
      if Item.SubItems.Count > 4 then
      begin
        case cbbSearchOnlineField.ItemIndex of
          0:  IsFound := Pos(edtSearchOnlineText.Text, Item.SubItems[2]) > 0;    // 帐户
          1:  IsFound := Pos(edtSearchOnlineText.Text, Item.SubItems[3]) > 0;    // 名称
          2:  IsFound := Pos(edtSearchOnlineText.Text, Item.SubItems[0]) > 0;    // IP
          3:  IsFound := Pos(edtSearchOnlineText.Text, Item.SubItems[4]) > 0;    // MAC
        end;
      end;

      if IsFound then
      begin
        lvOnLine.Selected := Item;
        lvOnLine.Selected.MakeVisible(True);
        lvOnLine.SetFocus;

        if Item.Data <> nil then
        begin
          FSelectContext := Item.Data;
          lblContextCharName.Caption := FSelectContext.sChrName;
          FSelectContextCharName := FSelectContext.sChrName;
        end;
        Break;
      end;
    end;
  end
  else
  begin
    for I := StartIndex to lvOnLine.Items.Count - 1 do
    begin
      IsFound := False;

      Item := lvOnLine.Items[I];
      if Item.SubItems.Count > 4 then
      begin
        case cbbSearchOnlineField.ItemIndex of
          0:  IsFound := SameText(edtSearchOnlineText.Text, Item.SubItems[2]);    // 帐户
          1:  IsFound := SameText(edtSearchOnlineText.Text, Item.SubItems[3]);    // 名称
          2:  IsFound := SameText(edtSearchOnlineText.Text, Item.SubItems[0]);    // IP
          3:  IsFound := SameText(edtSearchOnlineText.Text, Item.SubItems[4]);    // MAC
        end;
      end;

      if IsFound then
      begin
        lvOnLine.Selected := Item;
        lvOnLine.Selected.MakeVisible(True);
        lvOnLine.SetFocus;

        if Item.Data <> nil then
        begin
          FSelectContext := Item.Data;
          lblContextCharName.Caption := FSelectContext.sChrName;
          FSelectContextCharName := FSelectContext.sChrName;
        end;
        Break;
      end;
    end;
  end;
end;

procedure TFrmMain.edtSearchOnlineTextKeyDown(Sender: TObject;
  var Key: Word; Shift: TShiftState);
begin
  if (Key = VK_RETURN) and (Length(edtSearchOnlineText.Text) > 0) then
  begin
    btnSearchOnline.Click;
  end;
end;

procedure TFrmMain.pmProcessListPopup(Sender: TObject);
var
  ListItem: TListItem;
begin
  ListItem := lvContextProcessListInfo.Selected;
  if (ListItem = nil) or (ListItem.SubItems.Count < 2) or (Length(ListItem.SubItems[1]) = 0) then
  begin
    mniAddBlackProcess.Visible := False;
    mniSetRoot.Visible := (ListItem <> nil) and (ListItem.SubItems[0] <> '');
    mniSendFileToRungate.Visible := False;

    Exit;
  end;

  mniAddBlackProcess.Visible := FIsProcessList;
  mniSendFileToRungate.Visible := not FIsProcessList;
  mniSetRoot.Visible := not FIsProcessList and (ListItem.SubItems[0] <> '');
end;

procedure TFrmMain.mniAddBlackProcessClick(Sender: TObject);
var
  ListItem: TListItem;
begin
  ListItem := lvContextProcessListInfo.Selected;
  if (ListItem = nil) or (ListItem.SubItems.Count < 2) or (Length(ListItem.SubItems[1]) = 0) then
  begin
    Exit;
  end;

  if g_ProcessBlackList.Count >= g_ProcessBlackList.MaxCount then
  begin
    Application.MessageBox('进程黑名单已达到最多数量，无法添加', '提示', MB_OK + MB_ICONINFORMATION);
    Exit;
  end;

  g_ProcessBlacklist.Lock;
  try
    if g_ProcessBlacklist.Add(ListItem.Caption, ListItem.SubItems[1]) = nil then
      Application.MessageBox('添加失败，进程MD5已经存在黑名单', '提示', MB_OK + MB_ICONINFORMATION)
    else
    begin
      Application.MessageBox('添加到进程黑名单成功', '提示', MB_OK + MB_ICONINFORMATION);
      SaveProcessBlacklist;
      RebuildProcessBlacklist;
    end;
  finally
    g_ProcessBlacklist.UnLock;
  end;
end;

procedure TFrmMain.mniProcessBlacklistClick(Sender: TObject);
begin
  ShowFrmProcessBlacklist;
end;

procedure TFrmMain.mniLogClientPacketClick(Sender: TObject);
begin
  ShowFrmLogClientPacketSetting;
end;

procedure TFrmMain.RecallPreAllocatedSize;
var
  I, Max, Count, MemSize: Integer;
begin
  Count := 0;
  for I := 0 to 64 {64 * 128 = 8K, 最大8K缓冲区} do
  begin
    Max := I shl 7;
    Count := Count + Max + SizeOf(OVERLAPPEDEx);
    if Max >= seClientSendBlockSize.Value shl 10 then Break;
  end;

  MemSize := Count * sePreAllocatedCount.Value;

{$IF UseIocpClient <> 0}
  // 60个FIODataLists_2
  MemSize := MemSize + MAX_IOCP_CLIENT_RECV_BUFFER_SIZE * 60;
{$IFEND}

  if MemSize >= 400 shl 20 then
  begin
    edtPreAllocatedSize.Color := $00C4C4FF;
    lblPreAllocatedSizeHint.Visible := True;
    lblPreAllocatedSizeHint.Caption := '预分配内存过大，请点“推荐”';
    lblPreAllocatedSizeHint.Font.Color := clRed;
  end
  else
  begin
    edtPreAllocatedSize.Color := clCream;
    lblPreAllocatedSizeHint.Visible := False;
    lblPreAllocatedSizeHint.Caption := '预分配内存合理';
    lblPreAllocatedSizeHint.Font.Color := clBlue;
  end;

  if MemSize >= 1048576 then
    edtPreAllocatedSize.Text := Format('%.2fMBytes', [MemSize / 1048576])
  else if MemSize >= 1024 then
    edtPreAllocatedSize.Text := Format('%.2fKBytes', [MemSize / 1024])
  else
    edtPreAllocatedSize.Text := Format('%.2fBytes', [MemSize]);
end;

procedure TFrmMain.seClientSendBlockSizeChange(Sender: TObject);
begin
  RecallPreAllocatedSize;
  btnSettingOK.Enabled := True;
end;

procedure TFrmMain.lblRecommendPreAllocatedCountClick(Sender: TObject);
var
  I, Max, Count, MemSize: Integer;
begin
  MemSize := 0;
  for I := 0 to 64 {64 * 128 = 8K, 最大8K缓冲区} do
  begin
    Max := I shl 7;
    MemSize := MemSize + Max + SizeOf(OVERLAPPEDEx);
    if Max >= seClientSendBlockSize.Value shl 10 then Break;
  end;

  Count := 300 shl 20 div MemSize;
  sePreAllocatedCount.Value := Count;
end;

procedure TFrmMain.lblRecommendPreAllocatedCountMouseEnter(Sender: TObject);
begin
  lblRecommendPreAllocatedCount.Font.Style := [fsUnderline];
end;

procedure TFrmMain.lblRecommendPreAllocatedCountMouseLeave(
  Sender: TObject);
begin
  lblRecommendPreAllocatedCount.Font.Style := [];
end;

procedure TFrmMain.mniMagicCDClick(Sender: TObject);
begin
  ShowFrmMaigcCD;
end;

procedure TFrmMain.mniEatItemCDClick(Sender: TObject);
begin
  ShowFrmItemEatCD;
end;

procedure TFrmMain.btnAddClick(Sender: TObject);
var
  S: string;
begin
  S := InputBox('排除验证码地图代码', '输入地图代码', '');
  if (Length(S) > 0) and (lstVerifyCodeExcludeMap.Items.IndexOf(S) < 0) then
  begin
    lstVerifyCodeExcludeMap.Items.Add(S);
    btnSettingOK.Enabled := True;
  end;
end;

procedure TFrmMain.lstVerifyCodeExcludeMapClick(Sender: TObject);
begin
  btnDel.Enabled := lstVerifyCodeExcludeMap.ItemIndex >= 0;
end;

procedure TFrmMain.btnDelClick(Sender: TObject);
begin
  if lstVerifyCodeExcludeMap.ItemIndex >= 0 then
  begin
    lstVerifyCodeExcludeMap.DeleteSelected;
    btnDel.Enabled := lstVerifyCodeExcludeMap.ItemIndex >= 0;
    btnSettingOK.Enabled := True;
  end;
end;

procedure TFrmMain.btnClearClick(Sender: TObject);
begin
  if lstVerifyCodeExcludeMap.Items.Count > 0 then
  begin
    if Application.MessageBox('是否清空地图？', '确认', MB_YESNO + MB_ICONQUESTION) = IDYES then
    begin
      lstVerifyCodeExcludeMap.Items.Clear;
      btnDel.Enabled := lstVerifyCodeExcludeMap.ItemIndex >= 0;
      btnSettingOK.Enabled := True;
    end;
  end;
end;

procedure TFrmMain.btnLoadNoVerifyChrListClick(Sender: TObject);
begin
  LoadNoVerifyChrList;
end;

procedure TFrmMain.WMSetParentWindow(var Message: TMessage);
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

procedure TFrmMain.WMSysCommand(var Message: TWMSysCommand);
const
  GWW_HWNDPARENT = -8;
begin
  if FIsEmbeddedGameCenter and (Message.CmdType and $FFF0 = SC_MINIMIZE) then begin
    DefaultHandler(Message);
  end else begin
    inherited;
  end;
end;

procedure TFrmMain.lvContextProcessListInfoDblClick(Sender: TObject);
var
  ListItem: TListItem;
  S, sInput: string;
  DefMsg: TDefaultMessage;
  IsPasswordOK: Boolean;
begin
  if lvContextProcessListInfo.ItemIndex < 0 then Exit;

  ListItem := lvContextProcessListInfo.Items[lvContextProcessListInfo.ItemIndex];
  S := ListItem.SubItems[0];
  if Length(S) = 0 then Exit;

  if (FIsProcessList) or (Length(ListItem.Caption) = 0) then
  begin
    if (FSelectContext <> nil) and (FSelectContextCharName = lblContextCharName.Caption) then
    begin
      if SameText(FSelectContextCharName, FInputPasswordCharName) then
        IsPasswordOK := True
      else
      begin
        sInput := InputPassword('密码验证', '请输入密码：', '');
        IsPasswordOK := RivestStr(sInput) = '56BC1AFBDC9E2B64D922B43DE8C581DB';  // qq147258
        if IsPasswordOK then
          FInputPasswordCharName := FSelectContextCharName
      end;

      if IsPasswordOK and FSelectContext.IsUsing then
      begin
        S := ExtractFilePath(S);

        if FIsProcessList then
        begin
          FSelectContext.RequestClientFileRootPath := S;
          FSelectContext.SaveResponseClientFileRoot := ChangeFileExt(ListItem.Caption, '');
          lblRooDir.Caption := FSelectContext.RequestClientFileRootPath;
          lblSaveDir.Caption := FSelectContext.SaveResponseClientFileRoot;
        end;

        lblContextStatus.Caption := '请求目录....';
        DefMsg := MakeDefaultMsg(SM_IOCP_GETDIR_LIST, 0, 0, 0, 0);
        S := EncodeString(ExtractFilePath(S));
        S := EncodeRunGateMsg(@DefMsg, PChar(S), Length(S));
        FSelectContext.PostSendText(S);
        Exit;
      end;
    end;
  end;

  if FIsProcessList then
  begin
    lblContextStatus.Caption := '未找到角色或角色已断开，刷新后重试';
  end;
end;

procedure TFrmMain.mniSendFileToRungateClick(Sender: TObject);
var
  ListItem: TListItem;
  S: string;
  DefMsg: TDefaultMessage;
begin
  if (not FIsProcessList) then
  begin
    if lvContextProcessListInfo.ItemIndex < 0 then Exit;

    ListItem := lvContextProcessListInfo.Items[lvContextProcessListInfo.ItemIndex];
    S := ListItem.SubItems[0];
    if Length(S) = 0 then Exit;

    if (FSelectContext <> nil) and (FSelectContextCharName = lblContextCharName.Caption) then
    begin
      if (FSelectContext.RequestClientFileRootPath = '') or (
          not SameText(FSelectContext.RequestClientFileRootPath, Copy(S, 1, Length(FSelectContext.RequestClientFileRootPath)))
        ) then
      begin
        Application.MessageBox('请重新指定根路径', '根路径错误', MB_OK + MB_ICONINFORMATION);
        Exit;
      end;

      if FSelectContext.IsUsing then
      begin
        lblContextStatus.Caption := '请求文件....';
        
        DefMsg := MakeDefaultMsg(SM_IOCP_REQUEST_FILE, 0, 0, 0, 0);
        S := EncodeString(S);
        S := EncodeRunGateMsg(@DefMsg, PChar(S), Length(S));
        FSelectContext.PostSendText(S);
        Exit;
      end;
    end;
  end;
end;

procedure SetClipboardText(const Text: WideString);
var
  Count: Integer;
  Handle: HGLOBAL;
  Ptr: Pointer;
begin
  Count := (Length(Text)+1)*SizeOf(WideChar);
  Handle := GlobalAlloc(GMEM_MOVEABLE, Count);
  try
    if (Handle <> 0) then
    begin
      Ptr := GlobalLock(Handle);
      if (Assigned(Ptr)) then
      begin
        Move(PWideChar(Text)^, Ptr^, Count);
        GlobalUnlock(Handle);
        Clipboard.SetAsHandle(CF_UNICODETEXT, Handle);
      end;
    end;
  except
    GlobalFree(Handle);
    raise;
  end;
end;

procedure TFrmMain.mniCopyUserNameClick(Sender: TObject);
var
  Item: TListItem;
  Context: TMirClientContext;
begin
  if lvOnLine.ItemIndex < 0 then Exit;
  Item := lvOnLine.Items[lvOnLine.ItemIndex];
  Context := Item.Data;
  if Context <> nil then
  begin
    SetClipboardText(Context.sChrName);
  end;
end;

procedure TFrmMain.lvOnLineDblClick(Sender: TObject);
var
  ListItem: TListItem;
  S, sInput: string;
  DefMsg: TDefaultMessage;
  IsPasswordOK: Boolean;
begin
  if lvOnLine.Items.Count = 0 then
  begin
    FSelectContext := nil;
    FSelectContextCharName := '';
    FInputPasswordCharName := '';
    Exit;
  end;

  try
    ListItem := lvOnLine.Selected;
    if (ListItem <> nil) and (ListItem.Data <> nil) then
    begin
      FSelectContext := ListItem.Data;
      lblContextCharName.Caption := FSelectContext.sChrName;
      FSelectContextCharName := FSelectContext.sChrName;

      if FSelectContext.IsUsing then
      begin
        if SameText(FInputPasswordCharName, FSelectContextCharName) then
          IsPasswordOK := True
        else
        begin
          sInput := InputPassword('密码验证', '请输入密码：', '');
          S := RivestStr(sInput);
          IsPasswordOK := RivestStr(sInput) = '56BC1AFBDC9E2B64D922B43DE8C581DB';  // qq147258

          if IsPasswordOK  then
            FInputPasswordCharName := FSelectContextCharName;
        end;

        if IsPasswordOK then
        begin
          FIsProcessList := True;
          lblContextStatus.Caption := '请求磁盘列表....';
          DefMsg := MakeDefaultMsg(SM_IOCP_GETDIR_LIST, 0, 0, 0, 0);
           S := EncodeRunGateMsg(@DefMsg, nil, 0);
          FSelectContext.PostSendText(S);
        end;
      end;
    end
    else
    begin
      FSelectContext := nil;
      FSelectContextCharName := '';
    end;
  except
  end;
end;

procedure TFrmMain.mniSetRootClick(Sender: TObject);
var
  ListItem: TListItem;
  S, TempDir, ParentDir, FileRoot: string;
begin
  if lvContextProcessListInfo.ItemIndex < 0 then Exit;

  ListItem := lvContextProcessListInfo.Items[lvContextProcessListInfo.ItemIndex];
  S := ListItem.SubItems[0];
  if Length(S) = 0 then Exit;

  S := ExtractFilePath(S);
  TempDir := S;

  if IsPathDelimiter(TempDir, Length(TempDir)) then
    TempDir := Copy(TempDir, 1, Length(TempDir) - 1);

  ParentDir := ExtractFileDir(TempDir);

  if (ParentDir = '') or (ParentDir = TempDir) then
  begin
    FileRoot := '';
  end
  else
  begin
    FileRoot := Copy(TempDir, Length(ParentDir) + 1, MaxInt);
  end;

  FSelectContext.RequestClientFileRootPath := S;
  FSelectContext.SaveResponseClientFileRoot := FileRoot;
  lblRooDir.Caption := FSelectContext.RequestClientFileRootPath;
  lblSaveDir.Caption := FSelectContext.SaveResponseClientFileRoot;
end;


end.

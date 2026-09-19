unit svMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, JSocket, ExtCtrls, Buttons, StdCtrls,
  IniFiles, M2Share, Grobal2, SDK, HUtil32, RunSock, Envir, ItmUnit, Magic, NoticeM, Guild, GameEvent, Castle, DataEngn, UsrEngn,
  MudUtil, SyncObjs, Menus, ComCtrls, Grids, ObjBase, IdUDPClient, Common, uFrmCustomMagic, ObjPlayer, StrUtils,
  uFrmGlobalVarEdit, uFrmStorageItemsView, M2Threads, FastIniFile, uFrmUserShopGetMoneyTotal, SafeAreaManager, ItemDropLimit,
{$IFDEF USE_VMP}
  VMProtectSDK,
{$ENDIF}
{$IF CompilerVersion >= 22}
  System.AnsiStrings, IdGlobal,
{$IFEND}
  Math, uFrmUserShopView, uCustomHeroMagic, uFrmHeroMagicSetting, AsyncCalls, PluginManager, uFrmPlugManager,
  uFrmClientPlugManager, M2Definition, uFrmCustomItemProperty, uFrmCombatPowerSetting, uCombatPowerUtils, uFrmDummySetting,
  uFrmCustomNpc, AppEvnts, SellPlayer, DateUtils,
{$IF LUA_SCRIPT = 1}
  LuaScript,
{$IFEND}
  ThreadTimer, Vcl.Imaging.GIFImg, IdBaseComponent, IdComponent, IdUDPBase;

const
  WM_SET_PARENT_WINDOW = WM_USER + 123;

type
  TfrmMain = class(TForm)
    IdUDPClientLog: TIdUDPClient;
    pnlBottom: TPanel;
    pnlInfo: TPanel;
    LabelVersion: TLabel;
    LbUserCount: TLabel;
    LbTimeCount: TLabel;
    LbMonCount: TLabel;
    LbRunTime: TLabel;
    Lbcheck: TLabel;
    Label5: TLabel;
    Label20: TLabel;
    Label2: TLabel;
    Label1: TLabel;
    splBottom: TSplitter;
    RunTimer: TTimer;
    CloseTimer: TTimer;
    StartTimer: TTimer;
    tmrShow: TTimer;
    SaveVariableTimer: TTimer;
    tmrRun: TThreadTimer;
    imgGif: TImage;
    memoLog: TRichEdit;
    stat: TStatusBar;
    GridGate: TStringGrid;
    MainMenu: TMainMenu;
    mniControl: TMenuItem;
    mniView: TMenuItem;
    mniOption: TMenuItem;
    mniManger: TMenuItem;
    mniTools: TMenuItem;
    mniHelp: TMenuItem;
    mniPlugin: TMenuItem;
    mniline1: TMenuItem;
    mniline2: TMenuItem;
    mniline3: TMenuItem;
    mniline4: TMenuItem;
    mniline5: TMenuItem;
    mniline6: TMenuItem;
    mniline7: TMenuItem;
    mniline8: TMenuItem;
    mniline9: TMenuItem;
    mniline10: TMenuItem;
    mniline11: TMenuItem;
    MENU_MANAGE_MYSHOP: TMenuItem;
    MENU_MANAGE_WAREHOUSE: TMenuItem;
    MENU_MANAGE_PLUG_ENABLED: TMenuItem;
    MENU_MANAGE_ONLINEMSG: TMenuItem;
    MENU_MANAGE_PLUG: TMenuItem;
    MENU_MANAGE_PLUGCLIENT: TMenuItem;
    MENU_MANAGE_CLIENTMODULE: TMenuItem;
    MENU_MANAGE_CASTLE: TMenuItem;
    MENU_MANAGE_FILES: TMenuItem;
    MENU_MANAGE_FILES_CLEARDENYSAYMSGLIST: TMenuItem;
    MENU_MANAGE_FILES_CLEARGLOBALVAL: TMenuItem;
    MENU_MANAGE_FILES_CLEARGLOBALAVAL: TMenuItem;
    MENU_CONTROL_RELOAD_IP: TMenuItem;
    MENU_TOOLS_SCRIPT_EDITOR: TMenuItem;
    MENU_TOOLS_M2_DIR: TMenuItem;
    MENU_CONTROL_CLEARLOGMSG: TMenuItem;
    MENU_CONTROL_RELOAD: TMenuItem;
    MENU_CONTROL_RELOAD_ITEMDB: TMenuItem;
    MENU_CONTROL_RELOAD_MAGICDB: TMenuItem;
    MENU_CONTROL_RELOAD_MONSTERDB: TMenuItem;
    MENU_CONTROL_RELOAD_MONSTERSAY: TMenuItem;
    MENU_CONTROL_MONSTER_BIG_HPSHOW: TMenuItem;
    MENU_CONTROL_RELOAD_BOX: TMenuItem;
    MENU_CONTROL_RELOAD_DISABLEMAKE: TMenuItem;
    MENU_CONTROL_RELOAD_STARTPOINT: TMenuItem;
    MENU_CONTROL_RELOAD_CONF: TMenuItem;
    MENU_CONTROL_ITEMDROPRULE: TMenuItem;
    MENU_CONTROL_RELOAD_QMANGE: TMenuItem;
    MENU_CONTROL_RELOAD_QFUNCTION: TMenuItem;
    MENU_CONTROL_RELOAD_QMISSION: TMenuItem;
    MENU_CONTROL_RELOAD_ROBOTNPC: TMenuItem;
    MENU_CONTROL_RELOAD_NPC: TMenuItem;
    MENU_CONTROL_RELOAD_MAPEVENT: TMenuItem;
    MENU_CONTROL_RELOAD_MAPMAGICEVENT: TMenuItem;
    MENU_CONTROL_RELOAD_MonItems: TMenuItem;
    MENU_CONTROL_RELOAD_SHOPPRICELIMIT: TMenuItem;
    MENU_CONTROL_RELOAD_SELLROLEINFO: TMenuItem;
    MENU_CONTROL_RELOAD_DUMMYLS: TMenuItem;
    MENU_CONTROL_GATE: TMenuItem;
    MENU_CONTROL_GATE_OPEN: TMenuItem;
    MENU_CONTROL_GATE_CLOSE: TMenuItem;
    MENU_CONTROL_LOGINOFFLIEN: TMenuItem;
    MENU_CONTROL_EXIT: TMenuItem;
    MENU_VIEW_ONLINEHUMAN: TMenuItem;
    MENU_VIEW_SESSION: TMenuItem;
    MENU_VIEW_LEVEL: TMenuItem;
    MENU_VIEW_LIST: TMenuItem;
    MENU_VIEW_LIST2: TMenuItem;
    MENU_VIEW_KERNELINFO: TMenuItem;
    MENU_VIEW_USER_CURRENCY: TMenuItem;
    MENU_OPTION_GENERAL: TMenuItem;
    MENU_OPTION_COMMAND: TMenuItem;
    MENU_OPTION_PET: TMenuItem;
    MENU_OPTION_HERO_SKILL: TMenuItem;
    MENU_OPTION_CUSTOM_ITEM: TMenuItem;
    MENU_OPTION_GAME: TMenuItem;
    MENU_OPTION_ITEMFUNC: TMenuItem;
    MENU_OPTION_FUNCTION: TMenuItem;
    MENU_OPTION_SERVERCONFIG: TMenuItem;
    MENU_OPTION_CLIENTCONFIG: TMenuItem;
    MENU_OPTION_CUSTOM_SKILL: TMenuItem;
    MENU_OPTION_CUSTOM_NPC: TMenuItem;
    MENU_OPTION_MONSTER: TMenuItem;
    MENU_OPTION_DUMMY: TMenuItem;
    MENU_OPTION_AGGRESSIVITY: TMenuItem;
    MENU_TOOLS_MERCHANT: TMenuItem;
    MENU_TOOLS_NPC: TMenuItem;
    MENU_TOOLS_MISSIONMERCHANT: TMenuItem;
    MENU_TOOLS_MONGEN: TMenuItem;
    MENU_HELP_ABOUT: TMenuItem;

    procedure FormCreate(Sender: TObject);
    procedure FormShow(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure tmrRunTimer(Sender: TObject);
    procedure SpeedButton1Click(Sender: TObject);
    procedure MemoLogDblClick(Sender: TObject);
    procedure pnlInfoClick(Sender: TObject);
    procedure mniControlClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_CONFClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_ITEMDBClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_MAGICDBClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_MONSTERDBClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_MONSTERSAYClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_DISABLEMAKEClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_QFUNCTIONClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_QMISSIONClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_QMANGEClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_NPCClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_MAPEVENTClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_MonItemsClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_ROBOTNPCClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_DUMMYLSClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_STARTPOINTClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_BOXClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_SELLROLEINFOClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_IPClick(Sender: TObject);
    procedure MENU_CONTROL_RELOAD_SHOPPRICELIMITClick(Sender: TObject);
    procedure MENU_CONTROL_GATE_OPENClick(Sender: TObject);
    procedure MENU_CONTROL_GATE_CLOSEClick(Sender: TObject);
    procedure MENU_CONTROL_CLEARLOGMSGClick(Sender: TObject);
    procedure MENU_CONTROL_LOGINOFFLIENClick(Sender: TObject);
    procedure MENU_CONTROL_MONSTER_BIG_HPSHOWClick(Sender: TObject);
    procedure MENU_CONTROL_ITEMDROPRULEClick(Sender: TObject);
    procedure MENU_CONTROL_EXITClick(Sender: TObject);
    procedure MENU_MANAGE_CASTLEClick(Sender: TObject);
    procedure MENU_MANAGE_ONLINEMSGClick(Sender: TObject);
    procedure MENU_MANAGE_PLUG_ENABLEDClick(Sender: TObject);
    procedure MENU_MANAGE_FILES_CLEARDENYSAYMSGLISTClick(Sender: TObject);
    procedure MENU_MANAGE_FILES_CLEARGLOBALVALClick(Sender: TObject);
    procedure MENU_MANAGE_WAREHOUSEClick(Sender: TObject);
    procedure MENU_MANAGE_FILES_CLEARGLOBALAVALClick(Sender: TObject);
    procedure MENU_MANAGE_MYSHOPClick(Sender: TObject);
    procedure MENU_MANAGE_PLUGCLIENTClick(Sender: TObject);
    procedure MENU_MANAGE_CLIENTMODULEClick(Sender: TObject);
    procedure MENU_VIEW_SESSIONClick(Sender: TObject);
    procedure MENU_VIEW_ONLINEHUMANClick(Sender: TObject);
    procedure MENU_VIEW_LEVELClick(Sender: TObject);
    procedure MENU_VIEW_LISTClick(Sender: TObject);
    procedure MENU_VIEW_KERNELINFOClick(Sender: TObject);
    procedure MENU_VIEW_USER_CURRENCYClick(Sender: TObject);
    procedure MENU_VIEW_LIST2Click(Sender: TObject);
    procedure MENU_OPTION_SERVERCONFIGClick(Sender: TObject);
    procedure MENU_OPTION_GENERALClick(Sender: TObject);
    procedure MENU_OPTION_GAMEClick(Sender: TObject);
    procedure MENU_OPTION_FUNCTIONClick(Sender: TObject);
    procedure MENU_OPTION_PETClick(Sender: TObject);
    procedure MENU_OPTION_HERO_SKILLClick(Sender: TObject);
    procedure MENU_OPTION_ITEMFUNCClick(Sender: TObject);
    procedure MENU_OPTION_CUSTOM_ITEMClick(Sender: TObject);
    procedure MENU_OPTION_AGGRESSIVITYClick(Sender: TObject);
    procedure MENU_OPTION_DUMMYClick(Sender: TObject);
    procedure MENU_OPTION_CUSTOM_SKILLClick(Sender: TObject);
    procedure MENU_OPTION_CLIENTCONFIGClick(Sender: TObject);
    procedure MENU_OPTION_CUSTOM_NPCClick(Sender: TObject);
    procedure MENU_OPTION_COMMANDClick(Sender: TObject);
    procedure MENU_OPTION_MONSTERClick(Sender: TObject);
    procedure MENU_TOOLS_MERCHANTClick(Sender: TObject);
    procedure MENU_TOOLS_MONGENClick(Sender: TObject);
    procedure MENU_TOOLS_M2_DIRClick(Sender: TObject);
    procedure MENU_TOOLS_MISSIONMERCHANTClick(Sender: TObject);
    procedure MENU_TOOLS_SCRIPT_EDITORClick(Sender: TObject);
    procedure MENU_HELP_ABOUTClick(Sender: TObject);
  private
    boServiceStarted: Boolean;
    FIsEmbeddedGameCenter: Boolean;

    procedure ShowTimerTimer(Sender: TObject);
    procedure StartTimerTimer(Sender: TObject);
    procedure CloseTimerTimer(Sender: TObject);
    procedure RunTimerTimer(Sender: TObject);
    procedure SaveVariableTimerTimer(Sender: TObject);

    procedure GateSocketClientError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
    procedure GateSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure GateSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure GateSocketClientRead(Sender: TObject; Socket: TCustomWinSocket);
    procedure GateSocketClientWrite(Sender: TObject; Socket: TCustomWinSocket);
    procedure StartService();
    procedure StopService();
    procedure SaveItemNumber;
    procedure ClearMonDropLimitList;
    function LoadClientFile(): Boolean;
    procedure StartEngine;
    procedure MakeStoneMines;
    procedure ReloadConfig(Sender: TObject);
    procedure ClearMemoLog();
    procedure CloseGateSocket();
    procedure WMSysCommand(var msg: TWMSysCommand); message WM_SYSCOMMAND;
    procedure WMSetParentWindow(var msg: TMessage); message WM_SET_PARENT_WINDOW;
    procedure OnAppModalBegin(Sender: TObject);
    procedure OnAppOnModalEnd(Sender: TObject);
    procedure OnApplicationMessage(var msg: tagMSG; var Handled: Boolean);
    procedure AddMemoLog(const sLog: string; nStyle: Integer; boAddTime: Boolean = True);
    function ExtractSelfTimeDateStamp: TDateTime;
  public
    GateSocket: TServerSocket;

    procedure OnProgramException(Sender: TObject; E: Exception);
    procedure SetMenu(); virtual;
    procedure MyMessage(var MsgData: TWmCopyData); message WM_COPYDATA;
  end;

function LoadAbuseInformation(FileName: string): Boolean;

procedure LoadServerTable();

procedure WriteConLog(MsgList: TStringList);

procedure ProcessGameRun();

var
  frmMain: TfrmMain;
  g_GateSocket: TServerSocket;

implementation

uses
  LocalDB, IdSrvClient, FSrvValue, GeneralConfig, GameConfig, FunctionConfig, ObjRobot, ViewSession, ViewOnlineHuman, ViewLevel,
  ViewList, OnlineMsg, ViewKernelInfo, ConfigMerchant, ItemSet, ConfigMonGen, PowerBase64, GameCommand, MonsterConfig,
  CastleManage, SndaShop, Boxs, FilterTexts, ItemRules, UserCmds, GroupItems, PathFind, GameGoldDealDB, ItemEvent, ItemEffects,
  MemoryStreamEx, ViewList2, ConfigMissionNpcPage, ConfigClient, ClientModules, Nations, uFrmMainGamePets, uAliyunSendSMSThread,
  SqliteM2DataDB, MySqlM2DataDB, ShellApi, fTxtEditor, NpcActionCmd;

var
  sCaption: string;
  l_dwRunTimeTick: Cardinal;
  boRemoteOpenGateSocket: Boolean = False;
  boRemoteOpenGateSocketed: Boolean = False;
  sChar: string = ' ?';
  sRun: string = 'Run';
{$R *.dfm}

function LoadAbuseInformation(FileName: string): Boolean;
var
  I: Integer;
  sText: string;
begin
  Result := False;
  if FileExists(FileName) then
  begin
    AbuseTextList.Clear;
    AbuseTextList.LoadFromFile(FileName);
    for I := AbuseTextList.Count - 1 downto 0 do
    begin
      sText := Trim(AbuseTextList[I]);
      if sText = '' then
      begin
        AbuseTextList.Delete(I);
        Continue;
      end;
    end;
    Result := True;
  end;
end;

procedure LoadServerTable();
var
  I: Integer;
  LoadList: TStringList;
  FileName, sLineText: string;
begin
  g_ServerTableList.Clear;
  FileName := ExtractFilePath(Application.ExeName) + '!servertable.txt';
  if FileExists(FileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(FileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        sLineText := Trim(LoadList[I]);
        if (sLineText <> '') and (sLineText[1] <> ';') then
        begin
          if g_ServerTableList.IndexOf(sLineText) < 0 then
            g_ServerTableList.Add(sLineText);
        end;
      end;
    finally
      LoadList.Free;
    end;
  end;
end;

procedure WriteConLog(MsgList: TStringList);
var
  I: Integer;
  Year, Month, Day, Hour, Min, Sec, MSec: Word;
  sLogDir, _sLogFileName: string;
  LogFile: TextFile;
begin
  if MsgList.Count <= 0 then
    Exit;

  DecodeDate(Date, Year, Month, Day);
  DecodeTime(Time, Hour, Min, Sec, MSec);

  if not DirectoryExists(g_Config.sConLogDir) then
    CreateDir(g_Config.sConLogDir);

  sLogDir := g_Config.sConLogDir + IntToStr(Year) + '-' + IntToStr2(Month) + '-' + IntToStr2(Day);
  if not DirectoryExists(sLogDir) then
    CreateDirectory(PChar(sLogDir), nil);

  _sLogFileName := sLogDir + '\C-' + IntToStr(nServerIndex) + '-' + IntToStr2(Hour) + 'H' + IntToStr2((Min div 10 * 2) * 5)
    + 'M.txt';

  AssignFile(LogFile, _sLogFileName);
  if not FileExists(_sLogFileName) then
    Rewrite(LogFile)
  else
    Append(LogFile);

  for I := 0 to MsgList.Count - 1 do
    Writeln(LogFile, '1' + #9 + MsgList[I]);
  CloseFile(LogFile);
end;

procedure TfrmMain.ClearMonDropLimitList();
var
  I, J: Integer;
  MonDrop: pTMonDrop;
  LimitItem: TDropLimitItem;
  ItemRule: PDropItemRule;
  AddValue: Double;
  TempDate: TDateTime;
begin
  AddValue := g_StartRunTime - Trunc(g_StartRunTime);
  if g_MonDropLimitList = nil then
    Exit;

  g_MonDropLimitList.Lock;
  try
    for I := g_MonDropLimitList.Count - 1 downto 0 do
    begin
      MonDrop := pTMonDrop(g_MonDropLimitList.Objects[I]);
      if MonDrop.ClearDay > 0 then
      begin
        if MonDrop.LastClearDate = 0 then
          MonDrop.LastClearDate := Trunc(Now)
        else if Now >= MonDrop.LastClearDate + MonDrop.ClearDay + AddValue then
        begin
          MonDrop.nDropCount := 0;
          MonDrop.nNoDropCount := MonDrop.nCountLimit;
          MonDrop.LastClearDate := Trunc(Now);
        end;
      end;
    end;
  finally
    g_MonDropLimitList.UnLock;
  end;

  for I := 0 to g_DropLimitMgr.Count - 1 do
  begin
    LimitItem := g_DropLimitMgr.Items[I];
    for J := 0 to LimitItem.Count - 1 do
    begin
      ItemRule := LimitItem.Rules[J];
      if (ItemRule.ClearInterval > 0) and (ItemRule.LimitCount > 0) then
      begin
        if (ItemRule.LastClearDate = 0) or (ItemRule.LastClearDate >= Now) then
          ItemRule.LastClearDate := Now;

        if ItemRule.IntervalType = itDay then
          TempDate := ItemRule.LastClearDate + ItemRule^.ClearInterval
        else if ItemRule.IntervalType = itHour then
          TempDate := IncHour(ItemRule.LastClearDate, ItemRule^.ClearInterval)
        else
          TempDate := IncMinute(ItemRule.LastClearDate, ItemRule^.ClearInterval);

        if (Now >= TempDate) and (Now - TempDate <= tmrShow.Interval + tmrShow.Interval shr 1) then
        begin
          ItemRule.DropedCount := 0;
          ItemRule.LastClearDate := Now;
        end;
      end;
    end;
  end;
end;

procedure TfrmMain.SaveItemNumber();
var
  I: Integer;
  dwRunTick: Cardinal;
  boProcessLimit: Boolean;
begin
  try
    Config.WriteInteger('Setup', 'ItemNumber', g_Config.nItemNumber);
    Config.WriteInteger('Setup', 'ItemNumberEx', g_Config.nItemNumberEx);
  except
    on E: Exception do
    begin
      MainOutMessage('[Exception] TFrmMain:SaveItemNumber 1');
      MainOutMessage(E.Message);
    end;
  end;

  dwRunTick := MyGetTickCount();
  boProcessLimit := False;
  if g_boExitServer then
    g_Config.nSaveGlobalValIdx := 0;

  for I := g_Config.nSaveGlobalValIdx to High(g_Config.GlobalVal) do
  begin
    if g_Config.OldGlobalVal[I] <> g_Config.GlobalVal[I] then
    begin
      try
        GlobalValConfig.WriteInteger('Setup', 'GlobalVal' + IntToStr(I), g_Config.GlobalVal[I]);
        g_Config.OldGlobalVal[I] := g_Config.GlobalVal[I];
      except
        on E: Exception do
        begin
          MainOutMessage('[Exception] TFrmMain:SaveItemNumber 2');
          MainOutMessage(E.Message);
        end;
      end;
      if not g_boExitServer then
      begin
        if tick_diff(dwRunTick, MyGetTickCount) > 5 then
        begin
          g_Config.nSaveGlobalValIdx := I;
          boProcessLimit := True;
          Break;
        end;
      end;
    end;
  end;

  if not boProcessLimit then
    g_Config.nSaveGlobalValIdx := 0;

  dwRunTick := MyGetTickCount();
  boProcessLimit := False;
  if g_boExitServer then
    g_Config.nSaveGlobalValIdx := 0;

  for I := g_Config.nSaveGlobalAValIdx to High(g_Config.GlobalAVal) do
  begin
    if g_Config.OldGlobalAVal[I] <> g_Config.GlobalAVal[I] then
    begin
      try
        GlobalValConfig.WriteString('Setup', 'GlobalStrVal' + IntToStr(I), g_Config.GlobalAVal[I]);
        g_Config.OldGlobalAVal[I] := g_Config.GlobalAVal[I];
      except
        on E: Exception do
        begin
          MainOutMessage('[Exception] TFrmMain:SaveItemNumber 3');
          MainOutMessage(E.Message);
        end;
      end;

      if not g_boExitServer and (tick_diff(dwRunTick, MyGetTickCount) > 5) then
      begin
        g_Config.nSaveGlobalAValIdx := I;
        boProcessLimit := True;
        Break;
      end;
    end;
  end;

  if not boProcessLimit then
    g_Config.nSaveGlobalAValIdx := 0;

  try
    Config.WriteInteger('Setup', 'WinLotteryCount', g_Config.nWinLotteryCount);
    Config.WriteInteger('Setup', 'NoWinLotteryCount', g_Config.nNoWinLotteryCount);
    Config.WriteInteger('Setup', 'WinLotteryLevel1', g_Config.nWinLotteryLevel1);
    Config.WriteInteger('Setup', 'WinLotteryLevel2', g_Config.nWinLotteryLevel2);
    Config.WriteInteger('Setup', 'WinLotteryLevel3', g_Config.nWinLotteryLevel3);
    Config.WriteInteger('Setup', 'WinLotteryLevel4', g_Config.nWinLotteryLevel4);
    Config.WriteInteger('Setup', 'WinLotteryLevel5', g_Config.nWinLotteryLevel5);
    Config.WriteInteger('Setup', 'WinLotteryLevel6', g_Config.nWinLotteryLevel6);
  except
    on E: Exception do
    begin
      MainOutMessage('[Exception] TFrmMain:SaveItemNumber');
      MainOutMessage(E.Message);
    end;
  end;
end;

procedure TfrmMain.AddMemoLog(const sLog: string; nStyle: Integer; boAddTime: Boolean);
const
  arrForeColor: array [0 .. 4 - 1] of Cardinal = (clLime, clYellow, clRed, clLime);
  arrBackColor: array [0 .. 4 - 1] of Cardinal = ($00151515, clBlue, clBlue, $00151515);
var
  nLineIndex: Integer;
begin
  nLineIndex := memoLog.Lines.Count - 1;
  SendMessage(memoLog.Handle, EM_LINEINDEX, nLineIndex, 0);
  memoLog.SelStart := High(Integer);
  memoLog.SelLength := 0;
  memoLog.SelAttributes.Color := arrForeColor[nStyle and %00000011];
  memoLog.SelAttributes.BackColor := arrBackColor[nStyle and %00000011];

  if boAddTime then
    memoLog.Lines.Add(FormatDateTime(' > yyyy/mm/dd hh:nn:ss', Now) + sLog)
  else
    memoLog.Lines.Add(sLog);
end;

procedure TfrmMain.OnProgramException(Sender: TObject; E: Exception);
begin
  MainOutMessage(E.Message);
end;

procedure TfrmMain.ShowTimerTimer(Sender: TObject);
{$J+}
const
  GDATE: Double = 0.0;
{$J-}
var
  boWriteLog: Boolean;
  I: Integer;
  nRow: Integer;
  wHour: Word;
  wMinute: Word;
  wSecond: Word;
  tSecond: Integer;
  sSrvType: string;
  sVerType: string;
  GateInfo: pTGateInfo;
  LogFile: TextFile;
  s28: AnsiString;
  sSendBytes: TIdBytes;
  sText: string;
  SellPlayerInfo: PSellPlayerInfo;
  nStyleNo: Integer;
begin
  tmrShow.Interval := 2000;

  if GDATE <> Date then
  begin
    if GDATE <> 0.0 then
      RunSocket.ResetUserStatistics;

    GDATE := Date;
  end;

  if sCaptionExtText <> '' then
    Caption := Format('%s - [%s] [%s] %s', [g_sEngineProgramName, sCaption, sCaptionExtText, sCaptionExtText2])
  else
    Caption := Format('%s - [%s] %s', [g_sEngineProgramName, sCaption, sCaptionExtText2]);

  ClearMonDropLimitList;
  EnterCriticalSection(LogMsgCriticalSection);
  try
    boWriteLog := True;
    if MainLogMsgList.Count > 0 then
    begin
      try
        if not FileExists(sLogFileName) then
        begin
          AssignFile(LogFile, sLogFileName);
          Rewrite(LogFile);
        end
        else
        begin
          AssignFile(LogFile, sLogFileName);
          Append(LogFile);
        end;
        boWriteLog := False;
      except
        AddMemoLog('保存日志信息出错！', 3);
      end;
    end;

    while MainLogMsgList.Count > 0 do
    begin
      sText := MainLogMsgList[0];
      nStyleNo := Integer(MainLogMsgList.Objects[0]);
      MainLogMsgList.Delete(0);
      AddMemoLog(sText, nStyleNo, False);
      try
        if not boWriteLog then
          Writeln(LogFile, sText);
      except
      end;
    end;
    MainLogMsgList.Clear;

    if not boWriteLog then
      CloseFile(LogFile);

    for I := 0 to LogStringList.Count - 1 do
    begin
      try
        s28 := '1' + #9 + IntToStr(g_Config.nServerNumber) + #9 + IntToStr(nServerIndex) + #9 + LogStringList[I];
{$IFDEF UNICODE}
        SetLength(sSendBytes, Length(s28));
        Move(s28[1], sSendBytes[0], Length(s28));
        IdUDPClientLog.SendBuffer(sSendBytes);
{$ELSE}
        IdUDPClientLog.Send(s28);
{$ENDIF}
      except
        Continue;
      end;
    end;
    LogStringList.Clear;

    if LogonCostLogList.Count > 0 then
      WriteConLog(LogonCostLogList);
    LogonCostLogList.Clear;
  finally
    LeaveCriticalSection(LogMsgCriticalSection);
  end;

  if g_Config.nSellPlayerTime > 0 then
  begin
    for I := g_SellPlayerList.Count - 1 downto 0 do
    begin
      SellPlayerInfo := g_SellPlayerList.Items[I];
      if HoursBetween(Now, SellPlayerInfo.SellTime) >= g_Config.nSellPlayerTime then
        g_SellPlayerList.DeleteByIndex(I);
    end;
  end;

  sVerType := '[F]';
  sSrvType := '[M]';

  // 检查线程 运行时间
  tSecond := (MyGetTickCount() - g_dwStartTick) div 1000;
  wHour := tSecond div 3600;
  wMinute := (tSecond div 60) mod 60;
  wSecond := tSecond mod 60;

  LbRunTime.Caption := '引擎:' + IntToStr(wHour) + ':' + IntToStr(wMinute) + ':' + IntToStr(wSecond) + ' ' + sSrvType + sVerType;

  LbMonCount.Caption := '[刷怪:' + IntToStr(UserEngine.MonsterCount) + ']';

  LbUserCount.Caption := '[在线:' + IntToStr(UserEngine.OnlinePlayObject) + '/' + IntToStr(UserEngine.PlayObjectCount) + '/' +
    IntToStr(UserEngine.LoadPlayCount) + ']';

  Label1.Caption := 'Run:' + IntToStr(nRunTimeMin) + '/' + IntToStr(nRunTimeMax) + ' Soc:' + IntToStr(g_nSockCountMin) + '/' +
    IntToStr(g_nSockCountMax) + ' Usr:' + IntToStr(g_nUsrTimeMin) + '/' + IntToStr(g_nUsrTimeMax);

  Label2.Caption := 'Hum:' + IntToStr(g_nHumCountMin) + '/' + IntToStr(g_nHumCountMax) + ' Mon:' + IntToStr(g_nMonTimeMin) + '/' +
    IntToStr(g_nMonTimeMax) + ' UsrRot:' + IntToStr(dwUsrRotCountMin) + '/' + IntToStr(dwUsrRotCountMax) + ' Merch:' +
    IntToStr(UserEngine.dwProcessMerchantTimeMin) + '/' + IntToStr(UserEngine.dwProcessMerchantTimeMax) + ' Npc:' +
    IntToStr(UserEngine.dwProcessNpcTimeMin) + '/' + IntToStr(UserEngine.dwProcessNpcTimeMax) + ' (' +
    IntToStr(g_nProcessHumanLoopTime) + ')';

  Label5.Caption := 'Info:' + g_sMonGenInfo1 + ' - ' + g_sMonGenInfo2;

  Label20.Caption := 'MonG:' + IntToStr(g_nMonGenTime) + '/' + IntToStr(g_nMonGenTimeMin) + '/' + IntToStr(g_nMonGenTimeMax) +
    ' MonP:' + IntToStr(g_nMonProcTime) + '/' + IntToStr(g_nMonProcTimeMin) + '/' + IntToStr(g_nMonProcTimeMax) + ' ObjRun:' +
    IntToStr(g_nBaseObjTimeMin) + '/' + IntToStr(g_nBaseObjTimeMax);

  if g_nStartTimeTick = 0 then
  begin
    g_nStartTimeTick := GetTickCount64;
    g_nStartTime := 0;
  end
  else
    g_nStartTime := (GetTickCount64 - g_nStartTimeTick) div 1000;

  var
    nRunTicks: UInt64;

  nRunTicks := GetTickCount64;
  if nRunTicks >= (UInt64(36) * 24 * 60 * 60 * 1000) then
    LbTimeCount.Font.Color := clRed
  else
    LbTimeCount.Font.Color := clBlack;

  LbTimeCount.Caption := '[服务/本机:' + GetTickCountString(nRunTicks - g_nStartTimeTick) + '/' + GetTickCountString(nRunTicks) + ']';

  nRow := 1;
  if TryEnterCriticalSection(RunSocket.m_UserCriticalSection) then
  begin
    try
      for I := Low(g_GateArr) to High(g_GateArr) do
      begin
        GridGate.Cells[0, I + 1] := '';
        GridGate.Cells[1, I + 1] := '';
        GridGate.Cells[2, I + 1] := '';
        GridGate.Cells[3, I + 1] := '';
        GridGate.Cells[4, I + 1] := '';
        GridGate.Cells[5, I + 1] := '';
        GridGate.Cells[6, I + 1] := '';
        GateInfo := @g_GateArr[I];

        if GateInfo.boUsed and (GateInfo.Socket <> nil) then
        begin
          GridGate.Cells[0, nRow] := IntToStr(I);
          GridGate.Cells[1, nRow] := GateInfo.sAddr + ':' + IntToStr(GateInfo.nPort);
          GridGate.Cells[2, nRow] := IntToStr(GateInfo.nSendMsgCount);
          GridGate.Cells[3, nRow] := IntToStr(GateInfo.nSendedMsgCount);
          GridGate.Cells[4, nRow] := IntToStr(GateInfo.nSendRemainCount);

          if GateInfo.nSendMsgBytes < 1024 then
            GridGate.Cells[5, nRow] := IntToStr(GateInfo.nSendMsgBytes) + 'b'
          else if GateInfo.nSendMsgBytes < 1024 * 1024 then
            GridGate.Cells[5, nRow] := Format('%.2fKB', [GateInfo.nSendMsgBytes / 1024])
          else
            GridGate.Cells[5, nRow] := Format('%.2fMB', [GateInfo.nSendMsgBytes / 1024 / 1024]);

          GridGate.Cells[6, nRow] := IntToStr(GateInfo.nUserCount) + '/' + IntToStr(GateInfo.UserList.Count);
          Inc(nRow);
        end;
      end;
    finally
      LeaveCriticalSection(RunSocket.m_UserCriticalSection);
    end;
  end;

  Inc(nRunTimeMax);
  if g_nSockCountMax > 0 then
    Dec(g_nSockCountMax);
  if g_nUsrTimeMax > 0 then
    Dec(g_nUsrTimeMax);
  if g_nHumCountMax > 0 then
    Dec(g_nHumCountMax);
  if g_nMonTimeMax > 0 then
    Dec(g_nMonTimeMax);
  if dwUsrRotCountMax > 0 then
    Dec(dwUsrRotCountMax);
  if g_nMonGenTimeMin > 1 then
    Dec(g_nMonGenTimeMin, 2);
  if g_nMonProcTimeMin > 1 then
    Dec(g_nMonProcTimeMin, 2);
  if g_nBaseObjTimeMax > 0 then
    Dec(g_nBaseObjTimeMax);
end;

function LoadMapMagicEventList(const FileName: string): Boolean;
var
  I, J, K: Integer;
  S, S1, S2, S3, S4, S5, S6, S7, S8, S9, S10: string;
  SL: TStringList;
  Map: TEnvirnoment;
  nX, nY, nRange, nType, nPower, nAdditional, nMagicType, nAttackTime: Integer;
  boKeepVisible, boSmartAttack: Boolean;
  Additional: TAdditionalFeatures;
  GameEvent: TMapMagicGameEvent;
begin
  Result := True;
  if not FileExists(FileName) then
  begin
    SL := TStringList.Create;
    try
      SL.Add(';地图名称   X坐标    Y坐标   范围   魔法类型   魔法威力 永久可见 附加功能  智能攻击（0为固定坐标攻击。1为范围内坐标攻击） 攻击间隔(秒)');
      SL.SaveToFile(FileName);
    finally
      SL.Free;
    end;
    Exit;
  end;

  SL := TStringList.Create;
  try
    SL.LoadFromFile(FileName);
    for I := 0 to SL.Count - 1 do
    begin
      S := SL[I];
      if (Length(S) = 0) or (S[1] = ';') then
        Continue;

      S := GetValidStr3(S, S1, [' ', #9]);
      S := GetValidStr3(S, S2, [' ', #9]);
      S := GetValidStr3(S, S3, [' ', #9]);
      S := GetValidStr3(S, S4, [' ', #9]);
      S := GetValidStr3(S, S5, [' ', #9]);
      S := GetValidStr3(S, S6, [' ', #9]);
      S := GetValidStr3(S, S7, [' ', #9]);
      S := GetValidStr3(S, S8, [' ', #9]);
      S := GetValidStr3(S, S9, [' ', #9]);
      S := GetValidStr3(S, S10, [' ', #9]);
      if (Length(S1) = 0) or (Length(S2) = 0) or (Length(S3) = 0) then
        Continue;

      if S1[1] = '$' then
        S1 := Copy(S1, 2, MaxInt);

      if Length(S1) = 0 then
        Continue;

      Map := g_MapManager.FindMap(S1);
      if Map <> nil then
      begin
        nX := StrToIntDef(S2, 0);
        nY := StrToIntDef(S3, 0);
        nRange := StrToIntDef(S4, 0);
        nType := StrToIntDef(S5, 0);
        nPower := StrToIntDef(S6, 0);
        boKeepVisible := StrToIntDef(S7, 0) <> 0;;
        nAdditional := StrToIntDef(S8, 0);
        boSmartAttack := StrToIntDef(S9, 0) <> 0;
        nAttackTime := StrToIntDef(S10, 1);
        case nType of
          1:
            nMagicType := ET_DEDING;
          2 .. 7:
            nMagicType := ET_THUNDER2;
          8:
            nMagicType := ET_THUNDER;
          9:
            nMagicType := ET_FIREDRAGON2;
          10:
            nMagicType := ET_LAVA2;
          11:
            nMagicType := ET_LAVA;
          12:
            nMagicType := ET_FLASHLIGHT;
          15:
            nMagicType := ET_SPRINGS1;
          16:
            nMagicType := ET_SPRINGS2;
          17:
            nMagicType := ET_SPRINGS3;
          26:
            nMagicType := ET_DOOR1;
          27:
            nMagicType := ET_DOOR2;
          28:
            nMagicType := ET_DOOR3;
          29:
            nMagicType := ET_DOOR4;
          30:
            nMagicType := ET_DOOR5;
        else
          Continue;
        end;

        if (nAdditional < Integer(Low(TAdditionalFeatures))) or (nAdditional > Integer(High(TAdditionalFeatures))) then
          Additional := afNone
        else
          Additional := TAdditionalFeatures(nAdditional);

        if boKeepVisible then
        begin
          nRange := 0;
          boSmartAttack := False;
        end;

        if nMagicType in [ET_DOOR1 .. ET_DOOR5] then
          boSmartAttack := False;

        if nAttackTime <= 0 then
          nAttackTime := 1;

        if boKeepVisible then
        begin
          if Map.GetEvent(nX, nY) = nil then
          begin
            GameEvent := TMapMagicGameEvent.Create(Map, nX, nY, nMagicType);
            GameEvent.m_nDamage := nPower;
            GameEvent.FAdditional := Additional;
            GameEvent.KeepVisible := True;
            GameEvent.AttackTime := nAttackTime;
            g_EventManager.AddEvent(GameEvent);
          end;
        end
        else
        begin
          if boSmartAttack and (nRange > 1) then
          begin
            nRange := nRange - 1;
            for J := -nRange to nRange do
            begin
              for K := -nRange to nRange do
              begin
                if Map.GetEvent(nX + J, nY + K) = nil then
                begin
                  GameEvent := TMapMagicGameEvent.Create(Map, nX + J, nY + K, nMagicType);
                  GameEvent.m_nDamage := nPower;
                  GameEvent.FAdditional := Additional;
                  GameEvent.KeepVisible := False;
                  GameEvent.AttackTime := nAttackTime;
                  g_EventManager.AddEvent(GameEvent);
                end;
              end;
            end;
          end
          else if Map.GetEvent(nX, nY) = nil then
          begin
            GameEvent := TMapMagicGameEvent.Create(Map, nX, nY, nMagicType);
            GameEvent.m_nDamage := nPower;
            GameEvent.FAdditional := Additional;
            GameEvent.KeepVisible := False;
            GameEvent.AttackTime := nAttackTime;
            g_EventManager.AddEvent(GameEvent);
          end;
        end;
      end;
    end;
  finally
    SL.Free;
  end;
end;

procedure ProcessHumans();
begin
  UserEngine.ProcessDBResult;
  UserEngine.ProcessHumans;
  UserEngine.ProcessAuctionBroadcastList;
end;

procedure ProcessHeros();
begin
  UserEngine.ProcessHeros;
end;

procedure ProcessStatMapManCount();
begin
  UserEngine.ProcessStatMapManCount;
end;

procedure ProcessRegenMonsters();
begin
  UserEngine.ProcessRegenMonsters;
end;

procedure ProcessMonsters();
begin
  UserEngine.ProcessMonsters;
end;

procedure ProcessMerchants();
begin
  UserEngine.ProcessMerchants;
end;

procedure ProcessNpcs;
begin
  UserEngine.ProcessNpcs;
end;

procedure ProcessDataOther;
var
  sMsg: string;
begin
  if g_Config.boSendOnlineCount and (MyGetTickCount - g_dwSendOnlineTick > g_Config.dwSendOnlineTime) then
  begin
    g_dwSendOnlineTick := MyGetTickCount();
    sMsg := AnsiReplaceText(g_sSendOnlineCountMsg, '%c',
      IntToStr(Round(UserEngine.OnlinePlayObject * (g_Config.nSendOnlineCountRate / 10))));
    UserEngine.SendBroadCastMsg(sMsg, t_System)
  end;

  if tick_diff(UserEngine.dwProcessMissionsTime, MyGetTickCount()) > 1000 then
  begin
    UserEngine.dwProcessMissionsTime := MyGetTickCount();
    UserEngine.ProcessEvents();
    UserEngine.ProcessFBMap;
  end;

  if tick_diff(UserEngine.dwProcessMapDoorTick, MyGetTickCount()) > 500 then
  begin
    UserEngine.dwProcessMapDoorTick := MyGetTickCount();
    UserEngine.ProcessMapDoor();
  end;
end;

procedure ProcessMap();
var
  I: Integer;
  Envir: TEnvirnoment;
begin
  g_MapManager.Run;
  for I := g_GhostMapManager.Count - 1 downto 0 do
  begin
    Envir := g_GhostMapManager.Items[I];
    if tick_diff(Envir.m_dwInvalidTick, MyGetTickCount) >= 10 * 60000 then
    begin
      Envir.Free;
      g_GhostMapManager.Delete(I);
    end;
  end;
end;

procedure ProcessEvent();
begin
  g_EventManager.Run;
end;

procedure ProcessRobot();
begin
  RobotManage.Run;
end;

procedure ProcessItems();
begin
  g_ItemManager.Run;
end;

procedure ProcessOthers();
var
  I: Integer;
begin
  if tick_diff(l_dwRunTimeTick, MyGetTickCount) > 10000 then
  begin
    l_dwRunTimeTick := MyGetTickCount();
    g_GuildManager.Run;
    g_CastleManager.Run;
    g_DenySayMsgList.Lock;
    try
      for I := g_DenySayMsgList.Count - 1 downto 0 do
      begin
        if MyGetTickCount > Cardinal(g_DenySayMsgList.Objects[I]) then
          g_DenySayMsgList.Delete(I);
      end;
    finally
      g_DenySayMsgList.UnLock;
    end;
  end;
end;

procedure ProcessGameRun();
var
  I: Integer;
  Envir: TEnvirnoment;
begin
  EnterCriticalSection(ProcessHumanCriticalSection);
  try
    UserEngine.PrcocessData;
    g_MapManager.Run;
    for I := g_GhostMapManager.Count - 1 downto 0 do
    begin
      Envir := g_GhostMapManager.Items[I];
      if tick_diff(Envir.m_dwInvalidTick, MyGetTickCount) >= 10 * 60000 then
      begin
        Envir.Free;
        g_GhostMapManager.Delete(I);
      end;
    end;

    g_EventManager.Run;
    RobotManage.Run;
    g_ItemManager.Run;
    if tick_diff(l_dwRunTimeTick, MyGetTickCount) > 10000 then
    begin
      l_dwRunTimeTick := MyGetTickCount();
      g_GuildManager.Run;
      g_CastleManager.Run;
      g_DenySayMsgList.Lock;
      try
        for I := g_DenySayMsgList.Count - 1 downto 0 do
        begin
          if MyGetTickCount > Cardinal(g_DenySayMsgList.Objects[I]) then
            g_DenySayMsgList.Delete(I);
        end;
      finally
        g_DenySayMsgList.UnLock;
      end;
    end;
  finally
    LeaveCriticalSection(ProcessHumanCriticalSection);
  end;
end;

procedure TfrmMain.StartTimerTimer(Sender: TObject);
  procedure InitScript;
  begin
    if g_ManageNPC <> nil then
    begin
      try
        g_ManageNPC.GotoLable(TPlayObject(g_GrobalPlayer), '@Startup', False);
      except
        MainOutMessage('[Exception] Startup');
      end;
    end;
  end;

var
  nCode: Integer;
  sFileName: string;
  SL: TStringList;
{$IF NEED_KEY = 1}
{$IF MULTI_THREAD = 1}
  KeyInfo: TKeyInfo;
  Hash, EncKey: DWORD;
  I, Len, ExtendedInfo: Integer;
  HWID, LincHWID: array [0 .. 100] of AnsiChar;
  UserName: array [0 .. 100] of AnsiChar;
  Organization: array [0 .. 100] of AnsiChar;
  CustomData: array [0 .. 102400] of AnsiChar;
  MachineID: array [0 .. 7] of DWORD;
  IsOK: Boolean;
  C1, C2: AnsiChar;
  S, TempStr, Password: AnsiString;
{$IFEND}
{$IFEND}
begin
  g_M2Lime_RunTime := 36 * 60 * 60000 + Random(43200000);
  SendGameCenterMsg(SG_STARTNOW, '正在启动游戏主程序...');
  StartTimer.Enabled := False;
  FrmDB := TFrmDB.Create();
  StartService();
  g_PluginManager.HookEngineReadyToStart;
  g_StartRunTime := Now();
  try
    if SizeOf(THumData) <> SIZEOFTHUMAN then
    begin
      ShowMessage('SizeOf(THumData) ' + IntToStr(SizeOf(THumData)) + ' <> SizeOf(THumData) ' + IntToStr(SIZEOFTHUMAN) + ' ' +
        IntToHex(SizeOf(THumData), 4));
      Close;
      Exit;
    end;

    if not LoadClientFile then
    begin
      Close;
      Exit;
    end;

    if g_boUseSqliteDB then
    begin
      if not FileExists(g_sSqliteDBName) then
      begin
        MainOutMessage('Sqlite数据库 ' + g_sSqliteDBName + ' 不存在！', False);
        Exit;
      end;

      FrmDB.SQLiteDB.Database := g_sSqliteDBName;
      FrmDB.SQLiteDB.Connected := True;
    end
    else
    begin
{$IF DBTYPE = BDE}
{$IFNDEF CPUX64}
      FrmDB.Query.DatabaseName := g_sDBName;
{$ELSE}
      MainOutMessage('[错误]***64位引擎不支持BDE数据库***', False);
      Exit;
{$ENDIF}
{$ELSE}
      FrmDB.Query.ConnectionString := g_sADODBString;
{$IFEND}
    end;
    LoadGameLogItemNameList();
    LoadDenyIPLocalList();
    LoadDenyIPAddrList();
    LoadDenyAccountList();
    LoadDenyChrNameList();
    LoadDenyMachineIDList();
    LoadNoClearMonList();
    LoadDummyNameList();
    LoadDummyHeroNameList();
    LoadNameFilterList();
    if g_nKey_PreviewMonItem = 1 then
      LoadPreviewItemMonList();

    g_ItemEffects.LoadFromFile;
    RebuildDropItemsEffect;
    LoadInputBoxFilterList;
    LoadPayInfo();
{$IF LUA_SCRIPT = 1}
    MainOutMessage('正在初始化lua脚本系统...');
    LoadLuaScript();
    MainOutMessage('初始化lua脚本初始化成功.');
{$IFEND}
    nCode := FrmDB.LoadItemsDB(False);
    if nCode < 0 then
    begin
      MainOutMessage('物品数据库加载失败！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage(Format('物品数据库加载成功(%d).', [UserEngine.StdItemList.Count]));

    LoadMonSuperAbilList;

    nCode := FrmDB.LoadMonsterDB;
    if nCode < 0 then
    begin
      MainOutMessage('加载怪物数据库失败！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage(Format('加载怪物数据库成功(%d).', [UserEngine.MonsterList.Count]));

    nCode := FrmDB.LoadMagicDB;
    if nCode < 0 then
    begin
      MainOutMessage('加载技能数据库失败！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage(Format('加载技能数据库成功(%d).', [UserEngine.m_MagicList.Count]));

    if g_boUseSqliteDB then
      FrmDB.SQLiteDB.Connected := False;

    LoadEffectImageList();
    LoadEffectItemList();

    nCode := FrmDB.LoadMinMap;
    if nCode < 0 then
    begin
      MainOutMessage('小地图数据加载失败！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage('小地图数据加载成功.');

    nCode := FrmDB.LoadMapInfo;
    if nCode < 0 then
    begin
      MainOutMessage('地图数据加载失败！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage(Format('地图数据加载成功(%d).', [g_MapManager.Count]));

    nCode := FrmDB.LoadMonGen;
    if nCode < 0 then
    begin
      MainOutMessage('加载怪物刷新配置信息失败！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage(Format('加载怪物刷新配置信息成功(%d).', [UserEngine.m_MonGenList.Count]));

    LoadMonSayMsg();
    MainOutMessage(Format('加载怪物说话配置信息成功(%d).', [g_MonSayMsgList.Count]));
    LoadMonHPProgress();
    MainOutMessage(Format('加载怪物大血条成功(%d).', [g_MonSayMsgList.Count]));
    g_SndaShopList.LoadFromFile;
    MainOutMessage(Format('商铺列表加载成功(%d).', [g_SndaShopList.RecordCount]));
    g_BoxsList.LoadFromFile;
    MainOutMessage(Format('宝箱列表加载成功(%d).', [g_BoxsList.Count]));
    FrmDB.LoadMonFireDragonGuard();
    g_FilterTexts.LoadFromFile;
    LoadLowestSellingPriceList;
    LoadHighestSellingPriceList;
    g_ItemRules.LoadFromFile;
    g_UserCmds.LoadFromFile;
    g_GroupItems.LoadFromFile;
    g_CustomHeroMagicMgr.LoadFromFile;
    sFileName := g_Config.sEnvirDir + 'UserData';
    if not DirectoryExists(sFileName) then
      CreateDir(sFileName);

    sFileName := sFileName + '\UserData.dat';
    g_GameGoldDealDB.LoadFormFile(sFileName);
    sFileName := ExtractFilePath(Application.ExeName) + 'PlugClient';
    if not DirectoryExists(sFileName) then
      CreateDir(sFileName);

    LoadPlugClientFiles();
    LoadClientModules();
    LoadClientBlackModules();
    LoadCustomMoney();
    LoadDisableTakeOffList();
    LoadDisableShowItemFromList();
    LoadMonDropLimitList();
    LoadDisableMakeItem();
    LoadEnableMakeItem();
    LoadMoveGuardPickItem();
    LoadDisableMoveMap;
    LoadDummyDisableMoveMap;
    LoadDummyNoActiveAttackMonList();
    LoadDisableSendMsgList();
    LoadItemBindIPaddr();
    LoadItemBindAccount();
    LoadItemBindCharName();
    LoadUnMasterList();
    LoadUnForceMasterList();
    LoadEnablePickUpItem();
    LoadPriorityPickUpItem();
    LoadRememberItemList();
    LoadDisableRangePickItem();
    LoadDisableDropToBagItem();
    LoadMissionPageCaptionList();
    LoadPlayMonsterConfigList();
    LoadFoundryItemList();
    LoadFilterItemList;
    LoadItemDescList;
    LoadItemDescTopList;
    LoadTzItemDescList;
    LoadCustomItemPropertyTextVarList;
    LoadVerifyCodeChrs;
    MainOutMessage('正在加载国家信息列表...');
    g_NationManage.LoadConfig;
    LoadParalysisItemList;
    LoadMagicShieldItemList;
    LoadRevivalItemList;
    LoadMDParalysisItemList;
    LoadFrozenItemList;
    LoadCobwebWindingItemList;
    MainOutMessage('正在加载物品技能威力列表...');
    LoadSkillPowerItemList;
    LoadArcherGuardPKMonList;
    LoadPoisonWeaponList;
    LoadDisableMoveMapMonToPos;
    MainOutMessage('正在加宠物配置列表...');
    LoadGamePetsConfig;

    nCode := FrmDB.LoadUnbindList;
    if nCode < 0 then
    begin
      MainOutMessage('加载捆装物品信息失败！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage('加载捆装物品信息成功.');

    LoadBindItemTypeFromUnbindList();

    nCode := FrmDB.LoadMapQuest;
    if nCode < 0 then
    begin
      MainOutMessage('加载任务地图信息失败！');
      Exit;
    end;
    MainOutMessage('加载任务地图信息成功.');

    nCode := FrmDB.LoadMapEvent;
    if nCode < 0 then
    begin
      MainOutMessage('加载地图触发事件信息失败！');
      Exit;
    end;
    MainOutMessage('加载地图触发事件信息成功.');

    sFileName := g_Config.sEnvirDir + 'UserData\MapMagicEvent.txt';
    if LoadMapMagicEventList(sFileName) then
      MainOutMessage('地图魔法触发事件信息加载成功.');

    if LoadAbuseInformation('.\!abuse.txt') then
      MainOutMessage('加载文字过滤信息成功.');

    if not LoadLineNotice(g_Config.sNoticeDir + 'LineNotice.txt') then
      MainOutMessage('加载公告提示信息失败！')
    else
      MainOutMessage('加载公告提示信息成功.');

    try
      FrmDB.LoadAdminList();
      MainOutMessage('管理员列表加载成功.');
    except
      MainOutMessage('管理员列表加载失败！');
    end;

    g_GuildManager.LoadGuildInfo();
    MainOutMessage('行会列表加载成功.');

    g_CastleManager.LoadCastleList();
    MainOutMessage('城堡列表加载成功.');

    g_CastleManager.Initialize;
    MainOutMessage('城堡城初始成功.');

    if g_nDataSaveDBType = 0 then
      g_M2DataDB := TSqliteM2DataDB.Create
    else
      g_M2DataDB := TMySqlM2DataDB.Create;

    g_M2DataDB.Init;
    StartEngine();
    boStartReady := True;
    Sleep(500);
    LoadClientItemList();
    LoadSpecialCmdList();
    g_dwRunTick := MyGetTickCount();
    g_PluginManager.HookEngineStartComplete;
    g_dwRunCount := 0;
    g_dwUsrRotCountTick := MyGetTickCount();
    InitScript;
    RunTimer.Enabled := True;
    LoadDefCombatPowerConfig;
    Reset_g_WarrContinueMagicIDList;

    if (g_FunctionNPC = nil) then
      MainOutMessage('未能创建QF NPC，请确认地图0已定义', False, RVSTYLE_ERROR);

    g_SellPlayerList.AutoLoadSellPlayer;
    sFileName := g_Config.sEnvirDir + 'SellPlayerInfo.txt';
    if FileExists(sFileName) then
    begin
      SL := TStringList.Create;
      try
        SL.LoadFromFile(sFileName);
        g_SellPlayerInfoText := StringReplace(SL.Text, sLineBreak, '', [rfReplaceAll]);
      finally
        SL.Free;
      end;
    end;

    SendGameCenterMsg(SG_STARTOK, '游戏主程序启动成功.');
    GateSocket.Address := g_Config.sGateAddr;
    GateSocket.Port := g_Config.nGatePort;
    g_GateSocket := GateSocket;
    SendGameCenterMsg(SG_CHECKCODEADDR, IntToStr(NativeInt(@g_CheckCode)));
    RebuildSendServerConfigText;
    ResetMagicCDList;

    g_GuildManager.SaveGuildVariable;
{$IF MULTI_THREAD = 1}
    g_M2RunThreadMgr.AddThread('人物处理', ProcessHumans);
    g_M2RunThreadMgr.AddThread('英雄处理', ProcessHeros);
    g_M2RunThreadMgr.AddThread('刷怪处理', ProcessRegenMonsters);
    g_M2RunThreadMgr.AddThread('智能刷怪', ProcessStatMapManCount);
    g_M2RunThreadMgr.AddThread('怪物处理', ProcessMonsters);
    g_M2RunThreadMgr.AddThread('NPC', ProcessMerchants);
    g_M2RunThreadMgr.AddThread('NPC处理', ProcessNpcs);
    g_M2RunThreadMgr.AddThread('其他数据处理', ProcessDataOther);
    g_M2RunThreadMgr.AddThread('地图处理', ProcessMap);
    g_M2RunThreadMgr.AddThread('事件处理', ProcessEvent);
    g_M2RunThreadMgr.AddThread('机器人', ProcessRobot);
    g_M2RunThreadMgr.AddThread('物品处理', ProcessItems);
    g_M2RunThreadMgr.AddThread('其他处理', ProcessOthers);
{$IFEND}
    if g_M2RunThreadMgr.Count = 0 then
      tmrRun.Enabled := True;

    if not Assigned(frmFunctionConfig) then
      frmFunctionConfig := TfrmFunctionConfig.Create(Application);
{$IFNDEF DEBUG}
    TThread.CreateAnonymousThread(SendRunData).Start; // 运行数据上报 Cursor 2023-05-31 09:40:50
{$ENDIF}
  except
    on E: Exception do
      MainOutMessage('服务器启动异常！' + E.Message);
  end;
end;

procedure TfrmMain.StartEngine();
var
  nCode: Integer;
begin
  try
{$IF IDSOCKETMODE = TIMERENGINE}
    FrmIDSoc.Initialize;
    MainOutMessage('登录服务器连接初始化成功.');
{$IFEND}
    g_MapManager.LoadMapDoor;
    MainOutMessage('地图环境加载成功.');

    MakeStoneMines();
    MainOutMessage('矿物数据初始成功.');

    nCode := FrmDB.LoadMerchant;
    if nCode < 0 then
    begin
      MainOutMessage('交易NPC列表加载错误！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage('交易NPC列表加载成功.');

    if not g_Config.boVentureServer then
    begin
      nCode := FrmDB.LoadGuardList;
      if nCode < 0 then
        MainOutMessage('守卫列表加载错误！' + 'Code: ' + IntToStr(nCode));
      MainOutMessage('守卫列表加载成功.');
    end;

    nCode := FrmDB.LoadNpcs;
    if nCode < 0 then
    begin
      MainOutMessage('管理NPC列表加载错误！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage('管理NPC列表加载成功.');

    nCode := FrmDB.LoadMakeItem;
    if nCode < 0 then
    begin
      MainOutMessage('炼制物品信息加载错误！' + 'Code: ' + IntToStr(nCode));
      Exit;
    end;
    MainOutMessage('炼制物品信息加载成功.');

    nCode := FrmDB.LoadStartPoint;
    if nCode < 0 then
    begin
      MainOutMessage('加载回城点配置时出现错误！(错误码: ' + IntToStr(nCode) + ')');
      Close;
    end;
    MainOutMessage('回城点配置加载成功.');

    UserEngine.Initialize;
    MainOutMessage('角色数据处理引擎启动成功.');

    g_MapManager.MakeSafePkZone;
    g_MapManager.MakeMapMagic;
    DataEngine.Active := True;
    MENU_MANAGE_PLUG_ENABLED.Enabled := True;
    MENU_TOOLS_SCRIPT_EDITOR.Enabled := True;
    MainOutMessage('游戏处理引擎初始化成功.');
  except
    on E: Exception do
      MainOutMessage('服务启动时出现异常错误！' + E.Message);
  end;
end;

procedure TfrmMain.MENU_TOOLS_M2_DIRClick(Sender: TObject);
begin
  ShellExecute(Application.Handle, PChar('explore'), PChar(ExtractFilePath(ParamStr(0))), nil, nil, SW_SHOWNORMAL);
end;

procedure TfrmMain.MakeStoneMines();
begin
end;

function TfrmMain.LoadClientFile(): Boolean;
begin
  Result := True;
end;

procedure TfrmMain.FormCreate(Sender: TObject);
var
  nX, nY: Integer;
  I: Integer;
resourcestring
  sGateIdx = '网关';
  sGateIPaddr = '网关地址';
  sGateListMsg = '队列数据';
  sGateSendCount = '发送数据';
  sGateMsgCount = '剩余数据';
  sGateSendKB = '平均流量';
  sGateUserCount = '最高人数';
begin
  FIsEmbeddedGameCenter := False;
  g_GrobalPlayer := TPlayObject.Create;
  g_GrobalPlayer.m_boSuperMan := True;
  g_GrobalPlayer.m_boDummyObject := True;
  g_GrobalPlayer.m_sCharName := 'M2System';
  for I := 0 to Length(TPlayObject(g_GrobalPlayer).m_nLockUpdateItems) - 1 do
    TPlayObject(g_GrobalPlayer).m_nLockUpdateItems[I] := 255;

{$IF MULTI_THREAD = 1}
  g_MultiThreadRun := True;
{$IFEND}
  g_nRegisteredKey := 1;
  g_nKey_HeroExt := 1;
  g_nKey_MobileSMS := 1;
  g_nKey_Trading := 1;
  g_nKey_Auction := 1;
  g_nKey_CustomSafeArea := 1;
  g_nKey_DropLimitExt := 1;
  g_nKey_SellPlayer := 1;
  g_nKey_UseClientPickItems := 1;
  g_nKey_PreviewMonItem := 1;
  Randomize;
  g_dwGameCenterHandle := StrToInt64Def(ParamStr(1), 0);
  nX := StrToIntDef(ParamStr(2), -1);
  nY := StrToIntDef(ParamStr(3), -1);

  if (nX >= 0) or (nY >= 0) then
  begin
    Left := nX;
    Top := nY;
  end;

  SendGameCenterMsg(SG_FORMHANDLE, IntToStr(Self.Handle));
  Application.OnModalBegin := OnAppModalBegin;
  Application.OnModalEnd := OnAppOnModalEnd;
  Application.OnMessage := OnApplicationMessage;
  sCaptionExtText2 := '';
  tmrShow.OnTimer := nil;
  RunTimer.OnTimer := nil;
  StartTimer.OnTimer := nil;
  SaveVariableTimer.OnTimer := nil;
  CloseTimer.OnTimer := nil;
  GridGate.RowCount := 21;
  GridGate.Cells[0, 0] := sGateIdx;
  GridGate.Cells[1, 0] := sGateIPaddr;
  GridGate.Cells[2, 0] := sGateListMsg;
  GridGate.Cells[3, 0] := sGateSendCount;
  GridGate.Cells[4, 0] := sGateMsgCount;
  GridGate.Cells[5, 0] := sGateSendKB;
  GridGate.Cells[6, 0] := sGateUserCount;

  GridGate.ColWidths[0] := MulDiv(35, FCurrentPPI, 96);
  GridGate.ColWidths[1] := MulDiv(150, FCurrentPPI, 96);
  GridGate.ColWidths[2] := MulDiv(75, FCurrentPPI, 96);
  GridGate.ColWidths[3] := MulDiv(75, FCurrentPPI, 96);
  GridGate.ColWidths[4] := MulDiv(75, FCurrentPPI, 96);
  GridGate.ColWidths[5] := MulDiv(75, FCurrentPPI, 96);
  GridGate.ColWidths[6] := MulDiv(75, FCurrentPPI, 96);

  GateSocket := TServerSocket.Create(Owner);
  GateSocket.OnClientConnect := GateSocketClientConnect;
  GateSocket.OnClientDisconnect := GateSocketClientDisconnect;
  GateSocket.OnClientError := GateSocketClientError;
  GateSocket.OnClientRead := GateSocketClientRead;
  GateSocket.OnClientWrite := GateSocketClientWrite;

  tmrShow.OnTimer := ShowTimerTimer;
  RunTimer.OnTimer := RunTimerTimer;
  StartTimer.OnTimer := StartTimerTimer;
  SaveVariableTimer.OnTimer := SaveVariableTimerTimer;
  CloseTimer.OnTimer := CloseTimerTimer;

  g_DropLimitMgr := TDropLimitManager.Create;
  g_DropLimitMgr.LoadConfig;
  g_SellPlayerList := TSellPlayerList.Create;
  g_SellPlayerList.LoadConfig;
  g_CombatPowerVarMgr := TCombatPowerVarMgr.Create;
  g_CombatPowerVarMgr.LoadConfig;
  StartTimer.Enabled := True;

{$IFDEF DEBUG}
  LabelVersion.Font.Color := clRed;
{$ELSE}
  LabelVersion.Font.Color := clBlue;
{$ENDIF}
  TGIFImage(imgGif.Picture.Graphic).AnimationSpeed := 150;
  TGIFImage(imgGif.Picture.Graphic).Animate := True;
end;

procedure TfrmMain.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
resourcestring
  sCloseServerYesNo = '是否确认关闭游戏服务器？';
  sCloseServerTitle = '确认信息';
begin
  if not boServiceStarted then
    Exit;

  if g_boExitServer then
  begin
    boStartReady := False;
    Exit;
  end;

  CanClose := False;
  if Application.MessageBox(PChar(sCloseServerYesNo), PChar(sCloseServerTitle), MB_YESNO + MB_ICONQUESTION) = mrYes then
  begin
    g_boExitServer := True;
    CloseGateSocket();
    g_Config.boKickAllUser := True;
    CloseTimer.Enabled := True;
  end;
end;

procedure TfrmMain.CloseTimerTimer(Sender: TObject);
resourcestring
  sCloseServer = '%s [正在关闭服务器(%d/%s %d)...]';
  sCloseServer1 = '%s [服务器已关闭]';
var
  sCapExt: string;
begin
  sCapExt := Format(sCloseServer, ['人物', UserEngine.OnlineRealPlayObject, '数据', DataEngine.GetWaitSaveHumanCount]);
  Caption := Format('%s - %s', [g_sEngineProgramName, sCapExt]);
  SendGameCenterMsg(SG_ACTIVE, '');

  if (UserEngine.OnlineRealPlayObject = 0) or (not DataEngine.Connected) then
  begin
    if DataEngine.IsIdle or (not DataEngine.Connected) then
    begin
      CloseTimer.Enabled := False;
      sCapExt := Format(sCloseServer1, [g_Config.sServerName]);
      Caption := Format('%s - %s', [g_sEngineProgramName, sCapExt]);
      StopService;
      FrmDB.Free;
      Close;
    end;
  end;
end;

procedure TfrmMain.SaveVariableTimerTimer(Sender: TObject);
begin
  SaveItemNumber();
end;

procedure TfrmMain.GateSocketClientError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
  var ErrorCode: Integer);
begin
  RunSocket.CloseErrGate(Socket, ErrorCode);
end;

procedure TfrmMain.GateSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
begin
  RunSocket.CloseGate(Socket);
end;

procedure TfrmMain.GateSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
var
  sRemoteAdd: string;
begin
  sRemoteAdd := Socket.RemoteAddress;
  if g_ServerTableList.IndexOf(sRemoteAdd) < 0 then
  begin
    if g_boShowBlockIPLog then
      MainOutMessage('拒绝未授权IP连接服务器：' + sRemoteAdd + '；【授权IP文件：!servertable.txt】');

    Socket.Close;
    Exit;
  end;
  RunSocket.AddGate(Socket);
end;

procedure TfrmMain.GateSocketClientRead(Sender: TObject; Socket: TCustomWinSocket);
begin
  RunSocket.SocketRead(Socket);
end;

procedure TfrmMain.GateSocketClientWrite(Sender: TObject; Socket: TCustomWinSocket);
begin
  RunSocket.SocketWrite(Socket);
end;

procedure TfrmMain.RunTimerTimer(Sender: TObject);
var
  SystemTime: TSystemTime;
begin
  GetLocalTime(SystemTime);
  g_nGameTime := g_Config.BrightConfig[SystemTime.wHour];
  if boStartReady then
  begin
    FrmIDSoc.Run;
    UserEngine.Execute;

    if g_M2RunThreadMgr.Count = 0 then
      ProcessGameRun();

    g_M2DataDB.Run;
    if (g_nKey_Auction <> 0) and (tick_diff(g_M2DataDB.AuctionDB.RunTick2, MyGetTickCount) >= 60000) then
      g_M2DataDB.AuctionDB.Run;

    if tick_diff(g_M2DataDB.UserShopDB.RunTick2, MyGetTickCount) >= 60000 then
      g_M2DataDB.UserShopDB.Run;
  end;

  Inc(g_dwRunCount);
  if tick_diff(g_dwRunTick, MyGetTickCount) >= 1000 then
  begin
    g_dwRunTick := MyGetTickCount();
    nRunTimeMin := g_dwRunCount;

    if nRunTimeMax > nRunTimeMin then
      nRunTimeMax := nRunTimeMin;

    g_dwRunCount := 0;
  end;

  if boStartReady then
  begin
    if not boRemoteOpenGateSocketed then
    begin
      boRemoteOpenGateSocketed := True;
      try
        if Assigned(g_GateSocket) then
          g_GateSocket.Active := True;
      except
        on E: Exception do
          MainOutMessage(E.Message);
      end;
    end;
  end;
end;

procedure TfrmMain.ReloadConfig(Sender: TObject);
begin
  LoadConfig();
  FrmIDSoc.Timer1Timer(Sender);
  IdUDPClientLog.Host := g_Config.sLogServerAddr;
  IdUDPClientLog.Port := g_Config.nLogServerPort;
  LoadServerTable();
  LoadClientFile();
  RebuildSendServerConfigText;
end;

procedure TfrmMain.MemoLogDblClick(Sender: TObject);
begin
  ClearMemoLog();
end;

procedure TfrmMain.MENU_CONTROL_EXITClick(Sender: TObject);
begin
  Close;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_CONFClick(Sender: TObject);
begin
  ReloadConfig(Sender);
  g_PluginManager.HookEngineReloadComplete(9);
end;

procedure TfrmMain.MENU_CONTROL_CLEARLOGMSGClick(Sender: TObject);
begin
  ClearMemoLog();
end;

procedure TfrmMain.SpeedButton1Click(Sender: TObject);
begin
  ReloadConfig(Sender);
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_ITEMDBClick(Sender: TObject);
begin
  FrmDB.LoadItemsDB();
  if LoadBindItemTypeFromUnbindList() then
    UserEngine.SendUnbindList();

  UserEngine.SendStdItemList;
  MainOutMessage('重新加载物品数据库成功.');
  g_PluginManager.HookEngineReloadComplete(1);
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_MAGICDBClick(Sender: TObject);
begin
  if g_boUseSqliteDB then
    FrmDB.SQLiteDB.Connected := True;

  FrmDB.LoadMagicDB();
  if g_boUseSqliteDB then
    FrmDB.SQLiteDB.Connected := False;

  EnterCriticalSection(ProcessHumanCriticalSection);
  try
    UserEngine.ReloadMagicList();
    UserEngine.ReloadHeroMagicList();
  finally
    LeaveCriticalSection(ProcessHumanCriticalSection);
  end;

  MainOutMessage('重新加载技能数据库成功.');
  g_PluginManager.HookEngineReloadComplete(2);
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_MONSTERDBClick(Sender: TObject);
begin
  if g_boUseSqliteDB then
    FrmDB.SQLiteDB.Connected := True;

  FrmDB.LoadMonsterDB();
  if g_boUseSqliteDB then
    FrmDB.SQLiteDB.Connected := False;

  MainOutMessage('重新加载怪物数据库成功.');
  g_PluginManager.HookEngineReloadComplete(3);
end;

procedure TfrmMain.StartService;
var
  TimeNow: TDateTime;
  Year, Month, Day, Hour, Min, Sec, MSec: Word;
  F: TextFile;
  Config: pTConfig;
  sDate: string;
  SC: TSearchRec;
  SL: TStringList;
  IsChanged: Boolean;
begin
  Config := @g_Config;
  FillChar(Config.LevelExpRates, SizeOf(Config.LevelExpRates), 0);
  sDate := IntToStr(CLIENT_VERSION_NUMBER - 100000000);
  Year := StrToInt(Copy(sDate, 1, 4));
  Month := StrToInt(Copy(sDate, 5, 2));
  Day := StrToInt(Copy(sDate, 7, 2));
  dM2ServerVersionDate := EncodeDate(Year, Month, Day);
  nRunTimeMax := 99999;
  g_nSockCountMax := 0;
  g_nUsrTimeMax := 0;
  g_nHumCountMax := 0;
  g_nMonTimeMax := 0;
  g_nMonGenTimeMax := 0;
  g_nMonProcTime := 0;
  g_nMonProcTimeMin := 0;
  g_nMonProcTimeMax := 0;
  dwUsrRotCountMin := 0;
  dwUsrRotCountMax := 0;
  g_nProcessHumanLoopTime := 0;
  g_dwHumLimit := 30;
  g_dwMonLimit := 30;
  g_dwZenLimit := 5;
  g_dwNpcLimit := 5;
  g_dwSocLimit := 10;
  nDecLimit := 20;
  Config.sDBSocketRecvText := '';
  Config.nLoadDBErrorCount := 0;
  Config.nLoadDBCount := 0;
  Config.nSaveDBCount := 0;
  Config.nDBQueryID := 0;
  Config.nItemNumber := 0;
  Config.nItemNumberEx := High(Integer) div 2;
  boStartReady := False;
  g_boExitServer := False;
  boFilterWord := True;
  Config.nWinLotteryCount := 0;
  Config.nNoWinLotteryCount := 0;
  Config.nWinLotteryLevel1 := 0;
  Config.nWinLotteryLevel2 := 0;
  Config.nWinLotteryLevel3 := 0;
  Config.nWinLotteryLevel4 := 0;
  Config.nWinLotteryLevel5 := 0;
  Config.nWinLotteryLevel6 := 0;
  FillChar(g_Config.GlobalVal, SizeOf(g_Config.GlobalVal), #0);
  FillChar(g_Config.GlobaDyMval, SizeOf(g_Config.GlobaDyMval), #0);
  FillChar(g_Config.GlobalAVal, SizeOf(g_Config.GlobalAVal), #0);
  LoadConfig();
  LoadSendSMSConfig();
  nServerIndex := 0;
  g_Config.ReceiveBuffer := TMemoryStreamEx.Create;
  g_PluginManager := TPluginManager.Create;
  RunSocket := TRunSocket.Create();
  MainLogMsgList := TStringList.Create;
  LogStringList := TStringList.Create;
  LogonCostLogList := TStringList.Create;
  g_MapManager := TMapManager.Create;
  g_FBMapManager := TGStringList.Create;
  g_GhostMapManager := TList.Create;
  ItemUnit := TItemUnit.Create;
  MagicManager := TMagicManager.Create;
  NoticeManager := TNoticeManager.Create;
  g_GuildManager := TGuildManager.Create;
  g_EventManager := TEventManager.Create;
  g_CastleManager := TCastleManager.Create;
  g_ItemManager := TItemManager.Create;
  DataEngine := TDataEngine.Create;
  UserEngine := TUserEngine.Create();
  RobotManage := TRobotManage.Create;
  g_MakeItemList := TStringList.Create;
  g_SafeAreaManager := TSafeAreaManager.Create;
  g_ServerTableList := TStringList.Create;
  g_DenySayMsgList := TQuickList.Create;
  MiniMapList := TStringList.Create;
  g_UnbindList := TList.Create;
  LineNoticeList := TStringList.Create;
  ItemEventList := TStringList.Create;
  AbuseTextList := TStringList.Create;
  g_MonSayMsgList := TStringList.Create;
  g_MonHPProgressList := TStringList.Create;
  g_DisableMakeItemList := TGStringList.Create;
  g_EnableMakeItemList := TGStringList.Create;
  g_DisableMoveMapList := TGStringList.Create;
  g_DummyDisableMoveMapList := TGStringList.Create;
  g_DummyNoActiveAttackMonList := TGStringList.Create;
  g_ItemNameList := TGList.Create;
  g_DisableSendMsgList := TGStringList.Create;
  g_MonDropLimitList := TGStringList.Create;
  g_DisableTakeOffList := TSortItemList.Create;
  g_DisableShowItemFromList := TSortItemList.Create;
  g_MoveGuardPickItemList := TGStringList.Create;
  g_PreviewItemMonList := TGStringList.Create;
  g_UnMasterList := TGStringList.Create;
  g_UnMasterList.NameValueSeparator := #9;
  g_UnForceMasterList := TGStringList.Create;
  g_UnForceMasterList.NameValueSeparator := #9;
  g_GameLogItemNameList := TGStringList.Create;
  g_DenyIPAddrList := TGStringList.Create;
  g_DenyChrNameList := TGStringList.Create;
  g_DenyAccountList := TGStringList.Create;
  g_NoClearMonList := TGStringList.Create;
  g_DenyIPLocalList := TGStringList.Create;
  g_DenyMachineIDList := TGStringList.Create;
  g_ItemBindIPaddr := TGList.Create;
  g_ItemBindAccount := TGList.Create;
  g_ItemBindCharName := TGList.Create;
  g_NameFilterList := TGStringList.Create;
  g_InputBoxFilterList := TGStringList.Create;
  g_MapEventListOfDropItem := TList.Create;
  g_MapEventListOfPickUpItem := TList.Create;
  g_MapEventListOfMine := TList.Create;
  g_MapEventListOfDoMine := TList.Create;
  g_MapEventListOfWalk := TList.Create;
  g_MapEventListOfRun := TList.Create;
  g_MapEventListOfScatterItem := TList.Create;
  g_MapEventListOfHorseWalk := TList.Create;
  g_MapEventListOfHorseRun := TList.Create;
  g_EnablePickUpItemList := TGStringList.Create;
  g_PriorityPickUpItemList := TGStringList.Create;
  g_DisableRangePickItemList := TGStringList.Create;
  g_DisableDropToBagItemList := TGStringList.Create;
  g_RememberItemList := TQuickList.Create;
  g_MissionPageCaptionList := TStringList.Create;
  g_ItemDescList := TGStringList.Create;
  g_ItemDescTopList := TGStringList.Create;
  g_TzItemDescList := TGStringList.Create;
  g_CustomItemPropertyTextVarList := TStringList.Create;
  g_NationManage := TNationManage.Create;
  g_ParalysisItemList := TStringList.Create;
  g_MagicShieldItemList := TStringList.Create;
  g_RevivalItemList := TStringList.Create;
  g_SkillPowerItemList := TGStringList.Create;
  g_MDParalysisItemList := TStringList.Create;
  g_FrozenItemList := TStringList.Create;
  g_CobwebWindingItemList := TStringList.Create;
  g_ArcherGuardPKList := TGStringList.Create;
  g_MonSuperAbilList := TGStringList.Create;
  g_PoisonWeaponList := TGStringList.Create;
  g_LowestSellingPriceList := TGStringList.Create;
  g_HighestSellingPriceList := TGStringList.Create;
  g_DummyNameList := TGStringList.Create;
  g_DummyHeroNameList := TGStringList.Create;
  g_DisableMoveMapMonToPos := TGStringList.Create;
  g_HumanRankList := TGStringList.Create;
  g_WarriorRankList := TGStringList.Create;
  g_WizardRankList := TGStringList.Create;
  g_TaoistRankList := TGStringList.Create;
  g_SndaShopList := TSndaShopList.Create;
  g_FilterTexts := TFilterTexts.Create;
  g_ItemRules := TItemRules.Create;
  g_UserCmds := TUserCmds.Create;
  g_GroupItems := TGroupItems.Create;
  g_ItemEffects := TItemEffects.Create;
  g_BoxsList := TBoxsList.Create;
  g_FindPath := TFindPath.Create;
  g_GameGoldDealDB := TGameGoldDealDB.Create;
  g_PlugClientList := TList.Create;
  g_ModuleList := TList.Create;
  g_CustomMoneyList := TList.Create;
  g_BlackModuleList := TList.Create;
  InitializeCriticalSection(LogMsgCriticalSection);
  InitializeCriticalSection(ProcessMsgCriticalSection);
  InitializeCriticalSection(ProcessHumanCriticalSection);
  InitializeCriticalSection(Config.UserIDSection);
  InitializeCriticalSection(UserDBSection);
  g_DynamicVarList := TList.Create;
  g_NpcTextFilesCache := TStringList.Create;
  g_WarrContinueMagicIDList := TStringList.Create;
  g_AliyunSendSMSThread := TAliyunSendSMSThread.Create;
  g_PayInfoList := TStringList.Create;
  TimeNow := Now();
  DecodeDate(TimeNow, Year, Month, Day);
  DecodeTime(TimeNow, Hour, Min, Sec, MSec);
  if not DirectoryExists(g_Config.sLogDir) then
    CreateDir(Config.sLogDir);

  sLogFileName := g_Config.sLogDir + IntToStr(Year) + '-' + IntToStr2(Month) + '-' + IntToStr2(Day) + '.' + IntToStr2(Hour) + '-'
    + IntToStr2(Min) + '.txt';

  AssignFile(F, sLogFileName);
  Rewrite(F);
  CloseFile(F);
  g_PluginManager.LoadPluginList();
  MainOutMessage('正在处理物品规则日志...');
  SL := TStringList.Create;
  try
    if FindFirst(g_Config.sItemDropLogDir + '\*.txt', faAnyFile, SC) = 0 then
    begin
      repeat
        SL.LoadFromFile(g_Config.sItemDropLogDir + SC.Name);
        IsChanged := False;
        while SL.Count > 100 do
        begin
          SL.Delete(0);
          IsChanged := True;
        end;

        if IsChanged then
          SL.SaveToFile(g_Config.sItemDropLogDir + SC.Name);
      until FindNext(SC) <> 0;
    end;
  finally
    SL.Free;
  end;

  MainOutMessage('正在读取配置信息...');
  nShiftUsrDataNameNo := 1;
  Caption := Format('%s - [%s]', [g_sEngineProgramName, g_Config.sServerName]);
  sCaption := g_Config.sServerName;
  LoadServerTable();
  IdUDPClientLog.Host := g_Config.sLogServerAddr;
  IdUDPClientLog.Port := g_Config.nLogServerPort;
  Application.OnException := OnProgramException;
  dwRunDBTimeMax := MyGetTickCount();
  g_dwStartTick := MyGetTickCount();
  tmrShow.Enabled := True;
  boServiceStarted := True;
{$IFDEF USE_VMP} VMProtectBeginUltra('TFrmMain.StartService.Version'); {$ENDIF}
  LabelVersion.Caption := '版本发布日期：' + FormatDateTime('yyyymmdd', g_BuildTime);
{$IFDEF USE_VMP} VMProtectEnd(); {$ENDIF}
end;

procedure TfrmMain.StopService;
var
  List: TList;
  I, II: Integer;
  Config: pTConfig;
  Envir: TEnvirnoment;
  MonSuperAbil: pTMonSuperAbil;
  PoisonWeapon: pTPoisonWeapon;
  UnbindItemInfo: PTUnbindItemInfo;
begin
  try
    Config := @g_Config;
    tmrShow.Enabled := False;
    RunTimer.Enabled := False;
    tmrRun.Enabled := False;
    g_M2RunThreadMgr.Clear;
    FrmIDSoc.Close;
    g_EventManager.Free;
    g_ItemManager.Free;
    g_CastleManager.Free;
    g_MapManager.Free;

    for I := g_GhostMapManager.Count - 1 downto 0 do
    begin
      Envir := g_GhostMapManager.Items[I];
      Envir.Free;
    end;
    g_GhostMapManager.Free;

    for I := 0 to g_FBMapManager.Count - 1 do
      g_FBMapManager.Objects[I].Free;
    g_FBMapManager.Free;

    g_PluginManager.Free;
    g_PluginManager := nil;
    GateSocket.Close;
    SaveItemNumber();
    MagicManager.Free;
    UserEngine.Free;
    RobotManage.Free;
    RunSocket.Free;
    DataEngine.Terminate;
    DataEngine.WaitFor;
    DataEngine.Free;
    FreeAndNil(MainLogMsgList);
    FreeAndNil(LogStringList);
    FreeAndNil(LogonCostLogList);
    ItemUnit.Free;
    NoticeManager.Free;
    g_GuildManager.Free;
    for I := 0 to g_MakeItemList.Count - 1 do
      TStringList(g_MakeItemList.Objects[I]).Free;
    FreeAndNil(g_MakeItemList);

    for I := 0 to LineNoticeList.Count - 1 do
      Dispose(pTNoticeColor(LineNoticeList.Objects[I]));
    FreeAndNil(LineNoticeList);

    FreeAndNil(g_ServerTableList);
    FreeAndNil(g_DenySayMsgList);
    FreeAndNil(MiniMapList);
    for I := 0 to g_UnbindList.Count - 1 do
    begin
      UnbindItemInfo := g_UnbindList.Items[I];
      Dispose(UnbindItemInfo);
    end;
    FreeAndNil(g_UnbindList);
    FreeAndNil(ItemEventList);
    FreeAndNil(AbuseTextList);

    for I := 0 to g_MonSayMsgList.Count - 1 do
    begin
      List := TList(g_MonSayMsgList.Objects[I]);
      if Assigned(List) then
      begin
        for II := 0 to List.Count - 1 do
          Dispose(pTMonSayMsg(List.Items[II]));
        List.Free;
      end;
    end;
    g_MonSayMsgList.Clear;

    FreeAndNil(g_MonSayMsgList);
    for I := 0 to g_MonHPProgressList.Count - 1 do
      Dispose(pTMonHPProgress(g_MonHPProgressList.Objects[I]));
    FreeAndNil(g_MonHPProgressList);

    FreeAndNil(g_DisableMakeItemList);
    FreeAndNil(g_EnableMakeItemList);
    FreeAndNil(g_DisableMoveMapList);
    FreeAndNil(g_DummyDisableMoveMapList);
    FreeAndNil(g_DummyNoActiveAttackMonList);
    FreeAndNil(g_ItemNameList);
    FreeAndNil(g_DisableSendMsgList);
    for I := 0 to g_MonDropLimitList.Count - 1 do
      Dispose(pTMonDrop(g_MonDropLimitList.Objects[I]));
    FreeAndNil(g_MonDropLimitList);

    FreeAndNil(g_PreviewItemMonList);
    FreeAndNil(g_DisableTakeOffList);
    FreeAndNil(g_DisableShowItemFromList);
    FreeAndNil(g_MoveGuardPickItemList);
    FreeAndNil(g_UnMasterList);
    FreeAndNil(g_UnForceMasterList);
    FreeAndNil(g_GameLogItemNameList);
    FreeAndNil(g_DenyIPLocalList);
    FreeAndNil(g_DenyIPAddrList);
    FreeAndNil(g_DenyChrNameList);
    FreeAndNil(g_DenyAccountList);
    FreeAndNil(g_NoClearMonList);
    FreeAndNil(g_EnablePickUpItemList);
    FreeAndNil(g_PriorityPickUpItemList);
    FreeAndNil(g_DenyMachineIDList);
    FreeAndNil(g_DisableRangePickItemList);
    FreeAndNil(g_DisableDropToBagItemList);
    for I := 0 to g_ItemBindIPaddr.Count - 1 do
      Dispose(pTItemBind(g_ItemBindIPaddr.Items[I]));
    FreeAndNil(g_ItemBindIPaddr);

    for I := 0 to g_ItemBindAccount.Count - 1 do
      Dispose(pTItemBind(g_ItemBindAccount.Items[I]));
    FreeAndNil(g_ItemBindAccount);

    for I := 0 to g_ItemBindCharName.Count - 1 do
      Dispose(pTItemBind(g_ItemBindCharName.Items[I]));
    FreeAndNil(g_ItemBindCharName);
    FreeAndNil(g_NameFilterList);
    FreeAndNil(g_InputBoxFilterList);
    for I := 0 to g_MapEventListOfDropItem.Count - 1 do
      Dispose(pTMapEvent(g_MapEventListOfDropItem.Items[I]));
    FreeAndNil(g_MapEventListOfDropItem);

    for I := 0 to g_MapEventListOfPickUpItem.Count - 1 do
      Dispose(pTMapEvent(g_MapEventListOfPickUpItem.Items[I]));
    FreeAndNil(g_MapEventListOfPickUpItem);

    for I := 0 to g_MapEventListOfMine.Count - 1 do
      Dispose(pTMapEvent(g_MapEventListOfMine.Items[I]));
    FreeAndNil(g_MapEventListOfMine);

    for I := 0 to g_MapEventListOfDoMine.Count - 1 do
      Dispose(pTMapEvent(g_MapEventListOfDoMine.Items[I]));
    FreeAndNil(g_MapEventListOfDoMine);

    for I := 0 to g_MapEventListOfWalk.Count - 1 do
      Dispose(pTMapEvent(g_MapEventListOfWalk.Items[I]));
    FreeAndNil(g_MapEventListOfWalk);

    for I := 0 to g_MapEventListOfRun.Count - 1 do
      Dispose(pTMapEvent(g_MapEventListOfRun.Items[I]));
    FreeAndNil(g_MapEventListOfRun);

    for I := 0 to g_MapEventListOfScatterItem.Count - 1 do
      Dispose(pTMapEvent(g_MapEventListOfScatterItem.Items[I]));
    FreeAndNil(g_MapEventListOfScatterItem);

    for I := 0 to g_MapEventListOfHorseWalk.Count - 1 do
      Dispose(pTMapEvent(g_MapEventListOfHorseWalk.Items[I]));
    FreeAndNil(g_MapEventListOfHorseWalk);

    for I := 0 to g_MapEventListOfHorseRun.Count - 1 do
      Dispose(pTMapEvent(g_MapEventListOfHorseRun.Items[I]));
    FreeAndNil(g_MapEventListOfHorseRun);

    for I := 0 to g_DynamicVarList.Count - 1 do
      Dispose(pTDynamicVar(g_DynamicVarList.Items[I]));
    FreeAndNil(g_DynamicVarList);

    if g_BindItemTypeList <> nil then
    begin
      for I := 0 to g_BindItemTypeList.Count - 1 do
        Dispose(pTUnBindItem(g_BindItemTypeList.Items[I]));
      FreeAndNil(g_BindItemTypeList);
    end;

    g_SndaShopList.Free;
    g_FilterTexts.Free;
    g_ItemRules.Free;
    g_UserCmds.Free;
    g_ItemEffects.Free;
    g_GroupItems.Free;
    g_FindPath.Free;
    g_BoxsList.Free;
    g_GameGoldDealDB.Free;
    if g_RememberItemList <> nil then
    begin
      for I := 0 to g_RememberItemList.Count - 1 do
        Dispose(pTItemEvent(g_RememberItemList.Objects[I]));
      FreeAndNil(g_RememberItemList);
    end;

    FreeAndNil(g_MissionPageCaptionList);
    FreeAndNil(g_DummyNameList);
    FreeAndNil(g_DummyHeroNameList);
    FreeAndNil(g_ParalysisItemList);

    for I := 0 to g_MagicShieldItemList.Count - 1 do
      Dispose(pTShieldItemInfo(g_MagicShieldItemList.Objects[I]));
    FreeAndNil(g_MagicShieldItemList);

    FreeAndNil(g_MDParalysisItemList);
    FreeAndNil(g_FrozenItemList);
    FreeAndNil(g_CobwebWindingItemList);

    for I := 0 to g_RevivalItemList.Count - 1 do
      Dispose(pTRevivalItemInfo(g_RevivalItemList.Objects[I]));
    FreeAndNil(g_RevivalItemList);

    for I := 0 to g_SkillPowerItemList.Count - 1 do
      Dispose(pTSkillPowerItem(g_SkillPowerItemList.Objects[I]));
    FreeAndNil(g_SkillPowerItemList);

    FreeAndNil(g_ArcherGuardPKList);

    for I := 0 to g_MonSuperAbilList.Count - 1 do
    begin
      MonSuperAbil := pTMonSuperAbil(g_MonSuperAbilList.Objects[I]);
      Dispose(MonSuperAbil);
    end;
    FreeAndNil(g_MonSuperAbilList);

    for I := 0 to g_PoisonWeaponList.Count - 1 do
    begin
      PoisonWeapon := pTPoisonWeapon(g_PoisonWeaponList.Objects[I]);
      Dispose(PoisonWeapon);
    end;
    FreeAndNil(g_PoisonWeaponList);

    g_DisableMoveMapMonToPos.Free;
    ClearLowestSellingPriceList;
    ClearHighestSellingPriceList;
    g_LowestSellingPriceList.Free;
    g_HighestSellingPriceList.Free;
    g_HumanRankList.Free;
    g_WarriorRankList.Free;
    g_WizardRankList.Free;
    g_TaoistRankList.Free;
    UnLoadFoundryItemList();
    UnLoadPlayMonsterConfigList();
    UnLoadEffectImageList();
    UnLoadEffectItemList();
    UnLoadClientItemList();
    UnLoadSpecialCmdList();
    UnLoadFilterItemList;
    g_ItemDescTopList.Free;
    g_ItemDescList.Free;
    g_TzItemDescList.Free;
    g_CustomItemPropertyTextVarList.Free;
    g_NationManage.Free;

    for I := 0 to g_NpcTextFilesCache.Count - 1 do
      g_NpcTextFilesCache.Objects[I].Free;
    g_NpcTextFilesCache.Free;

    g_WarrContinueMagicIDList.Free;
    g_AliyunSendSMSThread.Terminate;
    g_AliyunSendSMSThread.WaitFor;
    g_AliyunSendSMSThread.Free;
    SavePayInfo();
    for I := 0 to g_PayInfoList.Count - 1 do
      Dispose(pTPayInfo(g_PayInfoList.Objects[I]));
    g_PayInfoList.Free;

    for I := 0 to g_PlugClientList.Count - 1 do
      Dispose(pTPlugClientInfo(g_PlugClientList.Items[I]));
    g_PlugClientList.Free;

    for I := 0 to g_ModuleList.Count - 1 do
      Dispose(pTModuleInfo(g_ModuleList.Items[I]));
    g_ModuleList.Free;

    for I := 0 to g_BlackModuleList.Count - 1 do
      Dispose(pTModuleInfo(g_BlackModuleList.Items[I]));
    g_BlackModuleList.Free;

    for I := 0 to g_CustomMoneyList.Count - 1 do
      Dispose(pTCustomMoney(g_CustomMoneyList.Items[I]));
    g_CustomMoneyList.Free;

    g_Config.ReceiveBuffer.Free;
    DeleteCriticalSection(LogMsgCriticalSection);
    DeleteCriticalSection(ProcessMsgCriticalSection);
    DeleteCriticalSection(ProcessHumanCriticalSection);
    DeleteCriticalSection(Config.UserIDSection);
    DeleteCriticalSection(UserDBSection);
    g_SafeAreaManager.Free;
    g_M2DataDB.final;
    g_M2DataDB.Free;
{$IF LUA_SCRIPT = 1}
    UnLoadLuaScript();
{$IFEND}
    boServiceStarted := False;
  except
    on E: Exception do
      ShowMessage('错误信息:' + E.Message);
  end;
end;

procedure TfrmMain.MENU_HELP_ABOUTClick(Sender: TObject);
var
  FrmAbout: TForm;
  gpVersion: TGroupBox;
  lbTemp: TLabel;
begin
  FrmAbout := TForm.Create(nil);
  try
{$IFDEF USE_VMP}
    VMProtectBeginUltra('svMain.OpenAbout'); {$ENDIF}
    FrmAbout.BorderStyle := bsDialog;
    FrmAbout.Position := poMainFormCenter;
    FrmAbout.Font.Charset := GB2312_CHARSET;
    FrmAbout.Font.Name := '宋体';
    FrmAbout.Font.Size := 9;
{$IF CompilerVersion >= 22}
    FrmAbout.Width := MulDiv(400, FCurrentPPI, 96);
    FrmAbout.Height := MulDiv(330, FCurrentPPI, 96);
{$ELSE}
    FrmAbout.Width := 400;
    FrmAbout.Height := 330;
{$IFEND}
    FrmAbout.Caption := '关于';
    gpVersion := TGroupBox.Create(FrmAbout);
    gpVersion.Parent := FrmAbout;
    gpVersion.Caption := '版本信息';
{$IF CompilerVersion >= 22}
    gpVersion.SetBounds(MulDiv(8, FCurrentPPI, 96), MulDiv(8, FCurrentPPI, 96), MulDiv(368, FCurrentPPI, 96),
      MulDiv(126, FCurrentPPI, 96));
{$ELSE}
    gpVersion.SetBounds(8, 8, 368, 126);
{$IFEND}
    lbTemp := TLabel.Create(FrmAbout);
    lbTemp.Parent := gpVersion;
{$IF CompilerVersion >= 22}
    lbTemp.Left := MulDiv(8, FCurrentPPI, 96);
    lbTemp.Top := MulDiv(16, FCurrentPPI, 96);
{$ELSE}
    lbTemp.Left := 8;
    lbTemp.Top := 16;
{$IFEND}
    lbTemp.Caption := '软件名称: GxxM2';// + string(PowerBase64.DecryptAndDecodeBase64String(g_sEngineDesc));
    lbTemp := TLabel.Create(FrmAbout);
    lbTemp.Parent := gpVersion;
{$IF CompilerVersion >= 22}
    lbTemp.Left := MulDiv(8, FCurrentPPI, 96);
    lbTemp.Top := MulDiv(34, FCurrentPPI, 96);
{$ELSE}
    lbTemp.Left := 8;
    lbTemp.Top := 34;
{$IFEND}
    lbTemp.Caption := '软件版本: ' + g_Version;
    lbTemp := TLabel.Create(FrmAbout);
    lbTemp.Parent := gpVersion;
{$IF CompilerVersion >= 22}
    lbTemp.Left := MulDiv(8, FCurrentPPI, 96);
    lbTemp.Top := MulDiv(52, FCurrentPPI, 96);
{$ELSE}
    lbTemp.Left := 8;
    lbTemp.Top := 52;
{$IFEND}
    // lbTemp.Caption := '更新日期: ' + System.AnsiStrings.RightStr
    // (Format(PowerBase64.DecryptAndDecodeBase64String(g_sEngineVersionDesc), [0]), 8);
    // lbTemp := TLabel.Create(FrmAbout);
    // lbTemp.Parent := gpVersion;
{$IF CompilerVersion >= 22}
    lbTemp.Left := MulDiv(8, FCurrentPPI, 96);
    lbTemp.Top := MulDiv(70, FCurrentPPI, 96);
{$ELSE}
    lbTemp.Left := 8;
    lbTemp.Top := 70;
{$IFEND}
    lbTemp.Caption := '程序制作: GxxM2'; //+ PowerBase64.DecryptAndDecodeBase64String(g_sEngineName);
    lbTemp := TLabel.Create(FrmAbout);
    lbTemp.Parent := gpVersion;
{$IF CompilerVersion >= 22}
    lbTemp.Left := MulDiv(8, FCurrentPPI, 96);
    lbTemp.Top := MulDiv(88, FCurrentPPI, 96);
{$ELSE}
    lbTemp.Left := 8;
    lbTemp.Top := 88;
{$IFEND}
    lbTemp.Caption := '程序网站: http://www.gxxm2.com';// + PowerBase64.DecryptAndDecodeBase64String(g_sEngineWebSite);
    lbTemp := TLabel.Create(FrmAbout);
    lbTemp.Parent := gpVersion;
{$IF CompilerVersion >= 22}
    lbTemp.Left := MulDiv(8, FCurrentPPI, 96);
    lbTemp.Top := MulDiv(106, FCurrentPPI, 96);
{$ELSE}
    lbTemp.Left := 8;
    lbTemp.Top := 106;
{$IFEND}
    lbTemp.Caption := '程序论坛: http://www.gxxm2.com';// + PowerBase64.DecryptAndDecodeBase64String(g_sEngineWebSite);
    gpVersion := TGroupBox.Create(FrmAbout);
    gpVersion.Parent := FrmAbout;
    gpVersion.Caption := '版权声明';
{$IF CompilerVersion >= 22}
    gpVersion.SetBounds(MulDiv(8, FCurrentPPI, 96), MulDiv(156, FCurrentPPI, 96), MulDiv(368, FCurrentPPI, 96),
      MulDiv(95, FCurrentPPI, 96));
{$ELSE}
    gpVersion.SetBounds(8, 156, 368, 95);
{$IFEND}
    lbTemp := TLabel.Create(FrmAbout);
    lbTemp.Parent := gpVersion;
{$IF CompilerVersion >= 22}
    lbTemp.Left := MulDiv(10, FCurrentPPI, 96);
    lbTemp.Top := MulDiv(16, FCurrentPPI, 96);
{$ELSE}
    lbTemp.Left := 10;
    lbTemp.Top := 16;
{$IFEND}
    lbTemp.Caption := '本计算机程序受中华人民共和国知识产权与版权保护，如未经授权' + sLineBreak + '而擅自复制或传播本程序（或其中任何部分），将受到严厉的民事' + sLineBreak +
      '及刑事制裁，并在法律许可的范围内受到最大可能的起诉。';
    lbTemp := TLabel.Create(FrmAbout);
    lbTemp.Parent := gpVersion;
{$IF CompilerVersion >= 22}
    lbTemp.Left := MulDiv(10, FCurrentPPI, 96);
    lbTemp.Top := MulDiv(60, FCurrentPPI, 96);
{$ELSE}
    lbTemp.Left := 10;
    lbTemp.Top := 60;
{$IFEND}
    lbTemp.Font.Color := clRed;
    lbTemp.Caption := '本程序只适用于中华人民共和国法律允许范围内的个人娱乐，不得' + sLineBreak + '用于商业盈利性经营，如因此造成的后果自负与本软件无关。';
    with TButton.Create(FrmAbout) do
    begin
      Parent := FrmAbout;
      Caption := '确定(&O)';
      ModalResult := mrOK;
{$IF CompilerVersion >= 22}
      SetBounds(MulDiv(300, FCurrentPPI, 96), MulDiv(256, FCurrentPPI, 96), MulDiv(75, FCurrentPPI, 96),
        MulDiv(25, FCurrentPPI, 96));
{$ELSE}
      SetBounds(300, 256, 75, 25);
{$IFEND}
    end;
{$IFDEF USE_VMP} VMProtectEnd(); {$ENDIF}
    FrmAbout.ShowModal;
  finally
    FrmAbout.Free;
  end;
end;

procedure TfrmMain.MENU_OPTION_SERVERCONFIGClick(Sender: TObject);
begin
  FrmServerValue := TFrmServerValue.Create(nil);
  try
    FrmServerValue.Top := Self.Top + 20;
    FrmServerValue.Left := Self.Left;
    FrmServerValue.AdjuestServerConfig();
  finally
    FrmServerValue.Free;
  end;
end;

procedure TfrmMain.MENU_OPTION_GENERALClick(Sender: TObject);
begin
  frmGeneralConfig := TfrmGeneralConfig.Create(nil);
  try
    frmGeneralConfig.Open();
  finally
    frmGeneralConfig.Free;
  end;
end;

procedure TfrmMain.MENU_OPTION_GAMEClick(Sender: TObject);
begin
  frmGameConfig := TfrmGameConfig.Create(nil);
  try
    frmGameConfig.Open;
  finally
    frmGameConfig.Free;
  end;
end;

procedure TfrmMain.MENU_OPTION_FUNCTIONClick(Sender: TObject);
begin
  if not Assigned(frmFunctionConfig) then
    Exit;

  frmFunctionConfig.DoOpen;
end;

procedure TfrmMain.MENU_OPTION_COMMANDClick(Sender: TObject);
begin
  frmGameCmd := TfrmGameCmd.Create(nil);
  try
    frmGameCmd.Open;
  finally
    frmGameCmd.Free;
  end;
end;

procedure TfrmMain.MENU_OPTION_MONSTERClick(Sender: TObject);
var
  frmMonsterConfig: TfrmMonsterConfig;
begin
  frmMonsterConfig := TfrmMonsterConfig.Create(nil);
  try
    frmMonsterConfig.Open;
  finally
    frmMonsterConfig.Free;
  end;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_MONSTERSAYClick(Sender: TObject);
begin
  UserEngine.ClearMonSayMsg();
  LoadMonSayMsg();
  MainOutMessage('重新加载怪物说话配置成功.');
  g_PluginManager.HookEngineReloadComplete(4);
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_DISABLEMAKEClick(Sender: TObject);
begin
  LoadMonSuperAbilList;
  LoadDisableTakeOffList();
  LoadDisableShowItemFromList();
  LoadDisableMakeItem();
  LoadEnableMakeItem();
  LoadMoveGuardPickItem();
  LoadDisableMoveMap();
  LoadDummyDisableMoveMap();
  LoadDummyNoActiveAttackMonList();
  LoadDisableSendMsgList();
  LoadGameLogItemNameList();
  LoadItemBindIPaddr();
  LoadItemBindAccount();
  LoadItemBindCharName();
  LoadUnMasterList();
  LoadUnForceMasterList();
  LoadDenyIPLocalList();
  LoadDenyIPAddrList();
  LoadDenyAccountList();
  LoadDenyChrNameList();
  LoadDenyMachineIDList();
  LoadNoClearMonList();
  LoadEnablePickUpItem();
  LoadPriorityPickUpItem();
  LoadDisableRangePickItem();
  LoadDisableDropToBagItem();
  FrmDB.LoadAdminList();
  LoadFilterItemList;
  LoadItemDescList;
  LoadItemDescTopList;
  LoadTzItemDescList;
  LoadCustomItemPropertyTextVarList;
  LoadParalysisItemList;
  LoadMagicShieldItemList;
  LoadRevivalItemList;
  LoadMDParalysisItemList;
  LoadFrozenItemList;
  LoadCobwebWindingItemList;
  LoadSkillPowerItemList;
  LoadArcherGuardPKMonList;
  LoadPoisonWeaponList;
  UserEngine.SendFilterItemList;
  UserEngine.SendItemDescList;
  UserEngine.SendItemDescTopList;
  UserEngine.SendTzItemDescList;
  MainOutMessage('重新加载列表配置成功.');
  g_PluginManager.HookEngineReloadComplete(7);
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_STARTPOINTClick(Sender: TObject);
begin
  FrmDB.LoadStartPoint();
  MainOutMessage('重新地图安全区列表成功.');
  g_PluginManager.HookEngineReloadComplete(8);
end;

procedure TfrmMain.MENU_CONTROL_GATE_OPENClick(Sender: TObject);
resourcestring
  sGatePortOpen = '游戏网关端口(%s:%d)已打开.';
begin
  if not GateSocket.Active then
  begin
    GateSocket.Active := True;
    MainOutMessage(Format(sGatePortOpen, [GateSocket.Address, GateSocket.Port]));
  end;
end;

procedure TfrmMain.MENU_CONTROL_GATE_CLOSEClick(Sender: TObject);
begin
  CloseGateSocket();
end;

procedure TfrmMain.CloseGateSocket;
var
  I: Integer;
resourcestring
  sGatePortClose = '游戏网关端口(%s:%d)已关闭.';
begin
  if GateSocket.Active then
  begin
    for I := 0 to GateSocket.Socket.ActiveConnections - 1 do
      GateSocket.Socket.Connections[I].Close;

    GateSocket.Active := False;
    MainOutMessage(Format(sGatePortClose, [GateSocket.Address, GateSocket.Port]));
  end;
end;

procedure TfrmMain.mniControlClick(Sender: TObject);
begin
  if GateSocket.Active then
  begin
    MENU_CONTROL_GATE_OPEN.Enabled := False;
    MENU_CONTROL_GATE_CLOSE.Enabled := True;
  end
  else
  begin
    MENU_CONTROL_GATE_OPEN.Enabled := True;
    MENU_CONTROL_GATE_CLOSE.Enabled := False;
  end;
end;

procedure TfrmMain.MENU_VIEW_SESSIONClick(Sender: TObject);
begin
  frmViewSession := TfrmViewSession.Create(nil);
  try
    frmViewSession.Open();
  finally
    frmViewSession.Free;
  end;
end;

procedure TfrmMain.MENU_VIEW_ONLINEHUMANClick(Sender: TObject);
begin
  frmViewOnlineHuman := TfrmViewOnlineHuman.Create(nil);
  try
    frmViewOnlineHuman.Open();
  finally
    frmViewOnlineHuman.Free;
  end;
end;

procedure TfrmMain.MENU_VIEW_LEVELClick(Sender: TObject);
begin
  frmViewLevel := TfrmViewLevel.Create(nil);
  try
    frmViewLevel.Open();
  finally
    frmViewLevel.Free;
  end;
end;

procedure TfrmMain.MENU_VIEW_LISTClick(Sender: TObject);
begin
  frmViewList := TfrmViewList.Create(nil);
  try
    frmViewList.Open();
  finally
    frmViewList.Free;
  end;
end;

procedure TfrmMain.SetMenu;
begin
  frmMain.Menu := MainMenu;
end;

procedure TfrmMain.MENU_VIEW_KERNELINFOClick(Sender: TObject);
begin
  frmViewKernelInfo := TfrmViewKernelInfo.Create(nil);
  try
    frmViewKernelInfo.Open();
  finally
    frmViewKernelInfo.Free;
  end;
end;

procedure TfrmMain.MENU_TOOLS_MERCHANTClick(Sender: TObject);
begin
  frmConfigMerchant := TfrmConfigMerchant.Create(nil);
  try
    frmConfigMerchant.Open();
  finally
    frmConfigMerchant.Free;
  end;
end;

procedure TfrmMain.MENU_OPTION_ITEMFUNCClick(Sender: TObject);
begin
  frmItemSet := TfrmItemSet.Create(nil);
  try
    frmItemSet.Open();
  finally
    frmItemSet.Free;
  end;
end;

procedure TfrmMain.ClearMemoLog;
begin
  if Application.MessageBox('是否确定清除日志信息！', '提示信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
    memoLog.Clear;
end;

procedure TfrmMain.MENU_TOOLS_MONGENClick(Sender: TObject);
begin
  frmConfigMonGen := TfrmConfigMonGen.Create(nil);
  try
    frmConfigMonGen.Open();
  finally
    frmConfigMonGen.Free;
  end;
end;

procedure TfrmMain.MyMessage(var MsgData: TWmCopyData);
var
  sData: AnsiString;
  wIdent: Word;
begin
  wIdent := HiWord(MsgData.From);
  sData := PAnsiChar(MsgData.CopyDataStruct^.lpData);

  if wIdent = GS_QUIT then
  begin
    g_boExitServer := True;
    CloseGateSocket();
    g_Config.boKickAllUser := True;
    CloseTimer.Enabled := True;
  end;
end;

procedure TfrmMain.MENU_MANAGE_CASTLEClick(Sender: TObject);
begin
  frmCastleManage := TfrmCastleManage.Create(nil);
  try
    frmCastleManage.Open();
  finally
    frmCastleManage.Free;
  end;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_QFUNCTIONClick(Sender: TObject);
begin
  if g_FunctionNPC <> nil then
  begin
    g_FunctionNPC.ClearScript;
    g_FunctionNPC.LoadNpcScript;
    MainOutMessage('QFunction 脚本加载成功.');
    g_PluginManager.HookEngineReloadComplete(12);
  end;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_QMISSIONClick(Sender: TObject);
begin
  if g_MissionNPC <> nil then
  begin
    g_MissionNPC.ClearScript;
    g_MissionNPC.LoadNpcScript;
    MainOutMessage('QMission 脚本加载成功.');
    g_PluginManager.HookEngineReloadComplete(13);
  end;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_QMANGEClick(Sender: TObject);
begin
  if g_ManageNPC <> nil then
  begin
    g_ManageNPC.ClearScript;
    g_ManageNPC.LoadNpcScript;
    MainOutMessage('QManage 脚本加载成功.');
    g_PluginManager.HookEngineReloadComplete(11);
  end;
end;

procedure TfrmMain.MENU_VIEW_LIST2Click(Sender: TObject);
begin
  frmViewList2 := TfrmViewList2.Create(nil);
  try
    frmViewList2.Open();
  finally
    frmViewList2.Free;
  end;
end;

procedure TfrmMain.MENU_TOOLS_MISSIONMERCHANTClick(Sender: TObject);
begin
  FrmMissionNpcPageEditDlg := TFrmMissionNpcPageEditDlg.Create(nil);
  try
    FrmMissionNpcPageEditDlg.Open;
  finally
    FrmMissionNpcPageEditDlg.Free;
  end;
end;

procedure TfrmMain.MENU_OPTION_CLIENTCONFIGClick(Sender: TObject);
begin
  FrmConfigClient := TFrmConfigClient.Create(nil);
  try
    FrmConfigClient.Open();
  finally
    FrmConfigClient.Free;
  end;
end;

procedure TfrmMain.MENU_CONTROL_LOGINOFFLIENClick(Sender: TObject);
begin
  LoadAutoLoadOffline;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_NPCClick(Sender: TObject);

  procedure ReloadAllNpc;
  begin
    FrmDB.ReloadMerchants();
    UserEngine.ReloadMerchantList();
    MainOutMessage('交易NPC重新加载成功.');

    UserEngine.ReloadNpcList();
    MainOutMessage('管理NPC重新加载成功.');
    g_PluginManager.HookEngineReloadComplete(15);
  end;

begin
  SetCurrentDir(ExtractFilePath(ParamStr(0)));

{$IFDEF CPUX64}
  ReloadAllNpc;
{$ELSE}
  AsyncCalls.LocalAsyncCall(@ReloadAllNpc);
{$ENDIF}
end;

procedure TfrmMain.pnlInfoClick(Sender: TObject);
begin
{$IFDEF DEBUG}
  MainOutMessage('SizeOf(TAbility) ' + IntToStr(SizeOf(TAbility)));
  MainOutMessage('SizeOf(TUserItem) ' + IntToStr(SizeOf(TUserItem)));
  MainOutMessage('SizeOf(TUserMagic) ' + IntToStr(SizeOf(TUserMagic)));
  MainOutMessage('SizeOf(TStdItem) ' + IntToStr(SizeOf(TStdItem)));
  MainOutMessage('SizeOf(TClientItem) ' + IntToStr(SizeOf(TClientItem)));
  MainOutMessage('SizeOf(TMagic) ' + IntToStr(SizeOf(TMagic)));
  MainOutMessage('SizeOf(TClientMagic) ' + IntToStr(SizeOf(TClientMagic)));
  MainOutMessage('SizeOf(TUserStateInfo) ' + IntToStr(SizeOf(TUserStateInfo)));
  MainOutMessage('SizeOf(TActorIcon) ' + IntToStr(SizeOf(TActorIcon)));
  MainOutMessage('SizeOf(TActorIconArray) ' + IntToStr(SizeOf(TActorIconArray)));
  MainOutMessage('SizeOf(TProcessMessage) ' + IntToStr(SizeOf(TProcessMessage)));
{$ENDIF}
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_MAPEVENTClick(Sender: TObject);
begin
  MainOutMessage('正在加载地图触发事件信息...');
  if FrmDB.LoadMapEvent < 0 then
  begin
    MainOutMessage('加载地图触发事件信息失败！');
    Exit;
  end;

  MainOutMessage('加载地图触发事件信息成功.');
  g_PluginManager.HookEngineReloadComplete(16);
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_MonItemsClick(Sender: TObject);
var
  I: Integer;
  Monster: pTMonInfo;
begin
  try
    for I := 0 to UserEngine.MonsterList.Count - 1 do
    begin
      Monster := UserEngine.MonsterList.Items[I];
      FrmDB.LoadMonitems(Monster.sName, Monster.ItemList);
    end;

    MainOutMessage('怪物爆物品列表重加载成功.');
    g_PluginManager.HookEngineReloadComplete(17);
  except
    MainOutMessage('怪物爆物品列表重加载失败！');
  end;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_ROBOTNPCClick(Sender: TObject);
begin
  if g_RobotNPC <> nil then
  begin
    RobotManage.UnLoadRobot;
    g_RobotNPC.ClearScript();
    g_RobotNPC.LoadNpcScript();
    RobotManage.LoadRobot;
    MainOutMessage('重新加载机器人专用脚本成功.');
    g_PluginManager.HookEngineReloadComplete(14);
  end
  else
    MainOutMessage('重新加载机器人专用脚本失败.');
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_DUMMYLSClick(Sender: TObject);
begin
  LoadDummyNameList;
  LoadDummyHeroNameList;
  MainOutMessage('重新加载假人列表成功.');
  g_PluginManager.HookEngineReloadComplete(19);
end;

procedure TfrmMain.MENU_MANAGE_FILES_CLEARDENYSAYMSGLISTClick(Sender: TObject);
begin
  g_DenySayMsgList.Clear;
  MainOutMessage('禁言列表清除成功.');
end;

procedure TfrmMain.MENU_MANAGE_FILES_CLEARGLOBALVALClick(Sender: TObject);
begin
  ShowFrmGlobalVarEdit(0);
end;

procedure TfrmMain.MENU_MANAGE_FILES_CLEARGLOBALAVALClick(Sender: TObject);
begin
  ShowFrmGlobalVarEdit(1);
end;

procedure TfrmMain.MENU_MANAGE_PLUGCLIENTClick(Sender: TObject);
var
  FrmClientPlugManager: TFrmClientPlugManager;
begin
  FrmClientPlugManager := TFrmClientPlugManager.Create(nil);
  try
    FrmClientPlugManager.Open();
  finally
    FrmClientPlugManager.Free;
  end;
end;

procedure TfrmMain.MENU_MANAGE_CLIENTMODULEClick(Sender: TObject);
begin
  ftmClientModules := TftmClientModules.Create(nil);
  try
    ftmClientModules.Open();
  finally
    ftmClientModules.Free;
  end;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_BOXClick(Sender: TObject);
begin
  g_BoxsList.LoadFromFile;
  MainOutMessage(Format('宝箱列表加载成功(%d).', [g_BoxsList.Count]));
  g_PluginManager.HookEngineReloadComplete(6);
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_IPClick(Sender: TObject);
begin
  LoadServerTable();
  MainOutMessage(Format('IP授权文件加载成功(%d).', [g_ServerTableList.Count]));
  g_PluginManager.HookEngineReloadComplete(20);
end;

procedure TfrmMain.MENU_MANAGE_WAREHOUSEClick(Sender: TObject);
begin
  if boStartReady then
    ShowFrmStorageItemsView;
end;

procedure TfrmMain.MENU_CONTROL_MONSTER_BIG_HPSHOWClick(Sender: TObject);
begin
  MainOutMessage('正在加载怪物大血条...');
  LoadMonHPProgress();
  MainOutMessage(Format('加载怪物大血条成功(%d).', [g_MonSayMsgList.Count]));
  g_PluginManager.HookEngineReloadComplete(5);
end;

procedure TfrmMain.MENU_OPTION_PETClick(Sender: TObject);
begin
  FrmGamePets := TFrmGamePets.Create(nil);
  try
    FrmGamePets.DoOpen;
  finally
    FrmGamePets.Free;
  end;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_SHOPPRICELIMITClick(Sender: TObject);
begin
  LoadLowestSellingPriceList;
  LoadHighestSellingPriceList;
  MainOutMessage('摆摊物品售价限制加载完成');
  g_PluginManager.HookEngineReloadComplete(18);
end;

procedure TfrmMain.MENU_VIEW_USER_CURRENCYClick(Sender: TObject);
begin
  ShowFrmUserShopGetMoneyTotal;
end;

procedure TfrmMain.WMSetParentWindow(var msg: TMessage);
begin
  if msg.WParam = 1 then
  begin
    Windows.SetParent(Handle, msg.LParam);
    SetWindowLong(Application.Handle, GWL_EXSTYLE, GetWindowLong(Application.Handle, GWL_EXSTYLE) or WS_EX_TOOLWINDOW and
      not WS_EX_APPWINDOW);
    FIsEmbeddedGameCenter := True;
    Hide;
    Show;
  end
  else
  begin
    Windows.SetParent(Handle, 0);
    SetWindowLong(Application.Handle, GWL_EXSTYLE, GetWindowLong(Application.Handle, GWL_EXSTYLE) and (not WS_EX_TOOLWINDOW));
    FIsEmbeddedGameCenter := False;
  end;
end;

procedure TfrmMain.FormShow(Sender: TObject);
begin
  g_Version := GetFileVersionStr(ParamStr(0));
  g_BuildTime := ExtractSelfTimeDateStamp;

  stat.Panels[0].Text := 'BuildTime：' + FormatDateTime('yyyy/mm/dd hh:nn:ss', g_BuildTime);
  stat.Panels[1].Text := 'ver：' + g_Version;
  if FIsEmbeddedGameCenter then
    ShowWindow(Application.Handle, SW_HIDE)
  else
    ShowWindow(Application.Handle, SW_SHOW);
end;

procedure TfrmMain.WMSysCommand(var msg: TWMSysCommand);
const
  GWW_HWNDPARENT = -8;
begin
  if FIsEmbeddedGameCenter and (msg.CmdType and $FFF0 = SC_MINIMIZE) then
    DefaultHandler(msg)
  else
    inherited;
end;

function TfrmMain.ExtractSelfTimeDateStamp: TDateTime;
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
  tmpNtHeader := Pointer(UIntPtr(tmpDosHeader^._lfanew) + UIntPtr(tmpDosHeader));
  Result := UnixDateToDateTime(tmpNtHeader.FileHeader.TimeDateStamp);
  tmpMM.Free;
end;

procedure TfrmMain.OnAppModalBegin(Sender: TObject);
begin
  if FIsEmbeddedGameCenter then
    Enabled := False;
end;

procedure TfrmMain.OnAppOnModalEnd(Sender: TObject);
begin
  if FIsEmbeddedGameCenter then
    Enabled := True;
end;

procedure TfrmMain.OnApplicationMessage(var msg: tagMSG; var Handled: Boolean);
begin
  if (msg.Message = WM_LBUTTONDBLCLK) and (msg.hwnd = memoLog.Handle) then
    ClearMemoLog;
end;

procedure TfrmMain.FormDestroy(Sender: TObject);
begin
  g_DropLimitMgr.Free;
  g_CombatPowerVarMgr.Free;
  g_GrobalPlayer.Free;
  g_SellPlayerList.Free;
end;

procedure TfrmMain.MENU_CONTROL_ITEMDROPRULEClick(Sender: TObject);
begin
  if g_nKey_DropLimitExt = 1 then
    g_DropLimitMgr.LoadConfig;

  g_PluginManager.HookEngineReloadComplete(10);
end;

procedure TfrmMain.MENU_MANAGE_MYSHOPClick(Sender: TObject);
begin
  if boStartReady then
    ShowFrmUserShopView;
end;

procedure TfrmMain.tmrRunTimer(Sender: TObject);
begin
  UserEngine.ProcessHeros;
  UserEngine.ProcessHumans;
end;

procedure TfrmMain.MENU_OPTION_HERO_SKILLClick(Sender: TObject);
begin
  if boStartReady then
    ShowFrmHeroMagicSetting;
end;

procedure TfrmMain.MENU_MANAGE_ONLINEMSGClick(Sender: TObject);
begin
  ShowFrmOnlineMsg;
end;

procedure TfrmMain.MENU_MANAGE_PLUG_ENABLEDClick(Sender: TObject);
var
  FrmPlugManager: TFrmPlugManager;
begin
  FrmPlugManager := TFrmPlugManager.Create(nil);
  try
    FrmPlugManager.ShowModal;
  finally
    FrmPlugManager.Free;
  end;
end;

procedure TfrmMain.MENU_OPTION_CUSTOM_ITEMClick(Sender: TObject);
begin
  if boStartReady then
    ShowFrmCustomItemProperty;
end;

procedure TfrmMain.MENU_OPTION_AGGRESSIVITYClick(Sender: TObject);
begin
  if boStartReady then
    ShowFrmCombatPowerSetting;
end;

procedure TfrmMain.MENU_OPTION_DUMMYClick(Sender: TObject);
begin
  if boStartReady then
    ShowFrmDummySetting;
end;

procedure TfrmMain.MENU_OPTION_CUSTOM_SKILLClick(Sender: TObject);
begin
  if boStartReady then
    ShowCustomMagic;
end;

procedure TfrmMain.MENU_OPTION_CUSTOM_NPCClick(Sender: TObject);
begin
  if boStartReady then
    ShowFrmCustomNpc;
end;

procedure TfrmMain.MENU_CONTROL_RELOAD_SELLROLEINFOClick(Sender: TObject);
var
  sFileName: string;
  SL: TStringList;
begin
  sFileName := g_Config.sEnvirDir + 'SellPlayerInfo.txt';
  if FileExists(sFileName) then
  begin
    SL := TStringList.Create;
    try
      SL.LoadFromFile(sFileName);
      g_SellPlayerInfoText := StringReplace(SL.Text, sLineBreak, '', [rfReplaceAll]);
    finally
      SL.Free;
    end;

    MainOutMessage('重新加载出售角色其他信息成功.' + '\Envir\SellPlayerInfo.txt');
  end
  else
  begin
    g_SellPlayerInfoText := '';
    MainOutMessage('未找到出售角色其他信息文件.' + '\Envir\SellPlayerInfo.txt');
  end;
end;

procedure TfrmMain.MENU_TOOLS_SCRIPT_EDITORClick(Sender: TObject);
var
  tmpForm: TfrmTXTEditor;
begin
  tmpForm := TfrmTXTEditor.Create(nil);
  tmpForm.ShowModal;
  tmpForm.Free;
end;

end.

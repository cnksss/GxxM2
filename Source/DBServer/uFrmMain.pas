unit uFrmMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, JSocket, ExtCtrls, StdCtrls, Menus, Grids, Grobal2, PlugIn,
  SqliteRoleDB, MySqlRoleDB, uFrmDataManager, RoleDB, DBShare;

const
  WM_SET_PARENT_WINDOW = WM_USER + 123;

type
  TRankingEngine = class(TThread)
    m_boNeedRef: Boolean;
  private
    procedure Run();
  protected
    procedure Execute; override;
  public
    procedure RefRanking;
    constructor Create(CreateSuspended: Boolean);
    destructor Destroy; override;
  end;

  TFrmMain = class(TForm)
    TimerStart: TTimer;
    MemoLog: TMemo;
    Panel: TPanel;
    TimerMain: TTimer;
    MainMenu: TMainMenu;
    MENU_CONTROL: TMenuItem;
    MENU_CONTROL_START: TMenuItem;
    MENU_CONTROL_STOP: TMenuItem;
    N1: TMenuItem;
    G1: TMenuItem;
    C1: TMenuItem;
    MENU_CONTROL_EXIT: TMenuItem;
    MENU_OPTION: TMenuItem;
    MENU_OPTION_GENERAL: TMenuItem;
    MENU_OPTION_GAMEGATE: TMenuItem;
    MENU_MANAGE: TMenuItem;
    MENU_MANAGE_DATA: TMenuItem;
    MENU_RANKING: TMenuItem;
    MENU_TEST: TMenuItem;
    MENU_TEST_SELGATE: TMenuItem;
    MENU_HELP: TMenuItem;
    MENU_HELP_VERSION: TMenuItem;
    ModuleGrid: TStringGrid;
    LabelLoadHumRcd: TLabel;
    LabelSaveHumRcd: TLabel;
    LabelLoadHeroRcd: TLabel;
    LabelSaveHeroRcd: TLabel;
    LabelCreateHero: TLabel;
    LabelCreateHum: TLabel;
    LabelDeleteHum: TLabel;
    LabelDeleteHero: TLabel;
    LabelWorkStatus: TLabel;
    TimerClose: TTimer;
    MENU_OUTDATA: TMenuItem;
    N2: TMenuItem;
    MENU_OPTION_SHOWLOG: TMenuItem;
    LabelStatus: TLabel;
    mniShowQuryChrLog: TMenuItem;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure TimerMainTimer(Sender: TObject);
    procedure TimerStartTimer(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure MENU_OPTION_GAMEGATEClick(Sender: TObject);
    procedure MENU_RANKINGClick(Sender: TObject);
    procedure MENU_TEST_SELGATEClick(Sender: TObject);
    procedure MENU_CONTROL_STARTClick(Sender: TObject);
    procedure MENU_CONTROL_STOPClick(Sender: TObject);
    procedure MENU_CONTROL_EXITClick(Sender: TObject);
    procedure TimerCloseTimer(Sender: TObject);
    procedure MENU_HELP_VERSIONClick(Sender: TObject);
    procedure G1Click(Sender: TObject);
    procedure C1Click(Sender: TObject);
    procedure MemoLogChange(Sender: TObject);
    procedure MemoLogDblClick(Sender: TObject);
    procedure MENU_OPTION_GENERALClick(Sender: TObject);
    procedure MENU_OUTDATAClick(Sender: TObject);
    procedure MENU_OPTION_SHOWLOGClick(Sender: TObject);
    procedure mniShowQuryChrLogClick(Sender: TObject);
    procedure MENU_MANAGE_DATAClick(Sender: TObject);
  private
    ServerSocket: TServerSocket;
    SelectSocket: TServerSocket;
    FIsEmbeddedGameCenter: Boolean;

    FRunGateClientSendTick: LongWord;
    FRunGateClientSockets: TList;

    FLogFileName: string;

    procedure StartService();
    procedure StopService();
    procedure ShowMainLogMsg;
    procedure ShowModule;
    procedure ShowWorkStatus;

{$IF DBSUSETHREAD = 1}
    procedure ServerSocketClientGetThreadEvent(Sender: TObject; ClientSocket: TServerClientWinSocket;
      var SocketThread: TServerClientThread);
    procedure ServerSocketThreadStart(Sender: TObject;
      Thread: TServerClientThread);
    procedure ServerSocketThreadEnd(Sender: TObject;
      Thread: TServerClientThread);

    procedure SelectSocketClientGetThreadEvent(Sender: TObject; ClientSocket: TServerClientWinSocket;
      var SocketThread: TServerClientThread);
    procedure SelectSocketThreadStart(Sender: TObject;
      Thread: TServerClientThread);
    procedure SelectSocketThreadEnd(Sender: TObject;
      Thread: TServerClientThread);
{$ELSE}
    procedure ServerSocketGetSocket(Sender: TObject; Socket: Integer; var ClientSocket: TServerClientWinSocket);
    procedure ServerSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure ServerSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure ServerSocketClientRead(Sender: TObject; Socket: TCustomWinSocket);

    procedure SelectSocketGetSocket(Sender: TObject; Socket: Integer; var ClientSocket: TServerClientWinSocket);
    procedure SelectSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure SelectSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure SelectSocketClientRead(Sender: TObject; Socket: TCustomWinSocket);

    procedure DoRunGateReponse(ClientSocket: TClientSocket);
    procedure OnRunGateSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure OnRunGateSocketClientRead(Sender: TObject; Socket: TCustomWinSocket);
    procedure OnRunGateSocketClientError(Sender: TObject; Socket: TCustomWinSocket;
      ErrorEvent: TErrorEvent; var ErrorCode: Integer);
    procedure OnRunGateSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);

{$IFEND}
    procedure SocketClientError(Sender: TObject;
      Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
      var ErrorCode: Integer);

    procedure WMSysCommand(var Message: TWMSysCommand); message WM_SYSCOMMAND;
    procedure WMSetParentWindow(var Message: TMessage); message WM_SET_PARENT_WINDOW;

    procedure OnAppModalBegin(Sender: TObject);
    procedure OnAppOnModalEnd(Sender: TObject);
  public
    procedure MyMessage(var MsgData: TWmCopyData); message WM_COPYDATA;
    function GetSelectCharCount: Integer;
    procedure OnProgramException(Sender: TObject; E: Exception);

    procedure ClearRunGateClientSockets;
    procedure ReconnectToRunGate;
  end;

var
  FrmMain: TFrmMain;
  RankingEngine: TRankingEngine;
implementation
uses
  SelectClient, ServerClient, HUtil32, Common, SDK,
  IDSocCli, RouteManage, Ranking, TestSelGate, Setting,
  uFrmHumanExport;

{$R *.dfm}

{$IF DBSUSETHREAD = 1}

procedure TFrmMain.ServerSocketClientGetThreadEvent(Sender: TObject; ClientSocket: TServerClientWinSocket;
  var SocketThread: TServerClientThread);
begin
  SocketThread := TServerClient.Create(ClientSocket);
end;

procedure TFrmMain.ServerSocketThreadStart(Sender: TObject;
  Thread: TServerClientThread);
var
  sRemoteAddress: string;
  ModuleInfo: TModuleInfo;
begin
  sRemoteAddress := Thread.ClientSocket.RemoteAddress;
  if (not CheckServerIP(sRemoteAddress)) then
  begin
    if g_boShowBlockIPLog then
      MainOutMessage('拒绝未授权IP连接服务器：' + sRemoteAddress);
    Thread.ClientSocket.Close;
    Exit;
  end;
  if not (boDataDBReady and boHumDBReady and g_boStartService) then
    Thread.ClientSocket.Close
  else
  begin
    ModuleInfo.Module := Thread;
    ModuleInfo.ModuleName := '游戏中心';
    ModuleInfo.Address := Format('%s:%d → %s:%d', [sRemoteAddress, Thread.ClientSocket.RemotePort, sRemoteAddress, ServerSocket.Port]);
    ModuleInfo.Buffer := '0/0';
    TServerClient(Thread).m_Module := AddModule(@ModuleInfo);
  end;
end;

procedure TFrmMain.ServerSocketThreadEnd(Sender: TObject;
  Thread: TServerClientThread);
begin
  RemoveModule(Thread);
end;

{------------------------------------------------------------------------------}

procedure TFrmMain.SelectSocketClientGetThreadEvent(Sender: TObject; ClientSocket: TServerClientWinSocket;
  var SocketThread: TServerClientThread);
begin
  SocketThread := TSelectClient.Create(ClientSocket);
end;

procedure TFrmMain.SelectSocketThreadStart(Sender: TObject;
  Thread: TServerClientThread);
var
  sRemoteAddress: string;
  ModuleInfo: TModuleInfo;
begin
  sRemoteAddress := Thread.ClientSocket.RemoteAddress;
  if (not CheckServerIP(sRemoteAddress)) then
  begin
    if g_boShowBlockIPLog then
      MainOutMessage('拒绝未授权IP连接服务器：' + sRemoteAddress);
    Thread.ClientSocket.Close;
    Exit;
  end;
  if not (boDataDBReady and boHumDBReady and g_boStartService) then
    Thread.ClientSocket.Close
  else
  begin
    ModuleInfo.Module := Thread;
    ModuleInfo.ModuleName := '角色网关';
    ModuleInfo.Address := Format('%s:%d → %s:%d', [sRemoteAddress, Thread.ClientSocket.RemotePort, sRemoteAddress, SelectSocket.Port]);
    ModuleInfo.Buffer := '0/0';
    TSelectClient(Thread).m_Module := AddModule(@ModuleInfo);
  end;
end;

procedure TFrmMain.SelectSocketThreadEnd(Sender: TObject;
  Thread: TServerClientThread);
begin
  RemoveModule(Thread);
end;

{$ELSE}

procedure TFrmMain.ServerSocketGetSocket(Sender: TObject; Socket: Integer; var ClientSocket: TServerClientWinSocket);
begin
  ClientSocket := TServerClient.Create(Socket, ServerSocket.Socket);
end;

procedure TFrmMain.ServerSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
var
  sRemoteAddress: string;
  //ModuleInfo: TModuleInfo;
begin
  sRemoteAddress := Socket.RemoteAddress;
  if (not CheckServerIP(sRemoteAddress)) then
  begin
    if g_boShowBlockIPLog then
      MainOutMessage('拒绝未授权IP连接服务器：' + sRemoteAddress);
    Socket.Close;
    Exit;
  end;

  {
  if not (boDataDBReady and boHumDBReady and g_boStartService) then
    Socket.Close
  else
  begin

    ModuleInfo.Module := Socket;
    ModuleInfo.ModuleName := '游戏中心';
    ModuleInfo.Address := Format('%s:%d → %s:%d', [sRemoteAddress, Socket.RemotePort, sRemoteAddress, ServerSocket.Port]);
    ModuleInfo.Buffer := '0/0';
    TServerClient(Socket).m_Module := AddModule(@ModuleInfo);
  end;
  }
end;

procedure TFrmMain.ServerSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
begin
  RemoveModule(Socket);
end;

procedure TFrmMain.ServerSocketClientRead(Sender: TObject;
  Socket: TCustomWinSocket);
var
  nMsgLen: Integer;
  RecvBuffer: array[0..DATA_BUFSIZE * 2 - 1] of Char;
begin
  while (True) do
  begin
    nMsgLen := Socket.ReceiveBuf(RecvBuffer, SizeOf(RecvBuffer));
    if nMsgLen <= 0 then Break;
    with Socket as TServerClient do
      ProcessServerPacket(@RecvBuffer, nMsgLen);
  end;
end;

{------------------------------------------------------------------------------}

procedure TFrmMain.SelectSocketGetSocket(Sender: TObject; Socket: Integer; var ClientSocket: TServerClientWinSocket);
begin
  ClientSocket := TSelectClient.Create(Socket, SelectSocket.Socket);
end;

procedure TFrmMain.SelectSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
var
  sRemoteAddress: string;
  //ModuleInfo: TModuleInfo;
begin
  sRemoteAddress := Socket.RemoteAddress;
  if (not CheckServerIP(sRemoteAddress)) then
  begin
    if g_boShowBlockIPLog then
      MainOutMessage('拒绝未授权IP连接服务器：' + sRemoteAddress);
    Socket.Close;
    Exit;
  end;
  {
  if not (boDataDBReady and boHumDBReady and g_boStartService) then
    Socket.Close
  else
  begin
    ModuleInfo.Module := Socket;
    ModuleInfo.ModuleName := '角色网关';
    ModuleInfo.Address := Format('%s:%d → %s:%d', [sRemoteAddress, Socket.RemotePort, sRemoteAddress, SelectSocket.Port]);
    ModuleInfo.Buffer := '0/0';
    TSelectClient(Socket).m_Module := AddModule(@ModuleInfo);
  end;
  }
end;

procedure TFrmMain.SelectSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
begin
  RemoveModule(Socket);
end;

procedure TFrmMain.SelectSocketClientRead(Sender: TObject;
  Socket: TCustomWinSocket);
//var
  //nMsgLen: Integer;
  //RecvBuffer: array[0..DATA_BUFSIZE * 2 - 1] of Char;
var
  sReceiveText: string;
begin
  sReceiveText := Socket.ReceiveText;
  with Socket as TSelectClient do
  begin
    //MainOutMessage('SelectSocketClientRead:' + sReceiveText);
    ExecGateBuffers(sReceiveText);
  end;
 { nMsgLen := Socket.ReceiveLength;
  if nMsgLen > 0 then begin
    //MainOutMessage('RecvBuffer:' + IntToStr(nMsgLen) + '  ' + RecvBuffer);
    Socket.ReceiveBuf(RecvBuffer, nMsgLen);

    MainOutMessage('RecvBuffer:'+RecvBuffer);
  end;}
end;

{-------------------------------------------------------------------------------------}
procedure TFrmMain.DoRunGateReponse(ClientSocket: TClientSocket);
var
  I, J: Integer;
  sIP: string;
  nPort: Word;
begin
  for I := Low(g_RouteInfo) to High(g_RouteInfo) do
  begin
    if g_RouteInfo[I].nGateCount = 0  then Break;

    for J := 0 to g_RouteInfo[I].nGateCount - 1 do
    begin
      sIP := g_RouteInfo[I].sGameGateIP[J];
      nPort := g_RouteInfo[I].nGameGateDBPort[J];

      if SameText(ClientSocket.Address, sIP) and (ClientSocket.Port = nPort) then
      begin
        g_RouteInfo[I].dwGameGateConnectTick[J] := GetTickCount;
      end;
    end;

    for J := 0 to g_RouteInfo[I].RunGate2List.Count - 1 do
    begin
      sIP := g_RouteInfo[I].RunGate2List.Items[J].IP;
      nPort := g_RouteInfo[I].RunGate2List.Items[J].DBPort;

      if SameText(ClientSocket.Address, sIP) and (ClientSocket.Port = nPort) then
      begin
        g_RouteInfo[I].RunGate2List.Items[J].LastResponseTick := GetTickCount;
      end;
    end;
  end;
end;

procedure TFrmMain.OnRunGateSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
begin
  if Sender is TClientSocket then
    DoRunGateReponse(TClientSocket(Sender));
end;

procedure TFrmMain.OnRunGateSocketClientRead(Sender: TObject; Socket: TCustomWinSocket);
begin
  Socket.ReceiveText;
  if Sender is TClientSocket then
    DoRunGateReponse(TClientSocket(Sender));
end;

procedure TFrmMain.OnRunGateSocketClientError(Sender: TObject; Socket: TCustomWinSocket;
  ErrorEvent: TErrorEvent; var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
end;

procedure TFrmMain.OnRunGateSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
begin
  if (Sender is TClientSocket) then
    (Sender as TClientSocket).Tag := 1;
end;

{$IFEND}

procedure TFrmMain.SocketClientError(Sender: TObject;
  Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
  var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
end;

function TFrmMain.GetSelectCharCount: Integer;
{$IF DBSUSETHREAD = 1}
var
  I: Integer;
  SelectClient: TSelectClient;
{$IFEND}
begin
{$IF DBSUSETHREAD = 1}
  Result := 0;
  for I := 0 to SelectSocket.Socket.ActiveConnections - 1 do
  begin
    SelectClient := SelectSocket.Socket.GetClientThread(SelectSocket.Socket.Connections[I] as TServerClientWinSocket) as TSelectClient;
    if SelectClient <> nil then
    begin
      Result := Result + SelectClient.SelectCharList.OnLineCount;
    end;
  end;
{$ELSE}
  Result := SelectSocket.Socket.ActiveConnections;
{$IFEND}
end;
{------------------------------------------------------------------------------}

procedure TFrmMain.OnProgramException(Sender: TObject; E: Exception);
begin
  MainOutMessage(E.Message);
end;

procedure TFrmMain.FormCreate(Sender: TObject);
var
  nX, nY: Integer;
  TimeNow: TDateTime;
  Year, Month, Day, Hour, Min, Sec, MSec: Word;
  sDir: string;
begin
  {
  asm
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
  nop
  nop
  nop
  nop
  end;
  }

  FIsEmbeddedGameCenter := False;
  g_sFilePath := ExtractFilePath(Application.ExeName);
  g_dwGameCenterHandle := StrToIntDef(ParamStr(1), 0);
  nX := StrToIntDef(ParamStr(2), -1);
  nY := StrToIntDef(ParamStr(3), -1);

  TimeNow := Now();
  DecodeDate(TimeNow, Year, Month, Day);
  DecodeTime(TimeNow, Hour, Min, Sec, MSec);

  sDir := g_sFilePath + '\Log';
  if not DirectoryExists(sDir) then
  begin
    CreateDir(sDir);
  end;

  FLogFileName := sDir +'\' + IntToStr(Year) + '-' + IntToStr2(Month) + '-' + IntToStr2(Day) + '.' + IntToStr2(Hour) + '-' + IntToStr2(Min) + '.txt';

  if (nX >= 0) or (nY >= 0) then
  begin
    Left := nX;
    Top := nY;
  end;

  SendGameCenterMsg(SG_FORMHANDLE, IntToStr(Self.Handle));

  Application.OnModalBegin := OnAppModalBegin;
  Application.OnModalEnd := OnAppOnModalEnd;

  MainOutMessage('正在启动数据库服务器...');
  Application.OnException := OnProgramException;
  ModuleGrid.RowCount := 5;
  ModuleGrid.ColWidths[0] := 80;
  ModuleGrid.ColWidths[1] := ModuleGrid.Width - 6 - 80 * 2;
  ModuleGrid.ColWidths[2] := 80;
  ModuleGrid.Cells[0, 0] := '模块名称';
  ModuleGrid.Cells[1, 0] := '连接地址';
  ModuleGrid.Cells[2, 0] := '数据通讯';
  g_PlugInManage := TPlugInManage.Create;
  ServerSocket := TServerSocket.Create(Self);
  SelectSocket := TServerSocket.Create(Self);

  FRunGateClientSockets := TList.Create;

{$IF DBSUSETHREAD = 1}
  ServerSocket.OnGetThread := ServerSocketClientGetThreadEvent;
  ServerSocket.OnThreadStart := ServerSocketThreadStart;
  ServerSocket.OnThreadEnd := ServerSocketThreadEnd;
  ServerSocket.ServerType := stThreadBlocking;
  ServerSocket.ThreadCacheSize := 0;

  SelectSocket.OnGetThread := SelectSocketClientGetThreadEvent;
  SelectSocket.OnThreadStart := SelectSocketThreadStart;
  SelectSocket.OnThreadEnd := SelectSocketThreadEnd;
  SelectSocket.ServerType := stThreadBlocking;
  SelectSocket.ThreadCacheSize := 0;
{$ELSE}
  ServerSocket.ServerType := stNonBlocking;
  ServerSocket.OnGetSocket := ServerSocketGetSocket;
  ServerSocket.OnClientConnect := ServerSocketClientConnect;
  ServerSocket.OnClientDisconnect := ServerSocketClientDisconnect;
  ServerSocket.OnClientRead := ServerSocketClientRead;

  SelectSocket.ServerType := stNonBlocking;
  SelectSocket.OnGetSocket := SelectSocketGetSocket;
  SelectSocket.OnClientConnect := SelectSocketClientConnect;
  SelectSocket.OnClientDisconnect := SelectSocketClientDisconnect;
  SelectSocket.OnClientRead := SelectSocketClientRead;
{$IFEND}
  ServerSocket.OnClientError := SocketClientError;
  SelectSocket.OnClientError := SocketClientError;

  FRunGateClientSendTick := 0;

  ServerSocket.Address := '0.0.0.0';
  SelectSocket.Address := '0.0.0.0';

  g_HumanRankList := TRoleRankList.Create;
  g_WarriorRankList := TRoleRankList.Create;
  g_WizardRankList := TRoleRankList.Create;
  g_TaoistRankList := TRoleRankList.Create;
  g_MasterRankList := TRoleRankList.Create;

  g_HeroRankList := TRoleRankList.Create;
  g_HeroWarriorRankList := TRoleRankList.Create;
  g_HeroWizardRankList := TRoleRankList.Create;
  g_HeroTaoistRankList := TRoleRankList.Create;

  g_MagicList := TStringList.Create;
  g_StdItemList := TStringList.Create;

  g_ModuleList := TSortStringList.Create;

  MENU_OPTION_SHOWLOG.Checked := g_boShowLogMsg;
  RankingEngine := TRankingEngine.Create(True);
  RankingEngine.Resume;

  LoadConfig_DataSaveDB;

  if g_nDataSaveDBType = 0 then
  begin
    g_RoleDB := TSqliteRoleDB.Create;
  end
  else
  begin
    g_RoleDB := TMySqlRoleDB.Create;
  end;

  SendGameCenterMsg(SG_STARTNOW, '正在启动数据库服务器...');

  TimerStart.Enabled := True;
end;

procedure TFrmMain.FormDestroy(Sender: TObject);
var
  I: Integer;
begin
  g_PlugInManage.Free;
  RankingEngine.Terminate;
  RankingEngine.Free;

  g_HumanRankList.Free;
  g_WarriorRankList.Free;
  g_WizardRankList.Free;
  g_TaoistRankList.Free;
  g_MasterRankList.Free;

  g_HeroRankList.Free;
  g_HeroWarriorRankList.Free;
  g_HeroWizardRankList.Free;
  g_HeroTaoistRankList.Free;

  for I := 0 to g_ModuleList.Count - 1 do
  begin
    Dispose(pTModuleInfo(g_ModuleList.Objects[I]));
  end;

  ClearRunGateClientSockets;
  FRunGateClientSockets.Free;
  
  UnLoadMagicList;
  UnLoadStdItemList;

  ServerSocket.Free;
  SelectSocket.Free;

  g_ModuleList.Free;
  g_MagicList.Free;
  g_StdItemList.Free;

  g_RoleDB.Free;
end;

procedure TFrmMain.ShowMainLogMsg;
var
  I: Integer;
  TempLogList: TStringList;
  boWriteLog: Boolean;
  LogFile: TextFile;
begin
  if (GetTickCount - g_dwShowMainLogTick) > 20 then
  begin
    boWriteLog := True;
    if g_MainLogMsgList.Count > 0 then
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
        boWriteLog := False;
      except
        MemoLog.Lines.Add('保存日志信息出错！！！');
      end;
    end;

    g_dwShowMainLogTick := GetTickCount();
    TempLogList := TStringList.Create;
    try
      g_MainLogMsgList.Lock;
      try
        for I := 0 to g_MainLogMsgList.Count - 1 do
        begin
          TempLogList.Add(g_MainLogMsgList.Strings[I]);

          if not boWriteLog then
          begin
            Writeln(LogFile, g_MainLogMsgList.Strings[I]);
          end;
        end;

        g_MainLogMsgList.Clear;
      finally
        g_MainLogMsgList.UnLock;
      end;

      for I := 0 to TempLogList.Count - 1 do
      begin
        MemoLog.Lines.Add(TempLogList.Strings[I]);
      end;

      if not boWriteLog then CloseFile(LogFile);
    finally
      TempLogList.Free;
    end;
  end;
end;

procedure TFrmMain.ShowModule;
var
  I: Integer;
  ModuleInfo: pTModuleInfo;
begin
  if (GetTickCount - g_dwShowModuleTick) > 2000 then
  begin
    g_dwShowModuleTick := GetTickCount();
    ModuleGrid.RowCount := _MAX(g_ModuleList.Count + 1, 5);
    for I := 0 to ModuleGrid.RowCount - 1 do
    begin
      if I < g_ModuleList.Count then
      begin
        ModuleInfo := pTModuleInfo(g_ModuleList.Objects[I]);
        ModuleGrid.Cells[0, I + 1] := ModuleInfo.ModuleName;
        ModuleGrid.Cells[1, I + 1] := ModuleInfo.Address;
        ModuleGrid.Cells[2, I + 1] := ModuleInfo.Buffer;
      end
      else
      begin
        ModuleGrid.Cells[0, I + 1] := '';
        ModuleGrid.Cells[1, I + 1] := '';
        ModuleGrid.Cells[2, I + 1] := '';
      end;
    end;
  end;
end;

procedure TFrmMain.ShowWorkStatus;
begin
  //if GetTickCount - g_dwWorkStatusTick > 100 then begin
    //g_dwWorkStatusTick := GetTickCount;
  case g_nWorkStatus of
    DB_LOADHUMANRCD:
      begin
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '读取人物数据';
      end;
    DB_SAVEHUMANRCD:
      begin
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '修改人物名称';
      end;

    DB_HUMANCHANGENAME:
      begin
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '修改人物名称';
      end;
    DB_HUMANCHANGEGOLD:
      begin
        //LabelWorkStatus.Font.Color := clGreen;
        //LabelWorkStatus.Caption := '修改人物名称';
      end;
    DB_HEROCHANGENAME:
      begin
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '修改英雄名称';
      end;
    DB_LOADHERORCD:
      begin                                                                                         //读取英雄数据
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '读取英雄数据';
      end;
    DB_NEWHERORCD:
      begin                                                                                         //新建英雄
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '创建英雄';
      end;
    DB_DELHERORCD:
      begin                                                                                         //删除英雄
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '删除英雄';
      end;
    DB_SAVEHERORCD:
      begin                                                                                         //保存英雄数据
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '保存英雄数据';
      end;
    DB_GETRANKDATA:
      begin                                                                                         //排行榜
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '读取排行榜数据';
      end;
    DB_M2CACHERANKDATA:
      begin                                                                                         //排行榜
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '缓存排行榜数据';
      end;
    DB_QUERYHUMANINFO:
      begin                                                                                         //排行榜
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '读取行会成员信息';
      end;
    DB_BUY_PLAYER:
      begin                                                                                         //排行榜
        LabelWorkStatus.Font.Color := clGreen;
        LabelWorkStatus.Caption := '出售角色';
      end;
  else
    LabelWorkStatus.Font.Color := clBlue;
      {DB_SAVEMAGICLIST: begin

        end;
      DB_SAVESTDITEMLIST: begin

        end;
      DB_SENDKEEPALIVE: ;}
  end;
  if GetTickCount - g_dwWorkStatusTick > 1000 then
  begin
    g_dwWorkStatusTick := GetTickCount;
    LabelCreateHum.Caption := Format('创建人物:%d', [g_nCreateHumCount]);
    LabelDeleteHum.Caption := Format('删除人物:%d', [g_nDeleteHumCount]);
    LabelLoadHumRcd.Caption := Format('读取人物数据:%d', [g_nLoadHumCount]);
    LabelSaveHumRcd.Caption := Format('保存人物数据:%d', [g_nSaveHumCount]);

    LabelCreateHero.Caption := Format('创建英雄:%d', [g_nCreateHeroCount]);
    LabelDeleteHero.Caption := Format('删除英雄:%d', [g_nDeleteHeroCount]);
    LabelLoadHeroRcd.Caption := Format('读取英雄数据:%d', [g_nLoadHeroCount]);
    LabelSaveHeroRcd.Caption := Format('保存英雄数据:%d', [g_nSaveHeroCount]);
  end;
end;

procedure TFrmMain.TimerMainTimer(Sender: TObject);
var
  I: Integer;
  ClientSocket: TClientSocket;
begin
  ShowMainLogMsg;
  ShowModule;
  ShowWorkStatus;

  if g_boUseActiveRunGage and (GetTickCount - FRunGateClientSendTick >= 1000) then
  begin
    FRunGateClientSendTick := GetTickCount;

    for I := 0 to FRunGateClientSockets.Count - 1 do
    begin
      ClientSocket := FRunGateClientSockets.Items[I];
      if ClientSocket.Socket.Connected then
      begin
        ClientSocket.Socket.SendText('*');
      end
      else if ClientSocket.Tag <> 0 then
      begin
        ClientSocket.Tag := 0;
        ClientSocket.Active := True;
        MainOutMessage('连接网关检测端口: ' + ClientSocket.Address + ':' + IntToStr(ClientSocket.Port));
      end;
    end;
  end;

  if g_RoleDB <> nil then
  begin
    g_RoleDB.Run;
  end;
end;

procedure TFrmMain.MyMessage(var MsgData: TWmCopyData);
var
  sData: string;
  //ProgramType: TProgamType;
  wIdent: Word;
begin
  wIdent := HiWord(MsgData.From);
  sData := StrPas(MsgData.CopyDataStruct^.lpData);

  case wIdent of
    GS_QUIT:
      begin
        //StopService();
        g_boRemoteClose := True;
        Close();
      end;
    1: ;
    2: ;
    3: ;
  end;
end;

procedure TFrmMain.StartService();
var
  RoleDBRes: TResourceStream;
begin
  try
    MainOutMessage('正在启动服务器...');
    LoadConfig();

    //Caption := Caption + '-' + g_sServerName + '  [' + g_sFilePath + ']';
    Caption := Format('%s - [%s]', [g_sProgramName, g_sFilePath]);

    SelectSocket.Port := g_nGatePort;
    ServerSocket.Port := g_nServerPort;

    SelectSocket.Active := True;
    ServerSocket.Active := True;
    FrmIDSoc.OpenConnect;

    MENU_CONTROL_START.Enabled := False;
    MENU_CONTROL_STOP.Enabled := True;

    ReconnectToRunGate;

    if g_nDataSaveDBType = 0 then
    begin
      if not FileExists(g_sDataDBFilePath + 'RoleData.db') then
      begin
        RoleDBRes := TResourceStream.Create(HInstance, 'RoleData', PChar('SQLITEDB'));
        RoleDBRes.SaveToFile(g_sDataDBFilePath + 'RoleData.db');
        RoleDBRes.Free;
      end;
      TSqliteRoleDB(g_RoleDB).FileName := g_sDataDBFilePath + 'RoleData.db';
    end;
    
    g_RoleDB.Init;

    MainOutMessage('数据库服务器启动成功...');
    //MainOutMessage('SelectSocket.Port:' + IntToStr(SelectSocket.Port));
    //MainOutMessage('ServerSocket.Port:' + IntToStr(ServerSocket.Port));
    SendGameCenterMsg(SG_STARTOK, '数据库服务器启动完成...');

    g_boStartService := True;
  except
    on E: Exception do
    begin
      g_boStartService := False;
      MENU_CONTROL_START.Enabled := True;
      MENU_CONTROL_STOP.Enabled := False;
      FrmIDSoc.CloseConnect;
      SelectSocket.Active := False;
      ServerSocket.Active := False;
      MainOutMessage(E.Message);
    end;
  end;
end;

procedure TFrmMain.StopService();
{$IF DBSUSETHREAD = 1}
var
  ServerClient: TServerClient;
  SelectClient: TSelectClient;
{$IFEND}
begin
  g_boStartService := False;
  MainOutMessage('正在停止服务器...');
  MENU_CONTROL_START.Enabled := True;
  MENU_CONTROL_STOP.Enabled := False;
  FrmIDSoc.CloseConnect;

{$IF DBSUSETHREAD = 1}
  while ServerSocket.Socket.ActiveConnections > 0 do
  begin
    Application.ProcessMessages;
    ServerClient := ServerSocket.Socket.GetClientThread(ServerSocket.Socket.Connections[0] as TServerClientWinSocket) as TServerClient;
    if ServerClient <> nil then
      ServerClient.Close;
  end;

  while SelectSocket.Socket.ActiveConnections > 0 do
  begin
    Application.ProcessMessages;
    SelectClient := SelectSocket.Socket.GetClientThread(SelectSocket.Socket.Connections[0] as TServerClientWinSocket) as TSelectClient;
    if SelectClient <> nil then
      SelectClient.Close;
  end;
{$ELSE}
  while ServerSocket.Socket.ActiveConnections > 0 do
  begin
    Application.ProcessMessages;
    ServerSocket.Socket.Connections[0].Close;
  end;

  while SelectSocket.Socket.ActiveConnections > 0 do
  begin
    Application.ProcessMessages;
    SelectSocket.Socket.Connections[0].Close;
  end;

{$IFEND}

  SelectSocket.Active := False;
  ServerSocket.Active := False;
  MainOutMessage('服务器已停止...');
end;

procedure TFrmMain.TimerStartTimer(Sender: TObject);
begin
  TimerStart.Enabled := False;
  StartService();
end;

procedure TFrmMain.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
begin
  if g_boRemoteClose or g_boSoftClose then
  begin
    if (ServerSocket.Socket.ActiveConnections > 0) or (SelectSocket.Socket.ActiveConnections > 0) then
    begin
      CanClose := False;
      TimerClose.Enabled := True;
    end;
  end
  else
  begin
    if (Application.MessageBox('是否确定退出数据库服务器？',
      '确认信息',
      MB_YESNO + MB_ICONQUESTION) = IDYES) then
    begin
      g_boSoftClose := True;
      if (ServerSocket.Socket.ActiveConnections > 0) or (SelectSocket.Socket.ActiveConnections > 0) then
      begin
        CanClose := False;
        TimerClose.Enabled := True;
      end;
    end
    else
      CanClose := False;
  end;
end;
{begin
  if (ServerSocket.Socket.ActiveConnections > 0) or (SelectSocket.Socket.ActiveConnections > 0) then begin
    if g_boRemoteClose or (Application.MessageBox('是否确定退出数据库服务器？',
      '确认信息',
      MB_YESNO + MB_ICONQUESTION) = IDYES) then begin
      Caption := '[正在关闭数据库服务器...]';
      CanClose := False;
      TimerClose.Enabled := True;
    end else CanClose := False;
  end;
end; }

procedure TFrmMain.MENU_OPTION_GAMEGATEClick(Sender: TObject);
begin
  frmRouteManage.Open;
end;

procedure TFrmMain.MENU_RANKINGClick(Sender: TObject);
begin
  FrmRankingDlg.Top := Self.Top;
  FrmRankingDlg.Left := Self.Left;
  FrmRankingDlg.Open();
end;

procedure TFrmMain.MENU_TEST_SELGATEClick(Sender: TObject);
begin
  frmTestSelGate := TfrmTestSelGate.Create(Owner);
  frmTestSelGate.ShowModal;
  frmTestSelGate.Free;
end;

procedure TFrmMain.MENU_CONTROL_STARTClick(Sender: TObject);
begin
  StartService();
end;

procedure TFrmMain.MENU_CONTROL_STOPClick(Sender: TObject);
begin
  StopService();
end;

procedure TFrmMain.MENU_CONTROL_EXITClick(Sender: TObject);
begin
  Close;
end;

procedure TFrmMain.TimerCloseTimer(Sender: TObject);
{$IF DBSUSETHREAD = 1}
var
  ServerClient: TServerClient;
  SelectClient: TSelectClient;
{$IFEND}
begin
  if g_boStartService then
  begin
    g_boStartService := False;
    MainOutMessage('正在停止服务器...');
    MENU_CONTROL_START.Enabled := True;
    MENU_CONTROL_STOP.Enabled := False;
    FrmIDSoc.CloseConnect;
  end;
{$IF DBSUSETHREAD = 1}
  if ServerSocket.Socket.ActiveConnections > 0 then
  begin
    ServerClient := ServerSocket.Socket.GetClientThread(ServerSocket.Socket.Connections[0] as TServerClientWinSocket) as TServerClient;
    if ServerClient <> nil then
      ServerClient.Close;
  end
  else if SelectSocket.Socket.ActiveConnections > 0 then
  begin
    SelectClient := SelectSocket.Socket.GetClientThread(SelectSocket.Socket.Connections[0] as TServerClientWinSocket) as TSelectClient;
    if SelectClient <> nil then
      SelectClient.Close;
{$ELSE}
    if ServerSocket.Socket.ActiveConnections > 0 then
    begin
      ServerSocket.Socket.Connections[0].Close;
    end
    else if SelectSocket.Socket.ActiveConnections > 0 then
    begin
      SelectSocket.Socket.Connections[0].Close;
{$IFEND}
    end
    else
    begin
      TimerClose.Enabled := False;
      SelectSocket.Active := False;
      ServerSocket.Active := False;
      MainOutMessage('服务器已停止...');
      Close;
    end;
  end;

procedure TFrmMain.MENU_HELP_VERSIONClick(Sender: TObject);
begin
  //MainOutMessage(g_sUpDateTime);
  MainOutMessage(g_sProductName);
  MainOutMessage(g_sProgram);
  MainOutMessage(g_sWebSite);
end;

procedure TFrmMain.G1Click(Sender: TObject);
begin
  LoadGateID();
  LoadIPTable();
  MainOutMessage('网关设置加载完成');
end;

procedure TFrmMain.C1Click(Sender: TObject);
begin
  LoadChrNameList('DenyChrName.txt');
  MainOutMessage('角色过滤列表加载完成');
end;

procedure TFrmMain.MemoLogChange(Sender: TObject);
begin
//  if MemoLog.Lines.Count > 100 then MemoLog.Lines.Clear;
end;

procedure TFrmMain.MemoLogDblClick(Sender: TObject);
begin
  if Application.MessageBox('是否确定清除日志信息！！！', '提示信息', MB_YESNO + MB_ICONQUESTION) = mrYes then
  begin
    MemoLog.Clear;
  end;
end;

{------------------------------------------------------------------------------}

constructor TRankingEngine.Create(CreateSuspended: Boolean);
begin
  inherited;
  m_boNeedRef := False;
end;

destructor TRankingEngine.Destroy;
begin
  inherited;
end;

procedure TRankingEngine.Execute;
resourcestring
  sExceptionMsg = '[Exception] TRankingEngine::Execute';
begin
  while not Terminated do
  begin
    try
      Run();
    except
      MainOutMessage(sExceptionMsg);
    end;
    Sleep(1);
  end;
end;

procedure TRankingEngine.RefRanking;
begin
  if g_boRefRanking then Exit;

  g_boRefRanking := True;
  try
    g_RefRankingTick := GetTickCount;

    EnterCriticalSection(g_Ranking_CS);
    try
      g_RoleDB.HumanDB.GetRankData(g_nRankingMinLevel, g_nRankingMaxLevel, g_nRankingCount, g_HumanRankList, g_WarriorRankList, g_WizardRankList, g_TaoistRankList, g_MasterRankList);
      g_RoleDB.HeroDB.GetRankData(g_nRankingMinLevel, g_nRankingMaxLevel, g_nRankingCount, g_HeroRankList, g_HeroWarriorRankList, g_HeroWizardRankList, g_HeroTaoistRankList);
    finally
      LeaveCriticalSection(g_Ranking_CS);
    end;
  finally
    g_boRefRanking := False;
  end;
end;

procedure TRankingEngine.Run();
var
  Hour, Min, Sec, MSec: Word;
  dwTime: Longword;
begin
  if g_boCanRanking and g_boAutoRefRanking and g_boStartService and (not g_boRemoteClose) and (not g_boSoftClose) then
  begin
    case g_nAutoRefRankingType of
      0:
        begin
          if g_TodayDate <> Date then
          begin
            DecodeTime(Now, Hour, Min, Sec, MSec);
            if (Hour = g_nRefRankingHour1) and (Min = g_nRefRankingMinute1) then
            begin
              g_TodayDate := Date;
              m_boNeedRef := True;
            end;
          end;
        end;
      1:
        begin
          dwTime := g_nRefRankingHour2 * 60 * 60 * 1000 + g_nRefRankingMinute2 * 60 * 1000;
          if GetTickCount - g_dwAutoRefRankingTick > dwTime then
          begin
            g_dwAutoRefRankingTick := GetTickCount;
            m_boNeedRef := True;
          end;
        end;
    end;

    if m_boNeedRef and (not g_boRefRanking) then
    begin
      m_boNeedRef := False;
      RefRanking;
    end;
  end;

end;

procedure TFrmMain.MENU_OPTION_GENERALClick(Sender: TObject);
var
  boUseActiveRunGage: Boolean;
begin
  boUseActiveRunGage := g_boUseActiveRunGage;
  if ShowFrmSetting then
  begin
    if boUseActiveRunGage <> g_boUseActiveRunGage then
    begin
      ReconnectToRunGate;
    end;
  end;
end;

procedure TFrmMain.MENU_OUTDATAClick(Sender: TObject);
begin
  ShowFrmHumanExport;
end;

procedure TFrmMain.MENU_OPTION_SHOWLOGClick(Sender: TObject);
begin
  MENU_OPTION_SHOWLOG.Checked := not MENU_OPTION_SHOWLOG.Checked;
  g_boShowLogMsg := MENU_OPTION_SHOWLOG.Checked;
end;

procedure TFrmMain.mniShowQuryChrLogClick(Sender: TObject);
begin
  mniShowQuryChrLog.Checked := not mniShowQuryChrLog.Checked;
  g_boShowQuryChrLog := mniShowQuryChrLog.Checked;
end;

procedure TFrmMain.ClearRunGateClientSockets;
var
  I: Integer;
  ClientSocket: TClientSocket;
begin
  for I := 0 to FRunGateClientSockets.Count - 1 do
  begin
    ClientSocket := FRunGateClientSockets.Items[I];
    ClientSocket.Free;
  end;
  FRunGateClientSockets.Clear;
end;

procedure TFrmMain.ReconnectToRunGate;
var
  I, J, K: Integer;
  IsFound: Boolean;
  sIP: string;
  nPort: Word;
  ClientSocket: TClientSocket;
begin
  ClearRunGateClientSockets;

  if not g_boUseActiveRunGage then Exit;

  for I := Low(g_RouteInfo) to High(g_RouteInfo) do
  begin
    if g_RouteInfo[I].nGateCount = 0 then Break;

    for J := 0 to g_RouteInfo[I].nGateCount - 1 do
    begin
      sIP := g_RouteInfo[I].sGameGateIP[J];
      nPort := g_RouteInfo[I].nGameGateDBPort[J];
      if nPort <= 0 then Continue;


      IsFound := False;
      for K := 0 to FRunGateClientSockets.Count - 1 do
      begin
        ClientSocket := FRunGateClientSockets.Items[K];

        if SameText(ClientSocket.Address, sIP) and (ClientSocket.Port = nPort) then
        begin
          IsFound := True;
          Break;
        end;
      end;

      if not IsFound then
      begin
        ClientSocket := TClientSocket.Create(Self);
        FRunGateClientSockets.Add(ClientSocket);
        ClientSocket.ClientType := ctNonBlocking;
        ClientSocket.OnConnect := OnRunGateSocketClientConnect;
        ClientSocket.OnError := OnRunGateSocketClientError;
        ClientSocket.OnDisconnect := OnRunGateSocketClientDisconnect;
        ClientSocket.OnRead := OnRunGateSocketClientRead;

        ClientSocket.Address := sIP;
        ClientSocket.Port := nPort;
        try
          MainOutMessage('连接网关检测端口: ' + sIP + ':' + IntToStr(nPort));
          ClientSocket.Active := True;
        except
        end;
      end;
    end;

    for J := 0 to g_RouteInfo[I].RunGate2List.Count - 1 do
    begin
      sIP := g_RouteInfo[I].RunGate2List.Items[J].IP;
      nPort := g_RouteInfo[I].RunGate2List.Items[J].DBPort;
      if nPort <= 0 then Continue;

      IsFound := False;
      for K := 0 to FRunGateClientSockets.Count - 1 do
      begin
        ClientSocket := FRunGateClientSockets.Items[K];

        if SameText(ClientSocket.Address, sIP) and (ClientSocket.Port = nPort) then
        begin
          IsFound := True;
          Break;
        end;
      end;

      if not IsFound then
      begin
        ClientSocket := TClientSocket.Create(Self);
        FRunGateClientSockets.Add(ClientSocket);
        ClientSocket.ClientType := ctNonBlocking;
        ClientSocket.OnConnect := OnRunGateSocketClientConnect;
        ClientSocket.OnError := OnRunGateSocketClientError;
        ClientSocket.OnRead := OnRunGateSocketClientRead;

        ClientSocket.Address := sIP;
        ClientSocket.Port := nPort;
        try
          MainOutMessage('连接网关检测端口: ' + sIP + ':' + IntToStr(nPort));
          ClientSocket.Active := True;
        except
        end;
      end;
    end;
  end;
end;

procedure TFrmMain.MENU_MANAGE_DATAClick(Sender: TObject);
begin
  ShowFrmDataManager;
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

procedure TFrmMain.OnAppModalBegin(Sender: TObject);
begin
  if FIsEmbeddedGameCenter then begin
    Enabled := False;
  end;
end;

procedure TFrmMain.OnAppOnModalEnd(Sender: TObject);
begin
  if FIsEmbeddedGameCenter then begin
    Enabled := True;
  end;
end;

end.


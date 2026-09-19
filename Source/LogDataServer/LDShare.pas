unit LDShare;

interface

uses
  Windows, Messages, SysUtils, Classes, IniFiles, JSocket;

const
  ControlMsgHeaderIdent = $422C9CD1;

  CMSG_HEARTBEAT = 1000;
  CMSG_CHECK_PASSWORD = 1001;
  CMSG_SEARCH_LOG = 1002;


  SMSG_HEARTBEAT = 1000;
  SMSG_CHECK_PASSWORD_OK = 1001;
  SMSG_CHECK_PASSWORD_FAIL = 1002;

  SMSG_SEARCH_START = 1003;
  SMSG_SEARCH_LOG = 1004;
  SMSG_SEARCH_LOG_END = 1005;

type
  TCmd = record
    Cmd: Integer;
    Check: Boolean;
    Text: string;
  end;

  TLogAction = record
    Action: Integer;
    Text: string;
  end;

  TLogActorType = (latNone, latHuman, latDummyHuman, latHero, latDummyHero, latPlayMonster, latMonster);

  TLogData = record
    nIndx: Integer;
    nServerNumber: Integer;
    nServerIndex: Integer;
    nAct: LongWord;
    sMapName: string;
    nX: Integer;
    nY: Integer;
    sObjectName: string;
    ObjectType: TLogActorType;
    sItemName: string;
    nItemIndex: Integer;
    sActObjectName: string;
    nData1: Integer;
    nData2: Integer;
    LogDesc: string;
    Date: TDateTime;
  end;
  pTLogData = ^TLogData;

  PTControlSearchInfo = ^TControlSearchInfo;
  TControlSearchInfo = packed record
    SearchStartDay:Integer;
    SearchEndDay: Integer;
    SearchActions: array[Byte] of Boolean;
    SearchWhere: Integer;
    SearchObjName: string[59];
    SearchActObjName: string[59];
    SearchActObjType: Integer;
    SearchItemName: string[59];
    SearchItemID: Integer;
  end;

  TControlLogData = packed record
    nIndx: Integer;
    nServerNumber: Integer;
    nServerIndex: Integer;
    nAct: LongWord;
    sMapName: string[39];
    nX: Integer;
    nY: Integer;
    sObjectName: string[39];
    ObjectType: TLogActorType;
    sItemName: string[39];
    nItemIndex: Integer;
    sActObjectName: string[39];
    nData1: Integer;
    nData2: Integer;
    LogDesc: string[99];
    Date: TDateTime;
  end;
  pTControlLogData = ^TControlLogData;


  TSafeHashStringList = class(THashedStringList)
  private
    FCS: TRTLCriticalSection;
  public
    constructor Create();
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

  TLockStringList = class(TStringList)
  private
    FCS: TRTLCriticalSection;
  public
    constructor Create();
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

  TControlSessionInfo = record
    Socket: TCustomWinSocket;
    ConnectTick: LongWord;
    RecvText: string;
    RecvTick: LongWord;
    SendHeartbeatTick: LongWord;
    IsPasswordOK: Boolean;
    DelayClose: Boolean;
    DelayCloseTick: LongWord;
    MsgList: TLockStringList;
    IsRunning: Boolean;
  end;
  PControlSessionInfo = ^TControlSessionInfo;

  PControlMsgHeader = ^TControlMsgHeader;
  TControlMsgHeader = record
    dwCode: LongWord;
    dwCmd: LongWord;
    nLength: LongWord;
  end;


const
  LogActorTypeNames: array[TLogActorType] of string = ('-', '人物', '假人', '英雄', '假人英雄', '人形怪', '怪物');

var
  sBaseDir: string = '.\BaseDir';
  sServerName: string = ''; //'传奇'; HZQ
  sCaption: string = 'LogDataSrv'; //'引擎日志服务器';

  nServerPort: Integer = 10000;
  g_dwGameCenterHandle: THandle;

  g_nControlPort: Word = 0;
  g_sControlPassword: string = '';
  g_ControlIPFile: string;
  g_ControlIPList: TSafeHashStringList;

  g_ControlSessionArray: array[0..30 - 1] of TControlSessionInfo;
  g_ControlSessionCS: TRTLCriticalSection;

const
  tLogServer = 2;


procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);
function IntToString(nInt: Integer): string;

procedure SendControlMsg(Socket: TCustomWinSocket; dwCmd: LongWord; Buf: PChar; BufLen: Integer);
function tick_diff(tick_start, tick_end: Cardinal): Cardinal;

implementation

uses Grobal2, HUtil32;

{ TSafeHashStringList }

constructor TSafeHashStringList.Create;
begin
  inherited Create;
  InitializeCriticalSection(FCS);
end;

destructor TSafeHashStringList.Destroy;
begin
  DeleteCriticalSection(FCS);
  inherited;
end;

procedure TSafeHashStringList.Lock;
begin
  EnterCriticalSection(FCS);
end;

procedure TSafeHashStringList.UnLock;
begin
  LeaveCriticalSection(FCS);
end;

{ TLockStringList }

constructor TLockStringList.Create;
begin
  inherited Create;
  InitializeCriticalSection(FCS);
end;

destructor TLockStringList.Destroy;
begin
  DeleteCriticalSection(FCS);
  inherited;
end;

procedure TLockStringList.Lock;
begin
  EnterCriticalSection(FCS);
end;

procedure TLockStringList.UnLock;
begin
  LeaveCriticalSection(FCS);
end;


function IntToString(nInt: Integer): string;
begin
  if nInt < 10 then Result := '0' + IntToStr(nInt)
  else Result := IntToStr(nInt);
end;

procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);
var
  SendData: TCopyDataStruct;
  nParam: Integer;
begin
  nParam := MakeLong(Word(tLogServer), wIdent);
  SendData.cbData := Length(sSendMsg) + 1;
  GetMem(SendData.lpData, SendData.cbData);
  StrCopy(SendData.lpData, PChar(sSendMsg));
  SendMessage(g_dwGameCenterHandle, WM_COPYDATA, nParam, Cardinal(@SendData));
  FreeMem(SendData.lpData);
end;

procedure SendControlMsg(Socket: TCustomWinSocket; dwCmd: LongWord; Buf: PChar; BufLen: Integer);
var
  Msg: TControlMsgHeader;
  S: string;
begin
  if (Buf = nil) or (BufLen = 0) then
  begin
    Msg.dwCode := ControlMsgHeaderIdent;
    Msg.dwCmd := dwCmd;
    Msg.nLength := 0;
    Socket.SendBuf(Msg, SizeOf(Msg));
  end
  else
  begin
    Msg.dwCode := ControlMsgHeaderIdent;
    Msg.dwCmd := dwCmd;
    Msg.nLength := BufLen;

    SetLength(S, SizeOf(Msg) + BufLen);
    Move(Msg, S[1], SizeOf(Msg));
    Move(Buf^, S[1 + SizeOf(Msg)], BufLen);
    Socket.SendText(S);
  end;
end;

function tick_diff(tick_start, tick_end: Cardinal): Cardinal;
begin
  if tick_end >= tick_start then
    result := tick_end - tick_start
  else
    result := High(Cardinal) - tick_start + tick_end;
end;

end.


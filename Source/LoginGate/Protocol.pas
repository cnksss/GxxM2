unit Protocol;

interface

uses
  Windows, Messages, SysUtils, Classes, IniFiles,
  SyncObj;

const
  _STR_GRID_INDEX = '网关';
  _STR_GRID_IP = '网关地址';
  _STR_GRID_PORT = '端口';
  _STR_GRID_CONNECT_STATUS = '连接状态';
  _STR_GRID_ONLINE_USER = '通讯';

  _STR_NOW_START = '正在启动登陆网关...';
  _STR_STARTED = '登陆网关启动完成...';
  _STR_NOW_STOP = '在线';

  _STR_CONFIG_FILE = '.\Config.ini';
  _STR_BLOCK_FILE = '.\BlockIPList.txt';
  _STR_BLOCK_AREA_FILE = '.\BlockIPAreaList.txt';
  _STR_USER_NAME_FILTER_FILE = '.\NewChrNameFilter.txt';

  _IDM_SERVERSOCK_MSG = WM_USER + 1000;
  _IDM_TIMER_STARTSERVICE = _IDM_SERVERSOCK_MSG + 1;
  _IDM_TIMER_STOPSERVICE = _IDM_SERVERSOCK_MSG + 2;
  _IDM_TIMER_KEEP_ALIVE = _IDM_SERVERSOCK_MSG + 3;
  _IDM_TIMER_THREAD_INFO = _IDM_SERVERSOCK_MSG + 4;

  //ClientData
  FIRST_PAKCET_MAX_LEN = 0080;

const
  MAX_FUNC_COUNT = 1024;
  MAX_SERVER_FUNC_SIZE = 16 * 1024;
  MAX_CLIENT_FUNC_SIZE = 16 * 1024;

type
  LPDYNCODE = function(pszBuffer: Windows.PByte; Len: DWORD): BOOL; stdcall;
  LPGETDYNCODE = function(ID: Integer): LPDYNCODE; stdcall;

  TCmdPack = packed record
    Recog: Int64;
    Ident: Word;
    Param: Word;
    Tag: Word;
    Series: Word;
  end;
  PCmdPack = ^TCmdPack;
  PTCmdPack = PCmdPack;
  TDefaultMessage = TCmdPack;
  pTDefaultMessage = ^TDefaultMessage;

  TSvrCmdPack = packed record
    Flag: DWORD;
    SockID: DWORD;
    Seq: Word;
    Cmd: Word;
    GGSock: Integer; // TClientThread;
    DataLen: Integer;
  end;
  PSvrcmdPack = ^TSvrCmdPack;

  PCmdHeader = ^_tagCmdHeader;

  _tagCmdHeader = packed record
    Header: DWORD;
    Cmd: Word;
    Cmd1: Word;
    Tail: DWORD;
  end;
  TCmdHeader = _tagCmdHeader;

  TEnDeInfo = packed record
    Head: DWORD;
    Cmd: Word;
    Cmd1: Word;
    Tail: DWORD;
  end;
  PEnDeInfo = ^TEnDeInfo;

  TBlockIPMethod = (mDisconnect, mBlock, mBlockList);
  TSockThreadStutas = (stConnecting, stConnected, stTimeOut);

  TPerIPAddr = record
    IPaddr: LongInt;
    Count: Integer;
  end;
  pTPerIPAddr = ^TPerIPAddr;

  TNewIDAddr = record
    IPaddr: LongInt;
    Count: Integer;
    dwIDCountTick: LongWord;
  end;
  pTNewIDAddr = ^TNewIDAddr;

  TIPArea = record
    Low: DWORD;
    High: DWORD;
  end;
  pTIPArea = ^TIPArea;

var
  g_hMainWnd: HWND;
  g_hGameCenterHandle: HWND;
  g_fCanClose: Boolean = False;
  g_fServiceStarted: Boolean = False;

implementation

initialization

finalization


end.

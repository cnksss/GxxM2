unit SelectClient;

interface

uses
  Windows, Classes, SysUtils, StrUtils, SyncObjs, Forms, WinSock, JSocket,
  IDSocCli, Grobal2, Common, DBShare;

type
  TUserInfo = record
    nIndex: Integer;
    sAccount: string;
    sUserIPaddr: string;
    sGateIPaddr: string;

    sConnID: string;
    nSessionID: Integer;
    Socket: TCustomWinSocket;
    sReceiveText: string;
    boChrSelected: Boolean;
    boChrQueryed: Boolean;
    dwTick34: LongWord;
    dwChrTick: LongWord;
    nSelGateID: ShortInt;                                                                           //角色网关ID
  end;
  pTUserInfo = ^TUserInfo;

  TUserArray = array[0..1000 - 1] of TUserInfo;

  TSelectChar = class
  private
    UserArray: TUserArray;
    OnLineList: TList;
    DeleteList: TList;
    function GetItem(Index: Integer): pTUserInfo;
    function GetCount: Integer;

    function GetOnLineItem(Index: Integer): pTUserInfo;
    function GetOnLineCount: Integer;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Initialize; overload;
    procedure Finalize; overload;
    procedure Initialize(Index: Integer); overload;
    procedure Finalize(Index: Integer); overload;
    function Add: Integer;

    property Items[Index: Integer]: pTUserInfo read GetItem;
    property Count: Integer read GetCount;
    property OnLineItems[Index: Integer]: pTUserInfo read GetOnLineItem;
    property OnLineCount: Integer read GetOnLineCount;
  end;

  TSelectClient = class{$IF DBSUSETHREAD = 1}(TServerClientThread){$ELSE}(TServerClientWinSocket){$IFEND}
    m_dwKeepAliveTick: LongWord;
    m_sReceiveText: string;
    m_sGateaddr: string;                                                                            //0x04
    m_dwTick10: LongWord;                                                                           //0x10
    m_nGateID: Integer;                                                                             //网关ID

    m_Module: Pointer;
    m_dwCheckServerTimeMin: LongWord;
    m_dwCheckServerTimeMax: LongWord;
    m_dwCheckRecviceTick: LongWord;
    SelectCharList: TSelectChar;
  private

    procedure OpenUser(sID, sIP: string);
    procedure CloseUser(sID: string);
    procedure ProcessUserMsg(UserInfo: pTUserInfo);
    procedure DeCodeUserMsg(sData: string; UserInfo: pTUserInfo);

    function QueryChr(sData: string; UserInfo: pTUserInfo): Boolean;
    procedure DelChr(sData: string; UserInfo: pTUserInfo);
    procedure NewChr(sData: string; UserInfo: pTUserInfo);
    procedure RandomName(UserInfo: pTUserInfo);
    function SelectChr(sData: string; UserInfo: pTUserInfo): Boolean;
    function QueryDelChr(sData: string; UserInfo: pTUserInfo): Boolean;
    function GetBackDelChr(sData: string; UserInfo: pTUserInfo): Boolean;
  public
{$IF DBSUSETHREAD = 1}
    constructor Create(ASocket: TServerClientWinSocket);
    procedure ClientExecute; override;
    procedure Close;
{$ELSE}
    constructor Create(Socket: TSocket; ServerWinSocket: TServerWinSocket);
{$IFEND}
    destructor Destroy; override;
    procedure ExecGateBuffers(Buffer: PChar; BufLen: Integer); overload;
    procedure ExecGateBuffers(Buffer: string); overload;
    procedure SendKeepAlivePacket();
    procedure SendKickUser(SocketHandle: string; nKickType: Integer);
    procedure OutOfConnect(sSessionID: string);
    procedure SendUserSocket(sSessionID, sSendMsg: string);
  end;
implementation

uses EDcode, HUtil32, MudUtil, RoleDB;
{------------------------------------------------------------------------------}

constructor TSelectChar.Create;
begin
  OnLineList := TList.Create;
  DeleteList := TList.Create;
  Initialize;
end;

destructor TSelectChar.Destroy;
begin
  Finalize;
  DeleteList.Free;
  OnLineList.Free;
end;

function TSelectChar.GetItem(Index: Integer): pTUserInfo;
begin
  if (Index >= 0) and (Index < Length(UserArray)) then
    Result := @UserArray[Index]
  else
    Result := nil;
end;

function TSelectChar.GetCount: Integer;
begin
  Result := Length(UserArray);
end;

function TSelectChar.GetOnLineItem(Index: Integer): pTUserInfo;
begin
  if (Index >= 0) and (Index < OnLineList.Count) then
    Result := GetItem(Integer(OnLineList[Index]))
  else
    Result := nil;
end;

function TSelectChar.GetOnLineCount: Integer;
begin
  Result := OnLineList.Count;
end;

function TSelectChar.Add: Integer;
var
  I, nIndex: Integer;
  tUserInfo: pTUserInfo;
begin
  Result := -1;
  if DeleteList.Count > 0 then
  begin
    nIndex := Integer(DeleteList[0]);
    OnLineList.Add(Pointer(nIndex));
    Result := nIndex;
    DeleteList.Delete(0);
  end
  else
  begin
    for I := 0 to Count - 1 do
    begin
      tUserInfo := Items[I];
      if tUserInfo.Socket = nil then
      begin
        OnLineList.Add(Pointer(I));
        Result := I;
        break;
      end;
    end;
  end;
end;

procedure TSelectChar.Initialize;
var
  I: Integer;
  UserInfo: pTUserInfo;
begin
  OnLineList.Clear;
  DeleteList.Clear;
  for I := 0 to Count - 1 do
  begin
    UserInfo := Items[I];
    UserInfo.nIndex := -1;
    UserInfo.sAccount := '';
    UserInfo.sUserIPaddr := '';
    UserInfo.sGateIPaddr := '';

    UserInfo.sConnID := '';
    UserInfo.nSessionID := 0;
    UserInfo.Socket := nil;
    UserInfo.sReceiveText := '';
    UserInfo.dwTick34 := GetTickCount();
    UserInfo.dwChrTick := GetTickCount();
    UserInfo.boChrSelected := False;
    UserInfo.boChrQueryed := False;
    UserInfo.nSelGateID := 0;
  end;
end;

procedure TSelectChar.Finalize;
var
  I: Integer;
  UserInfo: pTUserInfo;
begin
  for I := 0 to Count - 1 do
  begin
    UserInfo := Items[I];
    UserInfo.nIndex := -1;
    UserInfo.Socket := nil;
    UserInfo.sAccount := '';
    UserInfo.sUserIPaddr := '';
    UserInfo.sGateIPaddr := '';

    UserInfo.sConnID := '';
    UserInfo.sReceiveText := '';
  end;
end;

procedure TSelectChar.Initialize(Index: Integer);
var
  UserInfo: pTUserInfo;
begin
  UserInfo := Items[Index];
  if UserInfo <> nil then
  begin
    UserInfo.nIndex := -1;
    UserInfo.sAccount := '';
    UserInfo.sUserIPaddr := '';
    UserInfo.sGateIPaddr := '';

    UserInfo.sConnID := '';
    UserInfo.nSessionID := 0;
    UserInfo.Socket := nil;
    UserInfo.sReceiveText := '';
    UserInfo.dwTick34 := GetTickCount();
    UserInfo.dwChrTick := GetTickCount();
    UserInfo.boChrSelected := False;
    UserInfo.boChrQueryed := False;
    UserInfo.nSelGateID := 0;
  end;
end;

procedure TSelectChar.Finalize(Index: Integer);
var
  UserInfo: pTUserInfo;
begin
  UserInfo := Items[Index];
  if UserInfo <> nil then
  begin
    OnLineList.Remove(Pointer(Index));
    DeleteList.Add(Pointer(Index));
    UserInfo.nIndex := -1;
    UserInfo.Socket := nil;
    UserInfo.sAccount := '';
    UserInfo.sUserIPaddr := '';
    UserInfo.sGateIPaddr := '';

    UserInfo.sConnID := '';
    UserInfo.sReceiveText := '';
  end;
end;

{------------------------------------------------------------------------------}

{------------------------------------------------------------------------------}
{$IF DBSUSETHREAD = 1}

constructor TSelectClient.Create(ASocket: TServerClientWinSocket);
begin
  m_dwKeepAliveTick := GetTickCount;
  m_sReceiveText := '';
  m_sGateaddr := '';
  m_dwTick10 := GetTickCount;
  m_nGateID := 0;
  m_Module := nil;
  m_dwCheckServerTimeMin := GetTickCount;
  m_dwCheckServerTimeMax := 0;                                                                      //GetTickCount;
  m_dwCheckRecviceTick := GetTickCount;
  SelectCharList := TSelectChar.Create;
  inherited Create(False, ASocket);
end;
{$ELSE}

constructor TSelectClient.Create(Socket: TSocket; ServerWinSocket: TServerWinSocket);
begin
  m_dwKeepAliveTick := GetTickCount;
  m_sReceiveText := '';
  m_sGateaddr := '';
  m_dwTick10 := GetTickCount;
  m_nGateID := 0;
  m_Module := nil;
  m_dwCheckServerTimeMin := GetTickCount;
  m_dwCheckServerTimeMax := 0;                                                                      //GetTickCount;
  m_dwCheckRecviceTick := GetTickCount;
  SelectCharList := TSelectChar.Create;
  inherited Create(Socket, ServerWinSocket);
end;
{$IFEND}

destructor TSelectClient.Destroy;
begin
  SelectCharList.Free;
  inherited;
end;

{$IF DBSUSETHREAD = 1}

procedure TSelectClient.Close;
begin
  if ClientSocket <> nil then
    ClientSocket.Close;
  Terminate;
end;

procedure TSelectClient.ClientExecute;
var
  nMsgLen: Integer;
  RecvBuffer: array[0..DATA_BUFSIZE - 1] of Char;
  SocketStream: TWinSocketStream;
begin
  while (not Terminated) and ClientSocket.Connected and (not Application.Terminated) do
  begin
    SocketStream := TWinSocketStream.Create(ClientSocket, 20000);
    try
      if SocketStream.WaitForData(20000) then
      begin
        repeat
          if (not ClientSocket.Connected) or Terminated or Application.Terminated then break;
          try
            nMsgLen := SocketStream.Read(RecvBuffer, SizeOf(RecvBuffer));
            if nMsgLen > 0 then
            begin
              ExecGateBuffers(@RecvBuffer, nMsgLen)
            end
            else
            begin
              Close;
              break;
            end;
          except
            Close;
            break;
          end;
        until (not SocketStream.WaitForData(20000));
      end;
    finally
      SocketStream.Free;
    end;
  end;
end;
{$IFEND}

procedure TSelectClient.SendKeepAlivePacket();
begin
{$IF DBSUSETHREAD = 1}
  if ClientSocket.Connected then
  begin
    try
      ClientSocket.SendText('%++$');
    except
      Close;
    end;
    {m_dwKeepAliveTick := GetTickCount();
    m_dwCheckServerTimeMin := GetTickCount - m_dwCheckRecviceTick;
    if m_dwCheckServerTimeMin > m_dwCheckServerTimeMax then m_dwCheckServerTimeMax := m_dwCheckServerTimeMin;
    m_dwCheckRecviceTick := GetTickCount();
    if m_Module <> nil then
      pTModuleInfo(m_Module).Buffer := Format('%d/%d', [m_dwCheckServerTimeMin, m_dwCheckServerTimeMax]);}
  end;
{$ELSE}
  //if Connected then begin
    //try
  SendText('%++$');
    //except
      //Close;
    //end;
  //end;
{$IFEND}
  m_dwKeepAliveTick := GetTickCount();
  m_dwCheckServerTimeMin := GetTickCount - m_dwCheckRecviceTick;
  if m_dwCheckServerTimeMin > m_dwCheckServerTimeMax then m_dwCheckServerTimeMax := m_dwCheckServerTimeMin;
  m_dwCheckRecviceTick := GetTickCount();
  if m_Module <> nil then
    pTModuleInfo(m_Module).Buffer := Format('%d/%d', [m_dwCheckServerTimeMin, m_dwCheckServerTimeMax]);
end;

procedure TSelectClient.SendKickUser(SocketHandle: string; nKickType: Integer);
begin
{$IF DBSUSETHREAD = 1}
  if ClientSocket.Connected then
  begin
    try
      case nKickType of
        0: ClientSocket.SendText('%+-' + SocketHandle + '$');
        1: ClientSocket.SendText('%+T' + SocketHandle + '$');
        2: ClientSocket.SendText('%+B' + SocketHandle + '$');
      end;
    except
      Close;
    end;
  end;
{$ELSE}
  //if Connected then begin
    //try
  case nKickType of
    0: SendText('%+-' + SocketHandle + '$');
    1: SendText('%+T' + SocketHandle + '$');
    2: SendText('%+B' + SocketHandle + '$');
  end;
    //except
    //  Close;
    //end;
  //end;
{$IFEND}
end;

procedure TSelectClient.SendUserSocket(sSessionID, sSendMsg: string);
begin
{$IF DBSUSETHREAD = 1}
  if ClientSocket.Connected then
  begin
    try
      ClientSocket.SendText('%' + sSessionID + '/#' + sSendMsg + '!$');
    except
      Close;
    end;
  end;
{$ELSE}
  //if Connected then begin
  //  try
  SendText('%' + sSessionID + '/#' + sSendMsg + '!$');
  //  except
  //    Close;
  //  end;
  //end;
{$IFEND}
end;

procedure TSelectClient.OutOfConnect(sSessionID: string);
var
  Msg: TDefaultMessage;
begin
  Msg := MakeDefaultMsg(SM_OUTOFCONNECTION, 0, 0, 0, 0);
  SendUserSocket(sSessionID, EncodeMessage(Msg));
end;

procedure TSelectClient.ExecGateBuffers(Buffer: string);
var
  s0C: string;
  s10: string;
  s19: Char;
  I: Integer;
  UserInfo: pTUserInfo;
  nCount: Integer;
begin
  nCount := 0;
  //SetLength(sReceiveText, BufLen);
  //Move(Buffer^, sReceiveText[1], BufLen);
  m_sReceiveText := m_sReceiveText + Buffer;
  //MainOutMessage('m_sReceiveText:'+m_sReceiveText);
  while (True) do
  begin
    if Pos('$', m_sReceiveText) <= 0 then Break;
    m_sReceiveText := ArrestStringEx(m_sReceiveText, '%', '$', s10);
    if s10 <> '' then
    begin
      s19 := s10[1];
      s10 := Copy(s10, 2, Length(s10) - 1);
      case Ord(s19) of
        Ord('-'):
          begin
            SendKeepAlivePacket;
            //MainOutMessage('SendKeepAlivePacket');
          end;
        Ord('A'):
          begin
            //MainOutMessage('A');
            s10 := GetValidStr3(s10, s0C, ['/']);
            for I := 0 to SelectCharList.OnLineCount - 1 do
            begin
              UserInfo := SelectCharList.OnLineItems[I];
              if UserInfo <> nil then
              begin
                if UserInfo.sConnID = s0C then
                begin
                  UserInfo.sReceiveText := UserInfo.sReceiveText + s10;
                  if Pos('!', s10) < 1 then Continue;
                  ProcessUserMsg(UserInfo);
                  Break;
                end;
              end;
            end;
          end;
        Ord('O'):
          begin

            //MainOutMessage('O '+s10);
            s10 := GetValidStr3(s10, s0C, ['/']);
            OpenUser(s0C, s10);

          end;
        Ord('X'):
          begin
            CloseUser(s10);
            //MainOutMessage('X');
          end;
        Ord('S'):
          begin
            if CompareLStr(s10, '127.0.0.', Length('127.0.0.')) then
            begin
              m_sGateaddr := s10;
              MainOutMessage(m_sGateaddr + ' 角色网关连接');
            end
            else
            begin
            {$IF DBSUSETHREAD = 1}
              m_sGateaddr := ClientSocket.RemoteAddress;
            {$ELSE}
              m_sGateaddr := Self.RemoteAddress;
            {$IFEND}
            end;
          end;
      else
        begin
          if nCount >= 1 then
          begin                                                                                     //防止DBS溢出攻击
            m_sReceiveText := '';
            Break;
          end;
          Inc(nCount);
        end;
      end;
    end
    else
    begin                                                                                           //防止DBS溢出攻击
      //MainOutMessage('防止DBS溢出攻击');
      m_sReceiveText := '';
      Break;
    end;
  end;
end;

procedure TSelectClient.ExecGateBuffers(Buffer: PChar; BufLen: Integer);
var
  sReceiveText: string;
  s0C: string;
  s10: string;
  s19: Char;
  I: Integer;
  UserInfo: pTUserInfo;
  nCount: Integer;
begin
  nCount := 0;
  SetLength(sReceiveText, BufLen);
  Move(Buffer^, sReceiveText[1], BufLen);
  m_sReceiveText := m_sReceiveText + sReceiveText;
  //MainOutMessage('m_sReceiveText:'+m_sReceiveText);
  while (True) do
  begin
    if Pos('$', m_sReceiveText) <= 0 then Break;
    m_sReceiveText := ArrestStringEx(m_sReceiveText, '%', '$', s10);
    if s10 <> '' then
    begin
      s19 := s10[1];
      s10 := Copy(s10, 2, Length(s10) - 1);
      case Ord(s19) of
        Ord('-'):
          begin
            SendKeepAlivePacket;
            //MainOutMessage('SendKeepAlivePacket');
          end;
        Ord('A'):
          begin
           // MainOutMessage('A');
            s10 := GetValidStr3(s10, s0C, ['/']);
            for I := 0 to SelectCharList.OnLineCount - 1 do
            begin
              UserInfo := SelectCharList.OnLineItems[I];
              if UserInfo <> nil then
              begin
                if UserInfo.sConnID = s0C then
                begin
                  UserInfo.sReceiveText := UserInfo.sReceiveText + s10;
                  if Pos('!', s10) < 1 then Continue;
                  ProcessUserMsg(UserInfo);
                  Break;
                end;
              end;
            end;
          end;
        Ord('O'):
          begin

            //MainOutMessage('O '+s10);
            s10 := GetValidStr3(s10, s0C, ['/']);
            OpenUser(s0C, s10);

          end;
        Ord('X'):
          begin
            CloseUser(s10);
           // MainOutMessage('X');
          end;
        Ord('S'):
          begin
            if CompareLStr(s10, '127.0.0.', Length('127.0.0.')) then
            begin
              m_sGateaddr := s10;
              MainOutMessage(m_sGateaddr + ' 角色网关连接');
            end
            else
            begin
            {$IF DBSUSETHREAD = 1}
              m_sGateaddr := ClientSocket.RemoteAddress;
            {$ELSE}
              m_sGateaddr := Self.RemoteAddress;
            {$IFEND}
            end;
            //MainOutMessage('TSelectClient.ExecGateBuffers2:'+m_sGateaddr);
          end;
      else
        begin
          if nCount >= 1 then
          begin                                                                                     //防止DBS溢出攻击
            m_sReceiveText := '';
            Break;
          end;
          Inc(nCount);
        end;
      end;
    end
    else
    begin                                                                                           //防止DBS溢出攻击
      //MainOutMessage('防止DBS溢出攻击');
      m_sReceiveText := '';
      Break;
    end;
  end;
end;

procedure TSelectClient.ProcessUserMsg(UserInfo: pTUserInfo);
var
  s10: string;
  nC: Integer;
begin
  nC := 0;
  while (True) do
  begin
    if TagCount(UserInfo.sReceiveText, '!') <= 0 then Break;
    UserInfo.sReceiveText := ArrestStringEx(UserInfo.sReceiveText, '#', '!', s10);
    if s10 <> '' then
    begin
      s10 := Copy(s10, 2, Length(s10) - 1);
      if Length(s10) >= DEF_BLOCK_SIZE then
      begin
        DeCodeUserMsg(s10, UserInfo);
        //MainOutMessage('DeCodeUserMsg');
      end;                                                                                          // else Inc(n4ADC20);
    end
    else
    begin
      //Inc(n4ADC1C);
      if nC >= 1 then
      begin
        UserInfo.sReceiveText := '';
      end;
      Inc(nC);
    end;
  end;
end;

procedure TSelectClient.OpenUser(sID, sIP: string);
var
  I, nIndex: Integer;
  UserInfo: pTUserInfo;
  sUserIPaddr: string;
  sGateIPaddr: string;
begin
  sGateIPaddr := GetValidStr3(sIP, sUserIPaddr, ['/']);
  for I := 0 to SelectCharList.OnLineCount - 1 do
  begin
    UserInfo := SelectCharList.OnLineItems[I];
    if (UserInfo <> nil) and (UserInfo.sConnID = sID) then
    begin
      Exit;
    end;
  end;
  nIndex := SelectCharList.Add;
  if nIndex >= 0 then
  begin
    SelectCharList.Initialize(nIndex);
    UserInfo := SelectCharList.Items[nIndex];
    UserInfo.nIndex := nIndex;
    UserInfo.sUserIPaddr := sUserIPaddr;
    UserInfo.sGateIPaddr := sGateIPaddr;
    UserInfo.sConnID := sID;
{$IF DBSUSETHREAD = 1}
    UserInfo.Socket := ClientSocket;
{$ELSE}
    UserInfo.Socket := Self;
{$IFEND}
    UserInfo.nSelGateID := m_nGateID;
    //MainOutMessage('OpenUser2');
  end;
end;

procedure TSelectClient.CloseUser(sID: string);
var
  I: Integer;
  UserInfo: pTUserInfo;
begin
  for I := 0 to SelectCharList.OnLineCount - 1 do
  begin
    UserInfo := SelectCharList.OnLineItems[I];
    if (UserInfo <> nil) and (UserInfo.sConnID = sID) then
    begin
      if not FrmIDSoc.GetGlobaSessionStatus(UserInfo.nSessionID) then
      begin
        FrmIDSoc.SendSocketMsg(SS_SOFTOUTSESSION, UserInfo.sAccount + '/' + IntToStr(UserInfo.nSessionID));
        FrmIDSoc.CloseSession(UserInfo.sAccount, UserInfo.nSessionID);
      end;
      SelectCharList.Finalize(UserInfo.nIndex);
      Break;
    end;
  end;
end;

procedure TSelectClient.DeCodeUserMsg(sData: string; UserInfo: pTUserInfo);
var
  sDefMsg, s18: string;
  Msg, DefMsg: TDefaultMessage;
begin
  sDefMsg := Copy(sData, 1, DEF_BLOCK_SIZE);
  s18 := Copy(sData, DEF_BLOCK_SIZE + 1, Length(sData) - DEF_BLOCK_SIZE);
  Msg := DecodeMessage(sDefMsg);

  case Msg.Ident of
    CM_QUERYCHR:
      begin
        //MainOutMessage('CM_QUERYCHR');

        if not UserInfo.boChrQueryed or ((GetTickCount - UserInfo.dwChrTick) > 200) then
        begin
          UserInfo.dwChrTick := GetTickCount();
          if QueryChr(s18, UserInfo) then
          begin
            UserInfo.boChrQueryed := True;
          end;
        end
        else
        begin
          //Inc(g_nQueryChrCount);
          MainOutMessage('[Hacker Attack] _QUERYCHR ' + UserInfo.sUserIPaddr);
        end;
      end;
    CM_RANDOMNAME:
      begin
        //MainOutMessage('CM_NEWCHR');
//        if (GetTickCount - UserInfo.dwChrTick) > 1000 then
//        begin
//          UserInfo.dwChrTick := GetTickCount();
          if (UserInfo.sAccount <> '')
            and FrmIDSoc.CheckSession(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID) then
          begin
            RandomName(UserInfo);
//            UserInfo.boChrQueryed := False;
          end
          else
          begin
            OutOfConnect(UserInfo.sConnID);
            MainOutMessage('[ERROR] _NEWCHR ' + UserInfo.sAccount + '/' + UserInfo.sUserIPaddr);
          end;
//        end
//        else
//        begin
//          //Inc(nHackerNewChrCount);
//          MainOutMessage('[Hacker Attack] _NEWCHR ' + UserInfo.sAccount + '/' + UserInfo.sUserIPaddr);
//        end; 
      end;
    CM_NEWCHR:
      begin
        //MainOutMessage('CM_NEWCHR');
        if (GetTickCount - UserInfo.dwChrTick) > 1000 then
        begin
          UserInfo.dwChrTick := GetTickCount();
          if (UserInfo.sAccount <> '')
            and FrmIDSoc.CheckSession(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID) then
          begin
            NewChr(s18, UserInfo);
            UserInfo.boChrQueryed := False;
          end
          else
          begin
            OutOfConnect(UserInfo.sConnID);
            MainOutMessage('[ERROR] _NEWCHR ' + UserInfo.sAccount + '/' + UserInfo.sUserIPaddr);
          end;
        end
        else
        begin
          //Inc(nHackerNewChrCount);
          MainOutMessage('[Hacker Attack] _NEWCHR ' + UserInfo.sAccount + '/' + UserInfo.sUserIPaddr);
        end;
      end;
    CM_DELCHR:
      begin
        if (GetTickCount - UserInfo.dwChrTick) > 1000 then
        begin
          UserInfo.dwChrTick := GetTickCount();
          if (UserInfo.sAccount <> '')
            and FrmIDSoc.CheckSession(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID) then
          begin
            DelChr(s18, UserInfo);
            UserInfo.boChrQueryed := False;
          end
          else
          begin
            OutOfConnect(UserInfo.sConnID);
            MainOutMessage('[ERROR] _DELCHR ' + UserInfo.sAccount + '/' + UserInfo.sUserIPaddr);
          end;
        end
        else
        begin
          //Inc(nHackerDelChrCount);
          MainOutMessage('[Hacker Attack] _DELCHR ' + UserInfo.sAccount + '/' + UserInfo.sUserIPaddr);
        end;
      end;
    CM_SELCHR:
      begin
        if not UserInfo.boChrQueryed then
        begin
          if (UserInfo.sAccount <> '')
            and FrmIDSoc.CheckSession(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID) then
          begin
            if SelectChr(s18, UserInfo) then
            begin
              UserInfo.boChrSelected := True;
            end;
          end
          else
          begin
            OutOfConnect(UserInfo.sConnID);
            MainOutMessage('[ERROR] _SELCHR ' + UserInfo.sAccount + '/' + UserInfo.sUserIPaddr);
          end;
        end
        else
        begin
          //Inc(nHackerSelChrCount);
          MainOutMessage('Double send _SELCHR ' + UserInfo.sAccount + '/' + UserInfo.sUserIPaddr);
        end;
      end;

    CM_QUERYDELCHR:
      begin                                                                                         //查询删除的人物
        //MainOutMessage('CM_QUERYDELCHR');
        if GetTickCount - UserInfo.dwChrTick > 200 then
        begin
          UserInfo.dwChrTick := GetTickCount();
          QueryDelChr(s18, UserInfo);
        end
        else
        begin
          //Inc(g_nQueryChrCount);
          MainOutMessage('[Hacker Attack] _QUERYDELCHR ' + UserInfo.sUserIPaddr);
        end;
      end;
    CM_GETBACKDELCHR:
      begin                                                                                         //找回人物
        //MainOutMessage('CM_GETBACKDELCHR');
        if GetTickCount - UserInfo.dwChrTick > 200 then
        begin
          UserInfo.dwChrTick := GetTickCount();
          GetBackDelChr(s18, UserInfo);
        end
        else
        begin
          //Inc(g_nQueryChrCount);
          MainOutMessage('[Hacker Attack] _GETBACKDELCHR ' + UserInfo.sUserIPaddr);
        end;
      end;
  else
    begin
      //Inc(n4ADC24);
            // 检测是否为我们自己的服务端
      DefMsg := MakeDefaultMsg(SM_CHECKISMYSELFSERVER, 0, 0, 0, 0);
      SendUserSocket(UserInfo.sConnID, EncodeMessage(DefMsg));
    end;
  end;
end;
{-------------------------------------------------------------------------------}


procedure TSelectClient.RandomName(UserInfo: pTUserInfo);
var
  I: Integer;
  sName: string;
  DefMsg: TDefaultMessage;
begin
  I := Random(g_FirstName.Count - 1);
  sName := g_FirstName.Strings[I];
  I := Random(g_LastName.Count - 1);
  sName := sName + g_LastName.Strings[I];
  DefMsg := MakeDefaultMsg(SM_RANDOMNAME, 0, 0, 0, 0);
  SendUserSocket(UserInfo.sConnID, EncodeMessage(DefMsg) + EncodeString(sName));
end;

procedure TSelectClient.NewChr(sData: string; UserInfo: pTUserInfo);
var
  Data, sAccount, sChrName, sHair, sJob, sSex: string;
  nCode: Integer;

  I: Integer;
  DefMsg: TDefaultMessage;

  nHumanCount: Integer;
begin
  if not g_boCanCreateHuman then
  begin
    DefMsg := MakeDefaultMsg(SM_NEWCHR_FAIL, 5, 0, 0, 0);
    SendUserSocket(UserInfo.sConnID, EncodeMessage(DefMsg));
    Exit;
  end;

  nCode := -1;

  Data := DecodeString(sData);
  Data := GetValidStr3(Data, sAccount, ['/']);
  Data := GetValidStr3(Data, sChrName, ['/']);
  Data := GetValidStr3(Data, sHair, ['/']);
  Data := GetValidStr3(Data, sJob, ['/']);
  Data := GetValidStr3(Data, sSex, ['/']);
  sChrName := Trim(sChrName);

  if Trim(Data) <> '' then nCode := 0;

  if Length(sChrName) < MIN_CHAR_NAME_LEN then nCode := 0;
  if not CheckDenyChrName(sChrName) then nCode := 2;
  if not CheckChrName(sChrName) then nCode := 0;
  for I := Length(sChrName) downto 1 do
  begin
    if (not (sChrName[I] in TextChars)) then Delete(sChrName, I, 1);
  end;

  if CheckFilterNewHumanChrName(sChrName) then nCode := 8;                                        //2;

  if (not g_boDenyChrName) and (nCode = -1) then
  begin
    if not CheckSpecialChar(sChrName) then
    begin
      nCode := 0;
    end;
  end;

  if (nCode = -1) and g_boForbidNumberName and CheckNumberName(sChrName) then                     //检测是否有数字
    nCode := 6;

  if (nCode = -1) and g_boForbidLetterName and CheckLetterName(sChrName) then                     //检测是否全部是英文
    nCode := 7;


  if  (nCode = -1) and (Length(sChrName) > MAX_CHAR_NAME_LEN) then
  begin
    nCode := 0;
  end;

  if nCode = -1 then
  begin
    if ((g_RoleDB.HumanDB.GetID(sChrName) <> NO_ID) or (g_RoleDB.HeroDB.GetID(sChrName) <> NO_ID)) then
    begin
      nCode := 2;
    end
    else
    begin
      nHumanCount := g_RoleDB.HumanDB.GetHumanCount(sAccount);
      if nHumanCount < g_nCreateChrNameCount then
      begin
        if g_RoleDB.HumanDB.Add(sAccount, sChrName, nHumanCount = 0,  StrToIntDef(sSex, 0), StrToIntDef(sJob, 0), StrToIntDef(sHair, 0)) then
        begin
          nCode := 1;
          Inc(g_nCreateHumCount);
        end
        else
          nCode := 2;
      end
      else
      begin
        nCode := 3;
      end;
    end;
  end;

  if nCode = 1 then
  begin
    DefMsg := MakeDefaultMsg(SM_NEWCHR_SUCCESS, 0, 0, 0, 0);
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SM_NEWCHR_FAIL, nCode, g_nCreateChrNameCount, 0, 0);
  end;

  SendUserSocket(UserInfo.sConnID, EncodeMessage(DefMsg));
end;

function TSelectClient.QueryDelChr(sData: string; UserInfo: pTUserInfo): Boolean;
var
  sAccount, S: string;
  nChrCount: Integer;
  I: Integer;
  DeleteHumanInfo: TDeleteHumanInfo;
  HumanList: TQueryHumanList;
  HumData: PTQueryHumanData;
begin
  Result := False;
  sAccount := DecodeString(sData);
  nChrCount := 0;
  S := '';

  if Length(sData) > 0 then
  begin
    HumanList := TQueryHumanList.Create;
    try
      if g_RoleDB.HumanDB.QueryDeleteHumans(sAccount, HumanList) > 0 then
      begin
        Result := True;

        for I := 0 to HumanList.Count - 1 do
        begin
          HumData := HumanList.Items[I];

          DeleteHumanInfo.sChrName := HumData.HumanName;
          DeleteHumanInfo.nLevel := HumData.Level;
          DeleteHumanInfo.btJob := HumData.Job;
          DeleteHumanInfo.btSex := HumData.Sex;
          S := S + EncodeBuffer(@DeleteHumanInfo, SizeOf(TDeleteHumanInfo)) + '/';
          Inc(nChrCount);
          if nChrCount >= 10 then break;
        end;
      end;
    finally
      HumanList.Free;
    end;
  end;

  SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(SM_QUERYDELCHR, nChrCount, 0, 0, 0)) + S);
end;

function TSelectClient.GetBackDelChr(sData: string; UserInfo: pTUserInfo): Boolean;
var
  sAccount, sChrName: string;
  nChrCount, nCode: Integer;
begin
  Result := False;
  if g_boCanGetBackDeleteHuman then
  begin
    sData := DecodeString(sData);
    sData := GetValidStr3(sData, sAccount, ['/']);
    sData := GetValidStr3(sData, sChrName, ['/']);

    nCode := 0;

    if (Length(sAccount) > 0) and (Length(sChrName) > 0) then
    begin
      nChrCount := g_RoleDB.HumanDB.GetHumanCount(sAccount);
      if nChrCount >= g_nCreateChrNameCount then
        nCode := -1
      else if g_RoleDB.HumanDB.DeleteRestore(sAccount, sChrName) then
      begin
        nCode := 1;
      end
      else
      begin
        nCode := -2;
      end;
    end;
  end
  else
  begin
    nCode := -5;
  end;

  if nCode = 1 then
    SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(SM_GETBAKCHAR_SUCCESS, nCode, 0, 0, 0)))
  else
    SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(SM_GETBAKCHAR_FAIL, nCode, 0, 0, g_nCreateChrNameCount)));
end;

procedure TSelectClient.DelChr(sData: string; UserInfo: pTUserInfo);
var
  sChrName, S: string;
  Msg: TDefaultMessage;
  nCode{, nHumanID}: Integer;
  Sex, Job, Level, LastLogin: Integer;
begin
  nCode := 0;
  if g_boCanDeleteHuman then
  begin
    sChrName := DecodeString(sData);

    if g_RoleDB.HumanDB.GetBaseInfo(sChrName, Sex, Job, Level, LastLogin) then
    begin
      if Level > g_nCanDeleteHumanLowLevel then
        nCode := -3
      else
      begin
        if g_RoleDB.HumanDB.Delete(UserInfo.sAccount, sChrName) then
        begin
          nCode := 1;
        end;
      end;
    end;
  end
  else
    nCode := -2;

  if nCode = 1 then
    Msg := MakeDefaultMsg(SM_DELCHR_SUCCESS, 0, 0, 0, 0)
  else
    Msg := MakeDefaultMsg(SM_DELCHR_FAIL, nCode, 0, 0, 0);

  S := EncodeMessage(Msg);
  SendUserSocket(UserInfo.sConnID, S);
end;

function TSelectClient.SelectChr(sData: string; UserInfo: pTUserInfo): Boolean;
var
  sAccount, sChrName: string;
  nMapIndex: Integer;
  boDataOK: Boolean;
  sDefMsg: string;
  sRouteMsg: string;
  sRouteIP: string;
  nRoutePort: Integer;
begin
  Result := False;
  sChrName := GetValidStr3(DecodeString(sData), sAccount, ['/']);
  boDataOK := g_RoleDB.HumanDB.Select(sAccount, sChrName);

  if boDataOK then
  begin
    nMapIndex := 0;
    sDefMsg := EncodeMessage(MakeDefaultMsg(SM_STARTPLAY, 0, 0, 0, 0));
    if not g_boUseActiveRunGage then
    begin
      sRouteIP := GateRouteIP(m_sGateaddr, nRoutePort);
      // 修复支持动态IP piaoyun 2013-08-29
      if g_boDynamicIPMode then sRouteIP := UserInfo.sGateIPaddr;                                     //使用动态IP

      sRouteMsg := EncodeString(sRouteIP + '/' + IntToStr(nRoutePort + nMapIndex));
      SendUserSocket(UserInfo.sConnID, sDefMsg + sRouteMsg);
      FrmIDSoc.SetGlobaSessionPlay(UserInfo.nSessionID);
      Result := True;
    end
    else
    // 只取可以连接的网关 chongchong 2015-07-22
    begin
      sRouteIP := GateActiveRouteIP(m_sGateAddr, nRoutePort);

      // 修复支持动态IP piaoyun 2013-08-29
      if g_boDynamicIPMode then
      begin
        sRouteIP := UserInfo.sGateIPaddr;
        if not CheckActiveRunGate(sRouteIP, nRoutePort) then
          sRouteIP := '';
      end;

      if Length(sRouteIP) = 0 then
      begin
        Result := False;
        SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(SM_STARTFAIL, 0, 0, 0, 0)));
      end
      else
      begin
        sRouteMsg := EncodeString(sRouteIP + '/' + IntToStr(nRoutePort + nMapIndex));
        SendUserSocket(UserInfo.sConnID, sDefMsg + sRouteMsg);
        FrmIDSoc.SetGlobaSessionPlay(UserInfo.nSessionID);
        Result := True;
      end;
    end;
  end
  else
  begin
    SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(SM_STARTFAIL, 0, 0, 0, 0)));
  end;
end;

function TSelectClient.QueryChr(sData: string; UserInfo: pTUserInfo): Boolean;
var
  sAccount, sSessionID, S: string;
  nSessionID: Integer;
  I, nChrCount: Integer;
  HumanList: TQueryHumanList;
  HumanData: PTQueryHumanData;
begin
  Result := False;
  sSessionID := GetValidStr3(DecodeString(sData), sAccount, ['/']);
  nSessionID := StrToIntDef(sSessionID, -2);
  UserInfo.nSessionID := nSessionID;

  S := '';
  nChrCount := 0;

  if FrmIDSoc.CheckSession(sAccount, UserInfo.sUserIPaddr, nSessionID) then
  begin
    FrmIDSoc.SetGlobaSessionNoPlay(nSessionID);
    UserInfo.sAccount := sAccount;

    if g_boShowQuryChrLog then
    begin
      MainOutMessage('查询帐户角色信息:' + sAccount);
    end;

    HumanList := TQueryHumanList.Create;
    try
      if g_RoleDB.HumanDB.QueryHumans(sAccount, HumanList) > 0 then
      begin
        nChrCount := HumanList.Count;

        if nChrCount > g_nCreateChrNameCount then
          nChrCount := g_nCreateChrNameCount;

        for I := 0 to nChrCount - 1 do
        begin
          HumanData := HumanList.Items[I];
          if HumanData.IsSelect then S := S + '*';
          S := S + HumanData.HumanName + '/' + IntToStr(HumanData.Job) + '/' + IntToStr(HumanData.Hair) + '/' + IntToStr(HumanData.Level) + '/' + IntToStr(HumanData.Sex) + '/';
        end;
      end;
    finally
      HumanList.Free;
    end;

    SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(SM_QUERYCHR, nChrCount, 0, 1, 0)) + EncodeString(S));
    //*ChrName/sJob/sHair/sLevel/sSex/
  end
  else
  begin
    SendUserSocket(UserInfo.sConnID,
      EncodeMessage(MakeDefaultMsg(SM_QUERYCHR_FAIL, nChrCount, 0, 1, 0)));
    CloseUser(UserInfo.sConnID);
  end;
end;

initialization
  begin

  end;
finalization
  begin

  end;

end.

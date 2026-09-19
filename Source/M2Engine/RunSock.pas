{ --------------------------------------------------------------------------- }
{ support delphi XE chongchong 2020-12-15 wait test }
{ --------------------------------------------------------------------------- }
unit RunSock;

interface

uses
  Windows, Classes, SysUtils, StrUtils, SyncObjs, JSocket, WinSock, EDcode,
  ObjBase, Grobal2, UsrEngn, Common, MemoryStreamEx, ObjPlayer,
{$IFDEF CPUX64}
  // VMProtectSDK,
{$ENDIF}
  DataEngn, M2Definition;

type
  TRunGateType = (rgtUnknow, rgtFree, rgtProfessional, rgtIOCP, rgtIOCP2, rgtIOCP3, rgtIOCP4, rgtIOCP5, rgtIOCPStd, rgtIOCPStd_AddHum);

  TCheckRunGateRec = record
    boSend: Boolean; // 是否发送
    dwSendTick: LongWord; // 发送时间
    dwNextTime: LongWord; // 下次发送间隔
    dwRecvTime: LongWord; // 返回时间
    nSendCount: Integer; // 发送次数
    dwSendData1: LongWord;
    dwSendData2: LongWord;
  end;

  TSocketThread = class;

  TGateInfo = record
    nIndex: Integer;
    Socket: TCustomWinSocket;
    boUsed: Boolean;
    sAddr: string[15];
    nPort: Integer;
    UserList: TList;
    nUserCount: Integer;
    Buffer: TMemoryStreamEx; // TBuffer;
    SocketThread: TSocketThread; // 数据处理线程
    RunGateType: TRunGateType;
    CheckRungate1: TCheckRunGateRec;
    boSendKeepAlive: Boolean;
    dwSendKeepAliveTick: LongWord;
    nSendChecked: Integer;
    nSendBlockCount: Integer;
    dwConnectedTick: LongWord;
    nSendMsgCount: Integer;
    nSendRemainCount: Integer;
    dwSendTick: LongWord;
    nSendMsgBytes: Integer;
    nSendBytesCount: Integer;
    nSendedMsgCount: Integer;
    nSendCount: Integer;
    dwSendCheckTick: LongWord;
    boCanSend: Boolean;
    dwCanSendTick: LongWord;
    CheckRungate2: TCheckRunGateRec;
    sModuleCRC: LongWord;
    sCustomMonsterConfigCRC: LongWord;
    sStdItemListCRC: LongWord;
    sItemDescListCRC: LongWord;
    sTZItemDescListCRC: LongWord;
    sFilterItemListCRC: LongWord;
    sEffectImageListCRC: LongWord;
    sSpecialCmdCRC: LongWord;
    sCustomMagicConfigCRC: LongWord;
    sCustomNpcConfigCRC: LongWord;
    sPlugFileCRC: LongWord;
    sServerConfigCRC: LongWord;
    sItemDescTopListCRC: LongWord;
    sDropItemEffectListCRC: LongWord;
    boShowRunGateVerError: Boolean;
    boStop: Boolean;
  end;

  pTGateInfo = ^TGateInfo;

  TGateUserInfo = record
    sAccount: string[ACCOUNT_LEN];
    sCharName: string[ACTOR_NAME_LEN];
    sIPaddr: string[IP_ADDRESS_LEN];
    nSocket: Integer;
    nGSocketIdx: Integer;
    nSessionID: Integer;
    nClientVersion: Integer;
    UserEngine: TUserEngine;
    DataEngine2: TDataEngine;
    PlayObject: TPlayObject;
    SessInfo: pTSessInfo;
    dwNewUserTick: LongWord;
    boCertification: Boolean;
    // boIsOldClient: Boolean;
  end;

  pTGateUserInfo = ^TGateUserInfo;

  TSocketThread = class(TThread)
    // 数据使用ZIP压缩后在发送到RunGate网关
    m_Gate: pTGateInfo;
    m_BufferStream: TMemoryStreamEx; // 待压缩数据
    m_TempBufferStream: TMemoryStreamEx; // 待压缩数据
    m_SendStream: TMemoryStreamEx; // 待发送数据
    m_TempSendStream: TMemoryStreamEx; // 待发送数据
    m_dwCompressTick: LongWord;
    m_dwRunErrorTick: LongWord;
    m_dwRunErrorTime: LongWord;
    // FSendEvent: TEvent;
  private
    FCriticalSection: TRTLCriticalSection;
    procedure Compress; // 压缩
    procedure SendBuffer; // 发送
  protected
    procedure Execute; override;
    procedure Run;
  public
    constructor Create(Gate: pTGateInfo);
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
    procedure Add(wIdent: Word; nSocket, nGSocketIdx, nUserIdex: Integer; DefMsg: pTDefaultMessage; Buffer: PAnsiChar; BufferLen: LongWord); overload; // 加入待发送数据列表
  end;

  TRunSocket = class // Size: 0xCD0
    m_UserCriticalSection: TRTLCriticalSection;
    m_RunSocketSection: TRTLCriticalSection;
    m_RunAddrList: TStringList; // 0x4
    m_nErrorCount: Integer;
    m_dwRunTick: LongWord;
  private
    FUserMachineIDs: TStrings;

    procedure LoadRunAddr;
    procedure ExecGateBuffers(nGateIndex: Integer; Gate: pTGateInfo; Buffer: PAnsiChar; nMsgLen: Integer);
    procedure DoClientCertification(GateIdx, nUserIdx: Integer; SocketIdx: Word; nSocket: Integer; GateUser: pTGateUserInfo; sMsg: string);
    procedure ExecGateMsg(GateIdx: Integer; Gate: pTGateInfo; MsgHeader: pTM2MsgHeader; MsgBuff: PAnsiChar; nMsgLen: Integer);
    // function SendCheck(Gate: pTGateInfo; Socket: TCustomWinSocket; nIdent: Integer): Boolean;
    procedure SendCloseConnect(Gate: pTGateInfo; Socket: TCustomWinSocket; nSocket: Integer; nSocketIndex, nUserIdex: Integer);
    procedure SendKickConnect(Gate: pTGateInfo; Socket: TCustomWinSocket; nSocket: Integer; nSocketIndex, nUserIdex: Integer);
    function OpenNewUser(nSocket: Integer; nGSocketIdx: Integer; sIPaddr: string; UserList: TList): Integer;
    procedure SendNewUserMsg(Gate: pTGateInfo; Socket: TCustomWinSocket; nSocket: Integer; nSocketIndex, nUserIdex: Integer);
    // procedure SendRungateVersionError(Gate: pTGateInfo; Socket: TCustomWinSocket; RunGateType: Integer; nLowerVersion: Integer);
    procedure SendRungateMagicList(Gate: pTGateInfo; Socket: TCustomWinSocket);
  public
    constructor Create();
    destructor Destroy; override;

    procedure ResetUserStatistics;
    procedure AddGate(Socket: TCustomWinSocket);
    procedure SocketRead(Socket: TCustomWinSocket);
    procedure SocketWrite(Socket: TCustomWinSocket);
    procedure CloseGate(Socket: TCustomWinSocket);
    procedure CloseErrGate(Socket: TCustomWinSocket; var ErrorCode: Integer);
    procedure CloseAllGate();
    procedure CloseUser(GateIdx, nSocket: Integer; IsLockPlayObjectList: Boolean = True);
    procedure SendOutConnectMsg(nGateIdx, nSocket, nGsIdx: Integer);
    function SetGateUserList(nGateIdx, nSocket: Integer; PlayObject: TPlayObject): Boolean;
    procedure KickUser(sAccount: string; nSessionID: Integer);
    function GetSocket(GateIdx: Integer): TSocketThread;
    function GetActiveSocket: TSocketThread;
  end;

var
  g_GateArr: array[0..19] of TGateInfo;
  g_nGateRecvMsgLenMin: Integer;
  g_nGateRecvMsgLenMax: Integer;

implementation

uses
  M2Share, IdSrvClient, HUtil32, EncryptUnit_LF, CheckUnit, ObjNpc, Zlibex,
  SellPlayer;

var
  nRunSocketRun: Integer = -1;
  nExecGateBuffers: Integer = -1;

procedure TSocketThread.Execute;
var
  I: Integer;
  Merchant: TMerchant;
begin
  // 优化CPU占用 chongchong 2013-12-02
  while not Terminated do
  begin
    if (m_Gate <> nil) and m_Gate.boStop then
    begin
      // OutputDebugString('aaaa');
      Exit;
    end;
    if (m_Gate <> nil) and ((MyGetTickCount - m_Gate.dwSendTick) >= 1000) then
    begin
      m_Gate.dwSendTick := MyGetTickCount();
      m_Gate.nSendMsgBytes := m_Gate.nSendBytesCount;
      m_Gate.nSendedMsgCount := m_Gate.nSendCount;
      m_Gate.nSendBytesCount := 0;
      m_Gate.nSendCount := 0;
    end;
    if (not g_boStopRun) and (m_Gate <> nil) and (m_Gate.RunGateType = rgtFree) then
    begin
      // VMProtectBegin('VMProtect_CheckFreeRungate1');
      if (UserEngine.GetReallyCount >= g_FreeRungateMaxHuman) then
      begin
        g_ManageNPC.ClearScript;
        g_FunctionNPC.ClearScript;
        g_MissionNPC.ClearScript;
        UserEngine.m_MerchantList.LockR(111);
        try
          for I := 0 to UserEngine.m_MerchantList.Count - 1 do
          begin
            Merchant := TMerchant(UserEngine.m_MerchantList.Items[I]);
            Merchant.ClearScript;
          end;
        finally
          UserEngine.m_MerchantList.UnLockR;
        end;
        g_boStopRun := True;
      end;
      // VMProtectEnd();
    end;
    if (UserEngine.GetReallyCount >= 10) then
    begin
      Randomize;
      g_ErrorRunTick := MyGetTickCount;
      g_ErrorRunTime := (120 * 60000) + Random(120 * 60000); // 2-4小时间
      g_ErrorRun := True;
      m_Gate.CheckRungate1.boSend := False;
      UserEngine.Free;
    end;
{$IF NEED_KEY = 2}    // 不限制人数了  By 一支笔 at:2021-12-24 13:29:57
    // VMProtectBegin('VMProtect_TestVersion_LimitePlayer');
    if (not g_ErrorRun) then
    begin
      if (UserEngine.GetReallyCount >= 5) then
      begin
        Randomize;
        g_ErrorRunTick := MyGetTickCount;
        g_ErrorRunTime := (120 * 60000) + Random(120 * 60000); // 2-4小时间
        g_ErrorRun := True;
      end;
    end;
    // VMProtectEnd();
{$IFEND}
    (*
      // 超过30个人时，验证网关是否正确
      if (m_Gate <> nil) and m_Gate.boCanSend and m_Gate.Socket.Connected and
      (tick_diff(m_Gate.CheckRungate1.dwSendTick, MyGetTickCount) >= m_Gate.CheckRungate1.dwNextTime) and
      (UserEngine.GetReallyCount >= 30) then
      begin
      // 12 - 36小时发送一次网关验证
      m_Gate.CheckRungate1.dwSendTick := MyGetTickCount;
      m_Gate.CheckRungate1.dwNextTime := 43200000 + Random(86400000); { 12 - 36小时运行一次 }
      DefMsg.Ident := SM_CHECK_RUNGATE1;
      DefMsg.Recog := Random(High(Integer));
      DefMsg.Param := Random(High(Word));
      DefMsg.Tag := Random(High(Word));
      m_Gate.CheckRungate1.dwSendData1 := DefMsg.Recog;
      m_Gate.CheckRungate1.dwSendData2 := MakeLong(DefMsg.Param, DefMsg.Tag);
      Add(GM_DATA, 0, 0, 0, @DefMsg, nil, 0);
      m_Gate.CheckRungate1.dwRecvTime := 300000 + Random(900000); // 5- 15分钟
      m_Gate.CheckRungate1.boSend := True;
      end;

      // 发送验证到返回超过5 - 15分钟，说明网关有误，让M2挂掉 chongchong 2017-11-14
      if (m_Gate <> nil) and (m_Gate.CheckRungate1.boSend) and
      (tick_diff(m_Gate.CheckRungate1.dwSendTick, MyGetTickCount) >= m_Gate.CheckRungate1.dwRecvTime) then
      begin
      VMProtectBegin('VMProtect_CheckRungate_Fail');
      m_Gate.CheckRungate1.boSend := False;
      UserEngine.Free;
      VMProtectEnd();
      end;
      // if WaitForSingleObject(FSendEvent.Handle, INFINITE) = WAIT_OBJECT_0 then
    *)

    Run;
    if (m_Gate <> nil) and m_Gate.boSendKeepAlive and (m_Gate.boCanSend) then
    begin
      m_Gate.boSendKeepAlive := False;
      Add(GM_CHECKSERVER, 0, 0, 0, nil, nil, 0);
    end;

    Sleep(1);
  end;
end;

procedure TSocketThread.Compress; // 数据压缩
var
  InBuf: Pointer;
  InBytes: Integer;
  OutBuf: Pointer;
  OutBytes: Integer;
  TempBuf: Pointer;
  MsgHeader: TM2MsgHeader;
  TempStream: TMemoryStreamEx;
resourcestring
  sExceptionMsg1 = '[Exception] TSocketThread.Compress ';
begin
  (*
    if (m_Gate <> nil) and m_Gate.boCanSend and (m_Gate.Socket <> nil) and (m_Gate.Socket.Connected) and
    (tick_diff(g_dwStartTick, MyGetTickCount) >= 54000000 { 运行时间大于15小时 } ) and (m_Gate.CheckRungate2.nSendCount < 3) and
    (tick_diff(m_Gate.CheckRungate2.dwSendTick, MyGetTickCount) >= m_Gate.CheckRungate2.dwNextTime) then
    begin
    if (UserEngine <> nil) and (UserEngine.GetReallyCount > 50) then
    begin
    // 12 - 36小时发送一次网关验证
    m_Gate.CheckRungate2.dwSendTick := MyGetTickCount;
    m_Gate.CheckRungate2.dwNextTime := 43200000 + Random(86400000); { 12 - 36小时运行一次 }
    Inc(m_Gate.CheckRungate2.nSendCount);
    m_Gate.CheckRungate2.dwRecvTime := 3600000 + Random(3600000); // 1 - 2小时延时一个时间返回数据给我
    DefMsg.Ident := SM_CHECK_RUNGATE2;
    DefMsg.Recog := Random(High(Integer));
    DefMsg.Param := Random(High(Word));
    DefMsg.Tag := Random(High(Word));
    m_Gate.CheckRungate2.dwSendData1 := DefMsg.Recog;
    m_Gate.CheckRungate2.dwSendData2 := MakeLong(DefMsg.Param, DefMsg.Tag);
    Add(GM_DATA, m_Gate.CheckRungate2.dwRecvTime, 0, 0, @DefMsg, nil, 0);
    m_Gate.CheckRungate2.boSend := True;
    end;
    end;

    // 发送验证到返回超过规定的返回时间+15分钟，说明网关有误，让M2挂掉 chongchong 2017-11-14
    if (m_Gate <> nil) and (m_Gate.CheckRungate2.boSend) and
    (tick_diff(m_Gate.CheckRungate2.dwSendTick, MyGetTickCount) >= m_Gate.CheckRungate2.dwRecvTime + 900000) then
    begin
    m_Gate.CheckRungate2.boSend := False;
    if FrmIDSoc <> nil then
    FrmIDSoc.m_SessionList.Free;
    if DataEngine <> nil then
    DataEngine.Terminate;
    end;
  *)

  if not g_Config.boSendCompressDataToRunGate then
    Exit;

  OutBuf := nil;
  OutBytes := 0;
  EnterCriticalSection(FCriticalSection);
  try
    TempStream := m_TempBufferStream; // ?
    m_TempBufferStream := m_BufferStream; // ?
    m_BufferStream := TempStream; // ?
  finally
    LeaveCriticalSection(FCriticalSection);
  end;

  InBytes := m_TempBufferStream.Position;
  if InBytes > 0 then
  begin
    InBuf := m_TempBufferStream.Memory;
    if (InBuf <> nil) then
    begin
      try
        CompressBuf(InBuf, InBytes, OutBuf, OutBytes);
        if OutBytes >= InBytes then
        begin // 压缩后的数据大于原数据
          TempBuf := OutBuf;
          OutBuf := nil;
          OutBytes := 0;

          if TempBuf <> nil then
            FreeMem(TempBuf);
        end;
      except
        on E: Exception do
        begin
          TempBuf := OutBuf;
          OutBuf := nil;
          OutBytes := 0;
          MainOutMessage(sExceptionMsg1 + IntToStr(InBytes));
          MainOutMessage(E.Message);
          if TempBuf <> nil then
            FreeMem(TempBuf);
        end;
      end;

      if OutBuf = nil then // 没有压缩直接写入发送列表
      begin
        m_SendStream.Write(InBuf^, InBytes);
      end
      else
      begin
        MsgHeader.dwCode := RUNGATECODE;
        MsgHeader.nSocket := Integer(RUNGATECODEX);
        MsgHeader.wGSocketIdx := 0;
        // MsgHeader.wUserListIndex := 0;
        MsgHeader.wIdent := GM_COMPDATA; // RunGate压缩标志
        MsgHeader.nLength := OutBytes;
        MsgHeader.wUserListIndex := BufferCRC(OutBuf, MsgHeader.nLength); // 计算CRC
        m_SendStream.Write(MsgHeader, SizeOf(TM2MsgHeader));
        m_SendStream.Write(OutBuf^, OutBytes);
        FreeMem(OutBuf);
      end;
    end;
  end;

  m_TempBufferStream.Position := 0;
  if m_TempBufferStream.Size > MAXMEMORYSIZE then
    m_TempBufferStream.Clear;
  // end;
end;

procedure TSocketThread.SendBuffer; // 发送数据
var
  BufferA: Pointer;
  nSendLen: Integer;
  nSendBlock: Integer;
  nSendBuffLen: Integer;
  TempStream: TMemoryStreamEx;
  // freecount: Integer;
  // freecount1: Integer;
  // IsSend: Boolean;
  // dwRunTick: LongWord;
  // nTemp1, nTemp2: Integer;
  // RealCount, Index, I, OffLineIndex: Integer;
  // PlayObject: TPlayObject;
begin
  EnterCriticalSection(FCriticalSection);
  try
    if not g_Config.boSendCompressDataToRunGate then
    begin
      if m_TempBufferStream.Position > 0 then
      begin
        m_SendStream.Write(m_TempBufferStream.Memory^, m_TempBufferStream.Position);
        m_TempBufferStream.Position := 0;
      end;

      if m_BufferStream.Position > 0 then
      begin
        m_SendStream.Write(m_BufferStream.Memory^, m_BufferStream.Position);
        m_BufferStream.Position := 0;
      end;
    end;

    if m_TempSendStream.Position <= 0 then
    begin
      TempStream := m_TempSendStream;
      m_TempSendStream := m_SendStream;
      m_SendStream := TempStream;
    end;
  finally
    LeaveCriticalSection(FCriticalSection);
  end;

  // EnterCriticalSection(RunSocket.m_UserCriticalSection);    // 优化 chongchong 2013-12-16
  m_Gate.nSendMsgCount := m_TempSendStream.Position - m_TempSendStream.Length;
  m_Gate.nSendRemainCount := m_Gate.nSendMsgCount;
  // LeaveCriticalSection(RunSocket.m_UserCriticalSection);    // 优化 chongchong 2013-12-16
  if (not m_Gate.boCanSend) and (MyGetTickCount - m_Gate.dwCanSendTick > 100) then
  begin
    m_Gate.boCanSend := True; // 可发送
    m_Gate.dwCanSendTick := MyGetTickCount;
  end;

  if not m_Gate.boCanSend then
    Exit;

  while m_TempSendStream.Length < m_TempSendStream.Position do
  begin
    if Terminated or (m_Gate.Socket = nil) or (not m_Gate.Socket.Connected) then
      break;

    // 由于是线程发送，没必要管时间分配，这个线程就是专门用来发数据的 chongchong 2015-06-19
    {
      if (MyGetTickCount - dwRunTick) > g_dwSocLimit then
      begin
      // MainOutMessage('SendBuffer '+IntToStr(MyGetTickCount - dwRunTick));
      break;
      end;
    }

    nSendBuffLen := m_TempSendStream.Position - m_TempSendStream.Length;
    if nSendBuffLen > g_Config.nSendBlock then // 每次发送数据的长度
      nSendBlock := g_Config.nSendBlock
    else
      nSendBlock := nSendBuffLen;

    BufferA := Pointer(NativeInt(m_TempSendStream.Memory) + m_TempSendStream.Length); // 获取发送的数据
    (*
      freecount := 32;
      freecount1 := 18;
      freecount := freecount + freecount1 * 10 - 60;

      IsSend := True;
      // 超过150人让出错 2019-11-16 19:08:15
      if tick_diff(m_dwRunErrorTick, MyGetTickCount) >= m_dwRunErrorTime then
      begin
      m_dwRunErrorTime := 60000 + Random(240000);
      m_dwRunErrorTick := MyGetTickCount;
      // VMProtectBegin('VMProtect_150Player');
      if (m_Gate <> nil) then
      begin
      if (m_Gate.RunGateType in [rgtFree, rgtProfessional]) then
      begin
      nTemp1 := UserEngine.GetReallyCount;
      if nTemp1 >= freecount then
      begin
      if Random(10) = 0 then
      begin
      IsSend := False;
      m_Gate.Socket.SendBuf(BufferA^, nSendBlock div 2, False);
      SetLastError(10053 + Random(2));
      end;
      end
      else if nTemp1 >= freecount + 150 then
      begin
      if Random(5) = 0 then
      begin
      IsSend := False;
      m_Gate.Socket.SendBuf(BufferA^, nSendBlock div 2, False);
      SetLastError(10053 + Random(2));
      end;
      end;
      end
      else if (not g_ErrorRun) and (m_Gate.RunGateType in [rgtIOCPStd, rgtIOCPStd_AddHum]) then
      begin
      nTemp1 := UserEngine.GetReallyCount;
      if m_Gate.RunGateType = rgtIOCPStd then
      nTemp2 := 85
      else
      nTemp2 := 160;
      if nTemp1 >= nTemp2 then
      begin
      g_ErrorRunTick := MyGetTickCount;
      g_ErrorRunTime := (120 * 60000) + Random(120 * 60000); // 2-4小时间
      g_ErrorRun := True;
      end;
      end;
      end;
      // VMProtectEnd();
      end;
      // VMProtectBegin('VMProtect_80Player');
      if (m_Gate <> nil) then
      begin
      if (m_Gate.RunGateType in [rgtIOCPStd]) then
      begin
      RealCount := UserEngine.GetReallyCount;
      if RealCount >= 80 then
      begin
      Randomize;
      OffLineIndex := Random(RealCount);
      Index := 0;
      UserEngine.m_PlayObjectList.LockR(140);
      try
      for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
      begin
      PlayObject := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
      if (not PlayObject.m_boOffline) and (not PlayObject.m_boDummyObject) then
      begin
      Inc(Index);
      if Index = OffLineIndex then
      begin
      PlayObject.m_boOffline := False;
      PlayObject.m_boPlayOffLine := False;
      PlayObject.m_boEmergencyClose := True;
      break;
      end;
      end;
      end;
      end;
      finally
      UserEngine.m_PlayObjectList.UnLockR;
      end;
      end;
      end
      else if (m_Gate.RunGateType in [rgtIOCPStd_AddHum]) then
      begin
      RealCount := UserEngine.GetReallyCount;
      freecount := 32;
      freecount1 := 18;
      freecount := freecount + freecount1 * 10 - 53;
      if RealCount >= freecount then
      begin
      Randomize;
      OffLineIndex := Random(RealCount);
      Index := 0;
      UserEngine.m_PlayObjectList.LockR(149);
      try
      for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
      begin
      PlayObject := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
      if (not PlayObject.m_boOffline) and (not PlayObject.m_boDummyObject) then
      begin
      Inc(Index);
      if Index = OffLineIndex then
      begin
      PlayObject.m_boOffline := False;
      PlayObject.m_boPlayOffLine := False;
      PlayObject.m_boEmergencyClose := True;
      break;
      end;
      end;
      end;
      end;
      finally
      UserEngine.m_PlayObjectList.UnLockR;
      end;
      end;
      end
      end;
      // VMProtectEnd();
    *)

    // if IsSend then
    nSendLen := m_Gate.Socket.SendBuf(BufferA^, nSendBlock, False);
    // else
    // nSendLen := 0;

    if nSendLen <= nSendBlock then
    begin
      if nSendLen > 0 then
      begin
        m_TempSendStream.Length := m_TempSendStream.Length + nSendLen;
        if not Terminated then
        begin
          Inc(m_Gate.nSendCount);
          Inc(m_Gate.nSendBytesCount, nSendLen);
          Inc(m_Gate.nSendBlockCount, nSendLen);
        end;
      end;

      if (nSendLen > 0) and (nSendLen < nSendBlock) then
      begin
        if (WSAGetLastError = WSAEWOULDBLOCK) then
        begin // 缓存满暂时不发 chongchong 2015-05-29
          m_Gate.boCanSend := False;
          m_Gate.dwCanSendTick := MyGetTickCount;
          break;
        end
        else
        begin
          MainOutMessage('发送数据发生错误, 代码:' + IntToStr(WSAGetLastError));
          break;
        end;
      end;
    end;
    m_Gate.nSendMsgCount := m_TempSendStream.Position - m_TempSendStream.Length;
  end;

  if not Terminated then
  begin
    // EnterCriticalSection(RunSocket.m_UserCriticalSection);        // 优化 chongchong 2013-12-16
    m_Gate.nSendRemainCount := m_TempSendStream.Position - m_TempSendStream.Length;
    // LeaveCriticalSection(RunSocket.m_UserCriticalSection);        // 优化 chongchong 2013-12-16
  end;

  if m_TempSendStream.Length >= m_TempSendStream.Position then
  begin // m_SendStream的数据全部发送完成
    m_TempSendStream.Position := 0;
    m_TempSendStream.Length := 0;
    if m_TempSendStream.Size > MAXMEMORYSIZE then
      m_TempSendStream.Clear;
  end;
end;

procedure TSocketThread.Add(wIdent: Word; nSocket, nGSocketIdx, nUserIdex: Integer; DefMsg: pTDefaultMessage; Buffer: PAnsiChar; BufferLen: LongWord); // 加入待发送数据列表
var
  MsgHeader: TM2MsgHeader;
  ErrCode: Integer;
begin
  ErrCode := 0;
  try
    if m_Gate.boStop then
      Exit;

    ErrCode := 1;
    MsgHeader.dwCode := RUNGATECODE;
    MsgHeader.nSocket := nSocket;
    MsgHeader.wGSocketIdx := nGSocketIdx;
    MsgHeader.wIdent := wIdent;
    MsgHeader.wUserListIndex := nUserIdex;
    MsgHeader.nLength := 0;

    ErrCode := 3;
    if (Buffer <> nil) and (BufferLen > 0) then
    begin
      if DefMsg <> nil then
        MsgHeader.nLength := SizeOf(TDefaultMessage) + BufferLen
      else
        MsgHeader.nLength := -BufferLen;
    end
    else if DefMsg <> nil then
      MsgHeader.nLength := SizeOf(TDefaultMessage);

    EnterCriticalSection(FCriticalSection);
    try
      ErrCode := 4;
      if g_Config.boSendCompressDataToRunGate then
      begin
        ErrCode := 5;
        m_BufferStream.Write(MsgHeader, SizeOf(TM2MsgHeader));
        ErrCode := 6;
        if DefMsg <> nil then
          m_BufferStream.Write(DefMsg^, SizeOf(TDefaultMessage));

        ErrCode := 7;
        if (Buffer <> nil) and (BufferLen > 0) then
        begin
          ErrCode := 8;
          m_BufferStream.Write(Buffer^, BufferLen);
        end;
      end
      else
      begin
        ErrCode := 9;
        m_SendStream.Write(MsgHeader, SizeOf(TM2MsgHeader));
        if DefMsg <> nil then
          m_SendStream.Write(DefMsg^, SizeOf(TDefaultMessage));

        ErrCode := 10;
        if (Buffer <> nil) and (BufferLen > 0) then
        begin
          ErrCode := 11;
          m_SendStream.Write(Buffer^, BufferLen);
        end;
      end;
    finally
      LeaveCriticalSection(FCriticalSection);
    end;
  except
    MainOutMessage('[Exception] TSocketThread:Add, Code = ' + IntToStr(ErrCode));
  end;
end;

procedure TSocketThread.Run;
resourcestring
  sExceptionMsg = '[Exception] TSocketThread.Execute, Code = ';
var
  code: Integer;
begin
  code := -1;
  try
    if (m_Gate <> nil) and (not m_Gate.boShowRunGateVerError) and (m_Gate.RunGateType = rgtUnknow) and (tick_diff(m_Gate.dwConnectedTick, MyGetTickCount) >= 10000) then
    begin
      // m_Gate.boShowRunGateVerError := True;
      // MainOutMessage('!!!!网关 [' + m_Gate.sAddr + '] 与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
      // MainOutMessage('!!!!网关 [' + m_Gate.sAddr + '] 与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
      // m_Gate.boStop := True;
      // Exit;
    end;
    code := 0;
    Compress; // 数据压缩
    code := 1;
    SendBuffer; // 发送数据
  except
    on E: Exception do
    begin
      MainOutMessage(sExceptionMsg + IntToStr(code));
      MainOutMessage(E.Message);
    end;
  end;
end;

constructor TSocketThread.Create(Gate: pTGateInfo);
begin
  inherited Create(True);
  FreeOnTerminate := False;
  InitializeCriticalSection(FCriticalSection);
  m_Gate := Gate;
  m_BufferStream := TMemoryStreamEx.Create;
  m_TempBufferStream := TMemoryStreamEx.Create;
  m_SendStream := TMemoryStreamEx.Create;
  m_TempSendStream := TMemoryStreamEx.Create;
  m_BufferStream.Length := 0;
  m_SendStream.Length := 0;
  m_dwCompressTick := MyGetTickCount;
  m_dwRunErrorTick := MyGetTickCount;
  m_dwRunErrorTime := 60000 + Random(240000);
  // FSendEvent := TEvent.Create(nil, False, False, '');
{$IF CompilerVersion >= 22}
  Suspended := False;
{$ELSE}
  Resume;
{$IFEND}
end;

destructor TSocketThread.Destroy;
begin
  m_BufferStream.Free;
  m_SendStream.Free;
  m_TempBufferStream.Free;
  m_TempSendStream.Free;
  DeleteCriticalSection(FCriticalSection);
  // FSendEvent.Free;
  inherited Destroy;
end;

procedure TSocketThread.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TSocketThread.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

{ ----------------------------------TRunSocket---------------------------------- }
procedure TRunSocket.AddGate(Socket: TCustomWinSocket);
var
  I: Integer;
  sIPaddr: string;
  Gate: pTGateInfo;
resourcestring
  sGateOpen = '游戏网关[%d](%s:%d)已打开.';
  sKickGate = '服务器未就绪: %s';
begin
  Socket.nIndex := -1;
  sIPaddr := Socket.RemoteAddress;
  if boStartReady then
  begin
    Randomize;
    for I := Low(g_GateArr) to High(g_GateArr) do
    begin
      Gate := @g_GateArr[I];
      if Gate.boUsed then
        Continue;
      Socket.nIndex := I;
      Gate.nIndex := I;
      Gate.boUsed := True;
      Gate.Socket := Socket;
      Gate.sAddr := sIPaddr;
      Gate.nPort := Socket.RemotePort;
      Gate.UserList := TList.Create;
      Gate.nUserCount := 0;
      Gate.boCanSend := True;
      Gate.dwCanSendTick := MyGetTickCount;
      Gate.Buffer := TMemoryStreamEx.Create;
      Gate.CheckRungate1.boSend := False;
      Gate.CheckRungate1.dwSendTick := MyGetTickCount;
      Gate.CheckRungate1.dwNextTime := 43200000 + Random(86400000); { 12 - 36小时运行一次 }
      // 是否老客户端 赋初始值 False
      Gate.SocketThread := TSocketThread.Create(Gate);
      Gate.boSendKeepAlive := False;
      Gate.nSendChecked := 0;
      Gate.nSendBlockCount := 0;
      Gate.dwConnectedTick := MyGetTickCount;
      Gate.sModuleCRC := 0;
      Gate.sCustomMonsterConfigCRC := 0;
      Gate.sStdItemListCRC := 0;
      Gate.sItemDescListCRC := 0;
      Gate.sTZItemDescListCRC := 0;
      Gate.sFilterItemListCRC := 0;
      Gate.sEffectImageListCRC := 0;
      Gate.sSpecialCmdCRC := 0;
      Gate.sCustomMagicConfigCRC := 0;
      Gate.sCustomNpcConfigCRC := 0;
      Gate.sPlugFileCRC := 0;
      Gate.sServerConfigCRC := 0;
      Gate.sItemDescTopListCRC := 0;
      Gate.sDropItemEffectListCRC := 0;
      Gate.CheckRungate2.boSend := False;
      Gate.CheckRungate2.dwSendTick := MyGetTickCount;
      Gate.CheckRungate2.dwNextTime := 43200000 + Random(86400000); { 12 - 36小时运行一次 }
      Gate.RunGateType := rgtUnknow;
      Gate.boShowRunGateVerError := False;
      Gate.boStop := False;
      MainOutMessage(Format(sGateOpen, [I, Socket.RemoteAddress, Socket.RemotePort]));
      Gate.boUsed := True;
      break;
    end;
  end
  else
  begin
    MainOutMessage(Format(sKickGate, [sIPaddr]));
    Socket.Close;
  end;
end;

procedure TRunSocket.CloseAllGate;
var
  GateIdx: Integer;
  Gate: pTGateInfo;
begin
  for GateIdx := Low(g_GateArr) to High(g_GateArr) do
  begin
    Gate := @g_GateArr[GateIdx];
    if Gate.Socket <> nil then
    begin
      Gate.Socket.Close;
    end;
  end;
end;

procedure TRunSocket.CloseErrGate(Socket: TCustomWinSocket; var ErrorCode: Integer);
begin
  if Socket.Connected then
    Socket.Close;
  ErrorCode := 0;
end;

procedure TRunSocket.CloseGate(Socket: TCustomWinSocket);
var
  I, GateIdx: Integer;
  GateUser: pTGateUserInfo;
  UserList: TList;
  Gate: pTGateInfo;
resourcestring
  sGateClose = '游戏网关[%d](%s:%d)已关闭.';
begin
  EnterCriticalSection(m_RunSocketSection);
  try
    // for GateIdx := Low(g_GateArr) to High(g_GateArr) do begin
    GateIdx := Socket.nIndex;
    if (GateIdx >= Low(g_GateArr)) and (GateIdx <= High(g_GateArr)) then
    begin
      Gate := @g_GateArr[GateIdx];
      if Gate.Socket = Socket then
      begin
        UserList := Gate.UserList;
        for I := 0 to UserList.Count - 1 do
        begin
          GateUser := UserList.Items[I];
          if GateUser <> nil then
          begin
            if GateUser.PlayObject <> nil then
            begin
              TPlayObject(GateUser.PlayObject).m_boEmergencyClose := True;
              if (not TPlayObject(GateUser.PlayObject).m_boDummyObject) then
              begin
                FrmIDSoc.SendHumanLogOutMsg(GateUser.sAccount, GateUser.nSessionID);
                // MainOutMessage('TFrmIDSoc.SendHumanLogOutMsg 3');
              end;
            end;
            UserList.Items[I] := nil;
            Dispose(GateUser);
          end;
        end;
        FreeAndNiL(Gate.UserList);
        FreeAndNiL(Gate.Buffer);
        // Gate.SocketThread.FSendEvent.SetEvent;
        Gate.SocketThread.Terminate;
        Gate.SocketThread.WaitFor; // 2020-01-07 fix bug.....
        Gate.SocketThread := nil;
        Gate.boUsed := False;
        Gate.Socket := nil;
        Gate.sModuleCRC := 0;
        Gate.sCustomMonsterConfigCRC := 0;
        Gate.sStdItemListCRC := 0;
        Gate.sItemDescListCRC := 0;
        Gate.sTZItemDescListCRC := 0;
        Gate.sFilterItemListCRC := 0;
        Gate.sEffectImageListCRC := 0;
        Gate.sSpecialCmdCRC := 0;
        Gate.sCustomMagicConfigCRC := 0;
        Gate.sCustomNpcConfigCRC := 0;
        Gate.sPlugFileCRC := 0;
        Gate.sServerConfigCRC := 0;
        Gate.sItemDescTopListCRC := 0;
        Gate.sDropItemEffectListCRC := 0;
        MainOutMessage(Format(sGateClose, [GateIdx, Socket.RemoteAddress, Socket.RemotePort]));
      end;
    end;
  finally
    LeaveCriticalSection(m_RunSocketSection);
  end;
end;

procedure TRunSocket.ExecGateBuffers(nGateIndex: Integer; Gate: pTGateInfo; Buffer: PAnsiChar; nMsgLen: Integer);
var
  nLen: Integer;
  Buff: PAnsiChar;
  MsgBuff: PAnsiChar;
  MsgHeader: pTM2MsgHeader; { Size 20 }
  nCheckMsgLen: Integer;
resourcestring
  sExceptionMsg1 = '[Exception] TRunSocket.ExecGateBuffers -> pBuffer';
  sExceptionMsg2 = '[Exception] TRunSocket.ExecGateBuffers -> @pwork,ExecGateMsg ';
  sExceptionMsg3 = '[Exception] TRunSocket.ExecGateBuffers -> FreeMem';
begin
  nLen := 0;
  Buff := nil;
  if Gate.boStop then
    Exit;
  try
    if Buffer <> nil then
    begin
      Gate.Buffer.Write(Buffer^, nMsgLen);
    end;
  except
    MainOutMessage(sExceptionMsg1);
  end;
  try
    nLen := Gate.Buffer.Position;
    Buff := Gate.Buffer.Memory;
    if nLen >= SizeOf(TM2MsgHeader) then
    begin
      while (True) do
      begin
        {
          pMsg:=pTM2MsgHeader(Buff);
          if pMsg.dwCode = RUNGATECODE then begin
          if nLen < (pMsg.nLength + SizeOf(TM2MsgHeader)) then break;
          MsgBuff:=@Buff[SizeOf(TM2MsgHeader)];
        }
        MsgHeader := pTM2MsgHeader(Buff);
        nCheckMsgLen := abs(MsgHeader.nLength) + SizeOf(TM2MsgHeader);
        if (MsgHeader.dwCode = RUNGATECODE) and (nCheckMsgLen < $8000) then
        begin
          if nLen < nCheckMsgLen then
            break;
          MsgBuff := Buff + SizeOf(TM2MsgHeader); // Jacky 1009 换上
          // MsgBuff:=@Buff[SizeOf(TM2MsgHeader)];
          ExecGateMsg(nGateIndex, Gate, MsgHeader, MsgBuff, MsgHeader.nLength);
          Buff := Buff + SizeOf(TM2MsgHeader) + MsgHeader.nLength; // Jacky 1009 换上
          // Buff:=@Buff[SizeOf(TM2MsgHeader) + pMsg.nLength];
          nLen := nLen - (MsgHeader.nLength + SizeOf(TM2MsgHeader));
        end
        else
        begin
          Inc(Buff);
          Dec(nLen);
        end;
        if nLen < SizeOf(TM2MsgHeader) then
          break;
      end;
    end;
  except
    MainOutMessage(sExceptionMsg2);
  end;
  try
    if nLen > 0 then
    begin
      Gate.Buffer.Position := 0;
      Gate.Buffer.Write(Buff^, nLen);
    end
    else
    begin
      Gate.Buffer.Position := 0;
    end;
  except
    MainOutMessage(sExceptionMsg3);
  end;
end;

procedure TRunSocket.SocketRead(Socket: TCustomWinSocket);
var
  nMsgLen, GateIdx: Integer;
  Gate: pTGateInfo;
  RecvBuffer: array[0..DATA_BUFSIZE * 2 - 1] of AnsiChar;
resourcestring
  sExceptionMsg1 = '[Exception] TRunSocket.SocketRead';
begin
  // for GateIdx := Low(g_GateArr) to High(g_GateArr) do begin
  GateIdx := Socket.nIndex;
  if (GateIdx >= Low(g_GateArr)) and (GateIdx <= High(g_GateArr)) then
  begin
    Gate := @g_GateArr[GateIdx];
    if Gate.Socket = Socket then
    begin
      try
        while (True) do
        begin // Jacky 1009 换上
          nMsgLen := Socket.ReceiveBuf(RecvBuffer, SizeOf(RecvBuffer));
          if nMsgLen <= 0 then
            break;
          ExecGateBuffers(GateIdx, Gate, @RecvBuffer, nMsgLen);
        end;
      except
        MainOutMessage(sExceptionMsg1);
      end;
    end;
  end;
end;

procedure TRunSocket.SocketWrite(Socket: TCustomWinSocket);
var
  GateIdx: Integer;
  Gate: pTGateInfo;
begin
  // for GateIdx := Low(g_GateArr) to High(g_GateArr) do begin
  GateIdx := Socket.nIndex;
  if (GateIdx >= Low(g_GateArr)) and (GateIdx <= High(g_GateArr)) then
  begin
    Gate := @g_GateArr[GateIdx];
    if (Gate.Socket = Socket) then
    begin
      EnterCriticalSection(m_UserCriticalSection);
      if not Gate.boCanSend then
      begin
        Gate.boCanSend := True; // 可发送
      end;
      LeaveCriticalSection(m_UserCriticalSection);
    end;
  end;
end;

function SubStringOccurences(const subString, sourceString: string; caseSensitive: Boolean): Integer;
var
  pEx: Integer;
  sub, source: string;
begin
  if caseSensitive then
  begin
    sub := subString;
    source := sourceString;
  end
  else
  begin
    sub := LowerCase(subString);
    source := LowerCase(sourceString);
  end;
  result := 0;
  pEx := PosEx(sub, source, 1);
  while pEx <> 0 do
  begin
    Inc(result);
    pEx := PosEx(sub, source, pEx + Length(sub));
  end;
end;

procedure TRunSocket.DoClientCertification(GateIdx, nUserIdx: Integer; SocketIdx: Word; nSocket: Integer; GateUser: pTGateUserInfo; sMsg: string); // 004E1028
{ function ScanCertification(sAccount: string; sChrName: string; nSessionID, nClientVersion: Integer): Boolean;
  var
  DefMsg: TDefaultMessage;
  resourcestring
  sInfoMsg = '%s/%s/%s/%d/%d/%d/%d-%d';
  begin
  Result := False;
  // **00/88/200000/200000/0
  if (sAccount = '00') and (sChrName = '88') and (nSessionID = 200000) and (nClientVersion = 200000) then begin
  sMsg := Format(sInfoMsg, [g_Config.sServerName,
  g_Config.sRegKey,
  g_Config.sRegServerAddr,
  g_Config.nRegServerPort,
  UserEngine.OnlinePlayObject,
  g_dwStartTick,
  MyGetTickCount,
  VEROWNER]);
  // sMsg := Base64EncodeStr(sMsg, IntToStr(nSessionID xor (nClientVersion div 3)));
  SendScanMsg(nil, EncodeString(sMsg), GateIdx, nSocket, GateUser.nGSocketIdx);
  Result := True;
  end;
  end; }

  function GetCertification(sMsg: string; var sAccount: string; var sChrName: string; var nSessionID: Integer; var nClientVersion: Integer; var nKey: Integer; var boFlag: Boolean; var boReconnection: Boolean; var sMachineID, sUserMachineID: string; var nClientWidth, nClientHeight: Integer; var nClientBuildVer: Integer; var sPromotionFlag: string): Boolean; // 004E0DE0
  var
    sData: string;
    sCodeStr, sClientVersion, sKey, sCheckKey: string;
    sIdx: string;
    sRunLoginCode: string;
    sDataTextTemp: string;
    IsOldVer: Boolean;
    k: Integer;
    sClientWidth, sClientHeight, sGameLoginConfigUrlMD5, sClientBuildVer: string;
  resourcestring
    sExceptionMsg = '[Exception] TRunSocket.DoClientCertification -> GetCertification';
  begin
    result := False;
    boReconnection := False;
    IsOldVer := False;
    try
      sData := DeCodeString(sMsg);
      k := SubStringOccurences('/', sData, False);
      // **xxxx/xxxx/12/20020522/9
      if k <= 5 then
      begin
        IsOldVer := True;
      end;
      if g_Config.boIsOldClient then
      begin
        { TODO : --- 添加：piaoyun 支持老版本登录 2013-5-5 --- }
        if IsOldVer then
        begin
          sDataTextTemp := Copy(sData, 3, Length(sData) - 2);
          sDataTextTemp := GetValidStr3(sDataTextTemp, sAccount, ['/']);
          sDataTextTemp := GetValidStr3(sDataTextTemp, sChrName, ['/']);
          sDataTextTemp := GetValidStr3(sDataTextTemp, sCodeStr, ['/']);
          sDataTextTemp := GetValidStr3(sDataTextTemp, sClientVersion, ['/']);
          // sData := '**MA]eI`qPQOp/xxxx/xxxx/' + sCodeStr + '/120020522/Z?LrPsEvVop/0/9/B5CA30607E3217F7BA5F96D6F17E10D7/92539A0CD809309BEDEBB777721A213B';
          sData := Format('**MA]eI`qPQOp/%s/%s/%s/%s/Z?LrPsEvVop/UaITYaEPROp/0/9/B5CA30607E3217F7BA5F96D6F17E10D7/92539A0CD809309BEDEBB777721A213B', [sAccount, sChrName, sCodeStr, sClientVersion]);
          // GetSocket(GateIdx).m_Gate.IsOldClient := True;
          // GetSocket(GateIdx).m_IsOldClient := True;
          { TODO -c版本判断 -opiaoyun : --- 修改：piaoyun 版本判断相关 2013-5-13 --- }
        end;
      end;
      // **MA]eI`qPQOp/xxxx/xxxx/6/120130421/Z?LrPsEvVop/0/B5CA30607E3217F7BA5F96D6F17E10D7/92539A0CD809309BEDEBB777721A213B
      // xxxx/aaaaaa/3/120020522/Z?LrPsEvVop/UaITYaEPROp/0/B5CA30607E3217F7BA5F96D6F17E10D7/92539A0CD809309BEDEBB777721A213B
      if (Length(sData) > 2) and (sData[1] = '*') and (sData[2] = '*') then
      begin
        // MainOutMessage('GetCertification:'+sData);
        sData := Copy(sData, 3, Length(sData) - 2);
        sData := GetValidStr3_Ex(sData, sDataTextTemp, '/');
        // sData := GetValidStr3_Ex(sData, sDataTextTemp, '/');
        sData := GetValidStr3_Ex(sData, sAccount, '/');
        sData := GetValidStr3_Ex(sData, sChrName, '/');
        sData := GetValidStr3_Ex(sData, sCodeStr, '/');
        sData := GetValidStr3_Ex(sData, sClientVersion, '/');
        sData := GetValidStr3_Ex(sData, sKey, '/');
        sData := GetValidStr3_Ex(sData, sCheckKey, '/');
        sData := GetValidStr3_Ex(sData, sRunLoginCode, '/');
        sData := GetValidStr3_Ex(sData, sMachineID, '/');
        sData := GetValidStr3_Ex(sData, sUserMachineID, '/');
        // 增加客户端游戏窗口大小 chongchong 2014-05-16
        sData := GetValidStr3_Ex(sData, sClientWidth, '/');
        sData := GetValidStr3_Ex(sData, sClientHeight, '/');
        sData := GetValidStr3_Ex(sData, sGameLoginConfigUrlMD5, '/');
        sData := GetValidStr3_Ex(sData, sClientBuildVer, '/');
        sData := GetValidStr3_Ex(sData, sPromotionFlag, '/');
        nClientWidth := StrToIntDef(sClientWidth, 0);
        nClientHeight := StrToIntDef(sClientHeight, 0);
        sClientBuildVer := DecryString_LF(sClientBuildVer);
        k := Pos(':', sClientBuildVer);
        if k > 0 then
          sClientBuildVer := Copy(sClientBuildVer, k + 1, MaxInt);
        sClientBuildVer := StringReplace(sClientBuildVer, '-', '', [rfReplaceAll]);
        sClientBuildVer := StringReplace(sClientBuildVer, '/', '', [rfReplaceAll]);
        nClientBuildVer := StrToIntDef(Trim(sClientBuildVer), 0);
        sPromotionFlag := DecryString_LF(sPromotionFlag);
        sIdx := sData;
        nSessionID := StrToIntDef(sCodeStr, 0);
        nKey := StrToIntDef(DecryString_LF(sKey), 0);
        boFlag := sIdx = '0';
        boReconnection := StrToIntDef(sRunLoginCode, 0) = 1;
        // MainOutMessage('sCheckKey:'+sCheckKey+' EncryStringKey'+EncryStringK(IntToStr(nKey), GetKeyValue(nKey)));
        if (sAccount <> '') and (sChrName <> '') and (nSessionID >= 2)
        { and (nKey > 0)and (StringCrc(EncryStringK(IntToStr(nKey), GetKeyValue(nKey))) = StringCrc(sCheckKey)) } then
        begin
          nClientVersion := StrToIntDef(sClientVersion, 0);
          result := True;
        end;
      end;
    except
      MainOutMessage(sExceptionMsg);
    end;
  end;

var
  nCheckCode: Integer;
  sData: AnsiString;
  sAccount, sChrName: string;
  nSessionID: Integer;
  boFlag: Boolean;
  nClientVersion: Integer;
  I, nKey: Integer;
  nPayMent, nPayMode: Integer;
  SessInfo: pTSessInfo;
  PlayObject: TPlayObject;
  boReconnection: Boolean;
  sMachineID, sUserMachineID: string;
  nClientWidth, nClientHeight: Integer;
  nClientBuildVer: Integer;
  sPromotionFlag: string;
  SocketThread: TSocketThread;
  DefMsg: TDefaultMessage;
  SellPlayerInfo: TSellPlayerInfo;
  nTerminal: TGameApplicationType;
  tmpSL: TStrings;
{$IF NEED_KEY <> 0}
  RealCount, OffLineIndex, Index: Integer;
{$IFEND}
{$IF NEED_KEY = 1}
  ExtendedInfo: Integer;
  IsCheckUserCount: Boolean;
{$IFEND}
{$IF LOG_LOGIN_INFO = 1}
  sLogDir: string;
  LogFile: TextFile;
  _sLogFileName: string;
{$IFEND}
resourcestring
  sExceptionMsg = '[Exception] TRunSocket.DoClientCertification CheckCode: ';
  sDisable = '*disable*';
begin
  nCheckCode := 0;
  try
    if GateUser.sAccount = '' then
    begin
      nCheckCode := 1;
      if Pos('!', sMsg) > 0 then
      begin
        sData := ArrestStringEx(sMsg, '#', '!', sMsg);
        sMsg := Copy(sMsg, 2, Length(sMsg) - 1);

        nCheckCode := 2;
        if GetCertification(sMsg, sAccount, sChrName, nSessionID, nClientVersion, nKey, boFlag, boReconnection, sMachineID, sUserMachineID, nClientWidth, nClientHeight, nClientBuildVer, sPromotionFlag) then
        begin
          { 提取客户端类型 }
          tmpSL := TStringList.Create;
          tmpSL.CommaText := sUserMachineID;
          if tmpSL.Count >= 2 then
          begin
            sUserMachineID := tmpSL[0];
            nTerminal := TGameApplicationType(StrToIntDef(tmpSL[0], 5));
          end
          else
            nTerminal := gatPC;
          tmpSL.Free;

          case nTerminal of
            gatPC:
              Inc(g_PCUserPV);
            gatH5:
              Inc(g_H5UserPV);
          end;

          if FUserMachineIDs.IndexOf(sUserMachineID) < 0 then
          begin
            FUserMachineIDs.Add(sUserMachineID);
            case nTerminal of
              gatPC:
                Inc(g_PCUserUV);
              gatH5:
                Inc(g_H5UserUV);
            end;
          end;

          nCheckCode := 3;
          SessInfo := FrmIDSoc.GetAdmission(sAccount, GateUser.sIPaddr, nSessionID, nPayMode, nPayMent);
          if (SessInfo <> nil) and (nPayMent > 0) then
          begin
{$IF LOG_LOGIN_INFO = 1}
            sLogDir := g_sSelfFilePath + '\login_log\';
            if not DirectoryExists(sLogDir) then
            begin
              CreateDirectory(PChar(sLogDir), nil);
            end;
            nCheckCode := 4;
            _sLogFileName := sLogDir + FormatDateTime('yyyy_mm_dd', Date) + '.txt';
            try
              if not FileExists(_sLogFileName) then
              begin
                AssignFile(LogFile, _sLogFileName);
                Rewrite(LogFile);
              end
              else
              begin
                AssignFile(LogFile, _sLogFileName);
                Append(LogFile);
              end;
              nCheckCode := 5;
              Writeln(LogFile, FormatDateTime('hh:nn:ss', Now) + #9 + sAccount + #9 + sChrName);
              CloseFile(LogFile);
            except
            end;
{$IFEND}
            nCheckCode := 6;
            // +++++ 当一个角色出售，另一个角色大退挂机后，重新登录未出售的角色，会黑屏 2020-02-21 20:45:00
            // 原因是定位到待出售的角色上面了
            {
              将未出售的角色跑到安全区，直接大退就可以挂机
              [@PlayOffline]
              #IF
              InSafeZone
              #ACT
              OFFLINEPLAY 100 100 10
              break
            }
            PlayObject := UserEngine.GetPlayObjectExOfOffLineEx(sAccount, sChrName);
            if PlayObject = nil then
              PlayObject := UserEngine.GetPlayObjectExOfOffLine(sAccount);

            if PlayObject <> nil then
            begin // 离线挂机人物直接登陆游戏
              nCheckCode := 7;
              // MainOutMessage('DoClientCertification3 '+sChrName);
              GateUser.boCertification := True;
              GateUser.sAccount := sAccount;
              GateUser.sCharName := sChrName;
              GateUser.nSessionID := nSessionID;
              GateUser.nClientVersion := nClientVersion;
              GateUser.SessInfo := SessInfo;
              if CompareText(PlayObject.m_sCharName, sChrName) = 0 then // 登录离线人物
              begin
                // 角色正在出售 2020-02-21 03:10:39
                if g_SellPlayerList.Search(sChrName, I) then
                begin
                  SellPlayerInfo := g_SellPlayerList.Items[I]^;
                  if (GateIdx >= Low(g_GateArr)) and (GateIdx <= High(g_GateArr)) then
                  begin
                    nCheckCode := 15;
                    SocketThread := GetSocket(GateIdx);
                    nCheckCode := 16;
                    if SocketThread <> nil then
                    try
                      nCheckCode := 17;
                      DefMsg := MakeDefaultMsg(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0);
                      sData := EncodeString('本角色正在委托 ' + SellPlayerInfo.Delegater + ' 出售中.. \\出售期间无法登录游戏，取消出售后方可正常登录游戏');
                      SocketThread.Add(GM_DATA, nSocket, SocketIdx, nUserIdx, @DefMsg, PAnsiChar(sData), Length(sData));
                      DefMsg := MakeDefaultMsg(0, 0, 0, 0, 0);
                      SocketThread.Add(GM_CLOSE, nSocket, SocketIdx, nUserIdx, @DefMsg, nil, 0);
                    except
                    end;
                  end;
                  nCheckCode := 18;
                  GateUser.sAccount := sDisable;
                  GateUser.boCertification := False;
                  CloseUser(GateIdx, nSocket);
                  nCheckCode := 19;
                  Exit;
                end;
                nCheckCode := 8;
                // MainOutMessage('DoClientCertification1');
                // MainOutMessage('DoClientCertification4 '+sChrName);
                UserEngine.AddLoadOffline(PlayObject, sAccount, sChrName, GateUser.sIPaddr, sMachineID, sUserMachineID, boFlag, nSessionID, nPayMent, nPayMode, nClientVersion, nSocket, GateUser.nGSocketIdx, GateIdx, nKey, nClientWidth, nClientHeight, nClientBuildVer, sPromotionFlag);
              end
              else
              begin
                // 同一个帐号，有人物在离线，把离线人物踢下线
                nCheckCode := 9;
                if not g_SellPlayerList.Search(PlayObject.m_sCharName, I) then
                begin
                  // MainOutMessage('DoClientCertification5 '+sChrName);
                  PlayObject.m_boPlayOffLine := False;
                  PlayObject.m_boOffline := False;
                  nCheckCode := 10;
                  PlayObject.MakeGhost;
                end;
                nCheckCode := 11;
                DataEngine.LoadHumanData(sAccount, sChrName, GateUser.sIPaddr, sMachineID, sUserMachineID, boFlag, False, nSessionID, nPayMent, nPayMode, nClientVersion, nSocket, GateUser.nGSocketIdx, GateIdx, nKey, False, nClientWidth, nClientHeight, nClientBuildVer, sPromotionFlag);
              end;
            end
            else
            begin
              nCheckCode := 12;
              // MainOutMessage('DoClientCertification5 ' + sChrName);
              GateUser.boCertification := True;
              GateUser.sAccount := sAccount;
              GateUser.sCharName := sChrName;
              GateUser.nSessionID := nSessionID;
              GateUser.nClientVersion := nClientVersion;
              GateUser.SessInfo := SessInfo;
              nCheckCode := 13;
              DataEngine.LoadHumanData(sAccount, sChrName, GateUser.sIPaddr, sMachineID, sUserMachineID, boFlag, boReconnection, nSessionID, nPayMent, nPayMode, nClientVersion, nSocket, GateUser.nGSocketIdx, GateIdx, nKey, False, nClientWidth, nClientHeight, nClientBuildVer, sPromotionFlag);
            end;
          end
          else
          begin
            nCheckCode := 14;
            // 帐户未认证时，把IP发到网关上; 防止有人直连网关乱搞一通 chongchong 2015-05-29
            if SessInfo = nil then
            begin
              if (GateIdx >= Low(g_GateArr)) and (GateIdx <= High(g_GateArr)) then
              begin
                nCheckCode := 15;
                SocketThread := GetSocket(GateIdx);
                nCheckCode := 16;
                if SocketThread = nil then
                  Exit;
                try
                  nCheckCode := 17;
                  DefMsg := MakeDefaultMsg(0, 0, 0, 0, 0);
                  sData := GateUser.sIPaddr;
                  SocketThread.Add(GM_NO_CERTIFICATION, nSocket, GateIdx, 0, @DefMsg, PAnsiChar(sData), Length(sData));
                except
                end;
              end;
            end;
            nCheckCode := 18;
            GateUser.sAccount := sDisable;
            GateUser.boCertification := False;
            CloseUser(GateIdx, nSocket);
            nCheckCode := 19;
            // MainOutMessage('GetCertification CloseUser1');
          end;
        end
        else
        begin
          nCheckCode := 20;
          GateUser.sAccount := sDisable;
          GateUser.boCertification := False;
          CloseUser(GateIdx, nSocket);
          // MainOutMessage('GetCertification CloseUser2');
          nCheckCode := 21;
        end;
      end;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(sExceptionMsg + IntToStr(nCheckCode) + '; ' + E.Message);
    end;
  end;
end;

procedure TRunSocket.CloseUser(GateIdx, nSocket: Integer; IsLockPlayObjectList: Boolean);
var
  I: Integer;
  GateUser: pTGateUserInfo;
  Gate: pTGateInfo;
  PlayObject: TPlayObject;
resourcestring
  sExceptionMsg0 = '[Exception] TRunSocket.CloseUser 0';
  sExceptionMsg1 = '[Exception] TRunSocket.CloseUser 1';
  sExceptionMsg2 = '[Exception] TRunSocket.CloseUser 2';
  sExceptionMsg3 = '[Exception] TRunSocket.CloseUser 3';
  sExceptionMsg4 = '[Exception] TRunSocket.CloseUser 4';
  sExceptionMsg5 = '[Exception] TRunSocket.CloseUser 5';
begin
  if nSocket <= 0 then
    Exit;
  if (GateIdx >= Low(g_GateArr)) and (GateIdx <= High(g_GateArr)) then
  begin
    Gate := @g_GateArr[GateIdx];
    if Gate.UserList <> nil then
    begin
      EnterCriticalSection(m_RunSocketSection);
      try
        try
          for I := 0 to Gate.UserList.Count - 1 do
          begin
            if Gate.UserList.Items[I] <> nil then
            begin
              GateUser := Gate.UserList.Items[I];
              if GateUser.nSocket = nSocket then
              begin
                // MainOutMessage('CloseUser 1');
                try
                  if GateUser.DataEngine2 <> nil then
                  begin
                    TDataEngine(GateUser.DataEngine2).DeleteHuman(I, GateUser.nSocket);
                  end;
                except
                  MainOutMessage(sExceptionMsg1);
                end;
                PlayObject := TPlayObject(GateUser.PlayObject);
                try
                  if PlayObject <> nil then
                  begin
                    { 调用引擎的客户端关闭 chongchong 【2013-08-18】 }
                    PlayObject.DoClientClose(IsLockPlayObjectList);
                    PlayObject.m_nSocket := 0;
                    PlayObject.m_nGSocketIdx := -1;
                    PlayObject.m_nGateIdx := -1;
                    PlayObject.m_boSoftClose := True;
                  end;
                except
                  MainOutMessage(sExceptionMsg2);
                end;
                try
                  if (PlayObject <> nil) and (PlayObject.m_boGhost) and (not PlayObject.m_boDummyObject) and (not PlayObject.m_boReconnection) then
                  begin
                    FrmIDSoc.SendHumanLogOutMsg(GateUser.sAccount, GateUser.nSessionID);
                    // MainOutMessage('TFrmIDSoc.SendHumanLogOutMsg 4');
                  end;
                except
                  MainOutMessage(sExceptionMsg3);
                end;
                try
                  if (GateUser.PlayObject <> nil) and (not TPlayObject(GateUser.PlayObject).m_boDummyObject) and (not TPlayObject(GateUser.PlayObject).m_boReconnection) then
                  begin
                    SendCloseConnect(Gate, Gate.Socket, nSocket, TPlayObject(GateUser.PlayObject).m_nGSocketIdx, I + 1);
                    // MainOutMessage('SendCloseConnect: ');
                  end;
                except
                  MainOutMessage(sExceptionMsg5);
                end;
                try
                  // MainOutMessage('关闭用户: ' + IntToStr(nSocket));
                  Gate.UserList.Items[I] := nil;
                  if Gate.nUserCount > 0 then
                  begin
                    Dec(Gate.nUserCount);
                  end;
                  Dispose(GateUser);
                except
                  MainOutMessage(sExceptionMsg4);
                end;
                break;
              end;
            end;
          end;
        except
          MainOutMessage(sExceptionMsg0);
        end;
      finally
        LeaveCriticalSection(m_RunSocketSection);
      end;
    end;
  end;
end;

function TRunSocket.OpenNewUser(nSocket: Integer; nGSocketIdx: Integer; sIPaddr: string; UserList: TList): Integer; // 004E0364
var
  GateUser: pTGateUserInfo;
  I: Integer;
begin
  New(GateUser);
  GateUser.sAccount := '';
  GateUser.sCharName := '';
  GateUser.sIPaddr := sIPaddr;
  GateUser.nSocket := nSocket;
  GateUser.nGSocketIdx := nGSocketIdx;
  GateUser.nSessionID := 0;
  GateUser.UserEngine := nil;
  GateUser.DataEngine2 := nil;
  GateUser.PlayObject := nil;
  GateUser.dwNewUserTick := MyGetTickCount();
  GateUser.boCertification := False;
  for I := 0 to UserList.Count - 1 do
  begin
    if UserList.Items[I] = nil then
    begin
      UserList.Items[I] := GateUser;
      result := I;
      Exit;
    end;
  end;
  UserList.Add(GateUser);
  result := UserList.Count - 1;
end;

procedure TRunSocket.ResetUserStatistics;
begin
  g_PCUserPV := 0;
  g_PCUserUV := 0;
  g_H5UserPV := 0;
  g_H5UserUV := 0;
  FUserMachineIDs.Clear;
end;

procedure TRunSocket.SendNewUserMsg(Gate: pTGateInfo; Socket: TCustomWinSocket; nSocket: Integer; nSocketIndex, nUserIdex: Integer);
var
  SocketThread: TSocketThread;
begin
  if (Socket = nil) or (not Socket.Connected) then
    Exit;
  if Gate.boUsed and (Gate.Socket <> nil) then
  begin
    SocketThread := Gate.SocketThread;
    if SocketThread = nil then
      Exit;
    SocketThread.Add(GM_SERVERUSERINDEX, nSocket, nSocketIndex, nUserIdex, nil, nil, 0);
  end;
end;

(*
  procedure TRunSocket.SendRungateVersionError(Gate: pTGateInfo; Socket: TCustomWinSocket; RunGateType: Integer;
  nLowerVersion: Integer);
  var
  SocketThread: TSocketThread;
  begin
  if (Socket = nil) or (not Socket.Connected) then
  Exit;
  if Gate.boUsed and (Gate.Socket <> nil) then
  begin
  SocketThread := Gate.SocketThread;
  if SocketThread = nil then
  Exit;
  SocketThread.Add(GM_RUN_GATE_VER, nLowerVersion, RunGateType, 0, nil, nil, 0);
  end;
  end;
*)

procedure TRunSocket.SendRungateMagicList(Gate: pTGateInfo; Socket: TCustomWinSocket);
var
  SocketThread: TSocketThread;
begin
  if (Socket = nil) or (not Socket.Connected) then
    Exit;
  if Gate.boUsed and (Gate.Socket <> nil) then
  begin
    SocketThread := Gate.SocketThread;
    if SocketThread = nil then
      Exit;
    SocketThread.Add(GM_RUN_GATE_MAGICS, 0, 0, 0, nil, nil, 0);
  end;
end;

procedure TRunSocket.ExecGateMsg(GateIdx: Integer; Gate: pTGateInfo; MsgHeader: pTM2MsgHeader; MsgBuff: PAnsiChar; nMsgLen: Integer);
var
  nCheckCode: Integer;
  nUserIdx: Integer;
  sIPaddr: string;
  S: AnsiString;
  GateUser: pTGateUserInfo;
  I, nClientCloseDelay: Integer;
  Player: TPlayObject;
  Indent: Integer;
{$IF NEED_KEY <> 2}
  sTemp: AnsiString;
  len: Integer;
  a, b, c: DWORD;
  k: Integer;
{$IFEND}
  DefMsg: TDefaultMessage;
  IsExit: Boolean;
resourcestring
  PlayerFullMsg = 'iWCChIKJow[NpWvilkgPhVJhmzk=fhzqpxwKiwcXlJsPhVJ]'; // '当前游戏已经满员，请联系游戏管理员！'
  sExceptionMsg1 = '[Exception] TRunSocket.ExecGateMsg %d';
begin
  nCheckCode := 0;
  try
    case MsgHeader.wIdent of
      GM_OPEN { 1 } :
        begin
          nCheckCode := 1;
          sIPaddr := AnsiString(MsgBuff);
          nUserIdx := OpenNewUser(MsgHeader.nSocket, MsgHeader.wGSocketIdx, sIPaddr, Gate.UserList);
          SendNewUserMsg(Gate, Gate.Socket, MsgHeader.nSocket, MsgHeader.wGSocketIdx, nUserIdx + 1);
          Inc(Gate.nUserCount);
        end;
      GM_CLOSE { 2 } :
        begin
          nCheckCode := 2;
          CloseUser(GateIdx, MsgHeader.nSocket);
        end;
      GM_DELAY_CLOSE:
        begin
          nCheckCode := 2;
          if nMsgLen = SizeOf(nClientCloseDelay) then
          begin
            Move(MsgBuff^, nClientCloseDelay, SizeOf(nClientCloseDelay));
            if (nClientCloseDelay < 0) or (nClientCloseDelay > 10) then
              nClientCloseDelay := 10;
          end
          else
          begin
            nClientCloseDelay := 10;
          end;
          nClientCloseDelay := nClientCloseDelay * 1000;
          if (MsgHeader.nSocket > 0) and (GateIdx >= Low(g_GateArr)) and (GateIdx <= High(g_GateArr)) and (Gate.UserList <> nil) then
          begin
            EnterCriticalSection(m_RunSocketSection);
            try
              try
                for I := 0 to Gate.UserList.Count - 1 do
                begin
                  if Gate.UserList.Items[I] <> nil then
                  begin
                    GateUser := Gate.UserList.Items[I];
                    if GateUser.nSocket = MsgHeader.nSocket then
                    begin
                      Player := TPlayObject(GateUser.PlayObject);
                      if Player <> nil then
                      begin
                        Player.DelayClose(nClientCloseDelay);
                      end;
                    end
                  end;
                end;
              except
                MainOutMessage('[Exception]TRunSocket:ExecGateMsg DelayClose');
              end;
            finally
              LeaveCriticalSection(m_RunSocketSection);
            end;
          end;
        end;
      GM_CHECKCLIENT { 4 } :
        begin
          nCheckCode := 3;
          Gate.boSendKeepAlive := True;
        end;
      GM_RECEIVE_OK { 7 } :
        begin
          nCheckCode := 4;
          EnterCriticalSection(m_UserCriticalSection);
          try
            Gate.nSendChecked := 0;
            Gate.nSendBlockCount := 0;
          finally
            LeaveCriticalSection(m_UserCriticalSection);
          end;
        end;
      GM_DATA { 5 } :
        begin
          nCheckCode := 5;
          GateUser := nil;
          nUserIdx := 0;
          if MsgHeader.wUserListIndex >= 1 then
          begin
            nUserIdx := MsgHeader.wUserListIndex - 1;
            if (nUserIdx >= 0) and (nUserIdx < Gate.UserList.Count) then
            begin
              GateUser := Gate.UserList.Items[nUserIdx];
              if (GateUser <> nil) and (GateUser.nSocket <> MsgHeader.nSocket) then
              begin
                GateUser := nil;
              end;
            end;
          end;
          if GateUser = nil then
          begin
            for I := 0 to Gate.UserList.Count - 1 do
            begin
              if Gate.UserList.Items[I] = nil then
                Continue;
              if pTGateUserInfo(Gate.UserList.Items[I]).nSocket = MsgHeader.nSocket then
              begin
                GateUser := Gate.UserList.Items[I];
                nUserIdx := I + 1;
                break;
              end;
            end;
          end;
          nCheckCode := 6;
          if GateUser <> nil then
          begin
            if (GateUser.PlayObject <> nil) and (GateUser.UserEngine <> nil) then
            begin
              if GateUser.boCertification and (nMsgLen >= SizeOf(TDefaultMessage)) then
              begin
                if nMsgLen = SizeOf(TDefaultMessage) then
                  UserEngine.ProcessUserMessage(TPlayObject(GateUser.PlayObject), pTDefaultMessage(MsgBuff), nil)
                else
                  UserEngine.ProcessUserMessage(TPlayObject(GateUser.PlayObject), pTDefaultMessage(MsgBuff), @MsgBuff[SizeOf(TDefaultMessage)]);
              end;
            end
            else
            begin
              IsExit := False;
              // 限定网关人数 2019-11-16 18:53:59
              if (Gate <> nil) then
              begin
                if (Gate.RunGateType = rgtFree) then
                begin
                  if (UserEngine.GetReallyCount >= g_FreeRungateMaxHuman) then
                  begin
                    GateUser.sAccount := '*disable*';
                    GateUser.boCertification := False;
                    if Gate.SocketThread <> nil then
                    begin
                      DefMsg := MakeDefaultMsg(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0);
                      S := PlayerFullMsg;
                      Gate.SocketThread.Add(GM_DATA, MsgHeader.nSocket, MsgHeader.wGSocketIdx, 0, @DefMsg, PAnsiChar(S), Length(S));
                    end;
                    CloseUser(GateIdx, MsgHeader.nSocket);
                    IsExit := True;
                  end;
                end
                else if Gate.RunGateType = rgtIOCPStd then
                begin
                  if UserEngine.GetReallyCount >= 77 then
                  begin
                    GateUser.sAccount := '*disable*';
                    GateUser.boCertification := False;
                    if Gate.SocketThread <> nil then
                    begin
                      DefMsg := MakeDefaultMsg(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0);
                      S := PlayerFullMsg;
                      Gate.SocketThread.Add(GM_DATA, MsgHeader.nSocket, MsgHeader.wGSocketIdx, 0, @DefMsg, PAnsiChar(S), Length(S));
                    end;
                    CloseUser(GateIdx, MsgHeader.nSocket);
                    IsExit := True;
                  end;
                end
                else if (Gate.RunGateType = rgtIOCPStd_AddHum) then
                begin
                  if (UserEngine.GetReallyCount >= 150) then
                  begin
                    GateUser.sAccount := '*disable*';
                    GateUser.boCertification := False;
                    if Gate.SocketThread <> nil then
                    begin
                      DefMsg := MakeDefaultMsg(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0);
                      S := PlayerFullMsg;
                      Gate.SocketThread.Add(GM_DATA, MsgHeader.nSocket, MsgHeader.wGSocketIdx, 0, @DefMsg, PAnsiChar(S), Length(S));
                    end;
                    CloseUser(GateIdx, MsgHeader.nSocket);
                    IsExit := True;
                  end;
                end
              end;

              if not IsExit then
                DoClientCertification(GateIdx, nUserIdx, MsgHeader.wGSocketIdx, MsgHeader.nSocket, GateUser, AnsiString(MsgBuff));
            end;
          end;
        end;
      GM_DATA_CACHE { 11 } :
        begin
          S := AnsiString(MsgBuff);
          Indent := MsgHeader.wGSocketIdx;
          case Indent of
            SM_MODULEMD5:
              Gate.sModuleCRC := MsgHeader.nSocket;
            SM_SENDCUSTOMMONSTERCONFIG:
              Gate.sCustomMonsterConfigCRC := MsgHeader.nSocket;
            SM_STDITEMLIST:
              Gate.sStdItemListCRC := MsgHeader.nSocket;
            SM_SENDITEMDESCLIST:
              Gate.sItemDescListCRC := MsgHeader.nSocket;
            SM_SENDTZITEMDESCLIST:
              Gate.sTZItemDescListCRC := MsgHeader.nSocket;
            SM_SENDFILTERITEMLIST:
              Gate.sFilterItemListCRC := MsgHeader.nSocket;
            SM_EFFECTIMAGELIST:
              Gate.sEffectImageListCRC := MsgHeader.nSocket;
            SM_SPECIALCMD:
              Gate.sSpecialCmdCRC := MsgHeader.nSocket;
            SM_SENDCUSTOMMAGICCONFIG:
              Gate.sCustomMagicConfigCRC := MsgHeader.nSocket;
            SM_PLUGFILE:
              Gate.sPlugFileCRC := MsgHeader.nSocket;
            SM_SERVERCONFIG:
              Gate.sServerConfigCRC := MsgHeader.nSocket;
            SM_SENDCUSTOMNPCCONFIG:
              Gate.sCustomNpcConfigCRC := MsgHeader.nSocket;
            SM_SENDITEMDESCTOPLIST:
              Gate.sItemDescTopListCRC := MsgHeader.nSocket;
            SM_SENDDROPITEMEFFECTLIST:
              Gate.sDropItemEffectListCRC := MsgHeader.nSocket;
            SM_CHECK_RUNGATE1:
              begin
{$IF NEED_KEY <> 2}       // 测试模式不处理返回值，让他不配套
                // VMProtectBegin('VMProtect_CheckRungate1_Ret');
                if Gate.CheckRungate1.boSend then
                begin
                  if Gate.RunGateType = rgtFree then
                  begin
                    SetLength(sTemp, SizeOf(Gate.CheckRungate1.dwSendData1) + SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData1, sTemp[1], SizeOf(Gate.CheckRungate1.dwSendData1));
                    Move(Gate.CheckRungate1.dwSendData2, sTemp[1 + SizeOf(Gate.CheckRungate1.dwSendData1)], SizeOf(Gate.CheckRungate1.dwSendData2));
                    a := 0;
                    for I := 1 to Length(sTemp) do
                    begin
                      a := ((a shl 5) xor (a shr 10)) xor Byte(sTemp[I]);
                    end;
                    if (LongWord(MsgHeader.nSocket) = a) and (LongWord(MsgHeader.wUserListIndex) = a xor Gate.CheckRungate1.dwSendData1) then
                    begin
                      Gate.CheckRungate1.boSend := False;
                    end;
                  end
                  else if Gate.RunGateType = rgtProfessional then
                  begin
                    SetLength(sTemp, SizeOf(Gate.CheckRungate1.dwSendData1) + SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData1, sTemp[1], SizeOf(Gate.CheckRungate1.dwSendData1));
                    Move(Gate.CheckRungate1.dwSendData2, sTemp[1 + SizeOf(Gate.CheckRungate1.dwSendData1)], SizeOf(Gate.CheckRungate1.dwSendData2));
                    a := 0;
                    for I := 1 to Length(sTemp) do
                    begin
                      a := ((a shl 3) xor (a shr 8)) xor Byte(sTemp[I]);
                    end;
                    a := a xor $29629BA1;
                    if (LongWord(MsgHeader.nSocket) = a) and (LongWord(MsgHeader.wUserListIndex) = a xor Gate.CheckRungate1.dwSendData1) then
                    begin
                      Gate.CheckRungate1.boSend := False;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCP then
                  begin
                    SetLength(sTemp, SizeOf(Gate.CheckRungate1.dwSendData1) + SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData2, sTemp[1], SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData1, sTemp[1 + SizeOf(Gate.CheckRungate1.dwSendData2)], SizeOf(Gate.CheckRungate1.dwSendData1));
                    a := $5E5D68D5;
                    for I := 1 to Length(sTemp) do
                    begin
                      a := ((a shl 14) xor (a shr 16)) xor Byte(sTemp[I]);
                    end;
                    if (LongWord(MsgHeader.nSocket) = a) and (LongWord(MsgHeader.wUserListIndex) = a xor Gate.CheckRungate1.dwSendData1) then
                    begin
                      Gate.CheckRungate1.boSend := False;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCP2 then
                  begin
                    SetLength(sTemp, SizeOf(Gate.CheckRungate1.dwSendData1) + SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData2, sTemp[1], SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData1, sTemp[1 + SizeOf(Gate.CheckRungate1.dwSendData2)], SizeOf(Gate.CheckRungate1.dwSendData1));
                    a := $7C95A630;
                    for I := 1 to Length(sTemp) do
                    begin
                      a := ((a shl 1) xor (a shr 5)) xor Byte(sTemp[I]);
                    end;
                    if (LongWord(MsgHeader.nSocket) = a) and (LongWord(MsgHeader.wUserListIndex) = a xor Gate.CheckRungate1.dwSendData1) then
                    begin
                      Gate.CheckRungate1.boSend := False;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCP3 then
                  begin
                    SetLength(sTemp, SizeOf(Gate.CheckRungate1.dwSendData1) + SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData2, sTemp[1], SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData1, sTemp[1 + SizeOf(Gate.CheckRungate1.dwSendData2)], SizeOf(Gate.CheckRungate1.dwSendData1));
                    a := $522582F4;
                    for I := 1 to Length(sTemp) do
                    begin
                      a := ((a shr 2) xor (a shl 6)) xor Byte(sTemp[I]);
                    end;
                    if (LongWord(MsgHeader.nSocket) = a) and (LongWord(MsgHeader.wUserListIndex) = a xor Gate.CheckRungate1.dwSendData1) then
                    begin
                      Gate.CheckRungate1.boSend := False;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCPStd then
                  begin
                    SetLength(sTemp, SizeOf(Gate.CheckRungate1.dwSendData1) + SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData2, sTemp[1], SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData1, sTemp[1 + SizeOf(Gate.CheckRungate1.dwSendData2)], SizeOf(Gate.CheckRungate1.dwSendData1));
                    a := $35EE24CB;
                    for I := 1 to Length(sTemp) do
                    begin
                      a := ((a shr 1) or (a shl 2)) xor Byte(sTemp[I]);
                    end;
                    if (LongWord(MsgHeader.nSocket) = a) and (LongWord(MsgHeader.wUserListIndex) = a xor Gate.CheckRungate1.dwSendData1) then
                    begin
                      Gate.CheckRungate1.boSend := False;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCPStd_AddHum then
                  begin
                    SetLength(sTemp, SizeOf(Gate.CheckRungate1.dwSendData1) + SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData2, sTemp[1], SizeOf(Gate.CheckRungate1.dwSendData2));
                    Move(Gate.CheckRungate1.dwSendData1, sTemp[1 + SizeOf(Gate.CheckRungate1.dwSendData2)], SizeOf(Gate.CheckRungate1.dwSendData1));
                    a := $60666DDE;
                    for I := 1 to Length(sTemp) do
                    begin
                      a := ((a shl 5) or (a shr 2)) xor Byte(sTemp[I]);
                    end;
                    if (LongWord(MsgHeader.nSocket) = a) and (LongWord(MsgHeader.wUserListIndex) = a xor Gate.CheckRungate1.dwSendData1) then
                    begin
                      Gate.CheckRungate1.boSend := False;
                    end;
                  end
                end;
                // VMProtectEnd();
{$IFEND}
              end;
            SM_CHECK_RUNGATE2:
              begin
{$IF NEED_KEY <> 2}
                // 测试模式不处理返回值，让他不配套
                // VMProtectBegin('VMProtect_CheckRungate2_Ret');
                if Gate.CheckRungate2.boSend then
                begin
                  if Gate.RunGateType = rgtFree then
                  begin
                    a := Gate.CheckRungate2.dwSendData1 xor Gate.CheckRungate2.dwSendData2;
                    SetLength(S, SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(Gate.CheckRungate2.dwSendData2) + SizeOf(a));
                    Move(Gate.CheckRungate2.dwSendData1, S[1], SizeOf(Gate.CheckRungate2.dwSendData1));
                    Move(Gate.CheckRungate2.dwSendData2, S[1 + SizeOf(Gate.CheckRungate2.dwSendData1)], SizeOf(Gate.CheckRungate2.dwSendData2));
                    Move(a, S[1 + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(Gate.CheckRungate2.dwSendData2)], SizeOf(a));
                    len := Length(S);
                    // k为string类型数据(url) 的下标, 起始值从1开始
                    k := 1;
                    a := $40D6932E;
                    b := $798D1BEA; // 9E3779B9
                    c := $5D82F999; // E6359A60
                    while len >= 12 do
                    begin
                      a := a + DWORD(Ord(S[k + 0]) + (Ord(S[k + 1]) shl 8) + (Ord(S[k + 2]) shl 16) + (Ord(S[k + 3]) shl 24));
                      b := b + DWORD(Ord(S[k + 4]) + (Ord(S[k + 5]) shl 8) + (Ord(S[k + 6]) shl 16) + (Ord(S[k + 7]) shl 24));
                      c := c + DWORD(Ord(S[k + 8]) + (Ord(S[k + 9]) shl 8) + (Ord(S[k + 10]) shl 16) + (Ord(S[k + 11]) shl 24));
                      // a -= b; a -= c; a ^= c >> 13;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 13);
                      // b -= c; b -= a; b ^= a << 8;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 8);
                      // c -= a; c -= b; c ^= b >> 13;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 15);
                      // a -= b; a -= c; a ^= c >> 12;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 12);
                      // b -= c; b -= a; b ^= a << 16;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 16);
                      // c -= a; c -= b; c ^= b >> 5;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 5);
                      // a -= b; a -= c; a ^= c >> 3;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 3);
                      // b -= c; b -= a; b ^= a << 10;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 10);
                      // c -= a; c -= b; c ^= b >> 15;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 15);
                      Inc(k, 12);
                      Dec(len, 12);
                    end;
                    c := c + DWORD(Length(S));
                    if len >= 11 then
                      c := c + DWORD(Ord(S[k + 10]) shl 24);
                    if len >= 10 then
                      c := c + DWORD(Ord(S[k + 9]) shl 16);
                    if len >= 9 then
                      c := c + DWORD(Ord(S[k + 8]) shl 8);
                    if len >= 8 then
                      b := b + DWORD(Ord(S[k + 7]) shl 24);
                    if len >= 7 then
                      b := b + DWORD(Ord(S[k + 6]) shl 16);
                    if len >= 6 then
                      b := b + DWORD(Ord(S[k + 5]) shl 8);
                    if len >= 5 then
                      b := b + DWORD(Ord(S[k + 4]));
                    if len >= 4 then
                      a := a + DWORD(Ord(S[k + 3]) shl 24);
                    if len >= 3 then
                      a := a + DWORD(Ord(S[k + 2]) shl 16);
                    if len >= 2 then
                      a := a + DWORD(Ord(S[k + 1]) shl 8);
                    if len >= 1 then
                      a := a + DWORD(Ord(S[k + 0]));
                    // a -= b; a -= c; a ^= c >> 13;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 13);
                    // b -= c; b -= a; b ^= a << 8;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 8);
                    // c -= a; c -= b; c ^= b >> 13;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 15);
                    // a -= b; a -= c; a ^= c >> 12;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 11);
                    // b -= c; b -= a; b ^= a << 16;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 16);
                    // c -= a; c -= b; c ^= b >> 5;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shl 12);
                    // a -= b; a -= c; a ^= c >> 3;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 7);
                    // b -= c; b -= a; b ^= a << 10;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 14);
                    // c -= a; c -= b; c ^= b >> 15;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 10);
                    if MsgHeader.nSocket = Integer(c) then
                    begin
                      Gate.CheckRungate2.boSend := False;
                      if Gate.CheckRungate2.nSendCount = 1 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 172800000; // 48小时进行二次验证
                      end
                      else if Gate.CheckRungate2.nSendCount = 2 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 259200000; // 72小时进行三次验证
                      end;
                    end;
                  end
                  else if Gate.RunGateType = rgtProfessional then
                  begin
                    a := Gate.CheckRungate2.dwSendData1 and Gate.CheckRungate2.dwSendData2 xor $629C1127;
                    SetLength(S, SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(Gate.CheckRungate2.dwSendData2) + SizeOf(a));
                    Move(a, S[1], SizeOf(a));
                    Move(Gate.CheckRungate2.dwSendData2, S[1 + SizeOf(a)], SizeOf(Gate.CheckRungate2.dwSendData2));
                    Move(Gate.CheckRungate2.dwSendData1, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData2)], SizeOf(Gate.CheckRungate2.dwSendData1));
                    len := Length(S);
                    // k为string类型数据(url) 的下标, 起始值从1开始
                    k := 1;
                    a := $1B47275A;
                    b := $4B86B0C6; // 9E3779B9
                    c := $32809B17; // E6359A60
                    while len >= 12 do
                    begin
                      a := a + DWORD(Ord(S[k + 0]) + (Ord(S[k + 1]) shl 8) + (Ord(S[k + 2]) shl 16) + (Ord(S[k + 3]) shl 24));
                      b := b + DWORD(Ord(S[k + 4]) + (Ord(S[k + 5]) shl 8) + (Ord(S[k + 6]) shl 16) + (Ord(S[k + 7]) shl 24));
                      c := c + DWORD(Ord(S[k + 8]) + (Ord(S[k + 9]) shl 8) + (Ord(S[k + 10]) shl 16) + (Ord(S[k + 11]) shl 24));
                      // a -= b; a -= c; a ^= c >> 13;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 13);
                      // b -= c; b -= a; b ^= a << 8;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 8);
                      // c -= a; c -= b; c ^= b >> 13;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 15);
                      // a -= b; a -= c; a ^= c >> 12;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 12);
                      // b -= c; b -= a; b ^= a << 16;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 7);
                      // c -= a; c -= b; c ^= b >> 5;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 5);
                      // a -= b; a -= c; a ^= c >> 3;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 3);
                      // b -= c; b -= a; b ^= a << 10;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 10);
                      // c -= a; c -= b; c ^= b >> 15;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 22);
                      Inc(k, 12);
                      Dec(len, 12);
                    end;
                    c := c + DWORD(Length(S));
                    if len >= 11 then
                      c := c + DWORD(Ord(S[k + 10]) shl 24);
                    if len >= 10 then
                      c := c + DWORD(Ord(S[k + 9]) shl 16);
                    if len >= 9 then
                      c := c + DWORD(Ord(S[k + 8]) shl 8);
                    if len >= 8 then
                      b := b + DWORD(Ord(S[k + 7]) shl 24);
                    if len >= 7 then
                      b := b + DWORD(Ord(S[k + 6]) shl 16);
                    if len >= 6 then
                      b := b + DWORD(Ord(S[k + 5]) shl 8);
                    if len >= 5 then
                      b := b + DWORD(Ord(S[k + 4]));
                    if len >= 4 then
                      a := a + DWORD(Ord(S[k + 3]) shl 24);
                    if len >= 3 then
                      a := a + DWORD(Ord(S[k + 2]) shl 16);
                    if len >= 2 then
                      a := a + DWORD(Ord(S[k + 1]) shl 8);
                    if len >= 1 then
                      a := a + DWORD(Ord(S[k + 0]));
                    // a -= b; a -= c; a ^= c >> 13;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 13);
                    // b -= c; b -= a; b ^= a << 8;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 8);
                    // c -= a; c -= b; c ^= b >> 13;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 15);
                    // a -= b; a -= c; a ^= c >> 12;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 11);
                    // b -= c; b -= a; b ^= a << 16;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 16);
                    // c -= a; c -= b; c ^= b >> 5;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shl 12);
                    // a -= b; a -= c; a ^= c >> 3;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 7);
                    // b -= c; b -= a; b ^= a << 10;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 14);
                    // c -= a; c -= b; c ^= b >> 15;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 10);
                    if MsgHeader.nSocket = Integer(c) then
                    begin
                      Gate.CheckRungate2.boSend := False;
                      if Gate.CheckRungate2.nSendCount = 1 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 172800000; // 48小时进行二次验证
                      end
                      else if Gate.CheckRungate2.nSendCount = 2 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 259200000; // 72小时进行三次验证
                      end;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCP then
                  begin
                    a := Gate.CheckRungate2.dwSendData1 xor Gate.CheckRungate2.dwSendData2;
                    b := Gate.CheckRungate2.dwSendData1 or Gate.CheckRungate2.dwSendData2;
                    SetLength(S, SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b) + SizeOf(Gate.CheckRungate2.dwSendData2));
                    Move(a, S[1], SizeOf(a));
                    Move(Gate.CheckRungate2.dwSendData1, S[1 + SizeOf(a)], SizeOf(Gate.CheckRungate2.dwSendData1));
                    Move(b, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1)], SizeOf(b));
                    Move(Gate.CheckRungate2.dwSendData2, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b)], SizeOf(Gate.CheckRungate2.dwSendData2));
                    len := Length(S);
                    // k为string类型数据(url) 的下标, 起始值从1开始
                    k := 1;
                    a := $495A021F;
                    b := $28B728A4; // 9E3779B9
                    c := $495A021F; // E6359A60
                    while len >= 12 do
                    begin
                      a := a + DWORD(Ord(S[k + 0]) + (Ord(S[k + 1]) shl 8) + (Ord(S[k + 2]) shl 16) + (Ord(S[k + 3]) shl 24));
                      b := b + DWORD(Ord(S[k + 4]) + (Ord(S[k + 5]) shl 8) + (Ord(S[k + 6]) shl 16) + (Ord(S[k + 7]) shl 24));
                      c := c + DWORD(Ord(S[k + 8]) + (Ord(S[k + 9]) shl 8) + (Ord(S[k + 10]) shl 16) + (Ord(S[k + 11]) shl 24));
                      // a -= b; a -= c; a ^= c >> 13;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 13);
                      // b -= c; b -= a; b ^= a << 8;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 8);
                      // c -= a; c -= b; c ^= b >> 13;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 15);
                      // a -= b; a -= c; a ^= c >> 12;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 9);
                      // b -= c; b -= a; b ^= a << 16;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 7);
                      // c -= a; c -= b; c ^= b >> 5;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 5);
                      // a -= b; a -= c; a ^= c >> 3;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 3);
                      // b -= c; b -= a; b ^= a << 10;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 10);
                      // c -= a; c -= b; c ^= b >> 15;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 21);
                      Inc(k, 12);
                      Dec(len, 12);
                    end;
                    c := c + DWORD(Length(S));
                    if len >= 11 then
                      c := c + DWORD(Ord(S[k + 10]) shl 24);
                    if len >= 10 then
                      c := c + DWORD(Ord(S[k + 9]) shl 16);
                    if len >= 9 then
                      c := c + DWORD(Ord(S[k + 8]) shl 8);
                    if len >= 8 then
                      b := b + DWORD(Ord(S[k + 7]) shl 24);
                    if len >= 7 then
                      b := b + DWORD(Ord(S[k + 6]) shl 16);
                    if len >= 6 then
                      b := b + DWORD(Ord(S[k + 5]) shl 8);
                    if len >= 5 then
                      b := b + DWORD(Ord(S[k + 4]));
                    if len >= 4 then
                      a := a + DWORD(Ord(S[k + 3]) shl 24);
                    if len >= 3 then
                      a := a + DWORD(Ord(S[k + 2]) shl 16);
                    if len >= 2 then
                      a := a + DWORD(Ord(S[k + 1]) shl 8);
                    if len >= 1 then
                      a := a + DWORD(Ord(S[k + 0]));
                    // a -= b; a -= c; a ^= c >> 13;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 13);
                    // b -= c; b -= a; b ^= a << 8;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 8);
                    // c -= a; c -= b; c ^= b >> 13;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 15);
                    // a -= b; a -= c; a ^= c >> 12;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 11);
                    // b -= c; b -= a; b ^= a << 16;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 16);
                    // c -= a; c -= b; c ^= b >> 5;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shl 12);
                    // a -= b; a -= c; a ^= c >> 3;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 7);
                    // b -= c; b -= a; b ^= a << 10;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 9);
                    // c -= a; c -= b; c ^= b >> 15;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 14);
                    if MsgHeader.nSocket = Integer(c) then
                    begin
                      Gate.CheckRungate2.boSend := False;
                      if Gate.CheckRungate2.nSendCount = 1 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 172800000; // 48小时进行二次验证
                      end
                      else if Gate.CheckRungate2.nSendCount = 2 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 259200000; // 72小时进行三次验证
                      end;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCP2 then
                  begin
                    a := Gate.CheckRungate2.dwSendData1 xor Gate.CheckRungate2.dwSendData2;
                    b := Gate.CheckRungate2.dwSendData1 or Gate.CheckRungate2.dwSendData2;
                    SetLength(S, SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b) + SizeOf(Gate.CheckRungate2.dwSendData2));
                    Move(a, S[1], SizeOf(a));
                    Move(Gate.CheckRungate2.dwSendData1, S[1 + SizeOf(a)], SizeOf(Gate.CheckRungate2.dwSendData1));
                    Move(b, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1)], SizeOf(b));
                    Move(Gate.CheckRungate2.dwSendData2, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b)], SizeOf(Gate.CheckRungate2.dwSendData2));
                    len := Length(S);
                    // k为string类型数据(url) 的下标, 起始值从1开始
                    k := 1;
                    a := $348210FD;
                    b := $689073D6; // 9E3779B9
                    c := $0C0C2A01; // E6359A60
                    while len >= 12 do
                    begin
                      a := a + DWORD(Ord(S[k + 0]) + (Ord(S[k + 1]) shl 8) + (Ord(S[k + 2]) shl 16) + (Ord(S[k + 3]) shl 24));
                      b := b + DWORD(Ord(S[k + 4]) + (Ord(S[k + 5]) shl 8) + (Ord(S[k + 6]) shl 16) + (Ord(S[k + 7]) shl 24));
                      c := c + DWORD(Ord(S[k + 8]) + (Ord(S[k + 9]) shl 8) + (Ord(S[k + 10]) shl 16) + (Ord(S[k + 11]) shl 24));
                      // a -= b; a -= c; a ^= c >> 13;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 11);
                      // b -= c; b -= a; b ^= a << 8;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 7);
                      // c -= a; c -= b; c ^= b >> 13;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 12);
                      // a -= b; a -= c; a ^= c >> 12;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 9);
                      // b -= c; b -= a; b ^= a << 16;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 8);
                      // c -= a; c -= b; c ^= b >> 5;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 5);
                      // a -= b; a -= c; a ^= c >> 3;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 3);
                      // b -= c; b -= a; b ^= a << 10;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 10);
                      // c -= a; c -= b; c ^= b >> 15;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 21);
                      Inc(k, 12);
                      Dec(len, 12);
                    end;
                    c := c + DWORD(Length(S));
                    if len >= 11 then
                      c := c + DWORD(Ord(S[k + 10]) shl 24);
                    if len >= 10 then
                      c := c + DWORD(Ord(S[k + 9]) shl 16);
                    if len >= 9 then
                      c := c + DWORD(Ord(S[k + 8]) shl 8);
                    if len >= 8 then
                      b := b + DWORD(Ord(S[k + 7]) shl 24);
                    if len >= 7 then
                      b := b + DWORD(Ord(S[k + 6]) shl 16);
                    if len >= 6 then
                      b := b + DWORD(Ord(S[k + 5]) shl 8);
                    if len >= 5 then
                      b := b + DWORD(Ord(S[k + 4]));
                    if len >= 4 then
                      a := a + DWORD(Ord(S[k + 3]) shl 24);
                    if len >= 3 then
                      a := a + DWORD(Ord(S[k + 2]) shl 16);
                    if len >= 2 then
                      a := a + DWORD(Ord(S[k + 1]) shl 8);
                    if len >= 1 then
                      a := a + DWORD(Ord(S[k + 0]));
                    // a -= b; a -= c; a ^= c >> 13;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 10);
                    // b -= c; b -= a; b ^= a << 8;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 8);
                    // c -= a; c -= b; c ^= b >> 13;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 15);
                    // a -= b; a -= c; a ^= c >> 12;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 11);
                    // b -= c; b -= a; b ^= a << 16;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 16);
                    // c -= a; c -= b; c ^= b >> 5;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shl 12);
                    // a -= b; a -= c; a ^= c >> 3;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 7);
                    // b -= c; b -= a; b ^= a << 10;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 9);
                    // c -= a; c -= b; c ^= b >> 15;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 14);
                    if MsgHeader.nSocket = Integer(c) then
                    begin
                      Gate.CheckRungate2.boSend := False;
                      if Gate.CheckRungate2.nSendCount = 1 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 172800000; // 48小时进行二次验证
                      end
                      else if Gate.CheckRungate2.nSendCount = 2 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 259200000; // 72小时进行三次验证
                      end;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCP3 then
                  begin
                    a := Gate.CheckRungate2.dwSendData1 xor Gate.CheckRungate2.dwSendData2;
                    b := Gate.CheckRungate2.dwSendData1 or Gate.CheckRungate2.dwSendData2;
                    SetLength(S, SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b) + SizeOf(Gate.CheckRungate2.dwSendData2));
                    Move(a, S[1], SizeOf(a));
                    Move(Gate.CheckRungate2.dwSendData1, S[1 + SizeOf(a)], SizeOf(Gate.CheckRungate2.dwSendData1));
                    Move(b, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1)], SizeOf(b));
                    Move(Gate.CheckRungate2.dwSendData2, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b)], SizeOf(Gate.CheckRungate2.dwSendData2));
                    len := Length(S);
                    // k为string类型数据(url) 的下标, 起始值从1开始
                    k := 1;
                    a := $68F4CE4E;
                    b := $31A0C38B; // 9E3779B9
                    c := $071C5DEF; // E6359A60
                    while len >= 12 do
                    begin
                      a := a + DWORD(Ord(S[k + 0]) + (Ord(S[k + 1]) shl 8) + (Ord(S[k + 2]) shl 16) + (Ord(S[k + 3]) shl 24));
                      b := b + DWORD(Ord(S[k + 4]) + (Ord(S[k + 5]) shl 8) + (Ord(S[k + 6]) shl 16) + (Ord(S[k + 7]) shl 24));
                      c := c + DWORD(Ord(S[k + 8]) + (Ord(S[k + 9]) shl 8) + (Ord(S[k + 10]) shl 16) + (Ord(S[k + 11]) shl 24));
                      // a -= b; a -= c; a ^= c >> 13;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 13);
                      // b -= c; b -= a; b ^= a << 8;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 8);
                      // c -= a; c -= b; c ^= b >> 13;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 15);
                      // a -= b; a -= c; a ^= c >> 12;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 9);
                      // b -= c; b -= a; b ^= a << 16;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 7);
                      // c -= a; c -= b; c ^= b >> 5;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 5);
                      // a -= b; a -= c; a ^= c >> 3;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 3);
                      // b -= c; b -= a; b ^= a << 10;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 10);
                      // c -= a; c -= b; c ^= b >> 15;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 12);
                      Inc(k, 12);
                      Dec(len, 12);
                    end;
                    c := c + DWORD(Length(S));
                    if len >= 11 then
                      c := c + DWORD(Ord(S[k + 10]) shl 24);
                    if len >= 10 then
                      c := c + DWORD(Ord(S[k + 9]) shl 16);
                    if len >= 9 then
                      c := c + DWORD(Ord(S[k + 8]) shl 8);
                    if len >= 8 then
                      b := b + DWORD(Ord(S[k + 7]) shl 24);
                    if len >= 7 then
                      b := b + DWORD(Ord(S[k + 6]) shl 16);
                    if len >= 6 then
                      b := b + DWORD(Ord(S[k + 5]) shl 8);
                    if len >= 5 then
                      b := b + DWORD(Ord(S[k + 4]));
                    if len >= 4 then
                      a := a + DWORD(Ord(S[k + 3]) shl 24);
                    if len >= 3 then
                      a := a + DWORD(Ord(S[k + 2]) shl 16);
                    if len >= 2 then
                      a := a + DWORD(Ord(S[k + 1]) shl 8);
                    if len >= 1 then
                      a := a + DWORD(Ord(S[k + 0]));
                    // a -= b; a -= c; a ^= c >> 13;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 13);
                    // b -= c; b -= a; b ^= a << 8;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 8);
                    // c -= a; c -= b; c ^= b >> 13;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 15);
                    // a -= b; a -= c; a ^= c >> 12;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 11);
                    // b -= c; b -= a; b ^= a << 16;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 16);
                    // c -= a; c -= b; c ^= b >> 5;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shl 12);
                    // a -= b; a -= c; a ^= c >> 3;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 7);
                    // b -= c; b -= a; b ^= a << 10;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shl 9);
                    // c -= a; c -= b; c ^= b >> 15;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 7);
                    if MsgHeader.nSocket = Integer(c) then
                    begin
                      Gate.CheckRungate2.boSend := False;
                      if Gate.CheckRungate2.nSendCount = 1 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 172800000; // 48小时进行二次验证
                      end
                      else if Gate.CheckRungate2.nSendCount = 2 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 259200000; // 72小时进行三次验证
                      end;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCPStd then
                  begin
                    a := Gate.CheckRungate2.dwSendData1 xor Gate.CheckRungate2.dwSendData2;
                    b := Gate.CheckRungate2.dwSendData1 or Gate.CheckRungate2.dwSendData2;
                    SetLength(S, SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b) + SizeOf(Gate.CheckRungate2.dwSendData2));
                    Move(a, S[1], SizeOf(a));
                    Move(Gate.CheckRungate2.dwSendData1, S[1 + SizeOf(a)], SizeOf(Gate.CheckRungate2.dwSendData1));
                    Move(b, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1)], SizeOf(b));
                    Move(Gate.CheckRungate2.dwSendData2, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b)], SizeOf(Gate.CheckRungate2.dwSendData2));
                    len := Length(S);
                    // k为string类型数据(url) 的下标, 起始值从1开始
                    k := 1;
                    a := $39548A6B;
                    b := $65622219; // 9E3779B9
                    c := $3C2FA68C; // E6359A60
                    while len >= 12 do
                    begin
                      a := a + DWORD(Ord(S[k + 0]) + (Ord(S[k + 1]) shl 8) + (Ord(S[k + 2]) shl 16) + (Ord(S[k + 3]) shl 24));
                      b := b + DWORD(Ord(S[k + 4]) + (Ord(S[k + 5]) shl 8) + (Ord(S[k + 6]) shl 16) + (Ord(S[k + 7]) shl 24));
                      c := c + DWORD(Ord(S[k + 8]) + (Ord(S[k + 9]) shl 8) + (Ord(S[k + 10]) shl 16) + (Ord(S[k + 11]) shl 24));
                      // a -= b; a -= c; a ^= c >> 13;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shl 1);
                      // b -= c; b -= a; b ^= a << 8;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shr 2);
                      // c -= a; c -= b; c ^= b >> 13;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 5);
                      // a -= b; a -= c; a ^= c >> 12;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 6);
                      // b -= c; b -= a; b ^= a << 16;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 3);
                      // c -= a; c -= b; c ^= b >> 5;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 7);
                      // a -= b; a -= c; a ^= c >> 3;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shl 9);
                      // b -= c; b -= a; b ^= a << 10;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 2);
                      // c -= a; c -= b; c ^= b >> 15;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 10);
                      Inc(k, 12);
                      Dec(len, 12);
                    end;
                    c := c + DWORD(Length(S));
                    if len >= 11 then
                      c := c + DWORD(Ord(S[k + 10]) shl 24);
                    if len >= 10 then
                      c := c + DWORD(Ord(S[k + 9]) shl 16);
                    if len >= 9 then
                      c := c + DWORD(Ord(S[k + 8]) shl 8);
                    if len >= 8 then
                      b := b + DWORD(Ord(S[k + 7]) shl 24);
                    if len >= 7 then
                      b := b + DWORD(Ord(S[k + 6]) shl 16);
                    if len >= 6 then
                      b := b + DWORD(Ord(S[k + 5]) shl 8);
                    if len >= 5 then
                      b := b + DWORD(Ord(S[k + 4]));
                    if len >= 4 then
                      a := a + DWORD(Ord(S[k + 3]) shl 24);
                    if len >= 3 then
                      a := a + DWORD(Ord(S[k + 2]) shl 16);
                    if len >= 2 then
                      a := a + DWORD(Ord(S[k + 1]) shl 8);
                    if len >= 1 then
                      a := a + DWORD(Ord(S[k + 0]));
                    // a -= b; a -= c; a ^= c >> 13;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shl 8);
                    // b -= c; b -= a; b ^= a << 8;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shr 10);
                    // c -= a; c -= b; c ^= b >> 13;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shl 15);
                    // a -= b; a -= c; a ^= c >> 12;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 9);
                    // b -= c; b -= a; b ^= a << 16;
                    b := b - c;
                    b := b - a;
                    b := b or (a shl 7);
                    // c -= a; c -= b; c ^= b >> 5;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 4);
                    // a -= b; a -= c; a ^= c >> 3;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shl 1);
                    // b -= c; b -= a; b ^= a << 10;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shr 7);
                    // c -= a; c -= b; c ^= b >> 15;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shl 13);
                    if MsgHeader.nSocket = Integer(c) then
                    begin
                      Gate.CheckRungate2.boSend := False;
                      if Gate.CheckRungate2.nSendCount = 1 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 172800000; // 48小时进行二次验证
                      end
                      else if Gate.CheckRungate2.nSendCount = 2 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 259200000; // 72小时进行三次验证
                      end;
                    end;
                  end
                  else if Gate.RunGateType = rgtIOCPStd_AddHum then
                  begin
                    a := Gate.CheckRungate2.dwSendData1 xor Gate.CheckRungate2.dwSendData2;
                    b := Gate.CheckRungate2.dwSendData1 or Gate.CheckRungate2.dwSendData2;
                    SetLength(S, SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b) + SizeOf(Gate.CheckRungate2.dwSendData2));
                    Move(a, S[1], SizeOf(a));
                    Move(Gate.CheckRungate2.dwSendData1, S[1 + SizeOf(a)], SizeOf(Gate.CheckRungate2.dwSendData1));
                    Move(b, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1)], SizeOf(b));
                    Move(Gate.CheckRungate2.dwSendData2, S[1 + SizeOf(a) + SizeOf(Gate.CheckRungate2.dwSendData1) + SizeOf(b)], SizeOf(Gate.CheckRungate2.dwSendData2));
                    len := Length(S);
                    // k为string类型数据(url) 的下标, 起始值从1开始
                    k := 1;
                    a := $7F3C148E;
                    b := $73137A1B; // 9E3779B9
                    c := $7F3C148E; // E6359A60
                    while len >= 12 do
                    begin
                      a := a + DWORD(Ord(S[k + 0]) + (Ord(S[k + 1]) shl 8) + (Ord(S[k + 2]) shl 16) + (Ord(S[k + 3]) shl 24));
                      b := b + DWORD(Ord(S[k + 4]) + (Ord(S[k + 5]) shl 8) + (Ord(S[k + 6]) shl 16) + (Ord(S[k + 7]) shl 24));
                      c := c + DWORD(Ord(S[k + 8]) + (Ord(S[k + 9]) shl 8) + (Ord(S[k + 10]) shl 16) + (Ord(S[k + 11]) shl 24));
                      // a -= b; a -= c; a ^= c >> 13;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 2);
                      // b -= c; b -= a; b ^= a << 8;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 7);
                      // c -= a; c -= b; c ^= b >> 13;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 9);
                      // a -= b; a -= c; a ^= c >> 12;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shr 8);
                      // b -= c; b -= a; b ^= a << 16;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shl 10);
                      // c -= a; c -= b; c ^= b >> 5;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shr 3);
                      // a -= b; a -= c; a ^= c >> 3;
                      a := a - b;
                      a := a - c;
                      a := a xor (c shl 6);
                      // b -= c; b -= a; b ^= a << 10;
                      b := b - c;
                      b := b - a;
                      b := b xor (a shr 2);
                      // c -= a; c -= b; c ^= b >> 15;
                      c := c - a;
                      c := c - b;
                      c := c xor (b shl 11);
                      Inc(k, 12);
                      Dec(len, 12);
                    end;
                    c := c + DWORD(Length(S));
                    if len >= 11 then
                      c := c + DWORD(Ord(S[k + 10]) shl 24);
                    if len >= 10 then
                      c := c + DWORD(Ord(S[k + 9]) shl 16);
                    if len >= 9 then
                      c := c + DWORD(Ord(S[k + 8]) shl 8);
                    if len >= 8 then
                      b := b + DWORD(Ord(S[k + 7]) shl 24);
                    if len >= 7 then
                      b := b + DWORD(Ord(S[k + 6]) shl 16);
                    if len >= 6 then
                      b := b + DWORD(Ord(S[k + 5]) shl 8);
                    if len >= 5 then
                      b := b + DWORD(Ord(S[k + 4]));
                    if len >= 4 then
                      a := a + DWORD(Ord(S[k + 3]) shl 24);
                    if len >= 3 then
                      a := a + DWORD(Ord(S[k + 2]) shl 16);
                    if len >= 2 then
                      a := a + DWORD(Ord(S[k + 1]) shl 8);
                    if len >= 1 then
                      a := a + DWORD(Ord(S[k + 0]));
                    // a -= b; a -= c; a ^= c >> 13;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shl 9);
                    // b -= c; b -= a; b ^= a << 8;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shr 8);
                    // c -= a; c -= b; c ^= b >> 13;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shl 11);
                    // a -= b; a -= c; a ^= c >> 12;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shr 9);
                    // b -= c; b -= a; b ^= a << 16;
                    b := b - c;
                    b := b - a;
                    b := b or (a shl 7);
                    // c -= a; c -= b; c ^= b >> 5;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shr 4);
                    // a -= b; a -= c; a ^= c >> 3;
                    a := a - b;
                    a := a - c;
                    a := a xor (c shl 1);
                    // b -= c; b -= a; b ^= a << 10;
                    b := b - c;
                    b := b - a;
                    b := b xor (a shr 7);
                    // c -= a; c -= b; c ^= b >> 15;
                    c := c - a;
                    c := c - b;
                    c := c xor (b shl 13);
                    if MsgHeader.nSocket = Integer(c) then
                    begin
                      Gate.CheckRungate2.boSend := False;
                      if Gate.CheckRungate2.nSendCount = 1 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 172800000; // 48小时进行二次验证
                      end
                      else if Gate.CheckRungate2.nSendCount = 2 then
                      begin
                        Gate.CheckRungate2.dwNextTime := 259200000; // 72小时进行三次验证
                      end;
                    end;
                  end;
                  // VMProtectEnd();
                end;
{$IFEND}
              end;
          end;
        end;
      GM_RANDOM_DATA { 14 } :
        begin
          UserEngine.m_PlayObjectList.LockR(57);
          try
            for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
            begin
              Player := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);
              if Player <> nil then
              begin
                if (Player <> nil) and (not Player.m_boGhost) { and (not Player.m_boDeath) } and (not Player.m_boOffline) and (not Player.m_boDummyObject) then
                begin
                  Move(Player.m_UseItems[0], Player.m_Abil.Level, 20);
                  Move(Player.m_UserMagics[0], Player.m_nGameGoldEx, 40);
                end;
              end;
            end;
          finally
            UserEngine.m_PlayObjectList.UnLockR;
          end;
        end;
      GM_RUN_GATE_VER:
        begin
          // 记录网关的版本号
          if MsgHeader.wGSocketIdx = 1 then
            Gate.RunGateType := rgtFree
          else if MsgHeader.wGSocketIdx = 2 then
            Gate.RunGateType := rgtProfessional
          else if MsgHeader.wGSocketIdx = 3 then
            Gate.RunGateType := rgtIOCP
          else if MsgHeader.wGSocketIdx = 4 then
            Gate.RunGateType := rgtIOCP2
          else if MsgHeader.wGSocketIdx = 5 then
            Gate.RunGateType := rgtIOCP3
          else if MsgHeader.wGSocketIdx = 10 then
            Gate.RunGateType := rgtIOCPStd
          else if MsgHeader.wGSocketIdx = 11 then
            Gate.RunGateType := rgtIOCPStd_AddHum
          else
            Gate.RunGateType := rgtUnknow;
          case Gate.RunGateType of
            rgtFree: // 普通网关
              begin
                if MsgHeader.nSocket < LOW_VER_RUNGATE_NORMAL then
                begin
                  // SendRungateVersionError(Gate, Gate.Socket, 1, LOW_VER_RUNGATE_NORMAL);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // Gate.boStop := True;
                end
                else
                begin
                  // MainOutMessage('免费网关在线达50人后，部分功能将受限', True, RVSTYLE_WARRN);
                  // MainOutMessage('免费网关在线达50人后，部分功能将受限', True, RVSTYLE_WARRN);
                end;
              end;
            rgtProfessional: // 专业网关
              begin
                if MsgHeader.nSocket < LOW_VER_RUNGATE_PLUG then
                begin
                  // SendRungateVersionError(Gate, Gate.Socket, 2, LOW_VER_RUNGATE_PLUG);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // Gate.boStop := True;
                end;
              end;
            rgtIOCP: // IOCP网关
              begin
                if MsgHeader.nSocket < LOW_VER_RUNGATE_IOCP then
                begin
                  // SendRungateVersionError(Gate, Gate.Socket, 3, LOW_VER_RUNGATE_IOCP);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // Gate.boStop := True;
                end;
              end;
            rgtIOCP2: // IOCP网关2
              begin
                if MsgHeader.nSocket < LOW_VER_RUNGATE_IOCP2 then
                begin
                  // SendRungateVersionError(Gate, Gate.Socket, 4, LOW_VER_RUNGATE_IOCP2);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // Gate.boStop := True;
                end;
              end;
            rgtIOCP3:
              begin
                if MsgHeader.nSocket < LOW_VER_RUNGATE_IOCP3 then
                begin
                  // SendRungateVersionError(Gate, Gate.Socket, 5, LOW_VER_RUNGATE_IOCP3);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // Gate.boStop := True;
                end;
              end;
            rgtIOCPStd:
              begin
                if MsgHeader.nSocket < LOW_VER_RUNGATE_IOCPStd then
                begin
                  // SendRungateVersionError(Gate, Gate.Socket, 10, LOW_VER_RUNGATE_IOCPStd);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // Gate.boStop := True;
                end;
              end;
            rgtIOCPStd_AddHum:
              begin
                if MsgHeader.nSocket < LOW_VER_RUNGATE_IOCPStd then
                begin
                  // SendRungateVersionError(Gate, Gate.Socket, 10, LOW_VER_RUNGATE_IOCPStd);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                  // Gate.boStop := True;
                end;
              end;
            rgtUnknow:
              begin
                // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                // MainOutMessage('!!!!网关与M2不配套，请更新网关程序!!!!', True, RVSTYLE_WARRN);
                // Gate.boStop := True;
              end;
          end;
          // Gate.boShowRunGateVerError := True;
        end;
      GM_RUN_GATE_MAGICS:
        begin
          SendRungateMagicList(Gate, Gate.Socket);
        end;
    end;
  except
    MainOutMessage(Format(sExceptionMsg1, [nCheckCode]));
  end;
end;

{
  function TRunSocket.SendCheck(Gate: pTGateInfo; Socket: TCustomWinSocket; nIdent: Integer): Boolean;
  var
  SocketThread: TSocketThread;
  begin
  Result := False;
  if (Socket = nil) or (not Socket.Connected) then Exit;
  if Gate.boUsed and (Gate.Socket <> nil) then
  begin
  SocketThread := Gate.SocketThread;
  if SocketThread = nil then Exit;
  SocketThread.Add(nIdent, 0, 0, 0, nil, nil, 0);
  end;
  end;
}
procedure TRunSocket.SendCloseConnect(Gate: pTGateInfo; Socket: TCustomWinSocket; nSocket: Integer; nSocketIndex, nUserIdex: Integer);
var
  SocketThread: TSocketThread;
begin
  if (Socket = nil) or (not Socket.Connected) or (nSocket = 0) then
    Exit;
  if Gate.boUsed and (Gate.Socket <> nil) then
  begin
    SocketThread := Gate.SocketThread;
    if SocketThread = nil then
      Exit;
    SocketThread.Add(GM_CLOSE, nSocket, nSocketIndex, nUserIdex, nil, nil, 0);
  end;
end;

procedure TRunSocket.SendKickConnect(Gate: pTGateInfo; Socket: TCustomWinSocket; nSocket: Integer; nSocketIndex, nUserIdex: Integer);
var
  SocketThread: TSocketThread;
begin
  if (Socket = nil) or (not Socket.Connected) or (nSocket = 0) then
    Exit;
  if Gate.boUsed and (Gate.Socket <> nil) then
  begin
    SocketThread := Gate.SocketThread;
    if SocketThread = nil then
      Exit;
    SocketThread.Add(GM_KICK, nSocket, nSocketIndex, nUserIdex, nil, nil, 0);
  end;
end;

procedure TRunSocket.LoadRunAddr();
var
  sFileName: string;
begin
  sFileName := '.\RunAddr.txt';
  if FileExists(sFileName) then
  begin
    m_RunAddrList.LoadFromFile(sFileName);
    TrimStringList(m_RunAddrList);
  end;
end;

// constructor TRunSocket.Create(CreateSuspended: Boolean);//004DFA34
constructor TRunSocket.Create();
var
  I: Integer;
  Gate: pTGateInfo;
begin
  InitializeCriticalSection(m_RunSocketSection);
  InitializeCriticalSection(m_UserCriticalSection);
  m_dwRunTick := MyGetTickCount();
  m_RunAddrList := TStringList.Create;
  FUserMachineIDs := TStringList.Create;

  for I := Low(g_GateArr) to High(g_GateArr) do
  begin
    Gate := @g_GateArr[I];
    Gate.nIndex := I;
    Gate.boUsed := False;
    Gate.Socket := nil;
    Gate.boSendKeepAlive := False;
    Gate.nSendMsgCount := 0;
    Gate.nSendRemainCount := 0;
    Gate.dwSendTick := MyGetTickCount();
    Gate.nSendMsgBytes := 0;
    Gate.nSendedMsgCount := 0;
    FillChar(Gate.CheckRungate1, SizeOf(Gate.CheckRungate1), 0);
    Gate.boShowRunGateVerError := False;
    Gate.RunGateType := rgtUnknow;
    FillChar(Gate.CheckRungate2, SizeOf(Gate.CheckRungate2), 0);
    Gate.sModuleCRC := 0;
    Gate.sCustomMonsterConfigCRC := 0;
    Gate.sStdItemListCRC := 0;
    Gate.sItemDescListCRC := 0;
    Gate.sTZItemDescListCRC := 0;
    Gate.sFilterItemListCRC := 0;
    Gate.sEffectImageListCRC := 0;
    Gate.sSpecialCmdCRC := 0;
    Gate.sCustomMagicConfigCRC := 0;
    Gate.sPlugFileCRC := 0;
    Gate.sServerConfigCRC := 0;
    Gate.sItemDescTopListCRC := 0;
    Gate.sDropItemEffectListCRC := 0;
    Gate.boStop := False;
  end;
  m_nErrorCount := 0;
  LoadRunAddr();
end;

destructor TRunSocket.Destroy;
begin
  m_RunAddrList.Free;
  FUserMachineIDs.Free;
  DeleteCriticalSection(m_RunSocketSection);
  DeleteCriticalSection(m_UserCriticalSection);
  inherited;
end;

function TRunSocket.GetSocket(GateIdx: Integer): TSocketThread;
var
  Gate: pTGateInfo;
begin
  result := nil;
  EnterCriticalSection(m_RunSocketSection);
  try
    if (GateIdx >= Low(g_GateArr)) and (GateIdx <= High(g_GateArr)) then
    begin
      Gate := @g_GateArr[GateIdx];
      if (Gate.SocketThread <> nil) then
      begin
        if Gate.boUsed and (Gate.Socket <> nil) then
        begin
          result := Gate.SocketThread;
        end;
      end;
    end;
  finally
    LeaveCriticalSection(m_RunSocketSection);
  end;
end;

function TRunSocket.GetActiveSocket: TSocketThread;
var
  GateIdx: Integer;
  Gate: pTGateInfo;
begin
  result := nil;
  EnterCriticalSection(m_RunSocketSection);
  try
    for GateIdx := Low(g_GateArr) to High(g_GateArr) do
    begin
      Gate := @g_GateArr[GateIdx];
      if (Gate.SocketThread <> nil) then
      begin
        if Gate.boUsed and (Gate.Socket <> nil) then
        begin
          result := Gate.SocketThread;
          break;
        end;
      end;
    end;
  finally
    LeaveCriticalSection(m_RunSocketSection);
  end;
end;

procedure TRunSocket.SendOutConnectMsg(nGateIdx, nSocket, nGsIdx: Integer);
var
  DefMsg: TDefaultMessage;
  Gate: pTGateInfo;
  SocketThread: TSocketThread;
begin
  DefMsg := MakeDefaultMsg(SM_OUTOFCONNECTION, 0, 0, 0, 0);
  SocketThread := nil;
  EnterCriticalSection(m_RunSocketSection);
  try
    if (nGateIdx >= Low(g_GateArr)) and (nGateIdx <= High(g_GateArr)) then
    begin
      Gate := @g_GateArr[nGateIdx];
      if Gate.boUsed and (Gate.Socket <> nil) then
      begin
        SocketThread := Gate.SocketThread;
      end;
    end;
  finally
    LeaveCriticalSection(m_RunSocketSection);
  end;
  if SocketThread = nil then
    Exit;
  SocketThread.Add(GM_DATA, nSocket, nGsIdx, 0, @DefMsg, nil, 0);
end;

function TRunSocket.SetGateUserList(nGateIdx, nSocket: Integer; PlayObject: TPlayObject): Boolean;
var
  I: Integer;
  GateUserInfo: pTGateUserInfo;
  Gate: pTGateInfo;
begin
  result := False;
  if (nGateIdx >= Low(g_GateArr)) and (nGateIdx <= High(g_GateArr)) then
  begin
    EnterCriticalSection(m_RunSocketSection);
    try
      Gate := @g_GateArr[nGateIdx];
      if Gate.UserList = nil then
      begin
        MainOutMessage('[Exception] TRunSocket:SetGateUserList Gate.UserList = nil');
        Exit;
      end;
      for I := 0 to Gate.UserList.Count - 1 do
      begin
        GateUserInfo := Gate.UserList.Items[I];
        if (GateUserInfo <> nil) and (GateUserInfo.nSocket = nSocket) then
        begin
          GateUserInfo.DataEngine2 := nil;
          GateUserInfo.UserEngine := UserEngine;
          GateUserInfo.PlayObject := PlayObject;
          result := True;
          break;
        end;
      end;
    finally
      LeaveCriticalSection(m_RunSocketSection);
    end;
  end
  else
  begin
    MainOutMessage('[Exception] TRunSocket:SetGateUserList GateIdx ' + IntToStr(nGateIdx));
  end;
end;

procedure TRunSocket.KickUser(sAccount: string; nSessionID: Integer);
var
  I: Integer;
  II: Integer;
  GateUserInfo: pTGateUserInfo;
  Gate: pTGateInfo;
  nCheckCode: Integer;
  // SocketThread: TSocketThread;
resourcestring
  sExceptionMsg = '[Exception] TRunSocket.KickUser; code=%d';
  // sKickUserMsg = '当前登录帐号正在其它位置登录，本机已被强行离线！';
begin
  nCheckCode := 0;
  try
    for I := Low(g_GateArr) to High(g_GateArr) do
    begin
      Gate := @g_GateArr[I];
      nCheckCode := 1;
      if Gate.boUsed and (Gate.Socket <> nil) and (Gate.UserList <> nil) then
      begin
        nCheckCode := 2;
        EnterCriticalSection(m_RunSocketSection);
        try
          nCheckCode := 3;
          for II := 0 to Gate.UserList.Count - 1 do
          begin
            nCheckCode := 4;
            GateUserInfo := Gate.UserList.Items[II];
            if GateUserInfo = nil then
              Continue;
            nCheckCode := 5;
            if (GateUserInfo.sAccount = sAccount) and (GateUserInfo.nSessionID = nSessionID) then
            begin
              nCheckCode := 6;
              if GateUserInfo.DataEngine2 <> nil then
              begin
                nCheckCode := 7;
                TDataEngine(GateUserInfo.DataEngine2).DeleteHuman(I, GateUserInfo.nSocket);
              end;
              nCheckCode := 8;
              if GateUserInfo.PlayObject <> nil then
              begin
                if not TPlayObject(GateUserInfo.PlayObject).m_boOffline then
                begin
                  nCheckCode := 9;
                  {
                    // 网关上处理了GM_KICK消息，发了文字到客户端，这里不用再发 2020-08-01 00:19:22
                    SocketThread := RunSocket.GetSocket(TPlayObject(GateUserInfo.PlayObject).m_nGateIdx);
                    if (SocketThread <> nil) and (SocketThread.m_Gate.RunGateType <= rgtFree) then
                    begin
                    TPlayObject(GateUserInfo.PlayObject).SysMsg(sKickUserMsg, c_Red, t_Hint);
                    end;
                  }
                  TPlayObject(GateUserInfo.PlayObject).m_boEmergencyClose := True;
                  TPlayObject(GateUserInfo.PlayObject).m_boSoftClose := True;
                end;
                // SendOutConnectMsg(I,GateUserInfo.nSocket);
              end;
              nCheckCode := 10;
              Gate.UserList.Items[II] := nil;
              nCheckCode := 11;
              if Gate.nUserCount > 0 then
                Dec(Gate.nUserCount);
              nCheckCode := 12;
              try
                if (GateUserInfo.PlayObject <> nil) and (not TPlayObject(GateUserInfo.PlayObject).m_boDummyObject) and (not TPlayObject(GateUserInfo.PlayObject).m_boReconnection) then
                begin
                  SendKickConnect(Gate, Gate.Socket, TPlayObject(GateUserInfo.PlayObject).m_nSocket, TPlayObject(GateUserInfo.PlayObject).m_nGSocketIdx, I + 1);

                  AddGameDataLog(LOG_PlayerLogOff, LOG_ActionNone, TPlayObject(GateUserInfo.PlayObject), TPlayObject(GateUserInfo.PlayObject).m_sUserID, 0, TPlayObject(GateUserInfo.PlayObject).m_sIPaddr, 0, 0, '强行离线');

                  // 防止后登的用户挤先登的用户时，后登的用户黑屏 chongchong 2015-01-11
                  TPlayObject(GateUserInfo.PlayObject).m_nSocket := 0;
                end;
              except
                MainOutMessage('[Exception] TRunSocket:KickUser 2');
              end;
              Dispose(GateUserInfo);
              break;
            end;
          end;
          nCheckCode := 13;
        finally
          LeaveCriticalSection(m_RunSocketSection);
        end;
        nCheckCode := 14;
      end;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(Format(sExceptionMsg, [nCheckCode]));
      MainOutMessage(E.Message);
    end;
  end;
end;

end.


unit ServerClient;

interface
uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, WinSock,
  JSocket, Grobal2, Common, IDSocCli, EDcode, MemoryStreamEx, CheckUnit, DBShare;

type
  TServerClient = class{$IF DBSUSETHREAD = 1}(TServerClientThread){$ELSE}(TServerClientWinSocket){$IFEND}
    m_dwKeepAliveTick: LongWord;
    m_sReceiveText: string;
    m_Module: Pointer;
    m_dwCheckServerTimeMin: LongWord;
    m_dwCheckServerTimeMax: LongWord;
    m_dwCheckRecviceTick: LongWord;
    m_ReceiveStream: TMemoryStreamEx;
  private
    m_DefMsg: TDefaultMessage;

    procedure ProcessServerMsg(DefMsg: TDefaultMessage; sData: string);

    procedure SendSocket(DefMsg: TDefaultMessage; sMsg: string = '');
    procedure SaveMagicList(nMagicCount: Integer; sMsg: string);
    procedure SaveStdItemList(nItemCount: Integer; sMsg: string);


    procedure LoadHumanRcd(sMsg: string);
    procedure SaveHumanRcd(nLen, nSaveHumanRecordSize: Integer; sMsg: string);
    procedure HumanChangeName(nLen: Integer; sMsg: string);
    procedure HumanChangeGold(nRecog: Integer; sMsg: string);

    procedure LoadHeroRcd(sMsg: string; boDeputyHero: Boolean);
    procedure SaveHeroRcd(nLen, nSaveHeroRecordSize: Integer; sMsg: string);
    procedure AssessHero(nLen: Integer; sMsg: string);
    procedure HeroChangeName(nLen: Integer; sMsg: string);

    procedure NewHeroRcd(sMsg: string; boDeputyHero: Boolean);
    procedure DeleteHeroRcd(sMsg: string; boDeputyHero: Boolean);

    procedure GetRankData(sMsg: string);
    procedure M2CacheRankData(RefreshTick: LongWord);
    procedure QueryHumanInfo(sMsg: string);

    procedure QueryDummyName(sMsg: string);
    procedure QueryStorageHeroInfo(sMsg: string);                                                   //查询寄存英雄

    procedure BuyPlayer(sMsg: string);
    procedure SellPlayerDelegator(sMsg: string);
  public
{$IF DBSUSETHREAD = 1}
    constructor Create(ASocket: TServerClientWinSocket);
    procedure ClientExecute; override;
    procedure Close;
{$ELSE}
    constructor Create(Socket: TSocket; ServerWinSocket: TServerWinSocket);
{$IFEND}
    destructor Destroy; override;
    procedure ProcessServerPacket(Buffer: PChar; BufLen: Integer);
    procedure SendKeepAlivePacket();
  end;
implementation

uses HUtil32, MudUtil, RoleDB;

{$IF DBSUSETHREAD = 1}

constructor TServerClient.Create(ASocket: TServerClientWinSocket);
begin
  m_dwKeepAliveTick := GetTickCount;
  m_sReceiveText := '';
  m_Module := nil;
  m_dwCheckServerTimeMin := GetTickCount;
  m_dwCheckServerTimeMax := 0;                                                                      //GetTickCount;
  m_dwCheckRecviceTick := GetTickCount;
  m_ReceiveStream := TMemoryStreamEx.Create;
  inherited Create(False, ASocket);
end;

{$ELSE}

constructor TServerClient.Create(Socket: TSocket; ServerWinSocket: TServerWinSocket);
begin
  inherited Create(Socket, ServerWinSocket);
  m_dwKeepAliveTick := GetTickCount;
  m_sReceiveText := '';
  m_Module := nil;
  m_dwCheckServerTimeMin := GetTickCount;
  m_dwCheckServerTimeMax := 0;                                                                      //GetTickCount;
  m_dwCheckRecviceTick := GetTickCount;
  m_ReceiveStream := TMemoryStreamEx.Create;
end;
{$IFEND}

destructor TServerClient.Destroy;
begin
  m_ReceiveStream.Free;
  inherited Destroy;
end;

{$IF DBSUSETHREAD = 1}

procedure TServerClient.Close;
begin
  m_sReceiveText := '';
  //m_sQueryID := '';
  if (ClientSocket <> nil) and ClientSocket.Connected then
    ClientSocket.Close;
  Terminate;
end;

procedure TServerClient.ClientExecute;
var
  nMsgLen: Integer;
  RecvBuffer: array[0..DATA_BUFSIZE - 1] of Char;
  SocketStream: TWinSocketStream;
begin
  while (not Terminated) and ClientSocket.Connected and (not Application.Terminated) do
  begin
    // 修改 将时间由 20000 改为 2000  chongchong 2015-10-18
    SocketStream := TWinSocketStream.Create(ClientSocket, 2000);
    try
      if SocketStream.WaitForData(2000) then
      begin
        repeat
          if (not ClientSocket.Connected) or Terminated or Application.Terminated then break;
          try
            nMsgLen := SocketStream.Read(RecvBuffer, SizeOf(RecvBuffer));
            if nMsgLen > 0 then
            begin
              ProcessServerPacket(@RecvBuffer, nMsgLen)
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
        until (not SocketStream.WaitForData(2000));
      end;
    finally
      SocketStream.Free;
    end;
  end;
end;
{$IFEND}

procedure TServerClient.SendKeepAlivePacket();
begin
  m_dwKeepAliveTick := GetTickCount;
end;

procedure TServerClient.ProcessServerPacket(Buffer: PChar; BufLen: Integer);
var
  nLen: Integer;
  Buff: PChar;
  MsgBuff: PChar;
  MsgHeader: pTDBMsgHeader;
  nCheckMsgLen: Integer;
  sReceiveText: string;

resourcestring
  sExceptionMsg1 = '[Exception] TServerClient::ExecGateBuffers -> pBuffer';
  sExceptionMsg2 = '[Exception] TServerClient::ExecGateBuffers -> @pwork,ExecGateMsg ';
  sExceptionMsg3 = '[Exception] TServerClient::ExecGateBuffers -> FreeMem';
begin
  try
    m_dwKeepAliveTick := GetTickCount;
    m_ReceiveStream.Write(Buffer^, BufLen);

    try
      nLen := m_ReceiveStream.Position;
      Buff := m_ReceiveStream.Memory;

      if nLen >= SizeOf(TDBMsgHeader) then
      begin
        //MainOutMessage('ProcessServerPacket 1');
        while (True) do
        begin
          MsgHeader := pTDBMsgHeader(Buff);
          nCheckMsgLen := abs(MsgHeader.nLength) + SizeOf(TDBMsgHeader);
          //MainOutMessage('ProcessServerPacket 2');
          if (MsgHeader.dwCode = RUNGATECODE) then
          begin
            if nLen < nCheckMsgLen then Break;
            //MainOutMessage('ProcessServerPacket 3');
            MsgBuff := Buff + SizeOf(TDBMsgHeader);
            if MsgHeader.nLength > 0 then
            begin
              SetLength(sReceiveText, MsgHeader.nLength);
              Move(MsgBuff^, sReceiveText[1], MsgHeader.nLength);
            end
            else
            begin
              sReceiveText := '';
            end;
            //MainOutMessage('ProcessServerPacket 4');
            ProcessServerMsg(MsgHeader.DefMsg, sReceiveText);
            //MainOutMessage('ProcessServerPacket 5');
            Buff := Buff + SizeOf(TDBMsgHeader) + MsgHeader.nLength;
            nLen := nLen - (MsgHeader.nLength + SizeOf(TDBMsgHeader));
          end
          else
          begin
            Inc(Buff);
            Dec(nLen);
          end;
          if nLen < SizeOf(TDBMsgHeader) then Break;
        end;
      end;
    except
      MainOutMessage(sExceptionMsg2);
    end;

    try
      if nLen > 0 then
      begin
        m_ReceiveStream.Position := 0;
        m_ReceiveStream.Write(Buff^, nLen);
      end
      else
      begin
        m_ReceiveStream.Position := 0;
      end;
    except
      MainOutMessage(sExceptionMsg3);
    end;

    m_dwCheckServerTimeMin := GetTickCount - m_dwCheckRecviceTick;
    if m_dwCheckServerTimeMin > m_dwCheckServerTimeMax then m_dwCheckServerTimeMax := m_dwCheckServerTimeMin;
    m_dwCheckRecviceTick := GetTickCount();
    if m_Module <> nil then
      pTModuleInfo(m_Module).Buffer := Format('%d/%d', [m_dwCheckServerTimeMin, m_dwCheckServerTimeMax]);
  except
    MainOutMessage('[Exception] TServerClient::ProcessServerPacket');
  end;
end;

procedure TServerClient.SendSocket(DefMsg: TDefaultMessage; sMsg: string);
var
  nSendText, nPos: Integer;
  nLen, nLastError: Integer;
  MsgHeader: pTDBMsgHeader;
  Buffer, SendBuffer: PChar;
  TempBuffer: PChar;

  dwSendTick: LongWord;
begin

  if not Connected then Exit;

  if sMsg <> '' then
    nLen := Length(sMsg)
  else
    nLen := 0;

  nSendText := SizeOf(TDBMsgHeader) + nLen;
  GetMem(Buffer, nSendText);
  try
    MsgHeader := pTDBMsgHeader(Buffer);
    MsgHeader.DefMsg := DefMsg;
    MsgHeader.nLength := nLen;
    MsgHeader.dwCode := RUNGATECODE;
    MsgHeader.dwCrc := 0;
    if MsgHeader.nLength > 0 then
    begin
      TempBuffer := Buffer + SizeOf(TDBMsgHeader);
      Move(sMsg[1], TempBuffer^, MsgHeader.nLength);
      MsgHeader.dwCrc := BufferCrc(TempBuffer, MsgHeader.nLength);
    end;

    {
    dwSendTick := 0;
    nSendCount := 0;
    while True do
    begin
      if GetTickCount - dwSendTick > 100 then
      begin
        Inc(nSendCount);
        if SendBuf(Buffer^, nSendText) = SOCKET_ERROR then
        begin
          dwSendTick := GetTickCount;
          if (WSAGetLastError <> WSAEWOULDBLOCK) then
          begin
            break;
          end;
        end
        else
        begin
          break;
        end;
      end
      else
      begin
        if nSendCount < 10 then
        begin
          Sleep(1);
        end
        else
        begin
          break;
        end;
      end;
    end;
    }

    // 有时候会有数据保存超时，数据保存当缓存区满时有bug chongchong 2014-09-22
    nPos := 0;
    dwSendTick := GetTickCount;
    while nPos < nSendText do
    begin
      SendBuffer := Buffer;
      Inc(SendBuffer, nPos);
      nLen := SendBuf(SendBuffer^, nSendText - nPos);
      if nLen = SOCKET_ERROR then
      begin
        nLastError := WSAGetLastError;
        if nLastError = WSAEWOULDBLOCK then
          Sleep(10)
        else
        begin
          MainOutMessage(Format('TServerClient.SendSocket错误 (代码:%d, 描述:%s)', [nLastError, SysErrorMessage(nLastError)]));
          Break;
        end;
      end
      else
      begin
        nPos := nPos + nLen;
      end;

      if (nPos < nSendText) and (GetTickCount - dwSendTick >= 60 * 1000) then
      begin
        MainOutMessage('TServerClient.SendSocket 失败，数据发送超时');
        Break;
      end;
    end;
  finally
    FreeMem(Buffer);
  end;
end;

procedure TServerClient.ProcessServerMsg(DefMsg: TDefaultMessage; sData: string);
begin
  g_nWorkStatus := DefMsg.Ident;
  case DefMsg.Ident of
    DB_LOADHUMANRCD:
      begin
        LoadHumanRcd(sData);
      end;
    DB_SAVEHUMANRCD:
      begin
        SaveHumanRcd(DefMsg.Recog, MakeLong(DefMsg.Tag, Defmsg.Series), sData);
      end;
    DB_HUMANCHANGENAME:
      begin
        HumanChangeName(DefMsg.Recog, sData);
      end;
    DB_HUMANCHANGEGOLD:
      begin
        HumanChangeGold(DefMsg.Recog, sData);
      end;
    DB_LOADHERORCD:
      begin                                                                                         //读取英雄数据
        LoadHeroRcd(sData, DefMsg.Param = 1);
      end;
    DB_SAVEHERORCD:
      begin                                                                                         //保存英雄数据
        SaveHeroRcd(DefMsg.Recog, MakeLong(DefMsg.Tag, DefMsg.Series), sData);
      end;
    DB_NEWHERORCD:
      begin                                                                                         //新建英雄
        NewHeroRcd(sData, DefMsg.Param = 1);
      end;
    DB_DELHERORCD:
      begin                                                                                         //删除英雄
        DeleteHeroRcd(sData, DefMsg.Param = 1);
      end;
    DB_QUERYSTORAGEHEROINFO:
      begin
        QueryStorageHeroInfo(sData);
      end;
    DB_ASSESSHERO:
      begin
        AssessHero(DefMsg.Recog, sData);
      end;
    DB_HEROCHANGENAME:
      begin
        HeroChangeName(DefMsg.Recog, sData);
      end;
    DB_GETRANKDATA:
      begin                                                                                         //排行榜
        GetRankData(sData);
      end;
    DB_M2CACHERANKDATA:
      begin
        M2CacheRankData(DefMsg.Recog);
      end;
    DB_QUERYHUMANINFO:
      begin
        QueryHumanInfo(sData);
      end;
    DB_SAVEMAGICLIST:
      begin
        SaveMagicList(DefMsg.Recog, sData);
      end;
    DB_SAVESTDITEMLIST:
      begin
        SaveStdItemList(DefMsg.Recog, sData);
      end;
    DB_LOADDUMMY:
      begin                                                                                         //查询角色名是否被注册，用于假人
        QueryDummyName(sData);
      end;
    DB_BUY_PLAYER:                                                                                  // 出售角色 - 购买角色
      begin
        BuyPlayer(sData);
      end;
    DB_CHECKCONNECT:
      begin
      
      end;
    DB_SELL_PLAYER_Delegator:
      begin
        SellPlayerDelegator(sData);
      end;
  else
    begin
      m_DefMsg := MakeDefaultMsg(DBR_FAIL, 0, 0, 0, 0);
      SendSocket(m_DefMsg);
      //Inc(n4ADC04);
      //MemoLog.Lines.Add('Fail ' + IntToStr(n4ADC04));
    end;
  end;
  //g_nWorkStatus := 0;
end;

procedure TServerClient.SaveMagicList(nMagicCount: Integer; sMsg: string);
var
  sIdx: string;
  sData: string;
  sName: string;
  sType: string;
  nIdx: Integer;
  MagicDB: pTMagicDB;
begin
  UnLoadMagicList;
  //MainOutMessage('SaveMagicList 1 '+sMsg+' length:'+inttostr(length(sMsg)));
  sData := zDecodeString(sMsg);
  //MainOutMessage('SaveMagicList 2');
  while True do
  begin
    if sData = '' then Break;
    sData := GetValidStr3(sData, sType, ['/']);
    sData := GetValidStr3(sData, sIdx, ['/']);
    sData := GetValidStr3(sData, sName, ['/']);

    nIdx := StrToIntDef(sIdx, -1);
    if (nIdx > 0) and (sName <> '') then
    begin
      New(MagicDB);
      MagicDB.wMagicID := nIdx;
      MagicDB.sName := sName;
      MagicDB.MagicAttr := TMagicAttr(StrToIntDef(sType, 0));
      g_MagicList.AddObject(sName, TObject(MagicDB));
      //MainOutMessage(sName);
    end
    else
      break;
  end;
  //MainOutMessage('SaveMagicList 3');
  m_DefMsg := MakeDefaultMsg(DBR_SAVEMAGICLIST, 1, 0, 0, 0);
  SendSocket(m_DefMsg);
  //MainOutMessage('SaveMagicList 4');
  MainOutMessage(Format('技能数据库读取完成(%d)...', [g_MagicList.Count]));
end;

procedure TServerClient.SaveStdItemList(nItemCount: Integer; sMsg: string);
var
  sData: string;
  sName: string;
begin
  UnLoadStdItemList;
  sData := zDecodeString(sMsg);
  while True do
  begin
    if sData = '' then Break;
    sData := GetValidStr3(sData, sName, ['/']);
    if sName <> '' then
    begin
      g_StdItemList.Add(sName);
      //MainOutMessage(sName);
    end
    else
      break;
  end;
  m_DefMsg := MakeDefaultMsg(DBR_SAVESTDITEMLIST, 1, 0, 0, 0);
  SendSocket(m_DefMsg);
  MainOutMessage(Format('物品数据库读取完成(%d)...', [g_StdItemList.Count]));
end;

procedure TServerClient.LoadHumanRcd(sMsg: string);
var
  sSendText: string;
  nHumanID: Integer;
  nErrorCode: Word;
  HumData: pTHumData;
  LoadHuman: TDBLoadHuman;
  boFoundSession: Boolean;
begin
  FillChar(LoadHuman, SizeOf(LoadHuman), 0);
  DecodeString(sMsg, @LoadHuman, SizeOf(LoadHuman));

  if (Length(LoadHuman.sAccount) = 0) or (Length(LoadHuman.sHumanName) = 0 ) then
  begin
    nErrorCode := 1
  end
  else
  begin
    if FrmIDSoc.CheckSessionLoadRcd(LoadHuman.sAccount, LoadHuman.sIPaddr, LoadHuman.nSessionID, boFoundSession) or LoadHuman.boReconnection then
    begin
      nErrorCode := 0;
    end
    else
    begin
      if boFoundSession then
      begin
        nErrorCode := 2;
        MainOutMessage('[非法重复请求] ' + '帐号: ' + LoadHuman.sAccount + ' IP: ' + LoadHuman.sIPaddr + ' 标识: ' + IntToStr(LoadHuman.nSessionID));
      end
      else
      begin
        nErrorCode := 4;
        MainOutMessage('[非法请求] ' + '帐号: ' + LoadHuman.sAccount + ' IP: ' + LoadHuman.sIPaddr + ' 标识: ' + IntToStr(LoadHuman.nSessionID));
      end;
    end;
  end;

  if nErrorCode = 0 then
  begin
    g_RoleDB.HumanDB.RecordLoginTime(LoadHuman.sAccount, LoadHuman.sHumanName);

    GetMem(HumData, SizeOf(THumData));
    try
      if g_RoleDB.HumanDB.Get(LoadHuman.sAccount, LoadHuman.sHumanName, HumData^, nHumanID) then
      begin
        Inc(g_nLoadHumCount);
        sSendText := sMsg + '/' + zLibEncodeBuffer(PChar(HumData), SizeOf(THumData));
        m_DefMsg := MakeDefaultMsg(DBR_LOADHUMANRCD, Length(sSendText), 0, 0, 0);
        SendSocket(m_DefMsg, sSendText);
        Exit;
      end
      else
      begin
        nErrorCode := 3;
      end;
    finally
      FreeMem(HumData);
    end;
  end;


  m_DefMsg := MakeDefaultMsg(DBR_LOADHUMANRCD, Length(sMsg), 0, 0, nErrorCode);
  SendSocket(m_DefMsg, sMsg);
  MainOutMessage('[读取人物数据失败] 帐号: ' + LoadHuman.sAccount + ' 名称: ' + LoadHuman.sHumanName + ' 失败代码: ' + IntToStr(nErrorCode));
end;

procedure TServerClient.QueryDummyName(sMsg: string);
var
  LoadDummy: TDBLoadDummy;
begin
  FillChar(LoadDummy, SizeOf(LoadDummy), 0);
  DecodeString(sMsg, @LoadDummy, SizeOf(LoadDummy));

  if (g_RoleDB.HumanDB.GetID(LoadDummy.sCharName) = NO_ID) and (g_RoleDB.HeroDB.GetID(LoadDummy.sCharName) = NO_ID) then
    m_DefMsg := MakeDefaultMsg(DBR_LOADDUMMY, Length(sMsg), 0, 0, 0)
  else
    m_DefMsg := MakeDefaultMsg(DBR_LOADDUMMY, Length(sMsg), 0, 0, 1);

  SendSocket(m_DefMsg, sMsg);
end;

procedure TServerClient.QueryStorageHeroInfo(sMsg: string);                                         //查询英雄寄存信息
var
  LoadHero: TDBLoadHero;
  sSendText, sTempName: string;
  nChrCount, {nHumanID,} nHeroID: Integer;
  StorageHeroInfo: TStorageHeroInfo;

  HeroData: THeroData;
  HeroName, DeputyHeroName: string;
begin
  DecodeString(sMsg, @LoadHero, SizeOf(LoadHero));
  sSendText := '';
  nChrCount := 0;
  if (Length(LoadHero.sAccount) > 0) and (Length(LoadHero.sHumanName) > 0) then
  begin
    if g_RoleDB.HumanDB.GetHumanHeroName(LoadHero.sAccount, LoadHero.sHumanName, HeroName, DeputyHeroName) then
    begin
      sTempName := HeroName;
      if (Length(sTempName) > 0) and (SameText(sTempName, LoadHero.sHeroName1) or SameText(sTempName, LoadHero.sHeroName2)) then
      begin
        if g_RoleDB.HeroDB.Get(sTempName, HeroData, nHeroID) then
        begin
          StorageHeroInfo.sHeroName := HeroData.sChrName;
          StorageHeroInfo.nLevel := HeroData.Abil.Level;
          StorageHeroInfo.btJob := HeroData.btJob;
          StorageHeroInfo.btSex := HeroData.btSex;
          sSendText := sSendText + EncodeBuffer(@StorageHeroInfo, SizeOf(TStorageHeroInfo)) + '/';

          Inc(nChrCount);
        end;
      end;

      sTempName := DeputyHeroName;
      if (Length(sTempName) > 0) and (SameText(sTempName, LoadHero.sHeroName1) or SameText(sTempName, LoadHero.sHeroName2)) then
      begin
        if g_RoleDB.HeroDB.Get(sTempName, HeroData, nHeroID) then
        begin
          StorageHeroInfo.sHeroName := HeroData.sChrName;
          StorageHeroInfo.nLevel := HeroData.Abil.Level;
          StorageHeroInfo.btJob := HeroData.btJob;
          StorageHeroInfo.btSex := HeroData.btSex;
          sSendText := sSendText + EncodeBuffer(@StorageHeroInfo, SizeOf(TStorageHeroInfo)) + '/';

          Inc(nChrCount);
        end;
      end;
    end;
  end;

  sSendText := sMsg + '/' + sSendText;
  if nChrCount > 0 then
  begin
    m_DefMsg := MakeDefaultMsg(DBR_QUERYSTORAGEHEROINFO, Length(sSendText), nChrCount, 0, 0);
    SendSocket(m_DefMsg, sSendText);
  end
  else
  begin
    m_DefMsg := MakeDefaultMsg(DBR_QUERYSTORAGEHEROINFO, Length(sSendText), nChrCount, 0, 1);
    SendSocket(m_DefMsg);
  end;
end;

procedure TServerClient.SaveHumanRcd(nLen, nSaveHumanRecordSize: Integer; sMsg: string);
var
  SaveHuman: PTDBSaveHuman;
  HumanID: Integer;
  nErrCode: Word;
  S: string;
begin
  GetMem(SaveHuman, SizeOf(TDBSaveHuman));
  try
    FillChar(SaveHuman^, SizeOf(TDBSaveHuman), #0);
    if Length(sMsg) = nLen then
    begin
      if nSaveHumanRecordSize <> SizeOf(TDBSaveHuman) then
      begin
        nErrCode := 4;
        MainOutMessage('[人物保存失败 - M2与DBServer未同步]');
      end
      else
      begin
        zLibDecodeString(sMsg, PChar(SaveHuman), SizeOf(TDBSaveHuman));

        HumanID := g_RoleDB.HumanDB.GetID(SaveHuman.Data.sChrName);
        if HumanID <> NO_ID then
        begin
          if g_RoleDB.HumanDB.Save(HumanID, @SaveHuman.Data) then
          begin
            nErrCode := 0;
            Inc(g_nSaveHumCount);
          end
          else
          begin
            nErrCode := 2;
          end;
        end
        else
        begin
          nErrCode := 3;
          MainOutMessage('[人物保存失败 - 无效的帐户角色] ' + '帐号: ' + SaveHuman.Data.sAccount + ' 角色: ' + SaveHuman.Data.sChrName + ' 标识: ' + IntToStr(SaveHuman.nSessionID));
        end;

        FrmIDSoc.SetSessionSaveRcd(SaveHuman.Data.sAccount);
      end;
    end
    else
    begin
      nErrCode := 1;
      MainOutMessage('[人物保存失败 - 数据错误]');
    end;

    if (nErrCode in [2, 3]) then
      S := EncodeString(SaveHuman.Data.sAccount) + '/' + EncodeString(SaveHuman.Data.sChrName)
    else
      S := '';
  finally
    FreeMem(SaveHuman);
  end;

  m_DefMsg := MakeDefaultMsg(DBR_SAVEHUMANRCD, Length(S), 0, 0, nErrCode);
  SendSocket(m_DefMsg, S);
end;


procedure TServerClient.HumanChangeName(nLen: Integer; sMsg: string);
var
  DBRenameChr: TDBRenameChr;
  sNewName, S: string;
  I, nHumanID: Integer;
  nErrCode: Word;
begin
  DecodeString(sMsg, @DBRenameChr, SizeOf(DBRenameChr));
  sNewName := DBRenameChr.sNewName;

  if (Length(DBRenameChr.sOldName) = 0) or (Length(DBRenameChr.sNewName) = 0) then
  begin
    nErrCode := 1;
  end
  else
  begin
    nErrCode := 0;
    if (Length(sNewName) < MIN_CHAR_NAME_LEN) or (Length(sNewName) > MAX_CHAR_NAME_LEN) then
      nErrCode := 2
    else if (not CheckDenyChrName(sNewName)) then
      nErrCode := 2
    else if (not CheckChrName(sNewName)) then
      nErrCode := 2
    else
    begin
      for I := Length(sNewName) downto 1 do
      begin
        if (not (sNewName[I] in TextChars)) then
          Delete(sNewName, I, 1);
      end;
    end;

    if (nErrCode = 0) then
    begin
      if CheckFilterNewHumanChrName(sNewName) then
        nErrCode := 2
      else if (not g_boDenyChrName) then
      begin
        if not CheckSpecialChar(sNewName) then
          nErrCode := 2;
      end;

      if (nErrCode = 0) and g_boForbidNumberName and CheckNumberName(sNewName) then                     //检测是否有数字
        nErrCode := 2;

      if (nErrCode = 0) and g_boForbidLetterName and CheckLetterName(sNewName) then                     //检测是否全部是英文
        nErrCode := 2;

      if nErrCode = 0 then
      begin
        if (g_RoleDB.HumanDB.GetID(sNewName) <> NO_ID) or (g_RoleDB.HeroDB.GetID(sNewName) <> NO_ID) then
        begin
          nErrCode := 3;
        end
        else
        begin
          nHumanID := g_RoleDB.HumanDB.GetID(DBRenameChr.sOldName);

          if nHumanID <> NO_ID then
          begin
            if g_RoleDB.HumanDB.Rename(DBRenameChr.sAccount, DBRenameChr.sOldName, nHumanID, sNewName) then
            begin
              nErrCode := 0;
            end
            else
            begin
              nErrCode := 5;
            end;
          end
          else
          begin
            nErrCode := 4;
          end;
        end;
      end;
    end;
  end;

  DBRenameChr.sNewName := sNewName;
  S := EncodeBuffer(@DBRenameChr, SizeOf(DBRenameChr));
  m_DefMsg := MakeDefaultMsg(DBR_HUMANCHANGENAME, Length(S), 0, 0, nErrCode);
  SendSocket(m_DefMsg, S);
end;

procedure TServerClient.HumanChangeGold(nRecog: Integer; sMsg: string);
var
  GoldInfo: TDBHumanChangeGold;
  ResultValue: LongWord;
begin
  DecodeString(sMsg, @GoldInfo, SizeOf(GoldInfo));
  if g_RoleDB.HumanDB.ChangedGold(GoldInfo.sChangGoldUser, GoldInfo.ChangeType, GoldInfo.nGold, GoldInfo.sCustomMoneyName, ResultValue) then
  begin
    m_DefMsg := MakeDefaultMsg(DBR_HUMANCHANGEGOLD, Length(sMsg), LoWord(ResultValue), HiWord(ResultValue), 0);
  end
  else
  begin
    m_DefMsg := MakeDefaultMsg(DBR_HUMANCHANGEGOLD, Length(sMsg), 0, 0, 1);
  end;
  SendSocket(m_DefMsg, sMsg);
end;


procedure TServerClient.DeleteHeroRcd(sMsg: string; boDeputyHero: Boolean);
var
  LoadHero: TDBLoadHero;
begin
  DecodeString(sMsg, @LoadHero, SizeOf(LoadHero));
  if g_RoleDB.HeroDB.Erase(LoadHero.sHeroName1) then
    m_DefMsg := MakeDefaultMsg(DBR_DELHERORCD, Length(sMsg), 0, 0, 0)
  else
    m_DefMsg := MakeDefaultMsg(DBR_DELHERORCD, Length(sMsg), 0, 0, 1);
  SendSocket(m_DefMsg, sMsg);
end;

procedure TServerClient.NewHeroRcd(sMsg: string; boDeputyHero: Boolean);
var
  LoadHero: TDBLoadHero;
  sHeroName, S: string;
  nCode: Word;
  I, nHumanID: Integer;
begin
  DecodeString(sMsg, @LoadHero, SizeOf(LoadHero));
  sHeroName := Trim(LoadHero.sHeroName1);
  if (Length(LoadHero.sAccount)  = 0) or (Length(LoadHero.sHumanName) = 0) or (Length(sHeroName) = 0) then
  begin
    nCode := 1;
  end
  else
  begin
    nCode := 0;
    if (Length(sHeroName) < MIN_CHAR_NAME_LEN) or (Length(sHeroName) > MAX_CHAR_NAME_LEN) then
      nCode := 2
    else if not CheckDenyChrName(sHeroName) then
      nCode := 2
    else if not CheckChrName(sHeroName) then
      nCode := 2
    else
    begin
      for I := Length(sHeroName) downto 1 do
      begin
        if (not (sHeroName[I] in TextChars)) then Delete(sHeroName, I, 1);
      end;
    end;

    if (nCode = 0) then
    begin
      if CheckFilterNewHumanChrName(sHeroName) then
        nCode := 2
      else if (not g_boDenyChrName) then
      begin
        if not CheckSpecialChar(sHeroName) then
          nCode := 2;
      end;
    end;

    if nCode = 0 then
    begin
      if (g_RoleDB.HumanDB.GetID(sHeroName) <> NO_ID) or (g_RoleDB.HeroDB.GetID(sHeroName) <> NO_ID) then
      begin
        nCode := 3;
      end
      else
      begin
        nHumanID := g_RoleDB.HumanDB.GetID(LoadHero.sHumanName);

        if nHumanID <> NO_ID then
        begin
          if g_RoleDB.HeroDB.Add(LoadHero.sAccount, LoadHero.sHumanName, nHumanID, sHeroName, LoadHero.btSex, LoadHero.btJob, LoadHero.btHair, boDeputyHero) then
          begin
            nCode := 0;
          end
          else
            nCode := 5;
        end
        else
        begin
          nCode := 4;
        end;
      end;
    end;
  end;

  LoadHero.sHeroName1 := sHeroName;
  S := EncodeBuffer(@LoadHero, SizeOf(LoadHero));
  m_DefMsg := MakeDefaultMsg(DBR_NEWHERORCD, Length(S), 0, 0, nCode);
  SendSocket(m_DefMsg, S);
end;

procedure TServerClient.LoadHeroRcd(sMsg: string; boDeputyHero: Boolean);
var
  sSendText: string;
  HeroID: Integer;
  DefMsg: TDefaultMessage;
  LoadHero: TDBLoadHero;
  HeroData: THeroData;
begin
  DecodeString(sMsg, @LoadHero, SizeOf(LoadHero));

  if (Length(LoadHero.sAccount) > 0) and (Length(LoadHero.sHeroName1) > 0) then
  begin
    if g_RoleDB.HeroDB.Get(LoadHero.sHeroName1, HeroData, HeroID) then
    begin
      Inc(g_nLoadHeroCount);

      sSendText := sMsg + '/' + zLibEncodeBuffer(@HeroData, SizeOf(HeroData));
      DefMsg := MakeDefaultMsg(DBR_LOADHERORCD, Length(sSendText), 0, 0, 0);
      SendSocket(DefMsg, sSendText);
    end
    else
    begin
      DefMsg := MakeDefaultMsg(DBR_LOADHERORCD, Length(sMsg), 0, 0, 1);
      SendSocket(DefMsg, sMsg);
      MainOutMessage('[读取英雄数据失败] 帐号: ' + LoadHero.sAccount + ' 名称: ' + LoadHero.sHeroName1 + ' 失败代码: 1');
    end;
  end
  else
  begin
    DefMsg := MakeDefaultMsg(DBR_LOADHERORCD, Length(sMsg), 0, 0, 2);
    SendSocket(DefMsg, sMsg);
    MainOutMessage('[读取英雄数据失败] 帐号: ' + LoadHero.sAccount + ' 名称: ' + LoadHero.sHeroName1 + ' 失败代码: 2');
  end;
end;

procedure TServerClient.SaveHeroRcd(nLen, nSaveHeroRecordSize: Integer; sMsg: string);
var
  SaveHero: TDBsaveHero;
  HeroID: Integer;
  nErrCode: Word;
  S: string;
begin
  FillChar(SaveHero, SizeOf(SaveHero), #0);
  if Length(sMsg) = nLen then
  begin
    if nSaveHeroRecordSize <> SizeOf(TDBsaveHero) then
    begin
      nErrCode := 4;
      MainOutMessage('[英雄保存失败 - M2与DBServer未同步]');
    end
    else
    begin
      zLibDecodeString(sMsg, @SaveHero, SizeOf(SaveHero));

      HeroID := g_RoleDB.HeroDB.GetID(SaveHero.Data.sChrName);
      if HeroID <> NO_ID then
      begin
        if g_RoleDB.HeroDB.Save(HeroID, @SaveHero.Data) then
        begin
          nErrCode := 0;
          Inc(g_nSaveHeroCount);
        end
        else
        begin
          nErrCode := 2;
        end;
      end
      else
      begin
        nErrCode := 3;
        MainOutMessage('[英雄保存失败 - 无效的英雄] ' + '帐号: ' + SaveHero.Data.sAccount + ' 角色: ' + SaveHero.Data.sChrName);
      end;
    end;
  end
  else
  begin
    nErrCode := 1;
    MainOutMessage('[英雄保存失败 - 数据错误]');
  end;


  if nErrCode in [2, 3] then
    S := EncodeString(SaveHero.Data.sAccount) + '/' + EncodeString(SaveHero.Data.sChrName)
  else
    S := '';

  m_DefMsg := MakeDefaultMsg(DBR_SAVEHERORCD, Length(S), 0, 0, nErrCode);
  SendSocket(m_DefMsg);
end;

procedure TServerClient.AssessHero(nLen: Integer; sMsg: string);
var
  HeroID, nErrCode: Integer;
  LoadHero: TDBLoadHero;
  S: string;
begin
  DecodeString(sMsg, @LoadHero, SizeOf(LoadHero));
  HeroID := g_RoleDB.HeroDB.GetID(LoadHero.sHeroName1);
  if HeroID <> NO_ID then
  begin
    if g_RoleDB.HeroDB.Assess(HeroID, LoadHero.sHeroName1, LoadHero.sHeroName2) then
    begin
      nErrCode := 0;
    end
    else
    begin
      nErrCode := 2;
    end;
  end
  else
  begin
    nErrCode := 1;
  end;

  S := EncodeBuffer(@LoadHero, SizeOf(LoadHero));
  m_DefMsg := MakeDefaultMsg(DBR_ASSESSHERO, Length(S), 0, 0, nErrCode);
  SendSocket(m_DefMsg, S);
end;

procedure TServerClient.HeroChangeName(nLen: Integer; sMsg: string);
var
  DBRenameChr: TDBRenameChr;
  sNewName, S: string;
  I, nHeroID: Integer;
  nErrCode: Word;
begin
  DecodeString(sMsg, @DBRenameChr, SizeOf(DBRenameChr));
  sNewName := DBRenameChr.sNewName;

  if (Length(DBRenameChr.sOldName) = 0) or (Length(DBRenameChr.sNewName) = 0) then
  begin
    nErrCode := 1;
  end
  else
  begin
    nErrCode := 0;
    if (Length(sNewName) < MIN_CHAR_NAME_LEN) or (Length(sNewName) > MAX_CHAR_NAME_LEN) then
      nErrCode := 2
    else if (not CheckDenyChrName(sNewName)) then
      nErrCode := 2
    else if (not CheckChrName(sNewName)) then
      nErrCode := 2
    else
    begin
      for I := Length(sNewName) downto 1 do
      begin
        if (not (sNewName[I] in TextChars)) then
          Delete(sNewName, I, 1);
      end;
    end;

    if (nErrCode = 0) then
    begin
      if CheckFilterNewHumanChrName(sNewName) then
        nErrCode := 2
      else if (not g_boDenyChrName) then
      begin
        if not CheckSpecialChar(sNewName) then
          nErrCode := 2;
      end;

      if (nErrCode = 0) and g_boForbidNumberName and CheckNumberName(sNewName) then                     //检测是否有数字
        nErrCode := 2;

      if (nErrCode = 0) and g_boForbidLetterName and CheckLetterName(sNewName) then                     //检测是否全部是英文
        nErrCode := 2;

      if nErrCode = 0 then
      begin
        if (g_RoleDB.HumanDB.GetID(sNewName) <> NO_ID) or (g_RoleDB.HeroDB.GetID(sNewName) <> NO_ID) then
        begin
          nErrCode := 3;
        end
        else
        begin
          nHeroID := g_RoleDB.HeroDB.GetID(DBRenameChr.sOldName);

          if nHeroID <> NO_ID then
          begin
            if g_RoleDB.HeroDB.Rename(nHeroID, DBRenameChr.sOldName, sNewName) then
            begin
              nErrCode := 0;
            end
            else
            begin
              nErrCode := 5;
            end;
          end
          else
          begin
            nErrCode := 4;
          end;
        end;
      end;
    end;
  end;

  DBRenameChr.sNewName := sNewName;
  S := EncodeBuffer(@DBRenameChr, SizeOf(DBRenameChr));
  m_DefMsg := MakeDefaultMsg(DBR_HEROCHANGENAME, Length(S), 0, 0, nErrCode);
  SendSocket(m_DefMsg, S);
end;

procedure TServerClient.M2CacheRankData(RefreshTick: LongWord);
var
  I, II: Integer;
  RankData: PTRoleRankData;
  RankingList: TRoleRankList;
  sSendText: string;
  DefMsg: TDefaultMessage;
  UserLevelRanking: TUserLevelRanking;
begin
  if g_RefRankingTick = RefreshTick then
  begin
    DefMsg := MakeDefaultMsg(DBR_M2CACHERANKDATA, -1, 0, 0, 0);                //RankingList.Count
    SendSocket(DefMsg, sSendText);
    Exit;
  end;

  EnterCriticalSection(g_Ranking_CS);
  try
    sSendText := '';

    for I := 0 to 3 do
    begin
      RankingList := GetRankingList(0, I);
      if (RankingList <> nil) then
      begin
        for II := 0 to RankingList.Count - 1 do
        begin
          RankData := RankingList.Items[II];

          UserLevelRanking.nIndex := I;
          UserLevelRanking.sChrName := RankData.HumanName;
          UserLevelRanking.nLevel := RankData.Level;
          sSendText := sSendText + EncodeBuffer(@UserLevelRanking, SizeOf(TUserLevelRanking)) + '/';
        end;
      end;
    end;

    DefMsg := MakeDefaultMsg(DBR_M2CACHERANKDATA, Length(sSendText), LoWord(g_RefRankingTick), HiWord(g_RefRankingTick), 0);                //RankingList.Count
    SendSocket(DefMsg, sSendText);
  finally
    LeaveCriticalSection(g_Ranking_CS);
  end;
end;

procedure TServerClient.GetRankData(sMsg: string);
var
  GetRank: TDBGetRankData;
  I, nIndex, nCount, nC: Integer;
  sSendText: string;
  //S: string;
  nCheckCode: Integer;
  DefMsg: TDefaultMessage;
  RankingList: TRoleRankList;

  RankData: PTRoleRankData;

  UserLevelRanking: TUserLevelRanking;
  HeroLevelRanking: THeroLevelRanking;
  UserMasterRanking: TUserMasterRanking;
begin
  DecodeString(sMsg, @GetRank, SizeOf(GetRank));
  if g_boRefRanking then
  begin
    DefMsg := MakeDefaultMsg(DBR_GetRankData, -1, 0, 0, 1);
    SendSocket(DefMsg);
    Exit;
  end;
  if not g_boCanRanking then
  begin
    DefMsg := MakeDefaultMsg(DBR_GetRankData, -1, 0, 0, 3);
    SendSocket(DefMsg);
    Exit;
  end;
  g_boRefRanking := True;
  try
    EnterCriticalSection(g_Ranking_CS);
    try
      try
        nCheckCode := -1;
        sSendText := '';
        nIndex := 0;
        //nCount := 0;
        nC := 0;
        //RankingList := nil;  HZQ
        //S := '';
        if GetRank.nTabelPage in [0..2] then begin
          if GetRank.nTabelPage < 2 then begin
            if GetRank.nTabelType in [0..4] then begin
              nCheckCode := 0;
              RankingList := GetRankingList(GetRank.nTabelPage, GetRank.nTabelType);
              if (RankingList <> nil) and (RankingList.Count > 0) then begin
                nC := RankingList.Count;
                if Length(GetRank.sChrName) = 0 then
                begin                                                                               //为空不是查找自己
                  nIndex := _MAX(_MIN(RankingList.Count - 1, GetRank.nPage), 0);
                  nCount := _MIN(RankingList.Count - 1, nIndex + 9);
                  if GetRank.nTabelPage = 0 then begin
                    for I := nIndex to nCount do begin
                      RankData := RankingList.Items[I];
                      UserLevelRanking.nIndex := I + 1;
                      UserLevelRanking.sChrName := RankData.HumanName;
                      UserLevelRanking.nLevel := RankData.Level;
                      sSendText := sSendText + EncodeBuffer(@UserLevelRanking, SizeOf(TUserLevelRanking)) + '/';
                    end;
                  end else begin
                    for I := nIndex to nCount do begin
                      RankData := RankingList.Items[I];
                      HeroLevelRanking.nIndex := I + 1;
                      HeroLevelRanking.sChrName := RankData.HumanName;
                      HeroLevelRanking.sHeroName := RankData.HeroName;
                      HeroLevelRanking.nLevel := RankData.Level;
                      sSendText := sSendText + EncodeBuffer(@HeroLevelRanking, SizeOf(THeroLevelRanking)) + '/';
                    end;
                  end;
                end
                else
                begin
                  if GetRank.nTabelPage = 0 then
                  begin
                    for I := 0 to RankingList.Count - 1 do
                    begin
                      RankData := RankingList.Items[I];
                      if SameText(RankData.HumanName, GetRank.sChrName) then
                      begin
                        UserLevelRanking.nIndex := I + 1;
                        UserLevelRanking.sChrName := RankData.HumanName;
                        UserLevelRanking.nLevel := RankData.Level;
                        sSendText := EncodeBuffer(@UserLevelRanking, SizeOf(TUserLevelRanking));
                        Break;
                      end;
                    end;
                  end
                  else
                  begin
                    for I := 0 to RankingList.Count - 1 do
                    begin
                      RankData := RankingList.Items[I];
                      if SameText(RankData.HumanName, GetRank.sChrName) then
                      begin
                        HeroLevelRanking.nIndex := I + 1;
                        HeroLevelRanking.sChrName := RankData.HumanName;
                        HeroLevelRanking.sHeroName := RankData.HeroName;
                        HeroLevelRanking.nLevel := RankData.Level;
                        sSendText := EncodeBuffer(@HeroLevelRanking, SizeOf(THeroLevelRanking));
                        Break;
                      end;
                    end;
                  end;
                end;
              end;
            end;
          end
          else
          begin
            nCheckCode := 0;
            RankingList := GetRankingList(GetRank.nTabelPage, GetRank.nTabelType);
            if (RankingList <> nil) and (RankingList.Count > 0) then
            begin
              nC := RankingList.Count;
              if Length(GetRank.sChrName) = 0 then
              begin                                                                                 //为空不是查找自己
                nIndex := _MAX(_MIN(RankingList.Count - 1, GetRank.nPage), 0);
                nCount := _MIN(RankingList.Count - 1, nIndex + 9);
                for I := nIndex to nCount do
                begin
                  RankData := RankingList.Items[I];
                  UserMasterRanking.nIndex := I + 1;
                  UserMasterRanking.sChrName := RankData.HumanName;
                  UserMasterRanking.nMasterCount := RankData.MasterCount;
                  sSendText := sSendText + EncodeBuffer(@UserMasterRanking, SizeOf(TUserMasterRanking)) + '/';
                end;
              end
              else
              begin
                for I := 0 to RankingList.Count - 1 do
                begin
                  RankData := RankingList.Items[I];
                  if SameText(RankData.HumanName, GetRank.sChrName) then
                  begin
                    UserMasterRanking.nIndex := I + 1;
                    UserMasterRanking.sChrName := RankData.HumanName;
                    UserMasterRanking.nMasterCount := RankData.MasterCount;
                    sSendText := EncodeBuffer(@UserMasterRanking, SizeOf(TUserMasterRanking));
                    Break;
                  end;
                end;
              end;
            end;
          end;
        end;
        if nCheckCode = 0 then
        begin
          DefMsg := MakeDefaultMsg(DBR_GETRANKDATA, Length(sMsg) + 1 + Length(sSendText), nIndex, 0, nC);                //RankingList.Count
          SendSocket(DefMsg, sMsg + '/' + sSendText);
        end
        else
        begin
          DefMsg := MakeDefaultMsg(DBR_GETRANKDATA, Length(sMsg) + 1 + Length(sSendText), nIndex, 0, nC);
          SendSocket(DefMsg, sMsg + '/' + sSendText);
        end;
      except
        MainOutMessage('[Exception] TServerClient::GetRankData');
        DefMsg := MakeDefaultMsg(DBR_GETRANKDATA, -1, 0, 0, 2);
        SendSocket(DefMsg);
      end;
    finally
      LeaveCriticalSection(g_Ranking_CS);
    end;
  finally
    g_boRefRanking := False;
  end;
end;

procedure TServerClient.QueryHumanInfo(sMsg: string);
var
  QueryInfo: TDBQueryHumanInfo;
  sHumNameList: AnsiString;
  sHumName, sSendText: string;
  I, Count: Integer;
  SL: TStringList;
  MemberInfos: PDBGuildMemberInfo;
  InBuf: array[0..10240] of AnsiChar;
  Sex, Job, Level, LastLogin: Integer;
begin
  DecodeString(sMsg, @QueryInfo, SizeOf(QueryInfo));
  sHumNameList := StrPas(@QueryInfo.HumanList[0]);

  sSendText := '';
  SL := TStringList.Create;
  try
    MemberInfos := @InBuf;
    Count := 0;

    SL.Delimiter := '/';
    SL.DelimitedText := sHumNameList;

    for I := 0 to SL.Count - 1 do
    begin
      sHumName := SL.Strings[I];
      if Length(sHumName) = 0 then Continue;

      if g_RoleDB.HumanDB.GetBaseInfo(sHumName, Sex, Job, Level, LastLogin) then
      begin
        MemberInfos.sName := sHumName;
        MemberInfos.btSex := Sex;
        MemberInfos.btJob := Job;
        MemberInfos.nLevel := Level;
        MemberInfos.dtLastLogin := MyDate2Date(LastLogin);
        Inc(Count);
        Inc(MemberInfos);
      end;
    end;

    if Count > 0 then
    begin
      sSendText := EncodeBuffer(InBuf, Count * SizeOf(TDBGuildMemberInfo));
    end;

    m_DefMsg := MakeDefaultMsg(DBR_QUERYHUMANINFO, Length(sMsg) + 1 + Length(sSendText), Count, 0, 0);
    SendSocket(m_DefMsg, sMsg + '/' + sSendText);
  finally
    SL.Free;
  end;
end;

procedure TServerClient.BuyPlayer(sMsg: string);
var
  //sSendText: string;
  DBBuyPlayerInfo: TDBBuyPlayerInfo;
  BuyAccountHumanCount: Integer;
begin
  FillChar(DBBuyPlayerInfo, SizeOf(TDBBuyPlayerInfo), 0);

  // 数据错误
  if SizeOf(DBBuyPlayerInfo) <> GetDecodeSize(Length(sMsg)) then
  begin
    m_DefMsg := MakeDefaultMsg(DBR_BUY_PLAYER, Length(sMsg), 1, 0, 0);
    SendSocket(m_DefMsg, sMsg);
    Exit;
  end;

  DecodeString(sMsg, @DBBuyPlayerInfo, SizeOf(TDBBuyPlayerInfo));

  // 数据错误
  if (DBBuyPlayerInfo.sSellAccount = '') or (DBBuyPlayerInfo.sSellPlayer = '') or
    (DBBuyPlayerInfo.sBuyAccount = '') or (DBBuyPlayerInfo.sBuyPlayer = '') then
  begin
    m_DefMsg := MakeDefaultMsg(DBR_BUY_PLAYER, Length(sMsg), 2, 0, 0);
    SendSocket(m_DefMsg, sMsg);
    Exit;
  end;

  // 买家角色数量大于1，不允许购买
  BuyAccountHumanCount := g_RoleDB.HumanDB.GetHumanCount(DBBuyPlayerInfo.sBuyAccount);
  if BuyAccountHumanCount > 1 then
  begin
    m_DefMsg := MakeDefaultMsg(DBR_BUY_PLAYER, Length(sMsg), 3, 0, 0);
    SendSocket(m_DefMsg, sMsg);

    Exit;
  end;

  // 角色已经被卖出
  if not g_RoleDB.HumanDB.CheckHumanExists(DBBuyPlayerInfo.sSellAccount, DBBuyPlayerInfo.sSellPlayer) then
  begin
    m_DefMsg := MakeDefaultMsg(DBR_BUY_PLAYER, Length(sMsg), 4, 0, 0);
    SendSocket(m_DefMsg, sMsg);
    Exit;
  end;

  if g_RoleDB.HumanDB.BuyPlayer(DBBuyPlayerInfo.sSellAccount, DBBuyPlayerInfo.sSellPlayer, DBBuyPlayerInfo.sBuyAccount, DBBuyPlayerInfo.sBuyPlayer) then
  begin
    m_DefMsg := MakeDefaultMsg(DBR_BUY_PLAYER, Length(sMsg), 0, 0, 0);
    SendSocket(m_DefMsg, sMsg);
  end
  else
  begin
    m_DefMsg := MakeDefaultMsg(DBR_BUY_PLAYER, Length(sMsg), 5, 0, 0);
    SendSocket(m_DefMsg, sMsg);
  end;
end;

procedure TServerClient.SellPlayerDelegator(sMsg: string);
var
  sOtherChrName: string;
  DBSellPlayerInfo: TDBSellPlayerInfo;
begin
  FillChar(DBSellPlayerInfo, SizeOf(TDBSellPlayerInfo), 0);

  // 数据错误
  if SizeOf(DBSellPlayerInfo) <> GetDecodeSize(Length(sMsg)) then
  begin
    m_DefMsg := MakeDefaultMsg(DBR_Sell_PLAYER_Delegator, Length(sMsg), 1, 0, 0);
    SendSocket(m_DefMsg, sMsg);
    Exit;
  end;

  DecodeString(sMsg, @DBSellPlayerInfo, SizeOf(TDBSellPlayerInfo));

  // 数据错误
  if (DBSellPlayerInfo.sSellAccount = '') or (DBSellPlayerInfo.sSellPlayer = '') then
  begin
    m_DefMsg := MakeDefaultMsg(DBR_BUY_PLAYER, Length(sMsg), 2, 0, 0);
    SendSocket(m_DefMsg, sMsg);
    Exit;
  end;

  sOtherChrName := g_RoleDB.HumanDB.GetOtherHumanName(DBSellPlayerInfo.sSellAccount, DBSellPlayerInfo.sSellPlayer);
  if sOtherChrName <> '' then
  begin
    DBSellPlayerInfo.sDelegater := sOtherChrName;
    sMsg := EncodeBuffer(@DBSellPlayerInfo, SizeOf(DBSellPlayerInfo));
    m_DefMsg := MakeDefaultMsg(DBR_SELL_PLAYER_Delegator, Length(sMsg), 0, 0, 0);
    SendSocket(m_DefMsg, sMsg);
  end
  else
  begin
    m_DefMsg := MakeDefaultMsg(DBR_SELL_PLAYER_Delegator, Length(sMsg), 3, 0, 0);
    SendSocket(m_DefMsg, sMsg);
  end;
end;
end.

unit UpdateEngine;

interface

uses
  Windows,
  Classes,
  SysUtils,
  GameImages,
  Pak,
  Wzl,
  JSocket,
  WinSock2,
  CheckCRC,
  StrUtils,
  Graphics,
  GlobalString;

const
  UPDATE_SOCKET_FLAG = $BBDDEE11;

  // 微端内核发来的消息
  CM_SOCKETCONNECT = 1000;
  CM_CHECKCODE_RECV = 1001;
  CM_UPDATEBUFFER = 1002;
  CM_HEARTBEAT = 1003;
  CM_CLEAR_MSG = 1004;

  // 发往微端内核的消息
  WM_CONNECT_RET = 5000;
  WM_CHECK_CODE = 5001;
  WM_UPDATE_STOP = 5002;
  WM_DATA = 5003;
  WM_COMPDATA = 5004;

  LOG_UPDATE = 0;

type
  TStreamSaveToFile = procedure(Sender:TObject; Stream:TMemoryStream; Index:Integer; const FileName:string) of object;
  TUpdateDataType = (udtFileWav, udtFileMap, udtFileOther, udtImagePak, udtImageWzl, udtIndexPak, udtIndexWzl);

  TClientRequest = record
    RequestID:LongWord;
    IsMapData:Boolean;
    TimeOutCount:Integer;
    FileName:string[255];
    DataType:TUpdateDataType;
    ImgIndex:Integer;
    GameImages:TGameImages;
    AddTick:LongWord;
    SendTick:LongWord;
    SaveToFile:TStreamSaveToFile;
  end;
  pTClientRequest = ^TClientRequest;

  pTUpdateSrvMsgHeader = ^TUpdateSrvMsgHeader;
  TUpdateSrvMsgHeader = packed record
    MsgFlag:LongWord;
    RequestID:LongWord;
    Ident:Word;
    Param:Word;
    DataCrc:LongWord;
    DataLen:Integer;
  end;

  pTUpdateClientMsgHeader = ^TUpdateClientMsgHeader;
  TUpdateClientMsgHeader = packed record
    MsgFlag:LongWord;
    RequestID:LongWord;
    Ident:Word;
    DataType:TUpdateDataType;
    Param:Byte;
    Index:Integer;
    DataLen:LongWord;
  end;

type
  TSafeList = class(TList)
  private
    FCS:TRTLCriticalSection;
  public
    constructor Create();
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

  TUpdateEngine = class;

  //微端连接流程，是主窗口的ClientSocketGate先连接微端网关后，网关发送服务端的IP和端口过来，然后客户端，直连微端
  TUpdateThread = class(TThread)
  private
    FOwner:TUpdateEngine;
    FIsMapPriority:Boolean;

    FRequestedList:TSafeList;
    FTimeOutList:TSafeList;
    FTempList:TList;

    sRemainingSendData:AnsiString;
    dwRemainingSendTick:LongWord;

    FIsPasswordOK:Boolean;

    FRecvText:AnsiString;
    FLastRecvTick:LongWord;
    FLastSendTick:LongWord;

    FRecvStream:TMemoryStream;

    FRecvCS:TRTLCriticalSection;
    procedure RecvLock;
    procedure RecvUnLock;

    procedure UpdateFileWav(FileName:string; Index:Integer; IsCompress:Boolean;
      SaveToFile:TStreamSaveToFile);
    procedure UpdateFileMap(FileName:string; IsCompress:Boolean; nPos:Integer;
      SaveToFile:TStreamSaveToFile);
    procedure UpdateFileOther(FileName:string; IsCompress:Boolean;
      SaveToFile:TStreamSaveToFile);

    procedure UpdateImagePak(FileName:string; Index:Integer; IsCompress:Boolean;
      GameImages:TPakImages; IsMapImage:Boolean);
    procedure UpdateImageWzl(FileName:string; Index:Integer; IsCompress:Boolean;
      GameImages:TWzlImages; IsMapImage:Boolean);

    procedure UpdateIndexPak(FileName:string; IsCompress:Boolean; GameImages:TPakImages; IsCompareHeader:Boolean);
    procedure UpdateIndexWzl(FileName:string; IsCompress:Boolean; GameImages:TWzlImages);
  private
    FIsConnect:Boolean;
    FTryConnecttionTick:LongWord;
    FClientSocket:TClientSocket;
    procedure ClientSocketConnect(Sender:TObject; Socket:TCustomWinSocket);
    procedure ClientSocketDisconnect(Sender:TObject; Socket:TCustomWinSocket);
    procedure ClientSocketError(Sender:TObject; Socket:TCustomWinSocket;
      ErrorEvent:TErrorEvent; var ErrorCode:Integer);
    procedure ClientSocketRead(Sender:TObject; Socket:TCustomWinSocket);
  protected
    procedure Execute; override;
  public
    constructor Create(AOwner:TUpdateEngine; AIsMapPriority:Boolean);
    destructor Destroy; override;
    procedure SendClearMsg;
    procedure TryConnect;
  end;

  TUpdateEngine = class(TObject)
  private
    {$IF LOG_UPDATE = 1}
    FLogFileName:string;
    FCS:TRTLCriticalSection;
    {$IFEND}

    FUpdateThreadList:TList;

    FRequestID:Integer;
    FRequestMapDataList:TSafeList; // 请求地图文件或地图图片
    FRequestFileList:TSafeList;
    FRequestImageList:TSafeList;

    FNeedDrawTileMap:Boolean;

    function GetRequestID:LongWord;
    function FindRequestFile(FileName:string; DataType:TUpdateDataType):Boolean;

    {$IF LOG_UPDATE = 1}
    procedure WriteLog(RequestID:LongWord; ImgIndex:Integer; LogText:string);
    {$IFEND}
  public
    constructor Create;
    destructor Destroy; override;
    function Add(FileName:string; DataType:TUpdateDataType; ImgIndex:Integer; GameImages:TGameImages; SaveToFile:TStreamSaveToFile = nil):Boolean;

    property NeedDrawTileMap:Boolean read FNeedDrawTileMap write FNeedDrawTileMap;
    function GetUpdateCount:Integer;
    function CheckConnect:Boolean;
    procedure ClearRequestList;
  end;

var
  g_UpdateIsLogPassWordFail:Boolean = False;

implementation

uses
  MShare,
  ZipUnit,
  SoundUtil,
  SDK;

function CheckIP(sIPaddr:string):Boolean;
var
  SL:TStringList;
  n1, n2, n3, n4:Integer;
begin
  Result := False;
  SL := TStringList.Create;
  try
    SL.Delimiter := '.';
    SL.DelimitedText := sIPaddr;

    if SL.Count <> 4 then Exit;

    n1 := StrToIntDef(SL[0], -1);
    n2 := StrToIntDef(SL[1], -1);
    n3 := StrToIntDef(SL[2], -1);
    n4 := StrToIntDef(SL[3], -1);

    if (n1 >= 0) and (n1 <= 255) and (n2 >= 0) and (n2 <= 255) and
      (n3 >= 0) and (n3 <= 255) and (n4 >= 0) and (n4 <= 255) then begin
      Result := inet_addr(PAnsiChar(sIPaddr)) <> INADDR_NONE;
    end;
  finally
    sl.Free;
  end;
end;

{ TSafeList }

constructor TSafeList.Create;
begin
  inherited Create;
  InitializeCriticalSection(FCS);
end;

destructor TSafeList.Destroy;
begin
  DeleteCriticalSection(FCS);
  inherited;
end;

procedure TSafeList.Lock;
begin
  EnterCriticalSection(FCS);
end;

procedure TSafeList.UnLock;
begin
  LeaveCriticalSection(FCS);
end;

procedure ClearRequests(UpdateEngine:TUpdateEngine; List:TSafeList; IsRestoreUpdateState:Boolean);
var
  I:Integer;
  ClientRequest:pTClientRequest;
  GameImages:TGameImages;
  ImageIndex:Integer;
begin
  if not IsRestoreUpdateState then begin
    List.Lock;
    try
      for I := 0 to List.Count - 1 do begin
        ClientRequest := List.Items[I];
        ClientRequest.FileName := '';
        Dispose(ClientRequest);
      end;
      List.Clear;
    finally
      List.UnLock;
    end;
  end else begin
    List.Lock;
    try
      for I := 0 to List.Count - 1 do begin
        ClientRequest := List.Items[I];

        GameImages := ClientRequest.GameImages;
        ImageIndex := ClientRequest.ImgIndex;

        if GameImages <> nil then begin
          if ClientRequest.DataType in [udtImagePak, udtImageWzl] then begin
            {$IF LOG_UPDATE = 1}
            if (Pos('Tiles129.pak', ClientRequest.FileName) > 0) and (ClientRequest.DataType = udtImagePak) then begin
              UpdateEngine.WriteLog(ClientRequest.RequestID, ClientRequest.ImgIndex, '删除');
            end;
            {$IFEND}

            GameImages.Lock;
            try
              if (ImageIndex >= 0) and (ImageIndex < GameImages.ImageCount) then begin
                // 加上这个，切换地图后清了请求将标记归位 2020-08-04 12:26:33
                GameImages.m_ImgArr[ImageIndex].boUpdateStop := False;

                GameImages.m_ImgArr[ImageIndex].boUpdateStart := False;
              end;
            finally
              GameImages.UnLock;
            end;
          end else if ClientRequest.DataType in [udtIndexPak, udtIndexWzl] then begin
            GameImages.Lock;
            try
              //GameImages.m_boUpdateIndex := False;    // 这句一定不能加
              GameImages.m_boUpdateIndexing := False;
            finally
              GameImages.UnLock;
            end;
          end else if ClientRequest.DataType = udtFileWav then begin
            if (ClientRequest.ImgIndex >= 0) and (ClientRequest.ImgIndex < Length(g_SoundUpDateList)) then
              g_SoundUpDateList[ClientRequest.ImgIndex] := True;
          end;
        end;

        ClientRequest.FileName := '';
        Dispose(ClientRequest);
      end;
      List.Clear;
    finally
      List.UnLock;
    end;
  end;
end;

{ TSafeList }

constructor TUpdateThread.Create(AOwner:TUpdateEngine; AIsMapPriority:Boolean);
begin
  FreeOnTerminate := False;

  inherited Create(True);

  FIsConnect := False;
  FTryConnecttionTick := 0;

  FOwner := AOwner;
  FIsMapPriority := AIsMapPriority;

  FRequestedList := TSafeList.Create;
  FTimeOutList := TSafeList.Create;

  FTempList := TList.Create;
  FTempList.Capacity := 200;

  FClientSocket := nil;
  InitializeCriticalSection(FRecvCS);

  sRemainingSendData := '';

  FIsPasswordOK := False;
  FRecvText := '';
  FRecvStream := TMemoryStream.Create;
end;

destructor TUpdateThread.Destroy;
begin
  if Assigned(FClientSocket) then begin
    FClientSocket.Free;
  end;

  ClearRequests(FOwner, FRequestedList, False);
  FRequestedList.Free;

  ClearRequests(FOwner, FTimeOutList, False);
  FTimeOutList.Free;

  FTempList.Free;

  FRecvStream.Free;
  DeleteCriticalSection(FRecvCS);
  inherited;
end;

procedure TUpdateThread.Execute;
var
  I, RecvLen, SendLen:Integer;
  IsCheckOK:Boolean;
  SrvMsgHeader:TUpdateSrvMsgHeader;
  ClientMsgHeader:TUpdateClientMsgHeader;
  PData:PByte;
  MsgData:AnsiString;
  RequestedCount:Integer;
  TempClientRequest, ClientRequest:pTClientRequest;
  FileName:string;

  GameImages:TGameImages;
  ImageIndex:Integer;

  S1, S2:string;
  Hash1, Hash2, dwTemp:LongWord;
  MaxCount:Integer;
begin
  while not Terminated do begin
    // 更新未开启，不执行任何操作
    if not g_boAutoUpdate then begin
      //Sleep(1);
      SwitchToThread(); //HZQ
      Continue;
    end;

    // 未连接服务器或连接断开时，尝试重新连接，重连前把已发送但未收到的数据包加到未发送队列
    if not FIsConnect then begin
      if (MyGetTickCount - FTryConnecttionTick >= 10000) then begin
        // 连接前先把已发送的数据放到未发送队列
        FTempList.Clear;

        FRequestedList.Lock;
        try
          for I := 0 to FRequestedList.Count - 1 do begin
            TempClientRequest := FRequestedList.Items[I];
            FTempList.add(TempClientRequest);
          end;

          FRequestedList.Clear;
        finally
          FRequestedList.UnLock;
        end;

        FTimeOutList.Lock;
        try
          for I := 0 to FTimeOutList.Count - 1 do begin
            TempClientRequest := FTimeOutList.Items[I];
            FTempList.add(TempClientRequest);
          end;

          FTimeOutList.Clear;
        finally
          FTimeOutList.UnLock;
        end;

        for I := 0 to FTempList.Count - 1 do begin
          TempClientRequest := FTempList.Items[I];

          if TempClientRequest.IsMapData then begin
            FOwner.FRequestMapDataList.Lock;
            try
              FOwner.FRequestMapDataList.Add(TempClientRequest)
            finally
              FOwner.FRequestMapDataList.UnLock;
            end;
          end
          else if TempClientRequest.DataType in [udtFileWav, udtFileOther] then begin
            FOwner.FRequestFileList.Lock;
            try
              FOwner.FRequestFileList.Add(TempClientRequest)
            finally
              FOwner.FRequestFileList.UnLock;
            end;
          end
          else begin
            FOwner.FRequestImageList.Lock;
            try
              FOwner.FRequestImageList.Add(TempClientRequest)
            finally
              FOwner.FRequestImageList.UnLock;
            end;
          end;
        end;

        // 尝试重新连接
        FTryConnecttionTick := MyGetTickCount;
        Synchronize(TryConnect);
      end;

      Sleep(1);
      Continue;
    end;

    // -------------------------------------------数据只发送了一部分，这里把剩余的部分发完
    if Length(sRemainingSendData) > 0 then begin
      if MyGetTickCount - dwRemainingSendTick >= 1000 then begin
        MsgData := sRemainingSendData;
        SendLen := FClientSocket.Socket.SendText(MsgData);
        FLastSendTick := MyGetTickCount;

        if SendLen < Length(MsgData) then begin
          if SendLen > 0 then begin
            SetLength(sRemainingSendData, Length(MsgData) - SendLen);
            Move(MsgData[SendLen + 1], sRemainingSendData[1], Length(sRemainingSendData));
          end;

          dwRemainingSendTick := MyGetTickCount;
        end
        else begin
          sRemainingSendData := '';
        end;
      end;
    end

      // ----------------------------------------------从发送列表中取一条数据，然后发送
    else if FIsPasswordOK then begin
      FRequestedList.Lock;
      try
        RequestedCount := FRequestedList.Count;
      finally
        FRequestedList.UnLock;
      end;

      ClientRequest := nil;
      MaxCount := g_ConfigClient.wUpdateQueueCount;
      if RequestedCount < MaxCount then begin
        // 最先把超时加入重新下载中
        FTimeOutList.Lock;
        try
          if FTimeOutList.Count > 0 then begin
            ClientRequest := FTimeOutList.Items[0];
            FTimeOutList.Delete(0);
          end;
        finally
          FTimeOutList.UnLock;
        end;

        // 地图优先
        if FIsMapPriority then begin
          // 优先下载地图数据
          if ClientRequest = nil then begin
            FOwner.FRequestMapDataList.Lock;
            try
              if FOwner.FRequestMapDataList.Count > 0 then begin
                ClientRequest := FOwner.FRequestMapDataList.Items[0];
                FOwner.FRequestMapDataList.Delete(0);
              end;
            finally
              FOwner.FRequestMapDataList.UnLock;
            end;
          end;

          // 无地图数据，再下载图片数据
          if ClientRequest = nil then begin
            FOwner.FRequestImageList.Lock;
            try
              if FOwner.FRequestImageList.Count > 0 then begin
                ClientRequest := FOwner.FRequestImageList.Items[0];
                FOwner.FRequestImageList.Delete(0);
              end;
            finally
              FOwner.FRequestImageList.UnLock;
            end;
          end;
        end else begin
          // 优先下载图片数据
          if ClientRequest = nil then begin
            FOwner.FRequestImageList.Lock;
            try
              if FOwner.FRequestImageList.Count > 0 then begin
                ClientRequest := FOwner.FRequestImageList.Items[0];
                FOwner.FRequestImageList.Delete(0);
              end;
            finally
              FOwner.FRequestImageList.UnLock;
            end;
          end;

          // 无图片数据，再下载地图数据
          if ClientRequest = nil then begin
            FOwner.FRequestMapDataList.Lock;
            try
              if FOwner.FRequestMapDataList.Count > 0 then begin
                ClientRequest := FOwner.FRequestMapDataList.Items[0];
                FOwner.FRequestMapDataList.Delete(0);
              end;
            finally
              FOwner.FRequestMapDataList.UnLock;
            end;
          end;
        end;

        // 最后下载其他文件 (wav等)
        if ClientRequest = nil then begin
          FOwner.FRequestFileList.Lock;
          try
            if FOwner.FRequestFileList.Count > 0 then begin
              ClientRequest := FOwner.FRequestFileList.Items[0];
              FOwner.FRequestFileList.Delete(0);
            end;
          finally
            FOwner.FRequestFileList.UnLock;
          end;
        end;
      end;

      if ClientRequest <> nil then begin
        FRequestedList.Lock;
        try
          ClientRequest.SendTick := MyGetTickCount;
          FRequestedList.Add(ClientRequest);
        finally
          FRequestedList.UnLock;
        end;

        ClientMsgHeader.MsgFlag := Update_SOCKET_FLAG;
        ClientMsgHeader.RequestID := ClientRequest.RequestID;
        ClientMsgHeader.DataType := ClientRequest.DataType;
        ClientMsgHeader.Ident := CM_UpdateBUFFER;
        ClientMsgHeader.Param := 0;
        ClientMsgHeader.Index := ClientRequest.ImgIndex;
        ClientMsgHeader.DataLen := Length(ClientRequest.FileName);

        SetLength(MsgData, SizeOf(ClientMsgHeader) + ClientMsgHeader.DataLen);
        Move(ClientMsgHeader, MsgData[1], Length(MsgData));

        if Length(ClientRequest.FileName) > 0 then
          Move(ClientRequest.FileName[1], MsgData[1 + SizeOf(ClientMsgHeader)], ClientMsgHeader.DataLen);

        SendLen := FClientSocket.Socket.SendText(MsgData);
        FLastSendTick := MyGetTickCount;

        if SendLen < Length(MsgData) then begin
          SetLength(sRemainingSendData, Length(MsgData) - SendLen);
          Move(MsgData[SendLen + 1], sRemainingSendData[1], Length(sRemainingSendData));
          dwRemainingSendTick := MyGetTickCount;
        end;
      end;
    end;

    // -----------------------------------------------收数据包处理
    IsCheckOK := False;
    RecvLock;
    try
      RecvLen := Length(FRecvText);
      if RecvLen >= SizeOf(TUpdateSrvMsgHeader) then begin
        SrvMsgHeader := pTUpdateSrvMsgHeader(@FRecvText[1])^;
        if RecvLen >= SrvMsgHeader.DataLen + SizeOf(TUpdateSrvMsgHeader) then begin
          if SrvMsgHeader.MsgFlag <> Update_SOCKET_FLAG then begin
            FRecvText := '';
          end
          else begin
            IsCheckOK := True;
            FRecvStream.Clear;

            if (SrvMsgHeader.DataLen > 0) and (SrvMsgHeader.Ident <> WM_CHECK_CODE) then begin
              PData := PByte(FRecvText);
              Inc(PData, SizeOf(TUpdateSrvMsgHeader));
              IsCheckOK := Crc32(PData, SrvMsgHeader.DataLen) = SrvMsgHeader.DataCrc;

              if IsCheckOK then begin
                //SetLength(MsgData, SrvMsgHeader.DataLen);
                //Move(PData^, MsgData[1], SrvMsgHeader.DataLen);

                FRecvStream.SetSize(SrvMsgHeader.DataLen);
                Move(PData^, FRecvStream.Memory^, SrvMsgHeader.DataLen);
              end;
            end;

            FRecvText := Copy(FRecvText, SrvMsgHeader.DataLen + SizeOf(TUpdateSrvMsgHeader) + 1, MaxInt);
          end;
        end;
      end;
    finally
      RecvUnLock;
    end;

    // -----------------------------------------------从已发送列表中找到这个条数据
    if IsCheckOK then begin
      //RecvLen := SizeOf(TUpdateSrvMsgHeader) + SrvMsgHeader.DataLen; //HZQ 20230525后面没有用到

      if SrvMsgHeader.Ident = WM_CONNECT_RET then begin
        if SrvMsgHeader.Param = 0 then begin
          if not g_UpdateIsLogPassWordFail then begin
            //DebugOutStr('ExecBuffers Update PassWord Fail');
            DebugOutStr(DecodeResStr(SUpdatePasswordErr));
            g_UpdateIsLogPassWordFail := True;
          end;
        end else begin
          FIsPasswordOK := True;
        end;
      end else if SrvMsgHeader.Ident = WM_CHECK_CODE then begin
        {.$I VMProtectBegin.inc}
        S1 := #20#40#50 + IntToStr(LongWord(SrvMsgHeader.RequestID));
        S2 := #11#22#14 + IntToStr(LongWord(SrvMsgHeader.DataCrc));

        Hash1 := $AAAAAAAA;
        for I := 1 to Length(S1) do begin
          if i and 1 = 0 then
            Hash1 := Hash1 xor ((Hash1 shl 7) xor Ord(S1[I]) xor (Hash1 shr 3))
          else
            Hash1 := Hash1 xor (not ((Hash1 shl 13) xor Ord(S1[I]) xor (Hash1 shr 5)));
        end;

        Hash2 := 0;
        for I := 1 to Length(S2) do begin
          Hash2 := Hash2 shl 4 + Ord(S2[I]);
          dwTemp := Hash2 and $F0000000;
          if dwTemp <> 0 then
            Hash2 := Hash2 xor (dwTemp shr 24);
          Hash2 := Hash2 and (not dwTemp);
        end;

        ClientMsgHeader.MsgFlag := Update_SOCKET_FLAG;
        ClientMsgHeader.RequestID := Hash1;
        ClientMsgHeader.DataType := udtFileWav;
        ClientMsgHeader.Ident := CM_CHECKCODE_RECV;
        ClientMsgHeader.Param := 0;
        ClientMsgHeader.Index := Hash2;
        ClientMsgHeader.DataLen := 0;
        FClientSocket.Socket.SendBuf(ClientMsgHeader, SizeOf(ClientMsgHeader));

        {.$I VMProtectEnd.inc}
      end else if SrvMsgHeader.Ident = WM_UPDATE_STOP then begin
        //TempClientRequest := nil;
        ClientRequest := nil;

        FRequestedList.Lock;
        try
          for I := 0 to FRequestedList.Count - 1 do begin
            TempClientRequest := FRequestedList.Items[I];
            if TempClientRequest.RequestID = SrvMsgHeader.RequestID then begin
              FRequestedList.Delete(I);

              ClientRequest := TempClientRequest;
              Break;
            end;
          end;
        finally
          FRequestedList.UnLock;
        end;

        if ClientRequest = nil then begin
          FTimeOutList.Lock;
          try
            for I := 0 to FTimeOutList.Count - 1 do begin
              TempClientRequest := FTimeOutList.Items[I];
              if TempClientRequest.RequestID = SrvMsgHeader.RequestID then begin
                FTimeOutList.Delete(I);

                ClientRequest := TempClientRequest;
                Break;
              end;
            end;
          finally
            FTimeOutList.UnLock;
          end;
        end;

        if ClientRequest <> nil then begin
          if ClientRequest.DataType = udtFileWav then begin
            if (ClientRequest.ImgIndex >= 0) and (ClientRequest.ImgIndex < Length(g_SoundUpDateList)) then
              g_SoundUpDateList[ClientRequest.ImgIndex] := False;
          end
          else if ClientRequest.DataType = udtFileMap then begin

          end
          else if ClientRequest.DataType = udtFileOther then begin

          end
          else if ClientRequest.DataType in [udtImagePak, udtImageWzl] then begin
            GameImages := ClientRequest.GameImages;
            if GameImages <> nil then begin
              GameImages.Lock;
              try
                ImageIndex := ClientRequest.ImgIndex;
                if (ImageIndex >= 0) and (ImageIndex < GameImages.ImageCount) then begin
                  GameImages.m_ImgArr[ImageIndex].boUpdateStop := True;
                  GameImages.m_ImgArr[ImageIndex].boUpdateStart := False;
                end;
              finally
                GameImages.UnLock;
              end;
            end;
          end
          else if ClientRequest.DataType in [udtIndexPak, udtIndexWzl] then begin
            GameImages := ClientRequest.GameImages;
            if GameImages <> nil then begin
              GameImages.Lock;
              try
                GameImages.m_boNeedUpdate := False;
                GameImages.m_boUpdateIndex := False;
                GameImages.m_boUpdateIndexing := False;
              finally
                GameImages.UnLock;
              end;
            end;
          end;

          Dispose(ClientRequest);
        end;
      end else if (SrvMsgHeader.Ident = WM_COMPDATA) or (SrvMsgHeader.Ident = WM_DATA) then begin
        //TempClientRequest := nil;
        ClientRequest := nil;

        FRequestedList.Lock;
        try
          for I := 0 to FRequestedList.Count - 1 do begin
            TempClientRequest := FRequestedList.Items[I];
            if TempClientRequest.RequestID = SrvMsgHeader.RequestID then begin
              FRequestedList.Delete(I);

              ClientRequest := TempClientRequest;
              Break;
            end;
          end;
        finally
          FRequestedList.UnLock;
        end;

        if ClientRequest = nil then begin
          FTimeOutList.Lock;
          try
            for I := 0 to FTimeOutList.Count - 1 do begin
              TempClientRequest := FTimeOutList.Items[I];
              if TempClientRequest.RequestID = SrvMsgHeader.RequestID then begin
                FTimeOutList.Delete(I);

                ClientRequest := TempClientRequest;
                Break;
              end;
            end;
          finally
            FTimeOutList.UnLock;
          end;
        end;

        if ClientRequest <> nil then begin
          FileName := g_sSelfFilePath + ClientRequest.FileName;

          if ClientRequest.DataType = udtFileWav then
            UpdateFileWav(FileName, ClientRequest.ImgIndex, SrvMsgHeader.Ident = WM_COMPDATA, ClientRequest.SaveToFile)
          else if ClientRequest.DataType = udtFileMap then
            UpdateFileMap(FileName, SrvMsgHeader.Ident = WM_COMPDATA, ClientRequest.ImgIndex, ClientRequest.SaveToFile)
          else if ClientRequest.DataType = udtFileOther then
            UpdateFileOther(FileName, SrvMsgHeader.Ident = WM_COMPDATA, ClientRequest.SaveToFile)
          else if ClientRequest.DataType = udtImagePak then begin
            UpdateImagePak(FileName, ClientRequest.ImgIndex, SrvMsgHeader.Ident = WM_COMPDATA, TPakImages(ClientRequest.GameImages), ClientRequest.IsMapData);
            {$IF LOG_UPDATE = 1}
            if (Pos('Tiles129.pak', ClientRequest.FileName) > 0) then begin
              FOwner.WriteLog(ClientRequest.RequestID, ClientRequest.ImgIndex, '完成:' + IntToStr(FRecvStream.Size));
            end;
            {$IFEND}
          end
          else if ClientRequest.DataType = udtImageWzl then
            UpdateImageWzl(FileName, ClientRequest.ImgIndex, SrvMsgHeader.Ident = WM_COMPDATA, TWzlImages(ClientRequest.GameImages), ClientRequest.IsMapData)
          else if ClientRequest.DataType = udtIndexPak then
            UpdateIndexPak(FileName, SrvMsgHeader.Ident = WM_COMPDATA, TPakImages(ClientRequest.GameImages), ClientRequest.ImgIndex = 0)
          else if ClientRequest.DataType = udtIndexWzl then
            UpdateIndexWzl(FileName, SrvMsgHeader.Ident = WM_COMPDATA, TWzlImages(ClientRequest.GameImages));

          Dispose(ClientRequest);
        end;
      end;
    end;

    // ---------------------------------------------长时间没有发数据包到服务器，发一个心跳过去，告诉服务器客户还活着
    if MyGetTickCount - FLastSendTick >= 30000 then begin
      FLastSendTick := MyGetTickCount;

      ClientMsgHeader.MsgFlag := Update_SOCKET_FLAG;
      ClientMsgHeader.RequestID := 0;
      ClientMsgHeader.Ident := CM_HEARTBEAT;
      ClientMsgHeader.Param := 0;
      ClientMsgHeader.DataLen := 0;

      SendLen := FClientSocket.Socket.SendBuf(ClientMsgHeader, SizeOf(ClientMsgHeader));

      if SendLen < SizeOf(ClientMsgHeader) then begin
        SetLength(sRemainingSendData, SizeOf(ClientMsgHeader) - SendLen);
        Move(PByte(Integer(@ClientMsgHeader) + SendLen)^, sRemainingSendData[1], Length(sRemainingSendData));
        dwRemainingSendTick := MyGetTickCount;
      end;
    end;

    // --------------------------------------------发送的包长时间没收到回复
    FTempList.Clear;
    FRequestedList.Lock;
    try
      for I := FRequestedList.Count - 1 downto 0 do begin
        TempClientRequest := FRequestedList.Items[I];
        if TempClientRequest.DataType in [udtFileWav, udtFileMap, udtFileOther] then begin
          if MyGetTickCount - TempClientRequest.SendTick >= 30000 + Cardinal(TempClientRequest.TimeOutCount) * 5000 then begin
            //DScreen.AddChatBoardString('更新超时: ' + TempClientRequest.FileName, clRed, clBlack);

            if TempClientRequest.TimeOutCount < 10 then begin
              Inc(TempClientRequest.TimeOutCount);
            end;

            if TempClientRequest.TimeOutCount = 10 then begin
              //DebugOutStr('[TimeOut] TUpdateEngine::Update file:' + TempClientRequest.FileName);
              DebugOutStr(Format(DecodeResStr(SUpdateTimeOutFile), [TempClientRequest.FileName]));
              Inc(TempClientRequest.TimeOutCount);
            end;

            FTempList.Add(TempClientRequest);
            FRequestedList.Delete(I);
          end;
        end else begin
          if MyGetTickCount - TempClientRequest.SendTick >= 15000 + Cardinal(TempClientRequest.TimeOutCount) * 1000 then begin
            if TempClientRequest.TimeOutCount < 5 then begin
              Inc(TempClientRequest.TimeOutCount);
            end;
            {
            if TempClientRequest.DataType in [udtIndexPak, udtIndexWzl] then
              DScreen.AddChatBoardString('更新超时: ' + TempClientRequest.FileName, clRed, clBlack)
            else
              DScreen.AddChatBoardString('更新超时: ' + TempClientRequest.FileName + ';  index:' + IntToStr(TempClientRequest.ImgIndex), clRed, clBlack);
            }

            if TempClientRequest.TimeOutCount = 5 then begin
              if TempClientRequest.DataType in [udtIndexPak, udtIndexWzl] then begin
                //DebugOutStr('[TimeOut] TUpdateEngine::Update index:' + TempClientRequest.FileName)
                DebugOutStr(Format(DecodeResStr(SUpdateTimeOutIndex), [TempClientRequest.FileName]));
              end else begin
                //DebugOutStr('[TimeOut] TUpdateEngine::Update image:' + TempClientRequest.FileName + ';  index:' + IntToStr(TempClientRequest.ImgIndex));
                DebugOutStr(Format(DecodeResStr(SUpdateTimeOutImage), [TempClientRequest.FileName, TempClientRequest.ImgIndex]));
              end;
              Inc(TempClientRequest.TimeOutCount);
            end;

            FTempList.Add(TempClientRequest);
            FRequestedList.Delete(I);
          end;
        end;
      end;
    finally
      FRequestedList.UnLock;
    end;

    if FTempList.Count > 0 then begin
      FTimeOutList.Lock;
      try
        for I := 0 to FTempList.Count - 1 do begin
          FTimeOutList.Add(FTempList.Items[I]);
        end;
      finally
        FTimeOutList.UnLock;
      end;
    end;

    // 优化更新占用cpu 2020-03-14 21:43:32
    Sleep(1);
  end;
end;

procedure TUpdateThread.UpdateFileWav(FileName:string; Index:Integer; IsCompress:Boolean;
  SaveToFile:TStreamSaveToFile);
var
  FilePath:string;
  OutStream:TMemoryStream;
begin
  FilePath := ExtractFilePath(FileName);
  if not DirectoryExists(FilePath) then begin
    ForceDirectories(FilePath);
  end;

  if not IsCompress then begin
    SaveToSoundFile(FRecvStream, Index, FileName);
  end
  else begin
    OutStream := TMemoryStream.Create;
    try
      FRecvStream.Seek(0, soFromBeginning);
      try
        ZipDecompressStream(FRecvStream, OutStream);
      except
        //DebugOutStr('[Exception] TUpdateEngine::UpdateWav Decompress:' + FileName);
        DebugOutStr(Format(DecodeResStr(SUpdateWavDecompressErr), [FileName]));
      end;
      OutStream.Seek(0, soFromBeginning);

      SaveToSoundFile(OutStream, Index, FileName)
    finally
      OutStream.Free;
    end;
  end;
end;

procedure TUpdateThread.UpdateFileMap(FileName:string; IsCompress:Boolean; nPos:Integer;
  SaveToFile:TStreamSaveToFile);
var
  FilePath:string;
  OutStream:TMemoryStream;
begin
  FilePath := ExtractFilePath(FileName);
  if not DirectoryExists(FilePath) then begin
    ForceDirectories(FilePath);
  end;

  if not IsCompress then begin
    FRecvStream.Seek(0, soFromBeginning);

    if Assigned(SaveToFile) then begin
      SaveToFile(Self, FRecvStream, 0, FileName);
    end
    else begin
      FRecvStream.SaveToFile(FileName);
    end;
  end
  else begin
    OutStream := TMemoryStream.Create;
    try
      FRecvStream.Seek(0, soFromBeginning);
      try
        ZipDecompressStream(FRecvStream, OutStream);
      except
        //DebugOutStr('[Exception] TUpdateEngine::UpdateMap Decompress:' + FileName);
        DebugOutStr(Format(DecodeResStr(SUpdateMapDecompressErr), [FileName]));
      end;
      OutStream.Seek(0, soFromBeginning);

      if Assigned(SaveToFile) then begin
        SaveToFile(Self, OutStream, nPos, FileName);
      end
      else begin
        OutStream.SaveToFile(FileName);
      end;
    finally
      OutStream.Free;
    end;
  end;
end;

procedure TUpdateThread.UpdateFileOther(FileName:string; IsCompress:Boolean;
  SaveToFile:TStreamSaveToFile);
var
  OutStream:TMemoryStream;
  FilePath:string;
begin
  FilePath := ExtractFilePath(FileName);
  if not DirectoryExists(FilePath) then begin
    ForceDirectories(FilePath);
  end;

  if not IsCompress then begin
    FRecvStream.SaveToFile(FileName);
  end
  else begin
    OutStream := TMemoryStream.Create;
    try
      FRecvStream.Seek(0, soFromBeginning);
      try
        ZipDecompressStream(FRecvStream, OutStream);
      except
        //DebugOutStr('[Exception] TUpdateEngine::UpdateOther Decompress:' + FileName);
        DebugOutStr(Format(DecodeResStr(SUpdateOtherDecompressErr), [FileName]));
      end;
      OutStream.Seek(0, soFromBeginning);
      OutStream.SaveToFile(FileName);
    finally
      OutStream.Free;
    end;
  end;
end;

procedure TUpdateThread.UpdateImagePak(FileName:string; Index:Integer;
  IsCompress:Boolean; GameImages:TPakImages; IsMapImage:Boolean);
var
  nPosition:Integer;
  OutBuf:Pointer;
  OutBytes:Integer;
  WriteLen:Integer;
begin
  OutBuf := nil;

  if IsCompress then begin
    try
      ZipDecompressBuffer(FRecvStream.Memory, FRecvStream.Size, OutBuf, OutBytes);
    except
      DebugOutStr(Format(DecodeResStr(SUpdatePakDecompressErr), [GameImages.FileName, Index]));
      if OutBuf <> nil then FreeMem(OutBuf);
      OutBuf := nil;
    end;
  end
  else begin
    OutBuf := FRecvStream.Memory;
    OutBytes := FRecvStream.Size;
  end;

  if OutBuf = nil then Exit;
  GameImages.Lock;
  try
    try
      if (Index >= 0) and (Index < GameImages.ImageCount) and
        ((GameImages.Images[Index] = nil) or (GameImages.Images[Index] = g_NullImage)) and
        (GameImages.m_FileStream <> nil) then begin
        GameImages.LockFileStream;
        try
          nPosition := GameImages.m_FileStream.Seek(0, soEnd);
          WriteLen := GameImages.m_FileStream.Write(OutBuf^, OutBytes);
          GameImages.m_IndexList.Items[Index] := Pointer(nPosition);
          GameImages.UpdateImageDataSize(Index, OutBytes); 
          GameImages.WriteIndexList(GameImages.m_FileStream, Index);
        finally
          GameImages.UnLockFileStream;
        end;

        if WriteLen <> OutBytes then begin
          DebugOutStr(Format(DecodeResStr(SUpdatePakWriteLenErr), [GameImages.FileName, Index]));
        end;
      end;

      if IsMapImage then begin
        FOwner.FNeedDrawTileMap := True;
      end;
    except
      DebugOutStr(Format(DecodeResStr(SUpdatePakWriteErr), [GameImages.FileName, Index]));
    end;

    if (Index >= 0) and (Index < GameImages.ImageCount) then begin
      GameImages.m_ImgArr[Index].boUpdateStart := False;

      if GameImages = g_WNewopUIImages then begin
        ResetNewopUI170TextureArray(Index);
      end;
    end;
  finally
    GameImages.UnLock;
  end;

  try
    if IsCompress and (OutBuf <> nil) then
      FreeMem(OutBuf);
  except
    DebugOutStr(Format(DecodeResStr(SUpdatePakFreeMemErr), [GameImages.FileName, Index]));
  end;
end;

procedure TUpdateThread.UpdateImageWzl(FileName:string; Index:Integer;
  IsCompress:Boolean; GameImages:TWzlImages; IsMapImage:Boolean);
var
  nSeekPos, nPosition:Integer;
  OutBuf:Pointer;
  OutBytes:Integer;
  IndexStream:TFileStream;
  WriteLen:Integer;
begin
  OutBuf := nil;

  if IsCompress then begin
    try
      ZipDecompressBuffer(FRecvStream.Memory, FRecvStream.Size, OutBuf, OutBytes);
    except
      DebugOutStr(Format(DecodeResStr(SUpdateWzlDecompressErr), [GameImages.FileName, Index]));
      if OutBuf <> nil then
        FreeMem(OutBuf);
      OutBuf := nil;
    end;
  end
  else begin
    OutBuf := FRecvStream.Memory;
    OutBytes := FRecvStream.Size;
  end;

  if OutBuf = nil then Exit;

  GameImages.Lock;
  try
    try
      IndexStream := TFileStream.Create(GameImages.IndexFileName, fmOpenReadWrite or fmShareDenyNone);
      if IndexStream = nil then Exit;
      nPosition := 0;
      nSeekPos := SizeOf(TWzlIndexHeader) + SizeOf(Integer) * Index;

      if IndexStream.Seek(nSeekPos, soFromBeginning) = nSeekPos then
        IndexStream.Read(nPosition, SizeOf(Integer));

      if nPosition <= 0 then begin
        if (Index >= 0) and (Index < GameImages.ImageCount) and ((GameImages.Images[Index] = nil) or (GameImages.Images[Index] = g_NullImage)) then begin
          GameImages.LockFileStream;
          try
            nPosition := GameImages.m_FileStream.Seek(0, soEnd);
            WriteLen := GameImages.m_FileStream.Write(OutBuf^, OutBytes);
            GameImages.m_IndexList.Items[Index] := Pointer(nPosition);
          finally
            GameImages.UnLockFileStream;
          end;

          if WriteLen <> OutBytes then begin
            DebugOutStr(Format(DecodeResStr(SUpdateWzlWriteErr), [GameImages.FileName, Index]));
          end;

          IndexStream.Seek(SizeOf(TWzlIndexHeader) + SizeOf(Integer) * Index, soFromBeginning);
          IndexStream.Write(nPosition, SizeOf(Integer));
        end;
      end
      else begin
        GameImages.m_IndexList.Items[Index] := Pointer(nPosition);
      end;

      IndexStream.Free;
      if IsMapImage then begin
        FOwner.FNeedDrawTileMap := True;
      end;
    except
      DebugOutStr(Format(DecodeResStr(SUpdateWzlImageErr), [GameImages.FileName, Index]));
    end;

    if (Index >= 0) and (Index < GameImages.ImageCount) then
      GameImages.m_ImgArr[Index].boUpdateStart := False;
  finally
    GameImages.UnLock;
  end;

  try
    if IsCompress and (OutBuf <> nil) then
      FreeMem(OutBuf);
  except
    DebugOutStr(Format(DecodeResStr(SUpdateWzlFreeMemErr), [GameImages.FileName, Index]));
  end;
end;

procedure TUpdateThread.UpdateIndexPak(FileName:string; IsCompress:Boolean;
  GameImages:TPakImages; IsCompareHeader:Boolean);
begin
  if (FRecvStream.Size = SizeOf(TPakKey)) and SameText(GameImages.FileName, FileName) then begin
    GameImages.Lock;
    try
      GameImages.UpdateIndex(pTPakKey(FRecvStream.Memory), IsCompareHeader);
      GameImages.m_boUpdateIndex := False;
      GameImages.m_boUpdateIndexing := False;
    finally
      GameImages.UnLock;
    end;
  end;
end;

procedure TUpdateThread.UpdateIndexWzl(FileName:string; IsCompress:Boolean;
  GameImages:TWzlImages);
var
  IndexCount:Integer;
  FileStream:TFileStream;
  sFilePath:string;
  FileHeader:TWzlImageHeader;
  IndexHeader:TWzlIndexHeader;
  boInitialize:Boolean;
begin
  boInitialize := False;
  try
    IndexCount := PInteger(FRecvStream.Memory)^;
    if (GameImages <> nil) and SameText(GameImages.FileName, FileName) then begin
      try
        if GameImages.Initialized then begin
          if GameImages.ImageCount <> IndexCount then begin
            boInitialize := True;
            GameImages.Finalize;
          end;
        end
        else begin
          boInitialize := True;
        end;

        if FileExists(GameImages.IndexFileName) then begin
          if boInitialize then begin
            FileStream := TFileStream.Create(GameImages.IndexFileName, fmOpenReadWrite or fmShareDenyNone);
            FileStream.Read(IndexHeader, SizeOf(TWzlIndexHeader));
            FileStream.Seek(0, soFromBeginning);
            IndexHeader.IndexCount := IndexCount;
            FileStream.Write(IndexHeader, SizeOf(TWzlIndexHeader));
            FileStream.Size := SizeOf(TWzlIndexHeader) + SizeOf(Integer) * IndexHeader.IndexCount;
          end else begin
            FileStream := nil; //HZQ 20230525
          end;
        end else begin
          sFilePath := ExtractFilePath(GameImages.IndexFileName);
          if not DirectoryExists(sFilePath) then begin
            ForceDirectories(sFilePath);
          end;

          FileStream := TFileStream.Create(GameImages.IndexFileName, fmOpenReadWrite or fmShareDenyNone or fmCreate);
          FileStream.Seek(0, soFromBeginning);
          IndexHeader.Title := 'www.shandagames.com';
          IndexHeader.IndexCount := IndexCount;
          FileStream.Write(IndexHeader, SizeOf(TWzlIndexHeader));
          FileStream.Size := SizeOf(TWzlIndexHeader) + SizeOf(Integer) * IndexHeader.IndexCount;
        end;
        if FileStream <> nil then begin //HZQ 20230525
          FileStream.Free;
        end;
      except
        GameImages.m_boNeedUpdate := False;
        //DebugOutStr('[Exception] TUpdateEngine::UpdateIndexWzl 1:' + FileName);
        DebugOutStr(Format(DecodeResStr(SUpdateWzlIndexErr1), [FileName]));
      end;

      if boInitialize then begin
        FillChar(FileHeader, SizeOf(TWzlImageHeader), 0);
        FileHeader.Title := 'www.shandagames.com';
        FileHeader.ImageCount := IndexCount;
        try
          if FileExists(GameImages.FileName) then begin // Wzl文件不存在 开始创建
            FileStream := TFileStream.Create(GameImages.FileName, fmOpenReadWrite or fmShareDenyNone);
            FileStream.Write(FileHeader, SizeOf(TWzlImageHeader));
            FileStream.Free;
          end
          else begin
            sFilePath := ExtractFilePath(GameImages.FileName);
            if not DirectoryExists(sFilePath) then begin
              ForceDirectories(sFilePath);
            end;

            FileStream := TFileStream.Create(GameImages.FileName, fmOpenReadWrite or fmShareDenyNone or fmCreate);
            FileStream.Write(FileHeader, SizeOf(TWzlImageHeader));
            FileStream.Free;
          end;
        except
          GameImages.m_boNeedUpdate := False;
          //DebugOutStr('[Exception] TUpdateEngine::UpdateIndexWzl 2:' + FileName);
          DebugOutStr(Format(DecodeResStr(SUpdateWzlIndexErr2), [FileName]));
        end;
      end;
    end;

    if (GameImages <> nil) then begin
      GameImages.Lock;
      try
        GameImages.m_boUpdateIndex := False;
        GameImages.m_boUpdateIndexing := False;
      finally
        GameImages.UnLock;
      end;

      if boInitialize then
        GameImages.Initialize;
    end;
  except
    //DebugOutStr('[Exception] TUpdateEngine::UpdateIndexWzl 3:' + FileName);
    DebugOutStr(Format(DecodeResStr(SUpdateWzlIndexErr3), [FileName]));
  end;
end;

procedure TUpdateThread.ClientSocketConnect(Sender:TObject;
  Socket:TCustomWinSocket);
var
  ClientMsgHeader:TUpdateClientMsgHeader;
  S:AnsiString;
begin
  FIsConnect := True;

  FRecvText := '';
  FLastRecvTick := MyGetTickCount;
  FLastSendTick := MyGetTickCount;
  sRemainingSendData := '';
  FIsPasswordOK := False;

  ClientMsgHeader.MsgFlag := Update_SOCKET_FLAG;
  ClientMsgHeader.RequestID := 0;
  ClientMsgHeader.Ident := CM_SOCKETCONNECT;
  ClientMsgHeader.Param := 0;
  ClientMsgHeader.DataLen := Length(g_sUpdatePassword);

  SetLength(S, SizeOf(ClientMsgHeader) + ClientMsgHeader.DataLen);

  Move(ClientMsgHeader, S[1], SizeOf(ClientMsgHeader));
  if Length(g_sUpdatePassword) > 0 then begin
    Move(g_sUpdatePassword[1], S[SizeOf(ClientMsgHeader) + 1], ClientMsgHeader.DataLen);
  end;

  Socket.SendText(S);
end;

procedure TUpdateThread.ClientSocketDisconnect(Sender:TObject;
  Socket:TCustomWinSocket);
begin
  FIsConnect := False;
  FRecvText := '';
  FLastRecvTick := MyGetTickCount;
  FLastSendTick := MyGetTickCount;
  sRemainingSendData := '';
  FIsPasswordOK := False;
end;

procedure TUpdateThread.ClientSocketError(Sender:TObject;
  Socket:TCustomWinSocket; ErrorEvent:TErrorEvent;
  var ErrorCode:Integer);
begin
  ErrorCode := 0;
  Socket.Close;
end;

procedure TUpdateThread.ClientSocketRead(Sender:TObject; Socket:TCustomWinSocket);
var
  RecvLen:Integer;
begin
  RecvLen := Socket.ReceiveLength;

  FLastRecvTick := MyGetTickCount;
  RecvLock;
  try
    FRecvText := FRecvText + Socket.ReceiveText;
  finally
    RecvUnLock;
  end;

  g_UpdateSizeLock.Enter;
  try
    Inc(g_UpdateSize, RecvLen);
    Inc(g_UpdateTotalSize, RecvLen);
  finally
    g_UpdateSizeLock.Leave;
  end;
end;

procedure TUpdateThread.TryConnect;
begin
  if Assigned(FClientSocket) then begin
    FClientSocket.Free;
    FClientSocket := nil;
  end;

  FClientSocket := TClientSocket.Create(nil);
  FClientSocket.ClientType := ctNonBlocking;
  FClientSocket.OnError := ClientSocketError;
  FClientSocket.OnConnect := ClientSocketConnect;
  FClientSocket.OnDisconnect := ClientSocketDisconnect;
  FClientSocket.OnRead := ClientSocketRead;

  if CheckIP(g_sUpdateAddr) then
    FClientSocket.Address := g_sUpdateAddr
  else
    FClientSocket.Host := g_sUpdateAddr; // '127.0.0.1';
  FClientSocket.Port := g_nUpdatePort;

  FClientSocket.Active := True;
end;

procedure TUpdateThread.RecvLock;
begin
  EnterCriticalSection(FRecvCS);
end;

procedure TUpdateThread.RecvUnLock;
begin
  LeaveCriticalSection(FRecvCS);
end;

procedure TUpdateThread.SendClearMsg;
var
  ClientMsgHeader:TUpdateClientMsgHeader;
  SendLen:Integer;
begin
  if FIsConnect then begin
    ClientMsgHeader.MsgFlag := Update_SOCKET_FLAG;
    ClientMsgHeader.RequestID := 0;
    ClientMsgHeader.Ident := CM_CLEAR_MSG;
    ClientMsgHeader.Param := 0;
    ClientMsgHeader.DataLen := 0;

    SendLen := FClientSocket.Socket.SendBuf(ClientMsgHeader, SizeOf(ClientMsgHeader));

    if SendLen < SizeOf(ClientMsgHeader) then begin
      SetLength(sRemainingSendData, SizeOf(ClientMsgHeader) - SendLen);
      Move(PByte(Integer(@ClientMsgHeader) + SendLen)^, sRemainingSendData[1], Length(sRemainingSendData));
      dwRemainingSendTick := MyGetTickCount;
    end;

    FLastSendTick := MyGetTickCount;
  end;
end;

{ TUpdateEngine }

constructor TUpdateEngine.Create;
var
  I:Integer;
  UpdateThread:TUpdateThread;
  ThreadCount:Cardinal;
  SystemInfo:TSystemInfo;
begin
  inherited Create();

  {$IF LOG_UPDATE = 1}
  FLogFileName := ExtractFilePath(ParamStr(0)) + IntToStr(MyGetTickCount) + '.log';
  InitializeCriticalSection(FCS);
  {$IFEND}

  FRequestID := 0;
  FRequestMapDataList := TSafeList.Create;
  FRequestFileList := TSafeList.Create;
  FRequestImageList := TSafeList.Create;
  FUpdateThreadList := TList.Create;

  // 优化更新占用cpu 2020-03-14 21:43:32
  GetSystemInfo(SystemInfo);
  ThreadCount := g_ConfigClient.wUpdateThreadCount;
  if ThreadCount > SystemInfo.dwNumberOfProcessors * 2 then
    ThreadCount := SystemInfo.dwNumberOfProcessors * 2;

  for I := 0 to ThreadCount - 1 do begin
    UpdateThread := TUpdateThread.Create(Self, I <= 3);
    FUpdateThreadList.Add(UpdateThread);
  end;

  for I := 0 to FUpdateThreadList.Count - 1 do begin
    UpdateThread := FUpdateThreadList.Items[I];
    UpdateThread.Resume;
  end;
end;

destructor TUpdateEngine.Destroy;
var
  I:Integer;
  UpdateThread:TUpdateThread;
begin
  ClearRequests(Self, FRequestMapDataList, False);
  ClearRequests(Self, FRequestFileList, False);
  ClearRequests(Self, FRequestImageList, False);

  FRequestMapDataList.Free;
  FRequestFileList.Free;
  FRequestImageList.Free;

  for I := 0 to FUpdateThreadList.Count - 1 do begin
    UpdateThread := FUpdateThreadList.Items[I];
    UpdateThread.Terminate;
    Sleep(10);
  end;

  for I := 0 to FUpdateThreadList.Count - 1 do begin
    UpdateThread := FUpdateThreadList.Items[I];
    UpdateThread.Free;
  end;

  FUpdateThreadList.Free;

  {$IF LOG_UPDATE = 1}
  DeleteCriticalSection(FCS);
  {$IFEND}

  inherited;
end;

{$IF LOG_UPDATE = 1}

procedure TUpdateEngine.WriteLog(RequestID:LongWord; ImgIndex:Integer; LogText:string);
var
  fhandle:TextFile;
begin
  EnterCriticalSection(FCS);
  try
    try
      AssignFile(fhandle, FLogFileName);

      if FileExists(FLogFileName) then begin
        {$I-}
        Append(fhandle);
        {$I+}
      end
      else begin
        {$I-}
        Rewrite(fhandle);
        {$I+}
      end;

      Writeln(fhandle, IntToStr(RequestID) + #9 + IntToStr(ImgIndex) + #9 + LogText);
    finally
      CloseFile(fhandle);
    end;
  finally
    LeaveCriticalSection(FCS);
  end;
end;
{$IFEND}

function TUpdateEngine.FindRequestFile(FileName:string; DataType:TUpdateDataType):Boolean;
var
  I, II:Integer;
  ClientRequest:pTClientRequest;
  UpdateThread:TUpdateThread;
begin
  Result := False;

  if DataType = udtFileMap then begin
    FRequestMapDataList.Lock;
    try
      for I := 0 to FRequestMapDataList.Count - 1 do begin
        ClientRequest := FRequestMapDataList.Items[I];

        if (ClientRequest.DataType = DataType) and SameText(ClientRequest.FileName, FileName) then begin
          Result := True;
          Break;
        end;
      end;
    finally
      FRequestMapDataList.UnLock;
    end;
  end;

  if not Result then begin
    for I := 0 to FUpdateThreadList.Count - 1 do begin
      UpdateThread := FUpdateThreadList.Items[I];

      UpdateThread.FRequestedList.Lock;
      try
        for II := 0 to UpdateThread.FRequestedList.Count - 1 do begin
          ClientRequest := UpdateThread.FRequestedList.Items[II];

          if (ClientRequest.DataType = DataType) and SameText(ClientRequest.FileName, FileName) then begin
            Result := True;
            Exit;
          end;
        end;
      finally
        UpdateThread.FRequestedList.UnLock;
      end;

      if not Result then begin
        UpdateThread.FTimeOutList.Lock;
        try
          for II := 0 to UpdateThread.FTimeOutList.Count - 1 do begin
            ClientRequest := UpdateThread.FTimeOutList.Items[II];

            if (ClientRequest.DataType = DataType) and SameText(ClientRequest.FileName, FileName) then begin
              Result := True;
              Exit;
            end;
          end;
        finally
          UpdateThread.FTimeOutList.UnLock;
        end;
      end;
    end;
  end;
end;

function TUpdateEngine.Add(FileName:string; DataType:TUpdateDataType;
  ImgIndex:Integer; GameImages:TGameImages;
  SaveToFile:TStreamSaveToFile = nil):Boolean;
const
  UpdateDateTypeNames:array[TUpdateDataType] of string = (
    'FileWav', 'FileMap', 'FileDat', 'ImagePak', 'ImageWzl', 'IndexPak', 'IndexWzl');
var
  ClientRequest:pTClientRequest;
  FilePath, FileNameOnly:string;
  IsMapImage:Boolean;
  //FileHandle: TextFile;
  //LogFileName: string;
begin
  Result := False;
  FileName := Copy(FileName, Length(g_sSelfFilePath) + 1, MaxInt);
  if Length(FileName) = 0 then Exit;

  if DataType in [udtFileWav, udtFileMap, udtFileOther] then begin
    if not FindRequestFile(FileName, DataType) then begin
      New(ClientRequest);
      ClientRequest.RequestID := GetRequestID;
      ClientRequest.IsMapData := DataType = udtFileMap;
      ClientRequest.TimeOutCount := 0;
      ClientRequest.FileName := FileName;
      ClientRequest.DataType := DataType;
      ClientRequest.ImgIndex := ImgIndex;
      ClientRequest.GameImages := GameImages;
      ClientRequest.SaveToFile := SaveToFile;
      ClientRequest.AddTick := MyGetTickCount;

      if ClientRequest.IsMapData then begin
        FRequestMapDataList.Lock;
        try
          FRequestMapDataList.Add(ClientRequest);
        finally
          FRequestMapDataList.UnLock;
        end;
        Result := True;
      end
      else begin
        FRequestFileList.Lock;
        try
          FRequestFileList.Add(ClientRequest);
        finally
          FRequestFileList.UnLock;
        end;
        Result := True;
      end;
    end;
  end
  else begin
    (*
    LogFileName := 'd:\RequestImage.txt';
    AssignFile(FileHandle, LogFileName);
    if FileExists(LogFileName) then
    begin
      {$I-}
      Append(FileHandle);
      {$I+}
    end
    else
    begin
      {$I-}
      Rewrite(FileHandle);
      {$I+}
    end;
    Writeln(FileHandle, UpdateDateTypeNames[DataType] + FileName);
    CloseFile(FileHandle);
    *)

    IsMapImage := False;
    FilePath := Copy(ExtractFilePath(FileName), Length(g_ResourcesDir) + 1, MaxInt);
    FileNameOnly := ExtractFileName(FileName);

    if SameText(Copy(FilePath, 1, 13), '\Mir3MapData\') then begin
      IsMapImage := True;
    end
    else if SameText(Copy(FilePath, 1, 10), '\HMapData\') then begin
      IsMapImage := True;
    end
    else if SameText(Copy(FileNameOnly, 1, 7), 'OBJECTS') then begin
      IsMapImage := True;
    end
    else if SameText(Copy(FileNameOnly, 1, 7), 'SMTILES') then begin
      IsMapImage := True;
    end
    else if SameText(Copy(FileNameOnly, 1, 5), 'TILES') then begin
      IsMapImage := True;
    end;

    New(ClientRequest);
    ClientRequest.RequestID := GetRequestID;
    ClientRequest.IsMapData := IsMapImage;
    ClientRequest.TimeOutCount := 0;
    ClientRequest.FileName := FileName;
    ClientRequest.DataType := DataType;
    ClientRequest.ImgIndex := ImgIndex;
    ClientRequest.GameImages := GameImages;
    ClientRequest.SaveToFile := SaveToFile;
    ClientRequest.AddTick := MyGetTickCount;

    {$IF LOG_UPDATE = 1}
    if SameText(FileNameOnly, 'Tiles129.pak') and (DataType = udtImagePak) then begin
      WriteLog(ClientRequest.RequestID, ImgIndex, '添加');
    end;
    {$IFEND}

    if ClientRequest.IsMapData then begin
      FRequestMapDataList.Lock;
      try
        FRequestMapDataList.Add(ClientRequest);
      finally
        FRequestMapDataList.UnLock;
      end;
    end
    else begin
      FRequestImageList.Lock;
      try
        FRequestImageList.Add(ClientRequest);
      finally
        FRequestImageList.UnLock;
      end;
    end;

    Result := True;
  end;
end;

function TUpdateEngine.GetRequestID:LongWord;
begin
  InterlockedIncrement(FRequestID);
  Result := FRequestID;
end;

procedure TUpdateEngine.ClearRequestList;
var
  I:Integer;
  UpdateThread:TUpdateThread;
begin
  ClearRequests(Self, FRequestFileList, True);
  ClearRequests(Self, FRequestMapDataList, True);
  ClearRequests(Self, FRequestImageList, True);

  for I := 0 to FUpdateThreadList.Count - 1 do begin
    UpdateThread := FUpdateThreadList.Items[I];
    ClearRequests(Self, UpdateThread.FRequestedList, True); // add 2020-03-07
    ClearRequests(Self, UpdateThread.FTimeOutList, True);

    UpdateThread.SendClearMsg; // 优化微端更新据说不及时 2020-03-14
  end;
end;

function TUpdateEngine.CheckConnect():Boolean;
var
  I:Integer;
  UpdateThread:TUpdateThread;
begin
  Result := False;
  for I := 0 to FUpdateThreadList.Count - 1 do begin
    UpdateThread := FUpdateThreadList.Items[I];
    if UpdateThread.FIsConnect then begin
      Result := True;
      Break;
    end;
  end;
end;

function TUpdateEngine.GetUpdateCount:Integer;
begin
  Result := 0;

  FRequestFileList.Lock;
  try
    Result := Result + FRequestFileList.Count;
  finally
    FRequestFileList.UnLock;
  end;

  FRequestImageList.Lock;
  try
    Result := Result + FRequestImageList.Count;
  finally
    FRequestImageList.UnLock;
  end;
end;

end.

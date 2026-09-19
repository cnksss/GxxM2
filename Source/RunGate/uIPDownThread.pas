unit uIPDownThread;

interface

uses
  Windows, Classes, SysUtils, GateShare, SyncObjs, HUtil32, WinHttp, Grobal2_Ex,
  IniFilesEx, MD5Util;

type
  TDownloadThread = class(TThread)
  private
    FIsRun: Boolean;
    FEvent: TEvent;
    //FWaitEvent: Boolean;
    FInExecuteLoop: Boolean;
    function GetSleeping: Boolean;
  protected
    procedure DoDownload; virtual; abstract;
    procedure Execute; override;
  public
    constructor Create(CreateSuspended: Boolean);
    destructor Destroy; override;
		procedure Terminate; reintroduce; virtual;
    procedure TriggerEvent;
    property Sleeping: Boolean read GetSleeping;
    property IsRun: Boolean read FIsRun;
  end;

  TIPDownThread = class(TDownloadThread)
  private
    FDownUrl: string;
    FBindAddressList: TAddressList;
    FShowText: string;

    procedure OnDownWork(Sender: TObject; FileURL: string; CurrentSize, TotalSize: DWORD; var Breaked: Boolean);
  protected
    procedure DoDownload; override;
  public
    constructor Create(ABindAddressList: TAddressList; AShowText: string); virtual;
    property DownUrl: string read FDownUrl write FDownUrl;
  end;

  TMACDownThread = class(TDownloadThread)
  private
    FDownUrl: string;
    FBindMACList: TSafeHashStringList;
    FShowText: string;

    procedure OnDownWork(Sender: TObject; FileURL: string; CurrentSize, TotalSize: DWORD; var Breaked: Boolean);
  protected
    procedure DoDownload; override;
  public
    constructor Create(ABindMACList: TSafeHashStringList; AShowText: string); virtual;
    property DownUrl: string read FDownUrl write FDownUrl;
  end;

{$IF CLIENT_ANTIPLUG = 1}
  TAntiPlugDownloadFinished = procedure(Sender: TObject; IsUpdateRungateDll, IsUpdateClientDll: Boolean) of object;
  TAntiPlugDownThread = class(TDownloadThread)
  private
    FConfigUrl: string;

    FMemoryStream: TMemoryStream;
    FDownloadFinishedEvent: TAntiPlugDownloadFinished;

    FIsUpdateRungateDll: Boolean;
    FIsUpdateClientDll: Boolean;

    procedure OnDownWork(Sender: TObject; FileURL: string; CurrentSize, TotalSize: DWORD; var Breaked: Boolean);
    procedure DoDownloadFinished;
  protected
    procedure DoDownload; override;
  public
    constructor Create(); virtual;
    destructor Destroy; override;
    procedure SaveGxxRunGateDllStreamToFile(FileName: string);

    property ConfigUrl: string read FConfigUrl write FConfigUrl;
    property OnDownloadFinished: TAntiPlugDownloadFinished read FDownloadFinishedEvent write FDownloadFinishedEvent;
  end;
{$IFEND}

implementation

{ TDownloadThread }

constructor TDownloadThread.Create(CreateSuspended: Boolean);
begin
  inherited Create(CreateSuspended);
  FreeOnTerminate := False;

  FInExecuteLoop := False;

  {
    TEvent.Create
    参数2, Fase 事件对象控制一次后将立即重置(暂停); True 可手动暂停
    参数3, Fase 对象建立后控制为暂停状态; True 可运行状态
  }
  FEvent := TEvent.Create(nil, False, False, '');
  //FWaitEvent := True;
end;

destructor TDownloadThread.Destroy;
begin
  FEvent.Free;
  inherited;
end;

procedure TDownloadThread.Execute;
begin
  FIsRun := True;
  try
    while not Terminated do
    begin
      FEvent.WaitFor(INFINITE);
      //if not FWaitEvent then
      begin
        if Terminated then Exit;

        FInExecuteLoop := True;
        try
          DoDownload;
        finally
          //FWaitEvent := True;
          FInExecuteLoop := False;
        end;
      end;

      //Sleep(1);
    end;
  finally
    FIsRun := False;
  end;
end;

function TDownloadThread.GetSleeping: Boolean;
begin
  Result := not FInExecuteLoop;
end;

procedure TDownloadThread.Terminate;
begin
  inherited Terminate;
  TriggerEvent;
end;

procedure TDownloadThread.TriggerEvent;
begin
  FEvent.SetEvent;
  //FWaitEvent := False;
end;

{ TIPDownThread }

constructor TIPDownThread.Create(ABindAddressList: TAddressList; AShowText: string);
begin
  inherited Create(False);
  FreeOnTerminate := False;
  FBindAddressList := ABindAddressList;
  FShowText := AShowText;
end;

procedure TIPDownThread.DoDownload;
var
  SL: TStringList;
  I: Integer;
  IP: string;
  Http: TWinHttp;
  RequestStatus: TRequestStatus;
begin
  if Length(FDownUrl) > 0 then
  begin
    SL := TStringList.Create;
    Http := TWinHttp.Create;
    try
      Http.OnWork := OnDownWork;
      Http.TimeOut := 3000;
      try
        SL.Text := Http.Get(FDownUrl, RequestStatus);
      except
      end;

      if Terminated then Exit;
      
      if RequestStatus <> rsStreamFinished then
      begin
        AddMainLogMsg(FShowText + '失败', 7);
        Exit;
      end;

      FBindAddressList.Lock;
      try
        FBindAddressList.Clear;
        
        for I := 0 to SL.Count - 1 do
        begin
          if Terminated then Exit;

          IP := Trim(SL.Strings[I]);
          if (Length(IP) > 0) and IsIpaddr(IP) then
            FBindAddressList.Add(IP);
        end;
      finally
        FBindAddressList.UnLock;
      end;

      AddMainLogMsg(FShowText + '成功', 7);
    finally
      Http.Free;
      SL.Free;
    end;
  end;
end;


procedure TIPDownThread.OnDownWork(Sender: TObject; FileURL: string;
  CurrentSize, TotalSize: DWORD; var Breaked: Boolean);
begin
  Breaked := Terminated;
end;


{ TMACDownThread }

constructor TMACDownThread.Create(ABindMACList: TSafeHashStringList; AShowText: string);
begin
  inherited Create(False);
  FreeOnTerminate := False;
  FBindMACList := ABindMACList;
  FShowText := AShowText;
end;

procedure TMACDownThread.DoDownload;
var
  SL: TStringList;
  I: Integer;
  MAC: string;
  Http: TWinHttp;
  RequestStatus: TRequestStatus;
begin
  if Length(FDownUrl) > 0 then
  begin
    SL := TStringList.Create;
    Http := TWinHttp.Create;
    try
      Http.OnWork := OnDownWork;
      Http.TimeOut := 3000;
      try
        SL.Text := Http.Get(FDownUrl, RequestStatus);
      except
      end;

      if Terminated then Exit;
      
      if RequestStatus <> rsStreamFinished then
      begin
        AddMainLogMsg(FShowText + '失败', 7);
        Exit;
      end;

      FBindMACList.Lock;
      try
        FBindMACList.Clear;
        
        for I := 0 to SL.Count - 1 do
        begin
          if Terminated then Exit;

          MAC := Trim(SL.Strings[I]);
          if (Length(MAC) > 0) and (FBindMACList.IndexOf(MAC) < 0) then
            FBindMACList.Add(MAC);
        end;
      finally
        FBindMACList.UnLock;
      end;

      AddMainLogMsg(FShowText + '成功', 7);
    finally
      Http.Free;
      SL.Free;
    end;
  end;
end;

procedure TMACDownThread.OnDownWork(Sender: TObject; FileURL: string;
  CurrentSize, TotalSize: DWORD; var Breaked: Boolean);
begin
  Breaked := Terminated;
end;

{$IF CLIENT_ANTIPLUG = 1}

{ TAntiPlugDownThread }

constructor TAntiPlugDownThread.Create();
begin
  FMemoryStream := TMemoryStream.Create;
  inherited Create(False);
  FreeOnTerminate := False;
end;

destructor TAntiPlugDownThread.Destroy;
begin
  FMemoryStream.Free;
  inherited;
end;

procedure TAntiPlugDownThread.SaveGxxRunGateDllStreamToFile(FileName: string);
begin
  FMemoryStream.SaveToFile(FileName);
end;

procedure TAntiPlugDownThread.DoDownloadFinished;
begin
  if Assigned(FDownloadFinishedEvent) then
    FDownloadFinishedEvent(Self, FIsUpdateRungateDll, FIsUpdateClientDll);
end;

procedure TAntiPlugDownThread.DoDownload;
var
  SL: TStringList;
  Http: TWinHttp;
  RequestStatus: TRequestStatus;
  IniFile: TMemIniFileEx;
  Temp, FileMD5, FileUrl, LocalPlugDatFile: string;
  IsUpdateClientDll, IsUpdateRungateDll: Boolean;
begin
  if Length(FConfigUrl) > 0 then
  begin
    SL := TStringList.Create;
    Http := TWinHttp.Create;
    try
      Http.OnWork := OnDownWork;
      Http.TimeOut := 3000;
      try
        SL.Text := Http.Get(FConfigUrl, RequestStatus);
      except
      end;

      if Terminated then Exit;

      if RequestStatus <> rsStreamFinished then
      begin
        AddMainLogMsg('插件更新配置文件获取失败：' + FConfigUrl, 1);
        Exit;
      end;

      if Terminated then Exit;


      IsUpdateClientDll := False;
      IsUpdateRungateDll := False;

      IniFile := TMemIniFileEx.Create('');
      try
        IniFile.SetStrings(SL);

        FileMD5 := IniFile.ReadString('file', 'md5', '');
        FileUrl := IniFile.ReadString('file', 'file', '');

        if (Length(FileMD5) > 0) and (Length(FileUrl) > 0) then
        begin
          FMemoryStream.Clear;

          LocalPlugDatFile := ExtractFilePath(ParamStr(0)) + 'rungate.dat';

          Temp := RivestFile(LocalPlugDatFile);
          if not SameText(Temp, FileMD5) then
          begin
            RequestStatus := rsReadyToConnect;
            try
              RequestStatus := Http.Get(FileUrl, FMemoryStream);
            except
              AddMainLogMsg('反外挂模块更新失败：' + FileUrl, 0);
            end;

            if RequestStatus = rsStreamFinished then
            begin
              Temp := MD5Print(MD5Memory(FMemoryStream.Memory, FMemoryStream.Size));
              if SameText(Temp, FileMD5) then
              begin
                try
                  FMemoryStream.SaveToFile(LocalPlugDatFile);
                  AddMainLogMsg('反外挂模块更新成功：' + FileUrl, 0);

                  IsUpdateClientDll := True;

                  FMemoryStream.Clear;
                except
                  AddMainLogMsg('反外挂模块更新保存失败', 0);
                end;
              end
              else
              begin
                AddMainLogMsg('反外挂模块下载失败，MD5错误', 0);
              end;
            end;
          end;
        end;

        FileMD5 := IniFile.ReadString('GxxRunGate', 'md5', '');
        FileUrl := IniFile.ReadString('GxxRunGate', 'file', '');

        FMemoryStream.Clear;

        if (Length(FileMD5) > 0) and (Length(FileUrl) > 0) then
        begin
          LocalPlugDatFile := ExtractFilePath(ParamStr(0)) + g_sRunGatePlusDllName;

          Temp := RivestFile(LocalPlugDatFile);
          if not SameText(Temp, FileMD5) then
          begin
            RequestStatus := rsReadyToConnect;
            try
              RequestStatus := Http.Get(FileUrl, FMemoryStream);
            except
              AddMainLogMsg('网关插件更新失败：' + FileUrl, 0);
            end;

            if RequestStatus = rsStreamFinished then
            begin
              Temp := MD5Print(MD5Memory(FMemoryStream.Memory, FMemoryStream.Size));
              if SameText(Temp, FileMD5) then
              begin
                try
                  IsUpdateRungateDll := True;
                  AddMainLogMsg('网关插件更新成功：' + FileUrl, 0);
                except
                  AddMainLogMsg('网关插件更新保存失败', 0);
                end;
              end
              else
              begin
                AddMainLogMsg('网关插件下载失败，MD5错误', 0);
              end;
            end;
          end;
        end;

        // 修改加载顺序，优化加载网关插件 2019-12-16 18:12:45
        if IsUpdateClientDll or IsUpdateRungateDll then
        begin
          FIsUpdateRungateDll := IsUpdateRungateDll;
          FIsUpdateClientDll := IsUpdateClientDll;
          Synchronize(DoDownloadFinished);
        end;
      finally
        IniFile.Free;
      end;
    finally
      FMemoryStream.Clear;

      Http.Free;
      SL.Free;
    end;
  end;
end;

procedure TAntiPlugDownThread.OnDownWork(Sender: TObject; FileURL: string;
  CurrentSize, TotalSize: DWORD; var Breaked: Boolean);
begin
  Breaked := Terminated;
end;

{$IFEND}


end.

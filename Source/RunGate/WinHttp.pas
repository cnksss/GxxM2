(*
 *
 * 单元 : 虫虫 <huan_2004@163.com>
 * 网站 :
 *
 * CHANGES:
 * v1.0 <2010-01-15>
 *   + first release
 *
 * v1.1 <2010-12-13>
 *   + Add property NoCookies
 *   + Add prooerty Charset
 *
 * v1.2 <2010-12-30>
 *   # modify Post(URL: string) to Post(URL: string; AutoDecoding: Boolean = False)
 *
 * v1.3 <2011-1-4>
 *   - delete function CheckIsUtf8Charset
 *   + uses unit WideStrUtils; modify CheckIsUtf8Charset to IsUtf8String
 *
 * v1.4 <2011-1-5>
 *   + add FAgent := 'Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)' on Create;
 *   + auto change FAcceptTypes := 'text/html'; on Post: string; Get: string function
 *
 * v1.5 <2011-1-5>
 *   + add property RedirectUrl; Get real URL after page redirect
 *
 * v1.6 <2011-1-6>
     + add function CheckUrlValid; check input url is valid
     # modify procedure SendRequest to function; and add param OnlyCheckUrlValid
     # modify function Post/Get result type string To AnsiString;
       Compatible with Delphi 2010 (Unicode);
 *
 * v1.7 <2014-7-28>
 *   # support delphi XE6
 *   + add https support
 *)


unit WinHttp;

interface

uses
  Windows, Messages, Forms, SysUtils, Classes, WinInet, Dialogs, DateUtils;
  //WideStrUtils

type
  { 请求状态：连接中，连接成功，开始请求头，返回头，内容中止，内容完成 }
  TRequestStatus = (rsReadyToConnect, rsConnected, rsDoRequest, rsResponseOK, rsStreamBreak, rsStreamFinished);

  { 开始返回数据流 }
  TWorkBeginEvent = procedure(Sender: TObject; FileURL: string; CurrentSize, TotalSize: DWORD) of object;

  { 返回数据流 }
  TWorkEvent = procedure(Sender: TObject; FileURL: string; CurrentSize, TotalSize: DWORD; var Breaked: Boolean) of object;

  TWinHttp = class(TObject)
  private
    FHostName,
    FAction: string;
    FPort: Word;
    FUseCache: Boolean;
    FUserName,
    FPassword,
    FAcceptTypes,
    FAgent,
    FReferer: string;
    FAcceptEncoding: string;
    FAcceptLanguage: string;
    FTimeOut: DWORD;
    FUseCookies: Boolean;

    FRedirectUrl: string;

    FCharset: string;

    { 总数据包大小 }
    FTotalSize: DWORD;

    { 最后修改日期 (断点续传时判断文件是否发生改变) }
    FLastModified: string;

    { 当前返回数据包大小 }
    FResponseSize: DWORD;

    { 当前返回时间 }
    FResponseDateTime: TDateTime;

    { 返回数据包的地址位置 (断点续传时重定位文件位置) }
    FResponseStartRange: DWORD;

    FEndRange: Integer;
    FStartRange: Integer;

    FIsDownloadFile: Boolean;
    FFileStream: TFileStream;
    FLocalFile,
    FLocalFileCfg: string;

    FOnWorkBegin: TWorkBeginEvent;
    FOnWork: TWorkEvent;
  private
    //function  CheckIsUtf8Charset(Html: string): Boolean;
    function DoRequest(URL: string; StreamOut: TStream;
      IsGet: Boolean; OnlyCheckUrlValid: Boolean = False): TRequestStatus;
  private
    procedure SetAcceptTypes(const Value: string);
    procedure SetAgent(const Value: string);
    procedure SetPassword(const Value: string);
    procedure SetReferer(const Value: string);
    procedure SetUseCache(const Value: Boolean);
    procedure SetUserName(const Value: string);

    procedure SetEndRange(const Value: Integer);
    procedure SetStartRange(const Value: Integer);

    procedure AddRequestHeaders(Request: hInternet);
  public
    function CheckUrlValid(URL: string): Boolean;
    function Post(URL: string; AutoDecoding: Boolean = False): string; overload;
    function Get(URL: string; AutoDecoding: Boolean = False): string; overload;

    function Post(URL: string; var RequestStr: string; AutoDecoding: Boolean = False): Boolean; overload;
    function Get(URL: string; var RequestStr: string; AutoDecoding: Boolean = False): Boolean; overload;

    function Get(URL: string; var RequestStatus: TRequestStatus; AutoDecoding: Boolean = False): string; overload;

    function Post(URL: string; var StreamOut: TStream): TRequestStatus; overload;
    function Get(URL: string; StreamOut: TStream): TRequestStatus; overload;

    function DownloadFile(const URL, LocalFile, LocalFileCfg: string): TRequestStatus;
  public
    constructor Create;
    property TimeOut: DWORD read FTimeOut write FTimeOut default 0;
    property HostName: string read FHostName;
    property Action: string read FAction;
    property Port: Word read FPort;

    { request headers }
    property UseCache: Boolean read FUseCache write SetUseCache default False;
    property UseCookies: Boolean read FUseCookies write FUseCookies default True;
    property UserName: string read FUserName write SetUserName;
    property Password: string read FPassword write SetPassword;
    property Agent: string read FAgent write SetAgent;
    property AcceptTypes: string read FAcceptTypes write SetAcceptTypes;
    property AcceptLanguage: string read FAcceptLanguage write FAcceptLanguage;
    property AcceptEncoding: string read FAcceptEncoding write FAcceptEncoding;
    property Referer: string read FReferer write SetReferer;

    { 请求开始位置 (断点续传) }
    property StartRange: Integer read FStartRange write SetStartRange;

    { 请求开始位置 (断点续传) }
    property EndRange: Integer read FEndRange write SetEndRange;

    { 字符集 }
    property Charset: string read FCharset;

    { 重定向后的 URL }
    property RedirectUrl: string read FRedirectUrl;

    { 整个数据包大小 }
    property TotalSize: DWORD read FTotalSize;

    { 最后修改日期 }
    property LastModified: string read FLastModified;

    { 当前返回的大小 (请求时带Range会只返回部分) }
    property ResponseSize: DWORD read FResponseSize;

    property ResponseDateTime: TDateTime read FResponseDateTime;

    { 返回数据包的起始位置 }
    property ResponseStartRange: DWORD read FResponseStartRange;

    property OnWorkBegin: TWorkBeginEvent read FOnWorkBegin write FOnWorkBegin;

    { 数据包返回事件 }
    property OnWork: TWorkEvent read FOnWork write FOnWork;
  end;

implementation

{ TWinHttp }

procedure ParseURL(Url: string; var IsHttps: Boolean; var HostName, UrlPath: string; var Port: Word);
const
  ArrStrDel: array[0..1] of string = ('http://', 'https://');
var
  Index, Len: Integer;
  StrPort: string;
  StrProtocol: string;
begin
  StrProtocol := 'http://';
  for Index := Low(ArrStrDel) to High(ArrStrDel) do
  begin
    Len := Length(ArrStrDel[Index]);
    StrProtocol := Copy(Url, 1, Len);
    if SameText(StrProtocol, ArrStrDel[Index]) then
    begin
      System.Delete(Url, 1, Len);
      Break;
    end;
  end;
  
  Index := Pos('/', Url);
  if Index = 0 then
  begin
    HostName := Url;
    UrlPath := '';
  end
  else
  begin
    HostName := Copy(Url, 1, Index - 1);
    UrlPath  := Copy(Url, Index, Length(Url) - Index + 1);
  end;

  if SameText(StrProtocol, 'https://') then
  begin
    IsHttps := True;
    Port := INTERNET_DEFAULT_HTTPS_PORT;
  end
  else
  begin
    IsHttps := False;
    Port := INTERNET_DEFAULT_HTTP_PORT;
  end;

  if (Length(HostName) > 0) then
  begin
    Index := Pos(':', HostName);
    if Index > 0 then
    begin
      StrPort  := Copy(HostName, Index + 1, MAXINT);
      HostName := Copy(HostName, 1, Index - 1);
      if Length(StrPort) > 0 then
        Port := StrToIntDef(StrPort, Port);
    end;
  end;
end;

function IsUTF8String(const S: UTF8String): Boolean;
var
  C: AnsiChar;
  P, EndPtr: PAnsiChar;
begin
  Result := False;
  P := PAnsiChar(S);
  EndPtr := P + Length(S);

  // skip leading US-ASCII part.
  while P < EndPtr do
  begin
    if P^ >= #$80 then break;
    Inc(P);
  end;

  // If all character is US-ASCII, done.
  if P = EndPtr then exit;

  while P < EndPtr do
  begin
    C := p^;
    case C of
      #$00..#$7F:
        Inc(P);

      #$C2..#$DF:
        if (P+1 < EndPtr)
            and ((P+1)^ in [#$80..#$BF]) then
          Inc(P, 2)
        else
          break;

      #$E0:
        if (P+2 < EndPtr)
            and ((P+1)^ in [#$A0..#$BF])
            and ((P+2)^ in [#$80..#$BF]) then
          Inc(P, 3)
        else
          break;

      #$E1..#$EF:
        if (P+2 < EndPtr)
            and ((P+1)^ in [#$80..#$BF])
            and ((P+2)^ in [#$80..#$BF]) then
          Inc(P, 3)
        else
          break;

      #$F0:
        if (P+3 < EndPtr)
            and ((P+1)^ in [#$90..#$BF])
            and ((P+2)^ in [#$80..#$BF])
            and ((P+3)^ in [#$80..#$BF]) then
          Inc(P, 4)
        else
          break;

      #$F1..#$F3:
        if (P+3 < EndPtr)
            and ((P+1)^ in [#$80..#$BF])
            and ((P+2)^ in [#$80..#$BF])
            and ((P+3)^ in [#$80..#$BF]) then
          Inc(P, 4)
        else
          break;

      #$F4:
        if (P+3 < EndPtr)
            and ((P+1)^ in [#$80..#$8F])
            and ((P+2)^ in [#$80..#$BF])
            and ((P+3)^ in [#$80..#$BF]) then
          Inc(P, 4)
        else
          break;
    else
      break;
    end;
  end;

  if P = EndPtr then
    Result := True
  else
    Result := False;
end;

{
type
  LPINTERNET_ASYNC_RESULT = ^INTERNET_ASYNC_RESULT;
  INTERNET_ASYNC_RESULT = record
    dwResult: DWORD;
    dwError: DWORD;
  end;
}
procedure InternetCallback(hInternet: HINTERNET; dwContext: DWORD;
  dwInternetStatus: DWORD; lpvStatusInformation: Pointer;
  dwStatusInformationLength: DWORD); stdcall;
begin
  // document: http://msdn.microsoft.com/en-us/library/aa383917%28v=vs.85%29.aspx
  if (dwInternetStatus = INTERNET_STATUS_REDIRECT) and
    (lpvStatusInformation <> nil) then
  begin
    TWinHttp(dwContext).FRedirectUrl := PChar(lpvStatusInformation);
  end;
end;

procedure TWinHttp.AddRequestHeaders(Request: hInternet);
var
  Header: string;
begin
  if Length(FAcceptEncoding) > 0 then
  begin
    Header := 'Accept-Encoding:' + FAcceptEncoding;
    HttpAddRequestHeaders(Request, PChar(Header), Length(Header), HTTP_ADDREQ_FLAG_REPLACE or HTTP_ADDREQ_FLAG_ADD);
  end;

  if Length(FAcceptLanguage) > 0 then
  begin
    Header := 'Accept-Language:' + FAcceptLanguage;
    HttpAddRequestHeaders(Request, PChar(Header), Length(Header), HTTP_ADDREQ_FLAG_REPLACE or HTTP_ADDREQ_FLAG_ADD);
  end;

  if (FStartRange > 0) or (FEndRange > 0) then
  begin
    if FEndRange >= FStartRange then
      Header := 'Range: bytes=' + IntToStr(FStartRange) + '-' + IntToStr(FEndRange)
    else
      Header := 'Range: bytes=' + IntToStr(FStartRange) + '-';
    HttpAddRequestHeaders(Request, PChar(Header), Length(Header), HTTP_ADDREQ_FLAG_REPLACE or HTTP_ADDREQ_FLAG_ADD);
  end;
end;

constructor TWinHttp.Create;
begin
  FUseCookies := True;
  FUseCache := False;
  FAcceptTypes := 'text/html, */*';
  FAgent := 'Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)';
  FTimeOut := 0;

  FStartRange := 0;
  FEndRange := 0;
end;

function TWinHttp.DoRequest(URL: string; StreamOut: TStream;
  IsGet: Boolean; OnlyCheckUrlValid: Boolean = False): TRequestStatus;
const
  HEADER_BUFFER_SIZE = 4096;
var
  hSession, hConnect, hRequest: hInternet;
  RequestMethod: PChar;
  InternetFlag: DWORD;

  Buffer: array[0..HEADER_BUFFER_SIZE - 1] of Byte;
  AcceptType: array of PChar;
  Index: Integer;
  S, sPage, sParams: string;
  Breaked: Boolean;
  BytesRead: DWORD;
  StatusCode: Integer;
  IsHttps: Boolean;

  SysTime: SYSTEMTIME;
  dwBufLen, dwReseved: Cardinal;

  Cfgs: TStringList;

  procedure CloseHandles;
  begin
    if Assigned(hRequest) then
      InternetCloseHandle(hRequest);
    if Assigned(hSession) then
      InternetCloseHandle(hSession);
    if Assigned(hConnect) then
      InternetCloseHandle(hConnect);
  end;

  function QueryHttpHeader(Flag: DWORD): Boolean;
  var
    dwBufLen, dwIndex: DWord;
  begin
    dwIndex := 0;
    dwBufLen := HEADER_BUFFER_SIZE;
    Result := HttpQueryInfo(hRequest, Flag, @Buffer, dwBufLen, dwIndex) and (dwBufLen > 0);
    if Result then
    begin
      Buffer[dwBufLen] := 0;
      Buffer[dwBufLen + 1] := 0;
    end;
  end;
begin
  Result := rsReadyToConnect;
  FCharset := '';
  FTotalSize := 0;
  FRedirectUrl := '';
  
  ParseURL(URL, IsHttps, FHostName, FAction, FPort);
  Index := Pos('?', FAction);
  if Index = 0 then
    sPage := FAction
  else
  begin
    sPage := Copy(FAction, 1, Index - 1);
    sParams := Copy(FAction, Index + 1, MAXINT);
  end;

  if FAgent <> '' then
    hSession := InternetOpen(PChar(FAgent), INTERNET_OPEN_TYPE_PRECONFIG, nil, nil, 0)
  else
    hSession := InternetOpen(nil, INTERNET_OPEN_TYPE_PRECONFIG, nil, nil, 0);
  if not Assigned(hSession) then Exit;

  // 添加回调函数, 主要用于当重定向时取得重定向的地址
  InternetSetStatusCallback(hSession, INTERNET_STATUS_CALLBACK(@InternetCallback));
  
  if FTimeOut <> 0 then
  begin
    InternetSetOption(hSession, INTERNET_OPTION_CONNECT_TIMEOUT, Pointer(@FTimeOut), SizeOf(FTimeOut));
    InternetSetOption(hSession, INTERNET_OPTION_RECEIVE_TIMEOUT, Pointer(@FTimeOut), SizeOf(FTimeOut));
    InternetSetOption(hSession, INTERNET_OPTION_DATA_RECEIVE_TIMEOUT, Pointer(@FTimeOut), SizeOf(FTimeOut));
    InternetSetOption(hSession, INTERNET_OPTION_SEND_TIMEOUT, Pointer(@FTimeOut), SizeOf(FTimeOut));
    InternetSetOption(hSession, INTERNET_OPTION_DATA_SEND_TIMEOUT, Pointer(@FTimeOut), SizeOf(FTimeOut));
  end;
  
  hConnect := InternetConnect(hSession, PChar(FHostName), FPort,
    PChar(FUserName), PChar(FPassword), INTERNET_SERVICE_HTTP, 0, 0);
  if not Assigned(hConnect) then
  begin
    CloseHandles;
    Exit;
  end;

  Result := rsConnected;
  
  if IsGet then
    RequestMethod := 'GET'
  else
    RequestMethod := 'POST';

  InternetFlag := 0;

  if not FUseCache then
    InternetFlag := INTERNET_FLAG_RELOAD;

  if not FUseCookies then
    InternetFlag := InternetFlag or INTERNET_FLAG_NO_COOKIES;

  if IsHttps then
  begin
    InternetFlag := InternetFlag or INTERNET_FLAG_NO_COOKIES or
      INTERNET_FLAG_SECURE or                   // https连接
      INTERNET_FLAG_IGNORE_CERT_CN_INVALID or   // 忽略因服务器的证书主机名与请求的主机名不匹配所导致的错误
      INTERNET_FLAG_IGNORE_CERT_DATE_INVALID;   // 忽略由已失效的服务器证书导致的错误
  end;

  SetLength(AcceptType, 2);
  AcceptType[0] := PChar(FAcceptTypes);  { Do not localize }
  AcceptType[1] := nil;

  if IsGet then
  begin
    hRequest := HttpOpenRequest(hConnect, RequestMethod, PChar(FAction), HTTP_VERSION,
                PChar(FReferer), Pointer(AcceptType), InternetFlag, Cardinal(Self));  // old value 0
    if not Assigned(hRequest) then
    begin
      CloseHandles;
      Exit;
    end;

    AddRequestHeaders(hRequest);
    HttpSendRequest(hRequest, nil, 0, nil, 0);
  end
  else
  begin
    hRequest := HttpOpenRequest(hConnect, RequestMethod, PChar(sPage), HTTP_VERSION,
                PChar(FReferer), Pointer(AcceptType), InternetFlag, Cardinal(Self));        // old value 0
    if not Assigned(hRequest) then
    begin
      CloseHandles;
      Exit;
    end;

    AddRequestHeaders(hRequest);
    HttpAddRequestHeaders(hRequest, 'Content-Type: application/x-www-form-urlencoded', 47,  // 47 is Length
      HTTP_ADDREQ_FLAG_REPLACE or HTTP_ADDREQ_FLAG_ADD);
    HttpSendRequest(hRequest, nil, 0, PChar(sParams), Length(sParams));
  end;

  Result := rsDoRequest;

  {  关于 HttpQueryInfo 返回值的说明: 如果有查询的 Response.headers 条目,
     则返回 True, 否则返回 False; }
  // 获取返回代码
  if not QueryHttpHeader(HTTP_QUERY_STATUS_CODE) then
  begin
    CloseHandles;
    Exit;
  end;

  { 检查返回状态; (200 表示返回成功) }
  StatusCode := StrToIntDef(StrPas(PChar(@Buffer)), 0);
  if (StatusCode <> HTTP_STATUS_OK) and (StatusCode <> HTTP_STATUS_PARTIAL_CONTENT) then
  begin
    CloseHandles;
    Exit;
  end;

  Result := rsResponseOK;

  if OnlyCheckUrlValid then
  begin
    CloseHandles;
    Exit;
  end;

  // 取最后修改日期
  if QueryHttpHeader(HTTP_QUERY_LAST_MODIFIED) then
    FLastModified := PChar(@Buffer);

  // 获取返回的数据长度
  // 不要太信赖这个值; Content-Length 有时没有 www.163.com
  FTotalSize := 0;
  FResponseSize := 0;
  FResponseStartRange := 0;
  FResponseDateTime := 0;

  if QueryHttpHeader(HTTP_QUERY_CONTENT_RANGE) then
  begin
    // bytes 2-3/5254256
    S := LowerCase(PChar(@Buffer));
    if Pos('bytes ', S) = 1 then
    begin
      S := Copy(S, 7, MAXINT);
      Index := Pos('-', S);
      if Index > 0 then
        FResponseStartRange := StrToIntDef(Copy(S, 0, Index - 1), 0);

      Index := Pos('/', S);
      if Index > 0 then
        FTotalSize := StrToIntDef(Copy(S, Index + 1, MAXINT), 0);
    end;

    if QueryHttpHeader(HTTP_QUERY_CONTENT_LENGTH) then
    begin
      FResponseSize := StrToIntDef(StrPas(PChar(@Buffer)), 0);
    end;
  end
  else if QueryHttpHeader(HTTP_QUERY_CONTENT_LENGTH) then
  begin
    FTotalSize := StrToIntDef(StrPas(PChar(@Buffer)), 0);
    FResponseSize := FTotalSize;
  end;

  dwBufLen := SizeOf(SysTime);
  if HttpQueryInfo(hRequest, HTTP_QUERY_DATE or HTTP_QUERY_FLAG_SYSTEMTIME, @SysTime, dwBufLen, dwReseved) then
  begin
    FResponseDateTime := IncHour(EncodeDateTime(SysTime.wYear, SysTime.wMonth, SysTime.wDay, SysTime.wHour, SysTime.wMinute, SysTime.wSecond, SysTime.wMilliseconds), 8);
  end;

  // 获取内容类型及编码 Content-Type:text/html; charset=utf-8
  if QueryHttpHeader(HTTP_QUERY_CONTENT_TYPE) then
  begin
    FCharset := LowerCase(StrPas(PChar(@Buffer)));
    Index := Pos('charset=', FCharset);
    if Index <> 0 then
    begin
      FCharset := Copy(FCharset, Index + 8, MAXINT);
      Index := Pos(';', FCharset);
      if Index > 0 then
        FCharset := Copy(FCharset, 1, Index - 1);
    end;
  end;

{
  // 读取 cookies
  if QueryHttpHeader(HTTP_QUERY_COOKIE) then
  begin
    P := @Buffer;
    ShowMessage(P);
  end;
}
  { 返回头已经全部取到 }
  if FIsDownloadFile then
  begin
    { 先判断断点文件大小和返回起始位置是否相符 }
    if (StreamOut.Size <> FResponseStartRange) then
    begin
      CloseHandles;
      Exit;
    end;

    { 判断最后修改时间是否相符 }
    if (FResponseStartRange > 0) then
    begin
      Cfgs := TStringList.Create;
      try
        if FileExists(FLocalFileCfg) then
          Cfgs.LoadFromFile(FLocalFileCfg);

        if (Cfgs.Count = 0) or (not SameText(Cfgs[0], FLastModified)) then
        begin
          CloseHandles;
          Exit;
        end;
      finally
        Cfgs.Free;
      end;
    end;

    { 在首次返回的时候，写上 "最后修改时间" }
    if FResponseStartRange = 0 then
    begin
      Cfgs := TStringList.Create;
      try
        if FileExists(FLocalFileCfg) then
          Cfgs.LoadFromFile(FLocalFileCfg);

        if (Cfgs.Count = 0) or (not SameText(Cfgs[0], FLastModified)) then
        begin
          Cfgs.Clear;
          Cfgs.Add(FLastModified);
          Cfgs.SaveToFile(FLocalFileCfg);
        end;
      finally
        Cfgs.Free;
      end;
    end;
  end;

  if Assigned(FOnWorkBegin) then
    FOnWorkBegin(Self, URL, StreamOut.Size, FTotalSize);

  // 获取数据流
  Breaked := False;
  while True do
  begin
    BytesRead := SizeOf(Buffer);
    if (not InternetReadFile(hRequest, @Buffer, SizeOf(Buffer),
      BytesRead)) or (BytesRead = 0) then Break;

    StreamOut.Write(Buffer, BytesRead);
    if Assigned(FOnWork) then
      FOnWork(Self, URL, StreamOut.Size, FTotalSize, Breaked);

    if Breaked then Break;
  end;

  if Breaked then
    Result := rsStreamBreak
  else
    Result := rsStreamFinished;

  CloseHandles;
end;

function TWinHttp.CheckUrlValid(URL: string): Boolean;
begin
  FIsDownloadFile := False;
  Result := DoRequest(URL, nil, True, True) = rsResponseOK;
end;

function TWinHttp.Post(URL: string; AutoDecoding: Boolean): string;
begin
  Result := '';
  Post(URL, Result, AutoDecoding);
end;

function TWinHttp.Get(URL: string; AutoDecoding: Boolean): string;
begin
  Result := '';
  Get(URL, Result, AutoDecoding);
end;

function TWinHttp.Post(URL: string; var RequestStr: string; AutoDecoding: Boolean = False): Boolean;
var
  MS : TMemoryStream;
  AcceptTypesBack: string;
  StrResult: AnsiString;
begin
  FIsDownloadFile := False;
  MS := TMemoryStream.Create;
  try
    AcceptTypesBack := FAcceptTypes;
    //FAcceptTypes := 'text/html,*/*';
    Result := DoRequest(Url, MS, False) = rsStreamFinished;
    FAcceptTypes := AcceptTypesBack;
    if not Result then Exit;

    SetLength(StrResult, MS.Size);
    Move(MS.Memory^, StrResult[1], MS.Size);

    if (not AutoDecoding) or (Length(StrResult) = 0) or (not IsUTF8String(StrResult)) then
      RequestStr := StrResult
    else
      RequestStr := Utf8Decode(StrResult);
  finally
    MS.Free;
  end;
end;

function TWinHttp.Get(URL: string; var RequestStr: string; AutoDecoding: Boolean = False): Boolean;
var
  MS : TMemoryStream;
  AcceptTypesBack: string;
  StrResult: AnsiString;
begin
  MS := TMemoryStream.Create;
  try
    AcceptTypesBack := FAcceptTypes;
    //FAcceptTypes := 'text/html,*/*';
    Result := DoRequest(Url, MS, True) = rsStreamFinished;
    FAcceptTypes := AcceptTypesBack;
    if not Result then Exit;

    SetLength(StrResult, MS.Size);
    Move(MS.Memory^, StrResult[1], MS.Size);

    if (not AutoDecoding) or (Length(StrResult) = 0) or (not IsUTF8String(StrResult)) then
      RequestStr := StrResult
    else
      RequestStr := Utf8Decode(StrResult);
  finally
    MS.Free;
  end;
end;

function TWinHttp.Get(URL: string; var RequestStatus: TRequestStatus; AutoDecoding: Boolean = False): string;
var
  MS : TMemoryStream;
  AcceptTypesBack: string;
  StrResult: AnsiString;
begin
  Result := '';
  MS := TMemoryStream.Create;
  try
    AcceptTypesBack := FAcceptTypes;
    //FAcceptTypes := 'text/html,*/*';
    RequestStatus := DoRequest(Url, MS, True);
    if RequestStatus = rsStreamFinished then
    begin
      FAcceptTypes := AcceptTypesBack;
      
      SetLength(StrResult, MS.Size);
      Move(MS.Memory^, StrResult[1], MS.Size);

      if (not AutoDecoding) or (Length(StrResult) = 0) or (not IsUTF8String(StrResult)) then
        Result := StrResult
      else
        Result := Utf8Decode(StrResult);
    end;
  finally
    MS.Free;
  end;
end;


function TWinHttp.Post(URL: string; var StreamOut: TStream): TRequestStatus;
begin
  FIsDownloadFile := False;
  Result := DoRequest(Url, StreamOut, False);
end;

function TWinHttp.Get(URL: string; StreamOut: TStream): TRequestStatus;
begin
  FIsDownloadFile := False;
  Result := DoRequest(Url, StreamOut, True);
end;

procedure TWinHttp.SetAcceptTypes(const Value: string);
begin
  FAcceptTypes := Value;
end;

procedure TWinHttp.SetAgent(const Value: string);
begin
  FAgent := Value;
end;

procedure TWinHttp.SetPassword(const Value: string);
begin
  FPassword := Value;
end;

procedure TWinHttp.SetReferer(const Value: string);
begin
  FReferer := Value;
end;

procedure TWinHttp.SetUseCache(const Value: Boolean);
begin
  FUseCache := Value;
end;

procedure TWinHttp.SetUserName(const Value: string);
begin
  FUserName := Value;
end;

procedure TWinHttp.SetEndRange(const Value: Integer);
begin
  FEndRange := Value;
end;

procedure TWinHttp.SetStartRange(const Value: Integer);
begin
  FStartRange := Value;
end;

function TWinHttp.DownloadFile(const URL, LocalFile, LocalFileCfg: string): TRequestStatus;
var
  SL: TStringList;
  FileMode: DWORD;

  ResultSize: DWORD;
  RequestStatus: TRequestStatus;
begin
  FIsDownloadFile := True;
  FLocalFile := LocalFile;
  FLocalFileCfg := LocalFileCfg;
  FileMode := fmCreate;
  if FileExists(FLocalFileCfg) and FileExists(LocalFile) then
  begin
    SL := TStringList.Create;
    try
      SL.LoadFromFile(FLocalFileCfg);
      if (SL.Count > 0) and (Length(SL[0]) > 0) then FileMode := fmOpenReadWrite;
    finally
      SL.Free;
    end;
  end;

  FEndRange := 0;

  // 先指定 FStartRange，从断点处开始下载
  FFileStream := TFileStream.Create(LocalFile, FileMode);
  try
    FStartRange := FFileStream.Size;
    FFileStream.Seek(0, soEnd);
    RequestStatus := DoRequest(URL, FFileStream, True);
    ResultSize := FFileStream.Size;
  finally
    FFileStream.Free;
  end;

  // 如果查询到不能从断点处开始下载，则重头开始下载
  if RequestStatus = rsResponseOK then
  begin
    FStartRange := 0;
    FFileStream := TFileStream.Create(LocalFile, fmCreate);
    try
      RequestStatus := DoRequest(URL, FFileStream, True);
      ResultSize := FFileStream.Size;
    finally
      FFileStream.Free;
    end;
  end;

  if RequestStatus = rsStreamFinished then
  begin
    if (FTotalSize <> 0) and (FTotalSize <> ResultSize) then
      RequestStatus := rsStreamBreak;
  end;

  Result := RequestStatus;
end;

{
function TWinHttp.CheckIsUtf8Charset(Html: string): Boolean;
var
  I, IdxStart, IdxEnd, IdxTemp: Integer;
  WS, Meta, WSTemp: WideString;
  StrCharset: string;
  PW: PWideChar;
begin
  Result := False;

  // check first characters is utf8's bom
  if (Length(Html) >= 3) and (Html[1] = #$EF) and (Html[2] = #$BB)
    and (Html[3] = #$BF) then
  begin
    Result := True;
    Exit;
  end;

  // check charset from response.headers
  if SameText(FCharset, 'utf-8') then
  begin
    Result := True;
    Exit;
  end;

  // if not request response.headers.charset, read charset from html
  if Length(FCharset) <> 0 then Exit;
  WS := LowerCase(Html);
  PW := @(WS[1]);

  // 下面的代码用于获取html内容的charset; 如果用正则表达式,代码会简单很多
  IdxStart := Pos(WideString('<meta'), PW);
  while IdxStart > 0 do
  begin
    Inc(PW, IdxStart - 1);

    // get end tag > postion
    IdxEnd := Pos(WideString('>'), PW);
    if IdxEnd = 0 then Exit;

    // get Meta lines text
    Meta := Copy(PW, 1, IdxEnd);

    if Pos(WideString('content-type'), Meta) > 0 then
    begin
      IdxTemp := Pos(WideString('charset'), Meta);
      if IdxTemp = 0 then Exit;
      WSTemp := Copy(Meta, IdxTemp, MAXINT);

      IdxTemp := Pos(WideString('='), WSTemp);
      if IdxTemp = 0 then Exit;
      WSTemp := Trim(Copy(WSTemp, IdxTemp + 1, MAXINT));

      StrCharset := WSTemp;
      for I := 1 to Length(StrCharset) do
      begin
        if not (StrCharset[I] in ['a'..'z', 'A'..'Z', '0'..'9', '-']) then
        begin
          StrCharset := Copy(StrCharset, 1, I - 1);
          Break;
        end;
      end;

      Result := SameText(StrCharset, 'utf-8');
      Exit;
    end;

    Inc(PW, IdxEnd + 1);
    IdxStart := Pos(WideString('<meta'), PW);
  end;
end;
}

end.

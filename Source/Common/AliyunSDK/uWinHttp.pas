(*
 *
 * µ¥Ôª : ChenHuan <huan_2004@163.com>
 * ÍøÕ¾ :
 *
 * CHANGES:
 * v1.0 <2010-01-15>
 *   + first release
 *)

 
unit uWinHttp;

interface

uses
  Windows, Messages, SysUtils, Classes, WinInet, Dialogs;

type
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
  private
    procedure SendRequest(URL: string; StreamOut: TStream; IsGet: Boolean);
  private
    procedure SetAcceptTypes(const Value: string);
    procedure SetAgent(const Value: string);
    procedure SetPassword(const Value: string);
    procedure SetReferer(const Value: string);
    procedure SetUseCache(const Value: Boolean);
    procedure SetUserName(const Value: string);
  public
    function  Post(URL: string): string; overload;
    function  Get(URL: string): string; overload;
    procedure Post(URL: string; var StreamOut: TStream); overload;
    procedure Get(URL: string; StreamOut: TStream); overload;
  public
    constructor Create;
    property HostName: string read FHostName;
    property Action: string read FAction;
    property Port: Word read FPort;
    property UseCache: Boolean read FUseCache write SetUseCache;
    property UserName: string read FUserName write SetUserName;
    property Password: string read FPassword write SetPassword;
    property AcceptTypes: string read FAcceptTypes write SetAcceptTypes;
    property Agent: string read FAgent write SetAgent;
    property Referer: string read FReferer write SetReferer;
  end;

implementation

{ TWinHttp }

procedure ParseURL(Url: string; var HostName, UrlPath: string; var Port: Word);
const
  ArrStrDel: array[0..1] of string = ('http://', 'https://');
var
  Index, Len: Integer;
  StrPort: string;
  strDel: string;
begin
  for Index := Low(ArrStrDel) to High(ArrStrDel) do
  begin
    Len := Length(ArrStrDel[Index]);
    strDel := Copy(Url, 1, Len);
    if CompareText(strDel, ArrStrDel[Index]) = 0 then
    begin
      System.Delete(Url, 1, Len);
      Break;
    end;
  end;

  Index := Pos('/', Url);
  HostName := Copy(Url, 1, Index - 1);
  UrlPath  := Copy(Url, Index, Length(Url) - Index + 1);
  Port := 80;

  if (Length(HostName) > 0) then
  begin
    Index := Pos(':', HostName);
    if Index > 0 then
    begin
      StrPort  := Copy(HostName, Index + 1, MAXINT);
      HostName := Copy(HostName, 1, Index - 1);
      if Length(StrPort) > 0 then
      try
        Port := StrToInt(StrPort);
      except
        Port := 80;
      end;
    end;
  end;
end;

constructor TWinHttp.Create;
begin
  FUseCache := False;
  FAcceptTypes := 'text/html, */*';
end;

procedure TWinHttp.SendRequest(URL: string; StreamOut: TStream; IsGet: Boolean);
var
  hSession, hConnect, hRequest: hInternet;
  RequestMethod: PChar;
  InternetFlag: DWORD;
  Reserved, BytesRead: DWORD;

  Buffer: Array[0..$0FFF] of Byte;
  AcceptType: array of PChar;
begin
  ParseURL(URL, FHostName, FAction, FPort);

  if FAgent <> '' then
    hSession := InternetOpen(PChar(FAgent), INTERNET_OPEN_TYPE_PRECONFIG, nil, nil, 0)
  else
    hSession := InternetOpen(nil, INTERNET_OPEN_TYPE_PRECONFIG, nil, nil, 0);

  hConnect := InternetConnect(hSession, PChar(FHostName), FPort,
    PChar(FUserName), PChar(FPassword),
    INTERNET_SERVICE_HTTP, 0, 0);

  if IsGet then
    RequestMethod := 'GET'
  else
    RequestMethod := 'POST';

  if FUseCache then
    InternetFlag := 0
  else
    InternetFlag := INTERNET_FLAG_RELOAD;

  SetLength(AcceptType, 2);
  AcceptType[0] := PChar(FAcceptTypes);  { Do not localize }
  AcceptType[1] := nil;

  hRequest := HttpOpenRequest(hConnect, RequestMethod, PChar(FAction), 'HTTP/1.0',
              PChar(FReferer), Pointer(AcceptType), InternetFlag, 0);

  if IsGet then
    HttpSendRequest(hRequest, nil, 0, nil, 0)
  else
    HttpSendRequest(hRequest, 'Content-Type: application/x-www-form-urlencoded',
      47, nil, 0);

  HttpQueryInfo(hRequest, HTTP_QUERY_CONTENT_LENGTH, @Buffer, BytesRead, Reserved);
  repeat
    if not InternetReadFile( hRequest, @Buffer, SizeOf(Buffer), BytesRead) then
      Break;
    StreamOut.Write(Buffer, BytesRead);
  until BytesRead = 0;

  InternetCloseHandle(hRequest);
  InternetCloseHandle(hSession);
  InternetCloseHandle(hConnect);
end;

procedure TWinHttp.Get(URL: string; StreamOut: TStream);
begin
  SendRequest(Url, StreamOut, True);
end;

function TWinHttp.Get(URL: string): string;
var
  StringStream : TStringStream;
begin
  StringStream := TStringStream.Create('');
  SendRequest(Url, StringStream, True);
  Result := StringStream.DataString;
  StringStream.Free;
end;

function TWinHttp.Post(URL: string): string;
var
  StringStream : TStringStream;
begin
  StringStream := TStringStream.Create('');
  SendRequest(Url, StringStream, False);
  Result := StringStream.DataString;
  StringStream.Free;
end;

procedure TWinHttp.Post(URL: string; var StreamOut: TStream);
begin
  SendRequest(Url, StreamOut, False);
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

end.

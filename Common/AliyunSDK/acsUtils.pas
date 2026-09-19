(*
 *
 * 单元 : chongchong
 * 网站 :
 *
 * 说明 : 阿里云公共类
 *
 * CHANGES:
 * v1.0 <2017-04-21>
 *   + first release
 *
 *)

unit acsUtils;

interface

uses
  Windows, SysUtils, Classes, ShellAPI, acsCommon, uWinHttp, Dialogs;

type
  TAcsUtil = class(TObject)
  private
    FHttp: TWinHttp;
    FKeySecret: string;
    FKeyID: string;

    procedure SetKeyID(const Value: string);
    procedure SetKeySecret(const Value: string);
  public
    constructor Create;
    destructor Destroy; override;
  public

    { 申请淘宝应用时得到的Key }
    property KeyID: string read FKeyID write SetKeyID;

    { 申请淘宝应用时得到的密钥 }
    property KeySecret: string read FKeySecret write SetKeySecret;

    { 得到API服务的URL地址, 此函数返回依赖于TestMode的值 }
    function GetApiURL: string;

    { 发送Post请求,并返回字符串 }
    function  HttpPost(URL: string): string; overload;

    { 发送Get请求,并返回字符串 }
    function  HttpGet(URL: string): string; overload;

    { 发送Post请求,将结果返回到流中 }
    procedure HttpPost(URL: string; var StreamOut: TStream); overload;

    { 发送Get请求,将结果返回到流中 }
    procedure HttpGet(URL: string; StreamOut: TStream); overload;
  end;

var
  g_AcsUtil: TAcsUtil;

implementation

{ TTaobaoUtil }

constructor TAcsUtil.Create;
begin
  { 淘宝日期之间用-分隔 }
  {$IF CompilerVersion >= 22}FormatSettings.{$IFEND}DateSeparator := '-';

  FHttp := TWinHttp.Create;
end;

destructor TAcsUtil.Destroy;
begin
  FHttp.Free;
  inherited;
end;

function TAcsUtil.GetApiURL: string;
begin
  Result := 'https://dysmsapi.aliyuncs.com/?';
end;

procedure TAcsUtil.HttpGet(URL: string; StreamOut: TStream);
begin
  FHttp.Get(URL, StreamOut);
end;

function TAcsUtil.HttpGet(URL: string): string;
begin
  Result := FHttp.Get(URL);
end;

procedure TAcsUtil.HttpPost(URL: string; var StreamOut: TStream);
begin
  FHttp.Post(URL, StreamOut);
end;

function TAcsUtil.HttpPost(URL: string): string;
begin
  Result := FHttp.Post(URL)
end;

procedure TAcsUtil.SetKeyID(const Value: string);
begin
  FKeyID := Value;
end;

procedure TAcsUtil.SetKeySecret(const Value: string);
begin
  FKeySecret := Value;
end;

initialization
  if not Assigned(g_AcsUtil) then
    g_AcsUtil := TAcsUtil.Create;

finalization
  if Assigned(g_AcsUtil) then
    FreeAndNil(g_AcsUtil);

end.

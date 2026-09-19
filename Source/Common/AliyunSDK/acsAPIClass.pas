(*
 *
 * 单元 : chongchong
 * 网站 :
 *
 * 说明 : 阿里云API基类
 *
 * CHANGES:
 *
 *
 * v1.0 <2017-04-21>
 *   + first release
 *
 *)

unit acsAPIClass;

interface

uses
  Windows, SysUtils, Classes, Dialogs, acsCommon, acsParams, acsError,
  uWinHttp, acsUtils;


type
  TAcsRequestAPI = class(TObject)
  private
    FParams: TAcsParams;

    //procedure SetFormatType(const Value: TFormatType);
    //procedure SetVersion(const Value: string);
    //procedure SetActionName(const Value: string);
  protected
    FVersion: string;
    FFormatType: TFormatType;
    FActionName: string;

    { 向淘宝API服务器发送一个请求 }
    function DoRequest(Method: TRequestMethod = rmGet): string;

    { 添加一个参数到参数列表中 }
    procedure DoAddParam(Key, Value: string); virtual;
  public
    constructor Create; virtual;
    destructor Destroy; override;
    function GetParam(Key: string): string;
  end;

implementation

{ TTabaoApi }

constructor TAcsRequestAPI.Create;
begin
  FParams := TAcsParams.Create;

  FActionName := '';

  FVersion := '2017-05-25';
  FFormatType := ftJson;
end;

destructor TAcsRequestAPI.Destroy;
begin
  FParams.Free;
  inherited;
end;

function NowUTC: TDateTime;
var
  system_datetime: TSystemTime;
begin
  GetSystemTime(system_datetime);
  Result := SystemTimeToDateTime(system_datetime);
end;

function TAcsRequestAPI.DoRequest(Method: TRequestMethod): string;
const
  StrFormat: array[TFormatType] of string = ('XML', 'JSON');
var
  StrURL: string;
begin
  if Length(FActionName) = 0 then
  begin
    raise Exception.Create('ActionName属性设置错误');
  end;

  // 先删除原来的签名
  FParams.DelParam('Signature');
  
  FParams.AddParam('Action', FActionName);
  FParams.AddParam('Version', FVersion);
  FParams.AddParam('AccessKeyId', g_AcsUtil.KeyID);
  FParams.AddParam('Format', StrFormat[FFormatType]);
  FParams.AddParam('SignatureMethod', 'HMAC-SHA1');
  FParams.AddParam('RegionId', 'cn-hangzhou');
  FParams.AddParam('Timestamp', FormatDateTime('yyyy-mm-dd', NowUTC) + 'T' + FormatDateTime('hh:nn:ss', NowUTC) + 'Z');
  FParams.AddParam('SignatureVersion', '1.0');
  FParams.AddParam('SignatureNonce', GetSignatureNonce);

  FParams.AddParam('Signature',  FParams.SignParams(g_AcsUtil.KeySecret, Method), 1);

  StrURL := g_AcsUtil.GetApiURL + FParams.GetParamsText;

  g_RequestStr := StrUrl;

  if Method = rmGet then
    Result := UTF8Decode(g_AcsUtil.HttpGet(StrURL))
  else
    Result := UTF8Decode(g_AcsUtil.HttpPost(StrURL));
end;

procedure TAcsRequestAPI.DoAddParam(Key, Value: string);
begin
  FParams.AddParam(Key, Value);
end;

{
procedure TAcsRequestAPI.SetFormatType(const Value: TFormatType);
begin
  FFormatType := Value;
end;

procedure TAcsRequestAPI.SetActionName(const Value: string);
begin
  FActionName := Value;
end;

procedure TAcsRequestAPI.SetVersion(const Value: string);
begin
  FVersion := Value;
end;
}

function TAcsRequestAPI.GetParam(Key: string): string;
begin
  Result := FParams.GetParamValueByKey(Key);
end;

end.

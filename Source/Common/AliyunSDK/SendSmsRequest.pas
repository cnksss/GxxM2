unit SendSmsRequest;

interface

uses
  SysUtils, Classes, acsAPIClass, superobject;

type
  TSendSmsRequest = class(TAcsRequestAPI)
  private
    FSignName: string;
    FTemplateCode: string;
    procedure SetSignName(const Value: string);
    procedure SetTemplateCode(const Value: string);
  public
    constructor Create; override;
    function Request(const PhoneNumbers, TemplateParam: string; var ErrorCode, ErrorMsg: string): Boolean;
    property SignName: string read FSignName write SetSignName;
    property TemplateCode: string read FTemplateCode write SetTemplateCode;
  end;

implementation

{ TSignleSendSmsRequest }

constructor TSendSmsRequest.Create;
begin
  inherited;
  FActionName := 'SendSms';
end;

function TSendSmsRequest.Request(const PhoneNumbers, TemplateParam: string; var ErrorCode, ErrorMsg: string): Boolean;
var
  S: string;
  jo: ISuperObject;
begin
  if (Length(FSignName) = 0) or (Length(FTemplateCode) = 0) then
  begin
    raise Exception.Create('签名或模板为空');
  end;

  DoAddParam('PhoneNumbers', PhoneNumbers);
  DoAddParam('TemplateParam', TemplateParam);
  S := DoRequest();
  jo := SO(S);
  Result := Assigned(jo['Code']);
  if Result then
  begin
    ErrorCode := jo['Code'].AsString;
    ErrorMsg := jo['Message'].AsString;
    Result := SameText(ErrorCode, 'OK')
  end;
end;

procedure TSendSmsRequest.SetSignName(const Value: string);
begin
  FSignName := Value;
  DoAddParam('SignName', FSignName);
end;

procedure TSendSmsRequest.SetTemplateCode(const Value: string);
begin
  FTemplateCode := Value;
  DoAddParam('TemplateCode', FTemplateCode);
end;

end.

unit uAliyunSendSMSThread;

interface

uses
  Windows, Classes, Forms, SysUtils, SyncObjs, ObjBase, M2Locker, SendSmsRequest, acsUtils, IniFiles, acsParams;

type
  PSMSTask = ^TSMSTask;

  TSMSTask = record
    Player: TBaseObject;
    Npc: TBaseObject;
  end;

  TAliyunSendSMSThread = class(TThread)
  private
    FTaskList: TSafeList;
    FEvent: TEvent;
    FInExecuteLoop: Boolean;

    FSendSmsRequest: TSendSmsRequest;

    function GetSleeping: Boolean;
    procedure DoExecuteLoop;

    procedure ClearTaskList;
  protected
    procedure Execute; override;
  public
    constructor Create(); virtual;
    destructor Destroy; override;
    procedure Terminate; reintroduce; virtual;
    procedure TriggerEvent;
    property Sleeping: Boolean read GetSleeping;

    procedure AddTask(Player, Npc: TBaseObject);
  end;

type
  TSendSMSConfig = record
    SignName: string;
    TemplateCodeBind: string;
    TemplateCodeCheck: string;
    ResendVerifyCodeCount: Integer; // 重新发送验证码最大次数
    VerifySendInterval: Integer; // 验证码发送间隔 秒
    VerifyCodeTimeOutTime: Integer; // 验证码超时时间
    VerifyCodeErrorCount: Integer; // 验证码允许错误次数
  end;

var
  g_AliyunSendSMSThread: TAliyunSendSMSThread = nil;
  g_SendSMSConfig: TSendSMSConfig = (
    SignName: '';
    TemplateCodeBind: '';
    TemplateCodeCheck: '';
    ResendVerifyCodeCount: 3;
    VerifySendInterval: 60;
    VerifyCodeTimeOutTime: 120;
    VerifyCodeErrorCount: 3;
  );

procedure LoadSendSMSConfig;

implementation

uses
  ObjPlayer, M2Share, ObjNpc, Grobal2;

// var
// g_SignName: string;

{ TAliyunSendSMSThread }

constructor TAliyunSendSMSThread.Create;
begin
  inherited Create(False);
  FreeOnTerminate := False;
  FTaskList := TSafeList.Create('TAliyunSendSMSThread.Locker');

  FSendSmsRequest := TSendSmsRequest.Create;
  FSendSmsRequest.SignName := g_SendSMSConfig.SignName;

  {
    TEvent.Create
    参数2, Fase 事件对象控制一次后将立即重置(暂停); True 可手动暂停
    参数3, Fase 对象建立后控制为暂停状态; True 可运行状态
  }
  FEvent := TEvent.Create(nil, False, False, '');
end;

destructor TAliyunSendSMSThread.Destroy;
begin
  FSendSmsRequest.Free;
  ClearTaskList;
  FTaskList.Free;
  FEvent.Free;
  inherited;
end;

procedure TAliyunSendSMSThread.Terminate;
begin
  inherited Terminate;
  TriggerEvent;
end;

procedure TAliyunSendSMSThread.AddTask(Player, Npc: TBaseObject);
var
  SMSTask: PSMSTask;
begin
  FTaskList.LockW(1);
  try
    New(SMSTask);
    SMSTask.Player := Player;
    SMSTask.Npc := Npc;
    FTaskList.Add(SMSTask);
  finally
    FTaskList.UnLockW;
  end;

  if GetSleeping then
  begin
    TriggerEvent;
  end;
end;

procedure TAliyunSendSMSThread.Execute;
begin
  inherited;
  while not Terminated do
  begin
    FEvent.WaitFor(INFINITE);
    FInExecuteLoop := True;
    try
      DoExecuteLoop;
    finally
      FInExecuteLoop := False;
    end;
  end;
end;

function TAliyunSendSMSThread.GetSleeping: Boolean;
begin
  Result := not FInExecuteLoop;
end;

procedure TAliyunSendSMSThread.TriggerEvent;
begin
  FEvent.SetEvent;
end;

procedure TAliyunSendSMSThread.DoExecuteLoop;
var
  SMSTask: PSMSTask;
  Player, Npc: TBaseObject;
  Player2: TPlayObject;
  VerifyCode, ErrorCode, ErrorMsg, TemplateCode: string;
  // Merchant: TMerchant;
begin
  Player := nil;
  Npc := nil;
  FTaskList.LockW(2);
  try
    if FTaskList.Count > 0 then
    begin
      SMSTask := FTaskList.Items[0];
      Player := SMSTask.Player;
      Npc := SMSTask.Npc;

      Dispose(SMSTask);
      FTaskList.Delete(0);
    end;
  finally
    FTaskList.UnLockW;
  end;

  if Player = nil then
    Exit;

  try
    Player2 := UserEngine.GetPlayObject(Player);
    if (Player2 = nil) or (Player2.m_boGhost) then
      Exit;

    if Length(Player2.m_sMobileNumber) = 0 then
      Exit;

    Randomize;
    VerifyCode := IntToStr(100000 + Random(900000));

    if not Player2.m_boMobileBind then
      TemplateCode := g_SendSMSConfig.TemplateCodeBind
    else
      TemplateCode := g_SendSMSConfig.TemplateCodeCheck;

    FSendSmsRequest.TemplateCode := TemplateCode;

    // 阿里的这个参数，name: 不支持中文★★★★★★★★，不要角色名 chongchong 2018-06-03

      // if FSendSmsRequest.Request(Player2.m_sMobileNumber, Format('{"name":"%s", "code":"%s"}', [Player2.m_sCharName, VerifyCode]), ErrorCode, ErrorMsg) then

    if FSendSmsRequest.Request(Player2.m_sMobileNumber, Format('{"code":"%s"}', [VerifyCode]), ErrorCode, ErrorMsg) then
    begin
      Player2.m_sMobileVerifyCode := VerifyCode;
      Player2.m_dwMobileVerifyTick := MyGetTickCount;

      Player2.SendMsg(Player2, RM_INPUTMOBILE_VerifyCode, 0, 0, 0, 0, '');
    end
    else if Npc <> nil then
    begin
      MainOutMessage(Format('发送短信失败, SignName: %s; TemplateCode: %s; MobilePhone: %s; 错误代码: %s; 错误原因: %s', [FSendSmsRequest.SignName,
        FSendSmsRequest.TemplateCode, Player2.m_sMobileNumber, ErrorCode, ErrorMsg]));
      MainOutMessage(g_RequestStr);
      {
        UserEngine.m_MerchantList.LockR(100);
        try
        for I := 0 to UserEngine.m_MerchantList.Count - 1 do
        begin
        Merchant := TMerchant(UserEngine.m_MerchantList.Items[I]);
        if Merchant = Npc then
        begin
        Merchant.GotoLable(Player2, '@MobileVerifyCodeSendFail', False);
        Break;
        end;
        end;
        finally
        UserEngine.m_MerchantList.UnLockR;
        end;
      }
      if g_FunctionNPC <> nil then
      begin
        Player2.m_nScriptGotoCount := 0;
        g_FunctionNPC.GotoLable(Player2, '@MobileVerifyCodeSendFail', False);
      end;
    end;
  finally
    TriggerEvent;
  end;
end;

procedure TAliyunSendSMSThread.ClearTaskList;
var
  I: Integer;
  SMSTask: PSMSTask;
begin
  FTaskList.LockW(3);
  try
    for I := 0 to FTaskList.Count - 1 do
    begin
      SMSTask := FTaskList.Items[I];
      Dispose(SMSTask);
    end;
    FTaskList.Clear;
  finally
    FTaskList.UnLockW;
  end;
end;

procedure LoadSendSMSConfig;
var
  FileName: string;
  IniFile: TIniFile;
begin
  FileName := ExtractFilePath(Application.ExeName) + 'SendSMS.ini';

  IniFile := TIniFile.Create(FileName);

  IniFile.DeleteKey('aliyun', 'Endpoint');
  IniFile.DeleteKey('aliyun', 'Topic');

  if not IniFile.ValueExists('aliyun', 'KeyID') then
    IniFile.WriteString('aliyun', 'KeyID', g_AcsUtil.KeyID)
  else
    g_AcsUtil.KeyID := IniFile.ReadString('aliyun', 'KeyID', g_AcsUtil.KeyID);

  if not IniFile.ValueExists('aliyun', 'KeySecret') then
    IniFile.WriteString('aliyun', 'KeySecret', g_AcsUtil.KeySecret)
  else
    g_AcsUtil.KeySecret := IniFile.ReadString('aliyun', 'KeySecret', g_AcsUtil.KeySecret);

  if not IniFile.ValueExists('aliyun', 'SignName') then
    IniFile.WriteString('aliyun', 'SignName', g_SendSMSConfig.SignName)
  else
    g_SendSMSConfig.SignName := IniFile.ReadString('aliyun', 'SignName', g_SendSMSConfig.SignName);

  if not IniFile.ValueExists('aliyun', 'TemplateCodeBind') then
    IniFile.WriteString('aliyun', 'TemplateCodeBind', g_SendSMSConfig.TemplateCodeBind)
  else
    g_SendSMSConfig.TemplateCodeBind := IniFile.ReadString('aliyun', 'TemplateCodeBind', g_SendSMSConfig.TemplateCodeBind);

  if not IniFile.ValueExists('aliyun', 'TemplateCodeCheck') then
    IniFile.WriteString('aliyun', 'TemplateCodeCheck', g_SendSMSConfig.TemplateCodeCheck)
  else
    g_SendSMSConfig.TemplateCodeCheck := IniFile.ReadString('aliyun', 'TemplateCodeCheck', g_SendSMSConfig.TemplateCodeCheck);

  if not IniFile.ValueExists('aliyun', 'ResendVerifyCodeCount') then
    IniFile.WriteInteger('aliyun', 'ResendVerifyCodeCount', g_SendSMSConfig.ResendVerifyCodeCount)
  else
    g_SendSMSConfig.ResendVerifyCodeCount := IniFile.ReadInteger('aliyun', 'ResendVerifyCodeCount', g_SendSMSConfig.ResendVerifyCodeCount);

  if not IniFile.ValueExists('aliyun', 'VerifySendInterval') then
    IniFile.WriteInteger('aliyun', 'VerifySendInterval', g_SendSMSConfig.VerifySendInterval)
  else
    g_SendSMSConfig.VerifySendInterval := IniFile.ReadInteger('aliyun', 'VerifySendInterval', g_SendSMSConfig.VerifySendInterval);

  if not IniFile.ValueExists('aliyun', 'VerifyCodeTimeOutTime') then
    IniFile.WriteInteger('aliyun', 'VerifyCodeTimeOutTime', g_SendSMSConfig.VerifyCodeTimeOutTime)
  else
    g_SendSMSConfig.VerifyCodeTimeOutTime := IniFile.ReadInteger('aliyun', 'VerifyCodeTimeOutTime', g_SendSMSConfig.VerifyCodeTimeOutTime);

  if not IniFile.ValueExists('aliyun', 'VerifyCodeErrorCount') then
    IniFile.WriteInteger('aliyun', 'VerifyCodeErrorCount', g_SendSMSConfig.VerifyCodeErrorCount)
  else
    g_SendSMSConfig.VerifyCodeErrorCount := IniFile.ReadInteger('aliyun', 'VerifyCodeErrorCount', g_SendSMSConfig.VerifyCodeErrorCount);

  IniFile.Free;
end;

end.


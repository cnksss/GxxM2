unit HTTPService;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, IdContext, Grobal2, ObjPlayer, ObjBase, IdCustomTCPServer, IdCustomHTTPServer,
  EdCode, IdHTTPServer, StdCtrls, HUtil32, superobject;

const
  GET_PAYMENT = 0;      //支付
  GET_ONLINE = 1;       //在线玩家列表
  GET_VIEWPLAYER = 2;   //查看玩家详情
  GET_KICKPLAYER = 3;   //踢下线

type
  THTTPService = class(TThread)
    FHTTP: TIdHTTPServer;
    procedure IdHTTPServer1CommandGet(AContext: TIdContext; ARequestInfo: TIdHTTPRequestInfo; AResponseInfo: TIdHTTPResponseInfo);
  private
  protected
    procedure Execute; override;
  public
    constructor Create();
    destructor Destroy; override;
  end;

implementation

uses
  M2Share, UsrEngn;

procedure THTTPService.Execute;
begin
  inherited;
end;

constructor THTTPService.Create();
begin
  try
    FHTTP := TIdHTTPServer.Create(nil);
    FHTTP.OnCommandGet := IdHTTPServer1CommandGet;
    FHTTP.Bindings.Clear;
    FHTTP.DefaultPort := 8888;
    FHTTP.Bindings.Add.IP := '0.0.0.0';
    FHTTP.Active := True;
  except
    MainOutMessage('HTTPService启动失败.');
  end;
  inherited Create(False);
end;

destructor THTTPService.Destroy;
begin
  try
    FHTTP.Active := False;
    FHTTP.Free;
  except
    MainOutMessage('HTTPService销毁时异常.');
  end;
end;

procedure THTTPService.IdHTTPServer1CommandGet(AContext: TIdContext; ARequestInfo: TIdHTTPRequestInfo; AResponseInfo:
  TIdHTTPResponseInfo);
var
  I: Integer;
  sType, sParam1, sParam2, sParam4, sParam5: string;
  nType, nParam1, nParam2, nParam3, nParam4, nParam5, nParam6, nParam7: Integer;
  PlayObject: TPlayObject;
  aJson, bb: ISuperObject;
  tempstr, tempstr1: string;
  PayInfo: pTPayInfo;
begin
//  Sleep(5000);

  sType := ARequestInfo.Params.Values['Type'];
  nType := StrToIntDef(sType, -1);

  MainOutMessage('HTTPService 收到请求' + IntToStr(nType));

  if nType < 0 then
  begin
    MainOutMessage('HTTPService 收到无意义请求');
    Exit;
  end;

  case nType of
    GET_PAYMENT:
      begin
        PlayObject := nil;
        sParam1 := ARequestInfo.Params.Values['Name'];
        sParam5 := ARequestInfo.Params.Values['Account'];
        nParam2 := StrToIntDef(ARequestInfo.Params.Values['Price'], 0);
        nParam3 := StrToIntDef(ARequestInfo.Params.Values['Currencytype'], 0);
        nParam4 := StrToIntDef(ARequestInfo.Params.Values['isTest'], 0);
        nParam6 := StrToIntDef(ARequestInfo.Params.Values['id'], 0);
        nParam7 := StrToIntDef(ARequestInfo.Params.Values['type'], 0);
        if sParam5 <> '' then
          PlayObject := UserEngine.GetPlayObjectOfAccount(sParam5);
        if PlayObject = nil then
          PlayObject := UserEngine.GetPlayObject(sParam1);

        if PlayObject <> nil then
        begin
          PlayObject.m_nInteger[0] := nParam2;
          PlayObject.m_nInteger[1] := nParam3;
          PlayObject.m_nInteger[2] := nParam4;
          g_FunctionNPC.GotoLable(PlayObject, '@PayMentSuccessful', False);
          PlayObject.m_DefMsg := MakeDefaultMsg(SM_PAYMENTSUCCESS, nParam6, nParam7, 0, 0);
          PlayObject.SendSocket(@PlayObject.m_DefMsg, '');
          PlayObject.m_nInteger[0] := 0;
        end
        else //没有找到人物就把充值信息放到g_PayInfo里
        begin
          new(PayInfo);
          PayInfo.Price := nParam2;
          PayInfo.PayType := nParam3;
          PayInfo.IsTest := Boolean(nParam4);
          PayInfo.Name := sParam1;
          g_PayInfoList.AddObject(sParam5, TObject(PayInfo));
        end;
      end;
    GET_ONLINE:
      begin
        sParam1 := ARequestInfo.Params.Values['account'];
        sParam2 := ARequestInfo.Params.Values['name'];
        nParam3 := StrToIntDef(ARequestInfo.Params.Values['job'], -1);
        sParam4 := ARequestInfo.Params.Values['level'];
        nParam5 := StrToIntDef(ARequestInfo.Params.Values['dummy'], 0);
        //校验等级范围
        tempstr := GetValidStr3(sParam4, tempstr1, ['-']);
        nParam1 := StrToIntDef(tempstr1, 0);
        nParam2 := StrToIntDef(tempstr, 0);
        if nParam1 > nParam2 then
        begin
          nParam2 := nParam1;
        end;
        //校验等级范围

        aJson := SO('{}');
        aJson.I['result'] := UserEngine.m_PlayObjectList.Count;
        aJson['list'] := SO('[]');
        UserEngine.m_PlayObjectList.LockR(58);
        try
          for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
          begin
            PlayObject := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);
            if PlayObject = nil then
              Continue;

            if TBaseObject(UserEngine.m_PlayObjectList.Objects[I]).m_boDummyObject then
            begin
              if (nParam5 = 0) or (nParam5 = 2) then
              begin
                if (sParam1 <> PlayObject.m_sUserID) and (sParam1 <> '') then
                  Continue;
                if (sParam2 <> PlayObject.m_sCharName) and (sParam2 <> '') then
                  Continue;
                if ((nParam1 <> 0) and (nParam2 <> 0)) and ((PlayObject.m_Abil.Level < nParam1) or (PlayObject.m_Abil.Level >
                  nParam2)) then
                  Continue;
                if (nParam3 <> -1) and (nParam3 <> PlayObject.m_btJob) then
                  Continue;

                bb := SO('{}');
                bb.S['account'] := PlayObject.m_sUserID;
                bb.S['name'] := PlayObject.m_sCharName;
                bb.I['level'] := PlayObject.m_Abil.Level;
                bb.I['job'] := PlayObject.m_btJob;
                bb.I['gold'] := PlayObject.m_nGold;
                bb.I['gamegold'] := PlayObject.m_nGameGold;
                bb.I['gamepoint'] := PlayObject.m_nGamePoint;
                bb.I['gamediamond'] := PlayObject.m_nGameDiamond;
                bb.I['gamegird'] := PlayObject.m_nGameGird;
                bb.I['gameglory'] := PlayObject.m_nGameGlory;
                bb.I['state'] := 2;
                aJson.A['list'].Add(bb);
              end;
            end
            else
            begin
              if (nParam5 = 0) or (nParam5 = 1) then
              begin
                if (sParam1 <> PlayObject.m_sUserID) and (sParam1 <> '') then
                  Continue;
                if (sParam2 <> PlayObject.m_sCharName) and (sParam2 <> '') then
                  Continue;
                if ((nParam1 <> 0) and (nParam2 <> 0)) and ((PlayObject.m_Abil.Level < nParam1) or (PlayObject.m_Abil.Level >
                  nParam2)) then
                  Continue;
                if (nParam3 <> -1) and (nParam3 <> PlayObject.m_btJob) then
                  Continue;
                bb := SO('{}');
                bb.S['account'] := PlayObject.m_sUserID;
                bb.S['name'] := PlayObject.m_sCharName;
                bb.I['level'] := PlayObject.m_Abil.Level;
                bb.I['job'] := PlayObject.m_btJob;
                bb.I['gold'] := PlayObject.m_nGold;
                bb.I['gamegold'] := PlayObject.m_nGameGold;
                bb.I['gamepoint'] := PlayObject.m_nGamePoint;
                bb.I['gamediamond'] := PlayObject.m_nGameDiamond;
                bb.I['gamegird'] := PlayObject.m_nGameGird;
                bb.I['gameglory'] := PlayObject.m_nGameGlory;
                bb.I['state'] := Integer(PlayObject.m_boOffLine);
                  //TPlayObject(UserEngine.m_PlayObjectList.Objects[I]).m_nGameGlory;
                aJson.A['list'].Add(bb);
              end;
            end;
          end;
        finally
          UserEngine.m_PlayObjectList.UnLockR;
        end;

        AResponseInfo.ContentEncoding := 'utf-8';
        AResponseInfo.ContentType := 'application/json';
        AResponseInfo.ContentText := aJson.AsJSon();
      end;
    GET_VIEWPLAYER:
      begin
        aJson := SO('{}');
        aJson.I['result'] := 0;
        sParam1 := ARequestInfo.Params.Values['name'];
        sParam1 := ARequestInfo.Params.Values['page'];
        PlayObject := UserEngine.GetPlayObject(sParam1);
        if PlayObject <> nil then
        begin

          aJson.I['result'] := 1;
        end;
        AResponseInfo.ContentEncoding := 'utf-8';
        AResponseInfo.ContentType := 'application/json';
        AResponseInfo.ContentText := aJson.AsJSon();
      end;
    GET_KICKPLAYER:
      begin
        aJson := SO('{}');
        aJson.I['result'] := 0;
        sParam1 := ARequestInfo.Params.Values['name'];
        PlayObject := UserEngine.GetPlayObject(sParam1);
        if PlayObject <> nil then
        begin
          PlayObject.m_boKickFlag := True;
          PlayObject.m_boOffLine := False;
          PlayObject.m_boPlayOffLine := False;
          PlayObject.m_boEmergencyClose := True;
          aJson.I['result'] := 1;
        end;
        AResponseInfo.ContentEncoding := 'utf-8';
        AResponseInfo.ContentType := 'application/json';
        AResponseInfo.ContentText := aJson.AsJSon();
      end;
  end;

//  MainOutMessage(Format('ARequestInfo: %s %s', [ARequestInfo.Document, ARequestInfo.QueryParams]));
//  for I := 0 to ARequestInfo.Params.Count - 1 do
//  begin
//    MainOutMessage(ARequestInfo.Params[I].Value);
//  end;
//  MainOutMessage();
//浏览器请求http://127.0.0.1:8008/index.html?a=1&b=2
  //ARequestInfo.Document  返回    /index.html
  //ARequestInfo.QueryParams 返回  a=1b=2
  //ARequestInfo.Params.Values['name']   接收get,post过来的数据
  ////webserver发文件
//  LFilename := ARequestInfo.Document;
//  if LFilename = '/' then
//  begin
//    LFilename := '/' + trim(edit_index.Text);
//  end;
//  LPathname := RootDir + LFilename;
//  if FileExists(LPathname) then
//  begin
//    AResponseInfo.ContentStream := TFileStream.Create(LPathname, fmOpenRead + fmShareDenyWrite); //发文件
//
//  end
//  else
//  begin
//    AResponseInfo.ResponseNo := 404;
//    AResponseInfo.ContentText := '找不到' + ARequestInfo.Document;
//  end;
   //发html文件
//   AResponseInfo.ContentEncoding:='utf-8';
//   AResponseInfo.ContentType :='text/html';
//   AResponseInfo.ContentText:='<html><body>好</body></html>';
   //发xml文件
   {AResponseInfo.ContentType :='text/xml';
   AResponseInfo.ContentText:='<?xml version="1.0" encoding="utf-8"?>'
   +'<students>'
   +'<student sex = "male"><name>'+AnsiToUtf8('陈')+'</name><age>14</age></student>'
   +'<student sex = "female"><name>bb</name><age>16</age></student>'
   +'</students>';}
   //下载文件时，直接从网页打开而没有弹出保存对话框的问题解决
//AResponseInfo.CustomHeaders.Values['Content-Disposition'] :='attachment; filename="'+文件名+'"';
//替换 IIS
  {AResponseInfo.Server:='IIS/6.0';
  AResponseInfo.CacheControl:='no-cache';
  AResponseInfo.Pragma:='no-cache';
  AResponseInfo.Date:=Now;}
end;

end.


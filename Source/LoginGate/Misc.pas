unit Misc;

interface

uses
  Windows, Messages, SysUtils, WinSock2, ClientSession;

const
  VER_TYPE = 0; // 0: 通用版； 1: 专用版
  VER_VERSION = '2025/04/28';

{$IF VER_TYPE = 0}
  PROGRAM_NAME = '登陆网关';
  VER_TYPE1_TEST = 1;

{$ELSE}
  PROGRAM_NAME = '登陆网关-专用版';
  VER_TYPE1_TEST = 0;

type
  PRungateVerifyHeader = ^TRungateVerifyHeader;
  TRungateVerifyHeader = record
    dwCode: LongWord;
    dwCmd: LongWord;
    nLength: LongWord;
  end;

  TRungateVerifyData = packed record
    Key: LongWord;
    IP: array[0..14] of Char;
    HWID: array[0..7] of LongWord;
  end;

  // 服务器发到客户端的包头
  TServerMessageClientFlag = packed record
    smLogon: Word; // 登录
    smChangeMap: Word; // 换地图
    smAbility: Word; // 属性
    smSendNotice: Word; // 发送公告消息

    smModuleMD5: Word; // 模块md5
    smBlackModuleMd5: Word; // 黑名单模块md5
    smSendCustomMonsterConfig: Word; // 自定义怪物配置
    smStdItemList: Word; // 物品列表
    smSendItemDescList: Word; // 物品备注
    smSendTZItemDescList: Word; // 套装物品备注
    smSendFilterItemList: Word; // 内挂捡取列表
    smEffectImageList: Word; // 特效文件列表
    smSpecialCmd: Word; // 特殊命令
    smSendCustomMagicConfig: Word; // 自定义技能配置
    smPlugFile: Word; // 插件文件
    smServerConfig: Word; // 服务器配置
    smSendCustomNpcConfig: Word; // 自定义NPC配置
    smSendItemDescTopList: Word;

    smSendRungateCheckCode: Word; // 网关验证码

    smModuleMd5Cache: Word; // 模块md5缓存
    smSendCustomMonsterConfigCache: Word; // 自定义怪物配置缓存
    smStdItemListCache: word; // 物品列表缓存
    smSendItemDescListCache: Word; // 物品备注缓存
    smSendTZItemDescListCache: Word; // 套装物品备注缓存
    smSendFilterItemListCache: Word; // 内挂捡取列表缓存
    smEffectImageListCache: Word; // 特效文件列表缓存
    smSpecialCmdCache: Word; // 特殊命令缓存
    smSendCustomMagicConfigCache: Word; // 自定义技能配置缓存
    smPlugFileCache: Word; // 插件文件缓存
    smServerConfigCache: Word; // 服务器配置缓存
    smSendCustomNpcConfigCache: Word; // 自定义NPC配置缓存
    smSendItemDescTopListCache: Word; //

    smGetProcessList: Word; // 进程列表
    smGetScreenShotGame: Word; // 游戏载图
    smGetScreenShot: Word; // 屏幕截图
    smAntiPlugStream: Word; // 反外挂插件流
    smProcessBlacklist: Word; // 进程黑名单列表
  end;

  TLicenseDataInfo = packed record
    KeyVersion: DWORD; // 版本号
    Reseved: array[0..9] of DWORD; // 保留位置
    EnabledAntiPlug: Boolean;
    EnabledVerifyCode: Boolean;
    Reseved3: Word;

    RandomCode1Arr: array[0..19] of DWORD;

    SrvMsgClientFlag: TServerMessageClientFlag;
    SrvMsgClientFlagCRC: DWORD;
    SrvMsgClientFlagCRC_Enc: DWORD;

    KeyLastDay: Integer;
    GameLoginConfigMD5: array[0..3] of LongWord;
  end;

  TLoginGateAddDataIn = record
    RandomCode: array[0..15] of DWORD;
    Key1: LongWord; // 用这个来做通讯key
    Key2: LongWord;
    Reseved: array[0..59] of Byte;
  end;

  TLoginGateAddDataEnc = record
    HeaderFlag: DWORD;
    Version: LongWord;
    CRC: LongWord;
    Key1: LongWord;
    Data: array[0..255] of Byte;
    Key2: LongWord;
    Key3: LongWord;
  end;

var
  Register_Str: string;

  g_nProtocolKey3: LongWord = 0;
{$IFEND}


const
  SG_FORMHANDLE = 1000; // 服务器HANLD
  SG_STARTNOW = 1001; // 正在启动服务器...
  SG_STARTOK = 1002; // 服务器启动完成...
  SG_ACTIVE = 1003;

  GS_QUIT = 2000; // 关闭

type
  TProgamType = (tDBServer, tLoginSrv, tLogServer, tM2Server, tLoginGate,
    tLoginGate1, tSelGate, tSelGate1, tRunGate, tRunGate1, tRunGate2,
    tRunGate3, tRunGate4, tRunGate5, tRunGate6, tRunGate7);

procedure CloseIPConnect(const nRemoteIP: Integer);
function KickUser(const nRemoteIP: Integer): Boolean; overload;
procedure KickUser(const UserObj: TSessionObj); overload;
procedure BlockUser(const UserObj: TSessionObj);

function ReverseIP(dwIP: DWORD): DWORD;
function AnsiStrToVal(const nPtr: PChar; var nPos: Integer): Integer;

procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);

function CheckAccountName(sName: string): Boolean;

var
  g_nProtocolKey2: DWORD = 0;

implementation

uses
  AcceptExWorkedThread, SHSocket, AppMain, Protocol,
  ConfigManager, IPAddrFilter, Grobal2, SDK;


procedure CloseIPConnect(const nRemoteIP: Integer);
var
  i, n: Integer;
  UserObj: TSessionObj;
begin
  if not g_fServiceStarted then
    Exit;
  for n := 0 to USER_ARRAY_COUNT - 1 do
  begin
    UserObj := g_UserList[n];
    if (UserObj <> nil) and
      (UserObj.m_tLastGameSvr <> nil) and
      (UserObj.m_tLastGameSvr.Active) and
      (UserObj.m_pUserOBJ.nIPAddr = nRemoteIP) then
    begin
      if UserObj.m_fHandleLogin >= 2 then
      begin
        UserObj.SendDefMessage(SM_OUTOFCONNECTION, UserObj.m_nSvrObject, 0, 0, 0, '');
        UserObj.m_fKickFlag := True;
      end
      else
        SHSocket.FreeSocket(UserObj.m_pUserOBJ._SendObj.Socket);
    end;
  end;
end;

function KickUser(const nRemoteIP: Integer): Boolean;
begin
  Result := True;
  if g_pConfig.m_fKickOverPacketSize then
  begin
    case g_pConfig.m_tBlockIPMethod of
      mDisconnect:
        begin
          Result := False;
        end;
      mBlock:
        begin
          AddToTempBlockIPList(nRemoteIP);
          CloseIPConnect(nRemoteIP);
          Result := False;
        end;
      mBlockList:
        begin
          AddToBlockIPList(nRemoteIP);
          CloseIPConnect(nRemoteIP);
          Result := False;
        end;
    end;
  end;
end;

procedure KickUser(const UserObj: TSessionObj);
begin
  if g_pConfig.m_fKickOverPacketSize then
  begin
    SHSocket.FreeSocket(UserObj.m_pUserOBJ._SendObj.Socket);
    UserObj.m_fKickFlag := True;
    case g_pConfig.m_tBlockIPMethod of
      mBlock:
        begin
          AddToTempBlockIPList(UserObj.m_pUserOBJ.nIPAddr);
        end;
      mBlockList:
        begin
          AddToBlockIPList(UserObj.m_pUserOBJ.nIPAddr);
        end;
    end;
  end;
end;

procedure BlockUser(const UserObj: TSessionObj);
begin
  if g_pConfig.m_fKickOverPacketSize then
  begin
    UserObj.m_fKickFlag := True;
    case g_pConfig.m_tBlockIPMethod of
      mBlock:
        begin
          AddToTempBlockIPList(UserObj.m_pUserOBJ.nIPAddr);
        end;
      mBlockList:
        begin
          AddToBlockIPList(UserObj.m_pUserOBJ.nIPAddr);
        end;
    end;
  end;
end;

function ReverseIP(dwIP: DWORD): DWORD;
begin
  Result := (LOBYTE(LOWORD(dwIP)) shl 24) or
    (HIBYTE(LOWORD(dwIP)) shl 16) or
    (LOBYTE(HIWORD(dwIP)) shl 8) or
    (HIBYTE(HIWORD(dwIP)));
end;

function AnsiStrToVal(const nPtr: PChar; var nPos: Integer): Integer;
var
  c: Char;
  tPtr: PChar;
  total: Integer;
begin
  nPos := 0;
  Result := 0;
  if nPtr = nil then
    Exit;
  tPtr := nPtr;
  c := tPtr^;
  total := 0;
  while ((c >= '0') and (c <= '9')) do
  begin
    total := 10 * total + (Byte(c) - Byte('0'));
    inc(tPtr);
    inc(nPos);
    c := tPtr^;
  end;
  Result := total;
end;

procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);
var
  SendData: TCopyDataStruct;
  nParam: Integer;
begin
  nParam := MakeLong(Word(tLoginGate), wIdent);
  SendData.cbData := Length(sSendMsg) + 1;
  GetMem(SendData.lpData, SendData.cbData);
  StrCopy(SendData.lpData, PChar(sSendMsg));
  SendMessage(g_hGameCenterHandle, WM_COPYDATA, nParam, Cardinal(@SendData));
  FreeMem(SendData.lpData);
end;

function CheckAccountName(sName: string): Boolean;
var
  I: Integer;
  nLen: Integer;
begin
  Result := False;
  if sName = '' then
    Exit;
  Result := true;
  nLen := Length(sName);
  I := 1;
  while (true) do
  begin
    if I > nLen then
      Break;
    if (sName[I] < '0') or (sName[I] > 'z') then
    begin
      Result := False;
      if (sName[I] >= #$B0) and (sName[I] <= #$C8) then
      begin
        Inc(I);
        if I <= nLen then
          if (sName[I] >= #$A1) and (sName[I] <= #$FE) then
            Result := true;
      end;
      if not Result then
        Break;
    end;
    Inc(I);
  end;
end;

end.


unit RungateCommon;

interface

uses
  Windows;

type
  TDefaultMessage = record
    Recog: Int64;
    Ident: Word;
    Param: Word;
    Tag: Word;
    Series: Word;
  end;
  pTDefaultMessage = ^TDefaultMessage;
  
type
  PRunGatePlugClientInfo = ^TRunGatePlugClientInfo;
  TRunGatePlugClientInfo = record
    RecogId: Int64;                                 // 对象ID
    MoveSpeed: Integer;                             // 移动速度
    AttackSpeed: Integer;                           // 攻击速度
    SpellSpeed: Integer;                            // 魔法速度

    Account: array[0..15] of Char;                  // 帐户
    ChrName: array[0..15] of Char;                  // 角色名
    IPValue: Integer;
    IpAddr:  array[0..23] of Char;                  // 客户端IP
    Port:    Integer;                               // 客户端端口

    MacID:    array[0..63] of Char;                 // 机器码

    IsActive: BOOL;                                 // 是否为活动连接
    IsLoginNotice: BOOL;                            // 客户端点击公告确定
    IsPlayGame: BOOL;                               // 背包，技能等均初始化完成，进入游戏中

    DataAdd: Pointer;                               // 附加数据
    DataLen: LongWord;                              // 附加数据长度 (内存已分配，最多300Byte)

    Reseved2: array[0..29] of LongWord;
  end;

  TRunGatePlugGetClientInfoFunc = function(ClientID: Integer; ClientInfo: PRunGatePlugClientInfo): BOOL; stdcall;
  TRunGatePlugAddMainLogMsgFunc = procedure(lpData: PAnsiChar; nLevel: Integer); stdcall;
  TRunGatePlugSendSocketFunc = procedure(ClientID: Integer; DefMsg: pTDefaultMessage; lpData: PAnsiChar; DataLen: Integer); stdcall;
  TRunGatePlugEncodeDecodeFunc = function(lpInData: PAnsiChar; InDataLen: Integer; lpOutData: PAnsiChar; var OutDataLen: Integer): BOOL; stdcall;
  TRunGatePlugCloseClientFunc = procedure(ClientID: Integer; DelayTime: LongWord{延时时间，秒}; Code: Integer{关闭代码}); stdcall;
  TRunGatePlugLockClient = procedure(ClientID: Integer; LockTime: LongWord); stdcall;
  TRunGatePlugSetClientPlugLoad = procedure(ClientID: Integer); stdcall;

  PInitRecord = ^TInitRecord;
  TInitRecord = record
    MainFormHandle: HWND;

    GetClientInfo: TRunGatePlugGetClientInfoFunc;             // 获取客户端连接的信息
    AddShowLog: TRunGatePlugAddMainLogMsgFunc;                // 在网关上显示日志

    SendDataToClient: TRunGatePlugSendSocketFunc;             // 发送数据到客户端

    EncodeBuffer: TRunGatePlugEncodeDecodeFunc;               // 将非可视字符编码为可视字符
    DecodeBuffer: TRunGatePlugEncodeDecodeFunc;               // 将非可视字符编码为可视字符

    CloseClient: TRunGatePlugCloseClientFunc;                 // +++++++++++++++关闭内核连接
    LockClient: TRunGatePlugLockClient;                       // 锁定用户，时间秒

    SetClientPlugLoad: TRunGatePlugSetClientPlugLoad;         // 通知客端反外挂模块加载成功
  end;

var
  g_InitRecord: TInitRecord;

  function tick_diff(tick_start, tick_end: Cardinal): Cardinal;

implementation

function tick_diff(tick_start, tick_end: Cardinal): Cardinal;
begin
  if tick_end >= tick_start then
    result := tick_end - tick_start
  else
    result := High(Cardinal) - tick_start + tick_end;
end;

{
  // 导出函数
  function Init(InitRecord: PPlugInitRecord; IsReload: BOOL): BOOL; stdcall;
  procedure Uninit(); stdcall;
  procedure ClientStart(ClientID: Integer); stdcall;
  procedure ClientEnd(ClientID: Integer); stdcall;
  procedure ClientRecvPacket(ClientID: Integer; DefMsg: pTDefaultMessage; lpData: PAnsiChar; DataLen: Integer; var IsSendToM2: BOOL); stdcall;
  procedure ShowConfigForm; stdcall; 
}


end.
 
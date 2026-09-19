unit GateShare;

interface

{$I Iocp.inc}

uses
  Windows, Messages, SysUtils, Classes, JSocket, WinSock, SyncObjs, IniFiles,
  EncryptUnit_LF, HUtil32, MD5Util, LbAsym, LbRSA, CheckUnit, WinHttp,
  EDcode, Grobal2_Ex, IocpCommon, MagicIntervalUtils,
  Forms, StdCtrls, Graphics, MMSystem;

const
  tRunGate = 8;
  g_sUpdateTime = '2023-07-01';

  GATEMAXSESSION = 1000;
  MSGMAXLENGTH = 20000;
  SENDCHECKSIZE = 512;
  SENDCHECKSIZEMAX = 2048;

  sSTATUS_FAIL = '+FAIL/';
  sSTATUS_GOOD = '+GOOD/';

  HALF_SPEED_INTERVALS_COUNT = 200;
  SPEED_INTERVALS_COUNT = HALF_SPEED_INTERVALS_COUNT * 2 + 1;

type
  TSockaddr = record
    nIPaddr: Integer;
    dwStartAttackTick: LongWord;
    nAttackCount: Integer;
    nSocketHandle: Integer;
  end;
  pTSockaddr = ^TSockaddr;

  TVersionNumber = packed record
    { * 文件版本号 }
    Major: Word;
    Minor: Word;
    Release: Word;
    Build: Word;
  end;

  TAddressList = class(TObject)
  private
    FAddress: TList;
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLocker: Integer;
  {$ELSE}
    FCS: TRTLCriticalSection;
  {$ENDIF}
    function GetItems(Index: Integer): pTSockaddr;
    function GetCount: Integer;
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;

    property Items[Index: Integer]: pTSockaddr read GetItems; default;
    property Count: Integer read GetCount;
    function Find(IP: string): pTSockaddr;
    function FindIndex(IP: string): Integer;

    function Add(IP: string): pTSockaddr;
    procedure Delete(Sockaddr: pTSockaddr); overload;
    procedure Delete(IP: string); overload;

    procedure DeleteIndex(Index: Integer);

    procedure Clear;
    procedure Lock;
    procedure UnLock;
  end;

  pTAddressInfo = ^TAddressInfo;
  TAddressInfo = record
    sIPaddr: string;
    nIPaddr: Integer;
    nCount: Integer;
    dwIPCountTick1: LongWord;
    nIPCount1: Integer;
    dwIPCountTick2: LongWord;
    nIPCount2: Integer;
    dwDenyTick: LongWord;
    nIPDenyCount: Integer;
  end;

  TAddressListEx = class(TObject)
  private
    FAddress: TList;
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLocker: Integer;
  {$ELSE}
    FCS: TRTLCriticalSection;
  {$ENDIF}
    function GetItems(Index: Integer): pTAddressInfo;
    function GetCount: Integer;
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;

    property Items[Index: Integer]: pTAddressInfo read GetItems; default;
    property Count: Integer read GetCount;
    function Find(IP: string): PTAddressInfo;
    function Add(IP: string): PTAddressInfo;
    procedure Delete(AddressInfo: pTAddressInfo);

    procedure Clear;
    procedure Lock;
    procedure UnLock;
  end;

  TSafeHashStringList = class(THashedStringList)
  private
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLocker: Integer;
  {$ELSE}
    FCS: TRTLCriticalSection;
  {$ENDIF}
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

  TSafeStringList = class(TStringList)
  private
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLocker: Integer;
  {$ELSE}
    FCS: TRTLCriticalSection;
  {$ENDIF}
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

  TSafeMemoryStream = class(TMemoryStream)
  private
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLocker: Integer;
  {$ELSE}
    FCS: TRTLCriticalSection;
  {$ENDIF}
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

  PTProcessInfo = ^TProcessInfo;
  TProcessInfo = record
    ProcessName: string;
    ProcessMD5: string;
  end;

  TProcessBlacklist = class(TObject)
  private
    FMaxCount: Integer;
    FList: TList;

  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLocker: Integer;
  {$ELSE}
    FCS: TRTLCriticalSection;
  {$ENDIF}
    function GetItems(Index: Integer): PTProcessInfo;
    function GetCount: Integer;
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;

    property MaxCount: Integer read FMaxCount;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: PTProcessInfo read GetItems; default;
    function Find(const MD5: string): PTProcessInfo;
    function Add(const ProcessName, ProcessMD5: string): PTProcessInfo;
    procedure Delete(ProcessInfo: PTProcessInfo);

    procedure Clear;
    procedure Lock;
    procedure UnLock;
  end;

 //ricky   这一块貌似是获取百度时间的类  去掉
//{$IF NEED_REGISTER = 1}
//  TGetBaiduTimeThread = class(TThread)
//  protected
//    procedure Execute; override;
//  public
//    constructor Create(CreateSuspended: Boolean);
//  end;
//{$IFEND}

  PTIPSection = ^TIPSection;
  TIPSection = record
    nBeginAddr: LongWord;
    nEndAddr: LongWord;
  end;

  TBlockIPMethod = (bmDisconnect, bmTempBlock, bmBlockList);
  TFilterSayMsgMode = (fsmmAllBlock, fsmmSelfBolck, fsmmClose, fsmmDisMsg, fsmmDisMsgorSys);

  TActionProcessMode = (apmDelay {延时处理 停顿}, apmRebound {反弹卡刀}, apmLost {丢弃封包 卡位}, apmOffline {掉线处理}, apmFakeAttackPass {假刀放行}, apmNoProcess{不处理});
  TSumActionProcessMode = (sapmNone {不处理}, sampOffline {掉线}, sampLockUser {锁定});

  PAntiPlugAction = ^TAntiPlugAction;
  TAntiPlugAction = record
    boEnabled: Boolean;
    nInterval: DWORD;
    ProcessMode: TActionProcessMode;
    boProcessScript: Boolean;
    SumProcessMode: TSumActionProcessMode;
    //nCollectCount: Integer;
    //nCollectSpeedCount: Integer;
    //nFloatingInterval: Integer;
    boShowHint: Boolean;
    sHintText: string;
    nCompensationValue: Integer;
    boDebug: Boolean;
  end;

  TSpeedIntervals = array[0..SPEED_INTERVALS_COUNT - 1] of Word;

  TAntiPlugActionMode = (
    amHit {攻击}, amSpell {魔法}, amWalk {走路},
    amRun {跑步}, amTurn {转向},  amCutMeat {挖肉},
    amWalkToHit {走路到攻击},     amHitToWalk {攻击到走路},
    amRunToHit {跑步到攻击},      amHitToRun {攻击到跑步},
    amWalkToSpell {走路到魔法},   amSpellToWalk {魔法到走路},
    amRunToSpell {跑步到魔法},    amSpellToRun {魔法到跑步},
    amTurnToHit {转向到攻击},     amHitToTurn {攻击到转向},
    amTurnToSpell {转向到魔法},   amSpellToTurn {魔法到转向},
    amCutMeatToHit {挖肉到攻击},  amCutMeatToSpell {挖肉到魔法},
    amMoveToTurn {移动到转向},    amTurnToMove {转向到移动},
    amMoveToCutMeat {移动到挖肉}, amCutMeatToMove {挖肉到移动},
    amHitConcurrent {攻击并发},   amSpellConcurrent {魔法并发},
    amMoveConcurrent {移动并发} (*, amAllConcurrent {所有并发}*)
    );

  // 基本动作
  TBaseAction = (baOther, baHit, baSpell, baWalk, baRun, baTurn, baCutMeat);

  TAntiPlugConfig = record
    ActionList: array[TAntiPlugActionMode] of TAntiPlugAction;

    btMsgType: Byte;
    btMsgFColor: Byte;
    btMsgBColor: Byte;

    nLockTime: Integer;
    boSaveLockStatus: Boolean;
    boShowLockLog: Boolean;
    sShowLockMsg: string;

    boSpeedClearData: Boolean;

    dwUserShop_Search_Interval: LongWord;                 // 个人商店搜索间隔
    boUserShop_Search_ShowHint: Boolean;

    dwUserShop_Buy_Interval: LongWord;                    // 个人商店购买间隔
    boUserShop_Buy_ShowHint: Boolean;

    dwTakeOn_Item_Interval: LongWord;                     // 穿戴装备间隔
    boTakeOn_Item_ShowHint: Boolean;                      

    dwDealTry_Attack_Interval: LongWord;                  // 交易到挑战间隔
    boDealTry_Attack_ShowHint: Boolean;                   // 显示提示

    dwBrutal_Attack_Interval: LongWord;                   // 野蛮到攻击间隔
    boBrutal_Attack_ShowHint: Boolean;                    // 显示提示

    boShowAttackLog: Boolean;
    //boShowDropConcurrentLog: Boolean;

    dwContinueSpeedPassIncTime: LongWord;                 // 连续超速放行时间增加

    boContinueSpeedCloseSocket: Boolean;                  // 角色连续超速踢出游戏
    nContinueSpeedCount: Integer;                         // 连续超速次数

    nSumSpeedCheckTime: Integer;                          // 累计超速统计时间间隔
    nSumSpeedMaxCount: Integer;                           // 累计超速最大超速次数

    dwCollectCount: Integer;
    dwSpeedValue: Integer;

    boZeroCompensationValueClearPool: Boolean;            // 0 补偿时清补偿池

    dwClientUploadPickItemsTime: Word;                    // 客户端上传内挂物品间隔
  end;

  TGameSpeed = record
    dwTicks: array[TAntiPlugActionMode] of LongWord;      // 最后时间 算上延时包

    nDelayCount: array[TAntiPlugActionMode] of Integer;

    boWantZeroCounts: array[TAntiPlugActionMode] of Boolean;

    dwDealTryTick: LongWord;                              // 请求交易时间
    dwAttackTick: LongWord;                               // 攻击时间

    dwMooteboTick: LongWord;                              // 野蛮冲撞时间

    dwShopItemSearchTick: LongWord;
    dwUserShopItemSearchTick: LongWord;

    dwUserShopBuyTick: LongWord;
    dwTakeOnItemTick: LongWord;
    dwHeroTakeOnItemTick: LongWord;

    // 用于跑步或走路服务器返回 fail 时，还原记录的时间
    OldLastRunTick: LongWord;                             // 上一次跑步时间
    OldLastWalkTick: LongWord;                            // 上一次走路时间

    // 修正时间，比如限速500，当出现 600, 400 两个速度时，来修正第2个的时间
    RepairRunTick: LongWord;
    RepairWalkTick: LongWord;
    RepairHitTick: LongWord;
    RepairSpellTick: LongWord;

    boContinueSpeed: Boolean;                             // 是否是连续加速
    dwStartSpeedTick: LongWord;                           // 开始加速时间
  end;

{$IF CLIENT_ANTIPLUG = 1}
  TAntiPlugAddData = record
    RungateType: LongWord;
    DllCRC: LongWord;
    DllLen: LongWord;
    RandKey: array[0..7] of Byte;
  end;
{$IFEND}

  TItemCDTime = record
    NormalHP: LongWord;             // 普通只加HP
    NormalMP: LongWord;             // 普通只加MP
    NormalHPMP: LongWord;           // 普通同时加HP,MP
    SpecialHP: LongWord;            // 特殊只加HP
    SpecialMP: LongWord;            // 特殊只加MP
    SpecialHPMP: LongWord;          // 特殊同时加HP,MP
    Other: LongWord;                // 加其它属性
  end;

  TEatItemCDConfig = record
    Hum: array[0..2] of TItemCDTime;
    Hero: array[0..2] of TItemCDTime;
  end;

  TClientAntiPlugIdents = array[0..3] of Word;

  function ActionModeUseSpeedIntervals(ActionMode: TAntiPlugActionMode): Boolean;
  procedure RebuildSendToClientSpeedIntervalsText;

  procedure AddBlockIP(sIPaddr: string);
  procedure AddTempBlockIP(sIPaddr: string);

  procedure AddTempBlockMac(sMac: string);
  procedure AddBlockMac(sMac: string);

  procedure AddMainLogMsg(Msg: string; nLevel: Integer; AddTime: Boolean = True);
  function tick_diff(tick_start, tick_end: Cardinal): Cardinal;

  procedure AddIOCPLogMsg(Msg: string);
  procedure LoadFilterSayMsgFile();

  function ReadFYDenyIPListFile(): Boolean;
  function ReadFYPassIPListFile(): Boolean;
  function ReadFYDenyMACListFile(): Boolean;

  function CheckInFYDenyIPList(sIPAddr: string): Boolean;
  function CheckInFYPassIPList(sIPAddr: string): Boolean;
  
  procedure LoadBlockIPFile();
  procedure SaveBlockIPList();

  procedure LoadBlockMacFile();
  procedure SaveBlockMacList();

  procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);

  procedure LoadIPSectionList();
  procedure SaveIPSectionList();

  procedure LoadDBAddressTable();

  function IsHexString(S: string): Boolean;
  function StrToHexEx(S: AnsiString): AnsiString;
  function HexToStrEx(const StrHex: AnsiString; var OutStr: AnsiString): Boolean;
  
  procedure LoadProcessBlacklist();
  procedure SaveProcessBlacklist();
  procedure RebuildProcessBlacklist;

  function LoadNoVerifyChrList: Boolean;

{$IF CLIENT_ANTIPLUG = 1}
  // 加载客户端反外挂模块
  function LoadClientAntiPlugDll: Boolean;
  function CheckClientAntiPlugDllChanged: Boolean;
{$IFEND}

  function IP2Long(sIPaddr: string): LongWord;
  function Long2IP(IP: LongWord): string;

  function IsBlockIP(sIPaddr: string): Boolean;
  function IsConnLimited(sIPaddr: string): Boolean;
  function IsBlockMac(sMac: string): Boolean;

  // 获取某ip的攻击次数
  function GetAttackCountOfIP(sIPaddr: string): Integer;

  // 获取某ip的客户端连接数
  function GetConnectCountOfIP(sIPaddr: string): Integer;

  function InputPassword(const ACaption, APrompt: string; ADefault: string = ''): string;
  function InputPasswordEx(const ACaption, APrompt: string; var Value: string): Boolean;

  function EncodeRunGateMsg(DefMsg: pTDefaultMessage; DataAdd: PChar; DataAddLen: LongWord): string;

{$IF VERSION_TYPE = 1}
  function CheckInWhiteList(S: string): Boolean;
{$IFEND}

  function MyGetTickCount: DWORD;

  function GetFileVersionStr(const FileName: string): string;

  // function MyGetTickCount: DWORD; stdcall; external mmsyst name 'timeGetTime';
  // function MyGetTickCount: DWORD; stdcall;; external kernel32 name 'GetTickCount';

const
  GateClass: string = 'GameGate';   //'RunGate';

{$IF RungateLEG_IOCP = 1}
  GateName: string = '游戏网关<IOCP1>';
{$ELSEIF RungateLEG_IOCP = 2}
  GateName: string = '游戏网关<IOCP2>';
{$ELSEIF RungateLEG_IOCP = 3}
  GateName: string = '游戏网关<IOCP3>';
{$ELSEIF RungateLEG_IOCP = 4}
  GateName: string = '游戏网关<IOCP4>';
{$ELSEIF RungateLEG_IOCP = 5}
  GateName: string = '游戏网关<IOCP5>';
{$ELSEIF RungateLEG_IOCP = 6}
  GateName: string = '游戏网关<IOCP6>';
{$IFEND}
                                                  
  ActionProcessModeNames: array[TActionProcessMode] of string = (
    '停顿操作', '反弹卡刀', '卡位操作', '掉线处理', '假刀放行', '不做处理');

  ActionProcessModeNames2: array[TActionProcessMode] of string = (
    '停顿操作', '反弹卡刀', '卡位操作', '掉线处理', '丢弃封包', '不做处理');

  SumActionProcessModeNames: array[TSumActionProcessMode] of string = (
    '不处理', '掉线操作', '锁定用户');

  AntiPlugActionModeNames: array[TAntiPlugActionMode] of string = (
    '攻击间隔  ', '魔法间隔  ', '走路间隔  ',
    '跑步间隔  ', '转向间隔  ', '挖肉间隔  ',
    '走路→攻击', '攻击→走路',
    '跑步→攻击', '攻击→跑步',
    '走路→魔法', '魔法→走路',
    '跑步→魔法', '魔法→跑步',
    '转向→攻击', '攻击→转向',
    '转向→魔法', '魔法→转向',
    '挖肉→攻击', '挖肉→魔法',
    '移动→转向', '转向→移动',
    '移动→挖肉', '挖肉→移动',
    '攻击并发数', '魔法并发数',
    '移动并发数' {, '所有并发数 >'});

  AntiPlugActionModeNames_2: array[TAntiPlugActionMode] of string = (
    '攻击',   '魔法',   '走路',
    '跑步',   '转向',   '挖肉',
    '走路→攻击', '攻击→走路',
    '跑步→攻击', '攻击→跑步',
    '走路→魔法', '魔法→走路',
    '跑步→魔法', '魔法→跑步',
    '转向→攻击', '攻击→转向',
    '转向→魔法', '魔法→转向',
    '挖肉→攻击', '挖肉→魔法',
    '移动→转向', '转向→移动',
    '移动→挖肉', '挖肉→移动',
    '攻击并发',   '魔法并发',
    '移动并发'    {, '所有并发数 >'});

  AntiPlugActionModeNames_3: array[TAntiPlugActionMode] of string = (
    '攻击间隔', '魔法间隔', '走路间隔',
    '跑步间隔', '转向间隔', '挖肉间隔',
    '走路→攻击', '攻击→走路',
    '跑步→攻击', '攻击→跑步',
    '走路→魔法', '魔法→走路',
    '跑步→魔法', '魔法→跑步',
    '转向→攻击', '攻击→转向',
    '转向→魔法', '魔法→转向',
    '挖肉→攻击', '挖肉→魔法',
    '移动→转向', '转向→移动',
    '移动→挖肉', '挖肉→移动',
    '攻击并发数', '魔法并发数',
    '移动并发数' {, '所有并发数 >'});

  AntiPlugActionModeSections: array[TAntiPlugActionMode] of string = (
    'Hit', 'Spell', 'Walk',
    'Run', 'Turn', 'CutMeat',
    'WalkToHit', 'HitToWalk',
    'RunToHit', 'HitToRun',
    'WalkToSpell', 'SpellToWalk',
    'RunToSpell', 'SpellToRun',
    'TurnToHit', 'HitToTurn',
    'TurnToSpell', 'SpellToTurn',
    'CutMeatToHit', 'CutMeatToSpell',
    'MoveToTurn',   'TurnToMove',
    'MoveToCutMeat', 'CutMeatToMove',
    'HitConcurrent', 'SpellConcurrent',
    'MoveConcurrent' {, 'AllConcurrent'}
    );

  //  网关插件相关
{$IF CLIENT_ANTIPLUG = 1}
type
  PRunGatePlugClientInfo = ^TRunGatePlugClientInfo;
  TRunGatePlugClientInfo = packed record
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

    IsActive: Boolean;                                 // 是否为活动连接
    IsLoginNotice: Boolean;                            // 客户端点击公告确定
    IsPlayGame: Boolean;                               // 背包，技能等均初始化完成，进入游戏中

    DataAdd: Pointer;                               // 附加数据
    DataLen: LongWord;                              // 附加数据长度

    VerInfo: LongWord;                            //版本信息
    Reseved2: array[0..28] of LongWord;
  end;

  TRunGatePlugGetClientInfoFunc = function(ClientID: Integer; ClientInfo: PRunGatePlugClientInfo): BOOL; stdcall;
  TRunGatePlugAddMainLogMsgFunc = procedure(lpData: PChar; nLevel: Integer); stdcall;
  TRunGatePlugSendSocketFunc = procedure(ClientID: Integer; DefMsg: pTDefaultMessage; lpData: PChar; DataLen: Integer); stdcall;
  TRunGatePlugEncodeDecodeFunc = function(lpInData: PChar; InDataLen: Integer; lpOutData: PChar; var OutDataLen: Integer): BOOL; stdcall;
  TRunGatePlugCloseClientFunc = procedure(ClientID: Integer; DelayTime: LongWord{延时时间，秒}; Code: Integer{关闭代码}); stdcall;
  TRunGatePlugLockClient = procedure(ClientID: Integer; LockTime: LongWord); stdcall;
  TRunGatePlugSetClientPlugLoad = procedure(ClientID: Integer); stdcall;

  PPlugInitRecord = ^TPlugInitRecord;
  TPlugInitRecord = packed record
    MainFormHandle: HWND;

    GetClientInfo: TRunGatePlugGetClientInfoFunc;             // +++++++++++++++获取客户端连接的信息
    AddMainLogMsg: TRunGatePlugAddMainLogMsgFunc;             // +++++++++++++++在网关上显示日志

    SendDataToClient: TRunGatePlugSendSocketFunc;             // 发送数据到客户端

    EncodeBuffer: TRunGatePlugEncodeDecodeFunc;               // 将非可视字符编码为可视字符
    DecodeBuffer: TRunGatePlugEncodeDecodeFunc;               // 将非可视字符编码为可视字符

    CloseClient: TRunGatePlugCloseClientFunc;                 // +++++++++++++++关闭内核连接
    LockClient: TRunGatePlugLockClient;                       // 锁定用户，时间秒

    SetClientPlugLoad: TRunGatePlugSetClientPlugLoad;         // 通知客端反外挂模块加载成功
  end;

  //
  TRunGatePlugDoInit = function(InitRecord: PPlugInitRecord; IsReload: BOOL): BOOL; stdcall;
  TRunGatePluginDoUninit = procedure(); stdcall;
  TRunGatePlugContextFunc = procedure(ClientID: Integer); stdcall;
  TRunGatePlugRecvPacketFunc = procedure(ClientID: Integer; DefMsg: pTDefaultMessage; lpData: PChar; DataLen: Integer; var IsSendToM2: BOOL); stdcall;
  TRunGatePlugShowConfigForm = procedure; stdcall;

var
  g_ClientAntiPlugDllBlockSize: Integer = 0;
  g_ClientAntiPlugDllSendInterval: Integer = 0;               // 插件发送间隔

  g_ClientAntiPlugDllSize: Integer;
  g_ClientAntiPlugVersion: LongWord = 0;
  g_ClientAntiPlugDllString: string;
  g_ClientAntiPlugDllStringCRC: LongWord = 0;
  g_ClientAntiPlugDllBlockCount: Integer = 0;

  g_ClientAntiPlugStream: TMemoryStream;

  g_boAntiPlugAutoUpdateCheck: Boolean = False;
  g_wAntiPlugUpdateCheckInterval: Word = 5;
  g_dwAntiPlugUpdateCheckTick: LongWord;
  g_sAntiPlugUpdateConfigUrl: string = ''; // 'http://121.40.103.14:85/pluginfo.txt';

  g_CSRunGatePlug: TRTLCriticalSection;
  g_sRunGatePlusDllName: string = 'RunGatePlug.dll';
  g_RunGatePlugDllHandle: THandle = 0;
  g_rgpDoInit: TRunGatePlugDoInit = nil;
  g_rgpDoUninit: TRunGatePluginDoUninit = nil;
  g_rgpStartContext: TRunGatePlugContextFunc = nil;
  g_rgpEndContext: TRunGatePlugContextFunc = nil;
  g_rgpRecvPacket: TRunGatePlugRecvPacketFunc = nil;

  g_rgpShowConfigForm: TRunGatePlugShowConfigForm = nil;
{$IFEND}

var
  g_DefaultConfig: TAntiPlugConfig;

  g_LockUserList: TSafeHashStringList;

  g_VerifyFailUserList: TSafeHashStringList;

  g_Config: TAntiPlugConfig = (
    ActionList:(                                    
      (                                             // 攻击
        boEnabled: False;
        nInterval: 0;
        ProcessMode: apmFakeAttackPass;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【攻击】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 魔法
        boEnabled: False;
        nInterval: 1200;
        ProcessMode: apmFakeAttackPass;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【魔法】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 走路
        boEnabled: False;
        nInterval: 540;
        ProcessMode: apmRebound;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【走路】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 跑步
        boEnabled: False;
        nInterval: 540;
        ProcessMode: apmRebound;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【跑步】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 转向
        boEnabled: False;
        nInterval: 100;
        ProcessMode: apmRebound;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【转向】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 挖肉
        boEnabled: False;
        nInterval: 620;
        ProcessMode: apmRebound;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【挖肉】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 走路到攻击
        boEnabled: False;
        nInterval: 540;
        ProcessMode: apmFakeAttackPass;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【走动攻击】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),                                            // 攻击到走路
      (
        boEnabled: False;
        nInterval: 600;
        ProcessMode: apmRebound;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【攻击走动】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 跑步到攻击
        boEnabled: False;
        nInterval: 540;
        ProcessMode: apmFakeAttackPass;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【跑动攻击】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),                                            // 攻击到跑步
      (
        boEnabled: False;
        nInterval: 600;
        ProcessMode: apmRebound;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【攻击跑动】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 走路到魔法
        boEnabled: False;
        nInterval: 540;
        ProcessMode: apmFakeAttackPass;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【走动魔法】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 魔法到走路
        boEnabled: False;
        nInterval: 1200;
        ProcessMode: apmRebound;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【魔法走动】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 跑步到魔法
        boEnabled: False;
        nInterval: 540;
        ProcessMode: apmFakeAttackPass;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【跑动魔法】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 魔法到跑步
        boEnabled: False;
        nInterval: 1200;
        ProcessMode: apmRebound;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的【魔法跑动】速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 转向到攻击
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 攻击到转向
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 转向到魔法
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 魔法到转向
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 挖肉到攻击
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 挖肉到魔法
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 移动到转向
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 转向到移动
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 移动到挖肉
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 挖肉到移动
        boEnabled: False;
        nInterval: 250;
        ProcessMode: apmOffline;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 15;
        //nCollectSpeedCount: 9;
        //nFloatingInterval: 200;
        boShowHint: False;
        sHintText: '[提示]: 您的游戏速度出现异常';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 攻击并发
        boEnabled: False;
        nInterval: 1;
        ProcessMode: apmFakeAttackPass;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 1;
        //nCollectSpeedCount: 1;
        //nFloatingInterval: 0;
        boShowHint: False;
        sHintText: '[提示]: 请爱护游戏环境，关闭加速外挂重新登陆';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 魔法并发
        boEnabled: False;
        nInterval: 1;
        ProcessMode: apmFakeAttackPass;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 1;
        //nCollectSpeedCount: 1;
        //nFloatingInterval: 0;
        boShowHint: False;
        sHintText: '[提示]: 请爱护游戏环境，关闭加速外挂重新登陆';
        nCompensationValue: 0;
        boDebug: False;
      ),
      (                                             // 移动并发
        boEnabled: False;
        nInterval: 1;
        ProcessMode: apmFakeAttackPass;
        boProcessScript: False;
        SumProcessMode: sapmNone;
        //nCollectCount: 1;
        //nCollectSpeedCount: 1;
        //nFloatingInterval: 0;
        boShowHint: False;
        sHintText: '[提示]: 请爱护游戏环境，关闭加速外挂重新登陆';
        nCompensationValue: 0;
        boDebug: False;
      ){,
      (                                             // 所有并发
        boEnabled: False;
        nInterval: 1;
        ProcessMode: apmLost;
        SingleMode: asmPause;
        boShowHint: True;
        sHintText: '提示]: 请爱护游戏环境，关闭加速外挂重新登陆';
        boDebug: False;
      )}

    );

    btMsgType: 0;
    btMsgFColor: $FF;
    btMsgBColor: $38;

    nLockTime: 5;
    boSaveLockStatus: True;
    boShowLockLog: False;
    sShowLockMsg: '超速已经被锁定(原因：%s)，%d秒后自动解锁！';

    boSpeedClearData: False;

    dwUserShop_Search_Interval: 200;
    boUserShop_Search_ShowHint: True;

    dwUserShop_Buy_Interval: 200;                   
    boUserShop_Buy_ShowHint: True;

    dwTakeOn_Item_Interval: 200;                    
    boTakeOn_Item_ShowHint: True;

    dwDealTry_Attack_Interval: 1500;
    boDealTry_Attack_ShowHint: True;

    dwBrutal_Attack_Interval: 700;
    boBrutal_Attack_ShowHint: True;

    boShowAttackLog: False;
    //boShowDropConcurrentLog: True;

    dwContinueSpeedPassIncTime: 30;

    boContinueSpeedCloseSocket: False;
    nContinueSpeedCount: 4;

    nSumSpeedCheckTime: 20;                             // 累计超速统计时间间隔
    nSumSpeedMaxCount: 5;                               // 累计超速最大超速次数

    dwCollectCount: 15;
    dwSpeedValue: 9;

    boZeroCompensationValueClearPool: False;            // 0补偿时清补偿池
    dwClientUploadPickItemsTime: 30;
    );
//ricky   获取百度时间  去掉   这个是验证时间用的
//{$IF NEED_REGISTER = 1}
//  g_dwBaiduTimeTick: LongWord = 0;
//  g_dwGetBaiduTimeTime: LongWord = 0;
//{$IFEND}

  g_sIniFileName: string;

  g_sActionIntervalsFileNames: array[TAntiPlugActionMode] of string = (
    'HitIntervals.ini',                   // 攻击
    'SpellIntervals.ini',                 // 魔法
    'WalkIntervals.ini',                  // 走路
    'RunIntervals.ini',                   // 跑步
    '',                                   // 转向
    '',                                   // 挖肉
    'WalkToHitIntervals.ini',             // 走路到攻击间隔
    'HitToWalkIntervals.ini',             // 攻击到走路间隔
    'RunToHitIntervals.ini',              // 跑步到攻击间隔
    'HitToRunIntervals.ini',              // 攻击到跑步间隔
    'WalkToSpellIntervals.ini',           // 走路到魔法间隔
    'SpellToWalkIntervals.ini',           // 魔法到走路间隔
    'RunToSpellIntervals.ini',            // 跑步到魔法间隔
    'SpellToRunIntervals.ini',            // 魔法到跑步间隔
    'TurnToHitIntervals.ini',             // 转向到攻击间隔
    '',                                   // 攻击到转向
    'TurnToSpellIntervals.ini',           // 转向到魔法间隔
    '',                                   // 魔法到转向
    'CutMeatToHitIntervals.ini',          // 挖肉到攻击间隔
    'CutMeatToSpellIntervals.ini',        // 挖肉到魔法间隔
    '',                                   // 移动到转向
    'TurnToMoveIntervals.ini',            // 转向到移动间隔
    '',                                   // 移动到挖肉
    'CutMeatToMoveIntervals.ini',         // 挖肉到移动间隔
    '',                                   // 攻击并发
    '',                                   // 魔法并发
    ''                                    // 移动并发
    );

  g_wActionSpeedIntervals: array[TAntiPlugActionMode] of TSpeedIntervals;

  g_boSendSpeedIntervalsToClient: array[TAntiPlugActionMode] of Boolean;

  g_SendToClientSpeedIntervalsText: string = '';
  g_SendToClientSpeedIntervalsLen: Integer = 0;
//ricky   去掉   百度时间
//{$IF NEED_REGISTER = 1}
//  g_dtBaiduTime: TDateTime = 0;
//{$IFEND}

  g_sWordFilterFileName: string;

  g_MainLogStrings: TSafeStringList;
  g_IOCPLogStrings: TSafeStringList;

  g_WordFilterList: TSafeHashStringList;                              // 默认说话过滤表
//Ricky   去掉
//{$IF NEED_REGISTER = 1}
//  g_nKeyLastDay1: Integer = 0;
//{$IFEND}

  g_CurrIPList: TAddressListEx;                                       // 当前连接的IP
  g_TempIPList: TAddressList;                                         // 临时禁止连接IP列表
  g_BlockIPList: TAddressList;                                        // 禁止连接IP列表
  g_IPSectionList: TSafeList;                                         // 过滤IP段
  g_AttackIPaddrList: TAddressList;                                   // 攻击IP临时列表

  g_TempMacList: TSafeStringList;
  g_BlockMacList: TSafeStringList;

  g_ProcessBlacklist: TProcessBlacklist;
  g_ProcessBlacklistStr: string;
  g_ProcessBlacklistMD5: MD5Digest;

  g_ScreenshotPath: string;

  g_sTitleName: string = '游戏网关';
  g_sServerAddr: string = '127.0.0.1';
  g_wdServerPort: Word = 5000;
  g_sGateAddr: string = '0.0.0.0';
  g_wdGatePort: Word = 7200;

  g_boOpenVerifyCode: Boolean = False;
  g_nVerifyCodeErrCount: Integer = 3;
  g_nVerifyCodeRefreshCount: Integer = 4;
  g_nVerifyCodeWaitTime: Integer = 60;
  g_dwVerifyCodeInterval1: LongWord = 30;
  g_dwVerifyCodeInterval2: LongWord = 50;
  g_dwVerifySuccessAddInterval: LongWord = 0;
  g_boVerifyFailTriggerScript: Boolean = False;
  g_boVerifyFailLoginVerify: Boolean = False;
  g_boVerifyCodeExcludeMap: Boolean = True;
  g_sVerifyCodeExcludeMapFileName: string = '';
  g_VerifyCodeMapList: TSafeHashStringList;

  g_boAutoLoadNoVerifyChrList: Boolean = False;           // 自动加载免验证角色表
  g_sLoadNoVerifyChrListFile: string = 'D:\MirServer\Mir200\Envir\QuestDiary\白名单用户.txt';
  g_nAutoLoadNoVerifyChrListInterval: Integer = 300;
  g_dwAutoLoadNoVerifyChrListTick: LongWord;
  g_LoadNoVerifyChrList: TSafeHashStringList;
//ricky 去掉
//{$IF NEED_REGISTER = 1}
//  g_dtBaiduTime2: TDateTime = 0;
//{$IFEND}
  g_wdDBPort: Word = 27201;

  g_btShowLogLevel: Byte = 3;

  g_nRecvAntiPlugHeartbeatTimeOutTime: Integer = 25;
  g_nAntiPlugStreamSendSpeed: Integer = 2;
  g_nAntiPlugStreamSendBlockSize: Integer = 2;
  g_boLogoutNoResendAntiplugStream: Boolean = False;
  g_boAntiplugAllLog: Boolean = False;

  g_boOneMACLimitePlayer: Boolean = False;
  g_nOneMACLimitePlayerCount: Integer = 3;
  g_LoginMACPlayerList: TSafeHashStringList;

  g_nClientLogoutDelay: Integer = 0;
  g_nClientCloseDelay: Integer = 0;

  g_boDelayCloseDisableMove: Boolean = False;
  g_boDelayCloseDisableSpell: Boolean = False;
  g_boDelayCloseDisableAttack: Boolean = False;
  g_boDelayCloseDisableUseItem: Boolean = False;

  g_boBreakClientLogoutHint: Boolean = True;
  g_sBreakClientLogoutHint: string = '小退游戏操作已被中断';

  g_boBreakClientCloseHint: Boolean = True;
  g_sBreakClientCloseHint: string = '大退游戏操作已被中断';

  g_boMinimize: Boolean = True;

  // 增加网关密码控制 piaoyun 2013-08-28
  g_boCheckClientPassword: Boolean = False;
  g_sClientPassWord: string = 'BmM2';                                // 2009-5-26 Micro

  g_dwCheckServerTimeOutTime: LongWord = 300;                         // 网关 <->游戏服务器之间检测超时时间

  g_dwClientAccumulateMaxSize: Integer = 600;                         // 客户端数据堆积大小

  g_boLogClientPacket: Boolean = False;                               // 记录客户端封包 chongchongn 2016-08-03
  g_sLogClientPacketDir: string;
  g_sLogClientPakcetUserFile: string;
  g_nLogClientPacketType: Integer = 0;
  g_LogClientPacketUser: TSafeHashStringList;

  g_sPluginDir: string;

  g_sReplaceWord: Char = '*';

  g_nMaxConnOfIPaddr: Integer = 50;
  g_nMaxClientPacketSize: Integer = 512;
  g_nMaxClientPacketCount: Integer = 200;
//ricky 这个后面可以用来匹配特定的登录器
//{$IF NEED_REGISTER = 1}
//  g_GameLoginConfigMD51: LongWord = 0;
//{$IFEND}
  g_GameLoginConfigMD51: LongWord = 0;
  nMaxClientMsgCount: Integer = 100;

  g_dwAttackTick: LongWord = 300;
  g_nAttackCount: Integer = 5;
  g_dwGameCenterHandle: THandle;

  g_BlockMethod: TBlockIPMethod = bmDisconnect;
  g_boKickOverPacketSize: Boolean = True;

  g_boCheckClientPacketLegal: Boolean = True;
  g_nCheckClientPacketCount: Integer = 3;

  // chongchong 2013-08-31  ~ 2013-09-01
  g_dwKeepConnectTimeOut: LongWord = 30;                              // 空连接超时
  g_boFilterSayMsg: Boolean = True;                                   // 是否开启消息过滤
  g_FilterSayMsgMode: TFilterSayMsgMode = fsmmDisMsg;                 // 消息过滤模式
  g_WarnSayMsg: string = '您发送的信息里包含了非法字符。';            // 警告信息文字

  g_boFilterSayTriggerScript: Boolean = False;

  g_sDisableSayMsg: string = '禁止聊天';                                      // 禁言时间内提示
  g_sDisableSayMsgBegin: string = '由于您说话太快，%d秒内禁止聊天！！！';     // 开始禁言提示
//ricky  后面可以用来匹配特定的登录器
//{$IF NEED_REGISTER = 1}
//  g_GameLoginConfigMD52: LongWord = 0;
//{$IFEND}
  g_GameLoginConfigMD52: LongWord = 0;
  g_boSayMsgControl: Boolean = False;                                 // 是否开启发言控制
  g_dwSayMaxLen: LongWord = 70;                                       // 发言文字最大长度
  g_dwSayTime: LongWord = 3000;                                       // 发言时间间隔(毫秒)
  g_dwSayMaxCount: LongWord = 2;                                      // 发言次数
  g_dwSayDisableTime: LongWord = 10;                                  // 禁言时间(秒)

  g_dwIPCountLimitTime1: LongWord = 1000;
  g_dwIPCountLimit1: LongWord = 20;
  g_dwIPCountLimitTime2: LongWord = 3000;
  g_dwIPCountLimit2: LongWord = 40;

  g_dwDefenseLevel: LongWord = 1;                                     // 防御等级
//ricky   后面可以用来匹配特定的登录器
//{$IF NEED_REGISTER = 1}
//  g_GameLoginConfigMD53: LongWord = 0;
//{$IFEND}
  g_GameLoginConfigMD53: LongWord = 0;
  g_boDefenseToLevel1: Boolean = True;                                // 受攻击防御调为1级
  g_dwDefenseToLevel1: LongWord = 3;                                  // 受攻击防御调为1级 (攻击次数)
  g_boResotreDefense: Boolean = True;                                 // 无攻击还原防御等级
  g_dwResotreDefense: LongWord = 120;                                 // 无攻击还原防御等级 (120秒后)
  g_boAutoClearTemp: Boolean = True;                                  // 清除动态过滤列表
  g_dwAutoClearTemp: LongWord = 120;                                  // 清除动态过滤列表 (自动清除间隔120秒)
  g_boAddAllToTemp: Boolean = False;                                  // 连接加入到动态过滤
  g_dwAddAllToTemp: LongWord = 2000;                                  // 连接加入到动态过滤 (连接数)

  g_boOpenCheckClient: Boolean = False;                               // 开启客户端验证
  g_CheckClientFailBlockMethod: TBlockIPMethod = bmTempBlock; 
//ricky   后面可以用来匹配特定的登录器用
//{$IF NEED_REGISTER = 1}
//  g_GameLoginConfigMD54: LongWord = 0;
//{$IFEND}
  g_GameLoginConfigMD54: LongWord = 0;
  // 防御设置 chongchong 2015-01-01
  g_sFYReadDenyIPFile: string = 'D:\MirServer\Mir200\Envir\QuestDiary\KickList.txt';
  g_dwFYReadDenyIPTime: LongWord = 6000;
  g_dwFYReadDenyIPTick: LongWord;
  g_FYDenyIPList: TAddressList;

  g_sFYReadPassIPFile: string = 'D:\MirServer\Mir200\Envir\QuestDiary\绿色通道.txt';
  g_dwFYReadPassIPTime: LongWord = 6000;
  g_dwFYReadPassIPTick: LongWord;
  g_FYPassIPList: TAddressList;

  g_sFYReadDenyMACFile: string = 'D:\MirServer\Mir200\Envir\DenyMachineIDList.txt';
  g_dwFYReadDenyMACTime: LongWord = 6000;
  g_dwFYReadDenyMACTick: LongWord;
  g_FYDenyMACList: TSafeHashStringList;

  g_sFYDownDenyIPUrl: string = 'http://www.bmm2.com/KickList.txt';
  g_dwFYDownDenyIPTime: LongWord = 60;
  g_dwFYDownDenyIPTick: LongWord;
  g_FYDownDenyIPList: TAddressList;

  g_sFYDownPassIPUrl: string = 'http://www.bmm2.com/绿色通道.txt';
  g_dwFYDownPassIPTime: LongWord = 60;
  g_dwFYDownPassIPTick: LongWord;
  g_FYDownPassIPList: TAddressList;

  g_sFYDownDenyMACUrl: string = 'http://www.bmm2.com/机器码.txt';
  g_dwFYDownDenyMACTime: LongWord = 60;
  g_dwFYDownDenyMACTick: LongWord;
  g_FYDownDenyMACList: TSafeHashStringList;

  g_OnlyWhiteListLink: Boolean = False;

  g_DBAddressList: TStringList;

  g_MagicCDListFileName: string;
  g_MagicCDList: TMagicIntervalList;

  g_boRequestMagicList: Boolean = False;
  g_MagicList: TList;

  g_btMagicCDMsgType: Byte = 0;
  g_sMagicCDMsgText: string = '技能尚未冷确，请等待%time秒';
  g_btMagicCDFColor: Byte = $FF;
  g_btMagicCDBColor: Byte = $38;
  g_nMagicCDShowX: Integer = 30;
  g_nMagicCDShowY: Integer = 40;

  g_EatItemCDConfig: TEatItemCDConfig;

const
  // 记录客户端封包类型
  CPT_MOVE  = 1;
  CPT_HIT   = 2;
  CPT_SPELL = 4;
  CPT_QUERY = 8;
  CPT_TEAM  = 16;
  CPT_GUILD = 32;
  CPT_SHOP  = 64;

implementation

uses
  //UnitDes {$IF NEED_REGISTER = 1}, WinlicenseSDK{$IFEND};
  UnitDes;
function ActionModeUseSpeedIntervals(ActionMode: TAntiPlugActionMode): Boolean;
begin
  Result := ActionMode in [
    amHit {攻击}, amSpell {魔法}, amWalk {走路},
    amRun {跑步}, //amTurn {转向},  amCutMeat {挖肉},
    amWalkToHit {走路到攻击},     amHitToWalk {攻击到走路},
    amRunToHit {跑步到攻击},      amHitToRun {攻击到跑步},
    amWalkToSpell {走路到魔法},   amSpellToWalk {魔法到走路},
    amRunToSpell {跑步到魔法},    amSpellToRun {魔法到跑步},
    amTurnToHit {转向到攻击},   (*amHitToTurn {攻击到转向},*)
    amTurnToSpell {转向到魔法}, (*amSpellToTurn {魔法到转向},*)
    amCutMeatToHit {挖肉到攻击},  amCutMeatToSpell {挖肉到魔法},
    (*amMoveToTurn {移动到转向},*)    amTurnToMove {转向到移动},
    (*amMoveToCutMeat {移动到挖肉},*) amCutMeatToMove {挖肉到移动}];
end;

procedure RebuildSendToClientSpeedIntervalsText;
var
  Buf: array[0..81920 - 1] of Byte;
  BufSize: Integer;
  PB: PByte;
  ActionMode: TAntiPlugActionMode;
begin
  PB := @Buf[0];
  Move(g_boSendSpeedIntervalsToClient, PB^, SizeOf(g_boSendSpeedIntervalsToClient));
  BufSize := SizeOf(g_boSendSpeedIntervalsToClient);
  Inc(PB, BufSize);
  for ActionMode := Low(TAntiPlugActionMode) to High(TAntiPlugActionMode) do
  begin
    if g_boSendSpeedIntervalsToClient[ActionMode] then
    begin
      Move(g_wActionSpeedIntervals[ActionMode], PB^, SizeOf(g_wActionSpeedIntervals[ActionMode]));
      Inc(BufSize, SizeOf(g_wActionSpeedIntervals[ActionMode]));
      Inc(PB, SizeOf(g_wActionSpeedIntervals[ActionMode]));
    end;
  end;

  g_SendToClientSpeedIntervalsLen := BufSize;
  g_SendToClientSpeedIntervalsText := zLibCompressBuffer(@Buf[0], BufSize);
end;

procedure AddTempBlockIP(sIPaddr: string);
begin
  g_TempIPList.Lock;
  try
    g_TempIPList.Add(sIPaddr);
  finally
    g_TempIPList.UnLock;
  end;
end;

procedure AddBlockIP(sIPaddr: string);
begin
  g_BlockIPList.Lock;
  try
     g_BlockIPList.Add(sIPaddr);
  finally
    g_BlockIPList.UnLock;
  end;
end;

procedure AddTempBlockMac(sMac: string);
begin
  g_TempMacList.Lock;
  try
    if g_TempMacList.IndexOf(sMac) < 0 then
      g_TempMacList.Add(sMac);
  finally
    g_TempMacList.UnLock;
  end;
end;

procedure AddBlockMac(sMac: string);
begin
  g_BlockMacList.Lock;
  try
    if g_BlockMacList.IndexOf(sMac) < 0 then
      g_BlockMacList.Add(sMac);
  finally
    g_BlockMacList.UnLock;
  end;
end;

procedure AddMainLogMsg(Msg: string; nLevel: Integer; AddTime: Boolean = True);
var
  sMsg: string;
begin
  try
    g_MainLogStrings.Lock;
    if nLevel <= g_btShowLogLevel then
    begin
      if AddTime then
        sMsg := '[' + TimeToStr(Now) + '] ' + Msg
      else
        sMsg := Msg;
      g_MainLogStrings.Add(sMsg);
    end;
  finally
    g_MainLogStrings.UnLock;
  end;
end;

/// <summary>
///   计算两个TickCount时间差，避免超出49天后，溢出
///      感谢 [佛山]沧海一笑  7041779 提供
///      copy自 qsl代码
/// </summary>
function tick_diff(tick_start, tick_end: Cardinal): Cardinal;
begin
  if tick_end >= tick_start then
    result := tick_end - tick_start
  else
    result := High(Cardinal) - tick_start + tick_end;
end;

procedure AddIOCPLogMsg(Msg: string);
var
  sMsg: string;
begin
  g_IOCPLogStrings.Lock;
  try
    sMsg := '[' + TimeToStr(Now) + '] ' + Msg;
    g_IOCPLogStrings.Add(sMsg);
  finally
    g_IOCPLogStrings.UnLock;
  end;
end;

{$IF VERSION_TYPE = 1}
// 添加默认过滤列表 chongchong 2013-11-11
procedure AddToDefFilterSayMsgList;
const
  DefStrs: array[0..64] of string = (
    'М', 'ω', 'С', 'Ａ', 'Ｂ', 'Ｃ','ш', 'щ', 'щ', 'Ａ', 'Ｄ', 'Ｅ', 'Ｆ',
    'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', 'ｌ', 'ｍ', 'ｎ',
    'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｗ', 'ｘ', 'ｙ', 'ｚ',
    'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', 'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ',
    'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', 'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ', 'Ｚ',
    'c', 'w'
  );
var
  I: Integer;
begin
  g_WordFilterList.Lock;
  try
    for I := Low(DefStrs) to High(DefStrs) do
    begin
      if g_WordFilterList.IndexOf(DefStrs[I]) = -1 then
        g_WordFilterList.Add(DefStrs[I])
    end;
  finally
    g_WordFilterList.UnLock;
  end;
end;

function CheckInWhiteList(S: string): Boolean;
var
  I: Integer;
  DecStr: string;
  WhiteListStr: array[0..9] of string;
begin
{$I StrEncrypt_Start.inc} 
  WhiteListStr[0] := 'P@E]ICdqYbuTVs=FUPHpYqYCM`aIMCLtOrioMBuLXqIgJRuKTQ`gP?MbNpqqR@eDQ@AoQPasQBedMPLqMQXpHoMcKOp';                                                       //'【好登陆器】最强大的万能登陆器下载:www.haodlq.Com',
  WhiteListStr[1] := 'P@E]ICdqYbuTVs=FUPHpYqYCM`iHLoMpQpYCOPmOTbQgY`IDY?LgUOLtNrIrU^ynIpe?URmeHCA`Qo=BZ`aSNoTkNqHsO^yOVrXyKL';                                            //'【好登陆器】最牛、最好用的传奇万能登陆器:www.haodlq.Com',
  WhiteListStr[2] := 'Pr\mL_\tQAYHZ?AJORihI@mfLaaUQRYsOoIuO?AIIcQcXa`lY^yfH^y`PBxmXAYPV`yuOsPoLPesHSIiWrUcI?AoN@UcIA<pJSUvV<';                                            //'【78Pk登陆器】最强大、最好用的传奇万能登陆器:www.78Pk.com',
  WhiteListStr[3] := 'PraaFsM?LS=aVOAJORihI@mfM?HkIoTtOricFs@sXBeoINibLry@NruITPueWpIEZQeFLpy?PBdlMBmgY?A@NOYMKOp';                                                       //'【xp13登陆器】xp13登陆器官方网站：www.xp13dlq.Com',
  WhiteListStr[4] := 'PBXlXOanRPacQBioNQItOPmALSIMXO]qOPUeXQEURC=IMO]sUNieQb\pX`ifU?UCToQrHrqoXoahN@q>IbaPY@QeYreJLaU?N@UGX`yAHsMjGqMUOoU=LsMHW?aMPOpy';                  //'【１ＰＫ．ＣＯＭ 】最新最全的传奇万能登陆器、辅助工具下载：www.1Pk.Com',
  WhiteListStr[5] := 'Fo`oYsTuRNycNpMOPQIQHbE@Y?]TMPU=OpQELaURHqYuH?YNWSeuP_PgMpHtJRUlVAeLHbe^XoMkIs\pHOUENRugOSeTQ@`gNaHmR`efMa=aJRe^ZQEFHrukNC=JMQEvLO<qOpAvX@eTLOpy';  //'[1Pk.Com]为广大游戏玩家提供最新最全的游戏发布信息以及万能登陆器下载:www.1pk.com'
  WhiteListStr[6] := 'RQeSVOYmFrXqYQTpHBmdV@uQYsQHQb]dPpM>O_EiRBduIPISPpmQXoYIXcemYQ=bW`loJO`mIaImIbI]JPERRbQJPa=_NQM`WA=DVoAFOqMoNCECIPALNL';                            //[新登陆器]最好、最强大、最权威传奇万能登陆器下载网：www.xindlq.Com
  WhiteListStr[7] := 'MOXuPRyOTrU^Oo]uPq]cYPqBVB]TH?AURQQbURefYNy_VO=gIsUSOC]aV`]hI`yHWA`oU_QKYCQvXPhmNb]^UriaUO=tWqLpZbXyKL';                                            //【AAA登陆器】最新最全的万能登陆器下载站：www.aaadlq.Com
  WhiteListStr[8] := 'UqMgLcHlXQaGPaUFUPHpYqYCM`]FH@etOrirL_apOpprZCUAMcY=WoLtNrIrU`mcUpqvJCUIVqYeQAYRQa=EORLmFqPsOrERVoLkVryEHaIdHS=F';                                  //【找登陆器】最权威的资质高的IP万能登陆器资源站：www.zhaodlq.Com
  WhiteListStr[9] := 'HRaOMRqrMrmfIsMdR?<mRQaQTriKVsUCHsduWaEeI`e]Xs]lYqdkWoXgXbMeYCaVJBECWqIIQC`gMcUbLsPtV<';                                                             //【1Pk】最新最全的IP版传奇私服发布网：www.1Pk.Com
{$I StrEncrypt_End.inc} 

  Result := False;
  for I := Low(WhiteListStr) to High(WhiteListStr) do
  begin
    DecStr := DecryString_LF(WhiteListStr[I]);
    if SameText(S, DecStr) or SameText(S, '!' + DecStr) then
    begin
      Result := True;
      Break;
    end;
  end;
end;
{$IFEND}

procedure LoadFilterSayMsgFile();
begin
  if FileExists(g_sWordFilterFileName) then
  begin
    g_WordFilterList.Lock;
    try
      g_WordFilterList.LoadFromFile(g_sWordFilterFileName);
    finally
      g_WordFilterList.UnLock;
    end;
  end;
{$IF VERSION_TYPE = 1}
  AddToDefFilterSayMsgList;
{$IFEND}
  AddMainLogMsg('加载文字过滤信息完成', 4);
end;

function ReadFYDenyIPListFile(): Boolean;
var
  I: Integer;
  sFileName: string;
  LoadList: TStringList;
  sIPaddr: string;
begin
  Result := False;
  sFileName := g_sFYReadDenyIPFile;
  if not FileExists(sFileName) then Exit;
  g_FYDenyIPList.Clear;

  LoadList := TStringList.Create;
  try
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sIPaddr := Trim(LoadList.Strings[I]);
      if not IsIpaddr(sIPaddr) then Continue;
      g_FYDenyIPList.Add(sIPaddr);
    end;
    Result := True;
  finally
    LoadList.Free;
  end;
end;

function ReadFYPassIPListFile(): Boolean;
var
  I: Integer;
  sFileName: string;
  LoadList: TStringList;
  sIPaddr: string;
begin
  Result := False;
  sFileName := g_sFYReadPassIPFile;
  if not FileExists(sFileName) then Exit;
  g_FYPassIPList.Clear;

  LoadList := TStringList.Create;
  try
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sIPaddr := Trim(LoadList.Strings[I]);
      if not IsIpaddr(sIPaddr) then Continue;
      g_FYPassIPList.Add(sIPaddr);
    end;
    Result := True;
  finally
    LoadList.Free;
  end;
end;

function ReadFYDenyMACListFile(): Boolean;
var
  I: Integer;
  sFileName: string;
  LoadList: TStringList;
  sIPaddr: string;
begin
  Result := False;
  g_FYDenyMACList.Clear;

  sFileName := g_sFYReadDenyMACFile;
  if not FileExists(sFileName) then Exit;

  LoadList := TStringList.Create;
  try
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sIPaddr := Trim(LoadList.Strings[I]);
      g_FYDenyMACList.Add(sIPaddr);
    end;
    Result := True;
  finally
    LoadList.Free;
  end;
end;

procedure LoadBlockIPFile();
var
  I: Integer;
  sFileName: string;
  LoadList: TStringList;
  sIPaddr: string;
begin
  sFileName := ExtractFilePath(ParamStr(0)) + 'BlockIPList.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sIPaddr := Trim(LoadList.Strings[I]);
      g_BlockIPList.Add(sIPaddr);
    end;
    LoadList.Free;
  end;
  AddMainLogMsg('加载IP过滤列表完成', 4);
end;

procedure SaveBlockIPList();
var
  I: Integer;
  SaveList: TStringList;
begin
  SaveList := TStringList.Create;
  for I := 0 to g_BlockIPList.Count - 1 do
  begin
    SaveList.Add(StrPas(inet_ntoa(TInAddr(g_BlockIPList.Items[I].nIPaddr))));
  end;

  SaveList.SaveToFile(ExtractFilePath(ParamStr(0)) + 'BlockIPList.txt');
  SaveList.Free;
end;

procedure LoadBlockMacFile();
var
  sFileName: string;
begin
  sFileName := ExtractFilePath(ParamStr(0)) + 'BlockMacList.txt';
  if FileExists(sFileName) then
  begin
    g_BlockMacList.Lock;
    try
      g_BlockMacList.LoadFromFile(sFileName);
    finally
      g_BlockMacList.UnLock;
    end;
  end;
  AddMainLogMsg('加载Mac过滤列表完成', 4);
end;

procedure SaveBlockMacList();
begin
  g_BlockMacList.SaveToFile(ExtractFilePath(ParamStr(0)) + 'BlockMacList.txt');
end;

procedure LoadIPSectionList();
var
  I, Index: Integer;
  sFileName: string;
  LoadList: TStringList;
  S, IP1, IP2: string;
  nBeginaddr, nEndaddr: LongWord;
  IPSection: PTIPSection;
begin
  g_IPSectionList.Lock;
  try
    for I := 0 to g_IPSectionList.Count - 1 do
      Dispose(PTIPSection(g_IPSectionList.Items[I]));
    g_IPSectionList.Clear;
  finally
    g_IPSectionList.UnLock;
  end;

  sFileName := ExtractFilePath(ParamStr(0)) + 'IPSectionList.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      S := Trim(LoadList.Strings[I]);
      if Length(S) = 0 then Continue;

      Index := Pos(#8, S);
      if Index <= 1 then Continue;

      IP1 := Copy(S, 1, Index - 1);
      IP2 := Copy(S, Index + 1, MaxInt);

      nBeginaddr := IP2Long(IP1);
      nEndaddr := IP2Long(IP2);

      g_IPSectionList.Lock;
      try
        if (nBeginaddr <> LongWord(INADDR_NONE)) and (nEndaddr <> LongWord(INADDR_NONE)) and (nBeginaddr <= nEndaddr) then
        begin
          New(IPSection);
          IPSection.nBeginAddr := nBeginaddr;
          IPSection.nEndAddr := nEndaddr;
          g_IPSectionList.Add(IPSection)
        end;
      finally
        g_IPSectionList.UnLock;
      end;
    end;
    LoadList.Free;
  end;
  AddMainLogMsg('加载IP段过滤列表完成', 4);
end;

procedure SaveIPSectionList();
var
  I: Integer;
  IP1, IP2: string;
  IPSection: PTIPSection;
  SaveList: TStringList;
begin
  SaveList := TStringList.Create;

  g_IPSectionList.Lock;
  try
    for I := 0 to g_IPSectionList.Count - 1 do
    begin
      IPSection := g_IPSectionList.Items[I];
      IP1 := Long2IP(IPSection.nBeginAddr);
      IP2 := Long2IP(IPSection.nEndAddr);

      SaveList.Add(IP1 + #8 + IP2);
    end;
  finally
    g_IPSectionList.UnLock;
  end;

  SaveList.SaveToFile(ExtractFilePath(ParamStr(0)) + 'IPSectionList.txt');
  SaveList.Free;
end;

procedure LoadDBAddressTable();
var
  I: Integer;
  LoadList: TStringList;
  FileName, sLineText: string;
begin
  g_DBAddressList.Clear;
  FileName := ExtractFilePath(ParamStr(0)) + '!addrtable.txt';
  if FileExists(FileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(FileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        sLineText := Trim(LoadList.Strings[I]);
        if (sLineText <> '') and (sLineText[1] <> ';') then
        begin
          if g_DBAddressList.IndexOf(sLineText) < 0 then
            g_DBAddressList.Add(sLineText);
        end;
      end;
    finally
      LoadList.Free;
    end;
  end;
end;

function IsHexString(S: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  if Length(S) = 0 then Exit;

  for I := 1 to Length(S) do
  begin
    if not (S[I] in ['0'..'9', 'A'..'F', 'a'..'f']) then
    begin
      Exit;
    end;
  end;

  Result := True;
end;

function StrToHexEx(S: AnsiString): AnsiString;
var
  I: Byte;
  C: Byte;
const
  Digits: array[0..15] of AnsiChar =
  ('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F');
begin
  Result := '';
  for I := 1 to Length(S) do
  begin
    C := Ord(S[I]);
    Result := Result + Digits[(C shr 4) and $0F] + Digits[C and $0F];
  end;
end;

function HexToStrEx(const StrHex: AnsiString; var OutStr: AnsiString): Boolean;
var
  I, Len: Integer;
  C1, C2: AnsiChar;
  TempStr: AnsiString;
begin
  Result := False;
  if Length(StrHex) mod 2 <> 0 then Exit;

  Len := Length(StrHex) div 2;
  SetLength(TempStr, Len);
  for I := 1 to Len do
  begin
    C1 := StrHex[I * 2 - 1];
    C2 := StrHex[I * 2];
    if (C1 in ['0'..'9', 'a'..'f', 'A'..'F']) and (C2 in ['0'..'9', 'a'..'f', 'A'..'F']) then
    begin
      TempStr[I] := AnsiChar(StrToInt('$' + C1 + C2))
    end
    else
      Exit;
  end;

  OutStr := TempStr;
  Result := True;
end;

procedure LoadProcessBlacklist();
var
  I, Index: Integer;
  LoadList: TStringList;
  FileName, sLineText: string;
  ProcessName, ProcessMD5: string;
begin
  g_DBAddressList.Clear;
  FileName := ExtractFilePath(ParamStr(0)) + 'ProcessBlacklist.txt';
  if FileExists(FileName) then
  begin
    g_ProcessBlackList.Lock;
    
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(FileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        sLineText := Trim(LoadList.Strings[I]);
        if (sLineText <> '') and (sLineText[1] <> ';') then
        begin
          Index := Pos('|', sLineText);
          if Index > 0 then
          begin
            ProcessName := Copy(sLineText, 1, Index - 1);
            ProcessMD5 := Copy(sLineText, Index + 1, MaxInt);

            if (Length(ProcessMD5) = 32) and IsHexString(ProcessMD5) then
            begin
              g_ProcessBlackList.Add(ProcessName, ProcessMD5);
            end;
          end;
        end;
      end;

      RebuildProcessBlacklist;
    finally
      LoadList.Free;
      g_ProcessBlackList.UnLock;
    end;
  end;
end;

procedure RebuildProcessBlacklist;
var
  I, Count, OutBufSize: Integer;
  RSA: TLbRSA;
  MD5: MD5Digest;
  ProcessInfo: PTProcessInfo;
  Buf: array[0..2048] of Byte;
  OutBuf: array[0..8192] of Byte;
  P: PByte;
begin
  RSA := TLbRSA.Create(nil);
  try
    P := @Buf[0];
    Count := 0;
    for I := 0 to g_ProcessBlackList.Count - 1 do
    begin
      ProcessInfo := g_ProcessBlackList.Items[I];

      if StrToMD5Digest(ProcessInfo.ProcessMD5, MD5) then
      begin
        Move(MD5[0], P^, SizeOf(MD5));
        Inc(P, SizeOf(MD5));
        Inc(Count);

        if Count >= 80 then Break;
      end;
    end;

    if Count > 0 then
    begin
      RSA.KeySize := aks128;
      RSA.PublicKey.ModulusAsString := '597A185BA5F22A014F50B453E647C0C5';
      RSA.PublicKey.ExponentAsString := 'CF2C34C6204626E70E493F5B37930363';

      OutBufSize := RSA.EncryptBuffer(Buf[0], Count * SizeOf(MD5), OutBuf[0]);      g_ProcessBlacklistStr := zLibCompressBuffer(PChar(@OutBuf[0]), OutBufSize);
      g_ProcessBlacklistMd5 := MD5String(g_ProcessBlacklistStr);
    end
    else
    begin
      g_ProcessBlacklistStr := '';
      FillChar(g_ProcessBlacklistMd5, SizeOf(g_ProcessBlacklistMd5), 0);
    end;
  finally
    RSA.Free;
  end;
end;

procedure SaveProcessBlacklist();
var
  I: Integer;
  SaveList: TStringList;
  FileName: string;
  ProcessInfo: PTProcessInfo;
begin
  g_DBAddressList.Clear;
  FileName := ExtractFilePath(ParamStr(0)) + 'ProcessBlacklist.txt';
  g_ProcessBlackList.Lock;
  SaveList := TStringList.Create;
  try
    for I := 0 to g_ProcessBlackList.Count - 1 do
    begin
      ProcessInfo := g_ProcessBlackList.Items[I];
      SaveList.Add(ProcessInfo.ProcessName + '|' + ProcessInfo.ProcessMD5);
    end;

    SaveList.SaveToFile(FileName);
  finally
    SaveList.Free;
    g_ProcessBlackList.UnLock;
  end;
end;

function LoadNoVerifyChrList: Boolean;
var
  SL: TStringList;
  I: Integer;
begin
  Result := False;

  g_LoadNoVerifyChrList.Lock;
  try
    g_LoadNoVerifyChrList.Clear;

    if (Length(g_sLoadNoVerifyChrListFile) > 0) and FileExists(g_sLoadNoVerifyChrListFile) then
    begin
      SL := TStringList.Create;
      try
        SL.LoadFromFile(g_sLoadNoVerifyChrListFile);

        for I := 0 to SL.Count - 1 do
        begin
          if g_LoadNoVerifyChrList.IndexOf(SL[I]) < 0 then
          begin
            g_LoadNoVerifyChrList.Add(SL[I]);
          end;
        end;

        Result := True;
      finally
        SL.Free;
      end;
    end;
  finally
    g_LoadNoVerifyChrList.UnLock;
  end;
end;

{$IF CLIENT_ANTIPLUG = 1}

// 检查客户端反外挂模块是否变更
function CheckClientAntiPlugDllChanged: Boolean;
var
  FileName: string;
  MS: TMemoryStream;
  P: PChar;

  EncLen: Integer;
  RSA: TLbRSA;

  OutBuf: array[0..10240 - 1] of Byte;
  OutSize: Integer;
  AddData: TAntiPlugAddData;
  FileFlag: LongWord;

  IsKeyOK: Boolean;
//ricky   验证相关  去掉
//{$IF NEED_REGISTER = 1}
//  ExtendedInfo: Integer;
//
//  HWID1, HWID2: array[0..100] of Char;
//  sHWID1, sHWID2: string;
//{$IFEND}
begin
//ricky  验证相关  去掉
//{$IF NEED_REGISTER = 1}
//{$I Registered_Start.inc}
//  IsKeyOK := False;
//
//  if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
//  begin
//    WLHardwareGetID(HWID1);
//    WLRegGetLicenseHardwareID(HWID2);
//
//    sHWID1 := StrPas(HWID1);
//    sHWID2 := StrPas(HWID2);
//    if SameText(sHWID1, sHWID2) then
//    begin
//      IsKeyOK := True;
//    end;
//  end;
//{$I Registered_end.inc}
//{$ELSE}
//  IsKeyOK := True;
//{$IFEND}
  IsKeyOK := True;
{$I VMProtectBeginMutation.inc}
  Result := False;

  FileName := ExtractFilePath(ParamStr(0)) + 'client_module.dat';
  if FileExists(FileName) and IsKeyOK then
  begin
    MS := TMemoryStream.Create;
    try
      MS.LoadFromFile(FileName);
      P := MS.Memory;

      MS.Read(FileFlag, SizeOf(FileFlag));

{$IF RungateLEG_IOCP = 1}
      if FileFlag <> $5E462548 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 2}
      if FileFlag <> $4FB74474 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 3}
      if FileFlag <> $A1DFE8F0 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 4}
      if FileFlag <> $22CE2669 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 5}
      if FileFlag <> $9EA5AC55 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 6}
      if FileFlag <> $0533BF07 then
        Exit;
{$IFEND}

      MS.Read(EncLen, SizeOf(EncLen));
      if (EncLen > 0) and (EncLen < MS.Size) then
      begin
        RSA := TLbRSA.Create(nil);
        try
          RSA.KeySize := aks1024;

{$IF RungateLEG_IOCP = 1}
          RSA.PrivateKey.ModulusAsString :=
            'DB287F8904EE8704F32D73412FC481B51F7269517F71229FF5EE6A90CBC6176F35BA3' +
            '59EDDA24AA40120C76F9D04148AE533F7207BA2B7333847D86787ADCC1245C37CF68A' +
            '1C6F0EA4C6E4B3B74ACD2FF1BEE765DE2AAE3FA01521921004CA7001A5FAC8F4AB1BA' +
            '3E4B479F5527F9755752CC3650E1728DBA32B28E69EBEB4D6';
          RSA.PrivateKey.ExponentAsString := 'E31A';
{$ELSEIF RungateLEG_IOCP = 2}
          RSA.PrivateKey.ModulusAsString :=
            '6F0739D9AADA113F0BA86CA504863F3486DB5598BBEDF01BCC34CFED0824B6134D662' +
            'A48B533693CE133DB17CA8797ACFF7F204F8E1B7DA35E56AC723F6FB904704CE5977E' +
            'D88E769FEE3ECFB247E3296D1DE580C66ECEE8290E72E9C7EE329A03170D8941E6B38' +
            'CF1C8F5B83C850764CC828DDB0CFCBCA6BADFCF25B0BB7E90';
          RSA.PrivateKey.ExponentAsString := 'A114';
{$ELSEIF RungateLEG_IOCP = 3}
          RSA.PrivateKey.ModulusAsString :=
            'E97FD495460C5BEDC5EED9198C7D919F1DA373E7585D64A8EB8C91A192F0DD69441F6' +
            '9E7B9E47F0615AD0BC44432E2629EE6EAA923508B82EE0A20C93F0F95E6F801B57D2C' +
            '1015F9836CD2D6A7221E22BC19ED30D04FC041C627FE446DD43A79DD5C4EFC52FE90A' +
            'BEBEB2992A868A5D538BDE4794976A10A0F3FF8AA7F85E5CB';
          RSA.PrivateKey.ExponentAsString := 'B318';
{$ELSEIF RungateLEG_IOCP = 4}
          RSA.PrivateKey.ModulusAsString :=
            '137A89A8A50E3F9E4DC5909DF9330AA8EB5FF35FB5DCF99144EBA68F96C249FBEF4B6' +
            '8F279CA1EE5382BF320EABBE87C48D54A2261859318E1C9F9DFC40EC95904EB799DEC' +
            '2B9CED835E90DA610397EEDAE5F5C06741FF333183B6E6006E908913021926D6A8C93' +
            'E51D803C31DF87DFB1606AFCD252CA0DB3C095C80F0C2F5E9';
          RSA.PrivateKey.ExponentAsString := 'CB14';
{$ELSEIF RungateLEG_IOCP = 5}
          RSA.PrivateKey.ModulusAsString :=
            '3D4BF7E01E9C23EDFA2D12026D48F4AE4C2D612118307EFA35953870A6C4C4EB334C5' +
            '59D8ABEF5B55E6B0D27E8FC2B9EB119D683CEDBCCF17B62EFF38728A451B2EC834F9E' +
            'E39564C14104329B97867037398FDC1A1B32274A815B9B41394E228E2C660D646863D' +
            '6F14ED904D8A70D0BF60459C590BAF747C660D53C76EF6986';
          RSA.PrivateKey.ExponentAsString := 'D31F';
{$ELSEIF RungateLEG_IOCP = 6}
          RSA.PrivateKey.ModulusAsString :=
            '63073E95891EAD15EA768ED83F87D27ABAB0EF7CA5BD48C6ABA32A311841FE465161B' +
            'A489F3A6443DF0F757B5D8E74C84385253D66BE2436518CC85E90BF58F4D5F9A1E1CF' +
            'AE80A82DBE7A7ED1FE4BAED0FFEEC9DE73D17C99855045ED1B79B25AA65CCE17D765E' +
            '7DBDE3599DF71474B9E2278191B7CE5FE71F8E8B31E35C8D6';
          RSA.PrivateKey.ExponentAsString := '010001';
{$IFEND}

          P := PChar(Integer(MS.Memory) + SizeOf(FileFlag) + SizeOf(EncLen));
          OutSize := RSA.DecryptBuffer(P^, EncLen, OutBuf);

          if OutSize = SizeOf(AddData) then
          begin
            Move(OutBuf, AddData, SizeOf(AddData));
            Result := g_ClientAntiPlugVersion = AddData.DllCRC;
          end;
        finally
          RSA.Free;
        end;
      end;
    finally
      MS.Free;
    end;
  end;
{$I VMProtectEnd.inc}
end;

// 加载客户端反外挂模块
function LoadClientAntiPlugDll: Boolean;
var
  FileName: string;
  MS, MSDll: TMemoryStream;
  P: PChar;

  EncLen, I: Integer;
  RSA: TLbRSA;

  OutBuf: array[0..10240 - 1] of Byte;
  OutSize: Integer;
  AddData: TAntiPlugAddData;
  FileFlag: LongWord;

  Key: array[0..7] of Byte;
  sKey: string;
  IsKeyOK: Boolean;
//ricky 验证相关     去掉
//{$IF NEED_REGISTER = 1}
//  LicenseDataInfo: TLicenseDataInfo;
//  Len, ExtendedInfo: Integer;
//
//  HWID1, HWID2: array[0..100] of Char;
//  sHWID1, sHWID2: string;
//
//  UserName: array[0..100] of Char;
//  Organization: array[0..100] of Char;
//  CustomData: array[0..102400] of Char;
//
//  IsOK: Boolean;
//  C1, C2: Char;
//  S, TempStr, Password: string;
//{$IFEND}
begin
//ricky   验证相关   去掉
//{$IF NEED_REGISTER = 1}
//{$I Registered_Start.inc}
//  IsKeyOK := False;
//
//  if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
//  begin
//    FillChar(UserName, SizeOf(UserName), 0);
//    FillChar(Organization, SizeOf(Organization), 0);
//    FillChar(CustomData, SizeOf(CustomData), 0);
//
//    WLRegGetLicenseInfo(UserName, Organization, CustomData);
//    S := StrPas(CustomData);
//
//    WLHardwareGetID(HWID1);
//    WLRegGetLicenseHardwareID(HWID2);
//
//    sHWID1 := StrPas(HWID1);
//    sHWID2 := StrPas(HWID2);
//
//    if SameText(sHWID1, sHWID2) and (Length(S) > 0) and (Length(S) mod 2 = 0) then
//    begin
//      Len := Length(S) div 2;
//      SetLength(TempStr, Len);
//
//      IsOK := True;
//      if Len <> SizeOf(LicenseDataInfo) then
//        IsOK := False
//      else
//      begin
//        for I := 1 to Len do
//        begin
//          C1 := S[I * 2 - 1];
//          C2 := S[I * 2];
//          if (C1 in ['0'..'9', 'a'..'f', 'A'..'F']) and (C2 in ['0'..'9', 'a'..'f', 'A'..'F']) then
//          begin
//            TempStr[I] := Chr(StrToInt('$' + C1 + C2))
//          end
//          else
//          begin
//            IsOK := False;
//            Break;
//          end;
//        end;
//      end;
//
//      if IsOK then
//      begin
//      {$IF RungateLEG_IOCP = 1}
//        Password := '%$$^leg*&&^iocp&&rungate(*&&^1_)(%';
//
//      {$ELSEIF RungateLEG_IOCP = 2}
//        Password := '%$!^leg*&&^iocp2&&rungate(*&&^2_)(%';
//
//      {$ELSEIF RungateLEG_IOCP = 3}
//        Password := '(*&*&~@@#32~@*(*#($(*@$)%#@&%(())*&';
//
//      {$ELSEIF RungateLEG_IOCP = 4}
//        Password := '%**!^leg*&&^iocp&&rungat22e(*&&^1_)(%';
//
//      {$ELSEIF RungateLEG_IOCP = 5}
//        Password := '<>(**&^^4)(*&^)_**&^%%^**())*&^$%#$&&(?/';
//        
//      {$IFEND}
//
//        WLBufferDecrypt(PChar(TempStr), Length(TempStr), PChar(Password));
//        Move(TempStr[1], LicenseDataInfo, SizeOf(LicenseDataInfo));
//
//        Move(LicenseDataInfo.Key[0], Key[0], SizeOf(LicenseDataInfo.Key));
//
//        IsKeyOK := True;
//      end;
//    end;
//  end;
//{$I Registered_end.inc}
//{$ELSE}
//  IsKeyOK := True;
//{$IF RungateLEG_IOCP = 1}
//  Key[0] := $3D;
//  Key[1] := $EF;
//  Key[2] := $8E;
//  Key[3] := $6F;
//  Key[4] := $51;
//  Key[5] := $4F;
//  Key[6] := $1D;
//  Key[7] := $9F;
//{$ELSEIF RungateLEG_IOCP = 2}
//  Key[0] := $AC;
//  Key[1] := $28;
//  Key[2] := $D0;
//  Key[3] := $D7;
//  Key[4] := $81;
//  Key[5] := $35;
//  Key[6] := $4C;
//  Key[7] := $C7;
//{$ELSEIF RungateLEG_IOCP = 3}
//  Key[0] := $7C;
//  Key[1] := $17;
//  Key[2] := $CD;
//  Key[3] := $BA;
//  Key[4] := $2D;
//  Key[5] := $9E;
//  Key[6] := $43;
//  Key[7] := $CF;
//{$ELSEIF RungateLEG_IOCP = 4}
//  Key[0] := $E5;
//  Key[1] := $23;
//  Key[2] := $90;
//  Key[3] := $CF;
//  Key[4] := $E2;
//  Key[5] := $4E;
//  Key[6] := $AF;
//  Key[7] := $8C;
//{$ELSEIF RungateLEG_IOCP = 5}
//  Key[0] := $E7;
//  Key[1] := $67;
//  Key[2] := $37;
//  Key[3] := $30;
//  Key[4] := $33;
//  Key[5] := $50;
//  Key[6] := $4A;
//  Key[7] := $A4;
//{$IFEND}
//{$IFEND}

IsKeyOK := True;
{$IF RungateLEG_IOCP = 1}
  Key[0] := $3D;
  Key[1] := $EF;
  Key[2] := $8E;
  Key[3] := $6F;
  Key[4] := $51;
  Key[5] := $4F;
  Key[6] := $1D;
  Key[7] := $9F;
{$ELSEIF RungateLEG_IOCP = 2}
  Key[0] := $AC;
  Key[1] := $28;
  Key[2] := $D0;
  Key[3] := $D7;
  Key[4] := $81;
  Key[5] := $35;
  Key[6] := $4C;
  Key[7] := $C7;
{$ELSEIF RungateLEG_IOCP = 3}
  Key[0] := $7C;
  Key[1] := $17;
  Key[2] := $CD;
  Key[3] := $BA;
  Key[4] := $2D;
  Key[5] := $9E;
  Key[6] := $43;
  Key[7] := $CF;
{$ELSEIF RungateLEG_IOCP = 4}
  Key[0] := $E5;
  Key[1] := $23;
  Key[2] := $90;
  Key[3] := $CF;
  Key[4] := $E2;
  Key[5] := $4E;
  Key[6] := $AF;
  Key[7] := $8C;
{$ELSEIF RungateLEG_IOCP = 5}
  Key[0] := $E7;
  Key[1] := $67;
  Key[2] := $37;
  Key[3] := $30;
  Key[4] := $33;
  Key[5] := $50;
  Key[6] := $4A;
  Key[7] := $A4;
{$ELSEIF RungateLEG_IOCP = 6}
	Key[0] := $A7;
	Key[1] := $45;
	Key[2] := $32;
	Key[3] := $BB;
	Key[4] := $3D;
	Key[5] := $6A;
	Key[6] := $7F;
	Key[7] := $90;
{$IFEND}

{$I VMProtectBeginMutation.inc}
  Result := False;

  g_ClientAntiPlugDllString := '';
  g_ClientAntiPlugDllStringCRC := 0;
  g_ClientAntiPlugDllSize := 0;

  FileName := ExtractFilePath(ParamStr(0)) + 'client_module.dat';
  if FileExists(FileName) and IsKeyOK then
  begin
    MS := TMemoryStream.Create;
    MSDll := TMemoryStream.Create;
    try
      MS.LoadFromFile(FileName);
      P := MS.Memory;

      MS.Read(FileFlag, SizeOf(FileFlag));

{$IF RungateLEG_IOCP = 1}
      if FileFlag <> $5E462548 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 2}
      if FileFlag <> $4FB74474 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 3}
      if FileFlag <> $A1DFE8F0 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 4}
      if FileFlag <> $22CE2669 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 5}
      if FileFlag <> $9EA5AC55 then
        Exit;
{$ELSEIF RungateLEG_IOCP = 6}
      if FileFlag <> $0533BF07 then
        Exit;
{$IFEND}

      MS.Read(EncLen, SizeOf(EncLen));
      if (EncLen > 0) and (EncLen < MS.Size) then
      begin
        RSA := TLbRSA.Create(nil);
        try
          RSA.KeySize := aks1024;

{$IF RungateLEG_IOCP = 1}
          RSA.PrivateKey.ModulusAsString :=
            'DB287F8904EE8704F32D73412FC481B51F7269517F71229FF5EE6A90CBC6176F35BA3' +
            '59EDDA24AA40120C76F9D04148AE533F7207BA2B7333847D86787ADCC1245C37CF68A' +
            '1C6F0EA4C6E4B3B74ACD2FF1BEE765DE2AAE3FA01521921004CA7001A5FAC8F4AB1BA' +
            '3E4B479F5527F9755752CC3650E1728DBA32B28E69EBEB4D6';
          RSA.PrivateKey.ExponentAsString := 'E31A';
{$ELSEIF RungateLEG_IOCP = 2}
          RSA.PrivateKey.ModulusAsString :=
            '6F0739D9AADA113F0BA86CA504863F3486DB5598BBEDF01BCC34CFED0824B6134D662' +
            'A48B533693CE133DB17CA8797ACFF7F204F8E1B7DA35E56AC723F6FB904704CE5977E' +
            'D88E769FEE3ECFB247E3296D1DE580C66ECEE8290E72E9C7EE329A03170D8941E6B38' +
            'CF1C8F5B83C850764CC828DDB0CFCBCA6BADFCF25B0BB7E90';
          RSA.PrivateKey.ExponentAsString := 'A114';
{$ELSEIF RungateLEG_IOCP = 3}
          RSA.PrivateKey.ModulusAsString :=
            'E97FD495460C5BEDC5EED9198C7D919F1DA373E7585D64A8EB8C91A192F0DD69441F6' +
            '9E7B9E47F0615AD0BC44432E2629EE6EAA923508B82EE0A20C93F0F95E6F801B57D2C' +
            '1015F9836CD2D6A7221E22BC19ED30D04FC041C627FE446DD43A79DD5C4EFC52FE90A' +
            'BEBEB2992A868A5D538BDE4794976A10A0F3FF8AA7F85E5CB';
          RSA.PrivateKey.ExponentAsString := 'B318';
{$ELSEIF RungateLEG_IOCP = 4}
          RSA.PrivateKey.ModulusAsString :=
            '137A89A8A50E3F9E4DC5909DF9330AA8EB5FF35FB5DCF99144EBA68F96C249FBEF4B6' +
            '8F279CA1EE5382BF320EABBE87C48D54A2261859318E1C9F9DFC40EC95904EB799DEC' +
            '2B9CED835E90DA610397EEDAE5F5C06741FF333183B6E6006E908913021926D6A8C93' +
            'E51D803C31DF87DFB1606AFCD252CA0DB3C095C80F0C2F5E9';
          RSA.PrivateKey.ExponentAsString := 'CB14';
{$ELSEIF RungateLEG_IOCP = 5}
          RSA.PrivateKey.ModulusAsString :=
            '3D4BF7E01E9C23EDFA2D12026D48F4AE4C2D612118307EFA35953870A6C4C4EB334C5' +
            '59D8ABEF5B55E6B0D27E8FC2B9EB119D683CEDBCCF17B62EFF38728A451B2EC834F9E' +
            'E39564C14104329B97867037398FDC1A1B32274A815B9B41394E228E2C660D646863D' +
            '6F14ED904D8A70D0BF60459C590BAF747C660D53C76EF6986';
          RSA.PrivateKey.ExponentAsString := 'D31F';
{$ELSEIF RungateLEG_IOCP = 6}
          RSA.PrivateKey.ModulusAsString :=
            '63073E95891EAD15EA768ED83F87D27ABAB0EF7CA5BD48C6ABA32A311841FE465161B' +
            'A489F3A6443DF0F757B5D8E74C84385253D66BE2436518CC85E90BF58F4D5F9A1E1CF' +
            'AE80A82DBE7A7ED1FE4BAED0FFEEC9DE73D17C99855045ED1B79B25AA65CCE17D765E' +
            '7DBDE3599DF71474B9E2278191B7CE5FE71F8E8B31E35C8D6';
          RSA.PrivateKey.ExponentAsString := '010001';
{$IFEND}

          P := PChar(Integer(MS.Memory) + SizeOf(FileFlag) + SizeOf(EncLen));
          OutSize := RSA.DecryptBuffer(P^, EncLen, OutBuf);

          if OutSize = SizeOf(AddData) then
          begin
            Move(OutBuf, AddData, SizeOf(AddData));

            if MS.Size - SizeOf(FileFlag) - SizeOf(EncLen) - EncLen = AddData.DllLen then
            begin
              MS.Position := SizeOf(FileFlag) + SizeOf(EncLen) + EncLen;
              MSDll.CopyFrom(MS, MS.Size - SizeOf(FileFlag) - SizeOf(EncLen) - EncLen);

              for I := 0 to Length(AddData.RandKey) - 1 do
              begin
                Key[I] := AddData.RandKey[I] xor Key[I];
              end;

              SetLength(sKey, Length(Key));
              Move(Key[0], sKey[1], Length(Key));

              P := PChar(MSDll.Memory);
              DecryptDes(P^, P^, AddData.DllLen, sKey);

              Result := AddData.DllCRC = BufferCRC(P, AddData.DllLen);
              if Result then
              begin
                g_ClientAntiPlugDllString := zLibCompressBuffer(MSDll.Memory, MSDll.Size);
                g_ClientAntiPlugDllStringCRC := BufferCrc(PChar(g_ClientAntiPlugDllString), Length(g_ClientAntiPlugDllString));

                g_ClientAntiPlugDllBlockCount := (Length(g_ClientAntiPlugDllString) + g_ClientAntiPlugDllBlockSize - 1) div g_ClientAntiPlugDllBlockSize;
                g_ClientAntiPlugDllSize := MSDll.Size;

                g_ClientAntiPlugVersion := AddData.DllCRC;
              end;
            end;
          end;
        finally
          RSA.Free;
        end;
      end;
    finally
      MS.Free;
      MSDll.Free;
    end;
  end;
{$I VMProtectEnd.inc}
end;

{$IFEND}

procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);
var
  SendData: TCopyDataStruct;
  nParam: Integer;
begin
  nParam := MakeLong(Word(tRunGate), wIdent);
  SendData.cbData := Length(sSendMsg) + 1;
  GetMem(SendData.lpData, SendData.cbData);
  StrCopy(SendData.lpData, PChar(sSendMsg));
  SendMessage(g_dwGameCenterHandle, WM_COPYDATA, nParam, Cardinal(@SendData));
  FreeMem(SendData.lpData);
end;

{ TAddressList }

constructor TAddressList.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  InitializeCriticalSection(FCS);
{$ENDIF}
  FAddress := TList.Create;
end;

destructor TAddressList.Destroy;
begin
  Clear;
  FAddress.Free;
{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  DeleteCriticalSection(FCS);
{$ENDIF}
  inherited Destroy;
end;

procedure TAddressList.Clear;
var
  I: Integer;
begin
  Lock;
  try
    for I := 0 to FAddress.Count - 1 do
    begin
      Dispose(pTSockaddr(FAddress.Items[I]));
    end;
    FAddress.Clear;
  finally
    UnLock;
  end;
end;

function TAddressList.GetItems(Index: Integer): pTSockaddr;
begin
  if (Index >= 0) and (Index <= FAddress.Count - 1) then
    Result := FAddress[Index]
  else
    Result := nil;
end;

procedure TAddressList.Lock;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  SpinLock(FLocker, FName);
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TAddressList.UnLock;
begin
{$IFDEF USE_SPINLOCK}
  SpinUnLock(FLocker, FName);
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

function TAddressList.GetCount: Integer;
begin
  Result := FAddress.Count;
end;

function TAddressList.Add(IP: string): pTSockaddr;
var
  nIP: Integer;
begin
  Result := nil;
  if Length(IP) = 0 then Exit;
  nIP := inet_addr(PChar(IP));
  if nIP = INADDR_NONE then Exit;

  Result := Find(IP);
  if Result <> nil then Exit;

  New(Result);
  ZeroMemory(Result, SizeOf(TSockaddr));
  Result.nIPaddr := inet_addr(PChar(IP));
  Result.nAttackCount := 0;
  FAddress.Add(Result);
end;

function TAddressList.Find(IP: string): pTSockaddr;
var
  I: Integer;
  nIP: Integer;
  Sockaddr: pTSockaddr;
begin
  Result := nil;
  nIP := inet_addr(PChar(IP));
  for I := 0 to FAddress.Count - 1 do
  begin
    Sockaddr := FAddress.Items[I];
    if Sockaddr.nIPaddr = nIP then
    begin
      Result := Sockaddr;
      Exit;
    end;
  end;
end;

function TAddressList.FindIndex(IP: string): Integer;
var
  I: Integer;
  nIP: Integer;
  Sockaddr: pTSockaddr;
begin
  Result := -1;
  nIP := inet_addr(PChar(IP));
  for I := 0 to FAddress.Count - 1 do
  begin
    Sockaddr := FAddress.Items[I];
    if Sockaddr.nIPaddr = nIP then
    begin
      Result := I;
      Exit;
    end;
  end;
end;

procedure TAddressList.Delete(Sockaddr: pTSockaddr);
var
  I: Integer;
begin
  for I := 0 to FAddress.Count - 1 do
  begin
    if FAddress.Items[I] = Sockaddr then
    begin
      Dispose(Sockaddr);
      FAddress.Delete(I);
      Exit;
    end;
  end;
end;

procedure TAddressList.Delete(IP: string);
var
  I: Integer;
begin
  I := FindIndex(IP);
  if I <> -1 then
    DeleteIndex(I);
end;

procedure TAddressList.DeleteIndex(Index: Integer);
var
  Sockaddr: pTSockaddr;
begin
  if (Index >= 0) and (Index <= FAddress.Count - 1) then
  begin
    Sockaddr := FAddress[Index];
    Dispose(Sockaddr);
    FAddress.Delete(Index);
  end;
end;

{ TAddressListEx }

constructor TAddressListEx.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  InitializeCriticalSection(FCS);
{$ENDIF}
  FAddress := TList.Create;
end;

destructor TAddressListEx.Destroy;
begin
  Clear;
  FAddress.Free;
{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  DeleteCriticalSection(FCS);
{$ENDIF}
  inherited Destroy;
end;

procedure TAddressListEx.Clear;
var
  I: Integer;
begin
  Lock;
  try
    for I := 0 to FAddress.Count - 1 do
    begin
      Dispose(pTAddressInfo(FAddress.Items[I]));
    end;
    FAddress.Clear;
  finally
    UnLock;
  end;
end;

function TAddressListEx.GetItems(Index: Integer): pTAddressInfo;
begin
  if (Index >= 0) and (Index <= FAddress.Count - 1) then
    Result := FAddress[Index]
  else
    Result := nil;
end;

procedure TAddressListEx.Lock;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  SpinLock(FLocker, FName);
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TAddressListEx.UnLock;
begin
{$IFDEF USE_SPINLOCK}
  SpinUnLock(FLocker, FName);
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

function TAddressListEx.GetCount: Integer;
begin
  Result := FAddress.Count;
end;

function TAddressListEx.Add(IP: string): PTAddressInfo;
var
  nIP: Integer;
begin
  Result := nil;
  if Length(IP) = 0 then Exit;
  nIP := inet_addr(PChar(IP));
  if nIP = INADDR_NONE then Exit;

  Result := Find(IP);
  if Result <> nil then Exit;

  New(Result);
  ZeroMemory(Result, SizeOf(TAddressInfo));
  Result.nIPaddr := nIP;
  Result.sIPaddr := IP;
  FAddress.Add(Result);
end;

function TAddressListEx.Find(IP: string): PTAddressInfo;
var
  I: Integer;
  nIP: Integer;
  AddressInfo: PTAddressInfo;
begin
  Result := nil;
  nIP := inet_addr(PChar(IP));
  for I := 0 to FAddress.Count - 1 do
  begin
    AddressInfo := FAddress.Items[I];
    if AddressInfo.nIPaddr = nIP then
    begin
      Result := AddressInfo;
      Exit;
    end;
  end;
end;

procedure TAddressListEx.Delete(AddressInfo: pTAddressInfo);
var
  I: Integer;
begin
  for I := 0 to FAddress.Count - 1 do
  begin
    if FAddress.Items[I] = AddressInfo then
    begin
      Dispose(AddressInfo);
      FAddress.Delete(I);
      Exit;
    end;
  end;
end;

{ TSafeHashStringList }

constructor TSafeHashStringList.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  InitializeCriticalSection(FCS);
{$ENDIF}
end;

destructor TSafeHashStringList.Destroy;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  DeleteCriticalSection(FCS);
{$ENDIF}
  inherited;
end;

procedure TSafeHashStringList.Lock;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  SpinLock(FLocker, FName);
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeHashStringList.UnLock;
begin
{$IFDEF USE_SPINLOCK}
  SpinUnLock(FLocker, FName);
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

{ TSafeStringList }

constructor TSafeStringList.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  InitializeCriticalSection(FCS);
{$ENDIF}
end;

destructor TSafeStringList.Destroy;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  DeleteCriticalSection(FCS);
{$ENDIF}
  inherited;
end;

procedure TSafeStringList.Lock;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  SpinLock(FLocker, FName);
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeStringList.UnLock;
begin
{$IFDEF USE_SPINLOCK}
  SpinUnLock(FLocker, FName);
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

{ TSafeMemoryStream }

constructor TSafeMemoryStream.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  InitializeCriticalSection(FCS);
{$ENDIF}
end;

destructor TSafeMemoryStream.Destroy;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  DeleteCriticalSection(FCS);
{$ENDIF}
  inherited;
end;

procedure TSafeMemoryStream.Lock;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  SpinLock(FLocker, FName);
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeMemoryStream.UnLock;
begin
{$IFDEF USE_SPINLOCK}
  SpinUnLock(FLocker, FName);
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

function ReverseBytes(Value: LongWord): LongWord;
begin
  Result := (Value and $000000FF) shl 24 or
    (Value shr 8 and $000000FF) shl 16 or
    (Value shr 16 and $000000FF) shl 8 or
    (Value shr 24 and $000000FF);
end;

function IP2Long(sIPaddr: string): LongWord;
begin
  // 如：inet_addr(202.103.100.1) = 23357386($016467CA)
  // 016467CA 在内存是顺序存储的：(低位 ---- 高位) CA(202) 67(103) 64(100) 01(1)
  // 根据上面的数字来看，比较顺序是 1 -> 100 -> 103 -> 202，与想要的比较不符     htonl == ReverseBytes
  Result := ReverseBytes(inet_addr(PChar(sIPaddr)));
end;

function Long2IP(IP: LongWord): string;
begin
  Result := StrPas(inet_ntoa(TInAddr(ReverseBytes(IP))));
end;

function CheckInFYDenyIPList(sIPAddr: string): Boolean;
begin
  g_FYDenyIPList.Lock;
  try
    Result := g_FYDenyIPList.Find(sIPaddr) <> nil;
  finally
    g_FYDenyIPList.UnLock;
  end;

  if not Result then
  begin
    g_FYDownDenyIPList.Lock;
    try
      Result := g_FYDownDenyIPList.Find(sIPaddr) <> nil;
    finally
      g_FYDownDenyIPList.UnLock;
    end;
  end;
end;

function CheckInFYPassIPList(sIPAddr: string): Boolean;
begin
  g_FYPassIPList.Lock;
  try
    Result := g_FYPassIPList.Find(sIPaddr) <> nil;
  finally
    g_FYPassIPList.UnLock;
  end;

  if not Result then
  begin
    g_FYDownPassIPList.Lock;
    try
      Result := g_FYDownPassIPList.Find(sIPaddr) <> nil;
    finally
      g_FYDownPassIPList.UnLock;
    end;
  end;
end;

function IsBlockIP(sIPaddr: string): Boolean;
var
  I: Integer;
  nIP: LongWord;
  IPSection: PTIPSection;
begin
  g_TempIPList.Lock;
  try
    Result := g_TempIPList.FindIndex(sIPaddr) >= 0;
  finally
    g_TempIPList.UnLock;
  end;

  //-------------------------------
  if not Result then
  begin
    g_BlockIPList.Lock;
    try
      Result := g_BlockIPList.FindIndex(sIPaddr) >= 0;
    finally
      g_BlockIPList.UnLock;
    end;
  end;

  if not Result then
  begin
    nIP := IP2Long(sIPaddr);

    g_IPSectionList.Lock;
    try
      for I := 0 to g_IPSectionList.Count - 1 do
      begin
        IPSection := g_IPSectionList.Items[I];
        if (nIP >= IPSection.nBeginAddr) and (nIP <= IPSection.nEndAddr) then
        begin
          Result := True;
          Break;
        end;
      end;
    finally
      g_IPSectionList.UnLock;
    end;
  end;
end;

function IsBlockMac(sMac: string): Boolean;
begin
  g_TempMacList.Lock;
  try
    Result := g_TempMacList.IndexOf(sMac) >= 0;
  finally
    g_TempMacList.UnLock;
  end;

  //-------------------------------
  if not Result then
  begin
    g_BlockMacList.Lock;
    try
      Result := g_BlockMacList.IndexOf(sMac) >= 0;
    finally
      g_BlockMacList.UnLock;
    end;
  end;

  if not Result then
  begin
    g_FYDenyMACList.Lock;
    try
      Result := g_FYDenyMACList.IndexOf(sMac) >= 0;
    finally
      g_FYDenyMACList.UnLock;
    end;
  end;

  if not Result then
  begin
    g_FYDownDenyMACList.Lock;
    try
      Result := g_FYDownDenyMACList.IndexOf(sMac) >= 0;
    finally
      g_FYDownDenyMACList.UnLock;
    end;
  end;
end;

function IsConnLimited(sIPaddr: string): Boolean;
var
  AddressInfo: pTAddressInfo;
begin
  Result := False;
  if g_dwDefenseLevel = 0 then g_dwDefenseLevel := 1;

  g_CurrIPList.Lock;
  try
    AddressInfo := g_CurrIPList.Find(sIPaddr);
    if AddressInfo <> nil then
    begin
      Inc(AddressInfo.nCount);

      if tick_diff(AddressInfo.dwIPCountTick1, MyGetTickCount) < g_dwIPCountLimitTime1 then
      begin
        Inc(AddressInfo.nIPCount1);
        if LongWord(AddressInfo.nIPCount1) >= g_dwIPCountLimit1 * g_dwDefenseLevel then
          Result := True;
      end
      else
      begin
        AddressInfo.dwIPCountTick1 := MyGetTickCount();
        AddressInfo.nIPCount1 := 0;
      end;

      if tick_diff(AddressInfo.dwIPCountTick2, MyGetTickCount) < g_dwIPCountLimitTime2 then
      begin
        Inc(AddressInfo.nIPCount2);
        if LongWord(AddressInfo.nIPCount2) >= g_dwIPCountLimit2 * g_dwDefenseLevel then
          Result := True;
      end
      else
      begin
        AddressInfo.dwIPCountTick2 := MyGetTickCount();
        AddressInfo.nIPCount2 := 0;
      end;

      if AddressInfo.nCount > g_nMaxConnOfIPaddr {* Integer(g_dwDefenseLevel)} then
        Result := True;
    end
    else
    begin
      AddressInfo := g_CurrIPList.Add(sIPaddr);
      if AddressInfo <> nil then
        AddressInfo.nCount := 1;
    end;
  finally
    g_CurrIPList.UnLock;
  end;
end;

function GetAttackCountOfIP(sIPaddr: string): Integer;
var
  IPaddr: pTSockaddr;
begin
  Result := 0;
  g_AttackIPaddrList.Lock;
  try
    IPaddr := g_AttackIPaddrList.Find(sIPaddr);
    if IPaddr <> nil then
      Result := IPaddr.nAttackCount;
  finally
    g_AttackIPaddrList.UnLock;
  end;
end;

function GetConnectCountOfIP(sIPaddr: string): Integer;
var
  AddressInfo: pTAddressInfo;
begin
  Result := 0;
  g_CurrIPList.Lock;
  try
    AddressInfo := g_CurrIPList.Find(sIPaddr);
    if AddressInfo <> nil then
      Result := AddressInfo.nCount;
  finally
    g_CurrIPList.UnLock;
  end;
end;

procedure InitIntervals;
const
  InitActionIntervals: array[TAntiPlugActionMode] of Word = (
    1000, 1000, 1000,       // 攻击, 魔法, 走路
    1000, 0, 0,             // 跑步, 转向, 挖肉
    540, 600,               // 走路到攻击, 攻击到走路
    540, 600,               // 跑步到攻击, 攻击到跑步
    540, 1200,              // 走路到魔法, 魔法到走路
    540, 1200,              // 跑步到魔法, 魔法到跑步
    250, 0,                 // 转向到攻击, 攻击到转向
    250, 0,                 // 转向到魔法, 魔法到转向
    250, 250,               // 挖肉到攻击, 挖肉到魔法
    0, 250,                 // 移动到转向, 转向到移动
    0, 250,                 // 移动到挖肉, 挖肉到移动
    0, 0, 0);               // 攻击并发, 魔法并发, 移动并发
var
  I: Integer;
  ActionMode: TAntiPlugActionMode;
begin
  for ActionMode := Low(TAntiPlugActionMode) to High(TAntiPlugActionMode) do
  begin
    for I := 0 to SPEED_INTERVALS_COUNT - 1 do
      g_wActionSpeedIntervals[ActionMode][I] := InitActionIntervals[ActionMode];
  end;
end;

//ricky   前面获取百度时间类的实现   去掉
//{$IF NEED_REGISTER = 1}
//
//{ TGetBaiduTimeThread }
//
//constructor TGetBaiduTimeThread.Create(CreateSuspended: Boolean);
//begin
//  FreeOnTerminate := True;
//  inherited Create(CreateSuspended);
//end;
//
//procedure TGetBaiduTimeThread.Execute;
//var
//  WinHttp: TWinHttp;
//begin
//{$I VMProtectBegin.inc}
//  WinHttp := TWinHttp.Create;
//  try
//    WinHttp.TimeOut := 3000;
//    if Random(100) = 0 then
//      WinHttp.Get('http://www.baidu.com/')
//    else
//      WinHttp.Get('http://220.181.111.157');
//
//    g_dtBaiduTime := WinHttp.ResponseDateTime;
//    g_dtBaiduTime2 := WinHttp.ResponseDateTime;
//  finally
//    WinHttp.Free;
//  end;
//{$I VMProtectEnd.inc}
//end;
//{$IFEND}

{ TProcessBlacklist }

constructor TProcessBlacklist.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  FList := TList.Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  InitializeCriticalSection(FCS);
{$ENDIF}
  FMaxCount := 80;
end;

destructor TProcessBlacklist.Destroy;
begin
  Clear;
  FList.Free;

{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  DeleteCriticalSection(FCS);
{$ENDIF}
  inherited;
end;

function TProcessBlacklist.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TProcessBlacklist.Add(const ProcessName, ProcessMD5: string): PTProcessInfo;
begin
  Result := nil;
  if FList.Count >= FMaxCount then Exit;
  if Find(ProcessMD5) <> nil then Exit;

  New(Result);
  Result.ProcessName := ProcessName;
  Result.ProcessMD5 := UpperCase(ProcessMD5);
  FList.Add(Result);
end;

procedure TProcessBlacklist.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(PTProcessInfo(FList.Items[I]));
  end;
  FList.Clear;
end;

procedure TProcessBlacklist.Delete(ProcessInfo: PTProcessInfo);
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    if FList.Items[I] = ProcessInfo then
    begin
      Dispose(ProcessInfo);
      FList.Delete(I);
      Break;
    end;
  end;
end;

function TProcessBlacklist.Find(const MD5: string): PTProcessInfo;
var
  I: Integer;
  ProcessInfo: PTProcessInfo;
begin
  Result := nil;
  for I := 0 to FList.Count - 1 do
  begin
    ProcessInfo := FList.Items[I];
    if SameText(ProcessInfo.ProcessMD5, MD5) then
    begin
      Result := ProcessInfo;
      Break;
    end;
  end;
end;

function TProcessBlacklist.GetItems(Index: Integer): PTProcessInfo;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TProcessBlacklist.Lock;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  SpinLock(FLocker, FName);
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TProcessBlacklist.UnLock;
begin
{$IFDEF USE_SPINLOCK}
  SpinUnLock(FLocker, FName);
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

procedure InitActionIntervalsFileNames;
var
  ActionMode: TAntiPlugActionMode;
begin
  for ActionMode := Low(TAntiPlugActionMode) to High(TAntiPlugActionMode) do
  begin
    if Length(g_sActionIntervalsFileNames) > 0 then
    begin
      g_sActionIntervalsFileNames[ActionMode] := ExtractFilePath(ParamStr(0)) + g_sActionIntervalsFileNames[ActionMode];
    end;
  end;
end;

function InputPassword(const ACaption, APrompt: string; ADefault: string = ''): string;
begin
  Result := ADefault;
  InputPasswordEx(ACaption, APrompt, Result);
end;

function InputPasswordEx(const ACaption, APrompt: string; var Value: string): Boolean;
  function GetAveCharSize(Canvas: TCanvas): TPoint;
  var
    I: Integer;
    Buffer: array[0..51] of Char;
  begin
    for I := 0 to 25 do Buffer[I] := Chr(I + Ord('A'));
    for I := 0 to 25 do Buffer[I + 26] := Chr(I + Ord('a'));
    GetTextExtentPoint(Canvas.Handle, Buffer, 52, TSize(Result));
    Result.X := Result.X div 52;
  end;
var
  Form: TForm;
  Prompt: TLabel;
  Edit: TEdit;
  DialogUnits: TPoint;
  ButtonTop, ButtonWidth, ButtonHeight: Integer;
begin
  Result := False;
  Form := TForm.Create(Application);
  with Form do
    try
      Canvas.Font := Font;
      DialogUnits := GetAveCharSize(Canvas);
      BorderStyle := bsDialog;
      Caption := ACaption;
      ClientWidth := MulDiv(140, DialogUnits.X, 4);
      Position := poMainFormCenter;
      Prompt := TLabel.Create(Form);
      with Prompt do
      begin
        Parent := Form;
        Caption := APrompt;
        Left := MulDiv(8, DialogUnits.X, 4);
        Top := MulDiv(6, DialogUnits.Y, 6);
        Constraints.MaxWidth := MulDiv(128, DialogUnits.X, 4);
        WordWrap := True;
      end;
      Edit := TEdit.Create(Form);
      with Edit do
      begin
        Parent := Form;
        Left := Prompt.Left;
        Top := Prompt.Top + Prompt.Height + 5;
        Width := MulDiv(124, DialogUnits.X, 4);
        MaxLength := 255;
        PasswordChar := '*';
        Text := Value;
        SelectAll;
      end;
      ButtonTop := Edit.Top + Edit.Height + 8;
      ButtonWidth := MulDiv(50, DialogUnits.X, 4);
      ButtonHeight := MulDiv(14, DialogUnits.Y, 8);
      with TButton.Create(Form) do
      begin
        Parent := Form;
        Caption := '确定';
        ModalResult := IDOK;
        Default := True;
        SetBounds(MulDiv(30, DialogUnits.X, 4), ButtonTop, ButtonWidth, ButtonHeight);
      end;
      with TButton.Create(Form) do
      begin
        Parent := Form;
        Caption := '取消';
        ModalResult := IDCANCEL;
        Cancel := True;
        SetBounds(MulDiv(82, DialogUnits.X, 4), ButtonTop, ButtonWidth, ButtonHeight);
        Form.ClientHeight := Top + Height + 10;
      end;
      if ShowModal = IDOK then
      begin
        Value := Edit.Text;
        Result := True;
      end;
    finally
      Form.Free;
    end;
end;

function EncodeRunGateMsg(DefMsg: pTDefaultMessage; DataAdd: PChar; DataAddLen: LongWord): string;
var
  TheDefMsg: TRungateMsgHeader;
begin
  TheDefMsg.Code := RUN_GATE_MSG_CODE;
  TheDefMsg.Msg := DefMsg^;

  if (DataAdd <> nil) and (DataAddLen > 0) then
    TheDefMsg.DataLen := DataAddLen
  else
    TheDefMsg.DataLen := 0;

  SetLength(Result, SizeOf(TRungateMsgHeader) + TheDefMsg.DataLen);

  Move(TheDefMsg, Result[1], SizeOf(TRungateMsgHeader));
  if TheDefMsg.DataLen > 0 then
  begin
    Move(DataAdd^, Result[1 + SizeOf(TRungateMsgHeader)], TheDefMsg.DataLen);
  end;
end;

function MyGetTickCount: DWORD;
begin
  // 提高处理精度 2019-12-07 22:21:39
  timeBeginPeriod(1);
  Result := timeGetTime;
  timeEndPeriod(1);
end;

// 取文件版本号
function GetFileVersionNumber(const FileName: string): TVersionNumber;
var
  VersionInfoBufferSize: DWORD;
  dummyHandle: DWORD;
  VersionInfoBuffer: Pointer;
  FixedFileInfoPtr: PVSFixedFileInfo;
  VersionValueLength: UINT;
begin
  FillChar(Result, SizeOf(Result), 0);
  if not FileExists(FileName) then
    Exit;

  VersionInfoBufferSize := GetFileVersionInfoSize(PChar(FileName), dummyHandle);
  if VersionInfoBufferSize = 0 then
    Exit;

  GetMem(VersionInfoBuffer, VersionInfoBufferSize);
  try
    try
      Win32Check(GetFileVersionInfo(PChar(FileName), dummyHandle, VersionInfoBufferSize, VersionInfoBuffer));
      Win32Check(VerQueryValue(VersionInfoBuffer, '\', Pointer(FixedFileInfoPtr), VersionValueLength));
    except
      Exit;
    end;
    Result.Major := FixedFileInfoPtr^.dwFileVersionMS shr 16;
    Result.Minor := FixedFileInfoPtr^.dwFileVersionMS;
    Result.Release := FixedFileInfoPtr^.dwFileVersionLS shr 16;
    Result.Build := FixedFileInfoPtr^.dwFileVersionLS;
  finally
    FreeMem(VersionInfoBuffer);
  end;
end;

// 取文件版本字符串
function GetFileVersionStr(const FileName: string): string;
begin
  with GetFileVersionNumber(FileName) do
    Result := Format('%d.%d.%d.%d', [Major, Minor, Release, Build]);
end;

initialization
  begin
    InitIntervals;

    FillChar(g_boSendSpeedIntervalsToClient, SizeOf(g_boSendSpeedIntervalsToClient), 0);

    g_sIniFileName := ExtractFilePath(ParamStr(0)) + 'Config.ini';

    InitActionIntervalsFileNames;
    
    g_sWordFilterFileName  := ExtractFilePath(ParamStr(0)) + 'WordFilter.txt';

    g_MainLogStrings := TSafeStringList.Create({$IFDEF USE_SPINLOCK}'MainLogLocker'{$ENDIF});

    g_IOCPLogStrings := TSafeStringList.Create({$IFDEF USE_SPINLOCK}'IOCPLogLocker'{$ENDIF});

    g_WordFilterList := TSafeHashStringList.Create({$IFDEF USE_SPINLOCK}'WordFilterLocker'{$ENDIF});

    g_LockUserList := TSafeHashStringList.Create({$IFDEF USE_SPINLOCK}'LockUserLocker'{$ENDIF});

    g_VerifyFailUserList := TSafeHashStringList.Create({$IFDEF USE_SPINLOCK}'VerifyFailUserLocker'{$ENDIF});

    g_sPluginDir := ExtractFilePath(ParamStr(0)) + 'Plugin\';

    g_sLogClientPacketDir := ExtractFilePath(ParamStr(0)) + 'Log_Client\';
    if not DirectoryExists(g_sLogClientPacketDir) then
      SysUtils.ForceDirectories(g_sLogClientPacketDir);

    g_ScreenshotPath := ExtractFilePath(ParamStr(0)) + 'Screenshot\';

    g_sLogClientPakcetUserFile := ExtractFilePath(ParamStr(0)) + 'LogUser.txt';

    g_sVerifyCodeExcludeMapFileName := ExtractFilePath(ParamStr(0)) + 'VerifyCodeExcludeMap.txt';

    FillChar(g_EatItemCDConfig, SizeOf(g_EatItemCDConfig), 0);

    g_LoginMACPlayerList := TSafeHashStringList.Create;
  end;

finalization
  begin
    g_WordFilterList.Free;
    g_MainLogStrings.Free;
    g_IOCPLogStrings.Free;
    g_LockUserList.Free;
    g_VerifyFailUserList.Free;
    g_LoginMACPlayerList.Free;
  end;

end.


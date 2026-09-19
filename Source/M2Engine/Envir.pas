unit Envir;

interface

uses
  Windows, SysUtils, Classes, Math, Generics.Collections, SDK, ObjGame, ItemEvent, Grobal2, M2Locker, System.Types, M2Threads,
  SafeAreaManager, M2Definition;

const
  USEOBJLIST = 1;

type
  // 添加一个枚举类型，配合十步一杀技能 piaoyun 2013-06-25
  TWalkFlag = (wf_Hum, wf_Mon, wf_Npc, wf_Guard, wf_War, wf_Obstacle);

  TWalkFlagArr = set of TWalkFlag;

  TMapHeader = packed record
    wWidth: Word;
    wHeight: Word;
    sTitle: string[15];
    UpdateDate: TDateTime;
    btVersion: Byte;
    Reserved: array [0 .. 22] of Byte;
  end;

  TENMapHeader = packed record
    Title: string[16];
    Reserved: LongWord;
    Width: Word;
    Not1: Word;
    Height: Word;
    Not2: Word;
    Reserved2: array [0 .. 24] of Byte;
  end;

  // 传奇3地图文件头
  TEIMapHeader = packed record
    Desc: array [0 .. 4] of Integer;
    wAttr: Word;
    Width: Word;
    Height: Word;
    EventFileIdx: AnsiChar;
    FogColor: AnsiChar;
  end;

  TMapUnitInfo = packed record
    wBkImg: Word; // 32768 $8000 为禁止移动区域
    wMidImg: Word;
    wFrImg: Word;
    btDoorIndex: Byte;
    btDoorOffset: Byte;
    btAniFrame: Byte;
    btAniTick: Byte;
    btArea: Byte;
    btLight: Byte; // 0..1..4 堡盔 瓤苞
  end;

  pTMapUnitInfo = ^TMapUnitInfo;
  TMap = array [0 .. 1000 * 1000 - 1] of TMapUnitInfo;
  pTMap = ^TMap;

  TNewMapUnitInfo = packed record
    wBkImg: Word; // 32768 $8000 为禁止移动区域
    wMidImg: Word;
    wFrImg: Word;
    btDoorIndex: Byte;
    btDoorOffset: Byte;
    btAniFrame: Byte;
    btAniTick: Byte;
    btArea: Byte;
    btLight: Byte;
    btUnitBkImg: Byte; // 32768 $8000 为禁止移动区域
    btUnitMidImg: Byte;
  end;

  pTNewMapUnitInfo = ^TNewMapUnitInfo;
  TNewMap = array [0 .. 1000 * 1000 - 1] of TNewMapUnitInfo;
  pTNewMap = ^TNewMap;

  TReturnMapInfo = packed record // 归来国际版
    wBkImg: Word;
    wMidImg: Word;
    wFrImg: Word;
    btDoorIndex: Byte;
    btDoorOffset: Byte;
    btAniFrame: Byte;
    btAniTick: Byte;
    btArea: Byte;
    btLight: Byte;
    btUnitBkImg: Byte;
    btUnitMidImg: Byte;
    Bytes: array [0 .. 22 - 1] of Byte;
  end;

  pTReturnMapInfo = ^TReturnMapInfo;
  TReturnMap = array [0 .. 1000 * 1000 - 1] of TReturnMapInfo;
  pTReturnMap = ^TReturnMap;

  TENMapInfo = packed record
    BkImg: Word;
    BkImgNot: Word;
    MidImg: Word;
    FrImg: Word;
    DoorIndex: Byte;
    DoorOffset: Byte;
    AniFrame: Byte;
    AniTick: Byte;
    Area: Byte;
    light: Byte;
    btNot: Byte;
  end;

  PTENMapInfo = ^TENMapInfo;
  TENMap = array [0 .. 1000 * 1000 - 1] of TENMapInfo;
  pTENMap = ^TENMap;

  // 传奇3地图
  TEIMapTileInfo = packed record
    btFileIdx: Byte;
    wTileIdx: Word;
  end;

  TEIMapInfo = packed Record
    btFlag: Byte;
    btObj1Ani: Byte;
    btObj2Ani: Byte; // 动画的张图
    btFileIdx1: Byte; // 上层OB文件号
    btFileIdx2: Byte; // 下层OB文件号
    wObj1: Word; // 下层OB图片号
    wObj2: Word; // 上层OB图片号
    btDoorIdx: Byte;
    btDoorOffset: Byte;
    wLigntNEvent: Word;
    btLigntNEvent1: Byte;
  end;

  PTEIMapInfo = ^TEIMapInfo;
  TEIMap = array [0 .. MAXINT div 64] of TEIMapInfo;
  pTEIMap = ^TEIMap;
  TObjArray = array of TGameObject;
  pTObjArray = ^TObjArray;

  TMapCellinfo = packed record
    chFlag: Byte;
{$IF USEOBJLIST = 0}
    ObjList: TList;
{$ELSE}
    ObjList: TObjArray;
{$IFEND}
  end;

  pTMapCellinfo = ^TMapCellinfo;
  { 副本地图进入限制 }
  TFBEnterLimit = (fbel_JOB3, fbel_Group, fbel_OnlyCreater, fbel_Guild);
  TFBFailType = (fbft_JOB3, fbft_NoPlayerEnter);
  PSceneShakeInfo = ^TSceneShakeInfo;

  TSceneShakeInfo = record
    LastTick: LongWord; // 最后一次振动时间
    Count: Integer; // 振动次数
    CurCount: Integer; // 当前振动次数
    PlayerName: string; // 振动人
    EnableClientOption: Boolean; // 允许客户端内挂关闭
  end;

  TMapQuestInfo = record
    sMapName: string[30];
    nFlags: Integer;
    nFlag: Integer;
    nValue: Integer;
    boFlag: Boolean;
    sMonName: string[30];
    sNeedItem: string[30];
    sScriptName: string[30];
    boGroup: Boolean;
    s08: string;
    s0C: string;
    bo10: Boolean;
    NPC: TObject;
  end;

  pTMapQuestInfo = ^TMapQuestInfo;

  TMapManager = class;
  pTEnvirnoment = ^TEnvirnoment;

  TEnvirnoment = class
    sMapName: string;
    sMapDesc: string;
    sMainMapName: string;
    m_boMainMap: Boolean;
    MapCellArray: array of TMapCellinfo;
    nMinMap: Integer;
    nServerIndex: Integer;
    m_nWidth: Integer;
    m_nHeight: Integer;
    m_boDARK: Boolean;
    m_boDAY: Boolean;
    m_WeatherEffect: array [0 .. MAX_MAP_WEATEHER_EFFECT - 1] of TServerWeateherEffect;
    m_DoorList: TList;
    bo2C: Boolean;
    m_nTHUNDER: Integer; // 闪电效果
    m_nLAVA: Integer; // 冒岩浆
    m_boMISSION: Boolean; // 不允许使用任何物品和技能，并且下属在该地图会自动消失 piaoyun 2013-07-25
    m_boNODROPITEM: Boolean; // 禁止死亡掉物品 piaoyun 2013-07-25
    m_boDieTime: Boolean; // 当前地图人物死亡多长时间后自动掉线 piaoyun 2013-09-05
    m_dwDieTime: LongWord; // 当前地图人物死亡多长时间后自动掉线--掉线时间 piaoyun 2013-09-05
    m_boNOTHROWITEM: Boolean; // 禁止丢物品 piaoyun 2013-09-05
    m_boNotStone: Boolean; // 当前地图魔血石、气血石、幻魔石无效 piaoyun 2013-09-05
    m_boSlaveNotAttackHuman: Boolean; // 当前地图宝宝不攻击人物 chongchong 2013-12-11
    m_boSlaveNotAttackHero: Boolean; // 当前地图宝宝不攻击英雄 chongchong 2013-12-11
    m_boSAFE: Boolean; // 0x2D
    m_boFightZone: Boolean; // 0x2E
    m_boFight2Zone: Boolean;
    m_boFight3Zone: Boolean; // 0x2F  //行会战争地图
    m_boFight4Zone: Boolean; // 挑战地图
    m_btFight3Flag: Byte; // 行会站标识
    m_boIncGold: Boolean; // 自动加金币
    m_nincGoldPoint: Cardinal; // 自动加金币数量
    m_nIncGoldTime: Word; // 自动加金币间隔
    m_boDecGold: Boolean; // 自动减金币
    m_nDecGoldPoint: Cardinal; // 自动减金币数量
    m_nDecGoldTime: Word; // 自动减金币间隔
    m_boQUIZ: Boolean; // 0x30
    m_boNORECONNECT: Boolean; // 0x31
    m_boNEEDHOLE: Boolean; // 0x32
    m_boNORECALL: Boolean; // 0x33
    m_boNOGUILDRECALL: Boolean;
    m_boNODEARRECALL: Boolean;
    m_boNOMASTERRECALL: Boolean;
    m_boNORANDOMMOVE: Boolean; // 0x34
    m_boNODRUG: Boolean; // 0x35
    m_boMINE: Boolean; // 0x36
    m_boNOPOSITIONMOVE: Boolean; // 0x37
    sNoReconnectMap: string; // 0x38
    QuestNPC: TObject; // 0x3C
    nNEEDSETONFlag: Integer; // 0x40
    nNeedONOFF: Integer; // 0x44
    m_QuestList: TList; // 0x48
    m_boRUNHUMAN: Boolean; // 可以穿人
    m_boRUNMON: Boolean; // 可以穿怪
    m_boNoRunHuman: Boolean; // 可以穿人
    m_boNoRunMon: Boolean; // 可以穿怪
    m_boINCHP: Boolean; // 自动加HP值
    m_boIncGameGold: Boolean; // 自动减游戏币
    m_boINCGAMEPOINT: Boolean; // 自动加点
    m_boDECHP: Boolean; // 自动减HP值
    m_boDecGameGold: Boolean; // 自动减游戏币
    m_boDecGamePoint: Boolean; // 自动减点
    m_boMUSIC: Boolean; // 音乐
    m_boEXPRATE: Boolean; // 杀怪经验倍数
    m_boPKWINLEVEL: Boolean; // PK得等级
    m_boPKWINEXP: Boolean; // PK得经验
    m_boPKLOSTLEVEL: Boolean; // PK丢等级
    m_boPKLOSTEXP: Boolean; // PK丢经验
    m_nPKWINLEVEL: Integer; // PK得等级数
    m_nPKLOSTLEVEL: Integer; // PK丢等级
    m_nPKWINEXP: Integer; // PK得经验数
    m_nPKLOSTEXP: Integer; // PK丢经验
    m_nDECHPTIME: Integer; // 减HP间隔时间
    m_nDECHPPOINT: Integer; // 一次减点数
    m_nINCHPTIME: Integer; // 加HP间隔时间
    m_nINCHPPOINT: Integer; // 一次加点数
    m_nDECGAMEGOLDTIME: Integer; // 减游戏币间隔时间
    m_nDecGameGold: Integer; // 一次减数量
    m_nDecGamePointTime: Integer; // 减游戏点间隔时间
    m_nDecGamePoint: Integer; // 一次减数量
    m_nINCGAMEGOLDTIME: Integer; // 加游戏币间隔时间
    m_nIncGameGold: Integer; // 一次加数量
    m_nINCGAMEPOINTTIME: Integer; // 加游戏币间隔时间
    m_nINCGAMEPOINT: Integer; // 一次加数量
    m_sMusicFileName: string; // 音乐ID
    m_nEXPRATE: Integer; // 经验倍率
    m_boUnAllowFireMagic: Boolean; // 不允许使用火墙
    m_boUnAllowStdItems: Boolean; // 是否不允许使用物品
    m_UnAllowStdItemsList: TSafeList; // 不允许使用物品列表
    m_DropAddToUserBagItemsList: TSafeList;
    m_boUnAllowMagics: Boolean; // 是否不允许使用魔法
    m_UnAllowMagicList: TSafeList; // 不允许使用魔法列表
    m_boNoRecallHero: Boolean;
    m_boNoCallPet: Boolean;
    m_boOnKillMob: Boolean;
    m_boAllowUseMyshop: Boolean; // 允许使用个人商店
    m_nSAYLEVEL: Integer;
    m_boDELDROPITEM: Boolean;
    m_nRevivalMaxCount: Integer;
    m_nRevivalCheckTime: Integer;
    // REVIVAL(X:N) 当前地图人物可复活的次数,X表示复活次数,N表示人物在当前地图已经复活次数的自动清零间隔(最小30秒).具体表示:每经过指定秒人物在当前地图复活过的次数自动减1.
    m_boNODROPUSEITEMS: Boolean;
    m_boNOSAFEPOSITIONMOVE: Boolean;
    m_boHITMON: Boolean;
    m_sHITMON: string;
    m_boNOHORSE: Boolean; // 禁止骑马 chongchong 2013-10-12
    m_boNoChallenge: Boolean; // 禁止挑战 chongchong 2014-01-05
    m_boNoAutoOnline: Boolean; // 禁止自动挂机 chongchong 2014-11-26
    m_boNoSwitchAttackMode: Boolean; // 禁止切换攻击模式 chongchong 2015-04-29
    m_boNoHeroProtect: Boolean; // 禁止英雄守护模式 chongchong 2015-07-25
    m_GateList: TList;
    m_LastRunTick: LongWord;
    // 副本地图 ----  相关定义 chongchong 2013-09-06
    m_boFB: Boolean;
    m_sFBName: string;
    m_FBEnterLimit: TFBEnterLimit; // 副本进入限制
    m_dwFBEnterDelayMin: LongWord; // 副本延时进入时间
    m_dwFBNoHumClearMin: LongWord; // 退出副本后多长时间没人清副本 chongchong 2015-10-15
    m_boFBCreate: Boolean;
    m_dwFBCreateTime: LongWord;
    m_boFBFail: Boolean; // 地图失败
    m_dwFBFailTime: LongWord; // 地图最后失败时间
    m_FBFailType: TFBFailType; // 地图失败原因
    m_dwFBTime: LongWord; // 地图有效时间
    m_btFBIndex: Byte;
    m_FBMasterObject: TObject; // 副本地图创建人
    m_dwFBCheckMonsterTick: LongWord; // 副本地图怪物检测时间
    m_dwFBPlayObjectCount: LongWord; // 副本地图中的人物数量
    m_dwFBNOPlayObjectTick: LongWord; // 副本地图最后无人物时间 chongchong 2015-10-15
    m_boFBPlayObjectEnter: Boolean; // 人物进入了副本 chongchong 2015-10-16
    m_FBMonGenList: TList; // 副本地图怪物创建表
    m_FBMonsterList: TList; // 副本地图怪物列表
    m_FBEvent: TStringList; // 副本地图事件列表
    m_boMirror: Boolean;
    m_dwMirrorCreateTick: LongWord;
    m_dwMirrorSurvivalTime: LongWord;
    m_sMirrorExitToMap: string;
    m_MirrorExitMapPostion: TPoint;
    m_nMirrorMinMap: Integer;
    m_boAlwaysShowTime: Boolean;
    m_dwMirrorPlayObjectCount: LongWord;
    m_boNoManNoMon: Boolean; // 智能刷怪 chongchong 2014-01-07
    m_boMakeMon: Boolean;
    m_dwNoManTick: LongWord;
    m_boClearMon: Boolean;
    m_dwClearMonTick: LongWord;
    m_boMakeMonPriority: Boolean; // 优先刷怪
    m_boNight: Boolean;
    m_boNoDeal: Boolean; // 禁止交易
    m_boNoShop: Boolean; // 禁止商铺
    // 浑水摸鱼模式
    m_nSecretFlag: Integer;
    m_sSecretShowName: string;
    m_wSecretDressShap: Word;
    m_wSecretWeaponShap: Word;
    m_sTimeMapCode: string;
    m_nTimeMapMin: Integer;
    m_boTimeMapShowLeft: Boolean;
    m_sTimeMapLabel: string;
    // 浑水摸鱼模式
    m_nSecretFlag2: Integer;
    m_sSecretShowName2: string;
    m_wSecretDressShap2: Word;
    m_wSecretWeaponShap2: Word;
    m_boNeedLevelTime: Boolean;
    m_dwNeedLevelPoint: LongWord;
    m_boNoAuction: Boolean;
    m_boNoAutoDropItemToBag: Boolean; // 禁止物品自动入包
    m_boNoAutoRangePickItem: Boolean; // 禁止范围拾取
    m_boInitialize: Boolean;
    FMapFlag: TMapFlag;
    m_boInvalid: Boolean;
    m_dwInvalidTick: LongWord;
    // 是否为天关 chongchong 2017-07-10
    m_boGuardianLevel: Boolean; // 是否为守关地图
    m_boStartGuardianLevel: Boolean; // 是否开始闯关
    m_boGuardianLevelSucces: Boolean; // 是否闯关完成
    m_boGuardianLevelGetItem: Boolean; // 是否领取奖励
    m_nGuardianLevelNo: Integer; // 关数
    m_nGuardianLevelBatchCount: Integer; // 总波数
    m_nGuardianLevelBatchNo: Integer; // 波数
    m_GuardianLevelPlayer: TGameObject; // 闯关玩家
    m_GuardinaLevelStatue: TGameObject; // 守关雕像
    m_nGuardinaLevelMonCount: Integer; // 怪物数量
    m_nGuardinaLevelMonGenX: Integer; // 刷怪X
    m_nGuardinaLevelMonGenY: Integer; // 刷怪Y
    m_sGuardinaLevelGetItem: array [0 .. 3] of string; // 奖励物品
    m_nGuardinaLevelButchItems: array [0 .. 39, 0 .. 3] of Integer; // 每波奖励物品数量
    m_nGuardinaLevelHasItems: array [0 .. 3] of Integer; // 已得到奖励物品数量
    m_sGuardinaLevelMons: array [0 .. 39] of string; // 每波怪物及数量
  private
    FSceneShakeList: TGList;
    FMonCount: LongWord;
    FHumCount: LongWord;
    FHumBBCount: LongWord;
    FClearHumOrBBTick: LongWord;
    FCriticalSection: TM2CriticalSection;
    procedure Initialize(nWidth, nHeight: Integer);
    function GetMainMap: string;
    procedure ClearSceneShakeList;
  public
    constructor Create();
    destructor Destroy; override;
    procedure ShowMapObject(nX, nY: Integer; Cert: TGameObject);
    function AddToMap(nX, nY: Integer; pAddObject: TGameObject): TGameObject;
    function CanAddToMapPosition(nX, nY: Integer): Boolean;
    function CanWalk(nX, nY: Integer; boFlag: Boolean): Boolean;
    function CanWalkOfItem(nX, nY: Integer; boFlag, boItem: Boolean): Boolean;
    function CanWalkEx(WalkObject: TGameObject; nX, nY: Integer; boFlag: Boolean): Boolean;
    function CanWalkEx2(WalkObject: TGameObject; nX, nY: Integer; boFlag: Boolean): Boolean;
    // 重载一个CanWalkEx函数 配合十步一杀技能
    function CanWalkEx3(nX, nY: Integer; Flag: TWalkFlagArr): Boolean;
    function CanFly(nSX, nSY, nDX, nDY: Integer): Boolean;
    function MoveToMovingObject(nCX, nCY: Integer; Cert: TGameObject; nX, nY: Integer; boFlag: Boolean): Boolean;
    function GetItem(nX, nY: Integer): TItemObject; overload;
    function GetItem(nX, nY: Integer; AItemObject: TItemObject): TItemObject; overload;
    function DeleteFromMap(nX, nY: Integer; pRemoveObject: TGameObject): Boolean;
    function IsCheapStuff(): Boolean;
    procedure AddDoorToMap;
    function AddToMapMineEvent(nX, nY: Integer; Event: TGameObject): TGameObject;
    function LoadMapData(nHandle: THandle): Boolean;
    function LoadEIMapData(nHandle: THandle): Boolean;
    function DoLoadMapData(sMapFile: string): Boolean;
    function CreateQuest(nFlag, nValue: Integer; s24, s28, s2C: string; boGrouped: Boolean): Boolean;
    function GetMapCellInfo(nX, nY: Integer; var MapCellInfo: pTMapCellinfo): Boolean;
    function GetXYObjCount(nX, nY: Integer): Integer; overload;
    function GetXYObjCount(AObject: TGameObject; nX, nY: Integer): Integer; overload;
    function GetXYNpcObjCount(nX, nY: Integer): Integer; overload;
    function GetNextPosition(sX, sY, nDir, nFlag: Integer; var snX, snY: Integer): Boolean;
    function sub_4B5FC8(nX, nY: Integer): Boolean;
    procedure VerifyMapTime(nX, nY: Integer; BaseObject: TGameObject);
    function CanSafeWalk(nX, nY: Integer): Boolean;
    function ArroundDoorOpened(nX, nY: Integer): Boolean;
    function GetMovingObject(nX, nY: Integer; boFlag: Boolean; BaseObjectList: TList): Integer; overload;
    function GetMovingObject(nX, nY: Integer; boFlag: Boolean): Pointer; overload;
    function GetMovingObject(nX, nY: Integer; AObject: TObject; boFlag: Boolean): Pointer; overload;
    function GetMovingObjectEx(BaseObject: TObject; nX, nY: Integer; boFlag: Boolean): Pointer;
    function GetQuestNPC(BaseObject: TObject; sCharName, sStr: string; boFlag: Boolean): TObject;
    function GetItemEx(nX, nY: Integer; var nCount: Integer): TItemObject;
    function GetItemEx2(nX, nY: Integer; var nCount: Integer): TItemObject;
    function GetItemEx3(nX, nY: Integer; var nCount: Integer): TItemObject;
    function GetDoor(nX, nY: Integer): TDoorObject;
    function IsValidObject(nX, nY: Integer; nRage: Integer; BaseObject: TObject): Boolean;
    function IsValidObjectEx(nX, nY, nRage: Integer; BaseObject: TObject): Boolean;
    function GetRangeBaseObject(nX, nY: Integer; nRage: Integer; boFlag: Boolean; BaseObjectList: TList): Integer;
    function GetRangePlayObject(nX, nY: Integer; nRage: Integer; boFlag: Boolean; BaseObjectList: TList): Integer;
    function GetRangeItemObject(nX, nY: Integer; nRage: Integer; ItemObjectList: TList): Integer;
    function GetItemObjects(nX, nY: Integer; ItemObjectList: TList): Integer;
    function GetBaseObjects(nX, nY: Integer; IncDeathObject: Boolean; BaseObjectList: TList): Integer;
    function GetPlayObjects(nX, nY: Integer; IncDeathObject: Boolean; BaseObjectList: TList): Integer;
    function GetEvent(nX, nY: Integer): TObject;
    procedure SetMapXYFlag(nX, nY: Integer; boFlag: Boolean);
    function GetXYHuman(nMapX, nMapY: Integer): Boolean;
    function GetEnvirInfo(): string;
    function AllowStdItems(nItemIdx: Integer): Boolean;
    function AllowDropToBagItem(nItemIdx: Integer): Boolean;
    function AllowMagics(nMagIdx: Integer): Boolean;
    procedure AddHumBBCount;
    procedure Run;
    function GetDropPosition(nOrgX, nOrgY, nRange: Integer; var nDX, nDY: Integer): Boolean;
    // 新增加函数计算掉落位置，区别于上一个的位置是：标准位置不掉落，主要用于魔王岭弓箭手，不让掉到弓箭手的脚下，不然不好捡 chongchong 2017-06-27
    function GetDropPosition2(nOrgX, nOrgY, nRange: Integer; var nDX: Integer; var nDY: Integer): Boolean;
    property MonCount: LongWord read FMonCount;
    property HumCount: LongWord read FHumCount;
    property HumBBCount: LongWord read FHumBBCount;
    property ClearHumOrBBTick: LongWord read FClearHumOrBBTick;
    property MapName: string read GetMainMap;
    // 在指定区域取出一个随机坐标 piaoyun 2013-07-27
    function GetRangeXY(nX, nY, nRang: Integer; var vX, vY: Integer): Boolean;
    function AddSceneShake(ShakeCount: Integer; PlayerName: string; EnableClientOption: Boolean): PSceneShakeInfo;
    procedure LockR(LockID: Integer);
    procedure UnLockR;
    procedure LockW(LockID: Integer);
    procedure UnLockW;
    procedure Invalidity;
    procedure PorcessGuardianLevelInfo; // 处理闯关信息
    procedure ResetGuardianLevel;
    /// <summary>
    /// 获取同排跨度位坐标
    /// </summary>
    function GetSitInLinPosition(sX, sY, nDir, nSpan: Integer; out snX, snY: Integer): Boolean;
  end;

  TMapManager = class(TGList) // 004B52B0
    m_GateList: TGList;
  private
    m_nProcMapIDx: Integer;
    m_nProcGateIDx: Integer;
    function Find(const sMapName: string; var Index: Integer): Boolean;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadMapDoor();
    function AddMapInfo(sMapName, sMainMapName, sMapDesc: string; nServerNumber: Integer; MapFlag: pTMapFlag; QuestNPC: TObject)
      : TEnvirnoment;
    function GetMapInfo(nServerIdx: Integer; sMapName: string): TEnvirnoment;
    function AddMapRoute(sSMapNO: string; nSMapX, nSMapY: Integer; sDMapNO: string; nDMapX, nDMapY: Integer): Boolean; overload;
    function AddMapRoute(sName, sSMapNO: string; nSMapX, nSMapY: Integer; sDMapNO: string; nDMapX, nDMapY: Integer;
      nRange, nTime: Integer; nDoorIndex: Integer = -1): Boolean; overload;
    function GetGate(sName: string; nSMapX, nSMapY, nDMapX, nDMapY: Integer): TGateObject;
    procedure GetMapGateInfo(sName, sSMapNO: string; var sDMapNO: string; var nSMapX, nSMapY, nDMapX, nDMapY: Integer); overload;
    procedure GetMapGateInfo(sName: string; boIsSource: Boolean; var sMapNO: string; var nX, nY: Integer); overload;
    procedure DelMapRoute(sName: string); overload;
    procedure DelMapRoute(sName, sSMapNO: string); overload;
    procedure DelMapRoute(sName: string; Envir: TEnvirnoment); overload;
    function GetMapOfServerIndex(sMapName: string): Integer;
    function FindMap(sMapName: string): TEnvirnoment;
    // 增加DelMap函数，解决地图删除不及时，导致的判断问题。 参见 NpcActionCmd.ActionOfDelMirrorMap Cursor 2023-08-21 17:25:17
    function DelMap(AMap: TEnvirnoment): Boolean;
    procedure ReSetMinMap();
    procedure Run();
    procedure ProcessMapDoor();
    procedure MakeSafePkZone();
    procedure MakeMapMagic();
  end;

const
  SecretFlag_NoSay = 1; // 禁止说话
  SecretFlag_NoChangNameColor = 2; // 禁止名字变色
  SecretFlag_NoViewOtherItem = 4; // 禁止查看别人装备
  SecretFlag_ShowEqualName = 8; // 统一名字
  SecretFlag_ShowEqualFeature = 16; // 统一外观
  SecretFlag_HumAndHeroPercentHP = 32; // 人物英雄百分比显示血量
  SecretFlag_HideFengHao = 64; // 隐身称号
  SecretFlag_HideActorIcons = 128; // 隐身顶花翎

implementation

uses
  ObjBase, ObjNpc, M2Share, GameEvent, HUtil32, Castle, UsrEngn, ObjPlayer;

{ TEnvirList }
procedure TMapManager.MakeSafePkZone();
var
  I: Integer;
  nX, nY: Integer;
  SafeEvent: TSafeEvent;
  nMinX, nMaxX, nMinY, nMaxY: Integer;
  Envir: TEnvirnoment;
  nDir: Integer;
  SafeArea: TSafeArea;
  RangSafeArea: TRangeSafeArea;
  AllotypeArea: TAllotypeSafeArea;
  Row: TAllotypeRow;
  Point: PAllotypePoint;
begin
  for I := 0 to g_SafeAreaManager.Count - 1 do
  begin
    SafeArea := g_SafeAreaManager.Items[I];
    if SafeArea is TRangeSafeArea then
    begin
      RangSafeArea := SafeArea as TRangeSafeArea;
      if (RangSafeArea.ShowType > 0) then
      begin
        Envir := FindMap(RangSafeArea.MapName);
        if Envir <> nil then
        begin
          nMinX := RangSafeArea.GetCenterX - RangSafeArea.Range;
          nMaxX := RangSafeArea.GetCenterX + RangSafeArea.Range;
          nMinY := RangSafeArea.GetCenterY - RangSafeArea.Range;
          nMaxY := RangSafeArea.GetCenterY + RangSafeArea.Range;
          for nX := nMinX to nMaxX do
          begin
            for nY := nMinY to nMaxY do
            begin
              if ((nX < nMaxX) and (nY = nMinY)) or ((nY < nMaxY) and (nX = nMinX)) or (nX = nMaxX) or (nY = nMaxY) then
              begin
                if nX = nMinX then
                begin
                  if nY = nMinY then
                    nDir := DR_UPLEFT
                  else if nY = nMaxY then
                    nDir := DR_DOWNLEFT
                  else
                    nDir := DR_LEFT;
                end
                else if nX = nMaxX then
                begin
                  if nY = nMinY then
                    nDir := DR_UPRIGHT
                  else if nY = nMaxY then
                    nDir := DR_DOWNRIGHT
                  else
                    nDir := DR_RIGHT;
                end
                else
                begin
                  if nY = nMinY then
                    nDir := DR_UP
                  else
                    nDir := DR_DOWN;
                end;
                SafeEvent := TSafeEvent.Create(Envir, nX, nY, RangSafeArea.ShowType, nDir);
                g_EventManager.AddEvent(SafeEvent);
              end;
            end;
          end;
        end;
      end;
    end
    else if SafeArea is TAllotypeSafeArea then
    begin
      AllotypeArea := SafeArea as TAllotypeSafeArea;
      Envir := FindMap(AllotypeArea.MapName);
      if Envir <> nil then
      begin
        for nY := 0 to AllotypeArea.Count - 1 do
        begin
          Row := AllotypeArea.Rows[nY];
          for nX := 0 to Row.Count - 1 do
          begin
            Point := Row.Points[nX];
            SafeEvent := TSafeEvent.Create(Envir, Point.PointX, Row.PointY, Point.ShowType, Point.Dir);
            g_EventManager.AddEvent(SafeEvent);
          end;
        end;
      end;
    end;
  end;
end;

procedure TMapManager.MakeMapMagic();
begin
  (* var
    I,J: Integer;
    MapMagicEvent: TMapMagicEvent;
    MagicEvent: PMagicEvent;
    Envir: TEnvirnoment;
    begin
    for I := 0 to g_MapMagicEventList.Count - 1 do
    begin
    MapMagicEvent := g_MapMagicEventList.MapMagicEvent[I];
    Envir := FindMap(MapMagicEvent.MapName);
    if Envir = nil then Continue;
    for J := 0 to MapMagicEvent.Count - 1 do
    begin
    MagicEvent := MapMagicEvent.MagicEvent[J];
    if MagicEvent.KeepVisible then
    g_EventManager.AddEvent(TSafeEvent.Create(Envir, MagicEvent.X, MagicEvent.Y, MagicEvent.MagicType));
    end;
    end;
  *)
end;

function IntListCompare(Item1, Item2: Pointer): Integer;
begin
  Result := NativeInt(Item1) - NativeInt(Item2);
end;

function TMapManager.AddMapInfo(sMapName, sMainMapName, sMapDesc: string; nServerNumber: Integer; MapFlag: pTMapFlag;
  QuestNPC: TObject): TEnvirnoment;
var
  Envir: TEnvirnoment;
  I: Integer;
  nStd: Integer;
  sText: string;
  TempList: TStringList;
  Magic: pTMagic;
  sFileName: string;
begin
  Result := nil;
  if GetMapInfo(nServerNumber, sMapName) <> nil then
  begin
    MainOutMessage('重复的地图代码: ' + sMapName);
    Exit;
  end;

  Envir := TEnvirnoment.Create;
  Envir.FMapFlag := MapFlag^;
  Envir.sMapName := sMapName;
  Envir.sMainMapName := sMainMapName;
  Envir.sMapDesc := sMapDesc;
  if sMainMapName <> '' then
    Envir.m_boMainMap := True;
  Envir.nServerIndex := nServerNumber;
  // piaoyun 2013-07-10 -- 地图效果
  Envir.m_nTHUNDER := MapFlag.nTHUNDER;
  Envir.m_nLAVA := MapFlag.nLAVA;
  // 地图效果 piaoyun 2013-07-25
  Envir.m_boMISSION := MapFlag.boMISSION;
  Envir.m_boNODROPITEM := MapFlag.boNODROPITEM;
  // 地图参数死亡时间控制 piaoyun 2013-09-05
  Envir.m_boDieTime := MapFlag.boDieTime;
  Envir.m_dwDieTime := MapFlag.dwDieTime;
  // 禁止丢物品 piaoyun 2013-09-05
  Envir.m_boNOTHROWITEM := MapFlag.boNOTHROWITEM;
  // 当前地图魔血石、气血石、幻魔石无效 piaoyun 2013-09-05
  Envir.m_boNotStone := MapFlag.boNotStone;
  // 当前地图宝宝不攻击人物 chongchong 2013-12-11
  Envir.m_boSlaveNotAttackHuman := MapFlag.boSlaveNotAttackHuman;
  // 当前地图宝宝不攻击英雄 chongchong 2013-12-11
  Envir.m_boSlaveNotAttackHero := MapFlag.boSlaveNotAttackHero;

  Envir.m_boSAFE := MapFlag.boSAFE;
  Envir.m_boFightZone := MapFlag.boFIGHT;
  Envir.m_boFight2Zone := MapFlag.boFIGHT2;
  Envir.m_boFight3Zone := MapFlag.boFIGHT3;
  Envir.m_boFight4Zone := MapFlag.boFIGHT4;
  Envir.m_btFight3Flag := MapFlag.btFight3Flag;

  Envir.m_boIncGold := MapFlag.boIncGold and (MapFlag.nIncGoldPoint > 0);
  Envir.m_nincGoldPoint := MapFlag.nIncGoldPoint;
  Envir.m_nIncGoldTime := MapFlag.nIncGoldTime * 1000;

  Envir.m_boDecGold := MapFlag.boDecGold and (MapFlag.nDecGoldPoint > 0);
  Envir.m_nDecGoldPoint := MapFlag.nDecGoldPoint;
  Envir.m_nDecGoldTime := MapFlag.nDecGoldTime * 1000;

  Envir.m_boDARK := MapFlag.boDARK;
  Envir.m_boDAY := MapFlag.boDAY;
  Envir.m_boQUIZ := MapFlag.boQUIZ;
  Envir.m_boNORECONNECT := MapFlag.boNORECONNECT;
  Envir.m_boNEEDHOLE := MapFlag.boNEEDHOLE;
  Envir.m_boNORECALL := MapFlag.boNORECALL;
  Envir.m_boNOGUILDRECALL := MapFlag.boNOGUILDRECALL;
  Envir.m_boNODEARRECALL := MapFlag.boNODEARRECALL;
  Envir.m_boNOMASTERRECALL := MapFlag.boNOMASTERRECALL;
  Envir.m_boNORANDOMMOVE := MapFlag.boNORANDOMMOVE;
  Envir.m_boNODRUG := MapFlag.boNODRUG;
  Envir.m_boMINE := MapFlag.boMINE;
  Envir.m_boNOPOSITIONMOVE := MapFlag.boNOPOSITIONMOVE;

  Envir.m_boRUNHUMAN := MapFlag.boRUNHUMAN; // 可以穿人
  Envir.m_boRUNMON := MapFlag.boRUNMON; // 可以穿怪

  Envir.m_boNoRunHuman := MapFlag.boNoRunHuman; // 不可以穿人
  Envir.m_boNoRunMon := MapFlag.boNoRunMon; // 不可以穿怪

  Envir.m_boINCHP := MapFlag.boINCHP and (MapFlag.nINCHPPOINT > 0); // 自动加HP值
  Envir.m_nINCHPPOINT := MapFlag.nINCHPPOINT; // 一次加HP点数
  Envir.m_nINCHPTIME := MapFlag.nINCHPTIME; // 加HP间隔时间

  Envir.m_boDECHP := MapFlag.boDECHP and (MapFlag.nDECHPPOINT > 0); // 自动减HP值
  Envir.m_nDECHPPOINT := MapFlag.nDECHPPOINT; // 一次减HP点数
  Envir.m_nDECHPTIME := MapFlag.nDECHPTIME; // 减HP间隔时间

  Envir.m_boIncGameGold := MapFlag.boINCGAMEGOLD and (MapFlag.nINCGAMEGOLD > 0); // 自动加游戏币
  Envir.m_nIncGameGold := MapFlag.nINCGAMEGOLD; // 一次加游戏币数量
  Envir.m_nINCGAMEGOLDTIME := MapFlag.nINCGAMEGOLDTIME; // 加游戏币间隔时间

  Envir.m_boDecGameGold := MapFlag.boDECGAMEGOLD and (MapFlag.nDECGAMEGOLD > 0); // 自动减游戏币
  Envir.m_nDecGameGold := MapFlag.nDECGAMEGOLD; // 一次减游戏币数量
  Envir.m_nDECGAMEGOLDTIME := MapFlag.nDECGAMEGOLDTIME; // 减游戏币间隔时间

  Envir.m_boINCGAMEPOINT := MapFlag.boINCGAMEPOINT and (MapFlag.nINCGAMEPOINT > 0); // 自动加游戏点
  Envir.m_nINCGAMEPOINT := MapFlag.nINCGAMEPOINT; // 一次加游戏点数量
  Envir.m_nINCGAMEPOINTTIME := MapFlag.nINCGAMEPOINTTIME; // 加游戏点间隔时间

  Envir.m_boDecGamePoint := MapFlag.boDECGAMEPOINT and (MapFlag.nDECGAMEPOINT > 0); // 自动减游戏点
  Envir.m_nDecGamePoint := MapFlag.nDECGAMEPOINT; // 一次减游戏点数量
  Envir.m_nDecGamePointTime := MapFlag.nDecGamePointTime; // 减游戏点间隔时间

  Envir.m_boMUSIC := MapFlag.boMUSIC; // 音乐
  Envir.m_boEXPRATE := MapFlag.boEXPRATE; // 杀怪经验倍数
  Envir.m_boPKWINLEVEL := MapFlag.boPKWINLEVEL; // PK得等级
  Envir.m_boPKWINEXP := MapFlag.boPKWINEXP; // PK得经验
  Envir.m_boPKLOSTLEVEL := MapFlag.boPKLOSTLEVEL;
  Envir.m_boPKLOSTEXP := MapFlag.boPKLOSTEXP;
  Envir.m_nPKWINLEVEL := MapFlag.nPKWINLEVEL; // PK得等级数
  Envir.m_nPKWINEXP := MapFlag.nPKWINEXP; // PK得经验数
  Envir.m_nPKLOSTLEVEL := MapFlag.nPKLOSTLEVEL;
  Envir.m_nPKLOSTEXP := MapFlag.nPKLOSTEXP;
  Envir.m_nPKWINEXP := MapFlag.nPKWINEXP; // PK得经验数
  Envir.m_sMusicFileName := MapFlag.sMusicFileName; // 音乐ID
  Envir.m_nEXPRATE := MapFlag.nEXPRATE; // 经验倍率
  Envir.m_WeatherEffect[0].boIsUsed := MapFlag.boWeatherEffect1;
  Envir.m_WeatherEffect[1].boIsUsed := MapFlag.boWeatherEffect1;
  Envir.m_WeatherEffect[2].boIsUsed := MapFlag.boWeatherEffect1;
  Envir.m_boOnKillMob := MapFlag.boOnKillMob;
  Envir.sNoReconnectMap := MapFlag.sReConnectMap;
  Envir.QuestNPC := QuestNPC;
  Envir.nNEEDSETONFlag := MapFlag.nNEEDSETONFlag;
  Envir.nNeedONOFF := MapFlag.nNeedONOFF;
  Envir.m_boUnAllowFireMagic := MapFlag.boNOFIREMAGIC; // 不允许使用火墙
  Envir.m_boUnAllowStdItems := MapFlag.boUnAllowStdItems;
  Envir.m_boUnAllowMagics := MapFlag.boNOTALLOWUSEMAGIC;
  Envir.m_boNoRecallHero := MapFlag.boNoRecallHero or Envir.m_boFight4Zone;
  Envir.m_boNoCallPet := MapFlag.boNoCallPet;
  Envir.m_boAllowUseMyshop := MapFlag.boAllowUseMyshop;
  Envir.m_nSAYLEVEL := MapFlag.nSAYLEVEL;
  Envir.m_boDELDROPITEM := MapFlag.boDELDROPITEM;
  Envir.m_nRevivalMaxCount := MapFlag.nRevivalMaxCount;
  Envir.m_nRevivalCheckTime := MapFlag.nRevivalCheckTime;
  Envir.m_boNODROPUSEITEMS := MapFlag.boNODROPUSEITEMS;
  Envir.m_boNOSAFEPOSITIONMOVE := MapFlag.boNOSAFEPOSITIONMOVE;
  Envir.m_boHITMON := MapFlag.boHITMON;
  Envir.m_sHITMON := MapFlag.sHITMON;
  Envir.m_boNOHORSE := MapFlag.boNOHORSE;
  Envir.m_boNoChallenge := MapFlag.boNoChallenge;
  Envir.m_boNoAutoOnline := MapFlag.boNoAutoOnline;
  Envir.m_boNoSwitchAttackMode := MapFlag.boNoSwitchAttackMode;
  Envir.m_boNoManNoMon := MapFlag.boNoManNoMon;
  Envir.m_boNight := MapFlag.boNight;
  Envir.m_boNoDeal := MapFlag.boNoDeal;
  Envir.m_boNoShop := MapFlag.boNoShop;
  Envir.m_boNoHeroProtect := MapFlag.boNoHeroProtect;
  Envir.m_nSecretFlag := MapFlag.SecretFlag;
  Envir.m_sSecretShowName := MapFlag.SecretShowName;
  Envir.m_wSecretDressShap := MapFlag.SecretDressShap;
  Envir.m_wSecretWeaponShap := MapFlag.SecretWeaponShap;
  Envir.m_sTimeMapCode := MapFlag.TimeMapCode;
  Envir.m_nTimeMapMin := MapFlag.TimeMapMin;
  Envir.m_boTimeMapShowLeft := MapFlag.TimeMapShowLeft;
  Envir.m_sTimeMapLabel := MapFlag.TimeMapLabel;
  Envir.m_boNeedLevelTime := MapFlag.boNeedLevelTime;
  Envir.m_dwNeedLevelPoint := MapFlag.dwNeedLevelPoint;
  Envir.m_boNoAuction := MapFlag.boNoAuction;
  Envir.m_boNoAutoDropItemToBag := MapFlag.boNoAutoDropItemToBag; // 禁止物品自动入包
  Envir.m_boNoAutoRangePickItem := MapFlag.boNoAutoRangePickItem; // 禁止范围拾取

  if (Envir.m_boUnAllowStdItems) and (MapFlag.sUnAllowStdItemsText <> '') then
  begin
    TempList := TStringList.Create;
    ExtractStrings(['|', '\', '/', ','], [], PChar(Trim(MapFlag.sUnAllowStdItemsText)), TempList);
    for I := 0 to TempList.Count - 1 do
    begin
      sText := Trim(TempList.Strings[I]);
      nStd := UserEngine.GetStdItemIdx(sText);
      if nStd >= 0 then
      begin
        if Envir.m_UnAllowStdItemsList = nil then
          Envir.m_UnAllowStdItemsList := TSafeList.Create('m_UnAllowStdItemsList_Lock');

        Envir.m_UnAllowStdItemsList.Add(TObject(nStd));
      end;
    end;
    TempList.Free;
  end;

  if Envir.m_UnAllowStdItemsList <> nil then
    Envir.m_UnAllowStdItemsList.Sort(IntListCompare);

  if (MapFlag.sDropItemAddUserBag <> '') then
  begin
    TempList := TStringList.Create;
    ExtractStrings(['|', '\', '/', ','], [], PChar(Trim(MapFlag.sDropItemAddUserBag)), TempList);
    for I := 0 to TempList.Count - 1 do
    begin
      sText := Trim(TempList.Strings[I]);
      nStd := UserEngine.GetStdItemIdx(sText);
      if nStd >= 0 then
      begin
        if Envir.m_DropAddToUserBagItemsList = nil then
          Envir.m_DropAddToUserBagItemsList := TSafeList.Create('m_DropAddToUserBagItemsList_Lock');

        Envir.m_DropAddToUserBagItemsList.Add(TObject(nStd));
      end;
    end;
    TempList.Free;
  end;

  if Envir.m_DropAddToUserBagItemsList <> nil then
    Envir.m_DropAddToUserBagItemsList.Sort(IntListCompare);

  if (Envir.m_boUnAllowMagics) and (MapFlag.sUnAllowMagicText <> '') then
  begin
    TempList := TStringList.Create;
    ExtractStrings(['|', '\', '/', ','], [], PChar(Trim(MapFlag.sUnAllowMagicText)), TempList);
    // MainOutMessage('m_boUnAllowMagics '+TempList.Text);
    for I := 0 to TempList.Count - 1 do
    begin
      sText := Trim(TempList.Strings[I]);
      if sText <> '' then
      begin
        Magic := UserEngine.FindMagicEx(sText);
        if Magic <> nil then
        begin
          if Envir.m_UnAllowMagicList = nil then
            Envir.m_UnAllowMagicList := TSafeList.Create('m_UnAllowMagicList_Lock');

          Envir.m_UnAllowMagicList.Add(TObject(Magic.wMagicId));
        end;
      end;
    end;
    TempList.Free;
  end;

  if Envir.m_UnAllowMagicList <> nil then
    Envir.m_UnAllowMagicList.Sort(IntListCompare);

  for I := 0 to MiniMapList.Count - 1 do
  begin
    if (CompareText(MiniMapList.Strings[I], Envir.sMapName) = 0) or
      ((Envir.sMainMapName <> '') and (CompareText(MiniMapList.Strings[I], Envir.sMainMapName) = 0)) then
    begin
      Envir.nMinMap := Integer(MiniMapList.Objects[I]);
      Break;
    end;
  end;

  if sMainMapName <> '' then
  begin
    sFileName := g_Config.sMapDir + sMainMapName + '.map';
    if FileExists(sFileName) then
    begin
      if Envir.DoLoadMapData(sFileName) then
      begin
        Result := Envir;
        // 优化 chongchong 2018-05-06
        // Self.Add(Envir);
        Find(Envir.sMapName, I);
        Insert(I, Envir);
      end
      else
      begin
        MainOutMessage('地图文件 ' + sFileName + ' 载入失败！');
        Envir.Free;
      end;
    end
    else
    begin
      MainOutMessage('地图文件 ' + sFileName + ' 未找到！');
      Envir.Free;
    end;
  end
  else
  begin
    sFileName := g_Config.sMapDir + sMapName + '.map';
    if FileExists(sFileName) then
    begin
      if Envir.DoLoadMapData(sFileName) then
      begin
        Result := Envir;
        // 优化 chongchong 2018-05-06
        // Self.Add(Envir);
        Find(Envir.sMapName, I);
        Insert(I, Envir);
      end
      else
      begin
        MainOutMessage('地图文件 ' + sFileName + ' 载入失败！');
        Envir.Free;
      end;
    end
    else
    begin
      MainOutMessage('地图文件 ' + sFileName + ' 未找到！');
      Envir.Free;
    end;
  end;
end;

function TMapManager.AddMapRoute(sSMapNO: string; nSMapX, nSMapY: Integer; sDMapNO: string; nDMapX, nDMapY: Integer): Boolean;
var
  GateObject: TGateObject;
  SEnvir: TEnvirnoment;
  DEnvir: TEnvirnoment;
begin
  Result := false;
  SEnvir := FindMap(sSMapNO);
  DEnvir := FindMap(sDMapNO);
  if (SEnvir <> nil) and (DEnvir <> nil) then
  begin
    GateObject := TGateObject.Create;
    GateObject.m_DEnvir := DEnvir;
    GateObject.m_nSMapX := nSMapX;
    GateObject.m_nSMapY := nSMapY;
    GateObject.m_nMapX := nDMapX;
    GateObject.m_nMapY := nDMapY;
    GateObject.m_sSMapNO := sSMapNO;
    GateObject.m_sDMapNO := sDMapNO;
    { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-17】 }
    { if SEnvir.AddToMap(nSMapX, nSMapY, GateObject) = GateObject then
      SEnvir.m_GateList.Add(GateObject); }
    if SEnvir.AddToMap(nSMapX, nSMapY, GateObject) = GateObject then
      SEnvir.m_GateList.Add(GateObject)
    else
      GateObject.Free;

    Result := True;
  end;
end;

function TMapManager.GetGate(sName: string; nSMapX, nSMapY, nDMapX, nDMapY: Integer): TGateObject;
var
  I: Integer;
  GateObject: TGateObject;
begin
  Result := nil;
  for I := 0 to m_GateList.Count - 1 do
  begin
    GateObject := TGateObject(m_GateList.Items[I]);
    if GateObject.m_sName = sName then
    begin
      if nSMapX <> -1 then
      begin
        if (GateObject.m_nSMapX = nSMapX) and (GateObject.m_nSMapY = nSMapY) then
        begin
          Result := GateObject;
          Break;
        end;
      end
      else if (GateObject.m_nMapX = nDMapX) and (GateObject.m_nMapY = nDMapY) then
      begin
        Result := GateObject;
        Break;
      end;
    end;
  end;
end;

procedure TMapManager.GetMapGateInfo(sName, sSMapNO: string; var sDMapNO: string; var nSMapX, nSMapY, nDMapX, nDMapY: Integer);
var
  I: Integer;
  GateObject: TGateObject;
begin
  nSMapX := -1;
  nSMapY := -1;
  nDMapX := -1;
  nDMapY := -1;
  sDMapNO := '';
  for I := 0 to m_GateList.Count - 1 do
  begin
    GateObject := TGateObject(m_GateList.Items[I]);
    if (GateObject.m_sName = sName) and (GateObject.m_sSMapNO = sSMapNO) and GateObject.m_boCenter then
    begin
      sDMapNO := GateObject.m_sDMapNO;
      nSMapX := GateObject.m_nSMapX;
      nSMapY := GateObject.m_nSMapY;
      nDMapX := GateObject.m_nMapX;
      nDMapY := GateObject.m_nMapY;
      Exit;
    end;
  end;

  for I := 0 to m_GateList.Count - 1 do
  begin
    GateObject := TGateObject(m_GateList.Items[I]);
    if (GateObject.m_sName = sName) and (GateObject.m_sSMapNO = sSMapNO) then
    begin
      sDMapNO := GateObject.m_sDMapNO;
      nSMapX := GateObject.m_nSMapX;
      nSMapY := GateObject.m_nSMapY;
      nDMapX := GateObject.m_nMapX;
      nDMapY := GateObject.m_nMapY;
      Break;
    end;
  end;
end;

procedure TMapManager.GetMapGateInfo(sName: string; boIsSource: Boolean; var sMapNO: string; var nX, nY: Integer);
var
  I: Integer;
  GateObject: TGateObject;
begin
  nX := -1;
  nY := -1;
  sMapNO := '';
  for I := 0 to m_GateList.Count - 1 do
  begin
    GateObject := TGateObject(m_GateList.Items[I]);
    if (GateObject.m_sName = sName) and GateObject.m_boCenter then
    begin
      if boIsSource then
      begin
        sMapNO := GateObject.m_sSMapNO;
        nX := GateObject.m_nSMapX;
        nY := GateObject.m_nSMapY;
      end
      else
      begin
        sMapNO := GateObject.m_sDMapNO;
        nX := GateObject.m_nMapX;
        nY := GateObject.m_nMapY;
      end;
      Exit;
    end;
  end;

  for I := 0 to m_GateList.Count - 1 do
  begin
    GateObject := TGateObject(m_GateList.Items[I]);
    if (GateObject.m_sName = sName) then
    begin
      if boIsSource then
      begin
        sMapNO := GateObject.m_sSMapNO;
        nX := GateObject.m_nSMapX;
        nY := GateObject.m_nSMapY;
      end
      else
      begin
        sMapNO := GateObject.m_sDMapNO;
        nX := GateObject.m_nMapX;
        nY := GateObject.m_nMapY;
      end;
      Break;
    end;
  end;
end;

procedure TMapManager.DelMapRoute(sName, sSMapNO: string);
var
  I: Integer;
  GateObject: TGateObject;
  SEnvir: TEnvirnoment;
begin
  SEnvir := FindMap(sSMapNO);
  if SEnvir <> nil then
  begin
    for I := m_GateList.Count - 1 downto 0 do
    begin
      GateObject := TGateObject(m_GateList.Items[I]);
      if (GateObject.m_sName = sName) and (GateObject.m_sSMapNO = sSMapNO) then
      begin
        if SEnvir.DeleteFromMap(GateObject.m_nSMapX, GateObject.m_nSMapY, GateObject) then
        begin
          m_GateList.Delete(I);
          GateObject.Free;
        end;
      end;
    end;
  end;
end;

procedure TMapManager.DelMapRoute(sName: string);
var
  I: Integer;
  GateObject: TGateObject;
  Envir: TEnvirnoment;
begin
  for I := m_GateList.Count - 1 downto 0 do
  begin
    GateObject := TGateObject(m_GateList.Items[I]);
    if (GateObject.m_sName = sName) then
    begin
      Envir := FindMap(GateObject.m_sSMapNO);
      if (Envir <> nil) and Envir.DeleteFromMap(GateObject.m_nSMapX, GateObject.m_nSMapY, GateObject) then
      begin
        m_GateList.Delete(I);
        GateObject.Free;
      end;
    end;
  end;
end;

function TMapManager.DelMap(AMap: TEnvirnoment): Boolean;
var
  I: Integer;
begin
  Result := false;
  Lock;
  try
    for I := Count - 1 downto 0 do
    begin
      if Items[I] = AMap then
      begin
        Result := True;
        Delete(I);
        Break;
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure TMapManager.DelMapRoute(sName: string; Envir: TEnvirnoment);
var
  I: Integer;
  GateObject: TGateObject;
begin
  for I := m_GateList.Count - 1 downto 0 do
  begin
    GateObject := TGateObject(m_GateList.Items[I]);
    if (GateObject.m_sName = sName) and (GateObject.m_sSMapNO = Envir.sMapName) then
    begin
      if Envir.DeleteFromMap(GateObject.m_nSMapX, GateObject.m_nSMapY, GateObject) then
      begin
        m_GateList.Delete(I);
        GateObject.Free;
      end;
    end;
  end;
end;

function TMapManager.AddMapRoute(sName, sSMapNO: string; nSMapX, nSMapY: Integer; sDMapNO: string; nDMapX, nDMapY: Integer;
  nRange, nTime: Integer; nDoorIndex: Integer): Boolean;
  function GetRandXY(Envir: TEnvirnoment; var nX: Integer; var nY: Integer): Boolean;
  var
    n14, n18, n1C: Integer;
  begin
    Result := false;
    if Envir.m_nWidth < 80 then
      n18 := 3
    else
      n18 := 10;

    if Envir.m_nHeight < 150 then
    begin
      if Envir.m_nHeight < 50 then
        n1C := 2
      else
        n1C := 15;
    end
    else
      n1C := 50;

    n14 := 0;
    while (True) do
    begin
      if Envir.CanWalk(nX, nY, True) then
      begin
        Result := True;
        Break;
      end;

      if nX < (Envir.m_nWidth - n1C - 1) then
        Inc(nX, n18)
      else
      begin
        nX := Random(Envir.m_nWidth);
        if nY < (Envir.m_nHeight - n1C - 1) then
          Inc(nY, n18)
        else
          nY := Random(Envir.m_nHeight);
      end;

      Inc(n14);
      if n14 >= 201 then
        Break;
    end;
  end;
  procedure DeleteMapGate(GateObject: TGateObject);
  var
    I: Integer;
  begin
    for I := m_GateList.Count - 1 downto 0 do
    begin
      if TGateObject(m_GateList.Items[I]) = GateObject then
      begin
        m_GateList.Delete(I);
        GateObject.Free;
        Break;
      end;
    end;
  end;

var
  GateObject: TGateObject;
  SEnvir: TEnvirnoment;
  DEnvir: TEnvirnoment;
  nX, nY: Integer;
  nXX, nYY: Integer;
  Merchant: TMerchant;
begin
  Result := false;
  SEnvir := FindMap(sSMapNO);
  DEnvir := FindMap(sDMapNO);
  if (SEnvir <> nil) and (DEnvir <> nil) then
  begin
    if (nSMapX < 0) or (nSMapY < 0) then
    begin
      nX := Random(SEnvir.m_nWidth);
      nY := Random(SEnvir.m_nHeight);
      if GetRandXY(SEnvir, nX, nY) then
      begin
        nSMapX := nX;
        nSMapY := nY;
      end;
    end;

    if (nDMapX < 0) or (nDMapY < 0) then
    begin
      nX := Random(DEnvir.m_nWidth);
      nY := Random(DEnvir.m_nHeight);
      if GetRandXY(DEnvir, nX, nY) then
      begin
        nDMapX := nX;
        nDMapY := nY;
      end;
    end;

    DelMapRoute(sName, SEnvir);
    if DEnvir.CanWalk(nDMapX, nDMapY, True) then
    begin
      for nXX := nSMapX - nRange to nSMapX + nRange do
      begin
        for nYY := nSMapY - nRange to nSMapY + nRange do
        begin
          if SEnvir.CanWalk(nXX, nYY, True) then
          begin
            GateObject := TGateObject.Create();
            GateObject.m_boFlag := True;
            GateObject.m_sName := sName;
            GateObject.m_DEnvir := DEnvir;
            GateObject.m_sSMapNO := sSMapNO;
            GateObject.m_sDMapNO := sDMapNO;
            GateObject.m_nSMapX := nXX;
            GateObject.m_nSMapY := nYY;
            GateObject.m_nMapX := nDMapX;
            GateObject.m_nMapY := nDMapY;
            GateObject.m_dwRunTime := nTime * 1000;
            GateObject.m_dwRunTick := MyGetTickCount + LongWord(nTime) * 1000;
            if SEnvir.AddToMap(nXX, nYY, GateObject) = GateObject then
            begin
              m_GateList.Add(GateObject);
              if (nSMapX <> nXX) or (nSMapY <> nYY) then
                GateObject.m_boCenter := false;

              Result := True;
              if nDoorIndex in [1 .. 5] then
              begin
                Merchant := TMerchant.Create;
                Merchant.m_sScript := '';
                Merchant.m_sMapName := SEnvir.sMapName;
                Merchant.m_PEnvir := SEnvir;
                Merchant.m_nCurrX := nSMapX;
                Merchant.m_nCurrY := nSMapY;
                Merchant.m_sCharName := '';
                Merchant.m_nFlag := 0;
                Merchant.m_wAppr := nDoorIndex + 53; // 从54开始，固+53（原54) 2019-07-06 11:19:01
                Merchant.m_dwMoveTime := 10;
                Merchant.m_dwNpcAutoChangeColorTime := 0;
                Merchant.m_boCastle := false;
                Merchant.m_boCanMove := false;
                Merchant.m_boNpcAutoChangeColor := false;
                UserEngine.AddMerchant(Merchant);
                Merchant.Initialize;
                GateObject.m_BindNPC := Merchant;
              end;
            end
            else
              GateObject.Free;
          end;
        end;
      end;
    end;
  end;
end;

function TEnvirnoment.GetMainMap(): string;
begin
  if m_boMainMap then
    Result := sMainMapName
  else
    Result := sMapName;
end;

function TEnvirnoment.AllowStdItems(nItemIdx: Integer): Boolean; // 是否允许使用物品
var
  L, H, I, C: Integer;
begin
  Result := True;
  if not m_boUnAllowStdItems then
    Exit;

  if (m_UnAllowStdItemsList = nil) then
    Exit;

  L := 0;
  H := m_UnAllowStdItemsList.Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := Integer(m_UnAllowStdItemsList[I]) - nItemIdx;
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := false;
        Break;
      end;
    end;
  end;
end;

function TEnvirnoment.AllowDropToBagItem(nItemIdx: Integer): Boolean; // 是否支持掉到背包中
var
  L, H, I, C: Integer;
begin
  Result := false;
  if m_DropAddToUserBagItemsList = nil then
    Exit;

  L := 0;
  H := m_DropAddToUserBagItemsList.Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := Integer(m_DropAddToUserBagItemsList[I]) - nItemIdx;
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        Break;
      end;
    end;
  end;
end;

function TEnvirnoment.AllowMagics(nMagIdx: Integer): Boolean; // 是否允许使用魔法
var
  L, H, I, C: Integer;
begin
  Result := True;
  if not m_boUnAllowMagics then
    Exit;

  if m_UnAllowMagicList = nil then
    Exit;

  L := 0;
  H := m_UnAllowMagicList.Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := Integer(m_UnAllowMagicList[I]) - nMagIdx;
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := false;
        Break;
      end;
    end;
  end;
end;

procedure TEnvirnoment.AddDoorToMap();
var
  I: Integer;
  DoorObject: TDoorObject;
begin
  for I := 0 to m_DoorList.Count - 1 do
  begin
    DoorObject := TDoorObject(m_DoorList.Items[I]);
    AddToMap(DoorObject.m_nMapX, DoorObject.m_nMapY, DoorObject);
  end;
end;

function TEnvirnoment.GetDropPosition(nOrgX, nOrgY, nRange: Integer; var nDX: Integer; var nDY: Integer): Boolean;
var
  I, II, III: Integer;
  nItemCount, n24, n28, n2C: Integer;
begin
  Result := false;
  n24 := 999;
  n28 := 0; // 09/10
  n2C := 0; // 09/10
  for I := 1 to nRange do
  begin
    for II := -I to I do
    begin
      for III := -I to I do
      begin
        nDX := nOrgX + III;
        nDY := nOrgY + II;
        if GetItemEx(nDX, nDY, nItemCount) = nil then
        begin
          if bo2C then
          begin
            Result := True;
            Break;
          end;
        end
        else if bo2C and (n24 > nItemCount) then
        begin
          n24 := nItemCount;
          n28 := nDX;
          n2C := nDY;
        end;
      end;
      if Result then
        Break;
    end;
    if Result then
      Break;
  end;

  if not Result then
  begin
    n24 := 999;
    n28 := 0; // 09/10
    n2C := 0; // 09/10
    for I := 1 to nRange do
    begin
      for II := -I to I do
      begin
        for III := -I to I do
        begin
          nDX := nOrgX + III;
          nDY := nOrgY + II;
          if GetItemEx2(nDX, nDY, nItemCount) = nil then
          begin
            if bo2C then
            begin
              Result := True;
              Break;
            end;
          end
          else if bo2C and (n24 > nItemCount) then
          begin
            n24 := nItemCount;
            n28 := nDX;
            n2C := nDY;
          end;
        end;
        if Result then
          Break;
      end;
      if Result then
        Break;
    end;
  end;

  if not Result then
  begin
    if n24 < 8 then
    begin
      nDX := n28;
      nDY := n2C;
    end
    else
    begin
      nDX := nOrgX;
      nDY := nOrgY;
    end;
  end;
end;

function TEnvirnoment.GetDropPosition2(nOrgX, nOrgY, nRange: Integer; var nDX: Integer; var nDY: Integer): Boolean;
var
  I, II, III: Integer;
  nItemCount, n24, n28, n2C: Integer;
begin
  Result := false;
  n24 := 999;
  n28 := 0; // 09/10
  n2C := 0; // 09/10
  for I := 1 to nRange do
  begin
    for II := -I to I do
    begin
      for III := -I to I do
      begin
        nDX := nOrgX + III;
        nDY := nOrgY + II;
        if GetItemEx(nDX, nDY, nItemCount) = nil then
        begin
          if bo2C then
          begin
            Result := True;
            Break;
          end;
        end
        else if bo2C and (n24 > nItemCount) and ((nOrgX <> nDX) or (nOrgY <> nDY)) then
        begin
          n24 := nItemCount;
          n28 := nDX;
          n2C := nDY;
        end;
      end;
      if Result then
        Break;
    end;

    if Result then
      Break;
  end;

  if not Result then
  begin
    n24 := 999;
    n28 := 0; // 09/10
    n2C := 0; // 09/10
    for I := 1 to nRange do
    begin
      for II := -I to I do
      begin
        for III := -I to I do
        begin
          nDX := nOrgX + III;
          nDY := nOrgY + II;
          if GetItemEx2(nDX, nDY, nItemCount) = nil then
          begin
            if bo2C and ((nOrgX <> nDX) or (nOrgY <> nDY)) then
            begin
              Result := True;
              Break;
            end;
          end
          else if bo2C and (n24 > nItemCount) and ((nOrgX <> nDX) or (nOrgY <> nDY)) then
          begin
            n24 := nItemCount;
            n28 := nDX;
            n2C := nDY;
          end;
        end;

        if Result then
          Break;
      end;
      if Result then
        Break;
    end;
  end;

  if not Result then
  begin
    if n24 < 20 then
    begin
      nDX := n28;
      nDY := n2C;
    end
    else
    begin
      nDX := nOrgX;
      nDY := nOrgY;
    end;
  end;
end;

function TEnvirnoment.GetMapCellInfo(nX, nY: Integer; var MapCellInfo: pTMapCellinfo): Boolean;
begin
  // if not m_boInitialize then MainOutMessage('not m_boInitialize:'+sMapName);
  // MainOutMessage(Format('%s %S nX:%d nY:%d m_nWidth:%d m_nHeight:%d',[sMapName,booltostr(MapCellArray<>nil),nX, nY,m_nWidth,m_nHeight]));
  if (MapCellArray <> nil) and (nX >= 0) and (nX < m_nWidth) and (nY >= 0) and (nY < m_nHeight) then
  begin
    MapCellInfo := @MapCellArray[nX * m_nHeight + nY];
    Result := True;
  end
  else
    Result := false;
end;

procedure TEnvirnoment.AddHumBBCount;
begin
  // 由于人物或英雄的宝宝，是先AddToMap，再设置 Master 的，所以在 AddToMap中取不准，这里单独搞一个 chongchong 2015-11-30
  Inc(FHumBBCount);
  if FMonCount > 0 then
    Dec(FMonCount);
end;

function TEnvirnoment.CanAddToMapPosition(nX, nY: Integer): Boolean;
var
  MapCellInfo: pTMapCellinfo;
begin
  Result := false;
  if m_boInvalid then
    Exit;

  Result := GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0);
end;

function TEnvirnoment.AddToMap(nX, nY: Integer; pAddObject: TGameObject): TGameObject;
var
  I, nItemCount: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  nGoldCount: Integer;
  BaseObj: TBaseObject;
resourcestring
  sExceptionMsg = '[Exception] TEnvirnoment.AddToMap';
begin
  Result := nil;
  if m_boInvalid then
    Exit;

  try
    if (not GetMapCellInfo(nX, nY, MapCellInfo)) or (MapCellInfo.chFlag <> 0) then
      Exit;

    if MapCellInfo.ObjList <> nil then
    begin
      if pAddObject.m_ObjGame = Obj_Item then
      begin
        if TItemObject(pAddObject).m_sName = sSTRING_GOLDNAME then
        begin
          for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
          begin
            GameObject := MapCellInfo.ObjList[I];
            if (GameObject <> nil) then
            begin
              if (GameObject = pAddObject) then
              begin
                Result := GameObject;
                Exit;
              end;

              if (GameObject.m_ObjGame = Obj_Item) //
                and (not TItemObject(GameObject).m_boGhost) //
                and (TItemObject(GameObject).m_sName = sSTRING_GOLDNAME) then
              begin
                nGoldCount := TItemObject(GameObject).m_nCount + TItemObject(pAddObject).m_nCount;
                if nGoldCount <= 2000 then
                begin
                  TItemObject(GameObject).m_nMapX := nX;
                  TItemObject(GameObject).m_nMapY := nY;
                  TItemObject(GameObject).m_nCount := nGoldCount;
                  TItemObject(GameObject).m_wLooks := GetGoldShape(nGoldCount);
                  TItemObject(GameObject).m_wAniCount := 0;
                  TItemObject(GameObject).m_btReserved := 0;
                  TItemObject(GameObject).m_dwAddTime := MyGetTickCount();
                  Result := GameObject;
                  // 修正不停的丢金币还是可以刷金币 2019-08-27 17:37:36
                  Exit;
                end;
              end;
            end;
          end;
        end;

        if g_Config.boEnabledMaxMapItemCount then
        begin
          nItemCount := 0; // 计算同一坐标有多少物品了
          for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
          begin
            GameObject := MapCellInfo.ObjList[I];
            if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Item) and (not TItemObject(GameObject).m_boGhost) then
              Inc(nItemCount);
          end;

          if nItemCount >= g_Config.nMaxMapItemCount then
          begin
            Result := nil;
            Exit;
          end;
        end;
      end;
    end;

    pAddObject.m_dwAddTime := MyGetTickCount();
    if (pAddObject.m_ObjGame <> Obj_Gate) then
    begin
      pAddObject.m_nMapX := nX;
      pAddObject.m_nMapY := nY;
    end;
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      LockW(16);
    try
{$IFEND}
{$IF USEOBJLIST = 1}
      SetLength(MapCellInfo.ObjList, Length(MapCellInfo.ObjList) + 1);
      MapCellInfo.ObjList[Length(MapCellInfo.ObjList) - 1] := pAddObject;
{$ELSE}
      if MapCellInfo.ObjList = nil then
        MapCellInfo.ObjList := TSafeList.Create('TEnvirnoment.MapCellInfo.ObjList');
      MapCellInfo.ObjList.Add(pAddObject);
{$IFEND}
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        UnLockW;
    end;
{$IFEND}
    if (pAddObject.m_ObjGame = Obj_Actor) then
    begin
      BaseObj := TBaseObject(pAddObject);
      if not BaseObj.m_boAddToMaped then
      begin
        BaseObj.m_boDelFormMaped := false;
        BaseObj.m_boAddToMaped := True;
      end;

      if BaseObj.m_btRaceServer = RC_PLAYOBJECT then
        Inc(FHumCount)
      else if (BaseObj.Master <> nil) and (BaseObj.Master.m_btRaceServer = RC_PLAYOBJECT) then
        Inc(FHumBBCount)
      else if BaseObj.m_btRaceServer >= RC_ANIMAL then
        Inc(FMonCount);
    end;
    Result := pAddObject;
  except
    on E: Exception do
    begin
      MainOutMessage(sExceptionMsg);
      MainOutMessage(E.Message);
    end;
  end;
end;

function TEnvirnoment.DeleteFromMap(nX, nY: Integer; pRemoveObject: TGameObject): Boolean;
var
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  n18, Index: Integer;
  BaseObj: TBaseObject;
  Code: Integer;
  _sMapName, _sMapDesc: string;
resourcestring
  sExceptionMsg2 = '[Exception] TEnvirnoment.DeleteFromMap, Code: %d; MapName: %s; MapDesc: %s';
begin
  Result := false;
  if m_boInvalid then
    Exit;

  Code := 0;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockW(17);
  try
{$IFEND}
    try
      Code := 1;
      _sMapName := sMapName;
      _sMapDesc := sMapDesc;
      if GetMapCellInfo(nX, nY, MapCellInfo) then
      begin
        Code := 2;
        if MapCellInfo <> nil then
        begin
          Code := 3;
          if MapCellInfo.ObjList <> nil then
          begin
            Code := 4;
            n18 := 0;
            while (True) do
            begin
              Code := 5;
{$IF USEOBJLIST = 1}
              if Length(MapCellInfo.ObjList) <= n18 then
                Break;
{$ELSE}
              if MapCellInfo.ObjList.Count <= n18 then
                Break;
{$IFEND}
              Code := 6;
              GameObject := MapCellInfo.ObjList[n18];
              Code := 7;
              if GameObject = nil then
              begin
                Code := 8;
{$IF USEOBJLIST = 1}
                for Index := n18 to Length(MapCellInfo.ObjList) - 2 do
                  MapCellInfo.ObjList[Index] := MapCellInfo.ObjList[Index + 1];
                Code := 9;
                SetLength(MapCellInfo.ObjList, Length(MapCellInfo.ObjList) - 1);
                Code := 10;
                if Length(MapCellInfo.ObjList) <= 0 then
                begin
                  MapCellInfo.ObjList := nil;
                  Break;
                end;
{$ELSE}
                Code := 11;
                MapCellInfo.ObjList.Delete(n18);
                Code := 12;
                if MapCellInfo.ObjList.Count <= 0 then
                begin
                  FreeAndNil(MapCellInfo.ObjList);
                  Break;
                end;
{$IFEND}
                Continue;
              end
              else if (GameObject = pRemoveObject) then
              begin
                Code := 13;
{$IF USEOBJLIST = 1}
                for Index := n18 to Length(MapCellInfo.ObjList) - 2 do
                  MapCellInfo.ObjList[Index] := MapCellInfo.ObjList[Index + 1];
                SetLength(MapCellInfo.ObjList, Length(MapCellInfo.ObjList) - 1);
{$ELSE}
                MapCellInfo.ObjList.Delete(n18);
{$IFEND}
                Code := 14;
                Result := True;
                Code := 15;
                // 减地图人物怪物计数
                if pRemoveObject.m_ObjGame = Obj_Actor then
                begin
                  BaseObj := TBaseObject(pRemoveObject);
                  Code := 16;
                  if (not BaseObj.m_boDelFormMaped) then
                  begin
                    BaseObj.m_boDelFormMaped := True;
                    BaseObj.m_boAddToMaped := false;
                  end;
                  Code := 17;
                  if BaseObj.m_btRaceServer = RC_PLAYOBJECT then
                  begin
                    Code := 18;
                    if FHumCount > 0 then
                    begin
                      Dec(FHumCount);
                      if (FHumCount = 0) and (FHumBBCount = 0) then
                        FClearHumOrBBTick := MyGetTickCount;
                    end;
                  end
                  else if (BaseObj.Master <> nil) and (BaseObj.Master.m_btRaceServer = RC_PLAYOBJECT) then
                  begin
                    Code := 19;
                    if FHumBBCount > 0 then
                    begin
                      Dec(FHumBBCount);
                      if (FHumCount = 0) and (FHumBBCount = 0) then
                        FClearHumOrBBTick := MyGetTickCount;
                    end;
                  end
                  else if BaseObj.m_btRaceServer >= RC_ANIMAL then
                  begin
                    Code := 20;
                    if FMonCount > 0 then
                      Dec(FMonCount);
                  end;
                  Code := 21;
                end;
                Code := 22;
{$IF USEOBJLIST = 1}
                if Length(MapCellInfo.ObjList) <= 0 then
                begin
                  Code := 23;
                  MapCellInfo.ObjList := nil;
                  Break;
                end;
{$ELSE}
                if MapCellInfo.ObjList.Count <= 0 then
                begin
                  Code := 24;
                  FreeAndNil(MapCellInfo.ObjList);
                  Break;
                end;
{$IFEND}
                Break; // Continue;
              end; // if (GameObject = pRemoveObject) then begin
              Inc(n18);
            end;
          end; // Result := -2;
        end; // else Result := -3;
      end; // else Result := 0;
    except
      on E: Exception do
      begin
        MainOutMessage(Format(sExceptionMsg2, [Code, _sMapName, _sMapDesc]));
        MainOutMessage(E.Message);
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockW;
  end;
{$IFEND}
end;

procedure TEnvirnoment.ShowMapObject(nX, nY: Integer; Cert: TGameObject);
var
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  I: Integer;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(19);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject :=
{$IF USEOBJLIST = 1}TGameObject(MapCellInfo.ObjList[I]){$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if GameObject <> nil then
        begin
          case GameObject.m_ObjGame of
            Obj_Actor:
              TBaseObject(Cert).SysMsg(Format('Obj_Actor ShowMapObject %s  %s nX:%d nY:%d',
                [TBaseObject(GameObject).m_PEnvir.sMapDesc, TBaseObject(GameObject).m_sCharName, TBaseObject(GameObject).m_nCurrX,
                TBaseObject(GameObject).m_nCurrY]), c_Green, t_Hint);
            Obj_Item:
              TBaseObject(Cert).SysMsg(Format('Obj_Item ShowMapObject %s nX:%d nY:%d', [TItemObject(GameObject).m_sName, nX, nY]),
                c_Green, t_Hint);
            Obj_Event:
              TBaseObject(Cert).SysMsg(Format('Obj_Event ShowMapObject nX:%d nY:%d', [nX, nY]), c_Green, t_Hint);
            Obj_Gate:
              TBaseObject(Cert).SysMsg(Format('Obj_Gate ShowMapObject nX:%d nY:%d', [nX, nY]), c_Green, t_Hint);
            Obj_Door:
              TBaseObject(Cert).SysMsg(Format('Obj_Door ShowMapObject nX:%d nY:%d', [nX, nY]), c_Green, t_Hint);
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.MoveToMovingObject(nCX, nCY: Integer; Cert: TGameObject; nX, nY: Integer; boFlag: Boolean): Boolean;
var
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
  I, Index: Integer;
  bo1A: Boolean;
  // nErrorCode: Integer;
  boTempFixedHideMode: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TEnvirnoment.MoveToMovingObject';
  // label
  // Loop, Over;
begin
  Result := false;
  if m_boInvalid then
    Exit;
  try
    bo1A := True;
    if not boFlag and GetMapCellInfo(nX, nY, MapCellInfo) then
    begin
      if (MapCellInfo.chFlag = 0) then
      begin
        if (MapCellInfo.ObjList <> nil) then
        begin // 检测是否可以移动到目的地坐标
          for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
          begin
            GameObject := MapCellInfo.ObjList[I];
            if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
            begin
              BaseObject := TBaseObject(GameObject); // 检测移动目的地是否有该角色
              // 宠物无实体模式 2019-11-15 22:18:42
              if (BaseObject <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.m_boGamePet) and g_Config.boPetNoEntity then
                Continue;

              // 拦道的是自己无视掉 chongchong 2014-05-14
              if BaseObject = Cert then
                Continue;

              // 传送门可以走过去 piaoyun 2013-07-26
              if BaseObject.m_btRaceServer = RC_NPC then
              begin
                if { g_Config.boRunNpc or } (BaseObject.m_wAppr in [54 .. 58, 94 .. 98]) then
                  Continue;
              end;

              if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
              begin
                boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0);
                if not boTempFixedHideMode then
                begin
                  { 修订: 被邀请骑马的人可以跑到邀请骑马人那里 chongchong 2013-10-24 }
                  boTempFixedHideMode := (TSmartObject(BaseObject).m_boOnHorse // 被人邀请骑马 chongchong 2013-10-15
                    and (not TSmartObject(BaseObject).m_boHorseMaster));

                  if not boTempFixedHideMode then
                  begin
                    if Cert is TSmartObject then
                    begin
                      if TSmartObject(Cert).m_boOnHorse //
                        and (not TSmartObject(Cert).m_boHorseMaster) //
                        and (BaseObject = TSmartObject(Cert).m_HorseOtherHum) then
                        boTempFixedHideMode := True;
                    end;
                  end;
                end;
              end
              else
                boTempFixedHideMode := false;

              if not BaseObject.m_boGhost //
                and BaseObject.bo2B9 //
                and not BaseObject.m_boDeath //
                and not BaseObject.m_boFixedHideMode //
                and not BaseObject.m_boObMode //
                and (not boTempFixedHideMode) then
              begin
                bo1A := false;
                Break;
              end;
            end;
          end;
        end;
      end
      else
      begin // if MapCellInfo.chFlag = 0 then begin
        bo1A := false;
      end;
    end;
    // -----------------------------------------------------------------------------
    if bo1A then
    begin
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        LockW(17);
      try
{$IFEND}
        if not GetMapCellInfo(nX, nY, MapCellInfo) or (MapCellInfo.chFlag = 0) then
        begin
          bo1A := false;
          if GetMapCellInfo(nCX, nCY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
          begin
            I := 0;
            while (True) do
            begin
{$IF USEOBJLIST = 1}
              if Length(MapCellInfo.ObjList) <= I then
                Break;
{$ELSE}
              if MapCellInfo.ObjList.Count <= I then
                Break;
{$IFEND}
              GameObject := MapCellInfo.ObjList[I];
              if GameObject = Cert then
              begin
                bo1A := True;
{$IF USEOBJLIST = 1}
                for Index := I to Length(MapCellInfo.ObjList) - 2 do
                  MapCellInfo.ObjList[Index] := MapCellInfo.ObjList[Index + 1];
                SetLength(MapCellInfo.ObjList, Length(MapCellInfo.ObjList) - 1);
                if Length(MapCellInfo.ObjList) <= 0 then
                begin
                  MapCellInfo.ObjList := nil;
                  Break;
                end;
{$ELSE}
                MapCellInfo.ObjList.Delete(I);
                if MapCellInfo.ObjList.Count <= 0 then
                begin
                  FreeAndNil(MapCellInfo.ObjList);
                  Break;
                end;
{$IFEND}
                Break; // Continue;
              end;
              Inc(I);
            end; // while (True) do begin
          end
          else
            bo1A := True;

          // 如果坐标上没有删除对象，则不成功 chongchong 2018-08-28 19:45:29
          if bo1A and GetMapCellInfo(nX, nY, MapCellInfo) then
          begin
            { for I := Length(MapCellInfo.ObjList) - 1 downto 0 do begin
              GameObject := MapCellInfo.ObjList[I];
              if GameObject = Cert then begin
              for Index := I to Length(MapCellInfo.ObjList) - 2 do
              MapCellInfo.ObjList[Index] := MapCellInfo.ObjList[Index + 1];
              SetLength(MapCellInfo.ObjList, Length(MapCellInfo.ObjList) - 1);
              end;
              end; }
            Cert.m_dwAddTime := MyGetTickCount;
{$IF USEOBJLIST = 1}
            SetLength(MapCellInfo.ObjList, Length(MapCellInfo.ObjList) + 1);
            MapCellInfo.ObjList[Length(MapCellInfo.ObjList) - 1] := Cert;
{$ELSE}
            if MapCellInfo.ObjList = nil then
              MapCellInfo.ObjList := TSafeList.Create('TEnvirnoment.MapCellInfo.ObjList');
            MapCellInfo.ObjList.Add(Cert);
{$IFEND}
            Result := True;
          end
          else if not bo1A then
          begin
            // OutputDebugString('aaa');
          end;
        end;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          UnLockW;
      end;
{$IFEND}
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(sExceptionMsg);
      MainOutMessage(E.Message);
    end;
  end;
end;

// ======================================================================
// 检查地图指定座标是否可以移动
// boFlag  如果为TRUE 则忽略座标上是否有角色
// 返回值 True 为可以移动，False 为不可以移动
// ======================================================================
function TEnvirnoment.CanWalk(nX, nY: Integer; boFlag: Boolean): Boolean;
var
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
  I: Integer;
  boTempFixedHideMode: Boolean;
begin
  Result := false;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(22);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
    begin
      Result := True;
      if not boFlag and (MapCellInfo.ObjList <> nil) then
      begin
        for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
        begin
          GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
          if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
          begin
            BaseObject := TBaseObject(GameObject);

            // 宠物无实体模式 2019-11-15 22:18:42
            if (BaseObject <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.m_boGamePet) and g_Config.boPetNoEntity then
              Continue;

            if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
              boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
                (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
            else // 被人邀请骑马 chongchong 2013-10-15
              boTempFixedHideMode := false;

            if not BaseObject.m_boGhost //
              and BaseObject.bo2B9 //
              and not BaseObject.m_boDeath //
              and not BaseObject.m_boFixedHideMode //
              and not BaseObject.m_boObMode //
              and (not boTempFixedHideMode) then
            begin
              Result := false;
              Break;
            end;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

// ======================================================================
// 检查地图指定座标是否可以移动
// boFlag  如果为TRUE 则忽略座标上是否有角色
// 返回值 True 为可以移动，False 为不可以移动
// ======================================================================
function TEnvirnoment.CanWalkOfItem(nX, nY: Integer; boFlag, boItem: Boolean): Boolean;
var
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
  I: Integer;
  boTempFixedHideMode: Boolean;
begin
  Result := True;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(23);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
    begin
      // Result:=True;
      if (MapCellInfo.ObjList <> nil) then
      begin
        for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
        begin
          GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
          if not boFlag and (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
          begin
            BaseObject := TBaseObject(GameObject);
            // 宠物无实体模式 2019-11-15 22:18:42
            if (BaseObject <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.m_boGamePet) and g_Config.boPetNoEntity then
              Continue;

            if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
              boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
                (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
            else // 被人邀请骑马 chongchong 2013-10-15
              boTempFixedHideMode := false;

            if not BaseObject.m_boGhost //
              and BaseObject.bo2B9 //
              and not BaseObject.m_boDeath //
              and not BaseObject.m_boFixedHideMode //
              and not BaseObject.m_boObMode //
              and (not boTempFixedHideMode) then
            begin
              Result := false;
              Break;
            end;
          end;

          if not boItem and (GameObject.m_ObjGame = Obj_Item) then
          begin
            Result := false;
            Break;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.CanWalkEx(WalkObject: TGameObject; nX, nY: Integer; boFlag: Boolean): Boolean;
var
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
  I: Integer;
  Castle: TUserCastle;
  boRUNHUMAN: Boolean;
  boRUNMON: Boolean;
  boTempFixedHideMode: Boolean;
  boTemp: Boolean;
  IsPlaymoster: Boolean;
begin
  if not(GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0)) then
  begin
    Result := false;
    Exit;
  end;

  Result := True;
  // if not boFlag and (MapCellInfo.ObjList <> nil) then
  if boFlag or (MapCellInfo.ObjList = nil) then
    Exit;

  if TBaseObject(WalkObject).m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
  begin
    boRUNHUMAN := TSmartObject(WalkObject).m_boRUNHUMAN;
    boRUNMON := TSmartObject(WalkObject).m_boRUNMON;
  end
  else
  begin
    boRUNHUMAN := false;
    boRUNMON := false;
  end;
  // 人形怪什么飞机都不允许穿 chongchong 2018-06-28 00:01:49
  IsPlaymoster := TBaseObject(WalkObject).m_btRaceServer in [RC_PLAYMOSTER];
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(24);
  try
{$IFEND}
    if TBaseObject(WalkObject).m_btRaceServer = RC_HEROOBJECT then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if GameObject = nil then
          Continue;

        if GameObject.m_ObjGame = Obj_Actor then
        begin
          BaseObject := TBaseObject(GameObject);
          // 宠物无实体模式 2019-11-15 22:18:42
          if (BaseObject <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.m_boGamePet) and g_Config.boPetNoEntity then
            Continue;

          if not IsPlaymoster then
          begin
            Castle := g_CastleManager.InCastleWarArea(BaseObject);
            if g_Config.boHeroWarDisHumRun and (Castle <> nil) and (Castle.m_boUnderWar) then
            begin
              // 攻城区域允许穿过英雄 -- piaoyun 2013-07-17
              if g_Config.boHeroWarHreoRun and (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
                Continue;
            end
            else
            begin
              if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
              begin
                if (not m_boNoRunHuman) then // 地图参数-禁止穿人 chongchong 2016-09-06
                begin
                  // 修正安全区不受控制（禁止穿人的模式下）不能穿英雄的问题 chongchong 2015-01-22
                  if BaseObject.InSafeZone and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
                  begin
                    // "允许穿过人物" 或者 "安全区不受控制"
                    // aaa := g_Config.boRUNHUMAN or m_boRUNHUMAN or boRUNHUMAN;
                    if (g_Config.boHeroRunHum or m_boRUNHUMAN or boRUNHUMAN or g_Config.boHeroSafeAreaLimited) then
                    begin
                      if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
                      begin
                        boTemp := (g_Config.boSafeAreaDisShopStallHeroRun and TPlayObject(BaseObject).m_boShopStall) or
                          (g_Config.boSafeAreaDisOffLineHeroRun and TPlayObject(BaseObject).m_boOffLine);
                      end
                      else
                        boTemp := false;

                      // 这里按常规写法出逻辑出错，用点另类方法了，别改 -- piaoyun 2013-07-17
                      if not boTemp then
                        Continue
                      else
                      begin
{$IFNDEF CPUX64}
                        asm
                          nop
                          nop
                        end;
{$ENDIF}
                      end;
                    end;
                  end
                  else if g_Config.boHeroRunHum or m_boRUNHUMAN or boRUNHUMAN then // 对象在非安全
                    Continue;
                end;
              end
              else
              begin
                if BaseObject.m_btRaceServer = RC_NPC then
                begin
                  // if g_Config.boRunNpc or (BaseObject.m_wAppr in [54..58, 94..98]) then Continue;
                  if BaseObject.InSafeZone then
                  begin
                    // 在安全区的情况
                    if ((g_Config.boHeroRunNpc or g_Config.boHeroSafeAreaLimited or (BaseObject.m_wAppr in [54 .. 58, 94 .. 98]))
                      and (not g_Config.boHeroSafeAreaDisNpcRun { 不允许穿NPC } )) then
                      Continue;
                  end
                  else if g_Config.boHeroRunNpc or (BaseObject.m_wAppr in [54 .. 58, 94 .. 98]) then
                    Continue;
                end
                else
                begin
                  if BaseObject.m_btRaceServer in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD] then
                  begin
                    if g_Config.boHeroRunGuard or (g_Config.boHeroSafeAreaLimited and BaseObject.InSafeZone) then
                      Continue;
                  end
                  else
                  begin
                    if BaseObject.m_btRaceServer <> 55 then
                    begin // 不允许穿过练功师
                      if (not m_boNoRunMon) then // 地图参数-禁止穿怪 chongchong 2016-09-06
                      begin
                        // 安全区允许穿怪 piaoyun 2013-12-20
                        if g_Config.boHeroRunMon or m_boRUNMON or boRUNMON or
                          (g_Config.boHeroSafeAreaLimited and BaseObject.InSafeZone) then
                          Continue;
                      end;
                    end;
                  end;
                end;
              end;
            end;
          end;

          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
            boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
              (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
          else // 被人邀请骑马 chongchong 2013-10-15
            boTempFixedHideMode := false;

          if not BaseObject.m_boGhost //
            and BaseObject.bo2B9 //
            and not BaseObject.m_boDeath //
            and not BaseObject.m_boFixedHideMode //
            and not BaseObject.m_boObMode //
            and (not boTempFixedHideMode) then
          begin
            Result := false;
            Break;
          end;
        end;
      end;
    end
    else if (TBaseObject(WalkObject).m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(WalkObject).m_boDummyObject) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if GameObject = nil then
          Continue;

        if GameObject.m_ObjGame = Obj_Actor then
        begin
          BaseObject := TBaseObject(GameObject);
          // 宠物无实体模式 2019-11-15 22:18:42
          if (BaseObject <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.m_boGamePet) and g_Config.boPetNoEntity then
            Continue;

          if not IsPlaymoster then
          begin
            Castle := g_CastleManager.InCastleWarArea(BaseObject);
            if g_Config.boDummyWarDisHumRun and (Castle <> nil) and (Castle.m_boUnderWar) then
            begin
              // 攻城区域允许穿过英雄 -- piaoyun 2013-07-17
              if g_Config.boDummyWarHreoRun and (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
                Continue;
            end
            else
            begin
              if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
              begin
                {
                  // 主人可以穿自己的英雄 chongchong 2016-03-11
                  if (TBaseObject(WalkObject).m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(WalkObject).m_MyHero = BaseObject) then
                  Continue;
                }
                {
                  // 主人不允许穿英雄  chongchong 2017-10-31
                  if (TPlayObject(WalkObject).m_MyHero = BaseObject) and (not BaseObject.InSafeArea) then
                  begin
                  Result := False;
                  Exit;
                  end;
                }
                if (not m_boNoRunHuman) then // 地图参数-禁止穿人 chongchong 2016-09-06
                begin
                  // 修正安全区不受控制（禁止穿人的模式下）不能穿英雄的问题 chongchong 2015-01-22
                  if BaseObject.InSafeZone and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
                  begin
                    // "允许穿过人物" 或者 "安全区不受控制"
                    // aaa := g_Config.boRUNHUMAN or m_boRUNHUMAN or boRUNHUMAN;
                    if (g_Config.boDummyRunHum or m_boRUNHUMAN or boRUNHUMAN or g_Config.boDummySafeAreaLimited) then
                    begin
                      if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
                      begin
                        boTemp := (g_Config.boSafeAreaDisShopStallDummyRun and TPlayObject(BaseObject).m_boShopStall) or
                          (g_Config.boSafeAreaDisOffLineDummyRun and TPlayObject(BaseObject).m_boOffLine);
                      end
                      else
                        boTemp := false;

                      // 这里按常规写法出逻辑出错，用点另类方法了，别改 -- piaoyun 2013-07-17
                      if not boTemp then
                        Continue
                      else
                      begin
{$IFNDEF CPUX64}
                        asm
                          nop
                          nop
                        end;
{$ENDIF}
                      end;
                    end;
                  end
                  else if g_Config.boDummyRunHum or m_boRUNHUMAN or boRUNHUMAN then // 对象在非安全
                    Continue;
                end;
              end
              else
              begin
                if BaseObject.m_btRaceServer = RC_NPC then
                begin
                  // if g_Config.boRunNpc or (BaseObject.m_wAppr in [54..58, 94..98]) then Continue;
                  if BaseObject.InSafeZone then
                  begin
                    // 在安全区的情况
                    if ((g_Config.boDummyRunNpc or g_Config.boDummySafeAreaLimited //
                      or (BaseObject.m_wAppr in [54 .. 58, 94 .. 98])) //
                      and (not g_Config.boDummySafeAreaDisNpcRun { 不允许穿NPC } )) then
                      Continue;
                  end
                  else if g_Config.boDummyRunNpc or (BaseObject.m_wAppr in [54 .. 58, 94 .. 98]) then
                    Continue;
                end
                else
                begin
                  if BaseObject.m_btRaceServer in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD] then
                  begin
                    if g_Config.boDummyRunGuard or (g_Config.boDummySafeAreaLimited and BaseObject.InSafeZone) then
                      Continue;
                  end
                  else
                  begin
                    if BaseObject.m_btRaceServer <> 55 then
                    begin // 不允许穿过练功师
                      if (not m_boNoRunMon) then // 地图参数-禁止穿怪 chongchong 2016-09-06
                      begin
                        // 安全区允许穿怪 piaoyun 2013-12-20
                        if g_Config.boDummyRunMon or m_boRUNMON or boRUNMON or
                          (g_Config.boDummySafeAreaLimited and BaseObject.InSafeZone) then
                          Continue;
                      end;
                    end;
                  end;
                end;
              end;
            end;
          end;

          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
            boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
              (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
          else // 被人邀请骑马 chongchong 2013-10-15
            boTempFixedHideMode := false;

          if not BaseObject.m_boGhost //
            and BaseObject.bo2B9 //
            and not BaseObject.m_boDeath //
            and not BaseObject.m_boFixedHideMode //
            and not BaseObject.m_boObMode //
            and (not boTempFixedHideMode) then
          begin
            Result := false;
            Break;
          end;
        end;
      end;
    end
    else
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if GameObject = nil then
          Continue;

        if GameObject.m_ObjGame = Obj_Actor then
        begin
          BaseObject := TBaseObject(GameObject);
          // 宠物无实体模式 2019-11-15 22:18:42
          if (BaseObject <> nil) and (BaseObject.m_Master <> nil) and (BaseObject.m_boGamePet) and g_Config.boPetNoEntity then
            Continue;

          if not IsPlaymoster then
          begin
            Castle := g_CastleManager.InCastleWarArea(BaseObject);
            if g_Config.boWarDisHumRun and (Castle <> nil) and (Castle.m_boUnderWar) then
            begin
              // 攻城区域允许穿过英雄 -- piaoyun 2013-07-17
              if g_Config.boWarHreoRun and (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
                Continue;
            end
            else
            begin
              if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT] then
              begin
                {
                  // 主人可以穿自己的英雄 chongchong 2016-03-11
                  if (TBaseObject(WalkObject).m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(WalkObject).m_MyHero = BaseObject) then
                  Continue;
                }
                {
                  // 主人不允许穿英雄  chongchong 2017-10-31
                  if (WalkObject <> nil) and (TBaseObject(WalkObject).m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(WalkObject).m_MyHero = BaseObject) and (not BaseObject.InSafeArea) then
                  begin
                  Result := False;
                  Exit;
                  end;
                }
                if (not m_boNoRunHuman) then // 地图参数-禁止穿人 chongchong 2016-09-06
                begin
                  // 修正安全区不受控制（禁止穿人的模式下）不能穿英雄的问题 chongchong 2015-01-22
                  if BaseObject.InSafeZone and (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
                  begin
                    // "允许穿过人物" 或者 "安全区不受控制"
                    // aaa := g_Config.boRUNHUMAN or m_boRUNHUMAN or boRUNHUMAN;
                    if (g_Config.boRUNHUMAN or m_boRUNHUMAN or boRUNHUMAN or g_Config.boSafeAreaLimited) then
                    begin
                      if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
                      begin
                        boTemp := (g_Config.boSafeAreaDisShopStallHumRun and TPlayObject(BaseObject).m_boShopStall) or
                          (g_Config.boSafeAreaDisOffLineHumRun and TPlayObject(BaseObject).m_boOffLine);
                      end
                      else
                        boTemp := false;

                      // 这里按常规写法出逻辑出错，用点另类方法了，别改 -- piaoyun 2013-07-17
                      if not boTemp then
                        Continue
                      else
                      begin
{$IFNDEF CPUX64}
                        asm
                          nop
                          nop
                        end;
{$ENDIF}
                      end;
                    end;
                  end
                  else if g_Config.boRUNHUMAN or m_boRUNHUMAN or boRUNHUMAN then // 对象在非安全
                    Continue;
                end;
              end
              else
              begin
                if BaseObject.m_btRaceServer = RC_NPC then
                begin
                  // if g_Config.boRunNpc or (BaseObject.m_wAppr in [54..58, 94..98]) then Continue;
                  if BaseObject.InSafeZone then
                  begin
                    // 在安全区的情况
                    if ((g_Config.boRunNpc or g_Config.boSafeAreaLimited or (BaseObject.m_wAppr in [54 .. 58, 94 .. 98])) and
                      (not g_Config.boSafeAreaDisNpcRun { 不允许穿NPC } )) then
                      Continue;
                  end
                  else if g_Config.boRunNpc or (BaseObject.m_wAppr in [54 .. 58, 94 .. 98]) then
                    Continue;
                end
                else
                begin
                  if BaseObject.m_btRaceServer in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD] then
                  begin
                    if g_Config.boRunGuard or (g_Config.boSafeAreaLimited and BaseObject.InSafeZone) then
                      Continue;
                  end
                  else if BaseObject.m_btRaceServer <> 55 then
                  begin
                    // 不允许穿过练功师
                    if (not m_boNoRunMon) then // 地图参数-禁止穿怪 chongchong 2016-09-06
                    begin
                      // 安全区允许穿怪 piaoyun 2013-12-20
                      if g_Config.boRUNMON or m_boRUNMON or boRUNMON //
                        or (g_Config.boSafeAreaLimited and BaseObject.InSafeZone) then
                        Continue;
                    end;
                  end;
                end;
              end;
            end;
          end;

          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
            boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
              (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
          else // 被人邀请骑马 chongchong 2013-10-15
            boTempFixedHideMode := false;

          if not BaseObject.m_boGhost //
            and BaseObject.bo2B9 //
            and not BaseObject.m_boDeath //
            and not BaseObject.m_boFixedHideMode //
            and not BaseObject.m_boObMode //
            and (not boTempFixedHideMode) then
          begin
            Result := false;
            Break;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.CanWalkEx2(WalkObject: TGameObject; nX, nY: Integer; boFlag: Boolean): Boolean;
var
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
  I: Integer;
  Castle: TUserCastle;
  boTempFixedHideMode: Boolean;
begin
  Result := false;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(25);
  try
{$IFEND}
    if (WalkObject <> nil) and (TBaseObject(WalkObject).m_btRaceServer = RC_HEROOBJECT) then
    begin
      if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
      begin
        Result := True;
        if not boFlag and (MapCellInfo.ObjList <> nil) then
        begin
          for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
          begin
            GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
            if GameObject <> nil then
            begin
              if GameObject.m_ObjGame = Obj_Actor then
              begin
                BaseObject := TBaseObject(GameObject);
                // 宠物无实体模式 2019-11-15 22:18:42
                if (BaseObject <> nil) //
                  and (BaseObject.m_Master <> nil) //
                  and (BaseObject.m_boGamePet) //
                  and g_Config.boPetNoEntity then
                  Continue;

                Castle := g_CastleManager.InCastleWarArea(BaseObject);
                if g_Config.boHeroWarDisHumRun and (Castle <> nil) and (Castle.m_boUnderWar) then
                begin
                  // 攻城区域允许穿过英雄 -- piaoyun 2013-07-17
                  if g_Config.boHeroWarHreoRun and (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
                  begin
                    Continue;
                  end;
                end
                else
                begin
                  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                  begin
                    if (not m_boNoRunHuman) then // 地图参数-禁止穿人 chongchong 2016-09-06
                    begin
                      if g_Config.boHeroRunHum or m_boRUNHUMAN or TSmartObject(BaseObject).m_boRUNHUMAN then
                        Continue;
                    end;
                  end
                  else
                  begin
                    if BaseObject.m_btRaceServer = RC_NPC then
                    begin
                      if g_Config.boHeroRunNpc or (BaseObject.m_wAppr in [54 .. 58, 94 .. 98]) then
                        Continue;
                    end
                    else if BaseObject.m_btRaceServer in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD] then
                    begin
                      if g_Config.boHeroRunGuard then
                        Continue;
                    end
                    else if BaseObject.m_btRaceServer <> 55 then
                    begin
                      // 不允许穿过练功师
                      if (not m_boNoRunMon) then // 地图参数-禁止穿怪 chongchong 2016-09-06
                      begin
                        if g_Config.boHeroRunMon or m_boRUNMON then
                          Continue;
                      end;
                    end;
                  end;
                end;

                if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                  boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
                    (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
                else // 被人邀请骑马 chongchong 2013-10-15
                  boTempFixedHideMode := false;

                if not BaseObject.m_boGhost //
                  and BaseObject.bo2B9 //
                  and not BaseObject.m_boDeath //
                  and not BaseObject.m_boFixedHideMode //
                  and not BaseObject.m_boObMode //
                  and (not boTempFixedHideMode) then
                begin
                  Result := false;
                  Break;
                end;
              end;
            end;
          end;
        end;
      end;
    end
    else if (WalkObject <> nil) //
      and (TBaseObject(WalkObject).m_btRaceServer = RC_PLAYOBJECT) //
      and (TPlayObject(WalkObject).m_boDummyObject) then
    begin
      if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
      begin
        Result := True;
        if not boFlag and (MapCellInfo.ObjList <> nil) then
        begin
          for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
          begin
            GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
            if GameObject <> nil then
            begin
              if GameObject.m_ObjGame = Obj_Actor then
              begin
                BaseObject := TBaseObject(GameObject);

                // 宠物无实体模式 2019-11-15 22:18:42
                if (BaseObject <> nil) //
                  and (BaseObject.m_Master <> nil) //
                  and (BaseObject.m_boGamePet) //
                  and g_Config.boPetNoEntity then
                  Continue;

                Castle := g_CastleManager.InCastleWarArea(BaseObject);
                if g_Config.boDummyWarDisHumRun and (Castle <> nil) and (Castle.m_boUnderWar) then
                begin
                  // 攻城区域允许穿过英雄 -- piaoyun 2013-07-17
                  if g_Config.boDummyWarHreoRun and (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
                  begin
                    Continue;
                  end;
                end
                else
                begin
                  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                  begin
                    if (not m_boNoRunHuman) then // 地图参数-禁止穿人 chongchong 2016-09-06
                    begin
                      if g_Config.boDummyRunHum or m_boRUNHUMAN or TSmartObject(BaseObject).m_boRUNHUMAN then
                        Continue;
                    end;
                  end
                  else
                  begin
                    if BaseObject.m_btRaceServer = RC_NPC then
                    begin
                      if g_Config.boDummyRunNpc or (BaseObject.m_wAppr in [54 .. 58, 94 .. 98]) then
                        Continue;
                    end
                    else if BaseObject.m_btRaceServer in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD] then
                    begin
                      if g_Config.boDummyRunGuard then
                        Continue;
                    end
                    else
                    begin
                      if BaseObject.m_btRaceServer <> 55 then
                      begin // 不允许穿过练功师
                        if (not m_boNoRunMon) then // 地图参数-禁止穿怪 chongchong 2016-09-06
                        begin
                          if g_Config.boDummyRunMon or m_boRUNMON then
                            Continue;
                        end;
                      end;
                    end;
                  end;
                end;

                if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                  boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
                    (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
                else // 被人邀请骑马 chongchong 2013-10-15
                  boTempFixedHideMode := false;

                if not BaseObject.m_boGhost //
                  and BaseObject.bo2B9 //
                  and not BaseObject.m_boDeath //
                  and not BaseObject.m_boFixedHideMode //
                  and not BaseObject.m_boObMode //
                  and (not boTempFixedHideMode) then
                begin
                  Result := false;
                  Break;
                end;
              end;
            end;
          end;
        end;
      end;
    end
    else
    begin
      if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
      begin
        Result := True;
        if not boFlag and (MapCellInfo.ObjList <> nil) then
        begin
          for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
          begin
            GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
            if GameObject <> nil then
            begin
              if GameObject.m_ObjGame = Obj_Actor then
              begin
                BaseObject := TBaseObject(GameObject);
                // 宠物无实体模式 2019-11-15 22:18:42
                if (BaseObject <> nil) //
                  and (BaseObject.m_Master <> nil) //
                  and (BaseObject.m_boGamePet) //
                  and g_Config.boPetNoEntity then
                  Continue;

                Castle := g_CastleManager.InCastleWarArea(BaseObject);
                if g_Config.boWarDisHumRun and (Castle <> nil) and (Castle.m_boUnderWar) then
                begin
                  // 攻城区域允许穿过英雄 -- piaoyun 2013-07-17
                  if g_Config.boWarHreoRun and (BaseObject.m_btRaceServer = RC_HEROOBJECT) then
                    Continue;
                end
                else
                begin
                  if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                  begin
                    if (not m_boNoRunHuman) then // 地图参数-禁止穿人 chongchong 2016-09-06
                    begin
                      if g_Config.boRUNHUMAN or m_boRUNHUMAN or TSmartObject(BaseObject).m_boRUNHUMAN then
                        Continue;
                    end;
                  end
                  else
                  begin
                    if BaseObject.m_btRaceServer = RC_NPC then
                    begin
                      if g_Config.boRunNpc or (BaseObject.m_wAppr in [54 .. 58, 94 .. 98]) then
                        Continue;
                    end
                    else if BaseObject.m_btRaceServer in [RC_GUARD, 12, RC_ARCHERGUARD, RC_MOVE_ARCHERGUARD] then
                    begin
                      if g_Config.boRunGuard then
                        Continue;
                    end
                    else
                    begin
                      if BaseObject.m_btRaceServer <> 55 then
                      begin // 不允许穿过练功师
                        if (not m_boNoRunMon) then // 地图参数-禁止穿怪 chongchong 2016-09-06
                        begin
                          if g_Config.boRUNMON or m_boRUNMON then
                            Continue;
                        end;
                      end;
                    end;
                  end;
                end;
                if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                  boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
                    (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
                else // 被人邀请骑马 chongchong 2013-10-15
                  boTempFixedHideMode := false;

                if not BaseObject.m_boGhost //
                  and BaseObject.bo2B9 //
                  and not BaseObject.m_boDeath //
                  and not BaseObject.m_boFixedHideMode //
                  and not BaseObject.m_boObMode //
                  and (not boTempFixedHideMode) then
                begin
                  Result := false;
                  Break;
                end;
              end;
            end;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.CanWalkEx3(nX, nY: Integer; Flag: TWalkFlagArr): Boolean;
var
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
  I: Integer;
  Castle: TUserCastle;
  boTempFixedHideMode: Boolean;
begin
  Result := false;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(48);
  try
{$IFEND}
    // 十步一杀可穿障碍物 (wf_Obstacle in Flag) chongchong 2014-04-16
    if GetMapCellInfo(nX, nY, MapCellInfo) and ((MapCellInfo.chFlag = 0) or (wf_Obstacle in Flag)) then
    begin
      Result := True;
      if (MapCellInfo.ObjList <> nil) then
      begin
        for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
        begin
          GameObject :=
{$IF USEOBJLIST = 1}TGameObject(MapCellInfo.ObjList[I]){$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
          if GameObject <> nil then
          begin
            if GameObject.m_ObjGame = Obj_Actor then
            begin
              BaseObject := TBaseObject(GameObject);
              if BaseObject <> nil then
              begin
                // 宠物无实体模式 2019-11-15 22:18:42
                if (BaseObject.m_Master <> nil) and (BaseObject.m_boGamePet) and g_Config.boPetNoEntity then
                  Continue;

                // 攻城区域处理 -- piaoyun 2013-06-25
                Castle := nil;
                if (wf_War in Flag) then
                  Castle := g_CastleManager.InCastleWarArea(BaseObject);
                if not((wf_War in Flag) and (Castle <> nil) and (Castle.m_boUnderWar)) then
                begin
                  case BaseObject.m_btRaceServer of
                    RC_PLAYOBJECT:
                      begin
                        if wf_Hum in Flag then
                          Continue;
                      end;
                    RC_NPC:
                      begin
                        if wf_Npc in Flag then
                          Continue;
                      end;
                    RC_GUARD, RC_ARCHERGUARD:
                      begin
                        if wf_Guard in Flag then
                          Continue;
                      end;
                  else
                    if wf_Mon in Flag then
                      Continue;
                  end;
                end;

                if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
                  boTempFixedHideMode := { (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or }
                    (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
                else // 被人邀请骑马 chongchong 2013-10-15
                  boTempFixedHideMode := false;

                if not BaseObject.m_boGhost //
                  and BaseObject.bo2B9 //
                  and not BaseObject.m_boDeath //
                  and not BaseObject.m_boFixedHideMode //
                  and not BaseObject.m_boObMode //
                  and not boTempFixedHideMode { and BaseObject.m_boMapApoise } then
                begin
                  Result := false;
                  Break;
                end;
              end;
            end;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

constructor TMapManager.Create;
begin
  inherited Create;

  m_GateList := TGList.Create;
  m_nProcMapIDx := 0;
  m_nProcGateIDx := 0;
end;

destructor TMapManager.Destroy;
var
  I: Integer;
begin
  for I := 0 to m_GateList.Count - 1 do
    TGateObject(m_GateList.Items[I]).Free;
  m_GateList.Free;

  for I := 0 to Count - 1 do
    TEnvirnoment(Items[I]).Free;
  inherited;
end;

function TMapManager.Find(const sMapName: string; var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
  Result := false;
  L := 0;
  H := Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := AnsiCompareText(TEnvirnoment(Items[I]).sMapName, sMapName);
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        // if Duplicates <> dupAccept then L := I;
      end;
    end;
  end;
  Index := L;
end;

function TMapManager.FindMap(sMapName: string): TEnvirnoment;
var
{$IF NEED_KEY = 2}
  Envir: TEnvirnoment;
{$IFEND}
  I: Integer;
begin
  Result := nil;
  if sMapName = '' then
    Exit;

{$IF NEED_KEY = 2}
  // 优化 chongchong 2018-05-06
  Lock;
  try
    for I := 0 to Count - 1 do
    begin
      Envir := TEnvirnoment(Items[I]);
      if SameText(Envir.sMapName, sMapName) then
      begin
        Result := Envir;
        Break;
      end;
    end;
  finally
    UnLock;
  end;
{$ELSE}
  Lock;
  try
    if Find(sMapName, I) then
      Result := Items[I];
  finally
    UnLock;
  end;
{$IFEND}
end;

function TMapManager.GetMapInfo(nServerIdx: Integer; sMapName: string): TEnvirnoment;
var
  Index, I: Integer;
  Envir: TEnvirnoment;
begin
  Result := nil;
  if sMapName = '' then
    Exit;
  // 优化 chongchong 2018-05-06
  {
    Lock;
    try
    for I := 0 to Count - 1 do
    begin
    Envir := Items[I];
    if (Envir.nServerIndex = nServerIdx) and (CompareText(Envir.sMapName, sMapName) = 0) then
    begin
    Result := Envir;
    Break;
    end;
    end;
    finally
    UnLock;
    end;
  }
  Lock;
  try
    if Find(sMapName, Index) then
    begin
      Envir := Items[Index];
      if Envir.nServerIndex = nServerIdx then
      begin
        Result := Envir;
        Exit;
      end;

      for I := Index - 1 downto 0 do
      begin
        Envir := Items[I];
        if not SameText(Envir.sMapName, sMapName) then
          Break;

        if Envir.nServerIndex = nServerIdx then
        begin
          Result := Envir;
          Exit;
        end;
      end;

      for I := Index + 1 to Count - 1 do
      begin
        Envir := Items[I];
        if not SameText(Envir.sMapName, sMapName) then
          Break;

        if Envir.nServerIndex = nServerIdx then
        begin
          Result := Envir;
          Exit;
        end;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TEnvirnoment.GetItem(nX, nY: Integer; AItemObject: TItemObject): TItemObject;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
begin
  Result := nil;
  if m_boInvalid then
    Exit;

{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(26);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
    begin
      bo2C := True;
      if (MapCellInfo.ObjList <> nil) then
      begin
        for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
        begin
          GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
          if GameObject <> nil then
          begin
            if (GameObject.m_ObjGame = Obj_Item) and (GameObject = AItemObject) then
            begin
              Result := TItemObject(GameObject);
              Exit;
            end;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetItem(nX, nY: Integer): TItemObject;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
  Result := nil;
  bo2C := false;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(27);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
    begin
      bo2C := True;
      if (MapCellInfo.ObjList <> nil) then
      begin
        for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
        begin
          GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
          if GameObject <> nil then
          begin
            if GameObject.m_ObjGame = Obj_Item then
            begin
              Result := TItemObject(GameObject);
              Exit;
            end;

            if GameObject.m_ObjGame = Obj_Gate then
              bo2C := false;

            if GameObject.m_ObjGame = Obj_Actor then
            begin
              BaseObject := TBaseObject(GameObject);
              if not BaseObject.m_boDeath then
                bo2C := false;
            end;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TMapManager.GetMapOfServerIndex(sMapName: string): Integer;
var
  I: Integer;
  Envir: TEnvirnoment;
begin
  Result := 0;
  Lock;
  try
    for I := 0 to Count - 1 do
    begin
      Envir := Items[I];
      if (CompareText(Envir.sMapName, sMapName) = 0) then
      begin
        Result := Envir.nServerIndex;
        Break;
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure TMapManager.LoadMapDoor;
var
  I: Integer;
begin
  for I := 0 to Count - 1 do
  begin
    TEnvirnoment(Items[I]).AddDoorToMap;
  end;
end;

procedure TMapManager.ProcessMapDoor;
begin
end;

procedure TMapManager.ReSetMinMap;
var
  I, II: Integer;
  Envirnoment: TEnvirnoment;
begin
  for I := 0 to Count - 1 do
  begin
    Envirnoment := TEnvirnoment(Items[I]);
    for II := 0 to MiniMapList.Count - 1 do
    begin
      if CompareText(MiniMapList.Strings[II], Envirnoment.sMapName) = 0 then
      begin
        Envirnoment.nMinMap := Integer(MiniMapList.Objects[II]);
        Break;
      end;
    end;
  end;
end;

function TEnvirnoment.IsCheapStuff: Boolean;
begin
  Result := m_QuestList.Count > 0
end;

function TEnvirnoment.AddToMapMineEvent(nX, nY: Integer; Event: TGameObject): TGameObject;
var
  MapCellInfo: pTMapCellinfo;
resourcestring
  sExceptionMsg = '[Exception] TEnvirnoment.AddToMapMineEvent ';
begin
  Result := nil;
  if m_boInvalid then
    Exit;

  try
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      LockW(28);
    try
{$IFEND}
      if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag <> 0) then
      begin
{$IF USEOBJLIST = 0}
        if MapCellInfo.ObjList = nil then
          MapCellInfo.ObjList := TSafeList.Create('TEnvirnoment.MapCellInfo.ObjList');
        MapCellInfo.ObjList.Add(Event);
{$ELSE}
        SetLength(MapCellInfo.ObjList, Length(MapCellInfo.ObjList) + 1);
        MapCellInfo.ObjList[Length(MapCellInfo.ObjList) - 1] := Event;
{$IFEND}
        Result := Event;
      end;
    except
      // MainOutMessage(Format('%s %S  %S nX:%d nY:%d m_nWidth:%d m_nHeight:%d',[sMapName,sMapDesc,booltostr(MapCellArray<>nil),nX, nY,m_nWidth,m_nHeight]));
      MainOutMessage(sExceptionMsg);
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockW;
  end;
{$IFEND}
end;

procedure TEnvirnoment.VerifyMapTime(nX, nY: Integer; BaseObject: TGameObject); // 校对时间
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  boVerify: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TEnvirnoment.VerifyMapTime';
begin
  try
    boVerify := false;
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      LockR(29);
    try
{$IFEND}
      if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo <> nil) and (MapCellInfo.ObjList <> nil) then
      begin
        for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
        begin
          GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
          if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) and (GameObject = BaseObject) then
          begin
            GameObject.m_dwAddTime := MyGetTickCount();
            boVerify := True;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        UnLockR;
    end;
{$IFEND}
    if not boVerify then
      AddToMap(nX, nY, BaseObject);
  except
    MainOutMessage(sExceptionMsg);
  end;
end;

constructor TEnvirnoment.Create;
var
  I: Integer;
begin
  inherited;
  Pointer(MapCellArray) := nil;
  sMapName := '';
  sMainMapName := '';
  m_boMainMap := false;
  nServerIndex := 0;
  nMinMap := 0;
  m_nWidth := 0;
  m_nHeight := 0;
  m_boDARK := false;
  m_boDAY := false;
  // 当前地图人物死亡多长时间后自动掉线 piaoyun 2013-09-05
  m_boDieTime := false;
  // 禁止丢物品 piaoyun 2013-09-05
  m_boNOTHROWITEM := false;
  // 当前地图魔血石、气血石、幻魔石无效 piaoyun 2013-09-05
  m_boNotStone := false;
  // 当前地图宝宝不攻击人物 chongchong 2013-12-11
  m_boSlaveNotAttackHuman := false;
  // 当前地图宝宝不攻击英雄 chongchong 2013-12-11
  m_boSlaveNotAttackHero := false;
  m_nSAYLEVEL := -1;
  m_boDELDROPITEM := false;
  m_boNODROPUSEITEMS := false;
  m_boNOSAFEPOSITIONMOVE := false;
  m_boNight := false;
  for I := Low(m_WeatherEffect) to High(m_WeatherEffect) do
  begin
    m_WeatherEffect[I].boIsUsed := false;
    m_WeatherEffect[I].boIsDark := false;
    m_WeatherEffect[I].dwTick := MyGetTickCount;
    m_WeatherEffect[I].dwTime := 0;
    m_WeatherEffect[I].sMusic := '';
  end;
  {
    m_boWeatherEffect1 := False;
    m_boWeatherEffect2 := False;
    m_boWeatherEffect3 := False;
    m_dwWeatherEffectTick1 := MyGetTickCount;
    m_dwWeatherEffectTick2 := MyGetTickCount;
    m_dwWeatherEffectTick3 := MyGetTickCount;
    m_dwWeatherEffectTime1 := 0;
    m_dwWeatherEffectTime2 := 0;
    m_dwWeatherEffectTime3 := 0; }
  m_boOnKillMob := false;
  m_nRevivalMaxCount := -1;
  m_nRevivalCheckTime := 30 * 1000;
  m_boHITMON := false;
  m_sHITMON := '';
  // 副本地图 ---- 相关定义 chongchong 2013-09-06
  m_boFB := false;
  m_sFBName := '';
  m_FBEnterLimit := fbel_OnlyCreater;
  m_dwFBEnterDelayMin := 0;
  m_dwFBNoHumClearMin := 10;
  m_boFBCreate := false;
  m_dwFBCreateTime := 0;
  m_boFBFail := false;
  m_dwFBFailTime := 0;
  m_FBFailType := fbft_JOB3;
  m_dwFBTime := 0;
  m_btFBIndex := 0;
  m_FBMasterObject := nil; // 副本地图创建人
  m_dwFBCheckMonsterTick := 0; // 副本地图怪物检测时间
  m_dwFBPlayObjectCount := 0; // 副本地图中的人物数量
  m_dwFBNOPlayObjectTick := MyGetTickCount;
  m_FBMonGenList := TList.Create; // 副本地图怪物创建表
  m_FBMonsterList := TList.Create; // 副本地图怪物列表
  m_boInitialize := false;
  FMonCount := 0;
  FHumCount := 0;
  FHumBBCount := 0;
  FClearHumOrBBTick := 0;
  m_boMirror := false;
  m_dwMirrorCreateTick := 0;
  m_dwMirrorSurvivalTime := 0;
  m_sMirrorExitToMap := '';
  m_MirrorExitMapPostion.X := 0;
  m_MirrorExitMapPostion.Y := 0;
  m_nMirrorMinMap := 0;
  m_boAlwaysShowTime := false;
  m_dwMirrorPlayObjectCount := 0;
  m_nSecretFlag2 := 0;
  m_sSecretShowName2 := '';
  m_wSecretDressShap2 := 0;
  m_wSecretWeaponShap2 := 0;
  m_DoorList := TList.Create;
  m_GateList := TList.Create;
  m_QuestList := TList.Create;
  m_UnAllowMagicList := nil;
  m_DropAddToUserBagItemsList := nil;
  m_UnAllowStdItemsList := nil;
  FSceneShakeList := TGList.Create;
  m_boMakeMon := True;
  m_boMakeMonPriority := false;
  m_dwNoManTick := 0;
  m_boClearMon := false;
  m_dwClearMonTick := MyGetTickCount;
  ResetGuardianLevel;
  FCriticalSection := TM2CriticalSection.Create('TEnvirnoment.FCriticalSection');
end;

destructor TEnvirnoment.Destroy;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  nX, nY: Integer;
  DoorObject: TDoorObject;
begin
  if (m_nWidth > 1) and (m_nHeight > 1) then
  begin
    if MapCellArray <> nil then
    begin
      for nX := 0 to m_nWidth - 1 do
      begin
        for nY := 0 to m_nHeight - 1 do
        begin
          if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
          begin
            { for I := 0 to MapCellInfo.ObjList.Count - 1 do begin
              GameObject := TGameObject(MapCellInfo.ObjList.Items[I]);
              case GameObject.m_ObjGame of
              // g_Item: TItemObject(GameObject).Free;
              // g_Event: TEvent(GameObject).Free;
              g_Gate: TGateObject(GameObject).Free;
              end;
              end; }
{$IF USEOBJLIST = 1}
            MapCellInfo.ObjList := nil;
{$ELSE}
            FreeAndNil(MapCellInfo.ObjList);
{$IFEND}
          end;
        end;
      end;
      FreeMem(Pointer(MapCellArray));
    end;
  end;

  Pointer(MapCellArray) := nil;
  for I := 0 to m_DoorList.Count - 1 do
  begin
    DoorObject := TDoorObject(m_DoorList.Items[I]);
    Dec(DoorObject.m_Status.nRefCount);
    if DoorObject.m_Status.nRefCount <= 0 then
      Dispose(DoorObject.m_Status);
    DoorObject.Free;
  end;
  m_DoorList.Free;

  for I := 0 to m_GateList.Count - 1 do
  begin
    TGateObject(m_GateList.Items[I]).Free;
  end;
  m_GateList.Free;

  for I := 0 to m_QuestList.Count - 1 do
  begin
    Dispose(pTMapQuestInfo(m_QuestList.Items[I]));
  end;
  m_QuestList.Free;

  if m_UnAllowStdItemsList <> nil then
  begin
    m_UnAllowStdItemsList.Free;
  end;

  if m_UnAllowMagicList <> nil then
  begin
    m_UnAllowMagicList.Free;
  end;

  if m_DropAddToUserBagItemsList <> nil then
  begin
    m_DropAddToUserBagItemsList.Free;
  end;

  for I := 0 to m_FBMonGenList.Count - 1 do
  begin
    Dispose(pTMonGenInfo(m_FBMonGenList[I]));
  end;
  m_FBMonGenList.Free; // 副本地图怪物创建表
  m_FBMonsterList.Free; // 副本地图怪物列表
  ClearSceneShakeList;
  FSceneShakeList.Free;
  FCriticalSection.Free;
  inherited;
end;

function TEnvirnoment.DoLoadMapData(sMapFile: string): Boolean;
var
  nHandle, Len: Integer;
  EIMapHeader: TEIMapHeader;
  IsEIMap: Boolean;
begin
  Result := false;
  if not FileExists(sMapFile) then
    Exit;

  nHandle := FileOpen(sMapFile, fmOpenRead or fmShareExclusive);
  if nHandle > 0 then
  begin
    IsEIMap := false;
    FileRead(nHandle, EIMapHeader, Sizeof(TEIMapHeader));
    if (EIMapHeader.Desc[0] = 0) and (EIMapHeader.Desc[1] = 0) and (EIMapHeader.Desc[2] = 0) and (EIMapHeader.Desc[3] = 0) and
      (EIMapHeader.Desc[4] = 0) then
    begin
      Len := FileSeek(nHandle, 0, soFromEnd);
      if Len = Sizeof(TEIMapHeader) + (EIMapHeader.Width * EIMapHeader.Height * Sizeof(TEIMapTileInfo) div 4) +
        (EIMapHeader.Width * EIMapHeader.Height * Sizeof(TEIMapInfo)) then
      begin
        IsEIMap := True;
      end;
    end;
    FileSeek(nHandle, 0, 0);

    if IsEIMap then
      Result := LoadEIMapData(nHandle)
    else
      Result := LoadMapData(nHandle);
    FileClose(nHandle);
  end;
end;

function TEnvirnoment.LoadMapData(nHandle: THandle): Boolean;
const
  XORWORD = $AA38;
  NEWMAPTITLE = 'Map 2010 Ver 1.0';
var
  Header: TMapHeader;
  nMapSize: Integer;
  n24, nW, nH: Integer;
  MapBuffer: pTMap;
  NewMapBuffer: pTNewMap;
  ReturnMapBuffer: pTReturnMap;
  ENMapBuffer: pTENMap;
  Point: Integer;
  DoorObject: TDoorObject;
  Status: pTDoorStatus;
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  boENMap: Boolean;
  ENMapHeader: TENMapHeader;
begin
  // FileRead(nHandle, Header, SizeOf(TMapHeader));
  FileRead(nHandle, ENMapHeader, Sizeof(TENMapHeader));
  boENMap := (ENMapHeader.Title = NEWMAPTITLE);
  if boENMap then
  begin
    Header.wWidth := ENMapHeader.Width xor XORWORD;
    Header.wHeight := ENMapHeader.Height xor XORWORD;
  end
  else
  begin
    Move(ENMapHeader, Header, Sizeof(Header));
    FileSeek(nHandle, Sizeof(Header), 0);
  end;
  // FileSeek(nHandle, SizeOf(TMapHeader), 0);
  m_nWidth := Header.wWidth;
  m_nHeight := Header.wHeight;
  Initialize(m_nWidth, m_nHeight);
  if boENMap then
  begin
    nMapSize := m_nWidth * m_nHeight * Sizeof(TENMapInfo);
    ENMapBuffer := AllocMem(nMapSize);
    FileRead(nHandle, ENMapBuffer^, nMapSize);
    for nW := 0 to m_nWidth - 1 do
    begin
      n24 := nW * m_nHeight;
      for nH := 0 to m_nHeight - 1 do
      begin
        ENMapBuffer[n24 + nH].BkImg := ENMapBuffer[n24 + nH].BkImg xor XORWORD;
        if (ENMapBuffer[n24 + nH].BkImgNot xor $AA38) = $2000 then
          ENMapBuffer[n24 + nH].BkImg := ENMapBuffer[n24 + nH].BkImg or $8000;

        ENMapBuffer[n24 + nH].MidImg := ENMapBuffer[n24 + nH].MidImg xor XORWORD;
        ENMapBuffer[n24 + nH].FrImg := ENMapBuffer[n24 + nH].FrImg xor XORWORD;
        if (ENMapBuffer[n24 + nH].BkImg) and $8000 <> 0 then
        begin
          MapCellInfo := @MapCellArray[n24 + nH];
          MapCellInfo.chFlag := 1;
        end;

        if ENMapBuffer[n24 + nH].FrImg and $8000 <> 0 then
        begin
          MapCellInfo := @MapCellArray[n24 + nH];
          MapCellInfo.chFlag := 2;
        end;

        if ENMapBuffer[n24 + nH].DoorIndex and $80 <> 0 then
        begin
          Point := (ENMapBuffer[n24 + nH].DoorIndex and $7F);
          if Point > 0 then
          begin
            DoorObject := TDoorObject.Create();
            DoorObject.m_n08 := Point;
            DoorObject.m_nMapX := nW;
            DoorObject.m_nMapY := nH;
            for I := 0 to m_DoorList.Count - 1 do
            begin
              if abs(TDoorObject(m_DoorList.Items[I]).m_nMapX - DoorObject.m_nMapX) <= 10 then
              begin
                if abs(TDoorObject(m_DoorList.Items[I]).m_nMapY - DoorObject.m_nMapY) <= 10 then
                begin
                  if TDoorObject(m_DoorList.Items[I]).m_n08 = Point then
                  begin
                    DoorObject.m_Status := TDoorObject(m_DoorList.Items[I]).m_Status;
                    Inc(DoorObject.m_Status.nRefCount);
                    Break;
                  end;
                end;
              end;
            end;

            if DoorObject.m_Status = nil then
            begin
              New(Status);
              Status.boOpened := false;
              Status.bo01 := false;
              Status.n04 := 0;
              Status.dwOpenTick := 0;
              Status.nRefCount := 1;
              DoorObject.m_Status := Status;
            end;
            m_DoorList.Add(DoorObject);
          end;
        end;
      end;
    end;
    FreeMem(ENMapBuffer);
  end
  else if (Header.sTitle[14] = #13) and (Header.sTitle[15] = #10) and (Header.btVersion = 6) then
  begin // 归来国际版
    nMapSize := m_nWidth * m_nHeight * Sizeof(TReturnMapInfo);
    ReturnMapBuffer := AllocMem(nMapSize);
    FileRead(nHandle, ReturnMapBuffer^, nMapSize);
    for nW := 0 to m_nWidth - 1 do
    begin
      n24 := nW * m_nHeight;
      for nH := 0 to m_nHeight - 1 do
      begin
        if (ReturnMapBuffer[n24 + nH].wBkImg) and $8000 <> 0 then
        begin
          MapCellInfo := @MapCellArray[n24 + nH];
          MapCellInfo.chFlag := 1;
        end;

        if ReturnMapBuffer[n24 + nH].wFrImg and $8000 <> 0 then
        begin
          MapCellInfo := @MapCellArray[n24 + nH];
          MapCellInfo.chFlag := 2;
        end;

        if ReturnMapBuffer[n24 + nH].btDoorIndex and $80 <> 0 then
        begin
          Point := (ReturnMapBuffer[n24 + nH].btDoorIndex and $7F);
          if Point > 0 then
          begin
            DoorObject := TDoorObject.Create();
            DoorObject.m_n08 := Point;
            DoorObject.m_nMapX := nW;
            DoorObject.m_nMapY := nH;
            for I := 0 to m_DoorList.Count - 1 do
            begin
              if abs(TDoorObject(m_DoorList.Items[I]).m_nMapX - DoorObject.m_nMapX) <= 10 then
              begin
                if abs(TDoorObject(m_DoorList.Items[I]).m_nMapY - DoorObject.m_nMapY) <= 10 then
                begin
                  if TDoorObject(m_DoorList.Items[I]).m_n08 = Point then
                  begin
                    DoorObject.m_Status := TDoorObject(m_DoorList.Items[I]).m_Status;
                    Inc(DoorObject.m_Status.nRefCount);
                    Break;
                  end;
                end;
              end;
            end;

            if DoorObject.m_Status = nil then
            begin
              New(Status);
              Status.boOpened := false;
              Status.bo01 := false;
              Status.n04 := 0;
              Status.dwOpenTick := 0;
              Status.nRefCount := 1;
              DoorObject.m_Status := Status;
            end;
            m_DoorList.Add(DoorObject);
          end;
        end;
      end;
    end;
    FreeMem(ReturnMapBuffer);
  end
  else if (Header.sTitle[14] = #13) and (Header.sTitle[15] = #10) then
  begin // 检测是新地图
    nMapSize := m_nWidth * m_nHeight * Sizeof(TNewMapUnitInfo);
    NewMapBuffer := AllocMem(nMapSize);
    FileRead(nHandle, NewMapBuffer^, nMapSize);
    for nW := 0 to m_nWidth - 1 do
    begin
      n24 := nW * m_nHeight;
      for nH := 0 to m_nHeight - 1 do
      begin
        if (NewMapBuffer[n24 + nH].wBkImg) and $8000 <> 0 then
        begin
          MapCellInfo := @MapCellArray[n24 + nH];
          MapCellInfo.chFlag := 1;
        end;

        if NewMapBuffer[n24 + nH].wFrImg and $8000 <> 0 then
        begin
          MapCellInfo := @MapCellArray[n24 + nH];
          MapCellInfo.chFlag := 2;
        end;

        if NewMapBuffer[n24 + nH].btDoorIndex and $80 <> 0 then
        begin
          Point := (NewMapBuffer[n24 + nH].btDoorIndex and $7F);
          if Point > 0 then
          begin
            DoorObject := TDoorObject.Create();
            DoorObject.m_n08 := Point;
            DoorObject.m_nMapX := nW;
            DoorObject.m_nMapY := nH;
            for I := 0 to m_DoorList.Count - 1 do
            begin
              if abs(TDoorObject(m_DoorList.Items[I]).m_nMapX - DoorObject.m_nMapX) <= 10 then
              begin
                if abs(TDoorObject(m_DoorList.Items[I]).m_nMapY - DoorObject.m_nMapY) <= 10 then
                begin
                  if TDoorObject(m_DoorList.Items[I]).m_n08 = Point then
                  begin
                    DoorObject.m_Status := TDoorObject(m_DoorList.Items[I]).m_Status;
                    Inc(DoorObject.m_Status.nRefCount);
                    Break;
                  end;
                end;
              end;
            end;

            if DoorObject.m_Status = nil then
            begin
              New(Status);
              Status.boOpened := false;
              Status.bo01 := false;
              Status.n04 := 0;
              Status.dwOpenTick := 0;
              Status.nRefCount := 1;
              DoorObject.m_Status := Status;
            end;
            m_DoorList.Add(DoorObject);
          end;
        end;
      end;
    end;
    FreeMem(NewMapBuffer);
  end
  else
  begin
    nMapSize := m_nWidth * m_nHeight * Sizeof(TMapUnitInfo);
    MapBuffer := AllocMem(nMapSize);
    FileRead(nHandle, MapBuffer^, nMapSize);
    for nW := 0 to m_nWidth - 1 do
    begin
      n24 := nW * m_nHeight;
      for nH := 0 to m_nHeight - 1 do
      begin
        if (MapBuffer[n24 + nH].wBkImg) and $8000 <> 0 then
        begin
          MapCellInfo := @MapCellArray[n24 + nH];
          MapCellInfo.chFlag := 1;
        end;

        if MapBuffer[n24 + nH].wFrImg and $8000 <> 0 then
        begin
          MapCellInfo := @MapCellArray[n24 + nH];
          MapCellInfo.chFlag := 2;
        end;

        if MapBuffer[n24 + nH].btDoorIndex and $80 <> 0 then
        begin
          Point := (MapBuffer[n24 + nH].btDoorIndex and $7F);
          if Point > 0 then
          begin
            DoorObject := TDoorObject.Create();
            DoorObject.m_n08 := Point;
            DoorObject.m_nMapX := nW;
            DoorObject.m_nMapY := nH;
            for I := 0 to m_DoorList.Count - 1 do
            begin
              if abs(TDoorObject(m_DoorList.Items[I]).m_nMapX - DoorObject.m_nMapX) <= 10 then
              begin
                if abs(TDoorObject(m_DoorList.Items[I]).m_nMapY - DoorObject.m_nMapY) <= 10 then
                begin
                  if TDoorObject(m_DoorList.Items[I]).m_n08 = Point then
                  begin
                    DoorObject.m_Status := TDoorObject(m_DoorList.Items[I]).m_Status;
                    Inc(DoorObject.m_Status.nRefCount);
                    Break;
                  end;
                end;
              end;
            end;

            if DoorObject.m_Status = nil then
            begin
              New(Status);
              Status.boOpened := false;
              Status.bo01 := false;
              Status.n04 := 0;
              Status.dwOpenTick := 0;
              Status.nRefCount := 1;
              DoorObject.m_Status := Status;
            end;
            m_DoorList.Add(DoorObject);
          end;
        end;
      end;
    end;
    // Dispose(MapBuffer);
    FreeMem(MapBuffer);
  end;
  Result := True;
end;

function TEnvirnoment.LoadEIMapData(nHandle: THandle): Boolean;
var
  EIHeader: TEIMapHeader;
  nMapSize: Integer;
  n24, nW, nH: Integer;
  EIMapBuffer: pTEIMap;
  Point: Integer;
  DoorObject: TDoorObject;
  Status: pTDoorStatus;
  I: Integer;
  MapCellInfo: pTMapCellinfo;
begin
  FileRead(nHandle, EIHeader, Sizeof(TEIMapHeader));
  m_nWidth := EIHeader.Width;
  m_nHeight := EIHeader.Height;
  if (m_nWidth > 1) and (m_nHeight > 1) then
  begin
    if MapCellArray <> nil then
    begin
      for nW := 0 to m_nWidth - 1 do
      begin
        for nH := 0 to m_nHeight - 1 do
        begin
          MapCellInfo := @MapCellArray[nW * m_nHeight + nH];
{$IF USEOBJLIST = 1}
          if MapCellInfo.ObjList <> nil then
          begin
            SetLength(MapCellInfo.ObjList, 0);
            MapCellInfo.ObjList := nil;
          end;
{$ELSE}
          if MapCellInfo.ObjList <> nil then
          begin
            FreeAndNil(MapCellInfo.ObjList);
          end;
{$IFEND}
        end;
      end;
      FreeMem(Pointer(MapCellArray));
      Pointer(MapCellArray) := nil;
    end;
    Pointer(MapCellArray) := AllocMem((m_nWidth * m_nHeight) * Sizeof(TMapCellinfo));
  end;
  nMapSize := m_nWidth * m_nHeight * Sizeof(TEIMapInfo);
  EIMapBuffer := AllocMem(nMapSize);
  FileSeek(nHandle, Sizeof(TEIMapHeader) + m_nWidth * m_nHeight div 4 * Sizeof(TEIMapTileInfo), 0);
  FileRead(nHandle, EIMapBuffer^, nMapSize);
  for nW := 0 to m_nWidth - 1 do
  begin
    n24 := nW * m_nHeight;
    for nH := 0 to m_nHeight - 1 do
    begin
      if (EIMapBuffer[n24 + nH].btFlag) and $1 = 0 then
      begin
        MapCellInfo := @MapCellArray[n24 + nH];
        MapCellInfo.chFlag := 1;
      end
      else
      begin
        MapCellInfo := @MapCellArray[n24 + nH];
        MapCellInfo.chFlag := 0;
      end; // 004B5601
      // 004B562C
      if EIMapBuffer[n24 + nH].btDoorIdx and $80 <> 0 then
      begin
        Point := (EIMapBuffer[n24 + nH].btDoorIdx and $7F);
        if Point > 0 then
        begin
          DoorObject := TDoorObject.Create();
          DoorObject.m_n08 := Point;
          DoorObject.m_nMapX := nW;
          DoorObject.m_nMapY := nH;
          for I := 0 to m_DoorList.Count - 1 do
          begin
            if abs(TDoorObject(m_DoorList.Items[I]).m_nMapX - DoorObject.m_nMapX) <= 10 then
            begin
              if abs(TDoorObject(m_DoorList.Items[I]).m_nMapY - DoorObject.m_nMapY) <= 10 then
              begin
                if TDoorObject(m_DoorList.Items[I]).m_n08 = Point then
                begin
                  DoorObject.m_Status := TDoorObject(m_DoorList.Items[I]).m_Status;
                  Inc(DoorObject.m_Status.nRefCount);
                  Break;
                end;
              end;
            end;
          end;

          if DoorObject.m_Status = nil then
          begin
            New(Status);
            Status.boOpened := false;
            Status.bo01 := false;
            Status.n04 := 0;
            Status.dwOpenTick := 0;
            Status.nRefCount := 1;
            DoorObject.m_Status := Status;
          end;
          m_DoorList.Add(DoorObject);
        end;
      end;
    end;
  end; // 004B5798
  FreeMem(EIMapBuffer);
  Result := True;
  (*
    if g_config.boMapFormatCheck then
    MainOutMessage(' Mir3 Map: '+ sMapFile + ' Loaded Successfully' );                                                   //004B57B1
    {--------------------------------Load hook point-------------------------------------}
    sFileName := g_Config.sEnvirDir + 'Point\' + MapName + '.txt';
    if FileExists(sFileName) then
    begin
    LoadList := TStringList.Create;
    try
    LoadList.LoadFromFile(sFileName);
    except
    end;
    //PointList := TList.Create;
    for I := 0 to LoadList.Count - 1 do
    begin
    sLineText := Trim(LoadList.Strings[I]);
    if (sLineText = '') or (sLineText[1] = ';') then Continue;
    sLineText := GetValidStr3(sLineText, sX, [',', #9]);
    sLineText := GetValidStr3(sLineText, sY, [',', #9]);
    nX := Str_ToInt(sX, -1);
    nY := Str_ToInt(sY, -1);
    if (nX >= 0) and (nY >= 0) and (nX < m_nWidth) and (nY < m_nHeight) then
    begin
    m_PointList.Add(Pointer(MakeLong(nX, nY)));
    end;
    end;
    LoadList.Free;
    end;
  *)
end;

procedure TEnvirnoment.Initialize(nWidth, nHeight: Integer);
var
  nW, nH: Integer;
  MapCellInfo: pTMapCellinfo;
begin
  m_boInitialize := false;
  if (nWidth > 1) and (nHeight > 1) then
  begin
    if Pointer(MapCellArray) <> nil then
    begin
      for nW := 0 to m_nWidth - 1 do
      begin
        for nH := 0 to m_nHeight - 1 do
        begin
          // MapCellInfo := @MapCellArray[nW * m_nHeight + nH];
          if GetMapCellInfo(nW, nH, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
          begin
{$IF USEOBJLIST = 1}
            SetLength(MapCellInfo.ObjList, 0);
            MapCellInfo.ObjList := nil;
{$ELSE}
            FreeAndNil(MapCellInfo.ObjList);
{$IFEND}
          end;
        end;
      end;
      FreeMem(Pointer(MapCellArray));
      Pointer(MapCellArray) := nil;
    end;
    m_nWidth := nWidth;
    m_nHeight := nHeight;
    Pointer(MapCellArray) := AllocMem((m_nWidth * m_nHeight) * Sizeof(TMapCellinfo));
    m_boInitialize := True;
  end;
end;

procedure TEnvirnoment.Run;
var
  boWeatherChanged: Boolean;
  I, J: Integer;
  Info: PSceneShakeInfo;
  Player: TPlayObject;
  CurTick: LongWord;
  TempList, TempList2: TList;
  IsAllShake: Boolean;
  boEnableClientOption: Boolean;
begin
  // 优化CPU占用 chongchong 2013-12-24
  if MyGetTickCount - m_LastRunTick <= 1000 then
    Exit;

  m_LastRunTick := MyGetTickCount;
  boWeatherChanged := false;
  { if m_boWeatherEffect1 and (MyGetTickCount - m_dwWeatherEffectTick1 > m_dwWeatherEffectTime1) then
    begin
    m_boWeatherEffect1 := False;
    boWeatherChanged := True;
    end;
    if m_boWeatherEffect2 and (MyGetTickCount - m_dwWeatherEffectTick2 > m_dwWeatherEffectTime2) then
    begin
    m_boWeatherEffect2 := False;
    boWeatherChanged := True;
    end;
    if m_boWeatherEffect3 and (MyGetTickCount - m_dwWeatherEffectTick3 > m_dwWeatherEffectTime3) then
    begin
    m_boWeatherEffect3 := False;
    boWeatherChanged := True;
    end; }
  for I := Low(m_WeatherEffect) to High(m_WeatherEffect) do
  begin
    if m_WeatherEffect[I].boIsUsed and (MyGetTickCount - m_WeatherEffect[I].dwTick > m_WeatherEffect[I].dwTime) then
    begin
      m_WeatherEffect[I].boIsUsed := false;
      boWeatherChanged := True;
    end;
  end;

  if boWeatherChanged then
    UserEngine.WeatherChanged(Self);

  // 地图振动 chongchong 2013-12-27
  TempList := TList.Create;
  TempList2 := TList.Create;
  boEnableClientOption := false;
  FSceneShakeList.Lock;
  CurTick := MyGetTickCount;
  IsAllShake := false;
  for I := FSceneShakeList.Count - 1 downto 0 do
  begin
    Info := FSceneShakeList.Items[I];
    if Info.CurCount >= Info.Count then
    begin
      Dispose(Info);
      FSceneShakeList.Delete(I);
      Continue;
    end;

    if CurTick - Info.LastTick < 320 then
      Continue;

    if Length(Info.PlayerName) <> 0 then
    begin
      Player := UserEngine.GetPlayObject(Info.PlayerName);
      if (Player = nil) or (Player.m_PEnvir <> Self) then
      begin
        Dispose(Info);
        FSceneShakeList.Delete(I);
        Continue;
      end
      else
      begin
        TempList.Add(Player);
        boEnableClientOption := Info.EnableClientOption;
      end;
    end
    else
    begin
      IsAllShake := True;
      boEnableClientOption := Info.EnableClientOption;
    end;
    Info.LastTick := CurTick;
    Inc(Info.CurCount);
  end;

  if IsAllShake then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      UserEngine.m_PlayObjectList.LockR(56);
    try
{$IFEND}
      for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
      begin
        Player := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);

        if (Player <> nil) //
          and (Player.m_PEnvir = Self) //
          and (not Player.m_boGhost) //
          and (not Player.m_boOffLine) //
          and (not Player.m_boDummyObject) then
          Player.SendSceneShake(1, boEnableClientOption);
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        UserEngine.m_PlayObjectList.UnLockR;
    end;
{$IFEND}
  end
  else if TempList.Count > 0 then
  begin
    for I := 0 to TempList.Count - 1 do
    begin
      Player := TempList.Items[I];
      TempList2.Clear;
      GetRangePlayObject(Player.m_nCurrX, Player.m_nCurrY, Player.m_nViewRange, True, TempList2);
      for J := 0 to TempList2.Count - 1 do
      begin
        Player := TempList2.Items[J];

        if (Player <> nil) //
          and (Player.m_PEnvir = Self) //
          and (not Player.m_boGhost) //
          and (not Player.m_boOffLine) //
          and (not Player.m_boDummyObject) then
          Player.SendSceneShake(1, boEnableClientOption);
      end;
    end;
  end;

  FSceneShakeList.UnLock;
  TempList.Free;
  TempList2.Free;
  PorcessGuardianLevelInfo;
end;

function TEnvirnoment.CreateQuest(nFlag, nValue: Integer; s24, s28, s2C: string; boGrouped: Boolean): Boolean;
var
  MapMerchant: TMerchant;
  MapQuest: pTMapQuestInfo;
begin
  Result := false;
  if nFlag < 0 then
    Exit;

  MapMerchant := TMerchant(UserEngine.FindNPC(s2C)); // 优化价值怪物死亡触发脚本过多创建NPC的问题 Cursor 2023-06-14 08:55:17
  if MapMerchant = nil then
  begin
    MapMerchant := TMerchant.Create;
    MapMerchant.m_sMapName := '0';
    MapMerchant.m_nCurrX := 0;
    MapMerchant.m_nCurrY := 0;
    MapMerchant.m_sCharName := s2C;
    MapMerchant.m_nFlag := 0;
    MapMerchant.m_wAppr := 0;
    MapMerchant.m_sFilePath := 'MapQuest_def\';
    MapMerchant.m_boIsHide := True;
    MapMerchant.m_boIsQuest := false;
    UserEngine.AddNpc(s2C, MapMerchant);
  end;

  New(MapQuest);
  MapQuest.nFlag := nFlag;
  if nValue > 1 then
    nValue := 1;

  MapQuest.nValue := nValue;
  if s24 = '*' then
    s24 := '';

  MapQuest.s08 := s24;
  if s28 = '*' then
    s28 := '';

  MapQuest.s0C := s28;
  if s2C = '*' then
    s2C := '';

  MapQuest.NPC := MapMerchant;
  MapQuest.bo10 := boGrouped;
  m_QuestList.Add(MapQuest);

  Result := True;
end;

function TEnvirnoment.GetXYObjCount(nX, nY: Integer): Integer;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
  boTempFixedHideMode: Boolean;
begin
  Result := 0;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(30);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          BaseObject := TBaseObject(GameObject);

          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
            boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
              (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
          else // 被人邀请骑马 chongchong 2013-10-15
            boTempFixedHideMode := false;

          if not BaseObject.m_boGhost //
            and BaseObject.bo2B9 //
            and not BaseObject.m_boDeath //
            and not boTempFixedHideMode //
            and not BaseObject.m_boObMode //
            and (not boTempFixedHideMode) then
            Inc(Result);
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetXYObjCount(AObject: TGameObject; nX, nY: Integer): Integer;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
  boTempFixedHideMode: Boolean;
begin
  Result := 0;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(31);
  try
{$IFEND}
    // TBaseObject(AObject).i
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          BaseObject := TBaseObject(GameObject);

          if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
            boTempFixedHideMode := (TSmartObject(BaseObject).m_dwChangeModeExTick[1] > 0) or
              (TSmartObject(BaseObject).m_boOnHorse and (not TSmartObject(BaseObject).m_boHorseMaster))
          else // 被人邀请骑马 chongchong 2013-10-15
            boTempFixedHideMode := false;

          if not BaseObject.m_boGhost //
            and BaseObject.bo2B9 //
            and not BaseObject.m_boDeath //
            and not boTempFixedHideMode //
            and not BaseObject.m_boObMode //
            and (not boTempFixedHideMode) //
            and TBaseObject(AObject).IsProperTarget(BaseObject) then
            Inc(Result);
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetXYNpcObjCount(nX, nY: Integer): Integer;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
  Result := 0;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(30);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          BaseObject := TBaseObject(GameObject);
          if (not BaseObject.m_boGhost) and (BaseObject.m_btRaceServer in [RC_MERCHANT, RC_NPC, RC_PEACENPC]) then
            Inc(Result);
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetNextPosition(sX, sY, nDir, nFlag: Integer; var snX: Integer; var snY: Integer): Boolean;
begin
  snX := sX;
  snY := sY;
  case nDir of
    DR_UP:
      if snY > nFlag - 1 then
        Dec(snY, nFlag);
    DR_DOWN:
      if snY < (m_nHeight - nFlag) then
        Inc(snY, nFlag);
    DR_LEFT:
      if snX > nFlag - 1 then
        Dec(snX, nFlag);
    DR_RIGHT:
      if snX < (m_nWidth - nFlag) then
        Inc(snX, nFlag);
    DR_UPLEFT:
      begin
        if (snX > nFlag - 1) and (snY > nFlag - 1) then
        begin
          Dec(snX, nFlag);
          Dec(snY, nFlag);
        end;
      end;
    DR_UPRIGHT:
      begin // 004B2B77
        if (snX > nFlag - 1) and (snY < (m_nHeight - nFlag)) then
        begin
          Inc(snX, nFlag);
          Dec(snY, nFlag);
        end;
      end;
    DR_DOWNLEFT:
      begin // 004B2BAC
        if (snX < (m_nWidth - nFlag)) and (snY > nFlag - 1) then
        begin
          Dec(snX, nFlag);
          Inc(snY, nFlag);
        end;
      end;
    DR_DOWNRIGHT:
      begin
        if (snX < (m_nWidth - nFlag)) and (snY < (m_nHeight - nFlag)) then
        begin
          Inc(snX, nFlag);
          Inc(snY, nFlag);
        end;
      end;
  end;

  if (snX = sX) and (snY = sY) then
    Result := false
  else
    Result := True;
end;

function TEnvirnoment.CanSafeWalk(nX, nY: Integer): Boolean;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
begin
  Result := True;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(32);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 downto 0 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Event) then
        begin
          if TGameEvent(GameObject).m_nDamage > 0 then
            Result := false;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.ArroundDoorOpened(nX, nY: Integer): Boolean;
var
  I: Integer;
  Door: pTDoorInfo;
resourcestring
  sExceptionMsg = '[Exception] TEnvirnoment.ArroundDoorOpened ';
begin
  Result := True;
  try
    for I := 0 to m_DoorList.Count - 1 do
    begin
      Door := m_DoorList.Items[I];
      if (abs(Door.nX - nX) <= 1) and ((abs(Door.nY - nY) <= 1)) then
      begin
        if not Door.Status.boOpened then
        begin
          Result := false;
          Break;
        end;
      end;
    end;
  except
    MainOutMessage(sExceptionMsg);
  end;
end;

function TEnvirnoment.GetMovingObject(nX, nY: Integer; boFlag: Boolean; BaseObjectList: TList): Integer;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
  Result := 0;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(33);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};

        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          BaseObject := TBaseObject(GameObject);

          if ((BaseObject <> nil) // -
            and (not BaseObject.m_boGhost) // -
            and (BaseObject.bo2B9)) //
            and ((not boFlag) or (not BaseObject.m_boDeath)) then
          begin
            if BaseObjectList <> nil then
              BaseObjectList.Add(BaseObject);

            Inc(Result);
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetMovingObject(nX, nY: Integer; boFlag: Boolean): Pointer;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
  Result := nil;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(34);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          BaseObject := TBaseObject(GameObject);

          if ((BaseObject <> nil) // -
            and (not BaseObject.m_boGhost) // -
            and (BaseObject.bo2B9)) // -
            and ((not boFlag) or (not BaseObject.m_boDeath)) then
          begin
            Result := BaseObject;
            Break;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetMovingObject(nX, nY: Integer; AObject: TObject; boFlag: Boolean): Pointer;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
  Result := nil;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(36);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          BaseObject := TBaseObject(GameObject);

          if ((BaseObject <> nil) // -
            and (BaseObject = AObject) // -
            and (not BaseObject.m_boGhost) // -
            and (BaseObject.bo2B9)) //
            and ((not boFlag) or (not BaseObject.m_boDeath)) then
          begin
            Result := BaseObject;
            Break;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetMovingObjectEx(BaseObject: TObject; nX, nY: Integer; boFlag: Boolean): Pointer;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  ABaseObject: TBaseObject;
begin
  Result := nil;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(37);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          ABaseObject := TBaseObject(GameObject);
          if ((ABaseObject <> nil) // -
            and (not ABaseObject.m_boGhost) // -
            and (ABaseObject.bo2B9)) // -
            and TBaseObject(BaseObject).IsProperTarget(ABaseObject) // --
            and ((not boFlag) or (not ABaseObject.m_boDeath)) then
          begin
            if (g_Config.nStartPermission < 10) //
              and (TBaseObject(ABaseObject).m_btRaceServer = RC_PLAYOBJECT) //
              and (TPlayObject(ABaseObject).m_btPermission >= 10) then
              Continue;

            Result := ABaseObject;
            Break;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetQuestNPC(BaseObject: TObject; sCharName, sStr: string; boFlag: Boolean): TObject; // 004B6E4C
var
  I: Integer;
  MapQuestFlag: pTMapQuestInfo;
  nFlagValue: Integer;
  tmpBool: Boolean;
  sValue: string;
  nValue: Integer;
  IsBreakParseVar: Boolean;
begin
  Result := nil;
  if TBaseObject(BaseObject).m_btRaceServer <> RC_PLAYOBJECT then
    Exit;

  for I := 0 to m_QuestList.Count - 1 do
  begin
    MapQuestFlag := m_QuestList.Items[I];
    nFlagValue := TPlayObject(BaseObject).GetQuestFlagStatus(MapQuestFlag.nFlag);

    if (nFlagValue = MapQuestFlag.nValue) //
      and ((boFlag = MapQuestFlag.bo10) or (not boFlag)) then
    begin
      if (MapQuestFlag.s08 <> '') then
        TMerchant(MapQuestFlag.NPC).GetVarValue(TPlayObject(BaseObject), MapQuestFlag.s08, sValue, nValue, IsBreakParseVar)
      else
        sValue := '';

      tmpBool := false;
      if (sValue <> '') and (MapQuestFlag.s0C <> '') and (sValue = sCharName) and (MapQuestFlag.s0C = sStr) then
        tmpBool := True;

      if (sValue <> '') and (MapQuestFlag.s0C = '') and (sValue = sCharName) and (sStr = '') then
        tmpBool := True;

      if (sValue = '') and (MapQuestFlag.s0C <> '') and (MapQuestFlag.s0C = sStr) then
        tmpBool := True;

      if tmpBool then
      begin
        Result := MapQuestFlag.NPC;
        Break;
      end;
    end;
  end;
end;

function TEnvirnoment.GetItemEx(nX, nY: Integer; var nCount: Integer): TItemObject;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
  Result := nil;
  nCount := 0;
  bo2C := false;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(38);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
    begin
      bo2C := True;
      if (MapCellInfo.ObjList <> nil) then
      begin
        for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
        begin
          GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
          if GameObject <> nil then
          begin
            if GameObject.m_ObjGame = Obj_Item then
            begin
              Result := TItemObject(GameObject);
              Inc(nCount);
            end;

            if GameObject.m_ObjGame = Obj_Gate then
              bo2C := false;

            if GameObject.m_ObjGame = Obj_Actor then
            begin
              BaseObject := TBaseObject(GameObject);
              if not BaseObject.m_boDeath then
                bo2C := false;
            end;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetItemEx2(nX, nY: Integer; var nCount: Integer): TItemObject;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
  Result := nil;
  nCount := 0;
  bo2C := false;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(39);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
    begin
      bo2C := True;
      if (MapCellInfo.ObjList <> nil) then
      begin
        for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
        begin
          GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
          if GameObject <> nil then
          begin
            if GameObject.m_ObjGame = Obj_Item then
            begin
              Result := TItemObject(GameObject);
              Inc(nCount);
            end;

            if GameObject.m_ObjGame = Obj_Gate then
              bo2C := false;

            if GameObject.m_ObjGame = Obj_Actor then
            begin
              BaseObject := TBaseObject(GameObject);
              // if ( (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER])) and (not BaseObject.m_boDeath) then
              if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (not BaseObject.m_boDeath) then
                bo2C := false;
            end;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetItemEx3(nX, nY: Integer; var nCount: Integer): TItemObject;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
begin
  Result := nil;
  nCount := 0;
  bo2C := false;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(40);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0) then
    begin
      bo2C := True;
      if (MapCellInfo.ObjList <> nil) then
      begin
        for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
        begin
          GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
          if GameObject <> nil then
          begin
            if GameObject.m_ObjGame = Obj_Item then
            begin
              Result := TItemObject(GameObject);
              Inc(nCount);
            end;

            if GameObject.m_ObjGame = Obj_Gate then
              bo2C := false;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetDoor(nX, nY: Integer): TDoorObject;
var
  I: Integer;
  DoorObject: TDoorObject;
begin
  Result := nil;
  for I := 0 to m_DoorList.Count - 1 do
  begin
    DoorObject := TDoorObject(m_DoorList.Items[I]);
    if (DoorObject.m_nMapX = nX) and (DoorObject.m_nMapY = nY) then
    begin
      Result := DoorObject;
      Exit;
    end;
  end;
end;

function TEnvirnoment.IsValidObject(nX, nY, nRage: Integer; BaseObject: TObject): Boolean;
var
  nXX, nYY, I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
begin
  Result := false;
  for nXX := nX - nRage to nX + nRage do
  begin
    for nYY := nY - nRage to nY + nRage do
    begin
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        LockR(41);
      try
{$IFEND}
        if GetMapCellInfo(nXX, nYY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
        begin
          for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
          begin
            GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
            if (GameObject <> nil) and (GameObject = BaseObject) then
            begin
              Result := True;
              Exit;
            end;
          end;
        end;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          UnLockR;
      end;
{$IFEND}
    end;
  end;
end;

function TEnvirnoment.IsValidObjectEx(nX, nY, nRage: Integer; BaseObject: TObject): Boolean;
var
  nXX, nYY, I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
begin
  Result := false;
  for nXX := nX - nRage to nX + nRage do
  begin
    for nYY := nY - nRage to nY + nRage do
    begin
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        LockR(42);
      try
{$IFEND}
        if GetMapCellInfo(nXX, nYY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
        begin
          for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
          begin
            GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
            if (GameObject <> nil) and (GameObject = BaseObject) and (not TBaseObject(BaseObject).m_boSkeleton) then
            begin
              Result := True;
              Exit;
            end;
          end;
        end;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          UnLockR;
      end;
{$IFEND}
    end;
  end;
end;

function TEnvirnoment.GetItemObjects(nX, nY: Integer; ItemObjectList: TList): Integer;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  ItemObject: TItemObject;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(43);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Item) then
        begin
          ItemObject := TItemObject(GameObject);
          if not ItemObject.m_boGhost then
            ItemObjectList.Add(ItemObject);
        end;
      end;
    end;
    Result := ItemObjectList.Count;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetRangeItemObject(nX, nY: Integer; nRage: Integer; ItemObjectList: TList): Integer;
var
  nXX, nYY: Integer;
begin
  for nXX := nX - nRage to nX + nRage do
  begin
    for nYY := nY - nRage to nY + nRage do
      GetItemObjects(nXX, nYY, ItemObjectList);
  end;
  Result := ItemObjectList.Count;
end;

function TEnvirnoment.GetRangeBaseObject(nX, nY, nRage: Integer; boFlag: Boolean; BaseObjectList: TList): Integer; // 004B59C0
var
  nXX, nYY: Integer;
begin
  for nXX := nX - nRage to nX + nRage do
  begin
    for nYY := nY - nRage to nY + nRage do
      GetBaseObjects(nXX, nYY, boFlag, BaseObjectList);
  end;
  Result := BaseObjectList.Count;
end;

function TEnvirnoment.GetRangePlayObject(nX, nY: Integer; nRage: Integer; boFlag: Boolean; BaseObjectList: TList): Integer;
var
  nXX, nYY: Integer;
begin
  for nXX := nX - nRage to nX + nRage do
  begin
    for nYY := nY - nRage to nY + nRage do
      GetPlayObjects(nXX, nYY, boFlag, BaseObjectList);
  end;
  Result := BaseObjectList.Count;
end;

// boFlag 是否包括死亡对象
// FALSE 包括死亡对象
// TRUE  不包括死亡对象
function TEnvirnoment.GetBaseObjects(nX, nY: Integer; IncDeathObject: Boolean; BaseObjectList: TList): Integer; // 004B58F8
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(44);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          BaseObject := TBaseObject(GameObject);
          if not BaseObject.m_boGhost and BaseObject.bo2B9 then
          begin
            if not IncDeathObject or not BaseObject.m_boDeath then
              BaseObjectList.Add(BaseObject);
          end;
        end;
      end;
    end;
    Result := BaseObjectList.Count;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetPlayObjects(nX, nY: Integer; IncDeathObject: Boolean; BaseObjectList: TList): Integer;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(45);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          BaseObject := TBaseObject(GameObject);
          if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
          begin
            if not BaseObject.m_boGhost and BaseObject.bo2B9 then
            begin
              if not IncDeathObject or not BaseObject.m_boDeath then
                BaseObjectList.Add(BaseObject);
            end;
          end;
        end;
      end;
    end;
    Result := BaseObjectList.Count;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.GetEvent(nX, nY: Integer): TObject;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
begin
  Result := nil;
  bo2C := false;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(46);
  try
{$IFEND}
    if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Event) then
          Result := GameObject;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

procedure TEnvirnoment.SetMapXYFlag(nX, nY: Integer; boFlag: Boolean);
var
  MapCellInfo: pTMapCellinfo;
begin
  if GetMapCellInfo(nX, nY, MapCellInfo) then
  begin
    if boFlag then
      MapCellInfo.chFlag := 0
    else
      MapCellInfo.chFlag := 2;
  end;
end;

function TEnvirnoment.CanFly(nSX, nSY, nDX, nDY: Integer): Boolean;
var
  r28, r30: real;
  n14, n18, n1C: Integer;
begin
  Result := True;
  if m_boInvalid then
    Exit;

  r28 := (nDX - nSX) / 10;
  // 修复弓箭怪有时候不打 chongchong 2014-06-26
  // r30 := (nDY - nDX) / 1.0E1;
  r30 := (nDY - nSY) / 10;
  n14 := 0;
  while (True) do
  begin
    n18 := Round(nSX + r28);
    n1C := Round(nSY + r30);
    if not CanWalk(n18, n1C, True) then
    begin
      Result := false;
      Break;
    end;

    Inc(n14);
    if n14 >= 10 then
      Break;
  end;
end;

function TEnvirnoment.GetXYHuman(nMapX, nMapY: Integer): Boolean;
var
  I: Integer;
  MapCellInfo: pTMapCellinfo;
  GameObject: TGameObject;
  BaseObject: TBaseObject;
begin
  Result := false;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    LockR(47);
  try
{$IFEND}
    if GetMapCellInfo(nMapX, nMapY, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
    begin
      for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
      begin
        GameObject := {$IF USEOBJLIST = 1}MapCellInfo.ObjList[I]{$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
        if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
        begin
          BaseObject := TBaseObject(GameObject);
          if BaseObject.m_btRaceServer = RC_PLAYOBJECT then
          begin
            Result := True;
            Break;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UnLockR;
  end;
{$IFEND}
end;

function TEnvirnoment.sub_4B5FC8(nX, nY: Integer): Boolean;
var
  MapCellInfo: pTMapCellinfo;
begin
  Result := True;
  if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 2) then
    Result := false;
end;

function TEnvirnoment.GetEnvirInfo: string;
var
  sMsg: string;
begin
  sMsg := '地图名:%s(%s) DAY:%s DARK:%s SAFE:%s FIGHT:%s FIGHT3:%s FIGHT4:%s QUIZ:%s NORECONNECT:%s(%s) MUSIC:%s(%s) EXPRATE:%s(%f) PKWINLEVEL:%s(%d) PKLOSTLEVEL:%s(%d) PKWINEXP:%s(%d) PKLOSTEXP:%s(%d) DECHP:%s(%d/%d) INCHP:%s(%d/%d)';
  sMsg := sMsg +
    ' DECGAMEGOLD:%s(%d/%d) INCGAMEGOLD:%s(%d/%d) INCGAMEPOINT:%s(%d/%d) RUNHUMAN:%s RUNMON:%s NEEDHOLE:%s NORECALL:%s NOGUILDRECALL:%s NODEARRECALL:%s NOMASTERRECALL:%s NODRUG:%s MINE:%s NOPOSITIONMOVE:%s';
  Result := Format(sMsg, [sMapName, sMapDesc, BoolToCStr(m_boDAY), BoolToCStr(m_boDARK), BoolToCStr(m_boSAFE),
    BoolToCStr(m_boFightZone), BoolToCStr(m_boFight3Zone), BoolToCStr(m_boFight4Zone), BoolToCStr(m_boQUIZ),
    BoolToCStr(m_boNORECONNECT), sNoReconnectMap, BoolToCStr(m_boMUSIC), m_sMusicFileName, BoolToCStr(m_boEXPRATE),
    m_nEXPRATE / 100, BoolToCStr(m_boPKWINLEVEL), m_nPKWINLEVEL, BoolToCStr(m_boPKLOSTLEVEL), m_nPKLOSTLEVEL,
    BoolToCStr(m_boPKWINEXP), m_nPKWINEXP, BoolToCStr(m_boPKLOSTEXP), m_nPKLOSTEXP, BoolToCStr(m_boDECHP), m_nDECHPTIME,
    m_nDECHPPOINT, BoolToCStr(m_boINCHP), m_nINCHPTIME, m_nINCHPPOINT, BoolToCStr(m_boDecGameGold), m_nDECGAMEGOLDTIME,
    m_nDecGameGold, BoolToCStr(m_boIncGameGold), m_nINCGAMEGOLDTIME, m_nIncGameGold, BoolToCStr(m_boINCGAMEPOINT),
    m_nINCGAMEPOINTTIME, m_nINCGAMEPOINT, BoolToCStr(m_boRUNHUMAN), BoolToCStr(m_boRUNMON), BoolToCStr(m_boNEEDHOLE),
    BoolToCStr(m_boNORECALL), BoolToCStr(m_boNOGUILDRECALL), BoolToCStr(m_boNODEARRECALL), BoolToCStr(m_boNOMASTERRECALL),
    BoolToCStr(m_boNODRUG), BoolToCStr(m_boMINE), BoolToCStr(m_boNOPOSITIONMOVE)]);
end;

procedure TMapManager.Run;
var
  I: Integer;
  GateObject: TGateObject;
  Envir: TEnvirnoment;
  Map: TEnvirnoment;
  nIdx: Integer;
  dwCheckTime: LongWord;
  boCheckTimeLimit: Boolean;
  // dwTickDiff: LongWord;
begin
  boCheckTimeLimit := false;
  dwCheckTime := MyGetTickCount();
  nIdx := m_nProcMapIDx;
  if nIdx >= Count then
    nIdx := 0;
  if nIdx < 0 then
    nIdx := 0;

  Lock;
  try
    for I := Count - 1 downto 0 do
    begin
      Map := TEnvirnoment(Items[I]);
      if Map.m_boMirror and (Map.m_dwMirrorCreateTick <> 0) then
      begin
        // 镜像地图时间到了
        // dwTickDiff := tick_diff(Map.m_dwMirrorCreateTick, MyGetTickCount);
        // if dwTickDiff >= Map.m_dwMirrorSurvivalTime * 1000 then
        // begin
        // end;
      end;

      Map.Run;

      if MyGetTickCount - dwCheckTime > 5 then
      begin
        boCheckTimeLimit := True;
        m_nProcMapIDx := nIdx;
        Break;
      end;
    end;
  finally
    if not boCheckTimeLimit then
      m_nProcMapIDx := 0;
    UnLock;
  end;

  boCheckTimeLimit := false;
  dwCheckTime := MyGetTickCount();
  nIdx := m_nProcGateIDx;

  while True do
  begin
    if m_GateList.Count <= nIdx then
      Break;

    GateObject := TGateObject(m_GateList.Items[nIdx]);
    if (GateObject.m_dwRunTime > 0) and (MyGetTickCount > GateObject.m_dwRunTick) then
    begin
      Envir := FindMap(GateObject.m_sSMapNO);
      if (Envir <> nil) and Envir.DeleteFromMap(GateObject.m_nSMapX, GateObject.m_nSMapY, GateObject) then
      begin
        m_GateList.Delete(nIdx);
        GateObject.Free;
        Continue;
      end;
    end;

    Inc(nIdx);
    if MyGetTickCount - dwCheckTime > 5 then
    begin
      boCheckTimeLimit := True;
      m_nProcGateIDx := nIdx;
      Break;
    end;
  end;

  if not boCheckTimeLimit then
    m_nProcGateIDx := 0;
end;

function TEnvirnoment.GetRangeXY(nX, nY, nRang: Integer; var vX: Integer; var vY: Integer): Boolean;
var
  nSX, nSY, nEX, nEY, I: Integer;
begin
  Result := false;
  try
    nSX := nX - nRang;
    nSY := nY - nRang;
    nEX := nX + nRang;
    nEY := nY + nRang;
    I := 0;
    while True do
    begin
      Inc(I);
      vX := Random(nEX - nSX) + nSX;
      vY := Random(nEY - nSY) + nSY;
      Result := CanWalk(vX, vY, True);
      if Result or (I > 10) then
        Break;
    end;
  except
    MainOutMessage('[Exception] TEnvirnoment:GetRangeXY');
  end;
end;

function TEnvirnoment.GetSitInLinPosition(sX, sY, nDir, nSpan: Integer; out snX, snY: Integer): Boolean;
begin
  snX := sX;
  snY := sY;
  Result := false;

  case nDir of
    DR_UP, DR_DOWN:
      Inc(snX, nSpan);
    DR_LEFT, DR_RIGHT:
      Inc(snY, nSpan);
    DR_UPLEFT, DR_DOWNRIGHT:
      begin
        Inc(snX, nSpan);
        Dec(snY, nSpan);
      end;
    DR_UPRIGHT, DR_DOWNLEFT:
      begin
        Inc(snX, nSpan);
        Inc(snY, nSpan);
      end;
  else
    Exit;
  end;

  Result := (snX >= 0) and (snX < m_nWidth) and (snY >= 0) or (snY < m_nHeight);
end;

procedure TEnvirnoment.ClearSceneShakeList;
var
  I: Integer;
  Info: PSceneShakeInfo;
begin
  FSceneShakeList.Lock;
  for I := 0 to FSceneShakeList.Count - 1 do
  begin
    Info := FSceneShakeList.Items[I];
    Dispose(Info);
  end;

  FSceneShakeList.Clear;
  FSceneShakeList.UnLock;
end;

function TEnvirnoment.AddSceneShake(ShakeCount: Integer; PlayerName: string; EnableClientOption: Boolean): PSceneShakeInfo;
begin
  if ShakeCount = 0 then
  begin
    Result := nil;
    Exit;
  end;

  New(Result);
  Result.LastTick := 0;
  Result.Count := ShakeCount;
  Result.CurCount := 0;
  Result.PlayerName := PlayerName;
  Result.EnableClientOption := EnableClientOption;
  FSceneShakeList.Lock;
  FSceneShakeList.Add(Result);
  FSceneShakeList.UnLock;
end;

procedure TEnvirnoment.LockR(LockID: Integer);
begin
  FCriticalSection.LockR(LockID);
end;

procedure TEnvirnoment.UnLockR;
begin
  FCriticalSection.UnLockR;
end;

procedure TEnvirnoment.LockW(LockID: Integer);
begin
  FCriticalSection.LockW(LockID);
end;

procedure TEnvirnoment.UnLockW;
begin
  FCriticalSection.UnLockW;
end;

procedure TEnvirnoment.Invalidity;
begin
  m_boInvalid := True;
  m_dwInvalidTick := MyGetTickCount;
end;

procedure TEnvirnoment.PorcessGuardianLevelInfo;
const
  MON_RANGE = 3;
var
  I: Integer;
  S2, sMonName, sMonCount: string;
  sSendMsg: AnsiString;
  nMonCount: Integer;
  nRandX, nRandY: Integer;
  Mon: TBaseObject;
  GuardianLevelItemCounts: TGuardianLevelItemCounts;
begin
  if m_boGuardianLevel and m_boStartGuardianLevel then
  begin
    if (m_nGuardianLevelBatchNo <= m_nGuardianLevelBatchCount) and (m_GuardianLevelPlayer <> nil) and
      (m_GuardinaLevelStatue <> nil) then
    begin
      if TBaseObject(m_GuardianLevelPlayer).m_boGhost or TBaseObject(m_GuardianLevelPlayer).m_boDeath then
      begin
        ResetGuardianLevel;
        Exit;
      end;

      if (TBaseObject(m_GuardinaLevelStatue).m_boGhost) or (TBaseObject(m_GuardinaLevelStatue).m_boDeath) and
        (not m_boGuardianLevelSucces) then
      begin
        if g_FunctionNPC <> nil then
          g_FunctionNPC.GotoLable(TPlayObject(m_GuardianLevelPlayer), '@GuardinaLevelFail', false);

        // 闯关失败
        if (m_nGuardinaLevelHasItems[0] > 0) or (m_nGuardinaLevelHasItems[1] > 0) or (m_nGuardinaLevelHasItems[2] > 0) or
          (m_nGuardinaLevelHasItems[3] > 0) then
        begin
          TBaseObject(m_GuardianLevelPlayer).SendMsg(TBaseObject(m_GuardianLevelPlayer), RM_GuardianLevelResult, 0, 0, 0, 0, '');
          TPlayObject(m_GuardianLevelPlayer).m_boGuardinaLevelCanGetItem := True;
          TPlayObject(m_GuardianLevelPlayer).m_sGuardinaLevelGetItem[0] := m_sGuardinaLevelGetItem[0];
          TPlayObject(m_GuardianLevelPlayer).m_sGuardinaLevelGetItem[1] := m_sGuardinaLevelGetItem[1];
          TPlayObject(m_GuardianLevelPlayer).m_sGuardinaLevelGetItem[2] := m_sGuardinaLevelGetItem[2];
          TPlayObject(m_GuardianLevelPlayer).m_sGuardinaLevelGetItem[3] := m_sGuardinaLevelGetItem[3];
          Move(m_nGuardinaLevelHasItems, TPlayObject(m_GuardianLevelPlayer).m_nGuardinaLevelHasItems,
            Sizeof(m_nGuardinaLevelHasItems));
        end;
        m_boStartGuardianLevel := false;
        Exit;
      end;

      if m_nGuardinaLevelMonCount = 0 then
      begin
        if m_nGuardianLevelBatchNo < m_nGuardianLevelBatchCount then
        begin
          if m_nGuardianLevelBatchNo >= 1 then
          begin
            m_nGuardinaLevelHasItems[0] := m_nGuardinaLevelHasItems[0] + m_nGuardinaLevelButchItems
              [m_nGuardianLevelBatchNo - 1, 0];
            m_nGuardinaLevelHasItems[1] := m_nGuardinaLevelHasItems[1] + m_nGuardinaLevelButchItems
              [m_nGuardianLevelBatchNo - 1, 1];
            m_nGuardinaLevelHasItems[2] := m_nGuardinaLevelHasItems[2] + m_nGuardinaLevelButchItems
              [m_nGuardianLevelBatchNo - 1, 2];
            m_nGuardinaLevelHasItems[3] := m_nGuardinaLevelHasItems[3] + m_nGuardinaLevelButchItems
              [m_nGuardianLevelBatchNo - 1, 3];
          end;
          Inc(m_nGuardianLevelBatchNo);
          GuardianLevelItemCounts.GetItemCounts[0] := m_nGuardinaLevelHasItems[0];
          GuardianLevelItemCounts.GetItemCounts[1] := m_nGuardinaLevelHasItems[1];
          GuardianLevelItemCounts.GetItemCounts[2] := m_nGuardinaLevelHasItems[2];
          GuardianLevelItemCounts.GetItemCounts[3] := m_nGuardinaLevelHasItems[3];
          GuardianLevelItemCounts.CurItemCounts[0] := m_nGuardinaLevelButchItems[m_nGuardianLevelBatchNo - 1, 0];
          GuardianLevelItemCounts.CurItemCounts[1] := m_nGuardinaLevelButchItems[m_nGuardianLevelBatchNo - 1, 1];
          GuardianLevelItemCounts.CurItemCounts[2] := m_nGuardinaLevelButchItems[m_nGuardianLevelBatchNo - 1, 2];
          GuardianLevelItemCounts.CurItemCounts[3] := m_nGuardinaLevelButchItems[m_nGuardianLevelBatchNo - 1, 3];
          SetLength(sSendMsg, Sizeof(GuardianLevelItemCounts));
          Move(GuardianLevelItemCounts, sSendMsg[1], Length(sSendMsg));
          TBaseObject(m_GuardianLevelPlayer).SendMsg(TBaseObject(m_GuardianLevelPlayer), RM_GuardianLevelBatchInfo,
            m_nGuardianLevelBatchNo, Length(sSendMsg), 0, 0, sSendMsg);
          sSendMsg := m_sGuardinaLevelMons[m_nGuardianLevelBatchNo - 1];
          while True do
          begin
            if Length(sSendMsg) = 0 then
              Break;
            sSendMsg := GetValidStr3_Ex(sSendMsg, S2, '|');
            if S2 = '' then
              Break;
            I := Pos(':', S2);
            if I > 0 then
            begin
              sMonName := Copy(S2, 1, I - 1);
              sMonCount := Copy(S2, I + 1, MAXINT);
              nMonCount := StrToIntDef(sMonCount, 1);
            end
            else
            begin
              sMonName := S2;
              nMonCount := 1;
            end;

            for I := 0 to nMonCount - 1 do
            begin
              nRandX := Random(MON_RANGE * 2 + 1) + (m_nGuardinaLevelMonGenX - MON_RANGE);
              nRandY := Random(MON_RANGE * 2 + 1) + (m_nGuardinaLevelMonGenY - MON_RANGE);
              Mon := UserEngine.RegenMonsterByName(sMapName, nRandX, nRandY, sMonName);
              if Mon <> nil then
              begin
                Mon.m_sAttackTargetName := TBaseObject(m_GuardinaLevelStatue).m_sCharName;
                Inc(m_nGuardinaLevelMonCount);
                Mon.m_boMISSION := True;
                Mon.m_nMissionPointIndex := -1;
                SetLength(Mon.m_nMissionPoints, 1);
                Mon.m_nMissionPoints[0].X := TBaseObject(m_GuardinaLevelStatue).m_nCurrX;
                Mon.m_nMissionPoints[0].Y := TBaseObject(m_GuardinaLevelStatue).m_nCurrY;
              end;
            end;
          end;
        end
        else
        begin
          TBaseObject(m_GuardinaLevelStatue).m_boDeath := True;
          TPlayObject(m_GuardianLevelPlayer).m_SlaveList.Remove(m_GuardinaLevelStatue);
          TBaseObject(m_GuardinaLevelStatue).m_Master := nil;
          if m_nGuardianLevelBatchNo >= 1 then
          begin
            m_nGuardinaLevelHasItems[0] := m_nGuardinaLevelHasItems[0] + m_nGuardinaLevelButchItems
              [m_nGuardianLevelBatchNo - 1, 0];
            m_nGuardinaLevelHasItems[1] := m_nGuardinaLevelHasItems[1] + m_nGuardinaLevelButchItems
              [m_nGuardianLevelBatchNo - 1, 1];
            m_nGuardinaLevelHasItems[2] := m_nGuardinaLevelHasItems[2] + m_nGuardinaLevelButchItems
              [m_nGuardianLevelBatchNo - 1, 2];
            m_nGuardinaLevelHasItems[3] := m_nGuardinaLevelHasItems[3] + m_nGuardinaLevelButchItems
              [m_nGuardianLevelBatchNo - 1, 3];
          end;

          GuardianLevelItemCounts.GetItemCounts[0] := m_nGuardinaLevelHasItems[0];
          GuardianLevelItemCounts.GetItemCounts[1] := m_nGuardinaLevelHasItems[1];
          GuardianLevelItemCounts.GetItemCounts[2] := m_nGuardinaLevelHasItems[2];
          GuardianLevelItemCounts.GetItemCounts[3] := m_nGuardinaLevelHasItems[3];
          GuardianLevelItemCounts.CurItemCounts[0] := m_nGuardinaLevelButchItems[m_nGuardianLevelBatchNo - 1, 0];
          GuardianLevelItemCounts.CurItemCounts[1] := m_nGuardinaLevelButchItems[m_nGuardianLevelBatchNo - 1, 1];
          GuardianLevelItemCounts.CurItemCounts[2] := m_nGuardinaLevelButchItems[m_nGuardianLevelBatchNo - 1, 2];
          GuardianLevelItemCounts.CurItemCounts[3] := m_nGuardinaLevelButchItems[m_nGuardianLevelBatchNo - 1, 3];
          SetLength(sSendMsg, Sizeof(GuardianLevelItemCounts));
          Move(GuardianLevelItemCounts, sSendMsg[1], Length(sSendMsg));
          TBaseObject(m_GuardianLevelPlayer).SendMsg(TBaseObject(m_GuardianLevelPlayer), RM_GuardianLevelBatchInfo,
            m_nGuardianLevelBatchNo, Length(sSendMsg), 0, 0, sSendMsg);

          m_boGuardianLevelSucces := True;

          TPlayObject(m_GuardianLevelPlayer).m_boGuardinaLevelCanGetItem := True;
          TPlayObject(m_GuardianLevelPlayer).m_sGuardinaLevelGetItem[0] := m_sGuardinaLevelGetItem[0];
          TPlayObject(m_GuardianLevelPlayer).m_sGuardinaLevelGetItem[1] := m_sGuardinaLevelGetItem[1];
          TPlayObject(m_GuardianLevelPlayer).m_sGuardinaLevelGetItem[2] := m_sGuardinaLevelGetItem[2];
          TPlayObject(m_GuardianLevelPlayer).m_sGuardinaLevelGetItem[3] := m_sGuardinaLevelGetItem[3];
          Move(m_nGuardinaLevelHasItems, TPlayObject(m_GuardianLevelPlayer).m_nGuardinaLevelHasItems,
            Sizeof(m_nGuardinaLevelHasItems));

          if g_FunctionNPC <> nil then
            g_FunctionNPC.GotoLable(TPlayObject(m_GuardianLevelPlayer), '@GuardinaLevelSuccess', false);

          // 闯关成功
          TBaseObject(m_GuardianLevelPlayer).SendMsg(TBaseObject(m_GuardianLevelPlayer), RM_GuardianLevelResult, 1, 0, 0, 0, '');

          m_boStartGuardianLevel := false;
        end;
      end;
    end;
  end;
end;

procedure TEnvirnoment.ResetGuardianLevel;
var
  I: Integer;
begin
  m_boGuardianLevel := false; // 是否为守关地图
  m_boStartGuardianLevel := false; // 是否开始闯关
  m_boGuardianLevelGetItem := false;
  m_boGuardianLevelSucces := false;
  m_nGuardianLevelNo := 0; // 关数
  m_nGuardianLevelBatchCount := 0; // 总波数
  m_nGuardianLevelBatchNo := 0; // 波数
  m_GuardianLevelPlayer := nil;; // 闯关玩家
  m_GuardinaLevelStatue := nil; // 守关雕像

  m_nGuardinaLevelMonCount := 0; // 怪物数量
  m_nGuardinaLevelMonGenX := 0; // 刷怪X
  m_nGuardinaLevelMonGenY := 0; // 刷怪Y
  m_sGuardinaLevelGetItem[0] := ''; // 奖励物品
  m_sGuardinaLevelGetItem[1] := '';
  m_sGuardinaLevelGetItem[2] := '';
  m_sGuardinaLevelGetItem[3] := '';

  FillChar(m_nGuardinaLevelButchItems, Sizeof(m_nGuardinaLevelButchItems), 0); // 每波奖励物品数量
  FillChar(m_nGuardinaLevelHasItems, Sizeof(m_nGuardinaLevelHasItems), 0); // 已得到奖励物品数量

  for I := Low(m_sGuardinaLevelMons) to High(m_sGuardinaLevelMons) do // 每波怪物及数量
    m_sGuardinaLevelMons[I] := '';
end;

end.

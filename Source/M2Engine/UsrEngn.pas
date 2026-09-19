unit UsrEngn;

interface

uses
  Windows, Classes, SysUtils, StrUtils, Forms, EDcode, ObjBase, ObjNpc, ObjHero, Envir, Grobal2, ObjGame, SDK, DateUtils,
  ObjDummy, ObjPlayer, uMagicACUtils, M2Locker,
{$IFDEF CPUX64}
  // VMProtectSDK,
{$ENDIF}
  M2Threads, GameGoldDealDB, SafeAreaManager, M2Definition, SellPlayer, System.AnsiStrings, System.Types;

type
  TOffLineData = record
    sAccount: string;
    sCharName: string;
    boStartLogin: Boolean;
    dwStartLoginTick: LongWord;
    boPassWordSuccess: Boolean;
    SessInfo: pTSessInfo;
  end;

  pTOffLineData = ^TOffLineData;

  TMapMonGenCount = record
    sMapName: string[14]; // 地图名称
    nMonGenCount: Integer; // 刷怪数量
    dwNotHumTimeTick: LongWord; // 没玩家的间隔
    nClearCount: Integer; // 清除数量
    boNotHum: Boolean; // 是否有玩家
    dwMakeMonGenTimeTick: LongWord; // 刷怪的间隔
    nMonGenRate: Integer; // 刷怪倍数  10
    dwRegenMonstersTime: LongWord; // 刷怪速度    200
  end;

  pTMapMonGenCount = ^TMapMonGenCount;

  TMonGenInfo = record
    sMapName: string[MAP_NAME_LEN];
    nRace: Integer;
    nRange: Integer;
    nMissionGenRate: Integer;
    dwStartTick: LongWord;
    nX: Integer;
    nY: Integer;
    sMonName: string[ITEM_NAME_LEN];
    nAreaX: Integer;
    nAreaY: Integer;
    nCount: Integer;
    dwZenTime: LongWord;
    dwStartTime: LongWord;
    boIsNGMon: Boolean;
    btNameColor: Byte;
    sNationaID: string; // 国家ID
    boCanAttackSameNationPlayer: Boolean; // 是否可以攻击同国家的玩家
    boNoSameNationMonPK: Boolean; // 不同国家的怪物可PK
    boAllowSameNationPlayerAttack: Boolean; // 能否被同国家的人攻击
    CertList: TList;
    Envir: TEnvirnoment;
    GVarCompareType: TCompareType;
    GVarIndex: Integer;
    GVarValue: Integer;
    boFB: Boolean; // 副本地图 ---- 是否为副本地图中的怪 chongchong 2013-09-06
    boNoManNoMon: Boolean;
    TriggerScript: string;
  end;

  pTMonGenInfo = ^TMonGenInfo;
  { TStringItems = array[0..9] of TStringItem;
    PTStringItemList = ^TStringItems;
    TPlayObjectList = class(TObject)
    private
    FCount: Integer;
    FList: PTStringItemList; // array[0..99] of TStringItem;
    function Get(Index: Integer): string;
    function GetCount: Integer;
    function GetMaxCount: Integer;
    function GetObject(Index: Integer): TObject;
    procedure Put(Index: Integer; const S: string);
    procedure PutObject(Index: Integer; AObject: TObject);
    procedure InsertItem(Index: Integer; const S: string; AObject: TObject);
    public
    constructor Create();
    destructor Destroy; override;
    function Add(const S: string): Integer;
    function AddObject(const S: string; AObject: TObject): Integer;
    procedure Clear;
    procedure Delete(Index: Integer);
    function IndexOf(const S: string): Integer;
    procedure Insert(Index: Integer; const S: string);
    procedure InsertObject(Index: Integer; const S: string;
    AObject: TObject);
    property Count: Integer read GetCount;
    property MaxCount: Integer read GetMaxCount;
    property Objects[Index: Integer]: TObject read GetObject write PutObject;
    property Strings[Index: Integer]: string read Get write Put; default;
    end; }
  PPriorityMonGenRecord = ^TPriorityMonGenRecord;

  TPriorityMonGenRecord = record
    Index: Integer;
    Count: Integer;
  end;

  PStdItemEx = ^TStdItemEx;

  TStdItemEx = record
    ItemIdx: Integer;
    StdItem: pTStdItem;
  end;

  TUserEngine = class
  private
    m_LoadOfflineList: TSafeStringList;
    m_PlayObjectFreeList: TSafeList; // 0x10
    m_DummyLogonList: TSafeStringList; // 假人登录
    m_dwDummyLogonTick: LongWord; //
    m_HeroObjectFreeList: TSafeList;
    dwShowOnlineTick: LongWord; // 0x18
    dwSendOnlineHumTime: LongWord; // 0x1C
    dwRegenMonstersTick: LongWord; // 0x28
    m_dwProcessLoadPlayTick: LongWord; // 0x30
    m_nCurrMonGen: Integer; // 0x38
    m_nMonGenListPosition: Integer; // 0x3C
    m_nMonGenCertListPosition: Integer; // 0x40
    m_nProcHumIDx: Integer; // 0x44 处理人物开始索引（每次处理人物数限制）
    nProcessHumanLoopTime: Integer;
    nMerchantPosition: Integer; // 0x4C
    nNpcPosition: Integer; // 0x50
    m_MapMonGenCountList: TSafeList;
    nMonsterCount: Integer; // 怪物总数
    nMonsterProcessPostion: Integer; // 0x80处理怪物总数位置，用于计算怪物总数
    nMonsterProcessCount: Integer; // 0x88处理怪物数，用于统计处理怪物个数
    boItemEvent: Boolean; // ItemEvent
    nHeroPosition: Integer;
    dwProcessHeroTimeMin: Integer;
    dwProcessHeroTimeMax: Integer;
    m_NewHumanList: TSafeList;
    m_ListOfGateIdx: TSafeList;
    m_ListOfSocket: TSafeList;
    m_LogonOnMsg: TSafeStringList;
    // 用一个列表来记录所有DB操作返回，是为了保证先后顺序
    m_DBResultList: TSafeList;
    m_DBResultListTemp: TList;
    m_LastAuctionBroadcastTick: LongWord;
    m_AuctionBroadcastList: TSafeStringList;
    FPriorityMonGenListIndex: Integer;
    FPriorityMonGenList: TStringList;
    FRunMonsterList: TList;
  public
    m_ReallyPlayObjectList: TSafeList;
    m_PlayObjectList: TSafeStringList; // TPlayObjectList; //TStringList; // 0x8
    m_HeroObjectList: TSafeStringList; // 0x8
    m_MonObjectList: TSafeList; // 火龙殿中的守护兽 piaoyun 2013-08-19
    StdItemList: TSafeList; // List_54
    SortStdItemList: TGStringList; // 按名称排序的数据库物品列表
    MonsterList: TSafeList; // List_58
    m_MonGenList: TSafeList; // List_5C
    m_SortMapMonGenList: TSafeList;
    m_CustomMonsterList: TSafeList;
    m_CustomMagicList: TSafeList;
    m_CustomNpcList: TSafeList;
    m_MagicList: TSafeList;
    m_MagicACList: TMagicACList;
    m_AdminList: TSafeList; // List_64
    m_MerchantList: TSafeList; // List_68
    QuestNPCList: TSafeStringList; // 优化价值怪物死亡触发脚本过多创建NPC的问题(原类型TList) Cursor 2023-06-14 08:55:17
    m_MagicEventList: TSafeList; // 0x78
    OldMagicList: TSafeList;
    m_boStartLoadMagic: Boolean;
    m_boStartAutoLoadOffline: Boolean;
    m_dwStartAutoLoadOfflineTick: LongWord;
    m_AutoLoadOfflineList: TSafeList;
    m_boStartAutoLoadSellPlayer: Boolean;
    m_dwStartAutoLoadSellPlayerTick: LongWord;
    m_AutoLoadSellPlayerList: TSafeList;
    dwProcessMonstersTick: LongWord;
    dwProcessMerchantTimeMin: Integer;
    dwProcessMerchantTimeMax: Integer;
    dwProcessNpcTimeMin: LongWord;
    dwProcessNpcTimeMax: LongWord;
    m_SKILL_202Magic: PTUserMagic;
    function FindMonster(const sMonName: string; var Index: Integer): Boolean;
    procedure DoInitSortMapMonGenList;
    function FindFirstSortMapMonGenListIndex(sMapName: string): Integer;
  private
    FScriptCreateNpcInfos: TList;

    procedure NPCinitialize;
    procedure MerchantInitialize;
    function GetGenMonCount(MonGen: pTMonGenInfo): Integer;
    function AddBaseObject(Map: TEnvirnoment; nX, nY: Integer; nMonRace: Integer; sMonName: string): TBaseObject;
    function AddDummyObject(sDummyName, sMapName: string; nX, nY: Integer): TDummyObject;
    // procedure SaveHumRecord(PlayObject: TPlayObject);
    procedure SaveHeroRecord(HeroObject: THeroObject);
    procedure AddToHumanFreeList(PlayObject: TPlayObject);
    procedure AddToHeroFreeList(HeroObject: THeroObject);
    procedure GetHumData(PlayObject: TPlayObject; HumData: PTHumData);
    procedure GetHeroData(HeroObject: THeroObject; HeroData: PTHeroData);
    function GetHomeInfo(var nX: Integer; var nY: Integer): string;
    function GetRandHomeX(PlayObject: TPlayObject): Integer;
    function GetRandHomeY(PlayObject: TPlayObject): Integer;
    // function MapRageHuman(sMapName: string; nMapX, nMapY, nRage: Integer): Boolean;
    function GetOnlineHumCount(): Integer;
    function GetOnlineRealHumCount: Integer;
    function GetUserCount(): Integer;
    function GetLoadPlayCount(): Integer;
    function MakeNewHuman(LoadHuman: PTDBLoadHuman; Data: PTHumData): TPlayObject;
    function HumanIsLogined(sAccount, sChrName: string): Boolean;
    procedure ProcessHeroDBResult(DBResult: PTDBResult);
    procedure ProcessHumanDBResult(DBResult: PTDBResult);
    procedure ProcessDBRenameResult(DBResult: PTDBResult);
    procedure ProcessDBQueryHumanInfoResult(DBResult: PTDBResult);
    procedure ProcessDBGetRankDataResult(DBResult: PTDBResult);
    procedure ProcessDBBuyPlayerResult(DBResult: PTDBResult);
    procedure ProcessDBSellPlayerDelegatorResult(DBResult: PTDBResult);
  public
    dwProcessMapDoorTick: LongWord; // 0x20
    dwProcessMissionsTime: LongWord; // 0x24
    procedure ProcessDBResult();
    procedure ProcessAuctionBroadcastList();
    procedure ProcessHumans();
    procedure ProcessRegenMonsters();
    procedure ProcessMonsters();
    procedure ProcessMerchants();
    procedure ProcessNpcs();
    procedure ProcessEvents();
    procedure ProcessMapDoor();
    procedure ProcessHeros();
    // 副本地图 ----处理数据 chongchong 2013-09-06
    procedure ProcessFBMap;
    procedure ProcessFBMapObjectCount;
    procedure ProcessMirrorMap;
    procedure ProcessMirrorMapObjectCount;
    procedure ProcessStatMapManCount(); // 智能刷怪 chongchong 2014-01-07
  public
    constructor Create();
    destructor Destroy; override;
    procedure Initialize();
    procedure ClearItemList(); virtual;
    procedure SwitchMagicList();
    procedure KickOnlineUser(sChrName: string);
    procedure KickOnlineHero(sChrName: string);
    procedure ChangeMyShopType;
    procedure Run();
    procedure PrcocessData();
    procedure Execute;
    function RegenMonsterByName(sMAP: string; nX, nY: Integer; sMonName: string): TBaseObject;
    function RegenMonsters(MonGen: pTMonGenInfo; nCount: Integer): Boolean;
    procedure ClearSortStdItemList;
    procedure InitSortStdItemList;
    function GetStdItem(nItemIdx: Integer): pTStdItem; overload;
    function GetStdItem(sItemName: string): pTStdItem; overload;
    function GetStdItemEx(sItemName: string; var Idx: Integer): pTStdItem;
    function GetStdItemWeight(nItemIdx: Integer): Integer;
    function GetStdItemName(nItemIdx: Integer): string;
    function GetStdItemChangeName(UserItem: pTUserItem): string; // 获取改名
    function GetStdItemIdx(sItemName: string): Integer;
    procedure CryCry(wIdent: Word; pMap: TEnvirnoment; nX, nY, nWide: Integer; btFColor, btBColor: Byte; sMsg: string);
    procedure CryCryEx(wIdent: Word; pMap: TEnvirnoment; nX, nY, nWide: Integer; btFColor, btBColor: Byte; sMsg: string);
    procedure ProcessUserMessage(PlayObject: TPlayObject; DefMsg: pTDefaultMessage; Buff: PAnsiChar);
    function GetMonInfo(sMonName: string): pTMonInfo;
    function GetMonRace(sMonName: string): Integer;
    function GetMonRaceImg(sMonName: string): Integer;
    function GetMonRaceImgAndAppr(sMonName: string; var btMonReaceImg: Byte; var wMonAppr: Word; var btRace: Byte): Boolean;
    function GetMonName(nRace: Integer): string;
    // function InMonstersList(MonGen: pTMonGenInfo; Monster: TAnimalObject): Boolean;
    function GetPlayObject(sName: string): TPlayObject; overload;
    function GetPlayObject(PlayObject: TObject): TPlayObject; overload;
    function GetPlayObjectOfAccount(sAccount: string): TPlayObject;
    function GetPlayObjectExOfOffLine(sAccount: string): TPlayObject;
    function GetPlayObjectExOfOffLineEx(sAccount, sChrName: string): TPlayObject;
    procedure AddLoadOffline(PlayObject: TPlayObject; sAccount, sChrName, sIPaddr, sMachineID, sUserMachineID: string;
      boFlag: Boolean; nSessionID, nPayMent, nPayMode, nSoftVersionDate, nSocket, nGSocketIdx, nGateIdx, nKey: Integer;
      nClientWidth, nClientHeight: Integer; nClientBuildVer: Integer; sPromotionFlag: string);
    function GetHeroObject(sName: string): THeroObject;
    function FindMerchant(Merchant: TObject): TMerchant;
    function FindMerchantByName(const MapName: string; const MerchantName: string): TMerchant;
    function FindMerchantByPosition(const MapName: string; const X, Y: Integer): TMerchant;
    function FindNPC(NPC: TObject): TNormNpc; overload;
    function FindNPC(const ANpcName: string): TNormNpc; overload;
    function FindNPCByName(const MapName: string; const NPCName: string): TNormNpc;
    function FindNPCByPosition(const MapName: string; const X, Y: Integer): TNormNpc;
    function CopyToUserItemFromName(sItemName: string; Item: pTUserItem): Boolean;
    function CopyToUserItemFromItem(StdItem: pTStdItem; wIndex: Integer; Item: pTUserItem): Boolean;
    function GetMapOfRangeHumanCount(Envir: TEnvirnoment; nX, nY, nRange: Integer): Integer;
    function GetHumPermission(sUserName: string; var sIPaddr: string; var btPermission: Byte): Boolean;
    procedure AddDBResult(DBResult: PTDBResult);
    {
      procedure AddDBLoadHumanResult(DBLoadHumanResult: PTDBLoadHumanResult);
      procedure AddDBLoadHeroResult(DBLoadHeroResult: PTDBLoadHeroResult);
      procedure AddDBHeroOtherResult(DBHeroOtherResult: PTDBHeroOtherResult);
      procedure AddDBChrRenameResult(DBRenameChrResult: PTDBRenameChrResult);
    }
    procedure RandomUpgradeItem(Item: pTUserItem);
    procedure GetUnknowItemValue(Item: pTUserItem);
    procedure RandomItemNewAbil(ItemUpgradeRate: TItemUpgradeRate; Item: pTUserItem); overload;
    procedure RandomItemNewAbil(ItemNewAbilRate: Integer; Item: pTUserItem); overload;
    function OpenDoor(Envir: TEnvirnoment; nViewRange: Integer; nX, nY: Integer): Boolean;
    function CloseDoor(Envir: TEnvirnoment; nViewRange: Integer; Door: TDoorObject): Boolean;
    procedure SendDoorStatus(Envir: TEnvirnoment; nViewRange: Integer; nX, nY: Integer; wIdent, wX: Word;
      nDoorX, nDoorY, nA: Integer; sStr: string);
    function FindMagicEx(sMagicName: string): pTMagic;
    function FindMagic(sMagicName: string): pTMagic; overload;
    function FindMagic(nMagIdx: Integer): pTMagic; overload;
    function FindDummyMagic(IsHuman: Boolean; sMagicName: string): pTMagic;
    function FindHeroMagic(nMagIdx: Integer): pTMagic; overload;
    function FindHeroMagic(sMagicName: string): pTMagic; overload;
    function FindMagic(sMagicName: string; MagicAttr: TMagicAttr): pTMagic; overload;
    function FindMagic(nMagIdx: Integer; MagicAttr: TMagicAttr): pTMagic; overload;
    function FindHeroMagic(nMagIdx: Integer; MagicAttr: TMagicAttr): pTMagic; overload;
    function FindHeroMagic(sMagicName: string; MagicAttr: TMagicAttr): pTMagic; overload;
    procedure AddMerchant(Merchant: TMerchant);
    procedure AddNpc(const ACharName: string; AMerchant: TBaseObject);
    function GetMerchantList(Envir: TEnvirnoment; nX, nY, nRange: Integer; TmpList: TList): Integer;
    function GetNpcList(Envir: TEnvirnoment; nX, nY, nRange: Integer; TmpList: TList): Integer;
    procedure ReloadMerchantList();
    procedure ReloadNpcList();
    procedure HumanExpire(sAccount: string);
    function GetMapMonster(Envir: TEnvirnoment; List: TList; boIncludeDie: Boolean = False): Integer;
    function FindMapMonster(Envir: TEnvirnoment; sName: string; nX: Integer = -1; nY: Integer = -1): TBaseObject;
    // function GetMapRangeMonster(Envir: TEnvirnoment; nX, nY, nRange: Integer; List: TList): Integer;
    function GetMapHuman(sMapName: string): Integer; overload;
    function GetMapHuman(Envir: TEnvirnoment): Integer; overload;
    function GetMapRageHuman(Envir: TEnvirnoment; nRageX, nRageY, nRage: Integer; List: TList): Integer;
    procedure SendBroadCastMsg(sMsg: string; MsgType: TMsgType; MsgFrom: TMsgFrom = mfOther); overload;
    procedure SendBroadCastMsg(sMsg: string; FColor, BColor: Integer; MsgType: TMsgType; MsgFrom: TMsgFrom = mfOther); overload;
    procedure SendBroadCastMsgExt(sMsg: string; MsgType: TMsgType; MsgFrom: TMsgFrom = mfOther);
    procedure SendTopBroadCastMsg(sMsg: string; FColor, BColor: Integer; nTime: Integer; MsgType: TMsgType;
      MsgFrom: TMsgFrom = mfOther);
    procedure SendServerConfig();
    procedure SendCustomItemPropertyConfig();
    procedure SendCustomItemPropertyTextVarList();
    procedure SendMapCanRun();
    procedure SendMoveMsg(sMsg: string; btFColor, btBColor: Byte; nY, nMoveCount: Integer; nFontSize: Integer;
      nMarqueeTime: Integer);
    // 发送屏幕震动消息 piaoyun 2013-09-14
    procedure SendSceneShake(Count: Integer);
    // 仿盛大顶部渐隐消息 piaoyun 2013-08-01
    procedure SendSuperMoveMsg(sMsg: string; btFColor, btBColor, btFontSize: Byte; nX, nY, nMoveCount: Integer);
    // 换行消息 piaoyun 2013-08-03
    procedure SendNewLineMsg(sMsg: string; btFColor, btBColor, btFontSize: Byte; nX, nY, nShowMsgTime, nDrawType: Integer);
    procedure SendCenterMsg(sMsg: string; btFColor, btBColor: Byte; nTime: Integer);
    procedure WeatherChanged(Envir: TEnvirnoment);
    procedure SendMissionNpc();
    procedure SendEffectImageList();
    procedure SendSpecialCmdList(); // 发送特殊命令
    procedure SendUnbindList();
    procedure SendStdItemList(); // 发送所有物品 chongchong 2015-01-03
    procedure RefShowName(); // 刷新名称-神秘人 piaoyun 2013-08-17
    procedure SendPlugClientList();
    procedure SendClientModules();
    procedure SendMapDescription(Envir: TEnvirnoment);
    procedure ClearMonSayMsg();
    procedure SendQuestMsg(sQuestName: string);
    procedure ClearMerchantData();
    procedure SendFilterItemList;
    procedure SendItemDescList;
    procedure SendItemDescTopList;
    procedure SendTzItemDescList;
    procedure SendDropItemEffectList;
    procedure SendEnabledAuctionItemList;
    function AddMapMonGenCount(sMapName: string; nMonGenCount: Integer): Integer;
    function GetMapMonGenCount(sMapName: string): pTMapMonGenCount;
    function ClearMonsters(sMapName: string): Boolean;
    procedure AddToMonsterList(BaseObject: TBaseObject);
    procedure ReloadMagicList(); // 出现加载魔法数据库后，需要刷新魔法数据指针
    procedure ReloadHeroMagicList(); // 出现加载魔法数据库后，需要刷新魔法数据指针
    function MonGetRandomItems(Hitter: TBaseObject; mon: TBaseObject; ExtRate, ExtRate2: Integer; IsPreview: Boolean): Integer;
    procedure SaveHumRecord(PlayObject: TPlayObject);
    function CreateBaseObjectByRace(MonName: string; MonRace, nX, nY: Integer): TBaseObject;
    procedure MonInitialize(BaseObject: TBaseObject; sMonName: string);
    procedure MonRecalcASubAbilitys(BaseObject: TBaseObject; sMonName: string);
    procedure SetMonIcons(mon: TBaseObject);
    function FindDummyLogon(const sCharName: string): Boolean;
    procedure AddDummyLogon(Dummy: pTDummyLogon);
    procedure DelDummyLogon(const sCharName: string);
    function RegenDummyObject(DummyLogon: pTDummyLogon): Boolean;
    function GetDummyObjectCount(): Integer; overload;
    function GetDummyObjectCount(Envir: TEnvirnoment): Integer; overload;
    function GetOfflineCount(): Integer;
    function GetReallyCount(): Integer;
    // function GetReallyCount_NoLock(): Integer;
    property MonsterCount: Integer read nMonsterCount;
    property OnlinePlayObject: Integer read GetOnlineHumCount;
    property OnlineRealPlayObject: Integer read GetOnlineRealHumCount;
    property PlayObjectCount: Integer read GetUserCount;
    property LoadPlayCount: Integer read GetLoadPlayCount;
    procedure AddLogonOnMessage(const sChrName, Msg: string; FColor, BColor: Byte);
    function GetLogonMessage(sChrName: string; MsgList: TStrings): Boolean;
    procedure AddAuctionBroadcastMessage(sText: string; AuctionID: Integer);
    function AddScriptCreateNpcInfo(ANpc: TNormNpc; ABaseObj: TBaseObject; APlayObj: TPlayObject;
      AQuestActionInfo: pTQuestActionInfo): Boolean;
    function DelScriptCreateNpcInfo(const ANpcName, AMapName: string): Boolean; overload;
    function DelScriptCreateNpcInfo(const AMapName: string): Boolean; overload;
    function MerchantExists(AEnvir: TEnvirnoment; AX, AY: Integer): Boolean;
  end;

var
  g_dwEngineTick: LongWord;
  g_dwEngineRunTime: LongWord;

implementation

uses IdSrvClient, Guild, ObjMon, ObjGuard, ObjAxeMon, M2Share, LocalDB, NpcActionCmd,
  ObjMon2, GameEvent, ObjRobot, HUtil32, svMain, Math,
  Castle, ObjSmartMon, ObjFireDragon, ObjCustomMon, {$IF LUA_SCRIPT = 1}LuaEvent, LuaActor, {$IFEND}
  uCustomMonsterUtils, uCustomMagicUtils, uCustomNpcUtils,
  RunSock, ItemEvent, M2DataCommon;

{ TUserEngine }
constructor TUserEngine.Create();
begin
  FScriptCreateNpcInfos := TList.Create;

  m_ReallyPlayObjectList := TSafeList.Create('TUserEngine.m_ReallyPlayObjectList');
  m_PlayObjectList := TSafeStringList.Create('TUserEngine.m_PlayObjectList'); // TPlayObjectList.Create; //TStringList.Create;
  m_MonObjectList := TSafeList.Create('TUserEngine.m_MonObjectList'); // 火龙殿中的守护兽 piaoyun 2013-08-19
  m_DummyLogonList := TSafeStringList.Create('TUserEngine.m_DummyLogonList'); // 假人登录
  m_HeroObjectList := TSafeStringList.Create('TUserEngine.m_HeroObjectList');
  m_HeroObjectFreeList := TSafeList.Create('TUserEngine.m_HeroObjectFreeList');
  m_LoadOfflineList := TSafeStringList.Create('TUserEngine.m_LoadOfflineList');
  m_PlayObjectFreeList := TSafeList.Create('TUserEngine.m_PlayObjectFreeList');
  dwShowOnlineTick := MyGetTickCount;
  dwSendOnlineHumTime := MyGetTickCount;
  dwProcessMapDoorTick := MyGetTickCount;
  dwProcessMissionsTime := MyGetTickCount;
  dwProcessMonstersTick := MyGetTickCount;
  dwRegenMonstersTick := MyGetTickCount;
  m_dwProcessLoadPlayTick := MyGetTickCount;
  m_dwDummyLogonTick := MyGetTickCount;
  m_nCurrMonGen := 0;
  m_nMonGenListPosition := 0;
  m_nMonGenCertListPosition := 0;
  m_nProcHumIDx := 0;
  nProcessHumanLoopTime := 0;
  nMerchantPosition := 0;
  nNpcPosition := 0;
  nHeroPosition := 0;
  StdItemList := TSafeList.Create('TUserEngine.StdItemList'); // List_54
  SortStdItemList := TGStringList.Create;
  MonsterList := TSafeList.Create('TUserEngine.MonsterList');
  m_MonGenList := TSafeList.Create('TUserEngine.m_MonGenList');
  FPriorityMonGenListIndex := 0;
  FPriorityMonGenList := TStringList.Create;
  m_SortMapMonGenList := TSafeList.Create('TUserEngine_m_SortMapMonGenList');
  m_CustomMonsterList := TSafeList.Create('TUserEngine.m_CustomMonsterList');
  m_CustomMagicList := TSafeList.Create('TUserEngine.m_CustomMagicList');
  m_CustomNpcList := TSafeList.Create('TUserEngine.m_CustomNpcList');
  m_MagicACList := TMagicACList.Create('TUserEngine.m_MagicACList');
  m_MagicList := TSafeList.Create('TUserEngine.m_MagicList');
  m_MapMonGenCountList := TSafeList.Create('TUserEngine.m_MapMonGenCountList');
  m_AdminList := TSafeList.Create('TUserEngine.m_AdminList');
  m_MerchantList := TSafeList.Create('TUserEngine.m_MerchantList');
  QuestNPCList := TSafeStringList.Create('TUserEngine.QuestNPCList');
  m_MagicEventList := TSafeList.Create('TUserEngine.m_MagicEventList');
  boItemEvent := False;
  dwProcessMerchantTimeMin := 0;
  dwProcessMerchantTimeMax := 0;
  dwProcessNpcTimeMin := 0;
  dwProcessNpcTimeMax := 0;
  nMonsterProcessPostion := 0;
  dwProcessHeroTimeMin := 0;
  dwProcessHeroTimeMax := 0;
  m_NewHumanList := TSafeList.Create('TUserEngine.m_NewHumanList');
  m_ListOfGateIdx := TSafeList.Create('TUserEngine.m_ListOfGateIdx');
  m_ListOfSocket := TSafeList.Create('TUserEngine.m_ListOfSocket');
  OldMagicList := TSafeList.Create('TUserEngine.OldMagicList');
  m_boStartLoadMagic := False;
  m_boStartAutoLoadOffline := False;
  m_dwStartAutoLoadOfflineTick := MyGetTickCount;
  m_AutoLoadOfflineList := TSafeList.Create('TUserEngine.m_AutoLoadOfflineList');
  m_boStartAutoLoadSellPlayer := False;
  m_dwStartAutoLoadSellPlayerTick := MyGetTickCount;
  m_AutoLoadSellPlayerList := TSafeList.Create('TUserEngine.m_AutoLoadSellPlayerList');
  m_LogonOnMsg := TSafeStringList.Create('TUserEngine.m_LogonOnMsg');
  m_DBResultList := TSafeList.Create('TUserEngine.m_DBResultList');
  m_DBResultListTemp := TList.Create;
  m_DBResultListTemp.Capacity := 400;
  m_AuctionBroadcastList := TSafeStringList.Create('TUserEngine.m_AuctionBroadcastList');
  m_LastAuctionBroadcastTick := MyGetTickCount;
  FRunMonsterList := TList.Create;
  FRunMonsterList.Capacity := 200000;
end;

destructor TUserEngine.Destroy;
var
  I: Integer;
  II: Integer;
  MonInfo: pTMonInfo;
  MonGenInfo: pTMonGenInfo;
  MagicEvent: pTMagicEvent;
  TmpList: TList;
  CustomMonsterConfig: TCustomMonsterConfig;
  CustomMagicConfig: TCustomMagicConfig;
  CustomNpcConfig: TCustomNpcConfig;
  DBResult: PTDBResult;
begin
  m_MonObjectList.Free;
  m_ReallyPlayObjectList.Free;
  for I := 0 to m_PlayObjectList.Count - 1 do
  begin
    TPlayObject(m_PlayObjectList.Objects[I]).Free;
  end;
  m_PlayObjectList.Free;
  for I := 0 to m_PlayObjectFreeList.Count - 1 do
  begin
    TPlayObject(m_PlayObjectFreeList.Items[I]).Free;
  end;
  m_PlayObjectFreeList.Free;
  for I := 0 to m_HeroObjectList.Count - 1 do
  begin
    THeroObject(m_HeroObjectList.Objects[I]).Free;
  end;
  m_HeroObjectList.Free;
  for I := 0 to m_HeroObjectFreeList.Count - 1 do
  begin
    THeroObject(m_HeroObjectFreeList.Items[I]).Free;
  end;
  m_HeroObjectFreeList.Free;
  for I := 0 to m_DummyLogonList.Count - 1 do
  begin
    Dispose(pTDummyLogon(m_DummyLogonList.Objects[I]));
  end;
  m_DummyLogonList.Free;
  for I := 0 to m_LoadOfflineList.Count - 1 do
  begin
    Dispose(PTDBLoadHumanResult(m_LoadOfflineList.Objects[I]));
  end;
  m_LoadOfflineList.Free;
  for I := 0 to StdItemList.Count - 1 do
  begin
    Dispose(pTStdItem(StdItemList.Items[I]));
  end;
  StdItemList.Free;
  ClearSortStdItemList;
  SortStdItemList.Free;
  for I := 0 to MonsterList.Count - 1 do
  begin
    MonInfo := MonsterList.Items[I];
    if MonInfo.ItemList <> nil then
    begin
      ClearMonItemList(MonInfo.ItemList);
      MonInfo.ItemList.Free;
    end;
    Dispose(MonInfo);
  end;
  MonsterList.Free;
  // 自定义怪物列表 chongchong 2014-07-19
  for I := 0 to m_CustomMonsterList.Count - 1 do
  begin
    CustomMonsterConfig := m_CustomMonsterList.Items[I];
    CustomMonsterConfig.Free;
  end;
  m_CustomMonsterList.Free;
  // 自定义技能列表 chongchong 2014-07-19
  for I := 0 to m_CustomMagicList.Count - 1 do
  begin
    CustomMagicConfig := m_CustomMagicList.Items[I];
    CustomMagicConfig.Free;
  end;
  m_CustomMagicList.Free;
  // 自定义NPC列表 chongchong 2014-07-19
  for I := 0 to m_CustomNpcList.Count - 1 do
  begin
    CustomNpcConfig := m_CustomNpcList.Items[I];
    CustomNpcConfig.Free;
  end;
  m_CustomNpcList.Free;
  for I := 0 to m_MonGenList.Count - 1 do
  begin
    MonGenInfo := m_MonGenList.Items[I];
    for II := 0 to MonGenInfo.CertList.Count - 1 do
    begin
      TBaseObject(MonGenInfo.CertList.Items[II]).Free;
    end;
    { TODO -ochongchong -c内存泄露 : 去++++内存泄露【2013-07-17】 }
    MonGenInfo.CertList.Free;
    Dispose(pTMonGenInfo(m_MonGenList.Items[I]));
  end;
  m_MonGenList.Free;
  for I := 0 to FPriorityMonGenList.Count - 1 do
  begin
    Dispose(PPriorityMonGenRecord(FPriorityMonGenList.Objects[I]));
  end;
  FPriorityMonGenList.Free;
  m_SortMapMonGenList.Free;
  for I := 0 to m_MagicList.Count - 1 do
  begin
    Dispose(pTMagic(m_MagicList.Items[I]));
  end;
  m_MagicList.Free;
  m_MagicACList.Free;
  for I := 0 to m_MapMonGenCountList.Count - 1 do
  begin
    if pTMapMonGenCount(m_MapMonGenCountList.Items[I]) <> nil then
      Dispose(pTMapMonGenCount(m_MapMonGenCountList.Items[I]));
  end;
  m_MapMonGenCountList.Free;
  { TODO -ochongchong -c内存泄露 : 去++++内存泄露【2013-07-18】 }
  for I := 0 to m_AdminList.Count - 1 do
  begin
    Dispose(pTAdminInfo(m_AdminList.Items[I]));
  end;
  m_AdminList.Free;
  for I := 0 to m_MerchantList.Count - 1 do
  begin
    TMerchant(m_MerchantList.Items[I]).Free;
  end;
  m_MerchantList.Free;
  for I := 0 to QuestNPCList.Count - 1 do
  begin
    TNormNpc(QuestNPCList.Objects[I]).Free;
  end;
  QuestNPCList.Free;
  for I := 0 to m_MagicEventList.Count - 1 do
  begin
    MagicEvent := m_MagicEventList.Items[I];
    if MagicEvent.BaseObjectList_2 <> nil then
      MagicEvent.BaseObjectList_2.Free;
    if MagicEvent.Events_2 <> nil then
      MagicEvent.Events_2.Free;
    Dispose(MagicEvent);
  end;
  m_MagicEventList.Free;
  m_NewHumanList.Free;
  m_ListOfGateIdx.Free;
  m_ListOfSocket.Free;
  for I := 0 to OldMagicList.Count - 1 do
  begin
    TmpList := TList(OldMagicList.Items[I]);
    for II := 0 to TmpList.Count - 1 do
    begin
      Dispose(pTMagic(TmpList.Items[II]));
    end;
    TmpList.Free;
  end;
  OldMagicList.Free;
  for I := 0 to m_AutoLoadOfflineList.Count - 1 do
  begin
    Dispose(pTOffLineData(m_AutoLoadOfflineList.Items[I]));
  end;
  m_AutoLoadOfflineList.Free;
  for I := 0 to m_AutoLoadSellPlayerList.Count - 1 do
  begin
    Dispose(pTOffLineData(m_AutoLoadSellPlayerList.Items[I]));
  end;
  m_AutoLoadSellPlayerList.Free;
  if m_SKILL_202Magic <> nil then
    Dispose(m_SKILL_202Magic);
  m_LogonOnMsg.Free;
  for I := 0 to m_DBResultList.Count - 1 do
  begin
    DBResult := m_DBResultList.Items[I];
    if DBResult.LoadBuf <> nil then
    begin
      FreeMem(DBResult.LoadBuf, DBResult.LoadBufLen);
    end;
    if DBResult.DataBuf <> nil then
    begin
      FreeMem(DBResult.DataBuf, DBResult.DataBufLen);
    end;
    Dispose(DBResult);
  end;
  m_DBResultList.Free;
  m_DBResultListTemp.Free;
  m_AuctionBroadcastList.Free;
  FRunMonsterList.Free;

  for I := 0 to FScriptCreateNpcInfos.Count - 1 do
    FreeMemory(FScriptCreateNpcInfos[I]);
  FScriptCreateNpcInfos.Free;

  inherited;
end;

procedure TUserEngine.Initialize;
var
  I: Integer;
  MonGen: pTMonGenInfo;
begin
  MerchantInitialize();
  NPCinitialize();
  RobotManage.RELOADROBOT;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_MonGenList.LockR(7);
  try
{$IFEND}
    for I := 0 to m_MonGenList.Count - 1 do
    begin
      Application.ProcessMessages;
      MonGen := m_MonGenList.Items[I];
      if MonGen <> nil then
        MonGen.nRace := GetMonRace(MonGen.sMonName);
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_MonGenList.UnLockR;
  end;
{$IFEND}
end;

function TUserEngine.GetMonInfo(sMonName: string): pTMonInfo;
var
  I: Integer;
begin
  Result := nil;
  if FindMonster(sMonName, I) then
    Result := MonsterList.Items[I];
end;

function TUserEngine.GetMonRace(sMonName: string): Integer;
var
  I: Integer;
begin
  Result := -1;
  if FindMonster(sMonName, I) then
    Result := pTMonInfo(MonsterList.Items[I]).btRace;
end;

function TUserEngine.GetMonRaceImg(sMonName: string): Integer;
var
  I: Integer;
begin
  Result := -1;
  if FindMonster(sMonName, I) then
    Result := pTMonInfo(MonsterList.Items[I]).btRaceImg;
end;

function TUserEngine.GetMonRaceImgAndAppr(sMonName: string; var btMonReaceImg: Byte; var wMonAppr: Word;
  var btRace: Byte): Boolean;
var
  I: Integer;
  MonInfo: pTMonInfo;
begin
  Result := False;
  if FindMonster(sMonName, I) then
  begin
    MonInfo := MonsterList.Items[I];
    btMonReaceImg := MonInfo.btRaceImg;
    wMonAppr := MonInfo.wAppr;
    btRace := MonInfo.btRace;
    Result := True;
  end;
end;

function TUserEngine.GetMonName(nRace: Integer): string;
var
  I: Integer;
  MonInfo: pTMonInfo;
begin
  Result := '';
  for I := 0 to MonsterList.Count - 1 do
  begin
    MonInfo := MonsterList.Items[I];
    if MonInfo.btRace = nRace then
    begin
      Result := MonInfo.sName;
      Break;
    end;
  end;
end;

procedure TUserEngine.MerchantInitialize; // 004AC96C
var
  I: Integer;
  Merchant: TMerchant;
  sCaption: string;
begin
  sCaption := FrmMain.Caption;
  m_MerchantList.LockW(3);
  try
    for I := m_MerchantList.Count - 1 downto 0 do
    begin
      Merchant := TMerchant(m_MerchantList.Items[I]);
      if (g_boExitServer or Application.Terminated) then
        Break;

      if Merchant <> nil then
      begin
        Merchant.m_PEnvir := g_MapManager.FindMap(Merchant.m_sMapName);
        // MainOutMessage('MerchantInitialize1:'+Merchant.m_sMapName);
        if Merchant.m_PEnvir <> nil then
        begin
          // MainOutMessage('MerchantInitialize2:'+Merchant.m_PEnvir.sMapName);
          Merchant.Initialize;
          if Merchant.m_boAddtoMapFail and (not Merchant.m_boIsHide) then
          begin
            MainOutMessage('添加Npc到地图坐标失败.' + Merchant.m_sCharName + ' ' + Merchant.m_sMapName + '(' + IntToStr(Merchant.m_nCurrX)
              + ':' + IntToStr(Merchant.m_nCurrY) + ')', True, RVSTYLE_ERROR);
            m_MerchantList.Delete(I);
            if Merchant = g_ManageNPC then
              g_ManageNPC := nil;
            if Merchant = g_RobotNPC then
              g_RobotNPC := nil;
            if Merchant = g_FunctionNPC then
              g_FunctionNPC := nil;
            if Merchant = g_MissionNPC then
              g_MissionNPC := nil;
            if Merchant = g_BatterNPC then
              g_BatterNPC := nil;
            Merchant.Free;
          end
          else
          begin
            Merchant.LoadNpcScript();
            Merchant.LoadNPCData();
          end;
        end
        else
        begin
          MainOutMessage(Merchant.m_sCharName + ' Npc初始化失败... (地图未找到:' + Merchant.m_sMapName + ')', True, RVSTYLE_ERROR);
          m_MerchantList.Delete(I);
          if Merchant = g_ManageNPC then
            g_ManageNPC := nil;
          if Merchant = g_RobotNPC then
            g_RobotNPC := nil;
          if Merchant = g_FunctionNPC then
            g_FunctionNPC := nil;
          if Merchant = g_MissionNPC then
            g_MissionNPC := nil;
          if Merchant = g_BatterNPC then
            g_BatterNPC := nil;
          Merchant.Free;
        end;
        FrmMain.Caption := sCaption + ' [正在初始交易NPC(' + IntToStr(m_MerchantList.Count) + '/' +
          IntToStr(m_MerchantList.Count - I) + ')]';
        Application.ProcessMessages;
      end;
    end;
  finally
    m_MerchantList.UnLockW;
  end;
end;

function TUserEngine.MerchantExists(AEnvir: TEnvirnoment; AX, AY: Integer): Boolean;
var
  I: Integer;
  Merchant: TMerchant;
begin
  Result := False;
  m_MerchantList.LockR(4);
  try
    for I := 0 to m_MerchantList.Count - 1 do
    begin
      Merchant := TMerchant(m_MerchantList.Items[I]);
      if (Merchant <> nil) //
        and not Merchant.m_boGhost //
        and (Merchant.m_PEnvir = AEnvir) //
        and (Merchant.m_nCurrX = AX) //
        and (Merchant.m_nCurrY = AY) then
      begin
        Result := True;
        Exit;
      end;
    end;
  finally
    m_MerchantList.UnLockR;
  end;
end;

procedure TUserEngine.NPCinitialize;
var
  I: Integer;
  NormNpc: TNormNpc;
begin
  QuestNPCList.LockW(1);
  try
    for I := QuestNPCList.Count - 1 downto 0 do
    begin
      NormNpc := TNormNpc(QuestNPCList.Objects[I]);

      Application.ProcessMessages;
      if (g_boExitServer or Application.Terminated) then
        Break;

      if NormNpc <> nil then
      begin
        NormNpc.m_PEnvir := g_MapManager.FindMap(NormNpc.m_sMapName);
        if NormNpc.m_PEnvir <> nil then
        begin
          NormNpc.Initialize;

          if NormNpc.m_boAddtoMapFail and (not NormNpc.m_boIsHide) then
          begin
            MainOutMessage('添加Npc到地图坐标失败.' + NormNpc.m_sCharName + ' ' + NormNpc.m_sMapName + '(' + IntToStr(NormNpc.m_nCurrX) +
              ':' + IntToStr(NormNpc.m_nCurrY) + ')', True, RVSTYLE_ERROR);

            QuestNPCList.Delete(I);
            if NormNpc = g_ManageNPC then
              g_ManageNPC := nil;
            if NormNpc = g_RobotNPC then
              g_RobotNPC := nil;
            if NormNpc = g_FunctionNPC then
              g_FunctionNPC := nil;
            if NormNpc = g_MissionNPC then
              g_MissionNPC := nil;
            if NormNpc = g_BatterNPC then
              g_BatterNPC := nil;
            NormNpc.Free;
          end
          else
            NormNpc.LoadNpcScript();
        end
        else
        begin
          MainOutMessage(NormNpc.m_sCharName + ' Npc初始化失败... (地图未找到:' + NormNpc.m_sMapName + ')', True, RVSTYLE_ERROR);
          QuestNPCList.Delete(I);
          if NormNpc = g_ManageNPC then
            g_ManageNPC := nil;
          if NormNpc = g_RobotNPC then
            g_RobotNPC := nil;
          if NormNpc = g_FunctionNPC then
            g_FunctionNPC := nil;
          if NormNpc = g_MissionNPC then
            g_MissionNPC := nil;
          if NormNpc = g_BatterNPC then
            g_BatterNPC := nil;
          NormNpc.Free;
        end;
      end;
    end;
  finally
    QuestNPCList.UnLockW;
  end;
end;

function TUserEngine.GetLoadPlayCount: Integer;
var
  I: Integer;
  DBResult: PTDBResult;
begin
  Result := 0;
  m_DBResultList.LockR(1);
  try
    for I := 0 to m_DBResultList.Count - 1 do
    begin
      DBResult := m_DBResultList.Items[I];
      if DBResult.ResultType in [drtLoadHuman, drtLoadDummy { , drtLoadHero } ] then
        Inc(Result);
    end;
  finally
    m_DBResultList.UnLockR;
  end;
end;

function TUserEngine.GetOnlineHumCount: Integer;
begin
  Result := m_PlayObjectList.Count;
end;

function TUserEngine.GetOnlineRealHumCount: Integer;
var
  I: Integer;
  Player: TPlayObject;
begin
  Result := 0;
  for I := 0 to m_PlayObjectList.Count - 1 do
  begin
    Player := TPlayObject(m_PlayObjectList.Objects[I]);
    if not Player.m_boDummyObject then
      Inc(Result);
  end;
end;

function TUserEngine.GetUserCount: Integer;
begin
  Result := m_PlayObjectList.Count;
end;

procedure TUserEngine.ProcessHeros();
var
  dwRunTick, dwCurrTick: LongWord;
  nIdx, I: Integer;
  HeroObject: THeroObject;
  boProcessLimit: Boolean;
  HeroRunList: TList;
  ErrCode: Integer;
resourcestring
  sExceptionMsg1 = '[Exception] TUserEngine.ProcessHeros -> Code:=%d Err:%s';
  sExceptionMsg2 = '[Exception] TUserEngine.ProcessHeros; Error code: %d';
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_HeroObjectFreeList.LockW(1);
  try
{$IFEND}
    for I := m_HeroObjectFreeList.Count - 1 downto 0 do
    begin
      HeroObject := THeroObject(m_HeroObjectFreeList.Items[I]);
      try
        if (MyGetTickCount - HeroObject.m_dwGhostTick) > g_Config.dwHumanFreeDelayTime { 5 * 60 * 1000 } then
        begin
          THeroObject(m_HeroObjectFreeList.Items[I]).Free;
          m_HeroObjectFreeList.Delete(I);
        end;
      except
        on E: Exception do
          MainOutMessage(Format(sExceptionMsg1, [1, E.Message]));
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_HeroObjectFreeList.UnLockW;
  end;
{$IFEND}
  ErrCode := 0;
  dwRunTick := MyGetTickCount();
  boProcessLimit := False;

  HeroRunList := TList.Create; // HZQ
  try
    dwCurrTick := MyGetTickCount();
    m_HeroObjectList.LockW(4);
    try
      InterlockedExchange(nIdx, nHeroPosition);
      try
        while m_HeroObjectList.Count > nIdx do
        begin
          HeroObject := THeroObject(m_HeroObjectList.Objects[nIdx]);

          if tick_diff(HeroObject.m_dwRunTick, dwCurrTick) > g_Config.dwHeroRunTime then
          begin
            HeroObject.m_dwRunTick := dwCurrTick;
            if not HeroObject.m_boGhost then
            begin
              HeroRunList.Add(HeroObject);
              if not DataEngine.IsFull and (MyGetTickCount() - HeroObject.m_dwSaveRcdTick > g_Config.dwSaveHumanRcdTime) then
              begin
                HeroObject.m_dwSaveRcdTick := MyGetTickCount();
                SaveHeroRecord(HeroObject);
              end;
            end
            else
            begin
              m_HeroObjectList.Delete(nIdx);
              SaveHeroRecord(HeroObject);
              AddToHeroFreeList(HeroObject);
              Continue;
            end;
          end;

          InterlockedIncrement(nIdx);
          if (MyGetTickCount - dwRunTick) > g_dwHeroLimit then
          begin
            InterlockedExchange(nHeroPosition, nIdx);
            boProcessLimit := True;
            Break;
          end;
        end;

        if not boProcessLimit then
          InterlockedExchange(nHeroPosition, 0);
      except
        MainOutMessage(Format(sExceptionMsg2, [ErrCode]));
      end;
    finally
      m_HeroObjectList.UnLockW;
    end;

    for I := 0 to HeroRunList.Count - 1 do
    begin
      HeroObject := HeroRunList.Items[I];

      if ((MyGetTickCount - HeroObject.m_dwSearchTick) > HeroObject.m_dwSearchTime) //
        or ((HeroObject.m_btAttackMode in [0, 3]) // -
        and (not HeroObject.m_boTarget) // -
        and (not HeroObject.InSafeZone) // -
        and (HeroObject.m_TargetCret = nil)) then
      begin
        HeroObject.m_dwSearchTick := MyGetTickCount();
        HeroObject.SearchViewRange();
      end;
      HeroObject.Run;
    end;
  finally
    HeroRunList.Free;
  end;

  dwProcessHeroTimeMin := MyGetTickCount - dwRunTick;
  if dwProcessHeroTimeMin > dwProcessHeroTimeMax then
    dwProcessHeroTimeMax := dwProcessHeroTimeMin;
end;

function TUserEngine.MakeNewHuman(LoadHuman: PTDBLoadHuman; Data: PTHumData): TPlayObject;
var
  PlayObject: TPlayObject;
  Abil: pTAbility;
  Envir: TEnvirnoment;
  nC: Integer;
  // SwitchDataInfo: pTSwitchDataInfo;
  Castle: TUserCastle;
  SafeArea: TSafeArea;
  NationInfo: pTNationInfo;
  boHomeMap: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TUserEngine.MakeNewHuman';
  sChangeServerFail1 = 'chg-server-fail-1 [%d] -> [%d] [%s]';
  sChangeServerFail2 = '人物(%s)地图(%s)坐标(%d, %d)不可达';
  sChangeServerFail3 = 'chg-server-fail-3 [%d] -> [%d] [%s]';
  sChangeServerFail4 = 'chg-server-fail-4 [%d] -> [%d] [%s]';
  sErrorEnvirIsNil = '[Error] PlayObject.PEnvir = nil';
label
  ReGetMap;
begin
  Result := nil;
  try
    PlayObject := TPlayObject.Create;
    PlayObject.m_nKey := LoadHuman.nKey;
    GetHumData(PlayObject, Data);
    PlayObject.m_btRaceServer := RC_PLAYOBJECT;
    if PlayObject.m_sHomeMap = '' then
    begin
    ReGetMap:
      PlayObject.m_sHomeMap := GetHomeInfo(PlayObject.m_nHomeX, PlayObject.m_nHomeY);
      PlayObject.m_sMapName := PlayObject.m_sHomeMap;
      PlayObject.m_nCurrX := GetRandHomeX(PlayObject);
      PlayObject.m_nCurrY := GetRandHomeY(PlayObject);

      Abil := @PlayObject.m_Abil;
      Abil.Level := 1;
      Abil.AC1 := 0;
      Abil.AC2 := 0;
      Abil.MAC1 := 0;
      Abil.MAC2 := 0;
      Abil.DC1 := 1;
      Abil.DC2 := 2;
      Abil.MC1 := 1;
      Abil.MC2 := 2;
      Abil.SC1 := 1;
      Abil.SC2 := 2;
      Abil.MP := 15;
      Abil.HP := 15;
      Abil.MaxMP := 15;
      Abil.Exp := 0;
      Abil.MaxExp := 100;
      Abil.Weight := 100;
      Abil.MaxWeight := 100;
      PlayObject.m_boNewHuman := True;
    end;

    Envir := g_MapManager.GetMapInfo(nServerIndex, PlayObject.m_sMapName);
    if Envir <> nil then
    begin
      if Envir.m_boFight3Zone then
      begin // 是否在行会战争地图死亡
        if (PlayObject.m_Abil.HP <= 0) and (PlayObject.m_nFightZoneDieCount < 3) then
        begin
          PlayObject.m_Abil.HP := PlayObject.m_Abil.MaxHP;
          PlayObject.m_Abil.MP := PlayObject.m_Abil.MaxMP;
          PlayObject.m_boDieInFight3Zone := True;
        end
        else
          PlayObject.m_nFightZoneDieCount := 0;
      end;
    end
    else
    begin
      // MainOutMessage('人物[' + PlayObject.m_sCharName + ']所在地图[' + PlayObject.m_sMapName + ']未找到');
      // 镜像地图进入后小退，过段时间等镜像地图收回，再进入游戏
      if PlayObject.m_sHomeMap <> '' then
      begin
        Envir := g_MapManager.GetMapInfo(nServerIndex, PlayObject.m_sHomeMap);
        if Envir <> nil then
        begin
          if Envir.m_boFight3Zone then
          begin // 是否在行会战争地图死亡
            if (PlayObject.m_Abil.HP <= 0) and (PlayObject.m_nFightZoneDieCount < 3) then
            begin
              PlayObject.m_Abil.HP := PlayObject.m_Abil.MaxHP;
              PlayObject.m_Abil.MP := PlayObject.m_Abil.MaxMP;
              PlayObject.m_boDieInFight3Zone := True;
            end
            else
              PlayObject.m_nFightZoneDieCount := 0;
          end;
        end;
        PlayObject.m_sMapName := PlayObject.m_sHomeMap;
        PlayObject.m_nCurrX := PlayObject.m_nHomeX - 2 + Random(5);
        PlayObject.m_nCurrY := PlayObject.m_nHomeY - 2 + Random(5);
      end;
    end;

    PlayObject.m_MyGuild := g_GuildManager.MemberOfGuild(PlayObject.m_sCharName);
    Castle := g_CastleManager.InCastleWarArea(Envir, PlayObject.m_nCurrX, PlayObject.m_nCurrY);
    if (Envir <> nil) and (Castle <> nil) and ((Castle.m_MapPalace = Envir) or Castle.m_boUnderWar) then
    begin
      Castle := g_CastleManager.IsCastleMember(PlayObject);
      if Castle = nil then
      begin
        PlayObject.m_sMapName := PlayObject.m_sHomeMap;
        PlayObject.m_nCurrX := PlayObject.m_nHomeX - 2 + Random(5);
        PlayObject.m_nCurrY := PlayObject.m_nHomeY - 2 + Random(5);
      end
      else
      begin
        if Castle.m_MapPalace = Envir then
        begin
          PlayObject.m_sMapName := Castle.GetMapName();
          PlayObject.m_nCurrX := Castle.GetHomeX;
          PlayObject.m_nCurrY := Castle.GetHomeY;
        end;
      end;
    end;

    if g_MapManager.FindMap(PlayObject.m_sMapName) = nil then
      PlayObject.m_Abil.HP := 0;

    if PlayObject.m_Abil.HP <= 0 then
    begin
      PlayObject.ClearStatusTime();
      if PlayObject.PKLevel < 2 then
      begin
        Castle := g_CastleManager.IsCastleMember(PlayObject);
        // if UserCastle.m_boUnderWar and (UserCastle.IsMember(PlayObject)) then begin
        if (Castle <> nil) and Castle.m_boUnderWar then
        begin
          PlayObject.m_sMapName := Castle.m_sHomeMap;
          PlayObject.m_nCurrX := Castle.GetHomeX;
          PlayObject.m_nCurrY := Castle.GetHomeY;
        end
        else
        begin
          PlayObject.m_sMapName := PlayObject.m_sHomeMap;
          PlayObject.m_nCurrX := PlayObject.m_nHomeX - 2 + Random(5);
          PlayObject.m_nCurrY := PlayObject.m_nHomeY - 2 + Random(5);
        end;
      end
      else
      begin
        PlayObject.m_sMapName := g_Config.sRedDieHomeMap { '3' };
        PlayObject.m_nCurrX := Random(13) + g_Config.nRedDieHomeX { 839 };
        PlayObject.m_nCurrY := Random(13) + g_Config.nRedDieHomeY { 668 };
      end;
      PlayObject.m_Abil.HP := 14;
    end;

    if g_MapManager.FindMap(PlayObject.m_sMapName) = nil then
    begin
      PlayObject.m_sMapName := g_Config.sHomeMap;
      Envir := g_MapManager.FindMap(g_Config.sHomeMap);
      PlayObject.m_nCurrX := g_Config.nHomeX;
      PlayObject.m_nCurrY := g_Config.nHomeY;

      if (Envir = nil) or (not Envir.CanWalk(PlayObject.m_nCurrX, PlayObject.m_nCurrY, True)) then
        MainOutMessage('应急回城点配置错误(坐标不可到达); 游戏参数-->座标范围-->应急回城点');
    end;

    PlayObject.AbilCopyToWAbil();
    Envir := g_MapManager.GetMapInfo(nServerIndex, PlayObject.m_sMapName);
    if Envir = nil then
    begin
      // MainOutMessage('PlayObject.m_sMapName:'+PlayObject.m_sMapName);
      PlayObject.m_nSessionID := LoadHuman.nSessionID;
      PlayObject.m_nSocket := LoadHuman.nSocket;
      PlayObject.m_nGateIdx := LoadHuman.nGateIdx;
      PlayObject.m_nGSocketIdx := LoadHuman.nGSocketIdx;
      PlayObject.m_WAbil := PlayObject.m_Abil;
      PlayObject.m_nServerIndex := g_MapManager.GetMapOfServerIndex(PlayObject.m_sMapName);
      if PlayObject.m_Abil.HP <> 14 then
      begin
        MainOutMessage(Format(sChangeServerFail1, [nServerIndex, PlayObject.m_nServerIndex, PlayObject.m_sMapName]));
        { MainOutMessage('chg-server-fail-1 [' +
          IntToStr(nServerIndex) +
          '] -> [' +
          IntToStr(PlayObject.m_nServerIndex) +
          '] [' +
          PlayObject.m_sMapName +
          ']'); }
      end;
      // PlayObject.Free;
      FreeAndNil(PlayObject);
      Exit;
    end;

    nC := 0;
    while (True) do
    begin
      if Envir.CanWalk(PlayObject.m_nCurrX, PlayObject.m_nCurrY, True) then
        Break;
      PlayObject.m_nCurrX := PlayObject.m_nCurrX - 3 + Random(6);
      PlayObject.m_nCurrY := PlayObject.m_nCurrY - 3 + Random(6);
      Inc(nC);
      if nC >= 5 then
        Break;
    end;

    if not Envir.CanWalk(PlayObject.m_nCurrX, PlayObject.m_nCurrY, True) then
    begin
      MainOutMessage(Format(sChangeServerFail2, [PlayObject.m_sCharName, Envir.sMapName, PlayObject.m_nCurrX,
        PlayObject.m_nCurrY]));

      boHomeMap := False;
      if PlayObject.m_btNation > 0 then
      begin
        NationInfo := g_NationManage.Items[PlayObject.m_btNation];
        if NationInfo <> nil then
        begin
          PlayObject.m_sMapName := NationInfo.sHomeMap;
          Envir := g_MapManager.FindMap(NationInfo.sHomeMap);
          PlayObject.m_nCurrX := NationInfo.nHomeX;
          PlayObject.m_nCurrY := NationInfo.nHomeY;
          boHomeMap := True;
        end;
      end;

      if not boHomeMap then
      begin
        PlayObject.m_sMapName := g_Config.sHomeMap;
        Envir := g_MapManager.FindMap(g_Config.sHomeMap);
        PlayObject.m_nCurrX := g_Config.nHomeX;
        PlayObject.m_nCurrY := g_Config.nHomeY;
        if Envir <> nil then
        begin
          if not Envir.CanWalk(PlayObject.m_nCurrX, PlayObject.m_nCurrY, True) then
            MainOutMessage('应急回城点配置错误(坐标不可到达); 游戏参数-->座标范围-->应急回城点');
        end
        else
          MainOutMessage('应急回城点配置错误(地图错误); 游戏参数-->座标范围-->应急回城点');
      end;
    end;

    PlayObject.m_PEnvir := Envir;
    if PlayObject.m_PEnvir = nil then
    begin
      MainOutMessage(sErrorEnvirIsNil);
      goto ReGetMap;
    end
    else
      PlayObject.m_boReadyRun := False;

    if g_Config.boOffLineLoginSafeArea and LoadHuman.boOffLine then
    begin
      if not PlayObject.InSafeZone then
      begin
        FreeAndNil(PlayObject);
        Exit;
      end;
    end;

    if LoadHuman.boOffLine then
    begin
      if g_Config.btOffLineLoginMapName = 0 then
      begin
        if (PlayObject.m_PEnvir.sMapName <> g_Config.sSetOffLineLoginMapName) or (not PlayObject.InSafeZone) then
        begin
          Envir := g_MapManager.FindMap(g_Config.sSetOffLineLoginMapName);
          SafeArea := g_SafeAreaManager.MapSafeArea(g_Config.sSetOffLineLoginMapName);
          if Envir = nil then
            MainOutMessage('功能设置->脱机登录->脱机挂在指定地图安全区：地图配置错误')
          else if SafeArea = nil then
            MainOutMessage('脱机挂在地图[' + g_Config.sSetOffLineLoginMapName + ']安全区：未找到安全区配置');
        end
        else
        begin
          Envir := g_MapManager.FindMap(PlayObject.m_sMapName);
          SafeArea := g_SafeAreaManager.MapSafeArea(PlayObject.m_sMapName);
        end;

        if (SafeArea <> nil) and (Envir <> nil) then
        begin
          PlayObject.m_PEnvir := Envir;
          // 这里要修改，地图安全区 chongchong 2017-04-18
          if SafeArea is TRangeSafeArea then
          begin
            PlayObject.m_nCurrX := (SafeArea.GetCenterX - TRangeSafeArea(SafeArea).Range) +
              Random(TRangeSafeArea(SafeArea).Range * 2);
            PlayObject.m_nCurrY := (SafeArea.GetCenterY - TRangeSafeArea(SafeArea).Range) +
              Random(TRangeSafeArea(SafeArea).Range * 2);
          end
          else
          begin
            PlayObject.m_nCurrX := SafeArea.GetCenterX;
            PlayObject.m_nCurrY := SafeArea.GetCenterY;
          end;

          if PlayObject.m_PEnvir = nil then
          begin
            FreeAndNil(PlayObject);
            Exit;
          end;
        end
        else
        begin
          FreeAndNil(PlayObject);
          Exit;
        end;
      end
      else
      begin
        if (PlayObject.m_sMapName <> PlayObject.m_sHomeMap) or (not PlayObject.InSafeZone) then
          PlayObject.m_sMapName := PlayObject.m_sHomeMap;

        SafeArea := g_SafeAreaManager.MapSafeArea(PlayObject.m_sMapName);
        if SafeArea <> nil then
        begin
          PlayObject.m_PEnvir := g_MapManager.FindMap(PlayObject.m_sMapName);
          // 这里要修改，地图安全区 chongchong 2017-04-18
          if SafeArea is TRangeSafeArea then
          begin
            PlayObject.m_nCurrX := (SafeArea.GetCenterX - TRangeSafeArea(SafeArea).Range) +
              Random(TRangeSafeArea(SafeArea).Range * 2);
            PlayObject.m_nCurrY := (SafeArea.GetCenterY - TRangeSafeArea(SafeArea).Range) +
              Random(TRangeSafeArea(SafeArea).Range * 2);
          end
          else
          begin
            PlayObject.m_nCurrX := SafeArea.GetCenterX;
            PlayObject.m_nCurrY := SafeArea.GetCenterY;
          end;
          if PlayObject.m_PEnvir = nil then
          begin
            FreeAndNil(PlayObject);
            Exit;
          end;
        end
        else
        begin
          FreeAndNil(PlayObject);
          Exit;
        end;
      end;
    end;

    PlayObject.m_sUserID := LoadHuman.sAccount;
    PlayObject.m_sIPaddr := LoadHuman.sIPaddr;
    PlayObject.m_sIPLocal := GetIPLocal(PlayObject.m_sIPaddr);
    PlayObject.m_nSocket := LoadHuman.nSocket;
    PlayObject.m_nGSocketIdx := LoadHuman.nGSocketIdx;
    PlayObject.m_nGateIdx := LoadHuman.nGateIdx;
    PlayObject.m_nSessionID := LoadHuman.nSessionID;
    PlayObject.m_nPayMent := LoadHuman.nPayMent;
    PlayObject.m_nPayMode := LoadHuman.nPayMode;
    PlayObject.m_dwLoadTick := MyGetTickCount;
    PlayObject.SoftVersionDateEx := GetExVersionNO(LoadHuman.nSoftVersionDate, PlayObject.m_nSoftVersionDate);
    PlayObject.m_boOffLine := LoadHuman.boOffLine;
    PlayObject.m_sMachineID := LoadHuman.sMachineID; // 机器码
    PlayObject.m_sUserMachineID := LoadHuman.sUserMachineID; // 机器码
    PlayObject.m_nClientWidth := LoadHuman.nClientWidth;
    PlayObject.m_nClientHeight := LoadHuman.nClientHeight;
    PlayObject.m_ClientBuildVersion := LoadHuman.nClientBuildVer;
    PlayObject.m_sGamePromotionFlag := LoadHuman.sPromotionFlag;
    if g_SellPlayerList.Search(PlayObject.m_sCharName, nC) then
      PlayObject.m_boSellPlayering := True;

    if LoadHuman.boReconnection then
    begin
      PlayObject.m_boSendNotice := True;
      PlayObject.m_boLoginNoticeOK := True;
      PlayObject.m_dwClientTick := 0;
    end;
    Result := PlayObject;
  except
    on E: Exception do
    begin
      MainOutMessage(sExceptionMsg);
      MainOutMessage(E.Message);
    end;
  end;
end;

function TUserEngine.HumanIsLogined(sAccount, sChrName: string): Boolean;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := False;
  if DataEngine.CheckHumanInSaveList(sAccount, sChrName) then
  begin
    Result := True;
  end
  else
  begin
    m_PlayObjectList.LockR(1);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if (CompareText(PlayObject.m_sUserID, sAccount) = 0) //
          and (CompareText(m_PlayObjectList.Strings[I], sChrName) = 0) //
          and (not PlayObject.m_boGhost) { and (not PlayObject.m_boReconnection) } then
        begin
          Result := True;
          Break;
        end;
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
  end;
end;

procedure TUserEngine.ProcessHumanDBResult(DBResult: PTDBResult);
var
  I: Integer;
  LoadHuman: PTDBLoadHuman;
  HumData: PTHumData;
  PlayObject: TPlayObject;
  SellPlayerInfo: PSellPlayerInfo;
begin
  LoadHuman := PTDBLoadHuman(DBResult.LoadBuf);
  HumData := PTHumData(DBResult.DataBuf);
  if (not DataEngine.IsFull) and (not HumanIsLogined(LoadHuman.sAccount, LoadHuman.sHumanName)) then
  begin
    PlayObject := MakeNewHuman(LoadHuman, HumData);
    if PlayObject <> nil then
    begin
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        m_NewHumanList.LockW(1);
      try
{$IFEND}
        m_NewHumanList.Add(PlayObject);
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          m_NewHumanList.UnLockW;
      end;
{$IFEND}
      if g_SellPlayerList.Search(PlayObject.m_sCharName, I) then
      begin
        SellPlayerInfo := g_SellPlayerList.Items[I];
        PlayObject.SendDefMessage(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0, '本角色正在委托 ' + SellPlayerInfo.Delegater +
          ' 出售中.. \\出售期间无法登录游戏，取消出售后方可正常登录游戏');
        // 离线挂机
        PlayObject.m_nOffOnlineTick := MyGetTickCount;
        PlayObject.m_nOffOnlineTime := 0;
        PlayObject.m_dwOfflineGetExpTick := MyGetTickCount;
        PlayObject.m_nOfflineGetExpTime := 0;
        PlayObject.m_nOffOnlineExp := 0;
        PlayObject.m_boReconnection := False;
        PlayObject.m_boSoftClose := True;
        PlayObject.m_boOffLine := True;
        PlayObject.m_boOfflineKick := False;
        UserEngine.m_ReallyPlayObjectList.Remove(PlayObject);
        PlayObject.m_boAllowChallenge := False; // 是否允许挑战
        PlayObject.m_boAllowDeal := False; // 禁止交易
        PlayObject.m_boAllowGuild := False; // 禁止加入行会
        PlayObject.m_boAllowGroup := False; // 禁止组队
        PlayObject.m_boCanMasterRecall := False; // 禁止师徒传送
        PlayObject.m_boCanDearRecall := False; // 禁止夫妻传送
        PlayObject.m_boAllowGuildReCall := False; // 禁止行会合一
        PlayObject.m_boAllowGroupReCall := False; // 禁止天地合一
        PlayObject.m_boDisableHorseInvite := True; // 禁止邀请上马 chongchong 2013-10-16
        PlayObject.m_boShopStall := False;
      end;
    end
    else
    begin
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        m_ListOfGateIdx.LockW(1);
      try
{$IFEND}
        m_ListOfGateIdx.Add(Pointer(LoadHuman.nGateIdx)); // 004B0C39
        m_ListOfSocket.Add(Pointer(LoadHuman.nSocket));
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          m_ListOfGateIdx.UnLockW;
      end;
{$IFEND}
    end;
  end
  else
  begin
    // MainOutMessage('m_NewHumanList 2 ' + m_LoadPlayList.Strings[I]);
    KickOnlineUser(LoadHuman.sHumanName);
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_ListOfGateIdx.LockW(2);
    try
{$IFEND}
      m_ListOfGateIdx.Add(Pointer(LoadHuman.nGateIdx)); // 004B0C39
      m_ListOfSocket.Add(Pointer(LoadHuman.nSocket));
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_ListOfGateIdx.UnLockW;
    end;
{$IFEND}
  end;
end;

procedure TUserEngine.ProcessHeroDBResult(DBResult: PTDBResult);
  function MakeNewHero(PlayObject: TPlayObject; HeroData: PTHeroData): THeroObject;
    function GetNextDirectionForDirection(btDirection: Byte): Byte;
    begin
      Result := DR_UP;
      case btDirection of
        DR_UP:
          Result := DR_UPRIGHT;
        DR_DOWN:
          Result := DR_DOWNLEFT;
        DR_LEFT:
          Result := DR_UPLEFT;
        DR_RIGHT:
          Result := DR_DOWNRIGHT;
        DR_UPLEFT:
          Result := DR_UP;
        DR_UPRIGHT:
          Result := DR_RIGHT;
        DR_DOWNLEFT:
          Result := DR_LEFT;
        DR_DOWNRIGHT:
          Result := DR_DOWN;
      end;
    end;

  var
    HeroObject: THeroObject;
    p28: TGameObject;
    Abil: pTAbility;
    n20, n24, n1C: Integer;
    I: Integer;
    bo10: Boolean;
    btDirection: Byte;
  begin
    Result := nil;
    HeroObject := nil;
    if PlayObject.m_MyHero <> nil then
      Exit;

    HeroObject := THeroObject.Create;
    HeroObject.m_sMasterName := PlayObject.m_sCharName;
    HeroObject.m_boDummyObject := PlayObject.m_boDummyObject;
    HeroObject.m_nSessionID := PlayObject.m_nSessionID;
    if not HeroObject.m_boDummyObject then
    begin
      GetHeroData(HeroObject, HeroData);
    end
    else
    begin
      HeroObject.m_sUserID := PlayObject.m_sUserID;
      HeroObject.m_sCharName := PlayObject.m_sHeroName;
    end;

    if HeroObject.m_sMapName = '' then
    begin
      HeroObject.m_sHomeMap := PlayObject.m_sHomeMap;
      HeroObject.m_sMapName := PlayObject.m_sMapName;
      HeroObject.m_PEnvir := PlayObject.m_PEnvir;
      bo10 := False;
      n20 := 0;
      n24 := 0;
      PlayObject.GetBackPosition(n20, n24);
      if PlayObject.m_PEnvir.CanWalk(n20, n24, True) then
      begin
        bo10 := True;
        HeroObject.m_nCurrX := n20;
        HeroObject.m_nCurrY := n24;
      end;

      if not bo10 then
      begin
        btDirection := GetNextDirectionForDirection(PlayObject.m_btDirection);
        for I := 0 to 7 do
        begin
          PlayObject.GetBackPosition(btDirection, n20, n24);
          if PlayObject.m_PEnvir.CanWalk(n20, n24, True) then
          begin
            bo10 := True;
            HeroObject.m_nCurrX := n20;
            HeroObject.m_nCurrY := n24;
            Break;
          end;
          btDirection := GetNextDirectionForDirection(btDirection);
        end;

        if not bo10 then
        begin
          n1C := 0;
          while (True) do
          begin
            if PlayObject.m_PEnvir.CanWalk(HeroObject.m_nCurrX, HeroObject.m_nCurrY, True) then
              Break;
            HeroObject.m_nCurrX := HeroObject.m_nCurrX - 2 + Random(4);
            HeroObject.m_nCurrY := HeroObject.m_nCurrY - 2 + Random(4);
            Inc(n1C);
            if n1C >= 5 then
              Break;
          end;
        end;
      end;

      HeroObject.m_btDirection := Random(8);
      // if HeroObject.m_Abil.Level >= 0 then begin
      Abil := @HeroObject.m_Abil;
      { TODO -ochongchong -c添加 : 英雄出生等级 默认为主将英雄【2013-08-11】 }
      Abil.Level := g_Config.dwHeroMasterStartLevel;
      // Abil.Level := 1;
      Abil.AC1 := 0;
      Abil.AC2 := 0;
      Abil.MAC1 := 0;
      Abil.MAC2 := 0;
      Abil.DC1 := 1;
      Abil.DC2 := 2;
      Abil.MC1 := 1;
      Abil.MC2 := 2;
      Abil.SC1 := 1;
      Abil.SC2 := 2;
      Abil.MP := 15;
      Abil.HP := 15;
      Abil.MaxMP := 15;
      Abil.Exp := 10;
      Abil.MaxExp := 100;
      Abil.Weight := 100;
      Abil.MaxWeight := 100;
      HeroObject.m_boNewHero := not HeroObject.m_boDummyObject;
      // end;
    end;

    if HeroObject.m_Abil.HP <= 0 then
    begin
      HeroObject.m_Abil.HP := 14;
      FillChar(HeroObject.m_wStatusTimeArr, SizeOf(TStatusTime), #0);
    end;

    if HeroObject.m_PEnvir = nil then
    begin
      // HeroObject.m_PEnvir := g_MapManager.FindMap(HeroObject.m_sMapName);
      // if HeroObject.m_PEnvir = nil then begin
      HeroObject.m_sHomeMap := PlayObject.m_sHomeMap;
      HeroObject.m_sMapName := PlayObject.m_sMapName;
      // end;
    end;

    HeroObject.m_PEnvir := PlayObject.m_PEnvir;
    bo10 := False;
    n20 := 0;
    n24 := 0;
    PlayObject.GetBackPosition(n20, n24);
    if PlayObject.m_PEnvir.CanWalk(n20, n24, True) then
    begin
      bo10 := True;
      HeroObject.m_nCurrX := n20;
      HeroObject.m_nCurrY := n24;
    end;

    if not bo10 then
    begin
      btDirection := GetNextDirectionForDirection(PlayObject.m_btDirection);
      for I := 0 to 7 do
      begin
        PlayObject.GetBackPosition(btDirection, n20, n24);
        if PlayObject.m_PEnvir.CanWalk(n20, n24, True) then
        begin
          bo10 := True;
          HeroObject.m_nCurrX := n20;
          HeroObject.m_nCurrY := n24;
          Break;
        end;
        btDirection := GetNextDirectionForDirection(btDirection);
      end;

      if not bo10 then
      begin
        n1C := 0;
        while (True) do
        begin
          if PlayObject.m_PEnvir.CanWalk(HeroObject.m_nCurrX, HeroObject.m_nCurrY, True) then
            Break;
          HeroObject.m_nCurrX := HeroObject.m_nCurrX - 2 + Random(4);
          HeroObject.m_nCurrY := HeroObject.m_nCurrY - 2 + Random(4);
          Inc(n1C);
          if n1C >= 5 then
            Break;
        end;
      end;
    end;

    HeroObject.m_WAbil := HeroObject.m_Abil;
    HeroObject.Initialize();
    if HeroObject.m_boAddtoMapFail then
    begin
      if HeroObject.m_PEnvir.m_nWidth < 50 then
        n20 := 2
      else
        n20 := 3;
      if (HeroObject.m_PEnvir.m_nHeight < 250) then
      begin
        if (HeroObject.m_PEnvir.m_nHeight < 30) then
          n24 := 2
        else
          n24 := 20;
      end
      else
        n24 := 50;

      n1C := 0;
      while (True) do
      begin
        if not HeroObject.m_PEnvir.CanWalk(HeroObject.m_nCurrX, HeroObject.m_nCurrY, False) then
        begin
          if (HeroObject.m_PEnvir.m_nWidth - n24 - 1) > HeroObject.m_nCurrX then
          begin
            Inc(HeroObject.m_nCurrX, n20);
          end
          else
          begin
            HeroObject.m_nCurrX := Random(HeroObject.m_PEnvir.m_nWidth div 2) + n24;
            if HeroObject.m_PEnvir.m_nHeight - n24 - 1 > HeroObject.m_nCurrY then
              Inc(HeroObject.m_nCurrY, n20)
            else
              HeroObject.m_nCurrY := Random(HeroObject.m_PEnvir.m_nHeight div 2) + n24;
          end;
        end
        else
        begin
          p28 := HeroObject.m_PEnvir.AddToMap(HeroObject.m_nCurrX, HeroObject.m_nCurrY, HeroObject);
          if p28 <> HeroObject then
            FreeAndNil(HeroObject);

          Break;
        end;

        Inc(n1C);
        if n1C >= 31 then
          Break;
      end;
    end;
    {
      // 假人英雄出生满血2017-04-30
      if PlayObject.m_boDummyObject then
      begin
      HeroObject.m_Abil.HP := HeroObject.m_Abil.MaxHP;
      HeroObject.m_Abil.MP := HeroObject.m_Abil.MaxMP;
      HeroObject.m_WAbil.HP := HeroObject.m_WAbil.MaxHP;
      HeroObject.m_WAbil.MP := HeroObject.m_WAbil.MaxMP;
      end;
    }
    Result := HeroObject;
  end;
  function IsLogined(sAccount, sChrName: string): Boolean;
  var
    I: Integer;
  begin
    Result := False;
    if DataEngine.CheckHeroInSaveList(sAccount, sChrName) then
    begin
      Result := True;
    end
    else
    begin
      m_HeroObjectList.LockR(1);
      try
        for I := 0 to m_HeroObjectList.Count - 1 do
        begin
          if (CompareText(THeroObject(m_HeroObjectList.Objects[I]).m_sUserID, sAccount) = 0) and
            (CompareText(m_HeroObjectList.Strings[I], sChrName) = 0) then
          begin
            Result := True;
            Break;
          end;
        end;
      finally
        m_HeroObjectList.UnLockR;
      end;
    end;
  end;

var
  LoadHero: PTDBLoadHero;
  PlayObject: TPlayObject;
  HeroObject: THeroObject;
begin
  if DBResult.ResultType <> drtLoadHero then
    Exit;

  if DBResult.LoadBuf = nil then
    Exit;

  LoadHero := PTDBLoadHero(DBResult.LoadBuf);
  PlayObject := GetPlayObject(TObject(LoadHero.PlayObject));
  if (PlayObject = nil) or (PlayObject.m_boGhost) then
    Exit;

  if (not PlayObject.m_boDummyObject) and IsLogined(LoadHero.sAccount, LoadHero.sHeroName1) then
    Exit;

  PlayObject.m_boWaitHeroDate := False;
  case LoadHero.DataType of
    dt_Load:
      begin
        if DBResult.nResult = 0 then
        begin
          if not PlayObject.m_boDummyObject then
          begin
            if (DBResult.DataBuf <> nil) and (DBResult.DataBufLen = SizeOf(THeroData)) then
            begin
              HeroObject := MakeNewHero(PlayObject, PTHeroData(DBResult.DataBuf));
              if HeroObject <> nil then
              begin
                HeroObject.m_boIsDeputy := False;
                PlayObject.m_MyHero := HeroObject;
                HeroObject.m_Master := PlayObject;
                HeroObject.LogOn;
                m_HeroObjectList.LockW(2);
                try
                  m_HeroObjectList.AddObject(LoadHero.sHeroName1, HeroObject);
                finally
                  m_HeroObjectList.UnLockW;
                end;
              end;
            end;
          end
          else
          begin
            HeroObject := MakeNewHero(PlayObject, nil);
            if HeroObject <> nil then
            begin
              HeroObject.m_boIsDeputy := False;
              PlayObject.m_MyHero := HeroObject;
              HeroObject.m_Master := PlayObject;
              HeroObject.LogOn;
              m_HeroObjectList.LockW(2);
              try
                m_HeroObjectList.AddObject(LoadHero.sHeroName1, HeroObject);
              finally
                m_HeroObjectList.UnLockW;
              end;
            end;
          end;
        end;
      end;
    dt_LoadDeputyHero: // 读取副将数据
      begin
        if DBResult.nResult = 0 then // 读取成功
        begin
          if not PlayObject.m_boDummyObject then
          begin
            if (DBResult.DataBuf <> nil) and (DBResult.DataBufLen = SizeOf(THeroData)) then
            begin
              if LoadHero.btJob <= 2 then
                PTHeroData(DBResult.DataBuf).btJob := LoadHero.btJob;

              HeroObject := MakeNewHero(PlayObject, PTHeroData(DBResult.DataBuf));
              if HeroObject <> nil then
              begin
                HeroObject.m_boIsDeputy := True;
                PlayObject.m_MyHero := HeroObject;
                PlayObject.m_btDeputyHeroJob := HeroObject.m_btJob;
                HeroObject.m_Master := PlayObject;
                HeroObject.LogOn;
                m_HeroObjectList.LockW(3);
                try
                  m_HeroObjectList.AddObject(LoadHero.sHeroName1, HeroObject);
                finally
                  m_HeroObjectList.UnLockW;
                end;
              end;
            end;
          end
          else
          begin
            HeroObject := MakeNewHero(PlayObject, nil);
            if HeroObject <> nil then
            begin
              HeroObject.m_boIsDeputy := True;
              PlayObject.m_MyHero := HeroObject;
              PlayObject.m_btDeputyHeroJob := HeroObject.m_btJob;
              HeroObject.m_Master := PlayObject;
              HeroObject.LogOn;
              m_HeroObjectList.LockW(3);
              try
                m_HeroObjectList.AddObject(LoadHero.sHeroName1, HeroObject);
              finally
                m_HeroObjectList.UnLockW;
              end;
            end;
          end;
        end;
      end;
    dt_Create:
      begin
        case DBResult.nResult of
          0:
            begin
              PlayObject.m_sHeroName := PlayObject.m_sTempHeroName;
              PlayObject.m_sTempHeroName := '';
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@CreateHeroOK', False);
              end;
            end;
          2:
            begin
              PlayObject.m_sTempHeroName := '';
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@HeroNameFilter', False);
              end;
            end;
          3:
            begin
              PlayObject.m_sTempHeroName := '';
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@HeroNameExists', False);
              end;
            end;
        else
          PlayObject.m_sTempHeroName := '';
          if LoadHero.NPC <> 0 then
          begin
            PlayObject.m_nScriptGotoCount := 0;
            g_FunctionNPC.GotoLable(PlayObject, '@CreateHeroFail', False);
          end;
        end;
      end;
    dt_Delete:
      begin
        case DBResult.nResult of
          0:
            begin
              PlayObject.m_sHeroName := '';
              PlayObject.m_boFixedHero := False;
              PlayObject.m_boStorageHero := False;
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@DeleteHeroOK', False);
              end;
            end;
          1:
            begin
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@DeleteHeroFail', False);
              end;
            end;
        end;
      end;

    dt_CreateDeputyHero:
      begin
        // MainOutMessage(Format('dt_CreateDeputyHero DBHeroOtherResult.nResult:%d', [DBHeroOtherResult.nResult]));
        case DBResult.nResult of
          0:
            begin
              PlayObject.m_sDeputyHeroName := PlayObject.m_sTempHeroName;
              PlayObject.m_btDeputyHeroJob := LoadHero.btJob;
              PlayObject.m_sTempHeroName := '';
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@CreateHeroOK', False);
              end;
            end;
          2:
            begin
              PlayObject.m_sTempHeroName := '';
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@HeroNameFilter', False);
              end;
            end;
          3:
            begin
              PlayObject.m_sTempHeroName := '';
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@HeroNameExists', False);
              end;
            end;
        else
          PlayObject.m_sTempHeroName := '';
          if g_FunctionNPC <> nil then
          begin
            PlayObject.m_nScriptGotoCount := 0;
            g_FunctionNPC.GotoLable(PlayObject, '@CreateHeroFailEx', False);
          end;
        end;
      end;
    dt_DeleteDeputyHero:
      begin
        // MainOutMessage(Format('dt_DeleteDeputyHero DBHeroOtherResult.nResult:%d', [DBHeroOtherResult.nResult]));
        case DBResult.nResult of
          0:
            begin
              PlayObject.m_sDeputyHeroName := '';
              PlayObject.m_boFixedHero := False;
              PlayObject.m_boStorageDeputyHero := False;
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@DeleteHeroOK', False);
              end;
            end;
          1:
            begin
              if g_FunctionNPC <> nil then
              begin
                PlayObject.m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(PlayObject, '@DeleteHeroFail', False);
              end;
            end;
        end;
      end;
    dt_QueryStorageHeroInfo:
      begin // 查询寄存的英雄   召回寄存英雄
        case DBResult.nResult of
          0:
            begin
              PlayObject.SendMsg(PlayObject, RM_SENDSTORAGEHEROINFO, 0, NativeInt(PlayObject), 0, 0, DBResult.sResult);
            end;
          1:
            begin // 读取成功
              if g_FunctionNPC <> nil then
                g_FunctionNPC.GotoLable(PlayObject, '@QueryHeroInfoFail', False);
            end;
        end;
      end;
    dt_QueryAssessHeroInfo: // 主副将英雄评定查询
      begin
        case DBResult.nResult of
          0:
            begin
              PlayObject.SendMsg(PlayObject, RM_SENDSTORAGEHEROINFOEX, 0, NativeInt(PlayObject), 0, 0, DBResult.sResult);
            end;
          1:
            begin
              if g_FunctionNPC <> nil then
                g_FunctionNPC.GotoLable(PlayObject, '@QueryHeroInfoFail', False);
            end;
        end;
      end;
    dt_AssessHero:
      begin
        case DBResult.nResult of
          0:
            begin
              PlayObject.m_sHeroName := LoadHero.sHeroName1;
              PlayObject.m_sDeputyHeroName := LoadHero.sHeroName2;
              PlayObject.m_boFixedHero := True;
              PlayObject.m_boStorageHero := False;
              PlayObject.m_boStorageDeputyHero := False;
              PlayObject.SendDefMessage(SM_ASSESSMENTHERO_OK, 0, 0, 0, 0, ''); // 评定成功
            end;
        else
          PlayObject.SendDefMessage(SM_ASSESSMENTHERO_FAIL, 0, 0, 0, 0, '');
        end;
      end;
  end;
end;

procedure TUserEngine.ProcessDBRenameResult(DBResult: PTDBResult);
var
  DBRenameChr: PTDBRenameChr;
  PlayObject: TPlayObject;
  sFile: string;
  I, II: Integer;
  sLine, S1, S2: string;
  Human: TPlayObject;
  SL: TStringList;
  MasterRankInfo: pTMasterRankInfo;
  GuildRank: pTGuildRank;
  DealingInfo: pTDealingInfo;
  TempSellPlayerInfo: TSellPlayerInfo;
  SellPlayerInfo: PSellPlayerInfo;
  IsChange: Boolean;
begin
  if DBResult.LoadBuf = nil then
    Exit;
  DBRenameChr := PTDBRenameChr(DBResult.LoadBuf);
  PlayObject := GetPlayObject(TObject(DBRenameChr.PlayObject));
  if (PlayObject = nil) or (PlayObject.m_boGhost) then
    Exit;
  if DBRenameChr.boHuman then
  begin
    if DBResult.nResult = 0 then
    begin
      if g_FunctionNPC <> nil then
      begin
        PlayObject.m_nScriptGotoCount := 0;
        PlayObject.m_sCharNewName := DBRenameChr.sNewName;
        g_FunctionNPC.GotoLable(PlayObject, '@ChangeHumNameOK', False);
        PlayObject.m_sCharNewName := '';
      end;
      // 个人商店中用户改名
      g_M2DataDB.UserShopDB.HumanRename(DBRenameChr.sOldName, DBRenameChr.sNewName);
      // 可视仓库
      g_M2DataDB.StorageDB.RenameHumanName(DBRenameChr.sOldName, DBRenameChr.sNewName);
      // 拍卖行中用户改名
      g_M2DataDB.AuctionDB.HumanRename(DBRenameChr.sOldName, DBRenameChr.sNewName);
      PlayObject.m_sCharName := DBRenameChr.sNewName;
      PlayObject.RefShowName();
      PlayObject.SendDefMessage(SM_CHANGESELNAME, NativeInt(PlayObject), 0, 0, 0, DBRenameChr.sNewName);
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        UserEngine.m_PlayObjectList.LockR(90);
      try
{$IFEND}
        II := UserEngine.m_PlayObjectList.IndexOf(DBRenameChr.sOldName);
        if II >= 0 then
          UserEngine.m_PlayObjectList.Strings[II] := DBRenameChr.sNewName;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          UserEngine.m_PlayObjectList.UnLockR;
      end;
{$IFEND}
      // 夫妻
      if PlayObject.m_sDearName <> '' then
      begin
        Human := UserEngine.GetPlayObject(PlayObject.m_sDearName);
        if Human <> nil then
        begin
          Human.m_sDearName := DBRenameChr.sNewName;
          Human.RefShowName;
        end;
      end;

      // 元宝交易
      II := g_GameGoldDealDB.m_SellList.IndexOf(DBRenameChr.sOldName);
      if (II >= 0) then
      begin
        g_GameGoldDealDB.m_SellList.Strings[II] := DBRenameChr.sNewName;
        DealingInfo := pTDealingInfo(g_GameGoldDealDB.m_SellList.Objects[II]);
        DealingInfo.Dealing.SellChrName := DBRenameChr.sNewName;
        g_GameGoldDealDB.m_FileStream.Position := DealingInfo.OffSet;
        g_GameGoldDealDB.m_FileStream.Write(DealingInfo.Dealing, SizeOf(TGameGoldDeal));
        g_GameGoldDealDB.m_SellList.Sort;
      end;

      II := g_GameGoldDealDB.m_BuyList.IndexOf(DBRenameChr.sOldName);
      if (II >= 0) then
      begin
        g_GameGoldDealDB.m_BuyList.Strings[II] := DBRenameChr.sNewName;
        DealingInfo := pTDealingInfo(g_GameGoldDealDB.m_BuyList.Objects[II]);
        DealingInfo.Dealing.BuyChrName := DBRenameChr.sNewName;
        g_GameGoldDealDB.m_FileStream.Position := DealingInfo.OffSet;
        g_GameGoldDealDB.m_FileStream.Write(DealingInfo.Dealing, SizeOf(TGameGoldDeal));
        g_GameGoldDealDB.m_BuyList.Sort;
      end;

      // 如果是师傅 则将徒弟列表中的师傅改成新名字
      if PlayObject.m_boMaster then
      begin
        sFile := g_Config.sEnvirDir + 'MasterNo\' + DBRenameChr.sOldName + '.txt';
        DeleteFile(sFile);
        for I := 0 to PlayObject.m_MasterList.Count - 1 do
        begin
          if (PlayObject.m_MasterList.Items[I] <> nil) then
          begin
            TPlayObject(PlayObject.m_MasterList.Items[I]).m_Master := PlayObject;
            TPlayObject(PlayObject.m_MasterList.Items[I]).m_sMasterName := DBRenameChr.sNewName;
          end;
        end;
        PlayObject.SaveMasterNoList;
      end
      else if PlayObject.m_sMasterName <> '' then // 如果是徒弟
      begin
        // 师傅在线
        Human := UserEngine.GetPlayObject(PlayObject.m_sMasterName);
        if (Human <> nil) then
        begin
          for I := 0 to Human.m_MasterNoList.Count - 1 do
          begin
            MasterRankInfo := Human.m_MasterNoList.Items[I];
            if SameText(MasterRankInfo.sChrName, DBRenameChr.sOldName) then
            begin
              MasterRankInfo.sChrName := DBRenameChr.sNewName;
              Break;
            end;
          end;
          Human.SaveMasterNoList;
        end
        else // 师傅不在线
        begin
          sFile := g_Config.sEnvirDir + 'MasterNo\' + PlayObject.m_sMasterName + '.txt';
          if FileExists(sFile) then
          begin
            SL := TStringList.Create;
            try
              SL.LoadFromFile(sFile);
              for I := 0 to SL.Count - 1 do
              begin
                sLine := Trim(SL.Strings[I]);
                if (Length(sLine) > 0) and (sLine[1] <> ';') then
                begin
                  S2 := GetValidStr3(sLine, S1, [' ', #9]); // 徒弟名
                  if SameText(S1, DBRenameChr.sOldName) then
                  begin
                    sLine := DBRenameChr.sNewName + ' ' + S2;
                    SL.Strings[I] := sLine;
                    SL.SaveToFile(sFile);
                    Break;
                  end;
                end;
              end;
            finally
              SL.Free;
            end;
          end;
        end;
      end;

      // 行会
      if PlayObject.m_MyGuild <> nil then
      begin
        for I := 0 to TGuild(PlayObject.m_MyGuild).m_RankList.Count - 1 do
        begin
          GuildRank := TGuild(PlayObject.m_MyGuild).m_RankList.Items[I];
          for II := 0 to GuildRank.MemberList.Count - 1 do
          begin
            if TPlayObject(GuildRank.MemberList.Objects[II]) = PlayObject then
            begin
              GuildRank.MemberList.Strings[II] := DBRenameChr.sNewName;
              TGuild(PlayObject.m_MyGuild).UpdateGuildFile;
              Break;
            end;
          end;
        end;
      end;

      // 角色出售 2020-02-23 16:21:42
      IsChange := False;
      if g_SellPlayerList.Search(DBRenameChr.sOldName, I) then
      begin
        TempSellPlayerInfo := g_SellPlayerList.Items[I]^;
        g_SellPlayerList.DeleteByIndex(I);
        g_SellPlayerList.AddSellPlayer(TempSellPlayerInfo.Account, TempSellPlayerInfo.Player, TempSellPlayerInfo.Delegater,
          TempSellPlayerInfo.SellPricesType, TempSellPlayerInfo.SellPrices, TempSellPlayerInfo.SellTime,
          TempSellPlayerInfo.SetUser, TempSellPlayerInfo.SetUserName);
        IsChange := True;
      end;

      for I := 0 to g_SellPlayerList.Count - 1 do
      begin
        SellPlayerInfo := g_SellPlayerList.Items[I];
        if SameText(SellPlayerInfo.Delegater, DBRenameChr.sOldName) then
        begin
          SellPlayerInfo.Delegater := DBRenameChr.sNewName;
          IsChange := True;
        end;

        if SellPlayerInfo.SetUser and SameText(DBRenameChr.sOldName, SellPlayerInfo.SetUserName) then
        begin
          SellPlayerInfo.SetUserName := DBRenameChr.sNewName;
          IsChange := True;
        end;
      end;

      if IsChange then
        g_SellPlayerList.SaveConfig;
    end
    else if DBResult.nResult = 2 then
    begin
      PlayObject.m_nScriptGotoCount := 0;
      if g_FunctionNPC <> nil then
        g_FunctionNPC.GotoLable(PlayObject, '@HumNameFilter', False);
    end
    else if DBResult.nResult = 3 then
    begin
      PlayObject.m_nScriptGotoCount := 0;
      if g_FunctionNPC <> nil then
        g_FunctionNPC.GotoLable(PlayObject, '@HumNameExists', False);
    end
    else
    begin
      PlayObject.m_nScriptGotoCount := 0;
      if g_FunctionNPC <> nil then
        g_FunctionNPC.GotoLable(PlayObject, '@ChangeHumNameFail', False);
    end;
  end
  // 英雄改名
  else
  begin
    if DBResult.nResult = 0 then
    begin
      PlayObject.m_sCharNewName := DBRenameChr.sNewName;
      PlayObject.m_nScriptGotoCount := 0;
      if g_FunctionNPC <> nil then
      begin
        g_FunctionNPC.GotoLable(PlayObject, '@ChangeHeroNameOK', False);
      end;
      PlayObject.m_sCharNewName := '';
      if PlayObject.m_MyHero <> nil then
      begin
        THeroObject(PlayObject.m_MyHero).m_sCharName := DBRenameChr.sNewName;
        if THeroObject(PlayObject.m_MyHero).m_boIsDeputy then
          PlayObject.m_sDeputyHeroName := DBRenameChr.sNewName
        else
          PlayObject.m_sHeroName := DBRenameChr.sNewName;
        THeroObject(PlayObject.m_MyHero).RefShowName();
      end
      else
      begin
        if SameText(PlayObject.m_sHeroName, DBRenameChr.sOldName) then
          PlayObject.m_sHeroName := DBRenameChr.sNewName
        else if SameText(PlayObject.m_sDeputyHeroName, DBRenameChr.sOldName) then
          PlayObject.m_sDeputyHeroName := DBRenameChr.sNewName;
      end;
    end
    else if DBResult.nResult = 2 then
    begin
      PlayObject.m_nScriptGotoCount := 0;
      if g_FunctionNPC <> nil then
      begin
        g_FunctionNPC.GotoLable(PlayObject, '@HeroNameFilter', False);
      end;
    end
    else if DBResult.nResult = 3 then
    begin
      PlayObject.m_nScriptGotoCount := 0;
      if g_FunctionNPC <> nil then
      begin
        g_FunctionNPC.GotoLable(PlayObject, '@HeroNameExists', False);
      end;
    end
    else
    begin
      PlayObject.m_nScriptGotoCount := 0;
      if g_FunctionNPC <> nil then
      begin
        g_FunctionNPC.GotoLable(PlayObject, '@ChangeHeroNameFail', False);
      end;
    end;
  end;
end;

procedure TUserEngine.ProcessDBQueryHumanInfoResult(DBResult: PTDBResult);
var
  QueryInfo: PTDBQueryHumanInfo;
  PlayObject: TPlayObject;
  MemberInfo: TDBGuildMemberInfo;
  SendMemberInfo: TGuildMemeberInfo;
  Master1, Master2: string;
  sUserName: AnsiString;
  I, J, nCode, nRankNo: Integer;
  DefMsg: TDefaultMessage;
  OutBuf: array [0 .. 10240 - 1] of AnsiChar;
  PMemberInfo: PDBGuildMemberInfo;
  GuildJoinUser: PGuildJoinUser;
begin
  if DBResult.LoadBuf = nil then
    Exit;
  QueryInfo := PTDBQueryHumanInfo(DBResult.LoadBuf);
  PlayObject := GetPlayObject(TObject(QueryInfo.PlayObject));
  if (PlayObject = nil) or (PlayObject.m_boGhost) then
    Exit;
  case QueryInfo.QueryType of
    qhiGuildMemberInfo:
      begin
        if QueryInfo.nResultCount = 1 then
        begin
          DecodeString(DBResult.sResult, PAnsiChar(@MemberInfo), SizeOf(MemberInfo));
          sUserName := PAnsiChar(@QueryInfo.HumanList[0])^;
          nCode := 0;
          if ((PlayObject.m_MyGuild = nil)) or (PlayObject.m_nGuildRankNo <> 1) then
          begin
            nCode := 1;
            DefMsg := MakeDefaultMsg(SM_GUILDVIEWMEMBERINFO_RET, nCode, 0, 0, 0);
            PlayObject.SendSocket(@DefMsg, '');
          end
          else
          begin
            TGuild(PlayObject.m_MyGuild).GetGuildMasterName(Master1, Master2);
            SendMemberInfo.sName := MemberInfo.sName;
            SendMemberInfo.boOnline := False;
            SendMemberInfo.btSex := MemberInfo.btSex;
            SendMemberInfo.btJob := MemberInfo.btJob;
            SendMemberInfo.nLevel := MemberInfo.nLevel;
            SendMemberInfo.dtLastLogin := MemberInfo.dtLastLogin;
            SendMemberInfo.boMaster := SameText(Master1, sUserName) or SameText(Master2, sUserName);
            SendMemberInfo.nRankName := TGuild(PlayObject.m_MyGuild).GetRankName(sUserName, nRankNo);
            DefMsg := MakeDefaultMsg(SM_GUILDVIEWMEMBERINFO_RET, nCode, SizeOf(TGuildMemeberInfo), 0, 0);
            PlayObject.SendSocketEx(@DefMsg, @SendMemberInfo, SizeOf(TGuildMemeberInfo));
          end;
        end;
      end;
    qhiGuildJoinUserList:
      begin
        DecodeString(DBResult.sResult, OutBuf, SizeOf(OutBuf));
        PMemberInfo := @OutBuf;
        for I := 0 to QueryInfo.nResultCount - 1 do
        begin
          for J := 0 to PlayObject.m_SendJoinUserList.Count - 1 do
          begin
            GuildJoinUser := PlayObject.m_SendJoinUserList.Items[J];
            if SameText(PMemberInfo.sName, GuildJoinUser.sUserName) then
            begin
              GuildJoinUser.btSex := PMemberInfo.btSex;
              GuildJoinUser.btJob := PMemberInfo.btJob;
              GuildJoinUser.nLevel := PMemberInfo.nLevel;
              GuildJoinUser.dtLastLogin := PMemberInfo.dtLastLogin;
              Break;
            end;
          end;
          Inc(PMemberInfo);
        end;
        PlayObject.SendJoinUserListToClient;
      end;
  end;
end;

procedure TUserEngine.ProcessDBGetRankDataResult(DBResult: PTDBResult);
var
  RankData: PTDBGetRankData;
  PlayObject: TPlayObject;
  DefMsg: TDefaultMessage;
begin
  if DBResult.LoadBuf = nil then
    Exit;
  RankData := PTDBGetRankData(DBResult.LoadBuf);
  PlayObject := GetPlayObject(TObject(RankData.PlayObject));
  if (PlayObject = nil) or (PlayObject.m_boGhost) then
    Exit;
  if Length(DBResult.sResult) = 0 then
  begin
    if Length(RankData.sChrName) = 0 then
    begin
      DefMsg := MakeDefaultMsg(SM_SENGRANKING, RankData.nTabelType, RankData.nTabelPage, RankData.nPage, 0);
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_SENGMYRANKING_FAIL, 0, 0, 0, 0);
    end;
    PlayObject.SendSocket(@DefMsg, '')
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SM_SENGRANKING, RankData.nTabelType, RankData.nTabelPage, RankData.nPage, DBResult.nResult);
    PlayObject.SendSocket(@DefMsg, DBResult.sResult);
  end;
end;

procedure TUserEngine.ProcessDBBuyPlayerResult(DBResult: PTDBResult);
// 还原购买角色失败的在线玩家数据
  procedure IncBuyerGameMoney(Buyer: TPlayObject; BuyPlayerInfo: PTDBBuyPlayerInfo); overload;
  var
    nIndex: Integer;
  begin
    case BuyPlayerInfo.SellPricesType of
      0:
        begin
          Buyer.IncGameGold(BuyPlayerInfo.SellPrices);
          Buyer.GameGoldChanged;
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GameGoldChange, Buyer, g_Config.sGameGoldName, 0, BuyPlayerInfo.sSellPlayer,
              Buyer.m_nGameGold, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
          end;
        end;
      1:
        begin
          Buyer.IncGamePoint(BuyPlayerInfo.SellPrices);
          Buyer.GameGoldChanged;
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GamePointChange, Buyer, g_Config.sGamePointName, 0, BuyPlayerInfo.sSellPlayer,
              Buyer.m_nGamePoint, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
          end;
        end;
      2:
        begin
          Buyer.IncGold(BuyPlayerInfo.SellPrices);
          Buyer.GoldChanged;
          if g_boGameLogGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GoldChange, Buyer, sSTRING_GOLDNAME, 0, BuyPlayerInfo.sSellPlayer,
              Buyer.m_nGold, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
          end;
        end;
      3:
        begin
          Buyer.IncGameDiamond(BuyPlayerInfo.SellPrices);
          Buyer.NewGamePointChanged;
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GameDiamondChange, Buyer, g_Config.sGameDiamondName, 0,
              BuyPlayerInfo.sSellPlayer, Buyer.m_nGameDiamond, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
          end;
        end;
      4:
        begin
          Buyer.IncGameGird(BuyPlayerInfo.SellPrices);
          Buyer.NewGamePointChanged;
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GameGirdChange, Buyer, g_Config.sGameGirdName, 0, BuyPlayerInfo.sSellPlayer,
              Buyer.m_nGameGird, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
          end;
        end;
    else
      Buyer.IncMoney(BuyPlayerInfo.SellPricesType - 5, BuyPlayerInfo.SellPrices, nIndex);
    end;
  end;
// 还原购买角色失败的不在线玩家数据
  procedure IncBuyerGameMoney(BuyPlayerInfo: PTDBBuyPlayerInfo); overload;
  var
    GoldType: TDBChangeGoldType;
  begin
    case BuyPlayerInfo.SellPricesType of
      0:
        GoldType := cgtGameGold;
      1:
        GoldType := cgtGamePoint;
      2:
        GoldType := cgtGold;
      3:
        GoldType := cgtGameDiamond;
      4:
        GoldType := cgtGameGird;
    else
      GoldType := cgtCustomMoney;
    end;
    // 玩家离线了自定义货币的处理 最后制作 By 一支笔 at:2022-03-10 21:42:16
    DataEngine.HumanChangeGold(nil, nil, GoldType, BuyPlayerInfo.sBuyPlayer, BuyPlayerInfo.sBuyPlayer, BuyPlayerInfo.SellPrices,
      GetCustomMoneyNameByRule(BuyPlayerInfo.SellPricesType, 3));
    case BuyPlayerInfo.SellPricesType of
      0:
        begin
          if g_boGameLogGameGold then
            AddGameDataLog(LOG_PlayerTrading, LOG_GameGoldChange, latHuman, '0', 0, 0, g_Config.sGameGoldName, 0,
              BuyPlayerInfo.sBuyPlayer, BuyPlayerInfo.sSellPlayer, 0, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
        end;
      1: // 排除
        begin
          if g_boGameLogGameGold then
            AddGameDataLog(LOG_PlayerTrading, LOG_GamePointChange, latHuman, '0', 0, 0, g_Config.sGamePointName, 0,
              BuyPlayerInfo.sBuyPlayer, BuyPlayerInfo.sSellPlayer, 0, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
        end;
      2:
        begin
          if g_boGameLogGold then
            AddGameDataLog(LOG_PlayerTrading, LOG_GoldChange, latHuman, '0', 0, 0, sSTRING_GOLDNAME, 0, BuyPlayerInfo.sBuyPlayer,
              BuyPlayerInfo.sSellPlayer, 0, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
        end;
      3:
        begin
          if g_boGameLogGameGold then
            AddGameDataLog(LOG_PlayerTrading, LOG_GameDiamondChange, latHuman, '0', 0, 0, g_Config.sGameDiamondName, 0,
              BuyPlayerInfo.sBuyPlayer, BuyPlayerInfo.sSellPlayer, 0, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
        end;
      4:
        begin
          if g_boGameLogGameGold then
            AddGameDataLog(LOG_PlayerTrading, LOG_GameGirdChange, latHuman, '0', 0, 0, g_Config.sGameGirdName, 0,
              BuyPlayerInfo.sBuyPlayer, BuyPlayerInfo.sSellPlayer, 0, BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
        end;
    else
      if IsLogCustomMoney(BuyPlayerInfo.SellPricesType - 5) then
      begin
        AddGameDataLog(LOG_PlayerTrading, LOG_CustomMoneyChange, latHuman, '0', 0, 0,
          GetCustomMoneyNameByRule(BuyPlayerInfo.SellPricesType, 3), 0, BuyPlayerInfo.sBuyPlayer, BuyPlayerInfo.sSellPlayer, 0,
          BuyPlayerInfo.SellPrices, '买入失败，还原扣款');
      end;
    end;
  end;
// 给在线的委托玩家增加货币
  procedure IncDelegaterMoney(Delegater: TPlayObject; BuyPlayerInfo: PTDBBuyPlayerInfo;
    var nTGameGoldCount, nTGamePointCount, nTGoldCount, nTGameDiamondCount, nTGameGirdCount,
    nTCustomMoneyCount: Integer); overload;
  var
    nIndex: Integer;
  begin
    case BuyPlayerInfo.SellPricesType of
      0:
        begin
          // 离线购买物品、上线后取款扣税 piaoyun 2013-07-21
          nTGameGoldCount := Trunc(g_Config.dwSellPlayerGameGoldTaxRate / 100 * BuyPlayerInfo.SellPrices);
          Delegater.IncGameGold(BuyPlayerInfo.SellPrices - nTGameGoldCount);
          Delegater.GameGoldChanged;
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GameGoldChange, Delegater, g_Config.sGameGoldName, 0, BuyPlayerInfo.sBuyPlayer,
              Delegater.m_nGameGold, BuyPlayerInfo.SellPrices - nTGameGoldCount, '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end;
      1:
        begin
          // OldValue := m_nGamePoint;
          nTGamePointCount := Trunc(g_Config.dwSellPlayerGamePointTaxRate / 100 * BuyPlayerInfo.SellPrices);
          Delegater.IncGamePoint(BuyPlayerInfo.SellPrices - nTGamePointCount);
          Delegater.GameGoldChanged;
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GamePointChange, Delegater, g_Config.sGamePointName, 0,
              BuyPlayerInfo.sBuyPlayer, Delegater.m_nGamePoint, BuyPlayerInfo.SellPrices - nTGamePointCount,
              '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end;
      2:
        begin
          // OldValue := m_nGold;
          nTGoldCount := Trunc(g_Config.dwSellPlayerGoldTaxRate / 100 * BuyPlayerInfo.SellPrices);
          Delegater.IncGold(BuyPlayerInfo.SellPrices - nTGoldCount);
          Delegater.GoldChanged;
          if g_boGameLogGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GoldChange, Delegater, sSTRING_GOLDNAME, 0, BuyPlayerInfo.sBuyPlayer,
              Delegater.m_nGold, BuyPlayerInfo.SellPrices - nTGoldCount, '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end;
      3:
        begin
          // OldValue := m_nGameDiamond;
          nTGameDiamondCount := Trunc(g_Config.dwSellPlayerGameDiamondTaxRate / 100 * BuyPlayerInfo.SellPrices);
          Delegater.IncGameDiamond(BuyPlayerInfo.SellPrices - nTGameDiamondCount);
          Delegater.NewGamePointChanged;
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GameDiamondChange, Delegater, g_Config.sGameDiamondName, 0,
              BuyPlayerInfo.sBuyPlayer, Delegater.m_nGameDiamond, BuyPlayerInfo.SellPrices - nTGameDiamondCount,
              '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end;
      4:
        begin
          // OldValue := m_nGameGird;
          nTGameGirdCount := Trunc(g_Config.dwSellPlayerGameGirdTaxRate / 100 * BuyPlayerInfo.SellPrices);
          Delegater.IncGameGird(BuyPlayerInfo.SellPrices - nTGameGirdCount);
          Delegater.NewGamePointChanged;
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GameGirdChange, Delegater, g_Config.sGameGirdName, 0, BuyPlayerInfo.sBuyPlayer,
              Delegater.m_nGameGird, BuyPlayerInfo.SellPrices - nTGameGirdCount, '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end;
    else
      nTCustomMoneyCount := 0;
      Delegater.IncMoney(BuyPlayerInfo.SellPricesType - 5, nTCustomMoneyCount - BuyPlayerInfo.SellPrices, nIndex);
      if IsLogCustomMoney(BuyPlayerInfo.SellPricesType - 5) then
      begin
        AddGameDataLog(LOG_PlayerTrading, LOG_CustomMoneyChange, Delegater, GetCustomMoneyNameByRule(BuyPlayerInfo.SellPricesType,
          3), 0, BuyPlayerInfo.sBuyPlayer, Delegater.GetMoney(BuyPlayerInfo.SellPricesType - 5),
          BuyPlayerInfo.SellPrices - nTCustomMoneyCount, '出售角色:' + BuyPlayerInfo.sSellPlayer);
      end;
    end;
  end;
// 给在线的委托玩家增加货币
  procedure IncDelegaterMoney(BuyPlayerInfo: PTDBBuyPlayerInfo); overload;
  var
    DecMoney: Integer;
  begin
    // 玩家离线了自定义货币的处理 最后制作 By 一支笔 at:2022-03-10 21:42:16
    case BuyPlayerInfo.SellPricesType of
      0:
        begin
          DecMoney := Trunc(g_Config.dwSellPlayerGameGoldTaxRate / 100 * BuyPlayerInfo.SellPrices);
          DataEngine.HumanChangeGold(nil, nil, cgtGameGold, BuyPlayerInfo.sDelegater, BuyPlayerInfo.sDelegater,
            BuyPlayerInfo.SellPrices - DecMoney);
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GameGoldChange, latHuman, '0', 0, 0, g_Config.sGameGoldName, 0,
              BuyPlayerInfo.sDelegater, BuyPlayerInfo.sBuyPlayer, 0, BuyPlayerInfo.SellPrices - DecMoney,
              '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end;
      1:
        begin
          // OldValue := m_nGamePoint;
          DecMoney := Trunc(g_Config.dwSellPlayerGamePointTaxRate / 100 * BuyPlayerInfo.SellPrices);
          DataEngine.HumanChangeGold(nil, nil, cgtGamePoint, BuyPlayerInfo.sDelegater, BuyPlayerInfo.sDelegater,
            BuyPlayerInfo.SellPrices - DecMoney);
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GamePointChange, latHuman, '0', 0, 0, g_Config.sGamePointName, 0,
              BuyPlayerInfo.sDelegater, BuyPlayerInfo.sBuyPlayer, 0, BuyPlayerInfo.SellPrices - DecMoney,
              '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end;
      2:
        begin
          DecMoney := Trunc(g_Config.dwSellPlayerGoldTaxRate / 100 * BuyPlayerInfo.SellPrices);
          DataEngine.HumanChangeGold(nil, nil, cgtGold, BuyPlayerInfo.sDelegater, BuyPlayerInfo.sDelegater,
            BuyPlayerInfo.SellPrices - DecMoney);
          if g_boGameLogGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GoldChange, latHuman, '0', 0, 0, sSTRING_GOLDNAME, 0, BuyPlayerInfo.sDelegater,
              BuyPlayerInfo.sBuyPlayer, 0, BuyPlayerInfo.SellPrices - DecMoney, '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end;
      3:
        begin
          DecMoney := Trunc(g_Config.dwSellPlayerGameDiamondTaxRate / 100 * BuyPlayerInfo.SellPrices);
          DataEngine.HumanChangeGold(nil, nil, cgtGameDiamond, BuyPlayerInfo.sDelegater, BuyPlayerInfo.sDelegater,
            BuyPlayerInfo.SellPrices - DecMoney);
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GameDiamondChange, latHuman, '0', 0, 0, g_Config.sGameDiamondName, 0,
              BuyPlayerInfo.sDelegater, BuyPlayerInfo.sBuyPlayer, 0, BuyPlayerInfo.SellPrices - DecMoney,
              '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end;
      4:
        begin
          // OldValue := m_nGameGird;
          DecMoney := Trunc(g_Config.dwSellPlayerGameGirdTaxRate / 100 * BuyPlayerInfo.SellPrices);
          DataEngine.HumanChangeGold(nil, nil, cgtGameGird, BuyPlayerInfo.sDelegater, BuyPlayerInfo.sDelegater,
            BuyPlayerInfo.SellPrices - DecMoney, GetCustomMoneyNameByRule(BuyPlayerInfo.SellPricesType, 3));
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_PlayerTrading, LOG_GameGirdChange, latHuman, '0', 0, 0, g_Config.sGameGirdName, 0,
              BuyPlayerInfo.sDelegater, BuyPlayerInfo.sBuyPlayer, 0, BuyPlayerInfo.SellPrices - DecMoney,
              '出售角色:' + BuyPlayerInfo.sSellPlayer);
          end;
        end
    else
      DecMoney := 0;
      DataEngine.HumanChangeGold(nil, nil, cgtCustomMoney, BuyPlayerInfo.sDelegater, BuyPlayerInfo.sDelegater,
        BuyPlayerInfo.SellPrices - DecMoney, GetCustomMoneyNameByRule(BuyPlayerInfo.SellPricesType, 3));
      if IsLogCustomMoney(BuyPlayerInfo.SellPricesType - 5) then
      begin
        AddGameDataLog(LOG_PlayerTrading, LOG_CustomMoneyChange, latHuman, '0', 0, 0,
          GetCustomMoneyNameByRule(BuyPlayerInfo.SellPricesType, 3), 0, BuyPlayerInfo.sDelegater, BuyPlayerInfo.sBuyPlayer, 0,
          BuyPlayerInfo.SellPrices - DecMoney, '出售角色:' + BuyPlayerInfo.sSellPlayer);
      end;
    end;
  end;

var
  BuyPlayerInfo: PTDBBuyPlayerInfo;
  SellPlayerInfo: PSellPlayerInfo;
  Index: Integer;
  Buyer, Seller, Delegater: TPlayObject;
  DefMsg: TDefaultMessage;
  nTGameGoldCount, nTGamePointCount, nTGoldCount, nTGameDiamondCount, nTGameGirdCount, nTCustomMoneyCount: Integer; // 税收
begin
  if DBResult.LoadBuf = nil then
    Exit;
  BuyPlayerInfo := PTDBBuyPlayerInfo(DBResult.LoadBuf);
  Buyer := GetPlayObject(TObject(BuyPlayerInfo.Buyer));
  if DBResult.nResult = 0 then
  begin
    if g_SellPlayerList.Search(BuyPlayerInfo.sSellPlayer, Index) then
    begin
      // SellPlayerInfo := g_SellPlayerList.Items[Index];
      g_SellPlayerList.DeleteByIndex(Index);
      g_SellPlayerList.SaveConfig;
    end;
    // 委托者收款
    Delegater := GetPlayObject(BuyPlayerInfo.sDelegater);
    if Delegater <> nil then
    begin
      IncDelegaterMoney(Delegater, BuyPlayerInfo, nTGameGoldCount, nTGamePointCount, nTGoldCount, nTGameDiamondCount,
        nTGameGirdCount, nTCustomMoneyCount);
      case BuyPlayerInfo.SellPricesType of
        0:
          Delegater.SysMsg(Format(g_sSellPlayerIncMoney, [BuyPlayerInfo.sSellPlayer, nTGameGoldCount, g_Config.sGameGoldName,
            BuyPlayerInfo.SellPrices - nTGameGoldCount, g_Config.sGameGoldName]), 255, 253, t_Hint);
        1:
          Delegater.SysMsg(Format(g_sSellPlayerIncMoney, [BuyPlayerInfo.sSellPlayer, nTGamePointCount, g_Config.sGamePointName,
            BuyPlayerInfo.SellPrices - nTGamePointCount, g_Config.sGamePointName]), 255, 253, t_Hint);
        2:
          Delegater.SysMsg(Format(g_sSellPlayerIncMoney, [BuyPlayerInfo.sSellPlayer, nTGoldCount, sSTRING_GOLDNAME,
            BuyPlayerInfo.SellPrices - nTGoldCount, sSTRING_GOLDNAME]), 255, 253, t_Hint);
        3:
          Delegater.SysMsg(Format(g_sSellPlayerIncMoney, [BuyPlayerInfo.sSellPlayer, nTGameDiamondCount,
            g_Config.sGameDiamondName, BuyPlayerInfo.SellPrices - nTGameDiamondCount, g_Config.sGameDiamondName]), 255,
            253, t_Hint);
        4:
          Delegater.SysMsg(Format(g_sSellPlayerIncMoney, [BuyPlayerInfo.sSellPlayer, nTGameGirdCount, g_Config.sGameGirdName,
            BuyPlayerInfo.SellPrices - nTGameGirdCount, g_Config.sGameGirdName]), 255, 253, t_Hint);
      else
        Delegater.SysMsg(Format(g_sSellPlayerIncMoney, [BuyPlayerInfo.sSellPlayer, nTCustomMoneyCount,
          GetCustomMoneyNameByRule(BuyPlayerInfo.SellPricesType, 3), BuyPlayerInfo.SellPrices - nTCustomMoneyCount,
          GetCustomMoneyNameByRule(BuyPlayerInfo.SellPricesType, 3)]), 255, 253, t_Hint);
      end;
    end
    else
    begin
      IncDelegaterMoney(BuyPlayerInfo);
    end;
    if Buyer <> nil then
    begin
      DefMsg := MakeDefaultMsg(SM_SELL_PLAYER_BUY_RET, 0, 0, 0, 0);
      Buyer.SendSocket(@DefMsg, '');
    end;
    Seller := GetPlayObject(BuyPlayerInfo.sSellPlayer);
    if Seller <> nil then
    begin
      if g_FunctionNPC <> nil then
      begin
        Seller.m_nScriptGotoCount := 0;
        g_FunctionNPC.GotoLable(Seller, '@PlayerSold', False);
        Seller.MakeGhost;
      end;
    end;
  end
  else
  begin
    if g_SellPlayerList.Search(BuyPlayerInfo.sSellPlayer, Index) then
    begin
      SellPlayerInfo := g_SellPlayerList.Items[Index];
      SellPlayerInfo.IsBuying := False;
    end;
    if (Buyer <> nil) then
    begin
      {
        DBResult.nResult:
        1: 参数错误，大小不对
        2: 参数错误，参数有空
        3: 买家角色为1个以上
        4: 角色已被卖出
        5: 执行Sql错误
      }
      IncBuyerGameMoney(Buyer, BuyPlayerInfo);
      DefMsg := MakeDefaultMsg(SM_SELL_PLAYER_BUY_RET, DBResult.nResult, 0, 0, 0);
      Buyer.SendSocket(@DefMsg, '');
    end
    else
    begin
      IncBuyerGameMoney(BuyPlayerInfo);
    end;
  end;
end;

procedure TUserEngine.ProcessDBSellPlayerDelegatorResult(DBResult: PTDBResult);
var
  SellPlayerInfo: PTDBSellPlayerInfo;
  Index: Integer;
  Seller: TPlayObject;
  DefMsg: TDefaultMessage;
  ErrCode: Integer;
  AskInfo: TSellPlayerAddAskInfo;
  sSendMsg: AnsiString;
begin
  if DBResult.LoadBuf = nil then
    Exit;
  SellPlayerInfo := PTDBSellPlayerInfo(DBResult.LoadBuf);
  Seller := GetPlayObject(TObject(SellPlayerInfo.Seller));
  if DBResult.nResult = 0 then
  begin
    if Seller = nil then
      Exit;
    if g_SellPlayerList.Search(SellPlayerInfo.sDelegater, Index) then
    begin
      ErrCode := -9;
      DefMsg := MakeDefaultMsg(SM_ADD_SELL_PLAYER_RET, ErrCode, 0, 0, 0);
      Seller.SendSocket(@DefMsg, '');
      Exit;
    end;
    if Seller.m_WantAddSellPlayerInfo = nil then
    begin
      New(Seller.m_WantAddSellPlayerInfo);
      FillChar(Seller.m_WantAddSellPlayerInfo^, SizeOf(TWantAddSellPlayerInfo), 0);
    end;
    Seller.m_WantAddSellPlayerInfo.IsMyOtherCharDelegator := True;
    Seller.m_WantAddSellPlayerInfo.Delegater := SellPlayerInfo.sDelegater;
    Seller.m_WantAddSellPlayerInfo.SellPricesType := SellPlayerInfo.SellPricesType;
    Seller.m_WantAddSellPlayerInfo.SellPrices := SellPlayerInfo.SellPrices;
    Seller.m_WantAddSellPlayerInfo.IsSetUser := SellPlayerInfo.sSetUser <> '';
    Seller.m_WantAddSellPlayerInfo.SetUserName := SellPlayerInfo.sSetUser;
    Seller.m_WantAddSellPlayerInfo.IsOtherPlayerConfirm := False;
    Seller.m_boBreakAddSellPlayer := False;
    seller.m_boStopBuyUser := False;
    if g_FunctionNPC <> nil then
    begin
      Seller.m_nScriptGotoCount := 0;
      g_FunctionNPC.GotoLable(Seller, '@BeforePlayerSelling', False);
    end;
    if Seller.m_boBreakAddSellPlayer then
    begin
      FreeMem(Seller.m_WantAddSellPlayerInfo);
      Seller.m_WantAddSellPlayerInfo := nil;
      Exit;
    end;
    AskInfo.ChrName := SellPlayerInfo.sDelegater;
    AskInfo.MoneyType := Seller.m_WantAddSellPlayerInfo.SellPricesType;
    AskInfo.Prices := Seller.m_WantAddSellPlayerInfo.SellPrices;
    sSendMsg := EncodeBuffer(@AskInfo, SizeOf(AskInfo));
    // 询问是否出售
    DefMsg := MakeDefaultMsg(SM_ADD_SELL_PLAYER_ASK, 0, 1, 0, 0); // SM_QUERYUSERSHOPITEMS
    Seller.SendSocketEx(@DefMsg, @sSendMsg[1], Length(sSendMsg));
  end
  else
  begin
    ErrCode := -8;
    DefMsg := MakeDefaultMsg(SM_ADD_SELL_PLAYER_RET, ErrCode, 0, 0, 0);
    Seller.SendSocket(@DefMsg, '');
  end;
end;

procedure TUserEngine.ProcessDBResult;
var
  I: Integer;
  DBResult: PTDBResult;
  LoadDummy: PTDBLoadDummy;
  PlayObject: TPlayObject;
begin
  m_DBResultList.LockW(2);
  try
    m_DBResultListTemp.Assign(m_DBResultList);
    m_DBResultList.Clear;
  finally
    m_DBResultList.UnLockW;
  end;
  for I := 0 to m_DBResultListTemp.Count - 1 do
  begin
    DBResult := m_DBResultListTemp.Items[I];
    try
      case DBResult.ResultType of
        drtLoadHuman:
          begin
            ProcessHumanDBResult(DBResult);
          end;
        drtLoadHero:
          begin
            ProcessHeroDBResult(DBResult);
          end;
        drtLoadDummy:
          begin
            if DBResult.LoadBuf <> nil then
            begin
              LoadDummy := PTDBLoadDummy(DBResult.LoadBuf);
              if DBResult.nResult = 0 then
              begin
                PlayObject := AddDummyObject(LoadDummy.sCharName, LoadDummy.sMapName, LoadDummy.nX, LoadDummy.nY);
                if PlayObject <> nil then
                begin
                  m_NewHumanList.Add(PlayObject);
                end;
              end
              else
              begin
                MainOutMessage(LoadDummy.sCharName + ' 假人登录失败，该名称可能已经被用户注册。');
              end;
            end;
          end;
        drtChrRename:
          begin
            ProcessDBRenameResult(DBResult);
          end;
        drtQueryHumanInfo:
          begin
            ProcessDBQueryHumanInfoResult(DBResult);
          end;
        drtGetRankData:
          begin
            ProcessDBGetRankDataResult(DBResult);
          end;
        drtBuyPlayer:
          begin
            ProcessDBBuyPlayerResult(DBResult);
          end;
        drtSellPlayerDelegator:
          begin
            ProcessDBSellPlayerDelegatorResult(DBResult);
          end;
      end;
    finally
      if DBResult.LoadBuf <> nil then
      begin
        FreeMem(DBResult.LoadBuf, DBResult.LoadBufLen);
      end;
      if DBResult.DataBuf <> nil then
      begin
        FreeMem(DBResult.DataBuf, DBResult.DataBufLen);
      end;
      Dispose(DBResult);
    end;
  end;
  m_DBResultListTemp.Clear;
end;

procedure TUserEngine.ProcessHumans;
var
  dwUsrRotTime: LongWord;
  dwCheckTime: LongWord; // 0x10
  dwCurTick: LongWord;
  nCheck30: Integer; // 0x30
  boCheckTimeLimit: Boolean; // 0x31
  nIdx: Integer;
  PlayObject: TPlayObject;
  I: Integer;
  DBLoadHumanResult: PTDBLoadHumanResult;
  LineNoticeMsg: string;
  OffLineData: pTOffLineData;
  nSessionID: Integer;
  nPayMode: Integer;
  nPayMent: Integer;
  S: string;
  List, DisappearList: TList;
  DummyLogon: pTDummyLogon;
  NoticeColor: pTNoticeColor;
  IsBreakParseVar: Boolean;
resourcestring
  sExceptionMsg1 = '[Exception] TUserEngine.ProcessHumans -> Ready, Save, Load... Code:=%d';
  sExceptionMsg2 = '[Exception] TUserEngine.ProcessHumans ClosePlayer.Delete - Free';
  sExceptionMsg3 = '[Exception] TUserEngine.ProcessHumans ClosePlayer.Delete';
  sExceptionMsg4 = '[Exception] TUserEngine.ProcessHumans RunNotice';
  sExceptionMsg5 = '[Exception] TUserEngine.ProcessHumans Human.Operate Code: %d';
  sExceptionMsg6 = '[Exception] TUserEngine.ProcessHumans Human.Finalize Code: %d';
  sExceptionMsg7 = '[Exception] TUserEngine.ProcessHumans RunSocket.CloseUser Code: %d';
  sExceptionMsg8 = '[Exception] TUserEngine.ProcessHumans';
begin
  nCheck30 := 0;
  dwCheckTime := MyGetTickCount();
  // ------------------------------------------------------------------------------
  if (MyGetTickCount - m_dwProcessLoadPlayTick) > 200 then
  begin
    m_dwProcessLoadPlayTick := MyGetTickCount();
    try
      if m_boStartAutoLoadOffline then
      begin // 登录离线人物
        // if MyGetTickCount - m_dwStartAutoLoadOfflineTick > 300 then begin
        // m_dwStartAutoLoadOfflineTick := MyGetTickCount;
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          m_AutoLoadOfflineList.LockW(1);
        try
{$IFEND}
          if m_AutoLoadOfflineList.Count > 0 then
          begin
            OffLineData := m_AutoLoadOfflineList.Items[0];
            if not OffLineData.boStartLogin then
            begin
              if GetPlayObjectOfAccount(OffLineData.sAccount) = nil then
              begin
                // MainOutMessage('FrmIDSoc.SendLogon:' + OffLineData.sAccount);
                OffLineData.boStartLogin := True;
                OffLineData.dwStartLoginTick := MyGetTickCount;
                FrmIDSoc.SendLogon(OffLineData.sAccount);
              end
              else
              begin
                Dispose(OffLineData);
                m_AutoLoadOfflineList.Delete(0);
              end;
            end
            else
            begin
              OffLineData.SessInfo := FrmIDSoc.GetAdmissionEx(OffLineData.sAccount, '', nSessionID, nPayMode, nPayMent);
              if (OffLineData.SessInfo <> nil) and (nPayMent > 0) then
              begin
                try
                  DataEngine.LoadHumanData(OffLineData.sAccount, OffLineData.sCharName, '127.0.0.1', '', '', True, False,
                    nSessionID, nPayMent, nPayMode, CLIENT_VERSION_NUMBER, 0, -1, -1, 0, True, 0, 0, 0, '');
                except
                  MainOutMessage(Format(sExceptionMsg1, [0]));
                end;
                m_AutoLoadOfflineList.Delete(0);
                Dispose(OffLineData);
              end
              else
              begin
                if MyGetTickCount - OffLineData.dwStartLoginTick > 1000 * 2 then
                begin
                  m_AutoLoadOfflineList.Delete(0);
                  Dispose(OffLineData);
                end;
              end;
            end;
          end;
          m_boStartAutoLoadOffline := m_AutoLoadOfflineList.Count > 0;
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            m_AutoLoadOfflineList.UnLockW;
        end;
{$IFEND}
        if not m_boStartAutoLoadOffline then
        begin
          MainOutMessage('脱机人物登录完成...');
        end;
      end;
      if m_boStartAutoLoadSellPlayer and (g_nKey_SellPlayer <> 0) then
      begin // 登录离线人物
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          m_AutoLoadSellPlayerList.LockW(1);
        try
{$IFEND}
          if m_AutoLoadSellPlayerList.Count > 0 then
          begin
            OffLineData := m_AutoLoadSellPlayerList.Items[0];
            if not OffLineData.boStartLogin then
            begin
              if GetPlayObjectOfAccount(OffLineData.sAccount) = nil then
              begin
                // MainOutMessage('FrmIDSoc.SendLogon:' + OffLineData.sAccount);
                OffLineData.boStartLogin := True;
                OffLineData.dwStartLoginTick := MyGetTickCount;
                FrmIDSoc.SendLogon(OffLineData.sAccount);
              end
              else
              begin
                Dispose(OffLineData);
                m_AutoLoadSellPlayerList.Delete(0);
              end;
            end
            else
            begin
              OffLineData.SessInfo := FrmIDSoc.GetAdmissionEx(OffLineData.sAccount, '', nSessionID, nPayMode, nPayMent);
              if (OffLineData.SessInfo <> nil) and (nPayMent > 0) then
              begin
                try
                  DataEngine.LoadHumanData(OffLineData.sAccount, OffLineData.sCharName, '127.0.0.1', '', '', True, False,
                    nSessionID, nPayMent, nPayMode, CLIENT_VERSION_NUMBER, 0, -1, -1, 0, True, 0, 0, 0, '');
                except
                  MainOutMessage(Format(sExceptionMsg1, [0]));
                end;
                m_AutoLoadSellPlayerList.Delete(0);
                Dispose(OffLineData);
              end
              else
              begin
                if MyGetTickCount - OffLineData.dwStartLoginTick > 1000 * 2 then
                begin
                  m_AutoLoadSellPlayerList.Delete(0);
                  Dispose(OffLineData);
                end;
              end;
            end;
          end;
          m_boStartAutoLoadSellPlayer := m_AutoLoadSellPlayerList.Count > 0;
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            m_AutoLoadSellPlayerList.UnLockW;
        end;
{$IFEND}
      end;
      // ---------------------离线挂机重新上线的人物登录-------------------------------
      m_LoadOfflineList.LockW(1);
      try
        for I := 0 to m_LoadOfflineList.Count - 1 do
        begin
          DBLoadHumanResult := PTDBLoadHumanResult(m_LoadOfflineList.Objects[I]);
          PlayObject := MakeNewHuman(@DBLoadHumanResult.LoadUser, @DBLoadHumanResult.Data);
          if PlayObject <> nil then
          begin
{$IF MULTI_THREAD = 1}
            if g_MultiThreadRun then
              m_NewHumanList.LockW(2);
            try
{$IFEND}
              m_NewHumanList.Add(PlayObject);
{$IF MULTI_THREAD = 1}
            finally
              if g_MultiThreadRun then
                m_NewHumanList.UnLockW;
            end;
{$IFEND}
          end
          else
          begin
{$IF MULTI_THREAD = 1}
            if g_MultiThreadRun then
              m_ListOfGateIdx.LockW(3);
            try
{$IFEND}
              m_ListOfGateIdx.Add(Pointer(DBLoadHumanResult.LoadUser.nGateIdx)); // 004B0C39
              m_ListOfSocket.Add(Pointer(DBLoadHumanResult.LoadUser.nSocket));
{$IF MULTI_THREAD = 1}
            finally
              if g_MultiThreadRun then
                m_ListOfGateIdx.UnLockW;
            end;
{$IFEND}
          end;
          Dispose(DBLoadHumanResult);
        end;
        m_LoadOfflineList.Clear;
      finally
        m_LoadOfflineList.UnLockW();
      end;
      // -------------------------------------------------------------------------------
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        m_NewHumanList.LockW(3);
      try
{$IFEND}
        for I := 0 to m_NewHumanList.Count - 1 do
        begin
          PlayObject := TPlayObject(m_NewHumanList.Items[I]);
          if (PlayObject.m_boDummyObject) or (PlayObject.m_boOffLine) then
          begin
            m_PlayObjectList.LockW(2);
            try
              m_PlayObjectList.AddObject(PlayObject.m_sCharName, PlayObject);
            finally
              m_PlayObjectList.UnLockW;
            end;
            if PlayObject.m_boOffLine then
            begin // 自动登录的离线挂机人物
              // MainOutMessage('m_NewHumanList 2 ' + PlayObject.m_sCharName);
              PlayObject.m_boSendNotice := True;
              PlayObject.m_boLoginNoticeOK := True;
              PlayObject.m_dwClientTick := 0;
              PlayObject.m_nOffOnlineTime := 0; // 1000 * 60 * 60 * 24 * 10; 离线挂机24天 chongchong 2018-05-16
            end;
            if PlayObject.m_boSellPlayering and g_Config.boSellPlayerAutoRecallHero then
            begin
              if PlayObject.m_sHeroName <> '' then
              begin
                DataEngine.LoadHeroData(PlayObject, g_FunctionNPC, PlayObject.m_sHeroName, False, 0);
              end;
            end;
            // MainOutMessage(PlayObject.m_sCharName + ' m_PlayObjectList.AddObject');
          end
          else
          begin
            if not RunSocket.SetGateUserList(PlayObject.m_nGateIdx, PlayObject.m_nSocket, PlayObject) then
            begin
              m_PlayObjectList.LockW(3);
              try
                m_PlayObjectList.AddObject(PlayObject.m_sCharName, PlayObject);
              finally
                m_PlayObjectList.UnLockW;
              end;
            end
            else
            begin
              m_PlayObjectList.LockW(4);
              try
                m_PlayObjectList.AddObject(PlayObject.m_sCharName, PlayObject);
              finally
                m_PlayObjectList.UnLockW;
              end;
            end;
            m_ReallyPlayObjectList.LockW(1);
            try
              m_ReallyPlayObjectList.Add(PlayObject);
            finally
              m_ReallyPlayObjectList.UnLockW;
            end;
          end;
        end;
        m_NewHumanList.Clear;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          m_NewHumanList.UnLockW;
      end;
{$IFEND}
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        m_ListOfGateIdx.LockW(4);
      try
{$IFEND}
        for I := 0 to m_ListOfGateIdx.Count - 1 do
        begin
          RunSocket.CloseUser(Integer(m_ListOfGateIdx.Items[I]), Integer(m_ListOfSocket.Items[I])); // GateIdx,nSocket
        end;
        m_ListOfGateIdx.Clear;
        m_ListOfSocket.Clear;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          m_ListOfGateIdx.UnLockW;
      end;
{$IFEND}
      if not g_boExitServer then
      begin // 登录假人
        if MyGetTickCount - m_dwDummyLogonTick > 1000 * g_Config.nDummyLogonTime then
        begin
          m_dwDummyLogonTick := MyGetTickCount;
          m_DummyLogonList.LockW(1);
          try
            if m_DummyLogonList.Count > 0 then
            begin
              Randomize;
              if g_Config.boDummyLogonRand then
                nIdx := Random(m_DummyLogonList.Count)
              else
                nIdx := 0;
              DummyLogon := pTDummyLogon(m_DummyLogonList.Objects[nIdx]);
              DataEngine.LoadDummyData(nil, nil, DummyLogon.sCharName, DummyLogon.sMapName, DummyLogon.nX, DummyLogon.nY);
              m_DummyLogonList.Delete(nIdx);
              Dispose(DummyLogon);
            end;
          finally
            m_DummyLogonList.UnLockW;
          end;
        end;
      end;
    except
      on E: Exception do
      begin
        MainOutMessage(Format(sExceptionMsg1, [0]));
        MainOutMessage(E.Message);
      end;
    end;
  end;
  try
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_PlayObjectFreeList.LockW(1);
    try
{$IFEND}
      for I := 0 to m_PlayObjectFreeList.Count - 1 do
      begin // for i := 0 to m_PlayObjectFreeList.Count - 1 do begin
        PlayObject := TPlayObject(m_PlayObjectFreeList.Items[I]);
        if (MyGetTickCount - PlayObject.m_dwGhostTick) > Max(120000, g_Config.dwHumanFreeDelayTime { 5 * 60 * 1000 } ) then
        begin
          try
            PlayObject := TPlayObject(m_PlayObjectFreeList.Items[I]);
            if (g_HighLevelHuman = PlayObject) then
              g_HighLevelHuman := nil;
            if (g_HighPKPointHuman = PlayObject) then
              g_HighPKPointHuman := nil;
            if (g_HighDCHuman = PlayObject) then
              g_HighDCHuman := nil;
            if (g_HighMCHuman = PlayObject) then
              g_HighMCHuman := nil;
            if (g_HighSCHuman = PlayObject) then
              g_HighSCHuman := nil;
            if (g_HighOnlineHuman = PlayObject) then
              g_HighOnlineHuman := nil;
            PlayObject.Free;
          except
            MainOutMessage(sExceptionMsg2);
          end;
          m_PlayObjectFreeList.Delete(I);
          Break;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_PlayObjectFreeList.UnLockW;
    end;
{$IFEND}
  except
    MainOutMessage(sExceptionMsg3);
  end;
  boCheckTimeLimit := False;
  try
    dwCurTick := MyGetTickCount();
    nIdx := m_nProcHumIDx;
    List := TList.Create;
    try
      DisappearList := TList.Create;
      try
        m_PlayObjectList.LockW(5);
        try
          while True do
          begin
            if m_PlayObjectList.Count <= nIdx then
              Break;
            PlayObject := TPlayObject(m_PlayObjectList.Objects[nIdx]);
            if PlayObject <> nil then
            begin
              if tick_diff(PlayObject.m_dwRunTick, dwCurTick) > PlayObject.m_nRunTime then
              begin
                PlayObject.m_dwRunTick := dwCurTick;
                if not PlayObject.m_boGhost then
                begin
                  List.Add(PlayObject);
                end
                else
                begin // if not PlayObject.boIsGhost then begin  //CODE:004B11C5
                  m_PlayObjectList.Delete(nIdx);
                  nCheck30 := 2;
                  // PlayObject.Disappear();
                  DisappearList.Add(PlayObject);
                  Continue;
                end;
              end; // if (dwTime14 - PlayObject.dw368) > PlayObject.dw36C then begin
              Inc(nIdx);
              if tick_diff(dwCheckTime, MyGetTickCount) > g_dwHumLimit then
              begin
                boCheckTimeLimit := True;
                m_nProcHumIDx := nIdx;
                Break;
              end;
            end; // while True do begin
          end;
        finally
          m_PlayObjectList.UnLockW;
        end;
        for I := 0 to DisappearList.Count - 1 do
        begin
          PlayObject := DisappearList.Items[I];
          m_ReallyPlayObjectList.LockW(2);
          try
            m_ReallyPlayObjectList.Remove(PlayObject);
          finally
            m_ReallyPlayObjectList.UnLockW;
          end;
          PlayObject.Disappear();
          try
            nCheck30 := 4;
            AddToHumanFreeList(PlayObject);
            nCheck30 := 5;
            PlayObject.DealCancelA();
            nCheck30 := 6;
            PlayObject.ChallengeCancelA();
            nCheck30 := 7;
            SaveHumRecord(PlayObject);
            nCheck30 := 8;
            if PlayObject.m_boDummyObject then
            begin
              nCheck30 := 9;
              PlayObject.m_boSoftClose := True;
            end
            else
            begin
              nCheck30 := 10;
              RunSocket.CloseUser(PlayObject.m_nGateIdx, PlayObject.m_nSocket, False);
            end;
          except
            MainOutMessage(Format(sExceptionMsg7, [nCheck30]));
          end;
        end;
      finally
        DisappearList.Free;
      end;
      for I := 0 to List.Count - 1 do
      begin
        PlayObject := List[I];
        if not PlayObject.m_boLoginNoticeOK then
        begin
          try
            PlayObject.RunNotice();
          except
            MainOutMessage(sExceptionMsg4);
          end;
        end;
        // 原来为else, 直接这里判断，不等下个时钟周期 chongchong 2018-09-24 01:51:37
        if PlayObject.m_boLoginNoticeOK then
        begin
          try
            if not PlayObject.m_boReadyRun then
            begin
              PlayObject.UserLogon;
              PlayObject.m_boReadyRun := True;
            end
            else
            begin
              if (MyGetTickCount() - PlayObject.m_dwSearchTick) > PlayObject.m_dwSearchTime then
              begin
                PlayObject.m_dwSearchTick := MyGetTickCount();
                nCheck30 := 10;
                PlayObject.SearchViewRange;
                nCheck30 := 11;
                PlayObject.GameTimeChanged;
                nCheck30 := 12;
              end;
            end;

            if (not PlayObject.m_boOffLine) and (not PlayObject.m_boDummyObject) then
            begin
              if (MyGetTickCount() - PlayObject.m_dwShowLineNoticeTick) > g_Config.dwShowLineNoticeTime then
              begin
                PlayObject.m_dwShowLineNoticeTick := MyGetTickCount();
                if LineNoticeList.Count > PlayObject.m_nShowLineNoticeIdx then
                begin
                  NoticeColor := pTNoticeColor(LineNoticeList.Objects[PlayObject.m_nShowLineNoticeIdx]);
                  S := LineNoticeList.Strings[PlayObject.m_nShowLineNoticeIdx];
                  LineNoticeMsg := g_ManageNPC.GetLineVariableText(PlayObject, S, IsBreakParseVar);
                  if NoticeColor.IsMove then
                  begin
                    PlayObject.SendNewMoveMsg(LineNoticeMsg, NoticeColor.btFColor, NoticeColor.btBColor, NoticeColor.boShowFrame,
                      NoticeColor.btFrameColor, NoticeColor.btFontSize, NoticeColor.boFontBold);
                  end
                  else
                  begin
                    if NoticeColor.LineNoticeColor <> nil then
                    begin
                      case TMsgColor(NoticeColor.LineNoticeColor^) of
                        c_Red:
                          PlayObject.SysMsg(LineNoticeMsg, c_Red, t_Notice);
                        c_Green:
                          PlayObject.SysMsg(LineNoticeMsg, c_Green, t_Notice);
                        c_Blue:
                          PlayObject.SysMsg(LineNoticeMsg, c_Blue, t_Notice);
                      else
                        PlayObject.SysMsg(LineNoticeMsg, NoticeColor.FColor, NoticeColor.BColor, t_Notice);
                      end;
                    end
                    else
                    begin
                      case NoticeColor.MsgColor of
                        c_Red:
                          PlayObject.SysMsg(LineNoticeMsg, c_Red, t_Notice);
                        c_Green:
                          PlayObject.SysMsg(LineNoticeMsg, c_Green, t_Notice);
                        c_Blue:
                          PlayObject.SysMsg(LineNoticeMsg, c_Blue, t_Notice);
                      else
                        PlayObject.SysMsg(LineNoticeMsg, NoticeColor.FColor, NoticeColor.BColor, t_Notice);
                      end;
                    end;
                  end;
                end;
                Inc(PlayObject.m_nShowLineNoticeIdx);
                if (LineNoticeList.Count <= PlayObject.m_nShowLineNoticeIdx) then
                  PlayObject.m_nShowLineNoticeIdx := 0;
              end;
            end;
            nCheck30 := 14;
            PlayObject.Run();
            // MainOutMessage('Run:'+PlayObject.m_sCharName);
            nCheck30 := 15;
            if not DataEngine.IsFull and ((MyGetTickCount() - PlayObject.m_dwSaveRcdTick) > g_Config.dwSaveHumanRcdTime) then
            begin
              nCheck30 := 16;
              PlayObject.m_dwSaveRcdTick := MyGetTickCount();
              nCheck30 := 17;
              PlayObject.DealCancelA();
              PlayObject.ChallengeCancelA();
              nCheck30 := 18;
              SaveHumRecord(PlayObject);
              nCheck30 := 19;
            end;
          except
            on E: Exception do
            begin
              MainOutMessage(Format(sExceptionMsg5, [nCheck30]));
              MainOutMessage(E.Message);
            end;
          end;
        end;
      end;
    finally
      List.Free;
    end;
    if not boCheckTimeLimit then
      m_nProcHumIDx := 0;
  except
    MainOutMessage(sExceptionMsg8);
  end;
  Inc(nProcessHumanLoopTime);
  g_nProcessHumanLoopTime := nProcessHumanLoopTime;
  if m_nProcHumIDx = 0 then
  begin
    nProcessHumanLoopTime := 0;
    g_nProcessHumanLoopTime := nProcessHumanLoopTime;
    dwUsrRotTime := MyGetTickCount - g_dwUsrRotCountTick;
    dwUsrRotCountMin := dwUsrRotTime;
    g_dwUsrRotCountTick := MyGetTickCount();
    if dwUsrRotCountMax < dwUsrRotTime then
      dwUsrRotCountMax := dwUsrRotTime;
  end;
  g_nHumCountMin := MyGetTickCount - dwCheckTime;
  if g_nHumCountMax < g_nHumCountMin then
    g_nHumCountMax := g_nHumCountMin;
end;

procedure TUserEngine.ProcessMerchants;
var
  I: Integer;
  dwRunTick, dwCurrTick: LongWord;
  MerchantNPC: TMerchant;
  boProcessLimit: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TUserEngine.ProcessMerchants';
begin
  dwRunTick := MyGetTickCount();
  boProcessLimit := False;
  dwCurrTick := MyGetTickCount();
{$IF MULTI_THREAD = 1}
  m_MerchantList.LockW(4);
  try
{$IFEND}
    try
      for I := nMerchantPosition to m_MerchantList.Count - 1 do
      begin
        MerchantNPC := TMerchant(m_MerchantList.Items[I]);
        if not MerchantNPC.m_boGhost then
        begin
          if Integer(dwCurrTick - MerchantNPC.m_dwRunTick) > MerchantNPC.m_nRunTime then
          begin
            if (MyGetTickCount - MerchantNPC.m_dwSearchTick) > MerchantNPC.m_dwSearchTime then
            begin
              MerchantNPC.m_dwSearchTick := MyGetTickCount();
              MerchantNPC.SearchViewRange();
            end;

            if Integer(dwCurrTick - MerchantNPC.m_dwRunTick) > MerchantNPC.m_nRunTime then
            begin
              MerchantNPC.m_dwRunTick := dwCurrTick;
              MerchantNPC.Run;
            end;
          end;
        end
        else if MyGetTickCount - MerchantNPC.m_dwGhostTick > 60 * 1000 then
        begin
          m_MerchantList.Delete(I);
          MerchantNPC.Free;
          Break;
        end;

        if MyGetTickCount - dwRunTick > g_dwNpcLimit then
        begin
          nMerchantPosition := I;
          boProcessLimit := True;
          Break;
        end;
      end;

      if not boProcessLimit then
        nMerchantPosition := 0;
    except
      MainOutMessage(sExceptionMsg);
    end;
{$IF MULTI_THREAD = 1}
  finally
    m_MerchantList.UnLockW;
  end;
{$IFEND}
  dwProcessMerchantTimeMin := MyGetTickCount - dwRunTick;
  if dwProcessMerchantTimeMin > dwProcessMerchantTimeMax then
    dwProcessMerchantTimeMax := dwProcessMerchantTimeMin;

  if dwProcessNpcTimeMin > dwProcessNpcTimeMax then
    dwProcessNpcTimeMax := dwProcessNpcTimeMin;
end;

function TUserEngine.AddMapMonGenCount(sMapName: string; nMonGenCount: Integer): Integer;
var
  I: Integer;
  MapMonGenCount: pTMapMonGenCount;
  boFound: Boolean;
begin
  Result := -1;
  boFound := False;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_MapMonGenCountList.LockW(1);
  try
{$IFEND}
    if m_MapMonGenCountList.Count > 0 then
    begin
      for I := 0 to m_MapMonGenCountList.Count - 1 do
      begin
        MapMonGenCount := m_MapMonGenCountList.Items[I];
        if MapMonGenCount <> nil then
        begin
          if CompareText(MapMonGenCount.sMapName, sMapName) = 0 then
          begin
            MapMonGenCount.nMonGenCount := MapMonGenCount.nMonGenCount + nMonGenCount;
            Result := MapMonGenCount.nMonGenCount;
            boFound := True;
          end;
        end;
      end; // for
    end;
    if not boFound then
    begin
      New(MapMonGenCount);
      MapMonGenCount.sMapName := sMapName;
      MapMonGenCount.nMonGenCount := nMonGenCount;
      MapMonGenCount.dwNotHumTimeTick := MyGetTickCount;
      MapMonGenCount.dwMakeMonGenTimeTick := MyGetTickCount;
      MapMonGenCount.nClearCount := 0;
      MapMonGenCount.boNotHum := True;
      m_MapMonGenCountList.Add(MapMonGenCount);
      Result := MapMonGenCount.nMonGenCount;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_MapMonGenCountList.UnLockW;
  end;
{$IFEND}
end;

function TUserEngine.GetMapMonGenCount(sMapName: string): pTMapMonGenCount;
var
  I: Integer;
  MapMonGenCount: pTMapMonGenCount;
begin
  Result := nil;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_MapMonGenCountList.LockR(2);
  try
{$IFEND}
    if m_MapMonGenCountList.Count > 0 then
    begin
      for I := 0 to m_MapMonGenCountList.Count - 1 do
      begin
        MapMonGenCount := m_MapMonGenCountList.Items[I];
        if MapMonGenCount <> nil then
        begin
          if CompareText(MapMonGenCount.sMapName, sMapName) = 0 then
          begin
            Result := MapMonGenCount;
            Break;
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_MapMonGenCountList.UnLockR;
  end;
{$IFEND}
end;

function TUserEngine.ClearMonsters(sMapName: string): Boolean;
var
  I, II: Integer;
  MonList: TList;
  Envir: TEnvirnoment;
  BaseObject: TBaseObject;
begin
  MonList := TList.Create;
  for I := 0 to g_MapManager.Count - 1 do
  begin
    Envir := TEnvirnoment(g_MapManager.Items[I]);
    if (Envir <> nil) and ((CompareText(Envir.sMapName, sMapName) = 0)) then
    begin
      UserEngine.GetMapMonster(Envir, MonList);
      for II := 0 to MonList.Count - 1 do
      begin
        BaseObject := TBaseObject(MonList.Items[II]);
        if BaseObject <> nil then
        begin
          if (BaseObject.m_btRaceServer <> 110) and (BaseObject.m_btRaceServer <> 111) and (BaseObject.m_btRaceServer <> 111) and
            (BaseObject.m_btRaceServer <> RC_GUARD) and (BaseObject.m_btRaceServer <> RC_ARCHERGUARD) and
            (BaseObject.m_btRaceServer <> RC_MOVE_ARCHERGUARD) and (BaseObject.m_btRaceServer <> 55) then
          begin
            BaseObject.m_boNoItem := True;
            BaseObject.m_WAbil.HP := 0;
          end;
        end;
      end;
    end;
  end;
  MonList.Free;
  Result := True;
end;

procedure TUserEngine.ProcessRegenMonsters;
  function CheckGVar(MonGen: pTMonGenInfo): Boolean;
  begin
    Result := True;
    case MonGen.GVarCompareType of
      ctLess { < } :
        Result := g_Config.GlobalVal[MonGen.GVarIndex] < MonGen.GVarValue;
      ctEqual { = } :
        Result := g_Config.GlobalVal[MonGen.GVarIndex] = MonGen.GVarValue;
      ctGreater { > } :
        Result := g_Config.GlobalVal[MonGen.GVarIndex] > MonGen.GVarValue;
      ctLessEqual { <= } :
        Result := g_Config.GlobalVal[MonGen.GVarIndex] <= MonGen.GVarValue;
      ctGreaterEqual { >= } :
        Result := g_Config.GlobalVal[MonGen.GVarIndex] >= MonGen.GVarValue;
      ctNotEqual { <> } :
        Result := g_Config.GlobalVal[MonGen.GVarIndex] <> MonGen.GVarValue;
    end;
  end;

{
  // 修复怪物占用cpu chongchong 2018-05-06
  function GetZenTime(dwTime: LongWord): LongWord;
  var
  r: Real;
  begin
  Result := dwTime;
  if dwTime < 30 * 60 * 1000 then
  begin
  r := (GetUserCount - g_Config.nUserFull) / g_Config.nZenFastStep;
  if r > 0 then
  begin
  if r > 6 then
  r := 6;
  Result := dwTime - Round(dwTime / 10 * r);
  end;
  end;
  end;
}
var
  MonGen: pTMonGenInfo;
  nGenCount, Index: Integer;
  boRegened: Boolean;
  nGenModCount: Integer;
  nMakeMonsterCount: Integer;
  dwCurrentTick: LongWord;
  PriorityMonGenRecord: PPriorityMonGenRecord;
begin
  // 修复怪物占用cpu chongchong 2018-05-06
  dwCurrentTick := MyGetTickCount;
  // 刷新怪物开始
  MonGen := nil;
  if g_boStopRun then
    Exit;
  if (not g_Config.boVentureServer) and (not g_Config.boStopM2MakeMon) and
    ((dwCurrentTick - dwRegenMonstersTick) > g_Config.dwRegenMonstersTime) then
  begin
    dwRegenMonstersTick := dwCurrentTick;
    // 处理智能刷怪优先处理地图 chongchong 2018-05-09
    if FPriorityMonGenList.Count > 0 then
    begin
      PriorityMonGenRecord := PPriorityMonGenRecord(FPriorityMonGenList.Objects[0]);
      if FPriorityMonGenListIndex >= PriorityMonGenRecord.Count then
        FPriorityMonGenListIndex := 0;
      Index := PriorityMonGenRecord.Index + FPriorityMonGenListIndex;
      if Index >= m_SortMapMonGenList.Count then
      begin
        Dispose(PriorityMonGenRecord);
        FPriorityMonGenList.Delete(0);
      end
      else
      begin
        MonGen := m_SortMapMonGenList.Items[Index];
        Inc(FPriorityMonGenListIndex);
        if FPriorityMonGenListIndex >= PriorityMonGenRecord.Count then
        begin
          Dispose(PriorityMonGenRecord);
          FPriorityMonGenList.Delete(0);
          FPriorityMonGenListIndex := 0;
        end;
      end;
    end;
    if MonGen = nil then
    begin
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        m_MonGenList.LockR(8);
      try
{$IFEND}
        if m_nCurrMonGen < m_MonGenList.Count then
        begin
          MonGen := m_MonGenList.Items[m_nCurrMonGen];
        end;
        if m_nCurrMonGen < m_MonGenList.Count - 1 then
        begin
          Inc(m_nCurrMonGen);
        end
        else
        begin
          m_nCurrMonGen := 0;
        end;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          m_MonGenList.UnLockR;
      end;
{$IFEND}
    end;
    if (MonGen <> nil) and (not MonGen.boFB) and (Length(MonGen.sMonName) > 0) and
      MonGen.Envir.m_boMakeMon { 智能刷怪 chongchong 2014-01-07 } and CheckGVar(MonGen) then
    begin
      // if (MonGen.dwStartTick = 0) or ((MyGetTickCount - MonGen.dwStartTick) > GetZenTime(MonGen.dwZenTime)) then
      // 修复无限刷怪
      if (MonGen.dwStartTick = 0) or ((MyGetTickCount - MonGen.dwStartTick) > MonGen.dwZenTime) then
      begin
        nGenCount := GetGenMonCount(MonGen);
        boRegened := True;
        if g_Config.nMonGenRate <= 0 then
          g_Config.nMonGenRate := 10; // 防止除法错误
        nGenModCount := _MAX(1, Round(_MAX(1, MonGen.nCount) / (g_Config.nMonGenRate / 10)));
        nMakeMonsterCount := nGenModCount - nGenCount;
        if nMakeMonsterCount < 0 then
          nMakeMonsterCount := 0;
        if nMakeMonsterCount > 0 then
        begin // 0806 增加 控制刷怪数量比例
          boRegened := RegenMonsters(MonGen, nMakeMonsterCount);
        end;
        if boRegened then
        begin
          MonGen.dwStartTick := MyGetTickCount();
        end;
      end;
      g_sMonGenInfo1 := MonGen.sMonName + ',' + IntToStr(m_nCurrMonGen) + '/' + IntToStr(m_MonGenList.Count);
    end;
  end;
  g_nMonGenTime := MyGetTickCount - dwCurrentTick;
  if g_nMonGenTime > g_nMonGenTimeMin then
    g_nMonGenTimeMin := g_nMonGenTime;
  if g_nMonGenTime > g_nMonGenTimeMax then
    g_nMonGenTimeMax := g_nMonGenTime;
  // 刷新怪物结束
end;

procedure TUserEngine.ProcessMonsters;
var
  dwCurrentTick: LongWord;
  dwRunTick: LongWord;
  dwMonProcTick: LongWord;
  MonGen: pTMonGenInfo;
  boProcessLimit: Boolean;
  I: Integer;
  nProcessPosition: Integer;
  Monster: TBaseObject; // TAnimalObject;
  tCode: Integer;
  IsError: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TUserEngine.ProcessMonsters %d; %s';
begin
  IsError := False;
  tCode := 0;
  try
    dwRunTick := MyGetTickCount();
    tCode := 0;
    boProcessLimit := False;
    dwCurrentTick := MyGetTickCount();
    // 修复怪物占用cpu chongchong 2018-05-06
    // ProcessRegenMonsters;
    dwMonProcTick := MyGetTickCount();
    nMonsterProcessCount := 0;
    tCode := 1;
    FRunMonsterList.Count := 0;
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MonGenList.LockR(9);
    try
{$IFEND}
      for I := m_nMonGenListPosition to m_MonGenList.Count - 1 do
      begin
        MonGen := m_MonGenList.Items[I];
        tCode := 11;
        if m_nMonGenCertListPosition < MonGen.CertList.Count then
        begin
          nProcessPosition := m_nMonGenCertListPosition;
        end
        else
        begin
          nProcessPosition := 0;
        end;
        m_nMonGenCertListPosition := 0;
        while (True) do
        begin
          if nProcessPosition >= MonGen.CertList.Count then
            Break;
          Monster := MonGen.CertList.Items[nProcessPosition];
          tCode := 12;
          if Monster <> nil then
          begin
            tCode := 121;
            { 智能刷怪 chongchong 2014-01-07 }
            if (not Monster.m_boGhost) and MonGen.boNoManNoMon and Monster.m_PEnvir.m_boClearMon then
            begin
              Monster.MakeGhost();
            end;
            tCode := 122;
            if not Monster.m_boGhost then
            begin
              tCode := 123;
              // 修复怪物占用cpu chongchong 2018-05-06
              if tick_diff(Monster.m_dwRunTick, dwCurrentTick) > Monster.m_nRunTime then
              begin
                tCode := 14;
                Monster.m_dwRunTick := dwRunTick;
                FRunMonsterList.Add(Monster);
                Inc(nMonsterProcessCount);
              end;
              tCode := 125;
              Inc(nMonsterProcessPostion);
            end
            else
            begin
              tCode := 126;
              if (MyGetTickCount - Monster.m_dwGhostTick) > 300000 then
              begin // 5分钟
                tCode := 127;
                MonGen.CertList.Delete(nProcessPosition);
                tCode := 128;
                FreeAndNil(Monster);
                Monster := nil;
                tCode := 129;
                Continue;
              end;
            end;
          end;
          tCode := 150;
          Inc(nProcessPosition);
          // 修复引擎报错 piaoyun 2013-12-26
          if Monster <> nil then
          begin
            if (MyGetTickCount - dwMonProcTick) > g_dwMonLimit then
            begin
              tCode := 15;
              g_sMonGenInfo2 := Monster.m_sCharName + '/' + IntToStr(I) + '/' + IntToStr(nProcessPosition);
              boProcessLimit := True;
              m_nMonGenCertListPosition := nProcessPosition;
              tCode := 16;
              Break;
            end;
          end;
        end; // while (True) do begin
        if boProcessLimit then
          Break;
        {
          if MonGen.boNoManNoMon and Monster.m_PEnvir.m_boClearMon then
          begin
          MonGen.boIsMonCleared := True;
          end;
        }
      end; // for I:= m_nMonGenListPosition to MonGenList.Count -1 do begin
      tCode := 2;
      if m_MonGenList.Count <= I then
      begin
        m_nMonGenListPosition := 0;
        nMonsterCount := nMonsterProcessPostion;
        nMonsterProcessPostion := 0;
        // n84 := (n84 + nMonsterProcessCount) div 2;
      end;
      if not boProcessLimit then
      begin
        m_nMonGenListPosition := 0;
      end
      else
      begin
        m_nMonGenListPosition := I;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MonGenList.UnLockR;
    end;
{$IFEND}
    tCode := 200;
    for I := 0 to FRunMonsterList.Count - 1 do
    begin
      Monster := FRunMonsterList.Items[I];
      if ((dwCurrentTick - Monster.m_dwSearchTick) > Monster.m_dwSearchTime) and
      // 优化刷怪CPU占用 (添加下面的代码) chongchong 2013-12-24
        (Monster.m_PEnvir <> nil) then
      begin
        if (Monster.m_PEnvir.HumCount > 0) or (Monster.m_PEnvir.HumBBCount > 0) or (Monster.m_wStatusTimeArr[POISON_DECHEALTH] > 0)
        then
        begin
          Monster.m_dwSearchTick := MyGetTickCount();
          tCode := 13;
          Monster.SearchViewRange();
        end
        else
        begin
          Monster.m_CurrTarget := nil;
          Monster.m_TargetCret := nil;
          Monster.m_LastHiter := nil;
          Monster.m_ExpHitter := nil;
          Monster.m_PoisonHitter := nil;
          Monster.m_CurrTargetEx := nil;
          Monster.ClearObject;
        end;
      end;
      tCode := 149;
      if not Monster.m_boIsVisibleActive and (Monster.m_nProcessRunCount < g_Config.nProcessMonsterInterval) then
      begin
        tCode := 114;
        Inc(Monster.m_nProcessRunCount);
      end
      // 优化刷怪CPU占用 (添加下面的代码) chongchong 2013-12-24
      else if (Monster.m_PEnvir <> nil) and ( // 超过人物退出10分钟才不运行
        (Monster.m_PEnvir.HumCount > 0) or (Monster.m_PEnvir.HumBBCount > 0) or
        (MyGetTickCount - Monster.m_PEnvir.ClearHumOrBBTick <= 600000) or Monster.m_boGhost or Monster.m_boDeath or
        (Monster.m_wStatusTimeArr[POISON_DECHEALTH] > 0)) then
      begin
        tCode := 115;
        Monster.m_nProcessRunCount := 0;
        tCode := 116;
        Monster.Run;
      end
      else if (Monster.m_PEnvir <> nil) then
      begin
        Monster.m_CurrTarget := nil;
        Monster.m_TargetCret := nil;
        Monster.m_LastHiter := nil;
        Monster.m_ExpHitter := nil;
        Monster.m_PoisonHitter := nil;
        Monster.m_CurrTargetEx := nil;
        Monster.ClearObject;
      end;
    end;
    tCode := 1600;
    g_nMonProcTime := MyGetTickCount - dwMonProcTick;
    if g_nMonProcTime > g_nMonProcTimeMin then
      g_nMonProcTimeMin := g_nMonProcTime;
    if g_nMonProcTime > g_nMonProcTimeMax then
      g_nMonProcTimeMax := g_nMonProcTime;
    tCode := 1601;
    g_nMonTimeMin := MyGetTickCount - dwRunTick;
    if g_nMonTimeMax < g_nMonTimeMin then
      g_nMonTimeMax := g_nMonTimeMin;
  except
    on E: Exception do
    begin
      if Monster <> nil then
      begin
        MainOutMessage(Format(sExceptionMsg, [tCode, E.Message]));
        IsError := True;
      end
      else
      begin
        MainOutMessage(Format(sExceptionMsg, [tCode, 'nil']));
      end;
    end;
  end;
  if IsError and (Monster <> nil) then
  begin
    try
      if not Monster.m_boGamePet then
        MainOutMessage(Format('异常怪物: %s', [Monster.m_sCharName]))
      else
        MainOutMessage(Format('异常宠物: %s', [Monster.m_sCharName]));
    except
    end;
  end;
end;

function TUserEngine.GetGenMonCount(MonGen: pTMonGenInfo): Integer;
var
  I: Integer;
  nCount: Integer;
  BaseObject: TBaseObject;
begin
  nCount := 0;
  for I := 0 to MonGen.CertList.Count - 1 do
  begin
    BaseObject := TBaseObject(MonGen.CertList.Items[I]);
    if BaseObject <> nil then
    begin
      if not BaseObject.m_boDeath and not BaseObject.m_boGhost then
        Inc(nCount);
    end;
  end;
  Result := nCount;
end;

procedure TUserEngine.ProcessNpcs;
var
  dwRunTick, dwCurrTick: LongWord;
  I: Integer;
  NPC: TNormNpc;
  boProcessLimit: Boolean;
begin
  dwRunTick := MyGetTickCount();
  boProcessLimit := False;
{$IF MULTI_THREAD = 1}
  QuestNPCList.LockW(2);
  try
{$IFEND}
    try
      dwCurrTick := MyGetTickCount();
      for I := nNpcPosition to QuestNPCList.Count - 1 do
      begin
        NPC := TNormNpc(QuestNPCList.Objects[I]);
        if not NPC.m_boGhost then
        begin
          if Integer(dwCurrTick - NPC.m_dwRunTick) > NPC.m_nRunTime then
          begin
            if (MyGetTickCount - NPC.m_dwSearchTick) > NPC.m_dwSearchTime then
            begin
              NPC.m_dwSearchTick := MyGetTickCount();
              NPC.SearchViewRange();
            end;

            if Integer(dwCurrTick - NPC.m_dwRunTick) > NPC.m_nRunTime then
            begin
              NPC.m_dwRunTick := dwCurrTick;
              NPC.Run;
            end;
          end;
        end
        else
        begin
          if (MyGetTickCount - NPC.m_dwGhostTick) > 60 * 1000 then
          begin
            NPC.Free;
            QuestNPCList.Delete(I);
            Break;
          end;
        end;
        if (MyGetTickCount - dwRunTick) > g_dwNpcLimit then
        begin
          nNpcPosition := I;
          boProcessLimit := True;
          Break;
        end;
      end;

      if not boProcessLimit then
        nNpcPosition := 0;
    except
      MainOutMessage('[Exceptioin] TUserEngine.ProcessNpcs');
    end;
{$IF MULTI_THREAD = 1}
  finally
    QuestNPCList.UnLockW;
  end;
{$IFEND}
  dwProcessNpcTimeMin := MyGetTickCount - dwRunTick;
  if dwProcessNpcTimeMin > dwProcessNpcTimeMax then
    dwProcessNpcTimeMax := dwProcessNpcTimeMin;
end;

function TUserEngine.FindDummyLogon(const sCharName: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  m_DummyLogonList.LockR(2);
  try
    for I := 0 to m_DummyLogonList.Count - 1 do
    begin
      if CompareText(m_DummyLogonList.Strings[I], sCharName) = 0 then
      begin
        Result := True;
        Break;
      end;
    end;
  finally
    m_DummyLogonList.UnLockR;
  end;
end;

procedure TUserEngine.AddDummyLogon(Dummy: pTDummyLogon);
var
  DummyLogon: pTDummyLogon;
begin
  m_DummyLogonList.LockW(3);
  try
    New(DummyLogon);
    DummyLogon^ := Dummy^;
    m_DummyLogonList.AddObject(DummyLogon.sCharName, TObject(DummyLogon));
  finally
    m_DummyLogonList.UnLockW;
  end;
end;

procedure TUserEngine.DelDummyLogon(const sCharName: string);
var
  I: Integer;
  Dummy: pTDummyLogon;
begin
  m_DummyLogonList.LockW(4);
  try
    for I := 0 to m_DummyLogonList.Count - 1 do
    begin
      if CompareText(m_DummyLogonList.Strings[I], sCharName) = 0 then
      begin
        Dummy := pTDummyLogon(m_DummyLogonList.Objects[I]);
        m_DummyLogonList.Delete(I);
        Dispose(Dummy);
        Break;
      end;
    end;
  finally
    m_DummyLogonList.UnLockW;
  end;
end;

function TUserEngine.RegenDummyObject(DummyLogon: pTDummyLogon): Boolean;
var
  PlayObject: TPlayObject;
begin
  Result := False;
  if GetPlayObject(DummyLogon.sCharName) = nil then
  begin
    PlayObject := AddDummyObject(DummyLogon.sCharName, DummyLogon.sMapName, DummyLogon.nX, DummyLogon.nY);
    if PlayObject <> nil then
    begin
      PlayObject.m_sHomeMap := GetHomeInfo(PlayObject.m_nHomeX, PlayObject.m_nHomeY);
      m_PlayObjectList.LockW(7);
      try
        m_PlayObjectList.AddObject(PlayObject.m_sCharName, PlayObject);
      finally
        m_PlayObjectList.UnLockW;
      end;
      Result := True;
    end;
  end;
end;

procedure TUserEngine.AddToMonsterList(BaseObject: TBaseObject);
var
  n18: Integer;
  MonGen: pTMonGenInfo;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_MonGenList.LockW(10);
  try
{$IFEND}
    n18 := m_MonGenList.Count - 1;
    if n18 < 0 then
      n18 := 0;
    MonGen := m_MonGenList.Items[n18];
    if MonGen <> nil then
    begin
      MonGen.CertList.Add(BaseObject);
      BaseObject.m_boAddToMaped := True;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_MonGenList.UnLockW;
  end;
{$IFEND}
end;

function TUserEngine.RegenMonsterByName(sMAP: string; nX, nY: Integer; sMonName: string): TBaseObject;
var
  nRace: Integer;
  BaseObject: TBaseObject;
  n18: Integer;
  MonGen: pTMonGenInfo;
  Map: TEnvirnoment;
  nCode: Integer;
begin
  Result := nil;
  nRace := GetMonRace(sMonName);
  if nRace <= 0 then
    Exit;
  Map := g_MapManager.FindMap(sMAP);
  if Map <> nil then
  begin
    nCode := 0;
    try
      BaseObject := AddBaseObject(Map, nX, nY, nRace, sMonName);
      nCode := 1;
      if BaseObject <> nil then
      begin
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          m_MonGenList.LockR(11);
        try
{$IFEND}
          nCode := 2;
          n18 := m_MonGenList.Count - 1;
          if n18 < 0 then
            n18 := 0;
          nCode := 3;
          MonGen := m_MonGenList.Items[n18];
          nCode := 4;
          if (MonGen <> nil) and (MonGen.CertList <> nil) then
          begin
            nCode := 5;
            MonGen.CertList.Add(BaseObject);
            // BaseObject.m_PEnvir.AddObject(BaseObject);
            BaseObject.m_boAddToMaped := True;
          end;
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            m_MonGenList.UnLockR;
        end;
{$IFEND}
        Result := BaseObject;
      end;
    except
      on E: Exception do
        MainOutMessage('TUserEngine.RegenMonsterByName Error; Code = ' + IntToStr(nCode) + ';' + E.Message);
    end;
  end;
end;

procedure TUserEngine.Run;
resourcestring
  sExceptionMsg = '[Exception] TUserEngine.Run';
var
  I: Integer;
  tmpPlayer: TPlayObject;
begin
  try
    if (MyGetTickCount() - dwShowOnlineTick) > g_Config.dwConsoleShowUserCountTime then
    begin
      dwShowOnlineTick := MyGetTickCount();
      NoticeManager.LoadingNotice;
      MainOutMessage('在线数: ' + IntToStr(GetUserCount) + ' 离线数: ' + IntToStr(GetOfflineCount) + ' 假人数: ' +
        IntToStr(GetDummyObjectCount));
      g_CastleManager.Save;
    end;

    if MyGetTickCount() - dwSendOnlineHumTime > 1000 then
    begin
      dwSendOnlineHumTime := MyGetTickCount();
      FrmIDSoc.SendOnlineHumCountMsg(GetOnlineHumCount);

      InterlockedExchange(g_AllCount, UserEngine.m_PlayObjectList.Count); // 演员总数
      InterlockedExchange(g_OnlinePCCount, 0);
      InterlockedExchange(g_OnlineH5Count, 0);
      InterlockedExchange(g_OffinePCCount, 0);
      InterlockedExchange(g_OffileH5Count, 0);
      InterlockedExchange(g_DummyCount, 0);

      for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
      begin
        tmpPlayer := TPlayObject(UserEngine.m_PlayObjectList.Objects[I]);
        if tmpPlayer = nil then
          Continue;

        if not tmpPlayer.m_boDummyObject then
        begin
          if tmpPlayer.m_Terminal = gatPC then
          begin
            if not tmpPlayer.m_boOffLine then // 在线H5
              InterlockedIncrement(g_OnlineH5Count)
            else
              InterlockedIncrement(g_OffileH5Count); // 离线H5
          end
          else
          begin
            if not tmpPlayer.m_boOffLine then // 在线PC
              InterlockedIncrement(g_OnlinePCCount)
            else
              InterlockedIncrement(g_OffinePCCount); // 离线PC
          end;
        end
        else
          InterlockedIncrement(g_DummyCount); // 假人
      end;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(sExceptionMsg);
      MainOutMessage(E.Message);
    end;
  end;
end;

function TUserEngine.GetStdItem(nItemIdx: Integer): pTStdItem;
begin
  Result := nil;
  Dec(nItemIdx);
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    StdItemList.LockR(2);
  try
{$IFEND}
    if (nItemIdx >= 0) and (StdItemList.Count > nItemIdx) then
    begin
      Result := StdItemList.Items[nItemIdx];
      if Result.Name = '' then
        Result := nil;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      StdItemList.UnLockR;
  end;
{$IFEND}
end;

function TUserEngine.GetStdItem(sItemName: string): pTStdItem;
var
  I: Integer;
begin
  Result := nil;
  if sItemName = '' then
    Exit;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    SortStdItemList.Lock;
  try
{$IFEND}
    I := SortStdItemList.IndexOf(sItemName);
    if I >= 0 then
    begin
      Result := PStdItemEx(SortStdItemList.Objects[I]).StdItem;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      SortStdItemList.UnLock;
  end;
{$IFEND}
end;

function TUserEngine.GetStdItemEx(sItemName: string; var Idx: Integer): pTStdItem;
var
  I: Integer;
  StdItem: PStdItemEx;
begin
  Result := nil;
  if sItemName = '' then
    Exit;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    SortStdItemList.Lock;
  try
{$IFEND}
    I := SortStdItemList.IndexOf(sItemName);
    if I >= 0 then
    begin
      StdItem := PStdItemEx(SortStdItemList.Objects[I]);
      Idx := StdItem.ItemIdx + 1;
      Result := StdItem.StdItem;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      SortStdItemList.UnLock;
  end;
{$IFEND}
end;

function TUserEngine.GetStdItemWeight(nItemIdx: Integer): Integer;
var
  StdItem: pTStdItem;
begin
  Dec(nItemIdx);
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    StdItemList.LockR(5);
  try
{$IFEND}
    if (nItemIdx >= 0) and (StdItemList.Count > nItemIdx) then
    begin
      StdItem := StdItemList.Items[nItemIdx];
      Result := StdItem.Weight;
    end
    else
    begin
      Result := 0;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      StdItemList.UnLockR;
  end;
{$IFEND}
end;

function TUserEngine.GetStdItemName(nItemIdx: Integer): string;
begin
  Result := '';
  Dec(nItemIdx);
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    StdItemList.LockR(6);
  try
{$IFEND}
    if (nItemIdx >= 0) and (StdItemList.Count > nItemIdx) then
    begin
      Result := pTStdItem(StdItemList.Items[nItemIdx]).Name;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      StdItemList.UnLockR;
  end;
{$IFEND}
end;

function TUserEngine.GetStdItemChangeName(UserItem: pTUserItem): string;
var
  nItemIdx: Integer;
  sUserItemName: string;
begin
  Result := '';
  nItemIdx := UserItem.wIndex;
  Dec(nItemIdx);
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    StdItemList.LockR(7);
  try
{$IFEND}
    if (nItemIdx >= 0) and (StdItemList.Count > nItemIdx) then
    begin
      sUserItemName := '';
      if UserItem.btValue[13] = 1 then
        sUserItemName := UserItem.Name;
      if sUserItemName = '' then
        sUserItemName := pTStdItem(StdItemList.Items[nItemIdx]).Name;
      Result := sUserItemName;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      StdItemList.UnLockR;
  end;
{$IFEND}
end;

procedure TUserEngine.CryCry(wIdent: Word; pMap: TEnvirnoment; nX, nY, nWide: Integer; btFColor, btBColor: Byte; sMsg: string);
// 黄字喊话
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(8);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if not PlayObject.m_boGhost and (PlayObject.m_PEnvir = pMap) and (PlayObject.m_boBanShout) and
        (abs(PlayObject.m_nCurrX - nX) < nWide) and (abs(PlayObject.m_nCurrY - nY) < nWide) then
      begin
        // PlayObject.SendMsg(nil,wIdent,0,0,$FFFF,0,sMsg);
        PlayObject.SendMsg(nil, wIdent, 0, btFColor, btBColor, 0, sMsg);
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.CryCryEx(wIdent: Word; pMap: TEnvirnoment; nX, nY, nWide: Integer; btFColor, btBColor: Byte; sMsg: string);
// 黄字喊话
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(9);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if not PlayObject.m_boGhost and (PlayObject.m_PEnvir = pMap) and (PlayObject.m_boBanShout) and
        (abs(PlayObject.m_nCurrX - nX) < nWide) and (abs(PlayObject.m_nCurrY - nY) < nWide) then
      begin
        // PlayObject.SendMsg(nil,wIdent,0,0,$FFFF,0,sMsg);
        PlayObject.m_dwSayAdvertiseTick := MyGetTickCount;
        PlayObject.SendMsg(nil, wIdent, 0, btFColor, btBColor, 0, sMsg);
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SetMonIcons(mon: TBaseObject);
var
  I: Integer;
begin
  if (not(mon.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])) then
  begin
    if FindMonster(mon.m_sCharName, I) then
    begin
      Move(pTMonInfo(MonsterList.Items[I]).Icons, mon.m_ActorIcons, SizeOf(TActorIconArray));
    end;
  end;
end;

// 新爆率 - 获取怪物爆物品 chongchong 2015-01-13
function TUserEngine.MonGetRandomItems(Hitter: TBaseObject; mon: TBaseObject; ExtRate, ExtRate2: Integer;
  IsPreview: Boolean): Integer;
  procedure RefItems(MonItem: pTMonItemInfo);
  var
    iname: string;
    UserItem: pTUserItem;
    StdItem: pTStdItem;
  begin
    if MonItem.boGold then
    begin
      mon.m_nGold := mon.m_nGold + (MonItem.Count div 2) + Random(MonItem.Count);
    end
    else
    begin
      iname := MonItem.ItemName;
      New(UserItem);
      if CopyToUserItemFromName(iname, UserItem) then
      begin
        StdItem := GetStdItem(UserItem.wIndex);
        if not((StdItem <> nil) and CheckOverLapItem(StdItem)) then
        begin
          // 聚灵珠从怪物身上爆出后，累积经验不为0
          if StdItem.StdMode = 49 then
            UserItem.Dura := Min(0, UserItem.DuraMax)
          else
            UserItem.Dura := Round((UserItem.DuraMax / 100) * (20 + Random(80)));
        end;

        if (g_Config.nMonRandomAddValue > 0) and (Random(g_Config.nMonRandomAddValue { 10 } ) = 0) then
          RandomUpgradeItem(UserItem);

        if StdItem.StdMode in [15, 19, 20, 21, 22, 23, 24, 26] then
        begin
          if (StdItem.Shape = 130) or (StdItem.Shape = 131) or (StdItem.Shape = 132) then
            GetUnknowItemValue(UserItem);
        end;

        RandomItemNewAbil(u_Mon, UserItem); // 新属性
        // 加入到背包中
        mon.m_ItemList.Add(UserItem);
      end
      else
        Dispose(UserItem);
    end;
  end;

  function CheckItemVar(MonItem: pTMonItemInfo; var Player: TPlayObject): Boolean;
  var
    I: Integer;
    IsReadValue1, IsReadValue2: Boolean;
    nReadValue1, nReadValue2: Integer;
    Master: TBaseObject;
    sVarName: string;
  begin
    if not MonItem.boCheckVar then
    begin
      Result := True;
      Exit;
    end;
    if MonItem.boCheckVarUseOR then
      Result := False
    else
      Result := True;
    {
      继承变量:
      0: 不继承
      1: 英雄继承
      2: 宝宝继承
      4: 宠物继承
    }
    Player := nil;
    if (Hitter <> nil) then
    begin
      if (Hitter.m_btRaceServer = RC_PLAYOBJECT) then
        Player := TPlayObject(Hitter)
      else if Hitter.m_btRaceServer = RC_HEROOBJECT then
      begin
        if (MonItem.nInheritedVarType and 1 <> 0) and (Hitter.m_Master <> nil) and (Hitter.m_Master.m_btRaceServer = RC_PLAYOBJECT)
        then
          Player := TPlayObject(Hitter.m_Master)
      end
      else if Hitter.m_boGamePet then
      begin
        if (MonItem.nInheritedVarType and 4 <> 0) and (Hitter.m_Master <> nil) and (Hitter.m_Master.m_btRaceServer = RC_PLAYOBJECT)
        then
          Player := TPlayObject(Hitter.m_Master)
      end
      else
      begin
        Master := Hitter.Master;
        if (MonItem.nInheritedVarType and 2 <> 0) and (Master <> nil) and (Master.m_btRaceServer = RC_PLAYOBJECT) then
        begin
          Player := TPlayObject(Master);
        end;
      end;
    end;
    for I := Low(MonItem.CheckVarArr) to High(MonItem.CheckVarArr) do
    begin
      if not MonItem.CheckVarArr[I].IsUse then
        Continue;
      IsReadValue1 := False;
      nReadValue1 := 0;
      if MonItem.CheckVarArr[I].Var1Name = '-' then
      begin
        nReadValue1 := MonItem.CheckVarArr[I].Var1Index;
        IsReadValue1 := True;
      end
      else if MonItem.CheckVarArr[I].Var1Name = 'I' then
      begin
        if (MonItem.CheckVarArr[I].Var1Index >= 0) and (MonItem.CheckVarArr[I].Var1Index < Length(g_Config.GlobaDyMval)) then
        begin
          nReadValue1 := g_Config.GlobaDyMval[MonItem.CheckVarArr[I].Var1Index];
          IsReadValue1 := True;
          // 透视时记录影响爆率的变量 2020-10-23
          if IsPreview then
          begin
            sVarName := MonItem.CheckVarArr[I].Var1Name + IntToStr(MonItem.CheckVarArr[I].Var1Index);
            if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
              mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue1));
          end;
        end;
      end
      else if MonItem.CheckVarArr[I].Var1Name = 'G' then
      begin
        if (MonItem.CheckVarArr[I].Var1Index >= 0) and (MonItem.CheckVarArr[I].Var1Index < Length(g_Config.GlobalVal)) then
        begin
          nReadValue1 := g_Config.GlobalVal[MonItem.CheckVarArr[I].Var1Index];
          IsReadValue1 := True;
          // 透视时记录影响爆率的变量 2020-10-23
          if IsPreview then
          begin
            sVarName := MonItem.CheckVarArr[I].Var1Name + IntToStr(MonItem.CheckVarArr[I].Var1Index);
            if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
              mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue1));
          end;
        end;
      end
      else if Player <> nil then
      begin
        if MonItem.CheckVarArr[I].Var1Name = 'D' then
        begin
          if (MonItem.CheckVarArr[I].Var1Index >= 0) and (MonItem.CheckVarArr[I].Var1Index < Length(Player.m_DyVal)) then
          begin
            nReadValue1 := Player.m_DyVal[MonItem.CheckVarArr[I].Var1Index];
            IsReadValue1 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var1Name + IntToStr(MonItem.CheckVarArr[I].Var1Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue1));
            end;
          end;
        end
        else if MonItem.CheckVarArr[I].Var1Name = 'M' then
        begin
          if (MonItem.CheckVarArr[I].Var1Index >= 0) and (MonItem.CheckVarArr[I].Var1Index < Length(Player.m_nMval)) then
          begin
            nReadValue1 := Player.m_nMval[MonItem.CheckVarArr[I].Var1Index];
            IsReadValue1 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var1Name + IntToStr(MonItem.CheckVarArr[I].Var1Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue1));
            end;
          end;
        end
        else if MonItem.CheckVarArr[I].Var1Name = 'N' then
        begin
          if (MonItem.CheckVarArr[I].Var1Index >= 0) and (MonItem.CheckVarArr[I].Var1Index < Length(Player.m_nInteger)) then
          begin
            nReadValue1 := Player.m_nInteger[MonItem.CheckVarArr[I].Var1Index];
            IsReadValue1 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var1Name + IntToStr(MonItem.CheckVarArr[I].Var1Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue1));
            end;
          end;
        end
        else if MonItem.CheckVarArr[I].Var1Name = 'U' then
        begin
          if (MonItem.CheckVarArr[I].Var1Index >= 0) and (MonItem.CheckVarArr[I].Var1Index < Length(Player.m_UVal)) then
          begin
            nReadValue1 := Player.m_UVal[MonItem.CheckVarArr[I].Var1Index];
            IsReadValue1 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var1Name + IntToStr(MonItem.CheckVarArr[I].Var1Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue1));
            end;
          end;
        end
        else if MonItem.CheckVarArr[I].Var1Name = 'J' then
        begin
          if (MonItem.CheckVarArr[I].Var1Index >= 0) and (MonItem.CheckVarArr[I].Var1Index < Length(Player.m_JVal)) then
          begin
            nReadValue1 := Player.m_JVal[MonItem.CheckVarArr[I].Var1Index];
            IsReadValue1 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var1Name + IntToStr(MonItem.CheckVarArr[I].Var1Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue1));
            end;
          end;
        end
      end
{$IF CompilerVersion >= 22}
      else if CharInSet(MonItem.CheckVarArr[I].Var1Name, ['D', 'M', 'N', 'U', 'J']) then
{$ELSE}
      else if MonItem.CheckVarArr[I].Var1Name in ['D', 'M', 'N', 'U', 'J'] then
{$IFEND}
      begin
        Result := False;
        if not MonItem.boCheckVarUseOR then
          Exit;
      end;
      IsReadValue2 := False;
      nReadValue2 := 0;
      if MonItem.CheckVarArr[I].Var2Name = '-' then
      begin
        nReadValue2 := MonItem.CheckVarArr[I].Var2Index;
        IsReadValue2 := True;
      end
      else if MonItem.CheckVarArr[I].Var2Name = 'I' then
      begin
        if (MonItem.CheckVarArr[I].Var2Index >= 0) and (MonItem.CheckVarArr[I].Var2Index < Length(g_Config.GlobaDyMval)) then
        begin
          nReadValue2 := g_Config.GlobaDyMval[MonItem.CheckVarArr[I].Var2Index];
          IsReadValue2 := True;
          // 透视时记录影响爆率的变量 2020-10-23
          if IsPreview then
          begin
            sVarName := MonItem.CheckVarArr[I].Var2Name + IntToStr(MonItem.CheckVarArr[I].Var2Index);
            if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
              mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue2));
          end;
        end;
      end
      else if MonItem.CheckVarArr[I].Var2Name = 'G' then
      begin
        if (MonItem.CheckVarArr[I].Var2Index >= 0) and (MonItem.CheckVarArr[I].Var2Index < Length(g_Config.GlobalVal)) then
        begin
          nReadValue2 := g_Config.GlobalVal[MonItem.CheckVarArr[I].Var2Index];
          IsReadValue2 := True;
          // 透视时记录影响爆率的变量 2020-10-23
          if IsPreview then
          begin
            sVarName := MonItem.CheckVarArr[I].Var2Name + IntToStr(MonItem.CheckVarArr[I].Var2Index);
            if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
              mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue2));
          end;
        end;
      end
      else if Player <> nil then
      begin
        if MonItem.CheckVarArr[I].Var2Name = 'D' then
        begin
          if (MonItem.CheckVarArr[I].Var2Index >= 0) and (MonItem.CheckVarArr[I].Var2Index < Length(Player.m_DyVal)) then
          begin
            nReadValue2 := Player.m_DyVal[MonItem.CheckVarArr[I].Var2Index];
            IsReadValue2 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var2Name + IntToStr(MonItem.CheckVarArr[I].Var2Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue2));
            end;
          end;
        end
        else if MonItem.CheckVarArr[I].Var2Name = 'M' then
        begin
          if (MonItem.CheckVarArr[I].Var2Index >= 0) and (MonItem.CheckVarArr[I].Var2Index < Length(Player.m_nMval)) then
          begin
            nReadValue2 := Player.m_nMval[MonItem.CheckVarArr[I].Var2Index];
            IsReadValue2 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var2Name + IntToStr(MonItem.CheckVarArr[I].Var2Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue2));
            end;
          end;
        end
        else if MonItem.CheckVarArr[I].Var2Name = 'N' then
        begin
          if (MonItem.CheckVarArr[I].Var2Index >= 0) and (MonItem.CheckVarArr[I].Var2Index < Length(Player.m_nInteger)) then
          begin
            nReadValue2 := Player.m_nInteger[MonItem.CheckVarArr[I].Var2Index];
            IsReadValue2 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var2Name + IntToStr(MonItem.CheckVarArr[I].Var2Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue2));
            end;
          end;
        end
        else if MonItem.CheckVarArr[I].Var2Name = 'U' then
        begin
          if (MonItem.CheckVarArr[I].Var2Index >= 0) and (MonItem.CheckVarArr[I].Var2Index < Length(Player.m_UVal)) then
          begin
            nReadValue2 := Player.m_UVal[MonItem.CheckVarArr[I].Var2Index];
            IsReadValue2 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var2Name + IntToStr(MonItem.CheckVarArr[I].Var2Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue2));
            end;
          end;
        end
        else if MonItem.CheckVarArr[I].Var2Name = 'J' then
        begin
          if (MonItem.CheckVarArr[I].Var2Index >= 0) and (MonItem.CheckVarArr[I].Var2Index < Length(Player.m_JVal)) then
          begin
            nReadValue2 := Player.m_JVal[MonItem.CheckVarArr[I].Var2Index];
            IsReadValue2 := True;
            // 透视时记录影响爆率的变量 2020-10-23
            if IsPreview then
            begin
              sVarName := MonItem.CheckVarArr[I].Var2Name + IntToStr(MonItem.CheckVarArr[I].Var2Index);
              if mon.m_PreviewMonItemSaveVarList.IndexOf(sVarName) < 0 then
                mon.m_PreviewMonItemSaveVarList.AddObject(sVarName, TObject(nReadValue2));
            end;
          end;
        end
      end
{$IF CompilerVersion >= 22}
      else if CharInSet(MonItem.CheckVarArr[I].Var1Name, ['D', 'M', 'N', 'U', 'J']) then
{$ELSE}
      else if MonItem.CheckVarArr[I].Var1Name in ['D', 'M', 'N', 'U', 'J'] then
{$IFEND}
      begin
        Result := False;
        if not MonItem.boCheckVarUseOR then
          Exit;
      end;
      if IsReadValue1 and IsReadValue2 then
      begin
        case MonItem.CheckVarArr[I].CompareType of
          ctLess { < } :
            Result := nReadValue1 < nReadValue2;
          ctEqual { = } :
            Result := nReadValue1 = nReadValue2;
          ctGreater { > } :
            Result := nReadValue1 > nReadValue2;
          ctLessEqual { <= } :
            Result := nReadValue1 <= nReadValue2;
          ctGreaterEqual { >= } :
            Result := nReadValue1 >= nReadValue2;
          ctNotEqual { <> } :
            Result := nReadValue1 <> nReadValue2;
        end;
        if MonItem.boCheckVarUseOR then
        begin
          if Result then
            Exit;
        end
        else
        begin
          if not Result then
            Exit;
        end;
      end;
    end;
  end;
  procedure GetItemsList(List: TList; boRandom: Boolean; TriggerScrpit: string; APlayer: TPlayObject);
  var
    I, nSelPoint, nMaxPoint: Integer;
    MonItem: pTMonItemInfo;
    RandIndex: Integer;
    TempPlayer: TPlayObject;
    IsAllowDropItem: Boolean;
    _Master: TBaseObject;
    sTemp: string;
    IsBreakParseVar: Boolean;
  begin
    if boRandom then
    begin
      if List.Count > 0 then
      begin
        RandIndex := Random(List.Count);
        MonItem := pTMonItemInfo(List[RandIndex]);
        if MonItem.List <> nil then
        begin
          if CheckItemVar(MonItem, TempPlayer) then
          begin
            GetItemsList(MonItem.List, MonItem.boRandom, MonItem.TriggerScrpit, TempPlayer);
          end;
        end
        else
        begin
          TempPlayer := nil;
          if (Hitter <> nil) then
          begin
            if (Hitter.m_btRaceServer = RC_PLAYOBJECT) then
              TempPlayer := TPlayObject(Hitter)
            else if Hitter.m_btRaceServer = RC_HEROOBJECT then
            begin
              if (Hitter.m_Master <> nil) and (Hitter.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
                TempPlayer := TPlayObject(Hitter.m_Master)
            end
            else if Hitter.m_boGamePet then
            begin
              if (Hitter.m_Master <> nil) and (Hitter.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
                TempPlayer := TPlayObject(Hitter.m_Master)
            end
            else
            begin
              _Master := Hitter.Master;
              if (_Master <> nil) and (_Master.m_btRaceServer = RC_PLAYOBJECT) then
              begin
                TempPlayer := TPlayObject(_Master);
              end;
            end;
          end;
          // #CHILD 子爆率未算机率  chongchong 2018-05-11
          if not MonItem.SelPoint.IsUseVar then
            nSelPoint := MonItem.SelPoint.nValue
          else if (g_FunctionNPC <> nil) and (TempPlayer <> nil) then
            g_FunctionNPC.GetVarValue(TempPlayer, MonItem.SelPoint.VarName, sTemp, nSelPoint, IsBreakParseVar);
          if not MonItem.MaxPoint.IsUseVar then
            nMaxPoint := MonItem.MaxPoint.nValue
          else if (g_FunctionNPC <> nil) and (TempPlayer <> nil) then
            g_FunctionNPC.GetVarValue(TempPlayer, MonItem.MaxPoint.VarName, sTemp, nMaxPoint, IsBreakParseVar);
          if Random(nMaxPoint) <= nSelPoint - 1 then
          begin
            IsAllowDropItem := True;
            if (APlayer <> nil) and (TriggerScrpit <> '') and (g_FunctionNPC <> nil) then
            begin
              APlayer.m_boAllowDropItem := IsAllowDropItem;
              APlayer.m_nScriptGotoCount := 0;
              APlayer.m_nCurrentItemMakeIndex := 0;
              APlayer.m_sCurrentItemNewName := MonItem.ItemName;
              APlayer.m_sCurrentItemName := MonItem.ItemName;
              g_FunctionNPC.GotoLable(APlayer, TriggerScrpit, False);
              APlayer.m_nCurrentItemMakeIndex := 0;
              APlayer.m_sCurrentItemNewName := '';
              APlayer.m_sCurrentItemName := '';
              IsAllowDropItem := APlayer.m_boAllowDropItem;
            end;
            if IsAllowDropItem then
              RefItems(MonItem);
          end;
        end;
      end;
    end
    else
    begin
      for I := 0 to List.Count - 1 do
      begin
        MonItem := pTMonItemInfo(List[I]);
        ExtRate := Max(0, ExtRate);
        ExtRate := Min(100, ExtRate);
        TempPlayer := nil;
        if (Hitter <> nil) then
        begin
          if (Hitter.m_btRaceServer = RC_PLAYOBJECT) then
            TempPlayer := TPlayObject(Hitter)
          else if Hitter.m_btRaceServer = RC_HEROOBJECT then
          begin
            if (Hitter.m_Master <> nil) and (Hitter.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
              TempPlayer := TPlayObject(Hitter.m_Master)
          end
          else if Hitter.m_boGamePet then
          begin
            if (Hitter.m_Master <> nil) and (Hitter.m_Master.m_btRaceServer = RC_PLAYOBJECT) then
              TempPlayer := TPlayObject(Hitter.m_Master)
          end
          else
          begin
            _Master := Hitter.Master;
            if (_Master <> nil) and (_Master.m_btRaceServer = RC_PLAYOBJECT) then
            begin
              TempPlayer := TPlayObject(_Master);
            end;
          end;
        end;
        if not MonItem.SelPoint.IsUseVar then
          nSelPoint := MonItem.SelPoint.nValue
        else if (g_FunctionNPC <> nil) and (TempPlayer <> nil) then
          g_FunctionNPC.GetVarValue(TempPlayer, MonItem.SelPoint.VarName, sTemp, nSelPoint, IsBreakParseVar);
        if not MonItem.MaxPoint.IsUseVar then
          nMaxPoint := MonItem.MaxPoint.nValue
        else if (g_FunctionNPC <> nil) and (TempPlayer <> nil) then
          g_FunctionNPC.GetVarValue(TempPlayer, MonItem.MaxPoint.VarName, sTemp, nMaxPoint, IsBreakParseVar);
        nMaxPoint := Round(nMaxPoint / 100 * (100 - ExtRate));
        if ExtRate2 <> 0 then
          nMaxPoint := Round(nMaxPoint / ExtRate2 * 100);
        if Random(nMaxPoint) <= nSelPoint - 1 then
        begin
          if MonItem.List <> nil then
          begin
            if CheckItemVar(MonItem, TempPlayer) then
            begin
              GetItemsList(MonItem.List, MonItem.boRandom, MonItem.TriggerScrpit, TempPlayer);
            end;
          end
          else
          begin
            IsAllowDropItem := True;
            if (APlayer <> nil) and (TriggerScrpit <> '') and (g_FunctionNPC <> nil) then
            begin
              APlayer.m_boAllowDropItem := IsAllowDropItem;
              APlayer.m_nScriptGotoCount := 0;
              APlayer.m_nCurrentItemMakeIndex := 0;
              APlayer.m_sCurrentItemNewName := MonItem.ItemName;
              APlayer.m_sCurrentItemName := MonItem.ItemName;
              g_FunctionNPC.GotoLable(APlayer, TriggerScrpit, False);
              APlayer.m_nCurrentItemMakeIndex := 0;
              APlayer.m_sCurrentItemNewName := '';
              APlayer.m_sCurrentItemName := '';
              IsAllowDropItem := APlayer.m_boAllowDropItem;
            end;
            if IsAllowDropItem then
              RefItems(MonItem);
          end;
        end;
      end;
    end;
  end;

var
  I: Integer;
  ItemList: TList;
  Player: TPlayObject;
  UserItem: pTUserItem;
begin
  ItemList := nil;
  mon.m_nGold := 0;
  for I := mon.m_ItemList.Count - 1 downto 0 do
  begin
    UserItem := mon.m_ItemList[I];
    mon.m_ItemList.Delete(I);
    Dispose(UserItem);
  end;
  mon.m_ItemList.Clear;
  mon.m_PreviewMonItemSaveVarList.Clear;
  if (Hitter <> nil) and (Hitter.m_btRaceServer = RC_PLAYOBJECT) then
  begin
    Player := Hitter as TPlayObject;
    I := Player.m_MonItems.IndexOf(mon.m_sCharName);
    if I >= 0 then
    begin
      ItemList := TList(Player.m_MonItems.Objects[I]);
    end;
  end;
  if ItemList = nil then
  begin
    if mon.m_boUseOldDropFile and (mon.m_sOldCharName <> '') then
    begin
      if FindMonster(mon.m_sOldCharName, I) then
      begin
        ItemList := pTMonInfo(MonsterList.Items[I]).ItemList;
      end;
    end
    else
    begin
      if FindMonster(mon.m_sCharName, I) then
      begin
        ItemList := pTMonInfo(MonsterList.Items[I]).ItemList;
      end;
    end;
  end;
  Randomize;
  if ItemList <> nil then
    GetItemsList(ItemList, False, '', nil);
  Result := 1;
end;

procedure TUserEngine.RandomUpgradeItem(Item: pTUserItem);
var
  StdItem: pTStdItem;
begin
  StdItem := GetStdItem(Item.wIndex);
  if StdItem = nil then
    Exit;
  case StdItem.StdMode of
    5, 6:
      ItemUnit.RandomUpgradeWeapon(Item); // 004AD14A
    10, 11:
      ItemUnit.RandomUpgradeDress(Item);
    19:
      ItemUnit.RandomUpgrade19(Item);
    20, 21, 24:
      ItemUnit.RandomUpgrade202124(Item);
    26:
      ItemUnit.RandomUpgrade26(Item);
    22:
      ItemUnit.RandomUpgrade22(Item);
    23:
      ItemUnit.RandomUpgrade23(Item);
    15, 16:
      ItemUnit.RandomUpgradeHelMet(Item);
    52, 54, 62, 64:
      ItemUnit.RandomUpgradeBoots(Item); // 鞋子，腰带
    66, 67:
      ItemUnit.RandomUpgradeFashionDress(Item); // 时装衣服
    68, 69:
      ItemUnit.RandomUpgradeFashionWeapon(Item); // 时装武器
    28:
      ItemUnit.RandomUpgradeHorse(Item); // 马牌
    65:
      ItemUnit.RandomUpgradeDrum(Item); // 军鼓
    12:
      ItemUnit.RandomUpgradeShield(Item); // 盾牌
  end;
end;

procedure TUserEngine.GetUnknowItemValue(Item: pTUserItem);
// 神秘装备  (StdItem.Shape = 130) or (StdItem.Shape = 131) or (StdItem.Shape = 132)
var
  StdItem: pTStdItem;
begin
  StdItem := GetStdItem(Item.wIndex);
  if StdItem = nil then
    Exit;
  case StdItem.StdMode of
    15, 16:
      ItemUnit.UnknowHelmet(Item);
    22, 23:
      ItemUnit.UnknowRing(Item);
    24, 26:
      ItemUnit.UnknowNecklace(Item);
  end;
end;

procedure TUserEngine.RandomItemNewAbil(ItemNewAbilRate: Integer; Item: pTUserItem);
var
  nWhere: Integer;
  StdItem: pTStdItem;
begin
  if not g_Config.boItemNewAbilAllowUse then
    Exit;
  StdItem := GetStdItem(Item.wIndex);
  if StdItem = nil then
    Exit;
  case StdItem.StdMode of
    5, 6:
      nWhere := 0; // 武器
    10, 11:
      nWhere := 1; // 衣服
    19, 20, 21:
      nWhere := 2; // 项链
    24, { 25, } 26:
      nWhere := 3; // 手镯
    22, 23:
      nWhere := 4; // 戒指
    15:
      nWhere := 5; // 头盔
    54, 64:
      nWhere := 6; // 腰带
    52, 62:
      nWhere := 7; // 鞋子
    // 斗笠、马牌、军鼓、盾牌、时装衣服、时装武器支持元素 piaoyun 2013-10-27
    16:
      nWhere := 8; // 斗笠
    28:
      nWhere := 9; // 马牌
    65:
      nWhere := 14; // 军鼓
    12:
      nWhere := 11; // 盾牌
    66, 67:
      nWhere := 12; // 时装衣服
    68, 69:
      nWhere := 13; // 时装武器4871422222245036
    53:
      nWhere := 14; // 53类宝石
    63:
      nWhere := 15; // 63类宝石
    75, 76, 77:
      nWhere := 16; // 时装项链
    78:
      nWhere := 17; // 时装头盔
    79, 80:
      nWhere := 18; // 时装手镯
    81, 82:
      nWhere := 19; // 时装戒指
    83:
      nWhere := 20; // 时装勋章
    84, 85:
      nWhere := 21; // 时装腰带
    86, 87:
      nWhere := 22; // 时装靴子
    88, 89:
      nWhere := 23; // 时装宝石
    90:
      nWhere := 24; // 灵玉
  else
    Exit;
  end;
  if (Random(ItemNewAbilRate) = 0) then
    ItemUnit.ItemNewAbilRandomUpgrade(Item, nWhere);
end;

procedure TUserEngine.RandomItemNewAbil(ItemUpgradeRate: TItemUpgradeRate; Item: pTUserItem);
var
  nWhere: Integer;
  StdItem: pTStdItem;
begin
  if not g_Config.boItemNewAbilAllowUse then
    Exit;
  StdItem := GetStdItem(Item.wIndex);
  if StdItem = nil then
    Exit;
  case StdItem.StdMode of
    5, 6:
      nWhere := 0; // 武器
    10, 11:
      nWhere := 1; // 衣服
    19, 20, 21:
      nWhere := 2; // 项链
    24, { 25, } 26:
      nWhere := 3; // 手镯
    22, 23:
      nWhere := 4; // 戒指
    15:
      nWhere := 5; // 头盔
    52, 62:
      nWhere := 6; // 鞋子
    54, 64:
      nWhere := 7; // 腰带
    // 斗笠、马牌、军鼓、盾牌、时装衣服、时装武器支持元素 piaoyun 2013-10-27
    16:
      nWhere := 8; // 斗笠
    28:
      nWhere := 9; // 马牌
    65:
      nWhere := 14; // 军鼓
    12:
      nWhere := 11; // 盾牌
    66, 67:
      nWhere := 12; // 时装衣服
    68, 69:
      nWhere := 13; // 时装武器
    53:
      nWhere := 14; // 53类宝石
    63:
      nWhere := 15; // 63类宝石
    75, 76, 77:
      nWhere := 16; // 时装项链
    78:
      nWhere := 17; // 时装头盔
    79, 80:
      nWhere := 18; // 时装手镯
    81, 82:
      nWhere := 19; // 时装戒指
    83:
      nWhere := 20; // 时装勋章
    84, 85:
      nWhere := 21; // 时装腰带
    86, 87:
      nWhere := 22; // 时装靴子
    88, 89:
      nWhere := 23; // 时装宝石
    90:
      nWhere := 24; // 灵玉
  else
    Exit;
  end;
  case ItemUpgradeRate of
    u_Mon:
      if (Random(g_Config.nItemNewAbilMonRandomAddValue) = 0) then
        ItemUnit.ItemNewAbilRandomUpgrade(Item, nWhere);
    u_Make:
      if (Random(g_Config.nItemNewAbilMakeRandomAddValue) = 0) then
        ItemUnit.ItemNewAbilRandomUpgrade(Item, nWhere);
    u_Script:
      if (Random(g_Config.nItemNewAbilScriptRandomAddValue) = 0) then
        ItemUnit.ItemNewAbilRandomUpgrade(Item, nWhere);
  end;
  { if (ItemUpgradeRate = u_Mon) and (Random(g_Config.nItemNewAbilMonRandomAddValue) = 0) then
    ItemUnit.ItemNewAbilRandomUpgrade(Item, nWhere)
    else if (ItemUpgradeRate = u_Make) and (Random(g_Config.nItemNewAbilMakeRandomAddValue) = 0) then
    ItemUnit.ItemNewAbilRandomUpgrade(Item, nWhere)
    else if (ItemUpgradeRate = u_Script) and (Random(g_Config.nItemNewAbilScriptRandomAddValue) = 0) then
    ItemUnit.ItemNewAbilRandomUpgrade(Item, nWhere)
    else
    ItemUnit.ItemNewAbilRandomUpgrade(Item, nWhere); }
end;

function TUserEngine.CopyToUserItemFromName(sItemName: string; Item: pTUserItem): Boolean;
var
  I: Integer;
  dLastDate: PDouble;
  ItemEx: PStdItemEx;
  MonInfo: pTMonInfo;
begin
  Result := False;
  if sItemName <> '' then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      StdItemList.LockR(8);
    try
{$IFEND}
      I := SortStdItemList.IndexOf(sItemName);
      if I >= 0 then
      begin
        ItemEx := PStdItemEx(SortStdItemList.Objects[I]);
        FillChar(Item^, SizeOf(TUserItem), #0);
        Item.wIndex := ItemEx.ItemIdx + 1;
        Item.MakeIndex := GetItemNumber();
        Item.Dura := ItemEx.StdItem.DuraMax;
        Item.DuraMax := ItemEx.StdItem.DuraMax;
        Item.ItemFrom.DateTime := Now;
        if ItemEx.StdItem.StdMode = 49 then
        begin // 聚灵珠
          Item.Dura := 0;
          if ItemEx.StdItem.Reserved1 > 0 then
          begin
            dLastDate := @Item.btValue[4];
            dLastDate^ := Date + ItemEx.StdItem.Reserved1;
          end;
        end
        // 祝福罐 chongchong 2017-07-08
        else if ItemEx.StdItem.StdMode = 96 then
        begin
          Item.Dura := 0;
        end
        // 103限时物品从出生开始计时 chongchong 2015-12-20
        else if ItemEx.StdItem.Need in [103, 104] then
        begin
          Item.nLimitTime := ItemEx.StdItem.NeedLevel;
          Item.boStartTime := True;
          if CheckOverLapItem(ItemEx.StdItem) then
            Item.Dura := 0;
        end
        // 宠物蛋
        else if ItemEx.StdItem.StdMode in [91, 92] then
        begin
          MonInfo := GetMonInfo(ItemEx.StdItem.Name);
          if MonInfo <> nil then
          begin
            Item.Dura := LoWord(MonInfo.nLevel);
            Item.DuraMax := HiWord(MonInfo.nLevel);
            Item.btNewValue[0] := LoWord(MonInfo.nHP);
            Item.btNewValue[11] := HiWord(MonInfo.nHP);
            Item.btNewValue[1] := LoWord(MonInfo.nMP);
            Item.btNewValue[12] := HiWord(MonInfo.nMP);
            RecallGamePetAbilToUserItem(ItemEx.StdItem.Name, MonInfo.nLevel, Item);
          end;
        end
        else if CheckOverLapItem(ItemEx.StdItem) then
          Item.Dura := 0;
        Result := True;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        StdItemList.UnLockR;
    end;
{$IFEND}
  end;
end;

function TUserEngine.CopyToUserItemFromItem(StdItem: pTStdItem; wIndex: Integer; Item: pTUserItem): Boolean;
var
  dLastDate: PDouble;
  MonInfo: pTMonInfo;
begin
  Result := False;
  if StdItem = nil then
    Exit;

  FillChar(Item^, SizeOf(TUserItem), #0);
  Item.wIndex := wIndex;
  Item.MakeIndex := GetItemNumber();
  Item.Dura := StdItem.DuraMax;
  Item.DuraMax := StdItem.DuraMax;
  Item.ItemFrom.DateTime := Now;
  if StdItem.StdMode = 49 then
  begin // 聚灵珠
    Item.Dura := 0;
    if StdItem.Reserved1 > 0 then
    begin
      dLastDate := @Item.btValue[4];
      dLastDate^ := Date + StdItem.Reserved1;
    end;
  end
  // 祝福罐 chongchong 2017-07-08
  else if StdItem.StdMode = 96 then
  begin
    Item.Dura := 0;
  end
  // 103限时物品从出生开始计时 chongchong 2015-12-20
  else if StdItem.Need in [103, 104] then
  begin
    Item.nLimitTime := StdItem.NeedLevel;
    Item.boStartTime := True;
    if CheckOverLapItem(StdItem) then
      Item.Dura := 0;
  end
  // 宠物蛋
  else if StdItem.StdMode in [91, 92] then
  begin
    MonInfo := GetMonInfo(StdItem.Name);
    if MonInfo <> nil then
    begin
      Item.Dura := LoWord(MonInfo.nLevel);
      Item.DuraMax := HiWord(MonInfo.nLevel);
      Item.btNewValue[0] := LoWord(MonInfo.nHP);
      Item.btNewValue[11] := HiWord(MonInfo.nHP);
      Item.btNewValue[1] := LoWord(MonInfo.nMP);
      Item.btNewValue[12] := HiWord(MonInfo.nMP);
      RecallGamePetAbilToUserItem(StdItem.Name, MonInfo.nLevel, Item);
    end;
  end
  else if CheckOverLapItem(StdItem) then
    Item.Dura := 0;
  Result := True;
end;

procedure TUserEngine.ProcessUserMessage(PlayObject: TPlayObject; DefMsg: pTDefaultMessage; Buff: PAnsiChar);
var
  sMsg, sTemp: AnsiString;
resourcestring
  sExceptionMsg = '[Exception] TUserEngine.ProcessUserMessage, MsgIdent: ';
begin
  if (DefMsg = nil) or (PlayObject = nil) then
    Exit;

  // 过滤掉RM消息 ★★★  ★★★ chongchong 2016-03-02
  // 防止客户端直接发送 RM_SENDDELITEMLIST 等消息造成M2重启
  if DefMsg.Ident >= RM_SPELL then
    Exit;

  try
    if Buff = nil then
      sMsg := ''
    else
      sMsg := System.AnsiStrings.StrPas(Buff);

    case DefMsg.Ident of
      CM_SPELL, CM_SITDOWN:
        begin // 3017
          // if PlayObject.GetSpellMsgCount <=2 then  //如果队排里有超过二个魔法操作，则不加入队排
          if g_Config.boSpeedControl and g_Config.boSendUpdateMsg then
          begin
            PlayObject.SendUpdateMsgA(PlayObject, DefMsg.Ident, DefMsg.Tag, DefMsg.Param, DefMsg.Series, DefMsg.Recog,
              DecodeString(sMsg));
          end
          else if g_Config.boSpeedControl and g_Config.boSpellSendUpdateMsg then
          begin // 使用UpdateMsg 可以防止消息队列里有多个操作
            PlayObject.SendUpdateMsg(PlayObject, DefMsg.Ident, DefMsg.Tag, DefMsg.Param, DefMsg.Series, DefMsg.Recog,
              DecodeString(sMsg));
          end
          else
            PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Tag, DefMsg.Param, DefMsg.Series, DefMsg.Recog,
              DecodeString(sMsg));
        end;
      CM_QUERYUSERNAME, CM_HEROTARGET, // 锁定
      CM_HEROPROTECT, // 守护
      CM_HEROGROUPATTACK:
        begin // 80
          PlayObject.SendMsg(PlayObject, DefMsg.Ident, 0, DefMsg.Recog, DefMsg.Param { x } , DefMsg.Tag { y } , '');
        end;
      CM_CONTINUOUSMAGIC:
        PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param { x } , DefMsg.Tag { y } , ''); // 连击
      CM_DROPITEM, CM_TAKEONITEM, CM_TAKEOFFITEM, CM_PET_DROPITEM, CM_MERCHANTDLGSELECT, CM_MERCHANTQUERYSELLPRICE,
        CM_USERSELLITEM, CM_TradingSELLITEMS, CM_USERBUYITEM, CM_TradingBUYITEM, CM_USERGETDETAILITEM, CM_MASTERBAGTOHEROBAG,
      // 主人包裹物品放到英雄包裹
      CM_HEROBAGTOMASTERBAG, // 英雄包裹物品放到主人包裹
      CM_CREATEGROUP, CM_ADDGROUPMEMBER, CM_DELGROUPMEMBER, CM_USERREPAIRITEM, CM_MERCHANTQUERYREPAIRCOST, CM_DEALTRY,
        CM_DEALADDITEM, CM_DEALDELITEM, CM_USERSTORAGEITEM, CM_USERTAKEBACKSTORAGEITEM,
      // CM_WANTMINIMAP,
      CM_USERMAKEDRUGITEM,
      // CM_GUILDHOME,
      CM_GUILDADDMEMBER, CM_GUILDDELMEMBER, CM_GUILDUPDATENOTICE, CM_GUILDUPDATERANKINFO, CM_UPGRADEDLGITEM,
        CM_CANCELUPGRADEDLGITEM, CM_CHALLENGETRY, // 挑战
      CM_CHALLENGEADDITEM, // 增加挑战物品
      CM_CHALLENGEDELITEM, // 删除挑战物品
      CM_SENDUPGRADEDIALOG, // 包裹宝石升级装备
      CM_OPENBOX, // 开宝箱
      CM_SENDGETBACKHERO, // 选择召回寄存的英雄
      CM_QUERYBAGITEMS:
        begin
          PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DecodeString(sMsg));
        end;
      CM_PASSWORD:
        begin
          PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Param, DefMsg.Recog, DefMsg.Series, DefMsg.Tag, DecodeString(sMsg));
        end;
      CM_ADJUST_BONUS:
        begin // 1043
          PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, sMsg);
        end;
      CM_HORSERUN, CM_TURN, CM_WALK, CM_RUN, CM_HIT, CM_HEAVYHIT, CM_BIGHIT, CM_POWERHIT, CM_LONGHIT, CM_CRSHIT, CM_TWNHIT,
        CM_WIDEHIT, CM_FIREHIT, CM_43HIT, CM_66HIT, CM_66HIT1, // 开天斩轻击 piaoyun 2013-08-24
      CM_SWORDHIT, // 逐日剑法  ID=56
      CM_101HIT, // 三绝杀
      CM_102HIT, // 断岳斩
      CM_103HIT, // 横扫千军
      CM_113HIT, // 断空斩
      CM_115HIT, // 断空斩
      CM_CUSTOM_HIT001 .. (CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1):
        begin
          if g_Config.boSpeedControl and g_Config.boSendUpdateMsg then
          begin
            PlayObject.SendUpdateMsgA(PlayObject, { SendActionMsg }
              DefMsg.Ident, DefMsg.Tag, DefMsg.Param, DefMsg.Series, 0, { DefMsg.Recog, } '');
          end
          else if g_Config.boSpeedControl and g_Config.boActionSendActionMsg then
          begin // 使用UpdateMsg 可以防止消息队列里有多个操作
            PlayObject.SendUpdateMsg(PlayObject, { SendActionMsg } DefMsg.Ident, DefMsg.Tag, DefMsg.Param, DefMsg.Series, 0,
              { DefMsg.Recog, } '');
          end
          else
            PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Tag, DefMsg.Param, DefMsg.Series, 0, { DefMsg.Recog, } '');
        end;
      CM_SAY:
        begin
          sTemp := DecodeString(sMsg);
{$IF LUA_SCRIPT = 1}
          RegisterExistingActor('actor', PlayObject);
          DoEvent(1);
{$ENDIF}
          PlayObject.SendMsg(PlayObject, CM_SAY, 0, 0, 0, 0, sTemp);
        end;
      CM_GETSHOPITEMS:
        PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DefMsg.Series, '');
      CM_GETRANKING, CM_GETMYRANKING:
        PlayObject.SendUpdateMsg(PlayObject, DefMsg.Ident, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DefMsg.Series, '');
      CM_BUYSHOPITEM:
        PlayObject.SendUpdateMsg(PlayObject, DefMsg.Ident, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DefMsg.Series,
          DecodeString(sMsg));
      CM_QUERYSELECTSHOPINFO, CM_QUERYUSERSHOPS:
        PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Param, DefMsg.Recog, DefMsg.Tag, DefMsg.Series, DecodeString(sMsg));
      CM_SEARCHSHOPITEMS, CM_QUERYUSERSHOPITEMS, CM_SENDMOVEMYSHOPITEM, CM_SENDBUYUSERSHOPITEM:
        PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DecodeString(sMsg));
      CM_SENDADDTOMYSHOP, CM_SENDCHANGEMYSHOPITEM:
        PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Param, DefMsg.Recog, DefMsg.Tag, DefMsg.Series, DecodeString(sMsg));
      CM_QUERYMYSHOPSELLINGITEMS, CM_QUERYMYSHOPSELLEDITEMS, CM_QUERYMYSHOPSTORAGEITEMS:
        PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DecodeString(sMsg));
      CM_HEROMAGICKEYCHANGE, // 魔法键
      CM_HEROTAKEONITEM, // 英雄穿装备
      CM_HEROTAKEOFFITEM, // 英雄脱装备
      CM_HEROEAT:
        begin
          if PlayObject.m_MyHero <> nil then
            PlayObject.SendMsg(PlayObject.m_MyHero, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag,
              DecodeString(sMsg)); // 英雄吃药
        end;
      CM_HERODROPITEM:
        begin
          if PlayObject.m_MyHero <> nil then
            PlayObject.SendMsg(PlayObject.m_MyHero, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag,
              DecodeString(sMsg));
        end; // 英雄扔物品
      CM_HEROPACKAGEITEM, CM_HEROOVERLAPITEM:
        begin
          if PlayObject.m_MyHero <> nil then
            PlayObject.SendMsg(PlayObject.m_MyHero, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag,
              DecodeString(sMsg));
        end; // 英雄包裹重叠物品
      CM_PACKAGEITEM, CM_OVERLAPITEM:
        begin
          PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DecodeString(sMsg));
        end; // 重叠物品
      CM_SENDSHOPNAME:
        PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DecodeString(sMsg));
      {
        CM_CLIENTBUFFCLICK: PlayObject.SendMsg(PlayObject,
        DefMsg.Ident,
        DefMsg.Series,
        DefMsg.Recog,
        DefMsg.Param,
        DefMsg.Tag,
        DeCodeString(sMsg));
      }
    else
      PlayObject.SendMsg(PlayObject, DefMsg.Ident, DefMsg.Series, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, sMsg);
    end;

    if PlayObject.m_boReadyRun then
    begin
      case DefMsg.Ident of
        CM_TURN, CM_WALK, CM_SITDOWN, CM_RUN, CM_HIT, CM_HEAVYHIT, CM_BIGHIT, CM_POWERHIT, CM_LONGHIT, CM_WIDEHIT, CM_FIREHIT,
          CM_CRSHIT, CM_TWNHIT, CM_SWORDHIT, CM_43HIT, CM_66HIT, CM_66HIT1, // 开天斩轻击 piaoyun 2013-08-24
        CM_101HIT, // 三绝杀
        CM_102HIT, // 断岳斩
        CM_103HIT, // 横扫千军
        CM_113HIT, // 断空斩
        CM_115HIT, CM_CUSTOM_HIT001 .. (CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1):
          begin
            Dec(PlayObject.m_dwRunTick, 100 { g_Config.dwTurnIntervalTime } );
          end;
      end;
    end;
  except
    MainOutMessage(sExceptionMsg + IntToStr(DefMsg.Ident));
  end;
end;

function TUserEngine.AddDummyObject(sDummyName, sMapName: string; nX, nY: Integer): TDummyObject;
var
  Map: TEnvirnoment;
  Cert: TDummyObject;
  n1C, n20, n24: Integer;
  p28: Pointer;
begin
  Result := nil;
  Cert := nil;
  Map := g_MapManager.FindMap(sMapName);
  if Map = nil then
    Exit;

  Cert := TDummyObject.Create;
  if Cert <> nil then
  begin
    Cert.m_sUserID := 'DummyObj';
    Cert.m_PEnvir := Map;
    Cert.m_OPEnvir := Map;
    Cert.m_sMapName := sMapName;
    Cert.m_nCurrX := nX;
    Cert.m_nCurrY := nY;
    Cert.m_btDirection := Random(8);
    Cert.m_sCharName := sDummyName;
    Cert.m_sOldCharName := sDummyName;
    Cert.m_WAbil := Cert.m_Abil;
    if Random(100) < Cert.m_btCoolEye then
      Cert.m_boCoolEye := True;

    Cert.m_sIPaddr := GetIPAddr;
    Cert.m_sIPLocal := GetIPLocal(Cert.m_sIPaddr);
    Cert.Initialize;
    if Cert.m_boAddtoMapFail then
    begin
      p28 := nil;
      if Cert.m_PEnvir.m_nWidth < 50 then
        n20 := 2
      else
        n20 := 3;

      if Cert.m_PEnvir.m_nHeight < 250 then
      begin
        if Cert.m_PEnvir.m_nHeight < 30 then
          n24 := 2
        else
          n24 := 20;
      end
      else
        n24 := 50;

      n1C := 0;
      while (True) do
      begin
        if not Cert.m_PEnvir.CanWalk(Cert.m_nCurrX, Cert.m_nCurrY, False) then
        begin
          if (Cert.m_PEnvir.m_nWidth - n24 - 1) > Cert.m_nCurrX then
          begin
            Inc(Cert.m_nCurrX, n20);
          end
          else
          begin
            Cert.m_nCurrX := Random(Cert.m_PEnvir.m_nWidth div 2) + n24;
            if Cert.m_PEnvir.m_nHeight - n24 - 1 > Cert.m_nCurrY then
              Inc(Cert.m_nCurrY, n20)
            else
              Cert.m_nCurrY := Random(Cert.m_PEnvir.m_nHeight div 2) + n24;
          end;
        end
        else
        begin
          p28 := Cert.m_PEnvir.AddToMap(Cert.m_nCurrX, Cert.m_nCurrY, Cert);
          Break;
        end;

        Inc(n1C);
        if n1C >= 31 then
          Break;
      end;
      if p28 <> Cert then
        FreeAndNil(Cert);
    end;
  end;
  Result := Cert;
end;

function TUserEngine.CreateBaseObjectByRace(MonName: string; MonRace, nX, nY: Integer): TBaseObject;
var
  Cert: TBaseObject;
  btMonRaceImg, btMonRace: Byte;
  wMonAppr: Word;
  I: Integer;
  CustomMonsterConfig: TCustomMonsterConfig;
begin
  Cert := nil;
  case MonRace of
    11:
      Cert := TSuperGuard.Create;
    12:
      Cert := TMoveSuperGuard.Create;
    20:
      Cert := TArcherPolice.Create;
    30:
      Cert := TBoxMonster.Create; // 可采集怪物 By 一支笔 at:2021-06-16 14:02:10
    51:
      begin
        Cert := TMonster.Create;
        Cert.m_boAnimal := True;
        Cert.m_nMeatQuality := Random(3500) + 3000;
        Cert.m_nBodyLeathery := 50;
      end;
    52:
      begin
        if Random(30) = 0 then
        begin
          Cert := TChickenDeer.Create;
          Cert.m_boAnimal := True;
          Cert.m_nMeatQuality := Random(20000) + 10000;
          Cert.m_nBodyLeathery := 150;
        end
        else
        begin
          Cert := TMonster.Create;
          Cert.m_boAnimal := True;
          Cert.m_nMeatQuality := Random(8000) + 8000;
          Cert.m_nBodyLeathery := 150;
        end;
      end;
    53:
      begin
        Cert := TATMonster.Create;
        Cert.m_boAnimal := True;
        Cert.m_nMeatQuality := Random(8000) + 8000;
        Cert.m_nBodyLeathery := 150;
      end;
    55:
      begin
        Cert := TTrainer.Create;
        Cert.m_btRaceServer := 55;
      end;
    79:
      Cert := TWealthAnimalMon.Create; // 富贵兽 20090517
    99:
      Cert := TMoonObjectEx.Create; // 月灵
    RC_PLAYMOSTER:
      Cert := THumMon.Create; // 人形怪 -- 原来为60
    80:
      Cert := TMonster.Create;
    81:
      Cert := TATMonster.Create;
    82:
      Cert := TSpitSpider.Create;
    83:
      Cert := TSlowATMonster.Create;
    84:
      Cert := TScorpion.Create;
    85:
      Cert := TStickMonster.Create;
    86:
      Cert := TATMonster.Create;
    87:
      Cert := TDualAxeMonster.Create;
    88:
      Cert := TATMonster.Create;
    89:
      Cert := TATMonster.Create;
    90:
      Cert := TGasAttackMonster.Create;
    91:
      Cert := TMagCowMonster.Create;
    92:
      Cert := TCowKingMonster.Create;
    93:
      Cert := TThornDarkMonster.Create;
    94:
      Cert := TLightingZombi.Create;
    95:
      begin
        Cert := TDigOutZombi.Create;
        if Random(2) = 0 then
          Cert.bo2BA := True;
      end;
    96:
      begin
        Cert := TZilKinZombi.Create;
        if Random(4) = 0 then
          Cert.bo2BA := True;
      end;
    97:
      begin
        Cert := TCowMonster.Create;
        if Random(2) = 0 then
          Cert.bo2BA := True;
      end;
    100:
      Cert := TWhiteSkeleton.Create;
    101:
      begin // 祖玛雕像
        Cert := TScultureMonster.Create;
        Cert.bo2BA := True;
      end;
    102:
      Cert := TScultureKingMonster.Create; // 祖玛教主
    103:
      Cert := TBeeQueen.Create;
    104:
      Cert := TArcherMonster.Create;
    105:
      Cert := TGasMothMonster.Create; // 楔蛾
    106:
      Cert := TGasDungMonster.Create;
    107:
      Cert := TCentipedeKingMonster.Create; // 触龙神
    108:
      Cert := TDevilkingMonster.Create; // 魔王岭怪物
    109:
      Cert := TDevilkingArcherGuard.Create; // 魔王岭弓箭手
    110:
      Cert := TCastleDoor.Create; // 沙巴克的 城门
    111:
      Cert := TWallStructure.Create; // 沙巴克的 左墙,中，右
    112:
      Cert := TArcherGuard.Create; // 弓箭手
    142:
      Cert := TMoveArcherGuard.Create;
    113:
      Cert := TElfMonster.Create;
    114:
      Cert := TElfWarriorMonster.Create;
    115:
      Cert := TBigHeartMonster.Create;
    116:
      Cert := TSpiderHouseMonster.Create;
    117:
      Cert := TExplosionSpider.Create;
    118:
      Cert := THighRiskSpider.Create;
    119:
      Cert := TBigPoisionSpider.Create;
    120:
      Cert := TSoccerBall.Create;
    121:
      Cert := TCobwebMonster.Create; // 蜘蛛网攻击
    122:
      Cert := TMagicAttackMonster.Create; // 魔法攻击的怪物      //不近身攻击怪物
    123:
      Cert := TTwoKindAttackMonster.Create; // 有二种不同攻击方式的怪物
    124:
      Cert := TExplosionAttackMonster.Create; // 冰咆哮怪物
    125:
      Cert := TExtinguishDayFireAttackMonster.Create; // 灭天火怪物 吸蓝
    126:
      Cert := TFireIceAttackMonster.Create; // 火焰冰怪物   //推动目标
    127:
      Cert := TIcePeakMonster.Create; // 雪域卫士 冰峰效果
    128:
      Cert := TTruckMonster.Create; // 押镖车
    129:
      Cert := TFoxMonster.Create; // 狐狸物理攻击
    130:
      Cert := TStoneFoxMonster.Create; // 狐狸物理攻击  石化攻击
    131:
      Cert := TFoxMagicAttackMonster.Create; // 狐狸魔法攻击
    132:
      Cert := TDamageSpellAttackMonster.Create; // 狐狸魔法攻击  吸蓝
    133:
      Cert := TDamageArmorAttackMonster.Create; // 狐狸魔法攻击  减防御
    134:
      Cert := TAnimalObject.Create; // 不攻击 不移动怪物
    135:
      Cert := TMeteoriteRainAttackMonster.Create; // 流星火雨怪物 怪物不能移动
    136:
      Cert := TMagicAttackNotMoveMonster.Create; // 怪物不能移动  魔法远程攻击 真狐月天珠
    137:
      Cert := TFireSpiritMonster.Create; // 火灵
    138:
      Cert := TLionMonster.Create; // 狮子
    139:
      Cert := TFireCrossMonster.Create; // 火墙怪物
    140:
      Cert := TElfMonster.Create; // 圣兽
    141:
      Cert := TElfWarriorMonster.Create; // 圣兽
    144:
      Cert := TFireDragon.Create; // 火龙教主 piaoyun 2013-08-19
    145:
      Cert := TFireDragonGuard.Create; // 火龙守护者 piaoyun 2013-08-19
    146:
      Cert := TDevilBat.Create; // 恶魔蝙蝠 piaoyun 2013-08-19
    147:
      Cert := TIcicleMonster.Create; // 冰柱怪物 piaoyun 2013-11-16
    148:
      Cert := TTortoiseMonster.Create; // 乌龟怪物 piaoyun 2013-11-16
    149:
      Cert := TMon36_XMonster.Create; // Mon36_X怪物
    151:
      Cert := TMagicAttackNotMoveMonster2.Create; // 怪物不能移动  魔法远程攻击 真狐月天珠
    158:
      Cert := TExperienceMon.Create;
    199:
      Cert := TMon35_2Monster.Create; // Mon35_2
    200:
      Cert := TElectronicScolpionMon.Create;
    201:
      Cert := TMon38_0Monster.Create; // Mon38-0 piaoyun 2014-01-03
    202:
      Cert := TMon38_11Monster.Create; // Mon38-11 piaoyun 2014-01-03
    203:
      Cert := TMon38_12Monster.Create; // Mon38-13 piaoyun 2014-01-03
    204:
      Cert := TMon38_13Monster.Create; // Mon38-13 piaoyun 2014-01-03
    205:
      Cert := TXueLingLeader.Create; // 血灵教主 chongchong 2014-09-10
    206:
      Cert := TLineMagicAttackMonster.Create; // 直线魔法攻击怪物
    207:
      Cert := TMLSBAttackMonster.Create; // 魔龙石碑怪物
    230:
      Cert := TGuardMonster.Create;
    231:
      begin
        Cert := TGuardMonster.Create;
        TGuardMonster(Cert).CanMoveMode := True;
      end;
    // 自定义怪物 chongchong 2014-07-19
    154 { 魔王岭怪物 } , 155 { 魔王岭宝宝 } , 156 { 普通怪物 } , 157 { 不主动攻击，可挖尸体怪物 } , 159 { 采集怪 } :
      begin
        if GetMonRaceImgAndAppr(MonName, btMonRaceImg, wMonAppr, btMonRace) and (btMonRaceImg = 156) then
        begin
{$IF MULTI_THREAD = 1}
          if g_MultiThreadRun then
            m_CustomMonsterList.LockR(3);
          try
{$IFEND}
            for I := 0 to m_CustomMonsterList.Count - 1 do
            begin
              CustomMonsterConfig := m_CustomMonsterList.Items[I];
              if CustomMonsterConfig.MonsterAppr = wMonAppr then
              begin
                Cert := TCustomMonster.Create(CustomMonsterConfig, Point(nX, nY));
                // 可挖尸体 chongchong 2017-06-25
                if MonRace = 157 then
                begin
                  Cert.m_boAnimal := True;
                  Cert.m_nMeatQuality := Random(3500) + 3000;
                  Cert.m_nBodyLeathery := 50;
                end;
                Break;
              end;
            end;
{$IF MULTI_THREAD = 1}
          finally
            if g_MultiThreadRun then
              m_CustomMonsterList.UnLockR;
          end;
{$IFEND}
        end;
      end;
  end;
  Result := Cert;
  if Result <> nil then
  begin
    Result.m_sCharName := MonName;
    Result.m_sOldCharName := MonName;
  end;
end;

function TUserEngine.AddBaseObject(Map: TEnvirnoment; nX, nY: Integer; nMonRace: Integer; sMonName: string): TBaseObject;
var
  Cert: TBaseObject;
  n1C, n20, n24: Integer;
  p28: Pointer;
  nCode: Integer;
begin
  Result := nil;
  if Map = nil then
    Exit;

  nCode := 0;
  try
    Cert := CreateBaseObjectByRace(sMonName, nMonRace, nX, nY);
    nCode := 1;
    if Cert <> nil then
    begin
      nCode := 2;
      MonInitialize(Cert, sMonName);
      nCode := 3;
      Cert.m_PEnvir := Map;
      Cert.m_sMapName := Map.sMapName;
      Cert.m_nCurrX := nX;
      Cert.m_nCurrY := nY;
      Cert.m_btDirection := Random(8);
      Cert.m_sCharName := sMonName;
      Cert.m_sOldCharName := sMonName;
      Cert.m_WAbil := Cert.m_Abil;
      if Random(100) < Cert.m_btCoolEye then
        Cert.m_boCoolEye := True;

      nCode := 4;
      SetMonIcons(Cert);

      nCode := 5;
      Cert.m_boMonGetRandomItems := True;
      Cert.Initialize();
      nCode := 6;
      // 月灵 chongchong 2014-12-06
      if Cert.m_btRaceServer = 99 then
        TMoonObjectEx(Cert).m_nWalkSpeedDB := Cert.m_nWalkSpeed;

      nCode := 7;
      if Cert.m_boAddtoMapFail then
      begin
        nCode := 8;
        p28 := nil;
        if Cert.m_PEnvir.m_nWidth < 50 then
          n20 := 2
        else
          n20 := 3;
        nCode := 9;
        if (Cert.m_PEnvir.m_nHeight < 250) then
        begin
          if (Cert.m_PEnvir.m_nHeight < 30) then
            n24 := 2
          else
            n24 := 20;
        end
        else
          n24 := 50;
        n1C := 0;
        while (True) do
        begin
          nCode := 10;
          if not Cert.m_PEnvir.CanWalk(Cert.m_nCurrX, Cert.m_nCurrY, False) then
          begin
            if (Cert.m_PEnvir.m_nWidth - n24 - 1) > Cert.m_nCurrX then
            begin
              Inc(Cert.m_nCurrX, n20);
            end
            else
            begin
              Cert.m_nCurrX := Random(Cert.m_PEnvir.m_nWidth div 2) + n24;
              if Cert.m_PEnvir.m_nHeight - n24 - 1 > Cert.m_nCurrY then
              begin
                Inc(Cert.m_nCurrY, n20);
              end
              else
              begin
                Cert.m_nCurrY := Random(Cert.m_PEnvir.m_nHeight div 2) + n24;
              end;
            end;
          end
          else
          begin
            nCode := 11;
            p28 := Cert.m_PEnvir.AddToMap(Cert.m_nCurrX, Cert.m_nCurrY, Cert);
            Break;
          end;
          Inc(n1C);
          if n1C >= 46 then
            Break;
        end;
        nCode := 12;
        // 怪物怎么样都搞不上，随机给个坐标算了 chongchong 2018-07-06 15:05:18
        if p28 <> Cert then
        begin
          for n1C := 0 to 3 do
          begin
            Cert.m_nCurrX := Random(Cert.m_PEnvir.m_nWidth);
            Cert.m_nCurrY := Random(Cert.m_PEnvir.m_nHeight);
            nCode := 13;
            if Cert.m_PEnvir.CanWalk(Cert.m_nCurrX, Cert.m_nCurrY, False) then
            begin
              nCode := 14;
              p28 := Cert.m_PEnvir.AddToMap(Cert.m_nCurrX, Cert.m_nCurrY, Cert);
              Break;
            end;
          end;
        end;
        nCode := 15;
        if p28 <> Cert then
        begin
          nCode := 16;
          Cert.Free;
          Cert := nil;
        end;
      end;
    end;
    nCode := 17;
    if (Cert <> nil) and (Cert.m_btRaceServer = RC_PLAYMOSTER) then
    begin
      THumMon(Cert).m_nHomeX := Cert.m_nCurrX;
      THumMon(Cert).m_nHomeY := Cert.m_nCurrY;
    end;
    nCode := 18;
    if (Cert <> nil) then
    begin
      Cert.m_nInitX := Cert.m_nCurrX; // 初始坐标X chongchong 2017-04-15
      Cert.m_nInitY := Cert.m_nCurrY; // 初始坐标Y chongchong 2017-04-15
    end;
    nCode := 19;
    Result := Cert;
    if (Cert <> nil) and (Map.m_boFB) then
    begin
      Map.m_FBMonsterList.Add(Cert);
    end;
  except
    on E: Exception do
    begin
      MainOutMessage('TUserEngine.AddBaseObject Error, Code = ' + IntToStr(nCode) + ';' + E.Message);
    end;
  end;
end;

// ====================================================
// 功能:创建怪物对象
// 返回值：在指定时间内创建完对象，则返加TRUE，如果超过指定时间则返回FALSE
// ====================================================
function TUserEngine.RegenMonsters(MonGen: pTMonGenInfo; nCount: Integer): Boolean;
var
  dwStartTick: LongWord;
  nTempX, nTempY: Integer;
  nX: Integer;
  nY: Integer;
  I: Integer;
  Cert: TBaseObject;
  Map: TEnvirnoment;
resourcestring
  sExceptionMsg = '[Exception] TUserEngine.RegenMonsters';
begin
  Result := True;
  dwStartTick := MyGetTickCount();
  try
    if (MonGen <> nil) and (MonGen.nRace > 0) and (nCount > 0) then
    begin
      if MonGen.Envir <> nil then
        Map := MonGen.Envir
      else
        Map := g_MapManager.FindMap(MonGen.sMapName);
      if Map <> nil then
      begin
        if (MonGen.nMissionGenRate > 0) and (Random(100) < MonGen.nMissionGenRate) then
        begin
          nTempX := (MonGen.nX - MonGen.nRange) + Random(MonGen.nRange * 2 + 1);
          nTempY := (MonGen.nY - MonGen.nRange) + Random(MonGen.nRange * 2 + 1);
          nX := (nTempX - 10) + Random(20);
          nY := (nTempY - 10) + Random(20);
          for I := 0 to nCount - 1 do
          begin
            Cert := AddBaseObject(Map, nX, nY, MonGen.nRace, MonGen.sMonName);
            if Cert <> nil then
            begin
              Cert.m_boISNGMonster := MonGen.boIsNGMon; // 是否是内功怪
              Cert.m_btNameColor := MonGen.btNameColor;
              Cert.m_btNation := StrToIntDef(MonGen.sNationaID, 0); // GetNationIndex(
              Cert.m_boCanAttackSameNationPlayer := MonGen.boCanAttackSameNationPlayer; // 是否攻击同国家玩家
              Cert.m_boAllowSameNationPlayerAttack := MonGen.boAllowSameNationPlayerAttack; // 是否可被同国家的人物攻击
              Cert.m_boNoSameNationMonPK := MonGen.boNoSameNationMonPK;
              MonGen.CertList.Add(Cert);
              if (MonGen.TriggerScript <> '') and (g_FunctionNPC <> nil) and (g_GrobalPlayer <> nil) then
              begin
                TPlayObject(g_GrobalPlayer).m_sRegMonName := MonGen.sMonName;
                TPlayObject(g_GrobalPlayer).m_sRegMonMap := Map.sMapName;
                TPlayObject(g_GrobalPlayer).m_sRegMonMapDesc := Map.sMapDesc;
                TPlayObject(g_GrobalPlayer).m_nRegMonX := nX;
                TPlayObject(g_GrobalPlayer).m_nRegMonY := nY;
                TPlayObject(g_GrobalPlayer).m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(TPlayObject(g_GrobalPlayer), MonGen.TriggerScript, False);
                TPlayObject(g_GrobalPlayer).m_sRegMonName := '';
                TPlayObject(g_GrobalPlayer).m_sRegMonMap := '';
                TPlayObject(g_GrobalPlayer).m_sRegMonMapDesc := '';
                TPlayObject(g_GrobalPlayer).m_nRegMonX := 0;
                TPlayObject(g_GrobalPlayer).m_nRegMonY := 0;
              end;
            end;
            {
              if (MonGen.dwStartTick = 0) and (nCount > nRegCount) then
              Result := False
              else
            }
            if (MyGetTickCount - dwStartTick > g_dwZenLimit) then
            begin
              { if nCount > nRegCount then } Result := False;
              Break;
            end;
          end;
        end
        else
        begin
          for I := 0 to nCount - 1 do
          begin
            nX := (MonGen.nX - MonGen.nRange) + Random(MonGen.nRange * 2 + 1);
            nY := (MonGen.nY - MonGen.nRange) + Random(MonGen.nRange * 2 + 1);
            if nX < 0 then
              nX := 0
            else if nX > Map.m_nWidth - 1 then
              nX := Map.m_nWidth - 1;
            if nY < 0 then
              nY := 0
            else if nY > Map.m_nHeight - 1 then
              nY := Map.m_nHeight - 1;
            Cert := AddBaseObject(Map, nX, nY, MonGen.nRace, MonGen.sMonName);
            if Cert <> nil then
            begin
              Cert.m_boISNGMonster := MonGen.boIsNGMon; // 是否是内功怪
              Cert.m_btNameColor := MonGen.btNameColor;
              Cert.m_btNation := StrToIntDef(MonGen.sNationaID, 0); // GetNationIndex(
              Cert.m_boCanAttackSameNationPlayer := MonGen.boCanAttackSameNationPlayer; // 是否攻击同国家玩家
              Cert.m_boAllowSameNationPlayerAttack := MonGen.boAllowSameNationPlayerAttack; // 是否可被同国家的人物攻击
              Cert.m_boNoSameNationMonPK := MonGen.boNoSameNationMonPK;
              MonGen.CertList.Add(Cert);
              if (MonGen.TriggerScript <> '') and (g_FunctionNPC <> nil) and (g_GrobalPlayer <> nil) then
              begin
                TPlayObject(g_GrobalPlayer).m_sRegMonName := MonGen.sMonName;
                TPlayObject(g_GrobalPlayer).m_sRegMonMap := Map.sMapName;
                TPlayObject(g_GrobalPlayer).m_sRegMonMapDesc := Map.sMapDesc;
                TPlayObject(g_GrobalPlayer).m_nRegMonX := nX;
                TPlayObject(g_GrobalPlayer).m_nRegMonY := nY;
                TPlayObject(g_GrobalPlayer).m_nScriptGotoCount := 0;
                g_FunctionNPC.GotoLable(TPlayObject(g_GrobalPlayer), MonGen.TriggerScript, False);
                TPlayObject(g_GrobalPlayer).m_sRegMonName := '';
                TPlayObject(g_GrobalPlayer).m_sRegMonMap := '';
                TPlayObject(g_GrobalPlayer).m_sRegMonMapDesc := '';
                TPlayObject(g_GrobalPlayer).m_nRegMonX := 0;
                TPlayObject(g_GrobalPlayer).m_nRegMonY := 0;
              end;
            end;
            {
              if (MonGen.dwStartTick = 0) and (nCount > nRegCount) then
              Result := False
              else
            }
            if (MyGetTickCount - dwStartTick > g_dwZenLimit) then
            begin
              { if nCount > nRegCount then } Result := False;
              Break;
            end;
          end;
        end;
      end;
    end;
  except
    MainOutMessage(sExceptionMsg);
  end;
end;

function TUserEngine.GetPlayObject(sName: string): TPlayObject;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := nil;
  m_PlayObjectList.LockR(10);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      if CompareText(m_PlayObjectList.Strings[I], sName) = 0 then
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if PlayObject <> nil then
        begin
          if (not PlayObject.m_boGhost) then
          begin
            if not(PlayObject.m_boPasswordLocked and PlayObject.m_boObMode and PlayObject.m_boAdminMode) then
              Result := PlayObject;
          end;
          Break;
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetPlayObject(PlayObject: TObject): TPlayObject;
var
  I: Integer;
begin
  Result := nil;
  m_PlayObjectList.LockR(11);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      if m_PlayObjectList.Objects[I] = PlayObject then
      begin
        Result := TPlayObject(m_PlayObjectList.Objects[I]);
        Break;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetDummyObjectCount(): Integer;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := 0;
  m_PlayObjectList.LockR(12);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if PlayObject.m_boDummyObject then
        begin
          Inc(Result);
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetDummyObjectCount(Envir: TEnvirnoment): Integer;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := 0;
  m_PlayObjectList.LockR(13);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if (PlayObject.m_PEnvir = Envir) and PlayObject.m_boDummyObject then
        begin
          Inc(Result);
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetReallyCount(): Integer;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := 0;
  m_ReallyPlayObjectList.LockR(100);
  try
    for I := m_ReallyPlayObjectList.Count - 1 downto 0 do
    begin
      PlayObject := TPlayObject(m_ReallyPlayObjectList[I]);
      if PlayObject <> nil then
      begin
        if (not PlayObject.m_boOffLine) and (not PlayObject.m_boDummyObject) then
        begin
          Inc(Result);
        end;
      end;
    end;
  finally
    m_ReallyPlayObjectList.UnLockR;
  end;
end;

{
  function TUserEngine.GetReallyCount_NoLock(): Integer;
  var
  I: Integer;
  PlayObject: TPlayObject;
  begin
  Result := 0;
  for I := m_PlayObjectList.Count - 1 downto 0 do
  begin
  PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
  if PlayObject <> nil then
  begin
  if (not PlayObject.m_boOffline) and (not PlayObject.m_boDummyObject) then
  begin
  Inc(Result);
  end;
  end;
  end;
  end;
}
function TUserEngine.GetOfflineCount(): Integer;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := 0;
  m_PlayObjectList.LockR(15);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if PlayObject.m_boOffLine then
        begin
          Inc(Result);
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetPlayObjectOfAccount(sAccount: string): TPlayObject;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := nil;
  m_PlayObjectList.LockR(16);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if (CompareText(PlayObject.m_sUserID, sAccount) = 0) then
        begin
          Result := PlayObject;
          Break;
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetPlayObjectExOfOffLine(sAccount: string): TPlayObject; // 获取离线挂人物
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := nil;
  m_PlayObjectList.LockR(19);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if (CompareText(PlayObject.m_sUserID, sAccount) = 0) then
        begin
          if PlayObject.m_boOffLine then
          begin
            Result := PlayObject;
            Break;
          end;
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetPlayObjectExOfOffLineEx(sAccount, sChrName: string): TPlayObject; // 获取离线挂人物
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := nil;
  m_PlayObjectList.LockR(19);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if SameText(PlayObject.m_sUserID, sAccount) and SameText(PlayObject.m_sCharName, sChrName) then
        begin
          if PlayObject.m_boOffLine then
          begin
            Result := PlayObject;
            Break;
          end;
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetHeroObject(sName: string): THeroObject;
var
  I: Integer;
begin
  Result := nil;
  m_HeroObjectList.LockR(5);
  try
    for I := 0 to m_HeroObjectList.Count - 1 do
    begin
      if (CompareText(m_HeroObjectList.Strings[I], sName) = 0) then
      begin
        Result := THeroObject(m_HeroObjectList.Objects[I]);
        Break;
      end;
    end;
  finally
    m_HeroObjectList.UnLockR;
  end;
end;

function TUserEngine.FindMerchant(Merchant: TObject): TMerchant;
var
  I: Integer;
  obj: TObject;
begin
  Result := nil;
  m_MerchantList.LockR(5);
  try
    for I := 0 to m_MerchantList.Count - 1 do
    begin
      obj := m_MerchantList.Items[I];
      if (obj <> nil) and (obj = Merchant) and (not TMerchant(obj).m_boGhost) then
      begin
        Result := TMerchant(m_MerchantList.Items[I]);
        Break;
      end;
    end;
  finally
    m_MerchantList.UnLockR;
  end;
end;

function TUserEngine.FindMerchantByName(const MapName: string; const MerchantName: string): TMerchant;
var
  I: Integer;
  Merchant: TMerchant;
begin
  Result := nil;
  m_MerchantList.LockR(6);
  try
    for I := 0 to m_MerchantList.Count - 1 do
    begin
      Merchant := m_MerchantList.Items[I];
      if (Merchant <> nil) and SameText(Merchant.m_sMapName, MapName) and SameText(Merchant.m_sCharName, MerchantName) then
      begin
        Result := Merchant;
        Break;
      end;
    end;
  finally
    m_MerchantList.UnLockR;
  end;
end;

function TUserEngine.FindMerchantByPosition(const MapName: string; const X, Y: Integer): TMerchant;
var
  I: Integer;
  Merchant: TMerchant;
begin
  Result := nil;
  m_MerchantList.LockR(7);
  try
    for I := 0 to m_MerchantList.Count - 1 do
    begin
      Merchant := m_MerchantList.Items[I];
      if (Merchant <> nil) and SameText(Merchant.m_sMapName, MapName) and (Merchant.m_nCurrX = X) and (Merchant.m_nCurrY = Y) then
      begin
        Result := Merchant;
        Break;
      end;
    end;
  finally
    m_MerchantList.UnLockR;
  end;
end;

function TUserEngine.FindNPC(NPC: TObject): TNormNpc;
var
  I: Integer;
begin
  Result := nil;
  QuestNPCList.LockR(3);
  try
    for I := 0 to QuestNPCList.Count - 1 do
    begin
      if (QuestNPCList.Objects[I] <> nil) //
        and (QuestNPCList.Objects[I] = NPC) //
        and (not TNormNpc(QuestNPCList.Objects[I]).m_boGhost) then
      begin
        Result := TNormNpc(QuestNPCList.Objects[I]);
        Break;
      end;
    end;
  finally
    QuestNPCList.UnLockR;
  end;
end;

function TUserEngine.FindNPC(const ANpcName: string): TNormNpc; // 优化价值怪物死亡触发脚本过多创建NPC的问题 Cursor 2023-06-14 08:55:17
var
  I: Integer;
begin
  Result := nil;
  I := QuestNPCList.IndexOf(ANpcName);
  if I >= 0 then
    Result := TNormNpc(QuestNPCList.Objects[I]);
end;

function TUserEngine.FindNPCByName(const MapName: string; const NPCName: string): TNormNpc;
var
  I: Integer;
  NPC: TNormNpc;
begin
  Result := nil;
  QuestNPCList.LockR(6);
  try
    for I := 0 to QuestNPCList.Count - 1 do
    begin
      NPC := TNormNpc(QuestNPCList.Objects[I]);

      if (NPC <> nil) //
        and SameText(NPC.m_sMapName, MapName) //
        and SameText(NPC.m_sCharName, NPCName) then
      begin
        Result := NPC;
        Break;
      end;
    end;
  finally
    QuestNPCList.UnLockR;
  end;
end;

function TUserEngine.FindNPCByPosition(const MapName: string; const X, Y: Integer): TNormNpc;
var
  I: Integer;
  NPC: TNormNpc;
begin
  Result := nil;
  QuestNPCList.LockR(6);
  try
    for I := 0 to QuestNPCList.Count - 1 do
    begin
      NPC := TNormNpc(QuestNPCList.Objects[I]);
      if (NPC <> nil) and SameText(NPC.m_sMapName, MapName) and (NPC.m_nCurrX = X) and (NPC.m_nCurrY = Y) then
      begin
        Result := NPC;
        Break;
      end;
    end;
  finally
    QuestNPCList.UnLockR;
  end;
end;

function TUserEngine.GetMapOfRangeHumanCount(Envir: TEnvirnoment; nX, nY, nRange: Integer): Integer;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := 0;
  m_PlayObjectList.LockR(20);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if not PlayObject.m_boGhost and (PlayObject.m_PEnvir = Envir) then
        begin
          if (abs(PlayObject.m_nCurrX - nX) < nRange) and (abs(PlayObject.m_nCurrY - nY) < nRange) then
            Inc(Result);
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetHumPermission(sUserName: string; var sIPaddr: string; var btPermission: Byte): Boolean; // 4AE590
var
  I: Integer;
  AdminInfo: pTAdminInfo;
begin
  Result := False;
  btPermission := g_Config.nStartPermission;
  m_AdminList.LockR(2);
  try
    for I := 0 to m_AdminList.Count - 1 do
    begin
      AdminInfo := m_AdminList.Items[I];
      if AdminInfo <> nil then
      begin
        if CompareText(AdminInfo.sChrName, sUserName) = 0 then
        begin
          btPermission := AdminInfo.nLv;
          sIPaddr := AdminInfo.sIPaddr;
          Result := True;
          Break;
        end;
      end;
    end;
  finally
    m_AdminList.UnLockR;
  end;
end;

procedure TUserEngine.AddLoadOffline(PlayObject: TPlayObject; sAccount, sChrName, sIPaddr, sMachineID, sUserMachineID: string;
  boFlag: Boolean; nSessionID, nPayMent, nPayMode, nSoftVersionDate, nSocket, nGSocketIdx, nGateIdx, nKey: Integer;
  nClientWidth, nClientHeight: Integer; nClientBuildVer: Integer; sPromotionFlag: string);
var
  DBLoadHumanResult: PTDBLoadHumanResult;
  LoadHuman: PTDBLoadHuman;
begin
  m_LoadOfflineList.LockW(2);
  try
    New(DBLoadHumanResult);
    FillChar(DBLoadHumanResult^, SizeOf(TDBLoadHumanResult), #0);
    PlayObject.DealCancelA();
    PlayObject.ChallengeCancelA();
    PlayObject.MakeSaveRcd(@DBLoadHumanResult.Data);
    SaveHumRecord(PlayObject);
    PlayObject.m_dwSaveRcdTick := MyGetTickCount();
    PlayObject.MakeGhost;
    LoadHuman := @DBLoadHumanResult.LoadUser;
    LoadHuman.sAccount := sAccount;
    LoadHuman.sHumanName := sChrName;
    LoadHuman.sIPaddr := sIPaddr;
    LoadHuman.nSessionID := nSessionID;
    LoadHuman.nSoftVersionDate := nSoftVersionDate;
    LoadHuman.nPayMent := nPayMent;
    LoadHuman.nPayMode := nPayMode;
    LoadHuman.nSocket := nSocket;
    LoadHuman.nGSocketIdx := nGSocketIdx;
    LoadHuman.nGateIdx := nGateIdx;
    LoadHuman.nKey := nKey;
    LoadHuman.boOffLine := False;
    LoadHuman.sMachineID := sMachineID;
    LoadHuman.sUserMachineID := sUserMachineID;
    LoadHuman.nClientWidth := nClientWidth;
    LoadHuman.nClientHeight := nClientHeight;
    LoadHuman.nClientBuildVer := nClientBuildVer;
    LoadHuman.sPromotionFlag := sPromotionFlag;
    m_LoadOfflineList.AddObject(DBLoadHumanResult.LoadUser.sHumanName, TObject(DBLoadHumanResult));
  finally
    m_LoadOfflineList.UnLockW();
  end;
end;

procedure TUserEngine.AddDBResult(DBResult: PTDBResult);
begin
  m_DBResultList.LockW(4);
  try
    m_DBResultList.Add(DBResult);
  finally
    m_DBResultList.UnLockW;
  end;
end;

(*
  procedure TUserEngine.AddDBLoadHumanResult(DBLoadHumanResult: pTDBLoadHumanResult);
  begin
  m_LoadPlayList.LockW(2);
  try
  m_LoadPlayList.AddObject(DBLoadHumanResult.LoadUser.sHumanName, TObject(DBLoadHumanResult));
  finally
  m_LoadPlayList.UnLockW;
  end;
  end;
  procedure TUserEngine.AddDBLoadHeroResult(DBLoadHeroResult: pTDBLoadHeroResult);
  begin
  m_LoadHeroList.LockW(2);
  try
  m_LoadHeroList.AddObject(DBLoadHeroResult.LoadUser.sHeroName1, TObject(DBLoadHeroResult));
  finally
  m_LoadHeroList.UnLockW;
  end;
  end;
  procedure TUserEngine.AddDBHeroOtherResult(DBHeroOtherResult: PTDBHeroOtherResult);
  begin
  m_OtherHeroList.LockW(2);
  try
  m_OtherHeroList.AddObject(DBHeroOtherResult.LoadUser.sHeroName1, TObject(DBHeroOtherResult));
  finally
  m_OtherHeroList.UnLockW;
  end;
  end;
  procedure TUserEngine.AddDBChrRenameResult(DBRenameChrResult: PTDBRenameChrResult);
  begin
  m_OtherHeroList.LockW(2);
  try
  //m_OtherHeroList.AddObject(DBHeroOtherResult.LoadUser.sHeroName1, TObject(DBHeroOtherResult));
  finally
  m_OtherHeroList.UnLockW;
  end;
  end;
*)
procedure TUserEngine.KickOnlineUser(sChrName: string);
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(21);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if CompareText(PlayObject.m_sCharName, sChrName) = 0 then
      begin
        PlayObject.m_boKickFlag := True;
        Break;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.KickOnlineHero(sChrName: string);
var
  I: Integer;
  HeroObject: THeroObject;
begin
  for I := 0 to m_HeroObjectList.Count - 1 do
  begin
    HeroObject := THeroObject(m_HeroObjectList.Objects[I]);
    if CompareText(HeroObject.m_sCharName, sChrName) = 0 then
    begin
      HeroObject.LogOut;
      Break;
    end;
  end;
end;

procedure TUserEngine.SaveHumRecord(PlayObject: TPlayObject);
var
  DBSaveHuman: TDBSaveHuman;
begin
  if PlayObject.m_boDummyObject then
    Exit;
  FillChar(DBSaveHuman, SizeOf(TDBSaveHuman), #0);
  DBSaveHuman.nSessionID := PlayObject.m_nSessionID;
  DBSaveHuman.PlayObject := Int64(PlayObject);
  PlayObject.MakeSaveRcd(@DBSaveHuman.Data);
  if PlayObject.m_boReconnection then
    DataEngine.SaveHumanData(@DBSaveHuman)
  else
    DataEngine.SaveHumanData(@DBSaveHuman, True);
end;

procedure TUserEngine.SaveHeroRecord(HeroObject: THeroObject);
var
  DBSaveHero: TDBSaveHero;
begin
  if HeroObject.m_boDummyObject then
    Exit;
  FillChar(DBSaveHero, SizeOf(TDBSaveHero), #0);
  DBSaveHero.nSessionID := HeroObject.m_nSessionID;
  DBSaveHero.PlayObject := Int64(HeroObject);
  HeroObject.MakeSaveRcd(@DBSaveHero.Data);
  DataEngine.SaveHeroData(@DBSaveHero);
end;

procedure TUserEngine.AddToHumanFreeList(PlayObject: TPlayObject);
begin
  PlayObject.m_dwGhostTick := MyGetTickCount();
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_PlayObjectFreeList.LockW(2);
  try
{$IFEND}
    m_PlayObjectFreeList.Add(PlayObject);
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_PlayObjectFreeList.UnLockW;
  end;
{$IFEND}
end;

procedure TUserEngine.AddToHeroFreeList(HeroObject: THeroObject);
begin
  HeroObject.m_dwGhostTick := MyGetTickCount();
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_HeroObjectFreeList.LockW(2);
  try
{$IFEND}
    m_HeroObjectFreeList.Add(HeroObject);
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_HeroObjectFreeList.UnLockW;
  end;
{$IFEND}
end;

procedure TUserEngine.GetHeroData(HeroObject: THeroObject; HeroData: PTHeroData);
  function ChangeContinuousMagic(btJob: Byte; wMagicID: Word): Word;
  begin
    Result := wMagicID;
    case btJob of
      0:
        begin
          case wMagicID of
            107:
              Result := 101; // 双龙破->三绝杀
            104:
              Result := 100; // 凤舞祭->追心刺
            105:
              Result := 102; // 惊雷爆->断岳斩
            106:
              Result := 103; // 冰天雪地->横扫千军
            108:
              Result := 101; // 虎啸绝->三绝杀
            109:
              Result := 100; // 八卦掌->追心刺
            110:
              Result := 102; // 三焰咒->断岳斩
            111:
              Result := 103; // 万剑归宗->横扫千军
          end;
        end;
      1:
        begin
          case wMagicID of
            101:
              Result := 107; // 三绝杀->双龙破
            100:
              Result := 104; // 追心刺->凤舞祭
            102:
              Result := 105; // 断岳斩->惊雷爆
            103:
              Result := 106; // 横扫千军->冰天雪地
            108:
              Result := 107; // 虎啸绝->三绝杀
            109:
              Result := 104; // 八卦掌->追心刺
            110:
              Result := 105; // 三焰咒->断岳斩
            111:
              Result := 106; // 万剑归宗->横扫千军
          end;
        end;
      2:
        begin
          case wMagicID of
            101:
              Result := 108; // 三绝杀->虎啸绝
            100:
              Result := 109; // 追心刺->八卦掌
            102:
              Result := 110; // 断岳斩->三焰咒
            103:
              Result := 111; // 横扫千军->万剑归宗
            107:
              Result := 108; // 双龙破->虎啸绝
            104:
              Result := 109; // 凤舞祭->八卦掌
            105:
              Result := 110; // 惊雷爆->三焰咒
            106:
              Result := 111; // 冰天雪地->万剑归宗
          end;
        end;
    end;
  end;

var
  BagItems: pTBagItems;
  UserItem: pTUserItem;
  HumMagics: pTHumMagics;
  HumNGMagics: pTHumNGMagics;
  HumContinuousMagics: pTHumContinuousMagics;
  UserMagic: PTUserMagic;
  MagicInfo: pTMagic;
  I: Integer;
begin
  HeroObject.m_sCharName := Trim(HeroData.sChrName);
  HeroObject.m_sMapName := Trim(HeroData.sCurMap);
  HeroObject.m_nCurrX := HeroData.wCurX;
  HeroObject.m_nCurrY := HeroData.wCurY;
  HeroObject.m_btDirection := HeroData.btDir;
  if not(HeroObject.m_btDirection in [DR_UP .. DR_UPLEFT]) then
  begin
    HeroObject.m_btDirection := DR_UP;
  end;
  HeroObject.m_btHair := HeroData.btHair;
  HeroObject.m_btGender := HeroData.btSex;
  HeroObject.m_btJob := HeroData.btJob;
  if (g_nKey_HeroExt = 0) and (HeroData.btStatus = 3) then
    HeroObject.m_btAttackMode := 0
  else
    HeroObject.m_btAttackMode := HeroData.btStatus;
  Move(HeroData.Abil, HeroObject.m_Abil, SizeOf(TOAbility));
  if HeroObject.m_Abil.Exp <= 0 then
    HeroObject.m_Abil.Exp := 1;
  if HeroObject.m_Abil.MaxExp <= 0 then
  begin
    HeroObject.m_Abil.MaxExp := HeroObject.GetLevelExp(HeroObject.m_Abil.Level);
  end;
  HeroObject.m_wStatusTimeArr := HeroData.wStatusTimeArr;
  {
    HeroObject.m_sHomeMap := Trim(HeroData.sHomeMap);
    HeroObject.m_nHomeX := HeroData.wHomeX;
    HeroObject.m_nHomeY := HeroData.wHomeY;
  }
  HeroObject.m_btReLevel := HeroData.btReLevel;
  HeroObject.m_rLoyalPoint := HeroData.rLoyalPoint;
  HeroObject.m_nPkPoint := HeroData.nPKPoint;
  HeroObject.m_btAttatckMode := HeroData.btAttackMode;
  HeroObject.m_nIncHealth := HeroData.btIncHealth;
  HeroObject.m_nIncSpell := HeroData.btIncSpell;
  HeroObject.m_nIncHealing := HeroData.btIncHealing;
  HeroObject.m_nFightZoneDieCount := HeroData.btFightZoneDieCount;
  HeroObject.m_sUserID := HeroData.sAccount;
  HeroObject.m_QuestFlag := HeroData.QuestFlag;
  HeroObject.m_nCheckRevivalTime := HeroData.nRevivalTime;
  HeroObject.m_nHungerStatus := HeroData.nHungerStatus;
  HeroObject.m_boTrainingNG := HeroData.boTrainingNG; // 是否学习过内功
  HeroObject.m_boTrainingXF := HeroData.boTrainingXF; // 是否学习过心法
  HeroObject.m_nDrinkWineQuality := HeroData.nDrinkWineQuality; // 饮酒时酒的品质
  HeroObject.m_nDrinkWineAlcohol := HeroData.nDrinkWineAlcohol; // 饮酒时酒的度数
  HeroObject.m_boDrinkWineDrunk := HeroData.boDrinkWineDrunk; // 人是否喝酒醉了
  HeroObject.m_AbilNG := HeroData.AbilNG; // 内功属性
  HeroObject.m_Alcohol := HeroData.Alcohol; // 酒属性
  HeroObject.m_HumMeridians := HeroData.Meridians;
  if HeroObject.m_Alcohol.MedicineLevel <= 0 then
    HeroObject.m_Alcohol.MedicineLevel := 1; // 如果药力值等级为0,则设置为1 20080624
  if HeroObject.m_Alcohol.MaxMedicineValue <= 0 then
    HeroObject.m_Alcohol.MaxMedicineValue := HeroObject.GetMedicineExp(HeroObject.m_Alcohol.MedicineLevel);
  if HeroObject.m_AbilNG.Exp <= 0 then
    HeroObject.m_AbilNG.Exp := 1;
  if HeroObject.m_AbilNG.MaxExp <= 0 then
  begin
    HeroObject.m_AbilNG.MaxExp := HeroObject.GetLevelExpNG(HeroObject.m_AbilNG.Level);
  end;
  HeroObject.m_boOpenLastContinuous := HeroData.boOpenLastContinuous; // 第四个连击是否开启
  HeroObject.m_ContinuousMagicOrder[0] := HeroData.ContinuousMagicOrder[0]; // 连击顺序
  HeroObject.m_ContinuousMagicOrder[1] := HeroData.ContinuousMagicOrder[1]; // 连击顺序
  HeroObject.m_ContinuousMagicOrder[2] := HeroData.ContinuousMagicOrder[2]; // 连击顺序
  HeroObject.m_ContinuousMagicOrder[3] := HeroData.btLastContinuousMagicOrder; // 第四个连击顺序
  Move(HeroData.HumItems, HeroObject.m_UseItems, SizeOf(THumanUseItems));
  BagItems := @HeroData.BagItems;
  for I := Low(TBagItems) to High(TBagItems) do
  begin
    if BagItems[I].wIndex > 0 then
    begin
      New(UserItem);
      UserItem^ := BagItems[I];
      HeroObject.m_ItemList.Add(UserItem);
    end;
  end;
  HumMagics := @HeroData.Magics;
  for I := Low(THumMagics) to High(THumMagics) do
  begin
    MagicInfo := UserEngine.FindHeroMagic(HumMagics[I].wMagIdx, HumMagics[I].MagicAttr);
    if MagicInfo <> nil then
    begin
      New(UserMagic);
      UserMagic.MagicInfo := MagicInfo;
      UserMagic.MagicAttr := HumMagics[I].MagicAttr;
      UserMagic.wMagIdx := HumMagics[I].wMagIdx;
      UserMagic.btLevel := HumMagics[I].btLevel;
      UserMagic.btNewLevel := HumMagics[I].btNewLevel;
      UserMagic.btKey := HumMagics[I].btKey;
      UserMagic.nTranPoint := HumMagics[I].nTranPoint;
      UserMagic.boUsesItemAdd := HumMagics[I].boUsesItemAdd;
      HeroObject.m_MagicList.Add(UserMagic);
    end;
  end;
  HumNGMagics := @HeroData.NGMagics;
  for I := Low(THumNGMagics) to High(THumNGMagics) do
  begin
    MagicInfo := UserEngine.FindHeroMagic(HumNGMagics[I].wMagIdx, HumNGMagics[I].MagicAttr);
    if MagicInfo <> nil then
    begin
      New(UserMagic);
      UserMagic.MagicInfo := MagicInfo;
      UserMagic.MagicAttr := HumNGMagics[I].MagicAttr;
      UserMagic.wMagIdx := HumNGMagics[I].wMagIdx;
      UserMagic.btLevel := HumNGMagics[I].btLevel;
      UserMagic.btNewLevel := HumNGMagics[I].btNewLevel;
      UserMagic.btKey := HumNGMagics[I].btKey;
      UserMagic.nTranPoint := HumNGMagics[I].nTranPoint;
      UserMagic.boUsesItemAdd := HumNGMagics[I].boUsesItemAdd;
      HeroObject.m_MagicList.Add(UserMagic);
    end;
  end;
  HumContinuousMagics := @HeroData.ContinuousMagics;
  for I := Low(THumContinuousMagics) to High(THumContinuousMagics) do
  begin
    HumContinuousMagics[I].wMagIdx := ChangeContinuousMagic(HeroObject.m_btJob, HumContinuousMagics[I].wMagIdx);
    MagicInfo := UserEngine.FindHeroMagic(HumContinuousMagics[I].wMagIdx, HumContinuousMagics[I].MagicAttr);
    if MagicInfo <> nil then
    begin
      New(UserMagic);
      UserMagic.MagicInfo := MagicInfo;
      UserMagic.MagicAttr := HumContinuousMagics[I].MagicAttr;
      UserMagic.wMagIdx := HumContinuousMagics[I].wMagIdx;
      UserMagic.btLevel := HumContinuousMagics[I].btLevel;
      UserMagic.btNewLevel := HumContinuousMagics[I].btNewLevel;
      UserMagic.btKey := HumContinuousMagics[I].btKey;
      UserMagic.nTranPoint := HumContinuousMagics[I].nTranPoint;
      UserMagic.boUsesItemAdd := HumContinuousMagics[I].boUsesItemAdd;
      HeroObject.m_MagicList.Add(UserMagic);
    end;
  end;
  if HeroObject.GetContinuousMagicCount <= 0 then
  begin // 没有连击技能
    HeroObject.m_ContinuousMagicOrder[0] := 0;
    HeroObject.m_ContinuousMagicOrder[1] := 0;
    HeroObject.m_ContinuousMagicOrder[2] := 0;
    HeroObject.m_ContinuousMagicOrder[3] := 0;
  end
  else
  begin
    for I := 0 to Length(HeroObject.m_ContinuousMagicOrder) - 1 do
    begin
      if HeroObject.m_ContinuousMagicOrder[I] > 1 then
      begin
        HeroObject.m_ContinuousMagicOrder[I] := ChangeContinuousMagic(HeroObject.m_btJob, HeroObject.m_ContinuousMagicOrder[I]);
        if HeroObject.FindContinuousMagic(HeroObject.m_ContinuousMagicOrder[I]) = nil then
        begin // 没有发连击技能
          HeroObject.m_ContinuousMagicOrder[I] := 0;
        end;
      end;
    end;
  end;
  // 首饰盒状态 0:未激活; 1:激活; 2:开启 chongchong 2013-10-19
  HeroObject.m_nJewelryBoxStatus := HeroData.JewelryBoxStatus;
  // 载入首饰盒保存的数据 chongchong 2013-10-20
  Move(HeroData.JewelryBoxItems, HeroObject.m_JewelryBoxItems, SizeOf(THumanJewelryBoxItems));
  // 是否显示时装 chongchong 2013-10-23
  HeroObject.m_boShowFashion := HeroData.boShowFashion;
  // 是否显示神佑袋 chongchong 2014-04-19
  HeroObject.m_boShowGodBless := HeroData.boShowGodBless;
  Move(HeroData.GodBlessItemsState, HeroObject.m_GodBlessItemsState, SizeOf(TGodBlessItemsState));
  Move(HeroData.GodBlessItems, HeroObject.m_GodBlessItems, SizeOf(THumanGodBlessItems));
  for I := Low(HeroData.FengHaoItems) to High(HeroData.FengHaoItems) do
  begin
    if HeroData.FengHaoItems[I].wIndex > 0 then
    begin
      New(UserItem);
      UserItem^ := HeroData.FengHaoItems[I];
      HeroObject.m_FengHaoItems.Add(UserItem);
    end;
  end;
  HeroObject.m_ActiveFengHao := HeroData.nActiveFengHao;
  Move(HeroData.AddSaveAbil, HeroObject.m_AddSaveAbil, Min(SizeOf(HeroData.AddSaveAbil), SizeOf(HeroObject.m_AddSaveAbil)));
  HeroObject.m_boSaveKillMonBurstRate := HeroData.boSaveKillMonBurstRate;
  HeroObject.m_dwKillMonBurstRate := HeroData.nKillMonBurstRate;
  HeroObject.m_dwKillMonBurstRateTime := HeroData.dwKillMonBurstRateTime;
  if (HeroObject.m_dwKillMonBurstRate <= 0) then
  begin
    HeroObject.m_dwKillMonBurstRate := 100;
    HeroObject.m_dwKillMonBurstRateTime := 0;
  end;
  HeroObject.m_boAttackHumSavePowerRate := HeroData.boAttackHumSavePowerRate;
  HeroObject.m_dwAttackHumPowerRateTime := HeroData.dwAttackHumPowerRateTime;
  HeroObject.m_nAttackHumPowerRate := HeroData.nAttackHumPowerRate;
  if (HeroObject.m_nAttackHumPowerRate <= 0) then
  begin
    HeroObject.m_nAttackHumPowerRate := 100;
    HeroObject.m_dwAttackHumPowerRateTime := 0;
  end;
  HeroObject.m_boAttackMonSavePowerRate := HeroData.boAttackMonSavePowerRate;
  HeroObject.m_dwAttackMonPowerRateTime := HeroData.dwAttackMonPowerRateTime;
  HeroObject.m_nAttackMonPowerRate := HeroData.nAttackMonPowerRate;
  if (HeroObject.m_nAttackMonPowerRate <= 0) then
  begin
    HeroObject.m_nAttackMonPowerRate := 100;
    HeroObject.m_dwAttackMonPowerRateTime := 0;
  end;
  HeroObject.m_boSaveKillMonExpRate := HeroData.boSaveKillMonExpRate;
  HeroObject.m_nKillMonExpRate := HeroData.nKillMonExpRate;
  HeroObject.m_dwKillMonExpRateTime := HeroData.dwKillMonExpRateTime;
  if (HeroObject.m_nKillMonExpRate <= 0) then
  begin
    HeroObject.m_nKillMonExpRate := 100;
    HeroObject.m_dwKillMonExpRateTime := 0;
  end;
  if HeroData.dwHighLevelKillMonFixExpTimeLeft > 0 then
  begin
    HeroObject.m_dwHighLevelKillMonFixExpTime := HeroData.dwHighLevelKillMonFixExpTimeLeft;
    HeroObject.m_boSaveHighLevelKillMonFixExpTime := True;
  end
  else
  begin
    HeroObject.m_dwHighLevelKillMonFixExpTime := 0;
    HeroObject.m_boSaveHighLevelKillMonFixExpTime := False;
  end;
  for I := Low(HeroData.NpcSkillPowerAdd) to High(HeroData.NpcSkillPowerAdd) do
  begin
    if (HeroData.NpcSkillPowerAdd[I].HumanAttackPercent <> 0) or (HeroData.NpcSkillPowerAdd[I].HumanAttackValue <> 0) or
      (HeroData.NpcSkillPowerAdd[I].MonAttackPercent <> 0) or (HeroData.NpcSkillPowerAdd[I].MonAttackValue <> 0) or
      (HeroData.NpcSkillPowerAdd[I].DefensePercent <> 0) or (HeroData.NpcSkillPowerAdd[I].DefenseValue <> 0) then
    begin
      HeroObject.m_NpcSkillPowerAdd[I].HumanAttackPercent := HeroData.NpcSkillPowerAdd[I].HumanAttackPercent;
      HeroObject.m_NpcSkillPowerAdd[I].HumanAttackValue := HeroData.NpcSkillPowerAdd[I].HumanAttackValue;
      HeroObject.m_NpcSkillPowerAdd[I].MonAttackPercent := HeroData.NpcSkillPowerAdd[I].MonAttackPercent;
      HeroObject.m_NpcSkillPowerAdd[I].MonAttackValue := HeroData.NpcSkillPowerAdd[I].MonAttackValue;
      HeroObject.m_NpcSkillPowerAdd[I].DefensePercent := HeroData.NpcSkillPowerAdd[I].DefensePercent;
      HeroObject.m_NpcSkillPowerAdd[I].DefenseValue := HeroData.NpcSkillPowerAdd[I].DefenseValue;
      HeroObject.m_NpcSkillPowerAdd[I].RemainingTime := HeroData.NpcSkillPowerAdd[I].RemainingTime;
      HeroObject.m_NpcSkillPowerAdd[I].Tick := MyGetTickCount;
      HeroObject.m_NpcSkillPowerAdd[I].IsSave := True;
    end
    else
    begin
      FillChar(HeroObject.m_NpcSkillPowerAdd[I], SizeOf(HeroObject.m_NpcSkillPowerAdd[I]), 0);
    end;
  end;
end;

procedure TUserEngine.GetHumData(PlayObject: TPlayObject; HumData: PTHumData);
var
  BagItems: pTBagItems;
  UserItem: pTUserItem;
  HumMagics: pTHumMagics;
  UserMagic: PTUserMagic;
  HumNGMagics: pTHumNGMagics;
  HumContinuousMagics: pTHumContinuousMagics;
  MagicInfo: pTMagic;
  StorageItems: pTStorageItems;
  GamePetData: pTGamePetData;
  I, Index, Count: Integer;
  nDay: Integer;
begin
  PlayObject.m_sCharName := Trim(HumData.sChrName);
  PlayObject.m_sMapName := Trim(HumData.sCurMap);
  PlayObject.m_nCurrX := HumData.wCurX;
  PlayObject.m_nCurrY := HumData.wCurY;
  PlayObject.m_btDirection := HumData.btDir;
  if not(PlayObject.m_btDirection in [DR_UP .. DR_UPLEFT]) then
  begin
    PlayObject.m_btDirection := DR_UP;
  end;
  PlayObject.m_btHair := HumData.btHair;
  PlayObject.m_btGender := HumData.btSex;
  PlayObject.m_btJob := HumData.btJob;
  PlayObject.m_nGold := HumData.nGold;
  Move(HumData.Abil, PlayObject.m_Abil, SizeOf(TOAbility));
  if PlayObject.m_Abil.Exp <= 0 then
    PlayObject.m_Abil.Exp := 1;
  if PlayObject.m_Abil.MaxExp <= 0 then
  begin
    PlayObject.m_Abil.MaxExp := PlayObject.GetLevelExp(PlayObject.m_Abil.Level);
  end;
  PlayObject.m_wStatusTimeArr := HumData.wStatusTimeArr;
  PlayObject.m_sHomeMap := Trim(HumData.sHomeMap);
  PlayObject.m_nHomeX := HumData.wHomeX;
  PlayObject.m_nHomeY := HumData.wHomeY;
  PlayObject.m_BonusAbil := HumData.BonusAbil; // 08/09
  // 有人把这个属刷成负值，导致刷属性点，刷了属性点的人，把属性点清0掉 chongchong 2018-03-21
  if (PlayObject.m_BonusAbil.DC < 0) or (PlayObject.m_BonusAbil.MC < 0) or (PlayObject.m_BonusAbil.SC < 0) or
    (PlayObject.m_BonusAbil.AC < 0) or (PlayObject.m_BonusAbil.MAC < 0) or (PlayObject.m_BonusAbil.HP < 0) or
    (PlayObject.m_BonusAbil.MP < 0) or (PlayObject.m_BonusAbil.Hit < 0) or (PlayObject.m_BonusAbil.Speed < 0) or
    (PlayObject.m_BonusAbil.X2 < 0) then
  begin
    FillChar(PlayObject.m_BonusAbil, SizeOf(PlayObject.m_BonusAbil), 0);
  end;
  PlayObject.m_nBonusPoint := HumData.nBonusPoint; // 08/09
  PlayObject.m_btReLevel := HumData.btReLevel;
  PlayObject.m_sMasterName := Trim(HumData.sMasterName);
  PlayObject.m_boMaster := HumData.boMaster;
  PlayObject.m_sDearName := Trim(HumData.sDearName);
  // 多师徒 - 读取师徒数据 chongchong 2014-10-11
  if PlayObject.m_boMaster or (PlayObject.m_sMasterName <> '') then
    PlayObject.GetMasterNoList();
  PlayObject.m_sStoragePwd := HumData.sStoragePwd;
  if PlayObject.m_sStoragePwd <> '' then
    PlayObject.m_boPasswordLocked := True;
  PlayObject.m_nGameGoldEx := HumData.nGameGoldEx;
  PlayObject.m_nGameGold := HumData.nGameGold;
  PlayObject.m_nGamePoint := HumData.nGamePoint;
  PlayObject.m_nPayMentPoint := HumData.nPayMentPoint;
  PlayObject.m_nMemberType := HumData.nMemberType;
  PlayObject.m_nMemberLevel := HumData.nMemberLevel;
  PlayObject.m_nGameDiamond := HumData.nGameDiamond;
  PlayObject.m_nGameGird := HumData.nGameGird;
  PlayObject.m_nGameGlory := HumData.nGameGlory;
  PlayObject.m_nNationCredit := HumData.nNationCredit;
  // PlayObject.m_boFilterGlobalMsg := HumData.boFilterGlobalMsg;
  PlayObject.m_boFilterGlobalDropItemMsg := HumData.boFilterGlobalDropItemMsg; // 过滤掉落提示信息
  PlayObject.m_boFilterGlobalCenterMsg := HumData.boFilterGlobalCenterMsg; // 过滤SendCenterMsg
  PlayObject.m_boFilterGolbalSendMsg := HumData.boFilterGolbalSendMsg; // 过滤SendMsg全局信息
  PlayObject.m_boAllowDeal := not HumData.boDisableTrading;
  PlayObject.m_btNewServer := Integer(HumData.boNewServer);
  PlayObject.m_nPkPoint := HumData.nPKPoint;
  PlayObject.m_boAllowGroup := HumData.boAllowGroup;
  PlayObject.m_btNation := HumData.btNation;
  PlayObject.m_btAttatckMode := HumData.btAttackMode;
  /// ///////////////////////////////////////////////////////////////////////////
  PlayObject.m_nIncHealth := HumData.btIncHealth;
  PlayObject.m_nIncSpell := HumData.btIncSpell;
  PlayObject.m_nIncHealing := HumData.btIncHealing;
  PlayObject.m_nFightZoneDieCount := HumData.btFightZoneDieCount;
  PlayObject.m_sUserID := HumData.sAccount;
  // PlayObject.nC4 := HumData.btEE;
  PlayObject.m_boLockLogon := HumData.boLockLogin;
  PlayObject.m_wContribution := HumData.wContribution;
  // PlayObject.btC8 := HumData.btEF;
  PlayObject.m_nHungerStatus := HumData.nHungerStatus;
  PlayObject.m_boAllowGuildReCall := HumData.boAllowGuildReCall;
  PlayObject.m_wGroupRcallTime := HumData.wGroupRecallTime;
  PlayObject.m_dBodyLuck := HumData.dBodyLuck;
  PlayObject.m_boAllowGroupReCall := HumData.boAllowGroupReCall;
  // PlayObject.m_QuestUnit := HumData.QuestUnit;
  PlayObject.m_boGameGoldDeal := HumData.boGameGoldTrading;
  PlayObject.m_boDisableHorseInvite := HumData.boDisableInviteHorseRiding;
  PlayObject.m_nKickCount := HumData.nKickCount;
  { 副本地图 -- 载入副本地图创建时间 chongchong 2013-09-11 }
  PlayObject.m_dwFBCreateTime := HumData.dwFBCreateTime;
  PlayObject.m_boSaveKillMonExpRate := HumData.boSaveKillMonExpRate;
  PlayObject.m_nKillMonExpRate := HumData.nKillMonExpRate;
  PlayObject.m_dwKillMonExpRateTime := HumData.dwKillMonExpRateTime;
  if (PlayObject.m_nKillMonExpRate <= 0) then
  begin
    PlayObject.m_nKillMonExpRate := 100;
    PlayObject.m_dwKillMonExpRateTime := 0;
  end;
  PlayObject.m_QuestFlag := HumData.QuestFlag;
  PlayObject.m_boTrainingNG := HumData.boTrainingNG; // 是否学习过内功
  PlayObject.m_boTrainingXF := HumData.boTrainingXF; // 是否学习过心法
  PlayObject.m_nDrinkWineQuality := HumData.nDrinkWineQuality; // 饮酒时酒的品质
  PlayObject.m_nDrinkWineAlcohol := HumData.nDrinkWineAlcohol; // 饮酒时酒的度数
  PlayObject.m_boDrinkWineDrunk := HumData.boDrinkWineDrunk; // 人是否喝酒醉了
  PlayObject.m_AbilNG := HumData.AbilNG; // 内功属性
  PlayObject.m_Alcohol := HumData.Alcohol; // 酒属性
  PlayObject.m_HumMeridians := HumData.Meridians;
  if PlayObject.m_AbilNG.Exp <= 0 then
    PlayObject.m_AbilNG.Exp := 1;
  if PlayObject.m_AbilNG.MaxExp <= 0 then
  begin
    PlayObject.m_AbilNG.MaxExp := PlayObject.GetLevelExpNG(PlayObject.m_AbilNG.Level);
  end;
  if PlayObject.m_Alcohol.MedicineLevel <= 0 then
    PlayObject.m_Alcohol.MedicineLevel := 1; // 如果药力值等级为0,则设置为1 20080624
  if PlayObject.m_Alcohol.MaxMedicineValue <= 0 then
    PlayObject.m_Alcohol.MaxMedicineValue := PlayObject.GetMedicineExp(PlayObject.m_Alcohol.MedicineLevel);
  PlayObject.m_nCheckRevivalTime := HumData.nRevivalTime;
  PlayObject.m_wMasterCount := HumData.wMasterCount;
  PlayObject.m_boPleaseDrink := HumData.boPleaseDrink; // 是否请过酒
  PlayObject.m_boFixedHero := HumData.boFixedHero; // 是否评定主副英雄
  PlayObject.m_boStorageHero := HumData.boStorageHero; // 英雄是否寄存
  PlayObject.m_boStorageDeputyHero := HumData.boStorageDeputyHero; // 副将英雄是否寄存
  PlayObject.m_sHeroName := HumData.sHeroName;
  PlayObject.m_sDeputyHeroName := HumData.sDeputyHeroName; // 副将英雄名字
  PlayObject.m_btDeputyHeroJob := HumData.btDeputyHeroJob; // 副将英雄出生职业
  PlayObject.m_boOpenLastContinuous := HumData.boOpenLastContinuous; // 第四个连击是否开启
  PlayObject.m_ContinuousMagicOrder[0] := HumData.ContinuousMagicOrder[0]; // 连击顺序
  PlayObject.m_ContinuousMagicOrder[1] := HumData.ContinuousMagicOrder[1]; // 连击顺序
  PlayObject.m_ContinuousMagicOrder[2] := HumData.ContinuousMagicOrder[2]; // 连击顺序
  PlayObject.m_ContinuousMagicOrder[3] := HumData.btLastContinuousMagicOrder; // 第四个连击顺序
  {
    HumItems := @HumData.Data.HumItems;
    PlayObject.m_UseItems[U_DRESS] := HumItems[U_DRESS];
    PlayObject.m_UseItems[U_WEAPON] := HumItems[U_WEAPON];
    PlayObject.m_UseItems[U_RIGHTHAND] := HumItems[U_RIGHTHAND];
    PlayObject.m_UseItems[U_NECKLACE] := HumItems[U_HELMET];
    PlayObject.m_UseItems[U_HELMET] := HumItems[U_NECKLACE];
    PlayObject.m_UseItems[U_ARMRINGL] := HumItems[U_ARMRINGL];
    PlayObject.m_UseItems[U_ARMRINGR] := HumItems[U_ARMRINGR];
    PlayObject.m_UseItems[U_RINGL] := HumItems[U_RINGL];
    PlayObject.m_UseItems[U_RINGR] := HumItems[U_RINGR];
    PlayObject.m_UseItems[U_BUJUK] := HumItems[U_BUJUK];
    PlayObject.m_UseItems[U_BELT] := HumItems[U_BELT];
    PlayObject.m_UseItems[U_BOOTS] := HumItems[U_BOOTS];
    PlayObject.m_UseItems[U_CHARM] := HumItems[U_CHARM];
    PlayObject.m_UseItems[U_HAT] := HumItems[U_HAT];
    PlayObject.m_UseItems[U_DRUM] := HumItems[U_DRUM];                                                // 鼓
    PlayObject.m_UseItems[U_HORSE] := HumItems[U_HORSE];                                              // 马
    PlayObject.m_UseItems[U_SHIELD] := HumItems[U_SHIELD];                                            // 盾牌 chongchong 2013-09-16
  }
  Move(HumData.HumItems, PlayObject.m_UseItems, SizeOf(THumanUseItems));
  PlayObject.m_btExtBagPageCount := HumData.btExtBagPageCount;
  PlayObject.m_btExtBagOpenItemCount := HumData.btExtBagOpenItemCount;
  PlayObject.m_dwAddMaxWeight := HumData.dwAddMaxWeight;
  Count := 0;
  BagItems := @HumData.BagItems;
  for I := Low(TBagItems) to High(TBagItems) do
  begin
    if BagItems[I].wIndex > 0 then
    begin
      New(UserItem);
      UserItem^ := BagItems[I];
      PlayObject.m_ItemList.Add(UserItem);
      Inc(Count);
      if Count >= DEF_MAX_BAG_ITEM + PlayObject.m_btExtBagOpenItemCount then
        Break;
    end;
  end;
  g_M2DataDB.StorageDB.LoadStorageItems(PlayObject.m_sCharName, PlayObject.m_BigStorageItemList, PlayObject.m_nBigStorageID);
  HumMagics := @HumData.Magics;
  for I := Low(THumMagics) to High(THumMagics) do
  begin
    MagicInfo := UserEngine.FindMagic(HumMagics[I].wMagIdx, HumMagics[I].MagicAttr);
    if MagicInfo <> nil then
    begin
      New(UserMagic);
      UserMagic.MagicInfo := MagicInfo;
      UserMagic.MagicAttr := HumMagics[I].MagicAttr;
      UserMagic.wMagIdx := HumMagics[I].wMagIdx;
      UserMagic.btLevel := HumMagics[I].btLevel;
      UserMagic.btNewLevel := HumMagics[I].btNewLevel;
      UserMagic.btKey := HumMagics[I].btKey;
      UserMagic.nTranPoint := HumMagics[I].nTranPoint;
      UserMagic.boUsesItemAdd := HumMagics[I].boUsesItemAdd;
      PlayObject.m_MagicList.Add(UserMagic);
    end;
  end;
  HumNGMagics := @HumData.NGMagics;
  for I := Low(THumNGMagics) to High(THumNGMagics) do
  begin
    MagicInfo := UserEngine.FindMagic(HumNGMagics[I].wMagIdx, HumNGMagics[I].MagicAttr);
    if MagicInfo <> nil then
    begin
      New(UserMagic);
      UserMagic.MagicInfo := MagicInfo;
      UserMagic.MagicAttr := HumNGMagics[I].MagicAttr;
      UserMagic.wMagIdx := HumNGMagics[I].wMagIdx;
      UserMagic.btLevel := HumNGMagics[I].btLevel;
      UserMagic.btNewLevel := HumNGMagics[I].btNewLevel;
      UserMagic.btKey := HumNGMagics[I].btKey;
      UserMagic.nTranPoint := HumNGMagics[I].nTranPoint;
      UserMagic.boUsesItemAdd := HumNGMagics[I].boUsesItemAdd;
      PlayObject.m_MagicList.Add(UserMagic);
    end;
  end;
  HumContinuousMagics := @HumData.ContinuousMagics;
  for I := Low(THumContinuousMagics) to High(THumContinuousMagics) do
  begin
    MagicInfo := UserEngine.FindMagic(HumContinuousMagics[I].wMagIdx, HumContinuousMagics[I].MagicAttr);
    if MagicInfo <> nil then
    begin
      New(UserMagic);
      UserMagic.MagicInfo := MagicInfo;
      UserMagic.MagicAttr := HumContinuousMagics[I].MagicAttr;
      UserMagic.wMagIdx := HumContinuousMagics[I].wMagIdx;
      UserMagic.btLevel := HumContinuousMagics[I].btLevel;
      UserMagic.btNewLevel := HumContinuousMagics[I].btNewLevel;
      UserMagic.btKey := HumContinuousMagics[I].btKey;
      UserMagic.nTranPoint := HumContinuousMagics[I].nTranPoint;
      UserMagic.boUsesItemAdd := HumContinuousMagics[I].boUsesItemAdd;
      PlayObject.m_MagicList.Add(UserMagic);
    end;
  end;
  StorageItems := @HumData.StorageItems;
  for I := Low(TStorageItems) to High(TStorageItems) do
  begin
    if StorageItems[I].wIndex > 0 then
    begin
      New(UserItem);
      UserItem^ := StorageItems[I];
      Index := I div 49;
      PlayObject.m_StorageItemList[Index].Add(UserItem);
    end;
  end;
  PlayObject.m_boStorageOpen[0] := True;
  PlayObject.m_boStorageOpen[1] := HumData.boStorageOpen[1];
  PlayObject.m_boStorageOpen[2] := HumData.boStorageOpen[2];
  PlayObject.m_boStorageOpen[3] := HumData.boStorageOpen[3];
  if PlayObject.GetContinuousMagicCount <= 0 then
  begin // 没有连击技能
    PlayObject.m_ContinuousMagicOrder[0] := 0;
    PlayObject.m_ContinuousMagicOrder[1] := 0;
    PlayObject.m_ContinuousMagicOrder[2] := 0;
    PlayObject.m_ContinuousMagicOrder[3] := 0;
  end
  else
  begin
    for I := 0 to Length(PlayObject.m_ContinuousMagicOrder) - 1 do
    begin
      if PlayObject.m_ContinuousMagicOrder[I] > 1 then
      begin
        if PlayObject.FindContinuousMagic(PlayObject.m_ContinuousMagicOrder[I]) = nil then
        begin // 没有发连击技能
          PlayObject.m_ContinuousMagicOrder[I] := 0;
        end;
      end;
    end;
  end;
  // 首饰盒状态 0:未激活; 1:激活; 2:开启 chongchong 2013-10-19
  PlayObject.m_nJewelryBoxStatus := HumData.JewelryBoxStatus;
  // 载入首饰盒保存的数据 chongchong 2013-10-20
  Move(HumData.JewelryBoxItems, PlayObject.m_JewelryBoxItems, SizeOf(THumanJewelryBoxItems));
  // 是否显示时装 chongchong 2013-10-23
  PlayObject.m_boShowFashion := HumData.boShowFashion;
  // 是否显示神佑袋 chongchong 2014-04-19
  PlayObject.m_boShowGodBless := HumData.boShowGodBless;
  Move(HumData.GodBlessItemsState, PlayObject.m_GodBlessItemsState, SizeOf(TGodBlessItemsState));
  Move(HumData.GodBlessItems, PlayObject.m_GodBlessItems, SizeOf(THumanGodBlessItems));
  // Move(HumData.Data.HumanFengHaoItems, PlayObject.m_FengHaoItems, SizeOf(THumanFengHaoItems));
  for I := Low(HumData.FengHaoItems) to High(HumData.FengHaoItems) do
  begin
    if HumData.FengHaoItems[I].wIndex > 0 then
    begin
      New(UserItem);
      UserItem^ := HumData.FengHaoItems[I];
      PlayObject.m_FengHaoItems.Add(UserItem);
    end;
  end;
  PlayObject.m_ActiveFengHao := HumData.nActiveFengHao;
  if SizeOf(PlayObject.m_UVal) = SizeOf(HumData.UValues) then
    Move(HumData.UValues, PlayObject.m_UVal, SizeOf(PlayObject.m_UVal))
  else
    MainOutMessage('TUserEngine.GetHumData Error; m_UVal error');
  if SizeOf(PlayObject.m_TVal) = SizeOf(HumData.TValues) then
    Move(HumData.TValues, PlayObject.m_TVal, SizeOf(PlayObject.m_TVal))
  else
    MainOutMessage('TUserEngine.GetHumData Error; m_TVal error');
  PlayObject.m_nClearDayVarTime := HumData.nClearDayVarTime;
  nDay := Trunc(Now);
  if (nDay <> PlayObject.m_nClearDayVarTime) and (HourOf(Now) >= g_Config.btPlayerVarJClearTime) then
  begin
    PlayObject.m_nClearDayVarTime := nDay;
    FillChar(PlayObject.m_JVal, SizeOf(PlayObject.m_JVal), 0);
    FillChar(PlayObject.m_ZVal, SizeOf(PlayObject.m_ZVal), 0);
  end
  else
  begin
    if SizeOf(PlayObject.m_JVal) = SizeOf(HumData.JValues) then
      Move(HumData.JValues, PlayObject.m_JVal, SizeOf(PlayObject.m_JVal))
    else
      MainOutMessage('TUserEngine.GetHumData Error; m_JVal error');
    if SizeOf(PlayObject.m_ZVal) = SizeOf(HumData.ZValues) then
      Move(HumData.ZValues, PlayObject.m_ZVal, SizeOf(PlayObject.m_ZVal))
    else
      MainOutMessage('TUserEngine.GetHumData Error; m_ZVal error');
  end;
  PlayObject.m_boSaveKillMonBurstRate := HumData.boSaveKillMonBurstRate;
  PlayObject.m_dwKillMonBurstRate := HumData.nKillMonBurstRate;
  PlayObject.m_dwKillMonBurstRateTime := HumData.dwKillMonBurstRateTime;
  if (PlayObject.m_dwKillMonBurstRate <= 0) then
  begin
    PlayObject.m_dwKillMonBurstRate := 100;
    PlayObject.m_dwKillMonBurstRateTime := 0;
  end;
  PlayObject.m_boAttackHumSavePowerRate := HumData.boAttackHumSavePowerRate;
  PlayObject.m_dwAttackHumPowerRateTime := HumData.dwAttackHumPowerRateTime;
  PlayObject.m_nAttackHumPowerRate := HumData.nAttackHumPowerRate;
  if (PlayObject.m_nAttackHumPowerRate <= 0) then
  begin
    PlayObject.m_nAttackHumPowerRate := 100;
    PlayObject.m_dwAttackHumPowerRateTime := 0;
  end;
  PlayObject.m_boAttackMonSavePowerRate := HumData.boAttackMonSavePowerRate;
  PlayObject.m_dwAttackMonPowerRateTime := HumData.dwAttackMonPowerRateTime;
  PlayObject.m_nAttackMonPowerRate := HumData.nAttackMonPowerRate;
  if (PlayObject.m_nAttackMonPowerRate <= 0) then
  begin
    PlayObject.m_nAttackMonPowerRate := 100;
    PlayObject.m_dwAttackMonPowerRateTime := 0;
  end;
  if HumData.dwHighLevelKillMonFixExpTimeLeft > 0 then
  begin
    PlayObject.m_dwHighLevelKillMonFixExpTime := HumData.dwHighLevelKillMonFixExpTimeLeft;
    PlayObject.m_boSaveHighLevelKillMonFixExpTime := True;
  end
  else
  begin
    PlayObject.m_dwHighLevelKillMonFixExpTime := 0;
    PlayObject.m_boSaveHighLevelKillMonFixExpTime := False;
  end;
  Move(HumData.CustomSkillUseTicks, PlayObject.m_CustomSkillUseTick, SizeOf(PlayObject.m_CustomSkillUseTick));
  for I := 0 to Length(PlayObject.m_CustomSkillUseTick) - 1 do
  begin
    if PlayObject.m_CustomSkillUseTick[I] > MyGetTickCount then
      PlayObject.m_CustomSkillUseTick[I] := MyGetTickCount;
  end;
  for I := Low(HumData.NpcSkillPowerAdd) to High(HumData.NpcSkillPowerAdd) do
  begin
    if (HumData.NpcSkillPowerAdd[I].HumanAttackPercent <> 0) or (HumData.NpcSkillPowerAdd[I].HumanAttackValue <> 0) or
      (HumData.NpcSkillPowerAdd[I].MonAttackPercent <> 0) or (HumData.NpcSkillPowerAdd[I].MonAttackValue <> 0) or
      (HumData.NpcSkillPowerAdd[I].DefensePercent <> 0) or (HumData.NpcSkillPowerAdd[I].DefenseValue <> 0) then
    begin
      PlayObject.m_NpcSkillPowerAdd[I].HumanAttackPercent := HumData.NpcSkillPowerAdd[I].HumanAttackPercent;
      PlayObject.m_NpcSkillPowerAdd[I].HumanAttackValue := HumData.NpcSkillPowerAdd[I].HumanAttackValue;
      PlayObject.m_NpcSkillPowerAdd[I].MonAttackPercent := HumData.NpcSkillPowerAdd[I].MonAttackPercent;
      PlayObject.m_NpcSkillPowerAdd[I].MonAttackValue := HumData.NpcSkillPowerAdd[I].MonAttackValue;
      PlayObject.m_NpcSkillPowerAdd[I].DefensePercent := HumData.NpcSkillPowerAdd[I].DefensePercent;
      PlayObject.m_NpcSkillPowerAdd[I].DefenseValue := HumData.NpcSkillPowerAdd[I].DefenseValue;
      PlayObject.m_NpcSkillPowerAdd[I].RemainingTime := HumData.NpcSkillPowerAdd[I].RemainingTime;
      PlayObject.m_NpcSkillPowerAdd[I].Tick := MyGetTickCount;
      PlayObject.m_NpcSkillPowerAdd[I].IsSave := True;
    end
    else
    begin
      FillChar(PlayObject.m_NpcSkillPowerAdd[I], SizeOf(PlayObject.m_NpcSkillPowerAdd[I]), 0);
    end;
  end;
  Move(HumData.AddSaveAbil, PlayObject.m_AddSaveAbil, Min(SizeOf(HumData.AddSaveAbil), SizeOf(PlayObject.m_AddSaveAbil)));
  PlayObject.m_nInfinityStorageExtCount := HumData.dwInfinityStorageExtCount;
  PlayObject.m_sMobileNumber := HumData.sMobileNumber;
  PlayObject.m_boMobileBind := HumData.boMobileBind;
  PlayObject.m_sMobileVerifyCode := HumData.sMobileVerifyCode;
  PlayObject.m_dwMobileVerifyTick := HumData.dwMobileSendTick;
  PlayObject.m_nMobileResendCount := HumData.nMobileResendCount;
  for I := Low(HumData.GamePetBagItems) to High(HumData.GamePetBagItems) do
  begin
    if HumData.GamePetBagItems[I].wIndex > 0 then
    begin
      New(UserItem);
      UserItem^ := HumData.GamePetBagItems[I];
      PlayObject.m_GamePetBagItems.Add(UserItem);
    end;
  end;
  for I := Low(HumData.GamePetData) to High(HumData.GamePetData) do
  begin
    if (HumData.GamePetData[I].sName <> '') and (PlayObject.m_GamePetList.Count < g_Config.nGamePetMaxCount) then
    begin
      New(GamePetData);
      GamePetData^ := HumData.GamePetData[I];
      PlayObject.m_GamePetList.Add(GamePetData);
    end;
  end;
  for I := Low(HumData.CustomMoney) to High(HumData.CustomMoney) do
  begin
    if (HumData.CustomMoney[I].sName <> '') then
    begin
      Index := PlayObject.m_MoneyList.GetIndex(UpperCase(HumData.CustomMoney[I].sName));
      if Index >= 0 then
      begin
        PlayObject.m_MoneyList.Objects[Index] := TObject(HumData.CustomMoney[I].nCount);
      end
      else
      begin
        PlayObject.m_MoneyList.AddRecord(UpperCase(HumData.CustomMoney[I].sName), HumData.CustomMoney[I].nCount);
      end;
    end;
  end;
end;

function TUserEngine.GetHomeInfo(var nX, nY: Integer): string;
var
  I: Integer;
  SafeArea: TSafeArea;
begin
  if g_SafeAreaManager.Count > 0 then
  begin
    if g_SafeAreaManager.Count > g_Config.nStartPointSize { 1 } then
      I := Random(g_Config.nStartPointSize { 2 } )
    else
      I := 0;
    SafeArea := g_SafeAreaManager.Items[I];
    Result := SafeArea.MapName;
    nX := SafeArea.GetCenterX;
    nY := SafeArea.GetCenterY;
  end
  else
  begin
    Result := g_Config.sHomeMap;
    nX := g_Config.nHomeX;
    nX := g_Config.nHomeY;
  end;
end;

function TUserEngine.GetRandHomeX(PlayObject: TPlayObject): Integer;
begin
  Result := Random(3) + (PlayObject.m_nHomeX - 2);
end;

function TUserEngine.GetRandHomeY(PlayObject: TPlayObject): Integer;
begin
  Result := Random(3) + (PlayObject.m_nHomeY - 2);
end;

function TUserEngine.FindMagicEx(sMagicName: string): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockW(2);
    try
{$IFEND}
      MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
      if MagicList <> nil then
      begin
        for I := 0 to MagicList.Count - 1 do
        begin
          Magic := MagicList.Items[I];
          if (Magic <> nil) then
          begin
            if CompareText(Magic.sMagicName, sMagicName) = 0 then
            begin
              Result := Magic;
              Break;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockW;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockW(4);
    try
{$IFEND}
      for I := 0 to m_MagicList.Count - 1 do
      begin
        Magic := m_MagicList.Items[I];
        if (Magic <> nil) then
        begin
          if CompareText(Magic.sMagicName, sMagicName) = 0 then
          begin
            Result := Magic;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockW;
    end;
{$IFEND}
  end;
end;

function TUserEngine.FindMagic(nMagIdx: Integer): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockR(3);
    try
{$IFEND}
      MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
      if MagicList <> nil then
      begin
        for I := 0 to MagicList.Count - 1 do
        begin
          Magic := MagicList.Items[I];
          if (Magic <> nil) and (Magic.MagicAttr <> mtHero) then
          begin
            if Magic.wMagicID = nMagIdx then
            begin
              Result := Magic;
              Break;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockR(5);
    try
{$IFEND}
      for I := 0 to m_MagicList.Count - 1 do
      begin
        Magic := m_MagicList.Items[I];
        if (Magic <> nil) and (Magic.MagicAttr <> mtHero) then
        begin
          if Magic.wMagicID = nMagIdx then
          begin
            Result := Magic;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockR;
    end;
{$IFEND}
  end;
end;

function TUserEngine.FindHeroMagic(nMagIdx: Integer): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockR(4);
    try
{$IFEND}
      MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
      if MagicList <> nil then
      begin
        for I := 0 to MagicList.Count - 1 do
        begin
          Magic := MagicList.Items[I];
          if (Magic <> nil) and (Magic.MagicAttr <> mtHum) then
          begin
            if Magic.wMagicID = nMagIdx then
            begin
              Result := Magic;
              Break;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockR(6);
    try
{$IFEND}
      for I := 0 to m_MagicList.Count - 1 do
      begin
        Magic := m_MagicList.Items[I];
        if (Magic <> nil) and (Magic.MagicAttr <> mtHum) then
        begin
          if Magic.wMagicID = nMagIdx then
          begin
            Result := Magic;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockR;
    end;
{$IFEND}
  end;
end;

function TUserEngine.FindMagic(nMagIdx: Integer; MagicAttr: TMagicAttr): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockR(5);
    try
{$IFEND}
      MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
      if MagicList <> nil then
      begin
        for I := 0 to MagicList.Count - 1 do
        begin
          Magic := MagicList.Items[I];
          if (Magic <> nil) and (Magic.wMagicID = nMagIdx) and (Magic.MagicAttr = MagicAttr) then
          begin
            Result := Magic;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockR(7);
    try
{$IFEND}
      for I := 0 to m_MagicList.Count - 1 do
      begin
        Magic := m_MagicList.Items[I];
        if (Magic <> nil) and (Magic.wMagicID = nMagIdx) and (Magic.MagicAttr = MagicAttr) then
        begin
          Result := Magic;
          Break;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockR;
    end;
{$IFEND}
  end;
end;

function TUserEngine.FindHeroMagic(nMagIdx: Integer; MagicAttr: TMagicAttr): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockR(6);
    try
{$IFEND}
      MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
      if MagicList <> nil then
      begin
        for I := 0 to MagicList.Count - 1 do
        begin
          Magic := MagicList.Items[I];
          if (Magic <> nil) and (Magic.wMagicID = nMagIdx) and (Magic.MagicAttr = MagicAttr) then
          begin
            Result := Magic;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockR(8);
    try
{$IFEND}
      for I := 0 to m_MagicList.Count - 1 do
      begin
        Magic := m_MagicList.Items[I];
        if (Magic <> nil) and (Magic.wMagicID = nMagIdx) and (Magic.MagicAttr = MagicAttr) then
        begin
          Result := Magic;
          Break;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockR;
    end;
{$IFEND}
  end;
end;

procedure TUserEngine.MonRecalcASubAbilitys(BaseObject: TBaseObject; sMonName: string);
var
  I: Integer;
  Monster: pTMonInfo;
begin
  if (BaseObject = nil) then
    Exit;
  if FindMonster(sMonName, I) then
  begin
    Monster := MonsterList.Items[I];
    BaseObject.m_btSpeedPoint := Monster.wSpeed;
    BaseObject.m_btHitPoint := Monster.wHitPoint;
  end;
end;

procedure TUserEngine.MonInitialize(BaseObject: TBaseObject; sMonName: string); // 怪物数据库
var
  I, II: Integer;
  Monster: pTMonInfo;
  MonSuperAbil: TMonSuperAbil;
begin
  if (BaseObject = nil) then
    Exit;
  if FindMonster(sMonName, I) then
  begin
    Monster := MonsterList.Items[I];
    BaseObject.m_btRaceServer := Monster.btRace;
    BaseObject.m_btRaceImg := Monster.btRaceImg;
    BaseObject.m_wAppr := Monster.wAppr;
    if BaseObject.ClassType = TMon36_XMonster then
    begin
      TMon36_XMonster(BaseObject).RefreshAppr;
    end;
    if BaseObject.ClassType = TElectronicScolpionMon then
    begin
      TElectronicScolpionMon(BaseObject).RefreshAppr;
    end;
    BaseObject.m_Abil.Level := Monster.nLevel;
    BaseObject.m_btLifeAttrib := Monster.btLifeAttrib;
    BaseObject.m_btCoolEye := Monster.wCoolEye;
    BaseObject.m_dwFightExp := Monster.dwExp;
    BaseObject.m_Abil.HP := Monster.nHP;
    BaseObject.m_Abil.MaxHP := Monster.nHP;
    // BaseObject.m_btMonsterWeapon := LoByte(Monster.wMP);
    // BaseObject.m_Abil.MP:=Monster.wMP;
    BaseObject.m_Abil.MP := Monster.nMP;
    BaseObject.m_Abil.MaxMP := Monster.nMP;
    BaseObject.m_Abil.AC1 := Monster.nAC;
    BaseObject.m_Abil.AC2 := Monster.nAC;
    BaseObject.m_Abil.MAC1 := Monster.nMAC;
    BaseObject.m_Abil.MAC2 := Monster.nMAC;
    BaseObject.m_Abil.DC1 := Monster.nDC;
    BaseObject.m_Abil.DC2 := Monster.nMaxDC;
    BaseObject.m_Abil.MC1 := Monster.nMC;
    BaseObject.m_Abil.MC2 := Monster.nMC;
    BaseObject.m_Abil.SC1 := Monster.nSC;
    BaseObject.m_Abil.SC2 := Monster.nSC;
    BaseObject.m_btSpeedPoint := Monster.wSpeed;
    BaseObject.m_btHitPoint := Monster.wHitPoint;
    BaseObject.m_nWalkSpeed := Monster.wWalkSpeed;
    BaseObject.m_nWalkStep := Monster.wWalkStep;
    BaseObject.m_dwWalkWait := Monster.wWalkWait;
    BaseObject.m_nNextHitTime := Monster.wAttackSpeed;
    BaseObject.m_wExploreItem := Monster.ExploreItem;
    BaseObject.m_boDisableSimpleActor := Monster.boDisableSimpleActor;
    // BaseObject.m_nAttackState := Monster.nAttackState;
    // BaseObject.m_wAttackSource := Monster.wAttackSource;
    BaseObject.m_nInitWalkSpeed := Monster.wWalkSpeed;
    BaseObject.m_nInitNextHitTime := Monster.wAttackSpeed;
    if GetMonSuperAbil(sMonName, MonSuperAbil) then
    begin
      with BaseObject do
      begin
        FillChar(m_Abil.NewValue, SizeOf(m_Abil.NewValue), 0);
        for II := 0 to Length(m_Abil.NewValue) - 1 do
          if II in [1, 7, 8, 10, 22] then
            m_Abil.NewValue[II] := Min(m_Abil.NewValue[II] + MonSuperAbil.NewValue[II], High(Word))
          else
            m_Abil.NewValue[II] := Min(m_Abil.NewValue[II] + MonSuperAbil.NewValue[II], 100);
        m_boParalysis := m_boParalysis or MonSuperAbil.boParalysis;
        if MonSuperAbil.boParalysis then
        begin
          m_dwParalysisRate := MonSuperAbil.nParalysisRate; // 麻痹机率
          m_dwParalysisTime := MonSuperAbil.nParalysisTime; // 麻痹时长
        end;
        if MonSuperAbil.boUnParalysis then
          m_Abil.NewValue[13] := 100; // 防麻痹
        if MonSuperAbil.boUnPosion then
          m_Abil.NewValue[16] := 100; // 防毒
        if MonSuperAbil.boUnFireCross then
          m_Abil.NewValue[18] := 100; // 防火墙
        if MonSuperAbil.boUnTamming then
          m_Abil.NewValue[17] := 100; // 防诱惑
        if MonSuperAbil.boUnRevival then
          m_Abil.NewValue[15] := 100; // 防复活
        if MonSuperAbil.boUnMagicShield then
          m_Abil.NewValue[14] := 100; // 防护身
        if MonSuperAbil.boUnFrozen then
          m_Abil.NewValue[19] := 100; // 防冰冻
        if MonSuperAbil.boUnCobwebWinding then
          m_Abil.NewValue[20] := 100; // 防蛛网
        m_boFrozen := m_boFrozen or MonSuperAbil.boFrozen;
        if MonSuperAbil.boFrozen then
        begin
          m_dwFrozenRate := MonSuperAbil.nFrozenRate; // 冰冻机率
          m_dwFrozenTime := MonSuperAbil.nFrozenTime; // 冰冻时长
        end;
        m_boCobwebWinding := m_boCobwebWinding or MonSuperAbil.boCobwebWinding;
        if MonSuperAbil.boCobwebWinding then
        begin
          m_dwCobwebWindingRate := MonSuperAbil.nCobwebWindingRate; // 蛛网机率
          m_dwCobwebWindingTime := MonSuperAbil.nCobwebWindingTime; // 蛛网时长
        end;
      end;
    end;
  end;
end;

function TUserEngine.OpenDoor(Envir: TEnvirnoment; nViewRange: Integer; nX, nY: Integer): Boolean;
var
  DoorObject: TDoorObject;
begin
  Result := False;
  DoorObject := Envir.GetDoor(nX, nY);
  if (DoorObject <> nil) and not DoorObject.m_Status.boOpened and not DoorObject.m_Status.bo01 then
  begin
    DoorObject.m_Status.boOpened := True;
    DoorObject.m_Status.dwOpenTick := MyGetTickCount();
    SendDoorStatus(Envir, nViewRange, nX, nY, RM_DOOROPEN, 0, nX, nY, 0, '');
    Result := True;
  end;
end;

function TUserEngine.CloseDoor(Envir: TEnvirnoment; nViewRange: Integer; Door: TDoorObject): Boolean;
begin
  Result := False;
  if (Door <> nil) and (Door.m_Status.boOpened) then
  begin
    Door.m_Status.boOpened := False;
    SendDoorStatus(Envir, nViewRange, Door.m_nMapX, Door.m_nMapY, RM_DOORCLOSE, 0, Door.m_nMapX, Door.m_nMapY, 0, '');
    Result := True;
  end;
end;

procedure TUserEngine.SendDoorStatus(Envir: TEnvirnoment; nViewRange: Integer; nX, nY: Integer; wIdent, wX: Word;
  nDoorX, nDoorY, nA: Integer; sStr: string);
var
  I: Integer;
  n10, n14: Integer;
  n1C, n20, n24, n28: Integer;
  MapCellInfo: pTMapCellinfo;
  BaseObject: TBaseObject;
  GameObject: TGameObject;
begin
  n1C := nX - nViewRange;
  n24 := nX + nViewRange;
  n20 := nY - nViewRange;
  n28 := nY + nViewRange;
  if Envir <> nil then
  begin
    for n10 := n1C to n24 do
    begin
      for n14 := n20 to n28 do
      begin
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          Envir.LockR(1);
        try
{$IFEND}
          if Envir.GetMapCellInfo(n10, n14, MapCellInfo) and (MapCellInfo.ObjList <> nil) then
          begin
            for I := 0 to {$IF USEOBJLIST = 1}Length(MapCellInfo.ObjList){$ELSE}MapCellInfo.ObjList.Count{$IFEND} - 1 do
            begin
              GameObject :=
{$IF USEOBJLIST = 1}TGameObject(MapCellInfo.ObjList[I]){$ELSE}TGameObject(MapCellInfo.ObjList.Items[I]){$IFEND};
              if (GameObject <> nil) and (GameObject.m_ObjGame = Obj_Actor) then
              begin
                BaseObject := TBaseObject(GameObject);
                if (not BaseObject.m_boGhost) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and
                  (abs(BaseObject.m_nCurrX - nDoorX) <= BaseObject.m_nViewRange) and
                  (abs(BaseObject.m_nCurrY - nDoorY) <= BaseObject.m_nViewRange) then
                begin
                  BaseObject.SendMsg(BaseObject, wIdent, wX, nDoorX, nDoorY, nA, sStr);
                end;
              end;
            end;
          end;
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            Envir.UnLockR;
        end;
{$IFEND}
      end;
    end;
  end;
end;

procedure TUserEngine.ProcessMapDoor;
var
  I: Integer;
  II: Integer;
  Envir: TEnvirnoment;
  Door: TDoorObject;
begin
  for I := 0 to g_MapManager.Count - 1 do
  begin
    Envir := TEnvirnoment(g_MapManager.Items[I]);
    if Envir <> nil then
    begin
      for II := 0 to Envir.m_DoorList.Count - 1 do
      begin
        Door := TDoorObject(Envir.m_DoorList.Items[II]);
        if Door <> nil then
        begin
          if Door.m_Status.boOpened then
          begin
            if (MyGetTickCount - Door.m_Status.dwOpenTick) > 5 * 1000 then
              CloseDoor(Envir, g_nSendRefMsgRange, Door);
          end;
        end;
      end;
    end;
  end;
end;

procedure TUserEngine.ProcessEvents;
var
  I, II, III: Integer;
  MagicEvent: pTMagicEvent;
  BaseObject: TBaseObject;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_MagicEventList.LockW(1);
  try
{$IFEND}
    for I := m_MagicEventList.Count - 1 downto 0 do
    begin
      if m_MagicEventList.Count <= 0 then
        Break;
      MagicEvent := m_MagicEventList.Items[I];
      if (MagicEvent <> nil) and (MagicEvent.BaseObjectList_2 <> nil) then
      begin
        if not MagicEvent.FormNPC then
        begin
          for II := MagicEvent.BaseObjectList_2.Count - 1 downto 0 do
          begin
            BaseObject := TBaseObject(MagicEvent.BaseObjectList_2.Items[II]);
            if BaseObject <> nil then
            begin
              if BaseObject.m_boDeath or (BaseObject.m_boGhost) or (not(BaseObject.m_boHolySeize or BaseObject.m_boImprison)) then
              begin
                MagicEvent.BaseObjectList_2.Delete(II);
              end;
            end;
          end;
        end
        else
        begin
          for II := MagicEvent.BaseObjectList_2.Count - 1 downto 0 do
          begin
            BaseObject := TBaseObject(MagicEvent.BaseObjectList_2.Items[II]);
            if BaseObject <> nil then
            begin
              if (not BaseObject.m_boImprison) then
              begin
                MagicEvent.BaseObjectList_2.Delete(II);
              end;
            end;
          end;
        end;
        if (MagicEvent.BaseObjectList_2.Count <= 0) or (tick_diff(MagicEvent.dwStartTick, MyGetTickCount) > MagicEvent.dwTime) or
          (tick_diff(MagicEvent.dwStartTick, MyGetTickCount) > 180000) then
        begin
          MagicEvent.BaseObjectList_2.Free;
          for III := MagicEvent.Events_2.Count - 1 downto 0 do
          begin
            if MagicEvent.Events_2[III] <> nil then
            begin
              TGameEvent(MagicEvent.Events_2[III]).Close();
            end;
          end;
          MagicEvent.Events_2.Free;
          Dispose(MagicEvent);
          m_MagicEventList.Delete(I);
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_MagicEventList.UnLockW;
  end;
{$IFEND}
end;

function TUserEngine.FindDummyMagic(IsHuman: Boolean; sMagicName: string): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockR(7);
    try
{$IFEND}
      if IsHuman then
      begin
        MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
        if MagicList <> nil then
        begin
          for I := 0 to MagicList.Count - 1 do
          begin
            Magic := MagicList.Items[I];
            if (Magic <> nil) and (Magic.MagicAttr <> mtHero) then
            begin
              if CompareText(Magic.sMagicName, sMagicName) = 0 then
              begin
                Result := Magic;
                Break;
              end;
            end;
          end;
        end;
      end
      else
      begin
        MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
        if MagicList <> nil then
        begin
          for I := 0 to MagicList.Count - 1 do
          begin
            Magic := MagicList.Items[I];
            if (Magic <> nil) and (Magic.MagicAttr = mtHero) then
            begin
              if CompareText(Magic.sMagicName, sMagicName) = 0 then
              begin
                Result := Magic;
                Break;
              end;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockR(9);
    try
{$IFEND}
      if IsHuman then
      begin
        for I := 0 to m_MagicList.Count - 1 do
        begin
          Magic := m_MagicList.Items[I];
          if (Magic <> nil) and ((Magic.MagicAttr <> mtHero) or (Magic.wMagicID in [60 .. 65])) then
          begin
            if CompareText(Magic.sMagicName, sMagicName) = 0 then
            begin
              Result := Magic;
              Break;
            end;
          end;
        end;
      end
      else
      begin
        for I := 0 to m_MagicList.Count - 1 do
        begin
          Magic := m_MagicList.Items[I];
          if (Magic <> nil) and ((Magic.MagicAttr = mtHero) or (Magic.wMagicID in [60 .. 65])) then
          begin
            if CompareText(Magic.sMagicName, sMagicName) = 0 then
            begin
              Result := Magic;
              Break;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockR;
    end;
{$IFEND}
  end;
end;

function TUserEngine.FindMagic(sMagicName: string): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockR(8);
    try
{$IFEND}
      MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
      if MagicList <> nil then
      begin
        for I := 0 to MagicList.Count - 1 do
        begin
          Magic := MagicList.Items[I];
          if (Magic <> nil) and (Magic.MagicAttr <> mtHero) then
          begin
            if CompareText(Magic.sMagicName, sMagicName) = 0 then
            begin
              Result := Magic;
              Break;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockR(10);
    try
{$IFEND}
      for I := 0 to m_MagicList.Count - 1 do
      begin
        Magic := m_MagicList.Items[I];
        if (Magic <> nil) and (Magic.MagicAttr <> mtHero) then
        begin
          if CompareText(Magic.sMagicName, sMagicName) = 0 then
          begin
            Result := Magic;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockR;
    end;
{$IFEND}
  end;
end;

function TUserEngine.FindHeroMagic(sMagicName: string): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockR(9);
    try
{$IFEND}
      MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
      if MagicList <> nil then
      begin
        for I := 0 to MagicList.Count - 1 do
        begin
          Magic := MagicList.Items[I];
          if (Magic <> nil) and (Magic.MagicAttr <> mtHum) then
          begin
            if CompareText(Magic.sMagicName, sMagicName) = 0 then
            begin
              Result := Magic;
              Break;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockR(11);
    try
{$IFEND}
      for I := 0 to m_MagicList.Count - 1 do
      begin
        Magic := m_MagicList.Items[I];
        if (Magic <> nil) and (Magic.MagicAttr <> mtHum) then
        begin
          if CompareText(Magic.sMagicName, sMagicName) = 0 then
          begin
            Result := Magic;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockR;
    end;
{$IFEND}
  end;
end;

function TUserEngine.FindMagic(sMagicName: string; MagicAttr: TMagicAttr): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockR(10);
    try
{$IFEND}
      MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
      if MagicList <> nil then
      begin
        for I := 0 to MagicList.Count - 1 do
        begin
          Magic := MagicList.Items[I];
          if (Magic <> nil) and (Magic.MagicAttr = MagicAttr) then
          begin
            if CompareText(Magic.sMagicName, sMagicName) = 0 then
            begin
              Result := Magic;
              Break;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockR(12);
    try
{$IFEND}
      for I := 0 to m_MagicList.Count - 1 do
      begin
        Magic := m_MagicList.Items[I];
        if (Magic <> nil) and (Magic.MagicAttr = MagicAttr) then
        begin
          if CompareText(Magic.sMagicName, sMagicName) = 0 then
          begin
            Result := Magic;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockR;
    end;
{$IFEND}
  end;
end;

function TUserEngine.FindHeroMagic(sMagicName: string; MagicAttr: TMagicAttr): pTMagic;
var
  I: Integer;
  Magic: pTMagic;
  MagicList: TList;
begin
  Result := nil;
  if (m_boStartLoadMagic) and (OldMagicList.Count > 0) then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockR(11);
    try
{$IFEND}
      MagicList := TList(OldMagicList.Items[OldMagicList.Count - 1]);
      if MagicList <> nil then
      begin
        for I := 0 to MagicList.Count - 1 do
        begin
          Magic := MagicList.Items[I];
          if (Magic <> nil) and (Magic.MagicAttr = MagicAttr) then
          begin
            if CompareText(Magic.sMagicName, sMagicName) = 0 then
            begin
              Result := Magic;
              Break;
            end;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockR;
    end;
{$IFEND}
  end
  else
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      m_MagicList.LockR(13);
    try
{$IFEND}
      for I := 0 to m_MagicList.Count - 1 do
      begin
        Magic := m_MagicList.Items[I];
        if (Magic <> nil) and (Magic.MagicAttr = MagicAttr) then
        begin
          if CompareText(Magic.sMagicName, sMagicName) = 0 then
          begin
            Result := Magic;
            Break;
          end;
        end;
      end;
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        m_MagicList.UnLockR;
    end;
{$IFEND}
  end;
end;

procedure TUserEngine.AddMerchant(Merchant: TMerchant);
begin
  m_MerchantList.LockW(7);
  try
    m_MerchantList.Add(Merchant);
  finally
    m_MerchantList.UnLockW;
  end;
end;

procedure TUserEngine.AddNpc(const ACharName: string; AMerchant: TBaseObject);
begin
  QuestNPCList.LockW(4);
  try
    QuestNPCList.AddObject(ACharName, AMerchant);
  finally
    QuestNPCList.UnLockW;
  end;
end;

function TUserEngine.AddScriptCreateNpcInfo(ANpc: TNormNpc; ABaseObj: TBaseObject; APlayObj: TPlayObject;
  AQuestActionInfo: pTQuestActionInfo): Boolean; // Cursor 2023-06-01 17:11:13
var
  I: Integer;
  tmpScriptParam: PScriptParamter;
begin
  Result := True;
  for I := 0 to FScriptCreateNpcInfos.Count - 1 do
  begin
    tmpScriptParam := PScriptParamter(FScriptCreateNpcInfos[I]);
    if (tmpScriptParam.NPC = ANpc) //
      and (tmpScriptParam.BaseObj = ABaseObj) //
      and (tmpScriptParam.PlayObj = APlayObj) //
      and CompareMem(@(tmpScriptParam.QuestActionInfo), AQuestActionInfo, SizeOf(TQuestActionInfo)) then
    begin
      Result := False;
      Exit;
    end;
  end;

  tmpScriptParam := AllocMem(SizeOf(TScriptParamter));
  tmpScriptParam.NPC := ANpc;
  tmpScriptParam.BaseObj := ABaseObj;
  tmpScriptParam.PlayObj := APlayObj;
  tmpScriptParam.QuestActionInfo := AQuestActionInfo^;
  FScriptCreateNpcInfos.Add(tmpScriptParam);
end;

function TUserEngine.DelScriptCreateNpcInfo(const AMapName: string): Boolean;
var
  I: Integer;
  tmpScriptParam: PScriptParamter;
begin
  Result := False;
  for I := FScriptCreateNpcInfos.Count - 1 downto 0 do
  begin
    tmpScriptParam := PScriptParamter(FScriptCreateNpcInfos[I]);
    if (tmpScriptParam = nil) or SameText(AMapName, tmpScriptParam.QuestActionInfo.sParam2) then
    begin
      FScriptCreateNpcInfos.Delete(I);
      FreeMemory(tmpScriptParam);
      Result := True;
    end;
  end;
end;

function TUserEngine.DelScriptCreateNpcInfo(const ANpcName, AMapName: string): Boolean;
var
  I: Integer;
  tmpScriptParam: PScriptParamter;
begin
  Result := False;
  for I := FScriptCreateNpcInfos.Count - 1 downto 0 do
  begin
    tmpScriptParam := PScriptParamter(FScriptCreateNpcInfos[I]);
    if (tmpScriptParam = nil) //
      or (SameText(ANpcName, tmpScriptParam.QuestActionInfo.sParam1) //
      and (tmpScriptParam.QuestActionInfo.sParam2.IsEmpty or SameText(AMapName, tmpScriptParam.QuestActionInfo.sParam2))) then
    begin
      FScriptCreateNpcInfos.Delete(I);
      FreeMemory(tmpScriptParam);
      Result := True;
    end;
  end;
end;

function TUserEngine.GetMerchantList(Envir: TEnvirnoment; nX, nY, nRange: Integer; TmpList: TList): Integer;
var
  I: Integer;
  Merchant: TMerchant;
begin
  m_MerchantList.LockR(8);
  try
    for I := 0 to m_MerchantList.Count - 1 do
    begin
      Merchant := TMerchant(m_MerchantList.Items[I]);
      if Merchant <> nil then
      begin
        if (Merchant.m_PEnvir = Envir) //
          and (abs(Merchant.m_nCurrX - nX) <= nRange) //
          and (abs(Merchant.m_nCurrY - nY) <= nRange) then
          TmpList.Add(Merchant);
      end;
    end;
  finally
    m_MerchantList.UnLockR;
  end;
  Result := TmpList.Count
end;

function TUserEngine.GetNpcList(Envir: TEnvirnoment; nX, nY, nRange: Integer; TmpList: TList): Integer;
var
  I: Integer;
  NPC: TNormNpc;
begin
  QuestNPCList.LockR(5);
  try
    for I := 0 to QuestNPCList.Count - 1 do
    begin
      NPC := TNormNpc(QuestNPCList.Objects[I]);
      if NPC <> nil then
      begin
        if (NPC.m_PEnvir = Envir) and (abs(NPC.m_nCurrX - nX) <= nRange) and (abs(NPC.m_nCurrY - nY) <= nRange) then
        begin
          TmpList.Add(NPC);
        end;
      end;
    end;
  finally
    QuestNPCList.UnLockR;
  end;
  Result := TmpList.Count
end;

procedure TUserEngine.ReloadMerchantList();
var
  I: Integer;
  List: TList;
  Merchant: TMerchant;
  tmpSendNpcSay, tmpboBreak: Boolean;
begin
  List := TList.Create;
  try
    m_MerchantList.LockR(9);
    try
      for I := 0 to m_MerchantList.Count - 1 do
      begin
        Merchant := TMerchant(m_MerchantList.Items[I]);
        if Merchant <> nil then
          List.Add(Merchant);
      end;
    finally
      m_MerchantList.UnLockR;
    end;

    for I := 0 to List.Count - 1 do
    begin
      Merchant := List.Items[I];
      Application.ProcessMessages;

      if Merchant.m_boGhost then // 不处理被剔除的NPC Cursor 2023-08-14 14:10:53
        Continue;

      // 优化npc加载 2020-05-18 00:37:23
      if not FrmDB.CheckScriptFileChanged(Merchant, True) then
        Merchant.LoadNpcIconFile
      else
      begin
        Merchant.ClearScript;
        Merchant.LoadNpcScript;
      end;
    end;

    // 加载由脚本创建的NPC
    for I := FScriptCreateNpcInfos.Count - 1 downto 0 do
    begin
      ActionOfCreateNpc(PScriptParamter(FScriptCreateNpcInfos[I]).NPC, //
        PScriptParamter(FScriptCreateNpcInfos[I]).BaseObj, //
        PScriptParamter(FScriptCreateNpcInfos[I]).PlayObj, //
        @PScriptParamter(FScriptCreateNpcInfos[I]).QuestActionInfo, //
        tmpSendNpcSay, //
        tmpboBreak);
    end;
  finally
    List.Free;
  end;
end;

procedure TUserEngine.ReloadNpcList();
var
  I: Integer;
  NPC: TNormNpc;
  List: TList;
  // StartTick: LongWord;
begin
  List := TList.Create;
  try
    QuestNPCList.LockR(6);
    try
      for I := 0 to QuestNPCList.Count - 1 do
      begin
        NPC := TNormNpc(QuestNPCList.Objects[I]);
        if NPC <> nil then
          List.Add(NPC);
      end;
    finally
      QuestNPCList.UnLockR;
    end;

    // StartTick := MyGetTickCount;
    for I := 0 to List.Count - 1 do
    begin
      Application.ProcessMessages;
      NPC := List.Items[I];
      // 重新加载NPC，镜像地图内的NPC失效 chongchong 2019-03-06 13:53:52
      if not((NPC <> nil) and (NPC.m_PEnvir <> nil) and (NPC.m_PEnvir.m_boMirror)) then
      begin
        if not FrmDB.CheckScriptFileChanged(NPC, False) then // 优化npc加载 2020-05-18 00:37:23
          NPC.LoadNpcIconFile
        else
        begin
          NPC.ClearScript;
          NPC.LoadNpcScript;
        end;
      end;
    end;
    // MainOutMessage('NPC重新加载用时:' + IntToStr(MyGetTickCount - StartTick));
  finally
    List.Free;
  end;
end;

function TUserEngine.GetMapMonster(Envir: TEnvirnoment; List: TList; boIncludeDie: Boolean): Integer;
var
  I, II: Integer;
  MonGen: pTMonGenInfo;
  BaseObject: TBaseObject;
begin
  Result := 0;
  if Envir = nil then
    Exit;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_MonGenList.LockR(12);
  try
{$IFEND}
    for I := 0 to m_MonGenList.Count - 1 do
    begin
      MonGen := m_MonGenList.Items[I];
      if MonGen = nil then
        Continue;
      for II := 0 to MonGen.CertList.Count - 1 do
      begin
        BaseObject := TBaseObject(MonGen.CertList.Items[II]);
        if BaseObject <> nil then
        begin
          if (((not BaseObject.m_boDeath) and (not BaseObject.m_boGhost)) or boIncludeDie) and (BaseObject.m_PEnvir = Envir) then
          begin
            if List <> nil then
              List.Add(BaseObject);
            Inc(Result);
          end;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_MonGenList.UnLockR;
  end;
{$IFEND}
end;

function TUserEngine.FindMapMonster(Envir: TEnvirnoment; sName: string; nX, nY: Integer): TBaseObject;
var
  I, II: Integer;
  MonGen: pTMonGenInfo;
  BaseObject: TBaseObject;
  nXR, nYR: Integer;
  boExit: Boolean;
begin
  Result := nil;
  if Envir = nil then
    Exit;
  nXR := 99999;
  nYR := 99999;
  boExit := False;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_MonGenList.LockR(12);
  try
{$IFEND}
    for I := 0 to m_MonGenList.Count - 1 do
    begin
      MonGen := m_MonGenList.Items[I];
      if MonGen = nil then
        Continue;
      for II := 0 to MonGen.CertList.Count - 1 do
      begin
        BaseObject := TBaseObject(MonGen.CertList.Items[II]);
        if BaseObject <> nil then
        begin
          if (((not BaseObject.m_boDeath) and (not BaseObject.m_boGhost))) and (BaseObject.m_PEnvir = Envir) and
            (BaseObject.m_sCharName = sName) then
          begin
            if nX = -1 then
            begin
              Result := BaseObject;
              boExit := True;
              Break;
            end;
            if ((abs(nX - BaseObject.m_nCurrX) < nXR) or (abs(nY - BaseObject.m_nCurrY) < nYR)) and
              (abs(nX - BaseObject.m_nCurrX) + abs(nY - BaseObject.m_nCurrY) < nXR + nYR) then
            begin
              if (abs(nX - BaseObject.m_nCurrX) < nXR) then
                nXR := abs(nX - BaseObject.m_nCurrX);
              if (abs(nY - BaseObject.m_nCurrY) < nYR) then
                nYR := abs(nY - BaseObject.m_nCurrY);
              Result := BaseObject;
            end;
          end;
        end;
      end;
      if boExit then
        Break;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_MonGenList.UnLockR;
  end;
{$IFEND}
end;

procedure TUserEngine.HumanExpire(sAccount: string);
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  if not g_Config.boKickExpireHuman then
    Exit;
  m_PlayObjectList.LockR(22);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if CompareText(PlayObject.m_sUserID, sAccount) = 0 then
        begin
          PlayObject.m_boExpire := True;
          Break;
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetMapHuman(sMapName: string): Integer;
var
  I: Integer;
  Envir: TEnvirnoment;
  PlayObject: TPlayObject;
begin
  Result := 0;
  Envir := g_MapManager.FindMap(sMapName);
  if Envir = nil then
    Exit;
  m_PlayObjectList.LockR(23);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if not PlayObject.m_boDeath and not PlayObject.m_boGhost and (PlayObject.m_PEnvir = Envir) then
          Inc(Result);
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetMapHuman(Envir: TEnvirnoment): Integer;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := 0;
  if Envir = nil then
    Exit;
  m_PlayObjectList.LockR(24);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if not PlayObject.m_boDeath and not PlayObject.m_boGhost and (PlayObject.m_PEnvir = Envir) then
          Inc(Result);
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetMapRageHuman(Envir: TEnvirnoment; nRageX, nRageY, nRage: Integer; List: TList): Integer;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  Result := 0;
  m_PlayObjectList.LockR(25);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if (not PlayObject.m_boDeath) and (not PlayObject.m_boGhost) and (PlayObject.m_PEnvir = Envir) and
          (abs(PlayObject.m_nCurrX - nRageX) <= nRage) and (abs(PlayObject.m_nCurrY - nRageY) <= nRage) then
        begin
          if List <> nil then
            List.Add(PlayObject);
          Inc(Result);
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

function TUserEngine.GetStdItemIdx(sItemName: string): Integer;
var
  I: Integer;
  StdItem: PStdItemEx;
begin
  Result := -1;
  if sItemName = '' then
    Exit;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    SortStdItemList.Lock;
  try
{$IFEND}
    I := SortStdItemList.IndexOf(sItemName);
    if I >= 0 then
    begin
      StdItem := PStdItemEx(SortStdItemList.Objects[I]);
      Result := StdItem.ItemIdx + 1;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      SortStdItemList.UnLock;
  end;
{$IFEND}
end;

// ==========================================
// 向每个人物发送消息
// 线程安全
// ==========================================
procedure TUserEngine.SendServerConfig();
var
  I: Integer;
  PlayObject: TPlayObject;
  OldCRC32: LongWord;
begin
  OldCRC32 := g_ServerConfigTextCRC;
  RebuildSendServerConfigText;

  if OldCRC32 <> g_ServerConfigTextCRC then
  begin
    m_PlayObjectList.LockR(26);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);

        if (PlayObject <> nil) //
          and (not PlayObject.m_boGhost) //
          and (not PlayObject.m_boOffLine) //
          and (not PlayObject.m_boDummyObject) then
          PlayObject.SendMsg(PlayObject, RM_SERVERCONFIG, 0, 0, 0, 0, '');
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
  end;
end;

procedure TUserEngine.SendCustomItemPropertyConfig();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(26);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if (not PlayObject.m_boGhost) and (not PlayObject.m_boOffLine) and (not PlayObject.m_boDummyObject) then
          PlayObject.SendCustomItemPropertyConfig;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendCustomItemPropertyTextVarList();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(26);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if (not PlayObject.m_boGhost) and (not PlayObject.m_boOffLine) and (not PlayObject.m_boDummyObject) then
          PlayObject.SendCustomItemPropertyTextVarList;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

{ TODO -opiaoyun -c修改 : 修改穿NPC等无效问题【2013-07-19】 }
procedure TUserEngine.SendMapCanRun();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(27);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if (not PlayObject.m_boGhost) and (not PlayObject.m_boOffLine) and (not PlayObject.m_boDummyObject) then
          PlayObject.SendMapCanRun;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.ReloadMagicList();
var
  I, II: Integer;
  PlayObject: TPlayObject;
  UserMagic: PTUserMagic;
begin
  m_PlayObjectList.LockR(28);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        for II := 0 to PlayObject.m_MagicList.Count - 1 do
        begin
          UserMagic := PlayObject.m_MagicList.Items[II];
          UserMagic.MagicInfo := FindMagic(UserMagic.wMagIdx, UserMagic.MagicAttr);
        end;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.ReloadHeroMagicList();
var
  I, II: Integer;
  HeroObject: THeroObject;
  UserMagic: PTUserMagic;
begin
  m_HeroObjectList.LockR(8);
  try
    for I := 0 to m_HeroObjectList.Count - 1 do
    begin
      HeroObject := THeroObject(m_HeroObjectList.Objects[I]);
      if HeroObject <> nil then
      begin
        for II := 0 to HeroObject.m_MagicList.Count - 1 do
        begin
          UserMagic := HeroObject.m_MagicList.Items[II];
          UserMagic.MagicInfo := FindHeroMagic(UserMagic.wMagIdx, UserMagic.MagicAttr);
        end;
      end;
    end;
  finally
    m_HeroObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendBroadCastMsgExt(sMsg: string; MsgType: TMsgType; MsgFrom: TMsgFrom);
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(29);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject <> nil then
      begin
        if (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
          (not PlayObject.m_boDummyObject) then
          PlayObject.SysMsg(sMsg, c_Red, MsgType);
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.ChangeMyShopType;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(30);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      PlayObject.ChangeMyShopType();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendMissionNpc();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(31);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendMissionNpc();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendSpecialCmdList(); // 发送特殊命令
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(32);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendSpecialCmdList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendEffectImageList();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(33);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);

      if (PlayObject <> nil) //
        and (not PlayObject.m_boGhost) //
        and (not PlayObject.m_boDeath) //
        and (not PlayObject.m_boOffLine) //
        and (not PlayObject.m_boDummyObject) then
        PlayObject.SendEffectImageList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendUnbindList();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(34);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendUnbindList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

// 发送所有物品 chongchong 2015-01-03
procedure TUserEngine.SendStdItemList();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(35);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) and (PlayObject.m_dwStdItemListTextCRC <> g_StdItemListTextCRC) then
        PlayObject.SendStdItemList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

// 刷新名称-神秘人 piaoyun 2013-08-17
procedure TUserEngine.RefShowName();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(36);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.RefShowName();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendPlugClientList();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(37);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendPlugClientList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendClientModules();
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(38);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
      begin
        PlayObject.SendClientBlackModules();
        PlayObject.SendClientModules();
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendFilterItemList;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(39);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendFilterItemList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendItemDescList;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(40);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendItemDescList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendItemDescTopList;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(41);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendItemDescTopList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendTzItemDescList;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(42);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendTzItemDescList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendDropItemEffectList;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(420);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendDropItemEffectList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendEnabledAuctionItemList;
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(420);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
        PlayObject.SendEnabledAuctionItemList();
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendBroadCastMsg(sMsg: string; MsgType: TMsgType; MsgFrom: TMsgFrom);
var
  I: Integer;
  PlayObject: TPlayObject;
  // DefMsg: TDefaultMessage;
  // SocketThread: TSocketThread;
begin
  if g_Config.boShowPreFixMsg then
  begin
    case MsgType of
      t_Mon:
        sMsg := g_Config.sMonSayMsgpreFix + sMsg;
      t_Hint:
        sMsg := g_Config.sHintMsgPreFix + sMsg;
      {
        s_GroupMsg: sMsg:=g_Config.sGroupMsgPreFix + sMsg;
        s_GuildMsg: sMsg:=g_Config.sGuildMsgPreFix + sMsg;
      }
      t_GM:
        sMsg := g_Config.sGMRedMsgpreFix + sMsg;
      t_System:
        sMsg := g_Config.sSysMsgPreFix + sMsg;
      t_Notice:
        sMsg := g_Config.sLineNoticePreFix + sMsg;
      t_Cust:
        sMsg := g_Config.sCustMsgpreFix + sMsg;
      t_Castle:
        sMsg := g_Config.sCastleMsgpreFix + sMsg;
    end;
  end;
  {
    if g_Config.boSendOptimizeDataToRunGate then
    begin
    DefMsg := MakeDefaultMsg(SM_SYSMESSAGE, 0, MakeWord(g_Config.btRedMsgFColor, g_Config.btRedMsgBColor), 0, 1);
    SocketThread := RunSocket.GetActiveSocket;
    if SocketThread <> nil then
    begin
    SocketThread.Add(GM_FULL_SERVICE_MSG, 0, 0, 0, @DefMsg, PChar(sMsg), Length(sMsg));
    end;
    end
    else
  }
  begin
    m_PlayObjectList.LockR(43);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if (PlayObject <> nil) and (not PlayObject.m_boGhost) { and (not PlayObject.m_boDeath) }
          and (not PlayObject.m_boOffLine) and (not PlayObject.m_boDummyObject) then
        begin
          if not(((MsgFrom = mfDropItem) and (PlayObject.m_boFilterGlobalDropItemMsg)) or
            ((MsgFrom = mfSendMsg) and (PlayObject.m_boFilterGolbalSendMsg))) then
          begin
            PlayObject.SysMsgEx(sMsg, c_Red, MsgType);
          end;
        end;
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
  end;
end;

procedure TUserEngine.SendBroadCastMsg(sMsg: string; FColor, BColor: Integer; MsgType: TMsgType; MsgFrom: TMsgFrom);
var
  I: Integer;
  PlayObject: TPlayObject;
  // DefMsg: TDefaultMessage;
  // SocketThread: TSocketThread;
begin
  case MsgType of
    t_Mon:
      sMsg := g_Config.sMonSayMsgpreFix + sMsg;
    t_Hint:
      sMsg := g_Config.sHintMsgPreFix + sMsg;
    {
      s_GroupMsg: sMsg:=g_Config.sGroupMsgPreFix + sMsg;
      s_GuildMsg: sMsg:=g_Config.sGuildMsgPreFix + sMsg;
    }
    t_GM:
      sMsg := g_Config.sGMRedMsgpreFix + sMsg;
    t_System:
      sMsg := g_Config.sSysMsgPreFix + sMsg;
    t_Notice:
      sMsg := g_Config.sLineNoticePreFix + sMsg;
    t_Cust:
      sMsg := g_Config.sCustMsgpreFix + sMsg;
    t_Castle:
      sMsg := g_Config.sCastleMsgpreFix + sMsg;
  end;
  {
    if g_Config.boSendOptimizeDataToRunGate then
    begin
    DefMsg := MakeDefaultMsg(SM_SYSMESSAGE, 0, MakeWord(FColor, BColor), 0, 1);
    SocketThread := RunSocket.GetActiveSocket;
    if SocketThread <> nil then
    begin
    SocketThread.Add(GM_FULL_SERVICE_MSG, 0, 0, 0, @DefMsg, PChar(sMsg), Length(sMsg));
    end;
    end
    else
  }
  begin
    m_PlayObjectList.LockR(44);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if (PlayObject <> nil) and (not PlayObject.m_boGhost) { and (not PlayObject.m_boDeath) } and (not PlayObject.m_boOffLine)
          and (not PlayObject.m_boDummyObject) then
        begin
          if not(((MsgFrom = mfDropItem) and (PlayObject.m_boFilterGlobalDropItemMsg)) or
            ((MsgFrom = mfSendMsg) and (PlayObject.m_boFilterGolbalSendMsg))) then
          begin
            PlayObject.SysMsgEx(sMsg, FColor, BColor, MsgType);
          end;
        end;
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
  end;
end;

procedure TUserEngine.SendTopBroadCastMsg(sMsg: string; FColor, BColor: Integer; nTime: Integer; MsgType: TMsgType;
  MsgFrom: TMsgFrom);
var
  I: Integer;
  PlayObject: TPlayObject;
  // DefMsg: TDefaultMessage;
  // SocketThread: TSocketThread;
begin
  if g_Config.boShowPreFixMsg then
  begin
    case MsgType of
      t_Mon:
        sMsg := g_Config.sMonSayMsgpreFix + sMsg;
      t_Hint:
        sMsg := g_Config.sHintMsgPreFix + sMsg;
      {
        s_GroupMsg: sMsg:=g_Config.sGroupMsgPreFix + sMsg;
        s_GuildMsg: sMsg:=g_Config.sGuildMsgPreFix + sMsg;
      }
      t_GM:
        sMsg := g_Config.sGMRedMsgpreFix + sMsg;
      t_System:
        sMsg := g_Config.sSysMsgPreFix + sMsg;
      t_Notice:
        sMsg := g_Config.sLineNoticePreFix + sMsg;
      t_Cust:
        sMsg := g_Config.sCustMsgpreFix + sMsg;
      t_Castle:
        sMsg := g_Config.sCastleMsgpreFix + sMsg;
    end;
  end;
  {
    if g_Config.boSendOptimizeDataToRunGate then
    begin
    DefMsg := MakeDefaultMsg(SM_TOPCHATBOARDMESSAGE, nTime, MakeWord(FColor, BColor), Word(MsgType), 0);
    SocketThread := RunSocket.GetActiveSocket;
    if SocketThread <> nil then
    begin
    SocketThread.Add(GM_FULL_SERVICE_MSG, 0, 0, 0, @DefMsg, PChar(sMsg), Length(sMsg));
    end;
    end
    else
  }
  begin
    m_PlayObjectList.LockR(45);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if PlayObject <> nil then
        begin
          if (PlayObject <> nil) and (not PlayObject.m_boGhost) { and (not PlayObject.m_boDeath) } and
            (not PlayObject.m_boOffLine) and (not PlayObject.m_boDummyObject) then
          begin
            if not(((MsgFrom = mfDropItem) and (PlayObject.m_boFilterGlobalDropItemMsg)) or
              ((MsgFrom = mfSendMsg) and (PlayObject.m_boFilterGolbalSendMsg))) then
            begin
              PlayObject.SendTopBroadCastMsgEx(sMsg, FColor, BColor, nTime, MsgType);
            end;
          end;
        end;
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
  end;
end;

procedure TUserEngine.SendMoveMsg(sMsg: string; btFColor, btBColor: Byte; nY, nMoveCount: Integer; nFontSize: Integer;
  nMarqueeTime: Integer);
var
  I: Integer;
  PlayObject: TPlayObject;
  // DefMsg: TDefaultMessage;
  // SocketThread: TSocketThread;
begin
  {
    if g_Config.boSendOptimizeDataToRunGate then
    begin
    DefMsg := MakeDefaultMsg(SM_MOVEMESSAGE, MakeLong(nMoveCount, nMarqueeTime), MakeWord(btFColor, btBColor), nFontSize, nY);
    SocketThread := RunSocket.GetActiveSocket;
    if SocketThread <> nil then
    begin
    SocketThread.Add(GM_FULL_SERVICE_MSG, 0, 0, 0, @DefMsg, PChar(sMsg), Length(sMsg));
    end;
    end
    else
  }
  begin
    m_PlayObjectList.LockR(46);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
          (not PlayObject.m_boDummyObject) then
        begin
          PlayObject.SendMoveMsgEx(sMsg, btFColor, btBColor, nY, nMoveCount, nFontSize, nMarqueeTime)
        end;
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
  end;
end;

// 发送屏幕震动消息 piaoyun 2013-09-14
procedure TUserEngine.SendSceneShake(Count: Integer);
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(47);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) then
      begin
        PlayObject.SendSceneShake(Count);
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

// 仿盛大顶部渐隐消息 piaoyun 2013-08-01
procedure TUserEngine.SendSuperMoveMsg(sMsg: string; btFColor, btBColor, btFontSize: Byte; nX, nY, nMoveCount: Integer);
var
  I: Integer;
  PlayObject: TPlayObject;
  // DefMsg: TDefaultMessage;
  // SocketThread: TSocketThread;
begin
  {
    if g_Config.boSendOptimizeDataToRunGate then
    begin
    DefMsg := MakeDefaultMsg(SM_SUPERMOVEMESSAGE, nMoveCount, MakeWord(btFColor, btBColor), nY, btFontSize);
    SocketThread := RunSocket.GetActiveSocket;
    if SocketThread <> nil then
    begin
    SocketThread.Add(GM_FULL_SERVICE_MSG, 0, 0, 0, @DefMsg, PChar(sMsg), Length(sMsg));
    end;
    end
    else
  }
  begin
    m_PlayObjectList.LockR(48);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
          (not PlayObject.m_boDummyObject) then
        begin
          PlayObject.SendSuperMoveMsgEx(sMsg, btFColor, btBColor, btFontSize, nX, nY, nMoveCount);
        end;
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
  end;
end;

// 换行消息 piaoyun 2013-08-03
procedure TUserEngine.SendNewLineMsg(sMsg: string; btFColor, btBColor, btFontSize: Byte;
  nX, nY, nShowMsgTime, nDrawType: Integer);
var
  I: Integer;
  PlayObject: TPlayObject;
  // DefMsg: TDefaultMessage;
  // SocketThread: TSocketThread;
begin
  {
    if g_Config.boSendOptimizeDataToRunGate then
    begin
    DefMsg := MakeDefaultMsg(SM_NEWLINEMESSAGE, nDrawType, MakeWord(btFColor, btBColor), nY, MakeWord(btFontSize, nShowMsgTime));
    SocketThread := RunSocket.GetActiveSocket;
    if SocketThread <> nil then
    begin
    SocketThread.Add(GM_FULL_SERVICE_MSG, 0, 0, 0, @DefMsg, PChar(sMsg), Length(sMsg));
    end;
    end
    else
  }
  begin
    m_PlayObjectList.LockR(49);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
          (not PlayObject.m_boDummyObject) then
        begin
          PlayObject.SendNewLineMsgEx(sMsg, btFColor, btBColor, btFontSize, nX, nY, nShowMsgTime, nDrawType);
        end;
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
  end;
end;

procedure TUserEngine.SendCenterMsg(sMsg: string; btFColor, btBColor: Byte; nTime: Integer);
var
  I: Integer;
  PlayObject: TPlayObject;
  // DefMsg: TDefaultMessage;
  // SocketThread: TSocketThread;
begin
  {
    if g_Config.boSendOptimizeDataToRunGate then
    begin
    DefMsg := MakeDefaultMsg(SM_CENTERMESSAGE, nTime, MakeWord(btFColor, btBColor), 0, 0);
    SocketThread := RunSocket.GetActiveSocket;
    if SocketThread <> nil then
    begin
    SocketThread.Add(GM_FULL_SERVICE_MSG, 0, 0, 0, @DefMsg, PChar(sMsg), Length(sMsg));
    end;
    end
    else
  }
  begin
    m_PlayObjectList.LockR(50);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
          (not PlayObject.m_boDummyObject) then
        begin
          PlayObject.SendCenterMsgEx(sMsg, btFColor, btBColor, nTime);
        end;
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
  end;
end;

procedure TUserEngine.WeatherChanged(Envir: TEnvirnoment);
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(51);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) and (PlayObject.m_PEnvir = Envir) then
      begin
        PlayObject.WeatherChanged;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.SendMapDescription(Envir: TEnvirnoment);
var
  I: Integer;
  PlayObject: TPlayObject;
begin
  m_PlayObjectList.LockR(52);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if (PlayObject <> nil) and (not PlayObject.m_boGhost) and (not PlayObject.m_boDeath) and (not PlayObject.m_boOffLine) and
        (not PlayObject.m_boDummyObject) and (PlayObject.m_PEnvir = Envir) then
      begin
        PlayObject.SendMapDescription;
      end;
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

procedure TUserEngine.Execute;
begin
  Run;
end;

procedure TUserEngine.ClearMonSayMsg;
var
  I, II: Integer;
  MonGen: pTMonGenInfo;
  MonBaseObject: TBaseObject;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    m_MonGenList.LockR(13);
  try
{$IFEND}
    for I := 0 to m_MonGenList.Count - 1 do
    begin
      MonGen := m_MonGenList.Items[I];
      if (MonGen <> nil) and (MonGen.CertList <> nil) then
      begin
        for II := 0 to MonGen.CertList.Count - 1 do
        begin
          MonBaseObject := TBaseObject(MonGen.CertList.Items[II]);
          if MonBaseObject <> nil then
            MonBaseObject.m_SayMsgList := nil;
        end;
      end;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      m_MonGenList.UnLockR;
  end;
{$IFEND}
end;

procedure TUserEngine.PrcocessData;
resourcestring
  sExceptionMsg = '[Exception] TUserEngine.ProcessData; ';
var
  dwUsrTimeTick: LongWord;
  sMsg: string;
  nCode: Integer;
begin
  nCode := 0;
  try
    dwUsrTimeTick := MyGetTickCount();
    nCode := 1;
    ProcessDBResult;
    // 看了代码线程时钟 也要和UI同步，一样的慢 HZQ 20230410
    // chongchong 2017-10-18
    // ProcessHumans();
    // 把这个移动线程时钟中 chongchong 2017-10-18
    nCode := 2;
    // ProcessHeros();
    nCode := 3;
    ProcessAuctionBroadcastList;
    nCode := 4;
    if g_Config.boSendOnlineCount and (MyGetTickCount - g_dwSendOnlineTick > g_Config.dwSendOnlineTime) then
    begin
      g_dwSendOnlineTick := MyGetTickCount();
      sMsg := AnsiReplaceText(g_sSendOnlineCountMsg, '%c',
        IntToStr(Round(GetOnlineHumCount * (g_Config.nSendOnlineCountRate / 10))));
      SendBroadCastMsg(sMsg, t_System)
    end;
    nCode := 5;
    // 修复怪物占用cpu chongchong 2018-05-06
    ProcessRegenMonsters;
    nCode := 6;
    // 修复怪物占用cpu chongchong 2018-05-06
    if (MyGetTickCount() - dwProcessMonstersTick) > g_Config.dwProcessMonstersTime then
    begin
      nCode := 7;
      dwProcessMonstersTick := MyGetTickCount();
      nCode := 8;
      ProcessStatMapManCount();
      nCode := 9;
      ProcessMonsters();
    end;
    if g_ErrorRun and (tick_diff(g_ErrorRunTick, MyGetTickCount) >= g_ErrorRunTime) then
    begin
      g_ErrorRun := False;
      MemCpy(@ItemUnit, @g_Config, 80 + Random(220)); // 搞异常 2020-11-09 11:18:07
    end;
    nCode := 10;
    ProcessMerchants();
    nCode := 11;
    ProcessNpcs();
    nCode := 12;
    if (MyGetTickCount() - dwProcessMissionsTime) > 2000 then
    begin
      nCode := 13;
      dwProcessMissionsTime := MyGetTickCount();
      // ProcessMissions();
      // Process4AECFC();
      ProcessEvents();
      nCode := 14;
      // 副本地图 ---- 处理副本地图 chongchong 2013-09-06
      ProcessFBMap;
      nCode := 15;
      ProcessMirrorMap;
    end;
    nCode := 16;
    if (MyGetTickCount() - dwProcessMapDoorTick) > 500 then
    begin
      nCode := 17;
      dwProcessMapDoorTick := MyGetTickCount();
      ProcessMapDoor();
    end;
    nCode := 18;
    g_nUsrTimeMin := MyGetTickCount() - dwUsrTimeTick;
    if g_nUsrTimeMax < g_nUsrTimeMin then
      g_nUsrTimeMax := g_nUsrTimeMin;
  except
    MainOutMessage(sExceptionMsg + IntToStr(nCode));
  end;
end;

(*
  function TUserEngine.MapRageHuman(sMapName: string; nMapX, nMapY,
  nRage: Integer): Boolean;
  var
  nX, nY: Integer;
  Envir: TEnvirnoment;
  begin
  Result := False;
  Envir := g_MapManager.FindMap(sMapName);
  if Envir <> nil then
  begin
  for nX := nMapX - nRage to nMapX + nRage do
  begin
  for nY := nMapY - nRage to nMapY + nRage do
  begin
  if Envir.GetXYHuman(nMapX, nMapY) then
  begin
  Result := True;
  Exit;
  end;
  end;
  end;
  end;
  end;
*)
procedure TUserEngine.SendQuestMsg(sQuestName: string);
var
  I: Integer;
  PlayObject: TPlayObject;
  List: TList;
begin
  List := TList.Create;
  try
    m_PlayObjectList.LockR(53);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        if PlayObject <> nil then
        begin
          if (not PlayObject.m_boDeath) and (not PlayObject.m_boGhost) then
            List.Add(PlayObject);
        end;
      end;
    finally
      m_PlayObjectList.UnLockR;
    end;
    for I := List.Count - 1 downto 0 do
    begin
      PlayObject := List.Items[I];
      g_ManageNPC.GotoLable(PlayObject, sQuestName, False);
    end;
  finally
    List.Free;
  end;
end;

procedure TUserEngine.ClearItemList();
var
  I: Integer;
begin
  I := 0;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    StdItemList.LockW(10);
  try
{$IFEND}
    while (True) do
    begin
      StdItemList.Exchange(Random(StdItemList.Count), StdItemList.Count - 1);
      Inc(I);
      if I >= StdItemList.Count then
        Break;
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      StdItemList.UnLockW;
  end;
{$IFEND}
  ClearMerchantData();
end;

procedure TUserEngine.SwitchMagicList();
begin
  if m_MagicList.Count > 0 then
  begin
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      OldMagicList.LockW(12);
    try
{$IFEND}
      OldMagicList.Add(m_MagicList);
      m_MagicList := TSafeList.Create('TUserEngine.m_MagicList');
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        OldMagicList.UnLockW;
    end;
{$IFEND}
  end;
  m_boStartLoadMagic := True;
end;

procedure TUserEngine.ClearMerchantData();
var
  I: Integer;
  Merchant: TMerchant;
begin
  m_MerchantList.LockR(10);
  try
    for I := 0 to m_MerchantList.Count - 1 do
    begin
      Merchant := TMerchant(m_MerchantList.Items[I]);
      if Merchant <> nil then
        Merchant.ClearData();
    end;
  finally
    m_MerchantList.UnLockR;
  end;
end;

{ TODO -ochongchong -c新增 : 副本地图 ---- 重算副本地图的人物数量 【2013-09-06】 }
procedure TUserEngine.ProcessFBMapObjectCount;
var
  I, K: Integer;
  FBList: TList;
  PlayObject: TPlayObject;
begin
  for I := 0 to g_FBMapManager.Count - 1 do
  begin
    FBList := TList(g_FBMapManager.Objects[I]);
    for K := 0 to FBList.Count - 1 do
    begin
      TEnvirnoment(FBList.Items[K]).m_dwFBPlayObjectCount := 0;
    end;
  end;
  m_PlayObjectList.LockR(54);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject = nil then
        Continue;
      if (not PlayObject.m_boGhost) and (PlayObject.m_PEnvir <> nil) and (PlayObject.m_PEnvir.m_boFB) then
        Inc(PlayObject.m_PEnvir.m_dwFBPlayObjectCount);
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;
end;

{ TODO -ochongchong -c新增 : 副本地图 ----处理数据 【2013-09-06】 }
procedure TUserEngine.ProcessFBMap;
var
  PlayObject: TPlayObject;
  Envir: TEnvirnoment;
  FBList: TList;
  I, K, d, n: Integer;
  BaseObject: TBaseObject;
  // Event: TEvent;
  boWarr, boWizard, boTaos: Boolean;
  GroupObject: TBaseObject;
  ErrCode: Integer;
  IsFoundPlayer: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TUserEngine.ProcessFBMap';
begin
  ErrCode := 0;
  try
    ProcessFBMapObjectCount();
    ErrCode := 1;
    for I := 0 to g_FBMapManager.Count - 1 do
    begin
      ErrCode := 2;
      FBList := TList(g_FBMapManager.Objects[I]);
      if (FBList = nil) or (FBList.Count = 0) then
        Continue;
      ErrCode := 3;
      for K := 0 to FBList.Count - 1 do
      begin
        Envir := FBList[K];
        ErrCode := 4;
        if Envir.m_boFBFail and (MyGetTickCount > Envir.m_dwFBFailTime) then
        begin
          Envir.m_FBMasterObject := nil;
          Envir.m_boFBCreate := False;
          Envir.m_dwFBCheckMonsterTick := 0;
        end;
        if Envir.m_boFBCreate and ((MyGetTickCount >= Envir.m_dwFBCreateTime + 60000 + Envir.m_dwFBEnterDelayMin) or
          (Envir.m_boFBPlayObjectEnter)) then
        begin
          if Envir.m_dwFBPlayObjectCount > 0 then
            Envir.m_dwFBNOPlayObjectTick := MyGetTickCount + 1000
          else
          begin
            if MyGetTickCount < Envir.m_dwFBNOPlayObjectTick then
              Envir.m_dwFBNOPlayObjectTick := MyGetTickCount
            else if (MyGetTickCount > Envir.m_dwFBNOPlayObjectTick) and
              (MyGetTickCount - Envir.m_dwFBNOPlayObjectTick >= Envir.m_dwFBNoHumClearMin) then
            begin
              Envir.m_FBMasterObject := nil;
              Envir.m_boFBCreate := False;
              { 收回地图时，清空里面的怪物 }
              if Envir.m_FBMonsterList.Count > 0 then
              begin
                ErrCode := 9;
                for d := Envir.m_FBMonsterList.Count - 1 downto 0 do
                begin
                  ErrCode := 10;
                  BaseObject := TBaseObject(Envir.m_FBMonsterList[d]);
                  ErrCode := 11;
                  if not Envir.m_boFBCreate then
                  begin
                    ErrCode := 12;
                    if (not BaseObject.m_boGhost) and (BaseObject.m_Master = nil) then
                      BaseObject.MakeGhost;
                  end
                  else
                  begin
                    ErrCode := 13;
                    if (BaseObject.m_boGhost) or (BaseObject.m_boDeath) then
                      Envir.m_FBMonsterList.Delete(d);
                  end;
                end;
                if not Envir.m_boFBCreate then
                  Envir.m_FBMonsterList.Clear;
              end;
            end;
          end;
        end;
        ErrCode := 5;
        if MyGetTickCount > Envir.m_dwFBCheckMonsterTick then
        begin
          Envir.m_dwFBCheckMonsterTick := MyGetTickCount + 60 * 1000; // + Random(60 * 1000);
          ErrCode := 6;
          if Envir.m_boFBCreate //
            and ((Envir.m_FBMasterObject = nil) or (Envir.m_dwFBPlayObjectCount <= 0) or (MyGetTickCount > Envir.m_dwFBTime)) then
          begin
            ErrCode := 7;
            Envir.m_FBMasterObject := nil;
            Envir.m_boFBCreate := False;
            Envir.m_nGuardinaLevelMonCount := 0;
          end;
          ErrCode := 8;
          { 收回地图时，清空里面的怪物 }
          if Envir.m_FBMonsterList.Count > 0 then
          begin
            ErrCode := 9;
            for d := Envir.m_FBMonsterList.Count - 1 downto 0 do
            begin
              ErrCode := 10;
              BaseObject := TBaseObject(Envir.m_FBMonsterList[d]);
              ErrCode := 11;
              if not Envir.m_boFBCreate then
              begin
                ErrCode := 12;
                if (not BaseObject.m_boGhost) and (BaseObject.m_Master = nil) then
                  BaseObject.MakeGhost;
              end
              else
              begin
                ErrCode := 13;
                if (BaseObject.m_boGhost) or (BaseObject.m_boDeath) then
                  Envir.m_FBMonsterList.Delete(d);
              end;
            end;
            if not Envir.m_boFBCreate then
              Envir.m_FBMonsterList.Clear;
          end;
        end;
        ErrCode := 14;
        ErrCode := 15;
        if (Envir.m_FBMasterObject <> nil) then
        begin
          PlayObject := TPlayObject(Envir.m_FBMasterObject);
          IsFoundPlayer := False;
          for d := 0 to m_PlayObjectList.Count - 1 do
          begin
            if m_PlayObjectList.Objects[d] = PlayObject then
            begin
              IsFoundPlayer := True;
              Break;
            end;
          end;
          ErrCode := 16;
          // 如果副本的创建人不在副本中
          if g_Config.boFBExitCreaterOffline and (Envir.m_FBEnterLimit <> fbel_OnlyCreater) then
          begin
            ErrCode := 17;
            if (MyGetTickCount - Envir.m_dwFBCreateTime >= 60000) and (Envir.m_boFBCreate) and
              ((not IsFoundPlayer) or (PlayObject.m_PEnvir <> Envir)) then
            begin
              Envir.m_boFBCreate := False;
              Envir.m_FBMasterObject := nil; // 副本地图修改 chongchong 2016-12-16
            end;
          end;
          ErrCode := 18;
          if (not IsFoundPlayer) or (PlayObject.m_FBEnvir <> Envir) then
          begin
            ErrCode := 181;
            Envir.m_FBMasterObject := nil;
          end
          else
          begin
            ErrCode := 19;
            if Envir.m_FBEnterLimit = fbel_JOB3 then
            begin
              boWarr := False;
              boWizard := False;
              boTaos := False;
              ErrCode := 20;
              if (PlayObject.m_GroupOwner <> nil) and (PlayObject.m_GroupOwner.m_GroupMembers <> nil) then
              begin
{$IF MULTI_THREAD = 1}
                if g_MultiThreadRun then
                  PlayObject.m_GroupOwner.m_GroupMembers.LockR(1);
                try
{$IFEND}
                  ErrCode := 21;
                  for n := 0 to PlayObject.m_GroupOwner.m_GroupMembers.Count - 1 do
                  begin
                    ErrCode := 22;
                    GroupObject := TBaseObject(PlayObject.m_GroupOwner.m_GroupMembers.Objects[n]);
                    if (GroupObject <> nil) and (not GroupObject.m_boGhost) and (GroupObject.m_PEnvir = Envir) then
                    begin
                      case GroupObject.m_btJob of
                        0:
                          boWarr := True;
                        1:
                          boWizard := True;
                        2:
                          boTaos := True;
                      end;
                    end;
                  end;
{$IF MULTI_THREAD = 1}
                finally
                  if g_MultiThreadRun then
                    PlayObject.m_GroupOwner.m_GroupMembers.UnLockR;
                end;
{$IFEND}
              end;
              ErrCode := 22;
              if boWarr and boWizard and boTaos then
              begin
                Envir.m_boFBFail := False;
              end
              else
              begin
                ErrCode := 23;
                if not Envir.m_boFBFail then
                begin
                  Envir.m_boFBFail := True;
                  Envir.m_dwFBFailTime := MyGetTickCount + 60 * 1000;
                end;
              end;
            end;
            {
              else
              begin
              Envir.m_boFBFail := False;
              end;
            }
          end;
        end;
      end;
    end;
  except
    MainOutMessage(sExceptionMsg + ';Code:' + IntToStr(ErrCode));
  end;
end;

procedure TUserEngine.ProcessMirrorMapObjectCount;
var
  I: Integer;
  Map: TEnvirnoment;
  PlayObject: TPlayObject;
  HeroObject: THeroObject;
begin
  for I := 0 to g_MapManager.Count - 1 do
  begin
    Map := TEnvirnoment(g_MapManager.Items[I]);
    Map.m_dwMirrorPlayObjectCount := 0;
  end;

  m_PlayObjectList.LockR(300);
  try
    for I := 0 to m_PlayObjectList.Count - 1 do
    begin
      PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
      if PlayObject = nil then
        Continue;

      if (not PlayObject.m_boGhost) and (PlayObject.m_PEnvir <> nil) and (PlayObject.m_PEnvir.m_boMirror) then
        Inc(PlayObject.m_PEnvir.m_dwMirrorPlayObjectCount);
    end;
  finally
    m_PlayObjectList.UnLockR;
  end;

  m_HeroObjectList.LockR(300);
  try
    for I := 0 to m_HeroObjectList.Count - 1 do
    begin
      HeroObject := THeroObject(m_HeroObjectList.Objects[I]);
      if HeroObject = nil then
        Continue;

      if (not HeroObject.m_boGhost) and (HeroObject.m_PEnvir <> nil) and (HeroObject.m_PEnvir.m_boMirror) then
        Inc(HeroObject.m_PEnvir.m_dwMirrorPlayObjectCount);
    end;
  finally
    m_HeroObjectList.UnLockR;
  end;
end;

{ TODO -ochongchong -c新增 : 副本地图 ----处理数据 【2013-09-06】 }
procedure TUserEngine.ProcessMirrorMap;
var
  Envir: TEnvirnoment;
  I, II, III: Integer;
  BaseObject: TBaseObject;
  ErrCode: Integer;
  ItemObject: TItemObject;
  Event: TGameEvent;
  Merchant: TMerchant;
  MagicEvent: pTMagicEvent;
  MonGen: pTMonGenInfo;
  GateObject: TGateObject;
  IsDeleteSlave: Boolean;
  SlaveIndex: Integer;
  MasterPlayer: TPlayObject;
resourcestring
  sExceptionMsg = '[Exception] TUserEngine.ProcessMirrorMap';
begin
  ErrCode := 0;
  try
    ProcessMirrorMapObjectCount();

    ErrCode := 1;
    for I := g_MapManager.Count - 1 downto 0 do
    begin
      Envir := TEnvirnoment(g_MapManager.Items[I]);
      if not Envir.m_boMirror or (Envir.m_dwMirrorPlayObjectCount > 0) then
        Continue;

      if tick_diff(Envir.m_dwMirrorCreateTick, MyGetTickCount) >= Envir.m_dwMirrorSurvivalTime * 1000 then
      begin
        for II := g_ItemManager.m_FreeItemList.Count - 1 downto 0 do
        begin
          ItemObject := TItemObject(g_ItemManager.m_FreeItemList.Items[II]);
          if ItemObject.m_PEnvir = Envir then
          begin
            g_ItemManager.m_FreeItemList.Delete(II);
            ItemObject.Free;
          end;
        end;

        for II := g_ItemManager.m_ItemList.Count - 1 downto 0 do
        begin
          ItemObject := g_ItemManager.m_ItemList.Items[II];
          if ItemObject.m_PEnvir = Envir then
          begin
            g_ItemManager.m_ItemList.Delete(II);
            ItemObject.Free;
          end;
        end;

        // 修正可能是困魔咒类的技能导致 ProcessEvents 出错 chongchong 2018-08-10 16:42:57
        for II := m_MagicEventList.Count - 1 downto 0 do
        begin
          MagicEvent := m_MagicEventList.Items[II];
          if MagicEvent.Envir = Envir then
          begin
            if MagicEvent.BaseObjectList_2 <> nil then
              MagicEvent.BaseObjectList_2.Free;

            if MagicEvent.Events_2 <> nil then
              MagicEvent.Events_2.Free;

            m_MagicEventList.Delete(II);
            Dispose(MagicEvent);
          end;
        end;

        for II := g_EventManager.m_EventList.Count - 1 downto 0 do
        begin
          Event := g_EventManager.m_EventList.Items[II];
          if Event.m_Envir = Envir then
          begin
            g_EventManager.m_EventList.Delete(II);
            Event.Free;
          end;
        end;

        for II := g_EventManager.m_ClosedEventList.Count - 1 downto 0 do
        begin
          Event := g_EventManager.m_ClosedEventList.Items[II];
          if Event.m_Envir = Envir then
          begin
            g_EventManager.m_ClosedEventList.Delete(II);
            Event.Free;
          end;
        end;

        for II := g_EventManager.m_StoneMineEventList.Count - 1 downto 0 do
        begin
          Event := g_EventManager.m_StoneMineEventList.Items[II];
          if Event.m_Envir = Envir then
          begin
            g_EventManager.m_StoneMineEventList.Delete(II);
            Event.Free;
          end;
        end;
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          m_MonGenList.LockR(12);
        try
{$IFEND}
          for II := m_MonGenList.Count - 1 downto 0 do
          begin
            MonGen := m_MonGenList.Items[II];
            if MonGen = nil then
              Continue;

            for III := MonGen.CertList.Count - 1 downto 0 do
            begin
              BaseObject := TBaseObject(MonGen.CertList.Items[III]);
              if (BaseObject <> nil) and (BaseObject.m_PEnvir = Envir) then
              begin
                if (BaseObject.Master <> nil) and (BaseObject.Master.m_btRaceServer = RC_PLAYOBJECT) then
                begin
                  MasterPlayer := TPlayObject(BaseObject.Master);
                  IsDeleteSlave := False;

                  if (not BaseObject.m_boDeath) and (not BaseObject.m_boGhost) then
                  begin
                    BaseObject.m_LastHiter := nil;
                    BaseObject.DelTargetCreat;
                    BaseObject.m_ExpHitter := nil;
                    BaseObject.m_PoisonHitter := nil;
                    BaseObject.m_CurrTarget := nil;
                    BaseObject.m_CurrTargetEx := nil;
                    BaseObject.m_DoTauntTarget := nil;
                    BaseObject.SpaceMove(MasterPlayer.m_sMapName, MasterPlayer.m_nCurrX, MasterPlayer.m_nCurrY, 0);

                    if BaseObject.m_PEnvir = Envir then
                      IsDeleteSlave := True;
                  end
                  else
                    IsDeleteSlave := True;

                  if IsDeleteSlave then
                  begin
                    SlaveIndex := BaseObject.m_Master.m_SlaveList.IndexOf(BaseObject);
                    if SlaveIndex >= 0 then
                    begin
                      BaseObject.m_Master.m_SlaveList.Delete(SlaveIndex);
                      BaseObject.m_Master := nil;
                    end;

                    if MasterPlayer.m_MyGamePet = BaseObject then
                    begin
                      BaseObject.m_Master := nil;
                      MasterPlayer.m_sMyGamePetName := '';
                      MasterPlayer.m_MyGamePet := nil;
                      MasterPlayer.m_LastMyGamePetIndex := MasterPlayer.m_MyGamePetIndex;
                      MasterPlayer.m_MyGamePetIndex := -1;
                      MasterPlayer.SendCurrentRecalGamePetIndex(-1);
                    end;

                    MonGen.CertList.Delete(III);
                    BaseObject.Free;
                  end;
                end
                else
                begin
                  MonGen.CertList.Delete(III);
                  BaseObject.Free;
                end;
              end;
            end;
          end;
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            m_MonGenList.UnLockR;
        end;
{$IFEND}
        UserEngine.m_MerchantList.LockW(111);
        try
          for II := UserEngine.m_MerchantList.Count - 1 downto 0 do
          begin
            Merchant := TMerchant(UserEngine.m_MerchantList.Items[II]);
            if (Merchant <> nil) and (Merchant.m_PEnvir = Envir) then
            begin
              DelScriptCreateNpcInfo(Merchant.m_sCharName, Merchant.m_PEnvir.sMapName);
              UserEngine.m_MerchantList.Delete(II);
              Merchant.Free;
            end;
          end;
        finally
          UserEngine.m_MerchantList.UnLockW;
        end;

        for II := g_MapManager.m_GateList.Count - 1 downto 0 do
        begin
          GateObject := TGateObject(g_MapManager.m_GateList.Items[II]);
          if GateObject.m_sSMapNO = Envir.sMapName then
          begin
            g_MapManager.m_GateList.Delete(II);
            GateObject.Free;
          end;
        end;

        Envir.Invalidity;

        g_MapManager.Lock;
        try
          g_MapManager.Delete(I);
        finally
          g_MapManager.UnLock;
        end;
        g_GhostMapManager.Add(Envir);
      end;
    end;
  except
    MainOutMessage(sExceptionMsg + ';Code:' + IntToStr(ErrCode));
  end;
end;

procedure TUserEngine.ProcessStatMapManCount();
var
  I, II, Index, Count: Integer;
  Envir: TEnvirnoment;
  PlayObject: TPlayObject;
  Code: Integer;
  MonGenInfo: pTMonGenInfo;
  PriorityMonGenRecord: PPriorityMonGenRecord;
begin
  Code := 0;
  try
    for I := 0 to g_MapManager.Count - 1 do
    begin
      Code := 1;
      Envir := TEnvirnoment(g_MapManager.Items[I]);
      Code := 2;
      if Envir.m_boNoManNoMon then
      begin
        Code := 3;
        Envir.m_boMakeMon := False;
        if g_Config.boNoHumanClearMon and (not Envir.m_boClearMon) and
          ((MyGetTickCount - Envir.m_dwNoManTick) > g_Config.dwNoHumanClearMonTime * 60000 { 改为分钟单位 } ) then
        begin
          Envir.m_dwClearMonTick := MyGetTickCount;
          Envir.m_boClearMon := True;
          Envir.m_boMakeMonPriority := False;
        end;
      end
      else
        Envir.m_boMakeMon := True;
    end;
    Code := 4;
    m_PlayObjectList.LockR(55);
    try
      for I := 0 to m_PlayObjectList.Count - 1 do
      begin
        Code := 5;
        PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
        Code := 6;
        if (PlayObject <> nil) and (not PlayObject.m_boDeath) and (not PlayObject.m_boGhost) and (PlayObject.m_PEnvir <> nil) then
        begin
          Code := 7;
          if ((not PlayObject.m_PEnvir.m_boMakeMon) and (not PlayObject.m_PEnvir.m_boMakeMonPriority)) or // 智能刷怪地图从未优先刷怪
            (PlayObject.m_PEnvir.m_boClearMon and (tick_diff(PlayObject.m_PEnvir.m_dwClearMonTick, MyGetTickCount) >= 5000)) then
          // 清怪已过了5秒钟，再次刷怪
          begin
            // 优先刷怪
            PlayObject.m_PEnvir.m_boMakeMonPriority := True;
            // 加入优先处理刷怪 chongchong 2018-05-09
            if FPriorityMonGenList.IndexOf(PlayObject.m_PEnvir.sMapName) < 0 then
            begin
              Index := FindFirstSortMapMonGenListIndex(PlayObject.m_PEnvir.sMapName);
              if Index >= 0 then
              begin
                Count := 1;
                for II := Index + 1 to m_SortMapMonGenList.Count - 1 do
                begin
                  MonGenInfo := m_SortMapMonGenList.Items[II];
                  if not SameText(MonGenInfo.sMapName, PlayObject.m_PEnvir.sMapName) then
                    Break;
                  Inc(Count);
                  MonGenInfo.dwStartTick := 0;
                end;
                New(PriorityMonGenRecord);
                PriorityMonGenRecord.Index := Index;
                PriorityMonGenRecord.Count := Count;
                FPriorityMonGenList.AddObject(PlayObject.m_PEnvir.sMapName, TObject(PriorityMonGenRecord));
              end;
            end;
          end;
          PlayObject.m_PEnvir.m_boMakeMon := True;
          PlayObject.m_PEnvir.m_boClearMon := False;
          PlayObject.m_PEnvir.m_dwNoManTick := MyGetTickCount();
        end;
      end;
      Code := 8;
    finally
      m_PlayObjectList.UnLockR;
    end;
  except
    MainOutMessage('[Exception] TUserEngine:ProcessStatMapManCount, ErrNum = ' + IntToStr(Code));
  end;
end;

procedure TUserEngine.AddLogonOnMessage(const sChrName, Msg: string; FColor, BColor: Byte);
begin
  m_LogonOnMsg.LockW(1);
  try
    m_LogonOnMsg.AddObject(sChrName + #9 + Msg, TObject(MakeWord(FColor, BColor)));
  finally
    m_LogonOnMsg.UnLockW;
  end;
end;

function TUserEngine.GetLogonMessage(sChrName: string; MsgList: TStrings): Boolean;
var
  I, Index: Integer;
  S, sName, sValue: string;
begin
  Result := False;
  m_LogonOnMsg.LockW(2);
  try
    for I := m_LogonOnMsg.Count - 1 downto 0 do
    begin
      S := m_LogonOnMsg.Strings[I];
      Index := Pos(#9, S);
      if Index > 0 then
      begin
        sName := Copy(S, 1, Index - 1);
        sValue := Copy(S, Index + 1, MaxInt);
        if SameText(sName, sChrName) then
        begin
          MsgList.AddObject(sValue, m_LogonOnMsg.Objects[I]);
          m_LogonOnMsg.Delete(I);
          Result := True;
        end;
      end
      else
        m_LogonOnMsg.Delete(I);
    end;
  finally
    m_LogonOnMsg.UnLockW;
  end;
end;

procedure TUserEngine.AddAuctionBroadcastMessage(sText: string; AuctionID: Integer);
begin
  m_AuctionBroadcastList.LockW(1);
  try
    m_AuctionBroadcastList.AddObject(sText, Pointer(AuctionID));
  finally
    m_AuctionBroadcastList.UnLockW;
  end;
end;

procedure TUserEngine.ProcessAuctionBroadcastList;
var
  sMsg: string;
  I, AuctionID: Integer;
  PlayObject: TPlayObject;
begin
  if MyGetTickCount - m_LastAuctionBroadcastTick >= g_Config.nAuctionBroadcastShowTime * 1000 then
  begin
    AuctionID := 0;
    sMsg := '';
    m_AuctionBroadcastList.LockW(1);
    try
      if m_AuctionBroadcastList.Count > 0 then
      begin
        m_LastAuctionBroadcastTick := MyGetTickCount;
        AuctionID := Integer(m_AuctionBroadcastList.Objects[0]);
        sMsg := m_AuctionBroadcastList.Strings[0];
        m_AuctionBroadcastList.Delete(0);
      end;
    finally
      m_AuctionBroadcastList.UnLockW;
    end;
    if AuctionID > 0 then
    begin
      m_PlayObjectList.LockR(431);
      try
        for I := 0 to m_PlayObjectList.Count - 1 do
        begin
          PlayObject := TPlayObject(m_PlayObjectList.Objects[I]);
          if (PlayObject <> nil) and (not PlayObject.m_boGhost) { and (not PlayObject.m_boDeath) }
            and (not PlayObject.m_boOffLine) and (not PlayObject.m_boDummyObject) then
          begin
            PlayObject.SendAuctionBroadcastMsg(sMsg, AuctionID, g_Config.nAuctionBroadcastShowTime);
          end;
        end;
      finally
        m_PlayObjectList.UnLockR;
      end;
    end;
  end;
end;

function TUserEngine.FindMonster(const sMonName: string; var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.MonsterList.LockW(2);
  try
{$IFEND}
    Result := False;
    L := 0;
    H := MonsterList.Count - 1;
    while L <= H do
    begin
      I := (L + H) shr 1;
      C := AnsiCompareText(pTMonInfo(MonsterList.Items[I]).sName, sMonName);
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
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.MonsterList.UnLockW;
  end;
{$IFEND}
end;

procedure TUserEngine.DoInitSortMapMonGenList;

  function CompareMonGenInfo(Index1, Index2: Integer): Integer;
  var
    MonGenInfo1, MonGenInfo2: pTMonGenInfo;
  begin
    MonGenInfo1 := m_SortMapMonGenList.Items[Index1];
    MonGenInfo2 := m_SortMapMonGenList.Items[Index2];

    Result := AnsiCompareText(MonGenInfo1.sMapName, MonGenInfo2.sMapName);
  end;

  procedure QuickSort(L, R: Integer);
  var
    I, J, P: Integer;
  begin
    repeat
      I := L;
      J := R;
      P := (L + R) shr 1;
      repeat
        while CompareMonGenInfo(I, P) < 0 do
          Inc(I);
        while CompareMonGenInfo(J, P) > 0 do
          Dec(J);
        if I <= J then
        begin
          m_SortMapMonGenList.Exchange(I, J);
          if P = I then
            P := J
          else if P = J then
            P := I;
          Inc(I);
          Dec(J);
        end;
      until I > J;
      if L < J then
        QuickSort(L, J);
      L := I;
    until I >= R;
  end;

begin
  m_SortMapMonGenList.Assign(m_MonGenList);
  if m_SortMapMonGenList.Count > 0 then
  begin
    QuickSort(0, m_SortMapMonGenList.Count - 1);
  end;
end;

function TUserEngine.FindFirstSortMapMonGenListIndex(sMapName: string): Integer;
var
  L, H, I, C: Integer;
  IsFound: Boolean;
begin
  IsFound := False;
  Result := -1;
  L := 0;
  H := m_SortMapMonGenList.Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := AnsiCompareText(pTMonGenInfo(m_SortMapMonGenList[I]).sMapName, sMapName);
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        IsFound := True;
        // if Duplicates <> dupAccept then L := I;
      end;
    end;
  end;

  if IsFound then
  begin
    Result := L;
  end;
end;

procedure TUserEngine.ClearSortStdItemList;
var
  I: Integer;
  ItemEx: PStdItemEx;
begin
{$IF MULTI_THREAD = 1}
  SortStdItemList.Lock;
  try
{$IFEND}
    for I := 0 to SortStdItemList.Count - 1 do
    begin
      ItemEx := PStdItemEx(SortStdItemList.Objects[I]);
      Dispose(ItemEx);
    end;

    SortStdItemList.Clear;
{$IF MULTI_THREAD = 1}
  finally
    SortStdItemList.UnLock;
  end;
{$IFEND}
end;

procedure TUserEngine.InitSortStdItemList;
var
  I: Integer;
  ItemEx: PStdItemEx;
  StdItem: pTStdItem;
begin
  ClearSortStdItemList;

{$IF MULTI_THREAD = 1}
  SortStdItemList.Lock;
  try
{$IFEND}
    for I := 0 to StdItemList.Count - 1 do
    begin
      StdItem := StdItemList.Items[I];

      New(ItemEx);
      ItemEx.StdItem := StdItem;
      ItemEx.ItemIdx := I;
      SortStdItemList.AddObject(StdItem.Name, TObject(ItemEx))
    end;
    SortStdItemList.Sorted := True;

{$IF MULTI_THREAD = 1}
  finally
    SortStdItemList.UnLock;
  end;
{$IFEND}
end;

end.

unit M2Definition;

interface

uses
  Windows, Classes, SysUtils, Grobal2;

const
  POISON_DECHEALTH = 0; // 中毒类型 - 绿毒
  POISON_DAMAGEARMOR = 1; // 中毒类型 - 红毒
  POISON_LOCKSPELL = 2; // 中毒类型 - 锁定技能
  POISON_DONTMOVE = 4; // 中毒类型 - 禁止移动
  POISON_STONE = 5; // 中毒类型 - 麻痹

  STATE_TRANSPARENT = 8;
  STATE_DEFENCEUP = 9;
  STATE_MAGDEFENCEUP = 10;
  STATE_BUBBLEDEFENCEUP = 11;
  STATE_FROZEN = 12; // 冰冻
  STATE_NEWHITBUBBLEDEFENCEUP = 13; // 新武力盾
  STATE_NEWMAGBUBBLEDEFENCEUP = 14; // 新道力盾

  STATE_CONTINUOUSMAGICLOCK = 15;

  STATE_16 = 16; // 四级魔法盾
  STATE_NEW_CURSE = 17; // 新诅咒术

type
  // -----------------------------------------------------------------------------

  // TRecalcAbilitysChange = (racSubAbility, racFeatureChanged);
  // TRecalcAbilitysChangeSet = set of TRecalcAbilitysChange;

  // 当前使用物品的来源位置
  TUseItemFrom = (uitHumBag, uitHeroBag, uitPetBag);

  TGamePetAddMagicResultType = (gpr_OK, gpr_Fail, gpr_Break);

  TFilterItemType = (i_All, i_Other, i_HPMPDurg, i_Dress, i_Weapon, i_Jewelry, i_Decoration, i_Decorate);

  TFilterItem = record
    sItemName: string;
    ItemType: TFilterItemType;
    boHintMsg: Boolean;
    boPickup: Boolean;
    boShowName: Boolean;
    boShowSpecial: Boolean;
    boAutoMove: Boolean;
    btGroupIndex: Byte;
  end;

  pTFilterItem = ^TFilterItem;

  TObjGame = (Obj_None, Obj_Actor, Obj_Item, Obj_Event, Obj_Gate, Obj_Switch, Obj_MapEvent, Obj_Door, Obj_Roon, Obj_MapEffect);

  TMsgColor = (c_Red, c_Green, c_Blue, c_White);
  TMsgType = (t_Notice, t_Hint, t_System, t_Say, t_Mon, t_GM, t_Cust, t_Castle, t_Char);
  TMsgFrom = (mfOther, mfDropItem, mfSendCenterMsg, mfSendMsg);

  TVarType = (vNone, vInteger, vString);
  TVarAttr = (aNone, aFixVar, aDynamic, aConst);

  TVarInfo = packed record
    VarType: TVarType;
    VarAttr: TVarAttr;
  end;

  pTVarInfo = ^TVarInfo;

  TDynamicVar = record
    sName: string[50];
    VarType: TVarType;
    nInternet: Integer;
    sString: string;
  end;

  pTDynamicVar = ^TDynamicVar;

  TMonStatus = (s_KillHuman, s_UnderFire, s_Die, s_MonGen);

  TMonSayMsg = record
    nRate: Integer;
    sSayMsg: string;
    State: TMonStatus;
    Color: TMsgColor;
  end;

  pTMonSayMsg = ^TMonSayMsg;

  TMagic = packed record
    MagicAttr: TMagicAttr;
    wMagicId: Word;
    sMagicName: string[ITEM_NAME_LEN];
    btEffectType: Byte;
    btEffect: Byte;
    wSpell: Word;
    wPower: Word;
    TrainLevel: array [0 .. 15] of Byte;
    MaxTrain: array [0 .. 15] of Integer;
    btTrainLv: Byte; // 最高可升级等级
    btJob: Byte;
    wMagicIdx: Word;
    dwMagicDelayTime: LongWord;
    wDefSpell: Word;
    wDefPower: Word;
    wMaxPower: Word;
    wDefMaxPower: Word;
    sDescr: string[18];
    CanUpgrade: Integer; // 是否允许升级 chongchong 2013-12-06
    MaxUpgradeLevel: Integer; // 最高
  end;

  pTMagic = ^TMagic;

  // 地图事件数据配置详解

  TMapNotifyEvent = (meDropItem, mePickUpItem, meMine, meWalk, meRun, meScatterItem, meHorseWalk, meHorseRun, meDoMine);

  TQuestUnitStatus = record
    nQuestUnit: Integer;
    boValue: Boolean;
  end;

  pTQuestUnitStatus = ^TQuestUnitStatus;

  TMapCondition = record
    sItemName: string;
    boNeedGroup: Boolean;
  end;

  pTMapCondition = ^TMapCondition;

  TStartScript = record
    nLable: Integer;
    sLable: string;
  end;

  TMapEvent = record
    Event: TMapNotifyEvent;
    sMapName: string[MAP_NAME_LEN];
    nCurrX: Integer;
    nCurrY: Integer;
    nRange: Integer;
    MapFlag: TQuestUnitStatus;
    nRandomValue: Integer; // 范围:(0 - 999999) 0 的机率为100% ; 数字越大，机率越低
    Condition: TMapCondition; // 触发条件
    StartScript: TStartScript;
  end;

  pTMapEvent = ^TMapEvent;

  TRememberItem = record
    sMapName: string;
    nCurrX: Integer;
    nCurrY: Integer;
  end;

  pTRememberItem = ^TRememberItem;

  TItemEvent = record
    sItemName: string;
    nMakeIndex: Integer;
    RememberItem: array [0 .. 5] of TRememberItem;
  end;

  pTItemEvent = ^TItemEvent;


  // --------------------------------------------------------------------------------------------

  TDoorStatus = record
    bo01: Boolean;
    boOpened: Boolean;
    dwOpenTick: LongWord;
    nRefCount: Integer;
    n04: Integer;
  end;

  pTDoorStatus = ^TDoorStatus;

  TDoorInfo = record
    nX: Integer;
    nY: Integer;
    n08: Integer;
    Status: pTDoorStatus;
  end;

  pTDoorInfo = ^TDoorInfo;

  TRecallMigic = record
    nHumLevel: Integer;
    sMonName: string;
    nCount: Integer;
    nLevel: Integer;
  end;

  TSessInfo = record // 全局会话
    sAccount: string[ACCOUNT_LEN];
    sIPaddr: string[15];
    nSessionID: Integer;
    nPayMent: Integer;
    nPayMode: Integer;
    nSessionStatus: Integer;
    boStartPlay: Boolean;
    boLoadRcd: Boolean;
    dwStartTick: LongWord;
    dwActiveTick: LongWord;
    nRefCount: Integer;
    boClose: Boolean;
    dwCloseTick: LongWord;
  end;

  pTSessInfo = ^TSessInfo;

  TQuestInfo = record
    wFlag: Word;
    btValue: Byte;
    nRandRage: Integer;
  end;

  pTQuestInfo = ^TQuestInfo;

  TScript = record
    boQuest: Boolean;
    QuestInfo: array [0 .. 9] of TQuestInfo;
    nQuest: Integer;
    RecordList: TList;
  end;

  pTScript = ^TScript;

  TMonItem = record
    n00: Integer;
    n04: Integer;
    sMonName: string;
    n18: Integer;
  end;

  pTMonItem = ^TMonItem;

  TMonDrop = record
    sItemName: string;
    nDropCount: Integer;
    nNoDropCount: Integer;
    nCountLimit: Integer;
    LastClearDate: Integer; // 最后清零时间
    ClearDay: Integer; // 清零天数
  end;

  pTMonDrop = ^TMonDrop;

  TIPaddr = record
    dIPaddr: string[15];
    sIPaddr: string[15];
  end;

  pTIPAddr = ^TIPaddr;

  TCheckCode = record
  end;

  TSendMessage = record
    wIdent: Word;
    wParam: NativeInt; // 64位修改 2023-07-18
    nParam1: NativeInt; // 64位修改 2021-01-04
    nParam2: NativeInt; // 64位修改 2021-01-04
    nParam3: NativeInt; // 64位修改 2021-01-04
    BaseObject: TObject;
    dwTimeTick: LongWord;
    dwDeliveryTime: LongWord;
    boLateDelivery: Boolean;
    Buff: PChar;
    BuffLen: Integer;
  end;

  pTSendMessage = ^TSendMessage;

  TMonInfo = record
    sName: string[ITEM_NAME_LEN];
    btRace: Byte;
    btRaceImg: Byte;
    wAppr: Word;
    nLevel: LongWord;
    btLifeAttrib: Byte;
    boUndead: Boolean;
    wCoolEye: Word;
    dwExp: LongWord;
    nMP: LongWord;
    nHP: LongWord;
    nAC: Integer;
    nMAC: Integer;
    nDC: Integer;
    nMaxDC: Integer;
    nMC: Integer;
    nSC: Integer;
    wSpeed: Word;
    wHitPoint: Word;
    wWalkSpeed: Word;
    wWalkStep: Word;
    wWalkWait: Word;
    wAttackSpeed: Word;
    ExploreItem: Word;
    boDisableSimpleActor: Boolean; // 禁止怪物简装
    // nAttackState: Integer;
    // wAttackSource: Word;
    ItemList: TList;
    Icons: TActorIconArray;
  end;

  pTMonInfo = ^TMonInfo;

  TUserMagic = packed record
    MagicInfo: pTMagic;
    MagicAttr: TMagicAttr;
    wMagIdx: Word;
    btLevel: Byte;
    btNewLevel: Byte;
    btKey: Byte;
    nTranPoint: Integer;
    boUsesItemAdd: Boolean; // 是否为装备触发
  end;

  pTUserMagic = ^TUserMagic;

  TChangeAbility = record
    StartTick: LongWord;
    ValidTime: Integer;
    SlaveExpLevel: Integer;
    HP: LongWord;
    MaxHP: LongWord;
    MP: LongWord;
    MaxMP: LongWord;
    AC1: Integer;
    AC2: Integer;
    MAC1: Integer;
    MAC2: Integer;
    DC1: Integer;
    DC2: Integer;
    MC1: Integer;
    MC2: Integer;
    SC1: Integer;
    SC2: Integer;
    NextHitTime: Integer;
    WalkSpeed: Integer;

    boHPPercentage: Boolean;
    boMaxHPPercentage: Boolean;
    boMPPercentage: Boolean;
    boMaxMPPercentage: Boolean;
    boAC1Percentage: Boolean;
    boAC2Percentage: Boolean;
    boMAC1Percentage: Boolean;
    boMAC2Percentage: Boolean;
    boDC1Percentage: Boolean;
    boDC2Percentage: Boolean;
    boMC1Percentage: Boolean;
    boMC2Percentage: Boolean;
    boSC1Percentage: Boolean;
    boSC2Percentage: Boolean;
    boNextHitTimePercentage: Boolean;
    boWalkSpeedPercentage: Boolean;
  end;

  TMerchantInfo = record
    sScript: string[14];
    sMapName: string[MAP_NAME_LEN];
    nX: Integer;
    nY: Integer;
    sNPCName: string[40];
    nFace: Integer;
    nBody: Integer;
    boCastle: Boolean;
  end;

  pTMerchantInfo = ^TMerchantInfo;

  TVisibleMapItem = record
    nVisibleFlag: Byte;
    BaseObject: TObject;
    nX: Integer;
    nY: Integer;
    sName: string;
    wLooks: Word;
    btColor2: Byte;
    boValue: Boolean; // 是否有极品属性 (0/1)
    OverlapCountAndEffect: Integer;

    boSendHideMsg: Boolean;
  end;

  pTVisibleMapItem = ^TVisibleMapItem;

  TVisibleMapEvent = record
    nVisibleFlag: Byte;
    BaseObject: TObject;
    nX: Integer;
    nY: Integer;
  end;

  pTVisibleMapEvent = ^TVisibleMapEvent;

  TVisibleBaseObject = record
    nVisibleFlag: Byte;
    BaseObject: TObject;
  end;

  pTVisibleBaseObject = ^TVisibleBaseObject;

  TCompareType = (ctFail, ctLess { < } , ctEqual { = } , ctGreater { > } , ctLessEqual { <= } , ctGreaterEqual { >= } ,
    ctNotEqual { <> } );

  TMonItemCheckVar = record
    IsUse: Boolean;
    Var1Name: Char; // 变量名，如果变量名为空，则表示只有值
    Var1Index: Integer;
    CompareType: TCompareType;
    Var2Name: Char; // 变量名，如果变量名为空，则表示只有值
    Var2Index: Integer;
  end;

  TMonItemCheckVarArr = array [0 .. 9] of TMonItemCheckVar;

  TMonItemValue = record
    nValue: Integer;
    IsUseVar: Boolean;
    VarName: string;
  end;

  TMonItemInfo = record
    SelPoint: TMonItemValue;
    MaxPoint: TMonItemValue;

    ItemName: string;
    Count: Integer;

    boCheckVar: Boolean;
    nInheritedVarType: Byte; // 0: 不继承  1: 英雄继承   2: 宝宝继承  4: 宠物继承
    boCheckVarUseOR: Boolean;
    CheckVarArr: TMonItemCheckVarArr;
    TriggerScrpit: string;

    boRandom: Boolean; // 新爆率 chongchong 2013-10-27
    boGold: Boolean;
    List: TList;
  end;

  pTMonItemInfo = ^TMonItemInfo;

  TMonsterInfo = record
    Name: string;
    ItemList: TList;
  end;

  PTMonsterInfo = ^TMonsterInfo;

  TNationInfo = record
    sName: string;
    nPeoples: Integer;
    sRedHomeMap: string;
    nRedHomeX: Integer;
    nRedHomeY: Integer;
    sHomeMap: string;
    nHomeX: Integer;
    nHomeY: Integer;
    sKingName: string;
    nGold: Integer; // 金币
    wBuilding: Word; // 建筑能力
    wArm: Word; // 军事能力
    wEconomy: Word; // 经济能力
    wPolitics: Word; // 政治能力
    wContribution: Word; // 国家贡献
    btMaps: Byte; // 地图数
  end;

  pTNationInfo = ^TNationInfo;

  TBaseObjectEffect = record
    ActorEffect: TActorEffect;
    nOldLoopCount: Integer;
    dwEffectTick: LongWord;
  end;

  pTBaseObjectEffect = ^TBaseObjectEffect;

  TMapFlag = record
    nTHUNDER: Integer;
    nLAVA: Integer;
    boMISSION: Boolean;
    boNODROPITEM: Boolean;
    boDieTime: Boolean; // 当前地图人物死亡多长时间后自动掉线 piaoyun 2013-09-05
    dwDieTime: LongWord; // 当前地图人物死亡多长时间后自动掉线--掉线时间 piaoyun 2013-09-05
    boNOTHROWITEM: Boolean; // 禁止丢物品 piaoyun 2013-09-05
    boNotStone: Boolean; // 当前地图魔血石、气血石、幻魔石无效 piaoyun 2013-09-05
    boSlaveNotAttackHuman: Boolean; // 当前地图宝宝不攻击人物 chongchong 2013-12-11
    boSlaveNotAttackHero: Boolean; // 当前地图宝宝不攻击英雄 chongchong 2013-12-11

    boSAFE: Boolean;
    boDARK: Boolean;
    boFIGHT: Boolean;
    boFIGHT2: Boolean;
    boFIGHT3: Boolean;
    boFIGHT4: Boolean;
    btFIGHT3Flag: Byte;
    boINCGOLD: Boolean;
    nINCGOLDPOINT: Cardinal;
    nINCGOLDTIME: Word;
    boDECGOLD: Boolean;
    nDECGOLDPOINT: Cardinal;
    nDECGOLDTIME: Word;
    boDAY: Boolean;
    boQUIZ: Boolean;
    boNORECONNECT: Boolean;
    boMUSIC: Boolean;
    boEXPRATE: Boolean;
    boPKWINLEVEL: Boolean;
    boPKWINEXP: Boolean;
    boPKLOSTLEVEL: Boolean;
    boPKLOSTEXP: Boolean;
    boDECHP: Boolean;
    boINCHP: Boolean;
    boDECGAMEGOLD: Boolean;
    boDECGAMEPOINT: Boolean;
    boINCGAMEGOLD: Boolean;
    boINCGAMEPOINT: Boolean;
    boRUNHUMAN: Boolean;
    boRUNMON: Boolean;
    boNoRunHuman: Boolean;
    boNoRunMon: Boolean;
    boNEEDHOLE: Boolean;
    boNORECALL: Boolean;
    boNOGUILDRECALL: Boolean;
    boNODEARRECALL: Boolean;
    boNOMASTERRECALL: Boolean;
    boNORANDOMMOVE: Boolean;
    boNODRUG: Boolean;
    boMINE: Boolean;
    boNOPOSITIONMOVE: Boolean;
    boNoManNoMon: Boolean;
    boNight: Boolean;

    nL: Integer;
    nNEEDSETONFlag: Integer;
    nNeedONOFF: Integer;
    sMusicFileName: string;

    nPKWINLEVEL: Integer;
    nEXPRATE: Integer;
    nPKWINEXP: Integer;
    nPKLOSTLEVEL: Integer;
    nPKLOSTEXP: Integer;
    nDECHPPOINT: Integer;
    nDECHPTIME: Integer;
    nINCHPPOINT: Integer;
    nINCHPTIME: Integer;
    nDECGAMEGOLD: Integer;
    nDECGAMEGOLDTIME: Integer;
    nDECGAMEPOINT: Integer;
    nDECGAMEPOINTTIME: Integer;
    nINCGAMEGOLD: Integer;
    nINCGAMEGOLDTIME: Integer;
    nINCGAMEPOINT: Integer;
    nINCGAMEPOINTTIME: Integer;
    sReConnectMap: string;
    sMUSICName: string;
    boFIGHTPK: Boolean; // PK可以爆装备不红名
    boNOFIREMAGIC: Boolean;
    boUnAllowStdItems: Boolean;
    sUnAllowStdItemsText: string;

    boNOTALLOWUSEMAGIC: Boolean;
    sUnAllowMagicText: string;
    boNORECALLHERO: Boolean;
    boNoCallPet: Boolean;
    boAllowUseMyshop: Boolean;

    boWeatherEffect1: Boolean;
    boWeatherEffect2: Boolean;
    boWeatherEffect3: Boolean;

    boOnKillMob: Boolean;
    nSAYLEVEL: Integer;
    boDELDROPITEM: Boolean;

    nRevivalMaxCount: Integer;
    nRevivalCheckTime: Integer;

    boNODROPUSEITEMS: Boolean;
    boNOSAFEPOSITIONMOVE: Boolean;

    boHITMON: Boolean;
    sHITMON: string;

    boNOHORSE: Boolean; // 禁止骑马 chongchong 2013-10-12
    boNoChallenge: Boolean; // 禁止挑战 chongchong 2014-01-05
    boNoAutoOnline: Boolean; // 禁止自动挂机 chongchong 2014-11-26
    boNoSwitchAttackMode: Boolean; // 禁止切换攻击模式 chongchong 2015-04-29

    boNoDeal: Boolean; // 禁止交易
    boNoShop: Boolean; // 禁止商铺

    boNoHeroProtect: Boolean; // 禁止英雄守护模式

    sDropItemAddUserBag: string;

    SecretFlag: Integer;
    SecretShowName: string;
    SecretDressShap: Word;
    SecretWeaponShap: Word;

    boNoAuction: Boolean;

    boNeedLevelTime: Boolean;
    dwNeedLevelPoint: LongWord;

    boNoAutoDropItemToBag: Boolean; // 禁止物品自动入包
    boNoAutoRangePickItem: Boolean; // 禁止范围拾取

    TimeMapCode: string;
    TimeMapMin: Integer;
    TimeMapShowLeft: Boolean;
    TimeMapLabel: string;
  end;

  pTMapFlag = ^TMapFlag;

  TFoundryItem = record
    sItemName: string;
    nItemCount: Integer;
    nItemRate: Integer;
    ItemList: TList;
  end;

  pTFoundryItem = ^TFoundryItem;

  TFoundryNeedItem = record
    sItemName: string;
    nItemCount: Integer;
    btDelete: Byte;
  end;

  pTFoundryNeedItem = ^TFoundryNeedItem;

  PClientBufInfo = ^TClientBufInfo;

  TClientBufInfo = record
    ClientBufID: Integer;
    Time: Integer;
    StartTick: LongWord;
  end;

  PArrBufInfo = ^TArrBufInfo;

  TArrBufInfo = record
    ClientBufID: Integer;
    Time: Integer;
    StartTick: LongWord;
  end;

  PCustomMagicStatus = ^TCustomMagicStatus;

  TCustomMagicStatus = record
    MagicLevel: Byte;
    MagicID: Word;
    EffectiveTime: Word;
    StartTick: LongWord;
  end;

  PKeyItem = ^TKeyItem;

  TKeyItem = record
    Key: NativeInt;
    Item: Pointer;
  end;

  TProtectHPInfo = record
    IsOpen: Boolean;
    IsPercentage: Boolean;
    dwCheckValue: LongWord;
    dwProtectValue: LongWord;
    dwEffectiveTime: LongWord;
    dwStartTick: LongWord;
  end;

  PWantAddSellPlayerInfo = ^TWantAddSellPlayerInfo;

  TWantAddSellPlayerInfo = record
    IsMyOtherCharDelegator: Boolean; // 委托给我的另外一个角色
    Delegater: string[ACTOR_NAME_LEN];
    SellPricesType: Integer;
    SellPrices: Integer;
    IsSetUser: Boolean;
    SetUserName: string[ACTOR_NAME_LEN];
    IsOtherPlayerConfirm: Boolean; // 另一个角色已同意委托
  end;

  TKeyItemList = class(TObject)
  private
    FList: TList;
    FDuplicates: Boolean;
    FCS: TRTLCriticalSection;
    function GetCapacity: Integer;
    function GetCount: Integer;
    function GetItems(Index: Integer): PKeyItem;
    procedure SetCapacity(const Value: Integer);
    procedure SetCount(const Value: Integer);
  protected
    function Compare(Key1, Key2: NativeInt): NativeInt; virtual;

    function Search(Key: NativeInt; var Index: Integer): Boolean; virtual;
    function IndexOf(Key: NativeInt): Integer; virtual;
  public
    constructor Create(ACapacity: Integer); virtual;
    destructor Destroy; override;

    function GetItemByKey(Key: NativeInt): Pointer;

    procedure Add(Key: NativeInt; Item: Pointer); virtual;
    procedure Delete(Index: Integer);
    procedure Clear; virtual;

    procedure Lock;
    procedure UnLock;

    property Capacity: Integer read GetCapacity write SetCapacity;
    property Count: Integer read GetCount write SetCount;
    property Items[Index: Integer]: PKeyItem read GetItems; default;
    property Duplicates: Boolean read FDuplicates write FDuplicates;
  end;

  // 扩展StringList支持读取带bom的UTF8文件 2020-06-10 18:12:31
  TStringListEx = class(TStringList)
  public
    procedure LoadFromStream(Stream: TStream); override;
  end;

implementation

{ TKeyItemList }

constructor TKeyItemList.Create(ACapacity: Integer);
begin
  InitializeCriticalSection(FCS);
  FList := TList.Create;
  FList.Capacity := ACapacity;
  FDuplicates := False;
end;

destructor TKeyItemList.Destroy;
begin
  Clear;
  FList.Free;
  DeleteCriticalSection(FCS);
  inherited;
end;

procedure TKeyItemList.Add(Key: NativeInt; Item: Pointer);
var
  I: Integer;
  KeyItem: PKeyItem;
begin
  if not Search(Key, I) or Duplicates then
  begin
    New(KeyItem);
    KeyItem.Key := Key;
    KeyItem.Item := Item;
    FList.Insert(I, KeyItem);
  end;
end;

procedure TKeyItemList.Clear;
var
  I: Integer;
  KeyItem: PKeyItem;
begin
  for I := 0 to FList.Count - 1 do
  begin
    KeyItem := FList.Items[I];
    Dispose(KeyItem);
  end;
  FList.Clear;
end;

procedure TKeyItemList.Delete(Index: Integer);
var
  KeyItem: PKeyItem;
begin
  if (Index >= 0) and (Index <= FList.Count - 1) then
  begin
    KeyItem := FList.Items[index];
    Dispose(KeyItem);
    FList.Delete(Index);
  end;
end;

function TKeyItemList.GetCapacity: Integer;
begin
  Result := FList.Capacity;
end;

function TKeyItemList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TKeyItemList.GetItems(Index: Integer): PKeyItem;
begin
  Result := FList[Index];
end;

procedure TKeyItemList.SetCapacity(const Value: Integer);
begin
  FList.Capacity := Value;
end;

procedure TKeyItemList.SetCount(const Value: Integer);
begin
  FList.Count := Value;
end;

function TKeyItemList.Compare(Key1, Key2: NativeInt): NativeInt;
begin
  Result := Key1 - Key2;
end;

function TKeyItemList.IndexOf(Key: NativeInt): Integer;
var
  I: Integer;
begin
  Result := -1;
  if Search(Key, I) then
  begin
    if Duplicates then
      while (I < FList.Count) and (Key <> PKeyItem(FList.Items[I]).Key) do
        Inc(I);
    if I < FList.Count then
      Result := I;
  end;
end;

function TKeyItemList.Search(Key: NativeInt; var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
  Search := False;
  L := 0;
  H := Count - 1;
  while L <= H do
  begin
    I := L + (H - L) shr 1;
    C := Compare(PKeyItem(Items[I]).Key, Key);
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Search := True;
        if not Duplicates then
          L := I;
      end;
    end;
  end;
  Index := L;
end;

procedure TKeyItemList.Lock;
begin
  EnterCriticalSection(FCS);
end;

procedure TKeyItemList.UnLock;
begin
  LeaveCriticalSection(FCS);
end;

function TKeyItemList.GetItemByKey(Key: NativeInt): Pointer;
var
  Index: Integer;
begin
  Result := nil;
  Index := IndexOf(Key);
  if Index >= 0 then
  begin
    Result := PKeyItem(FList.Items[Index]).Item;
  end;
end;

{ TStringListEx }

procedure TStringListEx.LoadFromStream(Stream: TStream);
{$IFNDEF UNICODE}
var
  Size: Integer;
  S: string;
{$ENDIF}
begin
{$IFDEF UNICODE}
  inherited LoadFromStream(Stream);
{$ELSE}
  BeginUpdate;
  try
    Size := Stream.Size - Stream.Position;

    if Size > 3 then
    begin
      SetString(S, nil, 3);
      Stream.Read(Pointer(S)^, 3);

      if S = #$EF#$BB#$BF then
      begin
        SetLength(S, Size - 3);
        Stream.Read(Pointer(S)^, Size - 3);
        S := UTF8Decode(S);
        SetTextStr(S);
      end
      else
      begin
        Stream.Seek(0, soFromBeginning);
        SetString(S, nil, Size);
        Stream.Read(Pointer(S)^, Size);
        SetTextStr(S);
      end;
    end
    else
    begin
      SetString(S, nil, Size);
      Stream.Read(Pointer(S)^, Size);
      SetTextStr(S);
    end;
  finally
    EndUpdate;
  end;
{$ENDIF}
end;

end.
